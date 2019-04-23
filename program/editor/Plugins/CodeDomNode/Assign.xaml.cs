using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for Assign.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("逻辑.赋值", "赋值操作节点")]
    public partial class Assign : CodeGenerateSystem.Base.BaseNodeControl
    {
        public Assign(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);
            SetUpLinkElement(MethodLink_Pre);

            NodeName = "赋值";

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Pre, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink_Pre.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Value | CodeGenerateSystem.Base.enLinkType.Class, SetValueElement, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Value | CodeGenerateSystem.Base.enLinkType.Class, ValueElement, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueElement.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Next, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink_Next.BackBrush, false);
        }

        protected override void CollectionErrorMsg()
        {
            var methodLinkOI = GetLinkObjInfo(MethodLink_Pre);
            if(methodLinkOI.HasLink)
            {
                var linOI = GetLinkObjInfo(SetValueElement);
                if(!linOI.HasLink)
                    AddErrorMsg(SetValueElement, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "未设置输入值");
                linOI = GetLinkObjInfo(ValueElement);
                if (!linOI.HasLink)
                    AddErrorMsg(ValueElement, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "未设置输出值");
            }
        }

        public override bool HasMultiOutLink
        {
            get
            {
                var linkOI = GetLinkObjInfo(ValueElement);
                return linkOI.LinkInfos.Count > 1;
            }
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            if (element == MethodLink_Pre)
            {
                var linkValue = GetLinkObjInfo(ValueElement);
                var linkSetValue = GetLinkObjInfo(SetValueElement);
                if (linkValue.HasLink && linkSetValue.HasLink)
                {
                    var codeAss = new System.CodeDom.CodeAssignStatement();
                    codeAss.Left = linkValue.GetLinkObject(0, false).GCode_CodeDom_GetValue(linkValue.GetLinkElement(0, false));

                    if (!linkSetValue.GetLinkObject(0, true).IsOnlyReturnValue)
                        linkSetValue.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkSetValue.GetLinkElement(0, true));
                    codeAss.Right = new System.CodeDom.CodeCastExpression(linkValue.GetLinkObject(0, false).GCode_GetValueType(linkValue.GetLinkElement(0, false)),
                                                                          linkSetValue.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkSetValue.GetLinkElement(0, true)));

                    codeStatementCollection.Add(codeAss);
                }

                var linkOI = GetLinkObjInfo(MethodLink_Next);
                if(linkOI.HasLink)
                {
                    linkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, false));
                }
            }
        }
    }
}
