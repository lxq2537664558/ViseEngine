using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public enum PathFindServerState
    {
        None,
        WaitRegServer,
        Working,
    }

    [CSUtility.Editor.CDataEditorAttribute(".pfsrv")]
    public class PathFindServerParameter
    {
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
        UInt16 mListenPort = 25000;
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

        //缺省0的话，就根据CPU内核数量开启
        UInt16 mPFThreadNumber = 0;
        [CSUtility.Support.DataValueAttribute("PFThreadNumber", false)]
        public UInt16 PFThreadNumber
        {
            get { return mPFThreadNumber; }
            set { mPFThreadNumber = value; }
        }
    }
    [RPC.RPCClassAttribute(typeof(PathFindServer))]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class PathFindServer : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion

        static PathFindServer mInstance = null;
        public static PathFindServer Instance
        {
            get { return mInstance; }
            set { mInstance = value; }
        }

        #region 核心数据
        protected SCore.TcpServer.TcpServerHP mTcpSrv = new SCore.TcpServer.TcpServerHP();
        protected SCore.TcpServer.TcpClient mRegisterConnect = new SCore.TcpServer.TcpClient();

        PathFindServerState mLinkState = PathFindServerState.None;
        public PathFindServerState LinkState
        {
            get { return mLinkState; }
        }

        PathFindServerParameter mParameter;
        public PathFindServerParameter Parameter
        {
            get { return mParameter; }
        }

        #endregion

        #region 总操作
        public void Start(PathFindServerParameter parameter)
        {
            Stop();

            mParameter = parameter;

            Path.PathFinder.PathFinderManager.Instance.StartThread(parameter.PFThreadNumber);

            mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.CloseConnect += this.ServerDisConnected;
            mTcpSrv.NewConnect += this.ServerConnected;
            mRegisterConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegisterConnect.NewConnect += this.OnRegisterConnected;

            mLinkState = PathFindServerState.WaitRegServer;
            mRegisterConnect.Connect(parameter.RegServerIP, parameter.RegServerPort);            

            Log.FileLog.Instance.Begin("PathFindServer.log", false);
            Log.FileLog.WriteLine("PathFindServer Start!");
        }

        public void Stop()
        {
            Path.PathFinder.PathFinderManager.Instance.StopThread();

            mTcpSrv.ReceiveData -= RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.CloseConnect -= this.ServerDisConnected;
            mTcpSrv.NewConnect -= this.ServerConnected;
            mRegisterConnect.ReceiveData -= RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegisterConnect.NewConnect -= this.OnRegisterConnected;

            mRegisterConnect.Close();
            mTcpSrv.Close();
            mLinkState = PathFindServerState.None;

            Log.FileLog.Instance.End();
        }

        Int64 mTryRegServerReconnectTime;
        public void Tick()
        {
            IServer.Instance.Tick();
            var time = CSUtility.Helper.LogicTimer.GetTickCount();
            if (mLinkState == PathFindServerState.WaitRegServer)
            {
                //每过一段时间尝试连接一次
                if (mRegisterConnect.State != CSUtility.Net.NetState.Connect)
                {
                    if (time - mTryRegServerReconnectTime > 3000)
                    {
                        mTryRegServerReconnectTime = time;
                        mRegisterConnect.Reconnect();
                    }
                }
            }
            else if (mLinkState == PathFindServerState.Working)
            {
                if (mRegisterConnect.State == CSUtility.Net.NetState.Disconnect || mRegisterConnect.State == CSUtility.Net.NetState.Invalid)
                {
                    if (time - mTryRegServerReconnectTime > 3000)
                    {
                        mTryRegServerReconnectTime = time;
                        mRegisterConnect.Reconnect();
                    }
                }
            }

            mRegisterConnect.Update();
            mTcpSrv.Update();
            RPC.RPCNetworkMgr.Instance.Tick(IServer.Instance.GetElapseMilliSecondTime());
        }
        #endregion

        void OnRegisterConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;            
            
            RPC.PackageWriter pkg = new RPC.PackageWriter();

            H_RPCRoot.smInstance.HGet_RegServer(pkg).RegPathFindServer(pkg, mParameter.ServerId, mParameter.ListenPort);            
            pkg.WaitDoCommand(mRegisterConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                sbyte success = -1;
                _io.Read(out success);
                if (success == 1)
                {
                    if (mLinkState != PathFindServerState.Working)
                    {
                        if (false == mTcpSrv.Open(SCore.TcpServer.TcpOption.ForPathServer, mParameter.ListenPort))
                            return;
                    }

                    mLinkState = PathFindServerState.Working;
                }
            };            
        }

        public void ServerConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {

        }

        public void ServerDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {

        }

        #region RPC method

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true, true)]
        public void GlobalMapFindPath(Guid hallsId, Guid mapSourceId, Guid mapInstanceId, Guid roleId, SlimDX.Vector3 from, SlimDX.Vector3 to, CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            Path.PathFinder.PathFinderManager.Instance.QueryGlobalMapPath(connect as SCore.TcpServer.TcpConnect, fwd.ReturnSerialId, hallsId, mapSourceId, mapInstanceId, roleId, from, to, true);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void ReloadNavigation(Guid planesId, Guid mapSourceId, RPC.RPCForwardInfo fwd)
        {
            Path.PathFinder.PathFinderManager.Instance.ReloadNavigation(planesId, mapSourceId);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void SetMapBlocks(Guid planesId, Guid mapSourceId, Guid mapInstanceId, RPC.DataReader modifyBlocks, CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            Path.PathFinder.PathFinderManager.Instance.SetMapBlocks(planesId, mapSourceId, mapInstanceId, modifyBlocks);
        }

        #endregion

    }
}
