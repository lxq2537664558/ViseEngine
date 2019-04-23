using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for CodeControl.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("逻辑.代码块", "手工编写代码的节点")]
    public partial class CodeControl : CodeGenerateSystem.Base.BaseNodeControl
    {
        string mCodeExplain = "";
        public string CodeExplain
        {
            get { return mCodeExplain; }
            set
            {
                mCodeExplain = value;
                OnPropertyChanged("CodeExplain");
            }
        }

        public CodeControl(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);
            SetUpLinkElement(MethodLink_Pre);
            NodeName = "代码块";

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Pre, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink_Pre.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Next, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink_Next.BackBrush, false);
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("CodeExplain", CodeExplain);
            xmlNode.AddAttrib("Code", TextEditor_CodeEditor.Text);

            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            var att = xmlNode.FindAttrib("CodeExplain");
            if (att != null)
            {
                CodeExplain = att.Value;
            }
            att = xmlNode.FindAttrib("Code");
            if (att != null)
            {
                TextEditor_CodeEditor.Text = att.Value;
            }

            base.Load(xmlNode, deltaX, deltaY);
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            codeStatementCollection.Add(new System.CodeDom.CodeSnippetStatement("// " + CodeExplain + ";"));
            codeStatementCollection.Add(new System.CodeDom.CodeSnippetStatement(TextEditor_CodeEditor.Text + ";"));

            var linkOI = GetLinkObjInfo(MethodLink_Next);
            if(linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, false));
            }
        }
    }
}
