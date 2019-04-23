using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UIEditor
{
    /// <summary>
    /// GridLengthAssistControl.xaml 的交互逻辑
    /// </summary>
    public partial class GridLengthAssistControl : UserControl
    {
        public float LengthValue
        {
            get { return (float)GetValue(LengthValueProperty); }
            set { SetValue(LengthValueProperty, value); }
        }

        public static readonly DependencyProperty LengthValueProperty =
            DependencyProperty.Register("LengthValue", typeof(float), typeof(GridLengthAssistControl),
                                    new FrameworkPropertyMetadata(0.0f, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnLengthValueChanged))
                                    );
        public static void OnLengthValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLengthAssistControl;
            control.UpdateShow();

            float newValue = (float)e.NewValue;

            control.mLinkedDefinition.UserSize.Length = newValue;
            ((UISystem.Container.Grid.Grid)control.mLinkedDefinition.Parent).CellsStructureDirty = true;
            ((UISystem.Container.Grid.Grid)control.mLinkedDefinition.Parent).UpdateLayout();
        }

        public UISystem.Container.Grid.GridUnitType UnityType
        {
            get { return (UISystem.Container.Grid.GridUnitType)GetValue(UnityTypeProperty); }
            set { SetValue(UnityTypeProperty, value); }
        }

        public static readonly DependencyProperty UnityTypeProperty =
            DependencyProperty.Register("UnityType", typeof(UISystem.Container.Grid.GridUnitType), typeof(GridLengthAssistControl), 
                                    new FrameworkPropertyMetadata(UISystem.Container.Grid.GridUnitType.Star, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnUnityTypeChanged)));
        public static void OnUnityTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLengthAssistControl;
            control.UpdateShow();
            
            if (control.ComboBox_GridUnitType.SelectedItem.ToString() != e.NewValue.ToString())
                control.ComboBox_GridUnitType.SelectedItem = e.NewValue.ToString();

            //if(object.Equals(control.ComboBox_GridUnitType.SelectedValue, e.NewValue))
            //    control.ComboBox_GridUnitType.SelectedValue = e.NewValue;

            if (control.mLinkedDefinition.UserSize.GridUnitType != (UISystem.Container.Grid.GridUnitType)e.NewValue)
            {
                control.mLinkedDefinition.UserSize.GridUnitType = (UISystem.Container.Grid.GridUnitType)e.NewValue;
                ((UISystem.Container.Grid.Grid)control.mLinkedDefinition.Parent).CellsStructureDirty = true;
                ((UISystem.Container.Grid.Grid)control.mLinkedDefinition.Parent).UpdateLayout();

                control.UpdateAssistGridDefinitionShow();
            }
        }

        public float MaxValue
        {
            get { return (float)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(float), typeof(GridLengthAssistControl),
                                            new FrameworkPropertyMetadata(float.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMaxValueChanged)));

        public static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLengthAssistControl;
            control.UpdateShow();

            float newValue = (float)e.NewValue;

            if (System.Math.Abs(control.mLinkedDefinition.UserSize.MaxLength - newValue) > 0.00001f)
            {
                control.mLinkedDefinition.UserSize.MaxLength = newValue;
                ((UISystem.Container.Grid.Grid)control.mLinkedDefinition.Parent).CellsStructureDirty = true;
                ((UISystem.Container.Grid.Grid)control.mLinkedDefinition.Parent).UpdateLayout();

                control.UpdateAssistGridDefinitionShow();
            }
        }

        public float MinValue
        {
            get { return (float)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(float), typeof(GridLengthAssistControl),
                                            new FrameworkPropertyMetadata(0.0f, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMinValueChanged)));

        public static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GridLengthAssistControl;
            control.UpdateShow();

            float newValue = (float)e.NewValue;

            if (System.Math.Abs(control.mLinkedDefinition.UserSize.MinLength - newValue) > 0.00001f)
            {
                control.mLinkedDefinition.UserSize.MinLength = newValue;
                ((UISystem.Container.Grid.Grid)control.mLinkedDefinition.Parent).CellsStructureDirty = true;
                ((UISystem.Container.Grid.Grid)control.mLinkedDefinition.Parent).UpdateLayout();

                control.UpdateAssistGridDefinitionShow();
            }
        }

        public enum enType
        {
            Column,
            Row,
        }
        enType mType;

        private void UpdateShow()
        {
            TextBlock_Show.Text = mLinkedDefinition.UserSize.ToString();
        }

        Guid mId = Guid.NewGuid();

        DrawPanel mHostDrawPanel;
        UISystem.Container.Grid.DefinitionBase mLinkedDefinition;
        public GridLengthAssistControl(DrawPanel hostDrawPanel, UISystem.Container.Grid.DefinitionBase def, enType type)
        {
            InitializeComponent();

            NTE_Value.OnValueManualChanged += UpdateAssistGridDefinitionShow;

            mHostDrawPanel = hostDrawPanel;
            mLinkedDefinition = def;
            if (mLinkedDefinition != null)
            {
                this.SetBinding(GridLengthAssistControl.LengthValueProperty, new Binding("Length") { Source = mLinkedDefinition.UserSize, Mode = BindingMode.TwoWay });
                this.SetBinding(GridLengthAssistControl.UnityTypeProperty, new Binding("GridUnitType") { Source = mLinkedDefinition.UserSize, Mode = BindingMode.TwoWay });
            }

            mType = type;
            switch(mType)
            {
                case enType.Column:
                    {
                        TransRotate.Angle = 0;

                        HorizontalAlignment = HorizontalAlignment.Center;
                        VerticalAlignment = VerticalAlignment.Bottom;
                        Margin = new Thickness(-200, 0, -200, -40);
                    }
                    break;
                case enType.Row:
                    {
                        TransRotate.Angle = 0;// -90;

                        HorizontalAlignment = HorizontalAlignment.Right;
                        VerticalAlignment = VerticalAlignment.Center;
                        Margin = new Thickness(0, -200, -80, -200);
                    }
                    break;
            }
        }

        private void UpdateAssistGridDefinitionShow()
        {
            switch(mType)
            {
                case enType.Column:
                    {
                        var idx = mHostDrawPanel.GridAssistTarget.ColumnDefinitions.IndexOf(mLinkedDefinition);
                        mHostDrawPanel.UpdateGridAssistColmunData(idx);
                    }
                    break;
                case enType.Row:
                    {
                        var idx = mHostDrawPanel.GridAssistTarget.RowDefinitions.IndexOf(mLinkedDefinition);
                        mHostDrawPanel.UpdateGridAssistRowData(idx);
                    }
                    break;
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (mHostDrawPanel == null || mHostDrawPanel.GridAssistTarget == null)
                return;

            switch (mType)
            {
                case enType.Column:
                    {
                        if (mHostDrawPanel.GridAssistTarget.ColumnDefinitions.Count == 2)
                        {
                            mHostDrawPanel.GridAssistTarget.ColumnDefinitions.Clear();
                        }
                        else
                        {
                            mHostDrawPanel.GridAssistTarget.ColumnDefinitions.Remove(mLinkedDefinition);
                        }
                        mHostDrawPanel.GridAssistTarget.ColumnDefinitions = new CSUtility.Support.ThreadSafeObservableCollection<UISystem.Container.Grid.DefinitionBase>(mHostDrawPanel.GridAssistTarget.ColumnDefinitions);
                    }
                    break;
                case enType.Row:
                    {
                        if(mHostDrawPanel.GridAssistTarget.RowDefinitions.Count == 2)
                        {
                            mHostDrawPanel.GridAssistTarget.RowDefinitions.Clear();
                        }
                        else
                        {
                            mHostDrawPanel.GridAssistTarget.RowDefinitions.Remove(mLinkedDefinition);
                        }
                        mHostDrawPanel.GridAssistTarget.RowDefinitions = new CSUtility.Support.ThreadSafeObservableCollection<UISystem.Container.Grid.DefinitionBase>(mHostDrawPanel.GridAssistTarget.RowDefinitions);
                    }
                    break;
            }
            
            //mHostDrawPanel.GridAssistTarget.CellsStructureDirty = true;
            mHostDrawPanel.GridAssistTarget.UpdateLayout();
            mHostDrawPanel.InitializeGridAssist(true);
        }

        //private void ComboBox_GridUnitType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if (ComboBox_GridUnitType.SelectedIndex < 0)
        //        return;

        //    var enumValue = (UISystem.Container.Grid.GridUnitType)System.Enum.Parse(typeof(UISystem.Container.Grid.GridUnitType), ComboBox_GridUnitType.SelectedItem.ToString());
        //    if (UnityType != enumValue)
        //        UnityType = enumValue;
        //}

        private void Button_ResetMaxValue_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MaxValue = float.PositiveInfinity;
        }

        private void Button_ResetMinValue_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MinValue = 0;
        }

        private void userControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void userControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
