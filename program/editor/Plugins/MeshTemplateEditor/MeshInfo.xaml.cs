using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MeshTemplateEditor
{
    /// <summary>
    /// MeshInfo.xaml 的交互逻辑
    /// </summary>
    public partial class MeshInfo : UserControl
    {
        public MeshInfo()
        {
            InitializeComponent();
        }

        public void UpdateMeshInfo(CCore.Mesh.Mesh mesh)
        {
            TextBlock_Vertex.Text = mesh.GetMeshVertexNumber().ToString();
            TextBlock_Poly.Text = mesh.GetMeshPolyNumber().ToString();
            TextBlock_Atom.Text = mesh.GetMeshAtomNumber().ToString();
        }

    }
}
