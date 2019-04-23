using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ResourcesBrowser
{
    /// <summary>
    /// Interaction logic for FolderItem.xaml
    /// </summary>
    public partial class FolderItem : TreeViewItem, EditorCommon.DragDrop.IDragAbleObject, INotifyPropertyChanged
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

        string mAbsolutePath = "";
        public string AbsolutePath
        {
            get { return mAbsolutePath; }
            set
            {
                mAbsolutePath = value;
                PathName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(mAbsolutePath);
            }
        }

        string mPathName = null;
        public string PathName
        {
            get { return mPathName; }
            set
            {
                mPathName = value;
                OnPropertyChanged("PathName");
            }
        }
        
        BrowserControl mParentBrowser = null;

        // absFolder为绝对路径
        public FolderItem(string absFolder, BrowserControl parentBrowser)
        {
            InitializeComponent();

            mParentBrowser = parentBrowser;
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(Grid_Header);

            AbsolutePath = absFolder;
            Items.Clear();

            if (System.IO.Directory.Exists(absFolder))
            {
                foreach (var subFolder in System.IO.Directory.GetDirectories(absFolder))
                {
                    if (!mParentBrowser.IsFolderValid(subFolder))
                        continue;

                    FolderItem flItem = new FolderItem(subFolder, mParentBrowser);
                    flItem.OnItemDragEnter += parentBrowser.FolderItem_OnDragEnter;
                    flItem.OnItemDragLeave += parentBrowser.FolderItem_OnDragLeave;
                    flItem.OnItemDragOver += parentBrowser.FolderItem_OnDragOver;
                    flItem.OnItemDrop += parentBrowser.FolderItem_OnDrop;
                    flItem.MouseDown += parentBrowser.FolderItem_MouseDown;
                    flItem.MouseMove += parentBrowser.FolderItem_MouseMove;
                    Items.Add(flItem);
                }
            }
        }

        public void RefreshSubFolder()
        {
            if (string.IsNullOrEmpty(AbsolutePath) || mParentBrowser == null)
                return;

            Items.Clear();

            foreach (var subFolder in System.IO.Directory.GetDirectories(AbsolutePath))
            {
                if (!mParentBrowser.IsFolderValid(subFolder))
                    continue;

                FolderItem flItem = new FolderItem(subFolder, mParentBrowser);
                flItem.OnItemDragEnter += mParentBrowser.FolderItem_OnDragEnter;
                flItem.OnItemDragLeave += mParentBrowser.FolderItem_OnDragLeave;
                flItem.OnItemDragOver += mParentBrowser.FolderItem_OnDragOver;
                flItem.OnItemDrop += mParentBrowser.FolderItem_OnDrop;
                flItem.MouseDown += mParentBrowser.FolderItem_MouseDown;
                flItem.MouseMove += mParentBrowser.FolderItem_MouseMove;
                Items.Add(flItem);
            }
        }

        private void TreeViewItem_Unselected(object sender, System.Windows.RoutedEventArgs e)
        {
            var img = this.TryFindResource("Folder_CloseIcon") as Image;
            if (img != null)
            {
                Image_Icon.Source = img.Source;
            }
        }

        private void TreeViewItem_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            var img = this.TryFindResource("Folder_OpenIcon") as Image;
            if (img != null)
            {
                Image_Icon.Source = img.Source;
            }
        }

        private ValidationResult CheckFolderName(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "名称不能为空");

            string valueStr = (string)value;
            if (string.IsNullOrEmpty(valueStr))
                return new ValidationResult(false, "名称不能为空");

            if (Regex.IsMatch(valueStr, @"[\u4e00-\u9fa5]"))
                return new ValidationResult(false, "为保证系统兼容性，文件夹名称中不能包含中文");

            foreach (var invalidChar in System.IO.Path.GetInvalidFileNameChars())
            {
                if (valueStr.Contains(invalidChar))
                    return new ValidationResult(false, "名称中包含不合法的字符: " + invalidChar);
            }

            var newFolder = this.AbsolutePath + "/" + valueStr;
            if (System.IO.Directory.Exists(newFolder))
                return new ValidationResult(false, "已存在名称为" + valueStr + "的目录！");

            return new ValidationResult(true, null);
        }

        private void MenuItem_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (mParentBrowser == null)
                return;

            RefreshSubFolder();
            mParentBrowser.ShowSourcesInDir(AbsolutePath);
        }

        private void MenuItem_DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            var parItem = this.Parent as FolderItem;
            if (parItem == null)
            {
                EditorCommon.MessageBox.Show("根目录不能删除!");
                return;
            }

            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"版本控制:删除目录失败({this.AbsolutePath})");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                        {
                            if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"版本控制:删除目录失败({this.AbsolutePath})");
                            }
                        }, this.AbsolutePath, $"AutoCommit 删除目录{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(this.AbsolutePath)}");
                    }
                }, this.AbsolutePath);
            }
            else
            {
                System.IO.Directory.Delete(this.AbsolutePath, true);
            }

            parItem.Items.Remove(this);
        }

        private void MenuItem_NewFolder_Click(object sender, RoutedEventArgs e)
        {
            var win = new InputWindow.InputWindow();
            win.Description = "输入目录名称";
            win.Value = GetFolderName("new");
            if (win.ShowDialog(CheckFolderName) == false)
            {
                return;
            }

            var tagFolder = this.AbsolutePath + "/" + win.Value;
            System.IO.Directory.CreateDirectory(tagFolder);

            FolderItem flItem = new FolderItem(tagFolder, mParentBrowser);
            flItem.OnItemDragEnter += mParentBrowser.FolderItem_OnDragEnter;
            flItem.OnItemDragLeave += mParentBrowser.FolderItem_OnDragLeave;
            flItem.OnItemDragOver += mParentBrowser.FolderItem_OnDragOver;
            flItem.OnItemDrop += mParentBrowser.FolderItem_OnDrop;
            flItem.MouseDown += mParentBrowser.FolderItem_MouseDown;
            flItem.MouseMove += mParentBrowser.FolderItem_MouseMove;
            this.Items.Add(flItem);
            this.IsExpanded = true;

            EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
            {
                if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"版本控制:创建目录失败({tagFolder})");
                }
            }, tagFolder, $"AutoCommit 创建目录{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(tagFolder)}");
        }

        string GetFolderName(string name)
        {            
            int i = 1;            
            while (true)
            {
                string folder = this.AbsolutePath + "/" + name + i.ToString();
                
                if (System.IO.Directory.Exists(folder))
                {
                    ++i;
                }
                else
                {
                    return name + i.ToString();
                }
            }
        }
        private void MenuItem_Rename_Click(object sender, RoutedEventArgs e)
        {
            var win = new InputWindow.InputWindow();
            win.Description = "输入新目录名称";
            win.Value = this.PathName;
            if (win.ShowDialog(CheckFolderName) == false)
            {
                return;
            }

            if (this.PathName.ToLower() == win.Value.ToString().ToLower())
                return;

            var oldFolder = this.AbsolutePath;
            var newFolder = this.AbsolutePath.Replace(this.PathName, "") + win.Value;
            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"版本控制:移动失败，源:({oldFolder})，目标:({newFolder})");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultMove) =>
                        {
                            if (resultMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"版本控制:移动失败，源:({oldFolder})，目标:({newFolder})");
                            }
                        }, oldFolder, newFolder, $"AutoCommit 重命名目录，源:({oldFolder})，目标:({newFolder})");
                    }
                }, oldFolder);
            }
            else
            {
                System.IO.Directory.Move(oldFolder, newFolder);
            }

            AbsolutePath = newFolder;
            //var srcFolder = this.AbsolutePath;
            //var tagFolder = this.AbsolutePath.Remove(this.PathName) + win.Value;
            //System.IO.Directory.Move()
        }

        private void MenuItem_OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", AbsolutePath.Replace("/", "\\"));
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeViewItem = Program.VisualUpwardSearch<FolderItem>(e.OriginalSource as DependencyObject) as FolderItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

#region  DragDrop

        public System.Windows.FrameworkElement GetDragVisual()
        {
            var header = this.Header as System.Windows.FrameworkElement;
            return VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(header)) as System.Windows.FrameworkElement;
        }

        public delegate bool Delegate_OnItemDragEnter(FolderItem item, System.Windows.DragEventArgs e);
        public event Delegate_OnItemDragEnter OnItemDragEnter;
        public delegate bool Delegate_OnItemDragLeave(FolderItem item, System.Windows.DragEventArgs e);
        public event Delegate_OnItemDragLeave OnItemDragLeave;
        public delegate bool Delegate_OnItemDragOver(FolderItem item, System.Windows.DragEventArgs e);
        public event Delegate_OnItemDragOver OnItemDragOver;
        public delegate bool Delegate_OnItemDrop(FolderItem item, System.Windows.DragEventArgs e);
        public event Delegate_OnItemDrop OnItemDrop;

        EditorCommon.DragDrop.DropAdorner mDropAdorner;
        public EditorCommon.DragDrop.DropAdorner DropAdorner
        {
            get { return mDropAdorner; }
        }

        private void treeViewItem_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (OnItemDragEnter != null)
            {
                mDropAdorner.IsAllowDrop = OnItemDragEnter(this, e);
            }

            var pos = e.GetPosition(Grid_Header);
            if(pos.X > 0 && pos.X < Grid_Header.ActualWidth &&
               pos.Y > 0 && pos.Y < Grid_Header.ActualHeight)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
                layer.Add(mDropAdorner);
            }
        }

        private void treeViewItem_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            layer.Remove(mDropAdorner);

            if (OnItemDragLeave != null)
                OnItemDragLeave(this, e);
        }

        private void treeViewItem_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (OnItemDragOver != null)
                OnItemDragOver(this, e);
        }

        private void treeViewItem_Drop(object sender, System.Windows.DragEventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            layer.Remove(mDropAdorner);

            if (OnItemDrop != null)
                OnItemDrop(this, e);
        }

#endregion

    }
}
