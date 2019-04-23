using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace DelegateMethodEditor
{
    /// <summary>
    /// Interaction logic for EventListControl.xaml
    /// </summary>
    public partial class EventListControl : UserControl, INotifyPropertyChanged
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

        public delegate void Delegate_OnSelectionChanged(EventListItem item);
        public Delegate_OnSelectionChanged OnSelectionChanged;

        string mEventDelegateFilter = "";
        public string EventFilter
        {
            get { return mEventDelegateFilter; }
            set
            {
                mEventDelegateFilter = value;

                var strLower = mEventDelegateFilter.ToLower();
                foreach (EventListItem item in ListBox_Events.Items)
                {
                    if(string.IsNullOrEmpty(mEventDelegateFilter))
                        item.Visibility = Visibility.Visible;
                    else
                    {
                        if (item.NickName.ToLower().Contains(strLower))
                            item.Visibility = Visibility.Visible;
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                }

                OnPropertyChanged("EventFilter");
            }
        }

        Visibility mAddButtonVisibility = Visibility.Visible;
        public Visibility AddButtonVisibility
        {
            get { return mAddButtonVisibility; }
            set
            {
                mAddButtonVisibility = value;

                OnPropertyChanged("AddButtonVisibility");
            }
        }

        Visibility mDelButtonVisibility = Visibility.Visible;
        public Visibility DelButtonVisibility
        {
            get { return mDelButtonVisibility; }
            set
            {
                mDelButtonVisibility = value;

                OnPropertyChanged("DelButtonVisibility");
            }
        }

        Type mDelegateType = null;
        public Type DelegateType
        {
            get { return mDelegateType; }
            set
            {
                mDelegateType = value;

                ListBox_Events.Items.Clear();

                foreach (var item in Program.GetEventList(mDelegateType))
                {
                    EventListItem lbItem = new EventListItem(item.EventCallBack);
                    lbItem.Copy(item);
                    ListBox_Events.Items.Add(lbItem);
                }
            }
        }

        public EventListControl()
        {
            InitializeComponent();
        }

        public void SelectedEvent(Guid eventId)
        {
            foreach (EventListItem eItem in ListBox_Events.Items)
            {
                if (eItem.EventId == eventId)
                {
                    ListBox_Events.SelectedValue = eItem;
                    break;
                }
            }
        }

        private void ListBox_Events_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(ListBox_Events.SelectedIndex < 0)
                return;

            // 保存旧的函数连线


            if (OnSelectionChanged != null)
                OnSelectionChanged(ListBox_Events.SelectedItem as EventListItem);
        }

        private void Button_AddDelegate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DelegateType == null)
                return;

            CSUtility.Helper.EventCallBack callBack = new CSUtility.Helper.EventCallBack(CSUtility.Helper.enCSType.Client)
            {
                Id = Guid.NewGuid(),
                CBType = DelegateType,
                NickName = DelegateType.Name,
                Description = DelegateType.FullName,
            };

            EventListItem item = new EventListItem(callBack);
            Program.AddEvent(item);
            ListBox_Events.Items.Add(item);
        }

        private void Button_DelDelegate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ListBox_Events.SelectedIndex < 0)
                return;

            EventListItem item = ListBox_Events.SelectedItem as EventListItem;
            // SVN
            {
                var absDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultEventDirectory + "\\" + item.EventId.ToString();

                if(EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                {
                    EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                    {
                        if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} 目录{absDir}使用版本控制删除失败!");
                        }
                        else
                        {
                            EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                            {
                                if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                {
                                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} 目录{absDir}使用版本控制删除失败!");
                                }
                            }, absDir, $"AutoCommit 删除逻辑图{item.NickName}");
                        }
                    }, absDir);
                }
                else
                {
                    if(System.IO.Directory.Exists(absDir))
                        System.IO.Directory.Delete(absDir, true);
                }
            }

            CSUtility.Helper.EventCallBackVersionManager.Instance.RemoveEventCallBack(item.EventId);

            Program.DelEvent(item);
            ListBox_Events.Items.RemoveAt(ListBox_Events.SelectedIndex);
        }
    }
}
