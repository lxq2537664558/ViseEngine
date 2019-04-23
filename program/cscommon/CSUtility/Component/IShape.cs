namespace CSUtility.Component
{
    /// <summary>
    /// 组件阴影类
    /// </summary>
    public class IShape : IComponent
    {
        /// <summary>
        /// 连线检测
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="length">三维长度</param>
        /// <param name="matrix">对象的位置矩阵</param>
        /// <param name="result">点击结果</param>
        /// <returns>检测成功返回true，否则返回false</returns>
        public virtual bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 length, ref SlimDX.Matrix matrix, ref CSUtility.Support.stHitResult result)
        {
            return false; 
        }
    }
    /// <summary>
    /// 圆柱形状的阴影类
    /// </summary>
    public class IShapeCylinder : IShape
	{
        /// <summary>
        /// 中心高度
        /// </summary>
	    protected float	mHalfHeight = 2.0f;
        /// <summary>
        /// 最大高度
        /// </summary>
        protected float mCamHeight = 2.0f;
        /// <summary>
        /// 圆柱的半径
        /// </summary>
	    protected float	mRadius = 0.25f;
        /// <summary>
        /// 构造函数
        /// </summary>
        public IShapeCylinder()
        {

        }
        /// <summary>
        /// 连线检测
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="length">长度向量</param>
        /// <param name="matrix">位置矩阵</param>
        /// <param name="result">点击结果</param>
        /// <returns>检测通过返回true，否则返回false</returns>
        public override bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 length, ref SlimDX.Matrix matrix, ref CSUtility.Support.stHitResult result)
        {
            return false;
        }
        /// <summary>
        /// 中心高度
        /// </summary>
        public float HalfHeight
        {
            get { return mHalfHeight; }
            set { mHalfHeight = value; }
        }
        /// <summary>
        /// 最大高度
        /// </summary>
        public float CamHeight
        {
            get { return mCamHeight; }
            set { mCamHeight = value; }
        }
        /// <summary>
        /// 圆柱半径
        /// </summary>
        public float Radius
        {
            get { return mRadius; }
            set { mRadius = value; }
        }
	}
}
