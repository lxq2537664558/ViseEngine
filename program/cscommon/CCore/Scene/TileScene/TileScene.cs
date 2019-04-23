using System;
using System.Collections.Generic;
using System.ComponentModel;
/// <summary>
/// 瓦片场景的命名空间
/// </summary>
namespace CCore.Scene.TileScene
{
    /// <summary>
    /// 获取该游戏类型的寻路名称
    /// </summary>
    /// <param name="gameType">游戏类型值</param>
    /// <returns>返回对应该游戏类型的寻路名称指针</returns>
    public delegate IntPtr Delegate_GetGameTypePathName(UInt16 gameType);
    /// <summary>
    /// 瓦片场景信息
    /// </summary>
    public struct vTileSceneInfo
    {
        /// <summary>
        /// 修改的步数
        /// </summary>
        public SlimDX.Vector3 PatchStep;
        /// <summary>
        /// 场景块X和Z的数量
        /// </summary>
		public uint	LevelXCount, LevelZCount;
        /// <summary>
        /// 每块包含的片参数
        /// </summary>
		public uint	PatchDepthPerLevel;
        /// <summary>
        /// 每个场景块X和Z分别包含的片参数
        /// </summary>
		public uint	PatchPerLevelX, PatchPerLevelZ;
        /// <summary>
        /// 恢复默认信息
        /// </summary>
		public void ResetDefault()
		{
			LevelXCount = LevelZCount = 4;
			PatchDepthPerLevel = 1;//缺省一个Level有32*32个渲染单位
			PatchPerLevelX = PatchPerLevelZ = (uint)(System.Math.Pow(2,PatchDepthPerLevel));
			PatchStep.X = PatchStep.Y = PatchStep.Z = 16.0f;		// 缺省16*16米为一个格子
		}
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="xCount">场景块X的数量</param>
        /// <param name="zCount">场景块Z的数量</param>
        /// <param name="ppLevel">场景块</param>
		public void SetParameter(uint xCount, uint zCount, uint ppLevel)
		{
			LevelXCount = xCount;
			LevelZCount = zCount;
			PatchDepthPerLevel = ppLevel;
			PatchPerLevelX = PatchPerLevelZ = (uint)(System.Math.Pow(2, PatchDepthPerLevel));
			PatchStep.X = PatchStep.Y = PatchStep.Z = 16.0f;
		}
        /// <summary>
        /// 获取场景块X的修正值
        /// </summary>
        /// <returns>返回场景块X的修正值</returns>
		public float GetPatchX(){
			return PatchStep.X;
		}
        /// <summary>
        /// 获取场景块Z的修正值
        /// </summary>
        /// <returns>返回场景块Z的修正值</returns>
		public float GetPatchZ(){
			return PatchStep.Z;
		}
        /// <summary>
        /// 获取场景块X
        /// </summary>
        /// <returns>返回场景块X</returns>
        public float GetLevelX() {
            return PatchStep.X * PatchPerLevelX;
        }
        /// <summary>
        /// 获取场景块Z
        /// </summary>
        /// <returns>返回场景块Z</returns>
		public float GetLevelZ(){
			return PatchStep.Z * PatchPerLevelZ;
		}
    }
    /// <summary>
    /// 瓦片场景信息
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class TileSceneInfo : SceneGraphInfo
	{
        /// <summary>
        /// 瓦片场景信息实例对象
        /// </summary>
        public vTileSceneInfo Info = new vTileSceneInfo();
        /// <summary>
        /// 格子大小(米)
        /// </summary>
        [Description("每个格子的大小，高度不用设置，单位是米")]
        [DisplayName("格子大小(米)")]
        public SlimDX.Vector3 PatchStep
        {
            get { return Info.PatchStep; }
            set
            {
                Info.PatchStep = value;
                OnPropertyChanged("PatchStep");
            }
        }
        /// <summary>
        /// 场景块X的数量
        /// </summary>
        [DisplayName("场景块X")]
        public UInt32 LevelXCount
        {
            get { return Info.LevelXCount; }
            set
            {
                Info.LevelXCount = value;
                UpdateSizeMeterX();
                OnPropertyChanged("LevelXCount");
            }
        }
        /// <summary>
        /// 场景块Z的数量
        /// </summary>
        [DisplayName("场景块Z")]
        public UInt32 LevelZCount
        {
            get { return Info.LevelZCount; }
            set
            {
                Info.LevelZCount = value;
                UpdateSizeMeterZ();
                OnPropertyChanged("LevelZCount");
            }
        }
        /// <summary>
        /// 每块包含的片参数
        /// </summary>
        [Description("每个地形块包含的地形片数量调节参数，计算方式为Pow(2, 数值)")]
        [DisplayName("每块包含的片参数")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 6)]
        public UInt32 PatchDepthPerLevel
        {
            get { return Info.PatchDepthPerLevel; }
            set
            {
                Info.PatchDepthPerLevel = value;
                Info.PatchPerLevelX = Info.PatchPerLevelZ = (uint)(System.Math.Pow(2, PatchDepthPerLevel));
                UpdateSizeMeterX();
                UpdateSizeMeterZ();
                OnPropertyChanged("PatchPerLevelX");
                OnPropertyChanged("PatchPerLevelZ");
                OnPropertyChanged("PatchDepthPerLevel");
            }
        }
        /// <summary>
        /// 每地形块在X方向上包含的地形片数量(自动计算)
        /// </summary>
        [Description("每地形块在X方向上包含的地形片数量(自动计算)")]
        [DisplayName("地形片X")]
        public UInt32 PatchPerLevelX
        {
            get { return Info.PatchPerLevelX; }
        }
        /// <summary>
        /// 每地形块在Z方向上包含的地形片数量(自动计算)
        /// </summary>
        [Description("每地形块在Z方向上包含的地形片数量(自动计算)")]
        [DisplayName("地形片Z")]
        public UInt32 PatchPerLevelZ
        {
            get { return Info.PatchPerLevelZ; }
        }
        float mMapSizeMeterX;
        /// <summary>
        /// 场景X大小
        /// </summary>
        [Description("自动计算，单位是米")]
        [DisplayName("场景X大小")]
        public float MapSizeMeterX
        {
            get { return mMapSizeMeterX; }
        }
        float mMapSizeMeterZ;
        /// <summary>
        /// 场景Z大小
        /// </summary>
        [Description("自动计算，单位是米")]
        [DisplayName("场景Z大小")]
        public float MapSizeMeterZ
        {
            get { return mMapSizeMeterZ; }
        }
        /// <summary>
        /// 瓦片场景信息的构造函数，设置为默认值
        /// </summary>
        public TileSceneInfo()
		{
			ResetDefault();
		}
        /// <summary>
        /// 场景块的初始化函数
        /// </summary>
        /// <param name="worldInit">世界的初始化类</param>
        public override void Initialize(CCore.World.WorldInit worldInit)
        {
            var sInfo = worldInit.SceneGraphInfo as TileSceneInfo;
            if (sInfo != null)
                Info = sInfo.Info;
        }
        /// <summary>
        /// 获取场景类型
        /// </summary>
        /// <returns>返回场景类型为瓦片场景类型</returns>
        public override System.Type GetSceneGraphType()
        {
			return typeof(TileScene);
		}
        /// <summary>
        /// 恢复默认设置
        /// </summary>
		public override void ResetDefault()
        {
			Info.ResetDefault();
            UpdateSizeMeterX();
            UpdateSizeMeterZ();
        }
        /// <summary>
        /// 更新场景块X的大小
        /// </summary>
        private void UpdateSizeMeterX()
        {
            mMapSizeMeterX = Info.GetLevelX() * LevelXCount;
            OnPropertyChanged("MapSizeMeterX");
        }
        /// <summary>
        /// 更新场景块Z的大小
        /// </summary>
        private void UpdateSizeMeterZ()
        {
            mMapSizeMeterZ = Info.GetLevelZ() * LevelZCount;
            OnPropertyChanged("MapSizeMeterZ");
        }
    };
    /// <summary>
    /// 瓦片场景
    /// </summary>
    public class TileScene : SceneGraph
    {
        static Delegate_GetGameTypePathName ggtpEvent = GetGameTypePathName;
        /// <summary>
        /// 获取游戏类型的路径
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <returns>返回该游戏类型的路径指针</returns>
        public static IntPtr GetGameTypePathName(UInt16 gameType)
        {
            var str = ((CSUtility.Component.EActorGameType)gameType).ToString() + "/";
            return System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(str);
        }
        /// <summary>
        /// 瓦片地图对象指针
        /// </summary>
        protected IntPtr mTileScene; // vSceneGraph::vTileScene*
        /// <summary>
        /// 是否锁定该片
        /// </summary>
        protected bool mLockCulling;
        /// <summary>
        /// 是否锁定该片
        /// </summary>
        public bool LockCulling
        {
            get { return mLockCulling; }
            set { mLockCulling = value; }
        }
        /// <summary>
        /// 使用的摄像机
        /// </summary>
        protected CCore.Camera.CameraObject mCullingCamera = null;
        class CLocker { }
        CLocker mLocker = new CLocker();
        private LinkedList<Guid> mWaitRemoveTileObjects = new LinkedList<Guid>();
        /// <summary>
        /// 只读属性，等待清除瓦片对象
        /// </summary>
        public LinkedList<Guid> WaitRemoveTileObjects
        {
            get { return mWaitRemoveTileObjects; }
        }
        private Dictionary<Guid, CCore.World.Actor> mWaitAddTileObjects = new Dictionary<Guid, CCore.World.Actor>();
        /// <summary>
        /// 只读属性，等待添加瓦片对象列表
        /// </summary>
        public Dictionary<Guid, CCore.World.Actor> WaitAddTileObjects
        {
            get { return mWaitAddTileObjects; }
        }
        /// <summary>
        /// 获取等待添加瓦片对象的数量
        /// </summary>
        /// <returns>返回等待添加瓦片对象的数量</returns>
        public int GetWaitAddTileObjectsCount()
        {
            return mWaitAddTileObjects.Count;
        }
        private void AddToWaitAddList(Guid id, CCore.World.Actor actor)//, IntPtr obj)
        {
            lock (mLocker)
            {
                if (mWaitRemoveTileObjects.Contains(id))
                    mWaitRemoveTileObjects.Remove(id);

                if (mWaitAddTileObjects.ContainsKey(id))
                    return;

                mWaitAddTileObjects.Add(id, actor);
            }
        }
        private void AddToWaitRemoveList(Guid id)
        {
            lock (mLocker)
            {
                if (mWaitAddTileObjects.ContainsKey(id))
                    mWaitAddTileObjects.Remove(id);

                if (mWaitRemoveTileObjects.Contains(id))
                    return;

                mWaitRemoveTileObjects.AddLast(id);
            }
        }

        TileSceneInfo mTileSceneInfo;
        /// <summary>
        /// 只读属性，瓦片场景信息
        /// </summary>
        public TileSceneInfo TileSceneInfo
        {
            get { return mTileSceneInfo; }
        }
        CCore.World.World mWorld;
        private Dictionary<System.Guid, CCore.World.Actor> mAllTileObjects = new Dictionary<System.Guid, CCore.World.Actor>();
        /// <summary>
        /// 只读属性，所有的瓦片对象
        /// </summary>
        public Dictionary<System.Guid, CCore.World.Actor> AllTileObjects
        {
            get { return mAllTileObjects; }
        }

        //public static int TileObjectCount
        //{
        //    get 
        //    {
        //        if (sCurTileScene == null)
        //            return 0;
        //        return sCurTileScene.mAllTileObjects.Count; 
        //    }// return mAllTileObjects.Count; }
        //}

        static TileScene sCurTileScene;
        /// <summary>
        /// 只读属性，当前的瓦片场景
        /// </summary>
        public static TileScene CurTileScene
        {
            get { return sCurTileScene; }
        }
        /// <summary>
        /// 析构函数，删除对象，释放内存
        /// </summary>
        ~TileScene()
        {
            Cleanup();
        }
        
        CCore.Scene.TileScene.Delegate_TileObject_CreateTileObject createTileObjectEvent = null;
        CCore.Scene.TileScene.Delegate_TileObject_LoadFinish tileObjectLoadFinishEvent = null;

        private IntPtr CheckTileObjectExistAndOperationToAdd(Guid id)
        {
            lock (mLocker)
            {
                CCore.World.Actor actor = null;

                if (mWaitRemoveTileObjects.Contains(id))
                {
                    mWaitRemoveTileObjects.Remove(id);
                }

                if (mAllTileObjects.TryGetValue(id, out actor))
                    return actor.ActorPtr;

                if (mWaitAddTileObjects.TryGetValue(id, out actor))
                    return actor.ActorPtr;

                return IntPtr.Zero;
            }

        }
        /// <summary>
        /// 创建瓦片对象
        /// </summary>
        /// <param name="id">对象ID的指针</param>
        /// <param name="strName">对象名称的指针</param>
        /// <param name="scene">创建的场景指针</param>
        /// <param name="bIsNewObject">是否为新建的对象</param>
        /// <returns>返回创建的对象的指针</returns>
        public IntPtr CreateTileObject(IntPtr id, IntPtr strName, IntPtr scene, ref Int32 bIsNewObject)
        {
            unsafe
            {
                System.Guid actorId = *((System.Guid*)(id.ToPointer()));
                //System.IntPtr tileObject;
                CCore.World.Actor actor = null;

                IntPtr tempPtr = CheckTileObjectExistAndOperationToAdd(actorId);
                if (tempPtr != IntPtr.Zero)
                {
                    bIsNewObject = 0;
                    return tempPtr;
                }
                else
                {
                    bIsNewObject = 1;

                    var typeName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(strName);
                    
                    var type = CSUtility.Program.GetTypeFromSaveString(typeName);//IEngine.Instance.GetType(typeName);
                    if (type != null)
                    {
                        actor = (CCore.World.Actor)System.Activator.CreateInstance(type);
                        actor.LoadFinished = false;

                    }
                    else
                    {
                        return IntPtr.Zero;
                    }

                    DllImportAPI.vTileObject_SetTileScene(actor.ActorPtr, scene);
                    if (scene != mTileScene)
                    {
                        //System.Diagnostics.Debugger.Break();
                        Log.FileLog.WriteLine("SetTileScene: mTileScene = null");
                        return IntPtr.Zero;
                    }

                    AddToWaitAddList(actorId, actor);

                    return actor.ActorPtr;
                }
            }
        }
        /// <summary>
        /// 瓦片对象加载完成
        /// </summary>
        /// <param name="id">对象ID指针</param>
        /// <param name="actorPtr">Actor对象指针</param>
        public void TileObjectLoadFinish(IntPtr id, IntPtr actorPtr)
        {
            unsafe
            {
                if (actorPtr == IntPtr.Zero)
                    return;
                var handle = (System.Runtime.InteropServices.GCHandle)(actorPtr);
                var act = handle.Target as CCore.World.Actor;
                if (act == null)
                    return;

                act.LoadFinished = true;
            }
        }

        private bool AddTileObjectSceneManager(CCore.World.Actor act, IntPtr scene)
        {
            var client = CCore.Engine.Instance.Client;
            if (client.MainWorld != null && client.ChiefRole == act)
            {
                var saveTile = DllImportAPI.vTileObject_GetTileScene(act.ActorPtr);
                if (saveTile != scene)
                {
                    DllImportAPI.vTileObject_SetTileScene(act.ActorPtr, scene);
                }
            }
            if (act.HasFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor))
                return false;

            act.OnEnterScene(this);

            if (scene != mTileScene)
            {
                System.Diagnostics.Debugger.Break();
            }
            CCore.Scene.TileScene.TileObject.Bind2SceneGraph(mWorld, act, scene);

            // HitProxy
            if (act.Visual != null)
                act.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(act.Id));

            //if (act.HasFlag(CSUtility.Component.IActorInitBase.EActorFlag.SaveWithClient))
            //{
            //    // 服务器高度图置脏
            //    if (act.Placement != null)
            //    {
            //        SlimDX.Vector3 loc = act.Placement.GetLocation();
            //        var lvlX = IDllImportAPI.vTileScene_GetLevelIndexX(mTileScene, loc.X);
            //        var lvlZ = IDllImportAPI.vTileScene_GetLevelIndexZ(mTileScene, loc.Z);
            //        MidLayer.IServerHeightMapAssist.Instance.SetLevelDirty(lvlX, lvlZ);
            //    }
            //}

            return true;
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public override void Cleanup()
        {
            unsafe
            {
                lock (mLocker)
                {
                    foreach (var obj in mWaitAddTileObjects)
                    {
                        if (mAllTileObjects.ContainsKey(obj.Key))
                            continue;

                        if (mTileScene != IntPtr.Zero)
                        {
                            AddTileObjectSceneManager(obj.Value, mTileScene);
                            DllImportAPI.vTileObject_Tick(obj.Value.ActorPtr);
                            mAllTileObjects.Add(obj.Key, obj.Value);
                        }
                    }
                    mWaitAddTileObjects.Clear();

                    //IDllImportAPI.vTileScene_AddRef(mTileScene);
                    foreach (var actor in mAllTileObjects)
                    {
                        if (mTileScene != IntPtr.Zero)
                        {
                            var actorPtr = actor.Value.ActorPtr;
                            var saveTile = DllImportAPI.vTileObject_GetTileScene(actorPtr);
                        //    System.Diagnostics.Debug.Assert(saveTile != IntPtr.Zero);
                            if (saveTile !=null && saveTile != mTileScene)
                            {
                                DllImportAPI.vTileObject_SetTileScene(actorPtr, mTileScene);
                            }                            
                            DllImportAPI.vTileObject_RemoveFromAllReferPatches(actorPtr);
                            actor.Value.OnRemoveFromScene(this);
                            var refCount = DllImportAPI.vTileObject_GetRefCount(actorPtr);
                            if (refCount != 1)
                            {
                                //System.Diagnostics.Debugger.Break();
                                Log.FileLog.WriteLine(string.Format("Actor的RefCount={0}",refCount));
                            }
                        }
                        actor.Value.Cleanup();
                        actor.Value.World = null;
                    }
                    mAllTileObjects.Clear();
                    //IDllImportAPI.vTileScene_Release(mTileScene);
                }

                //IDllImportAPI.v3dCamera_Release(mCullingCamera);
                //mCullingCamera = IntPtr.Zero;
                if (mTileScene != IntPtr.Zero)
                {
                    DllImportAPI.vTileScene_Release(mTileScene);
                    mTileScene = IntPtr.Zero;
                }
            }

            mIsValid = false;
        }
        /// <summary>
        /// 初始化场景管理
        /// </summary>
        /// <param name="absMapFolder">地图所在路径</param>
        /// <param name="info">场景管理器参数</param>
        /// <param name="hostWorld">使用此场景管理器的地图</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(string absMapFolder, SceneGraphInfo info, CCore.World.World hostWorld)
        {
            unsafe
            {
                Cleanup();

                mWorld = hostWorld;
                mTileSceneInfo = info as TileSceneInfo;

                var fullFileName = absMapFolder + "/" + CSUtility.Map.WorldInit.ClientSceneGraphFileName;
                var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(fullFileName);
                var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(fullFileName);

                IntPtr thisHandle = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(this));
                mTileScene = DllImportAPI.vTileScene_New(thisHandle);
                

                fixed (vTileSceneInfo* pInfo = &mTileSceneInfo.Info)
                {
                    if (DllImportAPI.vTileScene_ConstructTileScene(mTileScene, file, path, pInfo) == 0)
                    {
                        return false;
                    }
                }

                mCullingCamera = new CCore.Camera.CameraObject();
                mCullingCamera.Initialize(new CCore.Camera.CameraInit());
                //mCullingCamera = IDllImportAPI.v3dCamera_New(IEngine.Instance.Client.Graphics.Device);

                DllImportAPI.vTileScene_InitializeEvent(mTileScene, ggtpEvent);

                mIsValid = true;
                return true;
            }
        }
        /// <summary>
        /// 创建场景管理
        /// </summary>
        /// <param name="absMapFolder">地图所在路径</param>
        /// <param name="info">场景管理器参数</param>
        /// <param name="hostWorld">使用此场景管理器的地图</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public override bool Create(string absMapFolder, SceneGraphInfo info, CCore.World.World hostWorld)
        {
            unsafe
            {
                if (!Initialize(absMapFolder, info, hostWorld))
                    return false;

                TileSceneInfo ti = info as TileSceneInfo;

                for (UInt32 x = 0; x < ti.LevelXCount; x++)
                {
                    for (UInt32 z = 0; z < ti.LevelZCount; z++)
                    {
                        AddLevel(x, z);
                    }
                }

                return true;
            }
        }
        /// <summary>
        /// 加载客户端场景
        /// </summary>
        /// <param name="mapPath">地图路径</param>
        /// <param name="name">场景名称</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadScene_ClientScene(string mapPath, string name)
        {
            Cleanup();

            sCurTileScene = this;
            
            mapPath = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mapPath);
            var fullName = mapPath + name;
            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(fullName);
            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(fullName);

            var thisHandle = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(this));
            mTileScene = DllImportAPI.vTileScene_New(thisHandle);

            createTileObjectEvent = this.CreateTileObject;
            tileObjectLoadFinishEvent = this.TileObjectLoadFinish;

            unsafe
            {
                DllImportAPI.vTileObject_SetCreateTileObjectEvent(mTileScene, createTileObjectEvent);
                DllImportAPI.vTileObject_SetTileObjectLoadFinishEvent(mTileScene, tileObjectLoadFinishEvent);
            }

            if (DllImportAPI.vTileScene_LoadTileScene(mTileScene, file, path, mapPath) == 0)
                return false;

            mCullingCamera = new CCore.Camera.CameraObject();
            mCullingCamera.Initialize(new CCore.Camera.CameraInit());
            //mCullingCamera = IDllImportAPI.v3dCamera_New(IEngine.Instance.Client.Graphics.Device);
            
            DllImportAPI.vTileScene_InitializeEvent(mTileScene, ggtpEvent);

            mIsValid = true;
            return true;
        }
        /// <summary>
        /// 在所有对象固定前一直刷新
        /// </summary>
        private void TickUntilAllObjectFixed()
        {
            while (mWaitAddTileObjects.Count > 0 || mWaitRemoveTileObjects.Count > 0)
            {
                this.Tick();

                System.Threading.Thread.Sleep(50);
            }
        }

        //public override bool SaveScene_All(string name, string serverName, bool forceSave)
        //{
        //    unsafe
        //    {
        //        if (mTileScene == IntPtr.Zero)
        //            return false;

        //        TickUntilAllObjectFixed();

        //        if (System.String.IsNullOrEmpty(name))
        //        {
        //            DllImportAPI.vTileScene_SaveDirtyLevel(mTileScene, null, null, forceSave);
        //            DllImportAPI.vTileScene_SaveServerDirtyLevel(mTileScene, null, null, (int)CSUtility.Component.EActorGameType.Unknow, forceSave);
        //        }
        //        else
        //        {
				    //var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(name);
				    //if(!System.IO.Directory.Exists(path))
					   // System.IO.Directory.CreateDirectory(path);
				    //var serverPath = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(serverName);
				    //if(!System.IO.Directory.Exists(serverPath))
					   // System.IO.Directory.CreateDirectory(serverPath);

				    //var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(name);
        //            DllImportAPI.vTileScene_SaveDirtyLevel(mTileScene, file, path, forceSave);
        //            DllImportAPI.vTileScene_SaveServerDirtyLevel(mTileScene, file, serverPath, (int)CSUtility.Component.EActorGameType.Unknow, forceSave);
        //        }

        //        return true;
        //    }
        //}
        /// <summary>
        /// 保存客户端场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="forceSave">是否强制保存</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public override bool SaveScene_ClientScene(string name, bool forceSave)
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return false;

                TickUntilAllObjectFixed();

                if (System.String.IsNullOrEmpty(name))
                {
                    DllImportAPI.vTileScene_SaveDirtyLevel(mTileScene, null, null, forceSave);
                }
                else
                {
                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(name);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(name);
                    DllImportAPI.vTileScene_SaveDirtyLevel(mTileScene, file, path, forceSave);
                }

                return true;
            }
        }

        //public override bool SaveScene_NPC(string name, bool forceSave)
        //{
        //    unsafe
        //    {
        //        if (mTileScene == IntPtr.Zero)
        //            return false;

        //        TickUntilAllObjectFixed();

        //        if (System.String.IsNullOrEmpty(name))
        //        {
        //            DllImportAPI.vTileScene_SaveServerDirtyLevel(mTileScene, null, null, (int)CSUtility.Component.EActorGameType.NpcInitializer, forceSave);
        //        }
        //        else
        //        {
        //            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(name);
        //            if (!System.IO.Directory.Exists(path))
        //                System.IO.Directory.CreateDirectory(path);

        //            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(name);
        //            DllImportAPI.vTileScene_SaveServerDirtyLevel(mTileScene, file, path, (int)CSUtility.Component.EActorGameType.NpcInitializer, forceSave);
        //        }

        //        return true;
        //    }
        //}

        //public override bool SaveScene_Trigger(string name, bool forceSave)
        //{
        //    unsafe
        //    {
        //        if (mTileScene == IntPtr.Zero)
        //            return false;

        //        if (System.String.IsNullOrEmpty(name))
        //        {
        //            IDllImportAPI.vTileScene_SaveServerDirtyLevel(mTileScene, null, null, (int)CSUtility.Component.EActorGameType.Trigger, forceSave);
        //        }
        //        else
        //        {
        //            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(name);
        //            if (!System.IO.Directory.Exists(path))
        //                System.IO.Directory.CreateDirectory(path);

        //            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(name);
        //            IDllImportAPI.vTileScene_SaveServerDirtyLevel(mTileScene, file, path, (int)CSUtility.Component.EActorGameType.Trigger, forceSave);
        //        }

        //        return true;
        //    }
        //}
        /// <summary>
        /// 保存特殊的场景
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="actorType">Actor类型</param>
        /// <param name="forceSave">是否强制保存</param>
        /// <returns>成功保存返回true，否则返回false</returns>
        public override bool SaveScene_Special(string path, UInt16 actorType, bool forceSave)
        {
            if (mTileScene == IntPtr.Zero)
                return false;

            if (string.IsNullOrEmpty(path))
                return false;

            TickUntilAllObjectFixed();

            unsafe
            {
                var fullPath = path + "/" + ((CSUtility.Component.EActorGameType)actorType).ToString() + "\\";
                if (!System.IO.Directory.Exists(fullPath))
                    System.IO.Directory.CreateDirectory(fullPath);

                DllImportAPI.vTileScene_SaveSpecialDirtyLevel(mTileScene, fullPath, actorType, forceSave);
            }

            return true;
        }
        /// <summary>
        /// 加载特殊场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="actorType">Actor类型</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadScene_Special(string name, UInt16 actorType)
        {
            // 读取服务器端地图数据需要放在读取客户端地图数据之后
            if (mTileScene == IntPtr.Zero)
                return false;

            unsafe
            {
                DllImportAPI.vTileScene_SetShowServerObject(mTileScene, actorType, true);

                var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(name);
			    var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(name);

                if (DllImportAPI.vTileScene_LoadServerTileScene(mTileScene, file, path) == 0)
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 创建地形块
        /// </summary>
        /// <param name="iCol">列</param>
        /// <param name="iRow">行</param>
        public void CreateLevel(UInt16 iCol, UInt16 iRow)
        {
            if (mTileScene == IntPtr.Zero)
                return;

            unsafe
            {
                DllImportAPI.vTileScene_NewLevel(mTileScene, iCol, iRow);
            }
        }

        float mPrevTravelToX = 0;
        float mPrevTravelToZ = 0;
        /// <summary>
        /// 将坐标转换到块状地图中
        /// </summary>
        /// <param name="x">X轴的坐标</param>
        /// <param name="z">Z轴的坐标</param>
        /// <returns>转换成功返回true，否则返回false</returns>
        public override bool TravelTo(float x, float z)
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return false;

                if (x < 0)
                    x = 0;
                else if (x > mTileSceneInfo.MapSizeMeterX)
                    x = mTileSceneInfo.MapSizeMeterX;

                if (z < 0)
                    z = 0;
                else if (z > mTileSceneInfo.MapSizeMeterZ)
                    z = mTileSceneInfo.MapSizeMeterZ;

                if (System.Math.Abs(x - mPrevTravelToX) > 16 || System.Math.Abs(z - mPrevTravelToZ) > 16)
                {
                    KickTileObjectTick = 10;
                }
                mPrevTravelToX = x;
                mPrevTravelToZ = z;
                return ((DllImportAPI.vTileScene_TravelTo(mTileScene, x, z, Engine.Instance.GetFrameMillisecond()) != 0) ? true : false);
            }
        }
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="lockCulling">是否进行绑定</param>
        public override void SetLockCulling(bool lockCulling)
        {
            LockCulling = lockCulling;
        }
        /// <summary>
        /// 获取绑定对象
        /// </summary>
        /// <returns>返回绑定对象</returns>
        public override bool GetLockCulling()
        {
            return LockCulling;
        }
        /// <summary>
        /// 是否可以获取场景
        /// </summary>
        /// <returns>返回获取的场景</returns>
        public bool IsAvailable()
        {
            return mTileScene != IntPtr.Zero;
        }
        /// <summary>
        /// 可访问的场景的X轴和Z轴方向的场景块数量
        /// </summary>
        /// <param name="x">X轴</param>
        /// <param name="z">Z轴</param>
        public override void GetLevelAvailableXZCount(ref uint x, ref uint z)
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                {
                    x = 1;
                    z = 1;
                    return;
                }

                x = 0;
                z = 0;

                vTileSceneInfo info = new vTileSceneInfo();
                DllImportAPI.vTileScene_GetTileInfo(mTileScene, &info);
                for (UInt32 i = 0; i < info.LevelZCount; i++)
                {
                    for (UInt32 j = 0; j < info.LevelXCount; j++)
                    {
                        var level = DllImportAPI.vTileScene_GetLevel(mTileScene, (UInt16)j, (UInt16)i);
                        if (level != IntPtr.Zero)
                        {
                            if(x < j)
                                x = j;
                            if(z < i)
                                z = i;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取每个场景块X轴方向的长度
        /// </summary>
        /// <returns>返回每个场景块X轴方向的长度</returns>
        public override float GetXLengthPerLevel()
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return 128;

                vTileSceneInfo info = new vTileSceneInfo();
                DllImportAPI.vTileScene_GetTileInfo(mTileScene, &info);
                return info.PatchStep.X * info.PatchPerLevelX;
            }
        }
        /// <summary>
        /// 获取每个场景块Z轴方向的长度
        /// </summary>
        /// <returns>返回每个场景块Z轴方向的长度</returns>
        public override float GetZLengthPerLevel()
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return 128;

                vTileSceneInfo info = new vTileSceneInfo();
                DllImportAPI.vTileScene_GetTileInfo(mTileScene, &info);
                return info.PatchStep.Z * info.PatchPerLevelZ;
            }
        }
        /// <summary>
        /// 网格线检查
        /// </summary>
        /// <param name="start">线段起始点</param>
        /// <param name="end">线段终点</param>
        /// <param name="result">点击结果</param>
        /// <returns>检查无错误返回true，否则返回false</returns>
        public override bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result)
        {
            return LineCheck(ref start, ref end, ref result, false);
        }
        /// <summary>
        /// 网格线检查
        /// </summary>
        /// <param name="start">线段起始点</param>
        /// <param name="end">线段终点</param>
        /// <param name="result">点击结果</param>
        /// <param name="withDeletedPatch">是否删除地形片</param>
        /// <returns>检查无错误返回true，否则返回false</returns>
        public virtual bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result, bool withDeletedPatch)
        {
            unsafe
            {
                if (mTileScene != IntPtr.Zero)
                {
                    fixed(SlimDX.Vector3* pinStart = &start)
                    {
                        fixed(SlimDX.Vector3* pinEnd = &end)
                        {
                            fixed(CSUtility.Support.stHitResult* pinResult = &result)
                            {
                                var retValue = DllImportAPI.vTileScene_LineCheck(mTileScene, pinStart, pinEnd, pinResult, withDeletedPatch, IntPtr.Zero);
                                return (retValue != 0) ? true : false;
                            }
                        }
                    }
                }

                return false;
            }
        }
        /// <summary>
        /// 网格线检查
        /// </summary>
        /// <param name="start">线段起点</param>
        /// <param name="end">线段终点</param>
        /// <param name="result">点击结果</param>
        /// <param name="exceptActor">其他的Actor对象列表</param>
        /// <returns>检查无错误返回true，否则返回false</returns>
        public override bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result, List<CCore.World.Actor> exceptActor)
        {
            unsafe
            {
                if (mTileScene != IntPtr.Zero)
                {
                    fixed (SlimDX.Vector3* pinStart = &start)
                    {
                        fixed (SlimDX.Vector3* pinEnd = &end)
                        {
                            fixed (CSUtility.Support.stHitResult* pinResult = &result)
                            {
                                IntPtr pinList = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(exceptActor));
                                var retValue = DllImportAPI.vTileScene_LineCheck(mTileScene, pinStart, pinEnd, pinResult, false, pinList);
                                return (retValue != 0) ? true : false;
                            }
                        }
                    }
                }

                return false;
            }
        }

        int KickTileObjectTick = 10;
        int Cpp_TileObjectTickTime = 500;
        private static CSUtility.Performance.PerfCounter TickActorTimer = new CSUtility.Performance.PerfCounter("TileScene.TickActor");
        private static CSUtility.Performance.PerfCounter TickActorOtherTimer = new CSUtility.Performance.PerfCounter("TileScene.TickActor.Other");
        /// <summary>
        /// 每帧调用
        /// </summary>
        public override void Tick()
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return;

                DllImportAPI.vTileScene_KickOffCache(mTileScene, Engine.Instance.GetFrameMillisecond(), 15000);
                DllImportAPI.vTileScene_Tick(mTileScene, Engine.Instance.GetElapsedMillisecond(), Engine.Instance.GetFrameMillisecond());
                

                bool bCppTileObjectTick = false;
                Cpp_TileObjectTickTime -= (int)Engine.Instance.GetElapsedMillisecond();
                if (Cpp_TileObjectTickTime <= 0)
                {
                    Cpp_TileObjectTickTime = 500;
                    bCppTileObjectTick = true;
                }
                try
                {
                    bool DoOutTest = false;
                    if (KickTileObjectTick <= 0)
                    {
                        DoOutTest = true;
                        KickTileObjectTick = 10;
                    }
                    else
                    {
                        KickTileObjectTick--;
                    }
                    TickActorTimer.Begin();
                    foreach (var i in mAllTileObjects)
                    {
                        TickActorOtherTimer.Begin();
                        if (bCppTileObjectTick)
                        {
                            DllImportAPI.vTileObject_Tick(i.Value.ActorPtr);
                            if (DoOutTest)
                            {
                                var loaded = DllImportAPI.vTileObject_IsOutofCurrentLevels(i.Value.ActorPtr);
                                if (loaded != 0)
                                {
                                    AddToWaitRemoveList(i.Key);
                                    continue;
                                }
                            }
                        }

                        var tileScene = DllImportAPI.vTileObject_GetTileScene(i.Value.ActorPtr);
                        if (tileScene != IntPtr.Zero && tileScene != mTileScene)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                        TickActorOtherTimer.End();
                        i.Value.Tick(Engine.Instance.GetElapsedMillisecond());
                    }
                    TickActorTimer.End();

                    lock (mLocker)
                    {
                        List<Guid> mRemoveList = new List<Guid>();
                        foreach (var obj in mWaitAddTileObjects)
                        {
                            if (!obj.Value.LoadFinished)
                                continue;

                            if (mAllTileObjects.ContainsKey(obj.Key))
                            {
                                mRemoveList.Add(obj.Key);
                                Log.FileLog.WriteLine("WaitAddTileObjects AllTileObjects.ContainsKey");
                                continue;
                            }

                            AddTileObjectSceneManager(obj.Value, mTileScene);
                            DllImportAPI.vTileObject_Tick(obj.Value.ActorPtr);
                            mAllTileObjects.Add(obj.Key, obj.Value);
                            obj.Value.World = mWorld;
                            mRemoveList.Add(obj.Key);
                        }
                        foreach (var key in mRemoveList)
                        {
                            mWaitAddTileObjects.Remove(key);
                        }
                        mRemoveList.Clear();

                        foreach (var id in mWaitRemoveTileObjects)
                        {
                            CCore.World.Actor actor;
                            if (mAllTileObjects.TryGetValue(id, out actor))
                            {
                                //if (actor.Placement != null)
                                //{
                                //    SlimDX.Vector3 loc = actor.Placement.GetLocation();
                                //    //IDllImportAPI.vTileObject_GetLocation(value, &loc);

                                //    var lvlX = IDllImportAPI.vTileScene_GetLevelIndexX(mTileScene, loc.X);
                                //    var lvlZ = IDllImportAPI.vTileScene_GetLevelIndexZ(mTileScene, loc.Z);
                                //    MidLayer.IServerHeightMapAssist.Instance.SetLevelDirty(lvlX, lvlZ);
                                //}

                                DllImportAPI.vTileObject_RemoveFromAllReferPatches(actor.ActorPtr);
                                actor.OnRemoveFromScene(this);
                                World.Actor tempActor;
                                if(mAllTileObjects.TryGetValue(id, out tempActor))
                                {
                                    tempActor.World = null;
                                }
                                mAllTileObjects.Remove(id);
                                actor.World = null;
                            }
                        }
                        mWaitRemoveTileObjects.Clear();
                    }

                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }
        }
        /// <summary>
        /// 添加Actor对象
        /// </summary>
        /// <param name="act">Actor对象</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public override bool AddActor(CCore.World.Actor act)
        {
            unsafe
            {
                AddToWaitAddList(act.Id, act);

                UInt16 minX = 0, maxX = 0, minZ = 0, maxZ = 0;
                var result = DllImportAPI.vTileScene_SetLevelDirty(mTileScene, act.ActorPtr, CCore.Engine.Instance.GetFrameMillisecond(), &minX, &maxX, &minZ, &maxZ);
                if (act.GameType == ((UInt16)CSUtility.Component.EActorGameType.Common))
                {
                    // 处理服务器高度图置脏
                    if (result != 0)
                    {
                        CCore.Support.ServerAltitudeAssist.Instance.UpdateServerHeightMapLevelDirtys(minX, maxX, minZ, maxZ);
                    }
                }

                return true;
            }
        }
        /// <summary>
        /// 删除相应的Actor对象
        /// </summary>
        /// <param name="act">Actor对象</param>
        /// <returns>成功删除返回true，否则返回false</returns>
        public override bool RemoveActor(CCore.World.Actor act)
        {
            unsafe
            {
                 AddToWaitRemoveList(act.Id);

                UInt16 minX = 0, maxX = 0, minZ = 0, maxZ = 0;
                var result = DllImportAPI.vTileScene_SetLevelDirty(mTileScene, act.ActorPtr, CCore.Engine.Instance.GetFrameMillisecond(), &minX, &maxX, &minZ, &maxZ);            
                if (act.GameType == ((UInt16)CSUtility.Component.EActorGameType.Common))
                {
                    // 处理服务器高度图置脏
                    if (result != 0)
                    {
                        CCore.Support.ServerAltitudeAssist.Instance.UpdateServerHeightMapLevelDirtys(minX, maxX, minZ, maxZ);
                    }
                }

                return true;
            }
        }
        /// <summary>
        /// 删除所有的Actor对象
        /// </summary>
        public override void RemoveAllActor()
        {
        }
        /// <summary>
        /// 根据ActorID查找Actor对象
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>返回找到的Actor对象</returns>
        public override CCore.World.Actor FindActor(ref Guid id)
        {
            CCore.World.Actor outActor;
            if (mAllTileObjects.TryGetValue(id, out outActor))
                return outActor;
            
            return null;
        }

        #region 遍历所有Actor的回调
        /// <summary>
        /// 保存所有的Actor的对象结构
        /// </summary>
        public struct AllActorsArg
        {
            /// <summary>
            /// 场景中的Actor对象列表
            /// </summary>
            public List<CCore.World.Actor> Actors;// = new List<IActor>();
        }
        /// <summary>
        /// 声明访问场景中所有的Actor对象时调用的委托对象
        /// </summary>
        /// <param name="obj">场景地图的对象指针</param>
        /// <param name="arg">Actor对象指针</param>
        /// <returns>成功获取返回true，否则返回false</returns>
        public delegate bool Delegate_OnVisitTileObject_GetAllActors(IntPtr obj, IntPtr arg);
        static Delegate_OnVisitTileObject_GetAllActors DOnVisitTileObject_GetAllActors = OnVisitTileObject_GetAllActors;
        static bool OnVisitTileObject_GetAllActors(IntPtr obj, IntPtr arg)
        {
            var argHandle = (System.Runtime.InteropServices.GCHandle)(arg);
            var allActors = (AllActorsArg)argHandle.Target;

            if (obj == IntPtr.Zero)
                return true;

            unsafe
            {
                var handlePtr = DllImportAPI.vTileObject_GetCSActor(obj);
                if (handlePtr == IntPtr.Zero)
                    return false;
                var handle = ((System.Runtime.InteropServices.GCHandle)(handlePtr));
                if(handle == null)
                    return false;

                var act = handle.Target as CCore.World.Actor;
                if (act == null)
                    return false;

                if (allActors.Actors == null)
                    allActors.Actors = new List<CCore.World.Actor>();
                allActors.Actors.Add(act);
                return true;
            }
        }
        static uint TourTileObjectSerialId = 0;
        /// <summary>
        /// Actor对象的动作
        /// </summary>
        /// <param name="actorType">Actor对象的类型</param>
        /// <param name="process">Actor对象的动作列表</param>
        public override void ProcessActors(UInt16 actorType, Action<World.Actor> process)
        {
            if (process == null)
                return;

            lock (mLocker)
            {
                if (actorType == ((UInt16)CSUtility.Component.EActorGameType.Unknow))
                {
                    foreach(var actor in mAllTileObjects.Values)
                    {
                        process(actor);
                    }
                }
                else
                {
                    foreach (var i in mAllTileObjects)
                    {
                        if (i.Value.GameType == actorType)
                            process(i.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 获取某一类型的所有Actor对象
        /// </summary>
        /// <param name="actorType">Actor的类型</param>
        /// <returns>返回某一类型的所有Actor对象</returns>
        public override List<CCore.World.Actor> GetActors(UInt16 actorType)
        {
            var retActors = new List<CCore.World.Actor>();

            lock (mLocker)
            {
                if (actorType == ((UInt16)CSUtility.Component.EActorGameType.Unknow))
                {
                    retActors.AddRange(mAllTileObjects.Values);
                }
                else
                {
                    foreach (var i in mAllTileObjects)
                    {
                        if (i.Value.GameType == actorType)
                            retActors.Add(i.Value);
                    }
                }
            }
            return retActors;
        }
        /// <summary>
        /// 获取对应区域的所有的Actor对象
        /// </summary>
        /// <param name="vStart">起点坐标</param>
        /// <param name="vEnd">终点坐标</param>
        /// <param name="actorType">Actor类型</param>
        /// <returns>返回对应区域的所有的Actor对象</returns>
        public override List<CCore.World.Actor> GetActors(ref SlimDX.Vector3 vStart, ref SlimDX.Vector3 vEnd, UInt16 actorType)
        {
            var retActors = new List<CCore.World.Actor>();

            if (mTileScene == IntPtr.Zero)
                return retActors;

            unsafe
            {
                fixed (SlimDX.Vector3* pinStart = &vStart)
                {
                    fixed (SlimDX.Vector3* pinEnd = &vEnd)
                    {
                        AllActorsArg arg;
                        arg.Actors = retActors;

                        IntPtr pinArg = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(arg));
                        TourTileObjectSerialId++;
                        DllImportAPI.vTileScene_TourActorsWithRange(mTileScene, DOnVisitTileObject_GetAllActors, pinStart, pinEnd, Engine.Instance.GetFrameMillisecond(), actorType, TourTileObjectSerialId, pinArg);
                        var argHandle = (System.Runtime.InteropServices.GCHandle)(pinArg);
                        argHandle.Target = null;
                        argHandle.Free();
                    }
                }
            }

            return retActors;
        }

        #endregion

#region RenderVisible

        /// <summary>
        /// 块状地图访问器
        /// </summary>
        public class TileVisitorArg
        {
            /// <summary>
            /// 摄像机对象
            /// </summary>
            public CCore.Camera.CameraObject Camera;
            /// <summary>
            /// 数量
            /// </summary>
            public int Count;
            /// <summary>
            /// 对象数量
            /// </summary>
            public int ObjCount;
            /// <summary>
            /// 时间
            /// </summary>
            public Int64 Time;
            /// <summary>
            /// 渲染环境
            /// </summary>
            public CCore.Graphics.REnviroment RenderEnv;
            /// <summary>
            /// ID
            /// </summary>
            public uint SerialId;
            /// <summary>
            /// 包含类型
            /// </summary>
            public CONTAIN_TYPE ContainType;
            /// <summary>
            /// 所处的世界对象
            /// </summary>
            public CCore.World.World OwnerWorld;
        }
        /// <summary>
        /// 是否寻路时停止
        /// </summary>
        public bool StopAtPatch = false;
        /// <summary>
        /// 是否忽视子对象
        /// </summary>
        public bool IgnoreChild = false;
        /// <summary>
        /// 声明块状场景访问时调用的委托事件
        /// </summary>
        /// <param name="patch">场景块指针</param>
        /// <param name="type">包含类型</param>
        /// <param name="arg">Actor对象</param>
        public delegate void Delegate_TileVisibleVisitor_Visit(IntPtr patch, CONTAIN_TYPE type, IntPtr arg);
        /// <summary>
        /// 声明访问块状对象时调用的委托对象
        /// </summary>
        /// <param name="tileObj">块状对象指针</param>
        /// <param name="arg">Actor对象</param>
        /// <returns>访问成功返回true，否则返回false</returns>
        public delegate bool Delegate_OnVisitTileObject(IntPtr tileObj, IntPtr arg);
        /// <summary>
        /// 定义块状场景访问时调用的委托事件
        /// </summary>
        static Delegate_TileVisibleVisitor_Visit tileVisibleVisitor_VisitEvent = TileVisibleVisitor_Visit;
        /// <summary>
        /// 定义访问块状对象时调用的委托对象
        /// </summary>
        static Delegate_OnVisitTileObject onVisitTileObjectEvent = OnVisitTileObject;

        private static void TileVisibleVisitor_Visit(IntPtr patch, CONTAIN_TYPE type, IntPtr arg)
        {
            unsafe
            {
                var argHandle = (System.Runtime.InteropServices.GCHandle)(arg);
                var visitorArg = (TileVisitorArg)(argHandle.Target);
                visitorArg.ContainType = type;
                DllImportAPI.vTilePatch_TourTileAllObjects(patch, onVisitTileObjectEvent, (int)(CSUtility.Component.EActorGameType.Unknow), visitorArg.SerialId, arg);
                visitorArg.Count++;
            }
        }

        private static bool OnVisitTileObject(IntPtr tileObj, IntPtr arg)
        {
            unsafe
            {
                TileVisitorArg visitorArg = (TileVisitorArg)(((System.Runtime.InteropServices.GCHandle)(arg)).Target);
                var csActor = DllImportAPI.vTileObject_GetCSActor(tileObj);
                if(csActor==IntPtr.Zero)
                    return true;
                var actor = ((System.Runtime.InteropServices.GCHandle)csActor).Target as CCore.World.Actor;
                if (actor == null)
                    return true;
                
                SlimDX.Matrix matrix = SlimDX.Matrix.Identity;
                if(false==actor.Placement.GetAbsMatrix(out matrix))
                    return true;

                // 检测Actor当前帧是否已经渲染过一次
                if( visitorArg.OwnerWorld != null && visitorArg.OwnerWorld.ActorOneFrameOneRender == true )
                {
                    if (actor.LastRenderFrame == Engine.Instance.CurRenderFrame)
                        return true;
                }
                actor.LastRenderFrame = Engine.Instance.CurRenderFrame;

                //if (actor.IsDynamic && visitorArg.ContainType == CONTAIN_TYPE.CONTAIN_TEST_REFER)
                if (visitorArg.ContainType == CONTAIN_TYPE.CONTAIN_TEST_REFER)
                {
                    var aInit = actor.ActorInit as CCore.World.ActorInit;
                    if (aInit!=null && aInit.VisibleCheck == true)
                    {
                        if (actor.VisibleCheckOBB)
                        //if (VisibleCheckOBB)
                        {
                            if (actor.Visual != null)
                            {
                                SlimDX.BoundingBox vOriBox = new SlimDX.BoundingBox();
                                SlimDX.Matrix fixMatrix = new SlimDX.Matrix();
                                actor.Visual.GetOBB(ref vOriBox.Minimum, ref vOriBox.Maximum, ref fixMatrix);
                                fixMatrix = fixMatrix * matrix;
                                if (visitorArg.Camera.IsFrustumContainOBB(ref vOriBox, ref fixMatrix) == false)
                                    return true;
                            }
                        }
                        else
                        {
                            SlimDX.Vector3 vMin = SlimDX.Vector3.Zero, vMax = SlimDX.Vector3.Zero;
                            SlimDX.BoundingBox vBox = new SlimDX.BoundingBox();
                            actor.GetAABB(ref vBox.Minimum, ref vBox.Maximum);
                            if (visitorArg.Camera.IsFrustumContainBox(ref vBox) == false)
                                return true;
                        }
                    }
                }

                #region 统计数量&编辑器
                Engine.Instance.RenderActorNumber++;
                switch((CSUtility.Component.EActorGameType)(actor.GameType))
                {
                    case CSUtility.Component.EActorGameType.Common:
                        Engine.Instance.RenderActor_Common_Number++;
                        break;
                    case CSUtility.Component.EActorGameType.Player:
                        Engine.Instance.RenderActor_Player_Number++;
                        break;
                    case CSUtility.Component.EActorGameType.Npc:
                        Engine.Instance.RenderActor_Npc_Number++;
                        break;
                    case CSUtility.Component.EActorGameType.Light:
                        Engine.Instance.RenderActor_Light_Number++;
                        break;
                    case CSUtility.Component.EActorGameType.Decal:
                        Engine.Instance.RenderActor_Decal_Number++;
                        break;
                    case CSUtility.Component.EActorGameType.NpcInitializer:
                        Engine.Instance.RenderActor_NpcInitializer_Number++;
                        break;
                    case CSUtility.Component.EActorGameType.Trigger:
                        Engine.Instance.RenderActor_Trigger_Number++;
                        break;
                    case CSUtility.Component.EActorGameType.Effect:
                        Engine.Instance.RenderActor_Effect_Number++;
                        break;
                    case CSUtility.Component.EActorGameType.EffectNpc:
                        Engine.Instance.RenderActor_EffectNpc_Number++;
                        break;
                }

                //if (actor.Visual != null)
                //{
                //    actor.Visual.EditorShow = CCore.Engine.Instance.Client.MainWorld.GetEditorObjShow(actor.GameType);
                //}

                // 应用RootMotion， 应该方到IMesh里面， 使用IMesh.mOwnerActor设置Placement。
                //var skinMesh = actor.Visual as IMesh;
                //if (skinMesh != null)
                //{
                //    if (skinMesh.GetAnimTree() != null)
                //    {
                //        var deltaPos = skinMesh.GetAnimTree().GetDeltaRootmotionPos();
                //        var deltaQuat = skinMesh.GetAnimTree().GetDeltaRootmotionQuat();
                //        if (deltaPos != SlimDX.Vector3.Zero)
                //        {
                //            var newPos = actor.Placement.GetLocation() + deltaPos;
                //            actor.Placement.SetLocation(ref newPos);
                //            var newQuat = SlimDX.Quaternion.Multiply(deltaQuat, actor.Placement.GetRotation());
                //            actor.Placement.SetRotation(ref newQuat);
                //        }
                //    }
                //}
                #endregion

                if (actor.Placement != null)
                {
                    if (actor.Visual != null)
                    {
                        actor.Visual.Commit(visitorArg.RenderEnv, ref matrix, visitorArg.Camera);
                    }
                    actor.OnCommitVisual(visitorArg.RenderEnv, ref matrix, visitorArg.Camera);
                }

                visitorArg.ObjCount++;
                return true;
            }
        }
        /// <summary>
        /// 绘制对象
        /// </summary>
        /// <param name="eye">视野</param>
        /// <param name="env">渲染环境</param>
        /// <param name="shadowLights">光源</param>
        public override void RenderVisible(CCore.Camera.CameraObject eye, CCore.Graphics.REnviroment env, CCore.Light.Light[] shadowLights)
        {
            if(mCullingCamera == null)
				return;
            
            TileVisitorArg visitor = new TileVisitorArg();
			visitor.SerialId = ++TourTileObjectSerialId;

			if(mLockCulling)
			{
                visitor.Camera = mCullingCamera;
				//visitor.SetCamera(mCullingCamera);
			}
			else
			{
                visitor.Camera = eye;
                DllImportAPI.v3dCamera_CopyData(mCullingCamera.CameraPtr, eye.CameraPtr);
			}

			visitor.Time = Engine.Instance.GetFrameMillisecond();
			visitor.RenderEnv = env;
			visitor.Count = 0;
			visitor.ObjCount = 0;
            visitor.OwnerWorld = mWorld;
            unsafe
            {
                IntPtr pinArg = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(visitor));

                if (mLockShadowCommit && shadowLights!=null)
                {
                    foreach(var shadowLight in shadowLights)
                    {
                        TileShadowVisitorArg visitorShadow = new TileShadowVisitorArg();
                        visitorShadow.SerialId = ++TourTileObjectSerialId;
                        visitorShadow.RenderEnv = env;

                        if (mLockCulling)
                            visitorShadow.Camera = mCullingCamera;
                        else
                            visitorShadow.Camera = eye;

                        visitorShadow.ShadowLight = shadowLight;

                        visitorShadow.Time = Engine.Instance.GetFrameMillisecond();
                        visitorShadow.Count = 0;

                        IntPtr pinArgShadow = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(visitorShadow));

                        DllImportAPI.vTileScene_CheckVisible(mTileScene, tileShadowCullSceneVisitor_VisitEvent, pinArgShadow, visitorShadow.Camera.CameraPtr, Engine.Instance.GetFrameMillisecond(), StopAtPatch?1:0, IgnoreChild?1:0);
                    }
                }
                else
                    DllImportAPI.vTileScene_CheckVisible(mTileScene, tileVisibleVisitor_VisitEvent, pinArg, visitor.Camera.CameraPtr, Engine.Instance.GetFrameMillisecond(), StopAtPatch?1:0, IgnoreChild?1:0);

                var argHandle = (System.Runtime.InteropServices.GCHandle)(pinArg);
                argHandle.Target = null;
                argHandle.Free();

            }
			visitor.RenderEnv = null;
			visitor.Camera = null;
        }


#endregion

#region RenderShadow
        /// <summary>
        /// 场景块中的阴影
        /// </summary>
        public class TileShadowVisitorArg
        {
            /// <summary>
            /// 数量
            /// </summary>
            public int Count;
            /// <summary>
            /// 时间
            /// </summary>
            public Int64 Time;
            /// <summary>
            /// 光源
            /// </summary>
            public CCore.Light.Light ShadowLight;
            /// <summary>
            /// 视野
            /// </summary>
            public CCore.Camera.CameraObject Camera;
            /// <summary>
            /// ID
            /// </summary>
            public uint SerialId;
            /// <summary>
            /// 渲染环境
            /// </summary>
            public CCore.Graphics.REnviroment RenderEnv;
        }

        static Delegate_TileVisibleVisitor_Visit tileShadowVisitor_VisitEvent = TileShadowVisitor_Visit;
        static Delegate_OnVisitTileObject onVisitTileObject_ShadowEvent = OnVisitTileObject_Shadow;
        private static void TileShadowVisitor_Visit(IntPtr patch, CONTAIN_TYPE type, IntPtr arg)
        {
            var argHandle = (System.Runtime.InteropServices.GCHandle)(arg);
            var visitorArg = (TileShadowVisitorArg)(argHandle.Target);
            DllImportAPI.vTilePatch_TourTileAllObjects(patch, onVisitTileObject_ShadowEvent, (int)(CSUtility.Component.EActorGameType.Unknow), visitorArg.SerialId, arg);
            visitorArg.Count++;
        }

        static Delegate_TileVisibleVisitor_Visit tileShadowCullSceneVisitor_VisitEvent = ShadowCullSceneVisitor_Visit;
        static Delegate_OnVisitTileObject onVisitTileObject_ShadowCullSceneEvent = OnVisitTileObject_ShadowCullScene;
        private static void ShadowCullSceneVisitor_Visit(IntPtr patch, CONTAIN_TYPE type, IntPtr arg)
        {
            unsafe
            {
                var argHandle = (System.Runtime.InteropServices.GCHandle)(arg);
                var visitorArg = (TileShadowVisitorArg)(argHandle.Target);
                DllImportAPI.vTilePatch_TourTileAllObjects(patch, onVisitTileObject_ShadowCullSceneEvent, (int)(CSUtility.Component.EActorGameType.Unknow), visitorArg.SerialId, arg);
                visitorArg.Count++;
            }
        }

        private static bool _CanCastShadow(CCore.World.Actor actor, TileShadowVisitorArg visitorArg)
        {
            if (actor == null)
                return false;

            if (actor.Placement == null)
                return false;

            if (actor.CastShadow == false)
                return false;

            // 如果MeshTemplate中指定不产生阴影，则强制返回。 
            var mesh = actor.Visual as CCore.Mesh.Mesh;
            if (mesh == null)
                return false;

            if(mesh.Visible != false || mesh.MainMeshVisible != false)
            {
                var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;
                if (meshInit != null)
                {
                    if (meshInit.CastShadow == false)
                        return false;
                }
            }

            SlimDX.Vector3 playerPos = SlimDX.Vector3.Zero;
            if (CCore.Engine.Instance.Client.ChiefRole != null)
                playerPos = CCore.Engine.Instance.Client.ChiefRole.Placement.GetLocation();
            float distance = 0;
            float topY = 0;

            SlimDX.Vector3 vEdge = SlimDX.Vector3.Zero;
            if (actor.VisibleCheckOBB)
            {
                SlimDX.BoundingBox vOriBox = new SlimDX.BoundingBox();
                SlimDX.Matrix fixMatrix = new SlimDX.Matrix();
                actor.Visual.GetOBB(ref vOriBox.Minimum, ref vOriBox.Maximum, ref fixMatrix);

                SlimDX.Vector3 obbPos, obbScale;
                SlimDX.Quaternion obbQuat;
                fixMatrix.Decompose(out obbScale, out obbQuat, out obbPos);
                distance = (playerPos - obbPos).Length();

                vEdge = vOriBox.Maximum - vOriBox.Minimum;
                topY = vOriBox.Maximum.Y - playerPos.Y;
            }
            else
            {
                distance = (playerPos - actor.Placement.GetLocation()).Length();

                SlimDX.Vector3 vMin = SlimDX.Vector3.Zero, vMax = SlimDX.Vector3.Zero;
                actor.GetAABB(ref vMin, ref vMax);
                vEdge = vMax - vMin;
                topY = vMax.Y - playerPos.Y;
            }
            vEdge.Y = 0;
            distance -= vEdge.Length() * 0.5f;

            //if (distance > visitorArg.ShadowLight.GameShadowCoverSize / 2.0f)
            //{
            //    if (visitorArg.isNight == true)
            //        return false;
            //    else
            //    {
            //        if (topY <= 0)      // 在玩家之下，则认为看不到他的影子
            //            return false;
            //        float fSunAngle = CCore.Engine.Instance.Client.MainWorld.SunAngle;
            //        if (fSunAngle > System.Math.PI / 2.0)
            //        {
            //            fSunAngle = (float)System.Math.PI - fSunAngle;
            //        }
            //        var tanSunRiseAngle = (float)System.Math.Tan(fSunAngle);
            //        //float tanSunRiseAngle = (float)System.Math.Tan(System.Math.PI / 6.0);
            //        var shadowMaxLength = topY / tanSunRiseAngle;
            //        if (distance > shadowMaxLength + visitorArg.ShadowLight.GameShadowCoverSize / 2.0f)
            //        {
            //            return false;
            //        }
            //    }
            //}

            return true;
        }

        private static bool OnVisitTileObject_ShadowCullScene(IntPtr tileObj, IntPtr arg)
        {
            unsafe
            {
                TileShadowVisitorArg visitorArg = (TileShadowVisitorArg)(((System.Runtime.InteropServices.GCHandle)(arg)).Target);
                var handPtr = DllImportAPI.vTileObject_GetCSActor(tileObj);
                if (handPtr == IntPtr.Zero)
                    return true;
                var handle = ((System.Runtime.InteropServices.GCHandle)(handPtr));
                if (handle == null)
                    return true;

                var actor = handle.Target as CCore.World.Actor;

                SlimDX.Matrix matrix = SlimDX.Matrix.Identity;
                if (false == actor.Placement.GetAbsMatrix(out matrix))
                    return true;

                if (_CanCastShadow(actor, visitorArg) == false)
                    return true;

                if (actor.Placement != null)
                {
                    if (actor.Visual != null)
                    {
                        actor.Visual.Commit(visitorArg.RenderEnv, ref matrix, visitorArg.Camera);
                    }
                    actor.OnCommitVisual(visitorArg.RenderEnv, ref matrix, visitorArg.Camera);
                }

                //SlimDX.Matrix matrix;
                //if (actor.Placement != null && actor.Placement.GetAbsMatrix(out matrix))
                //{
                //    if (actor.Visual != null)
                //        visitorArg.ShadowLight.AddVisual(actor.Visual.Layer, actor.Visual, ref matrix, actor.IsDynamic);
                //}

                return true;
            }
        }
        private static bool OnVisitTileObject_Shadow(IntPtr tileObj, IntPtr arg)
        {
            unsafe
            {
                TileShadowVisitorArg visitorArg = (TileShadowVisitorArg)(((System.Runtime.InteropServices.GCHandle)(arg)).Target);
                var handPtr = DllImportAPI.vTileObject_GetCSActor(tileObj);
                if (handPtr == IntPtr.Zero)
                    return true;
                var handle = ((System.Runtime.InteropServices.GCHandle)(handPtr));
                if(handle == null)
                    return true;

                var actor = handle.Target as CCore.World.Actor;

                if (_CanCastShadow(actor, visitorArg) == false)
                    return true;

                if (visitorArg.ShadowLight.IsCastedShadow(actor))
                    return true;

                SlimDX.Matrix matrix;
                if (actor.Placement != null && actor.Placement.GetAbsMatrix(out matrix))
                {
                    if (actor.Visual != null)
                        visitorArg.ShadowLight.AddVisual(actor.Visual.Layer, actor.Visual, ref matrix, true);
                }
                return true;
            }
        }
        /// <summary>
        /// 绘制阴影
        /// </summary>
        /// <param name="camera">摄像机</param>
        /// <param name="env">渲染环境</param>
        /// <param name="shadowLights">产生阴影的光源</param>
        public override void RenderShadow(CCore.Camera.CameraObject camera, CCore.Graphics.REnviroment env, CCore.Light.Light[] shadowLights)
        {
            if(shadowLights == null)
                return;

            foreach(var shadowLight in shadowLights)
            {
                if (shadowLight.ShadowType == CCore.Light.EShadowType.None)
                    continue;

                TileShadowVisitorArg visitor = new TileShadowVisitorArg();
                visitor.SerialId = ++TourTileObjectSerialId;
                visitor.RenderEnv = env;

                if(mLockCulling)
                    visitor.Camera = mCullingCamera;
                else
                    visitor.Camera = camera;

                visitor.ShadowLight = shadowLight;

                float CoverSize = 0;
                if(CCore.Engine.Instance.IsEditorMode)
                {
                    CoverSize = shadowLight.EditorShadowCoverSize;
                }
                else
                {
                    CoverSize = shadowLight.GameShadowCoverSize;
                }
                CoverSize *= 0.5F;

                var startV = CCore.Client.ChiefRoleInstance.Placement.GetLocation();//camera.Location;
                var endV = startV;//camera.Location;
                startV.X -= CoverSize;
                startV.Z -= CoverSize;
                endV.X += CoverSize;
                endV.Z += CoverSize;

                visitor.Time = Engine.Instance.GetFrameMillisecond();
                visitor.Count = 0;
                unsafe
                {
                    IntPtr pinArg = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(visitor));

                    shadowLight.ResetShadowActors();
                    DllImportAPI.vTileScene_CheckVisibleSquare(mTileScene, tileShadowVisitor_VisitEvent, pinArg, &startV, &endV, Engine.Instance.GetFrameMillisecond());

                    var argHandle = (System.Runtime.InteropServices.GCHandle)(pinArg);
                    argHandle.Target = null;
                    argHandle.Free();
                }
                visitor.ShadowLight = null;
                visitor.Camera = null;
            }
        }

#endregion

#region 导航渲染的各种回调处理
        /// <summary>
        /// 导航渲染的结构
        /// </summary>
        struct NavgationArg
        {
            /// <summary>
            /// 导航对象
            /// </summary>
            public CCore.Navigation.NavigationAssist Nav;
            /// <summary>
            /// X轴的长度
            /// </summary>
            public uint lvlX;
            /// <summary>
            /// Y轴的长度
            /// </summary>
            public uint lvlZ;
        }

        static Delegate_OnVisitTileObject onVisitTileObject_RenderNavigationEvent = OnVisitTileObject_RenderNavigation;

        private static bool OnVisitTileObject_RenderNavigation(IntPtr obj, IntPtr arg)
        {
            var visitorArg = (NavgationArg)(((System.Runtime.InteropServices.GCHandle)(arg)).Target);
            var actor = ((System.Runtime.InteropServices.GCHandle)(DllImportAPI.vTileObject_GetCSActor(obj))).Target as CCore.World.Actor;

            if (actor.Visual != null)
            {
                var mesh = actor.Visual as CCore.Mesh.Mesh;
                if (mesh == null)
                    return false;
                if (mesh.BlockNavigation)
                {
                    SlimDX.Matrix matrix;
                    if (actor.Placement != null && actor.Placement.GetAbsMatrix(out matrix))
                    {
                        visitorArg.Nav.CommitMesh(visitorArg.lvlX, visitorArg.lvlZ, mesh, ref matrix);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 渲染导航对象
        /// </summary>
        /// <param name="navLvlX">导航X轴的长度</param>
        /// <param name="navLvlZ">导航Z轴的长度</param>
        /// <param name="startX">导航起点X轴的值</param>
        /// <param name="startZ">导航起点Z轴的值</param>
        /// <param name="endX">导航终点X轴的值</param>
        /// <param name="endZ">导航终点Z轴的值</param>
        /// <param name="nav">导航对象</param>
        /// <returns>渲染成功返回true，否则返回false</returns>
        public override bool RenderNavigation(UInt32 navLvlX, UInt32 navLvlZ, float startX, float startZ, float endX, float endZ, CCore.Navigation.NavigationAssist nav)
        {
            unsafe
            {
                vTileSceneInfo info = new vTileSceneInfo();
                DllImportAPI.vTileScene_GetTileInfo(mTileScene, &info);

                UInt32 startLvlX = (UInt32)System.Math.Floor(startX / info.GetLevelX());
                UInt32 startLvlZ = (UInt32)System.Math.Floor(startZ / info.GetLevelZ());
                UInt32 endLvlX = (UInt32)System.Math.Ceiling(endX / info.GetLevelX());
                UInt32 endLvlZ = (UInt32)System.Math.Ceiling(endZ / info.GetLevelZ());

                for(UInt32 lvlX = startLvlX; lvlX < endLvlX; lvlX++)
                {
                    for(UInt32 lvlZ = startLvlZ; lvlZ < endLvlZ; lvlZ++)
                    {
                        var pLevel = DllImportAPI.vTileScene_GetRealLevel(mTileScene, (UInt16)lvlX, (UInt16)lvlZ, Engine.Instance.GetFrameMillisecond());

                        for (uint patchX = 0; patchX < info.PatchPerLevelX; patchX++)
                        {
                            for (uint patchZ = 0; patchZ < info.PatchPerLevelZ; patchZ++)
                            {
                                var patch = DllImportAPI.vTileLevel_GetPatch(pLevel, patchX, patchZ);
                                if (DllImportAPI.vTilePatch_IsDeleted(patch) == 0)
                                {
                                    NavgationArg navArg = new NavgationArg();
                                    navArg.Nav = nav;
                                    navArg.lvlX = navLvlX;
                                    navArg.lvlZ = navLvlZ;
                                    IntPtr pinArg = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(navArg));

                                    DllImportAPI.vTilePatch_TourTileAllObjects(patch, onVisitTileObject_RenderNavigationEvent, (int)(CSUtility.Component.EActorGameType.Common), ++TourTileObjectSerialId, pinArg);
                                    DllImportAPI.vTilePatch_TourTileAllObjects(patch, onVisitTileObject_RenderNavigationEvent, (int)(CSUtility.Component.EActorGameType.DynamicBlock), ++TourTileObjectSerialId, pinArg);
                            
                                    var argHandle = (System.Runtime.InteropServices.GCHandle)(pinArg);
                                    argHandle.Target = null;
                                    argHandle.Free();

                                }
                            }
                        }
                    }
                }

                return true;
            }
        }

#endregion
#region 服务器高度图的各种回调处理
        /// <summary>
        /// 服务器高度图
        /// </summary>
        struct ServerHeightMapArg
        {
            /// <summary>
            /// 服务器高度图的帮助对象
            /// </summary>
            public CCore.Support.ServerAltitudeAssist Shm;
            /// <summary>
            /// X轴的长度
            /// </summary>
            public uint lvlX;
            /// <summary>
            /// Z轴的高度
            /// </summary>
            public uint lvlZ;
        }

        static Delegate_OnVisitTileObject onVisitTileObject_RenderServerHeightMapEvent = OnVisitTileObject_RenderServerHeightMap;

        private static bool OnVisitTileObject_RenderServerHeightMap(IntPtr obj, IntPtr arg)
        {
            var visitorArg = (ServerHeightMapArg)(((System.Runtime.InteropServices.GCHandle)(arg)).Target);
            var actor = ((System.Runtime.InteropServices.GCHandle)(DllImportAPI.vTileObject_GetCSActor(obj))).Target as CCore.World.Actor;

            if (actor.Visual != null)
            {
                var mesh = actor.Visual as CCore.Mesh.Mesh;
                SlimDX.Matrix matrix;
                if (actor.Placement != null && actor.Placement.GetAbsMatrix(out matrix))
                {
                    visitorArg.Shm.CommitMesh(visitorArg.lvlX, visitorArg.lvlZ, mesh, ref matrix);
                }
            }
            return true;
        }
        /// <summary>
        /// 渲染服务器高度图
        /// </summary>
        /// <param name="lvlX">X轴的长度</param>
        /// <param name="lvlZ">Z轴的长度</param>
        /// <param name="shm">服务器高度图帮助对象</param>
        /// <returns>绘制成功返回true，否则返回false</returns>
        public override bool RenderServerHeightMap(uint lvlX, uint lvlZ, CCore.Support.ServerAltitudeAssist shm)
        {
            unsafe
            {
                var level = DllImportAPI.vTileScene_GetRealLevel(mTileScene, (UInt16)lvlX, (UInt16)lvlZ, Engine.Instance.GetFrameMillisecond());
                if (level == IntPtr.Zero)
                    return false;

                vTileSceneInfo info = new vTileSceneInfo();
                DllImportAPI.vTileScene_GetTileInfo(mTileScene, &info);

                for (uint patchX = 0; patchX < info.PatchPerLevelX; patchX++)
                {
                    for (uint patchZ = 0; patchZ < info.PatchPerLevelZ; patchZ++)
                    {
                        var patch = DllImportAPI.vTileLevel_GetPatch(level, patchX, patchZ);
                        if (DllImportAPI.vTilePatch_IsDeleted(patch) == 0)
                        {
                            ServerHeightMapArg shmArg = new ServerHeightMapArg();
                            shmArg.Shm = shm;
                            shmArg.lvlX = lvlX;
                            shmArg.lvlZ = lvlZ;
                            IntPtr pinArg = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(shmArg));

                            DllImportAPI.vTilePatch_TourTileAllObjects(patch, onVisitTileObject_RenderServerHeightMapEvent, (int)(CSUtility.Component.EActorGameType.Common), ++TourTileObjectSerialId, pinArg);
                        
                            var argHandle = (System.Runtime.InteropServices.GCHandle)(pinArg);
                            argHandle.Target = null;
                            argHandle.Free();
                        }
                    }
                }

                return true;
            }
        }

#endregion

        /// <summary>
        /// 向场景块中添加地形片
        /// </summary>
        /// <param name="x">坐标X值</param>
        /// <param name="z">坐标Z值</param>
        public void AddPatch(float x, float z)
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return;

                DllImportAPI.vTileScene_AddPatch(mTileScene, x, z, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 删除相应位置的场景片
        /// </summary>
        /// <param name="x">X位置坐标</param>
        /// <param name="z">Z位置坐标</param>
        public void DelPatch(float x, float z)
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return;

                DllImportAPI.vTileScene_DelPatch(mTileScene, x, z, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 增加场景块高度
        /// </summary>
        /// <param name="idu">u向高度</param>
        /// <param name="idv">v向高度</param>
        public override void AddLevel(uint idu, uint idv)
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return;

                DllImportAPI.vTileScene_AddLevel(mTileScene, idu, idv, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 删除对应高度
        /// </summary>
        /// <param name="idu">u向高度</param>
        /// <param name="idv">v向高度</param>
        public override void DelLevel(uint idu, uint idv)
        {
            unsafe
            {
                if (mTileScene == IntPtr.Zero)
                    return;

                DllImportAPI.vTileScene_DelLevel(mTileScene, idu, idv, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 获取场景高度数据
        /// </summary>
        /// <returns>返回每个场景是否获取到对应高度，得到返回true，否则返回false</returns>
        public List<bool> GetLevelData()
        {
            unsafe
            {
                var retData = new List<bool>();

                vTileSceneInfo info = new vTileSceneInfo();
                DllImportAPI.vTileScene_GetTileInfo(mTileScene, &info);

                for (uint z = 0; z < info.LevelZCount; z++)
                {
                    for (uint x = 0; x < info.LevelXCount; x++)
                    {
                        var level = DllImportAPI.vTileScene_GetLevel(mTileScene, (UInt16)x, (UInt16)z);
                        if (level == IntPtr.Zero)
                            retData.Add(false);
                        else
                            retData.Add(true);
                    }
                }

                return retData;
            }
        }
        /// <summary>
        /// 渲染场景块中的包围盒
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="z">Z轴坐标</param>
        public void RenderTileInheritBoundingBox(float x, float z)
        {
            unsafe
            {
                var patch = DllImportAPI.vTileScene_GetPatch(mTileScene, x, z, Engine.Instance.GetFrameMillisecond(), true);
                if (patch != IntPtr.Zero)
                {
                    DllImportAPI.vTilePatch_DrawInheritBoundingBox(patch, Engine.Instance.Client.Graphics.Device);
                }
            }
        }
        /// <summary>
        /// 设置边缘间隔
        /// </summary>
        /// <param name="value">间隔距离</param>
        public override void SetNeighborSide(uint value)
        {
            unsafe
            {
                DllImportAPI.vTileScene_SetNeighborSide(mTileScene, value, Engine.Instance.GetFrameMillisecond());
            }
        }
    }
}
