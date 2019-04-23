using System.Collections.Specialized;
using System.ComponentModel;

namespace UISystem.Content
{
    [CSUtility.Editor.UIEditor_Control("组件.ItemsPresenter")]
    public class ItemsPresenter : WinBase
    {
        public enum ItemsContainerType
        {
            StackPanel,
            WrapPanel,
        }
        protected ItemsContainerType mItemsContainerType = ItemsContainerType.StackPanel;

        public ItemsControl HostContentControl;
        [Browsable(false)]
        public WinBase[] Items
        {
            get
            {
                if (mItemsContainer == null)
                    return null;

                return mItemsContainer.GetChildWindows();
            }
        }

        protected Container.Container mItemsContainer = null;

        public ItemsPresenter()
        {
            ContainerType = enContainerType.One;
        }

        public override bool CanInsertChild()
        {
            if (mItemsContainer == null)
            {
                InitialContainer();
            }

            return mItemsContainer.CanInsertChild();
        }

        protected void InitialContainer()
        {
            if (mItemsContainer != null)
            {
                mItemsContainer.Parent = null;
            }

            switch (mItemsContainerType)
            {
                case ItemsContainerType.StackPanel:
                    {
                        mItemsContainer = new StackPanel();
                    }
                    break;

                case ItemsContainerType.WrapPanel:
                    {
                        mItemsContainer = new WrapPanel();
                    }
                    break;
            }

            mItemsContainer.IgnoreSaver = true;
            mItemsContainer.Parent = this;
            mItemsContainer.HorizontalAlignment = UI.HorizontalAlignment.Stretch;
            mItemsContainer.VerticalAlignment = UI.VerticalAlignment.Stretch;
            mItemsContainer.Width_Auto = true;
            mItemsContainer.Height_Auto = true;
            mItemsContainer.BackColor = CSUtility.Support.Color.FromArgb(0);
            //mItemsContainer.Width = 200;
            //mItemsContainer.Height = 500;

            mItemsContainer.AddChildWindowsCollectionChangedHandler(ContainerChildWindows_CollectionChanged);
        }

        protected override void OnAddChild(WinBase child)
        {
            if (ChildWindows.Count <= 0)
            {
                if (child is StackPanel)
                {
                    mItemsContainerType = ItemsContainerType.StackPanel;
                    base.OnAddChild(child);
                    mItemsContainer = child as Container.Container;
                    return;
                }
                else if (child is WrapPanel)
                {
                    mItemsContainerType = ItemsContainerType.WrapPanel;
                    base.OnAddChild(child);
                    mItemsContainer = child as Container.Container;
                    return;
                }
                else
                {
                    InitialContainer();
                }
            }

            //if (child == mItemsContainer)
            //{
            //    base.AddChild(child);
            //    return;
            //}

            //base.AddChild(child);
            child.Parent = mItemsContainer;
        }

        protected override void RemoveChild(WinBase child)
        {
            //base.RemoveChild(child);
        }

        protected void ContainerChildWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.ChildWindows_CollectionChanged(sender, e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (HostContentControl != null)
                        {
                            foreach (WinBase item in e.NewItems)
                            {
                                //HostContentControl.ChildShowInTreeView.Add(item);
                                HostContentControl.AddChildShowInTreeView(item);
                                //Items.Add(item);
                            }
                            //HostContentControl.ChildShowInTreeView.Add()
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (HostContentControl != null)
                        {
                            foreach (WinBase item in e.OldItems)
                            {
                                //HostContentControl.ChildShowInTreeView.Remove(item);
                                HostContentControl.RemoveChildShowInTreeView(item);
                                //Items.Remove(item);
                            }
                        }
                    }
                    break;
            }
        }

    }
}
