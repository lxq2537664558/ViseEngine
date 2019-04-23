using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MainEditor.Arrangement
{
    /// <summary>
    /// Interaction logic for DeleteArrangementWindow.xaml
    /// </summary>
    public partial class DeleteArrangementWindow : DockControl.Controls.DockAbleWindowBase
    {
        public DeleteArrangementWindow()
        {
            InitializeComponent();

            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);

            foreach (var arg in Arrangement.EditorArrangementManager.Instance.EditorArrangement)
            {
                TextBlock text = new TextBlock()
                {
                    Text = arg.Name,
                    Foreground = Brushes.White,
                    Tag = arg,
                };
                ComboBox_Arrangements.Items.Add(text);
            }

            if (ComboBox_Arrangements.Items.Count > 0)
                ComboBox_Arrangements.SelectedIndex = 0;
        }

        private void Button_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ComboBox_Arrangements.SelectedIndex < 0)
                return;

            Arrangement.ArrangementData arg = ((TextBlock)(ComboBox_Arrangements.SelectedItem)).Tag as Arrangement.ArrangementData;
            Arrangement.EditorArrangementManager.Instance.DeleteArrangement(arg.Id);

            ComboBox_Arrangements.Items.RemoveAt(ComboBox_Arrangements.SelectedIndex);

            MainEditor.MainWindow.Instance.UpdateArrangementConfigs();
        }
    }
}
