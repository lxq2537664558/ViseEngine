using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CodeDomNode
{
    // 类成员数据
    class ClassMemberData
    {
        /// <summary>
        /// 类类型
        /// </summary>
        public Type ClassType
        {
            get;
            private set;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        public List<PropertyInfo> PropertyInfos
        {
            get;
            private set;
        } = new List<PropertyInfo>();
        /// <summary>
        /// 函数信息
        /// </summary>
        public List<MethodInfo> MethodInfos
        {
            get;
            private set;
        } = new List<MethodInfo>();
        /// <summary>
        /// 继承类
        /// </summary>
        public Dictionary<Type, Type> InheritanceClasses
        {
            get;
            private set;
        } = new Dictionary<Type, Type>();

        public ClassMemberData(Type type, PropertyInfo[] propertyInfos, MethodInfo[] methodInfos)
        {
            ClassType = type;
            PropertyInfos = new List<PropertyInfo>(propertyInfos);
            MethodInfos = new List<MethodInfo>(methodInfos);
        }

        /// <summary>
        /// 计算classType在assemblys中的父类，并将其加入ClassMemberData中
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="assemblys"></param>
        /// <param name="dataDictionary"></param>
        public static void CalculateInheritanceClasses(Type classType, List<Assembly> assemblys, Dictionary<string, CodeDomNode.ClassMemberData> dataDictionary)
        {
            if (classType == null)
                return;

            var baseType = classType.BaseType;
            if (baseType != null && assemblys.Contains(baseType.Assembly))
            {
                if (baseType.FullName == null)
                    return;
                CodeDomNode.ClassMemberData data;
                if (dataDictionary.TryGetValue(baseType.FullName, out data))
                {
                    data.InheritanceClasses[classType] = classType;
                }

                CalculateInheritanceClasses(baseType, assemblys, dataDictionary);
            }

            var interfaceTypes = classType.GetInterfaces();
            foreach (var infType in interfaceTypes)
            {
                if (assemblys.Contains(infType.Assembly))
                {
                    CodeDomNode.ClassMemberData data;
                    if (dataDictionary.TryGetValue(infType.FullName, out data))
                    {
                        data.InheritanceClasses[classType] = classType;
                    }

                    CalculateInheritanceClasses(infType, assemblys, dataDictionary);
                }
            }
        }
    }
}
