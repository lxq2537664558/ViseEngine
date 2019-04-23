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
    /// Interaction logic for LauncherPublisher.xaml
    /// </summary>
    public partial class LauncherPublisher : UserControl, INotifyPropertyChanged
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

        public string SourceFolder
        {
            get { return Program.SourceFolder; }
            set
            {
                Program.SourceFolder = value;
                OnPropertyChanged("SourceFolder");
            }
        }

        public string TargetFolder
        {
            get { return Program.LauncherFolder; }
            set
            {
                Program.LauncherFolder = value;

                OnPropertyChanged("TargetFolder");
            }
        }

        public UInt32 Version_0
        {
            get { return Program.LauncherVersion_0; }
            set
            {
                Program.LauncherVersion_0 = value;
                UpdateAutoTargetFolder();
                OnPropertyChanged("Version_0");
            }
        }
        public UInt32 Version_1
        {
            get { return Program.LauncherVersion_1; }
            set
            {
                Program.LauncherVersion_1 = value;
                UpdateAutoTargetFolder();
                OnPropertyChanged("Version_1");
            }
        }
        public UInt32 Version_2
        {
            get { return Program.LauncherVersion_2; }
            set
            {
                Program.LauncherVersion_2 = value;
                UpdateAutoTargetFolder();
                OnPropertyChanged("Version_2");
            }
        }

        UInt64 mSVNVersion = 0;
        public UInt64 SVNVersion
        {
            get { return mSVNVersion; }
            set
            {
                mSVNVersion = value;
                OnPropertyChanged("SVNVersion");
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

        public LauncherPublisher()
        {
            InitializeComponent();

            CalculateCurrentVersion();
            SVNVersion = Program.CheckSVNVersion(SourceFolder);

            UpdateAutoTargetFolder();
        }

        private void UpdateAutoTargetFolder()
        {
            TargetFolder = SourceFolder.Replace("Release", "FinalRelease/Launcher") + Version_0 + "." + Version_1 + "." + Version_2 + "/";
        }

        #region Copy Launcher

        static string[] launcherCompressFiles = new string[]{
            "Client.Android.dll",
            "Client.dll",
            "Client.Windows.dll",
            "Client_A.dll",
            "ClientCommon.dll",
            "core.Windows.dll",

            "HPSocket4C_U.dll",
            "HPSocketCS.dll",

            "msvcp100.dll",
            "msvcr100.dll",
        };
        static string[] dllNeedToEncrypt = new string[]{
            "Client.Android.dll",
            "Client.dll",
            "Client.Windows.dll",
            "Client_A.dll",
            "ClientCommon.dll",
            "core.Windows.dll",
        };
        static string[] dllNeedToSign = new string[]{
            "Client.Android.dll",
            "Client.dll",
            "Client.Windows.dll",
            "Client_A.dll",
            "ClientCommon.dll",
            "core.Windows.dll",
        };

        public static void CopyLauncher(string targetFolder, bool encrypt, string srcFolder = "")
        {
            if (targetFolder[targetFolder.Length - 1] != '/' && targetFolder[targetFolder.Length - 1] != '\\')
                targetFolder += "/";

            var launcherSrcExeFile = srcFolder + "Launcher.exe";
            if (string.IsNullOrEmpty(srcFolder))
            {
                srcFolder = Program.SourceFolder + "binary/Launcher/";
                launcherSrcExeFile = srcFolder + "Launcher_Spoon_AnyCPU.exe";
            }

            if (!System.IO.Directory.Exists(targetFolder))
                System.IO.Directory.CreateDirectory(targetFolder);

            // copy exe
            Program.CopyFile(launcherSrcExeFile, targetFolder + "Launcher.exe", Program.CopyToZip);
            //if (encrypt)
            //    Program.Use_DotNET_Reactor_Encrypt(targetFolder + "Launcher.exe", targetFolder + "Launcher.exe");

            // 数字签名
            Program.Use_SignTool_Sign(targetFolder + "Launcher.exe");

            foreach (var file in launcherCompressFiles)
            {
                var targetFile = Program.CopyFile(srcFolder + "Launcher/" + file, targetFolder + "Launcher/" + file, Program.CopyToZip);
                // 文件加密
                if (dllNeedToEncrypt.Contains(file) && encrypt)
                {
                    Program.Use_DotNET_Reactor_Encrypt(targetFile, targetFile);
                }

                // 数字签名
                if(dllNeedToSign.Contains(file))
                    Program.Use_SignTool_Sign(targetFile);
            }
            // 
            //var launcherInfo = new LauncherDll.LauncherInfo();
            //var lastVersionInfo = PublisherInfoManager.Instance.GetLauncherLastVersionInfo();
            //launcherInfo.Version = lastVersionInfo.Version;//Program.LauncherVersion;
            //launcherInfo.LauncherDllMD5 = lastVersionInfo.IdentityString;
            //launcherInfo.HostIPAddress = Program.HostIPAddress;
            //launcherInfo.Save(targetFolder + "Info.xml");
        }

        // 压缩Launcher
        public static void CompressLauncher(string targetFolder)
        {
            var srcFolder = targetFolder + "Launcher/";

            CSUtility.Compress.CompressManager.ZipFile(launcherCompressFiles, srcFolder, targetFolder + "Launcher.zip");

            // 记录Zip的md5码
            var lastVersionInfo = PublisherInfoManager.Instance.GetLauncherLastVersionInfo();
            lastVersionInfo.IdentityString = CSUtility.Program.GetMD5HashFromFile(targetFolder + "Launcher.zip");

//             var launcherInfo = new LauncherDll.LauncherInfo();
//             launcherInfo.Version = lastVersionInfo.Version;//Program.LauncherVersion;
//             launcherInfo.LauncherDllMD5 = lastVersionInfo.IdentityString;
//             launcherInfo.Save(targetFolder + "Info.xml");
        }

        #endregion

        private void Button_SrcFolderSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = SourceFolder;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SourceFolder = fbd.SelectedPath;
            }
        }

        private void Button_TargetFolderSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = TargetFolder;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TargetFolder = fbd.SelectedPath;
            }
        }

        private void Ver_0_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Version_0++;
                Version_1 = 0;
                Version_2 = 0;
            }
        }
        private void Ver_1_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Version_1++;
                Version_2 = 0;
            }
        }
        private void Ver_2_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                CalculateCurrentVersion();
            }
        }

        private void CalculateCurrentVersion()
        {
            var lastVersion = PublisherInfoManager.Instance.GetLauncherLastVersionInfo();
            if (lastVersion == null)
            {
                Version_0 = 0;
                Version_1 = 0;
                Version_2 = 1;
            }
            else
            {
                var splits = lastVersion.Version.Split('.');
                System.Diagnostics.Debug.Assert(splits.Length == 3);
                Version_0 = System.Convert.ToUInt32(splits[0]);
                Version_1 = System.Convert.ToUInt32(splits[1]);
                Version_2 = System.Convert.ToUInt32(splits[2]) + 1;
            }
        }

        private void Button_Release_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show("请确认代码已编译为Release版!", "警告", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            if (System.IO.Directory.Exists(TargetFolder))
            {
                if (MessageBox.Show("文件夹" + TargetFolder + "已存在, 是否替换此文件夹中的内容?", "警告", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    return;

                try
                {
                    System.IO.Directory.Delete(@TargetFolder, true);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }

            PublisherInfo info = new PublisherInfo(Program.LauncherVersion, SVNVersion);
            PublisherInfoManager.Instance.LauncherInfos.Add(info);

            CopyLauncher(TargetFolder, Encrypt);
            CompressLauncher(TargetFolder);

            PublisherInfoManager.Instance.SaveInfos();

            MessageBox.Show("Launcher发布完成");
        }
    }
}
