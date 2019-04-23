using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace ScenePointEditor
{
    /// <summary>
    /// Interaction logic for ScenePointGroupItem.xaml
    /// </summary>
    public partial class ScenePointGroupItem : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        string mGroupShowName = "未命名";
        public string GroupShowName
        {
            get { return mGroupShowName; }
            set
            {
                mGroupShowName = value;
                OnPropertyChanged("GroupShowName");
            }
        }

        Visibility mDirtyVisibility = Visibility.Collapsed;
        public Visibility DirtyVisibility
        {
            get { return mDirtyVisibility; }
            set
            {
                mDirtyVisibility = value;
                OnPropertyChanged("DirtyVisibility");
            }
        }

        CSUtility.Map.ScenePointGroup mLinkedGroup;
        public CSUtility.Map.ScenePointGroup LinkedGroup
        {
            get { return mLinkedGroup; }
        }

        public ScenePointGroupItem(CSUtility.Map.ScenePointGroup linkedGroup)
        {
            InitializeComponent();
            mLinkedGroup = linkedGroup;
            mLinkedGroup.OnDirtyChanged = _OnDirtyChanged;
            mLinkedGroup.OnNameChanged = _OnNameChanged;
            mLinkedGroup.OnTypeChanged = _OnTypeChanged;
            UpdateShowName();
        }

        private void UpdateShowName()
        {
            if (mLinkedGroup == null)
                return;

            GroupShowName = mLinkedGroup.Name + "(" + mLinkedGroup.SPGType.ToString() + ")";
        }

        private void _OnDirtyChanged(bool dirty)
        {
            DirtyVisibility = (dirty) ? Visibility.Visible : Visibility.Collapsed;
        }
        private void _OnNameChanged(string name)
        {
            UpdateShowName();
        }
        private void _OnTypeChanged(CSUtility.Map.ScenePointGroup.enScenePointGroupType type)
        {
            UpdateShowName();
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Escape:
                    TextBox_Name.Visibility = Visibility.Collapsed;
                    TextBlock_Name.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox_Name.Visibility = Visibility.Collapsed;
            TextBlock_Name.Visibility = Visibility.Visible;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TextBox_Name.Visibility = Visibility.Visible;
                TextBlock_Name.Visibility = Visibility.Collapsed;
            }
        }

    }
}
