using System;
using System.ComponentModel;

namespace UISystem.Template
{
    public class ControlTemplate : WinBase
    {
        Trigger.UITriggerManager mUITriggerManager;
        [Browsable(false)]
        public Trigger.UITriggerManager UITriggerManager
        {
            get { return mUITriggerManager; }
        }

        Bind.PropertyBindManager mPropertyBindManager;
        [Browsable(false)]
        public Bind.PropertyBindManager PropertyBindManager
        {
            get { return mPropertyBindManager; }
        }

        protected Type mTargetType;
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UITypesSelect")]
        public Type TargetType
        {
            get { return mTargetType; }
            set
            {
                mTargetType = value;

                SetTargetType(mTargetType);

                OnPropertyChanged("TargetType");
            }
        }

        protected WinBase mTargetControl;
        public WinBase TargetControl
        {
            get { return mTargetControl; }
        }

        public ControlTemplate()
        {
            mUITriggerManager = new Trigger.UITriggerManager(this);
            mPropertyBindManager = new Bind.PropertyBindManager(this);
            ContainerType = enContainerType.Multi;
            EditDeleteAble = false;
        }

        protected void SetTargetType(Type type)
        {
            if (mTargetControl != null)
                mTargetControl.Parent = null;

            mTargetControl = this.GetType().Assembly.CreateInstance(type.ToString()) as WinBase;
            mTargetControl.Parent = this;
            mTargetControl.VisibleInTreeView = Visibility.Collapsed;
            mTargetControl.IgnoreSaver = true;

            mWinState = TargetControl.RState;
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (mUITriggerManager.IsHaveTrigger)
            {
                var triggerNode = pXml.AddNode("UITriggers", "",holder);
                mUITriggerManager.OnSave(triggerNode,holder);
            }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TargetType"))
                pXml.AddAttrib("TargetType", TargetType.ToString());

            if (TargetControl != null)
            {
                var targetNode = pXml.AddNode("TargetControl", "",holder);
                TargetControl.Save(targetNode,holder);
            }
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            var attr = pXml.FindAttrib("TargetType");
            if (attr != null)
                TargetType = System.Reflection.Assembly.GetExecutingAssembly().GetType(attr.Value);

            if (TargetControl != null)
            {
                var targetNode = pXml.FindNode("TargetControl");
                if (targetNode != null)
                    TargetControl.Load(targetNode);
            }
        }

        protected override void BeforeSave(CSUtility.Support.XmlNode pXml)
        {
            base.BeforeSave(pXml);

            mUITriggerManager.SetToDefaultProperty();
        }

        protected override void AfterLoad(CSUtility.Support.XmlNode pXml)
        {
            base.AfterLoad(pXml);

            var triggerNode = pXml.FindNode("UITriggers");
            if (triggerNode != null)
            {
                if (mUITriggerManager == null)
                    mUITriggerManager = new Trigger.UITriggerManager(this);
                mUITriggerManager.OnLoad(triggerNode);
            }

            //BuildCommandBinding();
        }
    }
}
