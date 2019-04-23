using System;
using System.ComponentModel;

namespace CSUtility.Map
{
    public class ScenePoint_Camera : ScenePoint, CSUtility.Animation.TimeLineKeyFrameObjectInterface
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

        public delegate void Delegate_OnKeyTimeChanged(ScenePoint_Camera sp);
        public Delegate_OnKeyTimeChanged OnKeyTimeChanged;

        public Object EditorObject = null;

        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoad]
        public string KeyFrameName
        {
            get;
            set;
        }

        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public Int64 KeyFrameMilliTimeStart
        {
            get { return KeyTime; }
            set { KeyTime = value; }
        }
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public Int64 KeyFrameMilliTimeEnd
        {
            get { return KeyTime; }
            set { KeyTime = value; }
        }

        public bool CanModityLength()
        {
            return false;
        }
        public virtual string GetTimeLineKeyFrameObjectEditorControlType()
        {
            return "MainEditor.Panel.CameraAnimation.CameraPointControl";
        }

        Int64 mKeyTime = 0;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public Int64 KeyTime
        {
            get { return mKeyTime; }
            set
            {
                mKeyTime = value;

                if (OnKeyTimeChanged != null)
                    OnKeyTimeChanged(this);

                OnPropertyChanged("KeyTime");
            }
        }

        public ScenePoint_Camera()
        {
            KeyTime = 0;
            KeyFrameName = "";
        }

        public ScenePoint_Camera(Int64 time, string name)
        {
            KeyTime = time;
            KeyFrameName = name;
        }
    }
}
