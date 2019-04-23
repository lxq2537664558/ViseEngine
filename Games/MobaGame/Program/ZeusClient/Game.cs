using GameData.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class GameInit
    {
        public bool ScaleUIWidthViewTarget = false;
        public CCore.Graphics.ViewTarget ViewTarget;
        public CCore.Graphics.EDeviceType DeviceType;
        public string AndroidPackageName;

        public float Vector2ScreenCoordScale
        {
            get
            {
                if(ScaleUIWidthViewTarget)
                {
                    return 1.0F;
                }
                else
                {
                    return ViewTarget.MRTScale;
                }
            }
        }
    }

    public partial class Game : CCore.MsgProc.MsgReceiver
    {
        public static Game Instance
        {
            get;
        } = new Game();

        public RootUIMsgReceiver RootUIMsg
        {
            set;
            get;
        } = new RootUIMsgReceiver();

        private Game()
        {
            if (CSUtility.Program.CurrentPlatform == CSUtility.enPlatform.Windows)
                CCore.Engine.EnableThreadLockInfo = false;
            else
                CCore.Engine.EnableThreadLockInfo = false;
        }

        public bool IsEditorMode
        {
            get { return CCore.Engine.Instance.IsEditorMode; }
            set
            {
                CCore.Engine.Instance.IsEditorMode = value;                
            }
        }

        public GameInit GInit;
        public bool Start(GameInit init)
        {
            CSUtility.Program.LogInfo("InitGame 2");
            GInit = init;
            //别在这个前面放代码了，这个才能保证路径构造顺序
            CSUtility.Support.IFileManager.Instance.Initialize(init.AndroidPackageName);            

            #region 编辑器内容初始化

            Role.RoleManager.Instance.InitRoleActionNames();
            #endregion

            GameData.Support.ConfigFile.Instance.LoadFile();
            AppConfig.Instance.LoadFile();
            //System.Net.HttpWebRequest.Create("http://www.etangonline.com/a.txt");
            
            InitializeDownloading();

            // 工厂初始化
            CCore.World.WorldInitFactory.Instance = new World.WorldInitFactory();
            CSUtility.Data.RoleTemplateInitFactory.Instance = new GameData.InitFactory.RoleTemplateInitFactory();
            CSUtility.Map.Role.NPCDataInitFactory.Instance = new GameData.InitFactory.NPCDataInitFactory();

            CSUtility.Support.ClassInfoManager.Instance.Load(AppConfig.Instance.FinalRelease);

            if (InitEngine(init.ViewTarget, init.DeviceType) == false)
                return false;

            CCore.Engine.Instance.Client.Graphics.SetOnGLError(OnGLError);
            CCore.Engine.Instance.Client.Graphics.SetOnAsyncLoadObject(OnAsyncLoadObject);
            CCore.Engine.Instance.Client.Graphics.StartIOThread();

            CSUtility.Program.LogInfo("InitGame 3");
            //网络启动
            GameRPC.Instance.StartNetWork();            
            GameRPC.Instance.Login("", "");

            CSUtility.Program.LogInfo("InitGame 6");
            // 消息处理注册
            CCore.Engine.Instance.Client.MsgRecieverMgr.RegReciever(this);            
            CCore.Engine.Instance.Client.MsgRecieverMgr.RegReciever(RootUIMsg);

            // 音乐初始化
            CCore.Audio.AudioManager.Instance.Initialize();

            CSUtility.Program.LogInfo("InitGame 7");
            #region AI
            if (CSUtility.Program.CurrentPlatform == CSUtility.enPlatform.Windows)
            {
                var mAIEditorAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Plugins/ResourcesBrowser/bin/AIEditor.dll");
                var aiType = mAIEditorAssembly?.GetType("AIEditor.Program");
                var aiMethod = aiType?.GetMethod("CompileAICodeWithAIGuid");
                if (aiMethod != null)
                {
                    CSUtility.AISystem.FStateMachineTemplate.OnCompileFSMCode =
                        (Guid id, int csType, bool bForceCompile) =>
                        {
                            return (bool)aiMethod.Invoke(null, new object[] { id, csType, bForceCompile,true,"" });
                        };
                }
            }

            CSUtility.AISystem.FSMTemplateVersionManager.Instance.Load(CSUtility.Helper.enCSType.Client);
            var fsms = CSUtility.AISystem.FSMTemplateVersionManager.Instance.FSMTemplateVersionDictionary;
            foreach (var i in fsms.Keys)
            {
                CSUtility.AISystem.FStateMachineTemplateManager.Instance.GetFSMTemplate(i, CSUtility.Helper.enCSType.Client, true);
            }
            #endregion

            CSUtility.Program.LogInfo("InitGame 8");
            #region DelegateMethod
            if (CSUtility.Program.CurrentPlatform == CSUtility.enPlatform.Windows)
            {
                var mDelegateMethodAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Plugins/DelegateMethodEditor/bin/DelegateMethodEditor.dll");
                var delMethodType = mDelegateMethodAssembly?.GetType("DelegateMethodEditor.CodeGenerator.CodeGenerator");
                var delMethod = delMethodType?.GetMethod("CompileEventCodeWithEventId");
                if (delMethod != null)
                {
                    CSUtility.Helper.EventCallBackManager.Instance.OnCompileEventCode =
                        (Guid id, int csType, bool bForceCompile) =>
                        {
                            return (bool)delMethod.Invoke(null, new object[] { id, csType, bForceCompile, true, "" });
                        };
                }
            }

            CSUtility.Helper.EventCallBackManager.Instance._SetCSType(CSUtility.Helper.enCSType.Client);
            CSUtility.Helper.EventCallBackVersionManager.Instance.Load(CSUtility.Helper.enCSType.Client);
            var events = CSUtility.Helper.EventCallBackVersionManager.Instance.EventCallBackVersionDictionary;
            foreach (var i in events.Keys)
            {
                CSUtility.Helper.EventCallBackManager.Instance.LoadCallee(i, false);
            }
            #endregion            

            CSUtility.Program.LogInfo("InitGame 9");
            CCore.MsgProc.MsgReceiver.ForceAsyncBehavior = true;
            CCore.MsgProc.MsgReceiver.MainThread = System.Threading.Thread.CurrentThread;

            //测试代码///////////////////////////////////////////////////////////////////////////////////
            var uiForm = CCore.Support.ReflectionManager.Instance.GetUIForm("Login") as UISystem.WinForm;
            //var uiForm = CCore.Support.ReflectionManager.Instance.GetUIForm("LoadMap") as UISystem.WinForm;
            if (uiForm != null)
            {
                uiForm.Parent = RootUIMsg.Root;
            }

            //if(CSUtility.Program.CurrentPlatform==CSUtility.enPlatform.Windows)
            //{
            //    CCore.Engine.Instance.SetPreUseForceMode(false);
            //}
            //else
            //{
            //    CCore.Engine.Instance.SetPreUseForceMode(false);
            //}

            CSUtility.Program.LogInfo("InitGame 10");
            //InitOutputUI();
            InitUI();
            /////////////////////////////////////////////////////////////////////////////////////////////                                    
            mHealthWatcher = new System.Threading.Thread(new System.Threading.ThreadStart(this.HealthWatcherProc));
            HealthWatchRun = true;
            mHealthWatcher.Start();
            IsEditorMode = true;
            CSUtility.Program.LogInfo("InitGame 11");

            // 设置寻路动态阻挡获取服务器阻挡值的远程调用方法
            CCore.World.DynamicBlockActor.LoadSceneDataRPCAction = new Action<CCore.World.DynamicBlockActor>((CCore.World.DynamicBlockActor dbActor) =>
            {
                var dbActorInit = dbActor.ActorInit as CSUtility.Map.DynamicBlock.DynamicBlockActorInit;
                if (dbActorInit == null)
                    return;
                RPC.PackageWriter pkg = new RPC.PackageWriter();
                Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_GetDynamicBlockValue(pkg, dbActorInit.DynamicBlockData.Id);
                pkg.WaitDoClient2PlanesPlayer(CCore.Engine.Instance.Client.GateSvrConnect, -1).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
                {
                    if (bTimeOut)
                        return;

                    sbyte successed = 0;
                    _io.Read(out successed);

                    if (successed == 1)
                    {
                        bool tempIsBlock;
                        _io.Read(out tempIsBlock);
                        dbActor.IsBlock = tempIsBlock;
                    }
                };
            });

            //CCore.Engine.BeginWatchNativeMem();

            return true;
        }
        public void Stop()
        {
            FinalDownloading();

            CCore.Engine.Instance.Client.Graphics.EndIOThread();
            
            CCore.Engine.EndWatchNativeMem();

            GameRPC.Instance.StopNetWork();
            IsEditorMode = false;
            HealthWatchRun = false;

            FinalRenderEnv();

            RPC.RPCNetworkMgr.Instance.ClearWaitHandles();

            if (mREnviroment != null)
            {
                mREnviroment.Cleanup();
                mREnviroment = null;
            }

            CCore.Engine.Instance.Client.MsgRecieverMgr.UnRegReciever(this);
            CCore.Engine.Instance.Client.MsgRecieverMgr.UnRegReciever(RootUIMsg);

            if (CCore.Engine.Instance.Client != null)
            {
                CCore.Engine.Instance.Client.MsgRecieverMgr.UnRegReciever(this);
                CCore.Engine.Instance.Client.MsgRecieverMgr.UnRegReciever(RootUIMsg);
                CCore.Engine.Instance.Client.MsgRecieverMgr.Cleanup();
            }
            
            CCore.Engine.Instance.Cleanup();

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            Log.FileLog.Instance.End();
        }
        public void Tick()
        {
            Int64 TryFreeResourceTime = 10 * 1000;
            CCore.Engine.Instance.Client.MsgRecieverMgr.Tick();
            GameRPC.Instance.TickNetWork();
            CSUtility.FileDownload.FileDownloadManager.Instance.Tick();
            //下面两段代码，应该多线程并行处理提升效率
            {
                MainThreadTick();
                CCore.Program.TickTryFreeResource(ref TryFreeResourceTime, 50 * 1024 * 1024, 90 * 1000, 1024 * 1024,
                                                        20 * 1024 * 1024, 90 * 1000, 1024 * 1024);
                //CCore.Program.TickTryFreeResource(ref TryFreeResourceTime, 0, 90 * 1000, 1024 * 1024,
                //                                        0, 90 * 1000, 1024 * 1024);
            }

        }
        public bool OnPause()
        {
            CCore.Engine.Instance.Client.Graphics.PauseIOThread();
            //Client.Game.Instance.REnviroment.SetView(null);
            PauseSpy = true;
            CCore.Engine.Instance.Client.Graphics.InvalidateDeviceResource();
            CCore.Engine.Instance.Client.Graphics.BeforeDeviceReset();

            return true;
        }
        public bool OnResume()
        {
            PauseSpy = false;
            CCore.Engine.Instance.Client.Graphics.InvalidateDeviceResource();
            if (CCore.Engine.Instance.Client.Graphics.TryDeviceReset(GInit.ViewTarget.Handle) == false)
                return false;

            CCore.Engine.Instance.Client.Graphics.RestoreDeviceResource();
            //var vinit = new CCore.Graphics.ViewInit();
            //vinit.ViewWnd = Client.Game.Instance.GInit.ViewTarget;
            //vinit.DSFormat = CCore.BufferFormat.D3DFMT_UNKNOWN;
            //vinit.Format = CCore.BufferFormat.D3DFMT_UNKNOWN;

            //var view = new CCore.Graphics.View();
            //view.Initialize(vinit);

            //Client.Game.Instance.REnviroment.SetView(view);

            CCore.Engine.Instance.Client.Graphics.ResumeIOThread();
            return true;
        }
        private bool InitEngine(CCore.Graphics.ViewTarget vTarget, CCore.Graphics.EDeviceType deviceType)
        {
            var _init = new CCore.EngineInit();
            _init.ClientInit = new CCore.ClientInit();
            _init.ClientInit.GraphicsInit = new CCore.Graphics.GraphicsInit();
            _init.ClientInit.GraphicsInit.hDeviceWindow = vTarget.Handle;
            _init.ClientInit.MsgRecieverMgrInit = new CCore.MsgProc.MsgRecieverMgrInit();
            _init.ClientInit.GraphicsInit.DeviceType = deviceType;
            if(CCore.Engine.Instance.Initialize(_init) == false)
            {
                //System.Windows.Forms.MessageBox.Show("显卡不兼容，游戏启动失败！");
                System.Environment.Exit(0);
            }

            UISystem.WinRoot.MainForm = vTarget;

            return InitRenderEnv(vTarget);
        }

        public void ViewSizeChanged(CCore.Graphics.
            View view, int width, int height)
        {
            if(GInit.ScaleUIWidthViewTarget)
            {
                RootUIMsg.Root.Width = (int)((float)width * this.GInit.ViewTarget.MRTScale);
                RootUIMsg.Root.Height = (int)((float)height * this.GInit.ViewTarget.MRTScale);
            }
            else
            {
                RootUIMsg.Root.Width = width;
                RootUIMsg.Root.Height = height;
            }
        }

        static CCore.DllImportAPI.Delegate_FOnAsyncLoadObject OnAsyncLoadObject = OnAsyncLoadObjectCallBack;
        private static void OnAsyncLoadObjectCallBack(int count, string classType, string sourceFile)
        {
            //if(classType == "PreUse True")
            //{
            //    if (count > 5000000)
            //        return;
            //}
            //else if (classType == "PreUse False")
            //{
            //    if (count > 5000000)
            //        return;
            //}
            return;
            //if (count == -1)
            //    return;
            //else if (count == -2) //(classType == "AsyncIOThread LoadPipe is empty")
            //    return;
            //else if (count == -3) //(classType == "Resource ref count is zero")
            //    return;
            //else if (count == -4) //(classType == "RestoreObjects Failed")
            //    return;
            //else
            //{
            //    //System.Diagnostics.Debug.WriteLine(string.Format("AsyncLoad {0}:{1}->{2}", count, classType, sourceFile));
            //    return;
            //}   
        }
        static CCore.DllImportAPI.Delegate_FOnGLError OnGLError = OnGLErrorCallBack;
        private static void OnGLErrorCallBack(string file, int line, int e, string info)
        {

        }
        public bool NetworkIsDisconnected = false;
        

        UISystem.UIRenderPipe mUIRenderPipe = new UISystem.UIRenderPipe();
        static CSUtility.Performance.PerfCounter mMainTickTimer = new CSUtility.Performance.PerfCounter("IGame.MTick");
        static CSUtility.Performance.PerfCounter mMainWaitTimer = new CSUtility.Performance.PerfCounter("IGame.MWait");
        void MainThreadTick()
        {
            if(RecordVManager.Instance.mDebugDP)
            {
                Client.Game.Instance.OnPause();
                Client.Game.Instance.OnResume();
                REnviroment.CleanAllCommits();
                UISystem.IRender.GetInstance().UIRenderer.ClearAllCommit(mUIRenderPipe);
                RecordVManager.Instance.mDebugDP = false;
            }
            
            mMainTickTimer.Begin();
            if (CCore.Engine.IsMultiThreadRendering)
            {
                mParallelBeginEvent.Set();

                try
                {
                    //LogicTick();
                    RenderTick();
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }

                mMainWaitTimer.Begin();
                mParallelEndEvent.WaitOne();
                mParallelEndEvent.Reset();
                mMainWaitTimer.End();
            }
            else
            {
                LogicTick();
                RenderTick();
            }

            mSyncTickTimer.Begin();
            CCore.Engine.Instance.SyncTick();

            CCore.Client.MainWorldInstance.SwapShadowPipes();
            
            if (mREnviroment != null)
            {
                mREnviroment.SwapPipe();
                //mREnviroment.ClearAllDrawingCommits();
            }
                
            
            UISystem.IRender.GetInstance().UIRenderer.SwapQueue(mUIRenderPipe);

            mDPCount = CCore.Engine.Instance.Client.Graphics.GetDPCount();
            mTriCount = CCore.Engine.Instance.Client.Graphics.GetDrawTriangleCount();
            mClearCount = CCore.Engine.Instance.Client.Graphics.GetClearCount();

            //System.GC.Collect();
            mSyncTickTimer.End();

            FrameCount++;
            if (FrameCount >= 20)
            {
                var graphics = CCore.Engine.Instance.Client.Graphics;
                Int64 nowTime = CCore.Engine.Instance._GetTickCount();
                Fps = (float)(FrameCount * 1000) / (float)(nowTime - PrevTickTime);
                FrameCount = 0;
                PrevTickTime = nowTime;
            }
            mMainTickTimer.End();
        }

        int LogicTickStep = 0;
        static CSUtility.Performance.PerfCounter mLogicEngineTimer = new CSUtility.Performance.PerfCounter("IGame.LTick.Engine");
        static CSUtility.Performance.PerfCounter mLogicRWorldTimer = new CSUtility.Performance.PerfCounter("IGame.LTick.RWorld");
        static CSUtility.Performance.PerfCounter mLogicStageTimer = new CSUtility.Performance.PerfCounter("IGame.LTick.Stage");
        void LogicTick()
        {
            if (IsProgress)
            {
                return;
            }

            LogicTickStep = 0;
            //mLogicTickTimer.AvgCounter = int.MaxValue;
            mLogicTickTimer.Begin();
            mREnviroment.ClearAllDrawingCommits();
            LogicTickStep = 1;

            mLogicEngineTimer.Begin();
            CCore.Engine.Instance.Tick(true);
            mLogicEngineTimer.End();
            LogicTickStep = 2;

            CCore.Engine.Instance.CurRenderFrame++;
            //if (!CCore.Client.MainWorldInstance.IsNullWorld)
            // 显示摄像机位置的场景
            mLogicRWorldTimer.Begin();
            CCore.Client.MainWorldInstance.Render2Enviroment(RenderParam);
            mLogicRWorldTimer.End();
            LogicTickStep = 3;

            if (mREnviroment!=null)
            {
                var tvLoc = mREnviroment.Camera.GetLocation();
                CCore.Client.MainWorldInstance.TravelTo(tvLoc.X, tvLoc.Z);

                mREnviroment.Tick();
            }
            LogicTickStep = 4;

            if (Client.Stage.MainStage.Instance.ChiefRole != null)
            {
                CCore.WeatherSystem.IlluminationManager.Instance.Tick(null, Client.Stage.MainStage.Instance.ChiefRole.Placement.GetLocation());
            }
            CSUtility.FileDownload.FileDownloadManager.Instance.Tick();
            LogicTickStep = 5;

            mLogicStageTimer.Begin();
            CCore.Audio.AudioManager.Instance.Tick();
            CurrentStage?.Tick(this);
            mLogicStageTimer.End();
            LogicTickStep = 6;

            TickUITimer.Begin();
            var elapsedMillisecond = CCore.Engine.Instance.GetElapsedMillisecond();
            RootUIMsg.Root.Tick(elapsedMillisecond);
            TickUITimer.End();
            LogicTickStep = 7;

            mCommitUITimer.Begin();
            var mat = SlimDX.Matrix.Identity;
            RootUIMsg.Root.Draw(mUIRenderPipe, 0, ref mat);
            mCommitUITimer.End();
            LogicTickStep = 8;

            mLogicTickTimer.End();

            UpdateDebugInfo();
            LogicTickStep = 9;

            TryHideProgressUI();
            LogicTickStep = 10;
        }
        
        public override CCore.MsgProc.FBehaviorProcess FindBehavior(CCore.MsgProc.BehaviorParameter bhInit)
        {
            return CurrentStage?.FindBehavior(bhInit);
        }

        #region Stage

        public IStage CurrentStage
        {
            get;
            protected set;
        }
        public void SetCurrentStage(IStage stage)
        {
            if (CurrentStage == stage)
                return;
            if (CurrentStage != null)
                CurrentStage.Leave(this);
            CurrentStage = stage;
            if (CurrentStage != null)
                CurrentStage.Enter(this);
        }
        #endregion

        #region Health Spy
        bool HealthWatchRun = false;
        System.Threading.Thread mHealthWatcher;
        UInt32 WatcherPrevFrame;

        bool PauseSpy = false;
        Int64 GCTime = CCore.Engine.Instance._GetTickCount();
        void HealthWatcherProc()
        {
            while (HealthWatchRun)
            {
                if(PauseSpy)
                {
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
                var now = CCore.Engine.Instance._GetTickCount();

                if (now - GCTime > 20000)
                {
                    //System.GC.Collect();
                    GCTime = now;
                }
                System.Threading.Thread.Sleep(1000);
                var curFrame = CCore.Engine.Instance.CurRenderFrame;
                if (curFrame == WatcherPrevFrame)
                {
                    var lockInfo = CCore.Engine.PrintThreadLockInfo();
                    System.Diagnostics.Debug.WriteLine(lockInfo);
                    System.Diagnostics.Debug.WriteLine("HealthWatcherProc!! Render = {0};Logic = {1}", RenderTickStep, LogicTickStep);
                    if (HealthWatchRun == false)
                    {
                        mParallThread.Suspend();
                        var stack = new System.Diagnostics.StackTrace(mParallThread, true);
                        System.Diagnostics.Debug.WriteLine(stack.ToString());
                    }
                }
                WatcherPrevFrame = curFrame;
            }
        }
        #endregion
    }
}