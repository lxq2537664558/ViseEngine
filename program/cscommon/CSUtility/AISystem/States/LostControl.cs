using System;

namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 失去控制的状态的参数
    /// </summary>
    public sealed class ILostControlParameter : StateParameter
    {
        
    }
    /// <summary>
    /// 失去控制的状态类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("失去控制", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("失去控制")]
    public class LostControl : State
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LostControl()
        {
            mStateName = "LostControl";
            Parameter = new ILostControlParameter();
        }

        /// <summary>
        /// 只读属性，失去控制状态的参数
        /// </summary>
        public ILostControlParameter LostControlParameter
        {
            get { return (ILostControlParameter)Parameter; }
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
        /// 进入该状态
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
        }
    }
}
