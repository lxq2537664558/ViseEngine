namespace UISystem
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class WinState_Text : WinState
    {
        public WinState_Text(WinBase win) : base(win)
        {
            
        }
        public override void Draw(UIRenderPipe pipe, int zOrder, WinBase pWin, ref SlimDX.Vector4 backColor, ref SlimDX.Matrix parentMatrix)
        {
            WinRoot root = pWin.GetRoot() as WinRoot;
            if (root == null)
                return;

            IRender pRender = IRender.GetInstance();
		    pRender.SetClipRect(pipe, pWin.ClipRect);

            if (UVAnim != null)
            {
                UVFrame frame = UVAnim.GetUVFrame(CCore.Engine.Instance.GetFrameSecondTimeFloatByUVAnim());
                CSUtility.Support.Rectangle dst = pWin.AbsRect;
                if (Material != null)
                {
                    if (frame.Scale9DrawRectangles.Count > 0)
                    {
                        foreach (var scaleInfo in frame.Scale9DrawRectangles)
                        {
                            var refRect = scaleInfo.GetDrawRect(dst);
                            pRender.DrawImage(pipe, zOrder, root.Width, root.Height, 
                                              Texture, 
                                              ref backColor,
                                              ref refRect,
                                              ref scaleInfo.mDrawUVRect, ref parentMatrix, pWin.Opacity, Material);
                        }
                    }
                    else
                    {
                        pRender.DrawImage(pipe, zOrder, root.Width, root.Height, Texture, ref backColor,
                        ref dst,
                        ref frame.mUVRect, ref parentMatrix, pWin.Opacity, Material);
                    }
                }
            }
            else
            {
                if (backColor.W != 0)
                {
                    pRender.FillRectangle(pipe, zOrder, root.Width, root.Height, pWin.AbsRect, ref backColor, ref parentMatrix);
                }
            }
            
            var parent = HostWin as TextBlock;
            var Text = parent.Text;

            int lineNum = parent.MultilineTexts.Count;
            int textHeight = parent.TextSize.Height;
            int textWidth = parent.TextSize.Width;
            int ctrlHeight = parent.Height;
            int ctrlWidth = parent.Width;
            
            if (Text!=null && Text != "")
            {
                CSUtility.Support.Point pt = new CSUtility.Support.Point();
                switch (TextAlign)
                {
                    case UI.ContentAlignment.TopLeft:
                        {
                            pt.X = 0;
                            pt.Y = textHeight - parent.MaxBottomLine;
                        }
                        break;
                    case UI.ContentAlignment.TopRight:
                        {
                            pt.X = ctrlWidth - textWidth;
                            pt.Y = textHeight - parent.MaxBottomLine;
                        }
                        break;
                    case UI.ContentAlignment.TopCenter:
                        {
                            pt.X = ctrlWidth / 2 - textWidth / 2;
                            pt.Y = textHeight - parent.MaxBottomLine;
                        }
                        break;
                    case UI.ContentAlignment.BottomLeft:
                        {
                            pt.X = 0;
                            pt.Y = ctrlHeight - parent.MaxBottomLine;
                        }
                        break;
                    case UI.ContentAlignment.BottomRight:
                        {
                            pt.X = ctrlWidth - textWidth;
                            pt.Y = ctrlHeight - parent.MaxBottomLine;
                        }
                        break;
                    case UI.ContentAlignment.BottomCenter:
                        {
                            pt.X = ctrlWidth / 2 - textWidth / 2;
                            pt.Y = ctrlHeight - parent.MaxBottomLine;
                        }
                        break;
                    case UI.ContentAlignment.MiddleLeft:
                        {
                            pt.X = 0;
                            pt.Y = ctrlHeight / 2 - (lineNum - 1) * parent.OneLineHeight + (textHeight / 2 - parent.MaxBottomLine);
                        }
                        break;
                    case UI.ContentAlignment.MiddleRight:
                        {
                            pt.X = ctrlWidth - textWidth;
                            pt.Y = ctrlHeight / 2 - (lineNum - 1) * parent.OneLineHeight + (textHeight / 2 - parent.MaxBottomLine);
                        }
                        break;
                    case UI.ContentAlignment.MiddleCenter:
                        {
                            pt.X = ctrlWidth / 2 - textWidth / 2;
                            pt.Y = ctrlHeight / 2 - (lineNum - 1) * parent.OneLineHeight + (textHeight / 2 - parent.MaxBottomLine);
                        }
                        break;
                    default:
                        pt.X = 0;
                        pt.Y = 0;
                        break;
                }
                CSUtility.Support.Point ptAbs = pWin.LocalToAbs(ref pt);

                if(parent!=null)
                {
                    foreach(var fontParam in parent.FontRenderParams.GetAllRenderParams())
                    {
                        fontParam.Opacity = pWin.Opacity;
                    }
                }

                if (parent.MultilineTexts.Count == 0)
                {
                    pRender.DrawString(pipe, zOrder + 1, ptAbs.X + TextOffset.X, ptAbs.Y + TextOffset.Y, Text, parent.AbsFontName, parent.FontSize, parent.FontRenderParams);
                }
                else
                {
                    int iLineIndex = 0;
                    //foreach (string text in parent.MultilineTexts)
                    for (int i = 0; i < parent.MultilineTexts.Count; ++i)
                    {
                        string tempText="";
                        lock (parent.MultilineTexts)
                        {
                            if (i < parent.MultilineTexts.Count)
                                tempText = parent.MultilineTexts[i];
                        }
                        int x = ptAbs.X + TextOffset.X;
                        int y = ptAbs.Y + TextOffset.Y + parent.OneLineHeight * iLineIndex;

                        if (parent != null)
                            pRender.DrawString(pipe, zOrder + 1, x, y, tempText, parent.AbsFontName, parent.FontSize, parent.FontRenderParams);

                        iLineIndex++;
                    }
                }
            }
        }
    }
}
