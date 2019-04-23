// IMatchFinder.cs

using System;

namespace SevenZip.Compression.LZ
{
    /// <summary>
    /// 窗口消息流
    /// </summary>
	interface IInWindowStream
	{
        /// <summary>
        /// 设置流对象
        /// </summary>
        /// <param name="inStream">输入输出流</param>
		void SetStream(System.IO.Stream inStream);
        /// <summary>
        /// 对象初始化
        /// </summary>
		void Init();
        /// <summary>
        /// 是否流对象
        /// </summary>
		void ReleaseStream();
        /// <summary>
        /// 获取相应流对象索引值的字节
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回对应的字节</returns>
		Byte GetIndexByte(Int32 index);
        /// <summary>
        /// 获取匹配的长度
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="distance">距离</param>
        /// <param name="limit">限定范围</param>
        /// <returns>返回匹配的长度</returns>
		UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit);
        /// <summary>
        /// 获取可用的字节数量
        /// </summary>
        /// <returns>返回可用的字节数量</returns>
		UInt32 GetNumAvailableBytes();
	}
    /// <summary>
    /// 符合的发现者接口
    /// </summary>
	interface IMatchFinder : IInWindowStream
	{
        /// <summary>
        /// 创建接口
        /// </summary>
        /// <param name="historySize">历史尺寸</param>
        /// <param name="keepAddBufferBefore">保持加载之前添加缓冲区</param>
        /// <param name="matchMaxLen">匹配的长度</param>
        /// <param name="keepAddBufferAfter">创建后添加缓冲区</param>
		void Create(UInt32 historySize, UInt32 keepAddBufferBefore,
				UInt32 matchMaxLen, UInt32 keepAddBufferAfter);
        /// <summary>
        /// 获取匹配对象
        /// </summary>
        /// <param name="distances">距离</param>
        /// <returns>返回匹配的对象</returns>
		UInt32 GetMatches(UInt32[] distances);
        /// <summary>
        /// 跳过
        /// </summary>
        /// <param name="num">数量</param>
		void Skip(UInt32 num);
	}
}
