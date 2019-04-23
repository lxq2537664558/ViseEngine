using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.ServerStates
{
    [CSUtility.AISystem.Attribute.StatementClass("站桩攻击状态", CSUtility.Helper.enCSType.Server)]
    public class StayAttack : GameData.AI.States.StayAttack
    {
        public bool  mBoolShooted = false;
        public SlimDX.Vector3 mStartJumpPos = SlimDX.Vector3.Zero;
        public Int64 mStartJumptime = 0;

        public void ProcBuffNotifiers(Role.RoleActor role, GameData.Skill.SkillData skill, CSUtility.Animation.AnimationTree anim)
        {
            if (anim == null)
                return;
            var levels = skill.GetSkillLevelTemp();
            if (levels == null || levels.Buffs.Count ==0)
                return;

            if (anim.Action != null)
            {
                var ntf = anim.Action.GetNotifier("Buff");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null)
                    {
                        foreach (var i in nplist)
                        {
                            foreach(var param in levels.Buffs)
                            {
                                var target =role.HostMap.GetRole(StayAttackParameter.TarSingle);
                                role.AddBuff(param,target,GameData.Skill.BuffAddType.SkillAdd);
                            }   
                        }
                    }
                }
            }
            var children = anim.GetAnimations();
            foreach (var i in children)
            {
                ProcBuffNotifiers(role, skill, i);
            }
        }

        public override bool OnActionNotify(CSUtility.Animation.ActionNotifier notifier, CSUtility.Animation.NotifyPoint np)
        {
            var role = Host as Role.RoleActor;
            if (Skill == null)
            {
                Skill = role.FindSkillData(StayAttackParameter.SkillId);
            }

            if (Skill == null)
                return false;
            //这个shootnotifier比较特殊，客户端在玩家身上也作了，然后通知服务器作的
            
            switch (Skill.Template.SkillAttackType)
            {
                case GameData.Skill.ESkillAttackType.Region:
                    {
                        if(notifier.NotifyName == "Hurt")
                            role.DoTargetBeHurt(np, Skill);
                    }
                    break;
                case GameData.Skill.ESkillAttackType.Summon:
                    {
                        if (notifier.NotifyName == "Shoot")
                            OnFireSkill();
                    }
                    break;
            }
            return true;
        }        

        //         public void ProcJumpNotifiers(Role.RoleActor role, Game.Skill.SkillData skill, .Animation.AnimationTree anim)
        //         {
        //             if (anim == null)
        //                 return;
        // 
        //             if (anim.Action != null)
        //             {
        //                 // 跳跃tick
        //                 var ntf = anim.Action.GetNotifier("StartJumpUp");
        //                 if (ntf != null)
        //                 {
        //                     var firstJumpUp = false;
        //                     var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
        //                     if (nplist != null && nplist.Count != 0)
        //                     {
        //                         foreach (var i in nplist)
        //                         {
        //                             //System.Diagnostics.Debug.WriteLine(string.Format("mStartJumpPos == {0}",role.Placement.GetLocation()));
        //                             mStartJumpPos = role.Placement.GetLocation();
        //                             mStartJumptime = role.GetCurFrameTickCount();
        //                             firstJumpUp = true;
        //                             // mMoveCollect = false;
        //                         }
        //                     }
        //                     if (mStartJumpPos != SlimDX.Vector3.Zero && mStartJumptime != 0)
        //                     {
        //                         if (rune.Template.OffsetType == CSCommon.Data.Skill.EOffsetType.JumpUp)
        //                             role.OnUpDateJumpUpNotify(skill.Template, rune, rune.Template.JumpUpTime, firstJumpUp, mStartJumpPos, mStartJumptime, StayAttackParameter.tarPos);
        //                         if (rune.Template.OffsetType == CSCommon.Data.Skill.EOffsetType.Spurt && mMoveCollect == false)
        //                         {
        //                             role.OnTickSpurtNotify(ref mMoveCollect, Skill.Template, rune.Template.JumpUpTime, firstJumpUp, Rune, mStartJumpPos, mStartJumptime, StayAttackParameter.tarPos);
        //                             if (mMoveCollect)
        //                             {
        //                                 OnActionFinished();
        //                             }
        //                         }
        //                     }
        //                 }
        //                 // 闪烁
        //                 ntf = anim.Action.GetNotifier("Flicker");
        //                 if (ntf != null)
        //                 {
        //                     var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
        //                     if (nplist != null && nplist.Count != 0)
        //                     {
        //                         //System.Diagnostics.Debug.WriteLine(string.Format("mStartJumpPos == {0}",role.Placement.GetLocation()));
        //                         if (rune.Template.OffsetType == CSCommon.Data.Skill.EOffsetType.Flicker)
        //                         {
        //                             var dir = StayAttackParameter.tarPos - role.Placement.GetLocation();
        //                             dir.Y = 0;
        //                             dir.Normalize();
        //                             var tarPos = StayAttackParameter.tarPos - dir * (role.RoleTemplate.Radius);
        //                             tarPos = ServerCommon.Planes.Role.RoleActor.FindTargetPos(role, tarPos, dir);
        //                             role.SetPosition2TargetPosClient(tarPos, 0);
        //                         }
        //                     }
        //                 }
        // 
        //                 // //添加buff,,notify
        //                 ntf = anim.Action.GetNotifier("CastBuff");
        //                 if (ntf != null)
        //                 {
        //                     var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
        //                     if (nplist != null && nplist.Count != 0)
        //                     {
        //                         if (Rune.Template.GetRuneLevelParam(Rune.RuneLevel) == null)
        //                             return;
        //                         foreach (var buffParam in Rune.Template.GetRuneLevelParam(Rune.RuneLevel).BuffParams)
        //                         {
        //                             var buff = CSCommon.Data.Skill.BuffTemplateManager.Instance.FindBuff((ushort)buffParam.BuffId);
        //                             if (buff == null)
        //                                 continue;
        //                             var buffadd = new CSCommon.Data.Skill.SkillBufffDataAdd();
        //                             buffadd.SkillId = Skill.SkillTemlateId;
        //                             buffadd.RuneId = Rune.RuneTemlateId;
        //                             buffadd.RuneLevel = Rune.RuneLevel;
        //                             buffParam.BuffAdd = buffadd;
        // 
        //                             if ((buff.BuffState == CSCommon.Data.Skill.BuffState.Self || buff.BuffState == CSCommon.Data.Skill.BuffState.SelfAll) && buff.BuffAdd == CSCommon.Data.Skill.BuffAddType.Notify)
        //                                 role.BuffBag.CreateBuffAndAutoAdd2Bag(role, role, buffParam);//给自己加
        //                             else
        //                             {
        //                                 var player = role as ServerCommon.Planes.PlayerInstance;
        //                                 if (player != null && buff.BuffState == CSCommon.Data.Skill.BuffState.SelfAll && buff.BuffAdd == CSCommon.Data.Skill.BuffAddType.Notify)
        //                                 {
        //                                     player.AddTeamerBuffByParam(buffParam);//给队友加
        //                                 }
        //                                 else if (buff.BuffState == CSCommon.Data.Skill.BuffState.Other && buff.BuffAdd == CSCommon.Data.Skill.BuffAddType.Notify)
        //                                 {
        //                                     var targetRole = role.AttackTarget;
        //                                     if (targetRole != null)
        //                                     {
        //                                         targetRole.BuffBag.CreateBuffAndAutoAdd2Bag(role, role, buffParam);//给敌人加
        //                                     }
        //                                 }
        //                             }
        //                         }
        //                     }
        //                 }
        // 
        //             }
        // 
        //         }

//         public void ProcEffectNotifiers(CSUtility.Animation.AnimationTree anim)
//         {
//             var role = Host as Role.RoleActor;
//             if (role == null)
//                 return;
// 
//             var action = anim.Action;
//             if (action != null)
//             {
//                 var notifyList = action.GetNotifiers(typeof(CSUtility.ActionNotify.EffectActionNotifier));
//                 foreach (var notify in notifyList)
//                 {
//                     if (notify == null)
//                         continue;
//                     if (notify.NotifyName != "SkillEffect")
//                         continue;
//                     var nplist = notify.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
//                     if (nplist != null) //默认同一个notify只播一次
//                     {
//                         foreach (var i in nplist)
//                         {
//                             if (mCrrentEffectNotifyName == i.NotifyName)
//                                 continue;
//                             //  mCurrentEffect =Rune.GetCurrentAttackNotifyEffect(notify.NotifyName);
//                             if (Skill != null && Skill.Template.AttackEffectRepeatAdd == false)
//                                 mCrrentEffectNotifyName = i.NotifyName;
//                          //   role.OnAttackEffectNotify(notify.NotifyName, i.NotifyName, Skill.Template, StayAttackParameter.RuneHandle);
//                         }
//                     }
//                 }
//             }
//         }

        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

            DoTickNotifier("Hurt");
            DoTickNotifier("Shoot");
        }

        public override void OnFireSkill()
        {
            var role = Host as Role.RoleActor;
            if (role == null)
                return;
            SlimDX.Vector3 pos = role.Placement.GetLocation();
            pos.Y = role.Placement.GetLocation().Y +Skill.Template.HalfHeight;
            StayAttackParameter.SummonPos = pos;
            if (Skill.Template.SkillSumType == GameData.Skill.ESkillSumType.Monster)
            {
                var dir = StayAttackParameter.tarPos - role.Placement.GetLocation();
                dir.Y = 0;
                dir.Normalize();
                role.OnSummonNpc(StayAttackParameter.SkillId, dir, StayAttackParameter.SummonPos);
            }
            else
            {
                role.OnFireSkill(StayAttackParameter.SkillId, StayAttackParameter.tarPos, StayAttackParameter.SummonPos,StayAttackParameter.TarSingle);
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
        }

        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
            var role = Host as Role.RoleActor;

            Parameter.IsPrimaryState = true;
            mStartJumpPos = SlimDX.Vector3.Zero;
            mStartJumptime = 0;
            var arg = this.Parameter as CSUtility.AISystem.States.IStayAttackParameter;
            var  Skill = role.FindSkillData(arg.SkillId);
            if (Skill == null)
            {
                Log.FileLog.WriteLine(string.Format("Skill==null,skillid =={0}",StayAttackParameter.SkillId));
                return;
            }

            if (StayAttackParameter.tarPos != null && StayAttackParameter.tarPos !=SlimDX.Vector3.Zero)
            {
                var fireDir = StayAttackParameter.tarPos - role.Placement.GetLocation();
                fireDir.Normalize();
                if (role.RoleTemplate.IsRotate)
                    role.Placement.SetRotationY(fireDir.Z, fireDir.X, role.RoleTemplate.MeshFixAngle, false);
            }
            else if (arg.tarDir != null && arg.tarDir != SlimDX.Vector3.Zero)
            {
                if (role.RoleTemplate.IsRotate)
                    role.Placement.SetRotationY(arg.tarDir.Z, arg.tarDir.X, role.RoleTemplate.MeshFixAngle);
            }
            else if (arg.TarAngle != float.MaxValue)
            {
                //role.Placement.SetRotationY(arg.TarAngle);
            }
           

        }
        public override void OnReEnterState()
        {
            base.OnReEnterState();
        }

        public override void OnExitState()
        {
            base.OnExitState();
            Skill = null;
        }

        public override void SetStateAction()
        {
            var role = Host as Role.RoleActor;
            if (role == null)
                return;
            var Skill = role.FindSkillData(StayAttackParameter.SkillId);

            if (string.IsNullOrEmpty(Skill.Template.LoopAction))
            {
                AddAction(Skill.Template.AttackAction, 100);
            }
            else
            {
                AddAction(Skill.Template.LoopAction, 100, Skill.Template.RotationSkillTime);

                if (string.IsNullOrEmpty(Skill.Template.EndAction))
                {
                    AddAction(Skill.Template.EndAction, 100);
                }
            }
        }

    }
}
