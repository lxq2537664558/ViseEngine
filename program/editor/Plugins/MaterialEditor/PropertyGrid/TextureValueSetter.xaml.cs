using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace MaterialEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for TextureValueSetter.xaml
    /// </summary>
    public partial class TextureValueSetter : UserControl
    {
        CCore.Material.MaterialShaderVarInfo mMatShaderValInfo = null;
        MaterialShaderValueControl mParentControl = null;
        public int nIdx = 0;

        bool bInitFinish = false;

        public string TexturePath
        {
            get { return (string)GetValue(TexturePathProperty); }
            set
            {
                SetValue(TexturePathProperty, value);
            }
        }

        public static readonly DependencyProperty TexturePathProperty =
            DependencyProperty.Register("TexturePath", typeof(string), typeof(TextureValueSetter), new PropertyMetadata(new PropertyChangedCallback(OnTexturePathChangedCallback)));

        static void OnTexturePathChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextureValueSetter control = sender as TextureValueSetter;

            string newValue = (string)e.NewValue;
            if (string.IsNullOrEmpty(newValue))
                return;

            string strPath = CSUtility.Support.IFileManager.Instance.Root + newValue;
            control.Image_PreView.Source = EditorCommon.ImageInit.GetImage(strPath);
            if(!control.mMatShaderValInfo.VarValue.Equals(newValue))
                control.mMatShaderValInfo.VarValue = newValue;
        }

        //public string TexturePath
        //{
        //    get { return TextBlock_TexturePath.Text; }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //            return;

        //        TextBlock_TexturePath.Text = value;
                
        //        string strPath = CSUtility.Support.IFileManager.Instance.Root + value;
        //        Image_PreView.Source = EditorCommon.ImageInit.GetImage(strPath);
        //    }
        //}

        public TextureValueSetter()
        {
            InitializeComponent();
            bInitFinish = true;

            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(MainGrid);
        }

        public TextureValueSetter(CCore.Material.MaterialShaderVarInfo info, MaterialShaderValueControl parentControl)
        {
            InitializeComponent();

            mMatShaderValInfo = info;
            mParentControl = parentControl;

            Binding bind = new Binding();
            bind.Source = info;
            bind.Path = new System.Windows.PropertyPath("NickName");
            Label_Name.SetBinding(TextBlock.TextProperty, bind);

            TexturePath = info.VarValue;

            bInitFinish = true;
        }

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!bInitFinish)
                return;

            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("Texture");
            if (data == null)
                return;

            if (data.Length > 0)
            {
                TexturePath = (string)data[0];
            }
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var textureFileName = CSUtility.Support.IFileManager.Instance.Root + TexturePath;
            if (!string.IsNullOrEmpty(TexturePath))
                EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", textureFileName });
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!bInitFinish)
                return;

            TexturePath = "";

            if (mMatShaderValInfo != null)
                mMatShaderValInfo.VarValue = TexturePath;

            if (mParentControl != null)
            {
                // 重新刷新list（临时代码，日后寻找更合适的值回传方案）
                var list = new List<CCore.Material.MaterialShaderVarInfo>();
                list.AddRange(mParentControl.ShaderValueList);
                list[nIdx] = mMatShaderValInfo;
                mParentControl.ShaderValueList = list;
            }
        }

        #region 拖放操作

        EditorCommon.DragDrop.DropAdorner mDropAdorner;
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

        private void Rectangle_DragEnter(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;

                if (mDropAdorner == null)
                    mDropAdorner = new EditorCommon.DragDrop.DropAdorner(MainGrid);
                mDropAdorner.IsAllowDrop = false;

                switch (AllowResourceItemDrop(e))
                {
                    case enDropResult.Allow:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "设置贴图资源";

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

        private void Rectangle_DragLeave(object sender, DragEventArgs e)
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

        private void Rectangle_DragOver(object sender, DragEventArgs e)
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

        private void Rectangle_Drop(object sender, DragEventArgs e)
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


        #endregion
    }
}
