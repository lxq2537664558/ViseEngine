using System;
using System.ComponentModel;
using System.Windows.Media;

namespace ResourcesBrowser
{
    internal class FilterResourceItem : INotifyPropertyChanged
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

        bool mIsChecked = true;
        public bool IsChecked
        {
            get { return mIsChecked; }
            set
            {
                mIsChecked = value;
                mBrowserControl.UpdateFilter();
                OnPropertyChanged("IsChecked");
            }
        }
        
        public ImageSource Icon
        {
            get
            {
                if (mInfo != null)
                    return mInfo.Value.ResourceIcon;
                return null;
            }
        }
        
        public string ResourceTypeName
        {
            get
            {
                if (mInfo != null)
                    return mInfo.Value.ResourceTypeName;
                return "";
            }
        }

        Lazy<ResourceInfo, IResourceInfoMetaData> mInfo;
        BrowserControl mBrowserControl;

        public string ResourceType
        {
            get
            {
                if (mInfo != null)
                    return mInfo.Metadata.ResourceInfoType;

                return "";
            }
        }

        public FilterResourceItem(Lazy<ResourceInfo, IResourceInfoMetaData> info, BrowserControl ctrl)
        {
            mInfo = info;

            mBrowserControl = ctrl;
        }
    }
}
