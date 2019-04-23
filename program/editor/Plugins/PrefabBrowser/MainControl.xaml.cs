using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace PrefabBrowser
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "PrefabBrowser")]
    [EditorCommon.PluginAssist.PluginMenuItem("资源浏览器/PrefabBrowser")]
    [Guid("2C4C5C8F-3BDD-40AC-89AA-6A079DCECAEF")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, EditorCommon.PluginAssist.IEditorPlugin
    {
        public string PluginName
        {
            get { return "PrefabBrowser"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "PrefabBrowser",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {
            if (!d3dShowInited)
                InitializeD3DShow();

            if (mD3DShowPlugin != null)
                mD3DShowPlugin.Tick();
        }

#region D3DPreView

        [Import("D3DShow", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
        EditorCommon.PluginAssist.IEditorPlugin mD3DShowPlugin = null;
        public EditorCommon.PluginAssist.IEditorPlugin D3DShowPlugin
        {
            get { return mD3DShowPlugin; }
        }

        bool d3dShowInited = false;
        void InitializeD3DShow(bool force = false)
        {
            if (d3dShowInited && !force)
                return;

            if (mD3DShowPlugin == null)
                return;

            if (mD3DShowPlugin is FrameworkElement)
            {
                mExtendControl.SetD3DViewer(mD3DShowPlugin as FrameworkElement);
            }

            d3dShowInited = true;
        }

#endregion

        ExtendControl mExtendControl;

        public MainControl()
        {
            InitializeComponent();

            mExtendControl = new ExtendControl();
            Browser.SetExtendControl(mExtendControl);

            Browser.OnCreateResourceInfo = _CreateResourceInfo;
            Browser.OnGetSnapshotImage = _GetSnapshotImage;
            Browser.OnGetResourceFileFilter = _GetResourceFileFilter;
            Browser.ShowFolderTree(CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(CSUtility.Support.IFileConfig.DefaultPrefabFolder));

        }

        private ResourceBrowser.ResourceInfo _CreateResourceInfo(string resourceAbsFile)
        {
            var retInfo = new PrefabResourceInfo();
            retInfo.HostControl = this;
            if (!retInfo.SetAbsFileName(resourceAbsFile))
                return null;

            return retInfo;
        }

        private System.Windows.Media.ImageSource _GetSnapshotImage(ResourceBrowser.SnapshotProcess.SnapshotProcessInfo info)
        {
            var resInfo = info.ResourceInfo as PrefabResourceInfo;
            if (resInfo == null)
                return null;

            if (string.IsNullOrEmpty(resInfo.AbsFileName))
                return null;

            var snapShotFile = resInfo.AbsFileName + "_Snapshot.png";
            if (System.IO.File.Exists(snapShotFile) && !info.ForceCreate)
            {
                return ResourceBrowser.Program.LoadImage(snapShotFile);
            }
            else
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var vMax = SlimDX.Vector3.UnitXYZ * float.MinValue;
                    var vMin = SlimDX.Vector3.UnitXYZ * float.MaxValue;
                    var prefabActors = resInfo.PrefabResource.Actors;
                    foreach (var actor in prefabActors)
                    {
                        SlimDX.Vector3 tempMax = SlimDX.Vector3.UnitXYZ, tempMin = SlimDX.Vector3.UnitXYZ;
                        actor.GetAABB(ref tempMin, ref tempMax);

                        if (vMax.X < tempMax.X)
                            vMax.X = tempMax.X;
                        if (vMax.Y < tempMax.Y)
                            vMax.Y = tempMax.Y;
                        if (vMax.Z < tempMax.Z)
                            vMax.Z = tempMax.Z;

                        if (vMin.X > tempMin.X)
                            vMin.X = tempMin.X;
                        if (vMin.Y > tempMin.Y)
                            vMin.Y = tempMin.Y;
                        if (vMin.Z > tempMin.Z)
                            vMin.Z = tempMin.Z;

                        ResourceBrowser.SnapshotProcess.SnapshotCreator.Instance.World.AddActor(actor);
                    }
                    ResourceBrowser.SnapshotProcess.SnapshotCreator.Instance.CalculateCamera(1.0f);

                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    ResourceBrowser.SnapshotProcess.SnapshotCreator.Instance.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);

                    foreach(var actor in prefabActors)
                        ResourceBrowser.SnapshotProcess.SnapshotCreator.Instance.World.RemoveActor(actor);
                }));

                return ResourceBrowser.Program.LoadImage(snapShotFile);
            }
        }

        private string _GetResourceFileFilter()
        {
            return "*" + CSUtility.Support.IFileConfig.PrefabResExtension;
        }

        PrefabResourceInfo mCurrentResourceInfo;
        public void ShowCurrentResourceInfo(PrefabResourceInfo resInfo)
        {
            if (resInfo == null)
            {
                mD3DShowPlugin.SetObjectToEdit(null);
                mCurrentResourceInfo = resInfo;
            }
            else
            {
                mCurrentResourceInfo = resInfo;
                D3DShowPlugin.SetObjectToEdit(new object[] { new object[] { "Prefab", false },
                                                             new object[] { mCurrentResourceInfo.PrefabResource }});
            }
        }
    }
}
