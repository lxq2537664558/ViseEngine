using System.Windows.Controls;
using System.ComponentModel;
using System;
using System.Windows;
using System.Windows.Data;

namespace ActionNotifyEditor
{
    /// <summary>
    /// Interaction logic for AttackNotifyPointListItem.xaml
    /// </summary>
    public partial class NotifyPointListItem : UserControl, INotifyPropertyChanged
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

        CSUtility.Animation.NotifyItemDataBase mPointItemData;
        public CSUtility.Animation.NotifyItemDataBase PointItemData
        {
            get { return mPointItemData; }
        }

        public NotifyPointListItem(CSUtility.Animation.NotifyItemDataBase itemData)
        {
            InitializeComponent();

            PropertyGrid_Pro.Instance = itemData;
            mPointItemData = itemData;

            HeadLine = mPointItemData.HostNotifyPoint.HeaderName + itemData.Index.ToString();

            BindingOperations.ClearBinding(this, IndexProperty);
            BindingOperations.SetBinding(this, IndexProperty, new Binding("Index") { Source = itemData });
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(NotifyPointListItem),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIndexChanged)));

        public static void OnIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotifyPointListItem control = d as NotifyPointListItem;

            int newValue = (int)e.NewValue;

            control.HeadLine = control.PointItemData.HostNotifyPoint.HeaderName + newValue.ToString();
        }

        string mHeadLine;
        public string HeadLine
        {
            get { return mHeadLine; }
            set
            {
                mHeadLine = value;
                OnPropertyChanged("HeadLine");
            }
        }
    }
}
