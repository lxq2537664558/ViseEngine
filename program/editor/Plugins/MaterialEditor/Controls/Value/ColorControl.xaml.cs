using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CCore.Material;
using CSUtility.Support;

namespace MaterialEditor.Controls
{
    /// <summary>
    /// Color.xaml 的交互逻辑
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("参数.颜色", "设置颜色值")]
    public partial class ColorControl : BaseNodeControl_ShaderVar
    {
        // 临时类，用于选中后显示参数属性
        CodeGenerateSystem.Base.GeneratorClassBase mTemplateClassInstance = null;
        public CodeGenerateSystem.Base.GeneratorClassBase TemplateClassInstance
        {
            get { return mTemplateClassInstance; }
        }
        
        string mValueName = "";
        public string ValueName
        {
            get { return mValueName; }
            set
            {
                if (mValueName == value)
                    return;

                var oldValue = mValueName;
                mValueName = value;

                if (ShaderVarInfo != null)
                {
                    if (ShaderVarInfo.Rename(GCode_GetValueName(null)) == false)
                    {
                        EditorCommon.MessageBox.Show("名称" + GCode_GetValueName(null) + "已经被使用，请换其他名称");
                        mValueName = oldValue;
                    }
                }

                IsDirty = true;

                OnPropertyChanged("ValueName");
            }
        }

        SolidColorBrush mColorBrush = Brushes.White;
        public SolidColorBrush ColorBrush
        {
            get { return mColorBrush; }
            set
            {
                mColorBrush = value;
                OnPropertyChanged("ColorBrush");
            }
        }

        public object ColorObject
        {
            get { return GetValue(ColorObjectProperty); }
            set { SetValue(ColorObjectProperty, value); }
        }
        public static readonly DependencyProperty ColorObjectProperty =
            DependencyProperty.Register("ColorObject", typeof(object), typeof(ColorControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnColorObjectChanged)));

        public static void OnColorObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ColorControl;

            var newColor = (CSUtility.Support.Color)e.NewValue;
            if(newColor != null)
            {
                control.ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B));
                control.ShaderVarInfo.VarValue = control.GetValueString();
            }

            control.IsDirty = true;
        }

        string mStrValueType = "float4";
        List<CodeGenerateSystem.Controls.LinkInControl> mInComponentLinks = new List<CodeGenerateSystem.Controls.LinkInControl>();
        List<CodeGenerateSystem.Controls.LinkOutControl> mOutComponentLinks = new List<CodeGenerateSystem.Controls.LinkOutControl>();

        public ColorControl(Canvas parentDrawCanvas, string strParam)
            : base(parentDrawCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);

            ShaderVarInfo.EditorType = "Color";
            //ShaderVarInfo.VarName = GCode_GetValueName(null);

            // 创建用于显示属性的临时类
            var cpInfos = new List<CodeGenerateSystem.Base.CustomPropertyInfo>();
            var cpInfo = CodeGenerateSystem.Base.CustomPropertyInfo.GetFromParamInfo(typeof(CSUtility.Support.Color), "Color", new Attribute[] { new DisplayNameAttribute("颜色") });
            cpInfos.Add(cpInfo);
            var classType = CodeGenerateSystem.Base.PropertyClassGenerator.CreateTypeFromCustomPropertys(cpInfos);
            mTemplateClassInstance = System.Activator.CreateInstance(classType) as CodeGenerateSystem.Base.GeneratorClassBase;
            var property = classType.GetProperty("Color");
            property.SetValue(mTemplateClassInstance, CSUtility.Support.Color.FromArgb(255,255,255,255));
            BindingOperations.SetBinding(this, ColorControl.ColorObjectProperty, new Binding("Color") { Source = mTemplateClassInstance });

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Float4, ValueIn, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Float4, ValueOut, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueOut.BackBrush, true);

            mInComponentLinks.Add(ValueInR);
            mInComponentLinks.Add(ValueInG);
            mInComponentLinks.Add(ValueInB);
            mInComponentLinks.Add(ValueInA);
            foreach(var link in mInComponentLinks)
                AddLinkObject(CodeGenerateSystem.Base.enLinkType.Float1, link, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
            mOutComponentLinks.Add(ValueOutR);
            mOutComponentLinks.Add(ValueOutG);
            mOutComponentLinks.Add(ValueOutB);
            mOutComponentLinks.Add(ValueOutA);
            foreach (var link in mOutComponentLinks)
                AddLinkObject(CodeGenerateSystem.Base.enLinkType.Float1, link, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, link.BackBrush, true);

            InitializeShaderVarInfo();
        }

        public override object GetShowPropertyObject()
        {
            return mTemplateClassInstance;
        }
        public override void Save(XmlNode xmlNode, bool newGuid, XmlHolder holder)
        {
            if(mTemplateClassInstance != null)
            {
                var node = xmlNode.AddNode("DefaultParamValue", "", holder);
                mTemplateClassInstance.Save(node, holder);
            }

            xmlNode.AddAttrib("ValueName", ValueName);
            xmlNode.AddAttrib("IsGeneric", IsGeneric.ToString());
            base.Save(xmlNode, newGuid, holder);
        }
        public override void Load(XmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);

            var nameValue = xmlNode.FindAttrib("ValueName");
            if (nameValue != null)
            {
                NameTextBox.Text = nameValue.Value;
                mValueName = nameValue.Value;
            }
            var isAtt = xmlNode.FindAttrib("IsGeneric");
            if (isAtt != null)
                IsGeneric = System.Convert.ToBoolean(isAtt.Value);

            if (mTemplateClassInstance != null)
            {
                var node = xmlNode.FindNode("DefaultParamValue");
                if (node != null)
                {
                    mTemplateClassInstance.Load(node);
                    var property = mTemplateClassInstance.GetType().GetProperty("Color");
                    var color = (CSUtility.Support.Color)property.GetValue(mTemplateClassInstance);
                    ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                }
            }

            InitializeShaderVarInfo();
        }
        
        protected override void InitializeShaderVarInfo()
        {
            ShaderVarInfo.VarName = GCode_GetValueName(null);
            ShaderVarInfo.VarType = GCode_GetValueType(null);
            ShaderVarInfo.VarValue = GetValueString();
        }

        public string GetValueString()
        {
            string retStr = (ColorBrush.Color.R / 255.0f).ToString() + "," +
                            (ColorBrush.Color.G / 255.0f).ToString() + "," +
                            (ColorBrush.Color.B / 255.0f).ToString() + "," +
                            (ColorBrush.Color.A / 255.0f).ToString();
            return retStr;
        }

        private void NameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            BindingExpression be = NameTextBox.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }

        private void NameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        NameTextBox.Text = ValueName;
                    }
                    break;

                case Key.Enter:
                    {
                        BindingExpression be = NameTextBox.GetBindingExpression(TextBox.TextProperty);
                        be.UpdateSource();
                    }
                    break;
            }
        }

        #region 代码生成

        public override string GCode_GetValueName(FrameworkElement element)
        {
            string strValueName = "";
            if (element == null || element == ValueOut || element == ValueIn)
            {
                if (string.IsNullOrEmpty(ValueName))
                    strValueName = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + CodeGenerateSystem.Program.GetValuedGUIDString(Id);
                else
                    strValueName = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + ValueName;
            }
            else if (element == ValueOutR || element == ValueInR)
                strValueName = GCode_GetValueName(null) + ".x";
            else if (element == ValueOutG || element == ValueInG)
                strValueName = GCode_GetValueName(null) + ".y";
            else if (element == ValueOutB || element == ValueInB)
                strValueName = GCode_GetValueName(null) + ".z";
            else if (element == ValueOutA || element == ValueInA)
                strValueName = GCode_GetValueName(null) + ".w";

            return strValueName;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == null || element == ValueOut || element == ValueIn)
                return mStrValueType;
            else if (mInComponentLinks.Contains(element) ||
                     mOutComponentLinks.Contains(element))
                return "float1";

            return "";
        }

        public override string GetValueDefine()
        {
            if (IsGeneric)
                return mStrValueType + " " + GCode_GetValueName(null) + ";\r\n";
            else
                return "";
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            string strTab = GCode_GetTabString(nLayer);
            if(!IsGeneric)
            {
                var strIdentity = mStrValueType + " " + GCode_GetValueName(null);
                var strDefinition = "    " + strIdentity + " = " + mStrValueType + "(" + GetValueString() + ");\r\n";

                if (!strDefinitionSegment.Contains(strDefinition))
                    strDefinitionSegment += strDefinition;
            }

            var lOI = GetLinkObjInfo(ValueIn);
            if(lOI.HasLink)
            {
                lOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, lOI.GetLinkElement(0, true));

                var inType = lOI.GetLinkObject(0, true).GCode_GetValueType(lOI.GetLinkElement(0, true));
                var rightStr = lOI.GetLinkObject(0, true).GCode_GetValueName(lOI.GetLinkElement(0, true));
                strSegment += strTab + GCode_GetValueName(null) + " = " + rightStr + ";\r\n";
            }

            // 处理xyzw等有链接的情况
            foreach(var link in mInComponentLinks)
            {
                var linkOI = GetLinkObjInfo(link);
                if(linkOI.HasLink)
                {
                    linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, linkOI.GetLinkElement(0, true));

                    var rightStr = linkOI.GetLinkObject(0, true).GCode_GetValueName(linkOI.GetLinkElement(0, true));
                    var inType = linkOI.GetLinkObject(0, true).GCode_GetValueType(linkOI.GetLinkElement(0, true));
                    if (inType == "float2" || inType == "float3" || inType == "float4")
                        rightStr += ".x";

                    strSegment += strTab + GCode_GetValueName(link) + " = " + rightStr + ";\r\n";
                }
            }
        }

        #endregion
    }
}
