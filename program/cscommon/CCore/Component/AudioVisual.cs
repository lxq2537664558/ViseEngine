using System;

namespace CCore.Component
{
    /// <summary>
    /// 可视化音频初始化类，用于创建音频时使用
    /// </summary>
    public class AudioVisualInit : VisualInit
    {
        /// <summary>
        /// 音频源数据
        /// </summary>
        protected CCore.Audio.AudioSourceData mSourceData = new CCore.Audio.AudioSourceData();
        /// <summary>
        /// 访问音频源数据
        /// </summary>
        public CCore.Audio.AudioSourceData SourceData
        {
            get { return mSourceData; }
            set { mSourceData = value; }
        }
    }
    /// <summary>
    /// 可视化音频显示类，用于显示音频的图标
    /// </summary>
    public class AudioVisual : Visual
    {
        AudioVisualInit mAudioVisualInit;
        /// <summary>
        /// 得到可视化音频的初始化类
        /// </summary>
        public AudioVisualInit AudioVisualInit
        {
            get { return mAudioVisualInit; }
        }
        /// <summary>
        /// 得到音频源数据
        /// </summary>
        public CCore.Audio.AudioSourceData AudioSourceData
        {
            get
            {
                if (mAudioVisualInit != null)
                    return mAudioVisualInit.SourceData;

                return null;
            }
        }
        CCore.Mesh.Mesh mAudioObjMesh = new CCore.Mesh.Mesh();
        CCore.Mesh.Mesh mAudioOutRangeMesh = new CCore.Mesh.Mesh();
        CCore.Mesh.Mesh mAudioInRangeMesh = new CCore.Mesh.Mesh();
        /// <summary>
        /// 可视化音频初始化，每次创建时需调用
        /// </summary>
        /// <param name="_init">可视化音频初始化类实例，填写默认数据</param>
        /// <param name="host">绑定可视化音频的Actor</param>
        /// <returns>初始化成功返回true</returns>
        public override bool Initialize(VisualInit _init, CCore.World.Actor host)
        {
            base.Initialize(_init, host);
            mAudioVisualInit = _init as AudioVisualInit;
            
            CCore.Mesh.MeshInit meshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = CSUtility.Support.IFileConfig.AudioObjectMeshTemplate,
                CanHitProxy = true
            };
            mAudioObjMesh.Initialize(meshInit, host);
            if(host != null)
                mAudioObjMesh.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(host.Id));

            meshInit = new Mesh.MeshInit()
            {
                MeshTemplateID = CSUtility.Support.IFileConfig.AudioInRangeMeshTemplate
            };
            mAudioInRangeMesh.Initialize(meshInit, host);

            meshInit = new Mesh.MeshInit()
            {
                MeshTemplateID = CSUtility.Support.IFileConfig.AudioOutRangeMeshTemplate
            };
            mAudioOutRangeMesh.Initialize(meshInit, host);

            return true;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="host">父Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            if (mAudioVisualInit == null)
                return;

            if (host != null && host.Placement != null)
            {
                mAudioVisualInit.SourceData.Position = host.Placement.GetLocation();
            }
        }
        /// <summary>
        /// 将设置好的可视化音频提交给World，以便处理
        /// </summary>
        /// <param name="renderEnv">当前要提交的环境</param>
        /// <param name="matrix">AudioVisual的矩阵</param>
        /// <param name="eye">视野信息</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if (mAudioVisualInit == null)
                return;

            //if (CCore.Engine.Instance.Client.MainWorld.GetEditorObjShow((UInt16)CSUtility.Component.EActorGameType.Sound))
            if(CCore.Program.IsActorTypeShow(CCore.Engine.Instance.Client.MainWorld, CCore.Program.SoundTypeName))
            {
                var tagPos = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Zero, matrix);

                // 根据屏幕计算缩放
                var size = eye.GetScreenSizeInWorld(tagPos, 0.05f);
                var mat = SlimDX.Matrix.Scaling(size * SlimDX.Vector3.UnitXYZ);
                var tagMat = SlimDX.Matrix.Translation(tagPos);
                var tempMat = mat * tagMat;
                mAudioObjMesh.Commit(renderEnv, ref tempMat, eye);

                var radius = mAudioVisualInit.SourceData.MinDistance;
                var inMat = SlimDX.Matrix.Multiply(SlimDX.Matrix.Scaling(radius, radius, radius), tagMat);
                mAudioInRangeMesh.Commit(renderEnv, ref inMat, eye);
                radius = mAudioVisualInit.SourceData.MaxDistance;
                var outMat = SlimDX.Matrix.Multiply(SlimDX.Matrix.Scaling(radius, radius, radius), tagMat);
                mAudioOutRangeMesh.Commit(renderEnv, ref outMat, eye);
            }
        }
    }
}
