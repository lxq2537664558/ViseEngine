using System;
using System.Collections.Generic;
/// <summary>
/// 骨骼的命名空间
/// </summary>
namespace CCore.Skeleton
{
    /// <summary>
    /// 骨骼
    /// </summary>
    public class Skeleton
    {
        /// <summary>
        /// 骨骼对象
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 骨骼对象指针
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                if( value == IntPtr.Zero )
                    return;

                DllImportAPI.V3DSkeleton_AddRef(value);
                mBones.Clear();
                DllImportAPI.V3DSkeleton_Release(mInner);
                mInner = value;
               
		        for(int i = 0; i < DllImportAPI.V3DSkeleton_GetBoneCount(mInner); ++i)
		        {
			        Bone bone = new Bone(this);
			        bone.Inner = DllImportAPI.V3DSkeleton_GetBone(mInner, i);
			        mBones.Add(bone);
		        }
            }
        }
		List<Bone> mBones = new List<Bone>();
        /// <summary>
        /// 骨骼的构造函数，用于创建实例对象
        /// </summary>
		public Skeleton()
        {
            mInner = DllImportAPI.V3DSkeleton_New();
        }
        /// <summary>
        /// 析构函数，删除对象，释放指针
        /// </summary>
		~Skeleton()
        {
            DllImportAPI.V3DSkeleton_Release(mInner);
        }
        /// <summary>
        /// 删除骨骼里所有的骨头对象
        /// </summary>
		public void Cleanup()
        {
            mBones.Clear();
        }
        /// <summary>
        /// 骨骼混合
        /// </summary>
        /// <param name="pState">骨骼状态指针</param>
        public void Merge(IntPtr pState)
        {
            DllImportAPI.V3DSkeleton_Merge(mInner, pState);
        }
        /// <summary>
        /// 为骨骼创建分层管理
        /// </summary>
        public void BuildHiberarchys()
        {
            DllImportAPI.V3DSkeleton_BuildHiberarchys(mInner);
        }
        /// <summary>
        /// 计算骨骼对象的包围盒
        /// </summary>
        public void CalcBoundingBox()
        {
            DllImportAPI.V3DSkeleton_CalcBoundingBox(mInner);
        }
        /// <summary>
        /// 只读属性，骨骼的最小顶点坐标
        /// </summary>
        public SlimDX.Vector3 vMin
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 temp = new SlimDX.Vector3();
                    DllImportAPI.V3DSkeleton_GetBoundingBoxMin(mInner, &temp);
                    return temp;
                }
            }
        }
        /// <summary>
        /// 只读属性，骨骼的最大顶点坐标
        /// </summary>
        public SlimDX.Vector3 vMax
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 temp = new SlimDX.Vector3();
                    DllImportAPI.V3DSkeleton_GetBoundingBoxMax(mInner, &temp);
                    return temp;
                }
            }
        }
        /// <summary>
        /// 获取骨骼的数量
        /// </summary>
        /// <returns>返回骨骼中骨头的数量</returns>
        public int GetBoneCount()
        {
            return DllImportAPI.V3DSkeleton_GetBoneCount(mInner);
        }
        /// <summary>
        /// 根据索引获取骨头对象
        /// </summary>
        /// <param name="i">索引值</param>
        /// <returns>返回相应的骨头对象</returns>
		public Bone GetBone(int i)
        {
            if(i>=mBones.Count)
			    return null;
		    return mBones[i];
        }
        /// <summary>
        /// 根据名称获取骨头对象
        /// </summary>
        /// <param name="name">骨头的名称</param>
        /// <returns>返回相应的骨头对象</returns>
        public Bone GetBone(string name)
        {
            for (int i = 0; i < mBones.Count; ++i)
            {
                if (mBones[i].Name == name)
                    return mBones[i];
            }
            return null;
        }
        /// <summary>
        /// 获取相应骨头的索引值
        /// </summary>
        /// <param name="name">骨头的名称</param>
        /// <returns>返回相应骨头的索引值</returns>
        public int GetBoneIndex(string name)
        {
            for (int i = 0; i < mBones.Count; ++i)
            {
                if (mBones[i].Name == name)
                    return i;
            }
            return 0;
        }
        /// <summary>
        /// 获取根骨头对象
        /// </summary>
        /// <returns>返回根部的骨头对象列表</returns>
        public List<Bone> GetRootBones()
        {
		    List<Bone> retRoots = new List<Bone>();

		    for(int i = 0; i < DllImportAPI.V3DSkeleton_GetRootBoneCount(mInner); ++i)
		    {
                retRoots.Add(mBones[DllImportAPI.V3DSkeleton_GetRootBoneIndex(mInner,i)]);
		    }

		    return retRoots;
        }

    }

}
