using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 2D文本类
    /// </summary>
    public class Text2D : CCore.Component.Visual
    {
        /// <summary>
        /// 2D文本实例对象
        /// </summary>
        protected IntPtr mText2DObject;     // model3::v3dText2DObject*
        /// <summary>
        /// 2D文本实例的内存地址
        /// </summary>
        public IntPtr Text2DObject
        {
            get { return mText2DObject; }
        }
        /// <summary>
        /// 字体
        /// </summary>
        protected CCore.Font.Font mFont;
        /// <summary>
        /// 字体渲染参数列表
        /// </summary>
        protected CCore.Font.FontRenderParamList mFondRenderParams;
        /// <summary>
        /// 只读属性，渲染参数
        /// </summary>
        public CCore.Font.FontRenderParamList RenderParams
        {
            get { return mFondRenderParams; }
        }
        /// <summary>
        /// 文本的名称
        /// </summary>
        public string Text
        {
            get
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.v3dText2DObject_GetText(mText2DObject));
            }
            set
            {
                DllImportAPI.v3dText2DObject_SetText(mText2DObject, value);
            }
        }
        /// <summary>
        /// 渲染的字体名称
        /// </summary>
        public string FontName
        {
            get
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.v3dText2DObject_GetFontName(mText2DObject));
            }
            set
            {
                DllImportAPI.v3dText2DObject_SetFontName(mText2DObject, value);
            }
        }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get
            {
                return DllImportAPI.v3dText2DObject_GetFontSize(mText2DObject);
            }
            set
            {
                DllImportAPI.v3dText2DObject_SetFontSize(mText2DObject, value);
            }
        }
        /// <summary>
        /// 2D文本所在的坐标X轴的位置
        /// </summary>
        public float X
        {
            get
            {
                return DllImportAPI.v3dText2DObject_GetX(mText2DObject);
            }
            set
            {
                DllImportAPI.v3dText2DObject_SetX(mText2DObject, value);
            }
        }
        /// <summary>
        /// 2D文本所在坐标的Y轴的位置
        /// </summary>
        public float Y
        {
            get
            {
                return DllImportAPI.v3dText2DObject_GetY(mText2DObject);
            }
            set
            {
                DllImportAPI.v3dText2DObject_SetY(mText2DObject, value);
            }
        }
        /// <summary>
        /// 2D文本所在坐标的Z轴的位置
        /// </summary>
        public float Z
        {
            get
            {
                return DllImportAPI.v3dText2DObject_GetZ(mText2DObject);
            }
            set
            {
                DllImportAPI.v3dText2DObject_SetZ(mText2DObject, value);
            }
        }
        /// <summary>
        /// 2D文本的构造函数
        /// </summary>
        public Text2D()
        {
            mText2DObject = DllImportAPI.v3dText2DObject_New();
            mLayer = RLayer.RL_SystemHelper;
            mFondRenderParams = new CCore.Font.FontRenderParamList(true);
            DllImportAPI.v3dText2DObject_SetParams(mText2DObject, mFondRenderParams.Inner);
            DllImportAPI.v3dText2DObject_Params_AddRef(mText2DObject);
        }
        /// <summary>
        /// 析构函数，释放对象指针
        /// </summary>
        ~Text2D()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放2D文本的对象指针
        /// </summary>
        public override void Cleanup()
        {
            if(mText2DObject != IntPtr.Zero)
            {
                DllImportAPI.v3dText2DObject_Release(mText2DObject);
                mText2DObject = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 将对象提交到渲染环境
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            unsafe
            {
                fixed(SlimDX.Matrix* pinMatrix = &matrix)
                {
                    DllImportAPI.vDSRenderEnv_CommitText2D(renderEnv.DSRenderEnv, (int)mGroup, (int)mLayer, mText2DObject, pinMatrix, false);
                }
            }
        }
    }
}
