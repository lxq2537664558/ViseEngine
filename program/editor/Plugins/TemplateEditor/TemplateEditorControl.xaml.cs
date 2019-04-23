using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace TemplateEditor
{
    /// <summary>
    /// Interaction logic for TemplateEditorControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "TemplateEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("窗口/模板编辑器")]
    [Guid("8FCB2C30-7705-4F1D-B90F-BB6FAF1289D8")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class TemplateEditorControl : System.Windows.Controls.UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string PluginName
        {
            get { return "模板编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "模板编辑器",
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
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

        ////////////////////////////////////////////////////////////////

        bool mButtonEnable = true;
        public bool ButtonEnable
        {
            get { return mButtonEnable; }
            set
            {
                mButtonEnable = value;
                OnPropertyChanged("ButtonEnable");
            }
        }

        ViewNodeItem mSelectedItem;

        System.Collections.ObjectModel.ObservableCollection<ViewNodeItem> mAllTreeViewItems = new System.Collections.ObjectModel.ObservableCollection<ViewNodeItem>();
        System.Collections.ObjectModel.ObservableCollection<ViewNodeItem> mShowTreeViewItems = new System.Collections.ObjectModel.ObservableCollection<ViewNodeItem>();
        public System.Collections.ObjectModel.ObservableCollection<ViewNodeItem> ShowTreeViewItems
        {
            get { return mShowTreeViewItems; }
            set
            {
                mShowTreeViewItems = value;
            }
        }
        
        object mDataTemplateInstance;
        public object DataTemplateInstance
        {
            get { return mDataTemplateInstance; }
            set
            {
                mDataTemplateInstance = value;
                OnPropertyChanged("DataTemplateInstance");
            }
        }

        string mSearchFolderText = CSUtility.Support.IFileConfig.DefaultTemplateDir;
        /// <summary>
        /// 当前搜索的目录，相对于Root
        /// </summary>
        public string SearchFolderText
        {
            get { return mSearchFolderText; }
            set
            {
                mSearchFolderText = value;
                OnPropertyChanged("SearchFolderText");
            }
        }

        bool mAutoSave = false;
        public bool AutoSave
        {
            get { return mAutoSave; }
            set
            {
                mAutoSave = value;
                OnPropertyChanged("AutoSave");
            }
        }

        string mSearchText = "";
        public string SearchText
        {
            get { return mSearchText; }
            set
            {
                mSearchText = value;
                UpdateFilterShow();
                OnPropertyChanged("SearchText");
            }
        }

        private void UpdateFilterShow()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    ShowTreeViewItems = new System.Collections.ObjectModel.ObservableCollection<ViewNodeItem>(mAllTreeViewItems);
                    foreach(var item in mAllTreeViewItems)
                    {
                        item.DisplayNameHighLightString = "";
                        item.IDHighLightString = "";
                    }
                    ItemView_Files.ItemsSource = ShowTreeViewItems;
                }
                else
                {
                    var lowSearchText = SearchText.ToLower();
                    ShowTreeViewItems.Clear();
                    foreach (var item in mAllTreeViewItems)
                    {
                        if (!string.IsNullOrEmpty(item.DisplayName) && item.DisplayName.ToLower().Contains(SearchText))
                        {
                            item.DisplayNameHighLightString = SearchText;
                            ShowTreeViewItems.Add(item);
                        }
                        else
                        {
                            item.DisplayNameHighLightString = "";
                        }

                        if (item.ID.ToString().Contains(SearchText))
                        {
                            item.IDHighLightString = SearchText;
                            if (!ShowTreeViewItems.Contains(item))
                                ShowTreeViewItems.Add(item);
                        }
                        else
                            item.IDHighLightString = "";
                    }

                    ItemView_Files.ItemsSource = ShowTreeViewItems;
                }
            });
        }

        public class CComboBoxItem
        {
            public Type Type;
            public CSUtility.Data.DataTemplateAttribute Att;

            public override string ToString()
            {
                return Type.ToString();
            }
        }

        public ExportConfig mConfig = new ExportConfig();

        public TemplateEditorControl()
        {
            InitializeComponent();
            
            ApplyConfig();
            ItemView_Files.Items.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));

            foreach (var type in CSUtility.Data.DataTemplateManagerAssist.Instance.DataTemplateManagerDictionary.Keys)
            {
                var atts = type.GetCustomAttributes(typeof(CSUtility.Data.DataTemplateAttribute), false);
                if (atts.Length <= 0)
                    continue;

                var att = atts[0] as CSUtility.Data.DataTemplateAttribute;
                var item = new CComboBoxItem()
                {
                    Type = type,
                    Att = att,
                };

                comboBox_Templates.Items.Add(item);
            }

            comboBox_Templates.SelectedIndex = 0;
            ItemView_ErrorFiles.ItemsSource = ErrorNodeItems;
        }

        private ValidationResult CheckID(object value, CComboBoxItem selectedItem, Type idType, bool CheckExist = true)
        {
            if(string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, "ID不能为空");
            if (!Regex.IsMatch(value.ToString(), @"^[0-9]*$"))
                return new ValidationResult(false, "只能输入数字");
            var fieldInfo = idType.GetField("MaxValue");
            if(fieldInfo != null)
            {
                var value1 = System.Convert.ToUInt64(value);
                var value2 = System.Convert.ToUInt64(fieldInfo.GetValue(null));
                if(value1 > value2)
                    return new ValidationResult(false, $"超出索引最大值{value2}");
            }

            if(CheckExist)
            {
                var tempIdx = System.Convert.ChangeType(value, idType);
                if (CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplate(tempIdx, selectedItem.Type) != null)
                    return new ValidationResult(false, $"ID {tempIdx} 已存在");
            }

            return new ValidationResult(true, null);
        }

        internal object GetIDFromFileName(string strAbsFile)
        {
            CComboBoxItem selectedItem = (CComboBoxItem)comboBox_Templates.SelectedItem;
            if (selectedItem == null)
                return null;

            var idType = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplateIDType(selectedItem.Type);
            if (idType == null)
                return null;

            var fileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(strAbsFile);
            var idStr = fileName.Substring(0, fileName.IndexOf('.'));
            return System.Convert.ChangeType(idStr, idType);
        }

        // 是否是创建的数据模板
        bool IsCreating = false;
        private void Button_New_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CComboBoxItem selectedItem = (CComboBoxItem)comboBox_Templates.SelectedItem;
            if (selectedItem == null)
                return;

            var idType = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplateIDType(selectedItem.Type);
            if (idType == null)
                return;

            var win = new InputWindow.InputWindow();
            win.Title = "数据模板ID设置";
            win.Description = "输入ID(只能输入正整数)";
            win.Value = System.Activator.CreateInstance(idType);
            if (win.ShowDialog((value, cultureInfo) =>
            {
                return CheckID(value, selectedItem, idType);
            }) == false)
                return;

            var idx = System.Convert.ChangeType(win.Value, idType);
            DataTemplateInstance = CSUtility.Data.DataTemplateManagerAssist.Instance.CreateDataTemplate(idx, selectedItem.Type, SearchFolderText);
            IsCreating = true;
        }

        // strFile为绝对路径
        private void OpenFile(string strAbsFile)
        {
            try
            {
                CComboBoxItem selectedItem = (CComboBoxItem)comboBox_Templates.SelectedItem;
                if (selectedItem == null)
                    return;

                var idType = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplateIDType(selectedItem.Type);
                if (idType == null)
                    return;

                var id = GetIDFromFileName(strAbsFile);
                DataTemplateInstance = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplate(id, selectedItem.Type);
                IsCreating = false;
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("TemplateEditor OpenFile Exception: \r\n" + e.ToString());
            }
        }

        private void Button_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CComboBoxItem item = comboBox_Templates.SelectedItem as CComboBoxItem;
            if (item == null)
                return;
            
            Save();
        }

        private void Button_SaveAs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mSelectedItem == null)
                return;

            CComboBoxItem item = comboBox_Templates.SelectedItem as CComboBoxItem;

            var idType = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplateIDType(item.Type);
            if (idType == null)
                return;

            var win = new InputWindow.InputWindow();
            win.Title = "数据模板ID设置";
            win.Description = "输入ID(只能输入正整数)";
            win.Value = System.Activator.CreateInstance(idType);
            if (win.ShowDialog((value, cultureInfo) =>
            {
                return CheckID(value, item, idType, false);
            }) == false)
                return;

            bool replace = false;
            var tagId = System.Convert.ChangeType(win.Value, idType);
            if (CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplate(tagId, item.Type) != null)
            {
                if (EditorCommon.MessageBox.Show($"ID{tagId}已存在，是否覆盖?", "警告", EditorCommon.MessageBox.enMessageBoxButton.YesNo) == EditorCommon.MessageBox.enMessageBoxResult.No)
                    return;

                replace = true;
            }

            var relTagFileName = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplateRelativeFileName(tagId, item.Type);
            var absTagFileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(relTagFileName);

            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"模板编辑器: 模板{DataTemplateInstance.GetType().FullName} {absTagFileName}更新失败!");
                    else
                    {
                        var data = DataTemplateInstance;
                        CSUtility.Data.DataTemplateManagerAssist.Instance.SaveAsDataTemplate(ref data, tagId);
                        DataTemplateInstance = data;
                        if(!replace)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                var node = new ViewNodeItem(DataTemplateInstance, this);
                                node.AbsFileName = absTagFileName;
                                mAllTreeViewItems.Add(node);
                            });
                            UpdateFilterShow();
                        }
                        IsCreating = false;

                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult commitResult) =>
                        {
                            if (commitResult.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"模板编辑器: 模板{DataTemplateInstance.GetType().FullName} {absTagFileName}上传失败!");
                            }
                            else
                            {
                                if (replace)
                                    UpdateServer(mSelectedItem, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType.Modify);
                                else
                                    UpdateServer(mSelectedItem, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType.Add);

                            }
                        }, absTagFileName, $"AutoCommit 修改模板{DataTemplateInstance.GetType().FullName} {relTagFileName}");

                    }
                }, absTagFileName);
            }
            else
            {
                var data = DataTemplateInstance;
                CSUtility.Data.DataTemplateManagerAssist.Instance.SaveAsDataTemplate(ref data, tagId);
                DataTemplateInstance = data;

                if(replace)
                    UpdateServer(mSelectedItem, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType.Modify);
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var node = new ViewNodeItem(DataTemplateInstance, this);
                        node.AbsFileName = absTagFileName;
                        mAllTreeViewItems.Add(node);
                    });
                    UpdateFilterShow();

                    UpdateServer(mSelectedItem, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType.Add);
                }

                IsCreating = false;
            }

        }

        private void Save()
        {
            if (DataTemplateInstance == null)
                return;
            
            var relFileName = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplateRelativeFileName(DataTemplateInstance);
            if (string.IsNullOrEmpty(relFileName))
                return;

            var nodeItem = mSelectedItem;
            var dataTemplate = DataTemplateInstance;
            var absFileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(relFileName);

            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"模板编辑器: 模板{dataTemplate.GetType().FullName} {absFileName}更新失败!");
                    }
                    else
                    {
                        if (!CSUtility.Data.DataTemplateManagerAssist.Instance.SaveDataTemplate(dataTemplate))
                            return;

                        if (IsCreating)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                nodeItem = new ViewNodeItem(dataTemplate, this);
                                nodeItem.AbsFileName = absFileName;
                                mAllTreeViewItems.Add(nodeItem);
                            });
                            UpdateFilterShow();
                        }
                        IsCreating = false;
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult commitResult) =>
                        {
                            if (commitResult.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"模板编辑器: 模板{dataTemplate.GetType().FullName} {absFileName}上传失败!");
                            }
                            else
                            {
                                UpdateServer(nodeItem, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType.Modify);
                            }
                        }, absFileName, $"AutoCommit 修改模板{dataTemplate.GetType().FullName} {relFileName}");
                    }
                }, absFileName);
            }
            else
            {
                CSUtility.Data.DataTemplateManagerAssist.Instance.SaveDataTemplate(dataTemplate);
                if (IsCreating)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        nodeItem = new ViewNodeItem(dataTemplate, this);
                        nodeItem.AbsFileName = absFileName;
                        mAllTreeViewItems.Add(nodeItem);
                    });
                    UpdateFilterShow();
                }
                UpdateServer(nodeItem, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType.Modify);
                IsCreating = false;
            }
        }

        private void Button_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataTemplateInstance == null)
                return;

            if (mSelectedItem == null)
            {
                EditorCommon.MessageBox.Show("请先选择一个模板后再删除!");
                return;
            }

            var selectedItem = mSelectedItem;
            var relFileName = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplateRelativeFileName(selectedItem);
            var absFile = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(relFileName);
            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"模板编辑器: 模板{selectedItem.GetType().FullName} {absFile}更新失败!");
                    else
                    {
                        if (!CSUtility.Data.DataTemplateManagerAssist.Instance.RemoveDataTemplate(selectedItem, false))
                            return;

                        mAllTreeViewItems.Remove(mSelectedItem);
                        UpdateFilterShow();

                        EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult delResult) =>
                        {
                            if (delResult.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"模板编辑器: 模板{selectedItem.GetType().FullName} {absFile}删除失败!");
                            else
                            {
                                UpdateServer(selectedItem, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType.Delete);
                                selectedItem = null;
                            }
                        }, absFile, $"AutoCommit 删除模板{selectedItem.GetType().FullName} {relFileName}");
                    }

                }, absFile);
            }
            else
            {
                CSUtility.Data.DataTemplateManagerAssist.Instance.RemoveDataTemplate(selectedItem, true);
                UpdateServer(selectedItem, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType.Delete);
                selectedItem = null;

                mAllTreeViewItems.Remove(mSelectedItem);
                UpdateFilterShow();
            }
        }

        private void UpdateServer(ViewNodeItem item, CSUtility.Data.DataTemplateManagerAssist.enDataTemplateOperationType opType)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (item == null)
                    return;

                // 通过GM指令来实现重读模板
                CCore.Support.GMCommandManager.Instance.GMExecute("ReloadTemplate", new object[] { item.DataTemplate.GetType().FullName, item.ID, opType });
            });
        }

        private void comboBox_Templates_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = comboBox_Templates.SelectedItem as CComboBoxItem;
            if (item == null)
                return;

            ShowTreeViewItems.Clear();
            mAllTreeViewItems.Clear();

            SearchFolderText = CSUtility.Data.DataTemplateManagerAssist.Instance.GetRelativeRootPath(item.Type);
            SearchFolder(SearchFolderText, item);
            //treeView_Files.ExpandAll();
        }

        //private TreeViewNodeItem GetTreeNode(string name, System.Windows.Controls.TreeView tree)
        //{
        //    foreach (TreeViewNodeItem node in tree.Items)
        //    {
        //        var retNode = GetTreeNode(name, node);
        //        if (retNode != null)
        //            return retNode;
        //    }

        //    return null;
        //}

        //private TreeViewNodeItem GetTreeNode(string name, TreeViewNodeItem parNode)
        //{
        //    if (parNode.DisplayName == name)
        //        return parNode;

        //    foreach (var node in parNode.Children)
        //    {
        //        var retNode = GetTreeNode(name, node);
        //        if (retNode != null)
        //            return retNode;
        //    }

        //    return null;
        //}

        private static TreeViewItem FindTreeViewItem(object obj)
        {
            DependencyObject dpObj = obj as DependencyObject;
            if (dpObj == null)
                return null;
            if (dpObj is TreeViewItem)
                return (TreeViewItem)dpObj;
            return FindTreeViewItem(VisualTreeHelper.GetParent(dpObj));
        }

        internal System.Collections.ObjectModel.ObservableCollection<ViewNodeItem> ErrorNodeItems = new System.Collections.ObjectModel.ObservableCollection<ViewNodeItem>();

        private void SearchFolder(string relativeFolder, CComboBoxItem item)
        {
            if (string.IsNullOrEmpty(relativeFolder))
                return;

            var idType = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplateIDType(item.Type);
            if (idType == null)
                return;

            var absFolder = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(relativeFolder);
            if(!System.IO.Directory.Exists(absFolder))
            {
                System.IO.Directory.CreateDirectory(absFolder);
                if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                {
                    EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                    {
                        if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"模板编辑器: 版本控制增加目录失败{absFolder}");
                        }
                        else
                        {
                            EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                            {
                                if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                {
                                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"模板编辑器: 版本控制增加目录失败{absFolder}");
                                    return;
                                }

                            }, absFolder, $"AutoCommit 增加模板目录{relativeFolder}");
                        }
                    }, absFolder);
                }
            }

            ErrorNodeItems.Clear();
            foreach (var file in System.IO.Directory.GetFiles(absFolder, "*" + item.Att.FileExtension, System.IO.SearchOption.AllDirectories))
            {
                var id = GetIDFromFileName(file);

                var dataTemplate = CSUtility.Data.DataTemplateManagerAssist.Instance.GetDataTemplate(id, item.Type);
                if (dataTemplate == null)
                    continue;

                this.Dispatcher.Invoke(() =>
                {
                    var node = new ViewNodeItem(dataTemplate, this);
                    if(!object.Equals(id, node.ID))
                    {
                        // 添加警告，id与文件名不一致
                        node.ErrorMessage = "ID与文件名不一致";
                        ErrorNodeItems.Add(node);
                    }
                    node.AbsFileName = file;
                    //treeNode.IsExpanded = true;
                    mAllTreeViewItems.Add(node);
                });

                // TreeView设置节点
                /*var relStr = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(relStr);
                if (xmlHolder == null)
                    continue;

                if (xmlHolder.RootNode != null && xmlHolder.RootNode.Name == item.Type.FullName)
                {
                    // 加入TreeView
                    var tempFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file, CSUtility.Support.IFileManager.Instance.Root + SearchFolderText);
                    tempFile.Replace("\\", "/");
                    var splits = tempFile.Split('/');
                    TreeViewNodeItem parentNode = null;
                    TreeViewNodeItem treeNode = null;
                    int i = 0;
                    foreach (var split in splits)
                    {
                        if (string.IsNullOrEmpty(split))
                            continue;

                        if (parentNode == null)
                            treeNode = GetTreeNode(split, treeView_Files);
                        else
                            treeNode = GetTreeNode(split, parentNode);

                        if (treeNode == null)
                        {
                            if (parentNode == null)
                            {
                                treeNode = new TreeViewNodeItem();
                                treeNode.DisplayName = split;
                                if (i == (splits.Length - 1))
                                    treeNode.Icon = new BitmapImage(new Uri("pack://application:,,,/ResourceLibrary;component/Icon/File/new.png"));
                                else
                                    treeNode.Icon = new BitmapImage(new Uri("pack://application:,,,/ResourceLibrary;component/Icon/File/folder_closed.png"));

                                treeNode.AbsFileName = file;
                                //treeNode.IsExpanded = true;
                                mAllTreeViewItems.Add(treeNode);
                            }
                            else
                            {
                                treeNode = new TreeViewNodeItem();
                                treeNode.DisplayName = split;
                                treeNode.AbsFileName = file;
                                //treeNode.IsExpanded = true;
                                parentNode.Children.Add(treeNode);
                            }
                        }

                        parentNode = treeNode;

                        i++;
                    }
                }*/
            }

            UpdateFilterShow();
            UpdateErrorInfo();
        }

        internal void UpdateErrorInfo()
        {
            if (ErrorNodeItems.Count > 0)
            {
                ItemView_ErrorFiles.Visibility = Visibility.Visible;
            }
            else
            {
                ItemView_ErrorFiles.Visibility = Visibility.Collapsed;
            }
        }

        bool mSelfSelectionProcess = false;
        private void treeView_Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mSelfSelectionProcess == true)
                return;

            var itemsControl = sender as System.Windows.Controls.ListView;
            if (itemsControl == null)
                return;

            mSelfSelectionProcess = true;
            if (itemsControl == ItemView_ErrorFiles)
                ItemView_Files.SelectedIndex = -1;
            else if (itemsControl == ItemView_Files)
                ItemView_ErrorFiles.SelectedIndex = -1;
            mSelfSelectionProcess = false;

            if (mSelectedItem != null && AutoSave)
            {
                //todo: dirty处理
                Save();
            }

            mSelectedItem = itemsControl.SelectedItem as ViewNodeItem;
            if (mSelectedItem != null)
                OpenFile(mSelectedItem.AbsFileName);
        }
        
        private void Button_OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", (CSUtility.Support.IFileManager.Instance.Root + SearchFolderText).Replace("/", "\\"));
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        System.Data.DataSet mDataset = null;
        TemplateEditor.ExportToExcel mExportToExcel = new TemplateEditor.ExportToExcel();

        string mExportFolder = "";
        public string ExportFolder
        {
            get { return mExportFolder; }
            set
            {
                mExportFolder = value;
                OnPropertyChanged("ExportFolder");
            }
        }

        string mFileType = "";
        public string FileType
        {
            get { return mFileType; }
            set
            {
                mFileType = value;
                OnPropertyChanged("FileType");
            }
        }

        string mExportState = "";
        public string ExportState
        {
            get { return mExportState; }
            set
            {
                mExportState = value;
                OnPropertyChanged("ExportState");
            }
        }

        public void LoadSuffixInfo(string fileName)
        {
            mExportToExcel.Clear();

            mDataset = null;
            var suffix = fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
            var folder = fileName.Remove(fileName.LastIndexOf('\\'));

            Type curType = null;
            mDataset = mExportToExcel.CreateDataSetFromFileBySuffix(this, suffix, folder, out curType);
            FileType = curType.ToString();
        }

        public void LoadSuffixInfo(string[] fileNames)
        {
            mExportToExcel.Clear();

            mDataset = null;
            var suffix = fileNames[0].Substring(fileNames[0].LastIndexOf('.'), fileNames[0].Length - fileNames[0].LastIndexOf('.'));
            Type curType = null;
            mDataset = mExportToExcel.CreateDataSetFromFileBySuffix(this, suffix, fileNames, out curType);
            FileType = curType.ToString();
        }

        string mImportFileName = "";
        public string ImportFileName
        {
            get { return mImportFileName; }
            set
            {
                mImportFileName = value;
                OnPropertyChanged("ImportFileName");
            }
        }

        string mImportFolder = "";
        public string ImportFolder
        {
            get { return mImportFolder; }
            set
            {
                mImportFolder = value;
                OnPropertyChanged("ImportFolder");
            }
        }

        System.DateTime mProcessStartTime;
        private string GetProcessTimeStr()
        {
            var time = System.DateTime.Now - mProcessStartTime;
            var timeStr = ((time.Days != 0) ? (time.Days + "天") : "") +
                          ((time.Hours != 0) ? (time.Hours + "小时") : "") +
                          ((time.Minutes != 0) ? (time.Minutes + "分钟") : "") +
                          ((time.Seconds != 0) ? (time.Seconds + "秒") : "");

            return timeStr;
        }

        string[] mOpendFileNames;
        string mOpendFile;
        private void OnClickOpenSelecetFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofD = new OpenFileDialog();

            var curPath = System.IO.Directory.GetCurrentDirectory();
            //var subPath = curPath.Remove(curPath.IndexOf("DemaciaBin"));
            ofD.Multiselect = true;

            //             if (string.IsNullOrEmpty(mConfig.OpenTemplatePath))
            //                 ofD.InitialDirectory = subPath + "DemaciaGame\\Template\\";
            //             else
            ofD.InitialDirectory = mConfig.OpenTemplatePath;
            ofD.Filter = "All Files(*.*)|*.*";

            foreach (var itm in comboBox_Templates.Items)
            {
                if (!(itm is CComboBoxItem))
                    continue;
                CComboBoxItem item = itm as CComboBoxItem;
                ofD.Filter += "|" + item.Type.ToString() + " Files(*" + item.Att.FileExtension + ")|*" + item.Att.FileExtension;
            }

            if (ofD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mOpendFileNames = ofD.FileNames;

                ButtonEnable = false;
                mProcessStartTime = System.DateTime.Now;

                var thread = new System.Threading.Thread(new System.Threading.ThreadStart(OpenSelectedFiles));
                thread.Name = "读取模板文件";
                thread.IsBackground = true;
                thread.Start();
            }
        }
        private void OpenSelectedFiles()
        {
            LoadSuffixInfo(mOpendFileNames);
            ExportState = "读取文件完成";

            EditorCommon.MessageBox.Show("读取文件完成！ 耗时:" + GetProcessTimeStr(), "模板编辑器");
            ButtonEnable = true;
        }

        private void OnClickOpenAllFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofD = new OpenFileDialog();

            var curPath = System.IO.Directory.GetCurrentDirectory();
            //             var subPath = curPath.Remove(curPath.IndexOf("DemaciaBin"));

            //             if (string.IsNullOrEmpty(mConfig.OpenTemplatePath))
            //                 ofD.InitialDirectory = subPath + "DemaciaGame\\Template\\";
            //             else
            ofD.InitialDirectory = mConfig.OpenTemplatePath;
            ofD.Filter = "All Files(*.*)|*.*";

            foreach (var itm in comboBox_Templates.Items)
            {
                if (!(itm is CComboBoxItem))
                    continue;
                CComboBoxItem item = itm as CComboBoxItem;
                ofD.Filter += "|" + item.Type.ToString() + " Files(*" + item.Att.FileExtension + ")|*" + item.Att.FileExtension;
            }

            if (ofD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ButtonEnable = false;

                mOpendFile = ofD.FileName;

                var thread = new System.Threading.Thread(new System.Threading.ThreadStart(OpenSelectedFile));
                thread.Name = "读取模板文件";
                thread.IsBackground = true;
                mProcessStartTime = System.DateTime.Now;
                thread.Start();
            }
            else
                return;
        }
        private void OpenSelectedFile()
        {
            LoadSuffixInfo(mOpendFile);
            ExportState = "读取文件完成";
            EditorCommon.MessageBox.Show("读取文件完成！ 耗时:" + GetProcessTimeStr(), "模板编辑器");
            ButtonEnable = true;
        }

        private void OnClickExportExcel(object sender, RoutedEventArgs e)
        {
            if (null == mDataset)
            {
                FileType = "没有选定文件";
                return;
            }
            ButtonEnable = false;
            ExportState = "正在导出中...";

            var thread = new System.Threading.Thread(new System.Threading.ThreadStart(ExportExcel));
            thread.Name = "导出到Excel";
            thread.IsBackground = true;
            mProcessStartTime = System.DateTime.Now;
            thread.Start();
        }
        private void ExportExcel()
        {
            mExportToExcel.DatasetExportToExcel(this, mDataset, ExportFolder);
            ExportState = "导出结束！！！";

            EditorCommon.MessageBox.Show("导出到Excel完成！ 耗时:" + GetProcessTimeStr(), "模板编辑器");

            ButtonEnable = true;
        }

        bool bIsImportExcel = false;
        string mImportState = "";
        public string ImportState
        {
            get { return mImportState; }
            set
            {
                mImportState = value;
                OnPropertyChanged("ImportState");
            }
        }
        private void OnClickImportExcel(object sender, RoutedEventArgs e)
        {
            ButtonEnable = false;

            OpenFileDialog ofD = new OpenFileDialog();
            ofD.Filter = "*.xls|*.xlsx";
            ofD.InitialDirectory = mConfig.OpenExcelPath;

            if (ofD.ShowDialog() != DialogResult.OK)
            {
                ButtonEnable = true;
                return;
            }

            ImportFileName = ofD.FileName;

            ImportState = "正在导入excel中...";

            var thread = new System.Threading.Thread(new System.Threading.ThreadStart(CreateDataSetFromExcel));
            thread.Name = "读取Excel";
            thread.IsBackground = true;
            mProcessStartTime = System.DateTime.Now;
            thread.Start();
        }
        private void CreateDataSetFromExcel()
        {
            //System.Windows.Controls.UserControl hostControl
            if (null == mExportToExcel.CreateDataSetFromExcel(ImportFileName, this))
                bIsImportExcel = false;
            else
                bIsImportExcel = true;
            ImportState = "导入完成";

            EditorCommon.MessageBox.Show("Excel 读取完成！ 耗时:" + GetProcessTimeStr(), "模板编辑器");

            ButtonEnable = true;
        }

        private void OnClickExportToFile(object sender, RoutedEventArgs e)
        {
            if (!bIsImportExcel)
            {
                ImportState = "没有导入Excel";
                return;
            }
            ButtonEnable = false;
            ImportState = "正在导出到文件中...";

            var thread = new System.Threading.Thread(new System.Threading.ThreadStart(ExportToFile));
            thread.Name = "从Excel导出到文件";
            thread.IsBackground = true;
            mProcessStartTime = System.DateTime.Now;
            thread.Start();
        }
        private void ExportToFile()
        {
            mExportToExcel.DatasetExportToTemplateFile(ImportFolder, this);
            ImportState = "导出文件完成";

            EditorCommon.MessageBox.Show("导出文件完成！ 耗时:" + GetProcessTimeStr(), "模板编辑器");

            ButtonEnable = true;
        }

        private void ApplyConfig()
        {
            if (string.IsNullOrEmpty(mConfig.ExportExcelPath))
            {
                string strFile = AppDomain.CurrentDomain.BaseDirectory;
                var iIndex = strFile.LastIndexOf("bin");
                strFile = strFile.Remove(iIndex);
                strFile += "ZeusGame\\Template";
                ExportFolder = strFile;
            }
            else
                ExportFolder = mConfig.ExportExcelPath;

            if (string.IsNullOrEmpty(mConfig.ExportFilePath))
            {
                string strFile = AppDomain.CurrentDomain.BaseDirectory;
                var iIndex = strFile.LastIndexOf("bin");
                strFile = strFile.Remove(iIndex);
                strFile += "ZeusGame\\Template";
                ImportFolder = strFile;
            }
            else
                ImportFolder = mConfig.ExportFilePath;
        }

        private void Button_SelectedImportDir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ImportFolder = dlg.SelectedPath;
            }
        }

        private void Button_SelectedExportDir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ExportFolder = dlg.SelectedPath;
            }

        }

    }


}
