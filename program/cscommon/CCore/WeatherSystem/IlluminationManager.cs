using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.WeatherSystem
{
    /// <summary>
    /// 光照管理类
    /// </summary>
    public class IlluminationManager : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托对象
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        static IlluminationManager smInstance = new IlluminationManager();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static IlluminationManager Instance
        {
            get { return smInstance; }
        }
        
        Illumination mCurrentIllumination = null;
        /// <summary>
        /// 只读属性，当前的光照地形
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Illumination CurrentIllumination
        {
            get { return mCurrentIllumination; }
        }

        Dictionary<Guid, Dictionary<Guid, Illumination>> mIlluminationDic = new Dictionary<Guid, Dictionary<Guid, Illumination>>();
        /// <summary>
        /// 只读属性，获取光照的Dictionary
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Dictionary<Guid, Dictionary<Guid, Illumination>> IlluminationDic
        {
            get { return mIlluminationDic; }
        }

        System.DateTime mCurrTime;
        /// <summary>
        /// 当前时间
        /// </summary>
        [System.ComponentModel.Category("全局")]
        [System.ComponentModel.DisplayName("当前时间")]
        [System.ComponentModel.Browsable(false)]
        public DateTime CurrTime
        {
            get { return mCurrTime; }
            set
            {
                mCurrTime = value;
                OnPropertyChanged("CurrTime");
            }
        }

        double mTimeAccelerate = 24;
        /// <summary>
        /// 时间流逝速率
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("系统")]
        [System.ComponentModel.DisplayName("时间流逝速率")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0, 10000.0)]
        public double TimeAccelerate
        {
            get { return mTimeAccelerate; }
            set
            {
                mTimeAccelerate = (float)value;
                //OnPropertyChanged("TimeAccelerate");
            }
        }

        bool mIsNight = false;
        /// <summary>
        /// 是否为夜晚
        /// </summary>
        [Browsable(false)]
        public bool IsNight
        {
            get { return mIsNight; }
            set { mIsNight = value; }
        }
        /// <summary>
        /// 保存光照对象
        /// </summary>
        /// <param name="world">所处的世界对象</param>
        /// <param name="id">对象ID</param>
        public void SaveIllumination(CCore.World.World world, Guid id)
        {
            if (world.Id == Guid.Empty)
                return;
            
            var absDir = Illumination.GetIlluminationAbsFolder(world);
            if (!System.IO.Directory.Exists(absDir))
            {
                System.IO.Directory.CreateDirectory(absDir);
            }

            Dictionary<Guid, Illumination> opValueDic = null;
            if (mIlluminationDic.TryGetValue(world.Id, out opValueDic))
            {
                Illumination opValue;
                if(opValueDic.TryGetValue(id, out opValue))
                {
                    var fileName = Illumination.GetIlluminationAbsFile(world, id);
                    opValue.Save(fileName);
                }
            }
        }
        /// <summary>
        /// 获取相应的光照对象
        /// </summary>
        /// <param name="world">所处的世界对象</param>
        /// <param name="id">对象ID</param>
        /// <param name="forceLoad">是否强制从磁盘加载</param>
        /// <returns>返回相应的光照对象</returns>
        public Illumination GetIllumination(CCore.World.World world , Guid id, bool forceLoad = false)
        {
            if (world == null || world.Id == Guid.Empty || id == Guid.Empty)
                return null;

            Dictionary<Guid, Illumination> opValueDic = null;
            if (!mIlluminationDic.TryGetValue(world.Id, out opValueDic))
            {
                opValueDic = new Dictionary<Guid, Illumination>();
                mIlluminationDic[world.Id] = opValueDic;
            }

            Illumination opValue = null;
            opValueDic.TryGetValue(id, out opValue);

            if (!forceLoad && opValue != null)
                return opValue;

            if (opValue == null)
            {
                opValue = new Illumination();
                opValueDic[id] = opValue;
            }

            var fileName = Illumination.GetIlluminationAbsFile(world, id);
            opValue.Load(fileName);

            return opValue;
        }
        /// <summary>
        /// 加载所有的光照对象
        /// </summary>
        /// <param name="world">对象所处的世界</param>
        public void LoadAllIllumination(CCore.World.World world)
        {
            if (world == null || world.Id == Guid.Empty)
                return;

            Dictionary<Guid, Illumination> illDic;
            if(!mIlluminationDic.TryGetValue(world.Id, out illDic))
            {
                illDic = new Dictionary<Guid, Illumination>();
                mIlluminationDic[world.Id] = illDic;
            }

            var absDir = Illumination.GetIlluminationAbsFolder(world);
            if (!System.IO.Directory.Exists(absDir))
                return;
            foreach (var file in System.IO.Directory.GetFiles(absDir))
            {
                var id = CSUtility.Program.GetIdFromFile(file);
                var ilu = GetIllumination(world, id);
                illDic[id] = ilu;
            }
        }
        /// <summary>
        /// 转换到场景的灯光对象
        /// </summary>
        /// <param name="world">当前的世界对象</param>
        /// <param name="id">对象ID</param>
        public void ChangedToIllumination(CCore.World.World world, Guid id)
        {
            //if (CurrentIllumination == null)
            //    mCurrentIllumination = GetIllumination(id);
            //else
            //{
            //    // 当前天气系统和目标天气系统平滑过渡
            //}
            mCurrentIllumination = GetIllumination(world, id);
            // todo: 绑定到场景的灯光对象设置
            //if (mCurrentIllumination != null)
            //{
            //    var world = MidLayer.IClient.MainWorldInstance as FrameSet.Scene.CellScene;
            //    if (world != null)
            //    {
            //        if (world.SunActor.Light!=null)
            //            world.SunActor.Light.ShadowType = mCurrentIllumination.ShadowType;
            //    }
            //}
        }
        /// <summary>
        /// 添加光照对象
        /// </summary>
        /// <param name="world">所处的世界</param>
        /// <param name="iluName">光照名称</param>
        /// <returns>返回添加的光照对象</returns>
        public Illumination AddIllumination(CCore.World.World world, string iluName)
        {
            if (world == null || world.Id == Guid.Empty)
                return null;

            Dictionary<Guid, Illumination> opValueDic = null;
            if (!mIlluminationDic.TryGetValue(world.Id, out opValueDic))
            {
                opValueDic = new Dictionary<Guid, Illumination>();
                mIlluminationDic[world.Id] = opValueDic;
            }

            Illumination ilu = new Illumination();
            ilu.Id = Guid.NewGuid();
            ilu.Name = iluName;

            opValueDic[ilu.Id] = ilu;
            return ilu;
        }
        /// <summary>
        /// 根据对象ID删除相应的光照对象
        /// </summary>
        /// <param name="world">所处的世界</param>
        /// <param name="id">对象ID</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool DelIllumination(CCore.World.World world, Guid id)
        {
            if (world == null || world.Id == Guid.Empty)
                return false;

            Dictionary<Guid, Illumination> opValueDic = null;
            if (!mIlluminationDic.TryGetValue(world.Id, out opValueDic))
                return false;

            Illumination ilu = null;
            if (opValueDic.TryGetValue(id, out ilu))
            {
                opValueDic.Remove(id);
            }

            return true;
        }

        long mShadowFadeDuration = 2000;
        /// <summary>
        /// 阴影淡出时间
        /// </summary>
        [System.ComponentModel.Category("全局")]
        [System.ComponentModel.DisplayName("阴影淡出时间")]
        public long ShadowFadeDuration
        {
            get { return mShadowFadeDuration; }
            set
            {
                mShadowFadeDuration = value;
                OnPropertyChanged("ShadowFadeDuration");
            }
        }

        long mShadowFadeCurrentTime = 0;
        float mPreTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();

        // todo: 天气系统去除对顶光和太阳光的依赖，调节的灯光由场景设置
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="headLight">顶光</param>
        /// <param name="targetPos">目标位置</param>
        public void Tick( CCore.World.LightActor headLight, SlimDX.Vector3 targetPos)
        {
            float nowTime = CCore.Engine.Instance.GetFrameSecondTimeFloat();
            float deltaTime = nowTime - mPreTime;
            CurrTime = CurrTime.AddSeconds(deltaTime * mTimeAccelerate);
            if (CurrTime.Year > 9000)
                CurrTime = CurrTime.AddYears(-7000);

            if (mCurrentIllumination != null)
                mCurrentIllumination.Tick(CurrTime, targetPos, mIsNight);

            if (mCurrentIllumination == null)
                return;
            var sunRise = CurrTime.TimeOfDay.CompareTo(mCurrentIllumination.SunRiseTime.TimeOfDay);
            var sunSet = CurrTime.TimeOfDay.CompareTo(mCurrentIllumination.SunSetTime.TimeOfDay);
            if (sunRise >= 0 && sunSet <= 0) // 晚于日出时间， 早于日落时间
            {
                if (mIsNight == true)
                {
                    mIsNight = false;
                    mShadowFadeCurrentTime = 0;
                }
            }
            else
            {
                if (mIsNight == false)
                {
                    mIsNight = true;
                    mShadowFadeCurrentTime = 0;
                }
            }

            if (mCurrentIllumination.mSaveUpdateMinuteInterval < 0)
                mCurrentIllumination.mSaveUpdateMinuteInterval = mCurrentIllumination.UpdateMinuteInterval;

            if (mShadowFadeCurrentTime < mShadowFadeDuration)
            {
                //mCurrentIllumination.UpdateMinuteInterval = 0.1;
                //sunLight.Light.ShadowType = mCurrentIllumination.ShadowType;
                //headLight.Light.ShadowType = mCurrentIllumination.ShadowType;
                float t = (float)mShadowFadeCurrentTime / ((float)mShadowFadeDuration/2.0f);
                if (mIsNight == true)
                {
                    if (t < 1.0f)
                    {
                        if(mCurrentIllumination.SunActor != null && mCurrentIllumination.SunActor.Light != null)
                            mCurrentIllumination.SunActor.Light.ShadowAlpha = 1 - t;
                        if(headLight != null)
                            headLight.Light.ShadowAlpha = 0;
                    }
                    else if (t >= 1.0f)
                    {
                        if (mCurrentIllumination.SunActor != null && mCurrentIllumination.SunActor.Light != null)
                        {
                            mCurrentIllumination.SunActor.Light.ShadowType = CCore.Light.EShadowType.None;
                        }
                        //headLight.Light.ShadowType = mCurrentIllumination.ShadowType;
                        if (headLight != null)
                            headLight.Light.ShadowAlpha = t-1;
                    }
                }
                else
                {
                    if (t < 1.0f)
                    {
                        if (headLight != null)
                            headLight.Light.ShadowAlpha = 1 - t;
                        if (mCurrentIllumination.SunActor != null && mCurrentIllumination.SunActor.Light != null)
                            mCurrentIllumination.SunActor.Light.ShadowAlpha = 0;
                    }
                    else if (t >= 1.0f)
                    {
                        if (headLight != null)
                            headLight.Light.ShadowType = CCore.Light.EShadowType.None;
                        //sunLight.Light.ShadowType = mCurrentIllumination.ShadowType;
                        if (mCurrentIllumination.SunActor != null && mCurrentIllumination.SunActor.Light != null)
                        {
                            mCurrentIllumination.SunActor.Light.ShadowAlpha = t - 1;
                            mCurrentIllumination.SunActor.Light.ShadowType = mCurrentIllumination.StoreSunShadowType;
                        }
                    }
                }
            }
            else
            {
                mCurrentIllumination.UpdateMinuteInterval = mCurrentIllumination.mSaveUpdateMinuteInterval;
                if (mIsNight == true)
                {
                    if (headLight != null)
                        headLight.Light.ShadowAlpha = 1;
                    if (mCurrentIllumination.SunActor != null && mCurrentIllumination.SunActor.Light != null)
                        mCurrentIllumination.SunActor.Light.ShadowType = CCore.Light.EShadowType.None;
                    //headLight.Light.ShadowType = mCurrentIllumination.ShadowType;
                }
                else
                {
                    if (mCurrentIllumination.SunActor != null && mCurrentIllumination.SunActor.Light != null)
                    {
                        mCurrentIllumination.SunActor.Light.ShadowAlpha = 1;
                        mCurrentIllumination.SunActor.Light.ShadowType = mCurrentIllumination.StoreSunShadowType;
                    }
                    if (headLight != null)
                        headLight.Light.ShadowType = CCore.Light.EShadowType.None;
                    //sunLight.Light.ShadowType = mCurrentIllumination.ShadowType;
                }
            }
            mShadowFadeCurrentTime += CCore.Engine.Instance.GetElapsedMillisecond();

            mPreTime = nowTime;
        }

    }
}
