using System;
using System.ComponentModel;
//using System.Linq;

namespace UISystem.ComboBox
{
    [CSUtility.Editor.UIEditor_ControlTemplateAble("ComboBox")]
    public class ComboBox : Content.ItemsControl
    {
        protected Guid mComboBoxItemTemplate;
        [Category("杂项")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UITemplateSetter")]
        public Guid ComboBoxItemTemplate
        {
            get { return mComboBoxItemTemplate; }
            set
            {
                mComboBoxItemTemplate = value;

                OnPropertyChanged("ComboBoxItemTemplate");
            }
        }

        protected ComboBoxItem mSelectedComboBoxItem = null;

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
                foreach (var child in mLogicChildren)
                {
                    child.Parent = null;
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
                if (mSelectedComboBoxItem == null)
                    return -1;

                int retValue = 0;
                foreach (var item in mItemsPresenter.Items)
                {
                    if (item == mSelectedComboBoxItem)
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
                    if (mSelectedComboBoxItem != null)
                        mSelectedComboBoxItem.IsSelected = false;
                    mSelectedComboBoxItem = null;
                    return;
                }

                if (value >= mItemsPresenter.Items.Length || value < 0)
                    return;

                mSelectedComboBoxItem = mItemsPresenter.Items[value] as ComboBoxItem;
                if (mSelectedComboBoxItem != null)
                    mSelectedComboBoxItem.IsSelected = true;

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

        // 每个child需要被一个ComboBoxItem包含
        protected override void OnAddChild(WinBase child)
        {
            if(child.IsTemplateControl)
                base.OnAddChild(child);
            else
            {
                if (CanInsertChild())
                {
                    if (child is ComboBoxItem)
                    {
                        child.Parent = mItemsPresenter;
                    }
                    else
                    {
                        var root = child.GetRoot(typeof(ComboBoxItem)) as ComboBoxItem;
                        if (root != null)
                        {
                            root.Parent = mItemsPresenter;
                        }
                        else
                        {
                            ComboBoxItem item = new ComboBoxItem();
                            item.Width_Auto = true;
                            item.Height_Auto = true;
                            item.TemplateId = ComboBoxItemTemplate;
                            child.Parent = item;
                            item.Parent = mItemsPresenter;

                            item.OnComboBoxItemSelected += item_OnComboBoxItemSelected;
                            item.OnContentSetted = OnComboBoxItemContentSetted;
                        }
                    }
                }
            }
        }

        void item_OnComboBoxItemSelected(ComboBoxItem item)
        {
            mSelectedComboBoxItem = item;

            int i = 0;
            foreach (ComboBoxItem cbItem in mItemsPresenter.Items)
            {
                if (cbItem == item)
                {
                    SelectedItem = cbItem.Content;
                    continue;
                }

                cbItem.IsSelected = false;
                i++;
            }
        }

        void OnComboBoxItemContentSetted(Content.ContentControl ctrl, object newValue, object oldValue)
        {
            if (newValue == null)
            {
                ctrl.Parent = null;
                RemoveChildShowInTreeView(oldValue as WinBase);
            }
        }

        public override void AddChildShowInTreeView(WinBase item)
        {
            if (item is ComboBoxItem)
            {
                LogicChildren.Add(((ComboBoxItem)item).Content);
            }
            else if (item.GetRoot(typeof(ComboBoxItem)) != null)
            {
                LogicChildren.Add(item);
            }
            else
            {

            }
        }

        protected override void SaveItems(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            if (mItemsPresenter != null && mItemsPresenter.Items != null)
            {
                foreach (ComboBoxItem item in mItemsPresenter.Items)
                {
                    var itemNode = pXml.AddNode(item.Content.GetType().Module.Name, "",holder);
                    item.Content.Save(itemNode,holder);
                }
            }
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml, holder);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ComboBoxItemTemplate"))
                pXml.AddAttrib("ComboBoxItemTemplate", ComboBoxItemTemplate.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlAttrib attr = pXml.FindAttrib("ComboBoxItemTemplate");
            if (attr != null)
            {
                Guid citId;
                citId = CSUtility.Support.IHelper.GuidTryParse(attr.Value);
                ComboBoxItemTemplate = citId;
            }

        }
    }
}
