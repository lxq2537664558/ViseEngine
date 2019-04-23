using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for ScenePointGroup.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("参数.场景点组", "获取地图中的场景点组")]
    public partial class ScenePointGroup : CodeGenerateSystem.Base.BaseNodeControl
    {
        Guid mMapId = Guid.Empty;
        Guid mGroupId = Guid.Empty;

        public ScenePointGroup(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;
            SetDragObject(RectangleTitle);

            NodeName = "参数.场景点组";

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class, ValueLinkHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueLinkHandle.BackBrush, true);

            UpdateMapComboBox();
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("MapId", mMapId.ToString());
            xmlNode.AddAttrib("GroupId", mGroupId.ToString());

            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            var att = xmlNode.FindAttrib("MapId");
            if (att != null)
            {
                Guid.TryParse(att.Value, out mMapId);

                foreach (TextBlock item in ComboBox_Map.Items)
                {
                    var mapInit = item.Tag as CSUtility.Map.WorldInit;
                    if (mapInit.WorldId == mMapId)
                    {
                        ComboBox_Map.SelectedItem = item;
                        break;
                    }
                }
            }
            att = xmlNode.FindAttrib("GroupId");
            if (att != null)
            {
                Guid.TryParse(att.Value, out mGroupId);

                foreach (TextBlock item in ComboBox_Group.Items)
                {
                    var group = item.Tag as CSUtility.Map.ScenePointGroup;
                    if (group.Id == mGroupId)
                    {
                        ComboBox_Group.SelectedItem = item;
                        break;
                    }
                }
            }

            base.Load(xmlNode, deltaX, deltaY);
        }

        private void UpdateMapComboBox()
        {
            var mapDir = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(CSUtility.Support.IFileConfig.MapDirectory);

            ComboBox_Map.Items.Clear();

            if (!System.IO.Directory.Exists(mapDir))
                return;
            foreach (var dir in System.IO.Directory.EnumerateDirectories(mapDir))
            {
                var file = dir + "//Config.map";

                var mapInit = new CSUtility.Map.WorldInit();
                CSUtility.Support.IConfigurator.FillProperty(mapInit, file);

                var text = new TextBlock()
                {
                    Text = mapInit.WorldName,
                    Tag = mapInit
                };
                ComboBox_Map.Items.Add(text);
            }
        }

        private void UpdateGroupComboBox()
        {
            ComboBox_Group.Items.Clear();

            var groups = CSUtility.Map.ScenePointGroupManager.Instance.LoadAllGroups(mMapId);

            foreach (var group in groups)
            {
                var text = new TextBlock()
                {
                    Text = group.Name,
                    Tag = group
                };
                ComboBox_Group.Items.Add(text);
            }
        }

        private void ComboBox_Map_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Map.SelectedItem != null)
            {
                var tb = ComboBox_Map.SelectedItem as TextBlock;
                var mapInit = tb.Tag as CSUtility.Map.WorldInit;

                mMapId = mapInit.WorldId;
            }

            UpdateGroupComboBox();
        }

        private void ComboBox_Group_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Group.SelectedItem != null)
            {
                var tb = ComboBox_Group.SelectedItem as TextBlock;
                var group = tb.Tag as CSUtility.Map.ScenePointGroup;

                mGroupId = group.Id;
            }
        }

        #region 代码生成

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "CSUtility.Map.ScenePointGroup";
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodeSnippetExpression("CSUtility.Map.ScenePointGroupManager.Instance.FindGroup(System.Guid.Parse(\"" + mMapId.ToString() + "\"), System.Guid.Parse(\"" + mGroupId.ToString() + "\"))");
        }

        #endregion
    }
}
