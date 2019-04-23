using System;
using System.Windows;
using System.Windows.Controls;

namespace AIEditor.LinkSystem.Value
{
    /// <summary>
    /// Interaction logic for Vector3.xaml
    /// </summary>
    [AIEditor.ShowInAIEditorMenu("参数.向量.Vector3")]
    public partial class Vector3 : CodeGenerateSystem.Base.BaseNodeControl
    {
        public Single x { get; set; }
        public Single y { get; set; }
        public Single z { get; set; }

        System.CodeDom.CodeVariableDeclarationStatement mVec3VariableDeclaration = new System.CodeDom.CodeVariableDeclarationStatement();

        public Vector3(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            ValueGrid.DataContext = this;

            SetDragObject(RectangleTitle);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Vector3, ValueLinkHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueLinkHandle.BackBrush, true);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Single, value_X, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Single, value_Y, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Single, value_Z, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Vector3, value_XYZ, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.ClassField, ClassFieldLinkHandle, CodeGenerateSystem.Base.enBezierType.Top, CodeGenerateSystem.Base.enLinkOpType.Start, ClassFieldLinkHandle.BackBrush, false);

            if (strParam != null)
            {
                string[] splits = strParam.Split(',');
                if (splits.Length < 4)
                {
                    //ValueNameBox.Text = "strParam.Split failed";
                    return;
                }
                x = System.Convert.ToSingle(splits[0]);
                y = System.Convert.ToSingle(splits[1]);
                z = System.Convert.ToSingle(splits[2]);

                ValueNameBox.Text = splits[3];
            }
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            StrParams = x + "," + y + "," + z + "," + ValueNameBox.Text;
            base.Save(xmlNode, newGuid,holder);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (String.IsNullOrEmpty(ValueNameBox.Text))
            {
                return "Vec_" + Program.GetValuedGUIDString(Id);
            }
            else
                return ValueNameBox.Text;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "SlimDX.Vector3";
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            if (element == ValueLinkHandle)
                return new System.CodeDom.CodeVariableReferenceExpression(GCode_GetValueName(null));
            else
                return new System.CodeDom.CodeObjectCreateExpression("SlimDX.Vector3", new System.CodeDom.CodeExpression[] {
                                                                                                             new System.CodeDom.CodePrimitiveExpression(x),
                                                                                                             new System.CodeDom.CodePrimitiveExpression(y),
                                                                                                             new System.CodeDom.CodePrimitiveExpression(z)});

            //return base.GCode_CodeDom_GetValue(element);
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            bool bHasXYZ = false;
            string strValueName = GCode_GetValueName(null);

            CodeGenerateSystem.Base.LinkObjInfo classFieldLink = GetLinkObjInfo(ClassFieldLinkHandle);
            CodeGenerateSystem.Base.LinkObjInfo linkOI = GetLinkObjInfo(value_XYZ);
            if (linkOI.HasLink)
            {
                bHasXYZ = true;

                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                {
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, element);
                }

                if (classFieldLink.HasLink)
                {
                    // 类的成员变量
                    System.CodeDom.CodeAssignStatement valAss = new System.CodeDom.CodeAssignStatement();
                    valAss.Left = new System.CodeDom.CodeVariableReferenceExpression(strValueName);
                    valAss.Right = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));
                    codeStatementCollection.Add(valAss);
                }
                else
                {
                    if (!codeStatementCollection.Contains(mVec3VariableDeclaration))
                    {
                        // 非类的成员变量
                        mVec3VariableDeclaration.Type = new System.CodeDom.CodeTypeReference("SlimDX.Vector3");
                        mVec3VariableDeclaration.Name = strValueName;
                        mVec3VariableDeclaration.InitExpression = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));
                        codeStatementCollection.Add(mVec3VariableDeclaration);
                    }
                }
            }

            if (!bHasXYZ && !classFieldLink.HasLink)
            {
                if (!codeStatementCollection.Contains(mVec3VariableDeclaration))
                {
                    // 非类的成员变量则进行初始化操作
                    mVec3VariableDeclaration.Type = new System.CodeDom.CodeTypeReference("SlimDX.Vector3");
                    mVec3VariableDeclaration.Name = strValueName;
                    mVec3VariableDeclaration.InitExpression = new System.CodeDom.CodeObjectCreateExpression("SlimDX.Vector3", new System.CodeDom.CodeExpression[] {
                                                                                                                                        new System.CodeDom.CodePrimitiveExpression(x),
                                                                                                                                        new System.CodeDom.CodePrimitiveExpression(y),
                                                                                                                                        new System.CodeDom.CodePrimitiveExpression(z)});
                    codeStatementCollection.Add(mVec3VariableDeclaration);
                }
            }

            // X
            linkOI = GetLinkObjInfo(value_X);
            if (linkOI.HasLink)
            {
                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));

                System.CodeDom.CodeFieldReferenceExpression fieldRef = new System.CodeDom.CodeFieldReferenceExpression();
                fieldRef.TargetObject = new System.CodeDom.CodeVariableReferenceExpression(strValueName);
                fieldRef.FieldName = "X";
                System.CodeDom.CodeAssignStatement statValAss = new System.CodeDom.CodeAssignStatement();
                statValAss.Left = fieldRef;
                statValAss.Right = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));
                codeStatementCollection.Add(statValAss);
            }

            // Y
            linkOI = GetLinkObjInfo(value_Y);
            if (linkOI.HasLink)
            {
                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));

                System.CodeDom.CodeFieldReferenceExpression fieldRef = new System.CodeDom.CodeFieldReferenceExpression();
                fieldRef.TargetObject = new System.CodeDom.CodeVariableReferenceExpression(strValueName);
                fieldRef.FieldName = "Y";
                System.CodeDom.CodeAssignStatement statValAss = new System.CodeDom.CodeAssignStatement();
                statValAss.Left = fieldRef;
                statValAss.Right = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));
                codeStatementCollection.Add(statValAss);
            }

            // Z
            linkOI = GetLinkObjInfo(value_Z);
            if (linkOI.HasLink)
            {
                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));

                System.CodeDom.CodeFieldReferenceExpression fieldRef = new System.CodeDom.CodeFieldReferenceExpression();
                fieldRef.TargetObject = new System.CodeDom.CodeVariableReferenceExpression(strValueName);
                fieldRef.FieldName = "Z";
                System.CodeDom.CodeAssignStatement statValAss = new System.CodeDom.CodeAssignStatement();
                statValAss.Left = fieldRef;
                statValAss.Right = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));
                codeStatementCollection.Add(statValAss);
            }
        }
    }
}
