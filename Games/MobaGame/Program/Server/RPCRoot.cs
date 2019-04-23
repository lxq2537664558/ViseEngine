using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [RPC.RPCClassAttribute(typeof(RPCRoot))]
    public class RPCRoot : RPC.RPCObject
    {
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }

        #region RPC ChildObject
        public RegisterServer mRegServer;
        [RPC.RPCChildObjectAttribute(0, (int)RPC.RPCExecuteLimitLevel.All, false)]
        public RegisterServer RegServer
        {
            get { return mRegServer; }
        }
        public GateServer mGateServer;
        [RPC.RPCChildObjectAttribute(1, (int)RPC.RPCExecuteLimitLevel.All, false)]
        public GateServer GateServer
        {
            get { return mGateServer; }
        }
        public DataServer mDataServer;
        [RPC.RPCChildObjectAttribute(2, (int)RPC.RPCExecuteLimitLevel.All, false)]
        public DataServer DataServer
        {
            get { return mDataServer; }
        }
        public HallServer mHallServer;
        [RPC.RPCChildObjectAttribute(3, (int)RPC.RPCExecuteLimitLevel.All, false)]
        public HallServer HallServer
        {
            get { return mHallServer; }
        }
        public PathFindServer mPathFindServer;
        [RPC.RPCChildObjectAttribute(4, (int)RPC.RPCExecuteLimitLevel.All, false)]
        public PathFindServer PathFindServer
        {
            get { return mPathFindServer; }
        }
        public ComServer mComServer;
        [RPC.RPCChildObjectAttribute(5, (int)RPC.RPCExecuteLimitLevel.All, false)]
        public ComServer ComServer
        {
            get { return mComServer; }
        }
        public Server.LogServer mLogServer;
        [RPC.RPCChildObjectAttribute(6, (int)RPC.RPCExecuteLimitLevel.All, false)]
        public Server.LogServer LogServer
        {
            get { return mLogServer; }
        }

        public Server.MonitorServer mMonitorServer;
        [RPC.RPCChildObjectAttribute(7, (int)RPC.RPCExecuteLimitLevel.All, false)]
        public Server.MonitorServer MonitorServer
        {
            get { return mMonitorServer; }
        }
        #endregion        
    }
}
