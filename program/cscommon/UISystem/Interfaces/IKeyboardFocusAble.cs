namespace UISystem.Interfaces
{
    // 可以获得键盘焦点的对象
    public interface IKeyboardFocusAble
    {
        bool KeyboardFocusEnable
        {
            get;
            set;
        }

        bool KeyboardFocused
        {
            get;
            set;
        }

        void TabPress();
    }
}
