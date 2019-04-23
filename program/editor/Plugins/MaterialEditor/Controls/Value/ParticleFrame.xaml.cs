using CodeGenerateSystem.Base;
using System.Windows;
using System.Windows.Controls;

namespace MaterialEditor.Controls.Value
{
    /// <summary>
    /// Interaction logic for ParticleFrame.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("参数.粒子.粒子动画帧(ParticleFrame)", "控制粒子帧动画关键帧(ParticleFrame)")]
    public partial class ParticleFrame : BaseNodeControl, MaterialStreamRequire
    {
        public ParticleFrame(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);
            AddLinkObject(enLinkType.Float1, TimeValueLink, enBezierType.Right, enLinkOpType.Start, TimeValueLink.BackBrush, true);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            return "pssem.mLocalTangent.w";
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "float";
        }

        public string GetStreamRequire()
        {
            return "LocalTangent";
        }
    }
}
