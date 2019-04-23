using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CCore.Material;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls
{
    /// <summary>
    /// FloatControl.xaml 的交互逻辑
    /// </summary>
    public partial class CommonValueControl : BaseNodeControl_ShaderVar
    {
        /*
        D3DDECLTYPE_FLOAT1 = 0,
        D3DDECLTYPE_FLOAT2 = 1,
        D3DDECLTYPE_FLOAT3 = 2,
        D3DDECLTYPE_FLOAT4 = 3,
        D3DDECLTYPE_D3DCOLOR = 4,
        D3DDECLTYPE_UBYTE4 = 5,
        D3DDECLTYPE_SHORT2 = 6,
        D3DDECLTYPE_SHORT4 = 7,
        D3DDECLTYPE_UBYTE4N = 8,
        D3DDECLTYPE_SHORT2N = 9,
        D3DDECLTYPE_SHORT4N = 10,
        D3DDECLTYPE_USHORT2N = 11,
        D3DDECLTYPE_USHORT4N = 12,
        D3DDECLTYPE_UDEC3 = 13,
        D3DDECLTYPE_DEC3N = 14,
        D3DDECLTYPE_FLOAT16_2 = 15,
        D3DDECLTYPE_FLOAT16_4 = 16,
        D3DDECLTYPE_UNUSED = 17,

        */
        
        string mTitle = "ValueX";
        public string Title
        {
            get { return mTitle; }
            set
            {
                mTitle = value;

                OnPropertyChanged("Title");
            }
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

        string mStrValueType;
        enLinkType mLinkType;

        List<TextBox> mValueTextboxs = new List<TextBox>();

        class ValueLinkData
        {
            public string m_name;
            public string m_type;
            public FrameworkElement m_inLink;
            public FrameworkElement m_outLink;
        }
        Dictionary<FrameworkElement, ValueLinkData> m_linkDataDictionary = new Dictionary<FrameworkElement, ValueLinkData>();
        List<FrameworkElement> m_inLinks = new List<FrameworkElement>();
        List<FrameworkElement> m_outLinks = new List<FrameworkElement>();

        public CommonValueControl(Canvas parentDrawCanvas, string strParam)
            : base(parentDrawCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(TitleLabel);
            m_linkDataDictionary.Clear();

            ShaderVarInfo.EditorType = "Vector";
            //ShaderVarInfo.VarName = GCode_GetValueName(null);

            mLinkType = LinkObjInfo.GetLinkTypeFromTypeString(strParam);
            switch (mLinkType)
            {
                case enLinkType.Int32:
                    ValueIn.BackBrush = Program.GetBrushFromValueType(strParam, this);
                    AddLinkObject(enLinkType.Int32 | enLinkType.Float1 | enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4, ValueIn, enBezierType.Left, enLinkOpType.End, null, false);
                    break;
                case enLinkType.Float1:
                    ValueIn.BackBrush = Program.GetBrushFromValueType(strParam, this);
                    AddLinkObject(enLinkType.Int32 | enLinkType.Float1 | enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4, ValueIn, enBezierType.Left, enLinkOpType.End, null, false);
                    break;

                case enLinkType.Float2:
                    ValueIn.BackBrush = Program.GetBrushFromValueType(strParam, this);
                    AddLinkObject(enLinkType.Float2 | enLinkType.Float3 | enLinkType.Float4, ValueIn, enBezierType.Left, enLinkOpType.End, null, false);
                    break;

                case enLinkType.Float3:
                    ValueIn.BackBrush = Program.GetBrushFromValueType(strParam, this);
                    AddLinkObject(enLinkType.Float3 | enLinkType.Float4, ValueIn, enBezierType.Left, enLinkOpType.End, null, false);
                    break;

                case enLinkType.Float4:
                    ValueIn.BackBrush = Program.GetBrushFromValueType(strParam, this);
                    AddLinkObject(enLinkType.Float4, ValueIn, enBezierType.Left, enLinkOpType.End, null, false);
                    break;
            }

            ValueOut.BackBrush = Program.GetBrushFromValueType(strParam, this);
            AddLinkObject(mLinkType, ValueOut, enBezierType.Right, enLinkOpType.Start, ValueOut.BackBrush, true);

            if (!string.IsNullOrEmpty(strParam))
            {
                mStrValueType = strParam;
                Title = strParam;

                switch (mStrValueType)
                {
                    case "int":
                        AddCommonValue("值", "值", "int");
                        ValueIn.Visibility = Visibility.Collapsed;
                        ValueOut.Visibility = Visibility.Collapsed;
                        break;

                    case "float":
                    case "float1":
                        AddCommonValue("x(r)", "x", "float1");
                        ValueIn.Visibility = Visibility.Collapsed;
                        ValueOut.Visibility = Visibility.Collapsed;
                        break;

                    case "float2":
                        AddCommonValue("x(r)", "x", "float1");
                        AddCommonValue("y(g)", "y", "float1");
                        break;

                    case "float3":
                        AddCommonValue("x(r)", "x", "float1");
                        AddCommonValue("y(g)", "y", "float1");
                        AddCommonValue("z(b)", "z", "float1");
                        break;

                    case "float4":
                        AddCommonValue("x(r)", "x", "float1");
                        AddCommonValue("y(g)", "y", "float1");
                        AddCommonValue("z(b)", "z", "float1");
                        AddCommonValue("w(a)", "w", "float1");
                        break;
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

        protected void AddCommonValue(string strShowName, string strName, string strType)
        {
            /*
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition />
                </Grid.ColumnDefinitions>
                <Label>X</Label>
                <TextBox Grid.Column="1" MinWidth="50" />
                <Ellipse Width="10" Height="10" Fill="LightGreen" Stroke="Black" HorizontalAlignment="Left" Margin="-15,0,0,0" />
                <Rectangle Grid.Column="1" Width="10" Height="10" Fill="LightGreen" Stroke="Black" HorizontalAlignment="Right" Margin="0,0,-15,0" />
            </Grid>
             */

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width=new GridLength(30) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            FloatValuesStack.Children.Add(grid);

            TextBlock nameLabel = new TextBlock()
            {
                Text = strShowName,
                Margin = new Thickness(3),
                Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TextBlockStyle_Default")) as System.Windows.Style
            };
            grid.Children.Add(nameLabel);

            TextBox textBox = new TextBox()
            {
                MinWidth = 50,
                Text = "0",
                Margin = new Thickness(3),
                Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TextBoxStyle_Default")) as System.Windows.Style
            };
            textBox.TextChanged += new TextChangedEventHandler(ValueTextBox_TextChanged);
            Grid.SetColumn(textBox, 1);
            grid.Children.Add(textBox);
            mValueTextboxs.Add(textBox);

            var ellipse = new CodeGenerateSystem.Controls.LinkInControl()
            {
                Width = 15,
                Height = 15,
                BackBrush = Program.GetBrushFromValueType(strType, this),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(-20,0,0,0),
                Direction = enBezierType.Left,
            };
            grid.Children.Add(ellipse);
            AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), ellipse, enBezierType.Left, enLinkOpType.End, null, false);
            m_inLinks.Add(ellipse);

            var rect = new CodeGenerateSystem.Controls.LinkOutControl()
            {
                Width = 15,
                Height = 15,
                BackBrush = Program.GetBrushFromValueType(strType, this),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, -20, 0),
                Direction = enBezierType.Right,
            };
            Grid.SetColumn(rect, 1);
            grid.Children.Add(rect);
            AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), rect, enBezierType.Right, enLinkOpType.Start, rect.BackBrush, true);
            m_outLinks.Add(rect);

            ValueLinkData linkData = new ValueLinkData();
            linkData.m_name = strName;
            linkData.m_type = strType;
            linkData.m_inLink = ellipse;
            linkData.m_outLink = rect;
            m_linkDataDictionary[ellipse] = linkData;
            m_linkDataDictionary[rect] = linkData;
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("ValueName", ValueName);
            xmlNode.AddAttrib("IsGeneric", IsGeneric.ToString());

            for(int i=0; i<mValueTextboxs.Count; ++i)
            {
                xmlNode.AddAttrib("Value" + i, mValueTextboxs[i].Text);
            }

            base.Save(xmlNode, newGuid,holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            var nameValue = xmlNode.FindAttrib("ValueName");
            if (nameValue != null)
            {
                NameTextBox.Text = nameValue.Value;
                mValueName = nameValue.Value;
            }
            var isAtt = xmlNode.FindAttrib("IsGeneric");
            if (isAtt != null)
                IsGeneric = System.Convert.ToBoolean(isAtt.Value);

            for (int i = 0; i < mValueTextboxs.Count; ++i)
            {
                var value = xmlNode.FindAttrib("Value" + i);
                if(value != null)
                    mValueTextboxs[i].Text = value.Value;
            }

            base.Load(xmlNode, deltaX, deltaY);

            InitializeShaderVarInfo();
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            string strValueName = "";

            if (element == null || element == ValueOut)
            {
                if (String.IsNullOrEmpty(ValueName))
                {
                    strValueName = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + CodeGenerateSystem.Program.GetValuedGUIDString(Id);//m_Guid.ToString();
                    //strValueName = strValueName.Replace("-", "_");
                }
                else
                    strValueName = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + ValueName;
            }
            else
            {
                string strBackword = "";
                switch(mStrValueType)
                {
                    case "int":
                    case "float":
                    case "float1":
                        strBackword = "";
                        break;

                    default:
                        strBackword = "." + m_linkDataDictionary[element].m_name;
                        break;
                }
                strValueName = GCode_GetValueName(null) + strBackword;
            }

            return strValueName;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == null || element == ValueOut)
                return StrParams;
            else
                return m_linkDataDictionary[element].m_type;
        }

        public string GetValueString()
        {
            string retStr = "";
            foreach (var textBox in mValueTextboxs)
            {
                float value = 0;
                try
                {
                    value = System.Convert.ToSingle(textBox.Text);
                    if (float.IsNaN(value))
                        value = 0.0f;
                }
                catch (System.Exception)
                {
                    value = 0;
                }

                retStr += value + ",";
            }

            retStr = retStr.Remove(retStr.Length - 1);
            return retStr;
        }

        public override string GetValueDefine()
        {
            if (IsGeneric)
                return StrParams + " " + GCode_GetValueName(null) + ";\r\n";
            else
                return "";
        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            string strTab = GCode_GetTabString(nLayer);

            // 变量声明
            if (!IsGeneric)
            {
                var strIdentity = mStrValueType + " " + GCode_GetValueName(null);
                var strDefinition = "";
                switch (mStrValueType)
                {
                    case "int":
                        {
                            int value = 0;
                            try
                            {
                                value = System.Convert.ToInt32(mValueTextboxs[0].Text);                                
                            }
                            catch (System.Exception)
                            {
                                value = 0;
                            }
                            strDefinition = "    " + strIdentity + " = " + value + ";\r\n";
                        }
                        break;

                    case "float":
                    case "float1":
                        {
                            float value = 0.0f;
                            try
                            {
                                value = System.Convert.ToSingle(mValueTextboxs[0].Text);
                                if (float.IsNaN(value))
                                    value = 0.0f;
                            }
                            catch (System.Exception)
                            {
                                value = 0.0f;
                            }
                            strDefinition = "    " + strIdentity + " = " + value + ";\r\n";
                        }
                        break;

                    case "float2":
                    case "float3":
                    case "float4":
                        {
                            strDefinition = "    " + strIdentity + " = " + mStrValueType + "(" + GetValueString() + ");\r\n";
                        }
                        break;
                }

                if (!strDefinitionSegment.Contains(strDefinition))
                {
                    strDefinitionSegment += strDefinition;
                }
            }


            var lOI = GetLinkObjInfo(ValueIn);
            if (lOI.HasLink)
            {
                lOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, lOI.GetLinkElement(0, true));

                var inType = lOI.GetLinkObject(0, true).GCode_GetValueType(lOI.GetLinkElement(0, true));
                var rightStr = lOI.GetLinkObject(0, true).GCode_GetValueName(lOI.GetLinkElement(0, true));
                switch (mLinkType)
                {
                    case enLinkType.Int32:
                    case enLinkType.Float1:
                        {
                            switch (inType)
                            {
                                case "float2":
                                case "float3":
                                case "float4":
                                    rightStr += ".x";
                                    break;
                            }
                        }
                        break;

                    case enLinkType.Float2:
                        {
                            switch (inType)
                            {
                                case "float3":
                                case "float4":
                                    rightStr += ".xy";
                                    break;
                            }
                        }
                        break;

                    case enLinkType.Float3:
                        {
                            switch (inType)
                            {
                                case "float4":
                                    rightStr += ".xyz";
                                    break;
                            }
                        }
                        break;
                }

                var assignStr = strTab + GCode_GetValueName(null) + " = " + rightStr + ";\r\n";
                // 这里先不做判断，连线中有if的情况下会导致问题
                // 判断赋值语句是否重复
                //if (!strSegment.Contains(assignStr))
                if (!Program.IsSegmentContainString(strSegment.Length - 1, strSegment, assignStr))
                    strSegment += assignStr;
            }

            //处理xyzw等有连接的情况
            foreach (var link in m_inLinks)
            {
                var linkOI = GetLinkObjInfo(link);
                if (linkOI.HasLink)
                {
                    linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, linkOI.GetLinkElement(0, true));

                    var rightStr = linkOI.GetLinkObject(0, true).GCode_GetValueName(linkOI.GetLinkElement(0, true));
                    var inType = linkOI.GetLinkObject(0, true).GCode_GetValueType(linkOI.GetLinkElement(0, true));
                    if (inType == "float2" || inType == "float3" || inType == "float4")
                        rightStr += ".x";

                    var assignStr = strTab + GCode_GetValueName(link) + " = " + rightStr + ";\r\n";
                    // 这里先不做判断，连线中有if的情况下会导致问题
                    //if (!strSegment.Contains(assignStr))
                    if(!Program.IsSegmentContainString(strSegment.Length - 1, strSegment, assignStr))
                        strSegment += assignStr;
                }
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ShaderVarInfo != null)
                ShaderVarInfo.VarValue = GetValueString();

            IsDirty = true;
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
    }
}
