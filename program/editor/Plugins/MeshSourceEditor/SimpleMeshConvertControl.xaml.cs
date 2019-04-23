using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace MeshSourceEditor
{
    /// <summary>
    /// Interaction logic for SimpleMeshConvertControl.xaml
    /// </summary>
    public partial class SimpleMeshConvertControl : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public List<MeshSourceResourceInfo> SelectedSourceControls
        {
            get;
            set;
        }

        public string CurrentSourceDir;

        // 批量处理
        bool mIsBatch = false;
        public bool IsBatch
        {
            get { return mIsBatch; }
            set
            {
                mIsBatch = value;
            }
        }

        // 只处理没有生成过碰撞模型的对象
        bool mOnlyProcessNewMesh = true;
        public bool OnlyProcessNewMesh
        {
            get { return mOnlyProcessNewMesh; }
            set
            {
                mOnlyProcessNewMesh = value;
            }
        }

        public bool ShowSimMesh = true;

        public CCore.Mesh.Mesh CurrentMesh = null;
        public MeshSourceResourceInfo CurrentSourceInfo = null;
        CCore.Support.vIConvexDecomposition mConvexDecomposition;

        public SimpleMeshConvertControl()
        {
            InitializeComponent();

            mConvexDecomposition = new CCore.Support.vIConvexDecomposition();
        }



        // meshName为相对于Release的路径
        private void GenerateSimplateMesh(string meshName, CCore.Mesh.Mesh.enSimpleMeshType type, bool showMessage)
        {
            if(CurrentMesh == null)
            {
                CurrentMesh = new CCore.Mesh.Mesh();
                
                var meshInit = new CCore.Mesh.MeshInit();
                var meshInitPart = new CCore.Mesh.MeshInitPart();
                meshInitPart.MeshName = meshName;
                meshInit.MeshInitParts.Add(meshInitPart);
                CurrentMesh.Initialize(meshInit, null);
            }

            if (CurrentMesh.HasSimplifyMesh && !CurrentMesh.IsSimplifyMeshCreateByEditor && showMessage)
            {
                if (EditorCommon.MessageBox.Show("模型(" + meshName + ")已有的碰撞模型非编辑器生成，是否覆盖？", "警告", EditorCommon.MessageBox.enMessageBoxButton.OKCancel) != EditorCommon.MessageBox.enMessageBoxResult.OK)
                {
                    return;
                }
            }

            // 判断是否有碰撞模型
            if (IsBatch && OnlyProcessNewMesh && CurrentMesh.HasSimplifyMesh)
            {
                return;
            }

            CurrentMesh.ClearSimplifyMesh();

            switch (type)
            {
                case CCore.Mesh.Mesh.enSimpleMeshType.Box:
                    break;

                case CCore.Mesh.Mesh.enSimpleMeshType.Cylinder:
                    break;

                case CCore.Mesh.Mesh.enSimpleMeshType.Sphere:
                    break;

                case CCore.Mesh.Mesh.enSimpleMeshType.Param:
                    mConvexDecomposition.SetConvexData(System.Convert.ToUInt32(DepthTextBox.Text),
                                                           System.Convert.ToDouble(CpercentTextBox.Text),
                                                           System.Convert.ToDouble(PpercentTextBox.Text),
                                                           System.Convert.ToUInt32(MaxVerticesTextBox.Text),
                                                           System.Convert.ToDouble(SkinWidthTextBox.Text));
                    break;
            }

            PerformConvexDecomposition(CurrentMesh, type);

            // SVN
            var absFile = CSUtility.Support.IFileManager.Instance.Root + meshName + CSUtility.Support.IFileConfig.SimpleMeshExtension;

            if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:碰撞模型{meshName} {CSUtility.Support.IFileManager.Instance.Root + meshName + CSUtility.Support.IFileConfig.SimpleMeshExtension}通过版本管理上传失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                        {
                            if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:碰撞模型{meshName} {CSUtility.Support.IFileManager.Instance.Root + meshName + CSUtility.Support.IFileConfig.SimpleMeshExtension}通过版本管理上传失败!");
                            }
                        }, CSUtility.Support.IFileManager.Instance.Root + meshName + CSUtility.Support.IFileConfig.SimpleMeshExtension, $"AutoCommit 修改碰撞模型{meshName}");
                    }
                }, CSUtility.Support.IFileManager.Instance.Root + meshName + CSUtility.Support.IFileConfig.SimpleMeshExtension);
            }
        }

        private void PerformConvexDecomposition(CCore.Mesh.Mesh mesh, CCore.Mesh.Mesh.enSimpleMeshType type)
        {
            try
            {
                switch (type)
                {
                    case CCore.Mesh.Mesh.enSimpleMeshType.Box:
                        {
                            for (int i = 0; i < mesh.MeshParts.Count; ++i)
                            {
                                // 生成简化模型
                                mesh.MeshParts[i].CreateSimplifyMesh_ByType(mConvexDecomposition, CCore.Mesh.Mesh.enSimpleMeshType.Box);
                            }
                        }
                        break;

                    case CCore.Mesh.Mesh.enSimpleMeshType.Sphere:
                        {
                            for (int i = 0; i < mesh.MeshParts.Count; ++i)
                            {
                                // 生成简化模型
                                mesh.MeshParts[i].CreateSimplifyMesh_ByType(mConvexDecomposition, CCore.Mesh.Mesh.enSimpleMeshType.Sphere);
                            }
                        }
                        break;

                    case CCore.Mesh.Mesh.enSimpleMeshType.Cylinder:
                        {
                            for (int i = 0; i < mesh.MeshParts.Count; ++i)
                            {
                                mesh.MeshParts[i].CreateSimplifyMesh_ByType(mConvexDecomposition, CCore.Mesh.Mesh.enSimpleMeshType.Cylinder);
                            }
                        }
                        break;

                    case CCore.Mesh.Mesh.enSimpleMeshType.Param:
                        {
                            for (int i = 0; i < mesh.MeshParts.Count; ++i)
                            {
                                mesh.MeshParts[i].CreateSimplifyMesh_ByType(mConvexDecomposition, CCore.Mesh.Mesh.enSimpleMeshType.Param);
                            }
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                EditorCommon.MessageBox.Show(ex.ToString());
            }
            finally
            {
                mesh.SaveSimplifyMesh();
            }
        }

        private void Button_Box_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsBatch)
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(CurrentSourceDir))
                {
                    int index = file.LastIndexOf('.');
                    if (file.Substring(index) != CSUtility.Support.IFileConfig.MeshSourceExtension)
                        continue;

                    var relFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
                    GenerateSimplateMesh(relFile.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Box, false);
                }
            }
            else
            {
                GenerateSimplateMesh(CurrentSourceInfo.RelativeResourceFileName.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Box, true);

                if (SelectedSourceControls == null)
                    return;

                foreach (var ctrl in SelectedSourceControls)
                {
                    //var mesh = new CCore.Mesh.Mesh();
                    //var meshInit = new CCore.Mesh.MeshInit();
                    //var meshInitPart = new CCore.Mesh.MeshInitPart();
                    //meshInitPart.MeshName = ctrl.RelativePath.Replace("/", "\\");
                    //meshInit.MeshInitParts.Add(meshInitPart);
                    //mesh.Initialize(meshInit, null);

                    //if (!mesh.IsSimplifyMeshCreateByEditor)
                    //{
                    //    if (EditorCommon.MessageBox.Show("模型(" + meshInitPart.MeshName + ")已有的简化模型非编辑器生成，是否覆盖？", "警告", MessageBoxButton.OKCancel) != System.Windows.MessageBoxResult.OK)
                    //    {
                    //        return;
                    //    }
                    //}

                    //mesh.ClearSimplifyMesh();
                    //mesh.performConvexDecomposition(CCore.Mesh.Mesh.enSimpleMeshType.Box);
                    GenerateSimplateMesh(ctrl.RelativeResourceFileName.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Box, true);
                }

                //if (SelectedSourceControls.Count > 0)
                //{
                //    var d3dCtrl = Program.GetD3DControl();
                //    if (d3dCtrl != null)
                //    {
                //        CurrentMesh = d3dCtrl.SetMesh(SelectedSourceControls[SelectedSourceControls.Count - 1].RelativePath);
                //        CurrentMesh.EditorShow = ShowSimMesh;
                //    }
                //}
            }
        }

        private void Button_Sphere_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsBatch)
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(CurrentSourceDir))
                {
                    int index = file.LastIndexOf('.');
                    if (file.Substring(index) != CSUtility.Support.IFileConfig.MeshSourceExtension)
                        continue;

                    var relFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
                    GenerateSimplateMesh(relFile.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Sphere, false);
                }
            }
            else
            {
                GenerateSimplateMesh(CurrentSourceInfo.RelativeResourceFileName.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Sphere, true);

                if (SelectedSourceControls == null)
                    return;

                foreach (var ctrl in SelectedSourceControls)
                {
                    //var mesh = new CCore.Mesh.Mesh();
                    //var meshInit = new CCore.Mesh.MeshInit();
                    //var meshInitPart = new CCore.Mesh.MeshInitPart();
                    //meshInitPart.MeshName = ctrl.RelativePath.Replace("/", "\\");
                    //meshInit.MeshInitParts.Add(meshInitPart);
                    //mesh.Initialize(meshInit, null);

                    //if (!mesh.IsSimplifyMeshCreateByEditor)
                    //{
                    //    if (EditorCommon.MessageBox.Show("模型(" + meshInitPart.MeshName + ")已有的简化模型非编辑器生成，是否覆盖？", "警告", MessageBoxButton.OKCancel) != System.Windows.MessageBoxResult.OK)
                    //    {
                    //        return;
                    //    }
                    //}

                    //mesh.ClearSimplifyMesh();
                    //mesh.performConvexDecomposition(CCore.Mesh.Mesh.enSimpleMeshType.Sphere);
                    GenerateSimplateMesh(ctrl.RelativeResourceFileName.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Sphere, true);
                }
                
                //if (SelectedSourceControls.Count > 0)
                //{
                //    var d3dCtrl = Program.GetD3DControl();
                //    if (d3dCtrl != null)
                //    {
                //        CurrentMesh = d3dCtrl.SetMesh(SelectedSourceControls[SelectedSourceControls.Count - 1].RelativePath);
                //        CurrentMesh.EditorShow = ShowSimMesh;
                //    }
                //}
            }
        }

        private void Button_Cylinder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsBatch)
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(CurrentSourceDir))
                {
                    int index = file.LastIndexOf('.');
                    if (file.Substring(index) != CSUtility.Support.IFileConfig.MeshSourceExtension)
                        continue;

                    var relFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
                    GenerateSimplateMesh(relFile.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Cylinder, false);
                }
            }
            else
            {
                GenerateSimplateMesh(CurrentSourceInfo.RelativeResourceFileName.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Cylinder, true);

                if (SelectedSourceControls == null)
                    return;

                foreach (var ctrl in SelectedSourceControls)
                {
                    //var mesh = new CCore.Mesh.Mesh();
                    //var meshInit = new CCore.Mesh.MeshInit();
                    //var meshInitPart = new CCore.Mesh.MeshInitPart();
                    //meshInitPart.MeshName = ctrl.RelativePath.Replace("/", "\\");
                    //meshInit.MeshInitParts.Add(meshInitPart);
                    //mesh.Initialize(meshInit, null);

                    //if (!mesh.IsSimplifyMeshCreateByEditor)
                    //{
                    //    if (EditorCommon.MessageBox.Show("模型(" + meshInitPart.MeshName + ")已有的简化模型非编辑器生成，是否覆盖？", "警告", MessageBoxButton.OKCancel) != System.Windows.MessageBoxResult.OK)
                    //    {
                    //        return;
                    //    }
                    //}

                    //mesh.ClearSimplifyMesh();
                    //mesh.performConvexDecomposition(CCore.Mesh.Mesh.enSimpleMeshType.Cylinder);
                    GenerateSimplateMesh(ctrl.RelativeResourceFileName.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Cylinder, true);
                }

                //if (SelectedSourceControls.Count > 0)
                //{
                //    var d3dCtrl = Program.GetD3DControl();
                //    if (d3dCtrl != null)
                //    {
                //        CurrentMesh = d3dCtrl.SetMesh(SelectedSourceControls[SelectedSourceControls.Count - 1].RelativePath);
                //        CurrentMesh.EditorShow = ShowSimMesh;
                //    }
                //}
            }
        }

        private void Button_Param_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsBatch)
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(CurrentSourceDir))
                {
                    int index = file.LastIndexOf('.');
                    if (file.Substring(index) != CSUtility.Support.IFileConfig.MeshSourceExtension)
                        continue;

                    var relFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
                    GenerateSimplateMesh(relFile.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Param, false);
                }
            }
            else
            {
                GenerateSimplateMesh(CurrentSourceInfo.RelativeResourceFileName.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Param, true);

                if (SelectedSourceControls == null)
                    return;

                foreach (var ctrl in SelectedSourceControls)
                {
                    //var mesh = new CCore.Mesh.Mesh();
                    //var meshInit = new CCore.Mesh.MeshInit();
                    //var meshInitPart = new CCore.Mesh.MeshInitPart();
                    //meshInitPart.MeshName = ctrl.RelativePath.Replace("/", "\\");
                    //meshInit.MeshInitParts.Add(meshInitPart);
                    //mesh.Initialize(meshInit, null);

                    //if (!mesh.IsSimplifyMeshCreateByEditor)
                    //{
                    //    if (EditorCommon.MessageBox.Show("模型(" + meshInitPart.MeshName + ")已有的简化模型非编辑器生成，是否覆盖？", "警告", MessageBoxButton.OKCancel) != System.Windows.MessageBoxResult.OK)
                    //    {
                    //        return;
                    //    }
                    //}

                    //mesh.ClearSimplifyMesh();
                    //mesh.ConvexDecomposition.mDepth = System.Convert.ToUInt32(DepthTextBox.Text);
                    //mesh.ConvexDecomposition.mCpercent = System.Convert.ToDouble(CpercentTextBox.Text);
                    //mesh.ConvexDecomposition.mPpercent = System.Convert.ToDouble(PpercentTextBox.Text);
                    //mesh.ConvexDecomposition.mMaxVertices = System.Convert.ToUInt32(MaxVerticesTextBox.Text);
                    //mesh.ConvexDecomposition.mSkinWidth = System.Convert.ToDouble(SkinWidthTextBox.Text);

                    //mesh.performConvexDecomposition(CCore.Mesh.Mesh.enSimpleMeshType.Param);
                    GenerateSimplateMesh(ctrl.RelativeResourceFileName.Replace("/", "\\"), CCore.Mesh.Mesh.enSimpleMeshType.Param, true);
                }

                //if (SelectedSourceControls.Count > 0)
                //{
                //    var d3dCtrl = Program.GetD3DControl();
                //    if (d3dCtrl != null)
                //    {
                //        CurrentMesh = d3dCtrl.SetMesh(SelectedSourceControls[SelectedSourceControls.Count - 1].RelativePath);
                //        CurrentMesh.EditorShow = ShowSimMesh;

                //        //ShowSimplateMesh = true;
                //        //ShowMainMesh = true;
                //    }
                //}
            }
        }

        private void Button_Clear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			try
			{
                if (IsBatch)
                {

                }
                else
                {
                    var simMeshPath = CurrentSourceInfo.RelativeResourceFileName.Replace("/", "\\") + CSUtility.Support.IFileConfig.SimpleMeshExtension;

                    if (System.IO.File.Exists(simMeshPath))
                        System.IO.File.Delete(simMeshPath);

                    if (CurrentMesh != null)
                        CurrentMesh.ClearSimplifyMesh();


                    if (SelectedSourceControls == null)
                        return;
                    foreach (var ctrl in SelectedSourceControls)
                    {
                        simMeshPath = ctrl.AbsResourceFileName + CSUtility.Support.IFileConfig.SimpleMeshExtension;

                        if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                        {
                            EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                            {
                                if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                {
                                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:碰撞模型{ctrl.Name} {simMeshPath}通过版本管理删除失败!");
                                }
                                else
                                {
                                    EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                                    {
                                        if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                        {
                                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"资源浏览器:碰撞模型{ctrl.Name} {simMeshPath}通过版本管理删除失败!");
                                        }
                                    }, simMeshPath, $"AutoCommit 删除{ctrl.ResourceTypeName}{Name}碰撞模型");
                                }
                            }, simMeshPath);
                        }

                        if (System.IO.File.Exists(simMeshPath))
                            System.IO.File.Delete(simMeshPath);
                        //ctrl.RelativePath
                    }

                    //var d3dCtrl = Program.GetD3DControl();
                    //if (d3dCtrl != null && d3dCtrl.ShowedVisual != null)
                    //{
                    //    ((CCore.Mesh.Mesh)d3dCtrl.ShowedVisual).ClearSimplifyMesh();
                    //}

                    if (CurrentMesh != null)
                        CurrentMesh.ClearSimplifyMesh();

                    // todo: 清理内存中的简化模型
                }
			}
            catch (System.Exception ex)
			{
                EditorCommon.MessageBox.Show("SimpleMeshConvertControl.Button_Clear_Click exception\r\n" + ex.Message);
			}      
            // 删除简化模型

        }
    }
}
