using System;
using System.ComponentModel;

namespace CCore.Audio
{
    /// <summary>
    /// 音频文件循环类型
    /// </summary>
    public enum enLoopType : uint
    {
        [Description("For non looping sounds")]
        Loop_Off = 0x00000001,
        [Description("For forward looping sounds")]
        Loop_Normal = 0x00000002,
        [Description("For bidirectional looping sounds")]
        Loop_Bidi = 0x00000004,
    }
    /// <summary>
    /// 声音衰减类型
    /// </summary>
    public enum enRollOffType : uint
    {
        [Description("This sound will follow the inverse rolloff model where mindistance = full volume, maxdistance = where sound stops attenuating, and rolloff is fixed according to the global rolloff factor.  (DEFAULT)")]
        FMOD_3D_INVERSEROLLOFF = 0x00100000,
        [Description("This sound will follow a linear rolloff model where mindistance = full volume, maxdistance = silence")]
        FMOD_3D_LINEARROLLOFF = 0x00200000,
        [Description("This sound will follow a linear-square rolloff model where mindistance = full volume, maxdistance = silence")]
        FMOD_3D_LINEARSQUAREROLLOFF = 0x00400000,
        [Description("This sound will follow the inverse rolloff model at distances close to mindistance and a linear-square rolloff close to maxdistance")]
        FMOD_3D_INVERSETAPEREDROLLOFF = 0x00800000,
        [Description("This sound will follow a rolloff model defined by Sound::set3DCustomRolloff / Channel::set3DCustomRolloff")]
        FMOD_3D_CUSTOMROLLOFF = 0x04000000,  
    }
    /// <summary>
    /// 音频文件管理类
    /// </summary>
    public class AudioManager : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 属性改变的委托事件
        /// </summary>
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
        /// <summary>
        /// 音频文件诊断类型
        /// </summary>
        public enum enAudioResultType
        {
            OK,                        /* No errors. */
            ERR_BADCOMMAND,            /* Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound::lock on a streaming sound). */
            ERR_CHANNEL_ALLOC,         /* Error trying to allocate a channel. */
            ERR_CHANNEL_STOLEN,        /* The specified channel has been reused to play another sound. */
            ERR_DMA,                   /* DMA Failure.  See debug output for more information. */
            ERR_DSP_CONNECTION,        /* DSP connection error.  Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts. */
            ERR_DSP_DONTPROCESS,       /* DSP return code from a DSP process query callback.  Tells mixer not to call the process callback and therefore not consume CPU.  Use this to optimize the DSP graph. */
            ERR_DSP_FORMAT,            /* DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map. */
            ERR_DSP_INUSE,             /* DSP is already in the mixer's DSP network. It must be removed before being reinserted or released. */
            ERR_DSP_NOTFOUND,          /* DSP connection error.  Couldn't find the DSP unit specified. */
            ERR_DSP_RESERVED,          /* DSP operation error.  Cannot perform operation on this DSP as it is reserved by the system. */
            ERR_DSP_SILENCE,           /* DSP return code from a DSP process query callback.  Tells mixer silence would be produced from read, so go idle and not consume CPU.  Use this to optimize the DSP graph. */
            ERR_DSP_TYPE,              /* DSP operation cannot be performed on a DSP of this type. */
            ERR_FILE_BAD,              /* Error loading file. */
            ERR_FILE_COULDNOTSEEK,     /* Couldn't perform seek operation.  This is a limitation of the medium (ie netstreams) or the file format. */
            ERR_FILE_DISKEJECTED,      /* Media was ejected while reading. */
            ERR_FILE_EOF,              /* End of file unexpectedly reached while trying to read essential data (truncated?). */
            ERR_FILE_ENDOFDATA,        /* End of current chunk reached while trying to read data. */
            ERR_FILE_NOTFOUND,         /* File not found. */
            ERR_FORMAT,                /* Unsupported file or audio format. */
            ERR_HEADER_MISMATCH,       /* There is a version mismatch between the FMOD header and either the FMOD Studio library or the FMOD Low Level library. */
            ERR_HTTP,                  /* A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere. */
            ERR_HTTP_ACCESS,           /* The specified resource requires authentication or is forbidden. */
            ERR_HTTP_PROXY_AUTH,       /* Proxy authentication is required to access the specified resource. */
            ERR_HTTP_SERVER_ERROR,     /* A HTTP server error occurred. */
            ERR_HTTP_TIMEOUT,          /* The HTTP request timed out. */
            ERR_INITIALIZATION,        /* FMOD was not initialized correctly to support this function. */
            ERR_INITIALIZED,           /* Cannot call this command after System::init. */
            ERR_INTERNAL,              /* An error occurred that wasn't supposed to.  Contact support. */
            ERR_INVALID_FLOAT,         /* Value passed in was a NaN, Inf or denormalized float. */
            ERR_INVALID_HANDLE,        /* An invalid object handle was used. */
            ERR_INVALID_PARAM,         /* An invalid parameter was passed to this function. */
            ERR_INVALID_POSITION,      /* An invalid seek position was passed to this function. */
            ERR_INVALID_SPEAKER,       /* An invalid speaker was passed to this function based on the current speaker mode. */
            ERR_INVALID_SYNCPOINT,     /* The syncpoint did not come from this sound handle. */
            ERR_INVALID_THREAD,        /* Tried to call a function on a thread that is not supported. */
            ERR_INVALID_VECTOR,        /* The vectors passed in are not unit length, or perpendicular. */
            ERR_MAXAUDIBLE,            /* Reached maximum audible playback count for this sound's soundgroup. */
            ERR_MEMORY,                /* Not enough memory or resources. */
            ERR_MEMORY_CANTPOINT,      /* Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used. */
            ERR_NEEDS3D,               /* Tried to call a command on a 2d sound when the command was meant for 3d sound. */
            ERR_NEEDSHARDWARE,         /* Tried to use a feature that requires hardware support. */
            ERR_NET_CONNECT,           /* Couldn't connect to the specified host. */
            ERR_NET_SOCKET_ERROR,      /* A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere. */
            ERR_NET_URL,               /* The specified URL couldn't be resolved. */
            ERR_NET_WOULD_BLOCK,       /* Operation on a non-blocking socket could not complete immediately. */
            ERR_NOTREADY,              /* Operation could not be performed because specified sound/DSP connection is not ready. */
            ERR_OUTPUT_ALLOCATED,      /* Error initializing output device, but more specifically, the output device is already in use and cannot be reused. */
            ERR_OUTPUT_CREATEBUFFER,   /* Error creating hardware sound buffer. */
            ERR_OUTPUT_DRIVERCALL,     /* A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted. */
            ERR_OUTPUT_FORMAT,         /* Soundcard does not support the specified format. */
            ERR_OUTPUT_INIT,           /* Error initializing output device. */
            ERR_OUTPUT_NODRIVERS,      /* The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails. */
            ERR_PLUGIN,                /* An unspecified error has been returned from a plugin. */
            ERR_PLUGIN_MISSING,        /* A requested output, dsp unit type or codec was not available. */
            ERR_PLUGIN_RESOURCE,       /* A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback) */
            ERR_PLUGIN_VERSION,        /* A plugin was built with an unsupported SDK version. */
            ERR_RECORD,                /* An error occurred trying to initialize the recording device. */
            ERR_REVERB_CHANNELGROUP,   /* Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection. */
            ERR_REVERB_INSTANCE,       /* Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist. */
            ERR_SUBSOUNDS,             /* The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have.  The operation may also not be able to be performed on a parent sound. */
            ERR_SUBSOUND_ALLOCATED,    /* This subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first. */
            ERR_SUBSOUND_CANTMOVE,     /* Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file. */
            ERR_TAGNOTFOUND,           /* The specified tag could not be found or there are no tags. */
            ERR_TOOMANYCHANNELS,       /* The sound created exceeds the allowable input channel count.  This can be increased using the 'maxinputchannels' parameter in System::setSoftwareFormat. */
            ERR_TRUNCATED,             /* The retrieved string is too long to fit in the supplied buffer and has been truncated. */
            ERR_UNIMPLEMENTED,         /* Something in FMOD hasn't been implemented when it should be! contact support! */
            ERR_UNINITIALIZED,         /* This command failed because System::init or System::setDriver was not called. */
            ERR_UNSUPPORTED,           /* A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified. */
            ERR_VERSION,               /* The version number of this file format is not supported. */
            ERR_EVENT_ALREADY_LOADED,  /* The specified bank has already been loaded. */
            ERR_EVENT_LIVEUPDATE_BUSY, /* The live update connection failed due to the game already being connected. */
            ERR_EVENT_LIVEUPDATE_MISMATCH, /* The live update connection failed due to the game data being out of sync with the tool. */
            ERR_EVENT_LIVEUPDATE_TIMEOUT, /* The live update connection timed out. */
            ERR_EVENT_NOTFOUND,        /* The requested event, bus or vca could not be found. */
            ERR_STUDIO_UNINITIALIZED,  /* The Studio::System object is not yet initialized. */
            ERR_STUDIO_NOT_LOADED,     /* The specified resource is not loaded, so it can't be unloaded. */

            ERR_INVALID_STRING,        /* An invalid string was passed to this function. */
            ERR_ALREADY_LOCKED,        /* The specified resource is already locked. */
            ERR_NOT_LOCKED,            /* The specified resource is not locked, so it can't be unlocked. */

            RESULT_FORCEINT = 65536    /* Makes sure this enum is signed 32bit. */
        }
        /// <summary>
        /// 音频播放模式
        /// </summary>
        [Flags]
        public enum enAudioMode : uint
        {
            [Description("Default for all modes listed below. FMOD_LOOP_OFF, FMOD_2D, FMOD_3D_WORLDRELATIVE, FMOD_3D_INVERSEROLLOFF")]
            FMOD_DEFAULT                   = 0x00000000,
            [Description("For non looping sounds. (DEFAULT).  Overrides FMOD_LOOP_NORMAL / FMOD_LOOP_BIDI")]
            FMOD_LOOP_OFF                  = 0x00000001,  
            [Description("For forward looping sounds")]
            FMOD_LOOP_NORMAL               = 0x00000002,  
            [Description("For bidirectional looping sounds. (only works on software mixed static sounds)")]
            FMOD_LOOP_BIDI                 = 0x00000004,  
            [Description("Ignores any 3d processing. (DEFAULT)")]
            FMOD_2D                        = 0x00000008,  
            [Description("Makes the sound positionable in 3D.  Overrides FMOD_2D")]
            FMOD_3D                        = 0x00000010,  
            [Description("Decompress at runtime, streaming from the source provided (ie from disk).  Overrides FMOD_CREATESAMPLE and FMOD_CREATECOMPRESSEDSAMPLE.  Note a stream can only be played once at a time due to a stream only having 1 stream buffer and file handle.  Open multiple streams to have them play concurrently")]
            FMOD_CREATESTREAM              = 0x00000080,  
            [Description("Decompress at loadtime, decompressing or decoding whole file into memory as the target sample format (ie PCM).  Fastest for playback and most flexible")]
            FMOD_CREATESAMPLE              = 0x00000100,  
            [Description("Load MP2/MP3/IMAADPCM/Vorbis/AT9 or XMA into memory and leave it compressed.  Vorbis/AT9 encoding only supported in the FSB file format.  During playback the FMOD software mixer will decode it in realtime as a 'compressed sample'.  Overrides FMOD_CREATESAMPLE.  If the sound data is not one of the supported formats, it will behave as if it was created with FMOD_CREATESAMPLE and decode the sound into PCM")]
            FMOD_CREATECOMPRESSEDSAMPLE    = 0x00000200,  
            [Description("Opens a user created static sample or stream. Use FMOD_CREATESOUNDEXINFO to specify format and/or read callbacks.  If a user created 'sample' is created with no read callback, the sample will be empty.  Use Sound::lock and Sound::unlock to place sound data into the sound if this is the case")]
            FMOD_OPENUSER                  = 0x00000400,  
            [Description("\"name_or_data\" will be interpreted as a pointer to memory instead of filename for creating sounds.  Use FMOD_CREATESOUNDEXINFO to specify length.  If used with FMOD_CREATESAMPLE or FMOD_CREATECOMPRESSEDSAMPLE, FMOD duplicates the memory into its own buffers.  Your own buffer can be freed after open.  If used with FMOD_CREATESTREAM, FMOD will stream out of the buffer whose pointer you passed in.  In this case, your own buffer should not be freed until you have finished with and released the stream")]
            FMOD_OPENMEMORY                = 0x00000800,  
            [Description("\"name_or_data\" will be interpreted as a pointer to memory instead of filename for creating sounds.  Use FMOD_CREATESOUNDEXINFO to specify length.  This differs to FMOD_OPENMEMORY in that it uses the memory as is, without duplicating the memory into its own buffers.  Cannot be freed after open, only after Sound::release.   Will not work if the data is compressed and FMOD_CREATECOMPRESSEDSAMPLE is not used")]
            FMOD_OPENMEMORY_POINT          = 0x10000000,  
            [Description("Will ignore file format and treat as raw pcm.  Use FMOD_CREATESOUNDEXINFO to specify format.  Requires at least defaultfrequency, numchannels and format to be specified before it will open.  Must be little endian data")]
            FMOD_OPENRAW                   = 0x00001000,  
            [Description("Just open the file, dont prebuffer or read.  Good for fast opens for info, or when sound::readData is to be used")]
            FMOD_OPENONLY                  = 0x00002000,  
            [Description("For System::createSound - for accurate Sound::getLength/Channel::setPosition on VBR MP3, and MOD/S3M/XM/IT/MIDI files.  Scans file first, so takes longer to open. FMOD_OPENONLY does not affect this")]
            FMOD_ACCURATETIME              = 0x00004000,  
            [Description("For corrupted / bad MP3 files.  This will search all the way through the file until it hits a valid MPEG header.  Normally only searches for 4k")]
            FMOD_MPEGSEARCH                = 0x00008000,  
            [Description("For opening sounds and getting streamed subsounds (seeking) asyncronously.  Use Sound::getOpenState to poll the state of the sound as it opens or retrieves the subsound in the background")]
            FMOD_NONBLOCKING               = 0x00010000,  
            [Description("Unique sound, can only be played one at a time")]
            FMOD_UNIQUE                    = 0x00020000,  
            [Description("Make the sound's position, velocity and orientation relative to the listener")]
            FMOD_3D_HEADRELATIVE           = 0x00040000,  
            [Description("Make the sound's position, velocity and orientation absolute (relative to the world). (DEFAULT)")]
            FMOD_3D_WORLDRELATIVE          = 0x00080000,  
            [Description("This sound will follow the inverse rolloff model where mindistance = full volume, maxdistance = where sound stops attenuating, and rolloff is fixed according to the global rolloff factor.  (DEFAULT)")]
            FMOD_3D_INVERSEROLLOFF         = 0x00100000,  
            [Description("This sound will follow a linear rolloff model where mindistance = full volume, maxdistance = silence")]
            FMOD_3D_LINEARROLLOFF          = 0x00200000,  
            [Description("This sound will follow a linear-square rolloff model where mindistance = full volume, maxdistance = silence")]
            FMOD_3D_LINEARSQUAREROLLOFF    = 0x00400000,  
            [Description("This sound will follow the inverse rolloff model at distances close to mindistance and a linear-square rolloff close to maxdistance")]
            FMOD_3D_INVERSETAPEREDROLLOFF  = 0x00800000,  
            [Description("This sound will follow a rolloff model defined by Sound::set3DCustomRolloff / Channel::set3DCustomRolloff")]
            FMOD_3D_CUSTOMROLLOFF          = 0x04000000,  
            [Description("Is not affect by geometry occlusion.  If not specified in Sound::setMode, or Channel::setMode, the flag is cleared and it is affected by geometry again")]
            FMOD_3D_IGNOREGEOMETRY         = 0x40000000,  
            /* Unused                        0x01000000,  Used to be FMOD_UNICODE */
            [Description("Skips id3v2/asf/etc tag checks when opening a sound, to reduce seek/read overhead when opening files (helps with CD performance)")]
            FMOD_IGNORETAGS                = 0x02000000,  
            [Description("Removes some features from samples to give a lower memory overhead, like Sound::getName.  See remarks")]
            FMOD_LOWMEM                    = 0x08000000,  
            [Description("Load sound into the secondary RAM of supported platform. On PS3, sounds will be loaded into RSX/VRAM")]
            FMOD_LOADSECONDARYRAM          = 0x20000000,  
            [Description("For sounds that start virtual (due to being quiet or low importance), instead of swapping back to audible, and playing at the correct offset according to time, this flag makes the sound play from the start")]
            FMOD_VIRTUAL_PLAYFROMSTART     = 0x80000000,  

        }
        /// <summary>
        /// 音频音质
        /// </summary>
        [Flags]
        public enum enAudioTimeUnit : int
        {
            FMOD_TIMEUNIT_MS = 0x00000001,
            FMOD_TIMEUNIT_PCM  = 0x00000002,
            FMOD_TIMEUNIT_PCMBYTES  = 0x00000004,
            FMOD_TIMEUNIT_RAWBYTES  = 0x00000008,
            FMOD_TIMEUNIT_PCMFRACTION  = 0x00000010,
            FMOD_TIMEUNIT_MODORDER  = 0x00000100,
            FMOD_TIMEUNIT_MODROW  = 0x00000200,
            FMOD_TIMEUNIT_MODPATTERN  = 0x00000400,
            FMOD_TIMEUNIT_BUFFERED  = 0x10000000,
        }
        /// <summary>
        /// 声明结束播放音频文件时调用的委托事件
        /// </summary>
        /// <param name="sourceId">音频文件的ID</param>
        public delegate void Delegate_AudioManager_OnPlayFinished(System.Guid sourceId);
        Delegate_AudioManager_OnPlayFinished onPlayFinishedEvent = null;
        /// <summary>
        /// 定义结束播放音频文件时调用的委托事件
        /// </summary>
        public event Delegate_AudioManager_OnPlayFinished OnPlayFinished;

        static AudioManager smInstance = new AudioManager();
        /// <summary>
        /// 创建AudioManage的实例，使其为单类
        /// </summary>
        [Browsable(false)]
        public static AudioManager Instance
        {
            get { return smInstance; }
        }
        /// <summary>
        /// 移除所有的音频文件
        /// </summary>
        public static void FinalInstance()
        {
            Instance.mListeners.Clear();
            smInstance = null;
        }

        IntPtr mAudioDevice = IntPtr.Zero;

        System.Collections.Generic.List<AudioListener> mListeners = new System.Collections.Generic.List<AudioListener>();
        /// <summary>
        /// 默认音频播放的属性，只读。
        /// </summary>
        [Browsable(false)]        
        public AudioListener DefaultListener
        {
            get
            {
                if(mListeners.Count <= 0)
                {
                    SetListenerNumbers(1);
                }

                return mListeners[0];
            }
        }
        /// <summary>
        /// 音频播放主音量控制属性，音量调节范围0-1.
        /// </summary>
        [CSUtility.Editor.Editor_ValueWithRange(0.0f, 1.0f)]
        public float MainVolume
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.vFMod_GetMainVolume(mAudioDevice);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.vFMod_SetMainVolume(mAudioDevice, value);
                }
            }
        }
        /// <summary>
        /// 音频在缓冲区存在时间，默认5分钟
        /// </summary>
        public UInt32 AudioBufferLifeTime = 300000; // 5分钟

        private AudioManager()
        {

        }
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
        ~AudioManager()
        {
            Cleanup();
        }
        /// <summary>
        /// 清除音频播放器
        /// </summary>
        public void Cleanup()
        {
            if (mAudioDevice != IntPtr.Zero)
            {
                DllImportAPI.vFMod_Delete(mAudioDevice);
                mAudioDevice = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 音频播放管理器的初始化，进行FMod的创建以及设置播放
        /// </summary>
        public void Initialize()
        {
            Cleanup();

            mAudioDevice = DllImportAPI.vFMod_New();
            DllImportAPI.vFMod_Initialize(mAudioDevice);
            
            onPlayFinishedEvent = this._OnPlayFinished;
            DllImportAPI.vFMod_SetOnPlayFinishedEvent(mAudioDevice, onPlayFinishedEvent);

            SetListenerNumbers(1);
        }
        /// <summary>
        /// 播放音频文件
        /// </summary>
        /// <param name="soundFile">需要播放的音频源文件及路径</param>
        /// <param name="id">音频文件的ID</param>
        /// <param name="soundType">音频文件的类型</param>
        /// <param name="loop">循环类型，默认为不循环，参数使用enLoopType枚举类型</param>
        /// <param name="rePlay">设置是否重复播放，默认为false</param>
        /// <param name="isStream">设置是否为音频流，默认false</param>
        /// <param name="pause">设置是否暂停播放，默认false</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType Play(string soundFile, Guid id, UInt32 soundType, enLoopType loop = enLoopType.Loop_Off, bool rePlay = false, bool isStream = false, bool pause = false)
        {
            var fileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(soundFile);

            unsafe{

                return (enAudioResultType)DllImportAPI.vFMod_Play(mAudioDevice, fileName, &id, soundType, loop, CCore.Engine.Instance.GetFrameMillisecond(), rePlay, isStream, pause);

            }
        }
        /// <summary>
        /// 精简播放音频文件
        /// </summary>
        /// <param name="soundFile">需要播放的音频源文件及路径</param>
        /// <param name="soundType">音频文件的类型</param>
        /// <param name="isStream">设置是否为流文件，默认false</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SimplePlay(string soundFile, UInt32 soundType, bool isStream = false)
        {
            var fileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(soundFile);

            unsafe
            {
                Guid id = Guid.NewGuid();
                return (enAudioResultType)DllImportAPI.vFMod_Play(mAudioDevice, fileName, &id, soundType, enLoopType.Loop_Off, CCore.Engine.Instance.GetFrameMillisecond(), false, isStream, false);
            }
        }
        /// <summary>
        /// 停止该ID的音频文件播放
        /// </summary>
        /// <param name="id">音频文件的ID</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType Stop(Guid id)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_Stop(mAudioDevice, &id);
            }
        }
        /// <summary>
        /// 设置该ID的音频文件是否暂停
        /// </summary>
        /// <param name="id">音频文件的ID</param>
        /// <param name="pause">设置是否暂停</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType Pause(Guid id, bool pause)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_Pause(mAudioDevice, &id, pause);
            }
        }
        /// <summary>
        /// 判断该ID的音频文件是否正在播放
        /// </summary>
        /// <param name="id">音频文件的ID</param>
        /// <returns>正在播放返回true，否则返回false</returns>
        public bool IsPlaying(Guid id)
        {
            if (mAudioDevice == IntPtr.Zero)
                return false;

            unsafe
            {
                return (DllImportAPI.vFMod_IsPlaying(mAudioDevice, &id) > 0) ? true : false;
            }
        }

        // 由C++回调的函数，当音乐播放完成时自动调用
        private void _OnPlayFinished(System.Guid sourceId)
        {
            if (OnPlayFinished != null)
                OnPlayFinished(sourceId);
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        public void Tick()
        {
            if (mAudioDevice == IntPtr.Zero)
                return;

            DllImportAPI.vFMod_Tick(mAudioDevice);
            DllImportAPI.vFMod_KickOffSource(mAudioDevice, CCore.Engine.Instance.GetFrameMillisecond(), AudioBufferLifeTime);
        }
        /// <summary>
        /// 设置播放器播放组数量
        /// </summary>
        /// <param name="num">音频文件播放组数量</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetListenerNumbers(int num)
        {
            var result = (enAudioResultType)DllImportAPI.vFMod_SetListenerNumbers(mAudioDevice, num);
            if (result == enAudioResultType.OK)
            {
                mListeners.Clear();
                for(int i=0; i<num; i++)
                {
                    var listener = new AudioListener(i);
                    mListeners.Add(listener);
                }
            }

            return result;
        }
        /// <summary>
        /// 设置音频播放组的属性
        /// </summary>
        /// <param name="pos">音频所在的位置，</param>
        /// <param name="vel">音频播放速率</param>
        /// <param name="forward">快进</param>
        /// <param name="up">加速</param>
        /// <param name="listenerId">播放组ID</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetListenerAttributes(ref SlimDX.Vector3 pos, ref SlimDX.Vector3 vel, ref SlimDX.Vector3 forward, ref SlimDX.Vector3 up, int listenerId = 0)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                fixed (SlimDX.Vector3* pinPos = &pos)
                {
                    fixed (SlimDX.Vector3* pinVel = &vel)
                    {
                        fixed (SlimDX.Vector3* pinForward = &forward)
                        {
                            fixed (SlimDX.Vector3* pinUp = &up)
                            {
                                return (enAudioResultType)DllImportAPI.vFMod_SetListenerAttributes(mAudioDevice, listenerId, pinPos, pinVel, pinForward, pinUp);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据类型获取声音长度
        /// </summary>
        /// <param name="soundFile">音频源文件</param>
        /// <param name="unit">音频类型</param>
        /// <returns>返回音频长度</returns>
        public UInt32 GetSoundLength(string soundFile, CCore.Audio.AudioManager.enAudioTimeUnit unit)
        {
            if (mAudioDevice == IntPtr.Zero)
                return 0;

            return DllImportAPI.vFMod_GetLength(mAudioDevice, soundFile, unit);
        }
        /// <summary>
        /// 设置3D音频属性
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="pos">音频所在位置</param>
        /// <param name="vel">音频播放速率</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DAttributes(Guid channelId, ref SlimDX.Vector3 pos, ref SlimDX.Vector3 vel)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                fixed (SlimDX.Vector3* pinPos = &pos)
                {
                    fixed (SlimDX.Vector3* pinVel = &vel)
                    {
                        return (enAudioResultType)DllImportAPI.vFMod_SetSound3DAttributes(mAudioDevice, &channelId, pinPos, pinVel);
                    }
                }
            }
        }
        /// <summary>
        /// 设置3D音频的圆锥方位
        /// </summary>
        /// <param name="channelId">音频文件通道ID</param>
        /// <param name="orientation">三维方向向量</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DConeOrientation(Guid channelId, ref SlimDX.Vector3 orientation)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                fixed (SlimDX.Vector3* pinOrientation = &orientation)
                {
                    return (enAudioResultType)DllImportAPI.vFMod_SetSound3DConeOrientation(mAudioDevice, &channelId, pinOrientation);
                }
            }
        }
        /// <summary>
        /// 3D音效的圆锥类型设置
        /// </summary>
        /// <param name="channelId">音频文件通道ID</param>
        /// <param name="insideconeangle">内圆锥角度</param>
        /// <param name="outsideconeangle">圆锥外角度</param>
        /// <param name="outsidevolume">圆锥外音量</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DConeSettings(Guid channelId, float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSound3DConeSettings(mAudioDevice, &channelId, insideconeangle, outsideconeangle, outsidevolume);
            }
        }
        /// <summary>
        /// 3D音频常规衰减
        /// </summary>
        /// <param name="channelId">音频文件通道ID</param>
        /// <param name="points">衰减区域</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DCustomRolloff(Guid channelId, SlimDX.Vector3[] points)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                fixed (SlimDX.Vector3* pinPoints = &points[0])
                {
                    return (enAudioResultType)DllImportAPI.vFMod_SetSound3DCustomRolloff(mAudioDevice, &channelId, pinPoints, points.Length);
                }
            }
        }
        /// <summary>
        /// 设置3D音效的距离
        /// </summary>
        /// <param name="channelId">音频通道的ID</param>
        /// <param name="custom">是否为定制的音频</param>
        /// <param name="customLevel">定制水平</param>
        /// <param name="centerFreq">中心频率</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DDistanceFilter(Guid channelId, bool custom, float customLevel, float centerFreq)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSound3DDistanceFilter(mAudioDevice, &channelId, custom, customLevel, centerFreq);
            }
        }
        /// <summary>
        /// 设置3D音频的多普勒级别
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="level">多普勒级别</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DDopplerLevel(Guid channelId, float level)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSound3DDopplerLevel(mAudioDevice, &channelId, level);
            }
        }
        /// <summary>
        /// 设置3D音频等级
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="level">音频等级</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DLevel(Guid channelId, float level)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSound3DLevel(mAudioDevice, &channelId, level);
            }
        }
        /// <summary>
        /// 设置3D音效的最小和最远距离
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="mindistance">最小距离</param>
        /// <param name="maxdistance">最远距离</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DMinMaxDistance(Guid channelId, float mindistance, float maxdistance)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSound3DMinMaxDistance(mAudioDevice, &channelId, mindistance, maxdistance);
            }
        }
        /// <summary>
        /// 设置3D音效遮挡
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="directocclusion">直接遮挡系数，1 代表没有声音可通过，0代表可以透过。</param>
        /// <param name="reverbocclusion">回响遮挡，0 表示反射音频被阻挡，1 表示该几何网格会阻挡所有回响反射。</param>
        /// <returns>返回结果是否成功的诊断信息</returns>
        public enAudioResultType SetSound3DOcclusion(Guid channelId, float directocclusion, float reverbocclusion)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSound3DOcclusion(mAudioDevice, &channelId, directocclusion, reverbocclusion);
            }
        }
        /// <summary>
        /// 设置3D声音传播
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="angle">传播角度，声音按照该角度进行传播</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSound3DSpread(Guid channelId, float angle)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSound3DSpread(mAudioDevice, &channelId, angle);
            }
        }
        /// <summary>
        /// 设置音频音量
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="volumn">音量大小，范围为0-1</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundVolumn(Guid channelId, float volumn)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundVolumn(mAudioDevice, &channelId, volumn);
            }
        }
        /// <summary>
        /// 设置音频的位置
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="position">位置信息</param>
        /// <param name="postype">音频音质</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundPosition(Guid channelId, UInt32 position, CCore.Audio.AudioManager.enAudioTimeUnit postype)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundPosition(mAudioDevice, &channelId, position, postype);
            }
        }
        /// <summary>
        /// 设置音频模式
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="mode">音频模式</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundMode(Guid channelId, CCore.Audio.AudioManager.enAudioMode mode)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundMode(mAudioDevice, &channelId, mode);
            }
        }
        /// <summary>
        /// 设置音频的频率
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="frequency">频率</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundFrequency(Guid channelId, float frequency)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundFrequency(mAudioDevice, &channelId, frequency);
            }
        }
        /// <summary>
        /// 设置音频的循环次数
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="loopcount">循环次数</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundLoopCount(Guid channelId, int loopcount)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundLoopCount(mAudioDevice, &channelId, loopcount);
            }
        }
        /// <summary>
        /// 设置音频循环点
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="loopstart">循环开始</param>
        /// <param name="loopstarttype">循环开始的方式</param>
        /// <param name="loopend">循环结束</param>
        /// <param name="loopendtype">循环结束的方式</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundLoopPoints(Guid channelId, UInt32 loopstart, CCore.Audio.AudioManager.enAudioTimeUnit loopstarttype, UInt32 loopend, CCore.Audio.AudioManager.enAudioTimeUnit loopendtype)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundLoopPoints(mAudioDevice, &channelId, loopstart, loopstarttype, loopend, loopendtype);
            }
        }
        /// <summary>
        /// 设置静音
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="mute">是否静音，true为静音，false则相反</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundMute(Guid channelId, bool mute)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundMute(mAudioDevice, &channelId, mute);
            }
        }
        /// <summary>
        /// 设置声道
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="pan">设置声道，-1:左声道，1:右声道，0:是双声道</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundPan(Guid channelId, float pan)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundPan(mAudioDevice, &channelId, pan);
            }
        }
        /// <summary>
        /// 设置音调
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="pitch">音调值，音调越高，同时播放时间越短</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundPitch(Guid channelId, float pitch)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundPitch(mAudioDevice, &channelId, pitch);
            }
        }
        /// <summary>
        /// 设置音频优先级
        /// </summary>
        /// <param name="channelId">音频通道ID</param>
        /// <param name="priority">优先级数值</param>
        /// <returns>返回结果的诊断信息</returns>
        public enAudioResultType SetSoundPriority(Guid channelId, int priority)
        {
            if (mAudioDevice == IntPtr.Zero)
                return enAudioResultType.ERR_INITIALIZATION;

            unsafe
            {
                return (enAudioResultType)DllImportAPI.vFMod_SetSoundPriority(mAudioDevice, &channelId, priority);
            }
        }
        /// <summary>
        /// 设置声音类型的音量
        /// </summary>
        /// <param name="soundType">音频类型</param>
        /// <param name="volumn">音量</param>
        public void SetSoundTypeVolume(UInt32 soundType, float volumn)
        {
            unsafe
            {
                DllImportAPI.vFMod_SetSoundTypeVolume(mAudioDevice, soundType, volumn);
            }
        }
        /// <summary>
        /// 得到相应音频类型的音量
        /// </summary>
        /// <param name="soundType">音频类型</param>
        /// <returns>返回结果的诊断信息</returns>
        public float GetSoundTypeVolume(UInt32 soundType)
        {
            unsafe
            {
                return DllImportAPI.vFMod_GetSoundTypeVolume(mAudioDevice, soundType);
            }
        }

    }
}
