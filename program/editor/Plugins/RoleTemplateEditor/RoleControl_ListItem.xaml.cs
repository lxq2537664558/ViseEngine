using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace RoleTemplateEditor
{
    /// <summary>
    /// Interaction logic for RoleControl_ListItem.xaml
    /// </summary>
    public partial class RoleControl_ListItem : UserControl, INotifyPropertyChanged
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

        public delegate void Delegate_OnPreviewCheckedChanged(RoleControl_ListItem item);
        public Delegate_OnPreviewCheckedChanged OnPreviewCheckedChanged;
        public delegate void Delegate_OnActionFileChanged(RoleControl_ListItem item);
        public Delegate_OnActionFileChanged OnActionFileChanged;

        bool mActionNameEditable = false;
        public bool ActionNameEditable
        {
            get { return mActionNameEditable; }
            set
            {
                mActionNameEditable = value;

                if (mActionNameEditable)
                {
                    TextBox_ActionName.Visibility = Visibility.Visible;
                    TextBlock_ActionName.Visibility = Visibility.Hidden;
                }
                else
                {
                    TextBox_ActionName.Visibility = Visibility.Hidden;
                    TextBlock_ActionName.Visibility = Visibility.Visible;
                }
            }
        }

        bool mPreViewChecked = false;
        public bool PreViewChecked
        {
            get { return mPreViewChecked; }
            set
            {
                mPreViewChecked = value;

                if (OnPreviewCheckedChanged != null)
                    OnPreviewCheckedChanged(this);

                OnPropertyChanged("PreViewChecked");
            }
        }

        public string ActionName
        {
            get { return (string)GetValue(ActionNameProperty); }
            set { SetValue(ActionNameProperty, value); }
        }
        public static readonly DependencyProperty ActionNameProperty =
            DependencyProperty.Register("ActionName", typeof(string), typeof(RoleControl_ListItem), new UIPropertyMetadata(null));

        public string ActionFile
        {
            get { return (string)GetValue(ActionFileProperty); }
            set { SetValue(ActionFileProperty, value); }
        }
        public static readonly DependencyProperty ActionFileProperty =
            DependencyProperty.Register("ActionFile", typeof(string), typeof(RoleControl_ListItem), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnActionFilePropertyChanged)));

        public static void OnActionFilePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RoleControl_ListItem ctrl = d as RoleControl_ListItem;

            if (ctrl.OnActionFileChanged != null)
            {
                ctrl.OnActionFileChanged(ctrl);
            }
        }

        public RoleControl_ListItem()
        {
            InitializeComponent();
        }
    }
}
