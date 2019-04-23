using System;

namespace CCore.Graphics
{
    /// <summary>
    /// 渲染的纹理类
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public class Texture
    {
        IntPtr mTexturePtr = IntPtr.Zero;    // RenderAPI::ITexture*
        /// <summary>
        /// 只读属性，纹理的内存地址
        /// </summary>
        public IntPtr TexturePtr
        {
            get { return mTexturePtr; }
        }

        string mSrcFile;
        /// <summary>
        /// 只读属性，源文件名
        /// </summary>
        public string SrcFile
        {
            get { return mSrcFile; }
        }

        UInt32[] mTextureDatas = new UInt32[]{0, 0};
        /// <summary>
        /// 只读属性，源文件是否存在
        /// </summary>
        public bool ResourceValid
        {
            get
            {
                if(mTexturePtr == IntPtr.Zero || DllImportAPI.ITexture_GetResouceSize(mTexturePtr) == 0)
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 只读属性，纹理尺寸
        /// </summary>
        public uint ResSize
        {
            get
            {
                return DllImportAPI.ITexture_GetResouceSize(mTexturePtr);
            }
        }
        /// <summary>
        /// 只读属性，引用计数器
        /// </summary>
        public long RefCount
        {
            get
            {
                long nRef = DllImportAPI.ITexture_AddRef(mTexturePtr);
                DllImportAPI.ITexture_Release(mTexturePtr);
                return nRef;
            }
        }
        /// <summary>
        /// 只读属性，图片纹理的宽度
        /// </summary>
        public uint Width
        {
            get { return DllImportAPI.ITexture_GetImageInfoWidth(mTexturePtr); }
        }
        /// <summary>
        /// 只读属性，图片纹理的高度
        /// </summary>
        public uint Height
        {
            get { return DllImportAPI.ITexture_GetImageInfoHeight(mTexturePtr); }
        }
        /// <summary>
        /// 只读属性，图片纹理的MipLevel
        /// </summary>
        public uint MipLevel
        {
            get { return DllImportAPI.ITexture_GetImageInfoMipLevels(mTexturePtr); }
        }
        /// <summary>
        /// 只读属性，图片纹理的格式
        /// </summary>
        public BufferFormat Format
        {
            get { return (BufferFormat)DllImportAPI.ITexture_GetImageInfoFormat(mTexturePtr); }
        }
        /// <summary>
        /// 纹理的彩色空间
        /// </summary>
        public TextureColorSpace ColorSpace
        {
            set { DllImportAPI.ITexture_SetColorSpace(mTexturePtr, (int)value); }
            get { return (TextureColorSpace)DllImportAPI.ITexture_GetColorSpace(mTexturePtr); }
        }
        /// <summary>
        /// 纹理的构造函数
        /// </summary>
        public Texture()
        {

        }
        /// <summary>
        /// 析构函数，删除对象，释放指针内存
        /// </summary>
        ~Texture()
        {
            Cleanup();
        }
        /// <summary>
        /// 清除该实例，释放内存
        /// </summary>
        public void Cleanup()
        {
            mSrcFile = null;
            mTextureDatas = null;
            if(mTexturePtr != IntPtr.Zero)
            {
                DllImportAPI.ITexture_Release(mTexturePtr);
                mTexturePtr = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 加载纹理图片
        /// </summary>
        /// <param name="name">纹理图片的名称</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public bool LoadTexture(string name)
        {
            Cleanup();

            mTexturePtr = DllImportAPI.v3dDevice_TextureMgr_LoadTexture(Engine.Instance.Client.Graphics.Device, name);
            if (mTexturePtr == IntPtr.Zero)
                return false;
            mSrcFile = name;
            return true;
        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="fmt">文件格式</param>
        public void SaveToFile(string name, enD3DXIMAGE_FILEFORMAT fmt)
        {
            //if (mTexture)
            //{
            //    StringManage2Native(name);
            //    //V3_SaveTexture( pChar , mTexture , (D3DXIMAGE_FILEFORMAT)fmt );
            //}
        }
        /// <summary>
        /// 设置图片纹理
        /// </summary>
        /// <param name="pTexture">图片纹理</param>
        public void __SetTexture(IntPtr pTexture)
        {
            DllImportAPI.ITexture_AddRef(pTexture);
            DllImportAPI.ITexture_Release(mTexturePtr);
            mTexturePtr = pTexture;
        }
        /// <summary>
        /// 提前使用该实例
        /// </summary>
        /// <param name="bForce">是否提前使用</param>
        /// <param name="time">提前使用的时间</param>
        public void PreUse(bool bForce, Int64 time)
        {
            if (mTexturePtr != IntPtr.Zero)
            {
                DllImportAPI.ITexture_PreUse(mTexturePtr, bForce, time);

                if (!string.IsNullOrEmpty(mSrcFile))
                {
                    var fileUrl = CSUtility.Program.FullPackageUrl + mSrcFile + ".zip";
                    if (CSUtility.FileDownload.FileDownloadManager.Instance.IsFileDownloading(fileUrl))
                    {
                        CSUtility.FileDownload.FileDownloadManager.Instance.ChangeDownloadFileProiority(fileUrl, 1);
                    }
                }
            }
        }
        /// <summary>
        /// 强迫重新加载纹理图片
        /// </summary>
        /// <param name="name">重新加载的纹理图片名称</param>
		public static void ForceReloadTexture(string name)
        {
            DllImportAPI.v3dDevice_TextureMgr_ForceReloadTexture(Engine.Instance.Client.Graphics.Device, name);
        }
        /// <summary>
        /// 创建图片纹理
        /// </summary>
        /// <param name="width">纹理图片的宽</param>
        /// <param name="height">纹理图片的高</param>
        /// <param name="format">纹理图片的格式</param>
        /// <param name="mipLevels">纹理图片的mipLevel</param>
        /// <param name="usage">经常使用的频率</param>
        /// <param name="pool">纹理池大小</param>
        /// <returns>返回创建好的纹理图片</returns>
        public static Texture CreateTexture(uint width, uint height, int format, uint mipLevels, UInt32 usage, int pool)
        {
            Texture iTexture = new Texture();
            var tex = DllImportAPI.v3dDevice_TextureMgr_CreateTexture(Engine.Instance.Client.Graphics.Device, width, height, format, mipLevels, usage, pool);
            iTexture.__SetTexture(tex);
            DllImportAPI.ITexture_Release(tex);
            return iTexture;
        }
        /// <summary>
        /// 保存提前使用的纹理图片
        /// </summary>
        public void	Flush()
        {
            DllImportAPI.ITexture_PreUse(mTexturePtr, true, Engine.Instance.GetFrameMillisecond());
        }
    }
}
