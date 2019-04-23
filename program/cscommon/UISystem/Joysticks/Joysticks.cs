using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace UISystem.Joysticks
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("Joysticks")]
    [CSUtility.Editor.UIEditor_Control("常用.Joysticks")]
    public class Joysticks : WinBase
    {
        CSUtility.Support.Point mStartPos = new CSUtility.Support.Point();
        protected CSUtility.Support.Point StartPos
        {
            get
            {
                //mStartPos.X = (Left + (int)(Width * (1 - mRockerRatio) / 2));
                //mStartPos.Y = (Top + (int)(Height * (1 - mRockerRatio) / 2));
                return mStartPos;
            }
        }
        protected CSUtility.Support.Point mCurPos;

        public int OffsetX = 0;
        public int OffsetY = 0;

        int mRockerWidth = 0;
        int mRockerHeight = 0;

        SlimDX.Vector3 mRockerDir = SlimDX.Vector3.Zero;

        int mPointerIndex = int.MaxValue;

        protected int mMinPosX
        {
            get
            {
                return mStartPos.X - mRockerWidth / 2;
            }
        }
        protected int mMaxPosX
        {
            get
            {
                return mStartPos.X + mRockerWidth / 2;
            }
        }

        protected int mMinPosY
        {
            get
            {
                return mStartPos.Y - mRockerHeight / 2;
            }
        }
        protected int mMaxPosY
        {
            get
            {
                return mStartPos.Y + mRockerHeight / 2;
            }
        }

        //摇杆大小
        protected float mRockerRatio = 0.5f;
        [Category("外观"), DisplayName("摇杆比例"), Description("摇杆与背景的比例")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public float RockerRatio
        {
            get { return mRockerRatio; }
            set
            {
                mRockerRatio = value;

                if (mRockerRatio > 1)
                    mRockerRatio = 1.0f;

                UpdateRocker();

                OnPropertyChanged("RockerRatio");
            }
        }

        protected WinState mRockerNormalState;
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [Browsable(false)]
        public WinState RockerNormalState
        {
            get { return mRockerNormalState; }
            set
            {
                mRockerNormalState.CopyFrom(value);
                OnPropertyChanged("RockerNormalState");
            }
        }
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("滑块正常图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid RockerNormalStateUVAnimId
        {
            get
            {
                if (RockerNormalState == null)
                    return Guid.Empty;
                return RockerNormalState.UVAnimId;
            }
            set
            {
                if (RockerNormalState != null)
                    RockerNormalState.UVAnimId = value;
                OnPropertyChanged("RockerNormalStateUVAnimId");
            }
        }

        protected WinState mRockerLightState;
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [Browsable(false)]
        public WinState RockerLightState
        {
            get { return mRockerLightState; }
            set
            {
                mRockerLightState.CopyFrom(value);
                OnPropertyChanged("RockerLightState");
            }
        }
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("滑块高亮图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid RockerLightStateUVAnimId
        {
            get
            {
                if (RockerLightState == null)
                    return Guid.Empty;
                return RockerLightState.UVAnimId;
            }
            set
            {
                if (RockerLightState != null)
                    RockerLightState.UVAnimId = value;
                OnPropertyChanged("RockerLightStateUVAnimId");
            }
        }

        protected WinState mRockerPressState;
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [Browsable(false)]
        public WinState RockerPressState
        {
            get { return mRockerPressState; }
            set
            {
                mRockerPressState.CopyFrom(value);
                OnPropertyChanged("RockerPressState");
            }
        }
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("滑块按下图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid RockerPressStateUVAnimId
        {
            get
            {
                if (RockerPressState == null)
                    return Guid.Empty;
                return RockerPressState.UVAnimId;
            }
            set
            {
                if (RockerPressState != null)
                    RockerPressState.UVAnimId = value;
                OnPropertyChanged("RockerPressStateUVAnimId");
            }
        }


        ////摇杆位置
        //protected double mRockerPosition = 0;
        //protected double mRockerMainRate = 0.5;
        //protected double mRockerSubordinationRate = 0.9;

        protected WinState mJoysticksNormalState;
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [Browsable(false)]
        public WinState JoysticksNormalState
        {
            get { return mJoysticksNormalState; }
            set
            {
                mJoysticksNormalState.CopyFrom(value);
                OnPropertyChanged("JoysticksNormalState");
            }
        }
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("背景图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid JoysticksNormalStateUVAnimId
        {
            get
            {
                if (JoysticksNormalState == null)
                    return Guid.Empty;
                return JoysticksNormalState.UVAnimId;
            }
            set
            {
                if (JoysticksNormalState != null)
                    JoysticksNormalState.UVAnimId = value;
                OnPropertyChanged("JoysticksNormalStateUVAnimId");
            }
        }

        protected WinState mRockerWinState;
        [Browsable(false)]
        public WinState RockerWinState
        {
            get { return mRockerWinState; }
            set
            {
                if (value == null)
                    return;
                mRockerWinState = value;
            }
        }

        //protected CSUtility.Support.Rectangle mRockerRect;
        //[Browsable(false)]
        //public CSUtility.Support.Rectangle RockerRect
        //{
        //    get { return mRockerRect; }
        //}

        //protected Rocker mRockerChild = null;

        public Joysticks()
        {
            mRockerNormalState = new WinState(this);
            mRockerLightState = new WinState(this);
            mRockerPressState = new WinState(this);
            mJoysticksNormalState = new WinState(this);
            
            mWinState = mJoysticksNormalState;
            mRockerWinState = mRockerNormalState;

            UpdateRocker();
            
            //mRockerChild = new Rocker(this);
            WinMouseEnter += MouseEnter;
            WinMouseLeave += MouseLeave;
        }
        
        void UpdateRocker()
        {
            mRockerWidth = (int)(Width * mRockerRatio);
            mRockerHeight = (int)(Height * mRockerRatio);
            //mStartPos.X = (AbsRect.X + (int)(Width * (1 - mRockerRatio) / 2));
            //mStartPos.Y = (AbsRect.Y + (int)(Height * (1 - mRockerRatio) / 2));
            mStartPos.X = (int)(Width * 0.5f);
            mStartPos.Y = (int)(Height * 0.5f);
            mCurPos = mStartPos;
            //mRockerRect.Width = (int)(mClipRect.Width * mRockerRatio);
            //mRockerRect.Height = (int)(mClipRect.Height * mRockerRatio);
        }

        bool RockerContains(int x, int y)
        {
            if (x < (mMinPosX + ClipRect.X) || x > (mMaxPosX + ClipRect.X) ||
                y < (mMinPosY + ClipRect.Y) || y > (mMaxPosY + ClipRect.Y))
                return false;
            return true;
        }

        protected void MouseEnter(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            if (RockerContains(pt.X, pt.Y))
            {
                //if (mWinState != mPressState)
                mRockerWinState = RockerLightState;
            }
        }
        protected void MouseLeave(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            //if (mWinState != mPressState)
            {
                if (mDraging)
                    mRockerWinState = RockerLightState;
                else
                    mRockerWinState = RockerNormalState;
            }
        }

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, Joysticks_OnMouseMove, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, Rocker_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, Rocker_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);

            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Down, Rocker_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer2Up, Rocker_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Down, Rocker_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Pointer3Up, Rocker_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);

        }

        private void Joysticks_OnMouseMove(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {            
            if (mDraging)
            {
                CSUtility.Support.Point ptMouse = GetLocalMousePoint();// .AbsToLocal(ref msg.pt);

                var ratio = 1.0f - RockerRatio;
                ptMouse.X = (int)((ptMouse.X - mDragLocation.X) / ratio) + mStartPos.X;
                ptMouse.Y = (int)((ptMouse.Y - mDragLocation.Y) / ratio) + mStartPos.Y;

                //int radius = Math.Min((int)(StartPos.X * ratio) + (int)(0.5 * mRockerWidth), (int)(StartPos.Y * ratio) + (int)(0.5 * mRockerHeight));
                int radius = Math.Min(StartPos.X, StartPos.Y);
                var distance = Math.Sqrt(Math.Pow(Math.Abs(ptMouse.X - StartPos.X), 2) + Math.Pow(Math.Abs(ptMouse.Y - StartPos.Y), 2));
                if (distance > radius)
                {
                    //var ratio = distance / (double)radius;
                    //var x = (int)(Math.Abs(ptMouse.X - StartPos.X) / ratio);
                    //var y = (int)(Math.Abs(ptMouse.Y - StartPos.Y) / ratio);

                    //if (ptMouse.X > StartPos.X)
                    //    ptMouse.X = x + StartPos.X;
                    //else
                    //    ptMouse.X = StartPos.X - x;

                    //if (ptMouse.Y > StartPos.Y)
                    //    ptMouse.Y = y + StartPos.Y;
                    //else
                    //    ptMouse.Y = StartPos.Y - y;

                    var star = new SlimDX.Vector2(StartPos.X, StartPos.Y);
                    var dir = new SlimDX.Vector2(ptMouse.X - StartPos.X, ptMouse.Y - StartPos.Y);
                    dir.Normalize();
                    var pos = star + dir * (float)radius;
                    ptMouse.X = (int)pos.X;
                    ptMouse.Y = (int)pos.Y;
                }

                mCurPos = ptMouse;
                OffsetX = ptMouse.X - StartPos.X;
                OffsetY = ptMouse.Y - StartPos.Y;

                if (OnValueChanged != null)
                    OnValueChanged(GetRockerDir());
                
                eventArgs.Handled = true;
             
            }
            else
            {
                var mb = init as CCore.MsgProc.Behavior.Mouse_Move;
                if (!RockerContains(mb.X, mb.Y))
                {
                    //if (mWinState != mPressState)
                    mRockerWinState = RockerNormalState;
                }
            }
        }

        private void Rocker_OnMouseLeftButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var index = init.BehaviorId;
            if (mPointerIndex != int.MaxValue)
            {
                if (mPointerIndex != index)
                    return;
            }
            mPointerIndex = index;
            var mb = init as CCore.MsgProc.Behavior.Mouse_Key;
            if (RockerContains(mb.X, mb.Y) && !isShowRockerActor)
            {
                mDraging = true;
                mDragLocation.X = mb.X - ClipRect.X;
                mDragLocation.Y = mb.Y - ClipRect.Y;
                //mDragLocation = AbsToLocal(mb.X, mb.Y);
                mDragOffset.X = mDragLocation.X - mLocation.X;
                mDragOffset.Y = mDragLocation.Y - mLocation.Y;

                UISystem.Device.Mouse.Instance.Capture(this,init.GetBehaviorType());

                mRockerWinState = RockerPressState;
            }
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left);
            _FWinMouseButtonDown(this, arg);
            _FWinLeftMouseButtonDown(this, arg);
            eventArgs.Handled = arg.Handled;
        }

        private void Rocker_OnMouseLeftButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var index = init.BehaviorId;
            if (mPointerIndex != int.MaxValue)
            {
                if (mPointerIndex != index)
                    return;
            }
            mPointerIndex = int.MaxValue;
            WinBase_OnMouseLeftButtonUp(init, eventArgs);

            var mb = init as CCore.MsgProc.Behavior.Mouse_Key;
            if (RockerContains(mb.X, mb.Y))
                mRockerWinState = RockerLightState;
            else
                mRockerWinState = RockerNormalState;

            mRockerDir = GetRockerDir();
            OffsetX = 0;
            OffsetY = 0;
            //mCurPos = StartPos;
            isShowRockerActor = true;
            int radius = Math.Min(StartPos.X, StartPos.Y);
            mRockerSpeed = radius / 5.0f;
            UISystem.Device.Mouse.Instance.ReleaseCapture(init.BehaviorId);
            //var distance = Math.Sqrt(Math.Pow(Math.Abs(mCurPos.X - StartPos.X), 2) + Math.Pow(Math.Abs(mCurPos.Y - StartPos.Y), 2));
            //var ratio = distance / radius;
            //mRockerSpeed = (float)ratio * 10.0f;

            if (OnValueChangedLeave != null)
                OnValueChangedLeave();
        }

        private SlimDX.Vector3 GetRockerDir()
        {
            var starVec = new SlimDX.Vector3(StartPos.X, 0, -StartPos.Y);
            var curVec = new SlimDX.Vector3(mCurPos.X, 0, -mCurPos.Y);
            return curVec - starVec;
        }

        public override void DrawUIState(UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
        {
            //if (mVisible)
            if (Visibility == Visibility.Visible && IsVisibleInEditor)
            {
                SlimDX.Matrix matTrans = mTransMatrix;// parentMatrix* mTransMatrix;

                BeforeStateDraw(pipe, zOrder);

                if (mWinState != null)
                {
                    //mWinState.Draw(this, ref mBackColorVertex, ScaleX, ScaleY, ref matTrans, ScaleCenter);
                    mWinState.Draw(pipe, zOrder, this, ref mBackColorVertex, ref matTrans);
                }
                if (mRockerWinState != null)
                {
                    //var transCenter = new SlimDX.Vector3((this.AbsRect.X + this.AbsRect.Width * TransCenterX),// * 2.0f / root.Width,
                    //                 (this.AbsRect.Y + this.AbsRect.Height * TransCenterY),// * 2.0f / root.Height,
                    //                 0);
                    //matTrans = SlimDX.Matrix.Transformation(transCenter, SlimDX.Quaternion.Identity, new SlimDX.Vector3(ScaleX, ScaleY, 1),
                    //         transCenter,
                    //         SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitZ, (float)(Rotation / 180.0f * System.Math.PI)),
                    //         new SlimDX.Vector3(ClipRect.Width * mRockerRatio / 2, ClipRect.Height * RockerRatio / 2, 0));
                    //var scal = new SlimDX.Vector3(mRockerRatio, mRockerRatio, 1);
                    //SlimDX.Matrix.Scaling(ref scal, out matTrans);

                    //var mat = SlimDX.Matrix.Scaling(mRockerRatio, mRockerRatio, 1) * SlimDX.Matrix.Translation(mCurPos.X, mCurPos.Y, 0) * matTrans;
                    //
                    //var offsetX = AbsRect.X + Width * 0.5f;
                    //var offsetY = AbsRect.Y + Height * 0.5f;
                    var offsetX = AbsRect.X + mCurPos.X;
                    var offsetY = AbsRect.Y + mCurPos.Y;
                    var mat = SlimDX.Matrix.Translation(-offsetX, -offsetY, 0) * SlimDX.Matrix.Scaling(mRockerRatio, mRockerRatio, 1) * 
                        SlimDX.Matrix.Translation(offsetX, offsetY, 0) * matTrans;
                   
                    mRockerWinState.Draw(pipe, zOrder + 1, this, ref mBackColorVertex, ref mat);
                }

                AfterStateDraw(pipe, zOrder);
            }
        }

        public override void UpdateClipRect(bool bWithChildren = true)
        {
            base.UpdateClipRect(bWithChildren);

            UpdateRocker();
        }

        bool isShowRockerActor = false;
        float mRockerSpeed = 0.0f;
        public override void Tick(float elapsedMillisecondTime)
        {
            base.Tick(elapsedMillisecondTime);

            if (isShowRockerActor)
            {
                var distance = Math.Sqrt(Math.Pow(Math.Abs(mCurPos.X - StartPos.X), 2) + Math.Pow(Math.Abs(mCurPos.Y - StartPos.Y), 2));
                if (distance <= mRockerSpeed)
                {
                    mCurPos = mStartPos;
                    isShowRockerActor = false;
                    return;
                }
                var star = new SlimDX.Vector2(mStartPos.X, mStartPos.Y);
                var dir = new SlimDX.Vector2(mCurPos.X - StartPos.X, mCurPos.Y - StartPos.Y);
                dir.Normalize();
                var pos = star + dir * (float)(distance - mRockerSpeed);
                mCurPos.X = (int)pos.X;
                mCurPos.Y = (int)pos.Y;
            }
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml, holder);

            if (RockerNormalState != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("RockerNormalState", "", holder);
                RockerNormalState.OnSave(stateNode, holder);
            }
            if (RockerLightState != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("RockerLightState", "", holder);
                RockerLightState.OnSave(stateNode, holder);
            }
            if (RockerPressState != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("RockerPressState", "", holder);
                RockerPressState.OnSave(stateNode, holder);
            }
            if (JoysticksNormalState != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("JoysticksNormalState", "", holder);
                JoysticksNormalState.OnSave(stateNode, holder);
            }
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "RockerRatio"))
                pXml.AddAttrib("RockerRatio", RockerRatio.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            //ClearChildWindows();

            base.OnLoad(pXml);

            CSUtility.Support.XmlNode stateNode = pXml.FindNode("RockerNormalState");
            if (stateNode != null)
            {
                if (RockerNormalState == null)
                    RockerNormalState = new WinState(this);
                RockerNormalState.OnLoad(stateNode);
            }
            stateNode = pXml.FindNode("RockerLightState");
            if (stateNode != null)
            {
                if (RockerLightState == null)
                    RockerLightState = new WinState(this);
                RockerLightState.OnLoad(stateNode);
            }
            stateNode = pXml.FindNode("RockerPressState");
            if (stateNode != null)
            {
                if (RockerPressState == null)
                    RockerPressState = new WinState(this);
                RockerPressState.OnLoad(stateNode);
            }
            stateNode = pXml.FindNode("JoysticksNormalState");
            if (stateNode != null)
            {
                if (JoysticksNormalState == null)
                    JoysticksNormalState = new WinState(this);
                JoysticksNormalState.OnLoad(stateNode);
            }
            var attr = pXml.FindAttrib("RockerRatio");
            if (attr != null)
                RockerRatio = System.Convert.ToSingle(attr.Value);

            mWinState = mJoysticksNormalState;
            mRockerWinState = mRockerNormalState;
            
        }

        #region Command

        public delegate void Delegate_OnValueChanged(SlimDX.Vector3 dir);
        [CSUtility.Editor.UIEditor_CommandEventAttribute]
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event Delegate_OnValueChanged OnValueChanged;

        public delegate void Delegate_OnValueChangedLeave();
        [CSUtility.Editor.UIEditor_CommandEventAttribute]
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event Delegate_OnValueChangedLeave OnValueChangedLeave;

        //public void _FOnValueChanged(SlimDX.Vector3 dir)
        //{
        //    if (OnValueChanged != null)
        //        OnValueChanged(dir);
        //}

        //public void _FOnValueChangedLeave()
        //{
        //    if (OnValueChangedLeave != null)
        //        OnValueChangedLeave();
        //}

        #endregion
    }
}
