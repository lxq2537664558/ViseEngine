using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CSUtility.Support
{
    public class StringFilterHelper
    {
        private Regex mRegex;

        #region Singleton
        private static readonly StringFilterHelper smInstance = new StringFilterHelper();
        public static StringFilterHelper Instance { get { return smInstance; } }
        private StringFilterHelper() { }
        #endregion

        public void InitFilter(string textFile)
        {
            System.IO.FileStream fs;
            if (System.IO.File.Exists(textFile))
            {
                fs = new System.IO.FileStream(textFile, FileMode.Open, FileAccess.Read);
                using (fs)
                {
                    ////TextRange text = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                    //text.Load(fs, DataFormats.Text);

                    //创建一个容量4M的数组
                    byte[] byteData = new byte[fs.Length];
                    //从文件中读取数据写入到数组中(数组对象，从第几个开始读，读多少个)
                    //返回读取的文件内容真实字节数
                    int length = fs.Read(byteData, 0, byteData.Length);
                    //如果字节数大于0，则转码
                    if (length > 0)
                    {
                        //将数组转以UTF-8字符集编码格式的字符串
                        var filterst = ".*(" + Encoding.UTF8.GetString(byteData) + ").*";
                        mRegex=new Regex(filterst);
                    }

                }
            }
            else
            {
                throw new FileNotFoundException("找不到屏蔽字库");
            }
        }

        /// <summary>
        /// 判断是否屏蔽
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsStFilter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                //throw new Exception("输入不能为空");
                return true;
            }
            if (mRegex == null)
            {
                //ReadRex();
                return false;
            }

            if (mRegex.IsMatch(input))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ReplaceIllegal(string input, string placeStr)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            if (mRegex == null)
            {
                return input;
            }
            return mRegex.Replace(input, placeStr);
        }
    }
}
