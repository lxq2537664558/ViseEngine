using System;

namespace SCore.Unit
{
    public class PlayerDBUnit
    {
        public DB.PlayerSaverThread SaverThread;

        public virtual Guid Id
        {
            get;
        }        
    }
}
