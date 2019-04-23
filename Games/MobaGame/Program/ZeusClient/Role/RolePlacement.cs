using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Role
{
    public class RolePlacement : CSUtility.Component.TrackerPlacement
    {
        public RolePlacement(RoleActor host)
            :base(host)
        {

        }

        public bool mIsUseBornRotate = false;
        public bool mIsRotateInit = false;

        static CSUtility.Performance.PerfCounter mRolePlacementChangedTimer = new CSUtility.Performance.PerfCounter("RolePlacement.SetLocation");
        public override bool SetLocation(ref SlimDX.Vector3 loc)
        {
            mRolePlacementChangedTimer.Begin();
            var role = mHostActor as RoleActor;
            if (role == null)
                return false;
            if (role.RoleTemplate != null)
            {
                loc += role.RoleTemplate.PosOffset;
            }
            role?.ProcessEnterTrigger(loc);

            if (role.RoleTemplate.FixedHeight)
            {
                if (role.RoleData.RoleType == EClientRoleType.Monster)
                {
                    loc.Y = role.RoleData.MonsterData.OriPosition.Y;
                }                
            }

            mRolePlacementChangedTimer.End();
//             if (role.RoleData.RoleType == EClientRoleType.Monster && role.RoleTemplate.MonsterType <= GameData.Role.MonsterType.Building)
//             {
//                 if (role.CurrentState.StateName == "Death")
//                 {
//                     CCore.Navigation.Navigation.Instance.NavigationData.SetDynamicNavData(GameData.Support.ConfigFile.Instance.DefaultMapId, role.mBlockPos.X, role.mBlockPos.Z, role.RoleTemplate.Radius, false);
//                     return true;
//                 }
// 
//                 if (loc != role.mBlockPos)
//                 {
//                     CCore.Navigation.Navigation.Instance.NavigationData.SetDynamicNavData(GameData.Support.ConfigFile.Instance.DefaultMapId, role.mBlockPos.X, role.mBlockPos.Z, role.RoleTemplate.Radius, false);
//                     CCore.Navigation.Navigation.Instance.NavigationData.SetDynamicNavData(GameData.Support.ConfigFile.Instance.DefaultMapId, loc.X, loc.Z, role.RoleTemplate.Radius, true);
//                     role.mBlockPos = loc;
//                 }
//             }

            return base.SetLocation(ref loc);
        }


        int mSlerpDuration = 100;
        int mSlerpTime = 0;
        SlimDX.Quaternion mTargetQuanternion;
        SlimDX.Quaternion mPreQuanternion;
        public override bool SetRotation(ref SlimDX.Quaternion quat, bool bImm)
        {
            var role = mHostActor as RoleActor;
            if (mTargetQuanternion == quat)
                return true;
            if (mIsUseBornRotate == true)
            {
                if (mIsRotateInit == false)
                    base.SetRotation(ref quat, bImm);
                mIsRotateInit = true;
                return true;
            }

            mTargetQuanternion = quat;
            if (System.Math.Abs(mTargetQuanternion.Angle - mQuanternion.Angle) > 3.14F * 5F / 180.0F)
                bImm = false;

            mSlerpTime = 0;
            mPreQuanternion = mQuanternion;

            if (role != null && role.RoleTemplate != null)
            {
                if (role.RoleTemplate.SlerpRotation == false)
                {
                    mSlerpTime = mSlerpDuration;

                    mQuanternion = mTargetQuanternion;
                    LSQ2Matrix();
                    
                    return true;
                }
            }

            if (bImm)
            {
                mSlerpTime = mSlerpDuration;
            }

            return true;
        }

        public override bool SetRotationY(float z, float x, float fixAngle, bool bImm)
        {
            if (mIsUseBornRotate == true)
            {
                if (mIsRotateInit == false)
                    base.SetRotationY(z, x, fixAngle, bImm);
                mIsRotateInit = true;
                return true;
            }
            return base.SetRotationY(z, x, fixAngle, bImm);
        }

        public bool SetRotationYbImm(float z, float x, float fixAngle)
        {
            SlimDX.Vector2 dir = new SlimDX.Vector2(x, z);
            dir.Normalize();
            var mRotationYAngle = -(float)System.Math.Atan2(dir.Y, dir.X);//这是一个右手法则函数所以要取反
            OnUpdateRoationY(mRotationYAngle);
            var quat = SlimDX.Quaternion.RotationYawPitchRoll((float)mRotationYAngle + fixAngle, 0, 0);
            mTargetQuanternion = quat;
            mQuanternion = mTargetQuanternion;
            LSQ2Matrix();
            _OnPlacementRotationChanged(ref quat);

            return true;
        }

        static CSUtility.Performance.PerfCounter UpdateMatrixTimer = new CSUtility.Performance.PerfCounter("Placement.UpdateMatrix");
        public override void Tick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            base.Tick(host, elapsedMillisecond);

            if (mSlerpTime < mSlerpDuration)
            {
                mSlerpTime += (int)elapsedMillisecond;
                float t = (float)mSlerpTime / (float)mSlerpDuration;
                mQuanternion = SlimDX.Quaternion.Slerp(mPreQuanternion, mTargetQuanternion, t);
            }
            else
            {
                mQuanternion = mTargetQuanternion;
            }

            UpdateMatrixTimer.Begin();
            LSQ2Matrix();
            UpdateMatrixTimer.End();
        }

    }

    public class ChiefPlayerPlacement : RolePlacement
    {
        public CSUtility.Net.TcpClient mGateConnect;
        public ChiefPlayerPlacement(RoleActor host)
                    : base(host)
        {

        }
        public override bool SetLocation(ref SlimDX.Vector3 loc)
        {            
            base.SetLocation(ref loc);

            OnUpdatePosition(ref mLocation);
            return true;
        }

        public override bool SetRotationY(float z, float x, float fixAngle, bool bImm)
        {

            return base.SetRotationY(z, x, fixAngle, bImm);
        }

        public override bool SetRotation(ref SlimDX.Quaternion quat, bool bImm)
        {
            base.SetRotation(ref quat, bImm);
            return true;
        }

        public override bool Move(ref SlimDX.Vector3 dir, float delta)
        {
            bool ret = base.Move(ref dir, delta);

            return ret;
        }

        static CSUtility.Performance.PerfCounter mPlacementChangedTimer = new CSUtility.Performance.PerfCounter("ChiefPlacement.UpdatePosition");
        SlimDX.Vector3 mPrevPos;
        private void OnUpdatePosition(ref SlimDX.Vector3 loc)
        {
            mPlacementChangedTimer.Begin();
            float dist = SlimDX.Vector3.Distance(mPrevPos, loc);
            if (dist < 0.25f)
            {
                mPlacementChangedTimer.End();
                return;
            }   

            Int64 nowTime = CCore.Engine.Instance._GetTickCount();
            mPrevPos = loc;

            if (mGateConnect == null)
                mGateConnect = CCore.Engine.Instance.Client.GateSvrConnect;
            //告诉服务器我的最新位置            
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_UpdatePosition(pkg, loc, Direction);
            pkg.DoClient2PlanesPlayer(mGateConnect);
            mPlacementChangedTimer.End();

        }
    }
}
