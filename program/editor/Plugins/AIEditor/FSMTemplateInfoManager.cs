using System;
using System.Collections.Generic;

namespace AIEditor
{
    public class FSMTemplateInfoManager
    {
        public delegate void Delegate_OnTellServerUpdateFSM(Guid fsmId);
        public Delegate_OnTellServerUpdateFSM OnTellServerUpdateFSM;

        protected static FSMTemplateInfoManager mInstance = new FSMTemplateInfoManager();
        public static FSMTemplateInfoManager Instance
        {
            get { return mInstance; }
        }
        
        public bool IsDebug
        {
            get;
            set;
        }

        // 记录状态机ID对应的目录
        Dictionary<Guid, string> mFStateMachineTemplateDirectoryDictionary = new Dictionary<Guid, string>();
        Dictionary<Guid, FSMTemplateInfo> mFSMInfosDictionary = new Dictionary<Guid, FSMTemplateInfo>();

        public FSMTemplateInfo GetFSMTemplateInfo(Guid templateId, bool bForceLoad)
        {
            FSMTemplateInfo result;

            if (!bForceLoad)
            {
                if (mFSMInfosDictionary.TryGetValue(templateId, out result))
                    return result;
            }

            var rootFolder = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;
            result = FindFSMTemplate(templateId, rootFolder);
            mFSMInfosDictionary[templateId] = result;

            return result;
        }

        public string GetFSMTemplateDirectory(Guid id)
        {
            string retValue = "";
            if (mFStateMachineTemplateDirectoryDictionary.TryGetValue(id, out retValue))
                return retValue;

            return retValue;
        }


        public static Guid GetIdFromFolder(string folder)
        {
            folder = folder.Replace("\\", "/");
            var idStr = folder.Substring(folder.LastIndexOf("/") + 1);
            Guid retGuid;
            if (System.Guid.TryParse(idStr, out retGuid))
                return retGuid;

            return Guid.Empty;
        }

        // folder为绝对路径
        private FSMTemplateInfo FindFSMTemplate(Guid id, string folder)
        {
            if (!System.IO.Directory.Exists(folder))
                return null;

            var dirs = System.IO.Directory.GetDirectories(folder, id.ToString(), System.IO.SearchOption.AllDirectories);
            if (dirs == null || dirs.Length == 0)
                return null;
            
            var dirId = GetIdFromFolder(dirs[0]);
            if (dirId == Guid.Empty)
            {
                return null;
            }

            var relativeDir = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(dirs[0]);
            FSMTemplateInfo fsm = new FSMTemplateInfo(id);
            fsm.LoadFSMTemplate(relativeDir);
            fsm.OnSave = OnHostAIInstanceSave;
            fsm.OnDelete = OnHostAIInstanceDelete;

            mFStateMachineTemplateDirectoryDictionary[id] = relativeDir;
            return fsm;
        }

        // folder为绝对路径,路径全部为"/"
        public FSMTemplateInfo CreateFSMTemplate(string folder)
        {
            var id = Guid.NewGuid();
            FSMTemplateInfo fsm = new FSMTemplateInfo(id);
            fsm.OnSave = OnHostAIInstanceSave;
            fsm.OnDelete = OnHostAIInstanceDelete;
            mFSMInfosDictionary[id] = fsm;
            folder = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(folder, CSUtility.Support.IFileManager.Instance.Root);
            mFStateMachineTemplateDirectoryDictionary[id] = folder + "/" + id.ToString();
            return fsm;
        }

        // folder为相对Release的目录
        public void SetFSMTemplateFolder(Guid id, string folder)
        {
            mFStateMachineTemplateDirectoryDictionary[id] = folder;
        }

        public void RemoveFSMTemplate(Guid id, bool bDeleteFile, bool withSVN)
        {
            FSMTemplateInfo fsm = null;
            if (mFSMInfosDictionary.TryGetValue(id, out fsm) && bDeleteFile)
            {
                var dir = mFStateMachineTemplateDirectoryDictionary[id];
                fsm.DeleteFiles(dir, withSVN);
            }

            mFSMInfosDictionary.Remove(id);
            mFStateMachineTemplateDirectoryDictionary.Remove(id);
        }

        public FSMTemplateInfo ForceReloadFSMTemplate(Guid id)
        {
            FSMTemplateInfo fsm = GetFSMTemplateInfo(id, true);
            var dir = mFStateMachineTemplateDirectoryDictionary[id];
            fsm.LoadFSMTemplate(dir);

            return fsm;
        }

        public void SaveFSMTemplate(Guid id, bool withSVN)
        {
            FSMTemplateInfo fsm = GetFSMTemplateInfo(id, false);
            var dir = mFStateMachineTemplateDirectoryDictionary[id];
            fsm.SaveFSMTemplate(dir, withSVN);

            CSUtility.AISystem.FStateMachineTemplateManager.Instance.GetFSMTemplate(id, CSUtility.Helper.enCSType.Client, true);
            
            // 通知服务器 读取 Server版本的fsm
            if (OnTellServerUpdateFSM != null)
                OnTellServerUpdateFSM(id);
        }


        private void OnHostAIInstanceDelete(AIEditor.FSMTemplateInfo info, bool withSVN)
        {
            // 删除dll文件
            // Client
            var cDllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.FSMDlls_Client_Directory;
            var cAssemblyName = cDllDir + CSUtility.AISystem.FStateMachineTemplate.GetAssemblyFileName(info.Id, CSUtility.Helper.enCSType.Client);
            //if (withSVN)
            //{
            //    SvnInterface.Commander.Instance.Delete(cAssemblyName);
            //    SvnInterface.Commander.Instance.Commit(cAssemblyName);
            //}
            if (System.IO.File.Exists(cAssemblyName))
                System.IO.File.Delete(cAssemblyName);

            var sDllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.FSMDlls_Server_Directory;
            var sAssemblyName = sDllDir + CSUtility.AISystem.FStateMachineTemplate.GetAssemblyFileName(info.Id, CSUtility.Helper.enCSType.Server);
            //if (withSVN)
            //{
            //    SvnInterface.Commander.Instance.Delete(sAssemblyName);
            //    SvnInterface.Commander.Instance.Commit(sAssemblyName);
            //}
            if (System.IO.File.Exists(sAssemblyName))
                System.IO.File.Delete(sAssemblyName);
        }
        private bool OnHostAIInstanceSave(AIEditor.FSMTemplateInfo info, bool withSVN)
        {
            if (!GenerateFSMCodeAndDll(info, CSUtility.Helper.enCSType.Client))
            {
                EditorCommon.MessageBox.Show("客户端AI代码未编译通过，请查看代码并检查错误！");
                return false;
            }
            if (!GenerateFSMCodeAndDll(info, CSUtility.Helper.enCSType.Server))
            {
                EditorCommon.MessageBox.Show("服务器端AI代码未编辑通过，请查看代码并检查错误！");
                return false;
            }

            // 判断客户端及服务器是否有不能删除的对象，如果有则需要更新新的版本号再保存
            TryDeleteOldDll(info, CSUtility.Helper.enCSType.Client);
            TryDeleteOldDll(info, CSUtility.Helper.enCSType.Server);

            //// 因为服务器和客户端的版本一致，所以这里改哪个都行
            //CSUtility.AISystem.FSMTemplateVersionManager.Instance.VersionUpdate(info.Id, CSUtility.Helper.enCSType.Client);

            return true;
        }

        private bool TryDeleteOldDll(AIEditor.FSMTemplateInfo info, CSUtility.Helper.enCSType csType)
        {
            string dllDir = "";
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    dllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.FSMDlls_Client_Directory;
                    break;

                case CSUtility.Helper.enCSType.Server:
                    dllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.FSMDlls_Server_Directory;
                    break;

                default:
                    return false;
            }
            //var dllDir = CSUtility.Support.IFileManager.Instance.Root +
            //             CSUtility.Support.IFileConfig.DefaultFSMDirectory + "/" + Program.AIDllsFolderName;
            if (!System.IO.Directory.Exists(dllDir))
            {
                System.IO.Directory.CreateDirectory(dllDir);
                //SvnInterface.Commander.Instance.AddFolder(dllDir);
                //SvnInterface.Commander.Instance.Commit(dllDir);
            }

            var assemblyName = CSUtility.AISystem.FStateMachineTemplate.GetAssemblyFileName(info.Id, csType);

            foreach (var file in System.IO.Directory.EnumerateFiles(dllDir))
            {
                if (file.Contains(info.Id.ToString()) && !file.Contains(assemblyName))
                {
                    try
                    {
                        //SvnInterface.Commander.Instance.Delete(file);
                        //SvnInterface.Commander.Instance.Commit(file);

                        System.IO.File.Delete(file);
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        continue;
                    }
                }
            }

            return true;

            //if (deleteFail)
            //{
            //    //info.Version++;
            //    CSUtility.AISystem.FSMTemplateVersionManager.Instance.VersionUpdate(info.Id, csType);
            //}
        }

        private bool GenerateFSMCodeAndDll(AIEditor.FSMTemplateInfo info, CSUtility.Helper.enCSType csType)
        {
            // 生成代码dll
            var tw = AIEditor.CodeGenerate.CodeGenerator.GenerateCode(info, csType);

            string dllDir = "";
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Client:
                    dllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.FSMDlls_Client_Directory;
                    break;

                case CSUtility.Helper.enCSType.Server:
                    dllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.FSMDlls_Server_Directory;
                    break;

                default:
                    return false;
            }

            if (!System.IO.Directory.Exists(dllDir))
            {
                System.IO.Directory.CreateDirectory(dllDir);

                //SvnInterface.Commander.Instance.AddFolder(dllDir);
                //SvnInterface.Commander.Instance.Commit(dllDir);
            }

            //var dllDir = CSUtility.Support.IFileManager.Instance.Root +
            //             CSUtility.Support.IFileConfig.DefaultFSMDirectory + "/" + Program.AIDllsFolderName;
            //if (!System.IO.Directory.Exists(dllDir))
            //    System.IO.Directory.CreateDirectory(dllDir);

            //bool deleteFail = false;
            //foreach (var file in System.IO.Directory.EnumerateFiles(dllDir))
            //{
            //    if (file.Contains(info.Id.ToString()))
            //    {
            //        try
            //        {
            //            System.IO.File.Delete(file);
            //        }
            //        catch (System.Exception ex)
            //        {
            //            System.Diagnostics.Debug.WriteLine(ex.ToString());
            //            deleteFail = true;
            //        }
            //    }
            //}

            //if (deleteFail)
            //{
            //    //info.Version++;
            //    CSUtility.AISystem.FSMTemplateVersionManager.Instance.VersionUpdate(info.Id, csType);
            //}
            var dllFile = dllDir + "\\" + CSUtility.AISystem.FStateMachineTemplate.GetAssemblyFileName(info.Id, csType);// info.AssemblyFileName;//info.Id + "_V" + info.Version + ".dll";
            var compileResult = AIEditor.CodeGenerate.CodeGenerator.CompileCode(tw.ToString(), csType, dllFile, IsDebug, info.Id);
            //if (compileResult.Errors.HasErrors)
            //{
            //    info.Version++;
            //    OnHostAIInstanceSave(info);
            //}
            System.Diagnostics.Debug.WriteLine(csType.ToString() + "\r\n" + compileResult.Output);

            if (compileResult.Errors.HasErrors)
            {
                foreach (var error in compileResult.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(error);
                }

                return false;
            }
            //else
            //{
            //    if (System.IO.File.Exists(dllFile))
            //    {
            //        var status = SvnInterface.Commander.Instance.CheckStatusEx(dllFile);
            //        switch (status)
            //        {
            //            case SvnInterface.SvnStatus.NotControl:
            //                {
            //                    SvnInterface.Commander.Instance.Add(new string[] { dllFile });
            //                    SvnInterface.Commander.Instance.Commit(dllFile);
            //                }
            //                break;
            //            case SvnInterface.SvnStatus.Conflict:
            //                {
            //                    EditorCommon.MessageBox.Show("AIDll " + dllFile + "冲突，请检查");
            //                }
            //                break;
            //            default:
            //                {
            //                    SvnInterface.Commander.Instance.Commit(dllFile);
            //                }
            //                break;
            //        }
            //    }
            //}

            return true;
        }
    }
}
