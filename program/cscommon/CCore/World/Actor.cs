using System;
using System.Collections.Generic;
using System.ComponentModel;
using CSUtility.Support;
using SlimDX;
/// <summary>
/// 世界对象的命名空间
/// </summary>
namespace CCore.World
{
    /// <summary>
    /// Actor的初始化类
    /// </summary>
    public class ActorInit : CSUtility.Component.ActorInitBase
    {
        /// <summary>
        /// 是否计算阴影，默认为true
        /// </summary>
        protected bool mCastShadow = true;
        /// <summary>
        /// 是否计算阴影
        /// </summary>
		[CSUtility.Support.AutoSaveLoadAttribute]
        public bool CastShadow
        {
            get { return mCastShadow; }
            set { mCastShadow = value; }
        }
        /// <summary>
        /// 是否接受阴影，默认为true
        /// </summary>
        protected bool mAcceptShadow = true;
        /// <summary>
        /// 是否接受阴影
        /// </summary>
		[CSUtility.Support.AutoSaveLoadAttribute]
        public bool AcceptShadow
        {
            get { return mAcceptShadow; }
            set { mAcceptShadow = value; }
        }
        /// <summary>
        /// 层属性
        /// </summary>
        protected CCore.RLayer mLayer = CCore.RLayer.RL_None;
        /// <summary>
        /// 所在层属性
        /// </summary>
		[CSUtility.Support.AutoSaveLoadAttribute]
        public CCore.RLayer Layer
        {
            get { return mLayer; }
            set { mLayer = value; }
        }

        bool mVisibleCheck = true;
        /// <summary>
        /// 可视化检查，默认为true
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool VisibleCheck
        {
            get { return mVisibleCheck; }
            set
            {
                mVisibleCheck = value;
            }
        }

        bool mVisibleCheckOBB = false;
        /// <summary>
        /// OBB包围盒的可视化检查
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool VisibleCheckOBB
        {
            get { return mVisibleCheckOBB; }
            set { mVisibleCheckOBB = value; }
        }

        bool mEnableHitProxyInGame = false;
        /// <summary>
        /// 游戏中可以点选
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool EnableHitProxyInGame
        {
            get { return mEnableHitProxyInGame; }
            set { mEnableHitProxyInGame = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Common;
            SceneFlag = CSUtility.Component.enActorSceneFlag.Static;
        }
    }

    //游戏中的逻辑对象
    /// <summary>
    /// Actor类，也是游戏中的逻辑对象
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public class Actor : CSUtility.Component.ActorBase
    {
        /// <summary>
        /// 只读属性，该对象是否为空
        /// </summary>
        [Browsable(false)]
        public virtual bool IsNullActor
        {
            get { return false; }
        }
        private IntPtr mActorPtr = IntPtr.Zero;
        /// <summary>
        /// 只读属性，Actor对象的指针
        /// </summary>
        [Browsable(false)]
        public IntPtr ActorPtr
        {
            get { return mActorPtr; }
        }
        /// <summary>
        /// Actor对象的ID
        /// </summary>
        protected Guid mId = Guid.NewGuid();
        /// <summary>
        /// Actor对象的ID
        /// </summary>
        [Browsable(false)]
        public override Guid Id
        {
	        get { return mId; }
            protected set { mId = value; }
        }
        /// <summary>
        /// 检查OBB包围盒是否可视化
        /// </summary>
        [Browsable(false)]
        public bool VisibleCheckOBB
        {
            get 
            {
                var init = ActorInit as ActorInit;
                if (init == null)
                    return false;
                return init.VisibleCheckOBB; 
            }
            set 
            {
                var init = ActorInit as ActorInit;
                if (init == null)
                    return;
                init.VisibleCheckOBB = value; 
            }
        }
        /// <summary>
        /// 加载完成，默认为true
        /// </summary>
        public bool LoadFinished = true;
        /// <summary>
        /// 是否可见
        /// </summary>
        [Browsable(false)]
        public virtual bool Visible
        {
            get
            {
                if (mVisual != null)
                    return mVisual.Visible;
                return false;
            }
            set
            {
                if(mVisual != null)
                    mVisual.Visible = value;
            }
        }
        /// <summary>
        /// 对象所在世界
        /// </summary>
        protected World mWorld;
        /// <summary>
        /// 对象所在世界
        /// </summary>
        [Browsable(false)]
        public virtual World World
        {
            get { return mWorld; }
            set { mWorld = value; }
        }
        /// <summary>
        /// 位置属性是否改变,默认为true
        /// </summary>
        protected bool mPlacementChanged = true;
        /// <summary>
        /// 可视化的最小顶点坐标
        /// </summary>
        protected SlimDX.Vector3 mPrevVisualMin;
        /// <summary>
        /// 可视化的最大顶点坐标
        /// </summary>
        protected SlimDX.Vector3 mPrevVisualMax;
        /// <summary>
        /// AABB包围盒对象
        /// </summary>
        protected SlimDX.BoundingBox mAABB;
        private CCore.Component.Visual mVisual;  // 可视
        /// <summary>
        /// 可视化对象
        /// </summary>
        [Browsable(false)]
        public virtual CCore.Component.Visual Visual
        {
            get { return mVisual; }
            set
            {
                mVisual = value;
                if(mVisual != null)
                    mVisual.HostActor = this;
            }
        }
        /// <summary>
        /// 是否产生阴影
        /// </summary>
        [System.ComponentModel.Category("属性")]
		[System.ComponentModel.DisplayName("产生阴影")]
        public bool CastShadow
        {
            get
            {
                var init = mActorInit as ActorInit;
                if(init != null)
                    return init.CastShadow;
                return false;
            }
            set
            {
                var init = mActorInit as ActorInit;
                if(init != null)
                    init.CastShadow = value;
            }
        }
        /// <summary>
        /// 是否接受阴影
        /// </summary>
        [System.ComponentModel.Category("属性")]
		[System.ComponentModel.DisplayName("接受阴影")]
        public bool AcceptShadow
        {
            get
            {
                var init = mActorInit as ActorInit;
                if(init != null)
                    return init.AcceptShadow;
                return false;
            }
            set
            {
                var init = mActorInit as ActorInit;
                if(init != null)
                    init.AcceptShadow = value;
            }
        }
        /// <summary>
        /// 渲染层
        /// </summary>
		[System.ComponentModel.Category("属性")]
        [System.ComponentModel.DisplayName("渲染层")]
        public RLayer Layer
        {
            get
            {
                var init = mActorInit as ActorInit;
                if(init != null)
                    return init.Layer;
                return RLayer.RL_None;
            }
            set
            {
                var init = mActorInit as ActorInit;
                if(init != null)
                    init.Layer = value;
            }
        }
        /// <summary>
        /// 游戏中可点选
        /// </summary>
		[System.ComponentModel.Category("属性")]
        [System.ComponentModel.DisplayName("游戏中可点选")]
        public bool EnableHitProxyInGame
        {
            get
            {
                var init = mActorInit as ActorInit;
                if (init != null)
                    return init.EnableHitProxyInGame;
                return false;
            }
            set
            {
                var init = mActorInit as ActorInit;
                if (init != null)
                    init.EnableHitProxyInGame = value;
            }
        }
        /// <summary>
        /// 其他组件列表
        /// </summary>
        protected Dictionary<System.Type, CSUtility.Component.IComponent> mComps; //其他组件
        /// <summary>
        /// 渲染的上一帧
        /// </summary>
        public UInt32 LastRenderFrame;
        /// <summary>
        /// 是否参与连线检查运算
        /// </summary>
        public bool ParticipationLineCheck = true;    // true 参与LineCheck运算
        /// <summary>
        /// 是否根据与主场景的摄像机距离，来决定更新频率
        /// </summary>
        public bool mUpdateAnimByDistance = true;   // 根据与主场景的摄像机距离，来决定更新频率。
        /// <summary>
        /// 是否正在渲染当前帧
        /// </summary>
        /// <returns>如果正在渲染当前帧返回true，否则返回false</returns>
        public bool IsCurFrameRender()
        {
            return (LastRenderFrame == Engine.Instance.CurRenderFrame);
        }
        /// <summary>
        /// 声明该对象从当前场景删除时调用的委托事件
        /// </summary>
        /// <param name="actor">Actor对象</param>
        /// <param name="scene">当前场景对象</param>
        public delegate void Delegate_OnActorRemoveFromScene(Actor actor, CCore.Scene.SceneGraph scene);
        /// <summary>
        /// 定义该对象从当前场景删除时调用的委托事件
        /// </summary>
        public event Delegate_OnActorRemoveFromScene OnActorRemoveFromScene;
        /// <summary>
        /// 声明该对象进入当前场景时调用的委托事件
        /// </summary>
        /// <param name="actor">Actor对象</param>
        /// <param name="scene">当前场景对象</param>
        public delegate void Delegate_OnActorEnterScene(Actor actor, CCore.Scene.SceneGraph scene);
        /// <summary>
        /// 定义该对象进入当前场景时调用的委托事件
        /// </summary>
        public event Delegate_OnActorEnterScene OnActorEnterScene;
        /// <summary>
        /// 对象从场景中删除
        /// </summary>
        /// <param name="scene">场景对象</param>
        public virtual void OnRemoveFromScene(CCore.Scene.SceneGraph scene)
        {
            if (OnActorRemoveFromScene != null)
                OnActorRemoveFromScene(this, scene);
        }
        /// <summary>
        /// 对象进入场景
        /// </summary>
        /// <param name="scene">场景对象</param>
        public virtual void OnEnterScene(CCore.Scene.SceneGraph scene)
        {
            if (OnActorEnterScene != null)
                OnActorEnterScene(this, scene);
        }

        static CCore.Scene.TileScene.Delegate_TileObject_Cleanup cuEvent = CCore.Scene.TileScene.TileObject.UnBindFromSceneGraph;
        static CCore.Scene.TileScene.Delegate_TileObject_PreUse puEvent = CCore.Scene.TileScene.TileObject.PreUse;
        static CCore.Scene.TileScene.Delegate_TileObject_GetAABB gAABBEvent = CCore.Scene.TileScene.TileObject.GetAABB;
        static CCore.Scene.TileScene.Delegate_TileObject_GetLocation glEvnet = CCore.Scene.TileScene.TileObject.GetLocation;
        static CCore.Scene.TileScene.Delegate_TileObject_GetOriginLocation glEvent = CCore.Scene.TileScene.TileObject.GetOriginLocation;
        static CCore.Scene.TileScene.Delegate_TileObject_LineCheck lcEvent = CCore.Scene.TileScene.TileObject.LineCheck;
        static CCore.Scene.TileScene.Delegate_TileObject_HasFlag hfEvent = CCore.Scene.TileScene.TileObject.HasFlag;
        static CCore.Scene.TileScene.Delegate_TileObject_SaveActor saEvent = CCore.Scene.TileScene.TileObject.SaveActor;
        static CCore.Scene.TileScene.Delegate_TileObject_LoadActor laEvent = CCore.Scene.TileScene.TileObject.LoadActor;
        static CCore.Scene.TileScene.Delegate_TileObject_GetTypeName gtnEvent = CCore.Scene.TileScene.TileObject.GetTypeName;
        static CCore.Scene.TileScene.Delegate_TileObject_ReleaseTypeName rtnEvent = CCore.Scene.TileScene.TileObject.ReleaseTypeName;
        //static CCore.Scene.TileScene.Delegate_TileObject_IsDynamic idEvent = CCore.Scene.TileScene.TileObject.IsDynamic;
        static CCore.Scene.TileScene.Delegate_TileObject_GetID giEvent = CCore.Scene.TileScene.TileObject.GetID;
        static CCore.Scene.TileScene.Delegate_TileObject_GetGameType ggtEvent = CCore.Scene.TileScene.TileObject.GetGameType;
        static CCore.Scene.TileScene.Delegate_TileObject_GetSceneFlag gsfEvent = CCore.Scene.TileScene.TileObject.GetSceneFlag;
        static CCore.Scene.TileScene.Delegate_TileObject_RestoreObjects roEvent = CCore.Scene.TileScene.TileObject.RestoreObjects;
        static CCore.Scene.TileScene.Delegate_TileObject_InvalidateObjects ioEvent = CCore.Scene.TileScene.TileObject.InvalidateObjects;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Actor()
        {
            mActorPtr = DllImportAPI.vTileObject_New();

            IntPtr pinActor = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(this));
            DllImportAPI.vTileObject_SetActor(ActorPtr, pinActor);

            DllImportAPI.vTileObject_InitializeEvent(ActorPtr,
                                                          cuEvent,
                                                          puEvent,
                                                          gAABBEvent,
                                                          glEvnet,
                                                          glEvent,
                                                          lcEvent,
                                                          hfEvent,
                                                          saEvent,
                                                          laEvent,
                                                          gtnEvent,
                                                          rtnEvent,
                                                          giEvent,
                                                          ggtEvent,
                                                          gsfEvent,
                                                          roEvent,
                                                          ioEvent);
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~Actor()
        {
            Dispose();
        }
        /// <summary>
        /// 删除对象处理
        /// </summary>
        public void Dispose()
        {
            Cleanup();
            DllImportAPI.vTileObject_Release(mActorPtr);
            mActorPtr = IntPtr.Zero;
        }
        /// <summary>
        /// 从场景中解除
        /// </summary>
        public virtual void OnUnbindFromSceneGraph()
        {
            mWorld = null;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        public virtual void Cleanup()
        {
            OnUnbindFromSceneGraph();
            if(mVisual != null)
            {
                mVisual.Cleanup();
                mVisual = null;
            }
            mComps = null;
        }

        static CSUtility.Performance.PerfCounter mBase_Visual_Timer = new CSUtility.Performance.PerfCounter("Actor.Visual");
        static CSUtility.Performance.PerfCounter mActorBase_Tick_Timer = new CSUtility.Performance.PerfCounter("ActorBase.Tick");
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public override void Tick(long elapsedMillisecond)
        {
            mActorBase_Tick_Timer.Begin();
            base.Tick(elapsedMillisecond);
            mActorBase_Tick_Timer.End();

            mBase_Visual_Timer.Begin();
            if (mVisual != null)
                mVisual?.Tick(this, elapsedMillisecond);
            mBase_Visual_Timer.End();
        }
        /// <summary>
        /// 提前使用
        /// </summary>
        /// <param name="bForce">是否强制从磁盘加载</param>
        /// <param name="time">时间</param>
        public void PreUse(bool bForce, UInt64 time)
        {
            if(mVisual != null)
            {
                mVisual.PreUse(bForce, time);
            }
        }
        /// <summary>
        /// 获取当前Actor的组件
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回Actor上的组件</returns>
        public CSUtility.Component.IComponent GetComponent(System.Type type)
        {
            CSUtility.Component.IComponent comp;
            if(mComps.TryGetValue(type, out comp))
                return comp;

            return null;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <param name="comp">要添加的组件</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public bool AddComponent(System.Type type, CSUtility.Component.IComponent comp)
        {
            //前面做 type的类型检测
		    mComps[type] = comp;
		    return true;
        }
        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="type">类型</param>
        public void RemoveComponent(System.Type type)
        {
            mComps.Remove(type);
        }
        /// <summary>
        /// 连线检查
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <returns>检查正常返回true，否则返回false</returns>
        public override bool LineCheck(ref Vector3 start, ref Vector3 end, ref stHitResult result)
        {
            if (ParticipationLineCheck == false)
            {
                return false;
            }

            if (base.LineCheck(ref start, ref end, ref result))
                return true;
            
            bool bRetValue = false;
            if (bRetValue == false && Visual != null)
                bRetValue = Visual.LineCheck(ref start, ref end, ref mPlacement.mMatrix, ref result);

            return bRetValue;
        }
        
        /// <summary>
        /// 世界下的连线检查
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <returns>检查无问题返回true，否则返回false</returns>
        public override bool WorldLineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result)
        {
            if (World == null)
                return false;
            
            return World.LineCheck(ref start, ref end, ref result);
        }
        /// <summary>
        /// 保存场景数据
        /// </summary>
        /// <param name="attribute">XND数据</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public virtual bool SaveSceneData(CSUtility.Support.XndAttrib attribute)
        {
            attribute.Write(Id);

            System.Diagnostics.Debug.Assert(mActorInit != null);
            var typeString = CSUtility.Program.GetTypeSaveString(mActorInit.GetType());
            attribute.Write(typeString);
            mActorInit.Write(attribute);

		    bool bHasPlacement = Placement != null;
		    attribute.Write(bHasPlacement);
		    if(bHasPlacement)
		    {
			    attribute.Write(CSUtility.Program.GetTypeSaveString(Placement.GetType()));
			    attribute.Write(Placement.GetLocation());
			    attribute.Write(Placement.GetRotation());
			    attribute.Write(Placement.GetScale());
		    }

		    bool bHasVisual = (Visual != null) && (Visual.VisualInit != null);
		    attribute.Write(bHasVisual);
		    if(bHasVisual)
		    {
			    attribute.Write(CSUtility.Program.GetTypeSaveString(Visual.GetType()));
			    attribute.Write(CSUtility.Program.GetTypeSaveString(Visual.VisualInit.GetType()));
			    Visual.VisualInit.Write(attribute);
		    }

		    return true;
        }
        /// <summary>
        /// 加载场景数据
        /// </summary>
        /// <param name="attribute">XND数据</param>
        /// <returns>加载成功返回true，否则返回false</returns>
		public virtual bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
		    Cleanup();

		    attribute.Read(out this.mId);

		    string initTypeStr;
		    attribute.Read(out initTypeStr);
            System.Type initType = CSUtility.Program.GetTypeFromSaveString(initTypeStr);
		    if(initType == null)
		    {
			    initType = typeof(CCore.World.ActorInit);
		    }
		    var actInit = (CSUtility.Component.ActorInitBase)System.Activator.CreateInstance(initType);
		    actInit.Read(attribute);		
		    Initialize(actInit);

		    bool bHasPlacement;
		    attribute.Read(out bHasPlacement);
		    if(bHasPlacement)
		    {
			    System.String typeStr;
			    attribute.Read(out typeStr);

			    System.Object[] paramArray = { this };
                System.Type placementType = CSUtility.Program.GetTypeFromSaveString(typeStr);
			    if(placementType == null)
			    {
				    placementType = typeof(CSUtility.Component.StandardPlacement);
			    }
			    mPlacement = (CSUtility.Component.StandardPlacement)System.Activator.CreateInstance( placementType, paramArray);

			    SlimDX.Vector3 vLoc;
			    attribute.Read(out vLoc);
			    Placement.SetLocation(ref vLoc);
			    SlimDX.Quaternion qRot;
			    attribute.Read(out qRot);
			    Placement.SetRotation(ref qRot);
			    SlimDX.Vector3 vScale;
			    attribute.Read(out vScale);
			    Placement.SetScale(ref vScale);
		    }

		    bool bHasVisual;
		    attribute.Read(out bHasVisual);
		    if(bHasVisual)
		    {
			    System.String typeStr;
			    attribute.Read(out typeStr);
                mVisual = (CCore.Component.Visual)System.Activator.CreateInstance(CSUtility.Program.GetTypeFromSaveString(typeStr));
                if (mVisual == null)
                {
                    bHasVisual = false;
                }

			    attribute.Read(out typeStr);
                var visualInit = (CCore.Component.VisualInit)System.Activator.CreateInstance(CSUtility.Program.GetTypeFromSaveString(typeStr));
			    visualInit.Read(attribute);
                if (mVisual != null)
                    mVisual.Initialize(visualInit, this);
                else
                    bHasVisual = false;
		    }

		    return true;
        }

        // for editor ========================
        /// <summary>
        /// 是否被选择，默认为false
        /// </summary>
        protected bool mIsSelected = false;
        /// <summary>
        /// 声明被选中时调用的委托事件
        /// </summary>
        /// <param name="actor">选中的Actor</param>
        /// <param name="selected">是否被选中</param>
        public delegate void Delegate_OnSelected(Actor actor, bool selected);
        /// <summary>
        /// 定义Actor被选中时调用的委托事件
        /// </summary>
        public event Delegate_OnSelected OnSelected;
        /// <summary>
        /// 鼠标位于Actor上
        /// </summary>
        public virtual void OnMouseEnter(){}
        /// <summary>
        /// 鼠标离开Actor
        /// </summary>
        public virtual void OnMouseLeave(){}
        /// <summary>
        /// 编辑选中的Actor
        /// </summary>
        public virtual void Editor_Selected()
        {
            mIsSelected = true;
            if(Gravity != null)
                Gravity.Suspend = true;
            OnSelected?.Invoke(this, true);
        }
        /// <summary>
        /// 编辑未选中的Actor
        /// </summary>
		public virtual void Editor_UnSelected()
        {
            mIsSelected = false;
            if(Gravity != null)
                Gravity.Suspend = true;
            OnSelected?.Invoke(this, false);
        }
        // ===================================

        // 选中的Actor显示的属性对象
        /// <summary>
        /// 获取选中的Actor显示的属性对象
        /// </summary>
        /// <returns>返回选中的Actor显示的属性对象</returns>
		public virtual System.Object GetShowPropertyObj()
        {
            return this;
        }
        /// <summary>
        /// 获取AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点坐标</param>
        /// <param name="vMax">最大顶点坐标</param>
		public virtual void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
		    if(mPlacement == null)
		    {
			    vMin = SlimDX.Vector3.UnitXYZ;
			    vMax = SlimDX.Vector3.UnitXYZ;
			    return;
		    }
		    if(mVisual == null)
		    {
			    vMin = mPlacement.GetLocation();
			    vMax = mPlacement.GetLocation();
			    return;
		    }
		
		    SlimDX.Vector3 visualMin = SlimDX.Vector3.Zero;
		    SlimDX.Vector3 visualMax = SlimDX.Vector3.Zero;
		    //mVisual.GetAABB(visualMin, visualMin);		
		    GetOrigionAABB(ref visualMin, ref visualMax);

		    if(mPrevVisualMin==visualMin&&mPrevVisualMax==visualMax)
		    {//visual的aabb没变
			    if(mPlacementChanged==false)
			    {//Actor的placement没变
				    vMin = mAABB.Minimum;
				    vMax = mAABB.Maximum;
				    return;
			    }
		    }

		    mPrevVisualMin = visualMin;
		    mPrevVisualMax = visualMax;

		    //				   0─────1  max
		    //				  ╱│    ╱│
		    //	Y			 3─┼───2 │
		    //	│ ╱Z		 │ ╱4──┼─5 
		    //	. ──X	min  7─────6╱
		    SlimDX.Vector3[] mTempVecArray = new SlimDX.Vector3[8];
		    mTempVecArray[0] = new SlimDX.Vector3(visualMin.X, visualMax.Y, visualMax.Z);
		    mTempVecArray[1] = visualMax;
		    mTempVecArray[2] = new SlimDX.Vector3(visualMax.X, visualMax.Y, visualMin.Z);
		    mTempVecArray[3] = new SlimDX.Vector3(visualMin.X, visualMax.Y, visualMin.Z);
		    mTempVecArray[4] = new SlimDX.Vector3(visualMin.X, visualMin.Y, visualMax.Z);
		    mTempVecArray[5] = new SlimDX.Vector3(visualMax.X, visualMin.Y, visualMax.Z);
		    mTempVecArray[6] = new SlimDX.Vector3(visualMax.X, visualMin.Y, visualMin.Z);
		    mTempVecArray[7] = visualMin;

		    SlimDX.Matrix matrix;
		    mPlacement.GetAbsMatrix(out matrix);

		    for(int i=0; i<8; i++)
		    {
			    mTempVecArray[i] = SlimDX.Vector3.TransformCoordinate(mTempVecArray[i], matrix);
		    }

		    vMin = mTempVecArray[0];
		    vMax = mTempVecArray[0];

		    for(int i=0; i<8; i++)
		    {
			    if(vMin.X > mTempVecArray[i].X)
				    vMin.X = mTempVecArray[i].X;
			    if(vMin.Y > mTempVecArray[i].Y)
				    vMin.Y = mTempVecArray[i].Y;
			    if(vMin.Z > mTempVecArray[i].Z)
				    vMin.Z = mTempVecArray[i].Z;

			    if(vMax.X < mTempVecArray[i].X)
				    vMax.X = mTempVecArray[i].X;
			    if(vMax.Y < mTempVecArray[i].Y)
				    vMax.Y = mTempVecArray[i].Y;
			    if(vMax.Z < mTempVecArray[i].Z)
				    vMax.Z = mTempVecArray[i].Z;
		    }

		    mAABB.Minimum = vMin;
		    mAABB.Maximum = vMax;

		    mPlacementChanged = false;
        }
        /// <summary>
        /// 获取原始的AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点坐标</param>
        /// <param name="vMax">最大顶点坐标</param>
        public virtual void GetOrigionAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            if(mVisual == null)
		    {
			    vMin = -SlimDX.Vector3.UnitXYZ;
			    vMax = SlimDX.Vector3.UnitXYZ;
			    return;
		    }

		    mVisual.GetAABB(ref vMin, ref vMax);
        }
        /// <summary>
        /// 提交可视化对象
        /// </summary>
        /// <param name="env">渲染环境</param>
        /// <param name="matrix">位置矩阵</param>
        /// <param name="eye">视野</param>
		public virtual void OnCommitVisual(CCore.Graphics.REnviroment env, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
		{

		}
        /// <summary>
        /// 位置矩阵改变
        /// </summary>
        /// <param name="placement">位置对象</param>
        public override void OnPlacementChanged(CSUtility.Component.IPlacement placement)
        {
            mPlacementChanged = true;
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">位置矩阵</param>
        /// <param name="eye">视野</param>
		public virtual void OnCommit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
		{

		}
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns>返回复制的Actor</returns>
        public virtual Actor Duplicate()
        {
            CSUtility.Component.ActorInitBase copyedActorInit = null;
            if (this.ActorInit != null)
            {
                copyedActorInit = (CSUtility.Component.ActorInitBase)System.Activator.CreateInstance(this.ActorInit.GetType());
                copyedActorInit.CopyFrom(this.ActorInit);
            }
            var srcActorTypeStr = this.GetType().FullName;
            var copyedActor = (Actor)this.GetType().Assembly.CreateInstance(srcActorTypeStr);
            copyedActor.Initialize(copyedActorInit);

            CCore.Component.VisualInit copyedVisInit = null;
            if (this.Visual.VisualInit != null)
            {
                copyedVisInit = (CCore.Component.VisualInit)System.Activator.CreateInstance(this.Visual.VisualInit.GetType());
                copyedVisInit.CopyFrom(this.Visual.VisualInit);
            }
            var copyedVisual = (CCore.Component.Visual)System.Activator.CreateInstance(this.Visual.GetType());
            copyedVisual.Initialize(copyedVisInit, copyedActor);
            copyedActor.Visual = copyedVisual;
            Object[] paramArray = new Object[] { copyedActor };
            var copyedPlacement = (CSUtility.Component.StandardPlacement)System.Activator.CreateInstance(this.Placement.GetType(), paramArray);
            copyedPlacement.SetMatrix(ref mPlacement.mMatrix);
            copyedActor.SetPlacement(copyedPlacement);
            //if (copyedActor is FrameSet.Role.RoleActor)
            //    MidLayer.IEngine.Instance.MainWorld.AddCommActor(copyedActor as ICommActor);
            //else
            //    MidLayer.IEngine.Instance.MainWorld.AddVisualActor(copyedActor);
            copyedActor.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(copyedActor.Id));

            return copyedActor;
        }
    }
}
