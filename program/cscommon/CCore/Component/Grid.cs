using System;

namespace CCore.Component
{
    /// <summary>
    /// 可视化网格
    /// </summary>
    public class Grid : Visual
    {
        /// <summary>
        /// 网格地址
        /// </summary>
        protected IntPtr mGridObject;   // model3::v3dGridObject*
        /// <summary>
        /// 只读属性，网格的地址
        /// </summary>
        public IntPtr GridObject
        {
            get { return mGridObject; }
        }
        /// <summary>
        /// 网格的列
        /// </summary>
        public int Col
        {
            get
            {
                return DllImportAPI.v3dGridObject_GetCol(mGridObject);
            }
            set
            {
                DllImportAPI.v3dGridObject_SetCol(mGridObject, value);
            }
        }
        /// <summary>
        /// 网格行
        /// </summary>
        public int Row
        {
            get
            {
                return DllImportAPI.v3dGridObject_GetRow(mGridObject);
            }
            set
            {
                DllImportAPI.v3dGridObject_SetRow(mGridObject, value);
            }
        }
        /// <summary>
        /// 网格X轴向的间隔距离
        /// </summary>
        public float DeltaX
        {
            get
            {
                return DllImportAPI.v3dGridObject_GetDeltaX(mGridObject);
            }
            set
            {
                DllImportAPI.v3dGridObject_SetDeltaX(mGridObject, value);
            }
        }
        /// <summary>
        /// 网格Z轴向的间隔距离
        /// </summary>
        public float DeltaZ
        {
            get
            {
                return DllImportAPI.v3dGridObject_GetDeltaZ(mGridObject);
            }
            set
            {
                DllImportAPI.v3dGridObject_SetDeltaZ(mGridObject, value);
            }
        }
        /// <summary>
        /// 网格的高度
        /// </summary>
        public float Height
        {
            get
            {
                return DllImportAPI.v3dGridObject_GetHeight(mGridObject);
            }
            set
            {
                DllImportAPI.v3dGridObject_SetHeight(mGridObject, value);
            }
        }
        /// <summary>
        /// 网格的颜色
        /// </summary>
        public CSUtility.Support.Color Color
        {
            get
            {
                return CSUtility.Support.Color.FromArgb((int)DllImportAPI.v3dGridObject_GetColor(mGridObject));
            }
            set
            {
                DllImportAPI.v3dGridObject_SetColor(mGridObject, (UInt32)(value.ToArgb()));
            }
        }
        /// <summary>
        /// 网格标准轴的颜色
        /// </summary>
        public CSUtility.Support.Color ColorAxis
        {
            get
            {
                return CSUtility.Support.Color.FromArgb((int)(DllImportAPI.v3dGridObject_GetColorAxis(mGridObject)));
            }
            set
            {
                DllImportAPI.v3dGridObject_SetColorAxis(mGridObject, (UInt32)(value.ToArgb()));
            }
        }
        /// <summary>
        /// 网格的相对矩阵
        /// </summary>
        public SlimDX.Matrix Transform
        {
            get
            {
                unsafe
                {
                    SlimDX.Matrix mat;
                    DllImportAPI.v3dGridObject_GetRelativeMatrix(mGridObject, &mat);
                    return mat;
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dGridObject_SetRelativeMatrix(mGridObject, &value);
                }
            }
        }
        /// <summary>
        /// 构造函数，默认右手坐标系
        /// </summary>
        public Grid()
        {
            mGridObject = DllImportAPI.v3dGridObject_New();
            mLayer = RLayer.RL_SystemHelper;
        }
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
        ~Grid()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放网格，将网格地址置空
        /// </summary>
        public override void Cleanup()
        {
            if(mGridObject != IntPtr.Zero)
            {
                DllImportAPI.v3dGridObject_Release(mGridObject);
                mGridObject = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 将网格提交到需要的环境中
        /// </summary>
        /// <param name="renderEnv">提交到的环境</param>
        /// <param name="matrix">网格的矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if (!Visible)
                return;

            DllImportAPI.vDSRenderEnv_CommitHelperGrid(renderEnv.DSRenderEnv, (int)mGroup, mGridObject);
        }
    }
}
