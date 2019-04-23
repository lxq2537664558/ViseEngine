using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainEditor
{
    /// <summary>
    /// GameAssistWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameAssistWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        static GameAssistWindow smInstance = new GameAssistWindow();
        public static GameAssistWindow Instance
        {
            get { return smInstance; }
        }

        public WorldEditorOperation WorldEditorOperation;

        bool mSelectedMode = true;
        public bool SelectedMode
        {
            get { return mSelectedMode; }
            set
            {
                if (mSelectedMode == value)
                    return;

                mSelectedMode = value;
                if(mSelectedMode)
                    EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_Camera);
                OnPropertyChanged("SelectedMode");
            }
        }

        bool mMoveMode = false;
        public bool MoveMode
        {
            get { return mMoveMode; }
            set
            {
                if (mMoveMode == value)
                    return;

                mMoveMode = value;
                if(mMoveMode)
                    EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove);
                OnPropertyChanged("MoveMode");
            }
        }

        bool mRotMode = false;
        public bool RotMode
        {
            get { return mRotMode; }
            set
            {
                if (mRotMode == value)
                    return;

                mRotMode = value;
                if(mRotMode)
                    EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot);
                OnPropertyChanged("RotMode");
            }
        }

        bool mScaleMode = false;
        public bool ScaleMode
        {
            get { return mScaleMode; }
            set
            {
                if (mScaleMode == value)
                    return;

                mScaleMode = value;
                if(mScaleMode)
                    EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale);
                OnPropertyChanged("ScaleMode");
            }
        }

        bool mAxisMode_World = false;
        public bool AxisMode_World
        {
            get { return mAxisMode_World; }
            set
            {
                mAxisMode_World = value;
                if (mAxisMode_World)
                {
                    WorldEditorOperation.SetAxisMode(CCore.Support.V3DAxis.enAxisMode.World);
                    WorldEditorOperation.ScaleAxis(AxisScaleRate);
                }
                OnPropertyChanged("AxisMode_World");
            }
        }
        bool mAxisMode_Local = true;
        public bool AxisMode_Local
        {
            get { return mAxisMode_Local; }
            set
            {
                mAxisMode_Local = value;
                if (mAxisMode_Local)
                {
                    WorldEditorOperation.SetAxisMode(CCore.Support.V3DAxis.enAxisMode.Local);
                    WorldEditorOperation.ScaleAxis(AxisScaleRate);
                }
                OnPropertyChanged("AxisMode_Local");
            }
        }

        bool mAxisOperationType_MoveObject = true;
        public bool AxisOperationType_MoveObject
        {
            get { return mAxisOperationType_MoveObject; }
            set
            {
                mAxisOperationType_MoveObject = value;
                if (mAxisOperationType_MoveObject)
                    WorldEditorOperation.SetAxisOperationType(CCore.Support.V3DAxis.enOperationType.MoveObject);
                OnPropertyChanged("AxisOperationType_MoveObject");
            }
        }
        bool mAxisOperationType_MoveAxis = false;
        public bool AxisOperationType_MoveAxis
        {
            get { return mAxisOperationType_MoveAxis; }
            set
            {
                mAxisOperationType_MoveAxis = value;
                if (mAxisOperationType_MoveAxis)
                    WorldEditorOperation.SetAxisOperationType(CCore.Support.V3DAxis.enOperationType.MoveAxis);
                OnPropertyChanged("AxisOperationType_MoveAxis");
            }
        }

        float mAxisScaleRate = 1;
        public float AxisScaleRate
        {
            get { return mAxisScaleRate; }
            set
            {
                mAxisScaleRate = value;
                WorldEditorOperation.ScaleAxis(mAxisScaleRate);
                OnPropertyChanged("AxisScaleRate");
            }
        }

        bool mIsRotSnap = true;
        // 是否锁定旋转
        public bool IsRotSnap
        {
            get { return mIsRotSnap; }
            set
            {
                mIsRotSnap = value;
                OnPropertyChanged("IsRotSnap");
            }
        }

        float mRotSnapAngle = 5;
        // 角度锁定值
        public float RotSnapAngle
        {
            get { return mRotSnapAngle; }
            set
            {
                if (value < 0)
                    return;
                mRotSnapAngle = value;
                OnPropertyChanged("RotSnapAngle");
            }
        }

        bool mIsScaleSnap = false;
        // 是否锁定缩放
        public bool IsScaleSnap
        {
            get { return mIsScaleSnap; }
            set
            {
                mIsScaleSnap = value;
                OnPropertyChanged("IsScaleSnap");
            }
        }

        float mScaleSnap = 1;
        // 缩放锁定值
        public float ScaleSnap
        {
            get { return mScaleSnap; }
            set
            {
                if (value < 0)
                    return;
                mScaleSnap = value;
                OnPropertyChanged("ScaleSnap");
            }
        }

        UInt32 mNeighborSide = 1;
        public UInt32 NeighborSide
        {
            get { return mNeighborSide; }
            set
            {
                mNeighborSide = value;
                CCore.Client.MainWorldInstance.SetNeighborSide(mNeighborSide);
                OnPropertyChanged("NeighborSide");
            }
        }

        // 摄像机调整 -----------------------------------------------//
        float mCameraMoveSpdRate = 0.5f;
        public float CameraMoveSpdRate
        {
            get { return mCameraMoveSpdRate; }
            set
            {
                if (mCameraMoveSpdRate == value)
                    return;

                mCameraMoveSpdRate = value;
                EditorCommon.Program.CameraMoveSpdRate = mCameraMoveSpdRate;

                OnPropertyChanged("CameraMoveSpdRate");
            }
        }

        float mCameraZoomSpdRate = 0.05f;
        public float CameraZoomSpdRate
        {
            get { return mCameraZoomSpdRate; }
            set
            {
                if (mCameraZoomSpdRate == value)
                    return;

                mCameraZoomSpdRate = value;
                EditorCommon.Program.CameraZoomSpdRate = mCameraZoomSpdRate;

                OnPropertyChanged("CameraZoomSpdRate");
            }
        }

        private GameAssistWindow()
        {
            InitializeComponent();
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            EditorCommon.WorldEditMode.Instance.OnEditModeChanged += WorldEditMode_OnEditModeChanged;

            InitializeGameTypeShowMenu();
            InitializeSaveOptionData();
            CCore.Program.OnWorldLoaded += Program_OnWorldLoaded;
            CCore.Program.OnSetActorTypeShow += Program_OnSetActorTypeShow;

            EditorCommon.Program.CameraMoveSpdRate = CameraMoveSpdRate;
            EditorCommon.Program.CameraZoomSpdRate = CameraZoomSpdRate;
        }

        string mAbsFolder = "";
        private void Program_OnWorldLoaded(System.String strAbsFolder, string componentName, CCore.World.World world)
        {
            this.Dispatcher.Invoke(() =>
            {
                mAbsFolder = strAbsFolder;
                InitializeGameTypeShowMenu();
            });
        }

        private void WorldEditMode_OnEditModeChanged(EditorCommon.WorldEditMode.enEditMode oldEditMode, EditorCommon.WorldEditMode.enEditMode newEditMode)
        {
            switch(newEditMode)
            {
                case EditorCommon.WorldEditMode.enEditMode.Edit_Camera:
                    SelectedMode = true;
                    break;
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove:
                    MoveMode = true;
                    break;
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot:
                    RotMode = true;
                    break;
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale:
                    ScaleMode = true;
                    break;
            }
        }

        public CSUtility.Support.Point Offset = new CSUtility.Support.Point(0, 0);

        bool mNeedClose = false;
        public static void FinalInstance()
        {
            if(EditorCommon.WorldEditMode.Instance !=null)
                EditorCommon.WorldEditMode.Instance.OnEditModeChanged -= Instance.WorldEditMode_OnEditModeChanged;
            Instance.mNeedClose = true;
            Instance.Close();
            smInstance = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!mNeedClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        bool mMouseDown = false;
        Point mMouseDownOffset;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            mMouseDown = true;
            mMouseDownOffset = e.GetPosition(this);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if(mMouseDown)
            {
                var pt = e.GetPosition(this);
                var temp = pt - mMouseDownOffset;
                SetPosition(this.Left + temp.X, this.Top + temp.Y);

                if (EditorCommon.Program.GameForm != null)
                {
                    var heightOffset = EditorCommon.Program.GameForm.Height - EditorCommon.Program.GameForm.ClientSize.Height;
                    var widthOffset = EditorCommon.Program.GameForm.Width - EditorCommon.Program.GameForm.ClientSize.Width;
                    Offset.X = (int)(this.Left - EditorCommon.Program.GameForm.Left - widthOffset);
                    Offset.Y = (int)(this.Top - EditorCommon.Program.GameForm.Top - heightOffset);
                }
            }
        }

        public void SetPosition(double x, double y)
        {
            this.Left = x;
            this.Top = y;

            var screen = System.Windows.Forms.Screen.FromHandle(EditorCommon.Program.GameForm.Handle);
            if (this.Left < screen.Bounds.X)
                this.Left = screen.Bounds.X;
            if (this.Top < screen.Bounds.Y)
                this.Top = screen.Bounds.Y;

            if (this.Left + this.ActualWidth > screen.Bounds.Width)
                this.Left = screen.Bounds.Width - this.ActualWidth;
            if (this.Top + this.ActualHeight > screen.Bounds.Height)
                this.Top = screen.Bounds.Height - this.ActualHeight;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            mMouseDown = false;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
        }

        public void FocusSelectedObject()
        {
            if (WorldEditorOperation.REnviroment == null || WorldEditorOperation.FreeCameraController == null)
                return;

            EditorCommon.GameActorOperation.FocusActors(new List<CCore.World.Actor>(WorldEditorOperation.SelectedActors));
        }

        private void Button_Focus_Click(object sender, RoutedEventArgs e)
        {
            FocusSelectedObject();
        }
        private void Button_OpenEditor_Click(object sender, RoutedEventArgs e)
        {
            EditorLoading.Instance.DoInitializeEditorMainWindow();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            var mi = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(Popup_Snap.IsOpen)
                mi.Invoke(Popup_Snap, null);
            if (Popup_RotateSet.IsOpen)
                mi.Invoke(Popup_RotateSet, null);
            if (Popup_ScaleSet.IsOpen)
                mi.Invoke(Popup_ScaleSet, null);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;
            var exp = textBox.GetBindingExpression(TextBox.TextProperty);
            switch(e.Key)
            {
                case Key.Enter:
                    exp.UpdateSource();
                    break;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;
            var exp = textBox.GetBindingExpression(TextBox.TextProperty);
            exp.UpdateSource();
        }

        #region GameTypeShow

        // 鼠标滑过时高亮
        bool mHighLightWhenMouseOver = false;
        public bool HighLightWhenMouseOver
        {
            get { return mHighLightWhenMouseOver; }
            set
            {
                mHighLightWhenMouseOver = value;
                WorldEditorOperation.PreChooseEnable = value;
                OnPropertyChanged("HighLightWhenMouseOver");
            }
        }

        bool mShowMapGrid = false;
        public bool ShowMapGrid
        {
            get { return mShowMapGrid; }
            set
            {
                mShowMapGrid = value;
                WorldEditorOperation.GroupGridActor.Visible = value;
                OnPropertyChanged("ShowMapGrid");
            }
        }

        Dictionary<string, MenuItem> mGameTypeShowMenuDic = new Dictionary<string, MenuItem>();
        private void InitializeGameTypeShowMenu()
        {
            CSUtility.Support.ThreadSafeObservableCollection<CCore.Program.ActorTypeShowData> datas = new CSUtility.Support.ThreadSafeObservableCollection<CCore.Program.ActorTypeShowData>();
            CCore.Program.ActorTypeShowDic.TryGetValue(CCore.Engine.Instance.Client.MainWorld, out datas);
            ListBox_GameTypeShowItems.ItemsSource = datas;
        }

        private void Program_OnSetActorTypeShow(CCore.World.World world, string typeString, bool show)
        {
            if (world != CCore.Engine.Instance.Client.MainWorld)
                return;

            CSUtility.Support.ThreadSafeObservableCollection<CCore.Program.ActorTypeShowData> datas = new CSUtility.Support.ThreadSafeObservableCollection<CCore.Program.ActorTypeShowData>();
            CCore.Program.ActorTypeShowDic.TryGetValue(world, out datas);
            ListBox_GameTypeShowItems.ItemsSource = datas;
        }

        #endregion

        #region SaveOperation

        bool mSaveOptionShow = false;
        public bool SaveOptionShow
        {
            get { return mSaveOptionShow; }
            set
            {
                mSaveOptionShow = value;
                OnPropertyChanged("SaveOptionShow");
            }
        }

        class SaveOptionData : INotifyPropertyChanged
        {
            #region INotifyPropertyChangedMembers
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion

            string mName;
            public string Name
            {
                get { return mName; }
                set
                {
                    mName = value;
                    OnPropertyChanged("Name");
                }
            }

            bool mSave;
            public bool Save
            {
                get { return mSave; }
                set
                {
                    mSave = value;
                    OnPropertyChanged("Save");
                }
            }

            public CCore.World.World.WorldComponentData WorldComponent;
        }
        ObservableCollection<SaveOptionData> mSaveDatas = new ObservableCollection<SaveOptionData>();

        void InitializeSaveOptionData()
        {
            foreach (var comp in CCore.World.World.WorldComponents)
            {
                var data = new SaveOptionData()
                {
                    Name = comp.Key,
                    Save = true,
                    WorldComponent = comp.Value,
                };
                mSaveDatas.Add(data);
            }

            ListBox_WorldSaveItems.ItemsSource = mSaveDatas;
        }

        private void Button_SaveWorld_Click(object sender, RoutedEventArgs e)
        {
            foreach(var data in mSaveDatas)
            {
                if(data.Save)
                {
                    CCore.Client.MainWorldInstance.SaveWorld("", true, data.Name);
                    switch(data.Name)
                    {
                        case "场景":
                        case "地形":
                            {
                                // 保存缩略图
                                var snapShotName = mAbsFolder + "_Snapshot.png";
                                EditorCommon.Program.GameREnviroment.Save2File(snapShotName, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);
                                EditorCommon.PluginAssist.PluginOperation.RefreshBrowserSnapshot(mAbsFolder + ".rinfo", false);
                            }
                            break;
                    }
                }
            }

            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"文件{mAbsFolder}使用版本控制删除失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                        {
                            if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"文件{mAbsFolder}使用版本控制删除失败!");
                            }
                        }, mAbsFolder, $"AutoCommit 保存地图{CCore.Client.MainWorldInstance.WorldInit.WorldName}");

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshot) =>
                        {
                            if (resultSnapshot.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"文件{mAbsFolder}_Snapshot.png 使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotCommit) =>
                                {
                                    if (resultSnapshotCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"文件{mAbsFolder}_Snapshot.png 使用版本控制删除失败!");
                                    }
                                }, mAbsFolder + "_Snapshot.png", $"AutoCommit 保存地图缩略图{CCore.Client.MainWorldInstance.WorldInit.WorldName}");
                            }
                        }, mAbsFolder + "_Snapshot.png");
                    }
                }, mAbsFolder);


            }
        }

        #endregion

        #region debug

        bool mWireFrame = false;
        public bool WireFrame
        {
            get { return mWireFrame; }
            set
            {
                mWireFrame = value;
                if(EditorCommon.Program.GameREnviroment != null && mWireFrame)
                    EditorCommon.Program.GameREnviroment.RenderMode = CCore.Graphics.enRenderMode.RM_Wireframe;
                OnPropertyChanged("WireFrame");
            }
        }

        bool mAlbedo = false;
        public bool Albedo
        {
            get { return mAlbedo; }
            set
            {
                mAlbedo = value;
                if(EditorCommon.Program.GameREnviroment != null && mAlbedo)
                    EditorCommon.Program.GameREnviroment.RenderMode = CCore.Graphics.enRenderMode.RM_Albedo;
                OnPropertyChanged("Albedo");
            }
        }

        bool mLighting = false;
        public bool Lighting
        {
            get { return mLighting; }
            set
            {
                mLighting = value;
                if(EditorCommon.Program.GameREnviroment != null && mLighting)
                    EditorCommon.Program.GameREnviroment.RenderMode = CCore.Graphics.enRenderMode.RM_Lighting;
                OnPropertyChanged("Lighting");
            }
        }

        bool mShading = true;
        public bool Shading
        {
            get { return mShading; }
            set
            {
                mShading = value;
                if (EditorCommon.Program.GameREnviroment != null && mShading)
                    EditorCommon.Program.GameREnviroment.RenderMode = CCore.Graphics.enRenderMode.RM_Shading;
                OnPropertyChanged("Shading");
            }
        }

        bool mLockTerrainCulling = false;
        public bool LockTerrainCulling
        {
            get { return mLockTerrainCulling; }
            set
            {
                mLockTerrainCulling = value;
                if(CCore.Client.MainWorldInstance.Terrain != null)
                    CCore.Client.MainWorldInstance.Terrain.LockCulling = mLockTerrainCulling;
                OnPropertyChanged("LockTerrainCulling");
            }
        }

        bool mLockSceneCulling = false;
        public bool LockSceneCulling
        {
            get { return mLockSceneCulling; }
            set
            {
                mLockSceneCulling = value;
                if (CCore.Client.MainWorldInstance.SceneGraph != null)
                    CCore.Client.MainWorldInstance.SceneGraph.SetLockCulling(mLockSceneCulling);
                OnPropertyChanged("LockSceneCulling");
            }
        }

        bool mLockShadowSceneCulling = false;
        public bool LockShadowSceneCulling
        {
            get { return mLockShadowSceneCulling; }
            set
            {
                mLockShadowSceneCulling = value;
                if (CCore.Client.MainWorldInstance.SceneGraph != null)
                    CCore.Client.MainWorldInstance.SceneGraph.LockShadowCommit = mLockShadowSceneCulling;
                OnPropertyChanged("LockShadowSceneCulling");
            }
        }

        bool mShowCullingBoundingBox = false;
        public bool ShowCullingBoundingBox
        {
            get { return mShowCullingBoundingBox; }
            set
            {
                mShowCullingBoundingBox = value;
                CCore.World.World.ShowTileInheritBoundingBox = mShowCullingBoundingBox;
                OnPropertyChanged("ShowCullingBoundingBox");
            }
        }

        #endregion

        private void RadioButton_Rot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Popup_RotateSet.IsOpen = true;
            e.Handled = true;
        }
        private void Button_RotSetClose_Click(object sender, RoutedEventArgs e)
        {
            Popup_RotateSet.IsOpen = false;
        }

        private void RadioButton_Scale_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Popup_ScaleSet.IsOpen = true;
            e.Handled = true;
        }

        private void Button_ScaleSetClose_Click(object sender, RoutedEventArgs e)
        {
            Popup_ScaleSet.IsOpen = false;
        }

        #region Snap

        private void RadioButton_Snap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Popup_Snap.IsOpen = true;
            e.Handled = true;
        }
        private void Button_CloseSnapSet_Click(object sender, RoutedEventArgs e)
        {
            Popup_Snap.IsOpen = false;
        }
        
        float mSnapRange = 1.0f;
        public float SnapRange
        {
            get { return mSnapRange; }
            set
            {
                mSnapRange = value;
                OnPropertyChanged("SnapRange");
            }
        }

        bool mSnapOrigin = false;
        public bool SnapOrigin
        {
            get { return mSnapOrigin; }
            set
            {
                mSnapOrigin = value;
                OnPropertyChanged("SnapOrigin");
            }
        }

        bool mSnapVertex = false;
        public bool SnapVertex
        {
            get { return mSnapVertex; }
            set
            {
                mSnapVertex = value;
                OnPropertyChanged("SnapVertex");
            }
        }

        bool mSnapFace = false;
        public bool SnapFace
        {
            get { return mSnapFace; }
            set
            {
                mSnapFace = value;
                OnPropertyChanged("SnapFace");
            }
        }

        bool mSnapTile = false;
        public bool SnapTile
        {
            get { return mSnapTile; }
            set
            {
                mSnapTile = value;
                OnPropertyChanged("SnapTile");
            }
        }

        float mTileDensity = 1.0f;
        public float TileDensity
        {
            get { return mTileDensity; }
            set
            {
                mTileDensity = value;
                OnPropertyChanged("TileDensity");
            }
        }

        Point mStartDragPoint;
        bool mPopupMouseDown = false;
        private void Popup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            mStartDragPoint = e.GetPosition(element);
            Mouse.Capture(element);
            mPopupMouseDown = true;
            e.Handled = true;
        }

        private void Popup_MouseUp(object sender, MouseEventArgs e)
        {
            Mouse.Capture(null);
            mPopupMouseDown = false;
        }

        private void Popup_MouseMove(object sender, MouseEventArgs e)
        {
            var element = sender as Popup;
            if (mPopupMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                var pt = e.GetPosition(element);
                element.HorizontalOffset += pt.X - mStartDragPoint.X;
                element.VerticalOffset += pt.Y - mStartDragPoint.Y;
                mStartDragPoint = pt;
            }
            e.Handled = true;
        }

        #endregion
    }
}
