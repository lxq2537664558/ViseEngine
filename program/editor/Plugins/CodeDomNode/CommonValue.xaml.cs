using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for CommonValue.xaml
    /// </summary>
    /// // 简单类型的参数（只有一个值，非集合，如整形、字符串等）
    public partial class CommonValue : CodeGenerateSystem.Base.BaseNodeControl
    {
        public string mStrValue { get; set; }
        Type mValueType;

        public CommonValue(Canvas parentCanvas, String strParams)
            : base(parentCanvas, strParams)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;
            ValueGrid.DataContext = this;
            SetDragObject(RectangleTitle);

            String[] splits = strParams.Split(',');
            if(splits[0].Equals("Common"))
            {
                // 通用数值处理
#warning 通用数值处理
            }
            else
            {
                mValueType = Type.GetType(splits[0]);
                TitleLabel.Text = mValueType.Name;

                AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(mValueType),
                              ValueLinkHandle,
                              CodeGenerateSystem.Base.enBezierType.Right,
                              CodeGenerateSystem.Base.enLinkOpType.Start,
                              ValueLinkHandle.BackBrush,
                              true);
                AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(mValueType),
                              SetValueLinkHandle,
                              CodeGenerateSystem.Base.enBezierType.Left,
                              CodeGenerateSystem.Base.enLinkOpType.End,
                              null, true);
            }
            mStrValue = "";

            if (splits.Length > 1)
                mStrValue = splits[1];
            if (splits.Length > 2)
                ValueNameTextBox.Text = splits[2];

            ValueTextBox.Text = mStrValue;
            NodeName = mStrValue;
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            if(mValueType == null)
            {
#warning 通用数值处理

            }
            else
                StrParams = mValueType.ToString() + "," + mStrValue + "," + ValueNameTextBox.Text;

            base.Save(xmlNode, newGuid, holder);
        }

        protected bool IsNumberic(string message)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (rex.IsMatch(message))
                return true;
            else
                return false;
        }

#region 生成代码

        public override string GCode_GetValueName(FrameworkElement element)
        {
            string strValueName;
            if (String.IsNullOrEmpty(ValueNameTextBox.Text))
            {
                strValueName = "Value_" + CodeGenerateSystem.Program.GetValuedGUIDString(Id);
            }
            else
                strValueName = ValueNameTextBox.Text;

            return strValueName;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (mValueType == null)
            {
#warning 通用数值处理
                return "";
            }

            return mValueType.ToString();
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            if (element == ValueLinkHandle)
            {
                if(mValueType == null)
                {

#warning 通用数值处理
                }
                else if (mValueType == typeof(string))
                    return new System.CodeDom.CodePrimitiveExpression(mStrValue);
                else if (mValueType == typeof(bool))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToBoolean(mStrValue));
                else if (mValueType == typeof(SByte))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToSByte(mStrValue));
                else if (mValueType == typeof(Int16))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToInt16(mStrValue));
                else if (mValueType == typeof(Int32))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToInt32(mStrValue));
                else if (mValueType == typeof(Int64))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToInt64(mStrValue));
                else if (mValueType == typeof(Byte))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToByte(mStrValue));
                else if (mValueType == typeof(UInt16))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToUInt16(mStrValue));
                else if (mValueType == typeof(UInt32))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToUInt32(mStrValue));
                else if (mValueType == typeof(UInt64))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToUInt64(mStrValue));
                else if (mValueType == typeof(Single))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToSingle(mStrValue));
                else if (mValueType == typeof(Double))
                    return new System.CodeDom.CodePrimitiveExpression(System.Convert.ToDouble(mStrValue));
            }
            else
                return new System.CodeDom.CodeVariableReferenceExpression(GCode_GetValueName(element));

            return base.GCode_CodeDom_GetValue(element);
        }

#endregion
    }
}
