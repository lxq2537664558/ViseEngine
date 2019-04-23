using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for UVAnimSetter.xaml
    /// </summary>
    public partial class AISetControl : UserControl
    {
        public string AIName
        {
            get { return (string)GetValue(AINameProperty); }
            set { SetValue(AINameProperty, value); }
        }
        public static readonly DependencyProperty AINameProperty =
            DependencyProperty.Register("AIName", typeof(string), typeof(AISetControl),
                                        new UIPropertyMetadata());

        public Guid AIId
        {
            get { return (Guid)GetValue(AIIdProperty); }
            set { SetValue(AIIdProperty, value); }
        }
        public static readonly DependencyProperty AIIdProperty =
            DependencyProperty.Register("AIId", typeof(Guid), typeof(AISetControl),
            new FrameworkPropertyMetadata(Guid.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnAIIdChanged)));

        public static void OnAIIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AISetControl control = d as AISetControl;

            Guid oldValue = (Guid)e.OldValue;
            Guid newValue = (Guid)e.NewValue;

            if (newValue == oldValue)
                return;
            
            if (newValue == Guid.Empty)
            {
                control.AIName = "";                
            }
            else
            {
                var info = FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(newValue, true);
                control.AIName = info?.Name;
            }
        }        

        public AISetControl()
        {
            InitializeComponent();
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(LayoutRoot);            
        }

        EditorCommon.DragDrop.DropAdorner mDropAdorner;       

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("AI");
            if (data == null)
                return;

            if (data.Length > 0)
            {
                AIId = (Guid)data[0];
            }
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Guid.Empty == AIId)
                return;

            var dir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;
            var dirs = System.IO.Directory.GetDirectories(dir, AIId.ToString(), System.IO.SearchOption.AllDirectories);
            if (dirs.Length > 0)
            {
                EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", dirs[0] });
            }            
        }

        private void Button_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AIId = Guid.Empty;
        }

        enum enDropResult
        {
            Denial_UnknowFormat,
            Denial_NoDragAbleObject,
            Allow,
        }
        // 是否允许拖放
        enDropResult AllowResourceItemDrop(System.Windows.DragEventArgs e)
        {
            var formats = e.Data.GetFormats();
            if (formats == null || formats.Length == 0)
                return enDropResult.Denial_UnknowFormat;

            var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
            if (datas == null)
                return enDropResult.Denial_NoDragAbleObject;

            bool containMeshSource = false;
            foreach (var data in datas)
            {
                var resInfo = data as ResourcesBrowser.ResourceInfo;
                if (resInfo.ResourceType == "AI")
                {
                    containMeshSource = true;
                    break;
                }
            }

            if (!containMeshSource)
                return enDropResult.Denial_NoDragAbleObject;

            return enDropResult.Allow;
        }

        private void Rectangle_AddAI_DragEnter(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                mDropAdorner.IsAllowDrop = false;

                switch (AllowResourceItemDrop(e))
                {
                    case enDropResult.Allow:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "添加AI资源";

                            mDropAdorner.IsAllowDrop = true;
                            var pos = e.GetPosition(element);
                            if (pos.X > 0 && pos.X < element.ActualWidth &&
                               pos.Y > 0 && pos.Y < element.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(element);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;

                    case enDropResult.Denial_NoDragAbleObject:
                    case enDropResult.Denial_UnknowFormat:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是AI资源";

                            mDropAdorner.IsAllowDrop = false;
                            var pos = e.GetPosition(element);
                            if (pos.X > 0 && pos.X < element.ActualWidth &&
                               pos.Y > 0 && pos.Y < element.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(element);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;
                }
            }
        }

        private void Rectangle_AddAI_DragLeave(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer.Remove(mDropAdorner);
            }
        }

        private void Rectangle_AddAI_DragOver(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                if (AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void Rectangle_AddAI_Drop(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer.Remove(mDropAdorner);

                if (AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    var formats = e.Data.GetFormats();
                    var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                    foreach (var data in datas)
                    {
                        var resInfo = data as AIResourceInfo;
                        if (resInfo == null)
                            continue;

                        AIId = resInfo.Id;
                    }
                }
            }
        }
    }
}
