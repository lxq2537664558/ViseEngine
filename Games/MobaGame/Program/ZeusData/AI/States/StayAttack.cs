using System;
using System.Collections.Generic;
using System.Text;
using CSUtility;

namespace GameData.AI.States
{
    public class StayAttack :CSUtility.AISystem.States.StayAttack
    {
        public GameData.Skill.SkillData Skill = null;

        public override void OnPreEnterState()
        {
            base.OnPreEnterState();

        }
    }
}
