using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameData.Role
{
    [CSUtility.Data.DataTemplate(".roleCom", "RoleCommon")]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class RoleCommonTemplate : CSUtility.Support.Copyable, CSUtility.Data.IDataTemplateBase<Byte>
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

        private SlimDX.Vector4 mEdgeDetectFriend;
        [CSUtility.Support.DataValueAttribute("EdgeDetectFriend")]
        [System.ComponentModel.Description("友方勾边颜色")]
        [System.ComponentModel.DisplayName("友方勾边颜色")]
        [System.ComponentModel.Category("勾边数据")]
        public SlimDX.Vector4 EdgeDetectFriend
        {
            get { return mEdgeDetectFriend; }
            set { mEdgeDetectFriend = value; }
        }

        private SlimDX.Vector4 mEdgeDetectNeutral;
        [CSUtility.Support.DataValueAttribute("EdgeDetectNeutral")]
        [System.ComponentModel.Description("中立勾边颜色")]
        [System.ComponentModel.DisplayName("中立勾边颜色")]
        [System.ComponentModel.Category("勾边数据")]
        public SlimDX.Vector4 EdgeDetectNeutral
        {
            get { return mEdgeDetectNeutral; }
            set { mEdgeDetectNeutral = value; }
        }

        private SlimDX.Vector4 mEdgeDetectEnemy;
        [CSUtility.Support.DataValueAttribute("EdgeDetectEnemy")]
        [System.ComponentModel.Description("敌方勾边颜色")]
        [System.ComponentModel.DisplayName("敌方勾边颜色")]
        [System.ComponentModel.Category("勾边数据")]
        public SlimDX.Vector4 EdgeDetectEnemy
        {
            get { return mEdgeDetectEnemy; }
            set { mEdgeDetectEnemy = value; }
        }

        private SlimDX.Vector4 mEdgeDetectLight;
        [CSUtility.Support.DataValueAttribute("EdgeDetectLight")]
        [System.ComponentModel.Description("光源勾边颜色")]
        [System.ComponentModel.DisplayName("光源勾边颜色")]
        [System.ComponentModel.Category("勾边数据")]
        public SlimDX.Vector4 EdgeDetectLight
        {
            get { return mEdgeDetectLight; }
            set { mEdgeDetectLight = value; }
        }

        private SlimDX.Vector4 mMonsterLight = new SlimDX.Vector4(1, 1, 0.2f, 1);
        [CSUtility.Support.DataValueAttribute("MonsterLight")]
        [System.ComponentModel.Description("高光")]
        [System.ComponentModel.DisplayName("高光")]
        [System.ComponentModel.Category("显示")]
        public SlimDX.Vector4 MonsterLight
        {
            get { return mMonsterLight; }
            set { mMonsterLight = value; }
        }

        private float mRimStart = 0.5f;
        [CSUtility.Support.DataValueAttribute("RimStart")]
        [System.ComponentModel.Description("高光区域起始百分比")]
        [System.ComponentModel.DisplayName("高光区域起始百分比")]
        [System.ComponentModel.Category("显示")]
        public float RimStart
        {
            get { return mRimStart; }
            set { mRimStart = value; }
        }

        private float mRimEnd = 1.0f;
        [CSUtility.Support.DataValueAttribute("RimEnd")]
        [System.ComponentModel.Description("高光区域结束百分比")]
        [System.ComponentModel.DisplayName("高光区域结束百分比")]
        [System.ComponentModel.Category("显示")]
        public float RimEnd
        {
            get { return mRimEnd; }
            set { mRimEnd = value; }
        }

        private float mRimCycle = 1.0f;
        [CSUtility.Support.DataValueAttribute("RimCycle")]
        [System.ComponentModel.Description("高光闪烁周期")]
        [System.ComponentModel.DisplayName("高光闪烁周期")]
        [System.ComponentModel.Category("显示")]
        public float RimCycle
        {
            get { return mRimCycle; }
            set { mRimCycle = value; }
        }

        private float mRimCycleStart = 0.2f;
        [CSUtility.Support.DataValueAttribute("RimCycleStart")]
        [System.ComponentModel.Description("高光闪烁周期起始值")]
        [System.ComponentModel.DisplayName("高光闪烁周期起始值")]
        [System.ComponentModel.Category("显示")]
        public float RimCycleStart
        {
            get { return mRimCycleStart; }
            set { mRimCycleStart = value; }
        }

    }
}
