
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SCore.TcpServer
{
    public class TcpOption
    {
        public TcpOption(int recv, int send)
        {
            this.RecvPacketNumLimitter = recv;
            this.SendPacketSizeLimitter = send;
        }

        //每个TcpConnect接受队列包个数限制器
        public int RecvPacketNumLimitter { get; private set; }

        //每个TcpConnect发送队列缓冲大小限制器
        public int SendPacketSizeLimitter { get; private set; }

        public static TcpOption ForRegServer = new TcpOption(1024, 1024 * 32);
        public static TcpOption ForGateServer = new TcpOption(1024, 1024 * 32);
        public static TcpOption ForDataServer = new TcpOption(1024 * 512, 1024 * 32);
        public static TcpOption ForPlanesServer = new TcpOption(1024 * 512, 1024 * 32);
        public static TcpOption ForComServer = new TcpOption(1024 * 512, 1024 * 32);
        public static TcpOption ForPathServer = new TcpOption(1024 * 512, 1024 * 32);
    }

    public delegate void NETSERVER_RECV_EVENT(TcpConnect pConnect, TcpServer pServer, byte[] pData, int nLength, Int64 recvTime);

    public class TcpServerHP : TcpServer
    {
        public event NETSERVER_RECV_EVENT ReceiveData;
        public event NETSERVER_EVENT NewConnect;
        public event NETSERVER_EVENT CloseConnect;
        
        //每个连接接受未处理的包超过限制，就会踢掉弱包，以保证处理速度
        public int RecvPacketNumLimitter
        {
            get { return mTcpSettings.RecvPacketNumLimitter; }
        }

        public int SendPacketSizeLimitter
        {
            get { return mTcpSettings.SendPacketSizeLimitter; }
        }
        public int LimitLevel = (int)RPC.RPCExecuteLimitLevel.Developer;
        public TcpOption mTcpSettings;

        HPSocketCS.TcpServer mHPServer = new HPSocketCS.TcpServer();
        internal HPSocketCS.TcpServer HPServer
        {
            get { return mHPServer; }
        }
        Dictionary<IntPtr, TcpConnectHP> mClientConnects = new Dictionary<IntPtr, TcpConnectHP>();
        
        public TcpServerHP()
        {
            // 设置服务器事件
            mHPServer.OnPrepareListen += OnPrepareListenEventHandler;
            mHPServer.OnAccept += OnAcceptEventHandler;
            mHPServer.OnSend += OnSendEventHandler;
            mHPServer.OnReceive += OnReceiveEventHandler;
            mHPServer.OnClose += OnCloseEventHandler;
            mHPServer.OnError += OnErrorEventHandler;
            mHPServer.OnShutdown += OnShutdownEventHandler;
        }
        ~TcpServerHP()
        {
            mHPServer.Destroy();
        }
        public bool Open(TcpOption socketSettings, UInt16 nPort, string ipAddr="0.0.0.0")
        {
            mTcpSettings = socketSettings;
            //mHPServer.WorkerThreadCount;
            //mHPServer.AcceptSocketCount;
            //mHPServer.SocketBufferSize;
            //mHPServer.SocketListenQueue;
            //mHPServer.FreeSocketObjLockTime;
            //mHPServer.FreeSocketObjPool;
            //mHPServer.FreeBufferObjPool;
            //mHPServer.KeepAliveTime;
            //mHPServer.KeepAliveInterval;
            //mHPServer.MaxShutdownWaitTime;
            //mHPServer.MarkSilence;
            mHPServer.RecvPolicy = HPSocketCS.RecvPolicy.Serial;
            mHPServer.SendPolicy = HPSocketCS.SendPolicy.Safe;
            //mHPServer.IpAddress = "0.0.0.0";
            mHPServer.IpAddress = ipAddr;
            mHPServer.Port = nPort;
            return mHPServer.Start();
        }

        public void Close()
        {
            mHPServer.Stop();
        }

        ConcurrentQueue<TcpConnectHP> mAcceptConnects = new ConcurrentQueue<TcpConnectHP>();
        ConcurrentQueue<TcpConnectHP> mClosedConnects = new ConcurrentQueue<TcpConnectHP>();
        public void Update()
        {
            //处理新建连接
            while (mAcceptConnects.Count > 0)
            {
                TcpConnectHP connect;
                if (mAcceptConnects.TryDequeue(out connect))
                {
                    mClientConnects[connect.ConnectId] = connect;
                    try
                    {
                        if (NewConnect != null)
                            NewConnect(connect, this, null, 1);
                    }
                    catch (System.Exception ex)
                    {
                        Log.FileLog.WriteLine(ex.ToString());
                        Log.FileLog.WriteLine(ex.StackTrace.ToString());
                    }
                }
            }

            //这里以后可以考虑多线程并行处理，利用WaitMulti同步
            foreach (var i in mClientConnects)
            {
                i.Value.Update();
            }
                        
            //处理关闭连接
            while (mClosedConnects.Count > 0)
            {
                TcpConnectHP connect;
                if (mClosedConnects.TryDequeue(out connect))
                {
                    try
                    {
                        mClientConnects.Remove(connect.ConnectId);
                        connect.ReleaseExtra();
                        if (CloseConnect != null)
                            CloseConnect(connect, this, null, 0);
                    }
                    catch (System.Exception ex)
                    {
                        Log.FileLog.WriteLine(ex.ToString());
                        Log.FileLog.WriteLine(ex.StackTrace.ToString());
                    }
                }
            }
        }

        public void DoReceiveData(TcpConnectHP connect, byte[] data, int length, Int64 recvTime)
        {
            try
            {
                if (ReceiveData != null)
                    ReceiveData(connect, this, data, length, recvTime);
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
        }

        #region 回掉转换
        private HPSocketCS.HandleResult OnPrepareListenEventHandler(IntPtr soListen)
        {
            return HPSocketCS.HandleResult.Ok;
        }
        
        private HPSocketCS.HandleResult OnAcceptEventHandler(IntPtr connId, IntPtr pClient)
        {
            var connect = new TcpConnectHP(this, connId, pClient);

            mAcceptConnects.Enqueue(connect);
            
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult OnSendEventHandler(IntPtr connId, IntPtr pData, int length)
        {
            TcpConnectHP connect = TcpConnectHP.GetTcpConnectHPFromConnId(this, connId);
            if (connect == null)
            {
                int pendingLen = 0;
                mHPServer.GetPendingDataLength(connId, ref pendingLen);
                Log.FileLog.WriteLine("OnSendEventHandler[{0}=>{1}] GetConnectionExtra null", connId, pendingLen);
                return HPSocketCS.HandleResult.Ok;
            }
            return connect.OnSendPackage(pData, length);
        }

        private HPSocketCS.HandleResult OnReceiveEventHandler(IntPtr connId, IntPtr pData, int length)
        {
            TcpConnectHP connect = TcpConnectHP.GetTcpConnectHPFromConnId(this, connId);
            if (connect != null)
            {
                connect.OnReceiveEventHandler(pData, length);
            }
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult OnCloseEventHandler(IntPtr connId)
        {
            TcpConnectHP connect = TcpConnectHP.GetTcpConnectHPFromConnId(this, connId);
            if (connect == null)
            {
                Log.FileLog.WriteLine("OnCloseEventHandler GetTcpConnectHPFromConnId {connId} failed", connect);
                while (connect == null)
                {
                    System.Threading.Thread.Sleep(1);
                    connect = TcpConnectHP.GetTcpConnectHPFromConnId(this, connId);
                }
                Log.FileLog.WriteLine("OnCloseEventHandler GetTcpConnectHPFromConnId {connId} ok", connect);
            }
            mClosedConnects.Enqueue(connect);

            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult OnErrorEventHandler(IntPtr connId, HPSocketCS.SocketOperation enOperation, int errorCode)
        {
            TcpConnectHP connect = TcpConnectHP.GetTcpConnectHPFromConnId(this, connId);
            if (connect==null)
            {
                Log.FileLog.WriteLine("OnErrorEventHandler GetTcpConnectHPFromConnId {connId} failed", connect);
                while (connect==null)
                {
                    System.Threading.Thread.Sleep(1);
                    connect = TcpConnectHP.GetTcpConnectHPFromConnId(this, connId);
                }
                Log.FileLog.WriteLine("OnErrorEventHandler GetTcpConnectHPFromConnId {connId} ok", connect);
            }

            Log.FileLog.WriteLine("Socket[{0}] Op[{1}] Error[{2}]:{3}", connect.NickName, enOperation, errorCode, mHPServer.GetSocketErrorDesc((HPSocketCS.SocketError)errorCode));
            mClosedConnects.Enqueue(connect);
            
            return HPSocketCS.HandleResult.Ok;
        }

        private HPSocketCS.HandleResult OnShutdownEventHandler()
        {
            return HPSocketCS.HandleResult.Ok;
        }
        #endregion
    }
}