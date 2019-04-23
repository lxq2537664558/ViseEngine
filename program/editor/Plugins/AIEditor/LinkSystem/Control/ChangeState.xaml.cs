using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIEditor.LinkSystem.Control
{
    /// <summary>
    /// Interaction logic for ChangeState.xaml
    /// </summary>
    [AIEditor.ShowInAIEditorMenu("控制.状态.ChangeState(转状态)")]
    public partial class ChangeState : CodeGenerateSystem.Base.BaseNodeControl
    {
        Type mStateParameterType;

        List<FrameworkElement> mParamRectList = new List<FrameworkElement>();      // 记录状态参数右侧的方框
        List<FrameworkElement> mParamEllipseList = new List<FrameworkElement>();   // 记录状态参数左侧的圆
        Dictionary<FrameworkElement, System.Reflection.PropertyInfo> mValueInfoDic = new Dictionary<FrameworkElement, System.Reflection.PropertyInfo>();
        
        public struct stInLinkData
        {
            public System.Reflection.PropertyInfo propertyInfo;
            public FrameworkElement linkElement;
        }

        Dictionary<string, stInLinkData> m_InLinkElements = new Dictionary<string, stInLinkData>();
        System.CodeDom.CodeVariableDeclarationStatement stateParamDeclaration = new System.CodeDom.CodeVariableDeclarationStatement();
        System.CodeDom.CodeVariableDeclarationStatement targetStateDeclaration = new System.CodeDom.CodeVariableDeclarationStatement();

        
        string mTargetStateType = null;

        public ChangeState(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(RectangleTitle);
            SetUpLinkElement(MethodLink_Pre);

            InitStatementComboBox();

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Pre, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink_Pre.BackBrush, false);
            
            Type paramType = null;
            var linkObj = AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class, ParamSetValue, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            if (paramType != null)
                linkObj.ClassType = paramType;
            linkObj = AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class, ParamGetValue, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ParamGetValue.BackBrush, true);
            if (paramType != null)
                linkObj.ClassType = paramType;


            if (!String.IsNullOrEmpty(strParam))
            {
                var splits = strParam.Split(',');
                paramType = Program.GetType(splits[0]);
                //SetStateParam(paramType);
                mTargetStateType = splits[1];//Program.GetType(splits[1]);
                if (!string.IsNullOrEmpty(mTargetStateType))// != null)
                {
                    //var atts = mTargetStateType.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
                    //AISystem.Attribute.StatementClassAttribute att = atts[0] as AISystem.Attribute.StatementClassAttribute;
                    ////ComboBox_TargetState.SelectedItem = att.m_strName;
                    //foreach (ComboBoxItem comboBoxItem in ComboBox_TargetState.Items)
                    //{
                    //    if (comboBoxItem.Content.ToString() == att.m_strName)
                    //    {
                    //        ComboBox_TargetState.SelectedItem = comboBoxItem;
                    //        break;
                    //    }
                    //}

                    int i=0;
                    foreach (var state in Program.CurrentHostAIInstanceInfo.StateTypes)
                    {
                        if(state == mTargetStateType)
                            break;
                        i++;
                    }

                    ComboBox_TargetState.SelectedIndex = i;
                    
                    //ComboBox_TargetState.SelectedItem = mTargetStateType;
                }

                ValueNameBox.Text = splits[2];
            }

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Next, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink_Next.BackBrush, false);
        }

        private void ClearStateParamLink()
        {
            //foreach (var link in m_linkObjList.Values)
            //{
            //    if (link.m_linkElement == stateParam_Leave || link.m_linkElement == UpLink)
            //        continue;

            //    link.Clear();
            //}
            foreach (var element in mParamRectList)
            {
                var linkObj = GetLinkObjInfo(element);
                linkObj.Clear();
            }
            mParamRectList.Clear();

            foreach (var element in mParamEllipseList)
            {
                var linkObj = GetLinkObjInfo(element);
                linkObj.Clear();
            }
            mParamEllipseList.Clear();

            ParamStack.Children.Clear();

            mValueInfoDic.Clear();

        }

        private void InitStatementComboBox()
        {
            //var stateTypes = Program.GetAllStatementType();
            //foreach (var type in stateTypes)
            //{
            //    var atts = type.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
            //    var att = atts[0] as AISystem.Attribute.StatementClassAttribute;

            //    ComboBoxItem item = new ComboBoxItem()
            //    {
            //        Content = att.m_strName,
            //        Tag = type
            //    };
            //    ComboBox_TargetState.Items.Add(item);
            //}

            //if (ComboBox_TargetState.Items.Count > 0)
            //    ComboBox_TargetState.SelectedIndex = 0;
            if (Program.CurrentHostAIInstanceInfo == null)
                return;

            foreach (var typeName in Program.CurrentHostAIInstanceInfo.StateTypes)
            {
                ComboBoxItem item = new ComboBoxItem()
                {
                    Tag = typeName,
                    Content = Program.CurrentHostAIInstanceInfo.GetStateNickName(typeName)
                };
                ComboBox_TargetState.Items.Add(item);
            }
        }

        private void ComboBox_TargetState_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_TargetState.SelectedIndex < 0)
                return;

            if (Program.CurrentHostAIInstanceInfo == null)
                return;

            var item = ComboBox_TargetState.SelectedItem as ComboBoxItem;
            mTargetStateType = (string)item.Tag;
            var baseType = Program.CurrentHostAIInstanceInfo.GetStateBaseTypeName(mTargetStateType);
            var stateParamType = CSUtility.AISystem.FStateMachineTemplateManager.Instance.GetParameterType(baseType);
            SetStateParam(stateParamType);

            IsDirty = true;
        }

        private void SetStateParam(Type paramType)
        {
            if (paramType == null)
                return;

            ClearStateParamLink();

            var pLObj = GetLinkObjInfo(ParamSetValue);
            if (pLObj != null)
                pLObj.ClassType = paramType;
            pLObj = GetLinkObjInfo(ParamGetValue);
            if (pLObj != null)
                pLObj.ClassType = paramType;

            mStateParameterType = paramType;

            var propertyInfos = paramType.GetProperties();
            foreach (var info in propertyInfos)
            {
                AddParamValue(info);
            }
        }

        private void AddParamValue(System.Reflection.PropertyInfo info)
        {
            Grid grid = new Grid();
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            var ellipse = new CodeGenerateSystem.Controls.LinkInControl()
            {
                Margin = new System.Windows.Thickness(-13, 0, 0, 0),
                Width = 10,
                Height = 10,
                BackBrush = TryFindResource("Link_ValueBrush") as Brush,
                HorizontalAlignment = HorizontalAlignment.Left,
                Direction = CodeGenerateSystem.Base.enBezierType.Left,
            };
            grid.Children.Add(ellipse);
            AddLinkObject(info.PropertyType, ellipse, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, ellipse.BackBrush, false);
            mParamEllipseList.Add(ellipse);

            TextBlock label = new TextBlock()
            {
                Text = info.Name + "(" + info.PropertyType.Name + ")",
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(2)
            };
            grid.Children.Add(label);

            var rect = new CodeGenerateSystem.Controls.LinkOutControl()
            {
                Margin = new Thickness(0, 0, -13, 0),
                Width = 10,
                Height = 10,
                BackBrush = TryFindResource("Link_ValueBrush") as Brush,//new SolidColorBrush(Color.FromRgb(243, 146, 243)),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                Visibility = System.Windows.Visibility.Collapsed,
                Direction = CodeGenerateSystem.Base.enBezierType.Right,
            };
            grid.Children.Add(rect);
            AddLinkObject(info.PropertyType, rect, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, rect.BackBrush, true);
            mParamRectList.Add(rect);

            ParamStack.Children.Add(grid);

            stInLinkData data = new stInLinkData();
            data.propertyInfo = info;
            data.linkElement = ellipse;
            m_InLinkElements[info.Name] = data;

            mValueInfoDic[rect] = info;
        }

#region 储存读取

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            if (mStateParameterType != null)
            {
                StrParams = mStateParameterType.ToString() + "," + mTargetStateType.ToString() + "," + ValueNameBox.Text;
            }

            base.Save(xmlNode, newGuid,holder);
        }

#endregion

#region 代码生成

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (String.IsNullOrEmpty(ValueNameBox.Text))
            {
                string strValueName = "stateParam_" + Program.GetValuedGUIDString(Id);
                return strValueName;
            }
            else
                return ValueNameBox.Text;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == null || element == ParamGetValue)
            {
                return mStateParameterType.ToString();
            }
            else
            {
                var linkOI = GetLinkObjInfo(element);
                if (linkOI.HasLink)
                {
                    System.Reflection.PropertyInfo paramInfo;
                    if (mValueInfoDic.TryGetValue(element, out paramInfo))
                        return paramInfo.PropertyType.ToString();
                }
            }

            return base.GCode_GetValueType(element);
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            if (mStateParameterType == null)
                return;

            string strValueName = GCode_GetValueName(null);

            // 代码: XXXState tagState = Host.AIStates.GetState(XXX);
            var targetStateName = "targetState_" + Program.GetValuedGUIDString(Id);
            if(!codeStatementCollection.Contains(targetStateDeclaration))
            {
                targetStateDeclaration.Type = new System.CodeDom.CodeTypeReference(mTargetStateType);
                targetStateDeclaration.Name = targetStateName;
                targetStateDeclaration.InitExpression = new System.CodeDom.CodeCastExpression(mTargetStateType, new System.CodeDom.CodeSnippetExpression("Host.AIStates.GetState(\"" + mTargetStateType + "\")"));
                codeStatementCollection.Add(targetStateDeclaration);            
            }

            System.CodeDom.CodeExpression stateParamRightExp;
            var linkOI = GetLinkObjInfo(ParamSetValue);
            if (linkOI.HasLink)
            {
                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                {
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));
                }

                // 代码: stateXXXParameter;
                stateParamRightExp = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));
            }
            else
            {
                // 代码: new XXXParameter;
                stateParamRightExp = new System.CodeDom.CodeObjectCreateExpression(mStateParameterType.ToString(), new System.CodeDom.CodeExpression[] { });
            }

            if (!codeStatementCollection.Contains(stateParamDeclaration))
            {
                // 代码: XXXP xxparam =
                if (mStateParameterType == null)
                    mStateParameterType = typeof(CSUtility.AISystem.StateParameter);
                stateParamDeclaration.Type = new System.CodeDom.CodeTypeReference(mStateParameterType);
                stateParamDeclaration.Name = strValueName;
                //stateParamDeclaration.InitExpression = stateParamRightExp;

                codeStatementCollection.Add(stateParamDeclaration);
            }

            // 代码: if(TagState.stateParam == null)
            // 代码:      stateParam = new XXXParameter;
            // 代码: else
            // 代码:      stateParam = TagState.stateParam;
            System.CodeDom.CodeConditionStatement tagStateParamIsNullCondition = new System.CodeDom.CodeConditionStatement();
            tagStateParamIsNullCondition.Condition = new System.CodeDom.CodeBinaryOperatorExpression(
                                new System.CodeDom.CodeSnippetExpression(targetStateName + ".Parameter"),
                                System.CodeDom.CodeBinaryOperatorType.ValueEquality,
                                new System.CodeDom.CodePrimitiveExpression(null));            
            System.CodeDom.CodeAssignStatement stateParamAss = new System.CodeDom.CodeAssignStatement();
            stateParamAss.Left = new System.CodeDom.CodeVariableReferenceExpression(strValueName);
            stateParamAss.Right = stateParamRightExp;
            tagStateParamIsNullCondition.TrueStatements.Add(stateParamAss);
            //codeStatementCollection.Add(stateParamAss);
            System.CodeDom.CodeAssignStatement tagParamAss = new System.CodeDom.CodeAssignStatement(
                                new System.CodeDom.CodeVariableReferenceExpression(strValueName),
                                new System.CodeDom.CodeCastExpression(mStateParameterType, new System.CodeDom.CodeSnippetExpression(targetStateName + ".Parameter")));
            tagStateParamIsNullCondition.FalseStatements.Add(tagParamAss);

            codeStatementCollection.Add(tagStateParamIsNullCondition);

            //if (codeStatementCollection.Contains(stateParamDeclaration))
            //{
            //    // 代码: xxxparam = xxxParam;
            //    System.CodeDom.CodeAssignStatement stateParamAss = new System.CodeDom.CodeAssignStatement();
            //    stateParamAss.Left = new System.CodeDom.CodeVariableReferenceExpression(strValueName);
            //    stateParamAss.Right = stateParamRightExp;
            //    codeStatementCollection.Add(stateParamAss);
            //}
            //else
            //{
            //    // 代码: XXXP xxparam =
            //    if (mStateParameterType == null)
            //        mStateParameterType = typeof(CSUtility.AISystem.StateParameter);
            //    stateParamDeclaration.Type = new System.CodeDom.CodeTypeReference(mStateParameterType);
            //    stateParamDeclaration.Name = strValueName;
            //    //stateParamDeclaration.InitExpression = stateParamRightExp;

            //    codeStatementCollection.Add(stateParamDeclaration);
            //}

            foreach (var param in m_InLinkElements.Keys)
            {
                stInLinkData data = m_InLinkElements[param];
                linkOI = GetLinkObjInfo(data.linkElement);
                if (!linkOI.HasLink)
                    continue;

                System.CodeDom.CodeExpression valueExp = null;
                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                {
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));
                }

                valueExp = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));

                System.CodeDom.CodeFieldReferenceExpression fieldRef = new System.CodeDom.CodeFieldReferenceExpression();
                fieldRef.TargetObject = new System.CodeDom.CodeVariableReferenceExpression(strValueName);
                fieldRef.FieldName = param;

                System.CodeDom.CodeAssignStatement statValAss = new System.CodeDom.CodeAssignStatement();
                statValAss.Left = fieldRef;
                statValAss.Right = valueExp;

                codeStatementCollection.Add(statValAss);
            }

            if (mTargetStateType != null)
            {
                System.CodeDom.CodeMethodInvokeExpression methodInvoke = new System.CodeDom.CodeMethodInvokeExpression(
                    new System.CodeDom.CodeThisReferenceExpression(), "ToState",
                    new System.CodeDom.CodeExpression[] {
                        new System.CodeDom.CodeSnippetExpression("\"" + mTargetStateType + "\""),
                        new System.CodeDom.CodeVariableReferenceExpression(strValueName)});

                codeStatementCollection.Add(methodInvoke);
            }

            var methodLinkNextOI = GetLinkObjInfo(MethodLink_Next);
            if(methodLinkNextOI.HasLink)
            {
                methodLinkNextOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, methodLinkNextOI.GetLinkElement(0, false));
            }
        }

#endregion
    }
}
