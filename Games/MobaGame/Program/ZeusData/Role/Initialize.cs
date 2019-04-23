using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Role
{
    public class Initialize
    {
        public static void InitializeData()
        {
            CSUtility.Data.RoleTemplateManager.Instance.ActionNameList.AddRange(new string[] {  "Idle",
                                                                                                "FightIdle",
                                                                                                "Walk",
                                                                                                "Run",
                                                                                                "Death",
                                                                                                "StayAttack",
                                                                                                "BeAttack",});
        }
    }
}
