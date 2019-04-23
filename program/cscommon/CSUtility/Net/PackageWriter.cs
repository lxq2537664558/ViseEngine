using System;

using System.Runtime.InteropServices;

namespace RPC
{
    public enum CommandTargetType
	{
		DefaultType		= 0,
		Planes			,
	};

    public class PackageWriter
    {
        public RPCNetworkMgr RPCManager = null;

        public RPC.RPCCallerCounter CallerCounter;

        public int mPos;
        public int mBuffSize;
        public int mStack;

        byte[] mHandle;

        public int GetPosition()
        {
            return mPos;
        }

        public int DataPtr()
        {
            return RPCHeader.SizeOf();
        }

        public int CurPtr()
        {
            return RPCHeader.SizeOf() + mPos;
        }

        int C_MAXDATASIZE = 64 * 1024;
        public void FixSize(int growSize)
        {
            if (growSize >= C_MAXDATASIZE)
            {
                Log.FileLog.WriteLine("PackageWriter FixSIze = {0}", growSize);
                throw new Exception("OutOfMemory By PackageWriter FixSIze");
            }
            int nsize = mPos + growSize;
            if (nsize > mBuffSize)
            {
                nsize += mBuffSize / 2;
                byte[] nBuffer = new byte[nsize + RPCHeader.SizeOf()];
                Buffer.BlockCopy(mHandle, 0, nBuffer,0, mPos + RPCHeader.SizeOf());
                mHandle = nBuffer;
                mBuffSize = nsize;
            }
        }

        public PackageWriter()
        {
            mHandle = new byte[128 + RPCHeader.SizeOf()];
            unsafe
            {
                fixed (byte* ptr = &mHandle[0])
                {
                    RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                    header->ToDefault();
                }
            }

            mBuffSize = 128;
            mStack = mPos = 0;

            //mPointer = new BYTE[128 + sizeof(RPCHeader)];
            //GetHeader()->ToDefault();
            //mBuffSize = 128;
            //mStack = mPos = 0;
        }
        public PackageWriter(byte[] data,int size)
        {
            int realBufferSize = size + 128;
            mHandle = new byte[realBufferSize];
            mBuffSize = realBufferSize - RPCHeader.SizeOf();
            mPos = size - RPCHeader.SizeOf();
            Buffer.BlockCopy(data, 0, mHandle, 0, size);            
            mStack = 0;

            //int realBufferSize = size + 128;
            //mPointer = new BYTE[realBufferSize];
            //mBuffSize = realBufferSize - sizeof(RPCHeader);
            //mPos = size - sizeof(RPCHeader);
            //memcpy(mPointer, pData, size);
            //mStack = 0;
        }
        //~PackageWriter()
        //{
        //    //unsafe
        //    //{
        //    //    IDllImportAPI.Net_DeletePackageWriter(mHandle.ToPointer());
        //    //}
        //}

        public static int HeaderSize
        {
            get
            {
                return RPCHeader.SizeOf();
            }
        }

        public int Position
        {
            get
            {
                return mPos;
            }
        }

        public byte[] Ptr
        {
            get
            {
                return mHandle;
            }
        }

        public PackageType PkgType
        {
            get
            {
                unsafe
                {
                    fixed (byte* ptr = &mHandle[0])
                    {
                        RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                        return header->GetPackageType();
                    }
                }
            }
            set
            {
                unsafe
                {
                    fixed (byte* ptr = &mHandle[0])
                    {
                        RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                        header->SetPackageType(value);
                    }
                }
            }
        }

        public ushort SerialId
        {
            get
            {
                unsafe
                {
                    fixed (byte* ptr = &mHandle[0])
                    {
                        RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                        return header->GetSerialId();
                    }
                }
            }
            set
            {
                unsafe
                {
                    fixed (byte* ptr = &mHandle[0])
                    {
                        RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                        header->SetSerialId(value);
                    }
                }
            }
        }

        public bool IsSinglePkg
        {
            get
            {
                unsafe
                {
                    fixed (byte* ptr = &mHandle[0])
                    {
                        RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                        return header->IsSinglePkg();
                    }
                }
            }
        }

        public bool IsWeakPkg
        {
            get
            {
                unsafe
                {
                    fixed (byte* ptr = &mHandle[0])
                    {
                        RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                        return header->IsWeakPkg();
                    }
                }
            }
        }

        public void SetWeakPkg()
        {
            unsafe
            {
                fixed (byte* ptr = &mHandle[0])
                {
                    RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                    header->SetWeakPkg();
                }
            }
        }

        public void SetSinglePkg()
        {
            unsafe
            {
                fixed (byte* ptr = &mHandle[0])
                {
                    RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                    header->SetSinglePkg();
                }
            }
        }

        public void SetMethod(byte method)
        {
            unsafe
            {
                fixed (byte* ptr = &mHandle[0])
                {
                    RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                    header->SetMethod(method);
                }
            }
        }

        public void PushStack(byte stk)
        {
            unsafe
            {
                fixed (byte* ptr = &mHandle[0])
                {
                    RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                    header->SetStack((short)mStack++, stk);
                }
            }
        }

        public void Reset()
        {
            unsafe
            {
                fixed (byte* ptr = &mHandle[0])
                {
                    RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                    header->ToDefault();
                }
            }
            mStack = mPos = 0;
        }

        #region Data Writer

        public void WritePODObject(System.Object obj)
	    {
		    System.Type type = obj.GetType();
		    if(type==typeof(System.SByte))
		    {
			    Write( System.Convert.ToSByte(obj));
		    }
		    else if(type==typeof(System.Int16))
		    {
			    Write( System.Convert.ToInt16(obj));
		    }
		    else if(type==typeof(System.Int32))
		    {
			    Write( System.Convert.ToInt32(obj));
		    }
		    else if(type==typeof(System.Int64))
		    {
			    Write( System.Convert.ToInt64(obj));
		    }
		    else if(type==typeof(System.Byte))
		    {
			    Write( System.Convert.ToByte(obj));
		    }
            else if (type == typeof(System.Boolean))
            {
                Write(System.Convert.ToBoolean(obj));
            }
		    else if(type==typeof(System.UInt16))
		    {
			    Write( System.Convert.ToUInt16(obj));
		    }
		    else if(type==typeof(System.UInt32))
		    {
			    Write( System.Convert.ToUInt32(obj));
		    }
		    else if(type==typeof(System.UInt64))
		    {
			    Write( System.Convert.ToUInt64(obj));
		    }
		    else if(type==typeof(System.Single))
		    {
			    Write( System.Convert.ToSingle(obj));
		    }
		    else if(type==typeof(System.Double))
		    {
			    Write( System.Convert.ToDouble(obj));
		    }
		    else if(type==typeof(System.Guid))
		    {
			    //Guid id = CSUtility.Support.IHelper.GuidParse(obj.ToString());
			    System.Guid id = (System.Guid)(obj);
			    Write( id );
		    }
            else if (type == typeof(SlimDX.Vector2))
            {
                SlimDX.Vector2 v = (SlimDX.Vector2)(obj);
                Write(v);
            }
            else if (type == typeof(SlimDX.Vector3))
            {
                //Guid id = CSUtility.Support.IHelper.GuidParse(obj.ToString());
                SlimDX.Vector3 v = (SlimDX.Vector3)(obj);
                Write(v);
            }
            else if (type == typeof(SlimDX.Vector4))
            {
                SlimDX.Vector4 v = (SlimDX.Vector4)(obj);
                Write(v);
            }
            else if (type == typeof(SlimDX.Quaternion))
            {
                SlimDX.Quaternion q = (SlimDX.Quaternion)(obj);
                Write(q);
            }
            else if (type == typeof(SlimDX.Matrix))
            {
                SlimDX.Matrix mat = (SlimDX.Matrix)(obj);
                Write(mat);
            }
            else if (type == typeof(System.String))
            {
                System.String str = (System.String)(obj);
                Write(str);
            }
            else if (type == typeof(DataWriter))
            {
                var dr = (DataWriter)(obj);
                Write(dr);
            }
	    }

        public void Write(IAutoSaveAndLoad data)
        {
            data.PackageWrite(this);
        }

        public void Write(IAutoSaveAndLoad data, System.Type forceType)
        {
            data.PackageWrite(this, forceType);
        }

        public void Write(DataWriter data)
        {
            Write((ushort)data.Length);
            Write(data.Ptr, data.Length);
        }

        public void Write(string data)
	    {
            //TODO 将来需要用ansi码传优化，减少数据量
            if(data==null)
            {
                Write((ushort)0);
                return;
            }
                
            Write((ushort)data.Length);
            if (data.Length == 0)
                return;

            unsafe
            {
                fixed (char* pPtr = data)
                {
                    Write((IntPtr)pPtr, sizeof(char) * data.Length);
                }
            }
	    }


        public void Write(System.Char data)
        {
            unsafe
            {
                System.Char* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.Char));
            }
        }

        public void Write(System.Single data)
        {
            unsafe
            {
                System.Single* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.Single));
            }
        }

        public void Write(System.Double data)
        {
            unsafe
            {
                System.Double* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.Double));
            }
        }

        public void Write(System.SByte data)
        {
            unsafe
            {
                System.SByte* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.SByte));
            }
        }

        public void Write(System.Int16 data)
        {
            unsafe
            {
                System.Int16* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.Int16));
            }
        }
		
        public void Write(System.Int32 data)
        {
            unsafe
            {
                System.Int32* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.Int32));
            }
        }
		
        public void Write(System.Int64 data)
        {
            unsafe
            {
                System.Int64* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.Int64));
            }
        }
		
        public void Write(System.Byte data)
        {
            unsafe
            {
                System.Byte* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.Byte));
            }
        }

        public void Write(System.Boolean data)
        {
            unsafe
            {
                System.Byte temp = (byte)((data == false) ? 0 : 1);
                System.Byte* pPtr = &temp;
                Write((IntPtr)pPtr, sizeof(System.Byte));
            }
        }
		
        public void Write(System.UInt16 data)
        {
            unsafe
            {
                System.UInt16* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.UInt16));
            }
        }
		
        public void Write(System.UInt32 data)
        {
            unsafe
            {
                System.UInt32* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.UInt32));
            }
        }
		
        public void Write(System.UInt64 data)
        {
            unsafe
            {
                System.UInt64* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.UInt64));
            }
        }

        public void Write(System.Guid data)
        {
            unsafe
            {
                System.Guid* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(System.Guid));                
            }
        }

        public void Write(System.DateTime data)
        {
            Write(data.ToBinary());
        }

        public void Write(SlimDX.Vector2 data)
        {
            unsafe
            {
                SlimDX.Vector2* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(SlimDX.Vector2));
            }
        }
		
        public void Write(SlimDX.Vector3 data)
        {
            unsafe
            {
                SlimDX.Vector3* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(SlimDX.Vector3));
            }
        }

        public void Write(SlimDX.Vector4 data)
        {
            unsafe
            {
                SlimDX.Vector4* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(SlimDX.Vector4));
            }
        }

        public void Write(SlimDX.Quaternion data)
        {
            unsafe
            {
                SlimDX.Quaternion* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(SlimDX.Quaternion));
            }
        }

        public void Write(SlimDX.Matrix data)
        {
            unsafe
            {
                SlimDX.Matrix* pPtr = &data;
                Write((IntPtr)pPtr, sizeof(SlimDX.Matrix));
            }
        }

        public void Write(byte[] data)
        {
            var tpl = new byte[data.Length];
            var cdata = RPC.IAutoSaveAndLoad.CompressArray(data, tpl, (UInt32)data.Length);

            int len = cdata.Length;
            Write(len);
            len = tpl.Length;
            Write(len);
            Write(cdata, cdata.Length);
        }

        public void Write(byte[] data,int length)
        {
            if (length == 0)
                return;
            FixSize(length);
            Buffer.BlockCopy(data, 0, mHandle, CurPtr(), length);
            mPos += length;
        }

        public void Write(IntPtr data,int length)
        {
            if (length == 0)
                return;
            FixSize(length);
            unsafe
            {
                Marshal.Copy(data, mHandle, CurPtr(), length);
            }
            mPos += length;
        }




        #endregion

        #region Send Package
        public void SendBuffer(CSUtility.Net.NetConnection conn)
	    {
            //int length = 0;
            //mHeader.head_length = (ushort)(GetPosition() + RPCHeader.SizeOf());

            if (CurPtr() < 65535-2)
		    {
                if (conn != null)
                {
                    if (this.CallerCounter != null)
                    {
                        this.CallerCounter.CallCounter++;
                        this.CallerCounter.WriteSize += this.CurPtr();
                    }
                    conn.SendBuffer(mHandle, 0, CurPtr());//这里内部做了多线程Lock
                }
		    }
            else
            {
                //assert(false);//这里以后要超过512就压缩一下包
                Log.FileLog.WriteLine("Error!!SendBuffer mPos>=65535-2");
            }
	    }

        public void DoCommand(CSUtility.Net.NetConnection conn, CommandTargetType target)
	    {
		    switch (target)
		    {
		    case CommandTargetType.DefaultType:
                PkgType = PackageType.PKGT_Send;
			    break;
		    case CommandTargetType.Planes:
			    PkgType = PackageType.PKGT_C2P_Send;
			    break;
		    }

		    SendBuffer(conn);
	    }

        public void DoReturnCommand(CSUtility.Net.NetConnection conn, CommandTargetType target)
	    {
		    switch (target)
		    {
		    case CommandTargetType.DefaultType:
                PkgType = PackageType.PKGT_Return;
			    break;
		    case CommandTargetType.Planes:
                PkgType = PackageType.PKGT_C2P_Return;
			    break;
		    }
		    SendBuffer(conn);
	    }

	    public void DoReturnCommand2(CSUtility.Net.NetConnection conn,System.UInt16 serialId)
	    {
		    SerialId = serialId;
            PkgType = PackageType.PKGT_Return;
		    SendBuffer(conn);
	    }

        public RPCWaitHandle WaitDoCommand(CSUtility.Net.NetConnection conn, CommandTargetType target, System.Diagnostics.StackTrace st)
	    {//其实这里有很微弱的多线程问题，后面如果设置delegate的时候，网络已经返回的话，不过这个情况应该基本不可能发生
		    switch (target)
		    {
		    case CommandTargetType.DefaultType:
			    PkgType = PackageType.PKGT_SendAndWait;
			    break;
		    case CommandTargetType.Planes:
			    PkgType = PackageType.PKGT_C2P_SendAndWait;
			    break;
		    }
            if (RPCManager == null)
                RPCManager = RPCNetworkMgr.Instance;
            RPCWaitHandle handle = RPCManager.NewWaitHandle(st);
            SerialId = handle.CallID;
		    SendBuffer(conn);
		    return handle;
	    }

        public RPCWaitHandle WaitDoCommandWithTimeOut(float timeOut, CSUtility.Net.NetConnection conn, CommandTargetType target, System.Diagnostics.StackTrace st)
        {//其实这里有很微弱的多线程问题，后面如果设置delegate的时候，网络已经返回的话，不过这个情况应该基本不可能发生
            switch (target)
            {
                case CommandTargetType.DefaultType:
                    PkgType = PackageType.PKGT_SendAndWait;
                    break;
                case CommandTargetType.Planes:
                    PkgType = PackageType.PKGT_C2P_SendAndWait;
                    break;
            }
            if (RPCManager == null)
                RPCManager = RPCNetworkMgr.Instance;
            RPCWaitHandle handle = RPCManager.NewWaitHandleTimeOut(st, timeOut);
            handle.mIsWeakPkg = this.IsWeakPkg;
            SerialId = handle.CallID;
            SendBuffer(conn);
            return handle;
        }

        public void _SyncDoCommand(CSUtility.Net.TcpClient cltConn, int timeout)
	    {
            if (RPCManager == null)
                RPCManager = RPCNetworkMgr.Instance;
		    PkgType = PackageType.PKGT_SendAndWait;
            RPCWaitHandle handle = RPCManager.NewWaitHandle(null);
		    SendBuffer(cltConn);
		    while(true)
		    {
			    cltConn.Update();
                if (null == RPCManager.GetWaitHandle(handle.CallID))
				    break;

			    System.Threading.Thread.Sleep(1);
		    }
	    }

        public void DoCommandPlanes2Client(CSUtility.Net.NetConnection conn, System.UInt16 clientLinkId)
	    {
		    Write(clientLinkId);
		    PkgType = PackageType.PKGT_P2C_Send;
		    SendBuffer(conn);
	    }

        public void DoReturnGate2Client(RPCForwardInfo fwd)
	    {
		    SerialId = fwd.ReturnSerialId;
		    PkgType = PackageType.PKGT_Return;
		    SendBuffer(fwd.Gate2ClientConnect);
	    }

        public void DoReturnPlanes2Client(RPCForwardInfo fwd)
	    {
		    SerialId = fwd.ReturnSerialId;
		    Write(fwd.Handle);
		    DoReturnCommand(fwd.Planes2GateConnect,CommandTargetType.Planes);
	    }

        public void DoReturnPlanes2Client(CSUtility.Net.NetConnection p2gConnect, UInt16 clientLinkeId, UInt16 serialId)
        {
            SerialId = serialId;
            Write(clientLinkeId);
            DoReturnCommand(p2gConnect, CommandTargetType.Planes);
        }

        #region 客户端特有

        public void DoClient2Planes(CSUtility.Net.NetConnection conn)
	    {
		    PkgType = PackageType.PKGT_C2P_Send;
		    SendBuffer(conn);
	    }

        public RPCWaitHandle WaitDoClient2Planes(CSUtility.Net.NetConnection conn)
	    {
		    PkgType = PackageType.PKGT_C2P_SendAndWait;

            if (RPCManager == null)
                RPCManager = RPCNetworkMgr.Instance;
            RPCWaitHandle handle = RPCManager.NewWaitHandle(null);
		    SerialId = handle.CallID;
		    SendBuffer(conn);
		    return handle;
	    }

        public void DoClient2PlanesPlayer(CSUtility.Net.NetConnection conn)
	    {
		    PkgType = PackageType.PKGT_C2P_Player_Send;
		    SendBuffer(conn);
	    }

        public RPCWaitHandle WaitDoClient2PlanesPlayer(CSUtility.Net.NetConnection conn,float timeOut)
	    {
		    PkgType = PackageType.PKGT_C2P_Player_SendAndWait;
            if (RPCManager == null)
                RPCManager = RPCNetworkMgr.Instance;
            RPCWaitHandle handle = RPCManager.NewWaitHandleTimeOut(null, timeOut);
		    SerialId = handle.CallID;
		    SendBuffer(conn);
		    return handle;
	    }
        #endregion

        #endregion
    }
}
