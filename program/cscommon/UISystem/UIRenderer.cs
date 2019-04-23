using System;
using System.Collections.Generic;

namespace UISystem
{
    public class UIRenderer
    {
        IntPtr mUIRenderPtr; // v3dUIRender

        string mAbsFontName;
        public string AbsFontName
        {
            get
            {
                if (string.IsNullOrEmpty(mAbsFontName))
                    mAbsFontName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(CSUtility.Support.IFileConfig.DefaultFont);
                return mAbsFontName;
            }
            set{ mAbsFontName = value; }
        }
        CSUtility.Support.Rectangle m_ClipRect;

        static CSUtility.Performance.PerfCounter CounterRectMeshRender = new CSUtility.Performance.PerfCounter("UIMeshRender");
        static UIRenderer()
        {
        }
        public UIRenderer()
        {
            unsafe
            {
                mUIRenderPtr = CCore.DllImportAPI.v3dUIRender_New(CCore.Engine.Instance.Client.Graphics.Device);
            }
        }
        ~UIRenderer()
        {
            Cleanup();
        }
        public void Cleanup()
        {
            unsafe
            {
                if (mUIRenderPtr != IntPtr.Zero)
                {
                    CCore.DllImportAPI.v3dUIRender_Release(mUIRenderPtr);
                    mUIRenderPtr = IntPtr.Zero;
                }
            }
        }
        public void CommitDrawCall(UIRenderPipe pipe)
        {
            CCore.DllImportAPI.v3dUIRender_CommitDrawCall(mUIRenderPtr, pipe.InnerPtr);
        }
        public void SwapQueue(UIRenderPipe pipe)
        {
            CCore.DllImportAPI.v3dUIRender_SwapQueue(mUIRenderPtr, pipe.InnerPtr);
        }
        public void ClearAllCommit(UIRenderPipe pipe)
        {
            CCore.DllImportAPI.v3dUIRender_ClearAllCommit(pipe.InnerPtr);
        }
        static CSUtility.Performance.PerfCounter mDrawImageTimer = new CSUtility.Performance.PerfCounter("UIRenderer.DrawImage");
        public void DrawImage(UIRenderPipe pipe, int zOrder, float ScreenWidth, float ScreenHeight, CCore.Graphics.Texture texture, ref SlimDX.Vector4 backColor, ref CSUtility.Support.Rectangle pDstOri, ref CSUtility.Support.RectangleF pSrcOri, ref SlimDX.Matrix transMat, float opacity, CCore.Material.Material mtl)
        {
            try
            {
                //mDrawImageTimer.Begin();
                unsafe
                {
                    if (texture == null)
                        return;

                    CSUtility.Support.Rectangle pDst = CSUtility.Support.Rectangle.Intersect(m_ClipRect, pDstOri);
                    if (pDst == CSUtility.Support.Rectangle.Empty)
                        return;

                    int[] rectData = new int[8];
                    rectData[0] = pDst.Left;
                    rectData[1] = pDst.Top;
                    rectData[2] = pDst.Right;
                    rectData[3] = pDst.Bottom;
                    rectData[4] = pDstOri.Left;
                    rectData[5] = pDstOri.Top;
                    rectData[6] = pDstOri.Right;
                    rectData[7] = pDstOri.Bottom;

                    float[] srcOriData = new float[4];
                    srcOriData[0] = pSrcOri.X;
                    srcOriData[1] = pSrcOri.Y;
                    srcOriData[2] = pSrcOri.Width;
                    srcOriData[3] = pSrcOri.Height;

                    float[] floatData = new float[2];
                    floatData[0] = ScreenWidth;
                    floatData[1] = ScreenHeight;
                    //floatData[2] = scaleX;
                    //floatData[3] = scaleY;
                    //floatData[4] = scaleCenter.X;
                    //floatData[5] = scaleCenter.Y;

                    fixed (int* pinRectData = rectData)
                    {
                        fixed (float* pinSrcOriData = srcOriData)
                        {
                            fixed (float* pinFloatData = floatData)
                            {
                                fixed (SlimDX.Matrix* pinTransMat = &transMat)
                                {
                                    fixed (SlimDX.Vector4* pinBackColor = &backColor)
                                    {
                                        CCore.DllImportAPI.v3dUIRender_DrawImage(pipe.InnerPtr, zOrder, mUIRenderPtr, texture.TexturePtr, pinBackColor, opacity, pinRectData, pinSrcOriData, pinFloatData, pinTransMat, mtl.MaterialPtr);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            finally
            {
                //mDrawImageTimer.End();
            }
        }
        static CSUtility.Performance.PerfCounter mFillRectTimer = new CSUtility.Performance.PerfCounter("UIRenderer.FillRectangle");
        public void FillRectangle(UIRenderPipe pipe, int zOrder, float screenWidth, float screenHeight, CSUtility.Support.Rectangle pDstOri, ref SlimDX.Vector4 color, ref SlimDX.Matrix transMat, CCore.Material.Material mtl)
        {
            try
            {
                //mFillRectTimer.Begin();
                if (color.W == 0)
                    return;
                CSUtility.Support.Rectangle pDst = CSUtility.Support.Rectangle.Intersect(m_ClipRect, pDstOri);
                if (pDst == CSUtility.Support.Rectangle.Empty)
                {
                    return;
                }
                else
                {
                    //mtl.SetFloat4("BackColor", ref color);

                    unsafe
                    {
                        fixed (SlimDX.Matrix* pinMat = &transMat)
                        {
                            fixed (SlimDX.Vector4* pinColor = &color)
                            {
                                CCore.DllImportAPI.v3dUIRender_FillRectangle(pipe.InnerPtr, zOrder, mUIRenderPtr, pinColor, pDst.Left, pDst.Top, pDst.Width, pDst.Height, screenWidth, screenHeight, pinMat, mtl.MaterialPtr);
                            }
                        }
                    }
                }
            }
            finally
            {
                //mFillRectTimer.End();
            }
        }
        static CSUtility.Performance.PerfCounter mDrawLineTimer = new CSUtility.Performance.PerfCounter("UIRenderer.DrawLine");
        public void DrawLine(UIRenderPipe pipe, int zOrder, CSUtility.Support.Point pBegin , CSUtility.Support.Point pEnd , CSUtility.Support.Color clr, float scaleX, float scaleY, CSUtility.Support.Point scaleCenter )
        {
            try
            {
                mDrawLineTimer.Begin();
                unsafe
                {
                    int[] intData = new int[10];
                    intData[0] = pBegin.X;
                    intData[1] = pBegin.Y;
                    intData[2] = pEnd.X;
                    intData[3] = pEnd.Y;
                    intData[4] = m_ClipRect.Left;
                    intData[5] = m_ClipRect.Right;
                    intData[6] = m_ClipRect.Top;
                    intData[7] = m_ClipRect.Bottom;
                    intData[8] = scaleCenter.X;
                    intData[9] = scaleCenter.Y;

                    fixed (int* pinIntData = intData)
                    {
                        CCore.DllImportAPI.v3dUIRender_DrawLine(pipe.InnerPtr, zOrder, mUIRenderPtr, pinIntData, (UInt32)(clr.ToArgb()), scaleX, scaleY);
                    }
                }
            }
            finally
            {
                mDrawLineTimer.End();
            }
        }

        public void DrawLine(UIRenderPipe pipe, int zOrder, CSUtility.Support.Point pBegin, CSUtility.Support.Point pEnd, int width, CSUtility.Support.Color clr, float scaleX, float scaleY, CSUtility.Support.Point scaleCenter)
        {
            try
            {
                mDrawLineTimer.Begin();
                unsafe
                {
                    int[] intData = new int[10];
                    intData[0] = pBegin.X;
                    intData[1] = pBegin.Y;
                    intData[2] = pEnd.X;
                    intData[3] = pEnd.Y;
                    intData[4] = m_ClipRect.Left;
                    intData[5] = m_ClipRect.Right;
                    intData[6] = m_ClipRect.Top;
                    intData[7] = m_ClipRect.Bottom;
                    intData[8] = scaleCenter.X;
                    intData[9] = scaleCenter.Y;

                    fixed (int* pinIntData = intData)
                    {
                        CCore.DllImportAPI.v3dUIRender_DrawWidthLine(pipe.InnerPtr, zOrder, mUIRenderPtr, pinIntData, width, (UInt32)(clr.ToArgb()), scaleX, scaleY);
                    }
                }
            }
            finally
            {
                mDrawLineTimer.End();
            }
        }

        public void DrawLine(UIRenderPipe pipe, int zOrder, float ScreenWidth, float ScreenHeight, SlimDX.Vector2 pBegin, SlimDX.Vector2 pEnd, int width, CCore.Graphics.Texture texture, int scale9Type, ref SlimDX.Vector4 backColor, ref SlimDX.Vector4 rectRate, ref CSUtility.Support.RectangleF pSrcOri, float opacity, CCore.Material.Material mtl, ref SlimDX.Matrix transMat)
        {
            try
            {
                mDrawLineTimer.Begin();
                unsafe
                {
                    if (texture == null)
                        return;

                    int[] rectData = new int[5];
                    rectData[0] = (int)pBegin.X;
                    rectData[1] = (int)pBegin.Y;
                    rectData[2] = (int)pEnd.X;
                    rectData[3] = (int)pEnd.Y;
                    rectData[4] = (int)scale9Type;

                    float[] srcOriData = new float[4];
                    srcOriData[0] = pSrcOri.X;
                    srcOriData[1] = pSrcOri.Y;
                    srcOriData[2] = pSrcOri.Width;
                    srcOriData[3] = pSrcOri.Height;

                    float[] floatData = new float[6];
                    floatData[0] = ScreenWidth;
                    floatData[1] = ScreenHeight;
                    floatData[2] = rectRate.X;
                    floatData[3] = rectRate.Y;
                    floatData[4] = rectRate.Z;
                    floatData[5] = rectRate.W;

                    //if (texture != null)
                    //{
                    //    if (mtl.CachedShaderVar0 == IntPtr.Zero)
                    //    {
                    //        mtl.CachedShaderVar0 = mtl.SetTexture("GDiffuseTexture", texture);
                    //    }
                    //    else
                    //    {
                    //        mtl.SetTextureByShaderVar(mtl.CachedShaderVar0, texture);
                    //    }
                    //}
                    //if (mtl.CachedShaderVar1 == IntPtr.Zero)
                    //{
                    //    mtl.CachedShaderVar1 = mtl.SetFloat4("BackColor", ref backColor);
                    //}
                    //else
                    //{
                    //    mtl.SetFloat4ByShaderVar(mtl.CachedShaderVar1, ref backColor);
                    //}

                    //if (mtl.CachedShaderVar2 == IntPtr.Zero)
                    //{
                    //    mtl.CachedShaderVar2 = mtl.SetFloat("Opacity", opacity);
                    //}
                    //else
                    //{
                    //    mtl.SetFloatByShaderVar(mtl.CachedShaderVar2, opacity);
                    //}

                    fixed (int* pinRectData = rectData)
                    {
                        fixed (float* pinSrcOriData = srcOriData)
                        {
                            fixed (float* pinFloatData = floatData)
                            {
                                fixed (SlimDX.Matrix* pinTransMat = &transMat)
                                    CCore.DllImportAPI.v3dUIRender_DrawLineWithMaterial(pipe.InnerPtr, zOrder, mUIRenderPtr, pinRectData, width, pinSrcOriData, pinFloatData, pinTransMat, mtl.MaterialPtr);
                            }
                        }
                    }
                }
            }
            finally
            {
                mDrawLineTimer.End();
            }
        }

        static CSUtility.Performance.PerfCounter mDrawStringTimer = new CSUtility.Performance.PerfCounter("UIRenderer.DrawString");
        public void DrawString(UIRenderPipe pipe, int zOrder, int x , int y , System.String text , int size, CSUtility.Support.Color clr )
        {
            //mDrawStringTimer.Begin();
            DrawString(pipe, zOrder, x, y, text, mAbsFontName, size, clr);
            //mDrawStringTimer.End();
        }
		public void DrawString(UIRenderPipe pipe, int zOrder, int x , int y , System.String text , int size, CCore.Font.FontRenderParamList pars )
        {
            //mDrawStringTimer.Begin();
            DrawString(pipe, zOrder, x, y, text, mAbsFontName, size, pars);
            //mDrawStringTimer.End();
        }
		public void DrawString(UIRenderPipe pipe, int zOrder, int x , int y , System.String text , System.String absFontName , int size, CSUtility.Support.Color clr )
        {
            if (string.IsNullOrEmpty(text))
                return;

            //mDrawStringTimer.Begin();
            unsafe
            {
                CCore.DllImportAPI.v3dUIRender_DrawString(pipe.InnerPtr, zOrder, mUIRenderPtr, x, y, absFontName, size, text, (UInt32)clr.ToArgb());
            }
            //mDrawStringTimer.End();
        }
		public void DrawString(UIRenderPipe pipe, int zOrder, int x , int y , System.String text , System.String absFontName , int size, CCore.Font.FontRenderParamList pars )
        {
            if (string.IsNullOrEmpty(text))
                return;

            //mDrawStringTimer.Begin();
            unsafe
            {
                
                CCore.DllImportAPI.V3DFontRenderParamList_SetFontAttribs(pars.Inner, absFontName, size, text);                
                CCore.DllImportAPI.v3dUIRender_DrawStringWithParams(pipe.InnerPtr, zOrder, mUIRenderPtr, x, y, pars.Inner);
            }
            //mDrawStringTimer.End();
        }

		public void MeasureStringInWidth( System.String text , int size, CCore.Font.FontRenderParamList pars, int limitWidth, ref CSUtility.Support.Size oTextSize )
        {
            int oneLineHeight = 0, maxTopLine = 0, maxBottomLine = 0, realSize = 0;
            MeasureStringInWidth(text, size, pars, limitWidth, ref oTextSize, null, ref oneLineHeight, ref maxTopLine, ref maxBottomLine, ref realSize);
        }
		public void MeasureStringInWidth( System.String text , int size, CCore.Font.FontRenderParamList pars, int limitWidth, ref CSUtility.Support.Size oTextSize, List<System.String> oMultilineTexts, ref int oneLineHeight, ref int maxTopLine, ref int maxBottomLine, ref int realSize )
        {
            MeasureStringInWidth(mAbsFontName, text, size, pars, limitWidth, ref oTextSize, null, ref oneLineHeight, ref maxTopLine, ref maxBottomLine, ref realSize);
        }
		public void MeasureStringInWidth( System.String fontName, System.String text , int size, CCore.Font.FontRenderParamList pars, int limitWidth, ref CSUtility.Support.Size oTextSize )
        {
            int oneLineHeight = 0, maxTopLine = 0, maxBottomLine = 0, realSize = 0;
            MeasureStringInWidth(fontName, text, size, pars, limitWidth, ref oTextSize, null, ref oneLineHeight, ref maxTopLine, ref maxBottomLine, ref realSize);
        }
		public void MeasureStringInWidth( System.String fontName, System.String text , int size, CCore.Font.FontRenderParamList pars, int limitWidth, ref CSUtility.Support.Size oTextSize, List<System.String> oMultilineTexts, ref int oneLineHeight, ref int maxTopLine, ref int maxBottomLine, ref int realSize )
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (string.IsNullOrEmpty(fontName))
                fontName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(CSUtility.Support.IFileConfig.DefaultFont);
            else
                fontName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(fontName);

            unsafe
            {
                fixed (CSUtility.Support.Size* pinTextSize = &oTextSize)
                {
                    fixed (int* pinOneLineHeight = &oneLineHeight)
                    {
                        fixed (int* pinMaxTopLine = &maxTopLine)
                        {
                            fixed (int* pinMaxBottomLine = &maxBottomLine)
                            {
                                fixed (int* pinRealSize = &realSize)
                                {
                                    int strVectorCount = 0;
                                    void** strArray = CCore.DllImportAPI.v3dUIRender_MeasureStringInWidth(mUIRenderPtr,
                                                                                   fontName,
                                                                                   size,
                                                                                   text,
                                                                                   text.Length,
                                                                                   pars.Inner,
                                                                                   limitWidth,
                                                                                   pinTextSize,
                                                                                   pinOneLineHeight,
                                                                                   pinMaxTopLine,
                                                                                   pinMaxBottomLine,
                                                                                   pinRealSize,
                                                                                   &strVectorCount);

                                    oMultilineTexts.Clear();
                                    for (int i = 0; i < strVectorCount; ++i)
                                    {
                                        var str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(strArray[i]));
                                        oMultilineTexts.Add(str);
                                    }

                                    CCore.DllImportAPI.v3dUIRender_DeleteStrings(strArray, strVectorCount);
                                }
                            }
                        }
                    }
                    
                }

            }
        }
		public void MeasureStringInLine( System.String text , int size, CCore.Font.FontRenderParamList pars, ref CSUtility.Support.Size oTextSize, ref int maxTopLine, ref int maxBottomLine, ref int realSize )
        {
            MeasureStringInLine(mAbsFontName, text, size, pars, ref oTextSize, ref maxTopLine, ref maxBottomLine, ref realSize);
        }
		public void MeasureStringInLine( System.String fontName, System.String text , int size, CCore.Font.FontRenderParamList pars, ref CSUtility.Support.Size oTextSize, ref int maxTopLine, ref int maxBottomLine, ref int realSize )
        {
            unsafe
            {
                fixed (CSUtility.Support.Size* pinTextSize = &oTextSize)
                {
                    fixed (int* pinMaxTopLine = &maxTopLine)
                    {
                        fixed (int* pinMaxBottomLine = &maxBottomLine)
                        {
                            fixed (int* pinRealSize = &realSize)
                            {
                                CCore.DllImportAPI.v3dUIRender_MeasureStringInLine(mUIRenderPtr,
                                                                              fontName,
                                                                              size,
                                                                              text,
                                                                              text.Length,
                                                                              pars.Inner,
                                                                              pinTextSize,
                                                                              pinMaxTopLine,
                                                                              pinMaxBottomLine,
                                                                              pinRealSize);
                            }
                        }
                    }
                }
            }
        }

		public void SplitTextInWidth( System.String fontName, System.String text , int size, CCore.Font.FontRenderParamList pars, int limitWidth, List<System.String> oTwolineTexts )
        {
            unsafe
            {
                int strVectorCount = 0;
                void** strArray = CCore.DllImportAPI.v3dUIRender_SplitTextInWidth(mUIRenderPtr, fontName, size, text, text.Length, pars.Inner, limitWidth, &strVectorCount);

                oTwolineTexts.Clear();
                for (int i = 0; i < strVectorCount; ++i)
                {
                    var str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(strArray[i]));
                    oTwolineTexts.Add(str);
                }

                CCore.DllImportAPI.v3dUIRender_DeleteStrings(strArray, strVectorCount);
            }
        }
		public void SplitTextInHalf( System.String fontName, System.String text , int size, CCore.Font.FontRenderParamList pars, int limitWidth, List<System.String> oTexts )
        {
            unsafe
            {
                int strVectorCount = 0;
                void** strArray = CCore.DllImportAPI.v3dUIRender_SplitTextInHalf(mUIRenderPtr, fontName, size, text, text.Length, pars.Inner, limitWidth, &strVectorCount);

                oTexts.Clear();
                for (int i = 0; i < strVectorCount; ++i)
                {
                    var str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(strArray[i]));
                    oTexts.Add(str);
                }

                CCore.DllImportAPI.v3dUIRender_DeleteStrings(strArray, strVectorCount);
            }
        }

		public void PointCheck(System.String fontName, System.String text , int size, CCore.Font.FontRenderParamList pars, int x, int y, ref int outX, ref int outY, ref int outPos )
        {
            unsafe
            {
                fixed (int* pinOutX = &outX)
                {
                    fixed (int* pinOutY = &outY)
                    {
                        fixed (int* pinOutPos = &outPos)
                        {
                            CCore.DllImportAPI.v3dUIRender_PointCheck(mUIRenderPtr, fontName, size, text, text.Length, pars.Inner, x, y, pinOutX, pinOutY, pinOutPos);
                        }
                    }
                }
            }
        }
        public void MeasureTextToPos(System.String fontName, System.String text, int size, CCore.Font.FontRenderParamList pars, int pos, ref int outWidth)
        {
            unsafe
            {
                fixed(int* pinOutWidth = &outWidth)
                {
                    CCore.DllImportAPI.v3dUIRender_MeasureTextToPos(mUIRenderPtr, fontName, size, text, text.Length, pars.Inner, pos, pinOutWidth);
                }
            }
        }

        public void SetClipRect(UIRenderPipe pipe, CSUtility.Support.Rectangle pRect)
        {
            unsafe
            {
                m_ClipRect = pRect;
                SlimDX.Vector4 rect = new SlimDX.Vector4(pRect.Left, pRect.Top, pRect.Right, pRect.Bottom);
                CCore.DllImportAPI.v3dUIRender_SetClipRect(pipe.InnerPtr, mUIRenderPtr, &rect);
            }
        }
    }
}
