using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.ServerStates
{
    [CSUtility.AISystem.Attribute.StatementClass("待机状态", CSUtility.Helper.enCSType.Server)]
    public class Idle : GameData.AI.States.Idle
    {
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
            var role = Host as Role.RoleActor;
            if (role != null)
            {
            }
        }
        public override void OnEnterState()
        {
            base.OnEnterState();
            var role = Host as Role.RoleActor;
            if (role != null)
            {
                role.OnEnterState("Idle");
            }
        }

        public override bool OnActionFinished()
        {
            base.OnActionFinished();
            return true;
        }

        public override bool DoFireSkill(UInt16 skillId, SlimDX.Vector3 dir, SlimDX.Vector3 summonPos, UInt16 runeLevel = 0)
        {
//             var skill = CSCommon.Data.Skill.SkillTemplateManager.Instance.FindSkill(skillId);
//             if (skill == null)
//                 return false;
// 
//             var role = Host as ServerCommon.Planes.Role.RoleActor;
//             var skillData = role.FindSkillData(skillId);
//             if (skillData == null)
//                 return false;
// //             if (skillData.SkillLevel <= 0)
// //                 return false;
// 
//             //SlimDX.Vector3 dir = targetPos - role.Placement.GetLocation();
//             //dir.Y = 0;
//             //dir.Normalize();
// 
            base.DoFireSkill(skillId, dir, summonPos,runeLevel);
            return true;
        }
    }
}
