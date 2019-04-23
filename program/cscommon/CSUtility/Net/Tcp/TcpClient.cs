
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
//using System.Collections.Concurrent;

namespace CSUtility.Net
{
    /// <summary>
    /// 线程安全队列
    /// </summary>
    /// <typeparam name="T">模板类型</typeparam>
    public class ThreadSafeQueue<T>
    {
        Queue<T> mQueueData = new Queue<T>();

        public void _VeryDangerousReplace(Queue<T> data)
        {
            mQueueData = data;
        }

        public int Count
        {
            get 
            { 
                lock(this)
                    return mQueueData.Count; 
            }
        }
        public bool IsEmpty
        {
            get 
            {
                lock (this) 
                    return mQueueData.Count == 0;
            } 
        }

        public void Clear()
        {
            lock (this)
            {
                mQueueData.Clear();
            }
        }

        public T Dequeue()
        {
            lock (this)
            {
                return mQueueData.Dequeue();
            }
        }

        public void Enqueue(T item)
        {
            lock (this)
            {
                mQueueData.Enqueue(item);
            }
        }

        public bool TryDequeue(out T result)
        {
            lock (this)
            {
                if (mQueueData.Count == 0)
                {
                    result = default(T);
                    return false;
                }
                result = mQueueData.Dequeue();
                return true;
            }
        }
    }

    /// <summary>
    /// 线程安全堆栈
    /// </summary>
    /// <typeparam name="T">模板类型</typeparam>
    public class ThreadSafeStack<T>
    {
        Stack<T> mStackData = new Stack<T>();

        public int Count 
        {
            get
            {
                lock (this)
                    return mStackData.Count;
            }
        }

        public void Push(T item)
        {
            lock (this)
            {
                mStackData.Push(item);
            }
        }

        public bool TryPop(out T result)
        {
            lock (this)
            {
                if (mStackData.Count == 0)
                {
                    result = default(T);
                    return false;
                }
                result = mStackData.Pop();
                return true;
            }
        }
    }

    public delegate void NETCLIENT_EVENT(TcpClient pClient, byte[] pData, int nLength);
    public delegate void NETCLIENT_RECV_EVENT(TcpClient pClient, byte[] pData, int nLength, Int64 recvTime);
    
    public class TcpClient : NetConnection
    {
        public virtual event NETCLIENT_EVENT NewConnect;
        public virtual event NETCLIENT_EVENT CloseConnect;
        public virtual event NETCLIENT_RECV_EVENT ReceiveData;

        System.Net.Sockets.Socket mSocket = null;

        protected class RecvData
        {
            public byte[] PkgData;
            public Int64 RecvTime;
        }
        protected Queue<RecvData> mRecvQueue = new Queue<RecvData>();

        public string HostIp;
        public UInt16 Port;
        System.Threading.Thread mRecvThread = null;
        bool mRun = false;

        public virtual void Connect(string strHostIp, UInt16 nPort)
        {
            Close();

            HostIp = strHostIp;
            Port = nPort;
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(HostIp), Port);

            try
            {
                mSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mSocket.Connect(ipe);
            }
            catch (Exception e)
            {
                CSUtility.Program.LogInfo("Connect e " + e.ToString());
                //400 620 0550
                //P98QC-7X8D4-T2XXJ-YBHBV-V47TY
                if (mSocket == null)
                {
                    mState = CSUtility.Net.NetState.Disconnect;
                    mState = CSUtility.Net.NetState.Disconnect;
                    try
                    {
                        if (NewConnect != null)
                            NewConnect(this, null, 0);
                    }
                    catch (System.Exception ex)
                    {
                        Log.FileLog.WriteLine(ex.ToString());
                        Log.FileLog.WriteLine(ex.StackTrace.ToString());
                    }
                    return;
                }   
            }

            if (mSocket.Connected == false)
            {
                mState = CSUtility.Net.NetState.Disconnect;
                try
                {
                    if (NewConnect != null)
                        NewConnect(this, null, 0);
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }
            else
            {
                mState = CSUtility.Net.NetState.Connect;

                mRun = true;
                mRecvThread = new System.Threading.Thread(this.RecvPackageProc);
                mRecvThread.Start();

                try
                {
                    if (NewConnect != null)
                        NewConnect(this, null, 1);
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }
        }

        void RecvPackageProc()
        {
            try
            {
                byte[] recvBuffer = new byte[UInt16.MaxValue];
                while (mRun)
                {
                    var recvBytes = mSocket.Receive(recvBuffer, recvBuffer.Length, 0);
                    if (recvBytes > 0)
                    {
                        unsafe
                        {
                            fixed (byte* ptr = &recvBuffer[0])
                            {
                                OnReceiveEventHandler((IntPtr)ptr, recvBytes);
                            }
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(0);
                    }
                }
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            finally
            {
                mRun = false;
                mPkgBuilder.ResetPacket();
            }
        }

        public int RecvPacketNumLimitter = 1024;
        public virtual void Update()
        {
            try
            {
                int nPacket = mRecvQueue.Count;
                bool bIgoreWeakPkg = false;
                if (nPacket > RecvPacketNumLimitter)
                {
                    Log.FileLog.WriteLine("TcpClient接受太多需要处理的包，强行踢掉弱包");
                    bIgoreWeakPkg = true;
                }

                int WeakPkgNum = 0;
                if (ReceiveData != null)
                {
                    RecvData vBytes = null;
                    while (mRecvQueue.Count > 0 && nPacket>0)
                    {
                        lock (mRecvQueue)
                        {
                            if (mRecvQueue.Count == 0)
                                continue;
                            vBytes = mRecvQueue.Dequeue();
                            nPacket--;
                            if (bIgoreWeakPkg)
                            {
                                unsafe
                                {
                                    fixed (byte* pPkg = &vBytes.PkgData[0])
                                    {
                                        if (((RPC.RPCHeader*)pPkg)->IsWeakPkg())
                                        {
                                            WeakPkgNum++;
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        ReceiveData(this, vBytes.PkgData, vBytes.PkgData.Length, vBytes.RecvTime);
                    }
                }

                if (bIgoreWeakPkg)
                {
                    Log.FileLog.WriteLine("TcpClient强行踢掉弱包 = {0}", WeakPkgNum);
                }
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }

            if(mRun==false && mState == CSUtility.Net.NetState.Connect)
            {
                mState = CSUtility.Net.NetState.Disconnect;
                if (CloseConnect != null)
                {
                    CloseConnect(this, null, 0);
                }
            }
        }

        public int SendPacketSizeLimitter = 1024 * 1024;//超过0.5M的发送缓冲，需要忽略若包的发送请求
        public int TotalSendSize = 0;
        public override void SendBuffer(byte[] data, int offset, int count)
        {
            unsafe
            {
                fixed (byte* pData = &data[0])
                {
                    RPC.RPCHeader* pHeader = (RPC.RPCHeader*)pData;
                    pHeader->PackageSize = (ushort)count;
                }
            }

            int pendingLen = 0;
            if (pendingLen > SendPacketSizeLimitter)
            {
                Log.FileLog.WriteLine("PendingDataLengthLimit : {0}", pendingLen);
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
            
            TotalSendSize += count;

            if(offset>0)
            {
                //data[offset]
            }

            mSocket?.Send(data, count, 0);
        }

        public virtual void Close()
        {
            mRun = false;
            mState = CSUtility.Net.NetState.Disconnect;

            try
            {
                if (mSocket != null && mSocket.Connected)
                    mSocket.Disconnect(true);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public virtual void Reconnect()
        {
            Connect(HostIp, Port);
        }

        public virtual bool Connected
        {
            get
            {
                if (mSocket == null)
                    return false;
                return mSocket.Connected;
            }
        }

        CSUtility.Net.NetState mState = CSUtility.Net.NetState.Invalid;
        public virtual CSUtility.Net.NetState State
        {
            get { return mState; }
        }   

        #region 回调转换
        
        private void OnPacketOK(byte[] pkg, object parameter, Int64 recvTime)
        {
            lock (mRecvQueue)
            {
                var rcvData = new RecvData();
                rcvData.PkgData = pkg;
                rcvData.RecvTime = recvTime;
                mRecvQueue.Enqueue(rcvData);
            }
        }
        protected PacketBuilder mPkgBuilder = new PacketBuilder();
        private void OnReceiveEventHandler(IntPtr pData, int length)
        {
            unsafe
            {
                bool result = mPkgBuilder.ParsePackage((byte*)pData.ToPointer(), (UInt32)length, this.OnPacketOK, this);
                if (result==false)
                {
                    Log.FileLog.WriteLine("TcpClient [{0}:{1}] ParsePackage Failed", HostIp, Port);
                }
            }
        }
        #endregion
    }    
}