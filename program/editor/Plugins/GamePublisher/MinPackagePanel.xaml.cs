using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GamePublisher
{
    /// <summary>
    /// Interaction logic for MinPackagePanel.xaml
    /// </summary>
    public partial class MinPackagePanel : UserControl, INotifyPropertyChanged
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

        public class ResData : INotifyPropertyChanged, IEquatable<ResData>
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

            string mRelativeFileName = "";
            public string RelativeFileName
            {
                get { return mRelativeFileName; }
                set
                {
                    mRelativeFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(value, Program.SourceFolder);

                    FileName = Program.FullPackageFolder + mRelativeFileName;

                    OnPropertyChanged("RelativeFileName");
                }
            }

            string mFileName = "";
            public string FileName
            {
                get { return mFileName; }
                set
                {
                    mFileName = value;

                    OnPropertyChanged("FileName");
                }
            }

            Visibility mVisible = Visibility.Visible;
            public Visibility Visible
            {
                get { return mVisible; }
                set
                {
                    mVisible = value;
                    OnPropertyChanged("Visible");
                }
            }

            bool mIsFolder = false;
            public bool IsFolder
            {
                get { return mIsFolder; }
                set
                {
                    mIsFolder = value;

                    if (IsFolder)
                        Foreground = Brushes.LightBlue;
                    else
                        Foreground = Brushes.White;

                    OnPropertyChanged("IsFolder");
                }
            }

            Brush mForeground = null;
            public Brush Foreground
            {
                get { return mForeground; }
                set
                {
                    mForeground = value;
                    OnPropertyChanged("Foreground");
                }
            }

            public bool Equals(ResData other)
            {
                return (this.RelativeFileName == other.RelativeFileName);
            }

            public void Save(CSUtility.Support.XmlNode node)
            {
                if (node == null)
                    return;

                node.AddAttrib("RelativeFileName", RelativeFileName);
                node.AddAttrib("IsFolder", IsFolder.ToString());
            }

            public void Load(CSUtility.Support.XmlNode node)
            {
                if (node == null)
                    return;

                var att = node.FindAttrib("RelativeFileName");
                if (att != null)
                {
                    RelativeFileName = att.Value;
                }

                att = node.FindAttrib("IsFolder");
                if (att != null)
                {
                    IsFolder = System.Convert.ToBoolean(att.Value);
                }
            }
        }

        System.Collections.ObjectModel.ObservableCollection<ResData> mResFiles = new System.Collections.ObjectModel.ObservableCollection<ResData>();
        public System.Collections.ObjectModel.ObservableCollection<ResData> ResFiles
        {
            get { return mResFiles; }
            set
            {
                mResFiles = value;
                OnPropertyChanged("ResFiles");
            }
        }

        string mFilterString = "";
        public string FilterString
        {
            get { return mFilterString; }
            set
            {
                mFilterString = value;

                UpdateFilter(mFilterString);

                OnPropertyChanged("FilterString");
            }
        }

        Visibility mInfoVisible = Visibility.Visible;
        public Visibility InfoVisible
        {
            get { return mInfoVisible; }
            set
            {
                mInfoVisible = value;
                OnPropertyChanged("InfoVisible");
            }
        }

        bool mAutoSaveMinPackage = true;
        public bool AutoSaveMinPackage
        {
            get { return mAutoSaveMinPackage; }
            set
            {
                mAutoSaveMinPackage = value;
                OnPropertyChanged("AutoSaveMinPackage");
            }
        }

        public MinPackagePanel()
        {
            InitializeComponent();
        }

        string mConfigFile = AppDomain.CurrentDomain.BaseDirectory + "PublisherMinPackageConfig.xml";
        private void SaveConfig()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("MinPackageConfig", "");

            foreach (var data in ResFiles)
            {
                var node = xmlHolder.RootNode.AddNode("ResData", "", xmlHolder);
                data.Save(node);
            }

            CSUtility.Support.XmlHolder.SaveXML(mConfigFile, xmlHolder, true);
        }
        private void LoadConfig()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(mConfigFile);
            if (xmlHolder == null)
                return;

            ResFiles.Clear();
            foreach (var node in xmlHolder.RootNode.FindNodes("ResData"))
            {
                var data = new ResData();
                data.Load(node);

                ResFiles.Add(data);
            }

            if(ResFiles.Count > 0)
                InfoVisible = Visibility.Collapsed;
        }

        private void UpdateFilter(string filterStr)
        {
            if (string.IsNullOrEmpty(filterStr))
            {
                foreach (var uiData in ResFiles)
                {
                    uiData.Visible = Visibility.Visible;
                }
            }
            else
            {
                var lowerFilter = filterStr.ToLower();

                foreach (var uiData in ResFiles)
                {
                    var str = uiData.RelativeFileName.ToLower();
                    if (str.Contains(lowerFilter))
                        uiData.Visible = Visibility.Visible;
                    else
                        uiData.Visible = Visibility.Collapsed;
                }
            }
        }

        private void ListBox_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] datas = (string[])(e.Data.GetData(DataFormats.FileDrop));
            if (datas == null || datas.Length <= 0)
                return;

            foreach (var dataStr in datas)
            {
                if (System.IO.Directory.Exists(dataStr))
                {
                    // 添加文件夹
                    var data = new ResData()
                    {
                        RelativeFileName = dataStr,
                        IsFolder = true
                    };

                    if (!ResFiles.Contains(data))
                        ResFiles.Add(data);
                }
                else if (System.IO.File.Exists(dataStr))
                {
                    var data = new ResData()
                    {
                        RelativeFileName = dataStr,
                        IsFolder = false
                    };

                    if (!ResFiles.Contains(data))
                        ResFiles.Add(data);
                }
            }

            InfoVisible = Visibility.Collapsed;
        }

        private void ListBox_Res_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedItems = ListBox_Res.SelectedItems;
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                ResFiles.Remove((selectedItems[i]) as ResData);
            }
        }

        private void Button_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveConfig();
        }

        private void userControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveConfig();
        }

        private void userControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadConfig();
        }
    }
}
