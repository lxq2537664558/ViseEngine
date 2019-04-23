using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GamePublisher
{
    public class CheckBoxEx : System.Windows.Controls.Control
    {
        static CheckBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxEx), new FrameworkPropertyMetadata(typeof(CheckBoxEx)));
        }

        public enum enCheckState
        {
            AllChecked,
            PartChecked,
            UnChecked,
        }

        public enCheckState CheckState
        {
            get { return (enCheckState)GetValue(CheckStateProperty); }
            set { SetValue(CheckStateProperty, value); }
        }

        public static readonly DependencyProperty CheckStateProperty = DependencyProperty.Register("CheckState", typeof(enCheckState), typeof(CheckBoxEx),
                                                                                                     new FrameworkPropertyMetadata(enCheckState.UnChecked));

        public CheckBoxEx()
            : base()
        {
            MouseLeftButtonDown += CheckBoxEx_MouseLeftButtonDown;
        }

        void CheckBoxEx_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CheckState != enCheckState.AllChecked)
                CheckState = enCheckState.AllChecked;
            else if (CheckState != enCheckState.UnChecked)
                CheckState = enCheckState.UnChecked;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
