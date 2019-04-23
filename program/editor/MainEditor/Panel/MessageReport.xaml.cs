using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace MainEditor.Panel
{
    public class Message : INotifyPropertyChanged
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

        ImageSource mIcon = null;
        public ImageSource Icon
        {
            get { return mIcon; }
            set
            {
                mIcon = value;
                OnPropertyChanged("Icon");
            }
        }

        EditorCommon.MessageReport.enMessageType mMessageType = EditorCommon.MessageReport.enMessageType.Info;
        public EditorCommon.MessageReport.enMessageType MessageType
        {
            get { return mMessageType; }
        }

        DateTime mTime = DateTime.Now;
        public DateTime Time
        {
            get { return mTime; }
        }

        string mMessageStr = "";
        public string MessageStr
        {
            get { return mMessageStr; }
            set
            {
                mMessageStr = value;
                OnPropertyChanged("MessageStr");
            }
        }

        Brush mMessageBrush = Brushes.White;
        public Brush MessageBrush
        {
            get { return mMessageBrush; }
            set
            {
                mMessageBrush = value;
                OnPropertyChanged("MessageBrush");
            }
        }

        public Message(EditorCommon.MessageReport.enMessageType type, string message, MessageReport report)
        {
            mMessageType = type;
            switch (mMessageType)
            {
                case EditorCommon.MessageReport.enMessageType.Info:
                    {
                        MessageBrush = Brushes.White;
                        Icon = (report.TryFindResource("InfoImage") as Image).Source;
                    }
                    break;
                case EditorCommon.MessageReport.enMessageType.Warning:
                    {
                        MessageBrush = Brushes.Yellow;
                        Icon = (report.TryFindResource("WarningImage") as Image).Source;
                    }
                    break;
                case EditorCommon.MessageReport.enMessageType.Error:
                    {
                        MessageBrush = Brushes.Red;
                        Icon = (report.TryFindResource("ErrorImage") as Image).Source;
                    }
                    break;
            }

            MessageStr = message;
        }
    }

    /// <summary>
    /// MessageReport.xaml 的交互逻辑
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "OutPut")]
    [EditorCommon.PluginAssist.PluginMenuItem("窗口/输出")]
    [Guid("F2D4EF49-D013-4A25-B619-65050B856A5B")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MessageReport : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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

        #region EditorPlugin
        public string PluginName
        {
            get { return "输出"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl;
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {
        }
        #endregion
        
        CSUtility.Support.ThreadSafeObservableCollection<Message> mMessages = new CSUtility.Support.ThreadSafeObservableCollection<Message>();
        public CSUtility.Support.ThreadSafeObservableCollection<Message> Messages
        {
            get { return mMessages; }
        }

        UInt64 mErrorCount = 0;
        public UInt64 ErrorCount
        {
            get { return mErrorCount; }
            protected set
            {
                mErrorCount = value;
                OnPropertyChanged("ErrorCount");
            }
        }
        UInt64 mWarningCount = 0;
        public UInt64 WarningCount
        {
            get { return mWarningCount; }
            protected set
            {
                mWarningCount = value;
                OnPropertyChanged("WarningCount");
            }
        }
        UInt64 mInfoCount = 0;
        public UInt64 InfoCount
        {
            get { return mInfoCount; }
            protected set
            {
                mInfoCount = value;
                OnPropertyChanged("InfoCount");
            }
        }

        bool mShowError = true;
        public bool ShowError
        {
            get { return mShowError; }
            set
            {
                mShowError = value;
                ListBox_Messages.Items.Filter = new Predicate<object>(Contains);
                OnPropertyChanged("ShowError");
            }
        }
        bool mShowWarning = true;
        public bool ShowWarning
        {
            get { return mShowWarning; }
            set
            {
                mShowWarning = value;
                ListBox_Messages.Items.Filter = new Predicate<object>(Contains);
                OnPropertyChanged("ShowWarning");
            }
        }
        bool mShowInfo = true;
        public bool ShowInfo
        {
            get { return mShowInfo; }
            set
            {
                mShowInfo = value;
                ListBox_Messages.Items.Filter = new Predicate<object>(Contains);
                OnPropertyChanged("ShowInfo");
            }
        }

        public MessageReport()
        {
            InitializeComponent();

            mInstructionControl = new System.Windows.Controls.TextBlock()
            {
                Text = "编辑器输出信息",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            EditorCommon.MessageReport.Instance.OnReportMessage += ShowMessage;
        }

        bool Contains(object de)
        {
            var msg = de as Message;
            if (msg == null)
                return false;

            if(ShowError && msg.MessageType == EditorCommon.MessageReport.enMessageType.Error)
                return true;
            if (ShowWarning && msg.MessageType == EditorCommon.MessageReport.enMessageType.Warning)
                return true;
            if (ShowInfo && msg.MessageType == EditorCommon.MessageReport.enMessageType.Info)
                return true;

            return false;
        }

        public void ShowMessage(EditorCommon.MessageReport.enMessageType type, string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                switch(type)
                {
                    case EditorCommon.MessageReport.enMessageType.Info:
                        InfoCount++;
                        break;
                    case EditorCommon.MessageReport.enMessageType.Warning:
                        WarningCount++;
                        break;
                    case EditorCommon.MessageReport.enMessageType.Error:
                        ErrorCount++;
                        break;
                }

                var msg = new Message(type, message, this);
                mMessages.Add(msg);
            }));
        }
    }
}
