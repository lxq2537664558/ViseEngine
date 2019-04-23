using System;

namespace CSUtility.Component
{
    /// <summary>
    /// 位置类
    /// </summary>
    public abstract class IPlacement : IComponent
    {
        /// <summary>
        /// 声明位置坐标改变时调用的委托对象
        /// </summary>
        /// <param name="loc">位置坐标</param>
        public delegate void Delegate_OnLocationChanged(ref SlimDX.Vector3 loc);
        /// <summary>
        /// 定义位置坐标改变时调用的委托对象
        /// </summary>
        public event Delegate_OnLocationChanged OnLocationChanged;
        /// <summary>
        /// 声明对象旋转时调用的委托对象
        /// </summary>
        /// <param name="rot">旋转四元数</param>
        public delegate void Delegate_OnRotationChanged(ref SlimDX.Quaternion rot);
        /// <summary>
        /// 定义对象旋转时调用的委托对象
        /// </summary>
        public event Delegate_OnRotationChanged OnRotationChanged;
        /// <summary>
        /// 声明对象缩放时调用的委托事件
        /// </summary>
        /// <param name="scale"></param>
        public delegate void Delegate_OnScaleChanged(ref SlimDX.Vector3 scale);
        /// <summary>
        /// 定义对象缩放时调用的委托事件
        /// </summary>
        public event Delegate_OnScaleChanged OnScaleChanged;
        /// <summary>
        /// 构造函数
        /// </summary>
        public IPlacement() { }
        /// <summary>
        /// 删除对象
        /// </summary>
        public void Cleanup() { }
        /// <summary>
        /// 获取Actor对象
        /// </summary>
        /// <returns>返回Actor对象</returns>
		public abstract ActorBase GetActor();        
        /// <summary>
        /// 获取场景对象的位置
        /// </summary>
        /// <returns>返回场景对象的位置</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.获取位置", CSUtility.Helper.enCSType.Common, "获取场景对象的位置")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.获取位置", CSUtility.Helper.enCSType.Common, "获取场景对象的位置")]
        public abstract SlimDX.Vector3 GetLocation();
        /// <summary>
        /// 获取场景对象的缩放值
        /// </summary>
        /// <returns>返回场景对象的缩放值</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.获取缩放", CSUtility.Helper.enCSType.Common, "获取场景对象的缩放")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.获取缩放", CSUtility.Helper.enCSType.Common, "获取场景对象的缩放")]
        public abstract SlimDX.Vector3 GetScale();
        /// <summary>
        /// 获取场景对象的旋转值
        /// </summary>
        /// <returns>返回场景对象的旋转值</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.获取旋转", CSUtility.Helper.enCSType.Common, "获取场景对象的旋转")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.获取旋转", CSUtility.Helper.enCSType.Common, "获取场景对象的旋转")]
        public abstract SlimDX.Quaternion GetRotation();
        /// <summary>
        /// 获取场景对象的方向
        /// </summary>
        /// <returns>返回场景对象的方向</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.获取方向", CSUtility.Helper.enCSType.Common, "获取场景对象的方向")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.获取方向", CSUtility.Helper.enCSType.Common, "获取场景对象的方向")]
        public abstract float GetDirection();
        /// <summary>
        /// 设置场景对象的位置矩阵
        /// </summary>
        /// <param name="matrix">位置矩阵</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public abstract bool SetMatrix(ref SlimDX.Matrix matrix);
        /// <summary>
        /// 设置场景对象的位置
        /// </summary>
        /// <param name="loc">场景对象的位置</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置位置", CSUtility.Helper.enCSType.Common, "设置场景对象的位置")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置位置", CSUtility.Helper.enCSType.Common, "设置场景对象的位置")]
        public bool SetLocation(SlimDX.Vector3 loc)
        {
            return SetLocation(ref loc);
        }
        /// <summary>
        /// 设置场景对象的位置
        /// </summary>
        /// <param name="loc">场景对象的位置</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public abstract bool SetLocation(ref SlimDX.Vector3 loc);
        /// <summary>
        /// 设置场景对象的缩放
        /// </summary>
        /// <param name="scale">场景对象的缩放</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置缩放", CSUtility.Helper.enCSType.Common, "设置场景对象的缩放")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置缩放", CSUtility.Helper.enCSType.Common, "设置场景对象的缩放")]
        public bool SetScale(SlimDX.Vector3 scale)
        {
            return SetScale(ref scale);
        }
        /// <summary>
        /// 设置场景对象的缩放
        /// </summary>
        /// <param name="scale">场景对象的缩放</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public abstract bool SetScale(ref SlimDX.Vector3 scale);
        /// <summary>
        /// 设置场景对象的旋转
        /// </summary>
        /// <param name="quat">场景对象的旋转四元数</param>
        /// <param name="Imm">是否立即生效，缺省为false</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的旋转")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的旋转")]
        public bool SetRotation(SlimDX.Quaternion quat, bool Imm = false)
        {
            return SetRotation(ref quat, Imm);
        }
        /// <summary>
        /// 设置场景对象的旋转
        /// </summary>
        /// <param name="quat">场景对象的旋转四元数</param>
        /// <param name="bImm">是否立即生效，缺省为false</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public abstract bool SetRotation(ref SlimDX.Quaternion quat, bool bImm = false);
        /// <summary>
        /// 设置场景对象的Y轴旋转量
        /// </summary>
        /// <param name="z">Z轴的旋转方向</param>
        /// <param name="x">X轴的旋转方向</param>
        /// <param name="fixAngle">修正角度</param>
        /// <param name="bImm">是否立即生效，缺省为false</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置Y轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Y轴旋转量, x,z为旋转方向, fixAngle为修正角度,与最终计算出的角度进行叠加")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置Y轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Y轴旋转量, x,z为旋转方向, fixAngle为修正角度,与最终计算出的角度进行叠加")]
        public abstract bool SetRotationY(float z, float x, float fixAngle, bool bImm = false);
        /// <summary>
        /// 设置场景对象的Y轴旋转量
        /// </summary>
        /// <param name="z">Z轴的旋转方向</param>
        /// <param name="x">X轴的旋转方向</param>
        /// <param name="bImm">是否立即生效，缺省为false</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置Y轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Y轴旋转量, fixAngle为修正角度,与最终计算出的角度进行叠加")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置Y轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Y轴旋转量, fixAngle为修正角度,与最终计算出的角度进行叠加")]
        public abstract bool SetRotationY(float z, float x, bool bImm = false);
        /// <summary>
        /// 设置场景对象的Y轴旋转量
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <param name="fixAngle">修正角度</param>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置Y轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Y轴旋转量, 修正角度在函数里实现,与最终计算出的角度进行叠加（要求重载）")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置Y轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Y轴旋转量, 修正角度在函数里实现,与最终计算出的角度进行叠加（要求重载）")]
        public abstract void SetRotationY(float angle, float fixAngle);
        /// <summary>
        /// 设置场景对象的Y轴旋转量
        /// </summary>
        /// <param name="angle">旋转角度</param>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置Y轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Y轴旋转量, 修正角度在函数里实现,与最终计算出的角度进行叠加（要求重载）")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置Y轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Y轴旋转量, 修正角度在函数里实现,与最终计算出的角度进行叠加（要求重载）")]
        public abstract void SetRotationY(float angle);
        /// <summary>
        /// 设置场景对象的X轴旋转量
        /// </summary>
        /// <param name="z">Z轴的旋转方向</param>
        /// <param name="y">Y轴的旋转方向</param>
        /// <param name="bImm">是否立即生效，缺省为false</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置X轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的X轴旋转量, 修正角度在函数里实现,与最终计算出的角度进行叠加（要求重载）")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置X轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的X轴旋转量, 修正角度在函数里实现,与最终计算出的角度进行叠加（要求重载）")]
        public abstract bool SetRotationX(float z, float y, bool bImm = false);
        /// <summary>
        /// 设置场景对象的Z轴旋转量
        /// </summary>
        /// <param name="y">Y轴的旋转方向</param>
        /// <param name="x">X轴的旋转方向</param>
        /// <param name="bImm">是否立即生效，缺省为false</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.设置Z轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Z轴旋转量, 修正角度在函数里实现,与最终计算出的角度进行叠加（要求重载）")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.设置Z轴旋转", CSUtility.Helper.enCSType.Common, "设置场景对象的Z轴旋转量, 修正角度在函数里实现,与最终计算出的角度进行叠加（要求重载）")]
        public abstract bool SetRotationZ(float y, float x, bool bImm = false);
        /// <summary>
        /// 获取场景对象的Y轴旋转量
        /// </summary>
        /// <returns>返回场景对象的Y轴旋转量</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.获取Y轴旋转", CSUtility.Helper.enCSType.Common, "获取场景对象的Y轴旋转量")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.获取Y轴旋转", CSUtility.Helper.enCSType.Common, "获取场景对象的Y轴旋转量")]
        public abstract float GetRotationY();
        /// <summary>
        /// 获取场景对象的Y轴旋转量
        /// </summary>
        /// <returns>返回场景对象的Y轴旋转量</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.获取Y轴旋转", CSUtility.Helper.enCSType.Common, "获取场景对象的Y轴旋转量")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.获取Y轴旋转", CSUtility.Helper.enCSType.Common, "获取场景对象的Y轴旋转量")]

        public abstract SlimDX.Vector2 GetRotationYVec();
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="dir">移动方向</param>
        /// <param name="delta">移动的距离</param>
        /// <returns>没有碰撞返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.移动", CSUtility.Helper.enCSType.Common, "向指定方向移动一定步长，内部进行碰撞检测等处理")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.移动", CSUtility.Helper.enCSType.Common, "向指定方向移动一定步长，内部进行碰撞检测等处理")]
        public abstract bool Move(ref SlimDX.Vector3 dir, float delta);//游戏逻辑，应该尽量调用这个Move函数，保证碰撞检测
        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="pos">指定的位置坐标</param>
        /// <param name="dir">移动的方向</param>
        /// <returns>没有碰撞返回true，否则返回false</returns>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.移动到", CSUtility.Helper.enCSType.Common, "移动到指定位置")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.移动到", CSUtility.Helper.enCSType.Common, "移动到指定位置")]
        public abstract bool MoveTo(ref SlimDX.Vector3 pos, ref SlimDX.Vector3 dir);
        /// <summary>
        /// 位置改变时调用的方法
        /// </summary>
        /// <param name="loc">位置坐标</param>
        public void _OnPlacementLocationChanged(ref SlimDX.Vector3 loc)
        {
            OnLocationChanged?.Invoke(ref loc);
        }
        /// <summary>
        /// 旋转时调用的方法
        /// </summary>
        /// <param name="quat">旋转四元数</param>
        public void _OnPlacementRotationChanged(ref SlimDX.Quaternion quat)
        {
            OnRotationChanged?.Invoke(ref quat);
        }
        /// <summary>
        /// 缩放时调用的方法
        /// </summary>
        /// <param name="scale">缩放值</param>
        public void _OnPlacementScaleChanged(ref SlimDX.Vector3 scale)
        {
            OnScaleChanged?.Invoke(ref scale);
        }

        /// <summary>
        /// 获取对象的绝对位置矩阵
        /// </summary>
        /// <param name="matrix">对象的位置矩阵</param>
        /// <returns>获取成功返回true，否则返回false</returns>
        public virtual bool GetAbsMatrix(out SlimDX.Matrix matrix)
        {
            matrix = SlimDX.Matrix.Identity;
            return false;
        }
    }
    /// <summary>
    /// 标准位置类
    /// </summary>
    public class StandardPlacement : IPlacement
	{
        /// <summary>
        /// 主Actor对象
        /// </summary>
	    protected ActorBase mHostActor;
        /// <summary>
        /// 最小值
        /// </summary>
	    public static float mEpsilon = 0.05f;
        /// <summary>
        /// 位置矩阵
        /// </summary>
	    public 	SlimDX.Matrix		mMatrix;
        /// <summary>
        /// 位置坐标
        /// </summary>
	    public 	SlimDX.Vector3		mLocation;
        /// <summary>
        /// 缩放值
        /// </summary>
	    public 	SlimDX.Vector3		mScale;
        /// <summary>
        /// 旋转四元数
        /// </summary>
	    public 	SlimDX.Quaternion	mQuanternion;
        /// <summary>
        /// 修正角度
        /// </summary>
        public float mFixAngle = 0;
        /// <summary>
        /// 矩阵转换到LSQ
        /// </summary>
        public void Matrix2LSQ()
        {
            SlimDX.Vector3 scale, loc;
            SlimDX.Quaternion quat;

            if (mMatrix.Decompose(out scale, out quat, out loc))
            {
                if (mScale != scale)
                {
                    _OnPlacementScaleChanged(ref scale);
                }
                if (mLocation != loc)
                {
                    _OnPlacementLocationChanged(ref loc);
                }
                if (mQuanternion != quat)
                {
                    _OnPlacementRotationChanged(ref quat);
                }

                mScale = scale;
                mQuanternion = quat;
                mLocation = loc;

                //ASSERT(FALSE);
                LSQ2Matrix();
            }
        }
        /// <summary>
        /// LSQ转换到矩阵
        /// </summary>
        public virtual void LSQ2Matrix()
        {
            SlimDX.Matrix.Transformation( mScale , mQuanternion , mLocation , out mMatrix );

            //mDirection = Quanternion.GetAngleWithAxis(SlimDX.Vector3.UnitY);
            float yaw = 0, pitch = 0, roll = 0;
            Quanternion.GetYawPitchRoll(out yaw, out pitch, out roll);
            mDirection = yaw;

            if (mHostActor!=null)
                mHostActor.OnPlacementChanged(this);
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="host">主Actor对象</param>
        public StandardPlacement(ActorBase host)
        {
            mHostActor = host;
		    mLocation = SlimDX.Vector3.Zero;
		    mScale.X = mScale.Y = mScale.Z = 1.0f;
		    mQuanternion = SlimDX.Quaternion.Identity;

		    LSQ2Matrix();
        }
        /// <summary>
        /// 只读属性，修正角度
        /// </summary>
        public float FixAngle
        {
            get { return mFixAngle; }
        }
        /// <summary>
        /// 只读属性，位置坐标
        /// </summary>
        public SlimDX.Vector3 Location
        {
            get{ return mLocation; }
        }
        /// <summary>
        /// 只读属性，缩放值
        /// </summary>
        public SlimDX.Vector3 Scale
        {
            get{ return mScale; }
        }
        /// <summary>
        /// 只读属性，旋转四元数
        /// </summary>
        public SlimDX.Quaternion Quanternion
        {
            get{ return mQuanternion; }
        }
        float mDirection;
        /// <summary>
        /// 只读属性，对象的方向
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("场景对象.函数.方向", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("场景对象.函数.方向", CSUtility.Helper.enCSType.Common, "")]
        public float Direction
        {
            get { return mDirection; }
        }
        /// <summary>
        /// 获取Actor对象
        /// </summary>
        /// <returns>返回Actor对象</returns>
        public override ActorBase GetActor()
        {
            return mHostActor;
        }
        /// <summary>
        /// 获取位置坐标
        /// </summary>
        /// <returns>返回位置坐标</returns>
		public override SlimDX.Vector3 GetLocation()
        {
            return mLocation;
        }
        /// <summary>
        /// 获取对象缩放值
        /// </summary>
        /// <returns>返回对象的缩放值</returns>
		public override SlimDX.Vector3 GetScale()
        {
            return mScale;
        }
        /// <summary>
        /// 获取对象的旋转四元数
        /// </summary>
        /// <returns>返回对象的旋转四元数</returns>
		public override SlimDX.Quaternion GetRotation()
        {
            return mQuanternion;
        }
        /// <summary>
        /// 获取对象的方向
        /// </summary>
        /// <returns>返回对象的方向</returns>
        public override float GetDirection()
        {
            return mDirection;
        }
        /// <summary>
        /// 设置对象的矩阵
        /// </summary>
        /// <param name="matrix">位置矩阵</param>
        /// <returns>设置成功返回true，否则返回false</returns>
		public override bool SetMatrix( ref SlimDX.Matrix matrix )
        {
            mMatrix = matrix;
            Matrix2LSQ();
            return true;
        }

        /// <summary>
        /// 设置对象的位置
        /// </summary>
        /// <param name="loc">对象的位置坐标</param>
        /// <returns>返回对象的位置坐标</returns>
		public override bool SetLocation( ref SlimDX.Vector3 loc )
        {
            mLocation = loc;

            LSQ2Matrix();

            _OnPlacementLocationChanged(ref loc);

            return true;
        }
        /// <summary>
        /// 设置对象的缩放值
        /// </summary>
        /// <param name="scale">对象的缩放值</param>
        /// <returns>设置成功返回true，否则返回false</returns>
		public override bool SetScale( ref SlimDX.Vector3 scale )
        {
            mScale = scale;
            LSQ2Matrix();

            _OnPlacementScaleChanged(ref scale);

            return true;
        }
        /// <summary>
        /// 设置对象旋转
        /// </summary>
        /// <param name="quat">旋转四元数</param>
        /// <param name="bImm">是否立即执行</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public override bool SetRotation(ref SlimDX.Quaternion quat, bool bImm=false)
        {
            mQuanternion = quat;
            LSQ2Matrix();

            _OnPlacementRotationChanged(ref quat);

            return true;
        }
        /// <summary>
        /// 获取场景对象的沿Y轴旋转量
        /// </summary>
        /// <returns>返回旋转量</returns>
        public override SlimDX.Vector2 GetRotationYVec()
        {
            var angle = -((float)mRotationYAngle);
       //     var rata = -angle * 180 / System.Math.PI;
            var y = System.Math.Sin(angle);
            var x = System.Math.Cos(angle);
            SlimDX.Vector2 rotate = new SlimDX.Vector2((float)x,(float)y);
            return rotate;
        }
        /// <summary>
        /// 设置场景对象的Y轴旋转量
        /// </summary>
        /// <param name="z">Z轴的旋转方向</param>
        /// <param name="x">X轴的旋转方向</param>
        /// <param name="fixAngle">修正角度</param>
        /// <param name="bImm">是否立即生效</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public override bool SetRotationY(float z, float x, float fixAngle, bool bImm=false)
        {
            SlimDX.Vector2 dir = new SlimDX.Vector2(x,z);
            mFixAngle = fixAngle;
            dir.Normalize();
            mRotationYAngle = -(float)System.Math.Atan2(dir.Y, dir.X);//这是一个右手法则函数所以要取反
            OnUpdateRoationY(mRotationYAngle);
            var quat = SlimDX.Quaternion.RotationYawPitchRoll((float)mRotationYAngle + FixAngle, mRotationXAngle, mRotationZAngle);
            return SetRotation(ref quat, bImm);
        }
        /// <summary>
        /// 设置场景对象的Z轴旋转量
        /// </summary>
        /// <param name="y">Y轴的旋转方向</param>
        /// <param name="x">X轴的旋转方向</param>
        /// <param name="bImm">是否立即生效</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public override bool SetRotationZ(float y, float x, bool bImm = false)
        {
            SlimDX.Vector2 dir = new SlimDX.Vector2(x, y);
            dir.Normalize();
            mRotationZAngle = (float)System.Math.Atan2(dir.Y, dir.X);
            var quat = SlimDX.Quaternion.RotationYawPitchRoll((float)mRotationYAngle + FixAngle, mRotationXAngle, mRotationZAngle);
            return SetRotation(ref quat, bImm);
        }
        /// <summary>
        /// 设置场景对象的X轴旋转量
        /// </summary>
        /// <param name="z">Z轴的旋转方向</param>
        /// <param name="y">Y轴的旋转方向</param>
        /// <param name="bImm">是否立即生效</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public override bool SetRotationX(float z, float y, bool bImm = false)
        {
            SlimDX.Vector2 dir = new SlimDX.Vector2(y, z);
            dir.Normalize();
            mRotationXAngle = (float)System.Math.Atan2(dir.Y, dir.X);
            var quat = SlimDX.Quaternion.RotationYawPitchRoll((float)mRotationYAngle + FixAngle, mRotationXAngle, mRotationZAngle);
            return SetRotation(ref quat, bImm);
        }
        /// <summary>
        /// 设置场景对象的Y轴旋转量
        /// </summary>
        /// <param name="z">Z轴的旋转方向</param>
        /// <param name="x">X轴的旋转方向</param>
        /// <param name="bImm">是否立即生效</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public override bool SetRotationY(float z, float x, bool bImm = false)
        {
            SlimDX.Vector2 dir = new SlimDX.Vector2(x, z);
            dir.Normalize();
            mRotationYAngle = -(float)System.Math.Atan2(dir.Y, dir.X);//这是一个右手法则函数所以要取反
            OnUpdateRoationY(mRotationYAngle);
            var quat = SlimDX.Quaternion.RotationYawPitchRoll((float)mRotationYAngle + FixAngle, mRotationXAngle, mRotationZAngle);
            return SetRotation(ref quat, bImm);
        }

        float mRotationXAngle;
        float mRotationYAngle;
        float mRotationZAngle;
        /// <summary>
        /// 设置场景对象的Y轴旋转量
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <param name="fixAngle">修正角度</param>
        public override void SetRotationY(float angle, float fixAngle)
        {
            mFixAngle = fixAngle;
            OnUpdateRoationY(angle);
            var quat = SlimDX.Quaternion.RotationYawPitchRoll((float)mRotationYAngle + FixAngle, mRotationXAngle, mRotationZAngle);
            SetRotation(ref quat, true);
        }
        /// <summary>
        /// 设置场景对象的Y轴旋转量
        /// </summary>
        /// <param name="angle">旋转角度</param>
        public override void SetRotationY(float angle)
        {
            OnUpdateRoationY(angle);
            var quat = SlimDX.Quaternion.RotationYawPitchRoll((float)mRotationYAngle + FixAngle, mRotationXAngle, mRotationZAngle);
            SetRotation(ref quat, true);
        }
        /// <summary>
        /// 获取场景对象的Y轴旋转量
        /// </summary>
        /// <returns>返回场景对象的Y轴旋转量</returns>
        public override float GetRotationY()
        {
            return mRotationYAngle;
        }
        /// <summary>
        /// 更新沿Y轴的旋转量
        /// </summary>
        /// <param name="angle">旋转角度</param>
        protected virtual void OnUpdateRoationY(float angle)
        {
            mRotationYAngle = angle;
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="dir">移动方向</param>
        /// <param name="delta">移动的距离</param>
        /// <returns>没有碰撞返回true，否则返回false</returns>
		public override bool Move(ref SlimDX.Vector3 dir , float delta)
        {
            ActorBase actor = GetActor();
            if (actor == null)
                return false;
            IShapeCylinder cylinder = mHostActor.Shape as IShapeCylinder;

            float fHalfHeight = 1;
            float fRadius = 0.5F;
            if (cylinder != null)
            {
                fHalfHeight = cylinder.HalfHeight;
                fRadius = cylinder.Radius;
            }

            var result = new CSUtility.Support.stHitResult();
            result.InitObject();
            SlimDX.Vector3 start = mLocation;// + dir*(fRadius+mEpsilon);
		    start.Y += fHalfHeight;//角色的位置在脚底，水平移动检测要提高到碰撞半高来处理.

		    SlimDX.Vector3 end;
		    end = start+dir*(delta+fRadius+mEpsilon);
		    //result.mHitFlags |= (uint)MidLayer::enHitFlag::HitMeshTriangle;

		    SlimDX.Vector3 newLoc;
            if (actor.WorldLineCheck(ref start, ref end, ref result))
		    {
                bool bRaiseWhenCross = mHostActor.OnCollideScene(ref start, ref end, result);
                if (bRaiseWhenCross)
                {
                    newLoc = result.mHitPosition - dir * (fRadius + mEpsilon);                    
                }
                else
                {
                    newLoc = start + dir * delta;
                    newLoc.Y -= fHalfHeight;
                    
                    SetLocation(ref newLoc);
                    return true;
                }
		    }
		    else
		    {
                newLoc = start+dir*delta;
		    }

            end.X = newLoc.X;
            end.Z = newLoc.Z;
            end.Y = newLoc.Y - fHalfHeight*1.2F - mEpsilon;

            result = new CSUtility.Support.stHitResult();
            result.InitObject();
            if (actor.WorldLineCheck(ref newLoc, ref end, ref result))
            //if (actor.WorldLineCheck(ref start, ref newLoc, out result))
		    {
			    //新旧位置之间有碰撞，说明穿过地板了
                newLoc.X = result.mHitPosition.X;
                newLoc.Z = result.mHitPosition.Z;
                newLoc.Y = result.mHitPosition.Y + mEpsilon;
		    }
            else
            {
                newLoc.Y -= fHalfHeight;
            }

            //下面是临时代码测试用
            //{
            //    if (mHostActor.WorldLineCheck(ref mLocation, ref newLoc, out result))
            //    {
            //        System.Diagnostics.Debugger.Break();
            //    }
            //}

            SetLocation(ref newLoc);
		    return true;
        }
        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="pos">指定的位置坐标</param>
        /// <param name="dir">移动的方向</param>
        /// <returns>没有碰撞返回true，否则返回false</returns>
        public override bool MoveTo(ref SlimDX.Vector3 pos,ref SlimDX.Vector3 dir)
        {
            ActorBase actor = GetActor();
            if (actor == null)
                return false;
            IShapeCylinder cylinder = mHostActor.Shape as IShapeCylinder;

            float fHalfHeight = 1;
            float fRadius = 0.5F;
            if (cylinder != null)
            {
                fHalfHeight = cylinder.HalfHeight;
                fRadius = cylinder.Radius;
            }

            var result = new CSUtility.Support.stHitResult();
            result.InitObject();
            SlimDX.Vector3 start = mLocation/* + dir*(fRadius+mEpsilon)*/;
            start.Y += fHalfHeight;//角色的位置在脚底，水平移动检测要提高到碰撞半高来处理.

            SlimDX.Vector3 end;
            end = pos;
            end.Y += fHalfHeight;
            //result.mHitFlags |= (uint)MidLayer::enHitFlag::HitMeshTriangle;

            SlimDX.Vector3 newLoc;
            if (actor.WorldLineCheck(ref start, ref end, ref result))
            {
                bool bRaiseWhenCross = mHostActor.OnCollideScene(ref start, ref end, result);
                if (bRaiseWhenCross)
                {
                    newLoc = result.mHitPosition - dir * (fRadius + mEpsilon);
                }
                else
                {
                    newLoc = pos;

                    SetLocation(ref newLoc);
                    return true;
                }
            }
            else
            {
                newLoc = pos;
                newLoc.Y += fHalfHeight;
            }

            end.X = newLoc.X;
            end.Z = newLoc.Z;
            end.Y = newLoc.Y - fHalfHeight*1.2F - mEpsilon;

            result = new CSUtility.Support.stHitResult();
            result.InitObject();
            if (actor.WorldLineCheck(ref newLoc, ref end, ref result))
            //if (actor.WorldLineCheck(ref start, ref newLoc, out result))
            {
                //新旧位置之间有碰撞，说明穿过地板了
                newLoc.X = result.mHitPosition.X;
                newLoc.Z = result.mHitPosition.Z;
                newLoc.Y = result.mHitPosition.Y + mEpsilon;
            }
            else
            {
                newLoc.Y -= (fHalfHeight-mEpsilon);
            }

            //下面是临时代码测试用
            //{
            //    if (mHostActor.WorldLineCheck(ref mLocation, ref newLoc, out result))
            //    {
            //        System.Diagnostics.Debugger.Break();
            //    }
            //}

            SetLocation(ref newLoc);
            return true;
        }

        /// <summary>
        /// 获取对象的绝对位置矩阵
        /// </summary>
        /// <param name="matrix">对象的位置矩阵</param>
        /// <returns>获取成功返回true，否则返回false</returns>
        public override bool GetAbsMatrix(out SlimDX.Matrix matrix)
        {
            ActorBase host = GetActor();
            if (host == null)
            {
                matrix = SlimDX.Matrix.Identity;
                return false;
            }

            if (host.Parent != null && host.Parent.Placement != null)
            {
                SlimDX.Matrix parentMatrix;
                if (false == host.Parent.Placement.GetAbsMatrix(out parentMatrix))
                {
                    matrix = SlimDX.Matrix.Identity;
                    return false;
                }

                matrix = mMatrix * parentMatrix;
            }
            else
            {
                matrix = mMatrix;
            }
            return true;
        }
    };
    /// <summary>
    /// 绑定追踪类
    /// </summary>
    public class RigidTracker
    {
        bool mIsAbsTracker = false;
        Int64 mMillionSecondTime = 0;
        SlimDX.Matrix mInvLastMatrix;
        /// <summary>
        /// 设置追踪对象
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="trackerId">追踪者ID</param>
        /// <param name="duration">持续时长</param>
        /// <param name="isAbsTracker">是否绝对追踪</param>
        public void SetTracker(Guid mapId, Guid trackerId, Int64 duration, bool isAbsTracker)
        {
            Duration = duration;
            mIsAbsTracker = isAbsTracker;
            mScenePoints = CSUtility.Map.ScenePointGroupManager.Instance.FindGroup(mapId, trackerId);
            var pos = mScenePoints.GetPosition(1);
            var rot = mScenePoints.GetRotation(1);
            mInvLastMatrix = SlimDX.Matrix.RotationQuaternion(rot);
            mInvLastMatrix.M41 = pos.X;
            mInvLastMatrix.M42 = pos.Y;
            mInvLastMatrix.M43 = pos.Z;
            mInvLastMatrix.Invert();

            mMillionSecondTime = 0;
        }
        CSUtility.Map.ScenePointGroup mScenePoints = null;
        Int64 mDuration;
        /// <summary>
        /// 持续时长
        /// </summary>
        public Int64 Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }
        /// <summary>
        /// 获取对象的矩阵
        /// </summary>
        /// <param name="millionSecondTime">时间</param>
        /// <returns>返回对象的位置矩阵</returns>
        public virtual SlimDX.Matrix GetMatrix(Int64 millionSecondTime)
        {
            if (mScenePoints == null)
                return SlimDX.Matrix.Identity;

            float percent = ((float)millionSecondTime)/((float)mDuration);
            var pos = mScenePoints.GetPosition(percent);
            var rot = mScenePoints.GetRotation(percent);
            var mat = SlimDX.Matrix.RotationQuaternion(rot);
            mat.M41 = pos.X;
            mat.M42 = pos.Y;
            mat.M43 = pos.Z;

            if (mIsAbsTracker)
                return mat;
            
            mat = mat * mInvLastMatrix;
            return mat;
        }
        /// <summary>
        /// 剩余时间
        /// </summary>
        /// <param name="elapsedMillionSecondTime">消耗的时间</param>
        /// <param name="oMatrix">输出的位置矩阵</param>
        /// <returns>返回剩余的时间</returns>
        public virtual Int64 RemainTime(Int64 elapsedMillionSecondTime, out SlimDX.Matrix oMatrix)
        {
            mMillionSecondTime += elapsedMillionSecondTime;
            if (mMillionSecondTime > Duration)
            {
                oMatrix = SlimDX.Matrix.Identity;
                return 0;
            }
            oMatrix = GetMatrix(mMillionSecondTime);
            return Duration - mMillionSecondTime;
        }
    }
    /// <summary>
    /// 追踪者位置类
    /// </summary>
    public class TrackerPlacement : StandardPlacement
    {
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="host">主Actor对象</param>
        public TrackerPlacement(ActorBase host) : base(host)
        {
        }
        RigidTracker mTracker = null;
        /// <summary>
        /// 追踪对象
        /// </summary>
        public RigidTracker Tracker
        {
            get { return mTracker; }
            set { mTracker = value; }
        }

        SlimDX.Matrix mCurTracker = SlimDX.Matrix.Identity;
        /// <summary>
        /// 获取对象位置矩阵
        /// </summary>
        /// <param name="matrix">位置矩阵</param>
        /// <returns>获取成功返回true，否则返回false</returns>
        public bool GetAfterTrackerMatrix(out SlimDX.Matrix matrix)
        {
            if (mTracker == null)
            {
                matrix = mMatrix;
            }
            else
            {
                matrix = mCurTracker * mMatrix;
            }
            return true;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="host">主Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public override void Tick(ActorBase host, long elapsedMillisecond)
        {
            //return;

            base.Tick(host, elapsedMillisecond);
            UpdateTracker(host.GetStateHost(), elapsedMillisecond);
        }
        /// <summary>
        /// 更新追踪对象
        /// </summary>
        /// <param name="host">主状态</param>
        /// <param name="elapsedMillionSecondTime">经过的时间</param>
        public void UpdateTracker( CSUtility.AISystem.IStateHost host, Int64 elapsedMillionSecondTime)
        {
            if (host == null)
                return;

            if (mTracker == null)
                return;

            var remainTime = mTracker.RemainTime(elapsedMillionSecondTime, out mCurTracker);
            if (host.CurrentState != null)
            {
                host.CurrentState.OnUpdateRigidTracker(remainTime);
            }
        }
    }

}
