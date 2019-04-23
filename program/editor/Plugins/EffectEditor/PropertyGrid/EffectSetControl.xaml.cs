using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EffectEditpr.PropertyGrid
{
    /// <summary>
    /// Interaction logic for UVAnimSetter.xaml
    /// </summary>
    public partial class EffectSetControl : UserControl
    {
        public string EffectName
        {
            get { return (string)GetValue(EffectNameProperty); }
            set { SetValue(EffectNameProperty, value); }
        }
        public static readonly DependencyProperty EffectNameProperty =
            DependencyProperty.Register("EffectName", typeof(string), typeof(EffectSetControl),
                                        new UIPropertyMetadata());

        public Guid EffectId
        {
            get { return (Guid)GetValue(EffectIdProperty); }
            set { SetValue(EffectIdProperty, value); }
        }
        public static readonly DependencyProperty EffectIdProperty =
            DependencyProperty.Register("EffectId", typeof(Guid), typeof(EffectSetControl),
            new FrameworkPropertyMetadata(Guid.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnEffectIdChanged)));

        public static void OnEffectIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EffectSetControl control = d as EffectSetControl;

            Guid oldValue = (Guid)e.OldValue;
            Guid newValue = (Guid)e.NewValue;

            if (newValue == oldValue)
                return;

            BindingOperations.ClearBinding(control, EffectNameProperty);
            if (newValue == Guid.Empty)
            {
                control.EffectName = "";
                control.ImageSource = null;
            }
            else
            {
                var effectTemplate = CCore.Effect.EffectManager.Instance.FindEffectTemplate(newValue);
                if (effectTemplate != null)
                {
                    BindingOperations.SetBinding(control, EffectNameProperty, new Binding("NickName") { Source = effectTemplate });
                }                
                else
                    control.EffectName = "";
                
                var file = CCore.Effect.EffectManager.Instance.GetEffectTemplateFile(newValue);
                if (!string.IsNullOrEmpty(file))
                {
                    var image = CSUtility.Support.IFileManager.Instance.Root + file + ResourcesBrowser.Program.SnapshotExt;
                    control.ImageSource = EditorCommon.ImageInit.GetImage(image, System.Drawing.Color.FromArgb(255, 0, 255));
                }                
            }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
                                            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(EffectSetControl),
                                                                        new UIPropertyMetadata());

        public EffectSetControl()
        {
            InitializeComponent();
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(LayoutRoot);            
        }

        EditorCommon.DragDrop.DropAdorner mDropAdorner;       

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("Effect");
            if (data == null)
                return;

            if (data.Length > 0)
            {
                EffectId = (Guid)data[0];
            }
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var fileName = CSUtility.Support.IFileManager.Instance.Root + CCore.Effect.EffectManager.Instance.GetEffectTemplateFile(EffectId);
            if (Guid.Empty != EffectId)
                EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", fileName });
        }

        private void Button_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           EffectId = Guid.Empty;
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
                if (resInfo.ResourceType == "Effect")
                {
                    containMeshSource = true;
                    break;
                }
            }

            if (!containMeshSource)
                return enDropResult.Denial_NoDragAbleObject;

            return enDropResult.Allow;
        }

        private void Rectangle_AddEffect_DragEnter(object sender, DragEventArgs e)
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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "添加Effect资源";

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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是Effect资源";

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

        private void Rectangle_AddEffect_DragLeave(object sender, DragEventArgs e)
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

        private void Rectangle_AddEffect_DragOver(object sender, DragEventArgs e)
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

        private void Rectangle_AddEffect_Drop(object sender, DragEventArgs e)
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
                        
                        EffectId = CSUtility.Program.GetIdFromFile(resInfo.RelativeResourceFileName);
                    }
                }
            }
        }
    }
}
