using System.ComponentModel;
using System.Windows.Controls;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for StateGridColumnHeader.xaml
    /// </summary>
    public partial class StateGridColumnHeader : UserControl, INotifyPropertyChanged
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

        string mNickName = "";
        public string NickName
        {
            get { return mNickName; }
            set
            {
                mNickName = value;
                OnPropertyChanged("NickName");
            }
        }

        string mStateType = null;
        public string StateType
        {
            get { return mStateType; }
            set
            {
                mStateType = value;
                OnPropertyChanged("StateType");
            }
        }

        public StateGridColumnHeader(string stateType)
        {
            InitializeComponent();

            StateType = stateType;
        }

        public void ChangeStateName(string stateName, string newNickName)
        {
            if (StateType == stateName)
                NickName = newNickName;
        }
    }
}
