using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem
{
    #region 字体参数

    //public enum FontOutlineType
    //{
    //    None = 0,
    //    Line = 1,
    //    Inner = 2,
    //    Outer = 3,
    //};

    //public class FourVertexColor
    //{
    //    public Color BLColor = Color.White;
    //    public Color BRColor = Color.White;
    //    public Color TLColor = Color.White;
    //    public Color TRColor = Color.White;

    //    public FourVertexColor()
    //    {

    //    }

    //    public FourVertexColor(Color bl, Color br, Color tl, Color tr)
    //    {
    //        BLColor = bl;
    //        BRColor = br;
    //        TLColor = tl;
    //        TRColor = tr;
    //    }
    //}

    //public class IFontRenderParams : WFontRenderParams//, INotifyPropertyChanged
    //{
    //#region INotifyPropertyChangedMembers
    //public event PropertyChangedEventHandler PropertyChanged;
    //private void OnPropertyChanged(string propertyName)
    //{
    //    PropertyChangedEventHandler handler = this.PropertyChanged;
    //    if (handler != null)
    //    {
    //        handler(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
    //#endregion

    //public void OnLoad(IXmlNode node)
    //{
    //    IXmlAttrib attr = null;
    //    int iARGB;
    //    attr = node.FindAttrib("BLColor");
    //    if (attr != null)
    //    {
    //        if (int.TryParse(attr.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out iARGB) == true)
    //        {
    //            BLColor = Color.FromArgb(iARGB);
    //        }
    //    }
    //    attr = node.FindAttrib("BRColor");
    //    if (attr != null)
    //    {
    //        if (int.TryParse(attr.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out iARGB) == true)
    //        {
    //            BRColor = Color.FromArgb(iARGB);
    //        }
    //    }
    //    attr = node.FindAttrib("TLColor");
    //    if (attr != null)
    //    {
    //        if (int.TryParse(attr.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out iARGB) == true)
    //        {
    //            TLColor = Color.FromArgb(iARGB);
    //        }
    //    }
    //    attr = node.FindAttrib("TRColor");
    //    if (attr != null)
    //    {
    //        if (int.TryParse(attr.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out iARGB) == true)
    //        {
    //            TRColor = Color.FromArgb(iARGB);
    //        }
    //    }
    //    attr = node.FindAttrib("OutlineType");
    //    if (attr != null)
    //        OutlineType = (UISystem.FontOutlineType)System.Enum.Parse(typeof(UISystem.FontOutlineType), attr.Value);
    //    attr = node.FindAttrib("OutlineThickness");
    //    if (attr != null)
    //        OutlineThickness = Convert.ToSingle(attr.Value);
    //}

    //public void OnSave(IXmlNode node)
    //{
    //    node.AddAttrib("BLColor", BLColor.Name);
    //    node.AddAttrib("BRColor", BRColor.Name);
    //    node.AddAttrib("TLColor", TLColor.Name);
    //    node.AddAttrib("TRColor", TRColor.Name);
    //    node.AddAttrib("OutlineType", OutlineType.ToString());
    //    node.AddAttrib("OutlineThickness", OutlineThickness.ToString());
    //}

    //[DisplayName("描边类型")]
    //public new UISystem.FontOutlineType OutlineType
    //{
    //    get { return (UISystem.FontOutlineType)base.OutlineType; }
    //    set
    //    {
    //        base.OutlineType = (MidLayer.FontOutlineType)value;
    //    }
    //}

    //[DisplayName("描边宽度")]
    //override public float OutlineThickness
    //{
    //    get { return base.OutlineThickness; }
    //    set
    //    {
    //        base.OutlineThickness = value;
    //    }
    //}

    //[Browsable(false)]
    //override public Color BLColor
    //{
    //    get { return base.BLColor; }
    //    set
    //    {
    //        if (base.BLColor == value)
    //            return;

    //        base.BLColor = value;

    //        FourVertexColor = new FourVertexColor(BLColor, BRColor, TLColor, TRColor);

    //        OnPropertyChanged("BLColor");
    //    }
    //}

    //[Browsable(false)]
    //override public Color BRColor
    //{
    //    get { return base.BRColor; }
    //    set
    //    {
    //        if (base.BRColor == value)
    //            return;

    //        base.BRColor = value;

    //        FourVertexColor = new FourVertexColor(BLColor, BRColor, TLColor, TRColor);

    //        OnPropertyChanged("BLColor");
    //    }
    //}

    //[Browsable(false)]
    //override public Color TLColor
    //{
    //    get { return base.TLColor; }
    //    set
    //    {
    //        if (base.TLColor == value)
    //            return;

    //        base.TLColor = value;

    //        FourVertexColor = new FourVertexColor(BLColor, BRColor, TLColor, TRColor);

    //        OnPropertyChanged("BLColor");
    //    }
    //}

    //[Browsable(false)]
    //override public Color TRColor
    //{
    //    get { return base.TRColor; }
    //    set
    //    {
    //        if (base.TRColor == value)
    //            return;

    //        base.TRColor = value;

    //        FourVertexColor = new FourVertexColor(BLColor, BRColor, TLColor, TRColor);

    //        OnPropertyChanged("BLColor");
    //    }
    //}


    //FourVertexColor m_fourVertexColor = new FourVertexColor();
    //[DisplayName("颜色")]
    //public FourVertexColor FourVertexColor
    //{
    //    get { return m_fourVertexColor; }
    //    set
    //    {
    //        m_fourVertexColor = value;

    //        if (value != null)
    //        {
    //            BLColor = value.BLColor;
    //            BRColor = value.BRColor;
    //            TLColor = value.TLColor;
    //            TRColor = value.TRColor;
    //        }

    //        OnPropertyChanged("FourVertexColor");
    //    }
    //}

    //}

    #endregion

    [CSUtility.Editor.UIEditor_Control("常用.TextBlock")]
    public class TextBlock : WinBase
    {
        public TextBlock()
        {
            mWinState = new WinState_Text(this);

        }

        ~TextBlock()
        {
        }

        public override bool IsTransparent
        {
            get
            {
                if(string.IsNullOrEmpty(Text))
                    return true;
                else
                    return false;
            }
        }
        
        string mText = "TextBlock";
        [Category("外观"), DisplayName("文本")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute(
            new Type[] {
                typeof(System.Byte),
                typeof(System.UInt16),
                typeof(System.UInt32),
                typeof(System.UInt64),
                typeof(System.SByte),
                typeof(System.Int16),
                typeof(System.Int32),
                typeof(System.Int64),
                typeof(System.Single),
                typeof(System.Double)
            })]
        public string Text
        {
            get { return mText; }
			set
            {
                if (mText == value)
                    return;

                mText = value;
                if (string.IsNullOrEmpty(mText))
                {
                    mText = "";
                }

                // 测量文字
                CSUtility.Support.Size oSize = new CSUtility.Support.Size();
                int maxTopLine = 0;
                int maxBottomLine = 0;
                int realSize = 0;
                IRender.GetInstance().MeasureStringInLine(AbsFontName, Text, FontSize, FontRenderParams, ref oSize, ref maxTopLine, ref maxBottomLine, ref realSize);
                TextSize = oSize;
                OneLineHeight = oSize.Height;
                MaxTopLine = maxTopLine;
                MaxBottomLine = maxBottomLine;
                RealSize = realSize;
                lock (mMultilineTexts)
                {
                    MultilineTexts.Clear();
                    MultilineTexts.Add(Text);
                }

                UpdateLayout();

                OnPropertyChanged("Text");
            }
		}

        public override string ToString()
        {
            return Text;
        }

        protected UI.ContentAlignment mTextAlign = UI.ContentAlignment.MiddleCenter;
        [Category("外观"), DisplayName("文字布局")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public UI.ContentAlignment TextAlign
        {
            get { return mTextAlign; }
			set
            {
                mTextAlign = value;
		        RState.TextAlign = value;

                UpdateLayout();
            }
		}

        //[Category("外观")]
        [Browsable(false)]
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
        //CSUtility.Support.Point mHoverPos = new CSUtility.Support.Point();
        CSUtility.Support.Point mCurPos = new CSUtility.Support.Point();
        //EHoverState mHoverState = EHoverState.MouseMoving;
	
        public delegate void FWinTextBlockClick( WinBase Sender );
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
		public event FWinTextBlockClick WinTextBlockClick;
        //public delegate void FWinTextBlockBeginHover(WinBase Sender);
        //[CSUtility.Editor.UIEditor_BindingEventAttribute]
        //public event FWinTextBlockBeginHover WinTextBlockBeginHover;
        //public delegate void FWinTextBlockEndHover(WinBase Sender);
        //[CSUtility.Editor.UIEditor_BindingEventAttribute]
        //public event FWinTextBlockEndHover WinTextBlockEndHover;
	
        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch( msg.message )
        //    {
        //    case (UInt32)MidLayer.SysMessage.VWM_LBUTTONUP:
        //        {
        //            if (WinTextBlockClick!=null)
        //                WinTextBlockClick(this);
        //        }
        //        break;
        //    case (UInt32)MidLayer.SysMessage.VWM_MOUSEMOVE:
        //        {
        //            mCurPos = msg.pt;
        //            mMouseMoveTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();
        //            if (mHoverState == EHoverState.Hovering)
        //            {
        //                if (Math.Abs(mCurPos.X - mHoverPos.X) > 2 || Math.Abs(mCurPos.Y - mHoverPos.Y) > 2)
        //                {
        //                    if (WinTextBlockEndHover != null)
        //                        WinTextBlockEndHover(this);
        //                    mHoverState = EHoverState.MouseMoving;
        //                }
        //            }
                    
        //        }
        //        break;
        //    }
        //    return base.OnMsg(ref msg);
        //}

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, WinBase_OnMouseLeftButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, TextBlock_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, WinBase_OnMouseRightButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, TextBlock_OnMouseMove, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinBase_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);
        }

        private void TextBlock_OnMouseLeftButtonUp(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            var mk = init as CCore.MsgProc.Behavior.Mouse_Key;
            var arg = new UISystem.Message.MouseEventArgs(mk.Clicks, mk.X, mk.Y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right);
            _FWinMouseButtonUp(this, arg);
            _FWinLeftMouseButtonUp(this, arg);

            if (WinTextBlockClick != null)
                WinTextBlockClick(this);
        }

        private void TextBlock_OnMouseMove(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        {
            mCurPos = UISystem.Device.Mouse.Instance.Position;
            mMouseMoveTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();
            //if (mHoverState == EHoverState.Hovering)
            //{
            //    if (Math.Abs(mCurPos.X - mHoverPos.X) > 2 || Math.Abs(mCurPos.Y - mHoverPos.Y) > 2)
            //    {
            //        if (WinTextBlockEndHover != null)
            //            WinTextBlockEndHover(this);
            //        mHoverState = EHoverState.MouseMoving;
            //    }
            //}

            var mm = init as CCore.MsgProc.Behavior.Mouse_Move;
            var arg = new Message.MouseEventArgs(mm.Clicks, mm.X, mm.Y, mm.button);
            _FWinMouseMove(this, arg);
        }

        //public void _WinTextBlockEndHover()
        //{
        //    if (mHoverState == EHoverState.Hovering)
        //    {
        //        if (WinTextBlockEndHover != null)
        //            WinTextBlockEndHover(this);
        //    }
        //}
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
            if(mMouseMoveTime==0)
                mMouseMoveTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();

            //float nowTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();
            //if (mHoverState != EHoverState.Hovering)
            //{
            //    if (nowTime - mMouseMoveTime > mHoverStayElapseTime)
            //    {
            //        if (UISystem.Device.Mouse.Instance.FocusWin != this)
            //        {
            //            _WinTextBlockEndHover();
            //            return;
            //        }
            //        if (WinTextBlockBeginHover != null)
            //            WinTextBlockBeginHover(this);
            //        mHoverPos = mCurPos;

            //        mHoverState = EHoverState.Hovering;
            //    }
            //}
        }

        string mFontName = "";
        [Category("文本属性"),DisplayName("字体")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [CSUtility.Editor.UIEditor_OpenFileEditorAttribute("ttf"), CSUtility.Editor.UIEditor_DefaultFontPathAttribute]
        public string FontName
        {
            get { return mFontName; }
            set
            {
                mFontName = value;

                if(mFontName != null)
                    mFontName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mFontName);

                OnPropertyChanged("FontName");

                UpdateLayout();

                if (string.IsNullOrEmpty(mFontName))
                    mAbsFontName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(CSUtility.Support.IFileConfig.DefaultFont);
                else
                    mAbsFontName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mFontName);
            }
        }

        string mAbsFontName;
        public string AbsFontName
        {
            get
            {
                if (string.IsNullOrEmpty(mAbsFontName))
                    mAbsFontName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(CSUtility.Support.IFileConfig.DefaultFont);
                return mAbsFontName;
            }
        }

        int mFontSize = 20;
        [Category("文本属性"), DisplayName("文字大小")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public int FontSize
        {
            get { return mFontSize; }
            set
            {
                mFontSize = value;
                OnPropertyChanged("FontSize");

                UpdateLayout();
            }
        }

        UI.TextWrapping mTextWrapping = UI.TextWrapping.Wrap;
        [Category("文本属性"), DisplayName("换行模式")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public UI.TextWrapping TextWrapping
        {
            get { return mTextWrapping; }
            set
            {
                mTextWrapping = value;
                OnPropertyChanged("TextWrapping");
                UpdateLayout();
            }
        }

        UI.TextTrimming mTextTrimming = UI.TextTrimming.None;
        [Category("文本属性"), DisplayName("裁剪模式")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public UI.TextTrimming TextTrimming
        {
            get { return mTextTrimming; }
            set
            {
                mTextTrimming = value;
                OnPropertyChanged("TextTrimming");
                UpdateLayout();
            }
        }

        CCore.Font.FontRenderParamList mFontRenderParams = new CCore.Font.FontRenderParamList(true);
        [CSUtility.Editor.UIEditor_FontParamCollectionAttribute]
        [Category("文本属性"), DisplayName("显示效果")]
        public CCore.Font.FontRenderParamList FontRenderParams
        {
            get { return mFontRenderParams; }
            set
            {
                mFontRenderParams = value;
                OnPropertyChanged("FontRenderParams");
                UpdateLayout();
            }
        }

        CSUtility.Support.Size mTextSize = new CSUtility.Support.Size();
        [Browsable(false)]
        public CSUtility.Support.Size TextSize
        {
            get { return mTextSize; }
            set
            {
                mTextSize = value;
            }
        }

        Int32 mOneLineHeight = 0;
        [Browsable(false)]
        public Int32 OneLineHeight
        {
            get { return mOneLineHeight; }
            set
            {
                mOneLineHeight = value;
            }
        }

        Int32 mMaxTopLine = 0;
        [Browsable(false)]
        public Int32 MaxTopLine
        {
            get { return mMaxTopLine; }
            set
            {
                mMaxTopLine = value;
            }
        }

        Int32 mMaxBottomLine = 0;
        [Browsable(false)]
        public Int32 MaxBottomLine
        {
            get { return mMaxBottomLine; }
            set
            {
                mMaxBottomLine = value;
            }
        }

        Int32 mRealSize = 0;
        [Browsable(false)]
        public Int32 RealSize
        {
            get { return mRealSize; }
            set
            {
                mRealSize = value;
            }
        }

        List<string> mMultilineTexts = new List<string>();
        [Browsable(false)]
        public List<string> MultilineTexts
        {
            get { return mMultilineTexts; }
            set
            {
                lock (mMultilineTexts)
                {
                    mMultilineTexts.Clear();
                    mMultilineTexts.AddRange(value);
                }
            }
        }

        protected override SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            SlimDX.Size returnDesiredSize = new SlimDX.Size();

            returnDesiredSize = availableSize;

            var textSize = new CSUtility.Support.Size();
            if (Width_Auto == true)
            {
                IRender.GetInstance().MeasureStringInLine(AbsFontName, Text, FontSize, FontRenderParams, ref textSize);
                if (textSize.Width < availableSize.Width)
                {
                    returnDesiredSize.Width = textSize.Width;
                }
            }

            if (Height_Auto == true)  
            {
                if (TextWrapping == UI.TextWrapping.NoWrap)
                {
                    IRender.GetInstance().MeasureStringInLine(AbsFontName, Text, FontSize, FontRenderParams, ref textSize);
                    returnDesiredSize.Height = textSize.Height;
                }
                else if (TextWrapping == UI.TextWrapping.Wrap)
                {
                    List<string> multilineTexts = new List<string>();
                    int oneLineHeight = 0;
                    IRender.GetInstance().MeasureStringInWidth(AbsFontName, Text, FontSize, FontRenderParams, (int)returnDesiredSize.Width, ref textSize, multilineTexts, ref oneLineHeight);
                    returnDesiredSize.Height = textSize.Height;
                }
            }
            
            return returnDesiredSize;
        }

        protected override SlimDX.Size ArrangeOverride(SlimDX.Size finalSize)
        {
            lock (mMultilineTexts)
            {
                MultilineTexts.Clear();
            }

            CSUtility.Support.Size oSize = new CSUtility.Support.Size();
            if (TextWrapping == UI.TextWrapping.NoWrap)
            {
                IRender.GetInstance().MeasureStringInLine(AbsFontName, Text, FontSize, FontRenderParams, ref oSize);
                OneLineHeight = oSize.Height;
                MultilineTexts.Add(Text);
            }
            else
            {
                int oneLineHeight = 0;
                IRender.GetInstance().MeasureStringInWidth(AbsFontName, Text, FontSize, FontRenderParams, (int)finalSize.Width, ref oSize, MultilineTexts, ref oneLineHeight);
                OneLineHeight = oneLineHeight;
            }
            TextSize = oSize;

            return finalSize;
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (State != null)
            {
                CSUtility.Support.XmlNode state = pXml.AddNode("WinState", "", holder);
                State.OnSave(state,holder);
            }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Text"))
                pXml.AddAttrib("Text", Text);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "FontName"))
                pXml.AddAttrib("FontName", FontName);
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "FontSize"))
                pXml.AddAttrib("FontSize", System.Convert.ToString(FontSize));
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TextWrapping"))
                pXml.AddAttrib("TextWrapping", System.Convert.ToString(TextWrapping));
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TextTrimming"))
                pXml.AddAttrib("TextTrimming", System.Convert.ToString(TextTrimming));
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TextAlign"))
                pXml.AddAttrib("TextAlign", TextAlign.ToString());

            CSUtility.Support.XmlNode pParamsNode = pXml.AddNode("FontRenderParams", "FontRenderParams",holder);
            //int iIndex = 0;
            //foreach(var fontParam in FontRenderParams)
            for (int iIndex = 0; iIndex < FontRenderParams.GetParamCount(); iIndex++)
            {
                CSUtility.Support.XmlNode pNode = pParamsNode.AddNode(System.Convert.ToString(iIndex), "", holder);
                var fontParam = FontRenderParams.GetParam(iIndex);
                fontParam.OnSave(pNode);
            }
        }
        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlAttrib attr = null;

            attr = pXml.FindAttrib("Text");
            if (attr != null)
                Text = attr.Value;

            CSUtility.Support.XmlNode stateNode = pXml.FindNode("WinState");
            if (stateNode != null)
            {
                if (State == null)
                    mWinState = new WinState(this);
                State.OnLoad(stateNode);
            }

            attr = pXml.FindAttrib("FontName");
            if (attr != null)
                FontName = attr.Value;
            attr = pXml.FindAttrib("FontSize");
            if (attr != null)
                FontSize = Convert.ToInt32(attr.Value);
            attr = pXml.FindAttrib("TextWrapping");
            if (attr != null)
                TextWrapping = (UI.TextWrapping)System.Enum.Parse(typeof(UI.TextWrapping), attr.Value);
            attr = pXml.FindAttrib("TextTrimming");
            if (attr != null)
                TextTrimming = (UI.TextTrimming)System.Enum.Parse(typeof(UI.TextTrimming), attr.Value);
            attr = pXml.FindAttrib("TextAlign");
            if (attr != null)
                TextAlign = (UI.ContentAlignment)System.Enum.Parse(typeof(UI.ContentAlignment), attr.Value);

            CSUtility.Support.XmlNode pParamsNode = pXml.FindNode("FontRenderParams");
            if (pParamsNode != null)
            {
                FontRenderParams.Cleanup();
                List<CSUtility.Support.XmlNode> lists = pParamsNode.GetNodes();
                foreach (CSUtility.Support.XmlNode i in lists)
                {
                    var newParam = FontRenderParams.AddParam();
                    newParam.OnLoad(i);
                }
            }
        }
    }
}
