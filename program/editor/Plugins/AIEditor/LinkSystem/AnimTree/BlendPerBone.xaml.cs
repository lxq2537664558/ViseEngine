using System.Windows.Controls;

namespace AIEditor.LinkSystem.AnimTree
{
    /// <summary>
    /// Interaction logic for Action.xaml
    /// </summary>
//    [AIEditor.ShowInAIEditorMenu("动画树.动作混合")]
    public partial class BlendPerBone : CodeGenerateSystem.Base.BaseNodeControl
    {
        public BlendPerBone(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.AnimNode, AN_OutLink, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.Start, AN_OutLink.BackBrush, false);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.AnimNode, AN_InLink0, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.End, AN_InLink0.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.AnimNode, AN_InLink1, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.End, AN_InLink1.BackBrush, false);

            ValueNameBox.Text = strParam;

            /*m_BlendPerBoneNode = new FrameSet.AnimTree.AnimTreeNode_BlendPerBone();
            m_BlendPerBoneNode.Initialize();
            PropertyGrid.Instance = m_BlendPerBoneNode;*/
        }

        /*FrameSet.AnimTree.AnimTreeNode_BlendPerBone m_BlendPerBoneNode;

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid)
        {
            xmlNode.AddAttrib("BranchBoneName", m_BlendPerBoneNode.BranchBoneName);
            base.Save(xmlNode, newGuid);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);
            CSUtility.Support.IXmlAttrib attrib = xmlNode.FindAttrib("BranchBoneName");
            if (attrib != null)
                m_BlendPerBoneNode.BranchBoneName = attrib.Value;
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            //FrameSet.AnimTree.AnimTreeNode_BlendPerBone blendNode = new FrameSet.AnimTree.AnimTreeNode_BlendPerBone();
            //blendNode.Initialize();
            //blendNode.BranchBoneName = m_BlendPerBoneNode.BranchBoneName;

            string nodeName = GCode_GetValueName(null);
            System.CodeDom.CodeTypeReference nodeType = new System.CodeDom.CodeTypeReference("FrameSet.AnimTree.AnimTreeNode_BlendPerBone");
            System.CodeDom.CodeVariableDeclarationStatement nodeDeclare = new System.CodeDom.CodeVariableDeclarationStatement(nodeType, nodeName,
                                                            new System.CodeDom.CodeObjectCreateExpression(nodeType, new System.CodeDom.CodeExpression[] { }));
            codeStatementCollection.Add(nodeDeclare);
            System.CodeDom.CodeVariableReferenceExpression nodeVariable = new System.CodeDom.CodeVariableReferenceExpression(nodeName);
            System.CodeDom.CodeMethodInvokeExpression initExpression = new System.CodeDom.CodeMethodInvokeExpression(nodeVariable, "Initialize", new System.CodeDom.CodeExpression[] { });
            codeStatementCollection.Add(initExpression);

            System.CodeDom.CodeAssignStatement assignName = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(nodeVariable, "BranchBoneName"), new System.CodeDom.CodePrimitiveExpression(m_BlendPerBoneNode.BranchBoneName));
            codeStatementCollection.Add(assignName);

            //blendNode.AddNode(action1);
            //blendNode.AddNode(action2);

            var objInfo = GetLinkObjInfo(AN_InLink0);
            if (objInfo.bHasLink)
            {
                if (!objInfo.GetLinkObject(0, true).m_bOnlyReturnValue)
                {
                    objInfo.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, objInfo.GetLinkElement(0, true));
                }

                string childNodeName = objInfo.GetLinkObject(0, true).GCode_GetValueName(objInfo.GetLinkElement(0, true));

                var addExpression = new System.CodeDom.CodeMethodInvokeExpression(nodeVariable, "AddNode", new System.CodeDom.CodeExpression[] { new System.CodeDom.CodeVariableReferenceExpression(childNodeName) });
                codeStatementCollection.Add(addExpression);
            }

            var objInfo1 = GetLinkObjInfo(AN_InLink1);
            if (objInfo1.bHasLink)
            {
                if (!objInfo1.GetLinkObject(0, true).m_bOnlyReturnValue)
                {
                    objInfo1.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, objInfo1.GetLinkElement(0, true));
                }

                string childNodeName = objInfo1.GetLinkObject(0, true).GCode_GetValueName(objInfo1.GetLinkElement(0, true));

                var addExpression = new System.CodeDom.CodeMethodInvokeExpression(nodeVariable, "AddNode", new System.CodeDom.CodeExpression[] { new System.CodeDom.CodeVariableReferenceExpression(childNodeName) });
                codeStatementCollection.Add(addExpression);
            }


        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (String.IsNullOrEmpty(ValueNameBox.Text))
            {
                string strValueName = "AnimNode_Action_" + Program.GetValuedGUIDString(m_Guid);

                return strValueName;
            }
            else
            {
                return m_strParams;
            }
        }*/

        private void ValueNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StrParams = ValueNameBox.Text;
        }

    }
}
