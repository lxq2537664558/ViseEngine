using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
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

        Server.DataServer mDataServer = new Server.DataServer();
        public Server.DataServer DataServer
        {
            get { return mDataServer; }
        }

        Server.RPCRoot mRoot;
        public void Start(string[] args)
        {            
            Server.DataServerParameter parameter = new Server.DataServerParameter();

            if (args.Length >= 3)
            {                
                parameter.ListenPort = Convert.ToUInt16(args[0]);
                parameter.RegServerIP = args[1];
                parameter.RegServerPort = Convert.ToUInt16(args[2]);
            }
            else
            {
                if (!System.IO.Directory.Exists(Server.IServer.Instance.ExePath + "srvcfg"))
                    System.IO.Directory.CreateDirectory(Server.IServer.Instance.ExePath + "srvcfg");
                string fullPathname = "server/srvcfg/DataServer.datasrv";
                if (false == CSUtility.Support.IConfigurator.FillProperty(parameter, fullPathname))
                {
                    //System.Windows.Forms.MessageBox.Show("请检查服务器配置文件");
                    CSUtility.Support.IConfigurator.SaveProperty(parameter, "DataServer", fullPathname);
                }
            }            

            Server.DataServer.Instance = mDataServer;
            mDataServer.Start(parameter);

            mRoot = new Server.RPCRoot();
            mRoot.mDataServer = mDataServer;
            RPC.RPCNetworkMgr.Instance.mRootObject = mRoot;
        }

        public void Stop()
        {
            mDataServer.Stop();
        }

        public void Tick()
        {
            mDataServer.Tick();
        }
    }
}
