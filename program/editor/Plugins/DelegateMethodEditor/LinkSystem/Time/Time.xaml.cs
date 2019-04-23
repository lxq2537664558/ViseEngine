using System.Windows;
using System.Windows.Controls;

namespace DelegateMethodEditor.LinkSystem.Time
{
    /// <summary>
    /// Interaction logic for Time.xaml
    /// </summary>
    [DelegateMethodEditor.ShowInEditorMenu("参数.时间.Time")]
    public partial class Time : CodeGenerateSystem.Base.BaseNodeControl
    {
        public Time(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;
            SetDragObject(RectangleTitle);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Single,
                          ValueLinkHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueLinkHandle.BackBrush, true);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "System.Single";
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodeSnippetExpression("CSUtility.Helper.TimeMgr.Instance.GetFrameSecondTimeFloat()");
        }
    }
}
