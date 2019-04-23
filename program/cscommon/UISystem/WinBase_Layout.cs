using System;
using System.ComponentModel;
using System.Diagnostics;
//using System.Windows.Threading;

namespace UISystem
{
    public partial class WinBase
    {
        private struct MinMax
        {
            internal MinMax(WinBase e)
            {
                maxHeight = e.MaxHeight;
                minHeight = e.MinHeight;
                float l = e.Height;

                //float height = (FloatUtil.IsNaN(l) ? Double.PositiveInfinity : l);
                float height = ((e.Height_Auto || FloatUtil.IsNaN(l)) ? float.PositiveInfinity : l);
                maxHeight = Math.Max(Math.Min(height, maxHeight), minHeight);

                //height = (FloatUtil.IsNaN(l) ? 0 : l);
                height = ((e.Height_Auto || FloatUtil.IsNaN(l)) ? 0 : l);
                minHeight = Math.Max(Math.Min(maxHeight, height), minHeight);

                maxWidth = e.MaxWidth;
                minWidth = e.MinWidth;
                l = e.Width;

                //float width = (FloatUtil.IsNaN(l) ? Double.PositiveInfinity : l);
                float width = ((e.Width_Auto || FloatUtil.IsNaN(l)) ? float.PositiveInfinity : l);
                maxWidth = Math.Max(Math.Min(width, maxWidth), minWidth);

                //width = (FloatUtil.IsNaN(l) ? 0 : l);
                width = ((e.Width_Auto || FloatUtil.IsNaN(l)) ? 0 : l);
                minWidth = Math.Max(Math.Min(maxWidth, width), minWidth);
            }

            internal float minWidth;
            internal float maxWidth;
            internal float minHeight;
            internal float maxHeight;
        }

        #region 布局

        [System.Flags]
        internal enum CoreFlags : uint
        {
            None = 0x00000000,
            SnapsToDevicePixelsCache = 0x00000001,
            ClipToBoundsCache = 0x00000002,
            MeasureDirty = 0x00000004,
            ArrangeDirty = 0x00000008,
            MeasureInProgress = 0x00000010,
            ArrangeInProgress = 0x00000020,
            NeverMeasured = 0x00000040,
            NeverArranged = 0x00000080,
            MeasureDuringArrange = 0x00000100,
            IsCollapsed = 0x00000200,
            IsKeyboardFocusWithinCache = 0x00000400,
            IsKeyboardFocusWithinChanged = 0x00000800,
            IsMouseOverCache = 0x00001000,
            IsMouseOverChanged = 0x00002000,
            IsMouseCaptureWithinCache = 0x00004000,
            IsMouseCaptureWithinChanged = 0x00008000,
            IsStylusOverCache = 0x00010000,
            IsStylusOverChanged = 0x00020000,
            IsStylusCaptureWithinCache = 0x00040000,
            IsStylusCaptureWithinChanged = 0x00080000,
            HasAutomationPeer = 0x00100000,
            RenderingInvalidated = 0x00200000,
            IsVisibleCache = 0x00400000,
            AreTransformsClean = 0x00800000,
            IsOpacitySuppressed = 0x01000000,
            ExistsEventHandlersStore = 0x02000000,
            TouchesOverCache = 0x04000000,
            TouchesOverChanged = 0x08000000,
            TouchesCapturedWithinCache = 0x10000000,
            TouchesCapturedWithinChanged = 0x20000000,
            TouchLeaveCache = 0x40000000,
            TouchEnterCache = 0x80000000,
        }
        private CoreFlags _flags;
        internal bool ReadFlag(CoreFlags field)
        {
            return (_flags & field) != 0;
        }

        internal void WriteFlag(CoreFlags field, bool value)
        {
            if (value)
            {
                _flags |= field;
            }
            else
            {
                _flags &= (~field);
            }
        }
        internal bool HasAutomationPeer
        {
            get { return ReadFlag(CoreFlags.HasAutomationPeer); }
            set { WriteFlag(CoreFlags.HasAutomationPeer, value); }
        }
        private bool RenderingInvalidated
        {
            get { return ReadFlag(CoreFlags.RenderingInvalidated); }
            set { WriteFlag(CoreFlags.RenderingInvalidated, value); }
        }
        internal bool SnapsToDevicePixelsCache
        {
            get { return ReadFlag(CoreFlags.SnapsToDevicePixelsCache); }
            set { WriteFlag(CoreFlags.SnapsToDevicePixelsCache, value); }
        }
        internal bool ClipToBoundsCache
        {
            get { return ReadFlag(CoreFlags.ClipToBoundsCache); }
            set { WriteFlag(CoreFlags.ClipToBoundsCache, value); }
        }
        internal bool MeasureDirty
        {
            get { return ReadFlag(CoreFlags.MeasureDirty); }
            set { WriteFlag(CoreFlags.MeasureDirty, value); }
        }
        internal bool ArrangeDirty
        {
            get { return ReadFlag(CoreFlags.ArrangeDirty); }
            set { WriteFlag(CoreFlags.ArrangeDirty, value); }
        }
        internal bool MeasureInProgress
        {
            get { return ReadFlag(CoreFlags.MeasureInProgress); }
            set { WriteFlag(CoreFlags.MeasureInProgress, value); }
        }
        internal bool ArrangeInProgress
        {
            get { return ReadFlag(CoreFlags.ArrangeInProgress); }
            set { WriteFlag(CoreFlags.ArrangeInProgress, value); }
        }
        internal bool NeverMeasured
        {
            get { return ReadFlag(CoreFlags.NeverMeasured); }
            set { WriteFlag(CoreFlags.NeverMeasured, value); }
        }
        internal bool NeverArranged
        {
            get { return ReadFlag(CoreFlags.NeverArranged); }
            set { WriteFlag(CoreFlags.NeverArranged, value); }
        }
        internal bool MeasureDuringArrange
        {
            get { return ReadFlag(CoreFlags.MeasureDuringArrange); }
            set { WriteFlag(CoreFlags.MeasureDuringArrange, value); }
        }
        internal bool AreTransformsClean
        {
            get { return ReadFlag(CoreFlags.AreTransformsClean); }
            set { WriteFlag(CoreFlags.AreTransformsClean, value); }
        }

        UInt32 mTreeLevel = 0;
        internal UInt32 TreeLevel
        {
            get { return mTreeLevel; }
            set
            {
                mTreeLevel = value;

                foreach(var child in mChildWindows)
                {
                    child.TreeLevel = mTreeLevel + 1;
                }
            }
        }

        //protected SlimDX.Size mDesiredSize = new SlimDX.Size();
        //[Browsable(false)]
        //public SlimDX.Size DesiredSize
        //{
        //    get { return mDesiredSize; }
        //}
        /// <summary> 
        /// Returns the size the element computed during the Measure pass.
        /// This is only valid if IsMeasureValid is true.
        /// </summary>
        private SlimDX.Size _desiredSize;
        [Browsable(false)]
        internal SlimDX.Size DesiredSize
        {
            get
            {
                if (this.Visibility == Visibility.Collapsed)
                    return new SlimDX.Size(0, 0);
                else
                    return _desiredSize;
            }
            set
            {
                _desiredSize = value;
            }
        }

        SlimDX.Rect mCurFinalRect;
        [Browsable(false)]
        internal SlimDX.Rect CurFinalRect
        {
            get { return mCurFinalRect; }
            set { mCurFinalRect = value; }
        }

        internal SlimDX.Rect PreviousArrangeRect
        {
            get { return mCurFinalRect; }
        }

        // Dirty的才需要布局操作
        //internal bool LayoutDirty = true;

        [Browsable(false)]
        public int TotalWidth
        {
            get { return (int)(Width + Margin.Left + Margin.Right); }
        }
        [Browsable(false)]
        public int TotalHeight
        {
            get { return (int)(Height + Margin.Top + Margin.Bottom); }
        }

        public delegate void Delegate_OnUpdateLayout();
        public Delegate_OnUpdateLayout OnUpdateLayout;

        internal Layout.LayoutManager.LayoutQueue.Request MeasureRequest;
        internal Layout.LayoutManager.LayoutQueue.Request ArrangeRequest;

        SlimDX.Size mClippedSizeBox = SlimDX.Size.Empty;
        [Browsable(false)]
        public CSUtility.Support.Size RenderSize
        {
            get
            {
                if (this.Visibility == Visibility.Collapsed)
                    return new CSUtility.Support.Size();
                else
                    return mSize;
            }
        }

        /// <summary> 
        /// Determines if the DesiredSize is valid.
        /// </summary> 
        /// <remarks> 
        /// A developer can force arrangement to be invalidated by calling InvalidateMeasure.
        /// IsArrangeValid and IsMeasureValid are related, 
        /// in that arrangement cannot be valid without measurement first being valid.
        /// </remarks>
        [Browsable(false)]
        public bool IsMeasureValid
        {
            get { return !MeasureDirty; }
        }

        /// <summary>
        /// Determines if the RenderSize and position of child elements is valid. 
        /// </summary>
        /// <remarks>
        /// A developer can force arrangement to be invalidated by calling InvalidateArrange.
        /// IsArrangeValid and IsMeasureValid are related, in that arrangement cannot be valid without measurement first 
        /// being valid.
        /// </remarks> 
        [Browsable(false)]
        public bool IsArrangeValid
        {
            get { return !ArrangeDirty; }
        }

        internal void InvalidateMeasureInternal()
        {
            MeasureDirty = true;
        }

        internal void InvalidateArrangeInternal()
        {
            ArrangeDirty = true;
        }

        /// <summary>
        /// This event fires every time Layout updates the layout of the trees associated with current Dispatcher.
        /// Layout update can happen as a result of some propety change, window resize or explicit user request.
        /// </summary> 
        public event EventHandler LayoutUpdated
        {
            add
            {
            //    LayoutEventList.ListItem item = getLayoutUpdatedHandler(value);

            //    if (item == null)
            //    {
            //        //set a weak ref in LM 
            //        item = ContextLayoutManager.From(Dispatcher).LayoutEvents.Add(value);
            //        addLayoutUpdatedHandler(value, item);
            //    }
            }
            remove
            {
            //    LayoutEventList.ListItem item = getLayoutUpdatedHandler(value);

            //    if (item != null)
            //    {
            //        removeLayoutUpdatedHandler(value);
            //        //remove a weak ref from LM 
            //        ContextLayoutManager.From(Dispatcher).LayoutEvents.Remove(item);
            //    }
            }
        }

        // 刷新布局
        public void UpdateLayout(bool bForce = false)
        {            
            if (!LoadFinished)
                return;

            InvalidateMeasure(bForce);
            //InvalidateArrange();

            //var winRoot = this;
            //while (winRoot.Parent != null)
            //{
            //    winRoot.InvalidateMeasure();
            //    winRoot.InvalidateArrange();

            //    winRoot = winRoot.Parent as WinBase;
            //}
        }

        public void InvalidateMeasure(bool bForce = false)
        {
            if ((!MeasureDirty && !MeasureInProgress) || bForce)
            {
                //Debug.Assert(MeasureRequest == null, "can't be clean and still have MeasureRequest");

                //if (!NeverMeasured) //only measured once elements are allowed in *update* queue 
                {
                    //ContextLayoutManager ContextLayoutManager = ContextLayoutManager.From(Dispatcher);
                    //if (EventTrace.IsEnabled(EventTrace.Keyword.KeywordLayout, EventTrace.Level.Verbose))
                    //{
                    //    // Knowing when the layout queue goes from clean to dirty is interesting.
                    //    if (ContextLayoutManager.MeasureQueue.IsEmpty)
                    //    {
                    //        EventTrace.EventProvider.TraceEvent(EventTrace.Event.WClientLayoutInvalidated, EventTrace.Keyword.KeywordLayout, EventTrace.Level.Verbose, PerfService.GetPerfElementID(this));
                    //    }
                    //}
                    Layout.LayoutManager.Instance.MeasureQueue.Add(this);
                }
                MeasureDirty = true;
            }
        }

        //protected bool mMeasured = false;
        //protected void SetMeasured(bool value)
        //{
        //    mMeasured = value;

        //    foreach (var child in ChildWindows)
        //    {
        //        child.SetMeasured(value);
        //    }
        //}

        /// <summary>
        /// Invalidates the rendering of the element. 
        /// Causes <see cref="System.Windows.UIElement.OnRender"/> to be called at a later time.
        /// </summary>
        public void InvalidateVisual()
        {
            InvalidateArrange();
            RenderingInvalidated = true;
        }

        /// <summary> 
        /// Invalidates the arrange state for the element.
        /// The element will be queued for an update layout that will occur asynchronously.
        /// MeasureCore will not be called unless InvalidateMeasure is also called - or that something
        /// else caused the measure state to be invalidated. 
        /// </summary>
        public void InvalidateArrange()
        {
            if (!ArrangeDirty
               && !ArrangeInProgress)
            {
                //Debug.Assert(ArrangeRequest == null, "can't be clean and still have MeasureRequest");

                //                 VerifyAccess(); 

                //if (!NeverArranged)
                {
                    //ContextLayoutManager ContextLayoutManager = ContextLayoutManager.From(Dispatcher);
                    //ContextLayoutManager.ArrangeQueue.Add(this);
                    Layout.LayoutManager.Instance.ArrangeQueue.Add(this);
                }


                ArrangeDirty = true;
            }
        } 

        SlimDX.Size _previousAvailableSize = SlimDX.Size.Empty;
        SlimDX.Size _preDesiredSize = SlimDX.Size.Empty;

        //private MeasureData mMeasuredData;
        //internal MeasureData MeasureData
        //{
        //    get { return mMeasuredData; }
        //    set
        //    {
        //        mMeasuredData = value;
        //    }
        //}

        internal SlimDX.Size PreviousConstraint
        {
            get
            {
                return _previousAvailableSize;
            }
        }

        //private MeasureData mPreviousMeasureData;
        //internal MeasureData PreviousMeasureData
        //{
        //    get { return mPreviousMeasureData; }
        //    private set
        //    {
        //        mPreviousMeasureData = value;

        //        Debug.Assert(_previousAvailableSize == value.AvailableSize);
        //    }
        //}

        private void switchVisibilityIfNeeded(Visibility visibility)
        {
            switch (visibility)
            {
                case Visibility.Visible:
                    ensureVisible();
                    break;

                case Visibility.Hidden:
                    ensureInvisible(false);
                    break;

                case Visibility.Collapsed:
                    ensureInvisible(true);
                    break;
            }
        }

        private void ensureVisible()
        {
            if (ReadFlag(CoreFlags.IsOpacitySuppressed))
            {
                //todo: restore Opacity

                if (ReadFlag(CoreFlags.IsCollapsed))
                {
                    WriteFlag(CoreFlags.IsCollapsed, false);

                    //invalidate parent if needed 
                    signalDesiredSizeChange();

                    //we are suppressing rendering (see IsRenderable) of collapsed children (to avoid 
                    //confusion when they see RenderSize=(0,0) reported for them)
                    //so now we should invalidate to re-render if some rendering props 
                    //changed while UIElement was Collapsed (Arrange will cause re-rendering)
                    InvalidateVisual();
                }
            }
        }

        private void ensureInvisible(bool collapsed)
        {
            if (!ReadFlag(CoreFlags.IsOpacitySuppressed))
            {
                //base.VisualOpacity = 0;
                WriteFlag(CoreFlags.IsOpacitySuppressed, true);
            }

            if (!ReadFlag(CoreFlags.IsCollapsed) && collapsed) //Hidden or Visible->Collapsed
            {
                WriteFlag(CoreFlags.IsCollapsed, true);

                //invalidate parent
                signalDesiredSizeChange();
            }
            else if (ReadFlag(CoreFlags.IsCollapsed) && !collapsed) //Collapsed -> Hidden 
            {
                WriteFlag(CoreFlags.IsCollapsed, false);

                //invalidate parent
                signalDesiredSizeChange();
            }
        }

        protected virtual void OnChildDesiredSizeChanged(WinBase child)
        {
            if (IsMeasureValid)
            {
                InvalidateMeasure();
            }
        }

        private void signalDesiredSizeChange()
        {
            //WinBase p;
            //IContentHost ich;

            //GetUIParentOrICH(out p, out ich); //only one will be returned

            //if (p != null)
            //    p.OnChildDesiredSizeChanged(this);
            //else if (ich != null)
            //    ich.OnChildDesiredSizeChanged(this);
        }

        //bool mMeasureInProcess = false;
        //public bool MeasureInProcess
        //{
        //    get { return mMeasureInProcess; }
        //    protected set
        //    {
        //        mMeasureInProcess = value;
        //    }
        //}
        // 测量
        /// <summary>
        /// Updates DesiredSize of the UIElement. Must be called by parents from theor MeasureCore, to form recursive update. 
        /// This is first pass of layout update.
        /// </summary>
        /// <remarks>
        /// Measure is called by parents on their children. Internally, Measure calls MeasureCore override on the same object, 
        /// giving it opportunity to compute its DesiredSize.<para/>
        /// This method will return immediately if child is not Dirty, previously measured 
        /// and availableSize is the same as cached. <para/> 
        /// This method also resets the IsMeasureinvalid bit on the child.<para/>
        /// In case when "unbounded measure to content" is needed, parent can use availableSize 
        /// as float.PositiveInfinity. Any returned size is OK in this case.
        /// </remarks>
        /// <param name="availableSize">Available size that parent can give to the child. May be infinity (when parent wants to
        /// measure to content). This is soft constraint. Child can return bigger size to indicate that it wants bigger space and hope 
        /// that parent can throw in scrolling...</param>
        public void Measure(SlimDX.Size availableSize)
        {
            //if (!LayoutDirty)
            //{
            //    if(FloatUtil.AreClose(availableSize, _previousAvailableSize))
            //        return;
            //}

            //LayoutDirty = true;
            
            try
            {
                // Disable reentrancy during the measure pass.  This is because much work is done
                // during measure - such as inflating templates, formatting PTS stuff, creating
                // fonts, etc.  Generally speaking, we cannot survive reentrancy in these code
                // paths. 
                //using (Dispatcher.DisableProcessing())
                {
                    if (FloatUtil.IsNaN(availableSize.Width) || FloatUtil.IsNaN(availableSize.Height))
                        throw new InvalidOperationException("Measure exception availableSize is NaN");

                    bool neverMeasured = NeverMeasured;

                    if (neverMeasured)
                    {
                        switchVisibilityIfNeeded(this.Visibility);
                    }

                    bool isCloseToPreviousMeasure = FloatUtil.AreClose(availableSize, _previousAvailableSize);

                    //if Collapsed, we should not Measure, keep dirty bit but remove request 
                    if (this.Visibility == Visibility.Collapsed)
                        //|| ((Visual)this).CheckFlagsAnd(VisualFlags.IsLayoutSuspended))
                    {
                        // reset measure request.
                        if (MeasureRequest != null)
                        {
                            Layout.LayoutManager.Instance.MeasureQueue.Remove(this);
                        }

                        //  remember though that parent tried to measure at this size 
                        //  in case when later this element is called to measure incrementally 
                        //  it has up-to-date information stored in _previousAvailableSize
                        if (!isCloseToPreviousMeasure)
                        {
                            //this will ensure that element will be actually re-measured at the new available size
                            //later when it becomes visible.
                            InvalidateMeasureInternal();

                            _previousAvailableSize = availableSize;
                        }
                        //else
                        //    MeasureDirty = false;

                        return;
                    }

                    //your basic bypass. No reason to calc the same thing.
                    if (IsMeasureValid                       //element is clean 
                        && !neverMeasured                       //previously measured
                        && isCloseToPreviousMeasure) //and contraint matches
                    {
                        return;
                    }

                    NeverMeasured = false;
                    var prevSize = _desiredSize;

                    //we always want to be arranged, ensure arrange request
                    //doing it before OnMeasure prevents unneeded requests from children in the queue
                    InvalidateArrange();
                    //_measureInProgress prevents OnChildDesiredSizeChange to cause the elements be put 
                    //into the queue.

                    MeasureInProgress = true;

                    var desiredSize = new SlimDX.Size(0, 0);
                    
                    bool gotException = true;

                    try
                    {
                        Layout.LayoutManager.Instance.EnterMeasure();
                        desiredSize = MeasureCore(availableSize);

                        gotException = false;
                    }
                    finally
                    {
                        // reset measure in progress
                        MeasureInProgress = false;

                        _previousAvailableSize = availableSize;

                        Layout.LayoutManager.Instance.ExitMeasure();

                        if (gotException)
                        {
                            // we don't want to reset last exception element on layoutManager if it's been already set.
                            if (Layout.LayoutManager.Instance.GetLastExceptionElement() == null)
                            {
                                Layout.LayoutManager.Instance.SetLastExceptionElement(this);
                            }
                        }
                    }

                    //enforce that MeasureCore can not return PositiveInfinity size even if given Infinte availabel size.
                    //Note: NegativeInfinity can not be returned by definition of Size structure.
                    if (float.IsPositiveInfinity(desiredSize.Width) || float.IsPositiveInfinity(desiredSize.Height))
                        throw new InvalidOperationException("Measure Exception: desiredSize IsPositiveInfinity");

                    //enforce that MeasureCore can not return NaN size . 
                    if (FloatUtil.IsNaN(desiredSize.Width) || FloatUtil.IsNaN(desiredSize.Height))
                        throw new InvalidOperationException("Measure Exception: desiredSize IsNaN");

                    //reset measure dirtiness

                    MeasureDirty = false;
                    //reset measure request.
                    if (MeasureRequest != null)
                        Layout.LayoutManager.Instance.MeasureQueue.Remove(this);

                    //cache desired size 
                    _desiredSize = desiredSize;

                    //notify parent if our desired size changed (watefall effect)
                    if (!MeasureDuringArrange
                       && !FloatUtil.AreClose(prevSize, desiredSize))
                    {
                        WinBase p = this.Parent as WinBase;
                        if (p != null && !p.MeasureInProgress) //this is what differs this code from signalDesiredSizeChange()
                            p.OnChildDesiredSizeChanged(this);
                    }
                }                
            }
            catch(System.Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(this.GetType().ToString() + " Measure Exception: \r\n" + e.ToString());
                System.Diagnostics.Debug.WriteLine(this.GetType().ToString() + " Measure Exception: \r\n" + e.ToString());
            }
            finally
            {
                //if (etwTracingEnabled == true)
                //{
                //    EventTrace.EventProvider.TraceEvent(EventTrace.Event.WClientMeasureElementEnd, EventTrace.Keyword.KeywordLayout, EventTrace.Level.Verbose, perfElementID, _desiredSize.Width, _desiredSize.Height);
                //}
            }
        }

        public void ApplyTemplate()
        {

        }

        protected virtual SlimDX.Size MeasureCore(SlimDX.Size availableSize)
        {
            var margin = Margin;
            var marginWidth = margin.Left + margin.Right;
            var marginHeight = margin.Top + margin.Bottom;

            //  parent size is what parent want us to be
            var frameworkAvailableSize = new SlimDX.Size(
                        Math.Max(availableSize.Width - marginWidth, 0),
                        Math.Max(availableSize.Height - marginHeight, 0));

            MinMax mm = new MinMax(this);

            // layoutTransform处理

            frameworkAvailableSize.Width = (float)Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = (float)Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));

            var desiredSize = MeasureOverride(frameworkAvailableSize);

            //  maximize desiredSize with user provided min size
            desiredSize = new SlimDX.Size(
                Math.Max(desiredSize.Width, mm.minWidth),
                Math.Max(desiredSize.Height, mm.minHeight));

            //here is the "true minimum" desired size - the one that is
            //for sure enough for the control to render its content.
            var unclippedDesiredSize = desiredSize;

            bool clipped = false;

            // User-specified max size starts to "clip" the control here. 
            //Starting from this point desiredSize could be smaller then actually
            //needed to render the whole control
            if (desiredSize.Width > mm.maxWidth)
            {
                desiredSize.Width = mm.maxWidth;
                clipped = true;
            }

            if (desiredSize.Height > mm.maxHeight)
            {
                desiredSize.Height = mm.maxHeight;
                clipped = true;
            }

            //  because of negative margins, clipped desired size may be negative.
            //  need to keep it as doubles for that reason and maximize with 0 at the 
            //  very last point - before returning desired size to the parent. 
            double clippedDesiredWidth = desiredSize.Width + marginWidth;
            double clippedDesiredHeight = desiredSize.Height + marginHeight;

            // In overconstrained scenario, parent wins and measured size of the child,
            // including any sizes set or computed, can not be larger then
            // available size. We will clip the guy later. 
            if (clippedDesiredWidth > availableSize.Width)
            {
                clippedDesiredWidth = availableSize.Width;
                clipped = true;
            }

            if (clippedDesiredHeight > availableSize.Height)
            {
                clippedDesiredHeight = availableSize.Height;
                clipped = true;
            }

            if (clipped
               || clippedDesiredWidth < 0
               || clippedDesiredHeight < 0)
            {
                if (mClippedSizeBox == SlimDX.Size.Empty)
                {
                    mClippedSizeBox = new SlimDX.Size(unclippedDesiredSize.Width, unclippedDesiredSize.Height);
                }
                else
                {
                    mClippedSizeBox.Width = unclippedDesiredSize.Width;
                    mClippedSizeBox.Height = unclippedDesiredSize.Height;
                }
            }
            else
            {
                if (mClippedSizeBox != SlimDX.Size.Empty)
                    mClippedSizeBox = SlimDX.Size.Empty;
            }

            return new SlimDX.Size(Math.Max(0, clippedDesiredWidth), Math.Max(0, clippedDesiredHeight));
        }

        protected virtual SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            SlimDX.Size returnDesiredSize = new SlimDX.Size();

            //foreach (var child in ChildWindows)
            for (int i = 0; i < ChildWindows.Count; i++ )
            {
                WinBase child = null;
                try
                {
                    child = ChildWindows[i];
                }
                catch (System.Exception)
                {
                    continue;
                }
                
                child.Measure(availableSize);
                returnDesiredSize.Width = System.Math.Max(returnDesiredSize.Width, child.DesiredSize.Width);
                returnDesiredSize.Height = System.Math.Max(returnDesiredSize.Height, child.DesiredSize.Height);
            }

            if (!Width_Auto)
            {
                returnDesiredSize.Width = System.Math.Max(Width,0);
            }
            //else
            //{
            //    switch (HorizontalAlignment)
            //    {
            //        case UI.HorizontalAlignment.Center:
            //        case UI.HorizontalAlignment.Left:
            //        case UI.HorizontalAlignment.Right:
            //            break;
            //        case UI.HorizontalAlignment.Stretch:
            //            {
            //                if(availableSize.Width < int.MaxValue)
            //                    returnDesiredSize.Width = System.Math.Max(returnDesiredSize.Width, availableSize.Width);
            //            }
            //            break;
            //    }
            //}
            if (!Height_Auto)
            {
                returnDesiredSize.Height = System.Math.Max(Height,0);
            }
            //else
            //{
            //    switch (VerticalAlignment)
            //    {
            //        case UI.VerticalAlignment.Center:
            //        case UI.VerticalAlignment.Top:
            //        case UI.VerticalAlignment.Bottom:
            //            break;
            //        case UI.VerticalAlignment.Stretch:
            //            {
            //                if (availableSize.Height < int.MaxValue)
            //                    returnDesiredSize.Height = System.Math.Max(returnDesiredSize.Height, availableSize.Height);
            //            }
            //            break;
            //    }
            //}

            return returnDesiredSize;
        }

        /// <summary> 
        /// Parents or system call this method to arrange the internals of children on a second pass of layout update.
        /// </summary> 
        /// <remarks> 
        /// This method internally calls ArrangeCore override, giving the derived class opportunity
        /// to arrange its children and/or content using final computed size. 
        /// In their ArrangeCore overrides, derived class is supposed to create its visual structure and
        /// prepare itself for rendering. Arrange is called by parents
        /// from their implementation of ArrangeCore or by system when needed.
        /// This method sets Bounds=finalSize before calling ArrangeCore. 
        /// </remarks>
        /// <param name="finalRect">This is the final size and location that parent or system wants this UIElement to assume.</param> 
        public void Arrange(SlimDX.Rect finalRect)
        {
            //if (!LayoutDirty)
            //{
            //    if(FloatUtil.AreClose(finalRect, CurFinalRect))
            //        return;
            //}
            
            //ContextLayoutManager ContextLayoutManager = ContextLayoutManager.From(Dispatcher);
            //if (ContextLayoutManager.AutomationEvents.Count != 0)
            //    UIElementHelper.InvalidateAutomationAncestors(this); 

            try
            {
                // Disable reentrancy during the arrange pass.  This is because much work is done
                // during arrange - such as formatting PTS stuff, creating
                // fonts, etc.  Generally speaking, we cannot survive reentrancy in these code 
                // paths.
                //using (Dispatcher.DisableProcessing())
                {
                    //enforce that Arrange can not come with Infinity size or NaN
                    if (float.IsPositiveInfinity(finalRect.Width)
                        || float.IsPositiveInfinity(finalRect.Height)
                        || FloatUtil.IsNaN(finalRect.Width)
                        || FloatUtil.IsNaN(finalRect.Height)
                      )
                    {
                        throw new InvalidOperationException("Arrange Exception: finalRect illegal!");
                    }

                    //if Collapsed, we should not Arrange, keep dirty bit but remove request 
                    if (this.Visibility == Visibility.Collapsed)
                        //|| ((Visual)this).CheckFlagsAnd(VisualFlags.IsLayoutSuspended))
                    {
                        //reset arrange request.
                        if (ArrangeRequest != null)
                            Layout.LayoutManager.Instance.ArrangeQueue.Remove(this);

                        //  remember though that parent tried to arrange at this rect
                        //  in case when later this element is called to arrange incrementally 
                        //  it has up-to-date information stored in _finalRect 
                        CurFinalRect = finalRect;

                        //ArrangeDirty = false;

                        return;
                    }

                    //in case parent did not call Measure on a child, we call it now. 
                    //parent can skip calling Measure on a child if it does not care about child's size
                    //passing finalSize practically means "set size" because that's what Measure(sz)/Arrange(same_sz) means 
                    //Note that in case of IsLayoutSuspended (temporarily out of the tree) the MeasureDirty can be true 
                    //while it does not indicate that we should re-measure - we just came of Measure that did nothing
                    //because of suspension 
                    if (MeasureDirty
                       || NeverMeasured)
                    {
                        try
                        {
                            MeasureDuringArrange = true;
                            //If never measured - that means "set size", arrange-only scenario 
                            //Otherwise - the parent previosuly measured the element at constriant
                            //and the fact that we are arranging the measure-dirty element now means 
                            //we are not in the UpdateLayout loop but rather in manual sequence of Measure/Arrange
                            //(like in HwndSource when new RootVisual is attached) so there are no loops and there could be
                            //measure-dirty elements left after previosu single Measure pass) - so need to use cached constraint
                            if (NeverMeasured)
                                Measure(finalRect.Size);
                            else
                            {
                                Measure(PreviousConstraint);
                            }
                        }
                        finally
                        {
                            MeasureDuringArrange = false;
                        }
                    }

                    //bypass - if clean and rect is the same, no need to re-arrange 
                    //if ((!IsArrangeValid || NeverArranged)
                    //    && (CurFinalRect.IsEmpty || !FloatUtil.AreClose(finalRect, CurFinalRect)))
                    if (!IsArrangeValid 
                        || NeverArranged
                        || !FloatUtil.AreClose(finalRect, CurFinalRect))
                    {
                        bool firstArrange = NeverArranged;
                        NeverArranged = false;
                        ArrangeInProgress = true;

                        var oldSize = RenderSize;
                        bool gotException = true;

                        try
                        {
                            Layout.LayoutManager.Instance.EnterArrange();

                            //This has to update RenderSize
                            ArrangeCore(finalRect);

                            gotException = false;
                        }
                        finally
                        {
                            ArrangeInProgress = false;
                            Layout.LayoutManager.Instance.ExitArrange();

                            if (gotException)
                            {
                                // we don't want to reset last exception element on layoutManager if it's been already set.
                                if (Layout.LayoutManager.Instance.GetLastExceptionElement() == null)
                                {
                                    Layout.LayoutManager.Instance.SetLastExceptionElement(this);
                                }
                            }
                        }

                        CurFinalRect = finalRect;

                        ArrangeDirty = false;

                        //reset request. 
                        if (ArrangeRequest != null)
                            Layout.LayoutManager.Instance.ArrangeQueue.Remove(this); 
                    }
                }
            }
            catch (System.Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(this.GetType().ToString() + " Measure Exception: \r\n" + e.ToString());
                System.Diagnostics.Debug.WriteLine(this.GetType().ToString() + " Measure Exception: \r\n" + e.ToString());
            }
            finally
            {
                //if (etwTracingEnabled == true)
                //{
                //    EventTrace.EventProvider.TraceEvent(EventTrace.Event.WClientArrangeElementEnd, EventTrace.Keyword.KeywordLayout, EventTrace.Level.Verbose, perfElementID, finalRect.Top, finalRect.Left, finalRect.Width, finalRect.Height);
                //}
            }
        }
        //// 排列
        //private bool mArrangeInProcess = false;
        //protected bool ArrangeInProcess
        //{
        //    get { return mArrangeInProcess; }
        //}
        //public void Arrange(SlimDX.Rect finalRect)
        //{
        //    if (finalRect == null)
        //        return;

        //    if (finalRect.X == int.MinValue || finalRect.X == int.MaxValue ||
        //       finalRect.Y == int.MinValue || finalRect.Y == int.MaxValue ||
        //       finalRect.Width < 0 || finalRect.Width == int.MaxValue ||
        //       finalRect.Height < 0 || finalRect.Height == int.MaxValue)
        //        return;
        //    //MeasureData previousMeasureData = PreviousMeasureData;

        //    //ContextLayoutManager ContextLayoutManager = ContextLayoutManager.From(System.Windows.Threading.Dispatcher);
        //    //if (ContextLayoutManager.AutomationEvents.Count != 0)
        //    //{

        //    //}

        //    //if(EventTrace)

        //    mArrangeInProcess = true;

        //    if (!mMeasured)
        //    {
        //        Measure(finalRect.Size);
        //        //Measure(new CSUtility.Support.Size(finalRect.Width, finalRect.Height));
        //    }

        //    switch (Visibility)
        //    {
        //        case Visibility.Visible:
        //        case Visibility.Hidden:
        //            {
        //                int marginWidth = (int)(Margin.Left + Margin.Right);
        //                int marginHeight = (int)(Margin.Top + Margin.Bottom);

        //                SlimDX.Size arrangeSize = new SlimDX.Size(
        //                                        finalRect.Width - marginWidth,
        //                                        finalRect.Height - marginHeight);

        //                if (HorizontalAlignment != UI.HorizontalAlignment.Stretch)
        //                {
        //                    arrangeSize.Width = DesiredSize.Width;
        //                }
        //                if (VerticalAlignment != UI.VerticalAlignment.Stretch)
        //                {
        //                    arrangeSize.Height = DesiredSize.Height;
        //                }

        //                if (Width_Auto)
        //                {
        //                    arrangeSize.Width = System.Math.Max(MinWidth, System.Math.Min(MaxWidth, arrangeSize.Width));
        //                }
        //                else
        //                {
        //                    var tempWidth = System.Math.Max(MinWidth, System.Math.Min(MaxWidth, Width));
        //                    arrangeSize.Width = tempWidth;
        //                }

        //                if (Height_Auto)
        //                {
        //                    arrangeSize.Height = System.Math.Max(MinHeight, System.Math.Min(MaxHeight, arrangeSize.Height));
        //                }
        //                else
        //                {
        //                    var tempHeight = System.Math.Max(MinHeight, System.Math.Min(MaxHeight, Height));
        //                    arrangeSize.Height = tempHeight;
        //                }
                        
        //                var innerInkSize = ArrangeOverride(arrangeSize);

        //                var clippedInkSize = new SlimDX.Size(Math.Min(innerInkSize.Width, MaxWidth),
        //                                                             Math.Min(innerInkSize.Height, MaxHeight));

        //                //var renderSize = innerInkSize;
        //                //var clippedInkSize = new CSUtility.Support.Size();
        //                //if (Width_Auto)
        //                //{
        //                //    innerInkSize.Width = System.Math.Max(MinWidth, System.Math.Min(innerInkSize.Width, MaxWidth));
        //                //}
        //                //else
        //                //{
        //                //    var tempWidth = System.Math.Max(MinWidth, System.Math.Min(Width, MaxWidth));
        //                //    clippedInkSize.Width = System.Math.Min(tempWidth, innerInkSize.Width);
        //                //}
        //                SlimDX.Size clientSize = new SlimDX.Size(
        //                                                        System.Math.Max(0, finalRect.Width - marginWidth),
        //                                                        System.Math.Max(0, finalRect.Height - marginHeight));

        //                var offset = ComputeAlignmentOffset(clientSize, clippedInkSize);
        //                Left = (int)(offset.X + finalRect.X + Margin.Left);
        //                Top = (int)(offset.Y + finalRect.Y + Margin.Top);
        //                if (Width_Auto)
        //                    Width = innerInkSize.Width;
        //                if (Height_Auto)
        //                    Height = innerInkSize.Height;

        //                //var widthToSet = System.Math.Min(DesiredSize.Width, finalRect.Width) - Margin.Left - Margin.Right;
        //                //var heightToSet = System.Math.Min(DesiredSize.Height, finalRect.Height) - Margin.Top - Margin.Bottom;

        //                //switch (HorizontalAlignment)
        //                //{
        //                //    case UI.HorizontalAlignment.Left:
        //                //        {
        //                //            if (Width_Auto)
        //                //                Width = (int)System.Math.Max(System.Math.Min(widthToSet, MaxWidth), MinWidth);
        //                //            Left = (int)(finalRect.X + Margin.Left);
        //                //        }
        //                //        break;
        //                //    case UI.HorizontalAlignment.Right:
        //                //        {
        //                //            if (Width_Auto)
        //                //                Width = (int)System.Math.Max(System.Math.Min(widthToSet, MaxWidth), MinWidth);
        //                //            Left = finalRect.X + (int)(finalRect.Width - Margin.Right - Width);
        //                //        }
        //                //        break;
        //                //    case UI.HorizontalAlignment.Center:
        //                //        {
        //                //            if (Width_Auto)
        //                //                Width = (int)System.Math.Max(System.Math.Min(widthToSet, MaxWidth), MinWidth);
        //                //            Left = finalRect.X + (int)(Margin.Left - Margin.Right + finalRect.Width / 2) - Width / 2;
        //                //        }
        //                //        break;
        //                //    case UI.HorizontalAlignment.Stretch:
        //                //        {
        //                //            if (Width_Auto)
        //                //            {
        //                //                widthToSet = finalRect.Width - Margin.Left - Margin.Right;
        //                //                Width = (int)System.Math.Max(System.Math.Min(widthToSet, MaxWidth), MinWidth);
        //                //            }
        //                //            var delta = Margin.Left;
        //                //            var widthlet = 1;
        //                //            if((Margin.Left + Margin.Right) != 0)
        //                //            {
        //                //                delta = (float)Margin.Left / (Margin.Left + Margin.Right);
        //                //                widthlet = finalRect.Width - Width;
        //                //            }
        //                //            Left = finalRect.X + (int)(delta * widthlet);
        //                //            //Left = (int)(finalRect.X + Margin.Left);
        //                //        }
        //                //        break;
        //                //}

        //                //switch (VerticalAlignment)
        //                //{
        //                //    case UI.VerticalAlignment.Top:
        //                //        {
        //                //            if (Height_Auto)
        //                //                Height = (int)System.Math.Max(System.Math.Min(heightToSet, MaxHeight), MinHeight);
        //                //            Top = (int)(finalRect.Y + Margin.Top);
        //                //        }
        //                //        break;
        //                //    case UI.VerticalAlignment.Bottom:
        //                //        {
        //                //            if (Height_Auto)
        //                //                Height = (int)System.Math.Max(System.Math.Min(heightToSet, MaxHeight), MinHeight);
        //                //            Top = finalRect.Y + (int)(finalRect.Height - Margin.Bottom - Height);
        //                //        }
        //                //        break;
        //                //    case UI.VerticalAlignment.Center:
        //                //        {
        //                //            if (Height_Auto)
        //                //                Height = (int)System.Math.Max(System.Math.Min(heightToSet, MaxHeight), MinHeight);
        //                //            Top = finalRect.Y + (int)(Margin.Top - Margin.Bottom + finalRect.Height / 2) - Height / 2;
        //                //        }
        //                //        break;
        //                //    case UI.VerticalAlignment.Stretch:
        //                //        {
        //                //            if (Height_Auto)
        //                //            {
        //                //                heightToSet = (int)(finalRect.Height - Margin.Top - Margin.Bottom);
        //                //                Height = (int)System.Math.Max(System.Math.Min(heightToSet, MaxHeight), MinHeight);
        //                //            }
        //                //            var delta = Margin.Top;
        //                //            var heightlet = 1;
        //                //            if ((Margin.Top + Margin.Bottom) != 0)
        //                //            {
        //                //                delta = (float)Margin.Top / (Margin.Top + Margin.Bottom);
        //                //                heightlet = finalRect.Height - Height;
        //                //            }
        //                //            Top = finalRect.Y + (int)(delta * heightlet);
        //                //        }
        //                //        break;
        //                //}

        //                //ArrangeOverride(new CSUtility.Support.Size(Width, Height));
        //            }
        //            break;

        //        case Visibility.Collapsed:
        //            break;
        //    }

        //    mArrangeInProcess = false;
        //}

        protected virtual void ArrangeCore(SlimDX.Rect finalRect)
        {
            // This is computed on every ArrangeCore. Depending on LayoutConstrained, actual clip may apply or not
            //NeedsClipBounds = false;

            // Start to compute arrange size for the child. 
            // It starts from layout slot or deisred size if layout slot is smaller then desired, 
            // and then we reduce it by margins, apply Width/Height etc, to arrive at the size
            // that child will get in its ArrangeOverride. 
            var arrangeSize = finalRect.Size;

            var margin = Margin;
            float marginWidth = (margin.Left + margin.Right);
            float marginHeight = (margin.Top + margin.Bottom);

            arrangeSize.Width = Math.Max(0, arrangeSize.Width - marginWidth);
            arrangeSize.Height = Math.Max(0, arrangeSize.Height - marginHeight);

            // compare against unclipped, transformed size.
            SlimDX.Size unclippedDesiredSize;
            if (mClippedSizeBox == SlimDX.Size.Empty)
            {
                unclippedDesiredSize = new SlimDX.Size(Math.Max(0, this.DesiredSize.Width - marginWidth),
                                                               Math.Max(0, this.DesiredSize.Height - marginHeight));
            }
            else
            {
                unclippedDesiredSize = new SlimDX.Size(mClippedSizeBox.Width, mClippedSizeBox.Height);
            }

            if (FloatUtil.LessThan(arrangeSize.Width, unclippedDesiredSize.Width))
            {
                //NeedsClipBounds = true; 
                arrangeSize.Width = unclippedDesiredSize.Width;
            }
            if (FloatUtil.LessThan(arrangeSize.Height, unclippedDesiredSize.Height))
            {
                //NeedsClipBounds = true; 
                arrangeSize.Height = unclippedDesiredSize.Height;
            }

            // Alignment==Stretch --> arrange at the slot size minus margins
            // Alignment!=Stretch --> arrange at the unclippedDesiredSize 
            if (HorizontalAlignment != UI.HorizontalAlignment.Stretch)
            {
                arrangeSize.Width = unclippedDesiredSize.Width;
            } 

            if (VerticalAlignment != UI.VerticalAlignment.Stretch) 
            { 
                arrangeSize.Height = unclippedDesiredSize.Height;
            }

            MinMax mm = new MinMax(this);

            //we have to choose max between UnclippedDesiredSize and Max here, because
            //otherwise setting of max property could cause arrange at less then unclippedDS.
            //Clipping by Max is needed to limit stretch here 
            var effectiveMaxWidth = Math.Max(unclippedDesiredSize.Width, mm.maxWidth);
            if (FloatUtil.LessThan(effectiveMaxWidth, arrangeSize.Width))
            {
                //NeedsClipBounds = true; 
                arrangeSize.Width = effectiveMaxWidth;
            }

            float effectiveMaxHeight = Math.Max(unclippedDesiredSize.Height, mm.maxHeight);
            if (FloatUtil.LessThan(effectiveMaxHeight, arrangeSize.Height))
            {
                //NeedsClipBounds = true; 
                arrangeSize.Height = effectiveMaxHeight;
            }

            //Here we use un-clipped InkSize because element does not know that it is
            //clipped by layout system and it shoudl have as much space to render as
            //it returned from its own ArrangeOverride 
            var oldSize = mSize;
            var innerInkSize = ArrangeOverride(arrangeSize);

            var renderSize = innerInkSize;

            //clippedInkSize differs from InkSize only what MaxWidth/Height explicitly clip the
            //otherwise good arrangement. For ex, DS<clientSize but DS>MaxWidth - in this
            //case we should initiate clip at MaxWidth and only show Top-Left portion 
            //of the element limited by Max properties. It is Top-left because in case when we
            //are clipped by container we also degrade to Top-Left, so we are consistent. 
            var clippedInkSize = new SlimDX.Size(Math.Min(innerInkSize.Width, mm.maxWidth),
                                                            Math.Min(innerInkSize.Height, mm.maxHeight));

            //Note that inkSize now can be bigger then layoutSlotSize-margin (because of layout 
            //squeeze by the parent or LayoutConstrained=true, which clips desired size in Measure). 

            // The client size is the size of layout slot decreased by margins. 
            // This is the "window" through which we see the content of the child.
            // Alignments position ink of the child in this "window".
            // Max with 0 is neccessary because layout slot may be smaller then unclipped desired size.
            var clientSize = new SlimDX.Size(Math.Max(0, finalRect.Width - marginWidth),
                                                        Math.Max(0, finalRect.Height - marginHeight));

            var offset = ComputeAlignmentOffset(clientSize, clippedInkSize);

            offset.X += finalRect.X + Margin.Left;
            offset.Y += finalRect.Y + Margin.Top;

            //                Left = (int)(offset.X + finalRect.X + Margin.Left);
            //                Top = (int)(offset.Y + finalRect.Y + Margin.Top);
            //                if (Width_Auto)
            //                    Width = innerInkSize.Width;
            //                if (Height_Auto)
            //                    Height = innerInkSize.Height;
            Left = (int)(offset.X);
            Top = (int)(offset.Y);

            if (Width_Auto)
                Width = (int)(clippedInkSize.Width);
            if(Height_Auto)
                Height = (int)(clippedInkSize.Height);

            MoveToWin(ref mLocation, true);
        }

        protected virtual SlimDX.Size ArrangeOverride(SlimDX.Size finalSize)
        {
            //foreach (var child in ChildWindows)
            for (int i = 0; i < ChildWindows.Count;i++ )
            {
                WinBase child = null;
                try
                {
                    child = ChildWindows[i];
                }
                catch (System.Exception)
                {
                    continue;
                }                
                child.Arrange(new SlimDX.Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            return finalSize;
        }

        private SlimDX.Vector2 ComputeAlignmentOffset(SlimDX.Size clientSize, SlimDX.Size inkSize)
        {
            SlimDX.Vector2 offset = new SlimDX.Vector2();
            //CSUtility.Support.Point offset = CSUtility.Support.Point.Empty;

            var ha = HorizontalAlignment;
            var va = VerticalAlignment;

            if (ha == UI.HorizontalAlignment.Stretch && inkSize.Width > clientSize.Width)
            {
                ha = UI.HorizontalAlignment.Left;
            }

            if (va == UI.VerticalAlignment.Stretch && inkSize.Height > clientSize.Height)
            {
                va = UI.VerticalAlignment.Top;
            }

            if (ha == UI.HorizontalAlignment.Center || ha == UI.HorizontalAlignment.Stretch)
            {
                offset.X = (clientSize.Width - inkSize.Width) * 0.5f;
            }
            else if (ha == UI.HorizontalAlignment.Right)
            {
                offset.X = clientSize.Width - inkSize.Width;
            }
            else
                offset.X = 0;

            if (va == UI.VerticalAlignment.Center || va == UI.VerticalAlignment.Stretch)
            {
                offset.Y = (clientSize.Height - inkSize.Height) * 0.5f;
            }
            else if (va == UI.VerticalAlignment.Bottom)
            {
                offset.Y = clientSize.Height - inkSize.Height;
            }
            else
                offset.Y = 0;

            return offset;
        }

        /////////////////////////////////////////////////////

        //protected virtual int GetWidthFromChildren()
        //{
        //    return 0;
        //}

        //protected virtual int GetHeightFromChildren()
        //{
        //    return 0;
        //}

        //protected virtual void UpdateWidthFromChildren()
        //{
        //    if (Width_Auto)
        //    {
        //        //int minLeft = int.MaxValue;
        //        int maxRight = int.MinValue;
        //        foreach (var child in ChildWindows)
        //        {
        //            //if (minLeft > child.Left)
        //            //    minLeft = child.Left;

        //            if (maxRight < child.Right + child.Margin.Right)
        //                maxRight = (int)(child.Right + child.Margin.Right);
        //        }

        //        Width = maxRight;// -minLeft;
        //    }
        //}

        //protected virtual void UpdateHeightFromChildren()
        //{
        //    if (Height_Auto)
        //    {
        //        //int minTop = int.MaxValue;
        //        int maxBottom = int.MinValue;
        //        foreach (var child in ChildWindows)
        //        {
        //            //if (minTop > child.Top)
        //            //    minTop = child.Top;

        //            if (maxBottom < child.Bottom + child.Margin.Bottom)
        //                maxBottom = (int)(child.Bottom + child.Margin.Bottom);
        //        }

        //        Height = maxBottom;// -minTop;
        //    }
        //}

        //// 更新横向布局
        //protected virtual void UpdateHorizontalArrangement()
        //{
        //    // 更新父
        //    if (Parent != null)
        //        Parent.UpdateWidthFromChildren();

        //    // 更新子
        //    foreach (var child in ChildWindows)
        //    {
        //        child.Margin = child.Margin;
        //    }
        //}
        //// 更新纵向布局
        //protected virtual void UpdateVerticalArrangement()
        //{
        //    // 更新父
        //    if (Parent != null)
        //        Parent.UpdateHeightFromChildren();

        //    // 更新子
        //    foreach (var child in ChildWindows)
        //    {
        //        child.Margin = child.Margin;
        //    }
        //}

        public virtual CSUtility.Support.Point GetChildOffset(WinBase child)
        {
            return CSUtility.Support.Point.Empty;
        }
        public virtual CSUtility.Support.Size GetSizeByChild(WinBase child)
        {
            return mSize;
        }

        //protected void GetLocationAndSize(ref int refLeft, ref int refTop, ref int refWidth, ref int refHeight)
        //{
        //    CSUtility.Support.Point offsetPos = CSUtility.Support.Point.Empty;
        //    CSUtility.Support.Size parentSize = CSUtility.Support.Size.Empty;
        //    if (Parent != null)
        //    {
        //        offsetPos = Parent.GetChildOffset(this);
        //        parentSize = Parent.GetSizeByChild(this);
        //    }

        //    switch (HorizontalAlignment)
        //    {
        //        case UI.HorizontalAlignment.Left:
        //            refLeft = (int)mMargin.Left + offsetPos.X;
        //            break;

        //        case UI.HorizontalAlignment.Center:
        //            {
        //                var parentCenter = this.Parent.Width / 2;
        //                refLeft = (int)(mMargin.Left - mMargin.Right + parentCenter) - refWidth / 2 + offsetPos.X;
        //            }
        //            break;

        //        case UI.HorizontalAlignment.Right:
        //            {
        //                refLeft = parentSize.Width - (int)mMargin.Right - refWidth + offsetPos.X;
        //            }
        //            break;

        //        case UI.HorizontalAlignment.Stretch:
        //            {
        //                if (Width_Auto)
        //                {
        //                    var actualWidth = parentSize.Width - mMargin.Left - mMargin.Right;
        //                    actualWidth = System.Math.Min(MaxWidth, actualWidth);
        //                    actualWidth = System.Math.Max(MinWidth, actualWidth);
        //                    double delta = mMargin.Left;
        //                    double widthlet = 1;
        //                    if ((mMargin.Left + mMargin.Right) != 0)
        //                    {
        //                        delta = mMargin.Left / (mMargin.Left + mMargin.Right);
        //                        widthlet = parentSize.Width - actualWidth;
        //                    }
        //                    refLeft = (int)(delta * widthlet) + offsetPos.X;
        //                    refWidth = (int)actualWidth;
        //                }
        //                else
        //                {
        //                    double delta = mMargin.Left;
        //                    double widthlet = 1;
        //                    if ((mMargin.Left + mMargin.Right) != 0)
        //                    {
        //                        delta = (float)mMargin.Left / (mMargin.Left + mMargin.Right);
        //                        widthlet = parentSize.Width - refWidth;
        //                    }
        //                    refLeft = (int)(delta * widthlet) + offsetPos.X;
        //                    var right = (int)((1 - delta) * widthlet);
        //                    if (refLeft != mMargin.Left && right != mMargin.Right)
        //                        mMargin = new CSCommon.Support.Thickness(refLeft, mMargin.Top, right, mMargin.Bottom);
        //                }
        //            }
        //            break;
        //    }

        //    switch (VerticalAlignment)
        //    {
        //        case UI.VerticalAlignment.Top:
        //            refTop = (int)mMargin.Top + offsetPos.Y;
        //            break;

        //        case UI.VerticalAlignment.Center:
        //            {
        //                var parentCenter = this.Parent.Height / 2;
        //                refTop = (int)(mMargin.Top - mMargin.Bottom + parentCenter) - refHeight / 2 + offsetPos.Y;
        //            }
        //            break;

        //        case UI.VerticalAlignment.Bottom:
        //            {
        //                refTop = parentSize.Height - (int)(mMargin.Bottom) - refHeight + offsetPos.Y;
        //            }
        //            break;

        //        case UI.VerticalAlignment.Stretch:
        //            {
        //                if (Height_Auto)
        //                {
        //                    var actualHeight = parentSize.Height - mMargin.Top - mMargin.Bottom;
        //                    actualHeight = System.Math.Min(MaxHeight, actualHeight);
        //                    actualHeight = System.Math.Max(MinHeight, actualHeight);
        //                    double delta = mMargin.Top;
        //                    double heightlet = 1;
        //                    if ((mMargin.Top + mMargin.Bottom) != 0)
        //                    {
        //                        delta = (float)mMargin.Top / (mMargin.Top + mMargin.Bottom);
        //                        heightlet = parentSize.Height - actualHeight;
        //                    }
        //                    refTop = (int)(delta * heightlet) + offsetPos.Y;
        //                    refHeight = (int)actualHeight;
        //                }
        //                else
        //                {
        //                    double delta = mMargin.Top;
        //                    double heightlet = 1;
        //                    if ((mMargin.Top + mMargin.Bottom) != 0)
        //                    {
        //                        delta = (float)mMargin.Top / (mMargin.Top + mMargin.Bottom);
        //                        heightlet = parentSize.Height - refHeight;
        //                    }
        //                    refTop = (int)(delta * heightlet) + offsetPos.Y;
        //                    var bottom = (int)((1 - delta) * heightlet);
        //                    if (refTop != mMargin.Top && bottom != mMargin.Bottom)
        //                        mMargin = new CSCommon.Support.Thickness(mMargin.Left, refTop, mMargin.Right, bottom);
        //                }
        //            }
        //            break;
        //    }
        //}

        public CSUtility.Support.Thickness GetMargin(int left, int top, int width, int height, WinBase parent)
        {
            double marginLeft = Margin.Left;
            double marginTop = Margin.Top;
            double marginRight = Margin.Right;
            double marginBottom = Margin.Bottom;

            if (parent == null)
            {
                marginLeft = left;
                marginTop = top;
                marginRight = 0;
                marginBottom = 0;
                return new CSUtility.Support.Thickness(marginLeft, marginTop, marginRight, marginBottom);
            }

            CSUtility.Support.Point offsetPos = ((WinBase)Parent).GetChildOffset(this);
            CSUtility.Support.Size parentSize = ((WinBase)Parent).GetSizeByChild(this);
            left -= offsetPos.X;
            top -= offsetPos.Y;

            switch (HorizontalAlignment)
            {
                case UI.HorizontalAlignment.Left:
                    marginLeft = left;
                    marginRight = 0;
                    //Margin = new CSCommon.Support.Thickness(left, Margin.Top, 0, Margin.Bottom);
                    break;
                case UI.HorizontalAlignment.Center:
                    {
                        var parentCenter = parent.Width / 2;
                        marginLeft = left + width / 2 - parentCenter;
                        marginRight = 0;
                        //Margin = new CSCommon.Support.Thickness(leftT, Margin.Top, 0, Margin.Bottom);
                    }
                    break;
                case UI.HorizontalAlignment.Right:
                    {
                        var right = parentSize.Width - (left + width);
                        marginLeft = 0;
                        marginRight = right;
                        //Margin = new CSCommon.Support.Thickness(0, Margin.Top, right, Margin.Bottom);
                    }
                    break;
                case UI.HorizontalAlignment.Stretch:
                    {
                        marginLeft = left;
                        marginRight = parentSize.Width - (left + width);
                    }
                    break;
            }
            switch (VerticalAlignment)
            {
                case UI.VerticalAlignment.Top:
                    marginTop = top;
                    marginBottom = 0;
                    break;
                case UI.VerticalAlignment.Center:
                    {
                        var parentCenter = parent.Height / 2;
                        var topT = top + height / 2 - parentCenter;
                        marginTop = topT;
                        marginBottom = 0;
                    }
                    break;
                case UI.VerticalAlignment.Bottom:
                    {
                        marginBottom = parentSize.Height - (top + height);
                        marginTop = 0;
                    }
                    break;
                case UI.VerticalAlignment.Stretch:
                    {
                        marginTop = top;
                        marginBottom = parentSize.Height - (top + height);
                    }
                    break;
            }

            return new CSUtility.Support.Thickness(marginLeft, marginTop, marginRight, marginBottom);
        }

        #endregion

    }
}
