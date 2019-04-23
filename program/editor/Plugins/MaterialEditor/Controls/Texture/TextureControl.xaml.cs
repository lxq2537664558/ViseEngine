using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CodeGenerateSystem.Base;
using System.Windows.Documents;
using CCore.Material;

namespace MaterialEditor.Controls
{
    /// <summary>
    /// TextureControl.xaml 的交互逻辑
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("参数.贴图(Texture)", "贴图")]
    public partial class TextureControl : BaseNodeControl_ShaderVar
    {
        string mTexturePath;
        public string TexturePath
        {
            get { return mTexturePath; }
            set
            {
                mTexturePath = value;

                if (!string.IsNullOrEmpty(mTexturePath))
                {
                    string strAbsPath = CSUtility.Support.IFileManager.Instance.Root + mTexturePath;
                    strAbsPath = strAbsPath.Replace("/\\", "\\");
                    Image_Texture.Source = EditorCommon.ImageInit.GetImage(strAbsPath);

                    TextureBlock_TextureFileName.Text = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(mTexturePath);
                }
                else
                    TextureBlock_TextureFileName.Text = "";

                ShaderVarInfo.VarValue = mTexturePath;

                IsDirty = true;
            }
        }

        string mTextureName = "";
        public string TextureName
        {
            get { return mTextureName; }
            set
            {
                if (mTextureName == value)
                    return;

                var oldValue = mTextureName;
                mTextureName = value;

                if (ShaderVarInfo != null)
                {
                    if (ShaderVarInfo.Rename(GetTextureName()) == false)
                    {
                        EditorCommon.MessageBox.Show("名称" + GetTextureName() + "已经被使用，请换其他名称");
                        mTextureName = oldValue;
                    }
                }

                IsDirty = true;

                OnPropertyChanged("TextureName");
            }
        }
        
        public TextureControl(Canvas paraentCanvas, string strParam)
            : base(paraentCanvas, strParam)
        {
            InitializeComponent();

            mIsGeneric = true;
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(Image_Texture);

            ShaderVarInfo.EditorType = "Texture";
            //ShaderVarInfo.VarName = GetTextureName();

            TexturePath = CSUtility.Support.IFileConfig.DefaultTextureFile;

            SetDragObject(TitleLabel);

            AddLinkObject(enLinkType.Texture, TextureLink, enBezierType.Right, enLinkOpType.Start, TextureLink.BackBrush, true);
            AddLinkObject(enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4, UVLink_2D, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(enLinkType.Float4, RGBALink, enBezierType.Right, enLinkOpType.Start, RGBALink.BackBrush, true);
            AddLinkObject(enLinkType.Float3, RGBLink, enBezierType.Right, enLinkOpType.Start, RGBLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, RLink, enBezierType.Right, enLinkOpType.Start, RLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, GLink, enBezierType.Right, enLinkOpType.Start, GLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, BLink, enBezierType.Right, enLinkOpType.Start, BLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, ALink, enBezierType.Right, enLinkOpType.Start, ALink.BackBrush, true);
            AddLinkObject(enLinkType.Float3 | enLinkType.Float4, UVLink_3D, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(enLinkType.Float4, Tex3DLink, enBezierType.Right, enLinkOpType.Start, Tex3DLink.BackBrush, true);

            InitializeShaderVarInfo();
        }

        protected override void InitializeShaderVarInfo()
        {
            ShaderVarInfo.VarName = GetTextureName();
            ShaderVarInfo.VarType = "texture";
            ShaderVarInfo.VarValue = TexturePath;
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("TexPath", TexturePath);
            xmlNode.AddAttrib("MipFilter", MipFilterComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("MinFilter", MinFilterComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("MagFilter", MagFilterComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("AddressU", AddressUComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("AddressV", AddressVComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("SRGBTexture", SRGBTextureComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("TexName", TextureName);
            base.Save(xmlNode, newGuid,holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            var valueAttrib = xmlNode.FindAttrib("TexPath");
            if (valueAttrib != null)
                TexturePath = valueAttrib.Value;
            valueAttrib = xmlNode.FindAttrib("MipFilter");
            if(valueAttrib != null)
                MipFilterComboBox.SelectedIndex = System.Convert.ToInt32(valueAttrib.Value);
            valueAttrib = xmlNode.FindAttrib("MinFilter");
            if(valueAttrib != null)
                MinFilterComboBox.SelectedIndex = System.Convert.ToInt32(valueAttrib.Value);
            valueAttrib = xmlNode.FindAttrib("MagFilter");
            if(valueAttrib != null)
                MagFilterComboBox.SelectedIndex = System.Convert.ToInt32(valueAttrib.Value);
            valueAttrib = xmlNode.FindAttrib("AddressU");
            if (valueAttrib != null)
                AddressUComboBox.SelectedIndex = System.Convert.ToInt32(valueAttrib.Value);
            valueAttrib = xmlNode.FindAttrib("AddressV");
            if (valueAttrib != null)
                AddressVComboBox.SelectedIndex = System.Convert.ToInt32(valueAttrib.Value);
            valueAttrib = xmlNode.FindAttrib("SRGBTexture");
            if (valueAttrib != null)
                SRGBTextureComboBox.SelectedIndex = System.Convert.ToInt32(valueAttrib.Value);
            valueAttrib = xmlNode.FindAttrib("TexName");
            if (valueAttrib != null)
            {
                TextureNameTextBox.Text = valueAttrib.Value;
                mTextureName = valueAttrib.Value;
            }
            base.Load(xmlNode, deltaX, deltaY);

            InitializeShaderVarInfo();
        }

        private void MenuItem_LoadTexture_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var strFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(ofd.FileName);
                TexturePath = strFileName;

                // 向连接到本节点的其他节点发送贴图设置的信息以便其他节点更新显示贴图
                var linkOI = GetLinkObjInfo(TextureLink);
                var linkObj = linkOI.GetLinkObject(0, false);
                if (linkObj is Texture.Tex2D)
                {
                    ((Texture.Tex2D)linkObj).TexturePath = TexturePath;
                }
            }
        }

        /*/////////////////////////////////////////////////////////////////////////
        D3DTEXF_NONE            = 0,    // filtering disabled (valid for mip filter only)
        D3DTEXF_POINT           = 1,    // nearest
        D3DTEXF_LINEAR          = 2,    // linear interpolation
        D3DTEXF_ANISOTROPIC     = 3,    // anisotropic
        D3DTEXF_PYRAMIDALQUAD   = 6,    // 4-sample tent
        D3DTEXF_GAUSSIANQUAD    = 7,    // 4-sample gaussian
        /////////////////////////////////////////////////////////////////////////*/

        public override string GetValueDefine()
        {
            string retStr = "";

            retStr += "texture " + GetTextureName() + ";\r\n";
            retStr += "sampler2D " + GetTextureSampName() + " = sampler_state\r\n";
            retStr += "{\r\n";
            retStr += "\tTexture = <" + GetTextureName() + ">;\r\n";
            retStr += "\tMipFilter = " + ((ComboBoxItem)MipFilterComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tMinFilter = " + ((ComboBoxItem)MinFilterComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tMagFilter = " + ((ComboBoxItem)MagFilterComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tAddressU = " + ((ComboBoxItem)AddressUComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tAddressV = " + ((ComboBoxItem)AddressVComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tSRGBTexture = " + ((ComboBoxItem)SRGBTextureComboBox.SelectedItem).Content + ";\r\n";           
            retStr += "};\r\n\r\n";

            return retStr;
        }

        public string GetTextureName()
        {
            string strValueName;
            if (String.IsNullOrEmpty(TextureName))
            {
                strValueName = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + CodeGenerateSystem.Program.GetValuedGUIDString(Id);
            }
            else
                strValueName = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + TextureName;

            return strValueName;
        }

        public string GetTextureSampName()
        {
            return "Samp_" + GetTextureName();
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == TextureLink)
                return "sampler2D";
            else if (element == RGBALink)
                return "float4";
            else if (element == RGBLink)
                return "float3";
            else if (element == RLink)
                return "float1";
            else if (element == GLink)
                return "float1";
            else if (element == BLink)
                return "float1";
            else if (element == ALink)
                return "float1";
            else if (element == Tex3DLink)
                return "float4";

            return base.GCode_GetValueType(element);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if(element == TextureLink)
                return GetTextureSampName();
            else if (element == RGBALink)
            {
                return GetTextureName() + "_2D";
            }
            else if(element == RGBLink)
            {
                return GetTextureName() + "_2D.xyz";
            }
            else if(element == RLink)
            {
                return GetTextureName() + "_2D.x";
            }
            else if (element == GLink)
            {
                return GetTextureName() + "_2D.y";
            }
            else if (element == BLink)
            {
                return GetTextureName() + "_2D.z";
            }
            else if (element == ALink)
            {
                return GetTextureName() + "_2D.w";
            }
            else if (element == Tex3DLink)
            {
                var uvLinkOI = GetLinkObjInfo(UVLink_3D);
                if (uvLinkOI.HasLink)
                {
                    string uvName = uvLinkOI.GetLinkObject(0, true).GCode_GetValueName(uvLinkOI.GetLinkElement(0, true)) + ".xyz";

                    return "tex3D(" + GetTextureSampName() + "," + uvName + ")";
                }
            }

            return base.GCode_GetValueName(element);
        }
        
        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            var strValueIdt = "float4 " + GetTextureName() + "_2D = float4(0,0,0,0);\r\n";
            if (!strDefinitionSegment.Contains(strValueIdt))
                strDefinitionSegment += "    " + strValueIdt;

            var strTab = GCode_GetTabString(nLayer);

            string uvName = "pssem.mDiffuseUV.xy";
            var linkOI = GetLinkObjInfo(UVLink_2D);
            if (linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
                uvName = linkOI.GetLinkObject(0, true).GCode_GetValueName(linkOI.GetLinkElement(0, true)) + ".xy";
            }
            var assignStr = strTab + GetTextureName() + "_2D = vise_tex2D(" + GetTextureSampName() + "," + uvName + ");\r\n";
            // 这里先不做判断，连线中有if的情况下会导致问题
            //if (!strSegment.Contains(assignStr))
            if (!Program.IsSegmentContainString(strSegment.Length - 1, strSegment, assignStr))
                strSegment += assignStr;

            linkOI = GetLinkObjInfo(UVLink_3D);
            if (linkOI.HasLink)
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
            
        }

        private void Button_SetTexturePath_Click(object sender, RoutedEventArgs e)
        {
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("Texture");
            if (data == null)
                return;

            if (data.Length <= 0)
                return;

            TexturePath = (string)data[0];
            ShaderVarInfo.VarValue = TexturePath;

            // 向连接到本节点的其他节点发送贴图设置的信息以便其他节点更新显示贴图
            var linkOI = GetLinkObjInfo(TextureLink);
            var linkObj = linkOI.GetLinkObject(0, false);
            if (linkObj is Texture.Tex2D)
            {
                ((Texture.Tex2D)linkObj).TexturePath = TexturePath;
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IsDirty = true;
        }

        private void Button_SearchTexture_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var textureFileName = CSUtility.Support.IFileManager.Instance.Root + TexturePath;
            if (!string.IsNullOrEmpty(TexturePath))
                EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", textureFileName });
        }

        private void TextureNameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            BindingExpression be = TextureNameTextBox.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }

        private void TextureNameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        TextureNameTextBox.Text = TextureName;
                    }
                    break;

                case Key.Enter:
                    {
                        BindingExpression be = TextureNameTextBox.GetBindingExpression(TextBox.TextProperty);
                        be.UpdateSource();
                    }
                    break;
            }
        }

        #region DragDrop

        EditorCommon.DragDrop.DropAdorner mDropAdorner;
        enum enDropResult
        {
            Denial_UnknowFormat,
            Denial_NoDragAbleObject,
            Allow,
        }
        // 是否允许拖放
        enDropResult AllowResourceItemDrop(System.Windows.DragEventArgs e)
        {
            var formats = e.Data.GetFormats();
            if (formats == null || formats.Length == 0)
                return enDropResult.Denial_UnknowFormat;

            var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
            if (datas == null)
                return enDropResult.Denial_NoDragAbleObject;

            bool containMeshSource = false;
            foreach (var data in datas)
            {
                var resInfo = data as ResourcesBrowser.ResourceInfo;
                if (resInfo.ResourceType == "Texture")
                {
                    containMeshSource = true;
                    break;
                }
            }

            if (!containMeshSource)
                return enDropResult.Denial_NoDragAbleObject;

            return enDropResult.Allow;
        }
        private void Image_Texture_DragEnter(object sender, DragEventArgs e)
        {
            if(EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                var element = sender as FrameworkElement;
                if (element == null)
                    return;

                e.Handled = true;
                mDropAdorner.IsAllowDrop = false;

                switch(AllowResourceItemDrop(e))
                {
                    case enDropResult.Allow:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "设置贴图资源";

                            mDropAdorner.IsAllowDrop = true;
                            var pos = e.GetPosition(element);
                            if (pos.X > 0 && pos.X < element.ActualWidth &&
                               pos.Y > 0 && pos.Y < element.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(element);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;

                    case enDropResult.Denial_NoDragAbleObject:
                    case enDropResult.Denial_UnknowFormat:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是贴图资源";

                            mDropAdorner.IsAllowDrop = false;
                            var pos = e.GetPosition(element);
                            if (pos.X > 0 && pos.X < element.ActualWidth &&
                               pos.Y > 0 && pos.Y < element.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(element);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;
                }
            }
        }

        private void Image_Texture_DragLeave(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer.Remove(mDropAdorner);
            }
        }

        private void Image_Texture_DragOver(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                if (AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void Image_Texture_Drop(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer.Remove(mDropAdorner);

                if (AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    var formats = e.Data.GetFormats();
                    var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                    foreach (var data in datas)
                    {
                        var resInfo = data as ResourcesBrowser.ResourceInfo;
                        if (resInfo == null)
                            continue;

                        TexturePath = resInfo.RelativeResourceFileName;
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
