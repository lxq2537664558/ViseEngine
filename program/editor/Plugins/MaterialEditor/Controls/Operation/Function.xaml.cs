using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CodeGenerateSystem.Base;
using CSUtility.Support;

namespace MaterialEditor.Controls.Operation
{
    /// <summary>
    /// Interaction logic for Function.xaml
    /// </summary>
    public partial class Function : BaseNodeControl
    {
        string mInclude = "";
        public string Include
        {
            get
            {
                return mInclude;
            }
        }

        string mStrFuncName = "";

        private FrameworkElement mReturnValueHandle;
        private struct stParamData
        {
            public int mPos;                       // 第几个参数
            public string mStrName;                // 参数名称
            public string mStrType;                // 参数类型
            public string mStrAttribute;           // 参数inout、out、return等说明
            public FrameworkElement mInElement;
            public FrameworkElement mOutElement;
        }
        private Dictionary<FrameworkElement, stParamData> mOutValueDataDictionary = new Dictionary<FrameworkElement, stParamData>();
        private List<stParamData> mInParamDataList = new List<stParamData>();
        private List<stParamData> mOnlyOutParamDataList = new List<stParamData>();

        // 临时类，用于选中后显示参数属性
        CodeGenerateSystem.Base.GeneratorClassBase mTemplateClassInstance = null;
        public CodeGenerateSystem.Base.GeneratorClassBase TemplateClassInstance
        {
            get { return mTemplateClassInstance; }
        }

        public Function(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);

            var splits = strParam.Split('|');

            mInclude = splits[0];
            if (mInclude.Contains(":"))
            {
                mInclude = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mInclude, CSUtility.Support.IFileManager.Instance.Root + "bin/");
            }
            mInclude = mInclude.Replace("\\", "/");
            if (mInclude.Contains("bin/"))
            {
                var nIdx = mInclude.IndexOf("bin/");
                mInclude = mInclude.Substring(nIdx + 4);
            }

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(splits[1]);

            mStrFuncName = xmlDoc.DocumentElement.GetAttribute("Name");
            TitleLabel.Text = mStrFuncName;
            var tempElements = xmlDoc.GetElementsByTagName("Param");
            if (tempElements.Count > 0)
            {
                var cpInfos = new List<CodeGenerateSystem.Base.CustomPropertyInfo>();

                var paramInElm = tempElements[0];
                int nIdx = 0;
                foreach (System.Xml.XmlElement node in paramInElm.ChildNodes)
                {
                    var typeStr = node.GetAttribute("Type");
                    var nameStr = node.GetAttribute("Name");
                    var strAttr = node.GetAttribute("Attribute");

                    switch(strAttr)
                    {
                        case "out":
                        case "return":
                            break;
                        default:
                            {
                                var cpInfo = CodeGenerateSystem.Base.CustomPropertyInfo.GetFromParamInfo(Program.GetTypeFromValueType(typeStr), nameStr);
                                if(cpInfo != null)
                                    cpInfos.Add(cpInfo);
                            }
                            break;
                    }

                    AddLink(nIdx, typeStr, nameStr, strAttr);
                    nIdx++;
                }

                var classType = CodeGenerateSystem.Base.PropertyClassGenerator.CreateTypeFromCustomPropertys(cpInfos);
                mTemplateClassInstance = System.Activator.CreateInstance(classType) as CodeGenerateSystem.Base.GeneratorClassBase;

                foreach(var property in mTemplateClassInstance.GetType().GetProperties())
                {
                    property.SetValue(mTemplateClassInstance, CodeGenerateSystem.Program.GetDefaultValueFromType(property.PropertyType));
                }
            }

            this.UpdateLayout();
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
            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(XmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);

            if(mTemplateClassInstance != null)
            {
                var node = xmlNode.FindNode("DefaultParamValue");
                if (node != null)
                    mTemplateClassInstance.Load(node);
            }
        }

        private void AddLink(int nIdx, string strType, string strName, string strAttribute)
        {
            var splits = strAttribute.Split('|');
            if (string.IsNullOrEmpty(strAttribute))
            {
                Grid grid = new Grid();
                var ellipse = new CodeGenerateSystem.Controls.LinkInControl()
                {
                    Width = 15,
                    Height = 15,
                    Margin = new System.Windows.Thickness(-20,0,0,0),
                    BackBrush = Program.GetBrushFromValueType(strType, this),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Direction = enBezierType.Left,
                };
                grid.Children.Add(ellipse);
                var linkType = LinkObjInfo.GetLinkTypeFromTypeString(strType);
                switch(linkType)
                {
                    case enLinkType.Int32:
                    case enLinkType.Int64:
                    case enLinkType.Single:
                    case enLinkType.Double:
                    case enLinkType.Byte:
                    case enLinkType.SByte:
                    case enLinkType.Int16:
                    case enLinkType.UInt16:
                    case enLinkType.UInt32:
                    case enLinkType.UInt64:
                    case enLinkType.Float1:
                        linkType = enLinkType.Float1 | enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4;
                        break;
                    case enLinkType.Float2:
                        linkType = enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4;
                        break;
                    case enLinkType.Float3:
                        linkType = enLinkType.Float3 | enLinkType.Float4;
                        break;
                }
                AddLinkObject(linkType, ellipse, enBezierType.Left, enLinkOpType.End, null, false);
                TextBlock label = new TextBlock()
                {
                    Text = strName + "(" + strType + ")",
                    Margin = new System.Windows.Thickness(3)
                };
                grid.Children.Add(label);
                StackPanel_InValue.Children.Add(grid);

                stParamData parData = new stParamData()
                {
                    mPos = nIdx,
                    mStrName = strName,
                    mStrType = strType,
                    mStrAttribute = "",
                    mInElement = ellipse
                };
                mInParamDataList.Add(parData);
            }
            else if (strAttribute == "out")
            {
                Grid grid = new Grid();
                var rect = new CodeGenerateSystem.Controls.LinkOutControl()
                {
                    Width = 15,
                    Height = 15,
                    Margin = new System.Windows.Thickness(0, 0, -20, 0),
                    BackBrush = Program.GetBrushFromValueType(strType, this),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Direction = enBezierType.Right,
                };
                grid.Children.Add(rect);
                AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), rect, enBezierType.Right, enLinkOpType.Start, rect.BackBrush, true);
                TextBlock label = new TextBlock()
                {
                    Text = strName + "(" + strType + ")",
                    Margin = new System.Windows.Thickness(3)
                };
                grid.Children.Add(label);
                StackPanel_OutValue.Children.Add(grid);

                stParamData ovData = new stParamData()
                {
                    mPos = nIdx,
                    mStrName = strName,
                    mStrType = strType,
                    mStrAttribute = strAttribute,
                    mOutElement = rect
                };

                mInParamDataList.Add(ovData);
                mOnlyOutParamDataList.Add(ovData);
                mOutValueDataDictionary[rect] = ovData;
            }
            else if(strAttribute == "inout")
            {
                Grid grid = new Grid();
                var ellipse = new CodeGenerateSystem.Controls.LinkInControl()
                {
                    Width = 15,
                    Height = 15,
                    Margin = new System.Windows.Thickness(-20, 0, 0, 0),
                    BackBrush = Program.GetBrushFromValueType(strType, this),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Direction = enBezierType.Left,
                };
                grid.Children.Add(ellipse);
                var linkType = LinkObjInfo.GetLinkTypeFromTypeString(strType);
                switch (linkType)
                {
                    case enLinkType.Int32:
                    case enLinkType.Int64:
                    case enLinkType.Single:
                    case enLinkType.Double:
                    case enLinkType.Byte:
                    case enLinkType.SByte:
                    case enLinkType.Int16:
                    case enLinkType.UInt16:
                    case enLinkType.UInt32:
                    case enLinkType.UInt64:
                    case enLinkType.Float1:
                        linkType = enLinkType.Float1 | enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4;
                        break;
                    case enLinkType.Float2:
                        linkType = enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4;
                        break;
                    case enLinkType.Float3:
                        linkType = enLinkType.Float3 | enLinkType.Float4;
                        break;
                }
                AddLinkObject(linkType, ellipse, enBezierType.Left, enLinkOpType.End, null, false);
                TextBlock label = new TextBlock()
                {
                    Text = strName + "(" + strType + ")",
                    Margin = new System.Windows.Thickness(3)
                };
                grid.Children.Add(label);
                StackPanel_InValue.Children.Add(grid);

                stParamData parData = new stParamData()
                {
                    mPos = nIdx,
                    mStrName = strName, 
                    mStrType = strType, 
                    mStrAttribute = strAttribute, 
                    mInElement = ellipse
                };
                mInParamDataList.Add(parData);

                Grid outGrid = new Grid();
                var rect = new CodeGenerateSystem.Controls.LinkOutControl()
                {
                    Width = 15,
                    Height = 15,
                    Margin = new System.Windows.Thickness(0, 0, -20, 0),
                    BackBrush = Program.GetBrushFromValueType(strType, this),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Direction = enBezierType.Right,
                };
                outGrid.Children.Add(rect);
                AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), rect, enBezierType.Right, enLinkOpType.Start, rect.BackBrush, true);
                label = new TextBlock()
                {
                    Text = strName + "(" + strType + ")",
                    Margin = new System.Windows.Thickness(3)
                };
                outGrid.Children.Add(label);
                StackPanel_OutValue.Children.Add(outGrid);

                stParamData ovData = new stParamData()
                {
                    mPos = nIdx,
                    mStrName = strName,
                    mStrType = strType,
                    mStrAttribute = strAttribute,
                    mInElement = ellipse,
                    mOutElement = rect
                };

                mOutValueDataDictionary[rect] = ovData;
            }
            else if (strAttribute == "return")
            {
                Grid grid = new Grid();
                var rect = new CodeGenerateSystem.Controls.LinkOutControl()
                {
                    Width = 15,
                    Height = 15,
                    Margin = new System.Windows.Thickness(0, 0, -20, 0),
                    BackBrush = Program.GetBrushFromValueType(strType, this),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Direction = enBezierType.Right,
                };
                grid.Children.Add(rect);
                AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), rect, enBezierType.Right, enLinkOpType.Start, rect.BackBrush, true);
                mReturnValueHandle = rect;
                TextBlock label = new TextBlock()
                {
                    Text = "Return(" + strType + ")",
                    Margin = new System.Windows.Thickness(3)
                };
                grid.Children.Add(label);
                StackPanel_OutValue.Children.Add(grid);

                stParamData ovData = new stParamData()
                {
                    mPos = nIdx,
                    mStrName = strName,
                    mStrType = strType,
                    mStrAttribute = strAttribute,
                    mOutElement = rect
                };

                mOutValueDataDictionary[rect] = ovData;
            }
        }

        protected override void CollectionErrorMsg()
        {
            foreach (var pardata in mInParamDataList)
            {
                if (pardata.mInElement == null)
                    continue;
                var lOI = GetLinkObjInfo(pardata.mInElement);
                if (!lOI.HasLink)
                {
                    if(mTemplateClassInstance != null)
                    {
                        if(mTemplateClassInstance.GetType().GetProperty(pardata.mStrName) == null)
                            AddErrorMsg(pardata.mInElement, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "未连接参数!");
                    }
                    else
                        AddErrorMsg(pardata.mInElement, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "未连接参数!");
                }
            }
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            string strTab = GCode_GetTabString(nLayer);
            string strIdt = "";

            strIdt = mStrFuncName + "(";
            foreach (var pardata in mInParamDataList)
            {
                if (pardata.mStrAttribute == "out")
                {
                    strIdt += GCode_GetValueName(pardata.mOutElement) + ",";
                }
                else
                {
                    var lOI = GetLinkObjInfo(pardata.mInElement);
                    if (lOI.HasLink)
                    {
                        lOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, lOI.GetLinkElement(0, true));
                        var linkVarName = lOI.GetLinkObject(0, true).GCode_GetValueName(lOI.GetLinkElement(0, true));
                        var linkType = lOI.GetLinkType(0, true);
                        switch (pardata.mStrType)
                        {
                            case "float":
                            case "float1":
                                if (linkType != enLinkType.Float1)
                                    linkVarName += ".x";
                                break;
                            case "float2":
                                if (linkType != enLinkType.Float2)
                                    linkVarName += ".xy";
                                break;
                            case "float3":
                                if (linkType != enLinkType.Float3)
                                    linkVarName += ".xyz";
                                break;
                        }
                        strIdt += linkVarName + ",";
                    }
                    else
                    {
                        // 写入默认值
                        if(mTemplateClassInstance != null)
                        {
                            var property = mTemplateClassInstance.GetType().GetProperty(pardata.mStrName);
                            if(property != null)
                            {
                                var propertyValue = property.GetValue(mTemplateClassInstance);
                                var value = Program.GetTypeValue(pardata.mStrType, propertyValue);
                                strIdt += value + ",";
                            }
                        }
                    }
                }
            }
            // 去掉最后一个","
            strIdt = strIdt.Remove(strIdt.Length - 1);
            strIdt += ");\r\n";

            if (mReturnValueHandle != null)
            {
                var pardata = mOutValueDataDictionary[mReturnValueHandle];
                strIdt = pardata.mStrType + " " + GCode_GetValueName(null) + " = " + strIdt;
            }

            strIdt = strTab + strIdt;
            // 判断该段代码是否调用过，同样的参数调用过则不再调用该函数
            //if (!strSegment.Contains(strIdt))
            if(!Program.IsSegmentContainString(strSegment.Length - 1, strSegment, strIdt))
            {                
                foreach (var pardata in mOnlyOutParamDataList)
                {
                    strSegment += strTab + pardata.mStrType + " " + GCode_GetValueName(pardata.mOutElement) + " = " + Program.GetInitialNewString(pardata.mStrType) + ";\r\n";
                }
                strSegment += strIdt;
            }

            base.GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, element);
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (element == null || element == mReturnValueHandle)
            {
                return "FuncRetValue_" + Program.GetValuedGUIDString(Id);
            }
            else
            {
                stParamData valData;
                if (mOutValueDataDictionary.TryGetValue(element, out valData))
                {
                    if (valData.mStrAttribute == "out")
                    {
                        return "FuncTempValue_" + Program.GetValuedGUIDString(Id) + "_" + valData.mPos;
                    }
                    else
                    {
                        var lOI = GetLinkObjInfo(valData.mInElement);
                        if (lOI.HasLink)
                        {
                            return lOI.GetLinkObject(0, true).GCode_GetValueName(lOI.GetLinkElement(0, true));
                        }
                    }
                }
            }

            return base.GCode_GetValueName(element);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            stParamData valData;
            if (mOutValueDataDictionary.TryGetValue(element, out valData))
                return valData.mStrType;

            return base.GCode_GetValueType(element);
        }
    }
}
