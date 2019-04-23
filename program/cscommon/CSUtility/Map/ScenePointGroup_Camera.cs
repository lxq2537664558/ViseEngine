using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CSUtility.Map
{
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Camera")]
    public delegate bool FOnCameraTick(AISystem.IStateHost host,float percent);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Camera")]
    public delegate bool FOnCameraFinish(AISystem.IStateHost host);

    public class ScenePointGroup_Camera : ScenePointGroup, CSUtility.Animation.TimeLineObjectInterface
    {
#region ForEditor

        public delegate SlimDX.Matrix Delegate_GetCameraTrans();
        public Delegate_GetCameraTrans OnGetCameraTrans;

        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public string TimeLineObjectName
        {
            get { return Name; }
            set
            {
                if(Name != value)
                    Name = value;

                OnPropertyChanged("TimeLineObjectName");
            }
        }

        public CSUtility.Animation.TimeLineKeyFrameObjectInterface AddKeyFrameObject(Int64 startTime, Int64 endTime, string name)
        {
            return AddCameraPoint(startTime, name);
        }
        public void RemoveKeyFrameObject(CSUtility.Animation.TimeLineKeyFrameObjectInterface frame)
        {
            var np = frame as ScenePoint;
            if (np != null)
                DeleteScenePoint(np);
        }

        public List<CSUtility.Animation.TimeLineKeyFrameObjectInterface> GetKeyFrames()
        {
            var retList = new List<CSUtility.Animation.TimeLineKeyFrameObjectInterface>();
            foreach (var kf in mPoints)
            {
                var obj = kf as CSUtility.Animation.TimeLineKeyFrameObjectInterface;
                if(obj != null)
                    retList.Add(obj);
            }

            return retList;
        }
        public virtual Type GetKeyFrameType()
        {
            return typeof(ScenePoint_Camera);
        }

        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (!base.Read(xndAtt))
                return false;

            foreach (var pt in Points)
            {
                var cPt = pt as ScenePoint_Camera;
                if (cPt != null)
                    cPt.OnKeyTimeChanged = new ScenePoint_Camera.Delegate_OnKeyTimeChanged(_OnKeyTimeChanged);
            }

            SortPoints();

            return true;
        }
        private void SortPoints()
        {
            Points.Sort(delegate(ScenePoint a, ScenePoint b)
            {
                var ca = a as ScenePoint_Camera;
                var cb = b as ScenePoint_Camera;
                if (ca == null || cb == null)
                    return 0;

                if (ca.KeyTime > cb.KeyTime)
                    return 1;
                else if (ca.KeyTime < cb.KeyTime)
                    return -1;
                else
                    return 0;
            });
        }
        private void _OnKeyTimeChanged(ScenePoint_Camera cp)
        {
            SortPoints();
        }

#endregion

        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public override enScenePointGroupType SPGType
        {
            get { return enScenePointGroupType.CameraPoint; }
            set
            {
                mSPGType = enScenePointGroupType.CameraPoint;
            }
        }

        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false)]
        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;

                if(TimeLineObjectName != value)
                    TimeLineObjectName = value;
            }
        }

        bool mLoop = false;
        [CSUtility.Support.AutoSaveLoad]
        [CategoryAttribute("属性")]
        public bool Loop
        {
            get { return mLoop; }
            set
            {
                mLoop = value;
            }
        }

        #region 回调
        CSUtility.Helper.EventCallBack mOnCameraTickCB;
        [System.ComponentModel.Browsable(false)]
        public CSUtility.Helper.EventCallBack OnCameraTickCB
        {
            get { return mOnCameraTickCB; }
        }
        Guid mOnCameraTick = Guid.Empty;
        [CSUtility.Support.AutoSaveLoad]
        [CategoryAttribute("回调")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet",new object[] { typeof(FOnCameraTick) } )]
        public Guid OnCameraTick
        {
            get { return mOnCameraTick; }
            set
            {
                mOnCameraTick = value;
                mOnCameraTickCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnCameraTick), value);
            }
        }

        CSUtility.Helper.EventCallBack mOnCameraFinishCB;
        [System.ComponentModel.Browsable(false)]
        public CSUtility.Helper.EventCallBack OnCameraFinishCB
        {
            get { return mOnCameraFinishCB; }
        }
        Guid mOnCameraFinish = Guid.Empty;
        [CSUtility.Support.AutoSaveLoad]
        [CategoryAttribute("回调")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FOnCameraFinish) })]
        public Guid OnCameraFinish
        {
            get { return mOnCameraFinish; }
            set
            {
                mOnCameraFinish = value;
                mOnCameraFinishCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnCameraFinish), value);
            }
        }
        #endregion

        public ScenePointGroup_Camera()
        {
            LineType = enLineType.Spline;
        }

        public CSUtility.Animation.TimeLineKeyFrameObjectInterface AddCameraPoint(Int64 startTime, string name)
        {
            var sPt = new ScenePoint_Camera(startTime, name);
            // todo: 取得当前摄像机的参数
            if (OnGetCameraTrans != null)
            {
                sPt.TransMatrix = OnGetCameraTrans();
            }

            for(int i = mPoints.Count - 1; i >= 0; i--)
            {
                var camPt = mPoints[i] as ScenePoint_Camera;
                if(camPt == null)
                    continue;

                if(camPt.KeyFrameMilliTimeStart < startTime)
                {
                    InsertScenePoint(i+1, sPt);
                    return sPt;
                }
                else if(camPt.KeyFrameMilliTimeStart == startTime)
                {
                    camPt.KeyTime = startTime;
                    camPt.KeyFrameName = name;
                    camPt.TransMatrix = sPt.TransMatrix;
                    return sPt;
                }
            }

            AddScenePoint(sPt);

            return sPt;
        }
    }
}
