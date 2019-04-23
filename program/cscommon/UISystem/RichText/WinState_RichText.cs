namespace UISystem
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class WinState_RichText : WinState
    {
        public WinState_RichText(WinBase win) : base(win)
        {
            
        }
        
        public override void Draw(UIRenderPipe pipe, int zOrder, WinBase pWin, ref SlimDX.Vector4 backColor, ref SlimDX.Matrix parentMatrix)
        {
            IRender pRender = IRender.GetInstance();
		    pRender.SetClipRect(pipe, pWin.ClipRect);

            WinRoot root = (WinRoot)pWin.GetRoot();
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

            var parent = HostWin as RichTextBox;

            foreach (var fontParam in parent.Doc.FontRenderParams.GetAllRenderParams())
            {
                fontParam.Opacity = pWin.Opacity;
            }           
            parent.Doc.Draw(pipe, zOrder + 1);
        }
    }
}
