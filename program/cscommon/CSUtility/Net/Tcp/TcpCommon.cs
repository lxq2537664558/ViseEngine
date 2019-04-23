using System;
using System.Collections.Generic;

namespace CSUtility.Net
{
    public class NetConnection
	{
	    public System.Object m_BindData;
		public int mLimitLevel;

        public virtual void SendBuffer(byte[] data, int offset, int count)
		{
			
		}

        public virtual void Disconnect()
		{
			
		}

        Dictionary<System.Reflection.MethodInfo, Int64> mCallTimes = new Dictionary<System.Reflection.MethodInfo, Int64>();
        //Dictionary<RPC.RPCMethodAttribute, Int64> m   CallTimes = new Dictionary<RPC.RPCMethodAttribute, Int64>();
        //public virtual bool IsValidCallTime(RPC.RPCMethodAttribute desc, Int64 recvTime, string funName = "Unknown")
        //{
        //    return true;
        //}
        public virtual bool IsValidCallTime(System.Reflection.MethodInfo method,RPC.RPCMethodAttribute desc, Int64 recvTime, string funName = "Unknown")
        {
            var now = CSUtility.DllImportAPI.vfxGetTickCount();
            if (now - recvTime > 50)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("RPCCallTime {0} Lag By Tick{1}]", funName, now - recvTime));
            }
            if (desc.MinCallInterval < 0)
            {
                return true;
            }
            Int64 prevTime = 0;
            if (mCallTimes.TryGetValue(method, out prevTime) == false)
            {
                mCallTimes[method] = recvTime;
                return true;
            }
            mCallTimes[method] = recvTime;
            if (recvTime - prevTime < desc.MinCallInterval)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("RPCCallTime {0} Is Valid[{1}:{2}]", funName, recvTime - prevTime, now - recvTime));
                return false;
            }
            return true;
        }
	};
}
