using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.ServerStates
{
    [CSUtility.AISystem.Attribute.StatementClass("吟唱状态", CSUtility.Helper.enCSType.Server)]
    public class StayChannel : GameData.AI.States.StayChannel
    {
        public override void OnEnterState()
        {
            base.OnEnterState();

            //var role = Host as ServerCommon.Planes.Role.RoleActor;
            //RPC.PackageWriter pkg = new RPC.PackageWriter();
            //ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, role.SingleId).RPC_UpdateDirection(pkg, role.Placement.GetRotationY());
            //role.HostMap.SendPkg2Clients(role, role.Placement.GetLocation(), pkg);
        }
    }
}
