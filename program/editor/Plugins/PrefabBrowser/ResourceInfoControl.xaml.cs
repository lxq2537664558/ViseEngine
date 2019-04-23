using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace PrefabBrowser
{
    /// <summary>
    /// Interaction logic for ResourceInfoControl.xaml
    /// </summary>
    public partial class ResourceInfoControl : UserControl, INotifyPropertyChanged
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

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(ResourceInfoControl), new FrameworkPropertyMetadata());

        Brush mForegroundBrush = Brushes.White;
        public Brush ForegroundBrush
        {
            get { return mForegroundBrush; }
            set
            {
                mForegroundBrush = value;
                OnPropertyChanged("ForegroundBrush");
            }
        }

        public ResourceInfoControl()
        {
            InitializeComponent();
        }

        public delegate void Delegate_OnOpenInExplorer();
        public Delegate_OnOpenInExplorer OnOpenInExplorer;
        private void Button_OpenInExplorer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OnOpenInExplorer != null)
                OnOpenInExplorer();
        }
    }
}
