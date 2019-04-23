using System;
using System.Collections.Generic;

namespace UISystem
{
    public class IRender
    {
        static IRender smRenderer = new IRender();
		public static IRender GetInstance()
        {
            return smRenderer;
        }
		UIRenderer mUIRenderer = new UIRenderer();
        public UIRenderer UIRenderer
        {
            get { return mUIRenderer; }
        }

        public void Cleanup()
        {
            UISystem.UVAnimMgr.Instance.Cleanup();
            UIRenderer.Cleanup();
//            WinKeyboard.Instance.Cleanup();
            UISystem.Template.TemplateMananger.Instance.Cleanup();
        }

        public void DrawImage(UIRenderPipe pipe, int zOrder, float ScreenWidth, float ScreenHeight, 
                                CCore.Graphics.Texture texture, 
                                ref SlimDX.Vector4 backColor,
                                ref CSUtility.Support.Rectangle dst, 
                                ref CSUtility.Support.RectangleF src, 
                                ref SlimDX.Matrix transMat,
                                float opacity,
                                CCore.Material.Material mtl)
        {
            mUIRenderer.DrawImage(pipe, zOrder, ScreenWidth, ScreenHeight, texture, ref backColor, ref dst, ref src, ref transMat, opacity, mtl);
        }

        CCore.Material.Material mDefaultUIRectangleMaterial = null;
        public void FillRectangle(UIRenderPipe pipe, int zOrder, float screenWidth, float screenHeight, CSUtility.Support.Rectangle pDst, ref SlimDX.Vector4 clr,
                                ref SlimDX.Matrix transMat)
        {
            if(mDefaultUIRectangleMaterial==null)
                mDefaultUIRectangleMaterial = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultUIRectangleMaterial);

            mUIRenderer.FillRectangle(pipe, zOrder, screenWidth, screenHeight, pDst, ref clr, ref transMat, mDefaultUIRectangleMaterial);
        }
		public void DrawLine(UIRenderPipe pipe, int zOrder, CSUtility.Support.Point pBegin , CSUtility.Support.Point pEnd , CSUtility.Support.Color clr,
                                float scaleX = 1,
                                float scaleY = 1,
                                CSUtility.Support.Point scaleCenter = new CSUtility.Support.Point())
        {
            mUIRenderer.DrawLine(pipe, zOrder, pBegin , pEnd , clr, scaleX, scaleY, scaleCenter );
        }
		public void DrawLine(UIRenderPipe pipe, int zOrder, CSUtility.Support.Point pBegin , CSUtility.Support.Point pEnd , int width , CSUtility.Support.Color clr,
                                float scaleX = 1,
                                float scaleY = 1,
                                CSUtility.Support.Point scaleCenter = new CSUtility.Support.Point())
        {
            mUIRenderer.DrawLine(pipe, zOrder, pBegin , pEnd , width, clr, scaleX, scaleY, scaleCenter );
        }
        public void DrawLine(UIRenderPipe pipe, int zOrder, float ScreenWidth, float ScreenHeight, SlimDX.Vector2 pBegin, SlimDX.Vector2 pEnd, int width,
                             UISystem.UVAnim uvAnim, float opacity = 1.0f, SlimDX.Vector4 backColor = new SlimDX.Vector4(), SlimDX.Matrix transMat = new SlimDX.Matrix())
        {
            if (uvAnim != null && uvAnim.Frames.Count > 0)
            {
                var frame = uvAnim.GetUVFrame(CCore.Engine.Instance.GetFrameSecondTimeFloatByUVAnim());
                if (uvAnim.MaterialObject != null && uvAnim.MaterialObject.IsMaterialValid())
                {
                    if (frame.Scale9DrawRectangles.Count > 1)
                    {
                        var dst = new CSUtility.Support.Rectangle();
                        dst.X = (int)(pBegin.X - width * 0.5f);
                        dst.Y = (int)(pBegin.Y);
                        dst.Width = width;
                        dst.Height = (int)((SlimDX.Vector2.Subtract(pEnd, pBegin)).Length());

                        foreach (var scaleInfo in frame.Scale9DrawRectangles)
                        {
                            var refRect = scaleInfo.GetDrawRect(dst);

                            SlimDX.Vector4 tempRect = new SlimDX.Vector4(refRect.X, refRect.Y, refRect.Width, refRect.Height);

                            if (dst.Width == 0 || dst.Height == 0)
                            {
                                tempRect = SlimDX.Vector4.Zero;
                            }

                            mUIRenderer.DrawLine(pipe, zOrder, ScreenWidth, ScreenHeight, pBegin, pEnd, width, uvAnim.TextureObject, (int)(scaleInfo.Scale9Type), ref backColor, ref tempRect, ref scaleInfo.mDrawUVRect, opacity, uvAnim.MaterialObject, ref transMat);
                        }
                    }
                    else
                    {
                        var tempRect = SlimDX.Vector4.Zero;
                        mUIRenderer.DrawLine(pipe, zOrder, ScreenWidth, ScreenHeight, pBegin, pEnd, width, uvAnim.TextureObject, (int)(UISystem.Scale9DrawInfo.enScale9Type.None), ref backColor, ref tempRect, ref frame.mUVRect, opacity, uvAnim.MaterialObject, ref transMat);
                    }
                }

                if (uvAnim.CurrentState == UVAnim.enState.FadeOut)
                {
                    // 渐隐处理
                    uvAnim.FadeOutProcess();
                }
            }
            else
            {
                if (backColor.W != 0)
                {
                    //CSUtility.Support.Rectangle dst = pWin.AbsRect;
                    //var tempStart = new CSUtility.Support.Point((int)(Start.X * pWin.Width + dst.Left), (int)(Start.Y * pWin.Height + dst.Top));
                    //var tempEnd = new CSUtility.Support.Point((int)(End.X * pWin.Width + dst.Left), (int)(End.Y * pWin.Height + dst.Top));
                    DrawLine(pipe, zOrder, new CSUtility.Support.Point((int)(pBegin.X), (int)(pBegin.Y)), new CSUtility.Support.Point((int)(pEnd.X), (int)(pEnd.Y)), width, CSUtility.Support.Color.FromArgb((int)(backColor.W * 255), (int)(backColor.X * 255), (int)(backColor.Y * 255), (int)(backColor.Z * 255)));
                }
            }
        }
        public void DrawRect(UIRenderPipe pipe, int zOrder, CSUtility.Support.Rectangle pDst, CSUtility.Support.Color clr,
                                float scaleX = 1,
                                float scaleY = 1,
                                CSUtility.Support.Point scaleCenter = new CSUtility.Support.Point())
        {
            CSUtility.Support.Point pt1 = new CSUtility.Support.Point(pDst.X, pDst.Y);
            CSUtility.Support.Point pt2 = new CSUtility.Support.Point(pDst.Right, pDst.Y);
            CSUtility.Support.Point pt3 = new CSUtility.Support.Point(pDst.Right, pDst.Bottom);
            CSUtility.Support.Point pt4 = new CSUtility.Support.Point(pDst.X, pDst.Bottom);
            mUIRenderer.DrawLine(pipe, zOrder, pt1, pt2, clr, scaleX, scaleY, scaleCenter);
            mUIRenderer.DrawLine(pipe, zOrder, pt2, pt3, clr, scaleX, scaleY, scaleCenter);
            mUIRenderer.DrawLine(pipe, zOrder, pt3, pt4, clr, scaleX, scaleY, scaleCenter);
            mUIRenderer.DrawLine(pipe, zOrder, pt4, pt1, clr, scaleX, scaleY, scaleCenter);
        }

        bool IsValidParam(CCore.Font.FontRenderParamList fontParams)
        {
            if (fontParams.GetParamCount() == 0)
                return false;
            for (int i = 0; i < fontParams.GetParamCount(); ++i)
            {
                var param = fontParams.GetParam(i);
                if (param != null)
                {
                    if (param.OutlineThickness < -1)
                        return false;
                }
            }
            return true;
        }
        public void DrawString(UIRenderPipe pipe, int zOrder, int x, int y, string text, int fontSize, CSUtility.Support.Color clr)
        {
            if (fontSize <= 0)
                return;
            mUIRenderer.DrawString(pipe, zOrder, x, y, text, fontSize, clr);
        }

        public void DrawString(UIRenderPipe pipe, int zOrder, int x, int y, string text, int fontSize, CCore.Font.FontRenderParamList fontParams)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.DrawString(pipe, zOrder, x, y, text, fontSize, fontParams);
        }

        public void DrawString(UIRenderPipe pipe, int zOrder, int x, int y, string text, string absFontName, int fontSize, CSUtility.Support.Color clr)
        {
            if (fontSize <= 0)
                return;
            mUIRenderer.DrawString(pipe, zOrder, x, y, text, absFontName, fontSize, clr);
        }

        public void DrawString(UIRenderPipe pipe, int zOrder, int x, int y, string text, string absFontName, int fontSize, CCore.Font.FontRenderParamList fontParams)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.DrawString(pipe, zOrder, x, y, text, absFontName, fontSize, fontParams);
        }

        public void MeasureStringInLine(string text, int fontSize, CCore.Font.FontRenderParamList fontParams, ref CSUtility.Support.Size oSize, ref int maxTopLine, ref int maxBottomLine, ref int realSize)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.MeasureStringInLine(text, fontSize, fontParams, ref oSize, ref maxTopLine, ref maxBottomLine, ref realSize);
        }

        public void MeasureStringInLine(string absFontName, string text, int fontSize, CCore.Font.FontRenderParamList fontParams, ref CSUtility.Support.Size oSize, ref int maxTopLine, ref int maxBottomLine, ref int realSize)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.MeasureStringInLine(absFontName, text, fontSize, fontParams, ref oSize, ref maxTopLine, ref maxBottomLine, ref realSize);
        }


        public void MeasureStringInLine(string text, int fontSize, CCore.Font.FontRenderParamList fontParams, ref CSUtility.Support.Size oSize)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            int maxTopLine = 0;
            int maxBottomLine = 0;
            int realSize = 0;
            MeasureStringInLine(text, fontSize, fontParams, ref oSize, ref maxTopLine, ref maxBottomLine, ref realSize);
        }

        public void MeasureStringInLine(string absFontName, string text, int fontSize, CCore.Font.FontRenderParamList fontParams, ref CSUtility.Support.Size oSize)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            int maxTopLine = 0;
            int maxBottomLine = 0;
            int realSize = 0;
            MeasureStringInLine(absFontName, text, fontSize, fontParams, ref oSize, ref maxTopLine, ref maxBottomLine, ref realSize);
        }

        public void MeasureStringInWidth(string text, int fontSize, CCore.Font.FontRenderParamList fontParams, int limitWidth, ref CSUtility.Support.Size oSize, List<String> oMultilineTexts, ref int oneLineHeight, ref int maxTopLine, ref int maxBottomLine, ref int realSize)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.MeasureStringInWidth(text, fontSize, fontParams, limitWidth, ref oSize, oMultilineTexts, ref oneLineHeight, ref maxTopLine, ref maxBottomLine, ref realSize);
        }

        public void MeasureStringInWidth(string absFontName, string text, int fontSize, CCore.Font.FontRenderParamList fontParams, int limitWidth, ref CSUtility.Support.Size oSize, List<String> oMultilineTexts, ref int oneLineHeight, ref int maxTopLine, ref int maxBottomLine, ref int realSize)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.MeasureStringInWidth(absFontName, text, fontSize, fontParams, limitWidth, ref oSize, oMultilineTexts, ref oneLineHeight, ref maxTopLine, ref maxBottomLine, ref realSize);
        }

        public void MeasureStringInWidth(string text, int fontSize, CCore.Font.FontRenderParamList fontParams, int limitWidth, ref CSUtility.Support.Size oSize, List<String> oMultilineTexts, ref int oneLineHeight )
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            int maxTopLine = 0;
            int maxBottomLine = 0;
            int realSize = 0;
            MeasureStringInWidth(text, fontSize, fontParams, limitWidth, ref oSize, oMultilineTexts, ref oneLineHeight, ref maxTopLine, ref maxBottomLine, ref realSize);
        }

        public void MeasureStringInWidth(string absFontName, string text, int fontSize, CCore.Font.FontRenderParamList fontParams, int limitWidth, ref CSUtility.Support.Size oSize, List<String> oMultilineTexts, ref int oneLineHeight)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            int maxTopLine = 0;
            int maxBottomLine = 0;
            int realSize = 0;
            MeasureStringInWidth(absFontName, text, fontSize, fontParams, limitWidth, ref oSize, oMultilineTexts, ref oneLineHeight, ref maxTopLine, ref maxBottomLine, ref realSize);
        }

        public void SplitTextInWidth(string absFontName, string text, int fontSize, CCore.Font.FontRenderParamList fontParams, int limitWidth, List<String> oTwolineTexts)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.SplitTextInWidth(absFontName, text, fontSize, fontParams, limitWidth, oTwolineTexts);
        }

        public void SplitTextInHalf(string absFontName, string text, int fontSize, CCore.Font.FontRenderParamList fontParams, int limitWidth, List<String> oTexts)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.SplitTextInHalf(absFontName, text, fontSize, fontParams, limitWidth, oTexts);
        }

        public void PointCheck(string absFontName, string text, int fontSize, CCore.Font.FontRenderParamList fontParams, int x, int y, ref int outX, ref int outY, ref int outPos)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.PointCheck(absFontName, text, fontSize, fontParams, x, y, ref outX, ref outY, ref outPos);
        }

        public void MeasureTextToPos(string absFontName, string text, int fontSize, CCore.Font.FontRenderParamList fontParams, int pos,ref int outWidth)
        {
            if (fontSize <= 0)
                return;
            if (!IsValidParam(fontParams))
                return;
            mUIRenderer.MeasureTextToPos(absFontName, text, fontSize, fontParams, pos, ref outWidth);
        }


        public void SetClipRect(UIRenderPipe pipe, CSUtility.Support.Rectangle pRect)
        {
            m_ClipRect = pRect;
            mUIRenderer.SetClipRect(pipe, pRect);
        }
        public CSUtility.Support.Rectangle GetClipRect()
        {
            return m_ClipRect;
        }
        //public void SetFont(System.Drawing.Font font)
        //{
        //    m_Font = font;
        //    mUIRenderer.SetFont(font);
        //}
        //public void WhenUISetFont(System.Drawing.Font font)
        //{
        //    mUIRenderer.WhenUISetFont(font);
        //}
        //protected System.Drawing.Font m_Font;
		protected CSUtility.Support.Rectangle m_ClipRect;
    }
}
