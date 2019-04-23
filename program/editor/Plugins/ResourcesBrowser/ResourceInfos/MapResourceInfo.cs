using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ResourcesBrowser.ResourceInfos
{
    [ResourcesBrowser.ResourceInfoAttribute(ResourceInfoType = "Map")]
    public class MapResourceInfo : ResourcesBrowser.ResourceInfo, ResourcesBrowser.IResourceInfoCreateEmpty, ResourcesBrowser.IResourceInfoEditor, ResourcesBrowser.IResourceInfoCustomCreateDialog
    {
        [ResourcesBrowser.ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "地图"; }
        }
        
        [ResourceToolTipAttribute]
        [DisplayName("ID")]
        public Guid MapId
        {
            get { return this.Id; }
            set
            {
                this.Id = value;
                OnPropertyChanged("MapId");
            }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_map.png", UriKind.Absolute));
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

        byte mWorldInitType = 0;
        [CSUtility.Support.DataValueAttribute("WorldInitType")]
        public byte WorldInitType
        {
            get { return mWorldInitType; }
            set
            {
                mWorldInitType = value;
                OnPropertyChanged("WorldInitType");
            }
        }
        
        public override ImageSource GetSnapshotImage(bool forceCreate)
        {
            var snapShotFile = AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt;
            if (System.IO.File.Exists(snapShotFile))
            {
                Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
            }
            else
                Snapshot = mResourceIcon;

            return Snapshot;
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
                            else
                            {
                                CSUtility.Map.MapManager.Instance.RemoveMapFile(this.Id);
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
                    }
                }, AbsResourceFileName);
            }
            else
            {
                System.IO.Directory.Delete(AbsResourceFileName, true);
                System.IO.File.Delete(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);

                CSUtility.Map.MapManager.Instance.RemoveMapFile(this.Id);
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
                                CSUtility.Map.MapManager.Instance.SetMapFile(this.Id, absFolder + ResourceFileName);
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
                    System.IO.Directory.Move(AbsResourceFileName, absFolder + ResourceFileName);
                    if(System.IO.File.Exists(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt))
                        System.IO.File.Move(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, absFolder + ResourceFileName + ResourcesBrowser.Program.SnapshotExt);

                    CSUtility.Map.MapManager.Instance.SetMapFile(this.Id, absFolder + ResourceFileName);
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

        public string CreateMenuPath
        {
            get { return "地图"; }
        }
        public ValidationResult ResourceNameAvailable(string resourceName)
        {
            return new ValidationResult(true, null);
        }

        public ResourcesBrowser.ResourceInfo CreateEmptyResource(string Absfolder, object createData)
        {
            if (!System.IO.Directory.Exists(Absfolder))
                return null;

            var win = createData as SceneCreateWindow;
            if (win == null)
                return null;
            
            // 根据创建窗口参数来创建场景
            var mapAbsDir = Absfolder + "/" + win.MapID.ToString();    // 地图路径
            if(!System.IO.Directory.Exists(mapAbsDir))
            {
                System.IO.Directory.CreateDirectory(mapAbsDir);
            }
            var mapRelDir = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mapAbsDir);

            var csInit = win.WorldInit;
            csInit.Save(mapAbsDir);

            var world = new CCore.World.World(csInit.WorldId);
            world.Initialize(csInit);

            if (csInit.ContainsTerrain)
            {
                // 包含地形
                world.Create(mapAbsDir, "地形");
                world.SaveWorld(mapAbsDir, true, "地形");
            }

            // 场景管理器
            world.Create(mapAbsDir, "场景");
            world.SaveWorld(mapAbsDir, true, "场景");
            world.SaveSpecialActors(mapAbsDir);

            world.Cleanup();

            var resourceName = csInit.WorldName;

            var retInfo = new MapResourceInfo();
            retInfo.Id = win.MapID;
            retInfo.Name = resourceName;
            retInfo.WorldInitType = win.WorldInitType;
            retInfo.ResourceType = "Map";
            retInfo.AbsInfoFileName = mapAbsDir + ResourcesBrowser.Program.ResourceInfoExt;
            retInfo.Save();

            CSUtility.Map.MapManager.Instance.SetMapFile(win.MapID, mapRelDir);
#warning SVN处理

            return retInfo;
        }

        public Window GetCustomCreateDialogWindow()
        {
            return new SceneCreateWindow();
        }

        #endregion

        public void OpenEditor()
        {
            if(CCore.Client.MainWorldInstance.IsDirty)
            {
                var result = EditorCommon.MessageBox.Show("当前场景还未保存，是否保存？", "警告", EditorCommon.MessageBox.enMessageBoxButton.YesNoCancel);
                switch(result)
                {
                    case EditorCommon.MessageBox.enMessageBoxResult.Yes:
                        // 保存当前场景
                        CCore.Client.MainWorldInstance.SaveWorld("", true);
                        break;
                    case EditorCommon.MessageBox.enMessageBoxResult.No:
                        break;
                    case EditorCommon.MessageBox.enMessageBoxResult.Cancel:
                        return;
                }
            }

            ParentBrowser?.OpenEditor(new object[] { "MapEditor", this });
        }
    }
}
