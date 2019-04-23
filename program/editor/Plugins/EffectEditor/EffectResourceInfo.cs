using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EffectEditor
{
    [ResourcesBrowser.ResourceInfoAttribute(ResourceInfoType = "Effect", ResourceExts = new string[] { ".eft" })]
    public class EffectResourceInfo : ResourcesBrowser.ResourceInfo, ResourcesBrowser.IResourceInfoCreateEmpty, ResourcesBrowser.IResourceInfoEditor, ResourcesBrowser.IResourceInfoDragToGameWindow
    {
        [ResourcesBrowser.ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "特效"; }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_Particle.png", UriKind.Absolute));
        public override ImageSource ResourceIcon
        {
            get { return mResourceIcon; }
            set
            {
                mResourceIcon = value;
                OnPropertyChanged("ResourceIcon");
            }
        }

        Brush mResourceTypeBrush = Brushes.Orange;
        public override Brush ResourceTypeBrush
        {
            get { return mResourceTypeBrush; }
            set
            {
                mResourceTypeBrush = value;
                OnPropertyChanged("ResourceTypeBrush");
            }
        }

        CCore.Effect.EffectTemplate mEffectTemplate;
        public CCore.Effect.EffectTemplate EffectTemplate
        {
            get { return mEffectTemplate; }
            protected set
            {
                mEffectTemplate = value;
                                
                if (mEffectTemplate != null)
                {
                    BindingOperations.ClearBinding(this, IsDirtyProperty);
                    BindingOperations.SetBinding(this, IsDirtyProperty, new Binding("IsDirty") { Source = mEffectTemplate });

                    BindingOperations.ClearBinding(this, NickNameProperty);
                    BindingOperations.SetBinding(this, NickNameProperty, new Binding("NickName") { Source = mEffectTemplate });
                }
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
                EffectTemplate = CCore.Effect.EffectManager.Instance.LoadEffectTemplate(AbsResourceFileName);

                if (EffectTemplate == null)
                {
                    Snapshot = ResourcesBrowser.Program.LoadImage("");
                    return Snapshot;
                }
                
                var effectInit = new CCore.World.EffectActorInit();
                var effectActor = new CCore.World.EffectActor();
                effectActor.Initialize(effectInit);
                effectActor.SetPlacement(new CSUtility.Component.StandardPlacement(effectActor));
                                
                var visual = new CCore.Component.EffectVisual();                

                var visInit = new CCore.Component.EffectVisualInit()
                {
                    EffectTemplateID = EffectTemplate.Id,
                    CanHitProxy = false
                };
                visual.Initialize(visInit, null);
                //visual.PreUse(true, (UInt64)(CCore.Engine.Instance.GetFrameMillisecond()));
                effectActor.Visual = visual;

                //this.ParentBrowser.Dispatcher.Invoke(new Action(() =>
                //{
                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.World.AddActor(effectActor);

                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);
                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.World.RemoveActor(effectActor);
                //}));

                Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
            }         

            return Snapshot;
        }

        protected override ResourcesBrowser.ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {            
            return null;            
        }

        public override void SetSelectedObjectData()
        {
            EditorCommon.PluginAssist.PropertyGridAssist.SetSelectedObjectData(ResourceType, new object[] { this.Id });
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
                        EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelelte) =>
                        {
                            if (resultDelelte.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsResourceFileName}使用版本控制删除失败!");
                            }
                        }, AbsResourceFileName, $"AutoCommit 删除{ResourceTypeName}{Name}");

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExt) => 
                        {
                            if (resultSnapshotExt.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtDelelte) =>
                                {
                                    if (resultSnapshotExtDelelte.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
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

                var id = CSUtility.Program.GetIdFromFile(AbsResourceFileName);
                CCore.Effect.EffectManager.Instance.DeleteEffectTemplate(id);
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
                                }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit {ResourceTypeName}{Name}缩略图从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt)}移动到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt)}");
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

        public override void Load(string absFileName)
        {
            base.Load(absFileName);

            EffectTemplate = CCore.Effect.EffectManager.Instance.LoadEffectTemplate(AbsResourceFileName);
        }

        public override void Save()
        {
            base.Save();
            Program.SaveEffectTemplate(this);
        }

        public ResourcesBrowser.ResourceInfo CreateEmptyResource(string Absfolder, object createData)
        {
            if (!System.IO.Directory.Exists(Absfolder))
                return null;

            var resourceName = (string)((createData as InputWindow.InputWindow).Value);
            var id = Guid.NewGuid();
            var absFileName = Absfolder + "/" + id.ToString() + CSUtility.Support.IFileConfig.EffectExtension;
            CCore.Effect.EffectManager.Instance.SaveEffectTemplate(absFileName, id);
            var effectTemplate = CCore.Effect.EffectManager.Instance.FindEffectTemplate(id);
            effectTemplate.NickName = resourceName;            
            CCore.Effect.EffectManager.Instance.SaveEffectTemplate(id);

            var retInfo = new EffectResourceInfo();
            retInfo.Id = id;
            retInfo.Name = resourceName;
            retInfo.ResourceType = "Effect";
            retInfo.AbsInfoFileName = absFileName + ResourcesBrowser.Program.ResourceInfoExt;
            retInfo.Save();

            return retInfo;
        }

        public ValidationResult ResourceNameAvailable(string resourceName)
        {
            return new ValidationResult(true, null);
        }

        public string CreateMenuPath
        {            
            get { return "粒子特效"; }
        }

        public void OpenEditor()
        {
            ParentBrowser?.OpenEditor(new object[] { "EffectEditor", this });
        }

        #region DragToGameWindow

        CCore.World.Actor mGameWindowDragActor = null;
        public void OnDragEnterGameWindow(System.Windows.Forms.Form gameWindow, System.Windows.Forms.DragEventArgs e)
        {
            if (EffectTemplate == null)
                return;

            var eftInit = new CCore.Component.EffectVisualInit()
            {
                EffectTemplateID = EffectTemplate.Id
            };
            var eft = new CCore.Component.EffectVisual();
            eft.Initialize(eftInit, null);

            var actorInit = new CCore.World.EffectActorInit();
            var actor = new CCore.World.EffectActor();
            actor.Initialize(actorInit);
            actor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            actor.Visual = eft;
            actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
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

            var eftInit = new CCore.Component.EffectVisualInit()
            {
                EffectTemplateID = EffectTemplate.Id
            };
            var eft = new CCore.Component.EffectVisual();
            eft.Initialize(eftInit, null);

            var actor = new CCore.World.MeshActor();
            actor.ParticipationLineCheck = false;
            var actorInit = new CCore.World.MeshActorInit();
            actor.Initialize(actorInit);
            actor.ActorName = EffectTemplate.NickName + CCore.Program.GetActorIndex();
            actor.Visual = eft;
            actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
            actor.Placement.SetLocation(ref hitPos);
            actor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient);

            CCore.Engine.Instance.Client.MainWorld.AddActor(actor);
            eft.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(actor.Id));

            return actor;
        }

        #endregion

    }
}
