using System;
using System.Collections.Generic;

namespace CSUtility.Animation
{
    /// <summary>
    /// 动画节点
    /// </summary>
    public class AnimNode : AnimationTree
    {
        ActionNode mAction;
        /// <summary>
        /// 当前动作
        /// </summary>
        public BaseAction Action
        {
            get { return mAction; }
            set { mAction = value as ActionNode; }
        }

        Int64 mCurNotifyTime;
        /// <summary>
        /// 当前监听时间
        /// </summary>
        public Int64 CurNotifyTime
        {
            get { return mCurNotifyTime; }
            set { mCurNotifyTime = value; }
        }

        List<AnimationTree> mAnimations = new List<AnimationTree>();
        /// <summary>
        /// 获取动画树
        /// </summary>
        /// <returns>返回当前的动画树列表</returns>
        public List<AnimationTree> GetAnimations()
        {
            return mAnimations;
        }
        //void SetAnimations(List<AnimationTree> anims);     
        /// <summary>
        /// 添加动画树节点
        /// </summary>
        /// <param name="node">动画树对象</param>
        public void AddNode(AnimationTree node)
        {
            mAnimations.Add(node);
        }
        /// <summary>
        /// 当前动作是否完成
        /// </summary>
        /// <returns>动作完成返回true，否则返回false</returns>
        public virtual bool IsActionFinished()
        {
            if (mAnimations.Count == 0)
                return false;

            foreach (var i in mAnimations)
            {
                if (i.IsActionFinished() == false)
                    return false;
            }

            return true;// return this->GetATFinished();
        }

        bool mbLoop = false;
        /// <summary>
        /// 设置动作是否循环
        /// </summary>
        /// <param name="bLoop">动作是否循环</param>
        public void SetLoop(bool bLoop)
        {
            mbLoop = bLoop;
        }
        /// <summary>
        /// 获取该动作是否循环
        /// </summary>
        /// <returns>返回该动作是否循环</returns>
        public bool GetLoop()
        {
            return mbLoop;
        }
        float mPlayRate = 1.0f;
        /// <summary>
        /// 设置播放速度
        /// </summary>
        /// <param name="playRate">播放速度</param>
        public void SetPlayRate(float playRate)
        {
            mPlayRate = playRate;
        }
        /// <summary>
        /// 获取播放速度
        /// </summary>
        /// <returns>返回当前播放速度</returns>
        public float GetPlayRate()
        {
            return mPlayRate;
        }

        Delegate_OnAnimTreeFinish mDelegateOnAnimTreeFinish;
        /// <summary>
        /// 动画树播放完成调用的方法
        /// </summary>
        public Delegate_OnAnimTreeFinish DelegateOnAnimTreeFinish
        {
            get { return mDelegateOnAnimTreeFinish; }
            set { mDelegateOnAnimTreeFinish = value; }
        }

        Delegate_OnActionFinish mDelegateOnActionFinish;
        /// <summary>
        /// 动作播放完成调用的方法
        /// </summary>
        public Delegate_OnActionFinish DelegateOnActionFinish
        {
            get { return mDelegateOnActionFinish; }
            set { mDelegateOnActionFinish = value; }
        }
    }
}
