using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 图像的后处理方式枚举
    /// </summary>
    public enum enPostProcessType : int
    {
        SSAO,
        Bloom,
        DepthFog,
        DOF,
        HDR,
        ColorGrading,
        Sharpen
    }
    /// <summary>
    /// 模糊处理方式枚举
    /// </summary>
    public enum enBlurType : int
    {
        BoxBlur,
        Gaussian
    }
    /// <summary>
    /// 后处理类
    /// </summary>
    public class PostProcess : CSUtility.Support.XndSaveLoadProxy
    {
        /// <summary>
        /// 后处理实例的指针
        /// </summary>
        public IntPtr mPostProcess; // vSimulation.vPostProcess
        /// <summary>
        /// 后处理实例的类型
        /// </summary>
        public enPostProcessType m_Type;
        /// <summary>
        /// 当前插值的环境
        /// </summary>
        public REnviroment mOwnerEnv = null;
        /// <summary>
        /// 后处理的名称
        /// </summary>
        public string mName;
        /// <summary>
        /// 后处理的名称
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string Name
        {
            get { return mName; }
        }
        /// <summary>
        /// 是否开启后处理
        /// </summary>
        [System.ComponentModel.DisplayName("开启")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool Enable
        {
            get
            {
                unsafe
                {
                    return (DllImportAPI.PostProcess_GetEnable(mPostProcess) != 0) ? true : false;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_SetEnable(mPostProcess, value);
                }
            }
        }

        float mLerpValue = 1;
        /// <summary>
        /// 两个PostProcess之间过度，使用的Lerp值
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public float LerpValue                          // 两个PostProcess之间过度，使用的Lerp值
        {
            get
            {
                unsafe
                {
                    return mLerpValue;
                }
            }
            set
            {
                unsafe
                {
                    mLerpValue = value;
                    DllImportAPI.PostProcess_SetLerpValue(mPostProcess, value);        // 向Shader中传值
                }
            }
        }
        /// <summary>
        /// 是否使用Lerp值
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool DoLerp
        {
            get
            {
                unsafe
                {
                    var val = DllImportAPI.PostProcess_GetDoLerp(mPostProcess);
                    if (val == 0.0f)
                        return false;
                    return true;
                }
            }
            set
            {
                unsafe
                {
                    if (value)
                        DllImportAPI.PostProcess_SetDoLerp(mPostProcess, 1);
                    else
                        DllImportAPI.PostProcess_SetDoLerp(mPostProcess, 0);
                }
            }
        }
        /// <summary>
        /// 两个PostProcess之间过度时间，默认为2000ms
        /// </summary>
        public long mLerpDuration = 2000;
        /// <summary>
        /// 使用的Lerp值的持续时间默认为0
        /// </summary>
        public long mLerpTime = 0;
        /// <summary>
        /// 清除该对象，释放对象指针
        /// </summary>
        public virtual void Cleanup()
        {

        }
        /// <summary>
        /// 在后处理时进行线性插值
        /// </summary>
        /// <param name="srcP">需要进行插值处理的实例</param>
        /// <param name="lerpDuration">两个线性插值之间的间隔</param>
        /// <param name="lerpValue">插值的大小</param>
        public virtual void LerpFrom(PostProcess srcP, long lerpDuration, float lerpValue)
        {
            DoLerp = true;
            LerpValue = lerpValue;
            mLerpDuration = lerpDuration;
            mLerpTime = (long)(lerpValue * lerpDuration);            
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public virtual void Tick(long elapsedMillisecond)
        {
            mLerpTime += elapsedMillisecond;

            if (mLerpTime > mLerpDuration)
            {
                DoLerp = false;
            }
            else
            {
                var s = (float)mLerpTime / (float)mLerpDuration;
                LerpValue = s;
            }
        }
        /// <summary>
        /// 线性插值计算
        /// </summary>
        /// <param name="x">线性插值计算的起始点</param>
        /// <param name="y">线性插值计算的终点</param>
        /// <param name="s">插值大小</param>
        /// <returns>返回计算好的插值大小</returns>
        public double Lerp( double x, double y, float s )
        {
            return x * (1 - s) + y * s;
        }
    }

}
