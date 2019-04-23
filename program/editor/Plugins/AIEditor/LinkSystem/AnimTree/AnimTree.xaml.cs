using System;
using System.Windows;
using System.Windows.Controls;

namespace AIEditor.LinkSystem.AnimTree
{
    /// <summary>
    /// Interaction logic for Action.xaml
    /// </summary>
    [AIEditor.ShowInAIEditorMenu("动画树.动画树")]
    public partial class AnimTree : CodeGenerateSystem.Base.BaseNodeControl
    {
        public AnimTree(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);
            SetUpLinkElement(MethodLink_Pre);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Pre, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink_Pre.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.AnimNode, AN_InLink, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.End, AN_InLink.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Next, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink_Next.BackBrush, false);

            ValueNameBox.Text = strParam;

            //mAnimTreeNode = new FrameSet.AnimTree.AnimTreeNode();
            //m_ActionNode.Initialize();
        }

        //FrameSet.AnimTree.AnimTreeNode mAnimTreeNode;

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);
        }

        public override string GCode_GetClassName()
        {
            var linkOI = GetLinkObjInfo(AN_InLink);
            if (!linkOI.HasLink)
                return base.GCode_GetClassName();

            return linkOI.GetLinkObject(0, true).GCode_GetClassName();
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            var roleDeclare = new System.CodeDom.CodeVariableDeclarationStatement("CSUtility.AISystem.StateHost", "role");
            var roleAssign = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeVariableReferenceExpression("role"),
                                                                                                              new System.CodeDom.CodeCastExpression("CSUtility.AISystem.StateHost",
                                                                                                                                new System.CodeDom.CodeSnippetExpression("this.Host")));
            codeStatementCollection.Add(roleDeclare);
            codeStatementCollection.Add(roleAssign);

            //var ifRole = new System.CodeDom.CodeConditionStatement();
            //ifRole.Condition = new System.CodeDom.CodeBinaryOperatorExpression(
            //                                        new System.CodeDom.CodeVariableReferenceExpression("role"),
            //                                        System.CodeDom.CodeBinaryOperatorType.IdentityInequality,
            //                                        new System.CodeDom.CodePrimitiveExpression(null));

            //var meshDeclare = new System.CodeDom.CodeVariableDeclarationStatement("CCore.Mesh.Mesh", "mesh");
            //var meshAssign = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeVariableReferenceExpression("mesh"),
            //                                                                                                  new System.CodeDom.CodeCastExpression("CCore.Mesh.Mesh",
            //                                                                                                                    new System.CodeDom.CodeSnippetExpression("role.mVisual")));
            //ifRole.TrueStatements.Add(meshDeclare);
            //ifRole.TrueStatements.Add(meshAssign);

            //var ifMesh = new System.CodeDom.CodeConditionStatement();
            //ifMesh.Condition = new System.CodeDom.CodeBinaryOperatorExpression(
            //                                        new System.CodeDom.CodeVariableReferenceExpression("mesh"),
            //                                        System.CodeDom.CodeBinaryOperatorType.IdentityInequality,
            //                                        new System.CodeDom.CodePrimitiveExpression(null));


            // FrameSet.AnimTree.AnimTreeNode animTreeName = new FrameSet.AnimTree.AnimTreeNode();
            string animTreeName = GCode_GetValueName(null);
            System.CodeDom.CodeTypeReference actionNodeType = new System.CodeDom.CodeTypeReference("CSUtility.Animation.AnimationTree");
            System.CodeDom.CodeVariableDeclarationStatement actionNodeDeclare = new System.CodeDom.CodeVariableDeclarationStatement(actionNodeType, animTreeName,
                                                            new System.CodeDom.CodeMethodInvokeExpression(new System.CodeDom.CodeVariableReferenceExpression("Host"), "CreateAnimationNode"));
            //ifMesh.TrueStatements.Add(actionNodeDeclare);
            codeStatementCollection.Add(actionNodeDeclare);

            // animTreeName.Initialize();
            var animTreeVariable = new System.CodeDom.CodeVariableReferenceExpression(animTreeName);
            //var initExpression = new System.CodeDom.CodeMethodInvokeExpression(animTreeVariable, "Initialize", new System.CodeDom.CodeExpression[] { });
            //ifMesh.TrueStatements.Add(initExpression);

            // animTreeName.AddNode(childNodeName);
            var objInfo = GetLinkObjInfo(AN_InLink);
            if (objInfo.HasLink)
            {
                if (!objInfo.GetLinkObject(0, true).IsOnlyReturnValue)
                {
                    //objInfo.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, ifMesh.TrueStatements, objInfo.GetLinkElement(0, true));
                    objInfo.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, objInfo.GetLinkElement(0, true));
                }

                string childNodeName = objInfo.GetLinkObject(0, true).GCode_GetValueName(objInfo.GetLinkElement(0, true));

                var addExpression = new System.CodeDom.CodeMethodInvokeExpression(animTreeVariable, "AddNode", new System.CodeDom.CodeExpression[] { new System.CodeDom.CodeVariableReferenceExpression(childNodeName) });
                //ifMesh.TrueStatements.Add(addExpression);
                codeStatementCollection.Add(addExpression);
            }

            // animTreeName.mDelegateOnAnimTreeFinish = OnActionFinished;
            var assignName = new System.CodeDom.CodeAssignStatement(
                new System.CodeDom.CodeFieldReferenceExpression(animTreeVariable, "DelegateOnAnimTreeFinish"),
                new System.CodeDom.CodeMethodReferenceExpression(new System.CodeDom.CodeThisReferenceExpression(), "OnActionFinished"));
            //ifMesh.TrueStatements.Add(assignName);
            codeStatementCollection.Add(assignName);

            // mesh.SetAnimTree(animTreeName);
            var setAnimTree = new System.CodeDom.CodeMethodInvokeExpression(
                new System.CodeDom.CodeVariableReferenceExpression("Host"),
                "SetAnimTree", animTreeVariable);
            //ifMesh.TrueStatements.Add(setAnimTree);
            codeStatementCollection.Add(setAnimTree);

            //ifMesh.TrueStatements.Add(new System.CodeDom.CodeSnippetStatement(objInfo.GetLinkObject(0, true).GCode_GetValueName(objInfo.GetLinkElement(0, true)) + "=null;"));
            //ifMesh.TrueStatements.Add(new System.CodeDom.CodeSnippetStatement(animTreeName + "=null;"));

            //ifRole.TrueStatements.Add(ifMesh);

            //codeStatementCollection.Add(ifRole);

            //codeStatementCollection.Add(new System.CodeDom.CodeSnippetStatement(objInfo.GetLinkObject(0, true).GCode_GetValueName(objInfo.GetLinkElement(0, true)) + "=null;"));
            //codeStatementCollection.Add(new System.CodeDom.CodeSnippetStatement(animTreeName + "=null;"));

            var linkOI = GetLinkObjInfo(MethodLink_Next);
            if(linkOI.HasLink)
            {
                linkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, false));
            }
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (String.IsNullOrEmpty(ValueNameBox.Text))
            {
                string strValueName = "AnimNode_Action_" + Program.GetValuedGUIDString(Id);

                return strValueName;
            }
            else
            {
                return StrParams;
            }
        }

        private void ValueNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StrParams = ValueNameBox.Text;
        }

    }
}
