using System;
using System.Collections;

namespace CSUtility.Support
{
    public class TestClass : CSUtility.Support.Copyable
    {
        public enum enTest
        {
            aaa,
            bbb,
            ccc,
        }
        //public enTest ValueEnum { get; set; } = enTest.aaa;
        //public int value1 { get; set; }
        //public string value2 { get; set; }
        //public bool ValueBool { get; set; } = true;
        //public TestClass value3 { get; set; }
        //public System.Collections.Generic.List<enTest> valueListEnum { get; set; } = new System.Collections.Generic.List<enTest>();
        //public System.Collections.Generic.List<float> value4 { get; set; } = new System.Collections.Generic.List<float>();
        //public System.Collections.Generic.Dictionary<int, UInt64> value5 { get; set; } = new System.Collections.Generic.Dictionary<int, UInt64>();
        //public CSUtility.Support.AsyncObjManager<int, float> valueAsyncObjManager = new AsyncObjManager<int, float>();
        //public CSUtility.Support.ConcurentObjManager<int, float> valueConcurentObjManager = new ConcurentObjManager<int, float>();
        public double[] array { get; set; }
        public double[,] array2 { get; set; }

        public void CopyTest()
        {
            //ValueEnum = enTest.bbb;
            //value1 = 5;
            //value2 = "string";
            //ValueBool = false;
            //value3 = new TestClass();
            //for(int i=0; i<3; i++)
            //{
            //    valueListEnum.Add((enTest)i);
            //}

            //value4 = new System.Collections.Generic.List<float>();
            //for (int i = 0; i < 3; i++)
            //{
            //    value4.Add(i);
            //}
            //value5 = new System.Collections.Generic.Dictionary<int, UInt64>();
            //for (int i = 0; i < 2; i++)
            //{
            //    value5[i] = (UInt64)i;
            //}
            array = new double[6];
            for(int i=0; i<6; i++)
            {
                array[i] = i;
            }

            array2 = new double[2, 3];
            for(int i=0; i<2; i++)
            {
                for(int j=0; j<3; j++)
                {
                    array2[i, j] = i + j * i;
                }
            }

            var tag = new TestClass();
            tag.CopyFrom(this);
        }
    }

    /// </summary>
    public class DoNotCopyAttribute : Attribute { }
    public interface ICopyable
    {
        bool CopyFrom(ICopyable src);
    }
    
    public class Copyable : ICopyable
    {
        private static object GetDefaultValue(Type type)
        {
            if (type == null)
                return null;
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        public static object CopyValue(object srcObj)
        {
            if (srcObj == null)
                return null;

            var type = srcObj.GetType();

            try
            {
                if(type.GetInterface((typeof(ICopyable)).FullName) != null)
                {
                    var retValue = System.Activator.CreateInstance(type) as ICopyable;
                    retValue.CopyFrom(srcObj as ICopyable);
                    return retValue;
                }
                else if(type.IsGenericType)
                {
                    if (type.IsValueType)
                    {
                        var proKey = type.GetProperty("Key");
                        var proValue = type.GetProperty("Value");
                        if (proKey == null || proValue == null)
                            return GetDefaultValue(type);

                        var key = proKey.GetValue(srcObj);
                        var value = proValue.GetValue(srcObj);
                        var copyedKey = CopyValue(key);
                        var copyedValue = CopyValue(value);
                        var retValue = System.Activator.CreateInstance(type, new object[] { copyedKey, copyedValue });
                        return retValue;
                    }
                    else
                    {
                        var retValue = System.Activator.CreateInstance(type);
                        System.Reflection.MethodInfo methodAdd = type.GetMethod("Add");
                        if (methodAdd == null)
                            return GetDefaultValue(type);
                        var methodParameter = methodAdd.GetParameters();
                        System.Reflection.PropertyInfo proCount = type.GetProperty("Count");
                        if (proCount == null)
                            return GetDefaultValue(type);

                        var enumerableValue = srcObj as IEnumerable;
                        if (enumerableValue == null)
                            return GetDefaultValue(type);

                        foreach(var item in enumerableValue)
                        {
                            var copyedItem = CopyValue(item);
                            if(methodParameter.Length == 1)
                                methodAdd.Invoke(retValue, new object[] { copyedItem });
                            else if(methodParameter.Length == 2)
                            {
                                var proKey = item.GetType().GetProperty("Key");
                                var proValue = item.GetType().GetProperty("Value");
                                if (proKey == null || proValue == null)
                                    return GetDefaultValue(type);

                                methodAdd.Invoke(retValue, new object[] { proKey.GetValue(copyedItem), proValue.GetValue(copyedItem) });
                            }
                        }
                        return retValue;
                    }
                }
                else if(type.IsArray)
                {
                    var array = srcObj as System.Array;
                    var rank = array.Rank;
                    var itemType = type.Assembly.GetType(type.FullName.Remove(type.FullName.IndexOf('[')));

                    var longLengths = new Int64[rank];
                    var idxs = new Int64[rank];
                    for (int i = 0; i < rank; i++)
                    {
                        longLengths[i] = array.GetLongLength(i);
                        idxs[i] = 0;
                    }
                    var retValue = Array.CreateInstance(itemType, longLengths);
                    for (int i = 0; i < array.Length; i++)
                    {
                        var srcItem = array.GetValue(idxs);
                        var copyedItem = CopyValue(srcItem);
                        retValue.SetValue(copyedItem, idxs);

                        idxs[rank - 1]++;
                        for (int j = rank - 1; j >= 0; j--)
                        {
                            if (idxs[j] >= longLengths[j])
                            {
                                if (j == 0)
                                {
                                    idxs[0] = 0;
                                    break;
                                }
                                idxs[j - 1]++;
                                idxs[j] = 0;
                            }
                        }
                    }
                    return retValue;
                }
                else
                {
                    return srcObj;
                }
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return GetDefaultValue(type);
        }

        public static bool CopyFrom(ICopyable src, ICopyable tag)
        {
            Type type = tag.GetType();
            if (src.GetType() != type)
                return false;

            System.Reflection.PropertyInfo[] props = type.GetProperties();
            foreach (System.Reflection.PropertyInfo i in props)
            {
                var attrs = i.GetCustomAttributes(typeof(DoNotCopyAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    continue;

                attrs = i.GetCustomAttributes(typeof(System.ComponentModel.ReadOnlyAttribute), true);
                if (attrs == null || attrs.Length != 0)
                    continue;

                if (!i.CanWrite)
                    continue;

                var srcValue = i.GetValue(src);
                var copyedValue = CopyValue(srcValue);
                i.SetValue(tag, copyedValue);
            }

            return true;
        }
        public virtual bool CopyFrom(ICopyable src)
        {
            return CopyFrom(src, this);
        }
    }
}
