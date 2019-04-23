using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TerrainEditor
{
    /// <summary>
    /// Interaction logic for TerrainGridPanel.xaml
    /// </summary>
    public partial class TerrainGridPanel : UserControl
    {
        static TerrainGridPanel smInstance = null;
        public static TerrainGridPanel Instance
        {
            get
            {
                if (smInstance == null)
                    smInstance = new TerrainGridPanel();
                return smInstance;
            }
        }

        CCore.Terrain.Terrain mHostTerrain;
        public CCore.Terrain.Terrain HostTerrain
        {
            get { return mHostTerrain; }
            set
            {
                mHostTerrain = value;

                InitializeWithTerrain(mHostTerrain);
            }
        }


        CCore.Camera.CameraController mEditorCameraController;
        public CCore.Camera.CameraController EditorCameraController
        {
            get { return mEditorCameraController; }
            set
            {
                mEditorCameraController = value;

                UpdateCamera();
            }
        }

        public CCore.Graphics.REnviroment HostEnviroment;
        public CCore.World.WorldRenderParam HostRenderParam;
        public CCore.World.World HostWorld;

        enum enEditMode
        {
            Camera,
            Brush,
            Erase,
        }
        enEditMode mEditMode = enEditMode.Camera;

        int mLevelXCount = 512;
        int mLevelZCount = 512;
        WriteableBitmap mTerrainLevelsBitmap = null;
        WriteableBitmap mBackgroundBitmap = new WriteableBitmap(1024, 1024, 72, 72, PixelFormats.Bgra32, null);
        byte[] mAvailableLevelColor = new byte[] { 0, 0, 255, 100 };
        byte[] mUnavailableLevelColor = new byte[] { 0, 0, 0, 0 };

        // 网格辅助线
        static int mHorizontalLineCount = 100;
        static int mVerticalLineCount = 100;
        Line[] mHorizontalLines = new Line[mHorizontalLineCount];
        Line[] mVerticalLines = new Line[mVerticalLineCount];
        static int mHorizontalLineShowLimit = 15;
        static int mVerticalLineShowLimit = 15;

        public TerrainGridPanel()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(Image_TerrainLevels, BitmapScalingMode.NearestNeighbor);
            InitializeAssistLines();
        }

        private void InitializeWithTerrain(CCore.Terrain.Terrain terrain)
        {
            if (terrain == null || !terrain.IsAvailable())
                return;

            mLevelXCount = (int)terrain.GetLevelXCount();
            mLevelZCount = (int)terrain.GetLevelZCount();

            mTerrainLevelsBitmap = new WriteableBitmap(mLevelXCount, mLevelZCount, 72, 72, PixelFormats.Bgra32, null);
            Image_TerrainLevels.Source = mTerrainLevelsBitmap;

            var levelData = terrain.GetLevelData();

            for (int z = 0; z < mLevelZCount; z++)
            {
                for (int x = 0; x < mLevelXCount; x++)
                {
                    var data = levelData[z * mLevelXCount + x];

                    var rect = new Int32Rect(x, mLevelZCount - 1 - z, 1, 1);
                    byte[] colorData = mUnavailableLevelColor; // bgra
                    if (data)
                        colorData = mAvailableLevelColor;
                    mTerrainLevelsBitmap.WritePixels(rect, colorData, 4, 0);
                }
            }
        }

        private void InitializeAssistLines()
        {
            for (int i = 0; i < mHorizontalLineCount; i++)
            {
                mHorizontalLines[i] = new Line()
                {
                    Stroke = Brushes.Yellow,
                    Opacity = 0.5,
                };
                Canvas_TerrainLevels.Children.Add(mHorizontalLines[i]);
            }
            for (int i = 0; i < mVerticalLineCount; i++)
            {
                mVerticalLines[i] = new Line()
                {
                    Stroke = Brushes.Yellow,
                    Opacity = 0.5,
                };
                Canvas_TerrainLevels.Children.Add(mVerticalLines[i]);
            }
        }

        private void UpdateAssistLines()
        {
            // 纵向线起始位置
            var horizontalStart = Canvas.GetLeft(Image_TerrainLevels);
            // 纵向线间隔
            var horizontalDelta = Image_TerrainLevels.Width / mLevelXCount;
            // 横向线起始位置
            var verticalStart = Canvas.GetTop(Image_TerrainLevels);
            // 横向线间隔
            var verticalDelta = Image_TerrainLevels.Height / mLevelZCount;

            if (horizontalDelta > mHorizontalLineShowLimit)
            {
                for (int i = 0; i < mHorizontalLineCount; i++)
                {
                    if (mHorizontalLines[i].Visibility != System.Windows.Visibility.Visible)
                        mHorizontalLines[i].Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                for (int i = 0; i < mHorizontalLineCount; i++)
                {
                    if (mHorizontalLines[i].Visibility != System.Windows.Visibility.Hidden)
                        mHorizontalLines[i].Visibility = System.Windows.Visibility.Hidden;
                }
            }

            if (verticalDelta > mVerticalLineShowLimit)
            {
                for (int i = 0; i < mVerticalLineCount; i++)
                {
                    if (mVerticalLines[i].Visibility != System.Windows.Visibility.Visible)
                        mVerticalLines[i].Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                for (int i = 0; i < mVerticalLineCount; i++)
                {
                    if (mVerticalLines[i].Visibility != System.Windows.Visibility.Hidden)
                        mVerticalLines[i].Visibility = System.Windows.Visibility.Hidden;
                }
            }

            if (horizontalStart < 0)
            {
                if (System.Math.Abs(horizontalStart) <= (Image_TerrainLevels.Width - horizontalDelta * (mHorizontalLineCount - 1)))
                    horizontalStart -= (int)(horizontalStart / horizontalDelta) * horizontalDelta;
                else
                    horizontalStart += Image_TerrainLevels.Width - horizontalDelta * (mHorizontalLineCount - 1);
            }

            var top = Canvas.GetTop(Image_TerrainLevels);
            var bottom = top + Image_TerrainLevels.Height;
            if (top < 0)
                top = 0;
            if (bottom > Canvas_TerrainLevels.Height)
                bottom = Canvas_TerrainLevels.Height;

            for (int i = 0; i < mHorizontalLineCount; i++)
            {
                mHorizontalLines[i].X1 = horizontalStart + i * horizontalDelta;
                mHorizontalLines[i].Y1 = top;
                mHorizontalLines[i].X2 = horizontalStart + i * horizontalDelta;
                mHorizontalLines[i].Y2 = bottom;
            }

            if (verticalStart < 0)
            {
                if (System.Math.Abs(verticalStart) <= (Image_TerrainLevels.Height - verticalDelta * (mVerticalLineCount - 1)))
                    verticalStart -= (int)(verticalStart / verticalDelta) * verticalDelta;
                else
                    verticalStart += Image_TerrainLevels.Height - verticalDelta * (mVerticalLineCount - 1);
            }

            var left = Canvas.GetLeft(Image_TerrainLevels);
            var right = left + Image_TerrainLevels.Width;
            if (left < 0)
                left = 0;
            if (right > Canvas_TerrainLevels.Width)
                right = Canvas_TerrainLevels.Width;

            for (int i = 0; i < mVerticalLineCount; i++)
            {
                mVerticalLines[i].X1 = left;
                mVerticalLines[i].Y1 = verticalStart + i * verticalDelta;
                mVerticalLines[i].X2 = right;
                mVerticalLines[i].Y2 = verticalStart + i * verticalDelta;
            }
        }

        public void UpdateCamera()
        {
            //////if (EditorCameraController == null || HostTerrain == null)
            //////    return;

            //////// 根据摄像机位置更新网格上摄像机的位置
            //////var eyeLoc = EditorCameraController.Camera.GetLocation();
            //////SlimDX.Vector3 terrainStartLoc = new SlimDX.Vector3();
            //////HostTerrain.GetStartLocation(ref terrainStartLoc);

            //////var lvImgLeft = Canvas.GetLeft(Image_TerrainLevels);
            //////var lvImgTop = Canvas.GetTop(Image_TerrainLevels);

            //////var x = (eyeLoc.X - terrainStartLoc.X) * (Image_TerrainLevels.Width / HostTerrain.GetGridX()) + lvImgLeft;
            //////var z = (Image_TerrainLevels.Height - (eyeLoc.Z - terrainStartLoc.Z) * (Image_TerrainLevels.Height / HostTerrain.GetGridZ())) + lvImgTop;

            //////Canvas.SetLeft(Grid_Camera, x);
            //////Canvas.SetTop(Grid_Camera, z);

            //////var dir = EditorCameraController.Camera.Direction;
            //////dir.Y = 0;
            //////dir.Normalize();
            //////var dirCross = SlimDX.Vector3.Cross(dir, SlimDX.Vector3.UnitX);
            //////var angle = System.Math.Acos(SlimDX.Vector3.Dot(dir, SlimDX.Vector3.UnitX));
            //////if (dirCross.Y > 0)
            //////    angle = -angle;
            //////ArcCameraRotate.Angle = angle * 180 / System.Math.PI;
        }

        #region 鼠标操作相关

        Point mMouseRightButtonDownPoint = new Point();
        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                mMouseRightButtonDownPoint = e.GetPosition(Image_TerrainLevels);
                Mouse.Capture(Image_TerrainLevels);
            }
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pt = e.GetPosition(Image_TerrainLevels);

                switch (mEditMode)
                {
                    case enEditMode.Camera:
                        {
                            var posX = pt.X / Image_TerrainLevels.Width * mLevelXCount;
                            var posZ = (Image_TerrainLevels.Height - pt.Y) / Image_TerrainLevels.Height * mLevelZCount;

                            posX = posX / mLevelXCount * HostTerrain.GetGridXCount();
                            posZ = posZ / mLevelZCount * HostTerrain.GetGridZCount();

                            var eyeLoc = EditorCameraController.Camera.GetLocation();
                            var eyeDir = EditorCameraController.Camera.Direction;
                            eyeLoc.X = (float)posX;
                            eyeLoc.Z = (float)posZ;
                            //EditorCameraController.Camera. .Location = eyeLoc;
                            EditorCameraController.SetPosDir(ref eyeLoc, ref eyeDir);

                            //if (HostTerrain != null)
                            //{
                            //    HostTerrain.TravelTo(eyeLoc.X, eyeLoc.Z);
                            //}
                            CCore.Client.MainWorldInstance.TravelTo(eyeLoc.X, eyeLoc.Z);

                            UpdateCamera();
                        }
                        break;

                    case enEditMode.Brush:
                        {
                            int posX = (int)(pt.X / Image_TerrainLevels.Width * mLevelXCount);
                            int posY = (int)(pt.Y / Image_TerrainLevels.Height * mLevelZCount);
                            if (posX >= 0 && posY >= 0 && posX < mLevelXCount && posY < mLevelZCount)
                            {
                                var rect = new Int32Rect(posX, posY, 1, 1);
                                byte[] colorData = mAvailableLevelColor;
                                HostTerrain.AddLevel((uint)posX, (uint)(mLevelZCount - posY - 1));

                                mTerrainLevelsBitmap.WritePixels(rect, colorData, 4, 0);
                            }
                        }
                        break;

                    case enEditMode.Erase:
                        {
                            int posX = (int)(pt.X / Image_TerrainLevels.Width * mLevelXCount);
                            int posY = (int)(pt.Y / Image_TerrainLevels.Height * mLevelZCount);
                            if (posX >= 0 && posY >= 0 && posX < mLevelXCount && posY < mLevelZCount)
                            {
                                var rect = new Int32Rect(posX, posY, 1, 1);
                                byte[] colorData = mUnavailableLevelColor;
                                HostTerrain.DelLevel((uint)posX, (uint)(mLevelZCount - posY - 1));

                                mTerrainLevelsBitmap.WritePixels(rect, colorData, 4, 0);
                            }
                        }
                        break;
                }

            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                // 中键移动
                var pt = e.GetPosition(Image_TerrainLevels);
                var delta = pt - mMouseRightButtonDownPoint;
                if (delta.X != 0)
                {
                    var left = Canvas.GetLeft(Image_TerrainLevels);
                    Canvas.SetLeft(Image_TerrainLevels, left + delta.X);
                }
                if (delta.Y != 0)
                {
                    var top = Canvas.GetTop(Image_TerrainLevels);
                    Canvas.SetTop(Image_TerrainLevels, top + delta.Y);
                }

                UpdateAssistLines();
                UpdateCamera();
            }
        }

        private void Canvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void Canvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var pt = e.GetPosition(Image_TerrainLevels);
            var oldWidth = Image_TerrainLevels.Width;
            var oldHeight = Image_TerrainLevels.Height;

            var delta = 1 + e.Delta * 0.001;

            Image_TerrainLevels.Width *= delta;
            Image_TerrainLevels.Height *= delta;

            var xDelta = pt.X / oldWidth * (Image_TerrainLevels.Width - oldWidth);
            var yDelta = pt.Y / oldHeight * (Image_TerrainLevels.Height - oldHeight);

            if (xDelta != 0)
            {
                var left = Canvas.GetLeft(Image_TerrainLevels);
                Canvas.SetLeft(Image_TerrainLevels, left - xDelta);
            }
            if (yDelta != 0)
            {
                var top = Canvas.GetTop(Image_TerrainLevels);
                Canvas.SetTop(Image_TerrainLevels, top - yDelta);
            }

            UpdateAssistLines();
            UpdateCamera();
        }

        #endregion

        private void EditMode_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
                mEditMode = (enEditMode)System.Enum.Parse(typeof(enEditMode), rb.Tag.ToString());
        }

        private void Button_MapSnapshot_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // todo: 新建Enviroment，将对象渲染到此Enviroment上，然后输出
            //if (HostEnviroment == null || 
            //    EditorCameraController == null ||
            //    HostWorld == null ||
            //    HostRenderParam == null)
            //    return;

            //var eyePos = EditorCameraController.Camera.GetLocation();
            //var eyeDir = EditorCameraController.Camera.Direction;

            //var newCameraPos = SlimDX.Vector3.Zero;
            //newCameraPos.Y += 100;
            //var newCameraDir = SlimDX.Vector3.Zero;//-SlimDX.Vector3.UnitY + newCameraPos;
            //EditorCameraController.SetPosLookAtUp(ref newCameraPos, ref newCameraDir, ref SlimDX.Vector3.UnitZ);
            //EditorCameraController.Camera.MakeOrtho(1024, 1024, 1024, 1024);

            //EditorCameraController.Tick();

            ////HostWorld.Tick();
            ////HostWorld.Render2Enviroment(HostRenderParam);

            //HostEnviroment.Tick();
            //HostEnviroment.Render();

            //MidLayer.IViewPort viewPort = new MidLayer.IViewPort();
            //viewPort.X = 0;
            //viewPort.Y = 0;
            //viewPort.Width = 1024;
            //viewPort.Height = 1024;
            //HostEnviroment.View.SetViewPort(viewPort);
            //HostEnviroment.Save2File("D:\\XXX.jpg", MidLayer.enD3DXIMAGE_FILEFORMAT.D3DXIFF_JPG);
        }
    }
}
