using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TemplateEditor
{
    public class ViewNodeItem : DependencyObject, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        TemplateEditorControl mHostControl;

        public ImageSource Icon { get; set; }

        public object ID
        {
            get { return GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("Name", typeof(object), typeof(ViewNodeItem), new PropertyMetadata(new PropertyChangedCallback((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            var item = d as ViewNodeItem;
            if(item.mHostControl.ErrorNodeItems.Contains(item))
            {
                if(object.Equals(e.NewValue, item.mHostControl.GetIDFromFileName(item.AbsFileName)))
                {
                    item.mHostControl.ErrorNodeItems.Remove(item);
                    item.mHostControl.UpdateErrorInfo();
                }
            }
        })));
        public string IDHighLightString
        {
            get { return (string)GetValue(IDHighLightStringProperty); }
            set { SetValue(IDHighLightStringProperty, value); }
        }
        public static readonly DependencyProperty IDHighLightStringProperty = DependencyProperty.Register("IDHighLightString", typeof(string), typeof(ViewNodeItem), new PropertyMetadata(""));

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(ViewNodeItem), new PropertyMetadata(""));

        public string DisplayNameHighLightString
        {
            get { return (string)GetValue(DisplayNameHighLightStringProperty); }
            set { SetValue(DisplayNameHighLightStringProperty, value); }
        }
        public static readonly DependencyProperty DisplayNameHighLightStringProperty = DependencyProperty.Register("DisplayNameHighLightString", typeof(string), typeof(ViewNodeItem), new PropertyMetadata(""));

        public string FileName { get; private set; }
        string mAbsFileName = "";
        public string AbsFileName
        {
            get { return mAbsFileName; }
            set
            {
                mAbsFileName = value;
                FileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(mAbsFileName);
            }
        }

        public object DataTemplate
        {
            get;
            private set;
        }

        public string ErrorMessage { get; set; } = "";

        //System.Collections.ObjectModel.ObservableCollection<TreeViewNodeItem> mChildren = new System.Collections.ObjectModel.ObservableCollection<TreeViewNodeItem>();
        //public System.Collections.ObjectModel.ObservableCollection<TreeViewNodeItem> Children
        //{
        //    get { return mChildren; }
        //    set { Children = value; }
        //}

        public ViewNodeItem(object dataTemplate, TemplateEditorControl hostControl)
        {
            mHostControl = hostControl;
            DataTemplate = dataTemplate;

            BindingOperations.SetBinding(this, DisplayNameProperty, new Binding("Name") { Source = dataTemplate });
            BindingOperations.SetBinding(this, IDProperty, new Binding("Id") { Source = dataTemplate });
        }
    }
}
