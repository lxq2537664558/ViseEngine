using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public partial class Game
    {
        public CCore.World.WorldRenderParam RenderParam
        {
            get;
        } = new CCore.World.WorldRenderParam();
        
        CCore.Graphics.REnviroment mREnviroment;
        public CCore.Graphics.REnviroment REnviroment
        {
            get { return mREnviroment; }
        }


     //   CCore.Mesh.Mesh mTestMesh = null;

        private void FinalUI()
        {
            Title.HitShowManager.FinalInstance();
            Title.TitleShowManager.FinalInstance();
            
            RootUIMsg = null;
            CCore.Support.ReflectionManager.Instance.Cleanup();
            UISystem.IRender.GetInstance().Cleanup();

            CCore.Support.ReflectionManager.FinalInstance();

            UISystem.Device.Mouse.Instance.Cleanup();
            UISystem.Device.Keyboard.Instance.Cleanup();
        }

        private bool InitRenderEnv(CCore.Graphics.ViewTarget vt)
        {
            var _reInit = new CCore.Graphics.REnviromentInit();
            _reInit.ViewInit = new CCore.Graphics.ViewInit();
            _reInit.ViewInit.ViewWnd = vt;
            _reInit.bUseIntZ = Client.AppConfig.Instance.UseIntZ;
            _reInit.bMainScene = true;
            var view = new CCore.Graphics.View();
            view.Initialize(_reInit.ViewInit);
            mREnviroment = new CCore.Graphics.REnviroment();
            if (mREnviroment.Initialize(_reInit, view) == false)
                return false;

            mREnviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.Black));
            mREnviroment.BeforeCopyTexture += BeforeRender2View;
            mREnviroment.AfterRender2View += AfterRender2View;
            mREnviroment.View.ViewSizeChanged += ViewSizeChanged;

            RenderParam.Enviroment = mREnviroment;

            if (GInit.ScaleUIWidthViewTarget)
            {
                RootUIMsg.Root.Width = (int)((float)mREnviroment.View.Width * vt.MRTScale);
                RootUIMsg.Root.Height = (int)((float)mREnviroment.View.Height * vt.MRTScale);
            }
            else
            {
                RootUIMsg.Root.Width = mREnviroment.View.Width;
                RootUIMsg.Root.Height = mREnviroment.View.Height;
            }

            CCore.Camera.CameraAnimationManager.Instance.SetCamera(mREnviroment.Camera);                

            CCore.Client.MainWorldInstance.PostProceses.Clear();

            //var toneMapping = new CCore.Graphics.PostProcess_ToneMapping();
            //toneMapping.Enable = true;
            //toneMapping.W = 1.0f;
            //toneMapping.BrightFactor = 1.0f;
            //CCore.Client.MainWorldInstance.PostProceses.Add(toneMapping);

            //var bloom = new CCore.Graphics.PostProcess_Bloom();
            //bloom.Enable = true;
            //bloom.BloomImageScale = 1.0f;
            //bloom.BlurStrength = 1.0f;
            //bloom.BlurAmount = 5;
            //bloom.BlurType = CCore.Graphics.enBlurType.BoxBlur;
            //CCore.Client.MainWorldInstance.PostProceses.Add(bloom);

            //var colorGrading = new CCore.Graphics.PostProcess_ColorGrading();
            //colorGrading.Enable = true;
            //colorGrading.ColorGradingTexName = "texture/Colorgrading/OriginalRGBTable16x1.png";
            //colorGrading.GammaCorrect = true;
            //CCore.Client.MainWorldInstance.PostProceses.Add(colorGrading);


            StartParallThread();
            //StartReadHitProxyThread();

            return true;
        }

        private void FinalRenderEnv()
        {
            StopParallThread();
            //StopReadHitProxyThread();

            mREnviroment.BeforeCopyTexture -= BeforeRender2View;
            mREnviroment.AfterRender2View -= this.AfterRender2View;
            mREnviroment.View.ViewSizeChanged -= this.ViewSizeChanged;
        }

        int DPCountUI = 0;
        private void BeforeRender2View(CCore.Graphics.REnviroment env)
        {
            if (GInit.ScaleUIWidthViewTarget)
            {
                var dpCount1 = CCore.Engine.Instance.Client.Graphics.GetDPCount();
                UISystem.IRender.GetInstance().UIRenderer.CommitDrawCall(mUIRenderPipe);
                var dpCount2 = CCore.Engine.Instance.Client.Graphics.GetDPCount();
                DPCountUI = dpCount2 - dpCount1;
            }
        }
        private void AfterRender2View(CCore.Graphics.REnviroment env)
        {
            if (GInit.ScaleUIWidthViewTarget == false)
            {
                var dpCount1 = CCore.Engine.Instance.Client.Graphics.GetDPCount();
                UISystem.IRender.GetInstance().UIRenderer.CommitDrawCall(mUIRenderPipe);
                var dpCount2 = CCore.Engine.Instance.Client.Graphics.GetDPCount();
                DPCountUI = dpCount2 - dpCount1;
            }
        }


        System.Threading.Thread mParallThread;
        bool mIsParallally = false;
        System.Threading.AutoResetEvent mParallelBeginEvent = new System.Threading.AutoResetEvent(false);
        System.Threading.AutoResetEvent mParallelEndEvent = new System.Threading.AutoResetEvent(false);
        private void StartParallThread()
        {
            CCore.Engine.IsMultiThreadRendering = Client.AppConfig.Instance.MTRendering;
            if (CCore.Engine.IsMultiThreadRendering)
            {
                mParallThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ParallelThreadProc));
                mIsParallally = true;
                IsParallelFinished = false;
                mParallThread.Name = "ParallThread";
                mParallThread.Start();
            }
        }

        private void StopParallThread()
        {
            if (CCore.Engine.IsMultiThreadRendering)
            {
                mIsParallally = false;
                mParallelBeginEvent.Set();

                while(IsParallelFinished==false)
                {
                    System.Threading.Thread.Sleep(50);
                }
            }
        }

        bool IsParallelFinished = false;
        private void ParallelThreadProc()
        {
            while (mIsParallally)
            {
                mParallelBeginEvent.WaitOne();
                mParallelBeginEvent.Reset();

                try
                {
                    LogicTick();
                    //RenderTick();
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                }

                mParallelEndEvent.Set();
            }
            IsParallelFinished = true;
        }

        static CSUtility.Performance.PerfCounter mRenderTickTimer = new CSUtility.Performance.PerfCounter("IGame.RenderTick");
        static CSUtility.Performance.PerfCounter mLogicTickTimer = new CSUtility.Performance.PerfCounter("IGame.LogicTick");
        static CSUtility.Performance.PerfCounter mSyncTickTimer = new CSUtility.Performance.PerfCounter("IGame.SyncTick");
        static CSUtility.Performance.PerfCounter mCommitUITimer = new CSUtility.Performance.PerfCounter("IGame.CommitUI");

        static CSUtility.Performance.PerfCounter TickUITimer = new CSUtility.Performance.PerfCounter("IGame.TickUI");

        //         private int mHitLagTime = 500;
        //         bool mNeedReadHitProxy = false;
        //         bool mReadingHitProxy = false;

        private long mUpdateDebugTexturesDuration = 500;
        
        int RenderTickStep = 0;
        void RenderTick()
        {
            if(IsProgress)
            {
                _RenderProgress();
                return;
            }
            RenderTickStep = 0;
            if (IsProgress)
                return;

            var elapsedMillisecond = CCore.Engine.Instance.GetElapsedMillisecond();

            mRenderTickTimer.Begin();
            if (CurrentStage!=null)
                CurrentStage.RenderThreadTick(this);
            RenderTickStep = 1;

            if (CCore.Engine.Instance.EnableRenderTick && mREnviroment != null)
            {
                CCore.Engine.Instance.Client.Graphics.BeginDraw();

                _RenderShadow();
                RenderTickStep = 2;
                mREnviroment.RefreshPostProcess(CCore.Client.MainWorldInstance.PostProceses);
                RenderTickStep = 3;
                mREnviroment.Render();
                RenderTickStep = 4;
                
                CCore.Engine.Instance.Client.Graphics.EndDraw();

                mUpdateDebugTexturesDuration -= elapsedMillisecond;
                if(mUpdateDebugTexturesDuration<0)
                {
                    mUpdateDebugTexturesDuration = 500;
                    mREnviroment.UpdateDebugTextures();
                }
            }

            mRenderTickTimer.End();
        }
        
        CSUtility.Performance.PerfCounter RShadowCounter = new CSUtility.Performance.PerfCounter("IGame.RShadow");
        int ShadowDP = 0;
        int ShadowTri = 0;
        void _RenderShadow()
        {
            RShadowCounter.Begin();
            var dp1 = CCore.Engine.Instance.Client.Graphics.GetDPCount();
            var tri1 = CCore.Engine.Instance.Client.Graphics.GetDrawTriangleCount();
            if (CCore.Client.MainWorldInstance != null)
            {
                CCore.Client.MainWorldInstance.RenderShadow(RenderParam);
            }
            var dp2 = CCore.Engine.Instance.Client.Graphics.GetDPCount();
            var tri2 = CCore.Engine.Instance.Client.Graphics.GetDrawTriangleCount();
            ShadowDP = dp2 - dp1;
            ShadowTri = tri2 - tri1;
            RShadowCounter.End();
        }
        
    }
}
 