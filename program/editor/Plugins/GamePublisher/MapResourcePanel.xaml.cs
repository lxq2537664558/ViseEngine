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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GamePublisher
{
    /// <summary>
    /// Interaction logic for MapResourcePanel.xaml
    /// </summary>
    public partial class MapResourcePanel : UserControl, INotifyPropertyChanged
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

        ObservableCollection<MapResourceItem> mHasItems = new ObservableCollection<MapResourceItem>();
        ObservableCollection<MapResourceItem> mReleaseItems = new ObservableCollection<MapResourceItem>();
        public ObservableCollection<MapResourceItem> ReleaseItems
        {
            get { return mReleaseItems; }
        }

        string mContainMapCountStr = "0个";
        public string ContainMapCountStr
        {
            get { return mContainMapCountStr; }
            set
            {
                mContainMapCountStr = value;
                OnPropertyChanged("ContainMapCountStr");
            }
        }

        string mReleaseMapCountStr = "0个";
        public string ReleaseMapCountStr
        {
            get { return mReleaseMapCountStr; }
            set
            {
                mReleaseMapCountStr = value;
                OnPropertyChanged("ReleaseMapCountStr");
            }
        }

        public MapResourcePanel()
        {
            InitializeComponent();

            ListBox_Contain.ItemsSource = mHasItems;
            ListBox_Release.ItemsSource = mReleaseItems;

            InitializeMaps();
        }

        private void UpdateContainCountShow()
        {
            ContainMapCountStr = mHasItems.Count + "个";
        }
        private void UpdateReleaseCountShow()
        {
            ReleaseMapCountStr = mReleaseItems.Count + "个";
        }

        private void InitializeMaps()
        {
            mHasItems.Clear();
            mReleaseItems.Clear();
            
            List<Guid> removedList = new List<Guid>();
            foreach(var i in CSUtility.Map.MapManager.Instance.MapFileDic)
            {
                var dir = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(i.Value);
                if (!System.IO.Directory.Exists(dir))
                {
                    removedList.Add(i.Key);
                    continue;
                }

                var mapFileName = dir + "\\Config.map";
                var csInit = new CCore.World.WorldInit();
                if (CSUtility.Support.IConfigurator.FillProperty(csInit, mapFileName))
                {
                    var item = new MapResourceItem()
                    {
                        MapInit = csInit,
                        MapFolder = dir,
                    };

                    item.Image = EditorCommon.ImageInit.GetImage(dir + "_SnapShot.png");

                    mHasItems.Add(item);
                }
            }

            foreach(var i in removedList)
            {
                CSUtility.Map.MapManager.Instance.RemoveMapFile(i);
            }                

            LoadMapResConfig();

            UpdateContainCountShow();
            UpdateReleaseCountShow();
        }

        private void Button_AddRelease(object sender, System.Windows.RoutedEventArgs e)
        {
            var items = ListBox_Contain.SelectedItems;
            for (int i = items.Count - 1; i >= 0; i-- )
            {
                mReleaseItems.Add(items[i] as MapResourceItem);
                mHasItems.Remove(items[i] as MapResourceItem);
            }

            UpdateContainCountShow();
            UpdateReleaseCountShow();

        }

        private void Button_RemoveRelease(object sender, System.Windows.RoutedEventArgs e)
        {
            var items = ListBox_Release.SelectedItems;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                mHasItems.Add(items[i] as MapResourceItem);
                mReleaseItems.Remove(items[i] as MapResourceItem);
            }

            UpdateContainCountShow();
            UpdateReleaseCountShow();

        }

        string mConfigFile = AppDomain.CurrentDomain.BaseDirectory + "PublisherMapResConfig.dat";
        public void LoadMapResConfig()
        {
            if (!System.IO.File.Exists(mConfigFile))
                return;

            var xmlholder = CSUtility.Support.XmlHolder.LoadXML(mConfigFile);
            if (xmlholder == null || xmlholder.RootNode == null)
                return;

            foreach (var relItem in mReleaseItems)
            {
                if (!mHasItems.Contains(relItem))
                    mHasItems.Add(relItem);
            }
            mReleaseItems.Clear();

            foreach (var node in xmlholder.RootNode.FindNodes("Map"))
            {
                var attId = node.FindAttrib("Id");
                if(attId == null)
                    continue;

                var mapId = CSUtility.Support.IHelper.GuidTryParse(attId.Value);
                if(mapId == Guid.Empty)
                    continue;

                foreach (var item in mHasItems)
                {
                    if (item.MapId == mapId)
                    {
                        mReleaseItems.Add(item);
                        mHasItems.Remove(item);
                        break;
                    }
                }
            }

        }

        public void SaveMapResConfig()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("MapResConfig", "");

            foreach (var item in mReleaseItems)
            {
                var node = xmlHolder.RootNode.AddNode("Map", "", xmlHolder);
                node.AddAttrib("Id", item.MapId.ToString());
                node.AddAttrib("Dir", item.MapFolder);
            }

            CSUtility.Support.XmlHolder.SaveXML(mConfigFile, xmlHolder, true);
        }
    }
}
