using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.ActionNotify
{
    /// <summary>
    /// 震动类型动作监听器
    /// </summary>
    public class ShakeActionNotifier : CSUtility.Animation.ActionNotifier
    {
        //public delegate void Delegate_ActiveShake(Guid ShakeId, CSCommon.Animation.ShakeNotifyPoint notifyPoint);
        //public Delegate_ActiveShake OnActiveShake;
        /// <summary>
        /// 返回该实例的enCStype为客户端类型
        /// </summary>
        /// <returns>返回实例的enCStype，客户端或者服务器类型</returns>
        public override CSUtility.Helper.enCSType GetNotifyCSType()
        {
            return CSUtility.Helper.enCSType.Client;
        }
        /// <summary>
        /// 返回关键帧的类型
        /// </summary>
        /// <returns>返回震动监听点的类型</returns>
        public override Type GetKeyFrameType()
        {
            return typeof(ShakeNotifyPoint);
        }
        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="time">监听的时间</param>
        /// <param name="name">监听器的名字</param>
        /// <returns>返回一个震动类型的监听器</returns>
        public override CSUtility.Animation.NotifyPoint AddNotifyPoint(long time, string name)
        {
            for (int i = 0; i < mNotifyPoints.Count; i++)
            {
                if (mNotifyPoints[i].NotifyTime >= time)
                {
                    var np = new ShakeNotifyPoint(time, name);
                    mNotifyPoints.Insert(i, np);
                    return np;
                }
            }

            var ntP = new ShakeNotifyPoint(time, name);
            mNotifyPoints.Add(ntP);
            return ntP;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="time">每帧之间的间隔时间</param>
        public override void Tick(Int64 time)
        {
            
        }
    }
}
