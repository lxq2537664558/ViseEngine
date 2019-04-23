using System;
using System.Collections;

namespace CSUtility.Support
{
    public class AutoSaveLoadAttribute : System.Attribute { };
    public interface IXndSaveLoadProxy : ICopyable
    {
        bool Read(XndAttrib xndAtt);
        bool Write(XndAttrib xndAtt);
    }
    public class XndSaveLoadProxy : IXndSaveLoadProxy
	{
        public static bool ReadValue(XndAttrib xndAtt, System.Type type, out object outValue)
		{
            outValue = null;

            if (type == null)
            {
                return false;
            }

			if(type == typeof(System.Boolean))
			{
				System.Boolean value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.Int16))
			{
				System.Int16 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.Int32))
			{
				System.Int32 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.Int64))
			{
				System.Int64 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.UInt16))
			{
				System.UInt16 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.UInt32))
			{
				System.UInt32 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.UInt64))
			{
				System.UInt64 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.Byte))
			{
				System.Byte value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.SByte))
			{
				System.SByte value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.Single))
			{
				System.Single value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.Double))
			{
				System.Double value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(SlimDX.Vector2))
			{
				SlimDX.Vector2 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(SlimDX.Vector3))
			{
				SlimDX.Vector3 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(SlimDX.Vector4))
			{
				SlimDX.Vector4 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(SlimDX.Quaternion))
			{
				SlimDX.Quaternion value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(SlimDX.Matrix))
			{
				SlimDX.Matrix value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(SlimDX.Matrix3x2))
			{
				SlimDX.Matrix3x2 value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.Guid))
			{
				System.Guid value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(System.String))
			{
				System.String value;
				xndAtt.Read(out value);
				outValue = value;
			}
			else if(type == typeof(CSUtility.Support.Color))
			{
				int color;
				xndAtt.Read(out color);
				outValue = CSUtility.Support.Color.FromArgb(color);
			}
            else if (type == typeof(System.DateTime))
            {
                string date;
                xndAtt.Read(out date);
                outValue = System.DateTime.Parse(date);
            }
            else if (type.IsEnum)
            {
                System.String enumValue = "";
                xndAtt.Read(out enumValue);
                try
                {
                    outValue = System.Enum.Parse(type, enumValue);
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
            else if (type.GetInterface((typeof(IXndSaveLoadProxy)).FullName) != null)//(type.IsSubclassOf(typeof(XndSaveLoadProxy)))
            {
                IXndSaveLoadProxy value;
                //if (xndAtt.Version >= 4)
                {
                    string valueTypeStr = "";
                    xndAtt.Read(out valueTypeStr);
                    var valueType = Program.GetTypeFromSaveString(valueTypeStr);
                    value = (IXndSaveLoadProxy)Activator.CreateInstance(valueType);
                    value.Read(xndAtt);
                }

                outValue = value;
            }
            else if (type.IsGenericType)
            {
                try
                {
                    if(type.IsValueType)
                    {
                        var pIKey = type.GetProperty("Key");
                        var pIValue = type.GetProperty("Value");
                        if (pIKey == null || pIValue == null)
                            return false;

                        string keyTypeStr;
                        xndAtt.Read(out keyTypeStr);
                        var keyType = Program.GetTypeFromSaveString(keyTypeStr);
                        object keyValue;
                        if (!ReadValue(xndAtt, keyType, out keyValue))
                            return false;

                        string valueTypeStr;
                        xndAtt.Read(out valueTypeStr);
                        var valueType = Program.GetTypeFromSaveString(valueTypeStr);
                        object value;
                        if (!ReadValue(xndAtt, valueType, out value))
                            return false;

                        outValue = Activator.CreateInstance(type, new object[] { keyValue, value });
                    }
                    else
                    {
                        var genericObj = System.Activator.CreateInstance(type);

                        System.Reflection.MethodInfo addMethod = type.GetMethod("Add");
                        if (addMethod == null)
                            return false;
                        var methodParameters = addMethod.GetParameters();

                        int count = 0;
                        xndAtt.Read(out count);

                        for(int i=0; i<count; i++)
                        {
                            string typeStr;
                            xndAtt.Read(out typeStr);
                            var itemType = Program.GetTypeFromSaveString(typeStr);
                            object itemValue;
                            if (!ReadValue(xndAtt, itemType, out itemValue))
                                return false;

                            if (methodParameters.Length == 1)
                                addMethod.Invoke(genericObj, new System.Object[] { itemValue });
                            else if(methodParameters.Length == 2)
                            {
                                var pIKey = itemValue.GetType().GetProperty("Key");
                                var pIValue = itemValue.GetType().GetProperty("Value");
                                if (pIKey == null || pIValue == null)
                                    return false;

                                addMethod.Invoke(genericObj, new object[] { pIKey.GetValue(itemValue), pIValue.GetValue(itemValue) });
                            }
                        }

                        outValue = genericObj;
                    }

                }
                catch(System.Exception exp)
                {
                    Log.FileLog.WriteLine(exp.ToString());
                    return false;
                }
                /*int count;
                xndAtt.Read(out count);
                System.Reflection.MethodInfo mi = type.GetMethod("Add");
                if (mi != null)
                {
                    System.Object lst = System.Activator.CreateInstance(type);

                    //System.Type itemType = type.GetGenericArguments()[0];

                    //array<System.Object> items = new array<System.Object>(count);
                    //System.Collections.Generic.List<System.Object> items = new System.Collections.Generic.List<System.Object>(count);
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Object value;

                            // 读取对象类型
                            System.String itemTypeStr;
                            xndAtt.Read(out itemTypeStr);

                            System.Type itemType = Program.GetTypeFromSaveString(itemTypeStr);

                            ReadValue(xndAtt, itemType, out value);
                            //items[i] = value;
                            //items.Add(value);
                            mi.Invoke(lst, new System.Object[] { value });
                        }
                    }
                    catch (System.Exception exp)
                    {
                        Log.FileLog.WriteLine(exp.ToString());
                        Log.FileLog.WriteLine(exp.StackTrace.ToString());
                    }


                    //mi.Invoke(lst, new array<System.Object>(1){items});
                    outValue = lst;
                }*/
            }
            else if(type.IsArray)
            {
                // 元素类型
                var elementTypeName = type.FullName.Remove(type.FullName.IndexOf('['));
                var elementType = type.Assembly.GetType(elementTypeName);

                int rank = 0;
                xndAtt.Read(out rank);
                Int64[] longLengths = new Int64[rank];
                for(int i=0; i<rank; i++)
                {
                    Int64 lLength;
                    xndAtt.Read(out lLength);
                    longLengths[i] = lLength;
                }

                int[] idxs = new int[rank];
                for (int i = 0; i < rank; i++)
                    idxs[i] = 0;

                int length;
                xndAtt.Read(out length);
                var retValue = Array.CreateInstance(elementType, longLengths);
                for(int idx=0; idx<length; idx++)
                {
                    string itemTypeStr;
                    xndAtt.Read(out itemTypeStr);
                    if (string.IsNullOrEmpty(itemTypeStr))
                        continue;

                    var itemType = Program.GetTypeFromSaveString(itemTypeStr);
                    object itemValue;
                    ReadValue(xndAtt, itemType, out itemValue);

                    retValue.SetValue(itemValue, idxs);
                    idxs[rank - 1]++;
                    for(int i=rank - 1; i>=0; i--)
                    {
                        if(idxs[i] >= longLengths[i])
                        {
                            if(i== 0)
                            {
                                idxs[0] = 0;
                                break;
                            }
                            idxs[i - 1]++;
                            idxs[i] = 0;
                        }
                    }
                }

                outValue = retValue;
            }

			return true;
		}
        public static bool WriteValue(XndAttrib xndAtt, System.Type type, Object InValue)
		{
            if (type == typeof(System.Boolean))
            {
                xndAtt.Write(System.Convert.ToBoolean(InValue));
            }
            else if (type == typeof(System.Int16))
            {
                xndAtt.Write(System.Convert.ToInt16(InValue));
            }
            else if (type == typeof(System.Int32))
            {
                xndAtt.Write(System.Convert.ToInt32(InValue));
            }
            else if (type == typeof(System.Int64))
            {
                xndAtt.Write(System.Convert.ToInt64(InValue));
            }
            else if (type == typeof(System.UInt16))
            {
                xndAtt.Write(System.Convert.ToUInt16(InValue));
            }
            else if (type == typeof(System.UInt32))
            {
                xndAtt.Write(System.Convert.ToUInt32(InValue));
            }
            else if (type == typeof(System.UInt64))
            {
                xndAtt.Write(System.Convert.ToUInt64(InValue));
            }
            else if (type == typeof(System.Byte))
            {
                xndAtt.Write(System.Convert.ToByte(InValue));
            }
            else if (type == typeof(System.SByte))
            {
                xndAtt.Write(System.Convert.ToSByte(InValue));
            }
            else if (type == typeof(System.Single))
            {
                xndAtt.Write(System.Convert.ToSingle(InValue));
            }
            else if (type == typeof(System.Double))
            {
                xndAtt.Write(System.Convert.ToDouble(InValue));
            }
            else if (type == typeof(SlimDX.Vector2))
            {
                xndAtt.Write((SlimDX.Vector2)InValue);
            }
            else if (type == typeof(SlimDX.Vector3))
            {
                xndAtt.Write((SlimDX.Vector3)InValue);
            }
            else if (type == typeof(SlimDX.Vector4))
            {
                xndAtt.Write((SlimDX.Vector4)InValue);
            }
            else if (type == typeof(SlimDX.Quaternion))
            {
                xndAtt.Write((SlimDX.Quaternion)InValue);
            }
            else if (type == typeof(SlimDX.Matrix))
            {
                xndAtt.Write((SlimDX.Matrix)InValue);
            }
            else if (type == typeof(SlimDX.Matrix3x2))
            {
                xndAtt.Write((SlimDX.Matrix3x2)InValue);
            }
            else if (type == typeof(System.Guid))
            {
                System.Guid value = (System.Guid)(InValue);
                xndAtt.Write(value);
            }
            else if (type == typeof(System.String))
            {
                System.String str = (System.String)(InValue);
                if (str == null)
                    str = "";
                xndAtt.Write(str);
            }
            else if (type == typeof(CSUtility.Support.Color))
            {
                int color = ((CSUtility.Support.Color)InValue).ToArgb();
                xndAtt.Write(color);
            }
            else if (type == typeof(System.DateTime))
            {
                var dateTime = (System.DateTime)(InValue);
                xndAtt.Write(dateTime.ToString());
            }
            else if (type.IsEnum)
            {
                System.String enumValue = InValue.ToString();
                xndAtt.Write(enumValue);
            }
            else if (type.GetInterface((typeof(IXndSaveLoadProxy)).FullName) != null)//(type.IsSubclassOf(typeof(XndSaveLoadProxy)))
            {
                IXndSaveLoadProxy value = (IXndSaveLoadProxy)(InValue);

                var typeStr = Program.GetTypeSaveString(InValue.GetType());
                xndAtt.Write(typeStr);
                value.Write(xndAtt);
            }
            else if (type.IsGenericType)
            {
                if (type.IsValueType)
                {
                    var pIKey = type.GetProperty("Key");
                    var pIValue = type.GetProperty("Value");
                    if (pIKey != null && pIValue != null)
                    {
                        var key = pIKey.GetValue(InValue, null);
                        var value = pIValue.GetValue(InValue, null);
                        if (key != null && value != null)
                        {
                            var keyTypeStr = Program.GetTypeSaveString(key.GetType());
                            xndAtt.Write(keyTypeStr);
                            WriteValue(xndAtt, key.GetType(), key);

                            var valueTypeStr = Program.GetTypeSaveString(value.GetType());
                            xndAtt.Write(valueTypeStr);
                            WriteValue(xndAtt, value.GetType(), value);
                        }
                    }
                }
                else
                {
                    System.Reflection.PropertyInfo pICount = type.GetProperty("Count");
                    if (pICount != null)
                    {
                        var enumerableValue = InValue as IEnumerable;
                        if (enumerableValue != null)
                        {
                            //var enumerator = enumerableValue.GetEnumerator();
                            int count = (int)pICount.GetValue(InValue, null);
                            xndAtt.Write(count);

                            foreach (var item in enumerableValue)
                            {
                                if (item == null)
                                    continue;
                                var typeStr = Program.GetTypeSaveString(item.GetType());
                                xndAtt.Write(typeStr);
                                WriteValue(xndAtt, item.GetType(), item);
                            }
                        }
                    }
                }
            }
            else if (type.IsArray)
            {
                var array = InValue as System.Array;
                if (array != null)
                {
                    int rank = array.Rank;
                    xndAtt.Write(rank);
                    for (int i = 0; i < rank; i++)
                    {
                        Int64 longLength = array.GetLongLength(i);
                        xndAtt.Write(longLength);
                    }

                    int length = array.Length;
                    xndAtt.Write(length);
                    foreach (var item in array)
                    {
                        if (item == null)
                        {
                            xndAtt.Write("");
                        }
                        else
                        {
                            var typeStr = Program.GetTypeSaveString(item.GetType());
                            xndAtt.Write(typeStr);
                            WriteValue(xndAtt, item.GetType(), item);
                        }
                    }
                }
            }

			return true;
		}
        public static bool Read(IXndSaveLoadProxy slObj, XndAttrib xndAtt)
        {
            try
            {
                System.Type objType = slObj.GetType();

                System.Int32 hashCode;
                xndAtt.Read(out hashCode);

                CSUtility.Support.ClassInfo classInfo = CSUtility.Support.ClassInfoManager.Instance.GetClassInfoWithHashCode(hashCode);
                if (classInfo == null)
                {
                    var newHashCode = (UInt32)hashCode;
                    classInfo = CSUtility.Support.ClassInfoManager.Instance.GetClassInfoWithHashCode(newHashCode);
                    if (classInfo == null)
                        return false;
                }

                //if (xndAtt.Version <= 2 || xndAtt.Version >= 4)
                {
                    foreach (CSUtility.Support.PropertyInfo proInfo in classInfo.mPropertyInfoDic.Values)
                    {
                        Object value = null;
                        if (!ReadValue(xndAtt, proInfo.mType, out value))
                            return false;

                        System.Reflection.PropertyInfo classProperty = proInfo.ProInfo;//objType.GetProperty(proInfo.mName, proInfo.mType);
                        if (classProperty == null)
                            continue;

                        if (!classProperty.CanWrite)
                            continue;

                        try
                        {
                            classProperty.SetValue(slObj, value, null);
                        }
                        catch (System.Exception ex)
                        {
                            Log.FileLog.WriteLine(ex.ToString());
                            Log.FileLog.WriteLine(ex.StackTrace.ToString());
                        }
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return false;
        }
        public virtual bool Read(XndAttrib xndAtt)
		{
            return Read(this, xndAtt);
		}
        public static bool Write(IXndSaveLoadProxy slObj, XndAttrib xndAtt)
        {
            System.Type objType = slObj.GetType();

			CSUtility.Support.ClassInfo classInfo = CSUtility.Support.ClassInfoManager.Instance.GetLastVersionClassInfo(objType);
			if(classInfo == null)
			{
				System.Int32 tempHashCode = 0;
				xndAtt.Write(tempHashCode);
				return false;
			}

            //xndAtt.Version = 4;

			System.Int32 hashCode = (System.Int32)(classInfo.mNewHashCode);//classInfo.mHashCode;
			xndAtt.Write(hashCode);

            foreach (CSUtility.Support.PropertyInfo property in classInfo.mPropertyInfoDic.Values)
            {
                System.Reflection.PropertyInfo classProperty = objType.GetProperty(property.mName, property.mType);
                WriteValue(xndAtt, property.mType, classProperty.GetValue(slObj, null));
            }

			return true;
        }
        public virtual bool Write(XndAttrib xndAtt)
		{
            return Write(this, xndAtt);
		}

        public virtual bool CopyFrom(ICopyable srcData)
		{
            return Copyable.CopyFrom(srcData, this);
		}
    };
}
