using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall.Role
{
    public class RolePlacement : CSUtility.Component.TrackerPlacement
    {
        public RolePlacement(RoleActor host)
                    : base(host)
        {

        }

        public override bool SetLocation(ref SlimDX.Vector3 loc)
        {
            var role = mHostActor as Role.RoleActor;
            if (role == null)
                return false;

            if (role.RoleTemplate != null)
            {
                loc += role.RoleTemplate.PosOffset;
            }

            base.SetLocation(ref loc);

            OnUpdatePosition(ref mLocation);
            
            if (role.RoleCreateType != GameData.Role.ERoleType.Monster && role.RoleCreateType != GameData.Role.ERoleType.Player)
                return false;

            if (role.RoleTemplate.MonsterType <= GameData.Role.MonsterType.Building)
            {                
                if (role.CurrentState.StateName == "Death")
                {                               
                    role.HostMap.MapTemplate.SetDynamicNavData(role.mBlockPos.X, role.mBlockPos.Z, role.RoleTemplate.Radius, false);                    

                    return true;
                }      

                if (loc != role.mBlockPos)
                {
                    lock (role.HostMap) 
                    {
                        role.HostMap.MapTemplate.SetDynamicNavData(role.mBlockPos.X, role.mBlockPos.Z, role.RoleTemplate.Radius, false);
                        role.HostMap.MapTemplate.SetDynamicNavData(loc.X, loc.Z, role.RoleTemplate.Radius, true);
                        role.mBlockPos = loc;
                    }
                }
            }
            return true;
        }

        public override bool Move(ref SlimDX.Vector3 dir, float delta)
        {
            var actor = GetActor() as Role.RoleActor;
            if (actor == null || actor.HostMap.IsNullMap)
                return false;

            SlimDX.Vector3 targetPoint = mLocation + dir * delta;

            if (actor.StopMoveOnBlock && actor.HostMap.IsBlocked(targetPoint.X, targetPoint.Z))
                return false;

            if (actor.RoleCreateType != GameData.Role.ERoleType.Summon)
            {
                targetPoint.Y = actor.HostMap.GetAltitude(targetPoint.X, targetPoint.Z);
            }
            else
            {
                targetPoint.Y = mLocation.Y;
            }
            SetLocation(ref targetPoint);

            //   OnUpdatePosition(ref mLocation);
            return true;
        }

        bool mIsUpdate2Client = true;
        public bool IsUpdate2Client
        {
            get { return mIsUpdate2Client; }
            set { mIsUpdate2Client = value; }
        }
        SlimDX.Vector3 mPrevPos;
        Int64 mPrevUpdateTime;
        private void OnUpdatePosition(ref SlimDX.Vector3 loc)
        {
            RoleActor role = mHostActor as RoleActor;
            if (role != null)
            {
                role.OnPlacementUpdatePosition(ref loc);

                if (mIsUpdate2Client == false)
                    return;
                Int64 time = CSUtility.Helper.LogicTimer.GetTickCount();
                if (time - mPrevUpdateTime > 3000)//3秒钟必然同步一次
                {
                    mPrevUpdateTime = time;
                    if (!role.HostMap.IsNullMap&&role.StateNotify2Remote)
                        role.HostMap.RolePositionChanged(role, ref loc);
                }
                else
                {
                    float dist = SlimDX.Vector3.Distance(loc, mPrevPos);
                    if (dist > 0.25)
                    {
                        mPrevPos = loc;
                        if (!role.HostMap.IsNullMap&&role.StateNotify2Remote)
                            role.HostMap.RolePositionChanged(role, ref loc);
                    }
                }
            }
        }

        float mPrevAngle;
        protected override void OnUpdateRoationY(float angle)
        {
            base.OnUpdateRoationY(angle);
            RoleActor role = mHostActor as RoleActor;
            if (role != null)
            {
                role.OnPlacementUpdateDirectionY(angle);
            }

            if (mIsUpdate2Client == false)
                return;

            Int64 time = CSUtility.Helper.LogicTimer.GetTickCount();
            if (time - mPrevUpdateTime > 3000)//3秒钟必然同步一次
            {
                mPrevUpdateTime = time;
                if (!role.HostMap.IsNullMap)
                    role.HostMap.RoleDirectionChanged(role, angle);
            }
            else
            {
                float dist = System.Math.Abs(mPrevAngle - angle);
                if (dist > System.Math.PI * 5 / 180)
                {
                    mPrevAngle = angle;

                    if (!role.HostMap.IsNullMap)
                        role.HostMap.RoleDirectionChanged(role, angle - role.RoleTemplate.MeshFixAngle);
                }
            }
        }
    }
}
