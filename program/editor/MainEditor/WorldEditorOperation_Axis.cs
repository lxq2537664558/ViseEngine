using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MainEditor
{
    public partial class WorldEditorOperation
    {
        // 坐标轴对象
        CCore.Support.V3DAxis m3dAxis;

        void Initialzie_Axis()
        {
            var axisInit = new CCore.Support.V3DAxisInit();
            m3dAxis = new CCore.Support.V3DAxis();
            m3dAxis.Initialize(axisInit);
            m3dAxis.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            m3dAxis.ActorName = "坐标轴";

            foreach (CCore.Support.V3DAxis.enAxisType value in System.Enum.GetValues(typeof(CCore.Support.V3DAxis.enAxisType)))
            {
                m3dAxis.SetHitProxy(value, CCore.Graphics.HitProxyMap.Instance.GenHitProxy(m3dAxis.Id));
            }

            EditorCommon.Program.Game3dAxis = m3dAxis;
            EditorCommon.WorldEditMode.Instance.OnEditModeChanged += WorldEditMode_OnEditModeChanged;
        }

        private void WorldEditMode_OnEditModeChanged(EditorCommon.WorldEditMode.enEditMode oldEditMode, EditorCommon.WorldEditMode.enEditMode newEditMode)
        {
            if (m3dAxis == null)
                return;

            switch (newEditMode)
            {
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove:
                    m3dAxis.EditMode = CCore.Support.V3DAxis.enEditMode.Move;
                    if (m3dAxis.HasTargets())
                    {
                        m3dAxis.Visible = true;
                        ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
                    }
                    else
                        m3dAxis.Visible = false;
                    break;
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot:
                    m3dAxis.EditMode = CCore.Support.V3DAxis.enEditMode.Rot;
                    if (m3dAxis.HasTargets())
                    {
                        m3dAxis.Visible = true;
                        ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
                    }
                    else
                        m3dAxis.Visible = false;
                    break;
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale:
                    m3dAxis.EditMode = CCore.Support.V3DAxis.enEditMode.Scale;
                    if (m3dAxis.HasTargets())
                    {
                        m3dAxis.Visible = true;
                        ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
                    }
                    else
                        m3dAxis.Visible = false;
                    break;
                default:
                    m3dAxis.Visible = false;
                    break;
            }
        }

        void OnWorldLoaded_Axis()
        {
            CCore.Client.MainWorldInstance.AddEditorActor(m3dAxis);
        }
        void FinalInstance_Axis()
        {
            EditorCommon.WorldEditMode.Instance.OnEditModeChanged -= WorldEditMode_OnEditModeChanged;
            m3dAxis = null;
        }

        public void SetAxisTargets(CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> actors)
        {
            m3dAxis.SetTargets(actors);
            if(actors == null || actors.Count == 0 || 
               (EditorCommon.WorldEditMode.Instance.EditMode != EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove &&
                EditorCommon.WorldEditMode.Instance.EditMode != EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot &&
                EditorCommon.WorldEditMode.Instance.EditMode != EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale))
            {
                m3dAxis.Visible = false;
            }
            else
            {
                ScaleAxis(GameAssistWindow.Instance.AxisScaleRate);
                m3dAxis.Visible = true;
            }
        }

        public void SetAxisMode(CCore.Support.V3DAxis.enAxisMode mode)
        {
            m3dAxis.AxisMode = mode;
        }

        public void SetAxisOperationType(CCore.Support.V3DAxis.enOperationType type)
        {
            m3dAxis.OperationType = type;
        }

        public void ScaleAxis(float scaleRate)
        {
            if (m3dAxis == null || m3dAxis.Placement == null)
                return;

            var screenSize = 0.2f;
            var size = FreeCameraController.Camera.GetScreenSizeInWorld(m3dAxis.Placement.GetLocation(), screenSize);
            var vScale = SlimDX.Vector3.UnitXYZ;
            var meshSize = 1.0f;
            vScale = size * vScale / meshSize * scaleRate;
            m3dAxis.Placement.SetScale(ref vScale);
        }

        //public bool IsTranslatingAxis(CCore.MsgProc.BehaviorParameter parameter)
        //{
        //    var mouseMove = parameter as CCore.MsgProc.Behavior.Mouse_Move;
        //    if (mouseMove == null)
        //        return false;

        //    var command = EditorCommon.Hotkey.HotkeyManager.Instance.GetCommand("游戏窗口操作", "操作坐标轴");
        //    bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        //    bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        //    bool alt = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
        //    bool assistKey = (ctrl == command.WithCtrl && shift == command.WithShift && alt == command.WithAlt);
        //    switch(command.Hotkey)
        //    {
        //        case CCore.MsgProc.BehaviorParameter.enKeys.LButton:
        //            if (mouseMove.IsKeyDown(CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left) && assistKey)
        //                return true;
        //            break;
        //        case CCore.MsgProc.BehaviorParameter.enKeys.RButton:
        //            if (mouseMove.IsKeyDown(CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right) && assistKey)
        //                return true;
        //            break;
        //        case CCore.MsgProc.BehaviorParameter.enKeys.MButton:
        //            if (mouseMove.IsKeyDown(CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Middle) && assistKey)
        //                return true;
        //            break;
        //    }

        //    command = EditorCommon.Hotkey.HotkeyManager.Instance.GetCommand("游戏窗口操作", "复制对象");
        //    ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        //    shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        //    alt = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
        //    assistKey = (ctrl == command.WithCtrl && shift == command.WithShift && alt == command.WithAlt);
        //    switch (command.Hotkey)
        //    {
        //        case CCore.MsgProc.BehaviorParameter.enKeys.LButton:
        //            if (mouseMove.IsKeyDown(CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left) && assistKey)
        //                return true;
        //            break;
        //        case CCore.MsgProc.BehaviorParameter.enKeys.RButton:
        //            if (mouseMove.IsKeyDown(CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right) && assistKey)
        //                return true;
        //            break;
        //        case CCore.MsgProc.BehaviorParameter.enKeys.MButton:
        //            if (mouseMove.IsKeyDown(CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Middle) && assistKey)
        //                return true;
        //            break;
        //    }
        //    return false;
        //}

        bool mStart = false;
        SlimDX.Vector3 mOldPosition = SlimDX.Vector3.Zero;
        SlimDX.Vector3 mMouseIntersectOffset = SlimDX.Vector3.Zero;
        SlimDX.Matrix mOldRotMat = SlimDX.Matrix.Identity;
        SlimDX.Quaternion mOldRotation = SlimDX.Quaternion.Identity;
        SlimDX.Vector3 mOldMouseIntersectPos = SlimDX.Vector3.Zero;
        SlimDX.Vector3 mTangentPlaneNormal = SlimDX.Vector3.UnitY;  // 旋转切线平面
        SlimDX.Vector3 mTangentDir = SlimDX.Vector3.UnitX;          // 旋转切线方向
        SlimDX.Vector3 mRotAxis = SlimDX.Vector3.UnitY;             // 旋转轴
        float mRotRadius = 1.0f;
        SlimDX.Vector3 mScaleXYZDir = SlimDX.Vector3.UnitY;
        bool IsTranslatingAxis = false;

        // 开始操作坐标轴
        public void StartTranslateAxis(CCore.MsgProc.BehaviorParameter parameter, object obj)
        {
            mOldPosition = m3dAxis.Placement.GetLocation();
            mOldRotation = m3dAxis.Placement.GetRotation();
            mOldRotMat = SlimDX.Matrix.RotationQuaternion(mOldRotation);
            mMouseIntersectOffset = SlimDX.Vector3.Zero;

            var right = SlimDX.Vector3.Cross(SlimDX.Vector3.UnitY, REnviroment.Camera.Direction);
            mScaleXYZDir = SlimDX.Vector3.Cross(REnviroment.Camera.Direction, right);
            mScaleXYZDir.Normalize();

            mStart = true;
            IsTranslatingAxis = true;
        }

        public void EndTranslateAxis(CCore.MsgProc.BehaviorParameter parameter, object obj)
        {
            IsTranslatingAxis = false;
        }

        void CalculateRotMatrix(int x, int y, ref SlimDX.Vector3 vNowIntersectPos, ref SlimDX.Vector3 vOldIntersectPos, ref SlimDX.Matrix matDelta, ref SlimDX.Matrix matFinal)
        {
            var newPt = IntersectWithZeroPlane(x, y, vOldIntersectPos, mTangentPlaneNormal);
            var length = SlimDX.Vector3.Dot((newPt - vOldIntersectPos), mTangentDir);
            var angle = length / mRotRadius;
            if(GameAssistWindow.Instance.IsRotSnap)
            {
                float snapAngle = (float)(GameAssistWindow.Instance.RotSnapAngle / 180.0f * System.Math.PI);
                if(System.Math.Abs(angle) > snapAngle)
                {
                    snapAngle = (int)(angle / snapAngle) * snapAngle;
                    matDelta = SlimDX.Matrix.RotationAxis(mRotAxis, snapAngle);
                    matFinal *= matDelta;
                    vOldIntersectPos = vNowIntersectPos;
                }
            }
            else
            {
                matDelta = SlimDX.Matrix.RotationAxis(mRotAxis, angle);
                matFinal *= matDelta;

                vOldIntersectPos = newPt;
            }
        }
        void CalculateRotMatrix(ref SlimDX.Vector3 vNowIntersectPos, ref SlimDX.Vector3 vOldIntersectPos, SlimDX.Vector3 vAxisSrc, ref SlimDX.Matrix matDelta, ref SlimDX.Matrix matFinal)
        {
            SlimDX.Vector3 rotAxis = SlimDX.Vector3.UnitY;
            switch (m3dAxis.AxisMode)
            {
                case CCore.Support.V3DAxis.enAxisMode.World:
                    {
                        rotAxis = vAxisSrc;
                    }
                    break;

                case CCore.Support.V3DAxis.enAxisMode.Local:
                    {
                        rotAxis = SlimDX.Vector3.TransformNormal(vAxisSrc, matFinal);
                    }
                    break;
            }

            var v1 = vNowIntersectPos - m3dAxis.Placement.GetLocation();
            v1.Normalize();
            var v2 = vOldIntersectPos - m3dAxis.Placement.GetLocation();
            v2.Normalize();
            float fAngle = (float)Math.Acos(SlimDX.Vector3.Dot(v1, v2));
            var dirAlpha = SlimDX.Vector3.Dot(rotAxis, SlimDX.Vector3.Cross(v1, v2));
            if (GameAssistWindow.Instance.IsRotSnap)
            {
                float snapAngle = (float)(GameAssistWindow.Instance.RotSnapAngle / 180.0f * System.Math.PI);
                if (System.Math.Abs(fAngle) > snapAngle)
                {
                    snapAngle *= (dirAlpha < 0) ? 1 : -1;
                    matDelta = SlimDX.Matrix.RotationAxis(rotAxis, snapAngle);
                    matFinal *= matDelta;

                    vOldIntersectPos = vNowIntersectPos;
                }
            }
            else
            {
                fAngle *= (dirAlpha < 0) ? 1 : -1;
                matDelta = SlimDX.Matrix.RotationAxis(rotAxis, fAngle);
                matFinal *= matDelta;

                vOldIntersectPos = vNowIntersectPos;
            }
        }

        // 沿轴向移动
        private SlimDX.Vector3 MoveWithAxis(int x, int y, SlimDX.Vector3 transAxis)
        {
            SlimDX.Vector3 end;
            var tempDir = SlimDX.Vector3.Cross(transAxis, REnviroment.Camera.Direction);
            tempDir.Normalize();
            var oriAxis = SlimDX.Vector3.Cross(transAxis, tempDir);
            oriAxis.Normalize();
            //var planeNormal = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitZ, mOldRotMat);
            end = IntersectWithZeroPlane(x, y, mOldPosition, oriAxis);
            end += mMouseIntersectOffset;
            float length = SlimDX.Vector3.Dot((end - mOldPosition), transAxis);
            end = transAxis * length + mOldPosition;
            GetNearstSnapPosition(end, ref end, mSelectedActors);
            return end;
        }
        // 沿平面移动
        private SlimDX.Vector3 MoveWithPlane(int x, int y, SlimDX.Vector3 transAxis)
        {
            var end = IntersectWithZeroPlane(x, y, mOldPosition, transAxis);
            end += mMouseIntersectOffset;
            GetNearstSnapPosition(end, ref end, mSelectedActors);
            return end;
        }

        // 操作坐标轴
        public void TranslateAxis(CCore.MsgProc.BehaviorParameter parameter, object obj)
        {
            var param = parameter as CCore.MsgProc.Behavior.Mouse_Move;
            if (param == null)
                return;

            var axisLoc = m3dAxis.Placement.GetLocation();
            var end = mOldPosition;
            SlimDX.Vector3 transAxis = SlimDX.Vector3.Zero;

            switch (m3dAxis.AxisType)
            {
                case CCore.Support.V3DAxis.enAxisType.Move_X:
                    {
                        switch (m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    transAxis = SlimDX.Vector3.UnitX;
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    transAxis = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitX, mOldRotMat);
                                }
                                break;
                        }
                        end = MoveWithAxis(param.X, param.Y, transAxis);
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Move_Y:
                    {
                        switch(m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = MoveWithAxis(param.X, param.Y, SlimDX.Vector3.UnitY);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    transAxis = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitY, mOldRotMat);
                                    end = MoveWithAxis(param.X, param.Y, transAxis);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Move_Z:
                    {
                        switch (m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = MoveWithAxis(param.X, param.Y, SlimDX.Vector3.UnitZ);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    transAxis = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitZ, mOldRotMat);
                                    end = MoveWithAxis(param.X, param.Y, transAxis);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Move_XY:
                    {
                        switch(m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = MoveWithPlane(param.X, param.Y, SlimDX.Vector3.UnitZ);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var planeNormal = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitZ, mOldRotMat);
                                    end = MoveWithPlane(param.X, param.Y, planeNormal);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Move_XZ:
                    {
                        switch(m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = MoveWithPlane(param.X, param.Y, SlimDX.Vector3.UnitY);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var planeNormal = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitY, mOldRotMat);
                                    end = MoveWithPlane(param.X, param.Y, planeNormal);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Move_YZ:
                    {
                        switch(m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = MoveWithPlane(param.X, param.Y, SlimDX.Vector3.UnitX);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var planeNormal = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitX, mOldRotMat);
                                    end = MoveWithPlane(param.X, param.Y, planeNormal);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Rot_X:
                    {
                        switch (m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = IntersectWithZeroPlane(param.X, param.Y, mOldPosition, SlimDX.Vector3.UnitX);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var planeNormal = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitX, mOldRotMat);
                                    end = IntersectWithZeroPlane(param.X, param.Y, mOldPosition, planeNormal);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Rot_Y:
                    {
                        switch (m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = IntersectWithZeroPlane(param.X, param.Y, mOldPosition, SlimDX.Vector3.UnitY);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var planeNormal = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitY, mOldRotMat);
                                    end = IntersectWithZeroPlane(param.X, param.Y, mOldPosition, planeNormal);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Rot_Z:
                    {
                        switch (m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = IntersectWithZeroPlane(param.X, param.Y, mOldPosition, SlimDX.Vector3.UnitZ);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var planeNormal = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitZ, mOldRotMat);
                                    end = IntersectWithZeroPlane(param.X, param.Y, mOldPosition, planeNormal);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Scale_X:
                    {
                        transAxis = SlimDX.Vector3.UnitX;
                        switch (m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = MoveWithAxis(param.X, param.Y, transAxis);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var axis = SlimDX.Vector3.TransformNormal(transAxis, mOldRotMat);
                                    end = MoveWithAxis(param.X, param.Y, axis);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Scale_Y:
                    {
                        transAxis = SlimDX.Vector3.UnitY;
                        switch (m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = MoveWithAxis(param.X, param.Y, transAxis);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var axis = SlimDX.Vector3.TransformNormal(transAxis, mOldRotMat);
                                    end = MoveWithAxis(param.X, param.Y, axis);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Scale_Z:
                    {
                        transAxis = SlimDX.Vector3.UnitZ;
                        switch (m3dAxis.AxisMode)
                        {
                            case CCore.Support.V3DAxis.enAxisMode.World:
                                {
                                    end = MoveWithAxis(param.X, param.Y, transAxis);
                                }
                                break;
                            case CCore.Support.V3DAxis.enAxisMode.Local:
                                {
                                    var axis = SlimDX.Vector3.TransformNormal(transAxis, mOldRotMat);
                                    end = MoveWithAxis(param.X, param.Y, axis);
                                }
                                break;
                        }
                    }
                    break;
                case CCore.Support.V3DAxis.enAxisType.Scale_XYZ:
                    {
                        end = MoveWithAxis(param.X, param.Y, mScaleXYZDir);
                    }
                    break;
            }

            if (float.IsNaN(end.X) || float.IsNaN(end.Y) || float.IsNaN(end.Z))
            {
                m3dAxis.Placement.SetLocation(ref mOldPosition);
                return;
            }

            var tagQuat = m3dAxis.Placement.GetRotation();
            SlimDX.Matrix scaleMat = SlimDX.Matrix.Identity;
            SlimDX.Matrix rotDelta = SlimDX.Matrix.Identity;
            SlimDX.Matrix rotMat = SlimDX.Matrix.RotationQuaternion(tagQuat);

            float fScaleRate = 0.1f;
            SlimDX.Vector3 vDelta = (end - mOldPosition) * fScaleRate;

            if (mStart)
            {
                mMouseIntersectOffset = mOldPosition - end;
                mOldMouseIntersectPos = end;

                // 计算旋转切线
                // 计算切线面向摄像机的平面
                var radiusVec = mOldMouseIntersectPos - axisLoc;
                var scale = m3dAxis.Placement.GetScale();
                mRotRadius = 0.3f * scale.X * scale.X;// radiusVec.Length();
                mRotAxis = SlimDX.Vector3.UnitY;
                switch(m3dAxis.AxisType)
                {
                    case CCore.Support.V3DAxis.enAxisType.Rot_X:
                        mRotAxis = SlimDX.Vector3.UnitX;
                        break;
                    case CCore.Support.V3DAxis.enAxisType.Rot_Y:
                        mRotAxis = SlimDX.Vector3.UnitY;
                        break;
                    case CCore.Support.V3DAxis.enAxisType.Rot_Z:
                        mRotAxis = SlimDX.Vector3.UnitZ;
                        break;
                }
                if(m3dAxis.AxisMode == CCore.Support.V3DAxis.enAxisMode.Local)
                    mRotAxis = SlimDX.Vector3.TransformNormal(mRotAxis, rotMat);

                mTangentDir = SlimDX.Vector3.Cross(mRotAxis, radiusVec);  // 切线方向
                var temDir = SlimDX.Vector3.Cross(mTangentDir, REnviroment.Camera.Direction);
                mTangentPlaneNormal = SlimDX.Vector3.Cross(mTangentDir, temDir);
                mTangentPlaneNormal.Normalize();

                mStart = false;
            }
            else
            {
                switch (EditorCommon.WorldEditMode.Instance.EditMode)
                {
                    case EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove:
                        {
                            switch(m3dAxis.OperationType)
                            {
                                case CCore.Support.V3DAxis.enOperationType.MoveObject:
                                    {
                                        var delta = end - mOldPosition;
                                        m3dAxis.SetLocationDeltaWithTargets(ref delta);
                                        mOldPosition = end;
                                    }
                                    break;
                                case CCore.Support.V3DAxis.enOperationType.MoveAxis:
                                    m3dAxis.SetAxisLocation(ref end);
                                    break;
                            }
                        }
                        break;
                    case EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot:
                        {
                            CalculateRotMatrix(param.X, param.Y, ref end, ref mOldMouseIntersectPos, ref rotDelta, ref rotMat);

                            switch (m3dAxis.OperationType)
                            {
                                case CCore.Support.V3DAxis.enOperationType.MoveObject:
                                    {
                                        m3dAxis.SetRotationDeltaWithTargets(ref rotDelta);
                                    }
                                    break;
                                case CCore.Support.V3DAxis.enOperationType.MoveAxis:
                                    {
                                        SlimDX.Vector3 vTempTrans, vTempScale;
                                        SlimDX.Quaternion quat;
                                        rotMat.Decompose(out vTempScale, out quat, out vTempTrans);
                                        m3dAxis.SetRotation(ref quat);
                                    }
                                    break;
                            }
                        }
                        break;
                    case EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale:
                        {
                            // 单独缩放坐标轴没有意义，所以这里无论是操作坐标轴还是操作物体，一律操作物体
                            var delta = SlimDX.Vector3.UnitXYZ;
                            var sign = 1;
                            if(m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Scale_XYZ)
                            {
                                var dotValue = SlimDX.Vector3.Dot((end - mOldPosition), mScaleXYZDir);
                                sign = dotValue > 0 ? 1 : -1;
                                delta.Z = delta.Y = delta.X = dotValue;
                                transAxis = SlimDX.Vector3.UnitXYZ;
                            }
                            else
                            {
                                transAxis.Normalize();
                                var dotValue = SlimDX.Vector3.Dot(end - mOldPosition, transAxis);
                                sign = dotValue > 0 ? 1 : -1;
                                delta = dotValue * transAxis;
                            }

                            if(GameAssistWindow.Instance.IsScaleSnap)
                            {
                                var length = delta.Length();
                                if(length > GameAssistWindow.Instance.ScaleSnap)
                                {
                                    length = (int)(length / GameAssistWindow.Instance.ScaleSnap) * GameAssistWindow.Instance.ScaleSnap;
                                    if (sign > 0)
                                        delta = length * transAxis + SlimDX.Vector3.UnitXYZ;
                                    else
                                        delta = SlimDX.Vector3.UnitXYZ - (1 - 1.0f / (length + 1)) * transAxis;
                                    m3dAxis.SetScaleDeltaWithTargets(ref delta);
                                    mOldPosition = end;
                                }
                            }
                            else
                            {
                                delta += SlimDX.Vector3.UnitXYZ;
                                m3dAxis.SetScaleDeltaWithTargets(ref delta);
                                mOldPosition = end;
                            }
                        }
                        break;
                }
            }

        }
    }
}
