using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UIEditor
{
    [ResourcesBrowser.ResourceInfoAttribute(ResourceInfoType = "UI", ResourceExts = new string[] { ".xml"})]
    public class UIResourceInfo : ResourcesBrowser.ResourceInfo, ResourcesBrowser.IResourceInfoEditor, ResourcesBrowser.IResourceInfoCreateEmpty, ResourcesBrowser.IResourceInfoValidName
    {
        [ResourcesBrowser.ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "界面"; }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_UIform.png", UriKind.Absolute));
        public override ImageSource ResourceIcon
        {
            get { return mResourceIcon; }
            set
            {
                mResourceIcon = value;
                OnPropertyChanged("ResourceIcon");
            }
        }

        Brush mResourceTypeBrush = Brushes.BurlyWood;
        public override Brush ResourceTypeBrush
        {
            get { return mResourceTypeBrush; }
            set
            {
                mResourceTypeBrush = value;
                OnPropertyChanged("ResourceTypeBrush");
            }
        }        

        public MainControl HostControl;
        public override ImageSource GetSnapshotImage(bool forceCreate)
        {            
            CCore.Support.ReflectionManager.Instance.SetUIFile(ResourceFileName, AbsResourceFileName);

            var snapShotFile = AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt;
            if (System.IO.File.Exists(snapShotFile) && !forceCreate)
            {
                Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
            }
            else
            {
                if (HostControl == null)
                {
                    Snapshot = ResourcesBrowser.Program.LoadImage("");
                    return Snapshot;
                }                    
                var parent = HostControl.UIDrawPanel.WinRootForm.Parent;

                ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.SetForm((UISystem.WinForm)HostControl.UIDrawPanel.WinRootForm);
                //this.Dispatcher.Invoke(new Action(() =>
                //{
                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                                        
                    ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);
                //}));

                Snapshot = EditorCommon.ImageInit.GetImage(snapShotFile);
                HostControl.UIDrawPanel.WinRootForm.Parent = parent;
                ResourcesBrowser.SnapshotProcess.SnapshotCreator.Instance.ReStoreForm();
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
                                CCore.Support.ReflectionManager.Instance.RemoveUIFile(ResourceFileName);
                            }
                        }, AbsResourceFileName, $"AutoCommit {ResourceTypeName}{Name}");

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
                                }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit {ResourceTypeName}{Name}缩略图");
                            }
                        }, AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                    }
                }, AbsResourceFileName);


            }
            else
            {
                System.IO.File.Delete(AbsResourceFileName);
                System.IO.File.Delete(AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt);
                CCore.Support.ReflectionManager.Instance.RemoveUIFile(ResourceFileName);
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
                                CCore.Support.ReflectionManager.Instance.SetUIFile(ResourceFileName, absFolder + ResourceFileName);
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
                    CCore.Support.ReflectionManager.Instance.SetUIFile(ResourceFileName, absFolder + ResourceFileName);
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
            ParentBrowser?.OpenEditor(new object[] { "UIEditor", this });
        }

        public ResourcesBrowser.ResourceInfo CreateEmptyResource(string Absfolder, object createData)
        {
            if (!System.IO.Directory.Exists(Absfolder))
                return null;

            var resourceName = (string)((createData as InputWindow.InputWindow).Value);
            var absFileName = Absfolder + "/" + resourceName + ".xml";            
            var file = absFileName.Replace(CSUtility.Support.IFileManager.Instance.Root, "");

            UISystem.WinBase.SaveToXml(file, new UISystem.WinForm());
            CCore.Support.ReflectionManager.Instance.SetUIFile(resourceName, file);

            var retInfo = new UIResourceInfo();           
            retInfo.Name = resourceName;
            retInfo.ResourceType = "UI";
            retInfo.AbsInfoFileName = absFileName + ResourcesBrowser.Program.ResourceInfoExt;
            retInfo.Save();

            return retInfo;
        }

        public ValidationResult ResourceNameAvailable(string resourceName)
        {
            var name = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(resourceName);

            if (string.IsNullOrEmpty(name))
                return new ValidationResult(false, "名称不能为空");

            if (Regex.IsMatch(name, @"[\u4e00-\u9fa5]"))
                return new ValidationResult(false, "为保证系统兼容性，文件夹名称中不能包含中文");

            foreach (var invalidChar in System.IO.Path.GetInvalidFileNameChars())
            {
                if (name.Contains(invalidChar))
                    return new ValidationResult(false, "名称中包含不合法的字符: " + invalidChar);
            }
            
            if (System.IO.Directory.GetFiles(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory, name + ".xml", SearchOption.AllDirectories).Length >= 1)
            {
                return new ValidationResult(false, "文件 " + name + ".xml" + "已存在");
            }
            return new ValidationResult(true, null);
        }

        public string CreateMenuPath
        {
            get { return "UI"; }
        }

        public string GetValidName()
        {
            string name = "NewUI";
            int i = 1;
            while (true)
            {
                string file =name + i.ToString() + ".xml";
                
                var files = System.IO.Directory.GetFiles(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory, file, SearchOption.AllDirectories);
                if (files.Length >= 1)
                {
                    ++i;
                }
                else
                {
                    return name + i.ToString();
                }
            }
        }
    }
}
