using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;
using CSUtility.Support;

namespace MaterialEditor.Controls.Operation
{
    /// <summary>
    /// Interaction logic for Abs.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("运算.Abs", "计算绝对值")]
    public partial class Abs : BaseNodeControl
    {

        public Abs(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Title);

            enLinkType linkType = enLinkType.Float1 | enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4;
            AddLinkObject(linkType, InputLink, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(linkType, ResultLink, enBezierType.Right, enLinkOpType.Start, ResultLink.BackBrush, true);
        }

        protected override void CollectionErrorMsg()
        {
            var linkOI = GetLinkObjInfo(InputLink);
            if (!linkOI.HasLink)
            {
                AddErrorMsg(InputLink, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "必需要有输入");
            }
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(InputLink);
            if (linkOI.HasLink)
            {
                string strInputValueName = linkOI.GetLinkObject(0, true).GCode_GetValueName(linkOI.GetLinkElement(0, true));
                return "abs(" + strInputValueName + ")";
            }

            return base.GCode_GetValueName(element);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            var lOI = GetLinkObjInfo(InputLink);
            if (lOI.HasLink)
            {
                return lOI.GetLinkObject(0, true).GCode_GetValueType(lOI.GetLinkElement(0, true));
            }

            return base.GCode_GetValueType(element);
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(InputLink);
            if (linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
            }
        }
    }
}
