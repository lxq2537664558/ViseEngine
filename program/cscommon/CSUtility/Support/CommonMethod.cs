namespace CSUtility.Support
{


    [CSUtility.Event.Attribute.AllowClass("ClassTestNew", CSUtility.Helper.enCSType.Common, "bbbbb", Event.Attribute.AllowClass.enClassType.New)]
    public class ClassTestNew
    {
        [CSUtility.Event.Attribute.AllowMember("函数.GetB", CSUtility.Helper.enCSType.Common, "bbbbb")]
        public int GetB()
        {
            return 0;
        }
    }

    public class Math
    {
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.PI", CSUtility.Helper.enCSType.Common, "表示圆的周长与其直径的比值，由常数 π 指定")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.PI", CSUtility.Helper.enCSType.Common, "表示圆的周长与其直径的比值，由常数 π 指定")]
        public static double PI
        {
            get { return System.Math.PI; }
        }
        static System.Random mRandom = new System.Random();
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.随机", CSUtility.Helper.enCSType.Common, "获取最小值到最大值之间的随机值")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.随机", CSUtility.Helper.enCSType.Common, "获取最小值到最大值之间的随机值")]
        public static int Random(int min, int max)
        {
            return mRandom.Next(min, max);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.度转弧度", CSUtility.Helper.enCSType.Common, "角度单位从度转为弧度")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.度转弧度", CSUtility.Helper.enCSType.Common, "角度单位从度转为弧度")]
        public static double Degree2Radian(double angle)
        {
            return angle / 180 * Math.PI;
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.弧度转度", CSUtility.Helper.enCSType.Common, "角度单位从弧度转为度")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.弧度转度", CSUtility.Helper.enCSType.Common, "角度单位从弧度转为度")]
        public static double Radian2Degree(double radian)
        {
            return radian / Math.PI * 180;
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Abs", CSUtility.Helper.enCSType.Common, "获取数字的绝对值")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Abs", CSUtility.Helper.enCSType.Common, "获取数字的绝对值")]
        public static double Abs(double value)
        {
            return System.Math.Abs(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Acos", CSUtility.Helper.enCSType.Common, "返回余弦值为指定数字的角度(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Acos", CSUtility.Helper.enCSType.Common, "返回余弦值为指定数字的角度(角度单位为弧度)")]
        public static double Acos(double value)
        {
            return System.Math.Acos(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Asin", CSUtility.Helper.enCSType.Common, "返回正弦值为指定数字的角度(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Asin", CSUtility.Helper.enCSType.Common, "返回正弦值为指定数字的角度(角度单位为弧度)")]
        public static double Asin(double value)
        {
            return System.Math.Asin(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Atan", CSUtility.Helper.enCSType.Common, "返回正切值为指定数字的角度(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Atan", CSUtility.Helper.enCSType.Common, "返回正切值为指定数字的角度(角度单位为弧度)")]
        public static double Atan(double value)
        {
            return System.Math.Atan(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Atan2", CSUtility.Helper.enCSType.Common, "返回正切值为两个指定数字的商的角度，x、y标识x、y坐标(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Atan2", CSUtility.Helper.enCSType.Common, "返回正切值为两个指定数字的商的角度，x、y标识x、y坐标(角度单位为弧度)")]
        public static double Atan2(double y, double x)
        {
            return System.Math.Atan2(y, x);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.BigMul", CSUtility.Helper.enCSType.Common, "生成两个 32 位数字的完整乘积")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.BigMul", CSUtility.Helper.enCSType.Common, "生成两个 32 位数字的完整乘积")]
        public static long BigMul(int a, int b)
        {
            return System.Math.BigMul(a, b);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Ceiling", CSUtility.Helper.enCSType.Common, "返回大于或等于指定的双精度浮点数的最小整数值")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Ceiling", CSUtility.Helper.enCSType.Common, "返回大于或等于指定的双精度浮点数的最小整数值")]
        public static double Ceiling(double value)
        {
            return System.Math.Ceiling(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Cos", CSUtility.Helper.enCSType.Common, "返回指定角度的余弦值(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Cos", CSUtility.Helper.enCSType.Common, "返回指定角度的余弦值(角度单位为弧度)")]
        public static double Cos(double value)
        {
            return System.Math.Cos(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Cosh", CSUtility.Helper.enCSType.Common, "返回指定角度的双曲余弦值(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Cosh", CSUtility.Helper.enCSType.Common, "返回指定角度的双曲余弦值(角度单位为弧度)")]
        public static double Cosh(double value)
        {
            return System.Math.Cosh(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.DivRem", CSUtility.Helper.enCSType.Common, "计算两个 32 位有符号整数的商，并通过输出参数返回余数")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.DivRem", CSUtility.Helper.enCSType.Common, "计算两个 32 位有符号整数的商，并通过输出参数返回余数")]
        public static int DivRem(int a, int b, out int result)
        {
            return System.Math.DivRem(a, b, out result);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.DivRem", CSUtility.Helper.enCSType.Common, "计算两个 64 位有符号整数的商，并通过输出参数返回余数")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.DivRem", CSUtility.Helper.enCSType.Common, "计算两个 64 位有符号整数的商，并通过输出参数返回余数")]
        public static long DivRem(long a, long b, out long result)
        {
            return System.Math.DivRem(a, b, out result);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Exp", CSUtility.Helper.enCSType.Common, "返回 e 的指定次幂")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Exp", CSUtility.Helper.enCSType.Common, "返回 e 的指定次幂")]
        public static double Exp(double value)
        {
            return System.Math.Exp(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Floor", CSUtility.Helper.enCSType.Common, "返回小于或等于指定双精度浮点数的最大整数")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Floor", CSUtility.Helper.enCSType.Common, "返回小于或等于指定双精度浮点数的最大整数")]
        public static double Floor(double value)
        {
            return System.Math.Floor(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.IEEERemainder", CSUtility.Helper.enCSType.Common,
                    "返回一指定数字被另一指定数字相除的余数。\r\n 一个数等于 x - (y Q)，其中 Q 是 x / y 的商的最接近整数（如果 x / y 在两个整数中间，则返回偶数）。\r\n如果 x - (y Q) 为零，则在 x 为正时返回值 +0，而在 x 为负时返回 -0。\r\n如果 y = 0，则返回 NaN。")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.IEEERemainder", CSUtility.Helper.enCSType.Common, 
                    "返回一指定数字被另一指定数字相除的余数。\r\n 一个数等于 x - (y Q)，其中 Q 是 x / y 的商的最接近整数（如果 x / y 在两个整数中间，则返回偶数）。\r\n如果 x - (y Q) 为零，则在 x 为正时返回值 +0，而在 x 为负时返回 -0。\r\n如果 y = 0，则返回 NaN。")]
        public static double IEEERemainder(double x, double y)
        {
            return System.Math.IEEERemainder(x, y);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Log", CSUtility.Helper.enCSType.Common, "返回指定数字的自然对数（底为 e）")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Log", CSUtility.Helper.enCSType.Common, "返回指定数字的自然对数（底为 e）")]
        public static double Log(double value)
        {
            return System.Math.Log(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Log", CSUtility.Helper.enCSType.Common, "返回指定数字在使用指定底时的对数")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Log", CSUtility.Helper.enCSType.Common, "返回指定数字在使用指定底时的对数")]
        public static double Log(double a, double newBase)
        {
            return System.Math.Log(a, newBase);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Log10", CSUtility.Helper.enCSType.Common, "返回指定数字以 10 为底的对数")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Log10", CSUtility.Helper.enCSType.Common, "返回指定数字以 10 为底的对数")]
        public static double Log10(double value)
        {
            return System.Math.Log10(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Max", CSUtility.Helper.enCSType.Common, "返回两个数字中较大的一个")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Max", CSUtility.Helper.enCSType.Common, "返回两个数字中较大的一个")]
        public static double Max(double val1, double val2)
        {
            return System.Math.Max(val1, val2);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Min", CSUtility.Helper.enCSType.Common, "返回两个数字中较小的一个")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Min", CSUtility.Helper.enCSType.Common, "返回两个数字中较小的一个")]
        public static double Min(double val1, double val2)
        {
            return System.Math.Min(val1, val2);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Pow", CSUtility.Helper.enCSType.Common, "返回指定数字的指定次幂")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Pow", CSUtility.Helper.enCSType.Common, "返回指定数字的指定次幂")]
        public static double Pow(double x, double y)
        {
            return System.Math.Pow(x, y);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Round", CSUtility.Helper.enCSType.Common, "将数值舍入为最接近的整数值")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Round", CSUtility.Helper.enCSType.Common, "将数值舍入为最接近的整数值")]
        public static double Round(double value)
        {
            return System.Math.Round(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Round", CSUtility.Helper.enCSType.Common, "将数值按指定的小数位数舍入,digits范围0~15")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Round", CSUtility.Helper.enCSType.Common, "将数值按指定的小数位数舍入,digits范围0~15")]
        public static double Round(double value, int digits)
        {
            if (digits < 0)
                digits = 0;
            else if (digits > 15)
                digits = 15;

            return System.Math.Round(value, digits);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Sign", CSUtility.Helper.enCSType.Common, "返回表示双精度浮点数字的符号的值\r\n返回值 \r\n  -1 value小于0, \r\n  0 value等于0 \r\n  1 value大于0")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Sign", CSUtility.Helper.enCSType.Common, "返回表示双精度浮点数字的符号的值\r\n返回值 \r\n  -1 value小于0, \r\n  0 value等于0 \r\n  1 value大于0")]
        public static int Sign(double value)
        {
            if (double.IsNaN(value))
                return 0;
            return System.Math.Sign(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Sin", CSUtility.Helper.enCSType.Common, "返回指定角度的正弦值(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Sin", CSUtility.Helper.enCSType.Common, "返回指定角度的正弦值(角度单位为弧度)")]
        public static double Sin(double value)
        {
            return System.Math.Sin(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Sinh", CSUtility.Helper.enCSType.Common, "返回指定角度的双曲正弦值(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Sinh", CSUtility.Helper.enCSType.Common, "返回指定角度的双曲正弦值(角度单位为弧度)")]
        public static double Sinh(double value)
        {
            return System.Math.Sinh(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Sqrt", CSUtility.Helper.enCSType.Common, "返回指定数字的平方根")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Sqrt", CSUtility.Helper.enCSType.Common, "返回指定数字的平方根")]
        public static double Sqrt(double value)
        {
            return System.Math.Sqrt(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Tan", CSUtility.Helper.enCSType.Common, "返回指定角度的正切值(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Tan", CSUtility.Helper.enCSType.Common, "返回指定角度的正切值(角度单位为弧度)")]
        public static double Tan(double value)
        {
            return System.Math.Tan(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Tanh", CSUtility.Helper.enCSType.Common, "返回指定角度的双曲正切值(角度单位为弧度)")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Tanh", CSUtility.Helper.enCSType.Common, "返回指定角度的双曲正切值(角度单位为弧度)")]
        public static double Tanh(double value)
        {
            return System.Math.Tanh(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.Truncate", CSUtility.Helper.enCSType.Common, "计算指定数值的整数部分")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.Truncate", CSUtility.Helper.enCSType.Common, "计算指定数值的整数部分")]
        public static double Truncate(double value)
        {
            return System.Math.Truncate(value);
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.向量归一化(Normalize)2D", CSUtility.Helper.enCSType.Common, "将向量归一化为长度为1的向量")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.向量归一化(Normalize)2D", CSUtility.Helper.enCSType.Common, "将向量归一化为长度为1的向量")]
        public static SlimDX.Vector2 Normalize(SlimDX.Vector2 value)
        {
            var retValue = new SlimDX.Vector2(value.X, value.Y);
            retValue.Normalize();
            return retValue;
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.向量归一化(Normalize)3D", CSUtility.Helper.enCSType.Common, "将向量归一化为长度为1的向量")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.向量归一化(Normalize)3D", CSUtility.Helper.enCSType.Common, "将向量归一化为长度为1的向量")]
        public static SlimDX.Vector3 Normalize(SlimDX.Vector3 value)
        {
            var retValue = new SlimDX.Vector3(value.X, value.Y, value.Z);
            retValue.Normalize();
            return retValue;
        }
        [CSUtility.AISystem.Attribute.AllowMember("数学函数.向量归一化(Normalize)4D", CSUtility.Helper.enCSType.Common, "将向量归一化为长度为1的向量")]
        [CSUtility.Event.Attribute.AllowMember("数学函数.向量归一化(Normalize)4D", CSUtility.Helper.enCSType.Common, "将向量归一化为长度为1的向量")]
        public static SlimDX.Vector4 Normalize(SlimDX.Vector4 value)
        {
            var retValue = new SlimDX.Vector4(value.X, value.Y, value.Z, value.W);
            retValue.Normalize();
            return retValue;
        }
    }

    public class CommonMethod
    {
        

        //[CSUtility.AISystem.Attribute.AllowMethod(CSUtility.Helper.enCSType.Common)]
        //[CSUtility.Event.Attribute.AllowMember("函数.新建列表", CSUtility.Helper.enCSType.Common, "创建一个")]
        //public List<object> NewObjectList()
        //{
        //    return new List<object>();
        //}

        //[CSUtility.AISystem.Attribute.AllowMethod(CSUtility.Helper.enCSType.Common)]
        //[CSUtility.Event.Attribute.AllowMember(CSUtility.Helper.enCSType.Common)]
        //public object GetListElement(List<object> lst, int index)
        //{
        //    if (lst == null)
        //        return null;
        //    if (index >= lst.Count)
        //        return null;
        //    return lst[index];
        //}

        //[CSUtility.AISystem.Attribute.AllowMethod(CSUtility.Helper.enCSType.Common)]
        //[CSUtility.Event.Attribute.AllowMethod(CSUtility.Helper.enCSType.Common)]
        //public void AddListElement(List<object> lst, object obj)
        //{
        //    if (lst == null)
        //        return;
        //    lst.Add(obj);
        //}

        //[CSUtility.AISystem.Attribute.AllowMethod(CSUtility.Helper.enCSType.Common)]
        //[CSUtility.Event.Attribute.AllowMethod(CSUtility.Helper.enCSType.Common)]
        //public void RemoveListElement(List<object> lst, object obj)
        //{
        //    if (lst == null)
        //        return;
        //    lst.Remove(obj);
        //}

        //[CSUtility.AISystem.Attribute.AllowMethod(CSUtility.Helper.enCSType.Common)]
        //[CSUtility.Event.Attribute.AllowMethod(CSUtility.Helper.enCSType.Common)]
        //public Guid ParseGuidFromString(string str)
        //{
        //    if (string.IsNullOrEmpty(str))
        //        return Guid.Empty;
        //    return CSUtility.Support.IHelper.GuidTryParse(str);
        //}
    }
}
