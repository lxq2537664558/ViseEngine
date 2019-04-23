namespace UISystem.Interfaces
{
    // 使控件具有Tab键切换键盘焦点的能力
    public interface ITabAble
    {
        int TabIndex
        {
            get;
            set;
        }
    }
}
