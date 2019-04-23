namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 死亡状态参数
    /// </summary>
    public sealed class IDeathParameter : StateParameter
    {
        uint mKillerId = uint.MaxValue;
        /// <summary>
        /// 击杀者ID
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("状态参数.击杀者id", CSUtility.Helper.enCSType.Common, "主要状态：击杀者id")]
        [CSUtility.Event.Attribute.AllowMember("状态参数.击杀者id", CSUtility.Helper.enCSType.Common, "主要状态：击杀者id")]
        public uint KillerId
        {
            get { return mKillerId; }
            set { mKillerId = value; }
        }
    }
    /// <summary>
    /// 死亡状态类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("死亡", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("死亡")]
    public class Death : State
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Death()
        {
            mStateName = "Death";
            Parameter = new IDeathParameter();
        }
        /// <summary>
        /// 进入该状态之前
        /// </summary>
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
            Host.FSMSetAction("Death", false, 2.0f, 100);
            mProcDeath = false;
        }
        /// <summary>
        /// 进入状态
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
        }
        /// <summary>
        /// 处理死亡状态
        /// </summary>
        [AISystem.Attribute.AllowMember("AI.死亡状态.处理死亡", CSUtility.Helper.enCSType.Common, "处理死亡")]
        [Event.Attribute.AllowMember("AI.死亡状态.处理死亡", CSUtility.Helper.enCSType.Common, "处理死亡")]
        public virtual void ProcDeath()
        {

        }

        bool mProcDeath = false;
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
        }
        /// <summary>
        /// 死亡是否动作完成
        /// </summary>
        /// <returns>动作完成返回true，否则返回false</returns>
        public override bool OnActionFinished()
        {
            base.OnActionFinished();

            if (mProcDeath == false)
            {
                mProcDeath = true;
                ProcDeath();
            }
            return true;
        }
    }
}
