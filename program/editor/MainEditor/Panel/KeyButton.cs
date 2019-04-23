using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MainEditor.Panel
{

    public class KeyButton : ContentControl
    {
        public delegate void Delegate_ButtonStateChanged(enKeyButtonState state, KeyButton keyButton);
        public event Delegate_ButtonStateChanged OnButtonStateChanged;

        public enum enKeyButtonState
        {
            None,
            Occupancy,
            Selected,
            Setting,
            Normal,
        }

        public Brush OccupancyBackground
        {
            get { return (Brush)GetValue(OccupancyBackgroundProperty); }
            set { SetValue(OccupancyBackgroundProperty, value); }
        }
        public static readonly DependencyProperty OccupancyBackgroundProperty =
            DependencyProperty.Register("OccupancyBackground", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush OccupancyBorderBrush
        {
            get { return (Brush)GetValue(OccupancyBorderBrushProperty); }
            set { SetValue(OccupancyBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty OccupancyBorderBrushProperty =
            DependencyProperty.Register("OccupancyBorderBrush", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush OccupancyForeground
        {
            get { return (Brush)GetValue(OccupancyForegroundProperty); }
            set { SetValue(OccupancyForegroundProperty, value); }
        }
        public static readonly DependencyProperty OccupancyForegroundProperty =
            DependencyProperty.Register("OccupancyForeground", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush SelectedBorderBrush
        {
            get { return (Brush)GetValue(SelectedBorderBrushProperty); }
            set { SetValue(SelectedBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty SelectedBorderBrushProperty =
            DependencyProperty.Register("SelectedBorderBrush", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush AssinBackground
        {
            get { return (Brush)GetValue(AssinBackgroundProperty); }
            set { SetValue(AssinBackgroundProperty, value); }
        }
        public static readonly DependencyProperty AssinBackgroundProperty =
            DependencyProperty.Register("AssinBackground", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush AssinBorderBrush
        {
            get { return (Brush)GetValue(AssinBorderBrushProperty); }
            set { SetValue(AssinBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty AssinBorderBrushProperty =
            DependencyProperty.Register("AssinBorderBrush", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush AssinForeground
        {
            get { return (Brush)GetValue(AssinForegroundProperty); }
            set { SetValue(AssinForegroundProperty, value); }
        }
        public static readonly DependencyProperty AssinForegroundProperty =
            DependencyProperty.Register("AssinForeground", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush NormalBackground
        {
            get { return (Brush)GetValue(NormalBackgroundProperty); }
            set { SetValue(NormalBackgroundProperty, value); }
        }
        public static readonly DependencyProperty NormalBackgroundProperty =
            DependencyProperty.Register("NormalBackground", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush NormalBorderBrush
        {
            get { return (Brush)GetValue(NormalBorderBrushProperty); }
            set { SetValue(NormalBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty NormalBorderBrushProperty =
            DependencyProperty.Register("NormalBorderBrush", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));
        public Brush NormalForeground
        {
            get { return (Brush)GetValue(NormalForegroundProperty); }
            set { SetValue(NormalForegroundProperty, value); }
        }
        public static readonly DependencyProperty NormalForegroundProperty =
            DependencyProperty.Register("NormalForeground", typeof(Brush), typeof(KeyButton), new PropertyMetadata(null));

        public enKeyButtonState ButtonState
        {
            get { return (enKeyButtonState)GetValue(ButtonStateProperty); }
            set { SetValue(ButtonStateProperty, value); }
        }
        public static readonly DependencyProperty ButtonStateProperty =
            DependencyProperty.Register("ButtonState", typeof(enKeyButtonState), typeof(KeyButton), new FrameworkPropertyMetadata(enKeyButtonState.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(ButtonStateChanged)));
        public static void ButtonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as KeyButton;
            if (ctrl == null)
                return;

            var state = (enKeyButtonState)e.NewValue;
            switch (state)
            {
                case enKeyButtonState.Normal:
                    ctrl.Background = ctrl.NormalBackground;
                    ctrl.BorderBrush = ctrl.NormalBorderBrush;
                    ctrl.Foreground = ctrl.NormalForeground;
                    break;
                case enKeyButtonState.Occupancy:
                    ctrl.Background = ctrl.OccupancyBackground;
                    ctrl.BorderBrush = ctrl.OccupancyBorderBrush;
                    ctrl.Foreground = ctrl.OccupancyForeground;
                    break;
                case enKeyButtonState.Selected:
                    ctrl.Background = ctrl.SelectedBackground;
                    ctrl.BorderBrush = ctrl.SelectedBorderBrush;
                    ctrl.Foreground = ctrl.SelectedForeground;
                    break;
                case enKeyButtonState.Setting:
                    ctrl.Background = ctrl.AssinBackground;
                    ctrl.BorderBrush = ctrl.AssinBorderBrush;
                    ctrl.Foreground = ctrl.AssinForeground;
                    break;
            }

            ctrl?.OnButtonStateChanged?.Invoke(state, ctrl);
        }

        static KeyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyButton), new FrameworkPropertyMetadata(typeof(KeyButton)));
        }

        public KeyButton()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.MouseDown += KeyButton_MouseDown;
            this.MouseEnter += KeyButton_MouseEnter;
            this.MouseLeave += KeyButton_MouseLeave;
            //mD3DDrawPanel = (System.Windows.Forms.Panel)Template.FindName("PART_D3DDrawPanel", this);

        }

        private void KeyButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            switch (ButtonState)
            {
                case enKeyButtonState.Normal:
                    Background = NormalBackground;
                    BorderBrush = NormalBorderBrush;
                    Foreground = NormalForeground;
                    break;
                case enKeyButtonState.Occupancy:
                    Background = OccupancyBackground;
                    BorderBrush = OccupancyBorderBrush;
                    Foreground = OccupancyForeground;
                    break;
                case enKeyButtonState.Selected:
                    Background = SelectedBackground;
                    BorderBrush = SelectedBorderBrush;
                    Foreground = SelectedForeground;
                    break;
                case enKeyButtonState.Setting:
                    Background = AssinBackground;
                    BorderBrush = AssinBorderBrush;
                    Foreground = AssinForeground;
                    break;
            }
        }
        
        private void KeyButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            switch(ButtonState)
            {
                case enKeyButtonState.Normal:
                case enKeyButtonState.Setting:
                    Background = TryFindResource("Button.MouseOver.Background") as Brush;
                    BorderBrush = TryFindResource("Button.MouseOver.Border") as Brush;
                    break;
            }
        }

        private void KeyButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsEnabled)
                return;

            switch(ButtonState)
            {
                case enKeyButtonState.Normal:
                    ButtonState = enKeyButtonState.Setting;
                    break;
                case enKeyButtonState.Setting:
                    ButtonState = enKeyButtonState.Normal;
                    break;
            }
        }
    }
}
