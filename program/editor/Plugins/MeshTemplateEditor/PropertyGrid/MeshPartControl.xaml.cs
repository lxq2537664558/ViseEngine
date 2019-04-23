using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Documents;

namespace MeshTemplateEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for MeshPartControl.xaml
    /// </summary>
    public partial class MeshPartControl : UserControl, INotifyPropertyChanged
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

        String mTempMeshName;

        public bool PreviewChecked
        {
            get { return (bool)GetValue(PreviewCheckedProperty); }
            set { SetValue(PreviewCheckedProperty, value); }
        }
        public static readonly DependencyProperty PreviewCheckedProperty =
            DependencyProperty.Register("PreviewChecked", typeof(bool), typeof(MeshPartControl), new PropertyMetadata(false, new PropertyChangedCallback(PreviewCheckedChangedCallback)));
        static void PreviewCheckedChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = sender as MeshPartControl;
            bool newValue = (bool)e.NewValue;

            if (null == ctrl.mParentControl)
                return;
            if (null == ctrl.LinkedMeshPart)
                return;

            if (newValue)
            {
                if (!string.IsNullOrEmpty(ctrl.mTempMeshName))
                    ctrl.LinkedMeshPart.MeshName = ctrl.mTempMeshName;
            }
            else
            {
                ctrl.mTempMeshName = ctrl.LinkedMeshPart.MeshName;
                ctrl.LinkedMeshPart.MeshName = "";
            }

            ctrl.mParentControl.UpdateMeshList();
        }

        public bool ShowChildren
        {
            get { return (bool)GetValue(ShowChildrenProperty); }
            set { SetValue(ShowChildrenProperty, value); }
        }
        public static readonly DependencyProperty ShowChildrenProperty =
            DependencyProperty.Register("ShowChildren", typeof(bool), typeof(MeshPartControl), new PropertyMetadata(true, new PropertyChangedCallback(ShowChildrenChangedCallback)));
        static void ShowChildrenChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = sender as MeshPartControl;
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                ctrl.Path_Expand_RotateTransform.Angle = 45;
                ctrl.StackPanel_Materials.Visibility = Visibility.Visible;
            }
            else
            {
                ctrl.Path_Expand_RotateTransform.Angle = 0;
                ctrl.StackPanel_Materials.Visibility = Visibility.Collapsed;
            }
        }

        private void Path_Expand_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowChildren = !ShowChildren;
        }

        public MeshInitControl mParentControl;

        string mMeshName = "";
        public string MeshName
        {
            get { return mMeshName; }
            set
            {
                mMeshName = value;

                OnPropertyChanged("MeshName");
            }
        }

        string mMeshFileName;
        public String MeshFileName
        {
            get { return mMeshFileName; }
            set
            {
                mMeshFileName = value;

                MeshName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(mMeshFileName);

                OnPropertyChanged("MeshFileName");
            }
        }

        CCore.Mesh.MeshInitPart mLinkdedMeshPart;
        public CCore.Mesh.MeshInitPart LinkedMeshPart
        {
            get { return mLinkdedMeshPart; }
            set
            { 
                if (null == StackPanel_Materials) 
                    return;
                mLinkdedMeshPart = value;
                if (null == mLinkdedMeshPart)
                    return;
                //TextBlock_MeshName.Text = m_linkdedMeshPart.MeshFileName;
                MeshFileName = mLinkdedMeshPart.MeshName;
                for(int i=0; i<mLinkdedMeshPart.Techs.Count; ++i)
                {
                    MaterialSetControl msCtrl = new MaterialSetControl(this);
                    msCtrl.Editable = Editable;
                    msCtrl.nIdx = i;
                    //msCtrl.materialNameStr = m_linkdedMeshPart.Materials[i];
                    //msCtrl.TechName = m_linkdedMeshPart.Techs[i];
                    msCtrl.MaterialTechId = mLinkdedMeshPart.Techs[i];
                    StackPanel_Materials.Children.Add(msCtrl);
                }
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

        public MeshPartControl()
        {
            InitializeComponent();
        }
        public MeshPartControl(MeshInitControl parent)
        {
            InitializeComponent();

            mParentControl = parent;
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(Rectangle_DragDropPlace);
        }

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StackPanel_Materials.Children.Clear();

            var objs = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("MeshSource");
            if (objs == null || objs.Length == 0)
                return;

            if (LinkedMeshPart == null)
                LinkedMeshPart = new CCore.Mesh.MeshInitPart();

            //MeshFileName = WPG.Data.EditorContext.SelectedMeshSourceData;
            //LinkedMeshPart.MeshName = WPG.Data.EditorContext.SelectedMeshSourceData;
            
            MeshFileName = (string)objs[0];
            LinkedMeshPart.MeshName = (string)objs[0];

            var mshInit = new CCore.Mesh.MeshInit();
            mshInit.MeshInitParts.Add(LinkedMeshPart);
            var mesh = new CCore.Mesh.Mesh();
            mesh.Initialize(mshInit, null);
            LinkedMeshPart.Techs.Clear();
            for (int i = 0; i < mesh.GetMaxMaterial(0); ++i)
            {
                MaterialSetControl msCtrl = new MaterialSetControl(this);
                msCtrl.nIdx = i;
                StackPanel_Materials.Children.Add(msCtrl);
                //LinkedMeshPart.Materials.Add("");
                //LinkedMeshPart.Techs.Add("");
                LinkedMeshPart.Techs.Add(Guid.Empty);
            }

            if (null != mParentControl)
                mParentControl.UpdateMeshList();
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(MeshName))
                EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", MeshFileName });
        }

        private void Button_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (null != mParentControl)
            {
                mParentControl.StackPanel_Parts.Children.Remove(this);
                if (null != LinkedMeshPart)
                {
                    mParentControl.MeshInitParts.Remove(LinkedMeshPart);
                    mParentControl.UpdateMeshList();
                }
            }
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
            foreach (var data in datas)
            {
                var resInfo = data as ResourcesBrowser.ResourceInfo;
                if (resInfo.ResourceType == "MeshSource")
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

        private void Rectangle_DragDropPlace_DragEnter(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                mDropAdorner.IsAllowDrop = false;

                switch(AllowResourceItemDrop(e))
                {
                    case enDropResult.Allow:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "创建模型部件";

                            mDropAdorner.IsAllowDrop = true;
                            var pos = e.GetPosition(Rectangle_DragDropPlace);
                            if (pos.X > 0 && pos.X < Rectangle_DragDropPlace.ActualWidth &&
                               pos.Y > 0 && pos.Y < Rectangle_DragDropPlace.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlace);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;

                    case enDropResult.Denial_NoDragAbleObject:
                    case enDropResult.Denial_UnknowFormat:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不包含合法的模型资源";

                            mDropAdorner.IsAllowDrop = false;
                            var pos = e.GetPosition(Rectangle_DragDropPlace);
                            if (pos.X > 0 && pos.X < Rectangle_DragDropPlace.ActualWidth &&
                               pos.Y > 0 && pos.Y < Rectangle_DragDropPlace.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlace);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;
                }
            }
        }

        private void Rectangle_DragDropPlace_DragLeave(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
                var layer = AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlace);
                layer.Remove(mDropAdorner);
            }
        }

        private void Rectangle_DragDropPlace_DragOver(object sender, DragEventArgs e)
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

        private void Rectangle_DragDropPlace_Drop(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                var layer = AdornerLayer.GetAdornerLayer(Rectangle_DragDropPlace);
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

                        if (resInfo.ResourceType != "MeshSource")
                            continue;

                        if (LinkedMeshPart == null)
                            LinkedMeshPart = new CCore.Mesh.MeshInitPart();

                        MeshFileName = resInfo.RelativeResourceFileName;
                        LinkedMeshPart.MeshName = resInfo.RelativeResourceFileName;

                        var mshInit = new CCore.Mesh.MeshInit();
                        mshInit.MeshInitParts.Add(LinkedMeshPart);
                        var mesh = new CCore.Mesh.Mesh();
                        mesh.Initialize(mshInit, null);
                        LinkedMeshPart.Techs.Clear();
                        for(int i=0; i<mesh.GetMaxMaterial(0); ++i)
                        {
                            var msCtrl = new MaterialSetControl(this);
                            msCtrl.nIdx = i;
                            StackPanel_Materials.Children.Add(msCtrl);
                            LinkedMeshPart.Techs.Add(Guid.Empty);
                        }

                        if (mParentControl != null)
                            mParentControl.UpdateMeshList();

                        break;
                    }
                }
            }
        }
        
        #endregion
    }
}
