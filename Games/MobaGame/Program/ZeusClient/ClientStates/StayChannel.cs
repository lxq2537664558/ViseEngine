using System;
using System.Collections.Generic;

using System.Text;

namespace Client.GameState
{
    [CSUtility.AISystem.Attribute.StatementClass("吟唱状态", CSUtility.Helper.enCSType.Client)]
    public class StayChannel : GameData.AI.States.StayChannel
    {
        public override void OnEnterState()
        {
            base.OnEnterState();

            var role = Host as Client.Role.RoleActor;
            if (role != null && role.IsChielfPlayer() == true)
            {
           
                var dir = StayChannelParameter.tarPos -role.Placement.GetLocation();
                dir.Normalize();
                role.Placement.SetRotationY(dir.Z, dir.X, role.RoleTemplate.MeshFixAngle);

                float rotY = role.Placement.GetRotationY();

//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
//                 ServerCommon.Planes.H_PlayerInstance.smInstance.RPC_UpdateDirection(pkg, role.SingleId , rotY);
//                 pkg.DoClient2PlanesPlayer(MidLayer.IEngine.Instance.Client.GateSvrConnect);
                
                //dir.Y = 0;

                //dir.Normalize();
                //float fAngle = -(float)System.Math.Atan2(dir.Z, dir.X);//这是一个右手法则
                //if (fAngle < 0)
                //    fAngle += (float)(2 * System.Math.PI);
                //double angle = fAngle * (180 / System.Math.PI);
                //System.Diagnostics.Debug.WriteLine(string.Format("y:{0},x:{1},angle:{2}", dir.Z, dir.X, angle));
                //fAngle += (float)(System.Math.PI * 1.5);
                
                ////var quat = SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitY, fAngle);
                //var quat = SlimDX.Quaternion.RotationYawPitchRoll((float)fAngle, 0, 0);
                //role.Placement.SetRotation(ref quat);

            }
        }
    }
}
