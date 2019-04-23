using System;

namespace CSUtility.AISystem.Attribute
{
    //public enum enCSType
    //{
    //    Common,
    //    Server,
    //    Client,
    //    All,
    //}
    /// <summary>
    /// 允许在逻辑图中使用的枚举类
    /// </summary>
    public sealed class AllowEnum : System.Attribute
    {
        /// <summary>
        /// C/S模式枚举
        /// </summary>
        public CSUtility.Helper.enCSType CSType
        {
            get;
            private set;
        }
        /// <summary>
        /// 在节点列表中显示的路径
        /// </summary>
        public string Path
        {
            get;
            private set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
        /// <summary>
        /// 安全等级
        /// </summary>
        public byte SecurityLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// 允许在逻辑图中使用的枚举
        /// </summary>
        /// <param name="path">在节点列表中显示的路径(aa.bb.cc)</param>
        /// <param name="csType">服务器客户端类型，标识此成员是在服务器端还是客户端调用</param>
        /// <param name="description">描述</param>
        /// <param name="securityLevel">安全级别，根据客户权限对能否调用进行筛选</param>
        public AllowEnum(string path, CSUtility.Helper.enCSType csType, string description, byte securityLevel = 0)
        {
            Path = path;
            CSType = csType;
            Description = description;
            SecurityLevel = securityLevel;
        }
    }
    /// <summary>
    /// 接口重载类
    /// </summary>
    public class OverrideInterface : System.Attribute
    {//最好后面要加上调用授权级别，只有调用级别高于指定的应用，才能使用。
        /// <summary>
        /// 服务器客户端类型
        /// </summary>
        public CSUtility.Helper.enCSType CSType
        {
            get;
            set;
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="csType">服务器客户端类型</param>
        public OverrideInterface(CSUtility.Helper.enCSType csType)
        {
            CSType = csType;
        }
    }
    /// <summary>
    /// 允许在逻辑图中使用的状态 
    /// </summary>
    public class AllowStateProperty : System.Attribute
    {//最好后面要加上调用授权级别，只有调用级别高于指定的应用，才能使用。
        /// <summary>
        /// 服务器客户端类型
        /// </summary>
        public CSUtility.Helper.enCSType CSType
        {
            get;
            set;
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="csType">服务器客户端类型</param>
        public AllowStateProperty(CSUtility.Helper.enCSType csType)
        {
            CSType = csType;
        }
    }

    /// <summary>
    /// 允许在AI连线工具中访问的成员
    /// </summary>
    public sealed class AllowMember : System.Attribute
    {
        /// <summary>
        /// 服务器客户端类型
        /// </summary>
        public CSUtility.Helper.enCSType CSType
        {
            get;
            private set;
        }
        /// <summary>
        /// 节点列表中显示的路径
        /// </summary>
        public string Path
        {
            get;
            private set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
        /// <summary>
        /// 安全等级
        /// </summary>
        public byte SecurityLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// 允许在逻辑图中使用的成员(函数、属性等)
        /// </summary>
        /// <param name="path">在节点列表中显示的路径(aa.bb.cc)</param>
        /// <param name="csType">服务器客户端类型，标识此成员是在服务器端还是客户端调用</param>
        /// <param name="description">描述</param>
        /// <param name="securityLevel">安全级别，根据客户权限对能否调用进行筛选</param>
        public AllowMember(string path, CSUtility.Helper.enCSType csType, string description, byte securityLevel = 0)
        {
            Path = path;
            CSType = csType;
            Description = description;
            SecurityLevel = securityLevel;
        }
    }

    /// <summary>
    /// 允许在AI连线工具中访问的类
    /// </summary>
    public sealed class AllowClass : System.Attribute
    {
        /// <summary>
        /// 类类型
        /// </summary>
        public enum enClassType
        {
            /// <summary>
            /// 单件类
            /// </summary>
            Instance,
            /// <summary>
            /// 可以创建的类
            /// </summary>
            New,
        }
        /// <summary>
        /// 服务器客户端类型
        /// </summary>
        public CSUtility.Helper.enCSType CSType
        {
            get;
            private set;
        }
        /// <summary>
        /// 在节点列表中显示的路径
        /// </summary>
        public string Path
        {
            get;
            private set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
        /// <summary>
        /// 类类型
        /// </summary>
        public enClassType ClassType
        {
            get;
            private set;
        }
        /// <summary>
        /// 安全等级
        /// </summary>
        public byte SecurityLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// 允许在逻辑图中直接使用的类
        /// </summary>
        /// <param name="path">在节点列表中显示的路径(aa.bb.cc)</param>
        /// <param name="csType">服务器客户端类型，标识此成员是在服务器端还是客户端调用</param>
        /// <param name="description">描述</param>
        /// <param name="classType">类类型</param>
        /// <param name="securityLevel">安全级别，根据客户权限对能否调用进行筛选</param>
        public AllowClass(string path, CSUtility.Helper.enCSType csType, string description, enClassType classType, byte securityLevel = 0)
        {
            Path = path;
            CSType = csType;
            Description = description;
            ClassType = classType;
            SecurityLevel = securityLevel;
        }
    }
    /// <summary>
    /// 提示工具属性
    /// </summary>
    public class ToolTipAttribute : System.Attribute
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        public string TipString
        {
            get;
            set;
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="strTip">提示信息</param>
        public ToolTipAttribute(string strTip)
        {
            TipString = strTip;
        }
    }
    /// <summary>
    /// 显示名称的属性
    /// </summary>
    public class DisplayNameAttribute : System.Attribute
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="name">名字</param>
        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }
    /// <summary>
    /// 声明类属性类
    /// </summary>
    public sealed class StatementClassAttribute : System.Attribute
    {
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="csType">服务器客户端类型</param>
        public StatementClassAttribute(String strName, CSUtility.Helper.enCSType csType)
        {
            StrName = strName;
            CSType = csType;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public String StrName
        {
            get;
            private set;
        }
        /// <summary>
        /// 服务器客户端类型
        /// </summary>
        public CSUtility.Helper.enCSType CSType
        {
            get;
            private set;
        }
    }
}
