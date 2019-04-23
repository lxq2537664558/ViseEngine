using System;
using System.ComponentModel;
//using System.Linq;

namespace UISystem.ListBox
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("ListBox")]
    public class ListBox : Content.ItemsControl
    {
        //protected List<ListBoxItem> mItems = new List<ListBoxItem>();

        protected Guid mItemContainerTemplate = CSUtility.Support.IHelper.GuidTryParse("a4651c29-cce6-42fa-80c9-11f59bf917b4");
        public Guid ItemContainerTemplate
        {
            get { return mItemContainerTemplate; }
            set
            {
                mItemContainerTemplate = value;

                OnPropertyChanged("ItemContainerTemplate");
            }
        }

        protected ListBoxItem mSelectedListBoxItem = null;

        [Category("杂项")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public WinBase[] Items
        {
            get
            {
                return mLogicChildren.ToArray();
            }
            set
            {
                //foreach (var child in mLogicChildren)
                //{
                //    child.Parent = null;
                //}
                for (int i = mLogicChildren.Count-1; i >= 0; i--)
                {
                    mLogicChildren[i].Parent = null;
                }
                mLogicChildren.Clear();

                foreach (var child in value)
                {
                    child.Parent = this;
                }

                OnPropertyChanged("Items");
            }
        }

        [Category("杂项")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public int SelectedIndex
        {
            get
            {
                if (mSelectedListBoxItem == null)
                    return -1;

                int retValue = 0;
                foreach (var item in mItemsPresenter.Items)
                {
                    if (item == mSelectedListBoxItem)
                        return retValue;

                    retValue++;
                }

                return -1;
            }
            set
            {
                if (mItemsPresenter == null)
                    return;

                if (value < 0)
                {
                    if (mSelectedListBoxItem != null)
                        mSelectedListBoxItem.IsSelected = false;
                    mSelectedListBoxItem = null;
                    return;
                }

                if (value >= mItemsPresenter.Items.Length || value < 0)
                    return;

                mSelectedListBoxItem = mItemsPresenter.Items[value] as ListBoxItem;
                if (mSelectedListBoxItem != null)
                    mSelectedListBoxItem.IsSelected = true;

                OnPropertyChanged("SelectedIndex");
            }
        }

        [Category("杂项")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public object SelectedItem
        {
            get
            {
                if (SelectedIndex < 0)
                    return null;

                return LogicChildren[SelectedIndex];
            }
            set
            {
                var idx = LogicChildren.IndexOf(value as WinBase);
                SelectedIndex = idx;

                OnPropertyChanged("SelectedItem");
            }
        }

        // 每一个child需要被一个ListBoxItem包含
        protected override void OnAddChild(WinBase child)
        {
            if (child.IsTemplateControl)
                base.OnAddChild(child);
            else
            {
                if (CanInsertChild())
                {
                    if (child is ListBoxItem)
                    {
                        child.Parent = mItemsPresenter;
                    }
                    else
                    {
                        var root = child.GetRoot(typeof(ListBoxItem)) as ListBoxItem;
                        if (root != null)
                        {
                            root.Parent = mItemsPresenter;
                        }
                        else
                        {
                            ListBoxItem item = new ListBoxItem();
                            item.Width_Auto = true;
                            item.Height_Auto = true;
                            item.TemplateId = ItemContainerTemplate;
                            child.Parent = item;
                            item.Parent = mItemsPresenter;

                            item.OnListBoxItemSelected += item_OnListBoxItemSelected;
                            item.OnContentSetted = OnListBoxItemContentSetted;
                        }
                    }   
                    //child.Parent = mItemsPresenter;
                }
            }
        }

        void item_OnListBoxItemSelected(ListBoxItem item)
        {
            mSelectedListBoxItem = item;

            int i = 0;
            foreach (ListBoxItem lbItem in mItemsPresenter.Items)
            {
                if (lbItem == item)
                {
                    //SelectedIndex = i;
                    SelectedItem = lbItem.Content;
                    continue;
                }

                lbItem.IsSelected = false;
                i++;
            }
        }

        void OnListBoxItemContentSetted(Content.ContentControl ctrl, object newValue, object oldValue)
        {
            if (newValue == null)
            {
                ctrl.Parent = null;
                RemoveChildShowInTreeView(oldValue as WinBase);
            }
        }

        public override void AddChildShowInTreeView(WinBase item)
        {
            if (item is ListBoxItem)
            {
                LogicChildren.Add(((ListBoxItem)item).Content);
            }
            else if (item.GetRoot(typeof(ListBoxItem)) != null)
            {
                LogicChildren.Add(item);
            }
            else
            {

            }
        }

        //public override void RemoveChildShowInTreeView(WinBase item)
        //{
        //    base.RemoveChildShowInTreeView(item);
        //}

        protected override void SaveItems(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            if (mItemsPresenter != null && mItemsPresenter.Items != null )
            {
                foreach (ListBoxItem item in mItemsPresenter.Items)
                {
                    var itemNode = pXml.AddNode(item.Content.GetType().Module.Name, "", holder);
                    item.Content.Save(itemNode,holder);
                }
            }
        }
    }
}
