using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem.Trigger
{
    public class PropertyTriggerConditon : INotifyPropertyChanged
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

        public enum enValueOperator
        {
            Equal,
            NotEqual,
            MoreThan,
            LessThan,
            MoreThanOrEqual,
            LessThanOrEqual,
        }

        //public delegate void Delegate_OnPropertyTriggerConditionChanged(PropertyTriggerConditon condition);
        //public Delegate_OnPropertyTriggerConditionChanged OnPropertyTriggerConditionChanged;

        public Guid TargetId
        {
            get
            {
                if (TargetControl == null)
                    return Guid.Empty;

                return TargetControl.Id;
            }
        }

        WinBase mTargetControl = null;
        public WinBase TargetControl
        {
            get { return mTargetControl; }
            set
            {
                mTargetControl = value;

                UpdateConditionInfoString();
            }
        }

        public string TargetName
        {
            get
            {
                if (TargetControl != null)
                    return TargetControl.NameInEditor;

                return "";
            }
        }

        public string TargetPropertyName
        {
            get
            {
                if (TargetProperty != null)
                    return TargetProperty.Name;

                return "";
            }
        }

        PropertyDescriptor mTargetProperty = null;
        public PropertyDescriptor TargetProperty
        {
            get { return mTargetProperty; }
            set
            {
                mTargetProperty = value;

                UpdateConditionInfoString();
            }
        }

        object mTargetValue;
        public object TargetValue
        {
            get { return mTargetValue; }
            set
            {
                mTargetValue = value;

                UpdateConditionInfoString();
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

        enValueOperator mValueOperator = enValueOperator.Equal;
        public enValueOperator ValueOperator
        {
            get { return mValueOperator; }
            set
            {
                mValueOperator = value;
                UpdateConditionInfoString();
            }
        }
        public string ValueOperatorString
        {
            get
            {
                return GetOperationString(mValueOperator);
            }
        }

        string mConditionInfoString = "";
        public string ConditionInfoString
        {
            get { return mConditionInfoString; }
            set
            {
                mConditionInfoString = value;
                OnPropertyChanged("ConditionInfoString");
            }
        }
        public void UpdateConditionInfoString()
        {
            ConditionInfoString = TargetName + "." + TargetPropertyName + ValueOperatorString + TargetValueString;
        }

        public static string GetOperationString(enValueOperator oper)
        {
            switch (oper)
            {
                case enValueOperator.Equal:
                    return "=";

                case enValueOperator.LessThan:
                    return "<";

                case enValueOperator.LessThanOrEqual:
                    return "<=";

                case enValueOperator.MoreThan:
                    return ">";

                case enValueOperator.MoreThanOrEqual:
                    return ">=";

                case enValueOperator.NotEqual:
                    return "!=";
            }

            return "?";
        }

        public PropertyTriggerConditon(WinBase control, PropertyDescriptor property, enValueOperator operate, object value)
        {
            mTargetControl = control;
            //if (mTargetControl != null)
            //{
            //    if (string.IsNullOrEmpty(control.WinName))
            //        control.WinName = control.GetType().Name;
            //}
            mTargetProperty = property;
            mTargetValue = value;
            mValueOperator = operate;

            UpdateConditionInfoString();
        }

        public bool IsConditionAgree()
        {
            if (TargetControl == null || TargetProperty == null)
                return false;

            var value = TargetProperty.GetValue(TargetControl);

            switch(ValueOperator)
            {
                case enValueOperator.Equal:
                    return object.Equals(TargetValue, value);

                case enValueOperator.LessThan:
                    {
                        if (TargetProperty.PropertyType == typeof(SByte))
                            return (SByte)value < (SByte)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int16))
                            return (Int16)value < (Int16)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int32))
                            return (Int32)value < (Int32)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int64))
                            return (Int64)value < (Int64)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Byte))
                            return (Byte)value < (Byte)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt16))
                            return (UInt16)value < (UInt16)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt32))
                            return (UInt32)value < (UInt32)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt64))
                            return (UInt64)value < (UInt64)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Single))
                            return (Single)value < (Single)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Double))
                            return (Double)value < (Double)TargetValue;
                        else
                            return false;
                    }

                case enValueOperator.LessThanOrEqual:
                    {
                        if (TargetProperty.PropertyType == typeof(SByte))
                            return (SByte)value <= (SByte)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int16))
                            return (Int16)value <= (Int16)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int32))
                            return (Int32)value <= (Int32)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int64))
                            return (Int64)value <= (Int64)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Byte))
                            return (Byte)value <= (Byte)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt16))
                            return (UInt16)value <= (UInt16)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt32))
                            return (UInt32)value <= (UInt32)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt64))
                            return (UInt64)value <= (UInt64)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Single))
                            return (Single)value <= (Single)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Double))
                            return (Double)value <= (Double)TargetValue;
                        else
                            return false;
                    }

                case enValueOperator.MoreThan:
                    {
                        if (TargetProperty.PropertyType == typeof(SByte))
                            return (SByte)value > (SByte)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int16))
                            return (Int16)value > (Int16)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int32))
                            return (Int32)value > (Int32)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int64))
                            return (Int64)value > (Int64)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Byte))
                            return (Byte)value > (Byte)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt16))
                            return (UInt16)value > (UInt16)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt32))
                            return (UInt32)value > (UInt32)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt64))
                            return (UInt64)value > (UInt64)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Single))
                            return (Single)value > (Single)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Double))
                            return (Double)value > (Double)TargetValue;
                        else
                            return false;
                    }

                case enValueOperator.MoreThanOrEqual:
                    {
                        if (TargetProperty.PropertyType == typeof(SByte))
                            return (SByte)value >= (SByte)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int16))
                            return (Int16)value >= (Int16)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int32))
                            return (Int32)value >= (Int32)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Int64))
                            return (Int64)value >= (Int64)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Byte))
                            return (Byte)value >= (Byte)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt16))
                            return (UInt16)value >= (UInt16)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt32))
                            return (UInt32)value >= (UInt32)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(UInt64))
                            return (UInt64)value >= (UInt64)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Single))
                            return (Single)value >= (Single)TargetValue;
                        else if (TargetProperty.PropertyType == typeof(Double))
                            return (Double)value >= (Double)TargetValue;
                        else
                            return false;
                    }

                case enValueOperator.NotEqual:
                    return !object.Equals(TargetValue, value);
            }

            return false;
        }
    }

    public class PropertyTrigger : UITrigger
    {
        public delegate void Delegate_OnTriggerPropertyChanged(PropertyTrigger trigger);
        public event Delegate_OnTriggerPropertyChanged OnTriggerPropertyChanged;

        public WinBase HostForm
        {
            get { return HostTriggerManager.HostForm; }
        }

        List<PropertyTriggerConditon> mConditions = new List<PropertyTriggerConditon>();
        public List<PropertyTriggerConditon> Conditions
        {
            get { return mConditions; }
        }
        List<TriggerAction> mActions = new List<TriggerAction>();
        public List<TriggerAction> Actions
        {
            get { return mActions; }
        }

        //string mInfoString = "";
        //public string InfoString
        //{
        //    get { return mInfoString; }
        //    set
        //    {
        //        mInfoString = value;
        //        OnPropertyChanged("InfoString");
        //    }
        //}

        public PropertyTrigger(UITriggerManager hostTriggerManager)
        {
            mHostTriggerManager = hostTriggerManager;
        }

        //// 更新Trigger的信息文字 (xxx.xx=x 和 xxxx.xx=xxx)
        //public void UpdateInfoString()
        //{
        //    string retValue = "";
        //    string andString = " 和 ";
        //    foreach (var condition in mConditions)
        //    {
        //        retValue += condition.TargetName + "." +
        //                    condition.TargetPropertyName + " = ";
        //        if (condition.TargetValue != null)
        //            retValue += condition.TargetValue.ToString();
        //        else
        //            retValue += "null";

        //        retValue += andString;
        //    }
        //    if(!string.IsNullOrEmpty(retValue))
        //        retValue = retValue.Remove(retValue.Length - andString.Length);

        //    InfoString = retValue;
        //}

        // 添加条件
        public void AddCondition(PropertyTriggerConditon condition)
        {
            mConditions.Add(condition);

            //condition.OnPropertyTriggerConditionChanged = OnTriggerConditionPropertyChanged;

            if (condition.TargetControl != null)
            {
                condition.TargetControl.OnPropertyChangedEvent += HostTriggerManager.OnPropertyTriggerConditionPropertyChanged;
            }

            if (OnTriggerPropertyChanged != null)
                OnTriggerPropertyChanged(this);
        }
        // 删除条件
        public void RemoveCondition(PropertyTriggerConditon condition)
        {
            mConditions.Remove(condition);

            //condition.OnPropertyTriggerConditionChanged = null;

            if (OnTriggerPropertyChanged != null)
                OnTriggerPropertyChanged(this);
        }

        // 添加触发响应
        public void AddAction(TriggerAction action)
        {
            mActions.Add(action);
            
            action.OnTriggerActionPropertyChanged = OnTriggerActionPropertyChanged;

            if (OnTriggerPropertyChanged != null)
                OnTriggerPropertyChanged(this);
        }
        // 删除触发响应
        public void RemoveAction(TriggerAction action)
        {
            mActions.Remove(action);

            action.OnTriggerActionPropertyChanged = null;

            if (OnTriggerPropertyChanged != null)
                OnTriggerPropertyChanged(this);
        }

        //private void OnTriggerConditionPropertyChanged(PropertyTriggerConditon condition)
        //{
        //    if (OnTriggerPropertyChanged != null)
        //        OnTriggerPropertyChanged(this);
        //}

        private void OnTriggerActionPropertyChanged(TriggerAction action)
        {
            if (OnTriggerPropertyChanged != null)
                OnTriggerPropertyChanged(this);
        }

        public TriggerAction_Property GetPropertyAction(string targetName, string targetPropertyName)
        {
            foreach (var action in mActions)
            {
                if (action is TriggerAction_Property)
                {
                    var propertyAction = action as TriggerAction_Property;
                    if (propertyAction.TargetName == targetName && propertyAction.TargetPropertyName == targetPropertyName)
                    {
                        return propertyAction;
                    }
                }
            }

            return null;
        }

        public bool IsConditionsContain(Guid targetId, string propertyName)
        {
            foreach (var condition in mConditions)
            {
                if (condition.TargetPropertyName == propertyName && condition.TargetId == targetId)
                    return true;
            }

            return false;
        }

        public override bool IsTriggerAvailable()
        {
            foreach (var condition in mConditions)
            {
                var control = HostForm.FindControl(condition.TargetId);
                if (control == null)
                    return false;

                var property = TypeDescriptor.GetProperties(control)[condition.TargetPropertyName];
                if (property == null)
                    return false;
                //var control = form.FindControl(condition.TargetName);
                //if (control == null)
                //    return false;
                //if (condition.TargetControl == null || condition.TargetProperty == null)
                //    return false;
            }

            return true;
        }

        public override bool TryActiveTrigger()
        {
            if (!mHostTriggerManager.EnableAction)
                return false;

            foreach (var condition in mConditions)
            {
                //var control = form.FindControl(condition.TargetName);
                //if (condition.TargetControl == null || condition.TargetProperty == null)
                //    return false;
                
                ////var property = TypeDescriptor.GetProperties(condition.TargetControl)[condition.TargetPropertyName];
                //var value = condition.TargetProperty.GetValue(condition.TargetControl);
                //if (!object.Equals(condition.TargetValue, value))
                //    return false;
                if(!condition.IsConditionAgree())
                    return false;
            }

            //mHostTriggerManager.SetToDefaultProperty();
            //foreach (var action in mActions)
            //{

            //    action.RunAction();
            //}
            SetToTriggerActionProperty();

            return true;
        }

        public override void SetToTriggerActionProperty()
        {
            mHostTriggerManager.SetToDefaultProperty();
            foreach (var action in mActions)
            {

                action.RunAction();
            }
        }

        public void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            //pXml.AddAttrib("TypeName", this.GetType().FullName);

            //var conditionsNode = pXml.AddNode("PropertyConditions", "");
            foreach (var condition in Conditions)
            {
                var condNode = pXml.AddNode("Condition", "",holder);
                //condNode.AddAttrib("SourceName", condition.TargetName);
                condNode.AddAttrib("TargetId", condition.TargetId.ToString());
                condNode.AddAttrib("Property", condition.TargetPropertyName);
                condNode.AddAttrib("Value", condition.TargetValueString);
                condNode.AddAttrib("Operate", condition.ValueOperator.ToString());
            }

            foreach (var action in Actions)
            {
                var actionNode = pXml.AddNode("Action", "", holder);
                actionNode.AddAttrib("TypeName", action.GetType().ToString());
                action.OnSave(actionNode);
            }
        }

        public void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            Conditions.Clear();

            var conditionNodes = pXml.FindNodes("Condition");
            foreach (var node in conditionNodes)
            {
                WinBase targetControl = null;
                PropertyDescriptor targetProperty = null;
                object targetValue = null;
                PropertyTriggerConditon.enValueOperator operate = PropertyTriggerConditon.enValueOperator.Equal;
                //var att = node.FindAttrib("SourceName");
                //if (att != null)
                //{
                //    targetControl = HostForm.FindControl(att.Value);
                //}
                var att = node.FindAttrib("TargetId");
                if (att != null)
                {
                    targetControl = HostForm.FindControl(CSUtility.Support.IHelper.GuidTryParse(att.Value));
                }
                att = node.FindAttrib("Property");
                if (att != null && targetControl != null)
                {
                    targetProperty = TypeDescriptor.GetProperties(targetControl)[att.Value];
                }
                att = node.FindAttrib("Value");
                if (att != null && targetProperty != null)
                {
                    targetValue = Assist.GetValueWithType(targetProperty.PropertyType, att.Value);
                }
                att = node.FindAttrib("Operate");
                if (att != null)
                {
                    operate  = (PropertyTriggerConditon.enValueOperator)System.Enum.Parse(typeof(PropertyTriggerConditon.enValueOperator), att.Value);
                }
                var condition = new PropertyTriggerConditon(targetControl, targetProperty, operate, targetValue);
                AddCondition(condition);
                //Conditions.Add(condition);

                //if (targetControl != null)
                //{
                //    targetControl.OnPropertyChangedEvent += HostTriggerManager.OnPropertyTriggerConditionPropertyChanged;
                //}
            }

            var actionNodes = pXml.FindNodes("Action");
            foreach (var node in actionNodes)
            {
                var att = node.FindAttrib("TypeName");
                var action = this.GetType().Assembly.CreateInstance(att.Value) as TriggerAction;
                action.HostForm = HostForm;
                action.OnLoad(node);

                AddAction(action);
                                
                if(action is TriggerAction_Property)
                {
                    var proAction = action as TriggerAction_Property;
                    if (proAction.TargetProperty != null && proAction.TargetControl != null)
                    {
                        var value = proAction.TargetProperty.GetValue(proAction.TargetControl);
                        mHostTriggerManager.SetDefaultProperty(proAction.TargetControl, proAction.TargetProperty, value);
                    }
                }
            }

            //UpdateInfoString();
        }

    }
}
