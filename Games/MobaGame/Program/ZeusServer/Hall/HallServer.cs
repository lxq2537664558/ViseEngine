using System;
using System.Collections.Generic;
using System.Text;
using CSUtility.Net;

namespace Server
{
    public enum HallServerState
    {
        None,
        WaitRegServer,
        WaitDataServer,
        Working,
    }

    [CSUtility.Editor.CDataEditorAttribute(".hallsrv")]
    public class HallServerParameter
    {
        [CSUtility.Support.DataValueAttribute("RegServerIP", false)]
        public string RegServerIP { get; set; } = "127.0.0.1";
        
        [CSUtility.Support.DataValueAttribute("RegServerPort", false)]
        public UInt16 RegServerPort { get; set; } = 8888;
        
        [CSUtility.Support.DataValueAttribute("ListenIP", false)]
        public string ListenIP { get; set; } = "127.0.0.1";
        [CSUtility.Support.DataValueAttribute("ListenPort", false)]
        public UInt16 ListenPort { get; set; } = 22000;
        [CSUtility.Support.DataValueAttribute("ConncetPort", false)]
        public UInt16 ConncetPort { get; set; } = 22100;

        [CSUtility.Support.DataValueAttribute("ServerId", false)]
        public Guid ServerId { get; set; } = Guid.NewGuid();
        //缺省0的话，就根据CPU内核数量开启
        [CSUtility.Support.DataValueAttribute("LogicProcessorNumber", false)]
        public UInt16 LogicProcessorNumber { get; set; } = 0;
        [CSUtility.Support.DataValueAttribute("LoadAllItemTemplate", false)]
        public bool LoadAllItemTemplate { get; set; } = true;
        
        [CSUtility.Support.DataValueAttribute("ServerPower", false)]
        public float ServerPower { get; set; } = 1.0F;
    }

    [RPC.RPCClassAttribute(typeof(HallServer))]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class HallServer : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion     

        public static HallServer Instance;

        public HallServerState LinkState { get; set; } = HallServerState.None;
        public HallServerParameter Parameter { get; set; } = null;
        SCore.TcpServer.TcpServerHP mTcpSrv = new SCore.TcpServer.TcpServerHP();
        SCore.TcpServer.TcpClient mRegConnect = new SCore.TcpServer.TcpClient();
        SCore.TcpServer.TcpClient mDataConnect = new SCore.TcpServer.TcpClient();
        SCore.TcpServer.TcpClient mComConnect = new SCore.TcpServer.TcpClient();
        SCore.TcpServer.TcpClient mPathFindConnect = new SCore.TcpServer.TcpClient();
        public SCore.TcpServer.TcpClient PathFindConnect
        {
            get { return mPathFindConnect; }
        }
        SCore.TcpServer.TcpClient mLogConnect = new SCore.TcpServer.TcpClient();

        #region 主Tick线程操作
        bool IsThreadRun = false;
        bool IsThreadExit = false;
        System.Threading.Thread mThread;
        private void StartTick()
        {
            IsThreadRun = true;
            IsThreadExit = false;
            mThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.OnTickThread));
            mThread.Start();
        }
        private void StopTick()
        {
            if (IsThreadRun == false)
                return;

            IsThreadRun = false;
            while (!IsThreadExit)
            {
                System.Threading.Thread.Sleep(50);
            }
        }

        bool bOutputInfo = false;
        private void OnTickThread()
        {
            while (IsThreadRun)
            {
                var v1 = CSUtility.Helper.LogicTimer.GetHPTickCount();
                this.Tick();
                var v2 = CSUtility.Helper.LogicTimer.GetHPTickCount();
                var delt = (int)(v2 - v1);
                if (delt < 5000)
                {
                    CSUtility.Helper.LogicTimer.TimeBeginPeriod(1);
                    System.Threading.Thread.Sleep((5000 - delt) / 1000);
                    CSUtility.Helper.LogicTimer.TimeEndPeriod(1);
                }
                else
                {
                    if (bOutputInfo)
                        System.Diagnostics.Trace.WriteLine("Hall TickTime " + delt);
                }
            }

            IsThreadExit = true;
        }
        #endregion

        public bool Start(HallServerParameter param)
        {
            CSUtility.Data.RoleTemplateInitFactory.Instance = new GameData.InitFactory.RoleTemplateInitFactory();

            CSUtility.Support.ClassInfoManager.Instance.Load(false);
            #region AI
            var mAIEditorAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "AIEditor.dll");
            var aiType = mAIEditorAssembly?.GetType("AIEditor.Program");
            var aiMethod = aiType?.GetMethod("CompileAICodeWithAIGuid");
            if (aiMethod != null)
            {
                CSUtility.AISystem.FStateMachineTemplate.OnCompileFSMCode =
                    (Guid id, int csType, bool bForceCompile) =>
                    {
                        return (bool)aiMethod.Invoke(null, new object[] { id, csType, bForceCompile,true,"" });
                    };
            }

            CSUtility.AISystem.FSMTemplateVersionManager.Instance.Load(CSUtility.Helper.enCSType.Server);

            var fsms = CSUtility.AISystem.FSMTemplateVersionManager.Instance.FSMTemplateVersionDictionary;
            foreach (var i in fsms.Keys)
            {
                CSUtility.AISystem.FStateMachineTemplateManager.Instance.GetFSMTemplate(i, CSUtility.Helper.enCSType.Server, true);
            }
            #endregion

            #region DelegateMethod
            var mDelegateMethodAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "DelegateMethodEditor.dll");
            var delMethodType = mDelegateMethodAssembly?.GetType("DelegateMethodEditor.CodeGenerator.CodeGenerator");
            var delMethod = delMethodType?.GetMethod("CompileEventCodeWithEventId");
            if (delMethod != null)
            {
                CSUtility.Helper.EventCallBackManager.Instance.OnCompileEventCode = (Guid id, int csType, bool bForceCompile) =>
                 {
                     return (bool)delMethod.Invoke(null, new object[] { id, csType, bForceCompile,true,"" });
                 };                
            }

            CSUtility.Helper.EventCallBackManager.Instance._SetCSType(CSUtility.Helper.enCSType.Server);
            CSUtility.Helper.EventCallBackVersionManager.Instance.Load(CSUtility.Helper.enCSType.Server);
            var events = CSUtility.Helper.EventCallBackVersionManager.Instance.EventCallBackVersionDictionary;
            foreach (var i in events.Keys)
            {
                CSUtility.Helper.EventCallBackManager.Instance.LoadCallee(i, false);
            }
            #endregion
            //CSUtility.Support.ClassInfoManager.Instance.Load(false);
            Parameter = param;
            var impl = new HallServerRootImpl();
            impl.mServer = this;
            RPC.RPCNetworkMgr.Instance.HallServerSpecialRoot = impl;

            mRegConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegConnect.NewConnect += this.OnRegServerConnected;
            mRegConnect.CloseConnect += this.OnRegServerDisConnected;
            mRegConnect.RecvPacketNumLimitter = 1024 * 64;

            mDataConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mDataConnect.NewConnect += this.OnDataServerConnected;
            mDataConnect.CloseConnect += this.OnDataServerDisConnected;
            mDataConnect.RecvPacketNumLimitter = 1024 * 64;

            mComConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mComConnect.NewConnect += this.OnComServerConnected;
            mComConnect.CloseConnect += this.OnComServerDisConnected;
            mComConnect.RecvPacketNumLimitter = 1024 * 64;

            mPathFindConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mPathFindConnect.NewConnect += this.OnPathFindServerConnected;
            mPathFindConnect.CloseConnect += this.OnPathFindServerDisConnected;
            mPathFindConnect.RecvPacketNumLimitter = 1024 * 64;

            mLogConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mLogConnect.NewConnect += this.OnLogServerConnected;
            mLogConnect.CloseConnect += this.OnLogServerDisConnected;
            mLogConnect.RecvPacketNumLimitter = 1024 * 64;

            LinkState = HallServerState.WaitRegServer;
            mRegConnect.Connect(Parameter.RegServerIP, Parameter.RegServerPort);            

            mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.NewConnect += this.OnGateServerConnected;
            mTcpSrv.CloseConnect += this.OnGateServerDisConnected;

            Hall.LogicProcessorManager.Instance.StartProcessors(0);            
            return true;
        }
        public void Stop()
        {
            Hall.LogicProcessorManager.Instance.StopProcessors();

            LinkState = HallServerState.None;
        }
        public Int64 TickTime = CSUtility.DllImportAPI.vfxGetTickCount();
        public Int64 ElapsedMiliSeccond;
        public void Tick()
        {
            var nowTime = CSUtility.DllImportAPI.vfxGetTickCount();
            ElapsedMiliSeccond = nowTime - TickTime;
            TickTime = nowTime;

            mRegConnect.Update();
            mDataConnect.Update();
            mComConnect.Update();
            mLogConnect.Update();
            mPathFindConnect.Update();
            mTcpSrv.Update();

            KeepServerLinker(ElapsedMiliSeccond);

            RPC.RPCNetworkMgr.Instance.Tick(ElapsedMiliSeccond);

            Hall.Map.MapInstanceManager.Instance.Tick(ElapsedMiliSeccond);
        }

        #region 服务器连接状态处理
        bool mIsRegRegServer = false;
        public bool IsRegRegServer
        {
            get { return mIsRegRegServer; }
        }
        bool mIsRegDataServer = false;
        public bool IsRegDataServer
        {
            get { return mIsRegDataServer; }
        }
        bool mIsRegComServer = false;
        public bool IsRegComServer
        {
            get { return mIsRegComServer; }
        }
        bool mIsRegLogServer = false;
        public bool IsRegLogServer
        {
            get { return mIsRegLogServer; }
        }
        bool mIsRegPathServer = false;
        public bool IsRegPathServer
        {
            get { return mIsRegPathServer; }
        }


        Int64 mTryRegServerReconnectTime = 0;
        Int64 mTryDataServerReconnectTime = 0;
        Int64 mTryPathFindServerReconnectTime = 0;
        Int64 mTryLogServerReconnectTime = 0;
        Int64 mTryComServerReconnectTime = 0;
        private void KeepServerLinker(long elapsedMiliSeccond)
        {
            if (mRegConnect.State == CSUtility.Net.NetState.Disconnect || mRegConnect.State == CSUtility.Net.NetState.Invalid)
            {
                mTryRegServerReconnectTime += elapsedMiliSeccond;
                if (mTryRegServerReconnectTime > 3000)
                {
                    mTryRegServerReconnectTime = 0;
                    if (mRegConnect.State != CSUtility.Net.NetState.Connect)
                    {
                        mRegConnect.Reconnect();
                    }
                }
            }
            else if (mRegConnect.State == CSUtility.Net.NetState.Connect && this.IsRegRegServer == false)
            {
                mTryRegServerReconnectTime += elapsedMiliSeccond;
                if (mTryRegServerReconnectTime > 3000)
                {
                    mTryRegServerReconnectTime = 0;
                    Log.FileLog.WriteLine("IsRegRegServer == false");
                    //mRegisterConnect.Close();
                }
            }

            if (mDataConnect.State == CSUtility.Net.NetState.Disconnect || mDataConnect.State == CSUtility.Net.NetState.Invalid)
            {
                mTryDataServerReconnectTime += elapsedMiliSeccond;
                if (mTryDataServerReconnectTime > 3000)
                {
                    mTryDataServerReconnectTime = 0;
                    if (mRegConnect.State == CSUtility.Net.NetState.Connect)
                    {
                        ConnectDataServer();
                    }
                }
            }
            else if (mDataConnect.State == CSUtility.Net.NetState.Connect && this.IsRegDataServer == false)
            {
                mTryDataServerReconnectTime += elapsedMiliSeccond;
                if (mTryDataServerReconnectTime > 3000)
                {
                    mTryDataServerReconnectTime = 0;
                    Log.FileLog.WriteLine("IsRegDataServer == false");
                    mDataConnect.Close();
                }
            }
            if (mComConnect.State == CSUtility.Net.NetState.Disconnect || mComConnect.State == CSUtility.Net.NetState.Invalid)
            {
                mTryComServerReconnectTime += elapsedMiliSeccond;
                if (mTryComServerReconnectTime > 3000)
                {
                    mTryComServerReconnectTime = 0;
                    if (mRegConnect.State == CSUtility.Net.NetState.Connect)
                    {
                        ConnectComServer();
                    }
                }
            }
            if (mPathFindConnect.State == CSUtility.Net.NetState.Disconnect || mPathFindConnect.State == CSUtility.Net.NetState.Invalid)
            {
                mTryPathFindServerReconnectTime += elapsedMiliSeccond;
                if (mTryPathFindServerReconnectTime > 3000)
                {
                    mTryPathFindServerReconnectTime = 0;
                    if (mRegConnect.State == CSUtility.Net.NetState.Connect)
                    {
                        ConnectPathFindServer();
                    }
                }
            }
            if (mLogConnect.State == CSUtility.Net.NetState.Disconnect || mLogConnect.State == CSUtility.Net.NetState.Invalid)
            {
                mTryLogServerReconnectTime += elapsedMiliSeccond;
                if (mTryLogServerReconnectTime > 3000)
                {
                    mTryLogServerReconnectTime = 0;
                    if (mRegConnect.State == CSUtility.Net.NetState.Connect)
                    {
                        ConnectLogServer();
                    }
                }
            }
        }
        void OnRegServerConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

            RPC.PackageWriter pkg = new RPC.PackageWriter();

            H_RPCRoot.smInstance.HGet_RegServer(pkg).RegHallServer(pkg, Parameter.ServerId, Parameter.ListenPort);            
            pkg.WaitDoCommand(mRegConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                sbyte success = -1;
                _io.Read(out success);
                if (success == 1)
                {                    
                    LinkState = HallServerState.WaitDataServer;
                    mIsRegRegServer = true;
                }
            };
        }
        void OnRegServerDisConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            mIsRegRegServer = false;
        }
        void OnDataServerConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;
            if (LinkState != HallServerState.Working)
            {
                if (false == mTcpSrv.Open(SCore.TcpServer.TcpOption.ForPlanesServer, Parameter.ListenPort))
                    return;
                System.Diagnostics.Debug.WriteLine("DateServer连接成功，PlanesServer开始接受GateServer接入");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("PlanesServer与DataServer断线后重连接成功");
            }
            LinkState = HallServerState.Working;

            RPC.PackageWriter pkg = new RPC.PackageWriter();

            H_RPCRoot.smInstance.HGet_DataServer(pkg).RegHallServer(pkg, Parameter.ListenIP, Parameter.ListenPort, Parameter.ServerId, Parameter.ServerPower);
            pkg.WaitDoCommand(mDataConnect, RPC.CommandTargetType.DefaultType, null).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                sbyte successed = -1;
                _io.Read(out successed);
                if (successed == 1)
                {
                    mIsRegDataServer = true;
                }
            };
        }
        void OnDataServerDisConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            Log.FileLog.WriteLine("位面与数据服务器连接断开了");
            mIsRegDataServer = false;
        }
        void OnComServerConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

            mIsRegComServer = true;
        }
        void OnComServerDisConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            mIsRegComServer = false;
        }
        void OnPathFindServerConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

            mIsRegPathServer = true;
        }
        void OnPathFindServerDisConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            mIsRegPathServer = true;
        }
        void OnLogServerConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

            mIsRegLogServer = true;
        }
        void OnLogServerDisConnected(CSUtility.Net.TcpClient pClientr, byte[] pData, int nLength)
        {
            mIsRegLogServer = false;
        }

        void OnGateConnect(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {

        }

        void OnGateDisConnecdt(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {

        }

        void OnGateServerConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
          //  mGateServers.Remove(pConnect);
        }
        void OnGateServerDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {

        }

        void ConnectDataServer()
        {
            Instance = this;

            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_RegServer(pkg).GetDataServer(pkg);
            pkg.WaitDoCommand(mRegConnect, RPC.CommandTargetType.DefaultType, null).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                RPC.DataReader dr;
                _io.Read(out dr);

                sbyte successed = -1;
                dr.Read(out successed);
                if (successed != 1)
                    return;
                string gsIpAddress = "";
                dr.Read(out gsIpAddress);
                UInt16 gsPort = 0;
                dr.Read(out gsPort);

                mDataConnect.Connect(gsIpAddress, gsPort);
            };
        }

        void ConnectComServer()
        {
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_RegServer(pkg).GetComServer(pkg);
            pkg.WaitDoCommand(mRegConnect, RPC.CommandTargetType.DefaultType, null).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                RPC.DataReader dr;
                _io.Read(out dr);

                sbyte successed = -1;
                dr.Read(out successed);
                if (successed != 1)
                    return;
                string gsIpAddress = "";
                dr.Read(out gsIpAddress);
                UInt16 gsPort = 0;
                dr.Read(out gsPort);

                mComConnect.Connect(gsIpAddress, gsPort);
            };
        }
        void ConnectPathFindServer()
        {            
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_RegServer(pkg).GetPathFindServers(pkg);
            pkg.WaitDoCommand(mRegConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                Byte count = 0;
                _io.Read(out count);
                if (count > 1)
                    count = 1;
                for (Byte i = 0; i < count; i++)
                {
                    string ip;
                    _io.Read(out ip);
                    UInt16 port;
                    _io.Read(out port);

                    mPathFindConnect.Connect(ip, port);
                }
            };
        }

        void ConnectLogServer()
        {            
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_RegServer(pkg).GetLogServer(pkg);
            pkg.WaitDoCommand(mRegConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;

                RPC.DataReader dr;
                _io.Read(out dr);

                sbyte successed = -1;
                dr.Read(out successed);
                if (successed != 1)
                    return;
                string ip;
                dr.Read(out ip);
                UInt16 port;
                dr.Read(out port);
                mLogConnect.Connect(ip, port);
            };
        }
        #endregion

        Dictionary<Guid, CSUtility.Net.NetEndPoint> mGateServers = new Dictionary<Guid, CSUtility.Net.NetEndPoint>();
        public Dictionary<Guid, CSUtility.Net.NetEndPoint> GateServers
        {
            get { return mGateServers; }
        }

        public CSUtility.Net.NetConnection GetGateConnect()
        {
            foreach(var ser in mGateServers)
            {
                if(ser.Key !=null)
                    return ser.Value.Connect;
            }
            return null;
        }

        #region RPC Method
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public Guid GetHallServerId()
        {
            return Parameter.ServerId;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void RegGateServer(string ip, UInt16 port, Guid id, CSUtility.Net.NetConnection connect)
        {
//             if (mGateServers.ContainsKey(id))
//                 return;
            //             SCore.TcpServer.TcpClient conn = new SCore.TcpServer.TcpClient();
            //             conn.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            //             conn.Port = port;
            //             conn.HostIp = ip;
            //             conn.Connect(ip, port);
            CSUtility.Net.NetEndPoint nep = new CSUtility.Net.NetEndPoint(ip, port);
            nep.Id = id;
            nep.Connect = connect;

            CSUtility.Net.NetEndPoint oldnep;
            if (mGateServers.TryGetValue(id, out oldnep) == true)
            {
                mGateServers[id] = nep;
            }
            else
            {
                mGateServers.Add(id, nep);
            }
        }

        int mFactionId = 1;

        #region 角色进出地图
        //这个函数必须是GateServer调用的
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void EnterMap(UInt16 cltHandle, string ip, int port,Guid mapSourceId,UInt16 roleTemplateId,CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            var pd = new GameData.Role.PlayerData();
            pd.RoleId = Guid.NewGuid();
            pd.TemplateId = roleTemplateId;
            pd.IpAddress = ip;
            pd.Port = port;

            var player = Hall.Role.Player.PlayerInstance.CreatePlayerInstance(pd, connect as SCore.TcpServer.TcpConnect, cltHandle);
            if (player == null)//创建角色
            {
                RPC.PackageWriter retPkg2 = new RPC.PackageWriter();
                retPkg2.Write((sbyte)-1);
                retPkg2.DoReturnCommand2(connect, fwd.ReturnSerialId);

                Log.FileLog.WriteLine("DataServer1 force disconnect player!");
                Log.FileLog.WriteLine(new System.Diagnostics.StackTrace(0, true).ToString());
                return;
            }

            Hall.PlayerInstanceManager.Instance.AddPlayerInstance(player);//进服务器
            var hallsData = new GameData.HallsData();
            hallsData.HallsId = GameData.Support.ConfigFile.Instance.DefaultHallId;

            //ToDo 这里初始化位面data
            var halls = Hall.Map.MapInstanceManager.Instance.HallsManager.GetHallsInstance(hallsData);
            halls.EnterHalls(player);//进大厅            

            pd.PlayerFaction = (++mFactionId) % 2 + 1;

            var faction = CSUtility.Data.DataTemplateManager<UInt16, GameData.Faction>.Instance.GetDataTemplate((UInt16)pd.PlayerFaction);// GameData.FactionManager.Instance.FindFaction((UInt16)pd.PlayerFaction);
            if (faction != null)
            {
                pd.OriPosition = pd.Position = faction.StarPoint;

                Hall.Map.MapInstanceManager.Instance.PlayerEnterMap(player, mapSourceId, pd.Position);//进地图

                RPC.PackageWriter retPkg = new RPC.PackageWriter();
                retPkg.Write((sbyte)1);
                retPkg.Write(player.SingleId);
                retPkg.Write(player.IndexInMap);
                retPkg.Write(player.HostMap.IndexInManager);
                retPkg.Write(pd);
                retPkg.Write(pd.Skills.Count);
                foreach (var skill in pd.Skills)
                {
                    retPkg.Write(skill);
                }
                var conn = GetGateConnect();
                retPkg.DoReturnCommand2(conn, fwd.ReturnSerialId);
            }                                        
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void LeaveMap(UInt16 cltHandle)
        {
            var player = Hall.PlayerInstanceManager.Instance.FindPlayerInstance(cltHandle);

            if (player == null)
                return;
            Hall.PlayerInstanceManager.Instance.RemovePlayerInstance(player);
            player.HallInstance.LeaveHalls(player.Id);
            player.HostMap.OnPlayerLeaveMap(player);

        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void RemoveInstanceMap(Guid mapInstanceId)
        {
            Hall.Map.MapInstanceManager.Instance.RemoveInstanceMap(mapInstanceId);
        }
        #endregion

        #region GM指令

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, false)]
        public void GM_ReloadTemplate(string templateType, UInt16 id, Byte opType, CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            var type = CSUtility.Program.GetTypeFromTypeFullName(templateType);
            if (type == null)
                return;

            CSUtility.Data.DataTemplateManagerAssist.Instance.ReloadDataTemplate(type, id, (CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType)opType);
        }

        #endregion

        #endregion
    }

    public class HallServerRootImpl : RPC.RPCHallServerSpecialRoot
    {
        public HallServer mServer;
        public override void Push2Processor(RPC.RPCSpecialHolder holder, RPC.PackageType ptype)
        {
            if (ptype == RPC.PackageType.PKGT_C2P_Player_Send || ptype == RPC.PackageType.PKGT_C2P_Player_SendAndWait)
            {
                Hall.Map.MapInstance mapInstance = Hall.Map.MapInstanceManager.Instance.GetMapInstance(holder.mForward.MapIndexInServer);
                if (mapInstance == null)
                {
                    holder.DestroyBuffer();
                    //Log.FileLog.WriteLine("玩家地图索引非法");
                    return;
                }

                RPC.RPCSpecialHolderProcessor RpcProcessor = mapInstance.RpcProcessor;
                if (RpcProcessor == null)
                {
                    holder.DestroyBuffer();
                    Log.FileLog.WriteLine("地图的RPC处理器不合法");
                    //RPC.PackageWriter pkg = new RPC.PackageWriter();
                    //H_RPCRoot.smInstance.HGet_GateServer(pkg).DisconnectPlayerByConnectHandle(pkg, holder.mForward.Handle);
                    //pkg.DoCommand(holder.mForward.Planes2GateConnect, RPC.CommandTargetType.DefaultType);
                    return;
                }
                var player = mapInstance.GetPlayerByIndex(holder.mForward.PlayerIndexInMap);
                if (player == null)
                {
                    holder.DestroyBuffer();
                    //Log.FileLog.WriteLine("找不到RPC处理的玩家");
                    //RPC.PackageWriter pkg = new RPC.PackageWriter();
                    //H_RPCRoot.smInstance.HGet_GateServer(pkg).DisconnectPlayerByConnectHandle(pkg, holder.mForward.Handle);
                    //pkg.DoCommand(holder.mForward.Planes2GateConnect, RPC.CommandTargetType.DefaultType);
                    return;
                }
                else
                {
                    if (player.Id != holder.mForward.RoleId)
                    {
                        holder.DestroyBuffer();
                        Log.FileLog.WriteLine("RPC:玩家{0}:[{1}]!={2}", player.RoleName, player.Id, holder.mForward.RoleId);
                        return;
                    }
                    holder.mRoot = player;
                    //RPC.RPCSpecialHolderProcessor.Process(holder);
                    RpcProcessor.PushHolder(holder);
                    return;
                }
            }

            holder.DestroyBuffer();
            return;
        }
        public override string GetPlayerInfoString(RPC.RPCForwardInfo fwd)
        {
            return "Unknown Planes";
        }
    };
}
