using System.Windows;
using System.Windows.Controls;

namespace AIEditor.LinkSystem.Value
{
    /// <summary>
    /// Interaction logic for Camera.xaml
    /// </summary>
    [AIEditor.ShowInAIEditorMenu("参数.Camera")]
    public partial class Camera : CodeGenerateSystem.Base.BaseNodeControl
    {
        System.CodeDom.CodeVariableDeclarationStatement mRoleVDS = new System.CodeDom.CodeVariableDeclarationStatement();
        System.CodeDom.CodeVariableDeclarationStatement mCameraVDS = new System.CodeDom.CodeVariableDeclarationStatement();

        public Camera(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Vector3, XVectorHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, XVectorHandle.BackBrush, true);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Vector3, YVectorHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, YVectorHandle.BackBrush, true);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Vector3, ZVectorHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ZVectorHandle.BackBrush, true);
        }

#region 代码生成

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            var fieldRef = new System.CodeDom.CodeFieldReferenceExpression();
            fieldRef.TargetObject = new System.CodeDom.CodeVariableReferenceExpression("eye");

            if (element == XVectorHandle)
            {
                var linkOI = GetLinkObjInfo(XVectorHandle);
                if (linkOI.HasLink)
                    fieldRef.FieldName = "GetXVector()";
            }

            if (element == YVectorHandle)
            {
                var linkOI = GetLinkObjInfo(YVectorHandle);
                if (linkOI.HasLink)
                    fieldRef.FieldName = "GetYVector()";
            }

            if (element == ZVectorHandle)
            {
                var linkOI = GetLinkObjInfo(ZVectorHandle);
                if (linkOI.HasLink)
                    fieldRef.FieldName = "GetZVector()";
            }

            return fieldRef;
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            if (!codeStatementCollection.Contains(mRoleVDS))
            {
                mRoleVDS.Type = new System.CodeDom.CodeTypeReference("Role.RoleActor");
                mRoleVDS.Name = "role";
                mRoleVDS.InitExpression = new System.CodeDom.CodeCastExpression("Role.RoleActor", new System.CodeDom.CodeFieldReferenceExpression(
                                                                                                        new System.CodeDom.CodeVariableReferenceExpression("HostStateSet"),
                                                                                                        "mBindActor"));
                codeStatementCollection.Add(mRoleVDS);
            }

            if (!codeStatementCollection.Contains(mCameraVDS))
            {
                mCameraVDS.Type = new System.CodeDom.CodeTypeReference("MidLayer.ICamera");
                mCameraVDS.Name = "eye";
                mCameraVDS.InitExpression = new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression("role.ActorController.CameraController"), "Camera");
                codeStatementCollection.Add(mCameraVDS);
            }
        }

#endregion
    }
}
