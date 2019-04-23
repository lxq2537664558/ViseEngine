using System;
using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("Button")]
    [CSUtility.Editor.UIEditor_Control("常用.Button")]
    public class Button : Content.ContentControl, 
                          UISystem.Interfaces.ITabAble, 
                          UISystem.Interfaces.IKeyboardFocusAble,
                          UISystem.Interfaces.IHotKey
    {
        protected WinState mNormalState;
		protected WinState mLightState;
		protected WinState mPressState;
		protected WinState mDisableState;
        protected bool mDisable;
		public Button()
        {
            mNormalState = new WinState(this);
            mLightState = new WinState(this);
            mPressState = new WinState(this);
            mDisableState = new WinState(this);

		    mLightState.TextOffset = new CSUtility.Support.Point(-1,-1);
            mPressState.TextOffset = new CSUtility.Support.Point(1, 1);

		    mWinState = mNormalState;

		    WinMouseEnter += this.MouseEnter;
		    WinMouseLeave += this.MouseLeave;
        }

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

        [Category("外观")]
        [Browsable(false)]
		public WinState NormalState
        {
			get { return mNormalState; }
            set
            {
                mNormalState.CopyFrom(value);
                OnPropertyChanged("NormalState");
            }
		}
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("正常图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid NormalStateUVAnimId
        {
            get
            {
                if (mNormalState == null)
                    return Guid.Empty;
                return mNormalState.UVAnimId;
            }
            set
            {
                if (mNormalState != null)
                    mNormalState.UVAnimId = value;
                OnPropertyChanged("NormalStateUVAnimId");
            }
        }

        [Category("外观")]
        [Browsable(false)]
		public WinState LightState
        {
			get { return mLightState; }
            set
            {
                mLightState.CopyFrom(value);
                OnPropertyChanged("LightState");
            }
		}
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("高亮图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid LightStateUVAnimId
        {
            get
            {
                if (LightState == null)
                    return Guid.Empty;
                return LightState.UVAnimId;
            }
            set
            {
                if (LightState != null)
                    LightState.UVAnimId = value;
                OnPropertyChanged("LightStateUVAnimId");
            }
        }

        [Category("外观")]
        [Browsable(false)]
		public WinState PressState
        {
			get { return mPressState; }
            set
            {
                mPressState.CopyFrom(value);
                OnPropertyChanged("PressState");
            }
		}
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("按下图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid PressStateUVAnimId
        {
            get
            {
                if (PressState == null)
                    return Guid.Empty;
                return PressState.UVAnimId;
            }
            set
            {
                if (PressState != null)
                    PressState.UVAnimId = value;
                OnPropertyChanged("PressStateUVAnimId");
            }
        }

        [Category("外观")]
        [Browsable(false)]
		public WinState DisableState
        {
			get { return mDisableState; }
            set
            {
                mDisableState.CopyFrom(value);
                OnPropertyChanged("DisableState");
            }
		}

        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("失效图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid DisableStateUVAnimId
        {
            get
            {
                if (mDisableState == null)
                    return Guid.Empty;
                return mDisableState.UVAnimId;
            }
            set
            {
                if (mDisableState != null)
                    mDisableState.UVAnimId = value;
                OnPropertyChanged("DisableStateUVAnimId");
            }
        }

        //[Category("外观")]
        //[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        //public override string Text
        //{
        //    get { return base.Text; }
        //    set
        //    {
        //        base.Text = value;
        //        NormalState.Text = value;
        //        LightState.Text = value;
        //        PressState.Text = value;
        //        DisableState.Text = value;
        //        OnPropertyChanged("Text");
        //    }
        //}
        //[Category("外观")]
        //[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        //public override UI.ContentAlignment TextAlign
        //{
        //    get { return base.TextAlign; }
        //    set
        //    {
        //        base.TextAlign = value;
        //        NormalState.TextAlign = value;
        //        LightState.TextAlign = value;
        //        PressState.TextAlign = value;
        //        DisableState.TextAlign = value;
        //        OnPropertyChanged("TextAlign");
        //    }
        //}

        [Category("杂项")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("无效")]
		public bool Disable
        {
			get { return mDisable; }
			set
            {
                mDisable = value;
                if (mDisable)
                {
                    mWinState = mDisableState;
                }
                else
                {
                    mWinState = mNormalState;
                }
                OnPropertyChanged("Disable");
            }
		}

        bool mIsCloseButton = false;
        [Category("杂项")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("是否是关闭按钮")]
        [Description("是关闭按钮时则点击会关闭此按钮所在的窗口")]
        public bool IsCloseButton
        {
            get { return mIsCloseButton; }
            set
            {
                mIsCloseButton = value;

                OnPropertyChanged("IsCloseButton");
            }
        }

        string mClickSound = "";
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("SoundSet")]
        [DisplayName("点击声音")]
        public string ClickSound
        {
            get { return mClickSound; }
            set
            {
                mClickSound = value;
                OnPropertyChanged("ClickSound");
            }
        }

        string mButtonDownSound = "";
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("SoundSet")]
        [DisplayName("按下声音")]
        public string ButtonDownSound
        {
            get { return mButtonDownSound; }
            set
            {
                mButtonDownSound = value;
                OnPropertyChanged("ButtonDownSound");
            }
        }

        string mButtonUpSound = "";
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("SoundSet")]
        [DisplayName("抬起声音")]
        public string ButtonUpSound
        {
            get { return mButtonUpSound; }
            set
            {
                mButtonUpSound = value;
                OnPropertyChanged("");
            }
        }

        string mHostKey = "";
        [CSUtility.Editor.Editor_HotKeySetter]
        [Category("行为"), Description("按下此快捷键后处理按钮点击消息，快捷键全界面唯一")]
        [DisplayName("快捷键")]
        public string HotKey
        {
            get { return mHostKey; }
            set
            {
                UISystem.Device.Keyboard.Instance.UnRegisterHotKey(mHostKey);

                mHostKey = value;

                UISystem.Device.Keyboard.Instance.RegisterHotKey(mHostKey, this);

                OnPropertyChanged("HotKey");
            }
        }

        public void HotKeyDownProcess(CCore.MsgProc.BehaviorParameter be)
        {
            if (mDisable)
                return;

            mWinState = mPressState;

            // 快捷键按下处理
            if (WinButtonDown != null)
            {
                WinButtonDown(this, be);
            }

            if (!string.IsNullOrEmpty(ButtonDownSound))
                CCore.Audio.AudioManager.Instance.SimplePlay(ButtonDownSound, (UInt32)(CCore.Performance.ESoundType.SoundEffect));

        }
        public void HotKeyUpProcess(CCore.MsgProc.BehaviorParameter be)
        {
            // 快捷键弹起处理
            if (AbsRect.Contains(UISystem.Device.Mouse.Instance.Position))
            {
                if (mLightState.UVAnim != null)
                    mWinState = mLightState;
                else
                    mWinState = mNormalState;
            }
            else
                mWinState = mNormalState;

            if (WinButtonClick != null)
            {
                WinButtonClick(this, be);
            }
            if (WinButtonUp != null)
            {
                WinButtonUp(this, be);
            }

            if (!string.IsNullOrEmpty(ButtonUpSound))
                CCore.Audio.AudioManager.Instance.SimplePlay(ButtonUpSound, (UInt32)(CCore.Performance.ESoundType.SoundEffect));
        }
	
        public delegate void FWinButtonClick(WinBase sender, CCore.MsgProc.BehaviorParameter beh);
        [CSUtility.Editor.UIEditor_CommandEventAttribute]
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
		public event FWinButtonClick WinButtonClick;

        [CSUtility.Editor.UIEditor_CommandEventAttribute]
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinButtonClick WinButtonDown;
        [CSUtility.Editor.UIEditor_CommandEventAttribute]
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinButtonClick WinButtonUp;

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Down, Button_KeyDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Up, Button_KeyUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, Button_MouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, Button_MouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);

            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Down, Button_MouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Up, Button_MouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Down, Button_MouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Up, Button_MouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);

            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, WinBase_OnMouseRightButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, WinBase_OnMouseMove, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinBase_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);
        }

        public override void OnMouseFocus()
        {
            base.OnMouseFocus();
        }
        public override void OnMouseUnFocus()
        {
            base.OnMouseUnFocus();

            if(!Disable)
                mWinState = mNormalState;
        }

		protected void MouseEnter( ref CSUtility.Support.Point pt , Message.RoutedEventArgs e )
        {
            //if (mWinState != mPressState)
            if(mLightState.UVAnim != null)
                mWinState = mLightState;
        }
        protected void MouseLeave(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            //if (mWinState != mPressState)
            {
                if (Disable)
                    mWinState = mDisableState;
                else if (KeyboardFocused)
                {
                    if(mLightState.UVAnim != null)
                        mWinState = mLightState;
                }
                else
                    mWinState = mNormalState;
            }
        }
	
        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch( msg.message )
        //    {
        //    case (UInt32)MidLayer.SysMessage.VWM_LBUTTONDOWN:
        //        {
        //            if( DragEnable==false && mDisable==false )
        //            {
        //                WinRoot.GetInstance().CaptureMouse(this);
        //                mWinState = mPressState;
        //            }
        //            return MSG_PROC.Finished;
        //        }
        //    case (UInt32)MidLayer.SysMessage.VWM_LBUTTONUP:
        //        {
        //            if( DragEnable==false && mDisable==false )
        //            {
        //                WinRoot.GetInstance().ReleaseMouse(this);
        //                mWinState = mNormalState;
        //                if (WinButtonClick!=null && AbsRect.Contains(msg.pt) )
        //                {                            
        //                    WinButtonClick(this);
        //                }
        //            }
        //            return MSG_PROC.Finished;
        //        }
        //    };

        //    return base.OnMsg(ref msg);
        //}
        
        private void Button_MouseLeftButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDisable)
                return;

            UISystem.Device.Keyboard.Instance.Focus(this);

			if( DragEnable==false )
			{
                UISystem.Device.Mouse.Instance.Capture(this,init.GetBehaviorType());
				mWinState = mPressState;
			}

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonDown(this, arg);
            _FWinLeftMouseButtonDown(this, arg);
            if (WinButtonDown != null)
                WinButtonDown(this, init);

            if (IsCloseButton)
            {
                var form = this.GetRoot(typeof(WinForm)) as WinForm;
                if (form != null)
                    form.Close(init.BehaviorId);
            }

            if (!string.IsNullOrEmpty(ButtonDownSound))
                CCore.Audio.AudioManager.Instance.SimplePlay(ButtonDownSound, (UInt32)(CCore.Performance.ESoundType.SoundEffect));


            eventArgs.Handled = true;
        }

        private void Button_MouseLeftButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDisable)
                return;

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonUp(this, arg);
            _FWinLeftMouseButtonUp(this, arg);

			if( DragEnable==false )
			{
                UISystem.Device.Mouse.Instance.ReleaseCapture(init.BehaviorId);

                if (AbsRect.Contains(UISystem.Device.Mouse.Instance.Position))
                {
                    if (mLightState.UVAnim != null)
                        mWinState = mLightState;
                    else
                        mWinState = mNormalState;

                }
                else
                    mWinState = mNormalState;

                if (AbsRect.Contains(UISystem.Device.Mouse.Instance.Position))
                {
                    if (WinButtonClick != null)
                    {
                        WinButtonClick(this, init);
                    }

                    if (!string.IsNullOrEmpty(ClickSound))
                    {
                        CCore.Audio.AudioManager.Instance.SimplePlay(ClickSound, (UInt32)(CCore.Performance.ESoundType.SoundEffect));
                    }
                }

                if (WinButtonUp != null)
                    WinButtonUp(this, init);

                if (!string.IsNullOrEmpty(ButtonUpSound))
                    CCore.Audio.AudioManager.Instance.SimplePlay(ButtonUpSound, (UInt32)(CCore.Performance.ESoundType.SoundEffect));
			}

            eventArgs.Handled = true;
        }

        public void TabPress()
        {
            var parWin = Parent as WinBase;
            if (parWin != null)
            {
                var win = parWin.FindNextTabItem(TabIndex+1);
                if (win != null && win != this)
                {
                    UISystem.Device.Keyboard.Instance.Focus(win);
                }
            }
        }

        private void Button_KeyDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDisable)
                return;

            var key = init as CCore.MsgProc.Behavior.KB_Char;
            
            switch (key.Key)
            {
                case CCore.MsgProc.BehaviorParameter.enKeys.Tab:    // 处理按Tab
                    TabPress();
                    break;

                case CCore.MsgProc.BehaviorParameter.enKeys.Enter:
                case CCore.MsgProc.BehaviorParameter.enKeys.Space:
                    {
                        mWinState = mPressState;
                    }
                    break;
            }
        }
        private void Button_KeyUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if(mDisable)
                return;

            var key = init as CCore.MsgProc.Behavior.KB_Char;

            switch (key.Key)
            {
                case CCore.MsgProc.BehaviorParameter.enKeys.Enter:
                case CCore.MsgProc.BehaviorParameter.enKeys.Space:
                    {
                        if(mLightState.UVAnim != null)
                            mWinState = mLightState;

                        if (WinButtonClick != null && AbsRect.Contains(UISystem.Device.Mouse.Instance.Position))
                        {
                            WinButtonClick(this, init);
                        }
                    }
                    break;
            }
        }

        bool mKeyboardFocusEnable = false;
        [Category("行为")]
        [DisplayName("允许键盘焦点")]
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
                    if(mLightState.UVAnim != null)
                        mWinState = mLightState;
                }
                else
                {
                    if (AbsRect.Contains(UISystem.Device.Mouse.Instance.Position))
                    {
                        if(mLightState.UVAnim != null)
                            mWinState = mLightState;
                    }
                    else
                        mWinState = mNormalState;
                }

                OnPropertyChanged("KeyboardFocused");
            }
        }
        
        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
		    base.OnSave(pXml,holder);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Disable"))
                pXml.AddAttrib("Disable", Disable.ToString());

		    if(mNormalState!=null)
		    {
                CSUtility.Support.XmlNode state = pXml.AddNode("NormalState", "",holder);
			    mNormalState.OnSave( state , holder);
		    }
		    if(mLightState!=null)
		    {
                CSUtility.Support.XmlNode state = pXml.AddNode("LightState", "",holder);
			    mLightState.OnSave( state , holder);
		    }
		    if(mPressState!=null)
		    {
                CSUtility.Support.XmlNode state = pXml.AddNode("PressState", "",holder);
			    mPressState.OnSave( state , holder);
		    }
		    if(mDisableState!=null)
		    {
                CSUtility.Support.XmlNode state = pXml.AddNode("DisableState", "",holder);
			    mDisableState.OnSave( state , holder);
		    }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "KeyboardFocusEnable"))
                pXml.AddAttrib("KeyboardFocusEnable", KeyboardFocusEnable.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TabIndex"))
                pXml.AddAttrib("TabIndex", TabIndex.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "HotKey"))
                pXml.AddAttrib("HotKey", HotKey);
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "IsCloseButton"))
                pXml.AddAttrib("IsCloseButton", IsCloseButton.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ClickSound"))
                pXml.AddAttrib("ClickSound", ClickSound.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ButtonDownSound"))
                pXml.AddAttrib("ButtonDownSound", ButtonDownSound.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ButtonUpSound"))
                pXml.AddAttrib("ButtonUpSound", ButtonUpSound.ToString());

        }
        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

		    CSUtility.Support.XmlAttrib attr = pXml.FindAttrib( "Disable" );
		    if(attr!=null)
			    Disable = System.Convert.ToBoolean( attr.Value );

		    CSUtility.Support.XmlNode state = pXml.FindNode( "NormalState" );
		    if(state!=null)
		    {
			    if(mNormalState==null)
                    mNormalState = new WinState(this);
			    mNormalState.OnLoad(state);
		    }
            mWinState = mNormalState;
		    state = pXml.FindNode( "LightState" );
		    if(state!=null)
		    {
			    if(mLightState==null)
                    mLightState = new WinState(this);
			    mLightState.OnLoad(state);
		    }
		    state = pXml.FindNode( "PressState" );
		    if(state!=null)
		    {
			    if(mPressState==null)
                    mPressState = new WinState(this);
			    mPressState.OnLoad(state);
		    }
		    state = pXml.FindNode( "DisableState" );
		    if(state!=null)
		    {
			    if(mDisableState==null)
                    mDisableState = new WinState(this);
			    mDisableState.OnLoad(state);
		    }

            attr = pXml.FindAttrib("KeyboardFocusEnable");
            if (attr != null)
                KeyboardFocusEnable = System.Convert.ToBoolean(attr.Value);
            attr = pXml.FindAttrib("TabIndex");
            if (attr != null)
                TabIndex = System.Convert.ToInt32(attr.Value);
            attr = pXml.FindAttrib("HotKey");
            if (attr != null)
                HotKey = attr.Value;
            attr = pXml.FindAttrib("IsCloseButton");
            if (attr != null)
                IsCloseButton = System.Convert.ToBoolean(attr.Value);
            attr = pXml.FindAttrib("ClickSound");
            if (attr != null)
                ClickSound = attr.Value;
            attr = pXml.FindAttrib("ButtonDownSound");
            if (attr != null)
                ButtonDownSound = attr.Value;
            attr = pXml.FindAttrib("ButtonUpSound");
            if (attr != null)
                ButtonUpSound = attr.Value;
        }
    }
}
