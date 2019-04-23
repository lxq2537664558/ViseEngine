using System;
using System.Collections.Generic;

namespace CCore.Support
{
    /// <summary>
    /// 3D坐标轴的初始化类
    /// </summary>
    public class V3DAxisInit : CCore.World.ActorInit
    {

    }
    /// <summary>
    /// 3D坐标轴
    /// </summary>
    public class V3DAxis : CCore.World.Actor
    {
        /// <summary>
        /// 坐标的类型
        /// </summary>
        public enum enAxisType
        {
            Null = -1,
            Move_X = 0,
            Move_Y,
            Move_Z,
            Move_XY,
            Move_XZ,
            Move_YZ,
            Move_XYZ,
            Rot_X,
            Rot_Y,
            Rot_Z,
            Scale_X,
            Scale_Y,
            Scale_Z,
            Scale_XY,
            Scale_XZ,
            Scale_YZ,
            Scale_XYZ,
        }
        /// <summary>
        /// 编辑模式枚举
        /// </summary>
        public enum enEditMode
		{
			None,
			Move,
			Rot,
			Scale,
		};
        /// <summary>
        /// 坐标模式枚举
        /// </summary>
		public enum enAxisMode
		{
			Local,
			World,
		};
        /// <summary>
        /// 操作方式枚举
        /// </summary>
        public enum enOperationType
        {
            MoveObject,
            MoveAxis,
        }
        /// <summary>
        /// 操作方式，默认为移动物体
        /// </summary>
        protected enOperationType mOperationType = enOperationType.MoveObject;
        /// <summary>
        /// 操作模式
        /// </summary>
        public enOperationType OperationType
        {
            get { return mOperationType; }
            set
            {
                mOperationType = value;
                OnPropertyChanged("OperationType");
            }
        }

        //protected System.Guid mId;
        /// <summary>
        /// 只读属性，对象ID
        /// </summary>
        public override System.Guid Id
        {
            get { return mId; }
        }
        /// <summary>
        /// 编辑模式
        /// </summary>
        protected enEditMode mEditMode;
        /// <summary>
        /// 编辑模式
        /// </summary>
        public enEditMode EditMode
        {
            get { return mEditMode; }
            set
            {
                mEditMode = value;

                switch (value)
                {
                    case enEditMode.Move:
                        Visual = mMoveAxis;
                        break;
                    case enEditMode.Rot:
                        Visual = mRotAxis;
                        break;
                    case enEditMode.Scale:
                        Visual = mScaleAxis;
                        break;
                    default:
                        Visual = new CCore.Component.Visual();
                        break;
                }
            }
        }
        /// <summary>
        /// 坐标轴模式
        /// </summary>
        protected enAxisMode mAxisMode;
        /// <summary>
        /// 坐标轴模式
        /// </summary>
        public enAxisMode AxisMode
        {
            get { return mAxisMode; }
            set
            {
                mAxisMode = value;

                switch (value)
                {
                    case enAxisMode.Local:
                        mDefaultPlacement.IsLocalAxis = true;
                        break;
                    case enAxisMode.World:
                        mDefaultPlacement.IsLocalAxis = false;
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 移动坐标轴的mesh对象
        /// </summary>
        protected CCore.Mesh.Mesh mMoveAxis;
        /// <summary>
        /// 旋转坐标轴的mesh对象
        /// </summary>
        protected CCore.Mesh.Mesh mRotAxis;
        /// <summary>
        /// 坐标进行缩放时显示的mesh
        /// </summary>
        protected CCore.Mesh.Mesh mScaleAxis;
        /// <summary>
        /// 红色材质
        /// </summary>
        protected CCore.Material.Material mMaterial_Red;
        /// <summary>
        /// 绿色材质
        /// </summary>
		protected CCore.Material.Material mMaterial_Green;
        /// <summary>
        /// 蓝色材质
        /// </summary>
		protected CCore.Material.Material mMaterial_Blue;
        /// <summary>
        /// 白色材质
        /// </summary>
		protected CCore.Material.Material mMaterial_White;
        /// <summary>
        /// 黄色材质
        /// </summary>
		protected CCore.Material.Material mMaterial_Yellow;
        /// <summary>
        /// 透明黄色材质
        /// </summary>
		protected CCore.Material.Material mMaterial_AlphaYellow;
        /// <summary>
        /// 透明色的材质
        /// </summary>
		protected CCore.Material.Material mMaterial_Alpha;
        /// <summary>
        /// XY轴选中时的材质
        /// </summary>
        protected CCore.Material.Material mMaterial_XY;
        /// <summary>
        /// XZ选中时的材质
        /// </summary>
        protected CCore.Material.Material mMaterial_XZ;
        /// <summary>
        /// YZ选中时的材质
        /// </summary>
        protected CCore.Material.Material mMaterial_YZ;
        /// <summary>
        /// 点击代理索引
        /// </summary>
        protected Dictionary<UInt32, enAxisType> mHitProxyIndexDic;
        /// <summary>
        /// 默认的位置
        /// </summary>
        protected CCore.Component.AxisPlacement mDefaultPlacement;
        /// <summary>
        /// 目标Actor列表
        /// </summary>
        protected CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> mTargetActors;
        /// <summary>
        /// 只读属性，目标Actor列表
        /// </summary>
        public CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> TargetActors
        {
            get { return mTargetActors; }
        }
        /// <summary>
        /// 是否存在目标Actor
        /// </summary>
        /// <returns>如果存在目标Actor返回true，否则返回false</returns>
        public bool HasTargets()
        {
            if (TargetActors != null && TargetActors.Count > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 移动轴的类型
        /// </summary>
        public CCore.Component.AxisPlacement.eAxisTransCenterType AxisTransCenterType
        {
            get
            {
                if (mDefaultPlacement != null)
                    return mDefaultPlacement.AxisTransCenterType;

                return CCore.Component.AxisPlacement.eAxisTransCenterType.ATCT_AxisPos;
            }
            set
            {
                if(mDefaultPlacement != null)
                    mDefaultPlacement.AxisTransCenterType = value;
            }
        }

        //public CCore.Component.AxisPlacement.eAxisType AxisClass
        //{
        //    get
        //    {
        //        if (mDefaultPlacement != null)
        //            return mDefaultPlacement.AxisType;

        //        return CCore.Component.AxisPlacement.eAxisType.AT_Local;
        //    }
        //    set
        //    {
        //        if(mDefaultPlacement != null)
        //            mDefaultPlacement.AxisType = value;
        //    }
        //}
        /// <summary>
        /// 设置ID
        /// </summary>
        /// <param name="id">ID</param>
        public override void _SetId(Guid id)
        {
            mId = id;
        }
        /// <summary>
        /// 移动轴的方向的ID枚举
        /// </summary>
        enum enMoveAxisMatId : int
        {
            XZ,
            Y,
            Z,
            X,
            YZ,
            XY,
        }
        /// <summary>
        /// 设置轴移动时通用材质
        /// </summary>
        protected void SetMoveAxisNormalMaterial()
        {
            //mMoveAxis.SetMaterial(0, 0, mMaterial_White);
            //mMoveAxis.SetMaterial(0, 1, mMaterial_Red);
            //mMoveAxis.SetMaterial(0, 2, mMaterial_Green);
            //mMoveAxis.SetMaterial(0, 3, mMaterial_Blue);
            //mMoveAxis.SetMaterial(0, 4, mMaterial_Red);
            //mMoveAxis.SetMaterial(0, 5, mMaterial_Green);
            //mMoveAxis.SetMaterial(0, 6, mMaterial_Blue);
            //mMoveAxis.SetMaterial(0, 7, mMaterial_Red);
            //mMoveAxis.SetMaterial(0, 8, mMaterial_Green);
            //mMoveAxis.SetMaterial(0, 9, mMaterial_Green);
            //mMoveAxis.SetMaterial(0, 10, mMaterial_Blue);
            //mMoveAxis.SetMaterial(0, 11, mMaterial_Blue);
            //mMoveAxis.SetMaterial(0, 12, mMaterial_Red);
            //mMoveAxis.SetMaterial(0, 13, mMaterial_Alpha);//mMaterial_AlphaYellow);
            //mMoveAxis.SetMaterial(0, 14, mMaterial_Alpha);
            //mMoveAxis.SetMaterial(0, 15, mMaterial_Alpha);

            mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.XZ), mMaterial_XZ);
            mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.Y), mMaterial_Green);
            mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.Z), mMaterial_Blue);
            mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.X), mMaterial_Red);
            mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.YZ), mMaterial_YZ);
            mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.XY), mMaterial_XY);
        }
        /// <summary>
        /// 设置轴缩放时通用材质
        /// </summary>
        protected void SetScaleAxisNormalMaterial()
        {
            mScaleAxis.SetMaterial(0, 0, mMaterial_White);
            mScaleAxis.SetMaterial(0, 1, mMaterial_Red);
            mScaleAxis.SetMaterial(0, 2, mMaterial_Green);
            mScaleAxis.SetMaterial(0, 3, mMaterial_Blue);
            mScaleAxis.SetMaterial(0, 4, mMaterial_Red);
            mScaleAxis.SetMaterial(0, 5, mMaterial_Green);
            mScaleAxis.SetMaterial(0, 6, mMaterial_Blue);
            mScaleAxis.SetMaterial(0, 7, mMaterial_Red);
            mScaleAxis.SetMaterial(0, 8, mMaterial_Green);
            mScaleAxis.SetMaterial(0, 9, mMaterial_Green);
            mScaleAxis.SetMaterial(0, 10, mMaterial_Blue);
            mScaleAxis.SetMaterial(0, 11, mMaterial_Blue);
            mScaleAxis.SetMaterial(0, 12, mMaterial_Red);
            mScaleAxis.SetMaterial(0, 13, mMaterial_Alpha);
            mScaleAxis.SetMaterial(0, 14, mMaterial_Alpha);
            mScaleAxis.SetMaterial(0, 15, mMaterial_Alpha);
        }
        /// <summary>
        /// 设置轴旋转时通用材质
        /// </summary>
        protected void SetRotAxisNormalMaterial()
        {
            mRotAxis.SetMaterial(0, 0, mMaterial_Red);
            mRotAxis.SetMaterial(0, 1, mMaterial_Green);
            mRotAxis.SetMaterial(0, 2, mMaterial_Blue);
        }
        /// <summary>
        /// 构造函数，创建对象
        /// </summary>
        public V3DAxis()
        {
            mId = GenId();
            ParticipationLineCheck = false;
            mTargetActors = new CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor>();
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="id">坐标轴对象的ID</param>
        public V3DAxis(System.Guid id)
        {
            mId = id;
            ParticipationLineCheck = false;
            mTargetActors = new CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor>();
        }
        /// <summary>
        /// 析构函数，删除对象
        /// </summary>
        ~V3DAxis()
        {
            Cleanup();
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">Actor的初始化类对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            mActorInit = _init;

            mHitProxyIndexDic = new Dictionary<uint, enAxisType>();
            mDefaultPlacement = new CCore.Component.AxisPlacement(this);
            mPlacement = mDefaultPlacement;

            mMaterial_Red =            CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialRed);
            mMaterial_Green =          CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialGreen);
		    mMaterial_Blue =           CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialBlue);
		    mMaterial_White =          CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialWhite);
		    mMaterial_Yellow =         CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialYellow);
		    mMaterial_AlphaYellow =    CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialAlphaYellow);
		    mMaterial_Alpha =          CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialAlpha);
            mMaterial_XY = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialXY);
            mMaterial_XZ = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialXZ);
            mMaterial_YZ = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.AxisMaterialYZ);

            // 用于移动的XYZ轴
            mMoveAxis = new CCore.Mesh.Mesh();
            var meshInit = new CCore.Mesh.MeshInit();
            var meshPart = new CCore.Mesh.MeshInitPart();
            meshPart.MeshName = CSUtility.Support.IFileConfig.MoveAxisMesh;
            meshInit.MeshInitParts.Add(meshPart);
            mMoveAxis.Initialize(meshInit, this);
            mMoveAxis.Layer = RLayer.RL_SystemHelper;
            SetMoveAxisNormalMaterial();
            mMoveAxis.CanHitProxy = true;

            // 用于旋转的XYZ轴
            mRotAxis = new CCore.Mesh.Mesh();
            meshInit = new CCore.Mesh.MeshInit();
            meshPart = new CCore.Mesh.MeshInitPart();
            meshPart.MeshName = CSUtility.Support.IFileConfig.RotAxisMesh;
            meshInit.MeshInitParts.Add(meshPart);
            mRotAxis.Initialize(meshInit, this);
            mRotAxis.Layer = RLayer.RL_SystemHelper;
            SetRotAxisNormalMaterial();
            mRotAxis.CanHitProxy = true;

            // 用于缩放的XYZ轴
            mScaleAxis = new CCore.Mesh.Mesh();
            meshInit = new CCore.Mesh.MeshInit();
            meshPart = new CCore.Mesh.MeshInitPart();
            meshPart.MeshName = CSUtility.Support.IFileConfig.ScaleAxisMesh;
            meshInit.MeshInitParts.Add(meshPart);
            mScaleAxis.Initialize(meshInit, this);
            mScaleAxis.Layer = RLayer.RL_SystemHelper;
            SetScaleAxisNormalMaterial();
            mScaleAxis.CanHitProxy = true;

		    EditMode = enEditMode.None;
		    AxisMode = enAxisMode.Local;

            return true;
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
            if (mTargetActors != null)
                mTargetActors.Clear();
        }
        /// <summary>
        /// 设置对象的点击代理
        /// </summary>
        /// <param name="axisType">坐标轴的类型</param>
        /// <param name="proxy">代理值</param>
        public void SetHitProxy(enAxisType axisType, UInt32 proxy)
        {
            mHitProxyIndexDic[proxy] = axisType;

		    switch (axisType)
		    {
		    case enAxisType.Move_X:
                {
                    //mMoveAxis.SetHitProxy(0, 1, proxy);
                    //mMoveAxis.SetHitProxy(0, 4, proxy);
                    mMoveAxis.SetHitProxy(0, (int)(enMoveAxisMatId.X), proxy);
                }
			    break;
		    case enAxisType.Move_Y:
                    {
                        //mMoveAxis.SetHitProxy(0, 2, proxy);
                        //mMoveAxis.SetHitProxy(0, 5, proxy);
                        mMoveAxis.SetHitProxy(0, (int)(enMoveAxisMatId.Y), proxy);
                    }
                    break;
		    case enAxisType.Move_Z:
                    {
                        //mMoveAxis.SetHitProxy(0, 3, proxy);
                        //mMoveAxis.SetHitProxy(0, 6, proxy);
                        mMoveAxis.SetHitProxy(0, (int)(enMoveAxisMatId.Z), proxy);
                    }
                    break;
		    case enAxisType.Move_XY:
                    {
                        //mMoveAxis.SetHitProxy(0, 7, proxy);
                        //mMoveAxis.SetHitProxy(0, 8, proxy);
                        mMoveAxis.SetHitProxy(0, (int)(enMoveAxisMatId.XY), proxy);
                    }
                    break;
		    case enAxisType.Move_XZ:
                    {
                        //mMoveAxis.SetHitProxy(0, 11, proxy);
                        //mMoveAxis.SetHitProxy(0, 12, proxy);
                        mMoveAxis.SetHitProxy(0, (int)(enMoveAxisMatId.XZ), proxy);
                    }
                    break;
		    case enAxisType.Move_YZ:
                    {
                        //mMoveAxis.SetHitProxy(0, 9, proxy);
                        //mMoveAxis.SetHitProxy(0, 10, proxy);
                        mMoveAxis.SetHitProxy(0, (int)(enMoveAxisMatId.YZ), proxy);
                    }
                    break;
		    case enAxisType.Move_XYZ:
			    //mMoveAxis.SetHitProxy(0, 0, proxy);
			    break;
		    case enAxisType.Rot_X:
			    mRotAxis.SetHitProxy(0, 0, proxy);
			    break;
		    case enAxisType.Rot_Y:
			    mRotAxis.SetHitProxy(0, 1, proxy);
			    break;
		    case enAxisType.Rot_Z:
			    mRotAxis.SetHitProxy(0, 2, proxy);
			    break;
		    case enAxisType.Scale_X:
			    mScaleAxis.SetHitProxy(0, 1, proxy);
			    mScaleAxis.SetHitProxy(0, 4, proxy);
			    break;
		    case enAxisType.Scale_Y:
			    mScaleAxis.SetHitProxy(0, 2, proxy);
			    mScaleAxis.SetHitProxy(0, 5, proxy);
			    break;
		    case enAxisType.Scale_Z:
			    mScaleAxis.SetHitProxy(0, 3, proxy);
			    mScaleAxis.SetHitProxy(0, 6, proxy);
			    break;
		    case enAxisType.Scale_XY:
			    mScaleAxis.SetHitProxy(0, 7, proxy);
			    mScaleAxis.SetHitProxy(0, 8, proxy);
			    break;
		    case enAxisType.Scale_XZ:
			    mScaleAxis.SetHitProxy(0, 11, proxy);
			    mScaleAxis.SetHitProxy(0, 12, proxy);
			    break;
		    case enAxisType.Scale_YZ:
			    mScaleAxis.SetHitProxy(0, 9, proxy);
			    mScaleAxis.SetHitProxy(0, 10, proxy);
			    break;
		    case enAxisType.Scale_XYZ:
			    mScaleAxis.SetHitProxy(0, 0, proxy);
			    break;
		    default:
			    break;
		    }
        }

        enAxisType mAxisType = enAxisType.Null;
        /// <summary>
        /// 只读属性，坐标轴的类型
        /// </summary>
        public enAxisType AxisType
        {
            get { return mAxisType; }
        }
        /// <summary>
        /// 鼠标点击的位置
        /// </summary>
        /// <param name="proxy">点击代理值</param>
        /// <returns>返回轴的类型</returns>
        public enAxisType OnMousePointAt(UInt32 proxy)
        {
            enAxisType axisType;
		    if(!mHitProxyIndexDic.TryGetValue(proxy, out axisType))
            {
                mAxisType = enAxisType.Null;
                return mAxisType;
            }

		    OnMousePointOut();

            mAxisType = axisType;
		    //m_HitIdx = (int)axisType;
		    //m_HitProxyIdx = proxy;

		    switch (mAxisType)
		    {
		    case enAxisType.Move_X:
                    //mMoveAxis.SetMaterial(0, 4, mMaterial_Yellow);
                    mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.X), mMaterial_Yellow);
			    break;
		    case enAxisType.Move_Y:
                    //mMoveAxis.SetMaterial(0, 5, mMaterial_Yellow);
                    mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.Y), mMaterial_Yellow);
			    break;
		    case enAxisType.Move_Z:
                    //mMoveAxis.SetMaterial(0, 6, mMaterial_Yellow);
                    mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.Z), mMaterial_Yellow);
			    break;
		    case enAxisType.Move_XY:
                    //mMoveAxis.SetMaterial(0, 7, mMaterial_Yellow);
                    //mMoveAxis.SetMaterial(0, 8, mMaterial_Yellow);
                    //mMoveAxis.SetMaterial(0, 13, mMaterial_AlphaYellow);
                    mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.XY), mMaterial_Yellow);
			    break;
		    case enAxisType.Move_XZ:
                    //mMoveAxis.SetMaterial(0, 11, mMaterial_Yellow);
                    //mMoveAxis.SetMaterial(0, 12, mMaterial_Yellow);
                    //mMoveAxis.SetMaterial(0, 14, mMaterial_AlphaYellow);
                    mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.XZ), mMaterial_Yellow);
			    break;
		    case enAxisType.Move_YZ:
                    //mMoveAxis.SetMaterial(0, 9, mMaterial_Yellow);
                    //mMoveAxis.SetMaterial(0, 10, mMaterial_Yellow);
                    //mMoveAxis.SetMaterial(0, 15, mMaterial_AlphaYellow);
                    mMoveAxis.SetMaterial(0, (int)(enMoveAxisMatId.YZ), mMaterial_Yellow);
			    break;
		    case enAxisType.Move_XYZ:
			    {
				    //for(int i=0; i<13; i++)
				    //{
					   // if(i==1 || i==2 || i==3)
						  //  continue;
					   // mMoveAxis.SetMaterial(0, i, mMaterial_Yellow);
				    //}
			    }
			    break;
		    case enAxisType.Rot_X:
			    mRotAxis.SetMaterial(0, 0, mMaterial_Yellow);
			    break;
		    case enAxisType.Rot_Y:
			    mRotAxis.SetMaterial(0, 1, mMaterial_Yellow);
			    break;
		    case enAxisType.Rot_Z:
			    mRotAxis.SetMaterial(0, 2, mMaterial_Yellow);
			    break;
		    case enAxisType.Scale_X:
			    mScaleAxis.SetMaterial(0, 4, mMaterial_Yellow);
			    break;
		    case enAxisType.Scale_Y:
			    mScaleAxis.SetMaterial(0, 5, mMaterial_Yellow);
			    break;
		    case enAxisType.Scale_Z:
			    mScaleAxis.SetMaterial(0, 6, mMaterial_Yellow);
			    break;
		    case enAxisType.Scale_XY:
			    mScaleAxis.SetMaterial(0, 7, mMaterial_Yellow);
			    mScaleAxis.SetMaterial(0, 8, mMaterial_Yellow);
			    mScaleAxis.SetMaterial(0, 13, mMaterial_AlphaYellow);
			    break;
		    case enAxisType.Scale_XZ:
			    mScaleAxis.SetMaterial(0, 11, mMaterial_Yellow);
			    mScaleAxis.SetMaterial(0, 12, mMaterial_Yellow);
			    mScaleAxis.SetMaterial(0, 14, mMaterial_AlphaYellow);
			    break;
		    case enAxisType.Scale_YZ:
			    mScaleAxis.SetMaterial(0, 9, mMaterial_Yellow);
			    mScaleAxis.SetMaterial(0, 10, mMaterial_Yellow);
			    mScaleAxis.SetMaterial(0, 15, mMaterial_AlphaYellow);
			    break;
		    case enAxisType.Scale_XYZ:
			    {
				    for(int i=0; i<13; i++)
				    {
					    if(i==1 || i==2 || i==3)
						    continue;
					    mScaleAxis.SetMaterial(0, i, mMaterial_Yellow);
				    }
			    }			
			    break;
		    default:
			    break;
		    }

		    return mAxisType;
        }
        /// <summary>
        /// 鼠标点击完成
        /// </summary>
        public void OnMousePointOut()
        {
            mAxisType = enAxisType.Null;
            switch (mEditMode)
		    {
		    case enEditMode.None:
			    break;
		    case enEditMode.Move:
			    SetMoveAxisNormalMaterial();
			    break;
		    case enEditMode.Rot:
			    SetRotAxisNormalMaterial();
			    break;
		    case enEditMode.Scale:
			    SetScaleAxisNormalMaterial();
			    break;
		    default:
			    break;
		    }
        }
        /// <summary>
        /// 设置目标Actor
        /// </summary>
        /// <param name="actors">目标Actor的列表</param>
        public void SetTargets(CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> actors)
        {
            if (actors != null && actors.Contains(this))
                actors.Remove(this);

            ResetOffset();
            mTargetActors = actors;
            if (actors != null && actors.Count > 0)
            {
                mDefaultPlacement.SetLinkedPlacements(mTargetActors);
                //mMoveAxis.Visible = true;
                //mRotAxis.Visible = true;
                //mScaleAxis.Visible = true;
            }
            else
            {
                //mMoveAxis.Visible = false;
                //mRotAxis.Visible = false;
                //mScaleAxis.Visible = false;
            }
        }
        /// <summary>
        /// 获取最后选择的Actor
        /// </summary>
        /// <returns>返回选中的Actor对象</returns>
        public CCore.World.Actor GetLastSelectedActor()
        {
            if (mTargetActors == null)
                return null;

            if (mTargetActors.Count == 0)
                return null;

            return mTargetActors[mTargetActors.Count - 1];
        }
        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="loc">位置坐标</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public bool SetLocation(ref SlimDX.Vector3 loc)
        {
            return Placement.SetLocation(ref loc);
        }
        /// <summary>
        /// 设置缩放值
        /// </summary>
        /// <param name="scale">缩放值</param>
        /// <returns>缩放成功返回true，否则返回false</returns>
        public bool SetScale(ref SlimDX.Vector3 scale)
        {
            return Placement.SetScale(ref scale);
        }
        /// <summary>
        /// 设置目标的缩放值
        /// </summary>
        /// <param name="scale">缩放值</param>
        /// <returns>缩放成功返回true，否则返回false</returns>
        public bool SetTagScale(ref SlimDX.Vector3 scale)
        {
		    switch (AxisTransCenterType)
		    {
		    case CCore.Component.AxisPlacement.eAxisTransCenterType.ATCT_AxisPos:
			    {
				    foreach (var act in mTargetActors)
				    {
					    act.Placement.SetScale(ref scale);
				    }
			    }
			    break;
		    case CCore.Component.AxisPlacement.eAxisTransCenterType.ATCT_ObjectPos:
			    break;
		    default:
			    break;
		    }

		    //if(TargetActor != nullptr)
		    //	return TargetActor->Placement->SetScale(scale);


		    return true;
        }
        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="quat">旋转四元数</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public bool SetRotation(ref SlimDX.Quaternion quat)
        {
            return Placement.SetRotation(ref quat);
        }
        /// <summary>
        /// 根据位置差值设置目标及轴的坐标
        /// </summary>
        /// <param name="loc">位置坐标的差值</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public bool SetLocationDeltaWithTargets(ref SlimDX.Vector3 loc)
        {
            ((CCore.Component.AxisPlacement)Placement).SetLocationDeltaWithTargets(ref loc);
            return true;
        }
        /// <summary>
        /// 根据差值设置目标及坐标的旋转值
        /// </summary>
        /// <param name="matDelta">旋转矩阵差值</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public bool SetRotationDeltaWithTargets(ref SlimDX.Matrix matDelta)
        {
            SlimDX.Vector3 vTempTrans, vTempScale;
		    SlimDX.Quaternion quat;
		    matDelta.Decompose(out vTempScale, out quat, out vTempTrans);

		    ((CCore.Component.AxisPlacement)(Placement)).SetRotationDeltaWithTargets(ref quat);
		    return true;
        }
        /// <summary>
        /// 根据缩放差值设置目标Actor及坐标轴的缩放
        /// </summary>
        /// <param name="scale">缩放的差值</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public bool SetScaleDeltaWithTargets(ref SlimDX.Vector3 scale)
        {
            ((CCore.Component.AxisPlacement)(Placement)).SetScaleDeltaWithTargets(ref scale);
		    return true;
        }
        /// <summary>
        /// 设置坐标轴的位置
        /// </summary>
        /// <param name="loc">位置坐标</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public bool SetAxisLocation(ref SlimDX.Vector3 loc)
        {
            ((CCore.Component.AxisPlacement)mPlacement).CalculateOffset(ref loc);
		    Placement.SetLocation(ref loc);

		    return true;
        }
        /// <summary>
        /// 获取目标Actor的缩放值
        /// </summary>
        /// <returns>返回目标Actor的缩放值</returns>
        public SlimDX.Vector3 GetTagScale()
        {
            return Placement.GetScale();
        }
        /// <summary>
        /// 获取顶点偏移值
        /// </summary>
        /// <returns>返回顶点的偏移值</returns>
        public SlimDX.Vector3 GetOffsetVertex()
        {
            return ((CCore.Component.AxisPlacement)mPlacement).OffsetVertex;
        }
        /// <summary>
        /// 重置偏移值
        /// </summary>
        public void ResetOffset() 
        {
            if(mPlacement == null)
			return;

		    //m_offsetMatrix = SlimDX::Matrix::Identity;
		    ((CCore.Component.AxisPlacement)mPlacement).OffsetVertex = SlimDX.Vector3.Zero;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(long elapsedMillisecond)
        {

        }
        /// <summary>
        /// 提交对象
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">对象矩阵</param>
        /// <param name="eye">视野</param>
        public override void OnCommit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {

        }
    }
}
