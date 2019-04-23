using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using EditorCommon.PluginAssist;
using System.IO;
using System.Windows.Media;
using System.Windows.Documents;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Linq;
using CSUtility.Support;

namespace FilesLeadToOtherEditor
{
    /// <summary>
    /// Interaction logic for FileLeadToOther.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "FilesLeadToOther")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/adb导入资源工具")]
    [Guid("07f56ea9-95bb-428f-8e36-1ec6a177563b")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class FilesLeadToOther : System.Windows.Controls.UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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

        public string PluginName
        {
            get { return "adb导入资源工具"; }
        }

        public string Version
        {
            get { return "1.0.0"; }
        }

        public string AssemblyPath
        {
            get { return this.GetType().Assembly.Location; }
        }

        UIElement mInstructionControl = new TextBlock()
        {
            Text = "adb导入资源工具",
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public UIElement InstructionControl
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

        ///////////////////////////////////////////////////////////

        string cmdPath = @"c:\windows\system32\cmd.exe";        // CMD的启动路径
        string mLogNamePath = "";                               //日志路径     

        string mSourcePathName;                                 // 源文件夹路径
        string mSourcePathNameLower;                            // 源文件夹路径的小写形式
        string mDestPathName;                                   // 目标文件夹名字

        string mDestSavePath;                                   // 目标保存路径
        string mErrorInformation;                               // 错误信息

        string mSourcePath;                                     // 源文件的路径
        string mSourceFileName;                                 // 源文件的名称

        string mTmpFileName = "_fileList";                      // 存储所需文件名的临时文件的名字
        string mTmpFileFoderName = "fileFoder";                 // 存储所需文件夹的临时文件的名字

        string output;                                          // 接受cmd消息的返回值

        int mTotalFileNum;                                      // 总文件的数量
        int mLoadFileNum;                                       // 已经上传的文件数量
        int mAddFileNum;                                        // 添加的文件数量
        int mModifiedFileNum;                                   // 由于本地修改而上传的文件数量
        int mIgnoreFileNum;                                     // 没有改变所有没有进行任何操作的文件数量
        int mFaildFileNum;                                      // 上传失败的文件数量
        int mDeleteFileNum;                                     // 由于本地已经删除而删除的文件数量

        bool isStart;                                           // 判断程序是否处于运行状态的标志位
        bool isStop;                                            // 判断是否按下停止的标志位

        Dictionary<string, string> mSourceFileList = new Dictionary<string, string>();             // 保存源文件的文件名及其MD5码
        List<string> mSourceFileFoderList = new List<string>();                      // 保存源文件的文件夹名字以便删除操作

        Dictionary<string, string> mDestFileList = new Dictionary<string, string>();               // 目标文件中的所有文件列表
        List<string> mDestFileFoderList = new List<string>();                        // 目标文件中所有的文件夹列表

        FiltersWindows mFiltersWindow = null;
        public FiltersWindows FiltersWindow
        {
            get { return mFiltersWindow; }
        }

        //过滤后缀名列表
        StringCollection mFilterFileLines = new StringCollection();
        public StringCollection FilterFileLines
        {
            get { return mFilterFileLines; }
        }

        //过滤文件夹列表
        StringCollection mFilterFolderLines = new StringCollection();
        public StringCollection FilterFolderLines
        {
            get { return mFilterFolderLines; }
        }

        //过滤文件相似列表
        StringCollection mFilterLikeFileLines = new StringCollection();
        public StringCollection FilterLikeFileLines
        {
            get { return mFilterLikeFileLines; }
        }

        public FilesLeadToOther()
        {
            InitializeComponent();
            
            FilterName.TextChanged += FilterName_TextChanged;
            mLogNamePath = CSUtility.Support.IFileManager.Instance.Root + "AdbConfig.cfg";
            if (!LoadPathName(mLogNamePath))
            {
                var path = CSUtility.Support.IFileManager.Instance.Root;
                SourceName.Text = path.Substring(0, path.Length - 1).Replace("/", "\\");

                FilterName.Text = ".rinfo@;" + "_Snapshot.png@;";
                //mFilterLikeFileLines.Add(".mtl.rinfo");
                //mFilterLikeFileLines.Add(".mtl_Snapshot.png");
            }
            ProgressBar_Copy.Visibility = Visibility.Collapsed;
        }

        private void FilterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            mFilterFileLines.Clear();
            mFilterFolderLines.Clear();
            mFilterLikeFileLines.Clear();
            var filters = FilterName.Text.Split(';');
            foreach (var child in filters)
            {
                //去除所有转义符
                var s = new string((from c in child.ToCharArray() where !char.IsControl(c) select c).ToArray());
                if (s.EndsWith("/"))
                {
                    var name = s.Substring(0, child.Length - 1);
                    mFilterFolderLines.Add(name);
                }
                else if (s.EndsWith("@"))
                {
                    var name = s.Substring(0, child.Length - 1);
                    mFilterLikeFileLines.Add(name);
                }
                else
                {
                    mFilterFileLines.Add(s);
                }
            }
        }

        private bool LoadPathName(string pathName)
        {
            if (pathName == string.Empty)
                return false;

            var xmlHolder = XmlHolder.LoadXML(pathName);
            if (xmlHolder == null)
                return false;
            var node = xmlHolder.RootNode.FindNode("SourcePathName");
            if (node != null)
            {
                var att = node.FindAttrib("Value");
                if (att != null)
                    SourceName.Text = att.Value;
            }
            node = xmlHolder.RootNode.FindNode("DestPathName");
            if (node != null)
            {
                var att = node.FindAttrib("Value");
                if (att != null)
                    DestName.Text = att.Value;
            }
            node = xmlHolder.RootNode.FindNode("FilterName");
            if (node != null)
            {
                var att = node.FindAttrib("Value");
                if (att != null)
                    FilterName.Text = att.Value;
            }
            return true;
        }

        private bool SavePathName(string pathName)
        {
            if (pathName == string.Empty)
                return false;

            var xmlHolder = XmlHolder.NewXMLHolder("Ver", "0");
            var node = xmlHolder.RootNode.AddNode("SourcePathName", "", xmlHolder);
            node.AddAttrib("Value", mSourcePathName);
            node = xmlHolder.RootNode.AddNode("DestPathName", "", xmlHolder);
            node.AddAttrib("Value", mDestPathName);
            node = xmlHolder.RootNode.AddNode("FilterName", "", xmlHolder);
            node.AddAttrib("Value", FilterName.Text);

            XmlHolder.SaveXML(pathName, xmlHolder, true);
            return true;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (!isStart)
            {
                mTotalFileNum = 0;
                mLoadFileNum = 0;
                mAddFileNum = 0;
                mModifiedFileNum = 0;
                mIgnoreFileNum = 0;
                mFaildFileNum = 0;
                mDeleteFileNum = 0;
                isStart = true;
                isStop = false;

                StartButton.Background = Brushes.Gray;
                mDestPathName = DestName.Text;
                mSourcePathName = SourceName.Text;
                mSourcePathNameLower = mSourcePathName.ToLower();

                TotalSum.Text = mTotalFileNum.ToString();
                LoadOnSum.Text = mLoadFileNum.ToString();
                AddFiles.Text = mAddFileNum.ToString();
                Modified.Text = mModifiedFileNum.ToString();
                Ignore.Text = mIgnoreFileNum.ToString();
                FaildNum.Text = mFaildFileNum.ToString();
                DeleteNum.Text = mDeleteFileNum.ToString();

                ListBox_Add.Items.Clear();
                ListBox_Failed.Items.Clear();

                mSourceFileFoderList.Clear();
                mSourceFileList.Clear();
                mDestFileFoderList.Clear();
                mDestFileList.Clear();

                if (mSourcePathName.Length != 0)
                {
                    mSourceFileName = System.IO.Path.GetFileName(mSourcePathName);
                    mSourcePath = mSourcePathName.Replace(mSourceFileName, "");
                    string testPath = System.IO.Path.GetPathRoot(mSourcePathName);

                    StartCopy();

                    SavePathName(mLogNamePath);
                }
                else
                {
                    System.Windows.MessageBox.Show("需要指定源文件路径", "错误");
                    isStart = false;
                    StartButton.Background = Brushes.White;
                }
            }
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            isStart = false;
            isStop = true;
            StartButton.Background = Brushes.White;
        }
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult mResult = openFile.ShowDialog();
            if (mResult == System.Windows.Forms.DialogResult.OK)
                this.SourceName.Text = openFile.SelectedPath.ToString();

            mSourcePathName = SourceName.Text;
        }

        private void Filters_Click(object sender, RoutedEventArgs e)
        {
            mFiltersWindow = new FiltersWindows(this);
            
            string filterFileStrs = "";
            for (int i = 0; i < mFilterFileLines.Count; i++)
            {
                if (mFilterFileLines[i] != string.Empty)
                    filterFileStrs += mFilterFileLines[i] + '\n';
            }
            mFiltersWindow.ExcludeFilesTextBox.Text = filterFileStrs;

            string filterFolderStrs = "";
            for (int i = 0; i < mFilterFolderLines.Count; i++)
            {
                if (mFilterFolderLines[i] != string.Empty)
                    filterFolderStrs += mFilterFolderLines[i] + '\n';
            }
            mFiltersWindow.ExcludeFoldersTextBox.Text = filterFolderStrs;

            string filterLikeFileStrs = "";
            for (int i = 0; i < mFilterLikeFileLines.Count; i++)
            {
                if (mFilterLikeFileLines[i] != string.Empty)
                    filterLikeFileStrs += mFilterLikeFileLines[i] + '\n';
            }
            mFiltersWindow.ExcludeLikeFilesTextBox.Text = filterLikeFileStrs;
            
            mFiltersWindow.ShowDialog();
        }

        public void AddFilters()
        {
            if (mFiltersWindow == null)
                return;

            string excludeFilters = "";
            mFilterFileLines.Clear();
            for (int line = 0; line < mFiltersWindow.ExcludeFilesTextBox.LineCount; line++)
            {
                var lineStr = mFiltersWindow.ExcludeFilesTextBox.GetLineText(line);
                var s = new string((from c in lineStr.ToCharArray() where !char.IsControl(c) select c).ToArray());
                if (s != string.Empty)
                {
                    excludeFilters += s + ';';
                    mFilterFileLines.Add(s);
                }
            }

            mFilterFolderLines.Clear();
            for (int line = 0; line < mFiltersWindow.ExcludeFoldersTextBox.LineCount; line++)
            {
                var lineStr = mFiltersWindow.ExcludeFoldersTextBox.GetLineText(line);
                var s = new string((from c in lineStr.ToCharArray() where !char.IsControl(c) select c).ToArray());
                if (s != string.Empty)
                {
                    excludeFilters += s + "/;";
                    mFilterFolderLines.Add(s);
                }
            }

            mFilterLikeFileLines.Clear();
            for (int line = 0; line < mFiltersWindow.ExcludeLikeFilesTextBox.LineCount; line++)
            {
                var lineStr = mFiltersWindow.ExcludeLikeFilesTextBox.GetLineText(line);
                var s = new string((from c in lineStr.ToCharArray() where !char.IsControl(c) select c).ToArray());
                if (s != string.Empty)
                {
                    excludeFilters += s + "@;";
                    mFilterLikeFileLines.Add(s);
                }
            }

            FilterName.Text = excludeFilters;
        }

        private void StartCopy()
        {
            mSourceFileFoderList.Clear();
            mSourceFileList.Clear();

            DeleteExitsFiles();

            PullTmpFileToRomote();

            ReadTmpFileToList();

            DeleteExitsFiles();

            Thread t = new Thread(ThreadStartCopyFiles);
            t.Start();
        }
        private void ThreadStartCopyFiles()
        {
            try
            {
                GetFilesToFileList();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("没有找到该路径");
            }

            PushFilesToDevice();

            DeleteFileOrFoder();

            PushTmpFiles();

            isStart = false;
            Dispatcher.Invoke(new Action(delegate { StartButton.Background = Brushes.White; }));
            
        }
        /// <summary>
        /// 将设备上的临时文件拿到本地
        /// </summary>
        private void PullTmpFileToRomote()
        {
            {
                string cmd = @"adb pull /sdcard/" + mDestPathName + @"/" + mTmpFileName + " " + mSourcePathName;
                RunCmd(cmd, out output);
            }
            {
                string cmd = @"adb pull /sdcard/" + mDestPathName + @"/" + mTmpFileFoderName + " " + mSourcePathName;
                RunCmd(cmd, out output);
            }
        }
        /// <summary>
        /// 删除在本地存在的临时文件
        /// </summary>
        private void DeleteExitsFiles()
        {
            if (File.Exists(mSourcePathName + "\\" + mTmpFileName))
            {
                File.Delete(mSourcePathName + "\\" + mTmpFileName);
            }
            if (File.Exists(mSourcePathName + "\\" + mTmpFileFoderName))
            {
                File.Delete(mSourcePathName + "\\" + mTmpFileFoderName);
            }
        }
        /// <summary>
        /// 将临时文件的内容读取到相应的存储列表里
        /// </summary>
        private void ReadTmpFileToList()
        {
            if (File.Exists(mSourcePathName + "\\" + mTmpFileName))
            {
                using (TextReader tr = File.OpenText(mSourcePathName + "\\" + mTmpFileName))
                {
                    string readFilePath;
                    string readMD5;
                    while (null != (readFilePath = tr.ReadLine()) && null != (readMD5 = tr.ReadLine()))
                    {
                        string tmp = readFilePath;
                        //var tmps = tmp.Split('.');
                        //var suffix = tmps[tmps.Length - 1];
                        //if (mFilterFileLines.Contains(suffix))
                        //    continue;
                        if (!tmp.Contains(".dll") && !tmp.Contains(".pdb"))
                            tmp = readFilePath.ToLower();
                        if (!mDestFileList.ContainsKey(tmp))
                            mDestFileList.Add(tmp, readMD5);
                    }
                }
            }

            if (File.Exists(mSourcePathName + "\\" + mTmpFileFoderName))
            {
                using (TextReader tr = File.OpenText(mSourcePathName + "\\" + mTmpFileFoderName))
                {
                    string readFilePath;
                    while (null != (readFilePath = tr.ReadLine()))
                    {
                        //var Strs = readFilePath.Split('\\');
                        //var folder = Strs[Strs.Length - 1];
                        //if (FilterFolderLines.Contains(folder))
                        //    continue;
                        string tmp = readFilePath.ToLower();
                        if (!mDestFileFoderList.Contains(tmp))
                            mDestFileFoderList.Add(tmp);
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否过滤（相似列表）
        /// </summary>
        /// <param name="fileName"></param>
        private bool IsFilterLikeFile(string fileName)
        {
            foreach (var filter in mFilterLikeFileLines)
            {
                if (fileName.EndsWith(filter))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 拿到源文件夹下的所有文件并将其保存在本程序列表里
        /// </summary>
        private void GetFilesToFileList()
        {
            Dispatcher.Invoke(new Action(delegate { this.Cursor = Cursors.Wait; }));
            string tmp = mSourceFileName + "\\";
            DirectoryInfo d = new DirectoryInfo(mSourcePath);
            DirectoryInfo[] items = d.GetDirectories(tmp, SearchOption.AllDirectories);
            List<string> mFilterFolderList = new List<string>();
            mSourceFileFoderList = new List<string>();
            foreach (var item in items)
            {
                string foder = item.FullName;
                var Strs = foder.Split('\\');
                var folder = Strs[Strs.Length - 1];
                if (FilterFolderLines.Contains(folder))
                {
                    mFilterFolderList.Add(item.FullName);
                    continue;
                }
                string foderLower = foder.ToLower();
                string saveFoderName = foderLower.Replace(mSourcePathNameLower, "");
                mSourceFileFoderList.Add(saveFoderName);
            }
            FileInfo[] files = d.GetFiles(tmp, SearchOption.AllDirectories);
            mSourceFileList = new Dictionary<string, string>();
            foreach (var file in files)
            {
                if (mFilterFolderList.Contains(file.DirectoryName))
                    continue;
                var fileName = file.FullName;
                var fileNameLower = file.FullName;
                string saveFileName = "";
                var tmps = fileName.Split('.');
                var suffix = tmps[tmps.Length - 1];
                if (mFilterFileLines.Contains(suffix))
                    continue;
                if (IsFilterLikeFile(fileName))
                    continue;
                if (!fileName.Contains(".dll") && !fileName.Contains(".pdb"))
                {
                    fileNameLower = fileName.ToLower();
                    saveFileName = fileNameLower.Replace(mSourcePathNameLower, "");
                }
                else
                {
                    saveFileName = fileNameLower.Replace(mSourcePathName, "");
                }
                try
                {
                    var fileMD5 = CSUtility.Program.GetMD5HashFromFile(fileName);
                    mSourceFileList.Add(saveFileName, fileMD5);
                }
                catch (Exception)
                {
                    
                }
            }
            Dispatcher.Invoke(new Action(delegate { this.Cursor = Cursors.Arrow; }));

            Dispatcher.Invoke(new Action(delegate { TotalSum.Text = mSourceFileList.Count.ToString(); }));
        }
        /// <summary>
        /// 比较源文件与设备中的文件的差别然后判断是否进行push操作
        /// </summary>
        private void PushFilesToDevice()
        {
            if (mSourceFileList.Count == 0)
            {
                StreamWriter swFilePath = File.AppendText(mSourcePathName + "\\" + mTmpFileName);           // 创建临时文件保存文件路径及其MD5码
                swFilePath.WriteLine("");                                                                   // 将文件路径写入临时文件
                swFilePath.Close();
            }
            else
            {
                Dispatcher.Invoke(new Action(delegate { ProgressBar_Copy.Visibility = Visibility.Visible; ProgressBar_Copy.Value = 0; }));
                int fileNums = 0;
                foreach (var fileAndMD5 in mSourceFileList)
                {
                    if (isStop)
                        break;
                    fileNums++;
                    Dispatcher.Invoke(new Action(delegate { ProgressBar_Copy.Value = (double)fileNums / (double)mSourceFileList.Count; }));
                    string fileName = fileAndMD5.Key;
                    string fileMd5Code = fileAndMD5.Value;
                    string mCodeMD5;

                    mDestSavePath = fileName.Replace("\\", "/");

                    if (!mDestFileList.TryGetValue(fileName, out mCodeMD5))
                    {
                        string cmd = @"adb push " + mSourcePathName + fileName + @" /sdcard/" + mDestPathName + mDestSavePath;
                        RunCmd(cmd, out output);
                        if (output != null)
                        {
                            if (!output.Contains("KB/s"))
                            {
                                mFaildFileNum++;
                                mErrorInformation = "拷贝失败 " + fileName + " 原因:" + output;
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    ListBox_Failed.Items.Add(mErrorInformation);
                                    ListBox_Failed.ScrollIntoView(mErrorInformation);
                                }));
                                continue;
                            }
                        }
                        mAddFileNum++;
                        mLoadFileNum++;
                        Dispatcher.Invoke(new Action(delegate
                        {
                            object obj = "添加 " + mDestSavePath;
                            ListBox_Add.Items.Add(obj);
                            ListBox_Add.ScrollIntoView(obj);
                        }));
                    }
                    else
                    {
                        if (mCodeMD5 != fileMd5Code)
                        {
                            string cmd = @"adb push " + mSourcePathName + fileName + @" /sdcard/" + mDestPathName + mDestSavePath;
                            RunCmd(cmd, out output);
                            if (output != null)
                            {
                                if (!output.Contains("KB/s"))
                                {
                                    mFaildFileNum++;
                                    mErrorInformation = "拷贝失败 " + fileName + " 原因:" + output;
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        ListBox_Failed.Items.Add(mErrorInformation);
                                        ListBox_Failed.ScrollIntoView(mErrorInformation);
                                    }));
                                    continue;
                                }
                                mModifiedFileNum++;
                                mLoadFileNum++;
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    object obj = "更新 " + mDestSavePath;
                                    ListBox_Add.Items.Add(obj);
                                    ListBox_Add.ScrollIntoView(obj);
                                }));
                            }
                        }
                        else
                        {
                            mIgnoreFileNum++;
                        }
                    }
                    StreamWriter swFilePath = File.AppendText(mSourcePathName + "\\" + mTmpFileName);           // 创建临时文件保存文件路径及其MD5码
                    swFilePath.WriteLine(fileName);                                                             // 将文件路径写入临时文件
                    swFilePath.WriteLine(fileMd5Code);                                                          // 将文件的MD5码写入临时文件
                    swFilePath.Close();
                    Dispatcher.Invoke(new Action(delegate { FaildNum.Text = mFaildFileNum.ToString(); }));      // 失败的文件数量
                    Dispatcher.Invoke(new Action(delegate { LoadOnSum.Text = mLoadFileNum.ToString(); }));      // 上传的文件数量
                    Dispatcher.Invoke(new Action(delegate { Modified.Text = mModifiedFileNum.ToString(); }));   // 修改的文件数量
                    Dispatcher.Invoke(new Action(delegate { AddFiles.Text = mAddFileNum.ToString(); }));        // 添加的文件数量
                    Dispatcher.Invoke(new Action(delegate { Ignore.Text = mIgnoreFileNum.ToString(); }));       // 忽略的文件数量
                }
            }
            Dispatcher.Invoke(new Action(delegate { ProgressBar_Copy.Visibility = Visibility.Collapsed; }));
            if (mSourceFileFoderList.Count == 0)
            {
                StreamWriter swFilePath = File.AppendText(mSourcePathName + "\\" + mTmpFileFoderName);      // 创建临时文件保存文件路径及其MD5码
                swFilePath.WriteLine("");                                                                   // 将文件路径写入临时文件
                swFilePath.Close();
            }
            else
            {
                foreach (var foder in mSourceFileFoderList)
                {
                    StreamWriter swFilePath = File.AppendText(mSourcePathName + "\\" + mTmpFileFoderName);       // 创建临时文件保存文件路径及其MD5码
                    swFilePath.WriteLine(foder);                                                                 // 将文件路径写入临时文件
                    swFilePath.Close();
                }
            }
        }
        /// <summary>
        /// 删除不存在的文件以及文件夹
        /// </summary>
        private void DeleteFileOrFoder()
        {
            Dispatcher.Invoke(new Action(delegate { ProgressBar_Copy.Visibility = Visibility.Visible; ProgressBar_Copy.Value = 0; }));
            int count = 0;
            int AllCounts = mDestFileList.Count + mDestFileFoderList.Count;
            foreach (var file in mDestFileList)
            {
                count++;
                Dispatcher.Invoke(new Action(delegate { ProgressBar_Copy.Value = (double)count / (double)AllCounts; }));
                string fileNamePath = file.Key;
                if (!mSourceFileList.ContainsKey(fileNamePath))
                {
                    string filePath = fileNamePath.Replace('\\', '/');
                    RunDeleteFileCmd(filePath);
                    mDeleteFileNum++;
                    Dispatcher.Invoke(new Action(delegate { DeleteNum.Text = mDeleteFileNum.ToString(); }));
                    Dispatcher.Invoke(new Action(delegate
                    {
                        object obj = "Delete " + filePath;
                        ListBox_Add.Items.Add(obj);
                        ListBox_Add.ScrollIntoView(obj);
                    }));
                }
            }
            foreach (var foder in mDestFileFoderList)
            {
                count++;
                Dispatcher.Invoke(new Action(delegate { ProgressBar_Copy.Value = (double)count / (double)AllCounts; }));
                if (!mSourceFileFoderList.Contains(foder))
                {
                    string foderPath = foder.Replace("\\", "/");
                    if (foderPath != "")
                        RunDeleteFileFoderCmd(foderPath);
                    Dispatcher.Invoke(new Action(delegate
                    {
                        object obj = "Delete " + foderPath;
                        ListBox_Add.Items.Add(obj);
                        ListBox_Add.ScrollIntoView(obj);
                    }));
                }
            }
            Dispatcher.Invoke(new Action(delegate { ProgressBar_Copy.Visibility = Visibility.Collapsed;}));
        }
        /// <summary>
        /// 将存在本地的临时文件传到device
        /// </summary>
        void PushTmpFiles()
        {
            string output = "";
            {
                string cmd = @"adb push " + mSourcePathName + "\\" + mTmpFileFoderName + @" /sdcard/" + mDestPathName + @"/";
                RunCmd(cmd, out output);
            }
            {
                string cmd = @"adb push " + mSourcePathName + @"\" + mTmpFileName + @" /sdcard/" + mDestPathName + @"/";
                RunCmd(cmd, out output);
            }
            // 删除创建和移出的临时文件
            DeleteExitsFiles();
            Dispatcher.Invoke(new Action(delegate
            {
                object obj = "所有文件拷贝结束";
                ListBox_Add.Items.Add(obj);
                ListBox_Add.ScrollIntoView(obj);
            }));
        }
        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="output"></param>
        private void RunCmd(string cmd, out string output)
        {
            cmd = cmd.Trim().TrimEnd('&') + "&exit";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = cmdPath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;
                output = p.StandardError.ReadLine();
                p.WaitForExit();
                p.Close();
            }
        }
        /// <summary>
        /// 删除device文件的CMD命令
        /// </summary>
        /// <param name="fileName"></param>
        void RunDeleteFileCmd(string fileName)
        {
            fileName = fileName.Trim().TrimEnd('&') + "&exit";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = cmdPath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                p.StandardInput.WriteLine("adb shell");
                p.StandardInput.AutoFlush = true;

                p.StandardInput.WriteLine("cd sdcard");
                p.StandardInput.AutoFlush = true;
                p.StandardInput.WriteLine("rm " + mDestPathName + fileName);
                p.StandardInput.AutoFlush = true;
                p.Close();
            }
        }
        /// <summary>
        /// 删除device文件夹的命令
        /// </summary>
        /// <param name="foderName"></param>
        void RunDeleteFileFoderCmd(string foderName)
        {
            foderName = foderName.Trim().TrimEnd('&') + "&exit";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = cmdPath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                p.StandardInput.WriteLine("adb shell");
                p.StandardInput.AutoFlush = true;

                p.StandardInput.WriteLine("cd sdcard");
                p.StandardInput.AutoFlush = true;

                p.StandardInput.WriteLine("rm -r " + mDestPathName + foderName);
                p.StandardInput.AutoFlush = true;
                p.Close();
            }
        }
    }
}
