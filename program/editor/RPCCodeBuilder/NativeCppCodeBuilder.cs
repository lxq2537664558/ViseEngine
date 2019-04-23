using System;
using System.Collections.Generic;

using System.Text;


namespace RPCCodeBuilder
{
    namespace NativeRPC
    {
        class ClassInfo
        {
            public string Namespace;
            public string ClassName;
            public string FullName
            {
                get { return Namespace + "." + ClassName; }
                set
                {
                    int len = value.LastIndexOf('.');
                    Namespace = value.Substring(0, len);
                    ClassName = value.Substring(len + 1, value.Length - len - 1);
                }
            }

            public List<MethodInfo> Methods = new List<MethodInfo>();
            public List<ChildObjectInfo> ChildObjects = new List<ChildObjectInfo>();
            public List<IndexerInfo> Indexer = new List<IndexerInfo>();

            public static ClassInfo BuildClassInfo(CSUtility.Support.XmlHolder xml)
            {
                ClassInfo cinfo = new ClassInfo();
                cinfo.FullName = xml.RootNode.FindAttrib("Name").Value;
                List<CSUtility.Support.XmlNode> nodes = xml.RootNode.GetNodes();
                int i = 0;
                foreach (CSUtility.Support.XmlNode c in nodes)
                {
                    if (c.Name == "ChildObject")
                    {
                        ChildObjectInfo cobj = new ChildObjectInfo();
                        cobj.HostClass = cinfo;
                        cobj.Index = System.Convert.ToInt32(c.FindAttrib("Index").Value);
                        cobj.TypeFullName = c.FindAttrib("Type").Value;
                        cobj.Name = c.FindAttrib("Name").Value;
                        cinfo.ChildObjects.Add(cobj);
                    }
                    else if (c.Name == "Method")
                    {
                        MethodInfo mi = new MethodInfo();
                        mi.HostClass = cinfo;
                        mi.Indexer = i++;//System.Convert.ToInt32(c.FindAttrib("Index").Value);
                        mi.ReturnType = c.FindAttrib("ReturnType").Value;
                        mi.Name = c.FindAttrib("Name").Value;
                        List<CSUtility.Support.XmlNode> argnodes = c.GetNodes();
                        foreach (CSUtility.Support.XmlNode anode in argnodes)
                        {
                            Parameter p = new Parameter();
                            p.TypeName = anode.FindAttrib("Type").Value;
                            p.Name = anode.FindAttrib("Name").Value;
                            mi.Parameters.Add(p);
                        }
                        cinfo.Methods.Add(mi);
                    }
                }
                return cinfo;
            }
        }
        class ChildObjectInfo
        {
            public ClassInfo HostClass;
            public int Index;
            public string TypeNamespace;
            public string TypeName;
            public string TypeFullName
            {
                get { return TypeNamespace + "." + TypeName; }
                set
                {
                    int len = value.LastIndexOf('.');
                    TypeNamespace = value.Substring(0, len);
                    TypeName = value.Substring(len + 1, value.Length - len - 1);
                }
            }
            public string Name;
        }
        class Parameter
        {
            public string TypeName;
            public string Name;
        }
        class MethodInfo
        {
            public ClassInfo HostClass;
            public int Indexer;
            public string ReturnType;
            public string Name;
            public List<Parameter> Parameters = new List<Parameter>();

            public UInt32 GetMethodHashCode()
            {
                string hashString = HostClass.FullName + Indexer.ToString() + ReturnType + Name;
                foreach (Parameter p in Parameters)
                {
                    hashString += p.TypeName + p.Name;
                }
                return (UInt32)hashString.GetHashCode();
            }
        }
        class IndexerInfo
        {
            public ClassInfo HostClass = null;
            public int Index = 0;
            public string TypeNamespace = "";
            public string TypeName = "";
            public string Name = "";
            public List<Parameter> Parameters = new List<Parameter>();

            public UInt32 GetIndexerHashCode()
            {
                string hashString = HostClass.FullName + Index.ToString() + TypeNamespace + TypeName + Name;
                foreach (Parameter p in Parameters)
                {
                    hashString += p.TypeName + p.Name;
                }
                return (UInt32)hashString.GetHashCode();
            }
        }
    }
    
    class NativeCppCodeBuilder
    {
#region Tools
        const string EndLine = "\r\n";
        const string TabStr = "    ";
        public System.Type StringToType(string str)
        {
            if (str == "Int8")
                return typeof(System.SByte);
            else if (str == "Int16")
                return typeof(System.Int16);
            else if (str == "Int32")
                return typeof(System.Int32);
            else if (str == "Int64")
                return typeof(System.Int64);
            else if (str == "UInt8")
                return typeof(System.Byte);
            else if (str == "UInt16")
                return typeof(System.UInt16);
            else if (str == "UInt32")
                return typeof(System.UInt32);
            else if (str == "UInt64")
                return typeof(System.UInt64);
            else if (str == "Single")
                return typeof(System.Single);
            else if (str == "Double")
                return typeof(System.Double);
            else if (str == "String")
                return typeof(System.String);
            else if (str == "Guid")
                return typeof(System.Guid);
            return null;
        }

        public static string CSharp2CppType(string strType)
        {
            return strType.Replace(".", "::");
        }
#endregion

        public static string MakeClientClassCode(NativeRPC.ClassInfo cinfo)
        {
            string strOut = "#if CSharpCompliler" + EndLine;
            strOut += "namespace " + cinfo.Namespace + "{" + EndLine;
            strOut += "public class H_" + cinfo.ClassName + EndLine;
            strOut += "{" + EndLine;
            strOut += TabStr + "public static " + cinfo.Namespace + ".H_" + cinfo.ClassName + " smInstance = new " + cinfo.Namespace + ".H_" + cinfo.ClassName + "();" + EndLine;
            foreach (NativeRPC.MethodInfo mi in cinfo.Methods)
            {
                strOut += MakeCallCode(mi);
            }
            foreach (NativeRPC.ChildObjectInfo cobj in cinfo.ChildObjects)
            {
                strOut += MakeChildObjectCode(cobj);
            }
            foreach (NativeRPC.IndexerInfo idx in cinfo.Indexer)
            {
                strOut += MakeIndexObjectCode(idx);
            }
            strOut += "}" + EndLine;
            strOut += "}" + EndLine;

            strOut += "#else" + EndLine;
            strOut += "//以下是C++代码，产生执行器和绑定执行器" + EndLine;

            strOut += EndLine;
            strOut += "namespace CppRPC_ExecuterNamespace{" + EndLine;
            foreach (NativeRPC.MethodInfo mi in cinfo.Methods)
            {
                string code = MakeMethodExecuterCode(mi);
                code = code.Replace("System::", "CppSystem::");
                strOut += code;
            }
            foreach (NativeRPC.IndexerInfo idx in cinfo.Indexer)
            {
                string code = MakeIndexerExecuterCode(idx);
                code = code.Replace("System::", "CppSystem::");
                strOut += code;
            }
            strOut += "}" + EndLine;

            //strOut += MakeClassInfoCreator(cinfo);

            strOut += "#endif" + EndLine;

            return strOut;
        }

        public static string MakeCallCode(NativeRPC.MethodInfo ma)
        {
            var ptIdx = ma.ReturnType.LastIndexOf('.');
            var typeValue = ma.ReturnType.Substring(ptIdx + 1);
            var retType = ma.ReturnType.Replace(typeValue, "H_" + typeValue);

            string strOut = "";
            if (ma.ReturnType == "System.Void")
                strOut += TabStr + "public void " + ma.Name + "(" + "RPC.PackageWriter pkg";
            else
            {
                strOut += TabStr + "public " + retType + " " + ma.Name + "(" + "RPC.PackageWriter pkg";
            }
            for (int i = 0; i < ma.Parameters.Count; i++)
            {
                if (ma.Parameters[i].TypeName == "CppRPC.DataReader")
                    strOut += "," + "RPC.DataWriter " + ma.Parameters[i].Name;
                else
                    strOut += "," + ma.Parameters[i].TypeName + " " + ma.Parameters[i].Name;
            }
            strOut += ")" + EndLine;
            strOut += TabStr + "{" + EndLine;
            for (int i = 0; i < ma.Parameters.Count; i++)
            {
                strOut += TabStr + TabStr + "pkg.Write(" + ma.Parameters[i].Name + ");" + EndLine;
            }
            strOut += TabStr + TabStr + "pkg.SetMethod(" + ma.Indexer + ");" + EndLine;
            if (ma.ReturnType != "System.Void")
            {
                strOut += TabStr + TabStr + "return " + retType + ".smInstance;" + EndLine;
            }
            strOut += TabStr + "}" + EndLine;
            return strOut;
        }

        public static string MakeChildObjectCode(NativeRPC.ChildObjectInfo cobj)
        {
            //FRPC_TestRPC ^ RPCGet_Child0(PackageWriter ^ pkg);
            //{
            //    pkg->PushStack(0);
            //    return mChild0;
            //}
            string strOut = "";
            strOut += TabStr + "public " + cobj.TypeNamespace + ".H_" + cobj.TypeName + " HGet_" + cobj.Name + "(RPC.PackageWriter pkg)" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "pkg.PushStack(" + cobj.Index + ");" + EndLine;
            strOut += TabStr + TabStr + "return " + cobj.TypeNamespace + ".H_" + cobj.TypeName + ".smInstance;" + EndLine;
            strOut += TabStr + "}" + EndLine;
            return strOut;
        }

        public static string MakeIndexObjectCode(NativeRPC.IndexerInfo idx)
        {
            //FRPC_TestRPC ^ RPCGet_Child0(PackageWriter ^ pkg);
            //{
            //    pkg->PushStack(0);
            //    return mChild0;
            //}
            string strCall = "";
            string strArgSerial = "";
            for (int i = 0; i < idx.Parameters.Count; i++)
            {
                strCall += "," + idx.Parameters[i].TypeName + " " + idx.Parameters[i].Name;
                strArgSerial += TabStr + TabStr + "pkg.Write(" + idx.Parameters[i].Name + ");" + EndLine;
            }
            string strOut = "";
            strOut += TabStr + "public H_" + idx.TypeName + " HIndex(RPC.PackageWriter pkg" + strCall + ")" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "pkg.PushStack(11+" + idx.Index + ");" + EndLine;
            strOut += strArgSerial;
            strOut += TabStr + TabStr + "return " + idx.TypeNamespace + ".H_" + idx.TypeName + ".smInstance;" + EndLine;
            strOut += TabStr + "}" + EndLine;
            return strOut;
        }

        public static string MakeIndexerExecuterCode(NativeRPC.IndexerInfo idx)
        {
            //struct HIndex_0xad111111 : public CppRPC::RPCIndexerExecuter
            //{
            //    virtual CppRPC::RPCObject* Execute(CppRPC::RPCObject* obj, CppRPC::PackageProxy& pkg)
            //    {
            //        CppRPC::RPCObject* host = (CppRPC::RPCObject*)obj;
            
            //        System::Guid Index;
            //        pkg.Read(Index);
            //        return host->RPCIndexer(Index);
            //    }
            //}
            string strOut = "";
            strOut += "struct HIndex_" + idx.GetIndexerHashCode() + ": public CppRPC::RPCIndexerExecuter" + EndLine;
            strOut += "{" + EndLine;
            strOut += TabStr + "static HIndex_" + idx.GetIndexerHashCode() + "* Instance(){ static " + "HIndex_" + idx.GetIndexerHashCode() + " obj; return &obj; }" + EndLine;
            strOut += TabStr + "virtual CppRPC::RPCObject* Execute(CppRPC::RPCObject* obj,CppRPC::PackageProxy& pkg)" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + CSharp2CppType(idx.HostClass.FullName) + "* host = (" + CSharp2CppType(idx.HostClass.FullName) + "*)obj;" + EndLine;

            string strCall = "";
            for (int i = 0; i < idx.Parameters.Count; i++)
            {
                strOut += TabStr + TabStr + CSharp2CppType(idx.Parameters[i].TypeName) + " " + idx.Parameters[i].Name + ";" + EndLine;
                strOut += TabStr + TabStr + "pkg.Read(" + idx.Parameters[i].Name + ");" + EndLine;

                if (i == idx.Parameters.Count - 1)
                    strCall += idx.Parameters[i].Name;  
                else
                    strCall += idx.Parameters[i].Name + ",";
            }

            strOut += TabStr + TabStr + "return host->RPCIndexer(" + strCall + ");" + EndLine;

            strOut += TabStr + "}" + EndLine;
            strOut += "};" + EndLine;
            return strOut;
        }

        public static string MakeMethodExecuterCode(NativeRPC.MethodInfo mi)
        {
            string strOut = "";
            strOut += "struct HExe_" + mi.GetMethodHashCode() + ": public CppRPC::RPCMethodExecuter" + EndLine;
            strOut += "{" + EndLine;
            strOut += TabStr + "static HExe_" + mi.GetMethodHashCode() + "* Instance(){ static " + "HExe_" + mi.GetMethodHashCode() + " obj; return &obj; }" + EndLine;
            strOut += TabStr + "virtual void* Execute(CppRPC::RPCObject* obj,CppRPC::PackageProxy& pkg)" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + CSharp2CppType(mi.HostClass.FullName) + "* host = (" + CSharp2CppType(mi.HostClass.FullName) + "*)obj;" + EndLine;

            string strCall = "";
            for (int i = 0; i < mi.Parameters.Count; i++)
            {
                strOut += TabStr + TabStr + CSharp2CppType(mi.Parameters[i].TypeName) + " " + mi.Parameters[i].Name + ";" + EndLine;
                strOut += TabStr + TabStr + "pkg.Read(" + mi.Parameters[i].Name + ");" + EndLine;

                if (i == mi.Parameters.Count - 1)
                    strCall += mi.Parameters[i].Name;
                else
                    strCall += mi.Parameters[i].Name + ",";
            }

            if(mi.ReturnType=="System.Void")
                strOut += TabStr + TabStr + "host->" + mi.Name + "(" + strCall + ");" + EndLine + TabStr + TabStr + "return NULL;" + EndLine;
            else
                strOut += TabStr + TabStr + "return (void*)host->" + mi.Name + "(" + strCall + ");" + EndLine;

            strOut += TabStr + "}" + EndLine;
            strOut += "};" + EndLine;
            return strOut;
        }

        public static string MakeClassInfoCreator(NativeRPC.ClassInfo cinfo)
        {
            //struct ClassInfoCreator_TestCppRPC
            //{
            //	ClassInfoCreator_TestCppRPC()
            //	{
            //		TestCppRPC obj;
            //		RPCClassInfo& cinfo = *obj.GetRPCClassInfo();
            //		cinfo.mChildObjects[0] = offsetof(TestCppRPC,ChildObj0);//(int)((TestCppRPC*)(NULL))->ChildObj0;
            //		cinfo.mMethods.push_back(&HExe_1213132112::Instance);
            //		cinfo.mChildObjects.push_back(&HIndex_1213132112::Instance);
            //	}
            //} obj_TestCppRPC;
            string strObjGetFn = "";
            string strObj = "";
            foreach(NativeRPC.ChildObjectInfo cobj in cinfo.ChildObjects)
            {
                //strObj += TabStr + TabStr + "cinfo.mChildObjects[" + cobj.Index + "] = offsetof(" + CSharp2CppType(cinfo.FullName) + "," + cobj.Name + ");" + EndLine;
                strObj += TabStr + TabStr + "cinfo.mFnChildObjects[" + cobj.Index + "] = Get_" + cobj.Name + ";" + EndLine;
                strObjGetFn += TabStr + "static CppRPC::RPCObject* Get_" + cobj.Name + "(CppRPC::RPCObject* host) { return ((" + CSharp2CppType(cinfo.FullName) + "*)host)->" + cobj.Name + ";}" + EndLine;
            }
            string strIndex = "";
            foreach (NativeRPC.IndexerInfo idx in cinfo.Indexer)
            {
                strObj += TabStr + TabStr + "cinfo.mIndexers[ " + idx.Index + "] = CppRPC_ExecuterNamespace::HIndex_" + idx.GetIndexerHashCode() + "::Instance();" + EndLine;
            }
            string strMethod = "";
            foreach (NativeRPC.MethodInfo mi in cinfo.Methods)
            {
                strObj += TabStr + TabStr + "cinfo.mMethods[ " + mi.Indexer + "] = CppRPC_ExecuterNamespace::HExe_" + mi.GetMethodHashCode() + "::Instance();" + EndLine;
            }
            string strOut = "";
            strOut += "struct " + cinfo.ClassName + (UInt32)cinfo.FullName.GetHashCode() + EndLine;
            strOut += "{" + EndLine;
            
            strOut += strObjGetFn;

            strOut += TabStr + cinfo.ClassName + (UInt32)cinfo.FullName.GetHashCode() + "()" + EndLine;
            strOut += TabStr + "{" + EndLine;
            strOut += TabStr + TabStr + "CppRPC::RPCClassInfo& cinfo = *((" + CSharp2CppType(cinfo.FullName) + "*)NULL)->" + CSharp2CppType(cinfo.FullName) + "::GetRPCClassInfo();" + EndLine;
            strOut += strObj;
            strOut += strIndex;
            strOut += strMethod;
            strOut += TabStr + "}" + EndLine;
            strOut += "} dummyobj_" + (UInt32)cinfo.FullName.GetHashCode() + ";" + EndLine;
            return strOut;
        }
    }
}
