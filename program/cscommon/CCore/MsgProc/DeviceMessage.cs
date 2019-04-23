using System;

namespace CCore.MsgProc
{
    //
    // 摘要:
    //     实现 设备消息(Windows、Android)。
    /// <summary>
    /// 设备消息(Windows、Android)。
    /// </summary>
    public struct DeviceMessage
    {
        /// <summary>
        /// 句柄指针
        /// </summary>
        public IntPtr HWnd { get; set; }
        /// <summary>
        /// 消息参数指针
        /// </summary>
        public IntPtr LParam { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public int Msg { get; set; }
        /// <summary>
        /// 消息结果指针
        /// </summary>
        public IntPtr Result { get; set; }
        /// <summary>
        /// 消息参数指针
        /// </summary>
        public IntPtr WParam { get; set; }
        string mHashString;
        /// <summary>
        /// 创建设备消息
        /// </summary>
        /// <param name="hWnd">窗口句柄指针</param>
        /// <param name="msg">消息</param>
        /// <param name="wparam">消息参数指针</param>
        /// <param name="lparam">消息参数指针</param>
        /// <returns>返回发送的设备消息</returns>
        public static DeviceMessage Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            var retValue = new DeviceMessage();
            retValue.HWnd = hWnd;
            retValue.LParam = lparam;
            retValue.Msg = msg;
            retValue.WParam = wparam;
            return retValue;
        }
        /// <summary>
        /// 判断两条消息是否相等
        /// </summary>
        /// <param name="o">设备消息对象</param>
        /// <returns>相同返回true，否则返回false</returns>
        public override bool Equals(object o)
        {
            var msg = (DeviceMessage)o;
            if (msg != null)
            {
                if ((HWnd == msg.HWnd) &&
                    (LParam == msg.LParam) &&
                    (Msg == msg.Msg) &&
                    (Result == msg.Result) &&
                    (WParam == msg.WParam))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 获取哈希表
        /// </summary>
        /// <returns>返回哈希值</returns>
        public override int GetHashCode()
        {
            if(string.IsNullOrEmpty(mHashString))
            {
                mHashString = Guid.NewGuid().ToString();
            }
            return mHashString.GetHashCode();
        }
        /// <summary>
        /// 重载“=”号运算符，执行equal函数
        /// </summary>
        /// <param name="a">设备消息对象</param>
        /// <param name="b">设备消息对象</param>
        /// <returns>两条消息相同返回true，否则返回false</returns>
        public static bool operator ==(DeviceMessage a, DeviceMessage b)
        {
            if (a == null && b == null)
                return true;
            if(a == null)
                return false;
            return a.Equals(b);
        }
        /// <summary>
        /// 重载“！=”操作符
        /// </summary>
        /// <param name="a">设备消息对象</param>
        /// <param name="b">设备消息对象</param>
        /// <returns>两条消息不相同返回true，否则返回false</returns>
        public static bool operator !=(DeviceMessage a, DeviceMessage b)
        {
            if (a == null && b == null)
                return false;
            if(a == null)
                return true;
            return !(a.Equals(b));
        }
    }
}
