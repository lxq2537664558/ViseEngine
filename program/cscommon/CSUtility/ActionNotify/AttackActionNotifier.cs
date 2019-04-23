using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.ActionNotify
{
    /// <summary>
    /// 攻击动作监听类
    /// </summary>
    public class AttackActionNotifier : Animation.ActionNotifier
    {
        /// <summary>
        /// 获取关键帧类型
        /// </summary>
        /// <returns>返回该关键帧类型为AttackNotifyPoint</returns>
        public override Type GetKeyFrameType()
        {
            return typeof(AttackNotifyPoint);
        }
        /// <summary>
        /// 添加监听点
        /// </summary>
        /// <param name="time">监听的时间</param>
        /// <param name="name">关键帧名称</param>
        /// <returns>返回添加的监听点</returns>
        public override Animation.NotifyPoint AddNotifyPoint(long time, string name)
        {
            for (int i = 0; i < mNotifyPoints.Count; i++)
            {
                if (mNotifyPoints[i].NotifyTime >= time)
                {
                    var np = new AttackNotifyPoint(time, name);
                    mNotifyPoints.Insert(i, np);
                    return np;
                }
            }

            var ntP = new AttackNotifyPoint(time, name);
            mNotifyPoints.Add(ntP);
            return ntP;
        }
    }
}
