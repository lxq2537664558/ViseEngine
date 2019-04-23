using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls
{
    /// <summary>
    /// FloatControl.xaml 的交互逻辑
    /// </summary>
    public partial class CommonValueControl : BaseNodeControl
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
        public bool IsGeneric
        {
            get;
            set;
        }

        string m_strValueType;

        List<TextBox> m_valueTextboxs = new List<TextBox>();

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

            AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strParam), ValueIn, enBezierType.Left, enLinkOpType.End, null, false);
            AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strParam), ValueOut, enBezierType.Right, enLinkOpType.Start, ValueOut.Fill, true);

            if (!string.IsNullOrEmpty(strParam))
            {
                m_strValueType = strParam;
                TitleLabel.Content = strParam;

                switch (m_strValueType)
                {
                    case "int":
                        AddCommonValue("值", "int");
                        ValueIn.Visibility = Visibility.Collapsed;
                        ValueOut.Visibility = Visibility.Collapsed;
                        break;

                    case "float1":
                        AddCommonValue("X", "float1");
                        ValueIn.Visibility = Visibility.Collapsed;
                        ValueOut.Visibility = Visibility.Collapsed;
                        break;

                    case "float2":
                        AddCommonValue("X", "float1");
                        AddCommonValue("Y", "float1");
                        break;

                    case "float3":
                        AddCommonValue("X", "float1");
                        AddCommonValue("Y", "float1");
                        AddCommonValue("Z", "float1");
                        break;

                    case "float4":
                        AddCommonValue("X", "float1");
                        AddCommonValue("Y", "float1");
                        AddCommonValue("Z", "float1");
                        AddCommonValue("W", "float1");
                        break;
                }
            }
        }

        protected void AddCommonValue(string strName, string strType)
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
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width=new GridLength(25) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            FloatValuesStack.Children.Add(grid);

            Label nameLabel = new Label()
            {
                Content = strName
            };
            grid.Children.Add(nameLabel);

            TextBox textBox = new TextBox()
            {
                MinWidth = 50,
                Text = "0"
            };
            Grid.SetColumn(textBox, 1);
            grid.Children.Add(textBox);
            m_valueTextboxs.Add(textBox);

            Ellipse ellipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.LightGreen,
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(-15,0,0,0)
            };
            grid.Children.Add(ellipse);
            AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), ellipse, enBezierType.Left, enLinkOpType.End, null, false);
            m_inLinks.Add(ellipse);

            Rectangle rect = new Rectangle()
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.LightGreen,
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, -15, 0)
            };
            Grid.SetColumn(rect, 1);
            grid.Children.Add(rect);
            AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), rect, enBezierType.Right, enLinkOpType.Start, rect.Fill, false);
            m_outLinks.Add(rect);

            ValueLinkData linkData = new ValueLinkData();
            linkData.m_name = strName;
            linkData.m_type = strType;
            linkData.m_inLink = ellipse;
            linkData.m_outLink = rect;
            m_linkDataDictionary[ellipse] = linkData;
            m_linkDataDictionary[rect] = linkData;
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            string strValueName = "";

            if (element == null || element == ValueOut)
            {
                if (String.IsNullOrEmpty(NameTextBox.Text))
                {
                    strValueName = "Value_" + CodeGenerateSystem.Program.GetValuedGUIDString(m_Guid);//m_Guid.ToString();
                    //strValueName = strValueName.Replace("-", "_");
                }
                else
                    strValueName = NameTextBox.Text;
            }
            else
            {
                string strBackword = "";
                switch(m_strValueType)
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
                return m_strParams;
            else
                return m_linkDataDictionary[element].m_type;
        }

        public string GetValueString()
        {
            string retStr = "";
            foreach (var textBox in m_valueTextboxs)
            {
                retStr += textBox.Text + ",";
            }

            retStr = retStr.Remove(retStr.Length - 1);
            return retStr;
        }

        public string GetCommonValueDefine()
        {
            return m_strParams + " " + GCode_GetValueName(null) + "\r\n";
        }

        public override void GCode_GenerateCode(ref string strSegment, int nLayer, FrameworkElement element)
        {
            string strTab = GCode_GetTabString(nLayer);

            // 检查参数是否已经在此代码段定义过
            string strIdentity = "";
            if(IsGeneric)
                strIdentity = GCode_GetValueName(null);
            else
                strIdentity = m_strValueType + " " + GCode_GetValueName(null);

            if (strSegment.Contains(strIdentity))
                return;

            var lOI = GetLinkObjInfo(ValueIn);
            if (lOI.bHasLink)
            {
                lOI.GetLinkObject(0, true).GCode_GenerateCode(ref strSegment, nLayer, lOI.GetLinkElement(0, true));
                if (!IsGeneric)
                {
                    strSegment += strTab + m_strValueType + " ";
                }
                else
                    strSegment += strTab;

                strSegment += GCode_GetValueName(null) + " = "
                                    + lOI.GetLinkObject(0, true).GCode_GetValueName(lOI.GetLinkElement(0, true)) + "\r\n";
            }
            else if(!IsGeneric)
            {
                switch(m_strValueType)
                {
                    case "int":
                    case "float1":
                        strSegment += strTab + m_strValueType + " " + GCode_GetValueName(null) + " = " + m_valueTextboxs[0].Text + ";\r\n";
                        break;

                    case "float2":
                    case "float3":
                    case "float4":
                        strSegment += strTab + m_strValueType + " " + GCode_GetValueName(null) + " = " + m_strValueType + "(" + GetValueString() + ");\r\n";
                        break;
                }
                
            }

            //处理xyzw等有连接的情况
            foreach (var link in m_inLinks)
            {
                var linkOI = GetLinkObjInfo(link);
                if (linkOI.bHasLink)
                {
                    linkOI.GetLinkObject(0, true).GCode_GenerateCode(ref strSegment, nLayer, linkOI.GetLinkElement(0, true));
                    strSegment += strTab + GCode_GetValueName(link) + " = " + linkOI.GetLinkObject(0, true).GCode_GetValueName(linkOI.GetLinkElement(0, true)) + "\r\n";
                }
            }
        }
    }
}
