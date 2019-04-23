using System.Windows;

namespace MainEditor.Arrangement
{
    /// <summary>
    /// Interaction logic for SaveArrangementWindow.xaml
    /// </summary>
    public partial class SaveArrangementWindow : DockControl.Controls.DockAbleWindowBase
    {
        public string ArrangementName
        {
            get;
            set;
        }

        public SaveArrangementWindow()
        {
            InitializeComponent();

            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
        }

        private void Button_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ArrangementName))
            {
                EditorCommon.MessageBox.Show("未设置名称");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Button_Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }        
    }
}
