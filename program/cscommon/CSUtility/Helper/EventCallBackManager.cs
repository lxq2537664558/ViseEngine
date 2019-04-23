using System;
using System.Collections.Generic;

namespace CSUtility.Helper
{
    public class EventCallBack
    {
        public delegate bool Delegate_OnCompileEventCode(Guid id, int csType, bool bForceCompile);
        //public Delegate_OnCompileEventCode OnCompileEventCode;

        public UInt64 Version = 0;
        // CallBackType
        public Type CBType;
        public string Description;
        public string NickName;
        public Guid Id;
        //public object ServerCallee;
        //public object ClientCallee;
        private object mCallee = null;
        public object Callee
        {
            // 通过函数GetCallee来取得调用，以便版本号不一致时自动更新
            private get { return mCallee; }
            set
            {
                mCallee = value;
            }
        }
        private System.Reflection.MethodInfo mMethod;

        #region LSharp

        private CLRSharp.IMethod mLMethod;

        #endregion

        // 运行时确定
        public CSUtility.Helper.enCSType CSType = CSUtility.Helper.enCSType.Common;
        private bool bDeleted = false;
        //public CSUtility.Helper.enCSType AllowType;

        // 设置为已删除
        internal void SetDeleted()
        {
            Version = 0;
            Callee = null;
            mLMethod = null;
            mMethod = null;
            bDeleted = true;
        }

        System.Reflection.Assembly mAssembly = null;

        private string AssemblyFileName
        {
            get
            {
                return GetAssemblyFileName(Id, CSType);
            }
        }

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
                    return id + "_" + csType.ToString() + "_V" + EventCallBackVersionManager.Instance.GetVersion(id, csType) + ".dll";
            }
        }
        public static string GetAssemblyFileDir(CSUtility.Helper.enCSType csType)
        {
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    return EventCallBackManager.Instance.TargetRootDirectory + CSUtility.Support.IFileConfig.EventDlls_Client_Directory;

                case CSUtility.Helper.enCSType.Server:
                    return EventCallBackManager.Instance.TargetRootDirectory + CSUtility.Support.IFileConfig.EventDlls_Server_Directory;
            }

            return "";
        }

        public EventCallBack(CSUtility.Helper.enCSType csType)
        {
            CSType = csType;
            //OnCompileEventCode = EventCallBackManager.Instance.OnCompileEventCode;
        }

        public bool Initialize(bool bForce)
        {
            if (!CreateAssembly(bForce))
                return false;

            if (!InitFromAssembly())
                return false;

            Version = EventCallBackVersionManager.Instance.GetVersion(Id, CSType);

            return true;
        }

        private bool LoadAssembly(string fileName)
        {
            var assemblyBytes = System.IO.File.ReadAllBytes(fileName);

            if (EventCallBackManager.Instance.UseLSharp)
            {
                var ms = new System.IO.MemoryStream(assemblyBytes);
                var pdbFile = fileName.Substring(0, fileName.LastIndexOf(".")) + ".pdb";
                bool pdbFileExist = System.IO.File.Exists(pdbFile);
                byte[] assemblyPDBBytes = null;
                System.IO.MemoryStream mspdb;

                try
                {
                    if (pdbFileExist)
                    {
                        assemblyPDBBytes = System.IO.File.ReadAllBytes(pdbFile);
                        mspdb = new System.IO.MemoryStream(assemblyPDBBytes);
                        EventCallBackManager.Instance.LSharpEnviroment.LoadModule(ms, mspdb, new Mono.Cecil.Pdb.PdbReaderProvider());
                    }
                    else
                        EventCallBackManager.Instance.LSharpEnviroment.LoadModule(ms);

                    return true;
                }
                catch(Exception e)
                {
                    try
                    {
                        EventCallBackManager.Instance.LSharpEnviroment.LoadModule(ms, null, null);
                        EventCallBackManager.Instance.Log_Error("符号文件无法识别");
                        return true;
                    }
                    catch
                    {
                        EventCallBackManager.Instance.Log_Error(e.ToString());
                        EventCallBackManager.Instance.Log_Error("模块未加载完成，请检查错误");
                    }
                }
            }
            else
            {
                mAssembly = System.Reflection.Assembly.Load(assemblyBytes);
                if (mAssembly != null)
                    return true;
            }

            return false;
        }

        private bool CreateAssembly(bool bForce)
        {
            string dllDir = GetAssemblyFileDir(CSType);
            if (string.IsNullOrEmpty(dllDir))
                return false;

            var fileName = dllDir + "/" + AssemblyFileName;
            if (System.IO.File.Exists(fileName) && !bForce)
            {
                if (LoadAssembly(fileName))
                    return true;
            }
            else
            {
                // 重新编译生成Assembly
                if (EventCallBackManager.Instance.OnCompileEventCode != null)
                {
                    if (EventCallBackManager.Instance.Exe_OnCompileEventCode(Id, (int)CSType, bForce))
                    {
                        if (LoadAssembly(fileName))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool InitFromAssembly()
        {
            Callee = null;

            if(EventCallBackManager.Instance.UseLSharp)
            {
                if (EventCallBackManager.Instance.LSharpEnviroment == null)
                    return false;

                var type = EventCallBackManager.Instance.LSharpEnviroment.GetType("EventCallBack.Class_" + Program.GetValuedGUIDString(Id)) as CLRSharp.Type_Common_CLRSharp;
                if (type == null)
                    return false;

                var methods = type.GetMethods("Invoke");
                if (methods == null || methods.Length == 0)
                    return false;

                mLMethod = methods[0];

                //if(Id.ToString() == "b03eaf8f-cdd7-4fa3-a5df-02696417b7dd")
                //{
                //    mLMethod.Invoke(EventCallBackManager.Instance.LSharpContext, null, new object[] { null, null });
                //    //var callee = Callee as CSUtility.Map.Trigger.FOnEnter;
                //    //callee?.Invoke(null, null);
                //}
            }
            else
            {
                if (mAssembly == null)
                    return false;

                var type = mAssembly.GetType("EventCallBack.Class_" + Program.GetValuedGUIDString(Id));
                if (type == null)
                    return false;

                //mMethod = type.GetMethod("Invoke");

                var field = type.GetField("EventCall");
                Callee = field.GetValue(null);
            }

            return true;
        }

        public object GetCallee()
        {
            if (bDeleted)
                return null;

            // 判断版本号
            var ver = EventCallBackVersionManager.Instance.GetVersion(Id, CSType);
            if (Version != ver)
            {
                // 更新版本
                CreateAssembly(false);
                InitFromAssembly();

                Version = ver;
            }

            //switch (CSType)
            //{
            //    case AISystem.Attribute.enCSType.Server:
            //        return ServerCallee;
            //    case AISystem.Attribute.enCSType.Client:
            //        return ClientCallee;
            //}
            return Callee;

            //return null;
        }

        public void InvokeCallee(object[] param)
        {
            if (bDeleted)
                return;

            // 判断版本号
            var ver = EventCallBackVersionManager.Instance.GetVersion(Id, CSType);
            if (Version != ver)
            {
                // 更新版本
                CreateAssembly(false);
                InitFromAssembly();

                Version = ver;
            }

            if(EventCallBackManager.Instance.UseLSharp)
            {
                mLMethod?.Invoke(EventCallBackManager.Instance.LSharpContext, null, param);
            }
            else
            {
                mMethod?.Invoke(null, param);
            }
        }

        public void Load(CSUtility.Support.XmlHolder holder, CSUtility.Helper.enCSType csType)  
        {
            var evtNode = holder.RootNode.FindNode("EventCallBackInfo");
            if (evtNode != null)
            {
                var att = evtNode.FindAttrib("CBType");
                if (att != null)
                {
                    CBType = CSUtility.Program.GetTypeFromSaveString(att.Value, csType);
                }
                att = evtNode.FindAttrib("Description");
                if (att != null)
                {
                    Description = att.Value;
                }
                att = evtNode.FindAttrib("NickName");
                if (att != null)
                {
                    NickName = att.Value;
                }
                att = evtNode.FindAttrib("Id");
                if (att != null)
                {
                    Id = CSUtility.Support.IHelper.GuidTryParse(att.Value);
                }
                att = evtNode.FindAttrib("Version");
                if (att != null)
                {
                    Version = System.Convert.ToUInt64(att.Value);
                }
            }
        }

        public void Save(CSUtility.Support.XmlHolder holder, CSUtility.Helper.enCSType csType)
        {
            var evtNode = holder.RootNode.AddNode("EventCallBackInfo", "", holder);
            evtNode.AddAttrib("CBType", CSUtility.Program.GetTypeSaveString(CBType, csType));// CBType.Assembly.FullName + "|" + CBType.FullName);
            evtNode.AddAttrib("Description", Description);
            evtNode.AddAttrib("NickName", NickName);
            evtNode.AddAttrib("Id", Id.ToString());
            evtNode.AddAttrib("Version", Version.ToString());
        }

    }

    public class EventCallBackVersionManager
    {
        bool bLoaded = false;

        private static EventCallBackVersionManager mInstance = new EventCallBackVersionManager();
        public static EventCallBackVersionManager Instance
        {
            get { return mInstance; }
        }

        Dictionary<Guid, UInt64> mEventCallBackVersionDictionary = new Dictionary<Guid, UInt64>();
        public Dictionary<Guid, UInt64> EventCallBackVersionDictionary
        {
            get { return mEventCallBackVersionDictionary; }
        }

        public void Load(CSUtility.Helper.enCSType csType)
        {
            mEventCallBackVersionDictionary.Clear();
            string dicFile = "";
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    {
                        var cDllDir = CSUtility.Support.IFileConfig.EventDlls_Client_Directory;
                        dicFile = cDllDir + "\\EventDic.dic";
                    }
                    break;

                case CSUtility.Helper.enCSType.Server:
                    {
                        var sDllDir = CSUtility.Support.IFileConfig.EventDlls_Server_Directory;
                        dicFile = sDllDir + "\\EventDic.dic";
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

                    mEventCallBackVersionDictionary[key] = value;
                }
            }

            bLoaded = true;
        }

        // Save只有编辑器调用，所以这里客户端和服务器端都做一次存储
        public void Save()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FileDic", "");
            foreach (var fv in mEventCallBackVersionDictionary)
            {
                var node = xmlHolder.RootNode.AddNode("FileNode", "", xmlHolder);
                node.AddAttrib("key", fv.Key.ToString());
                node.AddAttrib("Value", fv.Value.ToString());
            }

            var cDllDir = CSUtility.Support.IFileConfig.EventDlls_Client_Directory;
            var absCDllDir = CSUtility.Support.IFileManager.Instance.Root + cDllDir;
            if (!System.IO.Directory.Exists(absCDllDir))
            {
                System.IO.Directory.CreateDirectory(absCDllDir);
            }
            CSUtility.Support.XmlHolder.SaveXML(cDllDir + "\\EventDic.dic", xmlHolder, false);

            var sDllDir = CSUtility.Support.IFileConfig.EventDlls_Server_Directory;
            var absSDllDir = CSUtility.Support.IFileManager.Instance.Root + sDllDir;
            if (!System.IO.Directory.Exists(absSDllDir))
            {
                System.IO.Directory.CreateDirectory(absSDllDir);
            }
            CSUtility.Support.XmlHolder.SaveXML(sDllDir + "\\EventDic.dic", xmlHolder, true);
        }

        public UInt64 GetVersion(Guid id, CSUtility.Helper.enCSType csType)
        {
            if (!bLoaded)
                Load(csType);

            UInt64 retValue;
            if (mEventCallBackVersionDictionary.TryGetValue(id, out retValue))
                return retValue;

            mEventCallBackVersionDictionary[id] = 0;

            return 0;
        }

        // 版本号更新
        public void VersionUpdate(Guid id, CSUtility.Helper.enCSType csType)
        {
            if (!bLoaded)
                Load(csType);

            UInt64 ver;
            if (mEventCallBackVersionDictionary.TryGetValue(id, out ver))
                mEventCallBackVersionDictionary[id] = ver + 1;
            else
                mEventCallBackVersionDictionary[id] = 0;

            Save();
        }

        public void RemoveEventCallBack(Guid id)
        {
            if (mEventCallBackVersionDictionary.ContainsKey(id))
            {
                mEventCallBackVersionDictionary.Remove(id);
                Save();
            }
        }
    }

    public class EventCallBackManager : CLRSharp.ICLRSharp_Logger
    {
        public delegate void Delegate_OnTellServerUpdateEvent(Guid id);
        public Delegate_OnTellServerUpdateEvent OnTellServerUpdateEvent;

        public EventCallBack.Delegate_OnCompileEventCode OnCompileEventCode;
        public bool Exe_OnCompileEventCode(Guid id, int csType, bool bForceCompile)
        {
            lock(this)
            {
                if (OnCompileEventCode!=null)
                    return OnCompileEventCode(id, csType, bForceCompile);
                return false;
            }
        }

        //List<Type>
        //bool mIsClient = false;
        //public void _SetClient(bool value)
        //{
        //    mIsClient = value;
        //}
        //public bool IsClient
        //{
        //    get { return mIsClient = true; }
        //}
        CSUtility.Helper.enCSType mCSType = CSUtility.Helper.enCSType.Common;
        public void _SetCSType(CSUtility.Helper.enCSType csType)
        {
            mCSType = csType;
        }
        public CSUtility.Helper.enCSType CSType
        {
            get { return mCSType; }
        }

        public string TargetRootDirectory = CSUtility.Support.IFileManager.Instance.Root;

        static EventCallBackManager smIntance = new EventCallBackManager();
        public static EventCallBackManager Instance
        {
            get { return smIntance; }
        }
        Dictionary<Guid, EventCallBack> mEventCallBacks = new Dictionary<Guid, EventCallBack>();

        #region LSharp

        public readonly bool UseLSharp = false;
        CLRSharp.ThreadContext mLSharpContext;
        public CLRSharp.ThreadContext LSharpContext
        {
            get { return mLSharpContext; }
        }
        CLRSharp.CLRSharp_Environment mLSharpEnviroment;
        public CLRSharp.CLRSharp_Environment LSharpEnviroment
        {
            get { return mLSharpEnviroment; }
        }

        public void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        public void Log_Warning(string str)
        {
            Log("<W>" + str);
        }

        public void Log_Error(string str)
        {
            Log("<Err>" + str);
        }

        #endregion

        private EventCallBackManager()
        {
            if (UseLSharp)
            {
                // 初始化LSharp
                mLSharpEnviroment = new CLRSharp.CLRSharp_Environment(this);
                mLSharpContext = new CLRSharp.ThreadContext(mLSharpEnviroment);
            }
        }

        //public void AddCallee(Type cbType,string desc,Guid id, object calleeserver, object calleeclient)
        //{
        //    EventCallBack ecb = new EventCallBack();
        //    ecb.CBType = cbType;
        //    ecb.Description = desc;
        //    ecb.Id = id;
        //    ecb.ServerCallee = calleeserver;
        //    ecb.ClientCallee = calleeclient;
        //    mEventCallBacks.Add(id, ecb);
        //}

        //private bool _OnCompileEventCode(CSUtility.Helper.EventCallBack evb, CSUtility.Helper.enCSType csType, bool bForceCompile)
        //{
        //    if (OnCompileEventCode != null)
        //        return OnCompileEventCode(evb, csType, bForceCompile);

        //    return false;
        //}

        public EventCallBack GetCallee(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            EventCallBack retECB = null;

            mEventCallBacks.TryGetValue(id, out retECB);
            return retECB;
        }

        public EventCallBack LoadCallee(Guid id, bool bForce)
        {
            if (id == Guid.Empty)
                return null;

            EventCallBack retECB = null;
            if (!bForce)
            {
                if (mEventCallBacks.TryGetValue(id, out retECB))
                    return retECB;
            }

            //if (CSUtility.Program.FinalRelease)
            //{
            //if (id.ToString() == "83e5bf74-cfe8-4d9d-a099-086d899ba08d")
            //    System.Windows.Forms.MessageBox.Show("LoadCallee 493 " + id.ToString());

            if (!bForce && CSUtility.Program.FinalRelease)
            {
                retECB = new EventCallBack(mCSType);
                retECB.Id = id;
                if (retECB.Initialize(bForce))
                {
                    mEventCallBacks[id] = retECB;

                    return retECB;
                }

            }        
            else
            {
                var fileName = CSUtility.Support.IFileConfig.DefaultEventDirectory + "\\" + id.ToString() + "\\" + id.ToString() + ".xml";
                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(fileName);
                if (xmlHolder == null)
                {
                    // 如果dictionary中已有callee但没找到文件，则说明这个callee被删除了
                    if (mEventCallBacks.TryGetValue(id, out retECB))
                    {
                        retECB.SetDeleted();
                    }
                    return null;
                }

                retECB = new EventCallBack(mCSType);                
                retECB.Load(xmlHolder, mCSType);

                mEventCallBacks[id] = retECB;
                retECB.Initialize(bForce);                
            }

            return retECB;
        }

        public EventCallBack GetCallee(Type type, Guid id, bool loadWhenNotFound = true)
        {
            lock (mEventCallBacks)
            {
                //if (id.ToString() == "83e5bf74-cfe8-4d9d-a099-086d899ba08d")
                //    System.Windows.Forms.MessageBox.Show("GetCallee " + id.ToString());

                EventCallBack result;
                if (mEventCallBacks.TryGetValue(id, out result))
                {
                    //if (CSUtility.Program.FinalRelease)
                    //{
                    //    //if (id.ToString() == "83e5bf74-cfe8-4d9d-a099-086d899ba08d")
                    //    //    System.Windows.Forms.MessageBox.Show("GetCallee return Result " + id.ToString());

                    //    return result;
                    //}
                    //else
                    {
                        if (result.CBType == type)
                            return result;
                        else
                            return null;
                    }
                }

                if (loadWhenNotFound)
                {
                    //if (id.ToString() == "83e5bf74-cfe8-4d9d-a099-086d899ba08d")
                    //    System.Windows.Forms.MessageBox.Show("GetCallee Load " + id.ToString());

                    result = LoadCallee(id, false);
                    if (result == null)
                        return null;

                    if (CSUtility.Program.FinalRelease)
                    {
                        //if (id.ToString() == "83e5bf74-cfe8-4d9d-a099-086d899ba08d")
                        //{
                        //    if(result.GetCallee() != null)
                        //        System.Windows.Forms.MessageBox.Show("GetCallee Load Success resultType " + result.GetCallee().GetType().ToString() + "/r/n" + id.ToString());
                        //    else
                        //        System.Windows.Forms.MessageBox.Show("GetCallee Load NULL resultType " + result.GetCallee().GetType().ToString() + "/r/n" + id.ToString());
                        //}

                        return result;
                    }
                    else
                    {
                        if (result.CBType == type)
                            return result;
                        else
                            return null;
                    }
                }
            }

            return null;
        }

        [CSUtility.AISystem.Attribute.AllowMember("EventCallBackManager.GetCallee", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("EventCallBackManager.GetCallee", CSUtility.Helper.enCSType.Common, "")]
        public EventCallBack GetCallee(string typeFullName, Guid id, bool loadWhenNotFound = true)
        {
            lock (mEventCallBacks)
            {
                EventCallBack result;
                if (mEventCallBacks.TryGetValue(id, out result))
                {
                    //if (CSUtility.Program.FinalRelease)
                    //{
                    //    return result;
                    //}
                    //else
                    {
                        if (result.CBType.FullName == typeFullName)
                            return result;
                        else
                            return null;
                    }
                }

                if (loadWhenNotFound)
                {
                    result = LoadCallee(id, true);
                    if (result == null)
                        return null;

                    if (CSUtility.Program.FinalRelease)
                    {
                        return result;
                    }
                    else
                    {
                        if (result.CBType.FullName == typeFullName)
                            return result;
                        else
                            return null;
                    }
                }
            }

            return null;
        }

        //public object GetCallee(Type type, Guid id, CSUtility.Helper.enCSType callType)
        //{
        //    EventCallBack result;
        //    if (mEventCallBacks.TryGetValue(id, out result))
        //    {
        //        if (callType == CSUtility.Helper.enCSType.Server)
        //        {
        //            if (result.ServerCallee.GetType() == type)
        //                return result.ServerCallee;
        //        }
        //        else if (callType == CSUtility.Helper.enCSType.Client)
        //        {
        //            if (result.ClientCallee.GetType() == type)
        //                return result.ClientCallee;
        //        }
        //    }
        //    return null;
        //}

        //[CSUtility.AISystem.Attribute.AllowMethod(CSUtility.Helper.enCSType.Client)]
        //public object GetClientCallee(string typeName,Guid id)
        //{
        //    EventCallBack result;
        //    if (mEventCallBacks.TryGetValue(id, out result))
        //    {
        //        if (result.ClientCallee.GetType().FullName == typeName)
        //            return result.ClientCallee;
        //    }
        //    return null;
        //}

        //[CSUtility.AISystem.Attribute.AllowMethod(CSUtility.Helper.enCSType.Server)]
        //public object GetServerCallee(string typeName, Guid id)
        //{
        //    EventCallBack result;
        //    if (mEventCallBacks.TryGetValue(id, out result))
        //    {
        //        if (result.ClientCallee.GetType().FullName == typeName)
        //            return result.ClientCallee;
        //    }
        //    return null;
        //}

        public EventCallBack GetEventCallee_FOnTimer(Guid id)
        {
            return GetCallee(typeof(FOnTimer), id);
        }

        public void UpdateEventWithServer(Guid id)
        {
            // 客户端和服务器刷新新版本event
            if (OnTellServerUpdateEvent != null)
                OnTellServerUpdateEvent(id);

            CSUtility.Helper.EventCallBackVersionManager.Instance.Load(CSUtility.Helper.enCSType.Client);
        }
    }
}
