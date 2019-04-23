using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls.Value
{
    /// <summary>
    /// Interaction logic for ParticleColor.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("参数.粒子.粒子颜色(ParticleColor)", "控制粒子颜色(ParticleColor)")]
    public partial class ParticleColor : BaseNodeControl, MaterialStreamRequire
    {
        public ParticleColor(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);
            AddLinkObject(enLinkType.Float4, TimeValueLink, enBezierType.Right, enLinkOpType.Start, TimeValueLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, TimeValueLink_R, enBezierType.Right, enLinkOpType.Start, TimeValueLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, TimeValueLink_G, enBezierType.Right, enLinkOpType.Start, TimeValueLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, TimeValueLink_B, enBezierType.Right, enLinkOpType.Start, TimeValueLink.BackBrush, true);
            AddLinkObject(enLinkType.Float1, TimeValueLink_A, enBezierType.Right, enLinkOpType.Start, TimeValueLink.BackBrush, true);

        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (element == TimeValueLink_R)
                return "pssem.mVertexColor0.r";
            if (element == TimeValueLink_G)
                return "pssem.mVertexColor0.g";
            if (element == TimeValueLink_B)
                return "pssem.mVertexColor0.b";
            if (element == TimeValueLink_A)
                return "pssem.mVertexColor0.a";

            return "pssem.mVertexColor0";
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == TimeValueLink_R ||
                element == TimeValueLink_G ||
                element == TimeValueLink_B ||
                element == TimeValueLink_A)
                return "float";

            return "float4";
        }

        public string GetStreamRequire()
        {
            return "VertexColor0";
        }
    }
}
