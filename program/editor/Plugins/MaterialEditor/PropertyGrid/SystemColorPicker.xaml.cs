using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MaterialEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for SystemColorPicker.xaml
    /// </summary>
    public partial class SystemColorPicker : UserControl
    {
        CCore.Material.MaterialShaderVarInfo mMatShaderVarInfo;
        MaterialShaderValueControl mParentControl;

        public int nIdx = 0;

        public CSUtility.Support.Color Color
        {
            get { return (CSUtility.Support.Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(CSUtility.Support.Color), typeof(SystemColorPicker),
            new FrameworkPropertyMetadata(CSUtility.Support.Color.White, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnColorChanged)));

        public static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SystemColorPicker control = d as SystemColorPicker;

            var newColor = (CSUtility.Support.Color)e.NewValue;
            var oldColor = (CSUtility.Support.Color)e.OldValue;
            if (newColor.Equals(oldColor))
                return;

            if(newColor.R == control.EditColor.R &&
               newColor.G == control.EditColor.G &&
               newColor.B == control.EditColor.B &&
               newColor.A == control.EditColor.A)
            {

            }
            else
            {
                control.EditColor = System.Windows.Media.Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B);
                control.Brush = new SolidColorBrush(control.EditColor);
                var valueStr = newColor.R / 255.0f + "," + newColor.G / 255.0f + "," + newColor.B / 255.0f + "," + newColor.A / 255.0f;
                if(!control.mMatShaderVarInfo.VarValue.Equals(valueStr))
                    control.mMatShaderVarInfo.VarValue = valueStr;
            }
        }

        public Color EditColor
        {
            get { return (Color)GetValue(EditColorProperty); }
            set { SetValue(EditColorProperty, value); }
        }
        public static readonly DependencyProperty EditColorProperty =
            DependencyProperty.Register("EditColor", typeof(Color), typeof(SystemColorPicker),
            new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnEditColorChanged)));

        public static void OnEditColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SystemColorPicker control = d as SystemColorPicker;

            var newColor = (Color)e.NewValue;
            var oldColor = (Color)e.OldValue;
            if (newColor.Equals(oldColor))
                return;

            if(newColor.R == control.Color.R &&
               newColor.G == control.Color.G &&
               newColor.B == control.Color.B &&
               newColor.A == control.Color.A)
            {

            }
            else
            {
                control.Color = CSUtility.Support.Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B);
                control.Brush = new SolidColorBrush(newColor);
                var valueStr = newColor.R / 255.0f + "," + newColor.G / 255.0f + "," + newColor.B / 255.0f + "," + newColor.A / 255.0f;
                if(!control.mMatShaderVarInfo.VarValue.Equals(valueStr))
                    control.mMatShaderVarInfo.VarValue = valueStr;
            }
        }

        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(Brush), typeof(SystemColorPicker), new UIPropertyMetadata(Brushes.AliceBlue));

        public SystemColorPicker()
        {
            InitializeComponent();
        }

        public SystemColorPicker(CCore.Material.MaterialShaderVarInfo info, MaterialShaderValueControl parentControl)
        {
            InitializeComponent();

            mMatShaderVarInfo = info;
            mParentControl = parentControl;
            var splits = info.VarValue.Split(',');
            if (splits.Length == 4)
            {
                var r = (int)(System.Convert.ToSingle(splits[0]) * 255);
                var g = (int)(System.Convert.ToSingle(splits[1]) * 255);
                var b = (int)(System.Convert.ToSingle(splits[2]) * 255);
                var a = (int)(System.Convert.ToSingle(splits[3]) * 255);
                Color = CSUtility.Support.Color.FromArgb(a, r, g, b);
            }

            Label_Name.SetBinding(TextBlock.TextProperty, new Binding("NickName") { Source = info });
        }

        private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Popup_Edit.IsOpen = !Popup_Edit.IsOpen;
        }       
    }
}
