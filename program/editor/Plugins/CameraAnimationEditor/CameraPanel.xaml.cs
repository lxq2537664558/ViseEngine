using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CameraAnimationEditor
{
    /// <summary>
    /// Interaction logic for CameraPanel.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin("CameraSet", PluginType = "CameraSet")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/CameraSet")]
    [Guid("25845475-F526-4DA8-9544-9079DA220820")]
    [PartCreationPolicy(CreationPolicy.Any)]
    public partial class CameraPanel : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public string PluginName
        {
            get { return "CameraSet"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "用于设置摄像机数据",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {
        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {
            if (!mInitialized)
            {
                //if (ChiefRole == null)
                //    return;

                PG_Current.Instance = mCurrentCameraParam;
                //PG_Nearest.Instance = ChiefRole.RoleData.NearestCamera;
                //PG_Farthest.Instance = ChiefRole.RoleData.FarthestCamera;
                //PG_Default.Instance = ChiefRole.RoleData.DefaultCamera;

                mInitialized = true;
            }
        }

        ///////////////////////////////////////////////////////

        class CurrentCameraParam : INotifyPropertyChanged
        {
            #region INotifyPropertyChangedMembers
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion

            [System.ComponentModel.Category("摄像机")]
            [System.ComponentModel.DisplayName("FOV")]
            [CSUtility.Editor.Editor_ValueWithRange(5.0, 180.0)]
            public double CameraFOV
            {
                get
                {
                    //if(ChiefRole == null)
                    //    return 30;
                    //return ChiefRole.RoleData.FOV;
                    return 35;
                }
                set
                {
                    //if (ChiefRole != null)
                    //    ChiefRole.RoleData.FOV = value;

                    OnPropertyChanged("CameraFOV");
                }
            }

            [System.ComponentModel.Category("摄像机")]
            [System.ComponentModel.DisplayName("距离")]
            [CSUtility.Editor.Editor_ValueWithRange(1.0, 100.0)]
            public double Camera2RoleDistance
            {
                get
                {
                    return 0.0;
                    //if (ChiefRole == null || CCore.Camera.CameraController.CurrentCameraController == null)
                    //    return 0.0;

                    //var roleLoc = ChiefRole.Placement.GetLocation();
                    //roleLoc.Y += ChiefRole.RoleTemplate.CameraPointHeight;
                    //var eyeLoc = CCore.Camera.CameraController.CurrentCameraController.Camera.Location;
                    //return (eyeLoc - roleLoc).Length();

                    //if (ChiefRole == null)
                    //    return 0.0;
                    //return ChiefRole.RoleData.Camera2RoleDistance;
                }
                set
                {
                    //if (ChiefRole != null)
                    //    ChiefRole.RoleData.Camera2RoleDistance = value;

                    OnPropertyChanged("Camera2RoleDistance");
                }
            }

            [System.ComponentModel.Category("摄像机")]
            [System.ComponentModel.DisplayName("X朝向")]
            [CSUtility.Editor.Editor_ValueWithRange(-1.0, 1.0)]
            public double CameraDirectionX
            {
                get
                {
                    if (CCore.Camera.CameraController.CurrentCameraController == null)
                        return 0;

                    return CCore.Camera.CameraController.CurrentCameraController.Camera.Direction.X;
                    //if (ChiefRole == null)
                    //    return 0;
                    //return ChiefRole.RoleData.CameraDirectionX;
                }
                set
                {
                    //if (ChiefRole != null)
                    //    ChiefRole.RoleData.CameraDirectionX = value;

                    OnPropertyChanged("CameraDirectionX");
                }
            }

            [System.ComponentModel.Category("摄像机")]
            [System.ComponentModel.DisplayName("Y朝向")]
            [CSUtility.Editor.Editor_ValueWithRange(-1.0, 1.0)]
            public double CameraDirectionY
            {
                get
                {
                    if (CCore.Camera.CameraController.CurrentCameraController == null)
                        return 0;

                    return CCore.Camera.CameraController.CurrentCameraController.Camera.Direction.Y;
                    //if (ChiefRole == null)
                    //    return 0;
                    //return ChiefRole.RoleData.CameraDirectionY;
                }
                set
                {
                    //if (ChiefRole != null)
                    //    ChiefRole.RoleData.CameraDirectionY = value;

                    OnPropertyChanged("CameraDirectionY");
                }
            }

            [System.ComponentModel.Category("摄像机")]
            [System.ComponentModel.DisplayName("Z朝向")]
            [CSUtility.Editor.Editor_ValueWithRange(-1.0, 1.0)]
            public double CameraDirectionZ
            {
                get
                {
                    if (CCore.Camera.CameraController.CurrentCameraController == null)
                        return 0;

                    return CCore.Camera.CameraController.CurrentCameraController.Camera.Direction.Z;
                    //if (ChiefRole == null)
                    //    return 0;
                    //return ChiefRole.RoleData.CameraDirectionZ;
                }
                set
                {
                    //if (ChiefRole != null)
                    //    ChiefRole.RoleData.CameraDirectionZ = value;

                    OnPropertyChanged("CameraDirectionZ");
                }
            }
        }

        CurrentCameraParam mCurrentCameraParam = new CurrentCameraParam();

        public CameraPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        bool mInitialized = false;
        //void EditorCommon.ITickInfo.Tick()
        //{
        //    if (!mInitialized)
        //    {
        //        if (ChiefRole == null)
        //            return;

        //        PG_Current.Instance = mCurrentCameraParam;
        //        PG_Nearest.Instance = ChiefRole.RoleData.NearestCamera;
        //        PG_Farthest.Instance = ChiefRole.RoleData.FarthestCamera;
        //        PG_Default.Instance = ChiefRole.RoleData.DefaultCamera;

        //        mInitialized = true;
        //        EditorCommon.TickInfo.Instance.RemoveTickInfo(this);
        //    }
        //}

        private void Button_SetNearestCam_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //ChiefRole.RoleData.NearestCamera.CameraFOV = mCurrentCameraParam.CameraFOV;
            //ChiefRole.RoleData.NearestCamera.Camera2RoleDistance = mCurrentCameraParam.Camera2RoleDistance;
            //ChiefRole.RoleData.NearestCamera.CameraDirectionX = mCurrentCameraParam.CameraDirectionX;
            //ChiefRole.RoleData.NearestCamera.CameraDirectionY = mCurrentCameraParam.CameraDirectionY;
            //ChiefRole.RoleData.NearestCamera.CameraDirectionZ = mCurrentCameraParam.CameraDirectionZ;

        }
        private void Button_PreviewNearestCam_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //mCurrentCameraParam.CameraFOV            = ChiefRole.RoleData.NearestCamera.CameraFOV;            
            //mCurrentCameraParam.Camera2RoleDistance  = ChiefRole.RoleData.NearestCamera.Camera2RoleDistance;  
            //mCurrentCameraParam.CameraDirectionX     = ChiefRole.RoleData.NearestCamera.CameraDirectionX;     
            //mCurrentCameraParam.CameraDirectionY     = ChiefRole.RoleData.NearestCamera.CameraDirectionY;
            //mCurrentCameraParam.CameraDirectionZ = ChiefRole.RoleData.NearestCamera.CameraDirectionZ;     

        }

        private void Button_SetFarthestCam_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //ChiefRole.RoleData.FarthestCamera.CameraFOV = mCurrentCameraParam.CameraFOV;
            //ChiefRole.RoleData.FarthestCamera.Camera2RoleDistance = mCurrentCameraParam.Camera2RoleDistance;
            //ChiefRole.RoleData.FarthestCamera.CameraDirectionX = mCurrentCameraParam.CameraDirectionX;
            //ChiefRole.RoleData.FarthestCamera.CameraDirectionY = mCurrentCameraParam.CameraDirectionY;
            //ChiefRole.RoleData.FarthestCamera.CameraDirectionZ = mCurrentCameraParam.CameraDirectionZ;

        }
        private void Button_PreviewFarthestCam_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //mCurrentCameraParam.CameraFOV             = ChiefRole.RoleData.FarthestCamera.CameraFOV;          
            //mCurrentCameraParam.Camera2RoleDistance   = ChiefRole.RoleData.FarthestCamera.Camera2RoleDistance;
            //mCurrentCameraParam.CameraDirectionX      = ChiefRole.RoleData.FarthestCamera.CameraDirectionX;   
            //mCurrentCameraParam.CameraDirectionY      = ChiefRole.RoleData.FarthestCamera.CameraDirectionY;
            //mCurrentCameraParam.CameraDirectionZ = ChiefRole.RoleData.FarthestCamera.CameraDirectionZ;   

        }

        private void Button_SetDefaultCam_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //ChiefRole.RoleData.DefaultCamera.CameraFOV = mCurrentCameraParam.CameraFOV;
            //ChiefRole.RoleData.DefaultCamera.Camera2RoleDistance = mCurrentCameraParam.Camera2RoleDistance;
            //ChiefRole.RoleData.DefaultCamera.CameraDirectionX = mCurrentCameraParam.CameraDirectionX;
            //ChiefRole.RoleData.DefaultCamera.CameraDirectionY = mCurrentCameraParam.CameraDirectionY;
            //ChiefRole.RoleData.DefaultCamera.CameraDirectionZ = mCurrentCameraParam.CameraDirectionZ;

        }
        private void Button_PreviewDefaultCam_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //mCurrentCameraParam.CameraFOV             = ChiefRole.RoleData.DefaultCamera.CameraFOV;            
            //mCurrentCameraParam.Camera2RoleDistance   = ChiefRole.RoleData.DefaultCamera.Camera2RoleDistance; 
            //mCurrentCameraParam.CameraDirectionX      = ChiefRole.RoleData.DefaultCamera.CameraDirectionX;    
            //mCurrentCameraParam.CameraDirectionY      = ChiefRole.RoleData.DefaultCamera.CameraDirectionY;
            //mCurrentCameraParam.CameraDirectionZ = ChiefRole.RoleData.DefaultCamera.CameraDirectionZ;    
           // 在此处添加事件处理程序实现。
        }

        private void Button_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
#warning 实现
            //var scene = CCore.Client.MainWorldInstance as FrameSet.Scene.CellScene;
            //if(scene != null)
            //    scene.SaveCamera("");
        }
        private void Button_Reload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
#warning 实现
            //var scene = CCore.Client.MainWorldInstance as FrameSet.Scene.CellScene;
            //if (scene != null)
            //{
            //    scene.LoadCamera("");
            //    PG_Current.Instance = mCurrentCameraParam;
            //    PG_Nearest.Instance = ChiefRole.RoleData.NearestCamera;
            //    PG_Farthest.Instance = ChiefRole.RoleData.FarthestCamera;
            //    PG_Default.Instance = ChiefRole.RoleData.DefaultCamera;
            //}
        }


    }
}
