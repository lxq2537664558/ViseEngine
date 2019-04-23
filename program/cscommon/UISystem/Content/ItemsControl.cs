namespace UISystem.Content
{
    public class ItemsControl : WinBase
    {
        protected ItemsPresenter mItemsPresenter;

        protected override void OnSetTemplateId(Template.ControlTemplateInfo templateInfo)
        {
            if (templateInfo == null)
                return;

            mItemsPresenter = FindControl(typeof(ItemsPresenter), true) as ItemsPresenter;
            if(mItemsPresenter != null && mItemsPresenter.HostContentControl != this)
                mItemsPresenter.HostContentControl = this;
        }

        public override bool CanInsertChild()
        {
            if (mItemsPresenter == null)
                return false;

            return mItemsPresenter.CanInsertChild();
        }

        protected override void OnAddChild(WinBase child)
        {
            if(child.IsTemplateControl)
                base.OnAddChild(child);
            else
            {
                if (CanInsertChild())
                {
                    child.Parent = mItemsPresenter;
                }
            }
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            SaveItems(pXml,holder);
        }

        protected virtual void SaveItems(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            if (mItemsPresenter != null)
            {
                foreach (var child in mItemsPresenter.Items)
                {
                    var childNode = pXml.AddNode(child.GetType().Module.Name, "",holder);
                    child.Save(childNode,holder);
                }
            }
        }

        public virtual void AddChildShowInTreeView(WinBase item)
        {
            LogicChildren.Add(item);
        }

        public virtual void RemoveChildShowInTreeView(WinBase item)
        {
            LogicChildren.Remove(item);
        }
    }
}
