using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls.Operation
{
    /// <summary>
    /// Normalize.xaml 的交互逻辑
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("运算.归一化(Normalize)", "向量归一化运算")]
    public partial class Normalize : BaseNodeControl
    {
        public Normalize(Canvas parentCanvas, string strParam)
            :base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            enLinkType linkType = enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4;
            AddLinkObject(linkType, InputLink, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(linkType, ResultLink, enBezierType.Right, enLinkOpType.Start, ResultLink.BackBrush, true);
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

        public override string GCode_GetValueName(FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(InputLink);
            if (linkOI.HasLink)
            {
                string strInputValueName = linkOI.GetLinkObject(0, true).GCode_GetValueName(linkOI.GetLinkElement(0, true));
                return "normalize(" + strInputValueName + ")";
            }

            return base.GCode_GetValueName(element);
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
