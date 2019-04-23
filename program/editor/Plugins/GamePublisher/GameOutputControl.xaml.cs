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
    /// Interaction logic for GameOutputControl.xaml
    /// </summary>
    public partial class GameOutputControl : UserControl, INotifyPropertyChanged
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

        public string FullPackageFolder
        {
            get { return Program.FullPackageFolder; }
            set
            {
                Program.FullPackageFolder = value;
                OnPropertyChanged("FullPackageFolder");
            }
        }

        public string MinPackageFolder
        {
            get { return Program.MinPackageFolder; }
            set
            {
                Program.MinPackageFolder = value;
                OnPropertyChanged("MinPackageFolder");
            }
        }

        bool mEnableButton = true;
        public bool EnableButton
        {
            get { return mEnableButton; }
            set
            {
                mEnableButton = value;
                OnPropertyChanged("EnableButton");
            }
        }

        bool mEncrypt = true;
        public bool Encrypt
        {
            get { return mEncrypt; }
            set
            {
                mEncrypt = value;
                OnPropertyChanged("Encrypt");
            }
        }

        System.Threading.Thread mCurrentThread;

        public delegate void Delegate_OnUpdateProcessPercent(double percent);
        public Delegate_OnUpdateProcessPercent OnUpdateProcessPercent;

        public GameOutputControl()
        {
            InitializeComponent();
        }

        public void SaveMinPackage()
        {
            if (System.IO.Directory.Exists(MinPackageFolder))
            {
                try
                {
                    System.IO.Directory.Delete(@MinPackageFolder, true);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }

            if (!System.IO.Directory.Exists(MinPackageFolder))
                System.IO.Directory.CreateDirectory(MinPackageFolder);

            //LauncherPublisher.CopyLauncher(MinPackageFolder, Encrypt, FullPackageFolder);

            List<CSUtility.Support.enResourceType> except = new List<CSUtility.Support.enResourceType>()
            {
                CSUtility.Support.enResourceType.Texture,
                CSUtility.Support.enResourceType.MeshSource,
                //CSUtility.Support.enResourceType.Action,
            };
            // 提取最小包
            foreach (var resData in ResourceDataManager.Instance.ResDataDic.Values)
            {
                if (resData.ResourceType == CSUtility.Support.enResourceType.Folder)
                    continue;
                if (resData.CheckState != CheckBoxEx.enCheckState.UnChecked)
                {                    
                    if (Program.CopyToZip)
                        Program.CopyFile(resData.TargetFile + ".zip", MinPackageFolder + resData.RelativeFile + ".zip", false);
                    else
                        Program.CopyFile(resData.TargetFile, MinPackageFolder + resData.RelativeFile, false);
                }
                else if (!except.Contains(resData.ResourceType))
                {
                    resData.CheckState = CheckBoxEx.enCheckState.AllChecked;

                    if (Program.CopyToZip)
                        Program.CopyFile(resData.TargetFile + ".zip", MinPackageFolder + resData.RelativeFile + ".zip", false);
                    else
                        Program.CopyFile(resData.TargetFile, MinPackageFolder + resData.RelativeFile, false);
                }
            }

            // 将最小包数据拷贝到完整包
            ResourceDataManager.Instance.SaveCheckedResourceDatas(MinPackageFolder + "CFiles.cfg", false);
            Program.CopyFile(MinPackageFolder + "CFiles.cfg", FullPackageFolder + "CFiles.cfg", false);
            
            var cfilesMD5 = CSUtility.Program.GetMD5HashFromFile(MinPackageFolder + "CFiles.cfg");

            if (Program.CopyToZip)
            {
                CSUtility.Compress.CompressManager.ZipFile("CFiles.cfg", MinPackageFolder, MinPackageFolder + "CFiles.cfg.zip");
                System.IO.File.Delete(MinPackageFolder + "CFiles.cfg");
            }

            var fullZipPackageFolder = FullPackageFolder.Remove(FullPackageFolder.Length - 1) + "Zip/";
            //CSUtility.Compress.CompressManager.ZipFile("CFiles.cfg", MinPackageFolder, fullZipPackageFolder + "CFiles.cfg.zip");

            var fullFilesMD5 = CSUtility.Program.GetMD5HashFromFile(FullPackageFolder + "FInfo.cfg");

            CSUtility.FileDownload.GameInfo gameInfo = new CSUtility.FileDownload.GameInfo();
            gameInfo.Version = Program.Version;
            gameInfo.MinPkgSize = 0;// totalSize;// GetDirectorySize(targetFolder);
            gameInfo.MinPkgFilesMD5 = cfilesMD5;
            gameInfo.FullPkgMD5 = fullFilesMD5;
            gameInfo.Save(MinPackageFolder + "Game.inf");
            gameInfo.Save(FullPackageFolder + "Game.inf");
            gameInfo.Save(fullZipPackageFolder + "Game.inf");
        }

        public void CalculateTotalSize()
        {
            Int64 totalSize = 0;

            var cfilesMD5 = CSUtility.Program.GetMD5HashFromFile(MinPackageFolder + "CFiles.cfg");

            var fullZipPackageFolder = FullPackageFolder.Remove(FullPackageFolder.Length - 1) + "Zip/";
            CSUtility.Compress.CompressManager.ZipFile("CFiles.cfg", MinPackageFolder, fullZipPackageFolder + "CFiles.cfg.zip");

            foreach (var resData in ResourceDataManager.Instance.ResDataDic.Values)
            {
                if (resData.ResourceType == CSUtility.Support.enResourceType.Folder)
                    continue;

                //if (resData.IsChecked == false)
                //    continue;
                if (resData.CheckState == CheckBoxEx.enCheckState.UnChecked)
                    continue;

                var fileName = fullZipPackageFolder + resData.RelativeFile + ".zip";
                var fileInfo = new System.IO.FileInfo(fileName);
                totalSize += fileInfo.Length;
            }

            var fullFilesMD5 = CSUtility.Program.GetMD5HashFromFile(FullPackageFolder + "FInfo.cfg");

            CSUtility.FileDownload.GameInfo gameInfo = new CSUtility.FileDownload.GameInfo();
            gameInfo.Version = Program.Version;
            gameInfo.MinPkgSize = totalSize;// GetDirectorySize(targetFolder);
            gameInfo.MinPkgFilesMD5 = cfilesMD5;
            gameInfo.FullPkgMD5 = fullFilesMD5;
            gameInfo.Save(MinPackageFolder + "Game.inf");
            gameInfo.Save(FullPackageFolder + "Game.inf");
            gameInfo.Save(fullZipPackageFolder + "Game.inf");
        }

        private void Button_SaveConfig_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveMinPackage();
        }

        private string mCompareConfigFile = "";
        private void Button_LoadConfig_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = MinPackageFolder;
            ofd.Filter = "FilesConfig|*.cfg";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(ofd.FileName))
                    return;

                mCompareConfigFile = ofd.FileName;

                mCurrentThread = new System.Threading.Thread(new System.Threading.ThreadStart(MakeCheckResourceDataWithResDatas));
                mCurrentThread.Name = "最小包选择";
                mCurrentThread.IsBackground = true;
                mCurrentThread.Start();

                EnableButton = false;
            }
        }

        private void Button_MinPackageFolderSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = MinPackageFolder;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MinPackageFolder = fbd.SelectedPath;
            }
        }

        private void MakeCheckResourceDataWithResDatas()
        {
            var resDatas = ResourceDataManager.Instance.LoadResourceDatas(mCompareConfigFile, false);
            var compareResDatas = ResourceDataManager.GetResourceDataListFromTree(resDatas.ToArray<ResourceData>(), null);
            var currentResDatas = ResourceDataManager.GetResourceDataListFromTree(ResourceDataManager.Instance.ResDataDic.Values.ToArray<ResourceData>(), null);

            double total = compareResDatas.Count * currentResDatas.Count;
            Int64 i = 0;
            foreach (var curData in currentResDatas)
            {
                foreach (var comData in compareResDatas)
                {
                    if (OnUpdateProcessPercent != null)
                    {
                        OnUpdateProcessPercent(i / total);
                        i++;
                    }

                    if (curData.Equals(comData))
                    {
                        //curData.IsChecked = true;
                        curData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                        break;
                    }
                }
            }

            EnableButton = true;
            System.Windows.Forms.MessageBox.Show("配置读取完成！");
        }

        string mInfoStr = "";
        public void OutputInfo(string info, Brush brush)
        {
            Paragraph p = new Paragraph()
            {
                Margin = new Thickness(0)
            };
            Span span = new Span(new Run(System.DateTime.Now.ToString() + ": "))
            {
                Foreground = Brushes.LightGray
            };
            p.Inlines.Add(span);
            span = new Span(new Run(info))
            {
                Foreground = brush
            };
            p.Inlines.Add(span);
            RichTextBox_Info.Document.Blocks.Add(p);

            mInfoStr += System.DateTime.Now.ToString() + ": " + info + "\r\n";

            RichTextBox_Info.ScrollToEnd();
        }

        public void AddMapInfoTab(MapResourceItem mapItem, ObservableCollection<ResourceData> mapResources)
        {
            var tabItem = new TabItem();
            var fiCtrl = new FilesInfoControl();
            fiCtrl.ItemsSource = mapResources;
            tabItem.Content = fiCtrl;
            tabItem.Header = mapItem.MapName;
            TabControl_Maps.Items.Add(tabItem);
        }

        public void SetFilesInfoData(ObservableCollection<ResourceData> resources)
        {
            FilesInfoCtrl.ItemsSource = resources;
        }

        public void SaveInfoToFile(string absFileName)
        {
            using (var writer = new System.IO.StreamWriter(absFileName, false, Encoding.Unicode))
            {
                writer.Write(mInfoStr);
            }
        }
    }
}
