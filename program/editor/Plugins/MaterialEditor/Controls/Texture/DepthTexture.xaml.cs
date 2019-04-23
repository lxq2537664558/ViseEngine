using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls
{
    /// <summary>
    /// DepthTexture.xaml 的交互逻辑
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("参数.深度缓存", "访问深度缓存数据")]
    public partial class DepthTextureControl : BaseNodeControl_ShaderVar
    {
        public DepthTextureControl(Canvas paraentCanvas, string strParam)
            : base(paraentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);

            AddLinkObject(enLinkType.Texture, TextureLink, enBezierType.Right, enLinkOpType.Start, TextureLink.BackBrush, true);
            AddLinkObject(enLinkType.Float2, UVLink_2D, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(enLinkType.Float4, Tex2DLink, enBezierType.Right, enLinkOpType.Start, Tex2DLink.BackBrush, true);

        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("MipFilter", MipFilterComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("MinFilter", MinFilterComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("MagFilter", MagFilterComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("AddressU", AddressUComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("AddressV", AddressVComboBox.SelectedIndex.ToString());
            xmlNode.AddAttrib("SRGBTexture", SRGBTextureComboBox.SelectedIndex.ToString());
            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            var valueAttrib = xmlNode.FindAttrib("MipFilter");
            if (valueAttrib != null)
                MipFilterComboBox.SelectedIndex = System.Convert.ToInt32(valueAttrib.Value);
            valueAttrib = xmlNode.FindAttrib("MinFilter");
            if (valueAttrib != null)
                MinFilterComboBox.SelectedIndex = System.Convert.ToInt32(valueAttrib.Value);
            valueAttrib = xmlNode.FindAttrib("MagFilter");
            if (valueAttrib != null)
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
            base.Load(xmlNode, deltaX, deltaY);
        }

        public override string GetValueDefine()
        {
            string retStr = "";

            retStr += "sampler2D " + GetTextureSampName() + " = sampler_state\r\n";
            retStr += "{\r\n";
            retStr += "\tTexture = <" + "g_PreFrameDepth" + ">;\r\n";
            retStr += "\tMipFilter = " + ((ComboBoxItem)MipFilterComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tMinFilter = " + ((ComboBoxItem)MinFilterComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tMagFilter = " + ((ComboBoxItem)MagFilterComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tAddressU = " + ((ComboBoxItem)AddressUComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tAddressV = " + ((ComboBoxItem)AddressVComboBox.SelectedItem).Content + ";\r\n";
            retStr += "\tSRGBTexture = " + ((ComboBoxItem)SRGBTextureComboBox.SelectedItem).Content + ";\r\n";
            retStr += "};\r\n\r\n";

            return retStr;
        }

        public string GetTextureSampName()
        {
            return "Samp_DepthTexture";
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == TextureLink)
                return "sampler2D";
            else if (element == Tex2DLink)
                return "float4";

            return base.GCode_GetValueType(element);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (element == TextureLink)
                return GetTextureSampName();
            else if (element == Tex2DLink)
            {
                string uvName = "pssem.mDiffuseUV.xy";
                var uvLinkOI = GetLinkObjInfo(UVLink_2D);
                if (uvLinkOI.HasLink)
                {
                    uvName = uvLinkOI.GetLinkObject(0, true).GCode_GetValueName(uvLinkOI.GetLinkElement(0, true));
                }

                return "vise_tex2D(" + GetTextureSampName() + "," + uvName + ")";
            }

            return base.GCode_GetValueName(element);
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(UVLink_2D);
            if (linkOI.HasLink)
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IsDirty = true;
        }
    }
}
