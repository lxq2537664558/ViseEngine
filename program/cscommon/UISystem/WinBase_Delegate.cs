namespace UISystem
{
    public partial class WinBase
    {
        public delegate void FWinSizeChanged(int w, int h, WinBase Sender);
        //public delegate void FWinMouseMove( ref CSUtility.Support.Point pt , WinBase Sender );
        public delegate void FWinDraging(ref CSUtility.Support.Point pt, WinBase Sender);
        public delegate void FWinDraged(ref CSUtility.Support.Point pt, WinBase Sender);
        public delegate void FWinMouseEnter(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e);//WinBase Sender );
        public delegate void FWinMouseLeave(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e);//WinBase Sender );
        //public delegate void FWinVisbleChanged( bool bVisible );
        public delegate void FWinVisibilityChanged(Visibility visibility);
        public delegate void FFormToTop(WinForm pWin);
        public delegate void FWinMouseButtonDown(WinBase sender, Message.MouseEventArgs e);
        public delegate void FWinMouseButtonUp(WinBase sender, Message.MouseEventArgs e);
        public delegate void FWinMouseMove(WinBase sender, Message.MouseEventArgs e);

        public delegate void FWinKeyDown(WinBase sender, Message.KeyEventArgs e);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinKeyDown KeyDown;
        public void _FWinKeyDown(WinBase sender, Message.KeyEventArgs e)
        {
            if (KeyDown != null)
                KeyDown(sender, e);
        }

        public delegate void FWinKeyUp(WinBase sender, Message.KeyEventArgs e);
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinKeyUp KeyUp;
        public void _FWinKeyUp(WinBase sender, Message.KeyEventArgs e)
        {
            if (KeyUp != null)
                KeyUp(sender, e);
        }

        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinSizeChanged WinSizeChanged;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinDraging WinDraging;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinDraged WinDraged;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseEnter WinMouseEnter;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseLeave WinMouseLeave;
        //[CSUtility.Editor.UIEditor_BindingEventAttribute]
        //public event FWinVisbleChanged	WinVisbleChanged;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinVisibilityChanged WinVisibilityChanged;

        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseButtonDown WinMouseButtonDown;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseButtonUp WinMouseButtonUp;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseButtonDown WinLeftMouseButtonDown;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseButtonUp WinLeftMouseButtonUp;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseButtonDown WinRightMouseButtonDown;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseButtonUp WinRightMouseButtonUp;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseButtonDown WinMidMouseButtonDown;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseButtonUp WinMidMouseButtonUp;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event FWinMouseMove WinMouseMove;
        public void _FWinMouseEnter(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)//WinBase Sender )
        {
            WinMouseEnter?.Invoke(ref pt, e);//Sender);
        }
        public void _FWinMouseLeave(ref CSUtility.Support.Point pt, Message.RoutedEventArgs e)//WinBase Sender )
        {
            WinMouseLeave?.Invoke(ref pt, e);//Sender);
        }

        public void _FWinMouseButtonDown(WinBase sender, Message.MouseEventArgs e)
        {
            WinMouseButtonDown?.Invoke(sender, e);
        }
        public void _FWinMouseButtonUp(WinBase sender, Message.MouseEventArgs e)
        {
            WinMouseButtonUp?.Invoke(sender, e);
        }
        public void _FWinLeftMouseButtonDown(WinBase sender, Message.MouseEventArgs e)
        {
            WinLeftMouseButtonDown?.Invoke(sender, e);
        }
        public void _FWinLeftMouseButtonUp(WinBase sender, Message.MouseEventArgs e)
        {
            WinLeftMouseButtonUp?.Invoke(sender, e);
        }
        public void _FWinRightMouseButtonDown(WinBase sender, Message.MouseEventArgs e)
        {
            WinRightMouseButtonDown?.Invoke(sender, e);
        }
        public void _FWinRightMouseButtonUp(WinBase sender, Message.MouseEventArgs e)
        {
            WinRightMouseButtonUp?.Invoke(sender, e);
        }
        public void _FWinMidMouseButtonDown(WinBase sender, Message.MouseEventArgs e)
        {
            WinMidMouseButtonDown?.Invoke(sender, e);
        }
        public void _FWinMidMouseButtonUp(WinBase sender, Message.MouseEventArgs e)
        {
            WinMidMouseButtonUp?.Invoke(sender, e);
        }
        public void _FWinMouseMove(WinBase sender, Message.MouseEventArgs e)
        {
            WinMouseMove?.Invoke(sender, e);
        }
        public void _FWinDraging(ref CSUtility.Support.Point pt, WinBase sender)
        {
            WinDraging?.Invoke(ref pt, sender);
        }
    }
}
