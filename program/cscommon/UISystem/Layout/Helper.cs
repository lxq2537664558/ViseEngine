using System;

namespace UISystem
{
    internal static class FloatUtil
    {
        internal const float DBL_EPSILON = 2.220446e-016f;
        internal const float FLT_MIN = 1.175494351e-38F; 

        public static bool AreClose(float value1, float value2)
        {
            if (value1 == value2) return true;
            // (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON 
            float eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0f) * DBL_EPSILON;
            float delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        public static bool LessThan(float value1, float value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }


        public static bool GreaterThan(float value1, float value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }

        public static bool LessThanOrClose(float value1, float value2)
        {
            return (value1 < value2) || AreClose(value1, value2);
        }

        public static bool GreaterThanOrClose(float value1, float value2)
        {
            return (value1 > value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// 判断数值是否趋近于1，相当于AreClose(float, 1)但比它快
        /// </summary>
        public static bool IsOne(float value)
        {
            return Math.Abs(value - 1.0) < 10.0f * DBL_EPSILON;
        }

        /// <summary>
        /// 判断数值是否趋近于0，相当于AreClose(float, 0)但比它快
        /// </summary> 
        public static bool IsZero(float value)
        {
            return Math.Abs(value) < 10.0f * DBL_EPSILON;
        }

        public static bool AreClose(SlimDX.Size size1, SlimDX.Size size2)
        {
            return FloatUtil.AreClose(size1.Width, size2.Width) &&
                   FloatUtil.AreClose(size1.Height, size2.Height);
        }

        public static bool AreClose(SlimDX.Vector2 vector1, SlimDX.Vector2 vector2)
        {
            return FloatUtil.AreClose(vector1.X, vector2.X) &&
                   FloatUtil.AreClose(vector1.Y, vector2.Y);
        }

        public static bool AreClose(SlimDX.Rect rect1, SlimDX.Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            return (!rect2.IsEmpty) &&
                FloatUtil.AreClose(rect1.X, rect2.X) &&
                FloatUtil.AreClose(rect1.Y, rect2.Y) &&
                FloatUtil.AreClose(rect1.Height, rect2.Height) &&
                FloatUtil.AreClose(rect1.Width, rect2.Width);
        }

        public static bool AreClose(CSUtility.Support.Rectangle rect1, CSUtility.Support.Rectangle rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            return (!rect2.IsEmpty) &&
                FloatUtil.AreClose(rect1.X, rect2.X) &&
                FloatUtil.AreClose(rect1.Y, rect2.Y) &&
                FloatUtil.AreClose(rect1.Height, rect2.Height) &&
                FloatUtil.AreClose(rect1.Width, rect2.Width);
        }

        public static bool AreSizeClose(SlimDX.Rect rect1, SlimDX.Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            return (!rect2.IsEmpty) &&
                FloatUtil.AreClose(rect1.Height, rect2.Height) &&
                FloatUtil.AreClose(rect1.Width, rect2.Width);
        }

        /// <summary> 
        /// 
        /// </summary>
        /// <param name="val"></param> 
        /// <returns></returns>
        public static bool IsBetweenZeroAndOne(float val)
        {
            return (GreaterThanOrClose(val, 0) && LessThanOrClose(val, 1));
        }

        /// <summary> 
        ///
        /// </summary> 
        /// <param name="val"></param>
        /// <returns></returns>
        public static int FloatToInt(float val)
        {
            return (0 < val) ? (int)(val + 0.5) : (int)(val - 0.5);
        }


        /// <summary> 
        /// 如果r的 X、Y、Height或Width为NaN的话返回true
        /// </summary>
        public static bool RectHasNaN(SlimDX.Rect r)
        {
            if (FloatUtil.IsNaN(r.X)
                 || FloatUtil.IsNaN(r.Y)
                 || FloatUtil.IsNaN(r.Height)
                 || FloatUtil.IsNaN(r.Width))
            {
                return true;
            }
            return false;
        }


#if !PBTCOMPILER

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct NanUnion
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            internal float FloatValue;
            [System.Runtime.InteropServices.FieldOffset(0)]
            internal UInt64 UintValue;
        }

        // 此IsNaN比float.IsNaN()性能好，在性能敏感代码处使用
        public static bool IsNaN(float value)
        {
            NanUnion t = new NanUnion();
            t.FloatValue = value;

            UInt64 exp = t.UintValue & 0xfff0000000000000;
            UInt64 man = t.UintValue & 0x000fffffffffffff;

            return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
        }
#endif
    } 

    internal static class DoubleUtil
    {
        internal const double DBL_EPSILON = 2.2204460492503131e-016; 
        internal const float FLT_MIN = 1.175494351e-38F; 

        public static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2) return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON 
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        public static bool LessThan(double value1, double value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }
        
        public static bool GreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }
        
        public static bool LessThanOrClose(double value1, double value2)
        {
            return (value1 < value2) || AreClose(value1, value2);
        }
        
        public static bool GreaterThanOrClose(double value1, double value2)
        {
            return (value1 > value2) || AreClose(value1, value2);
        }
        
        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * DBL_EPSILON;
        }
        
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }
        
        public static bool AreClose(SlimDX.Size size1, SlimDX.Size size2)
        {
            return DoubleUtil.AreClose(size1.Width, size2.Width) &&
                   DoubleUtil.AreClose(size1.Height, size2.Height);
        }
        
        public static bool AreClose(SlimDX.Vector2 vector1, SlimDX.Vector2 vector2)
        {
            return DoubleUtil.AreClose(vector1.X, vector2.X) &&
                   DoubleUtil.AreClose(vector1.Y, vector2.Y);
        }
        
        public static bool AreClose(SlimDX.Rect rect1, SlimDX.Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            return (!rect2.IsEmpty) &&
                DoubleUtil.AreClose(rect1.X, rect2.X) &&
                DoubleUtil.AreClose(rect1.Y, rect2.Y) &&
                DoubleUtil.AreClose(rect1.Height, rect2.Height) &&
                DoubleUtil.AreClose(rect1.Width, rect2.Width);
        }

        public static bool AreClose(CSUtility.Support.Rectangle rect1, CSUtility.Support.Rectangle rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            return (!rect2.IsEmpty) &&
                DoubleUtil.AreClose(rect1.X, rect2.X) &&
                DoubleUtil.AreClose(rect1.Y, rect2.Y) &&
                DoubleUtil.AreClose(rect1.Height, rect2.Height) &&
                DoubleUtil.AreClose(rect1.Width, rect2.Width);
        }

        public static bool AreSizeClose(SlimDX.Rect rect1, SlimDX.Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            return (!rect2.IsEmpty) &&
                DoubleUtil.AreClose(rect1.Height, rect2.Height) &&
                DoubleUtil.AreClose(rect1.Width, rect2.Width);
        }

        /// <summary> 
        /// 
        /// </summary>
        /// <param name="val"></param> 
        /// <returns></returns>
        public static bool IsBetweenZeroAndOne(double val)
        {
            return (GreaterThanOrClose(val, 0) && LessThanOrClose(val, 1));
        }

        /// <summary> 
        ///
        /// </summary> 
        /// <param name="val"></param>
        /// <returns></returns>
        public static int DoubleToInt(double val)
        {
            return (0 < val) ? (int)(val + 0.5) : (int)(val - 0.5);
        }


        public static bool RectHasNaN(SlimDX.Rect r)
        {
            if (DoubleUtil.IsNaN(r.X)
                 || DoubleUtil.IsNaN(r.Y)
                 || DoubleUtil.IsNaN(r.Height)
                 || DoubleUtil.IsNaN(r.Width))
            {
                return true;
            }
            return false;
        }


#if !PBTCOMPILER

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        private struct NanUnion
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            internal double DoubleValue;
            [System.Runtime.InteropServices.FieldOffset(0)]
            internal UInt64 UintValue;
        }
        
        // 此IsNaN比double.IsNaN()性能好，在性能敏感代码处使用
        public static bool IsNaN(double value)
        {
            NanUnion t = new NanUnion();
            t.DoubleValue = value;

            UInt64 exp = t.UintValue & 0xfff0000000000000;
            UInt64 man = t.UintValue & 0x000fffffffffffff;

            return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
        }
#endif
    } 

    
}
