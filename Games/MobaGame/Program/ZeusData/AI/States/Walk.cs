using System;
using System.Collections.Generic;
using System.Text;
using CSUtility;
using SlimDX;

namespace GameData.AI.States
{
    public class Walk : CSUtility.AISystem.States.Walk
    {
        public Queue<CSUtility.Helper.EventCallBack> OnTargetPosArrivedQueue = new Queue<CSUtility.Helper.EventCallBack>();

        public override void OnExitState()
        {
            base.OnExitState();
        }

        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
        }

        public override void OnArrived()
        {
            if (OnTargetPosArrivedQueue.Count != 0)
            {
                CSUtility.Helper.EventCallBack cb = OnTargetPosArrivedQueue.Dequeue();
                if (cb != null)
                {
                    var callee = cb.GetCallee() as CSUtility.AISystem.FOnStateExit;
                    if (callee != null)
                    {
                        callee(this, Host.CurrentState);  //调用委托
                        return;
                    } 
                }
            }
            base.OnArrived();
        }
    }
}
