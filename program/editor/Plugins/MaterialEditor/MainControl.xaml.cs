using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace MaterialEditor
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "MaterialEditor")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/MaterialEditor")]
    [Guid("8DB95031-4A06-418D-A123-10A622728206")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string PluginName
        {
            get { return "材质模板编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "材质模板编辑器",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            D3DViewer.Activated = true;
            return true;
        }
        public bool OnDeactive()
        {
            D3DViewer.Activated = false;
            return true;
        }

        public void Tick()
        {
            if (D3DViewer != null)
                D3DViewer.Tick();
        }

        protected bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
                mCurrentResourceInfo.IsDirty = mIsDirty;
                MaterialFileInfo.IsDirty = mIsDirty;
                //if (mIsDirty == false)
                //{
                //    foreach (var ctrl in NodesControl.CtrlNodeList)
                //    {
                //        ctrl.IsDirty = false;
                //    }
                //}

                //OnContainerDirtyChanged(mIsDirty);

                //OnDirtyChanged?.Invoke(mIsDirty);
            }
        }

        MaterialResourceInfo mCurrentResourceInfo;

        public MaterialEditor.MaterialFileInfo MaterialFileInfo
        {
            get;
            set;
        }
        Controls.MaterialControl mSetValueMaterialControl;
        //Controls.MaterialControl mGetValueMaterialControl;

        public void SetObjectToEdit(object[] obj)
        {
            if (obj == null)
                return;

            if (obj.Length == 0)
                return;
            
            try
            {
                if (MaterialFileInfo != null && MaterialFileInfo.MaterialId != Guid.Empty)
                {
                    if(MaterialFileInfo.IsDirty)
                    {
                        switch(EditorCommon.MessageBox.Show("当前文件还未保存，是否保存？", EditorCommon.MessageBox.enMessageBoxButton.YesNoCancel))
                        {
                            case EditorCommon.MessageBox.enMessageBoxResult.Yes:
                                {
                                    CodeGenerator.GenerateCode(MaterialFileInfo, NodesControl, mSetValueMaterialControl);
                                    var strFileName = GetLinkFileFromMaterialFile(mCurrentResourceInfo);
                                    SaveMaterialLink(strFileName + Program.LinkTemplateExtend, false);
                                }
                                break;

                            case EditorCommon.MessageBox.enMessageBoxResult.No:
                                MaterialFileInfo.IsDirty = false;
                                break;

                            case EditorCommon.MessageBox.enMessageBoxResult.Cancel:
                                return;
                        }
                    }
                }

                ////////////var oldResourceInfo = mCurrentResourceInfo;
                mCurrentResourceInfo = obj[0] as MaterialResourceInfo;//MaterialEditor.MaterialFileInfo;
                if (mCurrentResourceInfo == null)
                    return;                

                var matFile = mCurrentResourceInfo.MatInfo;
                if (matFile == null)
                    return;

                MaterialShowName = matFile.MaterialName;

                if (MaterialFileInfo != null && MaterialFileInfo.MaterialId == matFile.MaterialId)
                    return;

                ////////////// 切换选择时将连线暂存，以便真正存储的时候能够得到连线
                ////////////if (MaterialFileInfo != null && MaterialFileInfo.MaterialId != Guid.Empty)
                ////////////{
                ////////////    CodeGenerator.GenerateCode(MaterialFileInfo, NodesControl, mSetValueMaterialControl);

                ////////////    var strFileName = GetLinkFileFromMaterialFile(oldResourceInfo);
                ////////////    SaveMaterialLink(strFileName + Program.LinkTemplateExtend, false);

                ////////////    if (!m_materialLinkTempFileList.Contains(strFileName))
                ////////////        m_materialLinkTempFileList.Add(strFileName);
                ////////////}

                MaterialFileInfo = matFile;

                OnLoad(matFile);                
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
            }
        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }
        
        //[ImportMany(typeof(EditorCommon.PluginAssist.IEditorPlugin), AllowRecomposition = true)]
        //IEnumerable<Lazy<EditorCommon.PluginAssist.IEditorPlugin, EditorCommon.PluginAssist.IEditorPluginData>> Plugins = null;

        //[Import("D3DShow", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //EditorCommon.PluginAssist.IEditorPlugin mD3DShowPlugin = null;

        ////////////List<string> m_materialLinkTempFileList = new List<string>();

        string mMaterialShowName = "";
        public string MaterialShowName
        {
            get { return mMaterialShowName; }
            set
            {
                mMaterialShowName = value;
                OnPropertyChanged("MaterialShowName");
            }
        }

        bool mPreView = false;
        public bool PreView
        {
            get { return mPreView; }
            set
            {
                mPreView = value;

                //if (value == true)
                //{
                //    if (D3DEvrCtrl != null)
                //        D3DEvrCtrl.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    if (D3DEvrCtrl != null)
                //        D3DEvrCtrl.Visibility = Visibility.Collapsed;
                //}
            }
        }

        private void Button_RefreshPreview_Click(object sender, RoutedEventArgs e)
        {
            RefreshMeshMaterial();
        }

        // 临时使用的材质信息
        private MaterialEditor.MaterialFileInfo mTempMaterialFileInfo;
        //private MaterialEditor.MaterialTechniqueInfo TempMaterialTechniqueInfo;

        public MainControl()
        {
            InitializeComponent();

            Grid_Main.Children.Remove(ToolBar_Preview);
            D3DViewer.AddToolBar(ToolBar_Preview);
            D3DViewer.ShowSphere = true;
            D3DViewer.OnShowMeshChanged += D3DViewer_OnShowMeshChanged;

            //mGetValueMaterialControl = (Controls.MaterialControl)NodesControl.AddNodeControl(typeof(Controls.MaterialControl), "true", 0, 0);
            mSetValueMaterialControl = (Controls.MaterialControl)NodesControl.AddNodeControl(typeof(Controls.MaterialControl), "false", 800, 0);

            mTempMaterialFileInfo = new MaterialEditor.MaterialFileInfo(MaterialEditor.Program.TemplateMaterialId);
            //TempMaterialTechniqueInfo = new MaterialEditor.MaterialTechniqueInfo(TempMaterialFileInfo.MaterialId);
            //TempMaterialTechniqueInfo.Id = MaterialEditor.Program.TemplateTechniqueId;
            ////TempMaterialTechniqueInfo.mFileName = MaterialEditor.MaterialTechniqueInfo.TechniquesFolder + "/" + TempMaterialTechniqueInfo.Id.ToString() + CSUtility.Support.IFileConfig.MaterialTechniqueExtension;
            //TempMaterialFileInfo.AddTechnique(TempMaterialTechniqueInfo);

            airViewer.TargetNodesContainer = NodesControl;
            NodesList.NodesContainer = NodesControl;
            NodesControl.OnDirtyChanged += NodesControl_OnDirtyChanged;
            NodesControl.OnLinkControlSelected += NodesControl_OnLinkControlSelected;
            NodesControl.OnSelectNodeControl += NodesControl_OnSelectNodeControl;
            NodesControl.OnUnSelectNodes += NodesControl_OnUnSelectNodes;
            NodesControl.ErrorListCtrl = ErrorList;
            NodesControl.OnCodePreview = NodesControl_OnCodePreview;
            NodesControl.OnSave = NodesControl_OnSave;
            NodesControl.OnAddedNodeControl += NodesControl_OnAddedNodeControl;
            NodesControl.OnDeletedNodeControl += NodesControl_OnDeletedNodeControl;

            //NodesContainerCtrl.OnUpdateMaterialShow = new NodesContainerControl.Delegate_OnUpdateMaterialShow(OnUpdateMaterialShow);
            InitializeNodesList();

            var template = this.TryFindResource("MaterialShaderValue") as DataTemplate;
            WPG.Program.RegisterDataTemplate("MaterialShaderValue", template);
        }

        private void NodesControl_OnAddedNodeControl(CodeGenerateSystem.Base.BaseNodeControl node)
        {
            if (NodesControl.IsLoading == true)
                return;
            if (MaterialFileInfo == null)
                return;

            var varInfo = GetShaderVarInfoFromNode(node);
            if (varInfo != null)
                MaterialFileInfo.AddShaderValue(varInfo);
        }

        private void NodesControl_OnDeletedNodeControl(CodeGenerateSystem.Base.BaseNodeControl node)
        {
            if(node is Controls.BaseNodeControl_ShaderVar)
            {
                var svi = ((Controls.BaseNodeControl_ShaderVar)node).GetShaderVarInfo();
                if (svi != null)
                    MaterialFileInfo.RemoveShaderValue(svi);
            }
        }

        private void D3DViewer_OnShowMeshChanged()
        {
            UpdateMaterialShow(true);
        }

        private void NodesControl_OnSave(CodeGenerateSystem.Controls.NodesContainerControl ctrl)
        {
            if (MaterialFileInfo != null)
            {
                CodeGenerator.GenerateCode(MaterialFileInfo, NodesControl, mSetValueMaterialControl);
                Program.SaveMaterial(mCurrentResourceInfo);
                SaveMaterialLink(GetLinkFileFromMaterialFile(mCurrentResourceInfo), true);
            }
        }

        private void NodesControl_OnCodePreview(CodeGenerateSystem.Controls.NodesContainerControl ctrl)
        {
            NodesControl.CheckError();

            var tw = CodeGenerator.GenerateCode(mTempMaterialFileInfo, NodesControl, mSetValueMaterialControl);
            CodeViewControl.CodeString = tw.ToString();
        }

        private void NodesControl_OnUnSelectNodes(List<CodeGenerateSystem.Base.BaseNodeControl> nodes)
        {
            ProGrid.Instance = null;
        }

        private void NodesControl_OnSelectNodeControl(CodeGenerateSystem.Base.BaseNodeControl node)
        {
            if (node == null)
                ProGrid.Instance = null;
            else
                ProGrid.Instance = node.GetShowPropertyObject();
        }

        private void NodesControl_OnLinkControlSelected(CodeGenerateSystem.Base.LinkControl linkCtrl)
        {
        }

        #region NodesList
        private void InitializeNodesList()
        {
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mBloom", "float1,mBloom,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mCubeEnvUV", "float4,mCubeEnvUV,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mDepth", "float1,mDepth,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mDiffuseColor", "float4,mDiffuseColor,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mDiffuseUV", "float2,mDiffuseUV,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mDX9Fix_VIDTerrain", "float4,mDX9Fix_VIDTerrain,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mEmissiveColor", "float4,mEmissiveColor,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mEmissiveUV", "float4,mEmissiveUV,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mLocalPos", "float4,mLocalPos,,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mLocalNorm", "float4,mLocalNorm,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mLocalTangent", "float4,mLocalTangent,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mLoaclBinorm", "float4,mLoaclBinorm,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mLightMapUV", "float4,mLightMapUV,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mNormalUV", "float4,mNormalUV,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mProjPos", "float4,mProjPos,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mSpecularColor", "float4,mSpecularColor,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mSpecularUV", "float4,mSpecularUV,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mSpecularIntensity", "float1,mSpecularIntensity,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mSpecularPower", "float1,mSpecularPower,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mTerrainGradient", "float2,mTerrainGradient,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mViewPos", "float4,mViewPos,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mViewPixelNormal", "float4,mViewPixelNormal,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mViewVertexNormal", "float4,mViewVertexNormal,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mVertexColor0", "float4,mVertexColor0,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mWorldPos", "float4,mWorldPos,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mWorldNorm", "float4,mWorldNorm,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mWorldTangent", "float4,mWorldTangent,", "");
            NodesList.AddNodesFromType(typeof(Controls.Value.PixelMaterialData), "材质输入.mWorldBinorm", "float4,mWorldBinorm,", "");


            NodesList.AddNodesFromAssembly(this.GetType().Assembly);

            NodesList.AddNodesFromType(typeof(Controls.CommonValueControl), "参数.int", "int", "整形数据");
            NodesList.AddNodesFromType(typeof(Controls.CommonValueControl), "参数.float1", "float1", "一维浮点数");
            NodesList.AddNodesFromType(typeof(Controls.CommonValueControl), "参数.float2", "float2", "二维浮点数");
            NodesList.AddNodesFromType(typeof(Controls.CommonValueControl), "参数.float3", "float3", "三维浮点数");
            NodesList.AddNodesFromType(typeof(Controls.CommonValueControl), "参数.float4", "float4", "四维浮点数");
            NodesList.AddNodesFromType(typeof(Controls.Operation.Arithmetic), "运算.加(add ＋)", "＋", "加法运算");
            NodesList.AddNodesFromType(typeof(Controls.Operation.Arithmetic), "运算.减(sub －)", "－", "减法运算");
            NodesList.AddNodesFromType(typeof(Controls.Operation.Arithmetic), "运算.乘(mul ×)", "×", "乘法运算");
            NodesList.AddNodesFromType(typeof(Controls.Operation.Arithmetic), "运算.除(div ÷)", "÷", "除法运算");
            NodesList.AddNodesFromType(typeof(Controls.Operation.Arithmetic), "运算.dot", "dot", "向量点乘运算");
            NodesList.AddNodesFromType(typeof(Controls.Operation.Arithmetic), "运算.cross", "cross", "向量叉乘运算");

            GetAllFunctions();
            GetAllShaderAutoDatas();
        }

        private void GetAllFunctions()
        {
            string funcPath = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.MaterialFunctionDirectory;//"D:\\victory\\Release\\bin\\Shader\\Material\\Common";
            var files = System.IO.Directory.GetFiles(funcPath);
            foreach (var file in files)
            {
                if (file.Substring(file.LastIndexOf('.') + 1) != "function")
                    continue;

                var rd = new System.IO.StreamReader(file, CSUtility.Support.IFileManager.GetEncoding(file));
                var strTemp = rd.ReadLine();
                if (strTemp != "/*function")
                    continue;

                strTemp = "";

                var strFuncData = "";
                while (strTemp != "*/")
                {
                    strFuncData += strTemp;
                    strTemp = rd.ReadLine();
                }

                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(strFuncData);

                foreach (System.Xml.XmlElement element in xmlDoc.DocumentElement.ChildNodes)
                {
                    string funcName = element.GetAttribute("Name");
                    var description = element.GetAttribute("Description");
                    var path = element.GetAttribute("Path");
                    if (string.IsNullOrEmpty(path))
                        path = "函数." + funcName;

                    //var tx = new System.IO.StringWriter();
                    //System.Xml.XmlTextWriter wr = new System.Xml.XmlTextWriter(tx);
                    //element.WriteTo(wr);
                    //string strParam = tx.ToString();
                    var tempFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file, CSUtility.Support.IFileManager.Instance.Root + "bin\\");
                    NodesList.AddNodesFromType(typeof(Controls.Operation.Function), path, tempFile + "|" + element.OuterXml, description);
                }
            }
        }
        private void GetAllShaderAutoDatas()
        {
            foreach (var data in CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetShaderAutoDatas())
            {
                var splits = data.Split(',');
                NodesList.AddNodesFromType(typeof(Controls.ShaderAutoData), "参数." + splits[1], data, splits[2] + "(" + splits[0] + ")");
            }
        }
        #endregion

        private void OnLoad(MaterialEditor.MaterialFileInfo info)
        {
            if (info == null)
                return;

            if (MaterialFileInfo != null)
                MaterialFileInfo.OnMaterialFileDirtyChange -= OnMaterialFileDirtyChange;

            MaterialFileInfo = info;
            MaterialFileInfo.OnMaterialFileDirtyChange += OnMaterialFileDirtyChange;

            LoadMaterialLink(GetLinkFileFromMaterialFile(mCurrentResourceInfo));

            PropertyGrid_Material.Instance = MaterialFileInfo;
            PropertyGrid_DefTech.Instance = MaterialFileInfo.DefaultTechnique;
        }

        private void OnMaterialFileDirtyChange(MaterialEditor.MaterialFileInfo info)
        {
            PropertyGrid_DefTech.Instance = null;
            PropertyGrid_DefTech.Instance = MaterialFileInfo.DefaultTechnique;
        }


        void NodesControl_OnDirtyChanged(bool dirty)
        {
            if (mCurrentResourceInfo == null)
                return;

            IsDirty = dirty;

            if (dirty)
                MaterialShowName = MaterialFileInfo.MaterialName + "(未保存)";
            else
                MaterialShowName = MaterialFileInfo.MaterialName;
        }

        protected void UpdateMaterialShow(bool force)//String strTempFileName)
        {
            if (MaterialFileInfo == null)
                return;

            if (!PreView && !force)
                return;

            // todo: 先进行错误检查

            CodeGenerator.GenerateCode(MaterialFileInfo, NodesControl, mSetValueMaterialControl);

            mTempMaterialFileInfo.CopyFrom(MaterialFileInfo);
            //TempMaterialTechniqueInfo.CopyFrom(MaterialFileInfo.DefaultTechnique);//.GetTechnique(0));
            mTempMaterialFileInfo.Ver++;
            if (mTempMaterialFileInfo.Ver >= UInt32.MaxValue)
                mTempMaterialFileInfo.Ver = 0;
            mTempMaterialFileInfo.SaveMaterialFile(Program.TempMaterialFileName);

            // 读取临时材质以设置D3D窗口
            var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetMaterial(Program.TempMaterialFileName, true);
            CCore.Engine.Instance.Client.Graphics.MaterialMgr.RefreshEffect(mtl);

            if (mtl != null)
            {
                SetMaterialShow(0, 0, 0, mtl);
            }
        }

        public void SetMaterialShow(int actorIndex, int meshIndex, int materialIndex, CCore.Material.Material material)
        {
            if (D3DViewer != null)
            {
                D3DViewer.SetObjectToEdit(new object[] { new object[] { "Material", true },
                                                              new object[] { -1, -1, -1, material } });
            }
        }

        private void userControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        #region 存储读取

        public static string GetLinkFileFromMaterialFile(MaterialResourceInfo info)
        {
            return CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(info.AbsResourceFileName + Program.LinkFileExtend);
        }

        public void SaveMaterialLink(string strFileName, bool withSVN)
        {
            //if (string.IsNullOrEmpty(m_LinkFileName))
            //    return;

            //var saveFileName = m_LinkFileName;
            //if (bTemp)
            //    saveFileName += ".Temp";
            //string linkFileName = CSUtility.Support.IFileConfig.EditorSourceDirectory + "/Material/" + strFileName + Program.LinkFileExtend;
            //string strTemp = "";
            //if (bTemp)
            //{
            //    linkFileName = CSUtility.Support.IFileConfig.EditorSourceDirectory + "/Material/" + strFileName + "_Temp" + Program.LinkFileExtend;
            //    m_linkTempFiles.Add(linkFileName);
            //}
            //else
            //{

            //}
            // 创建文件夹
            var dir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(strFileName);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);

                if (withSVN)
                {
                    EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                    {
                        if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"材质{MaterialFileInfo.MaterialName}所在目录 {dir}使用版本控制添加失败!");
                        }
                    }, dir, $"AutoCommit 创建材质{MaterialFileInfo.MaterialName}所在目录", true);
                }
            }

            // 储存连线布局xml
            CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Nodes", null);

            xmlHolder.RootNode.AddAttrib("SetValueMatCtrlID", mSetValueMaterialControl.Id.ToString());
            //xmlHolder.RootNode.AddAttrib("GetValueMatCtrlID", mGetValueMaterialControl.Id.ToString());

            // 存储连线
            NodesControl.SaveXML(strFileName, xmlHolder);

            if (withSVN)
            {
                var fullFileName = CSUtility.Support.IFileManager.Instance.Root + strFileName;

                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"材质{MaterialFileInfo.MaterialName}连线 {dir}使用版本控制上传失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                        {
                            if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"材质{MaterialFileInfo.MaterialName}连线 {dir}使用版本控制上传失败!");
                            }
                        }, fullFileName, $"AutoCommit 修改材质{MaterialFileInfo.MaterialName}连线");
                    }
                }, fullFileName);
            }

            // 储存生成的材质xml
            //MaterialFileInfo.SaveMaterialFile(strFileName);
            //var tw = GenerateCode();
            //var writer = new StreamWriter("Shader\\Material\\" + m_materialFileName);
            //writer.Write(tw);
            //writer.Close();
        }

        protected string mLoadedFile = "";
        public string LoadedFile
        {
            get { return mLoadedFile; }
        }
        bool mIsLoading = false;
        public bool IsLoading
        {
            get { return mIsLoading; }
        }
        public void LoadMaterialLink(string strFileName)
        {
            mIsLoading = true;
            //MaterialFileInfo.LoadMaterialFile(strFileName);
            mLoadedFile = strFileName;

            //m_materialFileName = strFileName;
            //string linkFileName = CSUtility.Support.IFileConfig.EditorSourceDirectory + "/Material/" + strFileName + Program.LinkFileExtend;

            //m_LinkFileName = CSUtility.Support.IFileConfig.EditorSourceDirectory + "/Material/" + strFileName + Program.LinkFileExtend;
            //var loadFileName = m_LinkFileName; 
            //MaterialTechniqueInfoDic.Clear();

            //// 读文件头
            //var strFullFileName = CSUtility.Support.IFileManager.Instance.Root + "Shader/Material/" + strFileName;
            //var rd = new System.IO.StreamReader(strFullFileName);
            //string strFileHead = "";
            //rd.ReadLine();  // 跳过"/*Material"
            //string strTemp = rd.ReadLine();
            //while (strTemp != "*/")
            //{
            //    strFileHead += strTemp;
            //    strTemp = rd.ReadLine();
            //}

            //m_strMaterialCodeStringBlock = rd.ReadToEnd();

            //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            //xmlDoc.LoadXml(strFileHead);

            //m_strMatName = xmlDoc.DocumentElement.GetAttribute("Name");
            //m_strRequire = xmlDoc.DocumentElement.GetAttribute("Require");
            //m_strMain = xmlDoc.DocumentElement.GetAttribute("Main");
            //if (string.IsNullOrEmpty(m_strMain))
            //    m_strMain = "DoMaterial";

            //// 取得technique的信息
            //foreach (System.Xml.XmlElement cNode in xmlDoc.DocumentElement.ChildNodes)
            //{
            //    MaterialTechniqueInfo mtInfo = new MaterialTechniqueInfo();
            //    mtInfo.Name = cNode.GetAttribute("Name");
            //    mtInfo.AlphaRef = System.Convert.ToUInt64(cNode.GetAttribute("AlphaRef"));
            //    mtInfo.CullMode = (MaterialTechniqueInfo.enCullMode)System.Enum.Parse(typeof(MaterialTechniqueInfo.enCullMode), cNode.GetAttribute("CullMode"));
            //    mtInfo.ZDisable = System.Convert.ToInt32(cNode.GetAttribute("ZDisable")) > 0 ? true : false;
            //    mtInfo.ZWriteDisable = System.Convert.ToInt32(cNode.GetAttribute("ZWriteDisable")) > 0 ? true : false;
            //    mtInfo.WireFrame = System.Convert.ToInt32(cNode.GetAttribute("WireFrame")) > 0 ? true : false;
            //    mtInfo.AlphaType = (MaterialTechniqueInfo.enAlphaType)System.Enum.Parse(typeof(MaterialTechniqueInfo.enAlphaType), cNode.GetAttribute("AlphaType"));

            //    var shaderVarElements = cNode.GetElementsByTagName("ShaderVar");
            //    if (shaderVarElements.Count > 0)
            //    {
            //        foreach (System.Xml.XmlElement svNode in shaderVarElements[0].ChildNodes)
            //        {
            //            mtInfo.AddShaderValue(svNode.Name, svNode.GetAttribute("Type"), svNode.GetAttribute("Value"));
            //        }
            //    }

            //    MaterialTechniqueInfoDic.Add(mtInfo.Name, mtInfo);
            //}

            CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.LoadXML(strFileName);
            if (xmlHolder != null)
            {
                var guidSetValue = System.Guid.Parse(xmlHolder.RootNode.FindAttrib("SetValueMatCtrlID").Value);
                //var guidGetValue = System.Guid.Parse(xmlHolder.RootNode.FindAttrib("GetValueMatCtrlID").Value);

                //// 清除shaderVar, 已链接文件中存储的shaderVar为准
                //MaterialFileInfo.ClearShaderValues();

                NodesControl.LoadXML(strFileName, false);

                foreach (var node in NodesControl.CtrlNodeList)
                {
                    if (node.Id == guidSetValue)
                        mSetValueMaterialControl = node as MaterialEditor.Controls.MaterialControl;
                    //else if (node.Id == guidGetValue)
                    //    mGetValueMaterialControl = node as MaterialEditor.Controls.MaterialControl;

                    // 根据node设置一遍technique的shadervar
                    CCore.Material.MaterialShaderVarInfo varInfo = GetShaderVarInfoFromNode(node);
                    if (varInfo != null)
                        MaterialFileInfo.AddShaderValue(varInfo);

                    //if (node is Controls.TextureControl)
                    //{
                    //    var texCtrl = node as Controls.TextureControl;
                    //    foreach (var tech in MaterialTechniqueInfoDic.Values)
                    //    {
                    //        tech.AddShaderValue(node);
                    //    }
                    //}
                    //else if (node is Controls.CommonValueControl)
                    //{
                    //    var cvCtrl = node as Controls.CommonValueControl;
                    //    cvCtrl.OnIsGenericChanged = new Controls.CommonValueControl.Delegate_OnIsGenericChanged(OnCommonValueIsGenericChanged);
                    //    if (cvCtrl.IsGeneric)
                    //    {
                    //        foreach (var tech in MaterialTechniqueInfoDic.Values)
                    //        {
                    //            tech.AddShaderValue(cvCtrl);
                    //        }
                    //    }
                    //}
                }
            }
            else
            {
                foreach (var node in NodesControl.CtrlNodeList)
                {
                    node.Clear();
                }
                NodesControl.CtrlNodeList.Clear();

                //mGetValueMaterialControl = (Controls.MaterialControl)NodesControl.AddNodeControl(typeof(Controls.MaterialControl), "true", 0, 0);
                mSetValueMaterialControl = (Controls.MaterialControl)NodesControl.AddNodeControl(typeof(Controls.MaterialControl), "false", 800, 0);
            }

            //MaterialFileInfo.IsDirty = false;
            IsDirty = false;

            mIsLoading = false;
        }

        protected CCore.Material.MaterialShaderVarInfo GetShaderVarInfoFromNode(CodeGenerateSystem.Base.BaseNodeControl node)
        {
            CCore.Material.MaterialShaderVarInfo varInfo = null;

            if(node is Controls.BaseNodeControl_ShaderVar)
            {
                var ctrl = node as Controls.BaseNodeControl_ShaderVar;
                if (ctrl.OnIsGenericChanging == null)
                    ctrl.OnIsGenericChanging = new Controls.BaseNodeControl_ShaderVar.Delegate_OnIsGenericChanging(_OnIsGenericChanged);
                if (ctrl.OnShaderVarRenamed == null)
                    ctrl.OnShaderVarRenamed = new Controls.BaseNodeControl_ShaderVar.Delegate_OnShaderVarRenamed(_OnShaderValueRenamed);

                varInfo = ctrl.GetShaderVarInfo();

            }
            return varInfo;
        }

        #endregion

        private bool _OnShaderValueRenamed(Controls.BaseNodeControl_ShaderVar ctrl, string oldName, string newName)
        {
            if (MaterialFileInfo != null && ctrl != null)
                return MaterialFileInfo.ShaderValueRename(oldName, newName);
            return false;
        }

        private void _OnIsGenericChanged(Controls.BaseNodeControl_ShaderVar ctrl, bool oldValue, bool newValue)
        {
            if (oldValue == newValue)
                return;

            if (newValue)
            {
                //CCore.Material.MaterialShaderVarInfo info = new CCore.Material.MaterialShaderVarInfo()
                //{
                //    VarName = ctrl.GCode_GetValueName(null),
                //    VarType = ctrl.GCode_GetValueType(null),
                //    VarValue = ctrl.GetValueString()
                //};
                MaterialFileInfo.AddShaderValue(ctrl.GetShaderVarInfo(true));
            }
            else
            {
                MaterialFileInfo.RemoveShaderValue(ctrl.GetShaderVarInfo(true));
                //ctrl.ShaderVarInfo = null;
            }
        }

        // 刷新在D3D窗口显示的材质
        public void RefreshMeshMaterial()
        {
            if (MaterialFileInfo == null)
                return;

            // todo: 先进行错误检查
    
            UpdateMaterialShow(true);
        }

        private void Button_SaveSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentResourceInfo == null)
                return;

            var snapShotFile = mCurrentResourceInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt;

            this.Dispatcher.Invoke(new Action(() =>
            {                
                var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                D3DViewer.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);                
            }));

            mCurrentResourceInfo.Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);            
        }
    }
}
