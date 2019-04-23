using System;
using System.Collections.Generic;

namespace CCore.Component
{
    /// <summary>
    /// 可视化场景点组
    /// </summary>
    public class ScenePointGroupVisual : Visual
    {
        /// <summary>
        /// 直线
        /// </summary>
        protected List<IntPtr> mLineObjects = new List<IntPtr>();
        /// <summary>
        /// 场景点组的类型
        /// </summary>
        public CSUtility.Map.ScenePointGroup.enScenePointGroupType PointType;

        CSUtility.Support.Color mColor;
        /// <summary>
        /// 场景点的颜色
        /// </summary>
        public CSUtility.Support.Color Color
        {
            get { return mColor; }
            set
            {
                mColor = value;

                var colorValue = value.ToArgb();
                foreach (var ptr in mLineObjects)
                {
                    DllImportAPI.v3dLineObject_SetColor(ptr, (UInt32)colorValue);
                }
            }
        }
        /// <summary>
        /// 场景点的构造函数
        /// </summary>
        public ScenePointGroupVisual()
        {
            mLayer = CCore.RLayer.RL_SystemHelper;
            Color = CSUtility.Support.Color.LightGreen;
        }
        /// <summary>
        /// 析构函数，释放内存
        /// </summary>
        ~ScenePointGroupVisual()
        {
            Cleanup();
        }
        /// <summary>
        /// 场景点的清除
        /// </summary>
        public override void Cleanup()
        {
            foreach (var ptr in mLineObjects)
            {
                if (ptr != IntPtr.Zero)
                    DllImportAPI.v3dLineObject_Release(ptr);
            }
            mLineObjects.Clear();

            base.Cleanup();
        }

        /// <summary>
        /// 更新场景点
        /// </summary>
        /// <param name="group">需要更新的场景点组</param>
        public void UpdatePoints(CSUtility.Map.ScenePointGroup group)
        {
            if (group == null)
                return;

            if (group.Points.Count <= 0)
            {
                foreach (var ptr in mLineObjects)
                {
                    if (ptr != IntPtr.Zero)
                        DllImportAPI.v3dLineObject_Release(ptr);
                }
                mLineObjects.Clear();
            }
            else
            {
                switch (group.LineType)
                {
                    case CSUtility.Map.ScenePointGroup.enLineType.Line:
                        {
                            var groupPtsCount = group.Points.Count - 1;
                            if (mLineObjects.Count > groupPtsCount)
                            {
                                for (int i = groupPtsCount; i < mLineObjects.Count; i++)
                                {
                                    var ptr = mLineObjects[i];
                                    if (ptr != IntPtr.Zero)
                                        DllImportAPI.v3dLineObject_Release(ptr);
                                }
                                mLineObjects.RemoveRange(groupPtsCount, mLineObjects.Count - groupPtsCount);
                            }
                            else if (mLineObjects.Count < groupPtsCount)
                            {
                                var objCount = mLineObjects.Count;
                                var cnt = groupPtsCount - objCount;
                                var colorValue = Color.ToArgb();
                                for (int i = 0; i < cnt; i++)
                                {
                                    mLineObjects.Add(DllImportAPI.v3dLineObject_New());
                                    DllImportAPI.v3dLineObject_SetColor(mLineObjects[objCount + i], (UInt32)colorValue);
                                }
                            }

                            unsafe
                            {
                                var vStart = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Zero, group.Points[0].TransMatrix);

                                for (int i = 0; i < groupPtsCount; i++)
                                {
                                    var vEnd = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Zero, group.Points[i+1].TransMatrix);

                                    DllImportAPI.v3dLineObject_SetStart(mLineObjects[i], &vStart);
                                    DllImportAPI.v3dLineObject_SetEnd(mLineObjects[i], &vEnd);

                                    vStart = vEnd;
                                }
                            }
                        }
                        break;

                    case CSUtility.Map.ScenePointGroup.enLineType.Spline:
                        {
                            var ptCount = group.Points.Count * 6;
                            if (mLineObjects.Count > ptCount)
                            {
                                for (int i = ptCount; i < mLineObjects.Count; i++)
                                {
                                    var ptr = mLineObjects[i];
                                    if (ptr != IntPtr.Zero)
                                        DllImportAPI.v3dLineObject_Release(ptr);
                                }
                                mLineObjects.RemoveRange(ptCount, mLineObjects.Count - ptCount);
                            }
                            else if (mLineObjects.Count < ptCount)
                            {
                                var objCount = mLineObjects.Count;
                                var cnt = ptCount - objCount;
                                var colorValue = Color.ToArgb();
                                for (int i = 0; i < cnt; i++)
                                {
                                    mLineObjects.Add(DllImportAPI.v3dLineObject_New());
                                    DllImportAPI.v3dLineObject_SetColor(mLineObjects[objCount + i], (UInt32)colorValue);
                                }
                            }

                            unsafe
                            {
                                var start = group.GetPosition(0);

                                for (int i = 0; i < ptCount; i++)
                                {
                                    var end = group.GetPosition((i + 1.0f) / ptCount);

                                    DllImportAPI.v3dLineObject_SetStart(mLineObjects[i], &start);
                                    DllImportAPI.v3dLineObject_SetEnd(mLineObjects[i], &end);

                                    start = end;
                                }
                            }
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 提交场景点到需要的场景中
        /// </summary>
        /// <param name="renderEnv">提交到的场景</param>
        /// <param name="matrix">场景点矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            unsafe
            {
                switch (PointType)
                {
                    case CSUtility.Map.ScenePointGroup.enScenePointGroupType.NavigationPoint:
                        {
                            if (!CCore.Program.IsActorTypeShow(HostActor.World, CCore.Program.NavigationPointTypeName))
                                return;
                        }
                        break;

                    default:
                        {
                            if (!CCore.Program.IsActorTypeShow(HostActor.World, CCore.Program.ScenePointTypeName))
                                return;
                        }
                        break;
                }

                var mat = matrix;
                foreach (var ptr in mLineObjects)
                {
                    DllImportAPI.vDSRenderEnv_CommitHelperLine(renderEnv.DSRenderEnv, (int)mGroup, ptr, &mat);
                }
            }
        }
    }
}
