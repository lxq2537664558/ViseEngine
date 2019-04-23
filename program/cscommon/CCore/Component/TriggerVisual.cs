using System;

namespace CCore.Component
{

    /// <summary>
    /// 可视化触发器
    /// </summary>
    public class TriggerVisual : Visual
    {
        private IntPtr m_pMeshProxy;
        /// <summary>
        /// 可视化触发器的构造函数
        /// </summary>
        public TriggerVisual()
        {
            unsafe
            {
                mLayer = RLayer.RL_SystemHelper;
                var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultTriggerTechniqueId);
                m_pMeshProxy = DllImportAPI.v3dStagedObject_New();
                var mat = SlimDX.Matrix.Identity;
                var ms = DllImportAPI.v3dModelCooking_CookBox(CCore.Engine.Instance.Client.Graphics.Device, &mat, 1, 1, 1, 0, (int)CCore.RenderAPI.V3DPOOL.V3DPOOL_MANAGED);
                if (DllImportAPI.v3dStagedObject_CreateModel(m_pMeshProxy, CCore.Engine.Instance.Client.Graphics.Device, ms) != 0)
                {
                    DllImportAPI.v3dStagedObject_SetMaterial(m_pMeshProxy, 0, mtl.MaterialPtr, IntPtr.Zero);
                    DllImportAPI.v3dStagedObject_SetMaterial(m_pMeshProxy, 1, mtl.MaterialPtr, IntPtr.Zero);
                }
                DllImportAPI.v3dModelSource_Release(ms);
            }
        }
        /// <summary>
        /// 析构函数，是否实例内存
        /// </summary>
        ~TriggerVisual()
        {
            Cleanup();
        }
        /// <summary>
        /// 可视化触发器的删除并释放该实例内存
        /// </summary>
        public override void Cleanup()
        {
            unsafe
            {
                if (m_pMeshProxy != IntPtr.Zero)
                {
                    DllImportAPI.v3dStagedObject_Release(m_pMeshProxy);
                    m_pMeshProxy = IntPtr.Zero;
                }
            }
        }
        /// <summary>
        /// 将该触发器提交到渲染场景中
        /// </summary>
        /// <param name="renderEnv">渲染场景</param>
        /// <param name="matrix">触发器的矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            unsafe
            {
                if (!Visible)
                    return;

                if (!CCore.Program.IsActorTypeShow(HostActor.World, "触发器辅助"))
                    return;

                if (m_pMeshProxy != IntPtr.Zero)
                {
                    if (DllImportAPI.v3dStagedObject_GetModelSourceRenderAtomNumber(m_pMeshProxy) > 1)
                    {
                        fixed (SlimDX.Matrix* pinMatrix = &matrix)
                        {
                            DllImportAPI.vDSRenderEnv_CommitHelperMesh(renderEnv.DSRenderEnv, (int)mGroup, m_pMeshProxy, pinMatrix);
                        }
                    }

                    fixed (SlimDX.Matrix* pinMatrix = &matrix)
                        DllImportAPI.vDSRenderEnv_CommitHitProxy(renderEnv.DSRenderEnv, (int)mGroup, m_pMeshProxy, pinMatrix);
                }
            }
        }
        /// <summary>
        /// 设置触发器是否为可以用鼠标点击
        /// </summary>
        /// <param name="hitProxy">点击代理</param>
        public override void SetHitProxyAll(uint hitProxy)
        {
            unsafe
            {
                if (m_pMeshProxy != IntPtr.Zero)
                {
                    DllImportAPI.v3dStagedObject_SetHitProxy(m_pMeshProxy, 0, hitProxy);
                }
            }
        }
        /// <summary>
        /// 设置触发器的AABB包围盒
        /// </summary>
        /// <param name="vMin">包围盒的最小顶点</param>
        /// <param name="vMax">最大顶点</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
		    vMin = - SlimDX.Vector3.UnitXYZ * 0.5f;
		    vMax = SlimDX.Vector3.UnitXYZ * 0.5f;
        }
    }
}
