using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.Map
{
    //Guid到Int32的一个映射
    public class IdAllocator<OT> where OT : Role.RoleActor
    {
        public UInt32 Start;
        public UInt32 Count;

        public IdAllocator(UInt32 s, UInt32 c)
        {
            Start = s;
            Count = c;
            CurIndex = s;
        }

        private UInt32 CurIndex;

        CSUtility.Support.ConcurentObjManager<UInt32, OT> mObjects = new CSUtility.Support.ConcurentObjManager<UInt32, OT>();
        public CSUtility.Support.ConcurentObjManager<UInt32, OT> Objects
        {
            get { return mObjects; }
        }

        public OT FindObj(UInt32 id)
        {
            return mObjects.FindObj(id);
        }

        public UInt32 MapRole(OT role)
        {
            lock(this)
            {
                if (role.SingleId != 0)
                    UnmapRole(role);

                var maxCount = Count - (UInt32)mObjects.Count;
                for (var i=0; i< maxCount; i++)
                {
                    if(null==FindObj(CurIndex))
                    {
                        role._SetSingleId(CurIndex);
                        mObjects.Add(CurIndex, role);
                        CurIndex++;
                        return role.SingleId;
                    }
                    else
                    {
                        CurIndex++;
                        if (CurIndex >= Start + Count)
                            CurIndex = Start;
                    }
                }

                return 0;
            }
        }

        public void UnmapRole(OT role)
        {
            lock(this)
            {
                mObjects.Remove(role.SingleId);
                role._SetSingleId(0);
            }
        }
    }
    public class RoleIdManager
    {
        enum EIdIndexInfo : UInt32
        {
            PlayerStart = 1000,
            PlayerEnd = PlayerStart + 3000,

            MonsterStart = PlayerEnd + 1,
            MonsterEnd = MonsterStart + 10000,

            TriggerStart = MonsterEnd + 1,
            TriggerEnd = TriggerStart + 1000,

            SummonStart = TriggerEnd + 1,
            SummonEnd = SummonStart + 1000,

            DynamicBlockStart = SummonEnd + 1,
            DynamicBlockEnd = DynamicBlockStart + 1000,
        };

        public RoleIdManager()
        {
            PlayerManager = new IdAllocator<Role.Player.PlayerInstance>((UInt32)EIdIndexInfo.PlayerStart, (UInt32)EIdIndexInfo.PlayerEnd - (UInt32)EIdIndexInfo.PlayerStart);
            MonsterManager = new IdAllocator<Role.Monster.MonsterInstance>((UInt32)EIdIndexInfo.MonsterStart, (UInt32)EIdIndexInfo.MonsterEnd - (UInt32)EIdIndexInfo.MonsterStart);
            TriggerManager = new IdAllocator<Role.Trigger.TriggerInstance>((UInt32)EIdIndexInfo.TriggerStart, (UInt32)EIdIndexInfo.TriggerEnd - (UInt32)EIdIndexInfo.TriggerStart);
            SummonManager = new IdAllocator<Role.Summon.SummonRole>((UInt32)EIdIndexInfo.SummonStart, (UInt32)EIdIndexInfo.SummonEnd - (UInt32)EIdIndexInfo.SummonStart);
            DynamicBlockManager = new IdAllocator<Role.DynamicBlockInstance>((UInt32)EIdIndexInfo.DynamicBlockStart, (UInt32)EIdIndexInfo.DynamicBlockEnd - (UInt32)EIdIndexInfo.DynamicBlockStart);
        }

        public IdAllocator<Role.Player.PlayerInstance> PlayerManager { get; set; } = null;
        public IdAllocator<Role.Monster.MonsterInstance> MonsterManager { get; set; } = null;
        public IdAllocator<Role.Trigger.TriggerInstance> TriggerManager { get; set; } = null;
        public IdAllocator<Role.Summon.SummonRole> SummonManager { get; set; } = null;
        public IdAllocator<Role.DynamicBlockInstance> DynamicBlockManager { get; set; } = null;

        public List<Role.Monster.MonsterInstance> mRemoveMonster = new List<Role.Monster.MonsterInstance>();
        public List<Role.Summon.SummonRole> mRemoveSummon = new List<Role.Summon.SummonRole>();

        public void Tick(Int64 elapseMillsecond)
        {
            mRemoveMonster.Clear();
            mRemoveSummon.Clear();
            PlayerManager.Objects.For_Each((UInt32 id, Role.Player.PlayerInstance player, object argObj) =>
            {
                player.Tick(elapseMillsecond);
                return CSUtility.Support.EForEachResult.FER_Continue;
            } , null);

            MonsterManager.Objects.For_Each((UInt32 id, Role.Monster.MonsterInstance monster, object argObj) =>
            {
                monster.Tick(elapseMillsecond);
                if(monster.IsLeaveMap)
                {
                    mRemoveMonster.Add(monster);
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            TriggerManager.Objects.For_Each((UInt32 id, Role.Trigger.TriggerInstance trigger, object argObj) =>
            {
                trigger.Tick(elapseMillsecond);
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            SummonManager.Objects.For_Each((UInt32 id, Role.Summon.SummonRole summon, object argObj) =>
            {
                summon.Tick(elapseMillsecond);
                if(summon.IsLeaveMap)
                {
                    mRemoveSummon.Add(summon);
                }
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            DynamicBlockManager.Objects.For_Each((UInt32 id, Role.DynamicBlockInstance dynamicBlock, object argObj) =>
            {
                dynamicBlock.Tick(elapseMillsecond);
                return CSUtility.Support.EForEachResult.FER_Continue;
            },  null);

            foreach(var role in mRemoveSummon)
            {
                ProcRemoveSummon(role);
            }

            foreach (var role in mRemoveMonster)
            {
                ProcRemoveMonster(role);
            }
        }

        private void ProcRemoveSummon(Role.Summon.SummonRole role)
        {
            role.HostMap.RemoveRoleActor(role);
            SummonManager.UnmapRole(role);                       
        }

        private void ProcRemoveMonster(Role.Monster.MonsterInstance role)
        {
            role.HostMap.RemoveRoleActor(role);
            MonsterManager.UnmapRole(role);                       
        }
    }
}
