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

namespace MaterialEditor
{
    [ResourcesBrowser.ResourceInfoAttribute(ResourceInfoType = "Technique", ResourceExts = new string[] { ".tech" })]
    public class TechniqueResourceInfo : ResourcesBrowser.ResourceInfo, ResourcesBrowser.IResourceInfoCreateEmpty, ResourcesBrowser.IResourceInfoEditor
    {
        [ResourcesBrowser.ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "材质实例"; }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_technique.png", UriKind.Absolute));
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
        
        public string CreateMenuPath
        {
            get { return "材质实例"; }
        }

        MaterialTechniqueInfo mTechInfo;
        public MaterialTechniqueInfo TechInfo
        {
            get { return mTechInfo; }
            set
            {
                mTechInfo = value;

                if (mTechInfo != null)
                {
                    //this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(()=>
                    //{
                        BindingOperations.SetBinding(this, NickNameProperty, new Binding("Name") { Source = mTechInfo });
                    //}));
                }
            }
        }

        [CSUtility.Support.DataValueAttribute("HostMaterialID")]
        public Guid HostMaterialID
        {
            get;
            set;
        }

        public override ImageSource GetSnapshotImage(bool forceCreate)
        {
            var snapShotFile = AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt;
            if (System.IO.File.Exists(snapShotFile) && !forceCreate)
                Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
            else
            {
                var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(Id);
                if(mtl == null)
                {
                    Snapshot = ResourcesBrowser.Program.LoadImage("");
                    return Snapshot;
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
                    mshInitPart.MeshName = CSUtility.Support.IFileConfig.SphereMesh;
                    mshInit.MeshInitParts.Add(mshInitPart);
                    mshInit.CanHitProxy = false;
                    visual.Initialize(mshInit, null);
                    for (int i = 0; i < visual.GetMaxMaterial(0); ++i)
                    {
                        visual.SetMaterial(0, i, mtl);
                    }
                    actor.Visual = visual;

                    //this.Dispatcher.Invoke(new Action<CCore.World.Actor, string>((ikAct,  ikSnapShotFile) =>
                    //{
                        ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.World.AddActor(actor);

                        var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);

                        var delta = ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.CameraDelta;
                        ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.CameraDelta = 0.8;
                        ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);
                        ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.CameraDelta = delta;

                        ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.World.RemoveActor(actor);
                    //}), new object[] { actor, snapShotFile });

                    Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
                }
            }

            return Snapshot;
        }

        protected override ResourcesBrowser.ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {
            return null;
        }

        protected override void DeleteResourceOverride()
        {
            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
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
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{ResourceTypeName}{Name} {AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultSnapshotExtDelete) =>
                                {
                                    if (resultSnapshotExtDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
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
            }

            CCore.Engine.Instance.Client.Graphics.MaterialMgr.RemoveFildDictionary(Id);
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
                        }, AbsResourceFileName, absFolder + ResourceFileName, $"AutoCommit 移动{ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName)}到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName)}");

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
                                }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 移动{ResourceTypeName}{Name}从{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt)}到{CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt)}");
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
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:材质实例{Name}移动到目录{absFolder}失败，没有权限!");
                    return false;
                }
                catch (PathTooLongException)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:材质实例{Name}移动到目录{absFolder}失败，路径太长!");
                }
                catch (Exception e)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:材质实例{Name}移动到目录{absFolder}失败\r\n{e.ToString()}");
                }
            }

            // 先更新材质索引列表以防止冲突
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
                        // 更新FileDictionary中的值
                        var tagFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                        CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryFileValue(this.Id, tagFile);
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
                var tagFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFolder + ResourceFileName, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryFileValue(this.Id, tagFile);
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.SaveFileDictionary();
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
            var hostMaterialID = CSUtility.Support.IFileConfig.DefaultMaterialId;
            var id = Guid.NewGuid();
            var absFileName = Absfolder + "/" + id.ToString() + CSUtility.Support.IFileConfig.MaterialTechniqueExtension;
            var relFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFileName, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
            var techFileInfo = new MaterialTechniqueInfo(hostMaterialID);
            techFileInfo.Id = id;
            techFileInfo.Name = resourceName;
            techFileInfo.CopyFromMaterialDefaultTechnique(hostMaterialID);
            techFileInfo.Save(absFileName);

            var retInfo = new TechniqueResourceInfo();
            retInfo.Id = id;
            retInfo.HostMaterialID = hostMaterialID;
            retInfo.Name = resourceName;
            retInfo.ResourceType = "Technique";
            retInfo.AbsInfoFileName = absFileName + ResourcesBrowser.Program.ResourceInfoExt;
            retInfo.TechInfo = techFileInfo;
            retInfo.Save();

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
                        CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryFileValue(retInfo.Id, relFile);
                        CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryOwnerIdValue(retInfo.Id, hostMaterialID);
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
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryFileValue(retInfo.Id, relFile);
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.SetFileDictionaryOwnerIdValue(retInfo.Id, hostMaterialID);
                CCore.Engine.Instance.Client.Graphics.MaterialMgr.SaveFileDictionary();
            }

            return retInfo;
        }

        public void OpenEditor()
        {
            ParentBrowser?.OpenEditor(new object[] { "TechniqueEditor", this });
        }

        public override void Save()
        {
            base.Save();

            TechInfo.Save(AbsResourceFileName);

        }
        public override void Load(string absFileName)
        {
            base.Load(absFileName);

            TechInfo = new MaterialTechniqueInfo(HostMaterialID);
            TechInfo.Load(AbsResourceFileName);
        }
    }
}
