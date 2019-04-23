using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for Value_Boolean.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("数值.Boolean(布尔)", "布尔数值节点，提供true或false值")]
    public partial class Value_Boolean : CodeGenerateSystem.Base.BaseNodeControl
    {
        bool mValue = true;

        public Value_Boolean(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            String[] splits = strParam.Split(',');
            if (splits.Length == 2)
            {
                mValue = System.Convert.ToBoolean(splits[0]);
                ValueNameTextBox.Text = splits[1];

                if (mValue)
                {
                    ComboBox_TF.SelectedIndex = 0;
                }
                else
                    ComboBox_TF.SelectedIndex = 1;
            }

            IsOnlyReturnValue = true;
            SetDragObject(RectangleTitle);
            NodeName = "数值.Boolean(布尔)";

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Bool, ValueLinkHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueLinkHandle.BackBrush, true);
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            StrParams = mValue.ToString() + "," + ValueNameTextBox.Text;

            base.Save(xmlNode, newGuid, holder);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "System.Boolean";
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodePrimitiveExpression(mValue);
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_TF.SelectedIndex < 0)
                return;

            switch (ComboBox_TF.SelectedIndex)
            {
                case 0:
                    mValue = true;
                    break;

                case 1:
                    mValue = false;
                    break;
            }
        }
    }
}
