using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.ActionNotify
{
    /// <summary>
    /// 特效动作监听器类
    /// </summary>
    public class EffectActionNotifier : Animation.ActionNotifier
    {
        /// <summary>
        /// 声明激活特效时调用的委托事件
        /// </summary>
        /// <param name="data">特效元素数据对象</param>
        public delegate void Delegate_ActiveEffect(CSUtility.ActionNotify.EffectItemData data);
        /// <summary>
        /// 定义激活特效时调用的委托事件
        /// </summary>
        public Delegate_ActiveEffect OnActiveEffect;
        /// <summary>
        /// 获取关键帧类型
        /// </summary>
        /// <returns>返回该关键帧的类型为EffectNotifyPoint</returns>
        public override Type GetKeyFrameType()
        {
            return typeof(EffectNotifyPoint);
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
                    var np = new EffectNotifyPoint(time, name);
                    mNotifyPoints.Insert(i, np);
                    return np;
                }
            }

            var ntP = new EffectNotifyPoint(time, name);
            mNotifyPoints.Add(ntP);
            return ntP;
        }
        /// <summary>
        /// 更新特效是否激活
        /// </summary>
        /// <param name="preTime">上次时间</param>
        /// <param name="nowTime">当前时间</param>
        public void UpdateEffectActive(long preTime, long nowTime)
        {
            if (preTime == nowTime) // 当前时间和上次时间一样，说明动作暂停中
                return;

            if (OnActiveEffect != null)
            {
                var notifys = GetNotifyPoints(preTime, nowTime);
                foreach (EffectNotifyPoint ntf in notifys)
                {
                    foreach (var data in ntf.PointDatas)
                    {
                        OnActiveEffect((CSUtility.ActionNotify.EffectItemData)data);
                    }
                }
            }
        }
    }
}
