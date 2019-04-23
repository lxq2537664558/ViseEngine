using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace UISystem.Trigger
{
    public class UITrigger : INotifyPropertyChanged
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
        
        protected UITriggerManager mHostTriggerManager;
        public UITriggerManager HostTriggerManager
        {
            get { return mHostTriggerManager; }
        }

        //public UITrigger(UITriggerManager hostTriggerManager) { }

        public virtual bool IsTriggerAvailable() { return false; }
        public virtual bool TryActiveTrigger() { return false; }
        public virtual void SetToTriggerActionProperty() { }

        public static bool TryActiveTrigger(UITrigger trigger)
        {
            if (trigger != null)
                return trigger.TryActiveTrigger();

            return false;
        }

        public virtual void CopyFrom(UITrigger trigger, WinBase rootWin) { }
    }

    public class UITriggerEventTest
    {
        private UITrigger mTrigger = null;
        public UITrigger Trigger
        {
            get { return mTrigger; }
            set { mTrigger = value; }
        }

        public void ActiveTrigger(WinBase Sender)
        {
            mTrigger.TryActiveTrigger();
        }
    }

    public class UITriggerManager
    {
        WinBase mHostForm;
        public WinBase HostForm
        {
            get { return mHostForm; }
        }

        List<PropertyTrigger> mPropertyTriggerList = new List<PropertyTrigger>();
        public List<PropertyTrigger> PropertyTriggerList
        {
            get { return mPropertyTriggerList; }
        }

        List<EventTrigger> mEventTriggerList = new List<EventTrigger>();
        public List<EventTrigger> EventTriggerList
        {
            get { return mEventTriggerList; }
        }

        Dictionary<string, TriggerAction_Property> mDefaultPropertyDictionary = new Dictionary<string, TriggerAction_Property>();

        public bool EnableAction = true;

        public bool IsHaveTrigger
        {
            get
            {
                if(mPropertyTriggerList.Count > 0)
                    return true;

                if(mEventTriggerList.Count > 0)
                    return true;

                return false;
            }
        }

        public UITriggerManager(WinBase host)
        {
            mHostForm = host;
        }

        //private void OnAttachedWinControlPropertyChanged(WinBase control, string propertyName)
        //{
        //    ActivePropertyTrigger(propertyName);
        //}

        public void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            foreach (var propertyTrigger in mPropertyTriggerList)
            {
                var node = pXml.AddNode("PropertyTrigger", "",holder);
                propertyTrigger.OnSave(node,holder);
            }

            foreach (var eventTrigger in mEventTriggerList)
            {
                var node = pXml.AddNode("EventTrigger", "",holder);
                eventTrigger.OnSave(node,holder);
            }
        }

        public void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            var propertyTriggerNodes = pXml.FindNodes("PropertyTrigger");
            foreach (var node in propertyTriggerNodes)
            {
                PropertyTrigger trigger = new PropertyTrigger(this);
                trigger.OnLoad(node);
                AddPropertyTrigger(trigger);
            }

            var eventTriggerNodes = pXml.FindNodes("EventTrigger");
            foreach (var node in eventTriggerNodes)
            {
                EventTrigger trigger = new EventTrigger(this);
                trigger.OnLoad(node);
                AddEventTrigger(trigger);
            }
        }

        public void AddPropertyTrigger(PropertyTrigger trigger)
        {
            mPropertyTriggerList.Add(trigger);
        }
        public void RemovePropertyTrigger(PropertyTrigger trigger)
        {
            mPropertyTriggerList.Remove(trigger);
        }

        public void AddEventTrigger(EventTrigger trigger)
        {
            mEventTriggerList.Add(trigger);
        }
        public void RemoveEventTrigger(EventTrigger trigger)
        {
            mEventTriggerList.Remove(trigger);
        }

        public void RemoveTrigger(UITrigger trigger)
        {
            if (trigger is PropertyTrigger)
                RemovePropertyTrigger(trigger as PropertyTrigger);
            else if (trigger is EventTrigger)
                RemoveEventTrigger(trigger as EventTrigger);
        }

        //public void ActivePropertyTrigger(, string propertyName)
        //{
        //    foreach (var trigger in mPropertyTriggerList)
        //    {
        //        if(trigger.IsConditionsContain(mHostControl.WinName, propertyName))
        //            trigger.TryActiveTrigger();
        //    }
        //}

        public void SetDefaultProperty(WinBase control, PropertyDescriptor property, object value)
        {
            TriggerAction_Property defaultAction = null;
            if (mDefaultPropertyDictionary.TryGetValue(control.WinName + "_" + property.Name, out defaultAction))
            {
                defaultAction.TargetValue = value;
            }
            else
            {
                defaultAction = new TriggerAction_Property(control, property, value);
                mDefaultPropertyDictionary[control.WinName + "_" + property.Name] = defaultAction;
            }
        }

        public TriggerAction_Property GetDefaultProperty(string controlName, string propertyName)
        {
            TriggerAction_Property defaultAction = null;
            if (mDefaultPropertyDictionary.TryGetValue(controlName + "_" + propertyName, out defaultAction))
                return defaultAction;

            return null;
        }

        // 设置回默认值
        public void SetToDefaultProperty(string controlName = "", string propertyName = "")
        {
            if (string.IsNullOrEmpty(controlName) && string.IsNullOrEmpty(propertyName))
            {
                foreach (var value in mDefaultPropertyDictionary.Values)
                {
                    value.RunAction();
                }
            }
            else if (!string.IsNullOrEmpty(controlName) && string.IsNullOrEmpty(propertyName))
            {
                foreach (var value in mDefaultPropertyDictionary.Values)
                {
                    if (value.TargetPropertyName == controlName)
                        value.RunAction();
                }
            }
            else
            {
                TriggerAction_Property action = null;
                if (mDefaultPropertyDictionary.TryGetValue(controlName + "_" + propertyName, out action))
                    action.RunAction();
            }
        }

        public void OnPropertyTriggerConditionPropertyChanged(WinBase control, string propertyName)
        {
            foreach (var trigger in mPropertyTriggerList)
            {
                if (trigger.IsConditionsContain(control.Id, propertyName))
                {
                    trigger.TryActiveTrigger();
                }
            }
        }

        public void BuildTriggers(WinBase rootControl)
        {
            //foreach (var trigger in mPropertyTriggerList)
            //{
            //}

            if (mEventTriggerList.Count <= 0)
                return;

#if WIN || ANDROID


            AssemblyName aName = new AssemblyName();
            aName.Name = "UITriggerHandlerClass_" + mHostForm.Id.ToString().Replace("-", "_");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, System.Reflection.Emit.AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(aName.Name);
              // 定义类型
            foreach (var trigger in mEventTriggerList)
            {
                EventTrigger copyedTrigger = new EventTrigger(this);
                copyedTrigger.CopyFrom(trigger, rootControl);

                //var targetControl = rootControl.FindControl(trigger.Condition.TargetId);
                var targetControl = copyedTrigger.Condition.TargetControl;
                if (targetControl == null)
                    continue;

                var eventDescriptor = TypeDescriptor.GetEvents(targetControl)[copyedTrigger.Condition.TargetEventName];
                if (eventDescriptor == null)
                    continue;

                var methodInfo = copyedTrigger.GetType().GetMethod("TryActiveTrigger");

                Type handlerType = eventDescriptor.EventType;
                MethodInfo invokeMethod = handlerType.GetMethod("Invoke");
                ParameterInfo[] parms = invokeMethod.GetParameters();
                Type[] parmTypes = new Type[parms.Length];
                for (int i = 0; i < parms.Length; i++)
                {
                    parmTypes[i] = parms[i].ParameterType;
                }

                // 创建动态类型
                // public class TriggerHandler_XXX
                var classTypeName = "TriggerHandler_" + targetControl.Id.ToString().Replace("-", "_") + "_" + copyedTrigger.Condition.TargetEventName;
                //if (moduleBuilder.GetType(classTypeName) != null)
                //{
                //    moduleBuilder.ResolveType()
                //}
                var typeBuilder = moduleBuilder.DefineType(classTypeName, TypeAttributes.Class | TypeAttributes.Public);
                // private UITrigger mTrigger = null;
                var fieldTriggerBuilder = typeBuilder.DefineField("mTrigger", typeof(UITrigger), FieldAttributes.Private);
                fieldTriggerBuilder.SetConstant(null);
                // public UITrigger Trigger
                var propertyTriggerBuilder = typeBuilder.DefineProperty("Trigger", PropertyAttributes.None, typeof(UITrigger), null);
                // Trigger.get
                var getPropertyTriggerBuilder = typeBuilder.DefineMethod("get", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(UITrigger), Type.EmptyTypes);
                var getTriggerIL = getPropertyTriggerBuilder.GetILGenerator();
                getTriggerIL.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                getTriggerIL.Emit(System.Reflection.Emit.OpCodes.Ldfld, fieldTriggerBuilder);
                getTriggerIL.Emit(System.Reflection.Emit.OpCodes.Ret);
                // Trigger.set
                var setPropertyTriggerBuilder = typeBuilder.DefineMethod("set", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { typeof(UITrigger) });
                var setTriggerIL = setPropertyTriggerBuilder.GetILGenerator();
                setTriggerIL.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                setTriggerIL.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                setTriggerIL.Emit(System.Reflection.Emit.OpCodes.Stfld, fieldTriggerBuilder);
                setTriggerIL.Emit(System.Reflection.Emit.OpCodes.Ret);
                propertyTriggerBuilder.SetGetMethod(getPropertyTriggerBuilder);
                propertyTriggerBuilder.SetSetMethod(setPropertyTriggerBuilder);

                // public void ActiveTrigger(XXXX,XXX,...)
                var methodName = "ActiveTrigger";//trigger.Condition.TargetEventName + "_" + trigger.Condition.TargetControl.Id.ToString().Replace("-", "_") + "_Handler";
                var methodHandler = typeBuilder.DefineMethod(methodName, MethodAttributes.Public, invokeMethod.ReturnType, parmTypes);

                //var handlerMethod = new System.Reflection.Emit.DynamicMethod("CallEvent", null, parmTypes);
                //var activeTriggerMethodInfo = typeof(UITrigger).GetMethod("TryActiveTrigger", BindingFlags.Static | BindingFlags.Public, null, new Type[] {typeof(UITrigger)}, null);
                
                var il = methodHandler.GetILGenerator();
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Ldfld, fieldTriggerBuilder);
                il.Emit(System.Reflection.Emit.OpCodes.Callvirt, typeof(UITrigger).GetMethod("TryActiveTrigger", Type.EmptyTypes));
                il.Emit(System.Reflection.Emit.OpCodes.Pop);
                il.Emit(System.Reflection.Emit.OpCodes.Ret);
                
                Type triggerHandlerType = typeBuilder.CreateType();
                var triggerHandlerInstance = assemblyBuilder.CreateInstance(triggerHandlerType.ToString());

                var property = triggerHandlerType.GetProperty("Trigger");
                property.SetValue(triggerHandlerInstance, copyedTrigger, null);

                var eventMethodInfo = triggerHandlerInstance.GetType().GetMethod(methodName);

                Delegate d = Delegate.CreateDelegate(handlerType, triggerHandlerInstance, eventMethodInfo);
                eventDescriptor.AddEventHandler(targetControl, d);
            }
#elif IOS

#endif
        }

    }
}


//public class TriggerHandler_XXX
//{
//    private UITrigger mTrigger = null;

//    public UITrigger Trigger
//    {
//        get
//        {
//            return mTrigger;
//        }
//        set
//        {
//            mTrigger = value;
//        }
//    }

//    public bool ActiveTrigger()
//    {
//        return mTrigger.TryActiveTrigger();
//    }
//}