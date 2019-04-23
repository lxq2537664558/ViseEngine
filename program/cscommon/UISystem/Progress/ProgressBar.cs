using System;
using System.ComponentModel;

namespace UISystem.Progress
{
    [CSUtility.Editor.UIEditor_Control("常用.ProgressBar")]
    public class ProgressBar : WinBase
    {
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [Browsable(false)]
        public WinState_Progress State
        {
            get { return (WinState_Progress)mWinState; }
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

        [Category("公共属性"), DisplayName("进度百分比")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public double Percent
        {
            get
            {
                if (State != null)
                    return State.Percent;
                return 1;
            }
            set
            {
                if (State != null)
                    State.Percent = (float)value;
                OnPropertyChanged("Percent");
            }
        }

        [Category("公共属性"), DisplayName("横向类型")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public UISystem.WinState_Progress.enHorizontalType HorizontalType
        {
            get
            {
                if (State != null)
                    return State.HorizontalType;

                return UISystem.WinState_Progress.enHorizontalType.LeftToRight;
            }
            set
            {
                if (State != null)
                    State.HorizontalType = value;
                OnPropertyChanged("HorizontalType");
            }
        }

        [Category("公共属性"), DisplayName("纵向类型")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public UISystem.WinState_Progress.enVerticalType VerticalType
        {
            get
            {
                if (State != null)
                    return State.VerticalType;

                return UISystem.WinState_Progress.enVerticalType.None;
            }
            set
            {
                if (State != null)
                    State.VerticalType = value;
                OnPropertyChanged("VerticalType");
            }
        }

        [Category("公共属性"), DisplayName("进度类型")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public UISystem.WinState_Progress.enProgressType ProgressType
        {
            get
            {
                if (State != null)
                    return State.ProgressType;

                return UISystem.WinState_Progress.enProgressType.Normal;
            }
            set
            {
                if (State != null)
                    State.ProgressType = value;
                OnPropertyChanged("ProgressType");
            }
        }

        public ProgressBar()
        {
            mWinState = new WinState_Progress(this);
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml, holder);

            if (State != null)
            {
                CSUtility.Support.XmlNode state = pXml.AddNode("WinState", "", holder);
                State.OnSave(state, holder);
            }

            if(!mDefaultValueTemplate.IsEqualDefaultValue(this, "Percent"))
                pXml.AddAttrib("Percent", Percent.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "HorizontalType"))
                pXml.AddAttrib("HorizontalType", HorizontalType.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "VerticalType"))
                pXml.AddAttrib("VerticalType", VerticalType.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ProgressType"))
                pXml.AddAttrib("ProgressType", ProgressType.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlNode stateNode = pXml.FindNode("WinState");
            if (stateNode != null)
            {
                if (State == null)
                    mWinState = new WinState_Progress(this);
                State.OnLoad(stateNode);
            }

            var att = pXml.FindAttrib("Percent");
            if (att != null)
                Percent = System.Convert.ToDouble(att.Value);
            att = pXml.FindAttrib("HorizontalType");
            if (att != null)
                HorizontalType = (UISystem.WinState_Progress.enHorizontalType)System.Enum.Parse(typeof(UISystem.WinState_Progress.enHorizontalType), att.Value);
            att = pXml.FindAttrib("VerticalType");
            if (att != null)
                VerticalType = (UISystem.WinState_Progress.enVerticalType)System.Enum.Parse(typeof(UISystem.WinState_Progress.enVerticalType), att.Value);
            att = pXml.FindAttrib("ProgressType");
            if (att != null)
                ProgressType = (UISystem.WinState_Progress.enProgressType)System.Enum.Parse(typeof(UISystem.WinState_Progress.enProgressType), att.Value);
        }
    }
}
