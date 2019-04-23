using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace GamePublisher
{
    /// <summary>
    /// Interaction logic for MapResourceItem.xaml
    /// </summary>
    public partial class MapResourceItem : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        CCore.World.WorldInit mMapInit;        
        public CCore.World.WorldInit MapInit
        {
            get { return mMapInit; }
            set
            {
                mMapInit = value;
                if (mMapInit != null)
                {
                    MapId = mMapInit.WorldId;
                    MapName = mMapInit.WorldName;
                    MapSize = "Level: " + mMapInit.TerrainInfo.LevelX + "x" + mMapInit.TerrainInfo.LevelZ + "    " + mMapInit.TerrainInfo.MapSizeMeterX + "米 x " + mMapInit.TerrainInfo.MapSizeMeterZ + "米";
                }
            }
        }
        public string MapFolder = "";

        Guid mMapId = Guid.Empty;
        public Guid MapId
        {
            get { return mMapId; }
            set
            {
                mMapId = value;
                OnPropertyChanged("MapId");
            }
        }

        string mMapName = "";
        public string MapName
        {
            get { return mMapName; }
            set
            {
                mMapName = value;
                OnPropertyChanged("MapName");
            }
        }

        string mMapSize = "";
        public string MapSize
        {
            get { return mMapSize; }
            set
            {
                mMapSize = value;
                OnPropertyChanged("MapSize");
            }
        }

        ImageSource mImage = null;
        public ImageSource Image
        {
            get { return mImage; }
            set
            {
                mImage = value;
                OnPropertyChanged("Image");
            }
        }

        public MapResourceItem()
        {
            InitializeComponent();
        }
    }
}
