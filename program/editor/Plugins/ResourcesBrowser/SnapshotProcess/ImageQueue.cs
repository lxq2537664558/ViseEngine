using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace ResourcesBrowser.SnapshotProcess
{
    public class SnapshotProcessInfo
    {
        public ResourceInfo ResourceInfo = null;
        public bool ForceCreate = false;
    }

    /// <summary>
    /// 图片加载队列
    /// </summary>
    public class ImageQueue
    {
        #region 辅助类别
        private class ImageQueueInfo
        {
            public ResourceInfo sourceInfo { get; set; }
            public bool bForceCreate = false;
            public BrowserControl HostBrowserControl;
        }
        #endregion
        public delegate void ComplateDelegate(ResourceInfo i, System.Windows.Media.ImageSource b);
        //public event ComplateDelegate OnComplate;
        //private static AutoResetEvent autoEvent = null;
        //private static Queue<ImageQueueInfo> Stacks = null;
        private static List<ImageQueueInfo> InfoList = null;
        private static ImageQueue m_imageQueue = new ImageQueue();
        private bool mLoadImageThreadRuning = false;
        public bool NeedSort = false;
        public static ImageQueue Instance
        {
            get { return m_imageQueue; }
        }
        public ImageQueue()
        {
            //Stacks = new Queue<ImageQueueInfo>();
            //autoEvent = new AutoResetEvent(true);
            //Thread t = new Thread(new ThreadStart(LoadImage));
            ////t.ApartmentState = ApartmentState.STA;
            //t.Name = "加载图片";
            //t.IsBackground = true;
            //t.Start();
            InfoList = new List<ImageQueueInfo>();

            StartThread();
        }

        public void StartThread()
        {
            //Stacks = new Queue<ImageQueueInfo>();
            //autoEvent = new AutoResetEvent(true);
            mLoadImageThreadRuning = true;
            Thread t = new Thread(new ThreadStart(LoadImage));
            //t.ApartmentState = ApartmentState.STA;
            t.Name = "加载图片";
            t.IsBackground = true;
            t.Start();
        }
        public void StopThread()
        {
            mLoadImageThreadRuning = false;
        }

        static System.DateTime preDateTime = System.DateTime.Now;
        private void LoadImage()
        {
            while (mLoadImageThreadRuning)
            {
                var time = System.DateTime.Now - preDateTime;
                if (time.TotalMilliseconds < 20)
                {
                    Thread.Sleep(1);
                    continue;
                }
                else
                {
                    preDateTime = System.DateTime.Now;
                }

                ImageQueueInfo t = null;
                lock (InfoList)
                {
                    if (InfoList.Count > 0)
                    {
                        t = InfoList[0];
                        InfoList.Remove(t);
                    }

                    NeedSort = false;
                }
                if (t != null)
                {
                    try
                    {
                        SnapshotProcessInfo spInfo = new SnapshotProcessInfo()
                        {
                            ResourceInfo = t.sourceInfo,
                            ForceCreate = t.bForceCreate,
                        };
                        t.HostBrowserControl.Dispatcher.BeginInvoke(new Action<ImageQueueInfo>((queueInfo)=>
                        {
                            queueInfo.sourceInfo.Snapshot = queueInfo.sourceInfo.GetSnapshotImage(queueInfo.bForceCreate);
                        }), new object[] { t });
                        //System.Windows.Media.ImageSource image = t.sourceInfo.GetSnapshotImage(t.bForceCreate);
                        //SnapshotProcess.SnapshotCreator.Instance.SourceType = t.sourceInfo.ResourceType;

                        //if (image == null)
                        //    image = Program.LoadImage("");

                        ////if ("http".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase))
                        ////{
                        ////    //如果是HTTP下载文件
                        ////    WebClient wc = new WebClient();
                        ////    using (var ms = new MemoryStream(wc.DownloadData(uri)))
                        ////    {
                        ////        image = new BitmapImage();
                        ////        image.BeginInit();
                        ////        image.CacheOption = BitmapCacheOption.OnLoad;
                        ////        image.StreamSource = ms;
                        ////        image.EndInit();
                        ////    }
                        ////}
                        ////else if ("file".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase))
                        ////{
                        ////    using (var fs = new FileStream(t.url, FileMode.Open))
                        ////    {
                        ////        image = new BitmapImage();
                        ////        image.BeginInit();
                        ////        image.CacheOption = BitmapCacheOption.OnLoad;
                        ////        image.StreamSource = fs;
                        ////        image.EndInit();
                        ////    }
                        ////}
                        //if (image != null)
                        //{
                        //    if (image.CanFreeze) image.Freeze();
                        //    image.Dispatcher.BeginInvoke(new Action<ImageQueueInfo, System.Windows.Media.ImageSource>((i, bmp) =>
                        //    {
                        //        if (OnComplate != null)
                        //        {
                        //            OnComplate(i.sourceInfo, bmp);
                        //        }
                        //    }), new Object[] { t, image });
                        //}
                    }
                    catch (Exception e)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"资源浏览器: 加载缩略图失败（{t.sourceInfo.Name}）\r\n{e.ToString()}");
                        continue;
                    }
                }

                Thread.Sleep(30);
                //if (Stacks.Count > 0) continue;
                if (InfoList.Count > 0) continue;
                //autoEvent.WaitOne(3000, false);
            }
        }
        //public static void Queue(Image img, String url)
        public void Queue(ResourceInfo sCtrl, bool forceCreate = false)
        {
            //if (String.IsNullOrEmpty(url)) return;
            //lock (Stacks)
            //{
            //    Stacks.Enqueue(new ImageQueueInfo { sourceObj = obj, sourceControl = sCtrl, bForceCreate = forceCreate });
            //    autoEvent.Set();
            //}
            lock (InfoList)
            {
                foreach (var info in InfoList)
                {
                    if(info.sourceInfo.Id == sCtrl.Id)
                        return;
                }

                InfoList.Add(new ImageQueueInfo { 
                    sourceInfo = sCtrl, 
                    bForceCreate = forceCreate,
                    HostBrowserControl = sCtrl.ParentBrowser});

                NeedSort = true;

                //autoEvent.Set();
            }
        }

        public void Dequeue(ResourceInfo sCtrl)
        {
            lock (InfoList)
            {
                foreach (var info in InfoList)
                {
                    if (info.sourceInfo.Equals(sCtrl))
                    {
                        InfoList.Remove(info);
                        break;
                    }
                }
            }
        }

        public void Clear()
        {
            lock(InfoList)
            {
                InfoList.Clear();
            }
        }
    }
}
