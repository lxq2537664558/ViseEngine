using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ResourcesBrowser;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;

namespace AIEditor
{
    [ResourcesBrowser.ResourceInfoAttribute(ResourceInfoType = "AI")]
    public class AIResourceInfo : ResourcesBrowser.ResourceInfo, ResourcesBrowser.IResourceInfoCreateEmpty, ResourcesBrowser.IResourceInfoEditor
    {
        [ResourcesBrowser.ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "AI"; }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_AI_mini.png", UriKind.Absolute));
        public override ImageSource ResourceIcon
        {
            get { return mResourceIcon; }
            set
            {
                mResourceIcon = value;
                OnPropertyChanged("ResourceIcon");
            }
        }

        Brush mResourceTypeBrush = Brushes.Green;
        public override Brush ResourceTypeBrush
        {
            get { return mResourceTypeBrush; }
            set
            {
                mResourceTypeBrush = value;
                OnPropertyChanged("ResourceTypeBrush");
            }
        }

        FSMTemplateInfo mFSMTemplateInfo = null;
        public FSMTemplateInfo FSMTemplateInfo
        {
            get { return mFSMTemplateInfo; }
            set
            {
                mFSMTemplateInfo = value;
                if (value != null)
                {
                    BindingOperations.ClearBinding(this, IsDirtyProperty);
                    BindingOperations.SetBinding(this, IsDirtyProperty, new Binding("IsDirty") { Source = mFSMTemplateInfo });

                    BindingOperations.ClearBinding(this, NickNameProperty);
                    BindingOperations.SetBinding(this, NickNameProperty, new Binding("Name") { Source = mFSMTemplateInfo });
                }
            }
        }

        public override ImageSource GetSnapshotImage(bool forceCreate)
        {
            return mResourceIcon;
        }

        protected override ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {
            return null;
        }

        protected override void DeleteResourceOverride()
        {
            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsResourceFileName}使用版本控制删除失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                        {
                            if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsResourceFileName}使用版本控制删除失败!");
                            }
                        }, AbsResourceFileName, $"AutoCommit 删除{ResourceTypeName}{Name}");

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExt) =>
                        {
                            //if (resultSnapshotExt.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            //{
                            //    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsResourceFileName}使用版本控制删除失败!");
                            //}
                            //else
                            //{
                            //    EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtDelete) =>
                            //    {
                            //        if (resultSnapshotExtDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            //        {
                            //            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsResourceFileName}使用版本控制删除失败!");
                            //        }
                            //    }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 删除{ResourceTypeName}{Name}缩略图");
                            //}
                        }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                    }
                }, AbsResourceFileName);


            }
            else
            {
                System.IO.Directory.Delete(AbsResourceFileName, true);
            }
        }

        protected override bool MoveToFolderOverride(string absFolder)
        {
            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultMove) =>
                        {
                            if (resultMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                            }
                        }, AbsResourceFileName, absFolder + ResourceFileName, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName)}");
                    }
                }, AbsResourceFileName);
            }
            else
            {
                try
                {
                    System.IO.Directory.Move(AbsResourceFileName, absFolder + ResourceFileName);
                }
                catch (UnauthorizedAccessException)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:资源{Name}移动到目录{absFolder}失败，没有权限!");
                    return false;
                }
                catch (PathTooLongException)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:资源{Name}移动到目录{absFolder}失败，路径太长!");
                }
                catch (Exception)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:资源{Name}移动到目录{absFolder}失败，路径太长!");
                }
            }

            return true;
        }

        #region Create

        public override void SetSelectedObjectData()
        {
            EditorCommon.PluginAssist.PropertyGridAssist.SetSelectedObjectData(ResourceType, new object[] { this.Id });
        }

        public string CreateMenuPath
        {
            get { return "AI"; }
        }
        public ValidationResult ResourceNameAvailable(string resourceName)
        {
            return new ValidationResult(true, null);
        }

        public ResourcesBrowser.ResourceInfo CreateEmptyResource(string Absfolder, object createData)
        {
            if (!System.IO.Directory.Exists(Absfolder))
                return null;
            var resourceName = (string)((createData as InputWindow.InputWindow).Value);
            FSMTemplateInfo = FSMTemplateInfoManager.Instance.CreateFSMTemplate(Absfolder);
            FSMTemplateInfo.Name = resourceName;
            FSMTemplateInfoManager.Instance.SaveFSMTemplate(FSMTemplateInfo.Id, true);

            var retInfo = new AIResourceInfo();
            retInfo.Id = FSMTemplateInfo.Id;
            retInfo.Name = resourceName;
            retInfo.ResourceType = "AI";
            retInfo.AbsInfoFileName = Absfolder + "/" + FSMTemplateInfo.Id.ToString() + ResourcesBrowser.Program.ResourceInfoExt;
            retInfo.Save();

            return retInfo;
        }

        public override void Load(string absFileName)
        {
            base.Load(absFileName);
            
            var id = CSUtility.Program.GetIdFromFile(absFileName);
            FSMTemplateInfo = FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(id, false);
        }

        #endregion

        public void OpenEditor()
        {
            ParentBrowser?.OpenEditor(new object[] { "AIEditor", this });
        }

    }
}
