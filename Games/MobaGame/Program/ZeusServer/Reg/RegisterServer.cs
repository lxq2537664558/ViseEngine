using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    public enum RegServerState
    {
        None,
        Working,
    }

    public class ServerInfo
    {
        public Guid Id;
        public string Ip;
        public UInt16 Port;

        public CSUtility.Net.NetConnection Connect;

        public int LinkNumber;
    }

    [CSUtility.Editor.CDataEditorAttribute(".regsrv")]
    public class RegisterServerParameter
    {
        string mLocalNetIp = "127.0.0.1";
        [CSUtility.Support.DataValueAttribute("LocalNetIp", false)]
        public string LocalNetIp
        {
            get { return mLocalNetIp; }
            set { mLocalNetIp = value; }
        }
        UInt16 mListenPort = 8888;
        [CSUtility.Support.DataValueAttribute("ListenPort", false)]
        public UInt16 ListenPort
        {
            get { return mListenPort; }
            set { mListenPort = value; }
        }

        string mGlobalNetIp = "127.0.0.1";
        [CSUtility.Support.DataValueAttribute("GlobalNetIp", false)]
        public string GlobalNetIp
        {
            get { return mGlobalNetIp; }
            set { mGlobalNetIp = value; }
        }
        UInt16 mClientListenPort = 9998;
        [CSUtility.Support.DataValueAttribute("ClientListenPort", false)]
        public UInt16 ClientListenPort
        {
            get { return mClientListenPort; }
            set { mClientListenPort = value; }
        }
    }

    class ClientLinkedInfo
    {
        public long ConnectedTime = CSUtility.Helper.LogicTimer.GetTickCount();
        public int LandStep = 0;
    }

    [RPC.RPCClassAttribute(typeof(RegisterServer))]
    public class RegisterServer : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion  

        public Dictionary<UInt16,RPC.RPCWaitHandle> WaitHandles
        {
            get { return RPC.RPCNetworkMgr.Instance.WaitHandles; }
        }

        #region 核心数据
        RegServerState mLinkState = RegServerState.None;
        public RegServerState LinkState
        {
            get { return mLinkState; }
        }

        SCore.TcpServer.TcpServerHP mTcpSrv = new SCore.TcpServer.TcpServerHP();
        SCore.TcpServer.TcpServerHP mClientLoginSrv = new SCore.TcpServer.TcpServerHP();
        RegisterServerParameter mParameter;
        #endregion

        #region 总操作
        public void Start(RegisterServerParameter parameter)
        {
            if (mLinkState != RegServerState.None)
                return;

            mParameter = parameter;

            mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.NewConnect += this.ServerConnected;
            mTcpSrv.CloseConnect += this.ServerDisConnected;

            mClientLoginSrv.LimitLevel = (int)RPC.RPCExecuteLimitLevel.Player;
            mClientLoginSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mClientLoginSrv.NewConnect += this.ClientConnected;
            mClientLoginSrv.CloseConnect += this.ClientDisConnected;

            if (false == mTcpSrv.Open(SCore.TcpServer.TcpOption.ForComServer, mParameter.ListenPort))//, mParameter.LocalNetIp))
                return;
            System.Diagnostics.Debug.WriteLine("注册服务器启动，可以接客了");

            if (false == mClientLoginSrv.Open(SCore.TcpServer.TcpOption.ForRegServer, parameter.ClientListenPort))//, mParameter.GlobalNetIp))//在9998端口收听客户端登陆请求
                return;
            System.Diagnostics.Debug.WriteLine("登陆服务器启动，可以接客了");
            mLinkState = RegServerState.Working;

            Log.FileLog.Instance.Begin("RegisterServer.log", false);
            Log.FileLog.WriteLine("RegisterServer Start ===== ok!");            
        }

        public void Stop()
        {
            mTcpSrv.ReceiveData -= RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.NewConnect -= this.ServerConnected;
            mTcpSrv.CloseConnect -= this.ServerDisConnected;

            mClientLoginSrv.ReceiveData -= RPC.RPCNetworkMgr.Instance.ServerReceiveData;

            mClientLoginSrv.Close();
            mTcpSrv.Close();

            System.Diagnostics.Debug.WriteLine("注册服务器关闭");
            mLinkState = RegServerState.None;

            Log.FileLog.Instance.End();
        }

        public void Tick()
        {
            IServer.Instance.Tick();

            mTcpSrv.Update();
            mClientLoginSrv.Update();
            RPC.RPCNetworkMgr.Instance.Tick(IServer.Instance.GetElapseMilliSecondTime());

            KickInvalidLinker();
        }
        #endregion

        #region 服务器各种注册流程
        List<SCore.TcpServer.TcpConnect> mAllClientShortConnect = new List<SCore.TcpServer.TcpConnect>();
        
        CSUtility.Net.NetEndPoint mDataServer;
        public CSUtility.Net.NetEndPoint DataServer
        {
            get { return mDataServer; }
        }
        CSUtility.Net.NetEndPoint mComServer;
        public CSUtility.Net.NetEndPoint ComServer
        {
            get { return mComServer; }
        }
        CSUtility.Net.NetEndPoint mLogServer;
        public CSUtility.Net.NetEndPoint LogServer
        {
            get { return mLogServer; }
        }
        Dictionary<CSUtility.Net.NetConnection, ServerInfo> mGateServers = new Dictionary<CSUtility.Net.NetConnection, ServerInfo>();
        public Dictionary<CSUtility.Net.NetConnection, ServerInfo> GateServers
        {
            get { return mGateServers; }
        }
        Dictionary<CSUtility.Net.NetConnection, ServerInfo> mHallServers = new Dictionary<CSUtility.Net.NetConnection, ServerInfo>();
        public Dictionary<CSUtility.Net.NetConnection, ServerInfo> HallServers
        {
            get { return mHallServers; }
        }
        Dictionary<CSUtility.Net.NetConnection, ServerInfo> mPathFindServers = new Dictionary<CSUtility.Net.NetConnection, ServerInfo>();
        public Dictionary<CSUtility.Net.NetConnection, ServerInfo> PathFindServers
        {
            get { return mPathFindServers; }
        }        

        public void ServerConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
            if (pServer != this.mTcpSrv)
            {
                Log.FileLog.WriteLine("警告：内部服务器接入到了对外出口(Connect)");
                return;
            }
            lock (this)
            {
                ServerInfo si = new ServerInfo();
                si.Ip = pConnect.IpAddress;
                si.Connect = pConnect;
                                    
                pConnect.mLimitLevel = this.mTcpSrv.LimitLevel;
                pConnect.m_BindData = si;
                Log.FileLog.WriteLine("有服务器从[{0}:{1}]接入，分配对外服务端口{2}", pConnect.IpAddress, pConnect.Port, si.Port);
            }
        }

        public void ServerDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
            if (pServer != this.mTcpSrv)
            {
                Log.FileLog.WriteLine("警告：内部服务器接入到了对外出口(DisConnect)");
                return;
            }
            lock (this)
            {
                ServerInfo si = pConnect.m_BindData as ServerInfo;
                //mAllConnect.Remove(pConnect);
                if (mDataServer != null && pConnect == mDataServer.Connect)
                {
                    mDataServer = null;
                    Log.FileLog.WriteLine("DataServer{0}断开连接，分配端口{1}", si.Id, si.Port);
                }
                else if (mLogServer != null && pConnect == mLogServer.Connect)
                {
                    mLogServer = null;
                    Log.FileLog.WriteLine("LogServer{0}断开连接，分配端口{1}", si.Id, si.Port);
                }
                else if (mComServer != null && pConnect == mComServer.Connect)
                {
                    mComServer = null;
                    Log.FileLog.WriteLine("ComServer{0}断开连接，分配端口{1}", si.Id, si.Port);
                }
                else if (mGateServers.ContainsKey(pConnect))
                {
                    mGateServers.Remove(pConnect);
                    Log.FileLog.WriteLine("GateServer{0}断开连接，分配端口{1}", si.Id, si.Port);
                }
                else if (mHallServers.ContainsKey(pConnect))
                {
                    mHallServers.Remove(pConnect);
                    Log.FileLog.WriteLine("PlanesServer{0}断开连接，分配端口{1}", si.Id, si.Port);
                }
                else if (PathFindServers.ContainsKey(pConnect))
                {
                    PathFindServers.Remove(pConnect);
                    Log.FileLog.WriteLine("PathFindServer{0}断开连接，分配端口{1}", si.Id, si.Port);
                }
                else
                {
                    Log.FileLog.WriteLine("未知服务器{0}[{1}:{2}]尚未注册到RegServer就断开连接，服务端口{3}", si.Id, pConnect.IpAddress, pConnect.Port, si.Port);
                }                
            }
        }

        public void ClientConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
            if (pServer != this.mClientLoginSrv)
            {
                Log.FileLog.WriteLine("警告：有客户端接入内部服务器(Connect)");
                return;
            }
            lock (mAllClientShortConnect)
            {
                pConnect.mLimitLevel = (int)RPC.RPCExecuteLimitLevel.Player;
                foreach (SCore.TcpServer.TcpConnect conn in mAllClientShortConnect)
                {
                    if (conn == pConnect)
                        return;
                }
                var linkedInfo = new ClientLinkedInfo();
                pConnect.m_BindData = linkedInfo;

                mAllClientShortConnect.Add(pConnect);
            }
        }

        public void ClientDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
            if (pServer != this.mClientLoginSrv)
            {
                Log.FileLog.WriteLine("警告：有客户端接入内部服务器(DisConnect)");
                return;
            }
            lock (mAllClientShortConnect)
            {
                pConnect.m_BindData = null;
                if (mAllClientShortConnect.Contains(pConnect))
                {
                    mAllClientShortConnect.Remove(pConnect);
                }
                else
                {
                    Log.FileLog.WriteLine("神奇的bug，未知客户端断开连接");
                }
            }
        }

        long mPrevKickInvalidLinker = 0;
        void KickInvalidLinker()
        {
            var nowTime = CSUtility.Helper.LogicTimer.GetTickCount();
            if (nowTime - mPrevKickInvalidLinker < 10000)
                return;
            mPrevKickInvalidLinker = nowTime;

            try
            {
                System.Threading.Monitor.Enter(this);
                foreach (var i in mAllClientShortConnect)
                {
                    if (i == null)
                        continue;
                    var linkedInfo = i.m_BindData as ClientLinkedInfo;
                    if (linkedInfo == null)
                        continue;

                    if (nowTime - linkedInfo.ConnectedTime > 15000)
                    {
                        i.Disconnect();
                    }
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(this);
            }
        }

        public ServerInfo GetLowGateServerInfo()
        {
            int minValue = Int32.MaxValue;
            ServerInfo serverInfo = null;
            foreach (var l in mGateServers)
            {
                if (l.Value.LinkNumber < minValue)
                {
                    minValue = l.Value.LinkNumber;
                    serverInfo = l.Value;
                }
            }
            return serverInfo;
        }
        #endregion

        #region RPC Method
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public sbyte RegGateServer(Guid id,UInt16 port, string ip,CSUtility.Net.NetConnection connect)
        {
            ServerInfo si = connect.m_BindData as ServerInfo;
            si.Id = id;
            si.Ip = ip;
            si.Port = port;
            si.LinkNumber = 0;
            mGateServers[connect] = si;

            Log.FileLog.WriteLine("Gate服务器{0}注册，{1}:{2}", si.Id, si.Ip, si.Port);
            return 1;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public RPC.DataWriter GetGateServers()
        {
            RPC.DataWriter d = new RPC.DataWriter();
            System.Byte nCount = (Byte)mGateServers.Count;
            d.Write(nCount);
            foreach (var l in mGateServers)
            {
                d.Write(l.Value.Ip);
                d.Write(l.Value.Port);
            }
            return d;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public sbyte RegHallServer(Guid id,UInt16 port, CSUtility.Net.NetConnection connect)
        {
            if (id == Guid.Empty)
            {
                Log.FileLog.WriteLine("RegHallServer id is Empty");
            }
            foreach (var i in mHallServers)
            {
                if (i.Value.Id == id)
                {
                    mHallServers.Remove(i.Key);
                    mHallServers[connect] = i.Value;                  
                }
            }

            ServerInfo si = connect.m_BindData as ServerInfo;
            si.Id = id;
            si.Port = port;
            SCore.TcpServer.TcpConnect tcpConnect = connect as SCore.TcpServer.TcpConnect;
            si.Ip = tcpConnect.IpAddress;
            si.LinkNumber = 0;
            mHallServers[connect] = si;

            Log.FileLog.WriteLine("Hall服务器{0}注册，{1}:{2}", si.Id, si.Ip, si.Port);
            
            return 1;       
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public sbyte RegDataServer(Guid id,UInt16 port, CSUtility.Net.NetConnection connect)
        {
            ServerInfo si = connect.m_BindData as ServerInfo;
            if (mDataServer != null)
            {
                SCore.TcpServer.TcpConnect conn = mDataServer.Connect as SCore.TcpServer.TcpConnect;
                if (conn != null)
                {//数据服务器是唯一的，起来一个就要把另外一个踢下线
                    conn.Disconnect();
                }
            }
            mDataServer = new CSUtility.Net.NetEndPoint(si.Ip, port);
            mDataServer.Connect = connect;
            mDataServer.Id = id;           

            Log.FileLog.WriteLine("Data服务器{0}注册，{1}:{2}", id, si.Ip, si.Port);
            return 1;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public sbyte RegPathFindServer(Guid id, UInt16 port, CSUtility.Net.NetConnection connect)
        {
            ServerInfo si = connect.m_BindData as ServerInfo;
            si.Id = id;
            si.Port = port;
            si.LinkNumber = 0;

            PathFindServers[connect] = si;

            Log.FileLog.WriteLine("PathFind服务器{0}注册，{1}:{2}", si.Id, si.Ip, si.Port);
            return 1;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void GetPathFindServers(CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            retPkg.Write((Byte)PathFindServers.Count);
            foreach (var i in PathFindServers)
            {
                retPkg.Write(i.Value.Ip);
                retPkg.Write(i.Value.Port);
            }
            retPkg.DoReturnCommand2(connect, fwd.ReturnSerialId);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public sbyte RegComServer(Guid id,UInt16 port, CSUtility.Net.NetConnection connect)
        {
            ServerInfo si = connect.m_BindData as ServerInfo;
            si.Id = id;
            si.Port = port;
            si.LinkNumber = 0;

            if (mComServer != null)
            {
                SCore.TcpServer.TcpConnect conn = mComServer.Connect as SCore.TcpServer.TcpConnect;
                if (conn != null)
                {//Com服务器是唯一的，起来一个就要把另外一个踢下线
                    conn.Disconnect();
                }
            }
            mComServer = new CSUtility.Net.NetEndPoint(si.Ip, si.Port);
            mComServer.Connect = connect;
            mComServer.Id = id;

            Log.FileLog.WriteLine("Com服务器{0}注册，{1}:{2}", id, si.Ip, si.Port);
            return 1;
        }
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public sbyte RegLogServer(Guid id,UInt16 port, CSUtility.Net.NetConnection connect)
        {
            ServerInfo si = connect.m_BindData as ServerInfo;
            si.Id = id;
            si.Port = port;
            if (mLogServer != null)
            {
                SCore.TcpServer.TcpConnect conn = mLogServer.Connect as SCore.TcpServer.TcpConnect;
                if (conn != null)
                {//Log服务器是唯一的，起来一个就要把另外一个踢下线
                    conn.Disconnect();
                }
            }
            mLogServer = new CSUtility.Net.NetEndPoint(si.Ip, si.Port);
            mLogServer.Connect = connect;
            mLogServer.Id = id;

            Log.FileLog.WriteLine("Log服务器{0}注册，{1}:{2}", si.Id, si.Ip, si.Port);
            return 1;
        }
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void SetGateLinkNumber(CSUtility.Net.NetConnection connect, int num)
        {
            ServerInfo serverInfo;
            if (mGateServers.TryGetValue(connect, out serverInfo) == true)
            {
                serverInfo.LinkNumber = num;
            }
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public RPC.DataWriter GetLowGateServer(CSUtility.Net.NetConnection connect)
        {
            RPC.DataWriter d = new RPC.DataWriter();
            if (mGateServers.Count == 0)
            {
                return d;
            }

            var linkedInfo = connect.m_BindData as ClientLinkedInfo;
            if (linkedInfo == null)
            {
                return d;
            }

            int minValue = Int32.MaxValue;
            ServerInfo serverInfo = null;
            foreach (var l in mGateServers)
            {
                if (l.Value.LinkNumber < minValue)
                {
                    minValue = l.Value.LinkNumber;
                    serverInfo = l.Value;
                }
            }

            if (serverInfo != null)
            {
                d.Write(serverInfo.Ip);
                d.Write(serverInfo.Port);
            }

            linkedInfo.LandStep = 1;

            return d;
        }
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public RPC.DataWriter GetDataServer()
        {
            RPC.DataWriter d = new RPC.DataWriter();
            if (mDataServer == null)
            {
                d.Write((sbyte)(-1));
                return d;
            }
            d.Write((sbyte)(1));
            d.Write(mDataServer.IpAddress);
            d.Write(mDataServer.Port);

            return d;
        }
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public RPC.DataWriter GetComServer()
        {
            RPC.DataWriter d = new RPC.DataWriter();
            if (mComServer == null)
            {
                d.Write((sbyte)(-1));
                return d;
            }
            d.Write((sbyte)(1));
            d.Write(mComServer.IpAddress);
            d.Write(mComServer.Port);

            return d;
        }
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public RPC.DataWriter GetLogServer()
        {
            RPC.DataWriter d = new RPC.DataWriter();
            if (mLogServer == null)
            {
                d.Write((sbyte)(-1));
                return d;
            }
            d.Write((sbyte)(1));
            d.Write(mLogServer.IpAddress);
            d.Write(mLogServer.Port);

            return d;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public RPC.DataWriter GetHallServers()
        {
            RPC.DataWriter d = new RPC.DataWriter();

            Byte count = (Byte)mHallServers.Count;
            d.Write(count);
            foreach (var s in mHallServers)
            {
                if (s.Value.Id == Guid.Empty)
                {
                    Log.FileLog.WriteLine("GetPlanesServers时有的PlanesSever的Id不合法");
                    RPC.PackageWriter pkg = new RPC.PackageWriter();
                    H_RPCRoot.smInstance.HGet_HallServer(pkg).GetHallServerId(pkg);
                    pkg.WaitDoCommand(s.Value.Connect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
                    {
                        _io.Read(out s.Value.Id);
                    };
                }
                d.Write(s.Value.Id);
                d.Write(s.Value.Ip);
                d.Write(s.Value.Port);
            }

            return d;
        }


        List<GameData.HallsData> mAllHalls = new List<GameData.HallsData>();
        public List<GameData.HallsData> AllHalls
        {
            get { return mAllHalls; }
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, true)]
        public RPC.DataWriter GetAllHallsInfo()
        {
            RPC.DataWriter d = new RPC.DataWriter();

            lock (mAllHalls)
            {
                d.Write((UInt16)mAllHalls.Count);
                foreach (var i in mAllHalls)
                {
                    d.Write(i, true);
                }
            }

            return d;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void UpdatePlanesInfo(RPC.DataReader info)
        {
            lock (mAllHalls)
            {
                mAllHalls.Clear();
                UInt16 count = 0;
                info.Read(out count);
                for (UInt16 i = 0; i < count; i++)
                {
                    var pi = new GameData.HallsData();
                    info.Read(pi, false);
                    mAllHalls.Add(pi);
                }
            }
        }



        #endregion
    }
}
