using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Skill
{
    public class SkillController
    {
        public static SkillController Instance = new SkillController();

        List<SkillCommand> mCommandQueue = new List<SkillCommand>();//rule 1:一次只有一个指令（key是唯一的）
        public List<SkillCommand> CommandQueue
        {
            get { return mCommandQueue; }
            set { mCommandQueue = value; }
        }

        public Role.RoleActor Chief
        {
            get { return Stage.MainStage.Instance.ChiefRole; }
        }

        public float mLockRadius
        {
            get { return Chief.RoleTemplate.LockOnRadius; }
        }
        //锁定
        public Role.RoleActor mLockOnRole;
//         public UInt32 mLockRoleId
//         {
//             get
//             {
//                 FreshLockOnRole();
//                 if (mLockOnRole != null)
//                 {
//                     if (mLockOnRole.CurrentState.StateName == "Death")
//                     {
//                         mLockOnRole = null;
//                         return UInt32.MaxValue;
//                     }
//                     return mLockOnRole.SingleId;
//                 }
//                 else
//                     return UInt32.MaxValue;
//             }
//         }

        public UInt32 GetLockRoleId(GameData.Skill.SkillData skill)
        {
            FreshLockOnRole(skill);
            if (mLockOnRole != null)
            {
                if (mLockOnRole.CurrentState.StateName == "Death")
                {
                    mLockOnRole = null;
                    return UInt32.MaxValue;
                }
                return mLockOnRole.SingleId;
            }
            else
                return UInt32.MaxValue;
        }

        public void FreshLockOnRole(GameData.Skill.SkillData skill)
        {
            if (mLockOnRole == null)
            {
                TryCatchLockOne(skill);
            }
            else
            {
                {
                    var dist = mLockOnRole.Placement.GetLocation() - Chief.Placement.GetLocation();
                    dist.Y = 0;
                    if (dist.Length() > skill.Template.AttackRadius)
                    {
                        ClearLock();
                        TryCatchLockOne(skill);
                    }
                }
            }    
        }

        public void OnLBD(CCore.MsgProc.BehaviorParameter parameter)
        {
            var key = parameter as  CCore.MsgProc.Behavior.Mouse_Key;
            if (key == null)
                return;
            var role = Stage.MainStage.Instance.GetMouseClickRole(key.X, key.Y);
            if (role == null)
                return;
            if (role.CurrentState == null)
                return;
            if (role.CurrentState.StateName == "Death")
                return;

            if (role.SingleId == Chief.SingleId)
                return ;
            if (role.RoleData.RoleType == Role.EClientRoleType.Summon)
                return;
            if (role.RoleData.FactionId == Chief.RoleData.FactionId)
                return;
            mLockOnRole = role;
        }

        public void ClearLock()
        {
            mLockOnRole = null;
        }

        public bool TryCatchLockOne(GameData.Skill.SkillData skill)
        {
            var pos = Chief.Placement.GetLocation();
            var dirvec2 = Chief.Placement.GetRotationYVec();
            var dir = new SlimDX.Vector3(dirvec2.X,0,dirvec2.Y);
            dir.Normalize();

            float fHalfHeight = 1;
            float fRadius = 0.5F;
            var mRadius = skill.Template.AttackRadius;

            var cylinder = Chief.Shape as CSUtility.Component.IShapeCylinder;
            if (cylinder != null)
            {
                fHalfHeight = cylinder.HalfHeight;
                fRadius = cylinder.Radius;
            }

            var startPos = new SlimDX.Vector3(pos.X -mRadius, pos.Y - fHalfHeight, pos.Z - mRadius);
            var endPos = new SlimDX.Vector3(pos.X + mRadius, pos.Y + fHalfHeight, pos.Z + mRadius);

            UInt16 actorType = (UInt16)CSUtility.Component.EActorGameType.Player | (UInt16)CSUtility.Component.EActorGameType.Npc;
            var roles = CCore.Engine.Instance.Client.MainWorld.GetActors(ref startPos, ref endPos, actorType);
            float dist = float.MaxValue;
            Role.RoleActor monster=null;
            Role.RoleActor player=null;
            foreach (var i in roles)
            {
                var role = i as Role.RoleActor;
                if (role == null)
                    continue;
                if (role.SingleId == Chief.SingleId)
                    continue;
                if (role.RoleData.RoleType == Role.EClientRoleType.Summon)
                    continue;
                if (role.RoleData.FactionId == Chief.RoleData.FactionId)
                    continue;
                if (role.CurrentState.StateName == "Death")
                    continue;
                var distance = role.Placement.GetLocation() - pos;
                distance.Y = 0;
                if(distance.Length() <dist)
                {
                    if(role.RoleData.RoleType ==Role.EClientRoleType.Monster)
                    {
                        if (role.RoleData.MonsterData.Unrivaled || role.RoleData.RoleTemplate.MonsterType >= GameData.Role.MonsterType.Symbol)
                            continue;
                        monster = role;
                    }
                    else
                    {
                        player = role;
                        break;
                    }
                }
            }

            if (player != null)
                mLockOnRole = player;
            else if (monster != null)
                mLockOnRole = monster;
            else
                return false;

            return true;
        }

        public void AddCommand(GameData.Skill.SkillData skill)
        {
            if (mCommandQueue.Count > 50)
            {
                return;
            }
            SkillCommand command = new SkillCommand(skill);

            if (!FindTargetCommand(skill.TemplateId))
            {
                mCommandQueue.Insert(0, command);
            }
            else
            {
                mCommandQueue.Add(command);
            }
        }
        public bool FindTargetCommand(ushort skillid)
        {
            bool res = false;
            foreach (var com in mCommandQueue)
            {
                if (com.Skill.TemplateId == skillid)
                {
                    res = true;
                }
            }
            return res;
        }
        public SkillCommand PoPFirstCommand()
        {
            if (mCommandQueue.Count == 0)
            {
                return null;
            }

            var command = mCommandQueue[0];
            mCommandQueue.RemoveAt(0);
            return command;
        }

        Int64 mUpdateReady = 3000;//通信时间，延迟3秒就算失败

        public void Tick(long elapsedMillisecond)
        {
            if(Chief.CurrentState!=null)
            {
                if ((Chief.CurrentState.CanInterrupt == true || Chief.CurrentState.SkillCanInterrupt == true) && mCommandQueue.Count != 0 && Role.ChiefRoleActorController.Instance.FireSkillReady == true)
                {
                    var skill = mCommandQueue[0].Skill;
                    if (skill != null)
                    {
                        var command = PoPFirstCommand();
                        if (command != null)
                        {
                            Role.ChiefRoleActorController.Instance.FireSkillReady = false;
                            Chief.OnFireSkill(skill);
                        }
                    }
                }
            }

            for (int i = 0; i < mCommandQueue.Count; ++i)
            {
                var command = mCommandQueue[i];
                command.LiveTime += (Int32)elapsedMillisecond;
                if (command.LiveTime >= command.Duration)
                {
                    mCommandQueue.Remove(command);
                }
            }
            mUpdateReady -= elapsedMillisecond;
            if (mUpdateReady < 0 && Role.ChiefRoleActorController.Instance.FireSkillReady == false)
            {
                mUpdateReady = 3000;
                Role.ChiefRoleActorController.Instance.FireSkillReady = true;
            }
        }
    }

    public class SkillCommand
    {
        public SkillCommand(GameData.Skill.SkillData skillid)
        {
            mSkill = skillid;
            if (mSkill != null)
                mDuration = (Int32)mSkill.Template.ControlLife;
        }

        Int32 mLiveTime = 0;
        public Int32 LiveTime
        {
            get { return mLiveTime; }
            set { mLiveTime = value; }
        }

        Int32 mDuration = 360;
        public Int32 Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }

        GameData.Skill.SkillData mSkill;
        public GameData.Skill.SkillData Skill
        {
            get { return mSkill; }
            set { mSkill = value; }
        }
        SlimDX.Vector3 mSummonPos = SlimDX.Vector3.Zero;
        public SlimDX.Vector3 SummonPos
        {
            get { return mSummonPos; }
            set { mSummonPos = value; }
        }
    }
}
