using System;
using System.Collections.Generic;
using System.Text;
using GameData.Skill;

namespace Client.Buff
{
    public class BuffBag
    {
        public BuffBag(Role.RoleActor role)
        {
            mRole = role;
        }
        public Role.RoleActor mRole = null;
        List<Buff> mBuffs = new List<Buff>();

        public List<Buff> Buffs
        {
            get { return mBuffs; }
        }

        public Buff FindBuff(Guid id)
        {
            foreach (Buff i in mBuffs)
            {
                if (i.BuffData.BuffId == id)
                {
                    return i;
                }
            }
            return null;
        }

        public Buff GetBuff(ushort id)
        {
            foreach (Buff i in mBuffs)
            {
                if (i.BuffData.Template.Id == id)
                {
                    return i;
                }
            }
            return null;
        }

        private Buff DangerousCreateBuffEffect(BuffData data)//创建buff和特效
        {
            var bi = DangerCreateBuff(data);
            if (bi == null)
                return null;
            mRole.AddEffect(data.Template.BuffNotifyEffects,"");
            return bi;
        }

        private Buff DangerCreateBuff(BuffData data)
        {
            Buff bi = new Buff(data, mRole);
            mBuffs.Add(bi);
            return bi;
        }

        public Buff CreateAddBuff(BuffData data)//服务器通知客户端加buff
        {
            var repla = CheckRePlaceBuff(data);
            if (repla != null)
                return repla;
            var res = DangerousCreateBuffEffect(data);
            //mRole.AddBuffUI(buffid);
            return res;
        }

        Buff CheckRePlaceBuff(BuffData data)//替换规则
        {
            var bufftpl = data.Template;//BuffTemplateManager.Instance.FindBuff(data.BuffTemlateId);
            var level = data.BuffLevel;
            switch (bufftpl.BuffReplaceType)
            {
                case ReplaceType.SameTypeChange://同类型替换
                    {
                        foreach (var buff in mBuffs)
                        {
                            if (buff.BuffData.Template.BuffType == bufftpl.BuffType)
                            {
                                return RePlaceBuff(data);
                            }
                        }
                        break;
                    }
            }
            var abuff = GetBuff((ushort)data.BuffTemlateId);
            if (abuff == null)
                return null;
            switch (bufftpl.BuffReplaceType)
            {
                case ReplaceType.HighLvChangeLowLv://高等级替换低等级
                    {
                        if (abuff.BuffData.BuffLevel <= level)
                        {
                            return RePlaceBuff(data);
                        }
                        break;
                    }
                case ReplaceType.LowLvChangeHighLv://低等级替换高等级
                    {
                        if (abuff.BuffData.BuffLevel > level)
                        {
                            return RePlaceBuff(data);
                        }
                        break;
                    }
                case ReplaceType.SameLvChange://同等级相互替换
                    {
                        if (abuff.BuffData.BuffLevel == level)
                        {
                            return RePlaceBuff(data);
                        }
                        break;
                    }
                    //case CSCommon.Data.Skill.ReplaceType.SameLvStack://同等级叠加
                    //    {
                    //        if (abuff.BuffData.BuffLevel == buffparam.BuffLevel)
                    //        {
                    //            if (abuff.BuffData.BuffLayer < abuff.BuffTemplate.BuffStackValue)
                    //            {
                    //                SumBuff(owner, caster, levelparam.BuffShelfLife, abuff.Position);
                    //                return true;
                    //            }
                    //        }
                    //        break;
                    //    }
            }
            return null;
        }

        Buff RePlaceBuff(BuffData data)
        {
            RemoveBuff(data.BuffId);
            return DangerousCreateBuffEffect(data);
        }

        public void FreshBuffLiveTime(Guid buffid, long livetime)//处理buff叠加
        {
            for (int pos = 0; pos < mBuffs.Count; ++pos)
            {
                if (mBuffs.Count > 0 && mBuffs.Count - 1 >= pos && mBuffs[pos].BuffData.BuffId == buffid)
                {
                    switch (mBuffs[pos].BuffData.Template.BuffLiveTimeType)
                    {
                        case TimeType.ChangeTime:
                            {
                                mBuffs[pos].LiveTime = livetime;
                                mBuffs[pos].BuffData.StackNum++;
                            }
                            break;
                        case TimeType.DontChangeTime:
                            {
                                mBuffs[pos].BuffData.StackNum++;
                                break;
                            }
                        case TimeType.SumTime:
                            {
                                mBuffs[pos].LiveTime = livetime;
                                mBuffs[pos].BuffData.StackNum++;
                                break;
                            }
                    }
                    return;
                }
            }
        }

        //         public void AddBuff(BuffTemplate buff)
        //         {
        //             foreach (Buff i in mBuffs)
        //             {
        //                 if (i.Template.BuffId == buff.BuffId)
        //                 {
        //                     if (i.Template.MaxStackCount == 1)
        //                     {
        //                         i.AddTime = MidLayer.IEngine.Instance.GetFrameSecondTimeFloat();
        //                         i.Template.Raise_OnBuffAdded(mRole, i);
        //                     }
        //                     else if (i.StackCount + 1 <= i.Template.MaxStackCount)
        //                     {
        //                         i.AddTime = MidLayer.IEngine.Instance.GetFrameSecondTimeFloat();
        //                         i.StackCount++;
        //                     }
        //                     else
        //                     {//超过堆叠上限继续堆叠，只刷新时间
        //                         i.AddTime = MidLayer.IEngine.Instance.GetFrameSecondTimeFloat();
        //                     }
        //                     return;
        //                 }
        //             }
        // //             //找不到已经存在的同类型buff，那么就有互斥问题了//客户端不做这类的逻辑
        // //             if (buff.MutexBuffs != null)
        // //             {
        // //                 foreach (Guid id in buff.MutexBuffs)
        // //                 {
        // //                     BuffTemplate mutexBuff = BuffTemplateMgr.Instance.GetBuff(id, false);
        // //                     if (mutexBuff == null)
        // //                         continue;
        // //                     RemoveBuff(id, mutexBuff.MaxStackCount);
        // //                     //RemoveBuff(id, BuffTemplateMgr.Instance.GetBuff(id).32768);这样写可以减少一次Dictionary的查找，理论上也没问题
        // //                 }
        // //             }
        //             Buff bi = new Buff(buff,1,mRole);
        //             bi.AddTime = MidLayer.IEngine.Instance.GetFrameSecondTimeFloat();
        //             mBuffs.Add(bi);
        //             bi.Template.Raise_OnBuffAdded(mRole, bi);
        //             return;
        //         }

        public void RemoveBuff(Guid buff)
        {
            if (mBuffs.Count == 0)
                return;
            for (UInt16 i = 0; i < mBuffs.Count; i++)
            {
                if (mBuffs[i].BuffData.BuffId == buff)
                {
                    RemoveThis(mBuffs[i]);
                    i--;
                }
            }
        }

        void RemoveThis(Buff buff)
        {
            mBuffs.Remove(buff);
            mRole.RemoveEffects(buff.BuffData.Template.BuffNotifyEffects,"");
        }

        public void Tick(long elapsedMillisecond)
        {
            for (int i = 0; i < mBuffs.Count; ++i)
            {
                Buff cur = mBuffs[i];
                if (cur == null || cur.BuffData.Template.ImmortalLiving)//不计算时间
                    continue;
                if (cur.Tick(elapsedMillisecond) == false)
                {
                    RemoveThis(cur);
                    i--;
                }
            }
        }

    }

    public class Buff
    {
        public Buff(BuffData t, Role.RoleActor role)
        {
            BuffData = t;
            OwnerRole = role;
            InitBuff();
        }
        void InitBuff()
        {
            if (BuffData == null)
                return;
            var leveltemp = BuffData.GetLevelTemplate();
            if (leveltemp == null)
                return;
            LevelTemp = leveltemp;
            mLiveTime = leveltemp.BuffShelfLife;
            CreateTime = CSUtility.Helper.LogicTimer.GetTickCount();
        }

        BuffData mBuffData = null;
        public BuffData BuffData
        {
            get { return mBuffData; }
            set { mBuffData = value; }
        }

        long mLiveTime = 0;//一直在变
        public long LiveTime
        {
            get { return mLiveTime; }
            set { mLiveTime = value; }
        }
        long mCreateTime = 0;//创建时间
        public long CreateTime
        {
            get { return mCreateTime; }
            set { mCreateTime = value; }
        }

        BuffLevelTemplate mLevelTemp = null;
        public BuffLevelTemplate LevelTemp
        {
            get { return mLevelTemp; }
            set { mLevelTemp = value; }
        }

        Role.RoleActor mOwnerRole = null;
        public Role.RoleActor OwnerRole
        {
            get { return mOwnerRole; }
            set { mOwnerRole = value; }
        }

        float mAddTime;
        public float AddTime
        {
            get { return mAddTime; }
            set { mAddTime = value; }
        }

        bool mEnable;
        public bool Enable
        {
            get { return mEnable; }
            set { mEnable = value; }
        }

        public bool Tick(long elapsedMillisecond)
        {
            var time = CSUtility.Helper.LogicTimer.GetTickCount();
            LiveTime = BuffData.LiveTime - (time - CreateTime);
            if (LiveTime <= 0)
            {
                return false;
            }
            return true;
        }

        public void FreshCreateTime(Int64 livetime)
        {
            var time =  CSUtility.Helper.LogicTimer.GetTickCount();
            CreateTime = time - (BuffData.LiveTime - livetime);
        }
    }
}
