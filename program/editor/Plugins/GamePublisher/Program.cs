using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamePublisher
{
    public class Program
    {
        public static string SourceFolder = CSUtility.Support.IFileManager.Instance.Root;
        public static string TargetFolder;
        public static string FullPackageFolder;
        public static string MinPackageFolder;
        public static string LauncherFolder;
        public static string FinalPublishFolder;
        public static bool CompareFinalPublish = true;
        public static string HostIPAddress = "115.28.60.130";

        public static UInt32 Version_0 = 0;
        public static UInt32 Version_1 = 0;
        public static UInt32 Version_2 = 1;
        public static string Version
        {
            get { return Version_0 + "." + Version_1 + "." + Version_2; }
        }

        public static UInt32 LauncherVersion_0 = 0;
        public static UInt32 LauncherVersion_1 = 0;
        public static UInt32 LauncherVersion_2 = 1;
        public static string LauncherVersion
        {
            get { return LauncherVersion_0 + "." + LauncherVersion_1 + "." + LauncherVersion_2; }
        }

        public static bool CopyToZip = false;

        public static UInt64 CheckSVNVersion(string folder)
        {
            EditorCommon.VersionControl.VersionControlManager.Instance.Info((EditorCommon.VersionControl.VersionControlCommandResult result) =>
            {
                if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                {
                    
                }                
            }, folder);
            //var svnState = SvnInterface.Commander.Instance.Info(folder);

            string svnState = "";
            if (svnState == "SVN Not Enable")
            {
                System.Windows.Forms.MessageBox.Show("SVN命令行不可用!获取不到SVN版本!", "警告");
                return 0;
            }

            var start = svnState.IndexOf("Revision:");
            if (start < 0)
            {
                //System.Windows.Forms.MessageBox.Show("获取SVN版本失败!\r\n" + svnState, "警告");
                return 0;
            }
            var pStart = start + 10;
            var pEnd = svnState.IndexOf("Node Kind:");

            var verStr = svnState.Substring(pStart, pEnd - pStart);
            return System.Convert.ToUInt64(verStr);
        }

        public static string CopyFile(string srcFile, bool copyToZip)
        {
            srcFile = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(srcFile);
            srcFile = srcFile.Replace("\\", "/");
            var tagFile = srcFile.Replace(SourceFolder.Replace("\\", "/"), FullPackageFolder.Replace("\\", "/"));
            if (copyToZip)
                tagFile += ".zip";

            if (System.IO.File.Exists(srcFile))
            {
                var tagDir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(tagFile);
                if (!System.IO.Directory.Exists(tagDir))
                {
                    System.IO.Directory.CreateDirectory(tagDir);
                }

                if (copyToZip)
                {
                    var dir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(srcFile);
                    var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(srcFile);
                    CSUtility.Compress.CompressManager.ZipFile(file, dir, tagFile);
                }
                else
                    System.IO.File.Copy(srcFile, tagFile, true);
            }
            else
                return "";

            return tagFile;
        }

        public static string CopyFile(string srcFile, string tagFile, bool copyToZip)
        {
            //srcFile = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(srcFile);
            //srcFile = srcFile.Replace("\\", "/");
            //var tagFile = srcFile.Replace(SourceFolder.Replace("\\", "/"), FullPackageFolder.Replace("\\", "/"));

            if (copyToZip)
                tagFile += ".zip";

            if (System.IO.File.Exists(srcFile))
            {
                var tagDir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(tagFile);
                if (!System.IO.Directory.Exists(tagDir))
                {
                    System.IO.Directory.CreateDirectory(tagDir);
                }

                if (copyToZip)
                {
                    var dir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(srcFile);
                    var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(srcFile);
                    CSUtility.Compress.CompressManager.ZipFile(file, dir, tagFile);
                }
                else
                    System.IO.File.Copy(srcFile, tagFile, true);
            }
            else
                return "";

            return tagFile;
        }

        // 用.NET_Reactor加密文件
        public static void Use_DotNET_Reactor_Encrypt(string srcFile, string targetFile)
        {
            try
            {
                var temTagPath = CSUtility.Support.IFileManager.Instance.Root + "ZeusPublishTemp/";
                var tempTagFile = temTagPath + Guid.NewGuid().ToString();

                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.Arguments = "-file \"" + srcFile + "\" -targetfile \"" + tempTagFile + "\"";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;
                startInfo.FileName = @"C:\Users\Administrator\Desktop\dotnet_reactor_ha\dotNET_Reactor4.2.8.4\dotNET_Reactor 4.2.8.4 CH.exe";// SvnExePath;

                var process = new System.Diagnostics.Process();
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                process.Start();

                {
                    while (!process.HasExited)
                        System.Threading.Thread.Sleep(50);
                }

                System.IO.File.Copy(tempTagFile, targetFile, true);
                System.IO.File.Delete(tempTagFile);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("未能启动dotNET_Reactor!\r\n" + ex.ToString());
            }
        }

        // 数字签名
        public static void Use_SignTool_Sign(string file)
        {            
            //try
            //{
            //    var startInfo = new System.Diagnostics.ProcessStartInfo();
            //    // xp win7 "sign /v /f 北京忆唐创元文化有限公司.pfx /p 36onjiaoqSAOs /t http://timestamp.wosign.com/timestamp " + file;
            //    // win7、8 "sign /v /f 北京忆唐创元文化有限公司.pfx /p 36onjiaoqSAOs /tr http://timestamp.wosign.com/rfc3161 " + file;
            //    startInfo.Arguments = "sign /v /f D:\\victory\\SignTool\\北京忆唐创元文化有限公司.pfx /p 36onjiaoqSAOs /tr http://timestamp.wosign.com/rfc3161 " + file;
            //    startInfo.UseShellExecute = false;
            //    startInfo.CreateNoWindow = true;
            //    startInfo.RedirectStandardOutput = true;
            //    startInfo.RedirectStandardInput = true;
            //    startInfo.RedirectStandardError = true;
            //    startInfo.FileName = "D:\\victory\\SignTool\\SignTool.exe";

            //    var process = new System.Diagnostics.Process();
            //    process.StartInfo = startInfo;
            //    process.EnableRaisingEvents = true;
            //    process.Start();

            //    {
            //        while (!process.HasExited)
            //            System.Threading.Thread.Sleep(50);
            //    }

            //}
            //catch (System.Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show("数字签名失败!\r\n" + ex.ToString());
            //}
        }
    }
}
