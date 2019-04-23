//using System.Collections.Concurrent;


namespace SCore.TcpServer
{
    public delegate void NETSERVER_EVENT(TcpConnect pConnect, TcpServer pServer, byte[] pData, int nLength);
    public abstract class TcpServer
    {
        
    }

    public abstract class TcpConnect : CSUtility.Net.NetConnection
    {
        //protected TcpServer server;
        protected CSUtility.Net.NetState mState = CSUtility.Net.NetState.Invalid;

        public CSUtility.Net.NetState State
        {
            get
            {
                return mState;
            }
            set
            {
                mState = value;
            }
        }

        public virtual int Port
        {
            get { return 0; }
        }
        public virtual string IpAddress
        {
            get { return ""; }
        }
    }
}