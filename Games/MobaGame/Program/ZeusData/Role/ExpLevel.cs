using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameData.Role
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    [CSUtility.Data.DataTemplate(".exp", "ExpLevel", Byte.MaxValue)]
    public class ExpLevel : CSUtility.Data.IDataTemplateBase<Byte>
    {
        /// <summary>
        /// 数据模板ID
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Id")]
        public Byte Id
        {
            get;
            set;
        }

        /// <summary>
        /// 数据模板名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 数据模板版本号
        /// </summary>
        [ReadOnly(true)]
        [CSUtility.Support.DataValueAttribute("Version")]
        public UInt32 Version
        {
            get;
            set;
        }

        /// <summary>
        /// 数据模板置藏标志，共编辑器使用
        /// </summary>
        [Browsable(false)]
        public bool IsDirty
        {
            get;
            set;
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        /// <param name="src">源</param>
        /// <returns>复制成功返回true,否则返回false</returns>
        public bool CopyFrom(CSUtility.Support.ICopyable src)
        {
            return CSUtility.Support.Copyable.CopyFrom(src, this);
        }

        //static ExpLevel smInstance = new ExpLevel();
        //public static ExpLevel Instance
        //{
        //    get { return smInstance; }
        //}
        public ExpLevel()
        {
            //string pathname = "Template/ExpLevel/ExpLevel.exp";
            //CSUtility.Support.IConfigurator.FillProperty(this, pathname);
        }

        UInt16 mRoleMaxLevel = 18;
        [CSUtility.Support.DataValueAttribute("RoleMaxLevel")]
        [System.ComponentModel.DisplayName("角色最大等级")]
        [System.ComponentModel.Category("角色等级")]
        public UInt16 RoleMaxLevel
        {
            get { return mRoleMaxLevel; }
            set { mRoleMaxLevel = value; }
        }

        List<long> mLevelExps = new List<long>();
        [CSUtility.Support.DataValueAttribute("LevelExps")]
        [System.ComponentModel.DisplayName("角色等级经验表")]
        [System.ComponentModel.Category("角色等级")]
        public List<long> LevelExps
        {//级别经验表
            get { return mLevelExps; }
            set { mLevelExps = value; }
        }

        public long GetLevelupExp(int level)
        {
            if (level >= mLevelExps.Count || level == RoleMaxLevel)
                return long.MaxValue;
            return mLevelExps[level];
        }

        public UInt16 GetRoleMaxLevel()
        {
            return mRoleMaxLevel;
        }


        List<long> mDeathProvideExps = new List<long>();
        [CSUtility.Support.DataValueAttribute("DeathProvideExps")]
        public List<long> DeathProvideExps
        {//NPC死亡后提供基准经验表
            get { return mDeathProvideExps; }
            set { mDeathProvideExps = value; }
        }

        public long GetDeathProvideExps(int level)
        {
            if (level >= mDeathProvideExps.Count)
                return 1;
            return mDeathProvideExps[level];
        }

        public long GetDeathProvideExp(int level)
        {
            if (level >= mDeathProvideExps.Count)
                return 1;
            return mDeathProvideExps[level];
        }
        //以后还可能有什么其他的如技能经验表，佣兵经验表，建筑等级经验表等。
    }
}
