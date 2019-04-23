using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesLeadToOtherEditor
{
    class CopyFilesToDevices
    {
        Dictionary<string, string> fileListMD5;
        string cmdPath = @"c:\windows\system32\cmd.exe";
        string mTmpPushFilePath;
        string mTmpPullFilePath;
        string mTmpFileName;
        static CopyFilesToDevices _Instance = null;

        static public CopyFilesToDevices Instance
        {
            get
            {
                if (null == _Instance)
                    _Instance = new CopyFilesToDevices();
                return _Instance;
            }
        }

        /// <summary>
        /// CMD命令函数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="output"></param>
        void RunCmd(string cmd, out string output)
        {
            cmd = cmd.Trim().TrimEnd('&') + "&exit";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = cmdPath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
            }
        }

        /// <summary>
        /// 得到文件的MD5校验码
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        /// <summary>
        /// 得到需要拷贝的文件夹的所有文件
        /// </summary>
        /// <param name="fileName"></param>
        void GetAllFileInTheFile(string fileName)
        {
            DirectoryInfo d = new DirectoryInfo(fileName);      // 需要进行更新的文件路径
            DirectoryInfo[] items = d.GetDirectories();         // 该数组包含了子文件夹的信息
            FileInfo[] files = d.GetFiles();                    // 该数组包含了文件夹内的文件信息

            if (0 != files.Length)
            {
                foreach (var file in files)
                {
                    StreamWriter swFilePath = File.AppendText(mTmpPushFilePath + mTmpFileName);       // 创建临时文件保存文件路径及其MD5码
                    string MD5 = GetMD5HashFromFile(file.FullName);                                 // 拿到文件的MD5码
                    swFilePath.WriteLine(file.FullName);                                            // 将文件路径写入临时文件
                    swFilePath.WriteLine(MD5);                                                      // 将文件的MD5码写入临时文件
                    swFilePath.Close();

                    string mdCode = "";
                    if (!fileListMD5.TryGetValue(file.FullName, out mdCode))                        // 
                    {
                        mdCode = "";
                        string filePath = file.FullName.Substring(3);
                        string newPath = filePath.Replace('\\', '/');
                        string output = "";
                        {
                            string cmd = @"adb push " + file.FullName + @" /sdcard/" + newPath;
                            RunCmd(cmd, out output);
                        }
                        Console.WriteLine(file.FullName);
                        Console.WriteLine(MD5);
                    }
                    else
                    {
                        if (mdCode != MD5)
                        {
                            string filePath = file.FullName.Substring(3);
                            string newPath = filePath.Replace('\\', '/');
                            string output = "";
                            {
                                string cmd = @"adb push " + file.FullName + @" /sdcard/" + newPath;
                                RunCmd(cmd, out output);
                            }
                            Console.WriteLine(file.FullName);
                            Console.WriteLine(MD5);
                        }
                    }
                }

            }
            if (items.Length == 0)
                return;
            foreach (var item in items)
            {
                GetAllFileInTheFile(item.FullName);
            }
        }

        void DeleteExitsFile()
        {
            if (File.Exists(mTmpPushFilePath + mTmpFileName))
            {
                File.Delete(mTmpPushFilePath + mTmpFileName);
            }
            if (File.Exists(mTmpPullFilePath + mTmpFileName))
            {
                File.Delete(mTmpPullFilePath + mTmpFileName);
            }
        }

        string TmpFileName
        {
            set
            {
                mTmpFileName = value;
            }
            get
            {
                return mTmpFileName;
            }
        }

        string TmpPushFilePath
        {
            set
            {
                mTmpPushFilePath = value;
            }
            get
            {
                return mTmpPushFilePath;
            }
        }

        string TmpPullFilePath
        {
            set
            {
                mTmpPullFilePath = value;
            }
            get
            {
                return mTmpPullFilePath;
            }
        }

        /// <summary>
        /// 执行复制文件的主函数
        /// </summary>
        /// <param name="copyFullFileNamePath">需要复制的文件路径</param>
        /// <param name="tmpFileName">临时文件的文件名</param>
        /// <param name="tmpPushFilePath">临时文件的存放路径</param>
        /// <param name="tmpPullFilePath">临时文件的保存路径</param>
        public void StartCopy(string copyFullFileNamePath, string tmpFileName, string tmpPushFilePath, string tmpPullFilePath)
        {
            TmpFileName = tmpFileName;
            TmpPullFilePath = tmpPullFilePath;
            TmpPushFilePath = tmpPushFilePath;

            DeleteExitsFile();
            fileListMD5 = new Dictionary<string, string>();
            // 移动设备的文件信息复制到本地电脑当做TMP文件使用，记得用完做删除操作
            string output = "";

            {
                string cmd = @"adb pull /sdcard/" + mTmpFileName + " " + mTmpPullFilePath;
                RunCmd(cmd, out output);
            }

            if (File.Exists(mTmpPullFilePath + mTmpFileName))
            {
                using (TextReader tr = File.OpenText(mTmpPullFilePath + mTmpFileName))
                {
                    string readFilePath;
                    string readMD5;
                    while (null != (readFilePath = tr.ReadLine()) && null != (readMD5 = tr.ReadLine()))
                    {
                        if (!fileListMD5.ContainsKey(readFilePath))
                            fileListMD5.Add(readFilePath, readMD5);
                    }
                }
            }

            GetAllFileInTheFile(copyFullFileNamePath);

            {
                string cmd = @"adb push " + mTmpPushFilePath + mTmpFileName + @" /sdcard/";
                RunCmd(cmd, out output);
            }
            // 删除创建和移出的临时文件

            DeleteExitsFile();

            Console.WriteLine("It has copied over");
            Console.ReadLine();
        }

    }
}
