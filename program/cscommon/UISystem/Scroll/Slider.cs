using System;
using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("Slider")]
    [CSUtility.Editor.UIEditor_Control("常用.Slider")]
    public class Slider : WinBase
    {
        //protected UI.Orientation mOrientation = UI.Orientation.Horizontal;
        //[Category("杂项")]
        //[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        //public UI.Orientation Orientation
        //{
        //    get { return mOrientation; }
        //    set
        //    {
        //        var oldValue = mOrientation;
        //        mOrientation = value;

        //        //switch (mOrientation)
        //        //{
        //        //    case UI.Orientation.Horizontal:
        //        //        {
        //        //            mThumb.HorizontalAlignment = UI.HorizontalAlignment.Left;
        //        //            mThumb.VerticalAlignment = UI.VerticalAlignment.Center;
        //        //        }
        //        //        break;

        //        //    case UI.Orientation.Vertical:
        //        //        {
        //        //            mThumb.VerticalAlignment = UI.VerticalAlignment.Top;
        //        //            mThumb.HorizontalAlignment = UI.HorizontalAlignment.Center;
        //        //        }
        //        //        break;
        //        //}

        //        OnPropertyChanged("Orientation", oldValue, mOrientation);
        //    }
        //}

        protected float mMaximum = 100;
        [Category("杂项"), DisplayName("最大值")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float Maximun
        {
            get{ return mMaximum; }
            set
            {
                mMaximum = value;
                OnPropertyChanged("Maximun");
            }
        }

        protected float mMinimum = 0;
        [Category("杂项"), DisplayName("最小值")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float Minimum
        {
            get{ return mMinimum; }
            set
            {
                mMinimum = value;
                OnPropertyChanged("Minimum");
            }
        }

        protected float mLargeChange = 1;
        [Category("杂项"), DisplayName("最大改变量")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float LargeChange
        {
            get { return mLargeChange; }
            set
            {
                mLargeChange = value;
                OnPropertyChanged("LargeChange");
            }
        }

        protected float mSmallChange = 0.1f;
        [Category("杂项"), DisplayName("最小改变量")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float SmallChange
        {
            get { return mSmallChange; }
            set
            {
                mSmallChange = value;
                OnPropertyChanged("SmallChange");
            }
        }

        // 滑块大小
        //protected double mThumbSize = 10;
        // 滑块位置
        //protected double mThumbPosition = 0;
        //protected double mThumbMainRate = 0.5;
        //protected double mThumbSubordinationRate = 0.9;

        protected float mValue = 0;
        [Category("杂项"), DisplayName("当前值")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute(new Type[] { typeof(Int32) })]
        public float Value
        {
            get{ return mValue; }
            set
            {
                if (System.Math.Abs(mValue - value) < Assist.MinFloatValue)
                    return;

                mValue = value;

                if (OnValueChanged != null)
                    OnValueChanged(mValue, Minimum, Maximun);

                OnPropertyChanged("Value");
            }
        }

        //protected WinState mTrackState;
        //[Category("外观")]
        //public WinState TrackState
        //{
        //    get { return mThumb.State; }
        //    set
        //    {
        //        mThumb.State = value;
        //        //var oldValue = mTrackState;
        //        //mTrackState.CopyFrom(value);
        //        //OnPropertyChanged("TrackState", oldValue, mTrackState);
        //    }
        //}
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

        //protected Thumb mThumb = new Thumb();
        //protected WinState mThumbState;
        //[Category("外观")]
        //public WinState ThumbState
        //{
        //    get { return mThumbState; }
        //    set
        //    {
        //        var oldValue = mThumbState;
        //        mThumbState.CopyFrom(value);
        //        OnPropertyChanged("ThumbState", oldValue, mThumbState);
        //    }
        //}

        public Slider()
        {
            mWinState = new WinState(this);
            //mThumbState = new WinState(this);
            //mThumb.Parent = this;
            //mThumb.WinName = "Thumb";
            //mThumb.HorizontalAlignment = UI.HorizontalAlignment.Left;
            //mThumb.VerticalAlignment = UI.VerticalAlignment.Center;
            //mThumb.DragEnable = true;
            //mTrackState = mThumb.State;
            //mThumb.WinDraging += mImageThumb_WinDraging;
            
            //WinSizeChanged += Slider_WinSizeChanged;
        }

        //void mImageThumb_WinDraging(ref CSUtility.Support.Point pt, WinBase Sender)
        //{
        //    switch (Orientation)
        //    {
        //        case UI.Orientation.Horizontal:
        //            {
        //                var left = mThumb.Margin.Left + pt.X;
        //                left = System.Math.Min(this.Width - mThumb.Width, left);
        //                if (left < 0)
        //                    left = 0;
        //                mThumb.Margin = new CSCommon.Support.Thickness(left, 0, 0, 0);
        //            }
        //            break;

        //        case UI.Orientation.Vertical:
        //            {
        //                var top = mThumb.Margin.Top + pt.Y;
        //                top = System.Math.Min(this.Height - mThumb.Height, top);
        //                if (top < 0)
        //                    top = 0;
        //                mThumb.Margin = new CSCommon.Support.Thickness(0, top, 0, 0);
        //            }
        //            break;
        //    }
        //}

        //void Slider_WinSizeChanged(int w, int h, WinBase Sender)
        //{
        //    //switch (Orientation)
        //    //{
        //    //    case UI.Orientation.Horizontal:
        //    //        {
        //    //            var width = (int)(w * mThumbMainRate);
        //    //            mThumb.Width = System.Math.Max(System.Math.Min(mThumb.MaxThumbSize, width), mThumb.MinThumbSize);
        //    //            mThumb.Height = (int)(h * mThumbSubordinationRate);
        //    //            var left = (Value - Minimum) / (Maximun - Minimum) * (w - mThumb.Width);
        //    //            mThumb.Margin = new CSCommon.Support.Thickness(left, 0, 0, 0);
        //    //        }
        //    //        break;

        //    //    case UI.Orientation.Vertical:
        //    //        {
        //    //            var height = (int)(h * mThumbMainRate);
        //    //            mThumb.Height = System.Math.Max(System.Math.Min(mThumb.MaxThumbSize, height), mThumb.MinThumbSize);
        //    //            mThumb.Width = (int)(w * mThumbSubordinationRate);
        //    //            var top = (Value - Minimum) / (Maximun - Minimum) * (h - mThumb.Height);
        //    //            mThumb.Margin = new CSCommon.Support.Thickness(0, top, 0, 0);
        //    //        }
        //    //        break;
        //    //}
        //}

        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch (msg.message)
        //    {
        //        case (UInt32)MidLayer.SysMessage.VWM_LBUTTONDOWN:
        //            {
        //                WinRoot.GetInstance().CaptureMouse(this);
        //            }
        //            return MSG_PROC.Finished;

        //        case (UInt32)MidLayer.SysMessage.VWM_LBUTTONUP:
        //            {

        //            }
        //            return MSG_PROC.Finished;
        //    }

        //    return base.OnMsg(ref msg);
        //}

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (State != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("WinState", "",holder);
                State.OnSave(stateNode,holder);
            }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Maximun"))
                pXml.AddAttrib("Maximun", Maximun.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Minimum"))
                pXml.AddAttrib("Minimum", Minimum.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "LargeChange"))
                pXml.AddAttrib("LargeChange", LargeChange.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "SmallChange"))
                pXml.AddAttrib("SmallChange", SmallChange.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Value"))
                pXml.AddAttrib("Value", Value.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            //ClearChildWindows();

            base.OnLoad(pXml);

            CSUtility.Support.XmlNode stateNode = pXml.FindNode("WinState");
            if (stateNode != null)
            {
                if (State == null)
                    mWinState = new WinState(this);
                State.OnLoad(stateNode);
            }

            var attr = pXml.FindAttrib("Maximun");
            if (attr != null)
                Maximun = System.Convert.ToSingle(attr.Value);
            attr = pXml.FindAttrib("Minimum");
            if (attr != null)
                Minimum = System.Convert.ToSingle(attr.Value);
            attr = pXml.FindAttrib("LargeChange");
            if (attr != null)
                LargeChange = System.Convert.ToSingle(attr.Value);
            attr = pXml.FindAttrib("SmallChange");
            if (attr != null)
                SmallChange = System.Convert.ToSingle(attr.Value);
            attr = pXml.FindAttrib("Value");
            if (attr != null)
                Value = System.Convert.ToSingle(attr.Value);
        }

#region Command

        [CSUtility.Editor.UIEditor_CommandMethod]
        public void OnThumbDraged(float percentage)
        {
            Value = (Maximun - Minimum) * percentage + Minimum;
        }

        public delegate void Delegate_OnValueChanged(float value, float minValue, float maxValue);
        [CSUtility.Editor.UIEditor_CommandEvent]
        public event Delegate_OnValueChanged OnValueChanged;

#endregion
    }
}
