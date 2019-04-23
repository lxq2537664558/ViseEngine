namespace CCore.Navigation
{
    //#define DDefaultTextureWidth 1024
    //#define DDefaultTextureHeight 1024
    //#define XMeterPerPixel 0.5f			// X向一个像素表示多少米
    //#define ZMeterPerPixel 0.5f			// Z向一个像素表示多少米
    //#define DefaultNavigationBlockAngleDelta 35.0f		// 生成阻挡的坡度(角度)
    
    /// <summary>
    /// 寻路生成参数
    /// </summary>
    public class NavigationGenerateInfo
    {
        /// <summary>
        /// 生成阻挡的坡度(角度)
        /// </summary>
        public float mTerrainBlockAngleDelta = 35.0f;
        /// <summary>
        /// 清除人工数据
        /// </summary>
        public bool mClearManualData = false;
    }
}
