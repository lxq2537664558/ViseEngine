namespace CSUtility.Compress
{
    public class CompressManager
    {
        public delegate void Delegate_Debug(string file);
        public Delegate_Debug OnDebug;

        public delegate void Delegate_OnComplateUnZip();

        static CompressManager smInstance = new CompressManager();
        public static CompressManager Instance
        {
            get { return smInstance; }
        }


        public static void FinalInstance()
        {
            if (smInstance != null)
            {
                //smInstance.StopThread();
                //smInstance.StopDownloadThread();
                smInstance = null;
            }
        }

        // 实例：inputFiles("aa.txt")(相对路径), rootDir="d:/xxx/" zipFileName = "d:/xxx.zip"
        public static void ZipFile(string inputFile, string rootDir, string zipFileName)
        {
            CSUtility.Compress.SevZip.CompressContext_SevenZip.Instance.CompressFile(inputFile, rootDir, zipFileName);
        }

        public static void ZipFile(string[] inputFiles, string rootDir, string zipFileName)
        {
            CSUtility.Compress.SevZip.CompressContext_SevenZip.Instance.CompressFile(inputFiles, rootDir, zipFileName);
        }

        public void UnZipFile(string zipFileName, string targetFolder)
        {
            try
            {
                CSUtility.Compress.SevZip.CompressContext_SevenZip.Instance.UnCompressFile(zipFileName, targetFolder);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                if (OnDebug != null)
                    OnDebug(zipFileName);
            }
        }
    }
}
