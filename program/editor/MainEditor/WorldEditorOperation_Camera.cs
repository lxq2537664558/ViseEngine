using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainEditor
{
    public partial class WorldEditorOperation
    {
        public CCore.Camera.CameraController FreeCameraController
        {
            get;
            protected set;
        }

        void Initialize_Camera()
        {
            FreeCameraController = new CCore.Camera.MayaPosCameraController();
            FreeCameraController.Camera = mREnviroment.Camera;
            EditorCommon.Program.EditorCameraController = FreeCameraController;
            EditorCommon.GameCameraOperation.OnCameraOperationed += GameCameraOperation_OnCameraOperationed;

            SlimDX.Vector3 pos = new SlimDX.Vector3(-20, 20, -20);
            SlimDX.Vector3 lookAt = new SlimDX.Vector3(10, 0, 10);
            SlimDX.Vector3 up = new SlimDX.Vector3(0, 1, 0);
            FreeCameraController.SetPosLookAtUp(ref pos, ref lookAt, ref up);
        }

        private void GameCameraOperation_OnCameraOperationed()
        {
            ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
        }

        void FinalInstance_Camera()
        {
            FreeCameraController = null;
        }

        private void Tick_Camera()
        {
            //if(FreeCameraController != null)
            //{
            //    if(IsEditorMode)
            //    {
            //        FreeCameraController.Tick();

            //        var tIV = IntersectWithScene(REnviroment.View.Width / 2, REnviroment.View.Height / 2);
            //        CCore.Client.MainWorldInstance.TravelTo(tIV.X, tIV.Z);
            //    }
            //}
        }

        //Point mStartRotateCameraMousePos;
        //// 在开始旋转摄像机时设置初始值
        //public void StartTransCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;
        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;
        //}
        //// 旋转摄像机
        //public void RotateCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    //if (EditorCommon.WorldEditMode.Instance.EditMode != EditorCommon.WorldEditMode.enEditMode.Edit_Camera)
        //    //    return;

        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;

        //    var deltaX = mouseMoveParam.X - mStartRotateCameraMousePos.X;
        //    var deltaY = mouseMoveParam.Y - mStartRotateCameraMousePos.Y;

        //    FreeCameraController.Turn(CCore.CoordAxis.Y, deltaX * 0.01f);
        //    FreeCameraController.Turn(CCore.CoordAxis.X, deltaY * 0.01f);

        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;

        //    ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
        //}
        //// XZ平面移动摄像机
        //public void HorizontalMoveCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    //if (EditorCommon.WorldEditMode.Instance.EditMode != EditorCommon.WorldEditMode.enEditMode.Edit_Camera)
        //    //    return;

        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;

        //    var deltaX = mouseMoveParam.X - mStartRotateCameraMousePos.X;
        //    var deltaY = mouseMoveParam.Y - mStartRotateCameraMousePos.Y;

        //    var vFDir = FreeCameraController.Camera.Direction;
        //    vFDir.Y = 0;
        //    vFDir.Normalize();
        //    vFDir *= deltaY * MainEditor.MainWindow.Instance.CameraMoveSpdRate;

        //    FreeCameraController.Move(vFDir);
        //    FreeCameraController.Move(CCore.CoordAxis.X, -deltaX * MainEditor.MainWindow.Instance.CameraMoveSpdRate);

        //    //SlimDX.Vector3 tInt = IntersectWithTerrain(mREnviroment.View.Width / 2, mREnviroment.View.Height / 2, true);
        //    ////MidLayer.IClient.MainWorldInstance.TravelTo(tInt.X, tInt.Z);
        //    //if (SlimDX.Vector3.Dot(FreeCameraController.Camera.Direction, -SlimDX.Vector3.UnitY) > 0)
        //    //{
        //    //    SlimDX.Vector3 pos = FreeCameraController.Camera.Location;
        //    //    SlimDX.Vector3 up = FreeCameraController.Camera.GetYVector();
        //    //    FreeCameraController.SetPosLookAtUp(ref pos, ref tInt, ref up);
        //    //}

        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;

        //    ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
        //}
        //// 按视平面移动摄像机
        //public void ScreenMoveCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    //if (EditorCommon.WorldEditMode.Instance.EditMode != EditorCommon.WorldEditMode.enEditMode.Edit_Camera)
        //    //    return;

        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;

        //    var deltaX = mouseMoveParam.X - mStartRotateCameraMousePos.X;
        //    var deltaY = mouseMoveParam.Y - mStartRotateCameraMousePos.Y;

        //    var vFDir = FreeCameraController.Camera.Direction;
        //    vFDir.Y = 0;
        //    vFDir.Normalize();
        //    vFDir *= deltaY * MainEditor.MainWindow.Instance.CameraMoveSpdRate;

        //    FreeCameraController.Move(CCore.CoordAxis.X, -deltaX * MainEditor.MainWindow.Instance.CameraMoveSpdRate);
        //    FreeCameraController.Move(CCore.CoordAxis.Y, deltaY * MainEditor.MainWindow.Instance.CameraMoveSpdRate);

        //    //SlimDX.Vector3 tInt = IntersectWithTerrain(mREnviroment.View.Width / 2, mREnviroment.View.Height / 2, true);
        //    ////MidLayer.IClient.MainWorldInstance.TravelTo(tInt.X, tInt.Z);
        //    //if (SlimDX.Vector3.Dot(FreeCameraController.Camera.Direction, -SlimDX.Vector3.UnitY) > 0)
        //    //{
        //    //    SlimDX.Vector3 pos = FreeCameraController.Camera.Location;
        //    //    SlimDX.Vector3 up = FreeCameraController.Camera.GetYVector();
        //    //    FreeCameraController.SetPosLookAtUp(ref pos, ref tInt, ref up);
        //    //}

        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;

        //    ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
        //}
        //// 按摄像机方向移动摄像机
        //public void DirMoveCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    //if (EditorCommon.WorldEditMode.Instance.EditMode != EditorCommon.WorldEditMode.enEditMode.Edit_Camera)
        //    //    return;

        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;

        //    var deltaX = mouseMoveParam.X - mStartRotateCameraMousePos.X;
        //    var deltaY = mouseMoveParam.Y - mStartRotateCameraMousePos.Y;

        //    int delta = (deltaX + deltaY) / 2;
        //    FreeCameraController.Move(CCore.CoordAxis.Z, delta * 30 * MainEditor.MainWindow.Instance.CameraZoomSpdRate);

        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;
            
        //    ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
        //}
        //// 按摄像机方向移动摄像机
        //public void DirMoveCamera2(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    //if (EditorCommon.WorldEditMode.Instance.EditMode != EditorCommon.WorldEditMode.enEditMode.Edit_Camera)
        //    //    return;

        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Wheel;
            
        //    FreeCameraController.Move(CCore.CoordAxis.Z, mouseMoveParam.delta * MainEditor.MainWindow.Instance.CameraZoomSpdRate);

        //    ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
        //}

    }
}
