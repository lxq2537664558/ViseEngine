using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Mesh
{
    /// <summary>
    /// mesh的淡入淡出类型
    /// </summary>
    public enum MeshFadeType
    {
        None,
        FadeInOut,
        FadeIn,
        FadeOut,
    }
    /// <summary>
    /// 边缘检测模式
    /// </summary>
    public enum EdgeDetectMode
    {
        Outline,
        OutlineHighlight,
        Highlight,
        Solid,
        Rimlight,
    };
    /// <summary>
    /// 创建来源
    /// </summary>
    public enum enCreateFrom
    {
        ECF_Export,
        ECF_Editor,
    }

    // 与底层VModelDesc保持一致
    /// <summary>
    /// mesh信息参数结构体
    /// </summary>
    public struct MeshDesc
    {
        /// <summary>
        /// mesh标记
        /// </summary>
        public UInt32 Flags;
        /// <summary>
        /// 没有使用过
        /// </summary>
        public UInt32 UnUsed;
        /// <summary>
        /// 顶点数量
        /// </summary>
        public UInt32 VertexNumber;
        /// <summary>
        /// GeoTabe的数量
        /// </summary>
        public UInt32 GeoTabeNumber;
        /// <summary>
        /// 多边形的数量
        /// </summary>
        public UInt32 PolyNumber;
        /// <summary>
        /// 原子的数量
        /// </summary>
        public UInt32 AtomNumber;
    }
    /// <summary>
    /// meshpart的初始化类
    /// </summary>
    public class MeshInitPart : CSUtility.Support.XndSaveLoadProxy
    {
        string m_meshName = "";
        /// <summary>
        /// mesh名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MeshName")]
        [CSUtility.Support.AutoSaveLoad]
        public string MeshName
        {
            get { return m_meshName; }
            set { m_meshName = value; }
        }

        List<Guid> m_Techs = new List<Guid>();
        /// <summary>
        /// mesh的technique列表
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Techs")]
        [CSUtility.Support.AutoSaveLoad]
        public List<Guid> Techs
        {
            get { return m_Techs; }
            set { m_Techs = value; }
        }

        int mSelectRenderAtomIdx = -1;
        /// <summary>
        /// 选择渲染原子的索引
        /// </summary>
        public int SelectRenderAtomIdx
        {
            get { return mSelectRenderAtomIdx; }
            set { mSelectRenderAtomIdx = value; }
        }

        List<Guid> m_BakTechs = new List<Guid>();
        /// <summary>
        /// mesh的technique列表
        /// </summary>
        public List<Guid> BakTechs
        {
            get { return m_BakTechs; }
            set { m_BakTechs = value; }
        }

        Guid mOwnerMeshId = System.Guid.Empty;
        /// <summary>
        /// 唯一的meshID
        /// </summary>
        public Guid OwnerMeshId
        {
            get { return mOwnerMeshId; }
            set { mOwnerMeshId = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public MeshInitPart()
        {

        }
    }
    /// <summary>
    /// mesh的初始化类
    /// </summary>
    public class MeshInit : CCore.Component.VisualInit
    {
        bool mCanHitProxy = true;
        /// <summary>
        /// 可以进行点选
        /// </summary>
		[CSUtility.Support.AutoSaveLoadAttribute]
        public bool CanHitProxy
        {
            get { return mCanHitProxy; }
            set { mCanHitProxy = value; }
        }

        bool mBlockNavigation = true;
        /// <summary>
        /// 是否为块
        /// </summary>
		[CSUtility.Support.AutoSaveLoadAttribute]
        public bool BlockNavigation
        {
            get { return mBlockNavigation; }
            set { mBlockNavigation = value; }
        }
        /// <summary>
        /// meshpart的列表
        /// </summary>
        public List<MeshInitPart> MeshInitParts = new List<MeshInitPart>();
        /// <summary>
        /// 是否需要组装骨骼
        /// </summary>
        public bool m_bNeedCalcFullSkeleton;
        /// <summary>
        /// 当前动作的名称
        /// </summary>
        public string CurActionName;
        /// <summary>
        /// 播放速度
        /// </summary>
        public float mPlayRate;
        /// <summary>
        /// 蒙皮颜色的透明度
        /// </summary>
        public float mMaskColorOpacity = 0.0f;
        //public SlimDX.Vector3 mMaskColor = SlimDX.Vector3.UnitXYZ;
        /// <summary>
        /// 是否有阴影
        /// </summary>
        public bool CastShadow = true;
        /// <summary>
        /// mesh的渐隐类型，
        /// </summary>
        public MeshFadeType MeshFadeType = MeshFadeType.FadeInOut;
        /// <summary>
        /// 渐显时间，默认为500ms
        /// </summary>
        public UInt32 mFadeInTime = 500;
        /// <summary>
        /// 渐隐时间，默认为500ms
        /// </summary>
        public UInt32 mFadeOutTime = 500;
        //public bool mEnableTrail = false;
        /// <summary>
        /// 是否每帧调用
        /// </summary>
        public bool mTickAllTime = false;
        /// <summary>
        /// 是否强制从磁盘加载
        /// 
        /// </summary>
        public bool ForceLoad = false;
        /// <summary>
        /// mesh模板
        /// </summary>
        protected MeshTemplate m_meshTemplate;
        /// <summary>
        /// 只读属性，mesh模板
        /// </summary>
        public MeshTemplate MeshTemplate
        {
            get { return m_meshTemplate; }
        }
        private System.Guid m_meshTemplateID;
        /// <summary>
        /// mesh模板ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public System.Guid MeshTemplateID
        {
            get { return m_meshTemplateID; }
            set
            {
                m_meshTemplateID = value;
                m_meshTemplate = MeshTemplateMgr.Instance.FindMeshTemplate(m_meshTemplateID);

                if (m_meshTemplate == null)
                {
                    MeshInitParts.Clear();
                    return;
                }

                MeshInitParts.Clear();
                MeshInitParts.AddRange(m_meshTemplate.MeshInitList);

                m_bNeedCalcFullSkeleton = m_meshTemplate.NeedCalcFullSkeleton;
                CurActionName = m_meshTemplate.ActionName;
                //mLoop = m_meshTemplate.Loop;
                mPlayRate = (float)m_meshTemplate.PlayRate;
                //mPause = m_meshTemplate.Pause;
                //mMaskColorOpacity = (float)m_meshTemplate.MaskColorOpacity;
                //var tV = new SlimDX.Vector4();
                //CSUtility.Program.DrawColor2Vector(m_meshTemplate.MaskColor, out tV);
                //mMaskColor.X = tV.X;
                //mMaskColor.Y = tV.Y;
                //mMaskColor.Z = tV.Z;
                CastShadow = m_meshTemplate.CastShadow;
                mFadeInTime = m_meshTemplate.FadeInTime;
                mFadeOutTime = m_meshTemplate.FadeOutTime;
                MeshFadeType = m_meshTemplate.MeshFadeType;
                //mEnableTrail = m_meshTemplate.EnableTrail;
                mTickAllTime = m_meshTemplate.TickAllTime;
            }
        }
        /// <summary>
        /// mesh初始化类的构造函数
        /// </summary>
        public MeshInit()
        {

        }
        /// <summary>
        /// 设置meshPart
        /// </summary>
        /// <param name="parts">meshpart列表</param>
        public void SetMeshParts(List<MeshInitPart> parts)
        {
            MeshInitParts.AddRange(parts);
        }
    }
    /// <summary>
    /// meshPart类
    /// </summary>
    public class MeshPart
    {
        MeshInitPart mMeshPartInit = null;
        /// <summary>
        /// 只读属性，meshPart的初始化类
        /// </summary>
        public MeshInitPart MeshPartInit
        {
            get { return mMeshPartInit; }
        }

        IntPtr mMesh = IntPtr.Zero;              // model3::v3dStagedObject
        /// <summary>
        /// mesh的对象指针
        /// </summary>
        public IntPtr Mesh
        {
            get { return mMesh; }
        }

        IntPtr mPhysicShape = IntPtr.Zero;
        /// <summary>
        /// 只读属性，物理形状的指针
        /// </summary>
        public IntPtr PhysicShape
        {
            get { return mPhysicShape; }
        }

        IntPtr mSimplifyMesh = IntPtr.Zero;      // model3::v3dStagedObject
        /// <summary>
        /// 只读属性，精简mesh的指针
        /// </summary>
        public IntPtr SimplifyMesh
        {
            get { return mSimplifyMesh; }
        }
        IntPtr mPathMesh = IntPtr.Zero;          // model3::v3dStagedObject
        /// <summary>
        /// 只读属性，mesh的寻路网格指针
        /// </summary>
        public IntPtr PathMesh
        {
            get { return mPathMesh; }
        }
        IBSP mBSPOperator;
        /// <summary>
        /// 只读属性，BSPOperator
        /// </summary>
        public IBSP BSPOperator
        {
            get { return mBSPOperator; }
        }
        CCore.Socket.SocketTable mSocketTable;
        /// <summary>
        /// 只读属性，挂接面板
        /// </summary>
        public CCore.Socket.SocketTable SocketTable
        {
            get { return mSocketTable; }
        }

        IntPtr mTrailModifier = IntPtr.Zero;
        /// <summary>
        /// 只读属性，拖尾模拟器
        /// </summary>
        public IntPtr TrailModifier
        {
            get { return mTrailModifier; }
        }

        UInt32 mReplaceMeshVersion = 0;
        UInt32 mReplaceSimplifyMeshVersion = 0;
        
        Material.Material mMaterial = null;
        /// <summary>
        /// 只读属性，mesh的材质
        /// </summary>
        public Material.Material Material
        {
            get { return mMaterial; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public MeshPart()
        {

        }
        /// <summary>
        /// 析构函数，删除对象，释放指针内存
        /// </summary>
        ~MeshPart()
        {
            Cleanup();
        }
        /// <summary>
        /// 更新资源
        /// </summary>
        /// <returns>是否进行了更新，资源更新返回true，否则返回false</returns>
        public bool UpdateReplaceSource()
        {
            bool replaced = false;
            var sourceMeshReplaceVer = DllImportAPI.v3dStagedObject_GetReplaceVersion(mMesh);
            if (sourceMeshReplaceVer != mReplaceMeshVersion)
            {
                mReplaceMeshVersion = sourceMeshReplaceVer;
                DllImportAPI.v3dStagedObject_OnModelSourceReplaced(mMesh);
                replaced = true;
            }

            var simMeshReplaceVer = DllImportAPI.v3dStagedObject_GetReplaceVersion(mSimplifyMesh);
            if (simMeshReplaceVer != mReplaceSimplifyMeshVersion)
            {
                mReplaceSimplifyMeshVersion = simMeshReplaceVer;
                DllImportAPI.v3dStagedObject_OnModelSourceReplaced(mSimplifyMesh);
            }

            return replaced;
        }
        /// <summary>
        /// 清除精简mesh，释放指针内存
        /// </summary>
        public void ClearSimplifyMesh()
        {
            if (mSimplifyMesh != IntPtr.Zero)
            {
                DllImportAPI.v3dStagedObject_Release(mSimplifyMesh);
                mSimplifyMesh = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 删除物理形状的指针对象
        /// </summary>
        public void ClearPhysicShape()
        {
            if (mPhysicShape != IntPtr.Zero)
            {
                DllImportAPI.vPhysXShape_Release(mPhysicShape);
                mPhysicShape = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 删除对象，释放所有申请的内存指针
        /// </summary>
        public void Cleanup()
        {
            unsafe
            {
                if (mMesh != IntPtr.Zero)
                {
                    DllImportAPI.v3dStagedObject_Release(mMesh);
                    mMesh = IntPtr.Zero;
                }
                if (mSimplifyMesh != IntPtr.Zero)
                {
                    DllImportAPI.v3dStagedObject_Release(mSimplifyMesh);
                    mSimplifyMesh = IntPtr.Zero;
                }
                if (mPathMesh != IntPtr.Zero)
                {
                    DllImportAPI.v3dStagedObject_Release(mPathMesh);
                    mPathMesh = IntPtr.Zero;
                }
                if (mPhysicShape != IntPtr.Zero)
                {
                    DllImportAPI.vPhysXShape_Release(mPhysicShape);
                    mPhysicShape = IntPtr.Zero;
                }
            }
            //mTrailModifier = IntPtr.Zero;
            if (mBSPOperator!=null)
            {
                mBSPOperator.Cleanup();
            }
            mBSPOperator = null;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="tm">每帧之间的间隔时间</param>
        public void Tick(Int64 tm)
        {
            unsafe
            {
                if (mMesh != IntPtr.Zero)
                {
                    DllImportAPI.v3dStagedObject_ModStacks_UpdateTick(mMesh, tm);
                }
            }
        }
        /// <summary>
        /// 创建精简mesh
        /// </summary>
        /// <param name="ecf">创建来源的枚举</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public bool IsCreateSimplifyMesh(enCreateFrom ecf)
        {
            //if (mSimplifyMesh != IntPtr.Zero)
            //{
            //    IDllImportAPI.v3dStagedObject_Release(mSimplifyMesh);
            //    mSimplifyMesh = IntPtr.Zero;
            //}
            return (DllImportAPI.v3dStagedObject_ModelSource_IsCreateFrom(mSimplifyMesh, (int)(ecf)) != 0) ? true : false;
        }
        /// <summary>
        /// 加载mesh
        /// </summary>
        /// <param name="partInit">meshPart的初始化类</param>
        /// <param name="ms">指针</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public bool LoadMesh(MeshInitPart partInit, IntPtr ms)
        {
            mMeshPartInit = partInit;

            if (mMesh != IntPtr.Zero)
            {
                DllImportAPI.v3dStagedObject_Release(mMesh);
                mMesh = IntPtr.Zero;
            }

            mMesh = DllImportAPI.v3dStagedObject_New();
            if (DllImportAPI.v3dStagedObject_CreateModel(mMesh, Engine.Instance.Client.Graphics.Device, ms) == 0)
            {
                DllImportAPI.v3dStagedObject_Release(mMesh);
                //UpdateBoundingBox();
                return false;
            }

            var defMtl = Engine.Instance.Client.Graphics.MaterialMgr.GetDefaultMaterial();
            if (defMtl != null)
            {
                int matCount = (int)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(mMesh);
                //if (partInit.Techs == null || partInit.Techs.Count < matCount)
                //{
                //    for (int i = 0; i < matCount; ++i)
                //    {
                //        IDllImportAPI.v3dStagedObject_SetMaterial(mMesh, (uint)i, defMtl.Material, IntPtr.Zero);
                //    }
                //}
                //else
                {
                    int minCount = System.Math.Min(matCount, partInit.Techs.Count);
                    for (int i = 0; i < minCount; ++i)
                    {
                        var mtl = Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(partInit.Techs[i]);
                        if (mtl != null)
                        {
                            DllImportAPI.v3dStagedObject_SetMaterial(mMesh, (uint)i, mtl.MaterialPtr, IntPtr.Zero);
                            mMaterial = mtl;
                        }
                        else
                        {
                            DllImportAPI.v3dStagedObject_SetMaterial(mMesh, (uint)i, defMtl.MaterialPtr, IntPtr.Zero);
                            mMaterial = defMtl;
                        }
                    }
                    for (int i = minCount; i < matCount; ++i)
                    {
                        DllImportAPI.v3dStagedObject_SetMaterial(mMesh, (uint)i, defMtl.MaterialPtr, IntPtr.Zero);
                        mMaterial = defMtl;
                    }
                }
            }

            mTrailModifier = DllImportAPI.v3dStagedObject_QueryModifier(mMesh, vIIDDefine.vIID_v3dTrailModifier);
            return true;
        }
        /// <summary>
        /// 加载精简mesh
        /// </summary>
        /// <param name="simMs">精简mesh的指针</param>
        public void LoadSimplifyMesh(IntPtr simMs)
        {
            if (mSimplifyMesh != IntPtr.Zero)
            {
                DllImportAPI.v3dStagedObject_Release(mSimplifyMesh);
                mSimplifyMesh = IntPtr.Zero;
            }

            mSimplifyMesh = DllImportAPI.v3dStagedObject_New();
            if (DllImportAPI.v3dStagedObject_CreateModel(mSimplifyMesh, Engine.Instance.Client.Graphics.Device, simMs) == 0)
            {
                DllImportAPI.v3dStagedObject_Release(mSimplifyMesh);
            }
            else
            {
                var simMtl = Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultSimplateMeshTechniqueId);
                if (simMtl != null)
                {
                    var matCount = DllImportAPI.v3dStagedObject_GetMaxMaterialNum(mSimplifyMesh);
                    for (UInt64 i = 0; i < matCount; ++i)
                    {
                        DllImportAPI.v3dStagedObject_SetMaterial(mSimplifyMesh, (uint)i, simMtl.MaterialPtr, IntPtr.Zero);
                    }
                }
            }

            DllImportAPI.v3dStagedObject_PreUse(mSimplifyMesh, false, Engine.Instance.GetFrameMillisecond());
        }
        /// <summary>
        /// 设置BSP
        /// </summary>
        /// <param name="bsp">BSP对象指针</param>
        public void SetBSP(IntPtr bsp)
        {
            if (mBSPOperator != null)
                mBSPOperator.Cleanup();
            mBSPOperator = new IBSP(bsp);
        }
        /// <summary>
        /// 加载mesh寻路路径
        /// </summary>
        /// <param name="pathMs">mesh寻路的指针</param>
        public void LoadPathMesh(IntPtr pathMs)
        {
            if (mPathMesh != IntPtr.Zero)
            {
                DllImportAPI.v3dStagedObject_Release(mPathMesh);
                mPathMesh = IntPtr.Zero;
            }

            mPathMesh = DllImportAPI.v3dStagedObject_New();
            if (DllImportAPI.v3dStagedObject_CreateModel(mPathMesh, Engine.Instance.Client.Graphics.Device, pathMs) == 0)
            {
                DllImportAPI.v3dStagedObject_Release(mPathMesh);
            }
            else
            {
                var pathMtl = Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultPathMeshTechniqueId);
                if (pathMtl != null)
                {
                    var matCount = DllImportAPI.v3dStagedObject_GetMaxMaterialNum(mPathMesh);
                    for (UInt64 i = 0; i < matCount; ++i)
                    {
                        DllImportAPI.v3dStagedObject_SetMaterial(mPathMesh, (uint)i, pathMtl.MaterialPtr, IntPtr.Zero);
                    }
                }
            }
        }
        /// <summary>
        /// 设置挂接件
        /// </summary>
        /// <param name="sockets">挂接件的指针</param>
        public void SetSockets(IntPtr sockets)
        {
            mSocketTable = new CCore.Socket.SocketTable();
            mSocketTable.Inner = sockets;
        }
        /// <summary>
        /// 根据mesh类型创建精简mesh
        /// </summary>
        /// <param name="convexDecomposition">vIConvexDecomposition类</param>
        /// <param name="smt">精简类型的枚举</param>
        public void CreateSimplifyMesh_ByType(CCore.Support.vIConvexDecomposition convexDecomposition, CCore.Mesh.Mesh.enSimpleMeshType smt)
        {
            if (SimplifyMesh == IntPtr.Zero)
                mSimplifyMesh = DllImportAPI.v3dStagedObject_New();

            bool ret = false;
            switch (smt)
            {
                case CCore.Mesh.Mesh.enSimpleMeshType.Box:
                    ret = convexDecomposition.performBoxDecomposition(mSimplifyMesh, mMesh);
                    break;
                case CCore.Mesh.Mesh.enSimpleMeshType.Cylinder:
                    ret = convexDecomposition.performCylinderDecomposition(mSimplifyMesh, mMesh);
                    break;
                case CCore.Mesh.Mesh.enSimpleMeshType.Sphere:
                    ret = convexDecomposition.performSphereDecomposition(mSimplifyMesh, mMesh);
                    break;
                case CCore.Mesh.Mesh.enSimpleMeshType.Param:
                    ret = convexDecomposition.performConvexDecomposition(mSimplifyMesh, mMesh);
                    break;
            }

            if (ret)
            {   
                DllImportAPI.v3dStagedObject_ModelSource_SetCreateFrom(mSimplifyMesh, (int)enCreateFrom.ECF_Editor);

                // 生成BSP
                if (mBSPOperator != null)
                {
                    mBSPOperator.Cleanup();
                }
                //if (mBSPOperator == null)
                //{
                    var bspPtr = DllImportAPI.v3dBspSpace_New();
                    mBSPOperator = new IBSP(bspPtr);
                //}
                mBSPOperator.BSPSplit(mSimplifyMesh, 0.1f);
            }
        }
        /// <summary>
        /// 创建物理形状
        /// </summary>
        public void CreatePhysicShape()
        {
            if (mPhysicShape == IntPtr.Zero)
                mPhysicShape = DllImportAPI.vPhysXShape_New();

            //switch (type)
            //{
            //    case Physics.enPhysicGeometryType.Box:

            //        break;
            //}
        }
    }
    public partial class Mesh : CCore.Component.Visual
    {
        /// <summary>
        /// 声明mesh重新初始化调用的委托事件
        /// </summary>
        /// <param name="mesh">mesh类对象</param>
        public delegate void Delegate_OnMeshReInitialized(Mesh mesh);
        /// <summary>
        /// 定义mesh重新初始化时调用的委托事件
        /// </summary>
        public event Delegate_OnMeshReInitialized OnMeshReInitialized;
        /// <summary>
        /// 主mesh是否可见，默认为true
        /// </summary>
        protected bool mMainMeshVisible = true;
        /// <summary>
        /// 设置和获取主mesh是否可见
        /// </summary>
        public bool MainMeshVisible
        {
            get{ return mMainMeshVisible; }
            set{ mMainMeshVisible = value; }
        }
        /// <summary>
        /// 精简mesh是否可见，默认为true
        /// </summary>
		protected bool mSimplifyMeshVisible = true;
        /// <summary>
        /// 精简的mesh是否可见
        /// </summary>
        public bool SimplifyMeshVisible
        {
            get{ return mSimplifyMeshVisible; }
            set{ mSimplifyMeshVisible = value; }
        }
        /// <summary>
        /// mesh路径是否可见，默认为false
        /// </summary>
		protected bool mPathMeshVisible = false;
        /// <summary>
        /// mesh路径是否可见
        /// </summary>
        public bool PathMeshVisible
        {
            get{return mPathMeshVisible;}
            set{mPathMeshVisible = value;}
        }
        /// <summary>
        /// mesh的所属Actor
        /// </summary>
        [Browsable(false)]
        public override CCore.World.Actor HostActor
        {
            get { return mHostActor; }
            set
            {
                //if (mHostActor != null)
                //{
                //    mHostActor.OnActorEnterScene -= _OnActorEnterScene;
                //    mHostActor.OnActorRemoveFromScene -= _OnActorRemoveFromScene;
                //}

                mHostActor = value;

                //if (mHostActor != null)
                //{
                //    mHostActor.OnActorEnterScene += _OnActorEnterScene;
                //    mHostActor.OnActorRemoveFromScene += _OnActorRemoveFromScene;
                //}
            }
        }
        /// <summary>
        /// mesh的模板ID
        /// </summary>
        public System.Guid MeshTemplateID
        {
            get
            {
                Guid Id = Guid.Empty;
                var meshInit = VisualInit as MeshInit;
                if (meshInit != null)
                {
                    Id = meshInit.MeshTemplateID;
                }
                return Id;
            }
        }

        private UInt32 mVer = 0;
        /// <summary>
        /// meshPart列表
        /// </summary>
		protected List<MeshPart>	mMeshParts = new List<MeshPart>();
        /// <summary>
        /// 只读属性，meshPart列表
        /// </summary>
        public List<MeshPart> MeshParts
        {
            get { return mMeshParts; }
        }
        /// <summary>
        /// 只读属性，meshPart的数量
        /// </summary>
        public int MethPartsCount
        {
            get{ return mMeshParts.Count; }
        }

        /// <summary>
        /// 阴影渲染模式
        /// </summary>
		protected EShadingEnv			mShadingEnv;
        /// <summary>
        /// 只读属性，阴影渲染模式
        /// </summary>
        public EShadingEnv ShadingEnv
        {
            get { return mShadingEnv; }
        }
        /// <summary>
        /// 当前材质
        /// </summary>
		protected CCore.Material.Material			mCurMaterial;//SetCurMaterial设置后，这个mCurMaterial会被产生，然后可以用PropertyGrid看了。。。纯调试用的，没有工程实用价值
        /// <summary>
        /// 只读属性，当前材质
        /// </summary>
        public CCore.Material.Material CurMaterial
        {
            get { return mCurMaterial; }
        }
        /// <summary>
        /// 刷新所有的mesh，默认为false
        /// </summary>
        protected bool mTickAllTime = false;
        /// <summary>
        /// 是否可进行点击获取
        /// </summary>
        protected bool				mCanHitProxy = false;
        /// <summary>
        /// 是否可点击
        /// </summary>
        public bool CanHitProxy
        {
            get{ return mCanHitProxy; }
            set{ mCanHitProxy = value; }
        }
        /// <summary>
        /// 是否进行边缘检测，默认为false
        /// </summary>
        public bool mEdgeDetect = false;
        //public bool EdgeDetect
        //{
        //    get { return mEdgeDetect; }
        //    set { mEdgeDetect = value; }
        //}
        /// <summary>
        /// 是否块状区域化，默认为true
        /// </summary>
        protected bool mBlockNavigation = true;
        /// <summary>
        /// 是否为块状区域
        /// </summary>
        public bool BlockNavigation
        {
            get{return mBlockNavigation;}
            set{mBlockNavigation = value;}
        }

		// Skin
        /// <summary>
        /// 骨骼
        /// </summary>
		protected CCore.Skeleton.Skeleton	mFullSkeleton = null;
        /// <summary>
        /// 只读属性，骨骼
        /// </summary>
        public CCore.Skeleton.Skeleton FullSkeleton
        {
            get { return mFullSkeleton; }
        }
		// Socket
        /// <summary>
        /// 挂接件
        /// </summary>
		protected CCore.Socket.SocketTable mFullSocketTable = null;
        /// <summary>
        /// 只读属性，挂接件
        /// </summary>
        public CCore.Socket.SocketTable SocketTable
        {
            get { return mFullSocketTable; }
        }
        /// <summary>
        /// 是否蒙皮，默认为false
        /// </summary>
		protected bool mIsSkined = false;
        // 是否需要组装骨架， 默认为FALSE。 由外界设置
        /// <summary>
        /// 是否需要组装骨架， 默认为FALSE。 由外界设置
        /// </summary>
        protected bool mNeedCalcFullSkeleton = false;
        /// <summary>
        /// 是否需要组装骨架， 默认为FALSE。 由外界设置
        /// </summary>
        public bool NeedCalcFullSkeleton
        {
            get{return mNeedCalcFullSkeleton;}
            set{mNeedCalcFullSkeleton = value;}
        }
        /// <summary>
        /// 只读属性，精简mesh是否由编辑器创建
        /// </summary>
        public bool IsSimplifyMeshCreateByEditor
        {
            get
            {
                bool retValue = true;
                for(int i=0; i<mMeshParts.Count; ++i)
                {
                    if (mMeshParts[i].SimplifyMesh == IntPtr.Zero)
                    {
                        retValue = false;
                    }
                    else
                    {
                        var valCreateFrom = mMeshParts[i].IsCreateSimplifyMesh(enCreateFrom.ECF_Editor);
                        //var valCreateFrom = (IDllImportAPI.v3dStagedObject_ModelSource_CreateFrom(mMeshParts[i].mSimplifyMesh, (int)(enCreateFrom.ECF_Editor)) != 0) ? true : false;
                        retValue = retValue && valCreateFrom;
                    }
                }

                return retValue;
            }
        }
        /// <summary>
        /// 只读属性，是否拥有精简mesh
        /// </summary>
        public bool HasSimplifyMesh
        {
            get
            {
                for(int i=0; i<mMeshParts.Count; ++i)
                {
                    if(mMeshParts[i].SimplifyMesh != IntPtr.Zero)
                        return true;
                }

                return false;
            }
        }

        //protected int mCustomTime = 0;
        //public int CustomTime
        //{
        //    get{ return mCustomTime; }
        //    set{ mCustomTime = value; }
        //}
        /// <summary>
        /// 当前渐显次数
        /// </summary>
        public Int64 mCurrFadeTime = 0;
        float mFadePercent =1;
        /// <summary>
        /// 渐隐渐显的百分比
        /// </summary>
        public float FadePercent
        {
            get { return mFadePercent; }
            set { mFadePercent = value; }
        }
        /// <summary>
        /// 是否渐显，默认为false
        /// </summary>
        public bool mStartFadeIn = false;
        /// <summary>
        /// 是否渐隐，默认为false
        /// </summary>
        public bool mStartFadeOut = false;
        /// <summary>
        /// 包围盒
        /// </summary>
        protected SlimDX.BoundingBox	mBoundingBox = new SlimDX.BoundingBox();
        /// <summary>
        /// 精简mesh的类型
        /// </summary>
	    public enum enSimpleMeshType
		{
			Box,
			Sphere,
			Cylinder,
			Param,
		};
        /// <summary>
        /// 只读属性，顶点数量
        /// </summary>
        public int VertexCount
        {
            get
            {
                int iVertexCount = 0;
                for(int i=0; i<mMeshParts.Count; ++i)
                {
                    iVertexCount += (int)DllImportAPI.v3dStagedObject_ModelSource_GetVertexNumber(mMeshParts[i].Mesh);
                }

                return iVertexCount;
            }
        }
        /// <summary>
        /// 只读属性，三角形数量
        /// </summary>
        public int TriangleCount
        {
            get
            {
                int iTriangleCount = 0;
                for(int i=0; i<mMeshParts.Count; ++i)
                {
                    iTriangleCount += (int)DllImportAPI.v3dStagedObject_ModelSource_GetPolyNumber(mMeshParts[i].Mesh);
                }

                return iTriangleCount;
            }
        }
        /// <summary>
        /// 只读属性，最大顶点
        /// </summary>
        public SlimDX.Vector3 vMax
        {
            get
            {
                return mBoundingBox.Maximum;
            }
        }
        /// <summary>
        /// 只读属性，最小顶点
        /// </summary>
        public SlimDX.Vector3 vMin
        {
            get
            {
                return mBoundingBox.Minimum;
            }
        }
        /// <summary>
        /// 构造函数，将包围盒初始化为空盒子
        /// </summary>
        public Mesh()
        {
            mBoundingBox.InitEmptyBox();
            mCustomTime = 0;
        }
        /// <summary>
        /// 析构函数，删除对象，释放指针
        /// </summary>
		~Mesh()
        {
            Cleanup();
        }
        /// <summary>
        /// 更新包围盒
        /// </summary>
		protected virtual void UpdateBoundingBox()
        {
            unsafe
            {
                mBoundingBox.InitEmptyBox();

                if(m_pAnimTree != null)
                {
                    for(int i=0; i<mMeshParts.Count; ++i)
                    {
                        var pSkinModifier = DllImportAPI.v3dStagedObject_QueryModifier(mMeshParts[i].Mesh, vIIDDefine.vIID_v3dSkinModifier);
                        if(pSkinModifier != IntPtr.Zero)
                        {
                            //IDllImportAPI.v3dStagedObject_PreUse(mMeshParts[i].pMesh, false, IEngine.Instance.GetFrameMillisecond());
                            var tmax = new SlimDX.Vector3();
                            var tmin = new SlimDX.Vector3();
                            DllImportAPI.v3dStagedObject_ModelSource_GetSelfBox(mMeshParts[i].Mesh, &tmax, &tmin);
                            SlimDX.BoundingBox tbox = new SlimDX.BoundingBox(tmin, tmax);
                            mBoundingBox = SlimDX.BoundingBox.Merge(mBoundingBox, tbox);
                        }
                    }
                }
                else
                {
                    for(int i=0; i<mMeshParts.Count; ++i)
                    {
                        //IDllImportAPI.v3dStagedObject_PreUse(mMeshParts[i].pMesh, false, IEngine.Instance.GetFrameMillisecond());
                        var tmax = new SlimDX.Vector3();
                        var tmin = new SlimDX.Vector3();
                        DllImportAPI.v3dStagedObject_ModelSource_GetSelfBox(mMeshParts[i].Mesh, &tmax, &tmin);
                        SlimDX.BoundingBox tbox = new SlimDX.BoundingBox(tmin, tmax);
                        mBoundingBox = SlimDX.BoundingBox.Merge(mBoundingBox, tbox);
                    }
                }

                var max = new SlimDX.Vector3();
                var min = new SlimDX.Vector3();
                GetSocketAABB(out min, out max);
                SlimDX.BoundingBox tempBox = new SlimDX.BoundingBox(min, max);
                mBoundingBox = SlimDX.BoundingBox.Merge(mBoundingBox, tempBox);
                
            }
        }
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="_init">可视化的初始化类</param>
        /// <param name="host">所属Actor</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CCore.Component.VisualInit _init, CCore.World.Actor host)
        {
            Cleanup();

 	        base.Initialize(_init, host);

            mVisualInit = _init;
            var meshInit = _init as MeshInit;
            mCanHitProxy = meshInit.CanHitProxy;
            mBlockNavigation = meshInit.BlockNavigation;
            //EnableTrail = meshInit.mEnableTrail;
            mTickAllTime = meshInit.mTickAllTime;

            ResetMeshs(meshInit.MeshInitParts, meshInit.m_bNeedCalcFullSkeleton, meshInit.ForceLoad);

            //SetMaskColorOpacity(meshInit.mMaskColorOpacity, meshInit.mMaskColor);

            if(!string.IsNullOrEmpty(meshInit.CurActionName))
            {
                // 手动创建只有一个动作的AnimTree，用来播放默认动作。  
			    // 复杂动作树由外界调用SetAnimTree()来设置。
                var actionNode = new CCore.AnimTree.AnimTreeNode_Action();
                actionNode.Initialize();
                actionNode.ClearLink();
                actionNode.SetAction(meshInit.CurActionName);
                actionNode.PlayRate = meshInit.mPlayRate;
                SetAnimTree(actionNode);
            }
            
            if (meshInit.MeshTemplate != null)
            {
                mVer = meshInit.MeshTemplate.Ver;

                base.Layer = meshInit.MeshTemplate.Layer;

                mSocketComponents.Clear();
                meshInit.MeshTemplate.SocketComponentInfoList?.For_Each((Guid id, CCore.Socket.ISocketComponentInfo info, object arg) =>
                {
                    AddSocketItem(info);
                    return CSUtility.Support.EForEachResult.FER_Continue;
                }, null);
            }

            mCustomTime = 0;

            //StartFadeIn();

            return true;
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
		public override void Cleanup()
        {
            mEffectNotifiers.Clear();

            foreach (var i in mMeshParts)
            {
                i.Cleanup();
            }
            mMeshParts.Clear();
            ClearSimplifyMesh();
            m_pAnimTree = null;
            mFullSkeleton = null;
            mFullSocketTable = null;
            UpdateBoundingBox();
        }
        /// <summary>
        /// 使用之前的处理
        /// </summary>
        /// <param name="bForce">是否强制从磁盘加载</param>
        /// <param name="time">加载时间</param>
        public override void PreUse(bool bForce, ulong time)
        {
            foreach (var i in mMeshParts)
            {
                if(i == null)
                    continue;

                DllImportAPI.v3dStagedObject_PreUse(i.Mesh, bForce, (Int64)time);

                if (i.MeshPartInit != null)
                {
                    var fileName = i.MeshPartInit.MeshName;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var fileUrl = CSUtility.Program.FullPackageUrl + fileName + ".zip";
                        if (CSUtility.FileDownload.FileDownloadManager.Instance.IsFileDownloading(fileUrl))
                        {
                            CSUtility.FileDownload.FileDownloadManager.Instance.ChangeDownloadFileProiority(fileUrl, 1);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 提交阴影
        /// </summary>
        /// <param name="light">光源</param>
        /// <param name="matrix">对象的位置矩阵</param>
        /// <param name="isDynamic">是否为动态的(动态静态阴影都实时更新，只是动态阴影更新频率较高)</param>
        public override void CommitShadow(CCore.Light.Light light, ref SlimDX.Matrix matrix, bool isDynamic)
        {
            unsafe
            {
                fixed (SlimDX.Matrix* pinMatrix = &matrix)
                {
                    switch (ShadingEnv)
                    {
                        case CCore.EShadingEnv.SDE_Deffered:
                            {
                                for (int j = 0; j < MeshParts.Count; ++j)
                                {
                                    DllImportAPI.vLightProxy_CommitShadowMesh(light.Inner, Engine.Instance.GetFrameSecondTime(), MeshParts[j].Mesh, pinMatrix, isDynamic);
                                }
                            }
                            break;
                    }
                }
            }

            CommitSocketShadow(light, matrix, isDynamic);
        }

        //public virtual void CommitLighting(CCore.Light.Light light, ref SlimDX.Matrix matrix)
        //{
        //    if (light == null)
        //        return;

        //    foreach (var socktItem in mSocketItems)
        //    {
        //        var itemPos = socktItem.Info.Pos;
        //        var scale = socktItem.Info.Scale;
        //        var rotate = socktItem.Info.Rotate * (float)(System.Math.PI) / 180.0f;
        //        var itemQuat = SlimDX.Quaternion.RotationYawPitchRoll(rotate.Y, rotate.X, rotate.Z);

        //        foreach (var mesh in socktItem.Meshes)
        //        {
        //            if (mesh == null)
        //                continue;
        //            var meshInit = mesh.VisualInit as MeshInit;
        //            if (meshInit.mCastShadow == false)
        //                continue;

        //            SlimDX.Vector3 itemScale = scale * SlimDX.Vector3.UnitXYZ;
        //            if (meshInit.MeshTemplate != null)
        //            {
        //                itemScale = itemScale * meshInit.MeshTemplate.Scale;
        //            }

        //            var parentMatrix = CommitSocketMeshLighting(light, mesh, this, socktItem.Info.SocketName, itemPos, itemScale, itemQuat, ref matrix, true);
        //            mesh.CommitLighting(light, ref parentMatrix);
        //        }
        //    }
        //}

        SlimDX.Vector4 mEdgeDetectColor;
        /// <summary>
        /// 边缘检测的颜色
        /// </summary>
        public SlimDX.Vector4 EdgeDetectColor
        {
            get { return mEdgeDetectColor; }
            set
            {
                mEdgeDetectColor = value;
            }
        }

        EdgeDetectMode mEdgeDetectMode = EdgeDetectMode.Outline;
        /// <summary>
        /// 边缘检测的模式
        /// </summary>
        public EdgeDetectMode EdgeDetectMode
        {
            get { return mEdgeDetectMode; }
            set
            {
                mEdgeDetectMode = value;
            }
        }

        // 本体高亮的幅度
        float mEdgeDetectHightlight = 0;
        /// <summary>
        /// 本体高亮的幅度
        /// </summary>
        public float EdgeDetectHightlight
        {
            get { return mEdgeDetectHightlight; }
            set
            {
                mEdgeDetectHightlight = value;
            }
        }
        /// <summary>
        /// 前向渲染时是否提交，默认为true
        /// </summary>
        public static bool IsCommitFSTranslucent = true;
        /// <summary>
        /// 提交对象到渲染管道
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">对象的位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if (bUpdated == false && mIsSkined == true)
                return;

            unsafe
            {
                SlimDX.Matrix finalMatrix = matrix;
                // 测试代码，RootMotion数据是否合法
                //if (m_pAnimTree != null)
                //{
                //    var deltaPos = m_pAnimTree.GetDeltaRootmotionPos();
                //    var deltaQuat = m_pAnimTree.GetDeltaRootmotionQuat();
                //    var deltaM = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity, SlimDX.Vector3.UnitXYZ, SlimDX.Vector3.Zero, deltaQuat, deltaPos);
                //    finalMatrix = deltaM * matrix;
                //}

                if(mHostActor != null)
                    mHostActor.OnCommit(renderEnv, ref finalMatrix, eye);

                int renderGroup = (int)mGroup;
                Int64 time = Engine.Instance.GetFrameMillisecond();

                var simMeshShow = false;
                if (HostActor != null)
                    simMeshShow = CCore.Program.IsActorTypeShow(HostActor.World, CCore.Program.SimplyMeshTypeName);

                SlimDX.Matrix* pinMatrix = &finalMatrix;
                {
                    switch(ShadingEnv)
                    {
                        case EShadingEnv.SDE_Deffered:
                            if(Visible)
                            {

                                for(int i=0; i<MeshParts.Count; ++i)
                                {
                                    RLayer layer = mLayer;
                                    if(mHostActor != null)
                                    {
                                        if(mHostActor.Layer != RLayer.RL_None)
                                            layer = mHostActor.Layer;
                                    }

                                    switch(layer)
                                    {
                                        case RLayer.RL_DSBase:
                                            {
						                        if(MainMeshVisible && MeshParts[i].Mesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].Mesh, pinMatrix, eye.CommitCamera, mCustomTime,mFadePercent, mStartFadeIn?1:0, mStartFadeOut?1:0);
						                        if(simMeshShow && MeshParts[i].SimplifyMesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].SimplifyMesh, pinMatrix, eye.CommitCamera, mCustomTime, mFadePercent, mStartFadeIn ? 1 : 0, mStartFadeOut ? 1 : 0);
						                        if(PathMeshVisible && MeshParts[i].PathMesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].PathMesh, pinMatrix, eye.CommitCamera, mCustomTime, mFadePercent, mStartFadeIn ? 1 : 0, mStartFadeOut ? 1 : 0);
                                            }
                                            break;

                                        case RLayer.RL_DSTranslucent:
                                            {
						                        if(MainMeshVisible && MeshParts[i].Mesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSTranslucentMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].Mesh, pinMatrix, eye.CommitCamera, mCustomTime);
						                        if(simMeshShow && MeshParts[i].SimplifyMesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSTranslucentMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].SimplifyMesh, pinMatrix, eye.CommitCamera, mCustomTime);
						                        if(PathMeshVisible && MeshParts[i].PathMesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSTranslucentMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].PathMesh, pinMatrix, eye.CommitCamera, mCustomTime);
                                            }
                                            break;
                                        case RLayer.RL_DSPost:
                                            {
                                                if (MainMeshVisible && MeshParts[i].Mesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSPostMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].Mesh, pinMatrix, eye.CommitCamera, mCustomTime, mFadePercent, mStartFadeIn ? 1 : 0, mStartFadeOut ? 1 : 0);
                                                if (simMeshShow && MeshParts[i].SimplifyMesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSPostMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].SimplifyMesh, pinMatrix, eye.CommitCamera, mCustomTime, mFadePercent, mStartFadeIn ? 1 : 0, mStartFadeOut ? 1 : 0);
                                                if (PathMeshVisible && MeshParts[i].PathMesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitDSPostMesh(renderEnv.DSRenderEnv, time, renderGroup, MeshParts[i].PathMesh, pinMatrix, eye.CommitCamera, mCustomTime, mFadePercent, mStartFadeIn ? 1 : 0, mStartFadeOut ? 1 : 0);
                                            }
                                            break;

                                        case RLayer.RL_PreTranslucent:
                                        case RLayer.RL_Translucent:
                                        case RLayer.RL_PostTranslucent:
                                            {
                                                if(IsCommitFSTranslucent)
                                                {
                                                    if (MainMeshVisible && MeshParts[i].Mesh != IntPtr.Zero)
                                                        DllImportAPI.vDSRenderEnv_CommitFSMesh(renderEnv.DSRenderEnv, time, renderGroup, (int)layer, MeshParts[i].Mesh, pinMatrix, mCustomTime, mFadePercent, mStartFadeIn ? 1 : 0, mStartFadeOut ? 1 : 0);
                                                    if (simMeshShow && MeshParts[i].SimplifyMesh != IntPtr.Zero)
                                                        DllImportAPI.vDSRenderEnv_CommitFSMesh(renderEnv.DSRenderEnv, time, renderGroup, (int)layer, MeshParts[i].SimplifyMesh, pinMatrix, mCustomTime, mFadePercent, mStartFadeIn ? 1 : 0, mStartFadeOut ? 1 : 0);
                                                    if (PathMeshVisible && MeshParts[i].PathMesh != IntPtr.Zero)
                                                        DllImportAPI.vDSRenderEnv_CommitFSMesh(renderEnv.DSRenderEnv, time, renderGroup, (int)layer, MeshParts[i].PathMesh, pinMatrix, mCustomTime, mFadePercent, mStartFadeIn ? 1 : 0, mStartFadeOut ? 1 : 0);
                                                }
                                            }
                                            break;

                                        case RLayer.RL_SystemHelper:
                                            {
						                        if(MainMeshVisible && MeshParts[i].Mesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitHelperMesh(renderEnv.DSRenderEnv, renderGroup, MeshParts[i].Mesh, pinMatrix);
						                        if(simMeshShow && MeshParts[i].SimplifyMesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitHelperMesh(renderEnv.DSRenderEnv, renderGroup, MeshParts[i].SimplifyMesh, pinMatrix);
						                        if(PathMeshVisible && MeshParts[i].PathMesh != IntPtr.Zero)
                                                    DllImportAPI.vDSRenderEnv_CommitHelperMesh(renderEnv.DSRenderEnv, renderGroup, MeshParts[i].PathMesh, pinMatrix);
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    

                                    if (mEdgeDetect == true)
                                    {                                     
                                        if (MainMeshVisible && MeshParts[i].Mesh != IntPtr.Zero)
                                        {
                                            fixed(SlimDX.Vector4* pinEdgeColor = &mEdgeDetectColor)
                                            {
                                                DllImportAPI.vDSRenderEnv_CommitEdgeDetectMesh(renderEnv.DSRenderEnv, time, renderGroup, (int)layer, MeshParts[i].Mesh, pinMatrix, mCustomTime, pinEdgeColor, (int)mEdgeDetectMode, mEdgeDetectHightlight);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    if(Engine.Instance.IsEditorMode)
                    {
                        if (CanHitProxy && this.Visible)
                        {
                            for (int i = 0; i < MeshParts.Count; ++i)
                            {
                                DllImportAPI.vDSRenderEnv_CommitHitProxy(renderEnv.DSRenderEnv, renderGroup, MeshParts[i].Mesh, pinMatrix);
                            }
                        }
                    }
                    else
                    {
                        if (HostActor != null)
                        {
                            if (CanHitProxy && this.Visible && HostActor.EnableHitProxyInGame)
                            {
                                // 根据物体在屏幕中占的比例，来决定缩放倍数
                                //SlimDX.Vector3 loc, scale;
                                //SlimDX.Quaternion quat;
                                //finalMatrix.Decompose(out scale, out quat, out loc);
                                //var boxSize = mBoundingBox.Maximum - mBoundingBox.Minimum;
                                //boxSize.X = eye.GetWorldSizeInScreen( loc, System.Math.Abs(boxSize.X))*3;
                                //boxSize.Y = eye.GetWorldSizeInScreen( loc, System.Math.Abs(boxSize.Y))*3;
                                //boxSize.Z = eye.GetWorldSizeInScreen( loc, System.Math.Abs(boxSize.Z))*3;
                                //boxSize.X = boxSize.X>1 ? 1 : boxSize.X;
                                //boxSize.Y = boxSize.Y>1 ? 1 : boxSize.Y;
                                //boxSize.Z = boxSize.Z>1 ? 1 : boxSize.Z;
                                //SlimDX.Vector3 minScale = new SlimDX.Vector3(1.1f);
                                //SlimDX.Vector3 maxScale = new SlimDX.Vector3(3.0f);
                                //var lerpScale = SmoothStep(maxScale, minScale, boxSize);
                                //var tempScaleM = SlimDX.Matrix.Scaling(lerpScale);
                                var tempScaleM = SlimDX.Matrix.Scaling(1.2f, 1.2f, 1.2f);
                                var hitProxyMatrix = tempScaleM * finalMatrix;
                                SlimDX.Matrix* pinHitMatrix = &hitProxyMatrix;
                                for (int i = 0; i < MeshParts.Count; ++i)
                                {
                                    DllImportAPI.vDSRenderEnv_CommitHitProxy(renderEnv.DSRenderEnv, renderGroup, MeshParts[i].Mesh, pinHitMatrix);
                                }
                            }
                        }
                    }
                }

                CommitSocketItem(renderEnv, matrix, eye);

            }
        }
        /// <summary>
        /// 有效数量
        /// </summary>
        /// <param name="amount">数量</param>
        /// <returns>返回计算好的有效数量值</returns>
        float ValidAmount(float amount)
        {
            amount = (amount > 1.0f) ? 1.0f : ((amount < 0.0f) ? 0.0f : amount);
            amount = (amount * amount) * (3.0f - (2.0f * amount));
            return amount;
        }
        /// <summary>
        /// 平滑插值
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="amount">插值</param>
        /// <returns>返回插值</returns>
        SlimDX.Vector3 SmoothStep(SlimDX.Vector3 start, SlimDX.Vector3 end, SlimDX.Vector3 amount)
        {
            SlimDX.Vector3 vector = new SlimDX.Vector3();

            amount.X = ValidAmount(amount.X);
            amount.Y = ValidAmount(amount.Y);
            amount.Z = ValidAmount(amount.Z);

            vector.X = start.X + ((end.X - start.X) * amount.X);
            vector.Y = start.Y + ((end.Y - start.Y) * amount.Y);
            vector.Z = start.Z + ((end.Z - start.Z) * amount.Z);

            return vector;
        }
        /// <summary>
        /// 获取AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点坐标</param>
        /// <param name="vMax">最大顶点坐标</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            vMin = this.vMin;
            vMax = this.vMax;
        }
        /// <summary>
        /// 获取OBB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点坐标</param>
        /// <param name="vMax">最大顶点坐标</param>
        /// <param name="fixMatrix">当前绑定的位置矩阵</param>
        public override void GetOBB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax, ref SlimDX.Matrix fixMatrix)
        {
            if (mMeshParts.Count == 0 || mMeshParts[0] == null)
                return;

            unsafe
            {
                fixed (SlimDX.Vector3* pMin = &vMin)
                {
                    fixed (SlimDX.Vector3* pMax = &vMax)
                    {
                        fixed(SlimDX.Matrix* pFixMatrix = &fixMatrix)
                        {
                            DllImportAPI.v3dStagedObject_ModelSource_GetSelfOBB(mMeshParts[0].Mesh, pMax, pMin, pFixMatrix);
                        }
                    }
                }
            }
        }
		/// <summary>
        /// 重置mesh
        /// </summary>
        /// <param name="MeshInitParts">meshPart的初始化类</param>
        /// <param name="needCalcFullSkeleton">是否组装骨骼</param>
        /// <param name="forceLoad">是否强制从磁盘加载</param>
		public void ResetMeshs(List<MeshInitPart> MeshInitParts, bool needCalcFullSkeleton, bool forceLoad)
        {
            foreach (var i in mMeshParts)
            {
                i.Cleanup();
            }
            mMeshParts.Clear();

            for(int i=0; i<MeshInitParts.Count; i++)
            {
                var initPart = MeshInitParts[i];
                MeshPart meshPart = null;
                if(CreateMeshPart(initPart, ref meshPart, forceLoad) == false)
                    continue;
                mMeshParts.Add(meshPart);
            }

            mNeedCalcFullSkeleton = needCalcFullSkeleton;
            if(mNeedCalcFullSkeleton && mIsSkined == true)
            {
                _BuildFullSkeleton_FromMeshes();

                if(m_pAnimTree != null)
                    m_pAnimTree.SetSkeleton(mFullSkeleton);
            }

            _BuildFullSocketTable();
            UpdateBoundingBox();
        }
        /// <summary>
        /// 创建meshPart对象
        /// </summary>
        /// <param name="partInit">meshPart的初始化类</param>
        /// <param name="meshPart">meshPart类对象</param>
        /// <param name="forceLoad">是否强制从磁盘加载</param>
        /// <returns>创建成功返回true，否则返回false</returns>
		public bool CreateMeshPart(MeshInitPart partInit, ref MeshPart meshPart, bool forceLoad)
        {
            unsafe
            {
                if(partInit == null)
                    return false;

                IntPtr ms = IntPtr.Zero;        // model3::v3dModelSource
                IntPtr simMs = IntPtr.Zero;     // model3::v3dModelSource
                IntPtr pathMs = IntPtr.Zero;    // model3::v3dModelSource
                IntPtr bsp = IntPtr.Zero;       // model3::v3dBspSpace
                IntPtr sockets = IntPtr.Zero;   // model3::v3dSocketTable

                if(!string.IsNullOrEmpty(partInit.MeshName))
                {
                    SlimDX.Matrix matIdentity = SlimDX.Matrix.Identity;

                    #region 创建v3dModelSource对象
                    switch(partInit.MeshName)
                    {
                        case "@Box":
                            {
                                ms = DllImportAPI.v3dModelCooking_CookBox(Engine.Instance.Client.Graphics.Device, &matIdentity, 1, 1, 1, 0, (int)(RenderAPI.V3DPOOL.V3DPOOL_MANAGED));
                            }
                            break;

                        case "@Sphere":
                            {
                                ms = DllImportAPI.v3dModelCooking_CookSphere(Engine.Instance.Client.Graphics.Device, &matIdentity, 1.0f, 10, 10, 0, (int)(RenderAPI.V3DPOOL.V3DPOOL_MANAGED));
                            }
                            break;

                        case "@Rect":
                            {
                                ms = DllImportAPI.v3dModelCooking_CookModelRect(Engine.Instance.Client.Graphics.Device, &matIdentity, 0, 0, 10, 10, 0, 0, (int)(RenderAPI.V3DPOOL.V3DPOOL_MANAGED));
                            }
                            break;

                        case "@Trail":
                            {
                                ms = DllImportAPI.v3dModelCooking_CookModelTrail(Engine.Instance.Client.Graphics.Device, &matIdentity, 0, (int)(RenderAPI.V3DPOOL.V3DPOOL_MANAGED));
                            }
                            break;
                            
                        default:
                            {
                                // 所有的路径均使用\\而不是/，否则资源缓存的key上会出现重复(V3_ResourceMgr.m_ResQueue)
                                ms = DllImportAPI.v3dDevice_VMObjMgr_LoadModelSource(Engine.Instance.Client.Graphics.Device, partInit.MeshName, (uint)0, (int)(RenderAPI.V3DPOOL.V3DPOOL_DEFAULT), forceLoad);
                                // 读取简化模型和BSP信息
                                simMs = DllImportAPI.v3dDevice_VMObjMgr_LoadModelSource(Engine.Instance.Client.Graphics.Device, partInit.MeshName + CSUtility.Support.IFileConfig.SimpleMeshExtension, 0, (int)(RenderAPI.V3DPOOL.V3DPOOL_DEFAULT), forceLoad);
                                bsp = DllImportAPI.v3dDevice_RAMObjMgr_LoadBSPSource(Engine.Instance.Client.Graphics.Device, partInit.MeshName + CSUtility.Support.IFileConfig.SimpleMeshExtension, 0, (int)(RenderAPI.V3DPOOL.V3DPOOL_DEFAULT), false);
                                // 读取寻路模型信息
                                pathMs = DllImportAPI.v3dDevice_VMObjMgr_LoadModelSource(Engine.Instance.Client.Graphics.Device, partInit.MeshName + CSUtility.Support.IFileConfig.PathMeshExtension, 0, (int)(RenderAPI.V3DPOOL.V3DPOOL_DEFAULT), forceLoad);
                                // 读取Socket信息
                                sockets = DllImportAPI.v3dDevice_RAMObjMgr_LoadSocketTable(Engine.Instance.Client.Graphics.Device, partInit.MeshName + CSUtility.Support.IFileConfig.MeshSocketExtension);
                            }
                            break;
                    }

                    if(ms == IntPtr.Zero)
                    {
                        UpdateBoundingBox();
                        return false;
                    }
                    #endregion

                    meshPart = new MeshPart();
                    var ret = meshPart.LoadMesh(partInit, ms);
                    if (ret == false)
                    {
                        UpdateBoundingBox();
                        return false;
                    }

                    if (mIsSkined == false)
                    {
                        var pSkinModifier = DllImportAPI.v3dStagedObject_QueryModifier(meshPart.Mesh, vIIDDefine.vIID_v3dSkinModifier);
                        if (pSkinModifier != IntPtr.Zero)
                            mIsSkined = true;
                    }

                    //if (meshPart.TrailModifier != IntPtr.Zero)
                    //    mIsTrail = true;

                    if(simMs != IntPtr.Zero)
                    {
                        meshPart.LoadSimplifyMesh(simMs);
                    }

                    if(bsp != IntPtr.Zero)
                    {
                        meshPart.SetBSP(bsp);
                    }

                    if(pathMs != IntPtr.Zero)
                    {
                        meshPart.LoadPathMesh(pathMs);
                    }

                    if(sockets != IntPtr.Zero)
                    {
                        meshPart.SetSockets(sockets);
                    }

                    UpdateBoundingBox();

                    DllImportAPI.v3dModelSource_Release(ms);
                    DllImportAPI.v3dModelSource_Release(simMs);
                    DllImportAPI.v3dModelSource_Release(pathMs);
                    DllImportAPI.v3dBspSpace_Release(bsp);
                    DllImportAPI.V3DSocketTable_Release(sockets);

                    return true;
                }

                UpdateBoundingBox();
                return false;
            }
        }
        /// <summary>
        /// 使用之前的处理
        /// </summary>
        /// <param name="bForce">是否强制从磁盘加载</param>
        /// <param name="time">加载时间</param>
		public void Preuse(bool bForce, Int64 time)
        {
            unsafe
            {
                foreach (var mp in MeshParts)
                {
                    DllImportAPI.v3dStagedObject_PreUse(mp.Mesh, bForce, time);
                    if(mp.SimplifyMesh != IntPtr.Zero)
                    {
                        DllImportAPI.v3dStagedObject_PreUse(mp.SimplifyMesh, bForce, time);
                    }
                    uint matCount = (uint)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(mp.Mesh);
                    for(uint i=0; i<matCount; i++)
                    {
                        DllImportAPI.v3dStagedObject_MaterialTechnique_PreUse(mp.Mesh, i, bForce, time);
                    }
                }
            }
        }
        /// <summary>
        /// 获取材质的最大数量
        /// </summary>
        /// <param name="MeshIndex">mesh索引</param>
        /// <returns>返回材质的最大数量</returns>
        public int GetMaxMaterial( int MeshIndex )
        {
            if(MeshIndex >= mMeshParts.Count || mMeshParts.Count <= 0)
                return 0;

            return (int)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(mMeshParts[MeshIndex].Mesh);
        }
        /// <summary>
        /// 设置所有的mesh可以点击
        /// </summary>
        /// <param name="hitProxy">点击代理值</param>
        public override void SetHitProxyAll(UInt32 hitProxy)
        {
            if(!mCanHitProxy)
                return;

            for(int i=0; i<mMeshParts.Count; ++i)
            {
                var mesh = mMeshParts[i].Mesh;
                uint matCount = (uint)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(mesh);
                for(uint j=0; j<matCount; ++j)
                {
                    DllImportAPI.v3dStagedObject_SetHitProxy(mesh, j, hitProxy);
                }
            }

            // 组件的像素点选
            foreach(var component in mSocketComponents.Values)
            {
                var chp = component as CCore.Socket.ISocketComponentHitProxy;
                if (chp == null)
                    continue;

                if (chp.CanHitProxy)
                    chp.SetHitProxyAll(hitProxy);
            }
        }
        /// <summary>
        /// 设置单个对象可点击
        /// </summary>
        /// <param name="MeshIndex">mesh索引</param>
        /// <param name="MaterialIndex">材质索引</param>
        /// <param name="hitProxy">点击代理</param>
		public override void SetHitProxy( int MeshIndex, int MaterialIndex, UInt32 hitProxy)
        {
            if(!mCanHitProxy)
                return;

		    if( MeshIndex >= mMeshParts.Count || mMeshParts.Count <= 0 )
			    return;

            DllImportAPI.v3dStagedObject_SetHitProxy(mMeshParts[MeshIndex].Mesh, (uint)MaterialIndex, hitProxy);

            if (MeshIndex == 0)
            {
                // 组件的像素点选，默认与第0个模型相同
                foreach(var component in mSocketComponents.Values)
                {
                    var chp = component as CCore.Socket.ISocketComponentHitProxy;
                    if (chp == null)
                        continue;

                    if (chp.CanHitProxy)
                        chp.SetHitProxyAll(hitProxy);
                }
            }
        }
        /// <summary>
        /// 设置meshPart的mesh
        /// </summary>
        /// <param name="MeshIndex">mesh索引</param>
        /// <param name="initPart">meshPart的初始化类对象</param>
        /// <param name="forceLoad">是否强制从磁盘加载</param>
        public void SetPartMesh( int MeshIndex, MeshInitPart initPart, bool forceLoad)
        {
            if( MeshIndex >= mMeshParts.Count || mMeshParts.Count <= 0 )
			    return;

		    MeshPart meshPart = null;
		    if( CreateMeshPart(initPart, ref meshPart, forceLoad) == false )
			    return;

		    mMeshParts[MeshIndex] = meshPart;

		    if(mNeedCalcFullSkeleton)
			    _BuildFullSkeleton_FromMeshes();
		    _BuildFullSocketTable();
        }
        /// <summary>
        /// 设置材质
        /// </summary>
        /// <param name="MeshIndex">mesh索引</param>
        /// <param name="MaterialIndex">材质索引</param>
        /// <param name="mtl">材质对象</param>
		public void SetMaterial(int MeshIndex, int MaterialIndex, CCore.Material.Material mtl)
        {
		    if( MeshIndex >= mMeshParts.Count || mMeshParts.Count <= 0 || mtl == null)
			    return;

            DllImportAPI.v3dStagedObject_SetMaterial(mMeshParts[MeshIndex].Mesh, (uint)MaterialIndex, mtl.MaterialPtr, IntPtr.Zero);
        }

        /// <summary>
        /// 设置蒙皮颜色的透明度
        /// </summary>
        /// <param name="opacity">透明度</param>
        /// <param name="maskColor">颜色</param>
        public void SetMaskColorOpacity(float opacity, SlimDX.Vector3 maskColor)
        {
            unsafe
            {
                //fixed(SlimDX.Vector3* pinMaskColor = &maskColor)
                {
                    if(opacity == 0)
                        return;

                    for(int i=0; i<MeshParts.Count; ++i)
                    {
                        var pM = mMeshParts[i].Mesh;
                        if(pM != IntPtr.Zero)
                        {
                            uint matCount = (uint)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(pM);
                            for(uint j=0; j<matCount; ++j)
                            {
                                var materialInstance = DllImportAPI.v3dStagedObject_GetMaterial(pM, j);
                                DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "Opacity", opacity);
                                DllImportAPI.v3dStagedMaterialInstance_SetFloat3(materialInstance, "ChangeColor", &maskColor);
                            }
                        }
                    }
                }
            }
        }

        float mBeAttackFlashWhite = 0.0f;
        /// <summary>
        /// 获取攻击闪光的值
        /// </summary>
        /// <returns>返回攻击发光的值</returns>
        public float GetBeAttackFlashWhite()
        {
            return mBeAttackFlashWhite;
        }
        /// <summary>
        /// 设置攻击闪光的值
        /// </summary>
        /// <param name="t">攻击闪光的值</param>
        public void SetBeAttackFlashWhite(float t)
        {
            mBeAttackFlashWhite = t;
            unsafe
            {
                for (int i = 0; i < MeshParts.Count; ++i)
                {
                    var pM = mMeshParts[i].Mesh;
                    if (pM != IntPtr.Zero)
                    {
                        uint matCount = (uint)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(pM);
                        for (uint j = 0; j < matCount; ++j)
                        {
                            var materialInstance = DllImportAPI.v3dStagedObject_GetMaterial(pM, j);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "BeAttackFlashWhite", t);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 轮廓的起始值
        /// </summary>
        public float mRimStart = 0.5f;
        /// <summary>
        /// 轮廓的结束值
        /// </summary>
        public float mRimEnd = 1.0f;
        /// <summary>
        /// 轮廓的颜色
        /// </summary>
        public SlimDX.Vector4 mRimColor = new SlimDX.Vector4(1,1,1,1);
        /// <summary>
        /// 轮廓的混合度
        /// </summary>
        public float mRimMultiply = 1.0f;
        /// <summary>
        /// Bloom状轮廓的值为0
        /// </summary>
        public int mIsRimBloom = 0;
        /// <summary>
        /// 更新轮廓的参数
        /// </summary>
        public virtual void UpdateRimParameter()
        {
            unsafe
            {
                for (int i = 0; i < MeshParts.Count; ++i)
                {
                    var pM = mMeshParts[i].Mesh;
                    if (pM != IntPtr.Zero)
                    {
                        uint matCount = (uint)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(pM);
                        for (uint j = 0; j < matCount; ++j)
                        {
                            var materialInstance = DllImportAPI.v3dStagedObject_GetMaterial(pM, j);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "rimStart", mRimStart);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "rimEnd", mRimEnd);
                            fixed (SlimDX.Vector4* pinV = &mRimColor)
                                DllImportAPI.v3dStagedMaterialInstance_SetFloat4(materialInstance, "rimColor", pinV);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "rimMultiply", mRimMultiply);
                            DllImportAPI.v3dStagedMaterialInstance_SetInt(materialInstance, "isRimBloom", mIsRimBloom);                            
                        }
                    }
                }
            }
        }


        // 更新附魔粒子特效中的材质参数
        /// <summary>
        /// 更新附魔粒子特效中的材质参数
        /// </summary>
        /// <param name="texName">纹理名称</param>
        /// <param name="frameX">X轴的位置坐标</param>
        /// <param name="frameY">Y轴的位置坐标</param>
        /// <param name="duration">间隔时间</param>
        public void UpdateAffixParameter(string texName, UInt16 frameX, UInt16 frameY, float duration)
        {
            unsafe
            {
                for (int i = 0; i < MeshParts.Count; ++i)
                {
                    var pM = mMeshParts[i].Mesh;
                    if (pM != IntPtr.Zero)
                    {
                        uint matCount = (uint)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(pM);
                        for (uint j = 0; j < matCount; ++j)
                        {
                            var materialInstance = DllImportAPI.v3dStagedObject_GetMaterial(pM, j);
                            var texture = new CCore.Graphics.Texture();
                            texture.LoadTexture(texName);
                            DllImportAPI.v3dStagedMaterialInstance_SetTexture(materialInstance, "Diffuse", texture.TexturePtr);
                            SlimDX.Vector2 frameCount = new SlimDX.Vector2(frameX, frameY);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat2(materialInstance, "FrameCount", &frameCount);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "Duration", duration);                            
                        }
                    }
                }
            }
        }

        // 更新强化特效中的材质参数
        /// <summary>
        /// 更新强化特效中的材质参数
        /// </summary>
        /// <param name="meshTemplateId">mesh模板ID</param>
        /// <param name="glowTex">发光纹理的名称</param>
        /// <param name="haveGlow">有光</param>
        /// <param name="glowSpeed">发光速度</param>
        /// <param name="glowScale">发光的缩放值</param>
        /// <param name="glowIntensity">光强</param>
        /// <param name="glowColor">光照颜色</param>
        public void UpdateMeshPartEnhanceParameter(System.Guid meshTemplateId, string glowTex, int haveGlow, SlimDX.Vector2 glowSpeed, SlimDX.Vector2 glowScale, float glowIntensity, SlimDX.Vector4 glowColor)
        {
            unsafe
            {
                for (int i = 0; i < MeshParts.Count; ++i)
                {
                    if (mMeshParts[i].MeshPartInit.OwnerMeshId != meshTemplateId)
                        continue;

                    var pM = mMeshParts[i].Mesh;
                    if (pM != IntPtr.Zero)
                    {
                        uint matCount = (uint)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(pM);
                        for (uint j = 0; j < matCount; ++j)
                        {
                            var materialInstance = DllImportAPI.v3dStagedObject_GetMaterial(pM, j);
                            if (glowTex !=null && glowTex.Length != 0)
                            {
                                var texture = new CCore.Graphics.Texture();
                                texture.LoadTexture(glowTex);
                                DllImportAPI.v3dStagedMaterialInstance_SetTexture(materialInstance, "GlowTex", texture.TexturePtr);
                            }
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "HaveGlow", haveGlow);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat2(materialInstance, "GlowSpeed", &glowSpeed);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat2(materialInstance, "GlowScale", &glowScale);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "GlowIntensity", glowIntensity);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat4(materialInstance, "GlowColor", &glowColor);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 更新光照的参数
        /// </summary>
        /// <param name="glowTex">光照纹理名称</param>
        /// <param name="haveGlow">是否闪光</param>
        /// <param name="glowSpeed">闪光速度</param>
        /// <param name="glowScale">闪光的缩放值</param>
        /// <param name="glowIntensity">光强</param>
        /// <param name="glowColor">闪光的颜色</param>
        public void UpdateEnhanceParameter(string glowTex, int haveGlow, SlimDX.Vector2 glowSpeed, SlimDX.Vector2 glowScale, float glowIntensity, SlimDX.Vector4 glowColor)
        {
            unsafe
            {
                for (int i = 0; i < MeshParts.Count; ++i)
                {
                    var pM = mMeshParts[i].Mesh;
                    if (pM != IntPtr.Zero)
                    {
                        uint matCount = (uint)DllImportAPI.v3dStagedObject_GetMaxMaterialNum(pM);
                        for (uint j = 0; j < matCount; ++j)
                        {
                            var materialInstance = DllImportAPI.v3dStagedObject_GetMaterial(pM, j);
                            if (glowTex  !=null && glowTex.Length != 0)
                            {
                                var texture = new CCore.Graphics.Texture();
                                texture.LoadTexture(glowTex);
                                DllImportAPI.v3dStagedMaterialInstance_SetTexture(materialInstance, "GlowTex", texture.TexturePtr);
                            }
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "HaveGlow", haveGlow);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat2(materialInstance, "GlowSpeed", &glowSpeed);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat2(materialInstance, "GlowScale", &glowScale);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat(materialInstance, "GlowIntensity", glowIntensity);
                            DllImportAPI.v3dStagedMaterialInstance_SetFloat4(materialInstance, "GlowColor", &glowColor);
                        }
                    }
                }
            }
        }

        //void AddModifier( IMeshModifier^ mdf , bool AcceptSource );

        //public void performPhysicMeshGenerate(Physics.enPhysicGeometryType type)
        //{
        //    try
        //    {
        //        var meshInit = VisualInit as IMeshInit;

        //        for (int i = 0; i < mMeshParts.Count; ++i)
        //        {
        //            if (mMeshParts[i].SimplifyMesh == IntPtr.Zero)
        //            {
        //                System.Windows.Forms.MessageBox.Show(meshInit.MeshInitParts[i].MeshName + "没有设置碰撞模型，请先设置碰撞模型之后再生成物理");
        //                break;
        //            }
        //        }

        //        for (int i = 0; i < mMeshParts.Count; ++i)
        //        {
        //            mMeshParts[i].CreatePhysicShape();
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show(ex.ToString());
        //    }
        //    finally
        //    {
        //        SavePhysicMesh();
        //    }
        //}
        /// <summary>
        /// 删除简化模型，释放其指针内存
        /// </summary>
		public void ClearSimplifyMesh()
        {
            for(int i=0; i<mMeshParts.Count; ++i)
            {
                if (mMeshParts[i].SimplifyMesh != IntPtr.Zero)
                {
                    mMeshParts[i].ClearSimplifyMesh();
                }
            }
        }
        /// <summary>
        /// 清除物理mesh对象
        /// </summary>
        public void ClearPhysicMesh()
        {
            for (int i = 0; i < mMeshParts.Count; ++i)
            {
                if (mMeshParts[i].PhysicShape != IntPtr.Zero)
                {
                    mMeshParts[i].ClearPhysicShape();
                }
            }
        }
        /// <summary>
        /// 存储简化模型(模型信息、BSP、寻路网格)
        /// </summary>
		public void SaveSimplifyMesh()
        {
            // 存储简化模型(模型信息、BSP、寻路网格)
            var meshInit = mVisualInit as CCore.Mesh.MeshInit;

            for(int i=0; i<mMeshParts.Count; ++i)
            {
                var meshName = CSUtility.Support.IFileManager.Instance.Root + meshInit.MeshInitParts[i].MeshName + CSUtility.Support.IFileConfig.SimpleMeshExtension;
                DllImportAPI.v3dStagedObject_SaveMeshAndBSP(mMeshParts[i].SimplifyMesh, mMeshParts[i].BSPOperator.BSPSpace, meshName);
            }
        }

        //public void SavePhysicMesh()
        //{
        //    var meshInit = mVisualInit as IMeshInit;

        //    for (int i = 0; i < mMeshParts.Count; ++i)
        //    {
        //        if (mMeshParts[i].PhysicsGeometry == null)
        //            continue;

        //        // 存储物理模型
        //        var meshName = CSUtility.Support.IFileManager.Instance.Root + meshInit.MeshInitParts[i].MeshName + CSUtility.Support.IFileConfig.PhysicGeometryExtension;

        //        var xndHolder = CSUtility.Support.IXndHolder.NewXNDHolder();
        //        mMeshParts[i].PhysicsGeometry.SaveXND(xndHolder.Node);

        //        CSUtility.Support.IXndHolder.SaveXND(meshName, xndHolder);
        //    }
        //}
        /// <summary>
        /// mesh的BSP网格线检查数量
        /// </summary>
        public static int MeshBSPLineCheckNumber = 0;
        /// <summary>
        /// mesh的三角形网格线检查数量
        /// </summary>
        public static int MeshTriLineCheckNumber = 0;
        /// <summary>
        /// 总的测试用三角形
        /// </summary>
        public static int TotalTringleTest = 0;
        /// <summary>
        /// 所有的测试盒子数量
        /// </summary>
        public static int TotalBoxTest = 0;
        /// <summary>
        /// 计算检查的网格线数量
        /// </summary>
        public static CSUtility.Performance.PerfCounter CountLineCheck = new CSUtility.Performance.PerfCounter("LineCheck");
        /// <summary>
        /// 连线检查
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="matrix">对象的位置矩阵</param>
        /// <param name="result">检查的结果</param>
        /// <returns>检查成功返回true，否则返回false</returns>
        public override bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref SlimDX.Matrix matrix, ref CSUtility.Support.stHitResult result)
        {
            int bOBBCheck = mHostActor.VisibleCheckOBB ? 1 : 0;
            unsafe
            {
                fixed (SlimDX.Vector3* pinStart = &start)
                fixed (SlimDX.Vector3* pinEnd = &end)
                fixed (SlimDX.Matrix* pinMatrix = &matrix)
                fixed(CSUtility.Support.stHitResult* pinResult = &result)
                {
                    bool bRetValue = false;
                    float fLength = float.MaxValue;
                    SlimDX.Vector3 length = end - start;

                    if(result.HasFlag((uint)CSUtility.enHitFlag.HitMeshTriangle))
                    {
                        CountLineCheck.Begin();
                        bool bTempReturn = false;
                        for(int i=0; i<mMeshParts.Count; ++i)
                        {
                            bTempReturn = ((DllImportAPI.v3dStagedObject_LineCheck(bOBBCheck, mMeshParts[i].Mesh, pinStart, &length, pinMatrix, pinResult) != 0) ? true : false);
                            if(fLength >= pinResult->mHitLength && bTempReturn)
                            {
                                fLength = pinResult->mHitLength;
                                        
                                bRetValue = bTempReturn;
                            }
                            MeshTriLineCheckNumber++;
                            TotalTringleTest = pinResult->TriTest;
                        }
                        CountLineCheck.End();
                    }
                    else
                    {
                        bool bTempReturn = false;
                        for (int i=0; i<mMeshParts.Count; ++i)
                        {
                            if(mMeshParts[i].BSPOperator != null)
                            {
                                CountLineCheck.Begin();
                                // 使用BSP进行碰撞检测
                                if (bOBBCheck != 0)
                                {
                                    SlimDX.BoundingBox obb = new SlimDX.BoundingBox();
                                    SlimDX.Matrix fixMatrix = new SlimDX.Matrix();
                                    mHostActor.Visual.GetOBB(ref obb.Minimum, ref obb.Maximum, ref fixMatrix);
                                    fixMatrix = fixMatrix * matrix;
                                    if (DllImportAPI.v3dStageObject_LineIntersect(&fixMatrix, pinStart, pinEnd, &obb) == 0)
                                        continue;
                                }
                                        
                                bTempReturn = mMeshParts[i].BSPOperator.LineCheck(ref start, ref end, ref matrix, ref result);
                                if (fLength >= result.mHitLength && bTempReturn)
                                {
                                    fLength = result.mHitLength;
                                    bRetValue = true;
                                }
                                MeshBSPLineCheckNumber++;
                                TotalTringleTest = result.TriTest;
                                TotalBoxTest = result.BoxTest;
                                CountLineCheck.End();
                            }
                            else if(mMeshParts[i].SimplifyMesh != IntPtr.Zero)
                            {
                                CountLineCheck.Begin();
                                // 使用简化模型进行碰撞检测
                                bTempReturn = (DllImportAPI.v3dStagedObject_LineCheck(bOBBCheck, mMeshParts[i].SimplifyMesh, pinStart, &length, pinMatrix, pinResult) != 0) ? true : false;
                                if(fLength >= pinResult->mHitLength && bTempReturn)
                                {
                                    fLength = pinResult->mHitLength;
                                    bRetValue = true;
                                }
                                MeshTriLineCheckNumber++;
                                TotalTringleTest = pinResult->TriTest;
                                CountLineCheck.End();
                            }
                        }
                    }

                    return bRetValue;
                }

            }
        }

		// 编辑器用接口
        /// <summary>
        /// 获取最近的顶点坐标
        /// </summary>
        /// <param name="srcPos">资源的原始位置</param>
        /// <param name="tagPos">目标位置</param>
        /// <param name="matrix">对象的位置矩阵</param>
        /// <param name="fMinDistance">最小距离</param>
        /// <returns>得到返回true，否则返回false</returns>
		public bool GetNearestVertexPos(ref SlimDX.Vector3 srcPos, ref SlimDX.Vector3 tagPos, ref SlimDX.Matrix matrix, float fMinDistance)
        {
            unsafe
            {
                fixed(SlimDX.Vector3* pinSrcPos = &srcPos)
                {
                    fixed(SlimDX.Vector3* pinTagPos = &tagPos)
                    {
                        fixed(SlimDX.Matrix* pinMatrix = &matrix)
                        {
                            float fMinLength = fMinDistance * 10;
                            int returnValue = 0;
                            for(int i=0; i<mMeshParts.Count; ++i)
                            {
                                var temp = DllImportAPI.v3dStagedObject_GetNearestVertexPos(mMeshParts[i].Mesh, pinSrcPos, pinTagPos, pinMatrix, &fMinDistance);

                                float fCurLength = (tagPos - srcPos).Length();
                                if(fMinLength >= fCurLength)
                                {
                                    fMinLength = fCurLength;
                                    returnValue = temp;
                                }
                            }

                            return (returnValue != 0)? true : false;
                        }
                    }
                }
            }

        }

        bool bUpdated = false;
        /// <summary>
        /// 更新函数
        /// </summary>
        /// <param name="tm">更新时间</param>
		public void Update( Int64 tm )
        {
            bUpdated = true;
            mCustomTime += (int)tm;

            //foreach (var mp in mMeshParts)
            //{
            //    if(mp == null)
            //        continue;
            //    mp.Tick(tm);
            //}

            if (mIsSkined == true)
            {
                // 如果外界没有设置ANIMTREE， 同时MESH数据里包含ANIMTREE数据， 则更新之
                if(m_pAnimTree == null)
                {
                    for(int i=0; i<mMeshParts.Count; ++i)
                    {
                        var pSkinModifier = DllImportAPI.v3dStagedObject_QueryModifier(mMeshParts[i].Mesh, vIIDDefine.vIID_v3dSkinModifier);
                        if(pSkinModifier != IntPtr.Zero)
                        {
                            var result = DllImportAPI.v3dSkinModifier_Update(pSkinModifier, tm);
                            if(result != 0)
                            {
                                Engine.Instance.TickAnimationTreeNumber++;
                            }
                        }
                    }
                }
                else
                {
                    // 更新外界设置的ANIMTREE
                    float DistanceToCamera = 0;

                    if (HostActor != null && HostActor.mUpdateAnimByDistance==true && !mTickAllTime)
                    {
                        if (Engine.Instance.Client.MainWorld.Camera != null)
                        {
                            if (HostActor != null && HostActor.Placement != null)
                            {
                                var loc1 = HostActor.Placement.GetLocation();
                                //var loc2 = IEngine.Instance.MainWorld.Camera.GetLocation();
                                var crole = Engine.Instance.Client.ChiefRole;
                                if (crole != null)
                                {
                                    var loc2 = crole.Placement.GetLocation();
                                    loc1.Y = loc2.Y;
                                    DistanceToCamera = SlimDX.Vector3.DistanceSquared(loc1, loc2);
                                }
                            }
                        }
                    }

                    if (DistanceToCamera < RPC.RPCNetworkMgr.Sync2ClientRangeSq)
                    {
                        Engine.Instance.TickAnimationTreeNumber++;
                        m_pAnimTree.Update(tm);
                    }
                    else
                    {
                        //if (m_pAnimTree.mDoFirstTick == false)
                        //{
                        //    m_pAnimTree.Update(tm);
                        //    m_pAnimTree.mDoFirstTick = true;
                        //}

                        mOverRangeAnimTickTime -= (int)tm;
                        if(mOverRangeAnimTickTime < 0)
                        {
                            Engine.Instance.TickAnimationTreeNumber++;
                            m_pAnimTree.Update(tm);
                            //mOverRangeAnimTickTime = (int)(3000 * (dist / 512));
                            mOverRangeAnimTickTime = System.Math.Min((int)(3000 * (DistanceToCamera / 512)), 3000);
                        }
                    }
                    

                    var skl = m_pAnimTree.GetSkeleton();

                    // 将ANIMTREE得到的最终矩阵传给v3dStagedObject里的SKINMODIFIER， 由此MODIFIER用来计算蒙皮	
                    for(int i=0; i<mMeshParts.Count; ++i)
                    {
                        var pSkinModifier = DllImportAPI.v3dStagedObject_QueryModifier(mMeshParts[i].Mesh, vIIDDefine.vIID_v3dSkinModifier);
                        if(pSkinModifier != IntPtr.Zero)
                            DllImportAPI.v3dSkinModifier_SetFullSkeleton(pSkinModifier, skl);
                    }

                    if(mFullSocketTable != null)
                        mFullSocketTable.Update(skl);

                    if(mATFinished != m_pAnimTree.GetATFinished())
                    {
                        if(m_pAnimTree.DelegateOnAnimTreeFinish != null)
                            m_pAnimTree.DelegateOnAnimTreeFinish();
                        mATFinished = m_pAnimTree.GetATFinished();
                    }

                    //if (m_pAnimTree.Action != null)
                    //{
                    //    foreach (var notifyType in mNotifyUpdateTypes)
                    //    {
                    //        var notifys = m_pAnimTree.Action.GetNotifiers(notifyType);
                    //        foreach (var notify in notifys)
                    //        {
                    //            notify.Tick(tm);
                    //        }
                    //    }
                    //}
                }
            }

            bool replaceUpdated = false;
            foreach (var mp in mMeshParts)
            {
                if (mp == null)
                    continue;
                mp.Tick(tm);

                if (mp.UpdateReplaceSource())
                    replaceUpdated = true;
            }

            if (replaceUpdated)
            {
                mIsSkined = false;
                foreach (var mp in mMeshParts)
                {
                    var pSkinModifier = DllImportAPI.v3dStagedObject_QueryModifier(mp.Mesh, vIIDDefine.vIID_v3dSkinModifier);
                    if (pSkinModifier != IntPtr.Zero)
                        mIsSkined = true; 
                }

                if (mNeedCalcFullSkeleton)
                {
                    _BuildFullSkeleton_FromMeshes();
                    if (m_pAnimTree != null)
                        m_pAnimTree.SetSkeleton(mFullSkeleton);
                }
                _BuildFullSocketTable();

                UpdateBoundingBox();
            }

            _UpdateFadeInOut(tm);
        }
        /// <summary>
        /// 上一层的类型
        /// </summary>
        public   RLayer mPreLayer = RLayer.RL_DSBase;
        /// <summary>
        /// 开始渐显
        /// </summary>
        /// <param name="fadeType">渐隐渐显的类型</param>
        public void StartFadeIn(CCore.Mesh.MeshFadeType fadeType)
        {
            if (mStartFadeIn == true)
                return;
            
            if (Layer != RLayer.RL_DSBase &&
                Layer != RLayer.RL_DSTranslucent &&
                Layer != RLayer.RL_DSDecal
                )
                return;

            switch (fadeType)
            {
                case MeshFadeType.FadeIn:
                case MeshFadeType.FadeInOut:
                    {
                        mStartFadeIn = true;
                        mCurrFadeTime = 0;
                        mFadePercent = 0.0f;
                        mPreLayer = Layer;
                        //Layer = RLayer.RL_PreTranslucent;
                        Layer = RLayer.RL_DSPost;
                    }
                    break;
            }

            mSocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent socket, object arg) =>
            {
                socket.StartSocketComponentFadeIn(fadeType);
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }
        /// <summary>
        /// 开始渐隐
        /// </summary>
        /// <param name="fadeType">渐隐方式</param>
        public virtual void StartFadeOut(CCore.Mesh.MeshFadeType fadeType)
        {
            if (mStartFadeOut == true)
                return;
            
            if (Layer != RLayer.RL_DSBase &&
                Layer != RLayer.RL_DSTranslucent &&
                Layer != RLayer.RL_DSDecal
                )
                return;

            switch (fadeType)
            {
                case MeshFadeType.FadeOut:
                case MeshFadeType.FadeInOut:
                    {
                        mStartFadeOut = true;
                        mCurrFadeTime = 0;
                        mFadePercent = 1.0f;
                        mPreLayer = Layer;
                        //Layer = RLayer.RL_PreTranslucent;
                        Layer = RLayer.RL_DSPost;
                    }
                    break;
            }

            mSocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent socket, object arg) =>
            {
                socket.StartSocketComponentFadeOut(fadeType);
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
            /*foreach (var socket in mSocketItems)
            {
                foreach (var effect in socket.ParticleEffects)
                {
                    if (effect == null)
                        continue;

                    effect.EffectTemplate.EnableEmitter = false;
                    foreach (var meshData in effect.MeshDataList)
                    {
                        meshData.Mesh.Visible = meshData.Modifier.DelayDead;
                    }
                }

                for (int i = 0; i < socket.LightActors.Count; ++i)
                {
                    if (socket.LightActors[i] == null)
                        continue;

                    socket.LightActors.RemoveAt(i);
                    //                    socket.LightActors[i].Light.Visible = false;
                }
            }*/
        }
        /// <summary>
        /// 结束渐隐渐显模式
        /// </summary>
        public void EndFadeInOut()
        {
            if(mStartFadeIn==true)
            {
                mFadePercent = 1.0f;
                Layer = mPreLayer;
            }
            if (mStartFadeOut == true)
                mFadePercent = 0.0f;

            mStartFadeIn = false;
            mStartFadeOut = false;
            mCurrFadeTime = 0;
        }
        /// <summary>
        /// 更新渐隐渐显模式
        /// </summary>
        /// <param name="tm">更新时间</param>
        public void _UpdateFadeInOut(Int64 tm)
        {
            var meshInit = mVisualInit as CCore.Mesh.MeshInit;
            if (meshInit == null)
                return;

            if (mStartFadeIn == true && mCurrFadeTime !=-1)
            {
                if (mCurrFadeTime > meshInit.mFadeInTime)
                {
                    EndFadeInOut();
                }
                else
                {
                    mFadePercent = (float)mCurrFadeTime / (float)meshInit.mFadeInTime;
                    mFadePercent = System.Math.Min(1, mFadePercent);
                }
                mCurrFadeTime += tm;
            }
            else if (mStartFadeOut == true && mCurrFadeTime !=-1)
            {
                if (mCurrFadeTime > meshInit.mFadeOutTime)
                {
                    EndFadeInOut();
                }
                else
                {
                    mFadePercent = 1.0f - (float)mCurrFadeTime / (float)meshInit.mFadeOutTime;
                    mFadePercent = System.Math.Max(0, mFadePercent);
                }
                mCurrFadeTime += tm;
            }

            mSocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent socket, object arg) =>
            {
                socket.UpdateSocketComponentFadeInOut(FadePercent);
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }

        static CSUtility.Performance.PerfCounter mMeshUpdateTimer = new CSUtility.Performance.PerfCounter("Mesh.Update");
        static CSUtility.Performance.PerfCounter mMeshSocketTimer = new CSUtility.Performance.PerfCounter("Mesh.Socket");
        UInt32 mCurActionSourceVer = 0;
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="host">所属的Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            mMeshUpdateTimer.Begin();
            Update(elapsedMillisecond);
            mMeshUpdateTimer.End();
            
            if (mSocketComponents.Count > 0)
            {
                mSocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent socket, object arg) =>
                    {
                        mMeshSocketTimer.Begin();
                        socket.SocketComponentTick(host, elapsedMillisecond);
                        mMeshSocketTimer.End();
                        return CSUtility.Support.EForEachResult.FER_Continue;
                    }, null);
            }

            var meshInit = VisualInit as MeshInit;
            if (meshInit != null && meshInit.MeshTemplate != null && HostActor != null)
            {
                if (mVer != meshInit.MeshTemplate.Ver)
                {
                    // 重新初始化mesh
                    meshInit.MeshTemplateID = meshInit.MeshTemplate.MeshID;
                    Initialize(meshInit, HostActor);
                    SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(HostActor.Id));

                    if (OnMeshReInitialized != null)
                        OnMeshReInitialized(this);
                }
            }

            // Tick当前动作的刀光Notify
            if (GetAnimTree() != null)
            {
                var anim = GetAnimTree().Action as CCore.AnimTree.AnimTreeNode_Action;
                if (anim != null)
                {    
                    Int64 currTime = 0;
                    if (anim.Duration != 0)
                        currTime = anim.CurAnimTime % anim.Duration;

                    // 刀光
                    /*var ntf = anim.GetNotifier("TrailStart");
                    if (ntf != null)
                    {
                        var nplist = ntf.GetNotifyPoints(mPrevAnimTime, currTime);
                        if (nplist != null)
                        {
                            foreach (var i in nplist)
                            {
                                EnableTrail = true;
                            }
                        }
                    }
                    ntf = anim.GetNotifier("TrailEnd");
                    if (ntf != null)
                    {
                        var nplist = ntf.GetNotifyPoints(mPrevAnimTime, currTime);
                        if (nplist != null)
                        {
                            foreach (var i in nplist)
                            {
                                EnableTrail = false;
                            }
                        }
                    }*/

                    // Tick Effect
        
                    if (anim.ActionSource != null)
                    {
                        if (anim.ActionSource.Ver != mCurActionSourceVer)
                        {
                            mCurActionSourceVer = anim.ActionSource.Ver;
                            InitializeNotifys(GetAnimTree());
                        }
                    }
                                                                                                                            
                    foreach (var effectNotify in mEffectNotifiers)
                    {
                        effectNotify.UpdateEffectActive(mPrevAnimTime, currTime);
                    }

                    foreach (var notify in anim.GetNotifiers())
                    {
                        notify.UpdateActive(host, mPrevAnimTime, currTime);
                    }

                    mPrevAnimTime = currTime;
                }
            }
        }

        // 计算新的骨架。当换装时，重新计算骨架。
        /// <summary>
        /// 计算新的骨架。当换装时，重新计算骨架。
        /// </summary>
        public void _BuildFullSkeleton_FromMeshes()
        {
            if(mIsSkined == false)
                return;

            // 将SUBSKELETON组装成FULLSKELETON
		    // 更新SKINMODIFIER的REFSKELETON
            mFullSkeleton = new CCore.Skeleton.Skeleton();
            for(int i=0; i<mMeshParts.Count; ++i)
            {
                var pSkinModifier = DllImportAPI.v3dStagedObject_QueryModifier(mMeshParts[i].Mesh, vIIDDefine.vIID_v3dSkinModifier);
                if(pSkinModifier != IntPtr.Zero)
                {
                    var pSubSkeleton = DllImportAPI.v3dSkinModifier_GetSubSkeleton(pSkinModifier);
                    if(pSubSkeleton != IntPtr.Zero)
                        mFullSkeleton.Merge(pSubSkeleton);
                }
            }
            mFullSkeleton.BuildHiberarchys();

            // 将FULLSKELETON设置给动画树
            if(m_pAnimTree != null)
                m_pAnimTree.SetSkeleton(mFullSkeleton);
        }
        /// <summary>
        /// 重新计算挂接组件
        /// </summary>
		public void _BuildFullSocketTable()
        {
            if(mMeshParts.Count == 0)
                return;

            mFullSocketTable = new CCore.Socket.SocketTable();

            if(mIsSkined == true)
            {
                for(int i=0; i<mMeshParts.Count; ++i)
                {
                    var subSocketTable = mMeshParts[i].SocketTable;
                    if(subSocketTable != null)
                    {
                        mFullSocketTable.Merge(subSocketTable);
                    }
                }
                if(mFullSocketTable.GetSocketCount() > 0 && mFullSkeleton != null)
                {
                    if(mFullSkeleton != null)
                        mFullSocketTable.Build(mFullSkeleton.Inner);
                }
            }
            else
            {
                for(int i=0; i<mMeshParts.Count; ++i)
                {
                    var subSocketTable = mMeshParts[i].SocketTable;
                    if(subSocketTable != null)
                    {
                        for(int j=0; j<subSocketTable.GetSocketCount(); ++j)
                        {
                            mFullSocketTable.AddSocket(subSocketTable.GetSocket(j).Inner);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 转换LOL模型
        /// </summary>
        /// <param name="SknPaths">皮肤路径列表</param>
        /// <param name="SklPaths">骨骼路径列表</param>
        /// <param name="AnmNames">动画名称</param>
        /// <param name="bMakeMeshTemplate">是否制作mesh模板</param>
        /// <param name="TexPaths">纹理路径列表</param>
        /// <returns>转换成功返回true，否则返回false</returns>
        public static bool ConvertLOLModels(List<string> SknPaths, List<string> SklPaths, List<string> AnmNames, bool bMakeMeshTemplate, List<string> TexPaths)
        {
            // renwind test 先导出少量的资源测试
            if (SknPaths.Count > 1)
                return false;

            // 计算出角色名
            string roleName = "";
            {
                int iEndPos = SknPaths[0].LastIndexOf('.');
                int iBeginPos = SknPaths[0].LastIndexOf("\\") + 1;
                roleName = SknPaths[0].Substring(iBeginPos, iEndPos - iBeginPos);
            }

            //string roleName = "";
            //{
            //    int iEndPos = SknNames[0].LastIndexOf('.');
            //    int iBeginPos = SknNames[0].LastIndexOf("\\") + 1;
            //    roleName = SknNames[0].Substring(iBeginPos, iEndPos - iBeginPos);
            //    string vmsPath = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath("Mesh/Character/" + roleName + "/");
            //    System.IO.Directory.CreateDirectory(vmsPath);
            //    string vmsName = vmsPath + roleName + ".vms";

            //    ConvertLOLModel(SknNames[0], SklNames[0], vmsName);

            //}
            Guid tempGUID = new Guid();
            List<MeshTemplate> meshTemplates = new List<MeshTemplate>();
            // 计算vms的绝对路径，并转换，可能有多个vms
            for (int i = 0; i < SknPaths.Count; ++i)
            {
                string sknPath = SknPaths[i];
                string sklPath = SklPaths[i];

                string vmsPath = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath("Mesh/Character/" + roleName + "/");

                int iEndPos = sknPath.LastIndexOf('.');
                int iBeginPos = sknPath.LastIndexOf("\\") + 1;
                string sknName = sknPath.Substring(iBeginPos, iEndPos - iBeginPos);
                string vmsName = vmsPath + sknName + CSUtility.Support.IFileConfig.MeshSourceExtension;

                System.IO.Directory.CreateDirectory(vmsPath);
                List<String> outMatNames = new List<String>();
                bool bConvert = ConvertLOLModel(sknPath, sklPath, vmsName, outMatNames);
                if (bConvert == false)
                {
                    try
                    {
                        System.IO.Directory.Delete(vmsPath);
                    }
                    catch (System.Exception ex)
                    {
                        CCore.Support.MessageBox.Show("ConvertLOLModels Error: " + ex.Message);
                    }
                    return bConvert;
                }

                // 生成MeshTemplate
                if (bMakeMeshTemplate)
                {
                    string vmsReleativeName = "Mesh/Character/" + roleName + "/" + sknName + CSUtility.Support.IFileConfig.MeshSourceExtension;
                    Guid guid = Guid.NewGuid();
                    tempGUID = guid;
                    CCore.Mesh.MeshTemplateMgr.Instance.SaveMeshTemplate(guid);
                    var mt = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(guid);
                    meshTemplates.Add(mt);
                    mt.NickName = sknName;
                    var initPart = new CCore.Mesh.MeshInitPart();
                    initPart.MeshName = vmsReleativeName;
                    foreach (var mtl in outMatNames)
                    {
                        //initPart.Materials.Add("1T_Character.mtl");
                        //initPart.Techs.Add(sknName);
                    }
                    mt.MeshInitList.Add(initPart);
                    if (AnmNames.Count > 0)
                    {
                        //mt.ActionName = vmaReleativeName;
                        mt.NeedCalcFullSkeleton = true;
                    }
                }

            }

            // 解析当前SKN目录里的贴图文件
            TexPaths.AddRange(System.IO.Directory.GetFiles(SknPaths[0].Substring(0, SknPaths[0].IndexOf("\\")), "*.dds", System.IO.SearchOption.TopDirectoryOnly));

            string vmaReleativeName = "";
            int iTemp = 0;
            foreach (var AnmName in AnmNames)
            {
                int iEndPos = AnmName.LastIndexOf('.');
                int iBeginPos = AnmName.LastIndexOf("\\") + 1;
                string anmName = AnmName.Substring(iBeginPos, iEndPos - iBeginPos);
                string anmPath = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath("Action/Character/" + roleName + "/");
                System.IO.Directory.CreateDirectory(anmPath);
                string vmaName = anmPath + anmName + ".vma";

                ConvertLOLAnim(AnmName, SklPaths[0], vmaName);

                // 默认使用第一个动作生成MeshTemplate
                if (iTemp == 0)
                    vmaReleativeName = "Action/Character/" + roleName + "/" + anmName + ".vma";
                iTemp++;
            }

            foreach (var mt in meshTemplates)
            {
                mt.ActionName = vmaReleativeName;
                mt.IsDirty = true;
            }

            return true;
        }
        /// <summary>
        /// 转换LOL模型
        /// </summary>
        /// <param name="SknName">皮肤的名称</param>
        /// <param name="SklName">骨骼名称</param>
        /// <param name="DestName">目标名称</param>
        /// <param name="outTexs">输出的纹理列表</param>
        /// <returns>转换成功返回true，否则返回false</returns>
        public static bool ConvertLOLModel(string SknName, string SklName, string DestName, List<string> outTexs )
        {
            //std::vector<LPCSTR> outTexNames;
            //bool bRet =  IEngine::Instance->Client->Graphics->Device->GetVMObjMgr()->ConvertOLModel(StringManage2Native(SknName), StringManage2Native(SklName), StringManage2Native(DestName), outTexNames );
            //for(size_t i = 0; i < outTexNames.size(); ++i)
            //{
            //    outTexs->Add(gcnew System::String(outTexNames[i]));
            //}
            //return bRet;
            return false;
        }
        /// <summary>
        /// 转换LOL动画
        /// </summary>
        /// <param name="AnmName">动画名称</param>
        /// <param name="SklName">骨骼名称</param>
        /// <param name="DestName">目标名称</param>
        /// <returns>转换成功返回true，否则返回false</returns>
		public static bool ConvertLOLAnim(string AnmName, string SklName, string DestName )
        {
            //		return IEngine::Instance->Client->Graphics->Device->GetVMObjMgr()->ConvertOLAnim(StringManage2Native(AnmName), StringManage2Native(SklName), StringManage2Native(DestName) );
            return false;
        }


		//IParticleModifier^ AddParticleModifier();
        /// <summary>
        /// 设置粒子模拟器
        /// </summary>
        /// <param name="modifier">模拟器的对象</param>
        public void SetParticleModifier(CCore.Modifier.MeshModifier modifier)
        {
            foreach (var mp in mMeshParts)
            {
                // 插入PNTModifier之前
                DllImportAPI.v3dStagedObject_ModStacks_InsertModifier(mp.Mesh, modifier.Modifier, vIIDDefine.vIID_v3dPNTModifier, true);
            }
        }
        /// <summary>
        /// 强制重新加载mesh
        /// </summary>
        /// <param name="name">加载的mesh名称</param>
        public static void ForceReloadMesh(string name)
        {
            DllImportAPI.v3dDevice_VMObjMgr_ForceReloadModelSource(Engine.Instance.Client.Graphics.Device, name);
        }
        /// <summary>
        /// 获取所在层的名称
        /// </summary>
        /// <returns>返回层的名称</returns>
        public override string GetLayerName()
        {
            var meshInit = VisualInit as MeshInit;
            if (meshInit != null && meshInit.MeshTemplate != null)
            {
                return meshInit.MeshTemplate.LayerName;
            }

            return "Other";
        }
        /// <summary>
        /// 获取mesh的顶点数量
        /// </summary>
        /// <returns>返回mesh的顶点数量</returns>
        public UInt32 GetMeshVertexNumber()
        {
            unsafe
            {
                UInt32 retValue = 0;
                foreach (var mp in mMeshParts)
                {
                    var desc = new MeshDesc();
                    DllImportAPI.v3dStagedObject_GetModelDesc(mp.Mesh, &desc);

                    retValue = desc.VertexNumber;
                }

                return retValue;
            }
        }
        /// <summary>
        /// 获取mesh多边形的数量
        /// </summary>
        /// <returns>返回mesh多边形的数量</returns>
        public UInt32 GetMeshPolyNumber()
        {
            unsafe
            {
                UInt32 retValue = 0;
                foreach (var mp in mMeshParts)
                {
                    var desc = new MeshDesc();
                    DllImportAPI.v3dStagedObject_GetModelDesc(mp.Mesh, &desc);

                    retValue = desc.PolyNumber;
                }

                return retValue;
            }
        }
        /// <summary>
        /// 获取mesh原子的数量
        /// </summary>
        /// <returns>返回mesh原子的数量</returns>
        public UInt32 GetMeshAtomNumber()
        {
            unsafe
            {
                UInt32 retValue = 0;
                foreach (var mp in mMeshParts)
                {
                    var desc = new MeshDesc();
                    DllImportAPI.v3dStagedObject_GetModelDesc(mp.Mesh, &desc);

                    retValue = desc.AtomNumber;
                }

                return retValue;
            }
        }
    }
}
