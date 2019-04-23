using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("ScrollViewer")]
    //[CSUtility.Editor.UIEditor_Control("ScrollViewer")]
    public class ScrollViewer : Content.ContentControl
    {
        public enum ScrollBarVisibility
        {
            Visible,
            Hidden,
            Auto,
            Disable,
        }

        protected Visibility mComputedVerticalScrollBarVisibility;
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public Visibility ComputedVerticalScrollBarVisibility
        {
            get { return mComputedVerticalScrollBarVisibility; }
            set
            {
                mComputedVerticalScrollBarVisibility = value;

                OnPropertyChanged("ComputedVerticalScrollBarVisibility");
            }
        }

        protected Visibility mComputedHorizontalScrollBarVisibility;
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get { return mComputedHorizontalScrollBarVisibility; }
            set
            {
                mComputedHorizontalScrollBarVisibility = value;

                OnPropertyChanged("ComputedHorizontalScrollBarVisibility");
            }
        }

        protected ScrollBarVisibility mHorizontalScrollBarVisibility = ScrollBarVisibility.Disable;
        [Category("布局")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return mHorizontalScrollBarVisibility; }
            set
            {
                mHorizontalScrollBarVisibility = value;

                switch(mHorizontalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Visible:
                        ComputedHorizontalScrollBarVisibility = Visibility.Visible;
                        break;

                    case ScrollBarVisibility.Hidden:
                    case ScrollBarVisibility.Disable:
                        ComputedHorizontalScrollBarVisibility = Visibility.Collapsed;
                        break;

                    case ScrollBarVisibility.Auto:
                        {
                            var scrollContentPresenter = mContentPresenter as Content.ScrollContentPresenter;
                            if (scrollContentPresenter != null)
                            {
                                if (scrollContentPresenter.ScrollableWidth > scrollContentPresenter.ViewportWidth)
                                    ComputedHorizontalScrollBarVisibility = Visibility.Visible;
                                else
                                    ComputedHorizontalScrollBarVisibility = Visibility.Collapsed;
                            }
                        }
                        break;
                }

                OnPropertyChanged("HorizontalScrollBarVisibility");
            }
        }

        protected ScrollBarVisibility mVerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        [Category("布局")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return mVerticalScrollBarVisibility; }
            set
            {
                mVerticalScrollBarVisibility = value;

                switch (mVerticalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Visible:
                        ComputedVerticalScrollBarVisibility = Visibility.Visible;
                        break;

                    case ScrollBarVisibility.Hidden:
                    case ScrollBarVisibility.Disable:
                        ComputedVerticalScrollBarVisibility = Visibility.Collapsed;
                        break;

                    case ScrollBarVisibility.Auto:
                        {
                            var scrollContentPresenter = mContentPresenter as Content.ScrollContentPresenter;
                            if (scrollContentPresenter != null)
                            {
                                if (scrollContentPresenter.ScrollableHeight > scrollContentPresenter.ViewportHeight)
                                    ComputedVerticalScrollBarVisibility = Visibility.Visible;
                                else
                                    ComputedVerticalScrollBarVisibility = Visibility.Collapsed;
                            }
                        }
                        break;
                }

                OnPropertyChanged("VerticalScrollBarVisibility");
            }
        }

        public ScrollViewer()
        {
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "HorizontalScrollBarVisibility"))
                pXml.AddAttrib("HorizontalScrollBarVisibility", HorizontalScrollBarVisibility.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "VerticalScrollBarVisibility"))
                pXml.AddAttrib("VerticalScrollBarVisibility", VerticalScrollBarVisibility.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            var attr = pXml.FindAttrib("HorizontalScrollBarVisibility");
            if (attr != null)
            {
                HorizontalScrollBarVisibility = (ScrollBarVisibility)System.Enum.Parse(typeof(ScrollBarVisibility), attr.Value);
            }
            attr = pXml.FindAttrib("VerticalScrollBarVisibility");
            if (attr != null)
            {
                VerticalScrollBarVisibility = (ScrollBarVisibility)System.Enum.Parse(typeof(ScrollBarVisibility), attr.Value);
            }
        }

        public void ScrollVerticalToStart()
        {
            if (ComputedVerticalScrollBarVisibility != Visibility.Visible)
                return;

            if (this.mContentPresenter == null)
                return;

            var scrollCP = mContentPresenter as UISystem.Content.ScrollContentPresenter;
            if (scrollCP == null)
                return;

            scrollCP.VerticalOffset = 0;
        }
        public void ScrollVerticalToEnd()
        {
            if (ComputedVerticalScrollBarVisibility != Visibility.Visible)
                return;

            if (this.mContentPresenter == null)
                return;

            var scrollCP = mContentPresenter as UISystem.Content.ScrollContentPresenter;
            if (scrollCP == null)
                return;

            UpdateLayout();
            var root = GetRoot(typeof(WinRoot)) as WinRoot;
            if(root != null)
            {
                root.Tick(CCore.Engine.Instance.GetElapsedMillisecond());
            }
            scrollCP.VerticalOffset = scrollCP.ScrollableHeight;

            //UpdateLayout();
            //if (root != null)
            //{
            //    root.Tick(CCore.Engine.Instance.GetElapsedMillisecond());
            //}
        }

        public void ScrollHorizontalToStart()
        {
            if (ComputedHorizontalScrollBarVisibility != Visibility.Visible)
                return;

            if (this.mContentPresenter == null)
                return;

            var scrollCP = mContentPresenter as UISystem.Content.ScrollContentPresenter;
            if (scrollCP == null)
                return;

            scrollCP.HorizontalOffset = 0;
        }
        public void ScrollHorizontalToEnd()
        {
            if (ComputedHorizontalScrollBarVisibility != Visibility.Visible)
                return;

            if (this.mContentPresenter == null)
                return;

            var scrollCP = mContentPresenter as UISystem.Content.ScrollContentPresenter;
            if (scrollCP == null)
                return;

            UpdateLayout();
            var root = GetRoot(typeof(WinRoot)) as WinRoot;
            if (root != null)
            {
                root.Tick(CCore.Engine.Instance.GetElapsedMillisecond());
            }
            scrollCP.HorizontalOffset = scrollCP.ScrollableWidth;
        }
    }
}
