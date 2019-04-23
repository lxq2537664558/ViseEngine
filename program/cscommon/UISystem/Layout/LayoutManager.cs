using System;
using System.Collections.Generic;
using System.Text;

namespace UISystem.Layout
{
    internal sealed partial class LayoutManager
    {
        static LayoutManager smInstance = new LayoutManager();
        public static LayoutManager Instance
        {
            get { return smInstance; }
        }

        internal static int s_LayoutRecursionLimit = 4096;
        private int _arrangesOnStack;
        private int _measuresOnStack;

        private bool _isUpdating;
        private bool _isInUpdateLayout;
        private bool _gotException; // 当UpdateLayout有异常退出时为true

        private bool _layoutRequestPosted;
        //private bool _firePostLayoutEvents;

        private InternalMeasureQueue mMeasureQueue;
        internal LayoutQueue MeasureQueue
        {
            get
            {
                if (mMeasureQueue == null)
                    mMeasureQueue = new InternalMeasureQueue();
                return mMeasureQueue;
            }
        }

        private InternalArrangeQueue mArrangeQueue;
        internal LayoutQueue ArrangeQueue
        {
            get
            {
                if (mArrangeQueue == null)
                    mArrangeQueue = new InternalArrangeQueue();
                return mArrangeQueue;
            }
        }

        private bool HasDirtiness
        {
            get
            {
                return (!MeasureQueue.IsEmpty) || (!ArrangeQueue.IsEmpty);
            }
        }

        // 在极端情况下（内存溢出等），树的强制刷新包含该元素
        private WinBase _forceLayoutElement;
        private WinBase _lastExceptionElement;

        private void SetForceLayout(WinBase ui)
        {
            _forceLayoutElement = ui;
        }

        private LayoutManager()
        {

        }

        // 需要更新布局
        private void NeedsRecalculate()
        {
            if(!_layoutRequestPosted && !_isUpdating)
            {
                _layoutRequestPosted = true;
            }
        }

        internal void UpdateLayout()
        {
            if (_isInUpdateLayout || _measuresOnStack > 0 || _arrangesOnStack > 0)
                return;

            int cnt = 0;
            bool gotException = true;
            WinBase currentElement = null;
            try
            {
                InvalidateTreeIfRecovering();

                while(HasDirtiness)// || _firePostLayoutEvents)
                {
                    if(++cnt > Program.LayoutUpdateDeep)
                    {
                        //todo Layout继续在后台工作，保证前台程序不卡死
                        currentElement = null;
                        gotException = false;
                        return;
                    }

                    _isUpdating = true;
                    _isInUpdateLayout = true;

                    // Measure
                    int loopCounter = 0;
                    DateTime loopStartTime = new DateTime(0);
                    while(true)
                    {
                        if(++loopCounter > Program.LayoutUpdateDeep)
                        {
                            loopCounter = 0;
                            if (loopStartTime.Ticks == 0)
                                loopStartTime = DateTime.UtcNow;
                            else
                            {
                                var loopDuration = (DateTime.UtcNow - loopStartTime);
                                if(loopDuration.Milliseconds > Program.LayoutUpdateDeep * 2)
                                {
                                    //todo Layout继续在后台工作，保证前台程序不卡死
                                    currentElement = null;
                                    gotException = false;
                                    return;
                                }
                            }
                        }

                        currentElement = MeasureQueue.GetTopMost();
                        if (currentElement == null)
                            break;
                        //if(currentElement.Visibility != Visibility.Collapsed)
                        SlimDX.Size measureSize = currentElement.PreviousConstraint;
                        if(currentElement.NeverMeasured)
                        {
                            if (currentElement.Parent == null)
                            {
                                measureSize.Width = float.PositiveInfinity;
                                measureSize.Height = float.PositiveInfinity;
                            }
                            else
                            {
                                measureSize.Width = ((WinBase)currentElement.Parent).Width;
                                measureSize.Height = ((WinBase)currentElement.Parent).Height;
                            }
                        }

                        currentElement.Measure(measureSize);
                    }

                    // Arrange
                    loopCounter = 0;
                    loopStartTime = new DateTime(0);
                    while(MeasureQueue.IsEmpty)
                    {
                        if(++loopCounter > Program.LayoutUpdateDeep)
                        {
                            loopCounter = 0;
                            if (loopStartTime.Ticks == 0)
                                loopStartTime = DateTime.UtcNow;
                            else
                            {
                                var loopDuration = (DateTime.UtcNow - loopStartTime);
                                if(loopDuration.Milliseconds > Program.LayoutUpdateDeep * 2)
                                {
                                    //todo Layout继续在后台工作，保证前台程序不卡死
                                    currentElement = null;
                                    gotException = false;
                                    return;
                                }
                            }
                        }

                        currentElement = ArrangeQueue.GetTopMost();
                        if (currentElement == null)
                            break;
                        //if(currentElement.Visibility != Visibility.Collapsed)
                        {
                            var finalRect = GetProperArrangeRect(currentElement);
                            currentElement.Arrange(finalRect);
                        }
                    }

                    if (!MeasureQueue.IsEmpty)
                        continue;

                    _isInUpdateLayout = false;
                }

                currentElement = null;
                gotException = false;
            }
            finally
            {
                _isUpdating = false;
                _layoutRequestPosted = false;
                _isInUpdateLayout = false;

                if(gotException)
                {
                    _gotException = true;
                    _forceLayoutElement = currentElement;
                    //todo: Layout继续在后台工作，保证前台程序不卡死
                }
            }
        }

        private void InvalidateTreeIfRecovering()
        {
            if((_forceLayoutElement != null) || _gotException)
            {
                if(_forceLayoutElement != null)
                {
                    MarkTreeDirty(_forceLayoutElement);
                }

                _forceLayoutElement = null;
                _gotException = false;
            }
        }

        private void MarkTreeDirty(WinBase ui)
        {
            while(true)
            {
                var p = ui.Parent as WinBase;
                if (p == null)
                    break;
                ui = p;
            }

            MarkTreeDirtyHelper(ui);
            MeasureQueue.Add(ui);
            ArrangeQueue.Add(ui);
        }
        private void MarkTreeDirtyHelper(WinBase ui)
        {
            if (ui != null)
            {
                ui.InvalidateMeasureInternal();
                ui.InvalidateArrangeInternal();
            }
            
            foreach(var child in ui.GetChildWindows())
            {
                MarkTreeDirtyHelper(child);
            }
        }

        private SlimDX.Rect GetProperArrangeRect(WinBase win)
        {
            var arrangeRect = win.PreviousArrangeRect;

            if(win.Parent == null)
            {
                arrangeRect.X = arrangeRect.Y = 0;
                if (float.IsPositiveInfinity(win.PreviousConstraint.Width))
                    arrangeRect.Width = win.DesiredSize.Width;
                if (float.IsPositiveInfinity(win.PreviousConstraint.Height))
                    arrangeRect.Height = win.DesiredSize.Height;
            }
            else if(win.NeverArranged)
            {
                arrangeRect.X = 0;
                arrangeRect.Y = 0;
                arrangeRect.Width = ((WinBase)(win.Parent)).Width;
                arrangeRect.Height = ((WinBase)(win.Parent)).Height;
            }
            return arrangeRect;
        }

        internal void EnterMeasure()
        {
            _lastExceptionElement = null;
            _measuresOnStack++;
            if (_measuresOnStack > s_LayoutRecursionLimit)
                throw new InvalidOperationException("EnterMeasure _measuresOnStack > s_LayoutRecursionLimit");

            //_firePostLayoutEvents = true;
        }

        internal void ExitMeasure()
        {
            _measuresOnStack--;
        }

        internal void EnterArrange()
        {
            _lastExceptionElement = null;
            _arrangesOnStack++;
            if (_arrangesOnStack > s_LayoutRecursionLimit)
                throw new InvalidOperationException("EnterArrange _arrangesOnStack > s_LayoutRecursionLimit");

            //_firePostLayoutEvents = true;
        }
        internal void ExitArrange()
        {
            _arrangesOnStack--;
        }

        internal WinBase GetLastExceptionElement()
        {
            return _lastExceptionElement;
        }

        internal void SetLastExceptionElement(WinBase ui)
        {
            _lastExceptionElement = ui;
        }
    }
}
