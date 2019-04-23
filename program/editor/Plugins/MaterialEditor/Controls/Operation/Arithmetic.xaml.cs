using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;
using CSUtility.Support;

namespace MaterialEditor.Controls.Operation
{
    /// <summary>
    /// Arithmetic.xaml 的交互逻辑
    /// </summary>
    public partial class Arithmetic : BaseNodeControl
    {
        //// 临时类，用于选中后显示参数属性
        //CodeGenerateSystem.Base.GeneratorClassBase mTemplateClassInstance = null;
        //public CodeGenerateSystem.Base.GeneratorClassBase TemplateClassInstance
        //{
        //    get { return mTemplateClassInstance; }
        //}

        string m_strValueName1 = "";
        string m_strValueName2 = "";

        public Arithmetic(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            TitleLabel.Text = "运算(" + P1_Label.Text + " " + strParam + " " + P2_Label.Text + ")";

            enLinkType linkType = enLinkType.Float1 | enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4 | enLinkType.Int32 | enLinkType.Float4x4;
            AddLinkObject(linkType, Value1, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(linkType, Value2, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(linkType, ResultLink, enBezierType.Right, enLinkOpType.Start, ResultLink.BackBrush, true);


        }

        protected override void CollectionErrorMsg()
        {
            //var val1LinkType = GetLinkObjInfo(Value1).GetLinkType(0, true);
            //var val2LinkType = GetLinkObjInfo(Value2).GetLinkType(0, true);

            //switch (m_strParams)
            //{
            //    case "＋":
            //    case "－":
            //    case "·":   // 点乘
            //        if (val1LinkType != val2LinkType)
            //        {
            //            string strMsg = "左参右参类型不一致，左参：" + val1LinkType.ToString() + " 右参：" + val2LinkType.ToString();
            //            AddErrorMsg(new List<FrameworkElement> { Value1, Value2 }, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, strMsg);
            //        }
            //        break;
            //    case "×":   // 叉乘
            //        break;
            //    case "÷":
            //        break;
            //}
            //// 检测两个参数能否进行运算
            //// 叉乘只能是同维度之间
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            var valueLinkOI = GetLinkObjInfo(Value1);

            if (valueLinkOI.HasLink)
            {
                valueLinkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
                m_strValueName1 = valueLinkOI.GetLinkObject(0, true).GCode_GetValueName(valueLinkOI.GetLinkElement(0, true));
            }

            valueLinkOI = GetLinkObjInfo(Value2);

            if (valueLinkOI.HasLink)
            {
                valueLinkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
                m_strValueName2 = valueLinkOI.GetLinkObject(0, true).GCode_GetValueName(valueLinkOI.GetLinkElement(0, true));
            }
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            switch (StrParams)
            {
                case "＋":
                    return "(" + m_strValueName1 + " + " + m_strValueName2 + ")";
                case "－":
                    return "(" + m_strValueName1 + " - " + m_strValueName2 + ")";
                case "×": 
                    return "(" + m_strValueName1 + " * " + m_strValueName2 + ")";
                case "·":   // 点乘
                case "dot":
                    return "dot(" + m_strValueName1 + " ," + m_strValueName2 + ")";
                case "÷":
                    return "(" + m_strValueName1 + "/" + m_strValueName2 + ")";
                case "cross":  // 叉乘
                    {
                        var val1LinkOI = GetLinkObjInfo(Value1);
                        var val2LinkOI = GetLinkObjInfo(Value2);
                        if ((val1LinkOI.GetLinkType(0, true) == enLinkType.Float2 && val2LinkOI.GetLinkType(0, true) == enLinkType.Float2) ||
                            (val1LinkOI.GetLinkType(0, true) == enLinkType.Float3 && val2LinkOI.GetLinkType(0, true) == enLinkType.Float3) ||
                            (val1LinkOI.GetLinkType(0, true) == enLinkType.Float4 && val2LinkOI.GetLinkType(0, true) == enLinkType.Float4))
                            return "cross(" + m_strValueName1 + ", " + m_strValueName2 + ")";

                        return "(" + m_strValueName1 + " * " + m_strValueName2 + ")";
                    }
            }

            return base.GCode_GetValueName(element);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == null || element == ResultLink)
            {
                var lOI = GetLinkObjInfo(Value1);
                if (lOI.HasLink)
                    return lOI.GetLinkObject(0, true).GCode_GetValueType(lOI.GetLinkElement(0, true));
            }

            return base.GCode_GetValueType(element);
        }
    }
}
