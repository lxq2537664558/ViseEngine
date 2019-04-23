using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameData.Role
{
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("RoleTemplate")]
    public delegate void FOnCollision(CSUtility.Component.ActorBase hostRole, CSUtility.Component.ActorBase targetRole);

    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("RoleTemplate")]
    public delegate void FOnRoleDeath(CSUtility.AISystem.IStateHost host);
    public enum MonsterType
    {
        Normal,     //普通
        Building,   //建筑
        Symbol,     //符
        Other,
    }

    [CSUtility.Data.DataTemplate(".role")]
    [CSUtility.Editor.CDataEditorAttribute(".role")]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class RoleTemplate : CSUtility.Data.RoleTemplateBase
    {
        string mRoleName;
        [CSUtility.Support.DataValueAttribute("RoleName")]
        public string RoleName
        {
            get { return mRoleName; }
            set { mRoleName = value; }
        }

        List<UInt16> mRoleSkillList = new List<UInt16>();
        [CSUtility.Support.DataValueAttribute("RoleSkillList")]
        [DisplayName("角色技能列表")]
        public List<UInt16> RoleSkillList
        {
            get { return mRoleSkillList; }
            set { mRoleSkillList = value; }
        }

        List<Skill.SkillInfo> mSkillInfos = new List<Skill.SkillInfo>();
        [CSUtility.Support.DataValueAttribute("SkillInfos")]
        [DisplayName("怪物技能列表")]
        public List<Skill.SkillInfo> SkillInfos
        {
            get { return mSkillInfos; }
            set { mSkillInfos = value; }
        }

        float mMoveSpeed = 1.5F;
        [CSUtility.Support.DataValueAttribute("MoveSpeed")]
        [DisplayName("移动速度")]
        public float MoveSpeed
        {
            get { return mMoveSpeed; }
            set { mMoveSpeed = value; }
        }
        
        float mRunMoveSpeed = 3.0F;
        [CSUtility.Support.DataValueAttribute("RunMoveSpeed")]
        public float RunMoveSpeed
        {
            get { return mRunMoveSpeed; }
            set { mRunMoveSpeed = value; }
        }

        int mOiHp = 50;
        [CSUtility.Support.DataValueAttribute("OiHp")]
        [DisplayName("初始血量")]
        public int OiHp
        {
            get { return mOiHp; }
            set { mOiHp = value; }
        }

        int mOiMp = 50;
        [CSUtility.Support.DataValueAttribute("OiMp")]
        [DisplayName("初始魔法")]
        public int OiMp
        {
            get { return mOiMp; }
            set { mOiMp = value; }
        }

        int mBaseDamage = 10;
        [CSUtility.Support.DataValueAttribute("BaseDamage")]
        [DisplayName("初始攻击力")]
        public int BaseDamage
        {
            get { return mBaseDamage; }
            set { mBaseDamage = value; }
        }

        int mBaseDefend= 2;
        [CSUtility.Support.DataValueAttribute("BaseDefend")]
        [DisplayName("初始防御")]
        public int BaseDefend
        {
            get { return mBaseDefend; }
            set { mBaseDefend = value; }
        }

        bool mSlerpRotation = true;
        [CSUtility.Support.DataValueAttribute("SlerpRotation")]
        public bool SlerpRotation
        {
            get { return mSlerpRotation; }
            set { mSlerpRotation = value; }
        }
        
        bool mIsRotate = true;
        [CSUtility.Support.DataValueAttribute("IsRotate")]
        [DisplayName("是否旋转")]
        public bool IsRotate
        {
            get { return mIsRotate; }
            set { mIsRotate = value; }
        }

        SlimDX.Vector3 mOriginPosition;
        [CSUtility.Support.DataValueAttribute("OriginPosition")]
        [DisplayName("出生位置")]
        public SlimDX.Vector3 OriginPosition
        {
            get { return mOriginPosition; }
            set { mOriginPosition = value; }
        }

        int mOriginRotation;
        [CSUtility.Support.DataValueAttribute("OriginRotation")]
        public int OriginRotation
        {
            get { return mOriginRotation; }
            set { mOriginRotation = value; }
        }

        bool mCastShadow;
        [CSUtility.Support.DataValueAttribute("CastShadow")]
        public bool CastShadow
        {
            get { return mCastShadow; }
            set
            {
                mCastShadow = value;
            }
        }

        int mFactionId = 1;
        [CSUtility.Support.DataValueAttribute("FactionId")]//角色阵营id
        public int FactionId
        {
            get { return mFactionId; }
            set { mFactionId = value; }
        }

        float mHalfHeight;
        [CSUtility.Support.DataValueAttribute("HalfHeight")]
        [DisplayName("角色半高")]
        public float HalfHeight
        {
            get { return mHalfHeight; }
            set
            {
                mHalfHeight = value;
            }
        }

        Guid mAIGuid;
        [CSUtility.Support.DataValueAttribute("AIGuid")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("AISet")]        
        public Guid AIGuid
        {
            get { return mAIGuid; }
            set
            {
                mAIGuid = value;
            }
        }

        float mLockOnRadius = 4;
        [CSUtility.Support.DataValueAttribute("LockOnRadius")]        
        [DisplayName("索敌半径")]
        public float LockOnRadius
        {
            get { return mLockOnRadius; }
            set { mLockOnRadius = value; }
        }

        bool mReborn = false;
        [CSUtility.Support.DataValueAttribute("Reborn")]
        public bool Reborn
        {
            get { return mReborn; }
            set { mReborn = value; }
        }        

        Int64 mRebornElapsed;
        [CSUtility.Support.DataValueAttribute("RebornElapsed")]
        public Int64 RebornElapsed
        {
            get { return mRebornElapsed; }
            set { mRebornElapsed = value; }
        }

        float mRadius = 0.5f;
        [CSUtility.Support.DataValueAttribute("Radius")]
        [DisplayName("半径")]
        public float Radius
        {
            get { return mRadius; }
            set { mRadius = value; }
        }
        CSUtility.Helper.EventCallBack mOnCollisionCB;
        [System.ComponentModel.Browsable(false)]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack OnCollisionCB
        {
            get { return mOnCollisionCB; }
        }
        Guid mOnCollision = Guid.Empty;
        [Category("脚本")]
        [CSUtility.Support.DataValueAttribute("OnCollision")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FOnCollision) })]        
        public Guid OnCollision
        {
            get { return mOnCollision; }
            set
            {
                mOnCollision = value;
                mOnCollisionCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnCollision), value);
            }
        }

        CSUtility.Helper.EventCallBack mOnRoleDeathCB;
        [System.ComponentModel.Browsable(false)]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack OnRoleDeathCB
        {
            get { return mOnRoleDeathCB; }
        }
        Guid mOnRoleDeath = Guid.Empty;
        [Category("脚本")]
        [CSUtility.Support.DataValueAttribute("OnRoleDeath")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FOnRoleDeath) })]
        public Guid OnRoleDeath
        {
            get { return mOnRoleDeath; }
            set
            {
                mOnRoleDeath = value;
                mOnRoleDeathCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnRoleDeath), value);
            }
        }

        int mDeathExp = 0;
        [CSUtility.Support.DataValueAttribute("DeathExp")]
        public int DeathExp
        {
            get { return mDeathExp; }
            set { mDeathExp = value; }
        }

        bool mFixedHeight = false;
        [CSUtility.Support.DataValueAttribute("FixedHeight")]
        public bool FixedHeight
        {
            get { return mFixedHeight; }
            set { mFixedHeight = value; }
        }

        bool mCalCollission = false;
        [CSUtility.Support.DataValueAttribute("CalCollission")]
        public bool CalCollission
        {
            get { return mCalCollission; }
            set { mCalCollission = value; }
        }

        MonsterType mMonsterType = MonsterType.Normal;
        [CSUtility.Support.DataValueAttribute("MonsterType")]
        public MonsterType MonsterType
        {
            get { return mMonsterType; }
            set { mMonsterType = value; }
        }

        SlimDX.Vector3 mPosOffset = SlimDX.Vector3.Zero;
        [CSUtility.Support.DataValueAttribute("PosOffset")]
        public SlimDX.Vector3 PosOffset
        {
            get { return mPosOffset; }
            set { mPosOffset = value; }
        }

        bool mPrimaryRole = false;
        [CSUtility.Support.DataValueAttribute("PrimaryRole")]
        public bool PrimaryRole
        {
            get { return mPrimaryRole; }
            set { mPrimaryRole = value; }
        }

        UInt16 mBloodReturn = 0;
        [CSUtility.Support.DataValueAttribute("BloodReturn")]        
        public UInt16 BloodReturn
        {
            get { return mBloodReturn; }
            set { mBloodReturn = value; }
        }

        long mBloodReturnTimes = 0;
        [CSUtility.Support.DataValueAttribute("BloodReturnTimes")]
        public long BloodReturnTimes
        {
            get { return mBloodReturnTimes; }
            set { mBloodReturnTimes = value; }
        }
    }
}
