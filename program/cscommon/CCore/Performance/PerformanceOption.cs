using System;
using System.ComponentModel;

namespace CCore.Performance
{
    /// <summary>
    /// 声音类型枚举
    /// </summary>
    public enum ESoundType : uint
    {
        MusicSound, //音乐
        SoundEffect,//音效
        Environment,//环境
    }
    /// <summary>
    /// 窗口模式的枚举
    /// </summary>
    public enum EViewMode
    {
        Window = 0,
        WindowMax = 1,
    }
    /// <summary>
    /// 抗锯齿的类型枚举
    /// </summary>
    public enum EAntiAliasing
    {
        None = 0,
        LowFXAA = 1,
        HighFXAA = 2,
        BestFXAA = 3,
    }
    /// <summary>
    /// 锐化枚举
    /// </summary>
    public enum ESharpen            // 锐化
    {
        None = 0,                           // 无
        Normal = 1,                        // 普通
        //Better = 2,                         // 优良
    }
    /// <summary>
    /// 材质分辨率的枚举
    /// </summary>
    public enum EMaterialResolution
    {
        //Low = 0,
        Normal = 0,
        //Better = 2,
        Higher = 1,
    }
    /// <summary>
    /// 材质过滤类型的枚举
    /// </summary>
    public enum EMaterialFilter
    {
        Point = 0,
        BiLinear = 1,
        TriLinear = 2,
        Anisotropy2x = 3,
        //Anisotropy4x = 4,
        //Anisotropy8x = 5,
        //Anisotropy16x = 6,
    }
    /// <summary>
    /// 阴影质量的枚举
    /// </summary>
    public enum EShadowQuality
    {
        None = 0,
        Low = 1,
        Normal = 2,
        //Better = 2,
        Higher = 3,
        //Best = 4,
    }
    /// <summary>
    /// 粒子密度
    /// </summary>
    public enum EParticleDensity
    {
        Low = 0,
        Normal = 1,
        //Better = 2,
        Higher = 2,
        //Best = 4,
    }
    /// <summary>
    /// 高级光照效果的渲染品质
    /// </summary>
    public enum ESSAO
    {
        None = 0,
        Normal = 1,
        Better = 2,
    }
    /// <summary>
    /// 环境细节枚举
    /// </summary>
    public enum EEnvDetails
    {
        Low = 0,
        Normal = 1,
        //Better = 2,
        Higher = 2,
        //Best = 4,
    }
    /// <summary>
    /// 场景细节枚举
    /// </summary>
    public enum ESceneDetails
    {
        Low = 0,
        Normal = 1,
        //Better = 2,
        Higher = 2,
        //Best = 4,
    }
    /// <summary>
    /// 分辨率
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class Resolution
    {
        int mW = 1280;
        /// <summary>
        /// 宽，默认为1280像素
        /// </summary>
        [CSUtility.Support.DataValueAttribute("W")]
        public int W
        {
            get { return mW; }
            set { mW = value; }
        }
        int mH = 800;
        /// <summary>
        /// 高，默认为800像素
        /// </summary>
        [CSUtility.Support.DataValueAttribute("H")]
        public int H
        {
            get { return mH; }
            set { mH = value; }
        }
    };
    /// <summary>
    /// 性能选项模板
    /// </summary>
    [CSUtility.Data.DataTemplate(".po", "PerformanceOptions")]
    //[CSUtility.Editor.CDataEditorAttribute(".po")]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class PerformanceOptionsTemplate : CSUtility.Data.IDataTemplateBase<UInt16>
    {
        public static PerformanceOptionsTemplate CurrentPerformOptions = null;

        /// <summary>
        /// 数据模板版本号
        /// </summary>
        [ReadOnly(false)]
        [CSUtility.Support.DataValueAttribute("Version")]
        public UInt32 Version
        {
            get;
            set;
        }
        
        /// <summary>
        /// 性能选项名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Name")]
        [System.ComponentModel.DisplayName("性能选项名称")]
        [System.ComponentModel.Description("性能选项名称")]
        [System.ComponentModel.Category("显示")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 数据模板置藏标志，共编辑器使用
        /// </summary>
        [Browsable(false)]
        public bool IsDirty
        {
            get;
            set;
        }

        #region 显示
        UInt16 mId = UInt16.MaxValue;
        /// <summary>
        /// 性能选项ID
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Id")]
        [System.ComponentModel.DisplayName("性能选项ID")]
        [System.ComponentModel.Description("性能选项ID")]
        [System.ComponentModel.Category("显示")]
        public UInt16 Id
        {
            get { return mId; }
            set { mId = value; }
        }
        EViewMode mViewMode = EViewMode.Window;
        /// <summary>
        /// 显示模式
        /// </summary>
        [CSUtility.Support.DataValueAttribute("ViewMode")]
        [System.ComponentModel.DisplayName("显示模式")]
        [System.ComponentModel.Description("使你可以更改游戏的主要显示模式：全屏幕、窗口、窗口（最大化），窗口模式可能会导致运行速度下降")]
        [System.ComponentModel.Category("显示")]
        public EViewMode ViewMode
        {
            get { return mViewMode; }
            set { mViewMode = value; }
        }
        /// <summary>
        /// 显示模式预览
        /// </summary>
        public EViewMode mPreViewMode = EViewMode.Window;

        EAntiAliasing mAntiAliasing = EAntiAliasing.LowFXAA;
        /// <summary>
        /// 抗锯齿
        /// </summary>
        [CSUtility.Support.DataValueAttribute("AntiAliasing")]
        [System.ComponentModel.DisplayName("抗锯齿")]
        [System.ComponentModel.Description("抗锯齿技术可使参差不齐的边缘更加平滑，在低分辨率时效果更佳，但这会导致图像略微模糊。开启抗锯齿将降低游戏运行速度")]
        [System.ComponentModel.Category("显示")]
        public EAntiAliasing AntiAliasing
        {
            get { return mAntiAliasing; }
            set { mAntiAliasing = value; }
        }

        ESharpen mSharpen = ESharpen.None;
        /// <summary>
        /// 锐化类型
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Sharpen")]
        [System.ComponentModel.DisplayName("锐化")]
        [System.ComponentModel.Description("锐化技术可使画面更加清晰锐利，但这会导致图像略微模糊。开启锐化将降低游戏运行速度")]
        [System.ComponentModel.Category("显示")]
        public ESharpen Sharpen
        {
            get { return mSharpen; }
            set { mSharpen = value; }
        }

        bool mVerticalSync = false;
        /// <summary>
        /// 是否开启垂直同步，默认为false
        /// </summary>
        [CSUtility.Support.DataValueAttribute("VerticalSync")]
        [System.ComponentModel.DisplayName("垂直同步")]
        [System.ComponentModel.Description("将你的游戏帧数与显示器刷新率同步，可以解决游戏中图像无法正常显示的问题")]
        [System.ComponentModel.Category("显示")]
        public bool VerticalSync
        {
            get { return mVerticalSync; }
            set { mVerticalSync = value; }
        }

        Resolution mResolution = new Resolution();
        /// <summary>
        /// 分辨率
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Resolution")]
        [System.ComponentModel.DisplayName("分辨率")]
        [System.ComponentModel.Description("高分辨率能使显示更加清晰，但会极大影响游戏运行速度，选择一个宽高比适合于你显示器的分辨率。")]
        [System.ComponentModel.Category("显示")]
        public Resolution Resolution
        {
            get { return mResolution; }
            set { mResolution = value; }
        }
        #endregion

        public bool CopyFrom(CSUtility.Support.ICopyable src)
        {
            return CSUtility.Support.Copyable.CopyFrom(src, this);
        }

        #region 材质
        EMaterialResolution mMaterialResolution;
        /// <summary>
        /// 材质分辨率
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MaterialResolution")]
        [System.ComponentModel.DisplayName("材质分辨率")]
        [System.ComponentModel.Description("调节所有材质的细节等级，降低此项可以略微提高运行速度")]
        [System.ComponentModel.Category("材质")]
        public EMaterialResolution MaterialResolution
        {
            get { return mMaterialResolution; }
            set { mMaterialResolution = value; }
        }

        float mMaterialPrecision;
        /// <summary>
        /// 材质精度
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MaterialPrecision")]
        [System.ComponentModel.DisplayName("材质精度")]
        [System.ComponentModel.Description("调节所有材质的细节等级，降低此项可以略微提高运行速度")]
        [System.ComponentModel.Category("材质")]
        public float MaterialPrecision
        {
            get { return mMaterialPrecision; }
            set { mMaterialPrecision = value; }
        }

        EMaterialFilter mMaterialFilter;
        /// <summary>
        /// 材质过滤
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MaterialFilter")]
        [System.ComponentModel.DisplayName("材质过滤")]
        [System.ComponentModel.Description("提高材质的锐度，尤其是以倾斜视角观察的材质，降低此项可以提高运行速度。")]
        [System.ComponentModel.Category("材质")]
        public EMaterialFilter MaterialFilter
        {
            get { return mMaterialFilter; }
            set { mMaterialFilter = value; }
        }
        #endregion

        #region 效果
        EShadowQuality mShadowQuality;
        /// <summary>
        /// 阴影质量
        /// </summary>
        [CSUtility.Support.DataValueAttribute("ShadowQuality")]
        [System.ComponentModel.DisplayName("阴影质量")]
        [System.ComponentModel.Description("控制阴影渲染的方式和质量，降低此项可以大幅提高运行速度。")]
        [System.ComponentModel.Category("效果")]
        public EShadowQuality ShadowQuality
        {
            get { return mShadowQuality; }
            set { mShadowQuality = value; }
        }

        EParticleDensity mParticleDensity;
        /// <summary>
        /// 粒子密度
        /// </summary>
        [CSUtility.Support.DataValueAttribute("ParticleDensity")]
        [System.ComponentModel.DisplayName("粒子密度")]
        [System.ComponentModel.Description("控制法术、火焰等效果中使用的粒子数量，降低此项可以提高游戏运行速度。")]
        [System.ComponentModel.Category("效果")]
        public EParticleDensity ParticleDensity
        {
            get { return mParticleDensity; }
            set { mParticleDensity = value; }
        }

        float mParticleEffectDensity;
        /// <summary>
        /// 粒子特效密度
        /// </summary>
        [CSUtility.Support.DataValueAttribute("ParticleEffectDensity")]
        [System.ComponentModel.DisplayName("粒子密度")]
        [System.ComponentModel.Description("控制法术、火焰等效果中使用的粒子数量，降低此项可以提高游戏运行速度。")]
        [System.ComponentModel.Category("效果")]
        public float ParticleEffectDensity
        {
            get { return mParticleEffectDensity; }
            set { mParticleEffectDensity = value; }
        }

        ESSAO mSSAO;
        /// <summary>
        /// 高级光照效果的渲染品质
        /// </summary>
        [CSUtility.Support.DataValueAttribute("SSAO")]
        [System.ComponentModel.DisplayName("SSAO")]
        [System.ComponentModel.Description("控制高级光照效果的渲染品质，降低此项可以大幅提高游戏运行速度。")]
        [System.ComponentModel.Category("效果")]
        public ESSAO SSAO
        {
            get { return mSSAO; }
            set { mSSAO = value; }
        }
        #endregion

        #region 环境 
        ESceneDetails mSceneDetails;
        /// <summary>
        /// 环境细节
        /// </summary>
        [CSUtility.Support.DataValueAttribute("SceneDetails")]
        [System.ComponentModel.DisplayName("环境细节")]
        [System.ComponentModel.Description("景观物体（如地表植被）的显示密度和距离，降低此项可以提高游戏运行速度。")]
        [System.ComponentModel.Category("环境")]
        public ESceneDetails SceneDetails
        {
            get { return mSceneDetails; }
            set { mSceneDetails = value; }
        }
        #endregion

        #region 音效

        bool mIsOpenVoice = true;
        /// <summary>
        /// 是否开启声音，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsOpenVoice")]
        [System.ComponentModel.DisplayName("是否开启声音")]
        [System.ComponentModel.Description("控制游戏所有声音是否开启")]
        [System.ComponentModel.Category("音效")]
        public bool IsOpenVoice
        {
            get { return mIsOpenVoice; }
            set { mIsOpenVoice = value; }
        }

        float mMainVolume = 1;
        /// <summary>
        /// 主音量大小，默认为1
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MainVolume")]
        [System.ComponentModel.DisplayName("主音量大小")]
        [System.ComponentModel.Description("控制游戏所有声音音量大小")]
        [System.ComponentModel.Category("音效")]
        public float MainVolume
        {
            get { return mMainVolume; }
            set { mMainVolume = value; }
        }

        bool mIsOpenMusic = true;
        /// <summary>
        /// 是否开启音乐，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsOpenMusic")]
        [System.ComponentModel.DisplayName("是否开启音乐")]
        [System.ComponentModel.Description("控制游戏音乐是否开启")]
        [System.ComponentModel.Category("音效")]
        public bool IsOpenMusic
        {
            get { return mIsOpenMusic; }
            set { mIsOpenMusic = value; }
        }

        float mMusicVolume = 1;
        /// <summary>
        /// 音乐音量大小，默认为1
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MusicVolume")]
        [System.ComponentModel.DisplayName("音乐音量大小")]
        [System.ComponentModel.Description("控制游戏音乐音量大小")]
        [System.ComponentModel.Category("音效")]
        public float MusicVolume
        {
            get { return mMusicVolume; }
            set { mMusicVolume = value; }
        }

        bool mIsOpenSoundEffect = true;
        /// <summary>
        /// 是否开启音效，默认为1
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsOpenSoundEffect")]
        [System.ComponentModel.DisplayName("是否开启音效")]
        [System.ComponentModel.Description("控制游戏音效是否开启")]
        [System.ComponentModel.Category("音效")]
        public bool IsOpenSoundEffect
        {
            get { return mIsOpenSoundEffect; }
            set { mIsOpenSoundEffect = value; }
        }

        float mSoundEffectVolume = 1;
        /// <summary>
        /// 音效音量大小，默认为1
        /// </summary>
        [CSUtility.Support.DataValueAttribute("SoundEffectVolume")]
        [System.ComponentModel.DisplayName("音效音量大小")]
        [System.ComponentModel.Description("控制游戏音效音量大小")]
        [System.ComponentModel.Category("音效")]
        public float SoundEffectVolume
        {
            get { return mSoundEffectVolume; }
            set { mSoundEffectVolume = value; }
        }

        float mEnvironmentVolume = 1;
        /// <summary>
        /// 环境音量大小，默认为1
        /// </summary>
        [CSUtility.Support.DataValueAttribute("EnvironmentVolume")]
        [System.ComponentModel.DisplayName("环境音量大小")]
        [System.ComponentModel.Description("控制游戏环境音量大小")]
        [System.ComponentModel.Category("音效")]
        public float EnvironmentVolume
        {
            get { return mEnvironmentVolume; }
            set { mEnvironmentVolume = value; }
        }

        #endregion

        #region 个人

        bool mIsCanTrading = true;
        /// <summary>
        /// 是否允许交易，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsCanTrading")]
        [System.ComponentModel.DisplayName("是否允许交易")]
        [System.ComponentModel.Description("是否允许交易")]
        [System.ComponentModel.Category("个人")]
        public bool IsCanTrading
        {
            get { return mIsCanTrading; }
            set { mIsCanTrading = value; }
        }

        bool mIsShowRoleHelmet = true;
        /// <summary>
        /// 是否显示角色头盔，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsShowRoleHelmet")]
        [System.ComponentModel.DisplayName("是否显示角色头盔")]
        [System.ComponentModel.Description("是否显示角色头盔")]
        [System.ComponentModel.Category("个人")]
        public bool IsShowRoleHelmet
        {
            get { return mIsShowRoleHelmet; }
            set { mIsShowRoleHelmet = value; }
        }

        bool mIsShowMonsterName = true;
        /// <summary>
        /// 是否显示怪物名字，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsShowMonsterName")]
        [System.ComponentModel.DisplayName("是否显示怪物名字")]
        [System.ComponentModel.Description("是否显示怪物名字")]
        [System.ComponentModel.Category("个人")]
        public bool IsShowMonsterName
        {
            get { return mIsShowMonsterName; }
            set { mIsShowMonsterName = value; }
        }

        bool mIsShowRoleData = true;
        /// <summary>
        /// 是否显示角色数值，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsShowRoleData")]
        [System.ComponentModel.DisplayName("是否显示角色数值")]
        [System.ComponentModel.Description("是否显示角色数值")]
        [System.ComponentModel.Category("个人")]
        public bool IsShowRoleData
        {
            get { return mIsShowRoleData; }
            set { mIsShowRoleData = value; }
        }

        bool mIsShowOtherPlayerName = true;
        /// <summary>
        /// 是否显示其他玩家名字，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsShowOtherPlayerName")]
        [System.ComponentModel.DisplayName("是否显示其他玩家名字")]
        [System.ComponentModel.Description("是否显示其他玩家名字")]
        [System.ComponentModel.Category("个人")]
        public bool IsShowOtherPlayerName
        {
            get { return mIsShowOtherPlayerName; }
            set { mIsShowOtherPlayerName = value; }
        }

        bool mIsShowMyGuardName = true;
        /// <summary>
        /// 是否显示自己守护的名字，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsShowMyGuardName")]
        [System.ComponentModel.DisplayName("是否显示自己守护的名字")]
        [System.ComponentModel.Description("是否显示自己守护的名字")]
        [System.ComponentModel.Category("个人")]
        public bool IsShowMyGuardName
        {
            get { return mIsShowMyGuardName; }
            set { mIsShowMyGuardName = value; }
        }

        bool mIsShowHurtNumber = true;
        /// <summary>
        /// 是否显示伤害数字，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsShowHurtNumber")]
        [System.ComponentModel.DisplayName("是否显示伤害数字")]
        [System.ComponentModel.Description("是否显示伤害数字")]
        [System.ComponentModel.Category("个人")]
        public bool IsShowHurtNumber
        {
            get { return mIsShowHurtNumber; }
            set { mIsShowHurtNumber = value; }
        }

        bool mIsShowDropItemName = true;
        /// <summary>
        /// 是否显示掉落物品名字，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsShowDropItemName")]
        [System.ComponentModel.DisplayName("是否显示掉落物品名字")]
        [System.ComponentModel.Description("是否显示掉落物品名字")]
        [System.ComponentModel.Category("个人")]
        public bool IsShowDropItemName
        {
            get { return mIsShowDropItemName; }
            set { mIsShowDropItemName = value; }
        }

        bool mIsShowStallRoleModel = true;
        /// <summary>
        /// 是否显示摆摊造型，默认为true
        /// </summary>
        [CSUtility.Support.DataValueAttribute("IsShowStallRoleModel")]
        [System.ComponentModel.DisplayName("是否显示摆摊造型")]
        [System.ComponentModel.Description("是否显示摆摊造型")]
        [System.ComponentModel.Category("个人")]
        public bool IsShowStallRoleModel
        {
            get { return mIsShowStallRoleModel; }
            set { mIsShowStallRoleModel = value; }
        }

        #endregion

        #region 函数
        /// <summary>
        /// 获取当前的窗口模式
        /// </summary>
        /// <param name="type">窗口模式</param>
        /// <returns>返回当前窗口模式的描述</returns>
        public string GetStringViewMode(EViewMode type)
        {
            switch (type)
            {
                case EViewMode.Window:
                    {
                        return "窗口";
                    }
                case EViewMode.WindowMax:
                    {
                        return "窗口最大化";
                    }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取当前抗锯齿的质量
        /// </summary>
        /// <param name="type">抗锯齿质量</param>
        /// <returns>返回抗锯齿的质量</returns>
        public string GetStringAntiAliasing(EAntiAliasing type)
        {
            switch (type)
            {
                case EAntiAliasing.BestFXAA:
                    {
                        return "极佳";
                    }
                case EAntiAliasing.HighFXAA:
                    {
                        return "高";
                    }
                case EAntiAliasing.LowFXAA:
                    {
                        return "低";
                    }
                case EAntiAliasing.None:
                    {
                        return "无";
                    }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取当前材质过滤的类型
        /// </summary>
        /// <param name="type">材质过滤的类型</param>
        /// <returns>返回材质过滤的类型描述</returns>
        public string GetStringMaterialFilter(EMaterialFilter type)
        {
            switch (type)
            {
                case EMaterialFilter.BiLinear:
                    {
                        return "双线性";
                    }
                case EMaterialFilter.TriLinear:
                    {
                        return "三线性";
                    }
                case EMaterialFilter.Anisotropy2x:
                    {
                        return "各向异性";
                    }
                    //case EMaterialFilter.Anisotropy4x:
                    //    {
                    //        return "4x各向异性";
                    //    }
                    //case EMaterialFilter.Anisotropy8x:
                    //    {
                    //        return "8x各向异性";
                    //    }
                    //case EMaterialFilter.Anisotropy16x:
                    //    {
                    //        return "16x各向异性";
                    //    }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取阴影质量
        /// </summary>
        /// <param name="type">阴影质量</param>
        /// <returns>返回阴影的质量信息</returns>
        public string GetStringShadowQuality(EShadowQuality type)
        {
            switch (type)
            {
                case EShadowQuality.None:
                    {
                        return "无";
                    }
                case EShadowQuality.Normal:
                    {
                        return "普通";
                    }
                case EShadowQuality.Low:
                    {
                        return "低";
                    }
                case EShadowQuality.Higher:
                    {
                        return "高";
                    }
                    //case EShadowQuality.Better:
                    //    {
                    //        return "良好";
                    //    }
                    //case EShadowQuality.Best:
                    //    {
                    //        return "极佳";
                    //    }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取材质的分辨率类型
        /// </summary>
        /// <param name="type">材质分辨率</param>
        /// <returns>返回材质分辨率的类型描述</returns>
        public string GetStringMaterialResolution(EMaterialResolution type)
        {
            switch (type)
            {
                case EMaterialResolution.Normal:
                    {
                        return "普通";
                    }
                //case EMaterialResolution.Low:
                //    {
                //        return "低";
                //    }
                case EMaterialResolution.Higher:
                    {
                        return "优良";
                    }
                    //case EMaterialResolution.Better:
                    //    {
                    //        return "良好";
                    //    }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取当前高级光照质量
        /// </summary>
        /// <param name="type">高级光照的质量类型</param>
        /// <returns>返回高级光照质量的类型描述</returns>
        public string GetStringSSAO(ESSAO type)
        {
            switch (type)
            {
                case ESSAO.None:
                    {
                        return "无";
                    }
                case ESSAO.Normal:
                    {
                        return "普通";
                    }
                case ESSAO.Better:
                    {
                        return "极佳";
                    }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取粒子密度
        /// </summary>
        /// <param name="type">粒子密度类型</param>
        /// <returns>返回粒子密度的类型描述</returns>
        public string GetStringParticleDensity(EParticleDensity type)
        {
            switch (type)
            {
                case EParticleDensity.Normal:
                    {
                        return "普通";
                    }
                case EParticleDensity.Low:
                    {
                        return "低";
                    }
                case EParticleDensity.Higher:
                    {
                        return "高";
                    }
                    //case EParticleDensity.Better:
                    //    {
                    //        return "良好";
                    //    }
                    //case EParticleDensity.Best:
                    //    {
                    //        return "极佳";
                    //    }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取场景细节的类型
        /// </summary>
        /// <param name="type">场景细节的类型</param>
        /// <returns>返回场景细节的类型描述</returns>
        public string GetStringSceneDetails(ESceneDetails type)
        {
            switch (type)
            {
                case ESceneDetails.Normal:
                    {
                        return "普通";
                    }
                case ESceneDetails.Low:
                    {
                        return "低";
                    }
                case ESceneDetails.Higher:
                    {
                        return "高";
                    }
                    //case ESceneDetails.Better:
                    //    {
                    //        return "良好";
                    //    }
                    //case ESceneDetails.Best:
                    //    {
                    //        return "极佳";
                    //    }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取锐化类型
        /// </summary>
        /// <param name="type">锐化类型</param>
        /// <returns>返回锐化类型的描述</returns>
        public string GetStringSharpenQuality(ESharpen type)
        {
            switch (type)
            {
                case ESharpen.None:
                    {
                        return "关闭";
                    }
                case ESharpen.Normal:
                    {
                        return "开启";
                    }
                    //case ESharpen.Better:
                    //    {
                    //        return "良好";
                    //    }
            }
            return string.Empty;
        }
        #endregion
    }
}
