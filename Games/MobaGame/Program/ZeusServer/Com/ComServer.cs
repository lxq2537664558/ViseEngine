using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public enum ComServerState
    {
        None,
        WaitRegServer,
        Working,
    }

    [CSUtility.Editor.CDataEditorAttribute(".comsrv")]
    public class ComServerParameter
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

        UInt16 mListenPort = 24000;
        [CSUtility.Support.DataValueAttribute("ListenPort", false)]
        public UInt16 ListenPort
        {
            get { return mListenPort; }
            set { mListenPort = value; }
        }

        string mDateBaseIP = "192.168.1.139";
        [CSUtility.Support.DataValueAttribute("DateBaseIP", false)]
        public string DateBaseIP
        {
            get { return mDateBaseIP; }
            set { mDateBaseIP = value; }
        }

        Guid mServerId = Guid.NewGuid();
        [CSUtility.Support.DataValueAttribute("ServerId", false)]
        public Guid ServerId
        {
            get { return mServerId; }
            set { mServerId = value; }
        }

        string mDBName = "zeus";
        [CSUtility.Support.DataValueAttribute("DBName", false)]
        public string DBName
        {
            get { return mDBName; }
            set { mDBName = value; }
        }
    }

    [RPC.RPCClassAttribute(typeof(ComServer))]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class ComServer
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion

        static ComServer mInstance = null;
        public static ComServer Instance
        {
            get { return mInstance; }
            set { mInstance = value; }
        }

        #region 核心数据
        ComServerState mLinkState = ComServerState.None;
        public ComServerState LinkState
        {
            get { return mLinkState; }
        }

        protected SCore.TcpServer.TcpServerHP mTcpSrv = new SCore.TcpServer.TcpServerHP();
        protected SCore.TcpServer.TcpClient mRegisterConnect = new SCore.TcpServer.TcpClient();

        ComServerParameter mParameter;
        #endregion

        #region 总操作
        public void Start(ComServerParameter parameter)
        {
            Stop();
                       
            mParameter = parameter;
            mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.NewConnect += this.PlaneServerConnected;
            mTcpSrv.CloseConnect += this.PlaneServerDisConnected;

            mRegisterConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegisterConnect.NewConnect += this.OnRegisterConnected;

            SCore.AsyncExecuteThreadManager.Instance.InitManager(2);
            SCore.AsyncExecuteThreadManager.Instance.StartThread();

            mLinkState = ComServerState.WaitRegServer;
            mRegisterConnect.Connect(parameter.RegServerIP, parameter.RegServerPort);            

//             this.UserRoleManager.DBConnect.DBConnectStr = System.String.Format("server={0};database=zeus;uid=ZeusServer;pwd=123imgod;MultipleActiveResultSets=True", mParameter.DateBaseIP);
//             this.UserRoleManager.DBConnect.OpenConnect(parameter.DateBaseIP, parameter.DBName);
//             GuildManager.Instance.DB_InitAllGuilds();
//             CSCommon.Data.Item.ShopMenuTemplateManager.Instance.LoadAllTemplate();
//             CSCommon.Data.Guild.GuildTemplateManager.Instance.LoadGuildCommonPropertys();

            Log.FileLog.Instance.Begin("ComServer.log", false);
            Log.FileLog.WriteLine("ComServer Start!");            

            mInstance = this;
        }
       
        public void Stop()
        {
            SCore.AsyncExecuteThreadManager.Instance.StopThread();
            mTcpSrv.ReceiveData -= RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.Close();

            Log.FileLog.Instance.End();
        }

        Int64 mTryRegServerReconnectTime;
        public void Tick()
        {
            IServer.Instance.Tick();

            var time = CSUtility.Helper.LogicTimer.GetTickCount();
            if (mLinkState == ComServerState.WaitRegServer)
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
            else if (mLinkState == ComServerState.Working)
            {
                if (mRegisterConnect.State == CSUtility.Net.NetState.Disconnect || mRegisterConnect.State == CSUtility.Net.NetState.Invalid)
                {
                    if (time - mTryRegServerReconnectTime > 3000)
                    {
                        mTryRegServerReconnectTime = time;
                        mRegisterConnect.Reconnect();
                    }
//                     if (this.UserRoleManager.DBConnect.IsValidConnect() == false)
//                     {
//                         this.UserRoleManager.DBConnect.ReOpen();
//                     }
                }
            }

            mRegisterConnect.Update();
            mTcpSrv.Update();
            RPC.RPCNetworkMgr.Instance.Tick(IServer.Instance.GetElapseMilliSecondTime());
        }
        #endregion

        #region 服务器注册流程        
        void PlaneServerDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

        }

        void PlaneServerConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

        }

        void OnRegisterConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

            RPC.PackageWriter pkg = new RPC.PackageWriter();

            H_RPCRoot.smInstance.HGet_RegServer(pkg).RegComServer(pkg, mParameter.ServerId, mParameter.ListenPort);
            pkg.WaitDoCommand(mRegisterConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                sbyte success = -1;
                _io.Read(out success);
                if (success == 1)
                {
                    if (mLinkState != ComServerState.Working)
                    {
                        if (false == mTcpSrv.Open(SCore.TcpServer.TcpOption.ForComServer, mParameter.ListenPort))
                            return;
                    }

                    mLinkState = ComServerState.Working;
                }
            };
        }
        #endregion
    }
}
