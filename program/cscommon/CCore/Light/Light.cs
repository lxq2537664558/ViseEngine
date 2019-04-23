using System;
using System.ComponentModel;
using CCore.Component;
using CCore.World;
using System.Collections.Generic;

namespace CCore.Light
{
    /// <summary>
    /// 光源类型的枚举
    /// </summary>
    public enum ELightType
	{
		Dir = 0,
		Point = 1,
		Spot = 2,
		Unknow
	};
    /// <summary>
    /// 光源产生阴影的类型
    /// </summary>
	public enum EShadowType
	{
		None = 0,
		Standard = 1,
		VSM = 2,
	};
    /// <summary>
    /// 光源的初始化类
    /// </summary>
    public class LightInit : CCore.Component.VisualInit
    {
        ELightType mLightType = ELightType.Dir;
        /// <summary>
        /// 光源类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public ELightType LightType
        {
            get { return mLightType; }
            set
            {
                mLightType = value;
            }
        }
    }
    /// <summary>
    /// 光源类
    /// </summary>
    public class Light : CCore.Component.Visual, CCore.Socket.ISocketComponentInfo
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义光源属性改变时调用的委托事件
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 光源属性改变时调用
        /// </summary>
        /// <param name="propertyName">改变的属性的名称</param>
        protected new void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            if (OnPropertyUpdate != null)
                OnPropertyUpdate(this, propertyName);
        }
        #endregion

        #region SocketComponentInfo

        Guid mSocketComponentInfoId = Guid.NewGuid();
        /// <summary>
        /// 挂接点的成员ID
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DoNotCopy]
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
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("SocketName")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSocketSetter")]
        [Category("挂接基础属性")]
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
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Description")]
        [Category("挂接基础属性")]
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

        bool mInheritRotate = true;
        /// <summary>
        /// 是否跟随父对象旋转
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("InheritRotate")]
        [Category("挂接位置变换")]
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
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("InheritScale")]
        [Category("挂接位置变换")]
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
        /// 是否进行挂接位置变换
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("InheritSocketRotate")]
        [DisplayName("跟随挂接点旋转")]
        [Category("挂接位置变换")]
        public bool InheritSocketRotate
        {
            get { return mInheritSocketRotate; }
            set
            {
                mInheritSocketRotate = value;
                OnPropertyChanged("InheritSocketRotate");
            }
        }

        SlimDX.Vector3 mSocketPosOffset = SlimDX.Vector3.Zero;
        /// <summary>
        /// 挂接件的位置
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("SocketPosOffset")]
        [Category("挂接位置变换")]
        [DisplayName("位置")]
        public SlimDX.Vector3 SocketPosOffset
        {
            get { return mSocketPosOffset; }
            set
            {
                mSocketPosOffset = value;
                OnPropertyChanged("SocketPosOffset");
            }
        }

        SlimDX.Vector3 mSocketScale = SlimDX.Vector3.UnitXYZ;
        /// <summary>
        /// 挂接件的缩放
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("SocketScale")]
        [Category("挂接位置变换")]
        [DisplayName("缩放")]
        public SlimDX.Vector3 SocketScale
        {
            get { return mSocketScale; }
            set
            {
                mSocketScale = value;
                OnPropertyChanged("SocketScale");
            }
        }

        SlimDX.Vector3 mSocketRotate = SlimDX.Vector3.Zero;
        /// <summary>
        /// 挂接件的旋转
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("SocketRotate")]
        [Category("挂接位置变换")]
        [DisplayName("旋转")]
        public SlimDX.Vector3 SocketRotate
        {
            get { return mSocketRotate; }
            set
            {
                mSocketRotate = value;
                OnPropertyChanged("SocketRotate");
            }
        }
        /// <summary>
        /// 挂接件的成员类型声明为灯光类型
        /// </summary>
        [Browsable(false)]
        public virtual string SocketComponentType
        {
            get { return "灯光"; }
        }
        /// <summary>
        /// 复制挂接件
        /// </summary>
        /// <param name="srcInfo">需要复制的挂接件</param>
        public virtual void CopyComponentInfoFrom(CCore.Socket.ISocketComponentInfo srcInfo)
        {
            var info = srcInfo as Light;

            CSUtility.Support.Copyable.CopyFrom(info, this);
        }
        /// <summary>
        /// 得到挂接件的类型
        /// </summary>
        /// <returns>返回挂接件类型为光源</returns>
        public virtual Type GetSocketComponentType()
        {
            return typeof(CCore.World.LightActor);
        }

        #endregion
        /// <summary>
        /// 声明属性更新时调用的委托事件
        /// </summary>
        /// <param name="l">改变属性的光源</param>
        /// <param name="proName">改变的属性的名称</param>
        public delegate void Delegate_OnPropertyUpdate(Light l, string proName);
        /// <summary>
        /// 定义属性更新时调用的委托事件
        /// </summary>
        public Delegate_OnPropertyUpdate OnPropertyUpdate;

        /// <summary>
        /// 光源对象的指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 只读属性，光源对象的指针
        /// </summary>
        [Browsable(false)]
        public IntPtr Inner
        {
            get { return mInner; }
        }
        /// <summary>
        /// 只读属性，光源类型，在这里返回光源类型为未知
        /// </summary>
        [Browsable(false)]
        public virtual ELightType LightType
        {
            get { return ELightType.Unknow; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Light()
        {
            mLayer = CCore.RLayer.RL_CustomDSLighting;
        }
        /// <summary>
        /// 析构函数，释放内存
        /// </summary>
        ~Light()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象，释放实例内存
        /// </summary>
        public override void Cleanup()
        {
            if (mInner != IntPtr.Zero)
            {
                DllImportAPI.vLightProxy_Release(mInner);
                mInner = IntPtr.Zero;

            }
        }
        Dictionary<World.Actor, World.Actor> mShadowActors = new Dictionary<World.Actor, World.Actor>();
        public bool IsCastedShadow(CCore.World.Actor actor)
        {
            if (mShadowActors.ContainsKey(actor))
                return true;
            mShadowActors.Add(actor, actor);
            return false;
        }
        public void ResetShadowActors()
        {
            mShadowActors.Clear();
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">可视化对象的初始化类</param>
        /// <param name="host">所属的Actor</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(VisualInit _init, Actor host)
        {
            Cleanup();
            base.Initialize(_init, host);

            var lightInit = _init as LightInit;
            CreateLightProxy(lightInit.LightType);

            return true;
        }
        /// <summary>
        /// 计算光源的光照范围
        /// </summary>
        /// <returns>返回计算好的光照范围</returns>
        public virtual float CalcLightRange()
        {
            unsafe
            {
                if (mInner != IntPtr.Zero)
                {
                    return DllImportAPI.vLightProxy_CalcLightRange(mInner);
                }
            }
            return 0;
        }
        /// <summary>
        /// 根据光源类型设置光源可进行鼠标点击
        /// </summary>
        /// <param name="t">光源的类型</param>
        public virtual void CreateLightProxy(ELightType t)
        {
            Cleanup();

		    switch (t)
		    {
		    case ELightType.Dir:
                mInner = DllImportAPI.vDirLightProxy_New(Engine.Instance.Client.Graphics.Device);
			    break;
		    case ELightType.Point:
                mInner = DllImportAPI.vPointLightProxy_New(Engine.Instance.Client.Graphics.Device);
			    break;
		    case ELightType.Spot:
                mInner = DllImportAPI.vSpotLightProxy_New(Engine.Instance.Client.Graphics.Device);
			    break;
            }

            var mtl1 = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultLightEditorSignMeshTechniqueId);
            var mtl2 = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultLightEditorRangeMeshTechniqueId);
            if(mtl1!=null)
                DllImportAPI.vLightProxy_EditorMeshSetMaterial(mInner, 0, mtl1.MaterialPtr);
            if (mtl2 != null)
                DllImportAPI.vLightProxy_EditorMeshSetMaterial(mInner, 1, mtl1.MaterialPtr);
        }
        /// <summary>
        /// 是否进行边缘检测，默认为false
        /// </summary>
        public bool mEdgeDetect = false;
        //public IMaterial mEdgeDetectMaterialInstance = null;
        /// <summary>
        /// 是否提交光源，默认为true
        /// </summary>
        public static bool IsCommitLight = true;
        /// <summary>
        /// 将光源提交到渲染环境
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">光源的位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if (mInner == IntPtr.Zero)
                return;

            if(this.LightType!= ELightType.Dir)
            {
                if (IsCommitLight == false)
                    return;
            }

            Int64 time = Engine.Instance.GetFrameSecondTime();
            unsafe
            {
                fixed (SlimDX.Matrix* pinMatrix = &matrix)
                {
                    if (Visible)
                    {
                        DllImportAPI.vDSRenderEnv_CommitDSLighting(renderEnv.DSRenderEnv, (int)mGroup, mInner, pinMatrix, eye.CommitCamera);
                    }

                    if (Engine.Instance.IsEditorMode == true)
                    {
                        DllImportAPI.vLightProxy_CommitEditorMesh(mInner,
                            ShowRangeMesh ? 1 : 0, ShowSignMesh ? 1 : 0, SignMeshSize,
                            renderEnv.DSRenderEnv, (int)mGroup, pinMatrix);

                        if (mEdgeDetect == true)
                        {
                            var editorMesh = DllImportAPI.vLightProxy_GetEditorMesh(mInner);
                            SlimDX.Vector4 edgeColor = new SlimDX.Vector4();
                            CSUtility.Program.DrawColor2Vector(mDiffuse, out edgeColor);
                            //var edgeColor = CSUtility.Data.RoleCommonTemplateManager.Instance.CommonRole.EdgeDetectLight;
                            DllImportAPI.vDSRenderEnv_CommitEdgeDetectMesh(renderEnv.DSRenderEnv, time, (int)mGroup, (int)mLayer, editorMesh, pinMatrix, mCustomTime, &edgeColor, 0, 1.0f);
                        }               
                    }
                }
            }
        }
        /// <summary>
        /// 复制光源
        /// </summary>
        /// <param name="source">需要复制的光源</param>
        public void CopyFrom(Light source)
        {
		    foreach (System.ComponentModel.PropertyDescriptor pro in System.ComponentModel.TypeDescriptor.GetProperties(source))
		    {
                var att = pro.Attributes[typeof(CSUtility.Support.DataValueAttribute)];
                if (att == null)
                    continue;

                pro.SetValue(this, pro.GetValue(source));
		    }
        }
        /// <summary>
        /// 添加光源的可视化对象
        /// </summary>
        /// <param name="layer">光源所在的层</param>
        /// <param name="vis">添加的可视化对象</param>
        /// <param name="matrix">光源的位置矩阵</param>
        /// <param name="dynamic">是否为动态的</param>
	    public void	AddVisual(CCore.RLayer layer, CCore.Component.Visual vis, ref SlimDX.Matrix matrix, bool dynamic)
	    {
            vis.CommitShadow(this, ref matrix, dynamic);
            //var mesh = vis as CCore.Mesh.Mesh;

            //if(mesh==null || mesh.Visible == false || mesh.MainMeshVisible == false)
            //    return;

            //unsafe
            //{
            //    fixed (SlimDX.Matrix* pinMatrix = &matrix)
            //    {
            //        switch (mesh.ShadingEnv)
            //        {
            //            case CCore.EShadingEnv.SDE_Deffered:
            //                {
            //                    for (int j = 0; j < mesh.MeshParts.Count; ++j)
            //                    {
            //                        IDllImportAPI.vLightProxy_CommitShadowMesh(mInner, IEngine.Instance.GetFrameSecondTime(), mesh.MeshParts[j].Mesh, pinMatrix, dynamic);
            //                    }
            //                }
            //                break;
            //        }
            //    }
            //    mesh.CommitLighting(this, ref matrix);
            //}
	    }
        /// <summary>
        /// 交换管道
        /// </summary>
        public void SwapPipes()
	    {
            DllImportAPI.vLightProxy_SwapPipes(mInner);
	    }
        /// <summary>
        /// 对阴影进行渲染
        /// </summary>
        /// <param name="camera">摄像机</param>
	    public void	RenderShadow(CCore.Camera.CameraObject camera)
	    {
            if (CCore.Engine.Instance.IsEditorMode == true)
            {
                ShadowMapSize = EditorShadowMapSize;
                ShadowCoverSize = EditorShadowCoverSize;
            }
            else
            {
                ShadowMapSize = GameShadowMapSize;
                ShadowCoverSize = GameShadowCoverSize;
            }

            DllImportAPI.vLightProxy_RenderShadow(mInner, Engine.Instance.Client.Graphics.Device, camera.CommitCamera);
	    }
        /// <summary>
        /// 设置对象可用鼠标点击
        /// </summary>
        /// <param name="hitProxy">点击的代理ID</param>
        public override void SetHitProxyAll(uint hitProxy)
        {
            DllImportAPI.vLightProxy_SetHitProxy(mInner, (int)hitProxy);
        }
        /// <summary>
        /// Alpha的值
        /// </summary>
        [Browsable(false)]
		[CSUtility.Support.DoNotCopy]
        public double IntensityAlpha
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetIntensityAlpha(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetIntensityAlpha(mInner, (float)value);
                }
            }
        }
        /// <summary>
        /// 调整灯光的光源强度
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Intensity")]
		[System.ComponentModel.Category("常用")]
		[System.ComponentModel.DisplayName("光源强度")]
		[CSUtility.AISystem.Attribute.AllowMember("灯光.光源强度", CSUtility.Helper.enCSType.Client, "调整灯光的光源强度")]
        [CSUtility.Event.Attribute.AllowMember("灯光.光源强度", CSUtility.Helper.enCSType.Client, "调整灯光的光源强度")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 100)]
        public double Intensity
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetIntensity(mInner); } }
            set 
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetIntensity(mInner, (float)value);
                    CalcLightRange();
                }
                OnPropertyChanged("Intensity");
            }
        }
        /// <summary>
        /// 调整灯光的高光强度
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("SpecularIntensity")]
		[System.ComponentModel.Category("光源属性")]
		[System.ComponentModel.DisplayName("高光强度")]
        [CSUtility.AISystem.Attribute.AllowMember("灯光.高光强度", CSUtility.Helper.enCSType.Client, "调整灯光的高光强度")]
        [CSUtility.Event.Attribute.AllowMember("灯光.高光强度", CSUtility.Helper.enCSType.Client, "调整灯光的高光强度")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 100)]
        public double SpecularIntensity
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetSpecularIntensity(mInner); } }
            set 
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetSpecularIntensity(mInner, (float)value);
                    CalcLightRange();
                    OnPropertyChanged("SpecularIntensity");
                }
            }
        }

		CSUtility.Support.Color mDiffuse;
        /// <summary>
        /// 调整灯光的漫反射颜色
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Diffuse")]
		[System.ComponentModel.Category("常用")]
		[CSUtility.Editor.Editor_ColorPicker]
        [DisplayName("漫反射颜色")]
        [CSUtility.AISystem.Attribute.AllowMember("灯光.漫反射颜色", CSUtility.Helper.enCSType.Client, "调整灯光的颜色")]
        [CSUtility.Event.Attribute.AllowMember("灯光.漫反射颜色", CSUtility.Helper.enCSType.Client, "调整灯光的颜色")]
        public CSUtility.Support.Color Diffuse
        {
            get 
            {
                unsafe
                {
                    SlimDX.Vector4 color = new SlimDX.Vector4();
                    DllImportAPI.vLightProxy_GetDiffuse(mInner, &color);
                    CSUtility.Program.Vector2DrawColor(color, out mDiffuse);
                    return mDiffuse;
                }
            }
            set 
            {
                unsafe
                {
                    mDiffuse = value;
                    SlimDX.Vector4 color = new SlimDX.Vector4();
                    CSUtility.Program.DrawColor2Vector(mDiffuse, out color);
                    DllImportAPI.vLightProxy_SetDiffuse(mInner, &color);
                    CalcLightRange();
                }
                OnPropertyChanged("Diffuse");
            }
        }

		CSUtility.Support.Color mAmbient;
        /// <summary>
        /// 调整灯光的环境光颜色
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Ambient")]
		[System.ComponentModel.Category("常用")]
        [DisplayName("环境光颜色")]
        [CSUtility.Editor.Editor_ColorPicker]
        [CSUtility.AISystem.Attribute.AllowMember("灯光.环境光颜色", CSUtility.Helper.enCSType.Client, "调整灯光的环境光颜色")]
        [CSUtility.Event.Attribute.AllowMember("灯光.环境光颜色", CSUtility.Helper.enCSType.Client, "调整灯光的环境光颜色")]
        public CSUtility.Support.Color Ambient
        {
            get 
            {
                unsafe
                {
                    SlimDX.Vector4 color = new SlimDX.Vector4();
                    DllImportAPI.vLightProxy_GetAmbient(mInner, &color);
                    CSUtility.Program.Vector2DrawColor(color, out mAmbient);
                    return mAmbient;
                }
            }
            set 
            {
                unsafe
                {
                    mAmbient = value;
                    SlimDX.Vector4 color = new SlimDX.Vector4();
                    CSUtility.Program.DrawColor2Vector(mAmbient, out color);
                    DllImportAPI.vLightProxy_SetAmbient(mInner, &color);
                    CalcLightRange();
                }
                OnPropertyChanged("Ambient");
            }
        }

		CSUtility.Support.Color mSpecular;
        /// <summary>
        /// 调整灯光的反射光颜色
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Specular")]
		[System.ComponentModel.Category("常用")]
		[CSUtility.Editor.Editor_ColorPicker]
        [DisplayName("反射光颜色")]
        [CSUtility.AISystem.Attribute.AllowMember("灯光.反射光颜色", CSUtility.Helper.enCSType.Client, "调整灯光的反射光颜色")]
        [CSUtility.Event.Attribute.AllowMember("灯光.反射光颜色", CSUtility.Helper.enCSType.Client, "调整灯光的反射光颜色")]
        public CSUtility.Support.Color Specular
        {
            get 
            {
                unsafe
                {
                    SlimDX.Vector4 color = new SlimDX.Vector4();
                    DllImportAPI.vLightProxy_GetSpecular(mInner, &color);
                    CSUtility.Program.Vector2DrawColor(color, out mSpecular);
                    return mSpecular;
                }
            }
            set 
            {
                unsafe
                {
                    mSpecular = value;
                    SlimDX.Vector4 color = new SlimDX.Vector4();
                    CSUtility.Program.DrawColor2Vector(mSpecular, out color);
                    DllImportAPI.vLightProxy_SetSpecular(mInner, &color);
                    CalcLightRange();
                    OnPropertyChanged("Specular");
                }
            }
        }
        /// <summary>
        /// 设置光源的衰减
        /// </summary>
		[System.ComponentModel.BrowsableAttribute(false)]
        [CSUtility.Support.DoNotCopy]
        public SlimDX.Vector3 Attenuation
        {
            get 
            {
                unsafe
                {
                    SlimDX.Vector3 atten = new SlimDX.Vector3();
                    DllImportAPI.vLightProxy_GetAttenuation(mInner, &atten);
                    return atten;
                }
            }
            set 
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetAttenuation(mInner, &value);
                    CalcLightRange();
                }
                OnPropertyChanged("Attenuation");
            }
        }
        /// <summary>
        /// 光源的线性衰减
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Atten2")]
		[System.ComponentModel.Category("常用")]
		[System.ComponentModel.DisplayName("线性衰减")]
		[CSUtility.Editor.Editor_ValueWithRange(0, 10)]
        public double Atten2
        {
            get{return Attenuation.Y;}
            set
            {
                Attenuation = new SlimDX.Vector3(Attenuation.X, (float)value, Attenuation.Z);
                OnPropertyChanged("Atten2");
            }
        }
        /// <summary>
        /// 光源的二次方衰减
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Atten3")]
		[System.ComponentModel.Category("光源属性")]
		[System.ComponentModel.DisplayName("二次方衰减")]
		[CSUtility.Editor.Editor_ValueWithRange(0, 5)]
        public double Atten3
        {
            get { return Attenuation.Z; }
            set
            {
                Attenuation = new SlimDX.Vector3(Attenuation.X, Attenuation.Y, (float)value);
                OnPropertyChanged("Atten3");
            }
        }
        /// <summary>
        /// 光源上的黑点
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("BlackPoint")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.DisplayName("黑点")]
        [CSUtility.Editor.Editor_ValueWithRange(0.05, 1)]
        public double BlackPoint
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.vLightProxy_GetBlackPoint(mInner);
                }
            }
            set
            {
                DllImportAPI.vLightProxy_SetBlackPoint(mInner, (float)value);
                OnPropertyChanged("BlackPoint");
            }
        }

        // Spot Light Attributes
        /// <summary>
        /// 光源的Z值最近点
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("ZNear")]
		[System.ComponentModel.Category("光源属性")]
        public float ZNear
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetZNear(mInner); } }
            set 
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetZNear(mInner, value);
                }
                OnPropertyChanged("ZNear");
            }
        }
        /// <summary>
        /// 光源的Z值最远点
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("ZFar")]
		[System.ComponentModel.Category("光源属性")]
        public float ZFar
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetZFar(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetZFar(mInner, value);
                }
                OnPropertyChanged("ZFar");
            }
        }
        /// <summary>
        /// 点光源的百分比
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public double PointInnerPercent
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetPointInnerPercent(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetPointInnerPercent(mInner, (float)value);
                }
                OnPropertyChanged("PointInnerPercent");
            }
        }
        /// <summary>
        /// 聚光灯的FOV百分比
        /// </summary>
		[System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public double SpotInnerFOVPercent
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetSpotInnerPercent(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetSpotInnerPercent(mInner, (float)value);
                }
                OnPropertyChanged("SpotInnerFOVPercent");
            }
        }

        /// <summary>
        /// 是否可进行鼠标点选
        /// </summary>
		[System.ComponentModel.BrowsableAttribute(false)]
        [CSUtility.Support.DoNotCopy]
        public bool CanHitProxy
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetCanHitProxy(mInner)>0 ? true : false; } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetCanHitProxy(mInner, value==true ? 1 : 0);
                }
                OnPropertyChanged("CanHitProxy");
            }
        }

		bool	mShowRangeMesh = false;				// 是否显示代表光照范围的Mesh
        /// <summary>
        /// 是否显示代表光照范围的Mesh
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool ShowRangeMesh
        {
            get { return mShowRangeMesh; }
            set
            {
                mShowRangeMesh = value;
                OnPropertyChanged("ShowRangeMesh");
            }
        }

		bool mShowSignMesh = false;					// 是否显示代表光源的符号Mesh
        /// <summary>
        /// 是否显示代表光源的符号Mesh
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool ShowSignMesh
        {
            get { return mShowSignMesh; }
            set
            {
                mShowSignMesh = value;
                OnPropertyChanged("ShowSignMesh");
            }
        }

		float mSignMeshSize = 1;				// SignMesh的大小，保证在屏幕上大小相同
        /// <summary>
        /// SignMesh的大小，保证在屏幕上大小相同
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public float SignMeshSize
        {
            get { return mSignMeshSize; }
            set
            {
                mSignMeshSize = value;
                OnPropertyChanged("SignMeshSize");
            }
        }
        /// <summary>
        /// 光源的阴影类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("ShadowType")]
        [System.ComponentModel.Category("阴影属性")]
        [System.ComponentModel.DisplayName("阴影类型")]
        public EShadowType ShadowType
        {
            get { unsafe { return (EShadowType)DllImportAPI.vLightProxy_GetShadowType(mInner); } }
            set
            {
                unsafe
                {
                    if (this.HostActor != null && this.HostActor.World != null)
                    {
                        switch (value)
                        {
                            case EShadowType.None:
                                this.HostActor.World.ShadowLights.Remove(this.HostActor.Id);
                                break;
                            default:
                                if (!this.HostActor.World.ShadowLights.ContainsKey(this.HostActor.Id))
                                    this.HostActor.World.ShadowLights.Add(this.HostActor.Id, this);
                                break;
                        }
                    }

                    DllImportAPI.vLightProxy_SetShadowType(mInner, (int)value);
                    OnPropertyChanged("ShadowType");
                }
            }
        }
        /// <summary>
        /// SM纹理的大小
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("ShadowMapSize")]
		[System.ComponentModel.ReadOnly(true)]
		[System.ComponentModel.Category("阴影属性")]
		[System.ComponentModel.DisplayName("SM纹理大小")]
        public float ShadowMapSize
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetShadowMapSize(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetShadowMapSize(mInner, (float)value);
                    OnPropertyChanged("ShadowMapSize");
                }
            }
        }
        /// <summary>
        /// 阴影覆盖的大小
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("ShadowCoverSize")]
		[System.ComponentModel.ReadOnly(true)]
		[System.ComponentModel.Category("阴影属性")]
		[System.ComponentModel.DisplayName("阴影覆盖")]
        public float ShadowCoverSize
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetShadowCoverSize(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetShadowCoverSize(mInner, (float)value);
                    OnPropertyChanged("ShadowCoverSize");
                }
            }
        }

    	float mEditorShadowMapSize = 2048;
        /// <summary>
        /// Editor纹理的大小
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("EditorShadowMapSize")]
		[CSUtility.Editor.Editor_MultipleOfTwo]
		[System.ComponentModel.Category("阴影属性")]
		[System.ComponentModel.DisplayName("Editor纹理大小")]
        public float EditorShadowMapSize
        {
            get{return mEditorShadowMapSize;}
            set
            {
                mEditorShadowMapSize = value;
                OnPropertyChanged("EditorShadowMapSize");
            }
        }

		float mEditorShadowCoverSize = 400;
        /// <summary>
        /// Editor阴影覆盖的大小
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("EditorShadowCoverSize")]
		[System.ComponentModel.Category("阴影属性")]
		[System.ComponentModel.DisplayName("Editor阴影覆盖")]
        public float EditorShadowCoverSize
        {
            get{return mEditorShadowCoverSize;}
            set
            {
                mEditorShadowCoverSize = value;
                OnPropertyChanged("EditorShadowCoverSize");
            }
        }

		float mGameShadowMapSize = 1024;
        /// <summary>
        /// Game纹理大小
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("GameShadowMapSize")]
		[System.ComponentModel.Category("阴影属性")]
		[System.ComponentModel.DisplayName("Game纹理大小")]
        public float GameShadowMapSize
        {
            get{return mGameShadowMapSize;}
            set
            {
                mGameShadowMapSize = value;
                OnPropertyChanged("GameShadowMapSize");
            }
        }

		float mGameShadowCoverSize = 50;
        /// <summary>
        /// Game阴影覆盖的大小
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("GameShadowCoverSize")]
		[System.ComponentModel.Category("阴影属性")]
		[System.ComponentModel.DisplayName("Game阴影覆盖")]
        public float GameShadowCoverSize
        {
            get { return mGameShadowCoverSize; }
            set
            {
                mGameShadowCoverSize = value;
                OnPropertyChanged("GameShadowCoverSize");
            }
        }

        /// <summary>
        /// 设置开启阴影模糊
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("DoVSMBlur")]
        [System.ComponentModel.Category("阴影模糊")]
        [System.ComponentModel.DisplayName("开启阴影模糊")]
        public bool DoVSMBlur
        {
            get 
            { 
                unsafe 
                { 
                    return DllImportAPI.vLightProxy_GetDoVSMBlur(mInner)==0 ? false : true; 
                } 
            }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetDoVSMBlur(mInner, value?1:0);
                    OnPropertyChanged("DoVSMBlur");
                }
            }
        }

        /// <summary>
        /// 阴影不透明度
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("ShadowDarkScale")]
        [System.ComponentModel.Category("阴影属性")]
        [System.ComponentModel.DisplayName("阴影不透明度")]
        public float ShadowDarkScale
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetShadowDarkScale(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetShadowDarkScale(mInner, value);
                }
                OnPropertyChanged("ShadowDarkScale");
            }
        }
        /// <summary>
        /// 阴影的模糊次数
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("BlurAmount")]
        [System.ComponentModel.Category("阴影模糊")]
        [System.ComponentModel.DisplayName("模糊次数")]
        [CSUtility.Editor.Editor_ValueWithRange(1, 3)]
        public double BlurAmount
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetBlurAmount(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetBlurAmount(mInner, (float)value);
                }
                OnPropertyChanged("BlurAmount");
            }
        }
        /// <summary>
        /// 阴影模糊的LBR值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("LBRAmount")]
        [System.ComponentModel.Category("阴影模糊")]
        [System.ComponentModel.DisplayName("LBR")]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 1)]
        public double LBRAmount
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetLBRAmount(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetLBRAmount(mInner, (float)value);
                }
                OnPropertyChanged("LBRAmount");
            }
        }
        /// <summary>
        /// 阴影透明度
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public float ShadowAlpha
        {
            get { unsafe { return DllImportAPI.vLightProxy_GetShadowAlpha(mInner); } }
            set
            {
                unsafe
                {
                    DllImportAPI.vLightProxy_SetShadowAlpha(mInner, value);
                }
                OnPropertyChanged("ShadowAlpha");
            }
        }

        /// <summary>
        /// 光源的开启时间
        /// </summary>
        protected int mBrightStartHour = 0;
        /// <summary>
        /// 设置光源的开启时间
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("BrightStartHour")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.DisplayName("开启时间")]
        public int BrightStartHour
        {
            get { return mBrightStartHour; }
            set
            {
                mBrightStartHour = value;
        		OnPropertyChanged("BrightStartHour");
            }
        }
        /// <summary>
        /// 光源的开启时长
        /// </summary>
        protected int mBrightLifetime = 24;
        /// <summary>
        /// 光源的开启时长
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("BrightLifetime")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.DisplayName("开启时长")]
        public int BrightLifetime
        {
            get { return mBrightLifetime; }
            set
            {
                mBrightLifetime = value;
        		OnPropertyChanged("BrightLifetime");
            }
        }

        SlimDX.Vector3 mPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 光源的位置，可在Game窗口拖动改变
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Pos")]
		[System.ComponentModel.Browsable(false)]
        public SlimDX.Vector3 Pos
        {
            get
            {
                if (HostActor != null)
                    return HostActor.Placement.GetLocation();
                return mPos;
            }
            set
            {
                mPos = value;
		        if (HostActor != null)
			        HostActor.Placement.SetLocation(ref value);

        		OnPropertyChanged("Pos");
            }
        }

        SlimDX.Quaternion mQuat = SlimDX.Quaternion.Identity;
        /// <summary>
        /// 光源位置的四元数
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Quat")]
		[System.ComponentModel.Browsable(false)]
        public SlimDX.Quaternion Quat
        {
            get
            {
                if (HostActor != null)
                    return HostActor.Placement.GetRotation();
                return mQuat;
            }
            set
            {
                mQuat = value;
                if (HostActor != null)
                    HostActor.Placement.SetRotation(ref value);

                OnPropertyChanged("Quat");
            }
        }

        SlimDX.Vector3 mScale = SlimDX.Vector3.UnitXYZ;
        /// <summary>
        /// 光源的整体缩放值
        /// </summary>
		[CSUtility.Support.AutoSaveLoad]
		[CSUtility.Support.DataValueAttribute("Scale")]
		[System.ComponentModel.Browsable(false)]
        public SlimDX.Vector3 Scale
        {
            get
            {
                if (HostActor != null)
                    return HostActor.Placement.GetScale();
                return mScale;
            }
            set
            {
                mScale = value;
		        if (HostActor != null)
			        HostActor.Placement.SetScale(ref value);

        		OnPropertyChanged("Scale");
            }
        }
        /// <summary>
        /// 是否渲染静态阴影
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool IsRenderStaticShadow
        {
            get
            {
                return DllImportAPI.vLightProxy_GetIsRenderStaticShadow(mInner) != 0;
            }
            set
            {
                DllImportAPI.vLightProxy_SetIsRenderStaticShadow(mInner, value ? 1 : 0);
            }
        }
        /// <summary>
        /// 静态阴影的真正的次数
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool RealtimeStaticShadow
        {
            get
            {
                return DllImportAPI.vLightProxy_GetRealtimeStaticShadow(mInner) != 0;
            }
            set
            {
                DllImportAPI.vLightProxy_SetRealtimeStaticShadow(mInner, value ? 1 : 0);
            }
        }

        Int64 blendDuration = 3000;
        Int64 currBlendTime = 0;
        bool bCurrBright = false;
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="host">所属的Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(CSUtility.Component.ActorBase host, long elapsedMillisecond)
        {
            base.Tick(host, elapsedMillisecond);

            if (mBrightLifetime < 24)
            {
                bool bBright = false;
                var currHour = CCore.WeatherSystem.IlluminationManager.Instance.CurrTime.Hour;
                if (mBrightStartHour + mBrightLifetime > 24)
                {
                    if (currHour > mBrightStartHour || currHour + 24 < mBrightStartHour + mBrightLifetime)
                    {
                        bBright = true;
                    }
                }
                else
                {
                    if (currHour >= mBrightStartHour && currHour <= mBrightStartHour + mBrightLifetime)
                    {
                        bBright = true;
                    }
                }

                if (bCurrBright != bBright)
                {
                    currBlendTime = 0;
                    bCurrBright = bBright;
                }

                currBlendTime += elapsedMillisecond;


                if (bBright == false)
                {
                    IntensityAlpha = System.Math.Max(0.0, 1.0 - (double)currBlendTime / (double)blendDuration);
                    if (currBlendTime >= blendDuration)
                    {
                        // 变换完再关闭
                        Visible = bBright;
                    }
                }
                else
                {
                    // 需要先打开再变换
                    Visible = bBright;
                    IntensityAlpha = System.Math.Min(1.0, (double)currBlendTime / (double)blendDuration);
                }
            }



            // 调用进入回调
            if (OnTickCB != null)
            {
                var callee = OnTickCB.GetCallee() as FOnTick;
                if (callee != null)
                {
                    callee(this);
                }
            }
        }

        #region 回调函数
        /// <summary>
        /// 声明每帧调用时的委托事件
        /// </summary>
        /// <param name="light">光源对象</param>
        [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Light")]
        public delegate void FOnTick(Light light);
        CSUtility.Helper.EventCallBack mOnTickCB;
        /// <summary>
        /// 每帧调用的事件回调
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack OnTickCB
        {
            get { return mOnTickCB; }
        }

        Guid mOnTick = Guid.Empty;
        /// <summary>
        /// 进入Trigger调用回调
        /// </summary>
        [CSUtility.Support.DataValueAttribute("OnTick")]
        [CSUtility.Support.AutoSaveLoadAttribute]        
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FOnTick) })]
        public Guid OnTick
        {// 进入Trigger调用回调
            get { return mOnTick; }
            set
            {
                mOnTick = value;
                mOnTickCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnTick), value);
            }
        }
        #endregion

    }



}
