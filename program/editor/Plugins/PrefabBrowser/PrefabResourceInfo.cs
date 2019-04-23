using System;
using System.Windows.Media;

namespace PrefabBrowser
{
    public class PrefabResourceInfo : ResourceBrowser.ResourceInfo
    {
        public ResourceBrowser.Program.enCompareResult Compare(ResourceBrowser.ResourceInfo info)
        {
            var texResInfo = info as PrefabResourceInfo;
            if (texResInfo == null)
                return ResourceBrowser.Program.enCompareResult.Equal;

            return ResourceBrowser.Program.CompareString(FilterName, info.FilterName);
        }

        bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
            }
        }

        public string FilterName
        {
            get
            {
                if (PrefabResource != null)
                    return PrefabResource.PrefabName;

                return "";
            }
        }

        public string ResourceType
        {
            get { return "Prefab"; }
        }

        string mAbsFileName = "";
        public string AbsFileName
        {
            get { return mAbsFileName; }
        }

        ResourceInfoControl mInfoElement = null;
        public System.Windows.FrameworkElement InfoElement
        {
            get { return mInfoElement; }
        }

        public System.Windows.FrameworkElement ToolTipsElement
        {
            get { return null; }
        }

        bool mIsSelected = false;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                mIsSelected = value;

                if (mIsSelected)
                {
                    if (mInfoElement != null)
                        mInfoElement.ForegroundBrush = Brushes.Black;

                    if (HostControl != null)
                    {
                        HostControl.ShowCurrentResourceInfo(this);
                    }
                }
                else
                {
                    if (mInfoElement != null)
                        mInfoElement.ForegroundBrush = Brushes.White;

                    if (HostControl != null)
                    {
                        HostControl.ShowCurrentResourceInfo(null);
                    }
                }
            }
        }

        public void OnDeleted()
        {
        }

        public MainControl HostControl;
        CCore.World.Prefab.PrefabResource mPrefabResource;
        public CCore.World.Prefab.PrefabResource PrefabResource
        {
            get { return mPrefabResource; }
        }

        public ResourceBrowser.ResourceControl HostResourceControl { get; set; }
        public System.Windows.FrameworkElement GetDragVisual()
        {
            return HostResourceControl;
        }

        public PrefabResourceInfo()
        {
            mInfoElement = new ResourceInfoControl();
            mInfoElement.OnOpenInExplorer = _OnOnOpenInExplorer;
        }

        public bool SetAbsFileName(string absFileName)
        {
            if (!System.IO.File.Exists(absFileName))
                return false;

            var fileInfo = new System.IO.FileInfo(absFileName);
            if (fileInfo.Extension != CSUtility.Support.IFileConfig.PrefabResExtension)
                return false;

            var pureFile = fileInfo.Name.Replace(fileInfo.Extension, "");
            var prefabId = CSUtility.Support.IHelper.GuidTryParse(pureFile);
            if (prefabId == Guid.Empty)
                return false;

            mPrefabResource = CCore.World.Prefab.PrefabResourceManager.Instance.FindPrefabResource(prefabId);
            if (mPrefabResource == null)
                return false;

            mAbsFileName = absFileName;

            mInfoElement.SetBinding(ResourceInfoControl.FileNameProperty, new System.Windows.Data.Binding("PrefabName") { Source = mPrefabResource });
            return true;
        }

        private void _OnOnOpenInExplorer()
        {
            System.Diagnostics.Process.Start("Explorer", "/select," + AbsFileName.Replace("/", "\\"));
        }
    }
}
