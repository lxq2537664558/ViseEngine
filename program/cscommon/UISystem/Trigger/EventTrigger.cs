using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem.Trigger
{
    public class EventTriggerCondition : INotifyPropertyChanged
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

        public Guid TargetId
        {
            get
            {
                if (TargetControl == null)
                    return Guid.Empty;

                return TargetControl.Id;
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

        string mTargetEventName = "";
        public string TargetEventName
        {
            get { return mTargetEventName; }
            set
            {
                mTargetEventName = value;
                UpdateConditionInfoString();
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
            ConditionInfoString = TargetName + "." + TargetEventName;
        }

        public EventTriggerCondition()
        {
        }

        public virtual void CopyFrom(EventTriggerCondition condition, WinBase rootWin)
        {
            this.TargetControl = rootWin.FindControl(condition.TargetId);
            TargetEventName = condition.TargetEventName;
            ConditionInfoString = condition.ConditionInfoString;
        }
    }

    public class EventTrigger : UITrigger
    {
        public delegate void Delegate_OnTriggerPropertyChanged(EventTrigger trigger);
        public event Delegate_OnTriggerPropertyChanged OnTriggerPropertyChanged;

        public WinBase HostRoot
        {
            get { return HostTriggerManager.HostForm; }
        }

        EventTriggerCondition mConditon = new EventTriggerCondition();
        public EventTriggerCondition Condition
        {
            get { return mConditon; }
        }

        List<TriggerAction> mActions = new List<TriggerAction>();
        public List<TriggerAction> Actions
        {
            get { return mActions; }
        }

        public EventTrigger(UITriggerManager hostTriggerManager)
        {
            mHostTriggerManager = hostTriggerManager;
        }

        public override bool TryActiveTrigger()
        {
            if (!mHostTriggerManager.EnableAction)
                return false;

            SetToTriggerActionProperty();

            return true;
        }

        public override void CopyFrom(UITrigger trigger, WinBase rootWin)
        {
            EventTrigger evTrigger = trigger as EventTrigger;
            if(evTrigger == null)
                return;

            Condition.CopyFrom(evTrigger.Condition, rootWin);

            Actions.Clear();
            foreach (var action in evTrigger.Actions)
            {
                var triAction = (TriggerAction)(action.GetType().Assembly.CreateInstance(action.GetType().FullName));
                triAction.CopyFrom(action, rootWin);
                AddAction(triAction);
            }
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

        public void RemoveAllActions()
        {
            foreach (var action in mActions)
            {
                action.OnTriggerActionPropertyChanged = null;
            }
            mActions.Clear();
        }

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

        public override bool IsTriggerAvailable()
        {
            if (Condition == null)
                return false;

            var control = HostRoot.FindControl(Condition.TargetId);
            if (control == null)
                return false;

            var eventHandle = TypeDescriptor.GetEvents(control)[Condition.TargetEventName];
            if (eventHandle == null)
                return false;

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
            pXml.AddAttrib("ConditionTargetId", Condition.TargetId.ToString());
            pXml.AddAttrib("ConditionEventName", Condition.TargetEventName);

            foreach (var action in Actions)
            {
                var actionNode = pXml.AddNode("Action", "", holder);
                actionNode.AddAttrib("TypeName", action.GetType().ToString());
                action.OnSave(actionNode);
            }
        }

        public void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            var att = pXml.FindAttrib("ConditionTargetId");
            if (att != null)
            {
                Condition.TargetControl = HostRoot.FindControl(CSUtility.Support.IHelper.GuidTryParse(att.Value));
            }
            att = pXml.FindAttrib("ConditionEventName");
            if (att != null)
            {
                Condition.TargetEventName = att.Value;
            }

            var actionNodes = pXml.FindNodes("Action");
            foreach (var node in actionNodes)
            {
                var attr = node.FindAttrib("TypeName");
                var action = this.GetType().Assembly.CreateInstance(attr.Value) as TriggerAction;
                action.HostForm = HostRoot;
                action.OnLoad(node);

                AddAction(action);

                if (action is TriggerAction_Property)
                {
                    var proAction = action as TriggerAction_Property;
                    if (proAction.TargetProperty != null && proAction.TargetControl != null)
                    {
                        var value = proAction.TargetProperty.GetValue(proAction.TargetControl);
                        mHostTriggerManager.SetDefaultProperty(proAction.TargetControl, proAction.TargetProperty, value);
                    }
                }
            }
        }
    }
}
