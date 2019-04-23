using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComServer
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

        Server.ComServer mComServer = new Server.ComServer();
        public Server.ComServer ComServer
        {
            get { return mComServer; }
        }

        Server.RPCRoot mRoot;
        public void Start(string[] args)
        {
            Server.ComServerParameter parameter = new Server.ComServerParameter();
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
                string fullPathname = "server/srvcfg/ComServer.comsrv";
                if (false == CSUtility.Support.IConfigurator.FillProperty(parameter, fullPathname))
                {
                    //System.Windows.Forms.MessageBox.Show("请检查服务器配置文件");
                    CSUtility.Support.IConfigurator.SaveProperty(parameter, "ComServer", fullPathname);
                }
            }            

            mComServer.Start(parameter);
            mRoot = new Server.RPCRoot();
            mRoot.mComServer = mComServer;
            RPC.RPCNetworkMgr.Instance.mRootObject = mRoot;
        }

        public void Stop()
        {
            mComServer.Stop();
        }

        public void Tick()
        {
            try
            {
                mComServer.Tick();
            }
            catch (Exception ex)
            {

                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
                Log.FileLog.Instance.Flush();
            }            
        }
    }
}
