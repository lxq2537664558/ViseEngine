using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for Compare.xaml
    /// </summary>
    public partial class Compare : CodeGenerateSystem.Base.BaseNodeControl
    {
        public Compare(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(RectangleTitle);

            CodeGenerateSystem.Base.enLinkType linkType = CodeGenerateSystem.Base.enLinkType.NumbericalValue | CodeGenerateSystem.Base.enLinkType.VectorValue;

            switch (strParam)
            {
                case "＝":
                case "==":
                case "≠":
                    linkType = linkType | CodeGenerateSystem.Base.enLinkType.Class | CodeGenerateSystem.Base.enLinkType.Bool | CodeGenerateSystem.Base.enLinkType.String;
                    break;
            }

            AddLinkObject(linkType, ParamLink_1, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(linkType, ParamLink_2, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Bool, resultHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, resultHandle.BackBrush, true);

            resultLabel.Text = StrParams;
            TitleLabel.Text = TitleLabel.Text + "(" + P1_Label.Text + StrParams + P2_Label.Text + ")";
            NodeName = TitleLabel.Text;
        }

        #region 代码生成

        System.CodeDom.CodeExpression mParam1Exp, mParam2Exp;
        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            var linkParam1 = GetLinkObjInfo(ParamLink_1);
            var linkParam2 = GetLinkObjInfo(ParamLink_2);
            if (!linkParam1.HasLink || !linkParam2.HasLink)
                return;

            if (!linkParam1.GetLinkObject(0, true).IsOnlyReturnValue)
                linkParam1.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkParam1.GetLinkElement(0, true));

            mParam1Exp = linkParam1.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkParam1.GetLinkElement(0, true));

            if (!linkParam2.GetLinkObject(0, true).IsOnlyReturnValue)
                linkParam2.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkParam2.GetLinkElement(0, true));

            mParam2Exp = linkParam2.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkParam2.GetLinkElement(0, true));
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            var compareExp = new System.CodeDom.CodeBinaryOperatorExpression();
            compareExp.Left = mParam1Exp;
            compareExp.Right = mParam2Exp;

            switch (StrParams)
            {
                case "＞":
                    compareExp.Operator = System.CodeDom.CodeBinaryOperatorType.GreaterThan;
                    break;
                case "＝":
                case "==":
                    compareExp.Operator = System.CodeDom.CodeBinaryOperatorType.ValueEquality;
                    break;
                case "＜":
                    compareExp.Operator = System.CodeDom.CodeBinaryOperatorType.LessThan;
                    break;
                case "≥":
                    compareExp.Operator = System.CodeDom.CodeBinaryOperatorType.GreaterThanOrEqual;
                    break;
                case "≤":
                    compareExp.Operator = System.CodeDom.CodeBinaryOperatorType.LessThanOrEqual;
                    break;
                case "≠":
                    compareExp.Operator = System.CodeDom.CodeBinaryOperatorType.IdentityInequality;
                    break;
            }

            return compareExp;
        }

        #endregion
    }
}
