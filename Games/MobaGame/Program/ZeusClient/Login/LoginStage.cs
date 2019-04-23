using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Login
{
    public class LoginStage : UISystem.UIBindAutoUpdate , IStage
    {
        public static LoginStage Instance
        {
            get
            {
                return CCore.Support.ReflectionManager.Instance.GetClassObject<LoginStage>();
            }
        }

        ~LoginStage()
        {
        }

        public void Enter(Game game)
        {

        }

        public void Leave(Game game)
        {

        }

        public void Tick(Game game)
        {

        }

        public void RenderThreadTick(Game game)
        {

        }

        public CCore.MsgProc.FBehaviorProcess FindBehavior(CCore.MsgProc.BehaviorParameter bhInit)
        {
            return null;
        }
    }
}
