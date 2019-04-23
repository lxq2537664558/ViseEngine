using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameData.Data
{
    [CSUtility.Data.DataTemplate(".moncrt", "MonsterCreate")]
    public class MonsterCreateTemplate : CSUtility.Data.IDataTemplateBase<UInt16>
    {
        /// <summary>
        /// 数据模板ID
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Id")]
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

        List<CreateInfo> mMonsterList = new List<CreateInfo>();
        [CSUtility.Support.DataValueAttribute("MonsterList")]
        public List<CreateInfo> MonsterList
        {
            get { return mMonsterList; }
            set { mMonsterList = value; }
        }
    }

    public class CreateInfo
    {
        UInt16 mRoleTemplateId;
        [CSUtility.Support.DataValueAttribute("RoleTemplateId")]                        
        public UInt16 RoleTemplateId
        {
            get { return mRoleTemplateId; }
            set
            {
                mRoleTemplateId = value;
            }
        }
    }

}
