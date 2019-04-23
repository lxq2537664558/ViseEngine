using System;

namespace CCore.Component
{
    /// <summary>
    /// 直线
    /// </summary>
    public class Line : Visual
    {
        /// <summary>
        /// 直线的地址
        /// </summary>
        protected IntPtr mLineObject;   // model3::v3dLineObject*
        /// <summary>
        /// 直线实例的地址
        /// </summary>
        public IntPtr LineObject
        {
            get { return mLineObject; }
        }
        /// <summary>
        /// 直线的颜色
        /// </summary>
        public CSUtility.Support.Color Color
        {
            get { return CSUtility.Support.Color.FromArgb((int)(DllImportAPI.v3dLineObject_GetColor(mLineObject))); }
            set
            {
                DllImportAPI.v3dLineObject_SetColor(mLineObject, (UInt32)value.ToArgb());
            }
        }
        /// <summary>
        /// 直线的起始坐标
        /// </summary>
        public SlimDX.Vector3 Start
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 retV;
                    DllImportAPI.v3dLineObject_GetStart(mLineObject, &retV);
                    return retV;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dLineObject_SetStart(mLineObject, &value);
                }
            }
        }
        /// <summary>
        /// 直线的结束坐标
        /// </summary>
        public SlimDX.Vector3 End
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 retV;
                    DllImportAPI.v3dLineObject_GetEnd(mLineObject, &retV);
                    return retV;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dLineObject_SetEnd(mLineObject, &value);
                }
            }
        }
        /// <summary>
        /// 直线的初始化函数，默认右手坐标系
        /// </summary>
        public Line()
        {
            mLineObject = DllImportAPI.v3dLineObject_New();
            mLayer = RLayer.RL_SystemHelper;
        }
        /// <summary>
        /// 析构函数，释放实例的内存
        /// </summary>
        ~Line()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放直线实例的内存，并将地址置空
        /// </summary>
        public override void Cleanup()
        {
            if(mLineObject != IntPtr.Zero)
            {
                DllImportAPI.v3dLineObject_Release(mLineObject);
                mLineObject = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 将该实例提交到显示环境中
        /// </summary>
        /// <param name="renderEnv">提交到的环境</param>
        /// <param name="matrix">直线的矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            unsafe
            {
                fixed(SlimDX.Matrix* pinMatrix = &matrix)
                {
                    DllImportAPI.vDSRenderEnv_CommitHelperLine(renderEnv.DSRenderEnv, (int)mGroup, mLineObject, pinMatrix);
                }
            }
        }
    }
}
