namespace CCore.Camera
{
    /// <summary>
    /// 轨道相机控制器
    /// </summary>
    public class TracePosCameraController : CameraController
    {
        /// <summary>
        /// 相机方向
        /// </summary>
        protected SlimDX.Vector3 mDirection;
        /// <summary>
        /// 相机距离
        /// </summary>
        protected float mDistance;
        /// <summary>
        /// 摄像目标的位置坐标
        /// </summary>
        protected SlimDX.Vector3 mTargetPos;
        /// <summary>
        /// 轨道相机控制器的构造函数
        /// </summary>
        public TracePosCameraController()
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
        /// 相机的初始化
        /// </summary>
        /// <param name="dir">相机的摄影方向</param>
        /// <param name="dist">摄影距离</param>
        public void Initialize(ref SlimDX.Vector3 dir, float dist)
        {
		    mDirection = dir;
		    mDistance = dist;
        }
        /// <summary>
        /// 相机移动
        /// </summary>
        /// <param name="ax">移动轴</param>
        /// <param name="step">移动步数</param>
        /// <returns>返回移动步数</returns>
        public override float Move(CoordAxis ax, float step)
        {
		    switch(ax)
		    {
		    case CoordAxis.X:
			    {
				    mTargetPos += mCamera.GetXVector() * step;
			    }
			    break;
		    case CoordAxis.Y:
			    {
				    mTargetPos += mCamera.GetYVector() * step;
			    }
			    break;
		    case CoordAxis.Z:
			    {
				    mDistance += step;
				    if(mDistance<0.01F)
					    mDistance = 0.01F;
			    }
			    break;
		    }
		    return step;
        }
        /// <summary>
        /// 相机的旋转
        /// </summary>
        /// <param name="ax">旋转轴</param>
        /// <param name="step">旋转的步数</param>
        /// <returns>返回旋转的步数</returns>
        public override float Turn(CoordAxis ax, float step)
        {
            if (mCamera == null)
                return 0;

            switch (ax)
            {
                case CoordAxis.Y:
                    {
                        SlimDX.Matrix mat;
                        mat = SlimDX.Matrix.RotationY(step);
                        mDirection = SlimDX.Vector3.TransformNormal(mDirection, mat);
                    }
                    break;

                case CoordAxis.X:
                    {
                        SlimDX.Matrix mat;
                        SlimDX.Vector3 rotAxis;
                        rotAxis = SlimDX.Vector3.Cross(SlimDX.Vector3.UnitY, mDirection);
                        mat = SlimDX.Matrix.RotationAxis(rotAxis, step);
                        mDirection = SlimDX.Vector3.TransformNormal(mDirection, mat);
                    }
                    break;
            }

            mDirection.Normalize();
            return step;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        public override void Tick()
        {
		    if(!Enable)
			    return;

		    SlimDX.Vector3 lookAt = mTargetPos;
		    SlimDX.Vector3 eyeAt = lookAt - mDirection*mDistance;

		    SetPosLookAtUp( ref eyeAt , ref lookAt , ref SlimDX.Vector3.UnitY );
        }
        /// <summary>
        /// 设置摄像目标的位置
        /// </summary>
        /// <param name="pos">目标位置坐标</param>
        public void SetTargetPos(ref SlimDX.Vector3 pos)
        {
            mTargetPos = pos;
        }
        /// <summary>
        /// 设置摄像距离
        /// </summary>
        /// <param name="distance">摄像机的距离</param>
        public void SetDistance(float distance)
        {
            mDistance = distance;
        }
    }
}
