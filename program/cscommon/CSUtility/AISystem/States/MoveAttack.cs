using System;

namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 移动攻击状态参数类
    /// </summary>
    public sealed class IMoveAttackParameter : StateParameter
    {
        UInt16 mSkillId;
        /// <summary>
        /// 技能ID
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.移动攻击状态参数.技能ID", CSUtility.Helper.enCSType.Common, "移动攻击状态参数技能ID")]
        [CSUtility.Event.Attribute.AllowMember("AI.移动攻击状态参数.技能ID", CSUtility.Helper.enCSType.Common, "移动攻击状态参数技能ID")]
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
    /// 移动攻击状态类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("移动攻击", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("移动攻击")]
    public class MoveAttack : State
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MoveAttack()
        {
            mStateName = "MoveAttack";
            Parameter = new IMoveAttackParameter();
        }
        /// <summary>
        /// 只读属性，移动攻击状态参数
        /// </summary>
        public IMoveAttackParameter MoveAttackParameter
        {
            get { return (IMoveAttackParameter)Parameter; }
        }
        /// <summary>
        /// 进入该状态之前
        /// </summary>
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();

            IMoveAttackParameter attackParam = this.Parameter as IMoveAttackParameter;
            if (attackParam != null)
            {
                
            }
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
