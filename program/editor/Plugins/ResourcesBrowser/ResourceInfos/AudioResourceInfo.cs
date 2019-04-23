using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ResourcesBrowser.ResourceInfos
{
    [ResourceInfoAttribute(ResourceInfoType = "Audio", ResourceExts = new string[] { ".mp3", ".wma", ".wav", ".rm", ".midi" })]
    public class AudioResourceInfo : ResourceInfo, IResourceInfoEditor,IResourceInfoDragToGameWindow
    {
        CCore.World.Actor mGameWindowDragActor = null;
        public void OnDragEnterGameWindow(System.Windows.Forms.Form gameWindow, System.Windows.Forms.DragEventArgs e)
        {
            var audioActor = new CCore.World.AudioActor();
            var audioActorInit = new CCore.World.AudioActorInit();
            audioActor.ParticipationLineCheck = false;
            
            audioActor.Initialize(audioActorInit);
            audioActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            audioActor.SetPlacement(new CSUtility.Component.StandardPlacement(audioActor));
            CCore.Engine.Instance.Client.MainWorld.AddEditorActor(audioActor);
            mGameWindowDragActor = audioActor;
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

            var audioActor = new CCore.World.AudioActor();
            var audioActorInit = new CCore.World.AudioActorInit();
            audioActor.ParticipationLineCheck = false;
            audioActor.Initialize(audioActorInit);
            audioActor.ActorName = "声音" + CCore.Program.GetActorIndex();
            audioActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient);
            audioActor.SetPlacement(new CSUtility.Component.StandardPlacement(audioActor));
            audioActor.Placement.SetLocation(ref hitPos);
            audioActor.Visible = true;
            CCore.Engine.Instance.Client.MainWorld.AddActor(audioActor);
            return audioActor;
        }
        public AudioResourceInfo()
        {
            AudioResourceInfo.Init();
        }

        [ResourceToolTipAttribute]
        [DisplayName("类型")]
        public override string ResourceTypeName
        {
            get { return "音源"; }
        }        

        string mFileExtension;
        [ResourceToolTipAttribute]
        [DisplayName("扩展名")]
        public override string FileExtension
        {
            get { return mFileExtension; }
            set
            {
                mFileExtension = value;                

                OnPropertyChanged("FileExtension");
            }
        }

        Brush mResourceTypeBrush = Brushes.Red;
        public override Brush ResourceTypeBrush
        {
            get { return mResourceTypeBrush; }
            set
            {
                mResourceTypeBrush = value;
                OnPropertyChanged("ResourceTypeBrush");
            }
        }

        ImageSource mResourceIcon = new BitmapImage(new System.Uri("pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_Sound.png", UriKind.Absolute));
        public override ImageSource ResourceIcon
        {
            get { return mResourceIcon; }
            set
            {
                mResourceIcon = value;
                OnPropertyChanged("ResourceIcon");
            }
        }

        public override ImageSource GetSnapshotImage(bool forceCreate)
        {
            //var audioLength = CCore.Audio.AudioManager.Instance.GetSoundLength(AbsResourceFileName, CCore.Audio.AudioManager.enAudioTimeUnit.FMOD_TIMEUNIT_MS);

            Snapshot = mResourceIcon;
            return Snapshot;
        }

        protected override ResourceInfo CreateResourceInfoFromResourceOverride(string resourceFile)
        {
            var fileInfo = new System.IO.FileInfo(resourceFile);
            var retValue = new AudioResourceInfo();
            retValue.Name = fileInfo.Name.Replace(fileInfo.Extension, "");
            retValue.ResourceType = "Audio";
            
            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Info, "添加音频文件" + fileInfo.Name);

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
                        
        public void OpenEditor()
        {                                                            
            if (AudioResourceInfo.CurAudioRes == null || this.Id == AudioResourceInfo.CurAudioRes.Id)
            {
                if (CCore.Audio.AudioManager.Instance.IsPlaying(this.Id))
                {
                    CCore.Audio.AudioManager.Instance.Stop(this.Id);
                    AudioPlaying = false;
                }
                else
                {
                    AudioResourceInfo.CurAudioRes = this;
                    CCore.Audio.AudioManager.Instance.Play(this.RelativeResourceFileName, this.Id, (UInt32)(CCore.Performance.ESoundType.SoundEffect));
                    AudioPlaying = true;
                }
            }
            else
            {                
                CCore.Audio.AudioManager.Instance.Stop(AudioResourceInfo.CurAudioRes.Id);
                AudioPlaying = false;

                AudioResourceInfo.CurAudioRes = this;
                CCore.Audio.AudioManager.Instance.Play(this.RelativeResourceFileName, this.Id, (UInt32)(CCore.Performance.ESoundType.SoundEffect));
                AudioPlaying = true;
            }                        
        }

        #region Audio
        static DispatcherTimer mAnimaTimer = new DispatcherTimer();
        static AudioResourceInfo CurAudioRes { get; set; }

        static ImageSource[] mSoundIcon = new BitmapImage[4];
        static int frame = 0;
        static bool loaded = false;

        static bool mAudioPlaying = false;
        static bool AudioPlaying
        {
            set
            {
                mAudioPlaying = value;

                if (value)
                {
                    frame = 0;
                }
                else
                {
                    if (CurAudioRes != null)
                        CurAudioRes.Snapshot = mSoundIcon[3];
                }
                mAnimaTimer.IsEnabled = value;

            }
            get
            {
                return mAudioPlaying;
            }
        }
        static public void Init()
        {
            if (loaded)
                return;

            for (int i = 0; i < 4; ++i)
            {
                var url = "pack://application:,,,/ResourcesBrowser;component/Icon/ResourceIcons/format_thumbnail_Sound" + i.ToString() + ".png";
                mSoundIcon[i] = new BitmapImage(new System.Uri(url, UriKind.Absolute));
            }

            loaded = true;
            CCore.Audio.AudioManager.Instance.OnPlayFinished += (id) => 
            {
                AudioPlaying = false;
            };

            mAnimaTimer.Interval = TimeSpan.FromMilliseconds(300);
            mAnimaTimer.Tick += MAnimaTimer_Tick;
            mAnimaTimer.Start();      

        }

        private static void MAnimaTimer_Tick(object sender, EventArgs e)
        {
            if (AudioPlaying)
            {
                AudioResourceInfo.CurAudioRes.Snapshot = mSoundIcon[frame];

                frame = (frame + 1) % 4;
            }            
        }             
        #endregion
    }
}
