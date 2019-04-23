using System.ComponentModel;

namespace UISystem
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class WinState_Progress : WinState
    {
        public enum enHorizontalType
        {
            None,
            LeftToRight,
            RightToLeft,
            CenterToBorder,
            BorderToCenter,
        }
        public enum enVerticalType
        {
            None,
            TopToBottom,
            BottomToTop,
            CenterToBorder,
            BorderToCenter,
        }
        protected float mPercent = 1;
        [Browsable(false)]
        public float Percent
        {
            get { return mPercent; }
            set
            {
                mPercent = value;
                if (mPercent > 1)
                    mPercent = 1;
                else if (mPercent < 0)
                    mPercent = 0;
            }
        }
        enHorizontalType mHorizontalType = enHorizontalType.LeftToRight;
        [Browsable(false)]
        public enHorizontalType HorizontalType
        {
            get { return mHorizontalType; }
            set { mHorizontalType = value; }
        }
        enVerticalType mVerticalType = enVerticalType.None;
        [Browsable(false)]
        public enVerticalType VerticalType
        {
            get { return mVerticalType; }
            set { mVerticalType = value; }
        }

        public enum enProgressType
        {
            Normal,
            Frame,
        }
        enProgressType mProgressType = enProgressType.Normal;
        [Browsable(false)]
        public enProgressType ProgressType
        {
            get { return mProgressType; }
            set { mProgressType = value; }
        }

        public WinState_Progress(WinBase win)
            : base(win)
        {
            
        }

        public override void Draw(UIRenderPipe pipe, int zOrder, WinBase pWin, ref SlimDX.Vector4 backColor, ref SlimDX.Matrix parentMatrix)
        {
            //CounterDrawUI.Begin();

            IRender pRender = IRender.GetInstance();
            pRender.SetClipRect(pipe, pWin.ClipRect);
            //pRender.SetClipRect( pWin.AbsRect );

            WinRoot root = pWin.GetRoot() as WinRoot;
            if (root != null && UVAnim != null && UVAnim.Frames.Count > 0)
            {
                UVFrame frame;
                
                if(ProgressType != enProgressType.Frame)
                    frame = UVAnim.GetUVFrame(CCore.Engine.Instance.GetFrameSecondTimeFloatByUVAnim());
                else
                {
                    if (UVAnim.Frames.Count <= 0)
                        frame = null;
                    else
                    {
                        var idx = (int)(UVAnim.Frames.Count * Percent);
                        if (idx >= UVAnim.Frames.Count)
                            idx = UVAnim.Frames.Count - 1;
                        if (idx < 0)
                            idx = 0;
                        frame = UVAnim.Frames[idx];
                    }
                }

                if (frame == null)
                    return;
                CSUtility.Support.Rectangle dst = pWin.AbsRect;
                //CounterDrawImage.Begin();
                if (Material != null && Material.IsMaterialValid())
                {

                    if (frame.Scale9DrawRectangles.Count > 1)
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
                        var uvRect = frame.mUVRect;

                        if(ProgressType == enProgressType.Normal)
                        //if (Percent != 1)
                        {
                            switch (HorizontalType)
                            {
                                case enHorizontalType.None:
                                    break;
                                case enHorizontalType.LeftToRight:
                                    {
                                        uvRect.Width *= Percent;
                                        dst.Width = (int)(dst.Width * Percent);
                                    }
                                    break;
                                case enHorizontalType.RightToLeft:
                                    {
                                        uvRect.X += uvRect.Width * (1 - Percent);
                                        uvRect.Width *= Percent;
                                        dst.X += (int)(dst.Width * (1 - Percent));
                                        dst.Width = (int)(dst.Width * Percent);
                                    }
                                    break;
                                case enHorizontalType.CenterToBorder:
                                    {
                                        uvRect.Width *= Percent;
                                        uvRect.X += (frame.mUVRect.Width - uvRect.Width) / 2;
                                        dst.Width = (int)(dst.Width * Percent);
                                        dst.X = (int)(dst.X + (pWin.AbsRect.Width - dst.Width) / 2);
                                    }
                                    break;
                                case enHorizontalType.BorderToCenter:
                                    {
                                        uvRect.Width *= 1 - Percent;
                                        uvRect.X += (frame.mUVRect.Width - uvRect.Width) / 2;
                                        dst.Width = (int)(dst.Width * (1 - Percent));
                                        dst.X = (int)(dst.X + (pWin.AbsRect.Width - dst.Width) / 2);
                                    }
                                    break;
                            }

                            switch (VerticalType)
                            {
                                case enVerticalType.None:
                                    break;
                                case enVerticalType.TopToBottom:
                                    {
                                        uvRect.Height *= Percent;
                                        dst.Height = (int)(dst.Height * Percent);
                                    }
                                    break;
                                case enVerticalType.BottomToTop:
                                    {
                                        uvRect.Y += uvRect.Height * (1 - Percent);
                                        uvRect.Height *= Percent;
                                        dst.Y = (int)(dst.Y + dst.Height * (1 - Percent));
                                        dst.Height = (int)(dst.Height * Percent);
                                    }
                                    break;
                                case enVerticalType.CenterToBorder:
                                    {
                                        uvRect.Height *= Percent;
                                        uvRect.Y += (frame.mUVRect.Height - uvRect.Height) / 2;
                                        dst.Height = (int)(dst.Height * Percent);
                                        dst.Y = (int)(dst.Y + (pWin.AbsRect.Height - dst.Height) / 2);
                                    }
                                    break;
                                case enVerticalType.BorderToCenter:
                                    {
                                        uvRect.Height *= 1 - Percent;
                                        uvRect.Y += (frame.mUVRect.Height - uvRect.Height) / 2;
                                        dst.Height = (int)(dst.Height * (1 - Percent));
                                        dst.Y = (int)(dst.Y + (pWin.AbsRect.Height - dst.Height) / 2);
                                    }
                                    break;
                            }
                        }

                        pRender.DrawImage(pipe, zOrder, root.Width, root.Height, Texture, ref backColor,
                        ref dst,
                        ref uvRect, ref parentMatrix, pWin.Opacity, Material);
                    }
                }
                //CounterDrawImage.End();
            }
            else
            {
                if (backColor.W != 0)
                {
                    //CounterDrawRect.Begin();
                    pRender.FillRectangle(pipe, zOrder, root.Width, root.Height, pWin.AbsRect, ref backColor, ref parentMatrix);
                    //CounterDrawRect.End();
                }
            }

            //CounterDrawUI.End();
        }
    }
}
