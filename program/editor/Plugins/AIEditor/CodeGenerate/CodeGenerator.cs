using System;
using System.Collections.Generic;
using System.Windows;

namespace AIEditor.CodeGenerate
{
    public class CodeGenerator
    {
        //protected string mCodeNameSpace = "AIInstance";
        //public string CodeNameSpace
        //{
        //    get { return mCodeNameSpace; }
        //}

        private static string GetDelegateMethodName(string curStateName, string tagStateName, StateMethodsEditorControl.enMethodDelegateEditType editType, string methodName)
        {
            switch (editType)
            {
                case StateMethodsEditorControl.enMethodDelegateEditType.Default:
                    return "Default_" + methodName;

                case StateMethodsEditorControl.enMethodDelegateEditType.CurrentState:
                    return "ChangeStateTo" + tagStateName + "_" + methodName;

                case StateMethodsEditorControl.enMethodDelegateEditType.TargetState:
                    return "ChangeStateFrom" + curStateName + "_" + methodName;

                case StateMethodsEditorControl.enMethodDelegateEditType.SelfChange:
                    return "ChangeToSelfState_" + methodName;
            }

            return "";
        }

        private static string GetStatementTypeName(string stateType, AIEditor.FSMTemplateInfo info)
        {
            return stateType;
            //return stateType.Name;// +"_" + Program.GetValuedGUIDString(info.Id);
        }

        private static void GenerateStatementCode(System.CodeDom.CodeNamespace nameSpace, AIEditor.FSMTemplateInfo info, CSUtility.Helper.enCSType csType)
        {
            foreach (var stateType in info.StateTypes)
            {
                System.CodeDom.CodeTypeDeclaration stateClass = new System.CodeDom.CodeTypeDeclaration(GetStatementTypeName(stateType, info));
                stateClass.IsClass = true;
                stateClass.BaseTypes.Add(info.GetStateBaseType(stateType, csType).ToString());
                
                var stateAttName = info.GetStateNickName(stateType);
                //var atts = stateType.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
                //if (atts.Length > 0)
                //{
                //    System.CodeDom.CodeComment cmt = new System.CodeDom.CodeComment(((AISystem.Attribute.StatementClassAttribute)atts[0]).m_strName);
                //    System.CodeDom.CodeCommentStatement cms = new System.CodeDom.CodeCommentStatement(cmt);
                //    stateClass.Comments.Add(cms);                
                //}
                // 代码中添加状态说明注释
                var cmt = new System.CodeDom.CodeComment(stateAttName);
                var cms = new System.CodeDom.CodeCommentStatement(cmt);
                stateClass.Comments.Add(cms);
                
                // 构造函数
                System.CodeDom.CodeConstructor stateConstructor = new System.CodeDom.CodeConstructor();
                stateConstructor.Attributes = System.CodeDom.MemberAttributes.Public;
                // 代码: mStateName = XXX
                System.CodeDom.CodeAssignStatement stateNameAss = new System.CodeDom.CodeAssignStatement(
                                                new System.CodeDom.CodeVariableReferenceExpression("mStateName"),
                                                new System.CodeDom.CodeSnippetExpression("\"" + stateType + "\""));
                stateConstructor.Statements.Add(stateNameAss);
                stateClass.Members.Add(stateConstructor);

                // 状态属性
                foreach (var proInfo in info.GetStatePropertys(stateType))
                {
                    //var ppCode = new System.CodeDom.CodeMemberField

                    //var propertyCode = new System.CodeDom.CodeMemberProperty();
                    //propertyCode.Type = new System.CodeDom.CodeTypeReference(proInfo.PropertyType);
                    //propertyCode.Name = proInfo.PropertyName;
                    //propertyCode.Attributes = System.CodeDom.MemberAttributes.Public;
                    //propertyCode.GetStatements.Add(new System.CodeDom.CodeMethodReturnStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeThisReferenceExpression(), "testStringField")));
                    //propertyCode.SetStatements.Add(new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeThisReferenceExpression(), "testStringField"), new System.CodeDom.CodePropertySetValueReferenceExpression()));
                    var propertyCode = new System.CodeDom.CodeMemberField(proInfo.PropertyType, proInfo.PropertyName);
                    propertyCode.Attributes = System.CodeDom.MemberAttributes.Public;
                    propertyCode.InitExpression = new System.CodeDom.CodePrimitiveExpression(proInfo.DefaultValue);
                    stateClass.Members.Add(propertyCode);
                }

                List<System.Reflection.MethodInfo> delegatedMethods = new List<System.Reflection.MethodInfo>();
                List<System.Reflection.MethodInfo> generatedDefaultMethods = new List<System.Reflection.MethodInfo>();

                var ctrl = new CodeGenerateSystem.Controls.NodesContainerControl();

                // 默认函数代理实现
                var methodClassType = info.GetStateBaseType(stateType, csType);
                foreach (var method in methodClassType.GetMethods())// stateType.GetMethods())
                {
                    var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(method, typeof(CSUtility.AISystem.Attribute.OverrideInterface).FullName, true);
                    if (att == null)
                        continue;

                    var attCSTypeStr = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();

                    switch(csType)
                    {
                        case CSUtility.Helper.enCSType.Client:
                            {
                                if (attCSTypeStr.Equals(CSUtility.Helper.enCSType.Server.ToString()))
                                    continue;
                            }
                            break;

                        case CSUtility.Helper.enCSType.Server:
                            {
                                if (attCSTypeStr.Equals(CSUtility.Helper.enCSType.Client.ToString()))
                                    continue;
                            }
                            break;

                        case CSUtility.Helper.enCSType.Common:
                            continue;
                    }

                    CSUtility.Support.XmlHolder xmlHolder = null;
                    if (!info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(stateType, null, methodClassType, method, csType), out xmlHolder))
                    {
                        // 没有找到特定服务器或客户端的代码则生成Common的代码
                        if (!info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(stateType, null, methodClassType, method, CSUtility.Helper.enCSType.Common), out xmlHolder))
                            continue;
                    }
                    
                    if (!delegatedMethods.Contains(method))
                        delegatedMethods.Add(method);

                    if (!generatedDefaultMethods.Contains(method))
                        generatedDefaultMethods.Add(method);

                    ctrl.LoadXML(xmlHolder);

                    foreach (var origionNode in ctrl.OrigionNodeControls)
                    {
                        if (origionNode is CodeDomNode.MethodNode)
                        {
                            origionNode.GCode_CodeDom_GenerateCode(stateClass, null);
                            break;
                        }
                    }
                }
                //// 函数重载生成
                //foreach (var method in stateType.GetMethods())
                //{
                //    var methodAtts = method.GetCustomAttributes(typeof(AISystem.Attribute.OverrideInterface), true);
                //    if(methodAtts.Length <= 0)
                //        continue;

                //    CSUtility.Support.XmlHolder xmlHolder = null;
                //    if(!info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(stateType, method), out xmlHolder))
                //        continue;

                //    ctrl.LoadXML(xmlHolder);
                    
                //    foreach (var origionNode in ctrl.OrigionNodeControls)
                //    {
                //        if (origionNode is CodeDomNode.MethodNode)
                //        {
                //            origionNode.GCode_CodeDom_GenerateCode(stateClass, null);
                //            break;
                //        }
                //    }
                //}


                // override OnChangeStateTo
                System.CodeDom.CodeMemberMethod changeStateToMethodCode = new System.CodeDom.CodeMemberMethod();
                changeStateToMethodCode.Attributes = System.CodeDom.MemberAttributes.Override | System.CodeDom.MemberAttributes.Public;
                changeStateToMethodCode.Name = "System_OnChangeStateTo";
                changeStateToMethodCode.ReturnType = new System.CodeDom.CodeTypeReference(typeof(void));
                changeStateToMethodCode.Parameters.Add(new System.CodeDom.CodeParameterDeclarationExpression(typeof(CSUtility.AISystem.StateParameter), "param"));
                //changeStateToMethodCode.Parameters.Add(new System.CodeDom.CodeParameterDeclarationExpression(typeof(AISystem.StateSet), "stateSet"));
                changeStateToMethodCode.Parameters.Add(new System.CodeDom.CodeParameterDeclarationExpression(typeof(System.String), "stateType"));

                // 代码: State newState = Host.AIStates.GetState(stateType);
                var newStateDeclaration = new System.CodeDom.CodeVariableDeclarationStatement(
                                                        typeof(CSUtility.AISystem.State), "newState", 
                                                        new System.CodeDom.CodeMethodInvokeExpression(
                                                                        new System.CodeDom.CodeVariableReferenceExpression("Host.AIStates"),
                                                                        "GetState",
                                                                        new System.CodeDom.CodeExpression[] {
                                                                                    new System.CodeDom.CodeVariableReferenceExpression("stateType") }));
                changeStateToMethodCode.Statements.Add(newStateDeclaration);
                // 代码: if(newState == null) return;
                var newStateCondStatement = new System.CodeDom.CodeConditionStatement();
                newStateCondStatement.Condition = new System.CodeDom.CodeBinaryOperatorExpression(
                                                                new System.CodeDom.CodeVariableReferenceExpression("newState"),
                                                                System.CodeDom.CodeBinaryOperatorType.ValueEquality,
                                                                new System.CodeDom.CodePrimitiveExpression(null));
                newStateCondStatement.TrueStatements.Add(new System.CodeDom.CodeMethodReturnStatement());
                changeStateToMethodCode.Statements.Add(newStateCondStatement);
                bool bChangeStateHasCode = false;

                // 状态转换
                foreach (var targetStateType in info.StateTypes)
                {
                    List<System.Reflection.MethodInfo> toTargetMethodDelegates = new List<System.Reflection.MethodInfo>();
                    List<System.Reflection.MethodInfo> fromCurrentMethodDelegates = new List<System.Reflection.MethodInfo>();

                    foreach (var method in methodClassType.GetMethods())
                    {
                        var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(method, typeof(CSUtility.AISystem.Attribute.OverrideInterface).FullName, true);
                        if(att == null)
                            continue;

                        var attCSTypeStr = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();

                        switch (csType)
                        {
                            case CSUtility.Helper.enCSType.Client:
                                {
                                    if (attCSTypeStr.Equals(CSUtility.Helper.enCSType.Server.ToString()))
                                        continue;
                                }
                                break;

                            case CSUtility.Helper.enCSType.Server:
                                {
                                    if (attCSTypeStr.Equals(CSUtility.Helper.enCSType.Client.ToString()))
                                        continue;
                                }
                                break;

                            case CSUtility.Helper.enCSType.Common:
                                continue;
                        }

                        CSUtility.Support.XmlHolder xmlHolder = null;
                        if (!info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(stateType, targetStateType, methodClassType, method, csType), out xmlHolder))
                        {
                            info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(stateType, targetStateType, methodClassType, method, CSUtility.Helper.enCSType.Common), out xmlHolder);
                        }
                        
                        if(xmlHolder != null)
                        {
                            if (!delegatedMethods.Contains(method))
                                delegatedMethods.Add(method);

                            toTargetMethodDelegates.Add(method);

                            ctrl.LoadXML(xmlHolder);

                            foreach (var origionNode in ctrl.OrigionNodeControls)
                            {
                                if (origionNode is CodeDomNode.MethodNode)
                                {
                                    origionNode.GCode_CodeDom_GenerateCode(stateClass, null);
                                    break;
                                }
                            }
                        }

                        if (stateType != targetStateType)
                        {
                            xmlHolder = null;
                            if (!info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(targetStateType, stateType, methodClassType, method, csType), out xmlHolder))
                            {
                                info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(targetStateType, stateType, methodClassType, method, CSUtility.Helper.enCSType.Common), out xmlHolder);
                            }

                            if (xmlHolder != null)
                            {
                                if (!delegatedMethods.Contains(method))
                                    delegatedMethods.Add(method);

                                ctrl.LoadXML(xmlHolder);

                                foreach (var origionNode in ctrl.OrigionNodeControls)
                                {
                                    if (origionNode is CodeDomNode.MethodNode)
                                    {
                                        origionNode.GCode_CodeDom_GenerateCode(stateClass, null);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    var tagMethodType = info.GetStateBaseType(targetStateType, csType);
                    if (tagMethodType == null)
                        continue;
                    foreach (var method in tagMethodType.GetMethods())//targetStateType.GetMethods())
                    {
                        var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(method, typeof(CSUtility.AISystem.Attribute.OverrideInterface).FullName, true);
                        if (att == null)
                            continue;

                        var csTypeStr = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();

                        switch (csType)
                        {
                            case CSUtility.Helper.enCSType.Client:
                                {
                                    if (csTypeStr.Equals(CSUtility.Helper.enCSType.Server.ToString()))
                                        continue;
                                }
                                break;

                            case CSUtility.Helper.enCSType.Server:
                                {
                                    if (csTypeStr.Equals(CSUtility.Helper.enCSType.Client.ToString()))
                                        continue;
                                }
                                break;
                        }

                        CSUtility.Support.XmlHolder xmlHolder = null;
                        if (info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(stateType, targetStateType, tagMethodType, method, csType), out xmlHolder))
                        {
                            fromCurrentMethodDelegates.Add(method);
                        }
                        else if (info.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(stateType, targetStateType, tagMethodType, method, CSUtility.Helper.enCSType.Common), out xmlHolder))
                            fromCurrentMethodDelegates.Add(method);
                    }

                    // 代码: if(stateType == XXXType)
                    System.CodeDom.CodeConditionStatement condStatement = new System.CodeDom.CodeConditionStatement();
                    condStatement.Condition = new System.CodeDom.CodeBinaryOperatorExpression(
                                                            new System.CodeDom.CodeTypeReferenceExpression("stateType"),
                                                            System.CodeDom.CodeBinaryOperatorType.ValueEquality,
                                                            new System.CodeDom.CodeSnippetExpression("\"" + targetStateType + "\""));//.Name));

                    bool bHasCode = false;

                    if (stateType == targetStateType)
                    {
                        foreach (var toEvent in toTargetMethodDelegates)
                        {
                            // 代码: OnXXXEvent = SelfXXX_OnXXX;(代理设置)
                            var codeSetStatement = new System.CodeDom.CodeAssignStatement(
                                                            new System.CodeDom.CodeFieldReferenceExpression(
                                                                            new System.CodeDom.CodeThisReferenceExpression(),
                                                                            toEvent.Name + "Event"),
                                                            new System.CodeDom.CodeMethodReferenceExpression(
                                                                            new System.CodeDom.CodeThisReferenceExpression(),
                                                                            GetDelegateMethodName(stateType, targetStateType, StateMethodsEditorControl.enMethodDelegateEditType.SelfChange, toEvent.Name)));
                            condStatement.TrueStatements.Add(codeSetStatement);
                            bHasCode = true;
                        }
                    }
                    else
                    {
                        foreach (var toEvent in toTargetMethodDelegates)
                        {
                            // 代码: OnXXXEvent = ToXXX_OnXXX;(代理设置)
                            var codeSetStatement = new System.CodeDom.CodeAssignStatement(
                                                            new System.CodeDom.CodeFieldReferenceExpression(
                                                                            new System.CodeDom.CodeThisReferenceExpression(),
                                                                            toEvent.Name + "Event"),
                                                            new System.CodeDom.CodeMethodReferenceExpression(
                                                                            new System.CodeDom.CodeThisReferenceExpression(),
                                                                            GetDelegateMethodName(stateType, targetStateType, StateMethodsEditorControl.enMethodDelegateEditType.CurrentState, toEvent.Name)));
                            condStatement.TrueStatements.Add(codeSetStatement);
                            bHasCode = true;
                        }
                        foreach (var fromEvent in fromCurrentMethodDelegates)
                        {
                            // 代码: OnXXXEvent = XX.FromXXX_OnXXX;(代理设置)
                            var codeSetStatement = new System.CodeDom.CodeAssignStatement(
                                                            new System.CodeDom.CodeFieldReferenceExpression(
                                                                            new System.CodeDom.CodeCastExpression(
                                                                                    new System.CodeDom.CodeTypeReference(targetStateType), 
                                                                                    new System.CodeDom.CodeVariableReferenceExpression("newState")),
                                                                            fromEvent.Name + "Event"),
                                                            new System.CodeDom.CodeMethodReferenceExpression(
                                                                            new System.CodeDom.CodeCastExpression(
                                                                                    new System.CodeDom.CodeTypeReference(targetStateType), 
                                                                                    new System.CodeDom.CodeVariableReferenceExpression("newState")),
                                                                            GetDelegateMethodName(stateType, targetStateType, StateMethodsEditorControl.enMethodDelegateEditType.TargetState, fromEvent.Name)));
                            condStatement.TrueStatements.Add(codeSetStatement);
                            bHasCode = true;
                        }
                    }

                    if (bHasCode)
                    {
                        changeStateToMethodCode.Statements.Add(condStatement);
                        bChangeStateHasCode = true;
                    }
                }
                if (bChangeStateHasCode)
                    stateClass.Members.Add(changeStateToMethodCode);

                // 添加默认的代理初始化函数
                var initDefaultMethodCode = new System.CodeDom.CodeMemberMethod();
                initDefaultMethodCode.Attributes = System.CodeDom.MemberAttributes.Override | System.CodeDom.MemberAttributes.Public;
                initDefaultMethodCode.Name = "InitializeDefaultDelegate";
                initDefaultMethodCode.ReturnType = new System.CodeDom.CodeTypeReference(typeof(void));
                bool bInitDefaultHasCode = false;

                foreach (var delegateMethod in delegatedMethods)
                {
                    string retType = "void";
                    if (delegateMethod.ReturnType != typeof(void))
                        retType = delegateMethod.ReturnType.FullName;
                    var delegateStr = "        public delegate " + retType + " Delegate_" + delegateMethod.Name + "(";
                    foreach (var param in delegateMethod.GetParameters())
                    {
                        if (param.IsOut)
                            delegateStr += "out ";
                        else if (param.IsRetval)
                            delegateStr += "ref ";
                        delegateStr += param.ParameterType.ToString() + " ";
                        delegateStr += param.Name + ",";
                    }
                    if(delegateStr.LastIndexOf(',') == delegateStr.Length - 1)
                        delegateStr = delegateStr.Remove(delegateStr.Length - 1);
                    delegateStr += ");";
                    stateClass.Members.Add(new System.CodeDom.CodeSnippetTypeMember(delegateStr));
                    stateClass.Members.Add(new System.CodeDom.CodeSnippetTypeMember(
                                "        public Delegate_" + delegateMethod.Name + " " + delegateMethod.Name + "Event;"));
                
                    // 函数重载
                    var methodOverride = new System.CodeDom.CodeMemberMethod();
                    methodOverride.Attributes = System.CodeDom.MemberAttributes.Override | System.CodeDom.MemberAttributes.Public;
                    methodOverride.Name = delegateMethod.Name;
                    methodOverride.ReturnType = new System.CodeDom.CodeTypeReference(delegateMethod.ReturnType);
                    foreach (var param in delegateMethod.GetParameters())
                    {
                        methodOverride.Parameters.Add(new System.CodeDom.CodeParameterDeclarationExpression(param.ParameterType, param.Name));
                    }

                    var tempCondition = new System.CodeDom.CodeConditionStatement();
                    tempCondition.Condition = new System.CodeDom.CodeBinaryOperatorExpression(
                                                            new System.CodeDom.CodeEventReferenceExpression(new System.CodeDom.CodeThisReferenceExpression(), delegateMethod.Name + "Event"),
                                                            System.CodeDom.CodeBinaryOperatorType.IdentityInequality,
                                                            new System.CodeDom.CodePrimitiveExpression(null));
                    
                    var paramCodeExp = new System.CodeDom.CodeExpression[delegateMethod.GetParameters().Length];
                    int idx = 0;
                    foreach (var param in delegateMethod.GetParameters())
                    {
                        paramCodeExp[idx] = new System.CodeDom.CodeVariableReferenceExpression(param.Name);
                        idx++;
                    }

                    if (retType == "void")
                    {
                        tempCondition.TrueStatements.Add(new System.CodeDom.CodeDelegateInvokeExpression(
                                                                    new System.CodeDom.CodeEventReferenceExpression(new System.CodeDom.CodeThisReferenceExpression(), delegateMethod.Name + "Event"),
                                                                    paramCodeExp));
                    }
                    else
                    {
                        tempCondition.TrueStatements.Add(new System.CodeDom.CodeMethodReturnStatement(
                                                                    new System.CodeDom.CodeDelegateInvokeExpression(
                                                                    new System.CodeDom.CodeEventReferenceExpression(new System.CodeDom.CodeThisReferenceExpression(), delegateMethod.Name + "Event"),
                                                                    paramCodeExp)));
                    }

                    methodOverride.Statements.Add(tempCondition);

                    if (retType != "void")
                    {
                        // 返回默认值
                        if (delegateMethod.ReturnType.IsEnum)
                        {
                            methodOverride.Statements.Add(new System.CodeDom.CodeMethodReturnStatement(new System.CodeDom.CodePrimitiveExpression(System.Enum.GetNames(delegateMethod.ReturnType)[0])));
                        }
                        else if(delegateMethod.ReturnType == typeof(bool))
                        {
                            methodOverride.Statements.Add(new System.CodeDom.CodeMethodReturnStatement(new System.CodeDom.CodePrimitiveExpression(false)));
                        }
                        else if (delegateMethod.ReturnType == typeof(string))
                        {
                            methodOverride.Statements.Add(new System.CodeDom.CodeMethodReturnStatement(new System.CodeDom.CodePrimitiveExpression("")));
                        }
                        else if (delegateMethod.ReturnType.IsClass)
                        {
                            methodOverride.Statements.Add(new System.CodeDom.CodeMethodReturnStatement(new System.CodeDom.CodePrimitiveExpression(null)));
                        }
                        else
                        {
                            methodOverride.Statements.Add(new System.CodeDom.CodeMethodReturnStatement(new System.CodeDom.CodeCastExpression(retType, new System.CodeDom.CodePrimitiveExpression(0))));
                        }
                    }

                    stateClass.Members.Add(methodOverride);

                    // defaultMethod
                    if (!generatedDefaultMethods.Contains(delegateMethod))
                    {
                        var defaultMethod = new System.CodeDom.CodeMemberMethod();
                        defaultMethod.Attributes = System.CodeDom.MemberAttributes.Public;
                        defaultMethod.Name = GetDelegateMethodName("", "", StateMethodsEditorControl.enMethodDelegateEditType.Default, delegateMethod.Name);
                        defaultMethod.ReturnType = new System.CodeDom.CodeTypeReference(delegateMethod.ReturnType);
                        foreach (var param in delegateMethod.GetParameters())
                        {
                            defaultMethod.Parameters.Add(new System.CodeDom.CodeParameterDeclarationExpression(param.ParameterType, param.Name));
                        }

                        // 代码: base.XXXXX(XXX);
                        var paramExp = new System.CodeDom.CodeExpression[delegateMethod.GetParameters().Length];
                        int i = 0;
                        foreach (var param in delegateMethod.GetParameters())
                        {
                            paramExp[idx] = new System.CodeDom.CodeVariableReferenceExpression(param.Name);
                            i++;
                        }
                        defaultMethod.Statements.Add(new System.CodeDom.CodeMethodInvokeExpression(
                                                                new System.CodeDom.CodeBaseReferenceExpression(),
                                                                delegateMethod.Name,
                                                                paramExp));
                        stateClass.Members.Add(defaultMethod);
                    }

                    // initDefaultMethod
                    // 代码: XXXEvent = Default_XXX;
                    initDefaultMethodCode.Statements.Add(new System.CodeDom.CodeAssignStatement(
                                                                        new System.CodeDom.CodeFieldReferenceExpression(
                                                                            new System.CodeDom.CodeThisReferenceExpression(),
                                                                            delegateMethod.Name + "Event"),
                                                                        new System.CodeDom.CodeMethodReferenceExpression(
                                                                            new System.CodeDom.CodeThisReferenceExpression(),
                                                                            GetDelegateMethodName("", "", StateMethodsEditorControl.enMethodDelegateEditType.Default, delegateMethod.Name))));

                    bInitDefaultHasCode = true;
                }

                if (bInitDefaultHasCode)
                    stateClass.Members.Add(initDefaultMethodCode);

                //foreach (var targetStateType in info.StateTypes)
                //{
                //    var chgKey = AIEditor.FSMTemplateInfo.GetStateChangeDictionaryKey(stateType, targetStateType);
                //    CSUtility.Support.XmlHolder xmlHolder = null;
                //    if (!info.StateChangeXmlHolders.TryGetValue(chgKey, out xmlHolder))
                //        continue;

                //    ctrl.LoadXML(xmlHolder);

                //    System.CodeDom.CodeConditionStatement condStatement = new System.CodeDom.CodeConditionStatement();
                //    condStatement.Condition = new System.CodeDom.CodeBinaryOperatorExpression(
                //                                            new System.CodeDom.CodeTypeOfExpression("stateType"),
                //                                            System.CodeDom.CodeBinaryOperatorType.ValueEquality,
                //                                            new System.CodeDom.CodeTypeReferenceExpression(targetStateType.Name));
                //    changeStateToMethodCode.Statements.Add(condStatement);

                //    foreach (var origionNode in ctrl.OrigionNodeControls)
                //    {
                //        if (origionNode is LinkSystem.Control.MethodListNode)
                //        {
                //            origionNode.GCode_CodeDom_GenerateCode(stateClass, condStatement.TrueStatements, null);
                //            break;
                //        }
                //    }
                //}

                nameSpace.Types.Add(stateClass);
            }
        }

        public static System.IO.TextWriter GenerateCode(AIEditor.FSMTemplateInfo info, CSUtility.Helper.enCSType csType)
        {
            try
            {
                string codeNameSpace = CSUtility.AISystem.FStateMachineTemplate.GetFSMNameSpace(info.Id, csType);//"AIInstance_" + Program.GetValuedGUIDString(info.Id) + "_" + csType.ToString();

                System.CodeDom.Compiler.CodeDomProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
                System.CodeDom.CodeNamespace nameSpace = new System.CodeDom.CodeNamespace(codeNameSpace);

                System.CodeDom.Compiler.CodeGeneratorOptions option = new System.CodeDom.Compiler.CodeGeneratorOptions();
                option.BlankLinesBetweenMembers = false;
                option.BracingStyle = "C";
                option.IndentString = "    ";
                option.ElseOnClosing = false;
                option.VerbatimOrder = true;

                //string codeAIClassName = "AIInstance";// +Program.GetValuedGUIDString(info.Id);
                //string codeHostClassName = "AIInstanceStateSet";
                //string codeHostType = codeNameSpace + "." + codeHostClassName;
                string codeStateSwitchClassName = "StateSwitchDataIns";

                // Statements
                GenerateStatementCode(nameSpace, info, csType);

                // StateSwitchDataInsClass========================================================
                System.CodeDom.CodeTypeDeclaration stateSwitchClass = new System.CodeDom.CodeTypeDeclaration(codeStateSwitchClassName);
                stateSwitchClass.IsClass = true;
                stateSwitchClass.BaseTypes.Add(typeof(CSUtility.AISystem.FStateMachineTemplate.StateSwitchData));

                // 构造函数
                System.CodeDom.CodeConstructor constructorMethod = new System.CodeDom.CodeConstructor();
                constructorMethod.Attributes = System.CodeDom.MemberAttributes.Public;
                stateSwitchClass.Members.Add(constructorMethod);

                // 设置默认状态
                // 代码: mDefaultState = "XXX";
                System.CodeDom.CodeAssignStatement defaultStateAss = new System.CodeDom.CodeAssignStatement(
                                                new System.CodeDom.CodeVariableReferenceExpression("mDefaultState"),
                                                new System.CodeDom.CodeSnippetExpression("\"" + info.DefaultStateTypeName + "\""));
                constructorMethod.Statements.Add(defaultStateAss);

                // 状态转换
                string strKeyValName = "key";
                string strInfoValName = "info";
                System.CodeDom.CodeVariableDeclarationStatement variableDeclaration = new System.CodeDom.CodeVariableDeclarationStatement();
                variableDeclaration.Type = new System.CodeDom.CodeTypeReference(typeof(System.Collections.Generic.KeyValuePair<string, string>));
                variableDeclaration.Name = strKeyValName;
                constructorMethod.Statements.Add(variableDeclaration);
                variableDeclaration = new System.CodeDom.CodeVariableDeclarationStatement();
                variableDeclaration.Type = new System.CodeDom.CodeTypeReference(typeof(CSUtility.AISystem.FStateMachineTemplate.StateSwitchData.SwitchInfo));
                variableDeclaration.Name = strInfoValName;
                constructorMethod.Statements.Add(variableDeclaration);

                foreach (var kvInfo in info.StateSwitchManager)
                {
                    // 代码: key = new System.Collections.Generic.KeyValuePair<string, string>("XXX1", "XXX2");
                    System.CodeDom.CodeAssignStatement keyAss = new System.CodeDom.CodeAssignStatement();
                    keyAss.Left = new System.CodeDom.CodeVariableReferenceExpression(strKeyValName);
                    var kCreate = new System.CodeDom.CodeObjectCreateExpression();
                    kCreate.CreateType = new System.CodeDom.CodeTypeReference(typeof(System.Collections.Generic.KeyValuePair<string, string>));
                    kCreate.Parameters.Add(new System.CodeDom.CodeSnippetExpression("\"" + kvInfo.Key.Key + "\""));
                    kCreate.Parameters.Add(new System.CodeDom.CodeSnippetExpression("\"" + kvInfo.Key.Value + "\""));                                                             
                    keyAss.Right = kCreate;
                    constructorMethod.Statements.Add(keyAss);

                    // 代码: info = new CSUtility.AISystem.FStateMachineTemplate template.StateSwitchData.SwitchInfo("XXX", "XXX");
                    System.CodeDom.CodeAssignStatement infoAss = new System.CodeDom.CodeAssignStatement();
                    infoAss.Left = new System.CodeDom.CodeVariableReferenceExpression(strInfoValName);
                    var infoCreate = new System.CodeDom.CodeObjectCreateExpression();
                    infoCreate.CreateType = new System.CodeDom.CodeTypeReference(typeof(CSUtility.AISystem.FStateMachineTemplate.StateSwitchData.SwitchInfo));
                    infoCreate.Parameters.Add(new System.CodeDom.CodeSnippetExpression("\"" + kvInfo.Value.NewCurrentStateType + "\""));
                    if (string.IsNullOrEmpty(kvInfo.Value.NewTargetStateType))
                    {
                        infoCreate.Parameters.Add(new System.CodeDom.CodePrimitiveExpression(null));
                    }
                    else
                        infoCreate.Parameters.Add(new System.CodeDom.CodeSnippetExpression("\"" + kvInfo.Value.NewTargetStateType + "\""));
                    infoAss.Right = infoCreate;
                    constructorMethod.Statements.Add(infoAss);

                    // 代码: mStateSwitchInfoData.Add(key, info);
                    System.CodeDom.CodeMethodInvokeExpression mIvk = new System.CodeDom.CodeMethodInvokeExpression(
                                                    new System.CodeDom.CodeVariableReferenceExpression("mStateSwitchInfoData"),
                                                    "Add",
                                                    new System.CodeDom.CodeExpression[] {
                                                        new System.CodeDom.CodeVariableReferenceExpression(strKeyValName),
                                                        new System.CodeDom.CodeVariableReferenceExpression(strInfoValName)
                                                    });
                    constructorMethod.Statements.Add(mIvk);
                }

                nameSpace.Types.Add(stateSwitchClass);


                System.IO.TextWriter tw = new System.IO.StringWriter();
                tw.WriteLine("// Vise Engine!\n");
                tw.WriteLine("// AI编辑器生成代码");

                try
                {
                    codeProvider.GenerateCodeFromNamespace(nameSpace, tw, option);
                }
                catch (System.Exception ex)
                {
                    EditorCommon.MessageBox.Show(ex.ToString());
                }

                return tw;
            }
            catch (System.Exception ex)
            {
                EditorCommon.MessageBox.Show(ex.ToString());
            }

            System.IO.TextWriter retTw = new System.IO.StringWriter();
            retTw.WriteLine("// Vise Engine!\n");
            retTw.WriteLine("// AI编辑器生成代码错误!");
            return retTw;
        }

        public static System.CodeDom.Compiler.CompilerResults CompileCode(string codeStr, CSUtility.Helper.enCSType csType, string dllOutputFile = "", bool debug = false, Guid aiId = new Guid())
        {
            System.CodeDom.Compiler.CodeDomProvider cdProvider = new Microsoft.CSharp.CSharpCodeProvider();

            System.CodeDom.Compiler.CompilerParameters compilerParam = new System.CodeDom.Compiler.CompilerParameters();
            compilerParam.GenerateExecutable = false;
            compilerParam.GenerateInMemory = false;

            compilerParam.ReferencedAssemblies.Add("System.dll");
            //compilerParam.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            //compilerParam.ReferencedAssemblies.Add("CSUtility.dll");

            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    compilerParam.ReferencedAssemblies.Add(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Client_Directory + "/ClientCommon.dll");
                    compilerParam.ReferencedAssemblies.Add(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Client_Directory + "/Client.dll");
                    break;

                case CSUtility.Helper.enCSType.Server:
                    compilerParam.ReferencedAssemblies.Add(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Server_Directory + "/ServerCommon.dll");
                    compilerParam.ReferencedAssemblies.Add(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Server_Directory + "/Server.dll");
                    break;
            }


            if (!string.IsNullOrEmpty(dllOutputFile))
                compilerParam.OutputAssembly = dllOutputFile;

            System.CodeDom.Compiler.CompilerResults compilerResult = null;

            if (debug == true)
            {
                compilerParam.IncludeDebugInformation = true;

                var fileDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultFSMDirectory + "\\" + Program.CodeFilesFolderName;
                var fileName = fileDir + "\\" + CSUtility.AISystem.FStateMachineTemplate.GetAssemblyFileName(aiId, csType, true) + ".cs";

                if (!System.IO.Directory.Exists(fileDir))
                    System.IO.Directory.CreateDirectory(fileDir);
                using(var tw = new System.IO.StreamWriter(fileName))
                {
                    tw.Write(codeStr);
                }
                //fileName = "../" + CSUtility.Support.IFileConfig.DefaultFSMDirectory + "/CodeFiles/" + aiId.ToString() + ".cs";
                //fileName = "D:/victory/Development/Program/Client/Vitamin/AIEditor/Program.cs";
                fileName = fileName.Replace("/", "\\");
                compilerResult = cdProvider.CompileAssemblyFromFile(compilerParam, new string[] { fileName });
            }
            else
                compilerResult = cdProvider.CompileAssemblyFromSource(compilerParam, codeStr);

            return compilerResult;
        }
    }
}
