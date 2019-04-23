using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ResourcesBrowser
{
    /// <summary>
    /// 资源提示窗口标记
    /// </summary>
    public class ResourceToolTipAttribute : Attribute { }

    /// <summary>
    /// 可以强制重新加载的资源
    /// </summary>
    public interface IResourceInfoForceReload
    {
        void ForceReload();
    }

    /// <summary>
    /// 可以拖动到游戏窗口的资源
    /// </summary>
    public interface IResourceInfoDragToGameWindow
    {
        void OnDragEnterGameWindow(System.Windows.Forms.Form gameWindow, System.Windows.Forms.DragEventArgs e);
        void OnDragLeaveGameWindow(System.Windows.Forms.Form gameWindow, EventArgs e);
        void OnDragOverGameWindow(System.Windows.Forms.Form gameWindow, System.Windows.Forms.DragEventArgs e);
        object OnDragDropGameWindow(System.Windows.Forms.Form gameWindow, System.Windows.Forms.DragEventArgs e);
    }

    /// <summary>
    /// 资源合法名字创建
    /// </summary>
    public interface IResourceInfoValidName
    {
        string GetValidName();
    }

    /// <summary>
    /// 定制创建对话框
    /// </summary>
    public interface IResourceInfoCustomCreateDialog
    {
        Window GetCustomCreateDialogWindow();
    }

    /// <summary>
    /// 创建空资源
    /// </summary>
    public interface IResourceInfoCreateEmpty
    {
        /// <summary>
        /// 判断资源名称合法性
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <returns></returns>
        ValidationResult ResourceNameAvailable(string resourceName);
        /// <summary>
        /// 创建空白资源
        /// </summary>
        /// <param name="Absfolder">新资源所在的路径</param>
        /// <param name="resourceName">资源名称</param>
        /// <returns></returns>
        ResourceInfo CreateEmptyResource(string Absfolder, object createData);
        /// <summary>
        /// 创建目录路径，格式为(目录名称/目录名称)，不能为空
        /// </summary>
        string CreateMenuPath { get; }
    }
    /// <summary>
    /// 复制资源
    /// </summary>
    public interface IResourceInfoCopy
    {
        /// <summary>
        /// 用当前资源来复制资源
        /// </summary>
        /// <returns>复制出来的新资源</returns>
        ResourceInfo CopyResource();
    }
    /// <summary>
    /// 资源包含资源编辑器
    /// </summary>
    public interface IResourceInfoEditor
    {
        /// <summary>
        /// 打开编辑器
        /// </summary>
        void OpenEditor();
    }
    /// <summary>
    /// 资源可以被拖放
    /// </summary>
    public interface IResourceInfoDragDrop
    {
        /// <summary>
        /// 当对象被拖入正作为放置目标元素边界时调用
        /// </summary>
        /// <param name="e">拖放参数</param>
        /// <returns>可以拖放返回true，不能拖放返回false</returns>
        bool DragEnter(System.Windows.DragEventArgs e);
        /// <summary>
        /// 当对象被拖出正作为没有放置的放置目标的元素边界时发生
        /// </summary>
        /// <param name="e">拖放参数</param>
        void DragLeave(System.Windows.DragEventArgs e);
        /// <summary>
        /// 当正作为放置目标的元素边界内的拖动对象时持续发生
        /// </summary>
        /// <param name="e">拖放参数</param>
        void DragOver(System.Windows.DragEventArgs e);
        /// <summary>
        /// 当对象被拖入正作为放置目标的元素边界时发生
        /// </summary>
        /// <param name="e">拖放参数</param>
        void Drop(System.Windows.DragEventArgs e);
    }

    // 使用插件来实现不同资源的处理
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceInfoAttribute : ExportAttribute
    {
        public ResourceInfoAttribute()
            : base(typeof(ResourceInfo))
        {

        }
        
        /// <summary>
        /// 资源类型
        /// </summary>
        public string ResourceInfoType { get; set; }
        /// <summary>
        /// 资源扩展名
        /// </summary>
        public string[] ResourceExts { get; set; }
    }

    public interface IResourceInfoMetaData
    {
        string ResourceInfoType { get; }
        string[] ResourceExts { get; }
    }

    /// <summary>
    /// 资源信息基类
    /// </summary>
    public abstract class ResourceInfo :  DependencyObject, INotifyPropertyChanged, EditorCommon.DragDrop.IDragAbleObject
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public delegate void Delegate_OnDirtyChanged(bool dirty);
        public event Delegate_OnDirtyChanged OnDirtyChanged;

        public static string ExtString
        {
            get;
        } = Program.ResourceInfoExt;

        public bool IsDirty
        {
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
        }
        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.Register("IsDirty", typeof(bool), typeof(ResourceInfo), new PropertyMetadata(false, new PropertyChangedCallback(IsDirtyChangedCallback)));
        static void IsDirtyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var info = sender as ResourceInfo;
            bool newValue = (bool)e.NewValue;
            if(newValue)
                info.UnsaveVisibility = Visibility.Visible;
            else
                info.UnsaveVisibility = Visibility.Collapsed;

            info.OnDirtyChanged?.Invoke(newValue);
        }

        public virtual void SetSelectedObjectData()
        {
            EditorCommon.PluginAssist.PropertyGridAssist.SetSelectedObjectData(ResourceType,new object[] { RelativeResourceFileName});
        }        

        /// <summary>
        /// 与Name值一致，用于绑定
        /// </summary>
        public string NickName
        {
            get { return (string)GetValue(NickNameProperty); }
            set { SetValue(NickNameProperty, value); }
        }
        public static readonly DependencyProperty NickNameProperty =
            DependencyProperty.Register("NickName", typeof(string), typeof(ResourceInfo), new PropertyMetadata("", new PropertyChangedCallback(NameChangedCallback)));
        static void NameChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var info = sender as ResourceInfo;
            string newValue = (string)e.NewValue;

            info.Name = newValue;
        }

        //bool mIsDirty = false;
        //public bool IsDirty
        //{
        //    get { return mIsDirty; }
        //    set
        //    {
        //        mIsDirty = value;

        //        if (mIsDirty)
        //            UnsaveVisibility = Visibility.Visible;
        //        else
        //            UnsaveVisibility = Visibility.Collapsed;

        //        OnDirtyChanged?.Invoke(mIsDirty);
        //        OnPropertyChanged("IsDirty");
        //    }
        //}

        System.Guid mId = System.Guid.NewGuid();
        [CSUtility.Support.DataValueAttribute("Id")]
        public System.Guid Id
        {
            get { return mId; }
            set
            {
                mId = value;
                OnPropertyChanged("Id");
            }
        }

        string mName = "";
        [CSUtility.Support.DataValueAttribute("Name")]
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;
                OnPropertyChanged("Name");
            }
        }        

        string mResourceType = "";
        [CSUtility.Support.DataValueAttribute("ResourceType")]
        public virtual string ResourceType
        {
            get { return mResourceType; }
            protected set
            {
                mResourceType = value;
                OnPropertyChanged("ResourceType");
            }
        }

        ImageSource mSnapshot = null;
        public ImageSource Snapshot
        {
            get { return mSnapshot; }
            set
            {
                mSnapshot = value;
                if(mSnapshot != null)
                    StopWaitingProcess();
                OnPropertyChanged("Snapshot");
            }
        }

        Visibility mSVNLockVisibility = Visibility.Collapsed;
        public Visibility SVNLockVisibility
        {
            get { return mSVNLockVisibility; }
            set
            {
                mSVNLockVisibility = value;
                OnPropertyChanged("SVNLockVisibility");
            }
        }

        Visibility mUnsaveVisibility = Visibility.Collapsed;
        public Visibility UnsaveVisibility
        {
            get { return mUnsaveVisibility; }
            set
            {
                mUnsaveVisibility = value;
                OnPropertyChanged("UnsaveVisibility");
            }
        }

        Visibility mWaitingProcessVisibility = Visibility.Visible;
        public Visibility WaitingProcessVisibility
        {
            get { return mWaitingProcessVisibility; }
            set
            {
                mWaitingProcessVisibility = value;
                OnPropertyChanged("WaitingProcessVisibility");
            }
        }

        /// <summary>
        /// 资源类型名称
        /// </summary>
        [ResourceToolTipAttribute]
        [DisplayName("类型")]
        public abstract string ResourceTypeName { get; }
        /// <summary>
        /// 资源类型笔刷（用不同颜色标识不同资源）
        /// </summary>
        public abstract Brush ResourceTypeBrush { get; set; }
        /// <summary>
        /// 资源图标
        /// </summary>
        public abstract ImageSource ResourceIcon { get; set; }
        
        public BrowserControl ParentBrowser;
        string mAbsInfoFileName = "";
        /// <summary>
        /// ResourceInfo文件全名称（包含完整路径）
        /// </summary>
        public string AbsInfoFileName
        {
            get { return mAbsInfoFileName; }
            protected set
            {
                mAbsInfoFileName = value.Replace("\\", "/");
                AbsResourceFileName = mAbsInfoFileName.Replace(Program.ResourceInfoExt, "");
                RelativeResourceFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName);
                ResourceFileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(AbsResourceFileName);
                RelativeInfoFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mAbsInfoFileName);
                AbsPath = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(mAbsInfoFileName);
                RelativePath = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsPath);
                FileExtension = CSUtility.Support.IFileManager.Instance.GetFileExtension(AbsResourceFileName).Replace(".", "").ToLower();
            }
        }

        /// <summary>
        /// 资源文件名称（不包含路径，包含扩展名）
        /// </summary>
        public virtual string ResourceFileName
        {
            get;
            protected set;
        }
        /// <summary>
        /// 资源全文件名（绝对路径）
        /// </summary>
        public virtual string AbsResourceFileName
        {
            get;
            protected set;
        }
        /// <summary>
        /// 资源文件名（路径相对于release）
        /// </summary>
        public virtual string RelativeResourceFileName
        {
            get;
            protected set;
        }
        /// <summary>
        /// 资源文件扩展名
        /// </summary>
        public virtual string FileExtension
        {
            get; set;
        }
        /// <summary>
        /// ResourceInfo文件名（路径相对于release）
        /// </summary>
        public virtual string RelativeInfoFileName
        {
            get;
            protected set;
        } = "";

        /// <summary>
        /// 资源相对路径
        /// </summary>
        [ResourceToolTipAttribute]
        [DisplayName("路径")]
        public string RelativePath
        {
            get;
            protected set;
        } = "";

        // 绝对路径
        [Browsable(false)]
        public string AbsPath
        {
            get;
            protected set;
        } = "";
        
        internal void StopWaitingProcess()
        {
            WaitingProcessVisibility = Visibility.Collapsed;
        }

        public virtual void Save()
        {
            CSUtility.Support.IConfigurator.SaveProperty(this, "ResourceInfo", AbsInfoFileName);
            //IsDirty = false;
        }
        public virtual void Load(string absFileName)
        {
            absFileName.Replace("\\", "/");
            CSUtility.Support.IConfigurator.FillProperty(this, absFileName);
            AbsInfoFileName = absFileName;
        }

        /// <summary>
        /// 显示缩略图
        /// </summary>
        /// <param name="force">强制刷新</param>
        public void TryShowSnapshot(bool force)
        {
            if (Snapshot == null || force)
                SnapshotProcess.ImageQueue.Instance.Queue(this);
        }

        /// <summary>
        /// 筛选操作
        /// </summary>
        /// <param name="filterString">筛选关键字</param>
        /// <returns></returns>
        public virtual bool DoFilter(string filterString)
        {
            if (Name.ToLower().Contains(filterString.ToLower()))
                return true;
            return false;
        }
        
        /// <summary>
        /// 获取拖动的可视对象
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetDragVisual()
        {
            if(ParentBrowser != null && ParentBrowser.ListBox_Resources != null)
            {
                return ParentBrowser.ListBox_Resources.ItemContainerGenerator.ContainerFromItem(this) as FrameworkElement;
            }

            return null;
        }
        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="forceCreate">是否强制创建</param>
        /// <returns></returns>
        public abstract ImageSource GetSnapshotImage(bool forceCreate);
        /// <summary>
        /// 从导入资源创建资源信息文件
        /// </summary>
        /// <param name="resourceAbsFile">资源文件名</param>
        /// <returns></returns>
        internal ResourceInfo CreateResourceInfoFromResource(string resourceAbsFile)
        {
            var retInfo = CreateResourceInfoFromResourceOverride(resourceAbsFile);
            if (retInfo == null)
                return null;
            retInfo.AbsInfoFileName = resourceAbsFile + ResourceInfo.ExtString;
            return retInfo;
        }
        protected abstract ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile);
        /// <summary>
        /// 删除资源
        /// </summary>
        public void DeleteResource()
        {
            DeleteResourceOverride();

            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsInfoFileName}使用版本控制删除失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                        {
                            if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsInfoFileName}使用版本控制删除失败!");
                            }
                        }, AbsInfoFileName, $"AutoCommit 删除{ResourceTypeName}{Name}");
                    }
                }, AbsInfoFileName);
            }
            else
                System.IO.File.Delete(AbsInfoFileName);
        }
        protected abstract void DeleteResourceOverride();
        /// <summary>
        /// 将资源移动到目标目录
        /// </summary>
        /// <param name="absFolder">目标目录</param>
        /// <returns></returns>
        public bool MoveToFolder(string absFolder)
        {
            if (absFolder[absFolder.Length - 1] != '/' && absFolder[absFolder.Length - 1] != '\\')
                absFolder += "/";

            if (MoveToFolderOverride(absFolder) == false)
                return false;

            // 移动Info文件
            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:资源{Name}移动到目录{absFolder}失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultMove) =>
                        {
                            if (resultMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:资源{Name}移动到目录{absFolder}失败!");
                            }
                        }, AbsInfoFileName, absFolder + ResourceFileName + Program.ResourceInfoExt, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsInfoFileName)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + Program.ResourceInfoExt)}");
                    }
                }, AbsInfoFileName);
            }
            else
            {
                System.IO.File.Move(AbsInfoFileName, absFolder + ResourceFileName + Program.ResourceInfoExt);
            }

            AbsInfoFileName = absFolder + ResourceFileName + Program.ResourceInfoExt;

            return true;
        }
        protected abstract bool MoveToFolderOverride(string absFolder);
    }

    internal class CommonResourceInfo : ResourceInfo
    {
        public override string ResourceTypeName { get; }
        public override Brush ResourceTypeBrush { get; set; }
        public override ImageSource ResourceIcon { get; set; }

        public override ImageSource GetSnapshotImage(bool forceCreate)
        {
            return null;
        }
        protected override ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {
            return null;
        }

        public override void Load(string fileName)
        {
            CSUtility.Support.IConfigurator.FillProperty(this, fileName);
            AbsInfoFileName = fileName;
        }

        protected override void DeleteResourceOverride() { }
        protected override bool MoveToFolderOverride(string absFolder) { return false; }
    }
}
