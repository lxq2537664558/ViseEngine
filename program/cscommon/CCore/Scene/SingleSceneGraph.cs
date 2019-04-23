using System;
using System.Collections.Generic;

/// <summary>
/// 场景类命名空间
/// </summary>
namespace CCore.Scene
{
    /// <summary>
    /// 单一场景信息
    /// </summary>
    public class SingleSceneGraphInfo : SceneGraphInfo
    {
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="worldInit">世界对象的初始化对象</param>
        public override void Initialize(CCore.World.WorldInit worldInit)
        {

        }
        /// <summary>
        /// 获取场景类型
        /// </summary>
        /// <returns>返回场景类型为单一场景类型</returns>
        public override System.Type GetSceneGraphType()
        {
            return typeof(SingleSceneGraph);
        }
        /// <summary>
        /// 恢复默认设置
        /// </summary>
        public override void ResetDefault()
        {
            
        }
    }
    /// <summary>
    /// 单一场景
    /// </summary>
    public class SingleSceneGraph : SceneGraph
    {
        /// <summary>
        /// 场景中所有的Actor对象
        /// </summary>
        protected List<CCore.World.Actor> mActors = new List<CCore.World.Actor>();
        /// <summary>
        /// 构造函数
        /// </summary>
        public SingleSceneGraph()
        {
            
        }
        /// <summary>
        /// 初始化场景管理
        /// </summary>
        /// <param name="absMapFolder">地图所在路径</param>
        /// <param name="info">场景管理器参数</param>
        /// <param name="hostWorld">使用此场景管理器的地图</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(string absMapFolder, SceneGraphInfo info, CCore.World.World hostWorld)
        {
            return false;
        }
        /// <summary>
        /// 创建场景管理
        /// </summary>
        /// <param name="absMapFolder">地图所在路径</param>
        /// <param name="info">场景管理器参数</param>
        /// <param name="hostWorld">使用此场景管理器的地图</param>
        /// <returns>创建成功返回true，否则返回false</returns>
        public override bool Create(string absMapFolder, SceneGraphInfo info, CCore.World.World hostWorld)
        {
            return false;
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public override void Cleanup()
        {
            if (mActors != null)
            {
                foreach (var act in mActors)
                {
                    act.Cleanup();
                }
                mActors.Clear();
                mActors = null;
            }
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        public override void Tick()
        {
            foreach (var actor in mActors)
            {
                actor.Tick(CCore.Engine.Instance.GetElapsedMillisecond());
            }
        }
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="lock">是否进行绑定</param>
        public override void SetLockCulling(bool @lock) { }
        /// <summary>
        /// 获取绑定对象
        /// </summary>
        /// <returns>没有绑定对象，返回false</returns>
        public override bool GetLockCulling() { return false; }
        /// <summary>
        /// 添加Actor对象
        /// </summary>
        /// <param name="act">Actor对象</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public override bool AddActor(CCore.World.Actor act)
        {
            foreach (var actor in mActors)
            {
                if (actor == act)
                    return true;
            }
            mActors.Add(act);
            return true;
        }
        /// <summary>
        /// 删除相应的Actor对象
        /// </summary>
        /// <param name="act">Actor对象</param>
        /// <returns>成功删除返回true，否则返回false</returns>
        public override bool RemoveActor(CCore.World.Actor act)
        {
            if (act != null)
            {
                mActors.Remove(act);
                return true;
            }

            return false;
        }
        /// <summary>
        /// 删除所有的Actor对象
        /// </summary>
        public override void RemoveAllActor()
        {
            mActors.Clear();
        }
        /// <summary>
        /// 根据ActorID查找Actor对象
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>返回找到的Actor对象</returns>
        public override CCore.World.Actor FindActor(ref Guid id)
        {
            foreach (var act in mActors)
            {
                if(act.Id == id)
                    return act;
            }

            return null;
        }
        /// <summary>
        /// Actor对象的动作
        /// </summary>
        /// <param name="actorType">Actor对象的类型</param>
        /// <param name="process">Actor对象的动作列表</param>
        public override void ProcessActors(UInt16 actorType, Action<World.Actor> process)
        {

        }
        /// <summary>
        /// 获取某一类型的所有Actor对象
        /// </summary>
        /// <param name="actorType">Actor的类型</param>
        /// <returns>返回某一类型的所有Actor对象</returns>
        public override List<CCore.World.Actor> GetActors(UInt16 actorType)
        {
            if(actorType == ((UInt16)CSUtility.Component.EActorGameType.Unknow))
                return mActors;

            List<CCore.World.Actor> retValue = new List<World.Actor>();
            foreach(var actor in mActors)
            {
                if(actor.GameType == actorType)
                    retValue.Add(actor);
            }
            return retValue;
        }
        /// <summary>
        /// 获取对应区域的所有的Actor对象
        /// </summary>
        /// <param name="vStart">起点坐标</param>
        /// <param name="vEnd">终点坐标</param>
        /// <param name="actorType">Actor类型</param>
        /// <returns>返回对应区域的所有的Actor对象</returns>
        public override List<CCore.World.Actor> GetActors(ref SlimDX.Vector3 vStart, ref SlimDX.Vector3 vEnd, UInt16 actorType)
        {
            var retActors = new List<CCore.World.Actor>();

            foreach (var act in mActors)
            {
                SlimDX.Vector3 vMin = SlimDX.Vector3.Zero, vMax = SlimDX.Vector3.Zero;
                act.GetAABB(ref vMin, ref vMax);
                if (!(vMin.X > vEnd.X ||
                     vMin.Y > vEnd.Y ||
                     vMin.Z > vEnd.Z ||
                     vMax.X < vStart.X ||
                     vMax.Y < vStart.Y ||
                     vMax.Z < vStart.Z))
                {
                    retActors.Add(act);
                }
            }

            return retActors;
        }
        /// <summary>
        /// 绘制对象
        /// </summary>
        /// <param name="eye">视野</param>
        /// <param name="env">渲染环境</param>
        /// <param name="shadowLights">光源</param>
        public override void RenderVisible(CCore.Camera.CameraObject eye, CCore.Graphics.REnviroment env, CCore.Light.Light[] shadowLights)
        {
            foreach (var i in mActors)
            {
                SlimDX.Matrix matrix;
                if (i.Placement != null && i.Placement.GetAbsMatrix(out matrix))
                {
                    if (i.Visual != null)
                    {
                        i.Visual.Commit(env, ref matrix, eye);
                        i.OnCommitVisual(env, ref matrix, eye);
                    }
                }
            }
        }
        /// <summary>
        /// 绘制阴影
        /// </summary>
        /// <param name="camera">摄像机</param>
        /// <param name="env">渲染环境</param>
        /// <param name="shadowLights">产生阴影的光源</param>
        public override void RenderShadow(CCore.Camera.CameraObject camera, CCore.Graphics.REnviroment env, CCore.Light.Light[] shadowLights)
        {
            if (shadowLights == null || shadowLights.Length == 0)
                return;

            foreach (var i in mActors)
            {
                SlimDX.Matrix matrix;
                if (i.Placement != null && i.Placement.GetAbsMatrix(out matrix))
                {
                    foreach(var shadowLight in shadowLights)
                        shadowLight.AddVisual(i.Visual.Layer, i.Visual, ref matrix, true);
                }
            }
        }
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
        public override bool RenderNavigation(UInt32 navLvlX, UInt32 navLvlZ, float startX, float startZ, float endX, float endZ, CCore.Navigation.NavigationAssist nav)
        {
            return false;
        }
        /// <summary>
        /// 渲染服务器高度图
        /// </summary>
        /// <param name="lvlX">X轴的长度</param>
        /// <param name="lvlZ">Z轴的长度</param>
        /// <param name="shm">服务器高度图帮助对象</param>
        /// <returns>绘制成功返回true，否则返回false</returns>
        public override bool RenderServerHeightMap(uint lvlX, uint lvlZ, CCore.Support.ServerAltitudeAssist shm)
        {
            return false;
        }
        /// <summary>
        /// 网格线检查
        /// </summary>
        /// <param name="start">线段起始点</param>
        /// <param name="end">线段终点</param>
        /// <param name="result">点击结果</param>
        /// <returns>检查无错误返回true，否则返回false</returns>
        public override bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result)
        {
            //result = new CSUtility.Support.stHitResult();
            return false;
        }
        /// <summary>
        /// 网格线检查
        /// </summary>
        /// <param name="start">线段起点</param>
        /// <param name="end">线段终点</param>
        /// <param name="result">点击结果</param>
        /// <param name="exceptActor">其他的Actor对象列表</param>
        /// <returns>检查无错误返回true，否则返回false</returns>
        public override bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result, List<CCore.World.Actor> exceptActor)
        {
            //result = new CSUtility.Support.stHitResult();
            return false;
        }
        /// <summary>
        /// 保存客户端场景
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="forceSave">是否强制保存</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public override bool SaveScene_ClientScene(string name, bool forceSave)
        {
            return false;
        }
        /// <summary>
        /// 加载客户端场景
        /// </summary>
        /// <param name="mapPath">地图路径</param>
        /// <param name="name">场景名称</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadScene_ClientScene(string mapPath, string name)
        {
            return false;
        }
        //public override bool SaveScene_Trigger(string name, bool forceSave)
        //{
        //    return false;
        //}
        /// <summary>
        /// 保存特殊的场景
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="actorType">Actor类型</param>
        /// <param name="bForceSave">是否强制保存</param>
        /// <returns>成功保存返回true，否则返回false</returns>
        public override bool SaveScene_Special(string path, UInt16 actorType, bool bForceSave)
        {
            return false;
        }
        /// <summary>
        /// 加载特殊场景
        /// </summary>
        /// <param name="mapPath">场景名称</param>
        /// <param name="actorType">Actor类型</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadScene_Special(string mapPath, UInt16 actorType)
        {
            return false;
        }
        /// <summary>
        /// 可访问的场景的X轴和Z轴方向的场景块数量
        /// </summary>
        /// <param name="x">X轴</param>
        /// <param name="z">Z轴</param>
        public override void GetLevelAvailableXZCount(ref uint x, ref uint z)
        {
            x = 1;
            z = 1;
        }
        /// <summary>
        /// 获取每个场景块X轴方向的长度
        /// </summary>
        /// <returns>返回每个场景块X轴方向的长度</returns>
        public override float GetXLengthPerLevel()
        {
            return 1024;
        }
        /// <summary>
        /// 获取每个场景块Z轴方向的长度
        /// </summary>
        /// <returns>返回每个场景块Z轴方向的长度</returns>
        public override float GetZLengthPerLevel()
        {
            return 1024;
        }

        // idu,idv为level的在terrain索引
        /// <summary>
        /// 增加场景块高度
        /// </summary>
        /// <param name="idu">u向高度</param>
        /// <param name="idv">v向高度</param>
        public override void AddLevel(uint idu, uint idv)
        {
        }
        // idu,idv为level的在terrain索引
        /// <summary>
        /// 删除对应高度
        /// </summary>
        /// <param name="idu">u向高度</param>
        /// <param name="idv">v向高度</param>
        public override void DelLevel(uint idu, uint idv)
        {
        }
        /// <summary>
        /// 将坐标转换到块状地图中
        /// </summary>
        /// <param name="x">X轴的坐标</param>
        /// <param name="z">Z轴的坐标</param>
        /// <returns>转换成功返回true，否则返回false</returns>
        public override bool TravelTo(float x, float z) { return false; }
        /// <summary>
        /// 设置边缘间隔
        /// </summary>
        /// <param name="value">间隔距离</param>
        public override void SetNeighborSide(uint value)
        {
        }

    }
}
