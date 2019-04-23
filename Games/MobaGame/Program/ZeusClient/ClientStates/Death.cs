using System;
using System.Collections.Generic;

using System.Text;

namespace Client.GameState
{
    [CSUtility.AISystem.Attribute.StatementClass("死亡状态", CSUtility.Helper.enCSType.Client)]
    public class Death : GameData.AI.States.Death
    {
        public override void OnEnterState()
        {
            base.OnEnterState();
       //     mSumActionFinish = false;
            var role = Host as Client.Role.RoleActor;
            if (role !=null)
            {
            }
        }

    //    Int64 mSummonDeathDelay = 0;
     //   bool mSumActionFinish = false;
        public override void ProcDeath()
        {
            var role = Host as Client.Role.RoleActor;
            if (role == null)
                return;

            role.ProcDeath();

            if (!role.IsChielfPlayer())
            {
                role.DoLeaveMap();
            }
        }
        public override void OnExitState()
        {
            base.OnExitState();
        }

        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
        }
    }
}
