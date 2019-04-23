using System;
using System.Collections.Generic;

namespace CCore.Component
{
    /// <summary>
    /// 可视化角色Actor的初始化类
    /// </summary>
    public class RoleActorVisualInit : VisualInit
    {
        List<Guid> mMeshTemplateIds = new List<Guid>();
        /// <summary>
        /// mesh模板的ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public List<Guid> MeshTemplateIds
        {
            get { return mMeshTemplateIds; }
            set { mMeshTemplateIds = value; }
        }

        bool mCanHitProxy = false;
        /// <summary>
        /// 是否可点击
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public bool CanHitProxy
        {
            get { return mCanHitProxy; }
            set { mCanHitProxy = value; }
        }

        float mRimStart = 0.5f;
        /// <summary>
        /// 轮廓
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public float RimStart
        {
            get { return mRimStart; }
            set { mRimStart = value; }
        }
        /// <summary>
        /// 轮廓
        /// </summary>
        public float mRimEnd = 1.0f;
        /// <summary>
        /// 轮廓修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public float RimEnd
        {
            get { return mRimEnd; }
            set { mRimEnd = value; }
        }
        /// <summary>
        /// 轮廓的颜色
        /// </summary>
        public SlimDX.Vector4 mRimColor = new SlimDX.Vector4(1, 1, 1, 1);
        /// <summary>
        /// 轮廓的颜色
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public SlimDX.Vector4 RimColor
        {
            get { return mRimColor; }
            set { mRimColor = value; }
        }
        /// <summary>
        /// 轮廓的混合度
        /// </summary>
        public float mRimMultiply = 1.0f;
        /// <summary>
        /// 轮廓混合度
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public float RimMultiply
        {
            get { return mRimMultiply; }
            set { mRimMultiply = value; }
        }
        /// <summary>
        /// 轮廓的闪亮度
        /// </summary>
        public int mIsRimBloom = 0;
        /// <summary>
        /// 轮廓的闪亮度
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public int IsRimBloom
        {
            get { return mIsRimBloom; }
            set { mIsRimBloom = value; }
        }
    }
    /// <summary>
    /// 可视化角色Actor类
    /// </summary>
    public class RoleActorVisual : CCore.Mesh.Mesh
    {
        /// <summary>
        /// mesh模板数据
        /// </summary>
        class MeshTemplateData
        {
            /// <summary>
            /// 模板的版本
            /// </summary>
            public UInt32 Ver;
            /// <summary>
            /// mesh模板
            /// </summary>
            public CCore.Mesh.MeshTemplate MeshTemplate;
        }

        List<Guid> mAddedSocketList = new List<Guid>();
        List<MeshTemplateData> mUsedMeshTemplateList = new List<MeshTemplateData>();
        RoleActorVisualInit mRoleActorInit = null;
        /// <summary>
        /// 可视化角色Actor的初始化
        /// </summary>
        public RoleActorVisualInit RoleActorInit
        {
            get { return mRoleActorInit; }
        }
        /// <summary>
        /// 是否进行勾边
        /// </summary>
        public bool EdgeDetect
        {
            get { return mEdgeDetect; }
            set 
            { 
                // 将自己和挂接的SocketMesh标识为勾边
                mEdgeDetect = value;
                SocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent socketItem, object arg) =>
                {
                    var mesh = socketItem as Mesh.Mesh;
                    if (mesh == null)
                        return CSUtility.Support.EForEachResult.FER_Continue;

                    //if (mesh.IsTrail != true)
                    //{
                    mesh.mEdgeDetect = value;
                    mesh.EdgeDetectColor = EdgeDetectColor;
                    mesh.EdgeDetectMode = EdgeDetectMode;
                    mesh.EdgeDetectHightlight = EdgeDetectHightlight;
                    //}

                    return CSUtility.Support.EForEachResult.FER_Continue;
                }, null);
            }
        }
        float mTransPercent =1;
        /// <summary>
        ///  转换率
        /// </summary>
        public float TransPercent
        {
            get { return mTransPercent; }
            set 
            {
                mTransPercent = value;
                SetMeshTransPercent(this, value);

                SocketComponents.For_Each((Guid id, Socket.ISocketComponent comp, object arg) =>
                {
                    var mesh = comp as Mesh.Mesh;
                    if (mesh == null)
                        return CSUtility.Support.EForEachResult.FER_Continue;

                    SetMeshTransPercent(mesh, value);
                    return CSUtility.Support.EForEachResult.FER_Continue;
                }, null);
            }
        }
        /// <summary>
        /// 设置轮廓的高亮参数
        /// </summary>
        /// <param name="rimStart">轮廓的起始</param>
        /// <param name="rimEnd">轮廓的结尾</param>
        /// <param name="rimColor">轮廓的颜色</param>
        /// <param name="rimMultiply">轮廓的混合度</param>
        /// <param name="rimBloom">轮廓闪亮度</param>
        public void SetRimLightParameter(float rimStart, float rimEnd, SlimDX.Vector4 rimColor, float rimMultiply, int rimBloom)
        {
            mRimStart = rimStart;
            mRimEnd = rimEnd;
            mRimColor = rimColor;
            mRimMultiply = rimMultiply;
            mIsRimBloom = rimBloom;

            SocketComponents.For_Each((Guid id, Socket.ISocketComponent comp, object arg) =>
            {
                var mesh = comp as Mesh.Mesh;
                if (mesh == null)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                mesh.mRimColor = rimColor;

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }        
        /// <summary>
        /// 更新轮廓的参数
        /// </summary>
        public override void UpdateRimParameter()
        {            
            base.UpdateRimParameter();            
            SocketComponents.For_Each((Guid id, Socket.ISocketComponent comp, object arg) =>
            {
                var mesh = comp as Mesh.Mesh;
                if (mesh == null)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                mesh.UpdateRimParameter();

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);            
        }
        /// <summary>
        /// 设置mesh的转换比例
        /// </summary>
        /// <param name="mesh">mesh对象</param>
        /// <param name="value">比例值</param>
        public void SetMeshTransPercent(CCore.Mesh.Mesh mesh,float value)
        {
            if (value != 1)
            {
                mesh.mStartFadeIn = true;
                mesh.FadePercent = value;
                mesh.mCurrFadeTime = -1;
                mesh.mPreLayer = mesh.Layer;
                mesh.Layer = RLayer.RL_DSPost;
                if (value ==0)
                {
                    var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;
                    if (meshInit != null)
                    {
                        meshInit.CastShadow = false;
                        mesh.Visible = false;
                    }
                }
            }
            else
            {
                mesh.mStartFadeIn = false;
                mesh.FadePercent = 1;
                mesh.mCurrFadeTime = 0;
                mesh.Layer = mesh.mPreLayer;
                var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;
                if (meshInit != null)
                {
                    meshInit.CastShadow = true;
                    mesh.Visible = true;
                }
            }
        }
        /// <summary>
        /// 角色Actor的初始化
        /// </summary>
        /// <param name="_init">角色Actor的初始化类</param>
        /// <param name="host">所属Actor</param>
        /// <returns>初始化成功返回true</returns>
        public override bool Initialize(CCore.Component.VisualInit _init, CCore.World.Actor host)
        {
            mRoleActorInit = _init as RoleActorVisualInit;

            var meshInit = new CCore.Mesh.MeshInit();
            meshInit.CanHitProxy = mRoleActorInit.CanHitProxy;
            meshInit.m_bNeedCalcFullSkeleton = true;

            mUsedMeshTemplateList.Clear();
            foreach (var meshId in mRoleActorInit.MeshTemplateIds)
            {
                var mt = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(meshId);
                if (mt == null)
                    continue;

                MeshTemplateData data = new MeshTemplateData();
                data.MeshTemplate = mt;
                data.Ver = mt.Ver;
                mUsedMeshTemplateList.Add(data);

                // 保存MeshPart是从哪个MeshTemplate来的
                foreach (var part in mt.MeshInitList)
                {
                    part.OwnerMeshId = meshId;
                }
                meshInit.MeshInitParts.AddRange(mt.MeshInitList);
            }

            base.Initialize(meshInit, host);
            mRimStart = mRoleActorInit.RimStart;
            mRimEnd = mRoleActorInit.RimEnd;
            mRimColor = mRoleActorInit.RimColor;
            mRimMultiply = mRoleActorInit.RimMultiply;
            mIsRimBloom = mRoleActorInit.IsRimBloom;

            foreach (var addedSocket in mAddedSocketList)
            {
                RemoveSocketItem(addedSocket);
            }
            mAddedSocketList.Clear();
            foreach (var meshId in mRoleActorInit.MeshTemplateIds)
            {
                var mt = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(meshId);
                if (mt == null)
                    continue;

                mt.SocketComponentInfoList.For_Each((Guid id, CCore.Socket.ISocketComponentInfo socketInfo, object arg) =>
                {
                    var comp = AddSocketItem(socketInfo);
                    if(comp != null)
                        mAddedSocketList.Add(comp.SocketComponentInfo.SocketComponentInfoId);
                    return CSUtility.Support.EForEachResult.FER_Continue;
                }, null);
            }

            if (mUsedMeshTemplateList.Count > 0)
            {
                var actionName = mUsedMeshTemplateList[0].MeshTemplate.ActionName;
                var playRate = mUsedMeshTemplateList[0].MeshTemplate.PlayRate;
                if (!string.IsNullOrEmpty(actionName))
                {
                    var actionNode = new CCore.AnimTree.AnimTreeNode_Action();
                    actionNode.Initialize();
                    SetAnimTree(actionNode);
                    actionNode.ClearLink();
                    actionNode.SetAction(actionName);
                    actionNode.PlayRate = playRate;
                }
            }

            return true;
        }
        /// <summary>
        /// 将该对象提交到渲染环境
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, Camera.CameraObject eye)
        {
            switch((CSUtility.Component.EActorGameType)HostActor.GameType)
            {
                case CSUtility.Component.EActorGameType.NpcInitializer:
                    {
                        if (!CCore.Program.IsActorTypeShow(HostActor.World, CCore.Program.NPCInitializerAssistTypeName))
                            return;
                    }                    
                    break;
            }
            base.Commit(renderEnv, ref matrix, eye);                        
        }

        //public void SetAnimTree(CSCommon.Animation.AnimationTree animtree)
        //{
        //    foreach (var mesh in mMeshList)
        //    {
        //        mesh.SetAnimTree(animtree as MidLayer.IAnimTreeNode);
        //    }
        //}

        //public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        //{
        //    vMin.X = float.MaxValue;
        //    vMin.Y = float.MaxValue;
        //    vMin.Z = float.MaxValue;
        //    vMax.X = float.MinValue;
        //    vMax.Y = float.MinValue;
        //    vMax.Z = float.MinValue;

        //    foreach (var mesh in mMeshList)
        //    {
        //        SlimDX.Vector3 meshMin = SlimDX.Vector3.UnitXYZ;
        //        SlimDX.Vector3 meshMax = SlimDX.Vector3.UnitXYZ;
        //        mesh.GetAABB(ref meshMin, ref meshMax);

        //        if (vMax.X < meshMax.X)
        //            vMax.X = meshMax.X;
        //        if (vMax.Y < meshMax.Y)
        //            vMax.Y = meshMax.Y;
        //        if (vMax.Z < meshMax.Z)
        //            vMax.Z = meshMax.Z;

        //        if (vMin.X > meshMin.X)
        //            vMin.X = meshMin.X;
        //        if (vMin.Y > meshMin.Y)
        //            vMin.Y = meshMin.Y;
        //        if (vMin.Z > meshMin.Z)
        //            vMin.Z = meshMin.Z;
        //    }
        //}        
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="host">所属的Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {            
            base.Tick(host, elapsedMillisecond);            

            //foreach (var mesh in mMeshList)
            //{
            //    mesh.Tick(host, elapsedMillisecond);
            //}
            foreach (var data in mUsedMeshTemplateList)
            {
                if (data.Ver != data.MeshTemplate.Ver)
                {
                    Initialize(mRoleActorInit, host as CCore.World.Actor);
                    SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(host.Id));
                    break;
                }
            }
        }

        //public void Update(Int64 tm)
        //{
        //    foreach (var mesh in mMeshList)
        //    {
        //        mesh.Update(tm); 
        //    }
        //}

        //public void Preuse(bool bForce, Int64 time)
        //{
        //    foreach (var mesh in mMeshList)
        //    {
        //        mesh.Preuse(bForce, time);
        //    }
        //}

        //public override void SetHitProxyAll(uint hitProxy)
        //{
        //    foreach (var mesh in mMeshList)
        //    {
        //        mesh.SetHitProxyAll(hitProxy);
        //    }
        //}
    }
}
