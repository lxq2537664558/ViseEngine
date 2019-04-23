using System;
using System.Collections.Generic;

using System.Text;
using SlimDX;

namespace Client.GameState
{
    [CSUtility.AISystem.Attribute.StatementClass("行走状态", CSUtility.Helper.enCSType.Client)]
    public class Walk : GameData.AI.States.Walk
    {
        public delegate void FOnWalkTickCB(long elapsedMillisecond);
        public event FOnWalkTickCB OnWalkTickCB;

        public Role.RoleActor mLockOnRole = null;

        public override void OnEnterState()
        {
            base.OnEnterState();
            var role = Host as Role.RoleActor;
            if (role == null)
                return;
            if (role.RoleData.RoleType ==Role.EClientRoleType.Summon)
            {
                mLockOnRole = Client.Stage.MainStage.Instance[role.RoleData.SummonData.LockOnRoleId];
            }
        }


        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

       //     var elapsedHighPrecision = MidLayer.IEngine.Instance.GetElapsedHighPrecision();
         //   base.HP_Tick(MidLayer.IEngine.Instance.GetFrameMillisecond());

            if (OnWalkTickCB != null)
                OnWalkTickCB(elapsedMillisecond);

            var role = Host as Role.RoleActor;
            if (role == null)
                return;
            if (role.RoleData.RoleType == Role.EClientRoleType.Summon)
            {
                if (mLockOnRole != null)
                {
                    var lockpos = mLockOnRole.Placement.GetLocation();
                    lockpos.Y = mLockOnRole.GetAltitude(lockpos.X,lockpos.Z) + mLockOnRole.RoleTemplate.HalfHeight;
                    var distance = lockpos - WalkParameter.FinalPosition;
   
                    if (distance.Length() > role.RoleData.SummonData.SkillData.Template.Radius)
                    {
                        WalkParameter.TargetPosition = lockpos;
                        WalkParameter.FinalPosition = lockpos;
                    }
                    var dist = role.Placement.GetLocation() - lockpos;
                    if (dist.Length() < role.RoleData.SummonData.SkillData.Template.Radius +mLockOnRole.RoleTemplate.Radius/2)
                    {
                        if (role.RoleData.SummonData.SkillData.Template.Immediate2Death)
                            this.ToState("Death", null);
                    }
                }
            }

        }

        //public static Int64 STickMovementElapse;
        //public static float STickMovementDistance;
        SlimDX.Vector3 PreMoveDir = new SlimDX.Vector3(0,0,1);
        long mMoveTickTime = 0;
        protected override bool TickMovement(Int64 elapsedHighPrecision, Int64 frameMillisecond)
        {
            if (Host.Actor == null || Host.Actor.Placement == null)
                return false;
            var role = Host.Actor as Client.Role.RoleActor;
            if (role == null)
                return false;
            var time = CSUtility.Helper.LogicTimer.GetTickCount();
            if (mMoveTickTime == 0)
                mMoveTickTime = time;

            var  elapsed = time - mMoveTickTime;
            if (elapsed == 0)
                return true;

            var moveDist = ((float)elapsedHighPrecision) * 0.001f * WalkParameter.MoveSpeed /** 1.2f*/;
            mMoveTickTime = time;

            var curLoc = Host.Actor.Placement.GetLocation();
            var moveDir = WalkParameter.TargetPosition - curLoc;//要走的方向
            moveDir.Y = 0;
            var dist = moveDir.Length();
            moveDir.Normalize();
            var targetPos = curLoc + moveDir * moveDist;//逻辑的位置
            var blockpos = curLoc + (moveDist + role.RoleTemplate.Radius) * moveDir;
            if (IsBlock(targetPos))
            {
                SetPropertyPos(moveDir, moveDist);
                return true;
            }            

            if (moveDist >dist)//这里不能确定自己是肯定走到要走到位置，要预判
            {
                var tarLoc = WalkParameter.TargetPosition;
                if (Host.Actor.Gravity != null)
                    tarLoc.Y = Host.GetAltitude(tarLoc.X, tarLoc.Z);
                Host.Actor.Placement.SetLocation(ref tarLoc);
                return false;//到了
            }
            else
            {
                Host.Actor.Placement.SetRotationY(moveDir.Z, moveDir.X, role.RoleTemplate.MeshFixAngle, false);
                PreMoveDir = moveDir;
                SlimDX.Vector3 tarLoc;
                if (role.RoleData.RoleType == Client.Role.EClientRoleType.Summon)
                {
                    if (false == SommonMove(ref moveDir, moveDist))
                        return false;
                }
                else
                {
                    if (false == Host.Actor.Placement.Move(ref moveDir, moveDist))
                        return false;
                }
                tarLoc = Host.Actor.Placement.GetLocation();//移动成功，并且考虑重力时设定高度
                //                 if (Host.Actor.Gravity != null)
                //                 {
                //                     tarLoc.Y = Host.GetAltitude(tarLoc.X, tarLoc.Z);
                //                     Host.Actor.Placement.SetLocation(ref tarLoc);
                //                 }
                if (SlimDX.Vector3.Distance(tarLoc, WalkParameter.TargetPosition) < 0.2)//小于0.02就到了
                    return false;
                return true;
            }
        }

        public override void OnExitState()
        {
            base.OnExitState();

            var role = Host.Actor as Client.Role.RoleActor;
            if (role.IsChielfPlayer())
            {
//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
//                 ServerCommon.Planes.H_PlayerInstance.smInstance.RPC_UpdatePosition(pkg, role.Placement.GetLocation(), role.Placement.GetDirection());
//                 pkg.DoClient2PlanesPlayer(MidLayer.IEngine.Instance.Client.GateSvrConnect);
            }
            if (role.RoleData != null && (role.RoleData.RoleType == Client.Role.EClientRoleType.Monster || role.RoleData.RoleType == Client.Role.EClientRoleType.OtherPlayer))
            {
               // role.UpdateClientPos();
            }
        }

        public override void OnCollideScene()
        {
            base.OnCollideScene();

            var summon = Host as Client.Role.RoleActor;
            if (summon.RoleData.RoleType == Client.Role.EClientRoleType.Summon /*&& summon.OwnerRole == MidLayer.IEngine.Instance.Client.ChiefRole*/)
            {
                //                 RPC.PackageWriter pkg = new RPC.PackageWriter();
                //                 ServerCommon.Planes.H_PlayerInstance.smInstance.RPC_OnCollideScene(pkg, summon.SingleId);
                //                 pkg.DoClient2PlanesPlayer(MidLayer.IEngine.Instance.Client.GateSvrConnect);
                // 
                var deathState = summon.AIStates.GetState("Death");
                if (deathState != null)
                {
                    TargetToState(summon, "Death", deathState.Parameter);
                }
            }
        }

        public override bool IsBlock(Vector3 pos)
        {
            var role = Host as Role.RoleActor;
            if (role == null || role.RoleData.RoleType != Role.EClientRoleType.ChiefPlayer)
                return false;
            
//             if(CollideRoles(pos))
//             {
//                 return true;
//             }

            var rolePos = role.Placement.GetLocation();
            var dir = pos - rolePos;
            dir.Normalize();
            var newLoc = rolePos + dir * role.RoleTemplate.Radius;

            ///CCore.Navigation.Navigation.Instance.NavigationData.SetDynamicNavData(GameData.Support.ConfigFile.Instance.DefaultMapId, rolePos.X, rolePos.Z, role.RoleTemplate.Radius, false);
            return CCore.Navigation.Navigation.Instance.HasBarrier(Stage.MainStage.Instance.CurMapId, newLoc.X, newLoc.Z, pos.X, pos.Z);

        }
//         public override void OnCollideRoles()
//         {
//             base.OnCollideRoles();
// 
//             var summon = Host as FrameSet.Role.RoleActor;
//             if (summon.RoleData.RoleType != Role.EClientRoleType.Summon)
//                 return;
// 
//             var st = summon.RoleData.SummonData.SkillData.Template;
//             if (st == null)
//                 return;
// 
//             var rt = summon.RoleData.SummonData.RuneData.Template;
//             if (rt == null)
//                 return;
// 
//             //if (st.PassThroughRole == false)
//             {//是否穿过，这个被Skill的MaxDamageTargetNumber控制，这个在服务器上处理了，客户端只管请求计算，如果超过了，服务器会通知客户端summon死亡
//                 byte count = 0;
//                 foreach (Role.RoleActor target in summon.CollideRoles)
//                 {
//                     //var lostControl = target.AIStates.GetState("LostControl");
//                     //if (lostControl != null)
//                     //{
//                     //    var p = lostControl.Parameter as CSCommon.AISystem.States.ILostControlParameter;
//                     //    CSCommon.AISystem.State.TargetToState(target, "LostControl", p);
//                     //}
//                     count++;
//                 }
// 
//                 // 通知服务器端校验
//                 RPC.DataWriter dw = new RPC.DataWriter();
//                 dw.Write(count);
//                 foreach (Role.RoleActor i in summon.CollideRoles)
//                 {
//                     dw.Write(i.SingleId);
//                     dw.Write(i.Placement.GetLocation());
//                 }
// 
//                 dw.Write(summon.Placement.GetLocation());
// 
//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
//                 ServerCommon.Planes.H_PlayerInstance.smInstance.RPC_SummonCollideRole(pkg, dw, summon.SingleId);
//                 pkg.DoClient2PlanesPlayer(MidLayer.IEngine.Instance.Client.GateSvrConnect);
// 
//                 if(rt.DeathOnDamage==true)
//                 {
//                     var deathState = summon.AIStates.GetState("Death");
//                     if (deathState != null)
//                     {
//                         CSCommon.AISystem.State.TargetToState(summon, "Death", deathState.Parameter);
//                     }
//                 }
//             }
//         }

        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
            SetStateAction();
        }

        public override  bool SommonMove(ref SlimDX.Vector3 dir, float delta)
        {
            var  actor = Host.Actor;
            if (actor == null)
                return false;
            var role = Host as Role.RoleActor;
            if (role == null || role.RoleData.RoleType !=Role.EClientRoleType.Summon)
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
            var x = System.Math.Sqrt(dir.X *dir.X + dir.Z * dir.Z);
            Host.Actor.Placement.SetRotationZ(dir.Y, (float )x);
            return true;
        }


        float mRadius = 3f;
        public bool CollideRoles(SlimDX.Vector3 nextPos)
        {            
            var role = Host as Role.RoleActor;
            if (role == null)
                return false;            

            float fHalfHeight = 5;            
            
            var pos = role.Placement.GetLocation();
            var startPos = new SlimDX.Vector3(pos.X - mRadius, pos.Y - fHalfHeight, pos.Z - mRadius);
            var endPos = new SlimDX.Vector3(pos.X + mRadius, pos.Y + fHalfHeight, pos.Z + mRadius);

            var roles = CCore.Engine.Instance.Client.MainWorld.GetActors(ref startPos, ref endPos, 0);
            nextPos.Y = 0;
            foreach (var i in roles)
            {
                if (i == role)
                    continue;
                var targetRole = i as Role.RoleActor;
                if (targetRole == null)
                    continue;
                if (targetRole.RoleTemplate.MonsterType > GameData.Role.MonsterType.Building)
                    continue;
                if (targetRole.SingleId == role.SingleId)
                    continue;    

                var rolePos = targetRole.Placement.GetLocation();         
                rolePos.Y = 0;
                
                if (SlimDX.Vector3.Distance(nextPos, rolePos) <targetRole.RoleTemplate.Radius)
                {                    
                    var dir = rolePos - nextPos;
                    dir.Normalize();

                    var newLoc = rolePos - dir * (role.RoleTemplate.Radius + targetRole.RoleTemplate.Radius + 0.05f);
                    newLoc.Y = role.GetAltitude(newLoc.X, newLoc.Z);
                    role.Placement.SetLocation(ref newLoc);
                    return true;
                }
            }
            return false;
        }

        public void SetPropertyPos(SlimDX.Vector3 dir, float dist)
        {
            var role = Host as Role.RoleActor;
            if (role == null)
                return;
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
            }
            else if(!IsBlock(rightForwardLoc))
            {
                role.Placement.SetRotationY(rightForwardDir.Z, rightForwardDir.X, role.RoleTemplate.MeshFixAngle);
                //rightLoc.Y = Host.GetAltitude(rightLoc.X, rightLoc.Z);
                Host.Actor.Placement.SetLocation(ref rightForwardLoc);
            }            
            else if (!IsBlock(leftLoc))
            {
                role.Placement.SetRotationY(leftDir.Z, leftDir.X, role.RoleTemplate.MeshFixAngle);
                //leftLoc.Y = Host.GetAltitude(leftLoc.X, leftLoc.Z);
                Host.Actor.Placement.SetLocation(ref leftLoc);
            }
            else if (!IsBlock(rightLoc))
            {
                role.Placement.SetRotationY(rightDir.Z, rightDir.X, role.RoleTemplate.MeshFixAngle);
                //rightLoc.Y = Host.GetAltitude(rightLoc.X, rightLoc.Z);
                Host.Actor.Placement.SetLocation(ref rightLoc);
            }                  
        }
    }
}
