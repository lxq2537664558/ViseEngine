using System.Windows.Controls;

namespace MeshSourceEditor
{
    /// <summary>
    /// Interaction logic for PhysConvertControl.xaml
    /// </summary>
    public partial class PhysConvertControl : UserControl
    {
        //public List<SourceControl> SelectedSourceControls
        //{
        //    get;
        //    set;
        //}
        public string CurrentSourceDir;

        public PhysConvertControl()
        {
            InitializeComponent();
        }

        //private void GeneratePhysicMesh(string meshName, MidLayer.Physics.enPhysicGeometryType geoType)
        //{
        //    CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.NewXNDHolder();
        //    var att = holder.Node.AddAttrib("GeoType");
        //    att.BeginWrite();
        //    att.Write(geoType.ToString());
        //    att.EndWrite();

        //    switch (geoType)
        //    {
        //        case MidLayer.Physics.enPhysicGeometryType.Box:
        //        case MidLayer.Physics.enPhysicGeometryType.Capsule:
        //        case MidLayer.Physics.enPhysicGeometryType.Sphere:
        //        case MidLayer.Physics.enPhysicGeometryType.Plane:
        //            break;
        //    }
        //}

        private void Button_ConvexMesh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (SelectedSourceControls == null)
            //    return;

            //foreach (var ctrl in SelectedSourceControls)
            //{

            //} 
        }
    }
}
