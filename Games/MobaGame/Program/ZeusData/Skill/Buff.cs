using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameData.Skill
{
    public enum BuffType
    {
        Dot,         //每次伤害是都加一次(伤害附加)
        SpeedDown,   //减速
        SwimState,   //眩晕
        MessState,   //混乱
        FrozenState, //冰冻
        Seal,        //封印（禁魔）
        Endure,      //霸体
        Shield,      //护盾
        Invincible,  //无敌不受到任何伤害，任何DEbuff都不会附加在拥有无敌BUFF的角色身上
        AvoidHurt,   //固定免伤
        PassiveSkill,//被动技能
    }

    public enum BuffUserType
    {
        Buff,
        Debuff,
    }

    public enum BuffAddType
    {
        Common,//自己用的通用的
        SkillAdd,//技能附加
        Damage,//伤害别人时附加
        HatredAdd,//仇恨附加
        ProcDeath,//npc死亡后添加BUFF
        SkillCritical,//技能暴击附加
        AddSummon,//加在summon上的
        BeDamage,//受到伤害时
        Notify,//通过名称为CastBuff的Notify添加
    }

    public enum EBuffDelegateType
    {
        None,//没有
        HurtAdd,        //伤害触发buff效果
        SkillAdd,       //现在无用
        SkillDamage,    //现在无用
        AvoidHurt,      //固定免伤  
        NpcTriggerDeath,//npc死亡之后触发buff效果
        HatredAdd,      //仇恨触发buff效果
        NpcDeathTimer,//死亡后多少时间再触发buff效果
        OnProHurt,//对别人造成伤害时触发buff效果（自己是攻击者）
        OnBeHurt,//被人对自己造成伤害是触发buff效果（自己是被攻击者）
        ChangeAttackState,//改变隐身，霸体等的攻击状态是调用
        HostEnterDeath,//buff拥有者进入死亡状态
        HostCritical,//buff拥有者暴击时
    }

    public enum BuffState
    {
        Other,    //对方
        OtherAll, //对方全体
        Self,     //自己
        SelfAll,  //自己全体
        All,//全体
        SummonRole,//召唤出的NPC
    }

    public enum ReplaceType
    {
        HighLvChangeLowLv,//高等级替换低等级
        LowLvChangeHighLv,//低等级替换高等级
        SameLvChange,//同等级相互替换
        SameTypeChange,//同类型替换
                       //         HighEffectChangeLowEffect,//高效果替换低效果
                       //         LowEffectChangeHighEffect,//低效果替换低效果
                       //        SameEffectChange,//同效果替换
        SameLvStack,//同等级叠加
                    //        SameEffectStack,//同效果叠加
        NoChange,
        SameIdChange,
        SameIdStack,
    }

    public enum TimeType
    {
        ChangeTime, // 时间替换
        DontChangeTime, // 不替换时间
        SumTime, // 时间叠加
    }

    public enum EBuffShowType
    {
        Show,   // 客户端显示Buff
        Hide,   // 客户端隐藏Buff
        Count,
    }


    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Buff")]
    public delegate bool FOnCreate(CSUtility.AISystem.IStateHost host, BuffData buff);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Buff")]
    public delegate bool FOnDelete(CSUtility.AISystem.IStateHost host, BuffData buff);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Buff")]
    public delegate bool FOnTimer(CSUtility.AISystem.IStateHost host, BuffData buff);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Buff")]
    public delegate bool FOnCondition(CSUtility.AISystem.IStateHost host, BuffData buff);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Buff")]
    public delegate void FOnStack(CSUtility.AISystem.IStateHost host, BuffData buff);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Buff")]
    public delegate bool FOnDelegate(CSUtility.AISystem.IStateHost host, BuffData buff, CSUtility.AISystem.IStateHost target, float param);

    [CSUtility.Data.DataTemplate(".buff", "Buff")]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    //buff模版数据
    public class BuffTemplate : RPC.IAutoSaveAndLoad, CSUtility.Data.IDataTemplateBase<UInt16>
    {
        /// <summary>
        /// 数据模板ID
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Id")]
        [System.ComponentModel.DisplayName("Id")]
        [System.ComponentModel.Description("此BUFF的唯一ID")]
        [System.ComponentModel.Category("buff基础属性")]
        public UInt16 Id
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

        List<BuffLevelTemplate> mBuffLevelData = new List<BuffLevelTemplate>();

        #region 脚本
        string mOnUsed = "";
        [CSUtility.Support.DataValueAttribute("OnUsed")]
        [System.ComponentModel.DisplayName("使用后脚本函数")]
        [System.ComponentModel.Description("此处填写技能的使用后调用的脚本函数")]
        [System.ComponentModel.Category("脚本")]
        public string OnUsed
        {
            get { return mOnUsed; }
            set { mOnUsed = value; }
        }
        public CSUtility.Helper.EventCallBack mOnStackCB;
        Guid mOnStack = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnStack")]
        [System.ComponentModel.DisplayName("Buff叠加回调")]
        [System.ComponentModel.Description("此处添加当技能创建时所需要的调用的回调函数")]
        [System.ComponentModel.Category("脚本")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("", new object[] { typeof(FOnStack) })]
        public Guid OnStack
        {
            get { return mOnStack; }
            set
            {
                mOnStack = value;
                mOnStackCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnStack), value);
            }
        }

        public CSUtility.Helper.EventCallBack mOnConditionCB;
        Guid mOnCondition = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnCondition")]
        [System.ComponentModel.DisplayName("Buff条件")]
        [System.ComponentModel.Description("此处添加当技能创建时所需要的调用的回调函数")]
        [System.ComponentModel.Category("脚本")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("", new object[] { typeof(FOnCondition) })]
        public Guid OnCondition
        {
            get { return mOnCondition; }
            set
            {
                mOnCondition = value;
                mOnConditionCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnCondition), value);
            }
        }


        public CSUtility.Helper.EventCallBack mOnCreateCB;
        Guid mOnCreate = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnCreate")]
        [System.ComponentModel.DisplayName("Buff创建回调")]
        [System.ComponentModel.Description("此处添加当技能创建时所需要的调用的回调函数")]
        [System.ComponentModel.Category("脚本")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("", new object[] { typeof(FOnCreate) })]
        public Guid OnCreate
        {
            get { return mOnCreate; }
            set
            {
                mOnCreate = value;
                mOnCreateCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnCreate), value);
            }
        }

        public CSUtility.Helper.EventCallBack mOnDeleteCB;
        Guid mOnDelete = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnDelete")]
        [System.ComponentModel.DisplayName("Buff删除回调")]
        [System.ComponentModel.Description("此处添加当技能删除时所需要的调用的回调函数")]
        [System.ComponentModel.Category("脚本")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("", new object[] { typeof(FOnDelete) })]
        public Guid OnDelete
        {
            get { return mOnDelete; }
            set
            {
                mOnDelete = value;
                mOnDeleteCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnDelete), value);
            }
        }

        public CSUtility.Helper.EventCallBack mOnTimerCB;
        Guid mOnTimer = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnTimer")]
        [System.ComponentModel.DisplayName("Buff定时回调")]
        [System.ComponentModel.Description("此处为buff需要定时做某些事时所需要的调用的回调函数")]
        [System.ComponentModel.Category("脚本")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("", new object[] { typeof(FOnTimer) })]
        public Guid OnTimer
        {
            get { return mOnTimer; }
            set
            {
                mOnTimer = value;
                mOnTimerCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnTimer), value);
            }
        }

        public CSUtility.Helper.EventCallBack mOnDelegateCB;
        Guid mOnDelegate = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnDelegate")]
        [System.ComponentModel.DisplayName("Buff代理回调")]
        [System.ComponentModel.Description("此处为buff需要在指定的位置做某些事时所需要的调用的回调函数")]
        [System.ComponentModel.Category("脚本")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("", new object[] { typeof(FOnDelegate) })]
        public Guid OnDelegate
        {
            get { return mOnDelegate; }
            set
            {
                mOnDelegate = value;
                mOnDelegateCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnDelegate), value);
            }
        }
        #endregion
        
        string mBuffName = "";
        [CSUtility.Support.DataValueAttribute("BuffName")]
        [System.ComponentModel.DisplayName("buff名称")]
        [System.ComponentModel.Description("buff名称")]
        [System.ComponentModel.Category("buff基础属性")]
        public string BuffName
        {
            get { return mBuffName; }
            set { mBuffName = value; }
        }

        EBuffShowType mBuffShowHide = EBuffShowType.Show;
        [CSUtility.Support.DataValueAttribute("BuffShowHide")]
        [System.ComponentModel.DisplayName("Buff是否显示")]
        [System.ComponentModel.Description("Buff是否显示")]
        [System.ComponentModel.Category("buff基础属性")]
        public EBuffShowType BuffShowHide
        {
            get { return mBuffShowHide; }
            set { mBuffShowHide = value; }
        }

        BuffUserType mEBuffUserType = BuffUserType.Buff;
        [CSUtility.Support.DataValueAttribute("EBuffUserType")]
        [System.ComponentModel.DisplayName("Buff增益类型")]
        [System.ComponentModel.Description("Buff增益类型")]
        [System.ComponentModel.Category("buff基础属性")]
        public BuffUserType EBuffUserType
        {
            get { return mEBuffUserType; }
            set { mEBuffUserType = value; }
        }

        Guid mBuffIcon = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("BuffIcon")]
        [System.ComponentModel.DisplayName("Buff图标")]
        [System.ComponentModel.Description("Buff显示的图标")]
        [System.ComponentModel.Category("buff基础属性")]
        public Guid BuffIcon
        {
            get { return mBuffIcon; }
            set { mBuffIcon = value; }
        }

        List<Skill.SkillTemplate.NotifyEffect> mBuffNotifyEffects = new List<Skill.SkillTemplate.NotifyEffect>();
        [CSUtility.Support.DataValueAttribute("BuffNotifyEffects")]
        [System.ComponentModel.DisplayName("BUFF播放的特效列表")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<Skill.SkillTemplate.NotifyEffect> BuffNotifyEffects
        {
            get { return mBuffNotifyEffects; }
            set { mBuffNotifyEffects = value; }
        }

        bool mIsVisual = false;
        [CSUtility.Support.DataValueAttribute("IsVisual")]
        [System.ComponentModel.DisplayName("BUFF是否可见")]
        [System.ComponentModel.Category("buff基础属性")]
        public bool IsVisual
        {
            get { return mIsVisual; }
            set { mIsVisual = value; }
        }

        bool mImmortalLiving = false;
        [CSUtility.Support.DataValueAttribute("ImmortalLiving")]
        [System.ComponentModel.DisplayName("buff永久存活不计时间")]
        [System.ComponentModel.Category("buff基础属性")]
        public bool ImmortalLiving
        {
            get { return mImmortalLiving; }
            set { mImmortalLiving = value; }
        }

        List<Skill.SkillTemplate.NotifyEffect> mBuffDeleteEffects = new List<Skill.SkillTemplate.NotifyEffect>();
        [CSUtility.Support.DataValueAttribute("BuffDeleteEffects")]
        [System.ComponentModel.DisplayName("BUFF删除时播放的特效列表")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<Skill.SkillTemplate.NotifyEffect> BuffDeleteEffects
        {
            get { return mBuffDeleteEffects; }
            set { mBuffDeleteEffects = value; }
        }

        //         //状态对应的技能播放特效
        //         Guid mBuffEffect = Guid.Empty;
        //         [CSUtility.Support.DataValueAttribute("BuffEffect")]
        //         [System.ComponentModel.DisplayName("BUFF播放的特效")]
        //         [System.ComponentModel.Description("BUFF的特效")]
        //         [System.ComponentModel.Category("buff基础属性")]
        //         [EditorCommon.Assist.Editor_EffectTemplateAttribute]
        //         public Guid BuffEffect
        //         {
        //             get { return mBuffEffect; }
        //             set { mBuffEffect = value; }
        //         }

        string mNotifyName = "";
        [CSUtility.Support.DataValueAttribute("NotifyName")]
        [System.ComponentModel.DisplayName("BUFF特效挂接点")]
        [System.ComponentModel.Description("BUFF特效挂接点")]
        [System.ComponentModel.Category("buff基础属性")]
        public string NotifyName
        {
            get { return mNotifyName; }
            set { mNotifyName = value; }
        }

        SByte mBuffStackValue = 1;
        [CSUtility.Support.DataValueAttribute("BuffStackValue")]
        [System.ComponentModel.DisplayName("Buff堆叠数量上限")]
        [System.ComponentModel.Description("Buff最多可以叠几层")]
        [System.ComponentModel.Category("buff基础属性")]
        public SByte BuffStackValue
        {
            get { return mBuffStackValue; }
            set { mBuffStackValue = value; }
        }

        BuffType mBuffType;
        [CSUtility.Support.DataValueAttribute("BuffType")]
        [System.ComponentModel.DisplayName("Buff类型")]
        [System.ComponentModel.Description("Buff增益, Debuff减益, Dot, SpeedDown减速,SwimState眩晕,MessState混乱 FrozenState冰冻  Seal封印（禁魔）Endure霸体 Shield护盾 Invincible无敌")]
        [System.ComponentModel.Category("buff基础属性")]
        public BuffType BuffType
        {
            get { return mBuffType; }
            set { mBuffType = value; }
        }

        BuffState mBuffState;
        [CSUtility.Support.DataValueAttribute("BuffState")]
        [System.ComponentModel.DisplayName("Buff目标")]
        [System.ComponentModel.Description(" Other对方 OtherAll对方全体 Self自己 SelfAll自己全体 All全体")]
        [System.ComponentModel.Category("buff基础属性")]
        public BuffState BuffState
        {
            get { return mBuffState; }
            set { mBuffState = value; }
        }

        BuffAddType mBuffAdd;
        [CSUtility.Support.DataValueAttribute("BuffAdd")]
        [System.ComponentModel.DisplayName("Buff添加")]
        [System.ComponentModel.Description("buff的添加位置")]
        [System.ComponentModel.Category("buff基础属性")]
        public BuffAddType BuffAdd
        {
            get { return mBuffAdd; }
            set { mBuffAdd = value; }
        }

        bool mFreshRoleAttribute = false;
        [CSUtility.Support.DataValueAttribute("FreshRoleAttribute")]
        [System.ComponentModel.DisplayName("是否更新角色数据")]
        [System.ComponentModel.Description("是否更新角色数据，如果不更改角色的属性信息，就设为false，只有采用默认的更改时调用")]
        [System.ComponentModel.Category("buff基础属性")]
        public bool FreshRoleAttribute
        {
            get { return mFreshRoleAttribute; }
            set { mFreshRoleAttribute = value; }
        }

        bool mDelBuffOnExitGame = false;
        [CSUtility.Support.DataValueAttribute("DelBuffOnExitGame")]
        [System.ComponentModel.DisplayName("下线清除Buff")]
        [System.ComponentModel.Description("下线是否清楚Buff")]
        [System.ComponentModel.Category("buff基础属性")]
        public bool DelBuffOnExitGame
        {
            get { return mDelBuffOnExitGame; }
            set { mDelBuffOnExitGame = value; }
        }

        bool mDelBuffOnLeaveMap = true;
        [CSUtility.Support.DataValueAttribute("DelBuffOnLeaveMap")]
        [System.ComponentModel.DisplayName("过图是否清除Buff")]
        [System.ComponentModel.Description("当离开地图的时候Buff是否需要被删除")]
        [System.ComponentModel.Category("buff基础属性")]
        public bool DelBuffOnLeaveMap
        {
            get { return mDelBuffOnLeaveMap; }
            set { mDelBuffOnLeaveMap = value; }
        }

        bool mDelBuffOnDeath = true;
        [CSUtility.Support.DataValueAttribute("DelBuffOnDeath")]
        [System.ComponentModel.DisplayName("死亡是否清除Buff")]
        [System.ComponentModel.Description("当死亡的时候Buff是否需要被删除")]
        [System.ComponentModel.Category("buff基础属性")]
        public bool DelBuffOnDeath
        {
            get { return mDelBuffOnDeath; }
            set { mDelBuffOnDeath = value; }
        }

        bool mDelBuffOnNpcReset = true;
        [CSUtility.Support.DataValueAttribute("DelBuffOnNpcReset")]
        [System.ComponentModel.DisplayName("npc重置是否清除Buff")]
        [System.ComponentModel.Description("当npc重置的时候Buff是否需要被删除")]
        [System.ComponentModel.Category("buff基础属性")]
        public bool DelBuffOnNpcReset
        {
            get { return mDelBuffOnNpcReset; }
            set { mDelBuffOnNpcReset = value; }
        }

        ReplaceType mBuffReplaceType;
        [CSUtility.Support.DataValueAttribute("BuffReplaceType")]
        [System.ComponentModel.DisplayName("Buff替换规则")]
        [System.ComponentModel.Description(" HighLvChangeLowLv,//高等级替换低等级 LowLvChangeHighLv,//低等级替换高等级 SameLvChange,//同等级相互替换  SameTypeChange,//同类型替换 HighEffectChangeLowEffect,//高效果替换低效果 LowEffectChangeHighEffect,//低效果替换低效果 SameEffectChange,//同效果替换 SameLvStack,//同等级叠加 SameEffectStack,//同效果叠加")]
        [System.ComponentModel.Category("buff基础属性")]
        public ReplaceType BuffReplaceType
        {
            get { return mBuffReplaceType; }
            set { mBuffReplaceType = value; }
        }

        TimeType mBuffLiveTimeType;
        [CSUtility.Support.DataValueAttribute("BuffLiveTimeType")]
        [System.ComponentModel.DisplayName("Buff时间替换规则")]
        [System.ComponentModel.Description("ChangeTime, // 时间替换，DontChangeTime, // 不替换时间，SumTime, // 时间叠加")]
        [System.ComponentModel.Category("buff基础属性")]
        public TimeType BuffLiveTimeType
        {
            get { return mBuffLiveTimeType; }
            set { mBuffLiveTimeType = value; }
        }
        EBuffDelegateType mBuffDelegateType;
        [CSUtility.Support.DataValueAttribute("BuffDelegateType")]
        [System.ComponentModel.DisplayName("Buff代理类型")]
        [System.ComponentModel.Description("决定OnDelegate在何处调用")]
        [System.ComponentModel.Category("buff基础属性")]
        public EBuffDelegateType BuffDelegateType
        {
            get { return mBuffDelegateType; }
            set { mBuffDelegateType = value; }
        }

        Int64 mDeathDelegateTimer = 0;
        [CSUtility.Support.DataValueAttribute("DeathDelegateTimer")]
        [System.ComponentModel.DisplayName("role死亡后调用代理的延迟时间")]
        [System.ComponentModel.Description("role死亡后调用代理的延迟时间，时间不能太久，否则role离开地图后就没用了")]
        [System.ComponentModel.Category("buff基础属性")]
        public Int64 DeathDelegateTimer
        {
            get { return mDeathDelegateTimer; }
            set { mDeathDelegateTimer = value; }
        }

        float mLimiteTeamerDis = 25;
        [CSUtility.Support.DataValueAttribute("LimiteTeamerDis")]
        [System.ComponentModel.DisplayName("可以给队友添加时离队友的最远距离")]
        [System.ComponentModel.Description("可以给队友添加时离队友的最远距离")]
        [System.ComponentModel.Category("buff基础属性")]
        public float LimiteTeamerDis
        {
            get { return mLimiteTeamerDis; }
            set { mLimiteTeamerDis = value; }
        }

        List<BuffType> mBuffMutexType = new List<BuffType>();
        [CSUtility.Support.DataValueAttribute("BuffMutexType")]
        [System.ComponentModel.DisplayName("Buff互斥规则")]
        [System.ComponentModel.Description("Buff增益, Debuff减益, Dot, SpeedDown减速,SwimState眩晕,MessState混乱 FrozenState冰冻  Seal封印（禁魔）Endure霸体 Shield护盾 Invincible无敌（Buff上不了）")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<BuffType> BuffMutexType
        {
            get { return mBuffMutexType; }
            set { mBuffMutexType = value; }
        }

        List<BuffType> mBuffAddConditionType = new List<BuffType>();
        [CSUtility.Support.DataValueAttribute("BuffAddConditionType")]
        [System.ComponentModel.DisplayName("Buff添加约束,这些类型的buff加不进去")]
        [System.ComponentModel.Description("Buff增益, Debuff减益, Dot, SpeedDown减速,SwimState眩晕,MessState混乱 FrozenState冰冻  Seal封印（禁魔）Endure霸体 Shield护盾 Invincible无敌（Buff上不了）")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<BuffType> BuffAddConditionType
        {
            get { return mBuffAddConditionType; }
            set { mBuffAddConditionType = value; }
        }

        List<Skill.TargetState> mBuffTargetState = new List<Skill.TargetState>();
        [CSUtility.Support.DataValueAttribute("BuffTargetState")]
        [System.ComponentModel.DisplayName("Buff触发目标状态")]
        [System.ComponentModel.Description("None,SwimState眩晕 MessState混乱 FrozenState冰冻 Seal封印")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<Skill.TargetState> BuffTargetState
        {
            get { return mBuffTargetState; }
            set { mBuffTargetState = value; }
        }

        List<BuffType> mCleanBuffType = new List<BuffType>();
        [CSUtility.Support.DataValueAttribute("CleanBuffType")]
        [System.ComponentModel.DisplayName("Buff清除规则")]
        [System.ComponentModel.Description("Buff增益, Debuff减益, Dot, SpeedDown减速,SwimState眩晕,MessState混乱 FrozenState冰冻  Seal封印（禁魔）Endure霸体 Shield护盾 Invincible无敌（清除其他Buff规则）")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<BuffType> CleanBuffType
        {
            get { return mCleanBuffType; }
            set { mCleanBuffType = value; }
        }

        List<UInt16> mBuffCleanBuffs = new List<UInt16>();
        [CSUtility.Support.DataValueAttribute("BuffCleanBuffs")]
        [System.ComponentModel.DisplayName("Buff清除其他Buff规则")]
        [System.ComponentModel.Description("当此Buff被添加时，那些Buff需要清除")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<UInt16> BuffCleanBuffs
        {
            get { return mBuffCleanBuffs; }
            set { mBuffCleanBuffs = value; }
        }

        List<List<UInt16>> mBuffCantCreate = new List<List<UInt16>>();
        [CSUtility.Support.DataValueAttribute("BuffCantCreate")]
        [System.ComponentModel.DisplayName("其他Buff清除此Buff规则")]
        [System.ComponentModel.Description("当此Buff被添加时，如果有这些Buff，则此Buff不能被添加")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<List<UInt16>> BuffCantCreate
        {
            get { return mBuffCantCreate; }
            set { mBuffCantCreate = value; }
        }

        [CSUtility.Support.DataValueAttribute("BuffLevelData")]
        [System.ComponentModel.DisplayName("BUFF等级数据")]
        [System.ComponentModel.Description("技能等级")]
        [System.ComponentModel.Category("buff基础属性")]
        public List<BuffLevelTemplate> BuffLevelData
        {
            get { return mBuffLevelData; }
            set { mBuffLevelData = value; }
        }

        public BuffLevelTemplate GetBuffLevelTemp(byte level)
        {
            if (level == 0)
                level = 1;
            if (BuffLevelData.Count < level)
                return null;
            return BuffLevelData[level - 1];
        }
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class BuffLevelTemplate : RPC.IAutoSaveAndLoad//buff模版数据
    {
        string mBuffDescription = "";
        [CSUtility.Support.DataValueAttribute("BuffDescription")]
        [System.ComponentModel.Description("BUff描述")]
        [System.ComponentModel.DisplayName("BUff描述")]
        [System.ComponentModel.Category("1.BUFF描述")]
        public string BuffDescription
        {
            get { return mBuffDescription; }
            set { mBuffDescription = value; }
        }

        #region BUFF时间
        Int64 mBuffShelfLife;
        [CSUtility.Support.DataValueAttribute("BuffShelfLife")]
        [System.ComponentModel.Description("BUff存活时间")]
        [System.ComponentModel.DisplayName("1.BUff存活时间")]
        [System.ComponentModel.Category("2.BUFF时间")]
        public Int64 BuffShelfLife
        {
            get { return mBuffShelfLife; }
            set { mBuffShelfLife = value; }
        }

        sbyte mBuffDotTick = 0;
        [CSUtility.Support.DataValueAttribute("BuffDotTick")]
        [System.ComponentModel.Description("BUffDot次数")]
        [System.ComponentModel.DisplayName("2.BUffDot次数")]
        [System.ComponentModel.Category("2.BUFF时间")]
        public sbyte BuffDotTick
        {
            get { return mBuffDotTick; }
            set { mBuffDotTick = value; }
        }

        Int64 mBuffDotTickTime;
        [CSUtility.Support.DataValueAttribute("BuffDotTickTime")]
        [System.ComponentModel.Description("BUffTick时间间隔")]
        [System.ComponentModel.DisplayName("3.BUffTick时间间隔")]
        [System.ComponentModel.Category("2.BUFF时间")]
        public Int64 BuffDotTickTime
        {
            get { return mBuffDotTickTime; }
            set { mBuffDotTickTime = value; }
        }

        Int64 mBuffEventTime;
        [CSUtility.Support.DataValueAttribute("BuffEventTime")]
        [System.ComponentModel.Description("用在连线里的时间")]
        [System.ComponentModel.DisplayName("用在连线里的时间")]
        [System.ComponentModel.Category("2.BUFF时间")]
        public Int64 BuffEventTime
        {
            get { return mBuffEventTime; }
            set { mBuffEventTime = value; }
        }

        #endregion

        #region 技能
        Int32 mTriggerBuffID;
        [CSUtility.Support.DataValueAttribute("TriggerBuffID")]
        [System.ComponentModel.Description("BUff触发BUFFId")]
        [System.ComponentModel.DisplayName("BUff触发BUFFId")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public Int32 TriggerBuffID
        {
            get { return mTriggerBuffID; }
            set { mTriggerBuffID = value; }
        }

        Int32 mTriggerBuffLv;
        [CSUtility.Support.DataValueAttribute("TriggerBuffLv")]
        [System.ComponentModel.Description("BUff触发BUFF的等级")]
        [System.ComponentModel.DisplayName("BUff触发BUFF的等级")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public Int32 TriggerBuffLv
        {
            get { return mTriggerBuffLv; }
            set { mTriggerBuffLv = value; }
        }

        float mTriggerBuffOdds = 1;
        [CSUtility.Support.DataValueAttribute("TriggerBuffOdds")]
        [System.ComponentModel.Description("BUff触发BUFF几率")]
        [System.ComponentModel.DisplayName("BUff触发BUFF几率")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public float TriggerBuffOdds
        {
            get { return mTriggerBuffOdds; }
            set { mTriggerBuffOdds = value; }
        }
        Int32 mTriggerSkillID;
        [CSUtility.Support.DataValueAttribute("TriggerSkillID")]
        [System.ComponentModel.Description("BUff触发技能Id")]
        [System.ComponentModel.DisplayName("BUff触发技能Id")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public Int32 TriggerSkillID
        {
            get { return mTriggerSkillID; }
            set { mTriggerSkillID = value; }
        }

        Int32 mTriggerSkillLv;
        [CSUtility.Support.DataValueAttribute("TriggerSkillLv")]
        [System.ComponentModel.Description("BUff触发技能的等级")]
        [System.ComponentModel.DisplayName("BUff触发技能的等级")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public Int32 TriggerSkillLv
        {
            get { return mTriggerSkillLv; }
            set { mTriggerSkillLv = value; }
        }

        float mTriggerSkillOdds;
        [CSUtility.Support.DataValueAttribute("TriggerSkillOdds")]
        [System.ComponentModel.Description("BUff触发技能几率")]
        [System.ComponentModel.DisplayName("BUff触发技能几率")]
        [System.ComponentModel.Category("3.BUFF技能")]
        [CSUtility.Event.Attribute.ToolTip("BUff触发技能几率")]
        public float TriggerSkillOdds
        {
            get { return mTriggerSkillOdds; }
            set { mTriggerSkillOdds = value; }
        }

        Int32 mTriggerSkillRuneID;
        [CSUtility.Support.DataValueAttribute("TriggerSkillRuneID")]
        [System.ComponentModel.Description("BUff触发技能符文Id")]
        [System.ComponentModel.DisplayName("BUff触发技能符文Id")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public Int32 TriggerSkillRuneID
        {
            get { return mTriggerSkillRuneID; }
            set { mTriggerSkillRuneID = value; }
        }

        Int32 mTriggerSkillRuneLv;
        [CSUtility.Support.DataValueAttribute("TriggerSkillRuneLv")]
        [System.ComponentModel.Description("BUff触发技能符文的等级")]
        [System.ComponentModel.DisplayName("BUff触发技能符文的等级")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public Int32 TriggerSkillRuneLv
        {
            get { return mTriggerSkillRuneLv; }
            set { mTriggerSkillRuneLv = value; }
        }

        //         float mTriggerRuneOdds;
        //         [CSUtility.Support.DataValueAttribute("TriggerRuneOdds")]
        //         [System.ComponentModel.Description("BUff触发技能几率")]
        //         [System.ComponentModel.DisplayName("BUff触发技能几率")]
        //         [System.ComponentModel.Category("BUFF技能")]
        //         [CSCommon.Event.Attribute.AllowMember(CSCommon.Helper.enCSType.Server)]
        //         public float TriggerRuneOdds
        //         {
        //             get { return mTriggerRuneOdds; }
        //             set { mTriggerRuneOdds = value; }
        //         }

        float mReduceSkillCD = 0;
        [CSUtility.Support.DataValueAttribute("ReduceSkillCD")]
        [System.ComponentModel.Description("BUff减少技能cd的时间")]
        [System.ComponentModel.DisplayName("BUff减少cd的时间")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public float ReduceSkillCD
        {
            get { return mReduceSkillCD; }
            set { mReduceSkillCD = value; }
        }

        float mResetSkillCD = -1;
        [CSUtility.Support.DataValueAttribute("ResetSkillCD")]
        [System.ComponentModel.Description("BUff重置技能cd的时间")]
        [System.ComponentModel.DisplayName("BUff重置技能cd的时间")]
        [System.ComponentModel.Category("3.BUFF技能")]
        public float ResetSkillCD
        {
            get { return mResetSkillCD; }
            set { mResetSkillCD = value; }
        }
        #endregion

        #region 连线数据
        float mReduceSpeedRateData;
        [CSUtility.Support.DataValueAttribute("ReduceSpeedRateData")]
        [System.ComponentModel.Description("增加或削减移动速率百分比，最后的移动速度是乘以1-这个值")]
        [System.ComponentModel.DisplayName("增加或削减移动速率百分比")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public float ReduceSpeedRateData
        {
            get { return mReduceSpeedRateData; }
            set { mReduceSpeedRateData = value; }
        }


        int mReduceHP;
        [CSUtility.Support.DataValueAttribute("ReduceHP")]
        [System.ComponentModel.Description("增加或削减HP")]
        [System.ComponentModel.DisplayName("增加或削减HP")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public int ReduceHP
        {
            get { return mReduceHP; }
            set { mReduceHP = value; }
        }

        float mReduceHPData;
        [CSUtility.Support.DataValueAttribute("ReduceHPData")]
        [System.ComponentModel.Description("增加或削减HP百分比")]
        [System.ComponentModel.DisplayName("增加或削减HP百分比")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public float ReduceHPData
        {
            get { return mReduceHPData; }
            set { mReduceHPData = value; }
        }

        int mReduceMP;
        [CSUtility.Support.DataValueAttribute("ReduceMP")]
        [System.ComponentModel.Description("增加或削减MP")]
        [System.ComponentModel.DisplayName("增加或削减MP")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public int ReduceMP
        {
            get { return mReduceMP; }
            set { mReduceMP = value; }
        }

        float mReduceMPData;
        [CSUtility.Support.DataValueAttribute("ReduceMPData")]
        [System.ComponentModel.Description("增加或削减MP百分比")]
        [System.ComponentModel.DisplayName("增加或削减MP百分比")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public float ReduceMPData
        {
            get { return mReduceMPData; }
            set { mReduceMPData = value; }
        }

        UInt32 mSkillDamageAdd;
        [CSUtility.Support.DataValueAttribute("SkillDamageAdd")]
        [System.ComponentModel.Description("Buff对技能伤害的加成")]
        [System.ComponentModel.DisplayName("Buff对技能伤害的加成")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public UInt32 SkillDamageAdd
        {
            get { return mSkillDamageAdd; }
            set { mSkillDamageAdd = value; }
        }

        float mSkillDamageData;
        [CSUtility.Support.DataValueAttribute("SkillDamageData")]
        [System.ComponentModel.Description("Buff对技能伤害百分比的加成")]
        [System.ComponentModel.DisplayName("Buff对技能伤害百分比的加成")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public float SkillDamageData
        {
            get { return mSkillDamageData; }
            set { mSkillDamageData = value; }
        }

        float mSkillDamageAddOdds;
        [CSUtility.Support.DataValueAttribute("SkillDamageAddOdds")]
        [System.ComponentModel.Description("Buff提升技能百分比伤害")]
        [System.ComponentModel.DisplayName("Buff提升技能百分比伤害")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public float SkillDamageAddOdds
        {
            get { return mSkillDamageAddOdds; }
            set { mSkillDamageAddOdds = value; }
        }

        Int64 mBeAttackDuration;
        [CSUtility.Support.DataValueAttribute("BeAttackDuration")]
        [System.ComponentModel.Description("Buff使角色进入被攻击状态时间")]
        [System.ComponentModel.DisplayName("Buff使角色进入被攻击状态时间")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public Int64 BeAttackDuration
        {
            get { return mBeAttackDuration; }
            set { mBeAttackDuration = value; }
        }
        #endregion

        #region buff生存恢复
        int mReduceMaxHP;
        [CSUtility.Support.DataValueAttribute("ReduceMaxHP")]
        [System.ComponentModel.Description("增加或削减最大HP")]
        [System.ComponentModel.DisplayName("增加或削减最大HP")]
        [System.ComponentModel.Category("42.BUFF生存恢复")]
        public int ReduceMaxHP
        {
            get { return mReduceMaxHP; }
            set { mReduceMaxHP = value; }
        }

        float mReduceHPMaxData;
        [CSUtility.Support.DataValueAttribute("ReduceHPMaxData")]
        [System.ComponentModel.Description("增加或削减maxHP百分比")]
        [System.ComponentModel.DisplayName("增加或削减maxHP百分比")]
        [System.ComponentModel.Category("42.BUFF生存恢复")]
        public float ReduceHPMaxData
        {
            get { return mReduceHPMaxData; }
            set { mReduceHPMaxData = value; }
        }

        int mReduceRecoverHP;
        [CSUtility.Support.DataValueAttribute("ReduceRecoverHP")]
        [System.ComponentModel.Description("增加或削减HP恢复点数")]
        [System.ComponentModel.DisplayName("增加或削减HP恢复点数")]
        [System.ComponentModel.Category("42.BUFF生存恢复")]
        public int ReduceRecoverHP
        {
            get { return mReduceRecoverHP; }
            set { mReduceRecoverHP = value; }
        }

        float mReduceHPRecoverData;
        [CSUtility.Support.DataValueAttribute("ReduceHPRecoverData")]
        [System.ComponentModel.Description("增加或削减HP恢复点数百分比")]
        [System.ComponentModel.DisplayName("增加或削减HP恢复点数百分比")]
        [System.ComponentModel.Category("42.BUFF生存恢复")]
        public float ReduceHPRecoverData
        {
            get { return mReduceHPRecoverData; }
            set { mReduceHPRecoverData = value; }
        }

        int mReduceRecoverMP;
        [CSUtility.Support.DataValueAttribute("ReduceRecoverMP")]
        [System.ComponentModel.Description("增加或削减MP恢复点数")]
        [System.ComponentModel.DisplayName("增加或削减MP恢复点数")]
        [System.ComponentModel.Category("42.BUFF生存恢复")]
        public int ReduceRecoverMP
        {
            get { return mReduceRecoverMP; }
            set { mReduceRecoverMP = value; }
        }

        float mReduceMPRecoverData;
        [CSUtility.Support.DataValueAttribute("ReduceMPRecoverData")]
        [System.ComponentModel.Description("增加或削减MP恢复点数百分比")]
        [System.ComponentModel.DisplayName("增加或削减MP恢复点数百分比")]
        [System.ComponentModel.Category("42.BUFF生存恢复")]
        public float ReduceMPRecoverData
        {
            get { return mReduceMPRecoverData; }
            set { mReduceMPRecoverData = value; }
        }


        #endregion


        #region 属性效果
        int mBuffRoleStrength;//力量
        [CSUtility.Support.DataValueAttribute("BuffRoleStrength")]
        [System.ComponentModel.Description("增加力量值")]
        [System.ComponentModel.DisplayName("增加力量值")]
        [System.ComponentModel.Category("4.BUFF属性效果")]
        public int BuffRoleStrength
        {
            get { return mBuffRoleStrength; }
            set { mBuffRoleStrength = value; }
        }
        float mBuffRoleStrengthData;//力量
        [CSUtility.Support.DataValueAttribute("BuffRoleStrengthData")]
        [System.ComponentModel.Description("增加力量值百分比")]
        [System.ComponentModel.DisplayName("增加力量值百分比")]
        [System.ComponentModel.Category("4.BUFF属性效果")]
        public float BuffRoleStrengthData
        {
            get { return mBuffRoleStrengthData; }
            set { mBuffRoleStrengthData = value; }
        }

        int mBuffRoleIntellect;//智力
        [CSUtility.Support.DataValueAttribute("BuffRoleIntellect")]
        [System.ComponentModel.Description("增加智力值")]
        [System.ComponentModel.DisplayName("增加智力值")]
        [System.ComponentModel.Category("4.BUFF属性效果")]
        public int BuffRoleIntellect
        {
            get { return mBuffRoleIntellect; }
            set { mBuffRoleIntellect = value; }
        }

        float mBuffRoleIntellectData;//智力
        [CSUtility.Support.DataValueAttribute("BuffRoleIntellectData")]
        [System.ComponentModel.Description("增加智力值百分比")]
        [System.ComponentModel.DisplayName("增加智力值百分比")]
        [System.ComponentModel.Category("4.BUFF属性效果")]
        public float BuffRoleIntellectData
        {
            get { return mBuffRoleIntellectData; }
            set { mBuffRoleIntellectData = value; }
        }

        int mBuffRoleSkillful;//精准，熟练度，灵巧
        [CSUtility.Support.DataValueAttribute("BuffRoleSkillful")]
        [System.ComponentModel.Description("增加精准敏捷值")]
        [System.ComponentModel.DisplayName("增加精准敏捷值")]
        [System.ComponentModel.Category("4.BUFF属性效果")]
        public int BuffRoleSkillful
        {
            get { return mBuffRoleSkillful; }
            set { mBuffRoleSkillful = value; }
        }

        float mBuffRoleSkillfulData;//精准，熟练度，灵巧
        [CSUtility.Support.DataValueAttribute("BuffRoleSkillfulData")]
        [System.ComponentModel.Description("增加精准敏捷值百分比")]
        [System.ComponentModel.DisplayName("增加精准敏捷值百分比")]
        [System.ComponentModel.Category("4.BUFF属性效果")]
        public float BuffRoleSkillfulData
        {
            get { return mBuffRoleSkillfulData; }
            set { mBuffRoleSkillfulData = value; }
        }
        #endregion

        #region 攻击
        Int32 mFinalHurtDamage;
        [CSUtility.Support.DataValueAttribute("FinalHurtDamage")]
        [System.ComponentModel.Description("对伤害的加成")]
        [System.ComponentModel.DisplayName("对伤害的加成")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public Int32 FinalHurtDamage
        {
            get { return mFinalHurtDamage; }
            set { mFinalHurtDamage = value; }
        }

        float mFinalHurtDamageData;
        [CSUtility.Support.DataValueAttribute("FinalHurtDamageData")]
        [System.ComponentModel.Description("Buff伤害百分比的加成")]
        [System.ComponentModel.DisplayName("Buff伤害百分比的加成")]
        [System.ComponentModel.Category("41.BUFF连线数据")]
        public float FinalHurtDamageData
        {
            get { return mFinalHurtDamageData; }
            set { mFinalHurtDamageData = value; }
        }

        UInt64 mReduceTureHurtValue;
        [CSUtility.Support.DataValueAttribute("mReduceTureHurtValue")]
        [System.ComponentModel.Description("增加或削减真实伤害")]
        [System.ComponentModel.DisplayName("增加或削减真实伤害")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public UInt64 ReduceTureHurtValue
        {
            get { return mReduceTureHurtValue; }
            set { mReduceTureHurtValue = value; }
        }

        float mReduceTureHurtData;
        [CSUtility.Support.DataValueAttribute("ReduceTureHurtData")]
        [System.ComponentModel.Description("增加或削减真实伤害百分比")]
        [System.ComponentModel.DisplayName("增加或削减真实伤害百分比")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceTureHurtData
        {
            get { return mReduceTureHurtData; }
            set { mReduceTureHurtData = value; }
        }
        int mReduceDefend;
        [CSUtility.Support.DataValueAttribute("ReduceDefend")]
        [System.ComponentModel.Description("增加或削减防御点数")]
        [System.ComponentModel.DisplayName("增加或削减防御点数")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public int ReduceDefend
        {
            get { return mReduceDefend; }
            set { mReduceDefend = value; }
        }

        float mReduceDefendData;
        [CSUtility.Support.DataValueAttribute("ReduceDefendData")]
        [System.ComponentModel.Description("增加或削减防御百分比")]
        [System.ComponentModel.DisplayName("增加或削减防御力百分比")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceDefendData
        {
            get { return mReduceDefendData; }
            set { mReduceDefendData = value; }
        }

        int mReduceMaxAtk;
        [CSUtility.Support.DataValueAttribute("ReduceMaxAtk")]
        [System.ComponentModel.Description("增加或削减最大攻击力")]
        [System.ComponentModel.DisplayName("增加或削减最大攻击力")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public int ReduceMaxAtk
        {
            get { return mReduceMaxAtk; }
            set { mReduceMaxAtk = value; }
        }

        float mReduceMaxAtkData;
        [CSUtility.Support.DataValueAttribute("ReduceMaxAtkData")]
        [System.ComponentModel.Description("增加或削减最大攻击力百分比")]
        [System.ComponentModel.DisplayName("增加或削减最大攻击力百分比")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceMaxAtkData
        {
            get { return mReduceMaxAtkData; }
            set { mReduceMaxAtkData = value; }
        }

        int mReduceMinAtk;
        [CSUtility.Support.DataValueAttribute("ReduceMinAtk")]
        [System.ComponentModel.Description("增加或削减最小攻击力")]
        [System.ComponentModel.DisplayName("增加或削减最小攻击力")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public int ReduceMinAtk
        {
            get { return mReduceMinAtk; }
            set { mReduceMinAtk = value; }
        }

        float mReduceMinAtkData;
        [CSUtility.Support.DataValueAttribute("ReduceMinAtkData")]
        [System.ComponentModel.Description("增加或削减最小攻击力百分比")]
        [System.ComponentModel.DisplayName("增加或削减最小攻击力百分比")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceMinAtkData
        {
            get { return mReduceMinAtkData; }
            set { mReduceMinAtkData = value; }
        }

        //----------------------------------------------------------------------------------------------
        int mReduceAtkSpeed;
        [CSUtility.Support.DataValueAttribute("ReduceAtkSpeed")]
        [System.ComponentModel.Description("增加或削减攻击速度")]
        [System.ComponentModel.DisplayName("增加或削减攻击速度")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public int ReduceAtkSpeed
        {
            get { return mReduceAtkSpeed; }
            set { mReduceAtkSpeed = value; }
        }

        float mReduceAtkSpeedData;
        [CSUtility.Support.DataValueAttribute("ReduceAtkSpeedData")]
        [System.ComponentModel.Description("增加或削减攻击速度百分比")]
        [System.ComponentModel.DisplayName("增加或削减攻击速度百分比")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceAtkSpeedData
        {
            get { return mReduceAtkSpeedData; }
            set { mReduceAtkSpeedData = value; }
        }

        int mReduceDodgeOdds;
        [CSUtility.Support.DataValueAttribute("ReduceBlockOdds")]
        [System.ComponentModel.Description("增加或削减闪避点数")]
        [System.ComponentModel.DisplayName("增加或削减闪避点数")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public int ReduceDodgeOdds
        {
            get { return mReduceDodgeOdds; }
            set { mReduceDodgeOdds = value; }
        }

        float mReduceDodgeOddsValue;
        [CSUtility.Support.DataValueAttribute("ReduceDodgeOddsValue")]
        [System.ComponentModel.Description("增加或削减闪避几率百分比")]
        [System.ComponentModel.DisplayName("增加或削减闪避几率百分比")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceDodgeOddsValue
        {
            get { return mReduceDodgeOddsValue; }
            set { mReduceDodgeOddsValue = value; }
        }

        float mReduceCriticaHurtData;
        [CSUtility.Support.DataValueAttribute("ReduceCriticaHurtData")]
        [System.ComponentModel.Description("增加或削减暴击伤害百分比")]
        [System.ComponentModel.DisplayName("增加或削减暴击伤害百分比")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceCriticaHurtData
        {
            get { return mReduceCriticaHurtData; }
            set { mReduceCriticaHurtData = value; }
        }


        float mReduceCriticalOdds;
        [CSUtility.Support.DataValueAttribute("mReduceCriticalOdds")]
        [System.ComponentModel.Description("增加或削减暴击几率")]
        [System.ComponentModel.DisplayName("增加或削减暴击几率")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceCriticalOdds
        {
            get { return mReduceCriticalOdds; }
            set { mReduceCriticalOdds = value; }
        }

        float mReduceCriticalOddsDate;
        [CSUtility.Support.DataValueAttribute("ReduceCriticalOddsDate")]
        [System.ComponentModel.Description("增加或削减暴击几率百分比")]
        [System.ComponentModel.DisplayName("增加或削减暴击几率百分比")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceCriticalOddsDate
        {
            get { return mReduceCriticalOddsDate; }
            set { mReduceCriticalOddsDate = value; }
        }

        float mReduceBeCriticalOddDate;
        [CSUtility.Support.DataValueAttribute("ReduceBeCriticalOddDate")]
        [System.ComponentModel.Description("被暴击几率加值，正值为添加被暴击几率，负值为减少被暴击几率")]
        [System.ComponentModel.DisplayName("被暴击几率加值")]
        [System.ComponentModel.Category("5.BUFF攻击效果")]
        public float ReduceBeCriticalOddDate
        {
            get { return mReduceBeCriticalOddDate; }
            set { mReduceBeCriticalOddDate = value; }
        }
        #endregion

        #region 固定免伤
        UInt64 mAvoidHurtValue;
        [CSUtility.Support.DataValueAttribute("AvoidHurtValue")]
        [System.ComponentModel.Description("每次收到伤害都回重新就算的免伤")]
        [System.ComponentModel.DisplayName("固定免伤")]
        [System.ComponentModel.Category("6.BUFF固定免伤")]
        public UInt64 AvoidHurtValue
        {
            get { return mAvoidHurtValue; }
            set { mAvoidHurtValue = value; }
        }
        UInt64 mMaxAvoidHurtValue;
        [CSUtility.Support.DataValueAttribute("MaxAvoidHurtValue")]
        [System.ComponentModel.Description("固定免伤总量，累加数值，")]
        [System.ComponentModel.DisplayName("固定免伤总量")]
        [System.ComponentModel.Category("6.BUFF固定免伤")]
        public UInt64 MaxAvoidHurtValue
        {
            get { return mMaxAvoidHurtValue; }
            set { mMaxAvoidHurtValue = value; }
        }

        float mReduceAvoidHurtData;
        [CSUtility.Support.DataValueAttribute("ReduceAvoidHurtData")]
        [System.ComponentModel.Description("增加或削减固定免伤百分比")]
        [System.ComponentModel.DisplayName("增加或削减固定免伤百分比")]
        [System.ComponentModel.Category("6.BUFF固定免伤")]
        public float ReduceAvoidHurtData
        {
            get { return mReduceAvoidHurtData; }
            set { mReduceAvoidHurtData = value; }
        }
        #endregion
    }

    public class BuffParam 
    {
        ushort mBuffId;
        [CSUtility.Support.DataValueAttribute("BuffId")]
        [System.ComponentModel.DisplayName("BuffId")]
        public ushort BuffId
        {
            get { return mBuffId; }
            set { mBuffId = value; }
        }

        byte mBuffLevel;
        [CSUtility.Support.DataValueAttribute("BuffLevel")]
        [System.ComponentModel.DisplayName("Buff等级")]
        public byte BuffLevel
        {
            get { return mBuffLevel; }
            set { mBuffLevel = value; }
        }

        float mSpellRate = 1;            // 释放机率
        [CSUtility.Support.DataValueAttribute("SpellRate")]
        [System.ComponentModel.DisplayName("Buff释放几率")]
        public float SpellRate
        {
            get { return mSpellRate; }
            set { mSpellRate = value; }
        }
    }



    public class BuffData : RPC.IAutoSaveAndLoad
    {
        #region 核心数据一般属性
        BuffTemplate mTemplate;
        [RPC.FieldDontAutoSaveLoad()]
        [System.ComponentModel.Browsable(false)]
        public BuffTemplate Template
        {
            get { return mTemplate; }
        }

        BuffLevelTemplate mLevelTemplate = new BuffLevelTemplate();
        [RPC.FieldDontAutoSaveLoad()]
        [System.ComponentModel.Browsable(false)]
        public BuffLevelTemplate LevelTemplate
        {
            get { return mLevelTemplate; }
            set { mLevelTemplate = value; }
        }

        System.DateTime mCreateTime;
        public System.DateTime CreateTime
        {
            get { return mCreateTime; }
            set { mCreateTime = value; }
        }

        UInt32 mBuffTemlateId;
        public UInt32 BuffTemlateId
        {
            get
            {
                if (mTemplate == null)
                    return mBuffTemlateId;
                return mTemplate.Id;
            }
            set
            {
                mBuffTemlateId = value;
                mTemplate = CSUtility.Data.DataTemplateManager<UInt16, BuffTemplate>.Instance.GetDataTemplate((ushort)value);// BuffTemplateManager.Instance.FindBuff((ushort)value);
            }
        }

        Guid mBuffId;
        public Guid BuffId
        {
            get { return mBuffId; }
            set { mBuffId = value; }
        }

        Guid mOwnerId;    //玩家ID
        public Guid OwnerId
        {
            get { return mOwnerId; }
            set { mOwnerId = value; }
        }

        Guid mCasterId;    //释放者ID
        public Guid CasterId
        {
            get { return mCasterId; }
            set { mCasterId = value; }
        }

        int mStackNum = 1;
        public int StackNum
        {
            get { return mStackNum; }
            set { mStackNum = value; }
        }

        Int64 mLiveTime = 0;                        // 存活的时间
        public Int64 LiveTime
        {
            get { return mLiveTime; }
            set { mLiveTime = value; }
        }

        byte mBuffLevel = 0;                        // 等级
        public byte BuffLevel
        {
            get
            {
                return mBuffLevel;
            }
            set
            {
                if (value == 0)
                    value = 1;
                mBuffLevel = value;
                SetLevelTemplate();
            }
        }

        byte mBuffLayer = 0;                        // 叠加层数
        public byte BuffLayer
        {
            get { return mBuffLayer; }
            set { mBuffLayer = value; }
        }
        #endregion

        #region 连线 
        public void SetOnTimer(sbyte buffdot, Int64 ticktime)
        {
            if (Template == null)
            {
                return;
            }
            if (BuffLevel >= Template.BuffLevelData.Count)
            {
                return;
            }
            //    Template.GetBuffLevelTemplate(BuffLevel).BuffDotTick = buffdot;
            //      Template.GetBuffLevelTemplate(BuffLevel).BuffDotTickTime = ticktime;
        }

        public BuffLevelTemplate GetLevelTemplate()
        {
            return mLevelTemplate;
        }

        void SetLevelTemplate()
        {
            if (Template == null)
            {
                return;
            }

            if (Template.BuffLevelData.Count < mBuffLevel)
            {
                return;
            }
            if (mBuffLevel == 0)
            {
                mBuffLevel = 1;
            }

            mLevelTemplate = Template.BuffLevelData[(byte)(mBuffLevel - 1)];
        }
        #endregion
    }

}
