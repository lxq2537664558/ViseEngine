using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MainEditor.BackgroundWorkerView
{
    /// <summary>
    /// BGViewWithLoading.xaml 的交互逻辑
    /// </summary>
    public partial class BGViewItem : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        string mKeyName = "";
        public string KeyName
        {
            get { return mKeyName; }
            set
            {
                mKeyName = value;
                OnPropertyChanged("KeyName");
            }
        }

        string mInfoString = "";
        public string InfoString
        {
            get { return mInfoString; }
            set
            {
                mInfoString = value;
                OnPropertyChanged("InfoString");
            }
        }

        float mProgress = 0;
        public float Progress
        {
            get { return mProgress; }
            set
            {
                mProgress = value;
                OnPropertyChanged("Progress");
            }
        }

        public BGViewItem(bool withProgress)
        {
            InitializeComponent();

            if(withProgress)
            {
                Grid_WithProgress.Visibility = Visibility.Visible;
                Grid_WithOutProgress.Visibility = Visibility.Collapsed;
            }
            else
            {
                Grid_WithProgress.Visibility = Visibility.Collapsed;
                Grid_WithOutProgress.Visibility = Visibility.Visible;
            }
        }
    }
}
