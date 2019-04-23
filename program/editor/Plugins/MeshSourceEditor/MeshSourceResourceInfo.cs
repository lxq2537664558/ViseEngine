using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MeshSourceEditor
{
    [ResourcesBrowser.ResourceInfoAttribute(ResourceInfoType = "MeshSource", ResourceExts = new string[] { ".vms" })]
    public class MeshSourceResourceInfo : ResourcesBrowser.ResourceInfo, ResourcesBrowser.IResourceInfoDragDrop, ResourcesBrowser.IResourceInfoEditor, ResourcesBrowser.IResourceInfoForceReload
    {
        [ResourcesBrowser.ResourceToolTipAttribute]
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
            var snapShotFile = AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt;
            if(System.IO.File.Exists(snapShotFile) && !forceCreate)
            {
                Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
            }
            else
            { 
                var actorInit = new CCore.World.MeshActorInit();
                var actor = new CCore.World.MeshActor();
                actor.Initialize(actorInit);
                actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));

                var visual = new CCore.Mesh.Mesh();
                var mshInit = new CCore.Mesh.MeshInit();
                CCore.Mesh.MeshInitPart mshInitPart = new CCore.Mesh.MeshInitPart();
                mshInitPart.MeshName = AbsResourceFileName;
                mshInit.MeshInitParts.Add(mshInitPart);
                mshInit.CanHitProxy = false;
                visual.Initialize(mshInit, null);
                if (forceCreate)
                    visual.PreUse(true, (UInt64)CCore.Engine.Instance.GetFrameMillisecond());

                actor.Visual = visual;

                //this.ParentBrowser.Dispatcher.Invoke(new Action(() =>
                //{
                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.World.AddActor(actor);

                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);

                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.World.RemoveActor(actor);
                //}));

                Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
            }

            return Snapshot;
        }

        protected override ResourcesBrowser.ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {
            var fileInfo = new System.IO.FileInfo(resourceFile);
            var retValue = new MeshSourceResourceInfo();
            retValue.Name = fileInfo.Name.Replace(fileInfo.Extension, "");
            retValue.ResourceType = "MeshSource";

            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Info, "添加模型资源文件" + fileInfo.Name);

            return retValue;
        }

        public void ForceReload()
        {
            CCore.Mesh.Mesh.ForceReloadMesh(RelativeResourceFileName);
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
                            if (resultSnapshotExt.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}缩略图 {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtDelete) =>
                                {
                                    if (resultSnapshotExtDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}缩略图 {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
                                    }
                                }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 删除{ResourceTypeName}{Name}缩略图");
                            }
                        }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultMeshSocketExtension) =>
                        {
                            if (resultMeshSocketExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}挂接组件 {AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultMeshSocketExtensionDelete) =>
                                {
                                    if (resultMeshSocketExtensionDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}挂接组件 {AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension}使用版本控制删除失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension, $"AutoCommit 删除{ResourceTypeName}{Name}挂接组件");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSimpleMeshExtension) =>
                        {
                            if (resultSimpleMeshExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}碰撞模型 {AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultSimpleMeshExtensionDelete) =>
                                {
                                    if (resultSimpleMeshExtensionDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}碰撞模型 {AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension}使用版本控制删除失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension, $"AutoCommit 删除{ResourceTypeName}{Name}碰撞模型");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultPhysicGeometryExtension) =>
                        {
                            if (resultPhysicGeometryExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}物理体 {AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultPhysicGeometryExtensionDelete) =>
                                {
                                    if (resultPhysicGeometryExtensionDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}物理体 {AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension}使用版本控制删除失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension, $"AutoCommit 删除{ResourceTypeName}{Name}物理体");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultPathMeshExtension) =>
                        {
                            if (resultPathMeshExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}寻路模型 {AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultPathMeshExtensionDelete) =>
                                {
                                    if (resultPathMeshExtensionDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name}寻路模型 {AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension}使用版本控制删除失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension, $"AutoCommit 删除{ResourceTypeName}{Name}寻路模型");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension);
                    }
                }, AbsResourceFileName);
            }
            else
            {
                System.IO.File.Delete(AbsResourceFileName);
                System.IO.File.Delete(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                System.IO.File.Delete(AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension);
                System.IO.File.Delete(AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension);
                System.IO.File.Delete(AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension);
                System.IO.File.Delete(AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension);

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
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtMove) =>
                                {
                                    if (resultSnapshotExtMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                                    }
                                }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt)}");
                            }
                        }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultMeshSocketExtension) =>
                        {
                            if (resultMeshSocketExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultMeshSocketExtensionMove) =>
                                {
                                    if (resultMeshSocketExtensionMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension)}");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSimpleMeshExtension) =>
                        {
                            if (resultSimpleMeshExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultSimpleMeshExtensionMove) =>
                                {
                                    if (resultSimpleMeshExtensionMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension)}");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultPhysicGeometryExtension) =>
                        {
                            if (resultPhysicGeometryExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultPhysicGeometryExtensionMove) =>
                                {
                                    if (resultPhysicGeometryExtensionMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension)}");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension);

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultPathMeshExtension) =>
                        {
                            if (resultPathMeshExtension.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Move((EditorCommon.VersionControl.VersionControlCommandResult resultPathMeshExtensionMove) =>
                                {
                                    if (resultPathMeshExtensionMove.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:{ResourceTypeName}{Name}移动到目录{absFolder}失败!");
                                    }
                                }, AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension, $"AutoCommit {ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension)}");
                            }
                        }, AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension);
                    }
                }, AbsResourceFileName);
            }
            else
            {
                try
                {
                    System.IO.File.Move(AbsResourceFileName, absFolder + ResourceFileName);
                    System.IO.File.Move(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                    if(System.IO.File.Exists(AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension))
                        System.IO.File.Move(AbsResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.MeshSocketExtension);
                    if (System.IO.File.Exists(AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension))
                        System.IO.File.Move(AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension);
                    if (System.IO.File.Exists(AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension))
                        System.IO.File.Move(AbsResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.PhysicGeometryExtension);
                    if (System.IO.File.Exists(AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension))
                        System.IO.File.Move(AbsResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension, absFolder + ResourceFileName + CSUtility.Support.IFileConfig.PathMeshExtension);
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
                catch (Exception e)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:资源{Name}移动到目录{absFolder}失败，{e.ToString()}!");
                }
            }

            return true;
        }

        public void OpenEditor()
        {
            ParentBrowser?.OpenEditor(new object[] { "MeshSourceEditor", this });
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
                            System.IO.File.Copy(file, tagFile,true);                            
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
                            System.IO.File.Copy(file, tagFile,true);                            
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
                            System.IO.File.Copy(file, tagFile,true);
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
                            System.IO.File.Copy(file, tagFile,true);
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
