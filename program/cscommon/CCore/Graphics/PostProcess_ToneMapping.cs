using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 后期特效的色泽贴图
    /// </summary>
    public class PostProcess_ToneMapping : PostProcess
    {
        /// <summary>
        /// 色泽贴图的S强度，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("S强度")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 3)]
        public float A
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ToneMapping_GetA(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetA(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 色泽贴图的Linear强度，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("Linear强度")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 3)]
        public float B
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ToneMapping_GetB(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetB(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 色泽贴图的Linear角度，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("Linear角度")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 3)]
        public float C
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ToneMapping_GetC(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetC(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 色泽贴图的T强度，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("T强度")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.01, 3)]
        public float D
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ToneMapping_GetD(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetD(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 色泽贴图的T分子，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("T分子")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.01, 0.1)]
        public float E
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ToneMapping_GetE(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetE(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 色泽贴图的T分母，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("T分母")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 3)]
        public float F
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ToneMapping_GetF(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetF(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 色泽贴图的WhitePoint，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("WhitePoint")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1, 10)]
        public float W
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ToneMapping_GetW(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetW(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 色泽贴图的Bloom亮度，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("Bloom亮度")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1, 10)]
        public float BrightFactor
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.PostProcess_ToneMapping_GetBrightFactor(mPostProcess);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetBrightFactor(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 只写属性，色泽贴图的PreA
        /// </summary>
        public float PreA
        {
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetPreA(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 只写属性，色泽贴图的PreB
        /// </summary>
        public float PreB
        {
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetPreB(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 只写属性，色泽贴图的PreC
        /// </summary>
        public float PreC
        {
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetPreC(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 只写属性，色泽贴图的PreD
        /// </summary>
        public float PreD
        {
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetPreD(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 只写属性，色泽贴图预置的T分子
        /// </summary>
        public float PreE
        {
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetPreE(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 只写属性，色泽贴图预置的T分母
        /// </summary>
        public float PreF
        {
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetPreF(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 只写属性，色泽贴图的PreW
        /// </summary>
        public float PreW
        {
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetPreW(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 更新加载采样器参数
        /// </summary>
        protected void _UpdateDownsamplerParams()
        {
            unsafe
            {
                DllImportAPI.PostProcess_ToneMapping_UpdateDownsamplerParams(mPostProcess, mDownsamplerCount, mDownsamplerRate1, mDownsamplerRate2, mDownsamplerRate3, mDownsamplerStrength1, mDownsamplerStrength2, mDownsamplerStrength3);
            }
        }

        int mDownsamplerCount = 3;
        /// <summary>
        /// 色泽贴图的光晕层数，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕层数")]
        [System.ComponentModel.Description("光晕效果由多级高光贴图叠加生成，每级贴图逐渐缩小并做Blur操作，最终将所有贴图叠加后生成光晕效果")]
        [System.ComponentModel.Category("光晕效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1, 3)]
        public int DownsamplerCount
        {
            get { return mDownsamplerCount; }
            set
            {
                mDownsamplerCount = value;
                _UpdateDownsamplerParams();
            }
        }

        float mDownsamplerRate1 = 3;
        /// <summary>
        /// 色泽贴图的光晕(1)缩小倍数,默认为3，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕(1)缩小倍数")]
        [System.ComponentModel.Description("光晕(1)缩小倍数，将场景中高光部分组成的贴图缩小指定倍数后得到光晕(1)")]
        [System.ComponentModel.Category("光晕效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1.5, 5)]
        public float DownsamplerRate1
        {
            get { return mDownsamplerRate1; }
            set
            {
                mDownsamplerRate1 = value;
                _UpdateDownsamplerParams();
            }
        }

        float mDownsamplerRate2 = 2;
        /// <summary>
        /// 色泽贴图的光晕(2)缩小倍数,默认为2，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕(2)缩小倍数")]
        [System.ComponentModel.Description("光晕(2)缩小倍数，将场景中高光部分组成的贴图缩小指定倍数后得到光晕(2)")]
        [System.ComponentModel.Category("光晕效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1.5, 5)]
        public float DownsamplerRate2
        {
            get { return mDownsamplerRate2; }
            set
            {
                mDownsamplerRate2 = value;
                _UpdateDownsamplerParams();
            }
        }

        float mDownsamplerRate3 = 2;
        /// <summary>
        /// 色泽贴图的光晕(3)缩小倍数,默认为2，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕(3)缩小倍数")]
        [System.ComponentModel.Description("光晕(3)缩小倍数，将场景中高光部分组成的贴图缩小指定倍数后得到光晕(3)")]
        [System.ComponentModel.Category("光晕效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(1.5, 5)]
        public float DownsamplerRate3
        {
            get { return mDownsamplerRate3; }
            set
            {
                mDownsamplerRate3 = value;
                _UpdateDownsamplerParams();
            }
        }


        float mDownsamplerStrength1 = 1;
        /// <summary>
        /// 色泽贴图的光晕(1)强度,默认为1，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕(1)强度")]
        [System.ComponentModel.Description("光晕(1)强度，将场景中高光部分组成的贴图缩小指定倍数后得到光晕(1) 接着执行Blur操作, 最后将结果乘以此强度值")]
        [System.ComponentModel.Category("光晕效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 5)]
        public float DownsamplerStrength1
        {
            get { return mDownsamplerStrength1; }
            set
            {
                mDownsamplerStrength1 = value;
                _UpdateDownsamplerParams();
            }
        }

        float mDownsamplerStrength2 = 1;
        /// <summary>
        /// 色泽贴图的光晕(2)强度,默认为1，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕(2)强度")]
        [System.ComponentModel.Description("光晕(2)强度，将场景中高光部分组成的贴图缩小指定倍数后得到光晕(2) 接着执行Blur操作, 最后将结果乘以此强度值")]
        [System.ComponentModel.Category("光晕效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 5)]
        public float DownsamplerStrength2
        {
            get { return mDownsamplerStrength2; }
            set
            {
                mDownsamplerStrength2 = value;
                _UpdateDownsamplerParams();
            }
        }

        float mDownsamplerStrength3 = 1;
        /// <summary>
        /// 色泽贴图的光晕(3)强度,默认为1，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("光晕(3)强度")]
        [System.ComponentModel.Description("光晕(3)强度，将场景中高光部分组成的贴图缩小指定倍数后得到光晕(3) 接着执行Blur操作, 最后将结果乘以此强度值")]
        [System.ComponentModel.Category("光晕效果")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_ValueWithRange(0.1, 5)]
        public float DownsamplerStrength3
        {
            get { return mDownsamplerStrength3; }
            set
            {
                mDownsamplerStrength3 = value;
                _UpdateDownsamplerParams();
            }
        }
        /// <summary>
        /// 色泽贴图是否开启高光Blur，可在编辑期内更改
        /// </summary>
        [System.ComponentModel.DisplayName("是否开启高光Blur")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool LensEffectsEnable
        {
            get
            {
                unsafe
                {
                    return (DllImportAPI.PostProcess_ToneMapping_GetLensEffectsEnable(mPostProcess) != 0) ? true : false;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.PostProcess_ToneMapping_SetLensEffectsEnable(mPostProcess, value);
                }
            }
        }
        /// <summary>
        /// 色泽贴图的构造函数
        /// </summary>
        public PostProcess_ToneMapping()
        {
            unsafe
            {
                mName = "HDR";
                m_Type = enPostProcessType.HDR;
                mPostProcess = DllImportAPI.PostProcess_ToneMapping_New(CCore.Engine.Instance.Client.Graphics.Device);
                DllImportAPI.PostProcess_ToneMapping_Initialize(mPostProcess);
            }
        }
        /// <summary>
        /// 析构函数，释放对象的内存
        /// </summary>
        ~PostProcess_ToneMapping()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放色泽贴图的内存，在消除后处理时调用
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
        /// 在后处理时进行线性插值
        /// </summary>
        /// <param name="srcP">需要进行插值的后处理数据</param>
        /// <param name="lerpDuration">插值间隔</param>
        /// <param name="lerpValue">插值大小</param>
        public override void LerpFrom(CCore.Graphics.PostProcess srcP, long lerpDuration, float lerpValue)
        {
            var src = srcP as PostProcess_ToneMapping;
            if (src == null)
                return;

            base.LerpFrom(srcP, lerpDuration, lerpValue);

            PreA = (float)src.A;
            PreB = (float)src.B;
            PreC = (float)src.C;
            PreD = (float)src.D;
            PreE = (float)src.E;
            PreF = (float)src.F;
            PreW = (float)src.W;
        }
    }
}
