using System.ComponentModel;

namespace UISystem.Content
{
    public class ContentControl : WinBase
    {
        public delegate void Delegate_OnContentSetted(ContentControl ctrl, object newValue, object oldValue);
        public Delegate_OnContentSetted OnContentSetted;

        protected WinBase mContent;
        [Browsable(false)]
        public WinBase Content
        {
            get { return mContent; }
            set
            {
                mContent = value;

                LogicChildren.Clear();
                if (mContent != null)
                    LogicChildren.Add(mContent);
            }
        }

        protected ContentPresenter mContentPresenter;

        protected override void OnSetTemplateId(Template.ControlTemplateInfo templateInfo)
        {
            if (templateInfo == null)
                return;

            mContentPresenter = FindControl(typeof(ContentPresenter), true) as ContentPresenter;
            if (mContentPresenter != null && mContentPresenter.HostContentControl != this)
            {
                mContentPresenter.HostContentControl = this;
            }        
        }

        public override bool CanInsertChild()
        {
            //var ctPCtrl = FindControl(typeof(ContentPresenter), true);
            //if (ctPCtrl == null)
            //    return false;

            //return ctPCtrl.CanInsertChild();
            if (mContentPresenter == null)
                return false;

            return mContentPresenter.CanInsertChild();
        }

        protected override void OnAddChild(WinBase child)
        {
            if (child.IsTemplateControl)
                base.OnAddChild(child);
            else
            {
                if (CanInsertChild())
                {
                    //if (mContentPresenter == null)
                    //{
                    //    //mContentPresenter = FindControl(typeof(ContentPresenter));
                    //    //if (mContentPresenter == null)
                    //        return;
                    //}

                    child.Parent = mContentPresenter;
                    //Content = child;
                }
            }
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (Content != null)
            {
                var contentNode = pXml.AddNode(Content.GetType().Module.Name, "",holder);
                Content.Save(contentNode,holder);
            }
            //if (mContentPresenter != null)
            //{
            //    foreach (var child in mContentPresenter.GetChildWindows())
            //    {
            //        var childNode = pXml.AddNode(child.GetType().Module.Name, "");
            //        child.Save(childNode);
            //    }
            //}
        }

        //protected override void OnLoad(CSUtility.Support.IXmlNode pXml)
        //{
        //    base.OnLoad(pXml);

        //    //var contentNode = pXml.FindNode("Content");
        //    //if (contentNode != null)
        //    //{
        //    //    string fullDllName = CSUtility.Support.IFileManager.Instance.Bin + contentNode.Name;
        //    //    fullDllName = fullDllName.Replace('/', '\\');
        //    //    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(fullDllName);
        //    //    WinBase pChildWin = (WinBase)(assembly.CreateInstance(contentNode.FindAttrib("TypeName").Value));
        //    //    if (pChildWin != null)
        //    //    {
        //    //        pChildWin.Parent = this;
        //    //        pChildWin.Load(contentNode);
        //    //    }
        //    //}
        //}

        public void OnSetContent(object newValue, object oldValue)
        {
            Content = newValue as WinBase;

            if (OnContentSetted != null)
                OnContentSetted(this, newValue, oldValue);
        }
    }
}
