using System;
using System.Collections.Generic;
using CSUtility.Support;

namespace UISystem.RichText
{
    public class HyperLinkObj : TextObj
    {
        public HyperLinkObj()
        {
            Type = "hyperlink";
        }

        bool mHyperlink = false;
        public bool Hyperlink
        {
            get { return mHyperlink; }
            set
            {
                mHyperlink = value;
            }
        }

        override public void Draw(UIRenderPipe pipe, int zOrder)
        {
            base.Draw(pipe, zOrder);

            Point starPos = new Point(PenX,PenY + 4);
            Point endPos = new Point(PenX + Width, PenY + 4);            
            var color = CSUtility.Support.Color.White;
            if (GetSuitableFontRenderParams() != null)
            {
                color = GetSuitableFontRenderParams().GetAllRenderParams()[0].TLColor;
            }
            IRender.GetInstance().DrawLine(pipe, zOrder, starPos, endPos,2,color);
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
                Return = Convert.ToInt32(attr.Value) > 0 ? true : false;

            attr = pXml.FindAttrib("Hyperlink");
            if (attr != null)
                mHyperlink = Convert.ToInt32(attr.Value) > 0 ? true : false;

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
            int iReturn = Return ? 1 : 0;
            pXml.AddAttrib("Return", iReturn.ToString());
            int iHyperlink = mHyperlink ? 1 : 0;
            pXml.AddAttrib("Hyperlink", iHyperlink.ToString());

            CSUtility.Support.XmlNode pParamsNode = pXml.AddNode("P", "P", holder);
            //int iIndex = 0;
            //foreach (var fontParam in FontRenderParams)
            for (int iIndex = 0; iIndex < FontRenderParams.GetParamCount(); iIndex++)
            {
                CSUtility.Support.XmlNode pNode = pParamsNode.AddNode(System.Convert.ToString(iIndex), "", holder);
                var fontParam = FontRenderParams.GetParam(iIndex);
                fontParam.OnSave(pNode);
            }

            pXml.AddAttrib("Content", Content);

            return true;
        }


        // 根据x,y坐标，计算出光标的显示位置，和光标在字符串中的位置
        override public bool PointCheck(int x, int y, ref int outCursorX, ref int outCursorY, ref int outCursorHeight, ref int outCurPos, ref FragmentObj pObj)
        {
            int l = PenX;
            int r = l + Width;

            outCursorX = r;
            outCurPos = 1;

            if (x >= l && x <= r)
            {
                int iRight = x - l;
                if (iRight > Width / 2)
                {
                    outCursorX = r;
                    outCurPos = 1;
                }
                else
                {
                    outCursorX = l;
                    outCurPos = 0;
                }

                pObj = this;
                return true;
            }

            return false;
        }

        // 根据光标在字符中的位置，计算出光标的显示位置
        public override void CalcCursorPos(int pos, ref int outCursorX, ref int outCursorY)
        {
            int iLocalPos = pos;
            if (iLocalPos < 0)
                return;

            if (iLocalPos > 0)
                outCursorX = PenX + Width;
            else
                outCursorX = PenX;
            outCursorY = Line.PenY + Line.MaxBottomLine - Line.Height;
        }

        public delegate void FHyperlinkClick(HyperLinkObj hyperlinkobj);
        public event FHyperlinkClick HyperlinkClick;
        public void HyperlinkClickEvent()
        {
            if (HyperlinkClick != null)
                HyperlinkClick(this);
        }

        public bool IsMouseEnter = false;

        public delegate void FHyperlinkWinMouseEnter(HyperLinkObj hyperlinkobj);
        public event FHyperlinkWinMouseEnter HyperlinkWinMouseEnter;
        public void HyperlinkWinMouseEnterEvent()
        {
            if (HyperlinkWinMouseEnter != null)
                HyperlinkWinMouseEnter(this);
            IsMouseEnter = true;
        }

        public delegate void FHyperlinkWinMouseLeave(HyperLinkObj hyperlinkobj);
        public event FHyperlinkWinMouseLeave HyperlinkWinMouseLeave;
        public void HyperlinkWinMouseLeaveEvent()
        {
            if (HyperlinkWinMouseLeave != null)
                HyperlinkWinMouseLeave(this);
            IsMouseEnter = false;
        }
    }

    public class HyperLinkObjFactory : FragmentObjFactory
    {
        override public string GetName() { return "hyperlink"; }
        override public FragmentObj CreateFragmentObj(Document parent)
        {
            var f = new HyperLinkObj();
            f.Doc = parent;
            if (parent != null)
            {
            }
            return f;
        }
    };
}