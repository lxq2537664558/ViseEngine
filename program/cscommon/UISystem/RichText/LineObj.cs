using System;
using System.Collections.Generic;

namespace UISystem.RichText
{
    public class LineObj : BaseObj
    {

        List<FragmentObj> mFragmentObjs = new List<FragmentObj>();    // 文档片段，包括文字、图片、链接等FragmentObj
        public List<FragmentObj> FragmentObjs
        {
            get { return mFragmentObjs; }
        }

        int mTextHeight;
        public int TextHeight
        {
            get{return mTextHeight;}
            set
            {
                mTextHeight = value;
            }
        }

        Document mDoc;
        public Document Doc
        {
            get { return mDoc; }
            set
            {
                mDoc = value;
            }
        }

        ~LineObj()
        {
            mFragmentObjs.Clear();
        }

        public void UpdateSize()
        {
            Height = 0;
            Width = 0;

            int i = 0;
            foreach (var pObj in mFragmentObjs)
            {
                if (pObj.Type.CompareTo("text") == 0)
                {
                    TextHeight = Math.Max(TextHeight, pObj.Height);
                }

                MaxBottomLine = Math.Max(MaxBottomLine, pObj.MaxBottomLine);

                // renwind modfied
                Height = Math.Max(Height, pObj.Height);
                //Height = Math.Max(Height, pObj.Height + mDoc.LineHeight);
                Width += pObj.Width;

                i++;
            }
        }

        public void UpdatePen(int iPenX, int iPenY)
	    {           
            PenX = iPenX;
            PenY = iPenY;
            switch (Doc.TextAlignment)
            {
                case UI.TextAlignment.Left:
                    PenX = iPenX;
                    break;
                case UI.TextAlignment.Right:
                    PenX = iPenX + Doc.Width - Width;
                    break;
                case UI.TextAlignment.Center:
                    PenX = iPenX + (Doc.Width - Width) / 2;
                    break;
            }
            int iTempPenX = PenX;
            foreach (var pObj in mFragmentObjs)
            {
                pObj.PenY = PenY;
                pObj.PenX = iTempPenX;
                iTempPenX += pObj.Width;
            }

            // TODO：为所有的IMGOBJ，设置iMaxBottomLine
            //for(std::vector<FragmentObj*>::iterator i = m_FragmentObjs.begin(); i != m_FragmentObjs.end(); ++i)
            //{
            //    FragmentObj *pObj = *i;
            //    if(pObj.Type.compare("img")==0)
            //    {
            //        pObj.MaxBottomLine = MaxBottomLine;
            //    }
            //}
        }

	    public void AddFragmentObj(FragmentObj pObj)
	    {
		    mFragmentObjs.Add(pObj);
		    pObj.Line = this;
	    }

        override public void Draw(UIRenderPipe pipe, int zOrder)
	    {
            foreach(var pObj in mFragmentObjs)
		    {
			    pObj.Draw(pipe, zOrder);
		    }
	    }

        public bool WinMouseClick(int x, int y)
        {
            int iLineBottomY = PenY + MaxBottomLine;
            int iLineTopY = iLineBottomY - Height;
            if (y <= iLineBottomY && y >= iLineTopY)
            {
                foreach (var obj in mFragmentObjs)
                {
                    if (obj.PointWinEnterObj(x, y))
                    {
                        var textObj = obj as UISystem.RichText.HyperLinkObj;
                        if (textObj != null)
                        {
                            if (!textObj.Hyperlink)
                                break;
                            else
                            {
                                textObj.HyperlinkClickEvent();
                                return true;
                            }
                        }
                        var imgObj = obj as UISystem.RichText.ImgObj;
                        if (imgObj != null)
                        {
                            imgObj.ImageClickEvent();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool WinMouseEnter(int x, int y)
        {
            int iLineBottomY = PenY + MaxBottomLine;
            int iLineTopY = iLineBottomY - Height;
            if(y <= iLineBottomY && y >= iLineTopY )
            {
                foreach (var obj in mFragmentObjs)
                {
                    if (obj.PointWinEnterObj(x, y))
                    {
                        var textObj = obj as UISystem.RichText.HyperLinkObj;
                        if (textObj != null)
                        {
                            //var textobj = obj as UISystem.RichText.TextObj;
                            if (!textObj.Hyperlink)
                                break;
                            else
                            {
                                textObj.HyperlinkWinMouseEnterEvent();
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        public bool WinMouseMove(int x, int y)
        {
            int iLineBottomY = PenY + MaxBottomLine;
            int iLineTopY = iLineBottomY - Height;
            if (y <= iLineBottomY && y >= iLineTopY)
            {
                foreach (var obj in mFragmentObjs)
                {
                    var textObj = obj as UISystem.RichText.HyperLinkObj;
                    if (textObj == null || !textObj.Hyperlink)
                        continue;
                    if (obj.PointWinEnterObj(x, y))
                    {
                        textObj.HyperlinkWinMouseEnterEvent();
                    }
                    else if (textObj.IsMouseEnter)
                    {
                        textObj.HyperlinkWinMouseLeaveEvent();
                    }
                }
            }
            return false;
        }

        override public bool PointCheck(int x, int y, ref int outCursorX, ref int outCursorY, ref int outCursorHeight, ref int outCurPos, ref FragmentObj pObj)
        {
            int iLineBottomY = PenY + MaxBottomLine;
            int iLineTopY = iLineBottomY - Height;

            outCursorX = PenX + Width;
            outCursorY = PenY + MaxBottomLine - Height;
            outCursorHeight = Height;
            outCurPos = 0;

            if( y<=iLineBottomY && y>=iLineTopY )
            {
                foreach (var obj in mFragmentObjs)
                {
                    if (obj.PointCheck(x, y, ref outCursorX, ref outCursorY, ref outCursorHeight, ref outCurPos, ref pObj))
                    {
                        //string str = obj.GetType().FullName;
//                         var textObj = obj as UISystem.RichText.HyperLinkObj;
//                         if (textObj != null)
//                         {
//                             //var textobj = obj as UISystem.RichText.TextObj;
//                             if (!textObj.Hyperlink)
//                                 break;
//                             else
//                             {
//                                 if (x != outCursorX && y != outCursorY)
//                                     textObj.HyperlinkClickEvent();
//                                 break;
//                             }
//                         }
//                         else
//                         {
//                             var imgObj = obj as UISystem.RichText.ImgObj;
//                             if (imgObj != null)
//                             {
//                                 if (x != outCursorX && y != outCursorY)
//                                     imgObj.ImageClickEvent();
//                             }
//                         }
                        
                        //switch (str)
                        //{
                        //    case "UISystem.RichText.TextObj":
                        //    case "UISystem.RichText.HyperLinkObj":
                        //        {
                                    
                        //        }
                        //    case "UISystem.RichText.ImgObj":
                        //        break;
                        //    default:
                        //        break;
                        //}
                        //break;
                    }
                }

                return true;
            }

            return false;
	    }
    }
}
