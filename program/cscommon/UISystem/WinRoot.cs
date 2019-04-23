using System;
using System.Collections.Generic;

namespace UISystem
{
    public delegate void FEventProcess(WinRoot root);
    public class WinRoot : WinBase, IDisposable
    {
        public static CCore.Graphics.ViewTarget MainForm = null;
        //public static System.Windows.Forms.Form MainForm
        //{
        //    get { return mMainForm;  }
        //    set
        //    {
        //        mMainForm = value;
        //    }
        //}

        List<FEventProcess> mEventProcs = new List<FEventProcess>();
        public void PostEventProcessor(FEventProcess proc)
        {
            mEventProcs.Add(proc);
        }

        private void ProcAllPostEvent()
        {
            foreach (var i in mEventProcs)
            {
                i(this);
            }
            mEventProcs.Clear();
        }

        public List<Popup> PopupedWins = new List<Popup>();
        public void CleanPopupedWins()
        {
            PopupedWins.Clear();
        }

        // 模态对话框,在模态对话框关闭完之前不处理当前模态对话框之外的消息
        public Stack<WinForm> DialogForms = new Stack<WinForm>();
		//static WinRoot smRoot = new WinRoot();

		public static int			smRootCount = 0;
		public WinRoot()
		{
			smRootCount++;
            HitThrough = true;
            //            mLBDownBehavior = Root_OnMouseLeftButtonDown;
            //mLBUpBehavior = Root_OnMouseLeftButtonUp;
            //mMouseMoveBehavior = ;
		}
        public void Dispose()
		{
			smRootCount--;
		}
		//public static WinRoot GetInstance(){ return smRoot; }

		public void SetSize( int w , int h )
        {
            BackColor = CSUtility.Support.Color.FromArgb(0,0,0,0);
		    Parent = null;

		    Left = 0;
		    Top = 0;

		    Width = w;
		    Height = h;
        }
        public Popup PopupedStayWindow(ref CSUtility.Support.Point pt)
        {
            for (int i=PopupedWins.Count - 1; i>=0; --i)
            {
                var popupWin = PopupedWins[i];
                var win = popupWin.PopupStayWindow(ref pt);
                if (win != null)
                    return win;
            }

            return null;
        }

        public static UInt16 LOWORD(IntPtr l)
        {
            return (UInt16)((UInt64)l & 0x000000000000FFFF);
        }
        public static UInt16 HIWORD(IntPtr l)
        {
            return (UInt16)(((UInt64)l & 0x00000000FFFFFFFF)>>16);
            //return ((UInt16)((((UInt32)(((UInt64)l&0x00000000FFFFFFFF)))) >> 16) & 0xffff));
        }
        
        public static int GET_X_LPARAM(IntPtr lp)
        {
            return ((int)(short)LOWORD(lp));
        }
        public static int GET_Y_LPARAM(IntPtr lp)
        {
            return ((int)(short)HIWORD(lp));
        }

        //public void DispatchMsg( ref System.Windows.Forms.Message msg )
        //{
        //    WinMSG myMsg=new WinMSG();
        //    myMsg.message = (UInt32)msg.Msg;
        //    myMsg.hwnd = (UInt32)msg.HWnd.ToInt32();
        //    myMsg.lParam = (UInt32)msg.LParam.ToInt32();
        //    myMsg.wParam = (UInt32)msg.WParam.ToInt32();
        //    myMsg.time = CCore.Engine.Instance.GetFrameMillisecond();
        //    myMsg.pt.X = GET_X_LPARAM(myMsg.lParam);
        //    myMsg.pt.Y = GET_Y_LPARAM(myMsg.lParam);

        //    switch( msg.Msg )
        //    {
        //    case (Int32)SysMessage.VWM_MOUSEMOVE:
        //        {
        //            Message.Mouse.Instance.Position = myMsg.pt;

        //            var popStay = PopupedStayWindow(ref myMsg.pt);
        //            WinBase pNewStay = null;
        //            if (popStay != null)
        //                pNewStay = popStay.StayWindow(ref myMsg.pt);
        //            else
        //                pNewStay = StayWindow(ref myMsg.pt);

        //            if( pNewStay!=mStayWin )
        //            {
        //                if (mStayWin != null)
        //                {
        //                    var arg = new Message.RoutedEventArgs();
        //                    arg.Source = mStayWin;
        //                    //mStayWin._FWinMouseLeave(ref myMsg.pt, arg);

        //                    var parent = mStayWin;
        //                    while(!arg.Handled && parent != null)
        //                    {
        //                        parent._FWinMouseLeave(ref myMsg.pt, arg);
        //                        parent = parent.Parent as WinBase;
        //                    }
        //                }
        //                if (pNewStay != null)
        //                {
        //                    var arg = new Message.RoutedEventArgs();
        //                    arg.Source = pNewStay;
        //                    //pNewStay._FWinMouseEnter(ref myMsg.pt, arg);

        //                    var parent = pNewStay;
        //                    while (!arg.Handled && parent != null)
        //                    {
        //                        parent._FWinMouseEnter(ref myMsg.pt, arg);
        //                        parent = parent.Parent as WinBase;
        //                    }
        //                }
        //                mStayWin = pNewStay;
        //                //if (mStayWin!=null&&mStayWin.HitTestVisible == false)
        //                //    mStayWin = null;
        //            }

        //            if (mStayWin != null)
        //            {
        //                var pt = AbsToLocal(ref myMsg.pt);
        //                var arg = new Message.MouseEventArgs(0, pt.X, pt.Y);
        //                var parent = mStayWin;
        //                while (!arg.Handled && parent != null)
        //                {
        //                    parent._FWinMouseMove(parent, arg);
        //                    parent = parent.Parent as WinBase;
        //                }
        //            }
        //        }
        //        break;
        //    case (Int32)SysMessage.VWM_LBUTTONDOWN:
        //        {
        //            Message.Mouse.Instance.MouseLeftButtonDown = true;

        //            var popStay = PopupedStayWindow(ref myMsg.pt);
        //            if (popStay != null)
        //            {
        //                for (int i = PopupedWins.Count - 1; i >= 0; i--)
        //                {
        //                    var popup = PopupedWins[i];
        //                    if (popStay != null)
        //                    {
        //                        if (popup == popStay)
        //                            continue;
        //                    }

        //                    if (!popup.StaysOpen)
        //                        popup.IsOpen = false;
        //                }
        //            }
        //            else
        //            {
        //                for (int i = PopupedWins.Count - 1; i >= 0; i-- )
        //                {
        //                    PopupedWins[i].IsOpen = false;
        //                }
        //            }

        //            WinBase pNewStay = null;
        //            if (popStay != null)
        //                pNewStay = popStay.StayWindow(ref myMsg.pt);
        //            else
        //                pNewStay = StayWindow(ref myMsg.pt);
        //            if (pNewStay == null)
        //                break;
        //            if (mCapMsgWin == null)
        //            {
        //                WinForm pForm = pNewStay.GetWinForm(this);
        //                if (pForm != null)
        //                {
        //                    WinBase pSaveParent = pForm.Parent as WinBase;
        //                    //将Form窗口置于最顶端
        //                    pForm.Parent = null;
        //                    pForm.Parent = pSaveParent;
        //                    //pSaveParent.MoveChild(pSaveParent.IndexOfChild(pForm), pSaveParent.GetChildWinCount() - 1);
        //                    _FFormToTop(pForm);

        //                    //if( pSaveParent==null )
        //                    //    break;
        //                    //pForm = pSaveParent.GetWinForm( this );
        //                }
        //            }

        //            {
        //                var pt = AbsToLocal(ref myMsg.pt);
        //                var arg = new Message.MouseEventArgs(1, pt.X, pt.Y);
        //                var parent = pNewStay;
        //                while (!arg.Handled && parent != null)
        //                {
        //                    parent._FWinMouseButtonDown(parent, arg);
        //                    parent = parent.Parent as WinBase;
        //                }
        //            }
        //        }
        //        break;
        //    case (Int32)SysMessage.VWM_LBUTTONUP:
        //        {
        //            Message.Mouse.Instance.MouseLeftButtonDown = false;

        //            var popStay = PopupedStayWindow(ref myMsg.pt);
        //            WinBase pNewStay = null;
        //            if (popStay != null)
        //                pNewStay = popStay.StayWindow(ref myMsg.pt);
        //            else
        //                pNewStay = StayWindow(ref myMsg.pt);
                    
        //            if (pNewStay == null)
        //                break;

        //            {
        //                var pt = AbsToLocal(ref myMsg.pt);
        //                var arg = new Message.MouseEventArgs(0, pt.X, pt.Y);
        //                var parent = pNewStay;
        //                while (!arg.Handled && parent != null)
        //                {
        //                    parent._FWinMouseButtonUp(parent, arg);
        //                    parent = parent.Parent as WinBase;
        //                }
        //            }
        //        }
        //        break;
        //    }
		
        //    if( mCapMsgWin!=null )
        //    {
        //        mCapMsgWin.ProcMessage( ref myMsg );
        //    }
        //    else if( mCapMouseWin!=null && (UInt32)SysMessage.VWM_MOUSEFIRST<=msg.Msg && msg.Msg<=(UInt32)SysMessage.VWM_MOUSELAST )
        //    {
        //        mCapMouseWin.ProcMessage( ref myMsg );
        //        if (mCapMouseWin!=null)
        //            mCapMouseWin.ForceOnMsg(ref myMsg);
        //    }
        //    else
        //    {
        //        ProcMessage( ref myMsg );
        //    }
        //}

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, Root_PreOnMouseMove, WinBase.enRoutingStrategy.Tunnel);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, Root_PreOnButtonDown, WinBase.enRoutingStrategy.Tunnel);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Down, Root_PreOnButtonDown, WinBase.enRoutingStrategy.Tunnel);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Down, Root_PreOnButtonDown, WinBase.enRoutingStrategy.Tunnel);
            //RegistBehaviorProcess(BehaviorType.BHT_LB_Down, Root_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Tunnel);
            //RegistBehaviorProcess(BehaviorType.BHT_LB_Up, Root_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Tunnel);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinRoot_OnPreWinSizeChanged, WinBase.enRoutingStrategy.Tunnel);
        }

        protected void WinRoot_OnPreWinSizeChanged(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var sizeChangedMsg = init as CCore.MsgProc.Behavior.Win_SizeChanged;
            this.Width = sizeChangedMsg.Width;
            this.Height = sizeChangedMsg.Height;
        }

        UISystem.Message.RoutedEventArgs GetRoutedEventArgsWithBehaviorType(CCore.MsgProc.BehaviorType bhType)
        {
            switch (bhType)
            {
                case CCore.MsgProc.BehaviorType.BHT_KB_Char_Down:
                case CCore.MsgProc.BehaviorType.BHT_KB_Char_Up:
                    return new UISystem.Message.RoutedEventArgs();
                case CCore.MsgProc.BehaviorType.BHT_Mouse_Move:
                case CCore.MsgProc.BehaviorType.BHT_Mouse_Wheel:
                case CCore.MsgProc.BehaviorType.BHT_Mouse_Key:
                case CCore.MsgProc.BehaviorType.BHT_LB_Down:
                case CCore.MsgProc.BehaviorType.BHT_LB_DoubleClick:
                case CCore.MsgProc.BehaviorType.BHT_LB_Up:
                case CCore.MsgProc.BehaviorType.BHT_RB_Down:
                case CCore.MsgProc.BehaviorType.BHT_RB_DoubleClick:
                case CCore.MsgProc.BehaviorType.BHT_RB_Up:
                case CCore.MsgProc.BehaviorType.BHT_MB_Down:
                case CCore.MsgProc.BehaviorType.BHT_MB_Up:
                case CCore.MsgProc.BehaviorType.BHT_Pointer2Down:
                case CCore.MsgProc.BehaviorType.BHT_Pointer2Up:
                case CCore.MsgProc.BehaviorType.BHT_Pointer3Down:
                case CCore.MsgProc.BehaviorType.BHT_Pointer3Up:
                    return new UISystem.Message.MouseEventArgs();
                case CCore.MsgProc.BehaviorType.BHT_WinSizeChanged:
                case CCore.MsgProc.BehaviorType.BHT_System_End:
                case CCore.MsgProc.BehaviorType.BHT_UVAnimSetted:
                    return new UISystem.Message.RoutedEventArgs();
            }

            return new UISystem.Message.RoutedEventArgs();
        }

        public void DispatchBehavior(CCore.MsgProc.BehaviorParameter bhInit, bool bAsync = false)
        {
            var reArgs = GetRoutedEventArgsWithBehaviorType((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()));

            // 提取鼠标位置
            if (bhInit.GetBehaviorType() > (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Begin && bhInit.GetBehaviorType() < (int)CCore.MsgProc.BehaviorType.BHT_Mouse_End)
            {
                switch(bhInit.GetBehaviorType())
                {
                    case (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Move:
                        {
                            var mm = bhInit as CCore.MsgProc.Behavior.Mouse_Move;
                            if (mm != null)
                            {

#if ANDROID || IOS
                                bhInit.BehaviorId = 10;//move 单独处理
#endif
                                UISystem.Device.Mouse.Instance.Position = new CSUtility.Support.Point(mm.X, mm.Y);
                            }                      
                        }
                        break;
                    default:
                        {
                            var mm = bhInit as CCore.MsgProc.Behavior.Mouse_Key;
                            if (mm != null)
                                UISystem.Device.Mouse.Instance.Position = new CSUtility.Support.Point(mm.X, mm.Y);
                        }
                        break;
                }
            }
            // 处理模态对话框
            WinBase procWin = this;
            if (DialogForms.Count > 0)
            {
                procWin = DialogForms.Peek();
            }

#if WIN
                   // 处理快捷键
            if (bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Keyboard_Begin &&
               bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Keyboard_End &&
               UISystem.Device.Keyboard.Instance.Enable &&
               !CCore.Engine.Instance.IsEditorMode)
            {
                UISystem.Device.Keyboard.Instance.ProcessHotKey(bhInit);
            }

            // 先处理隧道消息(由父向子)
            if (UISystem.Device.Keyboard.Instance.FocusWin != null &&
                ((bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Keyboard_Begin &&
                bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Keyboard_End) ||
                bhInit.GetBehaviorType() == (int)CCore.MsgProc.BehaviorType.BHT_TextInput) &&
                UISystem.Device.Keyboard.Instance.Enable)
            {
                // 键盘
                UISystem.Device.Keyboard.Instance.FocusWin.PreProcBehavior(bhInit, reArgs);
            }
            else if (UISystem.Device.Mouse.Instance.GetCapMouseWin(bhInit.GetBehaviorType()) != null)
            {
                if (bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Begin &&
                    bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_End)
                {
                    if(UISystem.Device.Mouse.Instance.Enable)
                    {
                        // 鼠标捕获对象处理
                        UISystem.Device.Mouse.Instance.GetCapMouseWin(bhInit.GetBehaviorType()).PreProcBehavior(bhInit, reArgs);
                    }
                }
            }
            //else if (UISystem.Device.Mouse.Instance.FocusWin != null &&
            //        bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Begin &&
            //        bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_End)
            //{
            //    // 鼠标焦点对象处理
            //    UISystem.Device.Mouse.Instance.FocusWin.PreProcBehavior(bhInit, reArgs);
            //}
            else
            {
                PreProcBehavior(bhInit, reArgs);
            }

            if (reArgs.Handled)
                return;

            // 再处理冒泡消息(由子向父)
            reArgs = GetRoutedEventArgsWithBehaviorType((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()));

            if(UISystem.Device.Keyboard.Instance.FocusWin != null &&
               ((bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Keyboard_Begin &&
                bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Keyboard_End) ||
                bhInit.GetBehaviorType() == (int)CCore.MsgProc.BehaviorType.BHT_TextInput) &&
                UISystem.Device.Keyboard.Instance.Enable)
            {
                // 键盘
                UISystem.Device.Keyboard.Instance.FocusWin.ProcBehavior(bhInit, reArgs);
            }
            else if (UISystem.Device.Mouse.Instance.GetCapMouseWin(bhInit.GetBehaviorType()) != null &&
                    bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Begin &&
                    bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_End &&
                    UISystem.Device.Mouse.Instance.Enable)
            {
                // 鼠标捕获对象处理
                UISystem.Device.Mouse.Instance.GetCapMouseWin(bhInit.GetBehaviorType()).ProcBehavior(bhInit, reArgs);
            }
            else if(UISystem.Device.Mouse.Instance.FocusWin != null &&
                    bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Begin &&
                    bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_End &&
                    UISystem.Device.Mouse.Instance.Enable)
            {
                // 鼠标焦点对象处理
                UISystem.Device.Mouse.Instance.FocusWin.ProcBehavior(bhInit, reArgs);
            }
            else
            {
                procWin.ProcBehavior(bhInit, reArgs);
            }

            if (reArgs.Handled)
                return;

            // 处理直接消息
            reArgs = GetRoutedEventArgsWithBehaviorType((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()));
            if (UISystem.Device.Keyboard.Instance.FocusWin != null &&
               ((bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Keyboard_Begin &&
                bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Keyboard_End) ||
                bhInit.GetBehaviorType() == (int)CCore.MsgProc.BehaviorType.BHT_TextInput) &&
                UISystem.Device.Keyboard.Instance.Enable)
            {
                // 键盘
                UISystem.Device.Keyboard.Instance.FocusWin.ProcDirBehavior(bhInit, reArgs);
            }
            else if (UISystem.Device.Mouse.Instance.GetCapMouseWin(bhInit.GetBehaviorType()) != null &&
                    bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Begin &&
                    bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_End &&
                    UISystem.Device.Mouse.Instance.Enable)
            {
                // 鼠标捕获对象处理
                UISystem.Device.Mouse.Instance.GetCapMouseWin(bhInit.GetBehaviorType()).ProcDirBehavior(bhInit, reArgs);
            }
            else if (UISystem.Device.Mouse.Instance.FocusWin != null &&
                    bhInit.GetBehaviorType() >= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Begin &&
                    bhInit.GetBehaviorType() <= (int)CCore.MsgProc.BehaviorType.BHT_Mouse_End &&
                    UISystem.Device.Mouse.Instance.Enable)
            {
                // 鼠标焦点对象处理
                UISystem.Device.Mouse.Instance.FocusWin.ProcDirBehavior(bhInit, reArgs);
            }
            else
            {
                procWin.ProcDirBehavior(bhInit, reArgs);
            }
#elif ANDROID || IOS
            switch(bhInit.GetBehaviorType())
            {
                case (int)CCore.MsgProc.BehaviorType.BHT_LB_Down:
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer2Down:
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer3Down:
                    {
                        //手势周期的开始
                        var finger= UISystem.Device.Mouse.Instance.GetCapFinger(bhInit.BehaviorId);
                        if(finger ==null)
                        {
                            MobileProcBehavior(this,bhInit,reArgs);
                        }
                        else
                        {
                        //    Log.FileLog.WriteLine("指令出错了");
                            UISystem.Device.Mouse.Instance.UnFocus(bhInit.BehaviorId);
                            MobileProcBehavior(this,bhInit,reArgs);
                     
                        }
                    }
                    break;
                case (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Move:
                    {
                        MobileProcBehavior(this,bhInit,reArgs);
                    }
                    break;
                default:
                    {
                        var finger = UISystem.Device.Mouse.Instance.GetCapFinger(bhInit.BehaviorId);
                        if(finger !=null)
                        {
                            MobileProcBehavior(finger,bhInit,reArgs);
                        }
                    }
                    break;
            }
#endif
        }

        private void MobileProcBehavior(WinBase finger, CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs reArgs)
        {
            finger.PreProcBehavior(bhInit, reArgs);//直传消息
            var win = UISystem.Device.Mouse.Instance.GetCapFinger(bhInit.BehaviorId);
            if(win !=null)
            {
                // 再处理冒泡消息(由子向父)
                reArgs = GetRoutedEventArgsWithBehaviorType((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()));
                win.ProcBehavior(bhInit, reArgs);

                // 处理直接消息
                reArgs = GetRoutedEventArgsWithBehaviorType((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()));
                win.ProcDirBehavior(bhInit, reArgs);
            }
            else
            {
                // 再处理冒泡消息(由子向父)
                reArgs = GetRoutedEventArgsWithBehaviorType((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()));
                finger.ProcBehavior(bhInit, reArgs);

                // 处理直接消息
                reArgs = GetRoutedEventArgsWithBehaviorType((CCore.MsgProc.BehaviorType)(bhInit.GetBehaviorType()));
                finger.ProcDirBehavior(bhInit, reArgs);
            }
        }

        private void Root_PreOnMouseMove(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs args)
        {
            CCore.MsgProc.Behavior.Mouse_Move mmBehavior = init as CCore.MsgProc.Behavior.Mouse_Move;

            var msPt = new CSUtility.Support.Point(mmBehavior.X, mmBehavior.Y);

            // 处理模态对话框
            WinBase rootWin = this;
            if (DialogForms.Count > 0)
            {
                rootWin = DialogForms.Peek();
            }

            var popStay = PopupedStayWindow(ref msPt);
            WinBase pNewStay = null;
            if (popStay != null)
                pNewStay = popStay.StayWindow(ref msPt, false);
            else
                pNewStay = rootWin.StayWindow(ref msPt, false);

#if WIN
            if (pNewStay != UISystem.Device.Mouse.Instance.FocusWin)
            {
                if (UISystem.Device.Mouse.Instance.FocusWin != null)
                {
                    var arg = new Message.RoutedEventArgs();
                    arg.Source = UISystem.Device.Mouse.Instance.FocusWin;
                    //mStayWin._FWinMouseLeave(ref myMsg.pt, arg);

                    var parent = UISystem.Device.Mouse.Instance.FocusWin;
                    while (!arg.Handled && parent != null)
                    {
                        parent._FWinMouseLeave(ref msPt, arg);
                        parent = parent.Parent as WinBase;
                    }
                }
                if (pNewStay != null)
                {
                    var arg = new Message.RoutedEventArgs();
                    arg.Source = pNewStay;
                    //pNewStay._FWinMouseEnter(ref myMsg.pt, arg);

                    var parent = pNewStay;
                    while (!arg.Handled && parent != null)
                    {
                        parent._FWinMouseEnter(ref msPt, arg);
                        parent = parent.Parent as WinBase;
                    }
                }
                UISystem.Device.Mouse.Instance.Focus(pNewStay);

                //if (mStayWin!=null&&mStayWin.HitTestVisible == false)
                //    mStayWin = null;
            }
#elif ANDROID || IOS

            var focuswin = UISystem.Device.Mouse.Instance.GetCapFinger(init.BehaviorId);

            if (pNewStay != focuswin)
            {
                if (focuswin != null)
                {
 
                }
                if (pNewStay != null)
                {

                }
                UISystem.Device.Mouse.Instance.FocusFinger(pNewStay, init.BehaviorId);
            }
#endif
            //if (mStayWin != null)
            //{
            //    var pt = AbsToLocal(ref msPt);
            //    var arg = new Message.MouseEventArgs(0, pt.X, pt.Y, mmBehavior.button);
            //    var parent = mStayWin;
            //    while (!arg.Handled && parent != null)
            //    {
            //        parent._FWinMouseMove(parent, arg);
            //        parent = parent.Parent as WinBase;
            //    }
            //}
        }

        private void Root_PreOnButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs args)
        {
            CCore.MsgProc.Behavior.Mouse_Key mmBehavior = init as CCore.MsgProc.Behavior.Mouse_Key;

            var msPt = new CSUtility.Support.Point(mmBehavior.X, mmBehavior.Y);

            // 处理模态对话框
            WinBase rootWin = this;
            if (DialogForms.Count > 0)
            {
                rootWin = DialogForms.Peek();
            }

            var popStay = PopupedStayWindow(ref msPt);
            WinBase pNewStay = null;
            if (popStay != null)
                pNewStay = popStay.StayWindow(ref msPt, false);
            else
                pNewStay = rootWin.StayWindow(ref msPt, false);
#if WIN
            if (pNewStay != UISystem.Device.Mouse.Instance.FocusWin)
            {
                UISystem.Device.Mouse.Instance.Focus(pNewStay);
            }
#elif ANDROID || IOS

            if (pNewStay != UISystem.Device.Mouse.Instance.GetCapFinger(init.BehaviorId))
            {
                UISystem.Device.Mouse.Instance.FocusFinger(pNewStay, init.BehaviorId);
            }
#endif
        }

        //private void Root_OnMouseLeftButtonDown(IBehaviorParameter init, UISystem.Message.RoutedEventArgs args)
        //{
        //    var msPt = MidLayer.IMsgRecieverMgr.MousePoint;

        //    Message.Mouse.Instance.MouseLeftButtonDown = true;

        //    var popStay = PopupedStayWindow(ref msPt);
        //    if (popStay != null)
        //    {
        //        for (int i = PopupedWins.Count - 1; i >= 0; i--)
        //        {
        //            var popup = PopupedWins[i];
        //            if (popStay != null)
        //            {
        //                if (popup == popStay)
        //                    continue;
        //            }

        //            if (!popup.StaysOpen)
        //                popup.IsOpen = false;
        //        }
        //    }
        //    else
        //    {
        //        for (int i = PopupedWins.Count - 1; i >= 0; i--)
        //        {
        //            PopupedWins[i].IsOpen = false;
        //        }
        //    }

        //    WinBase pNewStay = null;
        //    if (popStay != null)
        //        pNewStay = popStay.StayWindow(ref msPt, false);
        //    else
        //        pNewStay = StayWindow(ref msPt, false);
        //    if (pNewStay == null)
        //        return;
        //    if (mCapMsgWin == null)
        //    {
        //        WinForm pForm = pNewStay.GetWinForm(this);
        //        if (pForm != null)
        //        {
        //            WinBase pSaveParent = pForm.Parent as WinBase;
        //            //将Form窗口置于最顶端
        //            pForm.Parent = null;
        //            pForm.Parent = pSaveParent;
        //            //pSaveParent.MoveChild(pSaveParent.IndexOfChild(pForm), pSaveParent.GetChildWinCount() - 1);
        //            _FFormToTop(pForm);

        //            //if( pSaveParent==null )
        //            //    break;
        //            //pForm = pSaveParent.GetWinForm( this );
        //        }
        //    }

        //    {
        //        var pt = AbsToLocal(ref msPt);
        //        var arg = new Message.MouseEventArgs(1, pt.X, pt.Y);
        //        var parent = pNewStay;
        //        while (!arg.Handled && parent != null)
        //        {
        //            parent._FWinMouseButtonDown(parent, arg);
        //            parent = parent.Parent as WinBase;
        //        }
        //    }
        //}

        //private void Root_OnMouseLeftButtonUp(IBehaviorParameter init, UISystem.Message.RoutedEventArgs args)
        //{
        //    var msPt = MidLayer.IMsgRecieverMgr.MousePoint;

        //    Message.Mouse.Instance.MouseLeftButtonDown = false;

        //    var popStay = PopupedStayWindow(ref msPt);
        //    WinBase pNewStay = null;
        //    if (popStay != null)
        //        pNewStay = popStay.StayWindow(ref msPt, false);
        //    else
        //        pNewStay = StayWindow(ref msPt, false);

        //    if (pNewStay == null)
        //        return;

        //    {
        //        var pt = AbsToLocal(ref msPt);
        //        var arg = new Message.MouseEventArgs(0, pt.X, pt.Y);
        //        var parent = pNewStay;
        //        while (!arg.Handled && parent != null)
        //        {
        //            parent._FWinMouseButtonUp(parent, arg);
        //            parent = parent.Parent as WinBase;
        //        }
        //    }
        //}

        public override void Draw(UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
        {
            ProcAllPostEvent();

            base.Draw(pipe, zOrder, ref parentMatrix);

            // 最后画Popup
            for (int i = 0; i < PopupedWins.Count; i++ )
            //foreach (var popup in PopupedWins)
            {
                Popup popup = null;
                try
                {
                    popup = PopupedWins[i];
                }
                catch (System.Exception)
                {
                    continue;
                }
                popup.PopupDraw(pipe, zOrder + 10);
            }
        }
        
        private static CSUtility.Performance.PerfCounter LayoutTick = new CSUtility.Performance.PerfCounter("TickUI.Layout");
        private static CSUtility.Performance.PerfCounter UITick = new CSUtility.Performance.PerfCounter("TickUI.Tick");
        public override void Tick(float elapsedMillisecondTime)
        {
            LayoutTick.Begin();
            Layout.LayoutManager.Instance.UpdateLayout();
            LayoutTick.End();

            UITick.Begin();
            base.Tick(elapsedMillisecondTime);
            UITick.End();
        }
    }
}
