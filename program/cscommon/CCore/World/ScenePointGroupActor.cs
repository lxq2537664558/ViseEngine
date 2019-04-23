using System;

namespace CCore.World
{
    /// <summary>
    /// 场景点组的初始化类
    /// </summary>
    public class ScenePointGroupActorInit : CCore.World.ActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ScenePointGroupActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.ScenePoint;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager;
        }
    }
    /// <summary>
    /// 场景点组类
    /// </summary>
    public class ScenePointGroupActor : CCore.World.Actor
    {
        CSUtility.Map.ScenePointGroup mHostGroup;
        CCore.Component.ScenePointGroupVisual mSPGVisual;
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="hostGroup">主场景点组</param>
        public ScenePointGroupActor(CSUtility.Map.ScenePointGroup hostGroup)
        {
            mHostGroup = hostGroup;
            hostGroup.OnPointsChanged += this._OnPointsChanged;
            hostGroup.OnLineTypeChanged += this._OnLineTypeChanged;

            foreach (var point in mHostGroup.Points)
            {
                point.OnMatrixChanged += this._OnPointMatrixChanged;
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~ScenePointGroupActor()
        {
            foreach (var point in mHostGroup.Points)
            {
                point.OnMatrixChanged -= this._OnPointMatrixChanged;
            }

            if (mHostGroup != null)
                mHostGroup.OnPointsChanged -= this._OnPointsChanged;
        }
        /// <summary>
        /// 场景点组对象的初始化
        /// </summary>
        /// <param name="_init">用于初始化场景点组对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            if (base.Initialize(_init) == false)
                return false;

            mPlacement = new CSUtility.Component.StandardPlacement(this);
            mSPGVisual = new CCore.Component.ScenePointGroupVisual();
            Visual = mSPGVisual;

            Update();

            return true;
        }
        /// <summary>
        /// 更新场景点组
        /// </summary>
        public void Update()
        {
            if(mSPGVisual != null)
                mSPGVisual.UpdatePoints(mHostGroup);
        }

        private void _OnLineTypeChanged()
        {
            Update();
        }

        private void _OnPointsChanged()
        {
            if(mHostGroup == null)
                return;

            foreach (var point in mHostGroup.Points)
            {
                point.OnMatrixChanged -= this._OnPointMatrixChanged;
                point.OnMatrixChanged += this._OnPointMatrixChanged;
            }

            Update();
        }

        private void _OnPointMatrixChanged(CSUtility.Map.ScenePoint pt)
        {
            Update();
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

            if(mHostGroup != null && mSPGVisual != null && mHostGroup.SPGType != mSPGVisual.PointType)
            {
                mSPGVisual.PointType = mHostGroup.SPGType;
            }
        }
    }
}
