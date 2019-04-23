using Server.Hall.Role.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class PlayerInstanceManager
    {
        static PlayerInstanceManager smInstance = new PlayerInstanceManager();
        public static PlayerInstanceManager Instance
        {
            get { return smInstance; }
        }

        Dictionary<Guid, PlayerInstance> mPlayers = new Dictionary<Guid, PlayerInstance>();
        public Dictionary<Guid, PlayerInstance> Players
        {
            get { return mPlayers; }
        }

        public PlayerInstance FindPlayerInstance(Guid roleId)
        {
            lock (this)
            {
                PlayerInstance player;
                if (false == mPlayers.TryGetValue(roleId, out player))
                    return null;
                return player;
            }
        }

        public void AddPlayerInstance(PlayerInstance player)
        {
            lock (this)
            {
                mPlayers[player.Id] = player;
            }
        }

        public void RemovePlayerInstance(PlayerInstance player)
        {
            lock (this)
            {
                mPlayers.Remove(player.Id);
            }
        }

        public PlayerInstance FindPlayerInstance(UInt16 cltHandle)
        {
            lock (this)
            {                
                foreach(var i in mPlayers)
                {
                    if (i.Value.ClientLinkId == cltHandle)
                        return i.Value;
                }
                return null;
            }
        }

        public void OnLogoutPlane(PlayerInstance player)
        {

        }

        public void RemovePlayerInstance(Guid roleId)
        {
            lock (this)
            {
                mPlayers.Remove(roleId);
            }
        }
    }
}
