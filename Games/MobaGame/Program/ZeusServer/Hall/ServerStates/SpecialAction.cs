using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.ServerStates
{
    [CSUtility.AISystem.Attribute.StatementClass("特殊状态", CSUtility.Helper.enCSType.Server)]
    public class SpecialAction : GameData.AI.States.SpecialAction
    {
        public override bool OnActionFinished()
        {
            var player = Host as Role.Player.PlayerInstance;
            if (!base.OnActionFinished())
            {
                return false;
            }
            if (mStateName == "JumpMapChannel")
            {
                if (player != null || SpecialActionParameter.JumpMapTransId != UInt16.MaxValue)
                {
                //    player.JumpMap2Transfar(SpecialActionParameter.JumpMapTransId);
                }
                TargetToState(player, "Idle", null);
            }
            TargetToState(player, "Idle", null);
            return true;
        }
    }
}
