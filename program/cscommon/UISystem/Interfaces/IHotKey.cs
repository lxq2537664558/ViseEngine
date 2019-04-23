namespace UISystem.Interfaces
{
    public interface IHotKey
    {
        string HotKey  // 定义的快捷键，可以是组合键
        {
            get;
            set;
        }
        void HotKeyDownProcess(CCore.MsgProc.BehaviorParameter be);   // 快捷键按下处理
        void HotKeyUpProcess(CCore.MsgProc.BehaviorParameter be);     // 快捷键弹起处理
    }
}
