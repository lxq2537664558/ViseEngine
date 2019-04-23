using System;
using System.Collections.Generic;

using System.Text;

namespace Client.GameState
{
    [CSUtility.AISystem.Attribute.StatementClass("站桩状态", CSUtility.Helper.enCSType.Client)]
    public class StayAttack : GameData.AI.States.StayAttack
    {
        SlimDX.Vector3 mStartJumpPos =SlimDX.Vector3.Zero;
        SlimDX.Vector3 mEndMovePos = SlimDX.Vector3.Zero;
    //    bool mFirstStartJump =true;
      //  bool mMoveCollect = false;
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
           
            CanInterrupt = false;
         //   mMoveCollect = false;
            mStartJumpPos = SlimDX.Vector3.Zero;
            var player = Host as Client.Role.RoleActor;
            if (player == null)
                return;
            var pos = player.Placement.GetLocation();
            player.Placement.SetLocation(ref pos);
            if (Skill == null)
            {
                Skill = new GameData.Skill.SkillData();
                Skill.TemplateId = StayAttackParameter.SkillId;
            }

            if(StayAttackParameter.tarPos != null && StayAttackParameter.tarPos !=SlimDX.Vector3.Zero)
            {
                FireDir = StayAttackParameter.tarPos - pos;
                FireDir.Normalize();
                player.Placement.SetRotationY(FireDir.Z, FireDir.X, player.RoleTemplate.MeshFixAngle, false);
            }
            else if (StayAttackParameter.tarDir != SlimDX.Vector3.Zero)
            {
                player.Placement.SetRotationY(StayAttackParameter.tarDir.Z, StayAttackParameter.tarDir.X, player.RoleTemplate.MeshFixAngle, false);
            }
            else if (StayAttackParameter.TarAngle != float.MaxValue)
            {
                //player.Placement.SetRotationY(StayAttackParameter.TarAngle);
            }

            //  player.InitEffects(Skill,  StateName);
        }
        public SlimDX.Vector3 FireDir = new SlimDX.Vector3();
        public override void OnEnterState()
        {
            base.OnEnterState();
        }

        public override void SetStateAction()
        {
            if (Skill == null)
            {
                Skill = new GameData.Skill.SkillData();
                Skill.TemplateId = StayAttackParameter.SkillId;
            }

            if(string.IsNullOrEmpty(Skill.Template.LoopAction))
            {
                AddAction(Skill.Template.AttackAction, 100);
            }
            else
            {
                AddAction(Skill.Template.LoopAction,100,Skill.Template.RotationSkillTime);

                if(string.IsNullOrEmpty(Skill.Template.EndAction))
                {
                    AddAction(Skill.Template.EndAction,100);
                }
            }
        }

        public void ProcShootNotifiers(CSUtility.Animation.AnimationTree anim)
        {
            var player = Host as Client.Role.RoleActor;
            if (player == null)
                return;

            var action = anim.Action;
            if (action != null)
            {
                var ntf = action.GetNotifier("StartAttack");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null)
                    {
                        foreach (var i in nplist)
                        {
                         //   player.OnStartAttack(StayAttackParameter.SkillId);
                        }
                    }
                }

                // 开始位移
                ntf = action.GetNotifier("StartOffset");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null)
                    {
                        foreach (var i in nplist)
                        {
             
                      //      player.OnStartOffsetNotify(Skill, FireDir);
                        }
                    }
                }

                // 跳跃tick
                ntf = action.GetNotifier("StartJumpUp");
                if (ntf != null)
                {
                 //   System.Diagnostics.Debug.WriteLine(string.Format("StartJumpUp"));
                   // bool mFirstStartJump = false;
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null && nplist.Count !=0)
                    {
                        foreach (var i in nplist)
                        {
                            // System.Diagnostics.Debug.WriteLine(string.Format("mStartJumpPos == {0}",player.Placement.GetLocation()));
                            mStartJumpPos = player.Placement.GetLocation();
                     //       mFirstStartJump = true;
               
//                             if (Rune.OffsetType == CSCommon.Data.Skill.EOffsetType.Spurt) 
//                                 mEndMovePos = GetTargetPos(player, Skill, Rune, mStartJumpPos);
                        }
                    }
                    if (mStartJumpPos != SlimDX.Vector3.Zero)
                    {
//                         if(Rune.OffsetType ==CSCommon.Data.Skill.EOffsetType.JumpUp)
//                             player.OnUpDateJumpUpNotify(player,Skill, Rune, Rune.JumpUpTime, mFirstStartJump, mStartJumpPos);
//                         if (Rune.OffsetType == CSCommon.Data.Skill.EOffsetType.Spurt &&mMoveCollect == false) 
//                         {
//                            player.OnTickSpurtNotify(ref mMoveCollect, Skill, Rune, mStartJumpPos, mEndMovePos);
//                            if (mMoveCollect)
//                            {
//                                OnActionFinished();
//                            }
//                         }
                    }
                }

                // 结束位移
                ntf = action.GetNotifier("EndJumpUp");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null && nplist.Count != 0)
                    {
                        foreach (var i in nplist)
                        {
                         //   System.Diagnostics.Debug.WriteLine(string.Format("EndJumpUp"));
                       //     mStartJumpPos = SlimDX.Vector3.Zero;
                        }
                    }
                }
        
                // 打击点
                ntf = action.GetNotifier("Contact");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null)
                    {
                        foreach (var i in nplist)
                        {
                            CanInterrupt = true;
                        }
                    }
                }

                // 震屏
                ntf = action.GetNotifier("Shake");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null)
                    {
                        foreach (var i in nplist)
                        {
                     //       player.OnShakeNotify(Skill, Rune, i);
                        }
                    }
                }
                // 战斗音效
                ntf = action.GetNotifier("Sound");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null)
                    {
                        foreach (var i in nplist)
                        {
//                             var sountemp = CSCommon.Data.Sound.SoundTemplateManager.Instance.FindSound(Rune.AttackSoundId);
//                             if(sountemp !=null)
//                                 player.PlaySound(sountemp);
                        }
                    }
                }

            }
        }
    //    CSCommon.Data.Skill.RuneTemplate.NotifyEffect mCurrentEffect = null;

        public void ProcEffectNotifiers(CSUtility.Animation.AnimationTree anim)
        {
            var player = Host as Client.Role.RoleActor;
            if (player == null)
                return;

            var action = anim.Action;
            if (action != null)
            {
                var notifyList = action.GetNotifiers(typeof(CSUtility.ActionNotify.EffectActionNotifier));
                foreach (var notify in notifyList)
                {
                    if (notify == null)
                        continue;

                    var nplist = notify.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null) //默认同一个notify只播一次
                    {
                        foreach (var i in nplist)
                        {
                            if (mCrrentEffectNotifyName == i.NotifyName && Skill.Template.AttackEffectRepeatAdd == false)
                                continue;

                            if (Skill != null)
                            {
                                mCrrentEffectNotifyName = i.NotifyName;
                                player.AddEffect(Skill.Template.AttackNotifyEffects, i.HeaderName);
                            }                            
                        }
                    }                    
                }
            }
        }
        public void ProcMoveAttackNotifiers(CSUtility.Animation.AnimationTree anim,long elapsedMillisecond)
        {
            var player = Host as Client.Role.RoleActor;
            if (player == null || player.IsChielfPlayer() ==false)
                return;
            var action = anim.Action;
            if (action != null)
            {
                    // 移动攻击
                var ntf = action.GetNotifier("StartMoveAttack");
                if (ntf != null)
                {
                    var nplist = ntf.GetNotifyPoints(NotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null && nplist.Count != 0)
                    {
                        foreach (var i in nplist)
                        {
                            //System.Diagnostics.Debug.WriteLine(string.Format("mStartJumpPos == {0}",player.Placement.GetLocation()));
                     //       System.Diagnostics.Debug.WriteLine(string.Format("StartMoveAttack"));
                       //     player.RoleData.BoolDoMoveOnAttack = true;
                        }
                    }
                 //   if (player.RoleData.BoolDoMoveOnAttack == true)
                    {
                       // System.Diagnostics.Debug.WriteLine(string.Format("OnDoCanMoveAttack"));
                   //     FrameSet.Role.ChiefRoleActorController.Instance.OnDoCanMoveAttack(player, elapsedMillisecond);
                    }
                }
            }
        }

        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

        }
        public override void OnUserTick(long elapsedMillisecond)
        {
            base.OnUserTick(elapsedMillisecond);
            var player = Host as Client.Role.RoleActor;
            if (player == null)
                return;

            var anim = player.FSMGetCurrentAnimationTree();
            if (anim != null)
            {
                ProcShootNotifiers(anim);
                ProcEffectNotifiers(anim);
                ProcMoveAttackNotifiers(anim, elapsedMillisecond);
            }

        }

        public override void OnExitState()
        {
            base.OnExitState();
            mCrrentEffectNotifyName = "";
            mStartJumpPos = SlimDX.Vector3.Zero;
            var player = Host as Client.Role.RoleActor;

            RemoveSocketEffect();
            Skill = null;
        }


        public override bool OnActionFinished()
        {
            if (!base.OnActionFinished())
            {
                return false;
            }
            return true;
            //base.OnActionFinished();
           // System.Diagnostics.Debug.WriteLine(string.Format("mStartJumpPos == {0}", MidLayer.IEngine.Instance.GetFrameMillisecond()));
        }

        public SlimDX.Vector3 GetTargetPos(Client.Role.RoleActor role, GameData.Skill.SkillTemplate skillTemplate, SlimDX.Vector3 startPos)
        {
            var offsetMaxDistance = 0;//runeTemplate.RuneLevelParams[StayAttackParameter.RuneLevel].OffsetDistance;//是否超出技能释放距离
            if (offsetMaxDistance == 0)
            {
                offsetMaxDistance = 9;
            }
            var playerPos = role.Placement.GetLocation();
            var fireDir = StayAttackParameter.tarPos - playerPos;
            fireDir.Y = 0;
            fireDir.Normalize();
            var jumpPos = playerPos +fireDir* offsetMaxDistance;//要跳的位置
         //   SlimDX.Vector3 pathPoint;
//             var navResult = Navigation.INavigation.Instance.GetFarthestPathPointFromStartInLine(Guid.Empty, startPos.X, startPos.Z, jumpPos.X, jumpPos.Z, out pathPoint.X, out pathPoint.Z);
//             if (navResult)
//             {
//                 var resultY = role.GetAltitude(pathPoint.X, pathPoint.Z);
//                 pathPoint.Y = resultY;
//                 return pathPoint;
//             }
            return startPos;
        }

        public void RemoveSocketEffect()
        {
// 
//             if (string.IsNullOrEmpty(Rune.RemoveSocketName))
//                 return;
            var player = Host as Client.Role.RoleActor;
            if (player == null)
                return;
     //       var parentMesh = player.Visual as Client.Mesh.Mesh;
           // parentMesh.RemoveSocketItem(Rune.RemoveSocketName);
        }
    }
}
