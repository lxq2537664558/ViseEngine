using System.ComponentModel;
using System.Windows;

namespace WorldViewer
{
    /// <summary>
    /// Interaction logic for ActorLayerCreateWindow.xaml
    /// </summary>
    public partial class ActorLayerCreateWindow : Window, INotifyPropertyChanged
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

        string mLayerName = "新建层";
        public string LayerName
        {
            get { return mLayerName; }
            set
            {
                mLayerName = value;
                OnPropertyChanged("LayerName");
            }
        }

        public ActorLayerCreateWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_OK(object sender, System.Windows.RoutedEventArgs e)
        {
            // 判断层名称是否重复
            if (CSUtility.Component.IActorLayerManger.Instance.Layers.Contains(LayerName))
            {
                EditorCommon.MessageBox.Show("层名称重复，请修改!");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click_Cancel(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
