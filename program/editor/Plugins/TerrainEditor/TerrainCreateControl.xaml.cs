using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TerrainEditor
{
    /// <summary>
    /// Interaction logic for TerrainCreateControl.xaml
    /// </summary>
    public partial class TerrainCreateControl : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        string mHeightMapName = "";
        public string HeightMapName
        {
            get { return mHeightMapName; }
            set
            {
                if (mHeightMapName != value)
                {
                    mHeightMapName = value;

                    OnPropertyChanged("HeightMapName");                    
                }
            }
        }

        uint mUnitHeight = 255;
        public uint UnitHeight
        {
            get { return mUnitHeight; }
            set
            {
                mUnitHeight = value;
                OnPropertyChanged("UnitHeight");
            }
        }

        uint mTerrainSizeX = 2;
        public uint TerrainSizeX
        {
            get { return mTerrainSizeX; }
            set
            {
                if (value <= 0 || value > 512)
                    return;

                mTerrainSizeX = value;

                UpdateTerrainSizeInfo();

                OnPropertyChanged("TerrainSizeX");
            }
        }

        string mTerrainSizeInfoX = "场景X方向长512米";
        public string TerrainSizeInfoX
        {
            get { return mTerrainSizeInfoX; }
            set
            {
                mTerrainSizeInfoX = value;
                OnPropertyChanged("TerrainSizeInfoX");
            }
        }

        uint mTerrainSizeY = 2;
        public uint TerrainSizeY
        {
            get { return mTerrainSizeY; }
            set
            {
                if (value <= 0 || value > 512)
                    return;

                mTerrainSizeY = value;

                UpdateTerrainSizeInfo();

                OnPropertyChanged("TerrainSizeY");
            }
        }

        string mTerrainSizeInfoY = "场景Z方向长512米";
        public string TerrainSizeInfoY
        {
            get { return mTerrainSizeInfoY; }
            set
            {
                mTerrainSizeInfoY = value;
                OnPropertyChanged("TerrainSizeInfoY");
            }
        }

        uint mPatchPerLevel = 4;
        public uint PatchPerLevel
        {
            get { return mPatchPerLevel; }
            set
            {
                mPatchPerLevel = value;

                var count = System.Math.Pow(2, mPatchPerLevel);

                PatchPerLevelInfo = "每个Level边长" + System.Math.Pow(2, mPatchPerLevel) * System.Math.Pow(2, mTessellation) + "米";

                UpdateTerrainSizeInfo();

                OnPropertyChanged("PatchPerLevel");
            }
        }
        string mPatchPerLevelInfo = "每Level边长256米";
        public string PatchPerLevelInfo
        {
            get { return mPatchPerLevelInfo; }
            set
            {
                mPatchPerLevelInfo = value;
                OnPropertyChanged("PatchPerLevelInfo");
            }
        }

        uint mTessellation = 4;
        public uint Tessellation
        {
            get { return mTessellation; }
            set
            {
                mTessellation = value;

                var count = System.Math.Pow(2, mTessellation);
                TessellationInfo = "每个Patch包含" + count + "*" + count + "个格子";

                UpdateTerrainSizeInfo();

                OnPropertyChanged("Tessellation");
            }
        }
        string mTessellationInfo = "每个Patch包含16*16个格子";
        public string TessellationInfo
        {
            get { return mTessellationInfo; }
            set
            {
                mTessellationInfo = value;
                OnPropertyChanged("TessellationInfo");
            }
        }

        Guid mMaterialId = Guid.Empty;
        public Guid MaterialId
        {
            get { return mMaterialId; }
            set
            {
                mMaterialId = value;

                BaseMaterialName = EditorCommon.Material.MaterialFileAssist.GetMaterialName(mMaterialId);
                if (string.IsNullOrEmpty(BaseMaterialName))
                    BaseMaterialName = "找不到材质";
                else
                {
                    //var snapShotImageFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EditorSourceDirectory + "/Snapshots/Materials/" + mMaterialId.ToString() + ".bmp";
                    string str = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(mMaterialId);
                    if (!string.IsNullOrEmpty(str))
                    {
                        var snapShotImageFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + str + "_Snapshot.png";
                        Image = EditorCommon.ImageInit.GetImage(snapShotImageFile/*, System.Drawing.Color.FromArgb(255, 0, 255)*/);
                    }
                }

                OnPropertyChanged("MaterialId");
            }
        }

        //CCore.Terrain.TerrainInfo mTerrainInfo = new CCore.Terrain.TerrainInfo();

        string mBaseMaterialName = "";
        public string BaseMaterialName
        {
            get { return mBaseMaterialName; }
            set
            {
                mBaseMaterialName = value;
                OnPropertyChanged("BaseMaterialName");
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

        public TerrainCreateControl()
        {
            InitializeComponent();
            MaterialId = CSUtility.Support.IFileConfig.DefaultMaterialId;

            mDropHeightMapAdorner = new EditorCommon.DragDrop.DropAdorner(Rectangle_DragDropPlaceHeightMap);
            //mDropMaterialAdorner = new EditorCommon.DragDrop.DropAdorner(Rectangle_DragDropPlaceMaterial);
            ProGrid.Instance = new CCore.Terrain.TerrainInfoOperator();
        }

        public bool IsValid()
        {
            if (TerrainSizeX == 0 || TerrainSizeY == 0 ||
                TerrainSizeX >= 512 || TerrainSizeY >= 512)
                return false;

            if (mMaterialId == Guid.Empty)
                return false;

            return true;
        }

        private void UpdateTerrainSizeInfo()
        {
            TerrainSizeInfoX = "场景X方向长" + TerrainSizeX * System.Math.Pow(2, mPatchPerLevel) * System.Math.Pow(2, mTessellation) + "米";
            TerrainSizeInfoY = "场景Z方向长" + TerrainSizeY * System.Math.Pow(2, mPatchPerLevel) * System.Math.Pow(2, mTessellation) + "米";
        }

        private void Button_SetBaseMaterial_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("Material");
            if (data != null && data.Length > 0)
            {
                MaterialId = CSUtility.Program.GetIdFromFile((string)data[0]);
            }
//             Guid matId = Guid.Empty;
//             Guid.TryParse(WPG.Data.EditorContext.SelectedMaterialData, out matId);
//             MaterialId = matId;
        }

        // terrainName路径相对于Release
        public void CreateTerrain(string terrainName, CCore.Terrain.Terrain terrain)
        {
            if (terrain == null)
                return;

            var info = new CCore.Terrain.TerrainInfo();
            //info.ResetDefault();
            info.SetParameter(TerrainSizeX, TerrainSizeY, PatchPerLevel, Tessellation);
            info.GridStep = new SlimDX.Vector3(1.0f, 0.1f, 1.0f);
            terrain.Initialize(terrainName, ref info);

            for (uint y = 0; y < TerrainSizeY; y++)
            {
                for (uint x = 0; x < TerrainSizeX; x++)
                {
                    terrain.AddLevel(x, y);
                }
            }

            terrain.SetBaseMaterial(mMaterialId);
            terrain.TravelTo(0, 0);

#warning 插件化备注：创建地形时给编辑器设置地形
            ////////////var terrainPanel = Program.GetControl(typeof(MainEditor.Panel.TerrainPanel), MainEditor.Panel.TerrainPanel.StaticKeyValue) as MainEditor.Panel.TerrainPanel;
            ////////////if (terrainPanel != null)
            ////////////    terrainPanel.HostTerrain = terrain;
            //////////////MainEditor.Panel.TerrainPanel.Instance.UpdateTerrainMaterialLayers(terrain);
        }
        
//        public void UpdateTerrain(CCore.Terrain.Terrain terrain)
//        {
//            if (terrain == null)
//                return;
            
////             UpdateTerrainLevel(terrain);
////             UpdateTerrainPatch(terrain);
//            UpdateTerrainInfo(terrain);
////             terrain.SetBaseMaterial(mMaterialId);
////             UpdateTerrainHeight(terrain);
//        }

//        void UpdateTerrainInfo(CCore.Terrain.Terrain terrain)
//        {
//            var terrainInfoOperator = ProGrid.Instance as CCore.Terrain.TerrainInfoOperator;
//            if (terrainInfoOperator == null)
//                return;

//            CCore.Engine.Instance.Client.MainWorld.Create("", terrainInfoOperator.OpInfo, "地形");
//        }

        void UpdateTerrainHeight(CCore.Terrain.Terrain terrain)
        {
            if (string.IsNullOrEmpty(HeightMapName))
                return;

            var resourceIcon = new BitmapImage(new System.Uri(HeightMapName));
            if (resourceIcon == null)
                return;

            var stride = resourceIcon.Format.BitsPerPixel * resourceIcon.PixelWidth / 8;
            byte[] pixelData = new byte[resourceIcon.PixelHeight * stride];
            resourceIcon.CopyPixels(pixelData, stride, 0);

            byte[] tagImagePixelData = pixelData;
            var tagImageSizeX = (uint)resourceIcon.PixelWidth;
            var tagImageSizeY = (uint)resourceIcon.PixelHeight;

            var GridXCount = terrain.GetGridXCount();
            var GridZCount = terrain.GetGridZCount();

            for (uint i = 0; i < GridXCount; i++)
            {
                for (uint j = 0; j < GridZCount; j++)
                {
                    var pixelX = i * tagImageSizeX / GridXCount;
                    var pixelY = j * tagImageSizeY / GridZCount;

                    int modifyData = (int)(tagImagePixelData[pixelY * tagImageSizeX + pixelX] * mUnitHeight / 255);

                    CCore.Client.MainWorldInstance.Terrain.SetHeight(i, j, (short)modifyData, true);
                }
            }
        }

        void UpdateTerrainPatch(CCore.Terrain.Terrain terrain)
        {
            if (terrain.GetPatchXCountPerLevel() < mPatchPerLevel)
            {
                for (uint idu = terrain.GetPatchXCountPerLevel(); idu < mPatchPerLevel; idu++)
                {
                    for (uint idv = 0; idv < terrain.GetPatchZCountPerLevel(); idv++)
                    {
                        terrain.AddPatch(idu, idv);
                    }
                }
            }
            else if (terrain.GetPatchXCountPerLevel() > mPatchPerLevel)
            {
                for (uint idu = mPatchPerLevel; idu < terrain.GetPatchXCountPerLevel(); idu++)
                {
                    for (uint idv = 0; idv < terrain.GetPatchZCountPerLevel(); idv++)
                    {
                        terrain.DelPatch(idu, idv);
                    }
                }
            }
            if (terrain.GetPatchZCountPerLevel() < mPatchPerLevel)
            {
                for (uint idv = terrain.GetPatchZCountPerLevel(); idv < mPatchPerLevel; idv++)
                {
                    for (uint idu = 0; idu < terrain.GetPatchXCountPerLevel(); idu++)
                    {
                        terrain.AddPatch(idu, idv);
                    }
                }
            }
            else if (terrain.GetPatchZCountPerLevel() > mPatchPerLevel)
            {
                for (uint idv = mPatchPerLevel; idv < terrain.GetPatchZCountPerLevel(); idv++)
                {
                    for (uint idu = 0; idu < terrain.GetPatchXCountPerLevel(); idu++)
                    {
                        terrain.DelPatch(idu, idv);
                    }
                }
            }
        }

        void UpdateTerrainLevel(CCore.Terrain.Terrain terrain)
        {
            if (terrain.GetLevelXCount() < mTerrainSizeX)
            {
                for (uint idu = terrain.GetLevelXCount(); idu < mTerrainSizeX; idu++)
                {
                    for (uint idv = 0; idv < terrain.GetLevelZCount(); idv++)
                    {
                        terrain.AddLevel(idu, idv);
                    }
                }
            }
            else if (terrain.GetLevelXCount() > mTerrainSizeX)
            {
                for (uint idu = mTerrainSizeX; idu < terrain.GetLevelXCount(); idu++)
                {
                    for (uint idv = 0; idv < terrain.GetLevelZCount(); idv++)
                    {
                        terrain.DelLevel(idu, idv);
                    }
                }
            }
            if (terrain.GetLevelZCount() < mTerrainSizeY)
            {
                for (uint idv = terrain.GetLevelZCount(); idv < mTerrainSizeY; idv++)
                {
                    for (uint idu = 0; idu < terrain.GetLevelXCount(); idu++)
                    {
                        terrain.AddLevel(idu, idv); ;
                    }
                }
            }
            else if (terrain.GetLevelZCount() > mTerrainSizeY)
            {
                for (uint idv = mTerrainSizeY; idv < terrain.GetLevelZCount(); idv++)
                {
                    for (uint idu = 0; idu < terrain.GetLevelXCount(); idu++)
                    {
                        terrain.DelLevel(idu, idv);
                    }
                }
            }
        }

        private void Button_OpenHeightMapClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                HeightMapName = ofd.FileName;

            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression be = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }
        }

        private void Button_SearchMaterialClick(object sender, RoutedEventArgs e)
        {
            //MainEditor.MainWindow.Instance.ShowResource(EditorCommon.ResourceSearch.enResourceType.Material, MaterialId);
            EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "MaterialBrowser", MaterialId });
            //EditorCommon.ResourceSearch.ShowResource("MaterialBrowser", new object[] { MaterialId });
        }

        #region DragDrop

        // 检测资源文件是否可拖拽导入
        private bool CheckFileDropAvailable(string[] files)
        {
            var retImage = EditorCommon.ImageInit.GetImage(files[0]) as BitmapSource;
            if (retImage != null)
                return true;

            return false;
        }

//         enum enDropResult
//         {
//             Denial_UnknowFormat,
//             Denial_NoDragAbleObject,
//             Allow,
//         }
//         // 是否允许拖放
//         enDropResult AllowResourceHeightMapDrop(System.Windows.DragEventArgs e, string type)
//         {
//             var formats = e.Data.GetFormats();
//             if (formats == null || formats.Length == 0)
//                 return enDropResult.Denial_UnknowFormat;
// 
//             var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
//             if (datas == null)
//                 return enDropResult.Denial_NoDragAbleObject;
// 
//             bool containDefault = false;
//             foreach (var data in datas)
//             {
//                 var resInfo = data as ResourcesBrowser.ResourceInfo;
//                 if (resInfo.ResourceType == type)
//                 {
//                     containDefault = true;
//                     break;
//                 }
//             }
// 
//             if (!containDefault)
//                 return enDropResult.Denial_NoDragAbleObject;
// 
//             return enDropResult.Allow;
//         }

        EditorCommon.DragDrop.DropAdorner mDropHeightMapAdorner;
        public EditorCommon.DragDrop.DropAdorner DropHeightMapAdorner
        {
            get { return mDropHeightMapAdorner; }
        }

        private void Rectangle_DragDropPlaceHeightMap_DragEnter(object sender, DragEventArgs e)
        {
            bool allowDrop = false;

            var datas = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (datas != null && datas.Length > 0)
            {
                allowDrop = CheckFileDropAvailable(datas);
            }

            if (allowDrop)
            {
                e.Effects = DragDropEffects.Copy;
                mDropHeightMapAdorner.IsAllowDrop = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                mDropHeightMapAdorner.IsAllowDrop = false;
            }

            var pos = e.GetPosition(Rectangle_DragDropPlaceHeightMap);
            if (pos.X > 0 && pos.X < Rectangle_DragDropPlaceHeightMap.ActualWidth &&
               pos.Y > 0 && pos.Y < Rectangle_DragDropPlaceHeightMap.ActualHeight)
            {
                var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
                layer.Add(mDropHeightMapAdorner);
            }

            e.Handled = true;
            //             if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            //             {
            //                 e.Handled = true;
            //                 mDropHeightMapAdorner.IsAllowDrop = false;
            // 
            //                 switch (AllowResourceHeightMapDrop(e, "Texture"))
            //                 {
            //                     case enDropResult.Allow:
            //                         {
            //                             EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "创建模型部件";
            // 
            //                             mDropHeightMapAdorner.IsAllowDrop = true;
            //                             var pos = e.GetPosition(Rectangle_DragDropPlaceHeightMap);
            //                             if (pos.X > 0 && pos.X < Rectangle_DragDropPlaceHeightMap.ActualWidth &&
            //                                pos.Y > 0 && pos.Y < Rectangle_DragDropPlaceHeightMap.ActualHeight)
            //                             {
            //                                 var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
            //                                 layer.Add(mDropHeightMapAdorner);
            //                             }
            //                         }
            //                         break;
            // 
            //                     case enDropResult.Denial_NoDragAbleObject:
            //                     case enDropResult.Denial_UnknowFormat:
            //                         {
            //                             EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不包含合法的模型资源";
            // 
            //                             mDropHeightMapAdorner.IsAllowDrop = false;
            //                             var pos = e.GetPosition(Rectangle_DragDropPlaceHeightMap);
            //                             if (pos.X > 0 && pos.X < Rectangle_DragDropPlaceHeightMap.ActualWidth &&
            //                                pos.Y > 0 && pos.Y < Rectangle_DragDropPlaceHeightMap.ActualHeight)
            //                             {
            //                                 var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
            //                                 layer.Add(mDropHeightMapAdorner);
            //                             }
            //                         }
            //                         break;
            //                 }
            //             }
        }

        private void Rectangle_DragDropPlaceHeightMap_DragLeave(object sender, DragEventArgs e)
        {
            var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
            layer.Remove(mDropHeightMapAdorner);
            //             if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            //             {
            //                 e.Handled = true;
            //                 EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
            //                 var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
            //                 layer.Remove(mDropHeightMapAdorner);
            //             }
        }

        private void Rectangle_DragDropPlaceHeightMap_DragOver(object sender, DragEventArgs e)
        {
//             if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
//             {
//                 e.Handled = true;
//                 if (AllowResourceHeightMapDrop(e, "Texture") == enDropResult.Allow)
//                 {
//                     e.Effects = DragDropEffects.Move;
//                 }
//                 else
//                 {
//                     e.Effects = DragDropEffects.None;
//                 }
//             }
        }

        private void Rectangle_DragDropPlaceHeightMap_Drop(object sender, DragEventArgs e)
        {
            var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
            layer.Remove(mDropHeightMapAdorner);

            var datas = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (datas == null)
                return;
            if (datas.Length == 0)
                return;

            if (datas.Length > 0)
            {
                if (!CheckFileDropAvailable(datas))
                    return;

                HeightMapName = datas[0];
            }
            //             if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            //             {
            //                 e.Handled = true;
            //                 var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
            //                 layer.Remove(mDropHeightMapAdorner);
            // 
            //                 if (AllowResourceHeightMapDrop(e, "Texture") == enDropResult.Allow)
            //                 {
            //                     var formats = e.Data.GetFormats();
            //                     var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
            //                     foreach (var data in datas)
            //                     {
            //                         var resInfo = data as ResourcesBrowser.ResourceInfo;
            //                         if (resInfo == null)
            //                             continue;
            // 
            //                         if (resInfo.ResourceType != "Texture")
            //                             continue;
            // 
            //                         HeightMapName = resInfo.RelativeResourceFileName;
            // 
            //                         break;
            //                     }
            //                 }
            //             }
        }


//         EditorCommon.DragDrop.DropAdorner mDropMaterialAdorner;
//         public EditorCommon.DragDrop.DropAdorner DropMaterialAdorner
//         {
//             get { return mDropMaterialAdorner; }
//         }
// 
//         private void Rectangle_DragDropPlaceMaterial_DragEnter(object sender, DragEventArgs e)
//         {
//             if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
//             {
//                 e.Handled = true;
//                 mDropMaterialAdorner.IsAllowDrop = false;
// 
//                 switch (AllowResourceHeightMapDrop(e, "Material"))
//                 {
//                     case enDropResult.Allow:
//                         {
//                             EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "创建模型部件";
// 
//                             mDropMaterialAdorner.IsAllowDrop = true;
//                             var pos = e.GetPosition(Rectangle_DragDropPlaceHeightMap);
//                             if (pos.X > 0 && pos.X < Rectangle_DragDropPlaceHeightMap.ActualWidth &&
//                                pos.Y > 0 && pos.Y < Rectangle_DragDropPlaceHeightMap.ActualHeight)
//                             {
//                                 var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
//                                 layer.Add(mDropMaterialAdorner);
//                             }
//                         }
//                         break;
// 
//                     case enDropResult.Denial_NoDragAbleObject:
//                     case enDropResult.Denial_UnknowFormat:
//                         {
//                             EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不包含合法的模型资源";
// 
//                             mDropMaterialAdorner.IsAllowDrop = false;
//                             var pos = e.GetPosition(Rectangle_DragDropPlaceHeightMap);
//                             if (pos.X > 0 && pos.X < Rectangle_DragDropPlaceHeightMap.ActualWidth &&
//                                pos.Y > 0 && pos.Y < Rectangle_DragDropPlaceHeightMap.ActualHeight)
//                             {
//                                 var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
//                                 layer.Add(mDropMaterialAdorner);
//                             }
//                         }
//                         break;
//                 }
//             }
//         }
// 
//         private void Rectangle_DragDropPlaceMaterial_DragLeave(object sender, DragEventArgs e)
//         {
//             if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
//             {
//                 e.Handled = true;
//                 EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
//                 var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
//                 layer.Remove(mDropMaterialAdorner);
//             }
//         }
// 
//         private void Rectangle_DragDropPlaceMaterial_DragOver(object sender, DragEventArgs e)
//         {
//             if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
//             {
//                 e.Handled = true;
//                 if (AllowResourceHeightMapDrop(e, "Material") == enDropResult.Allow)
//                 {
//                     e.Effects = DragDropEffects.Move;
//                 }
//                 else
//                 {
//                     e.Effects = DragDropEffects.None;
//                 }
//             }
//         }
// 
//         private void Rectangle_DragDropPlaceMaterial_Drop(object sender, DragEventArgs e)
//         {
//             if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
//             {
//                 e.Handled = true;
//                 var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlaceHeightMap);
//                 layer.Remove(mDropMaterialAdorner);
// 
//                 if (AllowResourceHeightMapDrop(e, "Material") == enDropResult.Allow)
//                 {
//                     var formats = e.Data.GetFormats();
//                     var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
//                     foreach (var data in datas)
//                     {
//                         var resInfo = data as ResourcesBrowser.ResourceInfo;
//                         if (resInfo == null)
//                             continue;
// 
//                         if (resInfo.ResourceType != "Material")
//                             continue;
// 
//                         HeightMapName = resInfo.RelativeResourceFileName;
// 
//                         break;
//                     }
//                 }
//             }
//         }


        #endregion
    }
}
