using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Server
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

        UInt16 mListenPort = 9998;
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

        public void Start(MonitorConfig parameter)
        {
            mTcpSrv.ReceiveData += RPC.RPCNetworkMgr.Instance.ServerReceiveData;
            mTcpSrv.NewConnect += this.ClientConnected;
            mTcpSrv.CloseConnect += this.ClientDisConnected;

            InitPorts();

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

        #region 端口管理
        Queue<UInt16> mServerPorts = new Queue<UInt16>();
        UInt16 mStartPort = 32000;
        UInt16 mEndPort = 33000;
        void InitPorts()
        {
            for (UInt16 i = mStartPort; i < mEndPort + 1; ++i)
            {
                mServerPorts.Enqueue(i);
            }
        }

        List<ProcessInfo> mProcessList = new List<ProcessInfo>();
        ProcessInfo GetProcessInfo(Int32 id)
        {
            foreach(var i in mProcessList)
            {
                if (i.ProcessId == id)
                    return i;
            }

            return null;
        }

        void TickProcess()
        {
            lock(mProcessList)
            {
                foreach (var i in mProcessList)
                {
                    var proc = Process.GetProcessById(i.ProcessId);
                    if(proc == null)
                    {
                        mServerPorts.Enqueue(i.Port);
                        mProcessList.Remove(i);
                        break;
                    }                                        
                }
            }
        }
        #endregion

        #region RPCMethod
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void StartNewServer(string name,string args,CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            var port = mServerPorts.Dequeue();            

            var proc = System.Diagnostics.Process.Start(name + ".exe", port.ToString() + " " + args);

            mProcessList.Add(new ProcessInfo() {ProcessId = proc.Id, CurProcess = proc, Port = port });

            RPC.PackageWriter pkg = new RPC.PackageWriter();
            pkg.Write(proc.Id);
            pkg.Write(port);
            pkg.DoReturnCommand2(connect, fwd.ReturnSerialId);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Developer, true)]
        public void StopServer(int processId)
        {
            try
            {
                var procInfo = GetProcessInfo(processId);
                if (procInfo != null)
                {                    
                    if (!procInfo.CurProcess.HasExited)
                        procInfo.CurProcess.Kill();
                    mServerPorts.Enqueue(procInfo.Port);
                }               
            }
            catch (Exception)
            {

            }
        }
        #endregion
    }

    public class ProcessInfo
    {
        Int32 mProcessId;
        public Int32 ProcessId
        {
            get { return mProcessId; }
            set { mProcessId = value; }
        }

        Process mCurProcess = new Process();
        public Process CurProcess
        {
            get { return mCurProcess; }
            set { mCurProcess = value; }
        }

        UInt16 mPort;
        public UInt16 Port
        {
            get { return mPort; }
            set { mPort = value; }
        }
    }
}
