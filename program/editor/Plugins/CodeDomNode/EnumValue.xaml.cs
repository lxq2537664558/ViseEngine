using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for EnumValue.xaml
    /// </summary>
    public partial class EnumValue : CodeGenerateSystem.Base.BaseNodeControl
    {
        Type mEnumType;

        public EnumValue(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            IsOnlyReturnValue = true;

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Int32, ValueLinkHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueLinkHandle.BackBrush, true);

            if (!string.IsNullOrEmpty(strParam))
            {
                var splits = strParam.Split(',');

                NodeName = splits[0];
                mEnumType = CSUtility.Program.GetTypeFromSaveString(splits[1]);
                Combo_Keys.ItemsSource = System.Enum.GetNames(mEnumType);
                Combo_Keys.SelectedItem = splits[2];
            }
        }

        public static string GetEnumParam(string path, Type paramType)
        {
            if (!paramType.IsEnum)
                return "";

            string retStr = path;
            retStr += "," + CSUtility.Program.GetTypeSaveString(paramType);
            retStr += "," + System.Enum.GetName(paramType, 0);
            return retStr;
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            StrParams = Combo_Keys.SelectedItem.ToString() + "," + mEnumType.ToString();

            base.Save(xmlNode, newGuid, holder);
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeTypeReferenceExpression(mEnumType), Combo_Keys.SelectedItem.ToString());
        }
    }
}
