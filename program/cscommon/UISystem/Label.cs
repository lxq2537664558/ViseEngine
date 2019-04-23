using System.ComponentModel;

namespace UISystem
{
    enum EHoverState
    {
        MouseMoving,
        Hovering,
    }

    //    [CSUtility.Editor.UIEditor_Control("Label")]
    public class Label : WinControl
    {
        public Label()
        {
            mWinState = new WinState(this);
            //BackColor = CSUtility.Support.Color.FromArgb(200, 200, 200);
        }

        //[Category("外观")]
        //[CSUtility.Editor.UIEditor_BindingPropertyAttribute(
        //    new Type[] {
        //        typeof(System.Byte),
        //        typeof(System.UInt16),
        //        typeof(System.UInt32),
        //        typeof(System.UInt64),
        //        typeof(System.SByte),
        //        typeof(System.Int16),
        //        typeof(System.Int32),
        //        typeof(System.Int64),
        //        typeof(System.Single),
        //        typeof(System.Double)
        //    })]
        //public override string Text
        //{
        //    get { return base.Text; }
        //    set
        //    {
        //        base.Text = value;
        //        RState.Text = value;

        //        OnPropertyChanged("Text");
        //    }
        //}
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public override UI.ContentAlignment TextAlign
        {
            get { return base.TextAlign; }
            set
            {
                base.TextAlign = value;
                RState.TextAlign = value;
            }
        }

        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public WinState State
        {
            get { return mWinState; }
            set
            {
                mWinState.CopyFrom(value);
                OnPropertyChanged("State");
            }
        }

        float mMouseMoveTime = 0;
        CSUtility.Support.Point mHoverPos = new CSUtility.Support.Point();
        CSUtility.Support.Point mCurPos = new CSUtility.Support.Point();
        EHoverState mHoverState = EHoverState.MouseMoving;

        //public delegate void FWinLabelClick(WinBase Sender);
        //[CSUtility.Editor.UIEditor_BindingEventAttribute]
        //public event FWinLabelClick WinLabelClick;
        public delegate void FWinLabelBeginHover(WinBase Sender);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinLabelBeginHover WinLabelBeginHover;
        public delegate void FWinLabelEndHover(WinBase Sender);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinLabelEndHover WinLabelEndHover;

        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch( msg.message )
        //    {
        //    case (UInt32)MidLayer.SysMessage.VWM_LBUTTONUP:
        //        {
        //            if (WinLabelClick!=null)
        //                WinLabelClick(this);
        //        }
        //        break;
        //    case (UInt32)MidLayer.SysMessage.VWM_MOUSEMOVE:
        //        {
        //            mCurPos = msg.pt;
        //            mMouseMoveTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();
        //            if (mHoverState == EHoverState.Hovering)
        //            {
        //                if (Math.Abs(mCurPos.X - mHoverPos.X) > 5 || Math.Abs(mCurPos.Y - mHoverPos.Y) > 5)
        //                {
        //                    if (WinLabelEndHover != null)
        //                        WinLabelEndHover(this);
        //                    mHoverState = EHoverState.MouseMoving;
        //                }
        //            }

        //        }
        //        break;
        //    }
        //    return base.OnMsg(ref msg);
        //}
        public void _WinLabelEndHover()
        {
            if (mHoverState == EHoverState.Hovering)
            {
                if (WinLabelEndHover != null)
                    WinLabelEndHover(this);
            }
        }
        float mHoverStayElapseTime = 1.0F;
        [Category("行为"), DisplayName("鼠标悬停时间"), Description("鼠标开始悬停到鼠标悬停事件触发的间隔时间")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float HoverStayElapseTime
        {
            get { return mHoverStayElapseTime; }
            set { mHoverStayElapseTime = value; }
        }
        protected override void AfterStateDraw(UIRenderPipe pipe, int zOrder)
        {
            if (mMouseMoveTime == 0)
                mMouseMoveTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();

            float nowTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();
            if (mHoverState != EHoverState.Hovering)
            {
                if (nowTime - mMouseMoveTime > mHoverStayElapseTime)
                {
                    WinRoot root = this.GetRoot() as WinRoot;
                    root.PostEventProcessor(this.ProcHoverEvent);

                    mHoverPos = mCurPos;
                    mHoverState = EHoverState.Hovering;
                    //ProcHoverEvent(root);
                }
            }
        }

        public void ProcHoverEvent(WinRoot root)
        {
            if (UISystem.Device.Mouse.Instance.FocusWin != this)
            {
                _WinLabelEndHover();
                return;
            }
            if (WinLabelBeginHover != null)
                WinLabelBeginHover(this);
            //mHoverPos = mCurPos;

            //mHoverState = EHoverState.Hovering;
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml, holder);

            if (State != null)
            {
                CSUtility.Support.XmlNode state = pXml.AddNode("WinState", "", holder);
                State.OnSave(state, holder);
            }
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlNode stateNode = pXml.FindNode("WinState");
            if (stateNode != null)
            {
                if (State == null)
                    mWinState = new WinState(this);
                State.OnLoad(stateNode);
            }
        }
    }
}
