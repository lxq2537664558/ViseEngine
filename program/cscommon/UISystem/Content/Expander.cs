using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("Expander")]
    public class Expander : Content.ContentControl
    {
        protected bool mIsExpanded = false;
        [Category("公共属性")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set
            {
                mIsExpanded = value;

                if (OnExpandChanged != null)
                    OnExpandChanged(mIsExpanded);

                OnPropertyChanged("IsExpanded");
            }
        }

        public Expander()
        {

        }

        protected override void OnSetTemplateId(Template.ControlTemplateInfo templateInfo)
        {
            base.OnSetTemplateId(templateInfo);

            if (mContentPresenter != null)
            {
                // 刷新一次
                IsExpanded = IsExpanded;
            }
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "IsExpanded"))
                pXml.AddAttrib("IsExpanded", IsExpanded.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            var attr = pXml.FindAttrib("IsExpanded");
            if (attr != null)
                IsExpanded = System.Convert.ToBoolean(attr.Value);
        }

#region Command

        public delegate void Delegate_OnExpandChanged(bool isExpanded);
        [CSUtility.Editor.UIEditor_CommandEvent]
        [CSUtility.Editor.UIEditor_BindingEvent]
        public event Delegate_OnExpandChanged OnExpandChanged;

        [CSUtility.Editor.UIEditor_CommandMethod]
        public void HeaderClickCommand(WinBase sender)
        {
            IsExpanded = !IsExpanded;
        }

#endregion
    }
}
