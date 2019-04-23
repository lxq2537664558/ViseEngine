namespace CCore.Camera
{
    /// <summary>
    /// maya相机控制器
    /// </summary>
    public class MayaPosCameraController : CameraController
    {
        /// <summary>
        /// 相机的方向
        /// </summary>
        protected SlimDX.Vector3 mDirection;
        /// <summary>
        /// 摄像距离
        /// </summary>
        protected float mDistance;
        /// <summary>
        /// 目标的位置坐标
        /// </summary>
        protected SlimDX.Vector3 mTargetPos;
        /// <summary>
        /// maya相机控制器的构造函数
        /// </summary>
        public MayaPosCameraController()
        {
            mTargetPos.X = 0.0f;
            mTargetPos.Y = 0.0f;
            mTargetPos.Z = 0.0f;
            mDirection.X = 0;
            mDirection.Y = -0.5f;
            mDirection.Z = 0.5f;
            mDirection.Normalize();

            mDistance = 20.0f;
        }
        /// <summary>
        /// 相机初始化
        /// </summary>
        /// <param name="dir">相机的方向</param>
        /// <param name="dist">相机的摄像距离</param>
        public void Initialize(ref SlimDX.Vector3 dir, float dist)
        {
		    mDirection = dir;
		    mDistance = dist;
        }
        /// <summary>
        /// 相机移动
        /// </summary>
        /// <param name="ax">移动轴</param>
        /// <param name="step">移动的步数</param>
        /// <returns>返回移动的步数</returns>
        public override float Move(CoordAxis ax, float step)
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
                        float fNewLength = DllImportAPI.v3dCamera_GetViewLength(mCamera.CameraPtr) - step;
                        if (fNewLength > 0)
                            DllImportAPI.v3dCamera_SetPosByLength(mCamera.CameraPtr, fNewLength);
                    }
                    break;
            }
            return step;
        }
        /// <summary>
        /// 相机的旋转
        /// </summary>
        /// <param name="ax">旋转轴</param>
        /// <param name="step">旋转步数</param>
        /// <returns>返回旋转步数</returns>
        public override float Turn(CoordAxis ax, float step)
        {
            if (mCamera == null)
                return 0;

            switch (ax)
            {
                case CoordAxis.X:
                    {
                        DllImportAPI.v3dCamera_RotateLookAtByCamera(mCamera.CameraPtr, (int)(V3D_AXIS_TYPE.V3D_AXIS_X), step, true);
                    }
                    break;

                case CoordAxis.Y:
                    {
                        DllImportAPI.v3dCamera_RotateLookAtByAxis(mCamera.CameraPtr, (int)(V3D_AXIS_TYPE.V3D_AXIS_Y), step, true);
                    }
                    break;
            }

            return step;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        public override void Tick() { }
        /// <summary>
        /// 设置相机的lookAt向量
        /// </summary>
        /// <param name="lookAt">lookAt向量</param>
        /// <param name="fLength">相机的长度</param>
        public override void SetLookAt(ref SlimDX.Vector3 lookAt, float fLength)
        {
            unsafe
            {
                SlimDX.Vector3 dirVec, upVec;
                DllImportAPI.v3dCamera_GetDirVec(mCamera.CameraPtr, &dirVec);
                var eyePt = lookAt - dirVec * fLength;
                var tempLookAt = lookAt;
                DllImportAPI.v3dCamera_GetUpVec(mCamera.CameraPtr, &upVec);
                DllImportAPI.v3dCamera_SetPosLookAtUp(mCamera.CameraPtr, &eyePt, &tempLookAt, &upVec);
            }
        }
    }
}
