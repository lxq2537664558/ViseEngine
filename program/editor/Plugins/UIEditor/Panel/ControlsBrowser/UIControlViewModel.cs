using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UIEditor
{
    internal interface IUIControlVM : INotifyPropertyChanged
    {
        IUIControlVM Parent { get; }
        ObservableCollection<IUIControlVM> Children { get; set; }
        string ControlName { get; set; }
        bool IsExpanded { get; set; }
        Visibility Visibility { get; set; }
        string HighLightString { get; set; }
    }

    internal class UIControlViewModelParent : IUIControlVM
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        IUIControlVM mParent = null;
        public IUIControlVM Parent
        {
            get { return mParent; }
        }

        ObservableCollection<IUIControlVM> mChildren = new ObservableCollection<IUIControlVM>();
        public ObservableCollection<IUIControlVM> Children
        {
            get { return mChildren; }
            set { mChildren = value; }
        }

        string mControlName;
        public string ControlName
        {
            get { return mControlName; }
            set
            {
                mControlName = value;
                OnPropertyChanged("ControlName");
            }
        }

        bool mIsExpanded = true;
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set
            {
                if (mIsExpanded != value)
                {
                    mIsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                if (mIsExpanded && mParent != null)
                    mParent.IsExpanded = true;
            }
        }

        Visibility mVisibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return mVisibility; }
            set
            {
                if (mVisibility != value)
                {
                    mVisibility = value;
                    OnPropertyChanged("Visibility");
                }
            }
        }

        string mHighLightString = "";
        [Browsable(false)]
        public string HighLightString
        {
            get { return mHighLightString; }
            set
            {
                mHighLightString = value;
                OnPropertyChanged("HighLightString");
            }
        }
    }

    internal class UIControlViewModel : IUIControlVM, EditorCommon.DragDrop.IDragAbleObject
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        Type mControlType;
        public Type Control
        {
            get { return mControlType; }
        }

        string mControlName = "";
        public string ControlName
        {
            get { return mControlName; }
            set
            {
                mControlName = value;
                OnPropertyChanged("ControlName");
            }
        }

        ImageSource mIcon = null;
        public ImageSource Icon
        {
            get { return mIcon; }
            set
            {
                mIcon = value;
                OnPropertyChanged("Icon");
            }
        }

        IUIControlVM mParent;
        public IUIControlVM Parent
        {
            get { return mParent; }
        }

        ObservableCollection<IUIControlVM> mChildren = new ObservableCollection<IUIControlVM>();
        public ObservableCollection<IUIControlVM> Children
        {
            get { return mChildren; }
            set { mChildren = value; }
        }

        bool mIsExpanded = true;
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set
            {
                if(mIsExpanded != value)
                {
                    mIsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                if (mIsExpanded && mParent != null)
                    mParent.IsExpanded = true;
            }
        }

        Visibility mVisibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return mVisibility; }
            set
            {
                if(mVisibility != value)
                {
                    mVisibility = value;
                    OnPropertyChanged("Visibility");
                }
            }
        }

        string mHighLightString = "";
        [Browsable(false)]
        public string HighLightString
        {
            get { return mHighLightString; }
            set
            {
                mHighLightString = value;
                OnPropertyChanged("HighLightString");
            }
        }

        public UIControlViewModel(Type controlType)
            : this(controlType, null)
        {
        }

        public UIControlViewModel(Type controlType, UIControlViewModel parent)
        {
            mControlType = controlType;
            mParent = parent;

            if(mControlType != null)
            {
                var atts = controlType.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_ControlAttribute), false);
                if (atts.Length > 0)
                {
                    var name = ((CSUtility.Editor.UIEditor_ControlAttribute)atts[0]).Name;
                    var idx = name.LastIndexOf('.');
                    if (idx < 0)
                        ControlName = name;
                    else
                        ControlName = name.Substring(idx + 1, name.Length - (idx + 1));
                }
                else
                    ControlName = mControlType.Name;

                try
                {
                    Icon = new BitmapImage(new Uri("pack://application:,,,/UIEditor;component/Source/ControlIcons/" + mControlType.Name + ".png"));
                }
                catch(System.Exception)
                {
                    // 部分控件可能找不到图标，这里做一下异常捕获
                }
            }
        }

        private bool NameContainsText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(ControlName))
                return false;

            return ControlName.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        public System.Windows.FrameworkElement DragVisual = null;
        public System.Windows.FrameworkElement GetDragVisual()
        {
            return DragVisual;
        }
    }
}
