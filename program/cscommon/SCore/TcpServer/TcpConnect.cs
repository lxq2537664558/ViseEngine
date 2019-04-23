
using System;
using System.Net;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SCore.TcpServer
{
    //public class TcpConnectImpl : TcpConnect
    //{
    //    protected TcpServerImpl Server;

    //    private SocketAsyncEventArgs mIOReceiveEventArgs;
    //    private SocketAsyncEventArgs mIOSendEventArgs;
    //    private ConcurrentQueue<byte[]> mSendQueue;
    //    private int isInSending;
    //    public bool IsSending
    //    {
    //        get
    //        {
    //            return isInSending == 1;
    //        }
    //    }

    //    #region 对外接口
    //    public bool IsValidTcpConnect()
    //    {
    //        return WorkSocket != null;
    //    }

    //    public override int Port
    //    {
    //        get { return RemoteEndPoint.Port; }
    //    }

    //    public override string IpAddress
    //    {
    //        get { return RemoteEndPoint.Address.ToString(); }
    //    }

    //    public override void SendBuffer(byte[] data, int offset, int count)
    //    {
    //        if (State == NetState.Connect )
    //            Server.PostSend(this, data, offset, count);
    //    }

    //    public override void Disconnect()
    //    {
    //        if (Server == null)
    //        {//说明已经关过了
    //            Log.FileLog.WriteLine("Disconnect if (Server == null)");
    //            return;
    //        }

    //        Server.CloseSocket(this);
    //        //this.server.Raise_CloseConnect(this);
    //    }

    //    #endregion

    //    byte[] mRecvBuffer = null;
    //    byte[] mSendBuffer = null;
    //    public TcpConnectImpl(TcpServerImpl server, Socket acceptSocket, int bufferSize)
    //    {
    //        HashCode = Guid.NewGuid();
    //        Server = server;

    //        mRecvBuffer = new byte[bufferSize];
    //        mIOReceiveEventArgs = new SocketAsyncEventArgs();
    //        mIOReceiveEventArgs.AcceptSocket = acceptSocket;
    //        mIOReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(server.IO_Completed);
    //        mIOReceiveEventArgs.SetBuffer(mRecvBuffer, 0, bufferSize);
    //        AsyncUserToken dataToken = new AsyncUserToken();
    //        dataToken.bufferOffset = 0;
    //        dataToken.tcpConn = this;
    //        mIOReceiveEventArgs.UserToken = dataToken;            

    //        mSendBuffer = new byte[bufferSize];
    //        mIOSendEventArgs = new SocketAsyncEventArgs();
    //        mIOSendEventArgs.AcceptSocket = acceptSocket;
    //        mIOSendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(server.IO_Completed);
    //        mIOSendEventArgs.SetBuffer(mSendBuffer, 0, bufferSize);
    //        dataToken = new AsyncUserToken();
    //        dataToken.bufferOffset = 0;
    //        dataToken.tcpConn = this;
    //        mIOSendEventArgs.UserToken = dataToken;

    //        mSendQueue = new ConcurrentQueue<byte[]>();

    //        mState = NetState.Invalid;
    //    }

    //    Socket mAcceptSocket = null;
    //    public TcpConnectImpl(TcpServerImpl server, Socket socket)
    //    {
    //        HashCode = Guid.NewGuid();
    //        Server = server;
    //        mAcceptSocket = socket;
    //    }

    //    public void Init()
    //    {
    //        Server.GetIOArgPool().TryDequeue(out mIOReceiveEventArgs);
    //        mIOReceiveEventArgs.AcceptSocket = mAcceptSocket;
    //        var dataToken1 = (AsyncUserToken)mIOReceiveEventArgs.UserToken;
    //        mIOReceiveEventArgs.SetBuffer(dataToken1.bufferOffset, Server.socketSettings.BufferSize);
    //        dataToken1.tcpConn = this;

    //        Server.GetIOArgPool().TryDequeue(out mIOSendEventArgs);
    //        mIOSendEventArgs.AcceptSocket = mAcceptSocket;
    //        var dataToken2 = (AsyncUserToken)mIOSendEventArgs.UserToken;
    //        mIOSendEventArgs.SetBuffer(dataToken2.bufferOffset, Server.socketSettings.BufferSize);
    //        dataToken2.tcpConn = this;

    //        mSendQueue = new ConcurrentQueue<byte[]>();

    //        mLimitLevel = Server.LimitLevel;
    //        State = NetState.Connect;
    //    }

    //    ~TcpConnectImpl()
    //    {
    //        Close();
    //    }

    //    public Guid HashCode { get; private set; }

    //    public SocketAsyncEventArgs ReceiveEventArgs { get { return mIOReceiveEventArgs; } }
    //    public SocketAsyncEventArgs SendEventArgs { get { return mIOSendEventArgs; } }

    //    public bool Connected 
    //    { 
    //        get 
    //        {
    //            if (mAcceptSocket != null)
    //                return mAcceptSocket.Connected;
    //            return false;
    //        } 
    //    }

    //    internal Socket WorkSocket 
    //    { 
    //        get 
    //        {
    //            if (mAcceptSocket == null)
    //                return null;
    //            return mAcceptSocket; 
    //        } 
    //    }

    //    public IPEndPoint RemoteEndPoint 
    //    {
    //        get 
    //        {
    //            if (mAcceptSocket == null)
    //                return null;
    //            return (IPEndPoint)mAcceptSocket.RemoteEndPoint;
    //        } 
    //    }

    //    public int QueueLength 
    //    {
    //        get { return mSendQueue.Count; } 
    //    }

    //    internal void ClearSendQueue()
    //    {
    //        byte[] temp;
    //        while (mSendQueue.TryDequeue(out temp))
    //        {

    //        }
    //    }

    //    internal void Close()
    //    {
    //        mState = NetState.Disconnect;

    //        if (mIOSendEventArgs == null || mIOSendEventArgs == null)
    //            return;

    //        var dataToken0 = (AsyncUserToken)mIOSendEventArgs.UserToken;
    //        if (dataToken0 != null)
    //        {
    //            dataToken0.Reset(true);
    //            dataToken0.tcpConn = null;
    //        }
    //        var dataToken1 = (AsyncUserToken)mIOReceiveEventArgs.UserToken;
    //        if (dataToken1 != null)
    //        {
    //            dataToken1.Reset(true);
    //            dataToken1.tcpConn = null;
    //        }

    //        if (mSendBuffer == null)
    //            Server.GetIOArgPool().Enqueue(mIOSendEventArgs);
    //        mSendBuffer = null;

    //        if (mRecvBuffer == null)
    //            Server.GetIOArgPool().Enqueue(mIOReceiveEventArgs);
    //        mRecvBuffer = null;

    //        mAcceptSocket = null;

    //        mIOSendEventArgs.AcceptSocket = null;
    //        mIOReceiveEventArgs.AcceptSocket = null;
    //        mIOSendEventArgs = null;
    //        mIOReceiveEventArgs = null;
    //        Server = null;
    //    }

    //    public int MaxSendQueueSize = 1024 * 1024;
    //    internal void Enqueue(byte[] data)
    //    {
    //        if (mSendQueue.Count >= MaxSendQueueSize)
    //        {
    //            Log.FileLog.WriteLine("TcpConnect SendQueue Count = {0}", mSendQueue.Count);
    //            KickOffWeakPackage();
    //            if (mSendQueue.Count >= MaxSendQueueSize)
    //            {
    //                Log.FileLog.WriteLine("Force Clear TcpConnect SendQueue Count = {0}", mSendQueue.Count);
    //                byte[] temp;
    //                while (mSendQueue.TryDequeue(out temp))
    //                {

    //                }
    //            }
    //        }
    //        mSendQueue.Enqueue(data);
    //    }
    //    internal bool TryDequeue(out byte[] result)
    //    {
    //        return mSendQueue.TryDequeue(out result);
    //    }
    //    internal bool TrySetSendFlag()
    //    {
    //        return Interlocked.CompareExchange(ref isInSending, 1, 0) == 0;
    //    }
    //    internal void ResetSendFlag()
    //    {
    //        Interlocked.Exchange(ref isInSending, 0);
    //    }

    //    public unsafe void KickOffWeakPackage()//(List<RPC.RPCHeader> weaks)
    //    {
    //        lock (mSendQueue)
    //        {
    //            var strongs = new Queue<byte[]>();
    //            byte[] pkg;
    //            while (mSendQueue.TryDequeue(out pkg))
    //            {
    //                fixed(byte* header = &pkg[0])
    //                {
    //                    var rpcHeader = (RPC.RPCHeader*)header;
    //                    if (rpcHeader->IsWeakPkg())
    //                        continue;
    //                    //foreach (var i in weaks)
    //                    //{
    //                    //    if (i.IsSameMethod(rpcHeader))
    //                    //        continue;
    //                    //}
    //                }

    //                strongs.Enqueue(pkg);
    //            }

    //            foreach (var i in strongs)
    //            {
    //                mSendQueue.Enqueue(i);
    //            }
    //            //mSendQueue._VeryDangerousReplace(strongs);
    //        }
    //    }
    //}

    public class TcpConnectHP : TcpConnect
    {
        TcpServerHP mServer;
        IntPtr mConnectId;
        public IntPtr ConnectId
        {
            get { return mConnectId; }
        }
        IntPtr mClientPtr;

        IntPtr mThitPtr;

        IPEndPoint mRemoteEndPoint;

        public string NickName = "";

        class RecvData
        {
            public byte[] PkgData;
            public Int64 RecvTime;
        }
        ConcurrentQueue<RecvData> mRecvQueue = new ConcurrentQueue<RecvData>();

        internal static TcpConnectHP GetTcpConnectHPFromConnId(TcpServerHP server, IntPtr connId)
        {
            IntPtr pinArg = IntPtr.Zero;
            server.HPServer.GetConnectionExtra(connId, ref pinArg);
            if (pinArg == IntPtr.Zero)
                return null;
            //不需要这样了，因为队列后串行了
            //while (pinArg == IntPtr.Zero)
            //{
            //    System.Threading.Thread.Sleep(0);
            //    server.HPServer.GetConnectionExtra(connId, ref pinArg);
            //}
            GCHandle handle = (GCHandle)Marshal.PtrToStructure(pinArg, typeof(GCHandle));

            return handle.Target as TcpConnectHP;
        }

        public TcpConnectHP(TcpServerHP server, IntPtr connId, IntPtr pClient)
        {
            mServer = server;
            mConnectId = connId;
            mClientPtr = pClient;

            Init();
        }

        public void Init()
        {
            if (mThitPtr != IntPtr.Zero)
            {
                GCHandle handle = (GCHandle)Marshal.PtrToStructure(mThitPtr, typeof(GCHandle));
                handle.Target = null;
                handle.Free();
                mThitPtr = IntPtr.Zero;
            }

            mServer.HPServer.SetConnectionExtra(mConnectId, GCHandle.Alloc(this));
            mServer.HPServer.GetConnectionExtra(mConnectId, ref mThitPtr);

            NickName = mConnectId.ToString();
            string ip = "";
            UInt16 port = 0;
            mServer.HPServer.GetRemoteAddress(mConnectId, ref ip, ref port);
            IPAddress ipAdd;
            IPAddress.TryParse(ip, out ipAdd);
            mRemoteEndPoint = new IPEndPoint(ipAdd, port);

            this.mLimitLevel = mServer.LimitLevel;
            State = CSUtility.Net.NetState.Connect;
        }

        public void Update()
        {
            if (mServer == null)
                return;
            //处理所有接受包
            int nPacket = mRecvQueue.Count;
            bool bIgoreWeakPkg = false;
            if (nPacket > mServer.RecvPacketNumLimitter)
            {
                Log.FileLog.WriteLine("TcpConnect[{0}]接受太多需要处理的包，强行踢掉弱包", this.NickName);
                bIgoreWeakPkg = true;
            }

            int WeakPkgNum = 0;
            while (mRecvQueue.Count > 0 && nPacket > 0)
            {
                RecvData rcv;
                if (mRecvQueue.TryDequeue(out rcv))
                {
                    nPacket--;
                    if (bIgoreWeakPkg)
                    {
                        unsafe
                        {
                            fixed (byte* pPkg = &rcv.PkgData[0])
                            {
                                if (((RPC.RPCHeader*)pPkg)->IsWeakPkg())
                                {
                                    WeakPkgNum++;
                                    continue;
                                }
                            }
                        }
                    }
                    mServer.DoReceiveData(this, rcv.PkgData, rcv.PkgData.Length, rcv.RecvTime);
                }
            }

            if (bIgoreWeakPkg)
            {
                Log.FileLog.WriteLine("TcpServer[{0}]强行踢掉弱包 = {1}", this.NickName, WeakPkgNum);
            }
        }

        public override int Port
        {
            get 
            {
                if (RemoteEndPoint == null)
                    return 0;
                return RemoteEndPoint.Port; 
            }
        }

        public override string IpAddress
        {
            get 
            {
                if (RemoteEndPoint == null)
                    return "0.0.0.0";
                return RemoteEndPoint.Address.ToString(); 
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get{ return mRemoteEndPoint; }
        }

        public int GetPendingDataLength()
        {
            if (mServer == null)
                return 0;
            int pendingLength = 0;
            mServer.HPServer.GetPendingDataLength(mConnectId, ref pendingLength);
            return pendingLength;
        }

        public uint GetConnectPeriod()
        {
            if (mServer == null)
                return 0;
            uint period = 0;
            mServer.HPServer.GetConnectPeriod(mConnectId, ref period);
            return period;
        }

        public uint GetSilencePeriod()
        {
            if (mServer == null)
                return 0;
            uint period = 0;
            mServer.HPServer.GetSilencePeriod(mConnectId, ref period);
            return period;
        }

        public override void SendBuffer(byte[] data, int offset, int count)
        {
            if (State != CSUtility.Net.NetState.Connect)
            {
                return;
            }

            unsafe
            {
                fixed (byte* pData = &data[0])
                {
                    RPC.RPCHeader* pHeader = (RPC.RPCHeader*)pData;
                    pHeader->PackageSize = (ushort)count;
                }
            }
            if (GetPendingDataLength() > mServer.SendPacketSizeLimitter)
            {
                unsafe
                {
                    fixed (byte* pData = &data[0])
                    {
                        RPC.RPCHeader* pHeader = (RPC.RPCHeader*)pData;
                        if (pHeader->IsWeakPkg())
                            return;
                    }
                }
            }
            mServer.HPServer.Send( mConnectId, data, offset, count);
        }

        public HPSocketCS.HandleResult OnSendPackage(IntPtr pData, int length)
        {
            //HPSocketCS.HandleResult.Ignore 如果非常忙，可以在这里把weakpkg踢掉
            return HPSocketCS.HandleResult.Ok;//没啥用
        }

        public override void Disconnect()
        {
            if (State == CSUtility.Net.NetState.Disconnect)
                return;

            State = CSUtility.Net.NetState.Disconnect;
            if (mServer!=null)
                mServer.HPServer.Disconnect(mConnectId);
        }

        internal void ReleaseExtra()
        {
            if (mServer != null)
            {
                State = CSUtility.Net.NetState.Disconnect;
                mPkgBuilder.ResetPacket();

                if (mThitPtr != IntPtr.Zero)
                {
                    GCHandle handle = (GCHandle)Marshal.PtrToStructure(mThitPtr, typeof(GCHandle));
                    handle.Target = null;
                    handle.Free();
                    mThitPtr = IntPtr.Zero;
                }
                mServer.HPServer.SetConnectionExtra(mConnectId, null);
                mServer = null;
            }
        }

        CSUtility.Net.PacketBuilder mPkgBuilder = new CSUtility.Net.PacketBuilder();

        private void OnPacketOK(byte[] pkg, object parameter, Int64 recvTime)
        {
            lock (mRecvQueue)
            {
                RecvData rcv = new RecvData();
                rcv.PkgData = pkg;
                rcv.RecvTime = recvTime;
                mRecvQueue.Enqueue(rcv);
            }
        }

        public void OnReceiveEventHandler(IntPtr pData, int length)
        {
            unsafe
            {
                var result = mPkgBuilder.ParsePackage((byte*)pData.ToPointer(), (UInt32)length, this.OnPacketOK, null);
                if (result == false)
                {
                    Log.FileLog.WriteLine(string.Format("TcpConnect {0} ParsePackage Failed", mRemoteEndPoint.ToString()));
                }
            }
        }
    }
}