using System;
using System.Collections.Generic;
using System.Net;

namespace CSUtility.FileDownload
{
    public class FileDownInfo
    {
        private string id;
        /// <summary>
        /// ID唯一标识
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        object mTag;
        public object Tag
        {
            get { return mTag; }
            set { mTag = value; }
        }

        int mPriority = 0;
        /// <summary>
        /// 优先级(数字越大优先级越高)
        /// </summary>
        public int Proiority
        {
            get { return mPriority; }
            set { mPriority = value; }
        }

        private FileDownStatus status;
        /// <summary>
        /// 状态
        /// </summary>
        public FileDownStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        private string url;
        /// <summary>
        /// URL地址
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value.Replace("\\", "/"); }
        }

        private string savePath;
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath
        {
            get { return savePath; }
            set { savePath = value; }
        }

        private bool overwriteFile = false;
        public bool OverwriteFile
        {
            get { return overwriteFile; }
            set { overwriteFile = value; }
        }

        private long fileSize;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        private DateTime beginTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get { return beginTime; }
            set { beginTime = value; }
        }

        private DateTime endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        private TimeSpan useTime;
        /// <summary>
        /// 用时
        /// </summary>
        public TimeSpan UseTime
        {
            get { return useTime; }
            set { useTime = value; }
        }

        private string fileName;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private string errorMsg;
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }

        private int errorCount = 0;
        /// <summary>
        /// 错误次数
        /// </summary>
        public int ErrorCount
        {
            get { return errorCount; }
            set { errorCount = value; }
        }

        protected string mMD5 = "";
        /// <summary>
        /// 文件特征值
        /// </summary>
        public string MD5
        {
            get { return mMD5; }
            set { mMD5 = value; }
        }

        protected bool mCheckMD5 = false;
        public bool CheckMD5
        {
            get { return mCheckMD5; }
            set { mCheckMD5 = value; }
        }

        public bool UnzipWhenDownloadComplate = false;
        public string UnzipFolder = "";

        /// <summary>
        /// 获取备注
        /// </summary>
        /// <returns></returns>
        public string GetMemo()
        {
            string str = "";
            if (!string.IsNullOrEmpty(errorMsg))
            {
                str += "[" + errorCount.ToString() + "] 错误：" + errorMsg;
            }

            return str;
        }

        /// <summary>
        /// 下载状态
        /// </summary>
        public enum FileDownStatus
        {
            /// <summary>
            /// 等待下载
            /// </summary>
            Wait,

            /// <summary>
            /// 下载中
            /// </summary>
            Doing,

            /// <summary>
            /// 下载失败
            /// </summary>
            Error,

            /// <summary>
            /// 下载完成
            /// </summary>
            Finish
        }

        /// <summary>
        /// 获取下载状态名称
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetFileDownStatusName(FileDownStatus status)
        {
            string strReturn = "";
            switch (status)
            {
                case FileDownStatus.Wait:
                    strReturn = "等待下载";
                    break;
                case FileDownStatus.Doing:
                    strReturn = "下载中";
                    break;
                case FileDownStatus.Error:
                    strReturn = "下载失败";
                    break;
                case FileDownStatus.Finish:
                    strReturn = "下载完成";
                    break;
                default:
                    break;
            }

            return strReturn;
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="disk"></param>
        /// <param name="isCover"></param>
        /// <returns></returns>
        public static FileDownInfo AddDownFile(string url, string disk, bool overwriteFile, string md5, IList<FileDownInfo> fileDownList = null)
        {
            url = url.Replace("\\", "/");
            disk = disk.Replace("/", "\\");

            if(fileDownList != null)
            {
                foreach (FileDownInfo r in fileDownList)
                {
                    if (r.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return null;
                    }
                }
            }

            FileDownInfo file = new FileDownInfo();
            file.MD5 = md5;
            file.overwriteFile = overwriteFile;
            file.BeginTime = DateTime.MinValue;
            file.EndTime = DateTime.MinValue;
            file.ErrorMsg = "";
            file.FileName = url.Substring(url.LastIndexOf("/"));
            file.FileSize = 0;
            file.Id = Guid.NewGuid().ToString();
            file.SavePath = disk;
            //if (!file.savePath.Contains(".zip"))
            //    System.Diagnostics.Debugger.Break();
            file.Status = FileDownInfo.FileDownStatus.Wait;
            file.Url = url;
            file.UseTime = new TimeSpan();

            if (fileDownList != null)
                fileDownList.Add(file);

            return file;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static long GetUrlSize(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return 0;

                WebRequest req = HttpWebRequest.Create(url);
                req.Timeout = 1000;
                req.Method = "HEAD";
                WebResponse resp = req.GetResponse();
                if (resp == null)
                    return 0;

                long retValue;
                long.TryParse(resp.Headers.Get("Content-Length"), out retValue);
                resp.Close();
                return retValue;
            }
            catch(NotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch(ArgumentNullException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (System.Security.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (UriFormatException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (NotImplementedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return 0;
        }

        /// <summary>
        /// 格式化空间大小 形如：[2.63M]
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string FormatSize(long size)
        {
            string strReturn = "";
            double tempSize = Math.Abs(size);
            if (tempSize < 1024)
            {
                strReturn += tempSize.ToString() + "B";
            }
            else if (tempSize < 1024 * 1024)
            {
                tempSize = tempSize / 1024;
                strReturn += tempSize.ToString("0.##") + "K";
            }
            else if (tempSize < 1024 * 1024 * 1024)
            {
                tempSize = tempSize / 1024 / 1024;
                strReturn += tempSize.ToString("0.##") + "M";
            }
            else
            {
                tempSize = tempSize / 1024 / 1024 / 1024;
                strReturn += tempSize.ToString("0.##") + "G";
            }

            if (size < 0)
            {
                strReturn = "-" + strReturn;
            }

            return strReturn;
        }
    }
}
