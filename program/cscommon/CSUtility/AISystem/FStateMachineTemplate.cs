using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CSUtility.AISystem
{
    /// <summary>
    /// 模板版本管理器
    /// </summary>
    public class FSMTemplateVersionManager
    {
        bool bLoaded = false;

        private static FSMTemplateVersionManager mInstance = new FSMTemplateVersionManager();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static FSMTemplateVersionManager Instance
        {
            get { return mInstance; }
        }

        Dictionary<Guid, UInt64> mFSMTemplateVersionDictionary = new Dictionary<Guid, UInt64>();
        /// <summary>
        /// 只读属性，模板版本表
        /// </summary>
        public Dictionary<Guid, UInt64> FSMTemplateVersionDictionary
        {
            get { return mFSMTemplateVersionDictionary; }
        }
        /// <summary>
        /// 加载模板
        /// </summary>
        /// <param name="csType">服务器客户端类型</param>
        public void Load(CSUtility.Helper.enCSType csType)
        {
            mFSMTemplateVersionDictionary.Clear();
            string dicFile = "";
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    {
                        var cDllDir = CSUtility.Support.IFileConfig.FSMDlls_Client_Directory;
                        dicFile = cDllDir + "\\FSMDic.dic";
                    }
                    break;
                case CSUtility.Helper.enCSType.Server:
                    {
                        var sDllDir = CSUtility.Support.IFileConfig.FSMDlls_Server_Directory;
                        dicFile = sDllDir + "\\FSMDic.dic";
                    }
                    break;
            }
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(dicFile);
            if (xmlHolder != null)
            {
                var nodes = xmlHolder.RootNode.FindNodes("FileNode");
                foreach (var node in nodes)
                {
                    var att = node.FindAttrib("key");
                    if (att == null)
                        continue;

                    var key = CSUtility.Support.IHelper.GuidParse(att.Value);

                    att = node.FindAttrib("Value");
                    if (att == null)
                        continue;

                    var value = System.Convert.ToUInt64(att.Value);

                    mFSMTemplateVersionDictionary[key] = value;
                }
            }

            bLoaded = true;
        }

        // Save只有编辑器调用，所以这里客户端和服务器端都做一次存储
        /// <summary>
        /// 保存模板
        /// </summary>
        public void Save()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FileDic", "");
            foreach (var fv in mFSMTemplateVersionDictionary)
            {
                var node = xmlHolder.RootNode.AddNode("FileNode", "", xmlHolder);
                node.AddAttrib("key", fv.Key.ToString());
                node.AddAttrib("Value", fv.Value.ToString());
            }            
            var cDllDir = CSUtility.Support.IFileConfig.FSMDlls_Client_Directory;
            var absCDllDir = FStateMachineTemplateManager.Instance.RootDirectory + cDllDir;
            if (!System.IO.Directory.Exists(absCDllDir))
            {
                System.IO.Directory.CreateDirectory(absCDllDir);
            }
            CSUtility.Support.XmlHolder.SaveXML(cDllDir + "\\FSMDic.dic", xmlHolder,false);

            var sDllDir = CSUtility.Support.IFileConfig.FSMDlls_Server_Directory;
            var absSDllDir = FStateMachineTemplateManager.Instance.RootDirectory + sDllDir;
            if (!System.IO.Directory.Exists(absSDllDir))
            {
                System.IO.Directory.CreateDirectory(absSDllDir);
            }
            CSUtility.Support.XmlHolder.SaveXML(sDllDir + "\\FSMDic.dic", xmlHolder, true);
        }
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <param name="csType">服务器客户端类型</param>
        /// <returns>返回相应的版本号</returns>
        public UInt64 GetVersion(Guid id, CSUtility.Helper.enCSType csType)
        {
            if (!bLoaded)
            {
                Load(csType);
            }

            UInt64 retValue;
            if (mFSMTemplateVersionDictionary.TryGetValue(id, out retValue))
                return retValue;

            mFSMTemplateVersionDictionary[id] = 0;

            return 0;
        }

        // 版本号更新
        /// <summary>
        /// 版本号更新
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <param name="csType">服务器客户端类型</param>
        public void VersionUpdate(Guid id, CSUtility.Helper.enCSType csType)
        {
            if (!bLoaded)
            {
                Load(csType);
            }

            UInt64 ver;
            if (mFSMTemplateVersionDictionary.TryGetValue(id, out ver))
            {
                mFSMTemplateVersionDictionary[id] = ver + 1;
            }
            else
                mFSMTemplateVersionDictionary[id] = 0;

            Save();
        }
        /// <summary>
        /// 删除FSM
        /// </summary>
        /// <param name="id">模板ID</param>
        public void RemoveFSM(Guid id)
        {
            if (mFSMTemplateVersionDictionary.ContainsKey(id))
            {
                mFSMTemplateVersionDictionary.Remove(id);
                Save();
            }
        }
    }
    /// <summary>
    /// 状态机模板
    /// </summary>
    public class FStateMachineTemplate : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用的方法
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        /// <summary>
        /// 声明编译FSM代码时调用的委托事件
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="csType">服务器客户端类型</param>
        /// <param name="bForceCompile">是否强制编译</param>
        /// <returns>编译成功返回true，否则返回false</returns>
        public delegate bool Delegate_OnCompileFSMCode(Guid templateId, int csType, bool bForceCompile);
        /// <summary>
        /// 定义编译FSM代码时调用的委托事件
        /// </summary>
        public static Delegate_OnCompileFSMCode OnCompileFSMCode;
        /// <summary>
        /// 状态转换数据类
        /// </summary>
        public class StateSwitchData
        {
            /// <summary>
            /// 转换信息类
            /// </summary>
            public class SwitchInfo
            {
                /// <summary>
                /// 新名称
                /// </summary>
                public string mNewCurrent;
                /// <summary>
                /// 新的目标
                /// </summary>
                public string mNewTarget;
                /// <summary>
                /// 带参构造函数
                /// </summary>
                /// <param name="newCurrent">新名称</param>
                /// <param name="newTarget">新的目标</param>
                public SwitchInfo(string newCurrent, string newTarget)
                {
                    mNewCurrent = newCurrent;
                    mNewTarget = newTarget;
                }
            }

            // 默认状态
            /// <summary>
            /// 默认状态,默认为待机状态
            /// </summary>
            protected string mDefaultState = "Idle";
            /// <summary>
            /// 只读属性，默认状态
            /// </summary>
            public string DefaultState
            {
                get { return mDefaultState; }
            }
            /// <summary>
            /// 状态转换数据列表
            /// </summary>
            protected Dictionary<KeyValuePair<string, string>, SwitchInfo> mStateSwitchInfoData = new Dictionary<KeyValuePair<string, string>, SwitchInfo>();
            /// <summary>
            /// 只读属性，状态转换数据列表
            /// </summary>
            public Dictionary<KeyValuePair<string, string>, SwitchInfo> StateSwitchInfoData
            {
                get { return mStateSwitchInfoData; }
            }
        }
        StateSwitchData mSwitchData = null;
        /// <summary>
        /// 只读属性，状态转换数据
        /// </summary>
        public StateSwitchData SwitchData
        {
            get { return mSwitchData; }
        }

        #region Assembly

        private string AssemblyFileName
        {
            get
            {
                return GetAssemblyFileName(Id, mCSType);// Id + "_V" + Version + ".dll";
            }
        }
        System.Reflection.Assembly mAssembly = null;
        /// <summary>
        /// 获取装配的文件名称
        /// </summary>
        /// <param name="id">对象ID</param>
        /// <param name="csType">客户端服务器类型</param>
        /// <param name="withOutVersionAndExt">是否不计算版本，默认为false</param>
        /// <returns>返回装配的文件名称</returns>
        public static string GetAssemblyFileName(Guid id, CSUtility.Helper.enCSType csType, bool withOutVersionAndExt = false)
        {
            if (withOutVersionAndExt)
            {
                if (CSUtility.Program.FinalRelease)
                    return id.ToString();
                else
                    return id + "_" + csType.ToString();
            }
            else
            {
                if (CSUtility.Program.FinalRelease)
                    return id + ".dll";
                else
                    return id + "_" + csType.ToString() + "_V" + FSMTemplateVersionManager.Instance.GetVersion(id, csType) + ".dll";
            }
        }
        /// <summary>
        /// 获取装配方向
        /// </summary>
        /// <param name="csType">客户端服务器类型</param>
        /// <returns>返回装配方向</returns>
        public static string GetAssemblyDir(CSUtility.Helper.enCSType csType)
        {
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    return FStateMachineTemplateManager.Instance.RootDirectory + CSUtility.Support.IFileConfig.FSMDlls_Client_Directory;

                case CSUtility.Helper.enCSType.Server:
                    return FStateMachineTemplateManager.Instance.RootDirectory + CSUtility.Support.IFileConfig.FSMDlls_Server_Directory;
            }

            return "";
        }
        /// <summary>
        /// 获取状态机名称
        /// </summary>
        /// <param name="id">对象ID</param>
        /// <param name="csType">客户端服务器类型</param>
        /// <returns>返回状态机名称</returns>
        public static string GetFSMNameSpace(Guid id, CSUtility.Helper.enCSType csType)
        {
            return "FSM_" + id.ToString().Replace("-", "_") + "_" + csType.ToString();
        }

        private bool CreateAssembly(bool bForce)
        {
            string dllDir = GetAssemblyDir(mCSType);
            if (string.IsNullOrEmpty(dllDir))
                return false;

            var fileName = dllDir + "/" + AssemblyFileName;
            if (System.IO.File.Exists(fileName) && !bForce)
            {
                mAssembly = System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(fileName));
                if(mAssembly != null)
                    return true;
            }
            else
            {
                // 重新编译生成Assembly
                if (FStateMachineTemplate.OnCompileFSMCode != null)
                {
                    if (FStateMachineTemplate.OnCompileFSMCode(this.Id, (int)mCSType, bForce))
                    {
                        mAssembly = System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(fileName));
                        if (mAssembly != null)
                            return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// 获取装配值
        /// </summary>
        /// <returns>返回装配值</returns>
        public System.Reflection.Assembly GetAssembly()
        {
            // todo: 这里要做Assembly重读处理
            if (mAssembly == null)
            {
                CreateAssembly(false);
                InitFromAssembly();
            }

            return mAssembly;
        }
        /// <summary>
        /// 获取状态机类型
        /// </summary>
        /// <param name="stateName">状态机名称</param>
        /// <returns>返回状态机的类型</returns>
        public Type GetStateType(string stateName)
        {
            if (mAssembly == null)
                return null;

            return mAssembly.GetType(stateName);
        }

        private bool InitFromAssembly()
        {
            try
            {
                if (mAssembly == null)
                    return false;

                mAllState.Clear();
                // 从Assembly中初始化状态和状态转换
                foreach (var type in mAssembly.GetTypes())
                {
                    // 状态
                    var atts = type.GetCustomAttributes(typeof(CSUtility.AISystem.Attribute.StatementClassAttribute), true);
                    if (atts.Length > 0)
                    {
                        mAllState.Add(type.Name, type);
                    }
                }

                // 状态转换
                mSwitchData = mAssembly.CreateInstance(FStateMachineTemplate.GetFSMNameSpace(mId, mCSType) + ".StateSwitchDataIns") as StateSwitchData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        #endregion

        private Guid mId;
        /// <summary>
        /// 只读属性，状态机ID
        /// </summary>
        public Guid Id
        {
            get { return mId; }
        }

        private CSUtility.Helper.enCSType mCSType;
        /// <summary>
        /// 只读属性，客户端服务器类型
        /// </summary>
        public CSUtility.Helper.enCSType CSType
        {
            get { return mCSType; }
        }

        Dictionary<string, Type> mAllState = new Dictionary<string, Type>();
        /// <summary>
        /// 只读属性，所有的状态列表
        /// </summary>
        public Dictionary<string, Type> AllState
        {
            get { return mAllState; }
        }
        /// <summary>
        /// 获取状态机类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回状态机的类型</returns>
        public Type GetState(string type)
        {
            if (string.IsNullOrEmpty(type))
                return null;
            if (mAllState == null)
                return null;
            Type stateType;
            if (mAllState.TryGetValue(type, out stateType))
                return stateType;
            return null;
        }
        /// <summary>
        /// 状态机模板
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <param name="csType">客户端服务器类型</param>
        public FStateMachineTemplate(Guid id, CSUtility.Helper.enCSType csType)
        {
            mId = id;
            mCSType = csType;
        }

        ulong mVersion;
        /// <summary>
        /// 只读属性，版本号
        /// </summary>
        public ulong Version
        {
            get{ return mVersion; }
        }
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="bForce">是否强制从磁盘加载</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(bool bForce)
        {
            if (CreateAssembly(bForce) == false)
                return false;

            if (InitFromAssembly() == false)
                return false;

            mVersion = FSMTemplateVersionManager.Instance.GetVersion(mId, mCSType);

            return true;
        }

    }

}
