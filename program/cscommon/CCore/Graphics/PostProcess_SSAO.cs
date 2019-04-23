using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 后期特效的高级光照效果
    /// </summary>
    public class PostProcess_SSAO : PostProcess
    {
        /// <summary>
        /// 随机法线纹理的名称
        /// </summary>
        public string mRandomNormalTexName;
        /// <summary>
        /// SSAO的采样半径
        /// </summary>
        [System.ComponentModel.DisplayName("采样半径")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.01, 2)]
        public float SampleRad
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_SSAO_GetSampleRad(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_SSAO_SetSampleRad(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// SSAO的AO强度
        /// </summary>
        [System.ComponentModel.DisplayName("AO强度")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 5)]
        public float Intensity
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_SSAO_GetIntensity(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_SSAO_SetIntensity(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// SSAO的缩放距离，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("缩放距离")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 5)]
        public float Scale
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_SSAO_GetScale(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_SSAO_SetScale(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// SSAO的遮挡圆锥宽度
        /// </summary>
        [System.ComponentModel.DisplayName("遮挡圆锥宽度")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.01, 1)]
        public float Bias
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_SSAO_GetBias(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_SSAO_SetBias(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// SSAO是否开启模糊处理（Blur）
        /// </summary>
        [System.ComponentModel.DisplayName("开启Blur")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public virtual bool DoBlur
        {
            get
            {
                unsafe
                {
                    return (DllImportAPI.PostProcess_SSAO_GetDoBlur(mPostProcess) != 0) ? true : false;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_SSAO_SetDoBlur(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// SSAO的图像质量
        /// </summary>
        [System.ComponentModel.DisplayName("图像质量")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public CCore.Performance.ESSAO SSAOQuality
        {
            get
            {
                unsafe
                {
                    return (CCore.Performance.ESSAO)DllImportAPI.PostProcess_SSAO_GetFXAAQuality(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_SSAO_SetFXAAQuality(mPostProcess, (int)value);
                }
            }
        }
        /// <summary>
        /// 设置SSAO的随机法线纹理名称
        /// </summary>
        [System.ComponentModel.DisplayName("随机法线纹理")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("TextureSet")]
        public string RandomNormalTexName
        {
            get { return mRandomNormalTexName; }
            set
            {
                SetRandomNormal(value);
            }
        }

        /// <summary>
        /// SSAO的构造函数
        /// </summary>
        public PostProcess_SSAO()
        {
            unsafe
            {
                mName = "SSAO";
                mPostProcess = DllImportAPI.PostProcess_SSAO_New(CCore.Engine.Instance.Client.Graphics.Device);
                DllImportAPI.PostProcess_SSAO_Initialize(mPostProcess);
                m_Type = enPostProcessType.SSAO;
            }
        }
        /// <summary>
        /// 析构函数，删除对象并释放指针
        /// </summary>
        ~PostProcess_SSAO()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放SSAO的内存，在消除后处理时调用
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
        /// <summary>
        /// 根据随机法线名称设置随机法线
        /// </summary>
        /// <param name="texName">要设置的随机法线名称</param>
        public void SetRandomNormal(string texName)
        {
            unsafe
            {
                mRandomNormalTexName = texName;
                DllImportAPI.PostProcess_SSAO_SetRandomNormalTexture(mPostProcess, texName);
            }
        }
    }
}
