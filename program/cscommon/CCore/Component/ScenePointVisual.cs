using System;

namespace CCore.Component
{
    /// <summary>
    /// 可视化场景点
    /// </summary>
    public class ScenePointVisual : Visual
    {
        CCore.Mesh.Mesh mShowMesh;

        CSUtility.Map.ScenePointGroup.enScenePointGroupType mSPType = CSUtility.Map.ScenePointGroup.enScenePointGroupType.ScenePoint;
        /// <summary>
        /// 场景点组的类型
        /// </summary>
        public CSUtility.Map.ScenePointGroup.enScenePointGroupType PointType
        {
            get { return mSPType; }
            set
            {
                mSPType = value;

                Guid hostId = Guid.Empty;
                if (mHostActor != null)
                    hostId = mHostActor.Id;

                var meshInit = new CCore.Mesh.MeshInit();
                Guid meshGUID = Guid.Empty;
                switch (mSPType)
                {
                    case CSUtility.Map.ScenePointGroup.enScenePointGroupType.PatrolPoint:
                        {
                            meshGUID = CSUtility.Support.IFileConfig.PatrolPointMesh;
                        }
                        break;

                    case CSUtility.Map.ScenePointGroup.enScenePointGroupType.CameraPoint:
                        {
                            meshGUID = CSUtility.Support.IFileConfig.CameraMesh;
                        }
                        break;

                    case CSUtility.Map.ScenePointGroup.enScenePointGroupType.NavigationPoint:
                        {
                            meshGUID = CSUtility.Support.IFileConfig.NavigationPointMesh;
                        }
                        break;

                    case CSUtility.Map.ScenePointGroup.enScenePointGroupType.ScenePoint:
                    default:
                        {
                            meshGUID = CSUtility.Support.IFileConfig.ScenePointMesh;
                        }
                        break;
                }
                meshInit.MeshTemplateID = meshGUID;
                mShowMesh = new CCore.Mesh.Mesh();
                mShowMesh.Initialize(meshInit, null);
                mShowMesh.CanHitProxy = true;
                if (hostId != Guid.Empty)
                {
                    mShowMesh.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(hostId));
                }
            }
        }
        /// <summary>
        /// 可视化场景点的构造函数，将场景点类型设置为ScenePoint
        /// </summary>
        public ScenePointVisual()
        {
            mLayer = CCore.RLayer.RL_SystemHelper;

            PointType = CSUtility.Map.ScenePointGroup.enScenePointGroupType.ScenePoint;
        }
        /// <summary>
        /// 将场景点提交到场景中
        /// </summary>
        /// <param name="renderEnv">提交到的场景</param>
        /// <param name="matrix">场景点的矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            switch (PointType)
            {
                case CSUtility.Map.ScenePointGroup.enScenePointGroupType.NavigationPoint:
                    {
                        if (!CCore.Program.IsActorTypeShow(HostActor.World, CCore.Program.NavigationPointTypeName))
                            return;
                    }
                    break;

                default:
                    {
                        if (!CCore.Program.IsActorTypeShow(HostActor.World, CCore.Program.ScenePointTypeName))
                            return;
                    }
                    break;
            }

            if (Visible == false)
                return;

            if (mShowMesh == null)
                return;

            mShowMesh.Commit(renderEnv, ref matrix, eye);
        }
        /// <summary>
        /// 设置场景点是否可用鼠标进行点击
        /// </summary>
        /// <param name="hitProxy">点击代理</param>
        public override void SetHitProxyAll(uint hitProxy)
        {
            if(mShowMesh != null)
                mShowMesh.SetHitProxyAll(hitProxy);
        }
    }
}
