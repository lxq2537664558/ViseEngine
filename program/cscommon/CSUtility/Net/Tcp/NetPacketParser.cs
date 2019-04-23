using System;
using System.Net.Sockets;

namespace CSUtility.Net
{
    public class NetPacketParser
    {
        static public int PREFIX_SIZE = 2;

        static public int HandlePrefix(SocketAsyncEventArgs saea, AsyncUserToken dataToken, int remainingBytesToProcess)
        {
            if (saea.AcceptSocket.Connected == false)
            {
                Log.FileLog.WriteLine("HandlePrefix saea.AcceptSocket.Connected == false");
                return 0;
            }

            if (remainingBytesToProcess >= PREFIX_SIZE - dataToken.prefixBytesDone)
            {
                for (int i = 0; i < PREFIX_SIZE - dataToken.prefixBytesDone; i++)
                {
                    dataToken.prefixBytes[dataToken.prefixBytesDone + i] = saea.Buffer[dataToken.DataOffset + i];
                }
                remainingBytesToProcess = remainingBytesToProcess - PREFIX_SIZE + dataToken.prefixBytesDone;
                dataToken.bufferSkip += PREFIX_SIZE - dataToken.prefixBytesDone;
                dataToken.prefixBytesDone = PREFIX_SIZE;
                dataToken.messageLength = BitConverter.ToUInt16(dataToken.prefixBytes, 0);
            }
            else
            {
                for (int i = 0; i < remainingBytesToProcess; i++)
                {
                    dataToken.prefixBytes[dataToken.prefixBytesDone + i] = saea.Buffer[dataToken.DataOffset + i];
                }
                dataToken.prefixBytesDone += remainingBytesToProcess;
                remainingBytesToProcess = 0;
            }

            return remainingBytesToProcess;
        }

        static public int HandleMessage(SocketAsyncEventArgs saea, AsyncUserToken dataToken, int remainingBytesToProcess)
        {
            if (saea.AcceptSocket.Connected == false)
            {
                Log.FileLog.WriteLine("HandleMessage saea.AcceptSocket.Connected == false");
                return 0;
            }
            if (dataToken.messageBytesDone == 0)
            {
                if (dataToken.messageLength > 65535)
                {
                    Log.FileLog.WriteLine("HandleMessage dataToken.messageLength = {0}", dataToken.messageLength);
                    return 0;
                }
                dataToken.messageBytes = new byte[dataToken.messageLength];
            }

            var nonCopiedBytes = 0;
            if (remainingBytesToProcess + dataToken.messageBytesDone >= dataToken.messageLength)
            {
                var copyedBytes = dataToken.RemainByte;
                nonCopiedBytes = remainingBytesToProcess - copyedBytes;
                Buffer.BlockCopy(saea.Buffer, dataToken.DataOffset, dataToken.messageBytes, dataToken.messageBytesDone, copyedBytes);
                dataToken.messageBytesDone = dataToken.messageLength;
                dataToken.bufferSkip += copyedBytes;
            }
            else
            {
                Buffer.BlockCopy(saea.Buffer, dataToken.DataOffset, dataToken.messageBytes, dataToken.messageBytesDone, remainingBytesToProcess);
                dataToken.messageBytesDone += remainingBytesToProcess;
            }

            return nonCopiedBytes;
        }
    }

    public class PacketBuilder
    {
        byte[] mDataBuffer = new byte[UInt16.MaxValue+RPC.RPCHeader.SizeOf()];
        public byte[] DataBuffer
        {
            get { return mDataBuffer; }
        }

        UInt32 PacketSize = 0;
        UInt32 CurPos = 0;

        private unsafe byte* AppendData(byte* pSrc, UInt32 count, ref UInt32 length)
        {
            if (count == 0)
                return pSrc;

            if (length < count)
            {
                Log.FileLog.WriteLine("夭寿啦！");
                length = 0;
                return pSrc;
            }

            unsafe
            {
                fixed (byte* pTar = &DataBuffer[CurPos])
                {
                    for(int i=0;i<count;i++)
                    {
                        pTar[i] = pSrc[i];
                    }
                }
                CurPos += (uint)count;
                length -= count;
                
                return pSrc + count;
            }
        }

        public delegate void FOnPacketOK(byte[] pkg, object parameter, Int64 recvTime);
        const UInt32 PacketHeadSize = sizeof(ushort);
        public unsafe bool ParsePackage(byte* pData, UInt32 length, FOnPacketOK onPacket, object parameter)
        {
            lock (this)
            {
                if (length == 0)
                    return true;
                unsafe
                {
                    //包头还没有读出来，不知道长度
                    if (PacketSize == 0)
                    {
                        if (CurPos + length < PacketHeadSize)
                        {//加上新读取的都还没有完成包头
                            AppendData(pData, length, ref length);
                            return true;
                        }
                        else
                        {
                            var remainHead = PacketHeadSize - CurPos;
                            pData = AppendData(pData, remainHead, ref length);
                            fixed (byte* pBuffer = &mDataBuffer[0])
                            {
                                PacketSize = ((RPC.RPCHeader*)pBuffer)->PackageSize;
                            }
                        }
                    }

                    if (PacketSize == 0 || PacketSize >= UInt16.MaxValue)
                    {//被修改或者错误的包
                        return false;
                    }

                    if (CurPos + length < PacketSize)
                    {//还不够
                        AppendData(pData, length, ref length);
                        return true;
                    }
                    else
                    {
                        pData = AppendData(pData, PacketSize - CurPos, ref length);
                        var pkg = CreateFullPacket();
                        var time = CSUtility.DllImportAPI.vfxGetTickCount();
                        onPacket(pkg, parameter, time);
                        return ParsePackage(pData, length, onPacket, parameter);
                    }
                }
            }
        }

        private byte[] CreateFullPacket()
        {
            byte[] result = new byte[PacketSize];
            Buffer.BlockCopy(mDataBuffer, 0, result, 0, (int)PacketSize);
            CurPos = 0;
            PacketSize = 0;
            return result;
        }

        public void ResetPacket()
        {
            CurPos = 0;
            PacketSize = 0;
        }
    }
}
