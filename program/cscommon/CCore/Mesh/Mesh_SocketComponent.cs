using System;
using System.ComponentModel;

namespace CCore.Mesh
{
    /// <summary>
    /// mesh挂接件成员数据信息类
    /// </summary>
    [CCore.Socket.SocketComponentInfoAttribute("模型模板")]
    public class Mesh_SocketComponentInfo : CCore.Socket.ISocketComponentInfo, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        Guid mSocketComponentInfoId = Guid.NewGuid();
        /// <summary>
        /// 挂接成员的ID
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DataValueAttribute("SocketComponentInfoId")]
        public Guid SocketComponentInfoId
        {
            get { return mSocketComponentInfoId; }
            set { mSocketComponentInfoId = value; }
        }

        string mSocketName = "";
        /// <summary>
        /// 挂接点的名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("SocketName")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSocketSetter")]
        [Category("基础属性")]
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
        /// 挂接点的说明
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Description")]
        [Category("基础属性")]
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
        /// 只读属性，挂接成员的类型为“模型模板”
        /// </summary>
        [Browsable(false)]
        public string SocketComponentType
        {
            get { return "模型模板"; }
        }
        /// <summary>
        /// 所属的mesh
        /// </summary>
        [Browsable(false)]
        public CCore.Mesh.Mesh HostMesh
        {
            get;
            set;
        }

        Guid mMeshTemplateId = Guid.Empty;
        /// <summary>
        /// 挂接成员的mesh模型模板ID
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MeshTemplateId")]
        [Category("基础属性")]
        [DisplayName("模型模板")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSet")]
        public Guid MeshTemplateId
        {
            get { return mMeshTemplateId; }
            set
            {
                mMeshTemplateId = value;
                OnPropertyChanged("MeshTemplateId");
            }
        }

        bool mCanHitProxy = false;
        /// <summary>
        /// 是否可以点选
        /// </summary>
        [CSUtility.Support.DataValueAttribute("CanHitProxy")]
        [Category("基础属性")]
        [DisplayName("是否可点选")]
        public bool CanHitProxy
        {
            get { return mCanHitProxy; }
            set
            {
                mCanHitProxy = value;
                OnPropertyChanged("CanHitProxy");
            }
        }

        bool mInheritRotate = true;
        /// <summary>
        /// 是否跟随父对象旋转
        /// </summary>
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
        bool mInheritScale = true;
        /// <summary>
        /// 是否跟随父对象缩放
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
        bool mInheritSocketRotate = true;
        /// <summary>
        /// 是否跟随挂接点旋转
        /// </summary>
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
        SlimDX.Vector3 mPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 挂接成员的位置
        /// </summary>
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
        /// 挂接成员的缩放值
        /// </summary>
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
        /// 挂接成员的旋转值
        /// </summary>
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
        /// 复制另一个挂接成员的数据信息
        /// </summary>
        /// <param name="srcInfo">源数据</param>
        public void CopyComponentInfoFrom(CCore.Socket.ISocketComponentInfo srcInfo)
        {
            var info = srcInfo as Mesh_SocketComponentInfo;

            SocketName = info.SocketName;
            Description = info.Description;
            MeshTemplateId = info.MeshTemplateId;
            CanHitProxy = info.CanHitProxy;
            InheritRotate = info.InheritRotate;
            InheritSocketRotate = info.mInheritSocketRotate;
            Pos = info.Pos;
            Scale = info.Scale;
            Rotate = info.Rotate;
        }
        /// <summary>
        /// 获取挂接成员的类型
        /// </summary>
        /// <returns>返回挂接成员的类型</returns>
        public Type GetSocketComponentType()
        {
            return typeof(Mesh);
        }
    }
    public partial class Mesh : CCore.Socket.ISocketComponent, CCore.Socket.ISocketComponentHitProxy, CCore.Socket.ISocketComponentPublisherRes
    {
        #region 分析资源
        /// <summary>
        /// 只读属性，资源类型，mesh模板
        /// </summary>
        public virtual CSUtility.Support.enResourceType ResourceType
        {
            get { return CSUtility.Support.enResourceType.MeshTemplate; }            
        }
        /// <summary>
        /// 只读属性，mesh参数
        /// </summary>
        public virtual object[] Param
        {
            get
            {
                return new object[] { this };
            }
        }
        #endregion
        /// <summary>
        /// 挂接成员对象信息
        /// </summary>
        public CCore.Socket.ISocketComponentInfo SocketComponentInfo
        {
            get;
            protected set;
        } = null;
        /// <summary>
        /// 成员所属的Actor
        /// </summary>
        public CCore.Mesh.Mesh ComponentHostMesh
        {
            get;
            set;
        }
        /// <summary>
        /// 挂接成员的初始化
        /// </summary>
        /// <param name="info">挂接组件信息对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public virtual bool InitializeSocketComponent(CCore.Socket.ISocketComponentInfo info)
        {
            if (ComponentHostMesh == null)
                return false;

            var meshSocketCompInfo = info as Mesh_SocketComponentInfo;
            if (meshSocketCompInfo == null)
                return false;

            SocketComponentInfo = info;

            // 同模型不能嵌套挂接
            if (ComponentHostMesh.MeshTemplateID == meshSocketCompInfo.MeshTemplateId)
                return false;

            var meshInit = new CCore.Mesh.MeshInit();
            meshInit.MeshTemplateID = meshSocketCompInfo.MeshTemplateId;
            meshInit.CanHitProxy = meshSocketCompInfo.CanHitProxy;
            this.Initialize(meshInit, null);

            //if (mesh.IsTrail == true)
            //{
            //    mesh.SetTrailLife(info.TrailLifeTime);
            //    mesh.SetTrailEmitInterval(info.TrailEmitInterval);
            //    mesh.SetTrailMinDistance(info.TrailMinDistance);
            //    //mesh.SetTrailSegment();
            //}

            return true;
        }
        /// <summary>
        /// 计算最终的矩阵
        /// </summary>
        /// <param name="socketMatrix">挂接成员的矩阵</param>
        /// <param name="parentMatrix">父对象的矩阵</param>
        /// <returns>最终矩阵</returns>
        private SlimDX.Matrix CalculateFinalMatrix(ref SlimDX.Matrix socketMatrix, ref SlimDX.Matrix parentMatrix)
        {
            SlimDX.Vector3 socketPos, socketScale;
            SlimDX.Quaternion socketQuat;
            socketMatrix.Decompose(out socketScale, out socketQuat, out socketPos);
            SlimDX.Vector3 parentPos, parentSale;
            SlimDX.Quaternion parentQuat;
            parentMatrix.Decompose(out parentSale, out parentQuat, out parentPos);
            var meshInit = VisualInit as MeshInit;
            var info = SocketComponentInfo as Mesh_SocketComponentInfo;
            var itemScale = info.Scale;
            if (meshInit.MeshTemplate != null)
                itemScale = itemScale * meshInit.MeshTemplate.Scale;
            var itemQuat = SlimDX.Quaternion.RotationYawPitchRoll((float)(info.Rotate.Y / 180.0 * System.Math.PI),
                                                                  (float)(info.Rotate.X / 180.0 * System.Math.PI),
                                                                  (float)(info.Rotate.Z / 180.0 * System.Math.PI));
            var itemMat = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity,
                                                       itemScale, SlimDX.Vector3.Zero, itemQuat, info.Pos);
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
        /// <param name="socketMatrix">挂接组件的矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="eye">视野</param>
        public virtual void SocketComponentCommit(CCore.Graphics.REnviroment enviroment, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, CCore.Camera.CameraObject eye)
        {
            var meshInit = VisualInit as MeshInit;
            if (meshInit == null)
                return;

            var finalMat = CalculateFinalMatrix(ref socketMatrix, ref parentMatrix);
            this.Commit(enviroment, ref finalMat, eye);
        }
        /// <summary>
        /// 挂接组件提交阴影
        /// </summary>
        /// <param name="light">光源</param>
        /// <param name="socketMatrix">挂接组件的矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="isDynamic">是否为动态的(动态静态阴影都实时更新，只是动态阴影更新频率较高)</param>
        public virtual void SocketComponentCommitShadow(CCore.Light.Light light, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, bool isDynamic)
        {
            var meshInit = VisualInit as MeshInit;
            if (meshInit == null)
                return;
            if (!meshInit.CastShadow)
                return;

            var finalMat = CalculateFinalMatrix(ref socketMatrix, ref parentMatrix);
            this.CommitShadow(light, ref finalMat, isDynamic);
        }
        /// <summary>
        /// 挂接组件的每帧刷新
        /// </summary>
        /// <param name="host">父Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public void SocketComponentTick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            this.Tick(host, elapsedMillisecond);
        }
        /// <summary>
        /// 挂接组件开始渐显
        /// </summary>
        /// <param name="fadeType">渐隐渐显类型</param>
        public void StartSocketComponentFadeIn(CCore.Mesh.MeshFadeType fadeType)
        {
            this.StartFadeIn(fadeType);
        }
        /// <summary>
        /// 挂接组件开始渐隐
        /// </summary>
        /// <param name="fadeType">渐隐渐显类型</param>
        public void StartSocketComponentFadeOut(CCore.Mesh.MeshFadeType fadeType)
        {
            this.StartFadeOut(fadeType);
        }
        /// <summary>
        /// 更新挂接组件的渐隐渐显类型
        /// </summary>
        /// <param name="fadePercent">渐隐渐显的比例</param>
        public void UpdateSocketComponentFadeInOut(float fadePercent)
        {
            this.FadePercent = fadePercent;
        }
    }
}