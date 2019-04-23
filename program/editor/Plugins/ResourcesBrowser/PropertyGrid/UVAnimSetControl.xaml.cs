using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ResourcesBrowser.PropertyGrid
{
    /// <summary>
    /// Interaction logic for UVAnimSetter.xaml
    /// </summary>
    public partial class UVAnimSetControl : UserControl
    {
        public string UVAnimName
        {
            get { return (string)GetValue(UVAnimNameProperty); }
            set { SetValue(UVAnimNameProperty, value); }
        }
        public static readonly DependencyProperty UVAnimNameProperty =
            DependencyProperty.Register("UVAnimName", typeof(string), typeof(UVAnimSetControl),
                                        new UIPropertyMetadata());

        public Guid UVAnimId
        {
            get { return (Guid)GetValue(UVAnimIdProperty); }
            set { SetValue(UVAnimIdProperty, value); }
        }
        public static readonly DependencyProperty UVAnimIdProperty =
            DependencyProperty.Register("UVAnimId", typeof(Guid), typeof(UVAnimSetControl),
            new FrameworkPropertyMetadata(Guid.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnUVAnimIdChanged)));

        public static void OnUVAnimIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UVAnimSetControl control = d as UVAnimSetControl;

            Guid oldValue = (Guid)e.OldValue;
            Guid newValue = (Guid)e.NewValue;

            if (newValue == oldValue)
                return;

            BindingOperations.ClearBinding(control, UVAnimNameProperty);
            if (newValue == Guid.Empty)
            {
                control.UVAnimName = "";
                control.ImageSource = null;
            }
            else
            {
                var anim = UISystem.UVAnimMgr.Instance.Find(newValue, true);
                if (anim != null)
                {
                    //control.UVAnimName = anim.UVAnimName;
                    BindingOperations.SetBinding(control, UVAnimNameProperty, new Binding("UVAnimName") { Source = anim });
                }
                else
                    control.UVAnimName = "";
                
                var file = UISystem.UVAnimMgr.Instance.GetUVAnimFileName(newValue);
                if (!string.IsNullOrEmpty(file))
                {
                    var image = CSUtility.Support.IFileManager.Instance.Root + file + Program.SnapshotExt;
                    control.ImageSource = EditorCommon.ImageInit.GetImage(image);
                }                
            }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
                                            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(UVAnimSetControl),
                                                                        new UIPropertyMetadata());

        public UVAnimSetControl()
        {
            InitializeComponent();
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(LayoutRoot);            
        }

        EditorCommon.DragDrop.DropAdorner mDropAdorner;       

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("UVAnim");
            if (data == null)
                return;

            if (data.Length > 0)
            {
                UVAnimId = CSUtility.Program.GetIdFromFile((string)data[0]);

            }
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            var fileName = CSUtility.Support.IFileManager.Instance.Root + UISystem.UVAnimMgr.Instance.GetUVAnimFileName(UVAnimId);
            if (Guid.Empty != UVAnimId)
                EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", fileName });
        }

        private void Button_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UVAnimId = Guid.Empty;
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
                if (resInfo.ResourceType == "UVAnim")
                {
                    containMeshSource = true;
                    break;
                }
            }

            if (!containMeshSource)
                return enDropResult.Denial_NoDragAbleObject;

            return enDropResult.Allow;
        }

        private void Rectangle_AddUVAnim_DragEnter(object sender, DragEventArgs e)
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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "添加UVAnim资源";

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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是UVAnim资源";

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

        private void Rectangle_AddUVAnim_DragLeave(object sender, DragEventArgs e)
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

        private void Rectangle_AddUVAnim_DragOver(object sender, DragEventArgs e)
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

        private void Rectangle_AddUVAnim_Drop(object sender, DragEventArgs e)
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
                        var resInfo = data as ResourcesBrowser.ResourceInfo;
                        if (resInfo == null)
                            continue;
                        
                        UVAnimId = CSUtility.Program.GetIdFromFile(resInfo.RelativeResourceFileName);
                    }
                }
            }
        }
    }
}
