using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Content.PM;
using vise3d.moba;
using Android.Content.Res;

namespace Game.Android
{
    [Activity(Label = "vise3d.moba", MainLauncher = true,Icon = "@drawable/icon",LaunchMode = LaunchMode.SingleTask,
        ScreenOrientation = ScreenOrientation.Landscape,ConfigurationChanges =ConfigChanges.Orientation | ConfigChanges.ScreenSize| ConfigChanges.KeyboardHidden)]
    public class MainActivity : Activity, ISurfaceHolderCallback ,GestureDetector.IOnGestureListener
    {
        //int count = 1;
        GLView GameView = null;
        GestureDetector mGesture = null;//手势
    //    public MainService mBindServer = null;
     //   private IServiceConnection mServerConnection;

//         public class MainserverConnect :Java.Lang.Object,IServiceConnection
//         {
//             MainActivity mainAc;
//             public MainserverConnect(MainActivity ma)
//             {
//                 mainAc = ma;
//             }
// 
//             public void OnServiceConnected(ComponentName name, IBinder service)
//             {
//                 var bind = service as MainBind;
//                 if(bind !=null)
//                     mainAc.mBindServer = bind.Getservice();
//             }
// 
//             public void OnServiceDisconnected(ComponentName name)
//             {
//                 mainAc.mBindServer = null;
//             }
//         }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //System.Diagnostics.Trace.WriteLine("Vise3d =====");
         
            if (GameView==null)
            {
                Log.FileLog.Instance.Begin("vfx.log");
                Log.FileLog.WriteLine("Android MainActivity OnCreate");
                CSUtility.Program.CurrentPlatform = CSUtility.enPlatform.Android;

                var clientWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "ClientCommon.dll");
                CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Android, "cscommon", clientWindowsAssembly);
                var clientAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Client.dll");
                CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Android, "game", clientAssembly);

                GameView = new GLView(this);//,null, Resource.Layout.Main);
         //       RequestWindowFeature(global::Android.Views.WindowFeatures.NoTitle);

                SetContentView(GameView);
                mGesture = new GestureDetector(this);
                GameView.Holder.AddCallback(this);
            }
            
            //  ActivityManager activityManager = (ActivityManager)GetSystemService(ActivityService);
            //var task = activityManager.GetRunningTasks(10);
        }

        public override void OnConfigurationChanged(Configuration newConfig)//屏幕切换时调用
        {
            base.OnConfigurationChanged(newConfig);
        }
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }
        //      PowerManager.WakeLock wakeLock;
        protected override void OnDestroy()
        {
            //    UnbindService(mServerConnection);
            ActivityManager activityManager = (ActivityManager)GetSystemService(ActivityService);
            var task = activityManager.GetRunningTasks(10);
            base.OnDestroy();

        }
        protected override void OnStart()
        {
            base.OnStart();
        }
        protected override void OnStop()
        {
            
            base.OnStop();
        }        
        protected override void OnRestart()
        {
            base.OnRestart();
        }

        protected override void OnPause()
        {
            this.PauseGame();
            base.OnPause();
        }
        protected override void OnResume()
        {
            base.OnResume();
            //this.ResumeGame();
            
    //       mServerConnection = new MainserverConnect(this);
      //     BindService(new Intent(this, typeof(MainService)), mServerConnection, Bind.AutoCreate);
            //             PowerManager pm = (PowerManager)GetSystemService(PowerService);
            //             wl = pm.NewWakeLock(WakeLockFlags.Full, "");
            //             wl.Acquire();
            //             PowerManager pm = (PowerManager)GetSystemService(POWER_SERVICE);
            //             wakeLock = pm.NewWakeLock(PowerManager.SCREEN_BRIGHT_WAKE_LOCK
            //                     | PowerManager.ON_AFTER_RELEASE, "DPA");
        }
        public override void OnActionModeStarted(ActionMode mode)
        {
            base.OnActionModeStarted(mode);
        }
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            if (Client.Game.Instance.GInit == null)
                return;
            //var newHandle = GetMainWindow();
            //Client.Game.Instance.GInit.ViewTarget.Handle = newHandle;
            //CCore.Engine.Instance.Client.Graphics.InvalidateDeviceResource();
            //if (CCore.Engine.Instance.Client.Graphics.TryDeviceReset(newHandle) == false)
            //    return;

            //CCore.Engine.Instance.Client.Graphics.RestoreDeviceResource();
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (Client.Game.Instance.GInit == null)
                return;
            //var newHandle = GetMainWindow();
            //Client.Game.Instance.GInit.ViewTarget.Handle = newHandle;
            //CCore.Engine.Instance.Client.Graphics.InvalidateDeviceResource();
            //if (CCore.Engine.Instance.Client.Graphics.TryDeviceReset(newHandle) == false)
            //    return;

            //CCore.Engine.Instance.Client.Graphics.RestoreDeviceResource();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            //Client.Game.Instance.REnviroment.SetView(null);
        }
        public override bool MoveTaskToBack(bool nonRoot)
        {
            return base.MoveTaskToBack(nonRoot);
        }

        System.Threading.Thread mThreadGame = null;
        bool mIsGameThreadRunning = false;
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                if (mThreadGame != null)
                {
                    this.ResumeGame();
                }
                else
                {
                    mThreadGame = new System.Threading.Thread(new System.Threading.ThreadStart(this.AGameMain));
                    mIsGameThreadRunning = true;
                    mThreadGame.Name = "GameThread";
                    mThreadGame.Start();
                }
            }
        }

        private void StopGameThread()
        {
            mIsGameThreadRunning = false;
        }

        bool mIsGameStart = false;
        Client.GameInit mGInit;
        bool IsPaused = false;
        bool IsPauseOK = false;
        bool IsResumeOK = true;

        private IntPtr GetMainWindow()
        {
            var hd = GameView.Holder.Surface.Handle;//JNIEnv.ToJniHandle(this.Holder.Surface);
            return CCore.Graphics.ViewTarget.Android_ANWinFromSurface(JNIEnv.Handle, hd);
        }
        private void AGameMain()
        {
            CSUtility.Program.LogInfo("AGameMain");
            if (mIsGameStart == false)
            {
                if (false == InitGame())
                    return;
                mIsGameStart = true;
            }
            CSUtility.Program.LogInfo("AGameMain 1");

            while (mIsGameThreadRunning)
            {
                if (IsPaused)
                {
                    if (IsPauseOK == false)
                    {
                        Client.Game.Instance.OnPause();
                        IsPauseOK = true;
                    }
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
                else
                {
                    if (IsResumeOK == false)
                    {
                        var oldHandle = Client.Game.Instance.GInit.ViewTarget.Handle;
                        var newHandle = GetMainWindow();
                        Client.Game.Instance.GInit.ViewTarget.Handle = newHandle;
                        if (Client.Game.Instance.OnResume() == false)
                            continue;
                        IsResumeOK = true;
                    }
                    else
                    {
                        GameTick();
                    }
                }
            }
        }
        public void PauseGame()
        {
            if (mIsGameStart)
            {
                IsPauseOK = false;
                IsPaused = true;
                while (IsPauseOK == false)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
        public void ResumeGame()
        {
            if (mIsGameStart)
            {
                IsPauseOK = false;
                IsPaused = false;
                IsResumeOK = false;
            }
        }
        private bool InitGame()
        {
            Client.AppConfig.Instance.MTRendering = true;

            var init = new Client.GameInit();
            mGInit = init;
            //Client.AppConfig.Instance.AndroidPackageName = "vise3d.wolf";
            init.AndroidPackageName = Client.AppConfig.Instance.AndroidPackageName;// "vise3d.moba";
            init.ViewTarget = new CCore.Graphics.ViewTarget();
            init.ScaleUIWidthViewTarget = false;            

            init.ViewTarget.Handle = GetMainWindow();
            init.ViewTarget.Width = init.ViewTarget.ClientWidth = GameView.Width;
            init.ViewTarget.Height = init.ViewTarget.ClientHeight = GameView.Height;
            init.ViewTarget.MRTScale = 960.0F / (float)GameView.Width;// 0.5F; //0.3其实画面也能接受
            init.DeviceType = CCore.Graphics.EDeviceType.TypeGLES2;            

            Client.AppConfig.Instance.MTRendering = true;

            try
            {
                Client.Game.Instance.Start(init);
            }
            catch(Exception e)
            {
                CSUtility.Program.LogInfo("Client.Game.Instance.Start ->" + e.ToString());
                return false;
            }

            Client.Game.Instance.REnviroment.SetClearColorMRT(CSUtility.Support.Color.Black);
            
            FreeCameraController = new CCore.Camera.MayaPosCameraController();
            FreeCameraController.Camera = Client.Game.Instance.REnviroment.Camera;

            SlimDX.Vector3 pos = new SlimDX.Vector3(-5, 20, -5);
            SlimDX.Vector3 lookAt = new SlimDX.Vector3(0, 0, 0);
            SlimDX.Vector3 up = new SlimDX.Vector3(0, 1, 0);
            FreeCameraController.SetPosLookAtUp(ref pos, ref lookAt, ref up);

            CSUtility.Program.LogInfo("InitGame OK");
            return true;
        }
        private void GameTick()
        {
            try
            {
                this.ProcTouchEvents();
                CCore.Engine.Instance.EngineTickLoop(Client.AppConfig.Instance.Interval, () =>
                {
                    Client.Game.Instance.Tick();
                });
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            }
        }        

        #region Touch Process
        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            return base.DispatchTouchEvent(ev);
        }
        public CCore.Camera.CameraController FreeCameraController
        {
            get;
            protected set;
        }
        float mStartX;
        float mStartY;
        int mActivePointerId;
        System.Collections.Generic.Queue<CCore.MsgProc.IMsgTranslator.TransResult> mEvents = new System.Collections.Generic.Queue<CCore.MsgProc.IMsgTranslator.TransResult>();
        float GetMotionX(MotionEvent e,int actionIndex)
        {
            if (mGInit.ScaleUIWidthViewTarget)
                return e.GetX(actionIndex) * mGInit.ViewTarget.MRTScale;
            else
                return e.GetX(actionIndex);
        }
        float GetMotionY(MotionEvent e, int actionIndex)
        {
            if (mGInit.ScaleUIWidthViewTarget)
                return e.GetY(actionIndex) * mGInit.ViewTarget.MRTScale;
            else
                return e.GetY(actionIndex);
        }

        public override bool OnGenericMotionEvent(MotionEvent e)// joystick movements, mouse hovers, track pad touches, scroll wheel movements and other input events(来自官方文档)
        {
            switch ((int)e.Action)
            {
                case (int)MotionEventActions.Move:
                    {

                    }
                    break;
                case (int)MotionEventActions.HoverMove:
                    {

                    }
                    break;
                case 0x00000107:
                    {

                    }
                    break;
                case 0x00000207:
                    {

                    }
                    break;
            }
            return base.OnGenericMotionEvent(e);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            mGesture.OnTouchEvent(e);
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    {
                        mStartX = GetMotionX(e, e.ActionIndex);
                        mStartY = GetMotionY(e, e.ActionIndex);
                        mActivePointerId = e.GetPointerId(e.ActionIndex);

                        var _init = new CCore.MsgProc.Behavior.Mouse_Key();
                        _init.X = (int)(mStartX);
                        _init.Y = (int)(mStartY);
                        var pre = e.Pressure;
                        _init.BehaviorId = mActivePointerId;
                        _init.behavior = CCore.MsgProc.BehaviorType.BHT_LB_Down;
                        var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                        tr.BeType = _init.GetBehaviorType();
                        tr.BeInit = _init;

                        mEvents.Enqueue(tr);
                        //  Log.FileLog.WriteLine(string.Format("------Down------,e.ActionIndex : {0}, PointerId : {1}", e.ActionIndex, mActivePointerId));
                        //CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch(tr.BeType, tr.BeInit);
                    }
                    break;
                case MotionEventActions.Move:
                    {
                        //                         if (mActivePointerId == MotionEvent.InvalidPointerId)
                        //                             break;
                        for(int acindex=0;  acindex <e.PointerCount; acindex++)
                        {
                            var pointerId = e.GetPointerId(acindex);
                            //                         if (pointerIndex < 0)
                            //                             break;
                            float x = GetMotionX(e, acindex);
                            float y = GetMotionY(e, acindex);
                            var deltaX = x - mStartX;
                            var deltaY = y - mStartY;
                            if (FreeCameraController != null)
                            {
                                //FreeCameraController.Turn(CCore.CoordAxis.Y, deltaX * 0.015f);
                                //FreeCameraController.Turn(CCore.CoordAxis.X, deltaY * 0.015f);
                            }
                            mStartX = x;
                            mStartY = y;
                            var _init = new CCore.MsgProc.Behavior.Mouse_Move();
                            _init.X = (int)x;
                            _init.Y = (int)y;

                            _init.BehaviorId = pointerId;
                            _init.button = CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left;

                            var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                            tr.BeType = _init.GetBehaviorType();
                            tr.BeInit = _init;

                            mEvents.Enqueue(tr);
                        }
                        //      Log.FileLog.WriteLine(string.Format("------Move-------,e.ActionIndex_{0}, PointerId_{1}", e.ActionIndex, pointerId));
                    }
                    break;

                case MotionEventActions.Up:
                    {
                        mActivePointerId = MotionEvent.InvalidPointerId;
                        var currPointerId = e.GetPointerId(e.ActionIndex);
                        if (currPointerId == mActivePointerId) // 当前处理的
                        {
                            var newPointerIndex = e.ActionIndex == 0 ? 1 : 0;
                            mStartX = GetMotionX(e, newPointerIndex);
                            mStartY = GetMotionY(e, newPointerIndex);
                        }

                        var _init = new CCore.MsgProc.Behavior.Mouse_Key();
                        _init.X = (int)mStartX;
                        _init.Y = (int)mStartY;
                        var pre = e.Pressure;
                        _init.BehaviorId = currPointerId;
                        _init.behavior = CCore.MsgProc.BehaviorType.BHT_LB_Up;
                        var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                        tr.BeType = _init.GetBehaviorType();
                        tr.BeInit = _init;

                        mEvents.Enqueue(tr);
                        //Log.FileLog.WriteLine(string.Format("-----Up------,e.ActionIndex :  {0}, PointerId : {1}", e.ActionIndex, currPointerId));
                    }
                    break;
                case MotionEventActions.Cancel:
                    mActivePointerId = MotionEvent.InvalidPointerId;
                    break;
                case MotionEventActions.Pointer1Down://第二个触控
                    {
                        mStartX = GetMotionX(e, e.ActionIndex);
                        mStartY = GetMotionY(e, e.ActionIndex);
                        mActivePointerId = e.GetPointerId(e.ActionIndex);

                        var _init = new CCore.MsgProc.Behavior.Mouse_Key();
                        _init.X = (int)(mStartX);
                        _init.Y = (int)(mStartY);
                        var pre = e.Pressure;
                        _init.BehaviorId = mActivePointerId;
                        _init.behavior = CCore.MsgProc.BehaviorType.BHT_LB_Down;
                        var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                        tr.BeType = _init.GetBehaviorType();
                        tr.BeInit = _init;

                        mEvents.Enqueue(tr);
                        //Log.FileLog.WriteLine(string.Format("-------Pointer1Down-----,e.ActionIndex : {0}, PointerId : {1}", e.ActionIndex, mActivePointerId));
                        break;
                    }
                case MotionEventActions.PointerUp:
                    {
                        var currPointerId = e.GetPointerId(e.ActionIndex);
                        if (currPointerId == mActivePointerId) // 当前处理的
                        {
                            var newPointerIndex = e.ActionIndex == 0 ? 1 : 0;
                            mStartX = GetMotionX(e, newPointerIndex);
                            mStartY = GetMotionY(e, newPointerIndex);
                            mActivePointerId = e.GetPointerId(newPointerIndex);
                        }

                        var _init = new CCore.MsgProc.Behavior.Mouse_Key();
                        _init.X = (int)mStartX;
                        _init.Y = (int)mStartY;
                        var pre = e.Pressure;
                        _init.BehaviorId = currPointerId;
                        _init.behavior = CCore.MsgProc.BehaviorType.BHT_LB_Up;
                        var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                        tr.BeType = _init.GetBehaviorType();
                        tr.BeInit = _init;

                        mEvents.Enqueue(tr);
                        //    Log.FileLog.WriteLine(string.Format("------PointerUp------,e.ActionIndex_{0}, PointerId_{1}", e.ActionIndex, currPointerId));
                        //CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch(tr.BeType, tr.BeInit);
                    }
                    break;
                case MotionEventActions.Pointer2Down://第二个触控
                    {
                        mStartX = GetMotionX(e, e.ActionIndex);
                        mStartY = GetMotionY(e, e.ActionIndex);
                        mActivePointerId = e.GetPointerId(e.ActionIndex);

                        var _init = new CCore.MsgProc.Behavior.Mouse_Key();
                        _init.X = (int)mStartX;
                        _init.Y = (int)mStartY;
                        var pre = e.Pressure;
                        _init.BehaviorId = mActivePointerId;
                        _init.behavior = CCore.MsgProc.BehaviorType.BHT_Pointer2Down;
                        var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                        tr.BeType = _init.GetBehaviorType();
                        tr.BeInit = _init;

                        mEvents.Enqueue(tr);
                        //Log.FileLog.WriteLine(string.Format("------Pointer2Down-------,e.ActionIndex : {0}, PointerId : {1}", e.ActionIndex, mActivePointerId));
                    }
                    break;
                case MotionEventActions.Pointer2Up://第二个触控0
                    {
                        mStartX = GetMotionX(e, e.ActionIndex);
                        mStartY = GetMotionY(e, e.ActionIndex);
                        mActivePointerId = e.GetPointerId(e.ActionIndex);

                        var _init = new CCore.MsgProc.Behavior.Mouse_Key();
                        _init.X = (int)mStartX;
                        _init.Y = (int)mStartY;

                        _init.BehaviorId = mActivePointerId;
                        _init.behavior = CCore.MsgProc.BehaviorType.BHT_Pointer2Up;
                        var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                        tr.BeType = _init.GetBehaviorType();
                        tr.BeInit = _init;

                        mEvents.Enqueue(tr);
                       // Log.FileLog.WriteLine(string.Format("--------Pointer2Up-------,e.ActionIndex_{0}, PointerId_{1}", e.ActionIndex,mActivePointerId));
                    }
                    break;
                case MotionEventActions.Pointer3Down://第三个个触控
                    {
                        mStartX = GetMotionX(e, e.ActionIndex);
                        mStartY = GetMotionY(e, e.ActionIndex);
                        mActivePointerId = e.GetPointerId(e.ActionIndex);

                        var _init = new CCore.MsgProc.Behavior.Mouse_Key();
                        _init.X = (int)mStartX;
                        _init.Y = (int)mStartY;
                        var pre = e.Pressure;
                        _init.BehaviorId = mActivePointerId;
                        _init.behavior = CCore.MsgProc.BehaviorType.BHT_Pointer3Down;
                        var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                        tr.BeType = _init.GetBehaviorType();
                        tr.BeInit = _init;

                        mEvents.Enqueue(tr);
                       // Log.FileLog.WriteLine(string.Format("-------Pointer3Down-------,e.ActionIndex_{0}, PointerId_{1}", e.ActionIndex, mActivePointerId));
                    }
                    break;
                case MotionEventActions.Pointer3Up://第三个触控
                    {
                        mStartX = GetMotionX(e, e.ActionIndex);
                        mStartY = GetMotionY(e, e.ActionIndex);
                        mActivePointerId = e.GetPointerId(e.ActionIndex);

                        var _init = new CCore.MsgProc.Behavior.Mouse_Key();
                        _init.X = (int)mStartX;
                        _init.Y = (int)mStartY;
                        var pre = e.Pressure;
                        _init.BehaviorId = mActivePointerId;
                        _init.behavior = CCore.MsgProc.BehaviorType.BHT_Pointer3Up;
                        var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
                        tr.BeType = _init.GetBehaviorType();
                        tr.BeInit = _init;

                        mEvents.Enqueue(tr);
                       // Log.FileLog.WriteLine(string.Format("------Pointer3Up------,e.ActionIndex_{0}, PointerId_{1}", e.ActionIndex, mActivePointerId));
                    }
                    break;
            }

            if((int)e.Action ==0x00000107)
            {

            }
            if ((int)e.Action == 0x0000202)
            {

            }
            return base.OnTouchEvent(e);
        }

        public bool OnDown(MotionEvent e)
        {
            return false;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            // 当用户在屏幕上“拖动”时触发该方法 
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {
            // 当用户在屏幕上长按时触发该方法 
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            // 当屏幕“滚动”时触发该方法 
            return false;
        }

        public void OnShowPress(MotionEvent e)
        {
            // 当用户在触摸屏幕上按下、而且还未移动和松开时触发该方法   
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            // 在屏幕上的轻击事件将会触发该方法 
            return false;
        }

        private void ProcTouchEvents()
        {
            while (mEvents.Count>0)
            {
                var e = mEvents.Dequeue();
                CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch(e.BeType, e.BeInit);
            }
        }
        #endregion
    }
}

