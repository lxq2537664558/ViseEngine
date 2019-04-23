using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.World
{
    /// <summary>
    /// 世界渲染参数
    /// </summary>
    public class WorldRenderParam
    {
        /// <summary>
        /// 渲染环境
        /// </summary>
        protected CCore.Graphics.REnviroment m_Enviroment;
        /// <summary>
        /// 渲染环境
        /// </summary>
        public CCore.Graphics.REnviroment Enviroment
        {
            get { return m_Enviroment; }
            set { m_Enviroment = value; }
        }
    }
    /// <summary>
    /// 初始化世界工厂类
    /// </summary>
    public abstract class WorldInitFactory
    {
        /// <summary>
        /// 初始化世界工厂的实例对象
        /// </summary>
        public static WorldInitFactory Instance;
        /// <summary>
        /// 创建时间初始化类对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回初始化世界的类对象</returns>
        public abstract WorldInit CreateWorldInit(Byte type);
    }
    
    /// <summary>
    /// 世界初始化类
    /// </summary>
    public class WorldInit : CSUtility.Map.WorldInit, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用的函数
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// 背景音乐
        /// </summary>
        protected string mBackgroundMusic = "";
        /// <summary>
        /// 背景音乐
        /// </summary>
        [CSUtility.Support.DataValueAttribute("BackgroundMusic")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("SoundSet")]
        [Category("基础属性")]
        [DisplayName("背景音乐")]
        public string BackgroundMusic
        {
            get { return mBackgroundMusic; }
            set
            {
                mBackgroundMusic = value;
                IsDirty = true;
                OnPropertyChanged("BackgroundMusic");
            }
        }

        Guid mWeatherSystemId = Guid.Empty;
        /// <summary>
        /// 天气系统的ID
        /// </summary>
        [Category("基础属性")]
        [DisplayName("天气系统")]
        [Browsable(false)]
        [CSUtility.Support.DataValueAttribute("WeatherSystemId")]
        public Guid WeatherSystemId
        {
            get { return mWeatherSystemId; }
            set
            {
                mWeatherSystemId = value;
                OnPropertyChanged("WeatherSystemId");
            }
        }
        
        CCore.Scene.SceneGraphInfo mSceneGraphInfo = new CCore.Scene.SingleSceneGraphInfo();
        /// <summary>
        /// 场景参数
        /// </summary>
        [CSUtility.Support.DataValueAttribute("SceneGraphInfo")]
        [Category("场景")]
        [DisplayName("场景参数")]
        public CCore.Scene.SceneGraphInfo SceneGraphInfo
        {
            get { return mSceneGraphInfo; }
            set
            {
                mSceneGraphInfo = value;
                IsDirty = true;
                OnPropertyChanged("SceneGraphInfo");
            }
        }
        bool mContainsTerrain = false;
        /// <summary>
        /// 是否包含地形
        /// </summary>
        [CSUtility.Support.DataValueAttribute("ContainsTerrain")]
        [Category("地形")]
        [DisplayName("是否包含地形")]
        public bool ContainsTerrain
        {
            get { return mContainsTerrain; }
            set
            {
                mContainsTerrain = value;
                IsDirty = true;
                OnPropertyChanged("ContainsTerrain");
            }
        }
        CCore.Terrain.TerrainInfoOperator mTerrainInfo = new Terrain.TerrainInfoOperator();
        /// <summary>
        /// 地形参数
        /// </summary>
        [CSUtility.Support.DataValueAttribute("TerrainInfo")]
        [Category("地形")]
        [DisplayName("地形参数")]
        public CCore.Terrain.TerrainInfoOperator TerrainInfo
        {
            get { return mTerrainInfo; }
            set
            {
                mTerrainInfo = value;
                IsDirty = true;
                OnPropertyChanged("TerrainInfo");
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WorldInit()
        {
            mSceneGraphInfo.ResetDefault();
        }
    }
    /// <summary>
    /// 世界类
    /// </summary>
	[System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public partial class World : CSUtility.Map.IWorldBase
    {
        /// <summary>
        /// 只读属性，该世界是否为空
        /// </summary>
        public virtual bool IsNullWorld
        {
            get { return false; }
        }
        /// <summary>
        /// 世界的ID
        /// </summary>
        protected Guid mId;
        /// <summary>
        /// 只读属性，世界ID
        /// </summary>
        public Guid Id
        {
            get
            {
                if (mWorldInit == null)
                    return Guid.Empty;

                return mWorldInit.WorldId;
            }
        }

        WorldInit mWorldInit;
        /// <summary>
        /// 只读属性，世界初始化对象
        /// </summary>
        public WorldInit WorldInit
        {
            get { return mWorldInit; }
        }
        /// <summary>
        /// 场景图对象
        /// </summary>
        protected CCore.Scene.SceneGraph mSceneGraph;
        /// <summary>
        /// 场景图对象
        /// </summary>
        public CCore.Scene.SceneGraph SceneGraph
        {
            get{ return mSceneGraph; }
            set{ mSceneGraph = value; }
        }
        /// <summary>
        /// 地形
        /// </summary>
        protected CCore.Terrain.Terrain mTerrain;
        /// <summary>
        /// 当前的地形
        /// </summary>
        public CCore.Terrain.Terrain Terrain
        {
            get{ return mTerrain; }
            set{ mTerrain = value; }
        }
        /// <summary>
        /// 摄像机
        /// </summary>
        protected CCore.Camera.CameraObject mCamera = null;
        /// <summary>
        /// 只读属性，当前摄像机
        /// </summary>
        public CCore.Camera.CameraObject Camera
        {
            get{ return mCamera; }
        }
        /// <summary>
        /// 是否显示通用编辑，默认为false
        /// </summary>
        protected bool mShowEditorCommon = false;
        /// <summary>
        /// 是否显示通用编辑
        /// </summary>
        public bool ShowEditorCommon
        {
            get{ return mShowEditorCommon; }
            set{ mShowEditorCommon = value; }
        }
        /// <summary>
        /// 是否显示伤害盒子，默认为false
        /// </summary>
        protected bool mShowHurtBox = false;
        /// <summary>
        /// 是否显示伤害盒子
        /// </summary>
        public bool ShowHurtBox
        {
            get { return mShowHurtBox; }
            set { mShowHurtBox = value; }
        }
        /// <summary>
        /// 地形的性能计数
        /// </summary>
        protected static CSUtility.Performance.PerfCounter TerrainVisibleTimer = new CSUtility.Performance.PerfCounter("World.TerrainVisible");
        /// <summary>
        /// 块状地图的性能计数
        /// </summary>
        protected static CSUtility.Performance.PerfCounter TileVisibleTimer = new CSUtility.Performance.PerfCounter("World.TileVisible");
        /// <summary>
        /// 是否显示继承自包围盒的瓦片地图
        /// </summary>
        public static bool ShowTileInheritBoundingBox;
        /// <summary>
        /// 是否每帧都绘制Actor，默认为true
        /// </summary>
        protected bool mActorOneFrameOneRender = true;
        /// <summary>
        /// 是否每帧都绘制Actor
        /// </summary>
        public bool ActorOneFrameOneRender
        {
            get { return mActorOneFrameOneRender; }
            set { mActorOneFrameOneRender = value; }
        }

        // 不需要场景管理的对象
        CSUtility.Support.AsyncObjManager<Guid, Actor> mSpecialActorsDic = new CSUtility.Support.AsyncObjManager<Guid, Actor>();
        /// <summary>
        /// 只读属性，不需要场景管理的对象列表
        /// </summary>
        public CSUtility.Support.AsyncObjManager<Guid, Actor> SpecialActorsDic
        {
            get { return mSpecialActorsDic; }
        }
        
        float mSunAngle = 0;
        /// <summary>
        /// 太阳的角度
        /// </summary>
        public float SunAngle
        {
            get { return mSunAngle; }
            set
            {
                mSunAngle = value;
            }
        }


		//////////////////////////////////////////////////////////////////////////

        //private static IWorld()
        //{
        //    CounterRenderVisible = new IPerfCounter("World.RenderVisible");
        //}
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="id">地形ID</param>
        public World(Guid id)
        {
            mId = id;

            //mShowEditorTypeDic[(UInt16)(CSUtility.Component.EActorGameType.Light)] = false;
            //mShowEditorTypeDic[(UInt16)(CSUtility.Component.EActorGameType.Decal)] = false;
            //mShowEditorTypeDic[(UInt16)(CSUtility.Component.EActorGameType.ScenePoint)] = false;

            CCore.Program.InitializeActorTypeShow(this, CCore.Program.SoundTypeName, false, (UInt16)CSUtility.Component.EActorGameType.Sound);
            CCore.Program.InitializeActorTypeShow(this, CCore.Program.NavigationPointTypeName, false, (UInt16)CSUtility.Component.EActorGameType.NavigationPoint);
            CCore.Program.InitializeActorTypeShow(this, CCore.Program.ScenePointTypeName, false, (UInt16)CSUtility.Component.EActorGameType.ScenePoint);
            CCore.Program.InitializeActorTypeShow(this, CCore.Program.SimplyMeshTypeName, false, (UInt16)CSUtility.Component.EActorGameType.Unknow);
            CCore.Program.InitializeActorTypeShow(this, CCore.Program.DecalAssistTypeName, false, (UInt16)CSUtility.Component.EActorGameType.Decal);
            CCore.Program.InitializeActorTypeShow(this, CCore.Program.DynamicBlockTypeName, false, (UInt16)CSUtility.Component.EActorGameType.DynamicBlock);
            CCore.Program.InitializeActorTypeShow(this, CCore.Program.LightAssistTypeName, false, (UInt16)CSUtility.Component.EActorGameType.Light);
            CCore.Program.InitializeActorTypeShow(this, CCore.Program.TriggerAssistTypeName, false, (UInt16)CSUtility.Component.EActorGameType.Trigger);
            CCore.Program.InitializeActorTypeShow(this, CCore.Program.NPCInitializerAssistTypeName, false, (UInt16)CSUtility.Component.EActorGameType.NpcInitializer,
                                                 (World world, UInt16 actorType, bool show)=>
                                                 {
                                                     if(show)
                                                         world.LoadWorld(world.GetWorldLastLoadedAbsFolder("场景"), "种植NPC");
                                                 });

            RegSaveLoadComponent("场景", true, new WorldComponent_ClientScene(), ((world)=>
            {
                return world.WorldInit.SceneGraphInfo;
            }));
            RegSaveLoadComponent("地形", true, new WorldComponent_Terrain(), (world) =>
            {
                return world.WorldInit.TerrainInfo.OpInfo;
            });
            RegSaveLoadComponent("服务器高度图", false, new WorldComponent_ServerAltitude(), (world)=>
            {
                return null;
            });
            RegSaveLoadComponent("触发器", true, new WorldComponent_ServerActors(), (world)=>
            {
                return CSUtility.Component.EActorGameType.Trigger;
            });
            RegSaveLoadComponent("种植NPC", false, new WorldComponent_ServerActors(), (world) =>
            {
                return CSUtility.Component.EActorGameType.NpcInitializer;
            });
            RegSaveLoadComponent("寻路", true, new WorldComponent_Navigation(), (world)=>
            {
                return null;
            });
            RegSaveLoadComponent("后期特效", true, new WorldComponent_PostProcess(), (world) =>
            {
                return null;
            });
            RegSaveLoadComponent("世界对象", true, new WorldComponent_WorldActors(), (world) =>
            {
                return null;
            });
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~World()
        {
            Cleanup();
        }

        #region Initialize
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="init">世界初始化对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(WorldInit init)
        {
            Cleanup();
            mWorldInit = init;

            /*InitializeSceneGraph(init.SceneGraphInfo);
            InitializeNavigation();
            InitializeServerAltitude();*/

            return true;
        }
        //public bool Initialize(CCore.Scene.SceneGraph sg, CCore.Terrain.Terrain terrain)
        //{
        //    Cleanup();

        //    mSceneGraph = sg;
        //    mTerrains = terrain;

        //    return true;
        //}
        //public void InitializeSceneGraph(CCore.Scene.SceneGraphInfo info)
        //{
        //    if (mSceneGraph != null)
        //    {
        //        mSceneGraph.Cleanup();
        //        mSceneGraph = null;
        //    }

        //    mSceneGraph = System.Activator.CreateInstance(info.GetSceneGraphType()) as CCore.Scene.SceneGraph;
        //    mSceneGraph.Initialize(info, this);
        //}
        //public void InitializeNavigation()
        //{
        //    /*
        //    var navRelativeFile = "Navigation/Navigation.nav";
        //    //Navigation.INavigation.Instance.NavigationData = new Navigation.INavigationDataWrapper();

        //    // 初始化寻路参数
        //    var navInfo = new CSUtility.Navigation.INavigationInfo();
        //    navInfo.ResetDefault();

        //    float cellX = 0, cellZ = 0;
        //    // 根据场景管理器来计算每个寻路Level的大小
        //    //mSceneGraph.GetLevelAvailableXZCount(ref navInfo.mXValidLevelCount, ref navInfo.mZValidLevelCount);
        //    //if (navInfo.mXValidLevelCount == 0 || navInfo.mZValidLevelCount == 0)
        //    //{
        //    navInfo.mXValidLevelCount = mWorldInit.SceneSizeX;
        //    navInfo.mZValidLevelCount = mWorldInit.SceneSizeZ;
        //    //}
        //    cellX = mWorldInit.SceneMeterX / mWorldInit.SceneSizeX;//mSceneGraph.GetXLengthPerLevel();
        //    cellZ = mWorldInit.SceneMeterZ / mWorldInit.SceneSizeZ;//mSceneGraph.GetZLengthPerLevel();
        //    navInfo.mLevelLengthX = (UInt32)(cellX / navInfo.mMeterPerPixelX);
        //    navInfo.mLevelLengthZ = (UInt32)(cellZ / navInfo.mMeterPerPixelZ);

        //    var navFile = mWorldFileName + navRelativeFile;
        //    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(navFile);
        //    var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(navFile);

        //    CCore.Navigation.Navigation.Instance.Initialize(file, path, ref navInfo);*/
        //}
        //public virtual void InitializeServerAltitude()
        //{
        //    /*var shmInfo = new ServerMap.IServerAltitudeInfo();
        //    shmInfo.ResetDefault();

        //    // 根据场景管理器来计算每个服务器高度图Level的大小
        //    UInt32 xValidLevelCount = 0, zValidLevelCount = 0;
        //    mSceneGraph.GetLevelAvailableXZCount(ref xValidLevelCount, ref zValidLevelCount);
        //    if (xValidLevelCount == 0 || zValidLevelCount == 0)
        //    {
        //        xValidLevelCount = mWorldInit.SceneSizeX;
        //        zValidLevelCount = mWorldInit.SceneSizeZ;
        //    }
        //    shmInfo.XValidLevelCount = xValidLevelCount;
        //    shmInfo.ZValidLevelCount = zValidLevelCount;
        //    //float cellX = mSceneGraph.GetXLengthPerLevel();
        //    //float cellZ = mSceneGraph.GetZLengthPerLevel();
        //    float cellX = mWorldInit.SceneMeterX / mWorldInit.SceneSizeX;
        //    float cellZ = mWorldInit.SceneMeterZ / mWorldInit.SceneSizeZ;
        //    shmInfo.LevelLengthX = (UInt32)(cellX / shmInfo.MeterPerPixelX);
        //    shmInfo.LevelLengthZ = (UInt32)(cellZ / shmInfo.MeterPerPixelZ);

        //    //var shmFile = mWorldFileName + m_sceneInit.ServerHeightMapFileName;
        //    //var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(shmFile);
        //    //var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(shmFile);

        //    CCore.Support.ServerAltitudeAssist.Instance.InitializeServerHeightMapProxy(shmInfo);*/
        //}

        #endregion
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public virtual void PlayBackgroundMusic()
        {
            if (!string.IsNullOrEmpty(mWorldInit?.BackgroundMusic))
            {
                CCore.Audio.AudioManager.Instance.Play(mWorldInit.BackgroundMusic, mWorldInit.WorldId, (UInt32)(CCore.Performance.ESoundType.MusicSound), CCore.Audio.enLoopType.Loop_Normal, false, true);
            }
        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public virtual void StopBackgroundMusic()
        {
            if (CCore.Audio.AudioManager.Instance == null)
                return;
            if (!string.IsNullOrEmpty(mWorldInit?.BackgroundMusic))
            {
                CCore.Audio.AudioManager.Instance.Stop(mWorldInit.WorldId);
            }
        }

        /// <summary>
        /// 设置场景和地形的边缘间距
        /// </summary>
        /// <param name="value">间隔距离</param>
        public void SetNeighborSide(uint value)
        {
            if(mSceneGraph != null)
                mSceneGraph.SetNeighborSide(value);
            if(mTerrain != null)
            {
                mTerrain.SetNeighborSide(value);
            }
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public virtual void Cleanup()
        {
            StopBackgroundMusic();
            
            foreach (var i in mEditorActors.Values)
            {
                i.Cleanup();
            }
            mEditorActors.Clear();

            if (mSceneGraph != null)
            {
                mSceneGraph.Cleanup();
                mSceneGraph = null;
            }
            
            if(mTerrain!=null)
                mTerrain.Cleanup();

            foreach (var i in mPostProceses)
            {
                i.Cleanup();
            }
            mPostProceses.Clear();

            CSUtility.Support.AsyncObjManager<Guid, Actor>.FOnVisitObject visitor = delegate(Guid key, Actor value, object arg)
            {
                value.Cleanup();

                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            mSpecialActorsDic.For_Each(visitor, null);
            mSpecialActorsDic.Clear();
        }
        /// <summary>
        /// 根据ID查找Actor
        /// </summary>
        /// <param name="id">Actor的ID</param>
        /// <returns>返回对应ID的Actor</returns>
        public virtual Actor FindActor(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            var result = mEditorActors.FindObj(id);
            if (result != null)
                return result;

            result = mSpecialActorsDic.FindObj(id);
            if (result != null)
                return result;

            if (mSceneGraph != null)
                return mSceneGraph.FindActor(ref id);

            return null;
        }
        /// <summary>
        /// 声明添加Actor的委托事件
        /// </summary>
        /// <param name="act">添加的Actor对象</param>
        /// <param name="world">添加到的世界</param>
        public delegate void Delegaet_OnAddActor(Actor act, World world);
        /// <summary>
        /// 定义添加Actor的委托事件
        /// </summary>
        public event Delegaet_OnAddActor OnAddActor;
        /// <summary>
        /// 添加Actor对象
        /// </summary>
        /// <param name="act">要添加的Actor对象</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public virtual bool AddActor(Actor act)
        {
            bool retValue = false;
            act.World = this;

            if (act.HasFlag(CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager))
            {
                act.World = this;
                mSpecialActorsDic.Add(act.Id, act);
            }
            else if (mSceneGraph != null)
            {
                retValue = mSceneGraph.AddActor(act);
            }
            OnAddActor?.Invoke(act, this);

            return retValue;
        }
        /// <summary>
        /// 声明从世界中删除相应的Actor的委托事件
        /// </summary>
        /// <param name="act">要删除的Actor对象</param>
        /// <param name="world">世界对象</param>
        public delegate void Delegaet_OnRemoveActor(Actor act, World world);
        /// <summary>
        /// 定义从世界中删除相应的Actor的委托事件
        /// </summary>
        public event Delegaet_OnRemoveActor OnRemoveActor;
        /// <summary>
        /// 删除Actor对象
        /// </summary>
        /// <param name="act">要删除的Actor对象</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public virtual bool RemoveActor(Actor act)
        {
            bool retValue = false;
            act.World = null;
            mSpecialActorsDic.Remove(act.Id);
            
            if (mSceneGraph != null)
                retValue = mSceneGraph.RemoveActor(act);
            OnRemoveActor?.Invoke(act, this);
            return retValue;
        }
        /// <summary>
        /// 获取相同类型的Actor对象
        /// </summary>
        /// <param name="actorType">Actor的类型</param>
        /// <returns>返回相同类型的Actor对象列表</returns>
        public virtual List<Actor> GetActors(UInt16 actorType)
        {
            List<Actor> retValue = new List<Actor>();
            if (mSceneGraph != null)
                retValue = mSceneGraph.GetActors(actorType);

            CSUtility.Support.AsyncObjManager<Guid, Actor>.FOnVisitObject visitor = delegate(Guid key, Actor value, object arg)
            {
                if (value.GameType == actorType || actorType == 0)
                    retValue.Add(value);
                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            mSpecialActorsDic.For_Each(visitor, null);

            return retValue;
        }
        /// <summary>
        /// 获取某一区域相同类型的Actor对象
        /// </summary>
        /// <param name="vStart">起点坐标</param>
        /// <param name="vEnd">终点坐标</param>
        /// <param name="actorType">Actor的类型</param>
        /// <returns>返回该区域相同类型的Actor对象</returns>
		public virtual List<Actor> GetActors(ref SlimDX.Vector3 vStart, ref SlimDX.Vector3 vEnd, UInt16 actorType)
        {
            List<Actor> retValue = new List<Actor>();
            if (mSceneGraph != null)
                retValue = mSceneGraph.GetActors(ref vStart, ref vEnd, actorType);

            var vs = vStart;
            var ve = vEnd;
            CSUtility.Support.AsyncObjManager<Guid, Actor>.FOnVisitObject visitor = delegate(Guid key, Actor value, object arg)
            {
                if (value.GameType == actorType || actorType == 0)
                {
                    var loc = value.Placement.GetLocation();

                    if (loc.X >= vs.X && loc.X <= ve.X &&
                       loc.Y >= vs.Y && loc.Y <= ve.Y &&
                       loc.Z >= vs.Z && loc.Z <= ve.Z)
                        retValue.Add(value);
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            mSpecialActorsDic.For_Each(visitor, null);

            return retValue;
        }

        #region 后期特效
        /// <summary>
        /// 后期特效列表
        /// </summary>
        protected List<CCore.Graphics.PostProcess> mPostProceses = new List<CCore.Graphics.PostProcess>();
        /// <summary>
        /// 只读属性，后期特效列表
        /// </summary>
        public List<CCore.Graphics.PostProcess> PostProceses
        {
            get { return mPostProceses; }
        }
        /// <summary>
        /// 添加后期特效
        /// </summary>
        /// <param name="p">后期特效对象</param>
        public virtual void AddPostProcess(CCore.Graphics.PostProcess p)
        {
            mPostProceses.Add(p);
        }
        /// <summary>
        /// 删除相应的后期特效
        /// </summary>
        /// <param name="p">后期特效</param>
		public virtual void RemovePostProcess(CCore.Graphics.PostProcess p)
        {
            if(mPostProceses.Contains(p))
            {
                p.Cleanup();
                mPostProceses.Remove(p);
            }
        }
        /// <summary>
        /// 删除所有的后期特效
        /// </summary>
		public virtual void RemoveAllPostProcess()
        {
            foreach(var p in mPostProceses)
            {
                p.Cleanup();
            }
            mPostProceses.Clear();
        }
        #endregion

        #region 编辑器专用接口

        //PS_ReadOnly( System.Collections.Generic.List<IActor^>^ , VisulaActors , mVisualActors , , );
        //PS_ReadOnly( System.Collections.Generic.List<IActor^>^ , SpecEffectActors , mSpecEffectActors , , );

        // 编辑器用对象一般都有引用，而且编辑器始终伴随游戏，所以这里不做查询操作
        /// <summary>
        /// 编辑器中使用的Actor对象列表
        /// </summary>
        public CSUtility.Support.ConcurentObjManager<Guid, CCore.World.Actor> mEditorActors = new CSUtility.Support.ConcurentObjManager<Guid, Actor>();
        /// <summary>
        /// 添加Actor对象到编辑器对象列表
        /// </summary>
        /// <param name="act">要添加的Actor对象</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public bool AddEditorActor(Actor act)
        {
            if (act == null)
                return false;

            if (mEditorActors.ContainsKey(act.Id))
                return true;

            mEditorActors[act.Id] = act;
            act.World = this;

            return true;
        }
        /// <summary>
        /// 从编辑器对象列表中删除相应的Actor对象
        /// </summary>
        /// <param name="act">要删除的Actor对象</param>
        /// <returns>删除成功返回true，否则返回false</returns>
		public bool RemoveEditorActor(Actor act)
        {
            if (!mEditorActors.ContainsKey(act.Id))
                return false;

            act.World = null;
            mEditorActors.Remove(act.Id);

            return true;
        }
        
        /// <summary>
        /// 世界置脏标志，世界中对象改变时为true，世界保存后为false
        /// </summary>
        public virtual bool IsDirty
        {
            get;
            set;
        }


#endregion
        /// <summary>
        /// 每帧调用更新不需场景管理的Actor对象
        /// </summary>
        protected void TickSpecialActors()
        {
            mSpecialActorsDic.BeforeTick();

            var elapsedTime = Engine.Instance.GetElapsedMillisecond();
            CSUtility.Support.AsyncObjManager<Guid, Actor>.FOnVisitObject visitor = delegate(Guid key, Actor value, object arg)
            {
                value.Tick(elapsedTime);
                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            mSpecialActorsDic.For_Each(visitor, null);
        }
		/// <summary>
        /// 每帧调用
        /// </summary>
		public virtual void Tick()
        {
            mSceneGraph?.Tick();            
            mTerrain?.Tick();

            foreach (var pp in PostProceses)
            {
                pp.Tick(CCore.Engine.Instance.GetElapsedMillisecond());
            }

            foreach (var act in mEditorActors.Values)
            {
                act.Tick(Engine.Instance.GetElapsedMillisecond());
            }
            TickSpecialActors();
        }
        /// <summary>
        /// 渲染场景
        /// </summary>
        /// <param name="param">渲染参数</param>
		public virtual void Render2Enviroment(WorldRenderParam param)
        {
            if (param.Enviroment == null)
                return;

            BeginDraw();
            mCamera = param.Enviroment.Camera;

            //通过 Culling
            //param.Enviroment.CleanVisual();
            //渲染天空 RL_PreWorld

            //渲染地形 RL_World

            //渲染纯粹客户端表现对象
            TerrainVisibleTimer.Begin();
            mTerrain?.RenderVisible(param.Enviroment.Camera, param.Enviroment);
            TerrainVisibleTimer.End();

            TileVisibleTimer.Begin();
            mSceneGraph?.RenderVisible(param.Enviroment.Camera, param.Enviroment, null);
            TileVisibleTimer.End();

            // Commit特殊Actor
            SlimDX.Matrix matrix;

            CSUtility.Support.AsyncObjManager<Guid, Actor>.FOnVisitObject visitor = delegate(Guid key, Actor value, object arg)
            {
                if (value.Placement != null && value.Placement.GetAbsMatrix(out matrix))
                {
                    if (value.Visual != null && value.Visual.Visible)
                        value.Visual.Commit(param.Enviroment, ref matrix, param.Enviroment.Camera);

                    value.OnCommitVisual(param.Enviroment, ref matrix, param.Enviroment.Camera);
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            mSpecialActorsDic.For_Each(visitor, null);

            // Commit编辑器Actor
            mEditorActors.For_Each((Guid id, Actor editActor, object arg) =>
            {
                if (editActor.Placement != null && editActor.Placement.GetAbsMatrix(out matrix))
                {
                    if (editActor.Visual != null && editActor.Visual.Visible)
                        editActor.Visual.Commit(param.Enviroment, ref matrix, param.Enviroment.Camera);

                    editActor.OnCommitVisual(param.Enviroment, ref matrix, param.Enviroment.Camera);
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
            
            // 向灯光中提交产生阴影的Actor
            CommitShadow(param);

            foreach (var pp in PostProceses)
            {
                pp.mOwnerEnv = param.Enviroment;
            }

            ////渲染特效
            //for each( IActor^ act in mSpecEffectActors.Values )
            //{
            //	SlimDX::Matrix matrix;
            //	if( act.Placement!=nullptr && act.Placement.GetAbsMatrix( matrix ) )
            //	{
            //		act.Visual.Commit(param.Enviroment, matrix,param.Enviroment.Camera);
            //	}
            //}

            //
            EndDraw();
        }
        /// <summary>
        /// 开始绘制
        /// </summary>
		public void BeginDraw() { }
        /// <summary>
        /// 结束绘制
        /// </summary>
		public void EndDraw() { }

        /// <summary>
        /// 产生阴影的灯光列表
        /// </summary>
        public CSUtility.Support.AsyncObjManager<Guid, CCore.Light.Light> ShadowLights = new CSUtility.Support.AsyncObjManager<Guid, Light.Light>();
        /// <summary>
        /// 提交阴影对象
        /// </summary>
        /// <param name="param">世界渲染参数</param>
        public void CommitShadow(WorldRenderParam param)
        {
            if (IsShadow == false)
                return;

            if (mSceneGraph != null)
            {
                Light.Light[] lights = new Light.Light[ShadowLights.Count];
                ShadowLights.Values.CopyTo(lights, 0);
                mSceneGraph.RenderShadow(param.Enviroment.Camera, param.Enviroment, lights);
            }

            ShadowLights.For_Each((Guid id, Light.Light light, object argSL) =>
            {
                CSUtility.Support.AsyncObjManager<Guid, CCore.World.Actor>.FOnVisitObject visitor = delegate (Guid key, CCore.World.Actor value, object arg)
                {
                    if (value.Visual == null)
                        return CSUtility.Support.EForEachResult.FER_Continue;

                    SlimDX.Matrix matrix;
                    if (value.Placement != null && value.Placement.GetAbsMatrix(out matrix))
                    {
                        light.AddVisual(value.Visual.Layer, value.Visual, ref matrix, true);
                    }
                    return CSUtility.Support.EForEachResult.FER_Continue;
                };
                mSpecialActorsDic.For_Each(visitor, null);
                return CSUtility.Support.EForEachResult.FER_Continue;

            }, null);
        }
        public static bool IsShadow
        {
            get;
            set;
        } = true;
        /// <summary>
        /// 渲染阴影
        /// </summary>
        /// <param name="param">世界渲染参数</param>
        public void RenderShadow(WorldRenderParam param)
        {
            if (IsShadow == false)
                return;

            //foreach (var l in ShadowLights)
            ShadowLights.For_Each((Guid key, Light.Light value, object arg) =>
            {
                value.RenderShadow(param.Enviroment.Camera);
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }
        /// <summary>
        /// 交换阴影渲染管道
        /// </summary>
        public void SwapShadowPipes()
        {
            ShadowLights.BeforeTick();
            ShadowLights.For_Each((Guid key, Light.Light value, object arg) =>
            {
                value.SwapPipes();
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }

        /// <summary>
        /// 显示指定位置的地图（包括场景和地形，在格子场景管理中，调用此接口会加载指定位置所在的场景块以及附近指定范围的场景块）
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="z">Z轴坐标</param>
        public virtual void TravelTo(float x, float z)
        {
            mTerrain?.TravelTo(x, z);
            mSceneGraph?.TravelTo(x, z);

            CCore.Navigation.Navigation.Instance.NavigationData?.TravelTo(x, z, CCore.Engine.Instance.GetFrameMillisecond());
        }
        /// <summary>
        /// 连线检查
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <returns>检查无问题返回true，否则返回false</returns>
        static CSUtility.Performance.PerfCounter WorldLineCheck_Timer = new CSUtility.Performance.PerfCounter("World.LineCheck");
        public virtual bool LineCheck( ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result )
        {
            WorldLineCheck_Timer.Begin();
            bool b1 = false;
            var result1 = new CSUtility.Support.stHitResult();
            result1.mClosedDistSq = result.mClosedDistSq;
            result1.mClosedPos = result.mClosedPos;
            result1.mHitFlags = result.mHitFlags;
            if (mSceneGraph != null)
            {
                b1 = mSceneGraph.LineCheck(ref start, ref end, ref result1);
                result1.mHitLength = SlimDX.Vector3.Distance(result1.mHitPosition, start);
            }

            bool b2 = false;
            var result2 = new CSUtility.Support.stHitResult();
            result2.mClosedDistSq = result.mClosedDistSq;
            result2.mClosedPos = result.mClosedPos;
            result2.mHitFlags = result.mHitFlags;
            if (mTerrain != null)
            {
                b2 = mTerrain.LineCheck(ref start, ref end, ref result2);
                result2.mHitLength = SlimDX.Vector3.Distance(result2.mHitPosition, start);
            }

            if (b1 == false && b2 == false)
            {
                //result = new CSUtility.Support.stHitResult();
                WorldLineCheck_Timer.End();
                return false;
            }
            else if (b1 == true && b2 == false)
            {
                result.mHitLength = result1.mHitLength;
                result.mHitFlags = result1.mHitFlags;
                result.mHitNormal = result1.mHitNormal;
                result.mHitPosition = result1.mHitPosition;
                result.mHitActorId = result1.mHitActorId;
            }
            else if (b1 == false && b2 == true)
            {
                result.mHitLength = result2.mHitLength;
                result.mHitFlags = result2.mHitFlags;
                result.mHitNormal = result2.mHitNormal;
                result.mHitPosition = result2.mHitPosition;
                result.mHitActorId = Guid.Empty;
            }
            else
            {
                if (result1.mHitLength < result2.mHitLength)
                {
                    result.mHitLength = result1.mHitLength;
                    result.mHitFlags = result1.mHitFlags;
                    result.mHitNormal = result1.mHitNormal;
                    result.mHitPosition = result1.mHitPosition;
                    result.mHitActorId = result1.mHitActorId;
                }
                else
                {
                    result.mHitLength = result2.mHitLength;
                    result.mHitFlags = result2.mHitFlags;
                    result.mHitNormal = result2.mHitNormal;
                    result.mHitPosition = result2.mHitPosition;
                    result.mHitActorId = Guid.Empty;
                }
            }

            WorldLineCheck_Timer.End();
            return true;
        }
        /// <summary>
        /// 连线检查
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <param name="exceptActor">额外的Actor对象</param>
        /// <returns>检查问题返回true，否则返回false</returns>
		public virtual bool LineCheck( ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result, List<Actor> exceptActor )
        {
            WorldLineCheck_Timer.Begin();
            bool b1 = false;
            var result1 = new CSUtility.Support.stHitResult();
            result1.mHitFlags = result.mHitFlags;
            if (mSceneGraph != null)
            {
                b1 = mSceneGraph.LineCheck(ref start, ref end, ref result1, exceptActor);
                result1.mHitLength = SlimDX.Vector3.Distance(result1.mHitPosition, start);
            }

            bool b2 = false;
            var result2 = new CSUtility.Support.stHitResult();
            result2.mHitFlags = result.mHitFlags;
            if (mTerrain != null)
            {
                b2 = mTerrain.LineCheck(ref start, ref end, ref result2);
                result2.mHitLength = SlimDX.Vector3.Distance(result2.mHitPosition, start);
            }

            if (b1 == false && b2 == false)
            {
                //result = new CSUtility.Support.stHitResult();
                WorldLineCheck_Timer.End();
                return false;
            }
            else if (b1 == true && b2 == false)
            {
                result.mHitLength = result1.mHitLength;
                result.mHitFlags = result1.mHitFlags;
                result.mHitNormal = result1.mHitNormal;
                result.mHitPosition = result1.mHitPosition;
                result.mHitActorId = result1.mHitActorId;
            }
            else if (b1 == false && b2 == true)
            {
                result.mHitLength = result2.mHitLength;
                result.mHitFlags = result2.mHitFlags;
                result.mHitNormal = result2.mHitNormal;
                result.mHitPosition = result2.mHitPosition;
                result.mHitActorId = Guid.Empty;
            }
            else
            {
                if (result1.mHitLength < result2.mHitLength)
                {
                    result.mHitLength = result1.mHitLength;
                    result.mHitFlags = result1.mHitFlags;
                    result.mHitNormal = result1.mHitNormal;
                    result.mHitPosition = result1.mHitPosition;
                    result.mHitActorId = result1.mHitActorId;
                }
                else
                {
                    result.mHitLength = result2.mHitLength;
                    result.mHitFlags = result2.mHitFlags;
                    result.mHitNormal = result2.mHitNormal;
                    result.mHitPosition = result2.mHitPosition;
                    result.mHitActorId = Guid.Empty;
                }
            }

            WorldLineCheck_Timer.End();
            return true;
        }

        #region 地图组件
        /// <summary>
        /// 地图组件类
        /// </summary>
        public class WorldComponentData
        {
            /// <summary>
            /// 地图组件对象
            /// </summary>
            public IWorldComponent Component;
            /// <summary>
            /// 是否开启滚动条
            /// </summary>
            public bool AutoProcess;
            /// <summary>
            /// 获取功能列表
            /// </summary>
            public Func<CCore.World.World, object> GetArgumentFunc;
        }

        static Dictionary<string, WorldComponentData> mWorldComponents = new Dictionary<string, WorldComponentData>();
        /// <summary>
        /// 只读属性，地图组件
        /// </summary>
        public static Dictionary<string, WorldComponentData> WorldComponents
        {
            get { return mWorldComponents; }
        }
        /// <summary>
        /// 注册地图组件
        /// </summary>
        /// <param name="componentName">组件名称</param>
        /// <param name="autoProcess">跟随地图进行自动处理，如初始化、保存、加载</param>
        /// <param name="component">组件</param>
        /// <param name="getArgumentFunc">获取组件参数的函数</param>
        public void RegSaveLoadComponent(string componentName, bool autoProcess, IWorldComponent component, Func<CCore.World.World, object> getArgumentFunc)
        {
            if (!mWorldComponents.ContainsKey(componentName))
                mWorldComponents[componentName] = new WorldComponentData()
                {
                    Component = component,
                    AutoProcess = autoProcess,
                    GetArgumentFunc = getArgumentFunc,
                };
        }
        /// <summary>
        /// 删除注册的地图组件
        /// </summary>
        /// <param name="componentName">组件名称</param>
        public void UnRegSaveLoadComponent(string componentName)
        {
            mWorldComponents.Remove(componentName);
        }
        /// <summary>
        /// 保存地图配置
        /// </summary>
        /// <param name="absFolder">保存配置的绝对路径</param>
        public void SaveConfig(string absFolder)
        {
            WorldInit?.Save(absFolder);
        }
        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="strAbsFolder">地图绝对路径</param>
        /// <param name="componentName">组件名称，如果未设置则初始化全部标识为自动处理的组件</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public virtual bool Initialize(System.String strAbsFolder, string componentName = "")
        {
            if(string.IsNullOrEmpty(componentName))
            {
                bool retValue = true;
                foreach(var compData in mWorldComponents.Values)
                {
                    if(compData.AutoProcess)
                    {
                        retValue = retValue && compData.Component.InitializeComponent(this, strAbsFolder, compData.GetArgumentFunc(this));
                    }
                }

                return retValue;
            }
            else
            {
                WorldComponentData data;
                if (!mWorldComponents.TryGetValue(componentName, out data))
                    return false;

                return data.Component.InitializeComponent(this, strAbsFolder, data.GetArgumentFunc(this));
            }
        }
        
        /// <summary>
        /// 创建地图
        /// </summary>
        /// <param name="strAbsFolder">地图绝对路径</param>
        /// <param name="componentName">组件名称，如果未设置则创建全部标识为自动处理的组件</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public virtual bool Create(System.String strAbsFolder, string componentName = "")
        {
            if(string.IsNullOrEmpty(componentName))
            {
                bool retValue = true;
                foreach(var compData in mWorldComponents.Values)
                {
                    if (compData.AutoProcess)
                        retValue = retValue && compData.Component.CreateComponent(this, strAbsFolder, compData.GetArgumentFunc(this));
                }
                return retValue;
            }
            else
            {
                WorldComponentData data;
                if (!mWorldComponents.TryGetValue(componentName, out data))
                    return false;

                return data.Component.CreateComponent(this, strAbsFolder, data.GetArgumentFunc(this));
            }
        }

        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="strAbsFolder">地图绝对路径</param>
        /// <param name="forceSave">是否强制保存</param>
        /// <param name="componentName">组件名称，如果未设置则保存全部标识为自动处理的组件</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public virtual bool SaveWorld(System.String strAbsFolder, bool forceSave, string componentName = "")
        {
            if(string.IsNullOrEmpty(componentName))
            {
                bool retValue = true;
                foreach(var compData in mWorldComponents.Values)
                {
                    if (compData.AutoProcess)
                        retValue = retValue && compData.Component.SaveWorldComponent(this, strAbsFolder, compData.GetArgumentFunc(this), forceSave);
                }
                return retValue;
            }
            else
            {
                WorldComponentData data;
                if (!mWorldComponents.TryGetValue(componentName, out data))
                    return false;
            
                return data.Component.SaveWorldComponent(this, strAbsFolder, data.GetArgumentFunc(this), forceSave);
            }
        }

        /// <summary>
        /// 存储不需要场景管理的特殊对象
        /// </summary>
        /// <param name="strAbsFolder">地图绝对路径</param>
        /// <returns>存储成功返回true，否则返回false</returns>
        public virtual bool SaveSpecialActors(System.String strAbsFolder)
        {
            if (string.IsNullOrEmpty(strAbsFolder))
                return false;

            var fileName = strAbsFolder + "/SpecialActor.dat";
            var holder = CSUtility.Support.XndHolder.NewXNDHolder();
            var spNode = holder.Node.AddNode("SpecialActors", 0, 0);

            mSpecialActorsDic.For_Each((Guid id, Actor actor, object obj) =>
            {
                var typeName = CSUtility.Program.GetTypeSaveString(actor.GetType());
                var actorNode = spNode.AddNode(typeName, 0, 0);

                var actAtt = actorNode.AddAttrib("ActorData");
                actAtt.BeginWrite();
                actor.SaveSceneData(actAtt);
                actAtt.EndWrite();

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            CSUtility.Support.XndHolder.SaveXND(fileName, holder);

            return true;
        }

        /// <summary>
        /// 读取不需要场景管理的特殊对象
        /// </summary>
        /// <param name="strAbsFolder">地图绝对路径</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public virtual bool LoadSpecialActors(System.String strAbsFolder)
        {
            if (string.IsNullOrEmpty(strAbsFolder))
                return false;

            var fileName = strAbsFolder + "/SpecialActor.dat";
            if (!System.IO.File.Exists(fileName))
                return false;

            var holder = CSUtility.Support.XndHolder.LoadXND(fileName);
            var spNode = holder.Node.FindNode("SpecialActors");
            if (spNode == null)
                return false;

            mSpecialActorsDic.Clear();

            foreach(var actorNode in spNode.GetNodes())
            {
                var typeName = actorNode.GetName();

                var type = CSUtility.Program.GetTypeFromSaveString(typeName);
                if (type == null)
                    continue;

                var actor = System.Activator.CreateInstance(type) as CCore.World.Actor;
                actor.LoadFinished = false;

                var actAtt = actorNode.FindAttrib("ActorData");
                actAtt.BeginRead();
                actor.LoadSceneData(actAtt);
                actAtt.EndRead();
                actor.World = this;
                mSpecialActorsDic.Add(actor.Id, actor);
            }

            CCore.WeatherSystem.IlluminationManager.Instance.ChangedToIllumination(this, mWorldInit.WeatherSystemId);

            return true;
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="strAbsFolder">地图绝对路径</param>
        /// <param name="componentName">组建名称，如果未设置则加载全部标识为自动处理的组件</param>
        /// <returns>成功加载返回true，否则返回false</returns>
        public virtual bool LoadWorld(System.String strAbsFolder, string componentName = "")
        {
            bool retValue = true;
            if (string.IsNullOrEmpty(componentName))
            {
                foreach(var compData in mWorldComponents)
                {
                    if (compData.Value.AutoProcess)
                    {
                        retValue = retValue && compData.Value.Component.LoadWorldComponent(this, strAbsFolder, compData.Value.GetArgumentFunc(this));
                        CCore.Program._OnWorldLoaded(strAbsFolder, compData.Key, this);
                    }
                }
            }
            else
            {
                WorldComponentData data;
                if (!mWorldComponents.TryGetValue(componentName, out data))
                    return false;

                //mWorldFileName = strAbsFolder;
                retValue = data.Component.LoadWorldComponent(this, strAbsFolder, data.GetArgumentFunc(this));
                CCore.Program._OnWorldLoaded(strAbsFolder, componentName, this);
            }
            return retValue;
        }
        /// <summary>
        /// 分析加载的地图
        /// </summary>
        /// <param name="strAbsFolder">加载地图的绝对路径</param>
        /// <param name="componentName">地图名称</param>
        /// <returns>分析成功返回true，否则返回false</returns>
        public virtual bool AnalyseLoadWorld(System.String strAbsFolder, string componentName = "")
        {
            bool retValue = true;
            if (string.IsNullOrEmpty(componentName))
            {
                foreach (var compData in mWorldComponents)
                {
                    if (compData.Value.AutoProcess)
                    {
                        retValue = retValue && compData.Value.Component.LoadWorldComponent(this, strAbsFolder, compData.Value.GetArgumentFunc(this));
                    }
                }
            }
            else
            {
                WorldComponentData data;
                if (!mWorldComponents.TryGetValue(componentName, out data))
                    return false;

                //mWorldFileName = strAbsFolder;
                retValue = data.Component.LoadWorldComponent(this, strAbsFolder, data.GetArgumentFunc(this));
            }
            return retValue;
        }
        /// <summary>
        /// 获取最终加载的地图的绝对路径
        /// </summary>
        /// <param name="compnentName">组件名称，如果未设置则加载全部标识为自动处理的组件</param>
        /// <returns>返回该地图文件的绝对路径</returns>
        public string GetWorldLastLoadedAbsFolder(string compnentName)
        {
            if (string.IsNullOrEmpty(compnentName))
                compnentName = "场景";
            WorldComponentData data;
            if (!mWorldComponents.TryGetValue(compnentName, out data))
                return "";

            return data.Component.LastLoadedAbsFolder;
        }


        #endregion
    }
}
