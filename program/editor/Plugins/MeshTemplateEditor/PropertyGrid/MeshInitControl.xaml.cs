using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WPG.Data;

namespace MeshTemplateEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for MeshInitControl.xaml
    /// </summary>
    public partial class MeshInitControl : UserControl
    {
        public Property MyProperty
        {
            get { return (Property)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(Property), typeof(MeshInitControl), new UIPropertyMetadata(null));

        public List<CCore.Mesh.MeshInitPart> MeshInitParts
        {
            get { return (List<CCore.Mesh.MeshInitPart>)GetValue(MeshInitPartsProperty); }
            set { SetValue(MeshInitPartsProperty, value); }
        }
        public static readonly DependencyProperty MeshInitPartsProperty =
            DependencyProperty.Register("MeshInitParts", typeof(List<CCore.Mesh.MeshInitPart>), typeof(MeshInitControl), new PropertyMetadata(new PropertyChangedCallback(MeshInitPartsChangedCallback)));
        static void MeshInitPartsChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MeshInitControl meshInitCtrl = sender as MeshInitControl;
            meshInitCtrl.StackPanel_Parts.Children.Clear();
            if (e.NewValue != null)
            {
                foreach (var item in ((List<CCore.Mesh.MeshInitPart>)(e.NewValue)))
                {
                    MeshPartControl partCtrl = new MeshPartControl(meshInitCtrl);
                    partCtrl.Editable = meshInitCtrl.Editable;
                    partCtrl.LinkedMeshPart = item;
                    meshInitCtrl.StackPanel_Parts.Children.Add(partCtrl);
                }
            }
        }

        public bool ShowChildren
        {
            get { return (bool)GetValue(ShowChildrenProperty); }
            set { SetValue(ShowChildrenProperty, value); }
        }
        public static readonly DependencyProperty ShowChildrenProperty =
            DependencyProperty.Register("ShowChildren", typeof(bool), typeof(MeshInitControl), new PropertyMetadata(true, new PropertyChangedCallback(ShowChildrenChangedCallback)));
        static void ShowChildrenChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MeshInitControl meshInitCtrl = sender as MeshInitControl;
            bool newValue = (bool)e.NewValue;

            if(newValue)
            {
                meshInitCtrl.Path_Expand_RotateTransform.Angle = 45;
                meshInitCtrl.StackPanel_Parts.Visibility = Visibility.Visible;
            }
            else
            {
                meshInitCtrl.Path_Expand_RotateTransform.Angle = 0;
                meshInitCtrl.StackPanel_Parts.Visibility = Visibility.Collapsed;
            }
        }

        bool mEditable = true;
        public bool Editable
        {
            get { return mEditable; }
            set
            {
                mEditable = value;

                if (mEditable)
                    Button_Add.Visibility = Visibility.Visible;
                else
                    Button_Add.Visibility = Visibility.Collapsed;
            }
        }

        public void UpdateMeshList()
        {
            var list = new List<CCore.Mesh.MeshInitPart>();
            list.AddRange(MeshInitParts);
            MeshInitParts = list;
        }

        public MeshInitControl()
        {
            InitializeComponent();

            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(Retangle_AddMeshSource);
        }

        private void Button_Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MeshPartControl partCtrl = new MeshPartControl(this);
            CCore.Mesh.MeshInitPart iiPart = new CCore.Mesh.MeshInitPart();
            partCtrl.LinkedMeshPart = iiPart;
            MeshInitParts.Add(iiPart);
            StackPanel_Parts.Children.Add(partCtrl);

            UpdateMeshList();
        }

        private void Path_Expand_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowChildren = !ShowChildren;
        }


        #region DragDrop

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
            foreach(var data in datas)
            {
                var resInfo = data as ResourcesBrowser.ResourceInfo;
                if(resInfo.ResourceType == "MeshSource")
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
        public EditorCommon.DragDrop.DropAdorner DropAdorner
        {
            get { return mDropAdorner; }
        }

        private void Rectangle_AddMeshSource_DragEnter(object sender, DragEventArgs e)
        {
            if(EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                mDropAdorner.IsAllowDrop = false;

                switch(AllowResourceItemDrop(e))
                {
                    case enDropResult.Allow:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "创建模型部件";

                            mDropAdorner.IsAllowDrop = true;
                            var pos = e.GetPosition(Retangle_AddMeshSource);
                            if(pos.X > 0 && pos.X < Retangle_AddMeshSource.ActualWidth &&
                               pos.Y > 0 && pos.Y < Retangle_AddMeshSource.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(Retangle_AddMeshSource);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;

                    case enDropResult.Denial_NoDragAbleObject:
                    case enDropResult.Denial_UnknowFormat:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不包含合法的模型资源";

                            mDropAdorner.IsAllowDrop = false;
                            var pos = e.GetPosition(Retangle_AddMeshSource);
                            if(pos.X > 0 && pos.X < Retangle_AddMeshSource.ActualWidth &&
                               pos.Y > 0 && pos.Y < Retangle_AddMeshSource.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(Retangle_AddMeshSource);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;
                }
            }
        }

        private void Rectangle_AddMeshSource_DragLeave(object sender, DragEventArgs e)
        {
            if(EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
                var layer = AdornerLayer.GetAdornerLayer(Retangle_AddMeshSource);
                layer.Remove(mDropAdorner);
            }
        }

        private void Rectangle_AddMeshSource_DragOver(object sender, DragEventArgs e)
        {
            if(EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                if(AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void Rectangle_AddMeshSource_Drop(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                var layer = AdornerLayer.GetAdornerLayer(Retangle_AddMeshSource);
                layer.Remove(mDropAdorner);

                if(AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    var formats = e.Data.GetFormats();
                    var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                    foreach(var data in datas)
                    {
                        var resInfo = data as ResourcesBrowser.ResourceInfo;
                        if (resInfo == null)
                            continue;

                        if (resInfo.ResourceType != "MeshSource")
                            continue;

                        var partCtrl = new MeshPartControl(this);
                        var iiPart = new CCore.Mesh.MeshInitPart();
                        iiPart.MeshName = resInfo.RelativeResourceFileName;
                        var mshInit = new CCore.Mesh.MeshInit();
                        mshInit.MeshInitParts.Add(iiPart);
                        var mesh = new CCore.Mesh.Mesh();
                        mesh.Initialize(mshInit, null);
                        iiPart.Techs.Clear();
                        for (int i = 0; i < mesh.GetMaxMaterial(0); ++i)
                        {
                            iiPart.Techs.Add(Guid.Empty);
                        }
                        partCtrl.LinkedMeshPart = iiPart;
                        MeshInitParts.Add(iiPart);
                        StackPanel_Parts.Children.Add(partCtrl);

                        UpdateMeshList();
                    }
                }
            }
        }
        
        #endregion
    }
}
