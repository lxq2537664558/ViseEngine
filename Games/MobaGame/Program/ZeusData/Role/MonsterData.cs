using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameData.Role
{
    public class MonsterInit : CSUtility.Data.RoleDataBase
    {
        CSUtility.AISystem.IStateHost mHostMonster;

        public void _SetHostNpc(CSUtility.AISystem.IStateHost monster)
        {
            mHostMonster = monster;
        }

        protected void SendPackage(string proName)
        {
            if (mHostMonster == null)
                return;
            var proInfo = this.GetType().GetProperty(proName);
            if (proInfo == null)
                return;

            RPC.PackageWriter pkg = new RPC.PackageWriter();

            RPC.DataWriter dw = new RPC.DataWriter();
            if (proInfo.PropertyType == typeof(Byte))
                dw.Write(System.Convert.ToByte(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(UInt16))
                dw.Write(System.Convert.ToUInt16(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(UInt32))
                dw.Write(System.Convert.ToUInt32(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(UInt64))
                dw.Write(System.Convert.ToUInt64(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(SByte))
                dw.Write(System.Convert.ToSByte(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(Int16))
                dw.Write(System.Convert.ToInt16(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(Int32))
                dw.Write(System.Convert.ToInt32(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(Int64))
                dw.Write(System.Convert.ToInt64(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(float))
                dw.Write(System.Convert.ToSingle(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(double))
                dw.Write(System.Convert.ToDouble(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(System.Guid))
                dw.Write(CSUtility.Support.IHelper.GuidParse(proInfo.GetValue(this, null).ToString()));
            else if (proInfo.PropertyType.IsEnum)
                dw.Write(System.Convert.ToByte(proInfo.GetValue(this, null)));
            else if (proInfo.PropertyType == typeof(Boolean))
                dw.Write(System.Convert.ToBoolean(proInfo.GetValue(this, null)));
            else
                return;

            mHostMonster.OnValueChanged(proName, dw);
        }
    }
    public class MonsterData : MonsterInit
    {
        [CSUtility.Support.AutoSaveLoadAttribute]
        public List<GameData.Skill.SkillData> Skills { get; set; } = new List<GameData.Skill.SkillData>();

        [CSUtility.Support.AutoSaveLoadAttribute]
        public int MonsterFaction { get; set; }

        [Browsable(false)]
        public RoleTemplate RoleTemplate
        {
            get { return base.Template as RoleTemplate; }
        }

        float mRoleMoveSpeed = 0;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public float RoleMoveSpeed
        {
            get { return mRoleMoveSpeed; }
            set { mRoleMoveSpeed = value; }
        }

        int mRoleHp = 0;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public int RoleHp
        {
            get { return mRoleHp; }
            set
            {
                mRoleHp = value;
                SendPackage("RoleHp");
            }
        }
        int mRoleMp = 0;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public int RoleMp
        {
            get { return mRoleMp; }
            set
            {
                mRoleMp = value;
                SendPackage("RoleMp");
            }
        }

        int mMaxRoleHp = 10;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public  int MaxRoleHp
        {
            get { return mMaxRoleHp; }
            set
            {
                mMaxRoleHp = value;
                SendPackage("MaxRoleHp");
            }
        }
        int mMaxRoleMp = 10;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public  int MaxRoleMp
        {
            get { return mMaxRoleMp; }
            set
            {
                mMaxRoleMp = value;
                SendPackage("MaxRoleMp");
            }
        }

        [CSUtility.Editor.Editor_PropertyGridDataTemplate("AISet")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public Guid AIGuid { get; set; }

        bool mUnrivaled = false;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Event.Attribute.AllowMember("MonsterData.是否无敌", CSUtility.Helper.enCSType.Common, "是否无敌")]
        public bool Unrivaled
        {
            get { return mUnrivaled; }
            set
            {
                mUnrivaled = value;

                SendPackage("Unrivaled");
            }
        }

        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Event.Attribute.AllowMember("MonsterData.关联RoleID", CSUtility.Helper.enCSType.Common, "关联roleId")]
        public Guid NextLinkedRoleId { get; set; } = Guid.Empty;        
    }

    public class SummonData : MonsterData
    {
        public void Init(GameData.Skill.SkillData skillData)
        {
            RoleId = Guid.NewGuid();
            this.TemplateId = skillData.Template.ThrowRoleId;
            SkillTemplateId = (UInt16)skillData.TemplateId;
            SkillData = skillData;
            AIGuid = skillData.Template.AIGuid;
            LiveTime = skillData.Template.LiveTime;
        }

        byte mIsUpdatePos2Client = 0;
        public byte IsUpdatePos2Client
        {
            get { return mIsUpdatePos2Client; }
            set { mIsUpdatePos2Client = value; }
        }

        UInt16 mSkillTemplateId;
        public UInt16 SkillTemplateId
        {
            get { return mSkillTemplateId; }
            set
            {
                mSkillTemplateId = value;
            }
        }
        UInt32  mLockOnRoleId=UInt32.MaxValue;
        public UInt32 LockOnRoleId
        {
            get { return mLockOnRoleId; }
            set
            {
                mLockOnRoleId = value;
            }
        }
        GameData.Skill.SkillData mSkillData = new GameData.Skill.SkillData();
        public GameData.Skill.SkillData SkillData
        {
            get { return mSkillData; }
            set { mSkillData = value; }
        }

        SlimDX.Vector3 mOiDirection;
        public SlimDX.Vector3 OiDirection
        {
            get { return mOiDirection; }
            set { mOiDirection = value; }
        }

        SlimDX.Vector3 mAccelerationY = SlimDX.Vector3.Zero;
        public SlimDX.Vector3 AccelerationY
        {
            get { return mAccelerationY; }
            set { mAccelerationY = value; }
        }

        float mLiveTime;
        public float LiveTime
        {
            get { return mLiveTime; }
            set
            {
                mLiveTime = value;
            }
        }

        float mHeight;
        public float Height
        {
            get { return mHeight; }
            set
            {
                mHeight = value;
            }
        }

        bool mNeedServerCollider = false;
        public bool NeedServerCollider
        {
            get { return mNeedServerCollider; }
            set { mNeedServerCollider = value; }
        }
    }
}
