using System;
using System.Collections.Generic;
using System.ComponentModel;
/// <summary>
/// 场景功能命名空间
/// </summary>
namespace CCore.Scene
{
    /// <summary>
    /// 场景图信息
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public abstract class SceneGraphInfo : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用
        /// </summary>
        /// <param name="propertyName">改变的属性的名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="worldInit">世界对象的初始化对象</param>
        public abstract void Initialize(CCore.World.WorldInit worldInit);
        /// <summary>
        /// 获取场景的类型
        /// </summary>
        /// <returns>返回场景类型</returns>
        public abstract System.Type GetSceneGraphType();
        /// <summary>
        /// 恢复为默认参数设置
        /// </summary>
        public abstract void ResetDefault();
    }
    /// <summary>
    /// 场景图
    /// </summary>
    public abstract class SceneGraph
    {
        /// <summary>
        /// 该对象是否有效，默认为false
        /// </summary>
        protected bool mIsValid = false;
        /// <summary>
        /// 只读属性，场景对象是否有效
        /// </summary>
        public bool IsValid
        {
            get { return mIsValid; }
        }
        /// <summary>
        /// 是否绑定阴影提交，默认为false
        /// </summary>
        protected bool mLockShadowCommit = false;
        /// <summary>
        /// 场景带阴影提交
        /// </summary>
        public bool LockShadowCommit
        {
            get { return mLockShadowCommit; }
            set { mLockShadowCommit = value; }
        }

        /// <summary>
        /// 初始化场景管理
        /// </summary>
        /// <param name="absMapFolder">地图所在路径</param>
        /// <param name="info">场景管理器参数</param>
        /// <param name="hostWorld">使用此场景管理器的地图</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public abstract bool Initialize(string absMapFolder, SceneGraphInfo info, CCore.World.World hostWorld);

        /// <summary>
        /// 创建场景管理
        /// </summary>
        /// <param name="absMapFolder">地图所在路径</param>
        /// <param name="info">场景管理器参数</param>
        /// <param name="hostWorld">使用此场景管理器的地图</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public abstract bool Create(string absMapFolder, SceneGraphInfo info, CCore.World.World hostWorld);
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public abstract void Cleanup();
        /// <summary>
        /// 每帧调用
        /// </summary>
        public abstract void Tick();
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="lockCulling">是否进行绑定</param>
        public abstract void SetLockCulling(bool lockCulling);
        /// <summary>
        /// 获取绑定对象
        /// </summary>
        /// <returns>返回绑定对象</returns>
        public abstract bool GetLockCulling();
        /// <summary>
        /// 添加Actor对象
        /// </summary>
        /// <param name="act">Actor对象</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public abstract bool AddActor(CCore.World.Actor act);
        //virtual bool RemoveActor(IActor act) abstract;
        /// <summary>
        /// 删除相应的Actor对象
        /// </summary>
        /// <param name="act">Actor对象</param>
        /// <returns>成功删除返回true，否则返回false</returns>
        public abstract bool RemoveActor(CCore.World.Actor act);//ref System.Guid id);
        /// <summary>
        /// 删除所有的Actor对象
        /// </summary>
        public abstract void RemoveAllActor();
        /// <summary>
        /// 根据ActorID查找Actor对象
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>返回找到的Actor对象</returns>
        public abstract CCore.World.Actor FindActor(ref System.Guid id);
        /// <summary>
        /// Actor对象的动作
        /// </summary>
        /// <param name="actorType">Actor对象的类型</param>
        /// <param name="process">Actor对象的动作列表</param>
        public abstract void ProcessActors(UInt16 actorType, Action<World.Actor> process);
        /// <summary>
        /// 获取某一类型的所有Actor对象
        /// </summary>
        /// <param name="actorType">Actor的类型</param>
        /// <returns>返回某一类型的所有Actor对象</returns>
        public abstract List<CCore.World.Actor> GetActors(UInt16 actorType);
        /// <summary>
        /// 获取对应区域的所有的Actor对象
        /// </summary>
        /// <param name="vStart">起点坐标</param>
        /// <param name="vEnd">终点坐标</param>
        /// <param name="actorType">Actor类型</param>
        /// <returns>返回对应区域的所有的Actor对象</returns>
        public abstract List<CCore.World.Actor> GetActors(ref SlimDX.Vector3 vStart, ref SlimDX.Vector3 vEnd, UInt16 actorType);
        /// <summary>
        /// 绘制对象
        /// </summary>
        /// <param name="eye">视野</param>
        /// <param name="env">渲染环境</param>
        /// <param name="shadowLights">光源</param>
        public abstract void RenderVisible(CCore.Camera.CameraObject eye, CCore.Graphics.REnviroment env, CCore.Light.Light[] shadowLights);
        /// <summary>
        /// 绘制阴影
        /// </summary>
        /// <param name="camera">摄像机</param>
        /// <param name="env">渲染环境</param>
        /// <param name="shadowLights">产生阴影的光源</param>
        public abstract void RenderShadow(CCore.Camera.CameraObject camera, CCore.Graphics.REnviroment env, CCore.Light.Light[] shadowLights);
        /// <summary>
        /// 渲染导航对象
        /// </summary>
        /// <param name="navLvlX">导航X轴的长度</param>
        /// <param name="navLvlZ">导航Z轴的长度</param>
        /// <param name="startX">导航起点X轴的值</param>
        /// <param name="startZ">导航起点Z轴的值</param>
        /// <param name="endX">导航终点X轴的值</param>
        /// <param name="endZ">导航终点Z轴的值</param>
        /// <param name="nav">导航对象</param>
        /// <returns>渲染成功返回true，否则返回false</returns>
        public abstract bool RenderNavigation(UInt32 navLvlX, UInt32 navLvlZ, float startX, float startZ, float endX, float endZ, CCore.Navigation.NavigationAssist nav);
        /// <summary>
        /// 渲染服务器高度图
        /// </summary>
        /// <param name="lvlX">X轴的长度</param>
        /// <param name="lvlZ">Z轴的长度</param>
        /// <param name="shm">服务器高度图帮助对象</param>
        /// <returns>绘制成功返回true，否则返回false</returns>
        public abstract bool RenderServerHeightMap(UInt32 lvlX, UInt32 lvlZ, CCore.Support.ServerAltitudeAssist shm);
        /// <summary>
        /// 网格线检查
        /// </summary>
        /// <param name="start">线段起始点</param>
        /// <param name="end">线段终点</param>
        /// <param name="result">点击结果</param>
        /// <returns>检查无错误返回true，否则返回false</returns>
        public abstract bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result);
        /// <summary>
        /// 网格线检查
        /// </summary>
        /// <param name="start">线段起点</param>
        /// <param name="end">线段终点</param>
        /// <param name="result">点击结果</param>
        /// <param name="exceptActor">其他的Actor对象列表</param>
        /// <returns>检查无错误返回true，否则返回false</returns>
        public abstract bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result, List<CCore.World.Actor> exceptActor);

        //public abstract bool SaveScene_All(System.String name, System.String serverName, bool forceSave);
        /// <summary>
        /// 保存客户端场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="forceSave">是否强制保存</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public abstract bool SaveScene_ClientScene(string name, bool forceSave);
        /// <summary>
        /// 加载客户端场景
        /// </summary>
        /// <param name="mapPath">地图路径</param>
        /// <param name="name">场景名称</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public abstract bool LoadScene_ClientScene(string mapPath, string name);
        //public abstract bool SaveScene_NPC(string name, bool forceSave);
        //public abstract bool SaveScene_Trigger(string name, bool forceSave);
        /// <summary>
        /// 保存特殊的场景
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="actorType">Actor类型</param>
        /// <param name="bForceSave">是否强制保存</param>
        /// <returns>成功保存返回true，否则返回false</returns>
        public abstract bool SaveScene_Special(System.String path, UInt16 actorType, bool bForceSave);
        /// <summary>
        /// 加载特殊场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="actorType">Actor类型</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public abstract bool LoadScene_Special(System.String name, UInt16 actorType);
        /// <summary>
        /// 可访问的场景的X轴和Z轴方向的场景块数量
        /// </summary>
        /// <param name="x">X轴</param>
        /// <param name="z">Z轴</param>
        // Level
        public abstract void GetLevelAvailableXZCount(ref System.UInt32 x, ref System.UInt32 z);	// 获取可用Level的XZ向大小
        /// <summary>
        /// 获取每个场景块X轴方向的长度
        /// </summary>
        /// <returns>返回每个场景块X轴方向的长度</returns>
        public abstract float GetXLengthPerLevel();
        /// <summary>
        /// 获取每个场景块Z轴方向的长度
        /// </summary>
        /// <returns>返回每个场景块Z轴方向的长度</returns>
        public abstract float GetZLengthPerLevel();

        // idu,idv为level的在terrain索引
        /// <summary>
        /// 增加场景块高度
        /// </summary>
        /// <param name="idu">u向高度</param>
        /// <param name="idv">v向高度</param>
        public abstract void AddLevel(UInt32 idu, UInt32 idv);
        // idu,idv为level的在terrain索引
        /// <summary>
        /// 删除对应高度
        /// </summary>
        /// <param name="idu">u向高度</param>
        /// <param name="idv">v向高度</param>
        public abstract void DelLevel(UInt32 idu, UInt32 idv);
        /// <summary>
        /// 将坐标转换到块状地图中
        /// </summary>
        /// <param name="x">X轴的坐标</param>
        /// <param name="z">Z轴的坐标</param>
        /// <returns>转换成功返回true，否则返回false</returns>
        public abstract bool TravelTo(float x, float z);
        /// <summary>
        /// 设置边缘间隔
        /// </summary>
        /// <param name="value">间隔距离</param>
        public abstract void SetNeighborSide(uint value);
    }
}
