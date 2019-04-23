using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem
{
    public partial class WinBase
    {
        //#region Msg
        ////系统消息传递
        //public virtual MSG_PROC ProcMessage(ref WinMSG msg)
        //{
        //    //if (HitTestVisible == false)
        //    //    return MSG_PROC.SendToBrother;

        //    for (int i = mChildWindows.Count - 1; i >= 0; --i)
        //    {
        //        //if (mChildWindows[i].Visible==false)
        //        if (mChildWindows[i].Visibility != Visibility.Visible)
        //            continue;

        //        //鼠标消息判断区域是否在内来向下分发消息
        //        if ((UInt32)SysMessage.VWM_MOUSEFIRST <= msg.message && msg.message <= (UInt32)SysMessage.VWM_MOUSELAST)
        //        {
        //            if (mForceChildMsg || mChildWindows[i].mAbsRect.Contains(msg.pt))
        //            {
        //                //CSUtility.Support.Point ptClt = AbsToClient( ptAbs );
        //                MSG_PROC mp = mChildWindows[i].ProcMessage(ref msg);
        //                switch (mp)
        //                {
        //                    case MSG_PROC.Finished:
        //                        return MSG_PROC.Finished;
        //                    case MSG_PROC.SendToParent:
        //                        return this.OnMsg(ref msg);
        //                    case MSG_PROC.SendToBrother:
        //                        break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            mChildWindows[i].ProcMessage(ref msg);
        //        }
        //    }

        //    if (HitTestVisible == true)
        //        return OnMsg(ref msg);
        //    else
        //        return MSG_PROC.SendToBrother;
        //}

        ////几种常见消息分发模式
        //public MSG_PROC Send2Me(ref WinMSG msg)
        //{
        //    return OnMsg(ref msg);
        //}

        //public MSG_PROC Send2ChildWin(ref WinMSG msg)
        //{
        //    foreach (WinBase i in mChildWindows)
        //    {
        //        if (i.OnMsg(ref msg) == MSG_PROC.Finished)
        //        {
        //            return MSG_PROC.Finished;
        //        }
        //    }
        //    return MSG_PROC.Finished;
        //}

        //public MSG_PROC Send2BrotherWin(ref WinMSG msg)
        //{
        //    if (mParent == null)
        //        return MSG_PROC.Finished;

        //    return mParent.Send2ChildWin(ref msg);
        //}

        //public MSG_PROC Send2ParentWin(ref WinMSG msg)
        //{
        //    if (mParent != null)
        //        return mParent.OnMsg(ref msg);
        //    return MSG_PROC.Finished;
        //}

        //[System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetAsyncKeyState")]
        //private static extern UInt16 GetAsyncKeyState(UInt32 vKey);
        //public void ForceOnMsg(ref WinMSG msg)
        //{
        //    OnMsg(ref msg);
        //}
        //protected virtual MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch (msg.message)
        //    {
        //        case (UInt32)SysMessage.VWM_LBUTTONDOWN:
        //            {

        //                UInt16 altKey = GetAsyncKeyState(0x12);

        //                if (altKey != 0 || (mDragEnable))// && DockMode == System.Windows.Forms.DockStyle.None))
        //                {
        //                    mDraging = true;
        //                    mDragLocation = AbsToLocal(ref msg.pt);
        //                    WinRoot pRoot = GetRoot() as WinRoot;
        //                    if (pRoot != null)
        //                        pRoot.CaptureMouse(this);
        //                    return MSG_PROC.Finished;
        //                }
        //                return MSG_PROC.Finished;
        //            }
        //        case (UInt32)SysMessage.VWM_LBUTTONUP:
        //            {
        //                if (mDraging == true)
        //                {
        //                    CSUtility.Support.Point locPt = AbsToLocal(ref msg.pt);
        //                    if (WinDraged != null)
        //                        WinDraged(ref locPt, this);
        //                    mDraging = false;
        //                    WinRoot pRoot = GetRoot() as WinRoot;
        //                    if (pRoot != null)
        //                        pRoot.ReleaseMouse(this);
        //                }
        //                return MSG_PROC.Finished;
        //            }
        //        case (UInt32)SysMessage.VWM_MOUSEMOVE:
        //            {
        //                if (Parent != null && mDraging)
        //                {
        //                    //todo: 拖动的布局处理
        //                    CSUtility.Support.Point ptMouse = ((WinBase)Parent).AbsToLocal(ref msg.pt);
        //                    //var left = this.Margin.Left + ptMouse.X - mDragLocation.X;
        //                    //var top = this.Margin.Top + ptMouse.Y - mDragLocation.Y;
        //                    ptMouse.X -= mDragLocation.X;
        //                    ptMouse.Y -= mDragLocation.Y;
        //                    MoveWin(ref ptMouse);
        //                    //this.Margin = new CSCommon.Support.Thickness(left, top, this.Margin.Right, this.Margin.Bottom);

        //                    if (WinDraging != null)
        //                        WinDraging(ref ptMouse, this);

        //                    return MSG_PROC.Finished;
        //                }
        //                //CSUtility.Support.Point ptMouse1 = AbsToLocal(ref msg.pt);
        //                //if (WinMouseMove != null)
        //                //{

        //                //    WinMouseMove(ref ptMouse1, this);
        //                //}

        //                return MSG_PROC.Finished;
        //            }
        //        //break;
        //        case (UInt32)SysMessage.VWM_SIZE:
        //            {
        //                //if( mDockMode!=System.Windows.Forms.DockStyle.None )
        //                //{//本身有Dock，他的兄弟Dock也要变更
        //                //	mParent.DockChildWin();
        //                //}
        //                //else
        //                //if (mParent == null)
        //                //{//本身没有Dock，只要更新他的子窗口
        //                //    DockChildWin();
        //                //}
        //                //变更完，发出代理通知
        //                if (WinSizeChanged != null)
        //                    WinSizeChanged(mSize.Width, mSize.Height, this);
        //            }
        //            break;
        //    }
        //    return MSG_PROC.Finished;
        //}
        //#endregion

        public virtual void OnMouseFocus()
        {

        }
        public virtual void OnMouseUnFocus()
        {

        }


        #region MsgBehavior

        bool mHitThrough = false;
        [Category("布局")]
        [DisplayName("可点透")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public bool HitThrough
        {
            get { return mHitThrough; }
            set
            {
                mHitThrough = value;

                OnPropertyChanged("HitThrough");
            }
        }

        [Browsable(false)]
        public virtual bool IsTransparent
        {
            get
            {
                // 没有设置UIAnim和全透明的则不可点中
                if(((mWinState != null && mWinState.UVAnim == null) || mWinState == null) && BackColor.A == 0)
                    return true;

                return false;
            }
        }

        protected delegate void FUIBehaviorProcess(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs);
        
        protected enum enRoutingStrategy
        {
            Tunnel,     // 路由事件使用隧道策略，以便事件实例通过树向下路由（从根到源元素）。
            Bubble,     // 路由事件使用冒泡策略，以便事件实例通过树向上路由（从事件元素到根）
            Direct,     // 路由事件不通过元素树路由
        }

        // 冒泡消息
        Dictionary<CCore.MsgProc.BehaviorType, FUIBehaviorProcess> mBehaviorProcessDic = new Dictionary<CCore.MsgProc.BehaviorType, FUIBehaviorProcess>();
        // 隧道消息        
        Dictionary<CCore.MsgProc.BehaviorType, FUIBehaviorProcess> mPreBehaviorProcessDic = new Dictionary<CCore.MsgProc.BehaviorType, FUIBehaviorProcess>();
        // 直接消息
        Dictionary<CCore.MsgProc.BehaviorType, FUIBehaviorProcess> mDirBehaviorProcessDic = new Dictionary<CCore.MsgProc.BehaviorType, FUIBehaviorProcess>();

#region Tab处理

        protected int mMaxTabIndex = int.MinValue;
        protected int mMinTabIndex = int.MaxValue;
        public void CalculateTabIndexMaxMinValue()
        {
            //foreach (var child in mChildWindows)
            for (int i = 0; i < mChildWindows.Count;i++ )
            {
                WinBase child = null;
                try
                {
                    child = mChildWindows[i];
                }
                catch(System.Exception)
                {
                    continue;
                }
                if (child is UISystem.Interfaces.ITabAble)
                {
                    var idx = ((UISystem.Interfaces.ITabAble)child).TabIndex;
                    if (idx > mMaxTabIndex)
                        mMaxTabIndex = idx;
                    if (idx < mMinTabIndex)
                        mMinTabIndex = idx;
                }
            }

            if (mMinTabIndex < 0)
                mMinTabIndex = 0;
        }

        // 查找Tab的下一项, Tab只处理同父级的对象
        public WinBase FindNextTabItem(int nextTabIndex)
        {
            if(nextTabIndex > mMaxTabIndex)
                nextTabIndex = mMinTabIndex;

            while (nextTabIndex <= mMaxTabIndex)
            {
                foreach (var child in mChildWindows)
                {
                    if (child is UISystem.Interfaces.ITabAble)
                    {
                        var idx = ((UISystem.Interfaces.ITabAble)child).TabIndex;
                        if (idx == nextTabIndex)
                            return child;
                    }
                }

                nextTabIndex++;
            }

            return null;
        }

        #endregion
        
        protected virtual void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Down, WinBase_OnKeyboardCharDown, enRoutingStrategy.Tunnel);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Up, WinBase_OnKeyboardCharUp, enRoutingStrategy.Tunnel);

            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, WinBase_OnMouseLeftButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, WinBase_OnMouseLeftButtonUp, enRoutingStrategy.Bubble);
            //第二只手
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Down, WinBase_OnPointerDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Up, WinBase_OnPointerUp, enRoutingStrategy.Bubble);
            //第三只手
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Down, WinBase_OnPointerDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Up, WinBase_OnPointerUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, WinBase_OnMouseRightButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, WinBase_OnMouseMove, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinBase_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);
        }

        protected void RegistBehaviorProcess(CCore.MsgProc.BehaviorType bhType, FUIBehaviorProcess process, enRoutingStrategy rs)
        {
            switch (rs)
            {
                case enRoutingStrategy.Tunnel:
                    mPreBehaviorProcessDic[bhType] = process;
                    break;

                case enRoutingStrategy.Bubble:
                    mBehaviorProcessDic[bhType] = process;
                    break;

                case enRoutingStrategy.Direct:
                    mDirBehaviorProcessDic[bhType] = process;
                    break;
            }

        }
        protected void UnRegistBehaviorProcess(CCore.MsgProc.BehaviorType bhType, enRoutingStrategy rs)
        {
            switch (rs)
            {
                case enRoutingStrategy.Tunnel:
                    mPreBehaviorProcessDic.Remove(bhType);
                    break;

                case enRoutingStrategy.Bubble:
                    mBehaviorProcessDic.Remove(bhType);
                    break;

                case enRoutingStrategy.Direct:
                    mDirBehaviorProcessDic.Remove(bhType);
                    break;
            }
        }
        protected void ClearBehaviorProcess(enRoutingStrategy rs)
        {
            switch (rs)
            {
                case enRoutingStrategy.Tunnel:
                    mPreBehaviorProcessDic.Clear();
                    break;
                case enRoutingStrategy.Bubble:
                    mBehaviorProcessDic.Clear();
                    break;
                case enRoutingStrategy.Direct:
                    mDirBehaviorProcessDic.Clear();
                    break;
            }
        }

        public virtual void PreProcBehavior(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs args)
        {
            if (HitTestVisible == true)
                OnPreBehavior(bhInit, args);

            if (args.Handled)
                return;

            for (int i = mChildWindows.Count - 1; i >= 0; --i)
            {
                if (mChildWindows[i].Visibility != Visibility.Visible)
                    continue;

                //鼠标消息判断区域是否在内来向下分发消息
                if(bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Begin && bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_End)
                {
                    if (mChildWindows[i].mAbsRect.Contains(UISystem.Device.Mouse.Instance.Position))
                    {
                        mChildWindows[i].PreProcBehavior(bhInit, args);
                    }
                }
                else
                {
                    mChildWindows[i].PreProcBehavior(bhInit, args);
                }

                if (args.Handled)
                    return;
            }
        }

        public virtual void ProcBehavior(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs args)
        {
            if (HitTestVisible == true)
            {
                bhInit.Sender = this;
                OnBehavior(bhInit, args);
            }

            if (args.Handled)
                return;

            var win = Parent as WinBase;
            if (win != null)
            {
                bhInit.Sender = this;
                win.ProcBehavior(bhInit, args);
            }
        }

        public void ProcDirBehavior(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs args)
        {
            OnDirBehavior(bhInit, args);
        }

        // 几种常见的消息分发模式
        public void Send2Me(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs args)
        {
            OnBehavior(bhInit, args);
        }

        //public MSG_PROC Send2ChildWin(IBehaviorParameter bhInit)
        //{
        //    foreach (var win in mChildWindows)
        //    {
        //        if (win.OnBehavior(bhInit) == MSG_PROC.Finished)
        //            return MSG_PROC.Finished;
        //    }

        //    return MSG_PROC.Finished;
        //}

        //public MSG_PROC Send2ChildWin(IBehaviorParameter bhInit, List<WinBase> expWins)
        //{
        //    foreach (var win in mChildWindows)
        //    {
        //        if (expWins.Contains(win))
        //            continue;

        //        if (win.OnBehavior(bhInit) == MSG_PROC.Finished)
        //            return MSG_PROC.Finished;
        //    }

        //    return MSG_PROC.Finished;
        //}

        //public MSG_PROC Send2BrotherWin(IBehaviorParameter bhInit)
        //{
        //    if (mParent == null)
        //        return MSG_PROC.Finished;

        //    List<WinBase> expWins = new List<WinBase>();
        //    expWins.Add(this);

        //    return mParent.Send2ChildWin(bhInit, expWins);
        //}

        public void Send2ParentWin(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mParent != null)
                mParent.OnBehavior(bhInit, eventArgs);
        }

        //public void ForceOnBehavior(IBehaviorParameter bhInit)
        //{
        //    OnBehavior(bhInit);
        //}

        private void OnPreBehavior(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs args)
        {
            FUIBehaviorProcess process;
            if (mPreBehaviorProcessDic.TryGetValue((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()), out process))
            {
                if (process != null)
                    process(bhInit, args);
            }
        }
        private void OnBehavior(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs args)
        {
            FUIBehaviorProcess process;
            if (mBehaviorProcessDic.TryGetValue((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()), out process))
            {
                if (process != null)
                    process(bhInit, args);
            }
        }
        private void OnDirBehavior(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs args)
        {
            FUIBehaviorProcess process;
            if (mDirBehaviorProcessDic.TryGetValue((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()), out process))
            {
                if (process != null)
                    process(bhInit, args);
            }
        }

        protected void WinBase_OnKeyboardCharDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var kb = init as CCore.MsgProc.Behavior.KB_Char;
            var arg = new UISystem.Message.KeyEventArgs(kb.Key);
            _FWinKeyDown(this, arg);
            eventArgs.Handled = arg.Handled;
        }

        protected void WinBase_OnKeyboardCharUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var kb = init as CCore.MsgProc.Behavior.KB_Char;
            var arg = new UISystem.Message.KeyEventArgs(kb.Key);
            _FWinKeyUp(this, arg);
            eventArgs.Handled = arg.Handled;
        }
        
        protected void WinBase_OnMouseLeftButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            bool bSenderDragEnable = false;
            var winSender = init.Sender as WinBase;
            if(winSender != null)
                bSenderDragEnable = winSender.DragEnable;

            if (mDragEnable || bSenderDragEnable)
            {
                var mb = init as CCore.MsgProc.Behavior.Mouse_Key;

                mDraging = true;
                //mStartAction = false;
                mDragLocation = AbsToLocal(mb.X, mb.Y);
                mDragOffset.X = mDragLocation.X - mLocation.X;
                mDragOffset.Y = mDragLocation.Y - mLocation.Y;

                UISystem.Device.Mouse.Instance.Capture(this,init.GetBehaviorType());
            }

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonDown(this, arg);
            _FWinLeftMouseButtonDown(this, arg);
            eventArgs.Handled = arg.Handled;
            //UISystem.Device.Keyboard.Instance.UnFocus();
        }

        protected void WinBase_OnMouseLeftButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDraging == true)
            {
                var mb = init as CCore.MsgProc.Behavior.Mouse_Key;

                CSUtility.Support.Point locPt = AbsToLocal(mb.X, mb.Y);
                if (WinDraged != null)
                    WinDraged(ref locPt, this);
                mDraging = false;

                //if (mAstrictX || mAstrictY)
                //    mStartAction = true;

                UISystem.Device.Mouse.Instance.ReleaseCapture(init.BehaviorId);
            }

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonUp(this, arg);
            _FWinLeftMouseButtonUp(this, arg);
            eventArgs.Handled = arg.Handled;
        }

        protected void WinBase_OnPointerDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            bool bSenderDragEnable = false;
            var winSender = init.Sender as WinBase;
            if (winSender != null)
                bSenderDragEnable = winSender.DragEnable;

            if (mDragEnable || bSenderDragEnable)
            {
                var mb = init as CCore.MsgProc.Behavior.Mouse_Key;

                mDraging = true;
                //mStartAction = false;
                mDragLocation = AbsToLocal(mb.X, mb.Y);
                mDragOffset.X = mDragLocation.X - mLocation.X;
                mDragOffset.Y = mDragLocation.Y - mLocation.Y;

                UISystem.Device.Mouse.Instance.Capture(this, init.GetBehaviorType());
            }

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonDown(this, arg);
            _FWinLeftMouseButtonDown(this, arg);
            eventArgs.Handled = arg.Handled;
            //UISystem.Device.Keyboard.Instance.UnFocus();
        }

        protected void WinBase_OnPointerUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDraging == true)
            {
                var mb = init as CCore.MsgProc.Behavior.Mouse_Key;

                CSUtility.Support.Point locPt = AbsToLocal(mb.X, mb.Y);
                if (WinDraged != null)
                    WinDraged(ref locPt, this);
                mDraging = false;

                //if (mAstrictX || mAstrictY)
                //    mStartAction = true;

                UISystem.Device.Mouse.Instance.ReleaseCapture(init.BehaviorId);
            }

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonUp(this, arg);
            _FWinLeftMouseButtonUp(this, arg);
            eventArgs.Handled = arg.Handled;
        }

        protected void WinBase_OnMouseRightButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);
            _FWinMouseButtonDown(this, arg);
            _FWinRightMouseButtonDown(this, arg);
            eventArgs.Handled = arg.Handled;
        }
        protected void WinBase_OnMouseRightButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);
            _FWinMouseButtonUp(this, arg);
            _FWinRightMouseButtonUp(this, arg);
            eventArgs.Handled = arg.Handled;
        }

        protected void WinBase_OnMouseMidButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Middle);
            _FWinMouseButtonDown(this, arg);
            _FWinMidMouseButtonDown(this, arg);
            eventArgs.Handled = arg.Handled;
        }
        protected void WinBase_OnMouseMidButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Middle);
            _FWinMouseButtonUp(this, arg);
            _FWinMidMouseButtonUp(this, arg);
            eventArgs.Handled = arg.Handled;
        }

        protected void WinBase_OnMouseMove(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mm = init as CCore.MsgProc.Behavior.Mouse_Move;

            if (Parent != null && mDraging)
            {
                var parentBase = Parent as WinBase;
                CSUtility.Support.Point ptMouse = parentBase.AbsToLocal(mm.X, mm.Y);
                if (mLockingX)
                    ptMouse.X = mLocation.X;
                else
                    ptMouse.X -= mDragLocation.X;
                if (mLockingY)
                    ptMouse.Y = mLocation.Y;
                else
                    ptMouse.Y -= mDragLocation.Y;

                if (mAstrictX)
                {
                    if (Width > parentBase.Width)
                    {
                        if (ptMouse.X > 0)
                            ptMouse.X = 0;
                        if (ptMouse.X + Width < parentBase.Width)
                            ptMouse.X = parentBase.Width - Width;
                    }
                    else
                    {
                        ptMouse.X = 0;
                    }
                }
                if (mAstrictY)
                {
                    if (Height > parentBase.Height)
                    {
                        if (ptMouse.Y > 0)
                            ptMouse.Y = 0;
                        if (ptMouse.Y + Height < parentBase.Height)
                            ptMouse.Y = parentBase.Height - Height;
                    }
                    else
                    {
                        ptMouse.Y = 0;
                    }
                }
                    
                MoveWin(ref ptMouse);
                if (WinDraging != null)
                    WinDraging(ref ptMouse, this);
            }

            var arg = new Message.MouseEventArgs(mm.Clicks, mm.X, mm.Y, mm.button);
            _FWinMouseMove(this, arg);
            eventArgs.Handled = arg.Handled;
        }

        protected void WinBase_OnPreWinSizeChanged(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var ws = init as CCore.MsgProc.Behavior.Win_SizeChanged;

            if (WinSizeChanged != null)
                WinSizeChanged(ws.Width, ws.Height, this);
        }

        #endregion
    }
}
