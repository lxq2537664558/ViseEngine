using System;
using System.Collections.Generic;

namespace AIEditor
{
    public sealed class ShowInAIEditorMenu : CodeGenerateSystem.ShowInMenu
    {
        public ShowInAIEditorMenu(string showNames)
            : base(showNames)
        {

        }
    }

    public static class Program
    {
        static bool bCompileDebug = true;
        static string mOutPutDir = "";

        public static AIEditor.FSMTemplateInfo CurrentHostAIInstanceInfo = null;
        public static string CodeFilesFolderName = "CodeFiles";

        /*static bool mFrameSetAssmLoaded = false;
        static System.Reflection.Assembly mFrameSetAssm = null;
        public static System.Reflection.Assembly FrameSetAssm
        {
            get
            {
                if (mFrameSetAssm == null)
                {
                    try
                    {
                        if (mFrameSetAssmLoaded)
                            return null;

                        mFrameSetAssmLoaded = true;
                        mFrameSetAssm = CSUtility.Program.GetAssemblyFromDllFileName("FrameSet.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }                    
                }

                return mFrameSetAssm;
            }
        }
        static bool mCSCommonAssmLoaded = false;
        static System.Reflection.Assembly mCSCommonAssm = null;
        public static System.Reflection.Assembly CSCommonAssm
        {
            get
            {
                if (mCSCommonAssm == null)
                {
                    try
                    {
                        if (mCSCommonAssmLoaded)
                            return null;

                        mCSCommonAssmLoaded = true;
                        mCSCommonAssm = CSUtility.Program.GetAssemblyFromDllFileName("CSUtility.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                return mCSCommonAssm;
            }
        }
        static bool mServerAssmLoaded = false;
        static System.Reflection.Assembly mServerAssm = null;
        public static System.Reflection.Assembly ServerAssm
        {
            get
            {
                if (mServerAssm == null)
                {
                    try
                    {
                        if (mServerAssmLoaded)
                            return null;

                        mServerAssmLoaded = true;
                        mServerAssm = CSUtility.Program.GetAssemblyFromDllFileName("ServerCommon.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                return mServerAssm;
            }
        }
        static bool mSlimDXAssmLoaded = false;
        static System.Reflection.Assembly mSlimDXAssm = null;
        public static System.Reflection.Assembly SlimDXAssm
        {
            get
            {
                if (mSlimDXAssm == null)
                {
                    try
                    {
                        if (mSlimDXAssmLoaded)
                            return null;

                        mSlimDXAssmLoaded = true;
                        mSlimDXAssm = CSUtility.Program.GetAssemblyFromDllFileName("SlimDX.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                return mSlimDXAssm;
            }
        }
        //public static System.Reflection.Assembly AIStateAssem = System.Reflection.Assembly.LoadFrom("AISystem.dll");
        //public static string AIDllsFolderName = "AIDlls";

        public static Type GetType(string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr))
                return null;

            Type retType = null;

            retType = Type.GetType(typeStr);
            //if (retType == null)
            //    retType = AIStateAssem.GetType(typeStr);
            if (retType == null && CSCommonAssm != null)
                retType = CSCommonAssm.GetType(typeStr);
            if (retType == null && FrameSetAssm != null)
                retType = FrameSetAssm.GetType(typeStr);
            if (retType == null && ServerAssm != null)
                retType = ServerAssm.GetType(typeStr);
            if (retType == null && SlimDXAssm != null)
                retType = SlimDXAssm.GetType(typeStr);

            return retType;
        }

        public static Type GetType(string typeStr, CSUtility.Helper.enCSType csType)
        {
            Type retType = null;

            switch (csType)
            {
                case CSUtility.Helper.enCSType.Common:
                    {
                        retType = CSCommonAssm.GetType("CSUtility.AISystem.States." + typeStr);
                    }
                    break;

                case CSUtility.Helper.enCSType.Client:
                    {
                        retType = FrameSetAssm.GetType("FrameSet.ClientStates." + typeStr);
                        if(retType == null)
                            retType = CSCommonAssm.GetType("CSUtility.AISystem.States." + typeStr);
                    }
                    break;

                case CSUtility.Helper.enCSType.Server:
                    {
                        retType = ServerAssm.GetType("FrameSet.ServerStates." + typeStr);
                        if (retType == null)
                            retType = CSCommonAssm.GetType("CSUtility.AISystem.States." + typeStr);
                    }
                    break;
            }

            return retType;
        }

        public static Type[] GetTypes(CSUtility.Helper.enCSType csType)
        {
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Common:
                    {
                        return CSCommonAssm.GetTypes();
                    }

                case CSUtility.Helper.enCSType.Client:
                    {
                        List<Type> retTypes = new List<Type>();
                        retTypes.AddRange(FrameSetAssm.GetTypes());
                        retTypes.AddRange(CSCommonAssm.GetTypes());
                        return retTypes.ToArray();
                    }

                case CSUtility.Helper.enCSType.Server:
                    {
                        List<Type> retTypes = new List<Type>();
                        retTypes.AddRange(ServerAssm.GetTypes());
                        retTypes.AddRange(CSCommonAssm.GetTypes());
                        return retTypes.ToArray();
                    }

                case CSUtility.Helper.enCSType.All:
                    {
                        List<Type> retTypes = new List<Type>();
                        retTypes.AddRange(FrameSetAssm.GetTypes());
                        retTypes.AddRange(ServerAssm.GetTypes());
                        retTypes.AddRange(CSCommonAssm.GetTypes());
                        return retTypes.ToArray();
                    }
            }

            return null;
        }

        public static Type[] GetAllClientStatementType()
        {
            List<Type> statementTypes = new List<Type>();

            foreach (var type in FrameSetAssm.GetTypes())
            {
                var atts = type.GetCustomAttributes(typeof(CSUtility.AISystem.Attribute.StatementClassAttribute), true);
                if (atts.Length <= 0)
                    continue;

                statementTypes.Add(type);
            }

            return statementTypes.ToArray();
        }

        public static Type[] GetAllServerStatementType()
        {
            List<Type> statementTypes = new List<Type>();


            return statementTypes.ToArray();
        }*/

        public static Type[] GetTypes(CSUtility.Helper.enCSType csType)
        {
            List<Type> retValue = new List<Type>();
            string attFullName = typeof(CSUtility.AISystem.Attribute.AllowClass).FullName;
            var types = CSUtility.Program.GetTypes(attFullName, true);
            foreach(var type in types)
            {
                if (csType.ToString().Equals(CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(type, attFullName, "CSType", false).ToString()))
                    retValue.Add(type);
            }

            return retValue.ToArray();
            //CSUtility.Program.GetTypes
        }

        public static Type GetType(string typeFullName)
        {
            return CSUtility.Program.GetTypeFromTypeFullName(typeFullName);
        }

        public static string GetValuedGUIDString(Guid guid)
        {
            string retString = guid.ToString();
            retString = retString.Replace("-", "_");

            return retString;
        }

        static bool mRegAssembly = false;
        public static bool CompileAICodeWithAIGuid(Guid id,int csType,bool bForceCompile,bool bDebug = true,string outPutDir = "")
        {
            bCompileDebug = bDebug;
            mOutPutDir = outPutDir;

            if (!mRegAssembly)
            {
                var clientAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Client.dll");
                CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Windows, "game", clientAssembly);

                var serverAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "Server.dll");
                CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "game", serverAssembly);

                clientAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "ClientCommon.dll");
                CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Windows, "cscommon", clientAssembly);

                serverAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "ServerCommon.dll");
                CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "cscommon", serverAssembly);

                mRegAssembly = true;
            }

            return CompileAICodeWithFSMTemplate(id, (CSUtility.Helper.enCSType)csType, bForceCompile);
        }

        public static bool CompileAICodeWithFSMTemplate(Guid id, CSUtility.Helper.enCSType csType, bool bForceCompile)
        {
            var fsmTemplateInfo = FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(id, bForceCompile);
            if (fsmTemplateInfo == null)
                return false;

            CurrentHostAIInstanceInfo = fsmTemplateInfo;

            string dllDir = "";
            string dir = "";
            if (mOutPutDir == "")
            {
                dir = CSUtility.Support.IFileManager.Instance.Root;
            }
            else
            {
                dir = mOutPutDir;
            }
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    {
                        dllDir = dir + CSUtility.Support.IFileConfig.FSMDlls_Client_Directory;
                    }
                    break;

                case CSUtility.Helper.enCSType.Server:
                    {
                        dllDir = dir + CSUtility.Support.IFileConfig.FSMDlls_Server_Directory;
                    }
                    break;
            }

            var tw = AIEditor.CodeGenerate.CodeGenerator.GenerateCode(fsmTemplateInfo, csType);
            if (!System.IO.Directory.Exists(dllDir))
            {
                System.IO.Directory.CreateDirectory(dllDir);
            }

            var dllFile = dllDir + "\\" + CSUtility.AISystem.FStateMachineTemplate.GetAssemblyFileName(fsmTemplateInfo.Id, csType);
            if (!System.IO.File.Exists(dllFile) || bForceCompile)
            {
                var compileResult = AIEditor.CodeGenerate.CodeGenerator.CompileCode(tw.ToString(), csType, dllFile, bCompileDebug, fsmTemplateInfo.Id);
                if (compileResult.Errors.HasErrors)
                {
                    foreach (var error in compileResult.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine(error);
                    }

                    return false;
                }
            }

            //tw = AIEditor.CodeGenerate.CodeGenerator.GenerateCode(fsmTemplateInfo, CSUtility.Helper.enCSType.Server);
            //dllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.FSMDlls_Server_Directory;
            //if (!System.IO.Directory.Exists(dllDir))
            //    System.IO.Directory.CreateDirectory(dllDir);

            //dllFile = dllDir + "\\" + CSUtility.AISystem.FStateMachineTemplate template.GetAssemblyFileName(fsmTemplateInfo.Id, CSUtility.Helper.enCSType.Server);
            //if (!System.IO.File.Exists(dllFile))
            //{
            //    var compileResult = AIEditor.CodeGenerate.CodeGenerator.CompileCode(tw.ToString(), CSUtility.Helper.enCSType.Server, dllFile, false, fsmTemplateInfo.Id);
            //    if (compileResult.Errors.Count > 0)
            //    {
            //        System.Diagnostics.Debug.WriteLine(compileResult.Errors);
            //        return false;
            //    }
            //}

            return true;
        }
    }
}
