using System;
using System.Collections.Generic;

namespace CCore.Camera
{
    /// <summary>
    /// 摄像机动作管理类
    /// </summary>
    public class CameraAnimationManager
    {
        static CameraAnimationManager smInstance = new CameraAnimationManager();
        /// <summary>
        /// 声明该类为单类
        /// </summary>
        public static CameraAnimationManager Instance
        {
            get { return smInstance; }
        }
        /// <summary>
        /// 释放该实例，程序结束时调用
        /// </summary>
        public static void FinalInstance()
        {
            smInstance = null;
        }

        CameraController mCamAnimCameraController = new MayaPosCameraController();
        CameraController mOldCamAnimCameraControllerStore = null;
        Dictionary<Guid, Dictionary<Guid, CameraAnimation>> mCameraAnimDic = new Dictionary<Guid, Dictionary<Guid, CameraAnimation>>();
        CameraAnimation mCurrentAnimation = null;
        /// <summary>
        /// 设置摄像机的视野
        /// </summary>
        /// <param name="eye">视野</param>
        public void SetCamera(CameraObject eye)
        {
            mCamAnimCameraController.Camera = eye;
        }
        /// <summary>
        /// 播放摄像机动画
        /// </summary>
        /// <param name="mapId">地图的ID</param>
        /// <param name="animId">播放动画的ID</param>
        public void PlayCameraAnimation(Guid mapId, Guid animId)
        {
            Dictionary<Guid, CameraAnimation> camDic;
            if (!mCameraAnimDic.TryGetValue(mapId, out camDic))
            {
                camDic = new Dictionary<Guid, CameraAnimation>();
                mCameraAnimDic[mapId] = camDic;
            }

            CameraAnimation camAnim;
            if (!camDic.TryGetValue(animId, out camAnim))
            {
                var group = CSUtility.Map.ScenePointGroupManager.Instance.FindGroup(mapId, animId) as CSUtility.Map.ScenePointGroup_Camera;
                if (group == null)
                    return;
                camAnim = new CameraAnimation(group);
                camDic[animId] = camAnim;
            }

            if (mCurrentAnimation != null)
            {
                mCurrentAnimation.Reset();
            }
            mCurrentAnimation = camAnim;
            UISystem.Device.Mouse.Instance.Enable = false;//鼠标事件屏蔽

            if(CameraController.CurrentCameraController != mCamAnimCameraController)
                mOldCamAnimCameraControllerStore = CameraController.CurrentCameraController;
            CameraController.CurrentCameraController = mCamAnimCameraController;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecondTime">每帧之间的间隔时间</param>
        public void Tick(Int64 elapsedMillisecondTime)
        {
            if (mCurrentAnimation != null)
            {
                if (!mCurrentAnimation.IsFinished())
                {
                    mCurrentAnimation.Tick(elapsedMillisecondTime);

                    var camPos = mCurrentAnimation.GetCurrentCameraPosition();
                    var camRot = mCurrentAnimation.GetCurrentCameraDirection();

                    mCamAnimCameraController.SetPosDir(ref camPos, ref camRot);

                    if (mCurrentAnimation.CameraPath.OnCameraTickCB != null && mCurrentAnimation.BoolCallBackEvent)
                    {
                        var fun = mCurrentAnimation.CameraPath.OnCameraTickCB.GetCallee() as CSUtility.Map.FOnCameraTick;
                        if (fun != null && mCurrentAnimation.Duration !=0)
                        {
                            float percent =(float)mCurrentAnimation.CurAnimTime /(float)mCurrentAnimation.Duration;
                            var stateHost = CCore.Client.ChiefRoleInstance as CSUtility.AISystem.IStateHost;
                            if (stateHost != null)
                            {
                                mCurrentAnimation.BoolCallBackEvent =fun(stateHost, percent);
                            }
                        }
                    }
                }
                else
                {
                    if (mCurrentAnimation.CameraPath.OnCameraFinishCB != null)
                    {
                  //      System.Diagnostics.Debug.WriteLine(string.Format("EndCameraAnimation time== {0}:{1}", MidLayer.IEngine.Instance._GetTickCount(), MidLayer.IEngine.Instance.GetFrameMillisecond()));
                        var fun = mCurrentAnimation.CameraPath.OnCameraFinishCB.GetCallee() as CSUtility.Map.FOnCameraFinish;
                        if (fun != null)
                        {
                            var stateHost = CCore.Client.ChiefRoleInstance as CSUtility.AISystem.IStateHost;
                            if (stateHost != null)
                            {
                                fun(stateHost);
                            }
                        }
                    }

                    mCurrentAnimation.Reset();
                    mCurrentAnimation = null;
                    CameraController.CurrentCameraController = mOldCamAnimCameraControllerStore;
                }
            }
        }
    }
}
