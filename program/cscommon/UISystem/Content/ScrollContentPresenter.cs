using System.ComponentModel;

namespace UISystem.Content
{
    // ScrollViewer专用ContentPresenter
    [CSUtility.Editor.UIEditor_Control("组件.ScrollContentPresenter")]
    public class ScrollContentPresenter : ContentPresenter
    {
        protected float mScrollableHeight;
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float ScrollableHeight
        {
            get { return mScrollableHeight; }
            set
            {
                mScrollableHeight = value;

                OnPropertyChanged("ScrollableHeight");
            }
        }

        protected float mViewportHeight;
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float ViewportHeight
        {
            get { return mViewportHeight; }
            set
            {
                mViewportHeight = value;

                OnPropertyChanged("ViewportHeight");
            }
        }

        protected float mVerticalOffset;
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float VerticalOffset
        {
            get { return mVerticalOffset; }
            set
            {
                if (System.Math.Abs(mVerticalOffset - value) < Assist.MinFloatValue)
                    return;

                mVerticalOffset = value;

                if (Content != null)
                {
                    Content.Margin = new CSUtility.Support.Thickness(Content.Margin.Left,
                                                                   -mVerticalOffset / ScrollableHeight * (ScrollableHeight - ViewportHeight),
                                                                   Content.Margin.Right,
                                                                   Content.Margin.Bottom);
                }

                OnPropertyChanged("VerticalOffset");
            }
        }

        protected float mScrollableWidth;
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float ScrollableWidth
        {
            get { return mScrollableWidth; }
            set
            {
                mScrollableWidth = value;

                OnPropertyChanged("ScrollableWidth");
            }
        }

        protected float mViewportWidth;
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float ViewportWidth
        {
            get { return mViewportWidth; }
            set
            {
                mViewportWidth = value;

                OnPropertyChanged("ViewportWidth");
            }
        }

        protected float mHorizontalOffset;
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float HorizontalOffset
        {
            get { return mHorizontalOffset; }
            set
            {
                if (System.Math.Abs(mHorizontalOffset - value) < Assist.MinFloatValue)
                    return;

                mHorizontalOffset = value;

                if (Content != null)
                {
                    Content.Margin = new CSUtility.Support.Thickness(-mHorizontalOffset / ScrollableWidth * (ScrollableWidth - ViewportWidth),
                                                                   Content.Margin.Top,
                                                                   Content.Margin.Right,
                                                                   Content.Margin.Bottom);
                }

                OnPropertyChanged("HorizontalOffset");
            }
        }

        public ScrollContentPresenter()
        {
            ViewportHeight = this.Height;
            ViewportWidth = this.Width;

            this.WinSizeChanged += ScrollContentPresenter_WinSizeChanged;
        }

        protected override void OnAddChild(WinBase child)
        {
            base.OnAddChild(child);

            child.Margin = new CSUtility.Support.Thickness(0);
            child.HorizontalAlignment = UI.HorizontalAlignment.Left;
            //child.LockedHorizontals.Add(UI.HorizontalAlignment.Left);

            child.VerticalAlignment = UI.VerticalAlignment.Top;
            //child.LockedVerticals.Add(UI.VerticalAlignment.Top);

            //child.OnPropertyChangedEvent += Child_OnPropertyChangedEvent;

            ScrollableHeight = child.Height;
            ScrollableWidth = child.Width;

            child.WinSizeChanged += Child_WinSizeChanged;

            UpdateScrollViewerHVVisibility();
        }

        void ScrollContentPresenter_WinSizeChanged(int w, int h, WinBase Sender)
        {
            ViewportHeight = h;
            ViewportWidth = w;

            UpdateScrollViewerHVVisibility();
        }

        void Child_WinSizeChanged(int w, int h, WinBase Sender)
        {
            ScrollableHeight = h;
            ScrollableWidth = w;

            UpdateScrollViewerHVVisibility();
        }

        void UpdateScrollViewerHVVisibility()
        {
            var scrollViewer = HostContentControl as ScrollViewer;
            if (scrollViewer != null)
            {
                switch (scrollViewer.HorizontalScrollBarVisibility)
                {
                    case ScrollViewer.ScrollBarVisibility.Auto:
                        {
                            if (ScrollableWidth > ViewportWidth)
                                scrollViewer.ComputedHorizontalScrollBarVisibility = Visibility.Visible;
                            else
                                scrollViewer.ComputedHorizontalScrollBarVisibility = Visibility.Collapsed;
                        }
                        break;

                    case ScrollViewer.ScrollBarVisibility.Disable:
                    case ScrollViewer.ScrollBarVisibility.Hidden:
                        {
                            if (scrollViewer.ComputedHorizontalScrollBarVisibility != Visibility.Collapsed)
                                scrollViewer.ComputedHorizontalScrollBarVisibility = Visibility.Collapsed;
                        }
                        break;

                    case ScrollViewer.ScrollBarVisibility.Visible:
                        {
                            if (scrollViewer.ComputedHorizontalScrollBarVisibility != Visibility.Visible)
                                scrollViewer.ComputedHorizontalScrollBarVisibility = Visibility.Visible;
                        }
                        break;
                }

                switch (scrollViewer.VerticalScrollBarVisibility)
                {
                    case ScrollViewer.ScrollBarVisibility.Auto:
                        {
                            if (ScrollableHeight > ViewportHeight)
                                scrollViewer.ComputedVerticalScrollBarVisibility = Visibility.Visible;
                            else
                                scrollViewer.ComputedVerticalScrollBarVisibility = Visibility.Collapsed;
                        }
                        break;

                    case ScrollViewer.ScrollBarVisibility.Disable:
                    case ScrollViewer.ScrollBarVisibility.Hidden:
                        {
                            if (scrollViewer.ComputedVerticalScrollBarVisibility != Visibility.Collapsed)
                                scrollViewer.ComputedVerticalScrollBarVisibility = Visibility.Collapsed;
                        }
                        break;

                    case ScrollViewer.ScrollBarVisibility.Visible:
                        {
                            if (scrollViewer.ComputedVerticalScrollBarVisibility != Visibility.Visible)
                                scrollViewer.ComputedVerticalScrollBarVisibility = Visibility.Visible;
                        }
                        break;
                }
            }
        }

        //void Child_OnPropertyChangedEvent(WinBase control, string propertyName)
        //{
        //    switch (propertyName)
        //    {
        //        case "Width":
        //            {

        //            }
        //            break;

        //        case "Height":
        //            {

        //            }
        //            break;
        //    }
        //}

        protected override SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            if (ChildWindows.Count <= 0)
                return base.MeasureOverride(availableSize);

            var scrollViewer = HostContentControl as ScrollViewer;
            if(scrollViewer == null)
                return base.MeasureOverride(availableSize);

            //int retWidth = availableSize.Width;
            //int retHeight = availableSize.Height;

            var childAWidth = availableSize.Width;
            var childAHeight = availableSize.Height;

            switch (scrollViewer.HorizontalScrollBarVisibility)
            {
                case ScrollViewer.ScrollBarVisibility.Disable:
                    break;

                default:
                    childAWidth = float.PositiveInfinity;
                    break;
            }

            switch (scrollViewer.VerticalScrollBarVisibility)
            {
                case ScrollViewer.ScrollBarVisibility.Disable:
                    break;

                default:
                    childAHeight = float.PositiveInfinity;
                    break;
            }

            ChildWindows[0].Measure(new SlimDX.Size(childAWidth, childAHeight));

            //return ChildWindows[0].DesiredSize;
            return availableSize;
        }

        protected override SlimDX.Size ArrangeOverride(SlimDX.Size finalSize)
        {
            if (ChildWindows.Count <= 0)
            {
                return base.ArrangeOverride(finalSize);
            }
            
            var scrollViewer = HostContentControl as ScrollViewer;
            if (scrollViewer == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            var childFinalWidth = System.Math.Max(finalSize.Width, ChildWindows[0].DesiredSize.Width);
            var childFinalHeight = System.Math.Max(finalSize.Height, ChildWindows[0].DesiredSize.Height);

            //switch (scrollViewer.HorizontalScrollBarVisibility)
            //{
            //    case ScrollViewer.ScrollBarVisibility.Disable:
            //        break;

            //    default:
            //        childFinalWidth = float.PositiveInfinity;
            //        break;
            //}

            //switch (scrollViewer.VerticalScrollBarVisibility)
            //{
            //    case ScrollViewer.ScrollBarVisibility.Disable:
            //        break;

            //    default:
            //        childFinalHeight = float.PositiveInfinity;
            //        break;
            //}

            ChildWindows[0].Arrange(new SlimDX.Rect(0, 0, childFinalWidth, childFinalHeight));

            if (ChildWindows[0].Width <= ViewportWidth)
                HorizontalOffset = 0;
            if (ChildWindows[0].Height <= ViewportHeight)
                VerticalOffset = 0;

            return finalSize;
        }
    }
}
