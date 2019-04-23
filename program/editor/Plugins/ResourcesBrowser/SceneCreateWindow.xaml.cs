using System.Windows;
using System.ComponentModel;
using System;
using System.Windows.Controls;

namespace ResourcesBrowser
{
    /// <summary>
    /// Interaction logic for SceneCreateWindow.xaml
    /// </summary>
    public partial class SceneCreateWindow : DockControl.Controls.DockAbleWindowBase, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
        // 场景文件，包含所有的Actor和场景管理器属性
        string mSceneFileName = "Scene.dat";
        public string SceneFileName
        {
            get { return mSceneFileName; }
            set
            {
                mSceneFileName = value;

                OnPropertyChanged("SceneManagerFileName");
            }
        }

        string mPostProcessFileName = "PostProcess.dat";
        public string PostProcessFileName
        {
            get { return mPostProcessFileName; }
            set
            {
                mPostProcessFileName = value;

                OnPropertyChanged("PostProcessFileName");
            }
        }

        string mTerrainFileName = "Terrain/Terrain.terrain";
        public string TerrainFileName
        {
            get { return mTerrainFileName; }
            set
            {
                mTerrainFileName = value;

                OnPropertyChanged("TerrainFileName");
            }
        }

        string mNavigationFileName = "Navigation/Navigation.nav";
        public string NavigationFileName
        {
            get { return mNavigationFileName; }
            set
            {
                mNavigationFileName = value;

                OnPropertyChanged("NavigationFileName");
            }
        }

        CCore.World.WorldInit mWorldInit;
        public CCore.World.WorldInit WorldInit
        {
            get { return mWorldInit; }
            protected set
            {
                mWorldInit = value;
                if(mWorldInit != null)
                {
                    mWorldInit.WorldId = mMapID;
                    
                    ProGrid.Instance = mWorldInit;
                }
            }
        }

        //UInt32 mSceneWidth = 512;
        //public UInt32 SceneWidth
        //{
        //    get { return mSceneWidth; }
        //    set
        //    {
        //        mSceneWidth = value;
        //        OnPropertyChanged("SceneWidth");
        //    }
        //}

        //UInt32 mSceneHeight = 512;
        //public UInt32 SceneHeight
        //{
        //    get { return mSceneHeight; }
        //    set
        //    {
        //        mSceneHeight = value;
        //        OnPropertyChanged("SceneHeight");
        //    }
        //}


        byte mWorldInitType;
        public byte WorldInitType
        {
            get { return mWorldInitType; }
            set
            {
                mWorldInitType = value;

                WorldInit = CCore.World.WorldInitFactory.Instance.CreateWorldInit((byte)(mWorldInitType));
            }
        }

        Guid mMapID = Guid.NewGuid();
        public Guid MapID
        {
            get { return mMapID; }
        }
        
        public string mMapDir;

        public SceneCreateWindow()
        {
            InitializeComponent();

            LayoutManaged = false;

            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);

            ////////ComboBox显示注释
            //////var enumType = typeof(Client.World.enWorldInitType);
            //////foreach(var typeValue in System.Enum.GetValues(enumType))
            //////{
            //////    var comboxItem = new ComboBoxItem()
            //////    {
            //////        Content = typeValue.ToString(),
            //////        Tag = typeValue
            //////    };

            //////    var fi = enumType.GetField(typeValue.ToString());
            //////    var atts = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            //////    if (atts.Length > 0)
            //////        comboxItem.ToolTip = atts[0];

            //////    ComboBox_WorldType.Items.Add(comboxItem);
            //////}

            WorldInitType = 0;
            mWorldInit.WorldId = mMapID;
            ProGrid.Instance = mWorldInit;
        }
        
        private void Button_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = true;

            this.Close();
        }

        private void Button_Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;

            this.Close();
        }
    }
}
