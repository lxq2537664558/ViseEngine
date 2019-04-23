using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Role
{
    [Flags]
    public enum ERoleType : UInt32
    {
        Player = 1,
        Monster = (1 << 1),
        Item = (1 << 2),
        Npc = (1 << 3),
        Summon = (1 << 4),
        Gather = (1 << 5),
        Trigger = (1 << 6),
        DynamicBlock = (1<<7),
    }
}
