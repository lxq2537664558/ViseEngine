using System;
using System.ComponentModel;

namespace CCore.Support
{
    /// <summary>
    /// 网格线检查使用的Actor初始化类
    /// </summary>
    public class LinkCheckAssistActorInit : CCore.World.ActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LinkCheckAssistActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Other;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager |
                        CSUtility.Component.ActorInitBase.EActorFlag.ForEditor;
        }
    }
    /// <summary>
    /// 检查网格线的Actor对象
    /// </summary>
    public class LineCheckAssistActor : CCore.World.Actor, INotifyPropertyChanged
    {
        CCore.Component.Line mViewLine = new CCore.Component.Line();
        CCore.Component.Line mLine = new CCore.Component.Line();
        CCore.Mesh.Mesh mLineCheckPointMesh = new CCore.Mesh.Mesh();
        CCore.Mesh.Mesh mServerAltitudeMesh = new CCore.Mesh.Mesh();

        SlimDX.Vector3 mLineCheckPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 检查网格线的坐标
        /// </summary>
        public SlimDX.Vector3 LineCheckPos
        {
            get { return mLineCheckPos; }
            set
            {
                mLineCheckPos = value;
            }
        }

        SlimDX.Vector3 mServerAltitudePos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 服务器高度图的位置坐标
        /// </summary>
        public SlimDX.Vector3 ServerAltitudePos
        {
            get { return mServerAltitudePos; }
            set
            {
                mServerAltitudePos = value;
            }
        }

        SlimDX.Vector3 mStartPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 起点位置坐标
        /// </summary>
        public SlimDX.Vector3 StartPos
        {
            get { return mStartPos; }
            set
            {
                mStartPos = value;
                mLine.Start = StartPos;
            }
        }

        SlimDX.Vector3 mEndPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 终点位置坐标
        /// </summary>
        public SlimDX.Vector3 EndPos
        {
            get { return mEndPos; }
            set
            {
                mEndPos = value;
                mLine.End = EndPos;
            }
        }

        SlimDX.Vector3 mViewLineStart = SlimDX.Vector3.Zero;
        /// <summary>
        /// 观察线的起点坐标
        /// </summary>
        public SlimDX.Vector3 ViewLineStart
        {
            get { return mViewLineStart; }
            set
            {
                mViewLineStart = value;
                mViewLine.Start = mViewLineStart;
            }
        }
        SlimDX.Vector3 mViewLineEnd = SlimDX.Vector3.Zero;
        /// <summary>
        /// 观察线的终点坐标
        /// </summary>
        public SlimDX.Vector3 ViewLineEnd
        {
            get { return mViewLineEnd; }
            set
            {
                mViewLineEnd = value;
                mViewLine.End = mViewLineEnd;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public LineCheckAssistActor()
        {
            mPlacement = new CSUtility.Component.StandardPlacement(this);

            var meshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = CSUtility.Support.IFileConfig.LineCheckAssistMeshTemplate
            };
            mLineCheckPointMesh.Initialize(meshInit, null);
            mLineCheckPointMesh.CanHitProxy = false;

            meshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = CSUtility.Support.IFileConfig.ServerAltitudeAssistMeshTemplate
            };
            mServerAltitudeMesh.Initialize(meshInit, null);
            mServerAltitudeMesh.CanHitProxy = false;
        }
        /// <summary>
        /// 提交可视化对象
        /// </summary>
        /// <param name="env">渲染环境</param>
        /// <param name="matrix">位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void OnCommitVisual(CCore.Graphics.REnviroment env, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            base.OnCommitVisual(env, ref matrix, eye);

            var mat = SlimDX.Matrix.Identity;
            mLine.Commit(env, ref mat, eye);
            mViewLine.Commit(env, ref mat, eye);

            mat = SlimDX.Matrix.Translation(LineCheckPos);
            mLineCheckPointMesh.Commit(env, ref mat, eye);
            mat = SlimDX.Matrix.Translation(ServerAltitudePos);
            mServerAltitudeMesh.Commit(env, ref mat, eye);
        }
    }
}
