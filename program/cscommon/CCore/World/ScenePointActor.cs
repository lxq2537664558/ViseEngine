using System;

namespace CCore.World
{
    //     [EditorCommon.Assist.DelegateMethodEditor_AllowedDelegate("Point")]
    //     public delegate bool FOnScenePoint(CSCommon.AISystem.StateHost host);
    /// <summary>
    /// 场景点的初始化类
    /// </summary>
    public class ScenePointActorInit : CCore.World.ActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ScenePointActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.ScenePoint;
            SceneFlag = CSUtility.Component.enActorSceneFlag.Dynamic_Origion;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager;
        }
    }
    /// <summary>
    /// 场景点类
    /// </summary>
    public class ScenePointActor : CCore.World.Actor
    {
        CCore.Component.ScenePointVisual mScenePointVisual;
        /// <summary>
        /// 场景点组的类型
        /// </summary>
        public CSUtility.Map.ScenePointGroup.enScenePointGroupType PointType
        {
            get
            {
                if (mScenePointVisual != null)
                    return mScenePointVisual.PointType;

                return CSUtility.Map.ScenePointGroup.enScenePointGroupType.ScenePoint;
            }
            set
            {
                if (mScenePointVisual != null)
                    mScenePointVisual.PointType = value;
            }
        }

        CSUtility.Map.ScenePointGroup mHostGroup;
        CSUtility.Map.ScenePoint mHostPoint;
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="hostPoint">主场景点</param>
        public ScenePointActor(CSUtility.Map.ScenePoint hostPoint)
        {
            mHostPoint = hostPoint;
            mHostGroup = hostPoint.HostGroup;
            mPlacement = new CSUtility.Component.StandardPlacement(this);
            var mat = hostPoint.TransMatrix;
            mPlacement.SetMatrix(ref mat);
            mPlacement.OnLocationChanged += Placement_OnLocationChanged;
            mPlacement.OnRotationChanged += Placement_OnRotationChanged;
            mPlacement.OnScaleChanged += Placement_OnScaleChanged;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~ScenePointActor()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除场景点位置挂接的函数
        /// </summary>
        public override void Cleanup()
        {
            mPlacement.OnLocationChanged -= Placement_OnLocationChanged;
            mPlacement.OnRotationChanged -= Placement_OnRotationChanged;
            mPlacement.OnScaleChanged -= Placement_OnScaleChanged;
        }

        //         #region 回调
        //         CSCommon.Helper.EventCallBack mOnScenePointCB;
        //         [System.ComponentModel.Browsable(false)]
        //         public CSCommon.Helper.EventCallBack OnScenePointCB
        //         {
        //             get { return mOnScenePointCB; }
        //         }
        //         Guid mOnScenePoint = Guid.Empty;
        // //         [CSUtility.Support.AutoSaveLoad]
        // //         [CSUtility.Support.AutoCopy]
        // //         [System.ComponentModel.CategoryAttribute("回调")]
        //         [EditorCommon.Assist.DelegateMethodEditor_DelegateType(typeof(FOnScenePoint))]
        //         public Guid OnScenePoint
        //         {
        //             get { return mOnScenePoint; }
        //             set
        //             {
        //                 mOnScenePoint = value;
        //                 mOnScenePointCB = CSCommon.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnScenePoint), value);
        //             }
        //         }
        //         #endregion

        void Placement_OnLocationChanged(ref SlimDX.Vector3 loc)
        {
            mHostPoint.TransMatrix = mPlacement.mMatrix;
        }
        void Placement_OnRotationChanged(ref SlimDX.Quaternion rot)
        {
            mHostPoint.TransMatrix = mPlacement.mMatrix;
        }
        void Placement_OnScaleChanged(ref SlimDX.Vector3 scale)
        {
            mHostPoint.TransMatrix = mPlacement.mMatrix;
        }
        /// <summary>
        /// 场景点对象的初始化
        /// </summary>
        /// <param name="_init">用于初始化场景点的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            if (base.Initialize(_init) == false)
                return false;

            mScenePointVisual = new CCore.Component.ScenePointVisual();
            //Visual = mScenePointVisual;
            Visual = mScenePointVisual;
            Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(this.Id));
            
            return true;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

            if (mHostGroup != null && mHostGroup.SPGType != PointType)
            {
                PointType = mHostGroup.SPGType;
            }
        }
        /// <summary>
        /// 获取显示属性的场景点对象
        /// </summary>
        /// <returns>返回当前场景点对象</returns>
        public override object GetShowPropertyObj()
        {
            return base.GetShowPropertyObj();
        }
    }
}
