using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 后期特效的Bloom类
    /// </summary>
    public class PostProcess_Bloom : PostProcess
    {
        /// <summary>
        /// 后期特效是否进行模糊处理，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.DoNotCopy]
        public bool DoBlur
        {
            get
            {
                unsafe
                {
                    return (DllImportAPI.PostProcess_Bloom_GetDoBlur(mPostProcess) != 0) ? true : false;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Bloom_SetDoBlur(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 后期特效的Bloom缩放，取值为0.1 - 0.5，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("Bloom缩放")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 0.5)]
        public float BloomImageScale
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_Bloom_GetBloomImageScale(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Bloom_SetBloomImageScale(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 后期特效Bloom的Blur类型，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("Blur类型")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public enBlurType BlurType
        {
            get
            {
                unsafe
                {
                    return (enBlurType)(DllImportAPI.PostProcess_Bloom_GetBlurType(mPostProcess));
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Bloom_SetBlurType(mPostProcess, (int)value);
                }
            }
        }
        /// <summary>
        /// 后期特效Bloom的Blur间隔，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("Blur间隔")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 3)]
        public float BlurScale
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_Bloom_GetBlurScale(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Bloom_SetBlurScale(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 后期特效Bloom的Blur强度，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("Blur强度")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0, 5)]
        public float BlurStrength
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_Bloom_GetBlurStrength(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Bloom_SetBlurStrength(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 后期特效Bloom的Blur直径，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("Blur直径")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1, 5)]
        public float BlurAmount
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_Bloom_GetBlurAmount(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_Bloom_SetBlurAmount(mPostProcess, value);
                }
            }
        }
        
        void _UpdateEdgeDetectParams()
        {
            if (mOwnerEnv == null)
                return;
            unsafe
            {
                DllImportAPI.vDSRenderEnv_SetEdgeDetectParams(mOwnerEnv.DSRenderEnv, mEdgeDetectRate, mEdgeDetectStrength, mEdgeDetectScale);
            }
        }

        float mEdgeDetectRate = 3.0f;
        /// <summary>
        /// 后期特效Bloom的勾边贴图的缩小倍数，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("勾边贴图的缩小倍数")]
        [System.ComponentModel.Description("数值越大，勾边效果越明显")]
        [System.ComponentModel.Category("勾边的Blur效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1.5, 10)]
        public float EdgeDetectRate
        {
            get { return mEdgeDetectRate; }
            set
            {
                mEdgeDetectRate = value;
                _UpdateEdgeDetectParams();
            }
        }

        float mEdgeDetectStrength = 1.0f;
        /// <summary>
        /// 后期特效Bloom的勾边的强度，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("勾边的强度")]
        [System.ComponentModel.Category("勾边的Blur效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 5)]
        public float EdgeDetectStrength
        {
            get { return mEdgeDetectStrength; }
            set
            {
                mEdgeDetectStrength = value;
                _UpdateEdgeDetectParams();
            }
        }

        float mEdgeDetectScale = 1.0f;
        /// <summary>
        /// Blur效果的勾边的采样半径比例，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("勾边的采样半径比例")]
        [System.ComponentModel.Category("勾边的Blur效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.2, 5)]
        public float EdgeDetectScale
        {
            get { return mEdgeDetectScale; }
            set
            {
                mEdgeDetectScale = value;
                _UpdateEdgeDetectParams();
            }
        }


        void _UpdateFSBlurParams()
        {
            if (mOwnerEnv == null)
                return;
            unsafe
            {
                DllImportAPI.vDSRenderEnv_SetFSBlurParams(mOwnerEnv.DSRenderEnv, mFSRate, mFSBlurStrength, mFSBlurScale);
            }            
        }

        float mFSRate = 2.0f;
        /// <summary>
        /// 透明物体的Blur效果的光晕贴图的缩小倍数，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕贴图的缩小倍数")]
        [System.ComponentModel.Description("数值越大，光晕效果越明显")]
        [System.ComponentModel.Category("透明物体的Blur效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1.5, 10)]
        public float FSRate
        {
            get { return mFSRate; }
            set
            {
                mFSRate = value;
                _UpdateFSBlurParams();
            }
        }

        float mFSBlurStrength = 1.0f;
        /// <summary>
        /// 透明物体的Blur效果的光晕贴图的缩小倍数，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕的强度")]
        [System.ComponentModel.Category("透明物体的Blur效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 5)]
        public float FSBlurStrength
        {
            get { return mFSBlurStrength; }
            set
            {
                mFSBlurStrength = value;
                _UpdateFSBlurParams();
            }
        }

        float mFSBlurScale = 1.0f;
        /// <summary>
        /// 透明物体的Blur效果的光光晕的采样半径比例，可在编辑器内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕的采样半径比例")]
        [System.ComponentModel.Category("透明物体的Blur效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.2, 5)]
        public float FSBlurScale
        {
            get { return mFSBlurScale; }
            set
            {
                mFSBlurScale = value;
                _UpdateFSBlurParams();
            }
        }
        /// <summary>
        /// 后期特效Bloom的构造函数
        /// </summary>
        public PostProcess_Bloom()
        {
            unsafe
            {
                mName = "GLOW";
                mPostProcess = DllImportAPI.PostProcess_Bloom_New(CCore.Engine.Instance.Client.Graphics.Device);
                DllImportAPI.PostProcess_Bloom_Initialize(mPostProcess);
                m_Type = enPostProcessType.Bloom;
            }
        }
        /// <summary>
        /// 析构函数，清除该实例并释放指针
        /// </summary>
        ~PostProcess_Bloom()
        {
            Cleanup();
        }
        /// <summary>
        /// 清除该实例并释放
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
