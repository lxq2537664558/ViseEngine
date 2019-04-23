using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public enum DataServerState
    {
        None,
        WaitRegServer,
        Working,
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class HallServerInfo
    {
        CSUtility.Net.NetEndPoint mEndPoint;
        public CSUtility.Net.NetEndPoint EndPoint
        {
            get { return mEndPoint; }
            set { mEndPoint = value; }
        }

        int mHallNumber = 0;
        public int HallNumber
        {
            get { return mHallNumber; }
            set { mHallNumber = value; }
        }

        int mGlobalMapNumber = 0;
        public int GlobalMapNumber
        {
            get { return mGlobalMapNumber; }
            set { mGlobalMapNumber = value; }
        }

        int mInstanceMapNumber = 0;
        public int InstanceMapNumber
        {
            get { return mInstanceMapNumber; }
            set { mInstanceMapNumber = value; }
        }

        int mNPCNumber = 0;
        public int NPCNumber
        {
            get { return mNPCNumber; }
            set { mNPCNumber = value; }
        }

        int mPlayerNumber = 0;
        public int PlayerNumber
        {
            get { return mPlayerNumber; }
            set { mPlayerNumber = value; }
        }

        float mServerPower = 1.0F;
        public float ServerPower
        {
            get { return mServerPower; }
            set { mServerPower = value; }
        }
    }

    [CSUtility.Editor.CDataEditorAttribute(".datasrv")]
    public class DataServerParameter
    {
        string mDateBaseIP = "192.168.1.139";
        [CSUtility.Support.DataValueAttribute("DateBaseIP", false)]
        public string DateBaseIP
        {
            get { return mDateBaseIP; }
            set { mDateBaseIP = value; }
        }

        string mLogDBIP = "192.168.1.139";
        [CSUtility.Support.DataValueAttribute("LogDBIP", false)]
        public string LogDBIP
        {
            get { return mLogDBIP; }
            set { mLogDBIP = value; }
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
        UInt16 mListenPort = 23000;
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
        UInt16 mSaveThreadNumber = 2;
        [CSUtility.Support.DataValueAttribute("SaveThreadNumber", false)]
        public UInt16 SaveThreadNumber
        {
            get { return mSaveThreadNumber; }
            set { mSaveThreadNumber = value; }
        }
        string mDBName = "zeus";
        [CSUtility.Support.DataValueAttribute("DBName", false)]
        public string DBName
        {
            get { return mDBName; }
            set { mDBName = value; }
        }

        string mLogDBName = "log_zeus";
        [CSUtility.Support.DataValueAttribute("LogDBName", false)]
        public string LogDBName
        {
            get { return mLogDBName; }
            set { mLogDBName = value; }
        }

        bool mLoadAllItemTemplate = true;
        [CSUtility.Support.DataValueAttribute("LoadAllItemTemplate", false)]
        public bool LoadAllItemTemplate
        {
            get { return mLoadAllItemTemplate; }
            set { mLoadAllItemTemplate = value; }
        }
    }

    [RPC.RPCClassAttribute(typeof(DataServer))]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class DataServer : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion

        static DataServer mInstance = null;
        public static DataServer Instance
        {
            get { return mInstance; }
            set { mInstance = value; }
        }

        #region 核心数据
        protected SCore.TcpServer.TcpServerHP mTcpSrv = new SCore.TcpServer.TcpServerHP();
        protected SCore.TcpServer.TcpClient mRegisterConnect = new SCore.TcpServer.TcpClient();
        public SCore.TcpServer.TcpClient RegisterConnect
        {
            get { return mRegisterConnect; }
        }

        DataServerState mLinkState = DataServerState.None;
        public DataServerState LinkState
        {
            get { return mLinkState; }
        }

        DataServerParameter mParameter;
        public DataServerParameter Parameter
        {
            get { return mParameter; }
        }

        SCore.DB.DBConnect mDBLoaderConnect;
        public SCore.DB.DBConnect DBLoaderConnect
        {
            get { return mDBLoaderConnect; }
        }

        SCore.DB.DBConnect mDBLogConnect;
        public SCore.DB.DBConnect DBLogConnect
        {
            get { return mDBLogConnect; }
        }
        #endregion

        #region 总操作
        public void Start(DataServerParameter parameter)
        {
            Stop();

            mParameter = parameter;
            mDBLoaderConnect = new SCore.DB.DBConnect();
            mDBLoaderConnect.OpenConnect(DataServer.Instance.Parameter.DateBaseIP, DataServer.Instance.Parameter.DBName);

            mDBLogConnect = new SCore.DB.DBConnect();
            mDBLogConnect.OpenConnect(DataServer.Instance.Parameter.LogDBIP, DataServer.Instance.Parameter.LogDBName);

            //CSUtility.Support.ClassInfoManager.Instance.Load(false);

            Log.FileLog.Instance.Begin("DataServer.log", false);

            Log.FileLog.WriteLine("DataServer Start!");
            Log.FileLog.Instance.Flush();

            try
            {
                Log.FileLog.WriteLine("DBConnect OK!");

                mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
                mTcpSrv.CloseConnect += this.ServerDisConnected;

                mRegisterConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
                mRegisterConnect.NewConnect += this.OnRegisterConnected;

                mLinkState = DataServerState.WaitRegServer;
                mRegisterConnect.Connect(parameter.RegServerIP, parameter.RegServerPort);                

                //CSUtility.Support.StringFilterHelper.Instance.InitFilter(CSUtility.Support.IFileManager.Instance.Root + "ZeusGame/Sensitive/SensitiveWord.txt");
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("位面数据器初始化失败!=>{0}", ex.StackTrace.ToString()), "Error");
            }           
        }

        public void Stop()
        {
            mTcpSrv.ReceiveData -= RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.CloseConnect -= this.ServerDisConnected;
            mRegisterConnect.ReceiveData -= RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            mRegisterConnect.NewConnect -= this.OnRegisterConnected;

            mRegisterConnect.Close();
            mTcpSrv.Close();
            
            System.Diagnostics.Debug.WriteLine("数据服务器关闭");
            mLinkState = DataServerState.None;

            Log.FileLog.Instance.End();
        }

        Int64 mTryRegServerReconnectTime;
        Int64 mTryRemoveLogoutPlayerTime;
        public void Tick()
        {
            IServer.Instance.Tick();
            var time = CSUtility.Helper.LogicTimer.GetHPTickCount();
            if (time - mTryRemoveLogoutPlayerTime > 3000)
            {//把已经完成数据存盘的玩家清理了
                mTryRemoveLogoutPlayerTime = time;
                //mPlayerManager.Tick();
            }

            switch (mLinkState)
            {
                case DataServerState.Working:
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
                    break;
                case DataServerState.WaitRegServer:
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
                    break;
            }

            mRegisterConnect.Update();
            mTcpSrv.Update();
            RPC.RPCNetworkMgr.Instance.Tick(IServer.Instance.GetElapseMilliSecondTime());
        }
        #endregion

        #region 服务器各种注册流程
        string SelectDataServerIP(string[] ips)
        {
            string result = ips[0];
            foreach (string s in ips)
            {
                if (s.IndexOf("192.168.") == 0)
                {//选择局域网内部的
                    return s;
                }
            }
            return result;
        }

        void OnRegisterConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
                return;

            RPC.PackageWriter pkg = new RPC.PackageWriter();

            H_RPCRoot.smInstance.HGet_RegServer(pkg).RegDataServer(pkg, mParameter.ServerId, mParameter.ListenPort);
            pkg.WaitDoCommand(mRegisterConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace(1, true)).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                sbyte success = -1;
                _io.Read(out success);
                if (success == 1)
                {
                    if (mLinkState != DataServerState.Working)
                    {
                        if (false == mTcpSrv.Open(SCore.TcpServer.TcpOption.ForDataServer, mParameter.ListenPort))
                            return;
                    }

                    mLinkState = DataServerState.Working;
                }
            };        
        }        
        public void ServerDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
            lock (this)
            {                
                if (mGateServers.ContainsKey(pConnect))
                {
                    mGateServers.Remove(pConnect);  
                }

                if (mHallServers.ContainsKey(pConnect))
                {
                    mHallServers.Remove(pConnect);
                }                                                
            }
        }
        #endregion


        Dictionary<CSUtility.Net.NetConnection, CSUtility.Net.NetEndPoint> mGateServers = new Dictionary<CSUtility.Net.NetConnection, CSUtility.Net.NetEndPoint>();
        public Dictionary<CSUtility.Net.NetConnection, CSUtility.Net.NetEndPoint> GateServers
        {
            get { return mGateServers; }
        }
        Dictionary<CSUtility.Net.NetConnection, HallServerInfo> mHallServers = new Dictionary<CSUtility.Net.NetConnection, HallServerInfo>();
        public Dictionary<CSUtility.Net.NetConnection, HallServerInfo> HallServers
        {
            get { return mHallServers; }
        }

        public CSUtility.Net.NetEndPoint FindHallServer(Guid serverId)
        {
            foreach (var i in mHallServers)
            {
                if (i.Value.EndPoint.Id == serverId)
                    return i.Value.EndPoint;
            }
            return null;
        }

        #region Server Connect
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public sbyte RegGateServer(string ip, UInt16 port, Guid id, CSUtility.Net.NetConnection connect)
        {
            CSUtility.Net.NetEndPoint nep = new CSUtility.Net.NetEndPoint(ip, port);
            nep.Id = id;
            nep.Connect = connect;

            CSUtility.Net.NetEndPoint oldnep;
            if (mGateServers.TryGetValue(connect, out oldnep) == true)
            {
                mGateServers[connect] = nep;
            }
            else
            {
                mGateServers.Add(connect, nep);
            }
            return 1;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public sbyte RegHallServer(string ip, UInt16 port, Guid id, float power, CSUtility.Net.NetConnection connect)
        {
            CSUtility.Net.NetEndPoint nep = new CSUtility.Net.NetEndPoint(ip, port);
            nep.Id = id;
            nep.Connect = connect;

            HallServerInfo oldnep;
            if (mHallServers.TryGetValue(connect, out oldnep) == true)
            {
                mHallServers[connect].EndPoint = nep;
            }
            else
            {
                oldnep = new HallServerInfo();
                oldnep.EndPoint = nep;
                oldnep.ServerPower = power;
                mHallServers.Add(connect, oldnep);
            }
            return 1;
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void LoginRole(CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            foreach (var server in mHallServers)
            {
       //         if (server.Key == connect)
                {
                    retPkg.Write((sbyte)1);
                    retPkg.Write(server.Value.EndPoint.Id);
                    retPkg.DoReturnCommand2(connect, fwd.ReturnSerialId);
                    return;
                }
            }
        }
        #endregion
    }
}
