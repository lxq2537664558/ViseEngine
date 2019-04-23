using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace RPCCodeBuilder
{
    class MethodDesc
    {
        public System.Reflection.MethodInfo mi = null;
        //public RPC.RPCMethodAttribute rpcAttr = null;
        public System.Type hostType = null;
        public Byte MethodIndex = 0;
        public UInt32 HashcodeOfMethod = 0;

        public bool IsWeakPkg;
        public bool NoClientCall;
    }

    class RPCClassBuilder
    {
        const string EndLine = "\r\n";
        const string TabStr = "    ";
        public static string MakeClientClassCode(TreeViewItem node)
        {
            var rpcType = node.Tag as Type;
            if (rpcType == null)
                return "";

            string strOut = "namespace " + rpcType.Namespace + "{" +EndLine;
            strOut += "public class H_" + rpcType.Name + EndLine;
            strOut += "{" + EndLine;
            strOut += TabStr + "public static " + rpcType.Namespace + ".H_" + rpcType.Name + " smInstance = new " + rpcType.Namespace + ".H_" + rpcType.Name + "();" + EndLine;
            foreach( TreeViewItem childNode in node.Items )
            {
                MethodDesc ma = childNode.Tag as MethodDesc;
                if (ma!=null)
                {
                    strOut += MakeCallCode(ma);
                }
                System.Reflection.PropertyInfo pi = childNode.Tag as System.Reflection.PropertyInfo;
                if (pi != null)
                {
                    var index = Convert.ToInt32(childNode.Name.Remove(0, "ChildIndex".Length));
                    if (pi.Name == "Item")
                    {                        
                        strOut += MakeIndexObjectCode(pi, index);
                    }
                    else
                    {
                        strOut += MakeChildObjectCode(pi, index);
                    }
                }
            }
            strOut += "}" + EndLine;
            strOut += "}" + EndLine;

            return strOut;
        }

        public static string MakeCallCode(MethodDesc desc)
        {
            System.Reflection.MethodInfo ma = desc.mi;
            int Index = desc.MethodIndex;
            string strOut="";
            var counterInfo = string.Format("static RPC.RPCCallerCounter sCCTer_{0} = RPC.RPCEntrance.GetCallerCounter({1},\"{2}\");", desc.mi.Name, desc.HashcodeOfMethod, desc.hostType.FullName+"."+desc.mi.Name);
            strOut += TabStr + counterInfo + EndLine;
            strOut += TabStr + "public void " + ma.Name + "(" + "RPC.PackageWriter pkg" ;
            System.Reflection.ParameterInfo[] parameters = ma.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.FullName == "CSUtility.Net.NetConnection")
                    continue;
                if (parameters[i].ParameterType.FullName == "RPC.RPCForwardInfo")
                    continue;
                if (parameters[i].ParameterType.FullName=="RPC.DataReader")
                    strOut += "," + "RPC.DataWriter " + parameters[i].Name;
                else
                    strOut += "," + parameters[i].ParameterType.FullName + " " + parameters[i].Name;
            }
            strOut += ")" + EndLine;
            strOut += TabStr + "{" + EndLine;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.FullName == "CSUtility.Net.NetConnection")
                    continue;
                if (parameters[i].ParameterType.FullName == "RPC.RPCForwardInfo")
                    continue;
                strOut += TabStr + TabStr + "pkg.Write(" + parameters[i].Name + ");" + EndLine;
            }
            strOut += TabStr + TabStr + "pkg.SetMethod(" + Index + ");" + EndLine;
            var varName = "sCCTer_" + desc.mi.Name;
            //strOut += TabStr + TabStr + varName + ".CallCounter++;" + EndLine;
            //strOut += TabStr + TabStr + varName + ".WriteSize+=pkg.CurPtr();" + EndLine;
            strOut += TabStr + TabStr + "pkg.CallerCounter = " + varName + ";" + EndLine;
            if(desc.IsWeakPkg)
            {
                strOut += TabStr + TabStr + "pkg.SetWeakPkg();" + EndLine;
            }
            strOut += TabStr + "}" + EndLine;
            return strOut;
        }

        public static string MakeChildObjectCode(System.Reflection.PropertyInfo pi,int index)
        {
            //FRPC_TestRPC ^ RPCGet_Child0(PackageWriter ^ pkg);
            //{
            //    pkg->PushStack(0);
            //    return mChild0;
            //}
            string strOut = "";
            string strTypeFullName = pi.PropertyType.Namespace + ".H_" + pi.PropertyType.Name;
            strOut += TabStr + "public " + strTypeFullName + " HGet_" + pi.Name + "(RPC.PackageWriter pkg)" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "pkg.PushStack(" + index + ");" + EndLine;
            strOut += TabStr + TabStr + "return " + strTypeFullName + ".smInstance;" + EndLine;
            strOut += TabStr + "}" + EndLine;
            return strOut;
        }

        public static string MakeIndexObjectCode(System.Reflection.PropertyInfo pi, int index)
        {
            //FRPC_TestRPC ^ RPCGet_Child0(PackageWriter ^ pkg);
            //{
            //    pkg->PushStack(0);
            //    return mChild0;
            //}
            string strCall = "";
            string strArgSerial = "";
            System.Reflection.ParameterInfo[] parameters = pi.GetIndexParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                strCall += "," + parameters[i].ParameterType.FullName + " " + parameters[i].Name;
                strArgSerial += TabStr + TabStr + "pkg.Write(" + parameters[i].Name + ");" + EndLine;
            }
            string strOut = "";
            string strTypeFullName = pi.PropertyType.Namespace + ".H_" + pi.PropertyType.Name;
            strOut += TabStr + "public " + strTypeFullName + " HIndex(RPC.PackageWriter pkg" + strCall + ")" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "pkg.PushStack(11+" + index + ");" + EndLine;
            strOut += strArgSerial;
            strOut += TabStr + TabStr + "return " + strTypeFullName + ".smInstance;" + EndLine;
            strOut += TabStr + "}" + EndLine;
            return strOut;
        }
        
        public static string MakeIndexerExecuterCode(System.Reflection.PropertyInfo pi, Type ctype,bool bServer)
        {
            int LimitLevel = 0;

            var propAtt = CSUtility.Helper.AttributeHelper.GetCustomAttribute(pi, typeof(RPC.RPCIndexObjectAttribute).FullName, false);
            
            if (propAtt != null)
            {
                var limitLevel = (int)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(propAtt, "LimitLevel");                
                LimitLevel = limitLevel;
            }

            string strOut = "";
            strOut += "[RPC.RPCIndexExecuterTypeAttribute(" + RPC.RPCEntrance.GetIndexerHashCode(pi, ctype.FullName) + ",\"" + pi.Name + "\",typeof(HIndex_" + RPC.RPCEntrance.GetIndexerHashCode(pi, ctype.FullName) + "))]" + EndLine;
            strOut += "public class HIndex_" + RPC.RPCEntrance.GetIndexerHashCode(pi, ctype.FullName) + ": RPC.RPCIndexerExecuter" + EndLine;
            strOut += "{" + EndLine;
            strOut += TabStr + "public override int LimitLevel" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "get { return " + LimitLevel + "; }" + EndLine;
            strOut += TabStr + "}" + EndLine;
            strOut += TabStr + "public override RPC.RPCObject Execute(RPC.RPCObject obj,RPC.PackageProxy pkg)" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + ctype.FullName + " host = obj as " + ctype.FullName + ";" + EndLine;
            strOut += TabStr + TabStr + "if (host == null) return null;" + EndLine;

            var csType = bServer ? CSUtility.Helper.enCSType.Server : CSUtility.Helper.enCSType.Client;
            var tempType = CSUtility.Program.GetTypeFromTypeFullName(typeof(RPC.IAutoSaveAndLoad).FullName, csType);

            string strCall = "";
            System.Reflection.ParameterInfo[] parameters = pi.GetIndexParameters();
            for (int i = 0; i < parameters.Length; i++)
            {                
                if (parameters[i].ParameterType.IsSubclassOf(tempType))
                {
                    strOut += TabStr + TabStr + parameters[i].ParameterType.FullName + " " + parameters[i].Name + " = new " + parameters[i].ParameterType.FullName + "();" + EndLine;
                    strOut += TabStr + TabStr + "pkg.Read( " + parameters[i].Name + ");" + EndLine;
                }
                else
                {
                    strOut += TabStr + TabStr + parameters[i].ParameterType.FullName + " " + parameters[i].Name + ";" + EndLine;
                    strOut += TabStr + TabStr + "pkg.Read(out " + parameters[i].Name + ");" + EndLine;
                }

                if (i == parameters.Length - 1)
                    strCall += parameters[i].Name;
                else
                    strCall += parameters[i].Name + ",";
            }

            strOut += TabStr + TabStr + "return host[" + strCall + "];" + EndLine;

            strOut += TabStr + "}" + EndLine;
            strOut += "}" + EndLine;
            return strOut;
        }

        public static string MakeMethodExecuterCode(MethodDesc desc,bool bServer)
        {
            System.Reflection.MethodInfo mi = desc.mi;
            System.Type ctype = desc.hostType;

            int LimitLevel = 0;

            object[] propAttrs = mi.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), false);
            if (propAttrs != null && propAttrs.Length == 1)
            {
                RPC.RPCMethodAttribute propAtt = propAttrs[0] as RPC.RPCMethodAttribute;
                if (propAtt != null)
                    LimitLevel = propAtt.LimitLevel;
            }

            string strOut = "";
            strOut += "[RPC.RPCMethordExecuterTypeAttribute(" + RPC.RPCEntrance.GetMethodHashCode(mi, ctype.FullName) + ",\"" + mi.Name + "\",typeof(HExe_" + RPC.RPCEntrance.GetMethodHashCode(mi, ctype.FullName) + "))]" + EndLine;
            strOut += "public class HExe_" + RPC.RPCEntrance.GetMethodHashCode(mi, ctype.FullName) + ": RPC.RPCMethodExecuter" + EndLine;
            strOut += "{" + EndLine;
            strOut += TabStr + "static RPC.RPCMethodAttribute RPCMethodAttr = null;" + EndLine;
            strOut += TabStr + "static System.Reflection.MethodInfo Method = null;" + EndLine;
            strOut += TabStr + "public override int LimitLevel" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "get { return " + LimitLevel + "; }" + EndLine;
            strOut += TabStr + "}" + EndLine;
            strOut += TabStr + "public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + ctype.FullName + " host = obj as " + ctype.FullName + ";" + EndLine;
            strOut += TabStr + TabStr + "if (host == null) return null;" + EndLine;

            var csType = bServer ? CSUtility.Helper.enCSType.Server : CSUtility.Helper.enCSType.Client;
            var tempType = CSUtility.Program.GetTypeFromTypeFullName(typeof(RPC.IAutoSaveAndLoad).FullName, csType);

            string strCall = "";
            System.Reflection.ParameterInfo[] parameters = mi.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                string finalName = parameters[i].Name;

                if (parameters[i].ParameterType.FullName == "CSUtility.Net.NetConnection")
                {
                    finalName = "connect";
                }
                else if (parameters[i].ParameterType.FullName == "RPC.RPCForwardInfo")
                {
                    finalName = "fwd";
                }
                else if (parameters[i].ParameterType.IsSubclassOf(tempType))
                {
                    strOut += TabStr + TabStr + parameters[i].ParameterType.FullName + " " + parameters[i].Name + " = new " + parameters[i].ParameterType.FullName + "();" + EndLine;
                    strOut += TabStr + TabStr + "pkg.Read( " + parameters[i].Name + ");" + EndLine;
                }
                else
                {
                    strOut += TabStr + TabStr + parameters[i].ParameterType.FullName + " " + parameters[i].Name + ";" + EndLine;
                    strOut += TabStr + TabStr + "pkg.Read(out " + parameters[i].Name + ");" + EndLine;
                }

                if (i == parameters.Length - 1)
                    strCall += finalName;
                else
                    strCall += finalName + ",";
            }

            strOut += TabStr + TabStr + "TotalReadSize += pkg.CurPtr();" + EndLine;
            strOut += TabStr + TabStr + "TotalCallCount++;" + EndLine;

            strOut += TabStr + TabStr + "if (RPCMethodAttr == null)" + EndLine;
            strOut += TabStr + TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + TabStr + string.Format("Method = typeof({0}).GetMethod(\"{1}\");", ctype.FullName, mi.Name) + EndLine;
            strOut += TabStr + TabStr + TabStr + "var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);" + EndLine;
            strOut += TabStr + TabStr + TabStr + "RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;" + EndLine;
            strOut += TabStr + TabStr + "}" + EndLine;
            strOut += TabStr + TabStr + "if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)" + EndLine;
            strOut += TabStr + TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + TabStr + string.Format("var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, \"{0}\");", ctype.FullName + ":" + mi.Name) + EndLine;
            strOut += TabStr + TabStr + TabStr + "if (isValid==false)" + EndLine;
            strOut += TabStr + TabStr + TabStr + TabStr + "return null;" + EndLine;
            strOut += TabStr + TabStr + "}" + EndLine;

            if(mi.ReturnType==typeof(void))
                strOut += TabStr + TabStr + "host." + mi.Name + "(" + strCall + ");" + EndLine + "return null;" + EndLine;
            else
                strOut += TabStr + TabStr + "return host." + mi.Name + "(" + strCall + ");" + EndLine;

            strOut += TabStr + "}" + EndLine;
            strOut += "}" + EndLine;
            return strOut;
        }

        public static string MakeMethodIndexRegisterCode(List<object> curList, string modeName)
        {
            string NewLine = "\r\n";
            string TabStr = "\t";
            string code = "public class " + modeName + NewLine;
            code += "{" + NewLine;
            code += TabStr + "static RPC.RPCVersionManager smManager = null;" + NewLine;
            code += TabStr + "public static RPC.RPCVersionManager GetManager()" + NewLine;
            code += TabStr + "{" + NewLine;
            code += TabStr + TabStr + "if (smManager!=null)" + NewLine;
            code += TabStr + TabStr + TabStr + "return smManager;" + NewLine;
            code += TabStr + TabStr + "smManager = new RPC.RPCVersionManager();" + NewLine;

            string regCode = "";
            foreach (var i in curList)
            {
                var fieldClassName = i.GetType().GetField("ClassName");
                var fieldHashCode = i.GetType().GetField("HashCode");
                var fieldIndex = i.GetType().GetField("Index");
                var fieldFullName = i.GetType().GetField("FullName");

                regCode += TabStr + TabStr + string.Format("smManager.RegMethodIndex(\"{0}\", {1}, {2}, \"{3}\");", 
                                                            fieldClassName.GetValue(i), 
                                                            fieldHashCode.GetValue(i), 
                                                            fieldIndex.GetValue(i),
                                                            fieldFullName.GetValue(i)) + NewLine;
            }

            code += regCode;

            code += TabStr + TabStr + string.Format("smManager.RPCHashCode = {0};", regCode.GetHashCode()) + NewLine;

            code += TabStr + TabStr + "return smManager;" + NewLine;

            code += TabStr + "}" + NewLine;
            code += "}" + NewLine;
            return code;
        }
#region CppCodeBuilder
        public static string CSharp2CppType(string strType)
        {
            return strType.Replace(".", "::");
        }

        public static string BeginCppNamespaceDeclare(string strNamespace)
        {
            if (strNamespace == "")
                return "";
            string strOut = "";
            int start = 0;
            int pos = strNamespace.IndexOf('.', start);
            while (pos >= 0)
            {
                int endPos = pos;
                strOut += "namespace " + strNamespace.Substring(start, endPos - start) + "{ ";
                start = pos + 1;
                pos = strNamespace.IndexOf('.', start);
            }
            strOut += "namespace " + strNamespace.Substring(start, strNamespace.Length - start) + "{";

            strOut += EndLine;
            return strOut;
        }

        public static string EndCppNamespaceDeclare(string strNamespace)
        {
            if (strNamespace == "")
                return "";
            string strOut = "";
            int start = 0;
            int pos = strNamespace.IndexOf('.', start);
            while (pos >= 0)
            {
                strOut += "} ";
                start = pos + 1;
                pos = strNamespace.IndexOf('.', start);
            }
            strOut += "} ";
            
            strOut += EndLine;
            return strOut;
        }

        public static string MakeClientCppClassDeclareCodePure(TreeViewItem node)
        {
            RPC.RPCClassAttribute ca = node.Tag as RPC.RPCClassAttribute;
            if (ca == null)
                return "";

            string strOut = BeginCppNamespaceDeclare(ca.RPCType.Namespace);
            strOut += "struct H_" + ca.RPCType.Name + ";" + EndLine;
            strOut += EndCppNamespaceDeclare(ca.RPCType.Namespace);
            return strOut;
        }

        public static string MakeClientCppClassDeclareCode(TreeViewItem node)
        {
            RPC.RPCClassAttribute ca = node.Tag as RPC.RPCClassAttribute;
            if (ca == null)
                return "";

            string strOut = BeginCppNamespaceDeclare(ca.RPCType.Namespace);
            strOut += "struct H_" + ca.RPCType.Name + EndLine;
            strOut += "{" + EndLine;
            strOut += TabStr + "static " + CSharp2CppType(ca.RPCType.Namespace) + "::H_" + ca.RPCType.Name + "* smInstance();" + EndLine;
            foreach (TreeViewItem childNode in node.Items)
            {
                MethodDesc ma = childNode.Tag as MethodDesc;
                if (ma != null)
                {
                    strOut += MakeCppCallCodeDeclare(ma);
                }
                System.Reflection.PropertyInfo pi = childNode.Tag as System.Reflection.PropertyInfo;
                if (pi != null)
                {
                    if (pi.Name == "Item")
                    {
                        strOut += MakeCppIndexObjectCodeDeclare(pi, System.Convert.ToInt32(childNode.Name));
                    }
                    else
                    {
                        strOut += MakeCppChildObjectCodeDeclare(pi, System.Convert.ToInt32(childNode.Name));
                    }
                }
            }
            strOut += "};" + EndLine;
            strOut += EndCppNamespaceDeclare(ca.RPCType.Namespace);
            return strOut;
        }

        public static string MakeClientCppClassCode(TreeViewItem node)
        {
            RPC.RPCClassAttribute ca = node.Tag as RPC.RPCClassAttribute;
            if (ca == null)
                return "";

            string strOut = "";//= BeginCppNamespaceDeclare(ca.RPCType.Namespace);
            string strClassName = CSharp2CppType(ca.RPCType.Namespace) + "::H_" + ca.RPCType.Name;
            strOut += TabStr + strClassName + "* " + strClassName + "::smInstance(){ static " +
                                            strClassName + " gInst; return &gInst;}" + EndLine;
            foreach (TreeViewItem childNode in node.Items)
            {
                MethodDesc ma = childNode.Tag as MethodDesc;
                if (ma != null)
                {
                    strOut += MakeCppCallCode(ma,strClassName);
                }
                System.Reflection.PropertyInfo pi = childNode.Tag as System.Reflection.PropertyInfo;
                if (pi != null)
                {
                    var index = Convert.ToInt32(childNode.Name.Remove(0, "ChildIndex".Length));
                    if (pi.Name == "Item")
                    {
                        strOut += MakeCppIndexObjectCode(pi, System.Convert.ToInt32(childNode.Name), strClassName);
                    }
                    else
                    {
                        strOut += MakeCppChildObjectCode(pi, System.Convert.ToInt32(childNode.Name), strClassName);
                    }
                }
            }
            strOut += "";//EndCppNamespaceDeclare(ca.RPCType.Namespace);

            return strOut;
        }

        public static string MakeCppCallCodeDeclare(MethodDesc desc)
        {
            if (desc.NoClientCall)
                return "";
            System.Reflection.MethodInfo ma = desc.mi;
            int Index = desc.MethodIndex;
            string strOut = "";
            strOut += TabStr + "void " + ma.Name + "(" + "CppRPC::PackageWriter& pkg";
            System.Reflection.ParameterInfo[] parameters = ma.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.FullName == "RPC.DataReader")
                    strOut += "," + "CppRPC::DataWriter& " + parameters[i].Name;
                else if (parameters[i].ParameterType.FullName == "CSUtility.Net.NetConnection")
                    strOut += "";
                else if (parameters[i].ParameterType.FullName == "RPC.RPCForwardInfo")
                    strOut += "";
                else
                    strOut += "," + CSharp2CppType(parameters[i].ParameterType.FullName) + " " + parameters[i].Name;
            }
            strOut += ");" + EndLine;
            return strOut;
        }

        public static string MakeCppCallCode(MethodDesc desc,string strCName)
        {
            if (desc.NoClientCall)
                return "";
            System.Reflection.MethodInfo ma = desc.mi;
            int Index = desc.MethodIndex;
            string strOut = "";
            strOut += TabStr + "void " + strCName + "::" + ma.Name + "(" + "CppRPC::PackageWriter& pkg";
            System.Reflection.ParameterInfo[] parameters = ma.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.FullName == "RPC.DataReader")
                    strOut += "," + "CppRPC::DataWriter& " + parameters[i].Name;
                else if (parameters[i].ParameterType.FullName == "CSUtility.Net.NetConnection")
                    strOut += "";
                else if (parameters[i].ParameterType.FullName == "RPC.RPCForwardInfo")
                    strOut += "";
                else
                    strOut += "," + CSharp2CppType(parameters[i].ParameterType.FullName) + " " + parameters[i].Name;
            }
            strOut += ")" + EndLine;
            strOut += TabStr + "{" + EndLine;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.FullName != "CSUtility.Net.NetConnection" && parameters[i].ParameterType.FullName != "RPC.RPCForwardInfo")
                    strOut += TabStr + TabStr + "pkg.Write(" + parameters[i].Name + ");" + EndLine;
            }
            strOut += TabStr + TabStr + "pkg.SetMethod(" + Index + ");" + EndLine;
            strOut += TabStr + "};" + EndLine;
            return strOut;
        }

        public static string MakeCppChildObjectCodeDeclare(System.Reflection.PropertyInfo pi, int index)
        {
            //FRPC_TestRPC ^ RPCGet_Child0(PackageWriter ^ pkg);
            //{
            //    pkg->PushStack(0);
            //    return mChild0;
            //}
            string strOut = "";
            string strFullTypeName = pi.PropertyType.Namespace + ".H_" + pi.PropertyType.Name;
            strOut += TabStr + CSharp2CppType(strFullTypeName) + "* HGet_" + pi.Name + "(CppRPC::PackageWriter& pkg);" + EndLine;
            return strOut;
        }

        public static string MakeCppChildObjectCode(System.Reflection.PropertyInfo pi, int index, string strCName)
        {
            //FRPC_TestRPC ^ RPCGet_Child0(PackageWriter ^ pkg);
            //{
            //    pkg->PushStack(0);
            //    return mChild0;
            //}
            string strOut = "";
            string strFullTypeName = pi.PropertyType.Namespace + ".H_" + pi.PropertyType.Name;
            strOut += TabStr + CSharp2CppType(strFullTypeName) + "* " + strCName + "::HGet_" + pi.Name + "(CppRPC::PackageWriter& pkg)" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "pkg.PushStack(" + index + ");" + EndLine;
            strOut += TabStr + TabStr + "return " + CSharp2CppType(strFullTypeName) + "::smInstance();" + EndLine;
            strOut += TabStr + "}" + EndLine;
            return strOut;
        }

        public static string MakeCppIndexObjectCodeDeclare(System.Reflection.PropertyInfo pi, int index)
        {
            //FRPC_TestRPC ^ RPCGet_Child0(PackageWriter ^ pkg);
            //{
            //    pkg->PushStack(0);
            //    return mChild0;
            //}
            string strCall = "";
            System.Reflection.ParameterInfo[] parameters = pi.GetIndexParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                strCall += "," + CSharp2CppType(parameters[i].ParameterType.FullName) + " " + parameters[i].Name;
            }
            string strOut = "";
            string strFullTypeName = pi.PropertyType.Namespace + ".H_" + pi.PropertyType.Name;
            strOut += TabStr + CSharp2CppType(strFullTypeName) + "* HIndex_" + pi.Name + "(CppRPC::PackageWriter& pkg" + strCall + ");" + EndLine;
            return strOut;
        }

        public static string MakeCppIndexObjectCode(System.Reflection.PropertyInfo pi, int index, string strCName)
        {
            //FRPC_TestRPC ^ RPCGet_Child0(PackageWriter ^ pkg);
            //{
            //    pkg->PushStack(0);
            //    return mChild0;
            //}
            string strCall = "";
            string strArgSerial = "";
            string strFullTypeName = pi.PropertyType.Namespace + ".H_" + pi.PropertyType.Name;
            System.Reflection.ParameterInfo[] parameters = pi.GetIndexParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                strCall += "," + CSharp2CppType(parameters[i].ParameterType.FullName) + " " + parameters[i].Name;
                strArgSerial += TabStr + TabStr + "pkg.Write(" + parameters[i].Name + ");" + EndLine;
            }
            string strOut = "";
            strOut += TabStr + CSharp2CppType(strFullTypeName) + "* " + strCName + "::HIndex_" + pi.Name + "(CppRPC::PackageWriter& pkg" + strCall + ")" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "pkg.PushStack(11+" + index + ");" + EndLine;
            strOut += strArgSerial;
            strOut += TabStr + TabStr + "return " + CSharp2CppType(strFullTypeName) + "::smInstance();" + EndLine;
            strOut += TabStr + "}" + EndLine;
            return strOut;
        }
#endregion
    }
}
