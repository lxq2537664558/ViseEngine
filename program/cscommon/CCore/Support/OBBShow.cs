using System;
using System.Collections.Generic;

namespace CCore.Support
{
    /// <summary>
    /// OBB包围盒初始化
    /// </summary>
    public class OBBShowInit : CCore.World.ActorInit { }
    /// <summary>
    /// OBB包围盒对象
    /// </summary>
    public class OBBShowActor : CCore.World.Actor
    {
        /// <summary>
        /// 目标Actor对象
        /// </summary>
        protected CCore.World.Actor mTargetActor;
        /// <summary>
        /// 包围框
        /// </summary>
        protected List<CCore.Component.Line> mShowLineList;
        /// <summary>
        /// 最大顶点
        /// </summary>
        protected SlimDX.Vector3 mMax;
        /// <summary>
        /// 最小顶点
        /// </summary>
        protected SlimDX.Vector3 mMin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public OBBShowActor() { }
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="_init">Actor的初始化对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            mMax = SlimDX.Vector3.Zero;
            mMin = SlimDX.Vector3.Zero;

            Visual = new OBBVisual();
            mPlacement = new CSUtility.Component.StandardPlacement(this);

            return true;
        }
        /// <summary>
        /// 设置目标Actor对象
        /// </summary>
        /// <param name="actor">Actor对象</param>
        public void SetTarget(CCore.World.Actor actor)
        {
            mTargetActor = actor;

            if (mTargetActor != null)
            {
                mPlacement = mTargetActor.Placement;

                if (actor.VisibleCheckOBB)
                {
                    OBBVisual vis = Visual as OBBVisual;
                    SlimDX.Vector3 vMin = SlimDX.Vector3.Zero, vMax = SlimDX.Vector3.Zero;
                    actor.Visual.GetOBB(ref vMin, ref vMax, ref vis.FixObbMatrix);

                    vMin *= 1.01f;
                    vMax *= 1.01f;
                    vis.SetMaxMin(ref vMax, ref vMin);
                }
                else
                {
                    SlimDX.Vector3 vMin = SlimDX.Vector3.Zero, vMax = SlimDX.Vector3.Zero;
                    actor.GetOrigionAABB(ref vMin, ref vMax);
                    OBBVisual vis = Visual as OBBVisual;
                    vMin *= 1.01f;
                    vMax *= 1.01f;
                    vis.SetMaxMin(ref vMax, ref vMin);
                }
                
                Visual.Visible = true;
            }
            else
            {
                mPlacement = new CSUtility.Component.StandardPlacement(this);
                mMin = mMax = SlimDX.Vector3.Zero;
                Visual.Visible = false;
            }
        }
    }
    /// <summary>
    /// 可视OBB包围盒
    /// </summary>
    public class OBBVisual : CCore.Component.Visual
    {
        /// <summary>
        /// 包围框线对象指针列表
        /// </summary>
        protected IntPtr[] mLineObjects;
        /// <summary>
        /// 包围框线对象指针列表
        /// </summary>
        public IntPtr[] LineObjects
        {
            get { return mLineObjects; }
        }
        /// <summary>
        /// 包围盒颜色
        /// </summary>
        protected CSUtility.Support.Color mColor;
        /// <summary>
        /// 包围盒颜色
        /// </summary>
        public CSUtility.Support.Color Color
        {
            get { return mColor; }
            set
            {
                mColor = value;

                var colorValue = value.ToArgb();
                for (int i = 0; i < 24; ++i)
                {
                    DllImportAPI.v3dLineObject_SetColor(mLineObjects[i], (UInt32)colorValue);
                }
            }
        }
        /// <summary>
        /// 最大顶点
        /// </summary>
        protected SlimDX.Vector3 mMax;
        /// <summary>
        /// 最大顶点
        /// </summary>
        public SlimDX.Vector3 Max
        {
            get { return mMax; }
        }
        /// <summary>
        /// 最小顶点
        /// </summary>
        protected SlimDX.Vector3 mMin;
        /// <summary>
        /// 最小顶点
        /// </summary>
        public SlimDX.Vector3 Min
        {
            get { return mMin; }
        }
        /// <summary>
        /// 线条长度比例
        /// </summary>
        protected float mLineLengthRatio;
        /// <summary>
        /// 线条长度比例
        /// </summary>
        public float LineLengthRadio
        {
            get { return mLineLengthRatio; }
            set { mLineLengthRatio = value; }
        }
        /// <summary>
        /// 绑定OBB包围盒矩阵
        /// </summary>
        public SlimDX.Matrix FixObbMatrix = SlimDX.Matrix.Identity;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public OBBVisual()
        {
            mLayer = RLayer.RL_SystemHelper;

            mLineObjects = new IntPtr[24];
            for (int i = 0; i < 24; ++i)
            {
                mLineObjects[i] = DllImportAPI.v3dLineObject_New();
            }

            mMax = SlimDX.Vector3.Zero;
            mMin = SlimDX.Vector3.Zero;
            mLineLengthRatio = 0.3f;
            Color = CSUtility.Support.Color.FromArgb(11, 217, 235);
        }
        /// <summary>
        /// 析构函数，删除对象
        /// </summary>
        ~OBBVisual()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public override void Cleanup()
        {
            if (mLineObjects != null)
            {
                foreach (var obj in mLineObjects)
                {
                    if(obj != IntPtr.Zero)
                        DllImportAPI.v3dLineObject_Release(obj);
                }
                mLineObjects = null;
            }

            base.Cleanup();
        }
        /// <summary>
        /// 提交对象到渲染环境
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">该对象的位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            unsafe
            {
                var mat = FixObbMatrix * matrix; 
                for (int i = 0; i < 24; ++i)
                {   
                    DllImportAPI.vDSRenderEnv_CommitHelperLine(renderEnv.DSRenderEnv, (int)mGroup, mLineObjects[i], &mat);
                }
            }
        }
        /// <summary>
        /// 设置对象的最大顶点和最小顶点
        /// </summary>
        /// <param name="max">最大顶点</param>
        /// <param name="min">最小顶点</param>
        public void SetMaxMin(ref SlimDX.Vector3 max, ref SlimDX.Vector3 min)
        {
            unsafe
            {
                var vStart = min;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[0], &vStart);
                var vEnd = min;
                vEnd.Z = (max.Z - min.Z) * mLineLengthRatio + min.Z;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[0], &vEnd);

                vStart = min;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[1], &vStart);
                vEnd = min;
                vEnd.Y = (max.Y - min.Y) * mLineLengthRatio + min.Y;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[1], &vEnd);

                vStart = min;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[2], &vStart);
                vEnd = min;
                vEnd.X = (max.X - min.X) * mLineLengthRatio + min.X;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[2], &vEnd);

                var vTemp = min;
                vTemp.X = max.X;
                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[3], &vStart);
                vEnd = vTemp;
                vEnd.X = max.X - (max.X - min.X) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[3], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[4], &vStart);
                vEnd = vTemp;
                vEnd.Y = (max.Y - min.Y) * mLineLengthRatio + min.Y;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[4], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[5], &vStart);
                vEnd = vTemp;
                vEnd.Z = (max.Z - min.Z) * mLineLengthRatio + min.Z;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[5], &vEnd);

                vTemp = max;
                vTemp.Z = min.Z;
                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[6], &vStart);
                vEnd = vTemp;
                vEnd.Y = max.Y - (max.Y - min.Y) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[6], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[7], &vStart);
                vEnd = vTemp;
                vEnd.Z = (max.Z - min.Z) * mLineLengthRatio + min.Z;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[7], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[8], &vStart);
                vEnd = vTemp;
                vEnd.X = max.X - (max.X - min.X) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[8], &vEnd);

                vTemp = min;
                vTemp.Y = max.Y;
                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[9], &vStart);
                vEnd = vTemp;
                vEnd.Y = max.Y - (max.Y - min.Y) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[9], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[10], &vStart);
                vEnd = vTemp;
                vEnd.X = (max.X - min.X) * mLineLengthRatio + min.X;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[10], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[11], &vStart);
                vEnd = vTemp;
                vEnd.Z = (max.Z - min.Z) * mLineLengthRatio + min.Z;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[11], &vEnd);

                vTemp = max;
                vTemp.X = min.X;
                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[12], &vStart);
                vEnd = vTemp;
                vEnd.Z = max.Z - (max.Z - min.Z) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[12], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[13], &vStart);
                vEnd = vTemp;
                vEnd.X = (max.X - min.X) * mLineLengthRatio + min.X;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[13], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[14], &vStart);
                vEnd = vTemp;
                vEnd.Y = max.Y - (max.Y - min.Y) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[14], &vEnd);

                vTemp = min;
                vTemp.Z = max.Z;
                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[15], &vStart);
                vEnd = vTemp;
                vEnd.Y = (max.Y - min.Y) * mLineLengthRatio + min.Y;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[15], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[16], &vStart);
                vEnd = vTemp;
                vEnd.Z = max.Z - (max.Z - min.Z) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[16], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[17], &vStart);
                vEnd = vTemp;
                vEnd.X = (max.X - min.X) * mLineLengthRatio + min.X;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[17], &vEnd);

                vTemp = max;
                vTemp.Y = min.Y;
                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[18], &vStart);
                vEnd = vTemp;
                vEnd.X = max.X - (max.X - min.X) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[18], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[19], &vStart);
                vEnd = vTemp;
                vEnd.Z = max.Z - (max.Z - min.Z) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[19], &vEnd);

                vStart = vTemp;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[20], &vStart);
                vEnd = vTemp;
                vEnd.Y = (max.Y - min.Y) * mLineLengthRatio + min.Y;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[20], &vEnd);

                vStart = max;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[21], &vStart);
                vEnd = max;
                vEnd.Y = max.Y - (max.Y - min.Y) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[21], &vEnd);

                vStart = max;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[22], &vStart);
                vEnd = max;
                vEnd.Z = max.Z - (max.Z - min.Z) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[22], &vEnd);

                vStart = max;
                DllImportAPI.v3dLineObject_SetStart(mLineObjects[23], &vStart);
                vEnd = max;
                vEnd.X = max.X - (max.X - min.X) * mLineLengthRatio;
                DllImportAPI.v3dLineObject_SetEnd(mLineObjects[23], &vEnd);
            }
        }
    }
}
