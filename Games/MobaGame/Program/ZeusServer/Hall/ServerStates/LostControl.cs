using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.ServerStates
{
    [CSUtility.AISystem.Attribute.StatementClass("失控状态", CSUtility.Helper.enCSType.Server)]
    public class LostControl : GameData.AI.States.LostControl
    {
        Int64 mLiveTime;
        public override void OnEnterState()
        {
            base.OnEnterState();

            mLiveTime = 5000;
        }

        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

            mLiveTime -= elapsedMillisecond;

            if (mLiveTime < 0)
            {
                TargetToState(Host, "Idle", null);
            }
        }
    }
}
