using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;

namespace UVAnimEditor
{
    public class UIEditorWinRoot : UISystem.WinRoot
    {
        public string FPSString = "";
        //System.Drawing.Rectangle mDestRect = new System.Drawing.Rectangle();
        protected override void AfterStateDraw(UISystem.UIRenderPipe pipe, int zOrder)
        {
            //UISystem.IRender.GetInstance().SetClipRect(this.AbsRect);
            //UISystem.IRender pRender = UISystem.IRender.GetInstance();
            ////UISystem.IRender.GetInstance().SetClipRect(this.AbsRect);
            //pRender.DrawString(20, 20, FPSString, 20, System.Drawing.Color.FromArgb(255, 255, 0, 0));
        }

        //public override UISystem.MSG_PROC ProcMessage(ref UISystem.WinMSG msg)
        //{
        //    return base.ProcMessage(ref msg);
        //}
    }

    /// <summary>
    /// Interaction logic for PreViewPanel.xaml
    /// </summary>
    public partial class PreViewPanel : UserControl, EditorCommon.ITickInfo
    {
        private CCore.Graphics.REnviroment m_REnviroment;
        private CCore.World.WorldRenderParam m_Renderparam;
        CCore.World.World m_World;
        UISystem.WinForm mForm = new UISystem.WinForm();
        UIEditorWinRoot mWinRoot = new UIEditorWinRoot();

        bool mInitialized = false;

        public PreViewPanel()
        {
            InitializeComponent();
        }
        ~PreViewPanel()
        {
            Clear();
        }

        public void Clear()
        {
            EditorCommon.TickInfo.Instance.RemoveTickInfo(this);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!mInitialized)
            {
                m_Renderparam = new CCore.World.WorldRenderParam();
                this.InitD3DEnvironment();

            }

            EditorCommon.TickInfo.Instance.AddTickInfo(this);
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            EditorCommon.TickInfo.Instance.RemoveTickInfo(this);
        }

        private void UserControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            //if (m_REnviroment != null)
            //    m_REnviroment.ResizeInternalRT((System.UInt32)(e.NewSize.Width), (System.UInt32)(e.NewSize.Height));
            mWinRoot.Width = (System.Int32)(e.NewSize.Width);
            mWinRoot.Height = (System.Int32)(e.NewSize.Height);
            mForm.Width = (System.Int32)(e.NewSize.Width);
            mForm.Height = (System.Int32)(e.NewSize.Height);
            mWinDrawPanel.Width = (System.Int32)(e.NewSize.Width);
            mWinDrawPanel.Height = (System.Int32)(e.NewSize.Height);

            CreateRTBitmap(mWinDrawPanel.Width, mWinDrawPanel.Height);
        }

        System.Windows.Media.Imaging.WriteableBitmap mRenderTarget;
        System.Windows.Int32Rect mSourceRect;
        CCore.Graphics.Texture mTargetTexture;
        private void CreateRTBitmap(int pixelWidth, int pixelHeight)
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                if (mTargetTexture != null)
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

        UISystem.UIRenderPipe mUIRenderPipe = new UISystem.UIRenderPipe();
        //UISystem.UIRenderPipe NullUIPipe = new UISystem.UIRenderPipe(IntPtr.Zero);
        public void AfterRender2View(CCore.Graphics.REnviroment env)
        {
            //SlimDX.Matrix transMat = SlimDX.Matrix.Identity;
            //mWinRoot.Draw(mUIRenderPipe, 0, ref transMat);

            UISystem.IRender.GetInstance().UIRenderer.CommitDrawCall(mUIRenderPipe);
        }

        System.Windows.Forms.Panel mWinDrawPanel = new System.Windows.Forms.Panel();
        private void InitD3DEnvironment()
        {
            var _reInit = new CCore.Graphics.REnviromentInit();
            _reInit.ViewInit = new CCore.Graphics.ViewInit();
            _reInit.ViewInit.ViewWnd.SetControl(mWinDrawPanel);
            var view = new CCore.Graphics.View();
            view.Initialize(_reInit.ViewInit);
            m_REnviroment = new CCore.Graphics.REnviroment();
            if (false == m_REnviroment.Initialize(_reInit, view))
                return;

            m_REnviroment.AfterRender2View += this.AfterRender2View;

            m_Renderparam.Enviroment = m_REnviroment;

            var worldInit = new CCore.World.WorldInit();
            m_World = new CCore.World.World(Guid.NewGuid());
            m_World.Initialize(worldInit);
            //m_World.Initialize(new MidLayer.ISingleSceneGraph(), new MidLayer.ICollision(m_World), null);

            //mForm.Left = 0;
            //mForm.Top = 0;
            mForm.Margin = new CSUtility.Support.Thickness(0, 0, 0, 0);
            mForm.Parent = mWinRoot;
            //mForm.Visible = true;
            mForm.Visibility = UISystem.Visibility.Visible;
            mForm.DragEnable = false;
            //mForm.DockMode = System.Windows.Forms.DockStyle.None;
            mForm.ForeColor = CSUtility.Support.Color.White;
            mForm.BackColor = CSUtility.Support.Color.FromArgb(0, 0, 0, 0);// .Gray;
            mForm.FixSizeByUVAnim = false;

            //UI.UISystemManager.Instance.ResizeView(m_REnviroment.View);

            mInitialized = true;
        }

        public void SetUVAnim(UISystem.UVAnim uvAnim)
        {
            mForm.RState.UVAnim = uvAnim;
        }

        void EditorCommon.ITickInfo.Tick()
        {
            if (m_REnviroment != null)
            {
                m_World.Tick();
                m_World.Render2Enviroment(m_Renderparam);

                m_REnviroment.Tick();
                //CCore.Engine.Instance.Client.Graphics.BeginDraw();
                ////m_REnviroment.Render();
                m_REnviroment.RenderUIWithScale(1, 1, CSUtility.Support.Point.Empty);
                //CCore.Engine.Instance.Client.Graphics.EndDraw();

                UISystem.IRender.GetInstance().UIRenderer.ClearAllCommit(mUIRenderPipe);
                mWinRoot.Tick(CCore.Engine.Instance.GetFrameMillisecond());

                SlimDX.Matrix transMat = SlimDX.Matrix.Identity;
                mWinRoot.Draw(mUIRenderPipe, 0, ref transMat);

                UpdateRTBitmapData();

                UISystem.IRender.GetInstance().UIRenderer.SwapQueue(mUIRenderPipe);
            }
        }

    }
}
