using System.Net.Sockets;
using System.Collections.Generic;

namespace SCore.TcpServer
{
    /// <summary>
    /// 为了减少内存碎片，使用一个完整的大块内存，其内部分块作为IOCP缓冲
    /// <remarks>
    /// 参考MSDN代码： http://msdn.microsoft.com/zh-cn/library/bb517542(v=vs.110).aspx
    /// </remarks>
    /// </summary>
    public class BufferManager
    {
        int capacity;
        byte[] bufferBlock;
        Stack<int> freeIndexPool;
        int currentIndex;
        int saeaSize;

        public BufferManager(int capacity, int saeaSize)
        {
            this.capacity = capacity;
            this.saeaSize = saeaSize;
            this.freeIndexPool = new Stack<int>();
        }

        internal void InitBuffer()
        {
            this.bufferBlock = new byte[capacity];
        }

        internal bool AllocBuffer(SocketAsyncEventArgs args)
        {
            lock (this)
            {
                if (this.freeIndexPool.Count > 0)
                {
                    args.SetBuffer(this.bufferBlock, this.freeIndexPool.Pop(), this.saeaSize);
                }
                else
                {
                    if ((capacity - this.saeaSize) < this.currentIndex)
                    {
                        return false;
                    }
                    args.SetBuffer(this.bufferBlock, this.currentIndex, this.saeaSize);
                    this.currentIndex += this.saeaSize;
                }
                return true;
            }
        }

        internal void FreeBuffer(SocketAsyncEventArgs args)
        {
            lock (this)
            {
                this.freeIndexPool.Push(args.Offset);
                args.SetBuffer(null, 0, 0);
            }
        }
    }
}