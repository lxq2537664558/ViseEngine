using System.ComponentModel;

namespace CSUtility.Data
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class ActionNamePair : CSUtility.Support.ICopyable, INotifyPropertyChanged
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

        string mName = "";
        [CSUtility.Support.DataValueAttribute("Name")]
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;
                OnPropertyChanged("Name");
            }
        }
        string mActionFile = "";
        [CSUtility.Support.DataValueAttribute("ActionFile")]
        [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.Action)]
        public string ActionFile
        {
            get { return mActionFile; }
            set
            {
                mActionFile = value;
                OnPropertyChanged("ActionFile");
            }
        }
        float mPlayRate = 1.0F;
        [CSUtility.Support.DataValueAttribute("PlayRate")]
        public float PlayRate
        {
            get { return mPlayRate; }
            set { mPlayRate = value; }
        }

        public bool CopyFrom(CSUtility.Support.ICopyable src)
        {
            return CSUtility.Support.Copyable.CopyFrom(src, this);
        }
    }
}
