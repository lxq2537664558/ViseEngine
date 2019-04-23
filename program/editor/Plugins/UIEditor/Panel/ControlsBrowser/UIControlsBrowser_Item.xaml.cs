using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace UIEditor
{
    /// <summary>
    /// Interaction logic for UIControlsBrowser_Item.xaml
    /// </summary>
    public partial class UIControlsBrowser_Item : UserControl, INotifyPropertyChanged
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

        private string mControlName = "";
        public string ControlName
        {
            get { return mControlName; }
            set
            {
                mControlName = value;

                OnPropertyChanged("ControlName");
            }
        }

        public Type TargetType
        {
            get;
            set;
        }

        bool mIsTemplate = false;
        public bool IsTemplate
        {
            get { return mIsTemplate; }
        }

        UISystem.Template.ControlTemplateInfo mTemplateInfo = null;
        public UISystem.Template.ControlTemplateInfo TemplateInfo
        {
            get { return mTemplateInfo; }
            set
            {
                mTemplateInfo = value;
                if (mTemplateInfo != null)
                {
                    mIsTemplate = true;
                    TargetType = mTemplateInfo.ControlTemplate.TargetType;
                    if (TargetType == null)
                        ControlName = "[] " + mTemplateInfo.ControlTemplate.WinName;
                    else
                        ControlName = "[" + mTemplateInfo.ControlTemplate.TargetType.Name + "] " + mTemplateInfo.ControlTemplate.WinName;
                }
            }
        }

        public UIControlsBrowser_Item()
        {
            InitializeComponent();
        }
    }
}
