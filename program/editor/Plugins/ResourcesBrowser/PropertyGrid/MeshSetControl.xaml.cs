using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ResourcesBrowser.PropertyGrid
{
    /// <summary>
    /// MeshSetControl.xaml 的交互逻辑
    /// </summary>
    public partial class MeshSetControl : UserControl, INotifyPropertyChanged
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

        public MeshSetControl()
		{
			InitializeComponent();
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(LayoutRoot);           
        }

        bool mEditable = true;
        public bool Editable
        {
            get { return mEditable; }
            set
            {
                mEditable = value;

                if (mEditable)
                {
                    Button_Set.Visibility = Visibility.Visible;
                    Button_Delete.Visibility = Visibility.Visible;
                }
                else
                {
                    Button_Set.Visibility = Visibility.Collapsed;
                    Button_Delete.Visibility = Visibility.Collapsed;
                }
            }
        }
        
        public Guid CurMeshId
        {
            get { return (Guid)GetValue(CurMeshIdProperty); }
            set { SetValue(CurMeshIdProperty, value); }
        }

        public static readonly DependencyProperty CurMeshIdProperty =
            DependencyProperty.Register("CurMeshId", typeof(Guid), typeof(MeshSetControl),
            new FrameworkPropertyMetadata(Guid.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnCurMeshIdChanged)));
        public static void OnCurMeshIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MeshSetControl ctrl = d as MeshSetControl;

            Guid newValue = (Guid)e.NewValue;

            var meshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(newValue);
            if (meshTemplate != null)
            {                
                ctrl.TextBox_MeshId.Text = meshTemplate.NickName;

                var file = CCore.Mesh.MeshTemplateMgr.Instance.GetMeshTemplateFile(newValue);
                if (!string.IsNullOrEmpty(file))
                {
                    var image = CSUtility.Support.IFileManager.Instance.Root + file + Program.SnapshotExt;
                    ctrl.ImageSource = EditorCommon.ImageInit.GetImage(image);
                }
            }
        }

        ImageSource mImageSource;
        public ImageSource ImageSource
        {
            get { return mImageSource; }
            set
            {
                mImageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }        

        private void Button_Set_Click(object sender, RoutedEventArgs e)
        {
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("MeshTemplate");
            if (data == null)
                return;

            if (data.Length > 0)
            {                
                CurMeshId = CSUtility.Program.GetIdFromFile((string)data[0]);                
            }
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {                        
            var meshFileName = CSUtility.Support.IFileManager.Instance.Root + CCore.Mesh.MeshTemplateMgr.Instance.GetMeshTemplateFile(CurMeshId);
            
            EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", meshFileName });
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CurMeshId = Guid.Empty;
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
                if (resInfo.ResourceType == "MeshTemplate")
                {
                    containMeshSource = true;
                    break;
                }
            }

            if (!containMeshSource)
                return enDropResult.Denial_NoDragAbleObject;

            return enDropResult.Allow;
        }

        EditorCommon.DragDrop.DropAdorner mDropAdorner;


        private void Rectangle_AddMesh_DragEnter(object sender, DragEventArgs e)
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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "添加模型资源";

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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是模型资源";

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

        private void Rectangle_AddMesh_DragLeave(object sender, DragEventArgs e)
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

        private void Rectangle_AddMesh_DragOver(object sender, DragEventArgs e)
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

        private void Rectangle_AddMesh_Drop(object sender, DragEventArgs e)
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

                        CurMeshId = CSUtility.Program.GetIdFromFile(resInfo.RelativeResourceFileName);
                    }
                }
            }
        }
    }
}
