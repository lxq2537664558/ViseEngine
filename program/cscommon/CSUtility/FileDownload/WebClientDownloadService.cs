using System;
using System.Net;
using System.IO;

namespace CSUtility.FileDownload
{
    // 下载服务，每个服务在一个线程中执行, 每一个下载服务只能
    public class FileDownloadService : IDisposable
    {
        public delegate void Delegate_CompleteReport(FileDownloadService service);
        public Delegate_CompleteReport OnDownloadCompleted;
        public delegate void Delegate_OnProgressReport(FileDownloadService service, float percent);
        public Delegate_OnProgressReport OnProgressReport;
        public delegate void Delegate_ErrorReport(FileDownloadService service, DownloadException ex);
        public Delegate_ErrorReport OnErrorReport;
        public delegate void Delegate_CancelDownloading(FileDownloadService service);
        public Delegate_CancelDownloading OnCancelDownloading;

        private Guid mServiceId = Guid.NewGuid();
        private WebClient mWebClient = new WebClient();

        public void Dispose()
        {
            mWebClient.Dispose();
            mWebClient = null;
            GC.SuppressFinalize(this);
        }

        System.Threading.Thread mDownloadThread;

        public bool IsBusy
        {
            get
            {
                return mAsyncDownloadState != enDownloadState.Waiting;
            }
        }

        enum enDownloadState
        {
            Waiting,
            Downloading,
            Error,
            Complate,
            Cancel,
        }
        enDownloadState mAsyncDownloadState = enDownloadState.Waiting;

        private FileDownInfo mFileDownInfo;
        public FileDownInfo FileDownInfo
        {
            get { return mFileDownInfo; }
        }

        /// <summary>
        /// 下载限速(byte/秒)
        /// </summary>        
        Int64 mLimitSpeed = 0;
        public Int64 LimitSpeed
        {
            get { return mLimitSpeed; }
            set
            {
                // 最大下载速度4G
                if (value > int.MaxValue)
                    mLimitSpeed = int.MaxValue;
                else if (value == 0)    // 0为不限速
                    mLimitSpeed = 0;
                else if (value < 1024)  // 最小 1k/秒
                    mLimitSpeed = 1024;
                else
                    mLimitSpeed = value;
            }
        }

        /// <summary>
        /// 下载速度(byte/秒)
        /// </summary>        
        Int32 mDownloadSpeed = 0;
        public Int32 DownloadSpeed
        {
            get { return mDownloadSpeed; }
        }

        // 重试次数
        public Byte RetryTime = 5;
        Byte mRetryTimeRemain = 5;  // 至少为1次

        DownloadException mDownloadException = null;

        bool mDownloadThreadRunning = false;

        ~FileDownloadService()
        {
            StopDownload();
        }

        private void StartDownloadThread()
        {
            if (!mDownloadThreadRunning)
            {
                if (mDownloadThread != null)
                    mDownloadThread.Abort();

                mDownloadThreadRunning = true;
                mDownloadThread = new System.Threading.Thread(new System.Threading.ThreadStart(DownloadThreadTick));
                mDownloadThread.Name = "Downloading Thread:" + mServiceId;
                mDownloadThread.Start();
            }
        }
        private void StopDownloadThread()
        {
            mDownloadThreadRunning = false;

            if (mAsyncDownloadState == enDownloadState.Downloading)
            {
                mAsyncDownloadState = enDownloadState.Cancel;
            }
        }

        /// <summary>
        /// 停止下载
        /// </summary>
        public void StopDownload()
        {
            StopDownloadThread();
        }

        /// <summary>
        /// 异步下载文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>        
        public bool DownloadFileAsync(FileDownInfo info)
        {

            if(IsBusy)
                return false;

            if (string.IsNullOrEmpty(info.Url) || string.IsNullOrEmpty(info.SavePath))
                return false;

            mFileDownInfo = info;

            mRetryTimeRemain = RetryTime;

            StartDownloadThread();
            mAsyncDownloadState = enDownloadState.Downloading;

            return true;
        }

        /// <summary>
        /// 同步下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="overwriteFile"></param>
        /// <param name="md5"></param>
        public void DownloadFile(string url, string fileName, bool overwriteFile, string md5 = "")
        {
            mFileDownInfo = FileDownInfo.AddDownFile(url, fileName, overwriteFile, md5);
            DownloadFile(mFileDownInfo);
        }

        /// <summary>
        /// 同步下载文件
        /// </summary>
        /// <param name="info"></param>        
        public void DownloadFile(FileDownInfo info)
        {
            try
            {
                var folder = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(info.SavePath);                
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }
                mFileDownInfo = info;
                mFileDownInfo.Status = FileDownInfo.FileDownStatus.Doing;
                mWebClient.DownloadFile(info.Url, info.SavePath);
                mFileDownInfo.Status = FileDownInfo.FileDownStatus.Finish;
            }
            catch (System.Exception ex)
            {
                mDownloadException = new DownloadException(ex.Message, ex);

                System.Diagnostics.Debug.WriteLine(ex.ToString());
                mFileDownInfo.Status = FileDownInfo.FileDownStatus.Error;
            }
        }

        private void DownloadThreadTick()
        {

            while (mDownloadThreadRunning)//mRetryTimeRemain > 0)
            {
                System.Threading.Thread.Sleep(10);

                if (mFileDownInfo == null)
                {
                    mAsyncDownloadState = enDownloadState.Waiting;
                    continue;
                }

                if (mAsyncDownloadState != enDownloadState.Downloading)
                {
                    continue;
                }

                try
                {
                    mFileDownInfo.BeginTime = System.DateTime.Now;
                    mFileDownInfo.Status = FileDownInfo.FileDownStatus.Doing;

                    using (var stream = mWebClient.OpenRead(mFileDownInfo.Url))
                    {
                        var targetMD5 = mWebClient.ResponseHeaders.Get("Content-MD5");
                        if (!string.IsNullOrEmpty(mFileDownInfo.MD5))
                        {
                            if (!string.IsNullOrEmpty(targetMD5))
                            {
                                if (mFileDownInfo.CheckMD5 && mFileDownInfo.MD5 != targetMD5)
                                {
                                    // 抛出MD5码不匹配异常
                                    throw new DownloadException("MD5不匹配", DownloadException.enExceptionType.MD5NotMatch);
                                }
                            }
                        }
                        else
                            mFileDownInfo.MD5 = targetMD5;

                        var fileLength = System.Convert.ToInt64(mWebClient.ResponseHeaders.Get("Content-Length"));
                        mFileDownInfo.FileSize = fileLength;

                        // 如果要支持断点续传，则这里需要把读出来的流写入文件

                        Int64 tempLimitSpeed = LimitSpeed;
                        if (tempLimitSpeed == 0)
                            tempLimitSpeed = int.MaxValue;

                        var limitSpeedPerMS = tempLimitSpeed / 1000; // byte/毫秒

                        // 不支持超过4个G的文件下载
                        byte[] readBytes = new byte[fileLength];
                        var byteLengthRemaind = fileLength;
                        int byteStart = 0;
                        int readCount = (byteLengthRemaind > int.MaxValue) ? int.MaxValue : (int)byteLengthRemaind;

                        var startTime = System.DateTime.Now;
                        int readBytesTotal_Speed = 0;
                        while (byteLengthRemaind > 0)
                        {
                            if (!mDownloadThreadRunning)
                                return;

                            if (readCount > byteLengthRemaind)
                                readCount = (int)byteLengthRemaind;

                            var readedBytesCount = stream.Read(readBytes, byteStart, readCount);

                            readBytesTotal_Speed += readedBytesCount;
                            byteLengthRemaind -= readedBytesCount;
                            byteStart += readedBytesCount;

                            if (OnProgressReport != null)
                                OnProgressReport(this, byteStart * 1.0f / fileLength);
                            //System.Diagnostics.Debug.WriteLine("Progress:" + byteStart * 1.0f / fileLength);

                            if (readedBytesCount > limitSpeedPerMS)
                            {
                                var sleepTime = (int)((readedBytesCount - limitSpeedPerMS) / limitSpeedPerMS);
                                if (sleepTime > 1000)
                                {
                                    sleepTime = 1000;
                                    readCount = (int)tempLimitSpeed;
                                }
                                else
                                    readCount = (byteLengthRemaind > int.MaxValue) ? int.MaxValue : (int)byteLengthRemaind;

                                if (readCount == 0)
                                    readCount = (int)byteLengthRemaind;
                                //System.Diagnostics.Debug.WriteLine("Sleep:" + sleepTime + ", ReadCount:" + readCount + ", ReadByte:" + readedBytesCount);
                                System.Threading.Thread.Sleep(sleepTime);
                            }

                            if ((System.DateTime.Now - startTime).TotalSeconds > 1)
                            {
                                startTime = System.DateTime.Now;
                                mDownloadSpeed = readBytesTotal_Speed;
                                readBytesTotal_Speed = 0;
                                //System.Diagnostics.Debug.WriteLine("DownloadSpeed:" + mDownloadSpeed + ", LimitSpeed:" + LimitSpeed);
                            }
                        }

                        var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(mFileDownInfo.SavePath);
                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);

                        using (var fileStream = new FileStream(mFileDownInfo.SavePath, FileMode.Create, FileAccess.Write))
                        {
                            fileStream.Write(readBytes, 0, readBytes.Length);
                            fileStream.Close();
                            //System.Diagnostics.Debug.WriteLine("=== fileStream Close:" + mFileDownInfo.SavePath);
                        }

                        mFileDownInfo.EndTime = System.DateTime.Now;
                        mFileDownInfo.Status = FileDownInfo.FileDownStatus.Finish;

                        stream.Close();
                        //System.Diagnostics.Debug.WriteLine("=== stream Close:" + mFileDownInfo.SavePath);

                        if (mFileDownInfo.UnzipWhenDownloadComplate)
                        {
                            CSUtility.Compress.CompressManager.Instance.UnZipFile(mFileDownInfo.SavePath, mFileDownInfo.UnzipFolder);
                        }

                        mAsyncDownloadState = enDownloadState.Complate;
                        //System.Diagnostics.Debug.WriteLine("=== Complate:" + mFileDownInfo.SavePath);
                    }
                }
                catch (System.Exception ex)
                {
                    //if (mDownloadThreadRunning)
                    //    return;

                    mRetryTimeRemain--;
                    if (mRetryTimeRemain <= 0)
                    {
                        mDownloadException = new DownloadException(ex.Message, ex);
                        mFileDownInfo.Status = FileDownInfo.FileDownStatus.Error;

                        System.Diagnostics.Debug.WriteLine(ex.ToString());

                        mAsyncDownloadState = enDownloadState.Error;
                    }
                }

            }
        }

        public void Tick()
        {
            switch (mAsyncDownloadState)
            {
                case enDownloadState.Waiting:
                    {

                    }
                    break;

                case enDownloadState.Downloading:
                    {

                    }
                    break;

                case enDownloadState.Complate:
                    {
                        if (OnDownloadCompleted != null)
                            OnDownloadCompleted(this);

                        mAsyncDownloadState = enDownloadState.Waiting;
                    }
                    break;

                case enDownloadState.Error:
                    {
                        if (OnErrorReport != null)
                            OnErrorReport(this, mDownloadException);

                        mAsyncDownloadState = enDownloadState.Waiting;
                    }
                    break;

                case enDownloadState.Cancel:
                    {
                        //if (mDownloadThread != null)
                        //    mDownloadThread.Abort();

                        if(mFileDownInfo.Status == FileDownInfo.FileDownStatus.Doing)
                            mFileDownInfo.Status = FileDownInfo.FileDownStatus.Wait;

                        if (OnCancelDownloading != null)
                            OnCancelDownloading(this);
                        
                        mAsyncDownloadState = enDownloadState.Waiting;
                    }
                    break;
            }
        }
    }
}
