using System.ComponentModel;

namespace CCore.Audio
{
    /// <summary>
    /// 音频
    /// </summary>
    public class AudioListener : CSUtility.Support.XndSaveLoadProxy, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 属性改变的委托事件，在调整音频的位置等时调用
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //static AudioListener smInstance = new AudioListener();
        //public static AudioListener Instance
        //{
        //    get { return smInstance; }
        //}

        //public static void FinalInstance()
        //{
        //    smInstance = null;
        //}

        int mIndex = 0;
        /// <summary>
        /// 只读属性，索引值
        /// </summary>
        public int Index
        {
            get { return mIndex; }
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="idx">索引</param>
        public AudioListener(int idx)
        {
            mIndex = idx;
        }

        SlimDX.Vector3 mPosition = SlimDX.Vector3.Zero;
        /// <summary>
        /// 音频摆放的位置
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [Browsable(false)]
        public SlimDX.Vector3 Position
        {
            get { return mPosition; }
            set
            {
                if (mPosition == value)
                    return;

                mPosition = value;

                CCore.Audio.AudioManager.Instance.SetListenerAttributes(ref mPosition, ref mVelocity, ref mDirection, ref mUp, mIndex);

                OnPropertyChanged("Position");
            }
        }

        SlimDX.Vector3 mVelocity = SlimDX.Vector3.Zero;
        /// <summary>
        /// 音频的播放速度
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [Browsable(false)]
        public SlimDX.Vector3 Velocity
        {
            get { return mVelocity; }
            set
            {
                if (mVelocity == value)
                    return;

                mVelocity = value;

                CCore.Audio.AudioManager.Instance.SetListenerAttributes(ref mPosition, ref mVelocity, ref mDirection, ref mUp, mIndex);
                
                OnPropertyChanged("Velocity");
            }
        }

        SlimDX.Vector3 mDirection = SlimDX.Vector3.UnitX;
        /// <summary>
        /// 音频的朝向，默认为X轴
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [Browsable(false)]
        public SlimDX.Vector3 Direction
        {
            get { return mDirection; }
            set
            {
                if (mDirection == value)
                    return;

                mDirection = value;
                CCore.Audio.AudioManager.Instance.SetListenerAttributes(ref mPosition, ref mVelocity, ref mDirection, ref mUp, mIndex);
                OnPropertyChanged("Direction");
            }
        }

        SlimDX.Vector3 mUp = SlimDX.Vector3.UnitY;
        /// <summary>
        /// 声音的UP属性
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [Browsable(false)]
        public SlimDX.Vector3 Up
        {
            get { return mUp; }
            set
            {
                if (mUp == value)
                    return;

                mUp = value;
                CCore.Audio.AudioManager.Instance.SetListenerAttributes(ref mPosition, ref mVelocity, ref mDirection, ref mUp, mIndex);
                OnPropertyChanged("Up");
            }
        }
    }
}
