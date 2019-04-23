using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CodeGenerateSystem.Base;

namespace MaterialEditor.Controls
{
    /// <summary>
    /// MaterialControl.xaml 的交互逻辑
    /// </summary>
    public partial class MaterialControl : BaseNodeControl
    {
        // 确定此节点是用于给参数取值还是赋值
        // OutValue - true 此节点用于取值
        // OutValue - false 此节点用于赋值
        public bool OutValue
        {
            get;
            set;
        }

        class MaterialLinkData
        {
            public bool mCommon = false;
            public string mStrName;
            public string mStrType;
        }
        Dictionary<FrameworkElement, MaterialLinkData> mMaterialDataDic = new Dictionary<FrameworkElement, MaterialLinkData>();
        List<FrameworkElement> mInLinks = new List<FrameworkElement>();
        List<FrameworkElement> mOutLinks = new List<FrameworkElement>();

        public MaterialControl(Canvas parentDrawCanvas, string strParam)
            : base(parentDrawCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Title);

            IsDeleteable = false;

            if(!string.IsNullOrEmpty(strParam))
                OutValue = System.Convert.ToBoolean(strParam);

            mOutLinks.Clear();
            mInLinks.Clear();

            AddSemanticHandle("mLocalPos", "float4", false);
            AddSemanticHandle("mWorldPos", "float4", false);
            AddSemanticHandle("mViewPos", "float4", false);
            AddSemanticHandle("mProjPos", "float4", false);
            AddSemanticHandle("mDepth", "float1", false);
            AddSemanticHandle("mLocalNorm", "float4", false);
            AddSemanticHandle("mLocalTangent", "float4", false);
            AddSemanticHandle("mLoaclBinorm", "float4", false);
            AddSemanticHandle("mWorldNorm", "float4", false);
            AddSemanticHandle("mWorldTangent", "float4", false);
            AddSemanticHandle("mWorldBinorm", "float4", false);
            AddSemanticHandle("mViewPixelNormal", "float4", false);
            AddSemanticHandle("mViewVertexNormal", "float4", false);
            AddSemanticHandle("mVertexColor0", "float4", false);
            AddSemanticHandle("mEmissiveColor", "float4", false);
            AddSemanticHandle("mSpecularColor", "float4", false);
            AddSemanticHandle("mNormalUV", "float4", false);
            AddSemanticHandle("mEmissiveUV", "float4", false);
            AddSemanticHandle("mSpecularUV", "float4", false);
            AddSemanticHandle("mLightMapUV", "float4", false);
            AddSemanticHandle("mCubeEnvUV", "float4", false);
            AddSemanticHandle("mDX9Fix_VIDTerrain", "float4", false);
            AddSemanticHandle("mTerrainGradient", "float2", false);
            AddSemanticHandle("mDiffuseColor", "float4", true);
            AddSemanticHandle("mDiffuseUV", "float2", true);
            AddSemanticHandle("mBloom", "float1", true);
            AddSemanticHandle("mSpecularIntensity", "float1", true);
            AddSemanticHandle("mSpecularPower", "float1", true);
        }

        private void AddSemanticHandle(string strName, string strType, bool isCommon)
        {
            Grid grid = new Grid();

            if (isCommon)
                SemanticStackPanel.Children.Add(grid);
            else
                UnusedSemanticStackPanel.Children.Add(grid);

            TextBlock label = new TextBlock()
            {
                Text = strName + "(" + strType + ")",
                Foreground = Brushes.White,
                Margin = new Thickness(5)
            };
            grid.Children.Add(label);

            MaterialLinkData data = new MaterialLinkData();
            data.mStrName = strName;
            data.mStrType = strType;
            data.mCommon = isCommon;

            if (OutValue)
            {
                var rect = new CodeGenerateSystem.Controls.LinkOutControl()
                {
                    Width = 15,
                    Height = 15,
                    BackBrush = Program.GetBrushFromValueType(strType, this),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, -20, 0),
                    Direction = enBezierType.Right,
                };
                grid.Children.Add(rect);
                AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), rect, enBezierType.Right, enLinkOpType.Start, rect.BackBrush, true);

                mOutLinks.Add(rect);
                mMaterialDataDic[rect] = data;
            }
            else
            {
                var ellipse = new CodeGenerateSystem.Controls.LinkInControl()
                {
                    Width = 15,
                    Height = 15,
                    BackBrush = Program.GetBrushFromValueType(strType, this),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(-20, 0, 0, 0),
                    Direction = enBezierType.Left,
                };
                grid.Children.Add(ellipse);
                var linkObjInfo = AddLinkObject(LinkObjInfo.GetLinkTypeFromTypeString(strType), ellipse, enBezierType.Left, enLinkOpType.End, null, false);
                linkObjInfo.OnAddLinkInfo += LinkObjInfo_In_OnAddLinkInfo;
                linkObjInfo.OnDelLinkInfo += LinkObjInfo_In_OnDelLinkInfo;

                mInLinks.Add(ellipse);
                mMaterialDataDic[ellipse] = data;
            }
        }

        private void LinkObjInfo_In_OnAddLinkInfo(LinkInfo linkInfo)
        {
            var grid = linkInfo.m_linkToObjectInfo.LinkElement.Parent as Grid;
            if (grid == null)
                return;

            if(grid.Parent == UnusedSemanticStackPanel)
            {
                UnusedSemanticStackPanel.Children.Remove(grid);
                SemanticStackPanel.Children.Add(grid);
                mNeedLayoutUpdateLink = true;
            }
        }
        private void LinkObjInfo_In_OnDelLinkInfo(LinkInfo linkInfo)
        {
            var grid = linkInfo.m_linkToObjectInfo.LinkElement.Parent as Grid;
            if (grid == null)
                return;

            MaterialLinkData data;
            if(mMaterialDataDic.TryGetValue(linkInfo.m_linkToObjectInfo.LinkElement, out data))
            {
                if(!data.mCommon)
                {
                    if (grid.Parent == SemanticStackPanel)
                    {
                        SemanticStackPanel.Children.Remove(grid);
                        UnusedSemanticStackPanel.Children.Add(grid);
                        mNeedLayoutUpdateLink = true;
                    }
                }
            }

        }

        public override void GCode_GenerateCode(ref string strDefinitionSegment, ref string strSegment, int nLayer, FrameworkElement element)
        {
            string strTab = GCode_GetTabString(nLayer);

            foreach (var link in mInLinks)
            {
                var lOI = GetLinkObjInfo(link);
                if (!lOI.HasLink)
                    continue;

                lOI.GetLinkObject(0, true).GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, nLayer, lOI.GetLinkElement(0, true));
                strSegment += strTab + "pssem." + mMaterialDataDic[link].mStrName + " = " + lOI.GetLinkObject(0, true).GCode_GetValueName(lOI.GetLinkElement(0, true)) + ";\r\n";
            }
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            var data = mMaterialDataDic[element];
            return "pssem." + data.mStrName;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            var data = mMaterialDataDic[element];
            return data.mStrType;
        }

        // DiffuseColor | DiffuseUV | ...
        public string GetRequireValueString()
        {
            string strRet = "";

            if (OutValue)
            {
                foreach (var handle in mOutLinks)
                {
                    var lOI = GetLinkObjInfo(handle);
                    if (lOI.HasLink)
                        strRet += mMaterialDataDic[handle].mStrName.Substring(1) + "|";
                }
            }
            else
            {
                foreach (var handle in mInLinks)
                {
                    var lOI = GetLinkObjInfo(handle);
                    if(lOI.HasLink)
                        strRet += mMaterialDataDic[handle].mStrName.Substring(1) + "|";
                }
            }

            // 去掉最后一个" | "
            if(strRet.Length > 1)
                strRet = strRet.Remove(strRet.Length - 1);

            return strRet;
        }
    }
}
