using System;

namespace CSUtility.Support
{
    public class XndAttrib
    {
        private IntPtr mHandle;
        public IntPtr Handle
        {
            get { return mHandle; }
        }

        public Byte Version
        {
            get
            {
                unsafe
                {
                    if(mHandle != IntPtr.Zero)
                        return DllImportAPI.XNDAttrib_GetVersion(mHandle);
                    return 0;
                }
            }
            set
            {
                unsafe
                {
                    if (mHandle != IntPtr.Zero)
                        DllImportAPI.XNDAttrib_SetVersion(mHandle, value);
                }
            }
        }

        public UInt32 Length
        {
            get
            {
                unsafe
                {
                    if (mHandle != IntPtr.Zero)
                        return DllImportAPI.XNDAttrib_GetLength(mHandle);

                    return 0;
                }
            }
        }

        public string Key
        {
            get
            {
                unsafe
                {
                    if (mHandle != IntPtr.Zero)
                        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.XNDAttrib_GetKey(mHandle));
                    return "";
                }
            }
            set
            {
                unsafe
                {
                    if (mHandle != IntPtr.Zero)
                        DllImportAPI.XNDAttrib_SetKey(mHandle, value);
                }
            }
        }
        
        public XndAttrib(IntPtr attr)
		{
            unsafe
            {
                DllImportAPI.XNDAttrib_AddRef((IntPtr)(attr.ToPointer()));
                mHandle = (IntPtr)attr;
            }
		}
        ~XndAttrib()
        {
            unsafe
            {
                DllImportAPI.XNDAttrib_Release(mHandle);
            }
        }

        public string GetName()
		{
            unsafe
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.XNDAttrib_GetName(mHandle));
            }
		}

        public bool BeginRead()
		{
            if (mHandle == (IntPtr)0)
				return false;

            unsafe
            {
                DllImportAPI.XNDAttrib_BeginRead(mHandle);
            }
			return true;
		}

		public void EndRead()
		{
            if (mHandle != (IntPtr)0)
            {
                unsafe
                {
                    DllImportAPI.XNDAttrib_EndRead(mHandle);
                }
            }
		}

        public bool BeginWrite()
        {
            if (mHandle == (IntPtr)0)
                return false;

            unsafe
            {
                DllImportAPI.XNDAttrib_BeginWrite(mHandle);
            }
            return true;
        }

        public void EndWrite()
        {
            if (mHandle != (IntPtr)0)
            {
                unsafe
                {
                    DllImportAPI.XNDAttrib_EndWrite(mHandle);
                }
            }
        }

        #region 原生数据读取
		public bool Read(out System.Byte[] data, int length)
		{
            data = new System.Byte[length];

			if (mHandle == (IntPtr)0)
            {
				return false;
            }

            unsafe
            {
                fixed (void* pinData = &data[0])
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)pinData, sizeof(System.Byte) * length);
                }
            }

			return true;
		}

		public bool Read(out System.Boolean data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = false;
				return false;
            }

            unsafe
            {
                fixed (void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)pinData, sizeof(System.Boolean));
                }
            }

			return true;
		}

		public bool Read(out System.Int16 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(Int16));
                }
            }
			return true;
		}
		public bool Read(out System.Int32 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(Int32));
                }
            }
			return true;
		}
		public bool Read(out System.Int64 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(Int64));
                }
            }
			return true;
		}
		public bool Read(out System.UInt16 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(UInt16));
                }
            }
			return true;
		}
		public bool Read(out System.UInt32 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(UInt32));
                }
            }
			return true;
		}
		public bool Read(out System.UInt64 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(UInt64));
                }
            }
			return true;
		}
		public bool Read(out System.Single data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(Single));
                }
            }
			return true;
		}
		public bool Read(out System.Double data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(Double));
                }
            }
			return true;
		}
		public bool Read(out System.Byte data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(Byte));
                }
            }
			return true;
		}
		public bool Read(out System.SByte data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = 0;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(SByte));
                }
            }
			return true;
		}
		public bool Read(out System.Guid data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = Guid.Empty;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(Guid));
                }
            }
			return true;
		}
		public bool Read(out System.String data)
		{
            if (mHandle == (IntPtr)0)
            {
                data = "";
				return false;
            }

            unsafe
            {
                IntPtr strPtr = DllImportAPI.XNDAttrib_ReadStringW(mHandle);
                data = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strPtr);
                DllImportAPI.XNDAttrib_FreeStringW(strPtr);
            }
			return true;
		}

		/*bool Read(IXndSaveLoadProxy^ data)
		{
			if(data == nullptr)
				return false;

			return data->Read(this);
		}*/

		public bool Read(out SlimDX.Vector2 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = new SlimDX.Vector2();
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(SlimDX.Vector2));
                }
            }
			return true;
		}
		public bool Read(out SlimDX.Vector3 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = new SlimDX.Vector3();
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(SlimDX.Vector3));
                }
            }
			return true;
		}
		public bool Read(out SlimDX.Vector4 data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = new SlimDX.Vector4();
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(SlimDX.Vector4));
                }
            }
			return true;
		}
		public bool Read(out SlimDX.Quaternion data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = SlimDX.Quaternion.Identity;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(SlimDX.Quaternion));
                }
            }
			return true;
		}
		public bool Read(out SlimDX.Matrix data)
		{
			if (mHandle == (IntPtr)0)
            {
                data = SlimDX.Matrix.Identity;
				return false;
            }

            unsafe
            {
                fixed(void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData),sizeof(SlimDX.Matrix));
                }
            }
			return true;
		}
		public bool Read(out SlimDX.Matrix3x2 data)
		{
            if (mHandle == (IntPtr)0)
            {
                data = SlimDX.Matrix3x2.Identity;
                return false;
            }

            unsafe
            {
                fixed (void* pinData = &data)
                {
                    DllImportAPI.XNDAttrib_Read(mHandle, (IntPtr)(pinData), sizeof(SlimDX.Matrix3x2));
                }
            }
            return true;
		}

        #endregion 原生数据读取

        #region 原生数据写入

        public bool Write(System.Byte[] data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                fixed (byte* p = &data[0])
                {
                    DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(p), sizeof(System.Byte) * data.Length);
                }
            }

            return true;
        }
        public bool Write(System.Boolean data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(System.Boolean));
            }

            return true;
        }

        public bool Write(System.Int16 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(Int16));
            }
            return true;
        }
        public bool Write(System.Int32 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(Int32));
            }
            return true;
        }
        public bool Write(System.Int64 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(Int64));
            }
            return true;
        }
        public bool Write(System.UInt16 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(UInt16));
            }
            return true;
        }
        public bool Write(System.UInt32 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(UInt32));
            }
            return true;
        }
        public bool Write(System.UInt64 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(UInt64));
            }
            return true;
        }
        public bool Write(System.Single data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(Single));
            }
            return true;
        }
        public bool Write(System.Double data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(Double));
            }
            return true;
        }
        public bool Write(System.Byte data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(Byte));
            }
            return true;
        }
        public bool Write(System.SByte data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(SByte));
            }
            return true;
        }
        public bool Write(System.Guid data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(Guid));
            }
            return true;
        }
        public bool Write(System.String data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                //IntPtr strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalUni(data);
                DllImportAPI.XNDAttrib_WriteStringW(mHandle, data);
                //System.Runtime.InteropServices.Marshal.FreeHGlobal(strPtr);
            }
            return true;
        }

        /*bool Read(IXndSaveLoadProxy^ data)
        {
            if(data == nullptr)
                return false;

            return data->Read(this);
        }*/

        public bool Write(SlimDX.Vector2 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(SlimDX.Vector2));
            }
            return true;
        }
        public bool Write(SlimDX.Vector3 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(SlimDX.Vector3));
            }
            return true;
        }
        public bool Write(SlimDX.Vector4 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(SlimDX.Vector4));
            }
            return true;
        }
        public bool Write(SlimDX.Quaternion data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(SlimDX.Quaternion));
            }
            return true;
        }
        public bool Write(SlimDX.Matrix data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(SlimDX.Matrix));
            }
            return true;
        }
        public bool Write(SlimDX.Matrix3x2 data)
        {
            if (mHandle == (IntPtr)0)
            {
                return false;
            }

            unsafe
            {
                DllImportAPI.XNDAttrib_Write(mHandle, (IntPtr)(&data), sizeof(SlimDX.Matrix3x2));
            }
            return true;
        }
        #endregion
    }
}
