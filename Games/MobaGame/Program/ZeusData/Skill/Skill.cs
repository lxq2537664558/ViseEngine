using CSUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameData.Skill
{
    public enum FireSkillType
    {
        Prompt, //瞬发
        Chant,  //吟唱
        Lead,   //引导
    }

    public enum ESkillAttackType//技能攻击类型，技能判定，攻击盒还是召唤sum碰撞
    {
        Region,
        Summon,
    }

    public enum ESkillSumType//召唤类型
    {
        CommonSum,//普通召唤
        Monster,//召唤怪物

    }

    public enum ESkillOperationType //技能操作类型
    {
        NonSkillshot, //非指向型
        Skillshot, //指向型
    }

    public enum ESkillRangeType//技能有链接时链接类型
    {
        One,
        Multi,
        AOE,
    }

    public enum SkillTarget//技能目标
    {
        Other,    //对方
        OtherAll, //对方全体
        Self,     //自己
        SelfAll,  //自己全体
    }

    public enum TargetState//目标状态
    {
        None,
        SwimState,   //眩晕
        MessState,   //混乱
        FrozenState, //冰冻
        Seal,        //封印
    }

    public enum ESkillType//技能类型
    {
        InitiativeSkill = 0,
        PassiveSkill = 1,
        Unknown = 0xFF
    }

    public enum EOffsetType
    {
        None = 0,
        OffsetSelf = 1,                // 自身位移
        OffsetTarget = 2,             // 使目标位移,
        HitBack = 3,                    //击退
        JumpUp = 4,                      //跳跃
        Spurt = 5,                        //冲刺
        Flicker = 6,                      //闪烁
    }

    public enum EEffectChainType
    {
        Line = 0,                   // 直线
        Parabola = 1,           // 抛物线
        Curve = 2,               // 曲线
    }

    public enum EEmissionPathType
    {
        Line = 0,                   // 直线
        Parabola = 1,           // 抛物线
        Wave = 2,               // 波浪线
        LineTurn = 3,           // 折返线
    }
    public enum EEmissionType
    {
        Bunch = 0,                   // 穿成一串
        Distribute = 1,               // 散弹
        Random = 2,                   //随机
        SkillShotRan = 3,            //指向性技能随机
    }

    public enum ESkillSummonType//召唤物类型
    {
        Ball,//球
        Column,//圆柱
        Cuboid,//长方体

    }

    public enum EFireSkillType
    {
        Default,//默认施法，别的条件都不满足，就用这个了
        HpTrigger,//根据血量施法
        RateTrigger,//有一定的几率触发
        LengthPriotity,//指定距离内这个技能是最优的
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class SkillInfo
    {
        UInt16 mSkillId;
        [CSUtility.Support.DataValueAttribute("SkillId")]
        public UInt16 SkillId
        {
            get { return mSkillId; }
            set { mSkillId = value; }
        }

        byte mSkillLevel = 1;
        [CSUtility.Support.DataValueAttribute("SkillLevel")]
        public byte SkillLevel
        {
            get { return mSkillLevel; }
            set { mSkillLevel = value; }
        }

        EFireSkillType mSkillFireType = EFireSkillType.Default;
        [CSUtility.Support.DataValueAttribute("SkillFireType")]
        [System.ComponentModel.DisplayName("技能释放类型")]
        public EFireSkillType SkillFireType
        {
            get { return mSkillFireType; }
            set { mSkillFireType = value; }
        }

        float mSkillFireHpValue = 0;
        [CSUtility.Support.DataValueAttribute("SkillFireHpValue")]
        [System.ComponentModel.DisplayName("释放Hp百分比")]
        public float SkillFireHpValue
        {
            get { return mSkillFireHpValue; }
            set { mSkillFireHpValue = value; }
        }

        float mRate = 1;
        [CSUtility.Support.DataValueAttribute("Rate")]
        [System.ComponentModel.DisplayName("技能释放几率（默认技能不计算几率）")]
        public float Rate
        {
            get { return mRate; }
            set { mRate = value; }
        }

        float mMinLength = 0;
        [CSUtility.Support.DataValueAttribute("MinLength")]
        [System.ComponentModel.DisplayName("释放最小距离")]
        public float MinLength
        {
            get { return mMinLength; }
            set { mMinLength = value; }
        }

        float mMaxLength = float.MaxValue;
        [CSUtility.Support.DataValueAttribute("MaxLength")]
        [System.ComponentModel.DisplayName("释放最da距离")]
        public float MaxLength
        {
            get { return mMaxLength; }
            set { mMaxLength = value; }
        }


        Int64 mCD = 0;
        [CSUtility.Support.DataValueAttribute("CD")]
        [System.ComponentModel.DisplayName("技能cd（默认技能不计算CD）")]
        public Int64 CD
        {
            get { return mCD; }
            set { mCD = value; }
        }

        byte mCanFireTimes = byte.MaxValue;
        [CSUtility.Support.DataValueAttribute("CanFireTimes")]
        [System.ComponentModel.DisplayName("技能释放次数（默认技能不计算次数）")]
        public byte CanFireTimes
        {
            get { return mCanFireTimes; }
            set { mCanFireTimes = value; }
        }
    }

    public class SkillInfoData
    {
        Int64 mFireTime = 0;
        public Int64 FireTime
        {
            get { return mFireTime; }
            set { mFireTime = value; }
        }

        byte mCurFireTimes = 0;
        public byte CurFireTimes
        {
            get { return mCurFireTimes; }
            set { mCurFireTimes = value; }
        }
    }

    [CSUtility.Data.DataTemplate(".st", "Skill")]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class SkillTemplate : CSUtility.Support.Copyable, CSUtility.Data.IDataTemplateBase<UInt16>
    {
        /// <summary>
        /// 数据模板ID
        /// </summary>
        [Browsable(false)]
        public UInt16 Id
        {
            get { return mSkillId; }
            set { mSkillId = value; }
        }

        /// <summary>
        /// 数据模板名称
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get { return mSkillName; }
            set { mSkillName = value; }
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

        #region 常用
        UInt16 mSkillId;
        [CSUtility.Support.DataValueAttribute("SkillId")]
        [System.ComponentModel.DisplayName("技能ID")]
        [System.ComponentModel.Description("技能ID")]
        [System.ComponentModel.Category("1.常用")]
        public UInt16 SkillId
        {
            get { return mSkillId; }
            set { mSkillId = value; }
        }

        ESkillType mSkillType = ESkillType.Unknown;
        [CSUtility.Support.DataValueAttribute("SkillType")]
        [System.ComponentModel.DisplayName("技能类型")]
        [System.ComponentModel.Description("技能类型")]
        [System.ComponentModel.Category("1.常用")]
        public ESkillType SkillType
        {
            get { return mSkillType; }
            set { mSkillType = value; }
        }

        ESkillAttackType mSkillAttackType = ESkillAttackType.Region;
        [CSUtility.Support.DataValueAttribute("SkillAttackType")]
        [System.ComponentModel.DisplayName("技能攻击类型")]
        [System.ComponentModel.Description("技能攻击类型")]
        [System.ComponentModel.Category("1.常用")]
        public ESkillAttackType SkillAttackType
        {
            get { return mSkillAttackType; }
            set { mSkillAttackType = value; }
        }

        string mAttackAction = "StayAttack";
        [CSUtility.Support.DataValueAttribute("AttackAction")]
        [System.ComponentModel.DisplayName("攻击动作")]
        [System.ComponentModel.Category("1.常用")]
        public string AttackAction
        {
            get { return mAttackAction; }
            set { mAttackAction = value; }
        }


        float mAttackRadius = 2.0f;
        [CSUtility.Support.DataValueAttribute("AttackRadius")]
        [System.ComponentModel.DisplayName("技能释放半径")]
        [System.ComponentModel.Category("1.常用")]
        public float AttackRadius
        {
            get { return mAttackRadius; }
            set { mAttackRadius = value; }
        }

        List<NotifyEffect> mAttackNotifyEffects = new List<NotifyEffect>();
        [CSUtility.Support.DataValueAttribute("AttackNotifyEffects")]
        [System.ComponentModel.DisplayName("攻击特效列表")]
        [System.ComponentModel.Category("1.常用")]
        public List<NotifyEffect> AttackNotifyEffects
        {
            get { return mAttackNotifyEffects; }
            set { mAttackNotifyEffects = value; }
        }

        UInt16 mThrowRoleId = 0;
        [CSUtility.Support.DataValueAttribute("ThrowRoleId")]
        [System.ComponentModel.DisplayName("召唤物RoleId")]
        [System.ComponentModel.Category("1.常用")]
        public UInt16 ThrowRoleId
        {
            get { return mThrowRoleId; }
            set { mThrowRoleId = value; }
        }


        float mHalfHeight = 0;
        [CSUtility.Support.DataValueAttribute("HalfHeight")]
        [System.ComponentModel.DisplayName("召唤物半高")]
        [System.ComponentModel.Category("1.常用")]
        public float HalfHeight
        {
            get { return mHalfHeight; }
            set { mHalfHeight = value; }
        }

        float mHeightOffset = 0;
        [CSUtility.Support.DataValueAttribute("HeightOffset")]
        [System.ComponentModel.DisplayName("召唤物高度偏移")]
        [System.ComponentModel.Category("1.常用")]
        public float HeightOffset
        {
            get { return mHeightOffset; }
            set { mHeightOffset = value; }
        }

        float mRadius = 1;
        [CSUtility.Support.DataValueAttribute("Radius")]
        [System.ComponentModel.DisplayName("召唤物半径")]
        [System.ComponentModel.Category("1.常用")]
        public float Radius
        {
            get { return mRadius; }
            set { mRadius = value; }
        }


        float mThrowRoleSpeed = 5;
        [CSUtility.Support.DataValueAttribute("ThrowRoleSpeed")]
        [System.ComponentModel.DisplayName("召唤物速度")]
        [System.ComponentModel.Category("1.常用")]
        public float ThrowRoleSpeed
        {
            get { return mThrowRoleSpeed; }
            set { mThrowRoleSpeed = value; }
        }

        Guid mAIGuid = CSUtility.Support.IHelper.GuidParse("31737174-370b-4c56-b13a-c5b69551ccd2");
        [CSUtility.Support.DataValueAttribute("AIGuid")]
        [System.ComponentModel.DisplayName("召唤物AI")]
        [System.ComponentModel.Category("1.常用")]
        public Guid AIGuid
        {
            get { return mAIGuid; }
            set { mAIGuid = value; }
        }

        float mLiveTime = 3;
        [CSUtility.Support.DataValueAttribute("LiveTime")]
        [System.ComponentModel.DisplayName("召唤物存活时间")]
        [System.ComponentModel.Category("1.常用")]
        public float LiveTime
        {
            get { return mLiveTime; }
            set { mLiveTime = value; }
        }

        List<SkillLevelParam> mSkillLevelDatas = new List<SkillLevelParam>();
        [CSUtility.Support.DataValueAttribute("SkillLevelDatas")]
        [System.ComponentModel.DisplayName("技能等级属性")]
        [System.ComponentModel.Description("技能等级属性")]
        [System.ComponentModel.Category("1.常用")]
        public List<SkillLevelParam> SkillLevelDatas
        {
            get { return mSkillLevelDatas; }
            set { mSkillLevelDatas = value; }
        }

        #endregion

        #region 技能基础属性
        //技能名称
        string mSkillName = "";
        [CSUtility.Support.DataValueAttribute("SkillName")]
        [System.ComponentModel.DisplayName("技能名称")]
        [System.ComponentModel.Description("技能名称")]
        [System.ComponentModel.Category("技能基础属性")]
        public string SkillName
        {
            get { return mSkillName; }
            set { mSkillName = value; }
        }

        FireSkillType mFireSkill;
        [CSUtility.Support.DataValueAttribute("FireSkill")]
        [System.ComponentModel.DisplayName("技能释放方法")]
        [System.ComponentModel.Description("技能释放方法")]
        [System.ComponentModel.Category("技能基础属性")]
        public FireSkillType FireSkill
        {
            get { return mFireSkill; }
            set { mFireSkill = value; }
        }

        //如果是吟唱技能则持续时间标示的为吟唱时间
        //如果是引导技能则持续时间标示的为引导时间
        int mFireSkillTime = 0;
        [CSUtility.Support.DataValueAttribute("FireSkillTime")]
        [System.ComponentModel.DisplayName("技能持续时间")]
        [System.ComponentModel.Description("技能持续时间")]
        [System.ComponentModel.Category("技能基础属性")]
        public int FireSkillTime
        {
            get { return mFireSkillTime; }
            set { mFireSkillTime = value; }
        }
        bool mSkillActionLoop = false;
        [CSUtility.Support.DataValueAttribute("SkillActionLoop")]
        [System.ComponentModel.DisplayName("技能动作是否循环")]
        [System.ComponentModel.Description("技能动作是否循环")]
        [System.ComponentModel.Category("技能基础属性")]
        public bool SkillActionLoop
        {
            get { return mSkillActionLoop; }
            set { mSkillActionLoop = value; }
        }

        UInt32 mControlLife = 360;
        [CSUtility.Support.DataValueAttribute("ControlLife")]
        [System.ComponentModel.DisplayName("技能操控时效")]
        [System.ComponentModel.Description("技能压栈后的存活时间")]
        [System.ComponentModel.Category("技能基础属性")]
        public UInt32 ControlLife
        {
            get { return mControlLife; }
            set { mControlLife = value; }
        }

        //技能图标
        Guid mSkillIcon = Guid.Empty;
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        [CSUtility.Support.DataValueAttribute("SkillIcon")]
        [System.ComponentModel.DisplayName("技能图标")]
        [System.ComponentModel.Description("技能图标")]
        [System.ComponentModel.Category("技能基础属性")]
        [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.UVAnim)]
        public Guid SkillIcon
        {
            get { return mSkillIcon; }
            set { mSkillIcon = value; }
        }

        Guid mSkillPressIcon = Guid.Empty;
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        [CSUtility.Support.DataValueAttribute("SkillPressIcon")]
        [System.ComponentModel.DisplayName("技能按下图标")]
        [System.ComponentModel.Description("技能按下图标")]
        [System.ComponentModel.Category("技能基础属性")]
        [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.UVAnim)]
        public Guid SkillPressIcon
        {
            get { return mSkillPressIcon; }
            set { mSkillPressIcon = value; }
        }

        Guid mSkillDisableIcon = Guid.Empty;
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        [CSUtility.Support.DataValueAttribute("SkillDisableIcon")]
        [System.ComponentModel.DisplayName("技能无法开启时图标")]
        [System.ComponentModel.Description("技能图标")]
        [System.ComponentModel.Category("技能基础属性")]
        [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.UVAnim)]
        public Guid SkillDisableIcon
        {
            get { return mSkillDisableIcon; }
            set { mSkillDisableIcon = value; }
        }


        ESkillSumType mSkillSumType = ESkillSumType.CommonSum;
        [CSUtility.Support.DataValueAttribute("SkillSumType")]
        [System.ComponentModel.DisplayName("技能召唤类型")]
        [System.ComponentModel.Description("技能召唤类型")]
        [System.ComponentModel.Category("技能基础属性")]
        public ESkillSumType SkillSumType
        {
            get { return mSkillSumType; }
            set { mSkillSumType = value; }
        }

        ESkillRangeType mSkillRangeType = ESkillRangeType.One;
        [CSUtility.Support.DataValueAttribute("SkillRangeType")]
        [System.ComponentModel.DisplayName("技能伤害范围类型")]
        [System.ComponentModel.Description("技能伤害范围类型")]
        [System.ComponentModel.Category("技能基础属性")]
        public ESkillRangeType SkillRangeType
        {
            get { return mSkillRangeType; }
            set { mSkillRangeType = value; }
        }

        Guid mSkillshotMaterial = CSUtility.Support.IFileConfig.DefaultDecalEditorRangeMeshTechniqueId;
        [CSUtility.Support.DataValueAttribute("SkillshotMaterial")]
        [System.ComponentModel.DisplayName("指向技能区域材质")]
        [System.ComponentModel.Category("技能基础属性")]
        [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.Material)]
        public Guid SkillshotMaterial
        {
            get { return mSkillshotMaterial; }
            set { mSkillshotMaterial = value; }
        }

        #endregion

        #region 技能等级属性

        public class SkillLevelParam
        {
            byte mSkillLevel = 0;
            [CSUtility.Support.DataValueAttribute("SkillLevel")]
            [System.ComponentModel.DisplayName("技能等级")]
            [System.ComponentModel.Category("技能等级属性")]
            public byte SkillLevel
            {
                get { return mSkillLevel; }
                set { mSkillLevel = value; }
            }

            string mSkillDescription = "";
            [CSUtility.Support.DataValueAttribute("SkillDescription")]
            [System.ComponentModel.DisplayName("技能描述")]
            [System.ComponentModel.Category("技能等级属性")]
            public string SkillDescription
            {
                get { return mSkillDescription; }
                set { mSkillDescription = value; }
            }

            float mCD = 0;
            [CSUtility.Support.DataValueAttribute("CD")]
            [System.ComponentModel.DisplayName("技能CD")]
            [System.ComponentModel.Category("技能等级属性")]
            public float CD
            {
                get { return mCD; }
                set { mCD = value; }
            }

            UInt16 mSkillConsumeMP = 0;
            [CSUtility.Support.DataValueAttribute("SkillConsumeMP")]
            [System.ComponentModel.DisplayName("技能消耗MP")]
            [System.ComponentModel.Category("技能等级属性")]
            public UInt16 SkillConsumeMP
            {
                get { return mSkillConsumeMP; }
                set { mSkillConsumeMP = value; }
            }

            sbyte mLevelupNeedRoleLevel;
            [CSUtility.Support.DataValueAttribute("LevelupNeedRoleLevel")]
            [System.ComponentModel.DisplayName("升级需求角色等级")]
            [System.ComponentModel.Category("技能等级属性")]
            public sbyte LevelupNeedRoleLevel
            {
                get { return mLevelupNeedRoleLevel; }
                set { mLevelupNeedRoleLevel = value; }
            }

            float mRoleDamageRate;
            [CSUtility.Support.DataValueAttribute("RoleDamageRate")]
            [System.ComponentModel.DisplayName("攻击力%")]
            [System.ComponentModel.Category("技能伤害属性")]
            public float RoleDamageRate
            {
                get { return mRoleDamageRate; }
                set { mRoleDamageRate = value; }
            }

            Int32 mDamageAdd;
            [CSUtility.Support.DataValueAttribute("DamageAdd")]
            [System.ComponentModel.DisplayName("附加伤害")]
            [System.ComponentModel.Category("技能伤害属性")]
            public Int32 DamageAdd
            {
                get { return mDamageAdd; }
                set { mDamageAdd = value; }
            }

            List<BuffParam> mBuffs = new List<BuffParam>();
            [CSUtility.Support.DataValueAttribute("Buffs")]
            [System.ComponentModel.DisplayName("Buff")]
            [System.ComponentModel.Category("技能伤害属性")]
            public List<BuffParam> Buffs
            {
                get { return mBuffs; }
                set { mBuffs = value; }
            }
        }
        #endregion

        #region 技能动作属性

        EOffsetType mOffsetType = EOffsetType.None;
        [CSUtility.Support.DataValueAttribute("OffsetType")]
        [System.ComponentModel.DisplayName("位移类型")]
        [System.ComponentModel.Description("None:无位移，OffsetSelf:自身位移，OffsetTarget目标位移")]
        [System.ComponentModel.Category("技能动作属性")]
        public EOffsetType OffsetType
        {
            get { return mOffsetType; }
            set { mOffsetType = value; }
        }

        string mChannelAction = "";
        [CSUtility.Support.DataValueAttribute("ChannelAction")]
        [System.ComponentModel.DisplayName("吟唱动作")]
        [System.ComponentModel.Category("技能动作属性")]
        public string ChannelAction
        {
            get { return mChannelAction; }
            set { mChannelAction = value; }
        }



        UInt16 mAttackSoundId = UInt16.MaxValue;
        [CSUtility.Support.DataValueAttribute("AttackSoundId")]
        [System.ComponentModel.DisplayName("攻击音效Id")]
        [System.ComponentModel.Category("技能动作属性")]
        public UInt16 AttackSoundId
        {
            get { return mAttackSoundId; }
            set { mAttackSoundId = value; }
        }

        Int32 mRotationSkillTime = 0;
        [CSUtility.Support.DataValueAttribute("RotationSkillTime")]
        [System.ComponentModel.DisplayName("循环动作播放时间")]
        [System.ComponentModel.Description("循环动作播放时间")]
        [System.ComponentModel.Category("技能动作属性")]
        public Int32 RotationSkillTime
        {
            get { return mRotationSkillTime; }
            set { mRotationSkillTime = value; }
        }

        string mLoopAction = string.Empty;
        [CSUtility.Support.DataValueAttribute("LoopAction")]
        [System.ComponentModel.DisplayName("循环动作")]
        [System.ComponentModel.Category("技能动作属性")]
        public string LoopAction
        {
            get { return mLoopAction; }
            set { mLoopAction = value; }
        }

        string mEndAction = string.Empty;
        [CSUtility.Support.DataValueAttribute("EndAction")]
        [System.ComponentModel.DisplayName("结束动作")]
        [System.ComponentModel.Category("技能动作属性")]
        public string EndAction
        {
            get { return mEndAction; }
            set { mEndAction = value; }
        }

        bool mForce2BeAttack = false;
        [CSUtility.Support.DataValueAttribute("Force2BeAttack")]
        [System.ComponentModel.DisplayName("是否强制做被攻击动作")]
        [System.ComponentModel.Category("技能动作属性")]
        public bool Force2BeAttack
        {
            get { return mForce2BeAttack; }
            set { mForce2BeAttack = value; }
        }

        string mBeAttackAction = "BeAttack";
        [CSUtility.Support.DataValueAttribute("BeAttackAction")]
        [System.ComponentModel.DisplayName("被攻击动作")]
        [System.ComponentModel.Category("技能动作属性")]
        public string BeAttackAction
        {
            get { return mBeAttackAction; }
            set { mBeAttackAction = value; }
        }

        string mLoopBeAttackAction = string.Empty;
        [CSUtility.Support.DataValueAttribute("LoopBeAttackAction")]
        [System.ComponentModel.DisplayName("被攻击动作循环")]
        [System.ComponentModel.Category("技能动作属性")]
        public string LoopBeAttackAction
        {
            get { return mLoopBeAttackAction; }
            set { mLoopBeAttackAction = value; }
        }

        string mEndBeAttackAction = string.Empty;
        [CSUtility.Support.DataValueAttribute("EndBeAttackAction")]
        [System.ComponentModel.DisplayName("被攻击动作结束")]
        [System.ComponentModel.Category("技能动作属性")]
        public string EndBeAttackAction
        {
            get { return mEndBeAttackAction; }
            set { mEndBeAttackAction = value; }
        }

        bool mBeAttackLoop = false;
        [CSUtility.Support.DataValueAttribute("BeAttackLoop")]
        [System.ComponentModel.DisplayName("被攻击动作是否循环")]
        [System.ComponentModel.Category("技能动作属性")]
        public bool BeAttackLoop
        {
            get { return mBeAttackLoop; }
            set { mBeAttackLoop = value; }
        }

        float mBeAttackPlayRate = 1.0f;
        [CSUtility.Support.DataValueAttribute("BeAttackPlayRate")]
        [System.ComponentModel.DisplayName("被攻击动作速率")]
        [System.ComponentModel.Category("技能动作属性")]
        public float BeAttackPlayRate
        {
            get { return mBeAttackPlayRate; }
            set
            {
                mBeAttackPlayRate = value;
            }
        }

        Int64 mChannelDuration = 1000 * 3;
        [CSUtility.Support.DataValueAttribute("ChannelDuration")]
        [System.ComponentModel.DisplayName("吟唱时间")]
        [System.ComponentModel.Category("技能动作属性")]
        public Int64 ChannelDuration
        {
            get { return mChannelDuration; }
            set { mChannelDuration = value; }
        }

        bool mCanBeBreak = false;
        [CSUtility.Support.DataValueAttribute("CanBeBreak")]
        [System.ComponentModel.DisplayName("技能施法是否可以被打断")]
        [System.ComponentModel.Category("技能动作属性")]
        public bool CanBeBreak
        {
            get { return mCanBeBreak; }
            set { mCanBeBreak = value; }
        }

        float mJumpUpHeight = 1;
        [CSUtility.Support.DataValueAttribute("JumpUpHeight")]
        [System.ComponentModel.DisplayName("跳跃高度")]
        [System.ComponentModel.Category("技能动作属性")]
        public float JumpUpHeight
        {
            get { return mJumpUpHeight; }
            set { mJumpUpHeight = value; }
        }

        Int32 mJumpUpTime = 595;
        [CSUtility.Support.DataValueAttribute("JumpUpTime")]
        [System.ComponentModel.DisplayName("跳跃时间")]
        [System.ComponentModel.Category("技能动作属性")]
        public Int32 JumpUpTime
        {
            get { return mJumpUpTime; }
            set { mJumpUpTime = value; }
        }

        Int32 mMoveTime = 595;
        [CSUtility.Support.DataValueAttribute("MoveTime")]
        [System.ComponentModel.DisplayName("位移时间（毫秒）")]
        [System.ComponentModel.Category("技能动作属性")]
        public Int32 MoveTime
        {
            get { return mMoveTime; }
            set { mMoveTime = value; }
        }

        #endregion

        #region 技能特效属性
        [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        public class Effect : CSUtility.Support.Copyable
        {
            Guid mEffectId = Guid.Empty;
            [System.ComponentModel.DisplayName("EffectId")]
            [System.ComponentModel.Category("粒子")]
            [CSUtility.Support.DataValueAttribute("EffectId")]
            [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.Effect)]
            public Guid EffectId
            {
                get { return mEffectId; }
                set
                {
                    mEffectId = value;
                }
            }

            public long mDuration = long.MaxValue;
            [System.ComponentModel.DisplayName("特效存活时间")]
            [System.ComponentModel.Category("粒子")]
            [CSUtility.Support.DataValueAttribute("Duration")]
            public long Duration
            {
                get { return mDuration; }
                set
                {
                    mDuration = value;
                }
            }

            SlimDX.Vector3 mPos = SlimDX.Vector3.Zero;
            [System.ComponentModel.DisplayName("位置")]
            [CSUtility.Support.DataValueAttribute("Pos")]
            [System.ComponentModel.Category("位置变换")]
            public SlimDX.Vector3 Pos
            {
                get { return mPos; }
                set
                {
                    mPos = value;
                }
            }

            float mScale = 1;
            [System.ComponentModel.DisplayName("缩放")]
            [CSUtility.Support.DataValueAttribute("Scale")]
            [System.ComponentModel.Category("位置变换")]
            public float Scale
            {
                get { return mScale; }
                set
                {
                    mScale = value;
                }
            }

            SlimDX.Vector3 mRotate = SlimDX.Vector3.Zero;
            [System.ComponentModel.DisplayName("旋转（欧拉角）")]
            [CSUtility.Support.DataValueAttribute("Rotate")]
            [System.ComponentModel.Category("位置变换")]
            public SlimDX.Vector3 Rotate
            {
                get { return mRotate; }
                set
                {
                    mRotate = value;
                }
            }

            bool mInheritRotate = true;
            [System.ComponentModel.DisplayName("继承Socket的旋转")]
            [System.ComponentModel.Category("粒子")]
            [CSUtility.Support.DataValueAttribute("InheritRotate")]
            public bool InheritRotate
            {
                get { return mInheritRotate; }
                set
                {
                    mInheritRotate = value;
                }
            }

            bool mInheritRotateWhenBorn = false;
            [System.ComponentModel.DisplayName("发射时继承Socket的旋转")]
            [System.ComponentModel.Category("粒子")]
            [CSUtility.Support.DataValueAttribute("InheritRotateWhenBorn")]
            public bool InheritRotateWhenBorn
            {
                get { return mInheritRotateWhenBorn; }
                set
                {
                    mInheritRotateWhenBorn = value;
                }
            }

            EEffectChainType mChainType;
            [System.ComponentModel.DisplayName("线条类型")]
            [System.ComponentModel.Category("链接特效")]
            [CSUtility.Support.DataValueAttribute("ChainType")]
            public EEffectChainType ChainType
            {
                get { return mChainType; }
                set
                {
                    mChainType = value;
                }
            }

            Guid mChainId = Guid.Empty;
            [System.ComponentModel.DisplayName("刀光MeshId")]
            [System.ComponentModel.Category("链接特效")]
            [CSUtility.Support.DataValueAttribute("ChainId")]
            public Guid ChainId
            {
                get { return mChainId; }
                set
                {
                    mChainId = value;
                }
            }

            int mTrailPoolSize = 100;
            [System.ComponentModel.DisplayName("预分配大小")]
            [System.ComponentModel.Category("链接特效")]
            [CSUtility.Support.DataValueAttribute("TrailPoolSize")]
            public int TrailPoolSize
            {
                get { return mTrailPoolSize; }
                set
                {
                    mTrailPoolSize = value;
                }
            }

            float mTrailWidth = 0.5f;
            [System.ComponentModel.DisplayName("刀光宽度")]
            [System.ComponentModel.Category("链接特效")]
            [CSUtility.Support.DataValueAttribute("TrailWidth")]
            public float TrailWidth
            {
                get { return mTrailWidth; }
                set
                {
                    mTrailWidth = value;
                }
            }

            long mChainDuration = long.MaxValue;
            [System.ComponentModel.DisplayName("Chain存活时间")]
            [System.ComponentModel.Category("链接特效")]
            [CSUtility.Support.DataValueAttribute("ChainDuration")]
            public long ChainDuration
            {
                get { return mChainDuration; }
                set
                {
                    mChainDuration = value;
                }
            }

            long mChainSegmentDuration = long.MaxValue;
            [System.ComponentModel.DisplayName("每个Segment的存活时间")]
            [System.ComponentModel.Category("链接特效")]
            [CSUtility.Support.DataValueAttribute("ChainSegmentDuration")]
            public long ChainSegmentDuration
            {
                get { return mChainSegmentDuration; }
                set
                {
                    mChainSegmentDuration = value;
                }
            }

            float mSegmentMinDistance = 0.1f;
            [System.ComponentModel.DisplayName("每个Segment的最小间隔")]
            [System.ComponentModel.Category("链接特效")]
            [CSUtility.Support.DataValueAttribute("SegmentMinDistance")]
            public float SegmentMinDistance
            {
                get { return mSegmentMinDistance; }
                set
                {
                    mSegmentMinDistance = value;
                }
            }

            float mTopInLinePercent = 0.3f;
            [System.ComponentModel.DisplayName("抛物线顶点在线段中的百分比")]
            [System.ComponentModel.Category("链接特效_抛物线")]
            [CSUtility.Support.DataValueAttribute("TopInLinePercent")]
            public float TopInLinePercent
            {
                get { return mTopInLinePercent; }
                set
                {
                    mTopInLinePercent = value;
                }
            }

            float mTopHeight = 3.0f;
            [System.ComponentModel.DisplayName("抛物线高度差")]
            [System.ComponentModel.Category("链接特效_抛物线")]
            [CSUtility.Support.DataValueAttribute("TopHeight")]
            public float TopHeight
            {
                get { return mTopHeight; }
                set
                {
                    mTopHeight = value;
                }
            }

            float mTopWidth = 0f;
            [System.ComponentModel.DisplayName("抛物线宽度差")]
            [System.ComponentModel.Category("链接特效_抛物线")]
            [CSUtility.Support.DataValueAttribute("TopWidth")]
            public float TopWidth
            {
                get { return mTopWidth; }
                set
                {
                    mTopWidth = value;
                }
            }

            float mRanTopInLineMin = -1.0f;
            [System.ComponentModel.DisplayName("抛物线顶点在线段中的百分比最小值（小于0不随机）")]
            [System.ComponentModel.Category("链接特效_抛物线")]
            [CSUtility.Support.DataValueAttribute("RanTopInLineMin")]
            public float RanTopInLineMin
            {
                get { return mRanTopInLineMin; }
                set
                {
                    mRanTopInLineMin = value;
                }
            }

            float mRanTopHeightMin = float.MaxValue;
            [System.ComponentModel.DisplayName("抛物线高度差随机最小值（小于0不随机）")]
            [System.ComponentModel.Category("链接特效_抛物线")]
            [CSUtility.Support.DataValueAttribute("RanTopHeightMin")]
            public float RanTopHeightMin
            {
                get { return mRanTopHeightMin; }
                set
                {
                    mRanTopHeightMin = value;
                }
            }

            float mRanTopWidthMin = float.MaxValue;
            [System.ComponentModel.DisplayName("抛物线宽度差随机最小值（小于0不随机）")]
            [System.ComponentModel.Category("链接特效_抛物线")]
            [CSUtility.Support.DataValueAttribute("RanTopWidthMin")]
            public float RanTopWidthMin
            {
                get { return mRanTopWidthMin; }
                set
                {
                    mRanTopWidthMin = value;
                }
            }
        }
        [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        public class SocketEffect : CSUtility.Support.Copyable
        {
            public string mSocketName;
            [System.ComponentModel.DisplayName("Socket名")]
            [CSUtility.Support.DataValueAttribute("SocketName")]
            public string SocketName
            {
                get { return mSocketName; }
                set
                {
                    mSocketName = value;
                }
            }

            public List<Effect> mEffects = new List<Effect>();
            [System.ComponentModel.DisplayName("特效列表")]
            [CSUtility.Support.DataValueAttribute("Effects")]
            public List<Effect> Effects
            {
                get { return mEffects; }
                set
                {
                    mEffects = value;
                }
            }

        }
        [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        public class NotifyEffect : CSUtility.Support.Copyable
        {
            public string mNotifyName;
            [System.ComponentModel.DisplayName("Notify名")]
            [CSUtility.Support.DataValueAttribute("NotifyName")]
            public string NotifyName
            {
                get { return mNotifyName; }
                set
                {
                    mNotifyName = value;
                }
            }

            public List<SocketEffect> mSocketEffects = new List<SocketEffect>();
            [System.ComponentModel.DisplayName("Socket列表")]
            [CSUtility.Support.DataValueAttribute("SocketEffects")]
            public List<SocketEffect> SocketEffects
            {
                get { return mSocketEffects; }
                set
                {
                    mSocketEffects = value;
                }
            }

            public string mShakeName;
            [System.ComponentModel.DisplayName("震屏名称")]
            [CSUtility.Support.DataValueAttribute("ShakeName")]
            public string ShakeName
            {
                get { return mShakeName; }
                set
                {
                    mShakeName = value;
                }
            }
        }

        List<NotifyEffect> mChannelNotifyEffects = new List<NotifyEffect>();
        [CSUtility.Support.DataValueAttribute("ChannelNotifyEffects")]
        [System.ComponentModel.DisplayName("吟唱特效列表")]
        [System.ComponentModel.Category("技能特效属性")]
        public List<NotifyEffect> ChannelNotifyEffects
        {
            get { return mChannelNotifyEffects; }
            set { mChannelNotifyEffects = value; }
        }

        bool mAttackEffectRepeatAdd = false;
        [CSUtility.Support.DataValueAttribute("AttackEffectRepeatAdd")]
        [System.ComponentModel.DisplayName("一次技能施法是否可以重复添加相同的攻击特效")]
        [System.ComponentModel.Category("技能动作属性")]
        public bool AttackEffectRepeatAdd
        {
            get { return mAttackEffectRepeatAdd; }
            set { mAttackEffectRepeatAdd = value; }
        }

        public NotifyEffect GetCurrentAttackNotifyEffect(string name)
        {
            foreach (var effect in AttackNotifyEffects)
            {
                if (effect.NotifyName == name)
                {
                    return effect;
                }
            }
            return null;
        }

        List<NotifyEffect> mBeAttackNotifyEffects = new List<NotifyEffect>();
        [CSUtility.Support.DataValueAttribute("BeAttackNotifyEffects")]
        [System.ComponentModel.DisplayName("被动攻击特效列表")]
        [System.ComponentModel.Category("技能特效属性")]
        public List<NotifyEffect> BeAttackNotifyEffects
        {
            get { return mBeAttackNotifyEffects; }
            set { mBeAttackNotifyEffects = value; }
        }

        string mChainNotifyPointName = "";
        [CSUtility.Support.DataValueAttribute("ChainNotifyPointName")]
        [System.ComponentModel.DisplayName("链接法术的NotifyPoint名称")]
        [System.ComponentModel.Description("叫此名字的NotifyPoint处理伤害和链接法术特效，名字为空的话则当做普通NotifyPoint只处理伤害")]
        [System.ComponentModel.Category("技能特效属性")]
        public string ChainNotifyPointName
        {
            get { return mChainNotifyPointName; }
            set { mChainNotifyPointName = value; }
        }

        string mChainSocketName = "HP_FixSocket_gethit_effect";
        [CSUtility.Support.DataValueAttribute("ChainSocketName")]
        [System.ComponentModel.DisplayName("Chain目标Socket名")]
        [System.ComponentModel.Category("技能特效属性")]
        public string ChainSocketName
        {
            get { return mChainSocketName; }
            set { mChainSocketName = value; }
        }

        string mRemoveSocketName = "";
        [CSUtility.Support.DataValueAttribute("RemoveSocketName")]
        [System.ComponentModel.DisplayName("退出攻击状态要移除的特效")]
        [System.ComponentModel.Category("技能特效属性")]
        public string RemoveSocketName
        {
            get { return mRemoveSocketName; }
            set { mRemoveSocketName = value; }
        }

        [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        public class NotifyShake : CSUtility.Support.Copyable
        {
            public string mNotifyName;
            [System.ComponentModel.DisplayName("Notify名")]
            [CSUtility.Support.DataValueAttribute("NotifyName")]
            public string NotifyName
            {
                get { return mNotifyName; }
                set
                {
                    mNotifyName = value;
                }
            }

            public string mShakeName;
            [System.ComponentModel.DisplayName("震屏名称")]
            [CSUtility.Support.DataValueAttribute("ShakeName")]
            public string ShakeName
            {
                get { return mShakeName; }
                set
                {
                    mShakeName = value;
                }
            }
        }
        List<NotifyShake> mNotifyShakes = new List<NotifyShake>();
        [CSUtility.Support.DataValueAttribute("NotifyShakes")]
        [System.ComponentModel.DisplayName("震屏特效列表")]
        [System.ComponentModel.Category("技能特效属性")]
        public List<NotifyShake> NotifyShakes
        {
            get { return mNotifyShakes; }
            set { mNotifyShakes = value; }
        }
        #endregion

        #region 技能释放属性
        //技能目标
        SkillTarget mSkillTarget;
        [CSUtility.Support.DataValueAttribute("SkillTarget")]
        [System.ComponentModel.DisplayName("技能目标")]
        [System.ComponentModel.Description("技能目标")]
        [System.ComponentModel.Category("技能释放属性")]
        public SkillTarget SkillTarget
        {
            get { return mSkillTarget; }
            set { mSkillTarget = value; }
        }

        //技能目标个数
        int mSkillTargetNumber = 1;
        [CSUtility.Support.DataValueAttribute("SkillTargetNumber")]
        [System.ComponentModel.DisplayName("技能目标个数")]
        [System.ComponentModel.Description("技能目标个数")]
        [System.ComponentModel.Category("技能释放属性")]
        public int SkillTargetNumber
        {
            get { return mSkillTargetNumber; }
            set { mSkillTargetNumber = value; }
        }

        //bool mFixedCast = false;
        //[CSUtility.Support.DataValueAttribute("FixedCast")]
        //[System.ComponentModel.DisplayName("是否原地释放")]
        //[System.ComponentModel.Category("技能释放属性")]
        //public bool FixedCast
        //{
        //    get { return mFixedCast; }
        //    set
        //    {
        //        mFixedCast = value;
        //    }
        //}

        //操作
        ESkillOperationType mSkillOperation;
        [CSUtility.Support.DataValueAttribute("SkillOperation")]
        [System.ComponentModel.DisplayName("技能操作")]
        [System.ComponentModel.Description("技能操作")]
        [System.ComponentModel.Category("技能释放属性")]
        public ESkillOperationType SkillOperation
        {
            get { return mSkillOperation; }
            set { mSkillOperation = value; }
        }

        bool mReverseShoot = false;
        [CSUtility.Support.DataValueAttribute("ReverseShoot")]
        [System.ComponentModel.DisplayName("是否反方向释放")]
        [System.ComponentModel.Category("技能释放属性")]
        public bool ReverseShoot
        {
            get { return mReverseShoot; }
            set
            {
                mReverseShoot = value;
            }
        }

        float mRandomRadius = 12f;
        [CSUtility.Support.DataValueAttribute("RandomRadius")]
        [System.ComponentModel.DisplayName("随机半径")]
        [System.ComponentModel.Category("技能释放属性")]
        public float RandomRadius
        {
            get { return mRandomRadius; }
            set
            {
                mRandomRadius = value;
            }
        }

        //目标状态
        TargetState mTargetState;
        [CSUtility.Support.DataValueAttribute("TargetState")]
        [System.ComponentModel.DisplayName("释放需求目标状态")]
        [System.ComponentModel.Description("释放需求目标状态")]
        [System.ComponentModel.Category("技能释放属性")]
        public TargetState TargetState
        {
            get { return mTargetState; }
            set { mTargetState = value; }
        }

        //技能触发几率
        UInt32 mSkillSpellWeight;
        [CSUtility.Support.DataValueAttribute("SkillProbability")]
        [System.ComponentModel.DisplayName("技能触发权值")]
        [System.ComponentModel.Description("技能触发权值")]
        [System.ComponentModel.Category("技能释放属性")]
        public UInt32 SkillSpellWeight
        {
            get { return mSkillSpellWeight; }
            set { mSkillSpellWeight = value; }
        }

        float mC2SDistanceError = 5.0f;
        [CSUtility.Support.DataValueAttribute("C2SDistanceError")]
        [System.ComponentModel.DisplayName("允许误差")]
        [System.ComponentModel.Description("服务器端在处理碰撞时候允许的误差")]
        [System.ComponentModel.Category("技能释放属性")]
        public float C2SDistanceError
        {
            get { return mC2SDistanceError; }
            set { mC2SDistanceError = value; }
        }

        #endregion

        #region 技能召唤属性
        ESkillSummonType mSummonSkillType = ESkillSummonType.Ball;
        [CSUtility.Support.DataValueAttribute("SummonSkillType")]
        [System.ComponentModel.DisplayName("技能召唤物类型")]
        [System.ComponentModel.Category("召唤物属性")]
        public ESkillSummonType SummonSkillType
        {
            get { return mSummonSkillType; }
            set { mSummonSkillType = value; }
        }

        bool mSummonBindRole = false;
        [CSUtility.Support.DataValueAttribute("SummonBindRole")]
        [System.ComponentModel.DisplayName("召唤物是否绑定角色身上")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool SummonBindRole
        {
            get { return mSummonBindRole; }
            set { mSummonBindRole = value; }
        }

        bool mSummonLockOnRole = true;
        [CSUtility.Support.DataValueAttribute("SummonLockOnRole")]
        [System.ComponentModel.DisplayName("召唤物是否锁定角色")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool SummonLockOnRole
        {
            get { return mSummonLockOnRole; }
            set { mSummonLockOnRole = value; }
        }

        UInt16 mThrowRoleCount = 1;
        [CSUtility.Support.DataValueAttribute("ThrowRoleCount")]
        [System.ComponentModel.DisplayName("召唤个数")]
        [System.ComponentModel.Category("召唤物属性")]
        public UInt16 ThrowRoleCount
        {
            get { return mThrowRoleCount; }
            set { mThrowRoleCount = value; }
        }

        float mThrowRoleRate = 1;
        [CSUtility.Support.DataValueAttribute("ThrowRoleRate")]
        [System.ComponentModel.DisplayName("召唤几率")]
        [System.ComponentModel.Category("召唤物属性")]
        public float ThrowRoleRate
        {
            get { return mThrowRoleRate; }
            set { mThrowRoleRate = value; }
        }

        CSUtility.Component.EActorGameType mGameType = CSUtility.Component.EActorGameType.EffectNpc;
        [CSUtility.Support.DataValueAttribute("GameType")]
        [System.ComponentModel.DisplayName("召唤物类型")]
        [System.ComponentModel.Category("召唤物属性")]
        public CSUtility.Component.EActorGameType GameType
        {
            get { return mGameType; }
            set { mGameType = value; }
        }

        EOffsetType mSummonOffsetType = EOffsetType.None;
        [CSUtility.Support.DataValueAttribute("SummonOffsetType")]
        [System.ComponentModel.DisplayName("召唤物位移类型")]
        [System.ComponentModel.Description("None:无位移，OffsetSelf:自身位移，OffsetTarget目标位移")]
        [System.ComponentModel.Category("召唤物位移属性")]
        public EOffsetType SummonOffsetType
        {
            get { return mSummonOffsetType; }
            set { mSummonOffsetType = value; }
        }

        float mSummonOffsetSpeed = 1;
        [CSUtility.Support.DataValueAttribute("SummonOffsetSpeed")]
        [System.ComponentModel.DisplayName("召唤物位移速度")]
        [System.ComponentModel.Category("召唤物位移属性")]
        public float SummonOffsetSpeed
        {
            get { return mSummonOffsetSpeed; }
            set { mSummonOffsetSpeed = value; }
        }

        EEmissionType mEmissionType = EEmissionType.Distribute;
        [CSUtility.Support.DataValueAttribute("EmissionType")]
        [System.ComponentModel.DisplayName("发射类型")]
        [System.ComponentModel.Category("召唤物属性")]
        public EEmissionType EmissionType
        {
            get { return mEmissionType; }
            set { mEmissionType = value; }
        }

        UInt16 mEmissionAngle = 90;
        [CSUtility.Support.DataValueAttribute("EmissionAngle")]
        [System.ComponentModel.DisplayName("散射范围(角度)")]
        [System.ComponentModel.Category("召唤物属性")]
        public UInt16 EmissionAngle
        {
            get { return mEmissionAngle; }
            set { mEmissionAngle = value; }
        }

        EEmissionPathType mEmissionPathType = EEmissionPathType.Line;
        [CSUtility.Support.DataValueAttribute("EmissionPathType")]
        [System.ComponentModel.DisplayName("发射路径")]
        [System.ComponentModel.Category("召唤物属性")]
        public EEmissionPathType EmissionPathType
        {
            get { return mEmissionPathType; }
            set { mEmissionPathType = value; }
        }

        string mFireSocketName = "";
        [CSUtility.Support.DataValueAttribute("FireSocketName")]
        [System.ComponentModel.DisplayName("发射起始SOCKET名")]
        [System.ComponentModel.Category("召唤物属性")]
        public string FireSocketName
        {
            get { return mFireSocketName; }
            set { mFireSocketName = value; }
        }

        float mThrowOffset = 0.5F;
        [CSUtility.Support.DataValueAttribute("ThrowOffset")]
        [System.ComponentModel.DisplayName("召唤位置偏移")]
        [System.ComponentModel.Category("召唤物属性")]
        public float ThrowOffset
        {
            get { return mThrowOffset; }
            set { mThrowOffset = value; }
        }

        float mLevelOffset = 0F;
        [CSUtility.Support.DataValueAttribute("LevelOffset")]
        [System.ComponentModel.DisplayName("召唤水平位置偏移")]
        [System.ComponentModel.Category("召唤物属性")]
        public float LevelOffset
        {
            get { return mLevelOffset; }
            set { mLevelOffset = value; }
        }


        float mThrowRoleDistance = 8;
        [CSUtility.Support.DataValueAttribute("ThrowRoleDistance")]
        [System.ComponentModel.DisplayName("飞行最远距离")]
        [System.ComponentModel.Description("飞行最远距离，和ThrowRoleSpeed互斥，用来严格控制飞行物的目标点。目前LineTurn使用此属性")]
        [System.ComponentModel.Category("召唤物属性")]
        public float ThrowRoleDistance
        {
            get { return mThrowRoleDistance; }
            set { mThrowRoleDistance = value; }
        }

        //Guid mThrowRoleAIGuid = CSCommon.DN2.IHelper.GuidParse("31737174-370b-4c56-b13a-c5b69551ccd2");
        //[CSUtility.Support.DataValueAttribute("ThrowRoleAIGuid")]
        //[System.ComponentModel.DisplayName("AI")]
        //[System.ComponentModel.Category("召唤物属性")]
        //[EditorCommon.Assist.Editor_AISet]
        //public Guid ThrowRoleAIGuid
        //{
        //    get { return mThrowRoleAIGuid; }
        //    set { mThrowRoleAIGuid = value; }
        //}



        float mDeathTime = 0;
        [CSUtility.Support.DataValueAttribute("DeathTime")]
        [System.ComponentModel.DisplayName("死亡时间")]
        [System.ComponentModel.Category("召唤物属性")]
        public float DeathTime
        {
            get { return mDeathTime; }
            set { mDeathTime = value; }
        }

        bool mDeathOnDamage = true;
        [CSUtility.Support.DataValueAttribute("DeathOnDamage")]
        [System.ComponentModel.DisplayName("伤害目标后立即死亡")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool DeathOnDamage
        {
            get { return mDeathOnDamage; }
            set { mDeathOnDamage = value; }
        }


        Int64 mDamageDelayTime = 0;
        [CSUtility.Support.DataValueAttribute("DamageDelayTime")]
        [System.ComponentModel.DisplayName("第一次伤害的时间延迟")]
        [System.ComponentModel.Category("召唤物伤害属性")]
        public Int64 DamageDelayTime
        {
            get { return mDamageDelayTime; }
            set { mDamageDelayTime = value; }
        }

        Int64 mDamageClosedRoleInterval = 50;
        [CSUtility.Support.DataValueAttribute("DamageClosedRoleInterval")]
        [System.ComponentModel.DisplayName("判定伤害的时间间隔")]
        [System.ComponentModel.Category("召唤物伤害属性")]
        public Int64 DamageClosedRoleInterval
        {
            get { return mDamageClosedRoleInterval; }
            set { mDamageClosedRoleInterval = value; }
        }

        bool mNotifyDamageRepeat = false;
        [CSUtility.Support.DataValueAttribute("NotifyDamageRepeat")]
        [System.ComponentModel.DisplayName("一个hurtNotfy是否造成多次伤害")]
        [System.ComponentModel.Category("召唤物伤害属性")]
        public bool NotifyDamageRepeat
        {
            get { return mNotifyDamageRepeat; }
            set { mNotifyDamageRepeat = value; }
        }

        int mSameRoleDamageCount = 1;
        [CSUtility.Support.DataValueAttribute("SameRoleDamageCount")]
        [System.ComponentModel.DisplayName("同一个角色攻击次数")]
        [System.ComponentModel.Category("召唤物伤害属性")]
        public int SameRoleDamageCount
        {
            get { return mSameRoleDamageCount; }
            set { mSameRoleDamageCount = value; }
        }

        int mDamageCalculationCount = 1;
        [CSUtility.Support.DataValueAttribute("DamageCalculationCount")]
        [System.ComponentModel.DisplayName("攻击判定的次数")]
        [System.ComponentModel.Category("召唤物伤害属性")]
        public int DamageCalculationCount
        {
            get { return mDamageCalculationCount; }
            set { mDamageCalculationCount = value; }
        }

        int mMaxDamageTargetNumber = 1;
        [CSUtility.Support.DataValueAttribute("MaxDamageTargetNumber")]
        [System.ComponentModel.DisplayName("最多能在一次伤害检测中伤害角色数量")]
        [System.ComponentModel.Category("召唤物伤害属性")]
        public int MaxDamageTargetNumber
        {
            get { return mMaxDamageTargetNumber; }
            set { mMaxDamageTargetNumber = value; }
        }

        //         float mDeathOnDamageRate =0;
        //         [CSUtility.Support.DataValueAttribute("DeathOnDamageRate")]
        //         [System.ComponentModel.DisplayName("伤害目标后死亡几率")]
        //         [System.ComponentModel.Category("召唤物属性")]
        //         public float DeathOnDamageRate
        //         {
        //             get { return mDeathOnDamageRate; }
        //             set { mDeathOnDamageRate = value; }
        //         }

        bool mSummonRoleHasHatred = false;
        [CSUtility.Support.DataValueAttribute("SummonRoleHasHatred")]
        [System.ComponentModel.DisplayName("召唤出来的npc是否进仇恨列表")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool SummonRoleHasHatred
        {
            get { return mSummonRoleHasHatred; }
            set { mSummonRoleHasHatred = value; }
        }

        bool mSecondFireRemoveSum = true;
        [CSUtility.Support.DataValueAttribute("SecondFireRemoveSum")]
        [System.ComponentModel.DisplayName("二次施法移除召唤物")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool SecondFireRemoveSum
        {
            get { return mSecondFireRemoveSum; }
            set { mSecondFireRemoveSum = value; }
        }

        bool mImmediate2Death = true;
        [CSUtility.Support.DataValueAttribute("Immediate2Death")]
        [System.ComponentModel.DisplayName("死亡时立刻移除地图")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool Immediate2Death
        {
            get { return mImmediate2Death; }
            set { mImmediate2Death = value; }
        }

        bool mIsUpdatePos2Client = false;
        [CSUtility.Support.DataValueAttribute("IsUpdatePos2Client")]
        [System.ComponentModel.DisplayName("是否向Client即时同步")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool IsUpdatePos2Client
        {
            get { return mIsUpdatePos2Client; }
            set { mIsUpdatePos2Client = value; }
        }

        bool mIsCollideWithScene = true;
        [CSUtility.Support.DataValueAttribute("IsCollideWithScene")]
        [System.ComponentModel.DisplayName("是否与场景碰撞")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool IsCollideWithScene
        {
            get { return mIsCollideWithScene; }
            set { mIsCollideWithScene = value; }
        }



        float mLenghtLocX = 1;
        [CSUtility.Support.DataValueAttribute("LenghtLocX")]
        [System.ComponentModel.DisplayName("召唤长方体的宽度")]
        [System.ComponentModel.Category("召唤物属性")]
        public float LenghtLocX
        {
            get { return mLenghtLocX; }
            set { mLenghtLocX = value; }
        }

        float mLenghtLocY = 1;
        [CSUtility.Support.DataValueAttribute("LenghtLocY")]
        [System.ComponentModel.DisplayName("召唤长方体的高度")]
        [System.ComponentModel.Category("召唤物属性")]
        public float LenghtLocY
        {
            get { return mLenghtLocY; }
            set { mLenghtLocY = value; }
        }

        float mLenghtLocZ = 1;
        [CSUtility.Support.DataValueAttribute("LenghtLocZ")]
        [System.ComponentModel.DisplayName("召唤长方体的长度")]
        [System.ComponentModel.Category("召唤物属性")]
        public float LenghtLocZ
        {
            get { return mLenghtLocZ; }
            set { mLenghtLocZ = value; }
        }



        bool mGravity = false;
        [CSUtility.Support.DataValueAttribute("Gravity")]
        [System.ComponentModel.DisplayName("召唤物是否受重力")]
        [System.ComponentModel.Category("召唤物属性")]
        public bool Gravity
        {
            get { return mGravity; }
            set { mGravity = value; }
        }


        float mAIRotationAngle = 0.0F;
        [CSUtility.Support.DataValueAttribute("AIRotationAngle")]
        [System.ComponentModel.DisplayName("召唤物一次旋转的角度(是角度不是弧度)")]
        [System.ComponentModel.Category("召唤物属性")]
        [CSUtility.AISystem.Attribute.AllowMember("召唤物旋转的角度", CSUtility.Helper.enCSType.Common, "召唤一次物旋转的角度")]
        [CSUtility.Event.Attribute.AllowMember("召唤物旋转的角度", CSUtility.Helper.enCSType.Common, "召唤一次物旋转的角度")]
        public float AIRotationAngle
        {
            get { return mAIRotationAngle; }
            set { mAIRotationAngle = value; }
        }


        bool mCanFire = false;
        [CSUtility.Support.DataValueAttribute("CanFire")]
        [System.ComponentModel.DisplayName("是否发射技能")]
        [System.ComponentModel.Category("召唤物行为")]
        public bool CanFire
        {
            get { return mCanFire; }
            set { mCanFire = value; }
        }

//         [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
//         public class SkillParam : CSUtility.Support.Copyable
//         {
//             UInt16 mSkillId = UInt16.MaxValue;
//             [CSUtility.Support.DataValueAttribute("SkillId")]
//             [System.ComponentModel.DisplayName("技能id")]
//             [System.ComponentModel.Category("技能释放技能")]
//             [CSUtility.AISystem.Attribute.AllowMember("", CSUtility.Helper.enCSType.Common, "")]
//             [CSUtility.Event.Attribute.AllowMember("", CSUtility.Helper.enCSType.Common, "")]
//             public UInt16 SkillId
//             {
//                 get { return mSkillId; }
//                 set { mSkillId = value; }
//             }
//             UInt16 mSkillLevel = 0;
//             [CSUtility.Support.DataValueAttribute("SkillLevel")]
//             [System.ComponentModel.DisplayName("技能等级")]
//             [System.ComponentModel.Category("技能释放技能")]
//             [CSUtility.AISystem.Attribute.AllowMember(CSCommon.Helper.enCSType.Common)]
//             [CSCommon.Event.Attribute.AllowMember(CSCommon.Helper.enCSType.Common)]
//             public UInt16 SkillLevel
//             {
//                 get { return mSkillLevel; }
//                 set { mSkillLevel = value; }
//             }
// 
//             public UInt16 mOdds = 1;
//             [CSUtility.Support.DataValueAttribute("Odds")]
//             [System.ComponentModel.DisplayName("技能释放几率")]
//             [System.ComponentModel.Category("技能释放技能")]
//             [CSCommon.AISystem.Attribute.AllowMember(CSCommon.Helper.enCSType.Common)]
//             [CSCommon.Event.Attribute.AllowMember(CSCommon.Helper.enCSType.Common)]
//             public UInt16 Odds
//             {
//                 get { return mOdds; }
//                 set { mOdds = value; }
//             }
// 
// 
//             public UInt16 mSkillId = UInt16.MaxValue;
//             [CSUtility.Support.DataValueAttribute("SkillId")]
//             [System.ComponentModel.DisplayName("技能id")]
//             [System.ComponentModel.Category("技能释放技能")]
//             [CSCommon.AISystem.Attribute.AllowMember(CSCommon.Helper.enCSType.Common)]
//             [CSCommon.Event.Attribute.AllowMember(CSCommon.Helper.enCSType.Common)]
//             public UInt16 SkillId
//             {
//                 get { return mSkillId; }
//                 set { mSkillId = value; }
//             }
//         }
// 
//         List<SkillParam> mSummonSkills = new List<SkillParam>();
//         [CSUtility.Support.DataValueAttribute("SummonSkills")]
//         [System.ComponentModel.DisplayName("召唤物的技能列表")]
//         [System.ComponentModel.Category("召唤物行为")]
//         public List<SkillParam> SummonSkills
//         {
//             get { return mSummonSkills; }
//             set { mSummonSkills = value; }
//         }
// 
//         [CSCommon.AISystem.Attribute.AllowMethod(CSCommon.Helper.enCSType.Common)]
//         [CSCommon.Event.Attribute.AllowMethod(CSCommon.Helper.enCSType.Common)]
//         public SkillParam GetSummskillParam()
//         {
//             foreach (var skill in SummonSkills)
//             {
//                 if (skill != null)
//                 {
//                     return skill;
//                 }
//             }
//             return null;
//         }
        #endregion



        public SkillLevelParam GetLevelDamage(byte skillLevel)
        {
            foreach (var data in mSkillLevelDatas)
            {
                if (data.SkillLevel == skillLevel)
                    return data;
            }
            return null;
        }

    }

    public class SkillData : RPC.IAutoSaveAndLoad
    {
        UInt16 mTemplateId = UInt16.MaxValue;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public UInt16 TemplateId
        {
            get { return mTemplateId; }
            set
            {
                mTemplateId = value;

                mTemplate = CSUtility.Data.DataTemplateManager<UInt16, GameData.Skill.SkillTemplate>.Instance.GetDataTemplate(mTemplateId);// SkillTemplateManager.Instance.FindSkill(mTemplateId);
            }
        }

        SkillTemplate mTemplate = null;   
        [RPC.FieldDontAutoSaveLoadAttribute()]
        public SkillTemplate Template
        {
            get { return mTemplate; }
        }

        UInt16 mSkillLevel = 0;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public UInt16 SkillLevel
        {
            get { return mSkillLevel; }
            set { mSkillLevel = value; }
        }

        float mRemainCD = 0.0f;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public float RemainCD
        {
            get { return mRemainCD; }
            set { mRemainCD = value; }
        }

        SkillInfoData mFireSkillInfoData = null;
        [RPC.FieldDontAutoSaveLoadAttribute()]
        [CSUtility.Support.DoNotCopy]
        public SkillInfoData FireSkillInfoData
        {
            get { return mFireSkillInfoData; }
            set { mFireSkillInfoData = value; }
        }

        public SkillTemplate.SkillLevelParam GetSkillLevelTemp()
        {
            if (Template == null || Template.SkillLevelDatas.Count < SkillLevel || SkillLevel == 0)
                return null;
            return Template.SkillLevelDatas[SkillLevel - 1];
        }
    }
}
