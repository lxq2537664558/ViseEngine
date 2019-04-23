using System;
using System.ComponentModel;
/// <summary>
/// 挂接件的命名空间
/// </summary>
namespace CCore.Socket
{
    /// <summary>
    /// 挂接成员属性
    /// </summary>
    public sealed class SocketComponentInfoAttribute : Attribute
    {
        /// <summary>
        /// 描述名称
        /// </summary>
        string mDisplayName = "";
        /// <summary>
        /// 只读属性，描述信息
        /// </summary>
        public string DisplayName
        {
            get { return mDisplayName; }
        }
        /// <summary>
        /// 挂接成员的属性信息
        /// </summary>
        /// <param name="displayName">描述名称</param>
        public SocketComponentInfoAttribute(string displayName)
        {
            mDisplayName = displayName;
        }
    }
    // Socket组件
    // Socket组件的信息，用于存盘等
    /// <summary>
    /// 挂接组件的信息
    /// </summary>
    public interface ISocketComponentInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 挂接组件的ID
        /// </summary>
        Guid SocketComponentInfoId { get; set; }
        /// <summary>
        /// 挂接件的名称
        /// </summary>
        string SocketName { get; set; }
        /// <summary>
        /// 挂接件的描述
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// 挂接成员的类型
        /// </summary>
        string SocketComponentType { get; }
        /// <summary>
        /// 复制挂接组件
        /// </summary>
        /// <param name="srcInfo">挂接件的源数据</param>
        void CopyComponentInfoFrom(ISocketComponentInfo srcInfo);
        /// <summary>
        /// 获取挂接件的类型
        /// </summary>
        /// <returns>返回挂接件的类型</returns>
        Type GetSocketComponentType();
    }
    // Socket组件的实例化数据
    /// <summary>
    /// 挂接成员的实例化数据
    /// </summary>
    public interface ISocketComponent
    {
        /// <summary>
        /// 挂接件的主mesh
        /// </summary>
        CCore.Mesh.Mesh ComponentHostMesh { get; set; }
        /// <summary>
        /// 只读属性，挂接成员信息
        /// </summary>
        ISocketComponentInfo SocketComponentInfo { get; }
        /// <summary>
        /// 删除对象
        /// </summary>
        void Cleanup();
        /// <summary>
        /// 初始化挂接组件
        /// </summary>
        /// <param name="info">挂接成员信息</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        bool InitializeSocketComponent(ISocketComponentInfo info);
        /// <summary>
        /// 获取对象的AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点</param>
        /// <param name="vMax">最大顶点</param>
        void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax);
        /// <summary>
        /// 提交挂接成员
        /// </summary>
        /// <param name="enviroment">渲染环境</param>
        /// <param name="socketMatrix">挂接成员的位置矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="eye">视野</param>
        void SocketComponentCommit(CCore.Graphics.REnviroment enviroment, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, CCore.Camera.CameraObject eye);
        /// <summary>
        /// 提交挂接成员的阴影
        /// </summary>
        /// <param name="light">光源</param>
        /// <param name="socketMatrix">挂接成员的位置矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="isDynamic">是否为动态的</param>
        void SocketComponentCommitShadow(CCore.Light.Light light, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, bool isDynamic);
        /// <summary>
        /// 每帧调用刷新挂接成员
        /// </summary>
        /// <param name="host">主Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        void SocketComponentTick(CSUtility.Component.ActorBase host, long elapsedMillisecond);
        /// <summary>
        /// 挂接成员淡入
        /// </summary>
        /// <param name="fadeType">淡入淡出类型</param>
        void StartSocketComponentFadeIn(CCore.Mesh.MeshFadeType fadeType);
        /// <summary>
        /// 挂接成员的淡出
        /// </summary>
        /// <param name="fadeType">淡入淡出类型</param>
        void StartSocketComponentFadeOut(CCore.Mesh.MeshFadeType fadeType);
        /// <summary>
        /// 更新挂接成员的淡入淡出类型
        /// </summary>
        /// <param name="fadePercent">淡入淡出比例</param>
        void UpdateSocketComponentFadeInOut(float fadePercent);
    }
    // 继承后Socket组件将支持像素点选
    /// <summary>
    /// Socket组件的像素点选
    /// </summary>
    public interface ISocketComponentHitProxy
    {
        /// <summary>
        /// 是否可以进行鼠标点选
        /// </summary>
        bool CanHitProxy { get; }
        /// <summary>
        /// 设置所有成员可点选
        /// </summary>
        /// <param name="hitProxy">点击代理值</param>
        void SetHitProxyAll(UInt32 hitProxy);
    }

    //发布Socket组件挂接资源
    /// <summary>
    /// 发布Socket组件挂接资源
    /// </summary>
    public interface ISocketComponentPublisherRes
    {
        /// <summary>
        /// 只读属性，资源类型
        /// </summary>
        CSUtility.Support.enResourceType ResourceType { get; }
        /// <summary>
        /// 只读属性，资源参数
        /// </summary>
        object[] Param { get; }
    }
}
