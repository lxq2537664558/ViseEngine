using System;
using System.ComponentModel;
using System.Windows;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace WorldViewer
{
    /// <summary>
    /// Interaction logic for PropertyPanel.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "Propertys")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/Propertys")]
    [Guid("885159F4-ECBE-4BC5-B0E1-379F0F2F64DD")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class PropertyPanel : System.Windows.Controls.UserControl, System.ComponentModel.INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public string PluginName
        {
            get { return "Propertys"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "Propertys",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void Tick()
        {

        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        //////////////////////////////////////////////////////////////////////////

        //static PropertyPanel smInstance = null;
        //public static PropertyPanel Instance
        //{
        //    get 
        //    {
        //        if (smInstance == null)
        //            smInstance = new PropertyPanel();
        //        return smInstance;
        //    }
        //}

        public object PropertyInstance
        {
            get { return PropertyGrid.Instance; }
            set
            {
                PropertyGrid.Instance = value;
                if(value != null)
                    PropertyGrid.Headline = value.GetType().ToString();
            }
        }

        public PropertyPanel()
        {
            InitializeComponent();

        }
    }
}
