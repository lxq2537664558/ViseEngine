using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitor
{
    [CSUtility.Editor.CDataEditorAttribute(".motsrv")]
    public class MonitorConfig
    {
        string mLocalNetIp = "127.0.0.1";
        [CSUtility.Support.DataValueAttribute("LocalNetIp", false)]
        public string LocalNetIp
        {
            get { return mLocalNetIp; }
            set { mLocalNetIp = value; }
        }

        UInt16 mListenPort = 9999;
        [CSUtility.Support.DataValueAttribute("ListenPort", false)]
        public UInt16 ListenPort
        {
            get { return mListenPort; }
            set { mListenPort = value; }
        }
    }

    [RPC.RPCClassAttribute(typeof(MonitorServer))]
    public class MonitorServer : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion  

        static MonitorServer smInstance = new MonitorServer();
        public static MonitorServer Instance
        {
            get { return smInstance; }
        }

        SCore.TcpServer.TcpServerHP mTcpSrv = new SCore.TcpServer.TcpServerHP();

        public void Start()
        {
            MonitorConfig parameter = new MonitorConfig();

            if (!System.IO.Directory.Exists(Server.IServer.Instance.ExePath + "srvcfg"))
                System.IO.Directory.CreateDirectory(Server.IServer.Instance.ExePath + "srvcfg");
            string fullPathname = "srvcfg/MonitorServer.motsrv";
            if (false == CSUtility.Support.IConfigurator.FillProperty(parameter, fullPathname))
            {
                //System.Windows.Forms.MessageBox.Show("请检查服务器配置文件");
                CSUtility.Support.IConfigurator.SaveProperty(parameter, "MonitorServer", fullPathname);
            }

            var assembly = CSUtility.Program.GetAssemblyFromDllFileName("ServerMonitor.dll");
            RPC.RPCEntrance.BuildRPCMethordExecuter(assembly);

            mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.NewConnect += this.ClientConnected;
            mTcpSrv.CloseConnect += this.ClientDisConnected;

            if (false == mTcpSrv.Open(SCore.TcpServer.TcpOption.ForRegServer, parameter.ListenPort))//, mParameter.GlobalNetIp))//在9999端口收听客户端登陆请求
                return;            
        }

        public void Stop()
        {
            mTcpSrv.ReceiveData -= RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.NewConnect -= this.ClientConnected;
            mTcpSrv.CloseConnect -= this.ClientDisConnected;

            mTcpSrv.Close();
        }

        
        public void Tick()
        {
            Server.IServer.Instance.Tick();

            mTcpSrv.Update();
            RPC.RPCNetworkMgr.Instance.Tick(Server.IServer.Instance.GetElapseMilliSecondTime());            
        }

        void ClientConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {

        }
        public void ClientDisConnected(SCore.TcpServer.TcpConnect pConnect, SCore.TcpServer.TcpServer pServer, byte[] pData, int nLength)
        {
        }
    }
}
