using System;

namespace CCore.Support
{
    /// <summary>
    /// 转变类
    /// </summary>
    public class DDSConvert
    {
        /// <summary>
        /// 创建实例对象
        /// </summary>
        /// <returns>返回创建的对象的指针</returns>
        public static IntPtr New()
        {
            return DllImportAPI.v3dDDSConvert_New();
        }
        /// <summary>
        /// 删除相应的对象
        /// </summary>
        /// <param name="ddsc">要删除的对象</param>
        public static void Delete(IntPtr ddsc)
        {
            DllImportAPI.v3dDDSConvert_Delete(ddsc);
        }
        /// <summary>
        /// 创建D3D设备指针
        /// </summary>
        /// <param name="ddsc">DDS对象指针</param>
        /// <param name="hwnd">窗口句柄指针</param>
        /// <returns>返回创建的序号</returns>
        public static int CreateD3DDevice(IntPtr ddsc, IntPtr hwnd)
        {
            return DllImportAPI.v3dDDSConvert_CreateD3DDevice(ddsc, hwnd);
        }
        /// <summary>
        /// 转换DDS
        /// </summary>
        /// <param name="ddsc">DDS对象指针</param>
        /// <param name="szSrcFile">源文件路径</param>
        /// <param name="szDstFile">目标文件路径</param>
        /// <param name="generateMipmap">是否为关联映射</param>
        /// <returns></returns>
        public static int ConvertDDS(IntPtr ddsc, string szSrcFile, string szDstFile, bool generateMipmap)
        {
            return DllImportAPI.v3dDDSConvert_ConvertDDS(ddsc, szSrcFile, szDstFile, generateMipmap);
        }
        /// <summary>
        /// 加载DDS数据
        /// </summary>
        /// <param name="d3dDevice">D3D设备指针</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="pitch">文件的Picth值</param>
        /// <param name="imgWidth">图片的宽</param>
        /// <param name="imgHeight">图片的高</param>
        /// <param name="textureSize">纹理大小</param>
        /// <returns>返回加载的DDS数据</returns>
        public static IntPtr LoadDDSData(IntPtr d3dDevice, string fileName, IntPtr pitch, IntPtr imgWidth, IntPtr imgHeight, IntPtr textureSize)
        {
            return DllImportAPI.v3dDDSConvert_LoadDDSData(d3dDevice, fileName, pitch, imgWidth, imgHeight, textureSize);
        }
        /// <summary>
        /// 删除图片数据
        /// </summary>
        /// <param name="data">图片的数据指针</param>
        public static void DeleteImgData(IntPtr data)
        {
            DllImportAPI.v3dDDSConvert_DeleteImgData(data);
        }
    }
}
