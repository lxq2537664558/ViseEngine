using System.Collections.Specialized;

namespace UISystem.Content
{
    [CSUtility.Editor.UIEditor_Control("组件.ContentPresenter")]
    public class ContentPresenter : WinBase
    {
        public ContentControl HostContentControl;
        
        protected TextBlock mDefaultContent = new TextBlock();
        WinBase mContent = null;
        protected WinBase Content
        {
            get { return mContent; }
            set
            {
                var oldValue = mContent;
                mContent = value;

                if (HostContentControl != null)
                    HostContentControl.OnSetContent(mContent, oldValue);
            }
        }

        public ContentPresenter()
        {
            ContainerType = enContainerType.One;

            mDefaultContent.Text = "Content";
            mContent = mDefaultContent;

            Width_Auto = true;
            Height_Auto = true;
        }

        public override bool CanInsertChild()
        {
            if (mContent == mDefaultContent || mContent == null)
                return true;

            return false;
        }

        protected override void ChildWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.ChildWindows_CollectionChanged(sender, e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (HostContentControl != null)
                        {
                            HostContentControl.Content = e.NewItems[0] as WinBase;
                            //HostContentControl.ChildShowInTreeView.Add(e.NewItems[0] as WinBase);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (HostContentControl != null)
                        {
                            HostContentControl.Content = null;
                            //HostContentControl.ChildShowInTreeView.Remove(e.NewItems[0] as WinBase);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        protected override void OnAddChild(WinBase child)
        {
            base.OnAddChild(child);

            Content = child;
        }

        protected override void RemoveChild(WinBase child)
        {
            base.RemoveChild(child);

            Content = null;
        }


        //public override MSG_PROC ProcMessage(ref WinMSG msg)
        //{
        //    if (Content == null)
        //        return MSG_PROC.SendToParent;

        //    return Content.ProcMessage(ref msg);
        //}

        public override void PreProcBehavior(CCore.MsgProc.BehaviorParameter bhInit, Message.RoutedEventArgs args)
        {
            if (Content != null)
            {
                bhInit.Sender = this;
                Content.PreProcBehavior(bhInit, args);
            }
        }

        public override void ProcBehavior(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs args)
        {
            if (bhInit.Sender == Content)
            {
                var win = this.Parent as WinBase;
                bhInit.Sender = this;
                win.ProcBehavior(bhInit, args);
            }
            else if (Content != null)
            {
                bhInit.Sender = this;
                Content.ProcBehavior(bhInit, args);
            }
        }
    }
}
