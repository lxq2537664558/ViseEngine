using System;

namespace CCore.World
{
    /// <summary>
    /// mesh对象的初始化类
    /// </summary>
    public class MeshActorInit : ActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MeshActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Common;
            SceneFlag = CSUtility.Component.enActorSceneFlag.Static;
        }
    }
    /// <summary>
    /// mesh对象类
    /// </summary>
    public class MeshActor : Actor
    {

        CCore.Mesh.EdgeDetectMode oldEdgeMode = CCore.Mesh.EdgeDetectMode.Outline;
        SlimDX.Vector4 oldEdgeColor = SlimDX.Vector4.Zero;
        bool oldEdgeDetect = false;
        //float oldEdgeHeightlight = 0;
        /// <summary>
        /// 鼠标悬停在mesh对象上的状态
        /// </summary>
        public override void OnMouseEnter()
        {
            var meshVisual = Visual as CCore.Mesh.Mesh;
            if (meshVisual == null)
                return;

            meshVisual.EdgeDetectHightlight = 1.0f;
            meshVisual.mEdgeDetect = true;
        }
        /// <summary>
        /// 鼠标离开mesh对象时的状态
        /// </summary>
        public override void OnMouseLeave()
        {
            var meshVisual = Visual as CCore.Mesh.Mesh;
            if (meshVisual == null)
                return;

            if (!mIsSelected)
            {
                meshVisual.EdgeDetectHightlight = 0.0f;
                meshVisual.mEdgeDetect = false;
            }
        }
        /// <summary>
        /// 被选中时的状态
        /// </summary>
        public override void Editor_Selected()
        {
            var meshVisual = Visual as CCore.Mesh.Mesh;
            if (meshVisual == null)
                return;

            base.Editor_Selected();

            oldEdgeMode = meshVisual.EdgeDetectMode;
            meshVisual.EdgeDetectMode = CCore.Mesh.EdgeDetectMode.OutlineHighlight;
            oldEdgeColor = meshVisual.EdgeDetectColor;
            meshVisual.EdgeDetectColor = new SlimDX.Vector4(0, 0.9f, 1, 1.0f);
            meshVisual.EdgeDetectHightlight = 1.0f;
            oldEdgeDetect = meshVisual.mEdgeDetect;
            meshVisual.mEdgeDetect = true;
        }
        /// <summary>
        /// 未被选中的状态
        /// </summary>
        public override void Editor_UnSelected()
        {
            var meshVisual = Visual as CCore.Mesh.Mesh;
            if (meshVisual == null)
                return;

            base.Editor_UnSelected();

            meshVisual.EdgeDetectMode = oldEdgeMode;
            meshVisual.EdgeDetectColor = oldEdgeColor;
            meshVisual.EdgeDetectHightlight = 0;
            meshVisual.mEdgeDetect = oldEdgeDetect;
       }
        /// <summary>
        /// 提交可视化对象
        /// </summary>
        /// <param name="env">渲染环境</param>
        /// <param name="matrix">可视化对象的矩阵</param>
        /// <param name="eye">视野</param>
        public override void OnCommitVisual(CCore.Graphics.REnviroment env, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            //var meshVisual = Visual as FrameSet.Mesh.Mesh;
            //if(meshVisual != null)
            //    meshVisual.CommitSocketItem(env, ref matrix, eye);
        }
        /// <summary>
        /// 是否需要连线检查
        /// </summary>
        /// <param name="flag">点击标志</param>
        /// <returns>如果进行连线检查那么返回true，否则返回false</returns>
        public override bool NeedLineCheck(UInt32 flag)
        {
            if ((((CSUtility.enHitFlag)flag) & CSUtility.enHitFlag.IgnoreMouseLineCheckInGame) == CSUtility.enHitFlag.IgnoreMouseLineCheckInGame)
            {
                if(Visual != null)
                {
                    var meshInit = Visual.VisualInit as CCore.Mesh.MeshInit;
                    if(meshInit != null && meshInit.MeshTemplate != null)
                    {
                        if(meshInit.MeshTemplate.IgnoreMouseLineCheckInGame)
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public override void Tick(long elapsedMillisecond)
        {
            TickMeshActorTimer.Begin();
            base.Tick(elapsedMillisecond);
            TickMeshActorTimer.End();
        }

        static CSUtility.Performance.PerfCounter TickMeshActorTimer = new CSUtility.Performance.PerfCounter("TileScene.TickMeshActor");
    }
}
