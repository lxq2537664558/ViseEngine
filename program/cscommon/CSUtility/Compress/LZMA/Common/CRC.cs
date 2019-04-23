// Common/CRC.cs

namespace SevenZip
{
    /// <summary>
    /// 循环冗余码校验
    /// </summary>
	class CRC
	{
        /// <summary>
        /// 面板
        /// </summary>
		public static readonly uint[] Table;
        /// <summary>
        /// 构造函数
        /// </summary>
		static CRC()
		{
			Table = new uint[256];
			const uint kPoly = 0xEDB88320;
			for (uint i = 0; i < 256; i++)
			{
				uint r = i;
				for (int j = 0; j < 8; j++)
					if ((r & 1) != 0)
						r = (r >> 1) ^ kPoly;
					else
						r >>= 1;
				Table[i] = r;
			}
		}

		uint _value = 0xFFFFFFFF;
        /// <summary>
        /// 对象初始化
        /// </summary>
		public void Init() { _value = 0xFFFFFFFF; }
        /// <summary>
        /// 按字节更新
        /// </summary>
        /// <param name="b">字节</param>
		public void UpdateByte(byte b)
		{
			_value = Table[(((byte)(_value)) ^ b)] ^ (_value >> 8);
		}
        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移值</param>
        /// <param name="size">数据大小</param>
		public void Update(byte[] data, uint offset, uint size)
		{
			for (uint i = 0; i < size; i++)
				_value = Table[(((byte)(_value)) ^ data[offset + i])] ^ (_value >> 8);
		}
        /// <summary>
        /// 获取整理好的校验码
        /// </summary>
        /// <returns>返回整理好的校验码</returns>
		public uint GetDigest() { return _value ^ 0xFFFFFFFF; }

		static uint CalculateDigest(byte[] data, uint offset, uint size)
		{
			CRC crc = new CRC();
			// crc.Init();
			crc.Update(data, offset, size);
			return crc.GetDigest();
		}

		static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
		{
			return (CalculateDigest(data, offset, size) == digest);
		}
	}
}
