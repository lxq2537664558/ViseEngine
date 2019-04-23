using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 视口类
    /// </summary>
    public class ViewPort
    {
        /// <summary>
        /// 视口的指针
        /// </summary>
        protected IntPtr mViewPortPtr; // RenderAPI::V3DVIEWPORT9*
        /// <summary>
        /// 只读属性，视口指针
        /// </summary>
        public IntPtr ViewPortPtr
        {
            get { return mViewPortPtr; }
        }
        /// <summary>
        /// 视口的X值
        /// </summary>
        public UInt32 X
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DVIEWPORT9_GetX(mViewPortPtr);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DVIEWPORT9_SetX(mViewPortPtr, value);
                }
            }
        }
        /// <summary>
        /// 视口的Y值
        /// </summary>
        public UInt32 Y
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DVIEWPORT9_GetY(mViewPortPtr);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DVIEWPORT9_SetY(mViewPortPtr, value);
                }
            }
        }
        /// <summary>
        /// 视口的宽
        /// </summary>
        public UInt32 Width
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DVIEWPORT9_GetWidth(mViewPortPtr);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DVIEWPORT9_SetWidth(mViewPortPtr, value);
                }
            }
        }
        /// <summary>
        /// 视口的高
        /// </summary>
        public UInt32 Height
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DVIEWPORT9_GetHeight(mViewPortPtr);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DVIEWPORT9_SetHeight(mViewPortPtr, value);
                }
            }
        }
        /// <summary>
        /// 视口的最小Z值
        /// </summary>
        public float MinZ
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DVIEWPORT9_GetMinZ(mViewPortPtr);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DVIEWPORT9_SetMinZ(mViewPortPtr, value);
                }
            }
        }
        /// <summary>
        /// 视口的最大Z值
        /// </summary>
        public float MaxZ
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DVIEWPORT9_GetMaxZ(mViewPortPtr);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DVIEWPORT9_SetMaxZ(mViewPortPtr, value);
                }
            }
        }
        /// <summary>
        /// 视口的构造函数
        /// </summary>
        public ViewPort()
        {
            unsafe
            {
                mViewPortPtr = DllImportAPI.V3DVIEWPORT9_New();
                DllImportAPI.V3DVIEWPORT9_SetX(mViewPortPtr, 0);
                DllImportAPI.V3DVIEWPORT9_SetY(mViewPortPtr, 0);
                DllImportAPI.V3DVIEWPORT9_SetMinZ(mViewPortPtr, 0);
                DllImportAPI.V3DVIEWPORT9_SetMaxZ(mViewPortPtr, 0);
            }
        }
        /// <summary>
        /// 析构函数，清除对象
        /// </summary>
        ~ViewPort()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放该实例地址
        /// </summary>
        public void Cleanup()
        {
            unsafe
            {
                if (mViewPortPtr != IntPtr.Zero)
                {
                    DllImportAPI.V3DVIEWPORT9_Delete(mViewPortPtr);
                    mViewPortPtr = IntPtr.Zero;
                }
            }
        }
    }
    /// <summary>
    /// 声明画面对象改变的委托事件
    /// </summary>
    /// <param name="sender">改变的控件属性</param>
    /// <param name="e">改变的事件</param>
    public delegate void Delegate_IViewTargetChanged(System.Object sender, System.EventArgs e);
    // 平台相关，整个画面的输出对象（窗口、控件等）
    /// <summary>
    /// 平台相关，整个画面的输出对象（窗口、控件等）
    /// </summary>
    public sealed class ViewTarget
    {
        /// <summary>
        /// MRT的缩放值，默认为1
        /// </summary>
        public float MRTScale = 1.0F;
#if WIN
        System.Windows.Forms.Control mControl;
        /// <summary>
        /// 画面输出对象的句柄地址
        /// </summary>
        public IntPtr Handle => (IntPtr)(mControl?.Handle);
        /// <summary>
        /// 画面的宽
        /// </summary>
        public int Width 
        { 
            get { return (int)(mControl?.Width); } 
            set 
            {
                if(mControl != null)
                    mControl.Width = value;
            }
        }
        /// <summary>
        /// 画面的高
        /// </summary>
        public int Height
        { 
            get { return (int)(mControl?.Height); }
            set
            {
                if(mControl != null)
                    mControl.Height = value;
            }
        }
        /// <summary>
        /// 客户端的宽
        /// </summary>
        public int ClientWidth
        {
            get { return (int)(mControl?.ClientSize.Width); }
            set
            {
                if(mControl != null)
                    mControl.ClientSize = new System.Drawing.Size(value, ClientHeight);
            }
        }
        /// <summary>
        /// 客户端的高
        /// </summary>
        public int ClientHeight 
        { 
            get { return (int)(mControl?.ClientSize.Height); }
            set
            {
                if(mControl != null)
                    mControl.ClientSize = new System.Drawing.Size(ClientWidth, value);
            }
        }
        /// <summary>
        /// 定义画面对象改变时调用的委托事件
        /// </summary>
        public event Delegate_IViewTargetChanged SizeChanged;
        /// <summary>
        /// 设置画面的输出对象的控件
        /// </summary>
        /// <param name="ctrl">控件实例</param>
        public void SetControl(System.Windows.Forms.Control ctrl)
        {
            if(mControl != null)
                mControl.SizeChanged -= OnSizeChanged;
            mControl = ctrl;
            mControl.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, System.EventArgs e)
        {
            SizeChanged?.Invoke(sender, e);
        }
#else
        
        public static IntPtr Android_ANWinFromSurface(IntPtr env, IntPtr surface)
        {
            return DllImportAPI.Android_ANWinFromSurface(env, surface);
        }
        public IntPtr Handle { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ClientWidth { get; set; }
        public int ClientHeight { get; set; }

        public event Delegate_IViewTargetChanged SizeChanged;
        private void OnSizeChanged(object sender, System.EventArgs e)
        {
            SizeChanged?.Invoke(sender, e);
        }
#endif
    }
    /// <summary>
    /// 视图的初始化类
    /// </summary>
    public class ViewInit
    {
        /// <summary>
        /// 视图控件
        /// </summary>
		public ViewTarget ViewWnd;
        /// <summary>
        /// 缓冲格式
        /// </summary>
		public BufferFormat	Format;
        /// <summary>
        /// 延迟渲染的缓冲方式
        /// </summary>
		public BufferFormat	DSFormat;
        /// <summary>
        /// 视图初始化
        /// </summary>
        public ViewInit()
        {
            Format = BufferFormat.D3DFMT_UNKNOWN;
            DSFormat = BufferFormat.D3DFMT_UNKNOWN;
            ViewWnd = new ViewTarget();
        }
    }
    /// <summary>
    /// 声明视图尺寸改变时调用的委托事件
    /// </summary>
    /// <param name="view">改变的视图</param>
    /// <param name="width">视图的宽</param>
    /// <param name="height">视图的高</param>
    public delegate void FViewSizeChanged(View view, int width, int height);
    /// <summary>
    /// 视图类
    /// </summary>
    public class View
    {
        /// <summary>
        /// 视图的对象指针
        /// </summary>
        protected IntPtr mViewPtr; // RenderAPI::IRenderView*
        /// <summary>
        /// 只读属性，视图的内存地址
        /// </summary>
        public IntPtr ViewPtr
        {
            get{ return mViewPtr; }
        }
        /// <summary>
        /// 视图的输出对象（窗口、控件等）
        /// </summary>
        protected ViewTarget mViewWnd;
        /// <summary>
        /// 只读属性，视图的输出对象
        /// </summary>
        public ViewTarget ViewWnd
        {
            get { return mViewWnd; }
        }
        /// <summary>
        /// 展示的结果
        /// </summary>
        protected Int64 mPresentResult;
        /// <summary>
        /// 只读属性，展现结果
        /// </summary>
        public Int64 PresentResult
        {
            get { return mPresentResult; }
        }
        /// <summary>
        /// 视图的X值
        /// </summary>
        public UInt32 X
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.IRenderView_GetViewPortX(mViewPtr);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.IRenderView_SetViewPortX(mViewPtr, value);
                }
            }
        }
        /// <summary>
        /// 视图的Y值
        /// </summary>
        public UInt32 Y
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.IRenderView_GetViewPortY(mViewPtr);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.IRenderView_SetViewPortY(mViewPtr, value);
                }
            }
        }
        /// <summary>
        /// 只读属性，视图的宽
        /// </summary>
        public int Width
        {
            get
            {
                unsafe
                {
                    return (int)DllImportAPI.IRenderView_GetViewPortWidth(mViewPtr);
                }
            }
        }
        /// <summary>
        /// 只读属性，视图的高
        /// </summary>
        public int Height
        {
            get
            {
                unsafe
                {
                    return (int)DllImportAPI.IRenderView_GetViewPortHeight(mViewPtr);
                }
            }
        }
        /// <summary>
        /// 改变视图尺寸
        /// </summary>
        /// <param name="sender">控件属性</param>
        /// <param name="e">改变的事件</param>
        protected void ViewWndSizeChanged(System.Object sender, System.EventArgs e)
        {
            SetViewSize(mViewWnd.ClientWidth, mViewWnd.ClientHeight);
        }
        /// <summary>
        /// 定义改变视图尺寸时调用的委托事件
        /// </summary>
        public event FViewSizeChanged ViewSizeChanged;
        /// <summary>
        /// 设置视图的尺寸
        /// </summary>
        /// <param name="width">视图的宽</param>
        /// <param name="height">视图的高</param>
        public void SetViewSize(int width, int height)
        {
            unsafe
            {

                DllImportAPI.IRenderView_SetViewPortWidth(mViewPtr, (UInt32)width);
                DllImportAPI.IRenderView_SetViewPortHeight(mViewPtr, (UInt32)height);

                DllImportAPI.IRenderView_Resize(mViewPtr, (UInt32)width, (UInt32)height);
                ViewSizeChanged(this, width, height);
            }

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public View()
        {

        }
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
        ~View()
        {
            Cleanup();
        }
        /// <summary>
        /// 容器的尺寸
        /// </summary>
        public CSUtility.Support.Size mBorderSize = new CSUtility.Support.Size(16,38);
        /// <summary>
        /// 视图的初始化
        /// </summary>
        /// <param name="_init">视图初始化的实例</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(ViewInit _init)
        {
            Cleanup();

            mViewWnd = _init.ViewWnd;

            UInt32 width = (UInt32)((float)mViewWnd.ClientWidth);
            if (width == 0)
                width = 1;
            UInt32 height = (UInt32)(mViewWnd.ClientHeight);
            if (height == 0)
                height = 1;
            mBorderSize.Width = mViewWnd.Width - mViewWnd.ClientWidth;
            mBorderSize.Height = mViewWnd.Height - mViewWnd.ClientHeight;

            if (Engine.Instance.Client?.Graphics == null)
                return false;
            mViewPtr = DllImportAPI.v3dDevice_CreateRenderView(Engine.Instance.Client.Graphics.Device, mViewWnd.Handle,
                                                             width,
                                                             height,
                                                             _init.Format,
                                                             _init.DSFormat);

            if(mViewPtr == IntPtr.Zero)
                return false;

            mViewWnd.SizeChanged += ViewWndSizeChanged;
            return true;
        }
        /// <summary>
        /// 释放当前实例的内存
        /// </summary>
        public void Cleanup()
        {
            if(mViewWnd != null)
                mViewWnd.SizeChanged -= ViewWndSizeChanged;
            unsafe
            {
                if (mViewPtr != IntPtr.Zero)
                {
                    DllImportAPI.IRenderView_Release(mViewPtr);
                    mViewPtr = IntPtr.Zero;
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
                DllImportAPI.IRenderView_Begin(mViewPtr);
            }
        }
        /// <summary>
        /// 清理视图
        /// </summary>
        /// <param name="Flags">视图标志</param>
        /// <param name="Color">视图的颜色</param>
        /// <param name="Z">视图的Z值</param>
        /// <param name="Stencil">视图模板</param>
        public void Clear(UInt32 Flags, UInt32 Color, float Z, UInt32 Stencil)
        {
            unsafe
            {
                DllImportAPI.IRenderView_Clear(mViewPtr, Flags, Color, Z, Stencil);
            }
        }
        /// <summary>
        /// 结束绘制
        /// </summary>
        /// <param name="bUpdate">是否进行更新</param>
        public void EndDraw(bool bUpdate)
        {
            unsafe
            {
                DllImportAPI.IRenderView_End(mViewPtr);
                mPresentResult = DllImportAPI.IRenderView_GetPresent(mViewPtr);
            }
        }



    }
}
