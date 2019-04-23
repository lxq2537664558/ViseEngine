using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ResourcesBrowser.ResourceInfos
{
    [ResourceInfoAttribute(ResourceInfoType = "MeshSource", ResourceExts = new string[] { ".vms" })]
    public class MeshSourceResourceInfo : ResourceInfo, IResourceInfoDragDrop
    {
        [ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "模型资源"; }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_MeshSource.png", UriKind.Absolute));
        public override ImageSource ResourceIcon
        {
            get { return mResourceIcon; }
            set
            {
                mResourceIcon = value;
                OnPropertyChanged("ResourceIcon");
            }
        }

        Brush mResourceTypeBrush = Brushes.LightBlue;
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
            var snapShotFile = AbsResourceFileName + Program.SnapshotExt;
            if(System.IO.File.Exists(snapShotFile) && !forceCreate)
            {
                Snapshot = Program.LoadImage(snapShotFile);
            }
            else
            {
                var actorInit = new CCore.World.MeshActorInit();
                var actor = new CCore.World.MeshActor();
                actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));

                var visual = new CCore.Mesh.Mesh();
                var mshInit = new CCore.Mesh.MeshInit();
                CCore.Mesh.MeshInitPart mshInitPart = new CCore.Mesh.MeshInitPart();
                mshInitPart.MeshName = AbsResourceFileName;
                mshInit.MeshInitParts.Add(mshInitPart);
                mshInit.CanHitProxy = false;
                visual.Initialize(mshInit, null);

                this.ParentBrowser.Dispatcher.Invoke(new Action(() =>
                {
                    SnapshotProcess.SnapshotCreator.Instance.World.AddActor(actor);

                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    SnapshotProcess.SnapshotCreator.Instance.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);

                    SnapshotProcess.SnapshotCreator.Instance.World.RemoveActor(actor);
                }));

                Snapshot = Program.LoadImage(snapShotFile);
            }

            return Snapshot;
        }

        protected override ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {
            var fileInfo = new System.IO.FileInfo(resourceFile);
            var retValue = new MeshSourceResourceInfo();
            retValue.Name = fileInfo.Name.Replace(fileInfo.Extension, "");
            retValue.ResourceType = "MeshSource";

            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Info, "添加模型资源文件" + fileInfo.Name);

            return retValue;
        }

        protected override void DeleteResourceOverride()
        {
            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) => { }, AbsResourceFileName);
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) => { }, AbsResourceFileName + Program.SnapshotExt);

                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"文件{AbsResourceFileName}使用版本控制删除失败!");
                    }
                }, AbsResourceFileName, "AutoCommit");
                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"文件{AbsResourceFileName + Program.SnapshotExt}使用版本控制删除失败!");
                    }
                }, AbsResourceFileName + Program.SnapshotExt, "AutoCommit");
            }
            else
            {
                System.IO.File.Delete(AbsResourceFileName);
                System.IO.File.Delete(AbsResourceFileName + Program.SnapshotExt);

            }

        }

        protected override bool MoveToFolderOverride(string absFolder)
        {
            if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) => { }, AbsResourceFileName);
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) => { }, AbsResourceFileName + Program.SnapshotExt);

                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:资源{Name}移动到目录{absFolder}失败!");
                    }
                }, AbsResourceFileName, absFolder + ResourceFileName, "AutoCommit");
                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:资源{Name}移动到目录{absFolder}失败!");
                    }
                }, AbsResourceFileName + Program.SnapshotExt, absFolder + ResourceFileName + Program.SnapshotExt, "AutoCommit");

            }
            else
            {
                try
                {
                    System.IO.File.Move(AbsResourceFileName + Program.SnapshotExt, absFolder + ResourceFileName + Program.SnapshotExt);
                    System.IO.File.Move(AbsResourceFileName + Program.SnapshotExt, absFolder + ResourceFileName + Program.SnapshotExt);
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

        #region dragdrop
        
        public bool DragEnter(System.Windows.DragEventArgs e)
        {
            var datas = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (datas == null || datas.Length == 0)
                return false;

            bool retValue = false;
            foreach(var file in datas)
            {
                var fileInfo = new System.IO.FileInfo(file);
                if(fileInfo.Extension.Equals(CSUtility.Support.IFileConfig.MeshSocketExtension))
                {
                    // Socket
                    retValue = true;
                }
                if(fileInfo.Extension.Equals(CSUtility.Support.IFileConfig.SimpleMeshExtension))
                {
                    // Collection
                    retValue = true;
                }
                if(fileInfo.Extension.Equals(CSUtility.Support.IFileConfig.PhysicGeometryExtension))
                {
                    // Physics
                    retValue = true;
                }
                if(fileInfo.Extension.Equals(CSUtility.Support.IFileConfig.PathMeshExtension))
                {
                    // Navigation
                    retValue = true;
                }
            }

            if(retValue)
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }

            return retValue;
        }
        public void DragLeave(System.Windows.DragEventArgs e)
        {
        }
        public void DragOver(System.Windows.DragEventArgs e)
        {
        }
        public void Drop(System.Windows.DragEventArgs e)
        {
            var datas = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (datas == null || datas.Length == 0)
                return;

            var dir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(AbsResourceFileName);
            foreach (var file in datas)
            {
                var fileInfo = new System.IO.FileInfo(file);
                var tagFile = dir + fileInfo.Name;
                if (fileInfo.Extension.Equals(CSUtility.Support.IFileConfig.MeshSocketExtension))
                {
                    // Socket
                    if(System.IO.File.Exists(tagFile))
                    {
                        if(EditorCommon.MessageBox.Show($"文件{fileInfo.Name}已存在，是否覆盖？", EditorCommon.MessageBox.enMessageBoxButton.YesNo) == EditorCommon.MessageBox.enMessageBoxResult.Yes)
                        {
                            System.IO.File.Copy(file, tagFile);
                        }
                    }
                    else
                        System.IO.File.Copy(file, tagFile);
                }
                if (fileInfo.Extension.Equals(CSUtility.Support.IFileConfig.SimpleMeshExtension))
                {
                    // Collection
                    if(System.IO.File.Exists(tagFile))
                    {
                        if(EditorCommon.MessageBox.Show($"文件{fileInfo.Name}已存在，是否覆盖？", EditorCommon.MessageBox.enMessageBoxButton.YesNo) == EditorCommon.MessageBox.enMessageBoxResult.Yes)
                        {
                            System.IO.File.Copy(file, tagFile);
                        }
                    }
                    else
                        System.IO.File.Copy(file, tagFile);
                }
                if (fileInfo.Extension.Equals(CSUtility.Support.IFileConfig.PhysicGeometryExtension))
                {
                    // Physics
                    if(System.IO.File.Exists(tagFile))
                    {
                        if(EditorCommon.MessageBox.Show($"文件{fileInfo.Name}已存在，是否覆盖？", EditorCommon.MessageBox.enMessageBoxButton.YesNo) == EditorCommon.MessageBox.enMessageBoxResult.Yes)
                        {
                            System.IO.File.Copy(file, tagFile);
                        }
                    }
                    else
                        System.IO.File.Copy(file, tagFile);
                }
                if (fileInfo.Extension.Equals(CSUtility.Support.IFileConfig.PathMeshExtension))
                {
                    // Navigation
                    if(System.IO.File.Exists(tagFile))
                    {
                        if(EditorCommon.MessageBox.Show($"文件{fileInfo.Name}已存在，是否覆盖？", EditorCommon.MessageBox.enMessageBoxButton.YesNo) == EditorCommon.MessageBox.enMessageBoxResult.Yes)
                        {
                            System.IO.File.Copy(file, tagFile);
                        }
                    }
                    else
                        System.IO.File.Copy(file, tagFile);
                }
            }

        }

        #endregion
    }
}
