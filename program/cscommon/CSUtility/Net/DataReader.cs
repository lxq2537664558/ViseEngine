using System;

using System.Runtime.InteropServices;

namespace RPC
{
    public class DataReader
    {
        public byte[] mHandle;
        public int mPos;
        protected int mSize;

        public DataReader(Byte[] addr)
        {
            mHandle = addr;
            mPos = 0;
            mSize = addr.Length;
        }

        public DataReader(Byte[] addr,int startIndex, int size, int remain)
        {
            Proxy(addr, startIndex, size, remain);
        }

        public void Cleanup()
        {
            mHandle = null;
            mPos = 0;
        }

        void OnReadError()
        {

        }


        int DataPtr()
        {
            return 0;
        }
        int CurPtr()
        {
            return mPos;
        }

        public int Length
        {
            get { return mSize; }
        }

        public void Proxy(byte[] addr,int startIndex,int size,int remain)
        {
            Cleanup();
            if (size > remain)
            {
                return;
            }            
            mHandle = new byte[size];
            Buffer.BlockCopy(addr, startIndex, mHandle, 0, size);
            mPos = 0;
            mSize = size;
        }

        //void DeepCopy(byte[] addr)
        //{
        //    mHandle = new byte[addr.Length + 4];
        //    Buffer.BlockCopy(addr, 0, mHandle, 0, addr.Length);
        //    mPos = 0;
        //    mIsDeepCopy = true;
        //}

        #region Data Reader

        public void Read( IAutoSaveAndLoad result ,bool bToClient )
	    {
		    result.DataRead(this,bToClient);
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
            int len = 0;
            this.Read(out len);
            data = new byte[len];
            Read(data, len);
        }

	    public void Read(out DataReader data)
	    {
		    System.UInt16 size;
		    Read(out size);

            if (CurPtr() + size > mSize)
            {
                data = null;
                OnReadError();
                return;
            }

            data = new DataReader(mHandle,mPos, size, mSize - mPos);
            //unsafe
            //{
            //    int pos = IDllImportAPI.Net_StreamReader_Tell(mHandle.ToPointer());
            //    IntPtr ptr = (IntPtr)IDllImportAPI.Net_StreamReader_CurPtr(mHandle.ToPointer(), (int)size);
            //    if (ptr!=(IntPtr)0)
            //        data = new DataReader(ptr, (int)size, mSize - pos);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, buffSize);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                System.Byte temp =new System.Byte();
                System.Byte* pValue = &temp;
                {
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
                }
            }
            mPos += length;
        }

        public void Read(out System.DateTime data)
        {
            long bin = BitConverter.ToInt64(mHandle, CurPtr());
            if (CurPtr() + sizeof(long) > mSize)
            {
                data = System.DateTime.MinValue;
                OnReadError();
                return;
            }

            mPos += sizeof(long);
            data = System.DateTime.FromBinary(bin);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
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
                    Marshal.Copy(mHandle, mPos, (IntPtr)pValue, length);
                }
            }
            mPos += length;

        }


        #endregion
    }
}
