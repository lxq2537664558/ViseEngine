using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for Arithmetic.xaml
    /// </summary>
    public partial class Arithmetic : CodeGenerateSystem.Base.BaseNodeControl
    {
        public Arithmetic(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(RectangleTitle);
            SetUpLinkElement(MethodLink_Pre);

            CodeGenerateSystem.Base.enLinkType value1LinkType = CodeGenerateSystem.Base.enLinkType.Unknow;
            CodeGenerateSystem.Base.enLinkType value2LinkType = CodeGenerateSystem.Base.enLinkType.Unknow;
            CodeGenerateSystem.Base.enLinkType resultLinkType = CodeGenerateSystem.Base.enLinkType.Unknow;

            switch (strParam)
            {
                case "＋":
                    value1LinkType = CodeGenerateSystem.Base.enLinkType.NumbericalValue | CodeGenerateSystem.Base.enLinkType.VectorValue | CodeGenerateSystem.Base.enLinkType.String;
                    value2LinkType = CodeGenerateSystem.Base.enLinkType.NumbericalValue | CodeGenerateSystem.Base.enLinkType.VectorValue | CodeGenerateSystem.Base.enLinkType.String;
                    resultLinkType = CodeGenerateSystem.Base.enLinkType.NumbericalValue | CodeGenerateSystem.Base.enLinkType.VectorValue | CodeGenerateSystem.Base.enLinkType.String;
                    break;
                case "－":
                case "×":
                case "÷":
                    value1LinkType = CodeGenerateSystem.Base.enLinkType.NumbericalValue | CodeGenerateSystem.Base.enLinkType.VectorValue;
                    value2LinkType = CodeGenerateSystem.Base.enLinkType.NumbericalValue | CodeGenerateSystem.Base.enLinkType.VectorValue;
                    resultLinkType = CodeGenerateSystem.Base.enLinkType.NumbericalValue | CodeGenerateSystem.Base.enLinkType.VectorValue;
                    break;
                case "&&":
                case "||":
                    value1LinkType = CodeGenerateSystem.Base.enLinkType.Bool;
                    value2LinkType = CodeGenerateSystem.Base.enLinkType.Bool;
                    resultLinkType = CodeGenerateSystem.Base.enLinkType.Bool;
                    break;
                case "&":
                case "|":
                    value1LinkType = CodeGenerateSystem.Base.enLinkType.Byte | CodeGenerateSystem.Base.enLinkType.UInt16 | CodeGenerateSystem.Base.enLinkType.UInt32 | CodeGenerateSystem.Base.enLinkType.UInt64;
                    value2LinkType = CodeGenerateSystem.Base.enLinkType.Byte | CodeGenerateSystem.Base.enLinkType.UInt16 | CodeGenerateSystem.Base.enLinkType.UInt32 | CodeGenerateSystem.Base.enLinkType.UInt64;
                    resultLinkType = CodeGenerateSystem.Base.enLinkType.Byte | CodeGenerateSystem.Base.enLinkType.UInt16 | CodeGenerateSystem.Base.enLinkType.UInt32 | CodeGenerateSystem.Base.enLinkType.UInt64;
                    break;
            }

            TitleLabel.Text = "运算(" + P1_Label.Text + " " + strParam + " " + P2_Label.Text + ")";
            NodeName = TitleLabel.Text;

            AddLinkObject(value1LinkType, Value1, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(value2LinkType, Value2, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(resultLinkType, ResultLink, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ResultLink.BackBrush, true);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Pre, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.End, MethodLink_Pre.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Method, MethodLink_Next, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.Start, MethodLink_Next.BackBrush, false);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            var strValueName = "Value_" + CodeGenerateSystem.Program.GetValuedGUIDString(Id);

            return strValueName;
        }

        // 根据两个运算的类型获得一个合法的类型, 如果两个值不能运算则返回Unknow
        private CodeGenerateSystem.Base.enLinkType GetAvilableType(CodeGenerateSystem.Base.enLinkType type1, CodeGenerateSystem.Base.enLinkType type2)
        {
            switch (StrParams)
            {
                case "＋":
                case "－":
                case "×":
                case "÷":
                    {
                        if (type1 == CodeGenerateSystem.Base.enLinkType.String ||
                           type2 == CodeGenerateSystem.Base.enLinkType.String)
                            return CodeGenerateSystem.Base.enLinkType.String;

                        if((type1 & CodeGenerateSystem.Base.enLinkType.VectorValue) == type1)
                        {
                            if ((type2 & CodeGenerateSystem.Base.enLinkType.NumbericalValue) == type2)
                                return type1;
                            if (type1 == type2)
                                return type1;
                            return CodeGenerateSystem.Base.enLinkType.Unknow;
                        }
                        if ((type2 & CodeGenerateSystem.Base.enLinkType.VectorValue) == type2)
                        {
                            if ((type1 & CodeGenerateSystem.Base.enLinkType.NumbericalValue) == type1)
                                return type2;
                            if (type1 == type2)
                                return type1;
                            return CodeGenerateSystem.Base.enLinkType.Unknow;
                        }
                        if((type1 & CodeGenerateSystem.Base.enLinkType.NumbericalValue) == type1 &&
                           (type2 & CodeGenerateSystem.Base.enLinkType.NumbericalValue) == type2)
                        {
                            if (type1 == CodeGenerateSystem.Base.enLinkType.Double ||
                               type2 == CodeGenerateSystem.Base.enLinkType.Double)
                                return CodeGenerateSystem.Base.enLinkType.Double;
                            else if (type1 == CodeGenerateSystem.Base.enLinkType.Single ||
                                    type2 == CodeGenerateSystem.Base.enLinkType.Single)
                                return CodeGenerateSystem.Base.enLinkType.Single;
                            else
                            {
                                switch(type1)
                                {
                                    case CodeGenerateSystem.Base.enLinkType.UInt64:
                                        {
                                            switch (type2)
                                            {
                                                case CodeGenerateSystem.Base.enLinkType.UInt64:
                                                case CodeGenerateSystem.Base.enLinkType.UInt32:
                                                case CodeGenerateSystem.Base.enLinkType.UInt16:
                                                case CodeGenerateSystem.Base.enLinkType.Byte:
                                                    return CodeGenerateSystem.Base.enLinkType.UInt64;
                                                default:
                                                    return CodeGenerateSystem.Base.enLinkType.Unknow;
                                            }
                                        }
                                    case CodeGenerateSystem.Base.enLinkType.UInt32:
                                        {
                                            switch(type2)
                                            {
                                                case CodeGenerateSystem.Base.enLinkType.UInt64:
                                                    return CodeGenerateSystem.Base.enLinkType.UInt64;
                                                case CodeGenerateSystem.Base.enLinkType.UInt32:
                                                case CodeGenerateSystem.Base.enLinkType.UInt16:
                                                case CodeGenerateSystem.Base.enLinkType.Byte:
                                                    return CodeGenerateSystem.Base.enLinkType.UInt32;
                                                case CodeGenerateSystem.Base.enLinkType.Int64:
                                                case CodeGenerateSystem.Base.enLinkType.Int32:
                                                case CodeGenerateSystem.Base.enLinkType.Int16:
                                                case CodeGenerateSystem.Base.enLinkType.SByte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int64;
                                            }
                                        }
                                        break;
                                    case CodeGenerateSystem.Base.enLinkType.UInt16:
                                        {
                                            switch (type2)
                                            {
                                                case CodeGenerateSystem.Base.enLinkType.UInt64:
                                                    return CodeGenerateSystem.Base.enLinkType.UInt64;
                                                case CodeGenerateSystem.Base.enLinkType.UInt32:
                                                    return CodeGenerateSystem.Base.enLinkType.UInt32;
                                                case CodeGenerateSystem.Base.enLinkType.UInt16:
                                                case CodeGenerateSystem.Base.enLinkType.Byte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int32;
                                                case CodeGenerateSystem.Base.enLinkType.Int64:
                                                    return CodeGenerateSystem.Base.enLinkType.Int64;
                                                case CodeGenerateSystem.Base.enLinkType.Int32:
                                                case CodeGenerateSystem.Base.enLinkType.Int16:
                                                case CodeGenerateSystem.Base.enLinkType.SByte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int32;
                                            }
                                        }
                                        break;
                                    case CodeGenerateSystem.Base.enLinkType.Byte:
                                        {
                                            switch(type2)
                                            {
                                                case CodeGenerateSystem.Base.enLinkType.UInt64:
                                                    return CodeGenerateSystem.Base.enLinkType.UInt64;
                                                case CodeGenerateSystem.Base.enLinkType.UInt32:
                                                    return CodeGenerateSystem.Base.enLinkType.UInt32;
                                                case CodeGenerateSystem.Base.enLinkType.UInt16:
                                                case CodeGenerateSystem.Base.enLinkType.Byte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int32;
                                                case CodeGenerateSystem.Base.enLinkType.Int64:
                                                    return CodeGenerateSystem.Base.enLinkType.Int64;
                                                case CodeGenerateSystem.Base.enLinkType.Int32:
                                                case CodeGenerateSystem.Base.enLinkType.Int16:
                                                case CodeGenerateSystem.Base.enLinkType.SByte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int32;
                                            }
                                        }
                                        break;
                                    case CodeGenerateSystem.Base.enLinkType.Int64:
                                        {
                                            switch(type2)
                                            {
                                                case CodeGenerateSystem.Base.enLinkType.UInt64:
                                                    return CodeGenerateSystem.Base.enLinkType.Unknow;
                                                case CodeGenerateSystem.Base.enLinkType.UInt32:
                                                case CodeGenerateSystem.Base.enLinkType.UInt16:
                                                case CodeGenerateSystem.Base.enLinkType.Byte:
                                                case CodeGenerateSystem.Base.enLinkType.Int64:
                                                case CodeGenerateSystem.Base.enLinkType.Int32:
                                                case CodeGenerateSystem.Base.enLinkType.Int16:
                                                case CodeGenerateSystem.Base.enLinkType.SByte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int64;
                                            }
                                        }
                                        break;
                                    case CodeGenerateSystem.Base.enLinkType.Int32:
                                        {
                                            switch(type2)
                                            {
                                                case CodeGenerateSystem.Base.enLinkType.UInt64:
                                                    return CodeGenerateSystem.Base.enLinkType.Unknow;
                                                case CodeGenerateSystem.Base.enLinkType.UInt32:
                                                case CodeGenerateSystem.Base.enLinkType.Int64:
                                                    return CodeGenerateSystem.Base.enLinkType.Int64;
                                                case CodeGenerateSystem.Base.enLinkType.UInt16:
                                                case CodeGenerateSystem.Base.enLinkType.Byte:
                                                case CodeGenerateSystem.Base.enLinkType.Int32:
                                                case CodeGenerateSystem.Base.enLinkType.Int16:
                                                case CodeGenerateSystem.Base.enLinkType.SByte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int32;
                                            }
                                        }
                                        break;
                                    case CodeGenerateSystem.Base.enLinkType.Int16:
                                        {
                                            switch(type2)
                                            {
                                                case CodeGenerateSystem.Base.enLinkType.UInt64:
                                                    return CodeGenerateSystem.Base.enLinkType.Unknow;
                                                case CodeGenerateSystem.Base.enLinkType.UInt32:
                                                case CodeGenerateSystem.Base.enLinkType.Int64:
                                                    return CodeGenerateSystem.Base.enLinkType.Int64;
                                                case CodeGenerateSystem.Base.enLinkType.UInt16:
                                                case CodeGenerateSystem.Base.enLinkType.Byte:
                                                case CodeGenerateSystem.Base.enLinkType.Int32:
                                                case CodeGenerateSystem.Base.enLinkType.Int16:
                                                case CodeGenerateSystem.Base.enLinkType.SByte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int32;
                                            }
                                        }
                                        break;
                                    case CodeGenerateSystem.Base.enLinkType.SByte:
                                        {
                                            switch(type2)
                                            {
                                                case CodeGenerateSystem.Base.enLinkType.UInt64:
                                                    return CodeGenerateSystem.Base.enLinkType.Unknow;
                                                case CodeGenerateSystem.Base.enLinkType.UInt32:
                                                case CodeGenerateSystem.Base.enLinkType.Int64:
                                                    return CodeGenerateSystem.Base.enLinkType.Int64;
                                                case CodeGenerateSystem.Base.enLinkType.UInt16:
                                                case CodeGenerateSystem.Base.enLinkType.Byte:
                                                case CodeGenerateSystem.Base.enLinkType.Int32:
                                                case CodeGenerateSystem.Base.enLinkType.Int16:
                                                case CodeGenerateSystem.Base.enLinkType.SByte:
                                                    return CodeGenerateSystem.Base.enLinkType.Int32;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case "&&":
                case "||":
                    if (type1 == CodeGenerateSystem.Base.enLinkType.Bool && type2 == CodeGenerateSystem.Base.enLinkType.Bool)
                        return CodeGenerateSystem.Base.enLinkType.Bool;
                    break;
                case "&":
                case "|":
                    if(((type1 & CodeGenerateSystem.Base.enLinkType.UnsignedNumbericalValue) == type1) &&
                       ((type2 & CodeGenerateSystem.Base.enLinkType.UnsignedNumbericalValue) == type2))
                    {
                        switch(type1)
                        {
                            case CodeGenerateSystem.Base.enLinkType.Byte:
                                {
                                    switch(type2)
                                    {
                                        case CodeGenerateSystem.Base.enLinkType.Byte:
                                        case CodeGenerateSystem.Base.enLinkType.UInt16:
                                            return CodeGenerateSystem.Base.enLinkType.Int32;
                                        case CodeGenerateSystem.Base.enLinkType.UInt32:
                                            return CodeGenerateSystem.Base.enLinkType.UInt32;
                                        case CodeGenerateSystem.Base.enLinkType.UInt64:
                                            return CodeGenerateSystem.Base.enLinkType.UInt64;
                                    }
                                }
                                break;
                            case CodeGenerateSystem.Base.enLinkType.UInt16:
                                {
                                    switch(type2)
                                    {
                                        case CodeGenerateSystem.Base.enLinkType.Byte:
                                        case CodeGenerateSystem.Base.enLinkType.UInt16:
                                            return CodeGenerateSystem.Base.enLinkType.Int32;
                                        case CodeGenerateSystem.Base.enLinkType.UInt32:
                                            return CodeGenerateSystem.Base.enLinkType.UInt32;
                                        case CodeGenerateSystem.Base.enLinkType.UInt64:
                                            return CodeGenerateSystem.Base.enLinkType.UInt64;
                                    }
                                }
                                break;
                            case CodeGenerateSystem.Base.enLinkType.UInt32:
                                {
                                    switch(type2)
                                    {
                                        case CodeGenerateSystem.Base.enLinkType.Byte:
                                        case CodeGenerateSystem.Base.enLinkType.UInt16:
                                        case CodeGenerateSystem.Base.enLinkType.UInt32:
                                            return CodeGenerateSystem.Base.enLinkType.UInt32;
                                        case CodeGenerateSystem.Base.enLinkType.UInt64:
                                            return CodeGenerateSystem.Base.enLinkType.UInt64;
                                    }
                                }
                                break;
                            case CodeGenerateSystem.Base.enLinkType.UInt64:
                                {
                                    switch(type2)
                                    {
                                        case CodeGenerateSystem.Base.enLinkType.Byte:
                                        case CodeGenerateSystem.Base.enLinkType.UInt16:
                                        case CodeGenerateSystem.Base.enLinkType.UInt32:
                                        case CodeGenerateSystem.Base.enLinkType.UInt64:
                                            return CodeGenerateSystem.Base.enLinkType.UInt64;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

            return CodeGenerateSystem.Base.enLinkType.Unknow;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if(element == ResultLink || element == null)
            {
                var linkOILeft = GetLinkObjInfo(Value1);
                var linkOIRight = GetLinkObjInfo(Value2);
                if(linkOILeft.HasLink && linkOIRight.HasLink)
                {
                    var leftType = linkOILeft.GetLinkType(0, true);
                    var rightType = linkOIRight.GetLinkType(0, true);

                    var retType = GetAvilableType(leftType, rightType);
                    switch (retType)
                    {
                        case CodeGenerateSystem.Base.enLinkType.Bool:
                            return "System.Boolean";
                        case CodeGenerateSystem.Base.enLinkType.Int32:
                            return "System.Int32";
                        case CodeGenerateSystem.Base.enLinkType.Int64:
                            return "System.Int64";
                        case CodeGenerateSystem.Base.enLinkType.Single:
                            return "System.Single";
                        case CodeGenerateSystem.Base.enLinkType.Double:
                            return "System.Double";
                        case CodeGenerateSystem.Base.enLinkType.String:
                            return "System.String";
                        case CodeGenerateSystem.Base.enLinkType.Vector2:
                            return "SlimDX.Vector2";
                        case CodeGenerateSystem.Base.enLinkType.Vector3:
                            return "SlimDX.Vector3";
                        case CodeGenerateSystem.Base.enLinkType.Vector4:
                            return "SlimDX.Vector4";
                        case CodeGenerateSystem.Base.enLinkType.Byte:
                            return "System.Byte";
                        case CodeGenerateSystem.Base.enLinkType.SByte:
                            return "System.SByte";
                        case CodeGenerateSystem.Base.enLinkType.Int16:
                            return "System.Int16";
                        case CodeGenerateSystem.Base.enLinkType.UInt16:
                            return "System.UInt16";
                        case CodeGenerateSystem.Base.enLinkType.UInt32:
                            return "System.UInt32";
                        case CodeGenerateSystem.Base.enLinkType.UInt64:
                            return "System.UInt64";
                    }
                }
            }

            return base.GCode_GetValueType(element);
        }

        protected override void CollectionErrorMsg()
        {
            var methodLinkOI = GetLinkObjInfo(MethodLink_Pre);
            var resultLinkOI = GetLinkObjInfo(ResultLink);
            if (methodLinkOI.HasLink || resultLinkOI.HasLink)
            {
                var linkOILeft = GetLinkObjInfo(Value1);
                if (!linkOILeft.HasLink)
                    AddErrorMsg(Value1, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "未设置左参");
                var linkOIRight = GetLinkObjInfo(Value2);
                if (!linkOIRight.HasLink)
                    AddErrorMsg(Value2, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "未设置右参");

                var leftType = linkOILeft.GetLinkType(0, true);
                var rightType = linkOILeft.GetLinkType(0, true);
                if (GetAvilableType(leftType, rightType) == CodeGenerateSystem.Base.enLinkType.Unknow)
                    AddErrorMsg(new System.Collections.Generic.List<FrameworkElement>() { Value1, Value2 },
                                CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error,
                                "类型" + leftType + "与类型" + rightType + "不能进行" + StrParams + "运算");
            }
        }

        public override bool HasMultiOutLink
        {
            get
            {
                var linkOI = GetLinkObjInfo(ResultLink);
                return linkOI.LinkInfos.Count > 0;
            }
        }

        System.CodeDom.CodeAssignStatement mVariableDeclaration;
        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            var linkOI1 = GetLinkObjInfo(Value1);
            var linkOI2 = GetLinkObjInfo(Value2);
            if (!linkOI1.HasLink || !linkOI2.HasLink)
                return;

            // 参数1
            if (!linkOI1.GetLinkObject(0, true).IsOnlyReturnValue)
                linkOI1.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI1.GetLinkElement(0, true));

            System.CodeDom.CodeExpression valueExp1 = linkOI1.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI1.GetLinkElement(0, true));

            // 参数2
            if (!linkOI2.GetLinkObject(0, true).IsOnlyReturnValue)
                linkOI2.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI2.GetLinkElement(0, true));

            System.CodeDom.CodeExpression valueExp2 = linkOI2.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI2.GetLinkElement(0, true));

            // 运算
            var arithmeticExp = new System.CodeDom.CodeBinaryOperatorExpression();
            arithmeticExp.Left = valueExp1;
            arithmeticExp.Right = valueExp2;
            switch (StrParams)
            {
                case "＋":
                    arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.Add;
                    break;
                case "－":
                    arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.Subtract;
                    break;
                case "×":
                    arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.Multiply;
                    break;
                case "÷":
                    arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.Divide;
                    break;
                case "&&":
                    arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.BooleanAnd;
                    break;
                case "||":
                    arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.BooleanOr;
                    break;
                case "&":
                    arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.BitwiseAnd;
                    break;
                case "|":
                    arithmeticExp.Operator = System.CodeDom.CodeBinaryOperatorType.BitwiseOr;
                    break;
            }

            if (element == MethodLink_Pre)
            {
                var linkResult = GetLinkObjInfo(ResultLink);
                if (linkResult.HasLink)
                {
                    var leftExp = linkResult.GetLinkObject(0, false).GCode_CodeDom_GetValue(linkResult.GetLinkElement(0, false));

                    if (leftExp != null)
                    {
                        System.CodeDom.CodeAssignStatement valueAss = new System.CodeDom.CodeAssignStatement();
                        valueAss.Left = leftExp;
                        valueAss.Right = arithmeticExp;
                        codeStatementCollection.Add(valueAss);
                    }
                    else
                    {
                        codeStatementCollection.Add(new System.CodeDom.CodeCommentStatement(this.GetType().ToString() + " 被赋值对象无法取名称, 无法生成代码!"));
                    }
 
                }

                var linkOI = GetLinkObjInfo(MethodLink_Next);
                if(linkOI.HasLink)
                {
                    linkOI.GetLinkObject(0, false).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, false));
                }
            }
            else if (element == ResultLink)
            {
                // 创建结果并赋值
                if (mVariableDeclaration == null)
                {
                    mVariableDeclaration = new System.CodeDom.CodeAssignStatement();
                    mVariableDeclaration.Left = new System.CodeDom.CodeVariableReferenceExpression(GCode_GetValueName(null));
                    mVariableDeclaration.Right = arithmeticExp;
                }

                if (codeStatementCollection.Contains(mVariableDeclaration))
                {
                    //var assign = new System.CodeDom.CodeAssignStatement(GCode_CodeDom_GetValue(null) , arithmeticExp);
                    //codeStatementCollection.Add(assign);
                }
                else
                    codeStatementCollection.Add(mVariableDeclaration);
            }
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodeVariableReferenceExpression(GCode_GetValueName(null));
        }
    }
}
