using System;
using System.Collections.Generic;

namespace CCore.Support
{
    /// <summary>
    /// 绑定类的信息
    /// </summary>
    public class BindClassInfo
    {
        /// <summary>
        /// 类的类型
        /// </summary>
        public Type ClassType;
        /// <summary>
        /// 方法信息
        /// </summary>
        public List<System.Reflection.MethodInfo> MethodInfos = new List<System.Reflection.MethodInfo>();
        /// <summary>
        /// 属性信息
        /// </summary>
        public List<System.Reflection.PropertyInfo> PropertyInfos = new List<System.Reflection.PropertyInfo>();
    }
    /// <summary>
    /// 绑定命令信息
    /// </summary>
    public class BindCommandInfo
    {
        /// <summary>
        /// 目标控件
        /// </summary>
        public UISystem.UIInterface TargetControl;
        /// <summary>
        /// 方法信息列表
        /// </summary>
        public List<System.Reflection.MethodInfo> MethodInfos = new List<System.Reflection.MethodInfo>();
    }
    /// <summary>
    /// 反射管理器
    /// </summary>
    public sealed class ReflectionManager
    {
        static ReflectionManager mInstance = new ReflectionManager();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static ReflectionManager Instance
        {
            get { return mInstance; }
        }

        private ReflectionManager()
        {
            LoadFileDictionary();
        }
        /// <summary>
        /// 游戏结束时调用，释放指针
        /// </summary>
        public static void FinalInstance()
        {
            mInstance = null;
        }

        CSUtility.Support.ConcurentObjManager<string, System.Reflection.Assembly> mRegisteredAssemblys = new CSUtility.Support.ConcurentObjManager<string, System.Reflection.Assembly>();
        /// <summary>
        /// 注册应用程序构造块
        /// </summary>
        /// <param name="assembly">应用程序构造块</param>
        public void RegAssembly(System.Reflection.Assembly assembly)
        {
            mRegisteredAssemblys.Add(assembly.FullName, assembly);
        }
        /// <summary>
        /// 删除注册的应用程序构造块
        /// </summary>
        /// <param name="assembly">应用程序构造块</param>
        public void UnRegAssembly(System.Reflection.Assembly assembly)
        {
            mRegisteredAssemblys.Remove(assembly.FullName);
        }
        /// <summary>
        /// 清空注册的应用程序构造块
        /// </summary>
        public void ClearRegisteredAssemblys()
        {
            mRegisteredAssemblys.Clear();
        }

        #region ForEditor
        /// <summary>
        /// 获取相应类型的对象
        /// </summary>
        /// <param name="tagType">类对象的类型</param>
        /// <param name="value">对象实例</param>
        /// <returns>返回相应类型的对象</returns>
        public object GetValueForType(Type tagType, object value)
        {
            if (value == null)
                return null;

            if (tagType == value.GetType())
                return value;

            try
            {
                if (tagType == typeof(System.String))
                    return value.ToString();
                else if (tagType == typeof(System.Byte))
                    return System.Convert.ToByte(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.UInt16))
                    return System.Convert.ToUInt16(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.UInt32))
                    return System.Convert.ToUInt32(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.UInt64))
                    return System.Convert.ToUInt64(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.SByte))
                    return System.Convert.ToSByte(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.Int16))
                    return System.Convert.ToInt16(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.Int32))
                    return System.Convert.ToInt32(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.Int64))
                    return System.Convert.ToInt64(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.Single))
                    return System.Convert.ToSingle(System.Convert.ToDouble(value));
                else if (tagType == typeof(System.Double))
                    return System.Convert.ToDouble(value);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ReflectionManager.GetValueForType Exception:" + ex.ToString());
            }

            //else if (tagType.IsEnum)
            //{
            //    return 
            //}

            return null;
        }

        //protected Dictionary<Type, List<BindClassInfo>> mPropertyTypeBindDictionary = new Dictionary<Type, List<BindClassInfo>>();
        private Dictionary<System.ComponentModel.PropertyDescriptor, List<BindClassInfo>> mPropertyTypeBindDictionary = new Dictionary<System.ComponentModel.PropertyDescriptor, List<BindClassInfo>>();
        /// <summary>
        /// 获取相应属性类型的绑定的类的信息对象列表
        /// </summary>
        /// <param name="chkProInfo">成员属性信息对象</param>
        /// <returns>返回相应属性类型的绑定的类的信息对象列表</returns>
        public List<BindClassInfo> GetBindClassInfosWithPropertyType(System.ComponentModel.PropertyDescriptor chkProInfo)
        {
            List<BindClassInfo> retList = null;
            if (mPropertyTypeBindDictionary.TryGetValue(chkProInfo, out retList))
                return retList;

            retList = new List<BindClassInfo>();


            mRegisteredAssemblys.For_Each((string assemblyFullName, System.Reflection.Assembly assembly, object arg) =>
            {
                foreach (var classType in assembly.GetTypes())
                {
                    BindClassInfo bindClassInfo = new BindClassInfo();
                    bindClassInfo.ClassType = classType;

                    foreach (var propertyInfo in classType.GetProperties())
                    {
                        var attributes = propertyInfo.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_BindingPropertyAttribute), true);
                        if (attributes.Length <= 0)
                            continue;

                        if (propertyInfo.PropertyType != chkProInfo.PropertyType)
                        {
                            CSUtility.Editor.UIEditor_BindingPropertyAttribute chkBindAttribute = null;
                            foreach (var chkAttr in chkProInfo.Attributes)
                            {
                                if (chkAttr is CSUtility.Editor.UIEditor_BindingPropertyAttribute)
                                {
                                    chkBindAttribute = chkAttr as CSUtility.Editor.UIEditor_BindingPropertyAttribute;
                                    break;
                                }
                            }

                            if (chkBindAttribute == null || chkBindAttribute.AvailableTypes == null)
                                continue;

                            bool bFindType = false;
                            foreach (var chkType in chkBindAttribute.AvailableTypes)
                            {
                                if (chkType == propertyInfo.PropertyType)
                                {
                                    bFindType = true;
                                    break;
                                }
                            }

                            if (!bFindType)
                                continue;
                        }

                        bindClassInfo.PropertyInfos.Add(propertyInfo);
                    }

                    if (bindClassInfo.PropertyInfos.Count > 0)
                        retList.Add(bindClassInfo);
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            mPropertyTypeBindDictionary[chkProInfo] = retList;

            return retList;
        }

        private Dictionary<System.Reflection.EventInfo, List<BindClassInfo>> mEventTypeBindDictionary = new Dictionary<System.Reflection.EventInfo, List<BindClassInfo>>();
        /// <summary>
        /// 获取绑定相应事件的类对象信息
        /// </summary>
        /// <param name="info">事件信息</param>
        /// <returns>返回绑定相应事件的类对象信息</returns>
        public List<BindClassInfo> GetBindClassInfosWithEvent(System.Reflection.EventInfo info)
        {
            List<BindClassInfo> retList = null;
            if (mEventTypeBindDictionary.TryGetValue(info, out retList))
                return retList;

            retList = new List<BindClassInfo>();

            var evtInvoke = info.EventHandlerType.GetMethod("Invoke");
            var evtParamInfos = evtInvoke.GetParameters();

            mRegisteredAssemblys.For_Each((string assemblyFullName, System.Reflection.Assembly assembly, object arg) =>
            {
                foreach (var classType in assembly.GetTypes())
                {
                    BindClassInfo bindClassInfo = new BindClassInfo();
                    bindClassInfo.ClassType = classType;

                    foreach (var method in classType.GetMethods())
                    {
                        var attributes = method.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_BindingMethodAttribute), true);
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

                        bindClassInfo.MethodInfos.Add(method);
                    }

                    if (bindClassInfo.MethodInfos.Count > 0)
                        retList.Add(bindClassInfo);
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            mEventTypeBindDictionary[info] = retList;

            return retList;
        }

        private Dictionary<System.Reflection.EventInfo, List<BindCommandInfo>> mEventTypeBindCommandDictionary = new Dictionary<System.Reflection.EventInfo, List<BindCommandInfo>>();
        /// <summary>
        /// 获取绑定相应事件的命令信息
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="info">事件对象</param>
        /// <returns>返回绑定相应事件的命令信息</returns>
        public List<BindCommandInfo> GetBindCommandInfosWithEvent(UISystem.UIInterface control, System.Reflection.EventInfo info)
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

        private Dictionary<Type, List<BindClassInfo>> mPropertyAttributeClassInfoDictionary = new Dictionary<Type, List<BindClassInfo>>();
        /// <summary>
        /// 获取相应属性类型的类对象信息
        /// </summary>
        /// <param name="propertyAttType">属性类型</param>
        /// <returns>返回相应属性类型的类对象信息</returns>
        public List<BindClassInfo> GetClassInfosWithPropertyAttribute(Type propertyAttType)
        {
            List<BindClassInfo> retList = null;
            if (mPropertyAttributeClassInfoDictionary.TryGetValue(propertyAttType, out retList))
                return retList;

            retList = new List<BindClassInfo>();

            mRegisteredAssemblys.For_Each((string assemblyFullName, System.Reflection.Assembly assembly, object arg) =>
            {
                foreach (var classType in assembly.GetTypes())
                {
                    BindClassInfo bindClassInfo = new BindClassInfo();
                    bindClassInfo.ClassType = classType;

                    foreach (var propertyInfo in classType.GetProperties())
                    {
                        var attributes = propertyInfo.GetCustomAttributes(propertyAttType, true);
                        if (attributes.Length <= 0)
                            continue;

                        bindClassInfo.PropertyInfos.Add(propertyInfo);
                    }

                    if (bindClassInfo.PropertyInfos.Count > 0)
                        retList.Add(bindClassInfo);
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            mPropertyAttributeClassInfoDictionary[propertyAttType] = retList;

            return retList;
        }

        private Dictionary<Type, List<BindClassInfo>> mMethodAttributeClassInfoDictionary = new Dictionary<Type, List<BindClassInfo>>();
        /// <summary>
        /// 获取相应方法属性的类对象信息
        /// </summary>
        /// <param name="methodAttType">方法属性</param>
        /// <returns>返回相应方法属性的类对象信息</returns>
        public List<BindClassInfo> GetClassInfosWithMethodAttribute(Type methodAttType)
        {
            List<BindClassInfo> retList = null;
            if (mMethodAttributeClassInfoDictionary.TryGetValue(methodAttType, out retList))
                return retList;

            retList = new List<BindClassInfo>();

            mRegisteredAssemblys.For_Each((string assemblyFullName, System.Reflection.Assembly assembly, object arg) =>
            {
                foreach (var classType in assembly.GetTypes())
                {
                    BindClassInfo bindClassInfo = new BindClassInfo();
                    bindClassInfo.ClassType = classType;

                    foreach (var methodInfo in classType.GetMethods())
                    {
                        var attributes = methodInfo.GetCustomAttributes(methodAttType, true);
                        if (attributes.Length <= 0)
                            continue;

                        bindClassInfo.MethodInfos.Add(methodInfo);
                    }

                    if (bindClassInfo.MethodInfos.Count > 0)
                        retList.Add(bindClassInfo);
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            mMethodAttributeClassInfoDictionary[methodAttType] = retList;

            return retList;
        }

        #endregion

        private Dictionary<string, object> mClassObjectDictionary = new Dictionary<string, object>();
        private Dictionary<string, CSUtility.Support.XmlHolder> mUIStoreDictionary = new Dictionary<string, CSUtility.Support.XmlHolder>();
        private List<string> mLostUIForms = new List<string>();
        private Dictionary<string, UISystem.UIInterface> mUIFormDictionary = new Dictionary<string, UISystem.UIInterface>();
        private Dictionary<string, List<string>> mUIFormKeyDictionary = new Dictionary<string, List<string>>();
        Dictionary<string, string> mUIFileDic = new Dictionary<string, string>();
        /// <summary>
        /// 只读属性，UI文件
        /// </summary>
        public Dictionary<string, string> UIFileDic
        {
            get { return mUIFileDic; }
        }
        /// <summary>
        /// 删除所有对象
        /// </summary>
        public void Cleanup()
        {
            mClassObjectDictionary.Clear();
            mUIFormDictionary.Clear();
            mUIStoreDictionary.Clear();
        }

        private string GetFormFileName(string dir, string formName)
        {
            if (mUIFileDic.ContainsKey(formName))
            {
                var file = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mUIFileDic[formName]);
                if (!System.IO.File.Exists(file))
                {
                    RemoveUIFile(formName);
                }
                else
                {
                    return mUIFileDic[formName];
                }
            }

            dir = dir.ToLower();
            formName = formName.ToLower();

            var files = System.IO.Directory.GetFiles(dir, formName + ".xml", System.IO.SearchOption.AllDirectories);
            if (files.Length == 0)
                return "";
            
            var fileName = files[0].Replace(CSUtility.Support.IFileManager.Instance.Root, "");

            var name = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(files[0]);            
            SetUIFile(name,fileName);
            return fileName;
        }

        // UI窗口不能重名
        private UISystem.UIInterface RegisterUIForm(string formName, string instanceName)
        {
            // 遍历目录寻找UI xml，如果有重名又没有指定路径会有问题
            var dir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;
            var uiFormFile = GetFormFileName(dir, formName);
            uiFormFile = uiFormFile.ToLower();

            if (string.IsNullOrEmpty(uiFormFile))
                return null;

            CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.LoadXML(uiFormFile);
            var form = CreateUIFromXml(xmlHolder);
            if (form == null)
                return null;
            
            mUIStoreDictionary[formName] = xmlHolder;
            mUIFormDictionary[instanceName] = form;

            List<string> keyList = null;
            mUIFormKeyDictionary.TryGetValue(formName, out keyList);
            if (keyList == null)
            {
                keyList = new List<string>();
                mUIFormKeyDictionary[formName] = keyList;
            }

            keyList.Add(instanceName);
            
            return form;
        }

        private void RegisterUIForm(UISystem.UIInterface form, string formName, string instanceName)
        {
            mUIFormDictionary[instanceName] = form;

            List<string> keyList = null;
            mUIFormKeyDictionary.TryGetValue(formName, out keyList);
            if (keyList == null)
            {
                keyList = new List<string>();
                mUIFormKeyDictionary[formName] = keyList;
            }

            keyList.Add(instanceName);
        }

        private object RegisterClassObject(Type classType, string instanceName)
        {
            if (classType == null)
                return null;

            var classObject = classType.Module.Assembly.CreateInstance(classType.FullName);
            mClassObjectDictionary[instanceName] = classObject;
            return classObject;
        }
        /// <summary>
        /// 获取UI面板
        /// </summary>
        /// <param name="formName">面板名称</param>
        /// <param name="instanceName">实例对象名称</param>
        /// <returns>返回UI面板对象</returns>
        public UISystem.UIInterface GetUIForm(string formName, string instanceName = "")
        {
            if (string.IsNullOrEmpty(formName))
                return null;

            if (mLostUIForms.Contains(formName))
                return null;

            if (string.IsNullOrEmpty(instanceName))
                instanceName = "UI_" + formName;
            
            UISystem.UIInterface retForm = null;
            if (mUIFormDictionary.TryGetValue(instanceName, out retForm))
            {
                return retForm;
            }
            
            CSUtility.Support.XmlHolder xmlHolder;
            if (mUIStoreDictionary.TryGetValue(formName, out xmlHolder))
            {
                retForm = CreateUIFromXml(xmlHolder);
                mUIFormDictionary[instanceName] = retForm;
                return retForm;
            }
            
            retForm = RegisterUIForm(formName, instanceName);
            if(retForm == null)
            {
                mLostUIForms.Add(formName);
            }
            
            return retForm;
        }
        /// <summary>
        /// 从UI面板中删除相应的实例名称
        /// </summary>
        /// <param name="instanceName">实例对象名称</param>
        public void RemoveUIForm(string instanceName)
        {
            UISystem.UIInterface retForm = null;
            if (mUIFormDictionary.TryGetValue(instanceName, out retForm))
            {
                mUIFormDictionary.Remove(instanceName);
            }
        }
        /// <summary>
        /// 获取相应类型的类对象名称
        /// </summary>
        /// <param name="classType">类对象的类型</param>
        /// <returns>返回相应类型的类对象名称</returns>
        public static string GetDefaultClassInstanceName(Type classType)
        {
            return "Class_" + classType.FullName;
        }
        /// <summary>
        /// 获取相应实例对象名称的类型描述
        /// </summary>
        /// <param name="classType">类对象名称</param>
        /// <returns>返回相应实例对象名称的类型描述</returns>
        public static string GetDefaultClassInstanceName(string classType)
        {
            return "Class_" + classType;
        }
        /// <summary>
        /// 获取类定义的实例对象
        /// </summary>
        /// <param name="classType">类的类型</param>
        /// <param name="instanceName">实例的名称</param>
        /// <returns>返回对应的对象</returns>
        public object GetClassObject(Type classType, string instanceName = "")
        {
            if (classType == null)
                return null;

            if (string.IsNullOrEmpty(instanceName))
                instanceName = ReflectionManager.GetDefaultClassInstanceName(classType);//"Class_" + classType.FullName;

            object obj = null;
            lock (mClassObjectDictionary)
            {
                if (mClassObjectDictionary.TryGetValue(instanceName, out obj))
                    return obj;
                obj = RegisterClassObject(classType, instanceName);
            }

            return obj;
        }
        /// <summary>
        /// 获取类对象
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="instanceName">实例名称</param>
        /// <returns>返回对应的对象</returns>
        public T GetClassObject<T>(string instanceName = "")
        {
            return (T)GetClassObject(typeof(T), instanceName);
        }
        /// <summary>
        /// 获取一个程序集
        /// </summary>
        /// <param name="stDll">关键字名称</param>
        /// <returns>返回应用程序构造块</returns>
        public System.Reflection.Assembly GetAssembly(string stDll)
        {
            return CSUtility.Program.GetAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.Program.CurrentPlatform, stDll);
        }
        /// <summary>
        /// 更新UI
        /// </summary>
        public void UpdateUI()
        {
            var valueCollection = mUIFormDictionary.Values;
            foreach (var win in valueCollection)
            {
                win.UpdateUI();
            }
        }
        /// <summary>
        /// 重新加载UI
        /// </summary>
        /// <param name="formName">重新加载的UI的框架名称</param>
        public void ReloadUI(string formName)
        {
            List<string> keyList = null;
            mUIFormKeyDictionary.TryGetValue(formName, out keyList);
            if (keyList == null)
                return;

            //mUIStoreDictionary.Clear();

            foreach (var keyName in keyList)
            {
                var uiForm = GetUIForm(formName, keyName);
                if(uiForm.Parent==null)
                    continue;

                var formParent = uiForm.Parent;
                var visible = uiForm.Visibility;//.Visible;
                var index = formParent.IndexOfChild(uiForm);//.ChildWindows.IndexOf(uiForm);
                uiForm.Parent = null;

                //UIInterface tempForm = null;
                //if (mUIStoreDictionary.TryGetValue(formName, out tempForm))
                //{
                //    uiForm = tempForm.Clone();
                //}
                //else
                //{
                    var dir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultUIDirectory;
                    var uiFormFile = GetFormFileName(dir, formName);

                    if (string.IsNullOrEmpty(uiFormFile))
                        continue;

                    CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.LoadXML(uiFormFile);
                    uiForm = CreateUIFromXml(xmlHolder);//UIInterface.CreateFromXml(uiFormFile);
                    
                    if (uiForm == null)
                    {
                        mUIFormDictionary.Remove(keyName);
                        continue;
                    }

                    mUIStoreDictionary[formName] = xmlHolder;
                //    mUIStoreDictionary[formName] = uiForm;                
                //}
                
                uiForm.Parent = formParent;
                formParent.MoveChild(formParent.GetChildWinCount() - 1, index);
                //formParent.ChildWindows.Move(formParent.ChildWindows.Count - 1, index);
                //uiForm.Visible = visible;
                uiForm.Visibility = visible;
                mUIFormDictionary[keyName] = uiForm;

            }
        }
        /// <summary>
        /// 重新加载所有的UI
        /// </summary>
        public void ReloadAll()
        {
            foreach (var formName in mUIFormKeyDictionary.Keys)
            {
                ReloadUI(formName);
            }
        }
        #region UI
        /// <summary>
        /// 根据XML文件创建UI
        /// </summary>
        /// <param name="file">XML文件名称</param>
        /// <returns>返回UI界面</returns>
        public UISystem.UIInterface CreateUIFromXml(string file)
        {
            CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.LoadXML(file);

            return CreateUIFromXml(xmlHolder);
        }
        /// <summary>
        /// 根据XML节点创建UI
        /// </summary>
        /// <param name="xmlHolder">XML节点</param>
        /// <returns>返回创建的UI界面</returns>
        public UISystem.UIInterface CreateUIFromXml(CSUtility.Support.XmlHolder xmlHolder)
        {
            if (xmlHolder == null)
                return null;
            
            string dllKeyName = xmlHolder.RootNode.Name;//.Substring(0,xmlHolder.RootNode.Name.Length-1);
            var assembly = CSUtility.Program.GetAnalyseAssembly(CSUtility.Helper.enCSType.All, CSUtility.Program.CurrentPlatform, dllKeyName);
            if (assembly == null)
                return null;
            
            //string fullDllName = CSUtility.Support.IFileManager.Instance.Bin + dllKeyName;
            //fullDllName = fullDllName.Replace('/', '\\');
            //System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(fullDllName);
            var pChildWin = (UISystem.UIInterface)(assembly.CreateInstance(xmlHolder.RootNode.FindAttrib("TypeName").Value));
            if (pChildWin == null)
                return null;
            pChildWin.Load(xmlHolder.RootNode);
            
            return pChildWin;
        }

        #endregion

        string mFileName = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + "UIFileDic";
        /// <summary>
        /// 加载文件的Dictionary
        /// </summary>
        public void LoadFileDictionary()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(mFileName);
            if (xmlHolder == null)
                return;

            var nodes = xmlHolder.RootNode.FindNodes("FileNode");
            for (int i = 0; i < nodes.Count; i++)
            {
                string key = "";
                string file = "";

                var att = nodes[i].FindAttrib("key");
                if (att != null)
                    key = att.Value;

                att = nodes[i].FindAttrib("File");
                if (att != null)
                    file = att.Value;

                mUIFileDic[key] = file;
            }
        }
        /// <summary>
        /// 清空文件的Dictionary
        /// </summary>
        public void ClearFileDictionary()
        {
            mUIFileDic.Clear();
        }
        /// <summary>
        /// 保存文件的Dictionary
        /// </summary>
        public void SaveFileDictionary()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FileDic", "");

            foreach(var i in mUIFileDic)
            {
                var node = xmlHolder.RootNode.AddNode("FileNode", "", xmlHolder);
                node.AddAttrib("key", i.Key);
                node.AddAttrib("File", i.Value);
            }            
            CSUtility.Support.XmlHolder.SaveXML(mFileName, xmlHolder, true);
        }
        /// <summary>
        /// 删除UI文件
        /// </summary>
        /// <param name="file">UI文件名</param>
        public void RemoveUIFile(string file)
        {
            if (System.IO.Path.IsPathRooted(file))
                file = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
            mUIFileDic.Remove(file);
            SaveFileDictionary();
        }
        /// <summary>
        /// 设置UI文件
        /// </summary>
        /// <param name="id">UI的ID</param>
        /// <param name="file">文件名称</param>
        public void SetUIFile(string id, string file)
        {
            if (id.Contains(".xml"))
                id = id.Replace(".xml", "");
            file = file.Replace("\\", "/");                        
            if (System.IO.Path.IsPathRooted(file))
                file = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
            mUIFileDic[id] = file;
            SaveFileDictionary();
        }
    }
}
