using System;

namespace CCore
{
    /// <summary>
    /// BSP类
    /// </summary>
    public class IBSP
    {
        /// <summary>
        /// BSPSpace对象指针
        /// </summary>
        protected IntPtr mBSPSpace = IntPtr.Zero; // model3::v3dBspSpace*
        /// <summary>
        /// 只读属性，BSPSpace对象指针
        /// </summary>
        public IntPtr BSPSpace
        {
            get { return mBSPSpace; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bsp">BSP对象指针</param>
        public IBSP(IntPtr bsp)
        {
            if (mBSPSpace != IntPtr.Zero)
            {
                DllImportAPI.v3dBspSpace_Release(mBSPSpace);
            }
            DllImportAPI.v3dBspSpace_AddRef(bsp);
            mBSPSpace = bsp;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~IBSP()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public void Cleanup()
        {
            if (mBSPSpace != IntPtr.Zero)
            {
                DllImportAPI.v3dBspSpace_Release(mBSPSpace);
                mBSPSpace = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 保存对象数据到XND文件
        /// </summary>
        /// <param name="node">XND文件节点</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool Save(CSUtility.Support.XndNode node)
        {
            return DllImportAPI.v3dBspSpace_Save(mBSPSpace, node.GetRawNode()) != 0;
        }
        /// <summary>
        /// 从XND文件加载BSP
        /// </summary>
        /// <param name="node">XND文件节点</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public bool Load(CSUtility.Support.XndNode node)
        {
            return DllImportAPI.v3dBspSpace_Load(mBSPSpace, node.GetRawNode()) != 0;
        }
        /// <summary>
        /// BSP消逝
        /// </summary>
        /// <param name="mesh">mesh对象指针</param>
        /// <param name="epsilon">消逝持续时间</param>
        /// <returns>成功返回true，否则返回false</returns>
		public bool BSPSplit(IntPtr mesh, float epsilon)
        {
            unsafe
            {
                return (DllImportAPI.v3dBspSpace_Split(mBSPSpace, mesh, epsilon)!=0)? true : false;
            }
        }
        /// <summary>
        /// 连线检查
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="matrix">对象的位置矩阵</param>
        /// <param name="result">检查的结果</param>
        /// <returns>检查成功返回true，否则返回false</returns>
        public bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref SlimDX.Matrix matrix, ref CSUtility.Support.stHitResult result)
        {
            unsafe
            {
                fixed (SlimDX.Vector3* pinStart = &start)
                {
                    fixed (SlimDX.Vector3* pinEnd = &end)
                    {
                        fixed (SlimDX.Matrix* pinMatrix = &matrix)
                        {
                            fixed(CSUtility.Support.stHitResult* pResult = &result)
                            {
                                var invMatrix = matrix;
                                invMatrix.Invert();
                                //var invMatrix = SlimDX.Matrix.Invert(matrix);
                                //SlimDX.Matrix invMatrix;
                                //SlimDX.Matrix.Invert(ref matrix, out invMatrix);
                                //SlimDX.Vector3 hitPos;// = SlimDX.Vector3.Zero;
                                //SlimDX.Vector3 hitNormal;// = SlimDX.Vector3.Zero;

                                var retValue = DllImportAPI.v3dBspSpace_QueryRayIntersect(mBSPSpace, pinStart, pinEnd, pinMatrix, &invMatrix, pResult);
                                if (retValue != 0)
                                {
                                    //result.mHitPosition = hitResult;
                                    //result.mHitNormal = hitNormal;
                                    //result.mHitLength = SlimDX.Vector3.Subtract(hitPos, start).Length();
                                    return true;
                                }
                                else
                                {
                                    //System.Diagnostics.Trace.WriteLine("LineCheck RetValue = false");
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            //return false;
        }
    }
}
