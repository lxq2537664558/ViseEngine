using System;
using System.ComponentModel;

namespace UISystem.Trigger
{
    public class TriggerAction : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public delegate void Delegate_OnTriggerActionPropertyChanged(TriggerAction action);
        public Delegate_OnTriggerActionPropertyChanged OnTriggerActionPropertyChanged;

        protected WinBase mHostForm;
        public WinBase HostForm
        {
            get { return mHostForm; }
            set { mHostForm = value; }
        }

        public virtual bool RunAction()
        {
            return false;
        }

        public virtual void OnSave(CSUtility.Support.XmlNode pXml)
        {

        }

        public virtual void OnLoad(CSUtility.Support.XmlNode pXml)
        {

        }

        public virtual void CopyFrom(TriggerAction action, WinBase rootWin) { }
    }

    public class TriggerAction_Property : TriggerAction
    {
        public string TargetName
        {
            get
            {
                if(mTargetControl != null)
                    return mTargetControl.WinName;

                return "";
            }
        }

        public Guid TargetId
        {
            get
            {
                if (mTargetControl != null)
                    return mTargetControl.Id;

                return Guid.Empty;
            }
        }
        WinBase mTargetControl;
        public WinBase TargetControl
        {
            get { return mTargetControl; }
            //set
            //{
            //    mTargetControl = value;

            //    UpdateInfoString();
            //}
        }

        public string TargetPropertyName
        {
            get
            {
                if(mTargetProperty != null)
                    return mTargetProperty.Name;

                return "";
            }
        }
        PropertyDescriptor mTargetProperty;
        public PropertyDescriptor TargetProperty
        {
            get { return mTargetProperty; }
            //set
            //{
            //    mTargetProperty = value;

            //    UpdateInfoString();
            //}
        }
        object mTargetValue;
        public object TargetValue
        {
            get { return mTargetValue; }
            set
            {
                mTargetValue = value;

                UpdateInfoString();
            }
        }
        public string TargetValueString
        {
            get
            {
                if (TargetValue != null)
                    return TargetValue.ToString();

                return "{null}";
            }
        }

        string mInfoString;
        public string InfoString
        {
            get { return mInfoString; }
            set
            {
                mInfoString = value;
                OnPropertyChanged("InfoString");
            }
        }

        private void UpdateInfoString()
        {
            InfoString = "." + TargetPropertyName + " = " + mTargetValue;
        }

        public TriggerAction_Property()
        {

        }

        public TriggerAction_Property(WinBase control, PropertyDescriptor property, object value)
        {
            mTargetControl = control;
            //if (control != null)
            //{
            //    if (string.IsNullOrEmpty(control.WinName))
            //        control.WinName = control.GetType().Name;
            //}

            mTargetProperty = property;
            mTargetValue = value;

            UpdateInfoString();
        }

        public override void CopyFrom(TriggerAction action, WinBase rootWin)
        {
            TriggerAction_Property proAction = action as TriggerAction_Property;
            if(proAction == null)
                return;

            mTargetControl = rootWin.FindControl(proAction.TargetId);
            if(mTargetControl != null)
            {
                mTargetProperty = TypeDescriptor.GetProperties(mTargetControl)[proAction.TargetPropertyName];
                mTargetValue = proAction.TargetValue;
            }

            UpdateInfoString();
        }

        public override bool RunAction()
        {
            if (mTargetControl == null)
                return false;

            //var property = TypeDescriptor.GetProperties(ctrl)[mTargetProperty];
            if (mTargetProperty == null)
                return false;

            mTargetProperty.SetValue(mTargetControl, mTargetValue);

            return true;
        }

        public override void OnSave(CSUtility.Support.XmlNode pXml)
        {
            base.OnSave(pXml);
            //pXml.AddAttrib("TargetName", TargetName);
            pXml.AddAttrib("TargetId", TargetId.ToString());
            pXml.AddAttrib("Property", TargetPropertyName);
            pXml.AddAttrib("Value", TargetValueString);
        }

        public override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);
            //var att = pXml.FindAttrib("TargetName");
            //if(att != null)
            //{
            //    mTargetControl = mHostForm.FindControl(att.Value);
            //}
            var att = pXml.FindAttrib("TargetId");
            if (att != null)
            {
                mTargetControl = mHostForm.FindControl(CSUtility.Support.IHelper.GuidTryParse(att.Value));
            }
            att = pXml.FindAttrib("Property");
            if (att != null && mTargetControl != null)
            {
                mTargetProperty = TypeDescriptor.GetProperties(mTargetControl)[att.Value];
            }
            att = pXml.FindAttrib("Value");
            if (att != null && mTargetProperty != null)
            {
                mTargetValue = Assist.GetValueWithType(mTargetProperty.PropertyType, att.Value);
            }

            UpdateInfoString();
        }
    }

    public class TriggerAction_Event : TriggerAction
    {

    }

    public class TriggerAction_Animation : TriggerAction
    {

    }
}
