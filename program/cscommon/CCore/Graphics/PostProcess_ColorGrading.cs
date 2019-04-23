using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 后期特效的着色板
    /// </summary>
    public class PostProcess_ColorGrading : PostProcess
    {
        /// <summary>
        /// 调色板的名称
        /// </summary>
        public string mColorGradingTexName;
        /// <summary>
        /// 是否开启抗锯齿
        /// </summary>
        [System.ComponentModel.DisplayName("开启FXAA")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool EnableFXAA
        {
            get
            {
                unsafe
                {
                    var val = DllImportAPI.PostProcess_ColorGrading_GetEnableFXAA(mPostProcess);
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
                        DllImportAPI.PostProcess_ColorGrading_SetEnableFXAA(mPostProcess, 1);
                    else
                        DllImportAPI.PostProcess_ColorGrading_SetEnableFXAA(mPostProcess, 0);
                }
            }
        }
        /// <summary>
        /// 图像保真度
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public CCore.Performance.EAntiAliasing AAQuality
        {
            get
            {
                unsafe
                {
                    return (CCore.Performance.EAntiAliasing)DllImportAPI.PostProcess_ColorGrading_GetFXAAQuality(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ColorGrading_SetFXAAQuality(mPostProcess, (int)value);
                }
            }
        }
        /// <summary>
        /// 是否启用着色板
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool EnableColorGrading
        {
            get
            {
                unsafe
                {
                    var val = DllImportAPI.PostProcess_ColorGrading_GetEnableColorGrading(mPostProcess);
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
                        DllImportAPI.PostProcess_ColorGrading_SetEnableColorGrading(mPostProcess, 1);
                    else
                        DllImportAPI.PostProcess_ColorGrading_SetEnableColorGrading(mPostProcess, 0);
                }
            }
        }
        /// <summary>
        /// 只读属性，着色板的地址
        /// </summary>
        [CSUtility.Support.DoNotCopy]
        public IntPtr ColorGradingTexture
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ColorGrading_GetColorGrading(mPostProcess);
                }
            }
        }
        /// <summary>
        /// 着色板的名字
        /// </summary>
        [System.ComponentModel.DisplayName("LUT纹理")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("TextureSet")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string ColorGradingTexName
        {
            get { return mColorGradingTexName; }
            set
            {
                SetColorGrading(value);
            }
        }
        /// <summary>
        /// 是否进行Gamma校正
        /// </summary>
        [System.ComponentModel.DisplayName("GammaCorrect")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool GammaCorrect
        {
            get
            {
                return CCore.Engine.Instance.Client.Graphics.GammaCorrect;
            }
            set
            {
                CCore.Engine.Instance.Client.Graphics.GammaCorrect = value;
            }
        }
        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="srcP">需要进行线性插值的后期处理数据</param>
        /// <param name="lerpDuration">插值间隔</param>
        /// <param name="lerpValue">线性插值的插入值大小</param>
        public override void LerpFrom(CCore.Graphics.PostProcess srcP, long lerpDuration, float lerpValue)
        {
            var src = srcP as PostProcess_ColorGrading;
            if (src == null)
                return;

            base.LerpFrom(srcP, lerpDuration, lerpValue);

            SetSrcColorGrading(src.ColorGradingTexture);
        }
        /// <summary>
        /// 为后处理设置着色板
        /// </summary>
        /// <param name="texName">着色板的名字</param>
        public void SetColorGrading(string texName)
        {
            unsafe
            {
                mColorGradingTexName = texName;
                DllImportAPI.PostProcess_ColorGrading_SetColorGrading(mPostProcess, texName);
            }
        }

        #region 两个ColorGrading切换的过度参数


        /// <summary>
        /// 着色板的纹理名称
        /// </summary>
        public string mSrcColorGradingTexName;
        /// <summary>
        /// 为后处理设置纹理
        /// </summary>
        /// <param name="tex">使用的着色板纹理指针</param>
        public void SetSrcColorGrading(IntPtr tex)
        {
            unsafe
            {
                DllImportAPI.PostProcess_ColorGrading_SetSrcColorGradingTexture(mPostProcess, tex);
            }
        }
        #endregion
        /// <summary>
        /// 着色板的构造函数
        /// </summary>
        public PostProcess_ColorGrading()
        {
            unsafe
            {
                mName = "色调曲线";
                m_Type = enPostProcessType.ColorGrading;
                mPostProcess = DllImportAPI.PostProcess_ColorGrading_New(CCore.Engine.Instance.Client.Graphics.Device);
                DllImportAPI.PostProcess_ColorGrading_Initialize(mPostProcess);
                SetColorGrading(CSUtility.Support.IFileConfig.DefaultColorGradeFile);
            }
        }
        /// <summary>
        /// 析构函数，删除对象并释放该指针
        /// </summary>
        ~PostProcess_ColorGrading()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象并释放指针
        /// </summary>
        public override void Cleanup()
        {
            unsafe
            {
                if (mPostProcess != IntPtr.Zero)
                {
                    DllImportAPI.PostProcess_Release(mPostProcess);
                    mPostProcess = IntPtr.Zero;
                }
            }
        }
    }

}
