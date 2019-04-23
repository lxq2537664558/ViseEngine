using System;
using System.Windows;

namespace DelegateMethodEditor.CodeGenerator
{
    //public static class XXX
    //{
    //    public static CodeGenerator.FOnXXX methodCall = XXX.AAA;
    //    public static bool AAA()
    //    {
    //        return false;
    //    }
    //}

    public class CodeGenerator
    {
        //public delegate bool FOnXXX();

        static bool bCompileDebug = true;
        static string mTargetDir = "";

        public static System.IO.TextWriter GenerateCode(CSUtility.Helper.EventCallBack ecb, NodesContainerControl ctrl, CSUtility.Helper.enCSType csType)
        {
            try
            {
                if (!ctrl.ContainLinkNodes)
                {
                    System.IO.TextWriter rtw = new System.IO.StringWriter();
                    rtw.WriteLine("// Vise Engine!\n");
                    rtw.WriteLine("// 逻辑图生成代码");

                    return rtw;
                }

                System.CodeDom.Compiler.CodeDomProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
                System.CodeDom.CodeNamespace nameSpace = new System.CodeDom.CodeNamespace("EventCallBack");

                System.CodeDom.Compiler.CodeGeneratorOptions option = new System.CodeDom.Compiler.CodeGeneratorOptions();
                option.BlankLinesBetweenMembers = false;
                option.BracingStyle = "C";
                option.IndentString = "    ";
                option.ElseOnClosing = false;
                option.VerbatimOrder = true;

                var className = "Class_" + Program.GetValuedGUIDString(ecb.Id);
                System.CodeDom.CodeTypeDeclaration stateClass = new System.CodeDom.CodeTypeDeclaration(className);
                stateClass.IsClass = true;
                nameSpace.Types.Add(stateClass);

                System.CodeDom.CodeMemberField eventField = new System.CodeDom.CodeMemberField(ecb.CBType.FullName, "EventCall");
                eventField.Attributes = System.CodeDom.MemberAttributes.Static | System.CodeDom.MemberAttributes.Public;
                eventField.InitExpression = new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeTypeReferenceExpression(className), "Invoke");
                stateClass.Members.Add(eventField);

                foreach (var origionNode in ctrl.OrigionNodeControls)
                {
                    if (origionNode is CodeDomNode.MethodNode)
                    {
                        origionNode.GCode_CodeDom_GenerateCode(stateClass, null);
                        break;
                    }
                }

                System.IO.TextWriter tw = new System.IO.StringWriter();
                tw.WriteLine("// Vise Engine!\n");
                tw.WriteLine("// 逻辑图生成代码");

                codeProvider.GenerateCodeFromNamespace(nameSpace, tw, option);

                return tw;
            }
            catch (System.Exception ex)
            {
                EditorCommon.MessageBox.Show(ex.ToString());
            }

            System.IO.TextWriter retTw = new System.IO.StringWriter();
            retTw.WriteLine("// Vise Engine!\n");
            retTw.WriteLine("// 逻辑图生成代码错误!");
            return retTw;
        }

        public static System.IO.TextWriter GenerateCode(CSUtility.Helper.EventCallBack ecb, CSUtility.Helper.enCSType csType)
        {
            var baseDir = CSUtility.Support.IFileConfig.DefaultEventDirectory + "\\" + ecb.Id.ToString();
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(baseDir + "\\" + ecb.Id.ToString() + ".xml");

            if (xmlHolder == null)
            {
                System.IO.TextWriter rTw = new System.IO.StringWriter();
                rTw.WriteLine("// Vise Engine!\n");
                rTw.WriteLine("// 逻辑图没有代码生成!");
                return rTw;
            }

            bool bCommonHasLinks = false, bClientHasLinks = false, bServerHasLinks = false;
            var att = xmlHolder.RootNode.FindAttrib("CommonHasLinks");
            if (att != null)
            {
                bCommonHasLinks = System.Convert.ToBoolean(att.Value);
            }
            att = xmlHolder.RootNode.FindAttrib("ClientHasLinks");
            if (att != null)
            {
                bClientHasLinks = System.Convert.ToBoolean(att.Value);
            }
            att = xmlHolder.RootNode.FindAttrib("ServerHasLinks");
            if (att != null)
            {
                bServerHasLinks = System.Convert.ToBoolean(att.Value);
            }

            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    {
                        if (bClientHasLinks)
                            xmlHolder = CSUtility.Support.XmlHolder.LoadXML(baseDir + "\\" + EventListItem.GetFileName(ecb.Id, csType));
                        else if (bCommonHasLinks)
                            xmlHolder = CSUtility.Support.XmlHolder.LoadXML(baseDir + "\\" + EventListItem.GetFileName(ecb.Id, CSUtility.Helper.enCSType.Common));
                        else
                            xmlHolder = null;
                    }
                    break;

                case CSUtility.Helper.enCSType.Server:
                    {
                        if (bServerHasLinks)
                            xmlHolder = CSUtility.Support.XmlHolder.LoadXML(baseDir + "\\" + EventListItem.GetFileName(ecb.Id, csType));
                        else if (bCommonHasLinks)
                            xmlHolder = CSUtility.Support.XmlHolder.LoadXML(baseDir + "\\" + EventListItem.GetFileName(ecb.Id, CSUtility.Helper.enCSType.Common));
                        else
                            xmlHolder = null;
                    }
                    break;
            }

            if (xmlHolder == null)
            {
                System.IO.TextWriter rTw = new System.IO.StringWriter();
                rTw.WriteLine("// Vise Engine!\n");
                rTw.WriteLine("// 逻辑图没有代码生成!");
                return rTw;
            }

            //return System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
            //            new Delegate_LoadNodeControlAndDoGenerateCode(LoadNodeControlAndDoGenerateCode),
            //            System.Windows.Threading.DispatcherPriority.Normal,
            //            new object[] { xmlHolder, ecb, csType }
            //    ) as System.IO.TextWriter;
            return LoadNodeControlAndDoGenerateCode(xmlHolder, ecb, csType);
        }

        private delegate System.IO.TextWriter Delegate_LoadNodeControlAndDoGenerateCode(CSUtility.Support.XmlHolder xmlHolder, CSUtility.Helper.EventCallBack ecb, CSUtility.Helper.enCSType csType);
        private static System.IO.TextWriter LoadNodeControlAndDoGenerateCode(CSUtility.Support.XmlHolder xmlHolder, CSUtility.Helper.EventCallBack ecb, CSUtility.Helper.enCSType csType)
        {
            NodesContainerControl ctrl = new NodesContainerControl();
            ctrl.LoadXML(xmlHolder);

            return GenerateCode(ecb, ctrl, csType);
        }

        public static System.CodeDom.Compiler.CompilerResults CompileCode(string codeStr, CSUtility.Helper.enCSType csType, Guid eventId, string dllOutputFile = "", bool debug = false)
        {
            System.CodeDom.Compiler.CodeDomProvider cdProvider = new Microsoft.CSharp.CSharpCodeProvider();

            System.CodeDom.Compiler.CompilerParameters compilerParam = new System.CodeDom.Compiler.CompilerParameters();
            compilerParam.GenerateExecutable = false;
            compilerParam.GenerateInMemory = false;

            compilerParam.ReferencedAssemblies.Add("System.dll");            

            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    compilerParam.ReferencedAssemblies.Add(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Client_Directory + "/ClientCommon.dll");
                    compilerParam.ReferencedAssemblies.Add(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Client_Directory + "/Client.dll");
                    break;

                case CSUtility.Helper.enCSType.Server:
                    compilerParam.ReferencedAssemblies.Add(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Server_Directory + "/ServerCommon.dll");
                    compilerParam.ReferencedAssemblies.Add(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Server_Directory + "/Server.dll");
                    break;
            }

            if (!string.IsNullOrEmpty(dllOutputFile))
                compilerParam.OutputAssembly = dllOutputFile;

            System.CodeDom.Compiler.CompilerResults compilerResult = null;

            if (debug == true)
            {
                compilerParam.IncludeDebugInformation = true;

                var fileDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultEventDirectory + "\\" + Program.CodeFilesFolderName;
                string fileName = fileDir + "\\" + eventId.ToString() + "_" + csType.ToString() + ".cs";

                if (!System.IO.Directory.Exists(fileDir))
                    System.IO.Directory.CreateDirectory(fileDir);
                using (var tw = new System.IO.StreamWriter(fileName))
                {
                    tw.Write(codeStr);
                }
                fileName = fileName.Replace("/", "\\");
                compilerResult = cdProvider.CompileAssemblyFromFile(compilerParam, new string[] { fileName });
            }
            else
                compilerResult = cdProvider.CompileAssemblyFromSource(compilerParam, codeStr);

            return compilerResult;
        }

        static bool mRegAssembly = false;
        public static bool CompileEventCodeWithEventId(Guid eventId, int csType, bool bForceCompile,bool debug = true,string targetDir = "")
        {
            bCompileDebug = debug;
            mTargetDir = targetDir;

            if (!mRegAssembly)
            {
                var clientWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "ClientCommon.dll");
                if (clientWindowsAssembly != null)
                    CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Windows, "cscommon", clientWindowsAssembly);

                var serverWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "ServerCommon.dll");
                if (serverWindowsAssembly != null)
                    CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "cscommon", serverWindowsAssembly);

                clientWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Client.dll");
                if (clientWindowsAssembly != null)
                    CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Windows, "game", clientWindowsAssembly);

                serverWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "Server.dll");
                if (serverWindowsAssembly != null)
                    CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "game", serverWindowsAssembly);

                mRegAssembly = true;
            }

            CSUtility.Helper.EventCallBackManager.Instance._SetCSType((CSUtility.Helper.enCSType)csType);
            var evb = CSUtility.Helper.EventCallBackManager.Instance.LoadCallee(eventId, false);

            return CompileEventCodeWithEventCallback(evb, (CSUtility.Helper.enCSType)csType, bForceCompile);
        }                 

        public static bool CompileEventCodeWithEventCallback(CSUtility.Helper.EventCallBack evb, CSUtility.Helper.enCSType csType, bool bForceCompile)
        {
            if (evb == null)
                return false;

            string dllDir = "";
            string dir = "";
            if (mTargetDir == "")
            {
                dir = CSUtility.Support.IFileManager.Instance.Root;
            }
            else
            {
                dir = mTargetDir;
            }
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    {
                        dllDir = dir + CSUtility.Support.IFileConfig.EventDlls_Client_Directory;
                    }
                    break;

                case CSUtility.Helper.enCSType.Server:
                    {
                        dllDir = dir + CSUtility.Support.IFileConfig.EventDlls_Server_Directory;
                    }
                    break;
            }

            var tw = GenerateCode(evb, csType);
            if (!System.IO.Directory.Exists(dllDir))
            {
                System.IO.Directory.CreateDirectory(dllDir);
            }

            var dllFile = dllDir + "\\" + CSUtility.Helper.EventCallBack.GetAssemblyFileName(evb.Id, csType);
            if (!System.IO.File.Exists(dllFile) || bForceCompile)
            {
                // 尝试删除旧版本的dll文件
                var keyStr = evb.Id.ToString();
                var files = System.IO.Directory.EnumerateFiles(dllDir);
                foreach (var file in files)
                {
                    if (file.Contains(keyStr))
                    {
                        try
                        {
                            System.IO.File.Delete(file);
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }                        
                    }
                }

                var compileResult = CompileCode(tw.ToString(), csType, evb.Id, dllFile, bCompileDebug);
                if (compileResult.Errors.HasErrors)
                {
                    foreach (var error in compileResult.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine(error);
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
