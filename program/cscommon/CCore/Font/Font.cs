using System;

namespace CCore.Font
{
    /// <summary>
    /// 字体类
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public class Font
    {
        /// <summary>
        /// 对象实例的指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 实例内存地址
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                mInner = value;
            }
        }
        /// <summary>
        /// 字体的内存地址
        /// </summary>
        public IntPtr FontPtr
        {
            get{return mInner;}
        }

        /// <summary>
        /// 字体的构造函数
        /// </summary>
        public Font()
        {
        }
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
		~Font()
        {
            DllImportAPI.V3DFontW_Release(mInner);
            mInner = IntPtr.Zero;
        }

        static Font mDefFont = null;
        /// <summary>
        /// 字体单例
        /// </summary>
        public static Font DefFont
		{
			get
            {
                if(mDefFont==null)
                {
                    var hFont = DllImportAPI.V3DFontW_CreateDefaultFont();
			        mDefFont = new Font();
                    mDefFont.CreateFont(hFont);
                }
                return mDefFont;
            }
		}
        /// <summary>
        /// 根据字体地址创建字体
        /// </summary>
        /// <param name="hFont">字体地址</param>
        public void CreateFont(IntPtr hFont)
        {
            if (mInner != null)
            {
                DllImportAPI.V3DFontW_Release(mInner);
                mInner = IntPtr.Zero;
            }

            mInner = DllImportAPI.V3DFontW_New();
            DllImportAPI.V3DFontW_InitObjects(mInner, Engine.Instance.Client.Graphics.Device, hFont, 2, 1024);
        }
    }
}
