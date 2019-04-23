using System;
using System.Collections.Generic;

namespace CCore.World
{
    /// <summary>
    /// 导航点对象的初始化类
    /// </summary>
    public class NavigationPointActorInit : ActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NavigationPointActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.NavigationPoint;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager;
        }
    }
    /// <summary>
    /// 导航点对象类
    /// </summary>
    public class NavigationPointActor : Actor
    {
        CCore.Component.ScenePointVisual mScenePointVisual;

        Dictionary<NavigationPointActor, CCore.Component.Line> mLinkLines = new Dictionary<NavigationPointActor, Component.Line>();
        /// <summary>
        /// 构造函数
        /// </summary>
        public NavigationPointActor()
        {
            mPlacement = new CSUtility.Component.StandardPlacement(this);

            mPlacement.OnLocationChanged += Placement_OnLocationChanged;
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="id">对象的ID</param>
        public NavigationPointActor(Guid id)
        {
            mPlacement = new CSUtility.Component.StandardPlacement(this);

            mPlacement.OnLocationChanged += Placement_OnLocationChanged;

            mId = id;
        }
        /// <summary>
        /// 连线
        /// </summary>
        /// <param name="actor">导航点对象</param>
        /// <returns>返回连接的直线对象</returns>
        public CCore.Component.Line GetLine(NavigationPointActor actor)
        {
            CCore.Component.Line retLint = null;
            if (mLinkLines.TryGetValue(actor, out retLint))
                return retLint;

            return null;
        }
        /// <summary>
        /// 删除所有的链接对象
        /// </summary>
        public void RemoveAllLinks()
        {
            foreach (var lineData in mLinkLines)
            {
                lineData.Key.RemoveLink(this);
            }
            mLinkLines.Clear();
        }
        void Placement_OnLocationChanged(ref SlimDX.Vector3 loc)
        {
            foreach (var line in mLinkLines)
            {
                line.Value.Start = loc + SlimDX.Vector3.UnitY;
                line.Key.GetLine(this).End = loc + SlimDX.Vector3.UnitY;
            }

            CCore.Navigation.Navigation.Instance.NavigationPointData.MoveNavigationPoint(Id, loc.X, loc.Y, loc.Z);
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">用于初始化该对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            if (base.Initialize(_init) == false)
                return false;

            this.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            mScenePointVisual = new CCore.Component.ScenePointVisual();
            mScenePointVisual.PointType = CSUtility.Map.ScenePointGroup.enScenePointGroupType.NavigationPoint;
            Visual = mScenePointVisual;
            Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(this.Id));

            return true;
        }
        /// <summary>
        /// 更新连接
        /// </summary>
        /// <param name="actors">导航点列表</param>
        public void UpdateLinks(NavigationPointActor[] actors)
        {
            mLinkLines.Clear();

            var pos = this.Placement.GetLocation();
            foreach (var actor in actors)
            {
                var line = new CCore.Component.Line();
                line.Color = CSUtility.Support.Color.LightGreen;
                line.Start = pos + SlimDX.Vector3.UnitY;
                line.End = actor.Placement.GetLocation() + SlimDX.Vector3.UnitY;
                mLinkLines[actor] = line;
            }
        }
        /// <summary>
        /// 添加连接点
        /// </summary>
        /// <param name="actor">导航点对象</param>
        public void AddLink(NavigationPointActor actor)
        {
            if (mLinkLines.ContainsKey(actor))
                return;

            var pos = this.Placement.GetLocation();
            var line = new CCore.Component.Line();
            line.Color = CSUtility.Support.Color.LightGreen;
            line.Start = pos + SlimDX.Vector3.UnitY;
            line.End = actor.Placement.GetLocation() + SlimDX.Vector3.UnitY;
            mLinkLines[actor] = line;
        }
        /// <summary>
        /// 删除连接
        /// </summary>
        /// <param name="actor">导航点对象</param>
        public void RemoveLink(NavigationPointActor actor)
        {
            mLinkLines.Remove(actor);
        }
        /// <summary>
        /// 提交可视化对象
        /// </summary>
        /// <param name="env">渲染环境</param>
        /// <param name="matrix">位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void OnCommitVisual(CCore.Graphics.REnviroment env, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            //if (!CCore.Client.MainWorldInstance.IsActorGameTypeShow((UInt16)CSUtility.Component.EActorGameType.NavigationPoint))
            //    return;
            if (!CCore.Program.IsActorTypeShow(World, CCore.Program.NavigationPointTypeName))
                return;

            base.OnCommitVisual(env, ref matrix, eye);

            // 显示连线
            var mat = SlimDX.Matrix.Identity;
            foreach (var line in mLinkLines)
            {
                line.Value.Commit(env, ref mat, eye);
            }
        }
    }
}
