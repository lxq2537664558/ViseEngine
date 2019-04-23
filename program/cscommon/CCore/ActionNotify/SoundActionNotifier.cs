using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.ActionNotify
{    
    /// <summary>
    /// 声音动作监听器
    /// </summary>
    public class SoundActionNotifier : CSUtility.Animation.ActionNotifier
    {
        /// <summary>
        /// 得到监听器的客户端类型
        /// </summary>
        /// <returns>将CSType返回Client类型</returns>
        public override CSUtility.Helper.enCSType GetNotifyCSType()
        {
            return CSUtility.Helper.enCSType.Client;
        }
        /// <summary>
        /// 得到关键帧类型
        /// </summary>
        /// <returns>返回声音监听类型</returns>
        public override Type GetKeyFrameType()
        {
            return typeof(SoundNotifyPoint);
        }
        /// <summary>
        /// 增加监听点
        /// </summary>
        /// <param name="time">监听时间</param>
        /// <param name="name">监听器的名字</param>
        /// <returns>返回添加的动作监听点</returns>
        public override CSUtility.Animation.NotifyPoint AddNotifyPoint(long time, string name)
        {
            for (int i = 0; i < mNotifyPoints.Count; i++)
            {
                if (mNotifyPoints[i].NotifyTime >= time)
                {
                    var np = new SoundNotifyPoint(time, name);
                    mNotifyPoints.Insert(i, np);
                    return np;
                }
            }

            var ntP = new SoundNotifyPoint(time, name);
            mNotifyPoints.Add(ntP);
            return ntP;
        }
    }
}
