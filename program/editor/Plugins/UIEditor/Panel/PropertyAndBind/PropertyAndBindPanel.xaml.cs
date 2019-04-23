using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace UIEditor
{
    /// <summary>
    /// Interaction logic for PropertyAndBindPanel.xaml
    /// </summary>
    public partial class PropertyAndBindPanel : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public delegate void Delegate_OnPropertyChange();
        public event Delegate_OnPropertyChange OnPropertyChangedEvent;
        // todo: 绑定当前窗口控件中的属性

        //public string PropertyFilterString
        //{
        //    get { return UIPropertyGrid.Filter; }
        //    set
        //    {
        //        UIPropertyGrid.Filter = value;
        //        //UIPropertyGrid.Instance = PropertyInstanceObject;
        //        OnPropertyChanged("PropertyFilterString");
        //    }
        //}

        string mEventFilterString = "";
        public string EventFilterString
        {
            get { return mEventFilterString; }
            set
            {
                mEventFilterString = value;

                foreach (Panel.PropertyAndBind.EventBindControl ctrl in StackPanel_Events.Children)
                {
                    var eventName = ctrl.EventName.ToLower();
                    var filterName = mEventFilterString.ToLower();
                    if (!string.IsNullOrEmpty(mEventFilterString) && !eventName.Contains(filterName))
                        ctrl.Visibility = Visibility.Collapsed;
                    else
                        ctrl.Visibility = Visibility.Visible;

                }

                OnPropertyChanged("EventFilterString");
            }
        }

        Object mPropertyInstanceObject = null;
        public Object PropertyInstanceObject
        {
            get { return mPropertyInstanceObject; }
            set
            {
                mPropertyInstanceObject = value;

                BindingOperations.ClearBinding(TextBox_Name, TextBox.TextProperty);
                BindingOperations.ClearBinding(UITypeSelector, UIEditor.PropertyGrid.UIControlTypesSelector.ControlTypeProperty);

                if (mPropertyInstanceObject != null)
                {
                    TextBox_Name.SetBinding(TextBox.TextProperty, new Binding("WinName")
                                                            {
                                                                Source = mPropertyInstanceObject,
                                                                Mode = BindingMode.TwoWay
                                                            });
                    TextBlock_Type.Text = mPropertyInstanceObject.GetType().FullName;

                    if (mPropertyInstanceObject is UISystem.Template.ControlTemplate)
                    {
                        UITypeSelector.SetBinding(UIEditor.PropertyGrid.UIControlTypesSelector.ControlTypeProperty,
                                                  new Binding("TargetType") { Source = mPropertyInstanceObject, Mode = BindingMode.TwoWay });

                        TextBlock_TargetType.Visibility = Visibility.Visible;
                        UITypeSelector.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        TextBlock_TargetType.Visibility = Visibility.Collapsed;
                        UITypeSelector.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    TextBox_Name.Text = "";
                    TextBlock_Type.Text = "";
                }

                if (mPropertyInstanceObject is UISystem.Template.ControlTemplate)
                {
                    var template = mPropertyInstanceObject as UISystem.Template.ControlTemplate;
                    UpdateEventsShow(template.TargetControl);
                    UpdateCommandShow(template.TargetControl);
                    UIPropertyGrid.Instance = template.TargetControl;
                }
                else
                {
                    UpdateEventsShow(value);
                    UpdateCommandShow(value);
                    UIPropertyGrid.Instance = value;                    
                }

                OnPropertyChanged("PropertyInstanceObject");

                if (OnPropertyChangedEvent != null)
                    OnPropertyChangedEvent();
            }
        }

        public PropertyAndBindPanel()
        {
            InitializeComponent();
        }

        private void UpdateEventsShow(Object instance)
        {
            StackPanel_Events.Children.Clear();

            //if (instance is UISystem.Button)
            //{
            //    //UISystem.WinForm form = (UISystem.WinForm)instance;
            //    //form.WinMouseEnter += form_WinMouseEnterXXXXX;

            //    var evnt = instance.GetType().GetEvent("WinMouseEnter");
            //    var handler = typeof(PropertyAndBindPanel).GetMethod("form_WinMouseEnterXXXXX", 
            //        new Type[] {typeof(System.Drawing.Point).MakeByRefType(), typeof(UISystem.WinBase) });
            //    Delegate d = Delegate.CreateDelegate(evnt.EventHandlerType, this, handler);
            //    evnt.AddEventHandler(instance, d);
            //}

            if (instance == null)
                return;

            var events = instance.GetType().GetEvents();
            foreach (var evt in events)
            {
                var attributes = evt.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_BindingEventAttribute), true);
                if (attributes.Length <= 0)
                    continue;

                Panel.PropertyAndBind.EventBindControl ctrl = new Panel.PropertyAndBind.EventBindControl(instance);
                ctrl.Event = evt;

                //ctrl.EventName = evt.Name;
                //System.Reflection.MethodInfo invoke = evt.EventHandlerType.GetMethod("Invoke");
                //ctrl.ParameterInfos = invoke.GetParameters();

                StackPanel_Events.Children.Add(ctrl);
            }
        }

        private void TextBox_Name_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        TextBox textBox = sender as TextBox;
                        if (textBox != null)
                        {
                            var bind = textBox.GetBindingExpression(TextBox.TextProperty);
                            if(bind != null)
                                bind.UpdateSource();
                        }
                    }
                    break;
            }
        }

        private void UpdateCommandShow(Object instance)
        {
            StackPanel_Commandes.Children.Clear();

            if (instance == null)
                return;

            var events = instance.GetType().GetEvents();
            foreach (var evt in events)
            {
                var attributes = evt.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_CommandEventAttribute), true);
                if (attributes.Length <= 0)
                    continue;

                Panel.PropertyAndBind.CommandBindControl ctrl = new Panel.PropertyAndBind.CommandBindControl(instance);
                ctrl.Event = evt;

                StackPanel_Commandes.Children.Add(ctrl);
            }
        }

        #region PropertyDragMove

        //bool mDragScrollViewer_Property = false;
        //Point mScrollViewerMouseLeftButtonDownPt;
        //double mScrollViewerVerticalOffsetRestore;
        //private void PropertyScrollViewer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{

        //}

        //private void PropertyScrollViewer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    //if (e.LeftButton == MouseButtonState.Pressed)
        //    //{
        //    //    if (mDragScrollViewer_Property == false)
        //    //    {
        //    //        mScrollViewerMouseLeftButtonDownPt = e.GetPosition(ScrollViewer_Property);
        //    //        mScrollViewerVerticalOffsetRestore = ScrollViewer_Property.VerticalOffset;
        //    //        mDragScrollViewer_Property = true;
        //    //    }
        //    //    else
        //    //    {
        //    //        var pos = e.GetPosition(ScrollViewer_Property);
        //    //        var delta = mScrollViewerMouseLeftButtonDownPt.Y - pos.Y;

        //    //        ScrollViewer_Property.ScrollToVerticalOffset(mScrollViewerVerticalOffsetRestore + delta);
        //    //    }
        //    //}
        //}

        //private void PropertyScrollViewer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    //mDragScrollViewer_Property = false;
        //}
        #endregion
    }
}
