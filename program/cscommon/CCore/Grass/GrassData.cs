using System;
using System.ComponentModel;

namespace CCore.Grass
{
    /// <summary>
    /// 声明草数据保存的委托事件
    /// </summary>
    /// <param name="xndAttrib">XND数据的指针</param>
    public delegate void Delegate_GrassData_Save(IntPtr xndAttrib);
    /// <summary>
    /// 声明草数据加载时调用的委托事件
    /// </summary>
    /// <param name="xndAttrib">读取的XND数据的指针</param>
    public delegate void Delegate_GrassData_Load(IntPtr xndAttrib);
    //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
    //public unsafe extern static void V3DGrassData_InitializeCallback(IntPtr ptr, Delegate_GrassData_Save saveCallback, Delegate_GrassData_Load loadCallback);
    /// <summary>
    /// 声明草数据加载mesh模板时调用的委托事件
    /// </summary>
    /// <param name="strId">模型模板的实例地址</param>
    /// <returns>返回草数据的实例地址</returns>
    public delegate IntPtr Delegate_GrassData_LoadMeshTemplate(IntPtr strId);
    /// <summary>
    /// 草数据的基类
    /// </summary>
    public abstract class GrassDataBase : CSUtility.Support.XndSaveLoadProxy
    {
        /// <summary>
        /// 得到草数据的实例指针
        /// </summary>
        /// <returns>返回草数据的实例指针</returns>
        public abstract System.IntPtr GetInner();
    };
    /// <summary>
    /// 草的数据类
    /// </summary>
    public class GrassData : GrassDataBase, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 某一属性改变时调用该函数
        /// </summary>
        /// <param name="propertyName">改变的属性的名称</param>
        protected void OnPropertyChanged(string propertyName)
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
        /// <summary>
        /// 声明属性更新时调用的委托事件
        /// </summary>
        /// <param name="gd">更新的草数据</param>
        /// <param name="proName">草数据的名称</param>
        public delegate void Delegate_OnPropertyUpdate(GrassData gd, string proName);
        /// <summary>
        /// 定义草数据更新时调用的委托事件
        /// </summary>
        public Delegate_OnPropertyUpdate OnPropertyUpdate;
        /// <summary>
        /// 草数据的指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 草的实例地址
        /// </summary>
        /// <returns>该对象的地址</returns>
        public override IntPtr GetInner()
        {
            return mInner;
        }
        /// <summary>
        /// 构造函数，创建实例对象
        /// </summary>
        public GrassData()
        {
            unsafe
            {
                mInner = DllImportAPI.V3DGrassData_New();
                //saveCallback = Save;
                //loadCallback = Load;
                //MidLayer.IDllImportAPI.V3DGrassData_InitializeCallback(mInner, saveCallback, loadCallback);
            }
        }
        /// <summary>
        /// 带参的构造函数
        /// </summary>
        /// <param name="mod">草的实例地址</param>
        public GrassData(IntPtr mod)
        {
            unsafe
            {
                mInner = mod;
                if (mInner != IntPtr.Zero)
                {
                    DllImportAPI.V3DGrassData_AddRef(mInner);
                    //saveCallback = Save;
                    //loadCallback = Load;
                    //MidLayer.IDllImportAPI.V3DGrassData_InitializeCallback(mInner, saveCallback, loadCallback);
                }
            }
        }
        /// <summary>
        /// 析构函数，删除对象
        /// </summary>
        ~GrassData()
        {
            Cleanup();
        }
        void Save(IntPtr xndAttrib)
        {
            unsafe
            {
                var att = new CSUtility.Support.XndAttrib(xndAttrib);
                att.BeginWrite();
                Write(att);
                att.EndWrite();
            }
        }
        void Load(IntPtr xndAttrib)
        {
            unsafe
            {
                var att = new CSUtility.Support.XndAttrib(xndAttrib);
                att.BeginRead();
                Read(att);
                att.EndRead();
            }
        }
        /// <summary>
        /// 为草加载模型模板
        /// </summary>
        /// <param name="id">模型模板的实例地址</param>
        /// <returns>返回草的模型实例地址</returns>
        public static IntPtr LoadMeshTemplate(IntPtr id)
        {
            Guid guid = Guid.Empty;
            unsafe
            {
                guid = *(Guid*)id.ToPointer();
            }
            if (guid == Guid.Empty)
                return IntPtr.Zero;

            var mesh = new CCore.Mesh.Mesh();
            var meshInit = new CCore.Mesh.MeshInit();
            meshInit.MeshTemplateID = guid;
            mesh.Initialize(meshInit, null);
            if (mesh.MeshParts.Count > 0)
            {
                return mesh.MeshParts[0].Mesh;
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// 定义草数据加载mesh模板时调用的委托事件
        /// </summary>
        public static Delegate_GrassData_LoadMeshTemplate loadMTCallback = GrassData.LoadMeshTemplate;

        //MidLayer.IDllImportAPI.Delegate_GrassData_Save saveCallback;
        //MidLayer.IDllImportAPI.Delegate_GrassData_Load loadCallback;
        /// <summary>
        /// 释放实例对象的内存
        /// </summary>
        public virtual void Cleanup()
        {
            if(mInner != IntPtr.Zero)
            {
                DllImportAPI.V3DGrassData_Release(mInner);
                mInner = IntPtr.Zero;
            }
        }

        CCore.Mesh.Mesh mGrassMesh = new CCore.Mesh.Mesh();
        Guid mMeshTemplateId = Guid.Empty;
        /// <summary>
        /// 草模型的模板ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSet")]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("草模型")]
        public Guid MeshTemplateId
        {
            get
            {
                unsafe
                {
                    mMeshTemplateId = *DllImportAPI.V3DGrassData_GetMeshTemplateId(mInner);
                }
                return mMeshTemplateId;
            }
            set
            {
                if (mMeshTemplateId == value)
                    return;
                mMeshTemplateId = value;
                unsafe
                {
                    DllImportAPI.V3DGrassData_SetMeshTemplateId(mInner, &value);

                    var meshInit = new CCore.Mesh.MeshInit();
                    meshInit.MeshTemplateID = value;
                    mGrassMesh.Initialize(meshInit, null);
                    //MidLayer.IDllImportAPI.V3DGrassData_SetScale(mInner, meshInit.MeshTemplate.Scale);
                    if (mGrassMesh.MeshParts.Count > 0)
                    {
                        DllImportAPI.V3DGrassData_SetGrassObject(mInner, mGrassMesh.MeshParts[0].Mesh);
                    }

                    // 如果是编辑模式，刷新所有使用matId材质的地表植被。
                    if (CCore.Engine.Instance.IsEditorMode == true)
                    {
                        CCore.Client.MainWorldInstance.Terrain.ResetLayerGrass(mOwnerMatId, this);
                    }
                }
                OnPropertyChanged("MeshTemplateId");
            }
        }

        Guid mOwnerMatId = Guid.Empty;
        /// <summary>
        /// 草模型的材质ID
        /// </summary>
        [Browsable(false)]
        public Guid OwnerMatId
        {
            get{return mOwnerMatId;}
            set
            {
                mOwnerMatId = value;
            }
        }
        /// <summary>
        /// 草纹理帧数
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("草纹理帧数")]
        public int TextureFrameCount
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetTextureFrameCount(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetTextureFrameCount(mInner, value);
                }
                OnPropertyChanged("TextureFrameCount");
            }
        }
        /// <summary>
        /// 草模型的X轴缩放值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("缩放X")]
        public float Scale
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetScale(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetScale(mInner, value);
                }
                OnPropertyChanged("Scale");
            }
        }
        /// <summary>
        /// 草模型的Y轴缩放值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("缩放Y")]
        public float ScaleY
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetScaleY(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetScaleY(mInner, value);
                }
                OnPropertyChanged("ScaleY");
            }
        }
        /// <summary>
        /// 草模型在受外力后最大偏移(左右方向)
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("受外力后最大偏移(左右方向)")]
        public float MaxFalldownRightOffset
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetMaxFalldownRightOffset(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetMaxFalldownRightOffset(mInner, value);
                }
                OnPropertyChanged("MaxFalldownRightOffset");
            }
        }
        /// <summary>
        /// 草模型在受外力后最大偏移(前后方向)
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("受外力后最大偏移(前后方向)")]
        public float MaxFalldownBackOffset
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetMaxFalldownBackOffset(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetMaxFalldownBackOffset(mInner, value);
                }
                OnPropertyChanged("MaxFalldownBackOffset");
            }
        }
        /// <summary>
        /// 草模型的顶点随机权重
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("顶点随机权重")]
        public float VertexWeightOffset
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetVertexWeightOffset(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetVertexWeightOffset(mInner, value);
                }
                OnPropertyChanged("VertexWeightOffset");
            }
        }
        /// <summary>
        /// 种植草模型的间隔(米)
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("种植间隔(米)")]
        public float PlantRate
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetPlantRate(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetPlantRate(mInner, value);
                }
                OnPropertyChanged("PlantRate");
            }
        }
        /// <summary>
        /// 种植草模型的随机偏移(米)
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("种植随机偏移(米)")]
        public float PlantOffset
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetPlantOffset(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetPlantOffset(mInner, value);
                }
                OnPropertyChanged("PlantOffset");
            }
        }
        /// <summary>
        /// 草模型的覆盖率，范围从0.1-1
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("覆盖率")]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 1)]
        public float CoverPercent
        {
            get
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        return DllImportAPI.V3DGrassData_GetCoverPercent(mInner);
                    return 1;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.V3DGrassData_SetCoverPercent(mInner, value);
                }
                OnPropertyChanged("CoverPercent");
            }
        }

    }
}
