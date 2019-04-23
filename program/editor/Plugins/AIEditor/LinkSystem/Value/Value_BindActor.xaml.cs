using System.Windows;
using System.Windows.Controls;

namespace AIEditor.LinkSystem.Value
{
    /// <summary>
    /// Interaction logic for Value_BindActor.xaml
    /// </summary>
    //[AIEditor.ShowInAIEditorMenu("类.BindActor")]
    public partial class Value_BindActor : CodeGenerateSystem.Base.BaseNodeControl
    {
        CSUtility.Helper.enCSType mCSType = CSUtility.Helper.enCSType.Common;

        public Value_BindActor(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;
            mCSType = (CSUtility.Helper.enCSType)System.Enum.Parse(typeof(CSUtility.Helper.enCSType), strParam);

            SetDragObject(Rectangle_Title);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class, ClassHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ClassHandle.BackBrush, true);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            switch (mCSType)
            {
                case CSUtility.Helper.enCSType.Client:
                    return "FrameSet.Role.RoleActor";
                case CSUtility.Helper.enCSType.Server:
                    return "ServerCommon.Planes.Role.RoleActor";
            }

            return "CSUtility.Component.IActorBase";

        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            switch (mCSType)
            {
                case CSUtility.Helper.enCSType.Client:
                    {
                        return new System.CodeDom.CodeCastExpression("FrameSet.Role.RoleActor", 
                                                                    new System.CodeDom.CodeVariableReferenceExpression("Host"));
                    }

                case CSUtility.Helper.enCSType.Server:
                    {
                        return new System.CodeDom.CodeCastExpression("ServerCommon.Planes.Role.RoleActor",
                                                                    new System.CodeDom.CodeVariableReferenceExpression("Host"));
                    }
            }

            return null;
        }
    }
}
