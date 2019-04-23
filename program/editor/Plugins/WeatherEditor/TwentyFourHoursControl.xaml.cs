using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WeatherEditor
{
    /// <summary>
    /// Interaction logic for TwentyFourHoursControl.xaml
    /// </summary>
    public partial class TwentyFourHoursControl : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public delegate void Delegate_OnKeyPointSelectionChanged(CCore.WeatherSystem.SunProperty sunProperty);
        public Delegate_OnKeyPointSelectionChanged OnKeyPointSelectionChanged;

        //System.DateTime mCurrentTime = new System.DateTime();
        //public System.DateTime CurrentTime
        //{
        //    get { return mCurrentTime; }
        //    set
        //    {
        //        mCurrentTime = value;
        //        UpdateCurrentTimeShow();

        //        var mainWorld = CCore.Engine.Instance.MainWorld as FrameSet.Scene.CellScene;
        //        mainWorld.Illumination.CurrTime = mCurrentTime;
        //    }
        //}

        public System.DateTime CurrentTime
        {
            get { return (System.DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(System.DateTime), typeof(TwentyFourHoursControl), new FrameworkPropertyMetadata(new System.DateTime(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnCurrentTimeChanged)));
        public static void OnCurrentTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TwentyFourHoursControl ctrl = d as TwentyFourHoursControl;

            ctrl.UpdateCurrentTimeShow();
        }

        public double CurrentHour
        {
            get { return CurrentTime.Hour + CurrentTime.Minute / 60.0 + CurrentTime.Second / 3600.0; }
        }

        // 日出
        public System.DateTime SunRiseTime
        {
            get { return (System.DateTime)GetValue(SunRiseTimeProperty); }
            set { SetValue(SunRiseTimeProperty, value); }
        }

        public static readonly DependencyProperty SunRiseTimeProperty =
            DependencyProperty.Register("SunRiseTime", typeof(System.DateTime), typeof(TwentyFourHoursControl), new FrameworkPropertyMetadata(new System.DateTime(2014, 1, 1, 7, 0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSunRiseTimeChanged)));
        public static void OnSunRiseTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TwentyFourHoursControl ctrl = d as TwentyFourHoursControl;
            ctrl.UpdateSunRiseSetTimeShow();
        }
        public double SunRiseTimeHour
        {
            get { return SunRiseTime.Hour + SunRiseTime.Minute / 60.0 + SunRiseTime.Second / 3600.0; }
        }

        // 日落
        public System.DateTime SunSetTime
        {
            get { return (System.DateTime)GetValue(SunSetTimeProperty); }
            set { SetValue(SunSetTimeProperty, value); }
        }

        public static readonly DependencyProperty SunSetTimeProperty =
            DependencyProperty.Register("SunSetTime", typeof(System.DateTime), typeof(TwentyFourHoursControl), new FrameworkPropertyMetadata(new System.DateTime(2014, 1, 1, 19, 0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSunSetTimeChanged)));
        public static void OnSunSetTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TwentyFourHoursControl ctrl = d as TwentyFourHoursControl;
            ctrl.UpdateSunRiseSetTimeShow();
        }
        public double SunSetTimeHour
        {
            get { return SunSetTime.Hour + SunSetTime.Minute / 60.0 + SunSetTime.Second / 3600.0; }
        }

        Ellipse mSelectedKeyPoint = null;

        bool mShowDiffuse = true;
        public bool ShowDiffuse
        {
            get { return mShowDiffuse; }
            set
            {
                mShowDiffuse = value;

                if (mShowDiffuse)
                    DiffusePolyLine.Visibility = System.Windows.Visibility.Visible;
                else
                    DiffusePolyLine.Visibility = System.Windows.Visibility.Collapsed;

                OnPropertyChanged("ShowDiffuse");
            }
        }

        bool mShowSpecular = true;
        public bool ShowSpecular
        {
            get { return mShowSpecular; }
            set
            {
                mShowSpecular = value;

                if (mShowSpecular)
                    SpecularPolyLine.Visibility = System.Windows.Visibility.Visible;
                else
                    SpecularPolyLine.Visibility = System.Windows.Visibility.Collapsed;

                OnPropertyChanged("ShowSpecular");
            }
        }

        bool mShowAmbient = true;
        public bool ShowAmbient
        {
            get { return mShowAmbient; }
            set
            {
                mShowAmbient = value;

                if (mShowAmbient)
                    AmbientPolyLine.Visibility = System.Windows.Visibility.Visible;
                else
                    AmbientPolyLine.Visibility = System.Windows.Visibility.Collapsed;

                OnPropertyChanged("ShowAmbient");
            }
        }

        public TwentyFourHoursControl()
        {
            InitializeComponent();

            UpdateShowNumbers();

            //var ilu = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination;
            //if (ilu != null)
            //{
            //    BindingOperations.SetBinding(this, CurrentTimeProperty, new Binding("CurrTime"){Source = ilu, Mode = BindingMode.TwoWay});
            //    BindingOperations.SetBinding(this, SunRiseTimeProperty, new Binding("SunRiseTime") { Source = ilu, Mode = BindingMode.TwoWay });
            //    BindingOperations.SetBinding(this, SunSetTimeProperty, new Binding("SunSetTime") { Source = ilu, Mode = BindingMode.TwoWay });

            //    foreach (var sunProperty in ilu.SunProertiesByHour)
            //    {
            //        sunProperty.PropertyChanged += SunProperty_PropertyChanged;
            //    }
            //}


            UpdateColorLineShow();
            UpdateSunRiseSetTimeShow();
        }

        ~TwentyFourHoursControl()
        {
            var ilu = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination;
            if (ilu != null)
            {
                foreach (var sunProperty in ilu.SunProertiesByHour)
                {
                    sunProperty.PropertyChanged -= SunProperty_PropertyChanged;
                }
            }
        }

        void SunProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateColorLineShow();
        }

        public void UpdateColorLineShow()
        {
            var ilu = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination;
            if (ilu == null)
                return;

            double maxLuminance = ilu.GetMaxLuminance();

            DiffusePolyLine.Points.Clear();
            AmbientPolyLine.Points.Clear();
            SpecularPolyLine.Points.Clear();
            Canvas_Keys.Children.Clear();

            BindingOperations.ClearBinding(this, SunRiseTimeProperty);
            BindingOperations.ClearBinding(this, SunSetTimeProperty);
            SunRiseTime = ilu.SunRiseTime;
            SunSetTime = ilu.SunSetTime;
            BindingOperations.SetBinding(this, SunRiseTimeProperty, new Binding("SunRiseTime") { Source = ilu, Mode = BindingMode.TwoWay });
            BindingOperations.SetBinding(this, SunSetTimeProperty, new Binding("SunSetTime") { Source = ilu, Mode = BindingMode.TwoWay });

            foreach (var sunProperty in ilu.SunProertiesByHour)
            {
                sunProperty.PropertyChanged -= SunProperty_PropertyChanged;
                sunProperty.PropertyChanged += SunProperty_PropertyChanged;

                var x = sunProperty.Hour / 24 * Canvas_Draw.ActualWidth;
                var dY = Canvas_Draw.ActualHeight - sunProperty.DiffuseLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                DiffusePolyLine.Points.Add(new System.Windows.Point(x, dY));

                var aY = Canvas_Draw.ActualHeight - sunProperty.AmbientLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                AmbientPolyLine.Points.Add(new System.Windows.Point(x, aY));

                var sY = Canvas_Draw.ActualHeight - sunProperty.SpecularLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                SpecularPolyLine.Points.Add(new System.Windows.Point(x, sY));

                var keyPoint = new Ellipse()
                {
                    Width = 15,
                    Height = 15,
                    Fill = this.FindResource("Key_Normal") as Brush,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    RenderTransform = new TranslateTransform(-7.5, 0),
                    Tag = sunProperty,
                };
                keyPoint.MouseEnter += KeyPoint_MouseEnter;
                keyPoint.MouseLeave += KeyPoint_MouseLeave;
                keyPoint.MouseDown += KeyPoint_MouseDown;
                Canvas_Keys.Children.Add(keyPoint);
                Canvas.SetLeft(keyPoint, x);
            }

            if (ilu.SunProertiesByHour.Count > 0)
            {
                var sunProperty = ilu.SunProertiesByHour[0];
                // 如果第一个点不在0位置， 则添加点与第一个点相接
                if (sunProperty.Hour > 0)
                {
                    var d0Y = Canvas_Draw.ActualHeight - sunProperty.DiffuseLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                    DiffusePolyLine.Points.Insert(0, new System.Windows.Point(0, d0Y));

                    var a0Y = Canvas_Draw.ActualHeight - sunProperty.AmbientLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                    AmbientPolyLine.Points.Insert(0, new System.Windows.Point(0, a0Y));

                    var s0Y = Canvas_Draw.ActualHeight - sunProperty.SpecularLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                    SpecularPolyLine.Points.Insert(0, new System.Windows.Point(0, s0Y));
                }

                // 最后一个点与第一个点相接
                var x = Canvas_Draw.ActualWidth;
                var dY = Canvas_Draw.ActualHeight - sunProperty.DiffuseLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                DiffusePolyLine.Points.Add(new System.Windows.Point(x, dY));

                var aY = Canvas_Draw.ActualHeight - sunProperty.AmbientLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                AmbientPolyLine.Points.Add(new System.Windows.Point(x, aY));

                var sY = Canvas_Draw.ActualHeight - sunProperty.SpecularLuminance / maxLuminance * Canvas_Draw.ActualHeight;
                SpecularPolyLine.Points.Add(new System.Windows.Point(x, sY));
            }
        }

        void KeyPoint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mSelectedKeyPoint != null)
            {
                mSelectedKeyPoint.Fill = this.FindResource("Key_Normal") as Brush;
            }
            mSelectedKeyPoint = sender as Ellipse;
            var sunProperty = mSelectedKeyPoint.Tag as CCore.WeatherSystem.SunProperty;
            CurrentTime = CurrentTime.AddHours(sunProperty.Hour - CurrentHour);

            e.Handled = true;

            if (OnKeyPointSelectionChanged != null)
                OnKeyPointSelectionChanged(mSelectedKeyPoint.Tag as CCore.WeatherSystem.SunProperty);
        }

        void KeyPoint_MouseLeave(object sender, MouseEventArgs e)
        {
            var keyPoint = sender as Ellipse;
            if (keyPoint == mSelectedKeyPoint)
                keyPoint.Fill = this.FindResource("Key_Selected") as Brush;
            else
                keyPoint.Fill = this.FindResource("Key_Normal") as Brush;
        }

        void KeyPoint_MouseEnter(object sender, MouseEventArgs e)
        {
            var keyPoint = sender as Ellipse;
            keyPoint.Fill = this.FindResource("Key_MouseEnter") as Brush;
        }

        private void UpdateShowNumbers()
        {
            Grid_Numbers.Children.Clear();
            Grid_Numbers.ColumnDefinitions.Clear();

            int interval = 1;
            var numberCount = 24 / interval;
            for (int i = 0; i < numberCount; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new System.Windows.GridLength(1, GridUnitType.Star);
                Grid_Numbers.ColumnDefinitions.Add(column);

                TextBlock textBlock = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontSize = 10,
                    Foreground = Brushes.White,//new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
                    Text = string.Format("{0,2}", (i * interval).ToString()),
                    Margin = new Thickness(1)
                };
                Grid.SetColumn(textBlock, i);

                Grid_Numbers.Children.Add(textBlock);
            }
        }

        private void Canvas_Draw_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            UpdateColorLineShow();
            UpdateSunRiseSetTimeShow();
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Grid_Main);
            var deltaHour = pos.X / Grid_Main.ActualWidth * 24 - CurrentHour;
            CurrentTime = CurrentTime.AddHours(deltaHour);

            Mouse.Capture(Grid_Main);
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(Grid_Main);
                if (pos.X < 0)
                    pos.X = 0;
                else if (pos.X > Grid_Main.ActualWidth)
                    pos.X = Grid_Main.ActualWidth;
                var deltaHour = pos.X / Grid_Main.ActualWidth * 24 - CurrentHour;
                CurrentTime = CurrentTime.AddHours(deltaHour);
            }
        }

        private void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void UpdateCurrentTimeShow()
        {
            Rect_Time.Margin = new Thickness(CurrentHour / 24 * Grid_Main.ActualWidth, 0, 0, 0);
        }

        public void UpdateSunRiseSetTimeShow()
        {
            var sunRiseX = SunRiseTimeHour / 24 * Canvas_Draw.ActualWidth;
            var sunSetX = SunSetTimeHour / 24 * Canvas_Draw.ActualWidth;
            if (sunRiseX > sunSetX)
            {
                sunSetX = sunRiseX + 1 / 24 * Canvas_Draw.ActualWidth;
            }

            Canvas.SetLeft(SunRise, sunRiseX);
            Canvas.SetLeft(SunSet, sunSetX);
            Canvas.SetLeft(Rect_WhiteDay, sunRiseX);
            Rect_WhiteDay.Width = sunSetX - sunRiseX;
        }

        private void Grid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            UpdateCurrentTimeShow();
        }

        private void Button_AddKeyPoint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var ilu = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination;
            if (ilu == null)
                return;

            CCore.WeatherSystem.SunProperty currSun = null;
            if (ilu.SunProertiesByHour.Count > 0)
            {
                int addIndex = ilu.GetInterpolateSunIndex(CurrentHour);
                currSun = ilu.GetInterpolateSunProperty(CurrentHour);
                if (currSun != null)
                {
                    currSun.Hour = CurrentHour;
                    ilu.SunProertiesByHour.Insert(addIndex, currSun);
                }
            }
            else
            {
                currSun = new CCore.WeatherSystem.SunProperty();
                ilu.SunProertiesByHour.Add(currSun);
            }

            UpdateColorLineShow();
        }

        private void Button_DelKeyPoint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mSelectedKeyPoint == null)
                return;

            var ilu = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination;
            if (ilu == null)
                return;

            var sunPro = mSelectedKeyPoint.Tag as CCore.WeatherSystem.SunProperty;

            ilu.SunProertiesByHour.Remove(sunPro);
            UpdateColorLineShow();
        }

        #region SunRiseSet

        System.Windows.Point mSunRiseSetMouseDownPoint;
        private void SunRise_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mSunRiseSetMouseDownPoint = e.GetPosition(SunRise);
            Mouse.Capture(SunRise);
            e.Handled = true;
        }

        private void SunRise_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pt = e.GetPosition(SunRise);
                var hourDelta = (pt.X - mSunRiseSetMouseDownPoint.X) / Canvas_Draw.ActualWidth * 24;
                if (SunRiseTimeHour + hourDelta < 0)
                    hourDelta = -SunRiseTimeHour;
                var time = SunRiseTime.AddHours(hourDelta);
                if (time > SunSetTime.AddHours(-1))
                {
                    time = SunSetTime.AddHours(-1);
                }

                SunRiseTime = time;
            }

            e.Handled = true;
        }

        private void SunRise_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void SunSet_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mSunRiseSetMouseDownPoint = e.GetPosition(SunSet);
            Mouse.Capture(SunSet);
            e.Handled = true;
        }

        private void SunSet_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pt = e.GetPosition(SunSet);
                var hourDelta = (pt.X - mSunRiseSetMouseDownPoint.X) / Canvas_Draw.ActualWidth * 24;
                if (SunSetTimeHour + hourDelta > 24)
                    hourDelta = 24 - SunSetTimeHour - 1 / 3600.0;
                var time = SunSetTime.AddHours(hourDelta);
                if (time < SunRiseTime.AddHours(1))
                {
                    time = SunRiseTime.AddHours(1);
                }

                SunSetTime = time;
            }

            e.Handled = true;
        }

        private void SunSet_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        #endregion

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            //BindingOperations.ClearBinding(this, CurrentTimeProperty);
            BindingOperations.SetBinding(this, CurrentTimeProperty, new Binding("CurrTime") { Source = CCore.WeatherSystem.IlluminationManager.Instance, Mode = BindingMode.TwoWay });

        }
    }
}
