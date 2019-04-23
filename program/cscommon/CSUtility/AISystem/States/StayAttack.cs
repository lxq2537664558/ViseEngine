using System;

namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 站桩攻击状态的参数类
    /// </summary>
    public sealed class IStayAttackParameter : StateParameter
    {
        UInt16 mSkillId =UInt16.MaxValue;
        /// <summary>
        /// 技能ID
        /// </summary>
        public UInt16 SkillId
        {
            get { return mSkillId; }
            set { mSkillId = value; }
        }

        UInt16 mRuneId;
        /// <summary>
        /// 符文ID
        /// </summary>
        public UInt16 RuneId
        {
            get { return mRuneId; }
            set { mRuneId = value; }
        }

        UInt32 mTarSingle;
        /// <summary>
        /// 目标值
        /// </summary>
        public UInt32 TarSingle
        {
            get { return mTarSingle; }
            set { mTarSingle = value; }
        }

        float mTarAngle = float.MaxValue;
        /// <summary>
        /// 目标角度
        /// </summary>
        public float TarAngle
        {
            get { return mTarAngle; }
            set { mTarAngle = value; }
        }

        UInt16 mRuneLevel=0;
        /// <summary>
        /// 符文等级
        /// </summary>
        public UInt16 RuneLevel
        {
            get { return mRuneLevel; }
            set { mRuneLevel = value; }
        }

        int mRuneHandle;    // 每次发射技能的时候，分配一个随机的Handle。
        /// <summary>
        /// 每次发射技能的时候，分配一个随机的Handle。
        /// </summary>
        public int RuneHandle
        {
            get { return mRuneHandle; }
            set { mRuneHandle = value; }
        }
        SlimDX.Vector3 mtarDir = SlimDX.Vector3.UnitZ;
        /// <summary>
        /// 目标方向
        /// </summary>
        public SlimDX.Vector3 tarDir
        {
            get { return mtarDir; }
            set
            {
                mtarDir = value;
            }
        }

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

        SlimDX.Vector3 mSummonPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 召唤位置
        /// </summary>
        public SlimDX.Vector3 SummonPos
        {
            get { return mSummonPos; }
            set
            {
                mSummonPos = value;
            }
        }

        Int64 mDuration =0;
        /// <summary>
        /// 攻击时长
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.站桩攻击.时长", CSUtility.Helper.enCSType.Common, "获取或设置站桩攻击时长")]
        [CSUtility.Event.Attribute.AllowMember("AI.站桩攻击.时长", CSUtility.Helper.enCSType.Common, "获取或设置站桩攻击时长")]
        public Int64 Duration
        {
            get { return mDuration; }
            set 
            {
                mDuration = value;
            }
        }
    }
    /// <summary>
    /// 站桩攻击类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("站桩攻击", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("站桩攻击")]
    public class StayAttack : State
    {
        /// <summary>
        /// 系统随机数
        /// </summary>
        public static System.Random mRandom = new System.Random();
        /// <summary>
        /// 构造函数
        /// </summary>
        public StayAttack()
        {
            mStateName = "StayAttack";
            Parameter = new IStayAttackParameter();
        }

        string mActionName = "StayAttack";
        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName
        {
            get { return mActionName; }
            set { mActionName = value; }
        }
        bool mLoopAction = false;
        /// <summary>
        /// 动作是否循环
        /// </summary>
        public bool LoopAction
        {
            get { return mLoopAction; }
            set { mLoopAction = value; }
        }

        UInt16 mDamageCalculationCount = 0;
        /// <summary>
        /// 伤害计算值
        /// </summary>
        public UInt16 DamageCalculationCount
        {
            get { return mDamageCalculationCount; }
            set { mDamageCalculationCount = value; }
        }
        /// <summary>
        /// 当前特效监听点名称
        /// </summary>
        public string mCrrentEffectNotifyName = "";
        /// <summary>
        /// 只读属性，站桩攻击参数
        /// </summary>
        public IStayAttackParameter StayAttackParameter
        {
            get { return (IStayAttackParameter)Parameter; }
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

        }
        /// <summary>
        /// 进入该状态
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
            CanInterrupt = false;

        }
        /// <summary>
        /// 重新进入该状态
        /// </summary>
        public override void OnReEnterState()
        {
            base.OnReEnterState();
            CanInterrupt = false;
        }
        /// <summary>
        /// 离开该状态
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            mBoolInEndAction = false;
            mBoolInLoopAction = Int64.MaxValue;
            mLoopAction = false;
         //   mLiveTime = 0;
            mActionName = "StayAttack";
            StayAttackParameter.Duration = 0;
        }
        /// <summary>
        /// 是否是最后一个动作，默认为false
        /// </summary>
        public bool mBoolInEndAction = false;
        /// <summary>
        /// 动作循环次数，默认为无限循环
        /// </summary>
        public Int64 mBoolInLoopAction = Int64.MaxValue;
        /// <summary>
        /// 动作结束
        /// </summary>
        /// <returns>动作结束返回true，否则返回false</returns>
        public override bool OnActionFinished()
        {
            if (!base.OnActionFinished())
            {
                OnActionChanged(true);
                return false;
            }
            else
            {
                CanInterrupt = true;
                this.ToState("Idle", null);
            }
            return true;
        }
        /// <summary>
        /// 释放技能
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnFireSkill()
        {
        }
        
        /// <summary>
        /// 动作改变
        /// </summary>
        /// <param name="loop">动作是否循环，缺省为false</param>
         public virtual void OnActionChanged(bool loop =false)
        {

         }
        /// <summary>
        /// 设置该状态的动作
        /// </summary>
         public override void SetStateAction()
         {
            if (StayAttackParameter.SkillId == UInt16.MaxValue)
                Host.FSMSetAction("StayAttack", false, Parameter.ActionPlayRate, 100);
        }
    }
}
