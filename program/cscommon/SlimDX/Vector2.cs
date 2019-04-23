using System;

using System.Globalization;

namespace SlimDX
{
    /// <summary>
    /// 二维向量结构体
    /// </summary>
    [System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 4 )]
    //[System.ComponentModel.TypeConverter( typeof(SlimDX.Design.Vector2Converter) )]
	public struct Vector2 : System.IEquatable<Vector2>
    {
        /// <summary>
        /// X的值
        /// </summary>
        public float X;
        /// <summary>
        /// Y的值
        /// </summary>
		public float Y;
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="value">值</param>
        public Vector2( float value )
	    {
		    X = value;
		    Y = value;
	    }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="x">X的值</param>
        /// <param name="y">Y的值</param>
        public Vector2(float x, float y)
	    {
		    X = x;
		    Y = y;
	    }
        /// <summary>
        /// 根据索引值设置对象的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>返回设置的对象</returns>
        public float this[int index]
        {
            get
	        {
		        switch( index )
		        {
		        case 0:
			        return X;
		        case 1:
			        return Y;
		        default:
			        throw new ArgumentOutOfRangeException( "index", "Indices for Vector2 run from 0 to 1, inclusive." );
		        }
	        }
            set
	        {
		        switch( index )
		        {
		        case 0:
			        X = value;
			        break;
		        case 1:
			        Y = value;
			        break;
		        default:
			        throw new ArgumentOutOfRangeException( "index", "Indices for Vector2 run from 0 to 1, inclusive." );
		        }
	        }
        }
        /// <summary>
        /// 只读属性，空的二维数组,相当于(0,0)
        /// </summary>
        public static Vector2 Zero { get { return new Vector2(0, 0); } }
        /// <summary>
        /// 只读属性，X轴的单位向量,相当于(1,0)
        /// </summary>
	    public static Vector2 UnitX { get { return new Vector2(1, 0); } }
        /// <summary>
        /// 只读属性，Y轴的单位向量,相当于(0,1)
        /// </summary>
        public static Vector2 UnitY { get{ return new Vector2(0, 1); } }
        /// <summary>
        /// 只读属性，XY斜线的向量，相当于(1,1)
        /// </summary>
        public static Vector2 UnitXY { get { return new Vector2(1, 1); } }
        /// <summary>
        /// 只读属性，对象所占内存大小
        /// </summary>
        public static int SizeInBytes { get{ return System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector2)); } }

        #region Equal Override
        /// <summary>
        /// 转换到string类型的字符串
        /// </summary>
        /// <returns>返回转换的string类型的字符串</returns>
        public override string ToString()
	    {
		    return String.Format( CultureInfo.CurrentCulture, "X:{0} Y:{1}", X.ToString(CultureInfo.CurrentCulture), Y.ToString(CultureInfo.CurrentCulture) );
	    }
        /// <summary>
        /// 获取对象的哈希值
        /// </summary>
        /// <returns>返回对象的哈希值</returns>
	    public override int GetHashCode()
	    {
		    return X.GetHashCode() + Y.GetHashCode();
	    }
        /// <summary>
        /// 判断两个对象是否相同
        /// </summary>
        /// <param name="value">可以转换到Vector2类型的对象</param>
        /// <returns>如果两个对象相同返回true，否则返回false</returns>
	    public override bool Equals( object value )
	    {
		    if( value == null )
			    return false;

		    if( value.GetType() != GetType() )
			    return false;

		    return Equals( (Vector2)( value ) );
	    }
        /// <summary>
        /// 判断两个对象是否相同
        /// </summary>
        /// <param name="value">二维向量对象</param>
        /// <returns>如果两个对象相同返回true，否则返回false</returns>
	    public bool Equals( Vector2 value )
	    {
		    return ( X == value.X && Y == value.Y );
	    }
        /// <summary>
        /// 判断两个对象是否相同
        /// </summary>
        /// <param name="value1">二维向量对象</param>
        /// <param name="value2">二维向量对象</param>
        /// <returns>如果两个对象相同返回true，否则返回false</returns>
	    public static bool Equals( ref Vector2 value1, ref Vector2 value2 )
	    {
		    return ( value1.X == value2.X && value1.Y == value2.Y );
	    }
        #endregion
        /// <summary>
        /// 向量的长度
        /// </summary>
        /// <returns>返回向量的长度</returns>
        public float Length()
	    {
		    return (float)( Math.Sqrt( (X * X) + (Y * Y) ) );
	    }
        /// <summary>
        /// 长度的平方
        /// </summary>
        /// <returns>返回长度的平方</returns>
        public float LengthSquared()
        {
            return (X * X) + (Y * Y);
        }
        /// <summary>
        /// 二维向量的单位化
        /// </summary>
        public void Normalize()
        {
            float length = Length();
		    if( length == 0 )
			    return;
		    float num = 1 / length;
		    X *= num;
		    Y *= num;
	    }
        /// <summary>
        /// 两个向量的和
        /// </summary>
        /// <param name="left">二维向量对象</param>
        /// <param name="right">二维向量对象</param>
        /// <returns>返回两个向量的和</returns>
	    public static Vector2 Add( Vector2 left, Vector2 right )
	    {
		    return new Vector2( left.X + right.X, left.Y + right.Y );
	    }
        /// <summary>
        /// 两个向量的和
        /// </summary>
        /// <param name="left">二维向量对象</param>
        /// <param name="right">二维向量对象</param>
        /// <param name="result">两个向量的和</param>
        public static void Add( ref Vector2 left, ref Vector2 right, out Vector2 result )
	    {
            result = new Vector2(left.X + right.X, left.Y + right.Y);
	    }
        /// <summary>
        /// 两个向量的差
        /// </summary>
        /// <param name="left">二维向量对象</param>
        /// <param name="right">二维向量对象</param>
        /// <returns>返回两个向量的差</returns>
        public static Vector2 Subtract( Vector2 left, Vector2 right )
	    {
            return new Vector2(left.X - right.X, left.Y - right.Y);
	    }
        /// <summary>
        /// 两个向量的差
        /// </summary>
        /// <param name="left">二维向量对象</param>
        /// <param name="right">二维向量对象</param>
        /// <param name="result">两个向量的差</param>
        public static void Subtract( ref Vector2 left, ref Vector2 right, out Vector2 result )
	    {
            result = new Vector2(left.X - right.X, left.Y - right.Y);
	    }
        /// <summary>
        /// 两个向量的积
        /// </summary>
        /// <param name="left">二维向量对象</param>
        /// <param name="right">二维向量对象</param>
        /// <returns>返回两个向量的积</returns>
        public static Vector2 Modulate( Vector2 left, Vector2 right )
	    {
            return new Vector2(left.X * right.X, left.Y * right.Y);
	    }
        /// <summary>
        /// 两个向量的积
        /// </summary>
        /// <param name="left">二维向量对象</param>
        /// <param name="right">二维向量对象</param>
        /// <param name="result">两个向量的积</param>
        public static void Modulate( ref Vector2 left, ref Vector2 right, out Vector2 result )
	    {
            result = new Vector2(left.X * right.X, left.Y * right.Y);
	    }
        /// <summary>
        /// 向量与常数的积
        /// </summary>
        /// <param name="value">二维向量对象</param>
        /// <param name="scale">常数</param>
        /// <returns>返回向量与常数的积</returns>
        public static Vector2 Multiply( Vector2 value, float scale )
	    {
            return new Vector2(value.X * scale, value.Y * scale);
	    }
        /// <summary>
        /// 向量与常数的积
        /// </summary>
        /// <param name="value">二维向量对象</param>
        /// <param name="scale">常数</param>
        /// <param name="result">向量与常数的积</param>
        public static void Multiply( ref Vector2 value, float scale, out Vector2 result )
	    {
            result = new Vector2(value.X * scale, value.Y * scale);
	    }
        /// <summary>
        /// 向量与常数的商
        /// </summary>
        /// <param name="value">二维向量对象</param>
        /// <param name="scale">常数</param>
        /// <returns>返回向量与常数的商</returns>
        public static Vector2 Divide( Vector2 value, float scale )
	    {
            return new Vector2(value.X / scale, value.Y / scale);
	    }
        /// <summary>
        /// 向量与常数的商
        /// </summary>
        /// <param name="value">二维向量对象</param>
        /// <param name="scale">常数</param>
        /// <param name="result">向量与常数的商</param>
	    public static void Divide( ref Vector2 value, float scale, out Vector2 result )
	    {
            result = new Vector2(value.X / scale, value.Y / scale);
	    }
        /// <summary>
        /// 向量取反
        /// </summary>
        /// <param name="value">二维向量对象</param>
        /// <returns>返回向量取反的结果</returns>
	    public static Vector2 Negate( Vector2 value )
	    {
            return new Vector2(-value.X, -value.Y);
	    }
        /// <summary>
        /// 向量取反
        /// </summary>
        /// <param name="value">二维向量对象</param>
        /// <param name="result">向量取反的结果</param>
        public static void Negate( ref Vector2 value, out Vector2 result )
	    {
            result = new Vector2(-value.X, -value.Y);
	    }
        /// <summary>
        /// 求对象的质心
        /// </summary>
        /// <param name="value1">二维向量对象</param>
        /// <param name="value2">二维向量对象</param>
        /// <param name="value3">二维向量对象</param>
        /// <param name="amount1">参数</param>
        /// <param name="amount2">参数</param>
        /// <returns>返回对象的质心坐标</returns>
        public static Vector2 Barycentric( Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2 )
	    {
		    Vector2 vector = new Vector2();
		    vector.X = (value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X));
		    vector.Y = (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y));
		    return vector;
	    }
        /// <summary>
        /// 求对象的质心
        /// </summary>
        /// <param name="value1">二维向量对象</param>
        /// <param name="value2">二维向量对象</param>
        /// <param name="value3">二维向量对象</param>
        /// <param name="amount1">参数</param>
        /// <param name="amount2">参数</param>
        /// <param name="result">对象的质心坐标</param>
        public static void Barycentric( ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, float amount1, float amount2, out Vector2 result )
	    {
		    result = new Vector2((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)),
			    (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)) );
	    }
        /// <summary>
        /// 插值计算
        /// </summary>
        /// <param name="value1">二维坐标点</param>
        /// <param name="value2">二维坐标点</param>
        /// <param name="value3">二维坐标点</param>
        /// <param name="value4">二维坐标点</param>
        /// <param name="amount">插值数据</param>
        /// <returns>返回计算后的坐标点</returns>
        public static Vector2 CatmullRom( Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount )
	    {
            Vector2 vector = new Vector2();
		    float squared = amount * amount;
		    float cubed = amount * squared;

		    vector.X = 0.5f * ((((2.0f * value2.X) + ((-value1.X + value3.X) * amount)) + 
			    (((((2.0f * value1.X) - (5.0f * value2.X)) + (4.0f * value3.X)) - value4.X) * squared)) + 
			    ((((-value1.X + (3.0f * value2.X)) - (3.0f * value3.X)) + value4.X) * cubed));

		    vector.Y = 0.5f * ((((2.0f * value2.Y) + ((-value1.Y + value3.Y) * amount)) + 
			    (((((2.0f * value1.Y) - (5.0f * value2.Y)) + (4.0f * value3.Y)) - value4.Y) * squared)) + 
			    ((((-value1.Y + (3.0f * value2.Y)) - (3.0f * value3.Y)) + value4.Y) * cubed));

		    return vector;
	    }
        /// <summary>
        /// 插值计算
        /// </summary>
        /// <param name="value1">二维坐标点</param>
        /// <param name="value2">二维坐标点</param>
        /// <param name="value3">二维坐标点</param>
        /// <param name="value4">二维坐标点</param>
        /// <param name="amount">插值数据</param>
        /// <param name="result">计算后的坐标点</param>
        public static void CatmullRom( ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, ref Vector2 value4, float amount, out Vector2 result )
	    {
		    float squared = amount * amount;
		    float cubed = amount * squared;
            Vector2 r = new Vector2();

		    r.X = 0.5f * ((((2.0f * value2.X) + ((-value1.X + value3.X) * amount)) + 
			    (((((2.0f * value1.X) - (5.0f * value2.X)) + (4.0f * value3.X)) - value4.X) * squared)) + 
			    ((((-value1.X + (3.0f * value2.X)) - (3.0f * value3.X)) + value4.X) * cubed));

		    r.Y = 0.5f * ((((2.0f * value2.Y) + ((-value1.Y + value3.Y) * amount)) + 
			    (((((2.0f * value1.Y) - (5.0f * value2.Y)) + (4.0f * value3.Y)) - value4.Y) * squared)) + 
			    ((((-value1.Y + (3.0f * value2.Y)) - (3.0f * value3.Y)) + value4.Y) * cubed));

		    result = r;
	    }
        /// <summary>
        /// 载体计算
        /// </summary>
        /// <param name="value">二维坐标点</param>
        /// <param name="min">二维坐标点的最小值</param>
        /// <param name="max">二维坐标点的最大值</param>
        /// <returns>返回计算后的坐标</returns>
        public static Vector2 Clamp( Vector2 value, Vector2 min, Vector2 max )
	    {
		    float x = value.X;
		    x = (x > max.X) ? max.X : x;
		    x = (x < min.X) ? min.X : x;

		    float y = value.Y;
		    y = (y > max.Y) ? max.Y : y;
		    y = (y < min.Y) ? min.Y : y;

		    return new Vector2( x, y );
	    }
        /// <summary>
        /// 载体计算
        /// </summary>
        /// <param name="value">二维坐标点</param>
        /// <param name="min">二维坐标点的最小值</param>
        /// <param name="max">二维坐标点的最大值</param>
        /// <param name="result">计算后的坐标</param>
        public static void Clamp( ref Vector2 value, ref Vector2 min, ref Vector2 max, out Vector2 result )
	    {
		    float x = value.X;
		    x = (x > max.X) ? max.X : x;
		    x = (x < min.X) ? min.X : x;

		    float y = value.Y;
		    y = (y > max.Y) ? max.Y : y;
		    y = (y < min.Y) ? min.Y : y;

		    result = new Vector2( x, y );
	    }
        /// <summary>
        /// 艾米插值计算
        /// </summary>
        /// <param name="value1">二维向量</param>
        /// <param name="tangent1">二维向量的正切点坐标</param>
        /// <param name="value2">二维向量</param>
        /// <param name="tangent2">二维向量的正切点坐标</param>
        /// <param name="amount">插值</param>
        /// <returns>返回计算后的向量</returns>
        public static Vector2 Hermite( Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount )
	    {
            Vector2 vector = new Vector2();
		    float squared = amount * amount;
		    float cubed = amount * squared;
		    float part1 = ((2.0f * cubed) - (3.0f * squared)) + 1.0f;
		    float part2 = (-2.0f * cubed) + (3.0f * squared);
		    float part3 = (cubed - (2.0f * squared)) + amount;
		    float part4 = cubed - squared;

		    vector.X = (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4);
		    vector.Y = (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4);

		    return vector;
	    }
        /// <summary>
        /// 艾米插值计算
        /// </summary>
        /// <param name="value1">二维向量</param>
        /// <param name="tangent1">二维向量的正切点坐标</param>
        /// <param name="value2">二维向量</param>
        /// <param name="tangent2">二维向量的正切点坐标</param>
        /// <param name="amount">插值</param>
        /// <param name="result">计算后的向量</param>
        public static void Hermite( ref Vector2 value1, ref Vector2 tangent1, ref Vector2 value2, ref Vector2 tangent2, float amount, out Vector2 result )
	    {
		    float squared = amount * amount;
		    float cubed = amount * squared;
		    float part1 = ((2.0f * cubed) - (3.0f * squared)) + 1.0f;
		    float part2 = (-2.0f * cubed) + (3.0f * squared);
		    float part3 = (cubed - (2.0f * squared)) + amount;
		    float part4 = cubed - squared;

            Vector2 r = new Vector2();
		    r.X = (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4);
		    r.Y = (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4);

		    result = r;
        }
        /// <summary>
        /// 计算线性插值
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="factor">插值因子</param>
        /// <returns>返回计算后的向量</returns>
        public static Vector2 Lerp(Vector2 start, Vector2 end, float factor)
        {
            Vector2 vector = new Vector2();

            vector.X = start.X + ((end.X - start.X) * factor);
            vector.Y = start.Y + ((end.Y - start.Y) * factor);

            return vector;
	    }
        /// <summary>
        /// 计算线性插值
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="factor">插值因子</param>
        /// <param name="result">计算后的向量</param>
        public static void Lerp( ref Vector2 start, ref Vector2 end, float factor, out Vector2 result )
	    {
		    Vector2 r;
		    r.X = start.X + ((end.X - start.X) * factor);
		    r.Y = start.Y + ((end.Y - start.Y) * factor);

		    result = r;
	    }
	    /// <summary>
        /// 平滑插值计算
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="amount">插值因子</param>
        /// <returns>返回计算后的向量</returns>
	    public static Vector2 SmoothStep( Vector2 start, Vector2 end, float amount )
	    {
            Vector2 vector = new Vector2();

		    amount = (amount > 1.0f) ? 1.0f : ((amount < 0.0f) ? 0.0f : amount);
		    amount = (amount * amount) * (3.0f - (2.0f * amount));

		    vector.X = start.X + ((end.X - start.X) * amount);
		    vector.Y = start.Y + ((end.Y - start.Y) * amount);

		    return vector;
	    }
        /// <summary>
        /// 平滑插值计算
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        /// <param name="amount">插值因子</param>
        /// <param name="result">计算后的向量</param>
        public static void SmoothStep( ref Vector2 start, ref Vector2 end, float amount, out Vector2 result )
	    {
		    amount = (amount > 1.0f) ? 1.0f : ((amount < 0.0f) ? 0.0f : amount);
		    amount = (amount * amount) * (3.0f - (2.0f * amount));

            Vector2 r = new Vector2();
		    r.X = start.X + ((end.X - start.X) * amount);
		    r.Y = start.Y + ((end.Y - start.Y) * amount);

		    result = r;
	    }
        /// <summary>
        /// 计算两点间的距离
        /// </summary>
        /// <param name="value1">坐标点</param>
        /// <param name="value2">坐标点</param>
        /// <returns>返回两点间的距离</returns>
        public static float Distance( Vector2 value1, Vector2 value2 )
	    {
		    float x = value1.X - value2.X;
		    float y = value1.Y - value2.Y;

		    return (float)( Math.Sqrt( (x * x) + (y * y) ) );
	    }
        /// <summary>
        /// 计算两点间的距离的平方
        /// </summary>
        /// <param name="value1">坐标点</param>
        /// <param name="value2">坐标点</param>
        /// <returns>返回两点间的距离的平方</returns>
	    public static float DistanceSquared( Vector2 value1, Vector2 value2 )
	    {
		    float x = value1.X - value2.X;
		    float y = value1.Y - value2.Y;

		    return (x * x) + (y * y);
	    }
        /// <summary>
        /// 向量的点积
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <returns>返回点积值</returns>
        public static float Dot( Vector2 left, Vector2 right )
	    {
		    return (left.X * right.X + left.Y * right.Y);
	    }
	    /// <summary>
        /// 向量的单位化
        /// </summary>
        /// <param name="vector">二维向量</param>
        /// <returns>返回单位化后的向量</returns>
	    public static Vector2 Normalize( Vector2 vector )
	    {
		    vector.Normalize();
		    return vector;
	    }
        /// <summary>
        /// 向量的单位化
        /// </summary>
        /// <param name="vector">二维向量</param>
        /// <param name="result">单位化后的向量</param>
        public static void Normalize( ref Vector2 vector, out Vector2 result )
	    {
		    result = Normalize(vector);
	    }
        /// <summary>
        /// 二维向量的坐标转换运算
        /// </summary>
        /// <param name="vector">二维向量</param>
        /// <param name="transform">转换矩阵</param>
        /// <returns>返回转换后的向量</returns>
        public static Vector4 Transform( Vector2 vector, Matrix transform )
	    {
            Vector4 result = new Vector4();

		    result.X = (vector.X * transform.M11) + (vector.Y * transform.M21) + transform.M41;
		    result.Y = (vector.X * transform.M12) + (vector.Y * transform.M22) + transform.M42;
		    result.Z = (vector.X * transform.M13) + (vector.Y * transform.M23) + transform.M43;
		    result.W = (vector.X * transform.M14) + (vector.Y * transform.M24) + transform.M44;

		    return result;
	    }
        /// <summary>
        /// 二维向量的坐标转换运算
        /// </summary>
        /// <param name="vector">二维向量</param>
        /// <param name="transform">转换矩阵</param>
        /// <param name="result">转换后的向量</param>
        public static void Transform( ref Vector2 vector, ref Matrix transform, out Vector4 result )
	    {
            Vector4 r = new Vector4();
		    r.X = (vector.X * transform.M11) + (vector.Y * transform.M21) + transform.M41;
		    r.Y = (vector.X * transform.M12) + (vector.Y * transform.M22) + transform.M42;
		    r.Z = (vector.X * transform.M13) + (vector.Y * transform.M23) + transform.M43;
		    r.W = (vector.X * transform.M14) + (vector.Y * transform.M24) + transform.M44;

		    result = r;
	    }
        /// <summary>
        /// 二维向量的坐标转换运算
        /// </summary>
        /// <param name="vectors">二维向量列表</param>
        /// <param name="transform">转换矩阵</param>
        /// <returns>返回转换后的向量列表</returns>
        public static Vector4[] Transform( Vector2[] vectors, ref Matrix transform )
	    {
		    if( vectors == null )
			    throw new ArgumentNullException( "vectors" );

		    int count = vectors.Length;
		    Vector4[] results = new Vector4[ count ];

		    for( int i = 0; i < count; i++ )
		    {
                Vector4 r = new Vector4();
			    r.X = (vectors[i].X * transform.M11) + (vectors[i].Y * transform.M21) + transform.M41;
			    r.Y = (vectors[i].X * transform.M12) + (vectors[i].Y * transform.M22) + transform.M42;
			    r.Z = (vectors[i].X * transform.M13) + (vectors[i].Y * transform.M23) + transform.M43;
			    r.W = (vectors[i].X * transform.M14) + (vectors[i].Y * transform.M24) + transform.M44;

			    results[i] = r;
		    }

		    return results;
	    }
        /// <summary>
        /// 二维向量的坐标转换运算
        /// </summary>
        /// <param name="value">二维向量</param>
        /// <param name="rotation">旋转四元数</param>
        /// <returns>返回转换后的向量</returns>
        public static Vector4 Transform( Vector2 value, Quaternion rotation )
	    {
            Vector4 vector = new Vector4();
		    float x = rotation.X + rotation.X;
		    float y = rotation.Y + rotation.Y;
		    float z = rotation.Z + rotation.Z;
		    float wx = rotation.W * x;
		    float wy = rotation.W * y;
		    float wz = rotation.W * z;
		    float xx = rotation.X * x;
		    float xy = rotation.X * y;
		    float xz = rotation.X * z;
		    float yy = rotation.Y * y;
		    float yz = rotation.Y * z;
		    float zz = rotation.Z * z;

		    vector.X = ((value.X * ((1.0f - yy) - zz)) + (value.Y * (xy - wz)));
		    vector.Y = ((value.X * (xy + wz)) + (value.Y * ((1.0f - xx) - zz)));
		    vector.Z = ((value.X * (xz - wy)) + (value.Y * (yz + wx)));
		    vector.W = 1.0f;

		    return vector;
	    }
        /// <summary>
        /// 二维向量的坐标转换运算
        /// </summary>
        /// <param name="value">二维向量</param>
        /// <param name="rotation">旋转四元数</param>
        /// <param name="result">转换后的向量</param>
        public static void Transform( ref Vector2 value, ref Quaternion rotation, out Vector4 result )
	    {
		    float x = rotation.X + rotation.X;
		    float y = rotation.Y + rotation.Y;
		    float z = rotation.Z + rotation.Z;
		    float wx = rotation.W * x;
		    float wy = rotation.W * y;
		    float wz = rotation.W * z;
		    float xx = rotation.X * x;
		    float xy = rotation.X * y;
		    float xz = rotation.X * z;
		    float yy = rotation.Y * y;
		    float yz = rotation.Y * z;
		    float zz = rotation.Z * z;

            Vector4 r = new Vector4();
		    r.X = ((value.X * ((1.0f - yy) - zz)) + (value.Y * (xy - wz)));
		    r.Y = ((value.X * (xy + wz)) + (value.Y * ((1.0f - xx) - zz)));
		    r.Z = ((value.X * (xz - wy)) + (value.Y * (yz + wx)));
		    r.W = 1.0f;

		    result = r;
	    }
        /// <summary>
        /// 二维向量的坐标转换运算
        /// </summary>
        /// <param name="vectors">二维向量列表</param>
        /// <param name="rotation">旋转四元数</param>
        /// <returns>返回转换后的向量列表</returns>
	    public static Vector4[] Transform( Vector2[] vectors, ref Quaternion rotation )
	    {
		    if( vectors == null )
			    throw new ArgumentNullException( "vectors" );

		    int count = vectors.Length;
		    Vector4[] results = new Vector4[ count ];

		    float x = rotation.X + rotation.X;
		    float y = rotation.Y + rotation.Y;
		    float z = rotation.Z + rotation.Z;
		    float wx = rotation.W * x;
		    float wy = rotation.W * y;
		    float wz = rotation.W * z;
		    float xx = rotation.X * x;
		    float xy = rotation.X * y;
		    float xz = rotation.X * z;
		    float yy = rotation.Y * y;
		    float yz = rotation.Y * z;
		    float zz = rotation.Z * z;

		    for( int i = 0; i < count; i++ )
		    {
                Vector4 r = new Vector4();
			    r.X = ((vectors[i].X * ((1.0f - yy) - zz)) + (vectors[i].Y * (xy - wz)));
			    r.Y = ((vectors[i].X * (xy + wz)) + (vectors[i].Y * ((1.0f - xx) - zz)));
			    r.Z = ((vectors[i].X * (xz - wy)) + (vectors[i].Y * (yz + wx)));
			    r.W = 1.0f;

			    results[i] = r;
		    }

		    return results;
	    }
	    /// <summary>
        /// 坐标轴转换
        /// </summary>
        /// <param name="coord">坐标轴向量</param>
        /// <param name="transform">转换矩阵</param>
        /// <returns>返回转换后的向量</returns>
	    public static Vector2 TransformCoordinate( Vector2 coord, Matrix transform )
	    {
            Vector4 vector = new Vector4();

		    vector.X = (coord.X * transform.M11) + (coord.Y * transform.M21) + transform.M41;
		    vector.Y = (coord.X * transform.M12) + (coord.Y * transform.M22) + transform.M42;
		    vector.Z = (coord.X * transform.M13) + (coord.Y * transform.M23) + transform.M43;
		    vector.W = 1 / ((coord.X * transform.M14) + (coord.Y * transform.M24) + transform.M44);

		    return new Vector2( vector.X * vector.W, vector.Y * vector.W );
	    }
        /// <summary>
        /// 坐标轴转换
        /// </summary>
        /// <param name="coord">坐标轴向量</param>
        /// <param name="transform">转换矩阵</param>
        /// <param name="result">转换后的向量</param>
        public static void TransformCoordinate( ref Vector2 coord, ref Matrix transform, out Vector2 result )
	    {
            Vector4 vector = new Vector4();

		    vector.X = (coord.X * transform.M11) + (coord.Y * transform.M21) + transform.M41;
		    vector.Y = (coord.X * transform.M12) + (coord.Y * transform.M22) + transform.M42;
		    vector.Z = (coord.X * transform.M13) + (coord.Y * transform.M23) + transform.M43;
		    vector.W = 1 / ((coord.X * transform.M14) + (coord.Y * transform.M24) + transform.M44);

		    result = new Vector2( vector.X * vector.W, vector.Y * vector.W );
	    }
        /// <summary>
        /// 坐标轴转换
        /// </summary>
        /// <param name="coords">坐标轴向量列表</param>
        /// <param name="transform">转换矩阵</param>
        /// <returns>返回转换后的向量列表</returns>
        public static Vector2[] TransformCoordinate( Vector2[] coords, ref Matrix transform )
	    {
		    if( coords == null )
			    throw new ArgumentNullException( "coordinates" );

            Vector4 vector = new Vector4();
		    int count = coords.Length;
		    Vector2[] results = new Vector2[ count ];

		    for( int i = 0; i < count; i++ )
		    {
			    vector.X = (coords[i].X * transform.M11) + (coords[i].Y * transform.M21) + transform.M41;
			    vector.Y = (coords[i].X * transform.M12) + (coords[i].Y * transform.M22) + transform.M42;
			    vector.Z = (coords[i].X * transform.M13) + (coords[i].Y * transform.M23) + transform.M43;
			    vector.W = 1 / ((coords[i].X * transform.M14) + (coords[i].Y * transform.M24) + transform.M44);
			    results[i] = new Vector2( vector.X * vector.W, vector.Y * vector.W );
		    }

		    return results;
	    }
        /// <summary>
        /// 单位向量转换
        /// </summary>
        /// <param name="normal">单位向量</param>
        /// <param name="transform">转换矩阵</param>
        /// <returns>返回转换后的向量</returns>
	    public static Vector2 TransformNormal( Vector2 normal, Matrix transform )
	    {
            Vector2 vector = new Vector2();

		    vector.X = (normal.X * transform.M11) + (normal.Y * transform.M21);
		    vector.Y = (normal.X * transform.M12) + (normal.Y * transform.M22);

		    return vector;
	    }
        /// <summary>
        /// 单位向量转换
        /// </summary>
        /// <param name="normal">单位向量</param>
        /// <param name="transform">转换矩阵</param>
        /// <param name="result">转换后的向量</param>
        public static void TransformNormal( ref Vector2 normal, ref Matrix transform, out Vector2 result )
	    {
            Vector2 r = new Vector2();
		    r.X = (normal.X * transform.M11) + (normal.Y * transform.M21);
		    r.Y = (normal.X * transform.M12) + (normal.Y * transform.M22);

		    result = r;
	    }
        /// <summary>
        /// 单位向量转换
        /// </summary>
        /// <param name="normals">单位向量列表</param>
        /// <param name="transform">转换矩阵</param>
        /// <returns>返回转换后的向量列表</returns>
        public static Vector2[] TransformNormal( Vector2[] normals, ref Matrix transform )
	    {
		    if( normals == null )
			    throw new ArgumentNullException( "normals" );

		    int count = normals.Length;
		    Vector2[] results = new Vector2[ count ];

		    for( int i = 0; i < count; i++ )
		    {
                Vector2 r = new Vector2();
			    r.X = (normals[i].X * transform.M11) + (normals[i].Y * transform.M21);
			    r.Y = (normals[i].X * transform.M12) + (normals[i].Y * transform.M22);

			    results[i] = r;
		    }

		    return results;
	    }
        /// <summary>
        /// 向量的最小化
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <returns>返回最小化后的向量</returns>
        public static Vector2 Minimize( Vector2 left, Vector2 right )
	    {
            Vector2 vector = new Vector2();
		    vector.X = (left.X < right.X) ? left.X : right.X;
		    vector.Y = (left.Y < right.Y) ? left.Y : right.Y;
		    return vector;
	    }
        /// <summary>
        /// 向量的最小化
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <param name="result">最小化后的向量</param>
        public static void Minimize( ref Vector2 left, ref Vector2 right, out Vector2 result )
	    {
            Vector2 r = new Vector2();
		    r.X = (left.X < right.X) ? left.X : right.X;
		    r.Y = (left.Y < right.Y) ? left.Y : right.Y;

		    result = r;
	    }
        /// <summary>
        /// 向量的最大化
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <returns>返回最大化后的向量</returns>
        public static Vector2 Maximize( Vector2 left, Vector2 right )
	    {
            Vector2 vector = new Vector2();
		    vector.X = (left.X > right.X) ? left.X : right.X;
		    vector.Y = (left.Y > right.Y) ? left.Y : right.Y;
		    return vector;
	    }
        /// <summary>
        /// 向量的最大化
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <param name="result">最大化后的向量</param>
	    public static void Maximize( ref Vector2 left, ref Vector2 right, out Vector2 result )
	    {
            Vector2 r = new Vector2();
		    r.X = (left.X > right.X) ? left.X : right.X;
		    r.Y = (left.Y > right.Y) ? left.Y : right.Y;

		    result = r;
	    }
        /// <summary>
        /// 重载"+"号运算符
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <returns>返回计算后的向量</returns>
        public static Vector2 operator + ( Vector2 left, Vector2 right )
	    {
		    return new Vector2( left.X + right.X, left.Y + right.Y );
	    }
        /// <summary>
        /// 重载"-"号运算符
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <returns>返回计算后的向量</returns>
	    public static Vector2 operator - ( Vector2 left, Vector2 right )
	    {
		    return new Vector2( left.X - right.X, left.Y - right.Y );
	    }
        /// <summary>
        /// 重载"-"号运算符
        /// </summary>
        /// <param name="value">二维向量</param>
        /// <returns>返回计算后的向量</returns>
        public static Vector2 operator - ( Vector2 value )
	    {
            return new Vector2(-value.X, -value.Y);
	    }
        /// <summary>
        /// 重载"*"号运算符
        /// </summary>
        /// <param name="value">二维向量</param>
        /// <param name="scale">乘数</param>
        /// <returns>返回计算后的向量</returns>
        public static Vector2 operator * ( Vector2 value, float scale )
	    {
            return new Vector2(value.X * scale, value.Y * scale);
	    }
        /// <summary>
        /// 重载"*"号运算符
        /// </summary>
        /// <param name="scale">乘数</param>
        /// <param name="vec">二维向量</param>
        /// <returns>返回计算后的向量</returns>
        public static Vector2 operator * ( float scale, Vector2 vec )
	    {
		    return vec * scale;
	    }
        /// <summary>
        /// 重载"/"号运算符
        /// </summary>
        /// <param name="value">二维向量</param>
        /// <param name="scale">除数</param>
        /// <returns>返回计算后的向量</returns>
        public static Vector2 operator / ( Vector2 value, float scale )
	    {
            return new Vector2(value.X / scale, value.Y / scale);
	    }
        /// <summary>
        /// 重载"=="号运算符
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <returns>如果两个向量相等返回true，否则返回false</returns>
        public static bool operator == ( Vector2 left, Vector2 right )
	    {
		    return Equals( left, right );
	    }
        /// <summary>
        /// 重载"!="号运算符
        /// </summary>
        /// <param name="left">二维向量</param>
        /// <param name="right">二维向量</param>
        /// <returns>如果两个向量不相等返回true，否则返回false</returns>
        public static bool operator != ( Vector2 left, Vector2 right )
	    {
		    return !Equals( left, right );
	    }
    }
    /// <summary>
    /// 尺寸大小类
    /// </summary>
    public struct Size : System.IEquatable<Size>
    {
        /// <summary>
        /// 宽
        /// </summary>
        public float Width;
        /// <summary>
        /// 高
        /// </summary>
        public float Height;

        static Size sEmpty = new Size(0,0);
        /// <summary>
        /// 只读属性，大小为0
        /// </summary>
        public static Size Empty
        {
            get{return sEmpty;}
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        public Size(float w, float h)
	    {
		    Width = w;
		    Height = h;
	    }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        public Size(double w, double h)
        {
            Width = (float)w;
            Height = (float)h;
        }

        #region Equal Override
        /// <summary>
        /// 转换成string类型
        /// </summary>
        /// <returns>返回转换到的string类型字符串</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "Width:{0} Height:{1}", Width.ToString(CultureInfo.CurrentCulture), Height.ToString(CultureInfo.CurrentCulture));
        }
        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>返回对象的哈希值</returns>
        public override int GetHashCode()
        {
            return Width.GetHashCode() + Height.GetHashCode();
        }
        /// <summary>
        /// 判断两个对象是否相等
        /// </summary>
        /// <param name="value">可以转换成Size类型的对象</param>
        /// <returns>如果两个对象相等返回true，否则返回false</returns>
        public override bool Equals(object value)
        {
            if (value == null)
                return false;

            if (value.GetType() != GetType())
                return false;

            return Equals((Size)(value));
        }
        /// <summary>
        /// 判断两个对象是否相等
        /// </summary>
        /// <param name="value">尺寸大小对象</param>
        /// <returns>如果两个对象相等返回true，否则返回false</returns>
        public bool Equals(Size value)
        {
            return (Width == value.Width && Height == value.Height);
        }
        /// <summary>
        /// 判断两个对象是否相等
        /// </summary>
        /// <param name="value1">尺寸大小对象</param>
        /// <param name="value2">尺寸大小对象</param>
        /// <returns>如果两个对象相等返回true，否则返回false</returns>
        public static bool Equals(ref Size value1, ref Size value2)
        {
            return (value1.Width == value2.Width && value1.Height == value2.Height);
        }
        /// <summary>
        /// 重载"=="号运算符
        /// </summary>
        /// <param name="left">尺寸大小对象</param>
        /// <param name="right">尺寸大小对象</param>
        /// <returns>如果两个对象相等返回true，否则返回false</returns>
        public static bool operator ==(Size left, Size right)
        {
            return Equals(left, right);
        }
        /// <summary>
        /// 重载"!="号运算符
        /// </summary>
        /// <param name="left">尺寸大小对象</param>
        /// <param name="right">尺寸大小对象</param>
        /// <returns>如果两个对象不相等返回true，否则返回false</returns>
        public static bool operator !=(Size left, Size right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    //public struct Rect
    //{
    //    public Rect(Size size)
    //    {

    //    }
    //    //
    //    // Summary:
    //    //     Initializes a new instance of the System.Windows.Rect structure that is exactly
    //    //     large enough to contain the two specified points.
    //    //
    //    // Parameters:
    //    //   point1:
    //    //     The first point that the new rectangle must contain.
    //    //
    //    //   point2:
    //    //     The second point that the new rectangle must contain.
    //    public Rect(Point point1, Point point2);
    //    //
    //    // Summary:
    //    //     Initializes a new instance of the System.Windows.Rect structure that has
    //    //     the specified top-left corner location and the specified width and height.
    //    //
    //    // Parameters:
    //    //   location:
    //    //     A point that specifies the location of the top-left corner of the rectangle.
    //    //
    //    //   size:
    //    //     A System.Windows.Size structure that specifies the width and height of the
    //    //     rectangle.
    //    public Rect(Point location, Size size);
    //    //
    //    // Summary:
    //    //     Initializes a new instance of the System.Windows.Rect structure that is exactly
    //    //     large enough to contain the specified point and the sum of the specified
    //    //     point and the specified vector.
    //    //
    //    // Parameters:
    //    //   point:
    //    //     The first point the rectangle must contain.
    //    //
    //    //   vector:
    //    //     The amount to offset the specified point. The resulting rectangle will be
    //    //     exactly large enough to contain both points.
    //    public Rect(Point point, Vector vector);
    //    //
    //    // Summary:
    //    //     Initializes a new instance of the System.Windows.Rect structure that has
    //    //     the specified x-coordinate, y-coordinate, width, and height.
    //    //
    //    // Parameters:
    //    //   x:
    //    //     The x-coordinate of the top-left corner of the rectangle.
    //    //
    //    //   y:
    //    //     The y-coordinate of the top-left corner of the rectangle.
    //    //
    //    //   width:
    //    //     The width of the rectangle.
    //    //
    //    //   height:
    //    //     The height of the rectangle.
    //    //
    //    // Exceptions:
    //    //   System.ArgumentException:
    //    //     width is a negative value.-or-height is a negative value.
    //    public Rect(double x, double y, double width, double height);

    //    // Summary:
    //    //     Compares two rectangles for inequality.
    //    //
    //    // Parameters:
    //    //   rect1:
    //    //     The first rectangle to compare.
    //    //
    //    //   rect2:
    //    //     The second rectangle to compare.
    //    //
    //    // Returns:
    //    //     true if the rectangles do not have the same System.Windows.Rect.Location
    //    //     and System.Windows.Rect.Size values; otherwise, false.
    //    public static bool operator !=(Rect rect1, Rect rect2);
    //    //
    //    // Summary:
    //    //     Compares two rectangles for exact equality.
    //    //
    //    // Parameters:
    //    //   rect1:
    //    //     The first rectangle to compare.
    //    //
    //    //   rect2:
    //    //     The second rectangle to compare.
    //    //
    //    // Returns:
    //    //     true if the rectangles have the same System.Windows.Rect.Location and System.Windows.Rect.Size
    //    //     values; otherwise, false.
    //    public static bool operator ==(Rect rect1, Rect rect2);

    //    // Summary:
    //    //     Gets the y-axis value of the bottom of the rectangle.
    //    //
    //    // Returns:
    //    //     The y-axis value of the bottom of the rectangle. If the rectangle is empty,
    //    //     the value is System.Double.NegativeInfinity .
    //    public double Bottom { get; }
    //    //
    //    // Summary:
    //    //     Gets the position of the bottom-left corner of the rectangle
    //    //
    //    // Returns:
    //    //     The position of the bottom-left corner of the rectangle.
    //    public Point BottomLeft { get; }
    //    //
    //    // Summary:
    //    //     Gets the position of the bottom-right corner of the rectangle.
    //    //
    //    // Returns:
    //    //     The position of the bottom-right corner of the rectangle.
    //    public Point BottomRight { get; }
    //    //
    //    // Summary:
    //    //     Gets a special value that represents a rectangle with no position or area.
    //    //
    //    // Returns:
    //    //     The empty rectangle, which has System.Windows.Rect.X and System.Windows.Rect.Y
    //    //     property values of System.Double.PositiveInfinity, and has System.Windows.Rect.Width
    //    //     and System.Windows.Rect.Height property values of System.Double.NegativeInfinity.
    //    public static Rect Empty { get; }
    //    //
    //    // Summary:
    //    //     Gets or sets the height of the rectangle.
    //    //
    //    // Returns:
    //    //     A positive number that represents the height of the rectangle. The default
    //    //     is 0.
    //    //
    //    // Exceptions:
    //    //   System.ArgumentException:
    //    //     System.Windows.Rect.Height is set to a negative value.
    //    //
    //    //   System.InvalidOperationException:
    //    //     System.Windows.Rect.Height is set on an System.Windows.Rect.Empty rectangle.
    //    public double Height { get; set; }
    //    //
    //    // Summary:
    //    //     Gets a value that indicates whether the rectangle is the System.Windows.Rect.Empty
    //    //     rectangle.
    //    //
    //    // Returns:
    //    //     true if the rectangle is the System.Windows.Rect.Empty rectangle; otherwise,
    //    //     false.
    //    public bool IsEmpty { get; }
    //    //
    //    // Summary:
    //    //     Gets the x-axis value of the left side of the rectangle.
    //    //
    //    // Returns:
    //    //     The x-axis value of the left side of the rectangle.
    //    public double Left { get; }
    //    //
    //    // Summary:
    //    //     Gets or sets the position of the top-left corner of the rectangle.
    //    //
    //    // Returns:
    //    //     The position of the top-left corner of the rectangle. The default is (0,
    //    //     0).
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     System.Windows.Rect.Location is set on an System.Windows.Rect.Empty rectangle.
    //    public Point Location { get; set; }
    //    //
    //    // Summary:
    //    //     Gets the x-axis value of the right side of the rectangle.
    //    //
    //    // Returns:
    //    //     The x-axis value of the right side of the rectangle.
    //    public double Right { get; }
    //    //
    //    // Summary:
    //    //     Gets or sets the width and height of the rectangle.
    //    //
    //    // Returns:
    //    //     A System.Windows.Size structure that specifies the width and height of the
    //    //     rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     System.Windows.Rect.Size is set on an System.Windows.Rect.Empty rectangle.
    //    public Size Size { get; set; }
    //    //
    //    // Summary:
    //    //     Gets the y-axis position of the top of the rectangle.
    //    //
    //    // Returns:
    //    //     The y-axis position of the top of the rectangle.
    //    public double Top { get; }
    //    //
    //    // Summary:
    //    //     Gets the position of the top-left corner of the rectangle.
    //    //
    //    // Returns:
    //    //     The position of the top-left corner of the rectangle.
    //    public Point TopLeft { get; }
    //    //
    //    // Summary:
    //    //     Gets the position of the top-right corner of the rectangle.
    //    //
    //    // Returns:
    //    //     The position of the top-right corner of the rectangle.
    //    public Point TopRight { get; }
    //    //
    //    // Summary:
    //    //     Gets or sets the width of the rectangle.
    //    //
    //    // Returns:
    //    //     A positive number that represents the width of the rectangle. The default
    //    //     is 0.
    //    //
    //    // Exceptions:
    //    //   System.ArgumentException:
    //    //     System.Windows.Rect.Width is set to a negative value.
    //    //
    //    //   System.InvalidOperationException:
    //    //     System.Windows.Rect.Width is set on an System.Windows.Rect.Empty rectangle.
    //    public double Width { get; set; }
    //    //
    //    // Summary:
    //    //     Gets or sets the x-axis value of the left side of the rectangle.
    //    //
    //    // Returns:
    //    //     The x-axis value of the left side of the rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     System.Windows.Rect.X is set on an System.Windows.Rect.Empty rectangle.
    //    public double X { get; set; }
    //    //
    //    // Summary:
    //    //     Gets or sets the y-axis value of the top side of the rectangle.
    //    //
    //    // Returns:
    //    //     The y-axis value of the top side of the rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     System.Windows.Rect.Y is set on an System.Windows.Rect.Empty rectangle.
    //    public double Y { get; set; }

    //    // Summary:
    //    //     Indicates whether the rectangle contains the specified point.
    //    //
    //    // Parameters:
    //    //   point:
    //    //     The point to check.
    //    //
    //    // Returns:
    //    //     true if the rectangle contains the specified point; otherwise, false.
    //    public bool Contains(Point point);
    //    //
    //    // Summary:
    //    //     Indicates whether the rectangle contains the specified rectangle.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The rectangle to check.
    //    //
    //    // Returns:
    //    //     true if rect is entirely contained by the rectangle; otherwise, false.
    //    public bool Contains(Rect rect);
    //    //
    //    // Summary:
    //    //     Indicates whether the rectangle contains the specified x-coordinate and y-coordinate.
    //    //
    //    // Parameters:
    //    //   x:
    //    //     The x-coordinate of the point to check.
    //    //
    //    //   y:
    //    //     The y-coordinate of the point to check.
    //    //
    //    // Returns:
    //    //     true if (x, y) is contained by the rectangle; otherwise, false.
    //    public bool Contains(double x, double y);
    //    //
    //    // Summary:
    //    //     Indicates whether the specified object is equal to the current rectangle.
    //    //
    //    // Parameters:
    //    //   o:
    //    //     The object to compare to the current rectangle.
    //    //
    //    // Returns:
    //    //     true if o is a System.Windows.Rect and has the same System.Windows.Rect.Location
    //    //     and System.Windows.Rect.Size values as the current rectangle; otherwise,
    //    //     false.
    //    public override bool Equals(object o);
    //    //
    //    // Summary:
    //    //     Indicates whether the specified rectangle is equal to the current rectangle.
    //    //
    //    // Parameters:
    //    //   value:
    //    //     The rectangle to compare to the current rectangle.
    //    //
    //    // Returns:
    //    //     true if the specified rectangle has the same System.Windows.Rect.Location
    //    //     and System.Windows.Rect.Size values as the current rectangle; otherwise,
    //    //     false.
    //    public bool Equals(Rect value);
    //    //
    //    // Summary:
    //    //     Indicates whether the specified rectangles are equal.
    //    //
    //    // Parameters:
    //    //   rect1:
    //    //     The first rectangle to compare.
    //    //
    //    //   rect2:
    //    //     The second rectangle to compare.
    //    //
    //    // Returns:
    //    //     true if the rectangles have the same System.Windows.Rect.Location and System.Windows.Rect.Size
    //    //     values; otherwise, false.
    //    public static bool Equals(Rect rect1, Rect rect2);
    //    //
    //    // Summary:
    //    //     Creates a hash code for the rectangle.
    //    //
    //    // Returns:
    //    //     A hash code for the current System.Windows.Rect structure.
    //    public override int GetHashCode();
    //    //
    //    // Summary:
    //    //     Expands the rectangle by using the specified System.Windows.Size, in all
    //    //     directions.
    //    //
    //    // Parameters:
    //    //   size:
    //    //     Specifies the amount to expand the rectangle. The System.Windows.Size structure's
    //    //     System.Windows.Size.Width property specifies the amount to increase the rectangle's
    //    //     System.Windows.Rect.Left and System.Windows.Rect.Right properties. The System.Windows.Size
    //    //     structure's System.Windows.Size.Height property specifies the amount to increase
    //    //     the rectangle's System.Windows.Rect.Top and System.Windows.Rect.Bottom properties.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     This method is called on the System.Windows.Rect.Empty rectangle.
    //    public void Inflate(Size size);
    //    //
    //    // Summary:
    //    //     Expands or shrinks the rectangle by using the specified width and height
    //    //     amounts, in all directions.
    //    //
    //    // Parameters:
    //    //   width:
    //    //     The amount by which to expand or shrink the left and right sides of the rectangle.
    //    //
    //    //   height:
    //    //     The amount by which to expand or shrink the top and bottom sides of the rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     This method is called on the System.Windows.Rect.Empty rectangle.
    //    public void Inflate(double width, double height);
    //    //
    //    // Summary:
    //    //     Returns the rectangle that results from expanding the specified rectangle
    //    //     by the specified System.Windows.Size, in all directions.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The System.Windows.Rect structure to modify.
    //    //
    //    //   size:
    //    //     Specifies the amount to expand the rectangle. The System.Windows.Size structure's
    //    //     System.Windows.Size.Width property specifies the amount to increase the rectangle's
    //    //     System.Windows.Rect.Left and System.Windows.Rect.Right properties. The System.Windows.Size
    //    //     structure's System.Windows.Size.Height property specifies the amount to increase
    //    //     the rectangle's System.Windows.Rect.Top and System.Windows.Rect.Bottom properties.
    //    //
    //    // Returns:
    //    //     The resulting rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     rect is an System.Windows.Rect.Empty rectangle.
    //    public static Rect Inflate(Rect rect, Size size);
    //    //
    //    // Summary:
    //    //     Creates a rectangle that results from expanding or shrinking the specified
    //    //     rectangle by the specified width and height amounts, in all directions.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The System.Windows.Rect structure to modify.
    //    //
    //    //   width:
    //    //     The amount by which to expand or shrink the left and right sides of the rectangle.
    //    //
    //    //   height:
    //    //     The amount by which to expand or shrink the top and bottom sides of the rectangle.
    //    //
    //    // Returns:
    //    //     The resulting rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     rect is an System.Windows.Rect.Empty rectangle.
    //    public static Rect Inflate(Rect rect, double width, double height);
    //    //
    //    // Summary:
    //    //     Finds the intersection of the current rectangle and the specified rectangle,
    //    //     and stores the result as the current rectangle.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The rectangle to intersect with the current rectangle.
    //    public void Intersect(Rect rect);
    //    //
    //    // Summary:
    //    //     Returns the intersection of the specified rectangles.
    //    //
    //    // Parameters:
    //    //   rect1:
    //    //     The first rectangle to compare.
    //    //
    //    //   rect2:
    //    //     The second rectangle to compare.
    //    //
    //    // Returns:
    //    //     The intersection of the two rectangles, or System.Windows.Rect.Empty if no
    //    //     intersection exists.
    //    public static Rect Intersect(Rect rect1, Rect rect2);
    //    //
    //    // Summary:
    //    //     Indicates whether the specified rectangle intersects with the current rectangle.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The rectangle to check.
    //    //
    //    // Returns:
    //    //     true if the specified rectangle intersects with the current rectangle; otherwise,
    //    //     false.
    //    public bool IntersectsWith(Rect rect);
    //    //
    //    // Summary:
    //    //     Moves the rectangle by the specified vector.
    //    //
    //    // Parameters:
    //    //   offsetVector:
    //    //     A vector that specifies the horizontal and vertical amounts to move the rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     This method is called on the System.Windows.Rect.Empty rectangle.
    //    public void Offset(Vector offsetVector);
    //    //
    //    // Summary:
    //    //     Moves the rectangle by the specified horizontal and vertical amounts.
    //    //
    //    // Parameters:
    //    //   offsetX:
    //    //     The amount to move the rectangle horizontally.
    //    //
    //    //   offsetY:
    //    //     The amount to move the rectangle vertically.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     This method is called on the System.Windows.Rect.Empty rectangle.
    //    public void Offset(double offsetX, double offsetY);
    //    //
    //    // Summary:
    //    //     Returns a rectangle that is offset from the specified rectangle by using
    //    //     the specified vector.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The original rectangle.
    //    //
    //    //   offsetVector:
    //    //     A vector that specifies the horizontal and vertical offsets for the new rectangle.
    //    //
    //    // Returns:
    //    //     The resulting rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     rect is System.Windows.Rect.Empty.
    //    public static Rect Offset(Rect rect, Vector offsetVector);
    //    //
    //    // Summary:
    //    //     Returns a rectangle that is offset from the specified rectangle by using
    //    //     the specified horizontal and vertical amounts.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The rectangle to move.
    //    //
    //    //   offsetX:
    //    //     The horizontal offset for the new rectangle.
    //    //
    //    //   offsetY:
    //    //     The vertical offset for the new rectangle.
    //    //
    //    // Returns:
    //    //     The resulting rectangle.
    //    //
    //    // Exceptions:
    //    //   System.InvalidOperationException:
    //    //     rect is System.Windows.Rect.Empty.
    //    public static Rect Offset(Rect rect, double offsetX, double offsetY);
    //    //
    //    // Summary:
    //    //     Creates a new rectangle from the specified string representation.
    //    //
    //    // Parameters:
    //    //   source:
    //    //     The string representation of the rectangle, in the form "x, y, width, height".
    //    //
    //    // Returns:
    //    //     The resulting rectangle.
    //    public static Rect Parse(string source);
    //    //
    //    // Summary:
    //    //     Multiplies the size of the current rectangle by the specified x and y values.
    //    //
    //    // Parameters:
    //    //   scaleX:
    //    //     The scale factor in the x-direction.
    //    //
    //    //   scaleY:
    //    //     The scale factor in the y-direction.
    //    public void Scale(double scaleX, double scaleY);
    //    //
    //    // Summary:
    //    //     Returns a string representation of the rectangle.
    //    //
    //    // Returns:
    //    //     A string representation of the current rectangle. The string has the following
    //    //     form: "System.Windows.Rect.X,System.Windows.Rect.Y,System.Windows.Rect.Width,System.Windows.Rect.Height".
    //    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    //    public override string ToString();
    //    //
    //    // Summary:
    //    //     Returns a string representation of the rectangle by using the specified format
    //    //     provider.
    //    //
    //    // Parameters:
    //    //   provider:
    //    //     Culture-specific formatting information.
    //    //
    //    // Returns:
    //    //     A string representation of the current rectangle that is determined by the
    //    //     specified format provider.
    //    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    //    public string ToString(IFormatProvider provider);
    //    //
    //    // Summary:
    //    //     Transforms the rectangle by applying the specified matrix.
    //    //
    //    // Parameters:
    //    //   matrix:
    //    //     A matrix that specifies the transformation to apply.
    //    public void Transform(Matrix matrix);
    //    //
    //    // Summary:
    //    //     Returns the rectangle that results from applying the specified matrix to
    //    //     the specified rectangle.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     A rectangle that is the basis for the transformation.
    //    //
    //    //   matrix:
    //    //     A matrix that specifies the transformation to apply.
    //    //
    //    // Returns:
    //    //     The rectangle that results from the operation.
    //    public static Rect Transform(Rect rect, Matrix matrix);
    //    //
    //    // Summary:
    //    //     Expands the current rectangle exactly enough to contain the specified point.
    //    //
    //    // Parameters:
    //    //   point:
    //    //     The point to include.
    //    public void Union(Point point);
    //    //
    //    // Summary:
    //    //     Expands the current rectangle exactly enough to contain the specified rectangle.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The rectangle to include.
    //    public void Union(Rect rect);
    //    //
    //    // Summary:
    //    //     Creates a rectangle that is exactly large enough to include the specified
    //    //     rectangle and the specified point.
    //    //
    //    // Parameters:
    //    //   rect:
    //    //     The rectangle to include.
    //    //
    //    //   point:
    //    //     The point to include.
    //    //
    //    // Returns:
    //    //     A rectangle that is exactly large enough to contain the specified rectangle
    //    //     and the specified point.
    //    public static Rect Union(Rect rect, Point point);
    //    //
    //    // Summary:
    //    //     Creates a rectangle that is exactly large enough to contain the two specified
    //    //     rectangles.
    //    //
    //    // Parameters:
    //    //   rect1:
    //    //     The first rectangle to include.
    //    //
    //    //   rect2:
    //    //     The second rectangle to include.
    //    //
    //    // Returns:
    //    //     The resulting rectangle.
    //    public static Rect Union(Rect rect1, Rect rect2);
    //}
}
