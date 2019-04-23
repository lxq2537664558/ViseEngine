using System;
using System.Collections.Generic;

namespace CSUtility.AISystem
{
    /// <summary>
    /// 声明离开状态时调用的委托事件
    /// </summary>
    /// <param name="preState">前一个状态</param>
    /// <param name="curState">当前状态</param>
    public delegate void FOnStateExit(State preState,State curState);
    /// <summary>
    /// 状态绑定的HOST接口
    /// </summary>
    public interface IStateHost
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        State CurrentState { get; set; }
        /// <summary>
        /// 目标状态
        /// </summary>
        State TargetState { get; set; }
        /// <summary>
        /// 只读属性，AI状态
        /// </summary>
        FStateMachine AIStates { get; }
        /// <summary>
        /// 只读属性，绑定的Actor
        /// </summary>
        CSUtility.Component.ActorBase Actor { get; }
        /// <summary>
        /// 状态是否远程监控
        /// </summary>
        bool StateNotify2Remote { get; set; }
        /// <summary>
        /// 初始化状态机
        /// </summary>
        /// <param name="fsmId">状态机ID</param>
        /// <param name="bResetCurrentState">重置当前状态</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        bool InitFSM(Guid fsmId, bool bResetCurrentState);
        /// <summary>
        /// 过渡到某一状态
        /// </summary>
        /// <param name="curState">当前状态</param>
        /// <param name="param">状态机参数</param>
        /// <param name="newCurState">新的当前状态</param>
        /// <param name="newTarState">新的目标状态</param>
        void FSMOnToState(State curState, StateParameter param, State newCurState, State newTarState);
        /// <summary>
        /// 设置状态机的动作
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="bLoop">是否循环</param>
        /// <param name="playRate">播放速度</param>
        /// <param name="blendDuration">混合时间</param>
        void FSMSetAction(string name,bool bLoop, float playRate, int blendDuration);
        /// <summary>
        /// 设置状态机的混合动作
        /// </summary>
        /// <param name="lowHalf">名称</param>
        /// <param name="highHalf">名称</param>
        void FSMSetBlendAction(string lowHalf, string highHalf);
        /// <summary>
        /// 通过动作名称获取状态机中的动画树
        /// </summary>
        /// <param name="name">动作名称</param>
        /// <param name="blendDuration">混合时间</param>
        /// <returns>返回状态机中相应动作名称的动画树</returns>
        CSUtility.Animation.AnimationTree FSMGetAnimationTreeByActionName(string name, int blendDuration);
        /// <summary>
        /// 设置状态机当前的动画树
        /// </summary>
        /// <param name="anim">动画树对象</param>
        void FSMSetCurrentAnimationTree(CSUtility.Animation.AnimationTree anim);
        /// <summary>
        /// 获取当前状态机的动画树
        /// </summary>
        /// <returns>返回当前状态机的动画树</returns>
        CSUtility.Animation.AnimationTree FSMGetCurrentAnimationTree();
        /// <summary>
        /// 创建动画节点
        /// </summary>
        /// <returns>返回该节点所在的动画树</returns>
        CSUtility.Animation.AnimationTree CreateAnimationNode();
        /// <summary>
        /// 创建基础动作
        /// </summary>
        /// <returns>返回创建的基础动作</returns>
        CSUtility.Animation.BaseAction CreateBaseAction();
        /// <summary>
        /// 设置动画树
        /// </summary>
        /// <param name="animtree">动画树对象</param>
        void SetAnimTree(CSUtility.Animation.AnimationTree animtree);
        /// <summary>
        /// 只读属性，逻辑时间管理器
        /// </summary>
        Helper.LogicTimerManager TimerManager { get; }

        // callback函数类型 FOnStateExits
        /// <summary>
        /// 压入状态退出
        /// </summary>
        /// <param name="cb">回调的方法</param>
        void PushStateExit(CSUtility.Helper.EventCallBack cb);
        /// <summary>
        /// 弹出状态退出
        /// </summary>
        /// <returns>返回退出时的回调</returns>
        CSUtility.Helper.EventCallBack PopStateExit();
        /// <summary>
        /// 退出该状态
        /// </summary>
        /// <param name="curState">当前状态</param>
        void OnExitedState(State curState);
        /// <summary>
        /// 状态值改变
        /// </summary>
        /// <param name="name">状态名称</param>
        /// <param name="value">写入的数据</param>
        /// <returns>成功返回true，否则返回false</returns>
        bool OnValueChanged(string name, RPC.DataWriter value);
        /// <summary>
        /// 获取高度
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="z">Z轴坐标</param>
        /// <returns>返回该点的高度</returns>
        float GetAltitude(float x, float z);
        /// <summary>
        /// 设置绑定追踪对象
        /// </summary>
        /// <param name="tracker">追踪对象ID</param>
        /// <param name="duration">持续时间</param>
        /// <param name="isAbsTracker">是否绝对追踪</param>
        void SetRigidTracker(Guid tracker, Int64 duration, bool isAbsTracker);
    }
    /// <summary>
    /// 状态参数类
    /// </summary>
    public class StateParameter : RPC.IAutoSaveAndLoad
    {
        // PrimaryState or SecondaryState，由当前的技能动作来决定是否为主要状态
        // 主要状态：在动作播放到“打击帧”之前，不允许打断
        // 次要状态：任何时刻都可以打断该动作。（或特效帧结束后打断该动作）
        bool mIsPrimaryState = true;
        /// <summary>
        /// 是否为主要状态
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("状态参数.是否为主要状态", CSUtility.Helper.enCSType.Common, "主要状态：在动作播放到“打击帧”之前，不允许打断\r\n次要状态：任何时刻都可以打断该动作。（或特效帧结束后打断该动作）")]
        [CSUtility.Event.Attribute.AllowMember("状态参数.是否为主要状态", CSUtility.Helper.enCSType.Common, "主要状态：在动作播放到“打击帧”之前，不允许打断\r\n次要状态：任何时刻都可以打断该动作。（或特效帧结束后打断该动作）")]
        public bool IsPrimaryState
        {
            get { return mIsPrimaryState; }
            set { mIsPrimaryState = value; }
        }

        float mActionPlayRate = 1;
        /// <summary>
        /// 动作播放速度
        /// </summary>
        public float ActionPlayRate
        {
            get { return mActionPlayRate; }
            set { mActionPlayRate = value; }
        }
    }
    /// <summary>
    /// 状态的动作类
    /// </summary>
    public class StateAction
    {
        string mActionName;
        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName
        {
            get { return mActionName; }
            set { mActionName = value;}
        }

        Int64 mLoopTime = 0;
        /// <summary>
        /// 循环时间
        /// </summary>
        public Int64 LoopTime
        {
            get { return mLoopTime; }
            set { mLoopTime = value; }
        }
    }
    /// <summary>
    /// 状态机动作管理器
    /// </summary>
    public class StateActionManager
    {
        StateAction mCurAction =new StateAction();
        /// <summary>
        /// 当前的动作
        /// </summary>
        public StateAction CurAction
        {
            get { return mCurAction; }
            set { mCurAction = value; }
        }

        Queue<StateAction> mTargetActions = new Queue<StateAction>();
        /// <summary>
        /// 目标状态动作队列
        /// </summary>
        public Queue<StateAction> TargetActions
        {
            get { return mTargetActions; }
            set { mTargetActions = value; }
        }
    }
    /// <summary>
    /// 状态类
    /// </summary>
    public class State
    {
        IStateHost mHost = null;
        /// <summary>
        /// 只读属性，所属角色
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("所属角色", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("所属角色", CSUtility.Helper.enCSType.Common, "")]
        public IStateHost Host
        {
            get { return mHost; }
        }
        /// <summary>
        /// 状态名称
        /// </summary>
        protected string mStateName = "DftState";
        /// <summary>
        /// 只读属性，状态名称
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.状态名称", CSUtility.Helper.enCSType.Common, "获取状态的状态名称")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.状态名称", CSUtility.Helper.enCSType.Common, "获取状态的状态名称")]
        public string StateName
        {
            get { return mStateName; }
        }

        object mTag1 = null;
        /// <summary>
        /// 状态目标1
        /// </summary>
        public object Tag1
        {
            get { return mTag1; }
            set { mTag1 = value; }
        }

        object mTag2 = null;
        /// <summary>
        /// 状态目标2
        /// </summary>
        public object Tag2
        {
            get { return mTag2; }
            set { mTag2 = value; }
        }

        object mTag3 = null;
        /// <summary>
        /// 状态目标3
        /// </summary>
        public object Tag3
        {
            get { return mTag3; }
            set { mTag3 = value; }
        }

        // 编辑器生成专用，请勿修改
        /// <summary>
        /// 状态实例名称
        /// </summary>
        protected string mInstanceStateName = "";
        /// <summary>
        /// 状态实例名称
        /// </summary>
        public string InstanceStateName
        {
            get { return mInstanceStateName; }
            set { mInstanceStateName = value; }
        }
        /// <summary>
        /// 状态昵称
        /// </summary>
        protected string mNickStateName = "";
        /// <summary>
        /// 状态昵称
        /// </summary>
        public string NickStateName
        {
            get { return mNickStateName; }
            set { mNickStateName = value; }
        }

        Int32 mFreezeStateTime = 0;
        /// <summary>
        /// 释放状态的时间
        /// </summary>
        public Int32 FreezeStateTime
        {
            get { return mFreezeStateTime; }
            set { mFreezeStateTime = value; }
        }

        Int64 mEnterStateTime = 0;
        /// <summary>
        /// 进入该状态的时间
        /// </summary>
        public Int64 EnterStateTime
        {
            get { return mEnterStateTime; }
            set { mEnterStateTime = value; }
        }

        StateActionManager mCurStateAction;
        /// <summary>
        /// AI状态的当前动作
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.当前动作", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.当前动作", CSUtility.Helper.enCSType.Common, "")]
        public StateActionManager CurStateAction
        {
            get { return mCurStateAction; }
            set { mCurStateAction = value; }
        }


        Int64 mLoopActionDuration = Int64.MaxValue;//1000,000 ==1m
        /// <summary>
        /// AI状态的循环动作时长
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.循环动作时长", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.循环动作时长", CSUtility.Helper.enCSType.Common, "")]
        public Int64 LoopActionDuration
        {
            get { return mLoopActionDuration; }
            set { mLoopActionDuration = value; }
        }
        bool mSkillCanInterrupt = false;//不可打断状态是否可以被技能打断
        /// <summary>
        /// 不可打断状态是否可以被技能打断
        /// </summary>
        public bool SkillCanInterrupt
        {
            get {return mSkillCanInterrupt;}
            set { mSkillCanInterrupt = value; }
        }

        // 是否可以打断此状态
        bool mCanInterrupt = true;
        /// <summary>
        /// 是否可以打断此状态
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("状态参数.是否可打断", CSUtility.Helper.enCSType.Common, "是否可以打断本状态")]
        [CSUtility.Event.Attribute.AllowMember("状态参数.是否可打断", CSUtility.Helper.enCSType.Common, "是否可以打断本状态")]
        public bool CanInterrupt
        {
            get
            {
                return mCanInterrupt;
            }
            set { mCanInterrupt = value; }
        }

        StateParameter mParameter;
        /// <summary>
        /// AI状态的状态参数
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.状态参数", CSUtility.Helper.enCSType.Common, "状态的状态参数")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.状态参数", CSUtility.Helper.enCSType.Common, "状态的状态参数")]
        public StateParameter Parameter
        {
            get { return mParameter; }
            set { mParameter = value; }
        }
        /// <summary>
        /// 删除动作名称
        /// </summary>
        public virtual void ClearActionName()
        {
            if (CurStateAction ==null)
            {
                return;
            }
            CurStateAction.CurAction.ActionName = string.Empty;
            CurStateAction.CurAction.LoopTime = 0;
            CurStateAction.TargetActions.Clear();
        }
        /// <summary>
        /// 增加动作
        /// </summary>
        /// <param name="name">动作名称</param>
        /// <param name="lerp">动作插值</param>
        /// <param name="looptime">循环时间</param>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.初始化当前动作", CSUtility.Helper.enCSType.Common, "初始化当前动作")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.初始化当前动作", CSUtility.Helper.enCSType.Common, "初始化当前动作")]
        public virtual void AddAction(string name,Int32 lerp,Int64 looptime =0)
        {
            if (CurStateAction ==null)
            {
                CurStateAction = new StateActionManager();
            }

            if(string.IsNullOrEmpty(CurStateAction.CurAction.ActionName))
            {
                CurStateAction.CurAction.ActionName = name;
                CurStateAction.CurAction.LoopTime = looptime;
                if (CurStateAction.CurAction.LoopTime == 0)
                {
                    LoopActionDuration = Int64.MaxValue;
                    Host.FSMSetAction(CurStateAction.CurAction.ActionName, false, 1.0f, lerp);
                }
                else
                {
                    LoopActionDuration = CurStateAction.CurAction.LoopTime * 1000;
                    Host.FSMSetAction(CurStateAction.CurAction.ActionName, true, 1.0f, lerp);
                }
            }
            else
            {
                StateAction action = new StateAction();
                action.ActionName = name;
                action.LoopTime = looptime;
                CurStateAction.TargetActions.Enqueue(action);
            }

        }
        /// <summary>
        /// 获取状态参数
        /// </summary>
        /// <param name="statename">状态名称</param>
        /// <returns>返回状态参数</returns>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.获取状态参数", CSUtility.Helper.enCSType.Common, "获取指定状态名称的状态参数")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.获取状态参数", CSUtility.Helper.enCSType.Common, "获取指定状态名称的状态参数")]
        public StateParameter GetStateParameter(string statename)
        {
            if (string.IsNullOrEmpty(statename))
            {
                return Parameter;
            }
            var state = Host.AIStates.GetState(statename);
            if (state == null)
                return null;
            return state.Parameter;
        }
        /// <summary>
        /// 设置绑定追踪对象
        /// </summary>
        /// <param name="tracker">追踪对象ID</param>
        /// <param name="duration">持续时间</param>
        /// <param name="isAbsTracker">是否绝对追踪</param>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.SetRigidTracker", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.SetRigidTracker", CSUtility.Helper.enCSType.Common, "")]
        public void SetRigidTracker(Guid tracker, Int64 duration, bool isAbsTracker)
        {
            Host.SetRigidTracker(tracker, duration, isAbsTracker);
        }
        /// <summary>
        /// 获取参数类型
        /// </summary>
        /// <returns>返回参数的类型</returns>
        public static Type GetParameterType()
        {
            return typeof(StateParameter);
        }
        /// <summary>
        /// 初始化状态
        /// </summary>
        /// <param name="host">状态的主角色</param>
        public virtual void Initialize(IStateHost host)
        {
            mHost = host;
            InitializeDefaultDelegate();
        }
        /// <summary>
        /// 初始化默认的委托事件
        /// </summary>
        public virtual void InitializeDefaultDelegate()
        {
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void Tick(Int64 elapsedMillisecond)
        {
            if (IsCurrentActionLoop())
            {
                if(LoopActionDuration !=Int64.MaxValue)
                {
                    if (LoopActionDuration < 0)
                    {
                        OnActionFinished();
                    }
                    LoopActionDuration -= elapsedMillisecond;
                }
                else
                {
                    OnActionFinished();
                }
            }
            else 
            {
                if (IsCurrentActionFinished())
                {
                    OnActionFinished();
                }
            }
            TickAnimationTree(elapsedMillisecond);

            // todo: 添加时间间隔
            OnUserTick(elapsedMillisecond);
        }
        /// <summary>
        /// 退出当前状态
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnExitState()
        {
            CanInterrupt = true;
            ClearActionName();
        }
        /// <summary>
        /// 之前的状态
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.OnPreEnterState", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.OnPreEnterState", CSUtility.Helper.enCSType.Common, "")]
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnPreEnterState()
        {
            mLoopActionDuration = Int64.MaxValue;
            mSkillCanInterrupt = false;
            CanInterrupt = true;              
        }
        /// <summary>
        /// 进入状态
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnEnterState()
        {
            SetStateAction();
        }
        /// <summary>
        /// 发送进入的状态
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnPostEnterState()
        {

        }
        /// <summary>
        /// 重新进入状态
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnReEnterState()
        {
            mSkillCanInterrupt = false;
            
        }
        /// <summary>
        /// 主要处理
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnHostDispose()
        {

        }
        /// <summary>
        /// 用户的每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnUserTick(Int64 elapsedMillisecond)
        {
        }
        /// <summary>
        /// 动作监听
        /// </summary>
        /// <param name="notifier">动作监听器</param>
        /// <param name="np">监听点</param>
        /// <returns>返回false</returns>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual bool OnActionNotify(Animation.ActionNotifier notifier, Animation.NotifyPoint np)
        {//返回true的话，强制中断所有其他的Notifier处理
            return false;
        }
        /// <summary>
        /// 更新绑定的追踪对象
        /// </summary>
        /// <param name="remainTime">剩余时间时间</param>
        /// <returns>返回false</returns>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual bool OnUpdateRigidTracker(Int64 remainTime)
        {//返回true的话，强制中断所有其他的Notifier处理
            return false;
        }

        //处理动作播放完
        /// <summary>
        /// 处理动作播放完
        /// </summary>
        /// <returns>动作播放完返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        [CSUtility.AISystem.Attribute.ToolTip("处理动作播放完")]
        public virtual bool OnActionFinished()
        {
            if (CurStateAction !=null && CurStateAction.TargetActions.Count !=0)
            {
                CurStateAction.CurAction = CurStateAction.TargetActions.Dequeue();
                if (CurStateAction.CurAction.LoopTime ==0)
                {
                    LoopActionDuration = Int64.MaxValue;
                    Host.FSMSetAction(CurStateAction.CurAction.ActionName, false, 1.0f, 0);
                    return false;
                }
                else
                {
                    LoopActionDuration = CurStateAction.CurAction.LoopTime *1000;
                    Host.FSMSetAction(CurStateAction.CurAction.ActionName, true, 1.0f, 0);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 设置状态动作
        /// </summary>
        public virtual void SetStateAction()
        {
        }

        // 状态内重载代理使用，外部不允许调用
        /// <summary>
        /// 转换到下一个状态
        /// </summary>
        /// <param name="param">状态参数</param>
        /// <param name="stateType">状态类型</param>
        public virtual void System_OnChangeStateTo(StateParameter param, string stateType)
        {
        }
        /// <summary>
        /// 目标状态
        /// </summary>
        /// <param name="target">目标状态</param>
        /// <param name="stateType">状态类型</param>
        /// <param name="param">状态参数</param>
        public static void TargetToState(IStateHost target, string stateType, StateParameter param)
        {
            if (target.CurrentState == null)
                return;

            if (param == null)
            {
                var state = target.AIStates.GetState(stateType);
                if (state == null)
                {
                    return;
                }
                param = state.Parameter;
            }

            var saved = target.StateNotify2Remote;
            target.StateNotify2Remote = true;
            target.CurrentState.ToState(stateType, param);
            target.StateNotify2Remote = saved;
        }
        /// <summary>
        /// 设置目标到状态
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="stateType">状态类型</param>
        /// <param name="param">状态参数</param>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.设置目标到状态", CSUtility.Helper.enCSType.Common, "设置对象到目标状态")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.设置目标到状态", CSUtility.Helper.enCSType.Common, "设置对象到目标状态")]
        public void SetTargetToState(IStateHost target, string stateType, StateParameter param)
        {
            TargetToState(target, stateType, param);
        }
        /// <summary>
        /// 状态转换
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <param name="param">状态参数</param>
        [CSUtility.AISystem.Attribute.AllowMember("AI状态.状态转换", CSUtility.Helper.enCSType.Common, "转换到目标状态")]
        [CSUtility.Event.Attribute.AllowMember("AI状态.状态转换", CSUtility.Helper.enCSType.Common, "转换到目标状态")]
        public void ToState(string stateType , StateParameter param)
        {
            var stt = Host.AIStates.GetStateSwitchInfo(this.StateName, stateType); // GetState(stateType);
            if (stt == null || stt.NewCurrentStateType == null)
            {
                //状态机不允许的转换
                return;
            }

            if (param == null)
            {
                param = stt.NewCurrentStateType.Parameter;
            }

            if (false == CanConvertState(Host, this, stt.NewCurrentStateType, param))
            {
                //这里调用通知，当前环境你不能干xx事
                return;
            }
            
            if (stt.NewTargetStateType != null)
                Host.TargetState = stt.NewTargetStateType;

            System_OnChangeStateTo(param, stt.NewCurrentStateType.StateName);

            //System.Diagnostics.Debug.WriteLine(string.Format("FSMChangeState from {0} to {1}", Host.CurrentState.StateName, stt.NewCurrentStateType.StateName));

            if (Host.CurrentState == stt.NewCurrentStateType && Host.CurrentState.StateName != "StayAttack")
            {
                Host.CurrentState.InitializeDefaultDelegate();
                Host.CurrentState.Parameter = param;
                Host.CurrentState.OnReEnterState();
                if (Host.CurrentState.StateName == "Walk")
                {
                    Host.FSMOnToState(this, param, stt.NewCurrentStateType, stt.NewTargetStateType);
                }
            }
            else
            {
                Host.FSMOnToState(this, param, stt.NewCurrentStateType, stt.NewTargetStateType);

                //Host.CurrentState.OnExitState();
                // 退出后初始化默认代理
                //Host.CurrentState.InitializeDefaultDelegate();

                var preState = Host.CurrentState;
                Host.CurrentState = stt.NewCurrentStateType;
                if (Host.CurrentState.Parameter == null)
                {
                    if (Host.CurrentState.Parameter.GetType() == param.GetType())
                    {
                        Host.CurrentState.Parameter = param;
                    }
                    
                }
                else if (Host.CurrentState.Parameter.GetType() != param.GetType())
                {

                }
                else
                {
                    Host.CurrentState.Parameter = param;
                }

                try
                {
                    preState.OnExitState();
                    Host.OnExitedState(preState);
                    preState.InitializeDefaultDelegate();
                    Host.CurrentState.OnPreEnterState();
                    Host.CurrentState.OnEnterState();
                    Host.CurrentState.OnPostEnterState();

                    CSUtility.Helper.EventCallBack cb = Host.PopStateExit();
                    if (cb != null)
                    {
                        var callee = cb.GetCallee() as FOnStateExit;
                        if(callee != null)
                            callee(preState, Host.CurrentState);  //调用委托
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
        }
        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="key">按键值</param>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Client)]
        [AISystem.Attribute.ToolTip("按键按下")]
        public virtual void OnKeyDown(int key)
        {
            
        }
        /// <summary>
        /// 按键弹起
        /// </summary>
        /// <param name="key">按键值</param>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Client)]
        [AISystem.Attribute.ToolTip("按键弹起")]
        public virtual void OnKeyUp(int key)
        {

        }
        /// <summary>
        /// 处理被角色攻击
        /// </summary>
        /// <param name="attacker">状态主体</param>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        [AISystem.Attribute.ToolTip("处理被角色攻击")]
        public virtual void OnBeHurt(IStateHost attacker)
        {
            
        }
        /// <summary>
        /// 进入新地图
        /// </summary>
        /// <param name="mapId">地图ID</param>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        [AISystem.Attribute.ToolTip("进入新地图")]
        public virtual void OnEnterMap(Guid mapId)
        {

        }
        /// <summary>
        /// 进入触发器
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Server)]
        [AISystem.Attribute.ToolTip("进入Trigger")]
        public virtual void OnEnterTrigger()
        {

        }
        /// <summary>
        /// 离开Trigger
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Server)]
        [AISystem.Attribute.ToolTip("离开Trigger")]
        public virtual void OnLeaveTrigger()
        {

        }
        /// <summary>
        /// 当角色创建完
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Server)]
        [AISystem.Attribute.ToolTip("当角色创建完")]
        public virtual void OnRoleCreatEnd()
        {

        }
        /// <summary>
        /// 当走到最后的寻路点
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Server)]
        [AISystem.Attribute.ToolTip("当走到最后的寻路点")]
        public virtual void OnArrivedLastGroupPos()
        {

        }
        /// <summary>
        /// 处理被角色攻击
        /// </summary>
        /// <param name="target">攻击目标</param>
        /// <param name="OnTargetDeath">目标的是否死亡</param>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Server)]
        [AISystem.Attribute.ToolTip("处理被角色攻击")]
        public virtual void OnTargetDeath(IStateHost target, int OnTargetDeath)
        {
            
        }

        System.Random mRandom = new Random();
        //    [AISystem.Attribute.OverrideInterface(Attribute.enCSType.Common)]
        //   [AISystem.Attribute.ToolTip("处理角色释放技能")]
        /// <summary>
        /// 处理角色释放技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="tarPos">目标位置</param>
        /// <param name="summonPos">召唤位置</param>
        /// <param name="runeLevel">符文等级</param>
        /// <returns>返回true</returns>
        public virtual bool DoFireSkill(UInt16 skillId, SlimDX.Vector3 tarPos, SlimDX.Vector3 summonPos,UInt16 runeLevel =0)
        {
            return true;
        }
        /// <summary>
        /// 每帧调用的监听器
        /// </summary>
        /// <param name="name">监听器名称</param>
        [AISystem.Attribute.AllowMember("AI状态.DoTickNotifier", CSUtility.Helper.enCSType.Common, "")]
        [Event.Attribute.AllowMember("AI状态.DoTickNotifier", CSUtility.Helper.enCSType.Common, "")]
        [AISystem.Attribute.ToolTip("Tick指定的Notifier")]
        public void DoTickNotifier(string name)
        {
            var anim = Host.FSMGetCurrentAnimationTree();
            if (anim != null)
            {
                ProcNotifiers(anim, name);
            }
        }
        /// <summary>
        /// 添加计时器
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="interval">计时时间</param>
        [AISystem.Attribute.ToolTip("为角色添加一个逻辑计时器")]
        [AISystem.Attribute.AllowMember("AI状态.添加计时器", CSUtility.Helper.enCSType.Common, "为角色添加一个逻辑计时器AddLogicTimer")]
        [Event.Attribute.AllowMember("AI状态.添加计时器", CSUtility.Helper.enCSType.Common, "为角色添加一个逻辑计时器AddLogicTimer")]
        public void AddLogicTimer(string name,Int64 interval)
        {
            Host.TimerManager.AddLogicTimer(name, interval, this.State_OnTimer, CSUtility.Helper.enCSType.Common);
        }
        /// <summary>
        /// 添加客户端计时器
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="interval">计时时间</param>
        [AISystem.Attribute.ToolTip("为角色添加一个逻辑计时器")]
        [AISystem.Attribute.AllowMember("AI状态.添加客户端计时器", CSUtility.Helper.enCSType.Client, "为角色添加一个逻辑计时器AddClientLogicTimer")]
        [Event.Attribute.AllowMember("AI状态.添加客户端计时器", CSUtility.Helper.enCSType.Client, "为角色添加一个逻辑计时器AddClientLogicTimer")]
        public void AddClientLogicTimer(string name, Int64 interval)
        {
            Host.TimerManager.AddLogicTimer(name, interval, this.State_OnTimer, CSUtility.Helper.enCSType.Client);
        }
        /// <summary>
        /// 添加服务器计时器
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="interval">计时时间</param>
        [AISystem.Attribute.ToolTip("为角色添加一个逻辑计时器")]
        [AISystem.Attribute.AllowMember("AI状态.添加服务器计时器", CSUtility.Helper.enCSType.Server, "为角色添加一个逻辑计时器")]
        [Event.Attribute.AllowMember("AI状态.添加服务器计时器", CSUtility.Helper.enCSType.Server, "为角色添加一个逻辑计时器")]
        public void AddServerLogicTimer(string name, Int64 interval)
        {
            Host.TimerManager.AddLogicTimer(name, interval, this.State_OnTimer, CSUtility.Helper.enCSType.Server);
        }
        /// <summary>
        /// 计时器归零
        /// </summary>
        /// <param name="name">计时器名称</param>
        [AISystem.Attribute.ToolTip("角色逻辑计时器归零")]
        [AISystem.Attribute.AllowMember("AI状态.计时器归零", CSUtility.Helper.enCSType.Common, "角色计时器归零")]
        [Event.Attribute.AllowMember("AI状态.计时器归零", CSUtility.Helper.enCSType.Common, "角色计时器归零")]
        public void ResetCurLogicTimer(string name)
        {
            Host.TimerManager.ResetCurLogicTimer(name);
        }
        /// <summary>
        /// 移除计时器
        /// </summary>
        /// <param name="name">角色名称</param>
        [AISystem.Attribute.ToolTip("移除角色制定名字的逻辑计时器")]
        [AISystem.Attribute.AllowMember("AI状态.移除计时器", CSUtility.Helper.enCSType.Common, "移除角色制定名字的逻辑计时器")]
        [Event.Attribute.AllowMember("AI状态.移除计时器", CSUtility.Helper.enCSType.Common, "移除角色制定名字的逻辑计时器")]
        public void RemoveLogicTimer(string name)
        {
            Host.TimerManager.RemoveLogicTimer(name);
        }

        private bool State_OnTimer(string name)
        {
            return this.OnTimer(name);
        }
        /// <summary>
        /// 定时器触发
        /// </summary>
        /// <param name="name">定时器名称</param>
        /// <returns>返回false</returns>
        [CSUtility.AISystem.Attribute.ToolTip("定时器触发")]
        [CSUtility.AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual bool OnTimer(string name)
        {
            return false;
        }
        /// <summary>
        /// 用户操作行为
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="skillId">技能名称</param>
        /// <param name="arg">行为对象</param>
        [CSUtility.AISystem.Attribute.ToolTip("用户操作行为")]
        [CSUtility.AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Client)]
        public virtual void OnUserAction(string name,UInt16 skillId,object arg)
        {
            
        }

        #region 可以调用的函数
        // 取得里本角色最近的角色的距离
        /// <summary>
        /// 取得最近角色距离
        /// </summary>
        /// <returns>返回0</returns>
        [AISystem.Attribute.AllowMember("AI状态.取得最近角色距离", CSUtility.Helper.enCSType.Common, "取得里本角色最近的角色的距离")]
        [Event.Attribute.AllowMember("AI状态.取得最近角色距离", CSUtility.Helper.enCSType.Common, "取得里本角色最近的角色的距离")]
        public float GetNearestRoleDistance()
        {

            return 0;
        }
        /// <summary>
        /// 设置状态机的动作
        /// </summary>
        /// <param name="name">动作名称</param>
        /// <param name="bLoop">是否循环</param>
        /// <param name="playRate">动作播放速率</param>
        [AISystem.Attribute.AllowMember("AI状态.设置动作", CSUtility.Helper.enCSType.Common, "设置动作\r\nname: 动作名称\r\nbLoop: 是否循环\r\nplayRage: 动作播放速率")]
        [Event.Attribute.AllowMember("AI状态.设置动作", CSUtility.Helper.enCSType.Common, "设置动作\r\nname: 动作名称\r\nbLoop: 是否循环\r\nplayRage: 动作播放速率")]
        public void FSMSetAction(string name, bool bLoop, float playRate)
        {

            FSMSetAction(name, bLoop, playRate, 100);
        }
        /// <summary>
        /// 设置动作
        /// </summary>
        /// <param name="name">动作名称</param>
        /// <param name="bLoop">是否循环</param>
        /// <param name="playRate">动作播放速率</param>
        /// <param name="blendDuration">动作融合时常</param>
        [AISystem.Attribute.AllowMember("AI状态.设置动作", CSUtility.Helper.enCSType.Common, "设置动作\r\nname: 动作名称\r\nbLoop: 是否循环\r\nplayRage: 动作播放速率\r\nblendDuration: 动作融合时常")]
        [Event.Attribute.AllowMember("AI状态.设置动作", CSUtility.Helper.enCSType.Common, "设置动作\r\nname: 动作名称\r\nbLoop: 是否循环\r\nplayRage: 动作播放速率\r\nblendDuration: 动作融合时常")]
        public void FSMSetAction(string name, bool bLoop, float playRate, int blendDuration)
        {
            Host.FSMSetAction(name, bLoop, playRate, blendDuration);
        }
        #endregion
        /// <summary>
        /// 处理是否能够进行状态转换
        /// </summary>
        /// <param name="host">主状态</param>
        /// <param name="fromState">从哪个状态转换</param>
        /// <param name="toState">转换到的状态</param>
        /// <param name="param">状态参数</param>
        /// <returns>可以转换返回true，否则返回false</returns>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        [AISystem.Attribute.ToolTip("处理是否能够进行状态转换")]
        public virtual bool CanConvertState( IStateHost host, State fromState, State toState,StateParameter param)
        {
            if (toState.StateName == "Death")
                return true;
            if(SkillCanInterrupt ==true && toState.StateName == "StayAttack")//本状态是否可以被技能打断
                return true;
            if (CanInterrupt == false)
            {
                return false;
            }
            return true;
        }

        Int64 mNotifyPrevTime;
        /// <summary>
        /// 只读属性，上一个监听时间
        /// </summary>
        public Int64 NotifyPrevTime
        {
            get { return mNotifyPrevTime; }
        }
        /// <summary>
        /// 动作树的每帧循环
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        protected void TickAnimationTree(Int64 elapsedMillisecond)
        {
            var anim = Host.FSMGetCurrentAnimationTree();
            if (anim != null)
            {
                if (anim.CurNotifyTime > anim.Action.Duration && anim.Action.GetLoop() ==true)
                {
                    anim.CurNotifyTime -= anim.Action.Duration;
                }
                mNotifyPrevTime = anim.CurNotifyTime;
                anim.CurNotifyTime += (Int64)(((float)elapsedMillisecond) * anim.GetPlayRate());
            }
        }
        /// <summary>
        /// 监听过程
        /// </summary>
        /// <param name="anim">动画树</param>
        /// <param name="name">监听器名称</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool ProcNotifiers(CSUtility.Animation.AnimationTree anim, string name )
        {
            if (anim.Action != null)
            {
                Animation.ActionNotifier ntf = anim.Action.GetNotifier(name);
                if(ntf!=null)
                {
                    var nplist = ntf.GetNotifyPoints(mNotifyPrevTime, anim.CurNotifyTime);
                    if (nplist != null)

                    {
                        foreach (var i in nplist)
                        {
                            if (OnActionNotify(ntf, i) == false)
                                return false;
                        }
                        return true;
                    }
                }

            }

            var subAnims = anim.GetAnimations();
            foreach (var i in subAnims)
            {
                if (i != null)
                {
                    if (false == ProcNotifiers(i, name ))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 当前动作是否循环
        /// </summary>
        /// <returns>如果动作循环返回true，否则返回false</returns>
        public bool IsCurrentActionLoop()
        {
            var anim = Host.FSMGetCurrentAnimationTree();
            if (anim == null)
                return false;
            if (anim.Action.GetLoop())
                return true;
            return false;
        }
        /// <summary>
        /// 当前动作是否结束
        /// </summary>
        /// <returns>结束返回true，否则返回false</returns>
        public bool IsCurrentActionFinished()
        {
            var anim = Host.FSMGetCurrentAnimationTree();
            if (anim == null)
                return true;
            bool isFinished = anim.IsActionFinished();
            return isFinished;
        }
    
        //private CSUtility.Animation.AnimationTree TestBuildAnimationTree()
        //{
        //    CSUtility.Animation.AnimationTree anim1 = null;//new CSUtility.Animation.AnimationTree();
        //    CSUtility.Animation.BaseAction action1 = null;//new CSUtility.Animation.BaseAction();
        //    action1.ActionName = "Walk";
        //    action1.BlendFactor = 0.5;
        //    anim1.Action = action1;

        //    CSUtility.Animation.AnimationTree anim2 = null;
        //    CSUtility.Animation.BaseAction action2 = null;
        //    action2.ActionName = "Death";
        //    action2.BlendFactor = 0.5;
        //    anim2.Action = action2;

        //    CSUtility.Animation.AnimationTree blender = null;
        //    blender.SetAnimations(new List<CSUtility.Animation.AnimationTree>{
        //        anim1,
        //        anim2,
        //    });
            

        //    return blender;
        //}
    }
    /// <summary>
    /// 状态机类
    /// </summary>
    public class FStateMachine
    {
        /// <summary>
        /// 状态转换信息类
        /// </summary>
        public class StateSwitchInfo
        {
            State mNewCurrentStateType;
            /// <summary>
            /// 只读属性，新的当前状态类型
            /// </summary>
            public State NewCurrentStateType
            {
                get { return mNewCurrentStateType; }
            }
            State mNewTargetStateType;
            /// <summary>
            /// 只读属性，新的目标状态类型
            /// </summary>
            public State NewTargetStateType
            {
                get { return mNewTargetStateType; }
            }
            /// <summary>
            /// 带参构造函数
            /// </summary>
            /// <param name="newCurrent">新的当前状态</param>
            /// <param name="newTarget">新的目标状态对象</param>
            public StateSwitchInfo(State newCurrent, State newTarget)
            {
                mNewCurrentStateType = newCurrent;
                mNewTargetStateType = newTarget;
            }
        }

        IStateHost mHost;
        /// <summary>
        /// 只读属性，主状态
        /// </summary>
        public IStateHost Host
        {
            get { return mHost; }
        }

        State mDefaultState = null;
        /// <summary>
        /// 只读属性，默认状态
        /// </summary>
        public State DefaultState
        {
            get { return mDefaultState; }
        }

        FStateMachineTemplate mFSMTemplate;
        /// <summary>
        /// 只读属性，状态机模板
        /// </summary>
        public FStateMachineTemplate FSMTemplate
        {
            get { return mFSMTemplate; }
        }
        /// <summary>
        /// 初始化状态机
        /// </summary>
        /// <param name="host">主状态</param>
        /// <param name="fsmTpl">状态机模板</param>
        /// <param name="csType">服务器客户端类型</param>
        public void InitFSM(IStateHost host, FStateMachineTemplate fsmTpl, CSUtility.Helper.enCSType csType)
        {
            mFSMTemplate = fsmTpl;
            mHost = host;

            mAllState.Clear();
            mStateSwitchManager.Clear();

            if (fsmTpl == null)
                return;

            var fsmAssembly = fsmTpl.GetAssembly();
            if (fsmAssembly == null)
            {
                System.Diagnostics.Debug.Assert(false, "FStateMachine.InitFSM Failed, Assembly is NULL");
                return;
            }

            if (fsmTpl.SwitchData == null)
                return;
            
            // 从模板中取得所有的状态并生成实例
            foreach (var i in fsmTpl.AllState)
            {
                State stt = fsmAssembly.CreateInstance(i.Value.FullName) as State;
                if(stt==null)
                {
                    //报警
                    continue;
                }
                stt.Initialize(mHost);
                mAllState.Add(i.Key, stt);
            }

            // 从模板中取得所有状态转换信息
            foreach (var i in fsmTpl.SwitchData.StateSwitchInfoData)
            {
                var ssInfo = new StateSwitchInfo(GetState(i.Value.mNewCurrent), GetState(i.Value.mNewTarget));
                mStateSwitchManager.Add(i.Key, ssInfo);
            }

            // 取得默认状态
            mDefaultState = GetState(fsmTpl.SwitchData.DefaultState);
            if (mDefaultState != null)
            {
                try
                {
                    this.Host.CurrentState = mDefaultState;
                    mDefaultState.OnPreEnterState();
                    mDefaultState.OnEnterState();
                    mDefaultState.OnPostEnterState();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
        }

        Dictionary<string, State> mAllState = new Dictionary<string, State>();
        /// <summary>
        /// 只读属性，所有的状态列表
        /// </summary>
        public Dictionary<string, State> AllState
        {
            get { return mAllState; }
        }
        /// <summary>
        /// 注册状态
        /// </summary>
        /// <param name="t">状态名称</param>
        /// <param name="s">状态对象</param>
        public void RegState(string t, State s)
        {
            mAllState.Add(t, s);
        }
        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="t">状态名称</param>
        public void UnRegState(string t)
        {
            mAllState.Remove(t);
        }
        /// <summary>
        /// 获取相应类型的状态
        /// </summary>
        /// <param name="type">状态类型名称</param>
        /// <returns>返回相应的状态</returns>
        public State GetState(string type)
        {
            if (string.IsNullOrEmpty(type))
                return null;
            if (mAllState == null)
                return null;
            State state;
            if (mAllState.TryGetValue(type, out state))
                return state;
            return null;
        }

        Dictionary<KeyValuePair<string, string>, StateSwitchInfo> mStateSwitchManager = new Dictionary<KeyValuePair<string, string>, StateSwitchInfo>();
        /// <summary>
        /// 获取状态转换信息
        /// </summary>
        /// <param name="curState">当前状态</param>
        /// <param name="tarState">目标状态</param>
        /// <returns>返回状态转换的信息</returns>
        public StateSwitchInfo GetStateSwitchInfo(string curState, string tarState)
        {
            StateSwitchInfo cvtStateInfo;
            if( mStateSwitchManager.TryGetValue( new KeyValuePair<string, string>(curState,tarState) , out cvtStateInfo )==false)
                return null;
            return cvtStateInfo;
        }
    }
}
