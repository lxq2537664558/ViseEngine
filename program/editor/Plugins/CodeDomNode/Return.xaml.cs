using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for Return.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("逻辑.return", "函数返回值节点，设置脚本的返回值")]
    public partial class Return : CodeGenerateSystem.Base.BaseNodeControl
    {
        public Return(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);
            SetUpLinkElement(MethodLink);

            NodeName = "return";

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Value, ReturnValueLink, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink.BackBrush, true);
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            var retStatement = new System.CodeDom.CodeMethodReturnStatement();

            var linkInfo = GetLinkObjInfo(ReturnValueLink);
            if (linkInfo.HasLink)
            {
                if (!linkInfo.GetLinkObject(0, true).IsOnlyReturnValue)
                    linkInfo.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkInfo.GetLinkElement(0, true));

                retStatement.Expression = linkInfo.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkInfo.GetLinkElement(0, true));
            }

            codeStatementCollection.Add(retStatement);
        }
    }
}
