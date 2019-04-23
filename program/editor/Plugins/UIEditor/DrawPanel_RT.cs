using System;
using System.Drawing;
using System.Windows.Media;

namespace UIEditor
{
    public partial class DrawPanel
    {
        System.Windows.Media.Imaging.WriteableBitmap mRenderTarget;
        System.Windows.Int32Rect mSourceRect;
        CCore.Graphics.Texture mTargetTexture;

        private void CreateRTBitmap(int pixelWidth, int pixelHeight)
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                if(mTargetTexture != null)
                {
                    mTargetTexture.Cleanup();
                }
                mTargetTexture = CCore.Graphics.Texture.CreateTexture((UInt32)pixelWidth, (UInt32)pixelHeight, (int)CCore.BufferFormat.D3DFMT_A8R8G8B8, 0, (UInt32)CCore.RenderAPI.V3DUSAGE_DYNAMIC, (int)CCore.RenderAPI.V3DPOOL.V3DPOOL_SYSTEMMEM);

                mSourceRect = new System.Windows.Int32Rect(0, 0, pixelWidth, pixelHeight);
                float dpiX = graphics.DpiX;
                float dpiY = graphics.DpiY;
                mRenderTarget = new System.Windows.Media.Imaging.WriteableBitmap(pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Bgra32, null);

                Image_RT.Source = mRenderTarget;
            }
        }

        public void UpdateRTBitmapData()
        {
            if (mRenderTarget == null || mTargetTexture == null)
                return;

            byte[] datas = new byte[mRenderTarget.PixelWidth * mRenderTarget.PixelHeight * 4];
            m_REnviroment.GetFinalTexturePixelData(ref datas, mTargetTexture);
            mRenderTarget.WritePixels(mSourceRect, datas, mRenderTarget.PixelWidth * 4, 0);
        }
    }
}
