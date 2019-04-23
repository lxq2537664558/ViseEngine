using System;
using System.ComponentModel;

namespace UISystem.Bind
{
    public enum enBindingMode
    {
        TwoWay,     // 双向
        OneWay,     // 从数据源到界面
        OneWayToSource, // 从界面到数据源
    }

    public class PropertyBindInfo
    {
        protected enBindingMode mBindingMode = enBindingMode.OneWay;
        public enBindingMode BindingMode
        {
            get { return mBindingMode; }
            set { mBindingMode = value; }
        }

        //protected UIBindAutoUpdate mHostObject;
        //protected PropertyDescriptor mHostProperty;
        protected Guid mHostObjectId;
        public Guid HostObjectId
        {
            get { return mHostObjectId; }
        }
        protected String mHostPropertyName;
        public string HostPropertyName
        {
            get { return mHostPropertyName; }
        }

        public string mParseDataString = "";
        
        public virtual object GetBindedPropertyValue()
        {
            return null;
        }

        public virtual void SetBindedPropertyValue(UIBindAutoUpdate bindObj, object value)
        {
        }

        public virtual bool Parse(WinBase rootControl)
        {
            return false;
        }
    }

    public class ClassPropertyBindInfo : PropertyBindInfo
    {
        //enBindingMode mBindingMode = enBindingMode.TwoWay;
        //public enBindingMode BindingMode
        //{
        //    get { return mBindingMode; }
        //    set { mBindingMode = value; }
        //}

        string mDllName = "";
        public string DllName
        {
            get { return mDllName; }
            set { mDllName = value; }
        }
        Type mClassType = null;
        public Type ClassType
        {
            get { return mClassType; }
            set { mClassType = value; }
        }
        System.Reflection.PropertyInfo mPropertyInfo = null;
        public System.Reflection.PropertyInfo PropertyInfo
        {
            get { return mPropertyInfo; }
            set { mPropertyInfo = value; }
        }

        //public ClassPropertyBindInfo(UIBindAutoUpdate hostObject, PropertyDescriptor hostProperty)
        public ClassPropertyBindInfo(Guid hostObjectId, string hostPropertyName)
        {
            //mHostObject = hostObject;
            //mHostProperty = hostProperty;
            mHostObjectId = hostObjectId;
            mHostPropertyName = hostPropertyName;
        }

        public override bool Parse(WinBase rootControl)
        {
            if (string.IsNullOrEmpty(mParseDataString))
                return false;

            // xx.dll@className|propertyname|bindingMode
            var splitsDll = mParseDataString.Split('@');
            if (splitsDll.Length < 2)
                return false;

            DllName = splitsDll[0];            

            var classSplits = splitsDll[1].Split('|');
            if (classSplits.Length < 3)
                return false;

            var assembly = CCore.Support.ReflectionManager.Instance.GetAssembly(DllName);
            if (assembly == null)
                return false;

            var className = classSplits[0];
            ClassType = assembly.GetType(className);
            if (ClassType == null)
                return false;
            
            var propertyName = classSplits[1];
            PropertyInfo = ClassType.GetProperty(propertyName);
            if (PropertyInfo == null)
                return false;

            var hostObject = rootControl.FindControl(HostObjectId);
            if (hostObject == null)
                return false;
            var hostProperty = TypeDescriptor.GetProperties(hostObject)[HostPropertyName];
            if (hostProperty == null)
                return false;

            BindingMode = (enBindingMode)(System.Enum.Parse(typeof(enBindingMode), classSplits[2]));

            switch (BindingMode)
            {
                case enBindingMode.OneWay:
                    {
                        var bindClassObject = CCore.Support.ReflectionManager.Instance.GetClassObject(ClassType);
                        if (bindClassObject is UIBindAutoUpdate)
                        {
                            var bindClassInstance = bindClassObject as UIBindAutoUpdate;
                            var property = TypeDescriptor.GetProperties(bindClassInstance)[PropertyInfo.Name];
                            var info = new ControlPropertyBindInfo(bindClassInstance.Id, property.Name);
                            info.SourceObject = hostObject;
                            info.SourcePropertyInfo = hostProperty;
                            info.BindingMode = enBindingMode.OneWayToSource;
                            bindClassInstance.AddActionBind(PropertyInfo.Name, info);
                        }

                        {
                            var info = new ClassPropertyBindInfo(hostObject.Id, hostProperty.Name);
                            info.ClassType = ClassType;
                            info.DllName = DllName;
                            info.PropertyInfo = PropertyInfo;
                            info.BindingMode = BindingMode;
                            hostObject.AddPassiveBind(hostProperty.Name, info);
                        }
                    }
                    break;

                case enBindingMode.OneWayToSource:
                    {
                        var info = new ClassPropertyBindInfo(hostObject.Id, hostProperty.Name);
                        info.ClassType = ClassType;
                        info.DllName = DllName;
                        info.PropertyInfo = PropertyInfo;
                        info.BindingMode = BindingMode;
                        hostObject.AddActionBind(hostProperty.Name, info);
                    }
                    break;

                case enBindingMode.TwoWay:
                    {
                        var bindClassObject = CCore.Support.ReflectionManager.Instance.GetClassObject(ClassType);
                        if (bindClassObject is UIBindAutoUpdate)
                        {
                            var bindClassInstance = bindClassObject as UIBindAutoUpdate;
                            var property = TypeDescriptor.GetProperties(bindClassInstance)[PropertyInfo.Name];
                            var info = new ControlPropertyBindInfo(bindClassInstance.Id, property.Name);
                            info.SourceObject = hostObject;
                            info.SourcePropertyInfo = hostProperty;
                            info.BindingMode = enBindingMode.OneWayToSource;
                            bindClassInstance.AddActionBind(PropertyInfo.Name, info);
                        }

                        {
                            var info = new ClassPropertyBindInfo(hostObject.Id, hostProperty.Name);
                            info.ClassType = ClassType;
                            info.DllName = DllName;
                            info.PropertyInfo = PropertyInfo;
                            info.BindingMode = BindingMode;
                            hostObject.AddActionBind(hostProperty.Name, info);
                        }
                    }
                    break;
            }

            return true;
        }

        public override string ToString()
        {
            if (mClassType == null || mPropertyInfo == null)
                return "";
            
            return CSUtility.Program.GetTypeSaveString(mClassType) + "|" + mPropertyInfo.Name + "|" + BindingMode.ToString();
        }

        public override object GetBindedPropertyValue()
        {
            var bindClassObject = CCore.Support.ReflectionManager.Instance.GetClassObject(ClassType);
            if(bindClassObject == null)
                return null;

            return PropertyInfo.GetValue(bindClassObject, null);
        }

        public override void SetBindedPropertyValue(UIBindAutoUpdate bindObj, object value)
        {
            if (BindingMode == enBindingMode.OneWay)
                return;

            var bindClassObject = CCore.Support.ReflectionManager.Instance.GetClassObject(ClassType);
            if (bindClassObject == null)
                return;

            var proValue = CCore.Support.ReflectionManager.Instance.GetValueForType(PropertyInfo.PropertyType, PropertyInfo.GetValue(bindClassObject, null));
            var valueToType = CCore.Support.ReflectionManager.Instance.GetValueForType(PropertyInfo.PropertyType, value);
            //if (proValue == null || valueToType == null)
            //    return;
            if (!Object.Equals(proValue, valueToType))
                PropertyInfo.SetValue(bindClassObject, valueToType, null);
        }
    }


    public class ControlPropertyBindInfo : PropertyBindInfo
    {
        //enBindingMode mBindingMode = enBindingMode.TwoWay;
        //public enBindingMode BindingMode
        //{
        //    get { return mBindingMode; }
        //    set { mBindingMode = value; }
        //}

        UIBindAutoUpdate mSourceObject = null;
        public UIBindAutoUpdate SourceObject
        {
            get { return mSourceObject; }
            set { mSourceObject = value; }
        }

        Guid mSourceObjectId = Guid.Empty;
        public Guid SourceObjectId
        {
            get
            {
                if (SourceObject != null)
                    return SourceObject.Id;

                return Guid.Empty;
            }
        }

        PropertyDescriptor mSourcePropertyInfo = null;
        public PropertyDescriptor SourcePropertyInfo
        {
            get { return mSourcePropertyInfo; }
            set { mSourcePropertyInfo = value; }
        }
        
        //public ControlPropertyBindInfo(UIBindAutoUpdate hostObject, PropertyDescriptor hostProperty)
        public ControlPropertyBindInfo(Guid hostObjectId, string hostPropertyName)
        {
            //mHostObject = hostObject;
            //mHostProperty = hostProperty;
            mHostObjectId = hostObjectId;
            mHostPropertyName = hostPropertyName;
        }

        public override bool Parse(WinBase root)//string dataString)
        {
            if (string.IsNullOrEmpty(mParseDataString) || root == null)
                return false;

            var splits = mParseDataString.Split('|');
            if (splits.Length < 3)
                return false;

            Guid targetId = CSUtility.Support.IHelper.GuidTryParse(splits[0]);
            SourceObject = root.FindControl(targetId);
            if (SourceObject == null)
                return false;

            var propertyName = splits[1];
            SourcePropertyInfo = TypeDescriptor.GetProperties(SourceObject)[propertyName];
            if (SourcePropertyInfo == null)
                return false;

            var hostObject = root.FindControl(HostObjectId);// mHostObjectId);
            if (hostObject == null)
                return false;

            var hostProperty = TypeDescriptor.GetProperties(hostObject)[HostPropertyName];//mHostPropertyName];
            if (hostProperty == null)
                return false;

            BindingMode = (enBindingMode)(System.Enum.Parse(typeof(enBindingMode), splits[2]));

            switch (BindingMode)
            {
                case enBindingMode.OneWay:
                    {
                        // 当Source更新后更新绑定的值
                        ControlPropertyBindInfo info = new ControlPropertyBindInfo(SourceObject.Id, SourcePropertyInfo.Name);
                        info.SourceObject = hostObject;
                        info.SourcePropertyInfo = hostProperty;
                        info.BindingMode = enBindingMode.OneWayToSource;
                        SourceObject.AddActionBind(SourcePropertyInfo.Name, info);

                        info = new ControlPropertyBindInfo(hostObject.Id, hostProperty.Name);
                        info.SourceObject = SourceObject;
                        info.SourcePropertyInfo = SourcePropertyInfo;
                        info.BindingMode = BindingMode;
                        hostObject.AddPassiveBind(hostProperty.Name, info);
                    }
                    break;

                case enBindingMode.OneWayToSource:
                    {
                        var info = new ControlPropertyBindInfo(hostObject.Id, hostProperty.Name);
                        info.SourceObject = SourceObject;
                        info.SourcePropertyInfo = SourcePropertyInfo;
                        info.BindingMode = BindingMode;
                        hostObject.AddActionBind(hostProperty.Name, info);
                    }
                    break;

                case enBindingMode.TwoWay:
                    {
                        // 当Source更新后更新绑定的值
                        ControlPropertyBindInfo info = new ControlPropertyBindInfo(SourceObject.Id, SourcePropertyInfo.Name);
                        info.SourceObject = hostObject;
                        info.SourcePropertyInfo = hostProperty;
                        info.BindingMode = enBindingMode.TwoWay;
                        SourceObject.AddActionBind(SourcePropertyInfo.Name, info);

                        info = new ControlPropertyBindInfo(hostObject.Id, hostProperty.Name);
                        info.SourceObject = SourceObject;
                        info.SourcePropertyInfo = SourcePropertyInfo;
                        info.BindingMode = BindingMode;
                        hostObject.AddActionBind(hostProperty.Name, info);
                    }
                    break;
            }

            return true;
        }

        public override string ToString()
        {
            if (SourceObject == null || SourcePropertyInfo == null)
                return mParseDataString;
            return SourceObjectId + "|" + SourcePropertyInfo.Name + "|" + BindingMode.ToString();
        }

        public override object GetBindedPropertyValue()
        {
            if (SourceObject == null || SourcePropertyInfo == null)
                return null;

            return SourcePropertyInfo.GetValue(SourceObject);
        }

        public override void SetBindedPropertyValue(UIBindAutoUpdate bindObj, object value)
        {
            if (BindingMode == enBindingMode.OneWay)
                return;

            if (SourceObject == null || SourcePropertyInfo == null)
                return;

            var proValue = CCore.Support.ReflectionManager.Instance.GetValueForType(SourcePropertyInfo.PropertyType, SourcePropertyInfo.GetValue(SourceObject));
            var valueToType = CCore.Support.ReflectionManager.Instance.GetValueForType(SourcePropertyInfo.PropertyType, value);

            if (!Object.Equals(proValue, valueToType))
                SourcePropertyInfo.SetValue(SourceObject, valueToType);
        }
    }
}
