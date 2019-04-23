using System.Collections.Generic;
using System.ComponentModel;

namespace CSUtility.Component
{
    // 对象层管理器，用于世界浏览器对象分层管理
    /// <summary>
    /// Actor对象的层管理器
    /// </summary>
    public class IActorLayerManger : INotifyPropertyChanged
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
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        static IActorLayerManger smInstance = new IActorLayerManger();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static IActorLayerManger Instance
        {
            get { return smInstance; }
        }

        List<string> mLayers = new List<string>();
        /// <summary>
        /// 对象所在的层列表
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Layers")]
        public List<string> Layers
        {
            get { return mLayers; }
            set { mLayers = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public IActorLayerManger()
        {
            Load();
        }
        /// <summary>
        /// 添加层
        /// </summary>
        /// <param name="layerName">层名称</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public bool AddLayer(string layerName)
        {
            if (Layers.Contains(layerName))
                return false;

            Layers.Add(layerName);

            return true;
        }
        /// <summary>
        /// 删除层
        /// </summary>
        /// <param name="layerName">层名称</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool RemoveLayer(string layerName)
        {
            return Layers.Remove(layerName);
        }
        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <returns>返回文件名称</returns>
        public string GetFileName()
        {
            return CSUtility.Support.IFileConfig.EditorResourcePath + "/LayerConfig.xml";
        }
        /// <summary>
        /// 加载对象
        /// </summary>
        public void Load()
        {
            CSUtility.Support.IConfigurator.FillProperty(this, GetFileName());
        }
        /// <summary>
        /// 保存对象
        /// </summary>
        public void Save()
        {
            CSUtility.Support.IConfigurator.SaveProperty(this, "Layers", GetFileName());
        }
    }
}
