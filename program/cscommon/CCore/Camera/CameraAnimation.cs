using System;

namespace CCore.Camera
{
    /// <summary>
    /// 摄像机录像
    /// </summary>
    public class CameraAnimation
    {
        CSUtility.Map.ScenePointGroup_Camera mCameraPath;
        /// <summary>
        /// 只读属性，摄像机录像的路径
        /// </summary>
        public CSUtility.Map.ScenePointGroup_Camera CameraPath
        {
            get { return mCameraPath; }
        }
        /// <summary>
        /// 只读属性，摄像机路径的ID
        /// </summary>
        public Guid Id
        {
            get
            {
                if (CameraPath == null)
                    return Guid.Empty;

                return CameraPath.Id;
            }
        }
        /// <summary>
        /// 只读属性，持续时间
        /// </summary>
        public Int64 Duration
        {
            get
            {
                if (CameraPath == null)
                    return 0;

                if (CameraPath.Points.Count <= 0)
                    return 0;

                var cameraPt = CameraPath.Points[CameraPath.Points.Count - 1] as CSUtility.Map.ScenePoint_Camera;
                if (cameraPt == null)
                    return 0;

                return cameraPt.KeyFrameMilliTimeEnd;
            }
        }

        float mPlayRate = 1.0f;
        /// <summary>
        /// 播放速度
        /// </summary>
        public float PlayRate
        {
            get { return mPlayRate; }
            set
            {
                mPlayRate = value;
            }
        }

        Int64 mCurAnimTime = 0;
        /// <summary>
        /// 当前动画世界
        /// </summary>
        public Int64 CurAnimTime
        {
            get { return mCurAnimTime; }
            set
            {
                mCurAnimTime = value;
            }
        }

        bool mBoolCallBackEvent = true;
        /// <summary>
        /// 是否重新播放
        /// </summary>
        public bool BoolCallBackEvent
        {
            get { return mBoolCallBackEvent; }
            set { mBoolCallBackEvent = value; }
        }
        /// <summary>
        /// 摄像机动作
        /// </summary>
        /// <param name="spCam">摄像机地址</param>
        public CameraAnimation(CSUtility.Map.ScenePointGroup_Camera spCam)
        {
            mCameraPath = spCam;
        }
        /// <summary>
        /// 复位，将当前播放时间重置为0
        /// </summary>
        public void Reset()
        {
            CurAnimTime = 0;
        }
        /// <summary>
        /// 是否播放完成
        /// </summary>
        /// <returns>播放完成返回true，否则返回false</returns>
        public bool IsFinished()
        {
            if (CurAnimTime >= Duration)
            {
                UISystem.Device.Mouse.Instance.Enable = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecondTime">每帧之间的间隔时间</param>
        public void Tick(Int64 elapsedMillisecondTime)
        {
            if (mCameraPath == null)
                return;

            CurAnimTime += elapsedMillisecondTime;

            if (mCameraPath.Loop)
            {
                if (CurAnimTime >= Duration)
                    CurAnimTime = 0;
            }
            else
            {
                if (CurAnimTime >= Duration)
                    CurAnimTime = Duration;
            }
        }
        /// <summary>
        /// 当前摄像机的位置
        /// </summary>
        /// <returns>返回当前摄像机的位置坐标</returns>
        public SlimDX.Vector3 GetCurrentCameraPosition()
        {
            if(mCameraPath == null)
                return SlimDX.Vector3.Zero;

            return mCameraPath.GetPosition(CurAnimTime * 1.0f / Duration);
        }
        /// <summary>
        /// 当前摄像机的方向
        /// </summary>
        /// <returns>返回当前摄像机的方向</returns>
        public SlimDX.Vector3 GetCurrentCameraDirection()
        {
            if (mCameraPath == null)
                return SlimDX.Vector3.UnitZ;

            var rot = mCameraPath.GetRotation(CurAnimTime * 1.0f / Duration);
            return SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.UnitZ, rot);
        }
    }
}
