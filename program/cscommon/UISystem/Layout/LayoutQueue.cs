using System;
using System.Collections.Generic;
using System.Text;

namespace UISystem.Layout
{
    internal sealed partial class LayoutManager
    { 
        internal abstract class LayoutQueue
        {
            // 预创建链表大小
            private const int mRequestParentPoolMaxCount = 153;
            // 预创建
            private const int mPocketReserve = 8;

            private Request mRequestHead;
            private Request mCurrentRequest;
            private int mRequestSize;

            internal abstract Request GetRequest(WinBase ui);
            internal abstract void SetRequest(WinBase ui, Request r);
            internal abstract bool CanRelyOnParentRecalc(WinBase parent);
            internal abstract void Invalidate(WinBase ui);

            internal class Request
            {
                internal WinBase Target;
                internal Request Next;
                internal Request Prev;
            }

            internal LayoutQueue()
            {
                Request r;
                for(int i=0; i< mRequestParentPoolMaxCount; i++)
                {
                    r = new Request();
                    r.Next = mCurrentRequest;
                    mCurrentRequest = r;
                }
                mRequestSize = mRequestParentPoolMaxCount;
            }

            private Request GetNewRequest(WinBase ui)
            {
                Request r;
                if(mCurrentRequest != null)
                {
                    r = mCurrentRequest;
                    mCurrentRequest = r.Next;
                    mRequestSize--;
                    r.Next = r.Prev = null;
                }
                else
                {
                    try
                    {
                        r = new Request();
                    }
                    catch(System.OutOfMemoryException ex)
                    {
                        LayoutManager.Instance.SetForceLayout(ui);
                        throw ex;
                    }
                }

                r.Target = ui;
                return r;
            }

            private void AddRequest(WinBase ui)
            {
                var r = GetNewRequest(ui);
                if(r != null)
                {
                    r.Next = mRequestHead;
                    if (mRequestHead != null)
                        mRequestHead.Prev = r;
                    mRequestHead = r;

                    SetRequest(ui, r);
                }
            }

            internal void Add(WinBase ui)
            {
                lock(this)
                {
                    if (GetRequest(ui) != null)
                        return;

                    RemoveOrphans(ui);

                    if (mRequestSize > mPocketReserve)
                    {
                        AddRequest(ui);
                    }
                    else
                    {
                        while (ui != null)
                        {
                            WinBase p = ui.Parent as WinBase;

                            Invalidate(ui);

                            if (p != null && p.Visibility != Visibility.Collapsed)
                            {
                                Remove(ui);
                            }
                            else
                            {
                                if (p == null)
                                {
                                    if (GetRequest(ui) == null && ui.Visibility != Visibility.Collapsed)
                                    {
                                        RemoveOrphans(ui);
                                        AddRequest(ui);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            ui = p;
                        }
                    }

                    LayoutManager.Instance.NeedsRecalculate();
                }
            }

            internal void Remove(WinBase ui)
            {
                lock(this)
                {
                    Request r = GetRequest(ui);
                    if (r == null)
                        return;
                    RemoveRequest(r);
                    SetRequest(ui, null);
                }
            }

            internal void RemoveOrphans(WinBase parent)
            {
                lock(this)
                {
                    Request r = mRequestHead;
                    while (r != null)
                    {
                        WinBase child = r.Target;
                        Request next = r.Next;
                        ulong parentTreeLevel = parent.TreeLevel;

                        if ((child.TreeLevel == parentTreeLevel + 1)
                            && (child.Parent == parent))
                        {
                            RemoveRequest(GetRequest(child));
                            SetRequest(child, null);
                        }

                        r = next;
                    }
                }
            }

            internal bool IsEmpty
            {
                get { return (mRequestHead == null); }
            }

            internal WinBase GetTopMost()
            {
                lock(this)
                {
                    WinBase found = null;
                    var treeLevel = UInt32.MaxValue;

                    for (Request r = mRequestHead; r != null; r = r.Next)
                    {
                        WinBase t = r.Target;
                        //if (t.Visibility == Visibility.Collapsed)
                        //    continue;

                        var l = t.TreeLevel;

                        if (l < treeLevel)
                        {
                            treeLevel = l;
                            found = r.Target;
                        }
                    }

                    return found;
                }
            }

            private void RemoveRequest(Request req)
            {
                if (req.Prev == null)
                    mRequestHead = req.Next;
                else
                    req.Prev.Next = req.Next;

                if (req.Next != null)
                    req.Next.Prev = req.Prev;

                ReuseRequest(req);
            }

            private void ReuseRequest(Request req)
            {
                req.Target = null;
                if(mRequestSize < mRequestParentPoolMaxCount)
                {
                    req.Next = mCurrentRequest;
                    mCurrentRequest = req;
                    mRequestSize++;
                }
            }

        }

        internal class InternalMeasureQueue : LayoutQueue
        {
            internal override void SetRequest(WinBase ui, Request r)
            {
                ui.MeasureRequest = r;
            }
            internal override Request GetRequest(WinBase ui)
            {
                return ui.MeasureRequest;
            }
            internal override bool CanRelyOnParentRecalc(WinBase parent)
            {
                return !parent.IsMeasureValid && !parent.MeasureInProgress;
            }
            internal override void Invalidate(WinBase ui)
            {
                ui.InvalidateMeasureInternal();
            }
        }

        internal class InternalArrangeQueue : LayoutQueue
        {
            internal override void SetRequest(WinBase ui, Request r)
            {
                ui.ArrangeRequest = r;
            }
            internal override Request GetRequest(WinBase ui)
            {
                return ui.ArrangeRequest;
            }
            internal override bool CanRelyOnParentRecalc(WinBase parent)
            {
                return !parent.IsArrangeValid && !parent.ArrangeInProgress;
            }
            internal override void Invalidate(WinBase ui)
            {
                ui.InvalidateArrangeInternal();
            }
        }
    }
}
