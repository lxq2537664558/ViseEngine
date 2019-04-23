using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallServer
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

        Server.HallServer mHallServer = new Server.HallServer();
        public Server.HallServer HallServer
        {
            get { return mHallServer; }
        }

        Server.RPCRoot mRoot;
        public void Start(string[] args)
        {            
            Server.HallServerParameter parameter = new Server.HallServerParameter();

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
                string fullPathname = "server/srvcfg/HallServer.hallsrv";
                if (false == CSUtility.Support.IConfigurator.FillProperty(parameter, fullPathname))
                {
                    //System.Windows.Forms.MessageBox.Show("请检查服务器配置文件");
                    CSUtility.Support.IConfigurator.SaveProperty(parameter, "PlanesServer", fullPathname);
                }

            }

            try
            {
                mHallServer.Start(parameter);
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
                Log.FileLog.Instance.Flush();
            }

            mRoot = new Server.RPCRoot();
            mRoot.mHallServer = mHallServer;
            RPC.RPCNetworkMgr.Instance.mRootObject = mRoot;
        }

        public void Stop()
        {
            mHallServer.Stop();
        }

        public void Tick()
        {
            try
            {
                mHallServer.Tick();
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
                Log.FileLog.Instance.Flush();
            }
        }
    }
}
