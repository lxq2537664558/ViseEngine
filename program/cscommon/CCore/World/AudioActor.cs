using System;
/// <summary>
/// 世界的命名空间
/// </summary>
namespace CCore.World
{
    /// <summary>
    /// 媒体Actor的初始化类
    /// </summary>
    public class AudioActorInit : CCore.World.ActorInit
    {
        /// <summary>
        /// 构造函数，创建实例对象
        /// </summary>
        public AudioActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Sound;
            SceneFlag = CSUtility.Component.enActorSceneFlag.Dynamic_BoundBox;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient;
        }
        /// <summary>
        /// 带参的构造函数
        /// </summary>
        /// <param name="audioFile">媒体文件名</param>
        public AudioActorInit(string audioFile)
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Sound;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient;
            SourceData.AudioSource = audioFile;

            if (SourceData == null)
            {
                SourceData = new CCore.Audio.AudioSourceData();
                SourceData.SoundType = CCore.Performance.ESoundType.Environment;
            }

            SourceData.MaxDistance = 30;
            SourceData.Sound3D = true;
            SourceData.Loop = CCore.Audio.enLoopType.Loop_Normal;
            SourceData.RolloffType = CCore.Audio.enRollOffType.FMOD_3D_LINEARSQUAREROLLOFF;
        }
        /// <summary>
        /// 媒体源数据
        /// </summary>
        protected CCore.Audio.AudioSourceData mSourceData = new CCore.Audio.AudioSourceData();
        /// <summary>
        /// 媒体数据源
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public CCore.Audio.AudioSourceData SourceData
        {
            get { return mSourceData; }
            set { mSourceData = value; }
        }

    }
    /// <summary>
    /// 媒体Actor类
    /// </summary>
    public class AudioActor : CCore.World.Actor, CCore.Socket.ISocketComponent
    {
        #region SocketComponent
        /// <summary>
        /// 挂接件信息
        /// </summary>
        public CCore.Socket.ISocketComponentInfo SocketComponentInfo
        {
            get;
            protected set;
        } = null;
        /// <summary>
        /// 挂接的主mesh
        /// </summary>
        public CCore.Mesh.Mesh ComponentHostMesh
        {
            get;
            set;
        }
        /// <summary>
        /// 初始化挂接组件
        /// </summary>
        /// <param name="info">挂接件信息</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public virtual bool InitializeSocketComponent(CCore.Socket.ISocketComponentInfo info)
        {
            if (ComponentHostMesh == null)
                return false;

            var data = info as CCore.Audio.AudioSourceData;
            if (data == null)
                return false;

            SocketComponentInfo = info;

            var aaInit = new AudioActorInit();
            this.Initialize(aaInit);
            this.SourceData.CopyFrom(data);

            if (this.SourceData.PlayWhenInitialized)
                this.SourceData.Play();

            return true;
        }
        /// <summary>
        /// 提交挂接组件
        /// </summary>
        /// <param name="enviroment">渲染环境</param>
        /// <param name="socketMatrix">挂接组件的矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="eye">视野</param>
        public virtual void SocketComponentCommit(CCore.Graphics.REnviroment enviroment, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, CCore.Camera.CameraObject eye)
        {
            var finalMat = socketMatrix * parentMatrix;
            this.Placement.SetMatrix(ref finalMat);
            this.Visual.Commit(enviroment, ref finalMat, eye);
        }
        /// <summary>
        /// 提交挂接件阴影
        /// </summary>
        /// <param name="light">产生阴影的光源</param>
        /// <param name="socketMatrix">挂接件的矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="isDynamic">是否跟随父对象</param>
        public virtual void SocketComponentCommitShadow(CCore.Light.Light light, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, bool isDynamic)
        {
            var finalMat = socketMatrix * parentMatrix;
            this.Visual.CommitShadow(light, ref finalMat, isDynamic);
        }
        /// <summary>
        /// 挂接组件的每帧调用函数
        /// </summary>
        /// <param name="host">主Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public void SocketComponentTick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            this.Tick(elapsedMillisecond);
        }
        /// <summary>
        /// 挂接组件的淡入
        /// </summary>
        /// <param name="fadeType">淡入模式</param>
        public void StartSocketComponentFadeIn(CCore.Mesh.MeshFadeType fadeType)
        {
        }
        /// <summary>
        /// 挂接组件的淡出
        /// </summary>
        /// <param name="fadeType">淡出模式</param>
        public void StartSocketComponentFadeOut(CCore.Mesh.MeshFadeType fadeType)
        {
        }
        /// <summary>
        /// 更新挂接组件的淡入淡出模式
        /// </summary>
        /// <param name="fadePercent">淡入淡出比例</param>
        public void UpdateSocketComponentFadeInOut(float fadePercent)
        {
        }

        #endregion


        AudioActorInit mAAInit = null;
        /// <summary>
        /// 只读属性，媒体源数据
        /// </summary>
        public CCore.Audio.AudioSourceData SourceData
        {
            get
            {
                if (mAAInit == null)
                    return null;

                return mAAInit.SourceData;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AudioActor()
        {

        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public override void Cleanup()
        {
            CCore.Audio.AudioManager.Instance.Stop(mAAInit.SourceData.AudioChannelId);
            base.Cleanup();
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">初始化类对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            if (!base.Initialize(_init))
                return false;

            mAAInit = _init as AudioActorInit;
            if (mAAInit == null)
                return false;

            //if (mAAInit.SourceData.PlayWhenInitialized)
            //{
            //    mAAInit.SourceData.Play();
            //}

            var vsInit = new CCore.Component.AudioVisualInit();
            vsInit.SourceData = mAAInit.SourceData;
            var vs = new CCore.Component.AudioVisual();
            vs.Initialize(vsInit, this);
            Visual = vs;

            SetPlacement(new CSUtility.Component.StandardPlacement(this));

            return true;
        }

        //public override void OnCommitVisual(MidLayer.IREnviroment env, ref SlimDX.Matrix matrix, MidLayer.ICamera eye)
        //{
        //    if(mAAInit == null)
        //        return;
        //}
        /// <summary>
        /// 进入场景
        /// </summary>
        /// <param name="scene">场景图对象</param>
        public override void OnEnterScene(CCore.Scene.SceneGraph scene)
        {
            base.OnEnterScene(scene);

            if (mAAInit == null)
                return;

            if (mAAInit.SourceData.PlayWhenInitialized)
            {
                mAAInit.SourceData.Play();
            }
        }
        /// <summary>
        /// 从当前场景删除
        /// </summary>
        /// <param name="scene">当前场景对象</param>
        public override void OnRemoveFromScene(CCore.Scene.SceneGraph scene)
        {
            base.OnRemoveFromScene(scene);

            if (mAAInit == null)
                return;

            CCore.Audio.AudioManager.Instance.Stop(mAAInit.SourceData.AudioChannelId);
        }
        /// <summary>
        /// 获取该对象的AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点坐标</param>
        /// <param name="vMax">最大顶点坐标</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            if (mAAInit == null)
                return;

            GetOrigionAABB(ref vMin, ref vMax);

            vMin += mPlacement.GetLocation();
            vMax += mPlacement.GetLocation();
        }
        /// <summary>
        /// 获取原始的AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点坐标</param>
        /// <param name="vMax">最大顶点坐标</param>
        public override void GetOrigionAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            if (mAAInit == null)
                return;

            vMin = new SlimDX.Vector3(-mAAInit.SourceData.MaxDistance);
            vMax = new SlimDX.Vector3(mAAInit.SourceData.MaxDistance);
        }
        /// <summary>
        /// 获取层名称
        /// </summary>
        /// <returns>返回当前层为Sound</returns>
        public override string GetLayerName()
        {
            return "Sound";
        }
        /// <summary>
        /// 获取显示属性的对象
        /// </summary>
        /// <returns>返回当前的对象</returns>
        public override object GetShowPropertyObj()
        {
            if (mAAInit == null)
                return null;

            return mAAInit.SourceData;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
        }
        /// <summary>
        /// 加载场景数据
        /// </summary>
        /// <param name="attribute">XND数据文件</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
            return base.LoadSceneData(attribute);
        }
    }
}
