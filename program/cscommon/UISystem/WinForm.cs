using System;
using System.ComponentModel;

namespace UISystem
{
    public class WinForm : WinBase , IDisposable
    {
        Trigger.UITriggerManager mUITriggerManager;
        [Browsable(false)]
        public Trigger.UITriggerManager UITriggerManager
        {
            get { return mUITriggerManager; }
        }

        Bind.PropertyBindManager mPropertyBindManager;
        [Browsable(false)]
        public Bind.PropertyBindManager ControlBindManager
        {
            get { return mPropertyBindManager; }
        }

        bool mClickToTop = false;
        [Category("杂项"), DisplayName("点击置顶")]
        [Description("点击后本窗口显示到顶层")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public bool ClickToTop
        {
            get { return mClickToTop; }
            set
            {
                mClickToTop = value;
                OnPropertyChanged("ClickToTop");
            }
        }

        public WinForm()
        {
            mUITriggerManager = new Trigger.UITriggerManager(this);
            mPropertyBindManager = new Bind.PropertyBindManager(this);
            mWinState = new WinState(this);
		    //BackColor = CSUtility.Support.Color.FromArgb(128,128,128);
            EditDeleteAble = false;
            ContainerType = enContainerType.Multi;

        }
		public void Dispose()
        {
            if (WinFormClosed!=null)
                WinFormClosed();
        }

        //protected override void RegisterDefaultValue()
        //{
        //    base.RegisterDefaultValue();
        //    mDefaultValueTemplate.RegisterDefaultValue("FixSizeByUVAnim", false);
        //}

		public void LoadForm()
        {
            OnInitiliazed();
            if (WinFormLoaded != null)
                WinFormLoaded();
        }



		public virtual void OnInitiliazed()
        {
            
        }


        bool bShowAsDialog = false;
        [DisplayName("模态对话框")]
        public bool ShowAsDialog
        {
            get { return bShowAsDialog; }
        }
        public void Show()
        {
            bShowAsDialog = false;
            Visibility = Visibility.Visible;
        }

		public void ShowDialog()
        {
            if (!bShowAsDialog)
            {
                var root = this.GetRoot(typeof(WinRoot)) as WinRoot;
                if (root != null)
                {
                    bShowAsDialog = true;

                    UISystem.Device.Mouse.Instance.Focus(this);
                    UISystem.Device.Keyboard.Instance.Focus(this);

                    root.DialogForms.Push(this);
                }
            }

            //Visible = true;
            Visibility = Visibility.Visible;
        }

        public void Close(int index)
        {
            if (bShowAsDialog)
            {
                UISystem.Device.Mouse.Instance.UnFocus(index);
                UISystem.Device.Keyboard.Instance.UnFocus();

                var root = this.GetRoot(typeof(WinRoot)) as WinRoot;
                if (root != null)
                {
                    root.DialogForms.Pop();
                    bShowAsDialog = false;
                }

            }

            Visibility = Visibility.Collapsed;
        }

        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    if (msg.message == (UInt32)MidLayer.SysMessage.VWM_USER + (UInt32)MsgDefineUI.OnSetUVAnim)
        //    {
        //        if(mWinState!=null&&mWinState.UVAnim!=null)
        //        {
        //            MidLayer.ITexture texture = mWinState.UVAnim.TextureObject;
        //            if (texture != null)
        //            {
        //                if (mFixSizeByUVAnim)
        //                {
        //                    this.Width = (int)((float)texture.Width * mWinState.UVAnim.GetUVFrame(0.0F).SizeU);
        //                    this.Height = (int)((float)texture.Height * mWinState.UVAnim.GetUVFrame(0.0F).SizeV);
        //                }
        //                return MSG_PROC.Finished;
        //            }
        //        }
        //    }
        //    return base.OnMsg(ref msg);
        //}

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Down, WinBase_OnKeyboardCharDown, enRoutingStrategy.Tunnel);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_KB_Char_Up, WinBase_OnKeyboardCharUp, enRoutingStrategy.Tunnel);

            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_UVAnimSetted, WinForm_OnUVAnimSetted, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, WinForm_MouseLeftButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, WinBase_OnMouseLeftButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, WinBase_OnMouseRightButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, WinBase_OnMouseMove, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinForm_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);

        }

        protected void WinForm_OnPreWinSizeChanged(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            UpdateLayout();
        }

        private void WinForm_OnUVAnimSetted(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mWinState != null && mWinState.UVAnim != null)
            {
                CCore.Graphics.Texture texture = mWinState.UVAnim.TextureObject;
                if (texture != null)
                {
                    if (mFixSizeByUVAnim)
                    {
                        this.Width = (int)((float)texture.Width * mWinState.UVAnim.GetUVFrame(0.0F).SizeU);
                        this.Height = (int)((float)texture.Height * mWinState.UVAnim.GetUVFrame(0.0F).SizeV);
                    }
                }
            }
        }

        private void WinForm_MouseLeftButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;

            if (AbsRect.Contains(mk.X, mk.Y))
            {
                bool bSenderDragEnable = false;
                var winSender = init.Sender as WinBase;
                if (winSender != null)
                    bSenderDragEnable = winSender.DragEnable;

                if (mDragEnable || bSenderDragEnable)
                {
                    var mb = init as CCore.MsgProc.Behavior.Mouse_Key;

                    mDraging = true;
                    mDragLocation = AbsToLocal(mb.X, mb.Y);

                    UISystem.Device.Mouse.Instance.Capture(this,init.GetBehaviorType());
                }
            }

            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);
            _FWinMouseButtonDown(this, arg);
            _FWinLeftMouseButtonDown(this, arg);

            //UISystem.Device.Keyboard.Instance.UnFocus();

            // 窗口叠层顺序调整
            if (ClickToTop)
            {
                var pt = this.Parent;
                this.Parent = null;
                this.Parent = pt;
                _FFormToTop(this);
            }

            eventArgs.Handled = arg.Handled;
        }

        bool mFixSizeByUVAnim = false;
        [Category("外观"), DisplayName("根据图元设置大小")]
        public bool FixSizeByUVAnim
        {
            get { return mFixSizeByUVAnim; }
            set { mFixSizeByUVAnim = value; }
        }

        string mInstanceClassName="";
        [Browsable(false)]
        public string InstanceClassName
        {
            get { return mInstanceClassName; }
            set { mInstanceClassName = value; }
        }

        [Category("外观")]
        [Browsable(false)]
        public WinState State
        {
            get { return mWinState; }
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

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);
            if (mInstanceClassName != "")
            {
                //pXml._SetValue(mInstanceClassName);
            }

            if(!mDefaultValueTemplate.IsEqualDefaultValue(this, "FixSizeByUVAnim"))
                pXml.AddAttrib("FixSizeByUVAnim", FixSizeByUVAnim.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ClickToTop"))
                pXml.AddAttrib("ClickToTop", ClickToTop.ToString());

            if (mWinState != null)
            {
                CSUtility.Support.XmlNode state = pXml.AddNode("WinState", "",holder);
                mWinState.OnSave(state,holder);
            }

            if (mUITriggerManager.IsHaveTrigger)
            {
                var trigger = pXml.AddNode("UITriggers", "",holder);
                mUITriggerManager.OnSave(trigger,holder);
            }
        }
        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlAttrib attr = pXml.FindAttrib("FixSizeByUVAnim");
            if (attr != null)
                FixSizeByUVAnim = System.Convert.ToBoolean(attr.Value);
            attr = pXml.FindAttrib("ClickToTop");
            if (attr != null)
                ClickToTop = System.Convert.ToBoolean(attr.Value);

            CSUtility.Support.XmlNode state = pXml.FindNode("WinState");
            if (state != null)
            {
                if (mWinState == null)
                    mWinState = new WinState(this);
                mWinState.OnLoad(state);
            }
        }

        protected override void BeforeSave(CSUtility.Support.XmlNode pXml)
        {
            base.BeforeSave(pXml);

            mUITriggerManager.SetToDefaultProperty();
        }

        protected override void AfterLoad(CSUtility.Support.XmlNode pXml)
        {
            base.AfterLoad(pXml);

            var triggerNode = pXml.FindNode("UITriggers");
            if (triggerNode != null)
            {
                if (mUITriggerManager == null)
                    mUITriggerManager = new Trigger.UITriggerManager(this);
                mUITriggerManager.OnLoad(triggerNode);
            }

            BuildCommandBinding(this);
            mPropertyBindManager.BuildBindings(this);

            mUITriggerManager.BuildTriggers(this);

            UpdateClipRect();

            //UpdateLayout();
        }

        protected override void OnSetParent(WinBase parent)
        {
            UpdateLayout();
        }

        public delegate void FWinFormLoaded();
        public delegate void FWinFormClosed();
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
		public event FWinFormLoaded				WinFormLoaded;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
		public event FWinFormClosed				WinFormClosed;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FFormToTop                 FormToTop;
        public void _FFormToTop(WinForm pWin)
        {
            if (FormToTop != null)
                FormToTop(pWin);
        }
    }
}
