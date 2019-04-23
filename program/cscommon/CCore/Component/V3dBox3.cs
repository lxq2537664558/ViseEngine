using System;

namespace CCore.Component
{
    /// <summary>
    /// 3D盒子
    /// </summary>
    public class V3dBox3 : Visual
    {
        /// <summary>
        /// 盒子的指针
        /// </summary>
        protected IntPtr mBox3Object;       // model3::v3dBox3Object*
        /// <summary>
        /// 只读属性，实例地址
        /// </summary>
        public IntPtr Box3Object
        {
            get { return mBox3Object; }
        }
        /// <summary>
        /// 可视范围，默认为0
        /// </summary>
        protected float mViewSize = 0;
        /// <summary>
        /// 可视范围
        /// </summary>
        public float ViewSize
        {
            get { return mViewSize; }
            set { mViewSize = value; }
        }
        /// <summary>
        /// 实例颜色
        /// </summary>
        public CSUtility.Support.Color Color
        {
            get
            {
                return CSUtility.Support.Color.FromArgb((int)(DllImportAPI.v3dBox3Object_GetColor(mBox3Object)));
            }
            set
            {
                DllImportAPI.v3dBox3Object_SetColor(mBox3Object, (UInt32)(value.ToArgb()));
            }
        }
        /// <summary>
        /// 构造函数，默认右手坐标系
        /// </summary>
        public V3dBox3()
        {
            unsafe
            {
                mBox3Object = DllImportAPI.v3dBox3Object_New();
                var size = SlimDX.Vector3.UnitXYZ;
                DllImportAPI.v3dBox3Object_SetSize(mBox3Object, &size);
                mLayer = RLayer.RL_SystemHelper;
            }
        }
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
        ~V3dBox3()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放实例内存，指针置空
        /// </summary>
        public override void Cleanup()
        {
            if(mBox3Object != IntPtr.Zero)
            {
                DllImportAPI.v3dBox3Object_Release(mBox3Object);
                mBox3Object = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 将该实例提交给渲染场景
        /// </summary>
        /// <param name="renderEnv">渲染场景</param>
        /// <param name="matrix">实例矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            unsafe
            {
                fixed (SlimDX.Matrix* pinMatrix = &matrix)
                {
                    if (mViewSize > 0)
                    {
                        var unitMat = SlimDX.Matrix.Identity;
                        SlimDX.Vector3 pos, scale;
                        SlimDX.Quaternion quat;
                        matrix.Decompose(out scale, out quat, out pos);
                        float sinSize = eye.GetScreenSizeInWorld(pos, mViewSize);
                        unitMat = SlimDX.Matrix.AffineTransformation(0.5f * sinSize, SlimDX.Vector3.Zero, quat, pos);

                        DllImportAPI.vDSRenderEnv_CommitHelperBox(renderEnv.DSRenderEnv, (int)mGroup, mBox3Object, &unitMat);
                    }
                    else
                    {
                        DllImportAPI.vDSRenderEnv_CommitHelperBox(renderEnv.DSRenderEnv, (int)mGroup, mBox3Object, pinMatrix);
                    }
                }
            }

        }
        /// <summary>
        /// 实例的AABB包围盒
        /// </summary>
        /// <param name="vMin">最大顶点</param>
        /// <param name="vMax">最小顶点</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
			vMin = new SlimDX.Vector3(-0.5f, -0.5f, -0.5f);
			vMax = new SlimDX.Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}
