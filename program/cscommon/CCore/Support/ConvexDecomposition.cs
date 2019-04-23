using System;

namespace CCore.Support
{
    /// <summary>
    /// 分解凸面类
    /// </summary>
    public class vIConvexDecomposition
    {
        /// <summary>
        /// 该对象的指针
        /// </summary>
        protected IntPtr mConvexDecompositionPtr = IntPtr.Zero;
        /// <summary>
        /// 构造函数，创建实例对象
        /// </summary>
        public vIConvexDecomposition()
        {
            unsafe
            {
                mConvexDecompositionPtr = DllImportAPI.vfxConvexDecomposition_New();
            }
        }
        /// <summary>
        /// 析构函数，删除对象
        /// </summary>
        ~vIConvexDecomposition()
        {
            unsafe
            {
                DllImportAPI.vfxConvexDecomposition_Delete(mConvexDecompositionPtr);
            }
        }
        /// <summary>
        /// 设置凸面数据
        /// </summary>
        /// <param name="depth">深度</param>
        /// <param name="cpercent">C的百分比</param>
        /// <param name="ppercent">P的百分比</param>
        /// <param name="maxVertices">最大顶点数量</param>
        /// <param name="skinWidth">皮肤宽度</param>
        public void SetConvexData(uint depth, double cpercent, double ppercent, uint maxVertices, double skinWidth)
        {
            unsafe
            {
                DllImportAPI.vfxConvexDecomposition_SetConvexData(mConvexDecompositionPtr, depth, cpercent, ppercent, maxVertices, skinWidth);
            }
        }
        /// <summary>
        /// 进行凸面分解
        /// </summary>
        /// <param name="outMesh">凸出来的mesh指针</param>
        /// <param name="mesh">需要分解的mesh指针</param>
        /// <returns>分解成功返回true，否则返回false</returns>
        public bool performConvexDecomposition(IntPtr outMesh, IntPtr mesh)
        {
            unsafe
            {
                var mtl = Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultSimplateMeshTechniqueId);
                var retValue = DllImportAPI.vfxConvexDecomposition_performConvexDecomposition(mConvexDecompositionPtr, Engine.Instance.Client.Graphics.Device, outMesh, mesh, mtl.MaterialPtr);
                return (retValue != 0) ? true : false;
            }
        }
        /// <summary>
        /// 分解箱体
        /// </summary>
        /// <param name="outMesh">凸出来的mesh指针</param>
        /// <param name="mesh">需要分解的mesh指针</param>
        /// <returns>分解成功返回true，否则返回false</returns>
        public bool performBoxDecomposition(IntPtr outMesh, IntPtr mesh)
        {
            unsafe
            {
                var mtl = Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultSimplateMeshTechniqueId);
                var retValue = DllImportAPI.vfxConvexDecomposition_performBoxDecomposition(mConvexDecompositionPtr, Engine.Instance.Client.Graphics.Device, outMesh, mesh, mtl.MaterialPtr);
                return (retValue != 0) ? true : false;
            }
        }
        /// <summary>
        /// 分解球体
        /// </summary>
        /// <param name="outMesh">凸出来的mesh指针</param>
        /// <param name="mesh">需要分解的mesh指针</param>
        /// <returns>分解成功返回true，否则返回false</returns>
        public bool performSphereDecomposition(IntPtr outMesh, IntPtr mesh)
        {
            unsafe
            {
                var mtl = Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultSimplateMeshTechniqueId);
                var retValue = DllImportAPI.vfxConvexDecomposition_performSphereDecomposition(mConvexDecompositionPtr, Engine.Instance.Client.Graphics.Device, outMesh, mesh, mtl.MaterialPtr);
                return (retValue != 0) ? true : false;
            }
        }
        /// <summary>
        /// 分解圆柱体
        /// </summary>
        /// <param name="outMesh">凸出来的mesh指针</param>
        /// <param name="mesh">需要分解的mesh指针</param>
        /// <returns>分解成功返回true，否则返回false</returns>
        public bool performCylinderDecomposition(IntPtr outMesh, IntPtr mesh)
        {
            unsafe
            {
                var mtl = Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultSimplateMeshTechniqueId);
                var retValue = DllImportAPI.vfxConvexDecomposition_performCylinderDecomposition(mConvexDecompositionPtr, Engine.Instance.Client.Graphics.Device, outMesh, mesh, mtl.MaterialPtr);
                return (retValue != 0) ? true : false;
            }
        }

    }
}
