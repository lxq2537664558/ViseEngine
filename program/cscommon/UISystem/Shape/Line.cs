using System;
using System.ComponentModel;

namespace UISystem.Shape
{
    [CSUtility.Editor.UIEditor_Control("常用.Line")]
    public class Line : WinBase
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

        float mStartX = 0;
        [Category("外观"), DisplayName("起始点X")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public float StartX
        {
            get { return mStartX; }
            set
            {
                mStartX = value;

                var lineState = mWinState as WinState_Line;
                if (lineState != null)
                    lineState.Start = new SlimDX.Vector2(mStartX, mStartY);

                OnPropertyChanged("StartX");
            }
        }

        float mStartY = 0;
        [Category("外观"), DisplayName("起始点Y")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public float StartY
        {
            get { return mStartY; }
            set
            {
                mStartY = value;

                var lineState = mWinState as WinState_Line;
                if (lineState != null)
                    lineState.Start = new SlimDX.Vector2(mStartX, mStartY);

                OnPropertyChanged("StartY");
            }
        }

        float mEndX = 1;
        [Category("外观"), DisplayName("结束点X")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public float EndX
        {
            get { return mEndX; }
            set
            {
                mEndX = value;

                var lineState = mWinState as WinState_Line;
                if (lineState != null)
                    lineState.End = new SlimDX.Vector2(mEndX, mEndY);

                OnPropertyChanged("EndX");
            }
        }

        float mEndY = 1;
        [Category("外观"), DisplayName("结束点Y")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public float EndY
        {
            get { return mEndY; }
            set
            {
                mEndY = value;

                var lineState = mWinState as WinState_Line;
                if (lineState != null)
                    lineState.End = new SlimDX.Vector2(mEndX, mEndY);

                OnPropertyChanged("EndY");
            }
        }

        int mLineWidth = 1;
        [Category("外观"), DisplayName("线宽度")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public int LineWidth
        {
            get { return mLineWidth; }
            set
            {
                mLineWidth = value;

                var lineState = mWinState as WinState_Line;
                if (lineState != null)
                    lineState.LineWidth = mLineWidth;

                OnPropertyChanged("LineWidth");
            }
        }

        public Line()
        {
            mWinState = new WinState_Line(this);
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml, holder);

            if (State != null)
            {
                var state = pXml.AddNode("WinState", "", holder);
                State.OnSave(state, holder);
            }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "StartX"))
                pXml.AddAttrib("StartX", StartX.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "StartY"))
                pXml.AddAttrib("StartY", StartY.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "EndX"))
                pXml.AddAttrib("EndX", EndX.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "EndY"))
                pXml.AddAttrib("EndY", EndY.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "LineWidth"))
                pXml.AddAttrib("LineWidth", LineWidth.ToString());
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

            var attr = pXml.FindAttrib("StartX");
            if (attr != null)
                StartX = System.Convert.ToSingle(attr.Value);
            attr = pXml.FindAttrib("StartY");
            if (attr != null)
                StartY = System.Convert.ToSingle(attr.Value);
            attr = pXml.FindAttrib("EndX");
            if (attr != null)
                EndX = System.Convert.ToSingle(attr.Value);
            attr = pXml.FindAttrib("EndY");
            if (attr != null)
                EndY = System.Convert.ToSingle(attr.Value);
            attr = pXml.FindAttrib("LineWidth");
            if (attr != null)
                LineWidth = System.Convert.ToInt32(attr.Value);

        }
    }
}
