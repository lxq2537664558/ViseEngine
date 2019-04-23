using System.Windows.Controls;

namespace AIEditor.LinkSystem.AnimTree
{
    /// <summary>
    /// Interaction logic for Action.xaml
    /// </summary>
//    [AIEditor.ShowInAIEditorMenu("动画树.动作")]
    public partial class Action : CodeGenerateSystem.Base.BaseNodeControl
    {
        public Action(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.AnimNode, AN_OutLink, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.Start, AN_OutLink.BackBrush, false);

            ValueNameBox.Text = strParam;

            /*m_ActionNode = new CCore.AnimTree.AnimTreeNode_Action();
            m_ActionNode.Initialize();
            PropertyGrid.Instance = m_ActionNode;*/
        }

        /*CCore.AnimTree.AnimTreeNode_Action m_ActionNode;

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid)
        {
            xmlNode.AddAttrib("ActionName", m_ActionNode.ActionName);
            xmlNode.AddAttrib("TileScale", m_ActionNode.PlayRate.ToString());
            xmlNode.AddAttrib("PlayerMode", ((int)m_ActionNode.PlayerMode).ToString());
            xmlNode.AddAttrib("XRootMotionType", m_ActionNode.XRootmotionType.ToString());
            xmlNode.AddAttrib("YRootMotionType", m_ActionNode.YRootmotionType.ToString());
            xmlNode.AddAttrib("ZRootMotionType", m_ActionNode.ZRootmotionType.ToString());
            base.Save(xmlNode, newGuid);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);
            CSUtility.Support.IXmlAttrib attrib = xmlNode.FindAttrib("ActionName");
            if (attrib != null)
                m_ActionNode.ActionName = attrib.Value;
            attrib = xmlNode.FindAttrib("TileScale");
            if (attrib != null)
                m_ActionNode.PlayRate = System.Convert.ToSingle(attrib.Value);
            attrib = xmlNode.FindAttrib("PlayerMode");
            if (attrib != null)
                m_ActionNode.PlayerMode = (CSUtility.Animation.EActionPlayerMode)System.Convert.ToInt32(attrib.Value);
            attrib = xmlNode.FindAttrib("XRootMotionType");
            if (attrib != null)
                m_ActionNode.XRootmotionType = (CSUtility.Animation.AxisRootmotionType)System.Enum.Parse(typeof(CSUtility.Animation.AxisRootmotionType), attrib.Value);
            attrib = xmlNode.FindAttrib("YRootMotionType");
            if (attrib != null)
                m_ActionNode.YRootmotionType = (CSUtility.Animation.AxisRootmotionType)System.Enum.Parse(typeof(CSUtility.Animation.AxisRootmotionType), attrib.Value);
            attrib = xmlNode.FindAttrib("ZRootMotionType");
            if (attrib != null)
                m_ActionNode.ZRootmotionType = (CSUtility.Animation.AxisRootmotionType)System.Enum.Parse(typeof(CSUtility.Animation.AxisRootmotionType), attrib.Value);

        }

        public override string GCode_GetClassName()
        {
            var linkOI = GetLinkObjInfo(AN_OutLink);
            if (!linkOI.bHasLink)
                return base.GCode_GetClassName();

            return linkOI.GetLinkObject(0, true).GCode_GetClassName();
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            // CCore.AnimTree.AnimTreeNode_Action actionNodeName = new CCore.AnimTree.AnimTreeNode_Action();
            // actionNodeName.Initialize();
            string actionNodeName = GCode_GetValueName(null);
            System.CodeDom.CodeTypeReference actionNodeType = new System.CodeDom.CodeTypeReference("CSUtility.Animation.BaseAction");
            System.CodeDom.CodeVariableDeclarationStatement actionNodeDeclare = new System.CodeDom.CodeVariableDeclarationStatement(actionNodeType, actionNodeName,
                                                            new System.CodeDom.CodeMethodInvokeExpression(new System.CodeDom.CodeVariableReferenceExpression("Host"), "CreateBaseAction"));
            codeStatementCollection.Add(actionNodeDeclare);
            System.CodeDom.CodeVariableReferenceExpression actionNodeVariable = new System.CodeDom.CodeVariableReferenceExpression(actionNodeName);
            //System.CodeDom.CodeMethodInvokeExpression initExpression = new System.CodeDom.CodeMethodInvokeExpression(actionNodeVariable, "Initialize", new System.CodeDom.CodeExpression[] { });
            //codeStatementCollection.Add(initExpression);

            // Initialize actoinNode from m_ActionNode           
            // actionNodeName.AcitionName = m_ActionNode.ActionName;
            // actionNodeName.PlayRate = m_ActionNode.PlayRate;
            // actionNodeName.Loop = m_ActionNode.Loop;
            // actionNodeName.XRootmotionType = m_ActionNode.XRootmotionType;
            // actionNodeName.YRootmotionType = m_ActionNode.YRootmotionType;
            // actionNodeName.ZRootmotionType = m_ActionNode.ZRootmotionType;
            System.CodeDom.CodeAssignStatement assignActionName = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(actionNodeVariable, "ActionName"), new System.CodeDom.CodePrimitiveExpression(m_ActionNode.ActionName));
            System.CodeDom.CodeAssignStatement assignPlayRate = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(actionNodeVariable, "PlayRate"), new System.CodeDom.CodePrimitiveExpression(m_ActionNode.PlayRate));
            System.CodeDom.CodeAssignStatement assignLoop = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(actionNodeVariable, "PlayerMode"), new System.CodeDom.CodeCastExpression("CSUtility.Animation.EActionPlayerMode", new System.CodeDom.CodePrimitiveExpression((int)m_ActionNode.PlayerMode)));
            System.CodeDom.CodeAssignStatement assignXType = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(actionNodeVariable, "XRootmotionType"), new System.CodeDom.CodeCastExpression("CSUtility.Animation.AxisRootmotionType", new System.CodeDom.CodePrimitiveExpression((int)m_ActionNode.XRootmotionType)));
            System.CodeDom.CodeAssignStatement assignYType = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(actionNodeVariable, "YRootmotionType"), new System.CodeDom.CodeCastExpression("CSUtility.Animation.AxisRootmotionType", new System.CodeDom.CodePrimitiveExpression((int)m_ActionNode.YRootmotionType)));
            System.CodeDom.CodeAssignStatement assignZType = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(actionNodeVariable, "ZRootmotionType"), new System.CodeDom.CodeCastExpression("CSUtility.Animation.AxisRootmotionType", new System.CodeDom.CodePrimitiveExpression((int)m_ActionNode.ZRootmotionType)));

            codeStatementCollection.Add(assignActionName);
            codeStatementCollection.Add(assignPlayRate);
            codeStatementCollection.Add(assignLoop);
            codeStatementCollection.Add(assignXType);
            codeStatementCollection.Add(assignYType);
            codeStatementCollection.Add(assignZType);

            // 生成所属类的成员函数， 并加将此函数加到actionNodeName的相应代理中
            // TODO： 根据AnimNodeSupport_MethodControl类型判断生成的代码
            if (codeClass != null)
            {
                // renwind modified
                //foreach (AnimNodeSupport_MethodControl item in StackPanel_Method.Children)
                //{
                //    CodeGenerateSystem.Base.LinkObjInfo linkOI = item.GetLinkObjInfo(item.InLink);
                //    if (null != linkOI && linkOI.bHasLink)
                //    {
                //        // actionNodeName.mDelegateOnActionFinish = new MidLayer.Delegate_OnActionFinish(OnActionFinished);
                //        System.CodeDom.CodeAssignStatement assignDelegate = new System.CodeDom.CodeAssignStatement();
                //        assignDelegate.Left = new System.CodeDom.CodeFieldReferenceExpression(actionNodeVariable, "mDelegateOnActionFinish");
                //        assignDelegate.Right = new System.CodeDom.CodeObjectCreateExpression("MidLayer.Delegate_OnActionFinish", new System.CodeDom.CodeExpression[] { new System.CodeDom.CodeVariableReferenceExpression("OnActionFinished") });
                //        codeStatementCollection.Add(assignDelegate);

                //        System.CodeDom.CodeMemberMethod methodCode = new System.CodeDom.CodeMemberMethod();
                //        methodCode.Attributes = System.CodeDom.MemberAttributes.Public;
                //        methodCode.Name = item.strParams;
                //        methodCode.ReturnType = new System.CodeDom.CodeTypeReference("System.Void");

                //        linkOI.m_linkInfos[0].m_linkFromObjectInfo.m_linkObj.GCode_CodeDom_GenerateCode(codeClass, methodCode.Statements, linkOI.m_linkInfos[0].m_linkFromObjectInfo.m_linkElement);

                //        codeClass.Members.Add(methodCode);
                //    }
                //}
            }
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, FrameworkElement element)
        {
            if (StackPanel_Method.Children.Count <= 0)
                return;

            // renwind modified
            //foreach (AnimNodeSupport_MethodControl item in StackPanel_Method.Children)
            //{
            //    CodeGenerateSystem.Base.LinkObjInfo linkOI = GetLinkObjInfo(item.InLink);
            //    if (linkOI.bHasLink)
            //    {
            //        System.CodeDom.CodeMemberMethod methodCode = new System.CodeDom.CodeMemberMethod();
            //        methodCode.Attributes = System.CodeDom.MemberAttributes.Public;
            //        methodCode.Name = item.strParams;
            //        methodCode.ReturnType = new System.CodeDom.CodeTypeReference("System.Void");

            //        linkOI.m_linkInfos[0].m_linkFromObjectInfo.m_linkObj.GCode_CodeDom_GenerateCode(codeClass, methodCode.Statements, linkOI.m_linkInfos[0].m_linkFromObjectInfo.m_linkElement);

            //        codeClass.Members.Add(methodCode);
            //    }
            //}
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
