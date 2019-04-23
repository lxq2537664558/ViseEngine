using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem.Bind
{
    public class ControlBindInfo
    {
        WinBase mBindedControl = null;
        public WinBase BindedControl
        {
            get { return mBindedControl; }
        }

        PropertyDescriptor mBindedProperty = null;
        public PropertyDescriptor BindedProperty
        {
            get { return mBindedProperty; }
        }

        List<PropertyBindInfo> mBindInfoList = new List<PropertyBindInfo>();
        public List<PropertyBindInfo> BindInfoList
        {
            get { return mBindInfoList; }
        }

        public string Key
        {
            get
            {
                string retStr = "";

                if (BindedControl != null)
                    retStr += BindedControl.Id.ToString() + "_";
                if (BindedProperty != null)
                    retStr += BindedProperty.Name;

                return retStr;
            }
        }

        public ControlBindInfo(WinBase bindControl, PropertyDescriptor bindProperty)
        {
            mBindedControl = bindControl;
            mBindedProperty = bindProperty;
        }
    }

    // 窗口内部控件之间属性绑定的管理器
    public class PropertyBindManager
    {
        WinBase mHostControl;
        public WinBase HostControl
        {
            get { return mHostControl; }
        }

        public bool IsHaveBind
        {
            get
            {
                return mControlBindInfoDictionary.Count > 0;
            }
        }

        Dictionary<string, ControlBindInfo> mControlBindInfoDictionary = new Dictionary<string, ControlBindInfo>();

        public PropertyBindManager(WinBase hostControl)
        {
            mHostControl = hostControl;
        }

        public void AddBindInfo(ControlBindInfo info)
        {
            mControlBindInfoDictionary[info.Key] = info;
        }

        public ControlBindInfo GetBindInfo(Guid bindedControlId, string propertyName)
        {
            ControlBindInfo retInfo = null;
            mControlBindInfoDictionary.TryGetValue(bindedControlId + "_" + propertyName, out retInfo);
            return retInfo;
        }

        public void BuildBindings(WinBase rootControl)
        {
            foreach (var bindInfo in mControlBindInfoDictionary.Values)
            {
                foreach (var cpInfo in bindInfo.BindInfoList)
                {
                    //if (cpInfo is ClassPropertyBindInfo)
                    //{
                    //    ClassPropertyBindInfo.Parse(rootControl, cpInfo);
                    //}
                    //else if (cpInfo is ControlPropertyBindInfo)
                    //{
                    //    ControlPropertyBindInfo.Parse(rootControl, cpInfo);
                    //}
                    cpInfo.Parse(rootControl);
                }
            }
        }
    }
}
