using System.Windows;
using System.Windows.Controls;

namespace MainEditor.PluginAssist
{
    /// <summary>
    /// Interaction logic for PluginControlContainer.xaml
    /// </summary>
    public partial class PluginControlContainer : UserControl, EditorCommon.ITickInfo, DockControl.IDockAbleControl
    {
        public bool IsShowing { get; set; }
        public string KeyValue
        {
            get
            {
                if (PluginObject != null)
                    return PluginObject.PluginName;

                return "";
            }
        }

        public void SaveElement(CSUtility.Support.XmlNode node, CSUtility.Support.XmlHolder holder)
        {
            if (PluginObject != null)
            {
                var uiElement = PluginObject as System.Windows.FrameworkElement;
                if (uiElement == null)
                    return;

                node.AddAttrib("Type", uiElement.GetType().Assembly.FullName + "|" + uiElement.GetType().FullName);
                node.AddAttrib("GridRow", Grid.GetRow(uiElement).ToString());
                node.AddAttrib("GridColumn", Grid.GetColumn(uiElement).ToString());
                node.AddAttrib("HorizontalAlignment", uiElement.HorizontalAlignment.ToString());
                node.AddAttrib("VerticalAlignment", uiElement.VerticalAlignment.ToString());                

                foreach(var i in PluginManagerWindow.Instance.PluginItems)
                {
                    if (i.PluginObject == PluginObject)
                        node.AddAttrib("Id", i.Id.ToString());
                }
            }
        }
        public DockControl.IDockAbleControl LoadElement(CSUtility.Support.XmlNode node)
        {
            DockControl.IDockAbleControl ctr = null;
            if (node == null)
                return ctr;

            var childNode = node.FindNode("Element");
            if (childNode == null)
                return ctr;

            var att = childNode.FindAttrib("Id");
            if (att == null)
                return ctr;

            foreach (var i in PluginManagerWindow.Instance.PluginItems)
            {
                if (i.Id.ToString() == att.Value)
                {
                    ctr = Program.GetPluginControl(i.PluginObject);
                    break;
                }
            }

            return ctr;            

            //att = node.FindAttrib("GridRow");
            //if (att != null)
            //    Grid.SetRow(element, System.Convert.ToInt32(att.Value));
            //att = node.FindAttrib("GridColumn");
            //if (att != null)
            //    Grid.SetColumn(element, System.Convert.ToInt32(att.Value));
            //att = node.FindAttrib("HorizontalAlignment");
            //if (att != null)
            //{
            //    HorizontalAlignment alg;
            //    System.Enum.TryParse<HorizontalAlignment>(att.Value, out alg);
            //    element.HorizontalAlignment = alg;
            //}
            //att = node.FindAttrib("VerticalAlignment");
            //if (att != null)
            //{
            //    VerticalAlignment alg;
            //    System.Enum.TryParse<VerticalAlignment>(att.Value, out alg);
            //    element.VerticalAlignment = alg;
            //}
        }

        public void StartDrag()
        {
            var ddop = Content as EditorCommon.PluginAssist.IEditorDockManagerDragDropOperation;
            if (ddop != null)
                ddop.StartDrag();
        }
        public void EndDrag()
        {
            var ddop = Content as EditorCommon.PluginAssist.IEditorDockManagerDragDropOperation;
            if (ddop != null)
                ddop.EndDrag();
        }

        public bool LayoutNeedSaveContent
        {
            get { return true; }
        }

        public string PluginName = "";

        public PluginControlContainer()
        {
            InitializeComponent();
        }
        ~PluginControlContainer()
        {
            Clear();
        }

        EditorCommon.PluginAssist.IEditorPlugin mPluginObject;
        public EditorCommon.PluginAssist.IEditorPlugin PluginObject
        {
            get { return mPluginObject; }
            set
            {
                mPluginObject = value;

                if(mPluginObject is FrameworkElement)
                    this.Content = mPluginObject;
            }
        }

        public void Clear()
        {
            EditorCommon.TickInfo.Instance.RemoveTickInfo(this);
        }

        void EditorCommon.ITickInfo.Tick()
        {
            if (PluginObject != null)
                PluginObject.Tick();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            EditorCommon.TickInfo.Instance.AddTickInfo(this);
            PluginObject.OnActive();
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            EditorCommon.TickInfo.Instance.RemoveTickInfo(this);
            PluginObject.OnDeactive();
        }
    }
}
