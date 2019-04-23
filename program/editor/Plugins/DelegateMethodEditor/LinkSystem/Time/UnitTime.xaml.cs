using System.Windows;
using System.Windows.Controls;

namespace DelegateMethodEditor.LinkSystem.Time
{
    /// <summary>
    /// Interaction logic for UnitTime.xaml
    /// </summary>
    [DelegateMethodEditor.ShowInEditorMenu("参数.时间.UnitTime")]
    public partial class UnitTime : CodeGenerateSystem.Base.BaseNodeControl
    {
        public UnitTime(Canvas parentCanvas, string strParam)
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
            var value1 = new System.CodeDom.CodeVariableReferenceExpression("System.DateTime.Now.Millisecond");
            var value2 = new System.CodeDom.CodePrimitiveExpression(0.001f);
            var arithmeticExp = new System.CodeDom.CodeBinaryOperatorExpression();
            arithmeticExp.Left = value1;
            arithmeticExp.Right = value2;
            arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.Multiply;
            return arithmeticExp;
        }
    }
}
