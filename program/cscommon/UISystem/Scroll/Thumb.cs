using System;
using System.ComponentModel;

namespace UISystem
{
    // 滑块
    [CSUtility.Editor.UIEditor_Control("组件.Thumb")]
    public class Thumb : WinBase
    {
        protected WinState mNormalState;
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
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
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
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

        protected WinState mPressState;
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
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

        protected UI.Orientation mOrientation = UI.Orientation.Horizontal;
        [Category("布局"), DisplayName("方向")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public UI.Orientation Orientation
        {
            get { return mOrientation; }
            set
            {
                mOrientation = value;
                OnPropertyChanged("Orientation");
            }
        }

        protected int mMinPos = 0;
        protected int mMaxPos
        {
            get
            {
                if (Parent == null)
                    return 0;

                switch (Orientation)
                {
                    case UI.Orientation.Horizontal:
                        return ((WinBase)Parent).Width - Width;

                    case UI.Orientation.Vertical:
                        return ((WinBase)Parent).Height - Height;
                }

                return 0;
            }
        }

        protected float mSizeRate = 0.5f;

        protected float mPercent = 0;
        [Category("杂项"), DisplayName("进度百分比")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float Percent
        {
            get { return mPercent; }
            set
            {
                if (float.IsNaN(value))
                    return;
                //if (System.Math.Abs(mPercent - value) < Assist.MinFloatValue)
                //    return;

                mPercent = value;

                switch (Orientation)
                {
                    case UI.Orientation.Horizontal:
                        {
                            var margLeft = (mMaxPos - mMinPos) * mPercent + mMinPos;
                            this.Margin = new CSUtility.Support.Thickness(margLeft, 0, 0, 0);
                        }
                        break;

                    case UI.Orientation.Vertical:
                        {
                            var margTop = (mMaxPos - mMinPos) * mPercent + mMinPos;
                            this.Margin = new CSUtility.Support.Thickness(0, margTop, 0, 0);
                        }
                        break;
                }

                if (OnThumbDraged != null)
                    OnThumbDraged(mPercent);

                OnPropertyChanged("Percent");
            }
        }

        //// 滑块最大大小
        //protected int mMaxThumbSize = 50;
        //[Category("杂项")]
        //[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        //public int MaxThumbSize
        //{
        //    get { return mMaxThumbSize; }
        //    set
        //    {
        //        var oldValue = mMaxThumbSize;
        //        mMaxThumbSize = value;
        //        OnPropertyChanged("MaxThumbSize", oldValue, mMaxThumbSize);
        //    }
        //}

        //// 滑块最小大小
        //protected int mMinThumbSize = 10;
        //[Category("杂项")]
        //[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        //public int MinThumbSize
        //{
        //    get { return mMinThumbSize; }
        //    set
        //    {
        //        var oldValue = mMinThumbSize;
        //        mMinThumbSize = value;
        //        OnPropertyChanged("mMinThumbSize", oldValue, mMinThumbSize);
        //    }
        //}

        public Thumb()
        {
            mNormalState = new WinState(this);
            mLightState = new WinState(this);
            mPressState = new WinState(this);
            mWinState = mNormalState;

            mDragEnable = true;

            WinMouseEnter += this.MouseEnter;
            WinMouseLeave += this.MouseLeave;
        }

        protected void MouseEnter(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            //if (mWinState != mPressState)
            mWinState = mLightState;
        }
        protected void MouseLeave(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)
        {
            //if (mWinState != mPressState)
            {
                if (mDraging)
                    mWinState = mLightState;
                else
                    mWinState = mNormalState;
            }
        }

        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch (msg.message)
        //    {
        //        case (UInt32)MidLayer.SysMessage.VWM_MOUSEMOVE:
        //            {
        //                if (Parent != null && mDraging)
        //                {
        //                    CSUtility.Support.Point ptMouse = ((WinBase)Parent).AbsToLocal(ref msg.pt);
        //                    //float percentage = 0;
        //                    switch (Orientation)
        //                    {
        //                        case UI.Orientation.Horizontal:
        //                            ptMouse.X -= mDragLocation.X;
        //                            ptMouse.X = System.Math.Max(System.Math.Min(mMaxPos, ptMouse.X), mMinPos);
        //                            //this.Margin = new CSCommon.Support.Thickness(ptMouse.X, 0, 0, 0);
        //                            Percent = (float)(ptMouse.X - mMinPos) / (mMaxPos - mMinPos);
        //                            break;
        //                        case UI.Orientation.Vertical:
        //                            ptMouse.Y -= mDragLocation.Y;
        //                            ptMouse.Y = System.Math.Max(System.Math.Min(mMaxPos, ptMouse.Y), mMinPos);
        //                            //this.Margin = new CSCommon.Support.Thickness(0, ptMouse.Y, 0, 0);
        //                            Percent = (float)(ptMouse.Y - mMinPos) / (mMaxPos - mMinPos);
        //                            break;
        //                    }
        //                }
        //            }
        //            return MSG_PROC.Finished;
        //    }

        //    return base.OnMsg(ref msg);
        //}

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, Thumbs_OnMouseMove, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, Thumbs_OnMouseLeftButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, Thumbs_OnMouseLeftButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, WinBase_OnMouseRightButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinBase_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);
        }
        
        private void Thumbs_OnMouseMove(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (Parent != null && mDraging)
            {
                CSUtility.Support.Point ptMouse = ((WinBase)Parent).GetLocalMousePoint();// .AbsToLocal(ref msg.pt);
                //float percentage = 0;
                switch (Orientation)
                {
                    case UI.Orientation.Horizontal:
                        ptMouse.X -= mDragLocation.X;
                        ptMouse.X = System.Math.Max(System.Math.Min(mMaxPos, ptMouse.X), mMinPos);
                        //this.Margin = new CSCommon.Support.Thickness(ptMouse.X, 0, 0, 0);
                        Percent = (float)(ptMouse.X - mMinPos) / (mMaxPos - mMinPos);
                        break;
                    case UI.Orientation.Vertical:
                        ptMouse.Y -= mDragLocation.Y;
                        ptMouse.Y = System.Math.Max(System.Math.Min(mMaxPos, ptMouse.Y), mMinPos);
                        //this.Margin = new CSCommon.Support.Thickness(0, ptMouse.Y, 0, 0);
                        Percent = (float)(ptMouse.Y - mMinPos) / (mMaxPos - mMinPos);
                        break;
                }

                eventArgs.Handled = true;
            }
        }

        protected void Thumbs_OnMouseLeftButtonDown(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            WinBase_OnMouseLeftButtonDown(init, eventArgs);
            mWinState = mPressState;
        }

        protected void Thumbs_OnMouseLeftButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            WinBase_OnMouseLeftButtonUp(init, eventArgs);

            var mb = init as CCore.MsgProc.Behavior.Mouse_Key;
            if (this.AbsRect.Contains(mb.X, mb.Y))
                mWinState = mLightState;
            else
                mWinState = mNormalState;
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (NormalState != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("NormalState", "", holder);
                NormalState.OnSave(stateNode, holder);
            }
            if (LightState != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("LightState", "", holder);
                LightState.OnSave(stateNode, holder);
            }
            if (PressState != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("PressState", "", holder);
                PressState.OnSave(stateNode, holder);
            }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Orientation"))
                pXml.AddAttrib("Orientation", Orientation.ToString());
            //if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "MaxThumbSize"))
            //    pXml.AddAttrib("MaxThumbSize", MaxThumbSize.ToString());
            //if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "MinThumbSize"))
            //    pXml.AddAttrib("MinThumbSize", MinThumbSize.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlNode stateNode = pXml.FindNode("NormalState");
            if (stateNode != null)
            {
                if (NormalState == null)
                    NormalState = new WinState(this);
                NormalState.OnLoad(stateNode);
            }
            stateNode = pXml.FindNode("LightState");
            if (stateNode != null)
            {
                if (LightState == null)
                    LightState = new WinState(this);
                LightState.OnLoad(stateNode);
            }
            stateNode = pXml.FindNode("PressState");
            if (stateNode != null)
            {
                if (PressState == null)
                    PressState = new WinState(this);
                PressState.OnLoad(stateNode);
            }

            mWinState = NormalState;

            var attr = pXml.FindAttrib("Orientation");
            if (attr != null)
                Orientation = (UI.Orientation)System.Enum.Parse(typeof(UI.Orientation), attr.Value);
            //attr = pXml.FindAttrib("MaxThumbSize");
            //if (attr != null)
            //    MaxThumbSize = System.Convert.ToInt32(attr.Value);
            //attr = pXml.FindAttrib("MinThumbSize");
            //if (attr != null)
            //    MinThumbSize = System.Convert.ToInt32(attr.Value);
        }

        //protected override void OnSetParent(WinBase parent)
        //{
        //    switch (Orientation)
        //    {
        //        case UI.Orientation.Horizontal:
        //            {
        //                if (Width_Auto)
        //                    Width = (int)(parent.Width * mSizeRate);
        //            }
        //            break;

        //        case UI.Orientation.Vertical:
        //            {
        //                if (Height_Auto)
        //                    Height = (int)(parent.Height * mSizeRate);
        //            }
        //            break;
        //    }
        //}

        protected override SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            var returnDesiredSize = availableSize;

            //switch (Orientation)
            //{
            //    case UI.Orientation.Horizontal:
            //        {
            //            returnDesiredSize.Width = this.Width;
            //            var rateWidth = availableSize.Width * mSizeRate;
            //            returnDesiredSize.Width = System.Math.Min(System.Math.Max(MinWidth, rateWidth), MaxWidth);
            //        }
            //        break;

            //    case UI.Orientation.Vertical:
            //        {
            //            returnDesiredSize.Height = this.Height;
            //            var rateHeight = availableSize.Height * mSizeRate;
            //            returnDesiredSize.Height = System.Math.Min(System.Math.Max(MinHeight, rateHeight), mMaxHeight);
            //        }
            //        break;
            //}

            //return returnDesiredSize;
            //return availableSize;

            if (Parent == null)
                return base.MeasureOverride(availableSize);

            switch (Orientation)
            {
                case UI.Orientation.Horizontal:
                    {
                        if (Width_Auto)
                            returnDesiredSize.Width = ((WinBase)Parent).Width * mSizeRate;
                    }
                    break;

                case UI.Orientation.Vertical:
                    {
                        if (Height_Auto)
                            returnDesiredSize.Height = ((WinBase)Parent).Height * mSizeRate;
                    }
                    break;
            }

            return returnDesiredSize;
        }

        //protected override SlimDX.Size ArrangeOverride(SlimDX.Size finalSize)
        //{
        //    //SlimDX.Size retSize = finalSize;

        //    //switch (Orientation)
        //    //{
        //    //    case UI.Orientation.Horizontal:
        //    //        {
        //    //            retSize.Width = finalSize.Width * mSizeRate;
        //    //        }
        //    //        break;

        //    //    case UI.Orientation.Vertical:
        //    //        {
        //    //            retSize.Height = finalSize.Height * mSizeRate;
        //    //        }
        //    //        break;
        //    //}

        //    ////return base.ArrangeOverride(finalSize);
        //    //return retSize;
        //    return base.ArrangeOverride(finalSize);
        //}

#region Command

        // 设置大小的比例
        [CSUtility.Editor.UIEditor_CommandMethod]
        public void SetSizeRate(float size, float maxSize)
        {
            mSizeRate = size / maxSize;
            if (mSizeRate > 1)
                mSizeRate = 1;
            if (float.IsNaN(mSizeRate))
                mSizeRate = 1;

            if (Parent != null)
            {
                switch (Orientation)
                {
                    case UI.Orientation.Horizontal:
                        {
                            if (Width_Auto)
                                Width = (int)(((WinBase)Parent).Width * mSizeRate);
                        }
                        break;

                    case UI.Orientation.Vertical:
                        {
                            if (Height_Auto)
                                Height = (int)(((WinBase)Parent).Height * mSizeRate);
                        }
                        break;
                }
            }
        }

        [CSUtility.Editor.UIEditor_CommandMethod]
        public void OnSliderValueChanged(float value, float minValue, float maxValue)
        {
            if (float.IsNaN(value) || float.IsNaN(minValue) || float.IsNaN(maxValue))
                return;

            Percent = (value - minValue) / (maxValue - minValue);
        }

        // delta为百分比
        public delegate void Delegate_ThumbDraged(float percentage);
        [CSUtility.Editor.UIEditor_CommandEvent]
        public event Delegate_ThumbDraged OnThumbDraged;

#endregion
    }
}
