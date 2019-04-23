namespace CCore.Camera
{
    /// <summary>
    /// 摄像机控制类
    /// </summary>
    public class CameraController
    {
        static CameraController mCurrentCameraController = null;
        /// <summary>
        /// 摄像机控制器
        /// </summary>
        public static CameraController CurrentCameraController
        {
            get { return mCurrentCameraController; }
            set
            {
                if (mCurrentCameraController != null)
                    mCurrentCameraController.Enable = false;

                mCurrentCameraController = value;
                if (mCurrentCameraController != null)
                    mCurrentCameraController.Enable = true;
            }
        }
        /// <summary>
        /// 摄像机
        /// </summary>
        protected CameraObject mCamera;
        /// <summary>
        /// 当前控制的摄像机
        /// </summary>
        public CameraObject Camera
        {
            get { return mCamera; }
            set { mCamera = value; }
        }
        /// <summary>
        /// 是否进行控制
        /// </summary>
        protected bool mEnable;
        /// <summary>
        /// 是否对摄像机进行控制
        /// </summary>
        public bool Enable
        {
            get { return mEnable; }
            set { mEnable = value; }
        }
        /// <summary>
        /// 摄像机控制器的构造函数
        /// </summary>
        public CameraController()
        {

        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~CameraController()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除该实例
        /// </summary>
        public void Cleanup()
        {

        }
        /// <summary>
        /// 设置摄像机的位置等参数
        /// </summary>
        /// <param name="pos">摄像机的位置</param>
        /// <param name="lookAt">摄像机的lookAt向量</param>
        /// <param name="up">摄像机的UP向量</param>
        public void SetPosLookAtUp( ref SlimDX.Vector3 pos , ref SlimDX.Vector3 lookAt, ref SlimDX.Vector3 up )
        {
            unsafe
            {
                if (mCamera == null)
                    return;

                var tempPos = pos;
                var tempLookAt = lookAt;
                var tempUp = up;
                CCore.DllImportAPI.v3dCamera_SetPosLookAtUp(mCamera.CameraPtr, &tempPos, &tempLookAt, &tempUp);

                //MidLayer.Audio.AudioManager.Instance.Listener.Position = tempPos;
                //MidLayer.Audio.AudioManager.Instance.Listener.mDirection = SlimDX.Vector3.Normalize(tempLookAt - tempPos);
                //MidLayer.Audio.AudioManager.Instance.Listener.Up = tempUp;
            }
        }
        /// <summary>
        /// 设置摄像机在当前位置的朝向
        /// </summary>
        /// <param name="pos">当前位置</param>
        /// <param name="dir">摄像方向的向量</param>
		public void SetPosDir( ref SlimDX.Vector3 pos , ref SlimDX.Vector3 dir )
        {
            unsafe
            {
                var tempPos = pos;
                var tempDir = dir;
                CCore.DllImportAPI.v3dCamera_SetPosDir(mCamera.CameraPtr, &tempPos, &tempDir);

                //MidLayer.Audio.AudioManager.Instance.Listener.Position = tempPos;
                //MidLayer.Audio.AudioManager.Instance.Listener.mDirection = tempDir;
            }
        }
        /// <summary>
        /// 设置摄像机的lookAt向量
        /// </summary>
        /// <param name="lookAt">摄像机的lookAt向量</param>
        /// <param name="fLength">摄像长度</param>
		public virtual void SetLookAt(ref SlimDX.Vector3 lookAt, float fLength)
        {

        }
        /// <summary>
        /// 摄像机移动
        /// </summary>
        /// <param name="ax">摄像机沿哪条轴移动</param>
        /// <param name="step">移动步数</param>
        /// <returns>在移动轴向的移动步数</returns>
        public virtual float Move(CoordAxis ax, float step)
        {
            unsafe
            {
                if (mCamera == null)
                    return 0;

                switch (ax)
                {
                    case CoordAxis.X:
                        {
                            DllImportAPI.v3dCamera_MoveByCamera(mCamera.CameraPtr, (int)(V3D_AXIS_TYPE.V3D_AXIS_X), step, true);
                        }
                        break;

                    case CoordAxis.Y:
                        {
                            DllImportAPI.v3dCamera_MoveByCamera(mCamera.CameraPtr, (int)(V3D_AXIS_TYPE.V3D_AXIS_Y), step, true);
                        }
                        break;

                    case CoordAxis.Z:
                        {
                            DllImportAPI.v3dCamera_MoveByCamera(mCamera.CameraPtr, (int)(V3D_AXIS_TYPE.V3D_AXIS_Z), step, true);
                        }
                        break;
                }

                return step;
            }
        }
        /// <summary>
        /// 摄像机的移动
        /// </summary>
        /// <param name="axis">移动的向量</param>
		public virtual void Move(SlimDX.Vector3 axis)
        {
            if (mCamera == null)
                return;

            unsafe
            {
                DllImportAPI.v3dCamera_MoveByAxis(mCamera.CameraPtr, &axis, true);
            }
        }
        /// <summary>
        /// 摄像机的沿轴旋转
        /// </summary>
        /// <param name="ax">旋转轴</param>
        /// <param name="step">旋转的步数</param>
        /// <returns>返回旋转的步数</returns>
		public virtual float Turn(CoordAxis ax,float step)
        {
            unsafe
            {
                if (mCamera == null)
                    return 0;

                SlimDX.Vector3 pos = new SlimDX.Vector3();
                SlimDX.Vector3 dir = new SlimDX.Vector3();
                DllImportAPI.v3dCamera_GetPosition(mCamera.CameraPtr, &pos);
                DllImportAPI.v3dCamera_GetDirVec(mCamera.CameraPtr, &dir);
                var up = SlimDX.Vector3.UnitY;

                switch (ax)
                {
                    case CoordAxis.X:
                        {
                            DllImportAPI.v3dCamera_RotateCameraAtByCamera(mCamera.CameraPtr, (int)V3D_AXIS_TYPE.V3D_AXIS_X, step, true);
                            DllImportAPI.v3dCamera_SetPosDirUp(mCamera.CameraPtr, &pos, &dir, &up);
                        }
                        break;

                    case CoordAxis.Y:
                        {
                            DllImportAPI.v3dCamera_RotateCameraAtByCamera(mCamera.CameraPtr, (int)V3D_AXIS_TYPE.V3D_AXIS_Y, step, true);
                            DllImportAPI.v3dCamera_SetPosDirUp(mCamera.CameraPtr, &pos, &dir, &up);
                        }
                        break;

                    case CoordAxis.Z:
                        {
                            DllImportAPI.v3dCamera_RotateCameraAtByCamera(mCamera.CameraPtr, (int)V3D_AXIS_TYPE.V3D_AXIS_Z, step, true);
                            DllImportAPI.v3dCamera_SetPosDirUp(mCamera.CameraPtr, &pos, &dir, &up);
                        }
                        break;
                }

                return step;
            }
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
		public virtual void Tick(){}
    }
}
