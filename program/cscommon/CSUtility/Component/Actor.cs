using System;
using System.ComponentModel;

namespace CSUtility.Component
{
    // 此数据属于存盘数据，请勿修改顺序
    /// <summary>
    /// Actor的游戏类型枚举
    /// </summary>
    public enum EActorGameType : UInt16
    {
        Unknow = 0,
        Common = 1,
        Player = 2,
        Npc = 3,
        Light = 4,
        Decal = 5,
        NpcInitializer = 6,
        Potal = 7,
        Prefab = 8,
        Trigger = 9,
        Effect = 10,
        EffectNpc = 11,
        DropedItem = 12,
        GatherItem = 13,
        ScenePoint = 14,
        NavigationPoint = 15,
        Sound = 16,
        DynamicBlock = 17,

        // 前10000位为引擎预留，请勿使用及修改
        Other = 10000,
    }

    /// <summary>
    /// 对象场景管理标记，与引擎内保持一致，请勿修改
    /// </summary>
    public enum enActorSceneFlag : byte
    {
        Unknow,
        Static,             // 静态对象             "TileObjects"
        Trigger,            // 触发器               "TileObjects"
        Dynamic_Loc,        // 动态，按位置计算      "DynamicTileObjects"
        Dynamic_BoundBox,   // 动态，按包围盒计算    "DynamicTileObjects"
        Dynamic_Origion,    // 动态，原点           "DynamicOriTileObjects"
    }

    //public unsafe struct v3dxVector3 { };
    /// <summary>
    /// 初始化Actor的基类
    /// </summary>
    public class ActorInitBase : CSUtility.Support.XndSaveLoadProxy
    {
        // 与vSceneGraph::vTileObject::EActorFlag对应
        // 若修改则两处都要修改
        /// <summary>
        /// Actor标记的枚举
        /// </summary>
        public enum EActorFlag
        {
            None            = 0,
            SaveWithClient  = 1,
            SaveWithServer  = 1 << 1,
            ForEditor       = 1 << 2,
            WithOutSceneManager = 1 << 3,

            IgnoreMouseLineCheckInGame = 1 << 4,
        }

        private EActorFlag mActorFlag;
        /// <summary>
        /// Actor标记
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public EActorFlag ActorFlag
        {
            get { return mActorFlag; }
            set { mActorFlag = value; }
        }
        
        private UInt16 mGameType = 0;
        /// <summary>
        /// 对象类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public UInt16 GameType
        {
            get { return mGameType; }
            set { mGameType = value; }
        }

        CSUtility.Component.enActorSceneFlag mSceneFlag = CSUtility.Component.enActorSceneFlag.Unknow;
        /// <summary>
        /// 场景管理标记
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public CSUtility.Component.enActorSceneFlag SceneFlag
        {
            get { return mSceneFlag; }
            set { mSceneFlag = value; }
        }

        private string mActorName = "";
        /// <summary>
        /// Actor的名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string ActorName
        {
            get { return mActorName; }
            set { mActorName = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ActorInitBase()
        {
            ActorFlag = EActorFlag.None;
        }
    }
    /// <summary>
    /// Actor的基类
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public class ActorBase : INotifyPropertyChanged
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
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //protected System.Guid mId;
        /// <summary>
        /// 该对象的ID
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public virtual System.Guid Id
        {
            get;
            protected set;
        }
        /// <summary>
        /// Actor对象的名称
        /// </summary>
        [Category("属性")]
        [DisplayName("名称")]
        [Browsable(false)]
        public virtual string ActorName
        {
            get
            {
                if(mActorInit != null)
                    return mActorInit.ActorName;
                return "";
            }
            set
            {
                if(mActorInit != null)
                    mActorInit.ActorName = value;

                OnPropertyChanged("ActorName");
            }
        }
        /// <summary>
        /// 获取AI状态
        /// </summary>
        /// <returns>返回该对象的状态</returns>
        public virtual CSUtility.AISystem.IStateHost GetStateHost()
        {
            return null;
        }
        /// <summary>
        /// 设置对象ID
        /// </summary>
        /// <param name="id">对象ID</param>
        public virtual void _SetId(Guid id)
        {
            Id = id;
        }
        /// <summary>
        /// 获取对象所在的层名称
        /// </summary>
        /// <returns>返回该对象的层名称为Other</returns>
        public virtual string GetLayerName()
        {
            return "Other";
        }
        /// <summary>
        /// 父类对象
        /// </summary>
        protected ActorBase mParent;
        /// <summary>
        /// 父对象
        /// </summary>
		[System.ComponentModel.Browsable(false)]
        public ActorBase Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        #region Component
        /// <summary>
        /// 位置信息
        /// </summary>
        protected StandardPlacement mPlacement;
        /// <summary>
        /// 只读属性，对象的位置信息
        /// </summary>
		[System.ComponentModel.Browsable(false)]
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.属性.位置信息", CSUtility.Helper.enCSType.Common, "场景对象的位置变换属性")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.属性.位置信息", CSUtility.Helper.enCSType.Common, "场景对象的位置变换属性")]
        [CSUtility.AISystem.Attribute.ToolTip("位置信息")]
        public StandardPlacement Placement
        {
            get { return mPlacement; }
        }
        /// <summary>
        /// 对象的重力
        /// </summary>
        protected IGravityComp mGravity;
        /// <summary>
        /// 只读属性，重力值
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public IGravityComp Gravity
        {
            get { return mGravity; }
        }
        /// <summary>
        /// 阴影对象
        /// </summary>
        protected IShape mShape;
        /// <summary>
        /// 只读属性，对象的阴影
        /// </summary>
		[System.ComponentModel.Browsable(false)]
        public IShape Shape
        {
            get { return mShape; }
        }
        #endregion
        /// <summary>
        /// Actor的初始化对象
        /// </summary>
        protected ActorInitBase mActorInit;
        /// <summary>
        /// 只读属性，Actor的初始化对象
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public ActorInitBase ActorInit
        {
            get { return mActorInit; }
        }
        /// <summary>
        /// 只读属性，游戏类型
        /// </summary>
		[System.ComponentModel.Browsable(false)]
        public UInt16 GameType
        {
            get
            {
                if (mActorInit == null)
                    return 0;
                return mActorInit.GameType;
            }
            //set
            //{
            //    if(mActorInit != null)
            //        mActorInit.GameType = value;
            //}
        }
        /// <summary>
        /// 只读属性，场景标记
        /// </summary>
		[System.ComponentModel.Browsable(false)]
        public CSUtility.Component.enActorSceneFlag SceneFlag
        {
            get
            {
                if (mActorInit == null)
                    return CSUtility.Component.enActorSceneFlag.Unknow;
                return mActorInit.SceneFlag;
            }
        }

        #region flag
        /// <summary>
        /// 是否需要连线检测
        /// </summary>
        /// <param name="flags">标记</param>
        /// <returns>返回false</returns>
        public virtual bool NeedLineCheck(UInt32 flags)
        {
            return false;
        }
        /// <summary>
        /// 是否含有指定标记
        /// </summary>
        /// <param name="flag">Actor标记</param>
        /// <returns>如果含有指定标记返回true，否则返回false</returns>
        public virtual bool HasFlag(ActorInitBase.EActorFlag flag)
        {
            if (mActorInit == null)
                return false;

            return (mActorInit.ActorFlag & flag) == flag;
        }
        /// <summary>
        /// 添加标记
        /// </summary>
        /// <param name="flag">Actor标记</param>
        public void AddFlag(ActorInitBase.EActorFlag flag)
        {
            if (mActorInit != null)
            {
                mActorInit.ActorFlag = mActorInit.ActorFlag | flag;
            }
        }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="flag">Actor标记</param>
        public void RemoveFlag(ActorInitBase.EActorFlag flag)
        {
            if (mActorInit != null)
            {
                mActorInit.ActorFlag = mActorInit.ActorFlag & ~flag;
            }
        }

        #endregion
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="init">Actor的初始化对象</param>
        /// <returns>初始化成功返回true</returns>
        public virtual bool Initialize(ActorInitBase init)
        {
            mActorInit = init;
            return true;
        }
        /// <summary>
        /// 设置对象的位置
        /// </summary>
        /// <param name="placement">对象的位置</param>
        public void SetPlacement(StandardPlacement placement)
        {
            mPlacement = placement;
        }
        /// <summary>
        /// 对象的ID
        /// </summary>
        /// <returns>对象ID</returns>
        public static System.Guid GenId()
        {
            return System.Guid.NewGuid();
        }
        static CSUtility.Performance.PerfCounter mActorBase_Placement_Timer = new CSUtility.Performance.PerfCounter("ActorBase.Placement");
        static CSUtility.Performance.PerfCounter mActorBase_Gravity_Timer = new CSUtility.Performance.PerfCounter("ActorBase.Gravity");
        static CSUtility.Performance.PerfCounter mActorBase_Shape_Timer = new CSUtility.Performance.PerfCounter("ActorBase.Shape");
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public virtual void Tick(Int64 elapsedMillisecond)
        {
            if (mPlacement != null)
            {
                mActorBase_Placement_Timer.Begin();
                mPlacement.Tick(this, elapsedMillisecond);
                mActorBase_Placement_Timer.End();
            }
                
            if (mGravity != null)
            {
                mActorBase_Gravity_Timer.Begin();
                mGravity.Tick(this, elapsedMillisecond);
                mActorBase_Gravity_Timer.End();
            }
                
            if (mShape != null)
            {
                mActorBase_Shape_Timer.Begin();
                mShape.Tick(this, elapsedMillisecond);
                mActorBase_Shape_Timer.End();
            }   
        }

        //射线检测是否穿过我自己
        /// <summary>
        /// 连线检测
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <returns>检测成功返回true，否则返回false</returns>
        public virtual bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result)
        {
            if (Shape != null)
                return Shape.LineCheck(ref start, ref end, ref Placement.mMatrix, ref result);
            //else
            //    result = new CSUtility.Support.stHitResult();

            return false;
        }
        //世界内检测射线穿越
        /// <summary>
        /// 世界内检测射线穿越
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <returns>检测成功返回true，否则返回false</returns>
        public virtual bool WorldLineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result)
        {
            //result = new CSUtility.Support.stHitResult();
            //mHostActor->World->Collision->LineCheck( start , end , result ) 
            return false;
        }
        /// <summary>
        /// 设置对象的mesh
        /// </summary>
        /// <param name="templateId">mesh对象ID</param>
        /// <param name="forceLoad">是否强制从磁盘加载</param>
        public virtual void SetMesh(Guid templateId, bool forceLoad)
        {
            
        }
        /// <summary>
        /// 场景碰撞检测
        /// </summary>
        /// <param name="start">对象起点坐标</param>
        /// <param name="end">对象终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <returns>检测成功返回true，否则返回false</returns>
        public virtual bool OnCollideScene(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, CSUtility.Support.stHitResult result)
        {//如果返回true就是需要阻挡住不让穿越，否则不管他，飞过去
            return true;
        }
        /// <summary>
        /// 位置改变调用的方法
        /// </summary>
        /// <param name="placement">位置</param>
        public virtual void OnPlacementChanged(IPlacement placement)
        {

        }

        //#region TestCode
        //unsafe SlimDX.Vector3* vvv;

        //public void TestUnsafe(ref SlimDX.Vector3 v)
        //{
        //    unsafe
        //    {
        //        fixed (SlimDX.Vector3* pv = &v)
        //        {
        //            vvv = pv;
        //        }
        //    }
        //}

        //public void test()
        //{
        //    SlimDX.Vector3 v = new SlimDX.Vector3();
        //    TestUnsafe(ref v);

        //    unsafe
        //    {
        //        v3dxVector3 vv = new v3dxVector3();
        //        TestUnsafeA(&vv);
        //    }
        //}

        //public unsafe void TestUnsafeA(v3dxVector3* v)
        //{
        //    vvv = (SlimDX.Vector3*)v;
        //}
        //#endregion
    }
}
