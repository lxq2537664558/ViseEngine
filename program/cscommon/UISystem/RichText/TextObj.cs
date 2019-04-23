using System;
using System.Collections.Generic;
using System.ComponentModel;
using CSUtility.Support;

namespace UISystem.RichText
{
    public class TextObj : FragmentObj
    {
        public TextObj()
        {
            Type = "text";
        }

        override public bool OnLoad(XmlNode pXml)
        {
            // 解析的XML文本示例:
            //<F Type="Text" FontSize="42" Content="显示文本的内容">
            //    <P>
            //        <0 BLColor="ffff0000" BRColor="ff00ff00" TLColor="ffffff00" TRColor="ffff00ff" OutlineType="Line" OutlineThickness="1"/>
            //    </P>
            //</F>

            XmlAttrib attr = null;

            attr = pXml.FindAttrib("FontName");
            if (attr != null)
                FontName = attr.Value;

            attr = pXml.FindAttrib("FontSize");
            if (attr != null)
                FontSize = Convert.ToInt32(attr.Value);

            attr = pXml.FindAttrib("Return");
            if (attr != null)
                mReturn = Convert.ToInt32(attr.Value)>0 ? true : false;

            CSUtility.Support.XmlNode pParamsNode = pXml.FindNode("P");
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

            attr = pXml.FindAttrib("Content");
            if (attr != null)
                Content = attr.Value;

            return true;
        }

        override public bool OnSave(XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            pXml.AddAttrib("Type", Type);
            pXml.AddAttrib("FontName", FontName);
            pXml.AddAttrib("FontSize", FontSize.ToString());
            int iReturn = mReturn ? 1 : 0;
            pXml.AddAttrib("Return", iReturn.ToString());
            CSUtility.Support.XmlNode pParamsNode = pXml.AddNode("P", "P",holder);
            //int iIndex = 0;
            //foreach (var fontParam in FontRenderParams)
            for (int iIndex = 0; iIndex < FontRenderParams.GetParamCount();iIndex++)
            {
                CSUtility.Support.XmlNode pNode = pParamsNode.AddNode(System.Convert.ToString(iIndex), "", holder);
                var fontParam = FontRenderParams.GetParam(iIndex);
                fontParam.OnSave(pNode);
            }

            pXml.AddAttrib("Content", Content);

            return true;
        }

        //public virtual String ToFormatString()
        //{
            // 解析的XML文本示例:
            //<F Type="Text" FontSize="42" Content="显示文本的内容">
            //    <P>
            //        <0 BLColor="ffff0000" BRColor="ff00ff00" TLColor="ffffff00" TRColor="ffff00ff" OutlineType="Line" OutlineThickness="1"/>
            //    </P>
            //</F>
            //"<D><P><0 BLColor=\"-5888\" BRColor=\"1367342848\" TLColor=\"-65490\" TRColor=\"-14614273\" OutlineType=\"Line\" OutlineThickness=\"1\"/></P></F></D>"
            //String ret = String.Format("<F Type=\"{0}\" FontName=\"{1}\" FontSize=\"{2}\" Content=\"{3}\"><P>", 
            //    Type, mFontName, mFontSize, Content);

            //int iIndex = 0;
            //foreach (var fontParam in FontRenderParams)
            //{
            //    CSUtility.Support.IXmlNode pNode = pParamsNode.AddNode(System.Convert.ToString(iIndex), "");
            //    fontParam.OnSave(pNode);
            //    iIndex++;
            //}


        //    return ret;
        //}

		override public void Draw(UIRenderPipe pipe, int zOrder)
        {
            IRender.GetInstance().DrawString(pipe, zOrder, PenX, PenY, Content, mAbsFontName, mFontSize, GetSuitableFontRenderParams());
        }

        override public bool PointCheck(int x, int y, ref int outCursorX, ref int outCursorY, ref int outCursorHeight, ref int outCurPos, ref FragmentObj pObj)
        {
		    int l = PenX;
		    int r = PenX + Width;

            outCursorX = r;
            outCurPos = Content.Length;

		    if( x>=l && x<=r )
		    {
			    int iLocalX = x - PenX;
			    int iLocalY = y;
			    int outX = 0;
			    int outY = 0;
			    int outLocalPos = 0;
                IRender.GetInstance().PointCheck(AbsFontName, Content, FontSize, GetSuitableFontRenderParams(), iLocalX, iLocalY, ref outX, ref outY, ref outLocalPos);

			    outCursorX = PenX + outX;
			    outCurPos = outLocalPos;

			    pObj = this;
			    return true;
		    }
		    return false;
        }

        override public void CalcCursorPos(int pos, ref int outCursorX, ref int outCursorY)
        {
            if (Line == null)
                return;

            outCursorX = PenX;
            outCursorY = Line.PenY + Line.MaxBottomLine - Line.Height;

            if (pos < 0)
			    return;
	
		    int outX = 0;
            IRender.GetInstance().MeasureTextToPos(AbsFontName, Content, FontSize, GetSuitableFontRenderParams(), pos, ref outX);
		    outCursorX = PenX + outX;
        }

        public void SplitTextInWidth(int limitWidth, List<String> oTwolineTexts)
        {
            IRender.GetInstance().SplitTextInWidth(AbsFontName, Content, FontSize, GetSuitableFontRenderParams(), limitWidth, oTwolineTexts);
        }

        public void SplitTextInHalf(int limitWidth, List<String> oTwolineTexts)
        {
            IRender.GetInstance().SplitTextInHalf(AbsFontName, Content, FontSize, GetSuitableFontRenderParams(), limitWidth, oTwolineTexts);
        }

        public TextObj Clone(string newContent)
        {
            TextObj obj = new TextObj();
            //foreach (var property in typeof(TextObj).GetProperties())
            //{
            //    property.SetValue(obj, property.GetValue(this,null), null);
            //}
            obj.FontName = FontName;
            obj.FontSize = FontSize;
            obj.FontRenderParams = new CCore.Font.FontRenderParamList(FontRenderParams);
            obj.Line = Line;
            obj.Doc = Doc;
            obj.Content = newContent;

            return obj;
        }

        string mFontName = "";
        public string FontName
        {
            get { return mFontName; }
            set
            {
                mFontName = value;
                if (value == null)
                    return;
                mFontName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mFontName);

                if (string.IsNullOrEmpty(mFontName))
                    mAbsFontName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(CSUtility.Support.IFileConfig.DefaultFont);
                else
                    mAbsFontName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mFontName);
            }
        }

        string mAbsFontName = "";
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
        public int FontSize
        {
            get { return mFontSize; }
            set
            {
                mFontSize = value;
            }
        }

        bool mReturn = false;
        public bool Return
        {
            get { return mReturn; }
            set
            {
                mReturn = value;
            }
        }

//         bool mHyperlink = false;
//         public bool Hyperlink
//         {
//             get { return mHyperlink; }
//             set
//             {
//                 mHyperlink = value;
//             }
//         }

        public CCore.Font.FontRenderParamList GetSuitableFontRenderParams()
        {
            if (FontRenderParams.GetParamCount() > 0)
                return FontRenderParams;
            else
            {
                if(Doc!=null)
                    return Doc.FontRenderParams;
            }
            return null;
        }

        CCore.Font.FontRenderParamList mFontRenderParams = new CCore.Font.FontRenderParamList();
        public CCore.Font.FontRenderParamList FontRenderParams
        {
            get 
            {
                return mFontRenderParams;
            }
            set
            {
                mFontRenderParams = new CCore.Font.FontRenderParamList(value);
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

        //bool mReturn;					// 是否强制换行

        public override string Content
        {
            get { return base.Content; }
            set
            {
                if (Content == value)
                    return;

                base.Content = value;

                Length = Content.Length;
                CSUtility.Support.Size oSize = new CSUtility.Support.Size();
                int maxTopLine = 0;
                int maxBottomLine = 0;
                int realSize = 0;
                IRender.GetInstance().MeasureStringInLine(mAbsFontName, Content, FontSize, GetSuitableFontRenderParams(), ref oSize, ref maxTopLine, ref maxBottomLine, ref realSize);
                Width = oSize.Width;
                Height = oSize.Height;
                MaxTopLine = maxTopLine;
                MaxBottomLine = maxBottomLine;
                RealSize = realSize;
            }
        }

//         public delegate void FHyperlinkClick(TextObj textobj);
//         public event FHyperlinkClick HyperlinkClick;
//         public void HyperlinkClickEvent()
//         {
//             if (HyperlinkClick != null)
//                 HyperlinkClick(this);
//         }
    }

    public class TextObjFactory : FragmentObjFactory
	{
        override public string GetName() { return "text"; }
        override public FragmentObj CreateFragmentObj(Document parent)
        {
            var f = new TextObj();
            f.Doc = parent;
            if (parent != null)
            {
                f.FontSize = parent.FontSize;
                f.FontName = parent.FontName;
                f.FontRenderParams = parent.FontRenderParams;
            }
            return f; 
        }
	};

}
