using System;

namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 站桩引导状态参数
    /// </summary>
    public sealed class IStayChannelParameter : StateParameter
    {
        UInt16 mSkillId;
        /// <summary>
        /// 技能ID
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.站桩引导.技能ID", CSUtility.Helper.enCSType.Common, "获取或设置站桩引导技能ID")]
        [CSUtility.Event.Attribute.AllowMember("AI.站桩引导.技能ID", CSUtility.Helper.enCSType.Common, "获取或设置站桩引导技能ID")]
        public UInt16 SkillId
        {
            get { return mSkillId; }
            set { mSkillId = value; }
        }

        //UInt16 mRuneId;
        //[CSUtility.AISystem.Attribute.AllowMember(CSUtility.Helper.enCSType.Common)]
        //public UInt16 RuneId
        //{
        //    get { return mRuneId; }
        //    set { mRuneId = value; }
        //}

        SlimDX.Vector3 mtarPos = SlimDX.Vector3.UnitZ;
        /// <summary>
        /// 目标位置
        /// </summary>
        public SlimDX.Vector3 tarPos
        {
            get { return mtarPos; }
            set
            {
                mtarPos = value;
            }
        }
    }
    /// <summary>
    /// 站桩引导类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("站桩引导", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("站桩引导")]
    public class StayChannel : State
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public StayChannel()
        {
            mStateName = "StayChannel";
            Parameter = new IStayChannelParameter();
        }
        /// <summary>
        /// 只读属性，站桩引导参数
        /// </summary>
        public IStayChannelParameter StayChannelParameter
        {
            get { return (IStayChannelParameter)Parameter; }
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
            
            //Host.FSMSetAction(rune.ChannelAction, true, 1.0f, 100);
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
    }
}
