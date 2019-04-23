using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Component
{
    /// <summary>
    /// 粒子特效初始化类，用于创建粒子特效时进行初始化
    /// </summary>
    [CCore.Socket.SocketComponentInfoAttribute("粒子特效")]
    public class EffectVisualInit : VisualInit, CCore.Socket.ISocketComponentInfo, INotifyPropertyChanged
    {
        /// <summary>
        /// 属性转变的委托事件
        /// </summary>
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region SocketComponentInfo

        Guid mSocketComponentInfoId = Guid.NewGuid();
        /// <summary>
        /// SocketComponentInfoId的属性
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("SocketComponentInfoId")]
        public Guid SocketComponentInfoId
        {
            get { return mSocketComponentInfoId; }
            set { mSocketComponentInfoId = value; }
        }

        string mSocketName = "";
        /// <summary>
        /// 挂接点的名字
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("SocketName")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSocketSetter")]
        [DisplayName("挂接点")]
        public string SocketName
        {
            get { return mSocketName; }
            set
            {
                mSocketName = value;
                OnPropertyChanged("SocketName");
            }
        }
        string mDescription = "";
        /// <summary>
        /// 说明项
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Description")]
        [DisplayName("说明")]
        public string Description
        {
            get { return mDescription; }
            set
            {
                mDescription = value;
                OnPropertyChanged("Description");
            }
        }
        /// <summary>
        /// 只读属性，说明其为粒子特效类型
        /// </summary>
        [Browsable(false)]
        public string SocketComponentType
        {
            get { return "粒子特效"; }
        }

        #endregion
        /// <summary>
        /// 粒子模板
        /// </summary>
        protected CCore.Effect.EffectTemplate mEffectTemplate;
        /// <summary>
        /// 只读属性，获得粒子模板
        /// </summary>
        [Browsable(false)]
        public CCore.Effect.EffectTemplate EffectTemplate
        {
            get { return mEffectTemplate; }
        }

        private System.Guid mEffectTemplateID;
        /// <summary>
        /// 粒子模板ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("EffectTemplateID")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("EffectSet")]
        [DisplayName("特效")]
        public System.Guid EffectTemplateID
        {
            get { return mEffectTemplateID; }
            set
            {
                mEffectTemplateID = value;
                var eftTemp = CCore.Effect.EffectManager.Instance.FindEffectTemplate(mEffectTemplateID);
                if (eftTemp == null)
                    return;
                mEffectTemplate = new CCore.Effect.EffectTemplate();
                mEffectTemplate.CopyFrom(eftTemp);
                mEffectTemplate.Id = Guid.NewGuid();
                mEffectTemplate.ParentEffect = eftTemp;
                mEffectTemplate.Ver = eftTemp.Ver;
                mEffectTemplate.Reset();
                OnPropertyChanged("EffectTemplateID");
            }
        }

        private bool mCanHitProxy = true;
        /// <summary>
        /// 是否可以进行鼠标点选
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("CanHitProxy")]
        [DisplayName("鼠标点选")]
        public bool CanHitProxy
        {
            get { return mCanHitProxy; }
            set
            {
                mCanHitProxy = value;
                OnPropertyChanged("CanHitProxy");
            }
        }
        /// <summary>
        /// 持续时间，默认为最大值
        /// </summary>
        public long mDuration = long.MaxValue;
        /// <summary>
        /// 持续时间
        /// </summary>
        public long Duration
        {
            get { return mDuration; }
            set
            {
                mDuration = value;
                OnPropertyChanged("Duration");
            }
        }
        bool mInheritScale = true;
        /// <summary>
        /// 是否跟随父对象进行缩放
        /// </summary>
        [CSUtility.Support.DataValueAttribute("InheritScale")]
        [Category("位置变换")]
        [DisplayName("跟随父对象缩放")]
        public bool InheritScale
        {
            get { return mInheritScale; }
            set
            {
                mInheritScale = value;
                OnPropertyChanged("InheritScale");
            }
        }
        bool mInheritRotate = true;
        /// <summary>
        /// 是否跟随父对象旋转
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("InheritRotate")]
        [Category("位置变换")]
        [DisplayName("跟随父对象旋转")]
        public bool InheritRotate
        {
            get { return mInheritRotate; }
            set
            {
                mInheritRotate = value;
                OnPropertyChanged("InheritRotate");
            }
        }

        bool mInheritSocketRotate = true;
        /// <summary>
        /// 是否跟随挂接点进行旋转
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("InheritSocketRotate")]
        [DisplayName("跟随挂接点旋转")]
        [Category("位置变换")]
        public bool InheritSocketRotate
        {
            get { return mInheritSocketRotate; }
            set
            {
                mInheritSocketRotate = value;
                OnPropertyChanged("InheritSocketRotate");
            }
        }

        bool mInheritRotateWhenBorn = false;
        /// <summary>
        /// 是否继承出生时的变换
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("InheritRotateWhenBorn")]
        [DisplayName("继承出生时的变换")]
        [Category("位置变换")]
        public bool InheritRotateWhenBorn
        {
            get { return mInheritRotateWhenBorn; }
            set
            {
                mInheritRotateWhenBorn = value;
            }
        }

        SlimDX.Vector3 mPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 位置信息
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Pos")]
        [Category("位置变换")]
        [DisplayName("位置")]
        public SlimDX.Vector3 Pos
        {
            get { return mPos; }
            set
            {
                mPos = value;
                OnPropertyChanged("Pos");
            }
        }

        SlimDX.Vector3 mScale = SlimDX.Vector3.UnitXYZ;
        /// <summary>
        /// 缩放值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Scale")]
        [Category("位置变换")]
        [DisplayName("缩放")]
        public SlimDX.Vector3 Scale
        {
            get { return mScale; }
            set
            {
                mScale = value;
                OnPropertyChanged("Scale");
            }
        }

        SlimDX.Vector3 mRotate = SlimDX.Vector3.Zero;
        /// <summary>
        /// 旋转值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Rotate")]
        [Category("位置变换")]
        [DisplayName("旋转")]
        public SlimDX.Vector3 Rotate
        {
            get { return mRotate; }
            set
            {
                mRotate = value;
                OnPropertyChanged("Rotate");
            }
        }
        /// <summary>
        /// 复制另一个ISocketComponentInfo对象数据
        /// </summary>
        /// <param name="srcInfo">将要复制的源数据</param>
        public void CopyComponentInfoFrom(CCore.Socket.ISocketComponentInfo srcInfo)
        {
            var srcInit = srcInfo as EffectVisualInit;
            this.CopyFrom(srcInit);
        }
        /// <summary>
        /// 得到其类型为EffectVisual
        /// </summary>
        /// <returns>返回EffectVisual类型</returns>
        public Type GetSocketComponentType()
        {
            return typeof(EffectVisual);
        }
    }
    /// <summary>
    /// 特效的可视化类
    /// </summary>
    public class EffectVisual : Visual, CCore.Socket.ISocketComponent, CCore.Socket.ISocketComponentPublisherRes
    {
        #region 分析资源
        public CSUtility.Support.enResourceType ResourceType
        {
            get { return CSUtility.Support.enResourceType.Effect; }
        }
        public object[] Param
        {
            get
            {
                return new object[] { EffectTemplateID };
            }
        }
        #endregion

        #region SocketComponent
        /// <summary>
        /// SocketComponentInfo，默认为null
        /// </summary>
        public CCore.Socket.ISocketComponentInfo SocketComponentInfo
        {
            get;
            protected set;
        } = null;
        /// <summary>
        /// 父节点的mesh
        /// </summary>
        public CCore.Mesh.Mesh ComponentHostMesh
        {
            get;
            set;
        }
        /// <summary>
        /// 初始化ISocketComponentInfo类型
        /// </summary>
        /// <param name="info">需要进行初始化的ISocketComponentInfo</param>
        /// <returns>初始化成功返回true，失败返回false</returns>
        public virtual bool InitializeSocketComponent(CCore.Socket.ISocketComponentInfo info)
        {
            if (ComponentHostMesh == null)
                return false;

            var evi = info as EffectVisualInit;
            if (evi == null)
                return false;

            SocketComponentInfo = info;
            
            this.Initialize(evi, null);

            return true;
        }
        /// <summary>
        /// 计算挂接在EffectVisual的ISocketComponentInfo的placement矩阵
        /// </summary>
        /// <param name="socketMatrix">连接点的placement矩阵</param>
        /// <param name="parentMatrix">父节点的placement矩阵</param>
        /// <returns>返回计算完毕的ISocketComponentInfo的矩阵</returns>
        private SlimDX.Matrix CalculateFinalMatrix(ref SlimDX.Matrix socketMatrix, ref SlimDX.Matrix parentMatrix)
        {
            SlimDX.Vector3 socketPos, socketScale;
            SlimDX.Quaternion socketQuat;
            socketMatrix.Decompose(out socketScale, out socketQuat, out socketPos);
            SlimDX.Vector3 parentPos, parentSale;
            SlimDX.Quaternion parentQuat;
            parentMatrix.Decompose(out parentSale, out parentQuat, out parentPos);
            var info = SocketComponentInfo as EffectVisualInit;
            var itemScale = info.Scale;
            var itemQuat = SlimDX.Quaternion.RotationYawPitchRoll((float)(info.Rotate.Y / 180.0 * System.Math.PI),
                                                                  (float)(info.Rotate.X / 180.0 * System.Math.PI),
                                                                  (float)(info.Rotate.Z / 180.0 * System.Math.PI));
            var itemMat = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity,
                                                       itemScale, -info.Pos, itemQuat, info.Pos);
            var socketFinalMatrix = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity,
                                                                 socketScale, SlimDX.Vector3.Zero, info.InheritSocketRotate ? socketQuat : SlimDX.Quaternion.Identity,
                                                                 socketPos);
            var parentFinalMatrix = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity,
                                                                 info.InheritScale ? parentSale : SlimDX.Vector3.UnitXYZ,
                                                                 SlimDX.Vector3.Zero, info.InheritRotate ? parentQuat : SlimDX.Quaternion.Identity,
                                                                 parentPos);
            var finalMat = itemMat * socketFinalMatrix * parentFinalMatrix;
            if(info.InheritRotateWhenBorn)
            {
                if(this.mBorn == false)
                {
                    this.mBornMatrix = finalMat;
                    this.mBorn = true;
                }
                finalMat = this.mBornMatrix;
            }
            return finalMat;
        }
        /// <summary>
        /// 将挂接的该节点提交到世界环境中
        /// </summary>
        /// <param name="enviroment">提交到的环境</param>
        /// <param name="socketMatrix">节点矩阵</param>
        /// <param name="parentMatrix">父节点矩阵</param>
        /// <param name="eye">视野</param>
        public virtual void SocketComponentCommit(CCore.Graphics.REnviroment enviroment, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, CCore.Camera.CameraObject eye)
        {
            var finalMat = CalculateFinalMatrix(ref socketMatrix, ref parentMatrix);
            this.Commit(enviroment, ref finalMat, eye);
        }
        /// <summary>
        /// 提交节点阴影到世界中
        /// </summary>
        /// <param name="light">节点所处的环境光</param>
        /// <param name="socketMatrix">节点矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="isDynamic">是否为动态的</param>
        public virtual void SocketComponentCommitShadow(CCore.Light.Light light, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, bool isDynamic)
        {
            var finalMat = CalculateFinalMatrix(ref socketMatrix, ref parentMatrix);
            this.CommitShadow(light, ref finalMat, isDynamic);
        }
        /// <summary>
        /// 每帧调用，确保每个EffectVisual的类型都能刷新
        /// </summary>
        /// <param name="host">此EffectVisual的主Actor</param>
        /// <param name="elapsedMillisecond">调用的间隔时间</param>
        public void SocketComponentTick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            this.Tick(host, elapsedMillisecond);
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
        /// 析构函数
        /// </summary>
        ~EffectVisual()
        {
            
        }
        /// <summary>
        /// 图形数据类，保存mesh和粒子发射器
        /// </summary>
        public class MeshData
        {
            /// <summary>
            /// mesh对象
            /// </summary>
            public CCore.Mesh.Mesh Mesh;
            /// <summary>
            /// 粒子模拟器
            /// </summary>
            public CCore.Modifier.ParticleModifier Modifier;
        }
        /// <summary>
        /// 只读属性，粒子模板ID
        /// </summary>
        public System.Guid EffectTemplateID
        {
            get
            {
                Guid id = Guid.Empty;
                var effectInit = VisualInit as EffectVisualInit;
                if (effectInit != null)
                    id = effectInit.EffectTemplateID;

                return id;
            }
        }
        /// <summary>
        /// 只读属性，粒子模板
        /// </summary>
        public CCore.Effect.EffectTemplate EffectTemplate
        {
            get
            {
                var effectInit = VisualInit as EffectVisualInit;
                if (effectInit != null)
                    return effectInit.EffectTemplate;

                return null;
            }
        }
        /// <summary>
        /// 只读属性，是否允许鼠标点击
        /// </summary>
        public bool CanHitProxy
        {
            get
            {
                var effectInit = VisualInit as EffectVisualInit;
                if (effectInit != null)
                    return effectInit.CanHitProxy;

                return false;
            }
        }

        SlimDX.Vector3 mAABBMax = -SlimDX.Vector3.UnitXYZ;
        SlimDX.Vector3 mAABBMin = SlimDX.Vector3.UnitXYZ;
        List<MeshData> mMeshDataList = new List<MeshData>();
        /// <summary>
        /// 只读属性，保存meshdata的列表
        /// </summary>
        public List<MeshData> MeshDataList
        {
            get { return mMeshDataList; }
        }
        EffectVisualInit mEffectInit;
        /// <summary>
        /// 只读属性，初始化数据
        /// </summary>
        public EffectVisualInit EffectInit
        {
            get { return mEffectInit; }
        }
        /// <summary>
        /// 生存时间，默认为0
        /// </summary>
        public long mLiveTime = 0;
        /// <summary>
        /// 生存时间
        /// </summary>
        public long LiveTime
        {
            get { return mLiveTime; }
            set
            {
                mLiveTime = value;
            }
        }
        /// <summary>
        /// 粒子产生时的矩阵，默认为单位矩阵
        /// </summary>
        public SlimDX.Matrix mBornMatrix = SlimDX.Matrix.Identity;
        /// <summary>
        /// 是否产生粒子，默认为false
        /// </summary>
        public bool mBorn = false;
        /// <summary>
        /// 粒子系统初始化
        /// </summary>
        /// <param name="_init">使用CCore.Component.VisualInit作为EffectInit进行初始化</param>
        /// <param name="host">在世界中的主Actor</param>
        /// <returns>初始化成功返回true，失败返回fals</returns>
        public override bool Initialize(CCore.Component.VisualInit _init, CCore.World.Actor host)
        {
            base.Initialize(_init, host);

            mMeshDataList.Clear();

            mLiveTime = 0;
            mEffectInit = _init as EffectVisualInit;
            if (mEffectInit.EffectTemplate != null)
            {
                foreach (var modifier in mEffectInit.EffectTemplate.Modifiers)
                {
                    //modifier.SetCamera(MidLayer.IEngine.Instance.Client.Graphics.GetCameraIntPtr());
                    MeshData meshData = new MeshData();

                    var meshInit = new CCore.Mesh.MeshInit()
                    {
                        MeshTemplateID = modifier.MeshTemplateId,
                    };
                    var mesh = new CCore.Mesh.Mesh();
                    mesh.Initialize(meshInit, null);
                    mesh.SetParticleModifier(modifier);

                    meshData.Mesh = mesh;
                    meshData.Modifier = modifier;

                    mMeshDataList.Add(meshData);

                    SlimDX.Vector3 meshAABBMin = -SlimDX.Vector3.UnitXYZ;
                    SlimDX.Vector3 meshAABBMax = SlimDX.Vector3.UnitXYZ;
                    mesh.GetAABB(ref meshAABBMin, ref meshAABBMax);
                    if (mAABBMax.X < meshAABBMax.X)
                        mAABBMax.X = meshAABBMax.X;
                    if (mAABBMax.Y < meshAABBMax.Y)
                        mAABBMax.Y = meshAABBMax.Y;
                    if (mAABBMax.Z < meshAABBMax.Z)
                        mAABBMax.Z = meshAABBMax.Z;

                    if (mAABBMin.X > meshAABBMin.X)
                        mAABBMin.X = meshAABBMin.X;
                    if (mAABBMin.Y > meshAABBMin.Y)
                        mAABBMin.Y = meshAABBMin.Y;
                    if (mAABBMin.Z > meshAABBMin.Z)
                        mAABBMin.Z = meshAABBMin.Z;

                    foreach (var emitter in modifier.Emitters)
                    {
                        if(emitter.Shape == null)
                            continue;

                        var vMax = emitter.Shape.AABBMax;
                        //////vMax.X += System.Math.Max(emitter.PositionX.ConstantRangeBegin, emitter.PositionX.ConstantRangeEnd);
                        //////vMax.Y += System.Math.Max(emitter.PositionY.ConstantRangeBegin, emitter.PositionY.ConstantRangeEnd);
                        //////vMax.Z += System.Math.Max(emitter.PositionZ.ConstantRangeBegin, emitter.PositionZ.ConstantRangeEnd);
                        var vMin = emitter.Shape.AABBMin;
                        //////vMin.X += System.Math.Min(emitter.PositionX.ConstantRangeBegin, emitter.PositionX.ConstantRangeEnd);
                        //////vMin.Y += System.Math.Min(emitter.PositionY.ConstantRangeBegin, emitter.PositionY.ConstantRangeEnd);
                        //////vMin.Z += System.Math.Min(emitter.PositionZ.ConstantRangeBegin, emitter.PositionZ.ConstantRangeEnd);

                        if (mAABBMax.X < vMax.X)
                            mAABBMax.X = vMax.X;
                        if (mAABBMax.Y < vMax.Y)
                            mAABBMax.Y = vMax.Y;
                        if (mAABBMax.Z < vMax.Z)
                            mAABBMax.Z = vMax.Z;

                        if (mAABBMin.X > vMin.X)
                            mAABBMin.X = vMin.X;
                        if (mAABBMin.Y > vMin.Y)
                            mAABBMin.Y = vMin.Y;
                        if (mAABBMin.Z > vMin.Z)
                            mAABBMin.Z = vMin.Z;
                    }
                }
            }
            
            return true;
        }

        bool bCommitedPreFrame = true;
        /// <summary>
        /// 提交粒子到世界中
        /// </summary>
        /// <param name="renderEnv">提交到的环境</param>
        /// <param name="matrix">粒子的placement矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if (!Visible)
                return;

            bCommitedPreFrame = true;

            var particleDensity = CCore.Performance.EParticleDensity.Normal;
            if (CCore.Performance.PerformanceOptionsTemplate.CurrentPerformOptions != null)
                particleDensity = CCore.Performance.PerformanceOptionsTemplate.CurrentPerformOptions.ParticleDensity;

            if(mEffectInit != null && mEffectInit.EffectTemplate != null)
            {
                foreach (var modifier in mEffectInit.EffectTemplate.Modifiers)
                {
                    modifier.SetWorldTransMatrix(matrix);
                    modifier.SetCamera(eye);
                }

                foreach (var mesh in mMeshDataList)
                {
                    if (mesh.Modifier.ParticleDensity > particleDensity)
                        continue;

                    //bool enable = false;
                    //foreach (var emitter in mesh.Modifier.Emitters)
                    //{
                    //    if(emitter.Enabled)
                    //    {
                    //        enable = true;
                    //        break;
                    //    }
                    //}
                    //if(!enable)
                    //    continue;

                    switch (mesh.Modifier.Space)
                    {
                        case CCore.Particle.CoordinateSpaceCN.World:
                        case CCore.Particle.CoordinateSpaceCN.Local:
                            {
                                mesh.Mesh.Commit(renderEnv, ref SlimDX.Matrix.mIdentity, eye);
                            }
                            break;
                        case CCore.Particle.CoordinateSpaceCN.LocalWithDirection:
                            {
                                mesh.Mesh.Commit(renderEnv, ref matrix, eye);
                            }
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 设置粒子中的mesh是否可点击
        /// </summary>
        /// <param name="hitProxy">粒子mesh的点击代理</param>
        public override void SetHitProxyAll(uint hitProxy)
        {
            foreach (var mesh in mMeshDataList)
            {
                mesh.Mesh.SetHitProxyAll(hitProxy);
            }
        }
        /// <summary>
        /// 每帧调用，刷新粒子
        /// </summary>
        /// <param name="host">所属Actor</param>
        /// <param name="elapsedMillisecond">间隔时间</param>
        public override void Tick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            if (Visible == false)
                return;

            if (EffectInit == null)
                return;

            if (EffectInit.EffectTemplate != null && EffectInit.EffectTemplate.IsFinished && EffectInit.EffectTemplate.Loop == false)
                return;

            var particleDensity = CCore.Performance.EParticleDensity.Normal;
            if (CCore.Performance.PerformanceOptionsTemplate.CurrentPerformOptions != null)
                particleDensity = CCore.Performance.PerformanceOptionsTemplate.CurrentPerformOptions.ParticleDensity;

            if (bCommitedPreFrame == false)
                return;
            else
                bCommitedPreFrame = false;

            bool bEffectEnable = false;
            foreach (var mesh in mMeshDataList)
            {
                if (mesh.Modifier.ParticleDensity > particleDensity)
                    continue;

                //bool enable = false;
                //foreach (var emitter in mesh.Modifier.Emitters)
                //{
                //    if (emitter.Enabled)
                //    {
                //        enable = true;
                //        bEffectEnable = true;
                //        break;
                //    }
                //}
                //if (!enable)
                //    continue;

                CCore.Engine.Instance.Effect_ParticleMesh_Number++;
                CCore.Engine.Instance.Effect_ParticlePool_Number += mesh.Modifier.ParticlePoolSize;
                mesh.Mesh.Tick(host, elapsedMillisecond);
            }

            if (bEffectEnable && EffectInit.EffectTemplate != null)
                CCore.Engine.Instance.Effect_ParticleLive_Number += EffectInit.EffectTemplate.ParticlesCount;

            if(mEffectInit != null && mEffectInit.EffectTemplate != null && mEffectInit.EffectTemplate.ParentEffect != null)
            {
                if (mEffectInit.EffectTemplate.Ver != mEffectInit.EffectTemplate.ParentEffect.Ver)
                {
                    mEffectInit.EffectTemplateID = mEffectInit.EffectTemplateID;
                    this.Initialize(mEffectInit, mHostActor);
                }
            }

            if (EffectInit.EffectTemplate != null && EffectInit.EffectTemplate.IsFinished && EffectInit.EffectTemplate.Loop == true)
            {
                EffectInit.EffectTemplate.Reset();
            }

            mLiveTime += elapsedMillisecond;
        }
        /// <summary>
        /// 得到粒子初始化模板的LayerName
        /// </summary>
        /// <returns>返回LayerName，初始化类型为空或者模板为空返回Effect</returns>
        public override string GetLayerName()
        {
            if (mEffectInit != null && mEffectInit.EffectTemplate != null)
                return mEffectInit.EffectTemplate.LayerName;

            return "Effect";
        }
        /// <summary>
        /// 创建粒子的AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点</param>
        /// <param name="vMax">最大顶点</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            vMin = mAABBMin;
            vMax = mAABBMax;
        }
        /// <summary>
        /// 如果粒子没有设置延迟死亡则删除
        /// </summary>
        public void RemoveNotDelayDeath()
        {
            for(int i=0;i<mMeshDataList.Count;i++)
            {
                var mesh = mMeshDataList[i];
                if (mesh == null)
                    continue;
                var modi = mesh.Modifier;
                if(modi.DelayDead==false)
                {
                    mMeshDataList.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// 粒子结束标志
        /// </summary>
        /// <returns>粒子模板为空或者超出生存时间则返回true，否则返回false</returns>
        public bool IsFinished()
        {
            if (EffectInit.EffectTemplate == null)
                return true;
            if (EffectInit.EffectTemplate.IsFinished == true)
                return true;
            if (mLiveTime > EffectInit.Duration)
                return true;

            return false;
        }
    }
}