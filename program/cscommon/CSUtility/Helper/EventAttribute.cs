namespace CSUtility.Event.Attribute
{
    public sealed class AllowEnum : System.Attribute
    {
        public CSUtility.Helper.enCSType CSType
        {
            get;
            private set;
        }

        public string Path
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

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

        public CSUtility.Helper.enCSType CSType
        {
            get;
            private set;
        }

        public string Path
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }
        
        public enClassType ClassType
        {
            get;
            private set;
        }

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

    public sealed class AllowMember : System.Attribute
    { 
        public CSUtility.Helper.enCSType CSType
        {
            get;
            private set;
        }

        public string Path
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

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

    //public class AllowMethod : System.Attribute
    //{//最好后面要加上调用授权级别，只有调用级别高于指定的应用，才能使用。
    //    public CSUtility.Helper.enCSType CSType;

    //    /// <summary>
    //    /// 允许在逻辑图中使用的函数
    //    /// </summary>
    //    /// <param name="path"></param>
    //    /// <param name="csType"></param>
    //    public AllowMethod(string path, CSUtility.Helper.enCSType csType)
    //    {
    //        CSType = csType;
    //    }
    //}

    public class ToolTipAttribute : System.Attribute
    {
        public string TipString;

        public ToolTipAttribute(string strTip)
        {
            TipString = strTip;
        }
    }
}
