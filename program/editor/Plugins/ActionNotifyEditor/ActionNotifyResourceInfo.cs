using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ActionNotifyEditor
{
    [ResourcesBrowser.ResourceInfoAttribute(ResourceInfoType = "Action", ResourceExts = new string[] { ".vma" })]
    class ActionNotifyResourceInfo : ResourcesBrowser.ResourceInfo, ResourcesBrowser.IResourceInfoEditor
    {
        [ResourcesBrowser.ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "动作"; }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_Action.png", UriKind.Absolute));
        public override ImageSource ResourceIcon
        {
            get { return mResourceIcon; }
            set
            {
                mResourceIcon = value;
                OnPropertyChanged("ResourceIcon");
            }
        }

        Brush mResourceTypeBrush = Brushes.YellowGreen;
        public override Brush ResourceTypeBrush
        {
            get { return mResourceTypeBrush; }
            set
            {
                mResourceTypeBrush = value;
                OnPropertyChanged("ResourceTypeBrush");
            }
        }

        public override ImageSource GetSnapshotImage(bool forceCreate)
        {            
            var snapShotFile = AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt;
            if (System.IO.File.Exists(snapShotFile) && !forceCreate)
            {
                Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
            }
            else
            {
                var actionSource = CSUtility.Animation.ActionNodeManager.Instance.GetActionSource(AbsResourceFileName, false, CSUtility.Helper.enCSType.All);
                if (actionSource == null || actionSource.MeshId == Guid.Empty)
                {
                    Snapshot = ResourcesBrowser.Program.LoadImage("");
                    return Snapshot;
                }                                    

                var maInit = new CCore.World.MeshActorInit();
                var meshActor = new CCore.World.MeshActor();
                meshActor.Initialize(maInit);
                meshActor.SetPlacement(new CSUtility.Component.StandardPlacement(meshActor));

                var visual = new CCore.Mesh.Mesh();

                meshActor.Visual = visual;
                var meshInit = new CCore.Mesh.MeshInit();
                meshInit.MeshTemplateID = actionSource.MeshId;
                visual.Initialize(meshInit, null);

                //this.ParentBrowser.Dispatcher.Invoke(new Action(() =>
                //{
                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.World.AddActor(meshActor);

                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);
                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.World.RemoveActor(meshActor);
                //}));

                Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
            }

            return Snapshot;
        }

        protected override ResourcesBrowser.ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {
            var fileInfo = new System.IO.FileInfo(resourceFile);
            var retValue = new ActionNotifyResourceInfo();
            retValue.Name = fileInfo.Name.Replace(fileInfo.Extension, "");
            retValue.ResourceType = "Action";            

            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Info, "添加动作文件" + fileInfo.Name);

            return retValue;            
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

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshortExt) =>
                        {
                            if (resultSnapshortExt.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}缩略图 {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshortExtDelete) =>
                                {
                                    if (resultSnapshortExtDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}缩略图 {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
                                    }
                                }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 删除{ResourceTypeName}{Name}缩略图");
                            }
                        }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultActionNotifyExtension) =>
                        {
                            if (resultActionNotifyExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}事件 {AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultActionNotifyExtensionDelete) =>
                                {
                                    if (resultActionNotifyExtensionDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}事件 {AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension}使用版本控制删除失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension, $"AutoCommit 删除{ResourceTypeName}{Name}事件");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension);
                    }
                }, AbsResourceFileName);
            }
            else
            {
                System.IO.File.Delete(AbsResourceFileName);
                System.IO.File.Delete(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                System.IO.File.Delete(AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension);
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

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExt) =>
                        {
                            if (resultSnapshotExt.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}缩略图移动到目录{absFolder}失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtMove) =>
                                {
                                    if (resultSnapshotExtMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}缩略图移动到目录{absFolder}失败!");
                                    }
                                }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt)}");
                            }
                        }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultActionNotifyExtension) =>
                        {
                            if (resultActionNotifyExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}事件移动到目录{absFolder}失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultActionNotifyExtensionMove) =>
                                { 
                                    if (resultActionNotifyExtensionMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}事件移动到目录{absFolder}失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension)}");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension);
                    }
                }, AbsResourceFileName);
            }
            else
            {
                try
                {
                    System.IO.File.Move(AbsResourceFileName, absFolder + ResourceFileName);
                    System.IO.File.Move(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                    System.IO.File.Move(AbsResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension);
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

        public void OpenEditor()
        {
            ParentBrowser?.OpenEditor(new object[] { "ActionNotifyEditor", this });
        }
    }
}
