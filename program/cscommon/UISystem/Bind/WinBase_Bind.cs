using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem
{
    public partial class WinBase
    {
        // 事件绑定---------------------------------------
#region 事件绑定
        
        protected Dictionary<string, string> mEventBindInfoDictionary = new Dictionary<string, string>();
        public void SetBindingEventInfo(string eventName, string dllName, string className, string methodName)
        {
            mEventBindInfoDictionary[eventName] = dllName + "@" + className + "|" + methodName;
        }
        public bool GetBindingEventInfoFromEventName(string eventName, ref string dllName, ref string className, ref string methodName)
        {
            // xx.dll@className|methodname
            string outStr = "";
            if (!mEventBindInfoDictionary.TryGetValue(eventName, out outStr))
                return false;

            return GetEventBindingInfoFromString(outStr, ref dllName, ref className, ref methodName);
        }

        protected bool GetEventBindingInfoFromString(string strInfo, ref string dllName, ref string className, ref string methodName)
        {
            if (string.IsNullOrEmpty(strInfo))
                return false;

            // xx.dll@className|methodname
            var splitsDll = strInfo.Split('@');
            if (splitsDll.Length < 2)
                return false;

            dllName = splitsDll[0];

            var classSplits = splitsDll[1].Split('|');
            if (classSplits.Length < 2)
                return false;

            className = classSplits[0];
            methodName = classSplits[1];

            return true;
        }

        protected virtual void SaveEventBindInfo(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            var events = this.GetType().GetEvents();
            foreach (var evt in events)
            {
                var attributes = evt.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_BindingEventAttribute), true);
                if (attributes.Length <= 0)
                    continue;

                string eventBindInfo = "";
                if (!mEventBindInfoDictionary.TryGetValue(evt.Name, out eventBindInfo))
                    continue;

                pXml.AddAttrib(evt.Name, eventBindInfo);
            }
        }

        protected virtual void LoadEventBindInfo(CSUtility.Support.XmlNode pXml)
        {
            mEventBindInfoDictionary.Clear();
            var events = this.GetType().GetEvents();
            foreach (var evt in events)
            {
                var attributes = evt.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_BindingEventAttribute), true);
                if (attributes.Length <= 0)
                    continue;

                var attr = pXml.FindAttrib(evt.Name);
                if (attr == null)
                    continue;

                string dllName = "", className = "", methodName = "";
                if (!GetEventBindingInfoFromString(attr.Value, ref dllName, ref className, ref methodName))
                    continue;

                var assembly = CCore.Support.ReflectionManager.Instance.GetAssembly(dllName);
                if (assembly == null)
                    continue;

                var classType = assembly.GetType(className);
                if (classType == null)
                    continue;

                //SetBindingEventInfo(evt.Name, dllName, className, methodName);
                mEventBindInfoDictionary[evt.Name] = attr.Value;

                System.Reflection.MethodInfo invoke = evt.EventHandlerType.GetMethod("Invoke");
                var delegateParams = invoke.GetParameters();
                Type[] handerTypes = new Type[delegateParams.Length];
                //foreach (var paramInfo in delegateParams)
                for (int i = 0; i < delegateParams.Length; i++ )
                {
                    handerTypes[i] = delegateParams[i].ParameterType;
                }
                var handler = classType.GetMethod(methodName, handerTypes);
                if(handler==null)
                    continue;

                var targetInstance = CCore.Support.ReflectionManager.Instance.GetClassObject(classType);
                if (handler != null && targetInstance!=null)
                {   
                    Delegate d = Delegate.CreateDelegate(evt.EventHandlerType, targetInstance, handler);
                    evt.AddEventHandler(this, d);
                }
            }
        }

#endregion
        // 属性绑定---------------------------------------  

#region 属性绑定
        
        // 记录绑定数据
        Dictionary<string, List<Bind.PropertyBindInfo>> mPropertyBindInfoDictionary = new Dictionary<string, List<Bind.PropertyBindInfo>>();
        
        //// 执行绑定主动更新的字典表
        //Dictionary<string, List<Bind.PropertyBindInfo>> mPropertyBindInfoDictionaryAction = new Dictionary<string, List<Bind.PropertyBindInfo>>();
        //public void AddActionBind(string propertyName, Bind.PropertyBindInfo bindInfo)
        //{
        //    if (bindInfo == null)
        //        return;

        //    List<Bind.PropertyBindInfo> bindList = null;
        //    if (!mPropertyBindInfoDictionaryAction.TryGetValue(propertyName, out bindList))
        //    {
        //        bindList = new List<Bind.PropertyBindInfo>();
        //        mPropertyBindInfoDictionaryAction[propertyName] = bindList;
        //    }

        //    if (!string.IsNullOrEmpty(bindInfo.ToString()))
        //    {
        //        // 避免重复值
        //        foreach (var info in bindList)
        //        {
        //            if (info.ToString() == bindInfo.ToString())
        //            {
        //                bindList.Remove(info);
        //                break;
        //            }
        //        }
        //    }

        //    bindList.Add(bindInfo);
        //}

        //// 执行绑定被动更新的字典表
        //Dictionary<string, List<Bind.PropertyBindInfo>> mPropertyBindInfoDictionaryPassive = new Dictionary<string, List<Bind.PropertyBindInfo>>();
        //public void AddPassiveBind(string propertyName, Bind.PropertyBindInfo bindInfo)
        //{
        //    if (bindInfo == null)
        //        return;

        //    List<Bind.PropertyBindInfo> bindList = null;
        //    if (!mPropertyBindInfoDictionaryPassive.TryGetValue(propertyName, out bindList))
        //    {
        //        bindList = new List<Bind.PropertyBindInfo>();
        //        mPropertyBindInfoDictionaryPassive[propertyName] = bindList;
        //    }

        //    if (!string.IsNullOrEmpty(bindInfo.ToString()))
        //    {
        //        foreach (var info in bindList)
        //        {
        //            if (info.ToString() == bindInfo.ToString())
        //            {
        //                bindList.Remove(info);
        //                break;
        //            }
        //        }
        //    }

        //    bindList.Add(bindInfo);
        //}
        
        //Dictionary<string, List<Bind.ClassPropertyBindInfo>> mClassPropertyBindInfoDictionary = new Dictionary<string, List<Bind.ClassPropertyBindInfo>>();
        public void AddPropertyBind(string propertyName, Bind.PropertyBindInfo bindInfo)
        {
            if (bindInfo == null)
                return;

            List<Bind.PropertyBindInfo> bindList = null;
            if (!mPropertyBindInfoDictionary.TryGetValue(propertyName, out bindList))
            {
                bindList = new List<Bind.PropertyBindInfo>();
                mPropertyBindInfoDictionary[propertyName] = bindList;
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

            //if (bindInfo == null)
            //    mClassPropertyBindInfoDictionary.Remove(propertyName);
            //else
            //    mClassPropertyBindInfoDictionary[propertyName] = bindInfo;
        }

        public void RemovePropertyBind(string propertyName, Bind.PropertyBindInfo bindInfo)
        {
            if (bindInfo == null)
                return;

            List<Bind.PropertyBindInfo> bindList = null;
            if (mPropertyBindInfoDictionary.TryGetValue(propertyName, out bindList))
            {
                bindList.Remove(bindInfo);
            }
        }

        public List<Bind.PropertyBindInfo> GetPropertyBinds(string propertyName)
        {
            List<Bind.PropertyBindInfo> retValue = null;
            mPropertyBindInfoDictionary.TryGetValue(propertyName, out retValue);
            return retValue;
        }

        protected virtual void SavePropertyBindInfo(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            if (mPropertyBindInfoDictionary.Count > 0)
            {
                var classBindNode = pXml.AddNode("PropertyBind", "",holder);
                foreach (var bindInfo in mPropertyBindInfoDictionary)
                {
                    var bindInfoNode = classBindNode.AddNode("BindInfo", "",holder);
                    bindInfoNode.AddAttrib("Property", bindInfo.Key);

                    foreach (var bind in bindInfo.Value)
                    {
                        if (bind is Bind.ClassPropertyBindInfo)
                        {
                            var bindNode = bindInfoNode.AddNode("ClassBind", "",holder);
                            bindNode.AddAttrib("Value", bind.ToString());
                        }
                        else if (bind is Bind.ControlPropertyBindInfo)
                        {
                            var bindNode = bindInfoNode.AddNode("ControlBind", "",holder);
                            bindNode.AddAttrib("Value", bind.ToString());
                        }
                    }
                    //classBindNode.AddAttrib(bindInfo.Key + ".Bind", bindInfo.Value.ToString());
                }
            }
        }

        protected virtual void LoadPropertyBindInfo(CSUtility.Support.XmlNode pXml)
        {
            //mClassPropertyBindInfoDictionary.Clear();
            //foreach (var property in this.GetType().GetProperties())
            //{
            //    var attributes = property.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_BindingPropertyAttribute), true);
            //    if (attributes.Length <= 0)
            //        continue;

            //    var attr = pXml.FindAttrib(property.Name + ".Bind");
            //    if (attr == null)
            //        continue;

            //    Bind.ClassPropertyBindInfo bindInfo = new Bind.ClassPropertyBindInfo();
            //    if (!bindInfo.Parse(attr.Value))
            //        continue;

            //    mClassPropertyBindInfoDictionary[property.Name] = bindInfo;
            //}
            var classBindNode = pXml.FindNode("PropertyBind");
            if (classBindNode == null)
                return;

            Bind.PropertyBindManager rootControlBindManager = null;
            var rootControl = this.GetRoot(typeof(Template.ControlTemplate));
            if(rootControl == null)
            {
                rootControl = this.GetRoot(typeof(WinForm));
            }

            if (rootControl is Template.ControlTemplate)
            {
                rootControlBindManager = ((Template.ControlTemplate)rootControl).PropertyBindManager;
            }
            else if (rootControl is WinForm)
            {
                rootControlBindManager = ((WinForm)rootControl).ControlBindManager;
            }

            foreach (var bindInfoNode in classBindNode.FindNodes("BindInfo"))
            {
                var attr = bindInfoNode.FindAttrib("Property");
                if(attr == null)
                    continue;

                var propertyName = attr.Value;
                var property = TypeDescriptor.GetProperties(this)[propertyName];
                if(property == null)
                    continue;
                foreach (var bindNode in bindInfoNode.FindNodes("ClassBind"))
                {
                    var bindAttr = bindNode.FindAttrib("Value");
                    if (bindAttr == null)
                        continue;

                    var info = new Bind.ClassPropertyBindInfo(this.Id, property.Name);
                    info.mParseDataString = bindAttr.Value;
                    //if (!info.Parse(bindAttr.Value))
                    //    continue;

                    AddPropertyBind(propertyName, info);

                    var cbInfo = rootControlBindManager.GetBindInfo(this.Id, propertyName);
                    if (cbInfo == null)
                    {
                        cbInfo = new Bind.ControlBindInfo(this, property);
                        rootControlBindManager.AddBindInfo(cbInfo);
                    }
                    cbInfo.BindInfoList.Add(info);
                }
                foreach (var bindNode in bindInfoNode.FindNodes("ControlBind"))
                {
                    var bindAttr = bindNode.FindAttrib("Value");
                    if (bindAttr == null)
                        continue;

                    var info = new Bind.ControlPropertyBindInfo(this.Id, property.Name);
                    info.mParseDataString = bindAttr.Value;

                    AddPropertyBind(propertyName, info);

                    var cbInfo = rootControlBindManager.GetBindInfo(this.Id, propertyName);
                    if (cbInfo == null)
                    {
                        cbInfo = new Bind.ControlBindInfo(this, property);
                        rootControlBindManager.AddBindInfo(cbInfo);
                    }
                    cbInfo.BindInfoList.Add(info);
                }
            }
        }

#endregion
        
        // 从绑定数据更新UI上的数据
        public void UpdateUI()
        {
            foreach (var bindInfo in mPropertyBindInfoDictionaryPassive)
            {
                foreach (var bind in bindInfo.Value)
                {
                    if (bind.BindingMode == Bind.enBindingMode.OneWayToSource)
                        continue;

                    var property = this.GetType().GetProperty(bindInfo.Key);
                    if (property == null)
                        continue;

                    var bindPropertyValue = CCore.Support.ReflectionManager.Instance.GetValueForType(property.PropertyType, bind.GetBindedPropertyValue());
                    property.SetValue(this, bindPropertyValue, null);
                    //if (property.PropertyType == typeof(System.String))
                    //    property.SetValue(this, bindPropertyValue.ToString(), null);
                    //else// if(property.PropertyType == bindPropertyValue.GetType())
                    //    property.SetValue(this, info.Value.GetBindedPropertyValue(), null);
                }
            }

            //foreach (var bindInfo in mControlPropertyBindInfoDictionary)
            //{
            //    foreach (var bind in bindInfo.Value)
            //    {
            //        //if (bind.BindingMode == Bind.enBindingMode.OneWayToSource)
            //        //    continue;

            //        var property = this.GetType().GetProperty(bindInfo.Key);
            //        if (property == null)
            //            continue;

            //        var bindPropertyValue = UISystem.UIReflectionManager.Instance.GetValueForType(property.PropertyType, bind.GetBindedPropertyValue());
            //        property.SetValue(this, bindPropertyValue, null);
            //    }
            //}

            foreach (var child in ChildWindows)
            {
                child?.UpdateUI();
            }
            //for (int i = 0; i < ChildWindows.Count; i++)
            //{
            //    WinBase win = null;
            //    try
            //    {
            //        win = ChildWindows[i];
            //    }
            //    catch (System.Exception)
            //    {
            //        continue;
            //    }
            //    if (win != null)
            //        win.UpdateUI();
            //}

        }

        //// 从UI数据更新绑定的数据
        //protected void UpdateBindValue(string propertyName)
        //{
        //    var property = this.GetType().GetProperty(propertyName);
        //    if (property == null)
        //        return;

        //    List<Bind.PropertyBindInfo> bindInfos = null;
        //    if (mPropertyBindInfoDictionaryAction.TryGetValue(propertyName, out bindInfos))
        //    {
        //        foreach (var bindInfo in bindInfos)
        //        {
        //            bindInfo.SetBindedPropertyValue(property.GetValue(this, null));
        //        }
        //    }

        //    //List<Bind.ControlPropertyBindInfo> ctrlBindInfos = null;
        //    //if (mControlPropertyBindInfoDictionary.TryGetValue(propertyName, out ctrlBindInfos))
        //    //{
        //    //    foreach (var bindInfo in ctrlBindInfos)
        //    //    {
        //    //        bindInfo.SetBindedPropertyValue(property.GetValue(this, null));
        //    //    }
        //    //}
        //}
    }
}
