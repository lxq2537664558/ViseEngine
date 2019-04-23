using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for MethodNode.xaml
    /// </summary>
    public partial class MethodNode : CodeGenerateSystem.Base.BaseNodeControl, CodeGenerateSystem.Base.UsefulMember
    {
        public List<CodeGenerateSystem.Base.UsefulMemberHostData> GetUsefulMembers()
        {
            List<CodeGenerateSystem.Base.UsefulMemberHostData> retValue = new List<CodeGenerateSystem.Base.UsefulMemberHostData>();

            if (OverrideAble)
            {
                retValue.AddRange(ParamUsefulDatas);
            }
            else if(Useable)
            {
                var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
                {
                    ClassTypeFullName = MethodReturnType.FullName,
                    HostControl = this,
                    LinkObject = mReturnLinkObjInfo,
                };

                retValue.Add(memberData);
            }

            return retValue;
        }

        public List<CodeGenerateSystem.Base.UsefulMemberHostData> GetUsefulMembers(CodeGenerateSystem.Base.LinkControl linkCtrl)
        {
            List<CodeGenerateSystem.Base.UsefulMemberHostData> retValue = new List<CodeGenerateSystem.Base.UsefulMemberHostData>();
            if(OverrideAble)
            {
                foreach(var data in ParamUsefulDatas)
                {
                    if(data.LinkObject.LinkElement == linkCtrl)
                    {
                        retValue.Add(data);
                    }
                }
            }
            else if(Useable)
            {
                if(linkCtrl == ReturnValueLink)
                {
                    var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
                    {
                        ClassTypeFullName = MethodReturnType.FullName,
                        HostControl = this,
                        LinkObject = mReturnLinkObjInfo,
                    };

                    retValue.Add(memberData);
                }
            }

            return retValue;
        }

        // 返回值类型
        Type mReturnType;
        public Type MethodReturnType
        {
            get { return mReturnType; }
        }

        CodeGenerateSystem.Base.LinkObjInfo mReturnLinkObjInfo;

        // 参数数据
        public List<CodeGenerateSystem.Base.UsefulMemberHostData> ParamUsefulDatas
        {
            get;
            protected set;
        } = new List<CodeGenerateSystem.Base.UsefulMemberHostData>();

        Dictionary<FrameworkElement, string> mParamDic = new Dictionary<FrameworkElement, string>();   // 用于重载使用的参数

        string[] mParamSplits;

        Visibility mUseBaseVisibility = Visibility.Visible;
        public Visibility UseBaseVisibility
        {
            get { return mUseBaseVisibility; }
            set
            {
                mUseBaseVisibility = value;
                OnPropertyChanged("UseBaseVisibility");
            }
        }

        bool mOverrideAble = true;
        public bool OverrideAble
        {
            get { return mOverrideAble; }
            protected set
            {
                mOverrideAble = value;
                if (mOverrideAble)
                {
                    OverrideGroupBox.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    OverrideGroupBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
        bool mIsStatic = false;
        public bool IsStatic
        {
            get { return mIsStatic; }
            set
            {
                mIsStatic = value;
            }
        }

        bool mUseable = true;
        public bool Useable
        {
            get { return mUseable; }
            protected set
            {
                mUseable = value;
                if (mUseable)
                {
                    MethodLink.Visibility = System.Windows.Visibility.Visible;
                    stackPanel_InputParams.Visibility = System.Windows.Visibility.Visible;

                    if (mReturnType != typeof(void))
                    {
                        label_returnTypeName.Visibility = System.Windows.Visibility.Visible;
                        returnLink.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    MethodLink.Visibility = System.Windows.Visibility.Hidden;
                    stackPanel_InputParams.Visibility = System.Windows.Visibility.Collapsed;
                    label_returnTypeName.Visibility = System.Windows.Visibility.Collapsed;
                    returnLink.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        public MethodNode(Canvas parentCanvas, string methodInfo)
            : base(parentCanvas, methodInfo)
        {
            InitializeComponent();

            OverrideAble = false;

            SetDragObject(RectangleTitle);
            SetUpLinkElement(MethodLink);

            mParamSplits = methodInfo.Split(',');
            MethodName.Text = mParamSplits[0];
            NodeName = MethodName.Text; 

            SetParameters(mParamSplits[1]);
            SetReturns(CSUtility.Program.GetTypeFromSaveString(mParamSplits[2]));

            OverrideAble = System.Convert.ToBoolean(mParamSplits[4]);
            Useable = System.Convert.ToBoolean(mParamSplits[5]);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, OverwriteLink, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, OverwriteLink.BackBrush, false);

            UpdateLink();
        }

        public static string GetParamInMethodInfo(System.Reflection.MethodInfo info)
        {
            string strRet = info.Name + ",";

            System.Reflection.ParameterInfo[] parInfos = info.GetParameters();
            if (parInfos.Length > 0)
            {
                foreach (var pInfo in parInfos)
                {
                    var parameterTypeString = CSUtility.Program.GetTypeSaveString(pInfo.ParameterType);
                    string strFlag = "";
                    if (pInfo.IsOut)
                    {
                        strFlag = ":out";
                        parameterTypeString = parameterTypeString.Replace("&", "");
                    }
                    else if (pInfo.ParameterType.IsByRef)
                    {
                        strFlag = ":ref";
                        parameterTypeString = parameterTypeString.Replace("&", "");
                    }
                    strRet += pInfo.Name + ":" + parameterTypeString + strFlag + "/";
                }
                strRet = strRet.Remove(strRet.Length - 1);   // 去除最后一个"/"
            }

            strRet += "," + CSUtility.Program.GetTypeSaveString(info.ReturnType);
            strRet += "," + CSUtility.Program.GetTypeSaveString(info.ReflectedType);

            return strRet;
        }

        // 根据参数设置界面
        protected void SetParameters(string pInfos)
        {
            if (String.IsNullOrEmpty(pInfos))
                return;

            mParamDic.Clear();
            ParamUsefulDatas.Clear();

            string[] paramInfos = pInfos.Split('/');
            foreach (var param in paramInfos)
            {
                ParameterControl pc = new ParameterControl(ParentDrawCanvas, param);
                AddChildNode(pc, stackPanel_InputParams);

                StackPanel sp = new StackPanel();
                ParamLinkPanel.Children.Add(sp);

                paramInfos = param.Split(':');
                string paramName = paramInfos[0];
                Type paramType = CSUtility.Program.GetTypeFromSaveString(paramInfos[1]);


                TextBlock lb = new TextBlock()
                {
                    Text = paramName + "(" + paramType.Name + ")",
                    FontSize = 6,
                };
                sp.Children.Add(lb);

                var rect = new CodeGenerateSystem.Controls.LinkOutControl()
                {
                    Margin = new Thickness(0, 0, 0, -21),
                    Width = 10,
                    Height = 10,
                    BackBrush = FindResource("Link_ValueBrush") as Brush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                    Direction = CodeGenerateSystem.Base.enBezierType.Bottom,
                };
                sp.Children.Add(rect);

                // 函数重载参数
                var linkObj = AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(paramType), rect, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, rect.BackBrush, true);

                var paramData = new CodeGenerateSystem.Base.UsefulMemberHostData()
                {
                    ClassTypeFullName = paramType.FullName,
                    HostControl = this,
                    LinkObject = linkObj,
                };
                ParamUsefulDatas.Add(paramData);
                mParamDic[rect] = param;
            }
        }

        // 根据返回值设置界面
        protected void SetReturns(Type returnType)
        {
            mReturnType = returnType;

            if (returnType == typeof(void))
            {
                label_returnTypeName.Visibility = System.Windows.Visibility.Hidden;
                returnLink.Visibility = System.Windows.Visibility.Hidden;
                ReturnValueLink.Visibility = System.Windows.Visibility.Collapsed;
                ReturnValueLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                label_returnTypeName.Text = returnType.Name;

                mReturnLinkObjInfo = AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(returnType), returnLink, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, returnLink.BackBrush, true);
                AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(returnType), ReturnValueLink, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.End, null, true);
            }
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("Useable", Useable.ToString());
            xmlNode.AddAttrib("OverrideAble", OverrideAble.ToString());
            xmlNode.AddAttrib("CallBasePosition", CallBasePosition.ToString());
            xmlNode.AddAttrib("UseBaseVisibility", UseBaseVisibility.ToString());
            xmlNode.AddAttrib("IsStatic", IsStatic.ToString());

            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            try
            {
                Useable = System.Convert.ToBoolean(xmlNode.FindAttrib("Useable").Value);
                OverrideAble = System.Convert.ToBoolean(xmlNode.FindAttrib("OverrideAble").Value);
                var posAttribute = xmlNode.FindAttrib("CallBasePosition");
                if (posAttribute != null)
                    CallBasePosition = (CallBaseFuction_Position)System.Enum.Parse(typeof(CallBaseFuction_Position), posAttribute.Value);
                var ubvAttribute = xmlNode.FindAttrib("UseBaseVisibility");
                if (ubvAttribute != null)
                    UseBaseVisibility = (Visibility)System.Enum.Parse(typeof(Visibility), ubvAttribute.Value);
                var att = xmlNode.FindAttrib("IsStatic");
                if (att != null)
                    IsStatic = System.Convert.ToBoolean(att.Value);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            base.Load(xmlNode, deltaX, deltaY);
        }

#region 代码生成

        public static string GetDelegateMethodName(MethodNode node, string methodName)
        {
            //return "Method_" + Program.GetValuedGUIDString(node.m_Guid) + "_" + methodName;
            return methodName;
        }

        protected virtual string GetDelegateMethodName()
        {
            return GetDelegateMethodName(this, MethodName.Text);
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, FrameworkElement element)
        {
            if (element == OverwriteLink || element == null)
            {
                CodeGenerateSystem.Base.LinkObjInfo linkOI = GetLinkObjInfo(OverwriteLink);
                if (linkOI.HasLink)
                {
                    // 函数重载
                    System.CodeDom.CodeMemberMethod methodCode = new System.CodeDom.CodeMemberMethod();
                    methodCode.Attributes = System.CodeDom.MemberAttributes.Public;
                    if (OverrideAble && IsStatic)
                        methodCode.Attributes |= System.CodeDom.MemberAttributes.Static;

                    //string curStateName = "", tagStateName = "";
                    //if (CurrentState != null)
                    //    curStateName = CurrentState;//.Name;
                    //if (TargetState != null)
                    //    tagStateName = TargetState;//.Name;
                    methodCode.Name = GetDelegateMethodName();

                    //methodCode.Name = MethodName.Text;
                    methodCode.ReturnType = new System.CodeDom.CodeTypeReference(mReturnType);

                    System.CodeDom.CodeMethodInvokeExpression baseExp = new System.CodeDom.CodeMethodInvokeExpression();
                    baseExp.Method = new System.CodeDom.CodeMethodReferenceExpression(new System.CodeDom.CodeBaseReferenceExpression(), MethodName.Text);

                    foreach (var param in mParamDic.Values)
                    {
                        var splits = param.Split(':');

                        System.CodeDom.CodeParameterDeclarationExpression paramExp = new System.CodeDom.CodeParameterDeclarationExpression();
                        if (splits.Length > 2)
                        {
                            paramExp.Direction = (System.CodeDom.FieldDirection)System.Enum.Parse(typeof(System.CodeDom.FieldDirection), splits[2]);
                            splits[1] = splits[1].Remove(splits[1].Length - 1);
                        }
                        var typeStr = CSUtility.Program.GetAppTypeStringFromSaveString(splits[1]);
                        paramExp.Type = new System.CodeDom.CodeTypeReference(typeStr);
                        paramExp.Name = splits[0];
                        methodCode.Parameters.Add(paramExp);

                        baseExp.Parameters.Add(new System.CodeDom.CodeVariableReferenceExpression(paramExp.Name));
                    }

                    // 将所有可能出现的变量作为类成员声明，防止出现可能的重复声明
                    foreach (var node in this.HostNodesContainer.CtrlNodeList)
                    {
                        try
                        {
                            if (node.HasMultiOutLink)
                            {
                                var valueType = node.GCode_GetValueType(null);
                                var valueName = node.GCode_GetValueName(null);
                                System.CodeDom.CodeVariableDeclarationStatement variableDec = new System.CodeDom.CodeVariableDeclarationStatement(valueType, valueName);
                                var type = CSUtility.Program.GetTypeFromTypeFullName(valueType);
                                variableDec.InitExpression = new System.CodeDom.CodePrimitiveExpression(CodeGenerateSystem.Program.GetDefaultValueFromType(type));

                                methodCode.Statements.Add(variableDec);
                            }
                        }
                        catch (System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.ToString());
                        }
                    }                    

                    if (CallBasePosition == CallBaseFuction_Position.First)
                    {
                        methodCode.Statements.Add(baseExp);
                    }
                    
                    linkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, methodCode.Statements, linkOI.GetLinkElement(0, false));

                    if (CallBasePosition == CallBaseFuction_Position.Last)
                    {
                        methodCode.Statements.Add(baseExp);
                    }

                    codeClass.Members.Add(methodCode);
                }
            }
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            if (element == MethodLink)
            {
                System.CodeDom.CodeExpression[] expColls = new System.CodeDom.CodeExpression[mParamDic.Count];
                // 参数
                int i = 0;
                foreach (var paramNode in mChildNodes)
                {
                    if (paramNode.GetType() != typeof(ParameterControl))
                        continue;

                    ParameterControl paramCtrl = paramNode as ParameterControl;
                    paramCtrl.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, null);

                    expColls[i] = paramCtrl.GCode_CodeDom_GetValue(null);
                    i++;
                }

                // 函数调用
                System.CodeDom.CodeMethodReferenceExpression methodRef = new System.CodeDom.CodeMethodReferenceExpression();

                // Todo: 这里根据类的不同来源确定TargetObject
                // ========================================
                if (this.ParentNode != null)
                {
                    if (!this.ParentNode.IsOnlyReturnValue)
                    {
                        this.ParentNode.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, null);
                    }

                    methodRef.TargetObject = this.ParentNode.GCode_CodeDom_GetValue(null);
                }

                methodRef.MethodName = MethodName.Text;
                System.CodeDom.CodeExpressionStatement methodExpState = new System.CodeDom.CodeExpressionStatement(
                                                                                    new System.CodeDom.CodeMethodInvokeExpression(methodRef, expColls));

                codeStatementCollection.Add(methodExpState);
            }
            else if (element == returnLink)
            {
                foreach (var paramNode in mChildNodes)
                {
                    if (paramNode.GetType() != typeof(ParameterControl))
                        continue;

                    ParameterControl paramCtrl = paramNode as ParameterControl;
                    paramCtrl.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, null);
                }

                if (!ParentNode.IsOnlyReturnValue)
                    ParentNode.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, null);
            }
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            if (element == returnLink)
            {
                System.CodeDom.CodeExpression[] expColls = new System.CodeDom.CodeExpression[mParamDic.Count];
                // 参数
                int i = 0;
                foreach (var paramNode in mChildNodes)
                {
                    if (paramNode.GetType() != typeof(ParameterControl))
                        continue;

                    ParameterControl paramCtrl = paramNode as ParameterControl;

                    expColls[i] = paramCtrl.GCode_CodeDom_GetValue(null);
                    i++;
                }

                System.CodeDom.CodeMethodReferenceExpression methodRef = new System.CodeDom.CodeMethodReferenceExpression();
                if (this.ParentNode != null)
                {
                    methodRef.TargetObject = this.ParentNode.GCode_CodeDom_GetValue(null);
                }

                methodRef.MethodName = MethodName.Text;
                return new System.CodeDom.CodeMethodInvokeExpression(methodRef, expColls);
            }
            else
            {
                // 重载部分的参数获取
                string paramStr;
                if (mParamDic.TryGetValue(element, out paramStr))
                {
                    var splits = paramStr.Split(':');
                    return new System.CodeDom.CodeArgumentReferenceExpression(splits[0]);
                }
            }

            return base.GCode_CodeDom_GetValue(element);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == returnLink)
                return mReturnType.ToString();
            else
            {
                // 重载部分的参数类型获取
                string paramStr;
                if (mParamDic.TryGetValue(element, out paramStr))
                {
                    var splits = paramStr.Split(':');
                    return CSUtility.Program.GetAppTypeStringFromSaveString(splits[1]);
                }
            }

            return base.GCode_GetValueType(element);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (element == returnLink)
                return "";
            else
            {
                // 重载部分的参数类型获取
                string paramStr;
                if (mParamDic.TryGetValue(element, out paramStr))
                {
                    var splits = paramStr.Split(':');
                    return splits[0];
                }
            } 

            return base.GCode_GetValueName(element);
        }

        #endregion

        #region CallBaseFunction

        public enum CallBaseFuction_Position
        {
            First = 0,
            Last,
            None,
        }
        CallBaseFuction_Position m_CallBaseFuctionPosition;
        public CallBaseFuction_Position CallBasePosition
        {
            get { return m_CallBaseFuctionPosition; }
            set
            {
                m_CallBaseFuctionPosition = value;
                ComboBox_CallBase.SelectedIndex = (int)m_CallBaseFuctionPosition;
            }
        }
        private void ComboBox_CallBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_CallBaseFuctionPosition = (CallBaseFuction_Position)ComboBox_CallBase.SelectedIndex;
        }

#endregion
    }

    public class ParameterControl : CodeGenerateSystem.Base.BaseNodeControl
    {
        CodeGenerateSystem.Controls.LinkInControl mParamEllipse;
        public CodeGenerateSystem.Controls.LinkInControl ParamEllipse
        {
            get { return mParamEllipse; }
        }
        Type mParamType;
        public Type ParamType
        {
            get { return mParamType; }
        }
        string mParamName;
        public string ParamName
        {
            get { return mParamName; }
        }
        string mParamFlag = "";

        public ParameterControl(Canvas parentCanvas, string param)
            //public ParameterControl(Canvas parentCanvas, ParameterInfo param)
            : base(parentCanvas, param)
        {
            Grid grid = new Grid();
            AddChild(grid);

            string[] splits = param.Split(':');
            mParamName = splits[0];
            mParamType = CSUtility.Program.GetTypeFromSaveString(splits[1]);
            if (splits.Length > 2)
                mParamFlag = splits[2] + " ";

            mParamEllipse = new CodeGenerateSystem.Controls.LinkInControl()
            {
                Margin = new System.Windows.Thickness(-15, 0, 0, 0),
                Width = 10,
                Height = 10,
                BackBrush = new SolidColorBrush(Color.FromRgb(243, 146, 243)),
                HorizontalAlignment = HorizontalAlignment.Left,
                Direction = CodeGenerateSystem.Base.enBezierType.Left,
            };
            grid.Children.Add(mParamEllipse);
            AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(mParamType), mParamEllipse, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);

            TextBlock label = new TextBlock()
            {
                Text = mParamName + "(" + mParamFlag + mParamType.Name + ")",
                Foreground = Brushes.White,
                //BorderBrush = Brushes.Black,
                //BorderThickness = new CSUtility.Support.Thickness(0, 0.5, 0, 0.5)
                Margin = new Thickness(2)
            };
            grid.Children.Add(label);
        }

        public bool IsParamLinkRight(ref string strMsg)
        {
            var linkOI = GetLinkObjInfo(mParamEllipse);
            if (linkOI.HasLink)
            {
                Type parType = CSUtility.Program.GetTypeFromSaveString(linkOI.GetLinkObject(0, true).GCode_GetValueType(linkOI.GetLinkElement(0, true)));
                if (parType != mParamType)
                {
                    strMsg = "函数参数类型与连接的类型不匹配\r\n函数参数类型：" + mParamType.ToString() + "\r\n连接参数类型" + parType.ToString();
                    return false;
                }
            }
            else
            {
                strMsg = "函数没有设置参数 " + mParamName;
                return false;
            }

            return true;
        }

        //public override void Save(CSUtility.Support.XmlNode xmlNode)
        //{
        //    xmlNode.AddAttrib("Params", m_methodInfoToSave);

        //    base.Save(xmlNode);
        //}

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(mParamEllipse);
            if (linkOI.HasLink)
            {
                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));
            }
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(mParamEllipse);
            if (linkOI.HasLink)
            {
                System.CodeDom.FieldDirection fd = System.CodeDom.FieldDirection.In;
                switch (mParamFlag)
                {
                    case "ref ":
                        fd = System.CodeDom.FieldDirection.Ref;
                        break;
                    case "out ":
                        fd = System.CodeDom.FieldDirection.Out;
                        break;
                    default:
                        fd = System.CodeDom.FieldDirection.In;
                        break;
                }
                return new System.CodeDom.CodeDirectionExpression(fd, linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true)));
            }
            else
                return base.GCode_CodeDom_GetValue(element);
        }
    }

}
