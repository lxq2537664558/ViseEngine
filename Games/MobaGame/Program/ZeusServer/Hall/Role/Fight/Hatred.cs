using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall.Role.Fight
{
    public class HatredValue
    {
        public int Value;
        public RoleActor Attacker;
    }

    public class Hatred
    {
        Role.RoleActor mHostRole;
        public Hatred(Role.RoleActor role)
        {
            mHostRole = role;
        }

        CSUtility.Support.AsyncObjManager<Guid, HatredValue> mHatreds = new CSUtility.Support.AsyncObjManager<Guid, HatredValue>();
        public CSUtility.Support.AsyncObjManager<Guid, HatredValue> Hatreds
        {
            get { return mHatreds; }
        }

        int mRemainTickTime = 1000;

        RoleActor mFirstHatridTarget;
        RoleActor mNearHatridTarget;
        public void Tick(Int64 elapseMillisecond)
        {
            mRemainTickTime -= (int)elapseMillisecond;
            if (mRemainTickTime > 0)
            {
                return;
            }
            mRemainTickTime = 1000;

            mHatreds.BeforeTick();

            mFirstHatridTarget = null;
            mNearHatridTarget = null;
            int maxHatrid = int.MinValue;
            float nearDist = float.MaxValue;
            CSUtility.Support.AsyncObjManager<Guid, HatredValue>.FOnVisitObject visitor = delegate (Guid key, HatredValue value, object arg)
            {
                if (value.Attacker.IsLeaveMap || value.Attacker.IsDeath() || value.Attacker.HostMap != mHostRole.HostMap)
                {
                    mHatreds.Remove(key);
                    return CSUtility.Support.EForEachResult.FER_Continue;
                }

                var dist = SlimDX.Vector3.Distance(mHostRole.Placement.GetLocation(), value.Attacker.Placement.GetLocation());
                if (mHostRole.RoleCreateType != GameData.Role.ERoleType.Player)
                {
                    if (dist > 20)
                    {
                        mHatreds.Remove(key);
                    }
                    return CSUtility.Support.EForEachResult.FER_Continue;
                }

                if (dist > nearDist)
                {
                    dist = nearDist;
                    mNearHatridTarget = value.Attacker;
                }

                value.Value -= 1;
                if (maxHatrid < value.Value)
                {
                    mFirstHatridTarget = value.Attacker;
                    maxHatrid = value.Value;
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            mHatreds.For_Each(visitor, null);

            mHatreds.AfterTick();
        }

        public RoleActor GetNearTarget()
        {
            return mNearHatridTarget;
        }

        public RoleActor GetFirstTarget()
        {
            if (mFirstHatridTarget == null && mHatreds.Count > 0)
            {
                int maxHatrid = int.MinValue;
                CSUtility.Support.AsyncObjManager<Guid, HatredValue>.FOnVisitObject visitor = delegate (Guid key, HatredValue value, object arg)
                {
                    if (value.Attacker.IsLeaveMap || value.Attacker.IsDeath() || value.Attacker.HostMap != mHostRole.HostMap)
                    {
                        mHatreds.Remove(key);
                        return CSUtility.Support.EForEachResult.FER_Continue;
                    }

                    var dist = SlimDX.Vector3.Distance(mHostRole.Placement.GetLocation(), value.Attacker.Placement.GetLocation());
                    if (mHostRole.RoleCreateType != GameData.Role.ERoleType.Player)
                    {
                        if (dist > 20)
                        {
                            mHatreds.Remove(key);
                            return CSUtility.Support.EForEachResult.FER_Continue;
                        }
                    }

                    if (maxHatrid < value.Value)
                    {
                        mFirstHatridTarget = value.Attacker;
                        maxHatrid = value.Value;
                    }
                    return CSUtility.Support.EForEachResult.FER_Continue;
                };
                mHatreds.For_Each(visitor, null);
            }
            return mFirstHatridTarget;
        }

        public static int GetHatredValueByDamage(int damage, RoleActor attacker, RoleActor target, GameData.Skill.SkillData skill)
        {//郭鑫写，伤害到仇恨值的映射
            int result = damage;
            try
            {
                if (damage == 0)
                {
                    result = 1;
                }
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
            return result;
        }

        public void ClearHatred()
        {
            CSUtility.Support.AsyncObjManager<Guid, HatredValue>.FOnVisitObject visitor = delegate (Guid key, HatredValue value, object arg)
            {
                UpdateHatredSigle2Client(value.Attacker);
                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            mHatreds.For_Each(visitor, null);
            mHatreds.Clear();
        }

        public void AddHatred(RoleActor role, int value)
        {
            if (role == null)
            {
                return;
            }
            var hatred = mHatreds.FindObj(role.Id);
            if (hatred == null)
            {
                hatred = new HatredValue();
                hatred.Attacker = role;
                mHostRole.SetRoleAttackImider();
                SetAttackerSingle2Client(hatred.Attacker);
                mHatreds.Add(role.Id, hatred);
            }

            hatred.Value += value;
            if (role.OwnerRole != null && role.OwnerRole != role)//召唤物会给主人带来一半的仇恨
                AddHatred(role.OwnerRole, value / 2);
        }

        public void RemoveHatred(RoleActor role)
        {
            if (role == null)
                return;

            UpdateHatredSigle2Client(role);
            mHatreds.Remove(role.Id);
        }

        public HatredValue FindHatred(RoleActor attacker)
        {
            return mHatreds.FindObj(attacker.Id);
        }

        public void SetAttackerSingle2Client(RoleActor Attacker)
        {
            //if (Attacker != null && mHostRole.HostMap != null && mHostRole.RoleCreateType == CSCommon.Data.ERoleCreateType.Player)
            //{
            //    var player = mHostRole as PlayerInstance;
            //    if (player == null)
            //        return;

            //    RPC.PackageWriter pkg = new RPC.PackageWriter();
            //    ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, mHostRole.SingleId).RPC_UpdateRoleAttacker(pkg, Attacker.SingleId);
            //    pkg.DoCommandPlanes2Client(player.Planes2GateConnect, player.ClientLinkId);
            //}
            if (Attacker == null)
                return;
            uint singleId = Attacker.SingleId;
            var player = Attacker as Player.PlayerInstance;
            if (mHostRole.HostMap != null && player != null)
            {
                RPC.PackageWriter pkg = new RPC.PackageWriter();

//                 ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, mHostRole.SingleId).RPC_UpdateRoleAttacker(pkg, singleId);
//                 pkg.DoCommandPlanes2Client(player.Planes2GateConnect, player.ClientLinkId);
                //mHostRole.HostMap.SendPkg2Clients(mHostRole, mHostRole.Placement.GetLocation(), pkg);
            }
        }

        public void UpdateHatredSigle2Client(RoleActor role)
        {
            if (role == null)
                return;
            var player = role as Player.PlayerInstance;
            if (player != null)
            {
//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
//                 ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, mHostRole.SingleId).RPC_RemoveHatredSigleClient(pkg, role.SingleId);
//                 pkg.DoCommandPlanes2Client(player.Planes2GateConnect, player.ClientLinkId);
            }
            //             player =mHostRole as PlayerInstance;
            //             if (player !=null)
            //             {
            //                 RPC.PackageWriter pkg = new RPC.PackageWriter();
            //                 ExamplePlugins.ZeusGame.H_IGame.smInstance.HIndex(pkg, mHostRole.SingleId).RPC_RemoveHatredSigleClient(pkg, role.SingleId);
            //                 pkg.DoCommandPlanes2Client(player.Planes2GateConnect, player.ClientLinkId);
            //             }
        }

    }
}
