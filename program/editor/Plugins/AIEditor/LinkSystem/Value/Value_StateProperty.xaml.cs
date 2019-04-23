using System;
using System.Windows;
using System.Windows.Controls;

namespace AIEditor.LinkSystem.Value
{
    /// <summary>
    /// Interaction logic for Value_StateProperty.xaml
    /// </summary>
    public partial class Value_StateProperty : CodeGenerateSystem.Base.BaseNodeControl
    {
        Type mStatePropertyType = null;

        bool mIsOut = true;
        public bool IsOut
        {
            get { return mIsOut; }
            set
            {
                mIsOut = value;

                if (mIsOut)
                {
                    if (ValueLinkHandle_Out != null)
                        ValueLinkHandle_Out.Visibility = System.Windows.Visibility.Visible;
                    if (ValueLinkHandle_In != null)
                        ValueLinkHandle_In.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    if (ValueLinkHandle_Out != null)
                        ValueLinkHandle_Out.Visibility = System.Windows.Visibility.Collapsed;
                    if (ValueLinkHandle_In != null)
                        ValueLinkHandle_In.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        public Value_StateProperty(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;
            SetDragObject(RectangleTitle);

            if (!string.IsNullOrEmpty(strParam))
            {
                var splits = strParam.Split(',');

                mStatePropertyType = Program.GetType(splits[0]);
                AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(mStatePropertyType),
                              ValueLinkHandle_Out,
                              CodeGenerateSystem.Base.enBezierType.Right,
                              CodeGenerateSystem.Base.enLinkOpType.Start,
                              ValueLinkHandle_Out.BackBrush, false);
                AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromCommonType(mStatePropertyType),
                              ValueLinkHandle_In,
                              CodeGenerateSystem.Base.enBezierType.Left,
                              CodeGenerateSystem.Base.enLinkOpType.End,
                              ValueLinkHandle_In.BackBrush, false);

                TitleLabel.Text = splits[1];
            }
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            StrParams = mStatePropertyType.ToString() + "," + TitleLabel.Text;

            xmlNode.AddAttrib("IsOut", IsOut.ToString());

            base.Save(xmlNode, newGuid,holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            var att = xmlNode.FindAttrib("IsOut");
            if (att != null)
            {
                IsOut = System.Convert.ToBoolean(att.Value);

                if (IsOut)
                    ComboBox_InOut.SelectedIndex = 0;
                else
                    ComboBox_InOut.SelectedIndex = 1;
            }

            base.Load(xmlNode, deltaX, deltaY);
        }

        private void ComboBox_InOut_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (ComboBox_InOut.SelectedIndex)
            {
                case 0:
                    {
                        IsOut = true;
                    }
                    break;

                case 1:
                    {
                        IsOut = false;
                    }
                    break;
            }

            if (ValueLinkHandle_In != null)
            {
                var linkIn = GetLinkObjInfo(ValueLinkHandle_In);
                if (linkIn != null)
                    linkIn.Clear();
            }
            if (ValueLinkHandle_Out != null)
            {
                var linkOut = GetLinkObjInfo(ValueLinkHandle_Out);
                if (linkOut != null)
                    linkOut.Clear();
            }
        }

#region 代码生成

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (mStatePropertyType != null)
                return mStatePropertyType.FullName;

            return "";
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeThisReferenceExpression(), TitleLabel.Text);
        }

#endregion
    }
}
