namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 待机状态参数类
    /// </summary>
    public sealed class IIdleStateParameter : StateParameter
    {
        
    }
    /// <summary>
    /// 待机状态类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("待机状态", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("待机状态")]
    public class Idle : State
    {
        string mActionName = "Idle";
        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName
        {
            get { return mActionName; }
            set { mActionName = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Idle()
        {
            mStateName = "Idle";
            Parameter = new IIdleStateParameter();
        }
        /// <summary>
        /// 进入待机状态前
        /// </summary>
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
       //     LoopActionDuration = 5000000;
            SetStateAction();
        }
        /// <summary>
        /// 设置待机状态动作
        /// </summary>
        public override void SetStateAction()
        {
            Host.FSMSetAction("Idle", true, 1.0f, 100);
        }

        /// <summary>
        /// 进入待机状态
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
        }
        /// <summary>
        /// 动作是否完成
        /// </summary>
        /// <returns>动作完成返回true，否则返回false</returns>
        public override bool OnActionFinished()
        {
            base.OnActionFinished();
            return true;
        //    LoopActionDuration = 5000000;
        }
    }
}
