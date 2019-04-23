using System;
using System.Collections.Generic;
using System.Text;
using CCore.MsgProc;
using GameData.Role;

namespace Client.Stage
{
    public struct RecordV
    {
        public string name;
        public string value;
        public int index;
        public UISystem.WinForm form;
    }

    class MainStage : IStage
    {
        ~MainStage()
        {
        }

        public static MainStage Instance
        {
            get
            {
                return CCore.Support.ReflectionManager.Instance.GetClassObject<MainStage>();
            }
        }

        Role.RoleActor mChiefRole;
        public Role.RoleActor ChiefRole
        {
            get { return mChiefRole; }
        }

        CCore.World.WorldInit mWorldInit;
        public CCore.World.WorldInit WorldInit
        {
            get { return mWorldInit; }
        }

        public Guid CurMapId { get; set; }
        public void LoadMap(Guid mapId)
        {
            CurMapId = mapId;

            var absFileName = CSUtility.Map.MapManager.Instance.GetMapPath(mapId);            

            CCore.Engine.Instance.Client.MainWorld.Cleanup();
            var newWorld = new CCore.World.World(mapId);
            mWorldInit = CCore.World.WorldInitFactory.Instance.CreateWorldInit((byte)(Client.World.enWorldInitType.Tile));            
            mWorldInit.Load(absFileName);
            CCore.Engine.Instance.Client.MainWorld = newWorld;
            newWorld.Initialize(mWorldInit);
            newWorld.Initialize(absFileName);
            newWorld.LoadWorld(absFileName);

            foreach( var i in newWorld.PostProceses )
            {
                if(i.Name=="HDR")
                {
                    //i.Enable = false;
                }
            }

            CCore.WeatherSystem.IlluminationManager.Instance.TimeAccelerate = 1200;
        }       

        void HideUI()
        {
            var uiForm = CCore.Support.ReflectionManager.Instance.GetUIForm("Login");
            if (uiForm == null)
                return;
            uiForm.Parent = null;

            uiForm = CCore.Support.ReflectionManager.Instance.GetUIForm("RoleSelect");
            if (uiForm == null)
                return;
            uiForm.Parent = null;
        }

        public UInt16 RoleTemplateId;
        public void Enter(Game game)
        {
            try
            {                
                CCore.Engine.Instance.Client.MsgRecieverMgr.RegReciever(Role.ChiefRoleActorController.Instance);

                mKeyUpBehavior = OnKeyBoardUp;

                HideUI();

                var form = CCore.Support.ReflectionManager.Instance.GetUIForm("LoadingUI") as UISystem.WinForm;
                if (form != null)
                {
                    form.Parent = null;
                    form.Parent = Game.Instance.RootUIMsg.Root;
                    ///form.UpdateUI();
                }

                Game.FOnAsyncExec exec = delegate()
                {
                    LoadMap(GameData.Support.ConfigFile.Instance.DefaultMapId);
                    if (mOffLineLogin)
                    {
                        CreateChiefPlayer();
                        UI.MainUIManager.Instance.InitMainUI();
                    }
                    else
                    {
                        CreateRoleWithServer(GameData.Support.ConfigFile.Instance.DefaultMapId, RoleTemplateId);
                    }
                    
                    Game.Instance.IsEditorMode = false;
                    
                    CSUtility.Data.DataTemplateManager<Byte, RoleCommonTemplate>.Instance.LoadAllDataTemplate();
                    if (mChiefRole != null)
                    {
                        var playerPos = mChiefRole.Placement.GetLocation();
                        CCore.Client.MainWorldInstance.TravelTo(playerPos.X, playerPos.Z);
                    }                    
                };

                Game.Instance.ProgressBackMission(exec,false);
            }
            catch (Exception)
            {

                
            }            
        }

        public void OnChiefPlayerCreateFinished(Role.RoleActor chief)
        {
            mChiefRole = chief;

            CCore.Engine.Instance.Client.MsgRecieverMgr.RegReciever(Role.ChiefRoleActorController.Instance);

            Game.Instance.IsEditorMode = false;

            var uiForm = CCore.Support.ReflectionManager.Instance.GetUIForm("Login");
            if (uiForm != null)
            {
                uiForm.Parent = null;
            }
        }


        public Role.RoleActor this[UInt32 i]
        {
            get
            {
                if (i == 0)
                    return null;
                Role.RoleActor role = Role.RoleManager.Instance.FindRoleActor(i);
                if (role != null)
                {
                    if (role.World == null)
                    {
                        role.DoLeaveMap();
                    }
                    else
                    {
                        var fr = role.World.FindActor(role.Id);//不在世界管理器里面，光在角色管理器，这个时候说明，之前过了地图卸载的区域，所以卸载了
                        if (fr == null)
                        {
                            role.World.AddActor(role);
                        }
                    }

                    return role;
                }

                #region 向服务器请求角色数据创建
                RPC.PackageWriter pkg = new RPC.PackageWriter();
                Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_GetRoleCreateInfo(pkg, i);
                pkg.WaitDoClient2PlanesPlayer(CCore.Engine.Instance.Client.GateSvrConnect, 10).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
                {
                    if (bTimeOut)
                        return;
                    role = Role.RoleManager.Instance.FindRoleActor(i);
                    if (role != null)//多线程是可能的
                        return;
                    var roleIdManager = Role.RoleManager.Instance;
                    sbyte successed = -1;
                    _io.Read(out successed);
                    switch (successed)
                    {
                        case -100:
                            {
                                Log.FileLog.WriteLine("不正常的角色创建数据请求");
                            }
                            break;
                        case 2:
                            {
                                 var  ri = new PlayerData();
                                _io.Read(ri);
                                SlimDX.Vector3 loc;
                                _io.Read(out loc);
                                float direction = 0;
                                _io.Read(out direction);
                                UInt32 singleId;
                                _io.Read(out singleId);
                                string statename = "";
                                _io.Read(out statename);
                                if (statename == "Death")
                                    break;

                                if (ChiefRole !=null &&  singleId ==ChiefRole.SingleId)
                                {
                                   // PushBackText("创建角色Id和主角相同，", CSCommon.Data.Com.PushTextType.WarningType);
                                    return;
                                }
                                SlimDX.Quaternion rot = SlimDX.Quaternion.RotationYawPitchRoll(direction, 0, 0);
                                var player = Role.RoleActor.CreateOtherPlayer(singleId,ri,loc, rot);
                                //                                 int count = 0;
                                //                                 _io.Read(out count);
                                //                                 List<VisualBuff> buffs = new List<VisualBuff>();
                                //                                 if (count != 0)
                                //                                 {
                                //                                     for (int k = 0; k < count; ++k)
                                //                                     {
                                //                                         VisualBuff buff = new VisualBuff();
                                //                                         _io.Read(out buff.mTemplateId);
                                //                                         _io.Read(out buff.mBuffId);
                                //                                         _io.Read(out buff.mPosition);
                                //                                         _io.Read(out buff.mLevel);
                                //                                         var mbuff = player.BuffSlot.CreateAddBuff(buff.mBuffId, buff.mTemplateId, buff.mPosition, buff.mLevel);
                                //                                         buffs.Add(buff);
                                //                                         _io.Read(out buff.mLiveTime);
                                //                                         if (mbuff != null)
                                //                                         {
                                //                                             mbuff.FreshCreateTime(buff.mLiveTime);
                                //                                         }
                                //                                     }
                                //                                 }
                                //                                 bool isVendor = false;
                                //                                 string vendorName = "";
                                //                                 _io.Read(out isVendor);
                                //                                 _io.Read(out vendorName);
                                //                                 player.IsVendor = isVendor;
                                //                                 if (isVendor)
                                //                                 {
                                //                                     player.VendorName = vendorName;
                                //                                 }

                                var state = player.AIStates.GetState(statename);
                                if (state == null)
                                {
                                    player.CurrentState.ToState("Idle", null);
                                }
                                else
                                {
                                    var param = state.Parameter;
                                    if (param == null)
                                    {
                                        player.CurrentState.ToState("Idle", null);
                                    }
                                    _io.Read(param);
                                    player.CurrentState.ToState(statename, param);
                                }
                            }
                            break;
                        case 1:
                            {
                                try
                                {
                                    MonsterData data = new MonsterData();
                                    _io.Read(data);
                                    if (data.Template == null)
                                        break;
                                    UInt32 singleId;
                                    _io.Read(out singleId);
                                    string statename = "";
                                    _io.Read(out statename);
                                    if (statename == "Death")
                                        break;
                                    var monster = Role.RoleActor.CreateMonster(singleId, data);
                                    if (monster == null)
                                        return;
                                    var state = monster.AIStates.GetState(statename);
                                    if (state != null)
                                    {
                                        _io.Read(state.Parameter);
                                        monster.CurrentState.ToState(statename, state.Parameter);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    Log.FileLog.WriteLine("NPC创建异常");
                                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                                    break;
                                }
                            }
                            break;
                        default:
                            {

                            }
                            break;
                    }
                };
                #endregion

                return null;
            }
        }

        //本地创建测试主角
        public Role.RoleActor CreateChiefPlayer()
        {
            try
            {
                if (mChiefRole != null)
                    Role.RoleManager.Instance.UnmapRoleId(mChiefRole);

                var pd = new PlayerData();
                pd.TemplateId = 1;
                pd.RoleMoveSpeed = pd.RoleTemplate.MoveSpeed;
                pd.OriPosition = pd.Position = pd.RoleTemplate.OriginPosition;

                mChiefRole = Role.RoleActor.CreateChiefPlayer(1, pd);
                return mChiefRole;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public void Leave(Game game)
        {
            CCore.Engine.Instance.Client.MsgRecieverMgr.UnRegReciever(mChiefRole.ActorController);

            Role.RoleManager.Instance.ClearIds();

            CCore.Client.MainWorldInstance.Cleanup();
            mChiefRole = null;
            CCore.Engine.Instance.Client.MainWorld = null;
        }

        public void RenderThreadTick(Game game)
        {
            Title.TitleShowManager.Instance.Tick(CCore.Engine.Instance.GetElapsedMillisecond());
            UI.MainUIManager.Instance.TickEffect(CCore.Engine.Instance.GetElapsedMillisecond());
        }

        //long mTickEdgeDetectDeltaTime = 0;
        //long mTickEdgeDetectCurTime = 0;
        //bool mIsTriggleTitle = false;
        CCore.Component.RoleActorVisual mPreEdgeDetectMesh = null;

        public void Tick(Game game)
        {
            if (mChiefRole == null)
                return;

            if (mChiefRole.World == null)
                CCore.Client.MainWorldInstance.AddActor(mChiefRole);

            var elapsedMillisecond = CCore.Engine.Instance.GetElapsedMillisecond();
            var roleVisual = mChiefRole.Visual as CCore.Component.RoleActorVisual;
        
            var playerPos = mChiefRole.Placement.GetLocation();

            if (!Game.Instance.IsEditorMode)
            {
                CCore.Client.MainWorldInstance.TravelTo(playerPos.X, playerPos.Z);
                if (mChiefRole.ActorPtr == IntPtr.Zero || CCore.Client.MainWorldInstance.FindActor(mChiefRole.Id) == null)
                {
                    playerPos.Y = mChiefRole.GetAltitude(playerPos.X, playerPos.Z);
                    mChiefRole.Placement.SetLocation(ref playerPos);
                    CCore.Client.MainWorldInstance.AddActor(mChiefRole);
                }

                // 游戏模式下取消主角提交HitProxy
                if (roleVisual != null)
                    roleVisual.CanHitProxy = false;
            }
            else
            {
                if (roleVisual != null)
                    roleVisual.CanHitProxy = true;
            }

            ChiefRole.TickSkillCD(elapsedMillisecond);

            // 处理鼠标悬停提示
//             mTickEdgeDetectCurTime += elapsedMillisecond;
//             if (mTickEdgeDetectCurTime > mTickEdgeDetectDeltaTime && !mIsTriggleTitle)
//             {
//                 if (mPreEdgeDetectMesh != null)
//                     mPreEdgeDetectMesh.EdgeDetect = false;
// 
//                 var roleActor = GetMouseClickRole((int)mMousePos.X, (int)mMousePos.Y);
//                 if (roleActor != null && roleActor.CurrentState != null && roleActor.CurrentState.StateName == "Death")
//                     roleActor = null;
//                 if (!UISystem.Device.Mouse.Instance.IsHitOnWin())
//                 {
//                     if (CanShowEdgeDetect(roleActor))
//                         ShowSelectRoleEdgeDetect(roleActor);
//                     if (roleActor != null && roleActor.SingleId == 0 && roleActor.GameType == (UInt32)CSUtility.Component.EActorGameType.Player)
//                         roleActor.DoLeaveMap();
//                 }
//                 mTickEdgeDetectCurTime = 0;
//             }

            Role.RoleManager.Instance.Tick();
            Role.ChiefRoleActorController.Instance.Tick();
            Title.HitShowManager.Instance.Tick(elapsedMillisecond);
            Skill.SkillController.Instance.Tick(elapsedMillisecond);
        }

        public bool CanShowEdgeDetect(Role.RoleActor roleActor)
        {
            if (roleActor != null)
            {
                if (roleActor.IsChielfPlayer() == false && roleActor.CurrentState != null && roleActor.CurrentState.StateName != "Death")
                {
                    return true;
                }
            }
            return false;
        }

        public void ShowSelectRoleEdgeDetect(Role.RoleActor roleActor)
        {
            if (roleActor.IsChielfPlayer() == false)
            {
                var roleVisual = roleActor.Visual as CCore.Component.RoleActorVisual;
                if (roleVisual != null)
                {
                    roleVisual.EdgeDetect = true;
                    mPreEdgeDetectMesh = roleVisual;
                }
            }
        }

        public Role.RoleActor GetMouseClickRole(int mouseX, int mouseY)
        {
            var hitIdList = Game.Instance.REnviroment?.GetHitProxyArea(mouseX, mouseY, 32, 32, 8);
            if (hitIdList.Count == 0)
                return null;
            foreach (var id in hitIdList)
            {
                Guid roleId = CCore.Graphics.HitProxyMap.Instance.GetActorId(id);
                var role = CCore.Client.MainWorldInstance.FindActor(roleId);
                var roleactor = role as Role.RoleActor;
                if (roleactor == null)
                    continue;
                if (roleactor.CurrentState != null && roleactor.CurrentState.StateName == "Death")
                    continue;

                // TODO 优先级判断
                return roleactor;
            }

            return null;
        }

        public void AfterFire(GameData.Skill.SkillData skill)
        {
            if (skill.SkillLevel > 0 && skill.Template.SkillLevelDatas.Count >= skill.SkillLevel)
                skill.RemainCD = skill.Template.SkillLevelDatas[skill.SkillLevel - 1].CD;
        }

        public bool CanFire(GameData.Skill.SkillData skill)
        {
            if (skill == null || skill.Template == null || skill.Template.SkillLevelDatas.Count < skill.SkillLevel || skill.SkillLevel == 0)
                return false;
            if (skill.RemainCD <= 0)
            {
                if (mChiefRole.RoleData.RoleMP >= skill.Template.SkillLevelDatas[skill.SkillLevel - 1].SkillConsumeMP)
                {
                    return true;
                }
                //else
                //    MainActivity.Instance.UpdataHintModelUI("魔法不足!");
            }
            return false;
        }

        public CCore.MsgProc.FBehaviorProcess FindBehavior(CCore.MsgProc.BehaviorParameter bhInit)
        {
            switch (bhInit.GetBehaviorType())
            {   
                case (int)BehaviorType.BHT_KB_Char_Up:
                    return mKeyUpBehavior;
                case (int)BehaviorType.BHT_KB_Char_Down:
                    return OnKeyBoardDown;
                case (int)BehaviorType.BHT_Mouse_Move:
                    return OnMouseMove;
            }

            return null;

        }
        private CCore.MsgProc.FBehaviorProcess mKeyUpBehavior;

        int OnKeyBoardUp(CCore.MsgProc.BehaviorParameter parameter)
        {
            if (parameter is CCore.MsgProc.Behavior.KB_Char)
            {
                var param = parameter as CCore.MsgProc.Behavior.KB_Char; 
                switch (param.Key)
                {
                    case CCore.MsgProc.BehaviorParameter.enKeys.P:
                        {
                            Game.Instance.IsEditorMode = !Game.Instance.IsEditorMode;
                        }
                        break;
                    case BehaviorParameter.enKeys.F11:  //隐藏显示游戏UI
                        {
                            if (Game.Instance.RootUIMsg.Root.Visibility == UISystem.Visibility.Visible)
                            {
                                Game.Instance.RootUIMsg.Root.Visibility = UISystem.Visibility.Collapsed;
                                Game.Instance.RootUIMsg.Root.HitTestVisible = false;
                            }
                            else
                            {
                                Game.Instance.RootUIMsg.Root.Visibility = UISystem.Visibility.Visible;
                                Game.Instance.RootUIMsg.Root.HitTestVisible = true;
                            }
                        }
                        break;
                    //case BehaviorParameter.enKeys.Q:
                    //    {
                    //        if (mChiefRole == null || mChiefRole.RoleData == null || mChiefRole.RoleData.PlayerData  == null ||
                    //            mChiefRole.RoleData.PlayerData.Skills.Count ==0 )
                    //            return 0;
                            
                    //        OnFireSkill(mChiefRole.RoleData.PlayerData.Skills[0]);
                    //    }
                    //    break;
                    //case BehaviorParameter.enKeys.W:
                    //    {
                    //        if (mChiefRole == null || mChiefRole.RoleData == null || mChiefRole.RoleData.PlayerData == null ||
                    //            mChiefRole.RoleData.PlayerData.Skills.Count <2)
                    //            return 0;

                    //        OnFireSkill(mChiefRole.RoleData.PlayerData.Skills[1]);
                    //    }
                    //    break;
                    //case BehaviorParameter.enKeys.E:
                    //    {
                    //        if (mChiefRole == null || mChiefRole.RoleData == null || mChiefRole.RoleData.PlayerData == null ||
                    //            mChiefRole.RoleData.PlayerData.Skills.Count < 3)
                    //            return 0;

                    //        OnFireSkill(mChiefRole.RoleData.PlayerData.Skills[2]);
                    //    }
                    //    break;
                    //case BehaviorParameter.enKeys.R:
                    //    {
                    //        if (mChiefRole == null || mChiefRole.RoleData == null || mChiefRole.RoleData.PlayerData == null ||
                    //            mChiefRole.RoleData.PlayerData.Skills.Count < 4)
                    //            return 0;

                    //        OnFireSkill(mChiefRole.RoleData.PlayerData.Skills[3]);
                    //    }
                    //    break;
                }          
            }
            return 0;
        }

        int OnKeyBoardDown(BehaviorParameter parameter)
        {
            return 0;
        }

        int OnRBDown(BehaviorParameter parameter)
        {
            return 0;
        }

        SlimDX.Vector2 mMousePos = new SlimDX.Vector2();
        public SlimDX.Vector2 MousePos
        {
            get { return MousePos; }
        }

        public int OnMouseMove(BehaviorParameter parameter)
        {
            CCore.MsgProc.Behavior.Mouse_Move param = parameter as CCore.MsgProc.Behavior.Mouse_Move;
            mMousePos.X = param.X;
            mMousePos.Y = param.Y;
//             var mouse = Bag.MouseBag.Instance.UIHolder;
//             if (mouse != null)
//             {
//                 var pos = new System.Drawing.Point(param.X - 19, param.Y - 19);
//                 mouse.MoveWin(ref pos);
//             }

//             if (Store.StoreUI.Instance.bStoreUIDrag == true)
//             {
//                 var delta = param.X - Store.StoreUI.Instance.mStoreUIMousePos.X;
//                 Store.StoreUI.Instance.mStoreUIRoleAngle += delta;
//                 var rolePlacement = FrameSet.Role.RoleActor.mStoreChiefRoleActor.Placement;
//                 if (rolePlacement != null)
//                 {
//                     var tempDelta = mCaptureWorld.mPlayerInitRotation * SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitY, (float)(-Store.StoreUI.Instance.mStoreUIRoleAngle * System.Math.PI / 180));
//                     rolePlacement.SetRotation(ref tempDelta);
//                 }
//                 Store.StoreUI.Instance.mStoreUIMousePos.X = param.X;
//                 Store.StoreUI.Instance.mStoreUIMousePos.Y = param.Y;
//             }

            return 0;
        }

        public void OnFireSkill(GameData.Skill.SkillData skill)
        {
            if (skill == null)
                return;

            
            mChiefRole.OnFireSkill(skill);
        }

        void CreateRoleWithServer(Guid mapId,UInt16 roleTemplateId)
        {
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            CSUtility.Net.NetConnection connect = new CSUtility.Net.NetConnection();
            Server.H_RPCRoot.smInstance.HGet_GateServer(pkg).TryEnterGame(pkg, mapId, roleTemplateId);
            pkg.WaitDoCommandWithTimeOut(-1, CCore.Engine.Instance.Client.GateSvrConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace()).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;

                sbyte successed = 0;
                _io.Read(out successed);
                if (successed == 1)
                {
                    PlayerData pd = new PlayerData();
                    _io.Read(pd);
                    uint singleId;
                    _io.Read(out singleId);
                    int count = 0;
                    _io.Read(out count);
                    for (int i = 0; i < count; i++)
                    {
                        GameData.Skill.SkillData skill = new GameData.Skill.SkillData();
                        _io.Read(skill);
                        skill.TemplateId = skill.TemplateId;
                        pd.Skills.Add(skill);
                    }
                    mChiefRole = Role.RoleActor.CreateChiefPlayer(singleId, pd);
                    UI.MainUIManager.Instance.InitMainUI();
                }
            };
        }

        public void RPC_ChiefPlayerEnterMap(UInt32 chiefSingleId, Guid mapSourceId, SlimDX.Vector3 pos, RPC.DataReader sceneParameter)
        {//主角进入地图
            if (mChiefRole == null)
            {
                return;
            }
            
            Role.RoleManager.Instance.UnmapRoleId(mChiefRole);
            Role.RoleManager.Instance.ClearIds();
            Role.RoleManager.Instance.MapRoleId(mChiefRole, chiefSingleId);
            Role.RoleActor.RegionActor.RegionActivity = false;

            LoadMap(mapSourceId);

            mChiefRole.Placement.SetLocation(ref pos);
            CCore.Client.MainWorldInstance.TravelTo(pos.X, pos.Y);
            CCore.Client.MainWorldInstance.AddActor(mChiefRole);
            //FrameSet.Role.RoleIdManager.Instance.MapRoleId(mChiefRole, mChiefRole.SingleId);
            //CCore.Client.MainWorldInstance.ChiefRole = mChiefRole;

            // 读取周围的NPC
//             Byte npcCount = 0;
//             sceneParameter.Read(out npcCount);
//             for (Byte i = 0; i < npcCount; i++)
//             {
//                 CSCommon.Data.NPCData data = new CSCommon.Data.NPCData();
//                 sceneParameter.Read(data, true);
//                 UInt32 singleId;
//                 sceneParameter.Read(out singleId);
//                 int count;
//                 ushort Id;
//                 Guid roleId;
//                 sceneParameter.Read(out count);
//                 for (int j = 0; j < count; ++j)
//                 {
//                     sceneParameter.Read(out Id);
//                     data.AcceptTasks.Add(Id);
//                 }
//                 sceneParameter.Read(out count);
//                 for (int j = 0; j < count; ++j)
//                 {
//                     sceneParameter.Read(out Id);
//                     data.FinishTasks.Add(Id);
//                 }
//                 sceneParameter.Read(out count);
//                 for (int j = 0; j < count; ++j)
//                 {
//                     sceneParameter.Read(out Id);
//                     data.RandAcceptTasks.Add(Id);
//                 }
//                 sceneParameter.Read(out count);
//                 for (int j = 0; j < count; ++j)
//                 {
//                     sceneParameter.Read(out Id);
//                     data.RandFinishTasks.Add(Id);
//                 }
//                 sceneParameter.Read(out count);
//                 for (int j = 0; j < count; ++j)
//                 {
//                     sceneParameter.Read(out roleId);
//                     data.CaBeAttackRole.Add(roleId);
//                 }
// 
//                 var npc = FrameSet.Role.RoleIdManager.Instance.FindRoleActor(singleId);
//                 if (npc == null)
//                 {
//                     npc = FrameSet.Role.RoleActor.CreateNpc(singleId, data, MidLayer.IClient.MainWorldInstance, FrameSet.Role.RoleIdManager.Instance, OnHurtRoleIdChanged, OnBeAttack);
//                     npc.InitTaskEffect();
//                 }
//             }

            // 读取周围玩家
            Byte playerCount = 0;
            sceneParameter.Read(out playerCount);
            for (Byte playerIdx = 0; playerIdx < playerCount; playerIdx++)
            {
                PlayerDetail ri = new PlayerDetail();
                sceneParameter.Read(ri, true);
                UInt32 singleId;
                sceneParameter.Read(out singleId);
                var player = Role.RoleManager.Instance.FindRoleActor(singleId);
                if (player == null)
                {
                    SlimDX.Vector3 loc = new SlimDX.Vector3(ri.LocationX, ri.LocationY, ri.LocationZ);
                    // 角色默认只有Y轴方向旋转
                    SlimDX.Quaternion rot = SlimDX.Quaternion.RotationYawPitchRoll(ri.Direction, 0, 0);
                    //player = FrameSet.Role.RoleActor.CreateOtherPlayer(singleId, ref loc, ref rot, ri, MidLayer.IClient.MainWorldInstance, FrameSet.Role.RoleIdManager.Instance, OnHurtRoleIdChanged, OnBeAttack);
                }
            }
        }

        public static bool mOffLineLogin = true;

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void OnOffLineLogin(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter e)
        {
            mOffLineLogin = true;
            Game.Instance.SetCurrentStage(this);
        }

        public void UpdateRoleValue(string name)
        {
            switch (name)
            {
                case "RoleHp":
                    break;
                case "RoleMp":
                    break;
                case "MaxRoleHp":
                    break;
                case "MaxRoleMp":
                    break;
                case "RoleLevel":

                    break;
                case "RoleExp":
                    break;
                case "RoleGod":
                    break;
                case "RoleMoveSpeed":
                    break;
                case "RoleSkillPoint":
                    UI.MainUIManager.Instance.UpdateRoleSkill();
                    break;
            }
        }
    }
}
