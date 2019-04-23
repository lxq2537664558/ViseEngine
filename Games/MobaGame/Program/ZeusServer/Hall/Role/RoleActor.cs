using Server.Hall.Map;
using System;
using System.Collections.Generic;
using System.Text;
using GameData.Skill;
using GameData.Role;

namespace Server.Hall.Role
{
    [RPC.RPCClassAttribute(typeof(RoleActor))]
    public partial class RoleActor : CSUtility.Component.ActorBase, RPC.RPCObject, CSUtility.AISystem.IStateHost
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion

        [CSUtility.Event.Attribute.AllowMember("角色对象.所属地图", CSUtility.Helper.enCSType.Server, "角色所属地图")]
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.所属地图", CSUtility.Helper.enCSType.Server, "角色所属地图")]
        public Map.MapInstance HostMap { get; set; } = Map.NullMapInstance.Instance;
        public Map.MapCell Cell { get; set; } = null;

        public virtual string RoleName { get; set; } = "NoName";


        [CSUtility.Event.Attribute.AllowMember("角色对象.位置", CSUtility.Helper.enCSType.Server, "角色位置")]
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.位置", CSUtility.Helper.enCSType.Server, "角色位置")]
        public virtual SlimDX.Vector3 Location
        {
            get { return this.Placement.GetLocation(); }
        }

        #region ID管理
        Guid mRoleId;
        public override Guid Id
        {
            get { return mRoleId; }
        }
        public override void _SetId(Guid id)
        {
            mRoleId = id;
        }
        #endregion

        public virtual RoleTemplate RoleTemplate
        {
            get { return null; }
        }

        #region Guid到UInt32的一个映射
        UInt32 mSingleId = 0;
        public UInt32 SingleId
        {
            get { return mSingleId; }
        }
        public void _SetSingleId(UInt32 sid)
        {
            mSingleId = sid;
        }
        #endregion

        #region 组件管理
        Dictionary<System.Type, RoleComp> mComps = new Dictionary<Type, RoleComp>();
        public void RegComponent<CT>(CT comp) where CT : RoleComp
        {
            mComps[comp.GetType()] = comp;
            comp.Host = this;
        }
        public void UnregComponet<CT>() where CT : RoleComp
        {
            mComps.Remove(typeof(CT));
        }
        public CT GetComponent<CT>() where CT : RoleComp
        {
            RoleComp comp;
            if(mComps.TryGetValue(typeof(CT), out comp)== false)
                return null;
            return (CT)comp;
        }
        #endregion

        public long GetCurFrameTickCount()
        {
            return CSUtility.Helper.LogicTimer.GetTickCount();
//             if (HostMap == null || HostMap.IsNullMap)
//                 return IServer.Instance.GetElapseMilliSecondTime();
//             return HostMap.CurFrameTickCount;
        }

        #region StateHost
        ulong mCurFSMVersion;
        CSUtility.AISystem.FStateMachine mFSM = new CSUtility.AISystem.FStateMachine();
        public bool InitFSM(Guid fsmId, bool bResetCurrentState)
        {
            var tpl = CSUtility.AISystem.FStateMachineTemplateManager.Instance.GetFSMTemplate(fsmId, CSUtility.Helper.enCSType.Server);
            if (tpl == null)
                return false;
            mFSM.InitFSM(this, tpl, CSUtility.Helper.enCSType.Server);
            mCurFSMVersion = tpl.Version;

            if (bResetCurrentState)
                mCurrentState = mFSM.DefaultState;//mFSM.GetState("Idle");
            if (mCurrentState == null)
            {
                Log.FileLog.WriteLine(string.Format("FSM {0} DefaultState is null!", fsmId));
                return false;
                //          mCurrentState = new FrameSet.ServerStates.Idle();
            }
            return true;
        }

        bool mIsEnterHotSpring = false;
        public bool IsEnterHotSpring
        {
            get { return mIsEnterHotSpring; }
            set { mIsEnterHotSpring = value; }
        }

        #region 状态机
        public CSUtility.AISystem.FStateMachine AIStates
        {
            get { return mFSM; }
        }
        CSUtility.AISystem.State mCurrentState;
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.属性.当前状态", CSUtility.Helper.enCSType.Common, "获取或设置角色对象的当前状态")]
        [CSUtility.Event.Attribute.AllowMember("角色对象.属性.当前状态", CSUtility.Helper.enCSType.Common, "获取或设置角色对象的当前状态")]
        public CSUtility.AISystem.State CurrentState
        {
            get { return mCurrentState; }
            set
            {
                if (value == null)
                {
                    System.Diagnostics.Debugger.Break();
                }
                mCurrentState = value;
            }
        }
        CSUtility.AISystem.State mTargetState;
        public CSUtility.AISystem.State TargetState
        {
            get { return mTargetState; }
            set
            {
                mTargetState = value;
            }
        }
        public CSUtility.Component.ActorBase Actor
        {
            get
            {
                return this;
            }
        }
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.StateNotify2Remote", CSUtility.Helper.enCSType.Server, "StateNotify2Remote")]
        public virtual bool StateNotify2Remote { get; set; } = true;
        public virtual void OnExitedState(CSUtility.AISystem.State curState)
        {

        }

        public void FSMOnToState(CSUtility.AISystem.State curState, CSUtility.AISystem.StateParameter param, CSUtility.AISystem.State newCurState, CSUtility.AISystem.State newTarState)
        {
            RPC.DataWriter dwParam = new RPC.DataWriter();
            dwParam.Write(param, true);
            RPC.PackageWriter pkg = new RPC.PackageWriter();

            Client.H_GameRPC.smInstance.HIndex(pkg,this.SingleId).RPC_FSMChangeState(pkg, curState.StateName, newCurState.StateName, newTarState != null ? newTarState.StateName : "", dwParam);
            if (this.StateNotify2Remote)
            {
                HostMap.SendPkg2Clients(null, Placement.GetLocation(), pkg);
            }
            else
            {
                HostMap.SendPkg2Clients(this.OwnerRole, Placement.GetLocation(), pkg);
            }
        }
        public void FSMSetAction(string name, bool bLoop, float playRate, int blendDuration)
        {
            CSUtility.Animation.AnimationTree anim = FSMGetAnimationTreeByActionName(name, 0);

            if (anim != null)
            {
                anim.SetLoop(bLoop);
                anim.SetPlayRate(playRate);
                anim.CurNotifyTime = 0;
            }

            FSMSetCurrentAnimationTree(anim);
        }
        CSUtility.Animation.AnimationTree mCurrentAnimation = null;
        CSUtility.Animation.AnimationTree mIdleAnim = null;
        CSUtility.Animation.AnimationTree mWalkAnim = null;
        CSUtility.Animation.AnimationTree mDeathAnim = null;
        CSUtility.Animation.AnimationTree mBeAttackAnim = null;
        CSUtility.Animation.AnimationTree mMoveAttackAnim = null;
        CSUtility.Animation.AnimationTree mStayAttackAnim = null;
        CSUtility.Animation.AnimationTree mStayChannelAnim = null;
        CSUtility.Animation.AnimationTree mLostControlAnim = null;
        public Int64 GetCurrentAnimationTime()
        {
            return mCurrentAnimation.Action.Duration;
        }
        public void FSMSetBlendAction(string lowHalf, string highHalf)
        {
            List<CSUtility.Animation.AnimationTree> anims = new List<CSUtility.Animation.AnimationTree>{
                FSMGetAnimationTreeByActionName(lowHalf, 0),
                FSMGetAnimationTreeByActionName(highHalf, 0),
            };
            //mBlendActionAnim.SetAnimations(anims);

            FSMSetCurrentAnimationTree(null);
        }
        public CSUtility.Animation.ActionNode LoadAnimTree(string name)
        {
            if (RoleTemplate == null)
            {
                Log.FileLog.WriteLine("Role {0} LoadAnimTree {1} Failed", this.RoleName, name);
                return null;
            }
            var action = RoleTemplate.GetActionNamePair(name);
            if (action == null)
            {
                return null;
            }

            var anim_action = new CSUtility.Animation.ActionNode();
            //anim_action.Initialize();
            anim_action.ActionName =action.ActionFile;
            anim_action.PlayRate = 1.0F;//action.PlayRate;
            anim_action.PlayerMode = CSUtility.Animation.EActionPlayerMode.Default;
            anim_action.XRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
            anim_action.YRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
            anim_action.ZRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
            anim_action.CurNotifyTime = 0;

            return anim_action;
        }

        Dictionary<string, CSUtility.Animation.ActionNode> anims = new Dictionary<string, CSUtility.Animation.ActionNode>();

        public CSUtility.Animation.AnimationTree FSMGetAnimationTreeByActionName(string name, int blendDuration)
        {
            if (anims.ContainsKey(name))
            {
                var act = anims[name];
                if (act !=null)
                    return  act;
            }

            CSUtility.Animation.ActionNode anim = null;
            //这里暂时可以用switch，以后可以用Dictionary
            switch (name)
            {
                case "Idle":
                    if (mIdleAnim == null)
                        mIdleAnim = LoadAnimTree("Idle");
                    return mIdleAnim;
                case "Walk":
                    if (mWalkAnim == null)
                        mWalkAnim = LoadAnimTree("Walk");
                    return mWalkAnim;
                case "Death":
                    if (mDeathAnim == null)
                        mDeathAnim = LoadAnimTree("Death");
                    return mDeathAnim;
                case "BeAttack":
                    if (mBeAttackAnim == null)
                        mBeAttackAnim = LoadAnimTree("BeAttack");
                    return mBeAttackAnim;
                case "MoveAttack":
                    if (mMoveAttackAnim == null)
                        mMoveAttackAnim = LoadAnimTree("MoveAttack");
                    return mMoveAttackAnim;
                case "StayAttack":
                    if (mStayAttackAnim == null)
                        mStayAttackAnim = LoadAnimTree("StayAttack");
                    return mStayAttackAnim;
                case "StayChannel":
                    if (mStayChannelAnim == null)
                        mStayChannelAnim = LoadAnimTree("StayChannel");
                    return mStayChannelAnim;
                case "LostControl":
                    if (mLostControlAnim == null)
                        mLostControlAnim = LoadAnimTree("LostControl");
                    return mLostControlAnim;
                default:
                    break;
            }

            if (anim == null )//&& RoleTemplate != null)
            {
                anim =LoadAnimTree(name);
                //var action = RoleTemplate.GetActionNamePair(name);
                //if (action == null)
                //{
                //    return null;
                //}
                ////这里用 action.ActionFile 去设置动作文件
                //var anim_action = new CSUtility.Animation.ActionNode();
                ////anim_action.Initialize();
                //anim_action.ActionName = action.ActionFile;
                //anim_action.PlayRate = action.PlayRate;
                //anim_action.PlayerMode = CSUtility.Animation.EActionPlayerMode.Default;
                //anim_action.XRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
                //anim_action.YRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
                //anim_action.ZRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
                //anim_action.CurNotifyTime = 0;
                ////anim_action.DelegateOnActionFinish = 
                //anim = anim_action;
                //anim.CurNotifyTime = 0;
            }
            anims.Add(name, anim);
            return anim;
        }

        public CSUtility.Animation.AnimationTree FSMGetCurrentAnimationTree()
        {
            return mCurrentAnimation;
        }

        public void FSMSetCurrentAnimationTree(CSUtility.Animation.AnimationTree anim)
        {
            mCurrentAnimation = anim;
        }

        public CSUtility.Animation.AnimationTree CreateAnimationNode()
        {
            return new CSUtility.Animation.AnimNode();
        }

        public CSUtility.Animation.BaseAction CreateBaseAction()
        {
            return new CSUtility.Animation.ActionNode();
        }

        public void SetAnimTree(CSUtility.Animation.AnimationTree animtree)
        {
            mCurrentAnimation = animtree;
        }

        CSUtility.Helper.LogicTimerManager mTimerManager = new CSUtility.Helper.LogicTimerManager();
        public CSUtility.Helper.LogicTimerManager TimerManager
        {
            get
            {
                return mTimerManager;
            }
        }

        [CSUtility.AISystem.Attribute.AllowMember("角色对象.PushStateExitCallee", CSUtility.Helper.enCSType.Server, "")]
        public void PushStateExitCallee(Guid cbId)
        {
            var cb = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(CSUtility.AISystem.FOnStateExit), cbId);
            if (cb != null)
                PushStateExit(cb);
        }

        public void PushStateExit(CSUtility.Helper.EventCallBack cb)
        {
            mOnStateExitQueue.Enqueue(cb);
        }

        Queue<CSUtility.Helper.EventCallBack> mOnStateExitQueue = new Queue<CSUtility.Helper.EventCallBack>();
        public CSUtility.Helper.EventCallBack PopStateExit()
        {
            if (mOnStateExitQueue.Count == 0)
                return null;
            return mOnStateExitQueue.Dequeue();
        }

        #endregion
 
        public virtual bool OnValueChanged(string name, RPC.DataWriter value)
        {
            switch (name)
            {
                case "RoleHp":
                case "MaxRoleHp":
                case "RoleMp":
                case "MaxRoleMp":
                case "RoleLevel":
                case "Unrivaled":
                case "RoleMoveSpeed":
                    if (!HostMap.IsNullMap)
                    {
                        var pkg = new RPC.PackageWriter();
                        Client.H_GameRPC.smInstance.HIndex(pkg, this.SingleId).RPC_UpdateRoleValue(pkg, name, value);
                        HostMap.SendPkg2Clients(null, Placement.GetLocation(), pkg);
                    }
                    return true;
            }
            return false;
        }

        public float GetAltitude(float x, float z)
        {
            return this.HostMap.GetAltitude(x, z);
        }

        public virtual void OnEnterTrigger(Guid triggerid)
        {
            if (CurrentState != null)
                CurrentState.OnEnterTrigger();
        }

        public virtual void OnLeaveTrigger(Guid triggerid)
        {
            if (CurrentState != null)
                CurrentState.OnLeaveTrigger();
        }

        public virtual void OnEnterState(string name)
        {

        }

        public void SetRigidTracker(Guid tracker, Int64 duration, bool isAbsTracker)
        {
            var rolePlace = mPlacement as CSUtility.Component.TrackerPlacement;
            if (rolePlace == null)
                return;
            if (tracker == Guid.Empty)
            {
                rolePlace.Tracker = null;
            }
            else
            {
                //从文件里面加载tracker并且设置进去
                rolePlace.Tracker = new CSUtility.Component.RigidTracker();
                rolePlace.Tracker.SetTracker(this.HostMap.MapTemplate.MapGuid, tracker, duration, isAbsTracker);
            }
        }
        #endregion

        #region 技能系统
        List<SkillData> mSkills = new List<SkillData>();
        public virtual List<SkillData> Skills
        {
            get { return mSkills; }
            set { mSkills = value; }
        }
        public virtual SkillData FindSkillData(UInt16 templateId)
         {
            foreach(var data in Skills)
            {
                if (data.TemplateId == templateId)
                    return data;
            }
            var da = new SkillData();
            da.TemplateId = templateId;
            Skills.Add(da);
            return da;
        }
//         public virtual CSUtility.Data.Skill.RuneData FindRuneData(UInt16 templateId)
//         {
// 
//             return null;
//         }

//         public virtual CSUtility.Data.Skill.BuffData FindBuffData(UInt16 templateId)
//         {
//             return null;
//         }

//         SlimDX.Vector3 mJumpPos = SlimDX.Vector3.Zero;
//         //跳跃
//         public void OnUpDateJumpUpNotify(CSUtility.Data.Skill.SkillTemplate skillTemplate, CSUtility.Data.Skill.RuneData rune, Int32 dura, bool firstTime, SlimDX.Vector3 startPos, long startTime, SlimDX.Vector3 tarRolePos)
//         {
//             if (startPos == SlimDX.Vector3.Zero)
//                 return;
//             if (skillTemplate == null || rune.Template == null)
//                 return;
//             if (rune.Template.OffsetType != CSUtility.Data.Skill.EOffsetType.JumpUp)
//                 return;
//             var time = GetCurFrameTickCount() + HostMap.ElapsedMillisecond;
// 
//             var Jduraiton = time - startTime;
// 
//             if (Jduraiton > dura)//时间判定
//             {
//                 return;
//             }
//             if (firstTime == true || mJumpPos == SlimDX.Vector3.Zero)
//             {
//                 var distance = rune.Template.GetRuneLevelParam(rune.RuneLevel).OffsetDistance;//.RuneLevelParams[attackParam.RuneLevel].OffsetDistance;
//                 if (distance == 0)
//                     distance = 9;
//                 var mdir = tarRolePos - Placement.GetLocation();
//                 mdir.Y = 0;
//                 mdir.Normalize();
// 
//                 mJumpPos = FindTargetPos(this, tarRolePos, mdir);//计算逻辑目标位置
//                 var prejumpDist = mJumpPos - startPos;
//                 prejumpDist.Y = 0;
//                 if (distance < prejumpDist.Length())//计算是否可以直接跳到
//                 {
//                     mJumpPos = startPos + mdir * distance;
//                 }
//                 else
//                 {
//                     if (this.RoleCreateType == CSUtility.Data.ERoleCreateType.Npc)//npc的话去除半径
//                         mJumpPos -= mdir * (RoleTemplate.Radius + 1);
//                 }
//             }
//             if (mJumpPos == SlimDX.Vector3.Zero)
//             {
//                 return;
//             }
// 
//             var playerPos = Placement.GetLocation();
//             playerPos.Y = 0;
//             startPos.Y = 0;
//             mJumpPos.Y = 0;
//             var jumDistance = (playerPos - startPos).Length();//已经跳过距离
//             var totalLenth = (mJumpPos - startPos).Length();
//             var tarDistance = totalLenth * (float)Jduraiton / (float)dura;//这次之后应该要跳过的总距离
//             if (tarDistance > totalLenth + 1)
//                 return;
//             //             float offsetDistance = tarDistance - jumDistance;
//             //             if (offsetDistance <= 0)
//             //                 return;
// 
//             var jumpDir = mJumpPos - startPos;
//             var dir = new SlimDX.Vector3(jumpDir.X, 0, jumpDir.Z);
//             dir.Normalize();
// 
//             var targetPos = startPos + dir * tarDistance;// playerPos + dir * offsetDistance;
//             var remaintime = dura - Jduraiton;
//             float jumY = (float)((float)remaintime / (float)dura * (float)(Jduraiton) / (float)dura) * 3 * rune.Template.JumpUpHeight;
//             var resultY = GetAltitude(mJumpPos.X, mJumpPos.Z) / 2 + Placement.GetLocation().Y / 2 + jumY;
// 
//             SlimDX.Vector3 resultPos = new SlimDX.Vector3(targetPos.X, resultY, targetPos.Z);
//             //     Log.FileLog.WriteLine(String.Format("mJumpTime_{0},distance_{1},totaldist_{2}", Jduraiton,tarDistance,totalLenth));
//             Placement.SetLocation(ref resultPos);
//         }
// 
//         //冲刺
//         public virtual void OnTickSpurtNotify(ref bool collect, CSUtility.Data.Skill.SkillTemplate skillTemplate, Int32 dura, bool firstTime, CSUtility.Data.Skill.RuneData rune, SlimDX.Vector3 startPos, long startTime, SlimDX.Vector3 tarRolePos)
//         {
//             if (startPos == SlimDX.Vector3.Zero)
//                 return;
//             if (skillTemplate == null || rune == null)
//                 return;
//             if (rune.Template.OffsetType != CSUtility.Data.Skill.EOffsetType.Spurt)
//                 return;
//             var time = GetCurFrameTickCount() + HostMap.ElapsedMillisecond;
//             var Jduraiton = HostMap.ElapsedMillisecond;
//             var offsetMaxDistance = rune.Template.GetRuneLevelParam(rune.RuneLevel).OffsetDistance;//技能释放距离
//             if (offsetMaxDistance == 0)
//             {
//                 offsetMaxDistance = 9;
//             }
//             float speed = offsetMaxDistance / (float)dura;
//             float offsetDistance = speed * (time - startTime);
//             if (offsetDistance > offsetMaxDistance)
//             {
//                 collect = true;
//                 return;
//             }
//             var playerPos = new SlimDX.Vector3(Placement.GetLocation().X, 0, Placement.GetLocation().Z);
//             var dir = tarRolePos - playerPos;
//             dir.Y = 0;
//             dir.Normalize();
//             if (firstTime)
//             {
//                 SlimDX.Vector3 jumpPos = SlimDX.Vector3.Zero;
//                 jumpPos = startPos + dir * offsetMaxDistance;
//                 mJumpPos = FindTargetPos(this, tarRolePos, dir);
//             }
//             startPos.Y = 0;
//             mJumpPos.Y = 0;
//             var totalDist = SlimDX.Vector3.Distance(startPos, mJumpPos);//是否到了冲到的地方
//             if (offsetDistance >= totalDist)
//             {
//                 collect = true;
//                 return;
//             }
// 
//             var targetPos = startPos + dir * offsetDistance;
//             var resultY = GetAltitude(targetPos.X, targetPos.Z);
//             targetPos.Y = resultY;
// 
//             if (OnSpurtCollideRoles(rune.Template, tarRolePos))
//             {
//                 collect = true;
//                 return;
//             }
//             Placement.SetLocation(ref targetPos);
//             // Log.FileLog.WriteLine(String.Format("mJumpTime_{0},curDist_{1},totalDist_{2}", Jduraiton, offsetDistance, totalDist));
//         }
// 
//         bool OnSpurtCollideRoles(CSUtility.Data.Skill.RuneTemplate runeTemplate, SlimDX.Vector3 tarPos)
//         {
//             var pos = Placement.GetLocation();
//             var targetRoles = FindRolesNearBy(RoleTemplate.Radius);
//             var CollideRoles = SpurtCollideWithRoles(targetRoles, runeTemplate, tarPos);
//             if (CollideRoles.Count > 0)
//             {
//                 return true;
//             }
//             return false;
//         }
// 
//         public List<RoleActor> SpurtCollideWithRoles(List<RoleActor> roles, CSUtility.Data.Skill.RuneTemplate runeTemplate, SlimDX.Vector3 tarPos)
//         {
//             var mCollideRoles = new List<RoleActor>();
// 
//             foreach (RoleActor role in roles)
//             {
//                 if (!CanAttack(role))
//                 {
//                     continue;
//                 }
//                 if (role.CurrentState.StateName == "Death")
//                     continue;
//                 if (role.SingleId == SingleId)
//                 {
//                     continue;
//                 }
//                 if (role.OwnerRole != null && SingleId == role.OwnerRole.SingleId)
//                     continue;
// 
//                 var distance = Placement.GetLocation() - tarPos;
//                 distance.Y = 0;
//                 if (distance.Length() < runeTemplate.Radius + role.RoleTemplate.Radius)
//                 {
//                     mCollideRoles.Add(role);
//                 }
//             }
//             return mCollideRoles;
//         }
// 
//         public static SlimDX.Vector3 FindTargetPos(ServerCommon.Planes.Role.RoleActor role, SlimDX.Vector3 tarpos, SlimDX.Vector3 dir)
//         {
//             SlimDX.Vector3 findPoint;
//             var playerPos = role.Placement.GetLocation();
//             var pathPoints = new List<SlimDX.Vector2>();
//             var pkg = new RPC.PackageWriter();
//             tarpos.Y = role.GetAltitude(tarpos.X, tarpos.Z);
//             // var tarDistance = SlimDX.Vector3.Distance(playerPos, tarpos);
//             if (role.HostMap.MapSourceId == Guid.Empty)
//                 findPoint = playerPos;
//             else
//             {
//                 var navData = ServerCommon.Planes.Map.MapPathManager.Instance.GetGlobalMapNavigationAssistData(role.HostMap.MapSourceId);
//                 if (navData == null)
//                     return SlimDX.Vector3.Zero;
// 
//                 var navresult = role.HostMap.NavigationWrapper.GetFarthestPathPointFromStartInLine(role.HostMap.MapInstanceId, playerPos.X, playerPos.Z, tarpos.X, tarpos.Z, out findPoint.X, out findPoint.Z, navData.NavigationTileData);
//                 if (navresult)
//                 {
//                     findPoint.Y = role.HostMap.GetAltitude(findPoint.X, findPoint.Z);
//                 }
//                 else
//                 {
//                     findPoint = playerPos;
//                 }
//             }
// 
//             return findPoint;
//         }

        #endregion

        protected void InitActor()
        {
            mPlacement = new CSUtility.Component.StandardPlacement(this);
            Placement.OnLocationChanged -= this._OnLocationChanged;
            Placement.OnLocationChanged += this._OnLocationChanged;
            Placement.OnRotationChanged -= this._OnRotationChanged;
            Placement.OnRotationChanged += this._OnRotationChanged;

            var bag = new Bag.BagBase();
            this.RegComponent(bag);
            var bag1 = this.GetComponent<Bag.BagBase>();
        }

        protected void InitCylinderShape()
        {
            var cylinder = new CSUtility.Component.IShapeCylinder();

            //             cylinder.CamHeight = pd.PlayerDetail.Template.CameraPointHeight;
            cylinder.HalfHeight = RoleTemplate.HalfHeight;
            cylinder.Radius = RoleTemplate.Radius;
            this.mShape = cylinder;
        }

        #region 代理回掉
        private void _OnLocationChanged(ref SlimDX.Vector3 loc)
        {
            this.OnLocationChanged(ref loc);
        }
        private void _OnRotationChanged(ref SlimDX.Quaternion rot)
        {
            this.OnRotationChanged(ref rot);
        }
        #endregion
        protected virtual void OnLocationChanged(ref SlimDX.Vector3 loc)
        {

        }
        protected virtual void OnRotationChanged(ref SlimDX.Quaternion rot)
        {

        }

        protected int mRemainUpdateTime = 3000;
        public SlimDX.Vector3 mBlockPos = SlimDX.Vector3.Zero;
        public override void Tick(Int64 elapseMillsecond)
        {
            mRemainUpdateTime -= (int)elapseMillsecond;
            if (mRemainUpdateTime < 0)//3秒钟必然同步一次
            {
                //判断是否在其他role里，并做处理
             //   RoleInBlock();

                mRemainUpdateTime = 3000;
                if (this.CurrentState != null && this.CurrentState.StateName != "Death")
                {
                    var loc = Placement.GetLocation();
                    Placement.SetLocation(ref loc);
                }                
            }
            
            var templateFSM = mFSM.FSMTemplate;
            if (templateFSM != null)
            {
                if (mCurFSMVersion != templateFSM.Version)
                {                    
                    InitFSM(templateFSM.Id, true);
                }
            }

            TimerManager.Tick(elapseMillsecond);
            //HatredManager.Tick(elapseMillsecond);
            BuffBag.Tick(elapseMillsecond);
            TickCollision();
            TickSkillCD(elapseMillsecond);
            TickBloodReturn(elapseMillsecond);
            if (mCurrentState != null)
                mCurrentState.Tick(elapseMillsecond);
            try
            {                
                for (int i = 0; i < mSummons.Count; i++)
                {
                    if (mSummons[i].IsLeaveMap)
                    {
                        mSummons.RemoveAt(i);
                        i--;
                    }
                }
                foreach (var i in mComps)
                {
                    i.Value.Tick(elapseMillsecond);
                }
            }
            catch(Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
            }
        }


        #region 所属地图管理

        HallInstance mHallInstance;
        public HallInstance HallInstance
        {
            get { return mHallInstance; }
            set { mHallInstance = value; }
        }

        bool mIsLeaveMap = false;
        public virtual bool IsLeaveMap
        {
            get { return mIsLeaveMap; }
        }

        public void DoLeaveMap()
        {    
            mIsLeaveMap = true;
        }

        public virtual void OnEnterMap(MapInstance map)
        {
            HostMap = map;
            mIsLeaveMap = false;

            var loc = this.Placement.GetLocation();
            var mapCell_ = HostMap.GetMapCell(loc.X, loc.Z);
            mapCell_.Enter(this);

            if (this.CurrentState != null)
            {
                this.CurrentState.OnEnterMap(map.MapSourceId);
            }
        }

        public virtual void DangrouseOnLeaveMap()
        {
            // 广播给客户端
            HostMap.BroadcastRoleLeave(this);
            //从MapCell里面清除
            HostMap.RemoveRoleActor(this);
            //var loc = this.Placement.GetLocation();
            //var mapCell = mHostMap.GetMapCell(loc.X, loc.Z);
            //mapCell.RoleLeaveCell(this);

            //ProcessLeaveMap_Trigger();

            HostMap = NullMapInstance.Instance;

            //自己的召唤物(火球等)离开地图
            foreach (var i in mSummons)
            {
                //从地图里清除
                //i.OnLeaveMap();
                i.DoLeaveMap();
            }
            mSummons.Clear();
        }

//         public void SetHostMapDataStep(byte step, bool value)
//         {
//             this.HostMap.InstanceData.SetStep(step, value);
//         }
// 
//         public bool IsHostMapStep(byte step)
//         {
//             return this.HostMap.InstanceData.IsSetStep(step);
//         }
        #endregion

        #region 底层抽象

        int mRoleLevel;
        public virtual int RoleLevel
        {
            get { return mRoleLevel; }
            set { mRoleLevel = value; }
        }

        int mRoleStrength = 0;
        public virtual int RoleStrength
        {
            get { return mRoleStrength; }
            set { RoleStrength = value; }
        }

        int mRoleIntellect;
        public virtual int RoleIntellect
        {
            get { return mRoleIntellect; }
            set { mRoleIntellect = value; }
        }

        int mRoleSkillful;
        public virtual int RoleSkillful
        {
            get { return mRoleSkillful; }
            set { mRoleSkillful = value; }
        }

        int mRoleTenacity;
        public virtual int RoleTenacity
        {
            get { return mRoleTenacity; }
            set { mRoleTenacity = value; }
        }

        int mRolePhysical;
        public virtual int RolePhysical
        {
            get { return mRolePhysical; }
            set { mRolePhysical = value; }
        }

        GameData.Role.ERoleType mRoleCreateType = GameData.Role.ERoleType.Player;
        public GameData.Role.ERoleType RoleCreateType
        {
            get { return mRoleCreateType; }
            set { mRoleCreateType = value; }
        }

        static System.Random smRandom = new System.Random((int)IServer.Instance.GetElapseMilliSecondTime());
        public static float GetRandomUnit()
        {
            return (float)smRandom.NextDouble();
        }
        public static System.Random Random
        {
            get { return smRandom; }
        }
        [CSUtility.AISystem.Attribute.AllowMember("当前血量", CSUtility.Helper.enCSType.Server, "当前血量")]
        public virtual int RoleHP
        {
            get;
            set;
        }
        public virtual int RoleMP
        {
            get;
            set;
        }
        int mRoleMaxHp = 10;
        [CSUtility.AISystem.Attribute.AllowMember("最大血量", CSUtility.Helper.enCSType.Server, "最大血量")]
        public virtual int RoleMaxHP
        {
            get { return mRoleMaxHp; }
            set{ mRoleMaxHp = value; }
        }
        int mRoleMaxMp = 10;
        public virtual int RoleMaxMP
        {
            get { return mRoleMaxMp; }
            set { mRoleMaxMp = value; }
        }

        int mRoleDex = 1;
        public virtual int RoleDex
        {
            get
            {
                return mRoleDex;
            }
            set
            {
                mRoleDex = value;
                RPC.PackageWriter pkg = new RPC.PackageWriter();
                //ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, this.SingleId).RPC_UpdateRoleDex(pkg, value);

            }
        }
        public virtual int FactionId
        {
            get { return this.RoleTemplate.FactionId; }//玩家为0号阵营
        }
        //角色是否会进仇恨列表
        public virtual bool HasHatred
        {
            get
            {
                return true;
            }
        }
        public virtual bool StopMoveOnBlock
        {
            get { return false; }
        }
        public float PlacementX
        {
            get { return this.Placement.GetLocation().X; }
        }
        public float PlacementZ
        {
            get { return this.Placement.GetLocation().Z; }
        }
        public float PlacementY
        {
            get { return this.Placement.GetLocation().Y; }
        }
        public bool IsPlayerInstance()
        {
            if (RoleCreateType == GameData.Role.ERoleType.Player)
                return true;
            return false;
        }

        public bool IsNPCInstance()
        {
            if (RoleCreateType == GameData.Role.ERoleType.Npc)
                return true;
            return false;
        }
        
        public bool IsDropInstance()
        {
            if (RoleCreateType == GameData.Role.ERoleType.Item)
                return true;
            return false;
        }
        public bool IsGatherRole()
        {
            if (RoleCreateType == GameData.Role.ERoleType.Gather)
                return true;
            return false;
        }
        public bool IsSummon()
        {
            if (RoleCreateType == GameData.Role.ERoleType.Summon)
                return true;
            return false;
        }
        public bool IsTrigger()
        {
            if (RoleCreateType == GameData.Role.ERoleType.Trigger)
                return true;
            return false;
        }
        public virtual SlimDX.Vector3 BirthPosition
        {
            get;
            set;
        }
   //     float mSpeedRate = 1.0F;
        public float SpeedRate
        {
            get
            {
                var player = this as Player.PlayerInstance;
                if (player != null)
                {
                    var rate = player.PlayerData.RoleMoveSpeed / player.PlayerData.RoleTemplate.MoveSpeed;
                    return rate;
                }
                
                var monster = this as Monster.MonsterInstance;
                if (monster != null)
                {
                    var rate = monster.MonsterData.RoleMoveSpeed / monster.MonsterData.RoleTemplate.MoveSpeed;
                    return rate;
                }
                return 1.0f;
            }
        }

        SByte mIsRun = 0;
        public SByte IsRun
        {
            get { return mIsRun; }
            set { mIsRun = value; }
        }

        //public virtual float GetMoveSpeed(SByte run)
        //{
        //    if (run == 1)
        //        return this.RoleTemplate.RunMoveSpeed * mSpeedRate;
        //    else
        //        return this.RoleTemplate.MoveSpeed * mSpeedRate;
        //}

        public float MoveSpeed
        {
            get
            {
                var player = this as Player.PlayerInstance;
                if (player != null)
                    return player.PlayerData.RoleMoveSpeed;
                var monster = this as Monster.MonsterInstance;
                if (monster != null)
                    return monster.MonsterData.RoleMoveSpeed ;
                var summon = this as Summon.SummonRole;
                if (summon != null)
                    return summon.SummonData.RoleTemplate.MoveSpeed;
                return 0;
            }
        }
        //         public virtual float WanderLength
        //         {
        //             get { return this.RoleTemplate.WanderRadius; }
        //         }
        //         public virtual float LockOnRadius
        //         {
        //             get { return this.RoleTemplate.LockOnRadius; }
        //         }

        int mPreWanderAI = 30;
        public virtual int PreWanderAI
        {
            get { return mPreWanderAI; }
            set
            {
                mPreWanderAI = value;
            }
        }

        Int64 mDeathDelegateTimer = 0;//死亡后的代理调用时用的
        public Int64 DeathDelegateTimer
        {
            get { return mDeathDelegateTimer; }
            set { mDeathDelegateTimer = value; }
        }

        Role.Buff.BuffBag mBuffBag = null;
        public Buff.BuffBag BuffBag
        {
            get
            {
                if(mBuffBag ==null)
                    mBuffBag = new Buff.BuffBag(this);
                return mBuffBag;
            }
            set { mBuffBag = value; }
        }

        #endregion

        #region 怪物关系
        RoleActor mFatherRole = null;
        public virtual RoleActor FatherRole
        {
            get { return mFatherRole; }
            set { mFatherRole = value; }
        }
        public List<RoleActor> mSonRole = new List<RoleActor>();

        string mFallowedRole;
        public virtual string FallowedRole
        {
            get { return mFallowedRole; }
            set { mFallowedRole = value; }
        }

        public virtual RoleActor FindTargetTemplateRole()
        {
            return null;
        }

        [CSUtility.AISystem.Attribute.AllowMember("角色对象.召唤怪物", CSUtility.Helper.enCSType.Server, "召唤怪物")]
        public Role.Monster.MonsterInstance CreateMonster(ushort tempid,float posx,float posz)
        {
            var data = new MonsterData();
            data.TemplateId = tempid;
            data.RoleId = Guid.NewGuid();
            var pos = new SlimDX.Vector3(posx,0,posz);
            data.OriPosition = pos;
            var role = Role.Monster.MonsterInstance.CreateMonsterInstance(data,this.HallInstance,this.HostMap);
            return role;
        }
        #endregion

        #region 召唤生物管理
        public virtual RoleActor OwnerRole
        {
            get;
            set;
        }

        List<Summon.SummonRole> mSummons = new List<Role.Summon.SummonRole>();
        public List<Role.Summon.SummonRole> Summons
        {
            get { return mSummons; }
        }
        public void AddSummon(Role.Summon.SummonRole role)
        {
            foreach (var i in mSummons)
            {
                if (i == role)
                    return;
            }
            mSummons.Add(role);
        }
        public void RemoveSummon(Role.Summon.SummonRole role)
        {
            mSummons.Remove(role);
        }
        public Role.Summon.SummonRole FindSummon(UInt32 singleId)
        {
            foreach (var i in mSummons)
            {
                if (i.SingleId == singleId)
                {
                    return i;
                }
                else
                {
                    var result = i.FindSummon(singleId);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        #endregion

        #region 战斗处理
        public virtual bool DoFireSkill(UInt16 skillId, SlimDX.Vector3 tarDir, SlimDX.Vector3 tarPos,  float tarAngle, SlimDX.Vector3 summonPos, UInt32 tarRoleId,UInt16 runeLevel = 0)
        {
            var skill = FindSkillData(skillId);

            if (skill == null)
            {
                this.CurrentState.ToState("Idle", null);
                return false;
            }
            if (string.IsNullOrEmpty(skill.Template.ChannelAction))
            {
                var atk = AIStates.GetState("StayAttack") as ServerStates.StayAttack;
                if (atk == null)
                {
                    return false;
                }
                var info = AIStates.GetStateSwitchInfo(this.CurrentState.StateName, "StayAttack");
                if (info == null)
                    return false;
                atk.StayAttackParameter.SkillId = skillId;
                atk.StayAttackParameter.RuneLevel = runeLevel;
                atk.StayAttackParameter.RuneHandle = Random.Next(Int32.MaxValue);
                atk.StayAttackParameter.tarDir = tarDir;
                atk.StayAttackParameter.tarPos = tarPos;
                atk.StayAttackParameter.TarSingle = tarRoleId;
                atk.StayAttackParameter.TarAngle = tarAngle;
                atk.StayAttackParameter.SummonPos = summonPos;
                atk.StayAttackParameter.Duration = (Int64)skill.Template.FireSkillTime;
                this.CurrentState.ToState("StayAttack", atk.StayAttackParameter);

                if (skill.Template.SkillLevelDatas.Count >= skill.SkillLevel && skill.SkillLevel > 0)
                    skill.RemainCD = skill.Template.SkillLevelDatas[skill.SkillLevel - 1].CD;
            }
            else
            {
                var chn = AIStates.GetState("StayChannel") as ServerStates.StayChannel;
                if (chn == null)
                {
                    this.CurrentState.ToState("Idle", null);
                    return false;
                }
                var info = AIStates.GetStateSwitchInfo(CurrentState.StateName, "StayChannel");
                if (info == null)
                    return false;
                chn.StayChannelParameter.SkillId = skill.TemplateId;
                chn.StayChannelParameter.tarPos = tarPos;
                this.CurrentState.ToState("StayChannel", chn.StayChannelParameter);
            }
            return true;
        }

        public virtual void FreshRoleValue(bool freshhpmp)
        {
            var oldMaxHp = RoleMaxHP;
            var oldMaxMp = RoleMaxMP;

            RoleMaxHP = RoleTemplate.OiHp + RoleLevel * 8;
            RoleMaxMP = RoleTemplate.OiMp + RoleLevel * 2;

            if(freshhpmp ==true)
            {
                RoleHP = RoleMaxHP;
                RoleMP = RoleMaxMP;
            }
        }
        public virtual void GainExp(int exp)
        {

        }
        public virtual void GainMoney(int money)
        {

        }

        public virtual void AddBuff(BuffParam param,RoleActor target,BuffAddType addtype)//自己是释放者
        {
            var temp = CSUtility.Data.DataTemplateManager<UInt16, BuffTemplate>.Instance.GetDataTemplate(param.BuffId);// GameData.Skill.BuffTemplateManager.Instance.FindBuff(param.BuffId);
            if (temp == null)
                return;
            if (temp.BuffAdd != addtype)
                return;
 
            switch (temp.BuffState)
            {

                case BuffState.All:
                    {
                        BuffBag.CreateBuffAndAutoAdd2Bag(this, this, param);
                        if(target !=null)
                            target.BuffBag.CreateBuffAndAutoAdd2Bag(target, this, param);
                    }
                    break;
                case BuffState.Self:
                case BuffState.SelfAll:
                    {
                        BuffBag.CreateBuffAndAutoAdd2Bag(this, this, param);
                    }
                    break;
                case BuffState.Other:
                case BuffState.OtherAll:
                case BuffState.SummonRole:
                    {
                        if (target != null)
                            target.BuffBag.CreateBuffAndAutoAdd2Bag(target, this, param);
                    }
                    break;
            }                  
        }
// 
//         Fight.Hatred mHatredManager = null;
//         public Fight.Hatred HatredManager
//         {
//             get
//             {
//                 if (mHatredManager == null)
//                     mHatredManager = new Fight.Hatred(this);
//                 return mHatredManager;
//             }
//             set { mHatredManager = value; }
//         }

        //发现客户端作弊的时候调用此函数
        public virtual void OnClientCheat(int level, string reason)
        {

        }

        public virtual bool IsValidMoveSpeed(float speed, SByte run)
        {
            if (speed != MoveSpeed)
                return false;
            return true;
        }
        public virtual bool CanLockon(RoleActor target)
        {//可以主动选择作为攻击目标
            if (target == null)
                return false;
            var monster = target as Monster.MonsterInstance;
            if (monster != null && monster.MonsterData.Unrivaled)
            {
                return false;
            }
            if (target == this)
                return false;
            if (target.IsDeath())
                return false;
            if (target.RoleCreateType == ERoleType.Monster && target.RoleTemplate.MonsterType >= MonsterType.Symbol)
                return false;
            if (target == this.OwnerRole)//你不能杀死发射自己的人角色
                return false;
            if (FatherRole != null && target == this.FatherRole)
                return false;
            if (mSonRole != null)
            {
                foreach (var role in mSonRole)
                {
                    if (target.SingleId == role.SingleId)
                        return false;
                }
            }
            if (OwnerRole != null && target.OwnerRole != null)
            {
                if (target.OwnerRole.SingleId == OwnerRole.SingleId)
                    return false;
            }
            if (FactionId == target.FactionId)
                return false;
            return true;
        }
        
        public virtual bool CanAttack(RoleActor target)
        {
            if (target == null)
                return false;
            var monster = target as Monster.MonsterInstance;
            if (monster != null && monster.MonsterData.Unrivaled)
            {
                return false;
            }
            if (target.SingleId == this.SingleId)
                return false;
//             if (target.RoleAttackState.Contains(CSCommon.Data.EAttackState.SuperInvincible)) //目标超级无敌
//                 return false;
            if (target == this)
                return false;
            if (target.IsDeath())
                return false;
            if (target.RoleCreateType == ERoleType.Monster && target.RoleTemplate.MonsterType >= MonsterType.Symbol)
                return false;
            if (target == this.OwnerRole)//你不能杀死发射自己的人角色
                return false;
            if (FatherRole != null && target == this.FatherRole)
                return false;
            if (!target.GetRoleAttachFighting())
            {
                return false;
            }

            if (mSonRole != null)
            {
                foreach (var role in mSonRole)
                {
                    if (target.SingleId == role.SingleId)
                        return false;
                }
            }
            
            if (target.OwnerRole != null && OwnerRole != null)//同一个人放的两个火球也不能干架
            {
                if (target.OwnerRole.SingleId == SingleId)
                {
                    return false;
                }
                if (target.OwnerRole.SingleId == OwnerRole.SingleId)
                {
                    return false;
                }
            }

            if (FactionId == target.FactionId) //友军
                return false;

            return true;
        }

        public virtual bool GetRoleAttachFighting()
        {
            return true;
        }
        public virtual SlimDX.Vector3 GetWanderTarget()
        {
            return Placement.GetLocation();
        }

        Random rand = new Random();
        public virtual Role.RoleActor SelectAttackTarget()
        {//选择一个当前状态下最应该攻击的目标

            if (RoleCreateType == ERoleType.Monster && RoleTemplate.MonsterType >= MonsterType.Symbol)
                return null;

            RoleActor tar = null;
            var type = (uint)GameData.Role.ERoleType.Player | (uint)GameData.Role.ERoleType.Monster;
            var loc = Placement.GetLocation();
            List<RoleActor> sroundRoles = new List<RoleActor>();
            List<RoleActor> monsterRoles = new List<RoleActor>();
            RoleActor hpMinMonster = null;
            RoleActor nearestPlayer = null;
            //float lenth = float.MaxValue;
            HostMap.TourRoles(type, ref loc, (uint)RoleTemplate.LockOnRadius, (uint singleId, RoleActor role,object arg) =>
            {
                 if (this.SingleId == singleId || role.IsDeath() || !CanAttack(role))
                     return CSUtility.Support.EForEachResult.FER_Continue;
                 //                  var dir = role.Placement.GetLocation() - loc;
                 //                  dir.Y = 0;
                 //                  if (lenth > dir.Length())
                 //                  {
                 //                      lenth = dir.Length();
                 //                      tar = role;
                 //                  }
                 sroundRoles.Add(role);
                 if (role.RoleCreateType != ERoleType.Player)
                 {
                     if (hpMinMonster != null)
                     {
                         var lastRadio = ((float)hpMinMonster.RoleHP / (float)hpMinMonster.RoleMaxHP);
                         var nowRadio = ((float)role.RoleHP / (float)role.RoleMaxHP);
                         hpMinMonster = lastRadio > nowRadio ? role : hpMinMonster;
                     }
                     else
                     {
                         hpMinMonster = role;
                     }
                     monsterRoles.Add(role);
                 }
                 else
                 {
                     if (nearestPlayer != null)
                     {
                         var lenth = (nearestPlayer.Placement.GetLocation() - loc).Length();
                         var dir = role.Placement.GetLocation() - loc;
                         dir.Y = 0;
                         if (lenth > dir.Length())
                         {
                             lenth = dir.Length();
                             nearestPlayer = role;
                         }
                     }
                     else
                     {
                         nearestPlayer = role;
                     }
                 }
                 return CSUtility.Support.EForEachResult.FER_Continue;
             }, null);

            if (RoleCreateType == ERoleType.Monster)
            {
                if (RoleTemplate.MonsterType == MonsterType.Building)
                {
                    if (hpMinMonster != null)
                        tar = hpMinMonster;
                    else
                        tar = nearestPlayer;
                }
                else if (sroundRoles.Count > 0)
                {
                    tar = sroundRoles[rand.Next(sroundRoles.Count)];
                }
            }            
//             tar = HatredManager.GetFirstTarget();
//             if (FatherRole != null)
//             {
//                 var tarrole = FatherRole.HatredManager.GetFirstTarget();//父为主
//                 if (tarrole != null)
//                     tar = tarrole;
//             }
//             if (tar == null)
//             {
//                 foreach (var role in mSonRole)
//                 {
//                     if (role != null)
//                     {
//                         tar = role.HatredManager.GetFirstTarget();
//                     }
//                 }
//             }

//             if (tar != null)
//             {
//                 var dis = SlimDX.Vector3.Distance(this.Placement.GetLocation(), tar.Placement.GetLocation());
//                 var npc = this as Monster.MonsterInstance;
//                 if (npc != null)
//                 {
//                     //var wanderdis = SlimDX.Vector3.Distance(this.Placement.GetLocation(), npc.MonsterData.OriPosition);//超出巡逻范围了
//                     //if (wanderdis > npc.NPCData.Template.FallowUpRadius && npc.NPCData.WanderType == CSCommon.Data.ERoleWanderType.Common)
//                     //{
//                     ////    npc.CatchRunBack(npc.NPCData.OriPosition);
//                     //    return null;
//                     //}
//                 }
// 
//                 if (tar.RoleAttackState.Contains(CSCommon.Data.EAttackState.Stealth))//怪物是不会主动攻击隐身单位的
//                 {
//                     mHatredManager.RemoveHatred(tar);
//                     return null;
//                 }
//                 if (!CanAttack(tar))//可以攻击的才会攻击(这里应该是不可以锁定的)
//                 {
//                     //mHatredManager.RemoveHatred(tar);
//                     return null;
//                 }
//             }
            return tar;
        }

        public class Return_SelectAttackSkill
        {
            UInt16 mSkillId = UInt16.MaxValue;
            public UInt16 SkillId
            {
                get { return mSkillId; }
                set { mSkillId = value; }
            }
            bool mNeedMove = false;
            public bool NeedMove
            {
                get { return mNeedMove; }
                set { mNeedMove = value; }
            }
            SlimDX.Vector3 mMoveTarget;
            public SlimDX.Vector3 MoveTarget
            {
                get { return mMoveTarget; }
                set { mMoveTarget = value; }
            }
        }

        public virtual Return_SelectAttackSkill SelectAttackSkill(Role.RoleActor target, ushort skillid = 0)
        {
            var result = new Return_SelectAttackSkill();
            if (target == null)
                return result;
            var  dist = target.Placement.GetLocation()- Placement.GetLocation();
            dist.Y = 0;
            if (skillid != 0)
            {
                result.SkillId = skillid;
            }
            else
            {
                var skillinfo = GetPrioritySkill(dist.Length());
                if(skillinfo !=null)
                {
                    result.SkillId = skillinfo.SkillId;
                }
                else if (RoleTemplate.SkillInfos.Count != 0)
                {
                    result.SkillId = RoleTemplate.SkillInfos[0].SkillId;
                }
                else
                {
                    result.SkillId = 0;
                }
            }
            var skillData = FindSkillData(result.SkillId);
            if (skillData == null)
                return result;
            //如果射程都不够返回needMove，让AI去走过去           
            var attackRadius = skillData.Template.AttackRadius + RoleTemplate.Radius;
            if (dist.Length() > attackRadius)
            {
                var walkstate = this.AIStates.GetState("Walk");
                if(walkstate ==null)
                {
                    //不能移动
                    //this.HatredManager.RemoveHatred(target);
                    return null;
                }
                //需要移动，MoveTarge是一个位置
                result.NeedMove = true;
                var dir = Placement.GetLocation() - target.Placement.GetLocation();
                dir.Normalize();
                result.MoveTarget = target.Placement.GetLocation() + dir * (skillData.Template.AttackRadius + RoleTemplate.Radius - 0.6F) * 0.8F;
            }
            else
            {
                //不需要移动，MoveTarget是一个发射方向
                result.MoveTarget = target.Placement.GetLocation();
            }
            return result;
        }

        public SkillInfo GetPrioritySkill(float dist)
        {
            if (RoleTemplate.SkillInfos.Count == 0)
                return null;

            if (RoleTemplate.SkillInfos.Count > 2)
            {
                Random rand = new Random();
                var skillInfo = RoleTemplate.SkillInfos[rand.Next(1, RoleTemplate.SkillInfos.Count)];
                var skill = FindSkillData(skillInfo.SkillId);
                if (skill == null)
                    return null;
                if (skill.RemainCD > 0)
                    return null;
                return skillInfo;
            }
            return null;
            
// 
//             SkillInfo defaultSkill = null;
//             foreach (var skillinfo in RoleTemplate.SkillInfos)
//             {
//                 bool canFireType = false;
//                 switch (skillinfo.SkillFireType)
//                 {
//                     case EFireSkillType.Default://默认
//                         {
//                             defaultSkill = skillinfo;
//                         }
//                         break;
//                     case EFireSkillType.HpTrigger:
//                         {
//                             var rate = (float)RoleHP / (float)RoleMaxHP;
//                             if (rate <= skillinfo.SkillFireHpValue)
//                             {
//                                 canFireType = true;
//                             }
//                         }
//                         break;
//                     case EFireSkillType.RateTrigger://几率施法
//                         {
//                             canFireType = true;
//                         }
//                         break;
//                     case EFireSkillType.LengthPriotity://指定距离内施法
//                         {
//                             if (dist >= skillinfo.MinLength && dist < skillinfo.MaxLength)
//                             {
//                                 canFireType = true;
//                             }
//                         }
//                         break;
//                 }
//                 if (canFireType == false)
//                     continue;
//                 var skill = FindSkillData(skillinfo.SkillId);
//                 if (skill.FireSkillInfoData == null)
//                 {
//                     skill.FireSkillInfoData = new SkillInfoData();
//                 }
// 
//                 var cd = time - skill.FireSkillInfoData.FireTime;
//                 if (cd > skillinfo.CD && skill.FireSkillInfoData.CurFireTimes < skillinfo.CanFireTimes)//计算CD,计算施法次数
//                 {
//                     var rand = Random.NextDouble();
//                     if (rand <= skillinfo.Rate)
//                     {
//                         return skillinfo;
//                     }
//                 }
//             }
//             return defaultSkill;
        }

        public void FreshPriotitySkillById(UInt16 skillid)
        {
            var skilldata = FindSkillData(skillid);
            if (skilldata.FireSkillInfoData == null)
            {
                skilldata.FireSkillInfoData = new SkillInfoData();
            }
            //    Log.FileLog.WriteLine(string.Format("FreshPriotitySkillById,type_{0},runeid_{1}", skillinfo.SkillFireType, skillinfo.RuneId));
            skilldata.FireSkillInfoData.FireTime = GetCurFrameTickCount();
            if (skilldata.FireSkillInfoData.CurFireTimes < 254)
            {
                skilldata.FireSkillInfoData.CurFireTimes++;
            }
        }

        public bool ProcHurt(Role.RoleActor attacker, GameData.Skill.SkillData skillData, int damageNum = 1)
        {
            if (skillData == null)
                return false;
            if (CurrentState.StateName == "Death")
                return false;

            int damage = attacker.GetAttackResult(this, skillData);
            RoleHP -= damage;
            var hatredValue = Fight.Hatred.GetHatredValueByDamage(damage, attacker, this, skillData);

            //mHatredManager.AddHatred(attacker, hatredValue);

            if (CurrentState != null)
            {
                CurrentState.OnBeHurt(attacker);
            }
            if (damage != 0)//添加buff
            {
     //           HurtAddBuff(runeData, skillData, damage, attacker);
            }
            if (RoleHP < 1)
            {
                RoleHP = 0;
                if (skillData.Template.SummonOffsetType == GameData.Skill.EOffsetType.None && this.CurrentState.StateName != "Death"
                    && skillData.Template.OffsetType != GameData.Skill.EOffsetType.HitBack)
                {
                    var death = AIStates.GetState("Death");
                    if (death != null)
                    {
                        var param = death.Parameter as CSUtility.AISystem.States.IDeathParameter;
                        param.KillerId = attacker.SingleId;
                        CSUtility.AISystem.State.TargetToState(this, "Death", param);
                    }
                }
            }
            return true;
        }

        public int GetAttackResult(RoleActor target,SkillData skill)
        {
            var sesult = Random.Next(1,6) + RoleTemplate.BaseDamage +RoleLevel *5;
            var skilllevel = skill.GetSkillLevelTemp();
            if(skilllevel !=null)
            {
                sesult =(int)(sesult * (1 + skilllevel.RoleDamageRate));
                sesult += skilllevel.DamageAdd;
            }

            sesult =(int)(sesult * (1 - (target.RoleTemplate.BaseDefend * 0.06f) / (1 + target.RoleTemplate.BaseDefend * 0.06f)));
              
            //SendClientHurtDamage(target.SingleId,this.SingleId,skill.TemplateId,sesult,0);
            var pkg = new RPC.PackageWriter();
            Client.H_GameRPC.smInstance.HIndex(pkg, target.SingleId).RPC_UpdateCriticalValue(pkg, this.SingleId, target.SingleId, skill.TemplateId, sesult, 0);
            HostMap.SendPkg2Clients(null, target.Placement.GetLocation(), pkg);

            return sesult;
        }        

        public List<Summon.SummonRole> CreateSummonRolesBySkill(GameData.Skill.SkillData skillData, SlimDX.Vector3 summonPos, SlimDX.Vector3 rolePos, SlimDX.Vector3 tarPos,bool updateclient = true,UInt32 tarRoleId = UInt32.MaxValue)
        {
            var summonroles = new List<Summon.SummonRole>();
            for (UInt16 j = 0; j < skillData.Template.ThrowRoleCount; j++)
            {
                var fireDir = tarPos - rolePos;
                fireDir.Y = 0;
                fireDir.Normalize();
                if (skillData.Template.ReverseShoot)
                {
                    fireDir = -fireDir;
                }
                switch (skillData.Template.EmissionType)
                {
                    case GameData.Skill.EEmissionType.Bunch:
                        break;
                    case GameData.Skill.EEmissionType.Distribute:
                        {
                            if (skillData.Template.ThrowRoleCount > 1)
                            {
                                double emissionAngle = (double)(skillData.Template.EmissionAngle) / 180.0 * System.Math.PI;
                                double partAngle = emissionAngle / (skillData.Template.ThrowRoleCount - 1);
                                double startAngle = -emissionAngle / 2.0;
                                float angle = (float)(startAngle + partAngle * j);
                                var unitY = SlimDX.Vector3.UnitY;
                                SlimDX.Quaternion quat = new SlimDX.Quaternion();
                                SlimDX.Quaternion.RotationAxis(ref unitY, angle, out quat);
                                fireDir = SlimDX.Vector3.TransformCoordinate(fireDir, quat);
                            }
                        }
                        break;
                    case GameData.Skill.EEmissionType.SkillShotRan:
                        {
                            if (skillData.Template.ThrowRoleCount > 1)
                            {
                                var locx = Random.NextDouble();
                                var locz = Random.NextDouble();
                                var ranradius = skillData.Template.RandomRadius - 2 * skillData.Template.Radius;
                                summonPos.X = summonPos.X - ranradius + (float)(locx * 2 * ranradius);
                                summonPos.Z = summonPos.Z - ranradius + (float)(locz * 2 * ranradius);
                            }
                        }
                        break;
                }

                GameData.Role.SummonData smData = new GameData.Role.SummonData();
                smData.Init(skillData);
                smData.LockOnRoleId = tarRoleId;
                SlimDX.Vector3 loc = new SlimDX.Vector3();
                SlimDX.Vector3 levelDir = new SlimDX.Vector3();
                SlimDX.Vector3.Cross(ref SlimDX.Vector3.UnitY, ref fireDir, out levelDir);
                //levelDir.Normalize();
                loc = summonPos;
                loc += fireDir * skillData.Template.ThrowOffset;
                loc += levelDir * skillData.Template.LevelOffset;
                loc.Y += skillData.Template.HalfHeight;

                //switch (skillData.Template.SkillOperation)
                //{
                //     case GameData.Skill.ESkillOperationType.Skillshot: // 如果是指向技能，则使用玩家指定位置
                //         {
                //        loc = summonPos;
                //        // TODO： 安全判断， 看玩家传上来的位置是否合法
                //    }
                //    break;
                //    default:// 如果是非指向技能，则计算目的地
                //         {
                //        //SlimDX.Matrix absM = new SlimDX.Matrix();
                //        //Placement.GetMatrix(out absM);

                //        //loc = SlimDX.Vector3.TransformCoordinate(summonPos, absM);
                //    }
                //    break;
                //}

                if (skillData.Template.LevelOffset != 0 && skillData.Template.ThrowRoleCount <= 1)
                {
                    fireDir = tarPos - loc;
                    fireDir.Y = 0;
                    fireDir.Normalize();
                }
//                 if (runeData.Template.RuneType == CSCommon.Data.Skill.ERuneType.GuardSkill)
//                 {
//                     smData.Direction = -(float)System.Math.Atan2(fireDir.Z, fireDir.X);//这是一个右手法则函数所以要取反   
//                 }
//                 else
//                 {
//                     smData.Direction = Placement.GetRotationY();
//                 }

                Role.Summon.SummonRole sumrole = null;
                if (this.OwnerRole != null)
                {
                    sumrole = Summon.SummonRole.CreateSummonInstance(this.OwnerRole, smData, HallInstance, HostMap, ref loc, skillData, ref fireDir, updateclient);

                }
                else
                {
                    sumrole = Summon.SummonRole.CreateSummonInstance(this, smData, HallInstance, HostMap, ref loc, skillData, ref fireDir, updateclient);
                }
                summonroles.Add(sumrole);
            }
            return summonroles;
        }

        Role.RoleActor mAttackTarget = null;
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.攻击.攻击目标", CSUtility.Helper.enCSType.Server, "攻击目标(AttackTarget)")]
        public virtual Role.RoleActor AttackTarget
        {
            get
            {
                if (mAttackTarget == null)
                    mAttackTarget = SelectAttackTarget();
                return mAttackTarget;
            }
            set
            {
                mAttackTarget = value;
            }
        }
        public virtual void OnSummonNpc(UInt16 skillId, SlimDX.Vector3 dir, SlimDX.Vector3 summonPos)
        {
            var skillData = FindSkillData(skillId);
            if (skillData == null || skillData.Template == null)
                return;

            var fireDir = dir;
           // SlimDX.Vector3 loc;
        }

        public virtual void OnFireSkill(UInt16 skillId, SlimDX.Vector3 tarPos, SlimDX.Vector3 summonPos,UInt32 tarRoleId)
        {
            var skillData = FindSkillData(skillId);
            if (skillData == null || skillData.Template == null)
                return;
            if (skillData.Template.ThrowRoleCount > 0)
            {
                var summonroles = CreateSummonRolesBySkill(skillData, summonPos, mPlacement.GetLocation(), tarPos, true, tarRoleId);
            }
        }
        Random rd = new Random();
        public class BeHurtArgument
        {
            public CSUtility.Animation.NotifyPoint NotifyPt;
            public SkillData skillData;
            public SlimDX.BoundingBox absBox = new SlimDX.BoundingBox();
            public List<SlimDX.Matrix> absBoxMatrixList = new List<SlimDX.Matrix>();
            public int hurtNumber = 0;
            public int hurtCount = 0;
            public UInt32 actorTypes;
            public List<UInt32> roleIds = new List<UInt32>();
        }
        BeHurtArgument mCurrentBeHurtArg = new BeHurtArgument();
        protected bool OnVisitRole_ProcBeHurt(Role.RoleActor role, object arg)
        {
            if (!CanAttack(role))
                return true;
            if (role == this)
                return true;
            if (role.CurrentState.StateName == "Death")
                return true;
          //  bool mBoolHasDamage = false;
            var hurArg = arg as BeHurtArgument;
            hurArg.roleIds.Add(role.SingleId);
            hurArg.hurtNumber++;

            //AddSkillBuff2Bag(role, (ushort)hurArg.skillData.SkillTemlateId, hurArg.runeData, this);

            if (role.CurrentState.StateName != "StayAttack"/* || hurArg.runeData.Template.Force2BeAttack == true*/)
            {
                role.ProcDoBeAttack(hurArg.skillData, this);
            }

//             if (hurArg.NotifyPt != null)
//             {//这里今后要根据notifyPt的形状位置来判断
//                 mHitCount++;
//                 RoleProHurtCallBack(hurArg.runeData, this, role);
//                 mBoolHasDamage = role.ProcHurt(this, hurArg.skillData, hurArg.runeData, mHitCount);
//             }
// 
//             // 超过一次攻击个数上限或总个数超过了上限，则停止遍历
//             if (hurArg.roleIds.Count >= hurArg.runeData.Template.MaxDamageTargetNumber || hurArg.hurtNumber >= hurArg.runeData.Template.RuneTargetNumber)
//                 return false;

            return true;
        }
        public void DoTargetBeHurt(CSUtility.Animation.NotifyPoint ntp, GameData.Skill.SkillData skill)
        {
            Int64 time = this.GetCurFrameTickCount();
            if (ntp.NotifyName == "Chain01")
                return;

            UInt32 actorTypes = (1 << (Int32)CSUtility.Component.EActorGameType.Player) | (1 << (Int32)CSUtility.Component.EActorGameType.Npc);
            var arg = new BeHurtArgument();
            arg.NotifyPt = ntp;
            arg.skillData = skill;
            arg.hurtNumber = 0;

            var absBoxMatrixList = new List<SlimDX.Matrix>();
            SlimDX.Matrix absMatrix = new SlimDX.Matrix();
            Placement.GetAbsMatrix(out absMatrix);  //取得变换矩阵
            // 如果是AttackNotifyPoint，则用Box来做检测
            var atkPoint = ntp as CSUtility.ActionNotify.AttackNotifyPoint;
            if (atkPoint != null && atkPoint.BoxList.Count > 0)
            {
                SlimDX.BoundingBox absBox = new SlimDX.BoundingBox();           // 判定范围
                for (int iB = 0; iB < atkPoint.BoxList.Count; ++iB)
                {
                    var boxAbsM = atkPoint.BoxList[iB] * absMatrix;//矩阵变换，动作做角色矩阵的变换

                    SlimDX.Vector3 minBox = SlimDX.Vector3.UnitXYZ * -0.5f;
                    SlimDX.Vector3 maxBox = SlimDX.Vector3.UnitXYZ * 0.5f;
                    var bBox = new SlimDX.BoundingBox(minBox, maxBox);//单位大小
                    var corners = bBox.GetCorners();
                    for (int i = 0; i < 8; ++i)
                    {
                        corners[i] = SlimDX.Vector3.TransformCoordinate(corners[i], boxAbsM);//向量点乘矩阵
                    }

                    if (iB == 0)
                    {
                        absBox = SlimDX.BoundingBox.FromPoints(corners);
                    }
                    else
                    {
                        absBox = SlimDX.BoundingBox.Merge(absBox, SlimDX.BoundingBox.FromPoints(corners));
                    }

                    absBoxMatrixList.Add(SlimDX.Matrix.Invert(ref boxAbsM));//变换矩阵求逆后加进去
                }
                arg.absBox = absBox;
                CSUtility.Support.ConcurentObjManager<UInt32, Hall.Role.RoleActor>.FOnVisitObject  visitor = delegate (UInt32 i, Role.RoleActor role, object a)
                {
                    if (!CanAttack(role))
                        return CSUtility.Support.EForEachResult.FER_Continue;
                    if (role == this)
                        return CSUtility.Support.EForEachResult.FER_Continue;
                    if (role.CurrentState.StateName == "Death")
                        return CSUtility.Support.EForEachResult.FER_Continue;
                    bool mBoolHasDamage = false;
                    var hurArg = a as BeHurtArgument;
                    
                    //  AddSkillBuff2Bag(role, (ushort)hurArg.skillData.SkillTemlateId, hurArg.runeData, this);
                    if (role.CurrentState.StateName != "StayAttack" || hurArg.skillData.Template.Force2BeAttack == true)
                    {
                        role.ProcDoBeAttack(hurArg.skillData, this);
                    }
                    if (hurArg.NotifyPt != null&& RectInside(hurArg.absBox,role))
                    {//这里今后要根据notifyPt的形状位置来判断   
                        mBoolHasDamage = role.ProcHurt(this, hurArg.skillData);
                        if (mBoolHasDamage)
                        {
                            hurArg.roleIds.Add(role.SingleId);
                            hurArg.hurtNumber++;
                        }
                    }
                    // 超过一次攻击个数上限或总个数超过了上限，则停止遍历
                    if (hurArg.hurtNumber >= hurArg.skillData.Template.MaxDamageTargetNumber /*|| hurArg.hurtNumber >= hurArg.skillData.Template.RuneTargetNumber*/)
                        return CSUtility.Support.EForEachResult.FER_Stop; 
                    return CSUtility.Support.EForEachResult.FER_Continue;
                };
                var type = GameData.Role.ERoleType.Player | GameData.Role.ERoleType.Monster;
                HostMap.TourRole((uint)type,(uint)(absBox.Minimum.X/HostMap.CellSize-1),(uint)(absBox.Minimum.Z/HostMap.CellSize-1),(uint)(absBox.Maximum.X/HostMap.CellSize+1),(uint)(absBox.Maximum.Z/HostMap.CellSize+1), visitor, arg);
                arg.hurtCount++;
                mCurrentBeHurtArg = arg;
                mCurrentBeHurtArg.absBox = absBox;
                mCurrentBeHurtArg.absBoxMatrixList = absBoxMatrixList;
                mCurrentBeHurtArg.actorTypes = actorTypes;
                NotifyByChain(/*ntp,*/ mCurrentBeHurtArg);
            }
        }

        bool RectInside(SlimDX.BoundingBox box,RoleActor role)
        {
            var pos = role.Placement.GetLocation();
            var radius =role.RoleTemplate.Radius;
            var pos1 = new SlimDX.Vector3(pos.X- radius, pos.Y,pos.Z- radius);
            var pos2 = new SlimDX.Vector3(pos.X- radius, pos.Y, pos.Z+ radius);
            var pos3 = new SlimDX.Vector3(pos.X+ radius, pos.Y, pos.Z- radius);
            var pos4 = new SlimDX.Vector3(pos.X+ radius, pos.Y, pos.Z+ radius);

            if (box.Minimum.X <= pos.X && pos.X <= box.Maximum.X)
            {
                if (box.Minimum.Z <= pos.Z && pos.Z <= box.Maximum.Z)
                {
                    return true;
                }
            }
            if (box.Minimum.X <= pos1.X && pos1.X <= box.Maximum.X)
            {
                if (box.Minimum.Z<= pos1.Z && pos1.Z <= box.Maximum.Z)
                {
                    return true;
                }
            }
            if (box.Minimum.X <= pos2.X && pos2.X <= box.Maximum.X)
            {
                if (box.Minimum.Z <= pos2.Z && pos2.Z <= box.Maximum.Z)
                {
                    return true;
                }
            }

            if (box.Minimum.X <= pos3.X && pos3.X <= box.Maximum.X)
            {
                if (box.Minimum.Z <= pos3.Z && pos3.Z <= box.Maximum.Z)
                {
                    return true;
                }
            }
            if (box.Minimum.X <= pos4.X && pos4.X <= box.Maximum.X)
            {
                if (box.Minimum.Z <= pos4.Z && pos4.Z <= box.Maximum.Z)
                {
                    return true;
                }
            }

            return false;
        }

        public void NotifyByChain(/*CSCommon.Animation.NotifyPoint ntp, */ BeHurtArgument arg)
        {
            //                 if(rune.Template.ChainNotifyPointName!="")
            //                 {
            //                     if (ntp.NotifyName != rune.Template.ChainNotifyPointName)
            //                         return;
            //                 }

            if (arg.roleIds.Count > 0)
            {
                RPC.DataWriter dwParam = new RPC.DataWriter();
                dwParam.Write(arg.roleIds.Count);
                foreach (var id in arg.roleIds)
                {
                    dwParam.Write(id);
                }
//                     RPC.PackageWriter pkg = new RPC.PackageWriter();
//                     ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, this.SingleId).RPC_UpdateHurtRole(pkg, dwParam);
//                     HostMap.SendPkg2Clients(null, this.Placement.GetLocation(), pkg);
            }
        }

        float mMaxHurtRan = 0;
        bool mHasBeAttacked = false;
       public  class BlockArg
        {
            public List<Role.RoleActor> roles = new List<RoleActor>();
            public SlimDX.Vector3 pos = SlimDX.Vector3.Zero;
        }

        public void RoleInBlock()
        {
            if (mRoleCreateType != ERoleType.Monster)
                return;
            var pos = mPlacement.GetLocation();
            CSUtility.Support.ConcurentObjManager<UInt32, Hall.Role.RoleActor>.FOnVisitObject visitor = delegate (UInt32 i, Role.RoleActor role, object arg)
            {
                if (this.SingleId == role.SingleId || role.RoleTemplate.MonsterType != MonsterType.Normal)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                var dist = role.Placement.GetLocation() - pos;
                if (dist.Length() > role.RoleTemplate.Radius + this.RoleTemplate.Radius)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                //role.CurrentState.ToState("Idle", null);
                var tarpos = role.mPlacement.GetLocation();
                var distance = Math.Sqrt(Math.Pow(Math.Abs(tarpos.X - pos.X), 2) + Math.Pow(Math.Abs(tarpos.Z - pos.Z), 2));
                var ratio = distance / (double)RoleTemplate.Radius;
                var x = (int)(Math.Abs(tarpos.X - pos.X) / ratio);
                var z = (int)(Math.Abs(tarpos.Z - pos.Z) / ratio);

                if (tarpos.X > pos.X)
                    tarpos.X = x + pos.X + role.RoleTemplate.Radius + 0.5f;
                else
                    tarpos.X = pos.X - x - role.RoleTemplate.Radius - 0.5f;

                if (tarpos.Z > pos.Z)
                    tarpos.Z = z + pos.Z + role.RoleTemplate.Radius + 0.5f;
                else
                    tarpos.Z = pos.Z - z - role.RoleTemplate.Radius - 0.5f;

                role.Placement.SetLocation(tarpos);

                return CSUtility.Support.EForEachResult.FER_Continue;
            };

            var type = GameData.Role.ERoleType.Monster;
            //这个length是以pos为中心,判断該role是否在别的Role里面
            this.HostMap.TourRoles((UInt32)type, ref pos, (UInt32)(this.RoleTemplate.Radius * 3), visitor, null);
        }

        public bool IsBlock(SlimDX.Vector3 pos, bool checkRole = true)
        {
            var arg = BlockRole(pos, checkRole);
            if (arg == null || arg.roles.Count == 0)
                return false;
            //else
            //    return true;

            var rolePos = Placement.GetLocation();
            var dir = pos - rolePos;
            dir.Normalize();
            HostMap.MapTemplate.SetDynamicNavData(rolePos.X, rolePos.Z, RoleTemplate.Radius, false);
            var newLoc = rolePos + dir * RoleTemplate.Radius;
            var ret = HostMap.MapTemplate.HasBarrier(newLoc.X, newLoc.Z, pos.X, pos.Z);
            HostMap.MapTemplate.SetDynamicNavData(rolePos.X, rolePos.Z, RoleTemplate.Radius, true);

            return ret;
        }
        public BlockArg BlockRole(SlimDX.Vector3 pos,bool checkRole = true)
        {
            if (this.RoleCreateType == ERoleType.Summon)
                return null;
            var blockArg = new BlockArg();
            blockArg.pos = pos;
            CSUtility.Support.ConcurentObjManager<UInt32, Hall.Role.RoleActor>.FOnVisitObject visitor = delegate (UInt32 i, Role.RoleActor role, object arg)
            {
                if (this.SingleId == role.SingleId || role.RoleTemplate.MonsterType > MonsterType.Building)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                var rolePos = role.Placement.GetLocation();
                var dist = rolePos -blockArg.pos;
                dist.Y = 0;
                var distDir = dist.Length();
                var disrole = rolePos - Placement.GetLocation();
                disrole.Y = 0;
                var distRole = disrole.Length();

                bool dirBlock = false;
                //bool roleBock = false;
        
                if (distDir < role.RoleTemplate.Radius)//行走方向是否block
                {
                    dirBlock = true;
                }
                if(!dirBlock)
                {
                    dist.Normalize();
                    var newLoc = rolePos - dist * (role.RoleTemplate.Radius + this.RoleTemplate.Radius);
                }
//                 if (checkRole)//角色自己是否block
//                 {
//                     if (distRole < role.RoleTemplate.Radius + this.RoleTemplate.Radius)
//                     {
//                         roleBock = true;
//                     }
//                 }                

                if (dirBlock ==true /*&& roleBock ==true*/)
                {
                    blockArg.roles.Add(role);
                    return CSUtility.Support.EForEachResult.FER_Stop;
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            };

            var type = GameData.Role.ERoleType.Player | GameData.Role.ERoleType.Monster;
            //这个length是以pos为中心
            this.HostMap.TourRoles((UInt32)type, ref pos, (UInt32)(this.RoleTemplate.Radius*3), visitor, null);
            return blockArg;
        }
        #endregion

        #region 虚拟接口定义
        public virtual bool AIServerControl
        {
            get { return true; }
        }

        protected CSUtility.Support.EForEachResult OnVisitRole_ProcessTrigger(UInt32 id,Role.RoleActor role, object arg)
        {
            var trigger = role as Role.Trigger.TriggerInstance;

            if (trigger != null)
                trigger.ProcessActorEnter(this);

            return CSUtility.Support.EForEachResult.FER_Continue;
        }

        public virtual void OnPlacementUpdatePosition(ref SlimDX.Vector3 pos)
        {
            // 更新地图格
            if (!this.HostMap.IsNullMap)
            {
                var mapCell = this.HostMap.GetMapCell(pos.X, pos.Z);
                if (mapCell != null)
                {
                    if (this.Cell != mapCell)
                    {
                        mapCell.Enter(this);
                    }
                }
            }
        }

        public virtual void OnPlacementUpdateDirectionY(float angle)
        {

        }

        public virtual void OnRoleWalkToBlock()
        {

        }

        public virtual void SetRoleAttackImider()
        {

        }

        protected Guid mRebornTrackerId = Guid.Empty;
        public virtual void ResetDefaultState()
        {
            //             FreshRoleValue();
            RoleHP = RoleMaxHP;
            RoleMP = RoleMaxMP;

            //HatredManager.ClearHatred();

            //让npc出生的时候走一次RigidTracker设置
            var spState = this.AIStates.GetState("Reborn") as CSUtility.AISystem.States.SpecialAction;
            if (mRebornTrackerId != Guid.Empty && spState != null)
            {
                CurrentState = spState;
                if (spState != null)
                {
                    spState.OnPreEnterState();
                    spState.OnEnterState();
                }
                spState.SpecialActionParameter.ExtraInfomation = mRebornTrackerId.ToString();
            }
            else
            {
                if (AIStates.DefaultState != null)
                {
                    CurrentState = AIStates.DefaultState;
                }

                if (CurrentState != null)
                {
                    CurrentState.OnPreEnterState();
                    CurrentState.OnEnterState();
                    CurrentState.OnPostEnterState();
                }
            }
            mIsDeath = false;
        }

        public virtual bool NotCalcLevelDiff()
        {
            return false;
        }

        #endregion

        #region 状态转换
        #region 被攻击状态转换
        Int64 mUpdateProcBeAtt = 0;
        Int64 mUpdateChangeBeAtt = 0;
        public void ProcDoBeAttack(GameData.Skill.SkillData skillData, RoleActor attacker)
        {
            bool forceBeAttack = false;
            if (CurrentState.StateName == "BeAttack" || attacker.CurrentState.StateName == "Death")
            {
                return;
            }
            //             if (RoleAttackState.Contains(csu.Data.EAttackState.Invincible))//无敌状态不会进入被动
            //                 return;

            Int64 time = this.GetCurFrameTickCount();
            var state = AIStates.GetState("BeAttack");
            if (state == null)
            {
                return;
            }

            //处理 Buff

            //             if (runeData.Template.GetRuneLevelParam(runeData.RuneLevel) != null)
            //             {
            //                 var mRuneParam = runeData.Template.GetRuneLevelParam(runeData.RuneLevel);
            //                 foreach (var mparam in mRuneParam.BuffParams)
            //                 
            //                     var buff = CSCommon.Data.Skill.BuffTemplateManager.Instance.FindBuff((ushort)mparam.BuffId);
            //                     if (buff != null)
            //                     {
            //                         if (buff.BuffType == CSCommon.Data.Skill.BuffType.Endure)//有霸体buff就不做 beattack le
            //                         {
            //                             DoSambleBeAttack();
            //                             return;
            //                         }
            //                         //                         if (buff.BuffType == CSCommon.Data.Skill.BuffType.SwimState)
            //                         //                         {
            //                         //                             forceBeAttack = true;
            //                         //                         }
            //                     }
            //                 }
            //                 if (runeData.Template.Force2BeAttack)
            //                 {
            //                     forceBeAttack = true;
            //                 }
            //             }

            if (forceBeAttack == false)
            {
                if (this.CurrentState.StateName == "Walk")
                {
                    return;
                }
                if (mHasBeAttacked == true)
                {
                    DoSambleBeAttack();
                    return;
                }
                if (CurrentState.StateName == "StayAttack")
                {
                    //                     var attParam = CurrentState.Parameter as CSUtility.AISystem.States.IStayAttackParameter;
                    //                     if (attParam != null)
                    //                     {
                    //                         if (mRune != null && mRune.CanBeBreak == false)//正在放不被打断的技能
                    //                         {
                    //                             DoSambleBeAttack();
                    //                             return;
                    //                         }
                    //                     }
                }

                if (RoleCreateType == GameData.Role.ERoleType.Player)
                {
                    mMaxHurtRan -= (float)((time - mUpdateProcBeAtt) % 1000) / 2;
                    if (mMaxHurtRan <= 0)
                    {
                        mMaxHurtRan = 15;
                    }
                }
                else
                {
                    mMaxHurtRan -= (float)((time - mUpdateProcBeAtt) % 1000);
                    if (mMaxHurtRan <= 0)
                    {
                        mMaxHurtRan = 30;
                    }
                }
                mUpdateProcBeAtt = time;
                var rand = rd.Next(1, 100);
                if (rand >= mMaxHurtRan)
                {
                    DoSambleBeAttack();
                    return;
                }
            }

            CSUtility.AISystem.States.IBeAttackParameter param = new CSUtility.AISystem.States.IBeAttackParameter();

            if (attacker != null)
            {
                if (attacker.OwnerRole != null && attacker.RoleCreateType == GameData.Role.ERoleType.Summon)
                {
                    attacker = attacker.OwnerRole;
                }
                param.AttackerSingleId = attacker.SingleId;
                param.AttackerPos = attacker.Placement.GetLocation();
            }
            param.SkillId = (ushort)skillData.TemplateId;
            //             param.RuneId = (ushort)runeData.RuneTemlateId;
            //             param.RuneLevel = runeData.RuneLevel;
            //             param.ActionName = runeData.Template.BeAttackAction;
            //             var levelp = runeData.Template.GetRuneLevelParam(runeData.RuneLevel);
            //             if (levelp != null)
            //             {
            //                 param.Duration = levelp.BeAttackDuration;
            //             }

            CurrentState.CanInterrupt = true;
            //System.Diagnostics.Debug.WriteLine(string.Format("ProcDoBeAttack,name{0},tiem{1},attacker{2}", this.RoleTemplate.NickName, param.Duration, attacker.RoleTemplate.NickName));
            //CSUtility.AISystem.State.TargetToState(this, "BeAttack", param);
            mHasBeAttacked = true;
            if (RoleCreateType == GameData.Role.ERoleType.Player)
            {
                if (time - mUpdateChangeBeAtt > 5000)
                {
                    mHasBeAttacked = false;
                }
            }
            else
            {
                if (time - mUpdateChangeBeAtt > 3000)
                {
                    mHasBeAttacked = false;
                }
            }
            mUpdateChangeBeAtt = time;
        }

        void DoSambleBeAttack()
        {
            if (CurrentState.CanInterrupt == false)
                return;
            if (this.CurrentState.StateName == "Walk" || this.CurrentState.StateName == "BeAttack" || this.RoleCreateType == GameData.Role.ERoleType.Player)
            {
                return;
            }
            CSUtility.AISystem.States.IBeAttackParameter param = new CSUtility.AISystem.States.IBeAttackParameter();
            param.AttackerSingleId = SingleId;
            param.Duration = 50;
            //CurrentState.ToState("BeAttack", param);
        }

        protected bool OnVisitRole_ProcBeAttacks(Role.RoleActor role, object arg)
        {
            if (role == this)
                return true;

            var ntp = arg as CSUtility.Animation.NotifyPoint;

            var state = role.AIStates.GetState("BeAttack");
            if (state != null)
            {
                CSUtility.AISystem.States.IBeAttackParameter param = new CSUtility.AISystem.States.IBeAttackParameter();
                param.AttackerPos = this.Placement.GetLocation();
                CSUtility.AISystem.State.TargetToState(role, "BeAttack", param);
            }

            return true;
        }

        protected void DoTargetBeAttacks(CSUtility.Animation.NotifyPoint ntp)
        {
            var loc = Placement.GetLocation();
          //  UInt32 actorTypes = (1 << (Int32)CSUtility.Component.EActorGameType.Player) | (1 << (Int32)CSUtility.Component.EActorGameType.Npc);
        //    float radius = 2;//临时代码，正确的做法是从ntp里面获得当前打击范围
            //HostMap.TourRoles(ref loc, radius, actorTypes, this.OnVisitRole_ProcBeAttacks, ntp);
        }
        public void ProcTargetBeAttackNotifiers(CSUtility.Animation.AnimationTree anim)
        {
            if (CurrentState == null)
                return;
            if (anim.Action != null)
            {
                var ntf = anim.Action.GetNotifier("TargetBeAttack");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(CurrentState.NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null)
                    {
                        foreach (var i in nplist)
                        {
                            this.DoTargetBeAttacks(i);
                        }
                    }
                }
            }

            var subAnims = anim.GetAnimations();
            foreach (var i in subAnims)
            {
                if (i != null)
                {
                    ProcTargetBeAttackNotifiers(i);
                }
            }
        }
        #endregion

        #region 死亡
        protected bool mIsDeath = false;
        public bool IsDeath()
        {
            if (CurrentState != null)
            {
                if (CurrentState.StateName == "Death")
                    return true;
                else
                    return false;
            }
            return mIsDeath;
        }
        public delegate void OnProcDeath();
        public event OnProcDeath RoleProcDeath;
        public virtual void ProcDeath()
        {
            mIsDeath = true;
            if(RoleProcDeath !=null)
                RoleProcDeath();    

            if (RoleTemplate.OnRoleDeathCB != null)
            {
                var callee = RoleTemplate.OnRoleDeathCB.GetCallee() as GameData.Role.FOnRoleDeath;
                callee?.Invoke(this);
            }
            var loc = mPlacement.GetLocation();
            UInt32 actorTypes = (Int32)GameData.Role.ERoleType.Player;
            HostMap.TourRoles(actorTypes, ref loc, 10, this.OnVisitRole_GainExpToNearby, null);
        }

        public virtual void TickSkillCD(long elapseMillsecond)
        {

        }

        long booldReturnTimes = 0;
        public virtual void TickBloodReturn(long elapseMillsecond)
        {
            if (RoleTemplate == null)
                return;
            if (RoleTemplate.BloodReturn > 0)
            {
                booldReturnTimes -= elapseMillsecond;
                if (booldReturnTimes <= 0)
                {
                    booldReturnTimes = RoleTemplate.BloodReturnTimes;
                    RoleHP += RoleTemplate.BloodReturn;
                    if (RoleHP > RoleMaxHP)
                        RoleHP = RoleMaxHP;
                }
            }
        }

        protected CSUtility.Support.EForEachResult OnVisitRole_GainExpToNearby(UInt32 id, Role.RoleActor role, object arg)
        {
            var tarPlayer = role as Player.PlayerInstance;
            if (role == this || role.IsDeath())
                return CSUtility.Support.EForEachResult.FER_Continue;

            if (mRoleCreateType == ERoleType.Monster || mRoleCreateType == ERoleType.Player)
            {
                int exp = 0;
                if (mRoleCreateType == ERoleType.Player)
                {
                    var hostPlayer = this as Player.PlayerInstance;
                    exp = (int)CSUtility.Data.DataTemplateManager<Byte, ExpLevel>.Instance.GetDataTemplate((Byte)0).GetDeathProvideExp(hostPlayer.PlayerData.RoleLevel - 1);// (int)ExpLevel.Instance.GetDeathProvideExp(hostPlayer.PlayerData.RoleLevel - 1);
                }
                else
                {
                    exp = this.RoleTemplate.DeathExp;
                }
                if (tarPlayer != null && tarPlayer.FactionId != this.FactionId)
                    tarPlayer.GainExp(exp);
            }

            return CSUtility.Support.EForEachResult.FER_Continue;
        }

        public virtual void TargetDeath(RoleActor target, int index)
        {
            //mHatredManager.RemoveHatred(target);

            if (target.CurrentState == null)
                return;
            target.CurrentState.OnTargetDeath(target, index);
        }
        #endregion

        [CSUtility.Event.Attribute.AllowMember("角色对象.设置是否在温泉里属性", CSUtility.Helper.enCSType.Common, "设置是否在温泉里属性")]
        public void Trigger_SetHotSpring(CSUtility.Map.Trigger.TriggerData triggerData, bool value)
        {
            if (FactionId == triggerData.FactionId)
            {
                mIsEnterHotSpring = value;
            }
        }

        [CSUtility.AISystem.Attribute.AllowMember("角色对象.攻击.寻找仇恨目标攻击", CSUtility.Helper.enCSType.Server, "寻找仇恨目标攻击(DoAttackCurTargetAIBehavior)")]
        public bool DoAttackCurTargetAIBehavior()
        {            
            var target = this.AttackTarget;
            if (target == null)
                return false;

            var pos1 = target.Placement.GetLocation();
            var pos2 = this.Placement.GetLocation();
            pos1.Y = pos2.Y = 0;
            if (target.CurrentState.StateName == "Death" || SlimDX.Vector3.Distance(pos1,pos2) > RoleTemplate.LockOnRadius)
            {
                this.AttackTarget = null;
                return false;
            }
                
            if (CurrentState.StateName != "Idle" && CurrentState.StateName != "Walk")
                return false;

            var atk = AIStates.GetState("StayAttack") as CSUtility.AISystem.States.StayAttack;
            if (atk == null)
                return false;
                            
            var selector = SelectAttackSkill(target);
            if (selector == null)
                return false;
            if (selector.NeedMove)
            {
                DoWalkToTargetPos(selector.MoveTarget, (CSUtility.AISystem.FOnStateExit)this.OnStateExit_AttackAfterWalk);
            }
            else
            {
                var dir = target.Placement.GetLocation() - Placement.GetLocation();
                dir.Normalize();
                //     Log.FileLog.WriteLine(string.Format("DoAttackCurTargetAIBehavior:{0},Role_{1},skillid_{2},runeid_{3}", GetCurFrameTickCount(),this.RoleTemplate.NickName,selector.SkillId,selector.RuneId));
                DoFireSkill(selector.SkillId, SlimDX.Vector3.Zero, selector.MoveTarget, float.MaxValue, SlimDX.Vector3.Zero,target.SingleId);
                FreshPriotitySkillById(selector.SkillId);
            }
            return true;
                  
        }

        [CSUtility.AISystem.Attribute.AllowMember("角色对象.攻击.寻找可攻击的目标添加仇恨", CSUtility.Helper.enCSType.Server, "寻找可攻击的目标添加仇恨(FindCanAttackRoleAddHa)")]
        public void FindCanAttackRoleAddHa(float length)
        {
            CSUtility.Support.ConcurentObjManager<UInt32, Hall.Role.RoleActor>.FOnVisitObject visitor = delegate (UInt32 i, Role.RoleActor role, object arg)
            {
                if (this.SingleId == role.SingleId)
                    return CSUtility.Support.EForEachResult.FER_Continue;
                var dist = role.Placement.GetLocation() - this.Placement.GetLocation();
                dist.Y = 0;
                if(dist.Length()<length && this.CanAttack(role))
                {
                    //this.mHatredManager.AddHatred(role,1);
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            var pos = this.Placement.GetLocation();
            var type = GameData.Role.ERoleType.Player | GameData.Role.ERoleType.Monster;
            //这个length是以pos为中心
            this.HostMap.TourRoles((UInt32)type, ref pos, (uint)length, visitor, null);
        }
        private void OnStateExit_AttackAfterWalk(CSUtility.AISystem.State preState, CSUtility.AISystem.State curState)
        {
            if (curState.StateName == "Idle" || curState.StateName == "Walk")
            {
                DoAttackCurTargetAIBehavior();
            }
        }
        #region  Walk
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.寻路.寻找下个路点行走", CSUtility.Helper.enCSType.Server, "寻找下个路点行走(DoWanderAIBehavior)")]
        public void DoWanderAIBehavior()
        {
            var center = Placement.GetLocation();
            var ran = smRandom.Next(0, 100);
            if (ran >= PreWanderAI)
                return;

            var walkState = this.AIStates.GetState("Walk");
            if (walkState != null)
            {
                var wanderTarget = GetWanderTarget();
                if (wanderTarget == SlimDX.Vector3.Zero)
                    return;

                if (!CheckNeedFindWay2Walk(wanderTarget))
                {
                    return;
                }

                DoWalkToTargetPos(wanderTarget);
            }
        }
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.寻路.走到指定地点(x:z)", CSUtility.Helper.enCSType.Server, "走到指定地点(DoWalkToTargetPos)")]
        public void DoWalkToTargetPos(float x,float z)
        {
            var pos = new SlimDX.Vector3(x,0,z);
            DoWalkToTargetPos(pos,null);
        }

        class PathBlock
        {
            public float X;
            public float Z;
            public float Radius;
            public PathBlock(float x, float z, float radius)
            {
                X = x;
                Z = z;
                Radius = radius;
            }
        }
        public SlimDX.Vector3 mCurFindTargetPos = SlimDX.Vector3.Zero;
                
        List<PathBlock> mPathBlocks = new List<PathBlock>();
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.寻路.走到指定地点", CSUtility.Helper.enCSType.Server, "走到指定地点(DoWalkToTargetPos)")]
        public void DoWalkToTargetPos(SlimDX.Vector3 pos, CSUtility.AISystem.FOnStateExit callback =null)
        {
            var walkState = this.AIStates.GetState("Walk");
            if (walkState == null)
                return;
//             if (mCurFindTargetPos == pos)
//                 return;
           
            if (this.CurrentState.StateName == "Walk")
            {
                var curparam = this.CurrentState.Parameter as CSUtility.AISystem.States.IWalkParameter;
                if (curparam != null)
                {
                    foreach (var tarpos in curparam.TargetPositions)
                    {
                        var wanderDis = SlimDX.Vector3.Distance(tarpos, pos);
                        if (wanderDis <= 0.6)
                            return;
                    }
                    var Dis = SlimDX.Vector3.Distance(curparam.TargetPosition, pos);
                    if (Dis <= 0.6)
                        return;
                }
            }

            var start = FindSourroundFindWayPoint(pos);
            if (start == SlimDX.Vector3.Zero)
                return;
            mCurFindTargetPos = pos;
            mFindWayTargetPos = pos;
            var walkParam = walkState.Parameter as CSUtility.AISystem.States.IWalkParameter;
            var pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_PathFindServer(pkg).GlobalMapFindPath(pkg, this.HallInstance.HallsId, HostMap.MapSourceId, HostMap.MapTemplate.MapGuid, this.Id, Placement.GetLocation(), pos);
            pkg.WaitDoCommandWithTimeOut(3, HallServer.Instance.PathFindConnect, RPC.CommandTargetType.DefaultType, null).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {  
                if (bTimeOut)
                {
                }
                else
                {
                    Byte pathFindResult;
                    _io.Read(out pathFindResult);
                    switch ((CSUtility.Navigation.INavigationWrapper.enNavFindPathResult)pathFindResult)
                    {
                        case CSUtility.Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Cancel:
                            {
                                //之前的寻路请求被新的覆盖了
                            }
                            break;
                        case CSUtility.Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Success:
                            {
                                int count = 0;
                                _io.Read(out count);
                                walkParam.TargetPositions.Clear();
                                for (int i = 0; i < count; i++)
                                {
                                    SlimDX.Vector2 pathPt;
                                    _io.Read(out pathPt);
                                    var pt = new SlimDX.Vector3();
                                    pt.X = pathPt.X;
                                    pt.Z = pathPt.Y;
                                    pt.Y = HostMap.GetAltitude(pt.X, pt.Z);
                                    walkParam.TargetPositions.Enqueue(pt);
                                }
                                walkParam.MoveSpeed = MoveSpeed;
                                walkParam.TargetPosition = start;
                                walkParam.FinalPosition = pos;
                                walkParam.MaxCloseDistance = 0.05f;
                                walkParam.ActionPlayRate = SpeedRate;
                                walkParam.Run = 0;
                                this.IsRun = 0;
                                this.CurrentState.ToState("Walk", walkParam);

                                if (callback != null)
                                {
                                    var ecb = new CSUtility.Helper.EventCallBack(CSUtility.Helper.enCSType.Server)
                                    {
                                        CBType = typeof(CSUtility.AISystem.FOnStateExit),
                                        Id = Guid.NewGuid(),
                                        Callee = callback
                                    };
                                    this.PushStateExit(ecb);
                                }
                            }
                            break;
                        default:
                            {
                                this.CurrentState.ToState("Idle", null);
                            }
                            break;
                    }                    
                }
                mCurFindTargetPos = SlimDX.Vector3.Zero;
            };
        }


        public SlimDX.Vector3 FindSourroundFindWayPoint(SlimDX.Vector3 tarPos)
        {
            var nowloc = Placement.GetLocation();
            var radius = this.RoleTemplate.Radius +0.3f;
            var dir = tarPos - nowloc;
            dir.Y = 0;
            dir.Normalize();
            var startpos = nowloc + dir * radius;
            if (!IsBlock(startpos, false))
                return startpos;
            else
            {
                var dir2 = SlimDX.Vector3.Cross(dir,SlimDX.Vector3.UnitY);
                dir2.Y = 0;
                dir2.Normalize();
                var dir3 = dir + dir2;
                dir3.Normalize();
                var dir4 = dir - dir2;
                dir4.Normalize();
                startpos = nowloc + dir3 * radius;//右前
                if (!IsBlock(startpos, false))
                    return startpos;
                startpos = nowloc + dir4 * radius;//左前
                if (!IsBlock(startpos, false))
                    return startpos;
                startpos = nowloc + dir2 * radius;//右方
                if (!IsBlock(startpos,false))
                    return startpos;
                startpos = nowloc - dir2 * radius;//左方
                if (!IsBlock(startpos, false))
                    return startpos;
                startpos = nowloc - dir * radius;//后方
                if (!IsBlock(startpos, false))
                    return startpos;
            }
            return SlimDX.Vector3.Zero;
        }


        public bool CheckNeedFindWay2Walk(SlimDX.Vector3 targetPos)
        {
            if (this.CurrentState.StateName == "Walk")
            {
                if (targetPos == mFindWayTargetPos)//同一个寻路点不寻多次
                    return false;
                var curparam = this.CurrentState.Parameter as CSUtility.AISystem.States.IWalkParameter;
                if (curparam != null)
                {
                    var wanderDis = SlimDX.Vector3.Distance(curparam.TargetPosition, targetPos);
                    if (wanderDis <= 0.6)
                        return false;
                    foreach (var tarpos in curparam.TargetPositions)
                    {
                        var wanDis = SlimDX.Vector3.Distance(tarpos, targetPos);
                        if (wanDis <= 0.6)
                            return false;
                    }
                }
            }
            return true;
        }
        SlimDX.Vector3 mFindWayTargetPos = SlimDX.Vector3.Zero;
        #endregion
        #endregion

        public void TickCollision()
        {
            if (this.RoleTemplate == null)
                return;
            if (!this.RoleTemplate.CalCollission)
                return;

            if (this.CurrentState.StateName == "Death")
                return;

            var pos = this.Placement.GetLocation();

            var type = (UInt32)(GameData.Role.ERoleType.Player | GameData.Role.ERoleType.Monster);
            HostMap.TourRoles(type, ref pos, 5, (UInt32 id, Role.RoleActor role, object arg) =>
            {
                pos.Y = 0;
                var rolePos = role.Placement.GetLocation();
                rolePos.Y = 0;
                if (SlimDX.Vector3.Distance(pos, rolePos) < 1.0f && role.RoleTemplate.PrimaryRole)
                {                    
                    if (this.RoleTemplate.OnCollisionCB != null)
                    {
                        var callee = this.RoleTemplate.OnCollisionCB.GetCallee() as GameData.Role.FOnCollision;
                        callee?.Invoke(this, role);                        
                    }

                    var pkg = new RPC.PackageWriter();
                    Client.H_GameRPC.smInstance.HIndex(pkg, this.SingleId).RPC_RoleCollision(pkg, role.SingleId);
                    HostMap.SendPkg2Clients(this, Placement.GetLocation(), pkg);

                    return CSUtility.Support.EForEachResult.FER_Stop;
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }

        [CSUtility.AISystem.Attribute.AllowMember("角色对象.加血", CSUtility.Helper.enCSType.Server, "加血")]
        [CSUtility.Event.Attribute.AllowMember("角色对象.加血", CSUtility.Helper.enCSType.Server, "加血")]
        public void AddHp(int hp)
        {
            var curHp = RoleHP + hp;
            if(curHp > RoleMaxHP)
            {
                RoleHP = RoleMaxHP;
            }
            else
            {
                RoleHP = curHp;
            }
        }

        [CSUtility.AISystem.Attribute.AllowMember("角色对象.吃Buff", CSUtility.Helper.enCSType.Server, "吃Buff")]
        [CSUtility.Event.Attribute.AllowMember("角色对象.吃Buff", CSUtility.Helper.enCSType.Server, "吃Buff")]
        public bool EatBuff(string name)
        {
            var monster = this.HostMap.GetMonsterByName(name);
            if (monster != null)
            {                
                DoWalkToTargetPos(monster.Placement.GetLocation());
                return true;
            }

            return false;
        }        
    }    
}
