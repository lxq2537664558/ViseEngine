using System;
using System.ComponentModel;

namespace UISystem.Container
{
    public class Container : WinBase
    {
        public Container()
        {
            ContainerType = enContainerType.Multi;
            mWinState = new WinState(this);
        }

        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [Browsable(false)]
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

        private bool mDragParent = false;
        [Category("行为"), DisplayName("拖动父")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public bool DragParent
        {
            get { return mDragParent; }
            set { mDragParent = value; }
        }

        public delegate void FWinContainerClick(WinBase Sender);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinContainerClick WinContainerClick;

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, Container_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, Container_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, WinBase_OnMouseRightButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, WinBase_OnMouseMove, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinBase_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);
        }

        protected void Container_OnMouseLeftButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDragParent)
            {
                init.Sender = this;
                Send2ParentWin(init, eventArgs);
                return;
            }
            if (mDragEnable)//&& DockMode==System.Windows.Forms.DockStyle.None )
            {
                mDraging = true;
                mDragLocation = GetLocalMousePoint();

                UISystem.Device.Mouse.Instance.Capture(this,init.GetBehaviorType());
            }

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);
            _FWinMouseButtonDown(this, arg);
            _FWinLeftMouseButtonDown(this, arg);
        }

        protected void Container_OnMouseLeftButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDragParent)
            {
                init.Sender = this;
                Send2ParentWin(init, eventArgs);
            }
            if (mDragEnable)
            {
                mDraging = false;

                UISystem.Device.Mouse.Instance.ReleaseCapture(init.BehaviorId);
            }

            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);
            _FWinMouseButtonUp(this, arg);
            _FWinLeftMouseButtonUp(this, arg);

            if (WinContainerClick != null)
                WinContainerClick(this);
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "DragParent"))
                pXml.AddAttrib("DragParent", DragParent.ToString());

            if (State != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("WinState", "",holder);
                State.OnSave(stateNode,holder);
            }
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlAttrib attr = pXml.FindAttrib("DragParent");
            if (attr != null)
                DragParent = System.Convert.ToBoolean(attr.Value);

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
