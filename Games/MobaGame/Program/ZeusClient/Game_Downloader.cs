using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public partial class Game
    {
        // 资源列表，用于比对资源的新旧
        Dictionary<string, CSUtility.FileDownload.ResourceData> mResourceDic = new Dictionary<string, CSUtility.FileDownload.ResourceData>();
        public Dictionary<string, CSUtility.FileDownload.ResourceData> ResourceDic
        {
            get { return mResourceDic; }
        }

        // 已经下载的资源
        Dictionary<string, CSUtility.FileDownload.ResourceData> mDownloadedRes = new Dictionary<string, CSUtility.FileDownload.ResourceData>();
        public Dictionary<string, CSUtility.FileDownload.ResourceData> DownloadedRes
        {
            get { return mDownloadedRes; }
        }

        // 出错的资源
        Dictionary<string, CSUtility.FileDownload.ResourceData> mErrorRes = new Dictionary<string, CSUtility.FileDownload.ResourceData>();
        public Dictionary<string, CSUtility.FileDownload.ResourceData> ErrorRes
        {
            get { return mErrorRes; }
        }

        private void InitializeDownloading()
        {
            if (!CSUtility.Program.FinalRelease)
                return;

            if (!System.IO.File.Exists(CSUtility.Support.IFileManager.Instance.Root + "CFiles.cfg"))
                return;

            var gameInfo = new CSUtility.FileDownload.GameInfo();
            gameInfo.Load(CSUtility.Support.IFileManager.Instance.Root + "Game.inf");
            CSUtility.Program.DownloadServiceUrl = gameInfo.DownloadServerUrl;

            // 取得CPU核心数，设置最大下载线程数为CPU核心数
            var cpuInstanceCount = 1;//CSUtility.Support.CPUInfo.GetLogicalProcessorCount();
            if (cpuInstanceCount <= 1)
                CSUtility.FileDownload.FileDownloadManager.Instance.MaxDownloadServiceCount = 1;
            else
                CSUtility.FileDownload.FileDownloadManager.Instance.MaxDownloadServiceCount = (byte)(((cpuInstanceCount > byte.MaxValue) ? byte.MaxValue : (byte)cpuInstanceCount) / 2);

            CSUtility.FileDownload.FileDownloadManager.Instance.LimitSpeed = 300000;
            CSUtility.FileDownload.FileDownloadManager.Instance.MaxDownloadServiceCount = 1;
            CSUtility.FileDownload.FileDownloadManager.Instance.OnReDownloadFile = _OnReDownloadFile;
            CSUtility.FileDownload.FileDownloadManager.Instance.OnDownloadComplete += _OnDownloadComplete;
            CSUtility.FileDownload.FileDownloadManager.Instance.OnFileDownloadComplete += _OnFileDownloadComplete;
            CSUtility.FileDownload.FileDownloadManager.Instance.OnErrorReport += _OnDownloadError;

            // 下载完整信息包
            CSUtility.FileDownload.FileDownloadManager.Instance.DownloadFileSync(CSUtility.Program.FullPackageUrl + "FInfo.cfg.zip", CSUtility.Support.IFileManager.Instance.Root + "temp/FInfo.cfg.zip", true);
            CSUtility.Compress.CompressManager.Instance.UnZipFile(CSUtility.Support.IFileManager.Instance.Root + "temp/FInfo.cfg.zip", CSUtility.Support.IFileManager.Instance.Root + "temp/");
            System.IO.File.Delete(CSUtility.Support.IFileManager.Instance.Root + "temp/FInfo.cfg.zip");
            var newDatas = CSUtility.FileDownload.ResourceDataManager.LoadResourceDatas(CSUtility.Support.IFileManager.Instance.Root + "temp/FInfo.cfg");

            mDownloadedRes = CSUtility.FileDownload.ResourceDataManager.LoadResourceDatas(CSUtility.Support.IFileManager.Instance.Root + "temp/downloadedFiles.cfg");
            mErrorRes = CSUtility.FileDownload.ResourceDataManager.LoadResourceDatas(CSUtility.Support.IFileManager.Instance.Root + "temp/errorFiles.cfg");
            var minPackageDatas = CSUtility.FileDownload.ResourceDataManager.LoadResourceDatas(CSUtility.Support.IFileManager.Instance.Root + "CFiles.cfg");            
            var oldDatas = CSUtility.FileDownload.ResourceDataManager.LoadResourceDatas(CSUtility.Support.IFileManager.Instance.Root + "FInfo.cfg");                                        

            foreach (var resData in mErrorRes)
            {
                if (newDatas.ContainsKey(resData.Key))
                {
                    continue;
                }

                var downloadInfo = CSUtility.FileDownload.FileDownInfo.AddDownFile(CSUtility.Program.FullPackageUrl + resData.Value.RelativeFile + ".zip", CSUtility.Support.IFileManager.Instance.Root + resData.Value.RelativeFile + ".zip", true, "");
                downloadInfo.UnzipWhenDownloadComplate = true;
                downloadInfo.MD5 = resData.Value.MD5;
                downloadInfo.UnzipFolder = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(CSUtility.Support.IFileManager.Instance.Root + resData.Value.RelativeFile);
                CSUtility.FileDownload.FileDownloadManager.Instance.AddDownloadFile(downloadInfo, false);
            }
            mErrorRes.Clear();
            System.IO.File.Delete(CSUtility.Support.IFileManager.Instance.Root + "temp/errorFiles.cfg");

            if (newDatas.Count > 0)
            {
                foreach (var resData in newDatas)
                {
                    CSUtility.FileDownload.ResourceData minData;
                    if (minPackageDatas.TryGetValue(resData.Key,out minData))
                    {
                        if (minData.MD5 == resData.Value.MD5)
                            continue;
                    }                        

                    CSUtility.FileDownload.ResourceData downloadRes = null;
                    if (mDownloadedRes.TryGetValue(resData.Key, out downloadRes))
                    {
                        if (downloadRes.MD5 == resData.Value.MD5)
                            continue;
                    }

                    CSUtility.FileDownload.ResourceData oldData;
                    if (oldDatas.TryGetValue(resData.Key, out oldData))
                    {
                        if (oldData.MD5 == resData.Value.MD5)
                            continue;
                    }

                    if (resData.Value.ResourceType == CSUtility.Support.enResourceType.Folder)
                        continue;

                    var downloadInfo = CSUtility.FileDownload.FileDownInfo.AddDownFile(CSUtility.Program.FullPackageUrl + resData.Value.RelativeFile + ".zip", CSUtility.Support.IFileManager.Instance.Root + resData.Value.RelativeFile + ".zip", true, "");
                    downloadInfo.UnzipWhenDownloadComplate = true;
                    downloadInfo.MD5 = resData.Value.MD5;
                    downloadInfo.UnzipFolder = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(CSUtility.Support.IFileManager.Instance.Root + resData.Value.RelativeFile);
                    CSUtility.FileDownload.FileDownloadManager.Instance.AddDownloadFile(downloadInfo, false);
                }
            }
        }

        private void FinalDownloading()
        {
            CSUtility.FileDownload.FileDownloadManager.FinalInstance();
            CSUtility.FileDownload.ResourceDataManager.SaveResourceDatas(mDownloadedRes, CSUtility.Support.IFileManager.Instance.Root + "temp/downloadedFiles.cfg");
        }

        UInt64 mDownloadedFiles = 0;
        void _OnFileDownloadComplete(CSUtility.FileDownload.FileDownInfo fd)
        {
            lock (mDownloadedRes)
            {
                var fileName = fd.SavePath;
                fileName = fileName.Replace(".zip", "");

                fileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(fileName);
                var data = new CSUtility.FileDownload.ResourceData();
                data.RelativeFile = fileName;
                data.MD5 = fd.MD5;
                mDownloadedRes[fileName] = data;

                mDownloadedFiles++;

                // 每下载一定数量的文件保存下载的文件列表
                if (mDownloadedRes.Count % 10 == 0)
                    CSUtility.FileDownload.ResourceDataManager.SaveResourceDatas(mDownloadedRes, CSUtility.Support.IFileManager.Instance.Root + "temp/downloadedFiles.cfg");

            }
        }

        void _OnDownloadError(CSUtility.FileDownload.FileDownInfo fd, CSUtility.FileDownload.DownloadException ex)
        {
            var fileName = fd.SavePath;
            fileName = fileName.Replace(".zip", "");

            fileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(fileName);
            var data = new CSUtility.FileDownload.ResourceData();
            data.RelativeFile = fileName;
            mErrorRes[fileName] = data;

            CSUtility.FileDownload.ResourceDataManager.SaveResourceDatas(mErrorRes, CSUtility.Support.IFileManager.Instance.Root + "temp/errorFiles.cfg");
        }

        void _OnDownloadComplete()
        {
            if (System.IO.File.Exists(CSUtility.Support.IFileManager.Instance.Root + "temp/FInfo.cfg"))
            {
                if (System.IO.File.Exists(CSUtility.Support.IFileManager.Instance.Root + "FInfo.cfg"))
                {
                    System.IO.File.Delete(CSUtility.Support.IFileManager.Instance.Root + "FInfo.cfg");
                }

                System.IO.File.Move(CSUtility.Support.IFileManager.Instance.Root + "temp/FInfo.cfg", CSUtility.Support.IFileManager.Instance.Root + "FInfo.cfg");
            }

            if (System.IO.File.Exists(CSUtility.Support.IFileManager.Instance.Root + "temp/downloadedFiles.cfg"))
            {
                System.IO.File.Delete(CSUtility.Support.IFileManager.Instance.Root + "temp/downloadedFiles.cfg");
            }
            mDownloadedRes.Clear();
        }

        void _OnReDownloadFile(CSUtility.FileDownload.FileDownInfo fd)
        {
            // 再次添加下载文件说明此文件需要重新下载
            var fileName = fd.SavePath;
            fileName = fileName.Replace(".zip", "");

            fileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(fileName);

            mDownloadedRes.Remove(fileName);

            CSUtility.FileDownload.ResourceDataManager.SaveResourceDatas(mDownloadedRes, CSUtility.Support.IFileManager.Instance.Root + "temp/downloadedFiles.cfg");
        }
    }
}
