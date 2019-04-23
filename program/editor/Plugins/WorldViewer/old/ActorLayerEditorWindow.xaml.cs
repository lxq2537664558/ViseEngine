using System.Windows;

namespace WorldViewer
{
    /// <summary>
    /// Interaction logic for ActorLayerEditorWindow.xaml
    /// </summary>
    public partial class ActorLayerEditorWindow : Window
    {
        public ActorLayerEditorWindow()
        {
            InitializeComponent();

            foreach (var layerName in CSUtility.Component.IActorLayerManger.Instance.Layers)
            {
                ListBox_Layers.Items.Add(layerName);
            }
        }

        private void Button_Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActorLayerCreateWindow win = new ActorLayerCreateWindow();
            win.Owner = this;
            if (win.ShowDialog() == true)
            {
                CSUtility.Component.IActorLayerManger.Instance.AddLayer(win.LayerName);
                ListBox_Layers.Items.Add(win.LayerName);

                CSUtility.Component.IActorLayerManger.Instance.Save();
            }
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ListBox_Layers.SelectedIndex < 0)
                return;

            string layerName = ListBox_Layers.SelectedValue.ToString();
            if (CSUtility.Component.IActorLayerManger.Instance.RemoveLayer(layerName))
            {
                ListBox_Layers.Items.RemoveAt(ListBox_Layers.SelectedIndex);

                CSUtility.Component.IActorLayerManger.Instance.Save();
            }
        }
    }
}
