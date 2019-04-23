using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace GamePublisher
{
    /// <summary>
    /// Interaction logic for FilesInfoControl.xaml
    /// </summary>
    public partial class FilesInfoControl : UserControl
    {
        ObservableCollection<ResourceData> mItemsSource = new ObservableCollection<ResourceData>();
        public ObservableCollection<ResourceData> ItemsSource
        {
            get { return mItemsSource; }
            set
            {
                mItemsSource = value;
                TreeView_Resources.ItemsSource = mItemsSource;
            }
        }

        public FilesInfoControl()
        {
            InitializeComponent();
        }

        private void TreeView_Resources_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            ResourceData data = TreeView_Resources.SelectedItem as ResourceData;
            if (data != null)
            {
                ListBox_Ref.ItemsSource = data.RefSources.Values;
            }
            else
                ListBox_Ref.ItemsSource = null;
        }
    }
}
