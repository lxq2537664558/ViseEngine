using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls.Operation
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("运算.插值", "插值运算(Slerp)")]
    public partial class Slerp : BaseNodeControl
    {
        public Slerp(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            enLinkType linkType = enLinkType.Float1 | enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4;
            AddLinkObject(linkType, InputLinkX, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(linkType, InputLinkY, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(enLinkType.Float1, InputLinkAlpha, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(linkType, ResultLink, enBezierType.Right, enLinkOpType.Start, ResultLink.BackBrush, true);
        }
        protected override void CollectionErrorMsg()
        {
            var linkOI = GetLinkObjInfo(InputLinkX);
            if (!linkOI.HasLink)
            {
                AddErrorMsg(InputLinkX, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "X必需要有输入");
            }
            linkOI = GetLinkObjInfo(InputLinkY);
            if (!linkOI.HasLink)
            {
                AddErrorMsg(InputLinkY, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "Y必需要有输入");
            }
            linkOI = GetLinkObjInfo(InputLinkAlpha);
            if (!linkOI.HasLink)
            {
                AddErrorMsg(InputLinkAlpha, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "Alpha必需要有输入");
            }
        }
        public override string GCode_GetValueName(FrameworkElement element)
        {
            string strInputValueNameX;
            string strInputValueNameY;
            string strInputValueNameAlpha;
            string strFinalInputValueNameAlpha;
            var linkOIX = GetLinkObjInfo(InputLinkX);
            var linkOIY = GetLinkObjInfo(InputLinkY);
            var linkOIAlpha = GetLinkObjInfo(InputLinkAlpha);
            if (linkOIX.HasLink && linkOIY.HasLink && linkOIAlpha.HasLink )
            {
                strInputValueNameX = linkOIX.GetLinkObject(0, true).GCode_GetValueName(linkOIX.GetLinkElement(0, true));
                strInputValueNameY = linkOIY.GetLinkObject(0, true).GCode_GetValueName(linkOIY.GetLinkElement(0, true));
                strInputValueNameAlpha = linkOIAlpha.GetLinkObject(0, true).GCode_GetValueName(linkOIAlpha.GetLinkElement(0, true));

                strFinalInputValueNameAlpha = strInputValueNameAlpha;
                switch (linkOIX.GetLinkObject(0, true).GCode_GetValueType(linkOIX.GetLinkElement(0, true)))
                {
                    case "float4":
                        {
                            strFinalInputValueNameAlpha = "float4(" + strInputValueNameAlpha + "," + strInputValueNameAlpha + "," + strInputValueNameAlpha + "," + strInputValueNameAlpha + ")";
                        }
                        break;
                    case "float3":
                        {
                            strFinalInputValueNameAlpha = "float3(" + strInputValueNameAlpha + "," + strInputValueNameAlpha + "," + strInputValueNameAlpha + ")";
                        }
                        break;
                    case "float2":
                        {
                            strFinalInputValueNameAlpha = "float2(" + strInputValueNameAlpha + "," + strInputValueNameAlpha + ")";
                        }
                        break;
                }

                return "lerp(" + strInputValueNameX + "," + strInputValueNameY + "," + strFinalInputValueNameAlpha + ")";
            }

            return base.GCode_GetValueName(element);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            var lOI = GetLinkObjInfo(InputLinkX);
            if (lOI.HasLink)
            {
                return lOI.GetLinkObject(0, true).GCode_GetValueType(lOI.GetLinkElement(0, true));
            }

            return base.GCode_GetValueType(element);
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(InputLinkX);
            if (linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
            }
            linkOI = GetLinkObjInfo(InputLinkY);
            if (linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
            }
            linkOI = GetLinkObjInfo(InputLinkAlpha);
            if (linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
            }
        }
    }
}
