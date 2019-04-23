using System;
using System.Collections.Generic;

namespace CCore.Support
{
    /// <summary>
    /// GM命令特性，所有打上此标志的静态函数会自动注册到GM指令中
    /// </summary>
    public class GMCommandAttribute : Attribute
    {
        public string Name;
        public string Description;
        public GMCommandAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    /// <summary>
    /// GM命令
    /// </summary>
    public class GMCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 命令说明
        /// </summary>
        public string Description;

        /// <summary>
        /// 执行命令的函数
        /// </summary>
        public System.Reflection.MethodInfo CommandMethod = null;

        public string MethodFullName
        {
            get
            {
                if (CommandMethod == null)
                    return "";

                return $"{CommandMethod.DeclaringType.FullName}.{CommandMethod.Name}";
            }
        }
    }
    /// <summary>
    /// GM命令管理器
    /// </summary>
    public class GMCommandManager
    {
        /// <summary>
        /// GM指令字典表
        /// </summary>
        System.Collections.Concurrent.ConcurrentDictionary<string, GMCommand> mCommands = new System.Collections.Concurrent.ConcurrentDictionary<string, GMCommand>();
        public System.Collections.Concurrent.ConcurrentDictionary<string, GMCommand> Commands
        {
            get { return mCommands; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        GMCommandManager()
        {
            Initialize();
        }

        static GMCommandManager smInstance = new GMCommandManager();
        /// <summary>
        /// 声明该类为单件
        /// </summary>
        public static GMCommandManager Instance
        {
            get { return smInstance; }
        }

        /// <summary>
        /// 初始化GM指令
        /// </summary>
        public void Initialize()
        {
            foreach(var type in CSUtility.Program.GetTypes(CSUtility.Helper.enCSType.Client))
            {
                if (type == null)
                    continue;
                
                var methods = type.GetMethods();
                foreach(var method in methods)
                {
                    var atts = method.GetCustomAttributes(typeof(GMCommandAttribute), true);
                    if (atts.Length <= 0)
                        continue;

                    var cmdAtt = atts[0] as GMCommandAttribute;
                    GMCommand containCmd;
                    if(mCommands.TryGetValue(cmdAtt.Name, out containCmd))
                    {
                        System.Diagnostics.Debug.WriteLine($"GM命令重复: 指令名称{cmdAtt.Name} {method.DeclaringType.FullName}.{method.Name}与{containCmd.MethodFullName}的指令名称重复");
                        continue;
                    }

                    var cmd = new GMCommand()
                    {
                        Name = cmdAtt.Name,
                        Description = cmdAtt.Description,
                        CommandMethod = method,
                    };
                    mCommands[cmdAtt.Name] = cmd;
                }
            }
        }

        /// <summary>
        /// 执行GM命令
        /// </summary>
        /// <param name="cmdName">命令描述</param>
        /// <param name="args">命令参数</param>
        public void GMExecute(string cmdName, object[] args)
        {
            try
            {
                GMCommand exec;
                if (mCommands.TryGetValue(cmdName, out exec))
                {
                    if (exec.CommandMethod == null)
                        return;

                    var parameters = exec.CommandMethod.GetParameters();
                    if (!((args == null && parameters.Length == 0) || (args != null && parameters.Length == args.Length)))
                        return;

                    var argParams = new object[parameters.Length];
                    for(int i=0; i<parameters.Length; i++)
                    {
                        argParams[i] = System.Convert.ChangeType(args[i], parameters[i].ParameterType);
                    }
                    exec.CommandMethod.Invoke(null, argParams);
                }
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GM指令执行失败: " + cmdName + "\r\n" + e.ToString());
            }
        }
    }
}
