using System;
using System.Collections.Generic;
using SlimDX;
using System.ComponentModel;
/// <summary>
/// 天气系统的名空间
/// </summary>
namespace CCore.WeatherSystem
{
    /// <summary>
    /// 太阳属性
    /// </summary>
    public class SunProperty : CSUtility.Support.XndSaveLoadProxy, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
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
        /// <summary>
        /// 构造函数
        /// </summary>
        public SunProperty()
        {
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Sun_PropertyChanged);
        }
        void Sun_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DiffuseLuminance = GetLuminance(SunDiffuse) * DiffuseIntensity;
            AmbientLuminance = GetLuminance(SunAmbient) * DiffuseIntensity;
            SpecularLuminance = GetLuminance(SunSpecular) * SpecularIntensity;
        }

        double GetLuminance(CSUtility.Support.Color c)
        {
            return c.R / 255.0f * 0.2126f + c.G / 255.0f * 0.7152f + c.B / 255.0f * 0.0722f;
        }

        // 光源强度*颜色亮度
        double mDiffuseLuminance;
        /// <summary>
        /// 分散光强度
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.Category("天光属性")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0, 100.0)]
        public double DiffuseLuminance
        {
            get
            {
                return mDiffuseLuminance;
            }
            set
            {
                if (mDiffuseLuminance == value)
                    return;

                mDiffuseLuminance = value;
                OnPropertyChanged("DiffuseLuminance");
            }
        }

        double mAmbientLuminance;
        /// <summary>
        /// 环境光强度
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.Category("天光属性")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0, 100.0)]
        public double AmbientLuminance
        {
            get
            {
                return mAmbientLuminance;
            }
            set
            {
                if (mAmbientLuminance == value)
                    return;

                mAmbientLuminance = value;
                OnPropertyChanged("AmbientLuminance");
            }
        }

        double mSpecularLuminance;
        /// <summary>
        /// 反射光强度
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.Category("天光属性")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0, 100.0)]
        public double SpecularLuminance
        {
            get
            {
                return mSpecularLuminance;
            }
            set
            {
                if (mSpecularLuminance == value)
                    return;

                mSpecularLuminance = value;
                OnPropertyChanged("SpecularLuminance");
            }
        }

        double mHour = 0;
        /// <summary>
        /// 光照时长
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Browsable(false)]
        public double Hour
        {
            get { return mHour; }
            set
            {
                mHour = value;
                OnPropertyChanged("Hour");
            }
        }

        double mDiffuseIntensity = 1.0;
        /// <summary>
        /// 分散光亮度
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("天光属性")]
        [System.ComponentModel.DisplayName("Diffuse亮度")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0, 100.0)]
        public double DiffuseIntensity
        {
            get { return mDiffuseIntensity; }
            set
            {
                mDiffuseIntensity = value;
                OnPropertyChanged("DiffuseIntensity");
            }
        }

        CSUtility.Support.Color mSunDiffuse = CSUtility.Support.Color.White;
        /// <summary>
        /// 太阳光的分散光颜色
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("天光属性")]
        public CSUtility.Support.Color SunDiffuse
        {
            get { return mSunDiffuse; }
            set
            {
                mSunDiffuse = value;
                //MediaDiffuse.Color = System.Windows.Media.Color.FromRgb(value.R, value.G, value.B);
                //MediaDiffuse = MediaDiffuse;
                OnPropertyChanged("SunDiffuse");
            }
        }

        CSUtility.Support.Color mSunAmbient = CSUtility.Support.Color.Gray;
        /// <summary>
        /// 环境光颜色
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("天光属性")]
        public CSUtility.Support.Color SunAmbient
        {
            get { return mSunAmbient; }
            set
            {
                mSunAmbient = value;
                //MediaAmbient.Color = System.Windows.Media.Color.FromRgb(value.R, value.G, value.B);
                //MediaAmbient = MediaAmbient;
                OnPropertyChanged("SunAmbient");
            }
        }


        double mSpecularIntensity = 1.0;
        /// <summary>
        /// 反射光亮度
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("天光属性")]
        [System.ComponentModel.DisplayName("Specular亮度")]
        [CSUtility.Editor.Editor_ValueWithRange(0.0, 100.0)]
        public double SpecularIntensity
        {
            get { return mSpecularIntensity; }
            set
            {
                mSpecularIntensity = value;
                OnPropertyChanged("SpecularIntensity");
            }
        }

        CSUtility.Support.Color mSunSpecular = CSUtility.Support.Color.Black;
        /// <summary>
        /// 反射光颜色
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("天光属性")]
        public CSUtility.Support.Color SunSpecular
        {
            get { return mSunSpecular; }
            set
            {
                mSunSpecular = value;
                //MediaSpecular.Color = System.Windows.Media.Color.FromRgb(value.R, value.G, value.B);
                //MediaSpecular = MediaSpecular;
                OnPropertyChanged("SunSpecular");
            }
        }

        //Quaternion mQuat = new Quaternion();
        //[CSUtility.Support.AutoSaveLoad]
        //[System.ComponentModel.Category("天光属性")]
        //[System.ComponentModel.DisplayName("方向")]
        //public Quaternion Quat
        //{
        //    get { return mQuat; }
        //    set
        //    {
        //        mQuat = value;
        //        OnPropertyChanged("Quat");
        //    }
        //}

        //#region 编辑器使用属性
        //System.Windows.Media.SolidColorBrush mMediaDiffuse = new System.Windows.Media.SolidColorBrush();
        //[System.ComponentModel.Browsable(false)]
        //public System.Windows.Media.SolidColorBrush MediaDiffuse
        //{
        //    get { return mMediaDiffuse; }
        //    set
        //    {
        //        mMediaDiffuse = value;
        //        OnPropertyChanged("MediaDiffuse");
        //    }
        //}
        //System.Windows.Media.SolidColorBrush mMediaAmbient = new System.Windows.Media.SolidColorBrush();
        //[System.ComponentModel.Browsable(false)]
        //public System.Windows.Media.SolidColorBrush MediaAmbient
        //{
        //    get { return mMediaAmbient; }
        //    set
        //    {
        //        mMediaAmbient = value;
        //        OnPropertyChanged("MediaAmbient");
        //    }
        //}
        //System.Windows.Media.SolidColorBrush mMediaSpecular = new System.Windows.Media.SolidColorBrush();
        //[System.ComponentModel.Browsable(false)]
        //public System.Windows.Media.SolidColorBrush MediaSpecular
        //{
        //    get { return mMediaSpecular; }
        //    set
        //    {
        //        mMediaSpecular = value;
        //        OnPropertyChanged("MediaSpecular");
        //    }
        //}
        //#endregion
        /// <summary>
        /// 插入太阳光
        /// </summary>
        /// <param name="src">插入太阳光的源数据</param>
        /// <param name="dest">目标数据</param>
        /// <param name="amount">数量</param>
        /// <returns>返回插入的太阳光对象</returns>
        static public SunProperty Interpolate(SunProperty src, SunProperty dest, float amount)
        {
            SunProperty newProperty = new SunProperty();

            SlimDX.Vector4 vSrcDiffuse = new SlimDX.Vector4();
            CSUtility.Program.DrawColor2Vector(src.mSunDiffuse, out vSrcDiffuse);
            SlimDX.Vector4 vDestDiffuse = new SlimDX.Vector4();
            CSUtility.Program.DrawColor2Vector(dest.mSunDiffuse, out vDestDiffuse);

            SlimDX.Vector4 vSrcAmbient = new SlimDX.Vector4();
            CSUtility.Program.DrawColor2Vector(src.mSunAmbient, out vSrcAmbient);
            SlimDX.Vector4 vDestAmbient = new SlimDX.Vector4();
            CSUtility.Program.DrawColor2Vector(dest.mSunAmbient, out vDestAmbient);

            SlimDX.Vector4 vSrcSpecular = new SlimDX.Vector4();
            CSUtility.Program.DrawColor2Vector(src.mSunSpecular, out vSrcSpecular);
            SlimDX.Vector4 vDestSpecular = new SlimDX.Vector4();
            CSUtility.Program.DrawColor2Vector(dest.mSunSpecular, out vDestSpecular);

            newProperty.DiffuseIntensity = src.DiffuseIntensity * (1 - amount) + dest.DiffuseIntensity * amount;
            newProperty.SpecularIntensity = src.SpecularIntensity * (1 - amount) + dest.SpecularIntensity * amount;
            CSUtility.Program.Vector2DrawColor(SlimDX.Vector4.SmoothStep(vSrcDiffuse, vDestDiffuse, amount), out newProperty.mSunDiffuse);
            CSUtility.Program.Vector2DrawColor(SlimDX.Vector4.SmoothStep(vSrcAmbient, vDestAmbient, amount), out newProperty.mSunAmbient);
            CSUtility.Program.Vector2DrawColor(SlimDX.Vector4.SmoothStep(vSrcSpecular, vDestSpecular, amount), out newProperty.mSunSpecular);
            //newProperty.mQuat = SlimDX.Quaternion.Slerp(src.mQuat, dest.mQuat, amount);

            return newProperty;
        }
        /// <summary>
        /// 保存该对象到XND文件
        /// </summary>
        /// <param name="attr">XND数据节点</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool Save(CSUtility.Support.XndAttrib attr)
        {
            attr.BeginWrite();
            //int iVersion = 2;
            //attr.Write(iVersion);
            //attr.Write((float)DiffuseIntensity);
            //attr.Write((float)SpecularIntensity);
            //attr.Write((float)Hour);
            //SlimDX.Vector4 vD = new SlimDX.Vector4();
            //MidLayer.IUtility.IConverter.DrawColor2Vector(SunDiffuse,ref vD);
            //SlimDX.Vector4 vA = new SlimDX.Vector4();
            //MidLayer.IUtility.IConverter.DrawColor2Vector(SunAmbient, ref vA);
            //SlimDX.Vector4 vS = new SlimDX.Vector4();
            //MidLayer.IUtility.IConverter.DrawColor2Vector(SunSpecular, ref vS);
            //attr.Write(ref vD);
            //attr.Write(ref vA);
            //attr.Write(ref vS);
            ////attr.Write(ref mQuat);
            Write(attr);
            attr.EndWrite();

            return true;
        }
        /// <summary>
        /// 从XND文件加载对象数据
        /// </summary>
        /// <param name="attr">XND文件节点</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public bool Load(CSUtility.Support.XndAttrib attr)
        {
            attr.BeginRead();
            Read(attr);
            //int iVersion = 0;
            //attr.Read(out iVersion);
            //switch (iVersion)
            //{
            //    case 1:
            //        {
            //            float fTemp;
            //            attr.Read(out fTemp);
            //            DiffuseIntensity = fTemp;
            //            SpecularIntensity = fTemp;
            //            attr.Read(out fTemp);
            //            Hour = fTemp;
            //            SlimDX.Vector4 vTemp = new SlimDX.Vector4();
            //            Color cTemp = new Color();
            //            attr.Read(out vTemp);
            //            MidLayer.IUtility.IConverter.Vector2DrawColor(vTemp, ref cTemp);
            //            SunDiffuse = cTemp;
            //            attr.Read(out vTemp);
            //            MidLayer.IUtility.IConverter.Vector2DrawColor(vTemp, ref cTemp);
            //            SunAmbient = cTemp;
            //            attr.Read(out vTemp);
            //            MidLayer.IUtility.IConverter.Vector2DrawColor(vTemp, ref cTemp);
            //            SunSpecular = cTemp;
            //            Quaternion qTemp = new Quaternion();
            //            attr.Read(out qTemp);
            //            Quat = qTemp;
            //            attr.EndRead();
            //        }
            //        break;
            //    case 2:
            //        {
            //            float fTemp;
            //            attr.Read(out fTemp);
            //            DiffuseIntensity = fTemp;
            //            attr.Read(out fTemp);
            //            SpecularIntensity = fTemp;
            //            attr.Read(out fTemp);
            //            Hour = fTemp;
            //            SlimDX.Vector4 vTemp = new SlimDX.Vector4();
            //            Color cTemp = new Color();
            //            attr.Read(out vTemp);
            //            MidLayer.IUtility.IConverter.Vector2DrawColor(vTemp, ref cTemp);
            //            SunDiffuse = cTemp;
            //            attr.Read(out vTemp);
            //            MidLayer.IUtility.IConverter.Vector2DrawColor(vTemp, ref cTemp);
            //            SunAmbient = cTemp;
            //            attr.Read(out vTemp);
            //            MidLayer.IUtility.IConverter.Vector2DrawColor(vTemp, ref cTemp);
            //            SunSpecular = cTemp;
            //            Quaternion qTemp = new Quaternion();
            //            attr.Read(out qTemp);
            //            Quat = qTemp;
            //            attr.EndRead();
            //        }
            //        break;
            //}
            attr.EndRead();
            return true;
        }
    }

    /// <summary>
    /// 照明类
    /// </summary>
    public class Illumination : CSUtility.Support.XndSaveLoadProxy, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
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

        Guid mSunActorId = Guid.Empty;
        /// <summary>
        /// 太阳Actor的ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("全局")]
        [DisplayName("太阳")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("ActorSetter", new object[] { CSUtility.Component.EActorGameType.Light })]
        public Guid SunActorId
        {
            get { return mSunActorId; }
            set
            {
                mSunActorId = value;
                mSunActor = CCore.Client.MainWorldInstance.FindActor(mSunActorId) as CCore.World.LightActor;
                OnPropertyChanged("SunActorId");
            }
        }

        CCore.Light.EShadowType mStoreSunShadowType = CCore.Light.EShadowType.None;
        /// <summary>
        /// 太阳阴影类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("全局")]
        [System.ComponentModel.DisplayName("太阳阴影类型")]
        public CCore.Light.EShadowType StoreSunShadowType
        {
            get { return mStoreSunShadowType; }
            set
            {
                mStoreSunShadowType = value;
                OnPropertyChanged("StoreSunShadowType");
            }
        }

        CCore.World.LightActor mSunActor = null;
        /// <summary>
        /// 只读属性，太阳Actor对象
        /// </summary>
        [Browsable(false)]
        public CCore.World.LightActor SunActor
        {
            get { return mSunActor; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Illumination()
        {
            List<CSUtility.Support.Color> tempColors = new List<CSUtility.Support.Color>();
            //tempColors.Add(Color.LightBlue);
            //tempColors.Add(Color.LightGreen);
            //tempColors.Add(Color.LightSeaGreen);
            //tempColors.Add(Color.GreenYellow);
            //tempColors.Add(Color.LemonChiffon);
            //tempColors.Add(Color.CadetBlue);
            //tempColors.Add(Color.Goldenrod);
            //tempColors.Add(Color.Khaki);
            //tempColors.Add(Color.Linen);
            //tempColors.Add(Color.Ivory);
            //tempColors.Add(Color.HotPink);
            tempColors.Add(CSUtility.Support.Color.White);
            //float deltaAngle = (float)(System.Math.PI - System.Math.PI / 4) / 24.0f;
            for ( int i = 0; i < tempColors.Count; ++i )
            {
                SunProperty sp = new SunProperty();
                sp.Hour = i*(24 / tempColors.Count);
                //sp.Quat = Quaternion.RotationAxis(Vector3.UnitX, -(float)(System.Math.PI / 8) - deltaAngle * (float)sp.Hour);
                //sp.Quat.Normalize();
                sp.SunDiffuse = tempColors[i];

                mSunProertiesByHour.Add(sp);
            }
        }
        /// <summary>
        /// 获取插入的太阳属性
        /// </summary>
        /// <param name="hour">时间</param>
        /// <returns>返回得到的太阳属性</returns>
        public CCore.WeatherSystem.SunProperty GetInterpolateSunProperty(double hour)
        {
            if (mSunProertiesByHour.Count == 0)
                return null;

            for (int i = 0; i <= mSunProertiesByHour.Count; ++i)
            {
                SunProperty sp = null, preSP = null;
                if (i == 0)
                {
                    sp = mSunProertiesByHour[i];
                    if (sp.Hour > hour)
                    {
                        preSP = mSunProertiesByHour[mSunProertiesByHour.Count - 1];
                        float amount = 1;
                        return SunProperty.Interpolate(preSP, sp, amount);
                    }
                }
                else if (i == mSunProertiesByHour.Count)
                {
                    sp = mSunProertiesByHour[0];
                    preSP = mSunProertiesByHour[mSunProertiesByHour.Count - 1];
                    float amount = (float)((hour - preSP.Hour) / (24 - preSP.Hour));
                    return SunProperty.Interpolate(preSP, sp, amount);
                }
                else
                {
                    sp = mSunProertiesByHour[i];
                    if (sp.Hour > hour)
                    {
                        preSP = mSunProertiesByHour[i - 1];
                        float amount = (float)((hour - preSP.Hour) / (sp.Hour - preSP.Hour));
                        return SunProperty.Interpolate(preSP, sp, amount);
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// 获取插入的太阳索引
        /// </summary>
        /// <param name="hour">时间</param>
        /// <returns>返回对应的索引值</returns>
        public int GetInterpolateSunIndex(double hour)
        {
            if (mSunProertiesByHour.Count == 0)
                return 0;

            for (int i = 0; i <= mSunProertiesByHour.Count; ++i)
            {
                SunProperty sp = null;
                if (i == 0)
                {
                    sp = mSunProertiesByHour[i];
                    if (sp.Hour > hour)
                    {
                        return 0;
                    }
                }
                else if (i == mSunProertiesByHour.Count)
                {
                    return mSunProertiesByHour.Count;
                }
                else
                {
                    sp = mSunProertiesByHour[i];
                    if (sp.Hour > hour)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        float mPreTime = Engine.Instance.GetFrameSecondTimeFloat();
        float mRemainTime = 0;
        float mSunAngle = 0;
        /// <summary>
        /// 太阳光角度
        /// </summary>
        [Browsable(false)]
        public float SunAngle
        {
            get { return mSunAngle; }
            set
            {
                mSunAngle = value;
                CCore.Client.MainWorldInstance.SunAngle = value;
            }
        }

        SlimDX.Vector3 mPreTargetPos = SlimDX.Vector3.Zero;
        SlimDX.Vector3 mSunPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="currTime">当前日期</param>
        /// <param name="targetPos">目标位置</param>
        /// <param name="isNight">是否为夜晚</param>
        public void Tick(System.DateTime currTime, Vector3 targetPos, bool isNight)
        {
            if (Enable==false)
                return;

            if(mSunActor == null)
            {
                mSunActor = CCore.Client.MainWorldInstance.FindActor(mSunActorId) as CCore.World.LightActor;
                if (mSunActor == null)
                    return;
            }

            float nowTime = Engine.Instance.GetFrameSecondTimeFloat();
            float deltaTime = nowTime - mPreTime;

            SunProperty currSun = null;
            currSun = GetInterpolateSunProperty(currTime.Hour + currTime.Minute / 60.0 + currTime.Second / 3600.0 + currTime.Millisecond / 3600000.0);

            if (currSun != null)
            {
                var l = mSunActor.Light as CCore.Light.Light;
                if (l != null)
                {
                    l.Intensity = currSun.DiffuseIntensity;
                    l.SpecularIntensity = currSun.SpecularIntensity;
                    l.Diffuse = currSun.SunDiffuse;
                    l.Ambient = currSun.SunAmbient;
                    l.Specular = currSun.SunSpecular;
                }
            }

            // renwind modified 需要隔断时间TICK的主要是太阳的角度
            mRemainTime -= deltaTime;
            if (mRemainTime <= 0.0f)
            {
                var curTime = new DateTime(SunRiseTime.Year, SunRiseTime.Month, SunRiseTime.Day, currTime.Hour, currTime.Minute, currTime.Second);
                float initAngle = 0.0f;
                float rangeOfAngle = (float)System.Math.PI * 4.0f / 6.0f;
                if (curTime >= SunRiseTime && curTime <= SunSetTime)
                {
                    // 白天
                    // 计算当前角度
                    initAngle = (float)System.Math.PI / 6.0f;
                    SunAngle = initAngle + (float)((curTime - SunRiseTime).TotalHours / (SunSetTime - SunRiseTime).TotalHours * rangeOfAngle);
                }
                else
                {
                    // 晚上
                    initAngle = (float)System.Math.PI * 5.0f / 6.0f;
                    if (curTime < SunRiseTime)
                    {
                        var hour0 = new DateTime(SunRiseTime.Year, SunRiseTime.Month, SunRiseTime.Day, 0, 0, 0);
                        var hour24 = new DateTime(SunSetTime.Year, SunSetTime.Month, SunSetTime.Day + 1, 0, 0, 0);
                        SunAngle = (float)(initAngle - (((curTime - hour0).TotalHours + (hour24 - SunSetTime).TotalHours) / (24 - (SunSetTime - SunRiseTime).TotalHours) * rangeOfAngle));
                    }
                    else if (curTime > SunSetTime)
                    {
                        SunAngle = (float)(initAngle - ((curTime - SunSetTime).TotalHours / (24 - (SunSetTime - SunRiseTime).TotalHours) * rangeOfAngle));
                    }
                }

                // 计算太阳旋转轴
                var sunPos = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.UnitZ * 300, mSunRiseDirectionQuat);
                var riseDir = sunPos;
                riseDir.Normalize();
                var sunUpDir = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.UnitX, SlimDX.Quaternion.RotationAxis(riseDir, mSunAngleOfInclinationRadian));
                SlimDX.Vector3 sunAxis;
                if (SunRiseDirection <= 90 || SunRiseDirection >= 270)
                    sunAxis = SlimDX.Vector3.Cross(riseDir, sunUpDir);
                else
                    sunAxis = SlimDX.Vector3.Cross(sunUpDir, riseDir);
                var sunRotQuat = SlimDX.Quaternion.RotationAxis(sunAxis, SunAngle);
                sunPos = SlimDX.Vector3.TransformCoordinate(sunPos, sunRotQuat);
                mSunPos =  sunPos;
                // 计算太阳自身的旋转，保证光源方向总是指向targetPos。
                var tempAxis = SlimDX.Vector3.Cross(riseDir, SlimDX.Vector3.UnitY);
                var tempQuat = SlimDX.Quaternion.RotationAxis(tempAxis, SunAngle);
                Quaternion sunRot = mSunRiseDirectionQuat * tempQuat *
                                    Quaternion.Normalize(Quaternion.RotationAxis(tempAxis, (float)System.Math.PI / -2.0f)) *
                                    Quaternion.Normalize(Quaternion.RotationAxis(riseDir, mSunAngleOfInclinationRadian - (float)(System.Math.PI * 0.5)));

                mSunActor.Placement.SetRotation(ref sunRot);

                mRemainTime = (float)mUpdateMinuteInterval;
            }

            //// Shadow map pixel align
            //var stepPerPixel = sunLight.Light.ShadowCoverSize / sunLight.Light.ShadowMapSize;
            ////var lightMoveDistance = (targetPos - mPreTargetPos).Length();
            //var sunQuat = sunLight.Placement.GetRotation();
            //SlimDX.Quaternion sunQuatInv = new SlimDX.Quaternion();
            //SlimDX.Quaternion.Invert(ref sunQuat, out sunQuatInv);
            //var targetPosSun = SlimDX.Vector3.Transform(targetPos, sunQuat);
            //var preTargetPosSun = SlimDX.Vector3.Transform(mPreTargetPos, sunQuat);
            //var lightMoveVector = (targetPosSun - preTargetPosSun);
            //lightMoveVector.Y = 0;
            ////var lightMoveDeltaX = (targetPos - mPreTargetPos).X;
            ////var lightMoveDeltaZ = (targetPos - mPreTargetPos).Z;
            //var lightMoveDeltaX = lightMoveVector.X;
            //var lightMoveDeltaZ = lightMoveVector.Z;
            //if (System.Math.Abs(lightMoveDeltaX) >= stepPerPixel)
            //{
            //    lightMoveDeltaX = (int)(lightMoveDeltaX / stepPerPixel) * stepPerPixel;
            //}
            //else
            //    lightMoveDeltaX = 0;
            //if (System.Math.Abs(lightMoveDeltaZ) >= stepPerPixel)
            //{
            //    lightMoveDeltaZ = (int)(lightMoveDeltaZ / stepPerPixel) * stepPerPixel;
            //}
            //else
            //    lightMoveDeltaZ = 0;

            //var lightMoveDelta = new SlimDX.Vector3(lightMoveDeltaX, 0, lightMoveDeltaZ);
            //var delta = SlimDX.Vector3.Transform(lightMoveDelta, sunQuatInv);

            //targetPos.X = mPreTargetPos.X + delta.X;
            //targetPos.Z = mPreTargetPos.Z + delta.Z;

            //mPreTargetPos = targetPos;




            var sunFollowPos = mSunPos + targetPos;
            mSunActor.Placement.SetLocation(ref sunFollowPos);
            if (mSunActor.Light != null)
                mSunActor.Light.IsRenderStaticShadow = true;

            mPreTime = nowTime;
        }

        bool mEnable = true;
        /// <summary>
        /// 是否启用
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("全局")]
        [System.ComponentModel.DisplayName("启用")]
        public bool Enable
        {
            get { return mEnable; }
            set
            {
                mEnable = value;
                OnPropertyChanged("Enable");
            }
        }


        //double mTimeAxis = 0.0;
        //[CSUtility.Support.AutoSaveLoad]
        //[System.ComponentModel.Category("全局")]
        //[System.ComponentModel.DisplayName("时间轴")]
        //[EditorCommon.Assist.UIEditor_ValueWithRange(0.0, 1.0)]
        //public double TimeAxis
        //{
        //    get { return mTimeAxis; }
        //    set
        //    {
        //        mTimeAxis = (float)value;
        //        OnPropertyChanged("TimeAxis");
        //    }
        //}

        Guid mId = Guid.NewGuid();
        /// <summary>
        /// 对象ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Browsable(false)]
        public Guid Id
        {
            get { return mId; }
            set
            {
                mId = value;
                OnPropertyChanged("Id");
            }
        }

        // 阴影类型由每个添加到天气系统的灯光设置各自的属性，不再进行统一设置
        //CCore.Light.EShadowType mShadowType = CCore.Light.EShadowType.Standard;
        //[CSUtility.Support.AutoSaveLoad]
        //[System.ComponentModel.Category("全局")]
        //[System.ComponentModel.DisplayName("阴影类型")]
        //public CCore.Light.EShadowType ShadowType
        //{
        //    get { return mShadowType; }
        //    set
        //    {
        //        mShadowType = value;

        //        var world = CCore.IClient.MainWorldInstance as CCore.Scene.CellScene;
        //        if (world != null)
        //        {
        //            world.SunActor.Light.ShadowType = value;
        //        }

        //        OnPropertyChanged("ShadowType");
        //    }
        //}

        string mName = "";
        /// <summary>
        /// 对象名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("全局")]
        [System.ComponentModel.DisplayName("名称")]
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;
                OnPropertyChanged("Name");
            }
        }

        double mUpdateMinuteInterval = 0.1;
        /// <summary>
        /// 属性更新速率
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("全局")]
        [System.ComponentModel.DisplayName("属性更新速率")]
        [CSUtility.Editor.Editor_ValueWithRange(0.01, 10)]
        public double UpdateMinuteInterval
        {
            get { return mUpdateMinuteInterval; }
            set
            {
                mUpdateMinuteInterval = (float)value;
                OnPropertyChanged("UpdateMinuteInterval");
            }
        }
        public double mSaveUpdateMinuteInterval = -1;

        // 日出时间
        DateTime mSunRiseTime = new System.DateTime(2014, 1, 1, 7, 0, 0);
        /// <summary>
        /// 日出时间
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Browsable(false)]
        public DateTime SunRiseTime
        {
            get { return mSunRiseTime; }
            set
            {
                mSunRiseTime = value;
                OnPropertyChanged("SunRiseTime");
            }
        }
        // 日落时间
        DateTime mSunSetTime = new System.DateTime(2014, 1, 1, 19, 0, 0);
        /// <summary>
        /// 日落时间
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Browsable(false)]        
        public DateTime SunSetTime
        {
            get { return mSunSetTime; }
            set
            {
                mSunSetTime = value;
                OnPropertyChanged("SunSetTime");
            }
        }

        // 日出方向（弧度）
        SlimDX.Quaternion mSunRiseDirectionQuat = SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitY, 0);
        // 日出方向（角度单位为度）
        float mSunRiseDirection = 0;
        /// <summary>
        /// 日出方向（角度单位为度）
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("全局")]
        [System.ComponentModel.DisplayName("日出方向")]
        [CSUtility.Editor.Editor_Angle360Setter]
        public float SunRiseDirection
        {
            get { return mSunRiseDirection; }
            set
            {
                mSunRiseDirection = value;

                mSunRiseDirectionQuat = SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitY, (float)(mSunRiseDirection / 180 * System.Math.PI));

                OnPropertyChanged("SunRiseDirection");
            }
        }

        // 太阳偏斜角度（弧度）
        float mSunAngleOfInclinationRadian = 1.571f;
        // 太阳偏斜角度（角度单位为度）
        float mSunAngleOfInclination = 90;
        /// <summary>
        /// 太阳倾斜角度
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Category("全局")]
        [System.ComponentModel.DisplayName("太阳倾斜角度")]
        [CSUtility.Editor.Editor_Angle180Setter]
        public float SunAngleOfInclination
        {
            get { return mSunAngleOfInclination; }
            set
            {
                mSunAngleOfInclination = value;

                mSunAngleOfInclinationRadian = (float)(mSunAngleOfInclination / 180 * System.Math.PI);

                OnPropertyChanged("SunAngleOfInclination");
            }
        }

        //// 月亮方向（弧度）
        //SlimDX.Quaternion mMoonDirectionQuat = SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitY, 0);
        //// 月亮方向（角度单位为度）
        //float mMoonDirection = 0;
        //[CSUtility.Support.AutoSaveLoad]
        //[System.ComponentModel.Category("全局")]
        //[System.ComponentModel.DisplayName("月亮方向")]
        //[EditorCommon.Assist.Editor_Angle360Setter]
        //public float MoonDirection
        //{
        //    get { return mMoonDirection; }
        //    set
        //    {
        //        mMoonDirection = value;

        //        mMoonDirectionQuat = SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitY, (float)(mMoonDirection / 180 * System.Math.PI));

        //        OnPropertyChanged("MoonDirection");
        //    }
        //}

        //// 月亮角度（弧度）
        //float mMoonAngleRadian = 1.571f;
        //// 月亮角度（度）
        //float mMoonAngle = 90;
        //[CSUtility.Support.AutoSaveLoad]
        //[System.ComponentModel.Category("全局")]
        //[System.ComponentModel.DisplayName("月亮角度")]
        //[EditorCommon.Assist.Editor_Angle180Setter]
        //public float MoonAngle
        //{
        //    get { return mMoonAngle; }
        //    set
        //    {
        //        mMoonAngle = value;
        //        mMoonAngleRadian = (float)(mMoonAngle / 180 * System.Math.PI);
        //        OnPropertyChanged("MoonAngle");
        //    }
        //}

        List<SunProperty> mSunProertiesByHour = new List<SunProperty>();
        /// <summary>
        /// 太阳属形改变间隔时间
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.Browsable(false)]
        public List<SunProperty> SunProertiesByHour
        {
            get { return mSunProertiesByHour; }
            set
            {
                mSunProertiesByHour = value;
            }
        }
        /// <summary>
        /// 获取最大的光照强度
        /// </summary>
        /// <returns>返回最大的光照强度</returns>
        public double GetMaxLuminance()
        {
            double maxLuminance = 0;
            foreach (var sunProperty in SunProertiesByHour)
            {
                if (maxLuminance < sunProperty.DiffuseLuminance)
                    maxLuminance = sunProperty.DiffuseLuminance;
                if (maxLuminance < sunProperty.AmbientLuminance)
                    maxLuminance = sunProperty.AmbientLuminance;
                if (maxLuminance < sunProperty.SpecularLuminance)
                    maxLuminance = sunProperty.SpecularLuminance;
            }
            if (maxLuminance == 0)
                maxLuminance = 10;

            return maxLuminance;
        }
        /// <summary>
        /// 从文件中加载对象
        /// </summary>
        /// <param name="absFileName">文件的绝对路径</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public bool Load(string absFileName)
        {
            //string filename = path + "Illumination.dat";
            var holder = CSUtility.Support.XndHolder.LoadXND(absFileName);
            if (holder == null || holder.Node == null)
                return false;

            var commAttr = holder.Node.FindAttrib("Common");
            commAttr.BeginRead();
            Read(commAttr);
            commAttr.EndRead();

            holder.Node.TryReleaseHolder();

            return true;
        }
        /// <summary>
        /// 保存对象到文件
        /// </summary>
        /// <param name="absFileName">文件的绝对路径</param>
        /// <returns>保存成功返回true，否则返回返回false</returns>
        public bool Save(string absFileName)
        {
            //string filename = path + "Illumination.dat";
            absFileName = absFileName.ToLower();
            var holder = CSUtility.Support.XndHolder.NewXNDHolder();

            var commAttr = holder.Node.AddAttrib("Common");
            commAttr.BeginWrite();
            Write(commAttr);
            commAttr.EndWrite();

            CSUtility.Support.XndHolder.SaveXND(absFileName, holder);

            return true;
        }
        /// <summary>
        /// 转变为string类型
        /// </summary>
        /// <returns>返回string类型</returns>
        public override string ToString()
        {
            return Name;
        }
        /// <summary>
        /// 获取光强的文件路径
        /// </summary>
        /// <param name="world">当前对象所在的世界</param>
        /// <param name="id">对象ID</param>
        /// <returns>返回对象所在文件路径</returns>
        public static string GetIlluminationAbsFile(CCore.World.World world, Guid id)
        {
            var worldAbsFolder = world.GetWorldLastLoadedAbsFolder("");
            if (string.IsNullOrEmpty(worldAbsFolder))
                return "";
            return worldAbsFolder + "/Illumination/" + id.ToString() + CSUtility.Support.IFileConfig.IlluminationExtension;
        }
        /// <summary>
        /// 获取光强所在的文件夹绝对路径
        /// </summary>
        /// <param name="world">对象所在的世界</param>
        /// <returns>返回对象所在的文件夹绝对路径</returns>
        public static string GetIlluminationAbsFolder(CCore.World.World world)
        {
            var worldAbsFolder = world.GetWorldLastLoadedAbsFolder("");
            if (string.IsNullOrEmpty(worldAbsFolder))
                return "";
            return worldAbsFolder + "/Illumination";
        }
    }
}
