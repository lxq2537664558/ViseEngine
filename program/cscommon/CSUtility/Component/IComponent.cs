using System.ComponentModel;

namespace CSUtility.Component
{
    /// <summary>
    /// 组件成员类
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public class IComponent : CSUtility.Support.XndSaveLoadProxy, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用的方法
        /// </summary>
        /// <param name="propertyName"></param>
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
        /// 获取组件ID
        /// </summary>
        /// <returns>返回组件对象的ID</returns>
        public virtual int GetComponentId()
        {
            return this.GetType().FullName.GetHashCode();
        }
        /// <summary>
        /// 获取组件对象
        /// </summary>
        /// <param name="Id">组件ID</param>
        /// <returns>返回组件对象</returns>
        public IComponent GetComponent(int Id)
        {
            return null;
        }
        /// <summary>
        /// 获取相应类型的组件
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回相应的组件对象</returns>
        public IComponent GetComponent(System.Type type)
        {
            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <param name="comp">组件对象</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public bool AddComponent(System.Type type, IComponent comp)
        {
            return true;
        }
        /// <summary>
        /// 删除相应类型的组件
        /// </summary>
        /// <param name="type">类型</param>
        public void RemoveComponent(System.Type type)
        {
            return;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="host">主Actor对象</param>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public virtual void Tick(ActorBase host, long elapsedMillisecond) { }
    }
}
