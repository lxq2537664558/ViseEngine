using System.Windows;
using System.Windows.Controls;

namespace AIEditor.LinkSystem.Debug
{
    /// <summary>
    /// Interaction logic for CodeDebuggerControl.xaml
    /// </summary>
    [AIEditor.ShowInAIEditorMenu("调试.Debugger")]
    public partial class CodeDebuggerControl : CodeGenerateSystem.Base.BaseNodeControl
    {
        public CodeDebuggerControl(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);
            SetUpLinkElement(MethodLink_Pre);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Pre, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink_Pre.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Next, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink_Next.BackBrush, false);
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("Category", TextBox_DebuggerCategory.Text);
            xmlNode.AddAttrib("Message", TextBox_DebuggerMessage.Text);
            xmlNode.AddAttrib("IsBreak", CheckBox_Break.IsChecked.ToString());

            base.Save(xmlNode, newGuid,holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            var att = xmlNode.FindAttrib("Category");
            if (att != null)
                TextBox_DebuggerCategory.Text = att.Value;

            att = xmlNode.FindAttrib("Message");
            if(att != null)
                TextBox_DebuggerMessage.Text = att.Value;

            att = xmlNode.FindAttrib("IsBreak");
            if (att != null)
                CheckBox_Break.IsChecked = System.Convert.ToBoolean(att.Value);

            base.Load(xmlNode, deltaX, deltaY);
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            if (element == MethodLink_Pre)
            {
                var condition = new System.CodeDom.CodeConditionStatement();
                condition.Condition = new System.CodeDom.CodeSnippetExpression("System.Diagnostics.Debugger.IsAttached && System.Diagnostics.Debugger.IsLogging()");
                condition.TrueStatements.Add(new System.CodeDom.CodeMethodInvokeExpression(
                                                    new System.CodeDom.CodeTypeReferenceExpression(typeof(System.Diagnostics.Debugger)),
                                                    "Log",
                                                    new System.CodeDom.CodeExpression[]{
                                                            new System.CodeDom.CodeVariableReferenceExpression("1"),
                                                            new System.CodeDom.CodeSnippetExpression("\"" + TextBox_DebuggerCategory.Text + "\""),
                                                            new System.CodeDom.CodeSnippetExpression("\"" + TextBox_DebuggerMessage.Text + "\"")
                                                    }));


                //    new System.CodeDom.CodeSnippetStatement("System.Diagnostics.Debugger.Log(1, \"" + TextBox_DebuggerCategory.Text + "\", \"" + TextBox_DebuggerMessage.Text + "\");"));

                if (CheckBox_Break.IsChecked == true)
                {
                    condition.TrueStatements.Add(new System.CodeDom.CodeMethodInvokeExpression(
                                                        new System.CodeDom.CodeTypeReferenceExpression(typeof(System.Diagnostics.Debugger)),
                                                        "Break",
                                                        new System.CodeDom.CodeExpression[]{}));
                    //condition.TrueStatements.Add(new System.CodeDom.CodeSnippetStatement("System.Diagnostics.Debugger.Break();"));
                }
                
                //var waitOneStatment = new System.CodeDom.CodeSnippetExpression("Common.Program.mAutoEvent.WaitOne()");
                //condition.TrueStatements.Add(waitOneStatment);
                //condition.FalseStatements.Add(waitOneStatment);

                codeStatementCollection.Add(condition);

                var linkOI = GetLinkObjInfo(MethodLink_Next);
                if(linkOI.HasLink)
                {
                    linkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, false));
                }
            }
        }
    }
}
