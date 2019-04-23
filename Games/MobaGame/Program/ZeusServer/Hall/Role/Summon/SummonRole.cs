using System;
using System.Collections.Generic;
using System.Text;
using GameData.Role;
using GameData.Skill;

namespace Server.Hall.Role.Summon
{
    [RPC.RPCClassAttribute(typeof(SummonRole))]
    public class SummonRole : RoleActor, RPC.RPCObject
    {
        ~SummonRole()
        {
            SummonNumber--;
        }

        RoleActor mOwnerRole;
        [CSUtility.Event.Attribute.AllowMember("", CSUtility.Helper.enCSType.Server, "")]
        [CSUtility.AISystem.Attribute.AllowMember("", CSUtility.Helper.enCSType.Server, "")]
        public override RoleActor OwnerRole
        {
            get
            {
                return mOwnerRole;
            }
            set { mOwnerRole = value;}
        }

        public override bool AIServerControl
        {
            get
            {
                if (mOwnerRole.GetType() != typeof(Player.PlayerInstance))
                    return true;
                return false;
            }
        }

        public bool _SetOwnerRole(RoleActor owner)
        {
            var player = owner as Player.PlayerInstance;
            //if (player == null)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}

            mOwnerRole = owner;
            if (mOwnerRole == null)
            {
                return false;
            }
            if (mOwnerRole.GetType() == typeof(SummonRole))
            {
                return false;
            }

            if (mOwnerRole.Summons.Count >= 10)
            {//召唤生物不允许超过10个
                var role = mOwnerRole.Summons[0];
                mOwnerRole.RemoveSummon(role);
                role.HostMap.RoleManager.SummonManager.UnmapRole(role);
            }
            mOwnerRole.AddSummon(this);
            owner.HostMap.RoleManager.SummonManager.MapRole(this);
            return true;
        }

        #region 伤害计算
        bool mIsDamaged = false;
        public bool IsDamaged
        {
            get { return mIsDamaged; }
            set { mIsDamaged = value; }
        }

        int mDamageCount = 0;//伤害的role的数量
        public int DamageCount
        {
            get { return mDamageCount; }
            set { mDamageCount = value; }
        }

        int mDamageNumber = 0;//打击次数
        public int DamageNumber
        {
            get { return mDamageNumber; }
            set { mDamageNumber = value; }
        }
        Int64 mFirstDamageTime = 0;
        public Int64 FirstDamageTime//初次伤害时间
        {
            get { return mFirstDamageTime; }
            set { mFirstDamageTime = value; }
        }

        Int64 mDamageCurTime = 0;
        public Int64 DamageCurTime//造成伤害的时间
        {
            get { return mDamageCurTime; }
            set { mDamageCurTime = value; }
        }

        Int64 mDamageDelayTime = -1;
        public Int64 DamageDelayTime//伤害延迟时间，从造成伤害是算起
        {
            get { return mDamageDelayTime; }
            set { mDamageDelayTime = value; }
        }
        Int64 mBornTime = 0;
        public Int64 BornTime//伤害延迟时间，从造成伤害是算起
        {
            get { return mBornTime; }
            set { mBornTime = value; }
        }

        Int64 mGapDamageTime = 0;//伤害时间间隔
        public Int64 GapDamageTime
        {
            get { return mGapDamageTime; }
            set { mGapDamageTime = value; }
        }

        bool mBoolAddBuff = false;
        public bool BoolAddBuff
        {
            get { return mBoolAddBuff; }
            set { mBoolAddBuff = value; }
        }

        List<UInt32> mSkillAddBuffRoles = new List<UInt32>();
        public List<UInt32> SkillAddBuffRoles
        {
            get { return mSkillAddBuffRoles; }
            set { mSkillAddBuffRoles = value; }
        }

        public bool CheckNeedAddBuff(UInt32 rolesingleid)
        {
            for (UInt16 i = 0; i < SkillAddBuffRoles.Count; i++)
            {
                if (SkillAddBuffRoles[i] == rolesingleid)
                    return false;
            }
            SkillAddBuffRoles.Add(rolesingleid);
            return true;
        }

        #endregion


        #region 重载
        public override void OnEnterMap(Map.MapInstance map)
        {
            base.OnEnterMap(map);
            //把角色放到SceneGraph里面去

            map.AddSummon(this);
        }

        public override void DangrouseOnLeaveMap()
        {
            if (HostMap.IsNullMap)
            {
                HostMap.RemoveSummon(this);
            }
            base.DangrouseOnLeaveMap();
        }

        //protected CSUtility.Support.EForEachResult OnVisitRole_ProcessTrigger(UInt32 singleId, Role.RoleActor role, object arg)
        //{
        //    var trigger = role as Trigger.TriggerInstance;

        //    if (trigger != null)
        //        trigger.ProcessActorEnter(this);

        //    return CSUtility.Support.EForEachResult.FER_Continue;
        //}

        public override void OnPlacementUpdatePosition(ref SlimDX.Vector3 pos)
        {
            base.OnPlacementUpdatePosition(ref pos);
            SummonData.Position = pos;

            // Trigger计算
            if (!HostMap.IsNullMap)
            {
                UInt32 actorTypes = (1 << (Int32)ERoleType.Trigger);
                HostMap.TourRoles(actorTypes, ref pos, 1, this.OnVisitRole_ProcessTrigger, null);
            }
        }

        public override void OnPlacementUpdateDirectionY(float angle)
        {
            base.OnPlacementUpdateDirectionY(angle);
            SummonData.Direction = angle;
        }

        public override void OnRoleWalkToBlock()
        {
            //summon.AIServerControl
            //var skill = SummonData.SkillData.Template;
            if (SummonData.SkillData.Template.IsCollideWithScene)
            {
                CSUtility.AISystem.State.TargetToState(this, "Death", null);
            }
        }

        public override bool CanAttack(Role.RoleActor target)
        {
            if (!base.CanAttack(target))
            {
                return false;
            }
            if (OwnerRole.FactionId != target.FactionId)
                return true;
            return false;
        }

        //         public override CSCommon.Data.RoleTemplate RoleTemplate
        //         {
        //             get { return mSummonData.Template; }
        //         }

        #endregion

        SummonData mSummonData;
        public SummonData SummonData
        {
            get { return mSummonData; }
        }

        public override Guid Id
        {
            get
            {
                return mSummonData.RoleId;
            }
        }

        public override RoleTemplate RoleTemplate
        {
            get
            {
                return mSummonData.RoleTemplate;
            }
        }

        private bool InitSummonInstance(SummonData nd)
        {
            mSummonData = nd;

            mDamageClosedRoleInterval = nd.SkillData.Template.DamageClosedRoleInterval;
            mGapDamageTime = nd.SkillData.Template.DamageClosedRoleInterval;
            if (nd.SkillData.Template.SummonOffsetType == GameData.Skill.EOffsetType.OffsetTarget)
                mDamageDelayTime = (long)nd.SkillData.Template.DamageDelayTime;
            BornTime = CSUtility.Helper.LogicTimer.GetTickCount();
            mPlacement = new RolePlacement(this);
            var place = mPlacement as RolePlacement;
            if(place !=null)
                place.IsUpdate2Client = false;
            if (nd.SkillData.Template.Gravity)
                mGravity = new CSUtility.Component.IGravityComp(RoleTemplate.HalfHeight * 3);

            SummonData.RoleMoveSpeed = nd.RoleTemplate.MoveSpeed;
            return true;
        }

        #region 战斗
        int mRoleHP;
        public override int RoleHP
        {
            get { return mRoleHP; }
            set { mRoleHP = value; }
        }

        int mRoleMP;
        public override int RoleMP
        {
            get { return mRoleMP; }
            set { mRoleMP = value; }
        }

        public override int FactionId
        {
            get
            {
                if (mOwnerRole != null)
                    return mOwnerRole.FactionId;

                return this.RoleTemplate.FactionId;
            }
        }

        public override bool HasHatred
        {
            get
            {
                return base.HasHatred;
            }
        }

        public override bool StopMoveOnBlock
        {
            get { return true; }
        }

        public override Role.RoleActor AttackTarget
        {
            get
            {
                var tar = base.AttackTarget;
                if (tar != null)
                {
                    return tar;
                }
                return mOwnerRole.AttackTarget;
            }
        }

        bool bStartDeath = false;
        public override void ProcDeath()
        {
            base.ProcDeath();
            OnSummonDeath();
            if (bStartDeath == false)
            {
                //SummonData.LiveTime = SummonData.RuneData.Template.DeathTime;
                bStartDeath = true;
            }
        }

        protected CSUtility.Support.EForEachResult OnVisitRole_SelectClosestTarget(UInt32 singleId, Role.RoleActor role, object arg)
        {
            var selector = arg as Monster.LockOnSelector;
            if (selector == null)
                return CSUtility.Support.EForEachResult.FER_Stop;
            if (this.CanLockon(role) == false)
                return CSUtility.Support.EForEachResult.FER_Continue;

            float dist = SlimDX.Vector3.Distance(role.Placement.GetLocation(), Placement.GetLocation());
            if (selector.Distance > dist)
            {
                selector.LockOnTarget = role;
                selector.Distance = dist;
            }
            return CSUtility.Support.EForEachResult.FER_Continue;
        }
        public override Role.RoleActor SelectAttackTarget()
        {
            var target = base.SelectAttackTarget();
            if (target != null)
                return target;

            var pos = Placement.GetLocation();

            var arg = new Monster.LockOnSelector();
            UInt32 actorTypes = (1 << (Int32)ERoleType.Player) | (1 << (Int32)ERoleType.Npc);
            HostMap.TourRoles(actorTypes, ref pos, (uint)mSummonData.RoleTemplate.LockOnRadius, this.OnVisitRole_SelectClosestTarget, arg);

            if (arg.LockOnTarget != null)
            {//选择了这个对象作为打击目标，添加一点仇恨
                //HatredManager.AddHatred(arg.LockOnTarget, 1);
            }
            return arg.LockOnTarget;
        }

        float mMinDistance = float.MaxValue;
        Role.RoleActor mNearestRole = null;
        protected CSUtility.Support.EForEachResult OnVisitRole_CalcNearestPos(UInt32 singleId, Role.RoleActor role, object arg)
        {
            if (role == this)
                return CSUtility.Support.EForEachResult.FER_Continue;

            var summon = arg as Role.RoleActor;
            if (summon == null)
                return CSUtility.Support.EForEachResult.FER_Continue;
            if (summon.OwnerRole == role)
                return CSUtility.Support.EForEachResult.FER_Continue;
            if (summon.CanLockon(role) == false)
                return CSUtility.Support.EForEachResult.FER_Continue;

            var L = Placement.GetLocation() - role.Placement.GetLocation();
            var curDistance = L.Length();

            if (curDistance < mMinDistance)
            {
                mMinDistance = curDistance;
                mNearestRole = role;
            }

            return CSUtility.Support.EForEachResult.FER_Continue;
        }

        public void OnSummonHurt()
        {
            
        }

        public void OnSummonDeath()
        {

        }

        [CSUtility.AISystem.Attribute.AllowMember("召唤物攻击", CSUtility.Helper.enCSType.Server, "召唤物释放技能")]
        public void SummonFireSkill(UInt16 skillId)
        {
            if (OwnerRole == null)
                return;

            // 召唤物是否可以再发射召唤物
            if (mSummonData.SkillData.Template.CanFire == false)
                return;

            var skillData = FindSkillData(skillId);
            if (skillData == null)
                return;
            if (skillData.SkillLevel == 0)
                return;

            var skill = skillData.Template;
            if (skill == null)
                return;

            // 得到最近的敌对目标位置
            var pos = Placement.GetLocation();

            UInt32 actorTypes = (1 << (Int32)ERoleType.Player) | (1 << (Int32)ERoleType.Npc);
            mMinDistance = skill.AttackRadius;
            mNearestRole = null;
            if (HostMap.TourRoles(actorTypes, ref pos, (uint)skill.AttackRadius, this.OnVisitRole_CalcNearestPos, this) == true)
            {
                if (mNearestRole != null)
                {
                    var dir = mNearestRole.Placement.GetLocation() - pos;
                    dir.Normalize();

                    CurrentState.DoFireSkill(skillId, dir, SlimDX.Vector3.Zero);
                }
            }
        }

        public override bool IsValidMoveSpeed(float speed, sbyte run)
        {
            if (speed != mSummonData.SkillData.Template.ThrowRoleSpeed || speed != MoveSpeed)
                return false;
            return true;
        }

        public override void OnFireSkill(UInt16 skillId, SlimDX.Vector3 tarPos, SlimDX.Vector3 summonPos,UInt32 tarRoleId)
        {
            base.OnFireSkill(skillId, tarPos, summonPos,tarRoleId);
        }

        class AttackCollideArg
        {
            public SummonData mSummonData;
            public bool bDoDamage = false;
        }
        void OnPlayerCreateSumEnd()
        {
            if (OwnerRole == null)
                return;
            //             if (this.OwnerRole.RoleCreateType != CSCommon.Data.ERoleCreateType.Player)
            //                 return;
//             var player = this.OwnerRole as Player.PlayerInstance;
//             if (player != null)
//             {
//                 var skill = player.FindSkillData(SummonData.SkillTemplateId);
//                 if (skill != null)
//                     skill.AddSummon(SummonData.SkillTemplateId, this.SingleId);
//             }
            if (this.SummonData.SkillData.Template.DamageDelayTime != 0)//伤害延迟
                return;
            if (this.SummonData.SkillData.Template.SummonOffsetType != GameData.Skill.EOffsetType.None)
                return;
            DoAttackCollide(mDamageClosedRoleInterval);
            //  DamageCurTime = OwnerRole.GetCurFrameTickCount();
        }

        public Int64 mDamageClosedRoleInterval;
        int mCurDamageCalculationNumber = 0;
        public void DoAttackCollide(Int64 elapseMillisecond)
        {
            if (CurrentState == null)
                return;
            if (CurrentState.StateName != "Walk" && CurrentState.StateName != "Idle")
                return;
            var skill = SummonData.SkillData.Template;
            if (skill == null)
                return;
            
            if (skill.DamageClosedRoleInterval > skill.LiveTime * 1000)
                return;

            if (mCurDamageTargetNumber >= skill.MaxDamageTargetNumber)
                return;

            if (CurrentState.StateName == "Death")
                return;
            //if (OwnerRole != null && OwnerRole.RoleCreateType == CSCommon.Data.ERoleCreateType.Player && SummonData.NeedServerCollider ==false)
            //    return;

            AttackCollideArg arg = new AttackCollideArg();
            arg.mSummonData = mSummonData;
            mDamageClosedRoleInterval -= elapseMillisecond;
            if (mDamageClosedRoleInterval <= 0)  // 伤害判定的时间间隔
            {
                mDamageClosedRoleInterval = skill.DamageClosedRoleInterval;

                if (mCurDamageCalculationNumber < skill.DamageCalculationCount) // 伤害判定次数
                {
                    var actorTypes = ERoleType.Player| ERoleType.Monster;
                    var pos = Placement.GetLocation();
                    float radius = skill.Radius +1;
                    HostMap.TourRoles((uint)actorTypes, ref pos, (UInt32)radius, this.OnVisitRole_CollideTarget, arg);

                    if (arg.bDoDamage == true)
                    {
                        // 每次攻击判定后，重置攻击个数
                        mCurDamageTargetNumber = 0;
                        mCurDamageCalculationNumber++;

                        if (skill.DeathOnDamage == true)
                        {
                            this.CurrentState.ToState("Death", null);
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        int mCurDamageTargetNumber = 0;
        public int CurDamageTargetNumber
        {
            get { return mCurDamageTargetNumber; }
            set { mCurDamageTargetNumber = value; }
        }
        protected CSUtility.Support.EForEachResult OnVisitRole_CollideTarget(UInt32 singleId, Role.RoleActor role, object arg)
        {
            if (this.OwnerRole == null)
                return CSUtility.Support.EForEachResult.FER_Continue;
            if (this.OwnerRole.CanAttack(role) == false)
                return CSUtility.Support.EForEachResult.FER_Continue;
            if(this.SummonData.LockOnRoleId !=UInt32.MaxValue)
            {
                if (role.SingleId != this.SummonData.LockOnRoleId)
                    return CSUtility.Support.EForEachResult.FER_Continue;
            }
            var attackArg = arg as AttackCollideArg;
            var skillData = attackArg.mSummonData.SkillData;
            if (skillData == null)
                return CSUtility.Support.EForEachResult.FER_Continue;

            var distance = role.Placement.GetLocation() - Placement.GetLocation();
            distance.Y = 0;//按圆桶来计算
            float dist = distance.Length();
           // Log.FileLog.WriteLine(string.Format("dist:{0},tarrole,{1},state{2}",dist,role.RoleTemplate.NickName,this.CurrentState));
            if (skillData.Template.Radius +role.RoleTemplate.Radius +0.03f> dist)
            {
                if (mCurDamageTargetNumber >= skillData.Template.MaxDamageTargetNumber)
                    return CSUtility.Support.EForEachResult.FER_Stop;
                mCurDamageTargetNumber++;
                if (this.OwnerRole != null)
                {
                    if (role.CurrentState == null)
                    {
                        Log.FileLog.WriteLine(string.Format("被攻击者的 CurrentState = null"));
                    }
                    else
                    {
                        if (IsDamaged == false)
                        {
                            IsDamaged = true;
                            //AddSkillBuff2Bag(role, SummonData.SkillTemplateId, SummonData.RuneData, this.OwnerRole);
                        }
                        // 目标进入被击打状态
                        //if (role.CurrentState.StateName != "StayAttack" || skillData.Template.Force2BeAttack)
                        //{
                        //    role.ProcDoBeAttack(skillData, this);
                        //}
                        OnSummonHurt();
                        attackArg.bDoDamage = role.ProcHurt(this.OwnerRole, skillData, 0);  //这里伤害者用主角的吧
                        this.DamageCurTime = this.GetCurFrameTickCount();
                    }
                }
            }
            return CSUtility.Support.EForEachResult.FER_Continue;
        }

        #endregion

        Int64 mUpdateTime = 0;
        public override void Tick(long elapsedMillisecond)
        {
            if (IsLeaveMap == true)
                return;
            base.Tick(elapsedMillisecond);
            var time = CSUtility.Helper.LogicTimer.GetTickCount();

            if (mSummonData == null || CurrentState == null)
                return;
            DoAttackCollide(elapsedMillisecond);
            if (CurrentState.StateName == "Death")
            {
                mUpdateTime += elapsedMillisecond;
            }
            mSummonData.LiveTime = SummonData.SkillData.Template.LiveTime - (float)(time - BornTime) / 1000;
            if (mSummonData.LiveTime < 0)
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("tick,进入死亡状态_{0},id_{1},BornTime_{2}", SummonData.RuneData.Template.RuneName,SingleId,BornTime));
                if (CurrentState.StateName != "Death")
                {
                    CurrentState.ToState("Death", null);
                }

                if (mUpdateTime > 5000)//死亡动作最多5秒钟
                {
                    //ProcDeath();
                    //      System.Diagnostics.Debug.WriteLine(string.Format("tick,死亡超时name_{0} id=={1},BornTime_{2}", SummonData.RuneData.Template.RuneName,SingleId,BornTime));
                    SummonIm2Death();
                }
            }

            if (bStartDeath && SummonData.SkillData.Template.DeathTime > mUpdateTime)
            {
                //    System.Diagnostics.Debug.WriteLine(string.Format("SummonIm2Death,死亡时间到了name_{0}, id=={1}，Born_Time{2}", SummonData.RuneData.Template.RuneName,SingleId,mSummonData.LiveTime));
                SummonIm2Death();
            }

            switch (mSummonData.SkillData.Template.EmissionPathType)
            {
                case GameData.Skill.EEmissionPathType.LineTurn:
                    {
                        var walkState = AIStates.GetState("Walk") as CSUtility.AISystem.States.Walk;
                        if (walkState != null)
                        {
                            if (walkState.WalkParameter.TargetPositions.Count == 0)
                            {
                                if ((OwnerRole.Placement.GetLocation() - Placement.GetLocation()).LengthSquared() < 0.1)
                                {
                                    CurrentState.ToState("Death", null);
                                    DoLeaveMap();
                                }
                                else
                                {
                                    walkState.WalkParameter.TargetPosition = OwnerRole.Placement.GetLocation();
                                    if (CurrentState != null)
                                    {
                                        CurrentState.ToState("Walk", walkState.WalkParameter);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public void SummonIm2Death()
        {
            if (CurrentState.StateName != "Death")
                CurrentState.ToState("Death", null);
            DoLeaveMap();
            //OnEnterDeath();
        }

        public override SkillData FindSkillData(ushort templateId)
        {
            foreach (var sk in mSummonData.Skills)
            {
                if (templateId == sk.TemplateId)
                    return sk;
            }
            return null;
        }

        public static int SummonNumber = 0;
        public static SummonRole CreateSummonInstance(Role.RoleActor owner, SummonData smData, Hall.HallInstance halls, Map.MapInstance map, ref SlimDX.Vector3 pos, 
            GameData.Skill.SkillData skillData, ref SlimDX.Vector3 direction, bool updateclient = true)
        {
            if (smData.Template == null)
                return null;

            SummonRole ret = new SummonRole();
            CSUtility.Component.ActorInitBase actInit = new CSUtility.Component.ActorInitBase();
            actInit.GameType = (UInt16)skillData.Template.GameType;
            ret.Initialize(actInit);
            ret.HallInstance = halls;
            ret.OwnerRole = owner;
            smData.LiveTime = skillData.Template.LiveTime;
            smData.OiDirection = direction;
            ret.RoleCreateType = ERoleType.Summon;
            ret.InitSummonInstance(smData);
            if (!ret._SetOwnerRole(owner))
                return null;
            ret.SummonData.OriPosition = pos;
            ret.SummonData.Position = pos;
            ret.Placement.SetLocation(ref pos);
            direction.Normalize();
            ret.Placement.SetRotationY(direction.Z, direction.X,ret.RoleTemplate.MeshFixAngle,true);
            if (smData.AIGuid == Guid.Empty)
            {
                smData.AIGuid = ret.RoleTemplate.AIGuid;
            }
            ret.InitFSM(smData.AIGuid, true);
            if (ret.CurrentState == null)
                return null;

            if (skillData.Template.ThrowRoleSpeed > 0)
            {
                var walkState = ret.AIStates.GetState("Walk") as CSUtility.AISystem.States.Walk;
                if (walkState != null)
                {
                    walkState.WalkParameter.MoveSpeed = skillData.Template.ThrowRoleSpeed;

                    var targetPos = pos + direction * (skillData.Template.ThrowRoleSpeed * skillData.Template.LiveTime * 1.5f);
                    if (ret.SummonData.SkillData.Template.SummonLockOnRole)
                    {
                        var tarRole = ret.OwnerRole.HostMap.GetRole(ret.SummonData.LockOnRoleId);
                        if (tarRole != null)
                        {
                            targetPos = tarRole.Placement.GetLocation() ;
                            targetPos.Y +=tarRole.RoleTemplate.HalfHeight;
                        }   
                    }
                    switch (skillData.Template.EmissionPathType)
                    {
                        case GameData.Skill.EEmissionPathType.LineTurn:
                            walkState.WalkParameter.TargetPosition = pos + direction * (skillData.Template.ThrowRoleDistance);
                            walkState.WalkParameter.TargetPositions.Enqueue(owner.Placement.GetLocation());
                            break;
                        default:
                            walkState.WalkParameter.FinalPosition = targetPos;
                            walkState.WalkParameter.TargetPosition = targetPos;// 
                            break;
                    }
                    if (ret.CurrentState != null)
                    {
                        ret.CurrentState.ToState("Walk", walkState.WalkParameter);
                    }
                }
            }
            ret.OnEnterMap(map);

            ret.BornTime = owner.GetCurFrameTickCount();
            if (updateclient)
            {
                RPC.PackageWriter pkg = new RPC.PackageWriter();
                RPC.DataWriter param = new RPC.DataWriter();
                UInt16 count = 1;
                param.Write(count);
                param.Write(ret.SingleId);
                param.Write(smData, true);
                param.Write(owner.SingleId);
                param.Write(owner.Placement.GetLocation());
                param.Write(ret.CurrentState.StateName);
                param.Write(ret.CurrentState.Parameter, true);

                Client.H_GameRPC.smInstance.RPC_CreateSummon(pkg, param);
                map.SendPkg2Clients(null, pos, pkg);
            }

            ret.CurrentState.OnRoleCreatEnd();
            ret.OnPlayerCreateSumEnd();

            SummonNumber++;
            return ret;
        }
    }
}
