using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Server.Hall.ServerStates
{
    [CSUtility.AISystem.Attribute.StatementClass("行走状态", CSUtility.Helper.enCSType.Server)]
    public class Walk : GameData.AI.States.Walk
    {
        public override void OnEnterState()
        {
            base.OnEnterState();
            var role = Host as Role.RoleActor;
            var wp = WalkParameter;
            var summmon = role as Role.Summon.SummonRole;
            if (summmon != null)
                mLockOnRole = summmon.OwnerRole.HostMap.GetRole(summmon.SummonData.LockOnRoleId);

            if ( role.IsValidMoveSpeed(wp.MoveSpeed, wp.Run)==false )
            {
                //惩罚客户端，因为他作弊了
                role.OnClientCheat(100, "移动速度参数异常");
            }
          //  Log.FileLog.WriteLine(string.Format("Enter, Name:{2} , StartPos:{0}, EndPos:{1}", role.Placement.GetLocation(), wp.TargetPosition, role.RoleTemplate.RoleName));
        }

        public override void OnExitState()
        {
            base.OnExitState();
            var role = Host as Role.RoleActor;
            var wp = WalkParameter;
         //   Log.FileLog.WriteLine(string.Format("Exit, Name:{2} , StartPos:{0}, EndPos:{1}", role.Placement.GetLocation(), wp.TargetPosition, role.RoleTemplate.RoleName));
        }

        public override void OnReEnterState()
        {
            base.OnReEnterState();
            var role = Host as Role.RoleActor;
            var wp = WalkParameter;
        //    Log.FileLog.WriteLine(string.Format("ReEnter, Name:{2} , StartPos:{0}, EndPos:{1}", role.Placement.GetLocation(), wp.TargetPosition, role.RoleTemplate.RoleName));
        }
        public override void Tick(long elapsedMillisecond)
        {
            var role = Host as Role.RoleActor;
            
            base.Tick(elapsedMillisecond);
            //base.HP_Tick(elapsedMillisecond * 1000, ServerCommon.IServer.timeGetTime());

            if (role.IsValidMoveSpeed(WalkParameter.MoveSpeed, WalkParameter.Run) == false)
            {
                //惩罚客户端，因为他作弊了
                role.OnClientCheat(100, "移动速度参数异常");
            }

//             var pos = role.Placement.GetLocation();
//             var blocks = role.BlockRole(pos);
//             if(blocks.roles.Count !=0)
//             {
//                 var dir = pos - blocks.roles[0].Placement.GetLocation();
//                 var nowDir = SlimDX.Vector3.Cross(dir,SlimDX.Vector3.UnitY);
//                 nowDir.Y = 0;
//                 nowDir.Normalize();
//                 WalkParameter.TargetPosition = pos + nowDir * role.RoleTemplate.Radius;
//             } 
             if(role.RoleCreateType ==GameData.Role.ERoleType.Summon)
            {
                var summmon = role as Role.Summon.SummonRole;
                if (mLockOnRole != null)
                {
                    var lockpos = mLockOnRole.Placement.GetLocation();
                    lockpos.Y = mLockOnRole.GetAltitude(lockpos.X, lockpos.Z) + mLockOnRole.RoleTemplate.HalfHeight;
                    var distance = lockpos- WalkParameter.FinalPosition;
                    if(distance.Length() >summmon.SummonData.SkillData.Template.Radius)
                    {
                        WalkParameter.TargetPosition = lockpos;
                        WalkParameter.FinalPosition = lockpos;
                    }
                }
            }
            
        }

        public override bool IsBlock(Vector3 pos)
        {
            var role = Host as Role.RoleActor;
            if (role == null)
                return false;
            if (role.RoleCreateType == GameData.Role.ERoleType.Summon)
                return false;
            if (role.IsBlock(pos) && !role.IsPlayerInstance())
            {                
                ToState("Idle", null);       
                return true;
            }            
            return false;            
        }

        public override void OnArrived()
        {
            var role = Host as Role.RoleActor;
            if (role !=null && role.RoleCreateType == GameData.Role.ERoleType.Summon)
            {
                if (WalkParameter.TargetPositions.Count > 0)
                {
                    WalkParameter.TargetPosition = WalkParameter.TargetPositions.Dequeue();
                }
                else
                {
                    this.ToState(role.AIStates.DefaultState.StateName, null);
                }
            }
            else
            {
                base.OnArrived();
            }

        }

        protected override bool TickMovement(Int64 elapsedHighPrecision, Int64 frameMillisecond)
        {
            if (Host.Actor == null || Host.Actor.Placement == null)
                return false;
            var role = Host as Role.RoleActor;
            var curLoc = Host.Actor.Placement.GetLocation();
            var MoveDir = WalkParameter.TargetPosition - curLoc;
            MoveDir.Y = 0;
            MoveDir.Normalize();
            var moveDist = ((float)elapsedHighPrecision) * 0.001f * WalkParameter.MoveSpeed;
            curLoc.Y = WalkParameter.TargetPosition.Y;
            float dist = SlimDX.Vector3.Distance(WalkParameter.TargetPosition, curLoc);
            if (moveDist > dist)
            {
                if (WalkParameter.TargetPositions.Count > 0)
                {
                    WalkParameter.TargetPosition = WalkParameter.TargetPositions.Dequeue();
                    var newPos = curLoc + moveDist * MoveDir;
                    if (Host.Actor.Gravity != null)
                    {
                        newPos.Y = Host.GetAltitude(newPos.X, newPos.Z);
                    }
                    var blockpos = curLoc + (moveDist + role.RoleTemplate.Radius) * MoveDir;
                    if (IsBlock(blockpos))
                        return false;

                    Host.Actor.Placement.SetLocation(ref newPos);
                    Host.Actor.Placement.SetRotationY(MoveDir.Z, MoveDir.X, false);
                    return true;
                }
                else
                {
                    var newPos1 = WalkParameter.TargetPosition;
                    if (Host.Actor.Gravity != null)
                        newPos1.Y = Host.GetAltitude(newPos1.X, newPos1.Z);
                    //if (IsBlock(newPos1))
                     //   return false;
                    Host.Actor.Placement.SetLocation(ref newPos1);
                    return false;
                }
            }
            else
            {
                if (role != null && role.RoleCreateType == GameData.Role.ERoleType.Summon)
                {
                    SommonMove(ref MoveDir,moveDist);
                    return true;
                }
                var tarLoc = curLoc + MoveDir * moveDist;
                if (Host.Actor.Gravity != null)
                {
                    tarLoc.Y = Host.GetAltitude(tarLoc.X, tarLoc.Z);
                }
                var blockpos = curLoc + (moveDist + role.RoleTemplate.Radius) * MoveDir;
                if (IsBlock(blockpos))
                    return false;
                Host.Actor.Placement.SetLocation(ref tarLoc);
                Host.Actor.Placement.SetRotationY(MoveDir.Z, MoveDir.X, false);
                return true;
            }
        }

        Role.RoleActor mLockOnRole = null;
        public override bool SommonMove(ref SlimDX.Vector3 dir, float delta)
        {
            var actor = Host.Actor;
            if (actor == null)
                return false;
            var role = Host as Role.RoleActor;
            if (role == null || role.RoleCreateType != GameData.Role.ERoleType.Summon)
                return false;

            SlimDX.Vector3 start = actor.Placement.GetLocation();
            if (mLockOnRole != null)
            {
                var lockpos = mLockOnRole.Placement.GetLocation();
                lockpos.Y = mLockOnRole.GetAltitude(lockpos.X, lockpos.Z) + mLockOnRole.RoleTemplate.HalfHeight;
                dir = lockpos - start;
            }
            dir.Normalize();
            SlimDX.Vector3 newLoc;
            newLoc = start + dir * delta;
            Host.Actor.Placement.SetLocation(ref newLoc);
            var x = System.Math.Sqrt(dir.X * dir.X + dir.Z * dir.Z);
            Host.Actor.Placement.SetRotationZ(dir.Y, (float)x);
            return true;
        }

        public bool SetPropertyPos(SlimDX.Vector3 dir, float dist)
        {
            var role = Host as Role.RoleActor;
            if (role == null)
                return false;
            var pos = role.Placement.GetLocation();

            dir.Y = 0;
            dir.Normalize();
            var rightDir = SlimDX.Vector3.Cross(SlimDX.Vector3.UnitY, dir);
            rightDir.Normalize();
            var leftDir = -rightDir;
            var rightForwardDir = dir + rightDir;
            rightForwardDir.Normalize();
            var leftForwardDir = dir + leftDir;
            leftForwardDir.Normalize();

            var rightLoc = pos + dist * rightDir;
            var leftLoc = pos + dist * leftDir;
            var leftForwardLoc = pos + dist * leftForwardDir;
            var rightForwardLoc = pos + dist * rightForwardDir;

            if (!IsBlock(leftForwardLoc))
            {
                role.Placement.SetRotationY(leftForwardDir.Z, leftForwardDir.X, role.RoleTemplate.MeshFixAngle);
                //rightLoc.Y = Host.GetAltitude(rightLoc.X, rightLoc.Z);
                Host.Actor.Placement.SetLocation(ref leftForwardLoc);
                return true;
            }
            else if (!IsBlock(rightForwardLoc))
            {
                role.Placement.SetRotationY(rightForwardDir.Z, rightForwardDir.X, role.RoleTemplate.MeshFixAngle);
                //rightLoc.Y = Host.GetAltitude(rightLoc.X, rightLoc.Z);
                Host.Actor.Placement.SetLocation(ref rightForwardLoc);
                return true;
            }
            else if (!IsBlock(leftLoc))
            {
                role.Placement.SetRotationY(leftDir.Z, leftDir.X, role.RoleTemplate.MeshFixAngle);
                //leftLoc.Y = Host.GetAltitude(leftLoc.X, leftLoc.Z);
                Host.Actor.Placement.SetLocation(ref leftLoc);
                return true;
            }
            else if (!IsBlock(rightLoc))
            {
                role.Placement.SetRotationY(rightDir.Z, rightDir.X, role.RoleTemplate.MeshFixAngle);
                //rightLoc.Y = Host.GetAltitude(rightLoc.X, rightLoc.Z);
                Host.Actor.Placement.SetLocation(ref rightLoc);
                return true;
            }
            return false;
        }
    }
}
