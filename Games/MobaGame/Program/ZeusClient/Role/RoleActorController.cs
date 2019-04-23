using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Role
{
    public class RoleCameraParam
    {
        double mEyeFOV = 30;
        public double EyeFOV
        {
            get { return mEyeFOV; }
            set
            {
                mEyeFOV = value;
            }
        }

        double mEye2RoleDistance = 10.0f;
        public double Eye2RoleDistance
        {
            get { return mEye2RoleDistance; }
            set
            {
                mEye2RoleDistance = value;
            }
        }

        double mEyeDirectionX = 0;
        public double EyeDirectionX
        {
            get { return mEyeDirectionX; }
            set
            {
                mEyeDirectionX = value;
            }
        }
        double mEyeDirectionY = 0;
        public double EyeDirectionY
        {
            get { return mEyeDirectionY; }
            set
            {
                mEyeDirectionY = value;
            }
        }
        double mEyeDirectionZ = 0;
        public double EyeDirectionZ
        {
            get { return mEyeDirectionZ; }
            set
            {
                mEyeDirectionZ = value;
            }
        }
    }

    public class ChiefRoleActorController : CCore.Controller.ActorController
    {
        static ChiefRoleActorController mInstance = new ChiefRoleActorController();
        public static ChiefRoleActorController Instance
        {
            get { return mInstance; }
        }

        public static void FinalCleanup()
        {
            mInstance = null;

        }

        RoleCameraParam mDefaultCamer = new RoleCameraParam();
        public override CCore.World.Actor Host
        {
            get
            {
                return base.Host;
            }
            set
            {
                if (base.Host != null && base.Host.Placement != null)
                {
                    base.Host.Placement.OnLocationChanged -= Placement_OnLocationChanged;
                }
                base.Host = value;

                RoleActor role = mHost as RoleActor;
                if (role != null)
                {
                    mCurrentEye2RoleDistance = mDefaultCamer.Eye2RoleDistance;
                    mTargetEye2RoleDistance = mDefaultCamer.Eye2RoleDistance;

                    role.Placement.OnLocationChanged += Placement_OnLocationChanged;
                }
            }
        }

        void Placement_OnLocationChanged(ref SlimDX.Vector3 loc)
        {
            if (mHost == null)
                return;

            CCore.Audio.AudioManager.Instance.DefaultListener.Position = loc;
            CCore.Audio.AudioManager.Instance.DefaultListener.Direction = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.UnitZ, mHost.Placement.GetRotation());
        }

        double mEyeMoveSpeed = 0.01;
        bool mNeedUpdateEyeDistance = false;

        double mCurrentEye2RoleDistance = 0;
        double mTargetEye2RoleDistance = 0;
        double TargetEye2RoleDistance
        {
            get { return mTargetEye2RoleDistance; }
            set
            {
                mTargetEye2RoleDistance = value;
                mNeedUpdateEyeDistance = true;
            }
        }

        SlimDX.Vector3 mEyeDirection = new SlimDX.Vector3(0.31f, -0.71f, 0.65F);
        public SlimDX.Vector3 EyeDirection
        {
            get { return mEyeDirection; }
            set
            {
                mEyeDirection = value;
                mEyeDirection.Normalize();

                mCameraController.Initialize(ref mEyeDirection, mEye2RoleDistance);
            }
        }


        float mEye2RoleDistance = 15;
        public float Eye2RoleDistance
        {
            get { return mEye2RoleDistance; }
            set
            {
                mEye2RoleDistance = value;

                mCameraController.Initialize(ref mEyeDirection, mEye2RoleDistance);
            }
        }

        public double FOV
        {
            get
            {
                return CameraController.Camera.FOV;
            }
            set
            {
                CameraController.Camera.FOV = (float)value;
            }
        }

        public bool FireSkillReady = true;

        protected CCore.Camera.TraceCameraController mCameraController;
        public CCore.Camera.TraceCameraController CameraController
        {
            get { return mCameraController; }
        }

        private CCore.MsgProc.FBehaviorProcess mKBDBehavior;
        private CCore.MsgProc.FBehaviorProcess mKBUBehavior;
        private CCore.MsgProc.FBehaviorProcess mMouseLDBehavior;
        private CCore.MsgProc.FBehaviorProcess mMouseLUBehavior;
        private CCore.MsgProc.FBehaviorProcess mMouseRDBehavior;
        private CCore.MsgProc.FBehaviorProcess mMouseRUBehavior;
        private CCore.MsgProc.FBehaviorProcess mMouseMoveBehavior;
        private CCore.MsgProc.FBehaviorProcess mMouseWheelBehavior;

        public void CreateCameraController(CCore.Camera.CameraObject camera)
        {
            mCameraController = new CCore.Camera.TraceCameraController(mHost);
            mKBDBehavior = this.OnKeyDown;
            mKBUBehavior = this.OnKeyUp;
            mMouseLDBehavior = this.OnMouseLD;
            mMouseLUBehavior = this.OnMouseLU;
            mMouseRDBehavior = this.OnMouseRD;
            mMouseRUBehavior = this.OnMouseRU;
            mMouseMoveBehavior = this.OnMouseMove;
            mMouseWheelBehavior = this.OnMouseWheel;
            mCameraController.Camera = camera;

            mCameraController.Initialize(ref mEyeDirection, mEye2RoleDistance);
            CCore.Camera.CameraController.CurrentCameraController = mCameraController;
        }

    //    int mFindPathDeltaTime = 200;
    //    int mFindPathCurrTime = 0;
        public void Tick()
        {
            if (Game.Instance.IsEditorMode)
                return;
            if (mCameraController != null)
                mCameraController.Tick();

            RoleActor role = mHost as RoleActor;
            if (role == null)
                return;

//             if (mIsMouseRD == true)
//             {
//                 mFindPathCurrTime += (int)CCore.Engine.Instance.GetElapsedMillisecond();
//                 // 每隔mFindPathDeltaTime（比如200ms） 或者
//                 // 鼠标移动超过200像素
//                 if (mFindPathCurrTime > mFindPathDeltaTime ||
//                     (System.Math.Abs(mPreMouseMoveDelta.X) > 200 || System.Math.Abs(mPreMouseMoveDelta.Y) > 200))
//                 {
//                     MouseWalk(role, mOldMousePos.X, mOldMousePos.Y);
// 
//                     mFindPathCurrTime = 0;
//                     mPreMouseMoveDelta.X = 0;
//                     mPreMouseMoveDelta.Y = 0;
//                 }
//             }

            if (mNeedUpdateEyeDistance)
            {
                if (mCurrentEye2RoleDistance < mTargetEye2RoleDistance)
                {
                    mCurrentEye2RoleDistance += mEyeMoveSpeed * CCore.Engine.Instance.GetElapsedMillisecond();
                    if (mCurrentEye2RoleDistance > mTargetEye2RoleDistance)
                    {
                        mCurrentEye2RoleDistance = mTargetEye2RoleDistance;
                        mNeedUpdateEyeDistance = false;
                    }
                }
                else if (mCurrentEye2RoleDistance > mTargetEye2RoleDistance)
                {
                    mCurrentEye2RoleDistance -= mEyeMoveSpeed * CCore.Engine.Instance.GetElapsedMillisecond();
                    if (mCurrentEye2RoleDistance < mTargetEye2RoleDistance)
                    {
                        mCurrentEye2RoleDistance = mTargetEye2RoleDistance;
                        mNeedUpdateEyeDistance = false;
                    }
                }
                else
                {
                    mNeedUpdateEyeDistance = false;
                }

                Eye2RoleDistance = (float)mCurrentEye2RoleDistance;
            }

            CCore.Engine.Instance.ScreenShake(CameraController);
        }

        public override CCore.MsgProc.FBehaviorProcess FindBehavior(CCore.MsgProc.BehaviorParameter bhInit)
        {
            if (Game.Instance.IsEditorMode)
                return null;
            switch (bhInit.GetBehaviorType())
            {
                case (int)CCore.MsgProc.BehaviorType.BHT_KB_Char_Down:
                    {
                        if (mCameraController == null)
                            return null;
                        return mKBDBehavior;
                    }

                case (int)CCore.MsgProc.BehaviorType.BHT_KB_Char_Up:
                    {
                        if (mCameraController == null)
                            return null;
                        return mKBUBehavior;
                    }

                case (int)CCore.MsgProc.BehaviorType.BHT_LB_Down://android 点击屏幕
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseLDBehavior;
                    }

                case (int)CCore.MsgProc.BehaviorType.BHT_LB_Up://android 点击屏幕
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseLUBehavior;
                    }
                case (int)CCore.MsgProc.BehaviorType.BHT_RB_Down:
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseRDBehavior;
                    }

                case (int)CCore.MsgProc.BehaviorType.BHT_RB_Up:
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseRUBehavior;
                    }
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer2Down:
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseLDBehavior;
                    }
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer3Down:
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseLDBehavior;
                    }
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer2Up:
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseLUBehavior;
                    }
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer3Up:
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseLUBehavior;
                    }

                case (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Move://android 拖动
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseMoveBehavior;
                    }

                case (int)CCore.MsgProc.BehaviorType.BHT_Mouse_Wheel:
                    {
                        if (mCameraController == null)
                            return null;
                        return mMouseWheelBehavior;
                    }
            }
            return null;
        }

        public int OnMouseRD(CCore.MsgProc.BehaviorParameter parameter)
        {
            var role = mHost as RoleActor;
            if (role == null)
                return 0;
            if (role.CurrentState.StateName == "Death")
                return 0;
            //    mIsMouseRD = true;
            if (CCore.Engine.Instance.Client.Graphics.IsDeviceLost)
                return 0;
            if (UISystem.Device.Mouse.Instance.Enable == false)
                return 0;
            if (mCameraController == null)
                return 0;

            var param = parameter as CCore.MsgProc.Behavior.Mouse_Key;            

            if (CCore.Engine.ChiefRoleMoveWithClick)
            {
                if (UISystem.Device.Mouse.Instance.IsHitOnWin())
                    return 0;

                mPreMouseMoveDelta.X = 0;
                mPreMouseMoveDelta.Y = 0;

                var start = mCameraController.Camera.GetLocation();
                var end = start + mCameraController.Camera.GetPickDirection(param.X, param.Y) * 1000f;

                var result = new CSUtility.Support.stHitResult();
                result.InitObject();
                result.mHitFlags |= (uint)CSUtility.enHitFlag.IgnoreMouseLineCheckInGame;
                if (CCore.Client.MainWorldInstance.LineCheck(ref start, ref end, ref result))
                {
                    ChiefRoleChange2Move(result.mHitPosition.X, result.mHitPosition.Z);
                    //TODO：播放点击特效

                    //                     List<SlimDX.Vector2> path = new List<SlimDX.Vector2>();
                    //                     CCore.Navigation.Navigation.Instance.FindPath(Stage.MainStage.Instance.CurMapId, role.Placement.GetLocation().X, role.Placement.GetLocation().Z,
                    //                                                                 result.mHitPosition.X, result.mHitPosition.Z, 5, ref path, true, true);
                    // 
                    //                     var state = role.AIStates.GetState("Walk");
                    //                     if (state != null)
                    //                     {
                    //                         var wp = state.Parameter as CSUtility.AISystem.States.IWalkParameter;
                    //                         if (wp != null)
                    //                         {
                    //                             if (path.Count > 0)
                    //                             {
                    //                                 wp.TargetPositions.Clear();
                    //                                 bool bClosestCheck = true;
                    //                                 foreach (var pathPoint in path)
                    //                                 {
                    //                                     SlimDX.Vector3 vStart = new SlimDX.Vector3(pathPoint.X, 1000.0F, pathPoint.Y);
                    //                                     SlimDX.Vector3 vEnd = new SlimDX.Vector3(pathPoint.X, -1000.0F, pathPoint.Y);
                    //                                     CSUtility.Support.stHitResult hitResult = new CSUtility.Support.stHitResult();
                    //                                     role.WorldLineCheck(ref vStart, ref vEnd, ref hitResult);
                    //                                     SlimDX.Vector3 vPos = new SlimDX.Vector3(pathPoint.X, hitResult.mHitPosition.Y, pathPoint.Y);
                    //                                     if (bClosestCheck && SlimDX.Vector3.Distance(vPos, role.Placement.GetLocation()) < 0.5F)
                    //                                         continue;
                    //                                     bClosestCheck = false;
                    //                                     wp.TargetPositions.Enqueue(vPos);
                    //                                 }
                    //                                 if (wp.TargetPositions.Count >= 1)
                    //                                     wp.TargetPosition = wp.TargetPositions.Dequeue();
                    //                                 else
                    //                                     wp.TargetPosition = role.Placement.GetLocation();
                    // 
                    //                                 wp.Run = (SByte)(role.Run ? 1 : 0);
                    //                                 wp.MoveSpeed = role.MoveSpeed;
                    //                                 wp.ActionPlayRate = role.SpeedRate;
                    // 
                    //                                 role.CurrentState.ToState("Walk", wp);
                    //                             }
                    //                             else if (CCore.Engine.Instance.DirectMoveWhenNotFindPath)
                    //                             {
                    //                                 wp.TargetPosition = result.mHitPosition;
                    //                                 wp.TargetPositions.Clear();
                    // 
                    //                                 wp.Run = (SByte)(role.Run ? 1 : 0);
                    //                                 wp.MoveSpeed = role.MoveSpeed;
                    //                                 wp.ActionPlayRate = role.SpeedRate;
                    // 
                    //                                 role.CurrentState.ToState("Walk", wp);
                    //                             }
                    //                         }
                    //                     }
                }
            }
            return 0;
        }

        public void  ChiefRoleChange2Move(float X,float Z)
        {
            var role = mHost as RoleActor;
            if (role == null)
                return;
            var state = role.AIStates.GetState("Walk");
            if (state != null)
            {
                var wp = state.Parameter as CSUtility.AISystem.States.IWalkParameter;
                if (wp != null)
                {
                    List<SlimDX.Vector2> path = new List<SlimDX.Vector2>();
                    CCore.Navigation.Navigation.Instance.FindPath(Stage.MainStage.Instance.CurMapId, role.Placement.GetLocation().X, role.Placement.GetLocation().Z,
                                                                X, Z, 5, ref path, true, true);
                    if (path.Count > 0)
                    {
                        wp.TargetPositions.Clear();
                        bool bClosestCheck = true;
                        foreach (var pathPoint in path)
                        {
                            SlimDX.Vector3 vStart = new SlimDX.Vector3(pathPoint.X, 1000.0F, pathPoint.Y);
                            SlimDX.Vector3 vEnd = new SlimDX.Vector3(pathPoint.X, -1000.0F, pathPoint.Y);
                            CSUtility.Support.stHitResult hitResult = new CSUtility.Support.stHitResult();
                            hitResult.InitObject();
                            role.WorldLineCheck(ref vStart, ref vEnd, ref hitResult);
                            SlimDX.Vector3 vPos = new SlimDX.Vector3(pathPoint.X, hitResult.mHitPosition.Y, pathPoint.Y);
                            if (bClosestCheck && SlimDX.Vector3.Distance(vPos, role.Placement.GetLocation()) < 0.5F)
                                continue;
                            bClosestCheck = false;
                            wp.TargetPositions.Enqueue(vPos);
                        }
                        if (wp.TargetPositions.Count >= 1)
                            wp.TargetPosition = wp.TargetPositions.Dequeue();
                        else
                            wp.TargetPosition = role.Placement.GetLocation();

                        wp.Run = (SByte)(role.Run ? 1 : 0);
                        wp.MoveSpeed = role.MoveSpeed;
                        wp.ActionPlayRate = role.SpeedRate;

                        role.CurrentState.ToState("Walk", wp);
                    }
                    else if (CCore.Engine.Instance.DirectMoveWhenNotFindPath)
                    {
                        float Y = role.GetAltitude(X, Z);
                        wp.TargetPosition = new SlimDX.Vector3(X, Y, Z);
                        wp.TargetPositions.Clear();

                        wp.Run = (SByte)(role.Run ? 1 : 0);
                        wp.MoveSpeed = role.MoveSpeed;
                        wp.ActionPlayRate = role.SpeedRate;

                        role.CurrentState.ToState("Walk", wp);
                    }
                }
            }
        }

        public int OnMouseLD(CCore.MsgProc.BehaviorParameter parameter)
        {
            if (CCore.Engine.Instance.Client.Graphics.IsDeviceLost)
                return 0;
            var param = parameter as CCore.MsgProc.Behavior.Mouse_Key;
            mOldMousePosX = param.X;
            Skill.SkillController.Instance.OnLBD(parameter);
            return 0;
        }

        SlimDX.Vector3 mPointOnMousePress = SlimDX.Vector3.Zero;
        public SlimDX.Vector3 PointOnMousePress
        {
            get { return mPointOnMousePress; }
        }

        SlimDX.Vector3 mCastPointOnMousePress = SlimDX.Vector3.Zero;
        public SlimDX.Vector3 CastPointOnMousePress
        {
            get { return mCastPointOnMousePress; }
        }
        System.Drawing.Point mOldMousePos = new System.Drawing.Point(0, 0);
        System.Drawing.Point mPreMouseMoveDelta = new System.Drawing.Point(0, 0);
        int mOldMousePosX = 0;
        public int OnMouseMove(CCore.MsgProc.BehaviorParameter parameter)
        {
            if (CCore.Engine.Instance.Client.Graphics.IsDeviceLost)
                return 0;
            if (mCameraController == null)
                return 0;

            var param = parameter as CCore.MsgProc.Behavior.Mouse_Move;

            if (param.button == CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left && !UISystem.Device.Mouse.Instance.IsHitOnWin())                    
            {
//                 mCameraController.Turn(CCore.CoordAxis.Y, (param.X - mOldMousePosX) * 0.01f);
//                 mOldMousePosX = param.X;
            }
            else
            {
                // 更新魔法施法提示的Decal位置
                mPointOnMousePress = IntersectWithScene(param.X, param.Y, (UInt32)CSUtility.enHitFlag.None);
                Role.RoleActor role = mHost as Role.RoleActor;
                if (role != null)
                {
                    mCastPointOnMousePress = IntersectWithZeroPlane(param.X, param.Y, new SlimDX.Vector3(0, role.Placement.GetLocation().Y + role.RoleTemplate.HalfHeight, 0), SlimDX.Vector3.UnitY);
                }
                if (Role.RoleActor.RegionActor.Visible == true)
                    Role.RoleActor.RegionActor.Placement.SetLocation(ref mPointOnMousePress);

                // 处理按住鼠标左键不停行走, 叠加鼠标移动偏移               
                if (param.button == CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Right)
                {
                    mPreMouseMoveDelta.X += param.X - mOldMousePos.X;
                    mPreMouseMoveDelta.Y += param.Y - mOldMousePos.Y;
                }
            }

            mOldMousePos.X = param.X;
            mOldMousePos.Y = param.Y;

            return 0;
        }

        float mFarEye2RoleDistance = 25;
        float mNearEye2RoleDistance = 5;
        public int OnMouseWheel(CCore.MsgProc.BehaviorParameter parameter)
        {
            if (CCore.Engine.Instance.Client.Graphics.IsDeviceLost)
                return 0;
            if (mCameraController == null)
                return 0;

            var param = parameter as CCore.MsgProc.Behavior.Mouse_Wheel;

            TargetEye2RoleDistance -= param.delta * 0.03;
            if (TargetEye2RoleDistance > mFarEye2RoleDistance)
                TargetEye2RoleDistance = mFarEye2RoleDistance;
            if (TargetEye2RoleDistance < mNearEye2RoleDistance)
                TargetEye2RoleDistance = mNearEye2RoleDistance;


            return 0;
        }

        public int OnKeyDown(CCore.MsgProc.BehaviorParameter parameter)
        {
            //if (parameter is CCore.MsgProc.Behavior.KB_Char)
            //{
            //    var param = parameter as CCore.MsgProc.Behavior.KB_Char;
            //    switch (param.Key)
            //    {                   
            //        //case CCore.MsgProc.BehaviorParameter.enKeys.W:
            //        //    {
            //        //        Move(mCameraController.Camera.GetZVector());
            //        //    }
            //        //    break;
            //        //case CCore.MsgProc.BehaviorParameter.enKeys.S:
            //        //    {
            //        //        Move(mCameraController.Camera.GetZVector() * -1);
            //        //    }
            //        //    break;
            //        //case CCore.MsgProc.BehaviorParameter.enKeys.A:
            //        //    {
            //        //        Move(mCameraController.Camera.GetXVector() * -1);
            //        //    }
            //        //    break;
            //        //case CCore.MsgProc.BehaviorParameter.enKeys.D:
            //        //    {
            //        //        Move(mCameraController.Camera.GetXVector());
            //        //    }
            //        //    break;
            //    }
            //}
            return 0;
        }

        SlimDX.Vector3 oldPos;
        public void Move(SlimDX.Vector3 dir)
        {            
            var role = Host as Role.RoleActor;
            if (role == null)
                return;
            if (role.CurrentState == null || role.CurrentState.StateName == "Death")
                return;            
            dir.Y = 0;
            dir.Normalize();
            var rolePos = role.Placement.GetLocation();
            var targetPos =rolePos + dir * 45f;
            var curStateParam = role.CurrentState.Parameter as CSUtility.AISystem.States.IWalkParameter;
            if(curStateParam !=null)
            {
                var distance = oldPos - targetPos;
                if (distance.Length() < 6)
                    return;
            }
            oldPos =targetPos;
       //     Log.FileLog.WriteLine("chiefRoleMove tarpos:{0},oldpos:{1},time:{2}", targetPos,oldPos, CSUtility.Helper.LogicTimer.GetTickCount());
          //  ChiefRoleChange2Move(targetPos.X,targetPos.Z);

            role.RoleFireDir = dir;
            if (role.CurrentState != null)
            {
                CSUtility.AISystem.States.IWalkParameter wp = role.AIStates.GetState("Walk").Parameter as CSUtility.AISystem.States.IWalkParameter;

                dir.Normalize();
                wp.Run = (SByte)(role.Run ? 1 : 0);
                wp.MoveSpeed = role.MoveSpeed;
                wp.TargetPosition = targetPos;
                wp.TargetPositions.Clear();
                if (role.CurrentState.StateName == "Idle")
                {
                    role.CurrentState.CanInterrupt = true;
                }
                
                wp.ActionPlayRate = 1;
             //   if (role.CurrentState.StateName != "Walk")
                    role.CurrentState.ToState("Walk", wp);

                role.Placement.SetRotationY(dir.Z, dir.X, role.RoleTemplate.MeshFixAngle);
            }
        }
        public int OnMouseLU(CCore.MsgProc.BehaviorParameter parameter)
        {
            return 0;
        }

      //  bool mIsMouseRD = false;
        public int OnMouseRU(CCore.MsgProc.BehaviorParameter parameter)
        {
    //        mIsMouseRD = false;
            return 0;
        }

        public int OnKeyUp(CCore.MsgProc.BehaviorParameter parameter)
        {
            if (parameter is CCore.MsgProc.Behavior.KB_Char)
            {
                var param = parameter as CCore.MsgProc.Behavior.KB_Char;
                switch (param.Key)
                {
                    case CCore.MsgProc.BehaviorParameter.enKeys.W:
                    case CCore.MsgProc.BehaviorParameter.enKeys.S:
                    case CCore.MsgProc.BehaviorParameter.enKeys.A:
                    case CCore.MsgProc.BehaviorParameter.enKeys.D:
                        {
//                             var role = mHost as RoleActor;
//                             if (role != null)
//                             {
//                                 role.CurrentState.ToState("Idle", null);                                
//                             }
                        }
                        break;                    
                }
            }
            return 0;
        }

        void MouseWalk(Role.RoleActor role, int x, int y)
        {
            if (role.CurrentState.StateName != "Walk")
                return;

            if (CCore.Engine.ChiefRoleMoveWithClick)
            {
                if (UISystem.Device.Mouse.Instance.IsHitOnWin())
                    return;

                mPreMouseMoveDelta.X = 0;
                mPreMouseMoveDelta.Y = 0;

                var start = mCameraController.Camera.GetLocation();
                var end = start + mCameraController.Camera.GetPickDirection(x, y) * 1000f;

                var result = new CSUtility.Support.stHitResult();
                result.InitObject();
                result.mHitFlags |= (uint)CSUtility.enHitFlag.IgnoreMouseLineCheckInGame;
                if (CCore.Client.MainWorldInstance.LineCheck(ref start, ref end, ref result))
                {
                    //TODO：播放点击特效

                    List<SlimDX.Vector2> path = new List<SlimDX.Vector2>();
                    CCore.Navigation.Navigation.Instance.FindPath(Guid.Empty, role.Placement.GetLocation().X, role.Placement.GetLocation().Z,
                                                                result.mHitPosition.X, result.mHitPosition.Z, 5, ref path, true, true);

                    var state = role.AIStates.GetState("Walk");
                    if (state != null)
                    {
                        var wp = state.Parameter as CSUtility.AISystem.States.IWalkParameter;
                        if (wp != null)
                        {
                            if (path.Count > 0)
                            {
                                wp.TargetPositions.Clear();
                                bool bClosestCheck = true;
                                foreach (var pathPoint in path)
                                {
                                    SlimDX.Vector3 vStart = new SlimDX.Vector3(pathPoint.X, 1000.0F, pathPoint.Y);
                                    SlimDX.Vector3 vEnd = new SlimDX.Vector3(pathPoint.X, -1000.0F, pathPoint.Y);
                                    CSUtility.Support.stHitResult hitResult = new CSUtility.Support.stHitResult();
                                    hitResult.InitObject();
                                    role.WorldLineCheck(ref vStart, ref vEnd, ref hitResult);
                                    SlimDX.Vector3 vPos = new SlimDX.Vector3(pathPoint.X, hitResult.mHitPosition.Y, pathPoint.Y);
                                    if (bClosestCheck && SlimDX.Vector3.Distance(vPos, role.Placement.GetLocation()) < 0.5F)
                                        continue;
                                    bClosestCheck = false;
                                    wp.TargetPositions.Enqueue(vPos);
                                }
                                if (wp.TargetPositions.Count >= 1)
                                    wp.TargetPosition = wp.TargetPositions.Dequeue();
                                else
                                    wp.TargetPosition = role.Placement.GetLocation();

                                wp.Run = (SByte)(role.Run ? 1 : 0);
                                wp.MoveSpeed = role.MoveSpeed;
                                wp.ActionPlayRate = role.SpeedRate;

                                role.CurrentState.ToState("Walk", wp);
                            }
                            else //if (CCore.Engine.Instance.DirectMoveWhenNotFindPath)
                            {
                                wp.TargetPosition = result.mHitPosition;
                                wp.TargetPositions.Clear();

                                wp.Run = (SByte)(role.Run ? 1 : 0);
                                wp.MoveSpeed = role.MoveSpeed;
                                wp.ActionPlayRate = role.SpeedRate;

                                role.CurrentState.ToState("Walk", wp);
                            }
                        }
                    }
                }
            }

        }

        SlimDX.Vector3 IntersectWithZeroPlane(int x, int y, SlimDX.Vector3 planePoint, SlimDX.Vector3 planeNormal)
        {
            SlimDX.Vector3 start = mCameraController.Camera.GetLocation();
            SlimDX.Vector3 dir = mCameraController.Camera.GetPickDirection(x, y);

            var t = (SlimDX.Vector3.Dot(planeNormal, planePoint) - SlimDX.Vector3.Dot(planeNormal, start)) / SlimDX.Vector3.Dot(planeNormal, dir);
            return start + dir * t;
        }

        public SlimDX.Vector3 mPickDir;
        SlimDX.Vector3 IntersectWithScene(int x, int y, UInt32 hitFlag)
        {
            SlimDX.Vector3 start = mCameraController.Camera.GetLocation();
            mPickDir = mCameraController.Camera.GetPickDirection(x, y);
            SlimDX.Vector3 end = start + mCameraController.Camera.GetPickDirection(x, y) * 1000.0F;

            var result = new CSUtility.Support.stHitResult();
            result.InitObject();
            result.mHitFlags |= hitFlag;// (uint)CSCommon.enHitFlag.HitMeshTriangle;
            if (CCore.Client.MainWorldInstance.LineCheck(ref start, ref end, ref result))
                return result.mHitPosition;

            return IntersectWithZeroPlane(x, y, SlimDX.Vector3.Zero, SlimDX.Vector3.UnitY);
        }
    }
}
