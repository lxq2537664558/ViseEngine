using System;

using System.Runtime.InteropServices;

namespace RPC
{
    public class DataWriter
    {
        //System.IntPtr mHandle;
        byte[] mHandle;
        int mBuffSize = 128;
		int mPos = 0;
        public DataWriter()
        {
            mHandle = new byte[128];
        }

        public int DataPtr()
        {
            return 0;
        }
        public int CurPtr()
        {
            return mPos;
        }
        public int GetLength()
        {
            return mPos;
        }

        public byte[] Ptr
        {
            get
            {
                return mHandle;
            }
        }

        public int Length
        {
            get
            {
                return mPos;
            }
        }

        int C_MAXDATASIZE = 64 * 1024;
        public void FixSize(int growSize)
        {
            if (growSize >= C_MAXDATASIZE)
            {
                Log.FileLog.WriteLine("DataWrite FixSIze = {0}", growSize);
                throw new Exception("OutOfMemory By DataWrite FixSIze");
            }
            int nsize = mPos + growSize;
            if (nsize > mBuffSize)
            {
                nsize += mBuffSize / 2;
                byte[] nBuffer = new byte[nsize];
                Buffer.BlockCopy(mHandle, 0, nBuffer, 0, mPos);
                mHandle = nBuffer;
                mBuffSize = nsize;
            }
        }


        #region Data Writer
        public void Write(IAutoSaveAndLoad data,bool bToClient)
	    {
		    data.DataWrite(this,bToClient);
	    }

        public void Write(IAutoSaveAndLoad data, bool bToClient, System.Type forceType)
        {
            data.DataWrite(this, bToClient, forceType);
        }

        public void Write(DataWriter data)
        {
            Write((ushort)data.Length);
            Write(data.Ptr,data.CurPtr());
        }

        public void Write(string data)
        {
            //TODO 将来需要用ansi码传优化，减少数据量
            if (data == null)
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

        public void Write(byte[] data,int length)
        {
            FixSize(length);
            Buffer.BlockCopy(data, 0, mHandle, mPos, length);
            mPos += length;
        }

        public void Write(byte[] data)
        {
            int len = data.Length;
            Write(len);
            Write(data, data.Length);
        }

        public void Write(IntPtr data, int length)
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
    }
}
