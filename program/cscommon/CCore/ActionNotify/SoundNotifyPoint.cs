using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CCore.ActionNotify
{
    /// <summary>
    /// 声音监听元素类
    /// </summary>
    public class SoundNotifyPoint : CSUtility.Animation.NotifyPoint
    {
        List<CSUtility.Animation.NotifyItemDataBase> mPointDatas = new List<CSUtility.Animation.NotifyItemDataBase>();
        /// <summary>
        /// 所有监听元素的数据属性
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public override List<CSUtility.Animation.NotifyItemDataBase> PointDatas
        {
            get { return mPointDatas; }
            set
            {
                mPointDatas = value;
            }
        }
        /// <summary>
        /// 不带参的声音监听器构造函数
        /// </summary>
        public SoundNotifyPoint()
        {
            NotifyTime = 0;
            NotifyName = "";
            HeaderName = "Sound";
        }
        /// <summary>
        /// 带参的声音监听器构造函数
        /// </summary>
        /// <param name="time">该监听器的监听时间</param>
        /// <param name="name">监听器的名字</param>
        public SoundNotifyPoint(Int64 time, string name)
        {
            KeyFrameMilliTimeStart = time;
            KeyFrameMilliTimeEnd = time;
            NotifyTime = time;
            NotifyName = name;
            HeaderName = "Sound";
        }
        /// <summary>
        /// 添加监听类型的数据元素
        /// </summary>
        /// <param name="point">监听点</param>
        public override void AddItemDatas(CSUtility.Animation.NotifyPoint point)
        {
            for (int i = 0; i < PointDatas.Count; ++i)
            {
                PointDatas[i].Index = i;
                PointDatas[i].HostNotifyPoint = point;
            }
        }
        /// <summary>
        /// 向监听的数据元素中添加监听项
        /// </summary>
        /// <param name="point">监听点</param>
        /// <returns>返回一个监听数据元素</returns>
        public override CSUtility.Animation.NotifyItemDataBase AddNewItemData(CSUtility.Animation.NotifyPoint point)
        {
            var item = new Audio.AudioSourceData() { Index = PointDatas.Count };
            PointDatas.Add(item);
            item.HostNotifyPoint = point;

            return item;
        }
        /// <summary>
        /// 移出监听数据元素
        /// </summary>
        /// <param name="itemData">监听类型的数据元素</param>
        public override void RemoveItemData(CSUtility.Animation.NotifyItemDataBase itemData)
        {
            PointDatas.Remove(itemData);

            UpdateItemDataIndex();
        }
        /// <summary>
        /// 激活声音监听点
        /// </summary>
        /// <param name="host">监听点的Actor</param>
        public override void ActiveNotifyPoint(CSUtility.Component.ActorBase host)
        {
            if (host != null && host.Placement != null)
            {
                var pos = host.Placement.GetLocation();
                foreach (var data in PointDatas)
                {
                    ((Audio.AudioSourceData)data).Play(pos);
                }
            }
            else
            {
                foreach (var data in PointDatas)
                {
                    ((Audio.AudioSourceData)data).Play();
                }
            }
        }
    }
}
