using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem
{
    public class UIBindAutoUpdate
    {
        Dictionary<string, object> mPropertyOldValueDictionary = new Dictionary<string, object>();
        Dictionary<string, object> mPropertyNewValueDictionary = new Dictionary<string, object>();

        protected Guid mGuid = Guid.NewGuid();
        [Browsable(false)]
        public Guid Id
        {
            get { return mGuid; }
        }

        public void ResetId()
        {
            mGuid = Guid.NewGuid();
        }

        public object GetPropertyOldValue(string propertyName)
        {
            object retValue = null;
            mPropertyOldValueDictionary.TryGetValue(propertyName, out retValue);
            return retValue;
        }

        // 执行绑定主动更新的字典表
        protected Dictionary<string, List<Bind.PropertyBindInfo>> mPropertyBindInfoDictionaryAction = new Dictionary<string, List<Bind.PropertyBindInfo>>();
        public void AddActionBind(string propertyName, Bind.PropertyBindInfo bindInfo)
        {
            if (bindInfo == null)
                return;

            List<Bind.PropertyBindInfo> bindList = null;
            if (!mPropertyBindInfoDictionaryAction.TryGetValue(propertyName, out bindList))
            {
                bindList = new List<Bind.PropertyBindInfo>();
                mPropertyBindInfoDictionaryAction[propertyName] = bindList;
            }

            if (!string.IsNullOrEmpty(bindInfo.ToString()))
            {
                // 避免重复值
                foreach (var info in bindList)
                {
                    if (info.ToString() == bindInfo.ToString())
                    {
                        bindList.Remove(info);
                        break;
                    }
                }
            }

            bindList.Add(bindInfo);
        }
        // 执行绑定被动更新的字典表
        protected Dictionary<string, List<Bind.PropertyBindInfo>> mPropertyBindInfoDictionaryPassive = new Dictionary<string, List<Bind.PropertyBindInfo>>();
        public void AddPassiveBind(string propertyName, Bind.PropertyBindInfo bindInfo)
        {
            if (bindInfo == null)
                return;

            List<Bind.PropertyBindInfo> bindList = null;
            if (!mPropertyBindInfoDictionaryPassive.TryGetValue(propertyName, out bindList))
            {
                bindList = new List<Bind.PropertyBindInfo>();
                mPropertyBindInfoDictionaryPassive[propertyName] = bindList;
            }

            if (!string.IsNullOrEmpty(bindInfo.ToString()))
            {
                foreach (var info in bindList)
                {
                    if (info.ToString() == bindInfo.ToString())
                    {
                        bindList.Remove(info);
                        break;
                    }
                }
            }

            bindList.Add(bindInfo);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var property = TypeDescriptor.GetProperties(this)[propertyName];
            if (property == null)
                return;

            bool isEuqal = false;
            var newValue = property.GetValue(this);

            object oldValue;
            if (mPropertyNewValueDictionary.TryGetValue(propertyName, out oldValue))
            {
                if (object.Equals(oldValue, newValue))
                    isEuqal = true;
            }
            else
            {
                mPropertyNewValueDictionary[propertyName] = newValue;
            }

            if (!isEuqal)
            {
                mPropertyOldValueDictionary[propertyName] = mPropertyNewValueDictionary[propertyName];
                mPropertyNewValueDictionary[propertyName] = newValue;
                UpdateBindValue(propertyName);
            }
        }

        // 更新绑定的数据
        protected void UpdateBindValue(string propertyName)
        {
            var property = this.GetType().GetProperty(propertyName);
            if (property == null)
                return;

            List<Bind.PropertyBindInfo> bindInfos = null;
            if (mPropertyBindInfoDictionaryAction.TryGetValue(propertyName, out bindInfos))
            {
                foreach (var bindInfo in bindInfos)
                {
                    bindInfo.SetBindedPropertyValue(this, property.GetValue(this, null));
                }
            }
        }
    }

    //public class BindClassInfo
    //{
    //    public Type ClassType;
    //    public List<System.Reflection.MethodInfo> MethodInfos = new List<System.Reflection.MethodInfo>();
    //    public List<System.Reflection.PropertyInfo> PropertyInfos = new List<System.Reflection.PropertyInfo>();
    //}

    //public class BindCommandInfo
    //{
    //    public WinBase TargetControl;
    //    public List<System.Reflection.MethodInfo> MethodInfos = new List<System.Reflection.MethodInfo>();
    //}

    /*public class UIReflectionManager : Common.ReflectionManager
    {
        //static UIReflectionManager mInstance = new UIReflectionManager();
        //public static UIReflectionManager Instance
        //{
        //    get { return mInstance; }
        //}

//#region ForEditor

        //public object GetValueForType(Type tagType, object value)
        //{
        //    if (value == null)
        //        return null;

        //    if (tagType == value.GetType())
        //        return value;

        //    try
        //    {
        //        if (tagType == typeof(System.String))
        //            return value.ToString();
        //        else if (tagType == typeof(System.Byte))
        //            return System.Convert.ToByte(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.UInt16))
        //            return System.Convert.ToUInt16(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.UInt32))
        //            return System.Convert.ToUInt32(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.UInt64))
        //            return System.Convert.ToUInt64(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.SByte))
        //            return System.Convert.ToSByte(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.Int16))
        //            return System.Convert.ToInt16(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.Int32))
        //            return System.Convert.ToInt32(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.Int64))
        //            return System.Convert.ToInt64(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.Single))
        //            return System.Convert.ToSingle(System.Convert.ToDouble(value));
        //        else if (tagType == typeof(System.Double))
        //            return System.Convert.ToDouble(value);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("UIReflectionManager.GetValueForType Exception:" + ex.ToString());
        //    }

        //    //else if (tagType.IsEnum)
        //    //{
        //    //    return 
        //    //}

        //    return null;
        //}

        ////protected Dictionary<Type, List<BindClassInfo>> mPropertyTypeBindDictionary = new Dictionary<Type, List<BindClassInfo>>();
        //protected Dictionary<System.ComponentModel.PropertyDescriptor, List<BindClassInfo>> mPropertyTypeBindDictionary = new Dictionary<System.ComponentModel.PropertyDescriptor, List<BindClassInfo>>();
        //public List<BindClassInfo> GetBindClassInfosWithPropertyType(System.ComponentModel.PropertyDescriptor chkProInfo)
        //{
        //    List<BindClassInfo> retList = null;
        //    if (mPropertyTypeBindDictionary.TryGetValue(chkProInfo, out retList))
        //        return retList;

        //    retList = new List<BindClassInfo>();

        //    foreach (var classType in Program.GetTypes(Program.enAssemblyType.FrameSet))
        //    {
        //        BindClassInfo bindClassInfo = new BindClassInfo();
        //        bindClassInfo.ClassType = classType;

        //        foreach (var propertyInfo in classType.GetProperties())
        //        {
        //            var attributes = propertyInfo.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_BindingPropertyAttribute), true);
        //            if (attributes.Length <= 0)
        //                continue;

        //            if (propertyInfo.PropertyType != chkProInfo.PropertyType)
        //            {
        //                CSUtility.Editor.UIEditor_BindingPropertyAttribute chkBindAttribute = null;
        //                foreach (var chkAttr in chkProInfo.Attributes)
        //                {
        //                    if (chkAttr is CSUtility.Editor.UIEditor_BindingPropertyAttribute)
        //                    {
        //                        chkBindAttribute = chkAttr as CSUtility.Editor.UIEditor_BindingPropertyAttribute;
        //                        break;
        //                    }
        //                }

        //                if (chkBindAttribute == null || chkBindAttribute.AvailableTypes == null)
        //                    continue;

        //                bool bFindType = false;                        
        //                foreach (var chkType in chkBindAttribute.AvailableTypes)
        //                {
        //                    if (chkType == propertyInfo.PropertyType)
        //                    {
        //                        bFindType = true;
        //                        break;
        //                    }
        //                }

        //                if (!bFindType)
        //                    continue;
        //            }

        //            bindClassInfo.PropertyInfos.Add(propertyInfo);
        //        }

        //        if (bindClassInfo.PropertyInfos.Count > 0)
        //            retList.Add(bindClassInfo);
        //    }

        //    mPropertyTypeBindDictionary[chkProInfo] = retList;

        //    return retList;
        //}

        //protected Dictionary<System.Reflection.EventInfo, List<BindClassInfo>> mEventTypeBindDictionary = new Dictionary<System.Reflection.EventInfo, List<BindClassInfo>>();
        //public List<BindClassInfo> GetBindClassInfosWithEvent(System.Reflection.EventInfo info)
        //{
        //    List<BindClassInfo> retList = null;
        //    if (mEventTypeBindDictionary.TryGetValue(info, out retList))
        //        return retList;

        //    retList = new List<BindClassInfo>();

        //    var evtInvoke = info.EventHandlerType.GetMethod("Invoke");
        //    var evtParamInfos = evtInvoke.GetParameters();

        //    foreach (var classType in Program.GetTypes(Program.enAssemblyType.FrameSet))
        //    {
        //        BindClassInfo bindClassInfo = new BindClassInfo();
        //        bindClassInfo.ClassType = classType;

        //        foreach (var method in classType.GetMethods())
        //        {
        //            var attributes = method.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_BindingMethodAttribute), true);
        //            if (attributes.Length <= 0)
        //                continue;

        //            var methodParams = method.GetParameters();
        //            if (methodParams.Length != evtParamInfos.Length)
        //                continue;

        //            bool isEqual = true;
        //            for (int i = 0; i < methodParams.Length; i++)
        //            {
        //                if (methodParams[i].ParameterType != evtParamInfos[i].ParameterType)
        //                {
        //                    isEqual = false;
        //                    break;
        //                }
        //            }

        //            if (isEqual == false)
        //                continue;

        //            bindClassInfo.MethodInfos.Add(method);
        //        }

        //        if (bindClassInfo.MethodInfos.Count > 0)
        //            retList.Add(bindClassInfo);
        //    }

        //    mEventTypeBindDictionary[info] = retList;

        //    return retList;
        //}

        /*protected Dictionary<System.Reflection.EventInfo, List<BindCommandInfo>> mEventTypeBindCommandDictionary = new Dictionary<System.Reflection.EventInfo, List<BindCommandInfo>>();
        public List<BindCommandInfo> GetBindCommandInfosWithEvent(WinBase control, System.Reflection.EventInfo info)
        {
            List<BindCommandInfo> retList = null;
            if (mEventTypeBindCommandDictionary.TryGetValue(info, out retList))
                return retList;

            retList = new List<BindCommandInfo>();

            var evtInvoke = info.EventHandlerType.GetMethod("Invoke");
            var evtParamInfos = evtInvoke.GetParameters();

            var formChildControls = control.GetRoot().GetAllChildControls();
            foreach (var ctrl in formChildControls)
            {
                BindCommandInfo bindCmdInfo = new BindCommandInfo();
                bindCmdInfo.TargetControl = ctrl;

                foreach (var method in ctrl.GetType().GetMethods())
                {
                    var attributes = method.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_CommandMethodAttribute), true);
                    if (attributes.Length <= 0)
                        continue;

                    var methodParams = method.GetParameters();
                    if (methodParams.Length != evtParamInfos.Length)
                        continue;

                    bool isEqual = true;
                    for (int i = 0; i < methodParams.Length; i++)
                    {
                        if (methodParams[i].ParameterType != evtParamInfos[i].ParameterType)
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    if (isEqual == false)
                        continue;

                    bindCmdInfo.MethodInfos.Add(method);
                }

                if (bindCmdInfo.MethodInfos.Count > 0)
                    retList.Add(bindCmdInfo);
            }

            mEventTypeBindCommandDictionary[info] = retList;

            return retList;
        }

#endregion

        protected Dictionary<string, object> mClassObjectDictionary = new Dictionary<string, object>();
        protected Dictionary<string, WinBase> mUIFormDictionary = new Dictionary<string, WinBase>();
        protected Dictionary<string, List<string>> mUIFormKeyDictionary = new Dictionary<string, List<string>>();
        protected Dictionary<string, System.Reflection.Assembly> mAssemblyDictionary = new Dictionary<string, System.Reflection.Assembly>();

        protected string GetFormFileName(string dir, string formName)
        {
            var fileName = dir + "/" + formName + ".xml";
            if (System.IO.File.Exists(fileName))
                return fileName.Replace(CSUtility.Support.IFileManager.Instance.Root, "");
            else
            {
                foreach (var subDir in System.IO.Directory.EnumerateDirectories(dir))
                {
                    var retValue = GetFormFileName(subDir, formName);
                    if(!string.IsNullOrEmpty(retValue))
                        return retValue;
                }
            }

            return "";
        }

        // UI窗口不能重名
        protected WinBase RegisterUIForm(string formName, string instanceName)
        {
            // 遍历目录寻找UI xml，如果有重名又没有指定路径会有问题
            var dir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultUIDirectory;
            var uiFormFile = GetFormFileName(dir, formName);

            if (string.IsNullOrEmpty(uiFormFile))
                return null;

            var form = WinBase.CreateFromXml(uiFormFile);
            mUIFormDictionary[instanceName] = form;

            List<string> keyList = null;
            mUIFormKeyDictionary.TryGetValue(formName, out keyList);
            if(keyList == null)
            {
                keyList = new List<string>();
                mUIFormKeyDictionary[formName] = keyList;
            }

            keyList.Add(instanceName);

            return form;
        }

        protected object RegisterClassObject(Type classType, string instanceName)
        {
            if (classType == null)
                return null;

            var classObject = classType.Module.Assembly.CreateInstance(classType.FullName);
            mClassObjectDictionary[instanceName] = classObject;
            return classObject;
        }

        public WinBase GetUIForm(string formName, string instanceName = "")
        {
            if(string.IsNullOrEmpty(instanceName))
                instanceName = "UI_" + formName;

            WinBase retForm = null;
            if (mUIFormDictionary.TryGetValue(instanceName, out retForm))
            {
                return retForm;
            }

            retForm = RegisterUIForm(formName, instanceName);
            return retForm;
        }

        public object GetClassObject(Type classType, string instanceName = "")
        {
            if (classType == null)
                return null;

            if (string.IsNullOrEmpty(instanceName))
                instanceName = "Class_" + classType.FullName;

            object obj = null;
            if (mClassObjectDictionary.TryGetValue(instanceName, out obj))
                return obj;

            obj = RegisterClassObject(classType, instanceName);
            return obj;
        }

        public System.Reflection.Assembly GetAssembly(string stDll)
        {
            System.Reflection.Assembly assembly = null;
            if (mAssemblyDictionary.TryGetValue(stDll, out assembly))
                return assembly;

            assembly = System.Reflection.Assembly.LoadFrom(stDll);
            return assembly;
        }

        public void UpdateUI()
        {
            foreach (var win in mUIFormDictionary.Values)
            {
                win.UpdateUI();
            }
        }

        public void ReloadUI(string formName)
        {
            List<string> keyList = null;
            mUIFormKeyDictionary.TryGetValue(formName, out keyList);
            if(keyList == null)
                return;

            foreach (var keyName in keyList)
            {
                var uiForm = GetUIForm(formName, keyName);
                var formParent = uiForm.Parent;
                var visible = uiForm.Visibility;//.Visible;
                var index = formParent.IndexOfChild(uiForm);//.ChildWindows.IndexOf(uiForm);
                uiForm.Parent = null;

                var dir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultUIDirectory;
                var uiFormFile = GetFormFileName(dir, formName);

                if (string.IsNullOrEmpty(uiFormFile))
                    continue;

                uiForm = WinBase.CreateFromXml(uiFormFile);
                uiForm.Parent = formParent;
                formParent.MoveChild(formParent.GetChildWinCount() - 1, index);
                //formParent.ChildWindows.Move(formParent.ChildWindows.Count - 1, index);
                //uiForm.Visible = visible;
                uiForm.Visibility = visible;
                mUIFormDictionary[keyName] = uiForm;
            }
        }

        public void ReloadAll()
        {
            foreach (var formName in mUIFormKeyDictionary.Keys)
            {
                ReloadUI(formName);
            }
        }*/
    //}
}
