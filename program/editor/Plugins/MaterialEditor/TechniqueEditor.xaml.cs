using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MaterialEditor
{
    /// <summary>
    /// TechniqueEditor.xaml 的交互逻辑
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "TechniqueEditor")]
    [Guid("4B0BDA63-F3AF-4AA6-8A70-6948719CAEA7")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class TechniqueEditor : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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
            get { return "材质实例编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "材质实例编辑器",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            D3DViewer.Activated = true;
            return true;
        }
        public bool OnDeactive()
        {
            D3DViewer.Activated = false;
            return true;
        }

        public void Tick()
        {
            if (D3DViewer != null)
                D3DViewer.Tick();
        }

        public void SetObjectToEdit(object[] obj)
        {
            if (obj == null)
                return;
            if (obj.Length == 0)
                return;

            try
            {
                var oldResourceInfo = mCurrentResourceInfo;
                if (mCurrentResourceInfo != null)
                    mCurrentResourceInfo.TechInfo.OnDirtyChanged = null;

                mCurrentResourceInfo = obj[0] as TechniqueResourceInfo;
                if (mCurrentResourceInfo == null)
                    return;

                mTempTechniqueInfo = new MaterialTechniqueInfo(mCurrentResourceInfo.HostMaterialID);
                // 重新读取Technique以保证最新
                mCurrentResourceInfo.TechInfo.Load(mCurrentResourceInfo.AbsResourceFileName);
                mCurrentResourceInfo.TechInfo.OnDirtyChanged = TechInfo_OnDirtyChanged;
                UpdateMaterialShow();

                ProGrid.Instance = mCurrentResourceInfo.TechInfo;
                var file = CCore.Engine.Instance.Client.Graphics.MaterialMgr.FindMaterialFile(mCurrentResourceInfo.TechInfo.HostMaterialId);
                if(!string.IsNullOrEmpty(file))
                {
                    var absFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + file;
                    UpdateParentMaterial(absFile, false);
                }
            }
            catch(System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
            }
        }

        private void TechInfo_OnDirtyChanged(MaterialTechniqueInfo info)
        {
            mCurrentResourceInfo.IsDirty = info.IsDirty;
            UpdateMaterialShow();
        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        protected bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
                mCurrentResourceInfo.IsDirty = mIsDirty;
            }
        }

        TechniqueResourceInfo mCurrentResourceInfo;

        public TechniqueEditor()
        {
            InitializeComponent();

            MainGrid.Children.Remove(ToolBar_Main);
            D3DViewer.AddToolBar(ToolBar_Main);

            D3DViewer.ShowSphere = true;
            D3DViewer.OnShowMeshChanged += D3DViewer_OnShowMeshChanged;
        }

        private void D3DViewer_OnShowMeshChanged()
        {
            UpdateMaterialShow();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentResourceInfo == null)
                return;

            mCurrentResourceInfo.Save();
            var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(mCurrentResourceInfo.Id, true);
            CCore.Engine.Instance.Client.Graphics.MaterialMgr.RefreshEffect(mtl);
        }

        private MaterialTechniqueInfo mTempTechniqueInfo;
        protected void UpdateMaterialShow()
        {
            if (mCurrentResourceInfo == null || mCurrentResourceInfo.TechInfo == null)
                return;

            var matfile = CCore.Engine.Instance.Client.Graphics.MaterialMgr.FindMaterialFile(mCurrentResourceInfo.TechInfo.HostMaterialId);
            if (!string.IsNullOrEmpty(matfile))
            {
                mTempTechniqueInfo.CopyFrom(mCurrentResourceInfo.TechInfo);
                var techFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + Program.TempTechniqueFileName;
                mTempTechniqueInfo.Save(techFile);

                var material = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetMaterialInstance(matfile, Program.TempTechniqueFileName, true);
                D3DViewer.SetObjectToEdit(new object[] { new object[] { "Material", true },
                                                             new object[] { -1, -1, -1, material } });

                mCurrentResourceInfo.ParentBrowser.ReCreateSnapshot(mCurrentResourceInfo);
            }
        }

        #region 父材质

        string mParentMaterialName;
        public string ParentMaterialName
        {
            get { return mParentMaterialName; }
            set
            {
                mParentMaterialName = value;
                OnPropertyChanged("ParentMaterialName");
            }
        }

        MaterialResourceInfo mCurrentMaterialInfo;

        private void UpdateParentMaterial(string materialAbsFile, bool resetTechnique)
        {
            mCurrentMaterialInfo = new MaterialResourceInfo();
            // 材质不存在则使用默认材质
            if (string.IsNullOrEmpty(materialAbsFile) || !System.IO.File.Exists(materialAbsFile))
                materialAbsFile = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(CSUtility.Support.IFileConfig.DefaultMaterialId);
            mCurrentMaterialInfo.Load(materialAbsFile);
            ParentMaterialName = mCurrentMaterialInfo.Name;

            var snapShotFile = mCurrentMaterialInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt;
            Image_MatSnapshot.Source = ResourcesBrowser.Program.LoadImage(snapShotFile);

            // 复制父级材质的材质参数
            if(resetTechnique)
            {
                var matFileInfo = new MaterialFileInfo();
                matFileInfo.LoadMaterialFile(mCurrentMaterialInfo.AbsResourceFileName);
                mCurrentResourceInfo.TechInfo.CopyFrom(matFileInfo.DefaultTechnique);
                ProGrid.Instance = null;
                ProGrid.Instance = mCurrentResourceInfo.TechInfo;

                UpdateMaterialShow();
                IsDirty = true;

                // 先更新材质索引列表防止冲突
                if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                {
                    var idxFile = CSUtility.Support.IFileManager.Instance.Root + CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileName();
                    EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                    {
                        if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"材质编辑器：材质索引列表上传失败。{result.Result}");
                        }
                        else
                        {
                            CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryOwnerIdValue(mCurrentResourceInfo.Id, matFileInfo.MaterialId);
                            CCore.Engine.Instance.Client.Graphics.MaterialMgr.SaveFileDictionary();

                            // 保存索引文件后上传
                            EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult commitResult) =>
                            {
                                if (commitResult.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"材质编辑器：材质索引列表上传失败。{commitResult.Result}");
                            }, idxFile, "AutoCommit 修改材质索引列表");
                        }
                    }, idxFile);
                }
                else
                {
                    CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryOwnerIdValue(mCurrentResourceInfo.Id, matFileInfo.MaterialId);
                    CCore.Engine.Instance.Client.Graphics.MaterialMgr.SaveFileDictionary();
                }
            }
        }

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("Material");
            if (data == null)
                return;

            if (data.Length > 0)
            {
                var materialRelPath = (string)data[0];
                UpdateParentMaterial(CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(materialRelPath), true);
            }
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mCurrentMaterialInfo == null)
                return;
            
            EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", mCurrentMaterialInfo.AbsResourceFileName });
        }

        #region 拖放操作

        EditorCommon.DragDrop.DropAdorner mDropAdorner;
        enum enDropResult
        {
            Denial_UnknowFormat,
            Denial_NoDragAbleObject,
            Allow,
        }
        // 是否允许拖放
        enDropResult AllowResourceItemDrop(System.Windows.DragEventArgs e)
        {
            var formats = e.Data.GetFormats();
            if (formats == null || formats.Length == 0)
                return enDropResult.Denial_UnknowFormat;

            var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
            if (datas == null)
                return enDropResult.Denial_NoDragAbleObject;

            bool containMeshSource = false;
            foreach (var data in datas)
            {
                var resInfo = data as ResourcesBrowser.ResourceInfo;
                if (resInfo.ResourceType == "Material")
                {
                    containMeshSource = true;
                    break;
                }
            }

            if (!containMeshSource)
                return enDropResult.Denial_NoDragAbleObject;

            return enDropResult.Allow;
        }

        private void Rectangle_DragEnter(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;
            
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;

                if (mDropAdorner == null)
                    mDropAdorner = new EditorCommon.DragDrop.DropAdorner(Grid_ParentMaterial);
                mDropAdorner.IsAllowDrop = false;

                switch (AllowResourceItemDrop(e))
                {
                    case enDropResult.Allow:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "设置父材质模板";

                            mDropAdorner.IsAllowDrop = true;
                            var pos = e.GetPosition(element);
                            if (pos.X > 0 && pos.X < element.ActualWidth &&
                               pos.Y > 0 && pos.Y < element.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(element);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;

                    case enDropResult.Denial_NoDragAbleObject:
                    case enDropResult.Denial_UnknowFormat:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是材质模板资源";

                            mDropAdorner.IsAllowDrop = false;
                            var pos = e.GetPosition(element);
                            if (pos.X > 0 && pos.X < element.ActualWidth &&
                               pos.Y > 0 && pos.Y < element.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(element);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;
                }
            }
        }

        private void Rectangle_DragLeave(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer.Remove(mDropAdorner);
            }
        }

        private void Rectangle_DragOver(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                if (AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer.Remove(mDropAdorner);

                if (AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    var formats = e.Data.GetFormats();
                    var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                    foreach (var data in datas)
                    {
                        var resInfo = data as ResourcesBrowser.ResourceInfo;
                        if (resInfo == null)
                            continue;

                        UpdateParentMaterial(resInfo.AbsResourceFileName, true);
                    }
                }
            }
        }

        #endregion


        #endregion

    }
}
