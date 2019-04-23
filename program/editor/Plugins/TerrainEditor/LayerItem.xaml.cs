using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace TerrainEditor
{
    /// <summary>
    /// Interaction logic for LayerItem.xaml
    /// </summary>
    public partial class LayerItem : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public enum enLayerType
        {
            Unknow,
            HightMap,
            Material,
            Mesh,
            Count,
        }

        TerrainPanel mParentTerrainPanel = null;

        enLayerType mLayerType = enLayerType.Unknow;
        public enLayerType LayerType
        {
            get { return mLayerType; }
            protected set
            {
                mLayerType = value;

                switch (mLayerType)
                {
                    case enLayerType.HightMap:
                        {
                            LayerName = "高度信息";
                            Image = new BitmapImage(new Uri("pack://application:,,,/TerrainEditor;component/Icon/terrain.png"));
                            Button_SetValue.Visibility = System.Windows.Visibility.Collapsed;
                            Button_SearchValue.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        break;

                    case enLayerType.Material:
                    case enLayerType.Mesh:
                        break;
                }

                OnPropertyChanged("LayerType");
            }
        }

        ImageSource mImage = null;
        public ImageSource Image
        {
            get { return mImage; }
            set
            {
                mImage = value;
                OnPropertyChanged("Image");
            }
        }

        string mLayerName = "";
        public string LayerName
        {
            get { return mLayerName; }
            protected set
            {
                mLayerName = value;
                OnPropertyChanged("LayerName");
            }
        }

        public LayerItem(enLayerType layerType, TerrainPanel parentTerrainPanel)
        {
            InitializeComponent();

            LayerType = layerType;
            mParentTerrainPanel = parentTerrainPanel;

            switch (LayerType)
            {
                case enLayerType.HightMap:
                    Border_Grass.Visibility = Visibility.Collapsed;
                    TextBox_Remark.Visibility = Visibility.Collapsed;
                    break;

                default:
                    break;
            }
        }

        // 材质相关
        public Guid OldMaterialId = Guid.Empty;
        public Guid MaterialId = Guid.Empty;//CSUtility.Support.IFileConfig.DefaultMaterialId;
        protected CCore.Grass.GrassData mGrass = null;
        public CCore.Grass.GrassData Grass
        {
            get { return mGrass; }
            set
            {
                if (mGrass != null)
                    mGrass.PropertyChanged -= Grass_PropertyChanged;

                mGrass = value;
                if (mGrass != null)
                {
                    mGrass.OwnerMatId = MaterialId;
                    mGrass.PropertyChanged += Grass_PropertyChanged;
                }
                GrassPropertyGrid.Instance = value;
            }
        }

        void Grass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MeshTemplateId")
            {
                if (mGrass == null || mGrass.MeshTemplateId == Guid.Empty)
                    Border_Grass.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                else
                    Border_Grass.BorderBrush = Brushes.Green;
            }
        }

        public string mRemarks = "";
        public string Remarks
        {
            get { return mRemarks; }
            set
            {
                SetRemarks(value);
                mRemarks = value;
                OnPropertyChanged("Remarks");
            }
        }

        private void Button_SetValue_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (LayerType)
            {
                case enLayerType.Material:
                    {
                        var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("Material");
                        if (data != null && data.Length > 0)
                        {
                            var matId = CSUtility.Program.GetIdFromFile((string)data[0]);
                            SetMaterialData(matId);
                        }
//                         Guid matId = Guid.Empty;
//                         if (Guid.TryParse(WPG.Data.EditorContext.SelectedMaterialData, out matId))
//                         {
//                             //MaterialData = WPG.Data.EditorContext.SelectedMaterialData;
//                             //var snapShotImageFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EditorSourceDirectory + "/Snapshots/Materials/" + WPG.Data.EditorContext.SelectedMaterialData + ".bmp";
//                             //Image = EditorCommon.ImageInit.GetImage(snapShotImageFile);
// 
//                             //LayerName = EditorCommon.Material.MaterialFileAssist.GetMaterialName(matId);
//                             SetMaterialData(matId);
//                         }
                    }
                    break;

                case enLayerType.Mesh:
                    {

                    }
                    break;
            }
        }

        private void Button_SearchValue_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (LayerType)
            {
                case enLayerType.Material:
                    {
                        var materialFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(MaterialId);
                        EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", materialFile });
                        //EditorCommon.ResourceSearch.ShowResource("MaterialBrowser", new object[] { MaterialId });
                        //MainEditor.MainWindow.Instance.ShowResource(EditorCommon.ResourceSearch.enResourceType.Material, MaterialId);
                    }
                    break;

                case enLayerType.Mesh:
                    {

                    }
                    break;
            }
        }

        private void SetRemarks(string remark)
        {
            if (Remarks == remark)
                return;

            if (mParentTerrainPanel.HostTerrain != null)
            {
                mParentTerrainPanel.HostTerrain.ResetLayerRemarks(MaterialId, remark);
            }
        }

        public void SetMaterialData(Guid matId)
        {
            if (MaterialId == matId)
                return;

//             CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory
//             var str = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(matId);
//             str += "_Snapshot.png";

            // 判断设置的材质是否在列表中重复
            foreach (LayerItem item in mParentTerrainPanel.GetLayers())
            {
                if (item.LayerType == enLayerType.Material)
                {
                    if (item.MaterialId == matId)
                    {
                        var matName = EditorCommon.Material.MaterialFileAssist.GetMaterialName(matId);
                        EditorCommon.MessageBox.Show("列表中已有材质" + matName + ", 无法再设置");
                        return;
                    }
                }
            }

            bool bResetValue = false;
            if (MaterialId != Guid.Empty)
            {
                bResetValue = true;
            }

            OldMaterialId = MaterialId;
            MaterialId = matId;
            if (mGrass != null)
                mGrass.OwnerMatId = MaterialId;

            LayerName = EditorCommon.Material.MaterialFileAssist.GetMaterialName(matId);
            if (string.IsNullOrEmpty(LayerName))
            {
                LayerName = "找不到材质";

                TextBox_Remark.IsEnabled = false;
            }
            else
            {
                //var snapShotImageFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EditorSourceDirectory + "/Snapshots/Materials/" + MaterialId.ToString() + ".bmp";
                string str = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(matId);
                if (!string.IsNullOrEmpty(str))
                {
                    var snapShotImageFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + str + "_Snapshot.png";
                    Image = EditorCommon.ImageInit.GetImage(snapShotImageFile/*, System.Drawing.Color.FromArgb(255, 0, 255)*/);
                }

                TextBox_Remark.IsEnabled = true;
            }

            if (bResetValue)
            {
                mParentTerrainPanel._OnResetLayer(this);
            }
            else
            {
                if (mParentTerrainPanel.HostTerrain != null)
                {
                    mParentTerrainPanel.HostTerrain.AddLayerMaterial(MaterialId);
                }
            }
        }
    }
}
