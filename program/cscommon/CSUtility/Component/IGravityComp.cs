namespace CSUtility.Component
{
    /// <summary>
    /// 重力组件
    /// </summary>
    public class IGravityComp : IComponent
    {
        /// <summary>
        /// 重力值，缺省为-9.8
        /// </summary>
        protected static float	mWorldGravity = -9.8f;
        /// <summary>
        /// 最小值，缺省为0.05
        /// </summary>
		protected static float	mEpsilon = 0.05f;
        /// <summary>
        /// 重力最大值
        /// </summary>
        protected float mGravity = float.MaxValue;
        /// <summary>
        /// 速度向量
        /// </summary>
		protected SlimDX.Vector3	mVelocity;
        /// <summary>
        /// 是否暂停
        /// </summary>
		protected bool			mSuspend;
        /// <summary>
        /// 是否为流动的
        /// </summary>
		protected bool			mIsFloating;
        /// <summary>
        /// 只读属性，是否为流动的
        /// </summary>
        public bool IsFloating
        {
            get { return mIsFloating; }
        }
        /// <summary>
        /// 世界重力值
        /// </summary>
        public static float WorldGravity
        {
            get { return mWorldGravity; }
            set { mWorldGravity = value; }
        }
        /// <summary>
        /// 重力值
        /// </summary>
        public float Gravity
        {
            get { return mGravity; }
            set { mGravity = value; }
        }
        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool Suspend
        {
            get { return mSuspend; }
            set { mSuspend = value; }
        }
        /// <summary>
        /// 速度向量
        /// </summary>
        public SlimDX.Vector3 Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }

        static CSUtility.Performance.PerfCounter mBeforeLCheckTimer = new CSUtility.Performance.PerfCounter("Gravity.BeforeCCheck");
        static CSUtility.Performance.PerfCounter mAfterLCheckTimer = new CSUtility.Performance.PerfCounter("Gravity.AfterCCheck");

        public float LineCheckDistanceLimit
        {
            get;
            set;
        } = 0;

        public IGravityComp(float lineCheckLimit)
        {
            LineCheckDistanceLimit = lineCheckLimit * lineCheckLimit;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="host">主Actor对象</param>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public override void Tick(ActorBase host, long elapsedMillisecond)
        {
		    if( mSuspend )
			    return;
		    IPlacement placement = host.Placement;
		    if(placement==null)
			    return;

            float elapse = (float)elapsedMillisecond/1000.0F;
		    if(elapse==0)
			    return;

            mBeforeLCheckTimer.Begin();
            SlimDX.Vector3 startTest = host.Placement.GetLocation();
            SlimDX.Vector3 location = startTest;
		    if( mVelocity.Y!=0.0F )
		    {
			    if( host.Placement != null )
			    {
                    if (mVelocity.X != 0 || mVelocity.Z != 0)
                    {
                        SlimDX.Vector3 VelocityXZ = new SlimDX.Vector3(mVelocity.X, 0, mVelocity.Z);
                        float velocityXZ = VelocityXZ.Length();
                        VelocityXZ.Normalize();
                        float moveXZ = velocityXZ * elapse;
                        //host.Placement.Move(ref VelocityXZ, moveXZ);
                        location += VelocityXZ * moveXZ;
                    }
			    }
		    }
		
		    float gravity=mGravity;
            if (gravity == float.MaxValue)
                gravity = mWorldGravity;
		    float FallDistance = mVelocity.Y*elapse+0.5f*gravity*elapse*elapse;
		    mVelocity.Y = mVelocity.Y + gravity*elapse;
		    //location.Y -= (halfHeight+mEpsilon);

            startTest.Y += 200.0F;
            SlimDX.Vector3 endTest = new SlimDX.Vector3(location.X, location.Y + FallDistance - 0.3F, location.Z);
            var result = new CSUtility.Support.stHitResult();
            result.mClosedPos = host.Placement.GetLocation();
            result.mClosedDistSq = LineCheckDistanceLimit;
            result.mHitFlags |= (uint)CSUtility.enHitFlag.IgnoreMouseLineCheckInGame;
            mBeforeLCheckTimer.End();
            
            if (host.WorldLineCheck(ref startTest, ref endTest, ref result))
		    {//已经落到碰撞体上了
                mAfterLCheckTimer.Begin();
                location.Y = result.mHitPosition.Y+mEpsilon;
			    mVelocity.Y = 0.0f;
			    mVelocity.X = 0.0f;
			    mVelocity.Z = 0.0f;
			    mIsFloating = false;

                placement.SetLocation(ref location);
                mAfterLCheckTimer.End();
                return;
		    }
		    else
		    {
			    //location.Y = location.Y + FallDistance;
                location.Y = location.Y + FallDistance;
                
			    mIsFloating = true;
            }

            //下面是临时代码测试用
            //{
            //    var prevLoc = placement.GetLocation();
            //    if (host.WorldLineCheck(ref prevLoc, ref location, out result))
            //    {
            //        System.Diagnostics.Debugger.Break();
            //    }
            //}

            mAfterLCheckTimer.Begin();
            var prevPos = placement.GetLocation();
            var moveDir = location - prevPos;
            if (moveDir.Y * gravity <= 0)
                return;

            moveDir.Normalize();
            float dist = SlimDX.Vector3.Distance(location, prevPos);
            placement.Move(ref moveDir,dist*0.95F);
            //placement.SetLocation(ref location);
            mAfterLCheckTimer.End();
        }
    }
}
