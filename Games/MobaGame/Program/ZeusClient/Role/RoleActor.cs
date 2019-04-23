using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using GameData.Role;
using CCore.Scene;
using CCore.Camera;
using CCore.Graphics;
using SlimDX;

namespace Client.Role
{
    [RPC.RPCClassAttribute(typeof(RoleActor))]
    public partial class RoleActor : CCore.World.Actor, CSUtility.AISystem.IStateHost, RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion
        UInt32 mRoleTemplateVersion = 0;
        public void InitRoleActor(PlayerData pd)
        {
            mRoleData = new RoleData(pd, this);
            mRoleTemplateVersion = mRoleData.TemplateVersion;

        //    Decal.SetMaterial(CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IHelper.GuidParse("036b8663-3693-48e4-bbfc-2e00a8e4dd8f")));
        }

        public void InitRoleActor(PlayerData pd,bool otherPlayer)
        {
            mRoleData = new RoleData(pd, this, otherPlayer);
            mRoleTemplateVersion = mRoleData.TemplateVersion;            
        }

        public void InitRoleActor(MonsterData data)
        {
            mRoleData = new RoleData(data, this);
            mRoleTemplateVersion = mRoleData.TemplateVersion;
          //  Decal.SetMaterial(CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IHelper.GuidParse("036b8663-3693-48e4-bbfc-2e00a8e4dd8f")));
        }

        public void InitRoleActor(SummonData data)
        {
            mRoleData = new RoleData(data, this);
            mRoleTemplateVersion = mRoleData.TemplateVersion;
        }

        #region Role数据        

        RoleData mRoleData;
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.RoleData", CSUtility.Helper.enCSType.Client, "角色数据")]
        [CSUtility.Event.Attribute.AllowMember("角色对象.RoleData", CSUtility.Helper.enCSType.Client, "角色数据")]
        public RoleData RoleData
        {
            get { return mRoleData; }
        }

        UInt32 mSingleId;
        public UInt32 SingleId
        {
            get { return mSingleId; }
        }
                        
        public void _SetSingleId(UInt32 value)
        {
            mSingleId = value;
        }

        public string RoleName
        {
            get
            {
                return mRoleData.RoleName;
            }
        }

        bool mStateNotify2Remote = false;
        public virtual bool StateNotify2Remote
        {
            get { return mStateNotify2Remote; }
            set { mStateNotify2Remote = value; }
        }

        float mSpeedRate = 1.0F;
        public float SpeedRate
        {
            get { return mSpeedRate; }
            set
            {
                mSpeedRate = value;
            }
        }

        bool mRun = false;
        public bool Run
        {
            get { return mRun; }
            set { mRun = value; }
        }

        public RoleTemplate RoleTemplate
        {
            get
            {
                if (mRoleData != null)
                    return mRoleData.RoleTemplate;
                return null;
            }
        }

        public float MoveSpeed
        {
            get
            {                                                                   
                if (mRoleData != null)
                    return mRoleData.RoleMoveSpeed;
                                                
                return 0;          
            }
            set
            {
                if (mRoleData != null)
                    mRoleData.RoleMoveSpeed = value;
            }
        }

        bool mIsLeaveMap = false;
        public bool IsLeaveMap
        {
            get { return mIsLeaveMap; }
        }

        bool mReadyToLeaveMap = false;
        public bool ReadyToLeaveMap
        {
            get { return mReadyToLeaveMap; }
        }

        public SlimDX.Vector3 mBlockPos = SlimDX.Vector3.Zero;

        public static System.Random RandomAngle = new System.Random();

        Buff.BuffBag mBuffBag = null;
        public Buff.BuffBag BuffBag
        {
            get
            {
                if(mBuffBag ==null)
                    mBuffBag = new Buff.BuffBag(this);
                return mBuffBag;
            }
            set
            {
                mBuffBag = value;
            }
        }

        public bool IsChielfPlayer()
        {
            if (mPlacement == null)
                return false;
            if (SingleId == 0)
                return false;
            return mPlacement.GetType() == typeof(ChiefPlayerPlacement);
        }

        public override void Cleanup()
        {
            if (IsChielfPlayer())
                return;

            base.Cleanup();

            RoleManager.Instance.UnmapRoleId(this);
        }

        public void DoLeaveMap()
        {
            if (mIsLeaveMap)
                return;
            if (SingleId == 0)
            {
                RoleManager.Instance.AddErrorActor(this);
                mIsLeaveMap = true;
                return;
            }
            if (!mReadyToLeaveMap)
            {
                mReadyToLeaveMap = true;
                var parentMesh = Visual as CCore.Mesh.Mesh;
                if (parentMesh != null)
                {
                    parentMesh.StartFadeOut(CCore.Mesh.MeshFadeType.FadeOut);
                }
                RemoveEffectImmDeath();
            }
            var roleVisual = Visual as CCore.Mesh.Mesh;
            if (roleVisual != null)
            {
                roleVisual.mEdgeDetect = false;
                var roleInit = roleVisual.VisualInit as CCore.Mesh.MeshInit;
                roleInit.CastShadow = false;
                roleVisual.SetBeAttackFlashWhite(0.0f);
            }
        }

        public override void OnRemoveFromScene(SceneGraph scene)
        {
            base.OnRemoveFromScene(scene);

            if (IsChielfPlayer())
                return;

            DoLeaveMap();
        }
        #endregion

        public void InitRelationship()
        {
            var roleVisual = Visual as CCore.Component.RoleActorVisual;
            var chiefRole = CCore.Client.ChiefRoleInstance as RoleActor;
            if (chiefRole == null)
                return;

            var data = CSUtility.Data.DataTemplateManager<Byte, RoleCommonTemplate>.Instance.GetDataTemplate(0);
            if (data == null)
                return;
            if (RoleData.FactionId == UInt16.MaxValue)
            {
                roleVisual.EdgeDetectColor = data.EdgeDetectNeutral;// RoleCommonTemplateManager.Instance.CommonRole.EdgeDetectNeutral;
                return;
            }

            if (RoleData.FactionId != chiefRole.RoleData.FactionId)
            {
                roleVisual.EdgeDetectColor = data.EdgeDetectEnemy;
            }
            else
            {
                roleVisual.EdgeDetectColor = data.EdgeDetectFriend;
            }
        }

        #region 召唤物

        RoleActor mOwnerRole;
        public RoleActor OwnerRole
        {
            get { return mOwnerRole; }
            set { mOwnerRole = value; }
        }

        #endregion

        #region 战斗相关
        ChiefRoleActorController mActorController;
        public ChiefRoleActorController ActorController
        {
            get { return mActorController; }
            set { mActorController = value; }
        }

        UInt32 mAttackTargetSingle = 0;
        public virtual UInt32 AttackTargetSingle
        {
            get
            {
                return mAttackTargetSingle;
            }
            set
            {
                mAttackTargetSingle = value;
            }
        }


        List<UInt32> mHatredSingles = new List<UInt32>();
        public List<UInt32> HatredSingles
        {
            get { return mHatredSingles; }
        }

        RoleActor mSelectRole = null;
        public RoleActor SelectRole
        {
            get { return mSelectRole; }
            set { mSelectRole = value; }
        }

        #endregion

        long ProcessEnterTriggerTime = 500;
        public void ProcessEnterTrigger(SlimDX.Vector3 loc)
        {
            if (this.World != null)
            {
                ProcessEnterTriggerTime -= CCore.Engine.Instance.GetElapsedMillisecond();
                if(ProcessEnterTriggerTime<0)
                {
                    ProcessEnterTriggerTime = 500;
                    SlimDX.Vector3 vStart = loc - SlimDX.Vector3.UnitXYZ;
                    SlimDX.Vector3 vEnd = loc + SlimDX.Vector3.UnitXYZ;
                    var triggers = this.World.GetActors(ref vStart, ref vEnd, (UInt16)CSUtility.Component.EActorGameType.Trigger);
                    foreach (CCore.World.TriggerActor trigger in triggers)
                    {
                        var cData = new Trigger.TriggerProcessData_Client(trigger.Id, this);
                        trigger.ProcessTrigger(cData);
                    }
                }
            }
        }

        CSUtility.AISystem.State mCurrentState;
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.属性.当前状态", CSUtility.Helper.enCSType.Client, "获取或设置角色对象的当前状态")]
        [CSUtility.Event.Attribute.AllowMember("角色对象.属性.当前状态", CSUtility.Helper.enCSType.Client, "获取或设置角色对象的当前状态")]
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
            get
            {
                return mTargetState;
            }

            set
            {
                mTargetState = value;
            }
        }

        public CSUtility.AISystem.FStateMachine AIStates
        {
            get
            {
                return mFSM;
            }
        }

        public CSUtility.Component.ActorBase Actor
        {
            get
            {
                return this;
            }
        }

        CSUtility.Helper.LogicTimerManager mTimerManager = new CSUtility.Helper.LogicTimerManager();
        public CSUtility.Helper.LogicTimerManager TimerManager
        {
            get
            {
                return mTimerManager;
            }
        }

        Dictionary<UInt32, byte> mSummonColliderRoles = new Dictionary<UInt32, byte>();
        public Dictionary<UInt32, byte> SummonColliderRoles
        {
            get { return mSummonColliderRoles; }
            set { mSummonColliderRoles = value; }
        }

        CSUtility.Animation.AnimationTree mCurrentAnimation = null;

        ulong mCurFSMVersion;
        CSUtility.AISystem.FStateMachine mFSM = new CSUtility.AISystem.FStateMachine();
        public bool InitFSM(Guid fsmId, bool bResetCurrentState)
        {
            var tpl = CSUtility.AISystem.FStateMachineTemplateManager.Instance.GetFSMTemplate(fsmId, CSUtility.Helper.enCSType.Client);
            if (tpl == null)
                return false;
            mFSM.InitFSM(this, tpl, CSUtility.Helper.enCSType.Client);
            mCurFSMVersion = tpl.Version;

            if (bResetCurrentState)
                mCurrentState = mFSM.DefaultState;//mFSM.GetState("Idle");
            if (mCurrentState == null)
            {
                return false;
            }
            return true;
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

        public void FSMSetCurrentAnimationTree(CSUtility.Animation.AnimationTree anim)
        {
            mCurrentAnimation = anim;

            var mesh = Visual as CCore.Mesh.Mesh;
            mesh?.SetAnimTree(anim as CCore.AnimTree.AnimTreeNode);
        }

        public CSUtility.Animation.AnimationTree FSMGetAnimationTreeByActionName(string name, int blendDuration)
        {
            CSUtility.Animation.AnimationTree anim = null;
            if (anim == null && RoleTemplate != null)
            {
                var action = RoleTemplate.GetActionNamePair(name);
                if (action == null)
                {
                    return null;
                }

                //这里用 action.ActionFile 去设置动作文件
                var anim_action = new CCore.AnimTree.AnimTreeNode_Action();
                anim_action.Initialize();
                anim_action.ActionName = action.ActionFile;
                anim_action.PlayRate = action.PlayRate;
                anim_action.PlayerMode = CSUtility.Animation.EActionPlayerMode.Default;
                anim_action.XRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
                anim_action.YRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
                anim_action.ZRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Default;
                anim_action.CurNotifyTime = 0;
                anim = anim_action;

                var anim_blend = new CCore.AnimTree.AnimTreeNode();
                anim_blend.Initialize();
                anim_blend.CurNotifyTime = 0;
                anim_blend.AddNode(anim_action);
                anim_blend.BlendDuration = blendDuration;
                anim = anim_blend;
            }
            return anim;
        }

        public void FSMOnToState(CSUtility.AISystem.State curState, CSUtility.AISystem.StateParameter param, CSUtility.AISystem.State newCurState, CSUtility.AISystem.State newTarState)
        {            
            if (StateNotify2Remote)
            {
                RPC.DataWriter dwParam = new RPC.DataWriter();
                dwParam.Write(param, true);
                RPC.PackageWriter pkg = new RPC.PackageWriter();
                Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_FSMChangeState(pkg, this.SingleId, curState.StateName, newCurState.StateName, newTarState != null ? newTarState.StateName : "", dwParam);
                pkg.DoClient2PlanesPlayer(CCore.Engine.Instance.Client.GateSvrConnect);            
            }
        }

        CCore.AnimTree.AnimTreeNode_BlendPerBone mBlendActionAnim = new CCore.AnimTree.AnimTreeNode_BlendPerBone();
        public void FSMSetBlendAction(string lowHalf, string highHalf)
        {
            List<CSUtility.Animation.AnimationTree> anims = new List<CSUtility.Animation.AnimationTree>{
                FSMGetAnimationTreeByActionName(lowHalf,100),
                FSMGetAnimationTreeByActionName(highHalf,100),
            };
            mBlendActionAnim.SetAnimations(anims);

            FSMSetCurrentAnimationTree(mBlendActionAnim);
        }

        // 设置指向型法术范围Decal
        protected static CCore.World.IRegionActor mRegionActor = null;
        public static CCore.World.IRegionActor RegionActor
        {
            get
            {
                if (mRegionActor == null)
                {
                    mRegionActor = new CCore.World.IRegionActor();
                    var regionActorInit = new CCore.Component.RegionInit();
                    mRegionActor.Initialize(regionActorInit);
                    mRegionActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
                    CCore.Client.MainWorldInstance.AddEditorActor(mRegionActor);
                    mRegionActor.Visible = false;
                }
                if (mRegionActor.Visual == null)
                {
                    var regionActorInit = new CCore.Component.RegionInit();
                    mRegionActor.Initialize(regionActorInit);
                    mRegionActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
                    CCore.Client.MainWorldInstance.AddEditorActor(mRegionActor);
                }

                return mRegionActor;
            }
        }

        public CSUtility.Animation.AnimationTree FSMGetCurrentAnimationTree()
        {
            return mCurrentAnimation; ;
        }

        public CSUtility.Animation.AnimationTree CreateAnimationNode()
        {
            var anim = new CCore.AnimTree.AnimTreeNode();
            anim.Initialize();
            return anim;
        }

        public CSUtility.Animation.BaseAction CreateBaseAction()
        {
            var anim = new CCore.AnimTree.AnimTreeNode_Action();
            anim.Initialize();
            return anim;
        }

        public void SetAnimTree(CSUtility.Animation.AnimationTree animtree)
        {
            var mesh = Visual as CCore.Mesh.Mesh;
            if (mesh == null)
                return;

            var anim = animtree as CCore.AnimTree.AnimTreeNode;
            mesh.SetAnimTree(anim);
            mCurrentAnimation = anim;
        }

        Queue<CSUtility.Helper.EventCallBack> mOnStateExitQueue = new Queue<CSUtility.Helper.EventCallBack>();
        public void PushStateExit(CSUtility.Helper.EventCallBack cb)
        {
            mOnStateExitQueue.Enqueue(cb);
        }

        public CSUtility.Helper.EventCallBack PopStateExit()
        {
            if (mOnStateExitQueue.Count == 0)
                return null;
            return mOnStateExitQueue.Dequeue();
        }

        public void OnExitedState(CSUtility.AISystem.State curState)
        {

        }

        public bool OnValueChanged(string name, RPC.DataWriter value)
        {
            //这里今后处理当HP什么的变化了，需要做一些特效什么的处理
            switch (name)
            {
                case "RoleHP":
                    break;
            }
            return false;
        }

        float PrevAltitude = 0;
        float PrevX;
        float PrevZ;
        public float GetAltitude(float x, float z)
        {
            if (Math.Abs(x - PrevX) < 0.01 && Math.Abs(z - PrevZ) < 0.01)
            {
                return PrevAltitude;
            }
            PrevX = x;
            PrevZ = z;

            SlimDX.Vector3 start = new SlimDX.Vector3(x, 1000, z);
            SlimDX.Vector3 end = new SlimDX.Vector3(x, -1000, z);
            CSUtility.Support.stHitResult hitResult = new CSUtility.Support.stHitResult();
            hitResult.InitObject();
            if (this.WorldLineCheck(ref start, ref end, ref hitResult) == true)
            {
                PrevAltitude = hitResult.mHitPosition.Y;
                return hitResult.mHitPosition.Y;
            }
            PrevAltitude = 0;
            return 0;
        }

        public void SetRigidTracker(Guid tracker, long duration, bool isAbsTracker)
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
                rolePlace.Tracker.SetTracker(CCore.Engine.Instance.Client.MainWorld.Id, tracker, duration, isAbsTracker);
            }
        }

        public SlimDX.Vector3 FireDir
        {
            get
            {
                if (mActorController != null)
                {
                    //var halfHeightLoc = mActorController.PointOnMousePress - ChiefRoleActorController.Instance.mPickDir*2;
                    //var dir = halfHeightLoc - Placement.GetLocation();
                    var dir = mActorController.CastPointOnMousePress - Placement.GetLocation();
                    dir.Y = 0;
                    dir.Normalize();
                    return dir;
                }
                return SlimDX.Vector3.UnitY;
            }
        }

        SlimDX.Vector3 mRoleFireDir = SlimDX.Vector3.UnitY;
        public SlimDX.Vector3 RoleFireDir
        {
            get { return mRoleFireDir; }
            set { mRoleFireDir = value; }
        }

        [CSUtility.AISystem.Attribute.AllowMember("RoleActor.发射技能", CSUtility.Helper.enCSType.Client, "")]
        public void OnFireSkill(GameData.Skill.SkillData skill)
        {
            if (this.IsChielfPlayer() == false || skill == null || this.CurrentState.StateName == "Death")
                return;
            if (Skill.SkillController.Instance.mLockOnRole != null)
            {
                var dir = Skill.SkillController.Instance.mLockOnRole.Placement.GetLocation() - Placement.GetLocation();
                dir.Y = 0;
                //dir.Normalize();
                var length = dir.Length();
                var radius = skill.Template.AttackRadius + RoleTemplate.Radius + Skill.SkillController.Instance.mLockOnRole.RoleTemplate.Radius;
                if (length > radius)
                {
                    Skill.SkillController.Instance.mLockOnRole = null;
                }
            }
            var summonPos = Placement.GetLocation();
            summonPos.Y += 1;
            if (this.CurrentState.CanInterrupt == false)//在放技能
            {
                if (CurrentState.SkillCanInterrupt == true)//技能可打断
                {
                }
                else
                    return;//技能不可打断
            }
            _DoFireSkill(skill, summonPos);
        }

        void _DoFireSkill(GameData.Skill.SkillData skill, SlimDX.Vector3 summonPos)
        {
            if (!Stage.MainStage.Instance.CanFire(skill))
                return;
            var angle = this.Placement.GetDirection();
            var nowloc = this.Placement.GetLocation();
            var dir2 = this.Placement.GetRotationYVec();
            var mCurrentFireDir = new SlimDX.Vector3(dir2.X, 0, dir2.Y); 
            var lockid = Skill.SkillController.Instance.GetLockRoleId(skill);
            var tarPos = nowloc + mCurrentFireDir * skill.Template.ThrowRoleDistance;
            if (Skill.SkillController.Instance.mLockOnRole != null)
                tarPos = Skill.SkillController.Instance.mLockOnRole.Placement.GetLocation();

            if (skill.Template.SkillOperation == GameData.Skill.ESkillOperationType.Skillshot && lockid == UInt32.MaxValue)
                return;

            RPC.PackageWriter pkg = new RPC.PackageWriter();
            Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_FireInitiativeSkill(pkg, skill.TemplateId, lockid, summonPos, tarPos, mCurrentFireDir, angle);
            pkg.WaitDoClient2PlanesPlayer(CCore.Engine.Instance.Client.GateSvrConnect, -1).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                Role.ChiefRoleActorController.Instance.FireSkillReady = true;

                if (bTimeOut)
                    return;
                sbyte successed = -1;
                _io.Read(out successed);
                switch (successed)
                {
                    case 1:
                        {
                            //这里默认客户端和服务器同步，客户端过来服务器也没问题
                            Stage.MainStage.Instance.AfterFire(skill);

                            //     Placement.SetRotationY(mCurrentFireDir.Z, mCurrentFireDir.X, RoleTemplate.MeshFixAngle,true);
                        }
                        break;
                    case 2:
                        {
                            //
                        }
                        break;
                    default:
                        break;
                }
            };
        }

        double mCurrAngle = 0;
        public void TickRimEffect(long elapsedMillisecond)
        {
            var roleVisual = Visual as CCore.Component.RoleActorVisual;
            if (roleVisual == null)
                return;

            float rimStart = 0.5f;
            float rimEnd = 1;
            SlimDX.Vector4 rimColor = new SlimDX.Vector4(1, 1, 1, 1);
            float rimMultiply = 1;
            int rimBloom = 0;
            //             var cellScene = CCore.Client.MainWorldInstance as FrameSet.Scene.CellScene;
            //             if (cellScene != null)
            //             {
            //                 if (cellScene.SunActor != null && cellScene.SunActor.Light != null)
            //                 {
            //                     if (FrameSet.WeatherSystem.IlluminationManager.Instance.IsNight == true)
            //                         MidLayer.IUtility.IConverter.DrawColor2Vector(cellScene.SpotLightActor.Light.Diffuse, out rimColor);
            //                     else
            //                         MidLayer.IUtility.IConverter.DrawColor2Vector(cellScene.SunActor.Light.Diffuse, out rimColor);
            //                 }
            //             }
            
            // Boss
            if (RoleData.MonsterData != null)
            {
                var data = CSUtility.Data.DataTemplateManager<Byte, RoleCommonTemplate>.Instance.GetDataTemplate(0);
                if(data != null)
                {
                    rimColor = data.MonsterLight;
                    rimStart = data.RimStart;
                    rimEnd = data.RimEnd;
                    rimMultiply = 0.7f;
                    rimBloom = 1;
                
                    var cycle = data.RimCycle;
                    var cycleStart = data.RimCycleStart;
                    mCurrAngle += (double)elapsedMillisecond / 1000.0 * System.Math.PI / 2.0 / cycle;
                    rimMultiply = (float)((System.Math.Sin(mCurrAngle) + 1.0) / 2.0 + cycleStart);
                }
            }
            
            roleVisual.SetRimLightParameter(rimStart, rimEnd, rimColor, rimMultiply, rimBloom);            
            roleVisual.UpdateRimParameter();            
        }

        public void TestDoLeaveMap(long elapsedMillisecond)
        {
            float DistanceToChielfRole = RPC.RPCNetworkMgr.Sync2ClientRangeSq * 3;
            if (!CCore.Client.MainWorldInstance.IsNullWorld && CCore.Client.MainWorldInstance.Camera != null)
            {
                if (Placement != null)
                {
                    var loc1 = Placement.GetLocation();
                    var crole = CCore.Client.ChiefRoleInstance;
                    if (!crole.IsNullActor)
                    {
                        var loc2 = crole.Placement.GetLocation();
                        DistanceToChielfRole = SlimDX.Vector3.DistanceSquared(loc1, loc2);
                    }
                }
            }

            if (mIsLeaveMapByDistance == true)
            {
                if (DistanceToChielfRole > RPC.RPCNetworkMgr.Sync2ClientRangeSq /** 1.2 */&& CCore.Client.ChiefRoleInstance != this/* && this.RoleTemplate.RPC2ClientIgnoreDis == false*/)
                {
                    DoLeaveMap();
                }
            }

            if (this.RoleData != null)
            {
                this.RoleData.LiveTime -= ((float)elapsedMillisecond) * 0.001F;
                if (this.RoleData.LiveTime <= 0 && this.IsLeaveMap == false)
                {
                    if (CurrentState != null)
                        CurrentState.ToState("Death", null);
                    else
                        DoLeaveMap();
                }
            }

            if (RoleData != null && this.RoleData.RoleType == Role.EClientRoleType.Monster && this.SingleId == 0)//npc的singled==0是不正常的
                DoLeaveMap();
        }

        public bool mIsLeaveMapByDistance = true;
        long mFlashWhiteDuration = 200;
        long mFlashWhiteTime = 0;
        public long mForceLeaveMapTime = 5000;
        
        static CSUtility.Performance.PerfCounter mLogicActorTimer = new CSUtility.Performance.PerfCounter("RoleActor.Tick");
        public override void Tick(long elapsedMillisecond)
        {
            mLogicActorTimer.Begin();
            CCore.Engine.Instance.TickActorNumber++;

            if (this.RoleData != null)
            {
                if (mRoleTemplateVersion != mRoleData.TemplateVersion)
                {
                    mRoleTemplateVersion = mRoleData.TemplateVersion;

                    SlimDX.Vector3 scale = new SlimDX.Vector3(mRoleData.RoleTemplate.Scale);
                    this.Placement.SetScale(ref scale);
                }
            }

            var mesh = Visual as CCore.Mesh.Mesh;
            if (mesh != null)
            {
                if (mesh.GetBeAttackFlashWhite() > 0)
                {
                    mFlashWhiteTime += elapsedMillisecond;
                    if (mFlashWhiteTime > mFlashWhiteDuration)
                    {
                        mesh.SetBeAttackFlashWhite(0.0f);
                    }
                    else
                    {
                        mesh.SetBeAttackFlashWhite(1.0f - (float)mFlashWhiteTime / (float)mFlashWhiteDuration);
                    }
                }
            }

            if (IsCurFrameRender() == false)
            {                
                CCore.Engine.Instance.ActorNoVisualTickNumber++;
            }
            else
            {
                base.Tick(elapsedMillisecond);
            }
            
            TimerManager.Tick(elapsedMillisecond);            

            if (mFSM.FSMTemplate != null && mCurFSMVersion != mFSM.FSMTemplate.Version)
            {
                InitFSM(mFSM.FSMTemplate.Id, true);
            }

            if (CurrentState != null)
            {
                CurrentState.Tick(elapsedMillisecond);
            }

            if (mReadyToLeaveMap)
            {
                mForceLeaveMapTime -= elapsedMillisecond;
                var parentMesh = Visual as CCore.Mesh.Mesh;
                if (parentMesh != null)
                {
                    if (!parentMesh.mStartFadeOut || mForceLeaveMapTime < 0)
                    {
                        mForceLeaveMapTime = 5000;
                        mIsLeaveMap = true;
                        mReadyToLeaveMap = false;
                        Title.TitleShowManager.Instance.RemoveTitle(mSingleId);
                    }
                }
            }            
                                    
            //TickRimEffect(elapsedMillisecond);                        
            SummorTick(elapsedMillisecond);            
            TryQueryLocation(elapsedMillisecond);            
            //TickSkillCD(elapsedMillisecond);     
            TickRelife(elapsedMillisecond);

            mLogicActorTimer.End();
        }

        long mLastQueryPositionTime = 0;
        private void TryQueryLocation(long elapsedMillisecond)
        {
            if (this.World != CCore.Client.ChiefRoleInstance.World)
            {
                return;
            }
            if (this.RoleData == null)
                return;
            if (AllowServerFixPosition() == false)
                return;

            mLastQueryPositionTime -= elapsedMillisecond;
            if (mLastQueryPositionTime < 0 && this != CCore.Client.ChiefRoleInstance)
            {
                mLastQueryPositionTime = 3000;

                UpdateClientPos();
            }
        }

        public bool AllowServerFixPosition()
        {
            if (this.IsChielfPlayer())
            {
                return false;
            }
            else if (RoleData != null)
            {
                var smData = RoleData.SummonData;
                if (smData != null && smData.IsUpdatePos2Client == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateClientPos()
        {
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_GetRolePosition(pkg, this.SingleId);
            pkg.WaitDoClient2PlanesPlayer(CCore.Engine.Instance.Client.GateSvrConnect, 3).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;
                sbyte successed = -1;
                _io.Read(out successed);
                switch (successed)
                {
                    case 1:
                        SlimDX.Vector3 pos;
                        _io.Read(out pos);
                        pos.Y = this.Placement.GetLocation().Y;
                        var distance = SlimDX.Vector3.Distance(this.Placement.GetLocation(), pos);
                        if (distance > 3)
                        {
                            //Log.FileLog.WriteLine("更改客户端位置，serverpos:{0},clientPos:{1},rolestate:{2},distance:{3}", pos, this.Placement.GetLocation(), this.CurrentState.StateName,distance);
                            pos.Y = this.GetAltitude(pos.X, pos.Z);
                            this.Placement.SetLocation(ref pos);
                            //      if (this.CurrentState.StateName =="Walk")
                            //       {
                            //                                 if (this.CurrentState.StateName == "Walk")
                            //                                     FreshWalkState(CurrentState as ClientStates.Walk);
                            //        }
                            //        else
                            //         {
                            //                                 var walkState = this.AIStates.GetState("Walk") as ClientStates.Walk;
                            //                                 if (walkState != null)
                            //                                 {
                            //                                     CSCommon.AISystem.States.IWalkParameter walkParam = walkState.Parameter as CSCommon.AISystem.States.IWalkParameter;
                            //                                     walkParam.TargetPosition = pos;
                            //                                     walkParam.TargetPositions.Clear();
                            // 
                            //                                     walkParam.MoveSpeed = MoveSpeed;//distance / FixPositionTime;//(0.5/0.4)
                            //                                     this.CurrentState.ToState("Walk", walkParam);
                            //                                 }
                            //                                 else
                            //                                 {
                            //                                     
                            //                                 }
                            //           }
                        }
                        break;
                    case -1:
                        DoLeaveMap();
                        break;
                }
            };
        }

        public void TickSkillCD(long elapsedMillisecond)
        {
            foreach (var skill in RoleData.PlayerData.Skills)
            {
                if (skill.RemainCD > 0)
                    skill.RemainCD -= (float)elapsedMillisecond / 1000f;
                if (skill.RemainCD < 0)
                    skill.RemainCD = 0;
            }
        }

        public void SummorTick(long elapsedMillisecond)
        {
            if (RoleData.RoleType != EClientRoleType.Summon)
                return;

            if (RoleData.LiveTime <= 0)
                this.CurrentState.ToState("Death", null);
            else
                RoleData.LiveTime -= (float)elapsedMillisecond * 0.001f;
          //  RemoveEffectImmDeath();
        }

        public void RemoveEffectImmDeath()
        {
            if (mReadyToLeaveMap)
            {
                var parentMesh = Visual as CCore.Mesh.Mesh;
                parentMesh.SocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent socket, object arg) =>//DoLeaveMap后立刻移除要立刻死亡的特效
                {
                    var effect = socket as CCore.Component.EffectVisual;
                    if (effect != null)
                    {
                        effect.RemoveNotDelayDeath();
                    }
                    return CSUtility.Support.EForEachResult.FER_Continue;
                }, null);
            }
        }

        public bool CanAttack(RoleActor target)
        {
            if (target.RoleData.RoleType == EClientRoleType.Monster)
            {
                if (RoleData.FactionId != target.RoleData.FactionId)
                    return true;
            }

            return false;
        }

        public void TickCollisionPlayer()
        {
            if (!this.RoleTemplate.CalCollission)
                return;

            if (this.CurrentState.StateName == "Death")
                return;

            float mRadius = 3.0f;
            var pos = this.Placement.GetLocation();
            var startPos = new SlimDX.Vector3(pos.X - mRadius, pos.Y , pos.Z - mRadius);
            var endPos = new SlimDX.Vector3(pos.X + mRadius, pos.Y , pos.Z + mRadius);

            var roles = CCore.Engine.Instance.Client.MainWorld.GetActors(ref startPos, ref endPos, (UInt16)CSUtility.Component.EActorGameType.Player);

            pos.Y = 0;
            foreach(var role in roles)
            {
                var rolePos = role.Placement.GetLocation();
                rolePos.Y = 0;                
                if (SlimDX.Vector3.Distance(pos,rolePos) < 0.5f)
                {
                    if (this.RoleTemplate.OnCollisionCB != null)
                    {
                        var callee = this.RoleTemplate.OnCollisionCB.GetCallee() as GameData.Role.FOnCollision;
                        callee?.Invoke(this,role);
                    }
                    break;
                }
            }
        }

        #region 渲染相关

        public CCore.Mesh.Mesh mEffectMesh = null;
        public override void OnCommitVisual(REnviroment env, ref Matrix matrix, CameraObject eye)
        {
            if (IsLeaveMap || RoleData.RoleTemplate == null)
                return;

            SlimDX.Matrix absMat;
            Placement.GetAbsMatrix(out absMat);
            SlimDX.Vector3 pos;
            pos.X = absMat.M41;
            pos.Y = absMat.M42;
            pos.Z = absMat.M43;
            pos.Y += RoleData.RoleTemplate.HalfHeight * 2;
            var screenPt = eye.GetScreenCoord(ref pos, Client.Game.Instance.GInit.Vector2ScreenCoordScale);
            if (screenPt != SlimDX.Vector3.Zero)
            {
                Title.TitleShowManager.Instance.Add(screenPt.X, screenPt.Y, this, eye);
            }

            if (mEffectMesh != null)
                mEffectMesh.Commit(env, ref matrix, eye);
        }

        #endregion

        #region 特效
        void TickEffect(long elapsedMillisecond)
        {
            var parentMesh = Visual as CCore.Mesh.Mesh;
            if (parentMesh == null)
                return;

            parentMesh.SocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent socket, object arg) =>
            {
                var effect = socket as CCore.Component.EffectVisual;
                if (effect != null)
                {
                    if(effect.IsFinished())
                    {
                        //parentMesh.RemoveSocketItem(id);
                        return CSUtility.Support.EForEachResult.FER_Erase;
                    }
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }
        public void AddEffect(List<GameData.Skill.SkillTemplate.NotifyEffect> notifyEffect, string notifyName,bool addSame =false)
        {
            if (notifyEffect == null || notifyEffect.Count == 0)
                return;
            var parentMesh = Visual as CCore.Mesh.Mesh;
            if (parentMesh == null)
                return;
            List<CCore.Component.EffectVisual> mEffects = new List<CCore.Component.EffectVisual>();
            foreach (var ne in notifyEffect)
            {
                if (!string.IsNullOrEmpty(ne.mNotifyName))
                {
                    var name1 = notifyName.ToLower();
                    var name2 = ne.NotifyName.ToLower();
                    if (name1 != name2)
                        continue;
                }
                foreach (var se in ne.SocketEffects)
                {
                    foreach (var e in se.Effects)
                    {
                        if (e.EffectId != Guid.Empty)
                        {
                            var effect = new CCore.Component.EffectVisual();
                            var effectInit = new CCore.Component.EffectVisualInit();
                            effectInit.CanHitProxy = false;
                            effectInit.SocketName = se.SocketName;
                            effectInit.EffectTemplateID = e.EffectId;
                            effectInit.Duration = e.Duration;
                            effectInit.InheritRotate = e.InheritRotate;
                            effectInit.InheritRotateWhenBorn = e.InheritRotateWhenBorn;
                            effectInit.Pos = e.Pos;
                            effectInit.Scale = new SlimDX.Vector3(e.Scale, e.Scale, e.Scale);
                            effectInit.Rotate = e.Rotate;
                            effect.Initialize(effectInit, this);
                            if(addSame==false)
                                RemoveEffectsbyId(effect.EffectTemplateID);
                            parentMesh.AddSocketItem(effect.EffectInit);
                        }
                    }
                }
            }
        }
        public void RemoveEffectsbyId(Guid effictid)
        {
            var parentMesh = Visual as CCore.Mesh.Mesh;
            parentMesh.SocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent socket, object arg) =>
            {
                var effect = socket as CCore.Component.EffectVisual;
                if (effect != null)
                {
                    if(effect.EffectTemplateID ==effictid)
                    {
                        return CSUtility.Support.EForEachResult.FER_Erase;
                    }
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }
        public void RemoveEffects(List<GameData.Skill.SkillTemplate.NotifyEffect> notifyEffect,string notifyname)
        {
            var parentMesh = Visual as CCore.Mesh.Mesh;
            foreach (var notify in notifyEffect)
            {
                if (!string.IsNullOrEmpty(notifyname) && notify.NotifyName != notifyname)
                    continue;
                foreach (var socket in notify.SocketEffects)
                {
                    foreach (var effect in socket.Effects)
                    {
                        if (effect.EffectId == Guid.Empty)
                        {
                            return;
                        }
                        RemoveEffectsbyId(effect.EffectId);
                    }
                }
            }
        }
        #endregion

        #region RPC
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_FSMChangeState(string curState, string newCurState, string newtarState, RPC.DataReader parameter, RPC.RPCForwardInfo fwd)
        {
            if (this.CurrentState == null)
                return;

            var remote = this.StateNotify2Remote;
            this.StateNotify2Remote = false;            
            if (this.IsChielfPlayer())
            {
                //Log.FileLog.WriteLine("服务器要求主角改变状态{0}", newCurState);
                //System.Diagnostics.Trace.WriteLine(string.Format("服务器要求主角改变状态{0}", newCurState));
            }
            else
            {
                this.CurrentState.CanInterrupt = true;
            }
            if (!IsChielfPlayer() && this.CurrentState.StateName == "Death" && newCurState != "Death")
            {              
                return;
            }
            //服务器通知客户端有Role状态改变在这里处理
            var state = AIStates.GetState(newCurState);
            if (state == null)
            {
                //这里可以考虑转换成为一个Idle状态
                this.CurrentState.ToState("Idle", null);
                return;
            }
            var  param = state.Parameter;
            if (param == null)
            {
                //这里可以考虑转换成为一个Idle状态
                this.CurrentState.ToState("Idle", null);
                return;
            }

            parameter.Read(param, true);

            if (this.CurrentState != null)
            {
                if (newCurState == "BeAttack")
                {
                    //Client2BeAttack(newCurState, param);
                }
                else
                {
                   // if (newCurState == "StayAttack")
                   //     Log.FileLog.WriteLine(string.Format("ToAttack role:{0} time:{1}",this.SingleId,CSUtility.Helper.LogicTimer.GetTickCount()));
                    this.CurrentState.ToState(newCurState, param);
                }
            }
            this.StateNotify2Remote = remote;
        }


        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false, true)]
        public void RPC_UpdatePosition(Int16 x, Int16 z)
        {
            SlimDX.Vector3 pos;
            pos.X = ((float)x) * 0.1F;
            pos.Z = ((float)z) * 0.1F;

            pos.Y = Placement.GetLocation().Y;

            if (this.SingleId ==Stage.MainStage.Instance.ChiefRole.SingleId)
            {                
                Log.FileLog.WriteLine("RPC ChiefRole UpdatePosition");
                return;
            }
            if (this.World == null)
            {
                CCore.Engine.Instance.Client.MainWorld.AddActor(this);
                Log.FileLog.WriteLine("RPC_UpdatePosition的时候真的发生了这个对象没有进入World的情况");
            }
            if(this.RoleData.RoleType ==EClientRoleType.Summon)
            {
                //Log.FileLog.WriteLine("RPC_UpdatePosition服务器更新summon位置nowpos:{0} x:{1},z:{2}",this.Placement.GetLocation(),x,z);
                return;
            }

            SlimDX.Vector3 dist = pos - Placement.GetLocation();
            dist.Y = 0;
            
            if (dist.Length() > 1f)
            {
                if (CurrentState.StateName == "Death")
                {
                    this.CurrentState.ToState("Death", null);
                    return;
                }
                //Log.FileLog.WriteLine("RPC_UpdatePosition服务器更新role:{1} dist:{0}", dist.Length(), this.RoleTemplate.RoleName);
                //Log.FileLog.WriteLine("role:{0} state{1},serverPos:{2},clientPos{3}",this.RoleTemplate.RoleName,this.CurrentState.StateName,pos,Placement.GetLocation());
                //这个地方应该一直hold，知道改正位置
                RPC.PackageWriter pkg = new RPC.PackageWriter();
                Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_UpDateRoleState(pkg, this.SingleId);
                pkg.DoClient2PlanesPlayer(CCore.Engine.Instance.Client.GateSvrConnect);

                pos.Y = GetAltitude(pos.X,pos.Z);
                Placement.SetLocation(ref pos);          
            }
            else
            {
                pos.Y = GetAltitude(pos.X, pos.Z);
                Placement.SetLocation(ref pos);
            }
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_UpdateCriticalValue(uint attackerid, uint targetId, UInt16 skillid,int value, Byte hitType)
        {
            //var role = RoleManager.Instance.FindRoleActor(targetId);
//             var attack = RoleManager.Instance.FindRoleActor(attackerid);
//             if (attack == null)
//                 return;
            var parentMesh = this.Visual as CCore.Component.RoleActorVisual;
            if (parentMesh == null)
                return;
            if (parentMesh.TransPercent == 0)//隐身单位就不显示伤害数字了
                return;

            var skilltemp = CSUtility.Data.DataTemplateManager<UInt16, GameData.Skill.SkillTemplate>.Instance.GetDataTemplate(skillid);// GameData.Skill.SkillTemplateManager.Instance.FindSkill(skillid);
            if(skilltemp !=null)
            {
                AddEffect(skilltemp.BeAttackNotifyEffects,null);
            }

            Title.HitShowManager.Instance.ShowHit((Title.enHitType)hitType, value.ToString(), Game.Instance.REnviroment.Camera, this);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_AddBuff(GameData.Skill.BuffData data)
        {
            BuffBag.CreateAddBuff(data);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_RemoveBuff(GameData.Skill.BuffData data)
        {
            BuffBag.RemoveBuff(data.BuffId);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_FreshBuff(Guid  id,long time)
        {
            BuffBag.FreshBuffLiveTime(id,time);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_UpdateRoleValue(string name, RPC.DataReader value)
        {
            if (mRoleData == null)
                return;

            switch (name)
            {
                case "RoleHp":
                    int roleHp;
                    value.Read(out roleHp);
                    mRoleData.RoleHP = roleHp;
                    break;
                case "RoleMp":
                    int roleMp;
                    value.Read(out roleMp);
                    mRoleData.RoleMP = roleMp;
                    break;
                case "MaxRoleHp":
                    int maxRoleHp;
                    value.Read(out maxRoleHp);
                    mRoleData.RoleMaxHP = maxRoleHp;
                    break;
                case "MaxRoleMp":
                    int maxRoleMp;
                    value.Read(out maxRoleMp);
                    mRoleData.RoleMaxMP = maxRoleMp;
                    break;
                case "RoleLevel":
                    byte roleLevel;
                    value.Read(out roleLevel);
                    mRoleData.RoleLevel = roleLevel;
                    break;
                case "RoleExp":
                    long roleExp;
                    value.Read(out roleExp);
                    mRoleData.PlayerData.RoleExp = roleExp;
                    break;
                case "RoleGod":
                    int roleGold;
                    value.Read(out roleGold);
                    mRoleData.PlayerData.RoleGold = roleGold;
                    break;
                case "RoleMoveSpeed":
                    float roleMoveSpeed;
                    value.Read(out roleMoveSpeed);
                    mRoleData.RoleMoveSpeed = roleMoveSpeed;
                    break;
                case "RoleSkillPoint":
                    byte roleSkillPoint;
                    value.Read(out roleSkillPoint);
                    mRoleData.PlayerData.RoleSkillPoint = roleSkillPoint;
                    break;
                case "Unrivaled":
                    bool unrivaled;
                    value.Read(out unrivaled);
                    mRoleData.MonsterData.Unrivaled = unrivaled;
                    break;
            }

            Stage.MainStage.Instance.UpdateRoleValue(name);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_LevelUpEffect()
        {
            AddEffect("8a90610c-e0ff-4f55-9d85-00615e6a75f1", 0, 0, 0);
            //effectVisInit.EffectTemplateID = 
            //var cellScene = CCore.Client.MainWorldInstance as CCore.Scene.CellScene;
            //if (cellScene != null)
            //{
            //    var effectVisual = cellScene.LvlupEffectActor.Visual as CCore.Effect.EffectVisual;
            //    effectVisual.EffectInit.EffectTemplate.Reset();
            //    effectVisual.Visible = true;
            //}
        }

        //         CCore.Component.Decal Decal = new CCore.Component.Decal();
        //         public override void OnCommit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        //         {
        //             base.OnCommit(renderEnv, ref matrix, eye);
        // 
        //             var scaleMat = SlimDX.Matrix.Scaling(RoleTemplate.Radius, RoleTemplate.Radius, RoleTemplate.Radius);
        //             var transMat = scaleMat * SlimDX.Matrix.Translation(Placement.GetLocation());
        //             Decal.Commit(renderEnv, ref transMat, eye);
        //         }

        [CSUtility.Event.Attribute.AllowMember("角色对象.添加特效", CSUtility.Helper.enCSType.Client, "AddEffect")]
        public void AddEffect(string effectId, float offsetX, float offsetY, float offsetZ, float scale = 1f)
        {
            if (effectId == "")
                return;
            var effVisInit = new CCore.Component.EffectVisualInit()
            {
                EffectTemplateID = CSUtility.Support.IHelper.GuidParse(effectId),
                CanHitProxy = false
            };
            var effVis = new CCore.Component.EffectVisual();
            effVis.Initialize(effVisInit, null);

            var effActorInit = new CCore.World.EffectActorInit()
            {
                ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager
            };
            var effActor = new CCore.World.EffectActor();
            effActor.Initialize(effActorInit);
            effActor.Visual = effVis;
            effActor.LoopPlay = false;

            effActor.SetPlacement(new CSUtility.Component.StandardPlacement(effActor));
            SlimDX.Matrix matrix = SlimDX.Matrix.Identity;
            this.Placement.GetAbsMatrix(out matrix);
            effActor.Placement.SetMatrix(ref matrix);

            var tempLoc = effActor.Placement.GetLocation() + new SlimDX.Vector3(offsetX,offsetY,offsetZ);
            effActor.Placement.SetLocation(ref tempLoc);
            effActor.Placement.SetScale(new SlimDX.Vector3(scale,scale,scale));

            this.World.AddActor(effActor);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_RoleCollision(UInt32 targetSingleId)
        {
            var target = RoleManager.Instance.FindRoleActor(targetSingleId);
            if (target == null)
                return;
            if (this.RoleTemplate.OnCollisionCB != null)
            {
                var callee = this.RoleTemplate.OnCollisionCB.GetCallee() as GameData.Role.FOnCollision;
                callee?.Invoke(this, target);                
            }                                                         
        }
        #endregion

        public void ProcDeath()
        {
            if (RoleTemplate.OnRoleDeathCB != null)
            {
                var callee = RoleTemplate.OnRoleDeathCB.GetCallee() as GameData.Role.FOnRoleDeath;
                callee?.Invoke(this);
            }

            if (IsChielfPlayer())
            {
                RelifeTime = RoleData.PlayerData.RoleLevel * 2000;
                UI.MainUIManager.Instance.ShowDeathUI(true);
            }
            
        }

        Int64 mRelifeTime = 0;
        Int64 RelifeTime
        {
            get { return mRelifeTime; }
            set
            {
                mRelifeTime = value;
                UI.MainUIManager.Instance.RelifeTimeStr = ((mRelifeTime / 1000) + 1).ToString();
            }
        }

        void TickRelife(Int64 elapsedMillisecond)
        {
            if (RelifeTime > 0)
            {
                RelifeTime -= elapsedMillisecond;
                if (RelifeTime <= 0)
                {
                    Reborn();
                    UI.MainUIManager.Instance.ShowDeathUI(false);
                }
            }
        }

        private bool Reborn()
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
            if (RoleData.PlayerData != null)
                Placement.SetLocation(RoleData.PlayerData.OriPosition);

            RPC.PackageWriter pkg = new RPC.PackageWriter();
            Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_RelifeMe(pkg);
            pkg.DoClient2PlanesPlayer(CCore.Engine.Instance.Client.GateSvrConnect);
            return true;
        }       

        [CSUtility.Event.Attribute.AllowMember("结束游戏", CSUtility.Helper.enCSType.Client, "结束游戏")]
        public void GameOver()
        {
            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("ResultUI") as UISystem.WinForm;
            if (form == null)
                return;
            form.Parent = Game.Instance.RootUIMsg.Root;
            var winBtn = form.FindControl("win") as UISystem.Button;
            var loseBtn = form.FindControl("lose") as UISystem.Button;            
            if (this.RoleData.FactionId == Stage.MainStage.Instance.ChiefRole.RoleData.FactionId)
            {
                winBtn.Visibility = UISystem.Visibility.Collapsed;
                loseBtn.Visibility = UISystem.Visibility.Visible;
            }
            else
            {
                winBtn.Visibility = UISystem.Visibility.Visible;
                loseBtn.Visibility = UISystem.Visibility.Collapsed;
            }

            //System.Threading.Thread.CurrentThread.Suspend();
        }
    }
}
