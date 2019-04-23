using System;

namespace CCore.Component
{
    /// <summary>
    /// 顶端轴
    /// </summary>
    public class TipAxis : Visual
    {
        /// <summary>
        /// 顶端轴的实例地址
        /// </summary>
        protected IntPtr mTipAxisObject;
        /// <summary>
        /// 顶端轴的实例地址
        /// </summary>
        public IntPtr TipAxisObject
        {
            get { return mTipAxisObject; }
        }
        /// <summary>
        /// 轴的长度
        /// </summary>
        public float fAxisLen
        {
            get
            {
                return DllImportAPI.v3dTipAxisObject_GetAxisLen(mTipAxisObject);
            }
            set
            {
                DllImportAPI.v3dTipAxisObject_SetAxisLen(mTipAxisObject, value);
            }
        }
        /// <summary>
        /// 绘制的偏移值
        /// </summary>
        public float fDrawOffset
        {
            get
            {
                return DllImportAPI.v3dTipAxisObject_GetDrawOffset(mTipAxisObject);
            }
            set
            {
                DllImportAPI.v3dTipAxisObject_SetDrawOffset(mTipAxisObject, value);
            }
        }
        /// <summary>
        /// 构造函数，默认右手坐标系
        /// </summary>
        public TipAxis()
        {
            mTipAxisObject = DllImportAPI.v3dTipAxisObject_New();
            mLayer = RLayer.RL_SystemHelper;
        }
        /// <summary>
        /// 析构函数，释放实例对象的内存
        /// </summary>
        ~TipAxis()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放实例对象的内存
        /// </summary>
        public override void Cleanup()
        {
            if (mTipAxisObject != IntPtr.Zero)
            {
                DllImportAPI.v3dTipAxisObject_Release(mTipAxisObject);
                mTipAxisObject = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 将该对象提交到渲染环境
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">对象的矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            DllImportAPI.vDSRenderEnv_CommitHelperTipAxis(renderEnv.DSRenderEnv, (int)mGroup, mTipAxisObject);
        }
    }
}
