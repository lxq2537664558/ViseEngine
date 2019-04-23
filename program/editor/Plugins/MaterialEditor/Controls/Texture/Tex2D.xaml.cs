using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls.Texture
{
    /// <summary>
    /// Tex2D.xaml 的交互逻辑 SM1,2,3,4
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("参数.Tex2D", "2D贴图的数据")]
    public partial class Tex2D : BaseNodeControl
    {
        string m_strTexturePath;
        public string TexturePath
        {
            get { return m_strTexturePath; }
            set
            {
                m_strTexturePath = value;

                if (!string.IsNullOrEmpty(m_strTexturePath))
                {
                    string strAbsPath = CSUtility.Support.IFileManager.Instance.Root + m_strTexturePath;
                    strAbsPath = strAbsPath.Replace("/\\", "\\");
                    image_Texture.Source = EditorCommon.ImageInit.GetImage(strAbsPath);
                    //// 判断扩展名
                    //int nExtIdx = m_strTexturePath.LastIndexOf('.');
                    //string strExt = m_strTexturePath.Substring(nExtIdx);

                    ////string strAbsPath = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(m_strTexturePath);
                    //string strAbsPath = CSUtility.Support.IFileManager.Instance.Root + m_strTexturePath;
                    //strAbsPath = strAbsPath.Replace("/\\", "\\");

                    //switch (strExt)
                    //{
                    //    case ".dds":
                    //    case ".tga":
                    //        image_Texture.Source = FrameSet.Assist.DDSConverter.Convert(strAbsPath);
                    //        break;

                    //    default:
                    //        image_Texture.Source = new BitmapImage(new Uri(strAbsPath));
                    //        break;
                    //}
                }
            }
        }

        public Tex2D(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            var linkObj = AddLinkObject(enLinkType.Texture, TextureLink, enBezierType.Left, enLinkOpType.End, null, false);
            linkObj.OnAddLinkInfo += new LinkObjInfo.Delegate_OnOperateLinkInfo(TextureLink_OnAddLinkInfo);
            linkObj = AddLinkObject(enLinkType.Float2, UVLink, enBezierType.Left, enLinkOpType.End, null, false);
            linkObj.OnAddLinkInfo += new LinkObjInfo.Delegate_OnOperateLinkInfo(UVLink_OnAddLinkInfo);
            AddLinkObject(enLinkType.Float4, RGBALink, enBezierType.Right, enLinkOpType.Start, RGBALink.BackBrush, true);
            AddLinkObject(enLinkType.Float3, RGBLink, enBezierType.Right, enLinkOpType.Start, RGBLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, RLink, enBezierType.Right, enLinkOpType.Start, RLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, GLink, enBezierType.Right, enLinkOpType.Start, GLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, BLink, enBezierType.Right, enLinkOpType.Start, BLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, ALink, enBezierType.Right, enLinkOpType.Start, ALink.BackBrush, true);
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("TexPath", TexturePath);
            base.Save(xmlNode, newGuid,holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            TexturePath = xmlNode.FindAttrib("TexPath").Value;
            base.Load(xmlNode, deltaX, deltaY);
        }

        void TextureLink_OnAddLinkInfo(LinkInfo linkInfo)
        {
            if (!linkInfo.m_linkFromObjectInfo.mIsLoadingLinks && !linkInfo.m_linkToObjectInfo.mIsLoadingLinks)
            {
                TextureControl tCtrl = linkInfo.m_linkFromObjectInfo.m_linkObj as TextureControl;
                if(tCtrl != null)
                    TexturePath = tCtrl.TexturePath;
            }
        }

        void UVLink_OnAddLinkInfo(LinkInfo linkInfo)
        {
            // todo 增加UV的移动和动画效果
        }

        private string GetVarName()
        {
            return "Text2D_" + CodeGenerateSystem.Program.GetValuedGUIDString(Id);
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            var strValueIdt = "float4 " + GetVarName() + " = float4(0,0,0,0);\r\n";
            if (!strDefinitionSegment.Contains(strValueIdt))
                strDefinitionSegment += "    " + strValueIdt;

            var strTab = GCode_GetTabString(nLayer);

            string uvName = "pssem.mDiffuseUV.xy";
            var linkOI = GetLinkObjInfo(UVLink);
            if (linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
                uvName = linkOI.GetLinkObject(0, true).GCode_GetValueName(linkOI.GetLinkElement(0, true));
            }

            linkOI = GetLinkObjInfo(TextureLink);
            if (linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
                var ctrl = linkOI.GetLinkObject(0, true);
                string texSampName = "";
                if (ctrl is TextureControl)
                {
                    texSampName = ((TextureControl)ctrl).GetTextureSampName();
                }
                else if (ctrl is ShaderAutoData)
                {
                    texSampName = ((ShaderAutoData)ctrl).GCode_GetValueName(null);
                }
                var assignStr = strTab + GetVarName() + " = vise_tex2D(" + texSampName + ", " + uvName + ");\r\n";
                // 这里先不做判断，连线中有if的情况下会导致问题
                //if (!strSegment.Contains(assignStr))
                if(!Program.IsSegmentContainString(strSegment.Length - 1, strSegment, assignStr))
                    strSegment += assignStr;
            }
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if(element == RGBALink)
            {
                return GetVarName();
            }
            else if(element == RGBLink)
            {
                return GetVarName() + ".xyz";
            }
            else if(element == RLink)
            {
                return GetVarName() + ".x";
            }
            else if(element == GLink)
            {
                return GetVarName() + ".y";
            }
            else if(element == BLink)
            {
                return GetVarName() + ".z";
            }
            else if(element == ALink)
            {
                return GetVarName() + ".w";
            }

            return base.GCode_GetValueName(element);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == null || element == RGBALink)
                return "float4";
            else if (element == RGBLink)
                return "float3";
            else if (element == RLink ||
                     element == GLink ||
                     element == BLink ||
                     element == ALink)
                return "float";

            return base.GCode_GetValueType(element);
        }
    }
}
