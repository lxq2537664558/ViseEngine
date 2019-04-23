using System;
using System.ComponentModel;

namespace CCore.Component
{
    /// <summary>
    /// 贴花类，可以进行拖动种植
    /// </summary>
    [CCore.EditorAssist.PlantAbleAttribute("贴花.贴花", "", "")]
    public class Decal : Visual, EditorAssist.IPlantAbleObject
    {
        #region EditAssist
        /// <summary>
        /// 得到种植的贴花（Actor类型）
        /// </summary>
        /// <param name="world">种植贴花的世界</param>
        /// <returns>返回种植的贴花</returns>
        public CCore.World.Actor GetPlantActor(CCore.World.World world)
        {
            var decalActor = new CCore.World.DecalActor();
            var decalActorInit = new CCore.World.DecalActorInit();
            decalActorInit.ActorFlag |= CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient;
            decalActor.Initialize(decalActorInit);
            decalActor.SetPlacement(new CSUtility.Component.StandardPlacement(decalActor));

            if (mPropertyObject != null)
            {
                decalActor = mPropertyObject.Duplicate() as CCore.World.DecalActor;
            }

            decalActor.ActorName = "贴花" + Program.GetActorIndex();
            return decalActor;
        }
        /// <summary>
        /// 拖动是显示的Actor图标
        /// </summary>
        /// <param name="world">种植贴花的世界</param>
        /// <returns>返回拖动显示的Actor</returns>
        public CCore.World.Actor GetPreviewActor(CCore.World.World world)
        {
            if (!CCore.Program.IsActorTypeShow(world, CCore.Program.DecalAssistTypeName))
                CCore.Program.SetActorTypeShow(world, CCore.Program.DecalAssistTypeName, true);
            return GetPlantActor(world);
        }

        CCore.World.DecalActor mPropertyObject = null;
        /// <summary>
        /// 得到显示的Actor的特性
        /// </summary>
        /// <returns>需要显示的贴花的类</returns>
        public object GetPropertyShowObject()
        {
            var decalActor = new CCore.World.DecalActor();
            var decalActorInit = new CCore.World.DecalActorInit();
            decalActor.Initialize(decalActorInit);
            mPropertyObject = decalActor;
            return mPropertyObject;
        }

        #endregion
        /// <summary>
        /// 贴花的地址指针
        /// </summary>
        protected IntPtr mDecalPtr;    //vSimulation::vDecalProxy
        /// <summary>
        /// 贴花的地址指针
        /// </summary>
        public IntPtr DecalPtr
        {
            get { return mDecalPtr; }
        }
        /// <summary>
        /// 设置该贴花是否可点击
        /// </summary>
        public bool CanHitProxy
        {
            get
            {
                unsafe
                {
                    var retValue = DllImportAPI.BoxDecalProxy_GetCanHitProxy(mDecalPtr);
                    return (retValue != 0) ? true : false;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.BoxDecalProxy_SetCanHitProxy(mDecalPtr, value);
                }
            }
        }
        /// <summary>
        /// 是否显示代表光照范围的Mesh，默认不显示
        /// </summary>
        protected bool mShowRangeMesh = false;     	// 是否显示代表光照范围的Mesh
        /// <summary>
        /// 是否显示代表光照范围的Mesh
        /// </summary>
        [Browsable(false)]
        public bool ShowRangeMesh
        {
            get { return mShowRangeMesh; }
            set { mShowRangeMesh = value; }
        }
        /// <summary>
        /// 是否显示代表光源的符号Mesh
        /// </summary>
        protected bool mShowSignMesh = true;      	// 是否显示代表光源的符号Mesh
        /// <summary>
        /// 是否显示代表光源的符号Mesh
        /// </summary>
        [Browsable(false)]
        public bool ShowSignMesh
        {
            get { return mShowSignMesh; }
            set { mShowSignMesh = value; }
        }
        /// <summary>
        /// SignMesh的大小，保证在屏幕上大小相同
        /// </summary>
        protected float m_fSignMeshSize = 1.0f;   	// SignMesh的大小，保证在屏幕上大小相同
        /// <summary>
        /// SignMesh的大小，保证在屏幕上大小相同
        /// </summary>
        [Browsable(false)]
        public float SignMeshSize
        {
            get { return m_fSignMeshSize; }
            set { m_fSignMeshSize = value; }
        }
        /// <summary>
        /// 构造函数，创建贴花的材料等，创建贴花Actor时调用
        /// </summary>
        public Decal()
        {
            mLayer = RLayer.RL_DSDecal;

            unsafe
            {
                mDecalPtr = DllImportAPI.BoxDecalProxy_New(CCore.Engine.Instance.Client.Graphics.Device);

                var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultDecalTechniqueId);
                DllImportAPI.BoxDecalProxy_DSDecalMeshSetMaterial(mDecalPtr, mtl.MaterialPtr);

                var mtl1 = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultDecalEditorSignMeshTechniqueId);
                var mtl2 = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultDecalEditorRangeMeshTechniqueId);
                DllImportAPI.BoxDecalProxy_EditorDecalMeshSetMaterial(mDecalPtr, 0, mtl1.MaterialPtr);
                DllImportAPI.BoxDecalProxy_EditorDecalMeshSetMaterial(mDecalPtr, 1, mtl2.MaterialPtr);
            }
        }
        /// <summary>
        /// 析构函数，释放内存
        /// </summary>
        ~Decal()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放贴花内存
        /// </summary>
        public override void Cleanup()
        {
            unsafe
            {
                if (mDecalPtr != IntPtr.Zero)
                {
                    DllImportAPI.BoxDecalProxy_Release(mDecalPtr);
                    mDecalPtr = IntPtr.Zero;
                }
            }
        }
        /// <summary>
        /// 将贴花提交给World，以供编辑绘制
        /// </summary>
        /// <param name="renderEnv">放置贴花的环境</param>
        /// <param name="matrix">贴花Actor的Placement矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            unsafe
            {
                var time = CCore.Engine.Instance.GetFrameSecondTime();
                var transMat = matrix;

                if (Visible)
                {
                    DllImportAPI.vDSRenderEnv_CommitDSDecal(renderEnv.DSRenderEnv, (int)mGroup, mDecalPtr, &transMat, eye.CommitCamera);
                }

                if (Engine.Instance.IsEditorMode == true)
                {
                    // 提交编辑用RangeMesh
                    if (ShowRangeMesh)
                    {
                        DllImportAPI.BoxDecalProxy_CommitRangeMesh(mDecalPtr, renderEnv.DSRenderEnv, (int)mGroup, &transMat);
                    }

                    if (ShowSignMesh)
                    {
                        DllImportAPI.BoxDecalProxy_CommitSignMesh(mDecalPtr, renderEnv.DSRenderEnv, (int)mGroup, SignMeshSize, CanHitProxy, &transMat);
                    }
                }
            }
        }
        /// <summary>
        /// 设置贴花材质
        /// </summary>
        /// <param name="mtl">需要设置的材质</param>
        public void SetMaterial(CCore.Material.Material mtl)
        {
            if (mtl == null)
                return;

            unsafe
            {
                DllImportAPI.BoxDecalProxy_DSDecalMeshSetMaterial(mDecalPtr, mtl.MaterialPtr);
            }
        }
        /// <summary>
        /// 设置贴花为可点击
        /// </summary>
        /// <param name="hitProxy">点击代理值</param>
        public override void SetHitProxyAll(uint hitProxy)
        {
            unsafe
            {
                DllImportAPI.BoxDecalProxy_SetHitProxy(mDecalPtr, 0, hitProxy);
            }
        }
        /// <summary>
        /// 得到Actor的AABB包围盒
        /// </summary>
        /// <param name="vMin">Actor顶点的最小值</param>
        /// <param name="vMax">Actor顶点的最大值</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            vMin = - SlimDX.Vector3.UnitXYZ * 0.5f;
		    vMax = SlimDX.Vector3.UnitXYZ * 0.5f;
        }
    }
}
