using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace UIEditor.Panel.PropertyAndBind
{
    /// <summary>
    /// Interaction logic for CommandBindControl.xaml
    /// </summary>
    public partial class CommandBindControl : UserControl, INotifyPropertyChanged
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

        System.Reflection.EventInfo mEvent = null;
        public System.Reflection.EventInfo Event
        {
            get { return mEvent; }
            set
            {
                mEvent = value;

                if (mEvent != null)
                {
                    EventName = mEvent.Name;
                    UpdateCommandBindInfo();
                }
            }
        }

        string mEventName = "";
        public string EventName
        {
            get { return mEventName; }
            set
            {
                mEventName = value;
                OnPropertyChanged("EventName");
            }
        }

        protected UISystem.WinBase mHostWinBase = null;

        public CommandBindControl(object obj)
        {
            InitializeComponent();

            mHostWinBase = obj as UISystem.WinBase;
        }

        private void UpdateCommandBindInfo()
        {
            var form = mHostWinBase.GetRoot(typeof(UISystem.WinForm));

            var commandInfo = mHostWinBase.GetCommandBindingInfoFromEventName(EventName);
            foreach (var command in commandInfo)
            {
                // 不显示模板内的命令
                if (mHostWinBase.IsTemplateCommand(EventName, command))
                    continue;

                var splits = command.Split('.');
                if (splits.Length < 2)
                    return;

                CommandBindItem item = new CommandBindItem(mHostWinBase, mEvent);
                ListBox_Items.Items.Add(item);

                Guid id = Guid.Parse(splits[0]);
                var ctrl = ((UISystem.WinBase)form).FindControl(id);
                item.SetTarget(ctrl, splits[1]);
            }
        }

        private void Button_Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommandBindItem item = new CommandBindItem(mHostWinBase, mEvent);
            ListBox_Items.Items.Add(item);
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ListBox_Items.SelectedIndex < 0)
                return;

            //var list = mHostWinBase.GetCommandBindingInfoFromEventName(mEvent.Name);
            //list.RemoveAt(ListBox_Items.SelectedIndex);
            mHostWinBase.RemoveCommandBinding(mEvent.Name, ListBox_Items.SelectedIndex);
            ListBox_Items.Items.RemoveAt(ListBox_Items.SelectedIndex);
        }
    }
}
