using System;
using System.Collections.Generic;

namespace CCore
{
    /// <summary>
    /// 引擎的初始化类
    /// </summary>
    public class EngineInit
    {
        /// <summary>
        /// 客户端初始化类对象
        /// </summary>
        public ClientInit ClientInit;
    }
    /// <summary>
    /// 点击的坐标点
    /// </summary>
    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    public struct TagPOINT
    {
        /// <summary>
        /// X轴的坐标点
        /// </summary>
        public int x;
        /// <summary>
        /// Y轴的坐标点
        /// </summary>
        public int y;
    }

    //public struct TagMSG : System.IEquatable<TagMSG>
    //{
    //    public IntPtr hwnd;
    //    public uint message;
    //    public IntPtr wParam;
    //    public IntPtr lParam;
    //    public UInt32 time;
    //    public TagPOINT point;

    //    public override bool Equals(object value)
    //    {
    //        if (value == null)
    //            return false;

    //        if (value.GetType() != GetType())
    //            return false;

    //        return Equals((TagMSG)value);
    //    }

    //    public bool Equals(TagMSG value)
    //    {
    //        return (message == value.message &&
    //                wParam == value.wParam && 
    //                lParam == value.lParam &&
    //                point.x == value.point.x &&
    //                point.y == value.point.y);
    //    }

    //    public static bool Equals(ref TagMSG value1, ref TagMSG value2)
    //    {
    //        return (value1.message == value2.message &&
    //                value1.wParam == value2.wParam &&
    //                value1.lParam == value2.lParam &&
    //                value1.point.x == value2.point.x &&
    //                value1.point.y == value2.point.y);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return message.GetHashCode() + wParam.GetHashCode() + lParam.GetHashCode() + point.x.GetHashCode() + point.y.GetHashCode();
    //    }
    //}
    /// <summary>
    /// windows消息
    /// </summary>
    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    public struct WndMSG
    {
        /// <summary>
        /// 窗口句柄指针
        /// </summary>
        public IntPtr hwnd;
        /// <summary>
        /// 消息信息
        /// </summary>
        public UInt32 message;
        /// <summary>
        /// 消息参数
        /// </summary>
        public UInt32 wParam;
        /// <summary>
        /// 消息参数
        /// </summary>
        public UInt32 lParam;
        /// <summary>
        /// 消息产生时间
        /// </summary>
        public UInt32 time;
        /// <summary>
        /// 点击的坐标点
        /// </summary>
        public TagPOINT pt;
    };
    /// <summary>
    /// 声明引擎循环调用的委托事件
    /// </summary>
    public delegate void FEngineLoopTick();
    /// <summary>
    /// 引擎类
    /// </summary>
    public class Engine
    {
        // 多线程渲染
        /// <summary>
        /// 是否进行多线程渲染
        /// </summary>
        public static bool IsMultiThreadRendering = false;

        static Engine smEngine;
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static Engine Instance
        {
            get
            {
                if (smEngine == null)
                    smEngine = new Engine(); 
                return smEngine;
            }
        }
        /// <summary>
        /// 没有寻到路的时候是否进行直线移动
        /// </summary>
        protected bool mDirectMoveWhenNotFindPath = false;
        /// <summary>
        /// 没有寻到路的时候是否进行直线移动
        /// </summary>
        public bool DirectMoveWhenNotFindPath
        {
            get { return mDirectMoveWhenNotFindPath; }
            set
            {
                mDirectMoveWhenNotFindPath = value;
            }
        }
        /// <summary>
        /// 客户端对象
        /// </summary>
        protected Client mClient;
        /// <summary>
        /// 只读属性，获取当前的客户端对象
        /// </summary>
        public Client Client
        {
            get { return mClient; }
        }
        /// <summary>
        /// 只读属性，获取当前应用占用的内存地址
        /// </summary>
        public IntPtr CppMemUsed
        {
            get { return CSUtility.DllImportAPI.vfxMemory_MemoryUsed(); }
        }
        /// <summary>
        /// 只读属性，获取当前应用占用的最大内存地址
        /// </summary>
        public IntPtr CppMemMax
        {
            get { return CSUtility.DllImportAPI.vfxMemory_MemoryMax(); }
        }
        /// <summary>
        /// 只读属性，获取当前应用内存使用次数
        /// </summary>
        public IntPtr CppMemAllocTimes
        {
            get { return CSUtility.DllImportAPI.vfxMemory_MemoryAllocTimes(); }
        }        
        /// <summary>
        /// 帧率
        /// </summary>
        protected Int64 mFrameTickCount = CSUtility.DllImportAPI.vfxGetTickCount();
        /// <summary>
        /// 每帧之间的世界间隔
        /// </summary>
        protected Int64 mElapsedMillisecond;
        /// <summary>
        /// 每帧调用的次数
        /// </summary>
        protected Int64 mFrameTickHighPrecision = CSUtility.DllImportAPI.HighPrecision_GetTickCount();
        /// <summary>
        /// 每帧之间的高精度间隔时间
        /// </summary>
        protected Int64 mElapsedHighPrecision;
        /// <summary>
        /// 是否保持循环，默认为true
        /// </summary>
        protected bool mKeepLoop = true;

        //protected System.Reflection.Assembly m_FrameSetAssm = System.Reflection.Assembly.LoadFrom("FrameSet.dll");
        //protected System.Reflection.Assembly m_CSCommonAssm = System.Reflection.Assembly.LoadFrom("CSUtility.dll");

        private static CSUtility.Performance.PerfCounter ClientTick = new CSUtility.Performance.PerfCounter("LTick.Engine.Client");
        private static CSUtility.Performance.PerfCounter CounterLogicTimer = new CSUtility.Performance.PerfCounter("LTick.Engine.LogiTimer");
        /// <summary>
        /// 主角是否跟随点击移动，默认为true
        /// </summary>
        public static bool mChiefRoleMoveWithClick = true;
        /// <summary>
        /// 主角是否跟随点击移动
        /// </summary>
        public static bool ChiefRoleMoveWithClick
        {
            get { return mChiefRoleMoveWithClick; }
            set
            {
                mChiefRoleMoveWithClick = value;
            }
        }

        bool mIsEditorMode = false;
        /// <summary>
        /// 是否为编辑模式
        /// </summary>
        public bool IsEditorMode
        {
            get { return mIsEditorMode; }
            set
            {
                mIsEditorMode = value;
                Client.Graphics.IsEditorMode = value;
            }
        }
        /// <summary>
        /// 当前渲染的帧
        /// </summary>
        public UInt32 CurRenderFrame;
        /// <summary>
        /// 渲染的Actor数量
        /// </summary>
        public int RenderActorNumber;
        /// <summary>
        /// 渲染通用Actor的数量
        /// </summary>
        public int RenderActor_Common_Number;
        /// <summary>
        /// 渲染玩家Actor的数量
        /// </summary>
        public int RenderActor_Player_Number;
        /// <summary>
        /// 渲染NPC的数量
        /// </summary>
        public int RenderActor_Npc_Number;
        /// <summary>
        /// 渲染光源Actor的数量
        /// </summary>
        public int RenderActor_Light_Number;
        /// <summary>
        /// 渲染贴花Actor的数量
        /// </summary>
        public int RenderActor_Decal_Number;
        /// <summary>
        /// 渲染初始化NPC的数量
        /// </summary>
        public int RenderActor_NpcInitializer_Number;
        /// <summary>
        /// 渲染触发器Actor的数量
        /// </summary>
        public int RenderActor_Trigger_Number;
        /// <summary>
        /// 渲染特效Actor的数量
        /// </summary>
        public int RenderActor_Effect_Number;
        /// <summary>
        /// 渲染特效NPC的数量
        /// </summary>
        public int RenderActor_EffectNpc_Number;
        /// <summary>
        /// 每帧渲染Actor的数量
        /// </summary>
        public int TickActorNumber;
        /// <summary>
        /// 不可见的Actor的数量
        /// </summary>
        public int ActorNoVisualTickNumber;
        /// <summary>
        /// 动画的数量
        /// </summary>
        public int TickAnimationTreeNumber;
        /// <summary>
        /// 粒子发射mesh的数量
        /// </summary>
        public int Effect_ParticleMesh_Number;
        /// <summary>
        /// 粒子池的数量
        /// </summary>
        public int Effect_ParticlePool_Number;
        /// <summary>
        /// 生存的粒子数量
        /// </summary>
        public int Effect_ParticleLive_Number;
        /// <summary>
        /// 是否每帧渲染，默认为true
        /// </summary>
        public bool EnableRenderTick = true;
        /// <summary>
        /// 声明主框架激活时调用的委托事件
        /// </summary>
        /// <param name="active">是否激活</param>
        public delegate void Delegate_OnMainFormActivated(bool active);
        /// <summary>
        /// 定义主框架激活时调用的委托事件
        /// </summary>
        public event Delegate_OnMainFormActivated OnMainFormActivated;
        bool mMainFormActivated = true;
        /// <summary>
        /// 主框架是否激活
        /// </summary>
        public bool MainFormActivated
        {
            get { return mMainFormActivated; }
            set
            {
                mMainFormActivated = value;
                OnMainFormActivated?.Invoke(mMainFormActivated);
            }
        }
        /// <summary>
        /// 屏幕震动列表
        /// </summary>
        public List<CCore.Support.ShakeScreen> mShakeScreenList = new List<CCore.Support.ShakeScreen>();        
        /// <summary>
        /// 屏幕震动
        /// </summary>
        /// <param name="eye">视野</param>
        public void ScreenShake(Camera.CameraController eye)
        {
            if (eye == null)
                return;

            if (mShakeScreenList.Count <= 0)
                return;

            double x = 0;
            double y = 0;

            int iShakeCount = 0;
            foreach (var shake in mShakeScreenList)
            {
                if (shake.Enable == true)
                {
                    x += shake.mShakeX;
                    y += shake.mShakeY;
                }
                iShakeCount++;
            }

            x = x / iShakeCount;
            y = y / iShakeCount;
            SlimDX.Vector3 delta = new SlimDX.Vector3();
            delta.X = (float)x;
            delta.Y = (float)y;
            delta.Z = 0;

            eye.Move(delta);
        }

        private Engine()
        {

        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~Engine()
        {
            Cleanup();
        }
        static CCore.DllImportAPI.Delegate_FreeGCHandle OnFreeGCHandle = FreeGCHandle;
        private static void FreeGCHandle(IntPtr handle)
        {
            var gcHandle = System.Runtime.InteropServices.GCHandle.FromIntPtr(handle);
            gcHandle.Target = null;
            gcHandle.Free();
        }
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="_init">用于初始化该对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(EngineInit _init)
        {
            CCore.DllImportAPI.v3dGraphics_SetFreeGCHandleEvent(OnFreeGCHandle);
            if (mClient == null && _init.ClientInit != null)
            {
                mClient = new Client();
                if (mClient.Initialize(_init.ClientInit) == false)
                {
                    mClient = null;
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// 删除客户端对象
        /// </summary>
		public void Cleanup()
        {
            if (mClient != null)
            {
                mClient.Cleanup();
                mClient = null;
            }
        }
        /// <summary>
        /// 是否重置引擎数据
        /// </summary>
        /// <returns>返回false，即不重置引擎数据</returns>
		public bool ResetEngine()
        {
            return false;
        }
        /// <summary>
        /// 暂停循环
        /// </summary>
		public void _ShutdownEngineLoop()
        {
            mKeepLoop = false;
        }
        /// <summary>
        /// 引擎的每帧渲染
        /// </summary>
        /// <param name="interval">每帧之间间隔时间的最大值</param>
        /// <param name="loopTicker">回调函数</param>
        public void EngineTickLoop(int interval, FEngineLoopTick loopTicker)
        {
            Int64 nowTime = CSUtility.DllImportAPI.vfxGetTickCount();
            if (nowTime - mFrameTickCount < interval)
            {
                DllImportAPI.IEngine_Sleep(1);
                //Sleep(1);
            }
            else
            {
                mElapsedMillisecond = nowTime - mFrameTickCount;
                mFrameTickCount = nowTime;

                mElapsedHighPrecision = mElapsedMillisecond * 1000;
                mFrameTickHighPrecision = mFrameTickCount * 1000;

                DllImportAPI.IEngine_SetEngineTick(nowTime);

                if (loopTicker != null)
                {
                    loopTicker();
                }
            }
        }
        /// <summary>
        /// 持续循环
        /// </summary>
        /// <param name="interval">循环的间隔时间</param>
        /// <param name="loopTicker">回调函数</param>
        /// <param name="hWnd">句柄指针</param>
        /// <returns>返回常量0</returns>
        public int DoLoop(int interval, FEngineLoopTick loopTicker, System.IntPtr hWnd)
        {
            unsafe
            {
                if (interval < 3)
                    interval = 0;
                mFrameTickCount = CSUtility.DllImportAPI.vfxGetTickCount();
                for (;;)
                {
                    var msg = new WndMSG();
                    if (mKeepLoop == false)
                        break;

                    try
                    {
                        _UpdateFrameSecondTimeFloatByUVAnim();
                        //if(PeekMessage(&msg,NULL(HWND)hWnd.ToPointer(),0,0,0))
                        if (DllImportAPI.IEngine_PeekMessage((IntPtr)(&msg), IntPtr.Zero, 0, 0, 0) != 0)
                        {
                            if (msg.message == 0x0012)
                            {
                                break;
                            }

                            if (MainFormActivated)
                                mClient.MsgRecieverMgr.OnSystemMsg((int)msg.message, (UIntPtr)msg.wParam, (UIntPtr)msg.lParam);

#if WIN
                            System.Windows.Forms.Application.DoEvents();
#else
#endif
                        }
                        else
                        {
                            EngineTickLoop(interval, loopTicker);

                            //if(MainFormActivted)
                            //    mClient.MsgRecieverMgr.OnKeyboardStateCheck();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.FileLog.WriteLine(ex.ToString());
                        Log.FileLog.WriteLine(ex.StackTrace.ToString());
                    }
                }
            }

            return 0;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="bOnlyMainWorld">是否仅刷新主画面</param>
        public void Tick(bool bOnlyMainWorld)
        {
            //这个有可能被多次调用，导致Elapse不准
            /*INT64 nowTick = _GetTickCount();
		    mElapsedMillisecond = nowTick - mFrameTickCount;
		    mFrameTickCount = nowTick;
		    vfxSetEngineTick(nowTick); */

		    RenderActorNumber = 0;
            RenderActor_Common_Number = 0;
            RenderActor_Player_Number = 0;
            RenderActor_Npc_Number = 0;
            RenderActor_Light_Number = 0;
            RenderActor_Decal_Number = 0;
            RenderActor_NpcInitializer_Number = 0;
            RenderActor_Trigger_Number = 0;
            RenderActor_Effect_Number = 0;
            RenderActor_EffectNpc_Number = 0;
		    TickActorNumber = 0;
		    ActorNoVisualTickNumber = 0;
		    TickAnimationTreeNumber = 0;
            Effect_ParticleMesh_Number = 0;
            Effect_ParticlePool_Number = 0;
            Effect_ParticleLive_Number = 0;

		    CounterLogicTimer.Begin();
		    CSUtility.Helper.LogicTimerManager.Instance.Tick(GetElapsedMillisecond());
		    CounterLogicTimer.End();

		    ClientTick.Begin();
            if( mClient!=null )
		    {
                mClient.Tick();
                
                DllImportAPI.v3dDevice_AddCustomShaderTime(mClient.Graphics.Device, GetElapsedMillisecond());
			    //mClient.Graphics.Device.m_CustomShaderTime += GetElapsedMillisecond();
		    }
            ClientTick.End();

            for (int i = 0; i < mShakeScreenList.Count; ++i )
            {
                mShakeScreenList[i].Tick();
                if (mShakeScreenList[i].Enable == false)
                    mShakeScreenList.RemoveAt(i);
            }
        }
        /// <summary>
        /// 异步循环调用
        /// </summary>
		public void SyncTick()
        {
            unsafe
            {
                CSUtility.DllImportAPI.v3dSampMgr_Update();
            }
        }
        /// <summary>
        /// 获取调用次数
        /// </summary>
        /// <returns>返回调用次数</returns>
		public Int64 _GetTickCount()//真正的tickcount
        {
            //临时代码,今后要通过网络同步,从指定的StateServer上同步过来，定时修正
            //return IDllImportAPI.HighPrecision_GetTickCount() / 1000;
            return CSUtility.DllImportAPI.vfxGetTickCount();
        }
        /// <summary>
        /// 当前桢毫秒
        /// </summary>
        /// <returns>返回当前桢毫秒</returns>
		public Int64 GetFrameMillisecond()//当前桢毫秒
        {
            return mFrameTickCount;
        }
        /// <summary>
        /// 当前桢秒数
        /// </summary>
        /// <returns>返回当前桢秒数</returns>
        public Int64 GetFrameSecondTime()//当前桢秒数
        {
            return GetFrameMillisecond() / 1000;
        }
        /// <summary>
        /// 当前桢秒数，精确到小数点后一位
        /// </summary>
        /// <returns>返回当前桢秒数，精确到小数点后一位</returns>
		public float GetFrameSecondTimeFloat()//当前桢秒数，精确到小数点后一位
        {
            var time = GetFrameMillisecond();
            return time * 0.001f;
        }
        /// <summary>
        /// 更新UV动画每帧调用的时间
        /// </summary>
        public void _UpdateFrameSecondTimeFloatByUVAnim()
        {
            mFrameSecondTimeFloatByUVAnim = CSUtility.DllImportAPI.vfxGetTickCount() * 0.001F;// IDllImportAPI.HighPrecision_GetTickCount() * 0.000001F;
        }
        float mFrameSecondTimeFloatByUVAnim;
        /// <summary>
        /// 获取UV动画每帧调用的时间
        /// </summary>
        /// <returns>返回UV动画每帧调用的时间</returns>
        public float GetFrameSecondTimeFloatByUVAnim()
        {
            return mFrameSecondTimeFloatByUVAnim;
        }
        /// <summary>
        /// 获取每帧之间的间隔时间
        /// </summary>
        /// <returns>返回每帧之间的间隔时间</returns>
		public Int64 GetElapsedMillisecond()
        {
            return mElapsedMillisecond;
        }
        /// <summary>
        /// 获取每帧之间的高精度间隔时间
        /// </summary>
        /// <returns>返回每帧之间的高精度间隔时间</returns>
        public Int64 GetElapsedHighPrecision()
        {
            return mElapsedHighPrecision;
        }
        

        //public System.Type GetType(string typeName)
        //{
        //    if (string.IsNullOrEmpty(typeName))
        //        return null;

        //    System.Type retType = null;

        //    retType = System.Type.GetType(typeName);
        //    if (retType == null)
        //        retType = m_FrameSetAssm.GetType(typeName);
        //    if (retType == null)
        //        retType = m_CSCommonAssm.GetType(typeName);

        //    return retType;
        //}
        /// <summary>
        /// 清除导航状态
        /// </summary>
		public void DumpNativeMemoryState()
        {
            DllImportAPI.vfxMemory_DumpMemoryState("IEngine.DumpNativeMemoryState");
        }
        /// <summary>
        /// 清除导航状态
        /// </summary>
        /// <param name="info">导航信息描述</param>
		public void DumpNativeMemoryState(string info)
        {
            DllImportAPI.vfxMemory_DumpMemoryState(info);
        }
        /// <summary>
        /// 检查导航的内存状态
        /// </summary>
        /// <param name="info">导航信息</param>
		public void CheckNativeMemoryState(string info)
        {
            DllImportAPI.vfxMemory_CheckMemoryState(info);
        }
        /// <summary>
        /// 获取异步加载次数
        /// </summary>
        /// <returns>返回异步加载次数</returns>
		public int GetAsyncLoadNumber()
        {
            return DllImportAPI.vLoadPipe_GetAsyncLoadNumber();
        }
        /// <summary>
        /// 设置是否强制预加载
        /// </summary>
        /// <param name="force">是否强制预加载</param>
        public void SetPreUseForceMode(bool force)
        {
            if (force)
                DllImportAPI.vLoadPipe_SetPreUseForceMode(1);
            else
                DllImportAPI.vLoadPipe_SetPreUseForceMode(0);
        }
        static CSUtility.DllImportAPI.Delegate_OnNativeMemAlloc OnNativeMemAlloc = OnNativeMemAllocCB;
        static CSUtility.DllImportAPI.Delegate_OnNativeMemFree OnNativeMemFree = OnNativeMemFreeCB;
        private static void OnNativeMemAllocCB(UInt32 size, IntPtr file, UInt32 line, UInt32 id)
        {
            var strFile = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(file);
            if(string.IsNullOrEmpty(strFile) && line==0 && size==112)
            {
            }
        }
        private static void OnNativeMemFreeCB(UInt32 size, IntPtr file, UInt32 line, UInt32 id)
        {

        }
        /// <summary>
        /// 开始监视导航内存
        /// </summary>
        public static void BeginWatchNativeMem()
        {
            CSUtility.DllImportAPI.vfxMemory_SetMemAllocCallBack(OnNativeMemAlloc);
            CSUtility.DllImportAPI.vfxMemory_SetMemFreeCallBack(OnNativeMemFree);
        }
        /// <summary>
        /// 结束监视导航内存
        /// </summary>
        public static void EndWatchNativeMem()
        {
            CSUtility.DllImportAPI.vfxMemory_SetMemAllocCallBack(null);
            CSUtility.DllImportAPI.vfxMemory_SetMemFreeCallBack(null);
        }
        /// <summary>
        /// 只写属性，是否保证线程锁
        /// </summary>
        public static bool EnableThreadLockInfo
        {
            set
            {
                if(value)
                    CSUtility.DllImportAPI.VCriticalInfoManager_SetEnable(1);
                else
                    CSUtility.DllImportAPI.VCriticalInfoManager_SetEnable(0);
            }
        }
        /// <summary>
        /// 打印线程锁信息
        /// </summary>
        /// <returns>返回线程信息</returns>
        public static string PrintThreadLockInfo()
        {
            var ptr = CSUtility.DllImportAPI.VCriticalInfoManager_PrintLockInfo();
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);
        }
    }
}
