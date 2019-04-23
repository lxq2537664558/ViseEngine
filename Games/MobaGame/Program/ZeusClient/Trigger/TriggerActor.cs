using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Trigger
{
    public class TriggerProcessData_Client : CSUtility.Map.Trigger.TriggerProcessData
    {
        public TriggerProcessData_Client(Guid id, Role.RoleActor role)
        {
            Id = id;
            Actor = role;
        }
        public override Guid Id
        {
            get;
            set;
        }
    }    
}
