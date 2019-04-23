using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls.Value
{
    /// <summary>
    /// Interaction logic for ParticleLifeRate.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("参数.粒子.粒子生命周期比率(ParticleLifeRate)", "生命的周期比率，0为出生，1为死亡(ParticleLifeRate)")]
    public partial class ParticleLifeRate : BaseNodeControl, MaterialStreamRequire
    {
        public ParticleLifeRate(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);
            AddLinkObject(enLinkType.Float1, TimeValueLink, enBezierType.Right, enLinkOpType.Start, TimeValueLink.BackBrush, true);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            return "pssem.mLightMapUV.w";
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "float";
        }

        public string GetStreamRequire()
        {
            return "LightMapUV";
        }
    }
}
