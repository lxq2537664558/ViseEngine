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
using System.Collections.ObjectModel;

namespace GamePublisher
{
    /// <summary>
    /// Interaction logic for UIPanel.xaml
    /// </summary>
    public partial class UIPanel : UserControl, INotifyPropertyChanged
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

        public class UIData : INotifyPropertyChanged, IEquatable<UIData>
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

            string mFileName = "";
            public string FileName
            {
                get { return mFileName; }
                set
                {
                    mFileName = value;

                    Snapshot = EditorCommon.ImageInit.GetImage(mFileName + "_Snapshot.png") as BitmapSource;
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

            ImageSource mSnapshot = null;
            public ImageSource Snapshot
            {
                get { return mSnapshot; }
                set
                {
                    mSnapshot = value;
                    OnPropertyChanged("Snapshot");
                }
            }

            public bool Equals(UIData other)
            {
                return (this.FileName == other.FileName);
            }
        }

        ObservableCollection<UIData> mUIFiles = new ObservableCollection<UIData>();
        public ObservableCollection<UIData> UIFiles
        {
            get { return mUIFiles; }
            set
            {
                mUIFiles = value;
                OnPropertyChanged("UIFiles");
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

        public UIPanel()
        {
            InitializeComponent();

            GetAllUIFiles();
            //LoadResConfig();
        }

        private void UpdateFilter(string filterStr)
        {
            if (string.IsNullOrEmpty(filterStr))
            {
                foreach (var uiData in UIFiles)
                {
                    uiData.Visible = Visibility.Visible;
                }
            }
            else
            {
                var lowerFilter = filterStr.ToLower();

                foreach (var uiData in UIFiles)
                {
                    var str = uiData.FileName.ToLower();
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
                    AddFilesInFolder(dataStr);
                }
                else if (System.IO.File.Exists(dataStr))
                {
                    if (CSUtility.Support.IFileManager.Instance.GetFileExtension(dataStr) == "xml")
                    {
                        var newStr = dataStr.Replace("\\", "/");
                        var uiData = new UIData()
                        {
                            FileName = newStr
                        };

                        if(!UIFiles.Contains(uiData))
                            UIFiles.Add(uiData);
                    }
                }
            }

            InfoVisible = Visibility.Collapsed;
        }
        private void AddFilesInFolder(string folder)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(folder))
            {
                if (CSUtility.Support.IFileManager.Instance.GetFileExtension(file) == "xml")
                {
                    var newStr = file.Replace("\\", "/");
                    var uiData = new UIData()
                    {
                        FileName = newStr
                    };

                    if (!UIFiles.Contains(uiData))
                        UIFiles.Add(uiData);
                }
            }

            foreach (var subFolder in System.IO.Directory.EnumerateDirectories(folder))
            {
                AddFilesInFolder(subFolder);
            }
        }

        private void ListBox_UIs_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //    e.Effects = DragDropEffects.Link;
            //else
            //    e.Effects = DragDropEffects.None;
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedItems = ListBox_UIs.SelectedItems;
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                UIFiles.Remove((selectedItems[i]) as UIData);
            }

            if (UIFiles.Count == 0)
                InfoVisible = Visibility.Visible;
        }

        public void GetAllUIFiles()
        {
            UIFiles.Clear();
            List<string> removedFile = new List<string>();
            foreach(var file in CCore.Support.ReflectionManager.Instance.UIFileDic)
            {
                var name = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(file.Value);
                if (!System.IO.File.Exists(name))
                {
                    removedFile.Add(file.Key);
                    continue;
                }
                                        
                var uiData = new UIData()
                {
                    FileName = name
                };

                UIFiles.Add(uiData);
            }

            foreach(var i in removedFile)
            {
                CCore.Support.ReflectionManager.Instance.RemoveUIFile(i);
            }

            if (UIFiles.Count != 0)
                InfoVisible = Visibility.Collapsed;
        }

        //string mConfigFile = AppDomain.CurrentDomain.BaseDirectory + "PublisherUIResConfig.dat";
        //public void LoadResConfig()
        //{
        //    var xmlHolder = CSUtility.Support.IXmlHolder.LoadXML(mConfigFile);
        //    if (xmlHolder == null || xmlHolder.RootNode == null)
        //        return;

        //    foreach (var node in xmlHolder.RootNode.FindNodes("UI"))
        //    {
        //        var att = node.FindAttrib("File");
        //        if (att == null)
        //            continue;

        //        var uiData = new UIData()
        //        {
        //            FileName = att.Value
        //        };
        //        if (!UIFiles.Contains(uiData))
        //            UIFiles.Add(uiData);
        //    }

        //    if(UIFiles.Count > 0)
        //        InfoVisible = Visibility.Collapsed;
        //}

        //public void SaveResConfig()
        //{
        //    var xmlHolder = CSUtility.Support.IXmlHolder.NewXMLHolder("UIFiles", "");

        //    foreach (var uiFile in UIFiles)
        //    {
        //        var node = xmlHolder.RootNode.AddNode("UI", "", xmlHolder);
        //        node.AddAttrib("File", uiFile.FileName);
        //    }

        //    CSUtility.Support.IXmlHolder.SaveXML(mConfigFile, xmlHolder, true);
        //}
    }
}
