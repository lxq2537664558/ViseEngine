using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Documents;
using System.IO;

namespace ResourcesBrowser
{
    public enum enBrowserType
    {
        File,
        Folder,

    }

    /// <summary>
    /// Interaction logic for BrowserControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "ResourcesBrowser")]
    [EditorCommon.PluginAssist.PluginMenuItem("窗口/资源浏览器")]
    [Guid("E3199AA6-FAB5-4564-AB0D-E313323B909C")]
    [PartCreationPolicy(CreationPolicy.Any)]
    public partial class BrowserControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin, EditorCommon.PluginAssist.IObjectEditorOperation
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region EditorPlugin

        public string PluginName
        {
            get { return "资源浏览器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "资源浏览器",
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

        public void SetObjectToEdit(object[] obj)
        {
            if (obj == null)
                return;

            if (obj.Length == 0)
                return;

            try
            {
                // 资源反查
                var absFilename = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath((string)(obj[0]));
                absFilename += Program.ResourceInfoExt;
                if(System.IO.File.Exists(absFilename))
                {
                    mResourceSearchInfo = CreateResourceInfo(absFilename);
                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(absFilename);
                    ShowSourcesInDir(path);
                }
                else
                {
                    EditorCommon.MessageBox.Show($"资源已不存在！！！\r\n{absFilename}", "提示");
                }
            }
            catch(System.Exception e)
            {
                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, e.ToString());
            }
        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {

        }

        public event EditorCommon.PluginAssist.Delegate_OnOpenEditor OnOpenEditor;
        public void OpenEditor(object[] objs)
        {
            OnOpenEditor?.Invoke(objs);
        }

        #endregion

        public delegate object Delegate_GetDragType(BrowserControl browserCtrl);
        public Delegate_GetDragType OnGetDragType;

        // 判断文件夹的合法性
        public bool IsFolderValid(string absFolder)
        {
            var resInfoFile = absFolder + Program.ResourceInfoExt;
            if (System.IO.File.Exists(resInfoFile))
                return false;

            return true;
        }

        //文件资源检测 //////////////////////////////////////////////////////////////////
        static FileSystemWatcher mResourceFilesWatcher;
        public static void InitializeFileSystemWatcher(BrowserControl ctrl)
        {
            if(mResourceFilesWatcher == null)
            {
                mResourceFilesWatcher = new FileSystemWatcher(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                mResourceFilesWatcher.EnableRaisingEvents = true;
                mResourceFilesWatcher.IncludeSubdirectories = true;
                mResourceFilesWatcher.Renamed += (object sender, RenamedEventArgs e)=>
                {
                    OnFileSystemWatcherEventRise(sender, e, ctrl);
                };
                mResourceFilesWatcher.Created += (object sender, FileSystemEventArgs e) =>
                {
                    OnFileSystemWatcherEventRise(sender, e, ctrl);
                };
                mResourceFilesWatcher.Deleted += (object sender, FileSystemEventArgs e) =>
                {
                    OnFileSystemWatcherEventRise(sender, e, ctrl);
                };
                mResourceFilesWatcher.Changed += (object sender, FileSystemEventArgs e)=>
                {
                    OnFileSystemWatcherEventRise(sender, e, ctrl);
                };
            }
        }

        static void OnFileSystemWatcherEventRise(object sender, FileSystemEventArgs e, BrowserControl ctrl)
        {
            ctrl.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Changed:
                    case WatcherChangeTypes.Renamed:
                    case WatcherChangeTypes.Created:
                        {
                            var absFileName = e.FullPath.Replace("\\", "/");
                            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(e.FullPath).Replace("\\", "/");
                            if (path.Length > 0 && path[path.Length - 1] == '/')
                                path = path.Remove(path.Length - 1);
                            if (ctrl.CurrentAbsFolder.Equals(path, StringComparison.OrdinalIgnoreCase))
                            {
                                var curRes = ctrl.CurrentResources.ToArray();
                                foreach (var res in curRes)
                                {
                                    if (res.AbsResourceFileName.Equals(absFileName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        var frlRes = res as ResourcesBrowser.IResourceInfoForceReload;
                                        if(frlRes != null)
                                        {
                                            frlRes?.ForceReload();
                                            ctrl.ReCreateSnapshot(res);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                var file = e.FullPath + Program.ResourceInfoExt;
                                if (System.IO.File.Exists(file))
                                {
                                    var resInfo = ctrl.CreateResourceInfo(e.FullPath + Program.ResourceInfoExt);
                                    if (resInfo != null)
                                    {
                                        var frlRes = resInfo as ResourcesBrowser.IResourceInfoForceReload;
                                        if(frlRes != null)
                                        {
                                            resInfo.Save();
                                            frlRes?.ForceReload();
                                            ctrl.ReCreateSnapshot(resInfo);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }));
        }

        ////////////////////////////////////////////////////////////////////

        // 获取文件的扩展名筛选关键字
        public delegate string Delegate_GetResourceFileFilter();
        public Delegate_GetResourceFileFilter OnGetResourceFileFilter;

        bool mInitToShowResourcesInFolder = false;
        // 创建资源信息处理
        [ImportMany(AllowRecomposition = true)]
        IEnumerable<Lazy<ResourceInfo, IResourceInfoMetaData>> mResourceInfoProcessers = null;
        public ResourceInfo CreateResourceInfo(string resourceInfoAbsFile)
        {
            if (mResourceInfoProcessers == null)
                return null;
            if (mInitToShowResourcesInFolder)
                return null;

            var info = new CommonResourceInfo();
            info.Load(resourceInfoAbsFile);

            foreach (var processer in mResourceInfoProcessers)
            {
                try
                {
                    if (processer.Metadata.ResourceInfoType == info.ResourceType)
                    {
                        ResourceInfo retInfo = null;
                        this.Dispatcher.Invoke(() =>
                        {
                            retInfo = System.Activator.CreateInstance(processer.Value.GetType()) as ResourceInfo;
                        });
                        retInfo.ParentBrowser = this;
                        retInfo.Load(resourceInfoAbsFile);
                        return retInfo;
                    }
                }
                catch(System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
            }

            return null;
        }

        public ResourceInfo CreateResourceInfoFromResource(string resourceAbsFile)
        {
            if (mResourceInfoProcessers == null)
                return null;

            var fileInfo = new System.IO.FileInfo(resourceAbsFile);
            var ext = fileInfo.Extension.ToLower();
            foreach (var processer in mResourceInfoProcessers)
            {
                if (processer.Metadata.ResourceExts == null)
                    continue;

                if(processer.Metadata.ResourceExts.Length > 0)
                {
                    foreach (var resExt in processer.Metadata.ResourceExts)
                    {
                        if (resExt.ToLower().Equals(ext))
                        {
                            var resInfo = processer.Value.CreateResourceInfoFromResource(resourceAbsFile);
                            if (resInfo == null)
                                return null;
                            resInfo.ParentBrowser = this;
                            return resInfo;
                        }
                    }
                }
                else
                {
                    var resInfo = processer.Value.CreateResourceInfoFromResource(resourceAbsFile);
                    if (resInfo == null)
                        return null;
                    resInfo.ParentBrowser = this;
                    return resInfo;
                }
            }

            return null;
        }

        // 资源反查对象
        ResourceInfo mResourceSearchInfo;

        public delegate void Delegate_OnResourceInFolderAllShowed();
        public Delegate_OnResourceInFolderAllShowed OnResourceInFolderAllShowed;
        public void ResourceInFolderAllShowed()
        {
            OnResourceInFolderAllShowed?.Invoke();

            if(mResourceSearchInfo != null)
            {
                int i = 0;
                foreach(var resInfo in CurrentResources)
                {
                    if(resInfo.Id == mResourceSearchInfo.Id)
                    {
                        ListBox_Resources.SelectedItem = resInfo;
                        mResourceWrapPanel.ScrollToIndex(ListBox_Resources.SelectedIndex);
                        break;
                        //var searchObj = ListBox_Resources.ItemContainerGenerator.ContainerFromItem(resInfo) as ListBoxItem;
                        //if(searchObj != null)
                        //{
                        //    searchObj.BringIntoView();
                        //    searchObj.IsSelected = true;
                        //}
                    }

                    i++;
                }
                mResourceSearchInfo = null;
            }
        }

        class ResourceInfoQueueThread
        {
            //private Queue<ResourceInfo> mResourceInfoQueue = new Queue<ResourceInfo>();
            private Queue<string> mResourceInfoQueue = new Queue<string>();
            private BrowserControl mParentBrowserControl;

            System.Threading.AutoResetEvent mEvent = new System.Threading.AutoResetEvent(true);
            public System.Threading.AutoResetEvent Event
            {
                get { return mEvent; }
            }
            private System.Threading.Thread mLoadSourceControlThread;
            bool mThreadRuning = false;

            public ResourceInfoQueueThread(BrowserControl ctrl)
            {
                mParentBrowserControl = ctrl;
            }
            ~ResourceInfoQueueThread()
            {
                mThreadRuning = false;
            }

            void StartThread()
            {
                mThreadRuning = true;
                mLoadSourceControlThread = new System.Threading.Thread(new System.Threading.ThreadStart(Tick));
                mLoadSourceControlThread.Name = "BrowserInfo加载Resources";
                mLoadSourceControlThread.IsBackground = true;
                mLoadSourceControlThread.Start();
            }

            public void EndThread()
            {
                mThreadRuning = false;
                //mLoadSourceControlThread?.Abort();
            }

            public System.Threading.ThreadState GetThreadState()
            {
                if (mLoadSourceControlThread == null)
                    return System.Threading.ThreadState.Unstarted;
                return mLoadSourceControlThread.ThreadState;
            }

            public void Enqueue(string file)
            {
                lock (mResourceInfoQueue)
                    mResourceInfoQueue.Enqueue(file);

                if (!mThreadRuning)
                {
                    StartThread();
                }
            }

            public void Clear()
            {
                lock (mResourceInfoQueue)
                    mResourceInfoQueue.Clear();
            }

            public void Tick()
            {
                while (mThreadRuning)
                {
                    string file = null;
                    lock (mResourceInfoQueue)
                    {
                        if (mResourceInfoQueue.Count > 0)
                            file = mResourceInfoQueue.Dequeue();
                        else
                            mThreadRuning = false;
                    }

                    if (!string.IsNullOrEmpty(file))
                    {
                        mParentBrowserControl.Dispatcher.Invoke(new Action<string>((resFile) =>
                            {
                                if (mThreadRuning)
                                {
                                    var info = mParentBrowserControl.CreateResourceInfo(resFile);
                                    if (info != null)
                                    {
                                        if (!info.AbsInfoFileName.Contains(mParentBrowserControl.CurrentAbsFolder))
                                            return;

                                        mParentBrowserControl.CurrentResources.Add(info);                                        
                                        mParentBrowserControl.UpdateCountString();
                                        //var resControl = mParentBrowserControl.AddResourceControl(info);
                                        //mParentBrowserControl.UpdateCountString();

                                        //info.HostResourceControl = resControl;
                                    }
                                }

                            }), new object[] { file });

                    }

                    //lock (mResourceInfoQueue)
                    {
                        if (mResourceInfoQueue.Count <= 0)
                        {
                            mParentBrowserControl.Dispatcher.Invoke(new Action(() =>
                                {
                                    mParentBrowserControl.ResourceInFolderAllShowed();
                                }));
                        }
                    }

                    System.Threading.Thread.Sleep(1);
                }

                mEvent.Set();
            }
        }
        ResourceInfoQueueThread mResQueueThread;

        #region 筛选
        string mFilterString = "";
        public string FilterString
        {
            get { return mFilterString; }
            set
            {
                mFilterString = value;

                UpdateFilter();
                if (string.IsNullOrEmpty(mFilterString))
                {
                    ListBox_Resources.Items.Filter = null;
                }

                if (mResourceWrapPanel != null)
                {
                    mResourceWrapPanel.InvalidateMeasure();
                    mResourceWrapPanel.SetVerticalOffset(0);
                }

                OnPropertyChanged("FilterString");
            }
        }

        bool mIsCheckedAll = true;
        public bool IsCheckedAll
        {
            get { return mIsCheckedAll; }
            set
            {
                mIsCheckedAll = value;
                
                foreach(var i in mFilterItems)
                {
                    i.IsChecked = value;
                }                
            }
        }
        CSUtility.Support.ThreadSafeObservableCollection<FilterResourceItem> mFilterItems = new CSUtility.Support.ThreadSafeObservableCollection<FilterResourceItem>();
        public void InitializeFilter()
        {
            mFilterItems.Clear();
            if (mResourceInfoProcessers == null)
                return;
            
            foreach (var processer in mResourceInfoProcessers)
            {
                FilterResourceItem item = new FilterResourceItem(processer, this);
                mFilterItems.Add(item);
            }
            ListBox_FilterItems.ItemsSource = mFilterItems;
        }
        public void UpdateFilter()
        {
            ListBox_Resources.Items.Filter = new Predicate<object>((object obj) =>
            {
                var retValue = true;

                var info = obj as ResourceInfo;
                if (!string.IsNullOrEmpty(FilterString))
                {
                    if (info != null)
                    {
                        retValue = info.DoFilter(mFilterString);
                    }
                }

                foreach (var resItem in mFilterItems)
                {
                    if (resItem.ResourceType.Equals(info.ResourceType))
                    {
                        retValue = retValue && resItem.IsChecked;
                        break;
                    }
                }
                
                mResourceWrapPanel?.SetVerticalOffset(0);

                return retValue;
            });
        }

        #endregion

        #region 排序

        SortDescription mCurrentSortDescription = new SortDescription("Name", ListSortDirection.Ascending);
        private void Sort()
        {
            ListBox_Resources.Items.SortDescriptions.Clear();
            ListBox_Resources.Items.SortDescriptions.Add(mCurrentSortDescription);
        }

        private void StackPanel_ListType_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListSortDirection dir = ListSortDirection.Ascending;
            if (mCurrentSortDescription != null && mCurrentSortDescription.PropertyName.Equals("ResourceTypeName"))
            {
                switch (mCurrentSortDescription.Direction)
                {
                    case ListSortDirection.Ascending:
                        dir = ListSortDirection.Descending;
                        TypePathScale.ScaleY = -1;
                        break;
                    case ListSortDirection.Descending:
                        dir = ListSortDirection.Ascending;
                        TypePathScale.ScaleY = 1;
                        break;
                }
            }
            mCurrentSortDescription = new SortDescription("ResourceTypeName", dir);
            Path_ListName.Visibility = Visibility.Hidden;
            Path_ListType.Visibility = Visibility.Visible;
            Sort();
        }
        private void StackPanel_ListName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListSortDirection dir = ListSortDirection.Ascending;
            if (mCurrentSortDescription != null && mCurrentSortDescription.PropertyName.Equals("Name"))
            {
                switch (mCurrentSortDescription.Direction)
                {
                    case ListSortDirection.Ascending:
                        dir = ListSortDirection.Descending;
                        NamePathScale.ScaleY = -1;
                        break;
                    case ListSortDirection.Descending:
                        dir = ListSortDirection.Ascending;
                        NamePathScale.ScaleY = 1;
                        break;
                }
            }
            mCurrentSortDescription = new SortDescription("Name", dir);
            Path_ListName.Visibility = Visibility.Visible;
            Path_ListType.Visibility = Visibility.Hidden;
            Sort();
        }

        #endregion

        // 子对象数量统计
        string mSourceCountString = "";
        public string SourceCountString
        {
            get { return mSourceCountString; }
            set
            {
                mSourceCountString = value;
                OnPropertyChanged("SourceCountString");
            }
        }
        public void UpdateCountString()
        {
            SourceCountString = $"共{CurrentResources.Count}个资源(选中{ListBox_Resources.SelectedItems.Count}项)";
        }

        string mCurrentFolder = "";
        public string CurrentAbsFolder
        {
            get { return mCurrentFolder; }
            set
            {
                mCurrentFolder = value.Replace("\\", "/");
                if(mCurrentFolder.Length > 0 && mCurrentFolder[mCurrentFolder.Length - 1] == '/')
                {
                    mCurrentFolder = mCurrentFolder.Remove(mCurrentFolder.Length - 1);
                }

                CurrentFolderDescription = mCurrentFolder;
                if (SearchSubFolder)
                    CurrentFolderDescription += "及其子文件夹";

                UpdateFolderShortcut();

                OnPropertyChanged("CurrentFolder");
            }
        }
        string mCurrentFolderDescription;
        public string CurrentFolderDescription
        {
            get { return mCurrentFolderDescription; }
            set
            {
                mCurrentFolderDescription = value;
                OnPropertyChanged("CurrentFolderDescription");
            }
        }

        bool mSearchSubFolder = false;
        public bool SearchSubFolder
        {
            get { return mSearchSubFolder; }
            set
            {
                mSearchSubFolder = value;
                OnPropertyChanged("SearchSubFolder");
            }
        }

        public enBrowserType BrowserType = enBrowserType.File;

        // 当前显示的资源
        CSUtility.Support.ThreadSafeObservableCollection<ResourceInfo> mCurrentResources = new CSUtility.Support.ThreadSafeObservableCollection<ResourceInfo>();
        public CSUtility.Support.ThreadSafeObservableCollection<ResourceInfo> CurrentResources
        {
            get { return mCurrentResources; }
            set
            {
                mCurrentResources = value;
                OnPropertyChanged("CurrentFolderResources");
            }
        }

        public BrowserControl()
        {
            InitializeComponent();

            InitializeGameWindowDragDrop();
            InitializeFileSystemWatcher(this);

            mListBoxDropAdorner = new EditorCommon.DragDrop.DropAdorner(ListBox_Resources);

            mResQueueThread = new ResourceInfoQueueThread(this);
            ShowFolderTree(CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(CSUtility.Support.IFileConfig.DefaultResourceDirectory));

            RegisterDataTemplate();

            EditorCommon.PluginAssist.PluginOperation.OnRefreshBrowserSnapshot += RefreshBrowserSnapshot;
        }

        void RegisterDataTemplate()
        {
            var template = this.TryFindResource("TextureSetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("TextureSet", template);

            template = this.TryFindResource("MeshSetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("MeshSet", template);

            template = this.TryFindResource("UVAnimSetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("UVAnimSet", template);

            template = this.TryFindResource("ActionSetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("ActionSet", template);            

            template = this.TryFindResource("SoundSetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("SoundSet", template);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeFilter();
            InitializeCreateMenu();
        }

        VirtualizingWrapPanel mResourceWrapPanel;
        private void VirtualizingWrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            mResourceWrapPanel = sender as VirtualizingWrapPanel;
            if (mResourceWrapPanel != null)
            {
                mResourceWrapPanel.ChildWidth = mChildWidth * Slider_ResourceItemSize.Value;
                mResourceWrapPanel.ChildHeight = mChildHeight * Slider_ResourceItemSize.Value;
            }
        }

        #region 文件夹快捷访问
        // 更新文件夹快捷方式访问，用于快速访问文件夹
        private void UpdateFolderShortcut()
        {
            StackPanel_Folders.Children.Clear();
            var relDir = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(CurrentAbsFolder, CSUtility.Support.IFileManager.Instance.Root);
            relDir = relDir.Replace("\\", "/");
            var dirNames = relDir.Split('/');
            var dirs = "";
            foreach(var dirName in dirNames)
            {
                if (string.IsNullOrEmpty(dirName))
                    continue;

                dirs += dirName + "/";

                var btn = new Button()
                {
                    Content = dirName,
                    Margin = new Thickness(2),
                    Tag = dirs,
                    Style = TryFindResource("ButtonStyle_FolderSnapshot") as Style
                };
                btn.Click += (sender, e)=>
                {
                    // 点击显示相应目录
                    ShowSourcesInDir(CSUtility.Support.IFileManager.Instance.Root + (string)(btn.Tag));
                };
                StackPanel_Folders.Children.Add(btn);

                var tBtn = new Button()
                {
                    Margin = new Thickness(2),
                    Width = 20,
                    Tag = dirs,
                    Style = TryFindResource("ButtonStyle_FolderSnapshotSub") as Style
                };
                StackPanel_Folders.Children.Add(tBtn);

                var folders = GetSubFolders(CSUtility.Support.IFileManager.Instance.Root + dirs, System.IO.SearchOption.TopDirectoryOnly);
                if(folders.Count > 0)
                {
                    var contextMenu = new ContextMenu()
                    {
                        StaysOpen = false,
                        Style = TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "ContextMenu_Default")) as Style,
                        PlacementTarget = tBtn,
                        Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
                    };

                    foreach(var folder in folders)
                    {
                        bool isCurrentFolder = folder.Replace("\\", "/").Equals(CurrentAbsFolder.Replace("\\", "/"));
                        var menuItem = new MenuItem()
                        {
                            IsChecked = isCurrentFolder,
                            Header = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(folder),
                            Style = TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "MenuItem_Default")) as Style,
                        };
                        menuItem.Click += (sender, e) =>
                        {
                            ShowSourcesInDir(folder);
                        };
                        contextMenu.Items.Add(menuItem);
                    }

                    tBtn.Click += (sender, e) =>
                    {
                        //点击打开次级目录结构
                        contextMenu.IsOpen = true;
                    };
                }
            }
        }
        
        #endregion

        /// <summary>
        /// 查找有效的子目录(部分资源会以目录形式存在，此函数只查找有效的目录，不包含以目录形式存在的资源目录)
        /// </summary>
        /// <param name="absFolder">要查找的绝对路径</param>
        /// <param name="searchOption">查找选项</param>
        /// <returns></returns>
        private List<string> GetSubFolders(string absFolder, System.IO.SearchOption searchOption) 
        {
            List<string> retList = new List<string>();
            var folders = System.IO.Directory.EnumerateDirectories(absFolder, "*.*", searchOption);
            foreach(var folder in folders)
            {
                var file = folder + Program.ResourceInfoExt;
                // 有资源Info存在，所以此目录并非真正的文件结构目录
                if (System.IO.File.Exists(file))
                    continue;

                retList.Add(folder);
            }

            return retList;
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as FolderItem;

            ShowSourcesInDir(item.AbsolutePath);
        }

        private void TreeViewItem_UnSelected(object sender, RoutedEventArgs e)
        {

        }

        public void AddItemToToolbar(FrameworkElement element)
        {
            ToolBar_Main.Items.Add(element);
        }

        Guid mTempId = Guid.NewGuid();
        string mRootFolder = "";

        // targetFolder使用绝对坐标
        public void ShowFolderTree(string targetFolder)
        {
            mRootFolder = targetFolder.Replace("\\", "/");

            TreeView_Folders.Items.Clear();

            if (System.IO.Directory.Exists(targetFolder))
            {
                if (!IsFolderValid(targetFolder))
                    return;

                var item = new FolderItem(targetFolder, this);
                item.OnItemDragEnter += FolderItem_OnDragEnter;
                item.OnItemDragLeave += FolderItem_OnDragLeave;
                item.OnItemDragOver += FolderItem_OnDragOver;
                item.OnItemDrop += FolderItem_OnDrop;

                item.MouseDown += FolderItem_MouseDown;
                item.MouseMove += FolderItem_MouseMove;

                TreeView_Folders.Items.Add(item);

                item.IsExpanded = true;

                CurrentResources.Clear();
            }
        }

        private bool SelectedFolder(FolderItem item, List<string> folderNames)
        {
            if (folderNames == null)
                return true;

            if (folderNames.Count == 0)
                return true;

            if (item.PathName == folderNames[0])
            {
                if (item.Items.Count > 0)
                {
                    if (folderNames.Count == 1)
                    {
                        if (!item.IsSelected)
                        {
                            item.IsSelected = true;
                            item.BringIntoView();
                        }

                        return true;
                    }

                    folderNames.RemoveAt(0);
                    foreach (FolderItem subItem in item.Items)
                    {
                        if (SelectedFolder(subItem, folderNames))
                            return true;
                    }
                }
                else
                {
                    if (!item.IsSelected)
                    {
                        item.IsSelected = true;
                        item.BringIntoView();
                    }

                    //var itemParent = item.Parent as FolderItem;
                    //while (itemParent != null)
                    //{
                    //    if (!itemParent.IsExpanded)
                    //        itemParent.IsExpanded = true;
                    //}

                    return true;
                }
            }

            return false;
        }

        public void ShowSourcesInDir(string sourceAbsFolder)
        {
            if (mInitToShowResourcesInFolder)
                return;

            mInitToShowResourcesInFolder = true;
            mResQueueThread.EndThread();
            //mResQueueThread.Event.WaitOne();

            SnapshotProcess.ImageQueue.Instance.Clear();
            mResQueueThread.Clear();
            //while(mResQueueThread.GetThreadState() != System.Threading.ThreadState.Unstarted &&
            //      mResQueueThread.GetThreadState() != System.Threading.ThreadState.Stopped &&
            //      mResQueueThread.GetThreadState() != System.Threading.ThreadState.Aborted)//mResQueueThread.GetThreadState() == System.Threading.ThreadState.Running)
            //{
            //    System.Threading.Thread.Sleep(1);
            //}
            //System.Threading.Thread.Sleep(500);

            // 设置选中文件夹树形列表
            {
                var folderString = sourceAbsFolder.Replace("\\", "/");
                folderString = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(folderString, mRootFolder);
                folderString = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(mRootFolder) + "/" + folderString;
                if (folderString[folderString.Length - 1] == '/')
                    folderString = folderString.Remove(folderString.Length - 1);
                var folderNames = new List<string>(folderString.Split('/'));

                foreach (FolderItem item in TreeView_Folders.Items)
                {
                    if (SelectedFolder(item, folderNames))
                        break;
                }
            }

            if (!System.IO.Directory.Exists(sourceAbsFolder))
            {
                EditorCommon.MessageBox.Show("文件夹" + sourceAbsFolder + "不存在, 请刷新父级目录再试！");
                return;
            }
            
            CurrentResources.Clear();
            CurrentAbsFolder = sourceAbsFolder;
            mInitToShowResourcesInFolder = false;

            foreach (var file in System.IO.Directory.GetFiles(CurrentAbsFolder, "*" + Program.ResourceInfoExt, SearchSubFolder ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly))
            {
                // 多线程加载ResourceInfo
                mResQueueThread.Enqueue(file);
                // 单线程加载ResourceInfo
                //var info = CreateResourceInfo(file);
                //CurrentResources.Add(info);
            }

            UpdateCountString();
            Sort();

            mResourceWrapPanel?.SetVerticalOffset(0);

            return;
            
            /*System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly;
            if(SearchSubFolder)
                searchOption = System.IO.SearchOption.AllDirectories;

            IEnumerable<string> enumRes = null;
            switch (BrowserType)
            {
                case enBrowserType.File:
                    {
                        string fileFilter = "*.*";
                        if (OnGetResourceFileFilter != null)
                        {
                            fileFilter = OnGetResourceFileFilter();
                        }
                        if (string.IsNullOrEmpty(fileFilter))
                            fileFilter = "*.*";

                        enumRes = System.IO.Directory.EnumerateFiles(sourceAbsFolder, fileFilter, searchOption);

                    }
                    break;

                case enBrowserType.Folder:
                    {
                        string fileFilter = "*";
                        if (OnGetResourceFileFilter != null)
                        {
                            fileFilter = OnGetResourceFileFilter();
                        }
                        if (string.IsNullOrEmpty(fileFilter))
                            fileFilter = "*";

                        enumRes = System.IO.Directory.EnumerateDirectories(sourceAbsFolder, fileFilter, searchOption);

                    }
                    break;
            }

            if (enumRes != null)
            {
                foreach (var file in enumRes)
                {
                    //var resInfo = CreateResourceInfo(file);
                    //if (resInfo == null)
                    //    continue;

                    mResQueueThread.Enqueue(file);
                }
            }*/
        }

        public bool RemoveResourceControl(ResourceInfo info)
        {
            return CurrentResources.Remove(info);
        }

        public void RemoveSelectedResources()
        {
            //foreach (var ctrl in mSelectedResourceControls)
            //{
            //    ctrl.Info.OnDeleted();

            //    WrapPanel_Items.Children.Remove(ctrl);
            //}
        }

        public bool ReCreateSnapshot(ResourceInfo info)
        {
            //ResourceControl ctrl = null;
            //if (mResourcesDic.TryGetValue(info, out ctrl))
            //{
            //    SnapshotProcess.ImageQueue.Instance.Queue(ctrl, info, true);
            //    return true;
            //}
            SnapshotProcess.ImageQueue.Instance.Queue(info, true);

            return true;
        }

        public void RefreshBrowserSnapshot(string infoFile, bool reCreate)
        {
            var resInfo = CreateResourceInfo(infoFile);
            SnapshotProcess.ImageQueue.Instance.Queue(resInfo, reCreate);
        }

        public void SetExtendControl(FrameworkElement control)
        {
            Grid_Extend.Children.Clear();
            Grid_Extend.Children.Add(control);
            BindingOperations.SetBinding(Grid_Extend, Grid.VisibilityProperty, new Binding("Visibility") { Source = control });

            var parentGrid = Grid_Extend.Parent as Grid;
            if (parentGrid != null)
            {
                var colIdx = Grid.GetColumn(Grid_Extend);
                if (parentGrid.ColumnDefinitions.Count > colIdx)
                {
                    var colDef = parentGrid.ColumnDefinitions[colIdx];
                    if (colDef != null)
                    {
                        colDef.Width = new GridLength(0.3, GridUnitType.Star);
                    }
                }
            }
        }

        public void SelectResource(ResourceInfo info, bool isMulti = false)
        {
            //ResourceControl selCtrl = null;
            //if (mResourcesDic.TryGetValue(info, out selCtrl))
            //{
            //    if (!isMulti)
            //    {
            //        foreach (var ctrl in mSelectedResourceControls)
            //        {
            //            ctrl.Selected = false;
            //        }
            //        mSelectedResourceControls.Clear();
            //    }

            //    if (selCtrl != null)
            //    {
            //        selCtrl.Selected = true;
            //        mSelectedResourceControls.Add(selCtrl);
            //    }
            //}
        }

        public void RemoveResource(ResourceInfo info)
        {
            //ResourceControl ctrl = null;
            //if (mResourcesDic.TryGetValue(info, out ctrl))
            //{
            //    mSelectedResourceControls.Remove(ctrl);
            //    this.WrapPanel_Items.Children.Remove(ctrl);
            //}
        }

        public ResourceInfo[] GetSelectedInfos()
        {
            //ResourceInfo[] retInfos = new ResourceInfo[mSelectedResourceControls.Count];

            //for (int i = 0; i < mSelectedResourceControls.Count; i++)
            //{
            //    retInfos[i] = mSelectedResourceControls[i].Info;
            //}

            //return retInfos;
            return null;
        }

        Point mPoint = new Point(0, 0);
        void ResourceControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //var pt = e.GetPosition(this);
                //if (System.Math.Abs(pt.X - mPoint.X) > 10 ||
                //   System.Math.Abs(pt.Y - mPoint.Y) > 10)
                //{
                //    var selCtrl = sender as ResourceControl;

                //    if (mSelectedResourceControls.Count == 0 ||
                //        !mSelectedResourceControls.Contains(selCtrl))
                //    {
                //        // 单选
                //        foreach (var ctrl in mSelectedResourceControls)
                //        {
                //            ctrl.Selected = false;
                //        }
                //        mSelectedResourceControls.Clear();

                //        if (selCtrl != null)
                //        {
                //            selCtrl.Selected = true;
                //            mSelectedResourceControls.Add(selCtrl);
                //        }

                //        mLastSelectedCtrl = selCtrl;
                //    }

                //    var resInfo = new List<EditorCommon.DragDrop.IDragAbleObject>();//[mSelectedResourceControls.Count];
                //    foreach (var ctrl in mSelectedResourceControls)
                //    {
                //        var dragObj = ctrl.Info as EditorCommon.DragDrop.IDragAbleObject;
                //        if (dragObj == null)
                //            continue;

                //        resInfo.Add(dragObj);
                //    }

                //    if (resInfo.Count > 0)
                //    {
                //        if (OnGetDragType != null)
                //        {
                //            var dragType = OnGetDragType(this);
                //            EditorCommon.DragDrop.DragDropManager.Instance.StartDrag(dragType, resInfo.ToArray());
                //        }
                //    }
                //}
            }
        }

        void ResourceControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mPoint = e.GetPosition(this);
        }

        #region DragDrop

        // 检测资源文件是否可拖拽导入
        private bool CheckFileDropAvailable(string[] files)
        {
            foreach (var file in files)
            {
                var fileExt = "." + CSUtility.Support.IFileManager.Instance.GetFileExtension(file);

                foreach (var processer in mResourceInfoProcessers)
                {
                    if (processer.Metadata.ResourceExts != null && processer.Metadata.ResourceExts.Length > 0)
                    {
                        foreach (var ext in processer.Metadata.ResourceExts)
                        {
                            if (fileExt.Equals(ext))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        EditorCommon.DragDrop.DropAdorner mListBoxDropAdorner;
        private void ListBox_Resources_DragEnter(object sender, DragEventArgs e)
        {
            bool allowDrop = false;

            var datas = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (datas != null && datas.Length > 0)
            {
                allowDrop = CheckFileDropAvailable(datas);
            }

            if (allowDrop)
            {
                e.Effects = DragDropEffects.Copy;
                mListBoxDropAdorner.IsAllowDrop = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                mListBoxDropAdorner.IsAllowDrop = false;
            }

            var pos = e.GetPosition(ListBox_Resources);
            if(pos.X > 0 && pos.X < ListBox_Resources.ActualWidth &&
               pos.Y > 0 && pos.Y < ListBox_Resources.ActualHeight)
            {
                var layer = AdornerLayer.GetAdornerLayer(ListBox_Resources);
                layer.Add(mListBoxDropAdorner);
            }
            
            e.Handled = true;
        }
        private void ListBox_Resources_DragLeave(object sender, DragEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(ListBox_Resources);
            layer.Remove(mListBoxDropAdorner);
        }

        private void ListBox_Resources_DragOver(object sender, DragEventArgs e)
        {
            //ImportResources()
        }
        private void ListBox_Resources_Drop(object sender, DragEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(ListBox_Resources);
            layer.Remove(mListBoxDropAdorner);

            var datas = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (datas == null)
                return;
            if (datas.Length == 0)
                return;
            
            if (datas.Length > 0)
            {
                if (!CheckFileDropAvailable(datas))
                    return;
            }

            ImportResources(datas);
        }


        #region 文件夹拖动
        enum enDropResult
        {
            Denial_TargetPathIsEmpty,
            Denial_UnknowFormat,
            Denial_NoDragAbleObject,
            Denial_SamePath,
            Denial_SubFolder,
            Allow,
        }
        enDropResult AllowFolderItemDrop(FolderItem item, System.Windows.DragEventArgs e)
        {
            if (string.IsNullOrEmpty(item.AbsolutePath))
                return enDropResult.Denial_TargetPathIsEmpty;

            var formats = e.Data.GetFormats();
            if (formats == null || formats.Length == 0)
                return enDropResult.Denial_UnknowFormat;

            var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
            if (datas == null)
                return enDropResult.Denial_NoDragAbleObject;

            foreach (var data in datas)
            {
                var dragedItem = data as FolderItem;
                if (dragedItem.AbsolutePath.Equals(item.AbsolutePath))
                    return enDropResult.Denial_SamePath;

                if (item.AbsolutePath.Contains(dragedItem.AbsolutePath))
                    return enDropResult.Denial_SubFolder;
            }

            return enDropResult.Allow;
        }

        enDropResult AllowResourceItemDrop(FolderItem item, System.Windows.DragEventArgs e)
        {
            if (string.IsNullOrEmpty(item.AbsolutePath))
                return enDropResult.Denial_TargetPathIsEmpty;

            var formats = e.Data.GetFormats();
            if (formats == null || formats.Length == 0)
            {
                return enDropResult.Denial_UnknowFormat;
            }

            var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
            if (datas == null)
            {
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "无可移动对象";
                return enDropResult.Denial_NoDragAbleObject;
            }

            var tagFolderPath = item.AbsolutePath.Replace("\\", "/");
            if (tagFolderPath[tagFolderPath.Length - 1] == '/')
                tagFolderPath = tagFolderPath.Remove(tagFolderPath.Length - 1);
            foreach (var data in datas)
            {
                var dragedItem = data as ResourceInfo;
                var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(dragedItem.AbsInfoFileName).Replace("\\", "/");
                if (path[path.Length - 1] == '/')
                    path = path.Remove(path.Length - 1);
                if (path.Equals(tagFolderPath))
                {
                    return enDropResult.Denial_SamePath;
                }
            }

            return enDropResult.Allow;
        }

        public event FolderItem.Delegate_OnItemDragEnter OnFolderItemDragEnter;
        public event FolderItem.Delegate_OnItemDragLeave OnFolderItemDragLeave;
        public event FolderItem.Delegate_OnItemDragOver OnFolderItemDragOver;
        public event FolderItem.Delegate_OnItemDrop OnFolderItemDrop;
        public bool FolderItem_OnDragEnter(FolderItem item, System.Windows.DragEventArgs e)
        {
            if (Program.FolderDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
            {
                // 路径拖动
                e.Handled = true;
                switch (AllowFolderItemDrop(item, e))
                {
                    case enDropResult.Allow:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "移动到" + item.PathName;
                        return true;
                    case enDropResult.Denial_NoDragAbleObject:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "无可移动对象";
                        break;
                    case enDropResult.Denial_SamePath:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "不能移动到同目录";
                        break;
                    case enDropResult.Denial_SubFolder:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "不能移动到子目录";
                        break;
                    case enDropResult.Denial_UnknowFormat:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "不合法的格式";
                        break;
                    case enDropResult.Denial_TargetPathIsEmpty:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "目标目录为空";
                        break;
                }
            }
            else if (Program.ResourcItemDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
            {
                // 资源文件拖动
                e.Handled = true;

                switch(AllowResourceItemDrop(item, e))
                {
                    case enDropResult.Allow:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "移动到" + item.PathName;
                        return true;
                    case enDropResult.Denial_NoDragAbleObject:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "无可移动对象";
                        break;
                    case enDropResult.Denial_SamePath:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "不能移动到同目录";
                        break;
                    case enDropResult.Denial_SubFolder:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "不能移动到子目录";
                        break;
                    case enDropResult.Denial_UnknowFormat:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "不合法的格式";
                        break;
                    case enDropResult.Denial_TargetPathIsEmpty:
                        EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "目标目录为空";
                        break;
                }
            }
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var datas = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (datas == null)
                    return false;
                if (datas.Length == 0)
                    return false;
                if(CheckFileDropAvailable(datas))
                {
                    e.Effects = DragDropEffects.Link;
                    e.Handled = true;
                    return true;
                }
            }
            else
                OnFolderItemDragEnter?.Invoke(item, e);

            return false;
        }
        public bool FolderItem_OnDragLeave(FolderItem item, System.Windows.DragEventArgs e)
        {
            if (Program.FolderDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
            }
            else if (Program.ResourcItemDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
            }
            else
            {
                OnFolderItemDragLeave?.Invoke(item, e);
            }

            return false;
        }
        public bool FolderItem_OnDragOver(FolderItem item, System.Windows.DragEventArgs e)
        {
            if (Program.FolderDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
            {
                e.Handled = true;
                if (AllowFolderItemDrop(item, e) == enDropResult.Allow)
                {
                    e.Effects = DragDropEffects.Move;
                    return true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    return false;
                }
            }
            else if(Program.ResourcItemDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
            {
                e.Handled = true;
                if(AllowResourceItemDrop(item, e) == enDropResult.Allow)
                {
                    e.Effects = DragDropEffects.Move;
                    return true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    return false;
                }
            }
            else
            {
                OnFolderItemDragOver?.Invoke(item, e);
            }

            return false;
        }
        public bool FolderItem_OnDrop(FolderItem item, System.Windows.DragEventArgs e)
        {
            if (Program.FolderDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
            {
                e.Handled = true;
                if (AllowFolderItemDrop(item, e) == enDropResult.Allow)
                {
                    var formats = e.Data.GetFormats();
                    if (formats == null || formats.Length == 0)
                        return false;

                    var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                    if (datas == null)
                        return false;

                    foreach (var data in datas)
                    {
                        var dragedItem = data as FolderItem;
                        if (dragedItem == null)
                            continue;
                        if (string.IsNullOrEmpty(dragedItem.AbsolutePath))
                            continue;
                        if (!System.IO.Directory.Exists(dragedItem.AbsolutePath))
                            continue;

                        var tagPath = item.AbsolutePath + "/" + dragedItem.PathName;
                        try
                        {
                            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                                {
                                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"版本控制:移动失败！源(dragedItem.AbsolutePath), 目标(tagPath)");
                                    }
                                    else
                                    {
                                        EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultMove) =>
                                        {
                                            if (resultMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                            {
                                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"版本控制:移动失败！源(dragedItem.AbsolutePath), 目标(tagPath)");
                                            }
                                        }, dragedItem.AbsolutePath, tagPath, $"AutoCommit 从{dragedItem.AbsolutePath}移动到{tagPath}");
                                    }
                                }, dragedItem.AbsolutePath);
                            }
                            else
                                System.IO.Directory.Move(dragedItem.AbsolutePath, tagPath);

                            dragedItem.AbsolutePath = tagPath;
                            var parentItem = dragedItem.Parent as FolderItem;
                            if (parentItem != null)
                            {
                                parentItem.Items.Remove(dragedItem);
                            }

                            item.Items.Add(dragedItem);
                        }
                        catch (System.IO.PathTooLongException)
                        {
                            EditorCommon.MessageBox.Show(tagPath + "路径太长");
                        }
                        catch (System.IO.IOException)
                        {
                            EditorCommon.MessageBox.Show("目录" + tagPath + "已存在");
                        }
                    }
                }
            }
            else if (Program.ResourcItemDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
            {
                e.Handled = true;
                if (AllowResourceItemDrop(item, e) == enDropResult.Allow)
                {
                    var formats = e.Data.GetFormats();
                    if (formats == null || formats.Length == 0)
                        return false;

                    var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                    if (datas == null)
                        return false;

                    foreach (var data in datas)
                    {
                        var dragedItem = data as ResourceInfo;
                        if(dragedItem.MoveToFolder(item.AbsolutePath))
                        {
                            CurrentResources.Remove(dragedItem);
                        }
                        else
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:移动资源{dragedItem.Name}失败");
                        }
                    }

                    UpdateCountString();
                }
            }
            else if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var datas = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (datas == null)
                    return false;
                if (datas.Length == 0)
                    return false;
                if (CheckFileDropAvailable(datas))
                {
                    ImportResources(datas);
                }
            }
            else
            {
                OnFolderItemDrop?.Invoke(item, e);
            }

            return false;
        }

        Point mMouseDownPos = new Point();
        public void FolderItem_MouseMove(object sender, MouseEventArgs e)
        {
            var folderItem = sender as FolderItem;
            if (folderItem == null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(sender as FrameworkElement);
                if(((pos.X - mMouseDownPos.X) > 3) ||
                   ((pos.Y - mMouseDownPos.Y) > 3))
                {
                    EditorCommon.DragDrop.DragDropManager.Instance.StartDrag(Program.FolderDragType, new EditorCommon.DragDrop.IDragAbleObject[] { folderItem });
                }
            }
        }
        public void FolderItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var folderItem = sender as FolderItem;
            if (folderItem == null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mMouseDownPos = e.GetPosition(folderItem);
            }
        }
        #endregion

        #region 资源控件拖动
        bool mStartItemDrag = false;
        Point mStartDragPoint = new Point();
        private bool IsMouseInSelectedResourceItem()
        {
            foreach (var item in ListBox_Resources.SelectedItems)
            {
                var listBoxItem = ListBox_Resources.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (listBoxItem == null)
                    continue;
                
                var pos = Mouse.GetPosition(listBoxItem);
                if (pos.X >= 0 && pos.X < listBoxItem.ActualWidth &&
                    pos.Y >= 0 && pos.Y < listBoxItem.ActualHeight)
                {
                    return true;
                }
            }

            return false;
        }

        int mouseClickTime = 0;
        private void ListBox_ResourceItem_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if(IsMouseInSelectedResourceItem())
                {
                    mStartItemDrag = true;
                    mStartDragPoint = e.GetPosition(ListBox_Resources);
                    e.Handled = true;
                }

                mouseClickTime += 1;
                var timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
                timer.Tick += (s, e1) =>
                {
                    timer.IsEnabled = false;
                    mouseClickTime = 0;
                };
                timer.IsEnabled = true;
                if((mouseClickTime % 2) == 0)
                {
                    timer.IsEnabled = false;
                    mouseClickTime = 0;

                    if(ListBox_Resources.SelectedItems.Count > 0)
                    {
                        var rE = ListBox_Resources.SelectedItems[0] as IResourceInfoEditor;
                        if (rE != null)
                            rE.OpenEditor();
                    }
                }
            }
        }

        private void ListBox_ResourceItem_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void ListBox_ResourceItem_MouseMove(object sender, MouseEventArgs e)
        {
            if(mStartItemDrag)
            {
                var pt = e.GetPosition(ListBox_Resources);
                if(Math.Abs(pt.X - mStartDragPoint.X) > 3 || Math.Abs(pt.Y - mStartDragPoint.Y) > 3)
                {
                    var resItems = new EditorCommon.DragDrop.IDragAbleObject[ListBox_Resources.SelectedItems.Count];
                    for(int i=0; i<ListBox_Resources.SelectedItems.Count; i++)
                    {
                        resItems[i] = ListBox_Resources.SelectedItems[i] as EditorCommon.DragDrop.IDragAbleObject;
                    }
                    EditorCommon.DragDrop.DragDropManager.Instance.StartDrag(Program.ResourcItemDragType, resItems);

                    mStartItemDrag = false;
                }
            }
        }

        private void ListBox_ResourceItem_MouseUp(object sender, MouseEventArgs e)
        {
            var item = sender as ListBoxItem;
            if(mStartItemDrag == true)
            {
                var events = EventManager.GetRoutedEvents();
                foreach(var evt in events)
                {
                    if(evt.Name == "MouseDown")
                    {
                        var eg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                        eg.RoutedEvent = evt;
                        item.RaiseEvent(eg);
                        break;
                    }
                }
            }
            mStartItemDrag = false;
        }

        EditorCommon.DragDrop.DropAdorner mListBoxResourceItemDropAdorner;
        private void ListBox_ResourceItem_DragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;

            var item = sender as FrameworkElement;

            mListBoxResourceItemDropAdorner = new EditorCommon.DragDrop.DropAdorner(item);
            var data = item.DataContext as IResourceInfoDragDrop;
            if (data == null || data.DragEnter(e) == false)
                mListBoxResourceItemDropAdorner.IsAllowDrop = false;
            else
                mListBoxResourceItemDropAdorner.IsAllowDrop = true;

            var pos = e.GetPosition(item);
            if(pos.X > 0 && pos.X < item.ActualWidth &&
               pos.Y > 0 && pos.Y < item.ActualHeight)
            {
                var layer = AdornerLayer.GetAdornerLayer(this);
                layer.Add(mListBoxResourceItemDropAdorner);
            }
        }
        private void ListBox_ResourceItem_DragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;

            var item = sender as FrameworkElement;
            var data = item.DataContext as IResourceInfoDragDrop;
            if (data != null)
                data.DragLeave(e);

            if(mListBoxResourceItemDropAdorner != null)
            {
                var layer = AdornerLayer.GetAdornerLayer(this);
                layer.Remove(mListBoxResourceItemDropAdorner);
            }
        }
        private void ListBox_ResourceItem_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;

            var item = sender as FrameworkElement;
            var data = item.DataContext as IResourceInfoDragDrop;
            if (data != null)
                data.DragOver(e);
        }
        private void ListBox_ResourceItem_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;

            var item = sender as FrameworkElement;
            var data = item.DataContext as IResourceInfoDragDrop;
            if (data != null)
                data.Drop(e);

            if (mListBoxResourceItemDropAdorner != null)
            {
                var layer = AdornerLayer.GetAdornerLayer(this);
                layer.Remove(mListBoxResourceItemDropAdorner);
            }
        }

        #endregion

        #endregion

        private void ImportResources(string[] resourceFiles)
        {
            if (resourceFiles == null)
                return;

            if(string.IsNullOrEmpty(CurrentAbsFolder))
            {
                EditorCommon.MessageBox.Show("请先选择一个文件夹后再进行导入操作!");
                return;
            }

            StartProcess(() =>
            {
                var resDir = (CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory).Replace("/", "\\");

                ProcessPercent = 0;
                ProcessingInfo = "正在导入资源...";
                mProcessFinishAction = new Action(() =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        ShowSourcesInDir(CurrentAbsFolder);
                    }));
                });
                var count = resourceFiles.Length;
                EditorCommon.MessageBox.enMessageBoxResult messageBoxResult = EditorCommon.MessageBox.enMessageBoxResult.None;
                float idx = 0;
                foreach (var file in resourceFiles)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        var fileInfo = new System.IO.FileInfo(file);
                        if (fileInfo.Directory.FullName.Replace("/", "\\").Contains(resDir))
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"不能导入已在资源文件夹内的文件({fileInfo.Name})");
                            return;
                        }

                        var tagFile = CurrentAbsFolder + "/" + fileInfo.Name;

                        var resInfo = CreateResourceInfoFromResource(tagFile);
                        if (resInfo == null)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"导入文件{file}失败，目标类型不支持");
                            return;
                        }

                        resInfo.Save();

                        if (System.IO.File.Exists(tagFile))
                        {
                            switch (messageBoxResult)
                            {
                                case EditorCommon.MessageBox.enMessageBoxResult.YesAll:
                                    System.IO.File.Copy(file, tagFile, true);
                                    break;
                                case EditorCommon.MessageBox.enMessageBoxResult.NoAll:
                                    return;
                                default:
                                    {
                                        messageBoxResult = EditorCommon.MessageBox.Show("文件" + fileInfo.Name + "已存在，是否覆盖", "警告", EditorCommon.MessageBox.enMessageBoxButton.Yes_YesAll_No_NoAll);
                                        switch (messageBoxResult)
                                        {
                                            case EditorCommon.MessageBox.enMessageBoxResult.Yes:
                                                System.IO.File.Copy(file, tagFile, true);
                                                break;
                                            case EditorCommon.MessageBox.enMessageBoxResult.YesAll:
                                                System.IO.File.Copy(file, tagFile, true);
                                                break;
                                            case EditorCommon.MessageBox.enMessageBoxResult.No:
                                            case EditorCommon.MessageBox.enMessageBoxResult.NoAll:
                                                return;
                                        }

                                    }

                                    break;
                            }
                        }
                        else
                            System.IO.File.Copy(file, tagFile);

                        idx += 1;
                        ProcessPercent = idx / count;
                    }));
                }
            });
        }

        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(CurrentAbsFolder))
            {
                EditorCommon.MessageBox.Show("请先选择一个文件夹后再进行导入操作!");
                return;
            }

            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            
            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportResources(ofd.FileNames);
            }

        }

        #region 长时间处理

        Visibility mProcessingVisible = Visibility.Collapsed;
        public Visibility ProcessingVisible
        {
            get { return mProcessingVisible; }
            set
            {
                mProcessingVisible = value;
                OnPropertyChanged("ProcessingVisible");
            }
        }

        string mProcessingInfo = "";
        public string ProcessingInfo
        {
            get { return mProcessingInfo; }
            set
            {
                mProcessingInfo = value;
                OnPropertyChanged("ProcessingInfo");
            }
        }

        float mProcessPercent = 0;
        public float ProcessPercent
        {
            get { return mProcessPercent; }
            set
            {
                mProcessPercent = value;
                OnPropertyChanged("ProcessPercent");
            }
        }

        System.Threading.Thread mProcessThread;
        Action mProcessAction;
        Action mProcessFinishAction;
        void StartProcessThread()
        {
            mProcessThread = new System.Threading.Thread(new System.Threading.ThreadStart(DoProcess));
            mProcessThread.Name = "BrowserControl长时间处理操作线程";
            mProcessThread.IsBackground = true;
            mProcessThread.Start();
        }

        public void StartProcess(Action doAction)
        {
            ProcessingVisible = Visibility.Visible;
            mProcessAction = doAction;
            if (mProcessThread == null)
                StartProcessThread();
        }

        void DoProcess()
        {
            if (mProcessAction == null)
                return;

            mProcessAction.Invoke();

            ProcessingVisible = Visibility.Collapsed;
            mProcessThread = null;

            mProcessFinishAction.Invoke();
        }

        #endregion

        #region 资源显示大小

        double mChildWidth = 150;
        double mChildHeight = 195;
        private void Slider_ResourceSnapSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mResourceWrapPanel == null)
                return;

            mResourceWrapPanel.ChildWidth = mChildWidth * e.NewValue;
            mResourceWrapPanel.ChildHeight = mChildHeight * e.NewValue;

            mResourceWrapPanel.ScrollToIndex(ListBox_Resources.SelectedIndex);
        }

        #endregion

        private void ListBox_Resources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCountString();

            if (ListBox_Resources.SelectedItems.Count == 0)
                return;

            var resinfo = ListBox_Resources.SelectedItems[0] as ResourceInfo;
            if (resinfo != null)
                resinfo.SetSelectedObjectData();            
        }

        private void RadioButton_List_Checked(object sender, RoutedEventArgs e)
        {
            ListBoxItem selectedObject = null;
            if (ListBox_Resources.SelectedItems.Count > 0)
            {
                selectedObject = ListBox_Resources.ItemContainerGenerator.ContainerFromItem(ListBox_Resources.SelectedItems[0]) as ListBoxItem;
            }

            Grid_ListTitle.Visibility = Visibility.Visible;
            //ListBox_Resources.ItemsPanel = this.TryFindResource("StackPanelTemplate") as ItemsPanelTemplate;
            ListBox_Resources.ItemTemplate = this.TryFindResource("ResourceInfoDataTemplate_List") as DataTemplate;
            mResourceWrapPanel = Program.VisualTreeChildSearch<VirtualizingWrapPanel>(ListBox_Resources) as VirtualizingWrapPanel;
            mResourceWrapPanel.WrapType = VirtualizingWrapPanel.enWrapType.List;
            mResourceWrapPanel.ChildHeight = 30;
            if(ListBox_Resources.SelectedIndex < 0)
            {
                mResourceWrapPanel.SetVerticalOffset(0);
            }
            else
            {
                mResourceWrapPanel.ScrollToIndex(ListBox_Resources.SelectedIndex);
            }
            Slider_ResourceItemSize.IsEnabled = false;

            if (selectedObject != null)
                selectedObject.BringIntoView();
        }
        private void RadioButton_Tile_Checked(object sender, RoutedEventArgs e)
        {
            if(VisualTreeHelper.GetChildrenCount(ListBox_Resources) > 0)
            {
                ListBoxItem selectedObject = null;
                if (ListBox_Resources.SelectedItems.Count > 0)
                {
                    selectedObject = ListBox_Resources.ItemContainerGenerator.ContainerFromItem(ListBox_Resources.SelectedItems[0]) as ListBoxItem;
                }

                Grid_ListTitle.Visibility = Visibility.Collapsed;
                //ListBox_Resources.ItemsPanel = this.TryFindResource("WrapPanelTemplate") as ItemsPanelTemplate;
                ListBox_Resources.ItemTemplate = this.TryFindResource("ResourceInfoDataTemplate_Wrap") as DataTemplate;
                mResourceWrapPanel = Program.VisualTreeChildSearch<VirtualizingWrapPanel>(ListBox_Resources) as VirtualizingWrapPanel;
                mResourceWrapPanel.ChildWidth =  mChildWidth * Slider_ResourceItemSize.Value;
                mResourceWrapPanel.ChildHeight = mChildHeight * Slider_ResourceItemSize.Value;
                mResourceWrapPanel.WrapType = VirtualizingWrapPanel.enWrapType.Tile;
                if (ListBox_Resources.SelectedIndex < 0)
                {
                    mResourceWrapPanel.SetVerticalOffset(0);
                }
                else
                {
                    mResourceWrapPanel.ScrollToIndex(ListBox_Resources.SelectedIndex);
                }
                Slider_ResourceItemSize.IsEnabled = true;

                if(selectedObject != null)
                    selectedObject.BringIntoView();

            }
        }

        #region 资源创建
        private void ToggleButton_Create_Loaded(object sender, RoutedEventArgs e)
        {
            var menu = TryFindResource("CreateMenu") as ContextMenu;
            if (menu == null)
                return;

            menu.PlacementTarget = ToggleButton_Create;
            menu.SetBinding(ContextMenu.IsOpenProperty, new Binding("IsChecked") { Source = ToggleButton_Create, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            //ToggleButton_Create.ContextMenu = null;
        }

        public void InitializeCreateMenu()
        {
            if (mResourceInfoProcessers == null)
                return;
            var menu = TryFindResource("CreateMenu") as ContextMenu;
            if (menu == null)
                return;

            menu.Items.Clear();
            try
            {
                foreach (var processer in mResourceInfoProcessers)
                {
                    var resCreateInfo = processer.Value as IResourceInfoCreateEmpty;
                    if(resCreateInfo == null)
                        continue;

                    var menuNames = new List<string>(resCreateInfo.CreateMenuPath.Split('/'));

                    GenerateCreateMenu(menu.Items, menuNames, processer);
                }
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }
        private void GenerateCreateMenu(ItemCollection items, List<string> menuNames, Lazy<ResourceInfo, IResourceInfoMetaData> processer)
        {
            if(items.Count == 0)
            {
                GenerateCreateMenuItem(items, menuNames, processer);
            }
            else
            {
                bool bFind = false;
                foreach(var item in items)
                {
                    var menuItem = item as MenuItem;
                    if (menuItem == null)
                        continue;

                    if(menuItem.Header.ToString() == menuNames[0])
                    {
                        if(menuNames.Count == 1)
                        {
                            GenerateCreateMenuItem(items, menuNames, processer);
                        }
                        else
                        {
                            menuNames.RemoveAt(0);
                            GenerateCreateMenu(menuItem.Items, menuNames, processer);
                        }

                        bFind = true;
                        break;
                    }
                }

                if(!bFind)
                {
                    GenerateCreateMenuItem(items, menuNames, processer);
                }
            }
        }
        private void GenerateCreateMenuItem(ItemCollection items, List<string> menuNames, Lazy<ResourceInfo, IResourceInfoMetaData> processer)
        {
            MenuItem item = new MenuItem()
            {
                Header = menuNames[0],
                Style = TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "MenuItem_Default")) as Style
            };
            items.Add(item);
            if (menuNames.Count > 1)
            {
                menuNames.RemoveAt(0);
                GenerateCreateMenu(item.Items, menuNames, processer);
            }
            else
            {
                item.Tag = processer;
                item.Icon = new Image()
                {
                    Source = processer.Value.ResourceIcon,
                    Height = 24,
                };
                item.Click += (sender, e)=>
                {
                    if(string.IsNullOrEmpty(CurrentAbsFolder))
                    {
                        EditorCommon.MessageBox.Show("请先选择一个文件夹在进行创建！");
                        return;
                    }

                    var res = processer as Lazy<ResourceInfo, IResourceInfoMetaData>;
                    if (res == null)
                        return;

                    var resCreateInfo = res.Value as IResourceInfoCreateEmpty;
                    if (resCreateInfo == null)
                        return;

                    Window createWin = null;
                    var resCCInfo = res.Value as IResourceInfoCustomCreateDialog;
                    if(resCCInfo != null)
                    {
                        createWin = resCCInfo.GetCustomCreateDialogWindow();
                        if (createWin == null)
                            return;

                        if (createWin.ShowDialog() == false)
                            return;
                    }
                    else
                    {
                        var inputWindow = new InputWindow.InputWindow();
                        inputWindow.Title = $"创建{res.Value.ResourceTypeName}";
                        inputWindow.Description = $"输入新{res.Value.ResourceTypeName}的名称:";
                        inputWindow.Value = $"新建{res.Value.ResourceTypeName}";

                        var resNameInfo = res.Value as IResourceInfoValidName;
                        if (resNameInfo != null)
                        {
                            inputWindow.Value = resNameInfo.GetValidName();
                        }
                    
                        if (inputWindow.ShowDialog((value, cultureInfo) =>
                        {
                            if (value == null)
                                return new ValidationResult(false, "内容不合法");
                            return resCreateInfo.ResourceNameAvailable(CurrentAbsFolder + "/" + value.ToString());
                        }) == false)
                            return;

                        createWin = inputWindow;
                    }

                    var resourceInfo = resCreateInfo.CreateEmptyResource(CurrentAbsFolder, createWin);
                    resourceInfo.ParentBrowser = this;
                    CurrentResources.Add(resourceInfo);
                    UpdateCountString();

                    if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{resourceInfo.ResourceType}{resourceInfo.Name} {resourceInfo.AbsInfoFileName}使用版本控制添加失败!");
                            }
                        }, resourceInfo.AbsInfoFileName, $"AutoCommit 创建{resourceInfo.ResourceTypeName}{resourceInfo.Name}");
                    }
                };
            }
        }

        #endregion

        private void Button_CopySelected_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in ListBox_Resources.SelectedItems)
            {
                var resInfo = item as IResourceInfoCopy;
                if (resInfo == null)
                    continue;

                var copyedRes = resInfo.CopyResource();
                copyedRes.Save();
                CurrentResources.Add(copyedRes);
            }

            UpdateCountString();
        }

        private void Button_SaveSelected_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in ListBox_Resources.SelectedItems)
            {
                var resinfo = item as ResourceInfo;
                if (resinfo == null)
                    continue;

                resinfo.Save();
            }
        }

        Dictionary<Guid, ResourceInfo> mDirtyResources = new Dictionary<Guid, ResourceInfo>();
        private void Button_SaveAll_Click(object sender, RoutedEventArgs e)
        {
            foreach(var res in mDirtyResources.Values)
            {
                res.Save();
            }
        }

        private void Button_DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Resources.SelectedItems.Count <= 0)
                return;

            var list = new List<ResourceInfo>();
            foreach(var item in ListBox_Resources.SelectedItems)
            {
                var resInfo = item as ResourceInfo;
                if (resInfo == null)
                    continue;

                list.Add(resInfo);
            }

            foreach(var resInfo in list)
            {
                mDirtyResources.Remove(resInfo.Id);
                resInfo.DeleteResource();
                CurrentResources.Remove(resInfo);
            }

            mResourceWrapPanel.SetVerticalOffset(0);

            UpdateCountString();
        }

        #region 拖动到游戏窗口

        private void InitializeGameWindowDragDrop()
        {
            EditorCommon.WorldEditorOperation.OnGameWindowDragEnter += WorldEditorOperation_OnGameWindowDragEnter;
            EditorCommon.WorldEditorOperation.OnGameWindowDragLeave += WorldEditorOperation_OnGameWindowDragLeave;
            EditorCommon.WorldEditorOperation.OnGameWindowDragOver += WorldEditorOperation_OnGameWindowDragOver;
            EditorCommon.WorldEditorOperation.OnGameWindowDragDrop += WorldEditorOperation_OnGameWindowDragDrop;
        }

        private void WorldEditorOperation_OnGameWindowDragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            EditorCommon.DragDrop.DragDropManager.Instance.ShowFlyWindow(false);
            e.Effect = System.Windows.Forms.DragDropEffects.Copy;
            foreach(var dragObj in EditorCommon.DragDrop.DragDropManager.Instance.DragedObjectList)
            {
                var dtg = dragObj as IResourceInfoDragToGameWindow;
                if (dtg == null)
                    continue;

                dtg.OnDragEnterGameWindow(sender as System.Windows.Forms.Form, e);
            }
        }
        private void WorldEditorOperation_OnGameWindowDragLeave(object sender, EventArgs e)
        {
            EditorCommon.DragDrop.DragDropManager.Instance.ShowFlyWindow(true);
            foreach(var dragObj in EditorCommon.DragDrop.DragDropManager.Instance.DragedObjectList)
            {
                var dtg = dragObj as IResourceInfoDragToGameWindow;
                if (dtg == null)
                    continue;

                dtg.OnDragLeaveGameWindow(sender as System.Windows.Forms.Form, e);
            }
        }
        private void WorldEditorOperation_OnGameWindowDragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            foreach(var dragObj in EditorCommon.DragDrop.DragDropManager.Instance.DragedObjectList)
            {
                var dtg = dragObj as IResourceInfoDragToGameWindow;
                if (dtg == null)
                    continue;

                dtg.OnDragOverGameWindow(sender as System.Windows.Forms.Form, e);
            }
        }
        private void WorldEditorOperation_OnGameWindowDragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            List<CCore.World.Actor> actors = new List<CCore.World.Actor>();
            foreach (var dragObj in EditorCommon.DragDrop.DragDropManager.Instance.DragedObjectList)
            {
                var dtg = dragObj as IResourceInfoDragToGameWindow;
                if (dtg == null)
                    continue;

                var obj = dtg.OnDragDropGameWindow(sender as System.Windows.Forms.Form, e);
                var act = obj as CCore.World.Actor;
                if(act != null)
                {
                    actors.Add(act);
                }
            }

            EditorCommon.Assist.Assist.TrySelectActors(actors.ToArray());
        }


        #endregion
    }
}
