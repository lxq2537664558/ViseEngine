using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace DelegateMethodEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for EventSetControl.xaml
    /// </summary>
    public partial class EventSetControl : UserControl
    {
        private Type mDelegateType = null;
        //private string mClassType = "";

        public Guid EventId
        {
            get { return (Guid)GetValue(EventIdProperty); }
            set { SetValue(EventIdProperty, value); }
        }
        public static readonly DependencyProperty EventIdProperty =
            DependencyProperty.Register("EventId", typeof(Guid), typeof(EventSetControl),
            new FrameworkPropertyMetadata(Guid.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnEventIdChanged)));

        public static void OnEventIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EventSetControl ctrl = d as EventSetControl;           
        }
        
        public PropertyDescriptor BindProperty
        {
            get { return (PropertyDescriptor)GetValue(BindPropertyProperty); }
            set { SetValue(BindPropertyProperty, value); }
        }
        public static readonly DependencyProperty BindPropertyProperty =
                            DependencyProperty.Register("BindProperty", typeof(PropertyDescriptor), typeof(EventSetControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBindPropertyChanged)));

        public static void OnBindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EventSetControl ctrl = d as EventSetControl;

            PropertyDescriptor newPro = e.NewValue as PropertyDescriptor;
            foreach (var att in newPro.Attributes)
            {
                if (att is CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute)
                {
                    var tAtt = att as CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute;
                    if (tAtt.Args == null || tAtt.Args.Length == 0)
                        continue;
                    ctrl.mDelegateType = (Type)tAtt.Args[0];

                    ctrl.ComboBox_Events.Items.Clear();
                    foreach (var item in DelegateMethodEditor.Program.GetEventList(ctrl.mDelegateType))
                    {
                        var cbItem = new DelegateMethodEditor.EventListItem(item.EventCallBack);
                        cbItem.Copy(item);
                        ctrl.ComboBox_Events.Items.Add(cbItem);
                    }

                    int i = 0;
                    for (i = 0; i < ctrl.ComboBox_Events.Items.Count; i++)
                    {
                        var cbItem = ctrl.ComboBox_Events.Items[i] as DelegateMethodEditor.EventListItem;
                        if (cbItem.EventId == ctrl.EventId)
                        {
                            ctrl.ComboBox_Events.SelectedIndex = i;
                            break;
                        }
                    }

                    if (i >= ctrl.ComboBox_Events.Items.Count)
                        ctrl.ComboBox_Events.SelectedIndex = -1;

                    break;
                }
            }
        }

        public DelegateMethodEditor.EventListItem SelectedItem
        {
            get { return (DelegateMethodEditor.EventListItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
                            DependencyProperty.Register("SelectedItem", typeof(DelegateMethodEditor.EventListItem), typeof(EventSetControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSelectedItemChanged)));

        public static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EventSetControl ctrl = d as EventSetControl;

            DelegateMethodEditor.EventListItem item = e.NewValue as DelegateMethodEditor.EventListItem;

            if (item != null)
                ctrl.EventId = item.EventId;
            else
                ctrl.EventId = Guid.Empty;
        }

        public EventSetControl()
        {
            InitializeComponent();
        }

        //private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    if (DelegateMethodEditor.Program.SelectedEventListItem != null)
        //    {
        //        DelegateMethodEditor.Program.SelectedEventListItem
        //        EventId = DelegateMethodEditor.Program.SelectedEventListItem.EventId;
        //    }
        //    else
        //        EventId = Guid.Empty;
        //}

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "DelegateMethodEditor",null});
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EventId = Guid.Empty;
            SelectedItem = null;
        }
    }
}
