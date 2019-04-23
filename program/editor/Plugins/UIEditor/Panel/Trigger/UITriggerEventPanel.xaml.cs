using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace UIEditor.Panel.Trigger
{
    /// <summary>
    /// Interaction logic for UITriggerEventPanel.xaml
    /// </summary>
    public partial class UITriggerEventPanel : UserControl
    {
        UISystem.Trigger.EventTrigger mUITrigger;
        public UISystem.Trigger.EventTrigger UITrigger
        {
            get { return mUITrigger; }
            set
            {
                if (mUITrigger != null)
                    mUITrigger.OnTriggerPropertyChanged -= OnUITriggerPropertyChanged;

                mUITrigger = value;
                if (mUITrigger != null)
                {
                    mUITrigger.OnTriggerPropertyChanged += OnUITriggerPropertyChanged;

                    if (mUITrigger.Condition.TargetControl != null && !string.IsNullOrEmpty(mUITrigger.Condition.TargetEventName))
                    {
                        BindingOperations.ClearBinding(TextBlock_TargetControl, TextBlock.TextProperty);
                        BindingOperations.SetBinding(TextBlock_TargetControl, TextBlock.TextProperty, new Binding("NameInEditor") { Source = mUITrigger.Condition.TargetControl });

                        UpdateEventsComboBox(mUITrigger.Condition.TargetControl);

                        ComboBox_Events.SelectedItem = mUITrigger.Condition.TargetEventName;
                    }
                    else
                    {
                        if (Program.mSelectionWinControlsCollection.Count > 0)
                            SetEventConditionTargetControl(Program.mSelectionWinControlsCollection[0]);
                    }

                    StackPanel_ActivePropertys.Children.Clear();
                    foreach (var action in mUITrigger.Actions)
                    {
                        AddAction(action);
                    }
                }
            }
        }

        public UITriggerEventPanel()
        {
            InitializeComponent();
        }

        private void UpdateEventsComboBox(UISystem.WinBase control)
        {
            if (control == null)
                return;

            ComboBox_Events.Items.Clear();
            var events = TypeDescriptor.GetEvents(control);
            foreach (EventDescriptor eventInfo in events)
            {
                ComboBox_Events.Items.Add(eventInfo.Name);
            }
        }

        private void SetEventConditionTargetControl(UISystem.WinBase control)
        {
            if (control == null)
                return;

            var targetControl = control;
            if (control is UISystem.Template.ControlTemplate)
                targetControl = ((UISystem.Template.ControlTemplate)control).TargetControl;

            mUITrigger.Condition.TargetControl = targetControl;
            BindingOperations.ClearBinding(TextBlock_TargetControl, TextBlock.TextProperty);
            BindingOperations.SetBinding(TextBlock_TargetControl, TextBlock.TextProperty, new Binding("NameInEditor") { Source = mUITrigger.Condition.TargetControl });

            UpdateEventsComboBox(targetControl);

            if(ComboBox_Events.Items.Count > 0)
                ComboBox_Events.SelectedIndex = 0;
        }

        private void OnUITriggerPropertyChanged(UISystem.Trigger.UITrigger trigger)
        {
            StackPanel_ActivePropertys.Children.Clear();
            if (mUITrigger.Actions.Count > 0)
            {
                foreach (var action in mUITrigger.Actions)
                {
                    AddAction(action);
                }
            }
        }

        private TriggerPropertyActionItem AddAction(UISystem.Trigger.TriggerAction action)
        {
            if (action is UISystem.Trigger.TriggerAction_Property)
            {
                var item = new TriggerPropertyActionItem(action);
                item.OnRemove = RemovePropertyAction;
                StackPanel_ActivePropertys.Children.Add(item);
                return item;
            }

            return null;
        }

        private void RemovePropertyAction(TriggerPropertyActionItem item)
        {
            UITrigger.RemoveAction(item.Action);
        }

        private void Button_SetControl_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(Program.mSelectionWinControlsCollection.Count > 0)
                SetEventConditionTargetControl(Program.mSelectionWinControlsCollection[0]);
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Events.SelectedIndex < 0)
                return;

            mUITrigger.Condition.TargetEventName = ComboBox_Events.SelectedItem.ToString();
        }

        private void Button_AddOperationOnActive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
        }

        private void Button_AddOperationOnDeactive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
        }
    }
}
