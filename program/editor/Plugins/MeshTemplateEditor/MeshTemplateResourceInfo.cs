using CSUtility.AISystem.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;

namespace MeshTemplateEditor
{
    [ResourcesBrowser.ResourceInfoAttribute(ResourceInfoType = "MeshTemplate", ResourceExts = new string[] { ".mesh" })]
    public class MeshTemplateResourceInfo : ResourcesBrowser.ResourceInfo, ResourcesBrowser.IResourceInfoCreateEmpty, ResourcesBrowser.IResourceInfoEditor, ResourcesBrowser.IResourceInfoDragToGameWindow
    {
        [ResourcesBrowser.ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "模型模板"; }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_MeshTemplate.png", UriKind.Absolute));
        public override ImageSource ResourceIcon
        {
            get { return mResourceIcon; }
            set
            {
                mResourceIcon = value;
                OnPropertyChanged("ResourceIcon");
            }
        }

        Brush mResourceTypeBrush = Brushes.Blue;
        public override Brush ResourceTypeBrush
        {
            get { return mResourceTypeBrush; }
            set
            {
                mResourceTypeBrush = value;
                OnPropertyChanged("ResourceTypeBrush");
            }
        }

        CCore.Mesh.MeshTemplate mMeshTemplate;
        public CCore.Mesh.MeshTemplate MeshTemplate
        {
            get { return mMeshTemplate; }
            protected set
            {
                mMeshTemplate = value;

                if (mMeshTemplate != null)
                {                    
                    BindingOperations.ClearBinding(this, IsDirtyProperty);
                    BindingOperations.SetBinding(this, IsDirtyProperty, new Binding("IsDirty") { Source = mMeshTemplate });

                    BindingOperations.ClearBinding(this, NickNameProperty);
                    BindingOperations.SetBinding(this, NickNameProperty, new Binding("NickName") { Source = mMeshTemplate });
                }
            }
        }

        public string CreateMenuPath
        {
            get { return "模型模板"; }
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
                MeshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.LoadMeshTemplate(AbsResourceFileName);
                if (MeshTemplate == null || MeshTemplate.MeshInitList.Count == 0)
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
                meshInit.MeshTemplateID = MeshTemplate.MeshID;
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
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"删除{ResourceTypeName}{Name} {AbsResourceFileName}使用版本控制删除失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                        {
                            if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"删除{ResourceTypeName}{Name} {AbsResourceFileName}使用版本控制删除失败!");
                            }
                            else
                            {
                                CCore.Mesh.MeshTemplateMgr.Instance.RemoveMeshFile(MeshTemplate.MeshID);
                            }
                        }, AbsResourceFileName, $"AutoCommit 删除{ResourceTypeName}{Name}");

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExt) =>
                        {
                            if (resultSnapshotExt.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"删除{ResourceTypeName}{Name} {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtDelete) =>
                                {
                                    if (resultSnapshotExtDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"删除{ResourceTypeName}{Name} {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
                                    }
                                }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 删除{ResourceTypeName}{Name}缩略图");
                            }
                        }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                    }
                }, AbsResourceFileName);

            }
            else
            {
                System.IO.File.Delete(AbsResourceFileName);
                System.IO.File.Delete(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                
                CCore.Mesh.MeshTemplateMgr.Instance.RemoveMeshFile(MeshTemplate.MeshID);

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
                            else
                            {
                                CCore.Mesh.MeshTemplateMgr.Instance.SetMeshFile(MeshTemplate.MeshID, absFolder + ResourceFileName);
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
                    }
                }, AbsResourceFileName);
            }
            else
            {
                try
                {
                    System.IO.File.Move(AbsResourceFileName, absFolder + ResourceFileName);
                    System.IO.File.Move(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt);

                    CCore.Mesh.MeshTemplateMgr.Instance.SetMeshFile(MeshTemplate.MeshID, absFolder + ResourceFileName);
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

        public ValidationResult ResourceNameAvailable(string resourceName)
        {
            return new ValidationResult(true, null);
        }

        public ResourcesBrowser.ResourceInfo CreateEmptyResource(string Absfolder, object createData)
        {
            if (!System.IO.Directory.Exists(Absfolder))
                return null;

            var resourceName = (string)((createData as InputWindow.InputWindow).Value);
            var id = Guid.NewGuid();
            var absFileName = Absfolder + "/" + id.ToString() + CSUtility.Support.IFileConfig.MeshTemplateExtension;
            CCore.Mesh.MeshTemplateMgr.Instance.SaveMeshTemplate(absFileName, id);
            var meshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(id);
            meshTemplate.NickName = resourceName;
            CCore.Mesh.MeshTemplateMgr.Instance.SaveMeshTemplate(id);

            var retInfo = new MeshTemplateResourceInfo();
            retInfo.Id = id;
            retInfo.Name = resourceName;
            retInfo.ResourceType = "MeshTemplate";
            retInfo.AbsInfoFileName = absFileName + ResourcesBrowser.Program.ResourceInfoExt;
            retInfo.Save();

            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {retInfo.AbsResourceFileName}使用版本控制添加失败!");
                    }
                }, retInfo.AbsResourceFileName, $"AutoCommit {ResourceTypeName}{Name}");
                EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {retInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制添加失败!");
                    }
                }, retInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit {ResourceTypeName}{Name}缩略图");
            }

            return retInfo;
        }

        public override void Load(string absFileName)
        {
            base.Load(absFileName);

            MeshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.LoadMeshTemplate(AbsResourceFileName);
        }

        public void OpenEditor()
        {
            ParentBrowser?.OpenEditor(new object[] { "MeshTemplateEditor", this });
        }

        #region DragToGameWindow

        CCore.World.Actor mGameWindowDragActor = null;
        public void OnDragEnterGameWindow(System.Windows.Forms.Form gameWindow, System.Windows.Forms.DragEventArgs e)
        {
            if (MeshTemplate == null)
                return;

            var mesh = new CCore.Mesh.Mesh();
            var mshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = MeshTemplate.MeshID
            };
            mesh.Initialize(mshInit, null);

            var actor = new CCore.World.MeshActor();
            actor.ParticipationLineCheck = false;
            var actorInit = new CCore.World.MeshActorInit();
            actor.Initialize(actorInit);
            actor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            actor.Visual = mesh;
            actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
            var scale = new SlimDX.Vector3(MeshTemplate.Scale);
            actor.Placement.SetScale(ref scale);

            CCore.Engine.Instance.Client.MainWorld.AddEditorActor(actor);
            mGameWindowDragActor = actor;
        }

        public void OnDragLeaveGameWindow(System.Windows.Forms.Form gameWindow, EventArgs e)
        {
            if (mGameWindowDragActor == null)
                return;

            CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(mGameWindowDragActor);
            mGameWindowDragActor.Cleanup();
            mGameWindowDragActor = null;
        }
        public void OnDragOverGameWindow(System.Windows.Forms.Form gameWindow, System.Windows.Forms.DragEventArgs e)
        {
            if (mGameWindowDragActor == null)
                return;

            var pos = EditorCommon.DragDrop.DragDropManager.Instance.GetMousePos();
            pos.X -= gameWindow.Left;
            pos.Y -= gameWindow.Top;
            var hitPos = EditorCommon.Assist.Assist.IntersectWithWorld((int)pos.X, (int)pos.Y, CCore.Engine.Instance.Client.MainWorld, EditorCommon.Program.GameREnviroment.Camera, (UInt32)CSUtility.enHitFlag.HitMeshTriangle);
            mGameWindowDragActor.Placement.SetLocation(ref hitPos);
        }
        public object OnDragDropGameWindow(System.Windows.Forms.Form gameWindow, System.Windows.Forms.DragEventArgs e)
        {
            if (mGameWindowDragActor == null)
                return null;

            CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(mGameWindowDragActor);
            mGameWindowDragActor.Cleanup();
            mGameWindowDragActor = null;

            var pos = EditorCommon.DragDrop.DragDropManager.Instance.GetMousePos();
            pos.X -= gameWindow.Left;
            pos.Y -= gameWindow.Top;
            var hitPos = EditorCommon.Assist.Assist.IntersectWithWorld((int)pos.X, (int)pos.Y, CCore.Engine.Instance.Client.MainWorld, EditorCommon.Program.GameREnviroment.Camera, (UInt32)CSUtility.enHitFlag.HitMeshTriangle);

            var mesh = new CCore.Mesh.Mesh();
            var mshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = MeshTemplate.MeshID
            };
            mesh.Initialize(mshInit, null);

            var actor = new CCore.World.MeshActor();
            actor.ParticipationLineCheck = false;
            var actorInit = new CCore.World.MeshActorInit();
            actor.Initialize(actorInit);
            actor.ActorName = MeshTemplate.NickName + CCore.Program.GetActorIndex();
            actor.Visual = mesh;
            actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
            actor.Placement.SetLocation(ref hitPos);
            var scale = new SlimDX.Vector3(MeshTemplate.Scale);
            actor.Placement.SetScale(ref scale);
            actor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient);

            CCore.Engine.Instance.Client.MainWorld.AddActor(actor);
            mesh.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(actor.Id));

            return actor;
        }

        #endregion

    }
}
