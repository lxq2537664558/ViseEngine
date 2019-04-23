using System;
using System.Collections.Generic;

namespace CCore.Graphics
{
    /// <summary>
    /// 渲染模式的枚举
    /// </summary>
    public enum enRenderMode : int
	{
		RM_Wireframe = 0,
		RM_Albedo,
		RM_Lighting,
		RM_Shading,	
	}
    /// <summary>
    /// 检测图像边缘的模式
    /// </summary>
    public enum enEdgeDetectMode
    {
        Lerp = 0,
        Add,
    }
    /// <summary>
    /// 声明渲染之后调用的委托事件
    /// </summary>
    /// <param name="env">渲染环境</param>
    public delegate void FAfterRender2View(REnviroment env);
    /// <summary>
    /// 声明在复制纹理之前调用的委托事件
    /// </summary>
    /// <param name="env">所处的渲染环境</param>
    public delegate void FBeforeCopyTexture(REnviroment env);
    /// <summary>
    /// 渲染环境的初始化类
    /// </summary>
    public class REnviromentInit
    {
        /// <summary>
        /// 视图初始化
        /// </summary>
        public ViewInit ViewInit;
        /// <summary>
        /// 是否为主场景，默认为false
        /// </summary>
        public bool bMainScene = false;
        /// <summary>
        /// 是否使用Z值,默认为true
        /// </summary>
        public bool bUseIntZ = true;
        /// <summary>
        /// 是否使用RT，默认为false
        /// </summary>
        public bool bUseRT = false;
        /// <summary>
        /// 渲染场景的宽度
        /// </summary>
        public int Width;
        /// <summary>
        /// 渲染场景的高度
        /// </summary>
        public int Height;
        /// <summary>
        /// 视角范围
        /// </summary>
        public float Fov           = (float)(28 * System.Math.PI / 180.0f);
        /// <summary>
        /// 最近的Z值
        /// </summary>
        public float ZNear        = 0.1f;
        /// <summary>
        /// 最远的Z值
        /// </summary>
        public float ZFar          = 1000.0f;

    }
    /// <summary>
    /// 渲染环境类
    /// </summary>
    public class REnviroment
    {
        /// <summary>
        /// 是否为主场景，默认为false
        /// </summary>
        public bool bMainScene = false;
        /// <summary>
        /// X的挂接
        /// </summary>
        public static float TileInheritX;
        /// <summary>
        /// Z的挂接
        /// </summary>
        public static float TileInheritZ;
        /// <summary>
        /// 是否使用Z值
        /// </summary>
        public bool bUseIntZ;
        /// <summary>
        /// 是否使用RT
        /// </summary>
        public bool bUseRT;
        /// <summary>
        /// 是否使用HDR
        /// </summary>
        public bool bUseHDR = true;
        /// <summary>
        /// 渲染场景的宽
        /// </summary>
        public int mWidth;
        /// <summary>
        /// 渲染场景的高
        /// </summary>
        public int mHeight;
        private View mView;
        /// <summary>
        /// 只读属性，渲染视图
        /// </summary>
        public View View
        {
            get { return mView; }
        }
        /// <summary>
        /// 摄像机实例
        /// </summary>
        protected CCore.Camera.CameraObject mCamera;
        /// <summary>
        /// 只读属性，当前摄像机对象
        /// </summary>
        public CCore.Camera.CameraObject Camera
        {
            get { return mCamera; }
        }

        private IntPtr mDSRenderEnv; // vSimulation.vDSRenderEnv
        /// <summary>
        /// 只读属性，渲染环境
        /// </summary>
        public IntPtr DSRenderEnv
        {
            get { return mDSRenderEnv; }
        }
        /// <summary>
        /// 定义渲染之后调用的委托事件
        /// </summary>
        public event FAfterRender2View AfterRender2View;
        /// <summary>
        /// 定义复制纹理之前调用的委托事件
        /// </summary>
        public event FBeforeCopyTexture BeforeCopyTexture;
        private static CSUtility.Performance.PerfCounter CounterEnvRender = new CSUtility.Performance.PerfCounter("Env.Render");
        //private static IPerfCounter CounterEnvTick;// = gcnew IPerfCounter("Env.Tick");
        private static CSUtility.Performance.PerfCounter CounterEnvAfterR2V = new CSUtility.Performance.PerfCounter("Env.AfterR2V");
        private static CSUtility.Performance.PerfCounter CounterEnvDS2V = new CSUtility.Performance.PerfCounter("Env.DS2V");
        /// <summary>
        /// 渲染模式
        /// </summary>
        public enRenderMode RenderMode  
        {
            get
            {
                unsafe
                {
                    return (enRenderMode)(DllImportAPI.vDSRenderEnv_GetRenderMode(mDSRenderEnv));
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.vDSRenderEnv_SetRenderMode(mDSRenderEnv, (int)value);
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public REnviroment()
        {
        }
        /// <summary>
        /// 析构函数，释放对象内存
        /// </summary>
        ~REnviroment()
        {
            Cleanup();
        }
        /// <summary>
        /// 延迟渲染绘制回调
        /// </summary>
        public int DSDrawCall
        {
            get
            {
                return CCore.DllImportAPI.vDSRenderEnv_GetDSDrawCall(mDSRenderEnv);
            }
        }
        /// <summary>
        /// 延迟渲染绘制触发器
        /// </summary>
        public int DSDrawTri
        {
            get
            {
                return CCore.DllImportAPI.vDSRenderEnv_GetDSDrawTri(mDSRenderEnv);
            }
        }
        /// <summary>
        /// 前向渲染绘制回调
        /// </summary>
        public int FSDrawCall
        {
            get
            {
                return CCore.DllImportAPI.vDSRenderEnv_GetFSDrawCall(mDSRenderEnv);
            }
        }
        /// <summary>
        /// 前向渲染绘制触发器
        /// </summary>
        public int FSDrawTri
        {
            get
            {
                return CCore.DllImportAPI.vDSRenderEnv_GetFSDrawTri(mDSRenderEnv);
            }
        }
        /// <summary>
        /// 渲染环境的初始化
        /// </summary>
        /// <param name="_init">渲染环境初始化的实例</param>
        /// <param name="view">视图对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(REnviromentInit _init, View view)
        {
            unsafe
            {
                Cleanup();

                if(_init.ViewInit != null)
                {
                    //mView = new View();
                    //if (mView.Initialize(_init.ViewInit) == false)
                    //    return false;
                    //mView = view;
                    //mView.ViewSizeChanged += ViewSizeChanged;
                    SetView(view);

                    var scale = _init.ViewInit.ViewWnd.MRTScale;
                    mWidth = (int)(((float)mView.Width) * scale);
                    mHeight = (int)(((float)mView.Height) * scale);

                    DllImportAPI.v3dDevice_SetViewPort(Engine.Instance.Client.Graphics.Device, mView.ViewPtr);
                }
                else
                {
                    var scale = _init.ViewInit.ViewWnd.MRTScale;
                    mWidth = (int)(((float)_init.Width) * scale);
                    mHeight = (int)(((float)_init.Height) * scale);
                }

                mCamera = new CCore.Camera.CameraObject();
                var eyeInit = new CCore.Camera.CameraInit();
                eyeInit.Width = mWidth;
                eyeInit.Height = mHeight;
                eyeInit.Fov = _init.Fov;
                eyeInit.ZNear = _init.ZNear;
                eyeInit.ZFar = _init.ZFar;
                mCamera.Initialize(eyeInit);

                bUseIntZ = _init.bUseIntZ;
                bUseRT = _init.bUseRT;

                mDSRenderEnv = DllImportAPI.vDSRenderEnv_New(Engine.Instance.Client.Graphics.Device);
                if (DllImportAPI.vDSRenderEnv_Initialize(mDSRenderEnv, mWidth, mHeight, bUseIntZ) == 0)
                    return false;
                bMainScene = _init.bMainScene;
                DllImportAPI.vDSRenderEnv_SetMainSceneRenderEnv(mDSRenderEnv, _init.bMainScene);
                
                return true;
            }
        }
        /// <summary>
        /// 设置视图对象
        /// </summary>
        /// <param name="view">视图对象</param>
        public void SetView(View view)
        {
            if (mView != null)
            {
                mView.Cleanup();
            }
            mView = view;
            if (mView!=null)
            {
                mView.ViewSizeChanged += ViewSizeChanged;
            }
        }
        /// <summary>
        /// 清空实例内容，并将其指针置空
        /// </summary>
        public void Cleanup()
        {
            if (mView != null)
            {
                mView.ViewSizeChanged -= ViewSizeChanged;
                mView.Cleanup();
                mView = null;
            }
            CleanAllCommits();
            unsafe
            {
                if(mDSRenderEnv != IntPtr.Zero)
                {
                    DllImportAPI.vDSRenderEnv_Release(mDSRenderEnv);
                    mDSRenderEnv = IntPtr.Zero;
                }
            }
            if (mCamera != null)
            {
                mCamera.Cleanup();
                mCamera = null;
            }
        }
        /// <summary>
        /// 删除可视化的实例
        /// </summary>
        /// <param name="layer">删除的对象所在的层</param>
        /// <param name="vis">需要删除的实例对象</param>
        public void RemoveVisual(RLayer layer, CCore.Component.Visual vis)
        {
        }
        /// <summary>
        /// 清除所有提交的渲染环境实例
        /// </summary>
        public void CleanAllCommits()
        {
            unsafe
            {
                if(mDSRenderEnv != IntPtr.Zero)
                    DllImportAPI.vDSRenderEnv_ClearAllCommits(mDSRenderEnv);
            }
        }
        /// <summary>
        /// 交换渲染管道
        /// </summary>
        public void SwapPipe()
        {
            unsafe
            {
                if (mDSRenderEnv != IntPtr.Zero)
                    DllImportAPI.vDSRenderEnv_SwapPipe(mDSRenderEnv);
            }
            mCamera.CopyCommitCamera();
        }
        /// <summary>
        /// 清除所有的提交内容
        /// </summary>
        public void ClearAllDrawingCommits()
        {
            unsafe
            {
                if (mDSRenderEnv != IntPtr.Zero)
                    DllImportAPI.vDSRenderEnv_ClearAllDrawingCommits(mDSRenderEnv);
            }
        }

        /// <summary>
        /// 每帧调用
        /// </summary>
        public void	Tick()
        {
        }
        /// <summary>
        /// 渲染设置转换成文件进行记录
        /// </summary>
        /// <param name="fileName">转换成文件的文件名</param>
        public void RenderToFile(string fileName)
        {
            unsafe
            {
                if (mView == null)
                    return;

                DllImportAPI.v3dDevice_SetCamera(Engine.Instance.Client.Graphics.Device, mCamera.CommitCamera);

                Engine.Instance.Client.Graphics.SetViewPort(0, 0, mWidth, mHeight, 0.0F, 1.0F);
                DllImportAPI.vDSRenderEnv_DrawAll(mDSRenderEnv);

                DllImportAPI.vDSRenderEnv_DrawPostProcess(mDSRenderEnv);

                DllImportAPI.vDSRenderEnv_MRT_BeginFinalDrawing(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);
                if (BeforeCopyTexture != null)
                    BeforeCopyTexture(this);
                DllImportAPI.vDSRenderEnv_MRT_EndFinalDrawing(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);

                mView.BeginDraw();
                //mView.Clear((UInt32)(RenderAPI.V3DCLEAR_STENCIL | RenderAPI.V3DCLEAR_ZBUFFER), (UInt32)0x00000000, 1.0f, (UInt32)0);

                var finalTex = DllImportAPI.vDSRenderEnv_GetFinalTexture(mDSRenderEnv);
                Engine.Instance.Client.Graphics.CopyFrom(finalTex, (UInt32)0, (UInt32)0, (UInt32)mWidth, (UInt32)mHeight);

                if (AfterRender2View != null)
                    AfterRender2View(this);

                if (CCore.World.World.ShowTileInheritBoundingBox)
                {
                    var tileScene = (CCore.Scene.TileScene.TileScene)(Engine.Instance.Client.MainWorld.SceneGraph);
                    if (tileScene != null)
                    {
                        tileScene.RenderTileInheritBoundingBox(TileInheritX, TileInheritZ);
                    }
                }

                mView.EndDraw(true);

                DllImportAPI.vDSRenderEnv_SaveFinalTexture(mDSRenderEnv, fileName);
                //IDllImportAPI.vDSRenderEnv_SaveAlbedoTexture(mDSRenderEnv, fileName);   
            }
        }
        /// <summary>
        /// 更新调试纹理
        /// </summary>
        public void UpdateDebugTextures()
        {
            unsafe
            {
                if (mDSRenderEnv != IntPtr.Zero)
                    DllImportAPI.vDSRenderEnv_UpdateDebugTextures(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);
            }
        }

        /// <summary>
        /// 对UI进行渲染
        /// </summary>
        public void RenderUI()
        {
            if (AfterRender2View != null)
                AfterRender2View(this);
        }

        private int mHitLagTime = 500;
        //bool mNeedReadHitProxy = false;
        //bool mReadingHitProxy = false;

        static CSUtility.Performance.PerfCounter EnvRenderCounter = new CSUtility.Performance.PerfCounter("REnv.Render");
        static CSUtility.Performance.PerfCounter EnvRDrwAllCounter = new CSUtility.Performance.PerfCounter("REnv.Render.DrawAll");
        static CSUtility.Performance.PerfCounter EnvRPostCounter = new CSUtility.Performance.PerfCounter("REnv.Render.DrawPost");
        static CSUtility.Performance.PerfCounter EnvR2ViewCounter = new CSUtility.Performance.PerfCounter("REnv.Render.R2View");
        static CSUtility.Performance.PerfCounter EnvBeforeR2ViewCounter = new CSUtility.Performance.PerfCounter("REnv.Render.BeforeR2View");
        static CSUtility.Performance.PerfCounter EnvAfterR2ViewCounter = new CSUtility.Performance.PerfCounter("REnv.Render.AfterR2View");
        static CSUtility.Performance.PerfCounter EnvHitProxyCounter = new CSUtility.Performance.PerfCounter("REnv.Render.HitProxy");
        /// <summary>
        /// 是否使用后处理，默认为true
        /// </summary>
        public bool DoPostProcess = true;
        /// <summary>
        /// 是否设置点击代理，默认为true
        /// </summary>
        public bool DoHitProxy = true;
        /// <summary>
        /// 渲染实例对象
        /// </summary>
        public void Render()
        {
            EnvRenderCounter.Begin();
            unsafe
            {
		        if( mView!=null )
                {
                    DllImportAPI.v3dDevice_SetCamera(Engine.Instance.Client.Graphics.Device, mCamera.CommitCamera);
                    
                    Engine.Instance.Client.Graphics.SetViewPort(0, 0, mWidth, mHeight, 0.0F, 1.0F);

                    EnvRDrwAllCounter.Begin();
                    DllImportAPI.vDSRenderEnv_DrawAll(mDSRenderEnv);
                    EnvRDrwAllCounter.End();

                    EnvRPostCounter.Begin();
                    if (DoPostProcess)
                    {
                        DllImportAPI.vDSRenderEnv_DrawPostProcess(mDSRenderEnv);
                    }
                    EnvRPostCounter.End();
                    
                    EnvBeforeR2ViewCounter.Begin();
                    DllImportAPI.vDSRenderEnv_MRT_BeginFinalDrawing(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);
                    if (BeforeCopyTexture != null)
                        BeforeCopyTexture(this);
                    DllImportAPI.vDSRenderEnv_MRT_EndFinalDrawing(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);
                    EnvBeforeR2ViewCounter.End();

                    if (bUseRT == false)
                    {
                        mView.BeginDraw();
                        // 每次从FINAL TEXTURE拷贝，这里没必要Clear()
                        //mView.Clear((UInt32)(RenderAPI.V3DCLEAR_TARGET | RenderAPI.V3DCLEAR_STENCIL | RenderAPI.V3DCLEAR_ZBUFFER), (UInt32)0x00000000, 1.0f, (UInt32)0);

                        EnvR2ViewCounter.Begin();
                        Engine.Instance.Client.Graphics.SetViewPort(0, 0, mView.Width, mView.Height, 0.0F, 1.0F);
                        var finalTex = DllImportAPI.vDSRenderEnv_GetFinalTexture(mDSRenderEnv);
                        Engine.Instance.Client.Graphics.CopyFrom(finalTex, (UInt32)0, (UInt32)0, (UInt32)mView.Width, (UInt32)mView.Height);
                        EnvR2ViewCounter.End();

                        EnvAfterR2ViewCounter.Begin();
                        if (AfterRender2View != null)
                            AfterRender2View(this);
                        EnvAfterR2ViewCounter.End();

                        //if (CCore.World.World.ShowTileInheritBoundingBox)
                        //{
                        //    var tileScene = (CCore.Scene.TileScene.TileScene)(Engine.Instance.Client.MainWorld.SceneGraph);
                        //    if (tileScene != null)
                        //    {
                        //        tileScene.RenderTileInheritBoundingBox(TileInheritX, TileInheritZ);
                        //    }
                        //}

                        // DEBUG: 画Frustum
                        //IDllImportAPI.v3dCamera_DrawCameraFrustum(mCamera.CommitCamera);                    

                        //if (bMainScene == true)
                        //    IDllImportAPI.vNVPerf_Render();

                        if (mView != null)
                            mView.EndDraw(true);
                    }
                }
                else
                {
                    DllImportAPI.v3dDevice_SetCamera(Engine.Instance.Client.Graphics.Device, mCamera.CommitCamera);
                    Engine.Instance.Client.Graphics.SetViewPort(0, 0, mWidth, mHeight, 0.0F, 1.0F);
                    DllImportAPI.vDSRenderEnv_DrawAll(mDSRenderEnv);

                    DllImportAPI.vDSRenderEnv_DrawPostProcess(mDSRenderEnv);

                }

                EnvHitProxyCounter.Begin();
                if(DoHitProxy)
                {
                    var elapsedMillisecond = CCore.Engine.Instance.GetElapsedMillisecond();
                    mHitLagTime -= (int)elapsedMillisecond;
                    if (mHitLagTime < 0)
                    {
                        mHitLagTime = 500;
                        DrawHitProxy(false);
                        ReadHitProxyData();
                    }
                }
                EnvHitProxyCounter.End();
            }
            EnvRenderCounter.End();
        }
        /// <summary>
        /// 只渲染UI
        /// </summary>
        public void RenderOnlyUI()
        {
            Render();
            //mView.BeginDraw();
            //mView.Clear((UInt32)(RenderAPI.V3DCLEAR_STENCIL | RenderAPI.V3DCLEAR_TARGET | RenderAPI.V3DCLEAR_ZBUFFER), (UInt32)0xFF0000FF, 1.0f, (UInt32)0);

            ////DllImportAPI.vDSRenderEnv_MRT_BeginFinalDrawing(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);
            ////if (BeforeCopyTexture != null)
            ////    BeforeCopyTexture(this);
            ////DllImportAPI.vDSRenderEnv_MRT_EndFinalDrawing(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);
            //if (AfterRender2View != null)
            //    AfterRender2View(this);

            //mView.EndDraw(true);
        }
        /// <summary>
        /// 渲染UI的同时设置缩放属性
        /// </summary>
        /// <param name="scaleX">X方向缩放值</param>
        /// <param name="scaleY">Y方向缩放值</param>
        /// <param name="scaleCenter">缩放的中心点</param>
        public void RenderUIWithScale(float scaleX, float scaleY, CSUtility.Support.Point scaleCenter)
        {
            unsafe
            {
		        CounterEnvRender.Begin();
		        if( mView==null )
			        return;

                DllImportAPI.v3dDevice_SetCamera(Engine.Instance.Client.Graphics.Device, mCamera.CommitCamera);
                Engine.Instance.Client.Graphics.SetViewPort(0, 0, mWidth, mHeight, 0.0F, 1.0F);
                DllImportAPI.vDSRenderEnv_DrawAll(mDSRenderEnv);

		        CounterEnvAfterR2V.Begin();
                DllImportAPI.vDSRenderEnv_MRT_BeginFinalDrawing(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);
                DllImportAPI.v3dDevice_RenderDevice_Clear(Engine.Instance.Client.Graphics.Device, (UInt32)0,
                    (UInt32)(RenderAPI.V3DCLEAR_TARGET | RenderAPI.V3DCLEAR_ZBUFFER | RenderAPI.V3DCLEAR_STENCIL), (UInt32)0, 1.0f, (UInt32)0);
		        //mDSRenderEnv.SetTargetViewport( mView.View.GetViewPort() );
                DllImportAPI.v3dDevice_SetViewPort(Engine.Instance.Client.Graphics.Device, mView.ViewPtr);
                if (AfterRender2View != null)
                    AfterRender2View(this);
                DllImportAPI.vDSRenderEnv_MRT_EndFinalDrawing(mDSRenderEnv, Engine.Instance.Client.Graphics.Device);
		        CounterEnvAfterR2V.End();

		        //mView.BeginDraw();
		        //mView.Clear((uint)(RenderAPI.V3DCLEAR_STENCIL|RenderAPI.V3DCLEAR_TARGET|RenderAPI.V3DCLEAR_ZBUFFER), 0xFF808080, 1.0f, 0);

          //      bool bSave = false;
          //      if (bSave)
          //      {
          //          Save2File("e:/finaltexture.bmp");
          //      }

                //RenderDS2View		
                /*ITexture^ tempTexture = gcnew ITexture();
		        tempTexture.LoadTexture("texture/Tulips.jpg");
		        tempTexture.Texture.RestoreObjects();
		        IEngine.Instance.Client.Graphics.CopyFrom(tempTexture.Texture, 0, 0, mWidth, mHeight);*/

          //      UInt32 left = (UInt32)(-(scaleCenter.X * (scaleX - 1)));
          //      UInt32 top = (UInt32)(-(scaleCenter.Y * (scaleY - 1)));
		        //if(scaleX > 1)
			       // left = 0;
		        //if(scaleY > 1)
			       // top = 0;
          //      UInt32 width = (UInt32)(mWidth * scaleX);
          //      UInt32 height = (UInt32)(mHeight * scaleY);
          //      var finalTex = DllImportAPI.vDSRenderEnv_GetFinalTexture(mDSRenderEnv);
          //      Engine.Instance.Client.Graphics.CopyFrom(finalTex, left, top, width, height);		

		        /*CounterEnvAfterR2V.Begin();
		        AfterRender2View(this);
		        CounterEnvAfterR2V.End();*/
		 
		        //CounterEnvDS2V.Begin();
		        //mView.EndDraw(true);
		        //CounterEnvDS2V.End();

		        //IEngine.Instance.Client.Graphics.Device.EndDraw();

		        CounterEnvRender.End();
            }
        }
        /// <summary>
        /// 设置场景中的摄像机
        /// </summary>
        /// <param name="camera">摄像机实例</param>
        public void SetCamera(CCore.Camera.CameraObject camera)
        {
            DllImportAPI.v3dDevice_SetCamera(Engine.Instance.Client.Graphics.Device, camera.CameraPtr);
        }
        /// <summary>
        /// 提交前向渲染管道
        /// </summary>
        public void SubmitFSPipe()
        {
            unsafe
            {
                if (mDSRenderEnv != IntPtr.Zero)
                {
                    DllImportAPI.vDSRenderEnv_SubmitFSPipe(mDSRenderEnv, (int)(RGroup.RL_World));
                }
            }           
        }
        /// <summary>
        /// 设置透明颜色MRT
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetClearColorMRT(CSUtility.Support.Color color)
        {
            unsafe
            {
                if (mDSRenderEnv != IntPtr.Zero)
                {
                    DllImportAPI.vDSRenderEnv_SetClearColorMRT(mDSRenderEnv, (UInt32)color.ToArgb());
                }
            }
        }
        /// <summary>
        /// 绘制点击代理
        /// </summary>
        /// <param name="ToView">是否显示</param>
        public void DrawHitProxy(bool ToView)
        {
            unsafe
            {
                //IDllImportAPI.v3dDevice_BeginDraw(IEngine.Instance.Client.Graphics.Device);
                //IDllImportAPI.vDSRenderEnv_SetTargetViewport(mDSRenderEnv, mView.View);
                DllImportAPI.vDSRenderEnv_DrawHitProxy(mDSRenderEnv, Engine.Instance.Client.Graphics.Device, mView.ViewPtr);
                //IDllImportAPI.v3dDevice_EndDraw(IEngine.Instance.Client.Graphics.Device);
            }
        }
        /// <summary>
        /// 读取点击代理数据
        /// </summary>
        public void ReadHitProxyData()
        {
            unsafe
            {
                DllImportAPI.vDSRenderEnv_UpdateHitProxyResult(mDSRenderEnv);
            }
        }
        /// <summary>
        /// 根据屏幕像素点得到点击代理值
        /// </summary>
        /// <param name="x">鼠标点击屏幕的X坐标值</param>
        /// <param name="y">鼠标点击屏幕的Y坐标值</param>
        /// <returns>点击的代理值</returns>
        public UInt32 GetHitProxy(int x, int y)
        {
            unsafe
            {
                return DllImportAPI.vDSRenderEnv_GetHitProxy(mDSRenderEnv, x, y);
            }
        }
        /// <summary>
        /// 得到鼠标勾选区域的所有点击代理值
        /// </summary>
        /// <param name="x">鼠标按下时在屏幕上的X轴坐标值</param>
        /// <param name="y">鼠标按下时在屏幕上的Y轴坐标值</param>
        /// <param name="w">鼠标在屏幕上拖动的宽</param>
        /// <param name="h">鼠标在屏幕上拖动的高</param>
        /// <param name="step">选中的步数</param>
        /// <returns>返回勾选区域所有的点击代理值</returns>
        public List<UInt32> GetHitProxyArea(int x, int y, int w, int h, int step)
        {
            unsafe
            {
                List<UInt32> hitList = new List<UInt32>();
                int hitCount = 0;
                int hit1, hit2, hit3 = 0;
                DllImportAPI.vDSRenderEnv_GetHitProxyArea(mDSRenderEnv, x, y, w, h, step, &hitCount, &hit1, &hit2, &hit3);
                if (hit1 != 0)
                    hitList.Add((UInt32)hit1);
                if (hit2 != 0)
                    hitList.Add((UInt32)hit2);
                if (hit3 != 0)
                    hitList.Add((UInt32)hit3);
                return hitList;
            }
        }
        /// <summary>
        /// 获得range上下左右偏移像素值
        /// </summary>
        /// <param name="x">鼠标点击的屏幕区域X轴的值</param>
        /// <param name="y">鼠标点击的屏幕区域Y轴的值</param>
        /// <param name="range">偏移值</param>
        /// <param name="list">点击选中的列表</param>
        /// <returns>返回range上下左右偏移像素的点击代理值</returns>
        public bool GetHitProxy(int x, int y, int range, List<UInt32> list)	//range 上下左右偏移像素值
        {
            unsafe
            {
                if (list == null || mDSRenderEnv == IntPtr.Zero)
                    return false;

                bool bFinded = false;

                for (int i = x - range; i <= x + range; ++i)
                {
                    for (int j = y - range; j <= y + range; ++j)
                    {
                        var idx = DllImportAPI.vDSRenderEnv_GetHitProxy(mDSRenderEnv, i, j);
                        if (idx > 0)
                        {
                            bFinded = true;
                            list.Add(idx);
                        }
                    }
                }

                return bFinded;
            }
        }
        /// <summary>
        /// 设置检测图像的边缘模式
        /// </summary>
        /// <param name="mode">检测图像的边缘模式</param>
        public void SetEdgeDetectMode(enEdgeDetectMode mode)
        {
            unsafe
            {
                DllImportAPI.vDSRenderEnv_SetEdgeDetectMode(mDSRenderEnv, (int)mode);
            }
        }
        /// <summary>
        /// 改变视窗大小尺寸
        /// </summary>
        /// <param name="view">需要改变的窗口</param>
        /// <param name="width">窗口的宽度</param>
        /// <param name="height">窗口的高度</param>
        public void ViewSizeChanged(View view, int width, int height)
        {
            unsafe
            {
                if (mDSRenderEnv != IntPtr.Zero)
                {
                    mWidth = width;
                    mHeight = height;
                    DllImportAPI.vDSRenderEnv_ResizeInternalRT(mDSRenderEnv, (UInt32)width, (UInt32)height, bUseIntZ, bUseHDR);
                }

                if (mCamera != null)
                {
                    var fov = DllImportAPI.v3dCamera_GetFOV(mCamera.CameraPtr);
                    var zNear = DllImportAPI.v3dCamera_GetNear(mCamera.CameraPtr);
                    var zFar = DllImportAPI.v3dCamera_GetFar(mCamera.CameraPtr);
                    DllImportAPI.v3dCamera_MakePerspective(mCamera.CameraPtr, fov, width, height, zNear, zFar);
                }
            }
        }
        /// <summary>
        /// 保存最终纹理到文件
        /// </summary>
        /// <param name="strFileName">保存的文件名</param>
        /// <param name="fileFormat">文件格式，缺省值为enD3DXIMAGE_FILEFORMAT.D3DXIFF_BMP</param>
        public void	Save2File(System.String strFileName, enD3DXIMAGE_FILEFORMAT fileFormat = enD3DXIMAGE_FILEFORMAT.D3DXIFF_BMP)
        {
            unsafe
            {
                if (mDSRenderEnv != IntPtr.Zero)
                {
                    DllImportAPI.vDSRenderEnv_FinalTexture_Save2File(mDSRenderEnv, strFileName, (UInt32)fileFormat);
                }
            }
        }
        /// <summary>
        /// 保存深度纹理文件
        /// </summary>
        /// <param name="strFileName">保存的文件名</param>
        /// <param name="fileFormat">文件格式，默认为BMP格式</param>
        public void SaveDepth2File(System.String strFileName, enD3DXIMAGE_FILEFORMAT fileFormat = enD3DXIMAGE_FILEFORMAT.D3DXIFF_BMP)
        {
            unsafe
            {
                if (mDSRenderEnv != IntPtr.Zero)
                {
                    DllImportAPI.vDSRenderEnv_DepthTexture_Save2File(mDSRenderEnv, strFileName, (UInt32)fileFormat);
                }
            }
        }
        /// <summary>
        /// 后处理的渲染刷新
        /// </summary>
        /// <param name="postProceses">后处理的实例集合</param>
        public void RefreshPostProcess(List<PostProcess> postProceses)
        {
            unsafe
            {
                DllImportAPI.vDSRenderEnv_PostProcessPipe_Clear(mDSRenderEnv);
                for (int i = 0; i < postProceses.Count; ++i)
                {
                    var pProcess = postProceses[i];
                    if (pProcess.m_Type == enPostProcessType.HDR)
                    {
                        DllImportAPI.vDSRenderEnv_SetPostProcess_ToneMapping(mDSRenderEnv, pProcess.mPostProcess);
                    }
                    else if (pProcess.m_Type == enPostProcessType.SSAO)
                    {
                        DllImportAPI.vDSRenderEnv_SetPostProcess_SSAO(mDSRenderEnv, pProcess.mPostProcess);
                    }
                    else
                    {
                        if (pProcess.Enable == true)
                        {
                            DllImportAPI.vDSRenderEnv_PostProcessPipe_Push_back(mDSRenderEnv, pProcess.mPostProcess);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 提交渲染纹理等
        /// </summary>
        /// <param name="layer">渲染的层</param>
        /// <param name="obj">2D纹理</param>
        /// <param name="bImmediate">是否及时渲染</param>
        public void Commit(RLayer layer, CCore.Graphics.Text2D obj, bool bImmediate)
        {
            unsafe
            {
                var matrix = SlimDX.Matrix.Identity;
                DllImportAPI.vDSRenderEnv_CommitText2D(mDSRenderEnv, (int)(RGroup.RL_World), (int)layer, obj.Text2DObject, &matrix, bImmediate);
            }
        }
        /// <summary>
        /// 检查是否有设备丢失
        /// </summary>
        /// <returns>如果找不到设备返回false，否则返回true</returns>
        public bool CheckDeviceLost()
        {
            unsafe
            {
                var retValue = 0; // IDllImportAPI.vDSRenderEnv_CheckDeviceLost(mDSRenderEnv, IEngine.Instance.Client.Graphics.Device, (int)(mView.PresentResult), (UInt32)mWidth, (UInt32)mHeight, bUseIntZ);
                return (retValue != 0) ? true : false;
            }
        }
        /// <summary>
        /// 将渲染环境提交到MRT
        /// </summary>
        public void CaptureMRT()
        {
            unsafe
            {
                DllImportAPI.vDSRenderEnv_SaveMRT(mDSRenderEnv);
            }
        }
        /// <summary>
        /// 只写属性，是否进行前向渲染
        /// </summary>
        public bool DoFSBlur
        {
            set
            {
                if (value == true)
                    DllImportAPI.vDSRenderEnv_DoFSBlur(mDSRenderEnv, 1);
                else
                    DllImportAPI.vDSRenderEnv_DoFSBlur(mDSRenderEnv, 0);
            }
        }
        /// <summary>
        /// 只写属性，是否在提交最终纹理前进行复制
        /// </summary>
        public bool DoCopyPreFinal
        {
            set
            {
                if (value == true)
                    DllImportAPI.vDSRenderEnv_DoCopyPreFinal(mDSRenderEnv, 1);
                else
                    DllImportAPI.vDSRenderEnv_DoCopyPreFinal(mDSRenderEnv, 0);
            }
        }
        /// <summary>
        /// 只写属性，是否进行前向渲染的Blur处理
        /// </summary>
        public bool DoFSPostBlur
        {
            set
            {
                if (value == true)
                    DllImportAPI.vDSRenderEnv_DoFSPostBlur(mDSRenderEnv, 1);
                else
                    DllImportAPI.vDSRenderEnv_DoFSPostBlur(mDSRenderEnv, 0);
            }
        }

        // 刷新ShaderCache
        /// <summary>
        /// 刷新ShaderCache
        /// </summary>
        /// <param name="materialId">材质ID</param>
        public void Editor_UpdateShaderCache(System.Guid materialId)
        {
            unsafe
            {
                IntPtr material = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadMaterialInstance(ref materialId);
                DllImportAPI.vDSRenderEnv_Editor_UpdateShaderCache(mDSRenderEnv, material);
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.RemoveMaterial(materialId);
                DllImportAPI.v3dStagedMaterialInstance_Release(material);
            }
        }
        /// <summary>
        /// 将ShaderCache进行打包
        /// </summary>
        public void Editor_PackShaderCache()
        {
            unsafe
            {
                if (!System.IO.Directory.Exists(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultShaderCacheDirectory))
                {
                    return;
                }

                var device = Engine.Instance.Client.Graphics.Device;
                var packNode = DllImportAPI.vDSRenderEnv_Editor_BeginPackShaderCache(device);
                var files = System.IO.Directory.GetFiles(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultShaderCacheDirectory, "*.fxb", System.IO.SearchOption.AllDirectories);
                foreach (var i in files)
                {
                    DllImportAPI.vDSRenderEnv_Editor_PackShaderCache(device, packNode, i);
                }
                DllImportAPI.vDSRenderEnv_Editor_EndPackShaderCache(device, packNode);
            }
        }
        /// <summary>
        /// 得到最终纹理的像素数据
        /// </summary>
        /// <param name="datas">数据</param>
        /// <param name="tagTexture">纹理</param>
        /// <returns>如果得到返回true，否则返回false</returns>
        public bool GetFinalTexturePixelData(ref byte[] datas, Texture tagTexture)
        {
            unsafe
            {
                fixed(byte* pinData = datas)
                {
                    var result = DllImportAPI.vDSRenderEnv_GetFinalTexturePixelData(mDSRenderEnv, pinData, tagTexture.TexturePtr);
                    return result == 0 ? false : true;
                }
            }
        }
    }
}
