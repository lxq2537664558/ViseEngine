using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassMetadataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            CSUtility.Support.ClassInfoManager.Instance.Load(false);
            bool needSave = false;

            var assembly = CSUtility.Program.GetAssemblyFromDllFileName("Client.Windows.dll");
            if (assembly != null)
                CSUtility.Support.ClassInfoManager.Instance.GenerateClassInfoFromAssembly(assembly, ref needSave);
            assembly = CSUtility.Program.GetAssemblyFromDllFileName("Server.Windows.dll");
            if (assembly != null)
                CSUtility.Support.ClassInfoManager.Instance.GenerateClassInfoFromAssembly(assembly, ref needSave);
            assembly = CSUtility.Program.GetAssemblyFromDllFileName("Client.dll");
            if (assembly != null)
                CSUtility.Support.ClassInfoManager.Instance.GenerateClassInfoFromAssembly(assembly, ref needSave);

            if (needSave)
            {
                // svn
                var svnUrl = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.MetaDataDirectory;

                if()
                //// SVN更新
                //var logs = SvnInterface.Commander.Instance.Update(svnUrl, -1, false);
                //System.Diagnostics.Debug.WriteLine(logs);                

                var newFileList = CSUtility.Support.ClassInfoManager.Instance.Save(false, false);

                if (SvnInterface.Commander.Instance.SvnAvailable)
                {
                    if (newFileList.Count > 0)
                    {
                        var log = SvnInterface.Commander.Instance.Add(newFileList.ToArray(), true);
                             System.Diagnostics.Debug.WriteLine(log);
                        log = SvnInterface.Commander.Instance.Commit(newFileList.ToArray(), "ClassPropertyInfoGenerator Update", true);
                        System.Diagnostics.Debug.WriteLine(log);
                    }


                    // SVN上传
                    var logs = SvnInterface.Commander.Instance.Commit(svnUrl, "ClassPropertyInfoGenerator Update", true);
                    System.Diagnostics.Debug.WriteLine(logs);
                }//
            }
        }
    }
}
