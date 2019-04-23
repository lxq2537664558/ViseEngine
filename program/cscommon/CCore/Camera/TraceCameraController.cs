namespace CCore.Camera
{
    /// <summary>
    /// 轨道相机控制器
    /// </summary>
    public class TraceCameraController : CameraController
    {
        /// <summary>
        /// 摄像目标
        /// </summary>
        protected CCore.World.Actor mTarget;
        /// <summary>
        /// 摄像的方向
        /// </summary>
        protected SlimDX.Vector3 mDirection;
        /// <summary>
        /// 摄像的距离
        /// </summary>
        protected float mDistance;
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="actor">进行摄像的Actor</param>
        public TraceCameraController(CCore.World.Actor actor)
        {
            mTarget = actor;

            mDirection.X = 0;
            mDirection.Y = -0.5f;
            mDirection.Z = 0.5f;
            mDirection.Normalize();

            mDistance = 20.0f;
        }
        /// <summary>
        /// 轨道相机的初始化
        /// </summary>
        /// <param name="dir">摄像的方向</param>
        /// <param name="dist">摄像距离</param>
        public void Initialize(ref SlimDX.Vector3 dir, float dist)
        {
            mDirection = dir;
            mDistance = dist;
        }
        /// <summary>
        /// 摄像机的移动
        /// </summary>
        /// <param name="ax">移动轴</param>
        /// <param name="step">移动步数</param>
        /// <returns>返回移动的步数</returns>
        public override float Move(CoordAxis ax, float step)
        {
            switch(ax)
		    {
		    case CoordAxis.Z:
			    {
				    mDistance += step;
				    if(mDistance<2.0F)
					    mDistance = 2.0F;
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
                case CoordAxis.Y:
                    {
                        SlimDX.Matrix mat = SlimDX.Matrix.RotationY(step);
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

        SlimDX.Vector3 mCurrentLookAt = SlimDX.Vector3.Zero;
        float mMoveSpeed = 0.003f;
        /// <summary>
        /// 每帧调用
        /// </summary>
        public override void Tick()
        {
		    if(!Enable)
			    return;

            if (CCore.Engine.Instance.GetElapsedMillisecond() > 500)
                return;

            SlimDX.Vector3 camHeight = SlimDX.Vector3.Zero;
            var cylinder = mTarget.Shape as CSUtility.Component.IShapeCylinder;
            if(cylinder!=null)
            {
                camHeight.Y = cylinder.CamHeight;
            }
            var tagLookAt = mTarget.Placement.GetLocation() + camHeight;

            var delta = tagLookAt - mCurrentLookAt;
            var dir = SlimDX.Vector3.Normalize(delta);
            var length = delta.Length();
            if (length > 10)
            {
                mCurrentLookAt = tagLookAt;
            }
            else if (length > 1)
            {
                var moveDist = 0.005f * CCore.Engine.Instance.GetElapsedMillisecond();
                if( moveDist > length )
                    moveDist = length;
                mCurrentLookAt += delta * moveDist;

                //mCurrentLookAt += delta * 0.005f * MidLayer.IEngine.Instance.GetElapsedMillisecond();
            }
            else
            {
                var moveDist = mMoveSpeed * CCore.Engine.Instance.GetElapsedMillisecond();
                if (moveDist >= length)
                {
                    //moveDist = length;
                    mCurrentLookAt = tagLookAt;
                }
                else
                    mCurrentLookAt += dir * moveDist;

                ////mCurrentLookAt += dir * moveDist;
                ////var speed = mMoveSpeed * MidLayer.IEngine.Instance.GetElapsedMillisecond();

                //if(mCurrentLookAt.X > tagLookAt.X)
                //    mCurrentLookAt.X -= moveDist;
                //else if(mCurrentLookAt.X < tagLookAt.X)
                //    mCurrentLookAt.X += moveDist;
                //if (System.Math.Abs(tagLookAt.X - mCurrentLookAt.X) < moveDist)
                //    mCurrentLookAt.X = tagLookAt.X;

                //if(mCurrentLookAt.Y > tagLookAt.Y)
                //    mCurrentLookAt.Y -= moveDist;
                //else if(mCurrentLookAt.Y < tagLookAt.Y)
                //    mCurrentLookAt.Y += moveDist;
                //if (System.Math.Abs(tagLookAt.Y - mCurrentLookAt.Y) < moveDist)
                //    mCurrentLookAt.Y = tagLookAt.Y;

                //if (mCurrentLookAt.Z > tagLookAt.Z)
                //    mCurrentLookAt.Z -= moveDist;
                //else if (mCurrentLookAt.Z < tagLookAt.Z)
                //    mCurrentLookAt.Z += moveDist;
                //if (System.Math.Abs(tagLookAt.Z - mCurrentLookAt.Z) < moveDist)
                //    mCurrentLookAt.Z = tagLookAt.Z;
            }
            //if (System.Math.Abs(mCurrentLookAt.X - tagLookAt.X) < 0.0001f)
            //    mCurrentLookAt.X = tagLookAt.X;
            //if (System.Math.Abs(mCurrentLookAt.Y - tagLookAt.Y) < 0.0001f)
            //    mCurrentLookAt.Y = tagLookAt.Y;
            //if (System.Math.Abs(mCurrentLookAt.Z - tagLookAt.Z) < 0.0001f)
            //    mCurrentLookAt.Z = tagLookAt.Z;

            var eyeAt = mCurrentLookAt - mDirection * mDistance;

            SetPosLookAtUp(ref eyeAt, ref mCurrentLookAt, ref SlimDX.Vector3.UnitY);
        }
    }
}
