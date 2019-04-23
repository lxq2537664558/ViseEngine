using System;

namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 被攻击状态的参数
    /// </summary>
    public sealed class IBeAttackParameter : StateParameter
    {
        SlimDX.Vector3 mAttackerPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 攻击位置
        /// </summary>
        public SlimDX.Vector3 AttackerPos
        {
            get { return mAttackerPos; }
            set
            {
                mAttackerPos = value;
            }
        }

        UInt16 mSkillId = 65535;
        /// <summary>
        /// 技能ID
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.受攻击状态参数.技能ID", CSUtility.Helper.enCSType.Common, "技能ID")]
        [CSUtility.Event.Attribute.AllowMember("AI.受攻击状态参数.技能ID", CSUtility.Helper.enCSType.Common, "技能ID")]
        public UInt16 SkillId
        {
            get { return mSkillId; }
            set { mSkillId = value; }
        }

        //byte mRuneLevel = 0;
        //[CSUtility.AISystem.Attribute.AllowMember(CSUtility.Helper.enCSType.Common)]
        //public byte RuneLevel
        //{
        //    get { return mRuneLevel; }
        //    set { mRuneLevel = value; }
        //}
        //UInt16 mRuneId = 65535;
        //[CSUtility.AISystem.Attribute.AllowMember(CSUtility.Helper.enCSType.Common)]
        //public UInt16 RuneId
        //{
        //    get { return mRuneId; }
        //    set { mRuneId = value; }
        //}

        Int64 mDuration = -1;
        /// <summary>
        /// 攻击时常
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.受攻击状态参数.时常", CSUtility.Helper.enCSType.Common, "时常")]
        [CSUtility.Event.Attribute.AllowMember("AI.受攻击状态参数.时常", CSUtility.Helper.enCSType.Common, "时常")]
        public Int64 Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }

        int mBlendDuration = 100;
        /// <summary>
        /// 融合时常
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.受攻击状态参数.融合时常", CSUtility.Helper.enCSType.Common, "融合时常")]
        [CSUtility.Event.Attribute.AllowMember("AI.受攻击状态参数.融合时常", CSUtility.Helper.enCSType.Common, "融合时常")]
        public int BlendDuration
        {
            get { return mBlendDuration; }
            set { mBlendDuration = value; }
        }

        UInt32 mAttackerSingleId;
        /// <summary>
        /// 攻击者的单个ID
        /// </summary>
        public UInt32 AttackerSingleId
        {
            get { return mAttackerSingleId; }
            set { mAttackerSingleId = value; }
        }
        string mActionName = "";
        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName
        {
            get { return mActionName; }
            set { mActionName = value; }
        }
        string mLoopActionName = string.Empty;
        /// <summary>
        /// 循环动作名称
        /// </summary>
        public string LoopActionName
        {
            get { return mLoopActionName; }
            set { mLoopActionName = value; }
        }
        string mEndActionName = string.Empty;
        /// <summary>
        /// 终止动作的名称
        /// </summary>
        public string EndActionName
        {
            get { return mEndActionName; }
            set { mEndActionName = value; }
        }
    }
    /// <summary>
    /// 被攻击的状态类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("被攻击", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("被攻击")]
    public class BeAttack : State
    {
        /// <summary>
        /// 构造函数 
        /// </summary>
        public BeAttack()
        {
            mStateName = "BeAttack";
            Parameter = new IBeAttackParameter();
        }
        /// <summary>
        /// 只读属性，被攻击状态的参数
        /// </summary>
        public IBeAttackParameter BeAttackParameter
        {
            get { return (IBeAttackParameter)Parameter; }
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
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
            CanInterrupt = false;
        }
        /// <summary>
        /// 进入该状态
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();

        }
        /// <summary>
        /// 动作是否完成
        /// </summary>
        /// <returns>动作完成返回true，否则返回false</returns>
        public override bool OnActionFinished()
        {
            if (base.OnActionFinished())
            {
                ExitBeAttack();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 退出该状态
        /// </summary>
        public virtual void ExitBeAttack()
        {
            CanInterrupt = true;
            this.ToState("Idle", null);
        }
    }
}
