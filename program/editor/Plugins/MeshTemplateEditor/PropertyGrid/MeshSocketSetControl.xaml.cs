using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MeshTemplateEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for MeshSocketSetControl.xaml
    /// </summary>
    public partial class MeshSocketSetControl : UserControl
    {
        public string SocketName
        {
            get { return (string)GetValue(SocketNameProperty); }
            set { SetValue(SocketNameProperty, value); }
        }

        public static readonly DependencyProperty SocketNameProperty =
            DependencyProperty.Register("SocketName", typeof(string), typeof(MeshSocketSetControl), new UIPropertyMetadata(""));

        public MeshSocketSetControl()
        {
            InitializeComponent();
        }

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var objs = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("MeshSocket");
            if (objs == null)
                return;
            if (objs.Length == 0)
                return;

            SocketName = (string)(objs[0]);
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SocketName = "";
        }

        #region DragDrop

        EditorCommon.DragDrop.DropAdorner mDropAdorner;
        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(LayoutRoot);

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals(Program.SocketDragType))
            {
                e.Handled = true;

                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "设置模型挂接点";

                mDropAdorner.IsAllowDrop = true;
                var pos = e.GetPosition(element);
                if (pos.X > 0 && pos.X < element.ActualWidth &&
                    pos.Y > 0 && pos.Y < element.ActualHeight)
                {
                    var layer = AdornerLayer.GetAdornerLayer(element);
                    layer.Add(mDropAdorner);
                }

            }
        }
        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals(Program.SocketDragType))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer.Remove(mDropAdorner);
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals(Program.SocketDragType))
            {
                e.Handled = true;
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals(Program.SocketDragType))
            {
                e.Handled = true;
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer.Remove(mDropAdorner);

                var formats = e.Data.GetFormats();
                var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                if (datas.Length == 0)
                    return;

                var tb = datas[0] as EditorControlLib.CustomTextBlock;
                SocketName = tb.Text;
            }
        }

        #endregion
    }
}
