using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CSUtility.Animation
{
    /// <summary>
    /// 监听点类
    /// </summary>
    public class NotifyPoint : CSUtility.Support.XndSaveLoadProxy, Animation.TimeLineKeyFrameObjectInterface, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 属性改变时调用的方法
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用的方法
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        /// <summary>
        /// 关键帧名称
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public string KeyFrameName
        {
            get { return NotifyName; }
            set { NotifyName = value; }
        }

        Int64 mKeyFrameMilliTimeStart;
        /// <summary>
        /// 关键帧的开始时间
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public Int64 KeyFrameMilliTimeStart
        {
            get { return mKeyFrameMilliTimeStart; }
            set
            {
                mKeyFrameMilliTimeStart = value;
                NotifyTime = value;
            }
        }

        Int64 mKeyFrameMilliTimeEnd;
        /// <summary>
        /// 关键帧的结束时间
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public Int64 KeyFrameMilliTimeEnd
        {
            get { return mKeyFrameMilliTimeEnd; }
            set { mKeyFrameMilliTimeEnd = value; }
        }
        /// <summary>
        /// 是否可以修改长度
        /// </summary>
        /// <returns>返回false</returns>
        public virtual bool CanModityLength()
        {
            return false;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public NotifyPoint()
        {
            NotifyTime = 0;
            NotifyName = "";
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="time">监听时间</param>
        /// <param name="name">监听器名称</param>
        public NotifyPoint(Int64 time, string name)
        {
            NotifyTime = time;
            NotifyName = name;
        }
        Int64 mNotifyTime = 0;
        /// <summary>
        /// 监听时间
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [ReadOnly(true)]
        public Int64 NotifyTime
        {
            get { return mNotifyTime; }
            set
            {
                mNotifyTime = value;

                OnPropertyChanged("NotifyTime");
            }
        }
        string mNotifyName = "";
        /// <summary>
        /// 监听器名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string NotifyName
        {
            get { return mNotifyName; }
            set
            {
                mNotifyName = value;

                OnPropertyChanged("NotifyName");
            }
        }

        bool mIsModifyLength;
        /// <summary>
        /// 是否可以修改长度
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool IsModifyLength
        {
            get { return mIsModifyLength; }
            set
            {
                mIsModifyLength = value;
            }
        }
        /// <summary>
        /// 监听点数据列表
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public virtual List<NotifyItemDataBase> PointDatas
        {
            get;
            set;
        } = new List<NotifyItemDataBase>();
        /// <summary>
        /// 添加元素数据
        /// </summary>
        /// <param name="point"></param>
        public virtual void AddItemDatas(NotifyPoint point)
        {
            
        }
        /// <summary>
        /// 添加新的元素数据
        /// </summary>
        /// <param name="point">监听点</param>
        /// <returns>返回监听点元素数据</returns>
        public virtual NotifyItemDataBase AddNewItemData(NotifyPoint point)
        {
            return null;
        }
        /// <summary>
        /// 删除监听点元素数据
        /// </summary>
        /// <param name="itemData">监听点元素数据</param>
        public virtual void RemoveItemData(NotifyItemDataBase itemData)
        {
            
        }
        /// <summary>
        /// 更新元素数据索引
        /// </summary>
        public virtual void UpdateItemDataIndex()
        {
            for (int i = 0; i < PointDatas.Count; ++i)
            {
                PointDatas[i].Index = i;
            }
        }
        /// <summary>
        /// 头名称
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public virtual string HeaderName
        {
            get;
            set;
        } = "属性";
        /// <summary>
        /// 激活监听点
        /// </summary>
        /// <param name="host">主Actor</param>
        public virtual void ActiveNotifyPoint(CSUtility.Component.ActorBase host)
        {

        }
    }    
    /// <summary>
    /// 监听元素数据类
    /// </summary>
    public class NotifyItemDataBase: CSUtility.Support.XndSaveLoadProxy,INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用的方法
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        NotifyPoint mHostNotifyPoint = null;
        /// <summary>
        /// 所属的监听点
        /// </summary>
        [Browsable(false)]
        public NotifyPoint HostNotifyPoint
        {
            get { return mHostNotifyPoint; }
            set { mHostNotifyPoint = value; }
        }

        int mIndex = 0;
        /// <summary>
        /// 索引值
        /// </summary>
        [Browsable(false)]
        public int Index
        {
            get { return mIndex; }
            set
            {
                mIndex = value;
                OnPropertyChanged("Index");
            }
        }   
    }
    /// <summary>
    /// 获取可视化的Actor接口
    /// </summary>
    public interface IGetVisualActor
    {
        /// <summary>
        /// 获取可视化的Actor对象
        /// </summary>
        /// <returns></returns>
        CSUtility.Component.ActorBase GetVisualActor();
    }
}
