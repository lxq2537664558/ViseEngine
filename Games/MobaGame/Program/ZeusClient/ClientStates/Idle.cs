using System;
using System.Collections.Generic;

using System.Text;

namespace Client.GameState
{
    [CSUtility.AISystem.Attribute.StatementClass("待机状态", CSUtility.Helper.enCSType.Client)]
    public class Idle : GameData.AI.States.Idle
    {
        //Int64 mNpcInIdleTime = 0;

        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
            CanInterrupt = true;
        }

        public override void SetStateAction()
        {
            Host.FSMSetAction("Idle", true, 1.0f, 100);
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            var role = Host as Role.RoleActor;
            if(role !=null && role.IsChielfPlayer() ==true)
                Role.ChiefRoleActorController.Instance.FireSkillReady = true;
        }

        public override void OnExitState()
        {
            base.OnExitState();

        }

        public override void OnReEnterState()
        {
            base.OnReEnterState();
        }

       Int64 mUpdateNpcPosTime = 3000;
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

            var role = Host as Role.RoleActor;
            if (role.RoleData.RoleType == Role.EClientRoleType.Monster)
            {
                mUpdateNpcPosTime -= elapsedMillisecond;
                if (mUpdateNpcPosTime <= 0)
                {
                    mUpdateNpcPosTime = 3000;

                }
            }                        
        }

        public override bool OnActionFinished()
        {
            if (!base.OnActionFinished())
            {
                return false;
            }
            return true;
        }
    }
}