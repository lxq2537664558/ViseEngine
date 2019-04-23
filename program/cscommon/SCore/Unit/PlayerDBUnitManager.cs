namespace SCore.Unit
{
    public class PlayerDBUnitManager
    {
        public virtual bool OnAccountEnter(DB.AccountLoginHolder holder)
        {
            return true;
        }

        public virtual bool OnRoleEnter(DB.RoleEnterHolder holder)
        {
            return true;
        }

        
    }
}
