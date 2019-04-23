using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for IfNode.xaml
    /// </summary>
    //[PluginNodeAttribute(Path = "", EditorTypes = new string[] { "DelegateMethodEditor" })]
    [CodeGenerateSystem.ShowInNodeList("逻辑.if", "条件控制节点，根据条件控制脚本执行")]
    public partial class IfNode : CodeGenerateSystem.Base.BaseNodeControl
    {
        public IfNode(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            mNodeContainer = LinkStack;
            NodeName = "if";

            ConditionControl cc = new ConditionControl(parentCanvas);
            AddChildNode(cc, LinkStack);

            SetDragObject(RectangleTitle);
            SetUpLinkElement(MethodLink_Pre);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Pre, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink_Pre.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, FalseLinkHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, FalseLinkHandle.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Next, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink_Next.BackBrush, false);
        }

        private void AddConditionControl()
        {
            ConditionControl cc = new ConditionControl(ParentDrawCanvas);

            ContextMenu menu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "删除条件";
            menuItem.Click += new RoutedEventHandler(MenuItem_Click_DelMethod);
            menuItem.Tag = cc;
            menu.Items.Add(menuItem);
            cc.ContextMenu = menu;

            AddChildNode(cc, LinkStack);
        }

        private void MenuItem_Click_DelMethod(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ConditionControl node = item.Tag as ConditionControl;
            DelChildNode(node);
        }

        private void MenuItem_Click_AddLink(object sender, RoutedEventArgs e)
        {
            AddConditionControl();
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            var codeSC = codeStatementCollection;
            foreach (ConditionControl cc in mChildNodes)
            {
                cc.GCode_CodeDom_GenerateCode(codeClass, codeSC, null);
                codeSC = cc.ElseStatementCollection;
            }

            var linkOI = GetLinkObjInfo(FalseLinkHandle);
            if (linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeSC, linkOI.GetLinkElement(0, false));
            }

            var methodNextLink = GetLinkObjInfo(MethodLink_Next);
            if(methodNextLink.HasLink)
            {
                methodNextLink.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, methodNextLink.GetLinkElement(0, false));
            }
        }
    }

    public class ConditionControl : CodeGenerateSystem.Base.BaseNodeControl
    {
        CodeGenerateSystem.Controls.LinkOutControl mResultMethod;
        public CodeGenerateSystem.Controls.LinkOutControl ResultMethod
        {
            get { return mResultMethod; }
        }
        CodeGenerateSystem.Controls.LinkInControl mConditionEllipse;
        public CodeGenerateSystem.Controls.LinkInControl ConditionEllipse
        {
            get { return mConditionEllipse; }
        }

        //public bool m_bIsElseIf = false;

        public System.CodeDom.CodeStatementCollection ElseStatementCollection;

        public ConditionControl(Canvas parentCanvas)
            : base(parentCanvas, null)
        {
            //m_IsOneInLink = false;

            Grid grid = new Grid()
            {
                MinWidth = 100
            };
            AddChild(grid);

            var label = new TextBlock()
            {
                Text = "条件",
                Margin = new Thickness(3),
                Foreground = Brushes.White,//TryFindResource("TextForeground") as Brush,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            grid.Children.Add(label);

            label = new TextBlock()
            {
                Text = "true",
                Foreground = Brushes.White,//TryFindResource("TextForeground") as Brush,
                Margin = new Thickness(3),
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            grid.Children.Add(label);

            mConditionEllipse = new CodeGenerateSystem.Controls.LinkInControl()
            {
                Margin = new System.Windows.Thickness(-18, 0, 0, 0),
                Width = 13,
                Height = 13,
                BackBrush = new SolidColorBrush(Color.FromRgb(243, 146, 243)),
                HorizontalAlignment = HorizontalAlignment.Left,
                Direction = CodeGenerateSystem.Base.enBezierType.Left,
            };
            grid.Children.Add(mConditionEllipse);
            //m_conditionEllipse.MouseLeftButtonUp += new MouseButtonEventHandler(Condition_MouseLeftButtonUp);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Bool, mConditionEllipse, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, true);

            mResultMethod = new CodeGenerateSystem.Controls.LinkOutControl()
            {
                Margin = new Thickness(0, 0, -18, 0),
                Width = 13,
                Height = 13,
                BackBrush = new SolidColorBrush(Color.FromRgb(130, 130, 216)),
                HorizontalAlignment = HorizontalAlignment.Right,
                Direction = CodeGenerateSystem.Base.enBezierType.Right,
            };
            grid.Children.Add(mResultMethod);
            //methodEll.MouseLeftButtonDown += new MouseButtonEventHandler(ResultTrue_MouseLeftButtonDown);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, mResultMethod, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, mResultMethod.BackBrush, false);
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            CodeGenerateSystem.Base.LinkObjInfo linkOI = GetLinkObjInfo(mConditionEllipse);
            if (!linkOI.HasLink)
                return;

            if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));

            System.CodeDom.CodeConditionStatement condStatement = new System.CodeDom.CodeConditionStatement();
            condStatement.Condition = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));
            ElseStatementCollection = condStatement.FalseStatements;

            linkOI = GetLinkObjInfo(mResultMethod);
            if (linkOI.HasLink)
                linkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, condStatement.TrueStatements, linkOI.GetLinkElement(0, false));

            codeStatementCollection.Add(condStatement);
        }
    }
}
