using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 后期特效的锐化
    /// </summary>
    public class PostProcess_Sharpen : PostProcess
    {
        /// <summary>
        /// 是否开启高质量锐化，编辑期内可更改
        /// </summary>
        [System.ComponentModel.DisplayName("开启高质量锐化")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool EnableHighQuality
        {
            get
            {
                unsafe
                {
                    var val = DllImportAPI.PostProcess_Sharpen_GetHighQualitySharpen(mPostProcess);
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
                        DllImportAPI.PostProcess_Sharpen_SetHighQualitySharpen(mPostProcess, 1);
                    else
                        DllImportAPI.PostProcess_Sharpen_SetHighQualitySharpen(mPostProcess, 0);
                }
            }
        }
        /// <summary>
        /// 均值
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public float Average
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_Sharpen_GetAverage(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Sharpen_SetAverage(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// CoefBlur的属性设置
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public float CoefBlur
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_Sharpen_GetCoefBlur(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Sharpen_SetCoefBlur(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 边缘锐化
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public float SharpenEdge
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_Sharpen_GetSharpenEdge(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Sharpen_SetSharpenEdge(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 锐化数值
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public float SharpenValue
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_Sharpen_GetSharpenValue(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Sharpen_SetSharpenValue(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 锐化质量
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public CCore.Performance.ESharpen SharpenQuality
        {
            get
            {
                unsafe
                {
                    return (CCore.Performance.ESharpen)DllImportAPI.PostProcess_ColorGrading_GetSharpenQuality(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ColorGrading_SetSharpenQuality(mPostProcess, (int)value);
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PostProcess_Sharpen()
        {
            unsafe
            {
                mName = "锐化";
                m_Type = enPostProcessType.Sharpen;
                mPostProcess = DllImportAPI.PostProcess_Sharpen_New(CCore.Engine.Instance.Client.Graphics.Device);
                DllImportAPI.PostProcess_Sharpen_Initialize(mPostProcess);
            }
        }
        /// <summary>
        /// 析构函数，释放指针
        /// </summary>
        ~PostProcess_Sharpen()
        {
            Cleanup();
        }
        /// <summary>
        /// 清除对象，释放指针
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
