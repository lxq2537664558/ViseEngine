using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Role
{
    public class PlayerDetail : CSUtility.Data.RoleDataBase
    {
        CSUtility.AISystem.IStateHost mHostPlayer;

        public void _SetHostPlayer(CSUtility.AISystem.IStateHost player)
        {
            mHostPlayer = player;
        }

        protected new void OnPropertyChanged(string proName)
        {
            if (mHostPlayer == null)
                return;
            var proInfo = this.GetType().GetProperty(proName);

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
            else if (proInfo.PropertyType == typeof(String))
                dw.Write(System.Convert.ToString(proInfo.GetValue(this, null)));            
            else
                return;

            mHostPlayer.OnValueChanged(proName, dw);
        }
        public RoleTemplate RoleTemplate
        {
            get
            {
                return base.Template as RoleTemplate;
            }
        }
        public Guid HallsId { get; set; } = Guid.Empty;

        byte mRoleLevel = 1;
        public byte RoleLevel
        {
            get { return mRoleLevel; }
            set
            {
                mRoleLevel = value;

                OnPropertyChanged("RoleLevel");
            }
        }

        long mRoleExp = 0;
        public long RoleExp
        {
            get { return mRoleExp; }
            set
            {
                mRoleExp = value;

                OnPropertyChanged("RoleExp");
            }
        }

        float mRoleMoveSpeed = 0;
        public float RoleMoveSpeed
        {
            get { return mRoleMoveSpeed; }
            set
            {
                mRoleMoveSpeed = value;

                OnPropertyChanged("RoleMoveSpeed");
            }
        }

        int mRoleHp = 0;
        [CSUtility.Event.Attribute.AllowMember("角色数据.当前血量", CSUtility.Helper.enCSType.Common, "当前血量")]
        public int RoleHp
        {
            get { return mRoleHp; }
            set
            {                
                mRoleHp = value;

                OnPropertyChanged("RoleHp");
            }
        }

        int mRoleMp = 0;        
        public int RoleMp
        {
            get { return mRoleMp; }
            set
            {
                mRoleMp = value;

                OnPropertyChanged("RoleMp");
            }
        }

        int mMaxRoleHp = 0;
        [CSUtility.Event.Attribute.AllowMember("角色数据.最大血量", CSUtility.Helper.enCSType.Common, "最大血量")]
        public int MaxRoleHp
        {
            get { return mMaxRoleHp; }
            set
            {
                mMaxRoleHp = value;

                OnPropertyChanged("MaxRoleHp");
            }
        }

        int mMaxRoleMp = 0;
        public int MaxRoleMp
        {
            get { return mMaxRoleMp; }
            set
            {
                mMaxRoleMp = value;

                OnPropertyChanged("MaxRoleMp");
            }
        }

        int mRoleGold = 0;
        public int RoleGold
        {
            get { return mRoleGold; }
            set
            {
                mRoleGold = value;

                OnPropertyChanged("RoleGold");
            }
        }

        byte mRoleSkillPoint = 0;
        public byte RoleSkillPoint
        {
            get { return mRoleSkillPoint; }
            set
            {
                mRoleSkillPoint = value;

                OnPropertyChanged("RoleSkillPoint");
            }
        }

        public float LocationX { get; set; } = 0;
        public float LocationY { get; set; } = 0;
        public float LocationZ { get; set; } = 0;
    }
}
