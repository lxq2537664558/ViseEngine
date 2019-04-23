using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ResourcesBrowser.PropertyGrid
{
    /// <summary>
    /// TextureValueSetter.xaml 的交互逻辑
    /// </summary>
    public partial class TextureSetControl : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public TextureSetControl()
        {
            InitializeComponent();
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(LayoutRoot);            
        }

        EditorCommon.DragDrop.DropAdorner mDropAdorner;        

        public string TexturePath
        {
            get { return (string)GetValue(TexturePathProperty); }
            set
            {
                SetValue(TexturePathProperty, value);
            }
        }

        public static readonly DependencyProperty TexturePathProperty =
            DependencyProperty.Register("TexturePath", typeof(string), typeof(TextureSetControl), new PropertyMetadata(new PropertyChangedCallback(OnTexturePathChangedCallback)));

        static void OnTexturePathChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextureSetControl control = sender as TextureSetControl;

            string newValue = (string)e.NewValue;
            if (string.IsNullOrEmpty(newValue))
            {
                control.Image_PreView.Source = null;
                return;
            }
                

            string strPath = CSUtility.Support.IFileManager.Instance.Root + newValue;
            control.Image_PreView.Source = EditorCommon.ImageInit.GetImage(strPath);
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
                if (resInfo.ResourceType == "Texture")
                {
                    containMeshSource = true;
                    break;
                }
            }

            if (!containMeshSource)
                return enDropResult.Denial_NoDragAbleObject;

            return enDropResult.Allow;
        }                

        private void Rectangle_AddImage_DragEnter(object sender, DragEventArgs e)
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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "添加贴图资源";

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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是贴图资源";

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

        private void Rectangle_AddImage_DragLeave(object sender, DragEventArgs e)
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

        private void Rectangle_AddImage_DragOver(object sender, DragEventArgs e)
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

        private void Rectangle_AddImage_Drop(object sender, DragEventArgs e)
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

                        TexturePath = resInfo.RelativeResourceFileName;                        
                    }
                }
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("Texture");
            if (data == null)
                return;

            if (data.Length > 0)
            {
                TexturePath = (string)data[0];
            }
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            var textureFileName = CSUtility.Support.IFileManager.Instance.Root + TexturePath;
            if (!string.IsNullOrEmpty(TexturePath))
                EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", textureFileName });
        }

        private void Button_Del_Click(object sender, RoutedEventArgs e)
        {
            TexturePath = "";
        }
    }
}
