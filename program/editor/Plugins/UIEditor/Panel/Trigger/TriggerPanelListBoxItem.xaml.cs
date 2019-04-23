using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace UIEditor.Panel.Trigger
{
    /// <summary>
    /// Interaction logic for TriggerPanelListBoxItem.xaml
    /// </summary>
    public partial class TriggerPanelListBoxItem : UserControl, INotifyPropertyChanged
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

        UITriggerPanel mHostTriggerPanel;
        UISystem.Trigger.UITrigger mUITrigger;
        public UISystem.Trigger.UITrigger UITrigger
        {
            get { return mUITrigger; }
        }

        public string TriggerInfo
        {
            get { return (string)GetValue(TriggerInfoProperty); }
            set
            {
                SetValue(TriggerInfoProperty, value);
            }
        }

        public static readonly DependencyProperty TriggerInfoProperty =
            DependencyProperty.Register("TriggerInfo", typeof(string), typeof(TriggerPanelListBoxItem), new UIPropertyMetadata());

        Visibility mRecordVisible = Visibility.Collapsed;
        public Visibility RecordVisible
        {
            get { return mRecordVisible; }
            set
            {
                mRecordVisible = value;
                OnPropertyChanged("RecordVisible");
            }
        }

        Visibility mEnableVisible = Visibility.Collapsed;
        public Visibility EnableVisible
        {
            get { return mEnableVisible; }
            set
            {
                mEnableVisible = value;
                OnPropertyChanged("EnableVisible");
            }
        }

        bool mIsRecording = false;
        public bool IsRecording
        {
            get { return mIsRecording; }
            set
            {
                mIsRecording = value;

                Program.SetRecordMode("触发器 " + TriggerInfo, mIsRecording);
                if (mIsRecording)
                    Border_Record.Visibility = System.Windows.Visibility.Visible;
                else
                    Border_Record.Visibility = System.Windows.Visibility.Hidden;

                OnPropertyChanged("IsRecording");
            }
        }

        bool mIsSelected = false;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                mIsSelected = value;

                if (mUITrigger == null)
                {

                }
                else if (mUITrigger is UISystem.Trigger.PropertyTrigger)
                {
                    if (mIsSelected)
                    {
                        RecordVisible = Visibility.Visible;
                    }
                    else
                    {
                        RecordVisible = Visibility.Hidden;
                        if(IsRecording)
                            IsRecording = false;
                    }
                }
                else if (mUITrigger is UISystem.Trigger.EventTrigger)
                {
                    if (mIsSelected)
                        RecordVisible = Visibility.Visible;
                    else
                    {
                        RecordVisible = Visibility.Hidden;
                        if (IsRecording)
                            IsRecording = false;
                    }
                }

                OnPropertyChanged("IsSelected");
            }
        }

        public TriggerPanelListBoxItem(UISystem.Trigger.UITrigger trigger, UITriggerPanel hostTriggerPanel)
        {
            InitializeComponent();

            mUITrigger = trigger;

            mHostTriggerPanel = hostTriggerPanel;
            if (mUITrigger == null)
            {
                TriggerInfo = "默认值";
            }
            else if (mUITrigger is UISystem.Trigger.PropertyTrigger)
            {
                UISystem.Trigger.PropertyTrigger propertyTrigger = trigger as UISystem.Trigger.PropertyTrigger;

                propertyTrigger.OnTriggerPropertyChanged += PropertyTrigger_OnTriggerPropertyChanged;

                if (propertyTrigger.IsTriggerAvailable())
                    EnableVisible = Visibility.Visible;
                else
                    EnableVisible = Visibility.Hidden;

                RecordVisible = Visibility.Hidden;
                //BindingOperations.ClearBinding(this, TriggerInfoProperty);
                //BindingOperations.SetBinding(this, TriggerInfoProperty, new Binding("InfoString") { Source = propertyTrigger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
            else if (mUITrigger is UISystem.Trigger.EventTrigger)
            {
                UISystem.Trigger.EventTrigger eventTrigger = trigger as UISystem.Trigger.EventTrigger;

                eventTrigger.OnTriggerPropertyChanged += EventTrigger_OnTriggerPropertyChanged;

                if (eventTrigger.IsTriggerAvailable())
                    EnableVisible = Visibility.Visible;
                else
                    EnableVisible = Visibility.Hidden;

                RecordVisible = Visibility.Hidden;
            }

            UpdateShowWithTrigger(trigger);
        }

        public void UpdateEnable()
        {
            if (mUITrigger == null)
            {

            }
            else if (mUITrigger is UISystem.Trigger.PropertyTrigger)
            {
                UISystem.Trigger.PropertyTrigger propertyTrigger = mUITrigger as UISystem.Trigger.PropertyTrigger;
                if (propertyTrigger.IsTriggerAvailable())
                    EnableVisible = Visibility.Visible;
                else
                    EnableVisible = Visibility.Hidden;
            }
            else if (mUITrigger is UISystem.Trigger.EventTrigger)
            {
                UISystem.Trigger.EventTrigger eventTrigger = mUITrigger as UISystem.Trigger.EventTrigger;
                if (eventTrigger.IsTriggerAvailable())
                    EnableVisible = Visibility.Visible;
                else
                    EnableVisible = Visibility.Hidden;
            }
        }

        void PropertyTrigger_OnTriggerPropertyChanged(UISystem.Trigger.PropertyTrigger trigger)
        {
            UpdateShowWithTrigger(trigger);
        }

        void EventTrigger_OnTriggerPropertyChanged(UISystem.Trigger.EventTrigger trigger)
        {
            UpdateShowWithTrigger(trigger);
        }

        private void UpdateShowWithTrigger(UISystem.Trigger.UITrigger trigger)
        {
            if (trigger is UISystem.Trigger.PropertyTrigger)
            {
                var propertyTrigger = trigger as UISystem.Trigger.PropertyTrigger;

                StackPanel_Info.Children.Clear();

                int i = 0;
                foreach (var condition in propertyTrigger.Conditions)
                {
                    if (i > 0)
                    {
                        TextBlock txAnd = new TextBlock() { Text = " 和 " };
                        StackPanel_Info.Children.Add(txAnd);
                    }

                    //StackPanel stPanel = new StackPanel()
                    //{
                    //    Orientation = Orientation.Horizontal
                    //};
                    //TextBlock textWin = new TextBlock();
                    //BindingOperations.SetBinding(textWin, TextBlock.TextProperty, new Binding("TargetName") { Source = condition });
                    //stPanel.Children.Add(textWin);

                    //TextBlock tx = new TextBlock() { Text = "." };
                    //stPanel.Children.Add(tx);

                    //TextBlock textProprety = new TextBlock();
                    //BindingOperations.SetBinding(textProprety, TextBlock.TextProperty, new Binding("TargetPropertyName") { Source = condition });
                    //stPanel.Children.Add(textProprety);

                    //tx = new TextBlock() { Text = " = " };
                    //stPanel.Children.Add(tx);

                    //TextBlock textValue = new TextBlock();
                    //BindingOperations.SetBinding(textValue, TextBlock.TextProperty, new Binding("TargetValueString") { Source = condition });
                    //stPanel.Children.Add(textValue);

                    TextBlock textInfo = new TextBlock();
                    BindingOperations.SetBinding(textInfo, TextBlock.TextProperty, new Binding("ConditionInfoString") { Source = condition });

                    StackPanel_Info.Children.Add(textInfo);
                    i++;
                }
            }
            else if (trigger is UISystem.Trigger.EventTrigger)
            {
                var eventTrigger = trigger as UISystem.Trigger.EventTrigger;

                StackPanel_Info.Children.Clear();

                BindingOperations.ClearBinding(this, TriggerInfoProperty);
                BindingOperations.SetBinding(this, TriggerInfoProperty, new Binding("ConditionInfoString") { Source = eventTrigger.Condition });
            }
        }
    }
}
