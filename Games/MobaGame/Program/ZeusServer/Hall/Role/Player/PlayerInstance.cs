using System;
using System.Collections.Generic;
using System.Text;
using CSUtility.Component;
using RPC;
using Server.Hall.Map;
using GameData.Role;

namespace Server.Hall.Role.Player
{
    [RPC.RPCClassAttribute(typeof(PlayerInstance))]
    public partial class PlayerInstance : RoleActor, RPC.RPCObject
    {
        ~PlayerInstance()
        {
            PlayerInstanceNumber--;
        }

        public override int FactionId
        {
            get
            {
                return PlayerData.PlayerFaction;
            }
        }

        #region 地图内玩家下标
        UInt16 mIndexInMap = System.UInt16.MaxValue;
        public UInt16 IndexInMap
        {
            get { return mIndexInMap; }
        }
        internal void _SetIndexInMap(UInt16 v)
        {
            mIndexInMap = v;
        }
        #endregion

        UInt16 mClientLinkId = UInt16.MaxValue;
        public UInt16 ClientLinkId
        {
            get { return mClientLinkId; }
            set { mClientLinkId = value; }
        }

        [CSUtility.Event.Attribute.AllowMember("角色对象.PlayerData", CSUtility.Helper.enCSType.Server, "角色数据")]
        public PlayerData PlayerData { get; set; } = new PlayerData();

        SCore.TcpServer.TcpConnect mPlanes2GateConnect;
        public SCore.TcpServer.TcpConnect Planes2GateConnect
        {
            get { return mPlanes2GateConnect; }
            set { mPlanes2GateConnect = value; }
        }

//         SCore.TcpServer.TcpClient mPlanes2ClientConnect;
//         public SCore.TcpServer.TcpClient Planes2ClientConnect
//         {
//             get { return mPlanes2ClientConnect; }
//             set { mPlanes2ClientConnect = value; }
//         }

        #region 一些重载
        public override Guid Id
        {
            get
            {
                return PlayerData.RoleId;
            }
        }

        public override int RoleHP
        {
            get { return PlayerData.RoleHp; }
            set { PlayerData.RoleHp = value; }
        }

        public override  int RoleMaxHP
        {
            get{ return PlayerData.MaxRoleHp; }
            set { PlayerData.MaxRoleHp = value; }
        }

        public override int RoleMaxMP
        {
            get { return PlayerData.MaxRoleMp; }
            set { PlayerData.MaxRoleMp = value; }
        }

        public override int RoleMP
        {
            get { return PlayerData.RoleMp; }
            set { PlayerData.RoleMp = value; }
        }

        public override string ActorName
        {
            get
            {
                return RoleName;
            }
        }

        Role.RoleActor mAttackTarget = null;
        public override Role.RoleActor AttackTarget
        {
            get
            {
                if (mAttackTarget != null)
                    return mAttackTarget;

                return base.AttackTarget;
            }
        }

        public override RoleTemplate RoleTemplate
        {
            get { return PlayerData.RoleTemplate; }
        }

        public override void FreshRoleValue(bool freshhpmp)
        {
            base.FreshRoleValue(freshhpmp);
        }

        public override void OnPlacementChanged(IPlacement placement)
        {
            base.OnPlacementChanged(placement);
        }

        public override void OnEnterMap(MapInstance map)
        {
            //进入地图时设置坐标
            base.OnEnterMap(map);
        }

        public override void DangrouseOnLeaveMap()
        {
            base.DangrouseOnLeaveMap();
        }
        
        public override void Tick(long elapseMillsecond)
        {
            Int64 time = IServer.Instance.GetElapseMilliSecondTime();
            base.Tick(elapseMillsecond);

//             mUpdateDurableTime += elapsedMiliSeccond;
//             if (CurrentState.StateName == "StayAttack")
//             {
//                 FireTime += elapsedMiliSeccond;
//                 if (mUpdateDurableTime > 600000 && FireTime != 0)
//                 {
//                     FreshRoleEquipDurable(FireTime, true);
//                     FireTime = 0;
//                     mUpdateDurableTime = 0;
//                 }
//             }
//             mUpdateShoppingTime += elapsedMiliSeccond;
//             if (mUpdateShoppingTime > 20000)
//             {
//                 mUpdateShoppingTime = 0;
//                 if (DateTime.Today != ClearTime)
//                 {
//                     CleatShoppingData();
//                 }
//             }
        }

        public override void TickSkillCD(long elapseMillsecond)
        {
            base.TickSkillCD(elapseMillsecond);
            foreach (var skill in PlayerData.Skills)
            {
                if (skill.RemainCD > 0)
                    skill.RemainCD -= (float)elapseMillsecond / 1000f;
                if (skill.RemainCD < 0)
                    skill.RemainCD = 0;
            }
        }

        public override void TickBloodReturn(long elapseMillsecond)
        {
            base.TickBloodReturn(elapseMillsecond);
            if (IsEnterHotSpring)
            {
                RoleHP += (int)(((float)elapseMillsecond / 1000.0f) * 50);
                if (RoleHP > RoleMaxHP)
                    RoleHP = RoleMaxHP;
            }
        }

        public override bool CanLockon(RoleActor target)
        {
            return base.CanLockon(target);
        }

        public override bool CanAttack(RoleActor target)
        {
            if (!base.CanAttack(target))
                return false;

            if (FactionId != target.FactionId)
                return true;

            return false;
        }

        public override bool GetRoleAttachFighting()
        {
            return true;
        }

        public override bool OnValueChanged(string name, DataWriter value)
        {
            if (base.OnValueChanged(name, value))
                return true;

            switch (name)
            {
                case "RoleExp":
                case "RoleGold":
                case "RoleSkillPoint":
                    //这些数据只需要告诉玩家自己，不需要告诉别的客户端
                    var pkg = new RPC.PackageWriter();
                    Client.H_GameRPC.smInstance.HIndex(pkg, this.SingleId).RPC_UpdateRoleValue(pkg, name, value);
                    pkg.DoCommandPlanes2Client(this.Planes2GateConnect, this.ClientLinkId);
                    return true;
            }

            return true;

        }

        public override void GainExp(int exp)
        {
            if (exp < 0)
                return;
            var maxLevel = CSUtility.Data.DataTemplateManager<Byte, ExpLevel>.Instance.GetDataTemplate((Byte)0).GetRoleMaxLevel();// ExpLevel.Instance.GetRoleMaxLevel();
            if (RoleLevel >= maxLevel)
                return;

            PlayerData.RoleExp += exp;
            var curLevel = PlayerData.RoleLevel;
            var nextLevelExp = CSUtility.Data.DataTemplateManager<Byte, ExpLevel>.Instance.GetDataTemplate((Byte)0).GetLevelupExp(curLevel);// ExpLevel.Instance.GetLevelupExp(curLevel);
            while (nextLevelExp <= PlayerData.RoleExp)
            {
                PlayerData.RoleLevel += 1;  //角色升级
                PlayerData.RoleSkillPoint += 1; //获得一个技能点
                PlayerData.RoleExp -= nextLevelExp;
                nextLevelExp = CSUtility.Data.DataTemplateManager<Byte, ExpLevel>.Instance.GetDataTemplate((Byte)0).GetLevelupExp(PlayerData.RoleLevel);// ExpLevel.Instance.GetLevelupExp(PlayerData.RoleLevel);
            }
            if (curLevel != PlayerData.RoleLevel)
            {
                //通知客户端角色 播放等级升级特效,
                var pkg = new RPC.PackageWriter();
                Client.H_GameRPC.smInstance.HIndex(pkg, this.SingleId).RPC_LevelUpEffect(pkg);
                pkg.DoCommandPlanes2Client(this.Planes2GateConnect, this.ClientLinkId);

                //重新计算角色属性
                FreshRoleValue(false);
            }
        }

        public void OnKillerOther(Role.RoleActor role)
        {
            var addexp = 100 * role.RoleLevel;
            if (role.RoleCreateType == ERoleType.Player)
            {
                GainExp(addexp);
                GainMoney(200);
            } 
            else
            {
                GainMoney(45);
            }
        }

        public override void GainMoney(int money)
        {
            if (money == 0)
                return;
            PlayerData.RoleGold += money;

            //通知客户端角色金钱变化了
        }

        public override void OnPlacementUpdatePosition(ref SlimDX.Vector3 pos)
        {
            base.OnPlacementUpdatePosition(ref pos);

            PlayerData.Position = pos;

            HostMap.TourRoles((Int32)GameData.Role.ERoleType.Trigger, ref pos, 20, this.OnVisitRole_ProcessTrigger, null);
        }
        #endregion

        #region 初始化

        public static int PlayerInstanceNumber = 1;//这是一个RPC对象，有一个不通过CreatePlayerInstance创建的，然后销毁了
        public static PlayerInstance CreatePlayerInstance(PlayerData pd, SCore.TcpServer.TcpConnect p2gConnect, UInt16 linkId)
        {
            try
            {
                CSUtility.Component.ActorInitBase actInit = new CSUtility.Component.ActorInitBase();
                actInit.GameType = (UInt16)CSUtility.Component.EActorGameType.Player;
                PlayerInstance ret = new PlayerInstance();
                ret.Initialize(actInit);
                ret.RoleCreateType = ERoleType.Player;
                ret.OwnerRole = ret;
                ret.StateNotify2Remote = false;            
                if(pd.RoleTemplate ==null)
                {
                    Log.FileLog.WriteLine("");
                }
                ret.InitComponent();
                if (false == ret.InitRoleInstance(null, pd, p2gConnect, linkId))
                    return null;

                ret.InitFSM(Guid.Parse("dfd962c0-1a16-4974-b9d1-a4c3e65ce6ef"), true);
                
                if (ret.CurrentState != null)
                    ret.CurrentState.OnRoleCreatEnd();

                PlayerInstanceNumber++;
                return ret;
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
                return null;
            }      
        }
        private bool InitRoleInstance(Hall.HallInstance hall, PlayerData pd, SCore.TcpServer.TcpConnect p2gConnect, UInt16 linkId)
        {
            mPlanes2GateConnect = p2gConnect;
            mClientLinkId = linkId;

            PlayerData = pd;
            PlayerData._SetHostPlayer(this);

            #region 技能初始化
            PlayerData.Skills.Clear();
            var roleTemplate = PlayerData.RoleTemplate;
            if (roleTemplate != null)
            {
                bool first = true;
                foreach (var id in roleTemplate.RoleSkillList)
                {
                    var data = new GameData.Skill.SkillData();
                    data.TemplateId = id;
                    if (first)
                    {
                        //第一个技能恒定为普通攻击，等级为1
                        first = false;
                        data.SkillLevel = 1;
                    }
                    else
                    {
                        data.SkillLevel = 0;
                    }
                    if (data.Template != null)
                        PlayerData.Skills.Add(data);
                }
            }

            #endregion
            FreshRoleValue(true);
            PlayerData.RoleLevel = 1;
            PlayerData.RoleSkillPoint = 1;
            PlayerData.RoleMoveSpeed = RoleTemplate.MoveSpeed;
            return true;
        }

        private bool InitComponent()
        {            
            mPlacement = new Role.RolePlacement(this);
            float limit = 0;
            if (RoleTemplate!=null)
                limit = RoleTemplate.HalfHeight * 3;
            mGravity = new CSUtility.Component.IGravityComp(limit);
            return true;
        }

        #endregion

        #region 显示逻辑同步
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_FSMChangeState(UInt32 singleId, string curState, string newCurState, string newtarState, RPC.DataReader parameter, RPC.RPCForwardInfo fwd)
        {
            if (singleId == this.SingleId)
            {
                var state = AIStates.GetState(newCurState);
                if (state == null)
                {
                    //这里可以考虑转换成为一个Idle状态
                    this.CurrentState.ToState("Idle", null);
                    return;
                }
                CSUtility.AISystem.StateParameter param = state.Parameter;
                if (param == null)
                {
                    //这里可以考虑转换成为一个Idle状态
                    this.CurrentState.ToState("Idle", null);
                    return;
                }
                parameter.Read(param, true);
                if (this.CurrentState != null)
                    this.CurrentState.ToState(newCurState, param);
            }
            else
            {
                Role.RoleActor role = this.FindSummon(singleId);
                if (role != null)
                {
                    var state = role.AIStates.GetState(newCurState);
                    if (state == null)
                    {
                        //这里可以考虑转换成为一个Idle状态
                        role.CurrentState.ToState("Idle", null);
                        return;
                    }
                    CSUtility.AISystem.StateParameter param = state.Parameter;
                    if (param == null)
                    {
                        //这里可以考虑转换成为一个Idle状态
                        role.CurrentState.ToState("Idle", null);
                        return;
                    }
                    parameter.Read(param, true);
                    if (role.CurrentState != null)
                        role.CurrentState.ToState(newCurState, param);
                }
            }
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_UpDateRoleState(UInt32 roleid)
        {
            var role = this.HostMap.GetRole(roleid);
            if(role !=null)
                role.FSMOnToState(role.CurrentState,role.CurrentState.Parameter, role.CurrentState, null);
            Log.FileLog.WriteLine("RPC_UpDateRoleState State:" +role.CurrentState.StateName);
        }

        long mPrevRPCUpdatePositionTime = 0;
        SlimDX.Vector3 mPrevUpdatePosition;
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_UpdatePosition(SlimDX.Vector3 pos, float dir, RPC.RPCForwardInfo fwd)
        {
            var delta = SlimDX.Vector3.Distance(pos, this.Placement.GetLocation());
            if (delta < 0.5)
                return;
            var serverPos = this.Placement.GetLocation();
            this.Placement.SetRotationY(dir, this.RoleTemplate.MeshFixAngle);
            this.Placement.SetLocation(ref pos);

            //防止客户端告诉不靠谱的位置，这里目前没有处理连续发包，每个包位置差都控制在2米的情况
            var nowTime = IServer.Instance.GetElapseMilliSecondTime();
            float elapse = (float)(nowTime - mPrevRPCUpdatePositionTime) * 0.001F;
            SynchronousState2Client(serverPos, pos);
            if (elapse > 1.0)
            {
                float maxDist = MoveSpeed * 2 * elapse;
                float dist = SlimDX.Vector3.Distance(pos, mPrevUpdatePosition);

                if (dist > maxDist)
                {
                    OnClientCheat(50, string.Format("移动速度异常，同步间隔最大移动距离{0}，实际移动距离{1}", maxDist, dist));

                    //有可能是网络延迟导致的，告诉客户端你现在真正的位置，目前没法做，因为重力，地图碰撞等服务器没有走，会导致和客户端来回扯的问题
                    //this.Placement.SetLocation(ref mPrevUpdatePosition);
                    mPrevRPCUpdatePositionTime = nowTime;
                    mPrevUpdatePosition = pos;

                    //RPC.PackageWriter pkg = new RPC.PackageWriter();
                    //mPrevUpdatePosition = Placement.GetLocation();
                    //ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, this.SingleId).RPC_UpdatePosition(pkg, mPrevUpdatePosition, IServer.timeGetTime());
                    //pkg.DoCommandPlanes2Client(this.Planes2GateConnect, this.ClientLinkId);
                }
                else
                {
                    mPrevRPCUpdatePositionTime = nowTime;
                    mPrevUpdatePosition = pos;
                }
            }

            //if (!HostMap.IsNullMap)
            //{
            //    float alt = HostMap.GetAltitude(pos.X, pos.Z);
            //    RPC.PackageWriter retPkg = new RPC.PackageWriter();
            //    retPkg.Write(alt);
            //    retPkg.DoReturnPlanes2Client(fwd);
            //}
        }

        void SynchronousState2Client(SlimDX.Vector3 serverPos, SlimDX.Vector3 clientPos)
        {
            var dist = SlimDX.Vector3.Distance(serverPos, clientPos);

            UpdateMyPos2Near(clientPos);
            //同步新位置，同步状态，告诉周围玩家我来了
            var param = CurrentState.Parameter;
            this.FSMOnToState(CurrentState, param, CurrentState, null);
        }

        public void UpdateMyPos2Near(SlimDX.Vector3 pos)
        {
            //同步原来位置，同步位置，告诉周围玩家我走了
             RPC.PackageWriter pkg = new RPC.PackageWriter();
            Client.H_GameRPC.smInstance.HIndex(pkg, this.SingleId).RPC_UpdatePosition(pkg, (Int16)(pos.X * 10f), (Int16)(pos.Z * 10f));
            HostMap.SendPkg2Clients(this, Placement.GetLocation(), pkg);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_UpdateDirection(UInt32 singeId, float dir, RPC.RPCForwardInfo fwd)
        {
            Role.RoleActor role;
            if (SingleId == singeId)
            {
                role = this;
            }
            else
            {
                role = this.FindSummon(singeId);
            }
            if (role != null)
            {
                //role.Placement.SetRotationY(dir, this.RoleTemplate.MeshFixAngle);

//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
// 
//                 ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, singeId).RPC_UpdateDirection(pkg, dir);
// 
//                 HostMap.SendPkg2Clients(this, Placement.GetLocation(), pkg);
            }
        }


        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_GetRoleCreateInfo(UInt32 singleId, RPC.RPCForwardInfo fwd)
        {
            var map = this.HostMap;
            if (map == null)
                return;

            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            var player = map.RoleManager.PlayerManager.FindObj(singleId);
            if (player == null)
            {
                #region Player
                var npc = map.RoleManager.MonsterManager.FindObj(singleId);
                if (npc == null)
                {
                    retPkg.Write((sbyte)-100);//该NPC当前对玩家不可见
                    retPkg.DoReturnPlanes2Client(fwd);
                }
                else
                {
                    float fdistSq = SlimDX.Vector3.DistanceSquared(this.Placement.GetLocation(), npc.Placement.GetLocation());
                    if (fdistSq > RPC.RPCNetworkMgr.Sync2ClientRangeSq)
                    {
                        retPkg.Write((sbyte)-2);//距离太远
                        retPkg.DoReturnPlanes2Client(fwd);
                        return;
                    }
                    if (npc.CurrentState == null)
                    {
                        retPkg.Write((sbyte)-4);//Currentstate ==null
                        retPkg.DoReturnPlanes2Client(fwd);
                        return;
                    }
                    if (npc.CurrentState != null && npc.CurrentState.StateName == "Death")
                    {
                        retPkg.Write((sbyte)-3);//已经死了
                        retPkg.DoReturnPlanes2Client(fwd);
                        return;
                    }
                    else
                    {
                        retPkg.Write((sbyte)1);
                        retPkg.Write(npc.MonsterData);
                        retPkg.Write(npc.SingleId);
                        retPkg.Write(npc.CurrentState.StateName);                
                        retPkg.Write(npc.CurrentState.Parameter);
                        retPkg.DoReturnPlanes2Client(fwd);
                        return;
                    }
                }
                #endregion
            }
            else
            {
                if (player == this)
                {
                    retPkg.Write((sbyte)-100);//客户端要求得到自己角色信息，不正常的请求
                    retPkg.DoReturnPlanes2Client(fwd);
                    return;
                }
                float fdistSq = SlimDX.Vector3.DistanceSquared(this.Placement.GetLocation(), player.Placement.GetLocation());
                if (fdistSq > RPC.RPCNetworkMgr.Sync2ClientRangeSq)
                {
                    retPkg.Write((sbyte)-2);//距离太远
                    retPkg.DoReturnPlanes2Client(fwd);
                    return;
                }
                retPkg.Write((sbyte)2);
                retPkg.Write(player.PlayerData);
                retPkg.Write(player.Placement.GetLocation());
                retPkg.Write(player.Placement.GetDirection());
                retPkg.Write(player.SingleId);
                retPkg.Write(player.CurrentState.StateName);

                // buff背包
//                 var buffs = player.BuffBag.GetVisualBuffs();
//                 retPkg.Write(buffs.Count);
//                 if (buffs.Count != 0)
//                 {
//                     for (int i = 0; i < buffs.Count; ++i)
//                     {
//                         retPkg.Write(buffs[i].BuffTemplate.BuffId);
//                         retPkg.Write(buffs[i].BuffData.BuffId);
//                         retPkg.Write(buffs[i].Position);
//                         retPkg.Write(buffs[i].BuffData.BuffLevel);
//                         retPkg.Write(buffs[i].BuffData.LiveTime);
//                     }
//                 }
                retPkg.Write(player.CurrentState.Parameter);//状态参数
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_RelifeMe()
        {
            this.ResetDefaultState();            
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_GetRolePosition(UInt32 singleId, RPC.RPCForwardInfo fwd)
        {
            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            var player = HostMap.GetRole(singleId);

            if (player != null)
            {
                retPkg.Write((sbyte)1);
                retPkg.Write(player.Placement.GetLocation());
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }
            else
            {
                retPkg.Write((sbyte)-1);
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_GetRoleState(UInt32 singleId, RPC.RPCForwardInfo fwd)
        {
            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            var player = HostMap.GetRole(singleId);

            if (player != null)
            {
                retPkg.Write((sbyte)1);
                retPkg.Write(player.Placement.GetLocation());
                retPkg.Write(player.CurrentState.StateName);
                retPkg.Write(player.CurrentState.Parameter);
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }
            else
            {
                retPkg.Write((sbyte)-1);
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }
        }


        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_GetCurrentZeusTime(int timeAccelerate, RPC.RPCForwardInfo fwd)
        {
            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            DateTime centuryBegin = CSUtility.Support.IFileConfig.ZeusCenturyBegin;
            DateTime currentDate = DateTime.Now;
            long zeusTicks = (currentDate.Ticks - centuryBegin.Ticks) * timeAccelerate;
            retPkg.Write(zeusTicks);
            retPkg.DoReturnPlanes2Client(fwd);
        }

        #endregion

        #region 寻路
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_GlobalMapFindPath(SlimDX.Vector3 from, SlimDX.Vector3 to, CSUtility.Net.NetConnection connect, RPC.RPCForwardInfo fwd)
        {
            if (HostMap == null)
            {
                RPC.PackageWriter retPkg = new RPC.PackageWriter();
                int count = -100;
                retPkg.Write(count);
                retPkg.DoReturnCommand2(connect, fwd.ReturnSerialId);
                return;
            }
            var retSerialId = fwd.ReturnSerialId;
            var pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_PathFindServer(pkg).GlobalMapFindPath(pkg, this.HallInstance.HallsId, HostMap.MapSourceId, HostMap.MapSourceId, this.Id, from, to);
            pkg.WaitDoCommandWithTimeOut(3, HallServer.Instance.PathFindConnect, RPC.CommandTargetType.DefaultType, null).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                {
                    RPC.PackageWriter retPkg = new RPC.PackageWriter();
                    int count = -2;
                    retPkg.Write(count);
                    retPkg.DoReturnCommand2(connect, retSerialId);
                    return;
                }
                else
                {
                    RPC.PackageWriter retPkg = new RPC.PackageWriter();
                    Byte pathFindResult;
                    _io.Read(out pathFindResult);
                    switch ((CSUtility.Navigation.INavigationWrapper.enNavFindPathResult)pathFindResult)
                    {
                        case CSUtility.Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Success:
                            {
                                int count = 0;
                                _io.Read(out count);
                                retPkg.Write(count);
                                for (int i = 0; i < count; i++)
                                {
                                    SlimDX.Vector2 pathPt;
                                    _io.Read(out pathPt);

                                    var pt = new SlimDX.Vector3();
                                    pt.X = pathPt.X;
                                    pt.Z = pathPt.Y;
                                    pt.Y = HostMap.GetAltitude(pt.X, pt.Z);

                                    retPkg.Write(pt);
                                }
                                retPkg.DoReturnPlanes2Client(fwd);
                            }
                            break;
                        case CSUtility.Navigation.INavigationWrapper.enNavFindPathResult.ENFR_SESame:
                            {
                                int count = -3;
                                retPkg.Write(count);
                                retPkg.DoReturnPlanes2Client(fwd);
                            }
                            break;
                        case CSUtility.Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Cancel:
                            {
                                int count = -4;
                                retPkg.Write(count);
                                retPkg.DoReturnPlanes2Client(fwd);
                            }
                            break;
                        default:
                            {
                                int count = -1;
                                retPkg.Write(count);
                                retPkg.DoReturnPlanes2Client(fwd);
                            }
                            break;
                    }
                }
            };
        }
        #endregion

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_GetDynamicBlockValue(Guid actorId, RPC.RPCForwardInfo fwd)
        {
            RPC.PackageWriter retPkg = new RPC.PackageWriter();

            var map = this.HostMap;
            if (map == null)
            {
                retPkg.Write((sbyte)-1);
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }

            var dynamicBlock = map.GetDynamicBlock(actorId);
            if (dynamicBlock == null)
            {
                retPkg.Write((sbyte)-2);
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }

            if (dynamicBlock.DynamicBlockData == null)
            {
                retPkg.Write((sbyte)-3);
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }

            retPkg.Write((sbyte)1);
            retPkg.Write(dynamicBlock.DynamicBlockData.IsBlock);
            retPkg.DoReturnPlanes2Client(fwd);
        }

        /// <summary>
        /// 设置客户端场景对象属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <param name="targetValue"></param>
        public void SetClientSceneActorProperty(Guid id, string propertyName, string targetValue)
        {
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            CCore.H_CommonRPC.smInstance.RPC_SetSceneActorProperty(pkg, id, propertyName, targetValue);
            pkg.DoCommandPlanes2Client(Planes2GateConnect, ClientLinkId);
        }
    }
}
