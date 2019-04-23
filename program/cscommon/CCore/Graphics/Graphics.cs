using System;
using System.Collections.Generic;

namespace CCore.Graphics
{
    /// <summary>
    /// 设备类型枚举
    /// </summary>
    public enum EDeviceType
    {
        TypeD3D9 = 0,
        TypeGLES2 = 1
    };

    /// <summary>
    /// 声明加载材质时调用的委托事件
    /// </summary>
    /// <param name="name">材质名称</param>
    /// <param name="tech">材质的tchnique描述</param>
    /// <returns>返回加载的材质指针</returns>
    public delegate IntPtr Delegate_RealMaterialLoaderLoadMaterial(IntPtr name, string tech);
    /// <summary>
    /// 声明加载technique时调用的委托事件
    /// </summary>
    /// <param name="techId">材质的techniqueID</param>
    /// <returns>返回加载的technique对象指针</returns>
    public delegate IntPtr Delegate_RealMaterialLoaderLoadTechnique(IntPtr techId);
    /// <summary>
    /// 声明通过材质ID加载材质时调用的委托事件
    /// </summary>
    /// <param name="matId">材质ID</param>
    /// <returns>返回加载的材质的指针</returns>
    public delegate IntPtr Delegate_RealMaterialLoaderLoadMaterialWithGuid(IntPtr matId);
    /// <summary>
    /// 声明创建mesh时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <param name="bShareFile">是否共享</param>
    /// <returns>返回创建的mesh对象指针</returns>
    public delegate IntPtr Delegate_MeshResFactoryCreateRes(IntPtr pszFile, int bShareFile);
    /// <summary>
    /// 声明创建纹理对象时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <param name="bShareFile">是否共享</param>
    /// <returns>返回创建的纹理对象指针</returns>
    public delegate IntPtr Delegate_TextureResFactoryCreateRes(IntPtr pszFile, int bShareFile);
    /// <summary>
    /// 声明创建XND对象时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <param name="bShareFile">是否共享</param>
    /// <returns>返回创建的XND对象地址</returns>
    public delegate IntPtr Delegate_XndResFactoryCreateRes(IntPtr pszFile, int bShareFile);
    /// <summary>
    /// 声明下载纹理对象时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <returns>正在下载返回1，否则返回0</returns>
    public delegate int Delegate_TextureResIsDownloadingEvent(IntPtr pszFile);
    /// <summary>
    /// 声明获取默认纹理资源时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <returns>返回默认的纹理资源指针</returns>
    public delegate IntPtr Delegate_TextureResGetDefaultResource(IntPtr pszFile);
    /// <summary>
    /// 声明等待加载纹理资源时调用的委托事件
    /// </summary>
    /// <param name="pRes">资源指针</param>
    /// <param name="pszFile">文件地址</param>
    public delegate void Delegate_TextureResRegWaitDownloadResource(IntPtr pRes, IntPtr pszFile);

    /// <summary>
    /// 声明下载mesh对象时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <returns>正在下载返回1，否则返回0</returns>
    public delegate int Delegate_MeshResIsDownloadingEvent(IntPtr pszFile);
    /// <summary>
    /// 声明获取默认mesh资源时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <returns>返回默认的mesh资源指针</returns>
    public delegate IntPtr Delegate_MeshResGetDefaultResource(IntPtr pszFile);
    /// <summary>
    /// 声明等待加载mesh资源时调用的委托事件
    /// </summary>
    /// <param name="pRes">资源指针</param>
    /// <param name="pszFile">文件地址</param>
    public delegate void Delegate_MeshResRegWaitDownloadResource(IntPtr pRes, IntPtr pszFile);

    /// <summary>
    /// 声明下载XND数据时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <returns>正在下载返回1，否则返回0</returns>
    public delegate int Delegate_XndResIsDownloadingEvent(IntPtr pszFile);
    /// <summary>
    /// 声明获取默认XND数据资源时调用的委托事件
    /// </summary>
    /// <param name="pszFile">文件地址</param>
    /// <returns>返回默认的XND数据资源指针</returns>
    public delegate IntPtr Delegate_XndResGetDefaultResource(IntPtr pszFile);
    /// <summary>
    /// 声明等待加载XND数据资源时调用的委托事件
    /// </summary>
    /// <param name="pRes">XND数据资源指针</param>
    /// <param name="pszFile">文件地址</param>
    public delegate void Delegate_XndResRegWaitDownloadResource(IntPtr pRes, IntPtr pszFile);

    //struct RealMaterialLoader : public AuxIUnknown<RealMaterialLoader,model3::v3dMaterialLoader>
    //{
    //    virtual model3::v3dStagedMaterialInstance* LoadMaterial(LPCTSTR name,LPCTSTR tech)
    //    {
    //        System::String^ str = gcnew System::String(name);
    //        return NULL;
    //        //return IEngine::Instance->Client->Graphics->MaterialMgr->LoadMaterial( str , tech );
    //    }
    //    virtual model3::v3dStagedMaterialInstance* LoadTechnique(const Guid& techId)
    //    {
    //        std::string str;
    //        techId.ToString(str);
    //        System::String^ idStr = gcnew System::String(str.c_str());
    //        System::Guid tempId = System::Guid::Parse(idStr);

    //        return IEngine::Instance->Client->Graphics->MaterialMgr->LoadMaterialTechInstance(tempId);
    //    }
    //    virtual model3::v3dStagedMaterialInstance* LoadMaterial(const Guid& matId)
    //    {
    //        std::string str;
    //        matId.ToString(str);
    //        System::String^ idStr = gcnew System::String(str.c_str());
    //        System::Guid tempId = System::Guid::Parse(idStr);
    //        return IEngine::Instance->Client->Graphics->MaterialMgr->LoadMaterialInstance(tempId);
    //    }
    //};

    //struct MeshResFactory : public AuxIUnknown< MeshResFactory,VResFactory >
    //{
    //    virtual VRes2Memory* CreateRes(LPCTSTR pszFile,BOOL bShareFile)
    //    {
    //        return (VRes2Memory*)(VRes2Memory*)(CSUtility::Support::IFileManager::Instance->OpenFileForRead(gcnew System::String(pszFile) , CSUtility::EFileType::Mesh , false).ToPointer());
    //    }
    //};

    //struct TextureResFactory : public AuxIUnknown< TextureResFactory,VResFactory >
    //{
    //    virtual VRes2Memory* CreateRes(LPCTSTR pszFile,BOOL bShareFile)
    //    {
    //        return (VRes2Memory*)(VRes2Memory*)(CSUtility::Support::IFileManager::Instance->OpenFileForRead(gcnew System::String(pszFile) , CSUtility::EFileType::Texture , false).ToPointer());
    //    }
    //};

    //struct XndResFactory : public AuxIUnknown< XndResFactory,VResFactory >
    //{
    //    virtual VRes2Memory* CreateRes(LPCTSTR pszFile,BOOL bShareFile)
    //    {
    //        return (VRes2Memory*)(VRes2Memory*)(CSUtility::Support::IFileManager::Instance->OpenFileForRead(gcnew System::String(pszFile) , CSUtility::EFileType::Xnd, false).ToPointer());
    //    }
    //};
    /// <summary>
    /// 显示模式
    /// </summary>
    public class DisplayMode
    {
        /// <summary>
        /// 显示横向的像素值，默认为800
        /// </summary>
        public int w = 800;
        /// <summary>
        /// 显示垂直方向的像素值，默认为800
        /// </summary>
        public int h = 600;
    }
    /// <summary>
    /// 图形显示的初始化类
    /// </summary>
    public class GraphicsInit
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public EDeviceType DeviceType = EDeviceType.TypeD3D9;
        //选择第几块显卡
        /// <summary>
        /// 选择第几块显卡，缺省用主显卡
        /// </summary>
        public int Adapter = 0;//缺省用主显卡
        //设备附属窗口
        /// <summary>
        /// 设备附属窗口，缺省用主窗口
        /// </summary>
        public IntPtr hDeviceWindow;// = System.Convert.ToIntPtr(0);//缺省用主窗口
        //是否启动窗口模式
        /// <summary>
        /// 是否启动窗口模式，缺省用窗口模式
        /// </summary>
        public bool Windowed = true;//缺省用窗口模式
        //不垂直同步
        /// <summary>
        /// 是否垂直同步，缺省不垂直同步
        /// </summary>
        public bool Immediate = true;//缺省不垂直同步，用来看极限性能，正式发布需要关闭
        /// <summary>
        /// 构造函数
        /// </summary>
        public GraphicsInit() { }
    }
    /// <summary>
    /// 图形显示类
    /// </summary>
    public class Graphics
    {
        IntPtr mGraphics; // v3dGraphics
        /// <summary>
        /// 初始化图形显示的类对象
        /// </summary>
        public GraphicsInit mInit = null;

        bool mIsDeviceLost = false;
        /// <summary>
        /// 显示设备是否丢失
        /// </summary>
        public bool IsDeviceLost
        {
            get { return mIsDeviceLost; }
            set
            {
                mIsDeviceLost = value;
                if (value == true)
                {
                    Log.FileLog.WriteLine("DeviceLost!");
                }
            }
        }
        /// <summary>
        /// 只读属性，当前设备指针
        /// </summary>
        public IntPtr Device
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dGraphics_GetDevice(mGraphics);
                }
            }
        }
        /// <summary>
        /// 材质信息
        /// </summary>
        protected CCore.Material.MaterialMgr mMaterialMgr;
        /// <summary>
        /// 只读属性，材质信息
        /// </summary>
        public CCore.Material.MaterialMgr MaterialMgr
        {
            get { return mMaterialMgr; }
        }

        List<DisplayMode> mDisplayModes = new List<DisplayMode>();
        /// <summary>
        /// 只读属性，显示模式列表
        /// </summary>
        public List<DisplayMode> DisplayModes
        {
            get { return mDisplayModes; }
        }
        /// <summary>
        /// 当前材质过滤类型
        /// </summary>
        public CCore.Performance.EMaterialFilter CurrMaterialFilter
        {
            get
            {
                unsafe
                {
                    return (CCore.Performance.EMaterialFilter)DllImportAPI.v3dDevice_GetCurrMaterialFilter(Device);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dDevice_SetCurrMaterialFilter(Device, (int)value);
                }
            }
        }
        /// <summary>
        /// 当前的材质级别
        /// </summary>
        public int CurrMaterialLOD
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dDevice_GetCurrMaterialLOD(Device);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dDevice_SetCurrMaterialLOD(Device, (int)value);
                }
            }
        }
        /// <summary>
        /// 是否进行Gamma校正
        /// </summary>
        public bool GammaCorrect
        {
            get
            {
                unsafe
                {
                    return (DllImportAPI.v3dDevice_GetGammaCorrect(Device) != 0) ? true : false;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dDevice_SetGammaCorrect(Device, value);
                }
            }
        }

        // 是否使用SRGB空间的帖图来处理，
        /// <summary>
        /// 是否使用SRGB空间的帖图来处理
        /// </summary>
        public bool UseSRGBSpace
        {
            get
            {
                unsafe
                {
                    return (DllImportAPI.v3dDevice_GetUseSRGBSpace(Device) != 0) ? true : false;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dDevice_SetUseSRGBSpace(Device, value);
                }
            }
        }
        /// <summary>
        /// 只写属性，是否为编辑模式
        /// </summary>
        public bool IsEditorMode
        {
            set
            {
                unsafe
                {
                    DllImportAPI.v3dDevice_SetIsEditorMode(Device, value);
                }
            }
        }
        /// <summary>
        /// 开始绘制
        /// </summary>
        public void BeginDraw()
        {
            unsafe
            {
                DllImportAPI.v3dDevice_BeginDraw(Device);
            }
        }
        /// <summary>
        /// 结束绘制
        /// </summary>
        public void EndDraw()
        {
            unsafe
            {
                DllImportAPI.v3dDevice_EndDraw(Device);
            }
        }

        #region 统计信息
        /// <summary>
        /// 获取绘制的三角形数量
        /// </summary>
        /// <returns>返回绘制的三角形数量</returns>
        public int GetDrawTriangleCount()
        {
            unsafe
            {
                return DllImportAPI.v3dDevice_RenderDevice_GetDrawTriangleCount(Device);
            }
        }
        /// <summary>
        /// 获取绘制的DP数量
        /// </summary>
        /// <returns>返回绘制的DP数量</returns>
        public int GetDPCount()
        {
            unsafe
            {
                return DllImportAPI.v3dDevice_RenderDevice_GetDPCount(Device);
            }
        }
        /// <summary>
        /// 获取删除的对象数量
        /// </summary>
        /// <returns>返回删除的对象数量</returns>
        public int GetClearCount()
        {
            unsafe
            {
                return DllImportAPI.v3dDevice_RenderDevice_GetClearCount(Device);
            }
        }
        /// <summary>
        /// 获取相应类型的资源数量
        /// </summary>
        /// <param name="resType">资源类型</param>
        /// <returns>返回相应类型的资源数量</returns>
        public int GetResourceCountByType(int resType)
        {
            unsafe
            {
                return DllImportAPI.v3dGraphics_GetResourceCountByType(mGraphics,resType);
            }
        }
        /// <summary>
        /// 获取使用资源前强制加载的数量
        /// </summary>
        /// <returns>返回使用资源前强制加载的数量</returns>
        public int GetResourceForcePreUseCount()
        {
            unsafe
            {
                return DllImportAPI.v3dGraphics_GetResourceForcePreUseCount();
            }
        }
        /// <summary>
        /// 获取异步加载的资源数量
        /// </summary>
        /// <returns>返回异步加载的资源数量</returns>
        public int GetResourceAsyncCount()
        {
            unsafe
            {
                return DllImportAPI.v3dGraphics_GetResourceAsyncCount();
            }
        }
        #endregion
        /// <summary>
        /// 构造函数，创建实例对象
        /// </summary>
        public Graphics()
        {
            unsafe
            {
                mGraphics = DllImportAPI.v3dGraphics_New();
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
		~Graphics()
        {
            Cleanup();
        }

        static Delegate_RealMaterialLoaderLoadMaterial realMaterialLoaderLoadMaterialEvent = Graphics.RealMaterialLoader_LoadMaterial;
        static Delegate_RealMaterialLoaderLoadTechnique realMaterialLoaderLoadTechniqueEvent = Graphics.RealMaterialLoader_LoadTechnique;
        static Delegate_RealMaterialLoaderLoadMaterialWithGuid realMaterialLoaderLoadMaterialWithGuidEvent = Graphics.RealMaterialLoader_LoadMaterialWithGuid;
        static Delegate_MeshResFactoryCreateRes meshResFactoryCreateResEvent = Graphics.MeshResFactory_CreateRes;
        static Delegate_TextureResFactoryCreateRes textureResFactoryCreateResEvent = Graphics.TextureResFactory_CreateRes;
        static Delegate_XndResFactoryCreateRes xndResFactoryCreateRes = Graphics.XndResFactory_CreateRes;

        static Delegate_TextureResIsDownloadingEvent textureResIsDownloadingEvent = Graphics.TextureResFactory_IsDownloadingEvent;
        static Delegate_TextureResGetDefaultResource textureResGetDefaultResource = Graphics.TextureResFactory_GetDefaultResource;
        static Delegate_TextureResRegWaitDownloadResource textureResRegWaitDownloadResource = Graphics.TextureResFactory_RegWaitDownloadResource;

        static Delegate_MeshResIsDownloadingEvent meshResIsDownloadingEvent = Graphics.MeshResFactory_IsDownloadingEvent;
        static Delegate_MeshResGetDefaultResource meshResGetDefaultResource = Graphics.MeshResFactory_GetDefaultResource;
        static Delegate_MeshResRegWaitDownloadResource meshResRegWaitDownloadResource = Graphics.MeshResFactory_RegWaitDownloadResource;

        static Delegate_XndResIsDownloadingEvent xndResIsDownloadingEvent = Graphics.XndResFactory_IsDownloadingEvent;
        static Delegate_XndResGetDefaultResource xndResGetDefaultResource = Graphics.XndResFactory_GetDefaultResource;
        static Delegate_XndResRegWaitDownloadResource xndResRegWaitDownloadResource = Graphics.XndResFactory_RegWaitDownloadResource;
        /// <summary>
        /// 开辟IO线程
        /// </summary>
        public void StartIOThread()
        {
            DllImportAPI.v3dDevice_StartIOThread();
        }
        /// <summary>
        /// 结束IO线程
        /// </summary>
        public void EndIOThread()
        {
            DllImportAPI.v3dDevice_EndIOThread();
        }
        /// <summary>
        /// 暂停IO线程
        /// </summary>
        public void PauseIOThread()
        {
            CCore.DllImportAPI.vLoadPipe_Pause();
        }
        /// <summary>
        /// 继续开始IO线程
        /// </summary>
        public void ResumeIOThread()
        {
            CCore.DllImportAPI.vLoadPipe_Resume();
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">用于初始化该对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(GraphicsInit _init)
        {
            unsafe
            {
                if (Device != IntPtr.Zero)
                    return true;

                mInit = _init;

                var effectPath = CSUtility.Support.IFileManager.Instance.Root + "shader/";
                var fvfPath = CSUtility.Support.IFileManager.Instance.Root + "shader/fvf/";
                var binaryEffectPath = CSUtility.Support.IFileManager.Instance.Root + "shader/cachebin/";
                var defaultResourcePath = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/default/";
                
                var result = DllImportAPI.v3dGraphics_Initialize(mGraphics,
                                                                 (int)mInit.DeviceType,
                                                                 fvfPath, 
                                                                 effectPath, 
                                                                 binaryEffectPath,
                                                                 defaultResourcePath,
                                                                 (UInt32)_init.Adapter,
                                                                 _init.hDeviceWindow.ToPointer(),
                                                                 _init.Windowed,
                                                                 _init.Immediate,
                                                                 Engine.Instance.GetFrameMillisecond(),
                                                                 realMaterialLoaderLoadMaterialEvent,
                                                                 realMaterialLoaderLoadTechniqueEvent,
                                                                 realMaterialLoaderLoadMaterialWithGuidEvent,
                                                                 meshResFactoryCreateResEvent,
                                                                 textureResFactoryCreateResEvent,
                                                                 xndResFactoryCreateRes);

                if (result == 0)
                    return false;

                DllImportAPI.v3dGraphics_InitShadingEnv();

                mDisplayModes.Clear();
                for(int i = 0; i < DllImportAPI.v3dDevice_GetDisplayModeCount(Device); ++i )
                {
                    var mode = new DisplayMode();
                    fixed (int* w = &mode.w)
                    {
                        fixed (int* h = &mode.h)
                        {
                            DllImportAPI.v3dDevice_GetDisplayMode(Device, i, w, h);
                        }
                    }
                    mDisplayModes.Add(mode);
                }

                DllImportAPI.v3dGraphics_InitializeTextureResDownloadEvent(mGraphics,
                                                     textureResIsDownloadingEvent,
                                                     textureResGetDefaultResource,
                                                     textureResRegWaitDownloadResource);
                DllImportAPI.v3dGraphics_InitializeMeshResDownloadEvent(mGraphics,
                                                     meshResIsDownloadingEvent,
                                                     meshResGetDefaultResource,
                                                     meshResRegWaitDownloadResource);
                DllImportAPI.v3dGraphics_InitializeXndResDownloadEvent(mGraphics,
                                                     xndResIsDownloadingEvent,
                                                     xndResGetDefaultResource,
                                                     xndResRegWaitDownloadResource);
		    
                mMaterialMgr = new CCore.Material.MaterialMgr();

                var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.ShadowMapRenderTechniqueId);
                if(mtl!=null)
                    DllImportAPI.v3dDevice_SetSunLightShadowMapRenderMaterial(Device, mtl.MaterialPtr);
                var headLightMtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.HeadLightShadowMapRenderTechniqueId);
                if(headLightMtl!=null)
                    DllImportAPI.v3dDevice_SetHeadLightShadowMapRenderMaterial(Device, headLightMtl.MaterialPtr);
               

                CSUtility.FileDownload.FileDownloadManager.Instance.OnFileDownloadComplete -= _OnFileDownloadComplete;
                CSUtility.FileDownload.FileDownloadManager.Instance.OnFileDownloadComplete += _OnFileDownloadComplete;

                CSUtility.Compress.CompressManager.Instance.OnDebug = _OnDebug;

                DllImportAPI.v3dDevice_SetDefaultFontName(Device, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultFont);
                DllImportAPI.v3dDevice_SetDefaultShadowSmoothTextureName(Device, CSUtility.Support.IFileConfig.ShadowMapSmoothTextureFile);
                DllImportAPI.v3dDevice_SetDefaultEmptyTextureName(Device, CSUtility.Support.IFileConfig.DefaultEmptyTextureFile);
                

                return (result != 0) ? true : false;
            }
        }
        /// <summary>
        /// 放弃设备资源
        /// </summary>
        public void InvalidateDeviceResource()
        {
            CCore.DllImportAPI.v3dDevice_InvalidateObjects(Device);
        }
        /// <summary>
        /// 存储设备资源
        /// </summary>
        public void RestoreDeviceResource()
        {
            CCore.DllImportAPI.v3dDevice_RestoreObjects(Device);
        }
        /// <summary>
        /// 设备数据重置
        /// </summary>
        public void BeforeDeviceReset()
        {
            CCore.DllImportAPI.v3dDevice_BeforeDeviceReset(Device);
        }
        /// <summary>
        /// 尝试设备重置
        /// </summary>
        /// <param name="hWindow">窗口句柄指针</param>
        /// <returns>重置成功返回true，否则返回false</returns>
        public bool TryDeviceReset(IntPtr hWindow)
        {
            if (1 == CCore.DllImportAPI.v3dDevice_TryDeviceReset(Device, hWindow))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 设置异步加载的对象
        /// </summary>
        /// <param name="callback">异步加载时调用的方法函数</param>
        public void SetOnAsyncLoadObject(CCore.DllImportAPI.Delegate_FOnAsyncLoadObject callback)
        {
            CCore.DllImportAPI.vLoadPipe_SetAsyncLoadObjectCallBack(callback);
        }
        /// <summary>
        /// 设置OpenGL出错时调用的方法事件
        /// </summary>
        /// <param name="callback">OpenGL出错时调用的方法函数</param>
        public void SetOnGLError(CCore.DllImportAPI.Delegate_FOnGLError callback)
        {
            CCore.DllImportAPI.v3dGraphics_SetOnGLError(callback);
        }
        /// <summary>
        /// 图形中的三角形
        /// </summary>
        public void HelloTriangle()
        {
            DllImportAPI.v3dGraphics_HelloTriangle(this.mGraphics);
        }
        /// <summary>
        /// mesh缓冲池
        /// </summary>
        /// <param name="mesh">mesh对象</param>
        public void CacheMesh(string mesh)
        {

        }
        /// <summary>
        /// 纹理的缓冲池
        /// </summary>
        /// <param name="mesh">纹理对象</param>
        public void CacheTexture(string mesh)
        {

        }
        /// <summary>
        /// 动作缓冲池
        /// </summary>
        /// <param name="action">动作名称</param>
        public void CacheAction(string action)
        {

        }

        private void _OnDebug(string file)
        {
            //file = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
            //IDllImportAPI.v3dDevice_VMObjMgr_LoadModelSource(this.Device, file, 0, (int)(RenderAPI.V3DPOOL.V3DPOOL_DEFAULT));
            //MidLayer.ITexture.ForceReloadTexture(file);
        }

        void _OnFileDownloadComplete(CSUtility.FileDownload.FileDownInfo fd)
        {
            //var finalPath = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(fd.SavePath);
            var file = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(fd.SavePath);
            file = file.Remove(file.LastIndexOf('.'));

            if (!string.IsNullOrEmpty(fd.MD5))
            {
                // 判断文件的MD5码是否与文件列表存的一致，如果不一致则表示CDN文件没有更新，重新下载
                var preFileName = CSUtility.Support.IFileManager.Instance.Root + "temp/" + Guid.NewGuid().ToString();

                //var fileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(file);
                var md5 = CSUtility.Program.GetMD5HashFromFile(CSUtility.Support.IFileManager.Instance.Root + file);
                if (fd.MD5 != md5)
                {
                    // 重新加入下载
                    CSUtility.FileDownload.FileDownloadManager.Instance.ReDownloadFile(fd, true);

                    return;
                }
                else
                {
                    //System.IO.File.Copy(preFileName + fileName, CSUtility.Support.IFileManager.Instance.GetPathFromFullName(fd.SavePath) + fileName, true);
                    //System.IO.File.Delete(preFileName + fileName);
                    System.IO.File.Delete(fd.SavePath);
                }
            }
            else
            {
                System.IO.File.Delete(fd.SavePath);
            }

            DllImportAPI.v3dGraphics_TextureRes_OnDownloadFinished(mGraphics, file);
            DllImportAPI.v3dGraphics_MeshRes_OnDownloadFinished(mGraphics, file);
            DllImportAPI.v3dGraphics_XndRes_OnDownloadFinished(mGraphics, file);
        }

        private static IntPtr RealMaterialLoader_LoadMaterial(IntPtr name, string tech)
        {
            return IntPtr.Zero;
        }
        private static IntPtr RealMaterialLoader_LoadTechnique(IntPtr techIdPtr)
        {
            unsafe
            {
                Guid techId = *((Guid*)techIdPtr.ToPointer());
                return Engine.Instance.Client.Graphics.MaterialMgr.LoadMaterialTechInstance(ref techId);
            }
        }
        private static IntPtr RealMaterialLoader_LoadMaterialWithGuid(IntPtr matIdPtr)
        {
            unsafe
            {
                Guid matId = *((Guid*)matIdPtr.ToPointer());
                return Engine.Instance.Client.Graphics.MaterialMgr.LoadMaterialInstance(ref matId);
            }
        }
        private static IntPtr MeshResFactory_CreateRes(IntPtr pszFile, int bShareFile)
        {
            unsafe
            {
                var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
                return CSUtility.Support.IFileManager.Instance.NewRes2Memory(file, CSUtility.EFileType.Mesh, false);
            }
        }
        private static IntPtr TextureResFactory_CreateRes(IntPtr pszFile, int bShareFile)
        {
            unsafe
            {
                var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
                var resPtr = CSUtility.Support.IFileManager.Instance.NewRes2Memory(file, CSUtility.EFileType.Texture, false);
                if (resPtr != IntPtr.Zero)
                    return resPtr;
                return IntPtr.Zero;
                //return CSUtility.Support.IFileManager.Instance.NewDownloadRes2Memory(file, CSUtility.EFileType.Texture, false);
            }
        }
        private static IntPtr XndResFactory_CreateRes(IntPtr pszFile, int bShareFile)
        {
            unsafe
            {
                var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
                return CSUtility.Support.IFileManager.Instance.NewRes2Memory(file, CSUtility.EFileType.Xnd, false);
            }
        }

        private static int TextureResFactory_IsDownloadingEvent(IntPtr pszFile)
        {
            if (CSUtility.FileDownload.FileDownloadManager.Instance == null)
                return 0;
            if (CSUtility.Compress.CompressManager.Instance == null)
                return 0;
            var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
            var urlFile = CSUtility.Program.FullPackageUrl + file + ".zip";
            bool isDownloading = CSUtility.FileDownload.FileDownloadManager.Instance.IsFileDownloading(urlFile);
            //bool isUnZiping = CSUtility.Compress.CompressManager.Instance.IsFileUnZiping(CSUtility.Support.IFileManager.Instance.Root + file + ".zip");
            if (isDownloading)// || isUnZiping)
                return 1;

            return 0;
        }
        private static IntPtr TextureResFactory_GetDefaultResource(IntPtr pszFile)
        {
            return System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(CSUtility.Support.IFileConfig.DefaultTextureFile);
        }
        private static void TextureResFactory_RegWaitDownloadResource(IntPtr pRes, IntPtr pszFile)
        {
            if (CSUtility.FileDownload.FileDownloadManager.Instance == null)
                return;
            var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
            var fileUrl = CSUtility.Program.FullPackageUrl + file + ".zip";

            var downloadInfo = CSUtility.FileDownload.FileDownInfo.AddDownFile(fileUrl, CSUtility.Support.IFileManager.Instance.Root + file + ".zip", true, "");
            downloadInfo.UnzipWhenDownloadComplate = true;
            downloadInfo.Proiority = 1;
            downloadInfo.Tag = pRes;
            downloadInfo.UnzipFolder = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(CSUtility.Support.IFileManager.Instance.Root + file + ".zip");
            CSUtility.FileDownload.FileDownloadManager.Instance.AddDownloadFile(downloadInfo, true);
        }

        private static int MeshResFactory_IsDownloadingEvent(IntPtr pszFile)
        {
            if (CSUtility.FileDownload.FileDownloadManager.Instance == null)
                return 0;
            if (CSUtility.Compress.CompressManager.Instance == null)
                return 0;
            var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
            var urlFile = CSUtility.Program.FullPackageUrl + file + ".zip";
            bool isDownloading = CSUtility.FileDownload.FileDownloadManager.Instance.IsFileDownloading(urlFile);
            //bool isUnZiping = CSUtility.Compress.CompressManager.Instance.IsFileUnZiping(CSUtility.Support.IFileManager.Instance.Root + file + ".zip");
            if (isDownloading)// || isUnZiping)
                return 1;

            return 0;
        }
        private static IntPtr MeshResFactory_GetDefaultResource(IntPtr pszFile)
        {
            //return IntPtr.Zero;
            return System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(CSUtility.Support.IFileConfig.DefaultMesh);
        }
        private static void MeshResFactory_RegWaitDownloadResource(IntPtr pRes, IntPtr pszFile)
        {
            if (CSUtility.FileDownload.FileDownloadManager.Instance == null)
                return;
            var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
            var fileUrl = CSUtility.Program.FullPackageUrl + file + ".zip";
            var downloadInfo = CSUtility.FileDownload.FileDownInfo.AddDownFile(fileUrl, CSUtility.Support.IFileManager.Instance.Root + file + ".zip", true, "");
            downloadInfo.UnzipWhenDownloadComplate = true;
            downloadInfo.Proiority = 1;
            downloadInfo.Tag = pRes;
            downloadInfo.UnzipFolder = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(CSUtility.Support.IFileManager.Instance.Root + file + ".zip");
            CSUtility.FileDownload.FileDownloadManager.Instance.AddDownloadFile(downloadInfo, true);
        }

        private static int XndResFactory_IsDownloadingEvent(IntPtr pszFile)
        {
            if (CSUtility.FileDownload.FileDownloadManager.Instance == null)
                return 0;
            if (CSUtility.Compress.CompressManager.Instance == null)
                return 0;
            var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
            var urlFile = CSUtility.Program.FullPackageUrl + file + ".zip";
            bool isDownloading = CSUtility.FileDownload.FileDownloadManager.Instance.IsFileDownloading(urlFile);
            //bool isUnZiping = CSUtility.Compress.CompressManager.Instance.IsFileUnZiping(CSUtility.Support.IFileManager.Instance.Root + file + ".zip");
            if (isDownloading)// || isUnZiping)
                return 1;

            return 0;
        }
        private static IntPtr XndResFactory_GetDefaultResource(IntPtr pszFile)
        {
            return IntPtr.Zero;
        }
        private static void XndResFactory_RegWaitDownloadResource(IntPtr pRes, IntPtr pszFile)
        {
            if (CSUtility.FileDownload.FileDownloadManager.Instance == null)
                return;
            var file = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(pszFile);
            var fileUrl = CSUtility.Program.FullPackageUrl + file + ".zip";

            var downloadInfo = CSUtility.FileDownload.FileDownInfo.AddDownFile(fileUrl, CSUtility.Support.IFileManager.Instance.Root + file + ".zip", true, "");
            downloadInfo.UnzipWhenDownloadComplate = true;
            downloadInfo.Proiority = 1;
            downloadInfo.Tag = pRes;
            downloadInfo.UnzipFolder = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(CSUtility.Support.IFileManager.Instance.Root + file + ".zip");
            CSUtility.FileDownload.FileDownloadManager.Instance.AddDownloadFile(downloadInfo, true);
        }
        /// <summary>
        /// 删除对象，释放指针内存
        /// </summary>
        public void Cleanup()
        {
            if (mMaterialMgr != null)
            {
                mMaterialMgr.Cleanup();
                mMaterialMgr = null;
            }

            unsafe
            {
                DllImportAPI.v3dGraphics_Cleanup(mGraphics);
                DllImportAPI.v3dGraphics_Delete(mGraphics);
                mGraphics = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 是否重置
        /// </summary>
        /// <returns>返回false</returns>
        public bool Reset()
        {
            return false;
        }
        /// <summary>
        /// 复制图形数据
        /// </summary>
        /// <param name="pTexture">纹理指针</param>
        /// <param name="vpPosX">X轴坐标</param>
        /// <param name="vpPosY">Y轴坐标</param>
        /// <param name="vpSizeX">X轴方向的大小</param>
        /// <param name="vpSizeY">Y轴方向的大小</param>
        public void CopyFrom(IntPtr pTexture, UInt32 vpPosX, UInt32 vpPosY, UInt32 vpSizeX, UInt32 vpSizeY)
        {
            unsafe
            {
                SlimDX.Rect rect = new SlimDX.Rect(vpPosX, vpPosY, vpSizeX, vpSizeY);
                DllImportAPI.v3dGraphics_CopyFrom(mGraphics, pTexture, &rect);
            }
        }
        /// <summary>
        /// 删除所有的shader
        /// </summary>
        public void DumpAllShader()
        {
            unsafe
            {
                DllImportAPI.v3dGraphics_DumpAllShader(mGraphics);
            }
        }

        //public IntPtr GetDeviceIntPtr()
        //{
        //    return (IntPtr)mDevice;
        //}
        /// <summary>
        /// 获取当前摄像机对象的指针
        /// </summary>
        /// <returns>返回当前摄像机对象的指针</returns>
		public IntPtr GetCameraIntPtr()
		{
            unsafe
            {
                return DllImportAPI.v3dDevice_GetCamera(Device);
            }
		}
        /// <summary>
        /// 获取D3D设备指针
        /// </summary>
        /// <returns>返回D3D设备指针</returns>
		public IntPtr GetD3DDeviceIntPtr()
		{
            unsafe
            {
                return DllImportAPI.v3dDevice_GetD3DDevice(Device);
            }
		}
        /// <summary>
        /// 只读属性，纹理的总大小
        /// </summary>
        public UInt32 TextureTotalSize
        {
            get { return CCore.DllImportAPI.v3dDevice_TextureManager_GetTotalSize(Device); }
        }
        /// <summary>
        /// 只读属性，虚拟内存的总大小
        /// </summary>
        public UInt32 VMObjTotalSize
        {
            get { return CCore.DllImportAPI.v3dDevice_VMObjectManager_GetTotalSize(Device); }
        }
        /// <summary>
        /// 只读属性，RAM的总大小
        /// </summary>
        public UInt32 RAMObjTotalSize
        {
            get { return CCore.DllImportAPI.v3dDevice_RAMObjectManager_GetTotalSize(Device); }
        }
		/// <summary>
        /// 设置绘制成员的回调函数
        /// </summary>
        /// <param name="call">绘制时调用的方法</param>
        public void SetDrawSubsetCallBack(CCore.DllImportAPI.Delegate_FOnDrawSubset call)
        {
            CCore.DllImportAPI.v3dStagedObject_SetOnDrawSubsetCallBack(call);
        }
        /// <summary>
        /// 获取调试时纹理的数量
        /// </summary>
        /// <returns>返回调试时纹理的数量</returns>
        public int GetDebugTextureCount()
        {
            unsafe
            {
                return DllImportAPI.v3dDevice_GetDebugTextureCount(Device);
            }
        }
        /// <summary>
        /// 获取调试时的纹理参数
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="tex">纹理对象</param>
        /// <param name="name">纹理名称</param>
        /// <param name="width">纹理的宽</param>
        /// <param name="height">纹理的高</param>
        /// <param name="gray">是否有纹理灰度</param>
        public void GetDebugTextureParams(int index, out CCore.Graphics.Texture tex, out string name, out int width, out int height, out bool gray )
        {
            unsafe
            {
                tex = new CCore.Graphics.Texture();
                tex.__SetTexture(DllImportAPI.v3dDevice_GetDebugTexturePtr(Device, index));
                name = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.v3dDevice_GetDebugTextureName(Device, index));
                width = DllImportAPI.v3dDevice_GetDebugTextureWidth(Device, index);
                height = DllImportAPI.v3dDevice_GetDebugTextureHeight(Device, index);
                gray = DllImportAPI.v3dDevice_GetDebugTextureGray(Device, index)>0 ? true : false; 
            }
        }

        #region 显卡操作接口
        /// <summary>
        /// 设置显示接口
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="w">宽度</param>
        /// <param name="h">高度</param>
        /// <param name="minz">最小的Z值</param>
        /// <param name="maxz">最大的Z值</param>
        public void SetViewPort(int x, int y, int w, int h, float minz, float maxz)
        {
            DllImportAPI.v3dGraphics_SetViewPort(this.mGraphics, x, y, w, h, minz, maxz);
        }
        #endregion
    }
}
