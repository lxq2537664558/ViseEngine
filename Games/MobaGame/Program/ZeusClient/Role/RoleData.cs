using GameData.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Role
{
    public enum EClientRoleType
    {
        ChiefPlayer,
        OtherPlayer,
        Monster,
        Summon,
        DropedItem,
        GatherItem,
        Count
    }

    public class RoleData : CSUtility.Component.IComponent
    {
        RoleActor mRole;

        public RoleData(PlayerData data,RoleActor role)
        {
            mRoleType = EClientRoleType.ChiefPlayer;
            mPlayerData = data;
            mRole = role;
        }

        public RoleData(PlayerData data, RoleActor role,bool otherPlayer)
        {
            mRoleType = EClientRoleType.OtherPlayer;
            mPlayerData = data;
            mRole = role;
        }

        public RoleData(MonsterData data, RoleActor role)
        {
            mRoleType = EClientRoleType.Monster;
            mMonsterData = data;
            mRole = role;
        }

        public RoleData(SummonData data, RoleActor role)
        {
            mRoleType = EClientRoleType.Summon;
            mSummonData = data;
            mRole = role;
        }

        EClientRoleType mRoleType;
        public EClientRoleType RoleType
        {
            get { return mRoleType; }
        }

        PlayerData mPlayerData;
        [CSUtility.AISystem.Attribute.AllowMember("属性.玩家数据",CSUtility.Helper.enCSType.Client,"")]
        [CSUtility.Event.Attribute.AllowMember("属性.玩家数据",CSUtility.Helper.enCSType.Client,"")]
        public PlayerData PlayerData
        {
            get { return mPlayerData; }
        }

        MonsterData mMonsterData;
        [CSUtility.AISystem.Attribute.AllowMember("属性.怪物数据", CSUtility.Helper.enCSType.Client, "")]
        [CSUtility.Event.Attribute.AllowMember("属性.怪物数据", CSUtility.Helper.enCSType.Client, "")]
        public MonsterData MonsterData
        {
            get { return mMonsterData; }
        }

        SummonData mSummonData;
        [CSUtility.AISystem.Attribute.AllowMember("属性.召唤物数据", CSUtility.Helper.enCSType.Client, "")]
        [CSUtility.Event.Attribute.AllowMember("属性.召唤物数据", CSUtility.Helper.enCSType.Client, "")]
        public SummonData SummonData
        {
            get { return mSummonData; }
        }

        //         public CSUtility.Support.Color TitleColor
        //         {
        //             get
        //             {
        //                 switch (mRoleType)
        //                 {
        //                     case EClientRoleType.ChiefPlayer:
        // 
        //                 }
        //             }
        //         }

        float mLiveTime;
        [CSUtility.AISystem.Attribute.AllowMember("属性.存活时间", CSUtility.Helper.enCSType.Client, "")]
        [CSUtility.Event.Attribute.AllowMember("属性.存活时间", CSUtility.Helper.enCSType.Client, "")]
        public float LiveTime
        {
            get
            {
                return mLiveTime;
            }
            set
            {
                mLiveTime = value;
            }
        }

        public RoleTemplate RoleTemplate
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.Template as RoleTemplate;                        
                    case EClientRoleType.Monster:
                        return mMonsterData.Template as RoleTemplate;
                    case EClientRoleType.Summon:
                        return mSummonData.Template as RoleTemplate;              
                    default:
                        return null;
                }
            }
        }

        public Guid RoleId
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.RoleId;
                    case EClientRoleType.Monster:
                        return mMonsterData.RoleId;
                    case EClientRoleType.Summon:
                        return mSummonData.RoleId;
                    default:
                        return Guid.Empty;
                }
            }
        }

        public byte RoleLevel
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.RoleLevel;
                    case EClientRoleType.Monster:
                        return 0;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        mPlayerData.RoleLevel = value;
                        break;
                }
            }
        }

        public int FactionId
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.PlayerFaction;
                    case EClientRoleType.Monster:
                        return mMonsterData.MonsterFaction;
                    case EClientRoleType.Summon:
                        return mSummonData.MonsterFaction;
                    default:
                        return UInt16.MaxValue;
                }
            }
        }

        public string RoleName
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.Name;
                    case EClientRoleType.Monster:
                        return mMonsterData.Name;
                    case EClientRoleType.Summon:
                        return mSummonData.Name;
                    default:
                        return "";
                }
            }
        }

        public float RoleMoveSpeed
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.RoleMoveSpeed;
                    case EClientRoleType.Monster:
                        return mMonsterData.RoleMoveSpeed;
                    case EClientRoleType.Summon:
                        return mSummonData.RoleMoveSpeed;
                    default:
                        return 0.0f;
                }
            }
            set
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        mPlayerData.RoleMoveSpeed = value; ;
                        break;
                    case EClientRoleType.Monster:
                        mMonsterData.RoleMoveSpeed = value;
                        break;
                    case EClientRoleType.Summon:
                        mSummonData.RoleMoveSpeed = value;
                        break;
                }
            }
        }

        public UInt32 TemplateVersion
        {
            get
            {
                if (RoleTemplate != null)
                    return RoleTemplate.Version;

                return 0;
            }
        }

        Int64 mBornTime = 0;
        public Int64 BornTime
        {
            get { return mBornTime; }
            set { mBornTime = value; }
        }

        int mRoleHP = 0;
        [CSUtility.Event.Attribute.AllowMember("属性.当前血量", CSUtility.Helper.enCSType.Client, "RoleHP")]
        public int RoleHP
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.RoleHp;
                    case EClientRoleType.Monster:
                        return mMonsterData.RoleHp;
                    default:
                        return mRoleHP;
                }
            }
            set
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        mPlayerData.RoleHp = value; ;
                        break;
                    case EClientRoleType.Monster:
                        mMonsterData.RoleHp = value;
                        break;
                    default:
                        mRoleHP = value;
                        break;
                }
            }
        }

        int mRoleMaxHP = 0;
        public int RoleMaxHP
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.MaxRoleHp;
                    case EClientRoleType.Monster:
                        return mMonsterData.MaxRoleHp;
                    default:
                        return mRoleMaxHP;
                }
            }
            set
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        mPlayerData.MaxRoleHp = value;
                        break;
                    case EClientRoleType.Monster:
                        mMonsterData.MaxRoleHp = value;
                        break;
                    default:
                        mRoleMaxHP = value;
                        break;
                }
            }
        }

        int mRoleMP = 0;
        public int RoleMP
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.RoleMp;
                    case EClientRoleType.Monster:
                        return mMonsterData.RoleMp;
                    default:
                        return mRoleMP;
                }
            }
            set
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        mPlayerData.RoleMp = value;
                        break;
                    case EClientRoleType.Monster:
                        mMonsterData.RoleMp = value;
                        break;
                    default:
                        mRoleMP = value;
                        break;
                }
            }
        }

        int mRoleMaxMP = 0;
        public int RoleMaxMP
        {
            get
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        return mPlayerData.MaxRoleMp;
                    case EClientRoleType.Monster:
                        return mMonsterData.MaxRoleMp;
                    default:
                        return mRoleMaxMP;
                }
            }
            set
            {
                switch (mRoleType)
                {
                    case EClientRoleType.ChiefPlayer:
                    case EClientRoleType.OtherPlayer:
                        mPlayerData.MaxRoleMp = value;
                        break;
                    case EClientRoleType.Monster:
                        mMonsterData.MaxRoleMp = value;
                        break;
                    default:
                        mRoleMaxMP = value;
                        break;
                }
            }
        }

        Guid mAIID;
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("AISet")]
        public Guid AIGuid
        {
            get { return mAIID; }
            set
            {
                mAIID = value;

                //新的FSM初始化代码
                mRole.InitFSM(mAIID, true);
            }
        }
    }
}
