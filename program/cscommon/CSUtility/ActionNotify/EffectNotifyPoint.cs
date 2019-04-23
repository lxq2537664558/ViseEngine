using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSUtility.ActionNotify
{
    /// <summary>
    /// 特效监听点类
    /// </summary>
    public class EffectNotifyPoint : Animation.NotifyPoint
    {
        List<Animation.NotifyItemDataBase> mPointDatas = new List<Animation.NotifyItemDataBase>();
        /// <summary>
        /// 特效监听点元素数据列表
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public override List<Animation.NotifyItemDataBase> PointDatas
        {
            get { return mPointDatas; }
            set { mPointDatas = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public EffectNotifyPoint()
        {
            NotifyTime = 0;
            NotifyName = "";

            HeaderName = "Effect";
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="time">监听的时间</param>
        /// <param name="name">监听点的名称</param>
        public EffectNotifyPoint(Int64 time, string name)
        {
            KeyFrameMilliTimeEnd = time;
            KeyFrameMilliTimeStart = time;
            NotifyTime = time;
            NotifyName = name;

            HeaderName = "Effect";
        }
        /// <summary>
        /// 添加监听点元素数据
        /// </summary>
        /// <param name="point">特效监听点对象</param>
        public override void AddItemDatas(Animation.NotifyPoint point)
        {
            for (int i = 0; i < PointDatas.Count; ++i)
            {
                PointDatas[i].Index = i;
                PointDatas[i].HostNotifyPoint = point;
            }
        }
        /// <summary>
        /// 添加新的监听数据
        /// </summary>
        /// <param name="point">动作监听点对象</param>
        /// <returns>返回添加的监听点元素数据</returns>
        public override Animation.NotifyItemDataBase AddNewItemData(Animation.NotifyPoint point)
        {
            var item = new EffectItemData() { Index = PointDatas.Count };
            PointDatas.Add(item);
            item.HostNotifyPoint = point;

            return item;
        }
        /// <summary>
        /// 删除元素数据
        /// </summary>
        /// <param name="itemData">监听点元素数据</param>
        public override void RemoveItemData(Animation.NotifyItemDataBase itemData)
        {
            if (itemData == null)
                return;

            PointDatas.Remove(itemData);
            //EffectorList.RemoveAt(itemData.Index);

            UpdateItemDataIndex();
        }
        /// <summary>
        /// 是否可以修改长度
        /// </summary>
        /// <returns>返回true</returns>
        public override bool CanModityLength()
        {
            return true;
        }
    }
    /// <summary>
    /// 特效元素数据
    /// </summary>
    public class EffectItemData : Animation.NotifyItemDataBase
    {
        Guid mEffectId = Guid.Empty;
        /// <summary>
        /// 特效ID
        /// </summary>
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EffectSet")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public Guid EffectId
        {
            get { return mEffectId; }
            set
            {
                if (mEffectId == value)
                    return;

                mEffectId = value;
            }    
        }

        string mSocketName = "";
        /// <summary>
        /// 挂接件名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string SocketName
        {
            get { return mSocketName; }
            set
            {
                mSocketName = value;
            }
        }

        SlimDX.Vector3 mOffset = SlimDX.Vector3.Zero;
        /// <summary>
        /// 位置偏移值
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [DisplayName("偏移")]
        public SlimDX.Vector3 Offset
        {
            get { return mOffset; }
            set { mOffset = value; }
        }

        SlimDX.Vector3 mScale = new SlimDX.Vector3(1f,1f,1f);
        /// <summary>
        /// 缩放值
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [DisplayName("缩放")]
        public SlimDX.Vector3 Scale
        {
            get { return mScale; }
            set { mScale = value; }
        }

        SlimDX.Vector3 mRotationPos;
        /// <summary>
        /// 旋转轴
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [DisplayName("旋转轴")]
        public SlimDX.Vector3 RotationPos
        {
            get { return mRotationPos; }
            set { mRotationPos = value; }
        }

        int mAngle;
        /// <summary>
        /// 旋转角度
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [DisplayName("旋转角度")]
        public int Angle
        {
            get { return mAngle; }
            set { mAngle = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public EffectItemData()
        {            
        }
    }
}    
