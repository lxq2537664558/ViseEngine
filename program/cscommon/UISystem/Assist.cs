using System;

namespace UISystem
{
    public class Assist
    {
        public static float MinFloatValue = 0.0001f;

        public static object GetValueWithType(Type type, object value)
        {
            try
            {
                if (value == null)
                    return null;
                if (object.Equals(value, "{null}") || object.Equals(value, "{Null}"))
                    return null;
                if (type == typeof(bool))
                    return System.Convert.ToBoolean(value);
                if (type == typeof(SByte))
                    return System.Convert.ToSByte(value);
                if (type == typeof(Int16))
                    return System.Convert.ToInt16(value);
                if (type == typeof(Int32))
                    return System.Convert.ToInt32(value);
                if (type == typeof(Int64))
                    return System.Convert.ToInt64(value);
                if (type == typeof(Byte))
                    return System.Convert.ToByte(value);
                if (type == typeof(UInt16))
                    return System.Convert.ToUInt16(value);
                if (type == typeof(UInt32))
                    return System.Convert.ToUInt32(value);
                if (type == typeof(UInt64))
                    return System.Convert.ToUInt64(value);
                if (type == typeof(Single))
                    return System.Convert.ToSingle(value);
                if (type == typeof(Double))
                    return System.Convert.ToDouble(value);
                if (type == typeof(string))
                    return System.Convert.ToString(value);
                if (type == typeof(CSUtility.Support.Color))
                {
                    if (value is CSUtility.Support.Color)
                        return value;
                    if (value is System.String)
                    {
                        string strValue = (System.String)value;
                        if(string.IsNullOrEmpty(strValue))
                            return null;

                        var idx = strValue.IndexOf("A=");
                        int a = System.Convert.ToInt32(strValue.Substring(idx + 2, strValue.IndexOf(',', idx) - idx - 2));
                        idx = strValue.IndexOf("R=");
                        int r = System.Convert.ToInt32(strValue.Substring(idx + 2, strValue.IndexOf(',', idx) - idx - 2));
                        idx = strValue.IndexOf("G=");
                        int g = System.Convert.ToInt32(strValue.Substring(idx + 2, strValue.IndexOf(',', idx) - idx - 2));
                        idx = strValue.IndexOf("B=");
                        int b = System.Convert.ToInt32(strValue.Substring(idx + 2, strValue.IndexOf(']', idx) - idx - 2));
                        return CSUtility.Support.Color.FromArgb(a, r, g, b);
                    }

                    return null;
                }
                if (type == typeof(CSUtility.Support.Thickness))
                {
                    if (value is CSUtility.Support.Thickness)
                        return value;
                    if (value is System.String)
                    {
                        var splits = ((System.String)value).Split(',');
                        if (splits.Length < 4)
                            return null;
                        double left = System.Convert.ToDouble(splits[0]);
                        double top = System.Convert.ToDouble(splits[1]);
                        double right = System.Convert.ToDouble(splits[2]);
                        double bottom = System.Convert.ToDouble(splits[3]);
                        return new CSUtility.Support.Thickness(left, top, right, bottom);
                    }

                    return null;
                }
                if (type.IsEnum)
                    return System.Enum.Parse(type, value.ToString());
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }
    }
}
