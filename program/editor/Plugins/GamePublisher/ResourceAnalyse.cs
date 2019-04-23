using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GamePublisher
{
    public class ResourceAnalyse
    {
        static ResourceAnalyse mInstance = new ResourceAnalyse();
        public static ResourceAnalyse Instance
        {
            get { return mInstance; }
        }
        private ResourceAnalyse()
        {

        }

        List<string> mAnalyseResourceClass = new List<string>();
        Dictionary<string, List<string>> mAnalyseResClassDic = new Dictionary<string, List<string>>();

        public void Clear()
        {
            mAnalyseResourceClass.Clear();
            mAnalyseResClassDic.Clear();
        }
        public void Init()
        {
            var assemblys = CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.Client);
            foreach (var assembly in assemblys)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    foreach (var property in type.GetProperties())
                    {
                        var atts = property.GetCustomAttributes(typeof(CSUtility.Support.ResourcePublishAttribute), true);
                        if (atts.Length <= 0)
                            continue;
                        if (!mAnalyseResourceClass.Contains(type.FullName))
                            mAnalyseResourceClass.Add(type.FullName);
                    }
                }
            }            

            AnalyseResource();
        }

        int mPreAnalyseCount = 0;
        public void AnalyseResource()
        {
            mPreAnalyseCount = GetAnalyseCount();

            while (true)
            {
                var assemblys = CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.Client);
                foreach (var assembly in assemblys)
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        foreach (var property in type.GetProperties())
                        {
                            AnalyseResource(type.FullName, property.Name, property.PropertyType);
                        }
                    }
                }

                int count = GetAnalyseCount();
                if (mPreAnalyseCount == count)
                    break;

                mPreAnalyseCount = count;
            }           
        }

        int GetAnalyseCount()
        {
            int count = mAnalyseResourceClass.Count + mAnalyseResClassDic.Count;
            foreach(var i in mAnalyseResClassDic.Values)
            {
                count += i.Count;
            }

            return count;
        }
        void AnalyseResource(string typeName,string propertyName,Type type)
        {
            if (IsBaseType(type))
            {
                return;
            }                
            else if (type.IsGenericType)
            {
                var agTypes = type.GetGenericArguments();
                foreach (var i in agTypes)
                {                                    
                    AnalyseResource(typeName, propertyName, i);
                }
            }
            else if (type.IsArray)
            {
                var elementTypeName = type.FullName.Remove(type.FullName.IndexOf('['));
                var elementType = type.Assembly.GetType(elementTypeName);

                AnalyseResource(typeName, propertyName, elementType);
            }
            else
            {                
                AnalyseResource(typeName, propertyName, type.FullName);
//                 foreach (var property in type.GetProperties())
//                 {
//                     analyseOver = AnalyseResource(type.FullName, property.Name, property.PropertyType);
//                 }
            }            
        }
        public void AnalyseResource(string className,string propertyName,string propertyTypeName)
        {
            if (mAnalyseResourceClass.Contains(propertyTypeName))
            {
                if (!mAnalyseResClassDic.ContainsKey(className))
                {
                    var list = new List<string>();                    
                    mAnalyseResClassDic.Add(className, list);
                    
                    if (className != propertyTypeName)
                    {
                        mAnalyseResourceClass.Add(className);
                        list.Add(propertyName);
                    }
                }
                else
                {
                    var list = mAnalyseResClassDic[className];
                    if (list != null && !list.Contains(propertyName))
                    {
                        if (className != propertyTypeName)
                        {
                            list.Add(propertyName);
                        }                                                
                    }
                }
            }
        }

        bool IsBaseType(Type type)
        {            
            if (type == typeof(System.Boolean))
            {
                return true;
            }
            else if (type == typeof(System.Int16))
            {
                return true;
            }
            else if (type == typeof(System.Int32))
            {
                return true;
            }
            else if (type == typeof(System.Int64))
            {
                return true;
            }
            else if (type == typeof(System.UInt16))
            {
                return true;
            }
            else if (type == typeof(System.UInt32))
            {
                return true;
            }
            else if (type == typeof(System.UInt64))
            {
                return true;
            }
            else if (type == typeof(System.Byte))
            {
                return true;
            }
            else if (type == typeof(System.SByte))
            {
                return true;
            }
            else if (type == typeof(System.Single))
            {
                return true;
            }
            else if (type == typeof(System.Double))
            {
                return true;
            }
            else if (type == typeof(SlimDX.Vector2))
            {
                return true;
            }
            else if (type == typeof(SlimDX.Vector3))
            {
                return true;
            }
            else if (type == typeof(SlimDX.Vector4))
            {
                return true;
            }
            else if (type == typeof(SlimDX.Quaternion))
            {
                return true;
            }
            else if (type == typeof(SlimDX.Matrix))
            {
                return true;
            }
            else if (type == typeof(SlimDX.Matrix3x2))
            {
                return true;
            }
            else if (type == typeof(System.Guid))
            {
                return true;
            }
            else if (type == typeof(System.String))
            {
                return true;
            }
            else if (type == typeof(CSUtility.Support.Color))
            {
                return true;
            }
            else if (type == typeof(System.DateTime))
            {
                return true;
            }
            else if (type.IsEnum)
            {
                return true;
            }

            return false;
        }

        public bool NeedAnalyse(string typeName,string propertyName)
        {
            if (mAnalyseResClassDic.ContainsKey(typeName) && mAnalyseResClassDic[typeName] != null)
            {
                if (mAnalyseResClassDic[typeName].Contains(propertyName))
                    return true;
            }
            return false;
        }
    }
}
