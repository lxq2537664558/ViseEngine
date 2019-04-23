using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public partial class Game
    {
        public bool IsProgress
        {
            get;
            protected set;
        } = false;
        bool ProgressRenderStart = false;

        public delegate void FOnAsyncExec();
        FOnAsyncExec ProgressBackGroundExec = null;
        public bool ProgressBackMission(FOnAsyncExec exec,bool bImm)
        {
            if(bImm)
            {
                exec();
                return true;
            }
            if (IsProgress == true)
                return false;

            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("LoadingUI") as UISystem.WinForm;
            if (form != null)
            {
                form.WinName = "LoadingUI";
                form.Parent = null;
                form.Parent = Game.Instance.RootUIMsg.Root;
                form.UpdateLayout(true);
            }

            IsProgress = true;
            ProgressRenderStart = false;
            ProgressBackGroundExec = exec;
            var thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ProgressBackProc));
            thread.Start();
            return true;
        }
        private void TryHideProgressUI()
        {
            if (ProgressRenderStart == true)
            {
                var loadingNumber = CCore.Engine.Instance.GetAsyncLoadNumber();
                if (loadingNumber == 0)
                {
                    var form = CCore.Support.ReflectionManager.Instance.GetUIForm("LoadingUI") as UISystem.WinForm;
                    if (form != null)
                        form.Visibility = UISystem.Visibility.Collapsed;
                    ProgressRenderStart = false;
                }
                else
                {
                    var form = CCore.Support.ReflectionManager.Instance.GetUIForm("LoadingUI") as UISystem.WinForm;
                    if (form != null)
                    {
                        form.Parent = null;
                        form.Parent = Game.Instance.RootUIMsg.Root;
                    }
                }
            }
        }
        void ProgressBackProc()
        {
            while (ProgressRenderStart == false)
            {
                System.Threading.Thread.Sleep(30);
            }
            ProgressBackGroundExec();
            IsProgress = false;
            //ProgressRenderStart = false;

            //var form = CCore.Support.ReflectionManager.Instance.GetUIForm("LoadingUI") as UISystem.WinForm;
            //if (form != null)
            //    form.Visibility = UISystem.Visibility.Collapsed;
        }

        void _RenderProgress()
        {
            if(ProgressRenderStart == false)
            {
                ProgressRenderStart = true;
            }
            //System.Diagnostics.Debug.WriteLine("Progress");

            var elapsedMillisecond = CCore.Engine.Instance.GetElapsedMillisecond();
            RootUIMsg.Root.Tick(elapsedMillisecond);

            var mat = SlimDX.Matrix.Identity;
            RootUIMsg.Root.Draw(mUIRenderPipe, 0, ref mat);

            CCore.Engine.Instance.Client.Graphics.BeginDraw();
            mREnviroment.RenderOnlyUI();
            CSUtility.FileDownload.FileDownloadManager.Instance.Tick();

            CCore.Engine.Instance.Client.Graphics.EndDraw();
        }
    }
}
