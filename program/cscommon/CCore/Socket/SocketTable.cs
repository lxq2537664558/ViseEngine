using System;
using System.Collections.Generic;
/// <summary>
/// 挂接件的命名空间
/// </summary>
namespace CCore.Socket
{
    /// <summary>
    /// 挂接组
    /// </summary>
    public class SocketTable
    {      
        /// <summary>
        /// 挂接组的对象指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 挂接组的对象指针
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                if (value == IntPtr.Zero)
                    return;

                DllImportAPI.V3DSocketTable_AddRef(value);
                Cleanup();
                DllImportAPI.V3DSocketTable_Release(mInner);
                mInner = value;

                var count = DllImportAPI.V3DSocketTable_GetSocketCount(mInner);
                for (int i = 0; i < count; ++i)
                {
                    Socket result = Socket.CreateSocket(this, DllImportAPI.V3DSocketTable_GetSocket(mInner, i));
				    mSockets.Add(result);
                }
            }
        }

        List<Socket> mSockets = new List<Socket>();
        /// <summary>
        /// 构造函数，创建实例对象
        /// </summary>
        public SocketTable()
        {
            mInner = DllImportAPI.V3DSocketTable_New();
        }
        /// <summary>
        /// 析构函数，删除面板中所有的挂接件成员对象，并释放该对象的指针
        /// </summary>
        ~SocketTable()
        {
            Cleanup();
            DllImportAPI.V3DSocketTable_Release(mInner);
            mInner = IntPtr.Zero;
        }
        /// <summary>
        /// 清空挂接面板中的所有的挂接件
        /// </summary>
		protected void Cleanup()
        {
            mSockets.Clear();
        }
        /// <summary>
        /// 添加挂接成员
        /// </summary>
        /// <returns>返回添加的挂接成员</returns>
        public Socket AddSocket()
        {
            var socketInner = DllImportAPI.V3DSocket_New();
            return AddSocket(socketInner);
        }
        /// <summary>
        /// 添加指定的挂接成员
        /// </summary>
        /// <param name="sInner">挂接成员对象</param>
        /// <returns>返回添加的挂接成员</returns>
        public Socket AddSocket(IntPtr sInner)
        {
            DllImportAPI.V3DSocketTable_AddSocket(mInner, sInner);
            var socket = Socket.CreateSocket(this, sInner);
            mSockets.Add(socket);
            return socket;            
        }
        /// <summary>
        /// 获取指定的挂接成员对象
        /// </summary>
        /// <param name="i">挂接成员的索引</param>
        /// <returns>返回相应的挂接成员</returns>
        public Socket GetSocket(int i)
        {
            return mSockets[i];
        }
        /// <summary>
        /// 获取指定的挂接成员对象
        /// </summary>
        /// <param name="name">挂接成员的名称</param>
        /// <returns>返回相应的挂接成员对象</returns>
        public Socket GetSocket(string name)
        {
            return mSockets.Find((Socket s)=>
                {
                    return s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase);
                });
            //foreach (var s in mSockets)
            //{
            //    if (s.Name.ToLower() == name.ToLower())
            //        return s;
            //}
            //return null;
        }
        /// <summary>
        /// 删除指定的挂接成员对象
        /// </summary>
        /// <param name="index">挂接成员的索引</param>
        public void RemoveSocket(int index)
        {
            DllImportAPI.V3DSocketTable_RemoveSocket(mInner, index);
            mSockets.RemoveAt(index);
        }
        /// <summary>
        /// 获取挂接面板中成员的数量
        /// </summary>
        /// <returns>返回挂接面板中成员的数量</returns>
        public int GetSocketCount()
        {
            return mSockets.Count;
        }
        /// <summary>
        /// 合并两个挂接面板成员
        /// </summary>
        /// <param name="pSubTable">挂接面板对象</param>
        public void Merge(SocketTable pSubTable)
        {
            for (int i = 0; i < pSubTable.GetSocketCount(); ++i)
            {
                Socket pSubSocket = pSubTable.GetSocket(i);

                bool bFindSocket = false;
                for (int j = 0; j < GetSocketCount(); ++j)
                {
                    Socket pFullSocket = GetSocket(j);
                    if (pSubSocket.Name.ToLower() == pFullSocket.Name.ToLower())
                    {
                        bFindSocket = true;
                        break;
                    }
                }
                if (bFindSocket == false)
                {
                    var skt = DllImportAPI.V3DSocket_CloneSocket(pSubSocket.Inner);
                    var newSocket = AddSocket(skt);
                    DllImportAPI.V3DSocket_Release(skt);
                    newSocket.ParentIndexInFullSocketTable = mSockets.Count - 1;
                }
            }
        }

        //void Build(ISkeleton fullSkeleton)
        /// <summary>
        /// 创建挂接面板对象
        /// </summary>
        /// <param name="pFullSkeleton">骨骼对象指针</param>
        public void Build(IntPtr pFullSkeleton)
        {
            DllImportAPI.V3DSocketTable_Build(mInner, pFullSkeleton);
        }

        //void Update(ISkeleton fullSkeleton)
        /// <summary>
        /// 更新挂接面板
        /// </summary>
        /// <param name="pFullSkeleton">骨骼对象指针</param>
        public void Update(IntPtr pFullSkeleton)
        {
            DllImportAPI.V3DSocketTable_Update(mInner, pFullSkeleton);
        }
    }
}
