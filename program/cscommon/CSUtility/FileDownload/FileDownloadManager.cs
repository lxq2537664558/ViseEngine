using System;
using System.Collections.Generic;

namespace CSUtility.FileDownload
{
    /// <summary>
    /// 对下载线程进行管理
    /// </summary>
    public class FileDownloadManager : IDisposable
    {
        public delegate void Delegate_OnDownloadComplete();
        public event Delegate_OnDownloadComplete OnDownloadComplete;
        public delegate void Delegate_OnFileDownloadComplete(FileDownInfo fd);
        public event Delegate_OnFileDownloadComplete OnFileDownloadComplete;
        public delegate void Delegate_OnProgressReport(float total, float filePercent);
        public event Delegate_OnProgressReport OnProgressReport;
        public delegate void Delegate_OnErrorReport(FileDownInfo fd, DownloadException ex);
        public event Delegate_OnErrorReport OnErrorReport;
        public delegate void Delegate_OnReDownloadFile(FileDownInfo fd);
        public Delegate_OnReDownloadFile OnReDownloadFile;

        static FileDownloadManager smInstance;// = new FileDownloadManager();
        public static FileDownloadManager Instance
        {
            get
            {
                if (smInstance == null)
                    smInstance = new FileDownloadManager();
                return smInstance;
            }
        }

        public void Dispose()
        {
            this.StopAllDownload();

            mSyncDownloadService.Dispose();
            foreach (var service in mDownloadServices)
            {
                service.Dispose();
            }
        }

        //long mTotalFileSize = 0;
        
        /// <summary>
        /// 下载速度(单位b/秒)
        /// </summary>
        public long DownloadSpeed
        {
            get
            {
                long retValue = 0;
                foreach (var service in mDownloadServices)
                {
                    retValue += service.DownloadSpeed;
                }

                return retValue;
            }
        }

        long mLimitSpeed = 0;
        /// <summary>
        /// 限速(单位b)
        /// </summary>
        public long LimitSpeed
        {
            get { return mLimitSpeed; }
            set
            {
                mLimitSpeed = value;

                if (mLimitSpeed > 0)
                {
                    foreach (var service in mDownloadServices)
                    {
                        service.LimitSpeed = mLimitSpeed / mDownloadServices.Count;
                    }
                }
            }
        }
        int mSleepTime = 0;

        
        public int TotalDownloadFilesCount
        {
            get { return mFileInfoDic.Count + mDownloadedFiles.Count; }
        }
        public int DownloadedFilesCount
        {
            get { return mDownloadedFiles.Count; }
        }

        public static void FinalInstance()
        {
            if (smInstance != null)
            {
                smInstance.StopAllDownload();
                //smInstance.StopDownloadThread();
                smInstance = null;
            }
        }

        FileDownloadManager()
        {
            mSyncDownloadService.OnDownloadCompleted = _OnFileComplateReport;
            mSyncDownloadService.OnProgressReport = _OnProgressReport;
            mSyncDownloadService.OnErrorReport = _OnErrorReport;
            mSyncDownloadService.OnCancelDownloading = _OnCancelDownloading;

            //if (IsFinalize == false)
            //{
            //    StartDownloadThread();
            //}
        }
        ~FileDownloadManager()
        {
            //StopDownloadThread();
            StopAllDownload();

        }

        /// <summary>
        /// 停止所有下载
        /// </summary>
        public void StopAllDownload()
        {
            lock(this)
            {
                mFileInfos.Clear();
                mFileInfoDic.Clear();
                mDownloadingFile.Clear();
                mDownloadedFiles.Clear();
                mErrorList.Clear();
                mDownloadedUrl.Clear();
            }

            mSyncDownloadService.StopDownload();
            foreach (var service in mDownloadServices)
            {
                service.StopDownload();
            }
        }

        /// <summary>
        /// 同时下载的文件数
        /// </summary>        
        public Byte MaxDownloadServiceCount = 1;

        // 包含所有待下载文件的列表
        Dictionary<string, FileDownInfo> mFileInfoDic = new Dictionary<string, FileDownInfo>();
        /// <summary>
        /// 所有待下载文件的数量
        /// </summary>
        public int DownloadingFilesCount
        {
            get { return mFileInfoDic.Count; }
        }
        
        // 正在下载中的文件
        Dictionary<string, FileDownInfo> mDownloadingFile = new Dictionary<string, FileDownInfo>();

        // 优先下载文件列表
        List<FileDownInfo> mFileInfos = new List<FileDownInfo>();
        private void SortFileInfos()
        {
            mFileInfos.Sort(delegate(FileDownInfo a, FileDownInfo b)
            {
                if (a == null || b == null)
                    return 0;

                if (a.Proiority > b.Proiority)
                    return -1;
                else if (a.Proiority < b.Proiority)
                    return 1;
                else
                    return 0;
            });
        }

        // 同步下载服务
        FileDownloadService mSyncDownloadService = new FileDownloadService();
        // 异步下载服务
        List<FileDownloadService> mDownloadServices = new List<FileDownloadService>();
        /// <summary>
        /// 异步下载服务的个数
        /// </summary>
        public int DownloadingServicesCount
        {
            get { return mDownloadServices.Count; }
        }

        List<FileDownInfo> mErrorList = new List<FileDownInfo>();
        Dictionary<FileDownInfo, float> mDownloadedFiles = new Dictionary<FileDownInfo, float>();
        // 已下载的列表
        List<string> mDownloadedUrl = new List<string>();

        //enum enDownloadState
        //{
        //    Waiting,
        //    Downloading,
        //}
        //enDownloadState mDownloadState = enDownloadState.Waiting;

        //System.Threading.Thread mDownloadThread;
        //bool mDownloadThreadRuning = false;
        //System.DateTime mStartTime = System.DateTime.Now;

        //private void StartDownloadThread()
        //{
        //    mDownloadThreadRuning = true;
        //    mDownloadThread = new System.Threading.Thread(new System.Threading.ThreadStart(DownloadThreadTick));
        //    mDownloadThread.Name = "Downloading Thread";
        //    mDownloadThread.Start();
        //}
        //public void StopDownloadThread()
        //{
        //    mDownloadThreadRuning = false;
        //}

        public FileDownInfo GetDownloadFileInfo(string url)
        {
            FileDownInfo info = null;
            url = url.Replace("\\", "/");
            mFileInfoDic.TryGetValue(url, out info);
            return info;
        }

        public FileDownInfo[] GetCurrentDownloadingFiles()
        {
            //////lock (this)
            try
            {
                Program.InfoLock(this, new System.Diagnostics.StackFrame());

                var infos = new FileDownInfo[mDownloadServices.Count];

                //foreach (var services in mDownloadServices)
                for (int i = 0; i < mDownloadServices.Count; i++ )
                {
                    if(mDownloadServices[i].IsBusy)
                        infos[i] = mDownloadServices[i].FileDownInfo;
                }

                return infos;
            }
            finally
            {
                Program.InfoUnlock(this, new System.Diagnostics.StackFrame());
            }

        }

        /// <summary>
        /// 当前路径文件是否正在下载
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsFileDownloading(string url)
        {
            lock (this)
            //////try
            {
                //////Program.InfoLock(this, new System.Diagnostics.StackFrame());

                // 下载中和等待下载中的文件都属于正在下载的文件

                url = url.Replace("\\", "/");
                if (mFileInfoDic.ContainsKey(url))
                    return true;

                if (mDownloadingFile.ContainsKey(url))
                    return true;

                return false;
            }
            //////finally
            //////{
            //////    Program.InfoUnlock(this, new System.Diagnostics.StackFrame());
            //////}
        }

        /// <summary>
        /// 文件是否正在等待下载
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsFileWaitDownloading(string url)
        {
            //////lock (this)
            try
            {
                Program.InfoLock(this, new System.Diagnostics.StackFrame());

                url = url.Replace("\\", "/");
                if (mFileInfoDic.ContainsKey(url))
                    return true;

                return false;
            }
            finally
            {
                Program.InfoUnlock(this, new System.Diagnostics.StackFrame());
            }
        }

        /// <summary>
        /// 同步下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="targetFileName"></param>
        /// <param name="overwriteFile"></param>
        /// <param name="md5"></param>
        /// <param name="tag"></param>        
        public void DownloadFileSync(string url, string targetFileName, bool overwriteFile, string md5 = "", object tag = null)
        {
            //////lock (this)
            try
            {
                Program.InfoLock(this, new System.Diagnostics.StackFrame());

                url = url.Replace("\\", "/");

                if(mDownloadedUrl.Contains(url))
                    return;

                if(mDownloadingFile.ContainsKey(url))
                    return;

                FileDownInfo fileInfo;
                if(mFileInfoDic.TryGetValue(url, out fileInfo))
                {
                    fileInfo.Proiority = int.MaxValue;
                    mFileInfoDic.Remove(url);

                    if(tag != null)
                        fileInfo.Tag = tag;
                }
                else
                {
                    fileInfo = FileDownInfo.AddDownFile(url, targetFileName, overwriteFile, md5);
                    fileInfo.Proiority = int.MaxValue;
                    fileInfo.Tag = tag;
                }

                if (mFileInfos.Contains(fileInfo))
                    mFileInfos.Remove(fileInfo);

                mSyncDownloadService.DownloadFile(fileInfo);
            }
            finally
            {
                Program.InfoUnlock(this, new System.Diagnostics.StackFrame());
            }
        }

        /// <summary>
        /// 重新下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="targetFileName"></param>
        /// <param name="overwriteFile"></param>
        /// <param name="md5"></param>
        /// <param name="isEmergency"></param>
        /// <param name="proiority"></param>
        /// <param name="tag"></param>
        public void ReDownloadFile(string url, string targetFileName, bool overwriteFile, string md5, bool isEmergency = false, int proiority = 1, object tag = null)
        {
            var fileInfo = AddDownloadFile(url, targetFileName, overwriteFile, md5, isEmergency, proiority, tag, true);

            if (OnReDownloadFile != null)
                OnReDownloadFile(fileInfo);
        }

        /// <summary>
        /// 重新下载文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isEmergency"></param>
        public void ReDownloadFile(FileDownInfo info, bool isEmergency)
        {
            AddDownloadFile(info, isEmergency, true);

            if (OnReDownloadFile != null)
                OnReDownloadFile(info);
        }

        /// <summary>
        /// 添加文件到下载队列
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isEmergency"></param>
        /// <param name="forceDownload"></param>
        public void AddDownloadFile(FileDownInfo info, bool isEmergency, bool forceDownload = false)
        {
            lock (this)
            {
                if (mDownloadedUrl.Contains(info.Url))
                    return;

                if (!forceDownload && mDownloadingFile.ContainsKey(info.Url))
                    return;

                FileDownInfo tempInfo;
                if (mFileInfoDic.TryGetValue(info.Url, out tempInfo))
                {
                    if (tempInfo.Proiority != int.MaxValue)
                    {
                        if (info.Proiority == 0)
                            tempInfo.Proiority++;
                        else
                            tempInfo.Proiority += info.Proiority;
                    }

                    if (info.Tag != null && tempInfo.Tag == null)
                        tempInfo.Tag = info.Tag;

                    if (mFileInfos.Contains(tempInfo))
                        SortFileInfos();
                    else if (isEmergency)
                    {
                        mFileInfos.Add(tempInfo);
                        SortFileInfos();
                    }
                }
                else
                {
                    mFileInfoDic[info.Url] = info;
                    if (isEmergency)
                    {
                        mFileInfos.Add(info);
                        SortFileInfos();
                    }
                }
            }
        }
        
        /// <summary>
        /// 改变下载文件的优先级 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="delta"></param>
        public void ChangeDownloadFileProiority(string url, int delta)
        {
            lock(this)
            {
                FileDownInfo fileInfo;
                if (mFileInfoDic.TryGetValue(url, out fileInfo))
                {
                    fileInfo.Proiority += delta;

                    if (mFileInfos.Contains(fileInfo))
                        SortFileInfos();
                }
            }

        }

        /// <summary>
        /// 异步下载文件 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="targetFileName"></param>
        /// <param name="overwriteFile"></param>
        /// <param name="md5"></param>
        /// <param name="isEmergency">优先下载</param>
        /// <param name="proiority"></param>
        /// <param name="tag"></param>
        /// <param name="forceDownload"></param>
        /// <returns></returns>
        public FileDownInfo AddDownloadFile(string url, string targetFileName, bool overwriteFile, string md5, bool isEmergency = false, int proiority = 1, object tag = null, bool forceDownload = false)
        {
            //////lock (this)
            try
            {
                Program.InfoLock(this, new System.Diagnostics.StackFrame());

                url = url.Replace("\\", "/");
                if (mDownloadedUrl.Contains(url))
                    return null;

                if (!forceDownload && mDownloadingFile.ContainsKey(url))
                    return null;

                FileDownInfo fileInfo;
                if (mFileInfoDic.TryGetValue(url, out fileInfo))
                {
                    if (fileInfo.Proiority != int.MaxValue)
                    {
                        if (proiority == 0)
                            fileInfo.Proiority++;
                        else
                            fileInfo.Proiority += proiority;
                    }

                    if(tag != null)
                    {
                        fileInfo.Tag = tag;
                    }

                    if (mFileInfos.Contains(fileInfo))
                        SortFileInfos();
                    else if(isEmergency)
                    {
                        mFileInfos.Add(fileInfo);
                        SortFileInfos();
                    }
                }
                else
                {
                    fileInfo = FileDownInfo.AddDownFile(url, targetFileName, overwriteFile, md5);
                    fileInfo.Proiority = proiority;
                    fileInfo.Tag = tag;

                    //// 获取大小
                    //fileInfo.FileSize = FileDownInfo.GetUrlSize(fileInfo.Url);
                    //mTotalFileSize += fileInfo.FileSize;

                    mFileInfoDic[url] = fileInfo;
                    if (isEmergency)
                    {
                        mFileInfos.Add(fileInfo);
                        SortFileInfos();
                    }
                }

                return fileInfo;
            }
            finally
            {
                Program.InfoUnlock(this, new System.Diagnostics.StackFrame());
            }

            //if (mDownloadThread == null || mDownloadThread.ThreadState != System.Threading.ThreadState.Running)
            //    StartDownloadThread();
            //ChooseDownloadServiceToDownloadFile();
        }

        private void ChooseDownloadServiceToDownloadFile()
        {
            lock (this)
            {
                try
                {
                    //Program.InfoLock(this, new System.Diagnostics.StackFrame());

                    if (mFileInfoDic.Count <= 0)
                        return;

                    FileDownloadService service = null;
                    if (mDownloadServices.Count == 0)
                    {
                        service = new FileDownloadService();
                        service.OnDownloadCompleted = _OnFileComplateReport;
                        service.OnProgressReport = _OnProgressReport;
                        service.OnErrorReport = _OnErrorReport;
                        service.OnCancelDownloading = _OnCancelDownloading;
                        mDownloadServices.Add(service);

                        if (LimitSpeed > 0)
                        {
                            foreach (var sev in mDownloadServices)
                            {
                                sev.LimitSpeed = LimitSpeed / mDownloadServices.Count;
                            }
                        }
                    }
                    else
                    {
                        foreach (var sv in mDownloadServices)
                        {
                            if (sv.IsBusy)
                                continue;

                            service = sv;
                            break;
                        }

                        if (service == null)
                        {
                            if (mDownloadServices.Count < MaxDownloadServiceCount)
                            {
                                service = new FileDownloadService();
                                service.OnDownloadCompleted = _OnFileComplateReport;
                                service.OnProgressReport = _OnProgressReport;
                                service.OnErrorReport = _OnErrorReport;
                                service.OnCancelDownloading = _OnCancelDownloading;
                                mDownloadServices.Add(service);

                                if (LimitSpeed > 0)
                                {
                                    foreach (var sev in mDownloadServices)
                                    {
                                        sev.LimitSpeed = LimitSpeed / mDownloadServices.Count;
                                    }
                                }
                            }
                        }
                    }

                    if (service != null)
                    {
                        {
                            if (mFileInfos.Count > 0)
                            {
                                var info = mFileInfos[0];
                                mFileInfos.RemoveAt(0);
                                mDownloadingFile[info.Url] = info;
                                mFileInfoDic.Remove(info.Url);
                                service.DownloadFileAsync(info);
                                mDownloadedFiles[info] = 0;
                            }
                            else if (mFileInfoDic.Count > 0)
                            {
                                foreach (var info in mFileInfoDic)
                                {
                                    mDownloadingFile[info.Value.Url] = info.Value;
                                    mFileInfoDic.Remove(info.Key);
                                    service.DownloadFileAsync(info.Value);
                                    mDownloadedFiles[info.Value] = 0;
                                    break;
                                }
                            }
                        }
                    }

                }
                finally
                {
                    //Program.InfoUnlock(this, new System.Diagnostics.StackFrame());
                }
            }

        }

        private void _OnFileComplateReport(FileDownloadService service)
        {
            if (OnFileDownloadComplete != null)
                OnFileDownloadComplete(service.FileDownInfo);

            //////lock (this)
            try
            {
                Program.InfoLock(this, new System.Diagnostics.StackFrame());

                mDownloadedUrl.Add(service.FileDownInfo.Url);
                mDownloadingFile.Remove(service.FileDownInfo.Url);
            }
            finally
            {
                Program.InfoUnlock(this, new System.Diagnostics.StackFrame());
            }
        }

        private void _OnProgressReport(FileDownloadService service, float percent)
        {
            if (OnProgressReport != null)
            {
                float totalPercent = 0;
                //////lock (this)
                try
                {
                    Program.InfoLock(this, new System.Diagnostics.StackFrame());

                    // 计算下载的百分比
                    mDownloadedFiles[service.FileDownInfo] = percent;

                    //foreach (var data in mDownloadedFiles)
                    //{
                    //    currentSize += (Int64)(data.Key.FileSize * data.Value);
                    //}

                    totalPercent = DownloadedFilesCount * 1.0f / TotalDownloadFilesCount;
                }
                finally
                {
                    Program.InfoUnlock(this, new System.Diagnostics.StackFrame());
                }

                OnProgressReport(totalPercent, percent);
            }

            System.Threading.Thread.Sleep(mSleepTime);
        }
        private void _OnErrorReport(FileDownloadService service, DownloadException ex)
        {
            mErrorList.Add(service.FileDownInfo);
            if (OnErrorReport != null)
                OnErrorReport(service.FileDownInfo, ex);

            //else
            //{
            //    ChooseDownloadServiceToDownloadFile();
            //}
        }
        private void _OnCancelDownloading(FileDownloadService service)
        {
        }


        public void Tick()
        {
            ChooseDownloadServiceToDownloadFile();

            mSyncDownloadService.Tick();
            foreach (var service in mDownloadServices)
            {
                service.Tick();
            }

            if (mDownloadServices.Count > 0)
            {
                if (mFileInfoDic.Count <= 0 && mDownloadingFile.Count <= 0)
                {
                    foreach (var service in mDownloadServices)
                    {
                        service.StopDownload();
                    }
                    mDownloadServices.Clear();
                    mDownloadedFiles.Clear();

                    if (OnDownloadComplete != null)
                        OnDownloadComplete();
                }
            }
        }
    }
}
