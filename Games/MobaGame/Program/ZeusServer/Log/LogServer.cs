using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public enum LogServerState
    {
        None,
        WaitRegServer,
        Working,
    }

    [CSUtility.Editor.CDataEditorAttribute(".logsrv")]
    public class LogServerParameter
    {
        string mDateBaseIP = "192.168.1.139";
        [CSUtility.Support.DataValueAttribute("DateBaseIP", false)]
        public string DateBaseIP
        {
            get { return mDateBaseIP; }
            set { mDateBaseIP = value; }
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
        UInt16 mListenPort = 30000;
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

        string mDBName = "log_zeus";
        [CSUtility.Support.DataValueAttribute("DBName", false)]
        public string DBName
        {
            get { return mDBName; }
            set { mDBName = value; }
        }
    }

    [RPC.RPCClassAttribute(typeof(LogServer))]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class LogServer : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion

        #region 核心数据
        protected SCore.TcpServer.TcpServerHP mTcpSrv = new SCore.TcpServer.TcpServerHP();
        protected SCore.TcpServer.TcpClient mRegisterConnect = new SCore.TcpServer.TcpClient();

        LogServerState mLinkState = LogServerState.None;
        public LogServerState LinkState
        {
            get { return mLinkState; }
        }

        LogServerParameter mParameter;

        #endregion

        #region 总操作
        public void Start(LogServerParameter parameter)
        {
            Stop();
                          
            mParameter = parameter;

            Log.FileLog.Instance.Begin("LogServer.log", false);

            //LogSaverThread.Instance.StartThread(mParameter.DateBaseIP, mParameter.DBName);

            Log.FileLog.WriteLine("LogServer Start!");
            Log.FileLog.Instance.Flush();


            mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.CloseConnect += this.ServerDisConnected;
            mRegisterConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegisterConnect.NewConnect += this.OnRegisterConnected;

            mLinkState = LogServerState.WaitRegServer;
            mRegisterConnect.Connect(parameter.RegServerIP, parameter.RegServerPort);            
        }

        public void Stop()
        {
            //LogSaverThread.Instance.StopThread();

            mTcpSrv.ReceiveData -= RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.CloseConnect -= this.ServerDisConnected;
            mRegisterConnect.ReceiveData -= RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegisterConnect.NewConnect -= this.OnRegisterConnected;

            mRegisterConnect.Close();
            mTcpSrv.Close();
            System.Diagnostics.Debug.WriteLine("Log服务器关闭");
            mLinkState = LogServerState.None;

            Log.FileLog.Instance.End();
        }

        Int64 mTryRegServerReconnectTime;
        public void Tick()
        {
            IServer.Instance.Tick();
            var time = CSUtility.Helper.LogicTimer.GetTickCount();

            if (mRegisterConnect.State != CSUtility.Net.NetState.Connect)
            {
                if (time - mTryRegServerReconnectTime > 3000)
                {
                    mTryRegServerReconnectTime = time;
                    mRegisterConnect.Reconnect();
                }
            }
            mRegisterConnect.Update();
            mTcpSrv.Update();
            RPC.RPCNetworkMgr.Instance.Tick(IServer.Instance.GetElapseMilliSecondTime());
        }
        #endregion

        #region 服务器各种注册流程
        void OnRegisterConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

            RPC.PackageWriter pkg = new RPC.PackageWriter();

            H_RPCRoot.smInstance.HGet_RegServer(pkg).RegLogServer(pkg, mParameter.ServerId, mParameter.ListenPort);            
            pkg.WaitDoCommand(mRegisterConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                sbyte success = -1;
                _io.Read(out success);
                if (success == 1)
                {
                    if (mLinkState != LogServerState.Working)
                    {
                        if (false == mTcpSrv.Open(SCore.TcpServer.TcpOption.ForPathServer, mParameter.ListenPort))
                            return;
                    }

                    mLinkState = LogServerState.Working;
                }
            };
        }

        public void ServerDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {

        }
        #endregion

    }
}
