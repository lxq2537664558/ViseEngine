using System;
using System.IO;
using System.Text;

namespace Log
{
    public class FileLog
    {
        public delegate void Delegate_LogWriteCallback(IntPtr logStr, int flush);
        static Delegate_LogWriteCallback logWriteCallback = new Delegate_LogWriteCallback(_LogWriteCallback);
        
        static FileLog smInstance = new FileLog();
        public static FileLog Instance
        {
            get { return smInstance; }
        }

        protected System.IO.StreamWriter mLog;
        protected System.IO.StreamWriter mDataHeader;

        public FileStream fs;
        private FileLog()
        {
            CSUtility.DllImportAPI.Debug_SetWriteLogStringCallback(logWriteCallback);
        }
        ~FileLog()
        {
            CSUtility.DllImportAPI.Debug_UnSetWriteLogStringCallback();
        }

        private static void _LogWriteCallback(IntPtr logStr, int flush)
        {
            var info = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(logStr);
            WriteLine(info);
            //var info = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(logStr);
            //WriteLine(info);
        }
        
        string mPath = "";
   
        public void Begin(string logName, bool bCreateDataHeader = false)
        {
            System.DateTime dateTime = System.DateTime.Now;
            var strTime = dateTime.Month + "_" + dateTime.Day + "_" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second;

            var mExePath = CSUtility.Support.IFileManager.Instance.Bin;//System.AppDomain.CurrentDomain.BaseDirectory;
            var logDir = mExePath + "log/";
            if(!System.IO.Directory.Exists(logDir))
                System.IO.Directory.CreateDirectory(logDir);

            mPath = mExePath + "log/" + logName + strTime + ".log";
            fs = new FileStream(mPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            mLog = new System.IO.StreamWriter(fs);
            WriteLine(dateTime.ToString());
            if(bCreateDataHeader)
            {

            }
        }
        public void End()
        {
            if(mLog != null)
            {
                try
                {
                    mLog.Close();
                }
                finally
                {
                    mLog = null;
                }
            }
            if(mDataHeader != null)
            {
                try
                {
                    mDataHeader.Close();
                }
                finally
                {
                    mDataHeader = null;
                }
            }
        }
        public void Flush()
        {
            //lock (this)
            {
                if (mLog != null)
                {
                    mLog.Flush();
                }
                if (mDataHeader != null)
                {
                    mDataHeader.Flush();
                }
            }
        }

        public static void WriteLine(string str, bool flush = false)
        {
            lock (smInstance)
            {
                if (str == null)
                    return;
                str = System.DateTime.Now.ToString() + ":" + str;
                System.Diagnostics.Trace.WriteLine(str);
                if (smInstance.mLog == null)
                    return;

                smInstance.mLog.WriteLine(str);

                if (flush)
                {
                    smInstance.Flush();
                }
            }
        }

        public static void WriteLine(string format, params object[] args)
        {
            lock (smInstance)
            {
                var str = System.String.Format(format, args);
                if (str == null)
                    return;
                str = System.DateTime.Now.ToString() + ":" + str;
                System.Diagnostics.Trace.WriteLine(str);
                if (smInstance.mLog == null)
                    return;

                smInstance.mLog.WriteLine(str);
            }
        }

        public static void WriteDataHead(string str)
        {
            if(smInstance.mDataHeader == null)
                return;

            smInstance.mDataHeader.WriteLine(str);
        }
        public string ReadLog()
        {
            Flush();
            using (FileStream fs = new FileStream(mPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var buffer = new byte[fs.Length];
                fs.Position = 0;
                fs.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
