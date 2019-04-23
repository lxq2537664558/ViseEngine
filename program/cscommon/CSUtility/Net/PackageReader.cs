using System;

using System.Runtime.InteropServices;

namespace RPC
{
    public class FieldDontAutoSaveLoadAttribute : Attribute
	{
		
	};

	public class FieldDontAutoSingleSaveLoadAttribute : Attribute
	{

	};

	public class FieldAutoSaveLoadAttribute : Attribute
	{
		public int			Level;
        public FieldAutoSaveLoadAttribute(int level)
		{
			Level = level;
		}
	};

    public class EnumSizeAttribute : Attribute
    {
        public Type SizeType;
        public EnumSizeAttribute(Type sizeType)
        {
            SizeType = sizeType;
        }
    }

    public class FixedSizeAttribute : Attribute
    {
        public int FixedSize;
        public FixedSizeAttribute(int size)
        {
            FixedSize = size;
        }
    }

    public class PackageProxy
    {
        protected byte[] mHandle;
        protected int mPos;
        protected int mSize;
        public Int64 RecvTime;

        public PackageProxy(byte[] ptr,int size,Int64 time)
        {
            if (size > ptr.Length)
            {
                Log.FileLog.WriteLine("PackageProxy error size > ptr.Length");
            }

            mHandle = ptr;
            mSize = size;
            mPos = 0;
            RecvTime = time;
        }

        int DataPtr()
        {
            return RPCHeader.SizeOf();
        }
        public int CurPtr()
        {
            return RPCHeader.SizeOf() + mPos;
        }

        public void OnReadError()
        {

        }


        public static int HeaderSize
        {
            get
            {
                return RPCHeader.SizeOf();
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
                        RPC.RPCHeader * header = (RPC.RPCHeader*)ptr;
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

        public UInt16 SerialId
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

        public void SeekTo(int pos)
        {
            mPos = pos;
        }

        //public void DangrouseSetPkgLength(int length)
        //{
        //    unsafe
        //    {
        //        IDllImportAPI.Net_PackageReader_SetPkgLength(mHandle.ToPointer(), length);
        //    }
        //}

        public int GetMaxStack()
        {
            return 4;
        }

        public byte GetStack(short Index)
        {
            unsafe
            {
                fixed (byte* ptr = &mHandle[0])
                {
                    RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                    return header->GetStack(Index);
                }
            }
        }

        public byte GetMethod()
        {
            unsafe
            {
                fixed (byte* ptr = &mHandle[0])
                {
                    RPC.RPCHeader* header = (RPC.RPCHeader*)ptr;
                    return header->GetMethod();
                }
            }
        }

        #region Data Reader
        public void ReadLength(int size)
        {

        }

        public void Read(IAutoSaveAndLoad data)
        {
            data.PackageRead(this);
        }

        public void Read(out DataReader data)
        {
            UInt16 wSize = 0;
		    Read(out wSize);

            if (CurPtr() + wSize > mSize)
            {
                data = null;
                OnReadError();
                return;
            }

            data = new DataReader(mHandle, CurPtr(), wSize, mSize - mPos);
            mPos += wSize;
            //unsafe
            //{
            //    int pos = IDllImportAPI.Net_PackageReader_Tell(mHandle.ToPointer());
            //    IntPtr ptr = (IntPtr)IDllImportAPI.Net_PackageReader_CurPtr(mHandle.ToPointer(), wSize);
            //    if (ptr!=(IntPtr)0)
            //        data = new DataReader(ptr, wSize, mSize - pos);
            //    else
            //        data = new DataReader(ptr, 0, 0);
            //}
        }

        public void Read(out string data)
        {
            ushort length;
            Read(out length);
            if (length == 0)
            {
                data = "";
                return;
            }


            int buffSize = sizeof(char) * length;

            if (CurPtr() + buffSize > mSize)
            {
                data = "";
                OnReadError();
                return;
            }

            char[] chars = new char[length];
            unsafe
            {
                fixed (char* pValue = chars)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, buffSize);
                }
            }
            mPos += buffSize;
            data = new string(chars);
        }

        public void Read(out System.Char data)
        {
            int length = Marshal.SizeOf(typeof(System.Char));

            if (CurPtr() + length > mSize)
            {
                data = '0';
                return;
            }

            unsafe
            {
                fixed (System.Char* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.Single data)
        {
            int length = Marshal.SizeOf(typeof(System.Single));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.Single* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.Double data)
        {
            int length = Marshal.SizeOf(typeof(System.Double));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.Double* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.SByte data)
        {
            int length = Marshal.SizeOf(typeof(System.SByte));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.SByte* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.Int16 data)
        {
            int length = Marshal.SizeOf(typeof(System.Int16));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.Int16* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.Int32 data)
        {
            int length = Marshal.SizeOf(typeof(System.Int32));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.Int32* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.Int64 data)
        {
            int length = Marshal.SizeOf(typeof(System.Int64));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.Int64* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.Byte data)
        {
            int length = Marshal.SizeOf(typeof(System.Byte));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.Byte* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.Boolean data)
        {
            int length = Marshal.SizeOf(typeof(System.Byte));
            if (CurPtr() + length > mSize)
            {
                data = false;
                OnReadError();
                return;
            }
            unsafe
            {
                System.Byte temp = new System.Byte();
                System.Byte* pValue = &temp;
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                    mPos += length;
                }
                data = (temp == 0) ? false : true;
            }
        }

        public void Read(out System.UInt16 data)
        {
            int length = Marshal.SizeOf(typeof(System.UInt16));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.UInt16* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.UInt32 data)
        {
            int length = Marshal.SizeOf(typeof(System.UInt32));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.UInt32* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.UInt64 data)
        {
            int length = Marshal.SizeOf(typeof(System.UInt64));
            if (CurPtr() + length > mSize)
            {
                data = 0;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (System.UInt64* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.Guid data)
        {
            int length = Marshal.SizeOf(typeof(System.Guid));
            if (CurPtr() + length > mSize)
            {
                data = Guid.Empty;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (Guid* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.DateTime data)
        {
            if (CurPtr() + sizeof(long) > mSize)
            {
                data = System.DateTime.MinValue;
                OnReadError();
                return;
            }
            long bin;
            unsafe
            {
                long* pValue = &bin;
                Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, sizeof(long));
                try
                {
                    data = System.DateTime.FromBinary(bin);
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                    data = System.DateTime.Now;
                }
            }
            mPos += sizeof(long);
        }

        public void Read(out SlimDX.Vector2 data)
        {
            int length = Marshal.SizeOf(typeof(SlimDX.Vector2));
            if (CurPtr() + length > mSize)
            {
                data = SlimDX.Vector2.Zero;
                OnReadError();
                return;
            }
            unsafe
            {
                fixed (SlimDX.Vector2* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out SlimDX.Vector3 data)
        {
            int length = Marshal.SizeOf(typeof(SlimDX.Vector3));
            if (CurPtr() + length > mSize)
            {
                data = SlimDX.Vector3.Zero;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (SlimDX.Vector3* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out SlimDX.Vector4 data)
        {
            int length = Marshal.SizeOf(typeof(SlimDX.Vector4));
            if (CurPtr() + length > mSize)
            {
                data = new SlimDX.Vector4();
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (SlimDX.Vector4* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out SlimDX.Quaternion data)
        {
            int length = Marshal.SizeOf(typeof(SlimDX.Quaternion));
            if (CurPtr() + length > mSize)
            {
                data = SlimDX.Quaternion.Identity;
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (SlimDX.Quaternion* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out SlimDX.Matrix data)
        {
            int length = Marshal.SizeOf(typeof(SlimDX.Matrix));
            if (CurPtr() + length > mSize)
            {
                data = SlimDX.Matrix.Identity;
                OnReadError();
                return;
            }
            unsafe
            {
                fixed (SlimDX.Matrix* pValue = &data)
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }
            mPos += length;            

        }

        public void Read(byte[] data, int length)
        {
            if (data.Length < length)
                return;
            if (CurPtr() + length > mSize)
            {
                OnReadError();
                return;
            }

            unsafe
            {
                fixed (byte* pValue = &data[0])
                {
                    Marshal.Copy(mHandle, CurPtr(), (IntPtr)pValue, length);
                }
            }

            mPos += length;
        }

        public void Read(out byte[] data)
        {
            int clen = 0;
            int tlen = 0;
            this.Read(out clen);
            this.Read(out tlen); 
            
            if (tlen < 0 || tlen >= UInt16.MaxValue || clen < 0 || clen >= UInt16.MaxValue)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("PackageReader.Read byte[]: Compresslength = {0},TemplateLength = {1}", clen, tlen));
                System.Diagnostics.Trace.WriteLine(new System.Diagnostics.StackFrame());
                data = new byte[1];
                return;
            }

            data = new byte[clen];
            Read(data, clen);

            var tpl = new byte[tlen];
            var cdata = RPC.IAutoSaveAndLoad.UnCompressArray(data, tpl, (UInt32)tlen);
            data = cdata;
        }
        #endregion
    }

    public class RPCSpecialHolder : PackageProxy
	{
        public RPCObject mRoot;
        public RPCForwardInfo mForward;

        public RPCSpecialHolder(byte[] ptr, int size, Int64 rcvTime)
            : base(ptr, size, rcvTime)
        {
            DeepCopy(ptr, size);
        }
        private void DeepCopy(byte[] ptr, int size)
	    {
            mHandle = new byte[size + 4];
            Buffer.BlockCopy(ptr, 0, mHandle, 0, size);
	    }

        public void DestroyBuffer()
        {
        }
	};
}
