using System.Windows;
using System.Windows.Controls;

namespace DelegateMethodEditor.LinkSystem.Time
{
    /// <summary>
    /// Interaction logic for SinTime.xaml
    /// </summary>
    [DelegateMethodEditor.ShowInEditorMenu("参数.时间.SinTime")]
    public partial class SinTime : CodeGenerateSystem.Base.BaseNodeControl
    {
        public SinTime(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;
            SetDragObject(RectangleTitle);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Single,
                          ValueLinkHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueLinkHandle.BackBrush, true);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "System.Single";
        }
         
        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodeSnippetExpression("System.Math.Sin((System.DateTime.Now.Millisecond*0.001)*2*System.Math.PI)");
        }
    }
}
