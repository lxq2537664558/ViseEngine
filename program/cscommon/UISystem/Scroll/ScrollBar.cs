using System;
using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("ScrollBar")]
    [CSUtility.Editor.UIEditor_Control("组件.ScrollBar")]
    public class ScrollBar : WinBase
    {
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

        protected float mMaximum = 100;
        [Category("公共属性"), DisplayName("最大值")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float Maximum
        {
            get { return mMaximum; }
            set
            {
                mMaximum = value;

                if (OnViewportSizeChangedCommand != null)
                    OnViewportSizeChangedCommand(ViewportSize, mMaximum);

                OnPropertyChanged("Maximum");
            }
        }

        protected float mMinimum = 0;
        [Category("公共属性"), DisplayName("最小值")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float Minimum
        {
            get { return mMinimum; }
            set
            {
                mMinimum = value;
                OnPropertyChanged("Minimum");
            }
        }

        protected float mLargeChange = 1;
        [Category("公共属性"), DisplayName("最大改变量")]
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
        [Category("公共属性"), DisplayName("最小改变量")]
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

        protected float mValue = 0;
        [Category("公共属性"), DisplayName("当前值")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float Value
        {
            get { return mValue; }
            set
            {
                if (System.Math.Abs(mValue - value) < Assist.MinFloatValue)
                    return;

                mValue = value;

                if (OnValueChangedCommand != null)
                    OnValueChangedCommand(mValue, Minimum, Maximum);

                OnPropertyChanged("Value");
            }
        }

        protected float mViewportSize = 0;
        [Category("公共属性"), DisplayName("视口大小")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public float ViewportSize
        {
            get { return mViewportSize; }
            set
            {
                if (System.Math.Abs(mViewportSize - value) < Assist.MinFloatValue)
                    return;

                mViewportSize = value;

                if (OnViewportSizeChangedCommand != null)
                    OnViewportSizeChangedCommand(mViewportSize, Maximum);

                OnPropertyChanged("ViewportSize");
            }
        }

        public ScrollBar()
        {
            ContainerType = enContainerType.None;
            mWinState = new WinState(this);
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (State != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("WinState", "",holder);
                State.OnSave(stateNode,holder);
            }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Maximum"))
                pXml.AddAttrib("Maximum", Maximum.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Minimum"))
                pXml.AddAttrib("Minimum", Minimum.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "LargeChange"))
                pXml.AddAttrib("LargeChange", LargeChange.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "SmallChange"))
                pXml.AddAttrib("SmallChange", SmallChange.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Value"))
                pXml.AddAttrib("Value", Value.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ViewportSize"))
                pXml.AddAttrib("ViewportSize", ViewportSize.ToString());
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

            var attr = pXml.FindAttrib("Maximum");
            if (attr != null)
                Maximum = System.Convert.ToSingle(attr.Value);
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
            attr = pXml.FindAttrib("ViewportSize");
            if (attr != null)
                ViewportSize = System.Convert.ToSingle(attr.Value);
        }

#region Command

        public delegate void Delegate_OnValueChangedCommand(float value, float minValue, float maxValue);
        [CSUtility.Editor.UIEditor_CommandEvent]
        public event Delegate_OnValueChangedCommand OnValueChangedCommand;

        public delegate void Delegate_OnViewportSizeChangedCommand(float viewPortSize, float maxValue);
        [CSUtility.Editor.UIEditor_CommandEvent]
        public event Delegate_OnViewportSizeChangedCommand OnViewportSizeChangedCommand;

        [CSUtility.Editor.UIEditor_CommandMethod]
        public void ValueChangedCommand(float value, float minValue, float maxValue)
        {
            var percent = (value - minValue) / (maxValue - minValue);
            Value = (Maximum - Minimum) * percent + Minimum;
        }
        
        [CSUtility.Editor.UIEditor_CommandMethod]
        public void LineUpCommand(WinBase sender, CCore.MsgProc.BehaviorParameter behavior)
        {
            var tagValue = Value - SmallChange;
            tagValue = System.Math.Max(tagValue, Minimum);
            tagValue = System.Math.Min(tagValue, Maximum);

            Value = tagValue;
        }

        [CSUtility.Editor.UIEditor_CommandMethod]
        public void LineDownCommand(WinBase sender, CCore.MsgProc.BehaviorParameter behavior)
        {
            var tagValue = Value + SmallChange;
            tagValue = System.Math.Max(tagValue, Minimum);
            tagValue = System.Math.Min(tagValue, Maximum);

            Value = tagValue;
        }

#endregion
    }
}
