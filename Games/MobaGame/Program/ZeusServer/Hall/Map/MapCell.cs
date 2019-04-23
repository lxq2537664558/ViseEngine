using System;
using System.Collections.Generic;
using System.Text;
using CSUtility.Support;

namespace Server.Hall.Map
{
    public class MapCell
    {
        public ConcurentObjManager<UInt32, Role.RoleActor> Players = new ConcurentObjManager<UInt32, Role.RoleActor>();
        public ConcurentObjManager<UInt32, Role.RoleActor> Monsters = new ConcurentObjManager<UInt32, Role.RoleActor>();
        public ConcurentObjManager<UInt32, Role.RoleActor> Items = new ConcurentObjManager<UInt32, Role.RoleActor>();
        public ConcurentObjManager<UInt32, Role.RoleActor> Npcs = new ConcurentObjManager<UInt32, Role.RoleActor>();
        public ConcurentObjManager<UInt32, Role.RoleActor> Summons = new ConcurentObjManager<UInt32, Role.RoleActor>();
        public ConcurentObjManager<UInt32, Role.RoleActor> Gathers = new ConcurentObjManager<UInt32, Role.RoleActor>();
        public ConcurentObjManager<UInt32, Role.RoleActor> Triggers = new ConcurentObjManager<uint, Role.RoleActor>();

        int m_x;
        public int X
        {
            get { return m_x; }
        }
        int m_y;
        public int Y
        {
            get { return m_y; }
        }

        public MapCell(int x, int y)
        {
            m_x = x;
            m_y = y;
        }

        public void Enter(Role.RoleActor role)
        {
            if (role.Cell == this)
                return;
            if(role.Cell !=null)
                role.Cell.Leave(role);

            if (role.RoleCreateType ==GameData.Role.ERoleType.Player)
            {
                Players.Add(role.SingleId, (Role.Player.PlayerInstance)role);
            }
            if (role.RoleCreateType == GameData.Role.ERoleType.Monster)
            {
                Monsters.Add(role.SingleId, (Role.Monster.MonsterInstance)role);
            }
            if (role.RoleCreateType == GameData.Role.ERoleType.Summon)
            {
                Summons.Add(role.SingleId, (Role.Summon.SummonRole)role);
            }
            if (role.RoleCreateType == GameData.Role.ERoleType.Trigger)
            {
                Triggers.Add(role.SingleId, (Role.Trigger.TriggerInstance)role);
            }
            role.Cell = this;
        }

        public void Leave(Role.RoleActor role)
        {
            if (role.RoleCreateType == GameData.Role.ERoleType.Player)
            {
                Players.Remove(role.SingleId);
            }
            if (role.RoleCreateType == GameData.Role.ERoleType.Monster)
            {
                Monsters.Remove(role.SingleId);
            }
            if (role.RoleCreateType == GameData.Role.ERoleType.Summon)
            {
                Summons.Remove(role.SingleId);
            }
            if (role.RoleCreateType == GameData.Role.ERoleType.Trigger)
            {
                Triggers.Remove(role.SingleId);
            }
            role.Cell = null;
        }
        public bool ForEachRole(UInt32 types, CSUtility.Support.ConcurentObjManager<UInt32, Hall.Role.RoleActor>.FOnVisitObject visitor, object arg)
        {
            if ((types & (UInt32)GameData.Role.ERoleType.Player) != 0)
            {
                if (false == Players.For_Each(visitor, arg))
                    return false;
            }
            if ((types & (UInt32)GameData.Role.ERoleType.Monster) != 0)
            {
                if (false == Monsters.For_Each(visitor, arg))
                    return false;
            }
            if ((types & (UInt32)GameData.Role.ERoleType.Item) != 0)
            {
                if (false == Items.For_Each(visitor, arg))
                    return false;
            }
            if ((types & (UInt32)GameData.Role.ERoleType.Npc) != 0)
            {
                if (false == Npcs.For_Each(visitor, arg))
                    return false;
            }
            if ((types & (UInt32)GameData.Role.ERoleType.Summon) != 0)
            {
                if (false == Summons.For_Each(visitor, arg))
                    return false;
            }
            if ((types & (UInt32)GameData.Role.ERoleType.Gather) != 0)
            {
                if (false == Gathers.For_Each(visitor, arg))
                    return false;
            }
            if ((types & (UInt32)GameData.Role.ERoleType.Trigger) != 0)
            {
                if (false == Triggers.For_Each(visitor, arg))
                    return false;
            }
            return true;
        }
    }
}
