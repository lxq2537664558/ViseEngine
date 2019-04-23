using System;
using System.Collections.Generic;

namespace CCore.Navigation
{
    /// <summary>
    /// 导航
    /// </summary>
    public class Navigation
    {
        /// <summary>
        /// 声明查找路径时调用的委托事件
        /// </summary>
        /// <param name="result">路径坐标列表</param>
        public delegate void Delegate_FOnFindPath(List<SlimDX.Vector2> result);
        /// <summary>
        /// 定义查找路径的委托对象
        /// </summary>
        public Delegate_FOnFindPath OnFindPath;
        /// <summary>
        /// 寻路的contex
        /// </summary>
        protected CSUtility.Navigation.PathFindContextWrapper mPathFindContext;
        /// <summary>
        /// 寻路的路径点
        /// </summary>
        protected CSUtility.Navigation.PathFindContextWrapper_NavigationPoint mPathFindContext_NavigationPoint;
        /// <summary>
        /// 导航
        /// </summary>
        protected CSUtility.Navigation.INavigationWrapper mNavigation;
        /// <summary>
        /// 导航对象实例
        /// </summary>
        static Navigation mInstance = new Navigation();
        /// <summary>
        /// 声明该类为单例模式
        /// </summary>
        public static Navigation Instance
        {
            get { return mInstance; }
        }
        /// <summary>
        /// 导航数据
        /// </summary>
        protected CSUtility.Navigation.INavigationDataWrapper mNavData;
        /// <summary>
        /// 只读属性，导航数据
        /// </summary>
        public CSUtility.Navigation.INavigationDataWrapper NavigationData
        {
            get { return mNavData; }
        }
        /// <summary>
        /// 导航点数据
        /// </summary>
        protected CSUtility.Navigation.INavigationPointDataWrapper mNavPointData;
        /// <summary>
        /// 只读属性，导航点数据
        /// </summary>
        public CSUtility.Navigation.INavigationPointDataWrapper NavigationPointData
        {
            get { return mNavPointData; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        private Navigation()
        {
            mNavigation = new CSUtility.Navigation.INavigationWrapper();
        }
        /// <summary>
        /// 析构函数，删除对象和寻路器对象
        /// </summary>
        ~Navigation()
        {
            Cleanup();
            CleanupPathFinder();
        }
        /// <summary>
        /// 删除NavData，NavPointData，PathFindContext，PathFindContext_NavigationPoint对象
        /// </summary>
        public void Cleanup()
        {
            if (mNavData != null)
            {
                mNavData.Cleanup();
                mNavData = null;
            }
            if (mNavPointData != null)
            {
                mNavPointData.Cleanup();
                mNavPointData = null;
            }
            mPathFindContext = null;
            mPathFindContext_NavigationPoint = null;
        }
        /// <summary>
        /// 删除路径查找对象
        /// </summary>
        public void CleanupPathFinder()
        {
            if (mNavigation != null)
            {
                mNavigation.Cleanup();
                mNavigation = null;
            }
        }
        /// <summary>
        /// 导航信息
        /// </summary>
        public CSUtility.Navigation.NavigationInfo Info
        {
            get;
            set;
        } = new CSUtility.Navigation.NavigationInfo();
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        /// <param name="info">导航信息</param>
        public void Initialize(string name, string path, ref CSUtility.Navigation.NavigationInfo info)
        {
            Cleanup();

            Info = info;

            mNavData = new CSUtility.Navigation.INavigationDataWrapper();
            mNavData.OnGetDynamicBlockIsBlock = new CSUtility.Navigation.INavigationDataWrapper.Delegate_OnDynamicBlockIsBlock(_OnGetDynamicBlockIsBlock);
            mNavData.OnGetActorGUIDFromUInt32Id = new CSUtility.Navigation.INavigationDataWrapper.Delegate_OnGetActorGUIDFromUInt32Id(_OnGetActorGUIDFromUInt32Id);
            mNavData.OnGetActorUIntId = new CSUtility.Navigation.INavigationDataWrapper.Delegate_OnGetActorUIntID(_OnGetActorUIntID);
            mNavData.ConstrutNavigationData(name, path, info);

            mNavPointData = new CSUtility.Navigation.INavigationPointDataWrapper();
            mNavPointData.Initialize(info.GetLevelXMeterLength(), info.GetLevelZMeterLength(),
                info.GetValidXPixelLength() * info.GetPixelXMeterLength(),
                info.GetValidZPixelLength() * info.GetPixelZMeterLength());

            //mNavigation.InitNavigationData(mNavData);
            if (mPathFindContext==null)
                mPathFindContext = new CSUtility.Navigation.PathFindContextWrapper();
            mPathFindContext.Initialize(info);

            if (mPathFindContext_NavigationPoint == null)
                mPathFindContext_NavigationPoint = new CSUtility.Navigation.PathFindContextWrapper_NavigationPoint();
        }
        /// <summary>
        /// 保存该对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        /// <param name="forceSave">是否强制保存</param>
        public void Save(string name, string path, bool forceSave)
        {
            if (NavigationData != null)
                NavigationData.SaveNavigationData(name, path, forceSave);

            if (NavigationPointData != null)
                NavigationPointData.SaveNavigationData(name + "Pt", path, forceSave);
        }
        /// <summary>
        /// 加载导航对象
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="path">路径</param>
        public void Load(string name, string path)
        {
            NavigationData.LoadNavigationData(name, path, false);
            CSUtility.Navigation.NavigationInfo info;
            NavigationData.GetNavigationInfo(out info);
            Info = info;
            mPathFindContext.Initialize(Info);
            NavigationPointData.LoadNavigationData(name + "Pt", path);
        }
        private int _OnGetDynamicBlockIsBlock(Guid mapInstanceId, Guid actorId)
        {
            var actor = CCore.Engine.Instance.Client.MainWorld.FindActor(actorId);
            // 判断动态碰撞体是否能够通过
            var dynamicBlockActor = actor as CCore.World.DynamicBlockActor;
            if (dynamicBlockActor == null)
                return 0;

            if (dynamicBlockActor.IsBlock)
                return 1;

            return 0;
        }
        private Guid _OnGetActorGUIDFromUInt32Id(UInt32 id)
        {
            return CCore.Graphics.HitProxyMap.Instance.GetActorId(id);
        }
        private UInt32 _OnGetActorUIntID(Guid actorId)
        {
            return CCore.Graphics.HitProxyMap.Instance.GenHitProxy(actorId);
        }
        /// <summary>
        /// 寻路
        /// </summary>
        /// <param name="mapInstanceId">地图的实例ID</param>
        /// <param name="inStartX">寻路的起点X坐标</param>
        /// <param name="inStartZ">寻路的起点Z坐标</param>
        /// <param name="inEndX">寻路终点X坐标</param>
        /// <param name="inEndZ">寻路终点Z坐标</param>
        /// <param name="range">寻路的范围</param>
        /// <param name="result">找到的路的坐标列表</param>
        /// <param name="reFind">重新查找，默认为true</param>
        /// <param name="checkDirLine">检查线路方向，默认为false</param>
        /// <returns>返回寻路结果</returns>
        public CSUtility.Navigation.INavigationWrapper.enNavFindPathResult FindPath(Guid mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ, int range, ref List<SlimDX.Vector2> result, bool reFind = true, bool checkDirLine = false)
        {
            if (mNavigation == null || mNavData == null)
                return CSUtility.Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Error;

            var retValue = mNavigation.FindPath(mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, range, mNavData, mPathFindContext, mNavPointData, mPathFindContext_NavigationPoint, ref result);
//            var retValue = mNavigation.FindPath_NavTile(inStartX, inStartZ, inEndX, inEndZ, range, mNavData, mPathFindContext, ref result, reFind, checkDirLine);
//            var retValue = mNavigation.FindPath_NavPt(inStartX, 0, inStartZ, inEndX, 0, inEndZ, mNavPointData, mPathFindContext_NavigationPoint, ref result);

            if (OnFindPath != null)
                OnFindPath(result);

            return retValue;
        }
        /// <summary>
        /// 设置最大的寻路步数
        /// </summary>
        /// <param name="maxStep">寻路的最大步数</param>
        public void SetMaxStep(int maxStep)
        {
            if (mNavigation != null)
                mNavigation.SetMaxStep(maxStep);
        }
        /// <summary>
        /// 获取最大的步数
        /// </summary>
        /// <returns>返回最大步数</returns>
        public int GetMaxStep()
        {
            if (mNavigation != null)
                mNavigation.GetMaxStep();

            return 0;
        }
        /// <summary>
        /// 获取距离起始点最远的路点
        /// </summary>
        /// <param name="mapInstanceId">地图对象ID</param>
        /// <param name="inStartX">开始寻路的起始点X坐标</param>
        /// <param name="inStartZ">开始寻路的起始点Z坐标</param>
        /// <param name="inEndX">终点的X坐标</param>
        /// <param name="inEndZ">终点的Z坐标</param>
        /// <param name="outX">最远点的X坐标</param>
        /// <param name="outZ">最远点的Z坐标</param>
        /// <returns>找到最远点返回true，否则返回false</returns>
        public bool GetFarthestPathPointFromStartInLine(Guid mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ, out float outX, out float outZ)
        {
            outX = 0;
            outZ = 0;

            if (mNavigation != null)
                return mNavigation.GetFarthestPathPointFromStartInLine(mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, out outX, out outZ, mNavData);

            return false;
        }
        /// <summary>
        /// 是否有障碍
        /// </summary>
        /// <param name="mapInstanceId">地图对象ID</param>
        /// <param name="inStartX">寻路起点X坐标</param>
        /// <param name="inStartZ">寻路起点Z坐标</param>
        /// <param name="inEndX">寻路的终点X坐标</param>
        /// <param name="inEndZ">寻路的终点Z坐标</param>
        /// <returns>如果有障碍返回true，否则返回false</returns>
        public bool HasBarrier(Guid mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ)
        {
            if (mNavigation != null)
                return mNavigation.HasBarrier(mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, mNavData);

            return true;
        }
    }
}
