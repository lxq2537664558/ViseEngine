using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CSUtility
{
    /// <summary>
    /// 碰撞类型枚举
    /// </summary>
    public enum enHitFlag
    {
        None = 0,
        HitMeshTriangle = 1 << 0,		    // 检测模型的三角面
        IgnoreMouseLineCheckInGame = 1 << 1,    // 游戏中忽略鼠标LineCheck
    }

    /// <summary>
    /// 平台枚举
    /// </summary>
    public enum enPlatform
    {
        Android,
        IOS,
        Windows,
    }

    /// <summary>
    /// 对象属性设置数据，用于服务器端控制客户端对象属性修改
    /// </summary>
    public class SceneActorPropertyChangeData
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public Guid ActorId = Guid.Empty;
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName = "";
        /// <summary>
        /// 属性值
        /// </summary>
        public string TargetValue = "";
    }

    public class Program
    {
        static string mDownloadServiceUrl = "file://127.0.0.1/Game/0.0.1/";
        public static string DownloadServiceUrl
        {
            get { return mDownloadServiceUrl; }
            set
            {
                mDownloadServiceUrl = value;
                mFullPackageUrl = DownloadServiceUrl + "FullPackageZip/";
            }
        }
        static string mFullPackageUrl = DownloadServiceUrl + "FullPackageZip/";
        public static string FullPackageUrl
        {
            get { return mFullPackageUrl; }
        }

        public enum enDotNetVer
        {
            V2_0,
            V3_0,
            V3_5,
            V4_0,
            V4_5,
        }
        public static string GetDotNetVerStr(enDotNetVer ver)
        {
            switch (ver)
            {
                case enDotNetVer.V2_0:
                    return "v2.0";
                case enDotNetVer.V3_0:
                    return "v3.0";
                case enDotNetVer.V3_5:
                    return "v3.5";
                case enDotNetVer.V4_0:
                    return "v4.0";
                case enDotNetVer.V4_5:
                    return "v4.5";
            }

            return "v4.0";
        }
        public static Guid GetIdFromFile(string fileName)
        {
            fileName = fileName.Replace("/", "\\");
            var idStartIdx = fileName.LastIndexOf("\\");
            var idEndIdx = fileName.LastIndexOf(".");
            var idStr = fileName.Substring(idStartIdx + 1, idEndIdx - idStartIdx - 1);

            return CSUtility.Support.IHelper.GuidTryParse(idStr);
        }
        public static void DrawColor2Vector(CSUtility.Support.Color src, out SlimDX.Vector4 dst)
        {
            dst.X = src.R / 255.0f;
            dst.Y = src.G / 255.0f;
            dst.Z = src.B / 255.0f;
            dst.W = src.A / 255.0f;
        }

        public static void Vector2DrawColor(SlimDX.Vector4 src, out CSUtility.Support.Color dst)
        {
            dst = CSUtility.Support.Color.FromArgb(
                (int)(src.X * 255.0f),
                (int)(src.Y * 255.0f),
                (int)(src.Z * 255.0f));
        }

        #region thread info lock

        public static void InfoLock(object lockObj, System.Diagnostics.StackFrame stack)
        {
            var infoStr = "Lock: obj(" + lockObj.GetType().FullName + ")" + stack.ToString();
            //System.Diagnostics.Trace.WriteLine(infoStr);
            //Log.FileLog.WriteLine(infoStr, true);
            System.Threading.Monitor.Enter(lockObj);
        }
        public static void InfoUnlock(object lockObj, System.Diagnostics.StackFrame stack)
        {
            System.Threading.Monitor.Exit(lockObj);
            var infoStr = "UnLock: obj(" + lockObj.GetType().FullName + ")" + stack.ToString();
            //System.Diagnostics.Trace.WriteLine(infoStr);
            //Log.FileLog.WriteLine(infoStr, true);
        }

#endregion

        /// <summary>
        /// 从文件名获取Assembly
        /// </summary>
        /// <param name="csType">客户端服务器类型</param>
        /// <param name="dllName">dll文件名,支持绝对路径</param>
        /// <param name="relativePath">跟csType对应的相对路径</param>
        /// <returns></returns>
        public static System.Reflection.Assembly GetAssemblyFromDllFileName(CSUtility.Helper.enCSType csType, string dllName, string relativePath = "")
        {
            try
            {
                var assem = GetAnalyseAssembly(csType, CurrentPlatform, dllName);
                if (assem != null)
                    return assem;

                if (!dllName.Contains(".dll"))
                    dllName += ".dll";

                var assemblys = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblys)
                {
                    if (assembly.GetName().Name + ".dll" == dllName)
                        return assembly;
                }

                if (!System.IO.Path.IsPathRooted(dllName))
                {
                    var pathString = "";
                    switch(csType)
                    {
                        case Helper.enCSType.Client:
                            pathString = CSUtility.Support.IFileConfig.Client_Directory + "/";
                            dllName = CSUtility.Support.IFileManager.Instance.Root + pathString + relativePath + dllName;
                            break;
                        case Helper.enCSType.Server:
                            pathString = CSUtility.Support.IFileConfig.Server_Directory + "/";
                            dllName = CSUtility.Support.IFileManager.Instance.Root + pathString + relativePath + dllName;
                            break;
                        case Helper.enCSType.Common:
                        case Helper.enCSType.All:
                            dllName = CSUtility.Support.IFileManager.Instance.Bin + relativePath + dllName;
                            break;
                    }

                }

                return System.Reflection.Assembly.LoadFrom(dllName);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return null;
        }
        public static string GetAppTypeStringFromSaveString(string saveString)
        {
            var type = GetTypeFromSaveString(saveString);
            return GetAppTypeString(type);
        }
        public static string GetAppTypeString(Type type)
        {
            if (type == null)
                return "";

            if (type.IsGenericType)
            {
                string retValue = type.Namespace + "." + type.Name;
                var agTypes = type.GetGenericArguments();
                if (agTypes.Length == 0)
                    return retValue;

                retValue = retValue.Replace("`" + type.GetGenericArguments().Length, "");
                var agStr = "";
                for(int i=0; i<agTypes.Length; i++)
                {
                    if (i == 0)
                        agStr = GetAppTypeString(agTypes[i]);
                    else
                        agStr += "," + GetAppTypeString(agTypes[i]);
                }
                retValue += "<" + agStr + ">";
                return retValue;
            }
            else
                return type.FullName.Replace("+", ".");
        }
        public static string GetTypeSaveString(Type type, CSUtility.Helper.enCSType csType = CSUtility.Helper.enCSType.All)
        {
            var keyName = GetAnalyseAssemblyKeyName(csType, CurrentPlatform, type.Assembly);
            if (string.IsNullOrEmpty(keyName))
                keyName = type.Assembly.GetName().Name + ".dll@";
            else
                keyName += "@";
            if (type.IsGenericType)
            {
                string retValue = keyName + type.Namespace + "." + type.Name;
                var agTypes = type.GetGenericArguments();
                if (agTypes.Length == 0)
                    return retValue;

                var agTypeStr = "";
                for(int i=0; i<agTypes.Length; i++)
                {
                    agTypeStr += "[" + GetTypeSaveString(agTypes[i]) + "]";
                }

                retValue += "[" + agTypeStr + "]";
                return retValue;
            }
            else
                return keyName + type.FullName;
        }
        public static System.Reflection.Assembly GetTypeAssemblyFromSaveString(string str, CSUtility.Helper.enCSType csType = CSUtility.Helper.enCSType.All)
        {
            var idx = str.IndexOf('@');
            if (idx == -1)
                return null;
            var strDll = str.Substring(0, idx);
            return GetAssemblyFromDllFileName(csType, strDll);
        }
        /// <summary>
        /// 获取字符串中指定位置起始字符的对应结束字符位置
        /// </summary>
        /// <param name="startIdx"></param>
        /// <param name="str"></param>
        /// <param name="startChar">起始字符</param>
        /// <param name="endChar">结束字符</param>
        /// <returns></returns>
        private static int GetPairCharPosition(int startIdx, string str, char startChar, char endChar)
        {
            if (string.IsNullOrEmpty(str))
                return -1;

            if (startIdx < 0 || startIdx >= str.Length)
                return -1;
            if (str[startIdx] != startChar)
                return -1;

            int startCount = 0;
            for(int i=startIdx + 1; i<str.Length; i++)
            {
                if (str[i] == startChar)
                    startCount++;
                if(str[i] == endChar)
                {
                    if (startCount == 0)
                        return i;

                    startCount--;
                }
            }

            return -1;
        }

        #region Assembly分析

        class AssemblyPlatformData
        {
            public enPlatform Platform = enPlatform.Windows;

            Dictionary<string, System.Reflection.Assembly> mAssemblyPlatformMapping = new Dictionary<string, Assembly>();
            public Dictionary<string, System.Reflection.Assembly> AssemblyPlatformMapping
            {
                get { return mAssemblyPlatformMapping; }
            }
        }

        // 需要分析的Assembly列表
        static Dictionary<CSUtility.Helper.enCSType, Dictionary<enPlatform, AssemblyPlatformData>> mAnalyseAssemblyDic = new Dictionary<Helper.enCSType, Dictionary<enPlatform, AssemblyPlatformData>>();

        /// <summary>
        /// 当前运行平台
        /// </summary>
        public static enPlatform CurrentPlatform = enPlatform.Windows;

        /// <summary>
        /// 注册需要分析的程序集
        /// </summary>
        /// <param name="csType">程序集类型</param>
        /// <param name="platform">程序集平台</param>
        /// <param name="keyName">程序集注记名(不同平台的相同程序集注记名必须保持一致并且在同平台的不同程序集之间保持唯一)</param>
        /// <param name="assembly">程序集实例</param>
        /// <returns></returns>
        public static bool RegisterAnalyseAssembly(CSUtility.Helper.enCSType csType, enPlatform platform, string keyName, System.Reflection.Assembly assembly)
        {
            if (assembly == null || csType == Helper.enCSType.All)
                return false;

            Dictionary<enPlatform, AssemblyPlatformData> assemblyDic;
            if(!mAnalyseAssemblyDic.TryGetValue(csType, out assemblyDic))
            {
                assemblyDic = new Dictionary<enPlatform, AssemblyPlatformData>();
                mAnalyseAssemblyDic[csType] = assemblyDic;
            }

            AssemblyPlatformData data;
            if(!assemblyDic.TryGetValue(platform, out data))
            {
                data = new AssemblyPlatformData();
                assemblyDic[platform] = data;
            }

            data.AssemblyPlatformMapping[keyName] = assembly;

            return true;
        }
        /// <summary>
        /// 注销需要分析的程序集
        /// </summary>
        /// <param name="csType">服务器客户端类型</param>
        /// <param name="assembly">程序集实例</param>
        /// <returns></returns>
        public static bool UnRegisterAnalyseAssembly(CSUtility.Helper.enCSType csType, enPlatform platform, string keyName)
        {
            Dictionary<enPlatform, AssemblyPlatformData> assemblyDic;
            if (!mAnalyseAssemblyDic.TryGetValue(csType, out assemblyDic))
                return false;

            AssemblyPlatformData data;
            if (!assemblyDic.TryGetValue(platform, out data))
                return false;

            return data.AssemblyPlatformMapping.Remove(keyName);
        }

        /// <summary>
        /// 获取分析的程序集
        /// </summary>
        /// <param name="csType">服务器客户端类型</param>
        /// <returns></returns>
        public static System.Reflection.Assembly[] GetAnalyseAssemblys(CSUtility.Helper.enCSType csType)
        {
            List<System.Reflection.Assembly> retAssemblys = new List<System.Reflection.Assembly>();
            if(csType == Helper.enCSType.All)
            {
                foreach(var asmDic in mAnalyseAssemblyDic.Values)
                {
                    AssemblyPlatformData data;
                    if(asmDic.TryGetValue(CurrentPlatform, out data))
                    {
                        retAssemblys.AddRange(data.AssemblyPlatformMapping.Values);
                    }
                }
            }
            else
            {
                Dictionary<enPlatform, AssemblyPlatformData> assemblyDic;
                if (mAnalyseAssemblyDic.TryGetValue(csType, out assemblyDic))
                {
                    AssemblyPlatformData data;
                    if(assemblyDic.TryGetValue(CurrentPlatform, out data))
                    {
                        retAssemblys.AddRange(data.AssemblyPlatformMapping.Values);
                    }
                }
            }

            return retAssemblys.ToArray();
        }

        public static System.Reflection.Assembly GetAnalyseAssembly(CSUtility.Helper.enCSType csType, enPlatform platform, string keyName)
        {
            if(csType == Helper.enCSType.All)
            {
                foreach(var asmDic in mAnalyseAssemblyDic.Values)
                {
                    AssemblyPlatformData data;
                    if(asmDic.TryGetValue(platform, out data))
                    {
                        System.Reflection.Assembly assembly;
                        if (data.AssemblyPlatformMapping.TryGetValue(keyName, out assembly))
                            return assembly;
                    }
                }
            }
            else
            {
                Dictionary<enPlatform, AssemblyPlatformData> asmDic;
                if (mAnalyseAssemblyDic.TryGetValue(csType, out asmDic))
                {
                    AssemblyPlatformData data;
                    if (asmDic.TryGetValue(platform, out data))
                    {
                        System.Reflection.Assembly assembly;
                        if (data.AssemblyPlatformMapping.TryGetValue(keyName, out assembly))
                            return assembly;
                    }
                }
            }

            return null;
        }

        public static string GetAnalyseAssemblyKeyName(CSUtility.Helper.enCSType csType, enPlatform platform, Assembly assembly)
        {
            if(csType == Helper.enCSType.All)
            {
                foreach(var asmDic in mAnalyseAssemblyDic.Values)
                {
                    AssemblyPlatformData data;
                    if(asmDic.TryGetValue(platform, out data))
                    {
                        foreach(var valueData in data.AssemblyPlatformMapping)
                        {
                            if (valueData.Value == assembly)
                                return valueData.Key;
                        }
                    }
                }
            }
            else
            {
                Dictionary<enPlatform, AssemblyPlatformData> asmDic;
                if (mAnalyseAssemblyDic.TryGetValue(csType, out asmDic))
                {
                    AssemblyPlatformData data;
                    if (asmDic.TryGetValue(platform, out data))
                    {
                        foreach (var valueData in data.AssemblyPlatformMapping)
                        {
                            if (valueData.Value == assembly)
                                return valueData.Key;
                        }
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// 获取分析的程序集中的所有类型
        /// </summary>
        /// <returns></returns>
        public static Type[] GetTypes()
        {
            List<Type> retList = new List<Type>();
            foreach(var asmDic in mAnalyseAssemblyDic.Values)
            {
                AssemblyPlatformData data;
                if(asmDic.TryGetValue(CurrentPlatform, out data))
                {
                    foreach(var assembly in data.AssemblyPlatformMapping.Values)
                    {
                        retList.AddRange(assembly.GetTypes());
                    }
                }
            }

            return retList.ToArray();
        }

        /// <summary>
        /// 获取分析的程序集中的指定服务器客户端类型的所有类型
        /// </summary>
        /// <param name="csType">服务器客户端类型</param>
        /// <returns></returns>
        public static Type[] GetTypes(CSUtility.Helper.enCSType csType)
        {
            List<Type> retList = new List<Type>();
            if(csType == Helper.enCSType.All)
            {
                foreach(var assemDic in mAnalyseAssemblyDic.Values)
                {
                    AssemblyPlatformData data;
                    if(assemDic.TryGetValue(CurrentPlatform, out data))
                    {
                        foreach(var assembly in data.AssemblyPlatformMapping.Values)
                        {
                            retList.AddRange(assembly.GetTypes());
                        }
                    }
                }
            }
            else
            {
                Dictionary<enPlatform, AssemblyPlatformData> assemDic;
                if(mAnalyseAssemblyDic.TryGetValue(csType, out assemDic))
                {
                    AssemblyPlatformData data;
                    if (assemDic.TryGetValue(CurrentPlatform, out data))
                    {
                        foreach (var assembly in data.AssemblyPlatformMapping.Values)
                        {
                            retList.AddRange(assembly.GetTypes());
                        }
                    }
                }
            }

            return retList.ToArray();
        }

        /// <summary>
        /// 获取分析的程序集中的指定服务器客户端类型的所有拥有指定特性类型的类型
        /// </summary>
        /// <param name="csType">服务器客户端类型</param>
        /// <param name="attributeTypeFullName">要搜索的特性类型</param>
        /// <param name="inherit">搜索此成员的继承链以查找这些属性，则为true，否则为false</param>
        /// <returns></returns>
        public static Type[] GetTypes(CSUtility.Helper.enCSType csType, string attributeTypeFullName, bool inherit)
        {
            List<Type> retList = new List<Type>();
            if(csType == Helper.enCSType.All)
            {
                foreach (var assemDic in mAnalyseAssemblyDic.Values)
                {
                    AssemblyPlatformData data;
                    if (assemDic.TryGetValue(CurrentPlatform, out data))
                    {
                        foreach(var assembly in data.AssemblyPlatformMapping.Values)
                        {
                            foreach (var type in assembly.GetTypes())
                            {
                                var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(type, attributeTypeFullName, inherit);
                                if (att == null)
                                    continue;

                                retList.Add(type);
                            }
                        }
                    }
                }
            }
            else
            {
                Dictionary<enPlatform, AssemblyPlatformData> assemDic;
                if (mAnalyseAssemblyDic.TryGetValue(csType, out assemDic))
                {
                    AssemblyPlatformData data;
                    if (assemDic.TryGetValue(CurrentPlatform, out data))
                    {
                        foreach (var assembly in data.AssemblyPlatformMapping.Values)
                        {
                            foreach (var type in assembly.GetTypes())
                            {
                                var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(type, attributeTypeFullName, inherit);
                                if (att == null)
                                    continue;

                                retList.Add(type);
                            }
                        }
                    }
                }
            }
            return retList.ToArray();
        }
        /// <summary>
        /// 获取分析的程序集中的所有拥有指定特性类型的类型
        /// </summary>
        /// <param name="attributeTypeFullName">要搜索的特性类型全名称</param>
        /// <param name="inherit">搜索此成员的继承链以查找这些属性，则为true，否则为false</param>
        /// <returns>获取的类型</returns>
        public static Type[] GetTypes(string attributeTypeFullName, bool inherit)
        {
            return GetTypes(Helper.enCSType.All, attributeTypeFullName, inherit);
        }

        // 类型名称对应类型字典表
        static Dictionary<CSUtility.Helper.enCSType, Dictionary<enPlatform, Dictionary<string, Type>>> mTypeNameDic = new Dictionary<Helper.enCSType, Dictionary<enPlatform, Dictionary<string, Type>>>();
        /// <summary>
        /// 从类型全名称获取类型（不包含Assembly内容）
        /// </summary>
        /// <param name="typeFullName">类型的全名称</param>
        /// <returns>根据名称取得的类型，没有找到则返回null</returns>
        public static Type GetTypeFromTypeFullName(string typeFullName, CSUtility.Helper.enCSType csType = Helper.enCSType.All)
        {
            Dictionary<enPlatform, Dictionary<string, Type>> platFormDic;
            if (!mTypeNameDic.TryGetValue(csType, out platFormDic))
            {
                platFormDic = new Dictionary<enPlatform, Dictionary<string, Type>>();
                mTypeNameDic[csType] = platFormDic;
            }

            Dictionary<string, Type> typeDic;
            if (!platFormDic.TryGetValue(CurrentPlatform, out typeDic))
            {
                typeDic = new Dictionary<string, Type>();
                platFormDic[CurrentPlatform] = typeDic;
            }

            if (typeDic.ContainsKey(typeFullName))
                return typeDic[typeFullName];
            
            if (csType == Helper.enCSType.All)
            {
                foreach (var assemDic in mAnalyseAssemblyDic.Values)
                {
                    AssemblyPlatformData data;
                    if (assemDic.TryGetValue(CurrentPlatform, out data))
                    {
                        foreach (var assembly in data.AssemblyPlatformMapping.Values)
                        {
                            foreach (var type in assembly.GetTypes())
                            {
                                if (type.FullName.Equals(typeFullName))
                                {
                                    typeDic[typeFullName] = type;
                                    return type;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Dictionary<enPlatform, AssemblyPlatformData> assemDic;
                if (mAnalyseAssemblyDic.TryGetValue(csType, out assemDic))
                {
                    AssemblyPlatformData data;
                    if (assemDic.TryGetValue(CurrentPlatform, out data))
                    {
                        foreach (var assembly in data.AssemblyPlatformMapping.Values)
                        {
                            foreach (var type in assembly.GetTypes())
                            {
                                if (type.FullName.Equals(typeFullName))
                                {
                                    typeDic[typeFullName] = type;
                                    return type;
                                }
                            }
                        }
                    }
                }
            }

            var cmType = Type.GetType(typeFullName);
            if (cmType != null)
            {
                typeDic[typeFullName] = cmType;
                return cmType;
            }

            return null;
        }

        // 类继承关系字典
        static Dictionary<CSUtility.Helper.enCSType, Dictionary<enPlatform, Dictionary<string, List<Type>>>> mTypeInheritDic = new Dictionary<Helper.enCSType, Dictionary<enPlatform, Dictionary<string, List<Type>>>>();
        /// <summary>
        /// 获取类的继承类列表
        /// </summary>
        /// <param name="parentTypeString">父类类型全名称，不包含Assembly信息</param>
        /// <returns></returns>
        public static List<Type> GetInheritTypesFromType(string parentTypeString, CSUtility.Helper.enCSType csType)
        {
            if (string.IsNullOrEmpty(parentTypeString))
                return new List<Type>();

            Dictionary<enPlatform, Dictionary<string, List<Type>>> platFormDic;
            if(!mTypeInheritDic.TryGetValue(csType, out platFormDic))
            {
                platFormDic = new Dictionary<enPlatform, Dictionary<string, List<Type>>>();
                mTypeInheritDic[csType] = platFormDic;
            }

            Dictionary<string, List<Type>> typeDic;
            if(!platFormDic.TryGetValue(CurrentPlatform, out typeDic))
            {
                typeDic = new Dictionary<string, List<Type>>();
                platFormDic[CurrentPlatform] = typeDic;
            }

            if (typeDic.ContainsKey(parentTypeString))
                return typeDic[parentTypeString];

            var parentType = GetTypeFromTypeFullName(parentTypeString, csType);
            if (parentType == null)
                return new List<Type>();

            List<Type> retValue = new List<Type>();
            if(csType == Helper.enCSType.All)
            {
                foreach (var assemDic in mAnalyseAssemblyDic.Values)
                {
                    AssemblyPlatformData data;
                    if (assemDic.TryGetValue(CurrentPlatform, out data))
                    {
                        foreach (var assembly in data.AssemblyPlatformMapping.Values)
                        {
                            if (parentType.IsInterface)
                            {
                                foreach (var type in assembly.GetTypes())
                                {
                                    if (type.GetInterface(parentType.FullName) != null)
                                        retValue.Add(type);
                                }
                            }
                            else
                            {
                                foreach (var type in assembly.GetTypes())
                                {
                                    if (type.IsSubclassOf(parentType))
                                        retValue.Add(type);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Dictionary<enPlatform, AssemblyPlatformData> assemDic;
                if (mAnalyseAssemblyDic.TryGetValue(csType, out assemDic))
                {
                    AssemblyPlatformData data;
                    if (assemDic.TryGetValue(CurrentPlatform, out data))
                    {
                        foreach (var assembly in data.AssemblyPlatformMapping.Values)
                        {
                            if (parentType.IsInterface)
                            {
                                foreach (var type in assembly.GetTypes())
                                {
                                    if (type.GetInterface(parentType.FullName) != null)
                                        retValue.Add(type);
                                }
                            }
                            else
                            {
                                foreach (var type in assembly.GetTypes())
                                {
                                    if (type.IsSubclassOf(parentType))
                                        retValue.Add(type);
                                }
                            }
                        }
                    }
                }
            }

            typeDic[parentTypeString] = retValue;
            return retValue;
        }

        #endregion

        public static string GetTypeAppStringFromSaveString(string str, CSUtility.Helper.enCSType csType = CSUtility.Helper.enCSType.All)
        {
            if(str.Contains("["))
            {
                var idxStart = str.IndexOf('[');
                var idxEnd = str.LastIndexOf(']');
                var typeStr = str.Substring(0, idxStart);
                if(typeStr.IndexOf('`') < 0)
                {
                    // array
                    var splits = str.Split('@');
                    return splits[1];
                }
                else
                {
                    // generic
                    var argCount = System.Convert.ToInt32(typeStr.Substring(typeStr.IndexOf('`') + 1));
                    typeStr = GetTypeAppStringFromSaveString(typeStr);
                    var argsStr = str.Substring(idxStart + 1, idxEnd - (idxStart + 1));
                    int startIdx = 0;
                    string argsAppStr = "";
                    for(int i=0; i<argCount; i++)
                    {
                        var endidx = GetPairCharPosition(startIdx, argsStr, '[', ']');
                        if (endidx < 0)
                            return "";

                        var subString = argsStr.Substring(startIdx + 1, endidx - (startIdx + 1));
                        var dllKeyStr = subString.Substring(0, subString.IndexOf('@'));
                        var assem = GetAssemblyFromDllFileName(csType, dllKeyStr);
                        if (assem == null)
                            return "";

                        if(i == 0)
                            argsAppStr += "[" + GetTypeAppStringFromSaveString(subString) + ", " + assem.FullName + "]";
                        else
                            argsAppStr += ",[" + GetTypeAppStringFromSaveString(subString) + ", " + assem.FullName + "]";

                        startIdx = endidx + 1;
                    }

                    typeStr += "[" + argsAppStr + "]";
                }

                return typeStr;
            }
            else
            {
                var splits = str.Split('@');
                return splits[1];
            }

            /*/////////////////////////////////////////
            if (str.Contains("|"))
            {
                var splits = str.Split('|');
                var retStr = "";
                for (int i = splits.Length - 1; i > 0; i--)
                {
                    var agSplits = splits[i].Split(':');
                    for(int j = 0; j < agSplits.Length; j++)// var agStr in agSplits)
                    {
                        var typeSplits = agSplits[j].Split('@');
                        var assem = GetAssemblyFromDllFileName(typeSplits[0]);
                        if (assem == null)
                            return "";

                        if (string.IsNullOrEmpty(retStr))
                            retStr = typeSplits[1] + ", " + assem.FullName;
                        else
                            retStr = typeSplits[1] + "[[" + retStr + "]], " + assem.FullName;
                    }

                    //        retStr = typeSplits[1] + "[[" + retStr + "]], " + assem.FullName;
                }

                var typeSplits0 = splits[0].Split('@');
                retStr = typeSplits0[1] + "[[" + retStr + "]]";

                return retStr;
            }
            else
            {
                var splits = str.Split('@');
                return splits[1];
            }*/
        }
        public static Type GetTypeFromSaveString(string str, CSUtility.Helper.enCSType csType = CSUtility.Helper.enCSType.All)
        {
            var assem = GetTypeAssemblyFromSaveString(str, csType);
            if (assem == null)
                return null;
            var typeStr = GetTypeAppStringFromSaveString(str);
            return assem.GetType(typeStr);
        }

        public static string GetTempKey(UInt32 ver)
        {
            switch (ver)
            {
                case 0:
                    return "ak9";
            }

            return "";
        }

        public static int GetClassHash(System.Type classType, Type propertyAttributeType)
        {
            if (classType == null)
                return 0;

            return GetClassHashString(classType, propertyAttributeType).GetHashCode();
        }
        public static UInt32 GetNewClassHash(System.Type classType, Type propertyAttributeType)
        {
            if (classType == null)
                return 0;

            var hashStr = GetClassHashString(classType, propertyAttributeType);
            return CSUtility.Support.UniHash.DefaultHash(hashStr);
        }
        public static System.String GetClassHashString(System.Type classType, Type propertyAttributeType)
        {
            if (classType == null)
                return "";

            List<System.Reflection.PropertyInfo> propertys = new List<System.Reflection.PropertyInfo>(classType.GetProperties());

            propertys.Sort(new System.Comparison<System.Reflection.PropertyInfo>(ComparePropertyByName));
            // 计算类的哈希值
            System.String typesString = classType.ToString();
            foreach (System.Reflection.PropertyInfo property in propertys)
            {
                System.Object[] proAtts = property.GetCustomAttributes(propertyAttributeType, false);
                if (proAtts.Length == 0)
                    continue;

                string propertyTypeString = property.PropertyType.ToString();
                //if (property.PropertyType.IsEnum)
                //{
                //    foreach (var name in System.Enum.GetNames(property.PropertyType))
                //    {
                //        propertyTypeString += "_" + name;
                //    }
                //}

                if (propertyAttributeType == typeof(CSUtility.Support.DataValueAttribute))
                {
                    var att = proAtts[0] as CSUtility.Support.DataValueAttribute;
                    typesString += propertyTypeString + property.Name + att.Name;
                }
                else
                    typesString += propertyTypeString + property.Name;
            }

            return typesString;
        }
        public static int ComparePropertyByName(System.Reflection.PropertyInfo info1, System.Reflection.PropertyInfo info2)
        {
            var hash1 = CSUtility.Support.UniHash.DefaultHash(info1.Name);//.GetHashCode();
            var hash2 = CSUtility.Support.UniHash.DefaultHash(info2.Name);//.GetHashCode();

            if (hash1 > hash2)
                return 1;
            else if (hash1 < hash2)
                return -1;
            else
                return 0;
        }
        
        public static string GetValuedGUIDString(Guid guid)
        {
            string retString = guid.ToString();
            retString = retString.Replace("-", "_");

            return retString;
        }
        
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                using (var file = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        public static string GetTypeDefaultValueString(Type type)
        {
            if (type == typeof(bool))
                return "false";
            else if (type == typeof(char))
                return "''";
            else if (type == typeof(System.SByte))
                return "0";
            else if (type == typeof(System.Int16))
                return "0";
            else if (type == typeof(System.Int32))
                return "0";
            else if (type == typeof(System.Int64))
                return "0";
            else if (type == typeof(System.Byte))
                return "0";
            else if (type == typeof(System.UInt16))
                return "0";
            else if (type == typeof(System.UInt32))
                return "0";
            else if (type == typeof(System.UInt64))
                return "0";
            else if (type == typeof(System.Single))
                return "0";
            else if (type == typeof(System.Double))
                return "0";
            else if (type == typeof(System.Guid))
                return "System.Guid.Empty";
            else if (type == typeof(System.DateTime))
                return "System.DateTime.Now";
            else if (type == typeof(System.String))
                return "\"\"";
            else if (type == typeof(SlimDX.Vector2))
                return "SlimDX.Vector2.Zero";
            else if (type == typeof(SlimDX.Vector3))
                return "SlimDX.Vector3.Zero";
            else if (type == typeof(SlimDX.Vector4))
                return "new SlimDX.Vector4()";
            else if (type == typeof(SlimDX.Quaternion))
                return "SlimDX.Quaternion.Identity";
            else if (type == typeof(CSUtility.Support.Color))
                return "CSUtility.Support.Color.Black";
            else if (type.IsEnum)
                return "(" + type.FullName.Replace("+", ".") + ")0";
            else if (type.IsArray)
            {
                return "new " + GetAppTypeString(type).Replace("[]", "[0]");
            }

            return "";
        }

        // 将毫秒转为X小时X分中X秒
        public static string FormatTimeString(Int64 millionSecond)
        {
            var timeSpan = System.TimeSpan.FromMilliseconds(millionSecond);
            return ((timeSpan.Hours > 0) ? timeSpan.Hours + "小时" : "") +
                ((timeSpan.Minutes > 0) ? timeSpan.Minutes + "分钟" : "") +
                ((timeSpan.Seconds > 0) ? timeSpan.Seconds + "秒" : "");
        }

        public static bool FinalRelease = false;    // 标示是否是最终发布版本

        //public static AutoResetEvent mAutoEvent = new AutoResetEvent(false);


        public static void LogInfo(string info)
        {
#if ANDROID
            global::Android.Util.Log.Info("vise3d", info);
#endif
        }
    }
}