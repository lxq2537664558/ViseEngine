using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UIEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for UIControlTypesSelector.xaml
    /// </summary>
    public partial class UIControlTypesSelector : UserControl
    {
        public object BindInstance
        {
            get { return (object)GetValue(BindInstanceProperty); }
            set { SetValue(BindInstanceProperty, value); }
        }
        public static readonly DependencyProperty BindInstanceProperty =
                            DependencyProperty.Register("BindInstance", typeof(object), typeof(UIControlTypesSelector),
                            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBindInstanceChanged)));
        public static void OnBindInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public PropertyDescriptor BindProperty
        {
            get { return (PropertyDescriptor)GetValue(BindPropertyProperty); }
            set { SetValue(BindPropertyProperty, value); }
        }
        public static readonly DependencyProperty BindPropertyProperty =
                            DependencyProperty.Register("BindProperty", typeof(PropertyDescriptor), typeof(UIControlTypesSelector), new UIPropertyMetadata(null));

        public Type ControlType
        {
            get { return (Type)GetValue(ControlTypeProperty); }
            set { SetValue(ControlTypeProperty, value); }
        }
        public static readonly DependencyProperty ControlTypeProperty =
                            DependencyProperty.Register("ControlType", typeof(Type), typeof(UIControlTypesSelector),
                            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnControlTypeChanged)));

        public static void OnControlTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIControlTypesSelector control = d as UIControlTypesSelector;

            Type newValue = (Type)e.NewValue;
            control.ComboBox_Types.SelectedValue = newValue;
        }

        public UIControlTypesSelector()
        {
            InitializeComponent();

            //var assembly = CSUtility.Program.GetAssemblyFromDllFileName("UISystem.dll");
            //if (assembly != null)
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var assembly in assemblys)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute), true);
                    if (attributes.Length <= 0)
                        continue;

                    ComboBox_Types.Items.Add(type);
                }                
            }
        }

        private void ComboBox_Types_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;

            if (ControlType != (Type)e.AddedItems[0])
                ControlType = (Type)e.AddedItems[0];
            //if (ControlType != (Type)ComboBox_Types.SelectedValue)
            //    ControlType = (Type)ComboBox_Types.SelectedValue;
        }
    }
}
