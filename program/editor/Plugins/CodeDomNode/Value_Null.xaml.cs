using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for Value_Null.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("数值.null", "空数值节点，设置参数为空")]
    public partial class Value_Null : CodeGenerateSystem.Base.BaseNodeControl
    {
        public Value_Null(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;
            SetDragObject(RectangleTitle);
            NodeName = "null";

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class | CodeGenerateSystem.Base.enLinkType.Vector2 | CodeGenerateSystem.Base.enLinkType.Vector3 | CodeGenerateSystem.Base.enLinkType.Vector4,
                          ValueLinkHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueLinkHandle.BackBrush, true);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "System.Object";
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodePrimitiveExpression(null);
        }
    }
}
