using System;

namespace CCore.Socket
{
    /// <summary>
    /// 挂接
    /// </summary>
    public class Socket
    {
        /// <summary>
        /// 挂接件的指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 挂接件的对象指针
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                mInner = value;
            }
        }

		SocketTable mHolder;
        //model3::v3dBone *mParentBone;
        /// <summary>
        /// 构造函数
        /// </summary>
		protected Socket()
        {
        }
        /// <summary>
        /// 析构函数，删除对象，释放指针
        /// </summary>
        ~Socket()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
		protected void Cleanup()
        {
            if (mInner != IntPtr.Zero)
            {
                DllImportAPI.V3DSocket_Release(mInner);
                mInner = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 创建挂接件对象
        /// </summary>
        /// <param name="holder">挂接面板对象</param>
        /// <param name="inner">创建的挂接对象指针</param>
        /// <returns>返回创建好的挂接对象</returns>
        public static Socket CreateSocket(SocketTable holder, IntPtr inner)
		{
            var result = new Socket();
			result.mHolder = holder;
            result.Inner = inner;
            DllImportAPI.V3DSocket_AddRef(inner);
			return result;
		}
        /// <summary>
        /// 挂接件的名称
        /// </summary>
        public string Name
        {
            get { unsafe { return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.V3DSocket_GetName(mInner)); } }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetName(mInner, value);
                }
            }            
        }
        /// <summary>
        /// 父骨头的名称
        /// </summary>
        public string ParentBoneName
        {
            get { unsafe { return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.V3DSocket_GetParentBoneName(mInner)); } }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetParentBoneName(mInner, value);
                }
            }
        }
        /// <summary>
        /// 挂接件的位置坐标
        /// </summary>
        public SlimDX.Vector3 Pos
        {
            get 
            { 
                unsafe 
                { 
                    SlimDX.Vector3 temp = new SlimDX.Vector3();
                    DllImportAPI.V3DSocket_GetPos(mInner, &temp);
                    return temp;
                } 
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetPos(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 挂接件的缩放值
        /// </summary>
        public SlimDX.Vector3 Scale
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 temp = new SlimDX.Vector3();
                    DllImportAPI.V3DSocket_GetScale(mInner, &temp);
                    return temp;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetScale(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 挂接件的位置四元数
        /// </summary>
        public SlimDX.Quaternion Quat
        {
            get
            {
                unsafe
                {
                    SlimDX.Quaternion temp = new SlimDX.Quaternion();
                    DllImportAPI.V3DSocket_GetQuat(mInner, &temp);
                    return temp;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetQuat(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 挂接件的绝对坐标位置
        /// </summary>
        public SlimDX.Vector3 AbsPos
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 temp = new SlimDX.Vector3();
                    DllImportAPI.V3DSocket_GetAbsPos(mInner, &temp);
                    return temp;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetAbsPos(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 挂接件的绝对缩放值
        /// </summary>
        public SlimDX.Vector3 AbsScale
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 temp = new SlimDX.Vector3();
                    DllImportAPI.V3DSocket_GetAbsScale(mInner, &temp);
                    return temp;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetAbsScale(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 挂接件的绝对位置四元数
        /// </summary>
        public SlimDX.Quaternion AbsQuat
        {
            get
            {
                unsafe
                {
                    SlimDX.Quaternion temp = new SlimDX.Quaternion();
                    DllImportAPI.V3DSocket_GetAbsQuat(mInner, &temp);
                    return temp;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetAbsQuat(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 只读属性，挂接件的绝对矩阵
        /// </summary>
        public SlimDX.Matrix AbsMatrix
        {
            get
            {
                unsafe
                {
                    var temp = new SlimDX.Matrix();
                    DllImportAPI.V3DSocket_GetAbsMatrix(mInner, &temp);
                    return temp;
                }
            }
        }
        /// <summary>
        /// 父类对象在挂接面板中的索引
        /// </summary>
        public int ParentIndexInFullSocketTable
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DSocket_GetParentIndexInFullSocketTable(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetParentIndexInFullSocketTable(mInner, value);
                }
            }
        }
        /// <summary>
        /// 是否跟随父类对象旋转
        /// </summary>
        public bool IsInheritRotate
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DSocket_GetInheritRotate(mInner)>0?true:false;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DSocket_SetInheritRotate(mInner, value?1:0);
                }
            }
        }

    };
}
