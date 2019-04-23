using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AIEditor
{
    public class FSMTemplateInfo : INotifyPropertyChanged
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

        public class StateSwitchInfo
        {
            FSMTemplateInfo mHostFSM = null;
            public FSMTemplateInfo HostFSM
            {
                get { return mHostFSM; }
            }

            string mCurrentStateType = null;
            public string CurrentStateType
            {
                get { return mCurrentStateType; }
            }

            string mTargetStateType = null;
            public string TargetStateType
            {
                get { return mTargetStateType; }
            }

            string mNewCurrentStateType = null;
            public string NewCurrentStateType
            {
                get { return mNewCurrentStateType; }
                set { mNewCurrentStateType = value; }
            }

            string mNewTargetStateType = null;
            public string NewTargetStateType
            {
                get { return mNewTargetStateType; }
                set { mNewTargetStateType = value; }
            }

            public StateSwitchInfo(FSMTemplateInfo hostFSM, string curStateType, string tagStateType, string newCurStateType, string newTagStateType)
            {
                mHostFSM = hostFSM;
                mCurrentStateType = curStateType;
                mTargetStateType = tagStateType;
                NewCurrentStateType = newCurStateType;
                NewTargetStateType = newTagStateType;
            }

            public void ChangeStateName(string oldName, string newName)
            {
                if (mCurrentStateType == oldName)
                    mCurrentStateType = newName;
                if (mTargetStateType == oldName)
                    mTargetStateType = newName;
                if (mNewCurrentStateType == oldName)
                    mNewCurrentStateType = newName;
                if (mNewTargetStateType == oldName)
                    mNewTargetStateType = newName;
            }
        }
        public class StatePropertyInfo : INotifyPropertyChanged
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

            public delegate void Delegate_OnPropertyNameChange(string stateType, StatePropertyInfo info);
            public Delegate_OnPropertyNameChange OnPropertyNameChange;

            //public enum enStatePropertyCommonType
            //{
            //    Unknown,
            //    Boolean,
            //    Byte,
            //    UInt16,
            //    UInt32,
            //    UInt64,
            //    SByte,
            //    Int16,
            //    Int32,
            //    Int64,
            //    Float,
            //    Double,
            //    String,
            //}
            public static List<Type> mCommonTypes = new List<Type>(){ 
                                                         typeof(System.Boolean),
                                                         typeof(System.Byte),
                                                         typeof(System.UInt16),
                                                         typeof(System.UInt32),
                                                         typeof(System.UInt64),
                                                         typeof(System.SByte),
                                                         typeof(System.Int16),
                                                         typeof(System.Int32),
                                                         typeof(System.Int64),
                                                         typeof(System.Single),
                                                         typeof(System.Double),
                                                         typeof(System.String),
                                                         typeof(System.Object),};

            private string mHostStateType = null;
            private CSUtility.Helper.enCSType mCSType = CSUtility.Helper.enCSType.Common;
            public CSUtility.Helper.enCSType CSType
            {
                get { return mCSType; }
            }

            //enStatePropertyCommonType mStatePropertyTypeEnum = enStatePropertyCommonType.Unknown;
            //public enStatePropertyCommonType StatePropertyTypeEnum
            //{
            //    get { return mStatePropertyTypeEnum; }
            //    set
            //    {
            //        mStatePropertyTypeEnum = value;

            //        OnStatePropertyTypeEnumChanged(mStatePropertyTypeEnum);
            //    }
            //}

            string mPropertyName = "Unknown";
            public string PropertyName
            {
                get { return mPropertyName; }
                set
                {
                    mPropertyName = value;

                    if (OnPropertyNameChange != null)
                        OnPropertyNameChange(mHostStateType, this);
                }
            }

            Type mPropertyType = null;
            public Type PropertyType
            {
                get { return mPropertyType; }
                set
                {
                    mPropertyType = value;
                    OnStatePropertyTypeChanged(value);
                }
            }

            object mDefaultValue = "";
            public object DefaultValue
            {
                get { return mDefaultValue; }
                set
                {
                    mDefaultValue = GetValueWithType(PropertyType, value);
                    OnPropertyChanged("DefaultValue");
                }
            }

            public StatePropertyInfo(string hostStateType, CSUtility.Helper.enCSType csType)
            {
                mHostStateType = hostStateType;
                mCSType = csType;
            }

            private object GetValueWithType(Type valueType, object value)
            {
                if (valueType == typeof(System.Byte))
                    return System.Convert.ToByte(value);
                else if (valueType == typeof(System.Boolean))
                    return System.Convert.ToBoolean(value);
                else if (valueType == typeof(System.UInt16))
                    return System.Convert.ToUInt16(value);
                else if (valueType == typeof(System.UInt32))
                    return System.Convert.ToUInt32(value);
                else if (valueType == typeof(System.UInt64))
                    return System.Convert.ToUInt64(value);
                else if (valueType == typeof(System.SByte))
                    return System.Convert.ToSByte(value);
                else if (valueType == typeof(System.Int16))
                    return System.Convert.ToInt16(value);
                else if (valueType == typeof(System.Int32))
                    return System.Convert.ToInt32(value);
                else if (valueType == typeof(System.Int64))
                    return System.Convert.ToInt64(value);
                else if (valueType == typeof(System.Single))
                    return System.Convert.ToSingle(value);
                else if (valueType == typeof(System.Double))
                    return System.Convert.ToDouble(value);
                else if (valueType == typeof(System.String))
                    return System.Convert.ToString(value);

                return value;
            }

            private void OnStatePropertyTypeChanged(Type type)
            {
                if (type == typeof(System.Boolean))
                    DefaultValue = GetValueWithType(mPropertyType, false);
                else if (type == typeof(System.Byte))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.UInt16))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.UInt32))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.UInt64))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.SByte))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.Int16))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.Int32))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.Int64))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.Single))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.Double))
                    DefaultValue = GetValueWithType(mPropertyType, 0);
                else if (type == typeof(System.String))
                    DefaultValue = GetValueWithType(mPropertyType, "");
                else if (type.IsClass)
                    DefaultValue = null;
                else
                    DefaultValue = GetValueWithType(mPropertyType, null);
            }

            //private void OnStatePropertyTypeEnumChanged(enStatePropertyCommonType propertyTypeEnum)
            //{
            //    switch (propertyTypeEnum)
            //    {
            //        case enStatePropertyCommonType.Byte:
            //            mPropertyType = typeof(System.Byte);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.Byte)0;
            //            break;
            //        case enStatePropertyCommonType.Boolean:
            //            mPropertyType = typeof(System.Boolean);
            //            DefaultValue = GetValueWithType(mPropertyType, false);//(System.Boolean)false;
            //            break;
            //        case enStatePropertyCommonType.UInt16:
            //            mPropertyType = typeof(System.UInt16);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//System.UInt16)0;
            //            break;
            //        case enStatePropertyCommonType.UInt32:
            //            mPropertyType = typeof(System.UInt32);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.UInt32)0;
            //            break;
            //        case enStatePropertyCommonType.UInt64:
            //            mPropertyType = typeof(System.UInt64);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.UInt64)0;
            //            break;
            //        case enStatePropertyCommonType.SByte:
            //            mPropertyType = typeof(System.SByte);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.SByte)0;
            //            break;
            //        case enStatePropertyCommonType.Int16:
            //            mPropertyType = typeof(System.Int16);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.Int16)0;
            //            break;
            //        case enStatePropertyCommonType.Int32:
            //            mPropertyType = typeof(System.Int32);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.Int32)0;
            //            break;
            //        case enStatePropertyCommonType.Int64:
            //            mPropertyType = typeof(System.Int64);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.Int64)0;
            //            break;
            //        case enStatePropertyCommonType.Float:
            //            mPropertyType = typeof(System.Single);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.Single)0;
            //            break;
            //        case enStatePropertyCommonType.Double:
            //            mPropertyType = typeof(System.Double);
            //            DefaultValue = GetValueWithType(mPropertyType, "0");//(System.Double)0;
            //            break;
            //        case enStatePropertyCommonType.String:
            //            mPropertyType = typeof(System.String);
            //            DefaultValue = GetValueWithType(mPropertyType, "");//"";
            //            break;
            //    }
            //}
        }

        public class StateSpecialParams
        {
            public string BaseType = null;
            public string NickName = "";
            public List<StatePropertyInfo> StatePropertyInfos = new List<StatePropertyInfo>();
        }

        public FSMTemplateInfo(Guid id)
        {
            mId = id;
        }

        private Guid mId;
        public Guid Id
        {
            get { return mId; }
        }

        //private UInt64 mVersion = 0;
        //public UInt64 Version
        //{
        //    get { return mVersion; }
        //    set
        //    {
        //        mVersion = value;
        //        OnPropertyChanged("Version");
        //    }
        //}

        private bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
                OnPropertyChanged("IsDirty");
            }
        }

        private string mName = "UnknowFSM";
        public string Name
        {
            get { return mName; }
            set
            {
                if (mName == value)
                    return;

                mName = value;

                IsDirty = true;
                OnPropertyChanged("Name");
            }
        }

        private string mInfoString = "0个状态";
        public string InfoString
        {
            get { return mInfoString; }
            set
            {
                mInfoString = value;
                OnPropertyChanged("InfoString");
            }
        }

        public string[] StateTypes
        {
            get
            {
                List<string> retList = new List<string>();
                foreach (KeyValuePair<string, string> kvp in mStateSwitchManager.Keys)
                {
                    if(!retList.Contains(kvp.Key))
                        retList.Add(kvp.Key);
                }

                return retList.ToArray();
            }
        }

        private const string mFSMGridFileExp = "grid";
        private const string mMethodLinkExp = "method";

        private CSUtility.Support.XmlHolder mFSMGridXmlHolder;
        public CSUtility.Support.XmlHolder FSMGridXmlHolder
        {
            get { return mFSMGridXmlHolder; }
            set { mFSMGridXmlHolder = value; }
        }

        public string DefaultStateTypeName = "";

        // 状态基类信息
        private Dictionary<string, StateSpecialParams> mStateSpecialParamDictionary = new Dictionary<string, StateSpecialParams>();

        // 状态转换管理器
        Dictionary<KeyValuePair<string, string>, StateSwitchInfo> mStateSwitchManager = new Dictionary<KeyValuePair<string, string>, StateSwitchInfo>();
        public Dictionary<KeyValuePair<string, string>, StateSwitchInfo> StateSwitchManager
        {
            get { return mStateSwitchManager; }
        }
        // 获得状态转换信息
        public StateSwitchInfo GetStateSwitchInfo(string curState, string tarState)
        {
            StateSwitchInfo cvtStateInfo;
            if (mStateSwitchManager.TryGetValue(new KeyValuePair<string, string>(curState, tarState), out cvtStateInfo) == false)
                return null;
            return cvtStateInfo;
        }
        // 设置状态转换信息
        public void SetStateSwitchInfo(string curState, string tagState, StateSwitchInfo switchInfo)
        {
            var kvp = new KeyValuePair<string, string>(curState, tagState);
            mStateSwitchManager[kvp] = switchInfo;
        }

        private Dictionary<string, CSUtility.Support.XmlHolder> mStateMethodDelegateXmlHolders = new Dictionary<string, CSUtility.Support.XmlHolder>();
        public Dictionary<string, CSUtility.Support.XmlHolder> StateMethodDelegateXmlHolders
        {
            get { return mStateMethodDelegateXmlHolders; }
        }

        //public static string GetStateTypeAttributeName(Type stateType)
        //{
        //    if (stateType == null)
        //        return "";

        //    var atts = stateType.GetCustomAttributes(typeof(CSUtility.AISystem.Attribute.StatementClassAttribute), true);
        //    if (atts.Length > 0)
        //    {
        //        return ((CSUtility.AISystem.Attribute.StatementClassAttribute)atts[0]).m_strName;
        //    }

        //    return "";
        //}

        //public string GetStateTypeAttributeName(string strStateType)
        //{
        //    if (string.IsNullOrEmpty(strStateType))
        //        return "";

        //    StateSpecialParams stateTypeParam = null;
        //    if (mStateSpecialParamDictionary.TryGetValue(strStateType, out stateTypeParam))
        //    {
        //        return FSMTemplateInfo.GetStateTypeAttributeName(stateTypeParam.BaseType);
        //    }

        //    return "";
        //}

        public static string GetMethodDelegateDictionaryKey(string curType, string tagType, Type methodClassType, System.Reflection.MethodInfo methodInfo, CSUtility.Helper.enCSType csType)
        {
            if (string.IsNullOrEmpty(tagType))
            {
                if (methodInfo == null)
                    return csType.ToString() + "_" + curType + "_Default_";
                else
                    return csType.ToString() + "_" + curType + "_Default_" + methodInfo.Name;
            }

            return csType.ToString() + "_" + curType + "2" + tagType + "_" + methodClassType.ToString() + "_" + methodInfo.Name;
        }

        public static string GetStateChangeDictionaryKey(string curStateType, string tagStateType)
        {
            return curStateType + "2" + tagStateType;
        }

        public string GetStateBaseTypeName(string stateType)
        {
            if (string.IsNullOrEmpty(stateType))
                return "";

            StateSpecialParams retType = null;
            if(mStateSpecialParamDictionary.TryGetValue(stateType, out retType))
                return retType.BaseType;

            return "";
        }

        public Type GetStateBaseType(string stateType, CSUtility.Helper.enCSType csType)
        {
            if (string.IsNullOrEmpty(stateType))
                return null;

            StateSpecialParams retType = null;
            mStateSpecialParamDictionary.TryGetValue(stateType, out retType);
            
            var types = CSUtility.Program.GetInheritTypesFromType(retType.BaseType, csType);
            if (types == null || types.Count == 0)
            {
                return CSUtility.Program.GetTypeFromTypeFullName(retType.BaseType);
            }
            else
            {
                Type commonType = null;
                var attFullName = typeof(CSUtility.AISystem.Attribute.StatementClassAttribute).FullName;
                foreach(var type in types)
                {
                    var attCSType = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(type, attFullName, "CSType", false);
                    if (attCSType == null)
                        continue;
                    
                    if (csType.ToString().Equals(attCSType.ToString()))
                        return type;
                    if (attCSType.ToString().Equals(CSUtility.Helper.enCSType.Common.ToString()))
                        commonType = type;
                }

                if (commonType != null)
                    return commonType;

                return CSUtility.Program.GetTypeFromTypeFullName(retType.BaseType);
            }
            //return Program.GetType(retType.BaseType, csType);
        }

        public StateSpecialParams GetStateSpecialParams(string stateType)
        {
            if (string.IsNullOrEmpty(stateType))
                return null;

            StateSpecialParams retType = null;
            mStateSpecialParamDictionary.TryGetValue(stateType, out retType);

            return retType;
        }

        public string GetStateNickName(string stateType)
        {
            if (string.IsNullOrEmpty(stateType))
                return "";

            StateSpecialParams retType = null;
            if (mStateSpecialParamDictionary.TryGetValue(stateType, out retType))
                return retType.NickName;

            return "";
        }

        #region StateProperty处理

        //Dictionary<string, List<StatePropertyInfo>> mStatePropertyDictionary = new Dictionary<string, List<StatePropertyInfo>>();
        //public Dictionary<string, List<StatePropertyInfo>> StatePropertyDictionary
        //{
        //    get { return mStatePropertyDictionary; }
        //}
        protected void OnStatePropertyNameChanged(string stateType, StatePropertyInfo info)
        {
            StateSpecialParams ssp = null;
            if (mStateSpecialParamDictionary.TryGetValue(stateType, out ssp))
            {
                foreach (var proInfo in ssp.StatePropertyInfos)
                {
                    if (proInfo == info)
                        continue;

                    if (proInfo.PropertyName == info.PropertyName)
                    {
                        info.PropertyName = info.PropertyName + "_";
                        break;
                    }
                }
            }
        }

        public void AddStateProperty(string stateType, StatePropertyInfo info)
        {
            StateSpecialParams ssp = null;
            if (!mStateSpecialParamDictionary.TryGetValue(stateType, out ssp))
            {
                ssp = new StateSpecialParams();
                mStateSpecialParamDictionary[stateType] = ssp;
            }

            info.OnPropertyNameChange = OnStatePropertyNameChanged;
            ssp.StatePropertyInfos.Add(info);
        }

        public void RemoveStateProperty(string stateType, StatePropertyInfo info)
        {
            StateSpecialParams ssp = null;
            if (mStateSpecialParamDictionary.TryGetValue(stateType, out ssp))
            {
                ssp.StatePropertyInfos.Remove(info);
            }
        }
        public void RemoveStateProperty(string stateType, int index)
        {
            StateSpecialParams ssp = null;
            if (mStateSpecialParamDictionary.TryGetValue(stateType, out ssp))
            {
                if (index >= 0 && index < ssp.StatePropertyInfos.Count)
                {
                    ssp.StatePropertyInfos.RemoveAt(index);
                }
            }
        }

        public List<StatePropertyInfo> GetStatePropertys(string stateType)
        {
            StateSpecialParams ssp = null;
            if (!mStateSpecialParamDictionary.TryGetValue(stateType, out ssp))
            {
                ssp = new StateSpecialParams();
                mStateSpecialParamDictionary[stateType] = ssp;
            }

            return ssp.StatePropertyInfos;
        }

        #endregion

        #region 状态操作

        public void ChangeStateName(string oldName, string newName)
        {
            /*if (DefaultStateTypeName == oldName)
                DefaultStateTypeName = newName;

            List<KeyValuePair<string, string>> vpDelList = new List<KeyValuePair<string, string>>();
            foreach (var infoValuePair in mStateSwitchManager)
            {
                if (infoValuePair.Key.Key == oldName || infoValuePair.Key.Value == oldName)
                {
                    vpDelList.Add(infoValuePair.Key);
                }

                infoValuePair.Value.ChangeStateName(oldName, newName);
            }

            foreach (var pair in vpDelList)
            {
                if (pair.Key == oldName && pair.Value != oldName)
                {
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(newName, pair.Value);
                    var switchInfo = mStateSwitchManager[pair];
                    mStateSwitchManager.Remove(pair);
                    mStateSwitchManager[kvp] = switchInfo;
                }
                else if (pair.Key != oldName && pair.Value == oldName)
                {
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(pair.Key, newName);
                    var switchInfo = mStateSwitchManager[pair];
                    mStateSwitchManager.Remove(pair);
                    mStateSwitchManager[kvp] = switchInfo;
                }
                else if (pair.Key == oldName && pair.Value == oldName)
                {
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(newName, newName);
                    var switchInfo = mStateSwitchManager[pair];
                    mStateSwitchManager.Remove(pair);
                    mStateSwitchManager[kvp] = switchInfo;
                }
            }

            StateSpecialParams baseType = null;
            if (mStateSpecialParamDictionary.TryGetValue(oldName, out baseType))
            {
                mStateSpecialParamDictionary[newName] = baseType;
                mStateSpecialParamDictionary.Remove(oldName);
            }*/
        }

        public void ChangeNickName(string stateName, string newNickName)
        {
            StateSpecialParams outParam;
            if (mStateSpecialParamDictionary.TryGetValue(stateName, out outParam))
            {
                outParam.NickName = newNickName;
            }
        }

        public bool AddState(string stateType, string nickName, string baseType)
        {
            if (string.IsNullOrEmpty(stateType))// == null)
                return false;

            var stateTypes = StateTypes;

            if (stateTypes.Contains(stateType))
                return false;

            foreach (var sType in stateTypes)
            {
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(stateType, sType);
                StateSwitchInfo ssInfo = new StateSwitchInfo(this, stateType, sType, sType, "");
                mStateSwitchManager[kvp] = ssInfo;

                kvp = new KeyValuePair<string, string>(sType, stateType);
                ssInfo = new StateSwitchInfo(this, sType, stateType, stateType, "");
                mStateSwitchManager[kvp] = ssInfo;
            }

            var kvPair = new KeyValuePair<string, string>(stateType, stateType);
            StateSwitchInfo sswInfo = new StateSwitchInfo(this, stateType, stateType, stateType, "");
            mStateSwitchManager[kvPair] = sswInfo;
            
            StateSpecialParams ssp;
            if (!mStateSpecialParamDictionary.TryGetValue(stateType, out ssp))
            {
                ssp = new StateSpecialParams();
                mStateSpecialParamDictionary[stateType] = ssp;
            }
            ssp.BaseType = baseType;
            ssp.NickName = nickName;

            InfoString = StateTypes.Length + "个状态";

            return true;
        }

        public void RemoveState(string stateType)
        {
            var stateTypes = StateTypes;
            foreach (var sType in stateTypes)
            {
                var kvp = new KeyValuePair<string, string>(sType, stateType);
                mStateSwitchManager.Remove(kvp);

                kvp = new KeyValuePair<string, string>(stateType, sType);
                mStateSwitchManager.Remove(kvp);
            }

            var kvPair = new KeyValuePair<string, string>(stateType, stateType);
            mStateSwitchManager.Remove(kvPair);

            mStateSpecialParamDictionary.Remove(stateType);

            InfoString = StateTypes.Length + "个状态";
        }

        #endregion

        #region StateGrid

        private void LoadStateGrid(CSUtility.Support.XmlHolder xmlHolder)
        {
            if (xmlHolder == null)
                return;

            mStateSwitchManager.Clear();
            mStateSpecialParamDictionary.Clear();

            var nameAtt = xmlHolder.RootNode.FindAttrib("Name");
            if (nameAtt != null)
                Name = nameAtt.Value;
            //var verAtt = xmlHolder.RootNode.FindAttrib("Ver");
            //if (verAtt != null)
            //    Version = System.Convert.ToUInt64(verAtt.Value);

            var defStateNode = xmlHolder.RootNode.FindNode("DefaultState");
            if (defStateNode != null)
            {
                var att = defStateNode.FindAttrib("Type");
                if (att != null)
                    DefaultStateTypeName = att.Value;
            }

            foreach (var node in xmlHolder.RootNode.FindNodes("State"))
            {
                var att = node.FindAttrib("Type");
                if (att == null)
                    continue;
                var baseAtt = node.FindAttrib("BaseType");
                if (baseAtt == null)
                    continue;

                var nickNameAtt = node.FindAttrib("NickName");

                var curStateType = att.Value;//Type.GetType(att.Value); 
                //Type curStateBaseType = null;
                //var btSplits = baseAtt.Value.Split('|');
                //if (btSplits.Length == 2)
                //{
                //    System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(btSplits[0]);
                //    if (assembly != null)
                //        curStateBaseType = assembly.GetType(btSplits[1]);
                //}
                var curStateBaseType = baseAtt.Value;

                List<StatePropertyInfo> tempProList = new List<StatePropertyInfo>();

                var prosNode = node.FindNode("StatePropertys");
                if (prosNode != null)
                {
                    foreach (var proInfoNode in prosNode.FindNodes("Property"))
                    {
                        CSUtility.Helper.enCSType csType = CSUtility.Helper.enCSType.Common;
                        var proAtt = proInfoNode.FindAttrib("CSType");
                        if(proAtt != null)
                        {
                            System.Enum.TryParse<CSUtility.Helper.enCSType>(proAtt.Value, out csType);
                        }

                        StatePropertyInfo info = new StatePropertyInfo(curStateType, csType);

                        proAtt = proInfoNode.FindAttrib("Type");
                        if (proAtt != null)
                        {
                            var splits = proAtt.Value.Split('|');
                            if(splits.Length == 2)
                            {
                                var assembly = System.Reflection.Assembly.Load(splits[0]);
                                if(assembly != null)
                                {
                                    info.PropertyType = assembly.GetType(splits[1]);
                                }
                            }
                        }
                        //var proAtt = proInfoNode.FindAttrib("TypeEnum");
                        //if (proAtt != null)
                        //{
                        //    info.StatePropertyTypeEnum = (StatePropertyInfo.enStatePropertyCommonType)System.Enum.Parse(typeof(StatePropertyInfo.enStatePropertyCommonType), proAtt.Value);
                        //}

                        proAtt = proInfoNode.FindAttrib("Name");
                        if (proAtt != null)
                        {
                            info.PropertyName = proAtt.Value;
                        }

                        tempProList.Add(info);
                    }
                }
                
                StateSpecialParams ssp;
                if (!mStateSpecialParamDictionary.TryGetValue(curStateType, out ssp))
                {
                    ssp = new StateSpecialParams();
                    mStateSpecialParamDictionary[curStateType] = ssp;
                }
                ssp.BaseType = curStateBaseType;
                if (nickNameAtt != null)
                    ssp.NickName = nickNameAtt.Value;
                ssp.StatePropertyInfos = tempProList;
            }

            foreach (var node in xmlHolder.RootNode.FindNodes("StateSwitch"))
            {
                string fromState, toState, newCurrentState, newTargetState;

                var fromAtt = node.FindAttrib("FromState");
                if (fromAtt == null)
                    continue;
                fromState = fromAtt.Value;

                var toAtt = node.FindAttrib("ToState");
                if (toAtt == null)
                    continue;
                toState = toAtt.Value;

                var ncAtt = node.FindAttrib("NewCurrentState");
                if (ncAtt == null)
                    continue;
                newCurrentState = ncAtt.Value;

                var ntAtt = node.FindAttrib("NewTargetState");
                if (ntAtt == null)
                    continue;
                newTargetState = ntAtt.Value;

                StateSwitchInfo info = new StateSwitchInfo(this, fromState, toState, newCurrentState, newTargetState);
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(fromState, toState);
                StateSwitchManager[kvp] = info;
            }

            InfoString = StateTypes.Length + "个状态";
        }

        private CSUtility.Support.XmlHolder SaveStateGrid()
        {
            CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FSMGrid", "");

            xmlHolder.RootNode.AddAttrib("Name", Name);
            //xmlHolder.RootNode.AddAttrib("Ver", CSUtility.AISystem.FSMTemplateVersionManager.Instance.GetVersion(mId, CSUtility.Helper.enCSType.Client).ToString());//Version.ToString());

            var defStateNode = xmlHolder.RootNode.AddNode("DefaultState", "", xmlHolder);
            //string defaultStateTypeName = "AISystem.States.Idle";
            //if (DefaultStateType != null)
            //    defaultStateTypeName = DefaultStateType.ToString();
            defStateNode.AddAttrib("Type", DefaultStateTypeName);

            // 状态声明
            foreach (var sType in StateTypes)
            {
                var curStateNode = xmlHolder.RootNode.AddNode("State", "", xmlHolder);
                curStateNode.AddAttrib("Type", sType);
                var ssp = mStateSpecialParamDictionary[sType];
                curStateNode.AddAttrib("NickName", ssp.NickName);
                curStateNode.AddAttrib("BaseType", ssp.BaseType);//.Assembly.FullName + "|" + ssp.BaseType.FullName);

                var prosNode = curStateNode.AddNode("StatePropertys", "", xmlHolder);
                // 当前状态属性
                foreach (var proInfo in GetStatePropertys(sType))
                {
                    var proInfoNode = prosNode.AddNode("Property", "", xmlHolder);
                    proInfoNode.AddAttrib("CSType", proInfo.CSType.ToString());
                    proInfoNode.AddAttrib("Type", proInfo.PropertyType.Assembly.FullName + "|" + proInfo.PropertyType.FullName);
                    //proInfoNode.AddAttrib("TypeEnum", proInfo.StatePropertyTypeEnum.ToString());
                    proInfoNode.AddAttrib("Name", proInfo.PropertyName);
                }
            }

            // 状态转换
            foreach (var kvp in mStateSwitchManager)
            {
                var StateSwitchNode = xmlHolder.RootNode.AddNode("StateSwitch", "", xmlHolder);
                StateSwitchNode.AddAttrib("FromState", kvp.Key.Key);
                StateSwitchNode.AddAttrib("ToState", kvp.Key.Value);

                StateSwitchNode.AddAttrib("NewCurrentState", kvp.Value.NewCurrentStateType);
                StateSwitchNode.AddAttrib("NewTargetState", kvp.Value.NewTargetStateType);
            }

            return xmlHolder;
        }

        #endregion

        public void RemoveMethodDelegate(string key)
        {
            var dir = FSMTemplateInfoManager.Instance.GetFSMTemplateDirectory(mId);
            if (string.IsNullOrEmpty(dir))
                dir = CSUtility.Support.IFileConfig.DefaultFSMDirectory;
            mStateMethodDelegateXmlHolders.Remove(key);
            var fileName = CSUtility.Support.IFileManager.Instance.Root + dir + "\\" + key + "." + mMethodLinkExp;
            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
        }

        public delegate void Delegate_OnDelete(FSMTemplateInfo fsm, bool withSVN);
        public Delegate_OnDelete OnDelete;
        public void DeleteFiles(string strFolder, bool withSVN)
        {
            if (mId == Guid.Empty)
                return;

            if(OnDelete != null)
                OnDelete(this, withSVN);

            var dir = CSUtility.Support.IFileManager.Instance.Root + strFolder;

            if (withSVN)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"删除AI{Name} 目录{dir}使用版本控制删除失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                        {
                            if(resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"删除AI{Name} 目录{dir}使用版本控制删除失败!");
                            }
                        }, dir, $"AutoCommit 删除AI{Name}");
                    }
                }, dir);
            }

            if (System.IO.Directory.Exists(dir))
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(dir))
                {
                    System.IO.File.Delete(file);
                }
                System.IO.Directory.Delete(dir);
            }
        }


        // folder为相对Release的路径
        public bool LoadFSMTemplate(string folder)
        {
            if (mId == Guid.Empty)
                return false;

            var dir = CSUtility.Support.IFileManager.Instance.Root + folder;
            if (!System.IO.Directory.Exists(dir))
                return false;

            StateMethodDelegateXmlHolders.Clear();

            foreach (var file in System.IO.Directory.EnumerateFiles(dir))
            {
                //var expIdx = file.LastIndexOf('.');
                //var fileNameIdx = file.LastIndexOf('\\');
                //var fileName = file.Substring(fileNameIdx + 1, expIdx - fileNameIdx - 1);
                //var ext = file.Substring(file.LastIndexOf('.') + 1);
                var fileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(file);
                var ext = CSUtility.Support.IFileManager.Instance.GetFileExtension(file);
                fileName = fileName.Replace("." + ext, "");

                switch (ext)
                {
                    case mFSMGridFileExp:
                        {
                            mFSMGridXmlHolder = CSUtility.Support.XmlHolder.LoadXML(folder + "/" + fileName + "." + ext);
                            LoadStateGrid(mFSMGridXmlHolder);
                        }
                        break;

                    case mMethodLinkExp:
                        {
                            var holder = CSUtility.Support.XmlHolder.LoadXML(folder + "/" + fileName + "." + ext);
                            StateMethodDelegateXmlHolders[fileName] = holder;
                        }
                        break;
                }
            }

            IsDirty = false;

            return true;
        }

        public delegate bool Delegate_OnSave(FSMTemplateInfo fsm, bool withSVN);
        public Delegate_OnSave OnSave;
        public void SaveFSMTemplate(string folder, bool withSVN)
        {
            if (mId == Guid.Empty)
                return;

            // 每次保存都更新版本号
            //if (IsDirty)
            {
                CSUtility.AISystem.FSMTemplateVersionManager.Instance.VersionUpdate(mId, CSUtility.Helper.enCSType.Client);
                //Version += 1;
            }

            if ((OnSave?.Invoke(this, withSVN)) == false)
                return;

            var absFolder = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(folder);

            if (!System.IO.Directory.Exists(absFolder))
            {
                System.IO.Directory.CreateDirectory(absFolder);

                if (withSVN)
                {
                    EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                    {
                        if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"AI{Name} 目录{absFolder}使用版本控制增加失败!");
                        }
                    }, absFolder, $"AutoCommit 增加AI{Name}", true);
                }
            }
            //else
            //{
            //    if(withSVN)
            //    {
            //        SvnInterface.Commander.Instance.Update(absFolder);
            //    }
            //}

            mFSMGridXmlHolder = SaveStateGrid();
            var gridFile = folder + "\\FSMGrid." + mFSMGridFileExp;
            CSUtility.Support.XmlHolder.SaveXML(gridFile, mFSMGridXmlHolder, true);
            if (withSVN)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"AI{Name} 文件{gridFile}使用版本控制上传失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                        {
                            if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"AI{Name} 文件{gridFile}使用版本控制上传失败!");
                            }
                        }, CSUtility.Support.IFileManager.Instance.Root + gridFile, $"AutoCommit AI{Name} 上传二维状态表");
                    }
                }, CSUtility.Support.IFileManager.Instance.Root + gridFile);
            }

            foreach (var methodHolder in StateMethodDelegateXmlHolders)
            {
                if (methodHolder.Value != null)
                {
                    var fileName = folder + "\\" + methodHolder.Key + "." + mMethodLinkExp;
                    CSUtility.Support.XmlHolder.SaveXML(fileName, methodHolder.Value, true);

                    if(withSVN)
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"AI{Name} 文件{CSUtility.Support.IFileManager.Instance.Root + gridFile}使用版本控制上传失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                                {
                                    if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"AI{Name} 文件{CSUtility.Support.IFileManager.Instance.Root + gridFile}使用版本控制上传失败!");
                                    }
                                }, CSUtility.Support.IFileManager.Instance.Root + fileName, $"AutoCommit AI{Name} 上传函数文件{fileName}");
                            }
                        }, CSUtility.Support.IFileManager.Instance.Root + fileName);
                    }
                }
            }

            //if (withSVN)
            //{
            //    //var status = SvnInterface.Commander.Instance.CheckStatusEx(absFolder);
            //    //switch (status)
            //    //{
            //    //    case SvnInterface.SvnStatus.NotControl:
            //    //        {
            //    //            var addLog = SvnInterface.Commander.Instance.AddFolder(absFolder, true);
            //    //            System.Diagnostics.Debug.WriteLine(addLog);
            //    //        }
            //    //        break;
            //    //}

            //    var log = SvnInterface.Commander.Instance.Commit(absFolder);
            //    System.Diagnostics.Debug.WriteLine(log);
            //}


            IsDirty = false;
        }


    }
}
