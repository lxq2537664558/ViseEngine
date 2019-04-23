using System.Windows.Controls;
using System.Windows.Data;

namespace UIEditor.Panel.Trigger
{
    /// <summary>
    /// Interaction logic for TriggerPropertyActionItem.xaml
    /// </summary>
    public partial class TriggerPropertyActionItem : UserControl
    {
        UISystem.Trigger.TriggerAction mAction;
        public UISystem.Trigger.TriggerAction Action
        {
            get { return mAction; }
        }

        public delegate void Delegate_OnRemove(TriggerPropertyActionItem item);
        public Delegate_OnRemove OnRemove;

        public TriggerPropertyActionItem(UISystem.Trigger.TriggerAction action)
        {
            InitializeComponent();

            mAction = action;

            if(action is UISystem.Trigger.TriggerAction_Property)
            {
                var propertyAction = action as UISystem.Trigger.TriggerAction_Property;
                TextBlock_Control.SetBinding(TextBlock.TextProperty, new Binding("WinName") { Source = propertyAction.TargetControl });
            }
            TextBlock_Info.SetBinding(TextBlock.TextProperty, new Binding("InfoString") { Source = mAction });
        }

        private void Button_Remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OnRemove != null)
                OnRemove(this);
        }
    }
}
