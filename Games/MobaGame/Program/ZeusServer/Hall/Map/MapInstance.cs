using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall.Map
{
    public class MapInstance
    {
        public MapTemplate MapTemplate { get; set; } = null;
        public LogicProcessor LogicProcessor { get; set; }
        public bool WaitDestory { get; set; } = false;
        public UInt16 IndexInManager { get; set; } = UInt16.MaxValue;
        public RoleIdManager RoleManager{get;} = new RoleIdManager();        
        public RPC.RPCSpecialHolderProcessor RpcProcessor { get; } = new RPC.RPCSpecialHolderProcessor();        

        public virtual bool IsNullMap { get; } = false;

        public HallInstance Halls { get; set; } = null;

        public uint CellSize = 4;

        public Guid MapSourceId
        {
            get { return MapTemplate.MapGuid; }
        }

        #region 初始化处理
        public bool InitMap(HallInstance halls, UInt16 index, Guid mapSourceId)
        {
            Halls = halls;
            IndexInManager = index;

            MapTemplate = MapTemplateManager.Instance.FindMapTemplate(mapSourceId);
            if (MapTemplate == null)
                return false;
            InitPlayerPool(10);

            CellSize = (uint)MapTemplate.CellSize;
            var size = GameData.Support.ConfigFile.Instance.MapSize;
            // 从地图数据中创建实例数据
            mXCount = (uint)(size / CellSize) +(uint) ((size % CellSize) > 0 ? 1 : 0);
            mZCount = (uint)(size / CellSize) +(uint) ((size % CellSize) > 0 ? 1 : 0);

            mCells = new MapCell[mZCount, mXCount];
            for (int i = 0; i < mZCount; i++)
            {
                for (int j = 0; j < mXCount; j++)
                {
                    MapCell mapCell = new MapCell(j, i);
                    mCells[i, j] = mapCell;
                }
            }
            GetMapData(halls, MapTemplate);
            return true;
        }

        bool GetMapData(HallInstance hall, MapTemplate mapTemplate)
        {
            if (mapTemplate == null)
                return false;

            foreach(var triggerData in mapTemplate.Triggers)
            {
                try
                {
                    if (triggerData == null)
                        continue;

                    var newtriggerData = (CSUtility.Map.Trigger.TriggerData)triggerData.CloneObject();
                    Role.Trigger.TriggerInstance.CreateTriggerInstance(newtriggerData, hall, this);
                }
                catch (Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }

            foreach(var monsterData in mapTemplate.Monsters)
            {
                try
                {
                    if(monsterData == null)
                        continue;
                    var newMOnsterData = (GameData.Role.MonsterData)monsterData.CloneObject();
                    Role.Monster.MonsterInstance.CreateMonsterInstance(newMOnsterData,hall,this);
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }

            #region 测试代码
//             var monsters = new List<UInt16>() { 5,6};
//             var rand = new Random();
//             var index = rand.Next(monsters.Count);
//             var tempid = monsters[index];
//             var data = new GameData.Role.MonsterData();
//             data.TemplateId = tempid;
//             data.RoleId = Guid.NewGuid();
//             var faction = CSUtility.Data.DataTemplateManager<UInt16, GameData.Faction>.Instance.GetDataTemplate((UInt16)data.RoleTemplate.FactionId);//GameData.FactionManager.Instance.FindFaction((UInt16)data.RoleTemplate.FactionId);
//             if (faction != null)
//             {
//                 data.OriPosition = data.Position = faction.StarPoint;
//             }            
//             var role = Role.Monster.MonsterInstance.CreateMonsterInstance(data, this.Halls, this);
            #endregion
            return true;
        }

        #endregion
        long mCurFrameTickCount = 0;
        public long CurFrameTickCount
        {
            get { return mCurFrameTickCount; }
        }
        long mFpsCounterBegin = IServer.Instance.GetElapseMilliSecondTime();
        int mFpsCounter = 0;
        float mFps = 0;
        public float Fps
        {
            get { return mFps; }
        }
        
        public virtual void Tick(Int64 elapseMillsecond)
        {
            mCurFrameTickCount = IServer.Instance.GetElapseMilliSecondTime();
            mFpsCounter++;
            if (mFpsCounter >= 25)
            {
                mFps = ((float)mFpsCounter) * 1000.0F / (float)(mCurFrameTickCount - mFpsCounterBegin);
                mFpsCounterBegin = mCurFrameTickCount;
                mFpsCounter = 0;
            }

            mPlayerDictionary.BeforeTick();
            
            RpcProcessor.Tick();
            RoleManager.Tick(elapseMillsecond);

            mPlayerDictionary.AfterTick();

          //  UpdateCreateMonster(elapseMillsecond);
        }

        long mUpdateCreateMonsTime = 30000;
        public void UpdateCreateMonster(long elapseMillsecond)
        {
            //临时代码
            mUpdateCreateMonsTime -= elapseMillsecond;
            if (mUpdateCreateMonsTime >= 0)
                return;
            mUpdateCreateMonsTime = 20000;
            //Role.Monster.MonsterInstance.AutoCreateMonster(2, 26, 63, this.Halls, this);
            Role.Monster.MonsterInstance.AutoCreateMonster(4, 26, 63, this.Halls, this);
        }

        public bool IsBlocked(float x, float z)
        {
            if (MapTemplate == null)
                return true;
            return false;            
        }

        public float GetAltitude(float x, float z)
        {
            return 0;
        }
        public MapCell GetMapCell(float x, float z)
        {
            var ix = (UInt32)(x / MapTemplate.CellSize);
            var iz = (UInt32)(z / MapTemplate.CellSize);
            if (ix >= mXCount || iz >= mZCount)
                return null;
            return mCells[iz, ix];
        }
        public List<MapCell> GetMapCell(float startx, float startz, float endx, float endz)
        {
            List<MapCell> retMapCells = new List<MapCell>();

            int cellStartX = (int)(startx / MapTemplate.CellSize);
            int cellStartZ = (int)(startz / MapTemplate.CellSize);

            if (cellStartX < 0)
                cellStartX = 0;
            if (cellStartX >= mXCount)
                cellStartX = (int)mXCount - 1;

            if (cellStartZ < 0)
                cellStartZ = 0;
            if (cellStartZ >= mZCount)
                cellStartZ = (int)mZCount - 1;

            int cellEndX = (int)(endx / MapTemplate.CellSize);
            int cellEndZ = (int)(endz / MapTemplate.CellSize);

            if (cellEndX < 0)
                cellEndX = 0;
            if (cellEndX >= mXCount)
                cellEndX = (int)mXCount- 1;

            if (cellEndZ < 0)
                cellEndZ = 0;
            if (cellEndZ >= mZCount)
                cellEndZ = (int)mZCount - 1;

            for (int x = cellStartX; x < cellEndX + 1; ++x)
            {
                for (int z = cellStartZ; z < cellEndZ + 1; ++z)
                {
                    retMapCells.Add(mCells[z, x]);
                }
            }

            return retMapCells;
        }

        #region 地图格子系统
        MapCell[,] mCells = null;
        UInt32 mXCount = 0;
        UInt32 mZCount = 0;
        public delegate bool FOnVisitCell(MapCell cell, UInt32 x, UInt32 z);
        public bool TourCell(UInt32 sx, UInt32 sz, UInt32 ex, UInt32 ez, FOnVisitCell visitor)
        {
            if (sx >= mXCount || sz >= mZCount)
                return false;
            if(ex>mXCount)
                ex = mXCount;
            if (ez > mZCount)
                ez = mZCount;
            for (var x = sx;x < ex;x++)
            {
                for (var z = sz; z < ez; z++)
                {
                    if (mCells[z,x] == null)
                        continue;
                    if (false == visitor(mCells[z, x], x, z))
                        return false;
                }
            }
            return true;
        }
        public bool TourRole(UInt32 types, UInt32 sx, UInt32 sz, UInt32 ex, UInt32 ez, CSUtility.Support.ConcurentObjManager<UInt32, Hall.Role.RoleActor>.FOnVisitObject visitor, object arg)
        {
            if (sx >= mXCount || sz >= mZCount)
                return false;
            if (ex > mXCount)
                ex = mXCount;
            if (ez > mZCount)
                ez = mZCount;
            for (var x = sx; x < ex; x++)
            {
                for (var z = sz; z < ez; z++)
                {
                    if (mCells[z, x] == null)
                        continue;
                    if (false == mCells[z, x].ForEachRole(types, visitor, arg))
                        return false;
                }
            }
            return true;
        }

        public bool TourRoles(UInt32 types, ref SlimDX.Vector3 loc, UInt32 radius, CSUtility.Support.ConcurentObjManager<UInt32, Hall.Role.RoleActor>.FOnVisitObject visitor, object arg)
        {
            UInt32 startX = (UInt32)((loc.X - radius) / CellSize);
            UInt32 endX = (UInt32)((loc.X + radius) / CellSize)+1;
            UInt32 startZ = (UInt32)((loc.Z - radius)/ CellSize);
            UInt32 endZ = (UInt32)((loc.Z + radius)/ CellSize)+1;

            TourRole(types,startX,startZ,endX,endZ,visitor, arg);
            return true;
        }

        #endregion

        #region 玩家下标管理
        System.UInt16 mMaxPlayerCount;
        Role.Player.PlayerInstance[] mPlayerPool;
        public Role.Player.PlayerInstance[] PlayerPool
        {
            get { return mPlayerPool; }
        }
        Stack<System.UInt16> mFreeSlot = new Stack<System.UInt16>();
        CSUtility.Support.AsyncObjManager<Guid, Role.Player.PlayerInstance> mPlayerDictionary = new CSUtility.Support.AsyncObjManager<Guid, Role.Player.PlayerInstance>();
        public CSUtility.Support.AsyncObjManager<Guid, Role.Player.PlayerInstance> PlayerDictionary
        {
            get { return mPlayerDictionary; }
        }
        private void InitPlayerPool(System.UInt16 maxPly)
        {
            mMaxPlayerCount = maxPly;
            mPlayerPool = new Role.Player.PlayerInstance[maxPly];
            mFreeSlot.Clear();
            for (UInt16 i = maxPly; i > 0; i--)
            {
                mFreeSlot.Push((UInt16)(i - 1));
            }
        }        
        private bool EnterPlayerPool(Role.Player.PlayerInstance player)
        {
            if (IsFull)
                return false;
            player._SetIndexInMap(mFreeSlot.Pop());
            return true;
        }
        private void LeavePlayerPool(Role.Player.PlayerInstance player)
        {
            mFreeSlot.Push(player.IndexInMap);
            player._SetIndexInMap(System.UInt16.MaxValue);
        }
        public bool IsFull
        {
            get { return mFreeSlot.Count == 0; }
        }
        public Role.Player.PlayerInstance GetPlayerByIndex(UInt16 index)
        {
            return mPlayerPool[index];
        }
        public Role.Player.PlayerInstance FindPlayer(Guid id)
        {
            return mPlayerDictionary.FindObj(id);
        }
        public Role.Player.PlayerInstance FindPlayer(uint singleId)
        {
            return RoleManager.PlayerManager.FindObj(singleId);
        }
        #endregion

        #region Summon
        public Role.Summon.SummonRole GetSummonRole(UInt32 singleId)
        {
            return RoleManager.SummonManager.FindObj(singleId);
        }
        public void AddSummon(Role.Summon.SummonRole role)
        {
            RoleManager.SummonManager.MapRole(role);
        }
        public void RemoveSummon(Role.Summon.SummonRole role)
        {
            if (role != null)
                role.DoLeaveMap();
        }
        private void ProcRemoveSummon(Role.Summon.SummonRole role)
        {
            RoleManager.SummonManager.UnmapRole(role);
            RemoveRoleActor(role);
        }
        #endregion

        #region Monster

        [CSUtility.Event.Attribute.AllowMember("地图.根据ID查找怪物", CSUtility.Helper.enCSType.Server, "查找怪物")]
        [CSUtility.AISystem.Attribute.AllowMember("地图.根据ID查找怪物", CSUtility.Helper.enCSType.Server, "查找怪物")]
        public Role.Monster.MonsterInstance GetMonsterRole(Guid roleId)
        {
            Role.Monster.MonsterInstance monsterInstance = null;
            RoleManager.MonsterManager.Objects.For_Each((UInt32 id, Role.Monster.MonsterInstance monster,object arg) =>
            {
                if (monster.Id == roleId)
                {
                    monsterInstance = monster;
                    return CSUtility.Support.EForEachResult.FER_Stop;
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            return monsterInstance;   
        }

        [CSUtility.Event.Attribute.AllowMember("地图.根据名字查找怪物", CSUtility.Helper.enCSType.Server, "查找怪物")]
        [CSUtility.AISystem.Attribute.AllowMember("地图.根据名字查找怪物", CSUtility.Helper.enCSType.Server, "地图.根据名字查找怪物")]
        public Role.Monster.MonsterInstance GetMonsterByName(string name)
        {
            Role.Monster.MonsterInstance monsterInstance = null;
            RoleManager.MonsterManager.Objects.For_Each((UInt32 id, Role.Monster.MonsterInstance monster, object arg) =>
            {
                if (monster.RoleName == name && monster.CurrentState.StateName != "Death")
                {
                    monsterInstance = monster;
                    return CSUtility.Support.EForEachResult.FER_Stop;
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            return monsterInstance;
        }

        public Role.Monster.MonsterInstance GetMonsterRole(UInt32 singleId)
        {
            return RoleManager.MonsterManager.FindObj(singleId);
        }

        public void AddMonster(Role.Monster.MonsterInstance role)
        {
            RoleManager.MonsterManager.MapRole(role);
        }

        public void RemoveMonster(Role.Monster.MonsterInstance role)
        {
            if (role != null)
                role.DoLeaveMap();
        }

        private void ProcRemoveMonster(Role.Monster.MonsterInstance role)
        {
            RoleManager.MonsterManager.UnmapRole(role);
            RemoveRoleActor(role);
        }

        #endregion

        #region Trigger Manager //////////////////////////////////////////////////////////

        // 用于记录在当前地图中的Trigger        
        CSUtility.Support.AsyncObjManager<Guid, Role.Trigger.TriggerInstance> mTriggerDictionary = new CSUtility.Support.AsyncObjManager<Guid, Role.Trigger.TriggerInstance>();

        public Role.Trigger.TriggerInstance GetTrigger(UInt32 singleId)
        {
            return RoleManager.TriggerManager.FindObj(singleId);
        }

        public Role.Trigger.TriggerInstance GetTrigger(Guid id)
        {
            return mTriggerDictionary.FindObj(id);
        }

        public void AddTrigger(Role.Trigger.TriggerInstance trigger)
        {            
            mTriggerDictionary.Add(trigger.Id, trigger);
            RoleManager.TriggerManager.MapRole(trigger);
        }

        public void RemoveTrigger(Role.Trigger.TriggerInstance trigger)
        {
            mTriggerDictionary.Remove(trigger.Id);
            RoleManager.TriggerManager.UnmapRole(trigger);
        }

        #endregion

        #region DynamicBlock Manager//////////////////////////////////////////////////////////////////////////

        // 用于记录在当前地图的动态碰撞体
        CSUtility.Support.AsyncObjManager<Guid, Server.Hall.Role.DynamicBlockInstance> mDynamicBlockDictionary = new CSUtility.Support.AsyncObjManager<Guid, Server.Hall.Role.DynamicBlockInstance>();

        public Server.Hall.Role.DynamicBlockInstance GetDynamicBlock(UInt32 singleId)
        {
            return RoleManager.DynamicBlockManager.FindObj(singleId);
        }

        public Server.Hall.Role.DynamicBlockInstance GetDynamicBlock(Guid id)
        {
            return mDynamicBlockDictionary.FindObj(id);
        }
        
        public void AddDynamicBlock(Server.Hall.Role.DynamicBlockInstance dyIns)
        {
            mDynamicBlockDictionary.Add(dyIns.Id, dyIns);
        }

        public void RemoveDynamicBlock(Server.Hall.Role.DynamicBlockInstance dyIns)
        {
            if (dyIns != null)
                dyIns.DoLeaveMap();
        }
        private void ProcRemoveDynamicBlock(Server.Hall.Role.DynamicBlockInstance dyIns)
        {
            if (dyIns == null)
                return;

            RoleManager.DynamicBlockManager.UnmapRole(dyIns);
            mDynamicBlockDictionary.Remove(dyIns.Id);
            RemoveRoleActor(dyIns);
        }

        #endregion


        public static MapInstance CreateMapInstance(HallInstance halls, UInt16 index, Guid mapSourceId)
        {
            //var worldInit = Map.MapInstanceManager.GetMapInitBySourceId(mapSourceId);
         //   if (worldInit == null)
        //        return null;
            var map = new MapInstance();
            map.InitMap(halls, index, mapSourceId);            
            return map;
        }

        public void ReleaseInstanceMap()
        {
            WaitDestory = true;
        }

        public virtual void OnRoleDeath(Role.RoleActor role)
        {

        }
        public virtual bool OnPlayerEnterMap(Role.Player.PlayerInstance player, SlimDX.Vector3 pos)
        {
            lock (this)
            {
                if (mPlayerPool == null)
                    return false;

                if (mFreeSlot.Count == 0)
                    return false;
                if (player.HostMap != this)
                {
                    if (player.HostMap !=null)
                    {
                        player.HostMap.OnPlayerLeaveMap(player);
                    }
                    player._SetIndexInMap(mFreeSlot.Pop());
                    System.Diagnostics.Debug.Assert(mPlayerPool[player.IndexInMap] == null);
                    mPlayerPool[player.IndexInMap] = player;
                    mPlayerDictionary.Add(player.Id, player);

                    RoleManager.PlayerManager.MapRole(player);
                    player.OnEnterMap(this);
                }
                player.Placement.SetLocation(ref pos);
                return true;
            }
        }

        public virtual void OnPlayerLeaveMap(Role.Player.PlayerInstance player)
        {
            lock (this)
            {
                if (mPlayerPool == null)
                    return;
                if (player.IndexInMap >= mPlayerPool.Length)
                {
                    System.Diagnostics.Debugger.Break();
                    for (UInt16 i = 0; i < mPlayerPool.Length; i++)
                    {
                        if (mPlayerPool[i] == player)
                        {
                            player._SetIndexInMap(i);
                            break;
                        }
                    }
                }
                else if (mPlayerPool[player.IndexInMap] != player)
                {
                    return;
                }

                mPlayerDictionary.Remove(player.Id);
                mPlayerPool[player.IndexInMap] = null;
                mFreeSlot.Push(player.IndexInMap);
                player._SetIndexInMap(UInt16.MaxValue);

                player.DangrouseOnLeaveMap();

                RoleManager.PlayerManager.UnmapRole(player);
                return;
            }
        }
        public virtual void OnMapClosed()
        {
            //以后处理
        }

        public Role.RoleActor GetRole(UInt32 singleId)
        {
            Role.RoleActor role = FindPlayer(singleId);
            if (role != null)
                return role;

            role = GetMonsterRole(singleId);
            if (role != null)
                return role;
            // 
            //             role = GetSummonRole(singleId);
            //             if (role != null)
            //                 return role;
            // 
            //             role = GetDropedItemRole(singleId);
            //             if (role != null)
            //                 return role;
            // 
            //             role = GetGatherItem(singleId);
            //             if (role != null)
            //                 return role;
            //             role = GetTempNPC(singleId);
            //             if (role != null)
            //                 return role;

            return null;
        }

        public bool RemoveRoleActor(Role.RoleActor role)
        {
            if (role.HostMap != this)
                return false;
            if (role.Cell != null)
                role.Cell.Leave(role);
            return true;
        }

        public void RolePositionChanged(Role.RoleActor role, ref SlimDX.Vector3 loc)
        {
            if (this.IsNullMap)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Role {0} call RolePositionChanged in nullmap", role.RoleName));
                return;
            }
            if (role == null || role.RoleTemplate == null)
                return;
            if (/*role.RoleTemplate.RPC2ClientIgnoreDis*/true)//不控制距离都发了（慎用啊！！）
            {
                RPC.PackageWriter pkg = new RPC.PackageWriter();
                Client.H_GameRPC.smInstance.HIndex(pkg, role.SingleId).RPC_UpdatePosition(pkg, (Int16)(loc.X * 10.0F), (Int16)(loc.Z * 10.0F));
                CSUtility.Support.AsyncObjManager<Guid, Role.Player.PlayerInstance>.FOnVisitObject visitor = delegate (Guid key, Role.Player.PlayerInstance player, object arg)
                {
                    pkg.DoCommandPlanes2Client(player.Planes2GateConnect, player.ClientLinkId);
                    return CSUtility.Support.EForEachResult.FER_Continue;
                };
                mPlayerDictionary.For_Each(visitor, null);
            }
            else
            {
//                 var firstPlayer = this.FindFirstPlayer(loc, RPC.RPCNetworkMgr.Sync2ClientRange * 0.8F);
//                 if (firstPlayer != null)
//                 {
//                     RPC.PackageWriter pkg = new RPC.PackageWriter();
//                   
//                     //ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, role.SingleId).RPC_UpdatePosition(pkg, (Int16)(loc.X * 10.0F), (Int16)(loc.Z * 10.0F));
// 
//                     this.SendPkg2Clients(role, loc, pkg);
//                 }
            }
        }

        public void RoleDirectionChanged(Role.RoleActor role, float angle)
        {
            if (this.IsNullMap)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Role {0} call RoleDirectionChanged in nullmap", role.RoleName));
                return;
            }

            var loc = role.Placement.GetLocation();
//             var firstPlayer = this.FindFirstPlayer(loc, RPC.RPCNetworkMgr.Sync2ClientRange * 0.8F);
//             if (firstPlayer != null)
//             {
//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
// 
//                 ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, role.SingleId).RPC_UpdateDirection(pkg, angle);
// 
//                 this.SendPkg2Clients(role, loc, pkg);
//             }
        }

  
        public void BroadcastRoleLeave(Role.RoleActor role)
        {
            if (this.IsNullMap)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Role {0} call BroadcastRoleLeave in nullmap", role.RoleName));
                return;
            }

            RPC.PackageWriter pkg = new RPC.PackageWriter();
            Client.H_GameRPC.smInstance.RPC_RoleLeave(pkg, role.SingleId);
            this.SendPkg2Clients(role, role.Placement.GetLocation(), pkg);
        }

        #region RPC
        Random mRand = new Random((int)CSUtility.DllImportAPI.vfxGetTickCount());
        public void SendPkg2Clients(Role.RoleActor ignoreRole, SlimDX.Vector3 pos, RPC.PackageWriter pkg, int maxCount = 50)
        {
            bool chooseSend = false;
            int remainNumber = maxCount;
            int limit = maxCount * 2 / 10;
            CSUtility.Support.ConcurentObjManager<UInt32, Hall.Role.RoleActor>.FOnVisitObject visitor = delegate(UInt32 i,Role.RoleActor role, object arg)
            {
                var player = role as Role.Player.PlayerInstance;
                if (player == null)
                    return CSUtility.Support.EForEachResult.FER_Stop;

                if (ignoreRole == role)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                if (remainNumber < limit)
                {//玩家太多，接近了发送上限，开始随机选择发送目标 
                    chooseSend = true;
                    var choose = mRand.Next(maxCount);
                    if (choose > limit)
                        return CSUtility.Support.EForEachResult.FER_Continue;
                }

                remainNumber--;
                pkg.DoCommandPlanes2Client(player.Planes2GateConnect, player.ClientLinkId);
                if (remainNumber <= 0)
                    return CSUtility.Support.EForEachResult.FER_Stop;

                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            //这个range是以pos为中心广播多大范围内的玩家
            TourRoles((UInt32)GameData.Role.ERoleType.Player,ref pos, (UInt32)(RPC.RPCNetworkMgr.Sync2ClientRange * 0.8F), visitor, ignoreRole);

            if (chooseSend && remainNumber > 0)
            {
                System.Diagnostics.Debug.WriteLine("玩家太多，随机选择后，少同步了{0}/{1}", remainNumber, maxCount);
            }
        }
        #endregion

        #region MyRegion        

        [CSUtility.Event.Attribute.AllowMember("地图.创建怪物", CSUtility.Helper.enCSType.Server, "创建怪物")]
        public Role.Monster.MonsterInstance CreateMonster(UInt16 templateId,SlimDX.Vector3 pos)
        {
            var data = new GameData.Role.MonsterData();
            data.TemplateId = templateId;
            data.RoleId = Guid.NewGuid();
            data.Position = pos;
            data.OriPosition = pos;
            return Role.Monster.MonsterInstance.CreateMonsterInstance(data, this.Halls, this);            
        }

        [CSUtility.Event.Attribute.AllowMember("地图.创建怪物", CSUtility.Helper.enCSType.Server, "创建怪物")]
        public Role.Monster.MonsterInstance CreateMonsterByCreateTemplate(UInt16 templateId, SlimDX.Vector3 pos,int listIndex)
        {
            var monsterInfo = CSUtility.Data.DataTemplateManager<UInt16, GameData.Data.MonsterCreateTemplate>.Instance.GetDataTemplate(templateId);// GameData.Data.MonsterCreateManager.Instance.GetTemplate(templateId);
            if (monsterInfo == null)
                return null;
            if (monsterInfo.MonsterList.Count <= listIndex)
                return null;

            return CreateMonster(monsterInfo.MonsterList[listIndex].RoleTemplateId, pos);
        }
        #endregion

    }

    public class NullMapInstance : MapInstance
    {
        static NullMapInstance smInstance = new NullMapInstance();
        public static NullMapInstance Instance
        {
            get { return smInstance; }
        }
        public override bool IsNullMap
        {
            get { return true; }
        }
    }
}
