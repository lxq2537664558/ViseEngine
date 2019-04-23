using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitor
{
    class ServerModule
    {
        static ServerModule smInstance = new ServerModule();
        public static ServerModule Instance
        {
            get { return smInstance; }
        }
        static ServerModule()
        {
            Server.IServer.Instance.LoadRPCModule("Server.dll");
        }

        Server.MonitorServer mMonitorServer = new Server.MonitorServer();
        public Server.MonitorServer MonitorServer
        {
            get { return mMonitorServer; }
        }

        Server.RPCRoot mRoot;
        public void Start()
        {
            Server.MonitorConfig parameter = new Server.MonitorConfig();

            if (!System.IO.Directory.Exists(Server.IServer.Instance.ExePath + "srvcfg"))
                System.IO.Directory.CreateDirectory(Server.IServer.Instance.ExePath + "srvcfg");
            string fullPathname = "server/srvcfg/MonitorServer.motsrv";
            if (false == CSUtility.Support.IConfigurator.FillProperty(parameter, fullPathname))
            {
                //System.Windows.Forms.MessageBox.Show("请检查服务器配置文件");
                CSUtility.Support.IConfigurator.SaveProperty(parameter, "MonitorServer", fullPathname);
            }

            mMonitorServer.Start(parameter);
            mRoot = new Server.RPCRoot();
            mRoot.mMonitorServer = mMonitorServer;
            RPC.RPCNetworkMgr.Instance.mRootObject = mRoot;
        }

        public void Stop()
        {
            mMonitorServer.Stop();
        }

        public void Tick()
        {
            mMonitorServer.Tick();
        }
    }
}
