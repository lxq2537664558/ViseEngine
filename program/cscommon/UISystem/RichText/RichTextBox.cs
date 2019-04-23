using System;
using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_Control("常用.RichTextBox")]
    public class RichTextBox : WinBase,
                               UISystem.Interfaces.ITabAble,
                               UISystem.Interfaces.IKeyboardFocusAble
    {
        public RichTextBox()
        {
            mWinState = new WinState_RichText(this);
            mDoc = new RichText.Document(this);
            //WinRichTextBoxClick += OnClick;

//            KeyboardFocusEnable = true;

            //ed0015c4-824f-4806-9ac7-3b7b9df89fa0
            mCursor.Visibility = Visibility.Hidden;
            mCursor.HorizontalAlignment = UI.HorizontalAlignment.Left;
            mCursor.VerticalAlignment = UI.VerticalAlignment.Top;
            mCursor.IgnoreSaver = true;
            mCursor.VisibleInTreeView = Visibility.Collapsed;
            mCursor.Width = 2;
            mCursor.IsTemplateControl = true;
            mCursor.BackColor = CSUtility.Support.Color.LightGray;
            mCursor.Parent = this;
            mCursor.State.UVAnimId = CSUtility.Support.IFileConfig.TextBoxCursor;

            mIme = new ImeSupport(this);
            OnMessage += mIme.OnMsg;

            WinMouseEnter += this.MouseEnter;
            WinMouseLeave += this.MouseLeave;

            mDoc.CursorPosChange += OnCursorPosChange;
        }
        ~RichTextBox()
        {
        }

        public override bool IsTransparent
        {
            get
            {
                return false;
            }
        }

        //public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    switch (TextAlign)
        //    {
        //        case UI.ContentAlignment.TopLeft:
        //            {
        //                mPenLocalPos.X = 0;
        //                mPenLocalPos.Y = mDoc.TextHeight;
        //            }
        //            break;
        //        case UI.ContentAlignment.TopRight:
        //            {
        //                mPenLocalPos.X = Width - mDoc.TextWidth;
        //                mPenLocalPos.Y = mDoc.TextHeight;
        //            }
        //            break;
        //        case UI.ContentAlignment.TopCenter:
        //            {
        //                mPenLocalPos.X = Width / 2 - mDoc.TextWidth / 2;
        //                mPenLocalPos.Y = mDoc.TextHeight;
        //            }
        //            break;
        //        case UI.ContentAlignment.BottomLeft:
        //            {
        //                mPenLocalPos.X = 0;
        //                mPenLocalPos.Y = Height - mDoc.TextHeight;
        //            }
        //            break;
        //        case UI.ContentAlignment.BottomRight:
        //            {
        //                mPenLocalPos.X = Width - mDoc.TextWidth;
        //                mPenLocalPos.Y = Height - mDoc.TextHeight;
        //            }
        //            break;
        //        case UI.ContentAlignment.BottomCenter:
        //            {
        //                mPenLocalPos.X = Width / 2 - mDoc.TextWidth / 2;
        //                mPenLocalPos.Y = Height - mDoc.TextHeight;
        //            }
        //            break;
        //        case UI.ContentAlignment.MiddleLeft:
        //            {
        //                mPenLocalPos.X = 0;
        //                mPenLocalPos.Y = Height / 2 - mDoc.TextHeight / 2;
        //            }
        //            break;
        //        case UI.ContentAlignment.MiddleRight:
        //            {
        //                mPenLocalPos.X = Width - mDoc.TextWidth;
        //                mPenLocalPos.Y = Height / 2 - mDoc.TextHeight / 2;
        //            }
        //            break;
        //        case UI.ContentAlignment.MiddleCenter:
        //            {
        //                mPenLocalPos.X = Width / 2 - mDoc.TextWidth / 2;
        //                mPenLocalPos.Y = Height / 2 - mDoc.TextHeight / 2;
        //            }
        //            break;
        //        default:
        //            mPenLocalPos.X = 0;
        //            mPenLocalPos.Y = 0;
        //            break;
        //    }
        //    mPenPos = LocalToAbs(ref mPenLocalPos);

        //    mDoc.Update(mPenPos.X, mPenPos.Y, Width, Height);
        //}

        protected int mTabIndex = -1;
        [Category("行为"), Description("Tab焦点顺序")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public int TabIndex
        {
            get { return mTabIndex; }
            set
            {
                mTabIndex = value;

                var parWin = Parent as WinBase;
                if (parWin != null)
                {
                    parWin.CalculateTabIndexMaxMinValue();
                }

                OnPropertyChanged("TabIndex");
            }
        }

        protected bool mReadOnly = false;
        [Category("行为"), Description("是否只读"), DisplayName("只读")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public bool ReadOnly
        {
            get { return mReadOnly; }
            set
            {
                mReadOnly = value;
                KeyboardFocusEnable = !mReadOnly;

                OnPropertyChanged("ReadOnly");
            }
        }

        
        public void TabPress()
        {
            var parWin = Parent as WinBase;
            if (parWin != null)
            {
                var win = parWin.FindNextTabItem(TabIndex + 1);
                if (win != null && win != this)
                {
                    UISystem.Device.Keyboard.Instance.Focus(win);
                }
            }
        }

        RichText.Document mDoc;
        //[Category("文本属性"), DisplayName("文档")]
        //[CSUtility.Editor.UIEditor_DocumentEditorAttribute]
        [Browsable(false)]
        public RichText.Document Doc
        {
            get{return mDoc;}
            set 
            {
                mDoc = value;
                mDoc.ParentCtrl = this;

                OnPropertyChanged("Doc");
            }
        }

        [Category("文本属性")]
        [DisplayName("普通文本")]
        [CSUtility.Editor.UIEditor_DocumentTextEditor]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public string Text
        {
            get 
            {
                string text = "";
                if (mDoc != null)
                {
                    foreach (var obj in mDoc.FragmentObjs)
                    {
                        var textObj = obj as UISystem.RichText.TextObj;
                        if(textObj!=null)
                            text += textObj.Content;
                    }
                }
                return text;
            }
            set
            {
                if (mDoc == null)
                    return;

//                 mDoc.FragmentObjs.Clear();
//                 mDoc.OrginFragmentObjs.Clear();
//                 var textObj = UISystem.RichText.FragmentObjFactoryMgr.GetInstance().CreateFragmentObj("text", mDoc);
//                 if (textObj != null)
//                 {
//                    mDoc.FragmentObjs.AddLast(textObj);
//                    mDoc.OrginFragmentObjs.AddLast(textObj);
//                    mDoc.Update();
//                 }

                OnPropertyChanged("Text");
            }
        }


        [Category("文本属性")]
        [DisplayName("格式化文本")]
        [CSUtility.Editor.UIEditor_DocumentTextEditor]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public string OriginText
        {
            get
            {
                if(mDoc!=null)
                    return mDoc.Text;
                return "";
            }
            set
            {
                if (mDoc.Text == value)
                    return;

                if (mDoc == null)
                    return;

                mDoc.Text = value;

                OnPropertyChanged("OriginText");
            }
        }


        [Category("文本属性")]
        [DisplayName("字体")]
        [CSUtility.Editor.UIEditor_OpenFileEditorAttribute("ttf"), CSUtility.Editor.UIEditor_DefaultFontPathAttribute]
        public string FontName
        {
            get { return mDoc == null ? "" : mDoc.FontName; }
            set
            {
                if (mDoc == null)
                    return;

                mDoc.FontName = value;
                OnPropertyChanged("FontName");
            }
        }

        [Category("文本属性")]
        [DisplayName("文字大小")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public int FontSize
        {
            get { return mDoc == null ? 0 : mDoc.FontSize; }
            set
            {
                if (mDoc == null)
                    return;
                mDoc.FontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        [CSUtility.Editor.UIEditor_FontParamCollectionAttribute]
        [Category("文本属性")]
        [DisplayName("显示效果")]
        public CCore.Font.FontRenderParamList FontRenderParams
        {
            get { return mDoc == null ? null : mDoc.FontRenderParams; }
            set
            {
                if (mDoc == null)
                    return;
                mDoc.FontRenderParams = value;
                OnPropertyChanged("FontRenderParams");
            }
        }

        [Category("文本属性")]
        [DisplayName("行间距")]
        public int LineHeight
        {
            get { return mDoc == null ? 0 : mDoc.LineHeight; }
            set
            {
                if (mDoc == null)
                    return;
                mDoc.LineHeight = value;
                OnPropertyChanged("LineHeight");
            }
        }

        [Category("文本属性")]
        [DisplayName("对齐方式")]
        public UI.TextAlignment TextAlignment
        {
            get { return mDoc == null ? 0 : mDoc.TextAlignment; }
            set
            {
                if (mDoc == null)
                    return;
                mDoc.TextAlignment = value;
                OnPropertyChanged("TextAlignment");
            }
        }

        CSUtility.Support.Point mPenPos = new CSUtility.Support.Point( 0,0 );
        CSUtility.Support.Point mPenLocalPos = new CSUtility.Support.Point(0, 0);

        protected UI.ContentAlignment mTextAlign = UI.ContentAlignment.MiddleCenter;
        [Category("外观"), DisplayName("文本布局")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public UI.ContentAlignment TextAlign
        {
            get { return mTextAlign; }
            set
            {
                mTextAlign = value;

                UpdateTextAlign();

                OnPropertyChanged("TextAlign");
                UpdateLayout();
            }
        }

        [Category("文本属性"), DisplayName("换行模式")]
        public UI.TextWrapping TextWrapping
        {
            get { return mDoc == null ? UI.TextWrapping.WrapWithOverflow : mDoc.TextWrapping; }
            set
            {
                if (mDoc == null)
                    return;
                mDoc.TextWrapping = value;
                OnPropertyChanged("TextWrapping");
            }
        }

        [Category("文本属性")]
        [DisplayName("宽度过小时是否换行")]
        public bool WrapInSmallWidth
        {
            get { return mDoc == null ? false : mDoc.WrapInSmallWidth; }
            set
            {
                if (mDoc == null)
                    return;
                mDoc.WrapInSmallWidth = value;
                OnPropertyChanged("WrapInSmallWidth");
            }
        }

        [Browsable(false)]
        [Category("外观")]
        public WinState State
        {
            get { return mWinState; }
            set
            {
                mWinState.CopyFrom(value);
                OnPropertyChanged("State");
            }
        }
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("背景图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid StateUVAnimId
        {
            get
            {
                if (mWinState == null)
                    return Guid.Empty;
                return mWinState.UVAnimId;
            }
            set
            {
                if (mWinState != null)
                    mWinState.UVAnimId = value;
                OnPropertyChanged("StateUVAnimId");
            }
        }

        public delegate void FWinRichTextBoxClick(WinBase Sender, Message.MouseEventArgs e);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
		public event FWinRichTextBoxClick WinRichTextBoxClick;
        public delegate void FWinRichTextBoxRClick(WinBase Sender, Message.MouseEventArgs e);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinRichTextBoxRClick WinRichTextBoxRClick;

        ImeSupport mIme;
        public delegate void FOnMessage(ref WinMSG msg, ref string inputStr);
        public event FOnMessage OnMessage;
        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch( msg.message )
        //    {
        //    case (UInt32)MidLayer.SysMessage.VWM_LBUTTONUP:
        //        {
        //            if (WinRichTextBoxClick!=null)
        //                WinRichTextBoxClick(this, ref msg);
        //        }
        //        break;
        //    case (UInt32)MidLayer.SysMessage.VWM_RBUTTONUP:
        //        {
        //            if (WinRichTextBoxRClick != null)
        //                WinRichTextBoxRClick(this, ref msg);
        //        }
        //        break;
        //    case (UInt32)MidLayer.SysMessage.VWM_MOUSEMOVE:
        //        {
        //            mCurPos = msg.pt;
        //            mMouseMoveTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();
        //            if (mHoverState == EHoverState.Hovering)
        //            {
        //                if (Math.Abs(mCurPos.X - mHoverPos.X) > 2 || Math.Abs(mCurPos.Y - mHoverPos.Y) > 2)
        //                {
        //                    if (WinRichTextBoxEndHover != null)
        //                        WinRichTextBoxEndHover(this);
        //                    mHoverState = EHoverState.MouseMoving;
        //                }
        //            }
        //        }
        //        break;
        //    case (int)UISystem.WinMsg.WM_KEYDOWN:
        //        {
        //            if (KeyboardFocusEnable && KeyboardFocus)
        //            {
        //                switch (msg.wParam)
        //                {
        //                    case 0x2E:          // VK_DELETE
        //                        {
        //                            Doc.Delete(1);
        //                        }
        //                        break;
        //                }
        //            }
        //        }
        //        break;
        //    }

        //    if (KeyboardFocusEnable && KeyboardFocus)
        //    {
        //        string inputStr = "";
        //        if (OnMessage != null)
        //            OnMessage(ref msg, ref inputStr);

        //        if (inputStr == "\b")
        //        {
        //            Doc.DeleteBack(1);
        //            Text = Doc.Text;
        //        }
        //        else if (inputStr != "")
        //        {
        //            Doc.InsertString(inputStr);
        //            Text = Doc.Text;
        //        }
        //    }

        //    return base.OnMsg(ref msg);
        //}

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_TextInput, RichTextBox_OnTextInput, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Down, RichTextBox_OnKeyDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, RichTextBox_OnMouseLeftButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, RichTextBox_OnMouseLeftButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, RichTextBox_OnMouseRightButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, RichTextBox_OnMouseMove, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinBase_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);
        }

        public delegate void FEnterClick(WinBase sender);
        [CSUtility.Editor.UIEditor_CommandEventAttribute]
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FEnterClick EnterClick;

        protected void RichTextBox_OnKeyDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mReadOnly == true)
                return;

            var kb = init as CCore.MsgProc.Behavior.KB_Char;

            switch (kb.Key)
            {
                case CCore.MsgProc.BehaviorParameter.enKeys.Delete:
                    Doc.Delete(1);
                    UpdateLayout();
                    OnPropertyChanged("Text");
                    break;
                case CCore.MsgProc.BehaviorParameter.enKeys.Back:
                    Doc.DeleteBack(1);
                    UpdateLayout();
                    OnPropertyChanged("Text");
                    break;
                case CCore.MsgProc.BehaviorParameter.enKeys.Tab:
                    TabPress();
                    break;
                case CCore.MsgProc.BehaviorParameter.enKeys.Enter:
                    {
                        if (EnterClick != null)
                            EnterClick(this);
                    }
                    break;
            }
        }
        protected void RichTextBox_OnMouseLeftButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            Doc.PointClick(mk.X, mk.Y);
            Doc.PointMouseClick(mk.X, mk.Y);

            //mCursor.Height = Doc.GetCursorHeight();
            //mCursor.Margin = new CSCommon.Support.Thickness(Doc.CursorX - AbsRect.X, Doc.CursorY - AbsRect.Y - mCursor.Height / 2, 0, 0);

            UISystem.Device.Keyboard.Instance.Focus(this);

            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);
            _FWinMouseButtonDown(this, arg);
            _FWinLeftMouseButtonDown(this, arg);

            eventArgs.Handled = true;
        }
        protected void RichTextBox_OnMouseLeftButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;

            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);
            if (WinRichTextBoxClick != null)
                WinRichTextBoxClick(this, arg);

            _FWinMouseButtonUp(this, arg);
            _FWinLeftMouseButtonUp(this, arg);

            eventArgs.Handled = true;
        }
        protected void RichTextBox_OnMouseRightButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);

            if (WinRichTextBoxRClick != null)
                WinRichTextBoxRClick(this, arg);

            _FWinMouseButtonUp(this, arg);
            _FWinRightMouseButtonUp(this, arg);
        }

        protected void RichTextBox_OnTextInput(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mReadOnly == true)
                return;

            var tiMsg = init as CCore.MsgProc.Behavior.TextInput;

            WinMSG myMsg = new WinMSG();
            myMsg.message = tiMsg.Msg.Msg;
            myMsg.hwnd = tiMsg.Msg.HWnd;
            myMsg.lParam = tiMsg.Msg.LParam;
            myMsg.wParam = tiMsg.Msg.WParam;
            myMsg.time = CCore.Engine.Instance.GetFrameMillisecond();
            myMsg.pt.X = UISystem.WinRoot.GET_X_LPARAM(myMsg.lParam);
            myMsg.pt.Y = UISystem.WinRoot.GET_Y_LPARAM(myMsg.lParam);

            string inputStr = "";
            if (OnMessage != null)
                OnMessage(ref myMsg, ref inputStr);

            if (inputStr == "\t" ||     // tab
                inputStr == "\b" ||     // backspace
                inputStr == "\r" ||     // Enter
                inputStr == "\n"
                )      
            {

            }
            //if (inputStr == "\b")
            //{
            //    Doc.DeleteBack(1);
            //    Text = Doc.Text;
            //}
            else if (inputStr != "")
            {
                Doc.InsertString(inputStr);
                Text = Doc.Text;
            }

            if(!string.IsNullOrEmpty(inputStr))
                UpdateLayout();
        }

        protected void RichTextBox_OnMouseMove(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mm = init as CCore.MsgProc.Behavior.Mouse_Move;
            var arg = new Message.MouseEventArgs(mm.Clicks, mm.X, mm.Y, mm.button);
            Doc.PointMouseMove(mm.X, mm.Y);

            _FWinMouseMove(this, arg);
        }

        protected void MouseEnter(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            Doc.PointMouseEnter(pt.X, pt.Y);
        }
        protected void MouseLeave(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            Doc.PointMouseLeave(pt.X, pt.Y);
        }

        float mHoverStayElapseTime = 1.0F;
        [Category("行为"), DisplayName("鼠标悬停时间"), Description("鼠标开始悬停到鼠标悬停事件触发的间隔时间")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float HoverStayElapseTime
        {
            get { return mHoverStayElapseTime; }
            set { mHoverStayElapseTime = value; }
        }

//        void OnClick(WinBase Sender, ref WinMSG msg)
//        {
//            Doc.PointClick(msg.pt.X, msg.pt.Y);

//            mCursor.Margin = new CSCommon.Support.Thickness(Doc.CursorX - AbsRect.X, Doc.CursorY - AbsRect.Y, 0, 0);
//            mCursor.Height = Doc.GetCursorHeight();

////            UISystem.WinKeyboard.Instance.Focus(this);
//        }

        //protected override void OnGotKeyboardFocus()
        //{
        //    mIme.OnGotKeyboardFocus();
        //    mCursor.Visibility = Visibility.Visible;
        //}

        //protected override void OnLostKeyboardFocus()
        //{
        //    mIme.OnLostKeyboardFocus();
        //    mCursor.Visibility = Visibility.Hidden;
        //}

        void OnCursorPosChange()
        {
            mCursor.Height = mDoc.CursorHeight;

            //mCursor.Margin = new CSCommon.Support.Thickness(Doc.CursorX - AbsRect.X, Doc.CursorY - AbsRect.Y + 2, 0, 0);
            mCursor.Margin = new CSUtility.Support.Thickness(Doc.CursorX - AbsRect.X, Doc.CursorY - AbsRect.Y, 0, 0);
        }

        Image mCursor = new Image();
        [Category("光标"), DisplayName("光标图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid Cursor
        {
            get { return mCursor.State.UVAnimId; }
            set
            {
                mCursor.State.UVAnimId = value;
                OnPropertyChanged("Cursor");
            }
        }

        //int mCursorWidth = 5;
        [Category("光标"), DisplayName("光标宽度")]
        public int CursorWidth
        {
            get { return mCursor.Width; }
            set
            {
                mCursor.Width = value;
                OnPropertyChanged("CursorWidth");
            }
        }

        bool mKeyboardFocusEnable = true;
        [Category("行为"), DisplayName("允许键盘焦点")]
        public bool KeyboardFocusEnable
        {
            get { return mKeyboardFocusEnable; }
            set
            {
                mKeyboardFocusEnable = value;
                OnPropertyChanged("KeyboardFocusEnable");
            }
        }
        bool mKeyboardFocused = false;
        [Browsable(false)]
        public bool KeyboardFocused
        {
            get { return mKeyboardFocused; }
            set
            {
                mKeyboardFocused = value;

                if (mKeyboardFocused)
                {
                    mIme.OnGotKeyboardFocus();
                    mCursor.Visibility = Visibility.Visible;
                }
                else
                {
                    mIme.OnLostKeyboardFocus();
                    mCursor.Visibility = Visibility.Hidden;
                }
                
                OnPropertyChanged("KeyboardFocused");
            }
        }

        protected override SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            SlimDX.Size returnDesiredSize = SlimDX.Size.Empty;

            returnDesiredSize = new SlimDX.Size(availableSize.Width, availableSize.Height);

            if (Width_Auto == true)
            {
                int iMaxWidth = Doc.MeasureMaxWidth();

                if (iMaxWidth < availableSize.Width)
                {
                    returnDesiredSize.Width = iMaxWidth;
                }
            }

            if (Height_Auto == true)
            {
                // 根据前边算出的Width， 计算出文本高度
                Doc.Update(mPenPos.X, mPenPos.Y, (int)returnDesiredSize.Width, (int)returnDesiredSize.Height);
                returnDesiredSize.Height = Doc.TextHeight;
            }

            mCursor.Measure(availableSize);
            
            return returnDesiredSize;
        }
        
        protected override SlimDX.Size ArrangeOverride(SlimDX.Size finalSize)
        {
            Doc.Update(mPenPos.X, mPenPos.Y, (int)finalSize.Width, (int)finalSize.Height);

            mCursor.Arrange(new SlimDX.Rect(finalSize));

            return finalSize;
        }

        protected override void UpdateAbsRect(bool bWithChildren)
        {
            base.UpdateAbsRect(bWithChildren);

            UpdateTextAlign();

            mPenPos = LocalToAbs(ref mPenLocalPos);
            Doc.Update(mPenPos.X, mPenPos.Y, AbsRect.Width, AbsRect.Height);
        }

        protected void UpdateTextAlign()
        {
            // 当没有输入文本的时候， 使用文字高度来计算PenY及CursorY
            int textHeight = mDoc.TextHeight;
            if (textHeight == 0)
            {
                textHeight = mDoc.FontSize;
            }

            switch (mTextAlign)
            {
                case UI.ContentAlignment.TopLeft:
                    {
                        mPenLocalPos.X = 0;
                        mPenLocalPos.Y = 0;
                        //mPenLocalPos.Y = mDoc.TextHeight;
                    }
                    break;
                case UI.ContentAlignment.TopRight:
                    {
                        mPenLocalPos.X = Width - mDoc.TextWidth;
                        mPenLocalPos.Y = 0;
                        //mPenLocalPos.Y = mDoc.TextHeight;
                    }
                    break;
                case UI.ContentAlignment.TopCenter:
                    {
                        mPenLocalPos.X = Width / 2 - mDoc.TextWidth / 2;
                        mPenLocalPos.Y = 0;
                        //mPenLocalPos.Y = mDoc.TextHeight;
                    }
                    break;
                case UI.ContentAlignment.BottomLeft:
                    {
                        mPenLocalPos.X = 0;
                        mPenLocalPos.Y = Height - textHeight;
                    }
                    break;
                case UI.ContentAlignment.BottomRight:
                    {
                        mPenLocalPos.X = Width - mDoc.TextWidth;
                        mPenLocalPos.Y = Height - textHeight;
                    }
                    break;
                case UI.ContentAlignment.BottomCenter:
                    {
                        mPenLocalPos.X = Width / 2 - mDoc.TextWidth / 2;
                        mPenLocalPos.Y = Height - textHeight;
                    }
                    break;
                case UI.ContentAlignment.MiddleLeft:
                    {
                        mPenLocalPos.X = 0;
                        mPenLocalPos.Y = Height / 2 - textHeight / 2;
                    }
                    break;
                case UI.ContentAlignment.MiddleRight:
                    {
                        mPenLocalPos.X = Width - mDoc.TextWidth;
                        mPenLocalPos.Y = Height / 2 - textHeight / 2;
                    }
                    break;
                case UI.ContentAlignment.MiddleCenter:
                    {
                        mPenLocalPos.X = Width / 2 - mDoc.TextWidth / 2;
                        mPenLocalPos.Y = Height / 2 - textHeight / 2;
                    }
                    break;
                default:
                    mPenLocalPos.X = 0;
                    mPenLocalPos.Y = 0;
                    break;
            }
            mPenPos = LocalToAbs(ref mPenLocalPos);

            mDoc.Update(mPenPos.X, mPenPos.Y, Width, Height);
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (State != null)
            {
                CSUtility.Support.XmlNode state = pXml.AddNode("WinState", "", holder);
                State.OnSave(state,holder);
            }
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Cursor"))
                pXml.AddAttrib("Cursor", Cursor.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ReadOnly"))
                pXml.AddAttrib("ReadOnly", mReadOnly.ToString());

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TextAlign"))
                pXml.AddAttrib("TextAlign", TextAlign.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TabIndex"))
                pXml.AddAttrib("TabIndex", TabIndex.ToString());

            Doc.OnSave(pXml,holder);
        }
        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            Doc.OnLoad(pXml);

            base.OnLoad(pXml);

            //CSUtility.Support.IXmlAttrib attr = null;

            CSUtility.Support.XmlNode stateNode = pXml.FindNode("WinState");
            if (stateNode != null)
            {
                if (State == null)
                    mWinState = new WinState_RichText(this);
                State.OnLoad(stateNode);
            }
            var att = pXml.FindAttrib("Cursor");
            if (att != null)
            {
                Cursor = CSUtility.Support.IHelper.GuidTryParse(att.Value);
                if (Cursor.Equals( Guid.Empty ))
                {
                    Cursor = CSUtility.Support.IHelper.GuidParse("ed0015c4-824f-4806-9ac7-3b7b9df89fa0");
                }
            }
            att = pXml.FindAttrib("ReadOnly");
            if (att != null)
            {
                ReadOnly = bool.Parse(att.Value);
            }

            att = pXml.FindAttrib("TextAlign");
            if (att != null)
            {
                TextAlign = CSUtility.Support.IHelper.EnumTryParse<UI.ContentAlignment>(att.Value); ;
            }
            att = pXml.FindAttrib("TabIndex");
            if (att != null)
            {
                TabIndex = System.Convert.ToInt32(att.Value); 
            }

        }
        
    }
}
