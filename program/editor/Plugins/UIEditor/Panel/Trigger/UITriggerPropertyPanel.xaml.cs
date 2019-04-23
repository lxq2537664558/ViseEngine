using System.Windows.Controls;
using System.ComponentModel;

namespace UIEditor.Panel.Trigger
{
    /// <summary>
    /// Interaction logic for UITriggerPropertyPanel.xaml
    /// </summary>
    public partial class UITriggerPropertyPanel : UserControl
    {
        UISystem.Trigger.PropertyTrigger mUITrigger;
        public UISystem.Trigger.PropertyTrigger UITrigger
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

                    // 根据Trigger的信息设置属性内容
                    StackPanel_PropertysCondition.Children.Clear();
                    if (mUITrigger.Conditions.Count > 0)
                    {
                        foreach (var condition in mUITrigger.Conditions)
                        {
                            AddCondition(condition);
                        }
                    }
                    else
                    {
                        // 至少有一个条件
                        UISystem.WinBase selectedControl = null;
                        PropertyDescriptor targetProperty = null;
                        object targetValue = null;
                        if (Program.mSelectionWinControlsCollection.Count > 0)
                        {
                            selectedControl = Program.mSelectionWinControlsCollection[0];

                            if (selectedControl is UISystem.Template.ControlTemplate)
                                selectedControl = ((UISystem.Template.ControlTemplate)selectedControl).TargetControl;

                            targetProperty = TypeDescriptor.GetProperties(selectedControl)[0];
                            targetValue = targetProperty.GetValue(selectedControl);
                        }
                        var condition = new UISystem.Trigger.PropertyTriggerConditon(selectedControl, targetProperty, UISystem.Trigger.PropertyTriggerConditon.enValueOperator.Equal, targetValue);
                        mUITrigger.AddCondition(condition);
                        //UITrigger.UpdateInfoString();
                        //var item = AddCondition(condition);

                        //PropertyTriggerConditionItem item = StackPanel_PropertysCondition.Children[0] as PropertyTriggerConditionItem;
                        //if (Program.mSelectionWinControlsCollection.Count > 0)
                        //{
                        //    item.WinControl = Program.mSelectionWinControlsCollection[0];
                        //}
                    }

                    StackPanel_ActivePropertys.Children.Clear();
                    foreach (var action in mUITrigger.Actions)
                    {
                        AddAction(action);
                    }
                }
            }
        }

        public UITriggerPropertyPanel()
        {
            InitializeComponent();
        }

        private void OnUITriggerPropertyChanged(UISystem.Trigger.UITrigger trigger)
        {
            StackPanel_PropertysCondition.Children.Clear();
            if (mUITrigger.Conditions.Count > 0)
            {
                foreach (var condition in mUITrigger.Conditions)
                {
                    AddCondition(condition);
                }
            }

            StackPanel_ActivePropertys.Children.Clear();
            if (mUITrigger.Actions.Count > 0)
            {
                foreach (var action in mUITrigger.Actions)
                {
                    AddAction(action);
                }
            }
        }

        private PropertyTriggerConditionItem AddCondition(UISystem.Trigger.PropertyTriggerConditon condition)
        {
            PropertyTriggerConditionItem item = new PropertyTriggerConditionItem(condition);
            //item.OnUpdateTriggerInfo = UITrigger.UpdateInfoString;
            item.OnRemove = RemoveCondition;
            StackPanel_PropertysCondition.Children.Add(item);
            return item;
        }

        //public void OnSelectedWinControlsCollectionChanged(ObservableCollection<UISystem.WinBase> collection)
        //{
        //    if(collection.Count <= 0)
        //        return;

        //    foreach (PropertyTriggerConditionItem item in StackPanel_PropertysCondition.Children)
        //    {
        //        if (item.SetWinControlMode)
        //            item.WinControl = collection[0];
        //    }
        //}

        private void RemoveCondition(PropertyTriggerConditionItem item)
        {
            if (UITrigger.Conditions.Count == 1)
            {
                EditorCommon.MessageBox.Show("无法删除，触发器至少需要一个条件");
                return;
            }
            UITrigger.RemoveCondition(item.Condition);
            //StackPanel_PropertysCondition.Children.Remove(item);
            //UITrigger.UpdateInfoString();
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
            //UITrigger.UpdateInfoString();
        }
        
        private void Button_AddCondition_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UISystem.WinBase targetControl = null;
            PropertyDescriptor targetProperty = null;
            object targetValue = null;
            if (Program.mSelectionWinControlsCollection.Count > 0)
            {
                targetControl = Program.mSelectionWinControlsCollection[0];
                targetProperty = TypeDescriptor.GetProperties(targetControl)[0];
                targetValue = targetProperty.GetValue(targetControl);
            }
            UISystem.Trigger.PropertyTriggerConditon condition = new UISystem.Trigger.PropertyTriggerConditon(targetControl, targetProperty, UISystem.Trigger.PropertyTriggerConditon.enValueOperator.Equal, targetValue);
            UITrigger.AddCondition(condition);
            //UITrigger.UpdateInfoString();

            //AddCondition(condition);
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
