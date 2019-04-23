using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ResourcesBrowser.ResourceInfos
{
    [ResourceInfoAttribute(ResourceInfoType = "Texture", ResourceExts = new string[] { ".png", ".jpg", ".bmp", ".dds" })]
    public class TextureResourceInfo : ResourceInfo, IResourceInfoForceReload
    {
        [ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "贴图"; }
        }

        string mFileExtension;
        public override string FileExtension
        {
            get { return mFileExtension; }
            set
            {
                mFileExtension = value;

                switch(mFileExtension)
                {
                    case "png":
                        ResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_png.png", UriKind.Absolute));
                        break;
                    case "jpg":
                    case "jpeg":
                        ResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_jpg.png", UriKind.Absolute));
                        break;
                    case "bmp":
                        ResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_bmp.png", UriKind.Absolute));
                        break;
                    case "dds":
                        ResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_dds.png", UriKind.Absolute));
                        break;
                }

                OnPropertyChanged("FileExtension");
            }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_default.png", UriKind.Absolute));
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

        string mDimensions = "0X0";
        [ResourceToolTipAttribute]
        [DisplayName("像素尺寸")]
        public string Dimensions
        {
            get { return mDimensions; }
            set
            {
                mDimensions = value;
                OnPropertyChanged("Dimensions");
            }
        }

        PixelFormat mPixelFormat;
        [ResourceToolTipAttribute]
        [DisplayName("像素格式")]
        public PixelFormat PixelFormat
        {
            get { return mPixelFormat; }
            set
            {
                mPixelFormat = value;
                OnPropertyChanged("PixelFormat");
            }
        }

        public override ImageSource GetSnapshotImage(bool forceCreate)
        {
            var fileName = AbsInfoFileName.Replace(ResourceInfo.ExtString, "");
            fileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(fileName);
            var image = ResourcesBrowser.Program.LoadImage(fileName);

            if(image != null)
            {
                Dimensions = image.PixelWidth + "X" + image.PixelHeight;
                PixelFormat = image.Format;
            }

            Snapshot = image;
            return Snapshot;
        }

        protected override ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {
            var fileInfo = new System.IO.FileInfo(resourceFile);
            var retValue = new TextureResourceInfo();
            retValue.Name = fileInfo.Name.Replace(fileInfo.Extension, "");
            retValue.ResourceType = "Texture";

            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Info, "添加贴图文件" + fileInfo.Name);

            return retValue;
        }

        public void ForceReload()
        {
            CCore.Graphics.Texture.ForceReloadTexture(RelativeResourceFileName);
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
                    }
                }, AbsResourceFileName);
            }
            else
                System.IO.File.Delete(AbsResourceFileName);
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
                    System.IO.File.Move(AbsResourceFileName, absFolder + ResourceFileName);
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
    }
}
