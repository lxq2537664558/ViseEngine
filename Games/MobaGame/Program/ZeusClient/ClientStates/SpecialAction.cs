using System;
using System.Collections.Generic;
using System.Text;

namespace Client.GameState
{
    [CSUtility.AISystem.Attribute.StatementClass("特殊状态", CSUtility.Helper.enCSType.Client)]
    public class SpecialAction : GameData.AI.States.SpecialAction
    {
        public override void OnExitState()
        {
            base.OnExitState();
        }
        public override bool OnActionFinished()
        {
            if (!base.OnActionFinished())
            {
                return false;
            }
            //这里写动作播放完的客户端逻辑
            return true;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
        
        }
           
        public override void Tick(Int64 elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
        }

        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
         
        }

        public Int64 mCollectTotalTime = 0;
       // long mUpdateCollectTime = 0;
        public void UpdateCollectProgress(Int64 elapsedMillisecond)
        {
            if (mCollectTotalTime <= 0)
                return;

         
        }
    }
}
