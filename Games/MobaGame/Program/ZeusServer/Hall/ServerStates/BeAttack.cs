using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.ServerStates
{
    [CSUtility.AISystem.Attribute.StatementClass("被攻击状态", CSUtility.Helper.enCSType.Server)]
    public class BeAttack : GameData.AI.States.BeAttack
    {
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
        }
        public override void OnEnterState()
        {
            base.OnEnterState();
        }
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
        }

//         void SummonMoveOff(long elapsedMillisecond)
//         {
//             var role = Host as ServerCommon.Planes.Role.RoleActor;
//             if (Rune.Template == null)
//             {
//                 return;
//             }
//             if (Rune.Template.SummonOffsetType == CSCommon.Data.Skill.EOffsetType.OffsetTarget && BeAttackParameter.Duration > 0)
//             {
//                 if (Attacker == null || Attacker.RoleCreateType != CSCommon.Data.ERoleCreateType.Summon)
//                 {
//                     return;
//                 }
// 
//                 var dir = Attacker.Placement.GetLocation() - role.Placement.GetLocation();
//                 dir.Y = 0;
//                 if (dir.LengthSquared() < 0.1f)
//                     return;
//                 dir.Normalize();
//                 var moveStep = Rune.Template.SummonOffsetSpeed * (float)elapsedMillisecond / 1000.0f;
//                 var rolePlacement = role.Placement as ServerCommon.Planes.Role.RolePlacement;
// 
//                 if (rolePlacement != null)
//                 {
//                     rolePlacement.IsUpdate2Client = false;
//                     rolePlacement.Move(ref dir, moveStep);
//                     rolePlacement.SetRotationY(-dir.Z, -dir.X, role.RoleTemplate.MeshFixAngle, true);
//                     rolePlacement.IsUpdate2Client = true;
//                 }
//             }
// 
//         }

        SlimDX.Vector3 mBeHitBackPos = SlimDX.Vector3.Zero;
        SlimDX.Vector3 mBeHitBackStartPos = SlimDX.Vector3.Zero;
        void BeHitBack(long elapsedMillisecond)
        {
//             var role = Host as ServerCommon.Planes.Role.RoleActor;
//             if (role == null)
//                 return;
//             if (Attacker ==null)
//             {
//                 return;
//             }
//             var ower = Attacker;
//             if (Attacker.RoleCreateType == CSCommon.Data.ERoleCreateType.Summon)
//             {
//                 ower = Attacker.OwnerRole;
//             }
// 
//             if (Rune.Template.OffsetType == CSCommon.Data.Skill.EOffsetType.HitBack && BeAttackParameter.Duration > 0)
//             {
//                 if (ower == null)
//                     return;
// 
//                 var dir = role.Placement.GetLocation() - ower.Placement.GetLocation();
//                 dir.Y = 0;
//                 dir.Normalize();
// 
//                 if (mBeHitBackPos ==SlimDX.Vector3.Zero)
//                 {
//                     var tarpos = role.Placement.GetLocation() + dir * Rune.Template.GetRuneLevelParam(Rune.RuneLevel).OffsetDistance;
//                     mBeHitBackStartPos = role.Placement.GetLocation();
//                     mBeHitBackPos =FindTargetPos(role, ower, tarpos, dir);     
//                 }
//                 HitBackMovement(role,elapsedMillisecond,mBeHitBackStartPos,mBeHitBackPos,dir);
//             }
        }

        void HitBackMovement(Role.RoleActor role,Int64 elapsed,SlimDX.Vector3 startpos,SlimDX.Vector3 tarpos,SlimDX.Vector3 dir)
        {
            if (BeAttackParameter.Duration == 0)
                return;

            var distance =SlimDX.Vector3.Distance(startpos/*role.Placement.GetLocation()*/, tarpos) ;
            var offDistance = distance* elapsed / BeAttackParameter.Duration;
            var nowtarpos = startpos + dir * offDistance;
            if (SlimDX.Vector3.Distance(startpos, nowtarpos) > distance)
                return;
//             var rolePlacement = role.Placement as ServerCommon.Planes.Role.RolePlacement;
//             if (rolePlacement != null)
//             {
//               //  var update = rolePlacement.IsUpdate2Client;
//                 //rolePlacement.IsUpdate2Client = false;
//                 rolePlacement.Move(ref dir, offDistance);
//                 //rolePlacement.IsUpdate2Client = update;
//             }
        }

//         public SlimDX.Vector3 FindTargetPos(Role.RoleActor role, Role.RoleActor attacker, SlimDX.Vector3 tarpos,SlimDX.Vector3 dir)
//         {
//             SlimDX.Vector3  findPoint;
//             var playerPos = role.Placement.GetLocation();
//             var pathPoints = new List<SlimDX.Vector2>();
//             var pkg = new RPC.PackageWriter();
//             var tarDistance = SlimDX.Vector3.Distance(playerPos, tarpos);
// //             if (role.HostMap.MapSourceId == Guid.Empty)
// //                 findPoint = playerPos;
// //             else
// //             {
// //                 var navData = ServerCommon.Planes.Map.MapPathManager.Instance.GetGlobalMapNavigationAssistData(role.HostMap.MapSourceId);
// //                 if (navData == null)
// //                     return SlimDX.Vector3.Zero;
// // 
// //                 var navresult = role.HostMap.NavigationWrapper.GetFarthestPathPointFromStartInLine(role.HostMap.MapInstanceId, playerPos.X, playerPos.Z, tarpos.X, tarpos.Z, out findPoint.X, out findPoint.Z, navData.NavigationTileData);
// //                 if (navresult)
// //                 {
// //                     findPoint.Y = role.HostMap.GetAltitude(findPoint.X, findPoint.Z);
// //                 }
// //                 else
// //                 {
// //                     findPoint = playerPos;
// //                 }
// //             }
// 
//          //   return findPoint;
//             //HitBackMovement(role, attacker, findPoint, dir);
//         }

        public override void OnExitState()
        {
            base.OnExitState();
//             var role = Host as Role.RoleActor;
//             if (role == null)
//                 return;
        }

        public override void ExitBeAttack()
        {
            CanInterrupt = true;
            var role = Host as Role.RoleActor;
//             Role.RoleActor ower = Attacker;
//             if (role !=null && Skill !=null && Rune !=null && Attacker!=null)
//             {
//                 if (Attacker.RoleCreateType == CSCommon.Data.ERoleCreateType.Summon)
//                 {
//                     ower = Attacker.OwnerRole;
//                 }
//                 if (Rune.Template.SummonOffsetType == CSCommon.Data.Skill.EOffsetType.OffsetTarget)//此种移动是在移动之后计算伤害的
//                 {
//                     role.ProcHurt(ower,Skill, Rune, 0,1);
//                 }
//             }
// 
//             if (role != null && role.RoleHP <= 0)
//             {
//                 role.RoleHP = 0;
//                 CSCommon.AISystem.State.TargetToState(role, "Death", null);
//             }
//             else
            {
                this.ToState("Idle", null);
            }
        }
    }
}
