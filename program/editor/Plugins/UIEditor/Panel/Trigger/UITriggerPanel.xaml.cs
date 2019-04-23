using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UIEditor.Panel.Trigger
{
    /// <summary>
    /// Interaction logic for UITriggerPanel.xaml
    /// </summary>
    public partial class UITriggerPanel : UserControl
    {
        UISystem.WinBase mHostForm;
        public UISystem.WinBase HostForm
        {
            get { return mHostForm; }
        }

        public UISystem.Trigger.UITriggerManager UITriggerManager
        {
            get
            {
                if (mHostForm is UISystem.WinForm)
                    return ((UISystem.WinForm)mHostForm).UITriggerManager;
                else if (mHostForm is UISystem.Template.ControlTemplate)
                    return ((UISystem.Template.ControlTemplate)mHostForm).UITriggerManager;

                return null;
            }
        }

        private bool mDisableSetDefaultValue = false;

        public UITriggerPanel()
        {
            InitializeComponent();

            TriggerPanelListBoxItem item = new TriggerPanelListBoxItem(null, this);
            ListBox_Triggers.Items.Add(item);
        }

        // 添加事件触发器
        private void Button_AddEvent_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UISystem.Trigger.EventTrigger eventTrigger = new UISystem.Trigger.EventTrigger(UITriggerManager);
            UITriggerManager.AddEventTrigger(eventTrigger);
            TriggerPanelListBoxItem item = new TriggerPanelListBoxItem(eventTrigger, this);
            ListBox_Triggers.Items.Add(item);
            ListBox_Triggers.SelectedItem = item;
        }

        // 添加属性触发器
        private void Button_AddProperty_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UISystem.Trigger.PropertyTrigger propertyTrigger = new UISystem.Trigger.PropertyTrigger(UITriggerManager);
            UITriggerManager.AddPropertyTrigger(propertyTrigger);
            TriggerPanelListBoxItem item = new TriggerPanelListBoxItem(propertyTrigger, this);
            ListBox_Triggers.Items.Add(item);
            ListBox_Triggers.SelectedItem = item;
        }

        // 移除触发器
        private void Button_RemoveTrigger_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ListBox_Triggers.SelectedIndex < 1)
                return;

            TriggerPanelListBoxItem item = ListBox_Triggers.SelectedItem as TriggerPanelListBoxItem;
            ListBox_Triggers.Items.Remove(item);

            UITriggerManager.RemoveTrigger(item.UITrigger);
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            mDisableSetDefaultValue = true;

            foreach (var item in e.RemovedItems)
            {
                ((TriggerPanelListBoxItem)item).IsSelected = false;
            }
            foreach (var item in e.AddedItems)
            {
                ((TriggerPanelListBoxItem)item).IsSelected = true;
            }

            if (ListBox_Triggers.SelectedItem != null)
            {
                TriggerPanelListBoxItem item = ListBox_Triggers.SelectedItem as TriggerPanelListBoxItem;
                if (item.UITrigger == null)
                {
                    PropertyTriggerPanel.Visibility = System.Windows.Visibility.Hidden;
                    EventTriggerPanel.Visibility = System.Windows.Visibility.Hidden;

                    // 默认值
                    UITriggerManager.SetToDefaultProperty();
                }
                else if (item.UITrigger is UISystem.Trigger.PropertyTrigger)
                {
                    PropertyTriggerPanel.Visibility = System.Windows.Visibility.Visible;
                    EventTriggerPanel.Visibility = System.Windows.Visibility.Hidden;

                    PropertyTriggerPanel.UITrigger = item.UITrigger as UISystem.Trigger.PropertyTrigger;
                    item.UITrigger.SetToTriggerActionProperty();
                }
                else if (item.UITrigger is UISystem.Trigger.EventTrigger)
                {
                    PropertyTriggerPanel.Visibility = System.Windows.Visibility.Hidden;
                    EventTriggerPanel.Visibility = System.Windows.Visibility.Visible;

                    EventTriggerPanel.UITrigger = item.UITrigger as UISystem.Trigger.EventTrigger;
                    item.UITrigger.SetToTriggerActionProperty();
                }
                else
                {
                    PropertyTriggerPanel.Visibility = Visibility.Hidden;
                    EventTriggerPanel.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                PropertyTriggerPanel.Visibility = System.Windows.Visibility.Hidden;
                EventTriggerPanel.Visibility = System.Windows.Visibility.Hidden;
            }

            mDisableSetDefaultValue = false;
        }

        public void SetRootForm(UISystem.WinBase form)
        {
            mHostForm = form;

            ListBox_Triggers.Items.Clear();

            var item = new TriggerPanelListBoxItem(null, this);
            ListBox_Triggers.Items.Add(item);

            if (UITriggerManager != null)
            {
                UITriggerManager.EnableAction = false;

                foreach (var trigger in UITriggerManager.PropertyTriggerList)
                {
                    var tItem = new TriggerPanelListBoxItem(trigger, this);
                    ListBox_Triggers.Items.Add(tItem);
                }

                foreach (var trigger in UITriggerManager.EventTriggerList)
                {
                    var tItem = new TriggerPanelListBoxItem(trigger, this);
                    ListBox_Triggers.Items.Add(tItem);
                }
            }

            // 取得窗口中的Trigger并判断有效性
        }

        public void OnDeleteControls(List<UISystem.WinBase> controls)
        {
            foreach (TriggerPanelListBoxItem item in ListBox_Triggers.Items)
            {
                item.UpdateEnable();
            }
        }

        //public void OnSelectedWinControlsCollectionChanged(ObservableCollection<UISystem.WinBase> collection)
        //{
        //    if(PropertyTriggerPanel.Visibility == System.Windows.Visibility.Visible)
        //        PropertyTriggerPanel.OnSelectedWinControlsCollectionChanged(collection);
        //}

        // 控件属性改变时调用此接口
        public void OnWinControlPropertyChanged(UISystem.WinBase control, string propertyName)
        {
            var property = TypeDescriptor.GetProperties(control)[propertyName];
            if (property == null)
                return;

            if (property.IsReadOnly || !property.IsBrowsable)
                return;

            bool isRecording = false;
            foreach (TriggerPanelListBoxItem item in ListBox_Triggers.Items)
            {
                if (item.IsRecording)
                {
                    if (item.UITrigger is UISystem.Trigger.PropertyTrigger)
                    {
                        var propertyTrigger = item.UITrigger as UISystem.Trigger.PropertyTrigger;

                        // 判断Trigger中有没有此Action，没有的话记录默认值并加入Action，有的话更新Action属性值
                        var propertyAction = propertyTrigger.GetPropertyAction(control.WinName, propertyName);
                        var propertyValue = property.GetValue(control);
                        if (propertyAction == null)
                        {
                            UITriggerManager.SetDefaultProperty(control, property, control.GetPropertyOldValue(propertyName));

                            propertyAction = new UISystem.Trigger.TriggerAction_Property(control, property, propertyValue);
                            propertyTrigger.AddAction(propertyAction);
                        }
                        else
                        {
                            propertyAction.TargetValue = propertyValue;
                        }
                    }
                    else if (item.UITrigger is UISystem.Trigger.EventTrigger)
                    {
                        var eventTrigger = item.UITrigger as UISystem.Trigger.EventTrigger;

                        // 判断Trigger中有没有此Action，没有的话记录默认值并加入Action，有的话更新Action属性值
                        var propertyAction = eventTrigger.GetPropertyAction(control.WinName, propertyName);
                        var propertyValue = property.GetValue(control);
                        if (propertyAction == null)
                        {
                            UITriggerManager.SetDefaultProperty(control, property, control.GetPropertyOldValue(propertyName));

                            propertyAction = new UISystem.Trigger.TriggerAction_Property(control, property, propertyValue);
                            eventTrigger.AddAction(propertyAction);
                        }
                        else
                        {
                            propertyAction.TargetValue = propertyValue;
                        }
                    }
                    else
                    {

                    }

                    isRecording = true;
                }
            }

            if (!isRecording && !mDisableSetDefaultValue)
            {
                // 更新默认值
                var defaultAct = UITriggerManager.GetDefaultProperty(control.WinName, property.Name);
                if (defaultAct != null)
                {
                    defaultAct.TargetValue = property.GetValue(control);
                }
            }
        }
    }
}
