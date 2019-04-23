using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.World
{
    /// <summary>
    /// 地图组件
    /// </summary>
    public abstract class IWorldComponent
    {
        /// <summary>
        /// 记录最后一次加载的对象的路径
        /// </summary>
        protected string mLastLoadedAbsFolder = "";  // 记录最后一次加载的对象的路径
        /// <summary>
        /// 只读属性，最后一次加载的对象的路径
        /// </summary>
        public string LastLoadedAbsFolder
        {
            get { return mLastLoadedAbsFolder; }
        }
        /// <summary>
        /// 创建地图组件
        /// </summary>
        /// <param name="world">地图对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能对象</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public bool CreateComponent(CCore.World.World world, string worldAbsFolder, object argument)
        {
            if (string.IsNullOrEmpty(worldAbsFolder))
                worldAbsFolder = mLastLoadedAbsFolder;

            return CreateComponentOverride(world, worldAbsFolder, argument);
        }
        /// <summary>
        /// 创建地图组件
        /// </summary>
        /// <param name="world">地图对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能对象</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected abstract bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument);
        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="world">世界类对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool InitializeComponent(CCore.World.World world, string worldAbsFolder, object argument)
        {
            if (string.IsNullOrEmpty(worldAbsFolder))
                worldAbsFolder = mLastLoadedAbsFolder;

            return InitializeComponentOverride(world, worldAbsFolder, argument);
        }
        /// <summary>
        /// 初始化地图组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        protected abstract bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument);
        /// <summary>
        /// 保存地图组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool SaveWorldComponent(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
        {
            if (string.IsNullOrEmpty(worldAbsFolder))
                worldAbsFolder = mLastLoadedAbsFolder;

            return SaveWorldComponentOverride(world, worldAbsFolder, argument, forceSave);
        }
        /// <summary>
        /// 保存地图组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        protected abstract bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave);
        /// <summary>
        /// 加载地图组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public bool LoadWorldComponent(CCore.World.World world, string worldAbsFolder, object argument)
        {
            var retValue = LoadWorldComponentOverride(world, worldAbsFolder, argument);
            if(retValue)
            {
                mLastLoadedAbsFolder = worldAbsFolder;
            }

            return retValue;
        }
        /// <summary>
        /// 加载地图组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        protected abstract bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument);
    }
    /// <summary>
    /// 客户端地图组件场景类
    /// </summary>
    internal class WorldComponent_ClientScene : IWorldComponent
    {
        /// <summary>
        /// 创建场景组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected override bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            if(world.SceneGraph != null)
            {
                world.SceneGraph.Cleanup();
            }

            var sInfo = (CCore.Scene.SceneGraphInfo)argument;
            world.SceneGraph = System.Activator.CreateInstance(sInfo.GetSceneGraphType()) as CCore.Scene.SceneGraph;
            world.SceneGraph.Create(worldAbsFolder, sInfo, world);

            world.TravelTo(0, 0);
            // 增加一个默认的太阳光来照亮场景
            // 默认有一个方向光，否则整个场景是黑的
            var light = new CCore.World.LightActor();
            var lightInit = new CCore.World.LightActorInit();
            lightInit.LightType = CCore.Light.ELightType.Dir;
            light.Initialize(lightInit);
            light.ActorName = "太阳光";
            light._SetId(CCore.World.Actor.GenId());
            light.Light.Ambient = CSUtility.Support.Color.DarkGray;
            light.Light.Diffuse = CSUtility.Support.Color.White;
            light.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager);
            light.Light.Pos = new SlimDX.Vector3(0, 100, 0);
            light.Light.Quat = SlimDX.Quaternion.RotationYawPitchRoll((float)(System.Math.PI/6.0), 0, 0);
            world.AddActor(light);
            world.Tick();

            return true;
        }
        /// <summary>
        /// 初始化地图组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected override bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            if(world.SceneGraph != null)
            {
                world.SceneGraph.Cleanup();
            }

            var sInfo = (CCore.Scene.SceneGraphInfo)argument;
            world.SceneGraph = System.Activator.CreateInstance(sInfo.GetSceneGraphType()) as CCore.Scene.SceneGraph;
            world.SceneGraph.Initialize(worldAbsFolder, sInfo, world);
            
            return true;
        }
        /// <summary>
        /// 保存地图组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        protected override bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
        {
            // 场景
            world.SceneGraph?.SaveScene_ClientScene(worldAbsFolder + "/" + CSUtility.Map.WorldInit.ClientSceneGraphFileName, forceSave);

            // 保存服务器高度图
//             var info = new ServerMap.ServerAltitudeInfo();
//             var scene = world.SceneGraph as Scene.TileScene.TileScene;
//             //
//             info.LevelLengthX = (uint)scene.TileSceneInfo.MapSizeMeterX / scene.TileSceneInfo.LevelXCount;
//             info.LevelLengthX = (uint)scene.TileSceneInfo.MapSizeMeterZ / scene.TileSceneInfo.LevelZCount;
//             info.XMaxLevelCount = scene.TileSceneInfo.LevelXCount;
//             info.ZMaxLevelCount = scene.TileSceneInfo.LevelZCount;
//             info.XValidLevelCount = scene.TileSceneInfo.LevelXCount;
//             info.ZValidLevelCount = scene.TileSceneInfo.LevelZCount;
//             info.MeterPerPixelX = info.LevelLengthX / scene.TileSceneInfo.PatchPerLevelX;
//             info.MeterPerPixelX = info.LevelLengthZ / scene.TileSceneInfo.PatchPerLevelZ;
// 
//             CCore.Support.ServerAltitudeAssist.Instance.InitializeServerHeightMapProxy(info);
            CCore.Support.ServerAltitudeAssist.Instance.BuildServerHeightMapData(world);
            var shmDataFile = worldAbsFolder + "/" + CSUtility.Map.WorldInit.ServerAltitudeFileName;
            var shmPath = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(shmDataFile);
            var shmFile = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(shmDataFile);
            CCore.Support.ServerAltitudeAssist.Instance.Save(shmFile, shmPath);

            return true;
        }
        /// <summary>
        /// 加载地图组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        protected override bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            // 场景
            world.SceneGraph?.LoadScene_ClientScene(worldAbsFolder + "/", CSUtility.Map.WorldInit.ClientSceneGraphFileName);

            return true;
        }
    }
    /// <summary>
    /// 地形组件
    /// </summary>
    internal class WorldComponent_Terrain : IWorldComponent
    {
        string terrainRelativeFile = "Terrain/Terrain.dat";
        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="world">地图对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能对象</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected override bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            if (world.Terrain != null)
            {
                world.Terrain.Cleanup();
            }
            world.Terrain = new CCore.Terrain.Terrain();

            CCore.DllImportAPI.V3DGrassData_InitLoadMeshTemplateCallback(Grass.GrassData.loadMTCallback);

            var terrainInfo = (CCore.Terrain.TerrainInfo)argument;
            world.Terrain.Initialize(worldAbsFolder + "/" + terrainRelativeFile, ref terrainInfo);
            for (UInt32 x = 0; x < terrainInfo.LevelX; x++)
            {
                for (UInt32 z = 0; z < terrainInfo.LevelZ; z++)
                {
                    world.Terrain.CreateLevel(x, z);
                }
            }
            world.Terrain.SetBaseMaterial(CSUtility.Support.IFileConfig.DefaultMaterialId);

            return true;
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        protected override bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            if(world.Terrain != null)
            {
                world.Terrain.Cleanup();
            }
            world.Terrain = new CCore.Terrain.Terrain();

            CCore.DllImportAPI.V3DGrassData_InitLoadMeshTemplateCallback(Grass.GrassData.loadMTCallback);

            var terrainInfo = (CCore.Terrain.TerrainInfo)argument;
            world.Terrain.Initialize(worldAbsFolder + "/" + terrainRelativeFile, ref terrainInfo);

            return true;
        }
        /// <summary>
        /// 保存地形组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        protected override bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
        {
            // 保存地形
            if (world.Terrain != null)
            {
                var terrainDir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(worldAbsFolder + "/" + terrainRelativeFile);
                if (!System.IO.Directory.Exists(terrainDir))
                    System.IO.Directory.CreateDirectory(terrainDir);
                if (world.Terrain != null)
                    world.Terrain.SaveTerrain(worldAbsFolder + "/" + terrainRelativeFile, forceSave);
            }

            // 保存服务器高度图
            CCore.Support.ServerAltitudeAssist.Instance.BuildServerHeightMapData(world);
            var shmDataFile = worldAbsFolder + "/" + CSUtility.Map.WorldInit.ServerAltitudeFileName;
            var shmPath = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(shmDataFile);
            var shmFile = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(shmDataFile);
            CCore.Support.ServerAltitudeAssist.Instance.Save(shmFile, shmPath);

            return true;
        }
        /// <summary>
        /// 加载地形组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">地图功能</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        protected override bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            // 地形
            world.Terrain?.LoadTerrain(worldAbsFolder + "/" + terrainRelativeFile);
            return true;
        }
    }

    // 服务器高度图
    /// <summary>
    /// 服务器高度图组件
    /// </summary>
    internal class WorldComponent_ServerAltitude : IWorldComponent
    {
        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="world">地图对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected override bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            return false;
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        protected override bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {            
            //var shmInfo = (CSUtility.ServerMap.ServerAltitudeInfo)argument;
            //shmInfo.ResetDefault();

            //// 根据场景管理器来计算每个服务器高度图Level的大小
            //UInt32 xValidLevelCount = 0, zValidLevelCount = 0;
            //world.SceneGraph.GetLevelAvailableXZCount(ref xValidLevelCount, ref zValidLevelCount);
            //if (xValidLevelCount == 0 || zValidLevelCount == 0)
            //{
            //    xValidLevelCount = mWorldInit.SceneSizeX;
            //    zValidLevelCount = mWorldInit.SceneSizeZ;
            //}
            //shmInfo.XValidLevelCount = xValidLevelCount;
            //shmInfo.ZValidLevelCount = zValidLevelCount;
            ////float cellX = mSceneGraph.GetXLengthPerLevel();
            ////float cellZ = mSceneGraph.GetZLengthPerLevel();
            //float cellX = mWorldInit.SceneMeterX / mWorldInit.SceneSizeX;
            //float cellZ = mWorldInit.SceneMeterZ / mWorldInit.SceneSizeZ;
            //shmInfo.LevelLengthX = (UInt32)(cellX / shmInfo.MeterPerPixelX);
            //shmInfo.LevelLengthZ = (UInt32)(cellZ / shmInfo.MeterPerPixelZ);

            //var shmFile = mWorldFileName + m_sceneInit.ServerHeightMapFileName;
            //var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(shmFile);
            //var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(shmFile);

            //CCore.Support.ServerAltitudeAssist.Instance.InitializeServerHeightMapProxy(shmInfo);

            return true;
        }
        /// <summary>
        /// 保存组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        protected override bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
        {
            //world.SceneGraph?.SaveScene_Special(worldAbsFolder, (UInt16)argument, forceSave);
            return true;
        }
        /// <summary>
        /// 加载组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        protected override bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            /*world.SceneGraph?.LoadScene_Special(worldAbsFolder, (UInt16)argument);*/
            return true;
        }
    }
    /// <summary>
    /// 服务器组件类
    /// </summary>
    internal class WorldComponent_ServerActors : IWorldComponent
    {
        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="world">地图对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected override bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            return true ;
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        protected override bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            return true;
        }
        /// <summary>
        /// 保存组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        protected override bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
        {            
            world.SceneGraph?.SaveScene_Special(worldAbsFolder, (UInt16)argument, forceSave);
            return true;
        }
        /// <summary>
        /// 加载组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        protected override bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            world.SceneGraph?.LoadScene_Special(worldAbsFolder, (UInt16)argument);
            return true;
        }
    }
    /// <summary>
    /// 寻路组件
    /// </summary>
    internal class WorldComponent_Navigation : IWorldComponent
    {
        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected override bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            InitializeComponentOverride(world, worldAbsFolder, argument);

            return false;
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        protected override bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            var navRelativeFile = "Navigation/Navigation.nav";
            //Navigation.INavigation.Instance.NavigationData = new Navigation.INavigationDataWrapper();

            // 初始化寻路参数
            var navInfo = CCore.Navigation.Navigation.Instance.Info;// (CSUtility.Navigation.NavigationInfo)argument;// new CSUtility.Navigation.INavigationInfo();
            //navInfo.ResetDefault();

            //float cellX = 0, cellZ = 0;
            // 根据场景管理器来计算每个寻路Level的大小
            //mSceneGraph.GetLevelAvailableXZCount(ref navInfo.mXValidLevelCount, ref navInfo.mZValidLevelCount);
            //if (navInfo.mXValidLevelCount == 0 || navInfo.mZValidLevelCount == 0)
            //{
            //navInfo.mXValidLevelCount = mWorldInit.SceneSizeX;
            //navInfo.mZValidLevelCount = mWorldInit.SceneSizeZ;
            ////}
            //cellX = mWorldInit.SceneMeterX / mWorldInit.SceneSizeX;//mSceneGraph.GetXLengthPerLevel();
            //cellZ = mWorldInit.SceneMeterZ / mWorldInit.SceneSizeZ;//mSceneGraph.GetZLengthPerLevel();
            //navInfo.mLevelLengthX = (UInt32)(cellX / navInfo.mMeterPerPixelX);
            //navInfo.mLevelLengthZ = (UInt32)(cellZ / navInfo.mMeterPerPixelZ);

            var navFile = worldAbsFolder + "/" + navRelativeFile;
            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(navFile);
            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(navFile);

            CCore.Navigation.Navigation.Instance.Initialize(file, path, ref navInfo);

            return true;
        }
        /// <summary>
        /// 保存寻路组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        protected override bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
        {
            // 保存寻路
            var navFile = worldAbsFolder + "/Navigation/Navigation.nav";
            var navigationDir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(navFile);
            if (!System.IO.Directory.Exists(navigationDir))
                System.IO.Directory.CreateDirectory(navigationDir);
            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(navFile);
            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(navFile);
            CCore.Navigation.Navigation.Instance.Save(file, path, forceSave);

            return true;
        }
        /// <summary>
        /// 加载组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        protected override bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            var navFile = worldAbsFolder + "/Navigation/Navigation.nav";
            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(navFile);
            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(navFile);
            CCore.Navigation.Navigation.Instance.Load(file, path);
            return true;
        }
    }
    /// <summary>
    /// 后期特效组件类
    /// </summary>
    internal class WorldComponent_PostProcess : IWorldComponent
    {
        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected override bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            return false;
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        protected override bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            return true;
        }
        /// <summary>
        /// 保存组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        protected override bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
        {
            if (string.IsNullOrEmpty(worldAbsFolder))
                return false;

            var absFile = worldAbsFolder + "/PostProcess.dat";

            var holder = CSUtility.Support.XndHolder.NewXNDHolder();
            CSUtility.Support.XndAttrib headAttr = holder.Node.AddAttrib("Header");
            headAttr.BeginWrite();
            headAttr.Write(world.PostProceses.Count);
            headAttr.EndWrite();

            int i = 0;
            foreach (var pp in world.PostProceses)
            {
                CSUtility.Support.XndNode ppNode = holder.Node.AddNode(i.ToString(), 0, 0);
                CSUtility.Support.XndAttrib ppAttrib = ppNode.AddAttrib("data");
                ppAttrib.BeginWrite();
                ppAttrib.Write(CSUtility.Program.GetTypeSaveString(pp.GetType()));
                pp.Write(ppAttrib);
                ppAttrib.EndWrite();
                i++;
            }

            CSUtility.Support.XndHolder.SaveXND(absFile, holder);

            return true;
        }
        /// <summary>
        /// 加载组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        protected override bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            if (string.IsNullOrEmpty(worldAbsFolder))
                return false;

            var absFile = worldAbsFolder + "/PostProcess.dat";

            var holder = CSUtility.Support.XndHolder.LoadXND(absFile);
            if (holder != null)
            {
                var headAttr = holder.Node.FindAttrib("Header");
                headAttr.BeginRead();
                int Count;
                headAttr.Read(out Count);
                headAttr.EndRead();

                for(int i=0; i<Count; i++)
                {
                    var ppNode = holder.Node.FindNode(i.ToString());
                    var ppAttrib = ppNode.FindAttrib("data");
                    ppAttrib.BeginRead();
                    string typeStr;
                    ppAttrib.Read(out typeStr);
                    var type = CSUtility.Program.GetTypeFromSaveString(typeStr);
                    var pp = System.Activator.CreateInstance(type) as CCore.Graphics.PostProcess;
                    pp.Read(ppAttrib);
                    ppAttrib.EndRead();
                    world.AddPostProcess(pp);
                }
            }

            return true;
        }
    }
    /// <summary>
    /// 地图Actor组件类
    /// </summary>
    internal class WorldComponent_WorldActors : IWorldComponent
    {
        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        protected override bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            return InitializeComponentOverride(world, worldAbsFolder, argument);
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        protected override bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            CSUtility.Support.AsyncObjManager<Guid, Actor>.FOnVisitObject visitor = delegate (Guid key, Actor value, object arg)
            {
                value.Cleanup();

                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            world.SpecialActorsDic.For_Each(visitor, null);
            world.SpecialActorsDic.Clear();
            return true;
        }
        /// <summary>
        /// 保存组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <param name="forceSave">是否强制从磁盘加载</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        protected override bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
        {
            world.SaveSpecialActors(worldAbsFolder);
            return true;
        }
        /// <summary>
        /// 加载组件
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="worldAbsFolder">地图的绝对路径</param>
        /// <param name="argument">功能对象</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        protected override bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
        {
            world.LoadSpecialActors(worldAbsFolder);
            return true;
        }
    }
    //internal class WorldComponent_GroupGrid : IWorldComponent
    //{
    //    public string ComponentName
    //    {
    //        get { return "GroupGrid"; }
    //    }

    //    protected CCore.Support.GroupGrid mGroupGrid = new CCore.Support.GroupGrid();
    //    public CCore.Support.GroupGrid GroupGrid
    //    {
    //        get { return mGroupGrid; }
    //    }

    //    public bool CreateComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
    //    {
    //        return false;
    //    }
    //    public bool InitializeComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
    //    {
    //        return true;
    //    }
    //    public bool SaveWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument, bool forceSave)
    //    {
    //        return world.GroupGrid.Save(worldAbsFolder);
    //    }
    //    public bool LoadWorldComponentOverride(CCore.World.World world, string worldAbsFolder, object argument)
    //    {
    //        return world.GroupGrid.Load(worldAbsFolder);
    //    }
    //}

}
