using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.InitFactory
{
    public class RoleTemplateInitFactory : CSUtility.Data.RoleTemplateInitFactory
    {
        public override CSUtility.Data.RoleTemplateBase CreateRoleTemplate()
        {            
            return new Role.RoleTemplate();                         
        }
    }

    public class NPCDataInitFactory : CSUtility.Map.Role.NPCDataInitFactory
    {
        public override CSUtility.Data.RoleDataBase CreateNPCData()
        {
            return new Role.MonsterData();
        }
    }
}
