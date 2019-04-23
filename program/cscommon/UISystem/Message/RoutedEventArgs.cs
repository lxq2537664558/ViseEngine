namespace UISystem.Message
{
    public class RoutedEventArgs
    {
        public WinBase Source = null;
        public WinBase Origion = null;
        public bool Handled = false;
    }

    // MouseUp, MouseDown, MouseMove
    public class MouseEventArgs : RoutedEventArgs
    {
        CSUtility.Support.Point mLocation = new CSUtility.Support.Point();
        public CSUtility.Support.Point Location
        {
            get{ return mLocation; }
        }
        int mClicks = 0;
        public int Clicks
        {
            get { return mClicks; }
        }
        public int X
        {
            get { return mLocation.X; }
        }
        public int Y
        {
            get { return mLocation.Y; }
        }
        CCore.MsgProc.Behavior.Mouse_Move.MouseButtons mButton;
        public CCore.MsgProc.Behavior.Mouse_Move.MouseButtons Button
        {
            get { return mButton; }
        }

        public MouseEventArgs()
        {

        }

        public MouseEventArgs(int clicks, int x, int y, CCore.MsgProc.Behavior.Mouse_Move.MouseButtons button)
        {
            mClicks = clicks;
            mLocation = new CSUtility.Support.Point(x, y);
            mButton = button;
        }
    }

    public class KeyEventArgs : RoutedEventArgs
    {
        CCore.MsgProc.BehaviorParameter.enKeys mKey;
        public CCore.MsgProc.BehaviorParameter.enKeys Key
        {
            get { return mKey; }
        }

        public KeyEventArgs(CCore.MsgProc.BehaviorParameter.enKeys key)
        {
            mKey = key;
        }
    }
}
