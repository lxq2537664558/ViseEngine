using System;
using System.Collections.Generic;

using System.Text;

namespace Client.GameState
{
    [CSUtility.AISystem.Attribute.StatementClass("失控状态", CSUtility.Helper.enCSType.Client)]
    public class LostControl : GameData.AI.States.LostControl
    {
        public override void OnEnterState()
        {
            base.OnEnterState();

            FSMSetAction("Swim", true, 1.0f);
        }

        public override void OnExitState()
        {
            base.OnExitState();
        }
    }
}
