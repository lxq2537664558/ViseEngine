using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Component
{
    // 拖尾
    /// <summary>
    /// 拖尾信息
    /// </summary>
    [CCore.Socket.SocketComponentInfoAttribute("拖尾")]
    public class TrailInfo : CCore.Socket.ISocketComponentInfo, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 拖尾信息属性改变的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        Guid mSocketComponentInfoId = Guid.NewGuid();
        /// <summary>
        /// socket的成员ID
        /// </summary>
        [CSUtility.Support.DataValueAttribute("SocketComponentInfoId")]
        public Guid SocketComponentInfoId
        {
            get { return mSocketComponentInfoId; }
            set { mSocketComponentInfoId = value; }
        }

        string mSocketName = "";
        /// <summary>
        /// 挂接点的名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("SocketName")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSocketSetter")]
        [DisplayName("挂接点")]
        public string SocketName
        {
            get { return mSocketName; }
            set
            {
                mSocketName = value;
                OnPropertyChanged("SocketName");
            }
        }

        string mDescription = "";
        /// <summary>
        /// 挂接点的说明
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Description")]
        [DisplayName("说明")]
        public string Description
        {
            get { return mDescription; }
            set
            {
                mDescription = value;
                OnPropertyChanged("Description");
            }
        }
        /// <summary>
        /// 只读属性，挂接点成员类型
        /// </summary>
        [Browsable(false)]
        public string SocketComponentType
        {
            get { return "拖尾"; }
        }

        Guid mTechId = CSUtility.Support.IFileConfig.DefaultTechniqueId;
        /// <summary>
        /// 挂接点材质实例的ID
        /// </summary>
        [CSUtility.Support.DataValueAttribute("TechId")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("TechniqueSet")]
        [DisplayName("材质实例")]
        public Guid TechId
        {
            get { return mTechId; }
            set
            {
                mTechId = value;
                OnPropertyChanged("TechId");
            }
        }

        string mTargetSocketName = "HP_Socket_handweapon_R";
        /// <summary>
        /// 目标挂接点的名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("TargetSocketName")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSocketSetter")]
        [DisplayName("目标挂接点")]
        [Category("拖尾属性")]
        public string TargetSocketName
        {
            get { return mTargetSocketName; }
            set
            {
                mTargetSocketName = value;
                OnPropertyChanged("TargetSocketName");
            }
        }

        float mTrailLifeTime = 0.5f;
        /// <summary>
        /// 拖尾每个顶点的生存时间
        /// </summary>
        [CSUtility.Support.DataValueAttribute("TrailLifeTime")]
        [DisplayName("生存期(秒)")]
        [Description("拖尾每个顶点的生存时间")]
        [Category("拖尾属性")]
        public float TrailLifeTime
        {
            get { return mTrailLifeTime; }
            set
            {
                mTrailLifeTime = value;
                OnPropertyChanged("TrailLifeTime");
            }
        }

        float mTrailEmitInterval = 0.01f;
        /// <summary>
        /// 发射时间间隔(秒)
        /// </summary>
        [CSUtility.Support.DataValueAttribute("TrailEmitInterval")]
        [DisplayName("发射时间间隔(秒)")]
        [Description("拖尾每个顶点的生存时间")]
        [Category("拖尾属性")]
        public float TrailEmitInterval
        {
            get { return mTrailEmitInterval; }
            set
            {
                mTrailEmitInterval = value;
                OnPropertyChanged("TrailEmitInterval");
            }
        }

        float mTrailMinDistance = 0.005f;
        /// <summary>
        /// 发射距离间隔(米)
        /// </summary>
        [CSUtility.Support.DataValueAttribute("TrailMinDistance")]
        [DisplayName("发射距离间隔(米)")]
        [Description("拖尾两次发射的顶点之间最小距离，小于此距离就不发射顶点")]
        [Category("拖尾属性")]
        public float TrailMinDistance
        {
            get { return mTrailMinDistance; }
            set
            {
                mTrailMinDistance = value;
                OnPropertyChanged("TrailMinDistance");
            }
        }
        /// <summary>
        /// 复制另一个挂接成员的信息
        /// </summary>
        /// <param name="srcInfo">复制的挂接成员源数据</param>
        public void CopyComponentInfoFrom(CCore.Socket.ISocketComponentInfo srcInfo)
        {
            TrailInfo tInfo = srcInfo as TrailInfo;
            if (tInfo == null)
                return;

            SocketName = tInfo.SocketName;
            Description = tInfo.Description;
            TechId = tInfo.TechId;
            TargetSocketName = tInfo.TargetSocketName;
            TrailLifeTime = tInfo.TrailLifeTime;
            TrailEmitInterval = tInfo.TrailEmitInterval;
            TrailMinDistance = tInfo.TrailMinDistance;
        }
        /// <summary>
        /// 挂接成员类型
        /// </summary>
        /// <returns>返回挂接成员类型为trail</returns>
        public Type GetSocketComponentType()
        {
            return typeof(Trail);
        }
    }
    /// <summary>
    /// 拖尾
    /// </summary>
    public class Trail : CCore.Mesh.Mesh, CCore.Socket.ISocketComponent, CCore.Socket.ISocketComponentPublisherRes, INotifyPropertyChanged
    {
        #region 分析资源
        /// <summary>
        /// 只读属性，资源类型
        /// </summary>
        public override CSUtility.Support.enResourceType ResourceType
        {
            get { return CSUtility.Support.enResourceType.Technique; }
        }
        /// <summary>
        /// 只读属性，拖尾参数对象列表
        /// </summary>
        public override object[] Param
        {
            get
            {
                var tInfo = SocketComponentInfo as TrailInfo;
                return new object[] { tInfo.TechId };
            }
        }
        #endregion
        /// <summary>
        /// 初始化挂接点的成员
        /// </summary>
        /// <param name="info">挂接成员信息</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool InitializeSocketComponent(CCore.Socket.ISocketComponentInfo info)
        {
            var tInfo = info as TrailInfo;
            if (tInfo == null)
                return false;
   
            SocketComponentInfo = info;

            var meshInit = new CCore.Mesh.MeshInit();
            meshInit.MeshInitParts = new List<Mesh.MeshInitPart>()
            {
                new Mesh.MeshInitPart()
                {
                    MeshName = "@Trail",
                    Techs = new List<Guid>() { tInfo.TechId },
                },
            };
            meshInit.CanHitProxy = false;
            Initialize(meshInit, null);

            SetTrailLife(tInfo.TrailLifeTime);
            SetTrailEmitInterval(tInfo.TrailEmitInterval);
            SetTrailMinDistance(tInfo.TrailMinDistance);
            //SetTrailSegment();

            EnableTrail = true;

            return true;
        }
        /// <summary>
        /// 是否存在拖尾，默认为false
        /// </summary>
        protected bool mEnableTrail = false;
        /// <summary>
        /// 是否有拖尾
        /// </summary>
        public bool EnableTrail
        {
            get { return mEnableTrail; }
            set
            {
                mEnableTrail = value;

                foreach (var mp in mMeshParts)
                {
                    DllImportAPI.V3DTrailModifier_SetEnableTrail(mp.TrailModifier, value);
                }
            }
        }
        /// <summary>
        /// 是否自动繁衍，默认为true
        /// </summary>
        protected bool mAutoSpawn = true;
        /// <summary>
        /// 是否自动繁衍
        /// </summary>
        public bool AutoSpawn
        {
            get { return mAutoSpawn; }
            set
            {
                mAutoSpawn = value;

                foreach (var mp in mMeshParts)
                {
                    DllImportAPI.V3DTrailModifier_SetAutoSpawn(mp.TrailModifier, value);
                }
            }
        }
        /// <summary>
        /// 使用样条，默认为true
        /// </summary>
        protected bool mUseSpline = true;
        /// <summary>
        /// 是否使用样条
        /// </summary>
        public bool UseSpline
        {
            get { return mUseSpline; }
            set
            {
                mUseSpline = value;

                foreach (var mp in mMeshParts)
                {
                    DllImportAPI.V3DTrailModifier_SetUseSpline(mp.TrailModifier, value);
                }
            }
        }
        /// <summary>
        /// 是否死亡，默认为false
        /// </summary>
        public bool mIsDead = false;
        /// <summary>
        /// 拖尾缓冲池大小
        /// </summary>
        public int TrailPoolSize
        {
            get
            {
                foreach (var mp in mMeshParts)
                {
                    return DllImportAPI.V3DTrailModifier_GetTrailPoolSize(mp.TrailModifier);
                }
                return 0;
            }
            set
            {
                foreach (var mp in mMeshParts)
                {
                    DllImportAPI.V3DTrailModifier_SetTrailPoolSize(mp.TrailModifier, value);
                }
            }
        }


        int mMaxSegmentPerSpawnPoint = 5;
        /// <summary>
        /// 繁衍出的最大的数量
        /// </summary>
        public int MaxSegmentPerSpawnPoint
        {
            get { return mMaxSegmentPerSpawnPoint; }
            set
            {
                unsafe
                {
                    mMaxSegmentPerSpawnPoint = value;
                    foreach (var mp in mMeshParts)
                    {
                        DllImportAPI.V3DTrailModifier_SetMaxSegmentPerSpawnPoint(mp.TrailModifier, value);
                    }
                }
            }
        }

        //ISimpleSpline mSpline = new ISimpleSpline();
        /// <summary>
        /// 分配拖尾部分
        /// </summary>
        /// <param name="posList">位置坐标列表</param>
        /// <param name="width">宽</param>
        /// <param name="eye">视野</param>
        /// <param name="rightDir">方向</param>
        public void AllocTrailSegments(List<SlimDX.Vector3> posList, float width, CCore.Camera.CameraObject eye, SlimDX.Vector3 rightDir)
        {
            if (posList.Count < 2)
                return;

            unsafe
            {
                unsafe
                {
                    foreach (var mp in mMeshParts)
                    {
                        DllImportAPI.V3DTrailModifier_ClearTrail(mp.TrailModifier);
                        for (int i = 0; i < posList.Count; ++i)
                        {
                            var pos = posList[i];

                            var pos1 = pos + SlimDX.Vector3.Multiply(rightDir, width / 2.0f);
                            var pos2 = pos - SlimDX.Vector3.Multiply(rightDir, width / 2.0f);
                            DllImportAPI.V3DTrailModifier_AllocSegment(mp.TrailModifier, &pos1, &pos2);
                        }
                    }

                    // 使用一个TrailMesh的做法，   在沿着posRight向外扩充顶点的时候总是计算不对，三角形有交叉。
                    //SlimDX.Matrix bM=SlimDX.Matrix.Identity;
                    //eye.GetBillboardMatrix(ref bM);
                    //var eyeRight = SlimDX.Vector3.TransformNormal(SlimDX.Vector3.UnitX, bM);
                    //eyeRight.Normalize();

                    //mSpline.ClearPoints();
                    //foreach(var pos in posList)
                    //{
                    //    mSpline.AddPoint(pos);
                    //}
                    //mSpline.RecalcTangents();

                    //// 点的走向
                    //List<SlimDX.Vector3> pointDirList = new List<SlimDX.Vector3>();
                    //for ( int i = 0; i < posList.Count; ++i )
                    //{
                    //    SlimDX.Vector3 pDir = SlimDX.Vector3.UnitX;
                    //    if(i == 0)
                    //    {
                    //        pDir = posList[i+1]-posList[i];
                    //    }
                    //    else
                    //    {
                    //        pDir = posList[i]-posList[i-1];
                    //    }
                    //    pDir.Normalize();
                    //    pointDirList.Add(pDir);
                    //}

                    //var initDir = pointDirList[0];
                    //foreach (var mp in mMeshParts)
                    //{
                    //    IDllImportAPI.V3DTrailModifier_ClearTrail(mp.TrailModifier);
                    //    for (int i = 0; i < posList.Count; ++i)
                    //    {
                    //        var pos = posList[i];
                    //        var posDir = pointDirList[i];

                    //        // 点的法向量
                    //        var posNorm = pos - eye.Location;
                    //        posNorm.Normalize();
                    //        var tangent = mSpline.GetTangent(i);
                    //        tangent.Normalize();
                    //        var dotD = SlimDX.Vector3.Dot(posNorm, tangent);
                    //        SlimDX.Vector3 posRight = SlimDX.Vector3.UnitX;
                    //        if (dotD < 0.99)
                    //        {
                    //            //posRight = SlimDX.Vector3.Cross(posNorm, posDir);
                    //            posRight = SlimDX.Vector3.Cross(posNorm, tangent);
                    //        }
                    //        else
                    //        {
                    //            posRight = eyeRight;
                    //        }
                    //        posRight.Normalize();

                    //        // Method 1
                    //        //var pos1 = pos + SlimDX.Vector3.Multiply(tangent, width / 2.0f);
                    //        //var pos2 = pos - SlimDX.Vector3.Multiply(tangent, width / 2.0f);
                    //        //IDllImportAPI.V3DTrailModifier_AllocSegment(mp.TrailModifier, &pos1, &pos2);

                    //        // Method 2
                    //        var pos1 = pos + SlimDX.Vector3.Multiply(rightDir, width / 2.0f);
                    //        var pos2 = pos - SlimDX.Vector3.Multiply(rightDir, width / 2.0f);
                    //        IDllImportAPI.V3DTrailModifier_AllocSegment(mp.TrailModifier, &pos1, &pos2);


                    //        // Method 3
                    //        //if (i == 0)
                    //        //{
                    //        //    var pos1 = pos + SlimDX.Vector3.Multiply(posRight, width / 2.0f);
                    //        //    var pos2 = pos - SlimDX.Vector3.Multiply(posRight, width / 2.0f);
                    //        //    IDllImportAPI.V3DTrailModifier_AllocSegment(mp.TrailModifier, &pos1, &pos2);
                    //        //}
                    //        //else
                    //        //{
                    //        //    //float pAngle = SlimDX.Vector3.Dot(posDir, initDir);
                    //        //    var preDir = pointDirList[i - 1];
                    //        //    float pAngle = SlimDX.Vector3.Dot(tangent, preDir);
                    //        //    if (pAngle >= 0)            // 避免点交叉
                    //        //    {
                    //        //        var pos1 = pos + SlimDX.Vector3.Multiply(posRight, width / 2.0f);
                    //        //        var pos2 = pos - SlimDX.Vector3.Multiply(posRight, width / 2.0f);
                    //        //        IDllImportAPI.V3DTrailModifier_AllocSegment(mp.TrailModifier, &pos1, &pos2);
                    //        //    }
                    //        //    else
                    //        //    {
                    //        //        var pos2 = pos + SlimDX.Vector3.Multiply(posRight, width / 2.0f);
                    //        //        var pos1 = pos - SlimDX.Vector3.Multiply(posRight, width / 2.0f);
                    //        //        IDllImportAPI.V3DTrailModifier_AllocSegment(mp.TrailModifier, &pos1, &pos2);
                    //        //    }
                    //        //}

                    //    }
                    //}
                }
            }
        }
        /// <summary>
        /// 设置拖尾的生命值
        /// </summary>
        /// <param name="life">生命值</param>
        public void SetTrailLife(float life)
        {
            unsafe
            {
                foreach (var mp in mMeshParts)
                {
                    DllImportAPI.V3DTrailModifier_SetTrailLife(mp.TrailModifier, life);
                }
            }
        }
        /// <summary>
        /// 设置拖尾发射的区间
        /// </summary>
        /// <param name="interval">发射区间</param>
        public void SetTrailEmitInterval(float interval)
        {
            unsafe
            {
                foreach (var mp in mMeshParts)
                {
                    DllImportAPI.V3DTrailModifier_SetEmitInterval(mp.TrailModifier, interval);
                }
            }
        }
        /// <summary>
        /// 设置拖尾的最小距离
        /// </summary>
        /// <param name="minDist">最小的距离</param>
        public void SetTrailMinDistance(float minDist)
        {
            unsafe
            {
                foreach (var mp in mMeshParts)
                {
                    DllImportAPI.V3DTrailModifier_SetMinDistance(mp.TrailModifier, minDist);
                }
            }
        }
        /// <summary>
        /// 设置拖尾部分
        /// </summary>
        /// <param name="pos1">第一个位置坐标</param>
        /// <param name="pos2">第二个位置坐标</param>
        public void SetTrailSegment(SlimDX.Vector3 pos1, SlimDX.Vector3 pos2)
        {
            unsafe
            {
                foreach (var mp in mMeshParts)
                {
                    DllImportAPI.V3DTrailModifier_SetmTrailPos1(mp.TrailModifier, &pos1);
                    DllImportAPI.V3DTrailModifier_SetmTrailPos2(mp.TrailModifier, &pos2);
                }
            }
        }
        /// <summary>
        /// 拖尾的AABB包围盒
        /// </summary>
        /// <param name="vMin">最小的顶点坐标</param>
        /// <param name="vMax">最大的顶点坐标</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {

        }
        /// <summary>
        /// 提交socket成员
        /// </summary>
        /// <param name="enviroment">渲染环境</param>
        /// <param name="socketMatrix">挂接件的矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="eye">视野</param>
        public override void SocketComponentCommit(CCore.Graphics.REnviroment enviroment, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, CCore.Camera.CameraObject eye)
        {
            if (ComponentHostMesh == null)
                return;

            var tInfo = SocketComponentInfo as TrailInfo;
            if (tInfo == null)
                return;

            if (EnableTrail)
            {
                var s1 = ComponentHostMesh.GetSocket(tInfo.SocketName);
                var s2 = ComponentHostMesh.GetSocket(tInfo.TargetSocketName);
                if (s1 == null || s2 == null)
                    return;

                var absPos1 = SlimDX.Vector3.TransformCoordinate(s1.AbsPos, parentMatrix);
                var absPos2 = SlimDX.Vector3.TransformCoordinate(s2.AbsPos, parentMatrix);
                SetTrailSegment(absPos1, absPos2);

                var tempM = SlimDX.Matrix.Identity;
                Commit(enviroment, ref tempM, eye);
            }
            else if (AutoSpawn == false) // 链接法术
            {
                var tempM = SlimDX.Matrix.Identity;
                Commit(enviroment, ref tempM, eye);
            }
        }
        /// <summary>
        /// 将挂接成员带阴影提交
        /// </summary>
        /// <param name="light">所处的光照</param>
        /// <param name="socketMatrix">挂接的矩阵</param>
        /// <param name="parentMatrix">父矩阵</param>
        /// <param name="isDynamic">是否为动态的</param>
        public override void SocketComponentCommitShadow(CCore.Light.Light light, SlimDX.Matrix socketMatrix, SlimDX.Matrix parentMatrix, bool isDynamic)
        {

        }

    }
}