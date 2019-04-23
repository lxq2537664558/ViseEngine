using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class HallManager
    {
        Dictionary<Guid, HallInstance> mAllHallsInstance = new Dictionary<Guid, HallInstance>();
        public Dictionary<Guid, HallInstance> AllHallsInstance
        {
            get { return mAllHallsInstance; }
        }

        public HallInstance GetHallsInstance(GameData.HallsData hallsData)
        {
            HallInstance halls;
            if (AllHallsInstance.TryGetValue(hallsData.HallsId, out halls))
                return halls;
            halls = new HallInstance(hallsData);
            AllHallsInstance.Add(hallsData.HallsId, halls);
            return halls;
        }

        public HallInstance FindHallsInstance(Guid hallsId)
        {
            HallInstance halls;
            if (AllHallsInstance.TryGetValue(hallsId, out halls))
                return halls;
            return null;
        }   

        public Role.Player.PlayerInstance FindPlayer(Guid roleId)
        {
            foreach (var i in AllHallsInstance)
            {
                foreach (var p in i.Value.AllPlayers)
                {
                    if (p.Key == roleId)
                    {
                        return p.Value;
                    }
                }
            }

            return null;
        }
    }
}
