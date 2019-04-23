using System;
using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("ToggleButton")]
    [CSUtility.Editor.UIEditor_Control("常用.ToggleButton")]
    public class ToggleButton : Content.ContentControl,
                                UISystem.Interfaces.ITabAble,
                                UISystem.Interfaces.IKeyboardFocusAble
    {
        protected WinState mNormalState;
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
                if (NormalState == null)
                    return Guid.Empty;
                return NormalState.UVAnimId;
            }
            set
            {
                if (NormalState != null)
                    NormalState.UVAnimId = value;
                OnPropertyChanged("NormalStateUVAnimId");
            }
        }

        protected WinState mLightState;
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

        protected WinState mCheckedState;
        [Category("外观")]
        [Browsable(false)]
        public WinState CheckedState
        {
            get { return mCheckedState; }
            set
            {
                mCheckedState.CopyFrom(value);
                OnPropertyChanged("CheckedState");
            }
        }
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("选中图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid CheckedStateUVAnimId
        {
            get
            {
                if (CheckedState == null)
                    return Guid.Empty;
                return CheckedState.UVAnimId;
            }
            set
            {
                if (CheckedState != null)
                    CheckedState.UVAnimId = value;
                OnPropertyChanged("CheckedStateUVAnimId");
            }
        }

        protected WinState mDisableState;
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
                if (DisableState == null)
                    return Guid.Empty;
                return DisableState.UVAnimId;
            }
            set
            {
                if (DisableState != null)
                    DisableState.UVAnimId = value;
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
        //        CheckedState.Text = value;
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
        //        CheckedState.TextAlign = value;
        //        DisableState.TextAlign = value;
        //        OnPropertyChanged("TextAlign");
        //    }
        //}

        // 同父并且GroupName相同的ToggleButton之间只能Check一个
        protected string mGroupName = "";
        [Category("外观"), DisplayName("组名称"), Description("处在同一组中并且同一个父级的对象只能选中一个")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public string GroupName
        {
            get { return mGroupName; }
            set
            {
                mGroupName = value;
                OnPropertyChanged("GroupName");
            }
        }

        protected bool mChecked = false;
        [Category("数据")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public bool Checked
        {
            get { return mChecked; }
            set
            {
                mChecked = value;

                // radiobutton
                if (!string.IsNullOrEmpty(GroupName))
                {
                    if (mChecked)
                    {
                        var parent = this.Parent as WinBase;
                        if (parent != null)
                        {
                            foreach (var child in parent.GetChildWindows())//.ChildWindows)
                            {
                                if (child == this)
                                    continue;

                                if (!(child is ToggleButton))
                                    continue;

                                ToggleButton tg = child as ToggleButton;
                                if (tg.GroupName == this.GroupName)
                                {
                                    tg.Checked = false;
                                    //tg.LightState.UVAnimId = tg.NormalState.UVAnimId;
                                }
                            }
                            //LightState.UVAnimId = CheckedState.UVAnimId;
                        }
                    }
                    else
                    {
                        bool hasCheckedChild = false;
                        var parent = this.Parent as WinBase;
                        if (parent != null)
                        {
                            foreach (var child in parent.GetChildWindows())//.ChildWindows)
                            {
                                if (child == this)
                                    continue;

                                if(!(child is ToggleButton))
                                    continue;

                                ToggleButton tg = child as ToggleButton;
                                if(tg.GroupName == this.GroupName && tg.Checked)
                                {
                                    hasCheckedChild = true;
                                    break;
                                }
                            }
                        }

                        if(!hasCheckedChild)
                            mChecked = true;
                    }
                }

                if (mChecked)
                {
                    if (mMouseEnter)
                    {
                        if (mLightState.UVAnim != null)
                            mWinState = mLightState;
                        else
                            mWinState = mCheckedState;
                    }
                    else
                        mWinState = mCheckedState;
                    if (WinToggleButtonChecked != null)
                        WinToggleButtonChecked(this);
                }
                else
                {
                    if (mMouseEnter)
                    {
                        if (mLightState.UVAnim != null)
                            mWinState = mLightState;
                        else
                            mWinState = mNormalState;
                    }
                    else
                        mWinState = mNormalState;
                    if (WinToggleButtonUnchecked != null)
                        WinToggleButtonUnchecked(this);
                }

                OnPropertyChanged("Checked");
            }
        }

        protected bool mDisable = false;
        [Category("杂项"), DisplayName("无效")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public bool Disable
        {
            get { return mDisable; }
            set
            {
                mDisable = value;
                if (mDisable)
                    mWinState = mDisableState;
                else
                {
                    if (Checked)
                        mWinState = mCheckedState;
                    else
                        mWinState = mNormalState;
                }
                OnPropertyChanged("Disable");
            }
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

        bool mKeyboardFocusEnable = false;
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
                    else if (Checked)
                        mWinState = mCheckedState;
                    else
                        mWinState = mNormalState;
                }

                OnPropertyChanged("KeyboardFocused");
            }
        }

        public ToggleButton()
        {
            mDragEnable = false;

            mNormalState = new WinState(this);
            mLightState = new WinState(this);
            mCheckedState = new WinState(this);
            mDisableState = new WinState(this);

            mWinState = mNormalState;

            WinMouseEnter += this.MouseEnter;
            WinMouseLeave += this.MouseLeave;

        }

        bool mMouseEnter = false;
        protected void MouseEnter(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            if (Disable)
                return;

            if(mLightState.UVAnim != null)
                mWinState = mLightState;

            mMouseEnter = true;
        }

        protected void MouseLeave(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            if (Disable)
                return;

            if (Checked)
                mWinState = mCheckedState;
            else
                mWinState = mNormalState;

            mMouseEnter = false;
        }

        public delegate void FWinToggleButtonClick(WinBase sender, CCore.MsgProc.BehaviorParameter behavior);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinToggleButtonClick WinToggleButtonClick;

        public delegate void FWinToggleButtonChecked(WinBase sender);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinToggleButtonChecked WinToggleButtonChecked;

        public delegate void FWinToggleButtonUnchecked(WinBase sender);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinToggleButtonUnchecked WinToggleButtonUnchecked;

        public delegate void FWinKeepToggleButtonClick(WinBase sender, CCore.MsgProc.BehaviorParameter behavior);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinKeepToggleButtonClick WinKeepToggleButtonClick;

        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch (msg.message)
        //    {
        //        case (UInt32)MidLayer.SysMessage.VWM_LBUTTONDOWN:
        //            {
        //                if (mDisable == false)
        //                {
        //                    WinRoot.GetInstance().CaptureMouse(this);
        //                    Checked = !Checked;
        //                }
        //                return MSG_PROC.Finished;
        //            }
        //        case (UInt32)MidLayer.SysMessage.VWM_LBUTTONUP:
        //            {
        //                if (mDisable == false)
        //                {
        //                    WinRoot.GetInstance().ReleaseMouse(this);
        //                    if (WinToggleButtonClick != null && AbsRect.Contains(msg.pt))
        //                        WinToggleButtonClick(this);
        //                }
        //                return MSG_PROC.Finished;
        //            }
        //    }

        //    return base.OnMsg(ref msg);
        //}

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Down, ToggleButton_KeyDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Up, ToggleButton_KeyUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, ToggleButton_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, ToggleButton_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);

            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Down, ToggleButton_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Up, ToggleButton_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Down, ToggleButton_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Up, ToggleButton_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);

            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, WinBase_OnMouseRightButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, WinBase_OnMouseMove, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinBase_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);
        }

        private void ToggleButton_OnMouseLeftButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDisable)
                return;

            IsLeftButtonClick = true;
            mKeepClickInit = init;

            UISystem.Device.Keyboard.Instance.Focus(this);

            UISystem.Device.Mouse.Instance.Capture(this,init.GetBehaviorType());
            eventArgs.Handled = true;

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonDown(this, arg);
            _FWinLeftMouseButtonDown(this, arg);
        }

        private void ToggleButton_OnMouseLeftButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDisable)
                return;

            UISystem.Device.Mouse.Instance.ReleaseCapture(init.BehaviorId);

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonUp(this, arg);
            _FWinLeftMouseButtonUp(this, arg);


            if (WinToggleButtonClick != null && AbsRect.Contains(UISystem.Device.Mouse.Instance.Position) && !IsKeepButtonDown)
            {
                Checked = !Checked;
                WinToggleButtonClick(this, init);
            }
            IsKeepButtonDown = false;
            ResetKeepData();
        }

        void ResetKeepData()
        {
            KeepClickTimes = 3000f;
            IsLeftButtonClick = false;
        }

        private void ToggleButton_KeyDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDisable)
                return;

            var key = init as CCore.MsgProc.Behavior.KB_Char;
            switch (key.Key)
            {
                case CCore.MsgProc.BehaviorParameter.enKeys.Tab:
                    TabPress();
                    break;

                case CCore.MsgProc.BehaviorParameter.enKeys.Enter:
                case CCore.MsgProc.BehaviorParameter.enKeys.Space:
                    {
                        mWinState = mCheckedState;
                        Checked = true;
                    }
                    break;
            }
        }

        private void ToggleButton_KeyUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
        }

        bool IsLeftButtonClick = false;
        bool IsKeepButtonDown = false;
        float KeepClickTimes = 3000f;
        CCore.MsgProc.BehaviorParameter mKeepClickInit = null;
        public override void Tick(float elapsedMillisecondTime)
        {
            base.Tick(elapsedMillisecondTime);

            if (IsLeftButtonClick)
            {
                KeepClickTimes -= elapsedMillisecondTime;
                if (KeepClickTimes <= 0)
                {
                    ResetKeepData();
                    if (WinKeepToggleButtonClick != null)
                    {
                        IsKeepButtonDown = true;
                        WinKeepToggleButtonClick(this, mKeepClickInit);
                    }
                }
            }
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Disable"))
                pXml.AddAttrib("Disable", Disable.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "GroupName"))
                pXml.AddAttrib("GroupName", GroupName);
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Checked"))
                pXml.AddAttrib("Checked", Checked.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TabIndex"))
                pXml.AddAttrib("TabIndex", TabIndex.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "KeyboardFocusEnable"))
                pXml.AddAttrib("KeyboardFocusEnable", KeyboardFocusEnable.ToString());

            if (mNormalState != null)
            {
                var stateNode = pXml.AddNode("NormalState", "", holder);
                mNormalState.OnSave(stateNode,holder);
            }
            if (mLightState != null)
            {
                var stateNode = pXml.AddNode("LightState", "",holder);
                mLightState.OnSave(stateNode,holder);
            }
            if (mCheckedState != null)
            {
                var stateNode = pXml.AddNode("CheckedState", "",holder);
                mCheckedState.OnSave(stateNode,holder);
            }
            if (mDisableState != null)
            {
                var stateNode = pXml.AddNode("DisableState", "",holder);
                mDisableState.OnSave(stateNode,holder);
            }
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlAttrib attr = pXml.FindAttrib("Disable");
            if (attr != null)
                Disable = System.Convert.ToBoolean(attr.Value);
            attr = pXml.FindAttrib("GroupName");
            if (attr != null)
                GroupName = attr.Value;
            attr = pXml.FindAttrib("Checked");
            if (attr != null)
                Checked = System.Convert.ToBoolean(attr.Value);
            attr = pXml.FindAttrib("TabIndex");
            if (attr != null)
                TabIndex = System.Convert.ToInt32(attr.Value);
            attr = pXml.FindAttrib("KeyboardFocusEnable");
            if (attr != null)
                KeyboardFocusEnable = System.Convert.ToBoolean(attr.Value);

            var stateNode = pXml.FindNode("NormalState");
            if (stateNode != null)
            {
                if (mNormalState == null)
                    mNormalState = new WinState(this);
                mNormalState.OnLoad(stateNode);
            }
            stateNode = pXml.FindNode("LightState");
            if (stateNode != null)
            {
                if (mLightState == null)
                    mLightState = new WinState(this);
                mLightState.OnLoad(stateNode);
            }
            stateNode = pXml.FindNode("CheckedState");
            if (stateNode != null)
            {
                if (mCheckedState == null)
                    mCheckedState = new WinState(this);
                mCheckedState.OnLoad(stateNode);
            }
            stateNode = pXml.FindNode("DisableState");
            if (stateNode != null)
            {
                if (mDisableState == null)
                    mDisableState = new WinState(this);
                mDisableState.OnLoad(stateNode);
            }
        }
    }
}
