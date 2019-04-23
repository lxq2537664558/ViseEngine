using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameData
{
    [CSUtility.Data.DataTemplate(".faction", "Faction")]
    public class Faction : CSUtility.Data.IDataTemplateBase<UInt16>
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

        SlimDX.Vector3 mStarPoint;
        [CSUtility.Support.DataValueAttribute("StarPoint")]
        public SlimDX.Vector3 StarPoint
        {
            get { return mStarPoint; }
            set { mStarPoint = value; }
        }

        SlimDX.Vector3 mEndPoint;
        [CSUtility.Support.DataValueAttribute("EndPoint")]
        public SlimDX.Vector3 EndPoint
        {
            get { return mEndPoint; }
            set { mEndPoint = value; }
        }

        //         List<UInt16> mFriends = new List<UInt16>();
        //         [CSUtility.Support.DataValueAttribute("Friends")]
        //         public List<UInt16> Friends
        //         {//绝不伤害
        //             get { return mFriends; }
        //             set { mFriends = value; }
        //         }
        // 
        //         List<UInt16> mEnemys = new List<UInt16>();
        //         [CSUtility.Support.DataValueAttribute("Enemys")]
        //         public List<UInt16> Enemys
        //         {//仇人见面
        //             get { return mEnemys; }
        //             set { mEnemys = value; }
        //         }
        // 
        //         public bool IsFriend(UInt16 faction)
        //         {
        //             foreach (var i in mFriends)
        //             {
        //                 if (i == faction)
        //                     return true;
        //             }
        //             return false;
        //         }
        // 
        //         public bool IsEnemy(UInt16 faction)
        //         {
        //             foreach (var i in mEnemys)
        //             {
        //                 if (i == faction)
        //                     return true;
        //             }
        //             return false;
        //         }
    }
}
