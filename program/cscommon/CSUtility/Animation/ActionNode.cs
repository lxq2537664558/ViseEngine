using System;
using System.Collections.Generic;

namespace CSUtility.Animation
{
    /// <summary>
    /// 动作节点类
    /// </summary>
    public class ActionNode : AnimNode, BaseAction
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ActionNode()
        {
            base.Action = this;
        }
        string mActionName;
        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName
        {
            get { return mActionName; }
            set
            {
                mActionName = value;
                //真的加载动作文件
                //LoadActionNotifier(mActionName);
                mActionSource = ActionNodeManager.Instance.GetActionSource(value, false, Helper.enCSType.Server);

            }
        }

        AxisRootmotionType mXRootmotionType;
        /// <summary>
        /// X方向根动作类型
        /// </summary>
        public AxisRootmotionType XRootmotionType
        {
            get { return mXRootmotionType; }
            set { mXRootmotionType = value; }
        }
        AxisRootmotionType mYRootmotionType;
        /// <summary>
        /// Y方向根动作类型
        /// </summary>
        public AxisRootmotionType YRootmotionType
        {
            get { return mYRootmotionType; }
            set { mYRootmotionType = value; }
        }
        AxisRootmotionType mZRootmotionType;
        /// <summary>
        /// Z方向根动作类型
        /// </summary>
        public AxisRootmotionType ZRootmotionType
        {
            get { return mZRootmotionType; }
            set { mZRootmotionType = value; }
        }
        EActionPlayerMode mPlayerMode;
        /// <summary>
        /// 播放模式
        /// </summary>
        public EActionPlayerMode PlayerMode
        {
            get { return mPlayerMode; }
            set { mPlayerMode = value; }
        }
        /// <summary>
        /// 只读属性，持续时间
        /// </summary>
        public Int64 Duration
        {
            get
            {
                if (mActionSource == null)
                    return 0;
                return mActionSource.Duration;
            }
        }
        double mPlayRate = 1D;
        /// <summary>
        /// 播放速度
        /// </summary>
        public double PlayRate
        {
            get { return mPlayRate; }
            set { mPlayRate = value; }
        }

        ActionSource mActionSource = null;
        /// <summary>
        /// 获取监听器
        /// </summary>
        /// <param name="name">监听器名称</param>
        /// <returns>返回相应名称的监听器</returns>
        public ActionNotifier GetNotifier(string name)
        {
            if (mActionSource == null)
                return null;

            return mActionSource.GetFirstNotifier(name);
            //foreach (var notifier in mActionSource.NotifierList)
            //{
            //    if (notifier.NotifyName == name)
            //        return notifier;
            //}

            //return null;
        }
        /// <summary>
        /// 获取监听器
        /// </summary>
        /// <param name="index">监听器索引值</param>
        /// <returns>返回相应的动作监听器</returns>
        public ActionNotifier GetNotifier(UInt32 index)
        {
            if (mActionSource == null)
                return null;

            return mActionSource.GetNotifier((int)index);
            //if (index < mActionSource.NotifierList.Count)
            //    return mActionSource.NotifierList[(int)index];

            //return null;
        }
        /// <summary>
        /// 获取同一类型的监听器列表
        /// </summary>
        /// <param name="type">监听器类型</param>
        /// <returns>返回同一类型的监听器列表</returns>
        public List<ActionNotifier> GetNotifiers(System.Type type)
        {
            if (mActionSource == null)
                return new List<ActionNotifier>();

            return mActionSource.GetNotifier(type);
        }
        /// <summary>
        /// 动作是否结束
        /// </summary>
        /// <returns>动作结束返回true，否则返回false</returns>
        public override bool IsActionFinished()
        {
            if (mActionSource == null)
                return true;

            return CurNotifyTime >= mActionSource.Duration;
        }
    }
}
