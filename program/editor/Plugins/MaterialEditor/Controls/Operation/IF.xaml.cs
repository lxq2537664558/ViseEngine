using System.Windows;
using System.Windows.Controls;
using CodeGenerateSystem.Base;
using CSUtility.Support;

namespace MaterialEditor.Controls.Operation
{
    /// <summary>
    /// Interaction logic for IF.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("逻辑.if", "逻辑判断")]
    public partial class IF : BaseNodeControl
    {
        public IF(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            enLinkType codValueLinkType = enLinkType.Float1 | enLinkType.Int32 | enLinkType.Int64 | enLinkType.Double | enLinkType.Single;
            AddLinkObject(codValueLinkType, ValueA, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(codValueLinkType, ValueB, enBezierType.Left, enLinkOpType.End, null, false);
            enLinkType resultValueType = enLinkType.Int32 | enLinkType.Float1 | enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4;
            AddLinkObject(resultValueType, ValueAgtB, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(resultValueType, ValueAeqB, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(resultValueType, ValueAltB, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(resultValueType, ResultHandle, enBezierType.Right, enLinkOpType.Start, ResultHandle.BackBrush, true);
            //AddLinkObject(enLinkType.Bool, ValueBool, enBezierType.Left, enLinkOpType.End, null, false);
        }

        protected override void CollectionErrorMsg()
        {
            //var lOI = GetLinkObjInfo(ValueA);
            //if (!lOI.bHasLink)
            //    AddErrorMsg(ValueA, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "判断条件参数A未设置值");

            //lOI = GetLinkObjInfo(ValueB);
            //if (!lOI.bHasLink)
            //    AddErrorMsg(ValueB, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "判断条件参数B未设置值");

            //var lOIAgtB = GetLinkObjInfo(ValueAgtB);
            //var lOIAeqB = GetLinkObjInfo(ValueAeqB);
            //var lOIAltB = GetLinkObjInfo(ValueAltB);
            //if (!lOIAgtB.bHasLink && !lOIAeqB.bHasLink && !lOIAltB.bHasLink)
            //    AddErrorMsg(new List<FrameworkElement> { ValueAgtB, ValueAeqB, ValueAltB }, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "结果未设置值，必须至少设置一个");

            ////string vtAgtB = "", vtAeqB = "", vtAltB = "";
            ////if (lOIAgtB.bHasLink)
            ////    vtAgtB = lOIAgtB.GetLinkObject(0, true).GCode_GetValueType(lOIAgtB.GetLinkElement(0, true));
            ////if (lOIAeqB.bHasLink)
            ////    vtAeqB = lOIAeqB.GetLinkObject(0, true).GCode_GetValueType(lOIAeqB.GetLinkElement(0, true));
            ////if (lOIAltB.bHasLink)
            ////    vtAltB = lOIAltB.GetLinkObject(0, true).GCode_GetValueType(lOIAltB.GetLinkElement(0, true));
            //var lOI1LinkType = lOIAgtB.GetLinkType(0, true);
            //var lOI2LinkType = lOIAeqB.GetLinkType(0, true);
            //var lOI3LinkType = lOIAltB.GetLinkType(0, true);
            //if (lOI1LinkType != lOI2LinkType)
            //    AddErrorMsg(new List<FrameworkElement> { ValueAgtB, ValueAeqB }, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "参数类型不一致， A>B:" + lOI1LinkType.ToString() + " A=B:" + lOI2LinkType.ToString());
            //if (lOI1LinkType != lOI3LinkType)
            //    AddErrorMsg(new List<FrameworkElement> { ValueAgtB, ValueAltB }, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "参数类型不一致, A>B:" + lOI1LinkType.ToString() + " A<B:" + lOI3LinkType.ToString());
            //if (lOI2LinkType != lOI3LinkType)
            //    AddErrorMsg(new List<FrameworkElement> { ValueAeqB, ValueAltB }, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "参数类型不一致, A=B:" + lOI2LinkType.ToString() + " A<B:" + lOI3LinkType.ToString());

            //var lOIR = GetLinkObjInfo(ResultHandle);
            //if (!lOIR.bHasLink)
            //    AddErrorMsg(ResultHandle, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "未将结果设置给任意变量，当前节点没有作用");
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            string strTab = GCode_GetTabString(nLayer);

            var lOI = GetLinkObjInfo(ValueA);
            if (lOI == null || lOI.GetLinkObject(0, true) == null)
                return;

            string valueNameA;
            lOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, lOI.GetLinkElement(0, true));
            valueNameA = lOI.GetLinkObject(0, true).GCode_GetValueName(lOI.GetLinkElement(0, true));

            lOI = GetLinkObjInfo(ValueB);
            string valueNameB;
            lOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, lOI.GetLinkElement(0, true));
            valueNameB = lOI.GetLinkObject(0, true).GCode_GetValueName(lOI.GetLinkElement(0, true));

            var lOIAgtB = GetLinkObjInfo(ValueAgtB);
            var lOIAeqB = GetLinkObjInfo(ValueAeqB);
            var lOIAltB = GetLinkObjInfo(ValueAltB);
            lOI = lOIAgtB.HasLink ? lOIAgtB : (lOIAeqB.HasLink ? lOIAeqB : (lOIAltB.HasLink ? lOIAltB : null));

            if (lOI == null)
                return;

            // 判断变量是否已经声明过了，没有声明过则进行声明
            string strInitString = "";
            string strValueType = lOI.GetLinkObject(0, true).GCode_GetValueType(lOI.GetLinkElement(0, true));
            strInitString = Program.GetInitialNewString(strValueType);
            var strValueIdt = strValueType + " " + GCode_GetValueName(null) + " = " + strInitString + ";\r\n";
            if (!strDefinitionSegment.Contains(strValueIdt))
                strDefinitionSegment += "    " + strValueIdt;
            // A>B
            if (lOIAgtB.HasLink)
            {
                strSegment += strTab + "if( " + valueNameA + " > " + valueNameB + ")\r\n";
                strSegment += strTab + "{\r\n";
                string strIFSegment = "";
                lOIAgtB.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strIFSegment, nLayer + 1, lOIAgtB.GetLinkElement(0, true));
                strSegment += strIFSegment;
                strSegment += GCode_GetTabString(nLayer + 1) + GCode_GetValueName(null) + " = " + lOIAgtB.GetLinkObject(0, true).GCode_GetValueName(lOIAgtB.GetLinkElement(0, true)) + ";\r\n";
                strSegment += strTab + "}\r\n";
            }
            // A==B
            if (lOIAeqB.HasLink)
            {
                if (lOIAltB.HasLink)
                {
                    string strIfType;
                    if (lOIAgtB.HasLink)
                        strIfType = "else if";
                    else
                        strIfType = "if";
                    strSegment += strTab + strIfType + "( " + valueNameA + " == " + valueNameB + ")\r\n";
                }
                else
                    strSegment += strTab + "else\r\n";
                strSegment += strTab + "{\r\n";
                string strIFSegment = "";
                lOIAeqB.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strIFSegment, nLayer + 1, lOIAeqB.GetLinkElement(0, true));
                strSegment += strIFSegment;
                strSegment += GCode_GetTabString(nLayer + 1) + GCode_GetValueName(null) + " = " + lOIAeqB.GetLinkObject(0, true).GCode_GetValueName(lOIAeqB.GetLinkElement(0, true)) + ";\r\n";
                strSegment += strTab + "}\r\n";
            }
            // A<B
            if (lOIAltB.HasLink)
            {
                if (lOIAgtB.HasLink || lOIAeqB.HasLink)
                    strSegment += strTab + "else\r\n";
                else
                    strSegment += strTab + "if( " + valueNameA + " < " + valueNameB + ")\r\n";
                strSegment += strTab + "{\r\n";
                string strIFSegment = "";
                lOIAltB.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strIFSegment, nLayer + 1, lOIAltB.GetLinkElement(0, true));
                strSegment += strIFSegment;
                strSegment += GCode_GetTabString(nLayer + 1) + GCode_GetValueName(null) + " = " + lOIAltB.GetLinkObject(0, true).GCode_GetValueName(lOIAltB.GetLinkElement(0, true)) + ";\r\n";
                strSegment += strTab + "}\r\n";
            }
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            string strValueName = "";
            if (element == null || element == ResultHandle)
            {
                strValueName = "Value_" + CodeGenerateSystem.Program.GetValuedGUIDString(Id);
            }

            return strValueName;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            var lOIAgtB = GetLinkObjInfo(ValueAgtB);
            var lOIAeqB = GetLinkObjInfo(ValueAeqB);
            var lOIAltB = GetLinkObjInfo(ValueAltB);
            var lOI = lOIAgtB.HasLink ? lOIAgtB : (lOIAeqB.HasLink ? lOIAeqB : (lOIAltB.HasLink ? lOIAltB : null));

            if(lOI != null)
            {
                lOI.GetLinkObject(0, true).GCode_GetValueType(lOI.GetLinkElement(0, true));
            }

            return base.GCode_GetValueType(element);
        }
    }
}
