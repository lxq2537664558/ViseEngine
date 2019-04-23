using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResourcesBrowser
{
    /// <summary>
    /// ResourceToolTip.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceToolTip : UserControl
    {
        public object DataType
        {
            get { return GetValue(DataTypeProperty); }
            set
            {
                SetValue(DataTypeProperty, value);
            }
        }

        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.Register("DataType", typeof(object), typeof(ResourceToolTip),
                                                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnDataTypeChanged)
                                        ));

        public static void OnDataTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue as ResourceInfo;
            if(newValue != null)
            {
                var control = d as ResourceToolTip;
                control.Image_Icon.SetBinding(Image.SourceProperty, new Binding("ResourceIcon") { Source = newValue });
                control.TextBlock_Name.SetBinding(TextBlock.TextProperty, new Binding("Name") { Source = newValue });

                control.StackPanel_Infos.Children.Clear();
                foreach (var property in newValue.GetType().GetProperties())
                {
                    var atts = property.GetCustomAttributes(typeof(ResourceToolTipAttribute), true);
                    if (atts.Length <= 0)
                        continue;

                    var propertyName = "";
                    atts = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    if (atts.Length > 0)
                        propertyName = ((DisplayNameAttribute)atts[0]).DisplayName;
                    else
                        propertyName = property.Name;
                    
                    var stackPanel = new StackPanel()
                    {
                        Margin = new Thickness(3),
                        Orientation = Orientation.Horizontal,
                    };
                    var textName = new TextBlock()
                    {
                        Text = propertyName + ": ",
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.Gray,
                        Style = control.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TextBlockStyle_Default")) as Style
                    };
                    stackPanel.Children.Add(textName);
                    var textValue = new TextBlock()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Style = control.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TextBlockStyle_Default")) as Style
                    };
                    textValue.SetBinding(TextBlock.TextProperty, new Binding(property.Name) { Source = newValue });
                    stackPanel.Children.Add(textValue);

                    control.StackPanel_Infos.Children.Add(stackPanel);
                }
            }
        }

        public ResourceToolTip()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
