using System;

namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 特殊动作状态参数
    /// </summary>
    public sealed class ISpecialActionParameter : StateParameter
    {
        string mActionName = "";
        /// <summary>
        /// 动作名称
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.特殊动作状态参数.动作名称", CSUtility.Helper.enCSType.Common, "特殊动作状态参数动作名称")]
        [CSUtility.Event.Attribute.AllowMember("AI.特殊动作状态参数.动作名称", CSUtility.Helper.enCSType.Common, "特殊动作状态参数动作名称")]
        public string ActionName
        {
            get { return mActionName; }
            set { mActionName = value; }
        }

        Int64 mDuration = 0;
        /// <summary>
        /// 持续时间
        /// </summary>
        public Int64 Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }

        UInt32 mGatherSingleId = 0;
        /// <summary>
        /// 采集的唯一ID
        /// </summary>
        public UInt32 GatherSingleId
        {
            get { return mGatherSingleId; }
            set { mGatherSingleId = value; }
        }

        byte mLifeSkillType = 0;
        /// <summary>
        /// 生活技能类型
        /// </summary>
        public byte LifeSkillType
        {
            get { return mLifeSkillType; }
            set { mLifeSkillType = value; }
        }

        byte mNumber = 1;
        /// <summary>
        /// 数量
        /// </summary>
        public byte Number
        {
            get { return mNumber; }
            set { mNumber = value; }
        }

        byte mControlPara = 0;
        /// <summary>
        /// 控制参数
        /// </summary>
        public byte ControlPara
        {
            get { return mControlPara; }
            set { mControlPara = value; }
        }

        UInt16 mJumpMapTransId = UInt16.MaxValue;
        /// <summary>
        /// 传送地图ID
        /// </summary>
        public UInt16 JumpMapTransId
        {
            get { return mJumpMapTransId; }
            set { mJumpMapTransId = value; }
        }

        SlimDX.Vector3 mAddPosition = SlimDX.Vector3.Zero;
        /// <summary>
        /// 添加位置
        /// </summary>
        public SlimDX.Vector3 AddPosition
        {
            get { return mAddPosition; }
            set { mAddPosition = value; }
        }

        string mExtraInfomation = "";
        /// <summary>
        /// 扩展信息
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.特殊动作状态参数.扩展信息", CSUtility.Helper.enCSType.Common, "获取或设置特殊动作状态参数扩展信息")]
        [CSUtility.Event.Attribute.AllowMember("AI.特殊动作状态参数.扩展信息", CSUtility.Helper.enCSType.Common, "获取或设置特殊动作状态参数扩展信息")]
        public string ExtraInfomation
        {
            get { return mExtraInfomation; }
            set { mExtraInfomation = value; }
        }
        /// <summary>
        /// 获取扩展信息
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回扩展信息</returns>
        [CSUtility.AISystem.Attribute.AllowMember("AI.特殊动作状态参数.获取扩展信息", CSUtility.Helper.enCSType.Common, "获取特殊动作状态参数的指定扩展信息")]
        [CSUtility.Event.Attribute.AllowMember("AI.特殊动作状态参数.获取扩展信息", CSUtility.Helper.enCSType.Common, "获取特殊动作状态参数的指定扩展信息")]
        public string GetExtraInfomation(int index)
        {
            var seg = mExtraInfomation.Split('@');
            if (seg.Length <= index)
                return "";
            return seg[index];
        }
    }
    /// <summary>
    /// 特殊动作类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("特殊动作", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("特殊动作")]
    public class SpecialAction : State
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SpecialAction()
        {
            mStateName = "SpecialAction";
            Parameter = new ISpecialActionParameter();
        }
        /// <summary>
        /// 只读属性，特殊动作的参数
        /// </summary>
        public ISpecialActionParameter SpecialActionParameter
        {
            get { return (ISpecialActionParameter)Parameter; }
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(Int64 elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
        }
        /// <summary>
        /// 进入该状态之前
        /// </summary>
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();

        }
        /// <summary>
        /// 进入该状态
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
        }
        /// <summary>
        /// 离开该状态
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
        }
        /// <summary>
        /// 设置该状态的动作
        /// </summary>
        public override void SetStateAction()
        {
            base.SetStateAction();
            var acName = SpecialActionParameter.ActionName;
            AddAction(acName, 0, SpecialActionParameter.Duration);
        }
        /// <summary>
        /// 动作是否完成
        /// </summary>
        /// <returns>动作完成返回true，否则返回false</returns>
        public override bool OnActionFinished()
        {
            if (!base.OnActionFinished())//当基类的返回值为false时，只代表完成了当前的动作，并没有全部完成所有动作
            {
                return false;
            }
            if(mStateName =="Reborn")
            {
                ToState("Idle",null);
            }
            return true;
        }

    }
}
