using CSUtility.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public enum GateServerState
    {
        None,
        WaitRegServer,
        WaitDataServer,
        Working,
    }

    [CSUtility.Editor.CDataEditorAttribute(".gatesrv")]
    public class GateServerParameter
    {
        string mGateServerIP = "127.0.0.1";
        [CSUtility.Support.DataValueAttribute("GateServerIP", false)]
        public string GateServerIP
        {
            get
            {
                if(mGateServerIP== "127.0.0.1")
                {
                    var ip = GetHostIpv4();
                    if (!string.IsNullOrEmpty(ip))
                        mGateServerIP = ip;
                }
                return mGateServerIP;
            }
            set { mGateServerIP = value; }
        }

        string mRegServerIP = "127.0.0.1";
        [CSUtility.Support.DataValueAttribute("RegServerIP", false)]
        public string RegServerIP
        {
            get { return mRegServerIP; }
            set { mRegServerIP = value; }
        }
        UInt16 mRegServerPort = 8888;
        [CSUtility.Support.DataValueAttribute("RegServerPort", false)]
        public UInt16 RegServerPort
        {
            get { return mRegServerPort; }
            set { mRegServerPort = value; }
        }

        string mListenIP = "127.0.0.1";
        [CSUtility.Support.DataValueAttribute("ListenIP", false)]
        public string ListenIP
        {
            get { return mListenIP; }
            set { mListenIP = value; }
        }
        UInt16 mListenPort = 21000;
        [CSUtility.Support.DataValueAttribute("ListenPort", false)]
        public UInt16 ListenPort
        {
            get { return mListenPort; }
            set { mListenPort = value; }
        }
        Guid mServerId = Guid.NewGuid();
        [CSUtility.Support.DataValueAttribute("ServerId", false)]
        public Guid ServerId
        {
            get { return mServerId; }
            set { mServerId = value; }
        }

        static public string GetHostIpv4()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Log.FileLog.WriteLine("获取本机IP出错:" + ex.Message);
                return "";
            }
        }
    }

    [RPC.RPCClassAttribute(typeof(GateServer))]
    public class GateServer : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion

        static GateServer smInstance;
        public static GateServer Instance
        {
            get { return smInstance; }
        }
        public GateServer()
        {
            /*InitClientLinkerPool();*/
        }

        #region 核心数据
        protected SCore.TcpServer.TcpServerHP mTcpSrv = new SCore.TcpServer.TcpServerHP();

        protected SCore.TcpServer.TcpClient mRegisterConnect = new SCore.TcpServer.TcpClient();
        protected SCore.TcpServer.TcpClient mDataConnect = new SCore.TcpServer.TcpClient();

        GateServerState mLinkState = GateServerState.None;
        public GateServerState LinkState
        {
            get { return mLinkState; }
        }

        GateServerParameter mParameter;
        #endregion      

        public void Start(GateServerParameter parameter)
        {
            CSUtility.Data.RoleTemplateInitFactory.Instance = new GameData.InitFactory.RoleTemplateInitFactory();

            RPCGateServerForwardImp1 impl = new RPCGateServerForwardImp1();
            impl.mServer = this;
            RPC.RPCNetworkMgr.Instance.mGateServerForward = impl;

            mParameter = parameter;
            //GetInternetServerIP();

            mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.NewConnect += this.ClientConnected;
            mTcpSrv.CloseConnect += this.ClientDisConnected;

            mRegisterConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegisterConnect.NewConnect += this.OnRegisterConnected;
            mRegisterConnect.RecvPacketNumLimitter = 1024 * 64;

            mDataConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mDataConnect.NewConnect += this.OnDataServerConnected;
            mDataConnect.CloseConnect += this.OnDataServerDisConnected;
            mDataConnect.RecvPacketNumLimitter = 1024 * 64;

            mLinkState = GateServerState.WaitRegServer;
            mRegisterConnect.Connect(parameter.RegServerIP, parameter.RegServerPort);

            Log.FileLog.Instance.Begin("GateServer.log", false);
            Log.FileLog.WriteLine("GateServer Start ===== ok!");

            //var xml = RPC.RPCNetworkMgr.Instance.PrintAllRPCCounter();
            //CSUtility.Support.IXmlHolder.SaveXML("RPCCounter.xml", xml, true);

            //CSUtility.Support.StringFilterHelper.Instance.InitFilter(CSUtility.Support.IFileManager.Instance.Root + "ZeusGame/Sensitive/SensitiveWord.txt");

            InitClientLinkerPool();
            smInstance = this;
            StartTick();
        }

        #region 主Tick线程操作
        bool IsThreadRun = false;
        bool IsThreadExit = false;
        System.Threading.Thread mThread;
        void StartTick()
        {
            IsThreadRun = true;
            IsThreadExit = false;
            mThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.OnTickThread));
            mThread.Start();
        }

        void StopTick()
        {
            if (IsThreadRun == false)
                return;

            IsThreadRun = false;
            while (!IsThreadExit)
            {
                System.Threading.Thread.Sleep(50);
            }
        }

        bool bOutputInfo1 = false;
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
                    if (bOutputInfo1)
                        System.Diagnostics.Trace.WriteLine("Gate TickTime " + delt);
                }
            }
            IsThreadExit = true;
        }
        #endregion

        public void Stop()
        {
            StopTick();

            mTcpSrv.ReceiveData -= RPC.RPCNetworkMgr.Instance.ServerReceiveData;
          //  mTcpSrv.NewConnect -= this.ClientConnected;
          //  mTcpSrv.CloseConnect -= this.ClientDisConnected;

            mRegisterConnect.ReceiveData -= RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegisterConnect.NewConnect -= this.OnRegisterConnected;
            mDataConnect.ReceiveData -= RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mDataConnect.NewConnect -= this.OnDataServerConnected;

            mRegisterConnect.Close();
            mDataConnect.Close();
            mDataConnect.Close();

            System.Diagnostics.Debug.WriteLine("连接服务器关闭");
            mLinkState = GateServerState.None;

            Log.FileLog.Instance.End();
        }

        UInt64 mTryRegServerReconnectTime;
        UInt64 mTryDataServerReconnectTime;
        public void Tick()
        {
            var t1 = CSUtility.Helper.LogicTimer.GetHPTickCount();
            UInt64 time = ((UInt64)t1) / 1000;

            #region 维护服务器之间的链接
            switch (mLinkState)
            {
                case GateServerState.None:
                case GateServerState.WaitRegServer:
                    {
                        //每过一段时间尝试连接一次
                        if (mRegisterConnect.State == CSUtility.Net.NetState.Disconnect || mRegisterConnect.State == CSUtility.Net.NetState.Invalid)
                        {
                            if (time - mTryRegServerReconnectTime > 3000)
                            {
                                mTryRegServerReconnectTime = time;
                                mRegisterConnect.Reconnect();
                            }
                        }
                    }
                    break;
                case GateServerState.WaitDataServer:
                    {
                        if (mDataConnect.State == CSUtility.Net.NetState.Disconnect || mDataConnect.State == CSUtility.Net.NetState.Invalid)
                        {
                            //每过一段时间尝试连接一次
                            if (time - mTryDataServerReconnectTime > 3000)
                            {
                                mTryDataServerReconnectTime = time;
                                //mDataConnect.Reconnect();
                                ConnectDataServer();
                            }
                        }
                    }
                    break;
                case GateServerState.Working:
                    {
                        if (mRegisterConnect.State == CSUtility.Net.NetState.Disconnect || mRegisterConnect.State == CSUtility.Net.NetState.Invalid)
                        {
                            //每过一段时间尝试连接一次
                            if (time - mTryRegServerReconnectTime > 3000)
                            {
                                mTryRegServerReconnectTime = time;
                                mRegisterConnect.Reconnect();
                            }
                        }
                        if (mDataConnect.State == CSUtility.Net.NetState.Invalid || mDataConnect.State == CSUtility.Net.NetState.Disconnect)
                        {
                            //每过一段时间尝试连接一次
                            if (time - mTryDataServerReconnectTime > 3000)
                            {
                                mTryDataServerReconnectTime = time;
                                //mDataConnect.Reconnect();
                                ConnectDataServer();
                            }
                        }
                        else
                        {
                            if (IsRegDataServer == false)
                            {
                                RPC.PackageWriter pkg = new RPC.PackageWriter();
                                H_RPCRoot.smInstance.HGet_DataServer(pkg).RegGateServer(pkg, mParameter.ListenIP, mParameter.ListenPort, mParameter.ServerId);
                                pkg.WaitDoCommand(mDataConnect, RPC.CommandTargetType.DefaultType, null).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
                                {
                                    if (bTimeOut)
                                        return;

                                    IsRegDataServer = true;
                                };

                                RefreshHallConnect();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            #endregion

            IServer.Instance.Tick();

            mRegisterConnect.Update();
            mDataConnect.Update();
            mTcpSrv.Update();
            FreshConnect(t1);

            UpdateHallClientConnect();

        }

        long mUpdateTime = 0;
        void FreshConnect(long elesepe)
        {
            if (elesepe - mUpdateTime < 3000000)
                return;
            mUpdateTime = elesepe;
            RefreshHallConnect();

            foreach(var conn in mClientLinkers)
            {
                if (conn == null)
                    break;
                if (conn.Connect.State == CSUtility.Net.NetState.Disconnect)
                    ClientDisconnected(conn.Connect);
            }

        }

        void UpdateHallClientConnect()//hallserver connect client update
        {
            var t1 = CSUtility.Helper.LogicTimer.GetHPTickCount();
            foreach (var kv in mHallConnects)
            {
                if (kv.Value != null)
                {
                    if (kv.Value.State == NetState.Disconnect || kv.Value.State == NetState.Invalid)
                    {
                        kv.Value.Update();
                        mHallConnects.Remove(kv.Key);
                        break;
                    }
                    kv.Value.Update();
                }
            }

            var t2 = CSUtility.Helper.LogicTimer.GetHPTickCount();

            t1 = CSUtility.Helper.LogicTimer.GetHPTickCount();
            RPC.RPCNetworkMgr.Instance.Tick(IServer.Instance.GetElapseMilliSecondTime());
            t2 = CSUtility.Helper.LogicTimer.GetHPTickCount();
        }


        void ConnectDataServer()
        {
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_RegServer(pkg).GetDataServer(pkg);
            pkg.WaitDoCommand(mRegisterConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
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
                UInt16 gsPort = 0;
                if (dr.Length > 0)
                {
                    dr.Read(out gsIpAddress);
                    dr.Read(out gsPort);
                }
                if (gsIpAddress != "" && gsPort != 0)
                {
                    if (mLinkState == GateServerState.WaitRegServer)
                    {
                        mDataConnect.Connect(gsIpAddress, gsPort);
                        System.Diagnostics.Debug.WriteLine("GateServer成功连接RegServer，尝试连接DataServer:" + gsIpAddress + ":" + gsPort);
                    }
                    else
                    {
                        mDataConnect.Connect(gsIpAddress, gsPort);
                        System.Diagnostics.Debug.WriteLine("DataServer断线，重新从RegServer获得地址，尝试连接DataServer:" + gsIpAddress + ":" + gsPort);
                    }
                }
            };
        }

        void GetInternetServerIP()
        {
            if (string.IsNullOrEmpty(mParameter.ListenIP))
            {
                var ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
                foreach (var i in ips)
                {
                    if (i.IsIPv6LinkLocal)
                        continue;

                    var ip = i.ToString();
                    if (ip.Contains("192.168."))
                    {
                        continue;
                    }
                    else
                    {
                        mParameter.ListenIP = ip;
                        break;
                    }
                }
            }
            else
            {
                var ips = System.Net.Dns.GetHostAddresses(mParameter.ListenIP);
                mParameter.ListenIP = ips[0].ToString();
            }

            //return mParameter.ListenIP;
        }

        void OnRegisterConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;
            RPC.PackageWriter pkg = new RPC.PackageWriter();

            H_RPCRoot.smInstance.HGet_RegServer(pkg).RegGateServer(pkg, mParameter.ServerId, mParameter.ListenPort,mParameter.GateServerIP);
            pkg.WaitDoCommand(mRegisterConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                sbyte success = -1;
                _io.Read(out success);
                if (success == 1)
                {
                    if (mLinkState == GateServerState.WaitRegServer)
                    {
                        Log.FileLog.WriteLine("连接服务器({0})启动并且注册成功", mParameter.ServerId);
                        mLinkState = GateServerState.WaitDataServer;
                        ConnectDataServer();
                    }
                    else
                    {
                        Log.FileLog.WriteLine("GateServer断线重连接RegServer");
                    }
                }
            };
        }

        void OnRegisterDisConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {

        }

        bool IsRegDataServer = false;
        void OnDataServerConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;
            if (mLinkState != GateServerState.Working)
            {
                if (false == mTcpSrv.Open(SCore.TcpServer.TcpOption.ForGateServer, mParameter.ListenPort))
                    return;
                System.Diagnostics.Debug.WriteLine("DateServer连接成功，GateServer开始接受客户端接入");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("GateServer与DateServer断线后重连接成功");
            }
            mLinkState = GateServerState.Working;

            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_DataServer(pkg).RegGateServer(pkg, mParameter.ListenIP, mParameter.ListenPort, mParameter.ServerId);
            pkg.WaitDoCommand(mDataConnect, RPC.CommandTargetType.DefaultType, null).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;

                IsRegDataServer = true;
            };

            RefreshHallConnect();
        }

        void OnDataServerDisConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            IsRegDataServer = false;
        }

        #region 与HallServer建立连接
        protected Dictionary<Guid, CSUtility.Net.TcpClient> mHallConnects = new Dictionary<Guid, CSUtility.Net.TcpClient>();

        CSUtility.Net.TcpClient FindHallConnect(Guid serverId)
        {
            CSUtility.Net.TcpClient result;
            if(mHallConnects.ContainsKey(serverId))
            {
                if (mHallConnects.TryGetValue(serverId, out result))
                {
                    if (result.State == CSUtility.Net.NetState.Connect)
                        return result;
                }
            }
            return null;
        }

        void RefreshHallConnect()
        {
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_RegServer(pkg).GetHallServers(pkg);
            pkg.WaitDoCommand(mRegisterConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _pkg, bool bTimeOut)
            {
                RPC.DataReader dr;
                _pkg.Read(out dr);
                Byte count;
                dr.Read(out count);
                for (Byte i = 0; i < count; i++)
                {
                    Guid svrId;
                    dr.Read(out svrId);
                    string ip;
                    dr.Read(out ip);
                    UInt16 port;
                    dr.Read(out port);

                    if (null == FindHallConnect(svrId))
                    {
                        SCore.TcpServer.TcpClient conn = new SCore.TcpServer.TcpClient();
                        conn.NewConnect += this.HallsConnected;
                        conn.CloseConnect += this.HallsDisConnected;
                        conn.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
                        conn.Connect(ip, port);
                        if(conn.Connected)
                            mHallConnects.Add(svrId, conn);
                    }
                }
            };
        }

        public void HallsConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;
            System.Diagnostics.Debug.WriteLine("一个位面服务器与连接服务器建立连接");
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_HallServer(pkg).RegGateServer(pkg, mParameter.ListenIP, mParameter.ListenPort, mParameter.ServerId);
            pkg.DoCommand(pClient, RPC.CommandTargetType.DefaultType);
        }
        public void HallsDisConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            //断开所有进入此位面的玩家接入服务器连接
            foreach (Gate.ClientLinker lnk in mClientLinkers)
            {
                if (lnk != null && lnk.mForwardInfo.Gate2PlanesConnect == pClient)
                {
                    lnk.mForwardInfo.Gate2PlanesConnect = null;
                    lnk.mForwardInfo.Gate2ClientConnect.Disconnect();
                }
            }
        }

        #endregion

        #region 客户端接入断开管理
        UInt16 mMaxClientLinker = 8912;

        Gate.ClientLinker[] mClientLinkers;
        public Gate.ClientLinker[] ClientLinkers
        {
            get { return mClientLinkers; }
        }

        Stack<UInt16> mFreeClientLinkers = new Stack<ushort>();

        int mClientConnectCount = 0;
        public int ClientConnectCount
        {
            get { return mClientConnectCount; }
        }

        void InitClientLinkerPool()
        {
            mClientLinkers = new Gate.ClientLinker[mMaxClientLinker];
            mFreeClientLinkers.Clear();
            for (int i = mMaxClientLinker - 1; i >= 0; --i)
            {
                mFreeClientLinkers.Push((UInt16)i);
            }
        }

        bool AllocLinker(Gate.ClientLinker lnk)
        {
            lock (this)
            {
                if (mFreeClientLinkers.Count == 0)
                    return false;
                lnk.mForwardInfo.Handle = mFreeClientLinkers.Pop();
                mClientLinkers[lnk.mForwardInfo.Handle] = lnk;
                lnk.ConnectedTime = DateTime.Now;
                return true;
            }
        }

        bool FreeLinker(Gate.ClientLinker lnk)
        {
            lock (this)
            {
                if (lnk.mForwardInfo.Handle > mMaxClientLinker)
                {
                    Log.FileLog.WriteLine("Error!FreeLinker : lnk.mForwardInfo.Handle > mMaxClientLinker");
                    return false;
                }
                mFreeClientLinkers.Push(lnk.mForwardInfo.Handle);
                mClientLinkers[lnk.mForwardInfo.Handle] = null;
                lnk.mForwardInfo.Handle = UInt16.MaxValue;
                lnk.Connect = null;
                return true;
            }
        }

        void CheckFreeLinkers()
        {
            try
            {
                for (int i = 0; i < mClientLinkers.Length; i++)
                {
                    var lnk = mClientLinkers[i];
                    if (lnk == null)
                        continue;
                    var realLinker = lnk.mForwardInfo.Gate2ClientConnect.m_BindData as Gate.ClientLinker;
                    if (realLinker == lnk)
                        continue;

                    Log.FileLog.WriteLine("Error!CheckFreeLinkers check failed");
                    FreeLinker(lnk);
                    break;
                }
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
        }

        public Gate.ClientLinker FindClientLinker(UInt16 index)
        {
            if (index >= mMaxClientLinker)
                return null;

            return mClientLinkers[index];
        }

        public Gate.ClientLinker FindClientLinkerByAccountId(Guid accountId)
        {
            foreach (var i in mClientLinkers)
            {
                if (i == null)
                    continue;
                if (i.AccountInfo.Id == accountId)
                    return i;
            }
            return null;
        }

        public void ClientConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {         
            ClientConnect(pConnect);
        }

        public Gate.ClientLinker ClientConnect(SCore.TcpServer.TcpConnect pConnect)
        {
            if (pConnect.State == CSUtility.Net.NetState.Disconnect)
                return null;

            mClientConnectCount++;
            var cltLinker = new Gate.ClientLinker();
            if (AllocLinker(cltLinker))
            {
                cltLinker.mForwardInfo.Gate2ClientConnect = pConnect;
                
                cltLinker.mForwardInfo.LimitLevel = (int)RPC.RPCExecuteLimitLevel.Player;
                pConnect.mLimitLevel = (int)RPC.RPCExecuteLimitLevel.Player;

                pConnect.m_BindData = cltLinker;
                cltLinker.Connect = pConnect as SCore.TcpServer.TcpConnectHP;
   
                int linkNumber = mMaxClientLinker - mFreeClientLinkers.Count;
                return cltLinker;
            }
            else
            {//接入满了
                Log.FileLog.WriteLine("ClientConnected AllocLinker Failed");
                pConnect.Disconnect();

                CheckFreeLinkers();
                return null;
            }

        }

        public void ClientDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
            ClientDisconnected(pConnect);
        }

        void ClientDisconnected(SCore.TcpServer.TcpConnect pConnect)
        {
            mClientConnectCount--;
            Gate.ClientLinker cltLinker = pConnect.m_BindData as Gate.ClientLinker;
            if (cltLinker == null)
            {
                Log.FileLog.WriteLine("Error!ClientDisConnected : cltLinker==null");
                return;
            }

            NotifyOtherServers_ClientDisconnect(cltLinker);

            FreeLinker(cltLinker);          
            pConnect.m_BindData = null;
        }

        public void NotifyOtherServers_ClientDisconnect(Gate.ClientLinker cltLinker)
        {
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_HallServer(pkg).LeaveMap(pkg, cltLinker.mForwardInfo.Handle);
            pkg.DoCommand(cltLinker.mForwardInfo.Gate2PlanesConnect, RPC.CommandTargetType.DefaultType);
        }
        #endregion

        bool IsSameClientLinker(Gate.ClientLinker linker, CSUtility.Net.NetConnection connect)
        {
            Gate.ClientLinker cltLinker = connect.m_BindData as Gate.ClientLinker;
            if (cltLinker == null)
            {
                return false;
            }
            if (linker != cltLinker)
            {
                return false;
            }
            if (linker.LinkerSerialId != cltLinker.LinkerSerialId)
            {
                return false;
            }
            return true;
        }

        #region RPC method

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public sbyte TestRPCVersion(int clientHash, int serverHash)
        {
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //#warning 临时处理，记得改回来
            //            return 1;
            //
            if (RPCClientVersion.GetManager().RPCHashCode != clientHash)
                return -1;
            if (RPCServerVersion.GetManager().RPCHashCode != serverHash)
                return -2;
            return 1;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void TryEnterGame(Guid mapSourceId,UInt16 roleTemplateId, CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            var cltLinker = connect.m_BindData as Gate.ClientLinker;
            var conn = connect as SCore.TcpServer.TcpConnect;
            if (conn == null)
                return;

            if (cltLinker == null)
            {
                connect.Disconnect();
                return;
            }
            mDataConnect.mLimitLevel = 100;
            //从数据服务器获取玩家角色信息
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_DataServer(pkg).LoginRole(pkg);
            pkg.WaitDoCommand(mDataConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                {
                    RPC.PackageWriter retPkg = new RPC.PackageWriter();
                    retPkg.Write((sbyte)-110);
                    retPkg.DoReturnGate2Client(fwd);
                    return;
                }
                sbyte successed = -1;
                _io.Read(out successed);
                if (successed != 1)
                {
                    RPC.PackageWriter retPkg = new RPC.PackageWriter();
                    retPkg.Write(successed);
                    retPkg.DoReturnGate2Client(fwd);

                    if (successed != -1)
                    {
                        connect.Disconnect();
                    }
                    return;
                }
                Guid hallServerId = Guid.Empty;
                if (successed == 1)
                {
                    _io.Read(out hallServerId);
                }
                cltLinker.mForwardInfo.LimitLevel = 100;
                cltLinker.mForwardInfo.Gate2PlanesConnect = FindHallConnect(hallServerId);
                cltLinker.mForwardInfo.Gate2PlanesConnect.mLimitLevel = 100;
                if (cltLinker.mForwardInfo.Gate2PlanesConnect == null)
                {
                    RefreshHallConnect();

                    RPC.PackageWriter retPkg = new RPC.PackageWriter();
                    retPkg.Write((sbyte)-9);//服务器忙，稍后再尝试
                    retPkg.DoReturnGate2Client(fwd);
                    return;
                }
                //告诉位面服务器玩家进入
                RPC.PackageWriter pkg0 = new RPC.PackageWriter();

                H_RPCRoot.smInstance.HGet_HallServer(pkg0).EnterMap(pkg0, cltLinker.mForwardInfo.Handle,conn.IpAddress,conn.Port,mapSourceId,roleTemplateId);
                pkg0.WaitDoCommand(cltLinker.mForwardInfo.Gate2PlanesConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _pkg, bool bTimeOut1)
                {
                    if (IsSameClientLinker(cltLinker, connect) == false)
                    {
                        Log.FileLog.WriteLine("ClientLinker Error===>IsSameClientLinker");
                        connect.Disconnect();
                        return;
                    }

                    #region 玩家进入位面
                    sbyte result = -1;
                    _pkg.Read(out result);

                    switch (result)
                    {
                        case 1:
                            {
                                var pd = new GameData.Role.PlayerData();
                                UInt32 singleId;
                                _pkg.Read(out singleId);
                                UInt16 indexInMap;
                                _pkg.Read(out indexInMap);
                                UInt16 indexInServer;
                                _pkg.Read(out indexInServer);

                                cltLinker.mForwardInfo.PlayerIndexInMap = indexInMap;
                                cltLinker.mForwardInfo.MapIndexInServer = indexInServer;
                   
                                _pkg.Read(pd);
                                
                                RPC.PackageWriter retPkg0 = new RPC.PackageWriter();

                                retPkg0.Write((sbyte)1);//玩家进入位面终于成功了！
                                retPkg0.Write(pd);
                                retPkg0.Write(singleId);
                                cltLinker.mForwardInfo.RoleId = pd.RoleId;

                                int count = 0;
                                _pkg.Read(out count);
                                retPkg0.Write(count);
                                for (int i = 0; i < count; i++)
                                {
                                    GameData.Skill.SkillData skill = new GameData.Skill.SkillData();
                                    _pkg.Read(skill);
                                    retPkg0.Write(skill);
                                }
                                retPkg0.DoReturnGate2Client(fwd);
                                cltLinker.LandStep = Gate.LandStep.EnterGame;
                            }
                            break;
                        default:
                            {
                                //DisconnectPlayer(pd.PlayerDetail.AccountId, (sbyte)EServerType.Gate);
                                RPC.PackageWriter retPkg = new RPC.PackageWriter();
                                retPkg.Write((sbyte)-1);//玩家进入位面失败
                                retPkg.DoReturnGate2Client(fwd);
                            }
                            break;
                    }
                    #endregion
                };
            };
        }

        #region GM指令

        /// <summary>
        /// 重新读取数据模板
        /// </summary>
        /// <param name="templateType">模板类型</param>
        /// <param name="id">模板ID</param>
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.GM, false)]
        public void GM_ReloadTemplate(string templateType, UInt16 id, Byte opType, CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            var cltLinker = connect.m_BindData as Gate.ClientLinker;
            var conn = connect as SCore.TcpServer.TcpConnect;
            if (conn == null)
                return;

            if (cltLinker == null)
            {
                connect.Disconnect();
                return;
            }

            RPC.PackageWriter pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_HallServer(pkg).GM_ReloadTemplate(pkg, templateType, id, opType);
            pkg.DoCommand(cltLinker.mForwardInfo.Gate2PlanesConnect, RPC.CommandTargetType.DefaultType);
        }

        #endregion

        #endregion

        public class RPCGateServerForwardImp1 : RPC.RPCGateServerForward
        {
            public GateServer mServer;
            public override RPC.RPCForwardInfo GetForwardInfo(CSUtility.Net.NetConnection sender)
            {
                SCore.TcpServer.TcpConnect connect = sender as SCore.TcpServer.TcpConnect;
                if (connect == null)
                    return null;

                Gate.ClientLinker cltLinker = connect.m_BindData as Gate.ClientLinker;
                if (cltLinker == null)
                    return null;
                return cltLinker.mForwardInfo;
            }
            public override CSUtility.Net.NetConnection GetConnectByHandle(System.UInt16 handle)
            {
                Gate.ClientLinker cltLinker = mServer.FindClientLinker(handle);
                if (cltLinker == null)
                    return null;
                return cltLinker.mForwardInfo.Gate2ClientConnect;
            }
        }
    }
}