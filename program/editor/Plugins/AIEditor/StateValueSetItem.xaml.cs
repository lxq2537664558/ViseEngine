using System.Windows.Controls;
using System.Windows.Data;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for StateValueSetItem.xaml
    /// </summary>
    public partial class StateValueSetItem : UserControl
    {
        //CSUtility.Helper.enCSType mCSType;

        AIEditor.FSMTemplateInfo.StatePropertyInfo mInfo;
        public AIEditor.FSMTemplateInfo.StatePropertyInfo Info
        {
            get { return mInfo; }
            protected set
            {
                mInfo = value;
            }
        }

        public StateValueSetItem(AIEditor.FSMTemplateInfo.StatePropertyInfo info)
        {
            InitializeComponent();

            Info = info;

            foreach (var value in AIEditor.FSMTemplateInfo.StatePropertyInfo.mCommonTypes)
            {
                ComboBox_Types.Items.Add(value);
            }

            var attClassFullName = typeof(CSUtility.AISystem.Attribute.AllowStateProperty).FullName;
            foreach (var type in CSUtility.Program.GetTypes(attClassFullName, true))//Program.GetTypes(Info.CSType))
            {
                var csTypeStr = (string)(CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(type, attClassFullName, "CSType", false)).ToString();

                switch (Info.CSType)
                {
                    case CSUtility.Helper.enCSType.Common:
                        {
                            if (csTypeStr.Equals(Info.CSType.ToString()))
                            {
                                ComboBox_Types.Items.Add(type);
                            }
                        }
                        break;

                    case CSUtility.Helper.enCSType.Client:
                        {
                            if(!csTypeStr.Equals(CSUtility.Helper.enCSType.Server.ToString()))
                                ComboBox_Types.Items.Add(type);
                        }
                        break;

                    case CSUtility.Helper.enCSType.Server:
                        {
                            if(!csTypeStr.Equals(CSUtility.Helper.enCSType.Client.ToString()))
                                ComboBox_Types.Items.Add(type);
                        }
                        break;
                }
            }

            //BindingOperations.ClearBinding(ComboBox_Types, ComboBox.SelectedItemProperty);
            //ComboBox_Types.SetBinding(ComboBox.SelectedItemProperty, new Binding("StatePropertyTypeEnum") { Source = mInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            ComboBox_Types.SelectedValue = Info.PropertyType;

            BindingOperations.ClearBinding(TextBox_Name, TextBox.TextProperty);
            TextBox_Name.SetBinding(TextBox.TextProperty, new Binding("PropertyName") { Source = mInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            BindingOperations.ClearBinding(TextBox_DefaultValue, TextBox.TextProperty);
            TextBox_DefaultValue.SetBinding(TextBox.TextProperty, new Binding("DefaultValue") { Source = mInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

        }

        private void ComboBox_Types_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Info == null)
                return;

            if (ComboBox_Types.SelectedIndex < 0)
                return;

            if (ComboBox_Types.SelectedIndex >= ComboBox_Types.Items.Count)
                return;

            Info.PropertyType = ComboBox_Types.Items[ComboBox_Types.SelectedIndex] as System.Type;
        }
    }
}
