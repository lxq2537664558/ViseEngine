namespace CSUtility.FileDownload
{
    public class DownloadException : System.Exception
    {
        public enum enExceptionType
        {
            Unknow,
            ConnectClose,
            UrlError,
            Timeout,
            MD5NotMatch,
        }

        public DownloadException(string str, System.Exception ex)
            : base(str, ex)
        {
            switch (HResult)
            {
                case -2146232800:   // 无法从传输连接中读取数据: 连接已关闭
                    mExceptionType = enExceptionType.ConnectClose;
                    break;

                case -2146233079:   // 远程服务器返回错误: (404) 未找到 | 操作已超时
                    mExceptionType = enExceptionType.Timeout;
                    break;

                case -2146233088:   // 无法解析此远程名称 | 远程主机强迫关闭了一个现有的连接
                    mExceptionType = enExceptionType.UrlError;
                    break;

                default:
                    mExceptionType = enExceptionType.Unknow;
                    break;
            }
        }

        public DownloadException(string str, enExceptionType type)
            : base(str)
        {
            mExceptionType = type;
        }

        public int Result
        {
            get { return this.HResult; }
        }

        enExceptionType mExceptionType = enExceptionType.Unknow;
        public enExceptionType ExceptionType
        {
            get { return mExceptionType; }
        }
    }
}
