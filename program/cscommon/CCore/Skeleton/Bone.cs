using System;
using System.Collections.Generic;
/// <summary>
/// 骨骼的命名空间
/// </summary>
namespace CCore.Skeleton
{
    /// <summary>
    /// 骨头
    /// </summary>
    public class Bone
    {
        /// <summary>
        /// 骨头的指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 骨头的指针
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                mInner = value;
            }
        }

		Skeleton mSkeleton;
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="skeleton">骨骼对象</param>
		public Bone(Skeleton skeleton)
        {
            mSkeleton = skeleton;
        }
        /// <summary>
        /// 只读属性，骨头的名称
        /// </summary>
        public string Name
        {
            get { unsafe { return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.V3DBone_GetName(mInner)); } }
        }

        /// <summary>
        /// 骨头对象的位置坐标
        /// </summary>
        public SlimDX.Vector3 Pos
        {
            get 
            { 
                unsafe 
                { 
                    SlimDX.Vector3 temp = new SlimDX.Vector3();
                    DllImportAPI.V3DBone_GetPos(mInner, &temp);
                    return temp;
                } 
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DBone_SetPos(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 骨骼的缩放值
        /// </summary>
        public SlimDX.Vector3 Scale
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 temp = new SlimDX.Vector3();
                    DllImportAPI.V3DBone_GetScale(mInner, &temp);
                    return temp;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DBone_SetScale(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 骨骼位置的四元数
        /// </summary>
        public SlimDX.Quaternion Quat
        {
            get
            {
                unsafe
                {
                    SlimDX.Quaternion temp = new SlimDX.Quaternion();
                    DllImportAPI.V3DBone_GetQuat(mInner, &temp);
                    return temp;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DBone_SetQuat(mInner, &value);
                }
            }
        }
        /// <summary>
        /// 只读属性，骨骼位置的绝对矩阵
        /// </summary>
        public SlimDX.Matrix AbsMatrix
        {
            get
            {
                unsafe
                {
                    var temp = new SlimDX.Matrix();
                    DllImportAPI.V3DBone_GetAbsMatrix(mInner, &temp);
                    return temp;
                }
            }
        }
        /// <summary>
        /// 获取子类对象
        /// </summary>
        /// <returns>返回该对象的子类骨骼列表</returns>
        public List<Bone> GetChildren()
        {
            int iChildSize = DllImportAPI.V3DBone_GetChildSize(mInner);
            if (iChildSize <= 0)
			    return null;

		    System.Collections.Generic.List<Bone> childBones = new System.Collections.Generic.List<Bone>();
            for (int i = 0; i < iChildSize; ++i)
		    {
                int childIndex = DllImportAPI.V3DBone_GetChildIndex(mInner, i);
			    childBones.Add( mSkeleton.GetBone(childIndex) );
		    }

		    return childBones;

        }
    }

}
