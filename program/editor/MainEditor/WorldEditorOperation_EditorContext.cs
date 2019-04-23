using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace MainEditor
{

    public partial class WorldEditorOperation : CCore.EditorClassBase
    {
        EditorCommon.DragDrop.DragDropManager.TickDragData mDragTickData;
        private void Initialize_GameWindowDragOperation(System.Windows.Forms.Form gameForm)
        {
            mDragTickData = new EditorCommon.DragDrop.DragDropManager.TickDragData();
            mDragTickData.OnDragTick = GameDragTick;
            mDragTickData.OnTickDragDataDragEnter += EditorCommon.WorldEditorOperation._OnGameWindowDragEnter;
            mDragTickData.OnTickDragDataDragLeave += EditorCommon.WorldEditorOperation._OnGameWindowDragLeave;
            mDragTickData.OnTickDragDataDragOver += EditorCommon.WorldEditorOperation._OnGameWindowDragOver;
            mDragTickData.OnTickDragDataDragDrop += EditorCommon.WorldEditorOperation._OnGameWindowDragDrop;
            EditorCommon.DragDrop.DragDropManager.Instance.RegisterDragTickData("Game", mDragTickData);

            gameForm.LocationChanged += GameForm_LocationChanged;
            gameForm.Activated += GameForm_Activated;
            gameForm.Deactivate += GameForm_Deactivate;

            if (MainWindow.Instance.ShowGameAssistWindow && CCore.Engine.Instance.IsEditorMode)
            {
                /*GameAssistWindow.Instance.Show();
                var heightOffset = gameForm.Height - gameForm.ClientSize.Height;
                var widthOffset = gameForm.Width - gameForm.ClientSize.Width;
                GameAssistWindow.Instance.SetPosition(gameForm.Location.X + widthOffset + GameAssistWindow.Instance.Offset.X,
                                                      gameForm.Location.Y + heightOffset + GameAssistWindow.Instance.Offset.Y);*/

                GameAssistWindow.Instance.Show();
                var wndHelper = new WindowInteropHelper(GameAssistWindow.Instance);
                wndHelper.Owner = gameForm.Handle;


                //var wndHelper = new WindowInteropHelper(GameAssistWindow.Instance);
                //var oldStyle = DockControl.Win32.GetWindowLong(wndHelper.Handle, DockControl.Win32.GWL_STYLE);
                //DockControl.Win32.SetWindowLong(wndHelper.Handle, DockControl.Win32.GWL_STYLE, oldStyle | DockControl.Win32.WS_CHILD);
                //DockControl.Win32.SetParent(wndHelper.Handle, gameForm.Handle);
                //DockControl.Win32.ShowWindow(wndHelper.Handle, 1);
            }
        }

        //bool mGameFormActivating = false;
        private void GameForm_Deactivate(object sender, EventArgs e)
        {
            /*//var form = sender as System.Windows.Forms.Form;
            //if (mGameFormActivating)
            //{
            //    mGameFormActivating = false;
            //    return;
            //}

            EditorCommon.Win32.POINT pt = new EditorCommon.Win32.POINT();
            EditorCommon.Win32.GetCursorPos(ref pt);
            var handle = DockControl.Program.WindowFromPoint(pt.X, pt.Y);
            var childHelper = new WindowInteropHelper(GameAssistWindow.Instance);
            if (handle != childHelper.Handle)
                GameAssistWindow.Instance.Close();*/
        }

        private void GameForm_Activated(object sender, EventArgs e)
        {
            /*if(!mGameFormActivating)
            {
                if (MainWindow.Instance.ShowGameAssistWindow && CCore.Engine.Instance.IsEditorMode)
                {
                    mGameFormActivating = true;

                    GameAssistWindow.Instance.Show();
                    var form = sender as System.Windows.Forms.Form;
                    var heightOffset = form.Height - form.ClientSize.Height;
                    var widthOffset = form.Width - form.ClientSize.Width;
                    GameAssistWindow.Instance.SetPosition(form.Location.X + widthOffset + GameAssistWindow.Instance.Offset.X,
                                                          form.Location.Y + heightOffset + GameAssistWindow.Instance.Offset.Y);
                }
            }
            mGameFormActivating = false;*/
        }

        private void GameForm_LocationChanged(object sender, EventArgs e)
        {
            if(MainWindow.Instance.ShowGameAssistWindow)
            {
                var form = sender as System.Windows.Forms.Form;
                if(CCore.Engine.Instance.IsEditorMode)
                {
                    if (GameAssistWindow.Instance.Visibility != System.Windows.Visibility.Visible)
                    {
                        GameAssistWindow.Instance.Show();
                        var wndHelper = new WindowInteropHelper(GameAssistWindow.Instance);
                        wndHelper.Owner = form.Handle;
                    }
                }

                var heightOffset = form.Height - form.ClientSize.Height;
                var widthOffset = form.Width - form.ClientSize.Width;
                GameAssistWindow.Instance.SetPosition(form.Location.X + widthOffset + GameAssistWindow.Instance.Offset.X,
                                                      form.Location.Y + heightOffset + GameAssistWindow.Instance.Offset.Y);
            }
        }

        private void GameDragTick()
        {
            // 在拖动时让Game进行Tick，以便可以在场景中种植拖动对象
            CCore.Engine.Instance.EngineTickLoop(0, ()=>
            {
                LogicTick();
                RenderTick();
                CCore.Engine.Instance.SyncTick();
                CCore.Client.MainWorldInstance.SwapShadowPipes();
                mREnviroment?.SwapPipe();
                mREnviroment?.ClearAllDrawingCommits();
            });
        }
        private void LogicTick()
        {
            CCore.Engine.Instance.Tick(true);
            CCore.Engine.Instance.CurRenderFrame++;
            CCore.Client.MainWorldInstance.Render2Enviroment(RenderParam);
            if(mREnviroment != null)
            {
                var loc = mREnviroment.Camera.GetLocation();
                CCore.Client.MainWorldInstance.TravelTo(loc.X, loc.Z);
                mREnviroment.Tick();
            }
            CCore.Audio.AudioManager.Instance.Tick();
        }
        private void RenderTick()
        {            
            var elapsedMillisecond = CCore.Engine.Instance.GetElapsedMillisecond();

            if (CCore.Engine.Instance.EnableRenderTick && mREnviroment != null)
            {
                CCore.Engine.Instance.Client.Graphics.BeginDraw();

                CCore.Client.MainWorldInstance.RenderShadow(RenderParam);

                mREnviroment.RefreshPostProcess(CCore.Client.MainWorldInstance.PostProceses);
                mREnviroment.Render();

                CCore.Engine.Instance.Client.Graphics.EndDraw();
            }
        }

        public void OnGameWindowDragEnter(object sender, DragEventArgs e)
        {
            mDragTickData?._OnTickDragDataDragEnter(sender, e);
        }

        public void OnGameWindowDragLeave(object sender, EventArgs e)
        {
            mDragTickData?._OnTickDragDataDragLeave(sender, e);
        }

        public void OnGameWindowDragDrop(object sender, DragEventArgs e)
        {
            mDragTickData?._OnTickDragDataDragDrop(sender, e);
        }

        public void OnGameWindowDragOver(object sender, DragEventArgs e)
        {
            mDragTickData?._OnTickDragDataDragOver(sender, e);
        }
    }
}
