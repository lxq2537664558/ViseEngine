using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace UIEditor.Panel.Trigger
{
    /// <summary>
    /// Interaction logic for PropertyTriggerConditionItem.xaml
    /// </summary>
    public partial class PropertyTriggerConditionItem : UserControl
    {
        //public delegate void Delegate_UpdateTriggerInfo();
        //public Delegate_UpdateTriggerInfo OnUpdateTriggerInfo;

        public delegate void Delegate_OnRemove(PropertyTriggerConditionItem item);
        public Delegate_OnRemove OnRemove;

        // Trigger条件
        UISystem.Trigger.PropertyTriggerConditon mCondition;
        public UISystem.Trigger.PropertyTriggerConditon Condition
        {
            get { return mCondition; }
        }

        // Trigger条件对应的窗体控件
        UISystem.WinBase mWinControl;
        public UISystem.WinBase WinControl
        {
            get{ return mWinControl; }
            set
            {
                if (mWinControl != null)
                {
                    mWinControl.OnPropertyChangedEvent -= WinControl_OnPropertyChangedEvent;
                }

                if (value is UISystem.Template.ControlTemplate)
                {
                    mWinControl = ((UISystem.Template.ControlTemplate)value).TargetControl;
                }
                else
                {
                    mWinControl = value;
                }

                mWinControl.OnPropertyChangedEvent += WinControl_OnPropertyChangedEvent;

                BindingOperations.ClearBinding(TextBlock_ControlName, TextBlock.TextProperty);
                TextBlock_ControlName.SetBinding(TextBlock.TextProperty,
                                            new Binding("WinName")
                                            { Source = mWinControl, 
                                              Mode = BindingMode.TwoWay, 
                                              UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                //if (string.IsNullOrEmpty(mWinControl.WinName))
                //    mWinControl.WinName = mWinControl.GetType().Name;

                //mCondition.TargetName = mWinControl.WinName;
                mCondition.TargetControl = mWinControl;

                UpdateWinControlPropertys();

                //if (OnUpdateTriggerInfo != null)
                //    OnUpdateTriggerInfo();
            }
        }

        void WinControl_OnPropertyChangedEvent(UISystem.WinBase control, string propertyName)
        {
            switch (propertyName)
            {
                case "WinName":
                    {
                        Condition.UpdateConditionInfoString();
                    }
                    break;
            }
        }

        public bool SetWinControlMode
        {
            get { return (bool)GetValue(SetWinControlModeProperty); }
            set
            {
                SetValue(SetWinControlModeProperty, value);
            }
        }

        public static readonly DependencyProperty SetWinControlModeProperty =
            DependencyProperty.Register("SetWinControlMode", typeof(bool), typeof(PropertyTriggerConditionItem), 
                        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSetWinControlModeChanged)));
        public static void OnSetWinControlModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyTriggerConditionItem item = d as PropertyTriggerConditionItem;

            bool newValue = (bool)e.NewValue;
            if(newValue)
            {
                item.Border_SetWinControlMode.Visibility = Visibility.Visible;
            }
            else
            {
                item.Border_SetWinControlMode.Visibility = Visibility.Hidden;
            }
        }

        public PropertyTriggerConditionItem(UISystem.Trigger.PropertyTriggerConditon condition)
        {
            InitializeComponent();

            mCondition = condition;

            ComboBox_ValueOperate.Items.Clear();
            foreach (UISystem.Trigger.PropertyTriggerConditon.enValueOperator enumValue in System.Enum.GetValues(typeof(UISystem.Trigger.PropertyTriggerConditon.enValueOperator)))
            {
                ComboBox_ValueOperate.Items.Add(UISystem.Trigger.PropertyTriggerConditon.GetOperationString(enumValue));
            }

            ComboBox_ValueOperate_EqualOrNot.Items.Clear();
            ComboBox_ValueOperate_EqualOrNot.Items.Add(UISystem.Trigger.PropertyTriggerConditon.GetOperationString(UISystem.Trigger.PropertyTriggerConditon.enValueOperator.Equal));
            ComboBox_ValueOperate_EqualOrNot.Items.Add(UISystem.Trigger.PropertyTriggerConditon.GetOperationString(UISystem.Trigger.PropertyTriggerConditon.enValueOperator.NotEqual));

            if(mCondition != null && mCondition.TargetControl != null)
            {
                mWinControl = mCondition.TargetControl;
                mWinControl.OnPropertyChangedEvent += WinControl_OnPropertyChangedEvent;

                BindingOperations.ClearBinding(TextBlock_ControlName, TextBlock.TextProperty);
                TextBlock_ControlName.SetBinding(TextBlock.TextProperty,
                                            new Binding("WinName")
                                            {
                                                Source = mWinControl,
                                                Mode = BindingMode.TwoWay,
                                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                            });
                if (string.IsNullOrEmpty(mWinControl.WinName))
                    mWinControl.WinName = mWinControl.GetType().Name;

                ComboBox_Property.Items.Clear();
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(mWinControl))
                {
                    BrowsableAttribute att = property.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
                    if (att != null && att.Browsable == false)
                        continue;

                    ComboBox_Property.Items.Add(property.Name);
                }
                ComboBox_Property.SelectedItem = mCondition.TargetPropertyName;
                if (mCondition.TargetValue != null)
                    SetValueShow(mCondition.TargetValue.GetType(), mCondition.TargetValue);

                //UpdateOperateEditVisible(mCondition.TargetProperty.PropertyType);

                if (ComboBox_ValueOperate.Visibility == System.Windows.Visibility.Visible)
                    ComboBox_ValueOperate.SelectedIndex = (int)mCondition.ValueOperator;
                else if (ComboBox_ValueOperate_EqualOrNot.Visibility == System.Windows.Visibility.Visible)
                {
                    switch (mCondition.ValueOperator)
                    {
                        case UISystem.Trigger.PropertyTriggerConditon.enValueOperator.Equal:
                            ComboBox_ValueOperate_EqualOrNot.SelectedIndex = 0;
                            break;
                        case UISystem.Trigger.PropertyTriggerConditon.enValueOperator.NotEqual:
                            ComboBox_ValueOperate_EqualOrNot.SelectedIndex = 1;
                            break;
                    }
                }
            }
        }

        private void Button_Remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OnRemove != null)
                OnRemove(this);
        }

        private void UpdateWinControlPropertys()
        {
            ComboBox_Property.Items.Clear();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(mWinControl))
            {
                BrowsableAttribute att = property.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
                if (att != null && att.Browsable == false)
                    continue;

                ComboBox_Property.Items.Add(property.Name);
            }

            if (ComboBox_Property.Items.Count > 0)
            {
                ComboBox_Property.SelectedIndex = 0;
            }
        }

        // Property
        private void ComboBox_Property_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Property.SelectedIndex < 0)
            {
                return;
            }

            PropertyDescriptor property = TypeDescriptor.GetProperties(mWinControl)[ComboBox_Property.SelectedItem.ToString()];
            mCondition.TargetProperty = property;
            //SetValueShow(property.PropertyType, property.GetValue(mWinControl));
            UpdateValueEditVisible(property.PropertyType);
            UpdateOperateEditVisible(property.PropertyType);
            if (mCondition.TargetValue != null && mCondition.TargetValue.GetType() != property.PropertyType)
                SetValueShow(property.PropertyType, property.GetValue(mWinControl));
            //if (OnUpdateTriggerInfo != null)
            //    OnUpdateTriggerInfo();
        }

        private void UpdateValueEditVisible(Type type)
        {
            if (type.IsEnum)
            {
                ComboBox_Value.Visibility = Visibility.Visible;
                TextBox_Value.Visibility = Visibility.Collapsed;

                ComboBox_Value.Items.Clear();
                var enumNames = System.Enum.GetNames(type);
                for (int i = 0; i < enumNames.Length; i++)
                {
                    ComboBox_Value.Items.Add(enumNames[i]);
                }
            }
            else if (type == typeof(bool))
            {
                ComboBox_Value.Visibility = Visibility.Visible;
                TextBox_Value.Visibility = Visibility.Collapsed;

                ComboBox_Value.Items.Clear();
                ComboBox_Value.Items.Add("True");
                ComboBox_Value.Items.Add("False");
            }
            else
            {
                ComboBox_Value.Visibility = Visibility.Collapsed;
                TextBox_Value.Visibility = Visibility.Visible;
            }
        }

        private void UpdateOperateEditVisible(Type type)
        {
            if (type == typeof(SByte) ||
                type == typeof(Int16) ||
                type == typeof(Int32) ||
                type == typeof(Int64) ||
                type == typeof(Byte) ||
                type == typeof(UInt16) ||
                type == typeof(UInt32) ||
                type == typeof(UInt64) ||
                type == typeof(Single) ||
                type == typeof(Double))
            {
                ComboBox_ValueOperate_EqualOrNot.Visibility = System.Windows.Visibility.Collapsed;
                ComboBox_ValueOperate.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ComboBox_ValueOperate_EqualOrNot.Visibility = System.Windows.Visibility.Visible;
                ComboBox_ValueOperate.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void SetValueShow(Type propertyType, object value)
        {
            if (propertyType.IsEnum)
            {
                //ComboBox_Value.Items.Clear();
                
                var valueName = value.ToString();

                ComboBox_Value.SelectedItem = valueName;

            }
            else if (propertyType == typeof(bool))
            {
                //ComboBox_Value.Items.Clear();

                bool proValue = (bool)value;
                if (proValue)
                    ComboBox_Value.SelectedIndex = 0;
                else
                    ComboBox_Value.SelectedIndex = 1;

            }
            else
            {
                //if(propertyType.isn)

                if (value == null)
                    TextBox_Value.Text = "";
                else
                    TextBox_Value.Text = value.ToString();
            }
        }

        // Value
        private void ComboBox_Value_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Property.SelectedIndex < 0)
                return;
            if (ComboBox_Value.SelectedIndex < 0)
                return;

            PropertyDescriptor property = TypeDescriptor.GetProperties(mWinControl)[ComboBox_Property.SelectedItem.ToString()];
            if (property.PropertyType.IsEnum)
            {
                mCondition.TargetValue = System.Enum.Parse(property.PropertyType, ComboBox_Value.SelectedItem.ToString());
            }
            else if (property.PropertyType == typeof(bool))
            {
                mCondition.TargetValue = System.Convert.ToBoolean(ComboBox_Value.SelectedItem.ToString());
            }

            //if (OnUpdateTriggerInfo != null)
            //    OnUpdateTriggerInfo();
        }

        private void TextBox_Value_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (ComboBox_Property.SelectedIndex < 0)
                return;
            PropertyDescriptor property = TypeDescriptor.GetProperties(mWinControl)[ComboBox_Property.SelectedItem.ToString()];

            mCondition.TargetValue = UISystem.Assist.GetValueWithType(property.PropertyType, TextBox_Value.Text);

            //if (OnUpdateTriggerInfo != null)
            //    OnUpdateTriggerInfo();
        }

        private void Button_SetWinControl_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Program.mSelectionWinControlsCollection.Count <= 0)
                return;

            var targetControl = Program.mSelectionWinControlsCollection[0];
            if (targetControl is UISystem.Template.ControlTemplate)
            {
                WinControl = ((UISystem.Template.ControlTemplate)(targetControl)).TargetControl;
            }
            else
                WinControl = targetControl;
            //if (string.IsNullOrEmpty(WinControl.WinName))
            //    WinControl.WinName = WinControl.GetType().Name;
        }

        private void ComboBox_ValueOperate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(ComboBox_ValueOperate.SelectedIndex < 0)
                return;

            mCondition.ValueOperator = (UISystem.Trigger.PropertyTriggerConditon.enValueOperator)ComboBox_ValueOperate.SelectedIndex;
        }

        private void ComboBox_ValueOperate_EqualOrNot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_ValueOperate_EqualOrNot.SelectedIndex < 0)
                return;

            switch (ComboBox_ValueOperate_EqualOrNot.SelectedIndex)
            {
                case 0:
                    mCondition.ValueOperator = UISystem.Trigger.PropertyTriggerConditon.enValueOperator.Equal;
                    break;

                case 1:
                    mCondition.ValueOperator = UISystem.Trigger.PropertyTriggerConditon.enValueOperator.NotEqual;
                    break;
            }
        }
    }
}
