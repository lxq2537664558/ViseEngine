using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.ServerStates
{
    [CSUtility.AISystem.Attribute.StatementClass("死亡状态", CSUtility.Helper.enCSType.Server)]
    public class Death : GameData.AI.States.Death
    {
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
         
        }
        public override void OnEnterState()
        {
            base.OnEnterState();   
        }

        public override void ProcDeath()
        {
            var role = Host as Role.RoleActor;
            if (role != null)
            {                               
                var param = Parameter as CSUtility.AISystem.States.IDeathParameter;
                if(param !=null)
                {
                    var killer = role.HostMap.FindPlayer(param.KillerId);
                    if(killer !=null)
                    {
                        killer.OnKillerOther(role);
                    }
                }
                role.ProcDeath();
            } 
        }

        public override bool OnActionFinished()
        {
            return base.OnActionFinished();          
        }

        Int64 mUpdateDeathTimer = 0;
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
            mUpdateDeathTimer += elapsedMillisecond;
            var role = Host as Role.RoleActor;
            if (role !=null)
            {
                if (mUpdateDeathTimer >role.DeathDelegateTimer)
                {
               //     role.DoDelegateDeathTimer();
                    mUpdateDeathTimer = 0;
                }
            }
        }
    }
}
