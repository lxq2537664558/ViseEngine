using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.World
{
    public partial class World
    {
        System.Collections.Concurrent.ConcurrentBag<CSUtility.SceneActorPropertyChangeData> mProcessNextTimeList = new System.Collections.Concurrent.ConcurrentBag<CSUtility.SceneActorPropertyChangeData>();
        CSUtility.Net.ThreadSafeQueue<CSUtility.SceneActorPropertyChangeData> mPropertyChangeQueue = new CSUtility.Net.ThreadSafeQueue<CSUtility.SceneActorPropertyChangeData>();

        /// <summary>
        /// 将服务器端通知客户端需要改变属性的对象加入队列
        /// </summary>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <param name="targetValue"></param>
        public void QueueSceneActorPropertyChanged(Guid id, string propertyName, string targetValue)
        {
            var data = new CSUtility.SceneActorPropertyChangeData()
            {
                ActorId = id,
                PropertyName = propertyName,
                TargetValue = targetValue
            };
            mPropertyChangeQueue.Enqueue(data);
        }

        /// <summary>
        /// 当前时间标记，用于每帧处理时计算时间间隔
        /// </summary>
        Int64 mCurTime = 0;

        /// <summary>
        /// 属性改变每帧处理函数
        /// </summary>
        /// <param name="elapsedTime">每帧时间间隔</param>
        private void SceneActorPropertyChangeTick(Int64 elapsedTime)
        {
            mCurTime += elapsedTime;
            // 每隔一秒处理一次
            if (mCurTime > 1000)
            {
                mCurTime = 0;

                if (CCore.Client.MainWorldInstance.IsNullWorld)
                    return;

                while (mPropertyChangeQueue.Count > 0)
                {
                    var data = mPropertyChangeQueue.Dequeue();

                    var actor = CCore.Client.MainWorldInstance.FindActor(data.ActorId);
                    if (actor == null)
                    {
                        // 对象可能还未加载
                        mProcessNextTimeList.Add(data);
                        continue;
                    }

                    var propertyInfo = actor.GetType().GetProperty(data.PropertyName);
                    if (propertyInfo == null)
                    {
                        continue;
                    }

                    try
                    {
                        object targetValue = null;
                        if (propertyInfo.PropertyType == typeof(SByte))
                        {
                            targetValue = System.Convert.ToSByte(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(Int16))
                        {
                            targetValue = System.Convert.ToInt16(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(Int32))
                        {
                            targetValue = System.Convert.ToInt32(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(Int64))
                        {
                            targetValue = System.Convert.ToInt64(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(Byte))
                        {
                            targetValue = System.Convert.ToByte(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(UInt16))
                        {
                            targetValue = System.Convert.ToUInt16(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(UInt32))
                        {
                            targetValue = System.Convert.ToUInt32(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(UInt64))
                        {
                            targetValue = System.Convert.ToUInt64(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(Single))
                        {
                            targetValue = System.Convert.ToSingle(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(Double))
                        {
                            targetValue = System.Convert.ToDouble(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType == typeof(System.Boolean))
                        {
                            targetValue = System.Convert.ToBoolean(data.TargetValue);
                        }
                        else if (propertyInfo.PropertyType.IsEnum)
                        {
                            targetValue = System.Enum.Parse(propertyInfo.PropertyType, data.TargetValue);
                        }
                        else
                            continue;

                        propertyInfo.SetValue(actor, targetValue, null);
                    }
                    catch (System.Exception) { }
                }
                foreach (var data in mProcessNextTimeList)
                {
                    mPropertyChangeQueue.Enqueue(data);
                }
            }
        }
    }
}
