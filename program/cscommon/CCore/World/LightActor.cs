using System;
using System.ComponentModel;

namespace CCore.World
{
    /// <summary>
    /// 光照Actor的初始化类
    /// </summary>
    public class LightActorInit : CCore.World.ActorInit
    {
        CCore.Light.ELightType m_LightType;
        /// <summary>
        /// 光源类型
        /// </summary>
        public CCore.Light.ELightType LightType
        {
            get { return m_LightType; }
            set
            {
                m_LightType = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public LightActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Light;
            ActorFlag = EActorFlag.SaveWithClient;
            mLayer = CCore.RLayer.RL_CustomDSLighting;
        }
    }
    /// <summary>
    /// 光源Actor类
    /// </summary>
    public class LightActor : CCore.World.Actor, INotifyPropertyChanged, CCore.Socket.ISocketComponent
    {
        #region SocketComponent
        /// <summary>
        /// 挂接组件信息
        /// </summary>
        public CCore.Socket.ISocketComponentInfo SocketComponentInfo
        {
            get;
            protected set;
        } = null;
        /// <summary>
        /// 挂接到的主mesh对象
        /// </summary>
        public CCore.Mesh.Mesh ComponentHostMesh
        {
            get;
            set;
        }
        /// <summary>
        /// 初始化挂接组件
        /// </summary>
        /// <param name="info">挂接组件信息</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool InitializeSocketComponent(CCore.Socket.ISocketComponentInfo info)
        {
            if (ComponentHostMesh == null)
                return false;

            var lightInfo = info as CCore.Light.Light;
            if (lightInfo == null)
                return false;

            SocketComponentInfo = info;

            var laInit = new CCore.World.LightActorInit();
            laInit.LightType = lightInfo.LightType;

            this.Initialize(laInit);
            this.Light.CopyFrom(lightInfo);

            return true;
        }
        /// <summary>
        /// 计算最终矩阵
        /// </summary>
        /// <param name="socketMatrix">挂接件的位置矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <returns>返回计算后的矩阵</returns>
        private SlimDX.Matrix CalculateFinalMatrix(ref SlimDX.Matrix socketMatrix, ref SlimDX.Matrix parentMatrix)
        {
            SlimDX.Vector3 socketPos, socketScale;
            SlimDX.Quaternion socketQuat;
            socketMatrix.Decompose(out socketScale, out socketQuat, out socketPos);
            SlimDX.Vector3 parentPos, parentSale;
            SlimDX.Quaternion parentQuat;
            parentMatrix.Decompose(out parentSale, out parentQuat, out parentPos);
            var info = SocketComponentInfo as CCore.Light.Light;
            var itemScale = new SlimDX.Vector3(info.SocketScale.X * info.Scale.X,
                                               info.SocketScale.Y * info.Scale.Y,
                                               info.SocketScale.Z * info.Scale.Z);
            var itemQuat = SlimDX.Quaternion.RotationYawPitchRoll((float)(info.SocketRotate.Y / 180.0 * System.Math.PI),
                                                                  (float)(info.SocketRotate.X / 180.0 * System.Math.PI),
                                                                  (float)(info.SocketRotate.Z / 180.0 * System.Math.PI));
            var itemMat = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity,
                                                       itemScale, -info.SocketPosOffset, itemQuat, info.SocketPosOffset);
            var socketFinalMatrix = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity,
                                                                 socketScale, SlimDX.Vector3.Zero, info.InheritSocketRotate ? socketQuat : SlimDX.Quaternion.Identity,
                                                                 socketPos);
            var parentFinalMatrix = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity,
                                                                 info.InheritScale ? parentSale : SlimDX.Vector3.UnitXYZ,
                                                                 SlimDX.Vector3.Zero, info.InheritRotate ? parentQuat : SlimDX.Quaternion.Identity,
                                                                 parentPos);
            return itemMat * socketFinalMatrix * parentFinalMatrix;
        }
        /// <summary>
        /// 提交挂接组件
        /// </summary>
        /// <param name="enviroment">渲染环境</param>
        /// <param name="socketMatrix">挂接组件矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="eye">视野</param>
        public virtual void SocketComponentCommit(CCore.Graphics.REnviroment enviroment, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, CCore.Camera.CameraObject eye)
        {
            var finalMat = CalculateFinalMatrix(ref socketMatrix, ref parentMatrix);
            this.Light.Commit(enviroment, ref finalMat, eye);
        }
        /// <summary>
        /// 提交挂接组件的阴影
        /// </summary>
        /// <param name="light">产生阴影的光源</param>
        /// <param name="socketMatrix">挂接件的矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="isDynamic">是否为动态的</param>
        public virtual void SocketComponentCommitShadow(CCore.Light.Light light, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, bool isDynamic)
        {
            var finalMat = CalculateFinalMatrix(ref socketMatrix, ref parentMatrix);
            this.Light.CommitShadow(light, ref finalMat, isDynamic);
        }
        /// <summary>
        /// 挂接件的每帧刷新
        /// </summary>
        /// <param name="host">父Actor对象</param>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public void SocketComponentTick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            this.Tick(elapsedMillisecond);
        }
        /// <summary>
        /// 淡入
        /// </summary>
        /// <param name="fadeType">淡入淡出方式</param>
        public void StartSocketComponentFadeIn(CCore.Mesh.MeshFadeType fadeType)
        {
        }
        /// <summary>
        /// 淡出
        /// </summary>
        /// <param name="fadeType">淡入淡出方式</param>
        public void StartSocketComponentFadeOut(CCore.Mesh.MeshFadeType fadeType)
        {
        }
        /// <summary>
        /// 更新该对象的淡入淡出方式
        /// </summary>
        /// <param name="fadePercent">淡入淡出比例</param>
        public void UpdateSocketComponentFadeInOut(float fadePercent)
        {
        }

        #endregion

        /// <summary>
        /// 灯光对象所在的世界
        /// </summary>
        public override World World
        {
            get { return base.World; }
            set
            {
                base.World = value;
                if(value != null && Light != null)
                {
                    switch(Light.ShadowType)
                    {
                        case CCore.Light.EShadowType.None:
                            value.ShadowLights.Remove(Id);
                            break;
                        default:
                            if(!value.ShadowLights.ContainsKey(Id))
                            {
                                value.ShadowLights[Id] = Light;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LightActor()
        {
            mPlacement = new CSUtility.Component.StandardPlacement(this);
            m_fEditorMeshSizeInScreen = 0.04f;
        }
        /// <summary>
        /// 获取原始的AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点坐标</param>
        /// <param name="vMax">最大顶点坐标</param>
        public override void GetOrigionAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            //base.GetOrigionAABB(ref vMin, ref vMax);

            //var scale = this.Light.Scale;// this.Placement.GetScale();
            var scale = SlimDX.Vector3.UnitXYZ;

            vMin = -scale;
            vMax = scale;

            //#warning "小雨，这里应该返回灯光的AABB，否则CheckVisible可能不正确";
        }
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="_init">初始化该对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            if (base.Initialize(_init) == false)
                return false;

            var lightActorInit = _init as LightActorInit;

            switch (lightActorInit.LightType)
            {
                case CCore.Light.ELightType.Dir:
                    Visual = new CCore.Light.DirLight();
                    break;
                case CCore.Light.ELightType.Point:
                    Visual = new CCore.Light.PointLight();
                    break;
                case CCore.Light.ELightType.Spot:
                    Visual = new CCore.Light.SpotLight();
                    break;
            }
            var lightInit = new CCore.Light.LightInit();            
            lightInit.LightType = lightActorInit.LightType;
		    Visual.Initialize(lightInit, this);

            return true;
        }

        //public bool Initialize(CSUtility.Component.IActorInitBase _init, ILight light)
        //{
        //    if (base.Initialize(_init) == false)
        //        return false;

        //    var lightActorInit = _init as LightActorInit;

        //    Visual = light;
        //    Visual.HostActor = this;

        //    return true;
        //}
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(long elapsedMillisecond)
        {
            TickLightActorTimer.Begin();
            base.Tick(elapsedMillisecond);

            CalcEditorMeshSize(CCore.Client.MainWorldInstance.Camera);
		    ShowEditorObj();
            TickLightActorTimer.End();
        }
        static CSUtility.Performance.PerfCounter TickLightActorTimer = new CSUtility.Performance.PerfCounter("TileScene.TickLightActor");
        /// <summary>
        /// 光源的可视化对象
        /// </summary>
        public CCore.Light.Light Light
        {
            get {return (CCore.Light.Light)Visual;}
            set
            {
                Visual = value;
            }
        }
        /// <summary>
        /// 获取显示属性的对象
        /// </summary>
        /// <returns>返回当前的对象</returns>
        public override Object GetShowPropertyObj()
        {
            return Light;
        }
        /// <summary>
        /// 保存场景数据
        /// </summary>
        /// <param name="attribute">XND文件</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public override bool SaveSceneData(CSUtility.Support.XndAttrib attribute)
        {
            base.SaveSceneData(attribute);

            Visual.Write(attribute);

            return true;
        }
        /// <summary>
        /// 加载场景数据
        /// </summary>
        /// <param name="attribute">XND文件</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
            base.LoadSceneData(attribute);

            Visual.Read(attribute);
    
            return true;
        }
        /// <summary>
        /// 复制对象
        /// </summary>
        /// <returns>返回复制的对象</returns>
        public override Actor Duplicate()
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
            copyedVisual.CopyFrom(this.Visual);
            copyedActor.Visual = copyedVisual;

            return copyedActor;
        }

        #region Editor相关
        float m_fEditorMeshSizeInScreen;
        /// <summary>
        /// 编辑的mesh对象的大小
        /// </summary>
        public float EditorMeshSizeInScreen
        {
            get { return m_fEditorMeshSizeInScreen; }
            set
            {
                m_fEditorMeshSizeInScreen = value;
            }
        }
        /// <summary>
        /// 计算编辑的mesh对象大小
        /// </summary>
        /// <param name="eye">视野</param>
        void CalcEditorMeshSize(CCore.Camera.CameraObject eye)
        {
            if (eye != null)
            {
                if (Light != null)
                    Light.SignMeshSize = eye.GetScreenSizeInWorld(mPlacement.GetLocation(), m_fEditorMeshSizeInScreen);
            }
        }
        /// <summary>
        /// 该对象被选中
        /// </summary>
        public override void Editor_Selected()
        {
            base.Editor_Selected();

            if (Light != null)
            {
                //Light.mEdgeDetect = true;
                Light.ShowRangeMesh = true;
                Light.ShowSignMesh = true;
            }
        }
        /// <summary>
        /// 对象未被选中
        /// </summary>
        public override void Editor_UnSelected()
        {
            base.Editor_UnSelected();

            if (Light != null)
            {
                //Light.mEdgeDetect = false;
                Light.ShowRangeMesh = false;
                Light.ShowSignMesh = false;
            }
        }
        /// <summary>
        /// 显示该对象
        /// </summary>
        void ShowEditorObj()
        {
            if (Light == null)
                return;

            //if (CCore.Client.MainWorldInstance.IsActorGameTypeShow((UInt16)CSUtility.Component.EActorGameType.Light))
            if(CCore.Program.IsActorTypeShow(CCore.Engine.Instance.Client.MainWorld, CCore.Program.LightAssistTypeName))
            {
                Light.ShowSignMesh = true;
                Light.ShowRangeMesh = true;
            }
            else
            {
                Light.ShowSignMesh = false;
                Light.ShowRangeMesh = false;
            }
        }
        /// <summary>
        /// 获取对象的层名称
        /// </summary>
        /// <returns>返回对象的层名称为Light</returns>
        public override string GetLayerName()
        {
            return "Light";
        }
        #endregion

    }

}
