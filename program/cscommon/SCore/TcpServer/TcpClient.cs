using System;

namespace SCore.TcpServer
{
    public class TcpClient : CSUtility.Net.TcpClient
    {
        public override event CSUtility.Net.NETCLIENT_EVENT NewConnect;    
        public override event CSUtility.Net.NETCLIENT_EVENT CloseConnect;
        public override event CSUtility.Net.NETCLIENT_RECV_EVENT ReceiveData;

        HPSocketCS.TcpClient mClient = new HPSocketCS.TcpClient();

        public TcpClient()
        {
            mClient.OnPrepareConnect += this.OnPrepareConnectEventHandler;
            mClient.OnConnect += this.OnConnectEventHandler;
            mClient.OnSend += this.OnSendEventHandler;
            mClient.OnReceive += this.OnReceiveEventHandler;
            mClient.OnClose += this.OnCloseEventHandler;
            mClient.OnError += this.OnErrorEventHandler;
        }

        public override void Connect(string strHostIp, UInt16 nPort)
        {
            Close();

            HostIp = strHostIp;
            Port = nPort;

            //mClient.SocketBufferSize = 65535;

            if (mClient.Connetion(strHostIp, nPort, false) == false)
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
            }
        }

        //public int RecvPacketNumLimitter = 1024;
        public override void Update()
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
                    while (mRecvQueue.Count > 0 && nPacket > 0)
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
        }

//         public int SendPacketSizeLimitter = 1024 * 1024;//超过0.5M的发送缓冲，需要忽略若包的发送请求
//         public int TotalSendSize = 0;
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
            mClient.GetPendingDataLength(ref pendingLen);
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
            mClient.Send(data, offset, count);
        }

        public override void Close()
        {
            mState = CSUtility.Net.NetState.Disconnect;
            mClient.Stop();
        }

        public override void Reconnect()
        {
            Connect(HostIp, Port);
        }

        public override bool Connected
        {
            get { return mClient.IsStarted; }
        }

        CSUtility.Net.NetState mState = CSUtility.Net.NetState.Invalid;
        public override CSUtility.Net.NetState State
        {
            get { return mState; }
        }

        #region 回调转换
        private HPSocketCS.HandleResult OnPrepareConnectEventHandler(HPSocketCS.TcpClient sender, uint socket)
        {
            return HPSocketCS.HandleResult.Ok;
        }
        private HPSocketCS.HandleResult OnConnectEventHandler(HPSocketCS.TcpClient sender)
        {
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
            return HPSocketCS.HandleResult.Ok;
        }
        private HPSocketCS.HandleResult OnSendEventHandler(HPSocketCS.TcpClient sender, IntPtr pData, int length)
        {
            return HPSocketCS.HandleResult.Ok;
        }

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
        //CSUtility.Net.PacketBuilder mPkgBuilder = new CSUtility.Net.PacketBuilder();
        private HPSocketCS.HandleResult OnReceiveEventHandler(HPSocketCS.TcpClient sender, IntPtr pData, int length)
        {
            unsafe
            {
                bool result = mPkgBuilder.ParsePackage((byte*)pData.ToPointer(), (UInt32)length, this.OnPacketOK, sender);
                if (result == false)
                {
                    Log.FileLog.WriteLine("TcpClient [{0}:{1}] ParsePackage Failed", HostIp, Port);
                }
            }

            return HPSocketCS.HandleResult.Ok;
        }
        private HPSocketCS.HandleResult OnCloseEventHandler(HPSocketCS.TcpClient sender)
        {
            try
            {
                mState = CSUtility.Net.NetState.Disconnect;
                mPkgBuilder.ResetPacket();
                if (CloseConnect != null)
                    CloseConnect(this, null, 0);
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
            return HPSocketCS.HandleResult.Ok;
        }
        private HPSocketCS.HandleResult OnErrorEventHandler(HPSocketCS.TcpClient sender, HPSocketCS.SocketOperation enOperation, int errorCode)
        {
            try
            {
                mState = CSUtility.Net.NetState.Disconnect;
                mPkgBuilder.ResetPacket();
                if (CloseConnect != null)
                    CloseConnect(this, null, 0);
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
            return HPSocketCS.HandleResult.Ok;
        }
        #endregion
    }
}
