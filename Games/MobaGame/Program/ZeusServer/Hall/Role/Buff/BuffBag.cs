using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall.Role.Buff
{
    public class BuffBag
    {
       Buff[] mBuffs = null;
        byte mBagSize = 12;
        public byte BagSize
        {
            get { return mBagSize; }
            set { mBagSize = value; }
        }

        public BuffBag(RoleActor role)
        {
            InitBag(role);
        }

        public Buff this[UInt16 index]
        {
            get
            {
                if (index >= BagSize)
                    return null;
                return mBuffs[index];
            }
            set
            {
                if (index >= BagSize)
                    return;
                mBuffs[index] = value;
                if (value != null)
                {
                    value.Position = index;
                    value.BuffData.OwnerId = mHostRole.Id;
                }
            }
        }

        Role.RoleActor mHostRole = null;
        public Role.RoleActor HostRole
        {
            get { return mHostRole; }
        }
        public bool InitBag(Role.RoleActor role)
        {
            mHostRole = role;
            mBagSize = 12;
            mBuffs = new Buff[mBagSize];

            return true;
        }

        public List<Buff> GetVisualBuffs()
        {
            List<Buff> mVisualBuffs = new List<Buff>();
            foreach (var buff in mBuffs)
            {
                if (buff == null)
                {
                    continue;
                }
                if (buff.BuffTemplate.IsVisual)
                {
                    mVisualBuffs.Add(buff);
                }
            }
            return mVisualBuffs;
        }


        bool CreateAndAddBuff(Role.RoleActor owner, Role.RoleActor caster, GameData.Skill.BuffParam buffparam)//没确定放之前不要new，不然会有很多没有意义的空间
        {
            if (mBuffs == null)
                return false;
            ushort id = (ushort)buffparam.BuffId;
            if (buffparam.BuffLevel == 0)
            {
                buffparam.BuffLevel = 1;
            }
            byte level = buffparam.BuffLevel;

            var bufftemplate = CSUtility.Data.DataTemplateManager<UInt16, GameData.Skill.BuffTemplate>.Instance.GetDataTemplate(id);// GameData.Skill.BuffTemplateManager.Instance.FindBuff(id);
            if (bufftemplate == null)
            {
                return false;
            }

            var levelparam = bufftemplate.GetBuffLevelTemp(level);
            if (levelparam == null)
            {
                return false;
            }
            foreach (var i in mBuffs)
            {
                if (i == null)
                    continue;
                foreach (var j in bufftemplate.BuffMutexType)//存在互斥类型不能添加
                {
                    if (j == i.BuffTemplate.BuffType)
                        return false;
                }
                foreach (var t in i.BuffTemplate.BuffAddConditionType)//已经存在的buff排斥其他类型的buff
                {
                    if (t == i.BuffTemplate.BuffType)
                        return false;
                }
            }

            if (CheckRePlaceBuff(owner, caster, bufftemplate, buffparam))//替换与叠加
            {
                return true;
            }
            var mbuff = BuffBagCreateBuff(owner, caster, buffparam);
            AutoAddBuff(mbuff);
            return true;
        }

        bool CheckRePlaceBuff(Role.RoleActor owner, Role.RoleActor caster,GameData.Skill.BuffTemplate bufftpl, GameData.Skill.BuffParam buffparam)
        {
            switch (bufftpl.BuffReplaceType)
            {
                case GameData.Skill.ReplaceType.SameTypeChange://同类型替换
                    {
                        foreach (var buff in mBuffs)
                        {
                            if (buff.BuffTemplate.BuffType == bufftpl.BuffType)
                            {
                                RePlaceBuff(owner, caster, buffparam, (byte)buff.Position);
                                return true;
                            }
                        }
                        break;
                    }
            }
            var abuff = GetBuff(bufftpl.Id);
            if (abuff == null)
                return false;
            var levelparam = bufftpl.GetBuffLevelTemp(buffparam.BuffLevel);
            switch (bufftpl.BuffReplaceType)
            {
                case GameData.Skill.ReplaceType.NoChange://相同id的buff不叠加
                    {
                        return true;
                    }
                case GameData.Skill.ReplaceType.HighLvChangeLowLv://高等级替换低等级
                    {
                        if (abuff.BuffData.BuffLevel <= buffparam.BuffLevel)
                        {
                            RePlaceBuff(owner, caster, buffparam, (byte)abuff.Position);
                            return true;
                        }
                        break;
                    }
                case GameData.Skill.ReplaceType.LowLvChangeHighLv://低等级替换高等级
                    {
                        if (abuff.BuffData.BuffLevel > buffparam.BuffLevel)
                        {
                            RePlaceBuff(owner, caster, buffparam, (byte)abuff.Position);
                            return true;
                        }
                        break;
                    }
                case GameData.Skill.ReplaceType.SameLvChange://同等级相互替换
                    {
                        if (abuff.BuffData.BuffLevel == buffparam.BuffLevel)
                        {
                            RePlaceBuff(owner, caster, buffparam, (byte)abuff.Position);
                            return true;
                        }
                        break;
                    }
                case GameData.Skill.ReplaceType.SameLvStack://同等级叠加
                    {
                        if (abuff.BuffData.BuffLevel == buffparam.BuffLevel)
                        {
                            if (abuff.BuffData.BuffLayer + 1 < abuff.BuffTemplate.BuffStackValue)
                            {
                                SumBuff(owner, caster, levelparam.BuffShelfLife, abuff.Position);
                            }
                            return true;
                        }
                        break;
                    }
            }
            return false;
        }

        void RePlaceBuff(Role.RoleActor owner, Role.RoleActor caster, GameData.Skill.BuffParam buffparam, byte pos)
        {
            DeleteBuff(pos);
            var buff = BuffBagCreateBuff(owner, caster, buffparam);
            AddBuff(buff, pos);
        }

        void AutoAddBuff(Buff buff)
        {
            for (ushort i = 0; i < BagSize; ++i)
            {
                if (mBuffs[i] == null)
                {
                    AddBuff(buff, i);
                    return;
                }
            }
        }

        void SumBuff(Role.RoleActor owner, Role.RoleActor caster, long livetime, ushort pos)
        {
            switch (mBuffs[pos].BuffTemplate.BuffLiveTimeType)
            {
                case GameData.Skill.TimeType.ChangeTime:
                    {
                        mBuffs[pos].BuffData.LiveTime = livetime;
                        ++mBuffs[pos].BuffData.BuffLayer;
                        UpdateBuffLivTime(mBuffs[pos]);
                    }
                    break;
                case GameData.Skill.TimeType.DontChangeTime:
                    {
                        ++mBuffs[pos].BuffData.BuffLayer;
                        UpdateBuffLivTime(mBuffs[pos]);
                        break;
                    }
                case GameData.Skill.TimeType.SumTime:
                    {
                        mBuffs[pos].BuffData.LiveTime += livetime;
                        ++mBuffs[pos].BuffData.BuffLayer;
                        UpdateBuffLivTime(mBuffs[pos]);
                        break;
                    }
            }
            if (mBuffs[pos].BuffTemplate.FreshRoleAttribute)//叠加时更新数据
            {
                HostRole.FreshRoleValue(false);
            }
            try
            {
//                 if (mBuffs[pos].BuffTemplate.mOnStackCB != null)
//                 {
//                     var fun = mBuffs[pos].BuffTemplate.mOnStackCB.GetCallee() as CSCommon.Data.Skill.FOnStack;
//                     if (fun != null)
//                         fun(HostRole, mBuffs[pos].BuffData);
//                 }

            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }

        }

        void UpdateBuffLivTime(Buff buff)
        {
            if (mHostRole != null)
            {
                RPC.PackageWriter pkg = new RPC.PackageWriter();

                Client.H_GameRPC.smInstance.HIndex(pkg, mHostRole.SingleId).RPC_FreshBuff(pkg,  buff.BuffData.BuffId, buff.BuffData.LiveTime);
                mHostRole.HostMap.SendPkg2Clients(null, mHostRole.Placement.GetLocation(), pkg);
            }
        }

        void AddBuff(Buff buff, ushort pos)
        {
            if (pos > BagSize)
            {
                return;
            }
            ClearnOtherBuff(buff);//清楚其他buff规则
            buff.ParentBuff = this;
            this[pos] = buff;
            buff.Position = pos;
            buff.OnCreateAdd2Bag();//buff创建回调
                                   //      ReArrangeBuff();
                                   //这里做角色的数值计算
            if (buff.BuffTemplate.FreshRoleAttribute)
            {
                mHostRole.FreshRoleValue(false);
            }
            if (mHostRole != null)
            {
                RPC.PackageWriter pkg = new RPC.PackageWriter();
                Client.H_GameRPC.smInstance.HIndex(pkg, mHostRole.SingleId).RPC_AddBuff(pkg,buff.BuffData);
                mHostRole.HostMap.SendPkg2Clients(null, mHostRole.Placement.GetLocation(), pkg);
            }
        }//确定放就new

        void ClearnOtherBuff(Buff buff)
        {
            foreach (var j in buff.BuffTemplate.CleanBuffType)
            {
                for (ushort i = 0; i < BagSize; ++i)
                {
                    if (mBuffs[i] == null)
                        continue;

                    if (mBuffs[i].BuffTemplate.BuffType == j)
                    {
                        DeleteBuff(i);
                    }
                }

            }

            foreach (var j in buff.BuffTemplate.BuffCleanBuffs)
            {
                for (ushort i = 0; i < BagSize; ++i)
                {
                    if (mBuffs[i] == null)
                        continue;
                    if (mBuffs[i].BuffTemplate.Id == j)
                    {
                        DeleteBuff(i);
                    }
                }
            }
        }

        public Buff FindSkillById(Guid buffId)
        {
            foreach (var i in mBuffs)
            {
                if (i.BuffData.BuffId == buffId)
                    return i;
            }
            return null;
        }

        public Buff GetBuff(UInt16 buffId)
        {
            foreach (var i in mBuffs)
            {
                if (i == null)
                    continue;
                if (i.BuffTemplate.Id == buffId)
                {
                    return i;
                }
            }
            return null;
        }

        public virtual UInt16 FindEmptyPosition()
        {
            for (UInt16 i = 0; i < BagSize; i++)
            {
                if (mBuffs[i] == null)
                    return i;
            }
            return UInt16.MaxValue;
        }

        public string GetBagStateSaver(List<GameData.Skill.BuffData> resultBuffs)
        {
            if (mBuffs == null)
                return "";
            string bagOrder = "";

            for (int i = 0; i < mBuffs.Length; i++)
            {
                if (mBuffs[i] != null)
                {
                    if (mBuffs[i].BuffTemplate.DelBuffOnExitGame)
                        continue;
                    resultBuffs.Add(mBuffs[i].BuffData);
                    bagOrder += i.ToString() + ':' + mBuffs[i].BuffData.BuffId + ';';
                }
            }
            return bagOrder;
        }

        public UInt16 GetEmptyCount()
        {
            UInt16 count = 0;
            foreach (var i in mBuffs)
            {
                if (i == null)
                    count++;
            }
            return count;
        }

        public void Tick(Int64 elapsedMiliSeccond)
        {
            if (mBuffs == null)
                return;
            for (ushort i = 0; i < BagSize; ++i)
            {
                if (mBuffs[i] == null)
                    continue;
                if (mBuffs[i].BuffTemplate.BuffType == GameData.Skill.BuffType.AvoidHurt)
                {
                //    if (mHostRole.BuffRoleValue.TotalFixFressHurt == 0)
                    {
                        DeleteBuff(i);
                        continue;
                    }
                }

                if (mBuffs[i].Tick(elapsedMiliSeccond))
                {
                    DeleteBuff(i);
                    continue;
                }
                if (mBuffs[i].mImmediateDeath == true)
                {
                    DeleteBuff(i);
                    continue;
                }
            }
        }

        public void OnLeaveMapDeleteBuff()
        {
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                if (this[i].BuffTemplate.DelBuffOnLeaveMap)
                {
                    DeleteBuff(i);
                }
            }
        }

        public void DeleteBuffBuId(UInt16 tempid)
        {
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                if (this[i].BuffTemplate.Id == tempid)
                {
                    DeleteBuff(i);
                }
            }
        }

        public void DeleteBuff(ushort i)
        {
            var curBuff = this[i];

            bool fresh = curBuff.BuffTemplate.FreshRoleAttribute;
            if (curBuff != null)
            {
                if (mHostRole != null)
                {
                    RPC.PackageWriter pkg = new RPC.PackageWriter();
                    Client.H_GameRPC.smInstance.HIndex(pkg, mHostRole.SingleId).RPC_RemoveBuff(pkg,curBuff.BuffData);
                    mHostRole.HostMap.SendPkg2Clients(null, mHostRole.Placement.GetLocation(), pkg);
                }
                curBuff.DestroyFromDB(mHostRole);
                curBuff.FreshOnDelegate(false);
                curBuff.OnDeleteBuff();
                this[i] = null;
                ReArrangeBuff();
                if (fresh)
                {
                    mHostRole.FreshRoleValue(false);
                }
            }
        }

        public void DeleteBuffOnDeath()
        {
            if (mBuffs == null)
            {
                return;
            }
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                if (this[i].BuffTemplate.DelBuffOnDeath)
                {
                    DeleteBuff(i);
                }
            }
        }

        public void DeleteDBuff()//删除负面buff
        {
            if (mBuffs == null)
            {
                return;
            }
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                if (this[i].BuffTemplate.EBuffUserType == GameData.Skill.BuffUserType.Debuff)
                {
                    DeleteBuff(i);
                }
            }
        }

        public void DeleteAllBuff()
        {
            if (mBuffs == null)
            {
                return;
            }
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                DeleteBuff(i);
            }
        }

        public bool CheckBuffTypeOfSpeedDown()
        {
            if (mBuffs == null)
            {
                return false;
            }
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                if (this[i].BuffTemplate.BuffType == GameData.Skill.BuffType.SpeedDown)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckBuffTypeOfSeal()
        {
            if (mBuffs == null)
            {
                return false;
            }
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                if (this[i].BuffTemplate.BuffType == GameData.Skill.BuffType.Seal)
                {
                    return true;
                }
            }
            return false;
        }

        public void DeleteBuffOnNpcReset()
        {
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                if (this[i].BuffTemplate.DelBuffOnNpcReset)
                {
                    DeleteBuff(i);
                }
            }
        }

        public void DeletePassiveSkillBuff()
        {
            for (byte i = 0; i < mBagSize; ++i)
            {
                if (this[i] == null)
                {
                    continue;
                }
                if (this[i].BuffTemplate.BuffType == GameData.Skill.BuffType.PassiveSkill)
                {
                    DeleteBuff(i);
                }
            }
        }

        void ReArrangeBuff()
        {
            for (ushort i = 0; i < BagSize; ++i)
            {
                for (ushort j = (ushort)(i + 1); j < BagSize; ++j)
                {
                    if (this[i] == null && this[j] == null || (this[i] != null && this[j] == null))
                        continue;

                    if (this[i] == null && this[j] != null)
                    {
                        this[i] = this[j];
                        this[i].Position = i;
                        this[j] = null;
                        continue;
                    }
                }
            }
        }

        public Buff BuffBagCreateBuff(Role.RoleActor owner, Role.RoleActor caster, GameData.Skill.BuffParam buffparam)
        {
            GameData.Skill.BuffData data = new GameData.Skill.BuffData();
            data.BuffTemlateId = (uint)buffparam.BuffId;
            data.BuffLevel = buffparam.BuffLevel;
        //    data.SkillDataAdd = buffparam.BuffAdd;
            if (data.GetLevelTemplate() == null)
            {
                data.LiveTime = 0;
            }
            else
            {
                data.LiveTime = (long)data.GetLevelTemplate().BuffShelfLife;
            }

            var buff = Buff.DangerousCreateBuff(owner, caster, data, true);
            return buff;
        }

        public void CreateBuffAndAutoAdd2Bag(Role.RoleActor owner, Role.RoleActor caster, GameData.Skill.BuffParam buffparam)
        {
            if (owner == null)
            {
                return;
            }

            double ran = RoleActor.Random.NextDouble();
            if (ran <= buffparam.SpellRate)
            {
                CreateAndAddBuff(owner, caster, buffparam);
            }
        }

        public void FreshBuffRoleValue()
        {
            if (HostRole == null)
            {
                return;
            }

            foreach (var buff in mBuffs)
            {
                if (buff == null)
                    continue;
                if (buff.BuffTemplate.BuffLevelData == null)
                    continue;
                var data = buff.BuffData.LevelTemplate;
                if (data == null)
                    continue;
             //   HostRole.NonDevelopValue.CriticalValue += (int)data.ReduceCriticalValue;  //暴击都在NoDevelopvalue中处理 
              //  HostRole.NonDevelopValue.PerCriticalValue += data.ReduceCriticalOdds;
               // HostRole.NonDevelopValue.PerCriticalValue = HostRole.NonDevelopValue.PerCriticalValue * (1 + data.ReduceCriticalOddsDate);
                //HostRole.NonDevelopValue.PerCriticalDefendValue +=data.cri
               // HostRole.BuffRoleValue.AddValue(data.GetNpcRoleValue());
            }
        }

        public void FreshFinalRoleValue()
        {
            if (mBuffs == null)
                return;
            foreach (var buff in mBuffs)
            {
                if (buff == null)
                    continue;
                if (buff.BuffTemplate.BuffLevelData == null)
                    continue;
                var data = buff.BuffData.LevelTemplate;

                if (data == null)
                    continue;
                for (int i = 0; i < buff.BuffData.BuffLayer + 1; ++i)
                {
                }
            }
        }

        public void FreshBuffRoleBaseValue()
        {
            if (BagSize == byte.MaxValue)
                return;
            if (mBuffs == null)
                return;
            foreach (var buff in mBuffs)
            {
                if (buff == null)
                    continue;
                if (buff.BuffTemplate.BuffLevelData == null)
                    continue;
                var data = buff.BuffData.LevelTemplate;
                if (data == null)
                    continue;

                for (int i = 0; i < buff.BuffData.BuffLayer + 1; ++i)
                {
                    HostRole.RoleStrength = (int)(HostRole.RoleStrength * (1 + data.BuffRoleStrengthData));
                    HostRole.RoleIntellect = (int)(HostRole.RoleIntellect * (1 + data.BuffRoleIntellectData));
                    HostRole.RoleSkillful = (int)(HostRole.RoleSkillful * (1 + data.BuffRoleSkillfulData));

                    HostRole.RoleStrength += data.BuffRoleStrength;
                    HostRole.RoleIntellect += data.BuffRoleIntellect;
                    HostRole.RoleSkillful += data.BuffRoleSkillful;
                }
            }
        }


    }

    public class Buff
    {
        public UInt16 Position;

        GameData.Skill.BuffData mBuffData = null;
        public GameData.Skill.BuffData BuffData
        {
            get{ return mBuffData; }
            set { mBuffData = value; }
        }

        GameData.Skill.BuffTemplate mBuffTemplate = null;
        public GameData.Skill.BuffTemplate BuffTemplate
        {
            get { return mBuffTemplate; }
            set { mBuffTemplate = value; }
        }

        public Buff(UInt16 buffTemplateId)
        {
            mBuffTemplate = CSUtility.Data.DataTemplateManager<UInt16, GameData.Skill.BuffTemplate>.Instance.GetDataTemplate(buffTemplateId);// GameData.Skill.BuffTemplateManager.Instance.FindBuff(buffTemplateId);
            mLevel = mBuffData.BuffLevel;
        }

        public Buff(GameData.Skill.BuffData buff)//CSCommon中buff到Server中buff转换，派生类中的数值都要重新设置
        {
            mBuffTemplate = buff.Template;
            mBuffData = buff;
            mLevel = buff.BuffLevel;
        }
        Role.RoleActor mOwnerRole;

        Byte mLevel = 1;
        public Byte Level
        {
            get { return mLevel; }
        }

        BuffBag mParentBuff = null;
        public BuffBag ParentBuff
        {
            get { return mParentBuff; }
            set { mParentBuff = value; }
        }

        public void SetMyself2Death()
        {
            mBuffData.LiveTime = 0;
            mImmediateDeath = true;
        }
        public GameData.Skill.BuffLevelTemplate GetCurLevelData()
        {
            if (mBuffData.LevelTemplate == null)
            {
                return null;
            }
            return mBuffData.LevelTemplate;
        }

        Int64 mCreateTime = 0;
        public Int64 CreateTime
        {
            get { return mCreateTime; }
            set { mCreateTime = value; }
        }

        public bool mImmediateDeath = false;

        sbyte mBuffDot = 0;
        Int64 mUpdateBuffTime = 0;

        public static Buff DangerousCreateBuff(Role.RoleActor owner, Role.RoleActor caster, GameData.Skill.BuffData data, bool bAllocBuffId)
        {
            if (data.Template == null)
                return null;

            if (owner != null)
            {
                if (caster != null)
                {
                    data.CasterId = caster.Id;
                }
                data.OwnerId = owner.Id;
            }
            if (bAllocBuffId)
            {
                data.BuffId = Guid.NewGuid();
                data.CreateTime = System.DateTime.Now;
            }
            Buff result = new Buff(data);
            result.mOwnerRole = owner;
            result.CreateTime = owner.GetCurFrameTickCount();
            //             if (result.mBuffTemplate.mOnCreateCB != null)
            //             {
            //                 var fun = result.mBuffTemplate.mOnCreateCB.GetCallee() as CSCommon.Data.Skill.FOnCreate;
            //                 if (fun != null)
            //                 {
            //                     fun(result.mOwnerRole, data);
            //                 }                
            //             }
            //             result.FreshOnDelegate(true);
            return result;
        }

        public void OnCreateAdd2Bag()
        {
            try
            {
//                 if (mBuffTemplate.mOnCreateCB != null)
//                 {
//                     var fun = mBuffTemplate.mOnCreateCB.GetCallee() as CSCommon.Data.Skill.FOnCreate;
//                     if (fun != null)
//                     {
//                         fun(mOwnerRole, BuffData);
//                     }
//                 }
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
            FreshOnDelegate(true);
        }

        public void FreshOnDelegate(bool init)
        {

        }

        public void OnDeleteBuff()
        {
            if (BuffData.Template.BuffType == GameData.Skill.BuffType.FrozenState || BuffData.Template.BuffType == GameData.Skill.BuffType.SwimState)
            {
             //   mOwnerRole.BuffLeaveBeAttackState(BuffData);
            }
        }

        void DoOnDelegate(Role.RoleActor targetrole)
        {
            try
            {
//                 if (BuffTemplate.mOnDelegateCB != null)
//                 {
//                     var fun = BuffTemplate.mOnDelegateCB.GetCallee() as CSCommon.Data.Skill.FOnDelegate;
//                     if (fun != null)
//                     {
//                         fun(mOwnerRole, BuffData, targetrole, 0);
//                     }
//                 }
            }
            catch (Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }

        }

        void DoOnDelegate(Role.RoleActor targetrole, float hurtvalue)
        {
            try
            {
//                 if (BuffTemplate.mOnDelegateCB != null)
//                 {
//                     var fun = BuffTemplate.mOnDelegateCB.GetCallee() as CSCommon.Data.Skill.FOnDelegate;
//                     if (fun != null)
//                     {
//                         fun(mOwnerRole, BuffData, targetrole, hurtvalue);
//                     }
//                 }
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
        }

        void DoOnDeathOnTimer(Role.RoleActor targetrole)
        {
            try
            {
//                 if (BuffTemplate.mOnDelegateCB != null)
//                 {
//                     var fun = BuffTemplate.mOnDelegateCB.GetCallee() as CSCommon.Data.Skill.FOnDelegate;
//                     if (fun != null)
//                     {
//                         fun(mOwnerRole, BuffData, targetrole, 0);
//                     }
//                 }
//                 mOwnerRole.OnDeathOnTimer -= DoOnDeathOnTimer;
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
        }

        public void DestroyFromDB(Role.RoleActor role)
        {
         //   sbyte destroy = 1;
            //以后这里要根据物品重要程度决定是否真实从数据库删除
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            try
            {
//                 if (BuffData.Template.mOnDeleteCB != null)
//                 {
//                     var fun = BuffData.Template.mOnDeleteCB.GetCallee() as CSCommon.Data.Skill.FOnDelete;
//                     if (fun != null)
//                     {
//                         fun(role, BuffData);
//                     }
//                 }
            }
            catch (Exception e)
            {
                Log.FileLog.WriteLine(e.ToString());
                Log.FileLog.WriteLine(e.StackTrace.ToString());
            }
        }

    
        public bool Tick(Int64 elapsedMiliSeccond)
        {
            if (mOwnerRole == null)
                return false;
            Int64 time = mOwnerRole.GetCurFrameTickCount();
            var leveldata = mBuffData.LevelTemplate;
            if (leveldata == null)
            {
                return true;
            }
            mUpdateBuffTime += elapsedMiliSeccond;
            if (leveldata.BuffDotTick > 0 && mBuffDot < leveldata.BuffDotTick && mUpdateBuffTime > leveldata.BuffDotTickTime)
            {
                mUpdateBuffTime = 0;
                mBuffDot++;
//                 if (BuffTemplate.mOnTimerCB != null)
//                 {
//                     var fun = BuffTemplate.mOnTimerCB.GetCallee() as CSCommon.Data.Skill.FOnTimer;
//                     if (fun != null)
//                     {
//                         try
//                         {
//                             fun(mOwnerRole, BuffData);
//                         }
//                         catch (System.Exception ex)
//                         {
//                             Log.FileLog.WriteLine(ex.ToString());
//                             Log.FileLog.WriteLine(ex.StackTrace.ToString());
//                         }
//                     }
//                 }
            }
            if (this.mBuffTemplate.ImmortalLiving == false)
                mBuffData.LiveTime = leveldata.BuffShelfLife - (time - CreateTime);

            if (mBuffData.LiveTime <= 0 && mBuffTemplate.ImmortalLiving == false)
                return true;
            else
                return false;
        }
    }
}
