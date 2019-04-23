using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CSUtility.Map
{
    public class ScenePointGroup : CSUtility.Support.XndSaveLoadProxy, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public delegate void Delegate_OnDirtyChanged(bool dirty);
        public Delegate_OnDirtyChanged OnDirtyChanged;
        public delegate void Delegate_OnNameChanged(string name);
        public Delegate_OnNameChanged OnNameChanged;
        public delegate void Delegate_OnTypeChanged(enScenePointGroupType type);
        public Delegate_OnTypeChanged OnTypeChanged;
        public delegate void Delegate_OnPointsChanged();
        public event Delegate_OnPointsChanged OnPointsChanged;
        public delegate void Delegate_OnLineTypeChanged();
        public Delegate_OnLineTypeChanged OnLineTypeChanged;

        UInt32 mVer;
        [CSUtility.Support.AutoSaveLoad]
        [ReadOnlyAttribute(true)]
        [CategoryAttribute("属性")]
        public UInt32 Ver
        {
            get { return mVer; }
            set { mVer = value; }
        }

        protected Guid mId = Guid.NewGuid();
        [CSUtility.Event.Attribute.AllowMember("场景点.属性.ID", Helper.enCSType.Common, "获取场景点的ID")]
        [CSUtility.Support.AutoSaveLoad]
        [ReadOnlyAttribute(true)]
        [CategoryAttribute("属性")]
        public Guid Id
        {
            get { return mId; }
            set { mId = value; }
        }

        protected bool mIsDirty = false;
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;

                if (OnDirtyChanged != null)
                    OnDirtyChanged(mIsDirty);

                OnPropertyChanged("IsDirty");
            }
        }

        protected string mName = "";
        [CSUtility.Event.Attribute.AllowMember("场景点.属性.名称", Helper.enCSType.Common, "获取场景点名称")]
        [CSUtility.Support.AutoSaveLoad]
        [CategoryAttribute("属性")]
        public virtual string Name
        {
            get { return mName; }
            set
            {
                mName = value;
                IsDirty = true;

                if (OnNameChanged != null)
                    OnNameChanged(mName);

                OnPropertyChanged("Name");
            }
        }

        #region npc生成点属性        
        protected UInt16 mActivityId;
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("活动ID")]
        [CategoryAttribute("属性")]
        public UInt16 ActivityId
        {
            get { return mActivityId; }
            set
            {
                mActivityId = value;
                IsDirty = true;

                OnPropertyChanged("ActivityId");
            }
        }

        protected UInt16 mDynamicNpcId;
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("生成npc列表ID")]
        [CategoryAttribute("属性")]
        public UInt16 DynamicNpcId
        {
            get { return mDynamicNpcId; }
            set
            {
                mDynamicNpcId = value;
                IsDirty = true;

                OnPropertyChanged("DynamicNpcId");
            }
        }

        CSUtility.Helper.EventCallBack mScenePointOnTickCB;
        [System.ComponentModel.Browsable(false)]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack ScenePointOnTickCB
        {
            get { return mScenePointOnTickCB; }
        }

        Guid mScenePointOnTick = Guid.Empty;
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet",new object[] { typeof(FOnTick) } )]
        [Category("属性")]
        [DisplayName("ScenePointOnTick")]
        public Guid OnTick
        {
            get { return mScenePointOnTick; }
            set
            {
                mScenePointOnTick = value;
                mScenePointOnTickCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnTick), value);

                IsDirty = true;

                OnPropertyChanged("OnTick");
            }
        }

        CSUtility.Helper.EventCallBack mScenePointOnInitCB;
        [System.ComponentModel.Browsable(false)]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack ScenePointOnInitCB
        {
            get { return mScenePointOnInitCB; }
        }

        Guid mScenePointOnInit = Guid.Empty;
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet",new object[] { typeof(FOnInit) } )]
        [Category("属性")]
        public Guid ScenePointOnInit
        {
            get { return mScenePointOnInit; }
            set
            {
                mScenePointOnInit = value;
                mScenePointOnInitCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnInit), value);

                IsDirty = true;

                OnPropertyChanged("ScenePointOnInit");
            }
        }
        #endregion

        public enum enScenePointGroupType
        {
            ScenePoint,
            PatrolPoint,
            CameraPoint,
            TransferPoint,
            NavigationPoint,
            CreateNpcPoint,
        }
        protected enScenePointGroupType mSPGType = enScenePointGroupType.ScenePoint;
        [CSUtility.Event.Attribute.AllowMember("场景点.属性.类型", Helper.enCSType.Common, "获取场景点组类型")]
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("类型")]
        [CategoryAttribute("属性")]
        public virtual enScenePointGroupType SPGType
        {
            get { return mSPGType; }
            set
            {
                mSPGType = value;

                IsDirty = true;

                if (OnTypeChanged != null)
                    OnTypeChanged(mSPGType);
            }
        }

        // 注意：编辑器操作此成员需要通过Add、Insert、Remove函数来处理，不能直接操作List
        protected List<ScenePoint> mPoints = new List<ScenePoint>();
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoad]
        public List<ScenePoint> Points
        {
            get { return mPoints; }
            set
            {
                mPoints = value;
                IsDirty = true;

                if (OnPointsChanged != null)
                    OnPointsChanged();
            }
        }

        public enum enLineType
        {
            Line,
            Spline,
        }
        protected enLineType mLineType = enLineType.Line;
        [CSUtility.Support.AutoSaveLoad]
        [CategoryAttribute("属性")]
        public virtual enLineType LineType
        {
            get { return mLineType; }
            set
            {
                mLineType = value;
                IsDirty = true;

                if (OnLineTypeChanged != null)
                    OnLineTypeChanged();
            }
        }

        private float mTotalLength = 0;
        [Browsable(false)]
        public float TotalLength
        {
            get { return mTotalLength; }
        }

        CSUtility.Support.SimpleSpline mSimpleSpline;
        
        public ScenePointGroup()
        {
            mSimpleSpline = new Support.SimpleSpline();
        }

        ~ScenePointGroup()
        {
            mSimpleSpline.Clear();
        }

        private float CalculateTotalLength()
        {
            float totalLength = 0;
            for (int i = 1; i < Points.Count; i++)
            {
                totalLength += (Points[i].GetPosition() - Points[i - 1].GetPosition()).Length();
            }

            return totalLength;
        }

        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (!base.Read(xndAtt))
                return false;

            foreach (var pt in Points)
            {
                pt.HostGroup = this;
                pt.OnMatrixChanged += this.ScenePoint_OnMatrixChanged;
                var pos = pt.GetPosition();
                mSimpleSpline.AddPoint(ref pos);
            }

            mTotalLength = CalculateTotalLength();

            IsDirty = false;
            return true;
        }

        void ScenePoint_OnMatrixChanged(ScenePoint pt)
        {
            UInt16 idx = (UInt16)Points.IndexOf(pt);
            var pos = pt.GetPosition();
            mSimpleSpline.UpdatePoint(idx, ref pos);
        }

        public ScenePoint AddScenePoint(SlimDX.Matrix mat)
        {
            ScenePoint pt = new ScenePoint();
            pt.HostGroup = this;
            pt.TransMatrix = mat;
            pt.OnMatrixChanged += this.ScenePoint_OnMatrixChanged;
            Points.Add(pt);
            var pos = pt.GetPosition();
            mSimpleSpline.AddPoint(ref pos);
            IsDirty = true;

            mTotalLength = CalculateTotalLength();

            if (OnPointsChanged != null)
                OnPointsChanged();

            return pt;
        }
        public ScenePoint AddScenePoint(SlimDX.Vector3 pt)
        {
            ScenePoint sPt = new ScenePoint();
            sPt.HostGroup = this;
            sPt.TransMatrix = SlimDX.Matrix.Translation(pt);
            sPt.OnMatrixChanged += this.ScenePoint_OnMatrixChanged;
            Points.Add(sPt);
            var pos = sPt.GetPosition();
            mSimpleSpline.AddPoint(ref pos);
            IsDirty = true;

            mTotalLength = CalculateTotalLength();

            if (OnPointsChanged != null)
                OnPointsChanged();

            return sPt;
        }
        public void AddScenePoint(ScenePoint sPt)
        {
            if (sPt == null)
                return;

            sPt.HostGroup = this;
            sPt.OnMatrixChanged += this.ScenePoint_OnMatrixChanged;
            Points.Add(sPt);
            var pos = sPt.GetPosition();
            mSimpleSpline.AddPoint(ref pos);
            IsDirty = true;

            mTotalLength = CalculateTotalLength();

            if (OnPointsChanged != null)
                OnPointsChanged();
        }

        public ScenePoint InsertScenePoint(int idx, SlimDX.Matrix mat)
        {
            if (idx < 0)
                return null;
            if (idx >= Points.Count)
                return AddScenePoint(mat);

            ScenePoint pt = new ScenePoint();
            pt.HostGroup = this;
            pt.TransMatrix = mat;
            pt.OnMatrixChanged += this.ScenePoint_OnMatrixChanged;
            Points.Insert(idx, pt);
            var pos = pt.GetPosition();
            mSimpleSpline.InsertPoint(idx, ref pos);

            IsDirty = true;

            mTotalLength = CalculateTotalLength();

            if (OnPointsChanged != null)
                OnPointsChanged();

            return pt;
        }
        public ScenePoint InsertScenePoint(int idx, SlimDX.Vector3 pt)
        {
            if (idx < 0)
                return null;
            if (idx >= Points.Count)
                return AddScenePoint(pt);

            ScenePoint sPt = new ScenePoint();
            sPt.HostGroup = this;
            sPt.TransMatrix = SlimDX.Matrix.Translation(pt);
            sPt.OnMatrixChanged += this.ScenePoint_OnMatrixChanged;
            Points.Insert(idx, sPt);
            mSimpleSpline.InsertPoint(idx, ref pt);

            IsDirty = true;

            mTotalLength = CalculateTotalLength();

            if (OnPointsChanged != null)
                OnPointsChanged();

            return sPt;
        }
        public void InsertScenePoint(int idx, ScenePoint sPt)
        {
            if (idx < 0)
                return;
            if (idx >= Points.Count)
            {
                AddScenePoint(sPt);
                return;
            }

            sPt.HostGroup = this;
            sPt.OnMatrixChanged += this.ScenePoint_OnMatrixChanged;
            Points.Insert(idx, sPt);
            var pos = sPt.GetPosition();
            mSimpleSpline.InsertPoint(idx, ref pos);

            IsDirty = true;

            mTotalLength = CalculateTotalLength();

            if (OnPointsChanged != null)
                OnPointsChanged();
        }
        public void DeleteScenePoint(ScenePoint pt)
        {
            if (pt == null)
                return;
            
            mSimpleSpline.RemovePoint(mPoints.IndexOf(pt));
            mPoints.Remove(pt);
            pt.OnMatrixChanged -= this.ScenePoint_OnMatrixChanged;

            IsDirty = true;

            mTotalLength = CalculateTotalLength();

            if (OnPointsChanged != null)
                OnPointsChanged();
        }
        public void DeleteScenePoint(int idx)
        {
            if (idx < 0 || idx >= mPoints.Count)
                return;

            mSimpleSpline.RemovePoint(idx);
            mPoints.RemoveAt(idx);
            mPoints[idx].OnMatrixChanged -= this.ScenePoint_OnMatrixChanged;
            IsDirty = true;

            mTotalLength = CalculateTotalLength();

            if (OnPointsChanged != null)
                OnPointsChanged();
        }

        // percent 0~1
        public SlimDX.Vector3 GetPosition(float percent)
        {
            if(Points.Count == 0)
                return SlimDX.Vector3.Zero;

            if(percent < 0)
                percent = 0;
            else if(percent > 1)
                percent = 1;

            if (percent == 0)
                return Points[0].GetPosition();
            else if (percent == 1)
                return Points[Points.Count - 1].GetPosition();

            switch (LineType)
            {
                case enLineType.Spline:
                    return mSimpleSpline.Interpolate(percent);

                case enLineType.Line:
                    {
                        float targetLength = TotalLength * percent;
                        float length = 0;
                        for (int i = 1; i < Points.Count; i++)
                        {
                            var posEnd = Points[i].GetPosition();
                            var posStart = Points[i - 1].GetPosition();
                            var lineLength = (posEnd - posStart).Length();
                            length += lineLength;
                            if (length > targetLength)
                            {
                                var lenPer = (lineLength - (length - targetLength)) / lineLength;
                                return (posEnd - posStart) * lenPer + posStart;
                            }
                        }
                    }
                    break;
            }

            return SlimDX.Vector3.Zero;
        }

        // percent 0~1
        public SlimDX.Quaternion GetRotation(float percent)
        {
            if (Points.Count == 0)
                return SlimDX.Quaternion.Identity;

            if (percent < 0)
                percent = 0;
            else if (percent > 1)
                percent = 1;

            if (percent == 0)
                return Points[0].GetRotation();
            else if (percent == 1)
                return Points[Points.Count - 1].GetRotation();

            float targetLength = TotalLength * percent;
            float length = 0;
            for (int i = 1; i < Points.Count; i++)
            {
                var posEnd = Points[i].GetPosition();
                var posStart = Points[i - 1].GetPosition();
                var lineLength = (posEnd - posStart).Length();
                length += lineLength;
                if (length > targetLength)
                {
                    var lenPer = (lineLength - (length - targetLength)) / lineLength;
                    return SlimDX.Quaternion.Slerp(Points[i - 1].GetRotation(), Points[i].GetRotation(), lenPer);
                }
            }

            return SlimDX.Quaternion.Identity;
        }
    }
}
