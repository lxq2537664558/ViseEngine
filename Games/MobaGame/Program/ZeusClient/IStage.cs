using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public interface IStage
    {
        void Enter(Game game);
        void Leave(Game game);
        void Tick(Game game);
        void RenderThreadTick(Game game);

        CCore.MsgProc.FBehaviorProcess FindBehavior(CCore.MsgProc.BehaviorParameter bhInit);
    }
}
