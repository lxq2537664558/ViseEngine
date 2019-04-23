using System;
using System.Collections.Generic;
using System.Text;
using GameData.Skill;

namespace Client.GameState
{
    [CSUtility.AISystem.Attribute.StatementClass("被攻击状态", CSUtility.Helper.enCSType.Client)]
    public class BeAttack : GameData.AI.States.BeAttack
    {
        SkillData Skill;
      //  Client.Role.RoleActor Attacker = null;
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
            //mBeHitBackTime = 0;
            mBitHitOffPos = SlimDX.Vector3.Zero;
            var role = Host as Client.Role.RoleActor;
            if (role == null)
                return;

            Skill = new SkillData();
            Skill.TemplateId = BeAttackParameter.SkillId;
           
//             var att  = role.OnBeAttack(Skill, Rune, param.AttackerSingleId);
//             if (att !=null)
//             {
//                 Attacker = att;
//             }
            
        }
        
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

            if (Skill == null)
                return;
            var role = Host as Client.Role.RoleActor;
//             var param = Parameter as IBeAttackParameter;
//             if (Rune.SummonOffsetType == CSCommon.Data.Skill.EOffsetType.OffsetTarget && param.Duration > 0)
//             {
//                 if (Attacker == null  || Attacker.RoleData.RoleType !=FrameSet.Role.EClientRoleType.Summon)
//                     return;
// 
//                 var dir = Attacker.Placement.GetLocation() - role.Placement.GetLocation();
//                 dir.Y = 0;
// 
//                 if (dir.LengthSquared() < 0.1f)
//                     return;
//                 dir.Normalize();
// 
//                 var moveStep = Rune.SummonOffsetSpeed * (float)elapsedMillisecond / 1000.0f;
//                 role.Placement.Move(ref dir, moveStep);
//                 role.Placement.SetRotationY(-dir.Z, -dir.X, role.RoleTemplate.MeshFixAngle);
//             }
         //   BeHitBack(elapsedMillisecond);
        }

       // long mBeHitBackTime = 0;
      //  float offDistance = 0;
        SlimDX.Vector3 mBitHitOffPos = SlimDX.Vector3.Zero;
//         void BeHitBack(long elapsedMillisecond)
//         {
//             var role = Host as Client.Role.RoleActor;
//             if (role == null || Attacker==null)
//                 return;
//             var ower = Attacker;
//             if (Attacker.RoleData.RoleType ==Client.Role.EClientRoleType.Summon)
//             {
//                 ower = Attacker.OwnerRole;
// //                 if (ower != null)
// //                 {
// //                     Attacker = ower;
// //                 }
//             }
//             if (Rune.OffsetType == CSCommon.Data.Skill.EOffsetType.HitBack && BeAttackParameter.Duration > 0)
//             {
//                 if (ower == null)
//                     return;
//                 if (mBeHitBackTime > BeAttackParameter.Duration)
//                     return;
//             //    var lifeTime = BeAttackParameter.Duration - this.mLiveTime;
// 
//                 var dir = role.Placement.GetLocation() - ower.Placement.GetLocation();
//                 dir.Y = 0;
//                 if (dir.LengthSquared() < 0.1f)
//                     return;
//                 dir.Normalize();
//                 if (mBeHitBackTime ==0)
//                 {
//                     mBitHitOffPos = role.Placement.GetLocation() + dir * Rune.GetRuneLevelParam((byte)BeAttackParameter.RuneLevel).OffsetDistance;
//                     mBitHitOffPos = FindTargetPos(role, mBitHitOffPos);
//                     offDistance = SlimDX.Vector3.Distance(role.Placement.GetLocation(),mBitHitOffPos);
//                 }
//                 if (offDistance == 0)
//                 {
//                     return;
//                 }
// 
//                 var moveStep =offDistance * (float)elapsedMillisecond / BeAttackParameter.Duration;
//                 var nextStepPos = role.Placement.GetLocation() + moveStep * dir;
//                 var nextDir = mBitHitOffPos - nextStepPos;
//                 if (SlimDX.Vector3.Dot(dir, nextDir) < 0)
//                     return;
//                 role.Placement.Move(ref dir, moveStep);
//                 mBeHitBackTime += elapsedMillisecond;
//             }
//         }


//         public SlimDX.Vector3 FindTargetPos(Role.RoleActor role, SlimDX.Vector3 tarpos)
//         {
//             var playerPos = role.Placement.GetLocation();
//          //   var navResult = Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Error;//是否可寻路到
//             var pathPoints = new List<SlimDX.Vector2>();
//        //     int iCurrFindPathCount = 0;
//             var dir = tarpos - playerPos;
//             dir.Y = 0;
//             dir.Normalize();
//             var tarDistance = SlimDX.Vector3.Distance(playerPos, tarpos);
//             SlimDX.Vector3 pathPoint;
//             var navResult = Navigation.INavigation.Instance.GetFarthestPathPointFromStartInLine(Guid.Empty, playerPos.X, playerPos.Z, tarpos.X, tarpos.Z, out pathPoint.X, out pathPoint.Z);
//             if (navResult)
//             {
//                 var resultY = role.GetAltitude(pathPoint.X, pathPoint.Z);
//                 pathPoint.Y = resultY;
//                 return pathPoint;
//             }
//             return playerPos;
// //             while (navResult != Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Success &&
// //                    navResult != Navigation.INavigationWrapper.enNavFindPathResult.ENFR_SESame)
// //             {
// //                 navResult = Navigation.INavigation.Instance.FindPath(playerPos.X, playerPos.Z, tarpos.X, tarpos.Z, 3, ref pathPoints);
// //                 if (navResult == Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Success ||
// //                     navResult == Navigation.INavigationWrapper.enNavFindPathResult.ENFR_SESame)
// //                 {
// //                     break;
// //                 }
// //                 tarDistance -= 2;
// //                 if (tarDistance < 0)
// //                 {
// //                     tarDistance = 0;
// //                 }
// //                 tarpos = dir * tarDistance + playerPos;
// //                 iCurrFindPathCount++;
// //                 if (iCurrFindPathCount > 10)
// //                     break;
// //             }
// // 
// //             if (navResult == Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Success)
// //             {
// //                 var findPoint = new SlimDX.Vector2(pathPoints[pathPoints.Count - 1].X,pathPoints[pathPoints.Count - 1].Y);
// //                 var resultY = role.GetAltitude(findPoint.X, findPoint.Y);
// //                 tarpos.X = findPoint.X;
// //                 tarpos.Z = findPoint.Y;
// //             }
// //             else
// //             {
// //                 tarpos = playerPos;
// //             }
// //             return tarpos;
//         }

        public override void OnEnterState()
        {
            base.OnEnterState();

        }
        public override void OnExitState()
        {
            base.OnExitState();
         //   mBeHitBackTime = 0;
        }
        public override void ExitBeAttack()
        {
            CanInterrupt = true;
            var role = Host as Client.Role.RoleActor;
            if (role != null && role.RoleData.RoleHP == 0)
            {
                this.ToState("Death", null);
            }
            else
            {
                this.ToState("Idle", null);
            }
        }

    }
}
