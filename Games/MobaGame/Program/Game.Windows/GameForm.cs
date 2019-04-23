using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Windows
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
        }

        public void EngineLoopTick()
        {
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            Log.FileLog.Instance.Begin("vfx.log");
        }

        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void GameForm_Activated(object sender, EventArgs e)
        {
            CCore.Engine.Instance.MainFormActivated = true;
        }

        private void GameForm_Deactivate(object sender, EventArgs e)
        {
            CCore.Engine.Instance.MainFormActivated = false;
        }

        private void GameForm_SizeChanged(object sender, EventArgs e)
        {
            var _init = new CCore.MsgProc.Behavior.Win_SizeChanged();
            _init.Width = ClientSize.Width;
            _init.Height = ClientSize.Height;
            var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
            tr.BeType = _init.GetBehaviorType();
            tr.BeInit = _init;

            CCore.Engine.Instance.Client?.MsgRecieverMgr?.Dispatch(tr.BeType, tr.BeInit);
            EditorContext.Instance.OnGameWindowChanged(this.Left, this.Top, this.Height, this.Width);
        }

        private void GameForm_Move(object sender, EventArgs e)
        {
            EditorContext.Instance.OnGameWindowChanged(this.Left, this.Top, this.Height, this.Width);
        }

        #region 鼠标消息
        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            var _init = new CCore.MsgProc.Behavior.Mouse_Key();
            _init.X = e.X;
            _init.Y = e.Y;
            _init.Clicks = e.Clicks;
            switch(e.Button)
            {
                case MouseButtons.Left:
                    _init.behavior = CCore.MsgProc.BehaviorType.BHT_LB_Down;
                    break;
                case MouseButtons.Middle:
                    _init.behavior = CCore.MsgProc.BehaviorType.BHT_MB_Down;
                    break;
                case MouseButtons.Right:
                    _init.behavior = CCore.MsgProc.BehaviorType.BHT_RB_Down;
                    break;
            }
            var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
            tr.BeType = _init.GetBehaviorType();
            tr.BeInit = _init;

            CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch(tr.BeType, tr.BeInit);
        }
        private void GameForm_MouseUp(object sender, MouseEventArgs e)
        {
            var _init = new CCore.MsgProc.Behavior.Mouse_Key();
            _init.X = e.X;
            _init.Y = e.Y;
            _init.Clicks = e.Clicks;
            switch(e.Button)
            {
                case MouseButtons.Left:
                    _init.behavior = CCore.MsgProc.BehaviorType.BHT_LB_Up;
                    break;
                case MouseButtons.Middle:
                    _init.behavior = CCore.MsgProc.BehaviorType.BHT_MB_Up;
                    break;
                case MouseButtons.Right:
                    _init.behavior = CCore.MsgProc.BehaviorType.BHT_RB_Up;
                    break;
            }
            var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
            tr.BeType = _init.GetBehaviorType();
            tr.BeInit = _init;

            CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch(tr.BeType, tr.BeInit);
        }
        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {
            var result = CCore.Engine.Instance.Client?.Graphics?.IsDeviceLost;
            if (result == null || result == true)
            {
                return;
            }

            var _init = new CCore.MsgProc.Behavior.Mouse_Move();
            _init.X = e.X;
            _init.Y = e.Y;
            _init.Clicks = e.Clicks;
            switch(e.Button)
            {
                case MouseButtons.Left:
                    _init.button = CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left;
                    break;
                case MouseButtons.Middle:
                    _init.button = CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Middle;
                    break;
                case MouseButtons.Right:
                    _init.button = CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right;
                    break;
                default:
                    _init.button = CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.None;
                    break;
            }
            var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
            tr.BeType = _init.GetBehaviorType();
            tr.BeInit = _init;

            CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch(tr.BeType, tr.BeInit);
        }
        private void GameForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var result = CCore.Engine.Instance.Client?.Graphics?.IsDeviceLost;
            if (result == null || result == true)
            {
                return;
            }

            var _init = new CCore.MsgProc.Behavior.Mouse_Key();
            _init.X = e.X;
            _init.Y = e.Y;
            _init.Clicks = e.Clicks;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    _init.behavior = CCore.MsgProc.BehaviorType.BHT_LB_DoubleClick;
                    break;
                case MouseButtons.Right:
                    _init.behavior = CCore.MsgProc.BehaviorType.BHT_RB_DoubleClick;
                    break;
            }
            var tr = new CCore.MsgProc.IMsgTranslator.TransResult();
            tr.BeType = _init.GetBehaviorType();
            tr.BeInit = _init;

            CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch(tr.BeType, tr.BeInit);
        }
        #endregion

        protected override void WndProc(ref Message m)
        {
            var result = CCore.Engine.Instance.Client?.Graphics?.IsDeviceLost;
            if(result == null || result == true)
            {
                base.WndProc(ref m);
                return;
            }

            switch (m.Msg)
            {
                case (int)UISystem.WinMsg.WM_INPUTLANGCHANGE:
                case (int)UISystem.WinMsg.WM_IME_SETCONTEXT:
                case (int)UISystem.WinMsg.WM_IME_COMPOSITION:
                case (int)UISystem.WinMsg.WM_IME_ENDCOMPOSITION:
                case (int)UISystem.WinMsg.WM_IME_CHAR:
                case (int)UISystem.WinMsg.WM_CHAR:
                //case (int)UISystem.WinMsg.WM_KEYDOWN:
                    {
                        if (CCore.Engine.Instance.MainFormActivated)
                        {
                            //IEngine.Instance.Client.MsgRecieverMgr.OnSystemMsg(m.Msg, m.WParam, m.LParam);
                            var ti = new CCore.MsgProc.Behavior.TextInput();
                            ti.Msg = CCore.MsgProc.DeviceMessage.Create(m.HWnd, m.Msg, m.WParam, m.LParam);
                            CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch((int)(CCore.MsgProc.BehaviorType.BHT_TextInput), ti);
                        }
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        #region 键盘
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            var key = new CCore.MsgProc.Behavior.KB_Char();
            key.Key = (CCore.MsgProc.BehaviorParameter.enKeys)e.KeyValue;
            key.behavior = CCore.MsgProc.BehaviorType.BHT_KB_Char_Down;
            CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch((int)key.behavior, key);
        }
        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            var key = new CCore.MsgProc.Behavior.KB_Char();
            key.Key = (CCore.MsgProc.BehaviorParameter.enKeys)e.KeyValue;
            key.behavior = CCore.MsgProc.BehaviorType.BHT_KB_Char_Up;
            CCore.Engine.Instance.Client.MsgRecieverMgr.Dispatch((int)key.behavior, key);
        }
        #endregion

        #region 鼠标拖放
        private void GameForm_DragEnter(object sender, DragEventArgs e)
        {
            EditorContext.Instance._OnGameWindowDragEnter(sender, e);
        }
        private void GameForm_DragDrop(object sender, DragEventArgs e)
        {
            EditorContext.Instance._OnGameWindowDragDrop(sender, e);
        }
        private void GameForm_DragLeave(object sender, EventArgs e)
        {
            EditorContext.Instance._OnGameWindowDragLeave(sender, e);
        }
        private void GameForm_DragOver(object sender, DragEventArgs e)
        {
            EditorContext.Instance._OnGameWindowDragOver(sender, e);
        }
        #endregion
    }
}
