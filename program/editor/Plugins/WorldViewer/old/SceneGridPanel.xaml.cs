using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WorldViewer
{
    /// <summary>
    /// Interaction logic for SceneGridPanel.xaml
    /// </summary>
    public partial class SceneGridPanel : UserControl, INotifyPropertyChanged
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


        public CCore.Support.GroupGrid GroupGrid
        {
            get
            {
                ////////////return CCore.Client.MainWorldInstance.GroupGrid;
                return null;
            }
        }

        static SceneGridPanel smInstance = null;
        public static SceneGridPanel Instance
        {
            get
            {
                if (smInstance == null)
                    smInstance = new SceneGridPanel();
                return smInstance;
            }
        }

        //bool mShowGrid = false;
        public bool ShowGrid
        {
            get { return GroupGrid.Visible; }
            set
            {
                if (GroupGrid != null)
                    GroupGrid.Visible = value;

                OnPropertyChanged("ShowGrid");
            }
        }

        //float mMeterPerGrid = 1.0f;
        public float MeterPreGrid
        {
            get { return GroupGrid.DeltaX; }
            set
            {
                if (GroupGrid != null)
                    GroupGrid.DeltaX = GroupGrid.DeltaZ = value;

                OnPropertyChanged("MeterPreGrid");
            }
        }

        //int mMainLineInterval = 32;
        public int MainLineInterval
        {
            get { return GroupGrid.Interval; }
            set
            {
                if (GroupGrid != null)
                {
                    GroupGrid.Interval = value;
                }

                OnPropertyChanged("MainLineInterval");
            }
        }

        ////float mSceneXLength = 512;
        //public float SceneXLength
        //{
        //    get { return GroupGrid.XLength; }
        //    set
        //    {
        //        if (GroupGrid != null)
        //            GroupGrid.XLength = value;
        //    }
        //}

        ////float mSceneZLength = 512;
        //public float SceneZLength
        //{
        //    get { return GroupGrid.ZLength; }
        //    set
        //    {
        //        if (GroupGrid != null)
        //            GroupGrid.ZLength = value;
        //    }
        //}

        //float mLocationX = 0;
        public float LocationX
        {
            get { return GroupGrid.LocX; }
            set
            {
                if (GroupGrid != null)
                    GroupGrid.LocX = value;

                OnPropertyChanged("LocationX");
            }
        }

        //float mLocationY = 0;
        public float LocationY
        {
            get { return GroupGrid.LocY; }
            set
            {
                if (GroupGrid != null)
                    GroupGrid.LocY = value;

                OnPropertyChanged("LocationY");
            }
        }

        //float mLocationZ = 0;
        public float LocationZ
        {
            get { return GroupGrid.LocZ; }
            set
            {
                if (GroupGrid != null)
                    GroupGrid.LocZ = value;

                OnPropertyChanged("LocationZ");
            }
        }

        //float mXLength = 512;
        public float XLength
        {
            get { return GroupGrid.XLength; }
            set
            {
                if (GroupGrid != null)
                    GroupGrid.XLength = value;

                OnPropertyChanged("XLength");
            }
        }

        //float mZLength = 512;
        public float ZLength
        {
            get { return GroupGrid.ZLength; }
            set
            {
                if (GroupGrid != null)
                    GroupGrid.ZLength = value;

                OnPropertyChanged("ZLength");
            }
        }

        public SceneGridPanel()
        {
            InitializeComponent();
        }

        private void Button_ResetToScene_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        public void RefreshValue()
        {
            OnPropertyChanged("ShowGrid");
            OnPropertyChanged("MeterPreGrid");
            OnPropertyChanged("MainLineInterval");
            OnPropertyChanged("LocationX");
            OnPropertyChanged("LocationY");
            OnPropertyChanged("LocationZ");
            OnPropertyChanged("XLength");
            OnPropertyChanged("ZLength");
        }

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshValue();
        }
    }
}
