using System.Collections.Generic;
using SlimDX;

namespace CCore.Component
{
    /// <summary>
    /// 坐标轴的安置类
    /// </summary>
    public class AxisPlacement : CSUtility.Component.StandardPlacement
    {
        /// <summary>
        /// 轴的放置类型
        /// </summary>
        public enum eAxisPlacementType
        {
            APT_LastOne,
            APT_Center,
        }
        /// <summary>
        /// 移动轴的状态，单独移动轴还是轴和物体一起动
        /// </summary>
        public enum eAxisTransCenterType
		{
			ATCT_AxisPos,
			ATCT_ObjectPos,
		};
        /// <summary>
        /// 坐标系的类型
        /// </summary>
		public enum eAxisType
		{
			AT_Local,
			AT_World,
		};
        /// <summary>
        /// 挂接的数据类
        /// </summary>
        protected class stLinkedData
        {
            private CSUtility.Component.StandardPlacement mLinkedPlacement;
            /// <summary>
            /// 挂接件的位置
            /// </summary>
            public CSUtility.Component.StandardPlacement LinkedPlacement
            {
                get { return mLinkedPlacement; }
            }
            private SlimDX.Vector3 mOffsetVertex;
            /// <summary>
            /// 挂接件的偏移值
            /// </summary>
            public SlimDX.Vector3 OffsetVertex
            {
                get { return mOffsetVertex; }
            }
            private AxisPlacement mHostAxisPlacement;
            /// <summary>
            /// 挂接件的构造函数
            /// </summary>
            /// <param name="host">父对象位置</param>
            /// <param name="placement">挂接件的位置</param>
            public stLinkedData(AxisPlacement host, CSUtility.Component.StandardPlacement placement)
            {
                mHostAxisPlacement = host;
                mLinkedPlacement = placement;
                mOffsetVertex = SlimDX.Vector3.Subtract(mHostAxisPlacement.Location, placement.GetLocation());
            }
        }
        /// <summary>
        /// 保存挂接件的位置和数据
        /// </summary>
        protected Dictionary<CSUtility.Component.IPlacement, stLinkedData> mLinkedDataDictionary = new Dictionary<CSUtility.Component.IPlacement, stLinkedData>();
        /// <summary>
        /// 挂接件的偏移值，默认为0
        /// </summary>
        protected SlimDX.Vector3 m_offsetVertex = SlimDX.Vector3.Zero;
        /// <summary>
        /// 顶点偏移量
        /// </summary>
        public SlimDX.Vector3 OffsetVertex
        {
            get { return m_offsetVertex; }
            set { m_offsetVertex = value; }
        }
        /// <summary>
        /// 上一个挂接件数据，默认为空
        /// </summary>
        protected stLinkedData m_lastPlacementDataInDictionary = null;

        // 存储本placement的矩阵，保证坐标切换时能够正确的计算旋转
        /// <summary>
        /// 存储本placement的矩阵，保证坐标切换时能够正确的计算旋转
        /// </summary>
        protected SlimDX.Matrix m_MatStore = SlimDX.Matrix.Identity;
        /// <summary>
        /// 是否是局部坐标轴
        /// </summary>
        protected bool m_bIsLocalAxis = true;			// 是否是局部坐标轴
        /// <summary>
        /// 设置或得到坐标系是否是局部坐标系
        /// </summary>
        public bool IsLocalAxis
        {
            get { return m_bIsLocalAxis; }
            set
            {
                m_bIsLocalAxis = value;

                if (m_bIsLocalAxis && mLinkedDataDictionary.Count > 0)
                {
			        if(m_lastPlacementDataInDictionary != null)
			        {
				        m_lastPlacementDataInDictionary.LinkedPlacement.GetAbsMatrix(out m_MatStore);
				        SetMatrix(ref m_MatStore);
			        }
		        }
		        else
		        {
			        mQuanternion = SlimDX.Quaternion.Identity;
			        SlimDX.Matrix matTemp = mMatrix;
			        LSQ2Matrix();
			        m_MatStore = matTemp;
		        }
            }
        }
        /// <summary>
        /// 坐标轴的安置类型，默认为APT_LastOne
        /// </summary>
        protected eAxisPlacementType mAxisPlacementType = eAxisPlacementType.APT_LastOne;
        /// <summary>
        /// 坐标轴的安置类型
        /// </summary>
        public eAxisPlacementType AxisPlacementType
        {
            get { return mAxisPlacementType; }
            set { mAxisPlacementType = value; }
        }
        /// <summary>
        /// 坐标轴的移动方式，只移动轴或者与物体一起移动
        /// </summary>
        protected eAxisTransCenterType mAxisTransCenterType = eAxisTransCenterType.ATCT_AxisPos;
        /// <summary>
        /// 设置或者得到坐标轴的移动方式，只移动轴或者与物体一起移动
        /// </summary>
        public eAxisTransCenterType AxisTransCenterType
        {
            get { return mAxisTransCenterType; }
            set { mAxisTransCenterType = value; }
        }
        /// <summary>
        /// 坐标系类型
        /// </summary>
        protected eAxisType mAxisType;
        /// <summary>
        /// 设置或得到该坐标系是世界坐标系还是本地坐标系
        /// </summary>
        public eAxisType AxisType
        {
            get { return mAxisType; }
            set { mAxisType = value; }
        }
        /// <summary>
        /// 设置轴的标准坐标系统
        /// </summary>
        /// <param name="host">父Actor</param>
        public AxisPlacement(CCore.World.Actor host)
            : base(host)
        {

        }
        /// <summary>
        /// 将位置、缩放和旋转数据转换成矩阵
        /// </summary>
        public override void LSQ2Matrix()
        {
            var vLoc = mLocation;
            var vScale = mScale;
            var qRot = mQuanternion;

            //var identityQuat = SlimDX.Quaternion.Identity;
            SlimDX.Matrix.Transformation(vScale, qRot, vLoc, out mMatrix);

            m_MatStore = mMatrix;
        }
        /// <summary>
        /// 设置坐标轴连接的Actor的Placement
        /// </summary>
        /// <param name="actors">需要坐标轴操作的Actor</param>
        public void SetLinkedPlacements(CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> actors)
        {
            mLinkedDataDictionary.Clear();

            foreach (var act in actors)
            {
                if (act.Placement == null)
                    continue;

                var data = new stLinkedData(this, act.Placement);
                mLinkedDataDictionary[act.Placement] = data;
            }

            if ((actors.Count > 0) && (actors[actors.Count - 1].Placement != null))
            {
                mLocation = actors[actors.Count - 1].Placement.GetLocation();
                mQuanternion = actors[actors.Count - 1].Placement.GetRotation();
                LSQ2Matrix();
                m_lastPlacementDataInDictionary = mLinkedDataDictionary[actors[actors.Count - 1].Placement];
            }
            else
            {
                m_lastPlacementDataInDictionary = null;
            }
        }
		/// <summary>
        /// 设置坐标轴Placement的位置坐标
        /// </summary>
        /// <param name="loc">位置坐标</param>
        /// <returns>设置成功返回true</returns>
		public override bool SetLocation( ref SlimDX.Vector3 loc )
        {
            mLocation = loc;
            LSQ2Matrix();

            return true;
        }
        /// <summary>
        /// 设置坐标轴Placement的缩放
        /// </summary>
        /// <param name="scale">缩放值</param>
        /// <returns>设置成功返回true</returns>
        public override bool SetScale(ref SlimDX.Vector3 scale)
        {
            mScale = scale;
            LSQ2Matrix();

            return true;
        }
        /// <summary>
        /// 设置坐标轴Placement的旋转值，不计算连接的Placem变换
        /// </summary>
        /// <param name="quat">旋转四元数</param>
        /// <param name="bImm">是否立即改变</param>
        /// <returns>设置成功返回true</returns>
        public override bool SetRotation(ref SlimDX.Quaternion quat, bool bImm)	// 只进行本Placement的变换，不计算链接的Placement的变换
        {
            if (m_bIsLocalAxis)
            {
                mQuanternion = quat;
                LSQ2Matrix();
            }

            m_MatStore = mMatrix;
            return true;
        }
        /// <summary>
        /// 设置坐标轴和连接的Actor的位置
        /// </summary>
        /// <param name="loc">平移偏移量</param>
        public void SetLocationDeltaWithTargets(ref SlimDX.Vector3 loc)
        {
            foreach (var data in mLinkedDataDictionary.Values)
            {
                var lLoc = data.LinkedPlacement.GetLocation();
                lLoc = lLoc + loc;
                data.LinkedPlacement.SetLocation(ref lLoc);
            }

            m_MatStore *= SlimDX.Matrix.Translation(loc);
            SetMatrix(ref m_MatStore);
        }
        /// <summary>
        /// 设置坐标轴和连接的Actor的旋转偏移值
        /// </summary>
        /// <param name="quat">旋转偏移值</param>
        public void SetRotationDeltaWithTargets(ref SlimDX.Quaternion quat)	// 本placement和链接的placement都进行变换
        {
            var matTrans = SlimDX.Matrix.Transformation(mLocation, SlimDX.Quaternion.Identity, SlimDX.Vector3.UnitXYZ, mLocation, quat, SlimDX.Vector3.Zero);

            switch (mAxisTransCenterType)
            {
                case eAxisTransCenterType.ATCT_AxisPos:
                    {
                        foreach (var data in mLinkedDataDictionary.Values)
                        {
                            SlimDX.Matrix matTemp = data.LinkedPlacement.mMatrix * matTrans;
                            data.LinkedPlacement.SetMatrix(ref matTemp);
                        }
                    }
                    break;

                case eAxisTransCenterType.ATCT_ObjectPos:
                    {
                        foreach (var data in mLinkedDataDictionary.Values)
                        {
                            var vObjLoc = data.LinkedPlacement.GetLocation();
                            var objMatTrans = SlimDX.Matrix.Transformation(vObjLoc, SlimDX.Quaternion.Identity, SlimDX.Vector3.UnitXYZ, vObjLoc, quat, SlimDX.Vector3.Zero);
                            SlimDX.Matrix matTemp = data.LinkedPlacement.mMatrix * objMatTrans;
                            data.LinkedPlacement.SetMatrix(ref matTemp);
                        }
                    }
                    break;
            }

            m_MatStore = m_MatStore * matTrans;

            if (m_bIsLocalAxis)
            {
                SetMatrix(ref m_MatStore);
            }
        }
        /// <summary>
        /// 设置坐标轴和连接Actor的缩放值
        /// </summary>
        /// <param name="scale">缩放值</param>
        public void SetScaleDeltaWithTargets(ref SlimDX.Vector3 scale)
        {
            SlimDX.Quaternion scaleRotation = SlimDX.Quaternion.Identity;
		    if(mAxisType == eAxisType.AT_Local)
			    scaleRotation = mQuanternion;
		    SlimDX.Matrix matTrans = SlimDX.Matrix.Transformation(mLocation, scaleRotation, scale, mLocation, SlimDX.Quaternion.Identity, SlimDX.Vector3.Zero);

		    switch (mAxisTransCenterType)
		    {
                case eAxisTransCenterType.ATCT_AxisPos:
                    {
                        foreach (var data in mLinkedDataDictionary.Values)
                        {
                            SlimDX.Matrix matTemp = data.LinkedPlacement.mMatrix * matTrans;
                            data.LinkedPlacement.SetMatrix(ref matTemp);
                        }
                    }
                    break;
                case eAxisTransCenterType.ATCT_ObjectPos:
                    {
                        foreach (var data in mLinkedDataDictionary.Values)
                        {
                            SlimDX.Quaternion scaleRot = SlimDX.Quaternion.Identity;
                            if (mAxisType == eAxisType.AT_Local)
                                scaleRot = mQuanternion;

                            SlimDX.Vector3 vObjLoc = data.LinkedPlacement.GetLocation();
                            SlimDX.Matrix objMatTrans = SlimDX.Matrix.Transformation(vObjLoc, scaleRot, scale, vObjLoc, SlimDX.Quaternion.Identity, SlimDX.Vector3.Zero);
                            SlimDX.Matrix matTemp = data.LinkedPlacement.mMatrix * objMatTrans;
                            data.LinkedPlacement.SetMatrix(ref matTemp);
                        }
                    }
                    break;
                default:
                    break;
            }

            //m_MatStore = m_MatStore * matTrans;

            //if (m_bIsLocalAxis)
            //{
            //    SetMatrix(ref m_MatStore);
            //}
        }
        /// <summary>
        /// 计算偏移值
        /// </summary>
        /// <param name="vLoc">当前坐标</param>
        public void CalculateOffset(ref SlimDX.Vector3 vLoc)
        {

        }
        /// <summary>
        /// 得到坐标轴的绝对矩阵
        /// </summary>
        /// <param name="matrix">输出坐标轴的绝对矩阵</param>
        /// <returns>如果得到矩阵返回true，否则返回false</returns>
        public override bool GetAbsMatrix(out Matrix matrix)
        {
            if (!base.GetAbsMatrix(out matrix))
                return false;

            if(!m_bIsLocalAxis)
            {
                matrix = Matrix.Transformation(this.Scale, SlimDX.Quaternion.Identity, this.Location);
            }

            return true;
        }
    }
}
