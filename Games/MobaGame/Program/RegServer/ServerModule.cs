using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegServer
{
    public class ServerModule
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

        Server.RegisterServer mRegServer = new Server.RegisterServer();
        public Server.RegisterServer RegServer
        {
            get { return mRegServer; }
        }

        Server.RPCRoot mRoot;        
        public void Start(string[] args)
        {
            Server.RegisterServerParameter parameter = new Server.RegisterServerParameter();

            if (args.Length >= 3)
            {
                parameter.ListenPort = Convert.ToUInt16(args[0]);
            }
            else
            {
                if (!System.IO.Directory.Exists(Server.IServer.Instance.ExePath + "srvcfg"))
                    System.IO.Directory.CreateDirectory(Server.IServer.Instance.ExePath + "srvcfg");
                string fullPathname = "server/srvcfg/RegServer.regsrv";
                if (false == CSUtility.Support.IConfigurator.FillProperty(parameter, fullPathname))
                {
                    //System.Windows.Forms.MessageBox.Show("请检查服务器配置文件");
                    CSUtility.Support.IConfigurator.SaveProperty(parameter, "RegServer", fullPathname);
                }
            }                        

            mRegServer.Start(parameter);
            mRoot = new Server.RPCRoot();
            mRoot.mRegServer = mRegServer;
            RPC.RPCNetworkMgr.Instance.mRootObject = mRoot;            
        }

        public void Stop()
        {
            mRegServer.Stop();
        }

        public void Tick()
        {
            mRegServer.Tick();
        }
    }
}
