using System.Collections.Generic;

namespace CCore.Localization
{
    /// <summary>
    /// 代码管理器类
    /// </summary>
    public class ICodeStringManager
    {
        /// <summary>
        /// 代码名称关键字类
        /// </summary>
        class CodeStringKey
		{
            /// <summary>
            /// 文件名称
            /// </summary>
			public string File;
            /// <summary>
            /// 行数
            /// </summary>
			public int Line;
            /// <summary>
            /// 每一行的列数
            /// </summary>
			public int LineColume;
		};
        /// <summary>
        /// 获取文件和行数
        /// </summary>
        /// <param name="frame">Stack类型的框架</param>
        /// <returns>返回代码的关键字</returns>
		CodeStringKey GetFileAndLine(System.Diagnostics.StackFrame frame)
        {
            var st = new System.Diagnostics.StackTrace(frame);
			var sf = st.GetFrame(0);
			if(sf==null)
				return null;
			var csk = new CodeStringKey();
			csk.File = sf.GetFileName();
			csk.Line = sf.GetFileLineNumber();
			csk.LineColume = sf.GetFileColumnNumber();
			return csk;
        }

		Dictionary<CodeStringKey, string> mStrings = new Dictionary<CodeStringKey, string>();
		static ICodeStringManager mInstance = new ICodeStringManager();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static ICodeStringManager Instance
        {
            get{ return mInstance; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
		private ICodeStringManager()
		{
		}
        /// <summary>
        /// 析构函数
        /// </summary>
        ~ICodeStringManager()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除创建的对象
        /// </summary>
        public void Cleanup()
        {
            mStrings.Clear();
        }
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="protocol">代码关键字</param>
        /// <returns>返回生成的代码</returns>
		public string MappingString(string protocol)
        {
            var csk = GetFileAndLine( new System.Diagnostics.StackFrame(1,true) );
            if(csk==null)
                return protocol;
			string str;
			if( mStrings.TryGetValue(csk, out str) )
			{
				return str;
			}
			return protocol;
        }
    }
}
