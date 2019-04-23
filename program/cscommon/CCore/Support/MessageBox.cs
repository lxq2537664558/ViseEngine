namespace CCore.Support
{
    /// <summary>
    /// 对话框
    /// </summary>
    public class MessageBox
    {
        /// <summary>
        /// 对话框类型
        /// </summary>
        public enum DialogResult
        {
            Abort,
            Cancel,
            Ignore,
            No,
            None,
            OK,
            Retry,
            Yes,
        }
        /// <summary>
        /// 对话框按钮类型
        /// </summary>
        public enum MessageBoxButtons
        {
            AbortRetryIgnore,
            OK,
            OKCancel,
            RetryCancel,
            YesNo,
            YesNoCancel,
        }
        /// <summary>
        /// 对话框消息显示
        /// </summary>
        /// <param name="msg">显示内容</param>
        /// <returns>返回对话框类型</returns>
        public static DialogResult Show(string msg)
        {
#if WIN
            return (DialogResult)System.Windows.Forms.MessageBox.Show(msg);
#else
            return DialogResult.None;
#endif
        }
        /// <summary>
        /// 对话框消息显示
        /// </summary>
        /// <param name="msg">显示的内容</param>
        /// <param name="caption">窗口</param>
        /// <returns>返回对话框类型</returns>
        public static DialogResult Show(string msg, string caption)
        {
#if WIN
            return (DialogResult)System.Windows.Forms.MessageBox.Show(msg, caption);
#else
            return DialogResult.None;
#endif
        }
        /// <summary>
        /// 对话框消息显示
        /// </summary>
        /// <param name="msg">显示的内容</param>
        /// <param name="caption">窗口</param>
        /// <param name="btns">对话框按钮</param>
        /// <returns>返回对话框类型</returns>
        public static DialogResult Show(string msg, string caption, MessageBoxButtons btns)
        {
#if WIN
            return (DialogResult)System.Windows.Forms.MessageBox.Show(msg, caption, (System.Windows.Forms.MessageBoxButtons)btns);
#else
            return DialogResult.None;
#endif
        }
    }
}
