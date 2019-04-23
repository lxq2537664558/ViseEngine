using System;
using System.ComponentModel;

namespace CCore.Audio
{
    /// <summary>
    /// 音源信息数据类
    /// </summary>
    [CCore.Socket.SocketComponentInfoAttribute("音效")]
    public class AudioSourceData : CSUtility.Animation.NotifyItemDataBase, CCore.Socket.ISocketComponentInfo, INotifyPropertyChanged
    {
        #region SocketComponentInfo
        /// <summary>
        /// SocketComponentInfoId属性
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("SocketComponentInfoId")]
        public Guid SocketComponentInfoId
        {
            get { return mAudioChannelId; }
            set { mAudioChannelId = value; }
        }

        string mSocketName = "";
        /// <summary>
        /// 挂接点名称的属性
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("SocketName")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSocketSetter")]
        [Category("基础属性")]
        [DisplayName("挂接点")]
        public string SocketName
        {
            get { return mSocketName; }
            set
            {
                mSocketName = value;
                OnPropertyChanged("SocketName");
            }
        }

        string mDescription = "";
        /// <summary>
        /// 音频的说明描述信息
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Description")]
        [Category("基础属性")]
        [DisplayName("说明")]
        public string Description
        {
            get { return mDescription; }
            set
            {
                mDescription = value;
                OnPropertyChanged("Description");
            }
        }
        /// <summary>
        /// 只读属性，音效的类型，得到"音效"
        /// </summary>
        [Browsable(false)]
        public string SocketComponentType
        {
            get { return "音效"; }
        }
        /// <summary>
        /// 拷贝一份CCore.Socket.ISocketComponentInfo
        /// </summary>
        /// <param name="srcInfo">需要拷贝的数据</param>
        public void CopyComponentInfoFrom(CCore.Socket.ISocketComponentInfo srcInfo)
        {
            var src = srcInfo as AudioSourceData;
            this.CopyFrom(src);
        }
        /// <summary>
        /// 得到AudioActor数据的类型
        /// </summary>
        /// <returns>返回AudioActor数据的类型</returns>
        public Type GetSocketComponentType()
        {
            return typeof(CCore.World.AudioActor);
        }

        #endregion

        //Dictionary<System.Reflection.PropertyInfo, object> mTickToSetPropertyList = new Dictionary<System.Reflection.PropertyInfo, object>();

        Guid mAudioChannelId = Guid.NewGuid();
        //[CSUtility.Support.AutoCopyAttribute]
        //[CSUtility.Support.AutoSaveLoadAttribute]
        /// <summary>
        /// 得到音频通道ID
        /// </summary>
        [Browsable(false)]
        public Guid AudioChannelId
        {
            get { return mAudioChannelId; }
        }

        float mMaxDistance = 10000.0f;
        /// <summary>
        /// 音频最远距离的属性设置，可在编辑器中调节
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("MaxDistance")]
        [Category("基础属性")]
        [DisplayName("最远距离")]
        public float MaxDistance
        {
            get { return mMaxDistance; }
            set
            {
                if (mMaxDistance == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("MaxDistance")] = value;
                //    return;
                //}

                mMaxDistance = value;
                if (mMaxDistance < mMinDistance)
                    mMaxDistance = mMinDistance;

                CCore.Audio.AudioManager.Instance.SetSound3DMinMaxDistance(mAudioChannelId, mMinDistance, mMaxDistance);

                OnPropertyChanged("MaxDistance");
            }
        }
        float mMinDistance = 1.0f;
        /// <summary>
        /// 音频最近距离的属性设置，可在编辑器中调节
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("MinDistance")]
        [Category("基础属性")]
        [DisplayName("最近距离")]
        public float MinDistance
        {
            get { return mMinDistance; }
            set
            {
                if (mMinDistance == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("MinDistance")] = value;
                //    return;
                //}

                mMinDistance = value;
                if (mMinDistance > mMaxDistance)
                    mMinDistance = mMaxDistance;

                CCore.Audio.AudioManager.Instance.SetSound3DMinMaxDistance(AudioChannelId, mMinDistance, mMaxDistance);

                OnPropertyChanged("MinDistance");
            }
        }
        float mVolumn = 1;
        /// <summary>
        /// 音频音量设置，可在编辑器中调节
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Volumn")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0f, 1.0f)]
        [Category("基础属性")]
        [DisplayName("音量")]
        public float Volumn
        {
            get { return mVolumn; }
            set
            {
                if (mVolumn == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Volumn")] = value;
                //    return;
                //}

                mVolumn = value;
                if (mVolumn < 0)
                    mVolumn = 0;
                if (mVolumn > 1.0f)
                    mVolumn = 1.0f;

                CCore.Audio.AudioManager.Instance.SetSoundVolumn(mAudioChannelId, mVolumn);

                OnPropertyChanged("Volumn");
            }
        }
        /// <summary>
        /// 音效是否循环播放，默认为不循环
        /// </summary>
        protected CCore.Audio.enLoopType mLoop = CCore.Audio.enLoopType.Loop_Off;
        /// <summary>
        /// 音频循环类型的属性设置，可在编辑器中调节
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Loop")]
        [Category("基础属性")]
        [DisplayName("循环类型")]
        public CCore.Audio.enLoopType Loop
        {
            get { return mLoop; }
            set
            {
                if (mLoop == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Loop")] = value;
                //    return;
                //}

                mLoop = value;
                CCore.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, (CCore.Audio.AudioManager.enAudioMode)mLoop);

                //if (!IsPlaying())
                //    Play();

                OnPropertyChanged("Loop");
            }
        }
        /// <summary>
        /// 是否在初始化时进行播放，默认为true
        /// </summary>
        protected bool mPlayWhenInitialized = true;
        /// <summary>
        /// 设置音频是否在初始化时播放
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("PlayWhenInitialized")]
        [Category("基础属性")]
        [DisplayName("初始化时播放")]
        public bool PlayWhenInitialized
        {
            get { return mPlayWhenInitialized; }
            set
            {
                mPlayWhenInitialized = value;
                OnPropertyChanged("PlayWhenInitialized");
            }
        }
        /// <summary>
        /// 音源
        /// </summary>
        protected string mAudioSource = "";
        /// <summary>
        /// 设置音源的路径
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("AudioSource")]
        //[CSUtility.Editor.Editor_SoundFileSetAttribute]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("SoundSet")]
        [Category("基础属性")]
        [DisplayName("音源")]
        public string AudioSource
        {
            get { return mAudioSource; }
            set
            {
                if (mAudioSource == value)
                    return;

                bool isPlaying = IsPlaying();
                if (isPlaying && !string.IsNullOrEmpty(mAudioSource))
                    Stop();

                mAudioSource = value;

                if (isPlaying)
                {
                    mAudioChannelId = Guid.Empty;
                    Play();
                }

                OnPropertyChanged("AudioSource");
            }
        }

        // Cone outside volume, from 0.0 to 1.0, default = 1.0
        /// <summary>
        /// 圆锥外音量，默认为1.0
        /// </summary>
        protected float mConeOuterVolume = 1.0f;
        /// <summary>
        /// 设置圆锥外音量属性，可在编辑器中调节
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("ConeOuterVolume")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0f, 1.0f)]
        [Category("基础属性")]
        [DisplayName("圆锥外音量")]
        public float ConeOuterVolume
        {
            get { return mConeOuterVolume; }
            set
            {
                if (ConeOuterVolume == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("ConeOuterVolume")] = value;
                //    return;
                //}

                mConeOuterVolume = value;

                CCore.Audio.AudioManager.Instance.SetSound3DConeSettings(mAudioChannelId, ConeInnerAngle, ConeOuterAngle, ConeOuterVolume);

                OnPropertyChanged("ConeOuterVolume");
            }
        }

        // Inside cone angle, in degrees. This is the angle within which the sound is at its normal volume. Must not be greater than 'outsideconeangle'. Default = 360
        /// <summary>
        /// 内圆锥角度，默认为360度
        /// </summary>
        protected float mConeInnerAngle = 360;
        /// <summary>
        /// 设置音频的内圆锥角度，可在编辑器中调节
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("ConeInnerAngle")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0f, 360.0f)]
        [Category("基础属性")]
        [DisplayName("内圆锥角度")]
        public float ConeInnerAngle
        {
            get { return mConeInnerAngle; }
            set
            {
                if (mConeInnerAngle == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("ConeInnerAngle")] = value;
                //    return;
                //}

                mConeInnerAngle = value;
                if (mConeInnerAngle > mConeOuterAngle)
                    mConeInnerAngle = mConeOuterAngle;

                CCore.Audio.AudioManager.Instance.SetSound3DConeSettings(mAudioChannelId, ConeInnerAngle, ConeOuterAngle, ConeOuterVolume);

                OnPropertyChanged("ConeInnerAngle");
            }
        }

        // Outside cone angle, in degrees. This is the angle outside of which the sound is at its outside volume. Must not be less than 'insideconeangle'. Default = 360
        /// <summary>
        /// 外圆锥角度，默认为360度
        /// </summary>
        protected float mConeOuterAngle = 360;
        /// <summary>
        /// 设置音频的外圆锥角度，可在编辑器中调节
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("ConeOuterAngle")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0f, 360.0f)]
        [Category("基础属性")]
        [DisplayName("外圆锥角度")]
        public float ConeOuterAngle
        {
            get { return mConeOuterAngle; }
            set
            {
                if (mConeOuterAngle == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("ConeOuterAngle")] = value;
                //    return;
                //}

                mConeOuterAngle = value;
                if (mConeOuterAngle < mConeInnerAngle)
                    mConeOuterAngle = mConeInnerAngle;

                CCore.Audio.AudioManager.Instance.SetSound3DConeSettings(mAudioChannelId, ConeInnerAngle, ConeOuterAngle, ConeOuterVolume);

                OnPropertyChanged("ConeOuterAngle");
            }
        }
        /// <summary>
        /// 音频的位置，默认为坐标原点
        /// </summary>
        protected SlimDX.Vector3 mPosition = SlimDX.Vector3.Zero;
        /// <summary>
        /// 设置音频的位置，可以通过游戏界面内直接操作
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Position")]
        [Category("基础属性")]
        [DisplayName("位置")]
        public SlimDX.Vector3 Position
        {
            get { return mPosition; }
            set
            {
                if (mPosition == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Position")] = value;
                //    return;
                //}

                mPosition = value;

                CCore.Audio.AudioManager.Instance.SetSound3DAttributes(mAudioChannelId, ref mPosition, ref mVelocity);

                OnPropertyChanged("Position");
            }
        }

        //protected SlimDX.Vector3 mDirection = SlimDX.Vector3.UnitX;
        //[Browsable(false)]
        //[CSUtility.Support.AutoCopyAttribute]
        //[CSUtility.Support.AutoSaveLoadAttribute]
        //public SlimDX.Vector3 Direction
        //{
        //    get { return mDirection; }
        //    set
        //    {
        //        if (mDirection == value)
        //            return;
        //        if (mAudioChannelId == 0)
        //        {
        //            mTickToSetPropertyList[this.GetType().GetProperty("Direction")] = value;
        //            return;
        //        }

        //        mDirection = value;

        //        // OpenAL使用右手坐标系，所以这里需要转为左手坐标系
        //        MidLayer.Audio.AudioManager.Instance.SetSourceParam(mAudioChannelId, MidLayer.Audio.enAudioParamType.AL_DIRECTION, mDirection.X, mDirection.Y, -mDirection.Z);

        //        OnPropertyChanged("Direction");
        //    }
        //}
        /// <summary>
        /// 音频的播放速度，默认为0
        /// </summary>
        protected SlimDX.Vector3 mVelocity = SlimDX.Vector3.Zero;
        /// <summary>
        /// 设置音频的播放速度
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Velocity")]
        [Category("基础属性")]
        [DisplayName("速度")]
        public SlimDX.Vector3 Velocity
        {
            get { return mVelocity; }
            set
            {
                if (mVelocity == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Velocity")] = value;
                //    return;
                //}

                mVelocity = value;

                CCore.Audio.AudioManager.Instance.SetSound3DAttributes(mAudioChannelId, ref mPosition, ref mVelocity);

                OnPropertyChanged("Velocity");
            }
        }
        /// <summary>
        /// 音频是否为3D音效，默认为false
        /// </summary>
        protected bool mSound3D = false;
        /// <summary>
        /// 设置音频是否在3D音效
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Sound3D")]
        [Category("基础属性")]
        [DisplayName("3D音效")]
        public bool Sound3D
        {
            get { return mSound3D; }
            set
            {
                if (mSound3D == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Sound3D")] = value;
                //    return;
                //}

                mSound3D = value;

                if(mSound3D)
                    CCore.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, CCore.Audio.AudioManager.enAudioMode.FMOD_3D);
                else
                    CCore.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, CCore.Audio.AudioManager.enAudioMode.FMOD_2D);

                OnPropertyChanged("Sound3D");
            }
        }

        // Number of times to loop before stopping. 0 = oneshot, 1 = loop once then stop, -1 = loop forever, default = -1
        /// <summary>
        /// 循环播放次数，默认无限循环
        /// </summary>
        protected int mLoopCount = -1;
        /// <summary>
        /// 设置音频的循环次数，默认为无限循环
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("LoopCount")]
        [Category("基础属性")]
        [DisplayName("循环次数")]
        [Description("循环次数设置，0为播放一次，1为循环一次，-1为无限循环")]
        public int LoopCount
        {
            get { return mLoopCount; }
            set
            {
                if (mLoopCount == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("LoopCount")] = value;
                //    return;
                //}

                mLoopCount = value;
                CCore.Audio.AudioManager.Instance.SetSoundLoopCount(mAudioChannelId, mLoopCount);
                //if (!IsPlaying())
                //    Play();

                OnPropertyChanged("LoopCount");
            }
        }
        /// <summary>
        /// 音频是否为静音，默认为false
        /// </summary>
        protected bool mMute = false;
        /// <summary>
        /// 设置音频为静音
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Mute")]
        [Category("基础属性")]
        [DisplayName("静音")]
        public bool Mute
        {
            get { return mMute; }
            set
            {
                if (mMute == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Mute")] = value;
                //    return;
                //}

                mMute = value;
                CCore.Audio.AudioManager.Instance.SetSoundMute(mAudioChannelId, mMute);

                OnPropertyChanged("Mute");
            }
        }
        /// <summary>
        /// 音频的声道，默认为双声道
        /// </summary>
        protected float mPan = 0;
        /// <summary>
        /// 设置音频声道，-1:左声道，1:右声道，0:是双声道
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Pan")]
        [CSUtility.Editor.Editor_ValueWithRange(-1.0f, 1.0f)]
        public float Pan
        {
            get { return mPan; }
            set
            {
                if (mPan == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Pan")] = value;
                //    return;
                //}

                mPan = value;
                CCore.Audio.AudioManager.Instance.SetSoundPan(mAudioChannelId, mPan);

                OnPropertyChanged("Pan");
            }
        }
        /// <summary>
        /// 音调，默认为1
        /// </summary>
        protected float mPitch = 1.0f;
        /// <summary>
        /// 音调值，音调越高，同时播放时间越短
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Pitch")]
        public float Pitch
        {
            get { return mPitch; }
            set
            {
                if (mPitch == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Pitch")] = value;
                //    return;
                //}

                mPitch = value;
                CCore.Audio.AudioManager.Instance.SetSoundPitch(mAudioChannelId, mPitch);

                OnPropertyChanged("Pitch");
            }
        }
        /// <summary>
        /// 音频的优先级，默认为128
        /// </summary>
        protected int mPriority = 128;
        /// <summary>
        /// 设置音频的优先级，越小优先级越高
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Priority")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 256)]
        [Category("基础属性")]
        [DisplayName("优先级")]
        [Description("越值越小优先级越高")]
        public int Priority
        {
            get { return mPriority; }
            set
            {
                if (mPriority == value)
                    return;
                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("Priority")] = value;
                //    return;
                //}

                mPriority = value;
                CCore.Audio.AudioManager.Instance.SetSoundPriority(mAudioChannelId, mPriority);

                OnPropertyChanged("Priority");
            }
        }
        /// <summary>
        /// 声音传播，默认为0
        /// </summary>
        protected float mSpread = 0;
        /// <summary>
        /// 设置音频声音传播属性
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("Spread")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 360)]
        [Browsable(false)]
        public float Spread
        {
            get { return mSpread; }
            set
            {
                //if (mSpread == value)
                //    return;
                ////if (mAudioChannelId == Guid.Empty)
                ////{
                ////    mTickToSetPropertyList[this.GetType().GetProperty("Spread")] = value;
                ////    return;
                ////}

                //mSpread = value;
                //MidLayer.Audio.AudioManager.Instance.SetSound3DSpread(mAudioChannelId, mSpread);

                //OnPropertyChanged("Spread");
            }
        }
        /// <summary>
        /// 音频衰减类型
        /// </summary>
        CCore.Audio.enRollOffType mRolloffType = CCore.Audio.enRollOffType.FMOD_3D_INVERSEROLLOFF;
        /// <summary>
        /// 设置音频衰减类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("RolloffType")]
        [Category("基础属性")]
        [DisplayName("衰减类型")]
        public CCore.Audio.enRollOffType RolloffType
        {
            get { return mRolloffType; }
            set
            {
                if (mRolloffType == value)
                    return;

                //if (mAudioChannelId == Guid.Empty)
                //{
                //    mTickToSetPropertyList[this.GetType().GetProperty("RolloffType")] = value;
                //    return;
                //}

                mRolloffType = value;
                CCore.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, (CCore.Audio.AudioManager.enAudioMode)mRolloffType);

                OnPropertyChanged("RolloffType");
            }
        }
        /// <summary>
        /// 是否为音频流，默认为false
        /// </summary>
        bool mIsStream = false;
        /// <summary>
        /// 设置音频是否为音频流
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("IsStream")]
        [Category("基础属性")]
        [DisplayName("是否为音频流")]
        public bool IsStream
        {
            get { return mIsStream; }
            set
            {
                if (mIsStream == value)
                    return;

                mIsStream = value;
                OnPropertyChanged("IsStream");
            }
        }
        /// <summary>
        /// 声音类型，默认为音乐
        /// </summary>
        CCore.Performance.ESoundType mSoundType = CCore.Performance.ESoundType.MusicSound;
        /// <summary>
        /// 声音类型的属性，可以再编辑器中直接修改，主要分为音乐，音效以及环境音
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DataValueAttribute("SoundType")]
        [Category("基础属性")]
        [DisplayName("声音类型")]
        public CCore.Performance.ESoundType SoundType
        {
            get { return mSoundType; }
            set
            {
                if (mSoundType == value)
                    return;

                mSoundType = value;
                OnPropertyChanged("SoundType");
            }
        }
        /// <summary>
        /// 音频源文件数据的构造函数，默认衰减类型为FMOD_3D_LINEARSQUAREROLLOFF
        /// </summary>
        public AudioSourceData()
        {
            RolloffType = CCore.Audio.enRollOffType.FMOD_3D_LINEARSQUAREROLLOFF;
        }

        //public void UpdateSourceData()
        //{
        //    if (mAudioChannelId == Guid.Empty)
        //        return;

        //    if (mTickToSetPropertyList.Count > 0)
        //    {
        //        foreach (var data in mTickToSetPropertyList)
        //        {
        //            data.Key.SetValue(this, data.Value, null);
        //        }
        //        mTickToSetPropertyList.Clear();
        //    }
        //}
        private void ForceResetPropertys()
        {
            if (mSound3D)
                CCore.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, CCore.Audio.AudioManager.enAudioMode.FMOD_3D);
            else
                CCore.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, CCore.Audio.AudioManager.enAudioMode.FMOD_2D);
            CCore.Audio.AudioManager.Instance.SetSound3DMinMaxDistance(mAudioChannelId, mMinDistance, mMaxDistance);
            CCore.Audio.AudioManager.Instance.SetSoundVolumn(mAudioChannelId, mVolumn);
            CCore.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, (CCore.Audio.AudioManager.enAudioMode)mLoop);
            CCore.Audio.AudioManager.Instance.SetSound3DConeSettings(mAudioChannelId, ConeInnerAngle, ConeOuterAngle, ConeOuterVolume);
            CCore.Audio.AudioManager.Instance.SetSound3DAttributes(mAudioChannelId, ref mPosition, ref mVelocity);
            CCore.Audio.AudioManager.Instance.SetSoundLoopCount(mAudioChannelId, mLoopCount);
            CCore.Audio.AudioManager.Instance.SetSoundMute(mAudioChannelId, mMute);
            CCore.Audio.AudioManager.Instance.SetSoundPan(mAudioChannelId, mPan);
            CCore.Audio.AudioManager.Instance.SetSoundPitch(mAudioChannelId, mPitch);
            CCore.Audio.AudioManager.Instance.SetSoundPriority(mAudioChannelId, mPriority);
            CCore.Audio.AudioManager.Instance.SetSound3DSpread(mAudioChannelId, mSpread);
            CCore.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, (CCore.Audio.AudioManager.enAudioMode)mRolloffType);
        }
        /// <summary>
        /// 音频播放，默认不重播
        /// </summary>
        /// <param name="rePlay">是否重复播放，缺省为false</param>
        public void Play(bool rePlay = false)
        {
            if (mIsLoading)
                return;

            if (mAudioChannelId == Guid.Empty)
                mAudioChannelId = Guid.NewGuid();

            CCore.Audio.AudioManager.Instance.Play(mAudioSource, mAudioChannelId, (UInt32)SoundType, Loop, rePlay, IsStream, true);

            ForceResetPropertys();
            //UpdateSourceData();

            //MidLayer.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, (MidLayer.Audio.AudioManager.enAudioMode)mLoop);
            //MidLayer.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, (MidLayer.Audio.AudioManager.enAudioMode)mRolloffType);
            //if (mSound3D)
            //    MidLayer.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, MidLayer.Audio.AudioManager.enAudioMode.FMOD_3D);
            //else
            //    MidLayer.Audio.AudioManager.Instance.SetSoundMode(mAudioChannelId, MidLayer.Audio.AudioManager.enAudioMode.FMOD_2D);

            CCore.Audio.AudioManager.Instance.Pause(mAudioChannelId, false);
        }
        /// <summary>
        /// 音频播放，可传入监听点值用来触发播放
        /// </summary>
        /// <param name="position">触发音频播放的位置</param>
        public void Play(SlimDX.Vector3 position)
        {
            Position = position;
            Play();
        }
        /// <summary>
        /// 该音频是否正在播放
        /// </summary>
        /// <returns>正在播放返回true，否则返回false</returns>
        public bool IsPlaying()
        {
            return CCore.Audio.AudioManager.Instance.IsPlaying(mAudioChannelId);
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            if (mAudioChannelId == Guid.Empty)
                return;

            CCore.Audio.AudioManager.Instance.Stop(mAudioChannelId);
        }
        /// <summary>
        /// 设置是否暂停播放
        /// </summary>
        /// <param name="pause">设置是否暂停播放，true代表暂停，false则相反</param>
        public void Pause(bool pause)
        {
            if(mAudioChannelId == Guid.Empty)
                return;

            CCore.Audio.AudioManager.Instance.Pause(mAudioChannelId, pause);
        }

        bool mIsLoading = false;
        /// <summary>
        /// 加载音频文件
        /// </summary>
        /// <param name="xndAtt">音频文件</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            mIsLoading = true;
            var retValue = base.Read(xndAtt);
            mIsLoading = false;
            return retValue;
        }
    }
}
