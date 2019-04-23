using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using CSUtility.Support;

namespace CodeDomNode
{
    public partial class MethodInvokeNode : CodeGenerateSystem.Base.BaseNodeControl, CodeGenerateSystem.Base.UsefulMember
    {
        CodeGenerateSystem.Base.LinkObjInfo mReturnLinkInfo;

        public List<CodeGenerateSystem.Base.UsefulMemberHostData> GetUsefulMembers()
        {
            List<CodeGenerateSystem.Base.UsefulMemberHostData> retValue = new List<CodeGenerateSystem.Base.UsefulMemberHostData>();
            if (mReturnLinkInfo == null)
                return retValue;

            var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
            {
                ClassTypeFullName = MethodReturnType.FullName,
                HostControl = this,
                LinkObject = mReturnLinkInfo,
            };

            retValue.Add(memberData);

            return retValue;
        }

        public List<CodeGenerateSystem.Base.UsefulMemberHostData> GetUsefulMembers(CodeGenerateSystem.Base.LinkControl linkCtrl)
        {
            List<CodeGenerateSystem.Base.UsefulMemberHostData> retValue = new List<CodeGenerateSystem.Base.UsefulMemberHostData>();
            if (mReturnLinkInfo == null)
                return retValue;

            if (linkCtrl == returnLink)
            {
                var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
                {
                    ClassTypeFullName = MethodReturnType.FullName,
                    HostControl = this,
                    LinkObject = mReturnLinkInfo,
                };

                retValue.Add(memberData);
            }

            return retValue;
        }

        Type mReturnType;
        public Type MethodReturnType
        {
            get { return mReturnType; }
        }

        // 临时类，用于选中后显示参数属性
        CodeGenerateSystem.Base.GeneratorClassBase mTemplateClassInstance = null;
        public CodeGenerateSystem.Base.GeneratorClassBase TemplateClassInstance
        {
            get { return mTemplateClassInstance; }
        }

        // 类实例名称
        string mClassInstanceName;
        public string ClassInstanceName
        {
            get { return mClassInstanceName; }
            set
            {
                mClassInstanceName = value;
                OnPropertyChanged("ClassInstanceName");
            }
        }

        CodeGenerateSystem.Base.UsefulMemberHostData mHostUsefulMemberData = new CodeGenerateSystem.Base.UsefulMemberHostData();
        System.Reflection.MethodInfo mMethodInfo = null;

        public static string GetParamFromParamInfo(System.Reflection.ParameterInfo pInfo)
        {
            var parameterTypeString = CSUtility.Program.GetTypeSaveString(pInfo.ParameterType);
            string strFlag = "";
            if (pInfo.IsOut)
            {
                strFlag = ":out";
                parameterTypeString = parameterTypeString.Remove(parameterTypeString.Length - 1);
            }
            else if (pInfo.ParameterType.IsByRef)
            {
                strFlag = ":ref";
                parameterTypeString = parameterTypeString.Remove(parameterTypeString.Length - 1);
            }
            return pInfo.Name + ":" + parameterTypeString + strFlag;
        }
        public static string GetParamFromMethodInfo(System.Reflection.MethodInfo info, string path)
        {           
            string strRet = path + "," + CSUtility.Program.GetTypeSaveString(info.ReflectedType) + "," + info.Name + ",";
            
            System.Reflection.ParameterInfo[] parInfos = info.GetParameters();
            if (parInfos.Length > 0)
            {
                foreach (var pInfo in parInfos)
                {
                    strRet += GetParamFromParamInfo(pInfo) + "/";
                }
                strRet = strRet.Remove(strRet.Length - 1);   // 去除最后一个"/"
            }
            
            strRet += "," + CSUtility.Program.GetTypeSaveString(info.ReturnType);

            return strRet;
        }
        public static System.Reflection.MethodInfo GetMethodInfoFromParam(string param)
        {
            try
            {
                if (string.IsNullOrEmpty(param))
                    return null;

                var splits = param.Split(',');
                CSUtility.Helper.enCSType csType = CSUtility.Helper.enCSType.All;
                if (splits.Length > 5)
                    csType = (CSUtility.Helper.enCSType)CSUtility.Support.IHelper.EnumTryParse(typeof(CSUtility.Helper.enCSType), splits[5]);
                var path = splits[0];
                var classType = CSUtility.Program.GetTypeFromSaveString(splits[1], csType);
                var methodName = splits[2];

                if (!string.IsNullOrEmpty(splits[3]))
                {
                    var paramSplits = splits[3].Split('/');
                    Type[] paramTypes = new Type[paramSplits.Length];
                    for(int i=0; i<paramSplits.Length; i++)
                    {
                        var tempSplits = paramSplits[i].Split(':');
                        if(tempSplits.Length > 2)
                        {
                            switch(tempSplits[2])
                            {
                                case "ref":
                                case "out":
                                    tempSplits[1] += "&";
                                    break;
                            }
                        }
                        paramTypes[i] = CSUtility.Program.GetTypeFromSaveString(tempSplits[1], csType);
                    }

                    return classType.GetMethod(methodName, paramTypes);
                }
                else
                {
                    return classType.GetMethod(methodName, new Type[0]);
                }
            }
            catch (System.Exception e)
            {
                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, "逻辑图：" + e.ToString());
            }

            return null;
        }

        public MethodInvokeNode(Canvas parentCanvas, string methodParam)
            : base(parentCanvas, methodParam)
        {
            this.InitializeComponent();

            SetDragObject(RectangleTitle);
            SetUpLinkElement(MethodLink_Pre);

            if (string.IsNullOrEmpty(methodParam))
                return;

            var methodParamSplits = methodParam.Split(';');
            if (methodParamSplits.Length < 2)
                return;

            var splits = methodParamSplits[0].Split(',');
            MethodName.Text = splits[0];
            NodeName = MethodName.Text;

            mMethodInfo = GetMethodInfoFromParam(methodParamSplits[0]);
            if(mMethodInfo != null)
            {
                SetParameters(mMethodInfo.GetParameters());
                SetReturn(mMethodInfo.ReturnType);

                AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Pre, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink_Pre.BackBrush, false);
            }

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Next, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink_Next.BackBrush, false);

            OnDeleteNode += (node) =>
            {
                parentCanvas.Children.Remove(mParentLinkPath);
                if (mHostUsefulMemberData.LinkObject != null)
                    mHostUsefulMemberData.LinkObject.m_virtualNodes.Remove(this);
            };
        }

        private void InitializeLinkLine()
        {
            if (ParentDrawCanvas == null)
                return;

            if (mHostUsefulMemberData.LinkObject != null)
                mHostUsefulMemberData.LinkObject.m_virtualNodes.Add(this);

            BindingOperations.ClearBinding(this.mParentLinkPath, Path.VisibilityProperty);
            BindingOperations.SetBinding(this.mParentLinkPath, Path.VisibilityProperty, new Binding("Visibility") { Source = this });
            mParentLinkPath.Stroke = Brushes.LightGray;
            mParentLinkPath.StrokeDashArray = new DoubleCollection(new double[] { 2, 4 });
            //m_ParentLinkPath.StrokeThickness = 3;
            mParentLinkPathFig.Segments.Add(mParentLinkBezierSeg);
            PathFigureCollection pfc = new PathFigureCollection();
            pfc.Add(mParentLinkPathFig);
            PathGeometry pg = new PathGeometry();
            pg.Figures = pfc;
            mParentLinkPath.Data = pg;
            ParentDrawCanvas.Children.Add(mParentLinkPath);
        }

        public override void UpdateLink()
        {
            base.UpdateLink();

            if (mHostUsefulMemberData == null || mHostUsefulMemberData.LinkObject == null)
                return;

            mParentLinkPathFig.StartPoint = mHostUsefulMemberData.LinkObject.LinkElement.TranslatePoint(mHostUsefulMemberData.LinkObject.LinkElementOffset, ParentDrawCanvas);
            mParentLinkBezierSeg.Point3 = TranslatePoint(new System.Windows.Point(0, 0), ParentDrawCanvas);

            double delta = System.Math.Max(System.Math.Abs(mParentLinkBezierSeg.Point3.X - mParentLinkPathFig.StartPoint.X) / 2, 25);
            delta = System.Math.Min(150, delta);

            switch (mHostUsefulMemberData.LinkObject.m_bezierType)
            {
                case CodeGenerateSystem.Base.enBezierType.Left:
                    mParentLinkBezierSeg.Point1 = new System.Windows.Point(mParentLinkPathFig.StartPoint.X - delta, mParentLinkPathFig.StartPoint.Y);
                    break;
                case CodeGenerateSystem.Base.enBezierType.Right:
                    mParentLinkBezierSeg.Point1 = new System.Windows.Point(mParentLinkPathFig.StartPoint.X + delta, mParentLinkPathFig.StartPoint.Y);
                    break;
                case CodeGenerateSystem.Base.enBezierType.Top:
                    mParentLinkBezierSeg.Point1 = new System.Windows.Point(mParentLinkPathFig.StartPoint.X, mParentLinkPathFig.StartPoint.Y - delta);
                    break;
                case CodeGenerateSystem.Base.enBezierType.Bottom:
                    mParentLinkBezierSeg.Point1 = new System.Windows.Point(mParentLinkPathFig.StartPoint.X, mParentLinkPathFig.StartPoint.Y + delta);
                    break;
            }

            mParentLinkBezierSeg.Point2 = new System.Windows.Point(mParentLinkBezierSeg.Point3.X, mParentLinkBezierSeg.Point3.Y - delta);
        }

        public override void InitializeUsefulLinkDatas()
        {
            var methodParamSplits = this.StrParams.Split(';');
            if (methodParamSplits.Length < 2)
                return;

            mHostUsefulMemberData.ParseString(methodParamSplits[1], HostNodesContainer);
                        
            switch (mHostUsefulMemberData.HostType)
            {
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Static:
                    ClassInstanceName = mHostUsefulMemberData.ClassTypeFullName;
                    break;
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Instance:
                    ClassInstanceName = mHostUsefulMemberData.ClassTypeFullName + ".Instance";
                    break;
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Normal:
                    {
                        var name = mHostUsefulMemberData.HostControl.GCode_GetValueName(mHostUsefulMemberData.LinkObject.LinkElement);
                        ClassInstanceName = name + "(" + mHostUsefulMemberData.ClassTypeFullName + ")";
                    }
                    break;
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.This:
                    {
                        ClassInstanceName = "this(" + mHostUsefulMemberData.ClassTypeFullName + ")";
                    }
                    break;
            }

            InitializeLinkLine();
        }

        protected override void CollectionErrorMsg()
        {
            //AddErrorMsg(returnLink, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "错误测试");
        }

        // 设置函数参数
        protected void SetParameters(System.Reflection.ParameterInfo[] paramInfos)
        {
            if (paramInfos == null)
                return;

            stackPanel_InputParams.Children.Clear();

            var cpInfos = new List<CodeGenerateSystem.Base.CustomPropertyInfo>();
            
            foreach (var paramInfo in paramInfos)
            {
                var param = GetParamFromParamInfo(paramInfo);
                var pc = new MethodInvokeParameterControl(ParentDrawCanvas, param);
                var cpInfo = CodeGenerateSystem.Base.CustomPropertyInfo.GetFromParamInfo(paramInfo);

                foreach(var att in paramInfo.GetCustomAttributes(true))
                {
                    if(att is DescriptionAttribute)
                    {
                        pc.ToolTip = ((DescriptionAttribute)att).Description;
                    }
                }

                cpInfos.Add(cpInfo);
                AddChildNode(pc, stackPanel_InputParams);
            }

            var classType = CodeGenerateSystem.Base.PropertyClassGenerator.CreateTypeFromCustomPropertys(cpInfos);
            mTemplateClassInstance = System.Activator.CreateInstance(classType) as CodeGenerateSystem.Base.GeneratorClassBase;

            foreach(var property in mTemplateClassInstance.GetType().GetProperties())
            {
                property.SetValue(mTemplateClassInstance, CodeGenerateSystem.Program.GetDefaultValueFromType(property.PropertyType));
            }
        }

        // 根据返回值设置界面
        protected void SetReturn(Type returnType)
        {
            mReturnType = returnType;

            if (returnType == typeof(void))
            {
                label_returnTypeName.Visibility = System.Windows.Visibility.Hidden;
                returnLink.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                label_returnTypeName.Text = returnType.Name;

                mReturnLinkInfo = AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(returnType), returnLink, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, returnLink.BackBrush, true);
                mReturnLinkInfo.OnAddLinkInfo += OnReturnLinkAddLinkInfo;
                mReturnLinkInfo.OnDelLinkInfo += OnReturnLinkDelLinkInfo;
            }
        }

        // 返回节点增加链接时调用方法
        void OnReturnLinkAddLinkInfo(CodeGenerateSystem.Base.LinkInfo info)
        {
            var linkObjInfo = GetLinkObjInfo(MethodLink_Pre);
            if(linkObjInfo.HasLink)
            {
                linkObjInfo.Clear();
            }
            linkObjInfo = GetLinkObjInfo(MethodLink_Next);
            if(linkObjInfo.HasLink)
            {
                linkObjInfo.Clear();
            }

            MethodLink_Pre.Visibility = Visibility.Collapsed;
            MethodLink_Next.Visibility = Visibility.Collapsed;
        }
        // 返回节点删除连接时调用方法
        void OnReturnLinkDelLinkInfo(CodeGenerateSystem.Base.LinkInfo info)
        {
            var linkOI = GetLinkObjInfo(returnLink);
            if(!linkOI.HasLink)
            {
                MethodLink_Pre.Visibility = Visibility.Visible;
                MethodLink_Next.Visibility = Visibility.Visible;
            }
        }

        public override object GetShowPropertyObject()
        {
            return mTemplateClassInstance;
        }

        public override void Save(XmlNode xmlNode, bool newGuid, XmlHolder holder)
        {
            if (mTemplateClassInstance != null)
            {
                var node = xmlNode.AddNode("DefaultParamValue", "", holder);
                mTemplateClassInstance.Save(node, holder);
            }

            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(XmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);

            if(mTemplateClassInstance != null)
            {
                var node = xmlNode.FindNode("DefaultParamValue");
                if(node != null)
                {
                    mTemplateClassInstance.Load(node);
                }
            }
        }

        #region 代码生成

        public override void GCode_CodeDom_GenerateCode(CodeTypeDeclaration codeClass, CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            if(mMethodInfo == null)
            {
                base.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, element);
                return;
            }

            if (element == MethodLink_Pre)
            {
                System.CodeDom.CodeExpression[] expColls = new System.CodeDom.CodeExpression[mChildNodes.Count];
                // 参数
                int i = 0;
                foreach (var paramNode in mChildNodes)
                {
                    if (paramNode.GetType() != typeof(MethodInvokeParameterControl))
                        continue;

                    var paramCtrl = paramNode as MethodInvokeParameterControl;
                    paramCtrl.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, null);

                    expColls[i] = paramCtrl.GCode_CodeDom_GetValue(null);
                    i++;
                }

                // 函数调用
                System.CodeDom.CodeMethodReferenceExpression methodRef = new System.CodeDom.CodeMethodReferenceExpression();

                switch(mHostUsefulMemberData.HostType)
                {
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Instance:
                        {
                            methodRef.TargetObject = new System.CodeDom.CodeSnippetExpression(mHostUsefulMemberData.ClassTypeFullName + ".Instance");
                        }
                        break;
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Normal:
                        {
                            if(!mHostUsefulMemberData.HostControl.IsOnlyReturnValue)
                            {
                                mHostUsefulMemberData.HostControl.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, mHostUsefulMemberData.LinkObject.LinkElement);
                            }

                            methodRef.TargetObject = new System.CodeDom.CodeCastExpression(new System.CodeDom.CodeTypeReference(mHostUsefulMemberData.ClassTypeFullName),
                                        mHostUsefulMemberData.HostControl.GCode_CodeDom_GetValue(mHostUsefulMemberData.LinkObject.LinkElement));
                        }
                        break;
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Static:
                        {
                            methodRef.TargetObject = new System.CodeDom.CodeTypeReferenceExpression(mHostUsefulMemberData.ClassTypeFullName);
                        }
                        break;
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.This:
                        {
                            methodRef.TargetObject = new System.CodeDom.CodeThisReferenceExpression();
                        }
                        break;
                }

                methodRef.MethodName = mMethodInfo.Name;
                System.CodeDom.CodeExpressionStatement methodExpState = new System.CodeDom.CodeExpressionStatement(
                                                                                    new System.CodeDom.CodeMethodInvokeExpression(methodRef, expColls));

                codeStatementCollection.Add(methodExpState);

                var nextMethodLinkOI = GetLinkObjInfo(MethodLink_Next);
                if(nextMethodLinkOI.HasLink)
                {
                    nextMethodLinkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, nextMethodLinkOI.GetLinkElement(0, false));
                }
            }
            else if (element == returnLink)
            {
                foreach (var paramNode in mChildNodes)
                {
                    if (paramNode.GetType() != typeof(MethodInvokeParameterControl))
                        continue;

                    var paramCtrl = paramNode as MethodInvokeParameterControl;
                    paramCtrl.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, null);
                }
                
                switch(mHostUsefulMemberData.HostType)
                {
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Normal:
                        {
                            if (mHostUsefulMemberData != null && mHostUsefulMemberData.HostControl != null && !mHostUsefulMemberData.HostControl.IsOnlyReturnValue)
                                mHostUsefulMemberData.HostControl.GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, null);
                        }
                        break;
                }
            }
        }

        public override CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            if (mMethodInfo == null)
                return base.GCode_CodeDom_GetValue(element);

            if (element == returnLink)
            {
                System.CodeDom.CodeExpression[] expColls = new System.CodeDom.CodeExpression[mChildNodes.Count];
                // 参数
                int i = 0;
                foreach (var paramNode in mChildNodes)
                {
                    if (paramNode.GetType() != typeof(MethodInvokeParameterControl))
                        continue;

                    var paramCtrl = paramNode as MethodInvokeParameterControl;

                    expColls[i] = paramCtrl.GCode_CodeDom_GetValue(null);
                    i++;
                }

                System.CodeDom.CodeMethodReferenceExpression methodRef = new System.CodeDom.CodeMethodReferenceExpression();
                switch (mHostUsefulMemberData.HostType)
                {
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Instance:
                        {
                            methodRef.TargetObject = new System.CodeDom.CodeSnippetExpression(mHostUsefulMemberData.ClassTypeFullName + ".Instance");
                        }
                        break;
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Normal:
                        {
                            methodRef.TargetObject = new System.CodeDom.CodeCastExpression(new System.CodeDom.CodeTypeReference(mHostUsefulMemberData.ClassTypeFullName), 
                                mHostUsefulMemberData.HostControl.GCode_CodeDom_GetValue(mHostUsefulMemberData.LinkObject.LinkElement));
                        }
                        break;
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Static:
                        {
                            methodRef.TargetObject = new System.CodeDom.CodeTypeReferenceExpression(mHostUsefulMemberData.ClassTypeFullName);
                        }
                        break;
                    case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.This:
                        {
                            methodRef.TargetObject = new System.CodeDom.CodeThisReferenceExpression();
                        }
                        break;
                }

                methodRef.MethodName = mMethodInfo.Name;
                return new System.CodeDom.CodeMethodInvokeExpression(methodRef, expColls);
            }

            return base.GCode_CodeDom_GetValue(element);
        }

        #endregion
    }

    public class MethodInvokeParameterControl : CodeGenerateSystem.Base.BaseNodeControl
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

        public MethodInvokeParameterControl(Canvas parentCanvas, string param)
            //public MethodInvokeParameterControl(Canvas parentCanvas, ParameterInfo param)
            : base(parentCanvas, param)
        {
            Grid grid = new Grid();
            AddChild(grid);

            string[] splits = param.Split(':');
            mParamName = splits[0];
            mParamType = CSUtility.Program.GetTypeFromSaveString(splits[1]);
            if (splits.Length > 2)
                mParamFlag = splits[2];

            mParamEllipse = new CodeGenerateSystem.Controls.LinkInControl()
            {
                Margin = new System.Windows.Thickness(-15, 0, 0, 0),
                Width = 10,
                Height = 10,
                BackBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 146, 243)),
                HorizontalAlignment = HorizontalAlignment.Left,
                Direction = CodeGenerateSystem.Base.enBezierType.Left,
            };
            grid.Children.Add(mParamEllipse);
            AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(mParamType), mParamEllipse, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);

            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            grid.Children.Add(stackPanel);

            TextBlock label = new TextBlock()
            {
                Text = mParamName,
                Foreground = Brushes.White,
                //BorderBrush = Brushes.Black,
                //BorderThickness = new CSUtility.Support.Thickness(0, 0.5, 0, 0.5)
                Margin = new System.Windows.Thickness(2)
            };
            stackPanel.Children.Add(label);

            TextBlock textBlockType = new TextBlock()
            {
                Text = mParamFlag + (string.IsNullOrEmpty(mParamFlag) ? "" : " ") + CSUtility.Program.GetAppTypeString(mParamType),
                Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(160, 160, 160)),
                Margin = new System.Windows.Thickness(2)
            };
            stackPanel.Children.Add(textBlockType);
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

        System.CodeDom.CodeStatement mTemplateClassStatement;
        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(mParamEllipse);
            if (linkOI.HasLink)
            {
                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));
            }
            else
            {
                if(mTemplateClassStatement == null || !codeStatementCollection.Contains(mTemplateClassStatement))
                {
                    var node = ParentNode as MethodInvokeNode;
                    if(node != null && node.TemplateClassInstance != null)
                    {
                        var proInfo = node.TemplateClassInstance.GetType().GetProperty(ParamName);
                        var classValue = proInfo.GetValue(node.TemplateClassInstance);
                        var paramCodeName = "param_" + CodeGenerateSystem.Program.GetValuedGUIDString(this.Id);
                        if (ParamType.IsGenericType)
                        {
                            CodeGenerateSystem.Program.GenerateGenericInitializeCode(codeStatementCollection, ParamType, classValue, paramCodeName);
                        }
                        else if(ParamType.IsArray)
                        {
                            CodeGenerateSystem.Program.GenerateArrayInitializeCode(codeStatementCollection, ParamType, classValue, paramCodeName);
                        }
                        else if(ParamType == typeof(string))
                        {
                            switch(mParamFlag)
                            {
                                case "ref":
                                case "out":
                                    { 
                                        mTemplateClassStatement = new System.CodeDom.CodeVariableDeclarationStatement(
                                                                        ParamType,
                                                                        paramCodeName,
                                                                        new System.CodeDom.CodePrimitiveExpression(classValue));
                                        codeStatementCollection.Add(mTemplateClassStatement);
                                    }
                                    break;
                            }
                        }
                        else if(ParamType.IsClass)
                        {
                            // 生成
                            CodeGenerateSystem.Program.GenerateClassInitializeCode(codeStatementCollection, ParamType, classValue, paramCodeName);
                        }
                        else if(ParamType == typeof(SlimDX.Vector2))
                        {
                            var val = (SlimDX.Vector2)classValue;
                            var statement = new System.CodeDom.CodeVariableDeclarationStatement(ParamType, paramCodeName,
                                new System.CodeDom.CodeObjectCreateExpression(ParamType, new CodeExpression[] {
                                    new System.CodeDom.CodePrimitiveExpression(val.X),
                                    new System.CodeDom.CodePrimitiveExpression(val.Y),
                                }));
                            codeStatementCollection.Add(statement);
                        }
                        else if(ParamType == typeof(SlimDX.Vector3))
                        {
                            var val = (SlimDX.Vector3)classValue;
                            var statement = new System.CodeDom.CodeVariableDeclarationStatement(ParamType, paramCodeName,
                                new System.CodeDom.CodeObjectCreateExpression(ParamType, new CodeExpression[] {
                                    new System.CodeDom.CodePrimitiveExpression(val.X),
                                    new System.CodeDom.CodePrimitiveExpression(val.Y),
                                    new System.CodeDom.CodePrimitiveExpression(val.Z),
                                }));
                            codeStatementCollection.Add(statement);
                        }
                        else if (ParamType == typeof(SlimDX.Vector4))
                        {
                            var val = (SlimDX.Vector4)classValue;
                            var statement = new System.CodeDom.CodeVariableDeclarationStatement(ParamType, paramCodeName,
                                new System.CodeDom.CodeObjectCreateExpression(ParamType, new CodeExpression[] {
                                    new System.CodeDom.CodePrimitiveExpression(val.X),
                                    new System.CodeDom.CodePrimitiveExpression(val.Y),
                                    new System.CodeDom.CodePrimitiveExpression(val.Z),
                                    new System.CodeDom.CodePrimitiveExpression(val.W),
                                }));
                            codeStatementCollection.Add(statement);
                        }
                        else if(ParamType == typeof(SlimDX.Quaternion))
                        {
                            var val = (SlimDX.Quaternion)classValue;
                            var statement = new System.CodeDom.CodeVariableDeclarationStatement(ParamType, paramCodeName,
                                new System.CodeDom.CodeObjectCreateExpression(ParamType, new CodeExpression[] {
                                    new System.CodeDom.CodePrimitiveExpression(val.X),
                                    new System.CodeDom.CodePrimitiveExpression(val.Y),
                                    new System.CodeDom.CodePrimitiveExpression(val.Z),
                                    new System.CodeDom.CodePrimitiveExpression(val.W),
                                }));
                            codeStatementCollection.Add(statement);
                        }
                        else if(ParamType == typeof(SlimDX.Matrix))
                        {
                            var val = (SlimDX.Matrix)classValue;
                            var statement = new System.CodeDom.CodeVariableDeclarationStatement(ParamType, paramCodeName,
                                new System.CodeDom.CodeObjectCreateExpression(ParamType, new CodeExpression[] {}));
                            codeStatementCollection.Add(statement);
                            var assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M11"), new System.CodeDom.CodePrimitiveExpression(val.M11));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M12"), new System.CodeDom.CodePrimitiveExpression(val.M12));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M13"), new System.CodeDom.CodePrimitiveExpression(val.M13));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M14"), new System.CodeDom.CodePrimitiveExpression(val.M14));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M21"), new System.CodeDom.CodePrimitiveExpression(val.M21));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M22"), new System.CodeDom.CodePrimitiveExpression(val.M22));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M23"), new System.CodeDom.CodePrimitiveExpression(val.M23));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M24"), new System.CodeDom.CodePrimitiveExpression(val.M24));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M31"), new System.CodeDom.CodePrimitiveExpression(val.M31));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M32"), new System.CodeDom.CodePrimitiveExpression(val.M32));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M33"), new System.CodeDom.CodePrimitiveExpression(val.M33));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M34"), new System.CodeDom.CodePrimitiveExpression(val.M34));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M41"), new System.CodeDom.CodePrimitiveExpression(val.M41));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M42"), new System.CodeDom.CodePrimitiveExpression(val.M42));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M43"), new System.CodeDom.CodePrimitiveExpression(val.M43));
                            codeStatementCollection.Add(assignStatement);
                            assignStatement = new System.CodeDom.CodeAssignStatement(new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(paramCodeName), "M44"), new System.CodeDom.CodePrimitiveExpression(val.M44));
                            codeStatementCollection.Add(assignStatement);
                        }
                        else if (ParamType == typeof(Guid))
                        {
                            mTemplateClassStatement = new System.CodeDom.CodeVariableDeclarationStatement(ParamType, paramCodeName, new System.CodeDom.CodeSnippetExpression("CSUtility.Support.IHelper.GuidTryParse(\"" + classValue.ToString() + "\")"));
                            codeStatementCollection.Add(mTemplateClassStatement);
                        }
                        else
                        {
                            switch(mParamFlag)
                            {
                                case "ref":
                                case "out":
                                    { 
                                        mTemplateClassStatement = new System.CodeDom.CodeVariableDeclarationStatement(
                                                                        ParamType,
                                                                        paramCodeName,
                                                                        new System.CodeDom.CodePrimitiveExpression(classValue));
                                        codeStatementCollection.Add(mTemplateClassStatement);
                                    }
                                    break;
                            }
                        }
                    }
                }
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
                    case "ref":
                        fd = System.CodeDom.FieldDirection.Ref;
                        break;
                    case "out":
                        fd = System.CodeDom.FieldDirection.Out;
                        break;
                    default:
                        fd = System.CodeDom.FieldDirection.In;
                        break;
                }
                return new System.CodeDom.CodeDirectionExpression(fd, linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true)));
            }
            else
            {
                var fd = System.CodeDom.FieldDirection.In;
                System.CodeDom.CodeExpression exp;
                var paramName = "param_" + CodeGenerateSystem.Program.GetValuedGUIDString(this.Id);

                switch (mParamFlag)
                {
                    case "ref":
                        fd = System.CodeDom.FieldDirection.Ref;
                        exp = new System.CodeDom.CodeArgumentReferenceExpression(paramName);
                        break;
                    case "out":
                        fd = System.CodeDom.FieldDirection.Out;
                        exp = new System.CodeDom.CodeArgumentReferenceExpression(paramName);
                        break;
                    default:
                        {
                            fd = System.CodeDom.FieldDirection.In;
                            var node = ParentNode as MethodInvokeNode;
                            var proInfo = node.TemplateClassInstance.GetType().GetProperty(ParamName);
                            var classValue = proInfo.GetValue(node.TemplateClassInstance);

                            if (ParamType.IsGenericType)
                            {
                                exp = new System.CodeDom.CodeArgumentReferenceExpression(paramName);
                            }
                            else if (ParamType.IsArray)
                            {
                                exp = new System.CodeDom.CodeArgumentReferenceExpression(paramName);
                            }
                            else if (ParamType == typeof(string))
                            {
                                exp = new System.CodeDom.CodePrimitiveExpression(classValue);
                            }
                            else if (ParamType.IsClass)
                            {
                                exp = new System.CodeDom.CodeArgumentReferenceExpression(paramName);
                            }
                            else if(ParamType == typeof(Guid) ||
                                    ParamType == typeof(SlimDX.Vector2) ||
                                    ParamType == typeof(SlimDX.Vector3) ||
                                    ParamType == typeof(SlimDX.Vector4))
                            {
                                exp = new System.CodeDom.CodeArgumentReferenceExpression(paramName);
                            }
                            else
                            {
                                exp = new System.CodeDom.CodePrimitiveExpression(classValue);
                            }
                        }
                        break;
                }
                return new System.CodeDom.CodeDirectionExpression(fd, exp);
            }
        }
    }

}
