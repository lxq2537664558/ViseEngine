using System.Windows;
using System.ComponentModel;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for CodeViewer.xaml
    /// </summary>
    public partial class CodeViewer : DockControl.Controls.DockAbleWindowBase, INotifyPropertyChanged
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

        bool mIsClosed = false;
        public bool IsClosed
        {
            get { return mIsClosed; }
        }

        public string Text_Client
        {
            get { return TextEditor_Code_Client.Text; }
            set
            {
                TextEditor_Code_Client.Text = value;

                ListView_Errors_Client.ItemsSource = AIEditor.CodeGenerate.CodeGenerator.CompileCode(value, CSUtility.Helper.enCSType.Client).Errors;
            }
        }

        public string Text_Server
        {
            get { return TextEditor_Code_Server.Text; }
            set
            {
                TextEditor_Code_Server.Text = value;

                ListView_Errors_Server.ItemsSource = AIEditor.CodeGenerate.CodeGenerator.CompileCode(value, CSUtility.Helper.enCSType.Server).Errors;
            }
        }

        public CodeViewer()
        {
            InitializeComponent();
            this.LayoutManaged = false;
            TextEditor_Code_Client.ShowLineNumbers = true;
            TextEditor_Code_Server.ShowLineNumbers = true;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            mIsClosed = true;
        }

    }
}
