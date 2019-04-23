using System;
using System.Windows;
using System.Windows.Controls;

namespace UIEditor.PropertyGrid
{
    /// <summary>
    /// Interaction logic for UIControlTemplateSetter.xaml
    /// </summary>
    public partial class UIControlTemplateSetter : UserControl
    {
        public string UIControlTemplateName
        {
            get { return (string)GetValue(UIControlTemplateNameProperty); }
            set { SetValue(UIControlTemplateNameProperty, value); }
        }
        public static readonly DependencyProperty UIControlTemplateNameProperty =
            DependencyProperty.Register("UIControlTemplateName", typeof(string), typeof(UIControlTemplateSetter),
                                        new UIPropertyMetadata());

        public Guid UIControlTemplateId
        {
            get { return (Guid)GetValue(UIControlTemplateIdProperty); }
            set { SetValue(UIControlTemplateIdProperty, value); }
        }
        public static readonly DependencyProperty UIControlTemplateIdProperty =
            DependencyProperty.Register("UIControlTemplateId", typeof(Guid), typeof(UIControlTemplateSetter),
            new FrameworkPropertyMetadata(Guid.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnUIControlTemplateIdChanged)));

        public static void OnUIControlTemplateIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIControlTemplateSetter control = d as UIControlTemplateSetter;

            Guid oldValue = (Guid)e.OldValue;
            Guid newValue = (Guid)e.NewValue;

            if (newValue == oldValue)
                return;

            if (newValue == Guid.Empty)
            {
                control.UIControlTemplateName = "";
            }
            else
            {
                var ctrlTemplate = UISystem.Template.TemplateMananger.Instance.FindControlTemplate(newValue);
                if (ctrlTemplate != null)
                {
                    control.UIControlTemplateName = ctrlTemplate.ControlTemplate.WinName + "(" + ctrlTemplate.ControlTemplate.TargetType + ")";
                }
                else
                    control.UIControlTemplateName = "";
            }
        }

        public UIControlTemplateSetter()
        {
            InitializeComponent();
        }

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UIControlTemplateId = WPG.Data.EditorContext.SelectedUIControlTemplateId;
        }

        //private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    //if(UIControlTemplateId != Guid.Empty)
        //    EditorCommon.ResourceSearch.ShowResource(EditorCommon.ResourceSearch.enResourceType.UVAnim, UIControlTemplateId);
        //}

        //private void Button_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    UIControlTemplateId = Guid.Empty;
        //}
    }
}
