namespace CCore.Effect
{
    /*public class ChainMesh
    {
        public CCore.Mesh.Mesh mTrailMeshRight = null;
        public CCore.Mesh.Mesh mTrailMeshUp = null;

        public bool IsDead
        {
            set
            {
                if (mTrailMeshRight != null)
                    mTrailMeshRight.mIsDead = value;
                if (mTrailMeshUp != null)
                    mTrailMeshUp.mIsDead = value;
            }
        }

        public void AllocTrailSegments(List<SlimDX.Vector3> posList, float width)
        {
            var eye = FrameSet.Role.ChiefRoleActorController.Instance.CameraController.Camera;
            var eyeRight = new SlimDX.Vector3();
            var eyeUp = new SlimDX.Vector3();
            eye.GetLocalRight(ref eyeRight);
            eye.GetLocalUp(ref eyeUp);
            if(mTrailMeshRight!=null)
                mTrailMeshRight.AllocTrailSegments(posList, width, eye, eyeRight);
            if (mTrailMeshUp != null)
                mTrailMeshUp.AllocTrailSegments(posList, width, eye, eyeUp);
        }
    }
   
    public class ChainEffect
    {
        public enum EEffectChainType
        {
            Line = 0,       // 直线
            Parabola = 1,   // 抛物线
            Curve = 2,      // 曲线
        }
        public static ChainEffect CreateChainEffecct(CSCommon.Data.Skill.RuneTemplate.Effect e, FrameSet.Role.RoleActor ownerRole, CSCommon.Data.Skill.RuneTemplate rune, string fireSocketName)
        {
            var chain = new ChainEffect();

            chain.mOwnerRole = ownerRole;
            chain.mTrailMeshId = e.ChainId;
            chain.mTrailPoolSize = e.TrailPoolSize;
            chain.mTrailWidth = e.TrailWidth;
            chain.mDuration = e.ChainDuration;
            chain.mSegmentDuration = (float)e.ChainSegmentDuration/1000.0f;
            chain.mSegmentMinDistance = e.SegmentMinDistance;
            chain.mChainType = e.ChainType;
            chain.mOwnerRuneId = rune.RuneId;
            chain.mRune = rune;
            chain.mTopInLinePercent = e.TopInLinePercent;
            chain.mTopHeight = e.TopHeight;
            chain.mFireSocketName = fireSocketName;

            return chain;
        }

        CCore.Mesh.Mesh _CreateTrailMesh()
        {
            var meshInit = new CCore.Mesh.MeshInit();
            meshInit.MeshTemplateID = mTrailMeshId;
            var trailMesh = new CCore.Mesh.Mesh();
            trailMesh.Initialize(meshInit, null);
            if (trailMesh.IsTrail == false)
                return null;

            trailMesh.AutoSpawn = false;
            trailMesh.TrailPoolSize = mTrailPoolSize;
            trailMesh.SetTrailLife(mSegmentDuration);
            trailMesh.SetTrailEmitInterval(0.01f);
            trailMesh.SetTrailMinDistance(mSegmentMinDistance);
            trailMesh.MaxSegmentPerSpawnPoint = 20;
            switch(mChainType)
            {
                case EEffectChainType.Line:
                    trailMesh.UseSpline = false;
                    break;
                case EEffectChainType.Curve:
                case EEffectChainType.Parabola:
                    trailMesh.UseSpline = true;
                    break;
            }

            return trailMesh;
        }

        ChainMesh _AddChainMesh()
        {
            ChainMesh cm = new ChainMesh();

            cm.mTrailMeshRight = _CreateTrailMesh();
            cm.mTrailMeshUp = _CreateTrailMesh();

            mChainMeshes.Add(cm);

            return cm;
        }

        public List<CCore.Mesh.Mesh> CreateTrailMesh()
        {
            mChainMeshes.Clear();
            if(mRune.RuneType == CSCommon.Data.Skill.ERuneType.OneChain)
            {
                _AddChainMesh();
            }
            else if (mRune.RuneType == CSCommon.Data.Skill.ERuneType.MultiChain)
            {
                for( int i = 0; i < mRune.RuneTargetNumber; ++i )
                {
                    _AddChainMesh();
                }
            }

            var ret = new List<FrameSet.Mesh.Mesh>();
            foreach(var chainMesh in mChainMeshes)
            {
                ret.Add(chainMesh.mTrailMeshRight);
                ret.Add(chainMesh.mTrailMeshUp);
            }
            return ret;
        }
        
        public Guid mTrailMeshId;
        string mFireSocketName = "";

        List<ChainMesh> mChainMeshes = new List<ChainMesh>();

        public int mTrailPoolSize = 100;
        public long mDuration = 2;                         // 生存期
        public long mLiveTime = 0;                         // 存在时间
        bool mIsDead = false;
        public bool IsDead
        {
            get { return mIsDead; }
            set
            {
                mIsDead = value;
                foreach (var chainMesh in mChainMeshes)
                {
                    chainMesh.IsDead = value;
                }
            }
        }

        EEffectChainType mChainType = EEffectChainType.Line;
        public float mSegmentDuration;           // TrailSegment存活时间
        public float mTrailWidth = 0.5f;           // Trail宽度
        public float mSegmentMinDistance = 0.5f;       // TrailSegment最小间隔（米）
        public int mOwnerRuneId = 0;
        public int mRuneHandle = 0;

        float mTopInLinePercent = 0.3f;
        float mTopHeight = 3.0f;

        List<UInt32> mHurtRoleSingleIds = new List<UInt32>();           // 服务器传下来击中的RoleId
        public List<UInt32> HurtRoleSingleIds
        {
            get { return mHurtRoleSingleIds; }
            set
            {
                mHurtRoleSingleIds = value;
            }
        }
        public List<FrameSet.Role.RoleActor> mHurtRoles = new List<FrameSet.Role.RoleActor>();       

        public class RolePos
        {
            public FrameSet.Role.RoleActor role;
            public float distance;
        }
        private static int ComparePosByDistance(RolePos p1, RolePos p2)
        {
            if (p1.distance > p2.distance)
                return 1;
            else if (p1.distance < p2.distance)
                return -1;
            else
                return 0;
        }

        bool bRoleSorted = false;
        List<RolePos> rolePosList = new List<RolePos>();
        public void Tick(long elapsedMillisecond)
        {
            if (mChainMeshes.Count<=0)
                return;

            mLiveTime += elapsedMillisecond;
            if (mLiveTime > mDuration)
            {
                IsDead = true;
            }

            // 将击中的玩家的位置设置给TrailMesh
            if(mHurtRoles.Count>0 && mOwnerRole!=null)
            {
                if(bRoleSorted==false)
                {
                    var ownerRolePos = mOwnerRole.Placement.GetLocation();
                    rolePosList.Clear();
                    RolePos rolePos = new RolePos();
                    rolePos.role = mOwnerRole;
                    rolePos.distance = 0.0f;
                    rolePosList.Add(rolePos);
                    foreach (var hurtRole in mHurtRoles)
                    {
                        if (hurtRole == null)
                            continue;

                        rolePos = new RolePos();
                        rolePos.role = hurtRole;
                        rolePos.distance = (hurtRole.Placement.GetLocation() - ownerRolePos).LengthSquared();
                        rolePosList.Add(rolePos);
                    }

                    if(mRune.RuneType == CSCommon.Data.Skill.ERuneType.OneChain)
                        rolePosList.Sort(ComparePosByDistance);

                    bRoleSorted = true;
                }

                List<SlimDX.Vector3> posList = new List<SlimDX.Vector3>();
                for( int i = 0; i < rolePosList.Count; ++i )
                //foreach(var rPos in rolePosList)
                {
                    var rPos = rolePosList[i];
                    if(rPos.role==null)
                        continue;

                    var pos = rPos.role.Placement.GetLocation() + SlimDX.Vector3.UnitY;
                    var roleVisual = rPos.role.Visual as FrameSet.Role.RoleActorVisual;
                    if(roleVisual!=null)
                    {
                        CCore.Socket.Socket socket = null;
                        if (i == 0)
                            socket = roleVisual.GetSocket(mFireSocketName);
                        else
                            socket = roleVisual.GetSocket(mRune.ChainSocketName);

                        if (socket != null)
                        {
                            SlimDX.Vector3 parentPos = new SlimDX.Vector3();
                            SlimDX.Vector3 parentScale = new SlimDX.Vector3();
                            SlimDX.Quaternion parentQuat = new SlimDX.Quaternion();
                            SlimDX.Matrix roleMatrix = new SlimDX.Matrix();
                            rPos.role.Placement.GetAbsMatrix(out roleMatrix);
                            roleMatrix.Decompose(out parentScale, out parentQuat, out parentPos);
                            var tempPos = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Modulate(socket.AbsPos, parentScale), parentQuat);
                            pos = parentPos + tempPos;
                        }
                    }

                    posList.Add(pos);
                }

                switch(mRune.RuneType)
                {
                    case CSCommon.Data.Skill.ERuneType.OneChain:
                        {
                            foreach (var chainMesh in mChainMeshes)
                            {
                                List<SlimDX.Vector3> newPosList = new List<SlimDX.Vector3>();
                                for(int i = 0; i < posList.Count; ++i)
                                {
                                    if (i == posList.Count - 1)
                                    {
                                        newPosList.Add(posList[i]);
                                    }
                                    else
                                    {
                                        var pos0 = posList[i];
                                        var pos1 = posList[i + 1];
                                        newPosList.Add(pos0);
                                        if (mChainType == CSCommon.Data.Skill.EEffectChainType.Parabola)
                                        {
                                            var posMiddle = SlimDX.Vector3.SmoothStep(pos0, pos1, mTopInLinePercent);
                                            posMiddle += SlimDX.Vector3.UnitY * mTopHeight;
                                            newPosList.Add(posMiddle);
                                        }
                                        newPosList.Add(pos1);
                                    }
                                }
                                chainMesh.AllocTrailSegments(newPosList, mTrailWidth);
                            }
                        }
                        break;
                    case CSCommon.Data.Skill.ERuneType.MultiChain:
                        {
                            for(int i = 1; i < posList.Count; ++i)
                            {
                                List<SlimDX.Vector3> twoPosList = new List<SlimDX.Vector3>();
                                var pos0 = posList[0];
                                var pos1 = posList[i];
                                twoPosList.Add(posList[0]);
                                if (mChainType == CSCommon.Data.Skill.EEffectChainType.Parabola)
                                {
                                    var posMiddle = SlimDX.Vector3.SmoothStep(pos0, pos1, mTopInLinePercent);
                                    posMiddle += SlimDX.Vector3.UnitY * mTopHeight;
                                    twoPosList.Add(posMiddle);
                                }
                                twoPosList.Add(posList[i]);
                                if(i<mChainMeshes.Count)
                                    mChainMeshes[i - 1].AllocTrailSegments(twoPosList, mTrailWidth);
                            }
                        }
                        break;
                    default:
                        break;
                }

            }
        }
    }*/
}
