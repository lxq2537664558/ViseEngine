using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
    /// MainControl.xaml 的交互逻辑
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "GamePublisher")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/游戏发布工具")]
    [Guid("7de1bdcb-4e00-4210-b541-616e68c34826")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        public string PluginName
        {
            get { return "GamePublisher"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "GamePublisher",
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

        public void Tick()
        {

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

        ////////////////////////////////

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

        public MainControl()
        {
            InitializeComponent();
            
            CSUtility.Support.ClassInfoManager.Instance.Load(false);

            ResourceAnalyse.Instance.Init();

            InitEvn();

            PublisherInfoManager.Instance.LoadInfos();
            OutPutControl.SetFilesInfoData(ResourceDataManager.Instance.ResourceDatas);
            OutPutControl.OnUpdateProcessPercent = UpdateProcessPercent;

            CalculateCurrentVersion();
            SVNVersion = Program.CheckSVNVersion(SourceFolder);

            UpdateAutoTargetFolder();
            FinalPublishFolder = SourceFolder.Replace("Release", "FinalRelease/ZeusPublish/FullPackageZip");
        }

        CCore.Graphics.REnviroment mREnviroment;
        void InitEvn()
        {
            var _init = new CCore.EngineInit();
            _init.ClientInit = new CCore.ClientInit();
            _init.ClientInit.GraphicsInit = new CCore.Graphics.GraphicsInit();
            _init.ClientInit.GraphicsInit.hDeviceWindow = D3DDrawPanel.Handle;
            _init.ClientInit.MsgRecieverMgrInit = new CCore.MsgProc.MsgRecieverMgrInit();
            if (CCore.Engine.Instance.Initialize(_init) == false)
            {
                MessageBox.Show("Engine 初始化失败!");
            }

            var viewTarget = new CCore.Graphics.ViewTarget();
            viewTarget.SetControl(D3DDrawPanel);

            var _reInit = new CCore.Graphics.REnviromentInit();
            _reInit.ViewInit = new CCore.Graphics.ViewInit();
            _reInit.ViewInit.ViewWnd = viewTarget;
            _reInit.bUseIntZ = true;
            _reInit.bMainScene = true;
            var view = new CCore.Graphics.View();
            view.Initialize(_reInit.ViewInit);
            mREnviroment = new CCore.Graphics.REnviroment();
            if (mREnviroment.Initialize(_reInit, view) == false)
            {
                MessageBox.Show("IREnviroment 初始化失败!");
            }
        }
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
            get { return Program.TargetFolder; }
            set
            {
                Program.TargetFolder = value;

                FullPackageFolder = Program.TargetFolder + "FullPackage/";
                MinPackageFolder = Program.TargetFolder + "MinPackage/";

                OnPropertyChanged("TargetFolder");
            }
        }

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

        public bool CompareFinalPublish
        {
            get { return Program.CompareFinalPublish; }
            set
            {
                Program.CompareFinalPublish = value;
                OnPropertyChanged("CompareFinalPublish");
            }
        }

        bool mReCreateShaderCache = false;
        public bool ReCreateShaderCache
        {
            get { return mReCreateShaderCache; }
            set
            {
                mReCreateShaderCache = value;
                OnPropertyChanged("ReCreateShaderCache");
            }
        }

        public UInt32 Version_0
        {
            get { return Program.Version_0; }
            set
            {
                Program.Version_0 = value;
                UpdateAutoTargetFolder();
                OnPropertyChanged("Version_0");
            }
        }
        public UInt32 Version_1
        {
            get { return Program.Version_1; }
            set
            {
                Program.Version_1 = value;
                UpdateAutoTargetFolder();
                OnPropertyChanged("Version_1");
            }
        }
        public UInt32 Version_2
        {
            get { return Program.Version_2; }
            set
            {
                Program.Version_2 = value;
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

                OutPutControl.Encrypt = mEncrypt;

                OnPropertyChanged("Encrypt");
            }
        }
        public string FinalPublishFolder
        {
            get { return Program.FinalPublishFolder; }
            set
            {
                Program.FinalPublishFolder = value;
                OnPropertyChanged("FinalPublishFolder");
            }
        }

        string mProgressInfo = "";
        public string ProgressInfo
        {
            get { return mProgressInfo; }
            set
            {
                mProgressInfo = value;
                OnPropertyChanged("ProgressInfo");
            }
        }

        Visibility mRelaseProcessInfoShow = Visibility.Collapsed;
        public Visibility ReleaseProcessInfoShow
        {
            get { return mRelaseProcessInfoShow; }
            set
            {
                mRelaseProcessInfoShow = value;
                OnPropertyChanged("ReleaseProcessInfoShow");
            }
        }

        double mProgressPercent = 0;
        public double ProgressPercent
        {
            get { return mProgressPercent; }
            set
            {
                mProgressPercent = value;
                //                 this.Dispatcher.Invoke(new Action(() =>
                //                 {
                //                     TaskbarItemInfo.ProgressValue = value;
                //                 }));
                OnPropertyChanged("ProgressPercent");
            }
        }

        private void UpdateAutoTargetFolder()
        {
            TargetFolder = SourceFolder.Replace("Release", "FinalRelease/Game") + Version_0 + "." + Version_1 + "." + Version_2 + "/";
        }
        private void UpdateProcessPercent(double percent)
        {
            ProgressPercent = percent;
        }
        private void Button_FinalPublishFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = FinalPublishFolder;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FinalPublishFolder = fbd.SelectedPath;
            }
        }

        private void Button_SrcFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = SourceFolder;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SourceFolder = fbd.SelectedPath;
            }
        }

        private void Button_TargetFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = FullPackageFolder;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FullPackageFolder = fbd.SelectedPath;
            }
        }

        bool mProcessFinish = false;
        System.DateTime mStartTime;
        System.Threading.Thread mCurrentThread;

        CSUtility.Support.ConcurentObjManager<Guid, object> mCheckedResource = new CSUtility.Support.ConcurentObjManager<Guid, object>();
        private void Button_Release_Click(object sender, RoutedEventArgs e)
        {
            try
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
                
                MapResPanel.SaveMapResConfig();

                mStartTime = System.DateTime.Now;
                mProcessFinish = false;
                mCheckedResource.Clear();
                ProgressPercent = 0;
                ProgressInfo = "";
                ReleaseProcessInfoShow = Visibility.Visible;
                ResourceDataManager.Instance.Clear();
                //TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;

                mCurrentThread = new System.Threading.Thread(new System.Threading.ThreadStart(ReleaseProcess));
                mCurrentThread.Name = "版本发布";
                mCurrentThread.IsBackground = true;
                mCurrentThread.Start();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CalculateCurrentVersion()
        {
            var lastVersion = PublisherInfoManager.Instance.GetLastVersion();
            if (string.IsNullOrEmpty(lastVersion))
            {
                Version_0 = 0;
                Version_1 = 0;
                Version_2 = 1;
            }
            else
            {
                var splits = lastVersion.Split('.');
                System.Diagnostics.Debug.Assert(splits.Length == 3);
                Version_0 = System.Convert.ToUInt32(splits[0]);
                Version_1 = System.Convert.ToUInt32(splits[1]);
                Version_2 = System.Convert.ToUInt32(splits[2]) + 1;
            }
        }
        private void Ver_0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Version_0++;
                Version_1 = 0;
                Version_2 = 0;
            }
        }

        private void Ver_1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Version_1++;
                Version_2 = 0;
            }
        }

        private void Ver_2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                CalculateCurrentVersion();
            }
        }

        private string GetPropertyDisplayName(System.Reflection.PropertyInfo proInfo)
        {
            var atts = proInfo.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true);
            if (atts.Length <= 0)
                return "属性:" + proInfo.Name;

            var att = atts[0] as System.ComponentModel.DisplayNameAttribute;
            if (string.IsNullOrEmpty(att.DisplayName))
                return "属性:" + proInfo.Name;

            return "属性:" + att.DisplayName;
        }

        private void OutputInfo(string info, Brush brush)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                OutPutControl.OutputInfo(info, brush);
            }));
        }

        private void AddResource(ResourceData data, string refSource, ObservableCollection<ResourceData> rootResDatas = null)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                ResourceDataManager.Instance.AddResource(data, refSource, Program.CopyToZip, rootResDatas);
            }));
        }

        private void CopyDirectory(string srcDir, string tagDir, CSUtility.Support.enResourceType resType, List<string> exceptFolders = null, List<string> exceptFiles = null, ObservableCollection<ResourceData> resList = null)
        {
            if (exceptFolders != null)
            {
                foreach (var expFolder in exceptFolders)
                {
                    if (srcDir.Replace("\\", "/") == expFolder.Replace("\\", "/"))
                        return;
                }
            }

            System.IO.DirectoryInfo source = new System.IO.DirectoryInfo(srcDir);
            System.IO.DirectoryInfo target = new System.IO.DirectoryInfo(tagDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
                return;

            if (!target.Exists)
                target.Create();

            var folderResData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Folder, tagDir);
            if (folderResData != null)
                AddResource(folderResData, "", resList);

            var files = source.GetFiles();
            foreach (var fileInfo in files)
            {
                if (exceptFiles != null)
                {
                    bool bFind = false;
                    foreach (var expFile in exceptFiles)
                    {
                        if (fileInfo.FullName.Replace("\\", "/") == expFile.Replace("\\", "/"))
                        {
                            bFind = true;
                            break;
                        }
                    }

                    if (bFind)
                        continue;
                }

                ProgressInfo = "正在拷贝文件" + fileInfo.FullName;
                var tagFileName = Program.CopyFile(fileInfo.FullName, Program.CopyToZip);

                var resData = ResourceData.CreateResourceData(resType, tagFileName);
                if (resData == null)
                    OutputInfo("找不到 " + tagFileName, Brushes.Red);
                else
                {
                    //ResourceDataManager.Instance.AddResource(resData);
                    AddResource(resData, "", resList);
                }
            }
            var dirs = source.GetDirectories();
            foreach (var dirInfo in dirs)
            {
                CopyDirectory(dirInfo.FullName, target.FullName + @"\" + dirInfo.Name, resType, exceptFolders, exceptFiles, resList);
            }
        }

        double shaderCachePrecent = 0;
        private void ReleaseProcess()
        {            
            OutputInfo("开始发布版本：" + Version_0 + "." + Version_1 + "." + Version_2, Brushes.Green);

            CCore.Engine.Instance.Client.Graphics.MaterialMgr.ClearFileDictionary();            
            //LauncherPublisher.CopyLauncher(FullPackageFolder, mEncrypt);
            CopyBin(shaderCachePrecent + (1 - shaderCachePrecent) * 0.05);
            CopyProgramResources(shaderCachePrecent + (1 - shaderCachePrecent) * 0.1, CSUtility.Support.IFileConfig.Instance);
            CopyFont(shaderCachePrecent + (1 - shaderCachePrecent) * 0.15);
            CopyMaps(shaderCachePrecent + (1 - shaderCachePrecent) * 0.25);
            CopyUI(shaderCachePrecent + (1 - shaderCachePrecent) * 0.35);
            AnalyseEvent(shaderCachePrecent + (1 - shaderCachePrecent) * 0.4, CSUtility.Helper.enCSType.Client);
            AnalyseFSM(shaderCachePrecent + (1 - shaderCachePrecent) * 0.45, CSUtility.Helper.enCSType.Client);                     
            CopyMetaData(shaderCachePrecent + (1 - shaderCachePrecent) * 0.55);            
            CopySounds(shaderCachePrecent + (1 - shaderCachePrecent) * 0.6);            
            CopyTemplate(shaderCachePrecent + (1 - shaderCachePrecent) * 0.7);
            CopyFileCache(shaderCachePrecent + (1 - shaderCachePrecent) * 0.75);
            CopyDefaultRes(shaderCachePrecent + (1 - shaderCachePrecent) * 0.8);
            ProcessReCreateShaderCache(shaderCachePrecent + (1 - shaderCachePrecent) * 0.92);
            CopyShader(shaderCachePrecent + (1 - shaderCachePrecent) * 0.97);
            // 存储完整资源数据，用于发布器记录版本信息
            ResourceDataManager.Instance.SaveFullResourceDatas(TargetFolder + "FilesFullInfo.cfg");
            // 存储简化信息，用于跟随客户端发布
            ResourceDataManager.Instance.SaveSimpleResourceData(FullPackageFolder + "FInfo.cfg");            
            if (Program.CopyToZip)
            {
                CSUtility.Compress.CompressManager.ZipFile("FInfo.cfg", FullPackageFolder, FullPackageFolder + "FInfo.cfg.zip");
                System.IO.File.Delete(FullPackageFolder + "FInfo.cfg");
            }

            Dictionary<string, CSUtility.FileDownload.ResourceData> resPublishedDic = new Dictionary<string, CSUtility.FileDownload.ResourceData>();
            Dictionary<string, CSUtility.FileDownload.ResourceData> resNewDic = new Dictionary<string, CSUtility.FileDownload.ResourceData>();
            if (CompareFinalPublish)
            {
                CSUtility.Compress.CompressManager.Instance.UnZipFile(FinalPublishFolder + "FInfo.cfg.zip", CSUtility.Support.IFileManager.Instance.Root + "temp/");

                resPublishedDic = CSUtility.FileDownload.ResourceDataManager.LoadResourceDatas(CSUtility.Support.IFileManager.Instance.Root + "temp/FInfo.cfg");
                resNewDic = CSUtility.FileDownload.ResourceDataManager.LoadResourceDatas(FullPackageFolder + "FInfo.cfg");
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                // 勾选最小包资源
                foreach (var fileData in MinPkgRes.ResFiles)
                {
                    ResourceDataManager.Instance.CheckResourceWithRelativeFiles(fileData.RelativeFileName);
                }

                if (MinPkgRes.AutoSaveMinPackage)
                {
                    ProgressInfo = "正在制作最小包";
                    OutPutControl.SaveMinPackage();
                }

                var nowDate = System.DateTime.Now;
                var fileName = TargetFolder + "ErrorLog_" + Version_0 + "." + Version_1 + "." + Version_2 + "_" + nowDate.Year + "." + nowDate.Month + "." + nowDate.Day + " " + nowDate.Hour + "." + nowDate.Minute + "." + nowDate.Second + ".txt";
                OutPutControl.SaveInfoToFile(fileName);

                var timeRemain = System.DateTime.Now - mStartTime;
                OutputInfo("完整包可以进行测试了! 耗时: " + ((timeRemain.Days != 0) ? (timeRemain.Days + "天") : "") +
                                                    ((timeRemain.Hours != 0) ? (timeRemain.Hours + "小时") : "") +
                                                    ((timeRemain.Minutes != 0) ? (timeRemain.Minutes + "分") : "") +
                                                    ((timeRemain.Seconds != 0) ? (timeRemain.Seconds + "秒") : "") + "\r\n错误信息已存到文件:" + fileName, Brushes.Green);
            }));

            ProgressInfo = "正在压缩" + FullPackageFolder;
            ZipCopyFolder(FullPackageFolder, FullPackageFolder.Remove(FullPackageFolder.Length - 1) + "Zip/", resPublishedDic, resNewDic);

            ProgressPercent = 1;

            this.Dispatcher.Invoke(new Action(() =>
            {
                OutPutControl.CalculateTotalSize();

                ProgressInfo = "发布完成";

                var timeRemain = System.DateTime.Now - mStartTime;
                var str = "发布完成! 耗时: " + ((timeRemain.Days != 0) ? (timeRemain.Days + "天") : "") +
                                                    ((timeRemain.Hours != 0) ? (timeRemain.Hours + "小时") : "") +
                                                    ((timeRemain.Minutes != 0) ? (timeRemain.Minutes + "分") : "") +
                                                    ((timeRemain.Seconds != 0) ? (timeRemain.Seconds + "秒") : "");
                OutputInfo(str, Brushes.LightBlue);

                PublisherInfo info = new PublisherInfo(Program.Version, SVNVersion);
                PublisherInfoManager.Instance.Infos.Add(info);
                PublisherInfoManager.Instance.SaveInfos();

                mProcessFinish = true;
                //TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;

                MessageBox.Show(str);
            }));
        }

        private void ButtonProcessCancel_Click(object sender, RoutedEventArgs e)
        {
            if (!mProcessFinish)
            {
                if (MessageBox.Show("即将取消发布过程，是否确认取消？", "警告", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            if (mCurrentThread != null)
            {
                mCurrentThread.Abort();
                mCurrentThread = null;
                ProgressPercent = 0;
                ProgressInfo = "";
                ReleaseProcessInfoShow = Visibility.Collapsed;
            }
        }

        private void ProcessReCreateShaderCache(double progressPrecentEnd)
        {
            if (mREnviroment == null)
                return;

            // 重新生成ShaderCache
            if (ReCreateShaderCache)
            {
                var files = System.IO.Directory.EnumerateFiles(SourceFolder + CSUtility.Support.IFileConfig.DefaultShaderDirectory + "/cachebin", "*.*", System.IO.SearchOption.AllDirectories);
                var delta = (progressPrecentEnd - ProgressPercent) / files.Count<string>() * 0.1f;

                foreach (var file in files)
                {
                    ProgressInfo = "正在删除ShaderCache文件 " + file;
                    System.IO.File.Delete(file);

                    ProgressPercent += delta;
                }
            }

            var matFiles = System.IO.Directory.EnumerateFiles(FullPackageFolder + CSUtility.Support.IFileConfig.DefaultResourceDirectory, "*.mtl", System.IO.SearchOption.AllDirectories);
            var deltaM = (progressPrecentEnd - ProgressPercent) / matFiles.Count<string>();
            foreach (var file in matFiles)
            {
                ProgressInfo = "正在生成ShaderCache " + file;

                var fileInfo = new System.IO.FileInfo(file);
                var fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
                var tempGuid = CSUtility.Support.IHelper.GuidTryParse(fileName);
                if (tempGuid == Guid.Empty)
                    continue;

                mREnviroment.Editor_UpdateShaderCache(tempGuid);

                ProgressPercent += deltaM;
            }

            // 打包ShaderCache
            ProgressInfo = "正在打包ShaderCache!";
            mREnviroment.Editor_PackShaderCache();
        }

        #region CopyDefaultRes
        private void CopyDefaultRes(double progressPrecentEnd)
        {
            ProgressInfo = "复制默认资源...";

            var srcFolder = SourceFolder + "resources/default/";
            var targetFolder = FullPackageFolder + "resources/default/";

            if (!System.IO.Directory.Exists(targetFolder))
                System.IO.Directory.CreateDirectory(targetFolder);

            foreach(var i in System.IO.Directory.GetDirectories(srcFolder,"*",System.IO.SearchOption.AllDirectories))
            {
                var dir = i.Replace("\\", "/").Replace(srcFolder, targetFolder);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
            }

            var files = System.IO.Directory.GetFiles(srcFolder, "*", System.IO.SearchOption.AllDirectories);
            var delta = (progressPrecentEnd - ProgressPercent) / (float)files.Length;
            foreach (var i in files)
            {
                ProgressPercent += delta;
                if (CSUtility.Support.IFileManager.Instance.GetFileExtension(i) == "rinfo")
                    continue;
                if (i.LastIndexOf('_') != -1 && i.Substring(i.LastIndexOf('_')) == "_Snapshot.png")
                    continue;
                if (i.Substring(i.LastIndexOf('.') + 1) == "link")
                    continue;
                var file = i.Replace("\\", "/").Replace(srcFolder, targetFolder);                

                if (!System.IO.File.Exists(file))                                        
                {
                    System.IO.File.Copy(i, file);

                    var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Unknow, i);
                    if (resData == null)
                    {
                        OutputInfo("找不到文件 " + file, Brushes.Red);
                        continue;
                    }

                    resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                    AddResource(resData, "DefaultRes");
                }
                else
                {
                    var retFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file, Program.FullPackageFolder);
                    if (ResourceDataManager.Instance.ResDataDic.ContainsKey(retFile))
                    {
                        ResourceDataManager.Instance.ResDataDic[retFile].CheckState = CheckBoxEx.enCheckState.AllChecked;
                    }
                }
            }
        }
        #endregion
        #region Copy bin
        // 需要拷贝的文件
        string[] binFolderNeedCopyFiles = new string[]{
            "Game.Windows.exe",
            //"Client.Android.dll",
            "Client.dll",
            //"Client.Windows.dll",
            //"Client_A.dll",
            "ClientCommon.dll",
            "core.Windows.dll",

            "CodeGenerateSystem.dll",
            "DockControl.dll",
            "EditorCommon.dll",
            "EditorControlLib.dll",

            "D3DCompiler_42.dll",
            "d3dx9_42.dll",

            "fmod.dll",
            //"HPSocket4C_U.dll",
            "HPSocketCS.dll",
            "libEGL.dll",
            "libGLESv2.dll",
            "libMaliEmulator.dll",
            "log4cplus.dll",

        };
        // 需要加密的文件
        string[] binFileNeedEncrypt = new string[]{
            "Game.Windows.exe",
            "Client.Android.dll",
            "Client.dll",
            "Client.Windows.dll",
            "Client_A.dll",
            "ClientCommon.dll",

            "CodeGenerateSystem.dll",
            "DockControl.dll",
            "EditorCommon.dll",
            "EditorControlLib.dll",
        };
        // 需要数字签名的文件
        string[] binFileNeedSign = new string[]{
            "Game.Windows.exe",
            "Client.Android.dll",
            "Client.dll",
            "Client.Windows.dll",
            "Client_A.dll",
            "ClientCommon.dll",
            "core.Windows.dll",

            "CodeGenerateSystem.dll",
            "DockControl.dll",
            "EditorCommon.dll",
            "EditorControlLib.dll",
        };
        private void CopyBin(double progressPrecentEnd)
        {
            ProgressInfo = "复制文件中...";

            var srcFolder = SourceFolder + "binary/";
            var TagFolder = FullPackageFolder + "binary/";

            if (!System.IO.Directory.Exists(TagFolder))
                System.IO.Directory.CreateDirectory(TagFolder);

            var delta = (progressPrecentEnd - ProgressPercent) / binFolderNeedCopyFiles.Length * 0.5f;

            foreach (var file in binFolderNeedCopyFiles)
            {
                if (!System.IO.File.Exists(srcFolder + file))
                {
                    MessageBox.Show("文件" + srcFolder + file + "不存在！");
                    continue;
                }

                ProgressInfo = "正在拷贝文件" + srcFolder + file;
                var targetFile = Program.CopyFile(srcFolder + file, Program.CopyToZip);

                //文件加密--------------------------------------
                if (binFileNeedEncrypt.Contains(file) && Encrypt)
                {
                    //Program.Use_DotNET_Reactor_Encrypt(targetFile, targetFile);
                }
                //-----------------------------------------------

                // 数字签名---------------------------------------
                if (binFileNeedSign.Contains(file))
                    Program.Use_SignTool_Sign(targetFile);
                //-----------------------------------------------

                // exe开启大内存模式------------------------------
                if (string.Equals(file, "ZeusSingle.exe"))
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.Arguments = "-edit -LARGEADDRESSAWARE " + file;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardInput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.FileName = srcFolder + "link.exe";

                    var process = new System.Diagnostics.Process();
                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true;
                    process.Start();

                    {
                        while (!process.HasExited)
                            System.Threading.Thread.Sleep(50);
                    }
                }
                //-----------------------------------------------

                var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Dll, srcFolder + file);
                if (resData == null)
                {
                    OutputInfo("找不到文件 " + TagFolder + file, Brushes.Red);
                }
                else
                {
                    resData.MD5 = CSUtility.Program.GetMD5HashFromFile(FullPackageFolder + resData.RelativeFile);
                    resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                    AddResource(resData, "Bin");
                }

                ProgressPercent += delta;
            }

            // 设置config
            //Program.CopyFile(SourceFolder + "Zero.cfg", false);

            var assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Client.dll");
            if (assembly == null)
                return;

            var type = assembly.GetType("Client.AppConfig");
            if (type != null)
            {
                var config = System.Activator.CreateInstance(type);
                if (config != null)
                {
                    if (CSUtility.Support.IConfigurator.FillProperty(config, SourceFolder + "Zero.cfg"))
                    {
                        var pro = config.GetType().GetProperty("FinalRelease");
                        pro.SetValue(config, true);
                        CSUtility.Support.IConfigurator.SaveProperty(config, "", FullPackageFolder + "Zero.cfg");
                    }
                }
            }
                         
            var cfgResData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Config, FullPackageFolder + "Zero.cfg");
            if (cfgResData != null)
            {
                cfgResData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                AddResource(cfgResData, "");
            }
                
            if (Program.CopyToZip)
            {
                CSUtility.Compress.CompressManager.ZipFile("Zero.cfg", FullPackageFolder, FullPackageFolder + "Zero.cfg.zip");
                System.IO.File.Delete(FullPackageFolder + "Zero.cfg");
            }
        }

        #endregion

        #region 程序中使用的资源    
        private void CopyProgramResources(double progressPrecentEnd,object target,string preStr = "")
        {
            if (target == null)
                return;
            var type = target.GetType();
            var propertys = type.GetProperties();
            var delta = (progressPrecentEnd - ProgressPercent) / propertys.Length;
            foreach (var property in propertys)
            {
                if (ResourceAnalyse.Instance.NeedAnalyse(type.FullName, property.Name))
                {
                    var inValue = property.GetValue(target, null);
                    AnalyseResource(progressPrecentEnd,inValue,property.PropertyType,preStr);
                    continue;
                }
                    
                var atts = property.GetCustomAttributes(typeof(CSUtility.Support.ResourcePublishAttribute), true);
                if (atts.Length <= 0)                             
                    continue;                  
                
                if (string.IsNullOrEmpty(preStr))
                {
                    preStr = "Program Res: " + property.Name;
                }                

                ProgressInfo = "正在分析程序资源 " + property.Name;

                var resType = ((CSUtility.Support.ResourcePublishAttribute)(atts[0])).ResourceType;
                #region 资源类型
                switch (resType)
                {
                    case CSUtility.Support.enResourceType.Technique:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };
                                    var techId = (Guid)(pIItem.GetValue(proValue, param));
                                    AnalyseTechniqueResource(techId, preStr);
                                }
                            }
                            else
                            {
                                var techId = (Guid)(property.GetValue(target, null));
                                AnalyseTechniqueResource(techId, preStr);
                            }
                        }
                        break;
                    case CSUtility.Support.enResourceType.Material:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };
                                    var matId = (Guid)(pIItem.GetValue(proValue, param));
                                    AnalyseMaterialResource(matId, preStr);
                                }
                            }
                            else
                            {
                                var matId = (Guid)(property.GetValue(target, null));
                                AnalyseMaterialResource(matId, preStr);
                            }
                        }
                        break;
                    case CSUtility.Support.enResourceType.MeshTemplate:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };
                                    var meshTemplateId = (Guid)(pIItem.GetValue(proValue, param));
                                    AnalyseMeshResource(meshTemplateId, preStr);
                                }
                            }
                            else
                            {
                                var meshTemplateId = (Guid)(property.GetValue(target, null));
                                AnalyseMeshResource(meshTemplateId, preStr);
                            }
                        }
                        break;
                    case CSUtility.Support.enResourceType.Effect:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };
                                    var effectId = (Guid)(pIItem.GetValue(proValue, param));
                                    AnalyseEffectResource(effectId, preStr);
                                }
                            }
                            else
                            {
                                var effectId = (Guid)(property.GetValue(target, null));
                                AnalyseEffectResource(effectId, preStr);
                            }
                        }
                        break;
                    case CSUtility.Support.enResourceType.Action:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };
                                    var actionFile = (string)(pIItem.GetValue(proValue, param));
                                    AnalyseActionResource(actionFile, preStr);
                                }
                            }
                            else
                            {
                                var actionFile = (string)(property.GetValue(target, null));
                                AnalyseActionResource(actionFile, preStr);
                            }
                        }
                        break;
                    case CSUtility.Support.enResourceType.UVAnim:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };
                                    var uvAnimId = (Guid)(pIItem.GetValue(proValue, param));
                                    AnalyseUVAnimResource(preStr, uvAnimId);
                                }
                            }
                            else
                            {
                                var uvAnimId = (Guid)(property.GetValue(target, null));
                                AnalyseUVAnimResource(preStr, uvAnimId);
                            }
                        }
                        break;
                    case CSUtility.Support.enResourceType.RoleTemplate:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };
                                    var roleId = (UInt16)(pIItem.GetValue(proValue, param));
                                    AnalyseRoleTemplateResource(roleId, preStr);
                                }
                            }
                            else
                            {
                                var roleId = (UInt16)(property.GetValue(target, null));
                                AnalyseRoleTemplateResource(roleId, preStr);
                            }
                        }
                        break;
                    case CSUtility.Support.enResourceType.Texture:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };

                                    var file = (string)(pIItem.GetValue(proValue, param));
                                    if (CSUtility.Support.IFileManager.Instance.GetFileExtension(file) != "png")
                                    {
                                        OutputInfo(preStr + " 错误纹理格式 " + file, Brushes.Red);
                                    }
                                    var resData = ResourceData.CreateResourceData(resType, file);
                                    if (resData == null)
                                    {
                                        OutputInfo(preStr + " 找不到文件 " + file, Brushes.Red);
                                    }
                                    else
                                    {
                                        resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                                        AddResource(resData, preStr);
                                    }
                                }
                            }
                            else
                            {
                                var file = (string)(property.GetValue(target, null));
                                if (CSUtility.Support.IFileManager.Instance.GetFileExtension(file) != "png")
                                {
                                    OutputInfo(preStr + " 错误纹理格式 " + file, Brushes.Red);
                                }
                                var resData = ResourceData.CreateResourceData(resType, file);
                                if (resData == null)
                                {
                                    OutputInfo(preStr + " 找不到文件 " + file, Brushes.Red);
                                }
                                else
                                {
                                    resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                                    AddResource(resData, preStr);
                                }
                            }
                        }
                        break;
                    case CSUtility.Support.enResourceType.MeshSource:
                    case CSUtility.Support.enResourceType.Config:
                    case CSUtility.Support.enResourceType.SimMeshSource:
                    case CSUtility.Support.enResourceType.PathMeshSource:
                        {
                            if (property.PropertyType.IsGenericType)
                            {
                                var proValue = property.GetValue(target, null);
                                var pICount = property.PropertyType.GetProperty("Count");
                                var pIItem = property.PropertyType.GetProperty("Item");
                                int count = (int)pICount.GetValue(proValue, null);
                                for (int i = 0; i < count; i++)
                                {
                                    var param = new System.Object[1] { i };

                                    var file = (string)(pIItem.GetValue(proValue, param));
                                    var resData = ResourceData.CreateResourceData(resType, file);
                                    if (resData == null)
                                    {
                                        OutputInfo(preStr + " 找不到文件 " + file, Brushes.Red);
                                    }
                                    else
                                    {
                                        resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                                        AddResource(resData, preStr);
                                    }
                                }
                            }
                            else
                            {
                                var file = (string)(property.GetValue(target, null));
                                var resData = ResourceData.CreateResourceData(resType, file);
                                if (resData == null)
                                {
                                    OutputInfo(preStr + " 找不到文件 " + file, Brushes.Red);
                                }
                                else
                                {
                                    resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                                    AddResource(resData, preStr);
                                }
                            }
                        }
                        break;
                } 
                #endregion
                ProgressPercent += delta;
            }
        }

        void AnalyseResource(double progressPrecentEnd, object inValue,Type type, string preStr = "")
        {
            if (type.IsGenericType)
            {
                if (type.IsValueType)
                {
                    var pIKey = type.GetProperty("Key");
                    var pIValue = type.GetProperty("Value");

                    var key = pIKey.GetValue(inValue, null); 
                    var value = pIValue.GetValue(inValue, null);

                    AnalyseResource(progressPrecentEnd, key,key.GetType(), preStr);
                    AnalyseResource(progressPrecentEnd, value, value.GetType(), preStr);
                }
                else
                { 
                    var enumerableValue = inValue as IEnumerable;
                    if (enumerableValue != null)
                    {
                        foreach (var item in enumerableValue)
                        {
                            AnalyseResource(progressPrecentEnd, item, item.GetType(),preStr);
                        }
                    }
                }
            }
            else if (type.IsArray)
            {
                var values = inValue as System.Array;
                foreach (var i in values)
                {
                    AnalyseResource(progressPrecentEnd, i,i.GetType(), preStr);
                }
            }
            else
            {
                CopyProgramResources(progressPrecentEnd, inValue, preStr);
            }        
        }
        
        private void AnalyseActionResource(string actionFile, string parentStr, ObservableCollection<ResourceData> resourceList = null)
        {
            // Action
            var data = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Action, actionFile);
            if (data == null)
            {
                OutputInfo(parentStr + " 找不到文件 " + actionFile, Brushes.Red);
                return;
            }
            AddResource(data, parentStr, resourceList);

            // Notify
            data = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Notify, actionFile + CSUtility.Support.IFileConfig.ActionNotifyExtension);
            if (data != null)
            {
                AddResource(data, parentStr, resourceList);
            }
            var actionNode = new CCore.AnimTree.AnimTreeNode_Action();
            actionNode.Initialize();            
            actionNode.ClearLink();
            actionNode.SetAction(actionFile);

            var notifiers = actionNode.Action.GetNotifiers(typeof(CSUtility.ActionNotify.EffectActionNotifier));
            foreach(var i in notifiers)
            {
                foreach(var p in i.NotifyPoints)
                {
                    foreach(var d in p.PointDatas)
                    {
                        var dat = d as CSUtility.ActionNotify.EffectItemData;
                        if (dat != null)
                        {
                            AnalyseEffectResource(dat.EffectId, parentStr, null);
                        }
                    }
                }
            }
        }
        private void AnalyseMeshResource(Guid meshTemplateId, string parentSourceInfoStr, ObservableCollection<ResourceData> resourceList = null)
        {
            var meshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = meshTemplateId,
            };
            var mesh = new CCore.Mesh.Mesh();
            mesh.Initialize(meshInit, null);

            AnalyseMeshResource(mesh, parentSourceInfoStr, resourceList);

            mesh.Cleanup();            
        }
        private void AnalyseMeshResource(CCore.Mesh.Mesh mesh, string parentSourceInfoStr, ObservableCollection<ResourceData> resourceList = null)
        {
            if (mesh == null)
                return;

            var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;

            if (meshInit.MeshTemplateID != Guid.Empty)
            {
                if (!mCheckedResource.ContainsKey(meshInit.MeshTemplateID))
                {
                    var meshTemplateFile = CCore.Mesh.MeshTemplateMgr.Instance.GetMeshTemplateFile(meshInit.MeshTemplateID);
                    if (string.IsNullOrEmpty(meshTemplateFile))
                    {
                        OutputInfo(parentSourceInfoStr + " 找不到MeshTemplate " + meshInit.MeshTemplateID, Brushes.Red);
                        mCheckedResource.Add(meshInit.MeshTemplateID, meshInit.MeshTemplate);
                        return;
                    }

                    var data = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.MeshTemplate, meshTemplateFile);
                    if (data == null)
                    {
                        OutputInfo(parentSourceInfoStr + "找不到文件 " + meshTemplateFile, Brushes.Red);
                        mCheckedResource.Add(meshInit.MeshTemplateID, meshInit.MeshTemplate);
                        return;
                    }
                    AddResource(data, parentSourceInfoStr, resourceList);

                    if (meshInit.MeshTemplate != null)
                    {
                        var meshTemplatePreStr = parentSourceInfoStr + "\r\n MeshTemplate: " + meshInit.MeshTemplate.NickName + "(" + meshTemplateFile + ")";

                        #region MeshTemplate用的动作文件

                        if (!string.IsNullOrEmpty(meshInit.MeshTemplate.ActionName))
                        {
                            var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Action, meshInit.MeshTemplate.ActionName);
                            if (resData == null)
                            {
                                OutputInfo(meshTemplatePreStr + " 找不到文件 " + meshInit.MeshTemplate.ActionName, Brushes.Red);
                            }
                            else
                                AddResource(resData, meshTemplatePreStr, resourceList);
                        }

                        #endregion
                        #region MeshTemplate使用的模型文件

                        foreach (var meshPart in meshInit.MeshTemplate.MeshInitList)
                        {
                            var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.MeshSource, meshPart.MeshName);
                            if (resData == null)
                            {
                                OutputInfo(meshTemplatePreStr + " 找不到文件 " + meshPart.MeshName, Brushes.Red);
                                continue;
                            }
                            AddResource(resData, meshTemplatePreStr, resourceList);

                            // 碰撞模型
                            resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.SimMeshSource, meshPart.MeshName + CSUtility.Support.IFileConfig.SimpleMeshExtension);
                            if (resData != null)
                            {
                                AddResource(resData, meshTemplatePreStr, resourceList);
                                // 碰撞模型默认发布
                                //resData.IsChecked = true;
                                resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                            }

                            // 寻路模型不需要放到最终发布版
                            //resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.PathMeshSource, meshPart.MeshName + CSUtility.Support.IFileConfig.Instance.PathMeshExtension);
                            //if (resData != null)
                            //    AddResource(resData, meshTemplatePreStr, resourceList);

                            // Socket
                            resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Socket, meshPart.MeshName + CSUtility.Support.IFileConfig.MeshSocketExtension);
                            if (resData != null)
                            {
                                AddResource(resData, meshTemplatePreStr, resourceList);
                                //resData.IsChecked = true;
                                resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                            }

                            // 分析模型材质
                            foreach (var techId in meshPart.Techs)
                            {
                                if (techId != Guid.Empty)
                                    AnalyseTechniqueResource(techId, meshTemplatePreStr, resourceList);
                            }
                        }
                        #endregion
                        #region Socket
                        mesh.SocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent item, object arg) =>
                        {
                            if (item is CCore.Socket.ISocketComponentPublisherRes)
                            {
                                var socketRes = item as CCore.Socket.ISocketComponentPublisherRes;
                                switch(socketRes.ResourceType)
                                {
                                    case CSUtility.Support.enResourceType.MeshTemplate:
                                        AnalyseMeshResource((CCore.Mesh.Mesh)socketRes.Param[0], parentSourceInfoStr + " Mesh " + meshInit.MeshTemplate.NickName + " Socket:", resourceList);
                                        break;
                                    case CSUtility.Support.enResourceType.Effect:
                                        AnalyseEffectResource((Guid)socketRes.Param[0], meshTemplatePreStr, resourceList);
                                        break;
                                    case CSUtility.Support.enResourceType.Technique:                                        
                                        AnalyseTechniqueResource((Guid)socketRes.Param[0], meshTemplatePreStr, resourceList);
                                        break;
                                }
                            }                     
                            return CSUtility.Support.EForEachResult.FER_Continue;
                        }, null);                                         
                        #endregion
                    }
                    mCheckedResource.Add(meshInit.MeshTemplateID, meshInit.MeshTemplate);
                }
            }
        }
        private void AnalyseEffectResource(Guid effectId, string parentSourceInfoStr, ObservableCollection<ResourceData> resourceList = null)
        {
            if (effectId == Guid.Empty)
                return;

            if (mCheckedResource.ContainsKey(effectId))
                return;

            var effectTemplate = CCore.Effect.EffectManager.Instance.FindEffectTemplate(effectId);

            mCheckedResource.Add(effectId, effectTemplate);

            var effectFile = CCore.Effect.EffectManager.Instance.GetEffectTemplateFile(effectId);
            if (string.IsNullOrEmpty(effectFile))
            {
                OutputInfo(parentSourceInfoStr + " 找不到Effect " + effectId, Brushes.Red);
                return;
            }
            var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Effect, effectFile);
            if (resData == null)
            {
                OutputInfo(parentSourceInfoStr + " 找不到Effect文件 " + effectFile, Brushes.Red);
                return;
            }
            AddResource(resData, parentSourceInfoStr, resourceList);

            // Effect使用的模型
            if (effectTemplate != null)
            {
                foreach (var modifier in effectTemplate.Modifiers)
                {
                    AnalyseMeshResource(modifier.MeshTemplateId, parentSourceInfoStr + "\r\n Effect: " + effectTemplate.NickName + " (" + effectFile + ")", resourceList);

                    modifier.Cleanup();
                }
            }
        }
        private void AnalyseMaterialResource(Guid matId, string parentSourceInfoStr, ObservableCollection<ResourceData> resourceList = null)
        {
            if (mCheckedResource.ContainsKey(matId))
                return;

            var matRelFile = CCore.Engine.Instance.Client.Graphics.MaterialMgr.FindMaterialFile(matId,
                                                SourceFolder + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
            var matFile = SourceFolder + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matRelFile;
            if (string.IsNullOrEmpty(matFile))
            {
                OutputInfo(parentSourceInfoStr + " Terrain 找不到Material " + matId, Brushes.Red);
                return;
            }

            var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Material, matFile);
            if (resData == null)
            {
                OutputInfo(parentSourceInfoStr + " 找不到文件 " + matFile, Brushes.Red);
            }
            else
            {
                AddResource(resData, parentSourceInfoStr, resourceList);

                var tempRelFile = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(matId);
                if (!string.Equals(tempRelFile, matRelFile))
                {
                    CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryFileValue(matId, matRelFile);
                    CCore.Engine.Instance.Client.Graphics.MaterialMgr.SaveFileDictionary();
                }
            }

            var matFileInfo = new EditorCommon.Material.MaterialFileInfo();
            matFileInfo.LoadMaterialFile(matFile);
            mCheckedResource.Add(matId, matFileInfo);

            var materialPreStr = parentSourceInfoStr + "\r\n Material: " + matFileInfo.MaterialName + "(" + matFile + ")";

            // 分析默认材质中的贴图
            foreach (var shaderVar in matFileInfo.DefaultTechnique.ShaderVarInfos)
            {
                if (shaderVar.VarType == "texture")
                {
                    if (string.IsNullOrEmpty(shaderVar.VarValue))
                    {
                        OutputInfo(materialPreStr + " " + shaderVar.VarName + " 未设置", Brushes.Red);
                    }
                    else
                    {
                        if (CSUtility.Support.IFileManager.Instance.GetFileExtension(shaderVar.VarValue) != "png")
                        {
                            OutputInfo(materialPreStr + " 错误纹理格式 " + shaderVar.VarValue, Brushes.Red);
                        }
                        var texResData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Texture, shaderVar.VarValue);
                        if (texResData == null)
                        {
                            OutputInfo(materialPreStr + " 找不到文件 " + shaderVar.VarValue, Brushes.Red);
                        }
                        else
                        {
                            AddResource(texResData, materialPreStr, resourceList);
                        }
                    }
                }
            }
        }
        private void AnalyseTechniqueResource(Guid techId, string parentSourceInfoStr, ObservableCollection<ResourceData> resourceList = null)
        {
            if (techId == Guid.Empty)
                return;

            if (mCheckedResource.ContainsKey(techId))
                return;

            var techFile = CCore.Engine.Instance.Client.Graphics.MaterialMgr.FindTechniqueFile(techId,
                                                SourceFolder + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
            if (string.IsNullOrEmpty(techFile))
            {
                OutputInfo(parentSourceInfoStr + " 找不到Technique " + techId, Brushes.Red);
                return;
            }

            var techSrcFile = techFile;
            techFile = SourceFolder + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + techFile;
            var matGuid = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetMatGuidFromTechFile(techFile);            
            var matFile = CCore.Engine.Instance.Client.Graphics.MaterialMgr.FindMaterialFile(matGuid,SourceFolder + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
            var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Technique, techFile);
            if (resData == null)
            {
                OutputInfo(parentSourceInfoStr + " 找不到文件 " + techFile, Brushes.Red);
                return;
            }
            AddResource(resData, parentSourceInfoStr, resourceList);

            var tempRelFile = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(techId);
            if (!string.Equals(tempRelFile, techSrcFile))
            {
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryFileValue(techId, techSrcFile);
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryOwnerIdValue(techId, matGuid);
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.SaveFileDictionary();
            }

            matFile = SourceFolder + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
            resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Material, matFile);
            if (resData == null)
            {
                OutputInfo(parentSourceInfoStr + " 找不到文件 " + matFile, Brushes.Red);
            }
            else
            {
                AddResource(resData, parentSourceInfoStr, resourceList);
            }

            EditorCommon.Material.MaterialFileInfo matFileInfo;
            object matFileInfoObj = mCheckedResource.FindObj(matGuid);
            if (matFileInfoObj == null)
            {
                matFileInfoObj = new EditorCommon.Material.MaterialFileInfo();
                matFileInfo = matFileInfoObj as EditorCommon.Material.MaterialFileInfo;
                matFileInfo.LoadMaterialFile(matFile);
                mCheckedResource.Add(matGuid, matFileInfoObj);

                var materialPreStr = parentSourceInfoStr + "\r\n Material: " + matFileInfo.MaterialName + "(" + matFile + ")";

                // 分析默认材质中的贴图
                foreach (var shaderVar in matFileInfo.DefaultTechnique.ShaderVarInfos)
                {
                    if (shaderVar.VarType == "texture")
                    {
                        if (string.IsNullOrEmpty(shaderVar.VarValue))
                        {
                            OutputInfo(materialPreStr + " " + shaderVar.VarName + " 未设置", Brushes.Red);
                        }
                        else
                        {
                            if (CSUtility.Support.IFileManager.Instance.GetFileExtension(shaderVar.VarValue) != "png")
                            {
                                OutputInfo(materialPreStr + " 错误纹理格式 " + shaderVar.VarValue, Brushes.Red);
                            }
                            var texResData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Texture, shaderVar.VarValue);
                            if (texResData == null)
                            {
                                OutputInfo(materialPreStr + " 找不到文件 " + shaderVar.VarValue, Brushes.Red);
                            }
                            else
                            {
                                AddResource(texResData, materialPreStr, resourceList);
                            }
                        }
                    }
                }
            }
            matFileInfo = matFileInfoObj as EditorCommon.Material.MaterialFileInfo;

            var techInfo = new EditorCommon.Material.MaterialTechniqueInfo();
            techInfo.Load(techFile);
            
            mCheckedResource.Add(techId, techInfo);

            var techPreStr = parentSourceInfoStr + "\r\n Material: " + matFileInfo.MaterialName + "(" + matFile + ")" + "\r\n Tech: " + techInfo.Name + "(" + techFile + ")";

            // 分析材质中的贴图
            foreach (var shaderVar in techInfo.ShaderVarInfos)
            {
                if (shaderVar.VarType == "texture")
                {
                    if (string.IsNullOrEmpty(shaderVar.VarValue))
                    {
                        OutputInfo(techPreStr + " " + shaderVar.VarName + " 未设置", Brushes.Red);
                    }
                    else
                    {
                        if (CSUtility.Support.IFileManager.Instance.GetFileExtension(shaderVar.VarValue) != "png")
                        {
                            OutputInfo(techPreStr + " 错误纹理格式 " + shaderVar.VarValue, Brushes.Red);
                        }
                        var texResData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Texture, shaderVar.VarValue);
                        if (texResData == null)
                        {
                            OutputInfo(techPreStr + " 找不到文件 " + shaderVar.VarValue, Brushes.Red);
                        }
                        else
                        {
                            AddResource(texResData, techPreStr, resourceList);
                        }
                    }
                }                
            }
        }
        private void AnalyseAudioSourceData(CCore.Audio.AudioSourceData audioData, string parentSourceInfoStr, ObservableCollection<ResourceData> resourceList = null)
        {
            var data = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Sound, audioData.AudioSource);
            if (data == null)
            {
                OutputInfo(parentSourceInfoStr + " 找不到文件 " + audioData.AudioSource, Brushes.Red);
                return;
            }
            AddResource(data, parentSourceInfoStr, resourceList);
        }
        private void AnalyseUVAnimResource(string parentStr, Guid uvAnimId)
        {
            if (uvAnimId == Guid.Empty)
                return;

            var uvAnim = UISystem.UVAnimMgr.Instance.Find(uvAnimId, true);
            if (uvAnim == null)
            {
                OutputInfo(parentStr + " 找不到UVAnim " + uvAnimId, Brushes.Red);
                return;
            }

            var file = UISystem.UVAnimMgr.Instance.GetUVAnimFileName(uvAnimId);
            if (string.IsNullOrEmpty(file))
            {
                OutputInfo(parentStr + " 找不到UVAnim " + uvAnimId, Brushes.Red);
                return;
            }

            var data = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.UVAnim, file);
            if (data == null)
            {
                OutputInfo(parentStr + " " + uvAnimId + " 找不到文件 " + file, Brushes.Red);
                return;
            }
            AddResource(data, parentStr);

            var unAnimPreStr = parentStr + " UVAnim: " + uvAnim.UVAnimName + "(" + file + ")";

            // UVAnim中使用的Tech
            AnalyseTechniqueResource(uvAnim.TechId, unAnimPreStr);

            // UVAnim中使用的Texture
            if (!string.IsNullOrEmpty(uvAnim.Texture))
            {
                if (CSUtility.Support.IFileManager.Instance.GetFileExtension(uvAnim.Texture) != "png")
                {
                    OutputInfo(unAnimPreStr + " 错误纹理格式 " + uvAnim.Texture, Brushes.Red);
                }
                var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Texture, uvAnim.Texture);
                if (resData == null)
                {
                    OutputInfo(unAnimPreStr + " 找不到文件 " + uvAnim.Texture, Brushes.Red);
                }
                else
                    AddResource(resData, unAnimPreStr);
            }
            UISystem.UVAnimMgr.Instance.Remove(uvAnimId);
        }
        private void AnalyseRoleTemplateResource(UInt16 roleTemplateId, string parentSourceInfoStr, ObservableCollection<ResourceData> resourceList = null)
        {
            var roleTemplate = CSUtility.Data.RoleTemplateManager.Instance.FindRoleTemplate(roleTemplateId);
            if (roleTemplate == null)
                return;
            var roleTempPreStr = parentSourceInfoStr + " RoleTemplate: " + roleTemplate.NickName + "(" + roleTemplate.Id + ")";

            CopyProgramResources(mProgressPercent, roleTemplate, roleTempPreStr);
        }

        #endregion

        #region 字体
        private void CopyFont(double progressPrecentEnd)
        {
            var folder = SourceFolder + CSUtility.Support.IFileConfig.DefaultFontDirectory;
            if (!System.IO.Directory.Exists(folder))
                return;

            var files = System.IO.Directory.EnumerateFiles(folder, "*.ttf");
            var delta = (progressPrecentEnd - ProgressPercent) / files.Count<string>();
            foreach (var file in files)
            {
                var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Font, file);
                if (resData == null)
                {
                    OutputInfo("找不到文件 " + file, Brushes.Red);
                    continue;
                }

                AddResource(resData, "");

                ProgressPercent += delta;
            }
        }

        #endregion

        #region UI
        private void CopyUI(double progressPrecentEnd)
        {
            var delta = (progressPrecentEnd - ProgressPercent) / UICtrl.UIFiles.Count;

            foreach (var uiData in UICtrl.UIFiles)
            {          
                ProgressInfo = "正在分析UI " + uiData.FileName;

                var data = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.UI, uiData.FileName);
                if (data == null)
                {                    
                    OutputInfo("找不到文件 " + uiData.FileName, Brushes.Red);
                    continue;
                }
                AddResource(data, "UI");

                var ui = CCore.Support.ReflectionManager.Instance.CreateUIFromXml(uiData.FileName) as UISystem.WinBase;
                if (ui == null)
                {
                    OutputInfo("UI文件无法打开 " + uiData.FileName, Brushes.Red);
                    continue;
                }
                AnalyseUIResource(ui, uiData.FileName);

                ProgressPercent += delta;
            }

            System.GC.Collect();
            System.GC.WaitForFullGCComplete();
            System.GC.Collect();
            System.GC.WaitForFullGCComplete();
        }

        private void AnalyseUIResource(UISystem.WinBase win, string uiFileName)
        {
            if (win == null)
                return;

            if (win.GetType() == typeof(UISystem.Button))
            {
                var btn = win as UISystem.Button;
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.NormalState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.LightState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.PressState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.DisableState.UVAnimId);
            }
            else if (win.GetType() == typeof(UISystem.ToggleButton))
            {
                var btn = win as UISystem.ToggleButton;
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.NormalState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.LightState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.CheckedState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.DisableState.UVAnimId);
            }
            else if (win.GetType() == typeof(UISystem.Joysticks.Joysticks))
            {
                var btn = win as UISystem.Joysticks.Joysticks;
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.JoysticksNormalState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.RockerLightState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.RockerPressState.UVAnimId);
                AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, btn.RockerNormalState.UVAnimId);
            }
            else
            {
                if (win.RState != null && win.RState.UVAnimId != Guid.Empty)
                {
                    AnalyseUVAnimResource("UI(" + uiFileName + "): Ctrl: " + win.WinName, win.RState.UVAnimId);
                }
            }

            foreach (var child in win.GetChildWindows())
            {
                AnalyseUIResource(child, uiFileName);
            }
        }

        #endregion

        #region Event

        // Event重新生成release的dll文件        
        private void AnalyseEvent(double progressPrecentEnd, CSUtility.Helper.enCSType csType)
        {
            var mDelegateMethodAssembly = CSUtility.Program.GetAssemblyFromDllFileName(csType, "Plugins/DelegateMethodEditor/bin/DelegateMethodEditor.dll");
            var delMethodType = mDelegateMethodAssembly?.GetType("DelegateMethodEditor.CodeGenerator.CodeGenerator");
            var delMethod = delMethodType?.GetMethod("CompileEventCodeWithEventId");
            if (delMethod != null)
            {
                CSUtility.Helper.EventCallBackManager.Instance.OnCompileEventCode =
                    (Guid id, int cstype, bool bForceCompile) =>
                    {
                        bool retValue = false;
                 
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            retValue = (bool)delMethod.Invoke(null, new object[] { id, cstype, bForceCompile, false, FullPackageFolder });
                        }));
                      
                        return retValue;
                    };
            }
   
            CSUtility.Helper.EventCallBackManager.Instance._SetCSType(csType);
            CSUtility.Helper.EventCallBackManager.Instance.TargetRootDirectory = FullPackageFolder;

            var folders = System.IO.Directory.GetDirectories(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultEventDirectory);
            if (folders.Length == 0)
                return;
            var delta = (progressPrecentEnd - ProgressPercent) / folders.Length;

            foreach (var folder in folders)
            {
                var name = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(folder);
                var id = CSUtility.Support.IHelper.GuidTryParse(name);
                if (id == Guid.Empty)
                    continue;

                ProgressInfo = "正在生成Event " + id;
                CSUtility.Helper.EventCallBackManager.Instance.LoadCallee(id, true);

                // 修改名称去除后缀
                string dllDir = CSUtility.Helper.EventCallBack.GetAssemblyFileDir(csType);
                var eventFileName = dllDir + "/" + CSUtility.Helper.EventCallBack.GetAssemblyFileName(id, csType);
           
                var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Event, eventFileName);
                if (resData == null)
                {
                    var commonFile = folder + "/" + name + ".xml";
                    var clientFile = folder + "/" + name + "_Client.xml";
                    if (System.IO.File.Exists(commonFile) || System.IO.File.Exists(clientFile))
                        OutputInfo("未能生成Event " + eventFileName, Brushes.Red);
                }
                else
                {
                    AddResource(resData, "Event");

                    if (Program.CopyToZip)
                    {
                        var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(resData.TargetFile);
                        var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(resData.TargetFile);
                        CSUtility.Compress.CompressManager.ZipFile(file, path, path + file + ".zip");
                        System.IO.File.Delete(resData.TargetFile);
                    }
                }

                ProgressPercent += delta;
            }

            CSUtility.Helper.EventCallBackManager.Instance.OnCompileEventCode = null;

            System.GC.Collect();
            System.GC.WaitForFullGCComplete();
            System.GC.Collect();
            System.GC.WaitForFullGCComplete();
        }

        #endregion

        #region FSM

        private void AnalyseFSM(double progressPrecentEnd, CSUtility.Helper.enCSType csType)
        {
            var mAIEditorAssembly = CSUtility.Program.GetAssemblyFromDllFileName(csType, "Plugins/ResourcesBrowser/bin/AIEditor.dll");
            var aiType = mAIEditorAssembly?.GetType("AIEditor.Program");
            var aiMethod = aiType?.GetMethod("CompileAICodeWithAIGuid");
            if (aiMethod != null)
            {
                CSUtility.AISystem.FStateMachineTemplate.OnCompileFSMCode =
                    (Guid id, int cstype, bool bForceCompile) =>
                    {
                        bool retValue = false;
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            retValue = (bool)aiMethod.Invoke(null, new object[] { id, cstype, bForceCompile,false, FullPackageFolder });
                        }));
                        return retValue;                        
                    };
            }
            CSUtility.AISystem.FStateMachineTemplateManager.Instance.RootDirectory = FullPackageFolder;

            var delta = progressPrecentEnd - ProgressPercent;
            AnalyseFSMInFolder(delta, csType);

            System.GC.Collect();
            System.GC.WaitForFullGCComplete();
            System.GC.Collect();
            System.GC.WaitForFullGCComplete();
        }

        private void AnalyseFSMInFolder(double processTotal, CSUtility.Helper.enCSType csType)
        {
            CSUtility.AISystem.FSMTemplateVersionManager.Instance.Load(CSUtility.Helper.enCSType.Client);

            var processDelta = processTotal / CSUtility.AISystem.FSMTemplateVersionManager.Instance.FSMTemplateVersionDictionary.Count;
            foreach (var i in CSUtility.AISystem.FSMTemplateVersionManager.Instance.FSMTemplateVersionDictionary)
            {                
                var dirs = System.IO.Directory.GetDirectories(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory, i.Key.ToString(), System.IO.SearchOption.AllDirectories);
                if (dirs.Length == 0)
                    continue;

                var fsmId = i.Key;
                foreach (var dir in dirs)
                {
                    ProgressInfo = "正在生成AI " + fsmId;

                    var fsm = new CSUtility.AISystem.FStateMachineTemplate(fsmId, csType);
                    fsm.Initialize(true);

                    CSUtility.AISystem.FStateMachineTemplateManager.Instance.GetFSMTemplate(fsmId, csType, true);

                    // 修改名称去除后缀
                    string dllDir = CSUtility.AISystem.FStateMachineTemplate.GetAssemblyDir(csType);
                    var fsmFileName = dllDir + "/" + CSUtility.AISystem.FStateMachineTemplate.GetAssemblyFileName(fsmId, csType);

                    var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.FSM, fsmFileName);
                    if (resData == null)
                    {
                        OutputInfo("未能生成AI " + fsmFileName, Brushes.Red);
                    }
                    else
                    {
                        AddResource(resData, "FSM");

                        if (Program.CopyToZip)
                        {
                            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(resData.TargetFile);
                            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(resData.TargetFile);
                            CSUtility.Compress.CompressManager.ZipFile(file, path, path + file + ".zip");
                            System.IO.File.Delete(resData.TargetFile);
                        }
                    }
                }
            }
            ProgressPercent += processDelta;
        }

        #endregion

        #region MetaData

        private void CopyMetaData(double progressPrecentEnd)
        {
            var folder = SourceFolder + CSUtility.Support.IFileConfig.MetaDataDirectory;
            if (!System.IO.Directory.Exists(folder))
                return;

            var files = System.IO.Directory.EnumerateFiles(folder);
            var delta = (progressPrecentEnd - ProgressPercent) / (double)files.Count<string>();
            foreach (var file in files)
            {
                ProgressInfo = "正在复制MetaData: " + file;

                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(file);
                if (xmlHolder == null)
                    continue;

                var relFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
                var md5 = relFile.GetHashCode();
                var fileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(file);
                var targetFile = FullPackageFolder + relFile.Replace(fileName, md5.ToString() + ".dat");
                CSUtility.Crypt.Crypt_DES.DesEncrypt(file, targetFile, CSUtility.Crypt.Crypt_DES.GetDesKey(CSUtility.Crypt.Crypt_DES.CurrentVersion));

                /*/ 加密解密测试////////////////////////////////////////////////////////////////////////
                //CSUtility.Crypt.Crypt_DES.DesEncrypt(@"D:\victory\Development\Release\Metadata\CSCommon.Animation.ActionNotifier.xml", @"D:\victory\Development\Release\Metadata\CSCommon.Animation.ActionNotifier.dat", CSUtility.Crypt.Crypt_DES.GetDesKey(CSUtility.Crypt.Crypt_DES.CurrentVersion));

                //CSUtility.Support.IXndHolder holder = CSUtility.Support.IXndHolder.LoadXND(@"D:\victory\Development\Release\Metadata\CSCommon.Animation.ActionNotifier.dat", true);
                CSUtility.Support.IXndHolder holder = CSUtility.Support.IXndHolder.LoadXND(targetFile, true);
                var att = holder.Node.FindAttrib("DataAtt");
                if (att == null)
                    return;

                att.BeginRead();

                UInt32 keyVer;
                att.Read(out keyVer);
                string str;
                att.Read(out str);

                att.EndRead();

                System.String xmlString = CSUtility.Crypt.Crypt_DES.DesDecrypt(str, CSUtility.Crypt.Crypt_DES.GetDesKey(keyVer));
                string xmlstr = xmlString;

                /////////////////////////////////////////////////////////////////////////*/

                var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Metadata, targetFile);
                if (resData == null)
                {
                    resData.CheckState = CheckBoxEx.enCheckState.AllChecked;
                    OutputInfo("找不到文件 " + file, Brushes.Red);
                    continue;
                }

                AddResource(resData, "MetaData");

                ProgressPercent += delta;
            }
        }

        #endregion

        #region Shader

        private void CopyShader(double progressPrecentEnd)
        {
            var folder = SourceFolder + CSUtility.Support.IFileConfig.DefaultShaderDirectory;
            var tagFolder = FullPackageFolder + CSUtility.Support.IFileConfig.DefaultShaderDirectory;

            List<string> exceptFolder = new List<string>() {
                SourceFolder + CSUtility.Support.IFileConfig.DefaultShaderDirectory + "/Material"                
            };
            CopyDirectory(folder, tagFolder, CSUtility.Support.enResourceType.Shader, exceptFolder);

            var srcFile = SourceFolder + CSUtility.Support.IFileConfig.DefaultShaderDirectory + "/Material" + "/LayerBasedMaterial.mtl";
            ProgressInfo = "正在拷贝文件" + srcFile;
            var tagFile = Program.CopyFile(srcFile, Program.CopyToZip);
            var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Material, srcFile);
            AddResource(resData, "Shader");      
        }

        #endregion

        #region Template

        private void CopyTemplate(double progressPrecentEnd)
        {
            var folder = SourceFolder + CSUtility.Support.IFileConfig.DefaultTemplateDir;
            var tagFolder = FullPackageFolder + CSUtility.Support.IFileConfig.DefaultTemplateDir;

            var delta = (progressPrecentEnd - ProgressPercent);
            var progressDelta = delta * 0.3;

            CopyDirectory(folder, tagFolder, CSUtility.Support.enResourceType.Template, null);//new List<string>() { folder + "/PerformOptions/0.po" });
            ProgressPercent += progressDelta;

            Dictionary<string, Type> templateDic = new Dictionary<string, Type>();
            var assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Client.dll");
            if (assembly == null)
                return;

            var aTypes = assembly.GetTypes();
            foreach (var type in aTypes)
            {
                var atts = type.GetCustomAttributes(typeof(CSUtility.Editor.CDataEditorAttribute), true);

                if (atts.Length > 0)
                {
                    CSUtility.Editor.CDataEditorAttribute dea = atts[0] as CSUtility.Editor.CDataEditorAttribute;
                    if(dea != null)
                    {
                        templateDic[dea.m_strFileExt] = type;
                    }
                }
            }

            foreach (var file in System.IO.Directory.GetFiles(tagFolder,"*",System.IO.SearchOption.AllDirectories))
            {
                var ext = "." + CSUtility.Support.IFileManager.Instance.GetFileExtension(file);
                if (templateDic.ContainsKey(ext))
                {
                    var template = System.Activator.CreateInstance(templateDic[ext]);
                    if (CSUtility.Support.IConfigurator.FillProperty(template, file))
                    {
                        CopyProgramResources(progressPrecentEnd, template);
                    }
                }
            }

            // 角色模板
            CSUtility.Data.RoleTemplateManager.Instance.RoleFileNames.For_Each((UInt16 key, string value, object argObj) =>
            {
                var srcFileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(value);
                Program.CopyFile(value,false);
                var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.RoleTemplate,srcFileName);
                if (resData != null)
                    AddResource(resData, "");

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
            foreach (var i in CSUtility.Data.RoleTemplateManager.Instance.LoadAllRoleTemplate())
            {
                CopyProgramResources(progressPrecentEnd, i);
            }            
        }

        #endregion

        #region Sound

        private void CopySounds(double progressPrecentEnd)
        {
//             var folder = SourceFolder + CSUtility.Support.IFileConfig.Instance.DefaultSoundDirectory;
//             var tagFolder = FullPackageFolder + CSUtility.Support.IFileConfig.Instance.DefaultSoundDirectory;
// 
//             var progressDelta = (progressPrecentEnd - ProgressPercent) * 0.3;
// 
//             CopyDirectory(folder, tagFolder, CSUtility.Support.enResourceType.Sound, null);
//             ProgressPercent += progressDelta;
        }

        #endregion
        #region FileCache

        private void CopyFileCache(double progressPrecentEnd)
        {
            var delta = (progressPrecentEnd - ProgressPercent);

            ProgressInfo = "正在复制文件Cache";

            // 材质文件cache
            var absFile = SourceFolder + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + CSUtility.Support.IFileConfig.DefaultMaterialFileDictionaryFile;
            var resData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Config, absFile);
            if (resData != null)
            {
                AddResource(resData, "");
                ProgressPercent += delta;
            }
            else
            {
                OutputInfo("找不到文件 " + absFile, Brushes.Red);
            }

            var assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Client.dll");
            if (assembly == null)
                return;

            var type = assembly.GetType("GameData.Support.ConfigFile");
            if (type != null)
            {
                var config = System.Activator.CreateInstance(type);
                if (config != null)
                {
                    CopyProgramResources(progressPrecentEnd, config);
                }
            }
        }
        #endregion

        #region 地图

        string[] mapCopyFiles_Client = new string[]{
            "Actors.dat",
            "camera.dat",
            "common.dat",
            "Config.map",
            "lights.dat",
            "PostProcess.dat",
        };
        string[] mapCopyFolder_Client = new string[]{
            "Navigation",
            "Scene",
            "ScenePoint",
            "Terrain",
            "Trigger"
        };      
        private void CopyMaps(double progressPercentEnd)
        {
            var delta = (progressPercentEnd - ProgressPercent) / MapResPanel.ReleaseItems.Count;
            foreach (var mapItem in MapResPanel.ReleaseItems)
            {
                var tagMapFolder = mapItem.MapFolder.Replace(SourceFolder, FullPackageFolder);
                
                CopyDirectory(mapItem.MapFolder, tagMapFolder, CSUtility.Support.enResourceType.MapFiles, null);

                ObservableCollection<ResourceData> mapResources = new ObservableCollection<ResourceData>();

                //分析Map文件提取需要的资源
                AnalyseMapResource(delta, mapItem, ref mapResources);

                System.GC.Collect();
                System.GC.WaitForFullGCComplete();
                System.GC.Collect();
                System.GC.WaitForFullGCComplete();

                //将地图引用的文件拍平后存储到地图目录下
//                 var mapResList = ResourceDataManager.GetResourceDataListFromTree(
//                     mapResources.ToArray<ResourceData>(),
//                     new CSUtility.Support.enResourceType[]{
//                         CSUtility.Support.enResourceType.Texture,
//                         CSUtility.Support.enResourceType.MeshSource,
//                 });
//                 ResourceDataManager.SaveSimpleResourceData(tagMapFolder + "/file.dat", mapResList.ToArray<ResourceData>());
// 
//                 this.Dispatcher.Invoke(new Action(() =>
//                 {
//                     OutPutControl.AddMapInfoTab(mapItem, mapResources);
//                 }));
// 
//                 foreach (var data in mapResources)
//                 {
//                     AddResource(data, "Map:" + mapItem.MapName);
//                 }
            }

//             System.GC.Collect();
//             System.GC.WaitForFullGCComplete();
//             System.GC.Collect();
//             System.GC.WaitForFullGCComplete();
        }
        //分析地图所有的资源
        CCore.World.World mCurMapWorld = null;
        private void AnalyseMapResource(double progressDelta, MapResourceItem mapItem, ref ObservableCollection<ResourceData> mapResources)
        { 
            var worldInit = new CCore.World.WorldInit();
            worldInit.SceneGraphInfo = new CCore.Scene.TileScene.TileSceneInfo();

            var absFileName = CSUtility.Map.MapManager.Instance.GetMapPath(mapItem.MapId);
            worldInit.Load(absFileName);
            CCore.Engine.Instance.Client.MainWorld.Cleanup();
            var newWorld = new CCore.World.World(mapItem.MapId);            
            newWorld.Initialize(worldInit);
            newWorld.Initialize(absFileName);
            newWorld.AnalyseLoadWorld(absFileName);

            newWorld.AnalyseLoadWorld(newWorld.GetWorldLastLoadedAbsFolder("场景"), "种植NPC");
            mCurMapWorld = newWorld;
            //分析Scene------------------------------------------------------
            var scene = newWorld.SceneGraph as CCore.Scene.TileScene.TileScene;
            var delta = progressDelta * 0.8 / (mCurMapWorld.Terrain.GetLevelXCount() * mCurMapWorld.Terrain.GetLevelZCount());
            //             foreach (var actor in scene.AllTileObjects.Values)
            //             {
            //                 AnalyseActorResource(mapItem, actor, ref mapResources);
            //                 ProgressPercent += delta;
            //             }

            for (int i = 0; i < mCurMapWorld.Terrain.GetLevelXCount(); ++i)
            {
                for (int j = 0; j < mCurMapWorld.Terrain.GetLevelZCount(); ++j)
                {
                    var x = i * mCurMapWorld.Terrain.GetXLengthPerLevel() + mCurMapWorld.Terrain.GetXLengthPerLevel() / 2f;
                    var z = j * mCurMapWorld.Terrain.GetZLengthPerLevel() + mCurMapWorld.Terrain.GetZLengthPerLevel() / 2f;
                    mCurMapWorld.TravelTo(x, z);
                    mCurMapWorld.Tick();

                    foreach (var actor in scene.AllTileObjects.Values)
                    {
                        AnalyseActorResource(mapItem, actor, ref mapResources);                        
                    }
                    ProgressPercent += delta;
                }
            }

            //分析地形--------------------------------------------------------
            //设置草加载Mesh的回调

            List<Guid> matIdList = new List<Guid>();
            List<System.IntPtr> grassList = new List<System.IntPtr>();
            List<string> remarkList = new List<string>();
            newWorld.Terrain.GetLayerMaterials(matIdList, grassList, remarkList);

            delta = progressDelta * 0.2 / matIdList.Count;

            for (int i = 0; i < matIdList.Count; i++)
            {
                var matId = matIdList[i];
                if (matId == Guid.Empty)
                    continue;

                //地形材质
                AnalyseMaterialResource(matId, "Map: " + mapItem.MapName + "-Terrain-" + remarkList[i], mapResources);

                //草
                //             var grass = new FrameSet.Grass.GrassData(grassList[i]);
                //             AnalyseMeshResource(grass.MeshTemplateId, "Map: " + mapItem.MapName + "-Terrain-" + remarkList[i], mapResources);
                //             grass.Cleanup();

                ProgressPercent += delta;
            }
            newWorld.Terrain.Cleanup();
        }

        private void AnalyseActorResource(MapResourceItem mapItem, CCore.World.Actor actor, ref ObservableCollection<ResourceData> resourceList)
        {
            try
            {
                if (mCheckedResource.ContainsKey(actor.Id))
                    return;

                //缓存Actor会造成大量的内存使用导致内存溢出，这里不再缓存
                mCheckedResource[actor.Id] = actor;
                mCheckedResource.Add(actor.Id, null);

                if (actor is CCore.World.DecalActor)
                {
                    var decalActor = actor as CCore.World.DecalActor;
                    AnalyseTechniqueResource(decalActor.TechId, "Map DecalActor: " + mapItem.MapName, resourceList);
                }
                else if (actor is CCore.World.EffectActor)
                {
                    var effectVisual = actor.Visual as CCore.Component.EffectVisual;
                    AnalyseEffectResource(effectVisual.EffectTemplateID, "Map: " + mapItem.MapName, resourceList);
                }
                else if (actor is CCore.World.Role.NPCInitializerActor)
                {
                    var actorInit = actor.ActorInit as CSUtility.Map.Role.NPCInitializerActorInit;
                    AnalyseRoleTemplateResource(actorInit.NPCData.Template.Id, "Map: " + mapItem.MapName, resourceList);
                }
                else if (actor is CCore.World.MeshActor)
                {
                    if (actor.Visual is CCore.Component.EffectVisual)
                    {
                        AnalyseEffectResource((actor.Visual as CCore.Component.EffectVisual).EffectTemplateID, "Map: " + mapItem.MapName, resourceList);
                    }
                    else if (actor.Visual is CCore.Mesh.Mesh)
                    {
                        AnalyseMeshResource(actor.Visual as CCore.Mesh.Mesh, "Map: " + mapItem.MapName, resourceList);
                    }
                }
                else if (actor is CCore.World.AudioActor)
                {
                    var audioActor = actor as CCore.World.AudioActor;
                    AnalyseAudioSourceData(audioActor.SourceData, "Map: " + mapItem.MapName, resourceList);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        private void ZipCopyFolder(string srcFolder, string tagFolder, Dictionary<string, CSUtility.FileDownload.ResourceData> resPublishedDic, Dictionary<string, CSUtility.FileDownload.ResourceData> resNewDic)
        {
            if (srcFolder[srcFolder.Length - 1] != '/' && srcFolder[srcFolder.Length - 1] != '\\')
                srcFolder += "/";
            //var tagFolder = srcFolder + "Zip/";
            if (!System.IO.Directory.Exists(tagFolder))
                System.IO.Directory.CreateDirectory(tagFolder);

            foreach (var file in System.IO.Directory.EnumerateFiles(srcFolder))
            {
                ProgressInfo = "正在压缩文件 " + file;
                var fileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(file);
                var targetZipFile = tagFolder + fileName + ".zip";

                if (CompareFinalPublish)
                {
                    var relativeFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file, FullPackageFolder);
                    CSUtility.FileDownload.ResourceData dataPublished;
                    if (resPublishedDic.TryGetValue(relativeFileName, out dataPublished))
                    {
                        CSUtility.FileDownload.ResourceData dataNew;
                        if (resNewDic.TryGetValue(relativeFileName, out dataNew))
                        {
                            var finalFile = FinalPublishFolder + relativeFileName + ".zip";
                            if (System.IO.File.Exists(finalFile) && dataNew.MD5 == dataPublished.MD5)
                            {
                                // 文件与发布版本一样，直接从发布版本拷贝
                                System.IO.File.Copy(finalFile, FullPackageFolder.Remove(FullPackageFolder.Length - 1) + "Zip/" + relativeFileName + ".zip");
                            }
                            else
                                CSUtility.Compress.CompressManager.ZipFile(fileName, srcFolder, targetZipFile);
                        }
                        else
                        {
                            //System.Diagnostics.Debugger.Break();
                            CSUtility.Compress.CompressManager.ZipFile(fileName, srcFolder, targetZipFile);
                        }
                    }
                    else
                        CSUtility.Compress.CompressManager.ZipFile(fileName, srcFolder, targetZipFile);
                }
                else
                {
                    CSUtility.Compress.CompressManager.ZipFile(fileName, srcFolder, targetZipFile);
                }

                System.GC.Collect();
                System.GC.WaitForFullGCComplete();
                System.GC.Collect();
                System.GC.WaitForFullGCComplete();
            }

            foreach (var dir in System.IO.Directory.EnumerateDirectories(srcFolder))
            {
                var dirName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(dir);
                ZipCopyFolder(dir, tagFolder + dirName + "/", resPublishedDic, resNewDic);
            }
        }
        private Int64 GetDirectorySize(string folder)
        {
            Int64 len = 0;

            if (!System.IO.Directory.Exists(folder))
                return len;

            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(folder);

            foreach (var fi in info.GetFiles())
            {
                len += fi.Length;
            }

            foreach (var dir in info.GetDirectories())
            {
                len += GetDirectorySize(dir.FullName);
            }

            return len;
        }
    }
}
