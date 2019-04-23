using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Documents;

namespace MeshTemplateEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for MaterialSetControl.xaml
    /// </summary>
    public partial class MaterialSetControl : UserControl, INotifyPropertyChanged
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

        MeshPartControl m_ParentControl;

        public Guid MaterialTechId
        {
            get { return (Guid)GetValue(MaterialTechIdProperty); }
            set { SetValue(MaterialTechIdProperty, value); }
        }
        public static readonly DependencyProperty MaterialTechIdProperty =
            DependencyProperty.Register("MaterialTechId", typeof(Guid), typeof(MaterialSetControl), new PropertyMetadata(new PropertyChangedCallback(OnMaterialTechIdCallback)));
        static void OnMaterialTechIdCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MaterialSetControl control = sender as MaterialSetControl;
            Guid tagId = (Guid)e.NewValue;
            control.TechName = EditorCommon.Material.MaterialFileAssist.GetTechniqueName(tagId);
        }
        
        public int IdxShow
        {
            get { return mIdx + 1; }
        }

        int mIdx;
        public int nIdx
        {
            get { return mIdx; }
            set
            {
                mIdx = value;

                OnPropertyChanged("nIdx");
                OnPropertyChanged("IdxShow");
            }
        }

        //string m_nMaterialName;
        //public string materialNameStr
        //{
        //    get { return m_nMaterialName; }
        //    set
        //    {
        //        m_nMaterialName = value;
        //        OnPropertyChanged("materialNameStr");
        //    }
        //}

        string m_nTechName;
        public string TechName
        {
            get { return m_nTechName; }
            set
            {
                m_nTechName = value;
                OnPropertyChanged("TechName");
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

        public MaterialSetControl()
        {
            InitializeComponent();            
        }
        public MaterialSetControl(MeshPartControl parent)
        {
            InitializeComponent();

            m_ParentControl = parent;
        }

        private void Button_Set_Click(object sender, RoutedEventArgs e)
        {
            //materialNameStr = WPG.Data.EditorContext.SelectedMaterialData;
            //TechName = WPG.Data.EditorContext.SelectedMaterialTechData;

            //MidLayer.IMaterialParameter param = new MidLayer.IMaterialParameter();
            //param.Material = WPG.Data.EditorContext.SelectedMaterialData;
            //param.Tech = WPG.Data.EditorContext.SelectedMaterialTechData;
            //MaterialParameter = param;
//             if (WPG.Data.EditorContext.SelectedMaterialTechData == "")
//                 return;
//             MaterialTechId = Guid.Parse(WPG.Data.EditorContext.SelectedMaterialTechData);

            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("Technique");
            if (data == null)
                return;

            if (data.Length > 0)
            {
                MaterialTechId = CSUtility.Program.GetIdFromFile((string)data[0]);                
            }
            if (null!=m_ParentControl)
            {
                //m_ParentControl.LinkedMeshPart.Materials[nIdx] = materialNameStr;
                m_ParentControl.LinkedMeshPart.Techs[nIdx] = MaterialTechId;//TechName;

                if (null!=m_ParentControl.mParentControl)
                    m_ParentControl.mParentControl.UpdateMeshList();
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            //MidLayer.IMaterialParameter param = new MidLayer.IMaterialParameter();
            //param.Material = "";
            //param.Tech = "";
            //MaterialParameter = param;
            ////materialNameStr = "";
            ////TechName = "";
            MaterialTechId = Guid.Empty;

            if (null != m_ParentControl)
            {
                //m_ParentControl.LinkedMeshPart.Materials[nIdx] = "";
                m_ParentControl.LinkedMeshPart.Techs[nIdx] = Guid.Empty;
            }

            if (null != m_ParentControl && null!=m_ParentControl.mParentControl)
                m_ParentControl.mParentControl.UpdateMeshList();
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialTechId == Guid.Empty)
                return;
             
            var techFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(MaterialTechId);       
            EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", techFile });
        }

        private void Button_Hightlight_Click(object sender, RoutedEventArgs e)
        {
            if (null == m_ParentControl)
                return;

            if (m_ParentControl.LinkedMeshPart.BakTechs.Count != m_ParentControl.LinkedMeshPart.Techs.Count)
            {
                m_ParentControl.LinkedMeshPart.BakTechs.Clear();
                for(int i = 0; i < m_ParentControl.LinkedMeshPart.Techs.Count; ++i)
                {
                    m_ParentControl.LinkedMeshPart.BakTechs.Add(Guid.Parse("03cbbe58-1dd7-47ef-a218-7a43b0c7958f"));
                }
            }

            var t = m_ParentControl.LinkedMeshPart.Techs[nIdx];
            m_ParentControl.LinkedMeshPart.Techs[nIdx] = m_ParentControl.LinkedMeshPart.BakTechs[nIdx];
            m_ParentControl.LinkedMeshPart.BakTechs[nIdx] = t;

            if (null != m_ParentControl.mParentControl)
                m_ParentControl.mParentControl.UpdateMeshList();
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
                if (resInfo.ResourceType == "Technique")
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

        private void Rectangle_AddTech_DragEnter(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;

            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(LayoutRoot);

            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                mDropAdorner.IsAllowDrop = false;

                switch (AllowResourceItemDrop(e))
                {
                    case enDropResult.Allow:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "添加材质实例";

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
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是材质实例";

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

        private void Rectangle_AddTech_DragLeave(object sender, DragEventArgs e)
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

        private void Rectangle_AddTech_DragOver(object sender, DragEventArgs e)
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

        private void Rectangle_AddTech_Drop(object sender, DragEventArgs e)
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

                        MaterialTechId = CSUtility.Program.GetIdFromFile(resInfo.RelativeResourceFileName);
                        if (null != m_ParentControl)
                        {
                            //m_ParentControl.LinkedMeshPart.Materials[nIdx] = materialNameStr;
                            m_ParentControl.LinkedMeshPart.Techs[nIdx] = MaterialTechId;//TechName;

                            if (null != m_ParentControl.mParentControl)
                                m_ParentControl.mParentControl.UpdateMeshList();
                        }
                    }
                }
            }
        }
    }
}
