namespace UISystem.Device
{
    public class Mouse
    {
        public enum MouseButtons
        {
            None = 1<<0,
            Left = 1<<1,
            Right = 1<<2,
            Middle = 1<<3,
        }

        private static Mouse mInstance = new Mouse();
        public static Mouse Instance
        {
            get { return mInstance; }
        }

        bool mEnable = true;
        public bool Enable
        {
            get { return mEnable; }
            set { mEnable = value; }
        }

        public void Cleanup()
        {
            mCapMouseWin = null;
            mFocusWin = null;
        }
        
        private MouseButtons mButtons = MouseButtons.None;

        public bool MouseLeftButtonDown
        {
            get { return (mButtons & MouseButtons.Left) == MouseButtons.Left; }
            set
            {
                if (value)
                {
                    mButtons = mButtons | MouseButtons.Left;
                }
                else
                {
                    mButtons = mButtons | (~MouseButtons.Left);
                }
            }
        }
        public bool MouseRightButtonDown
        {
            get { return (mButtons & MouseButtons.Right) == MouseButtons.Right; }
            set
            {
                if (value)
                {
                    mButtons = mButtons | MouseButtons.Right;
                }
                else
                {
                    mButtons = mButtons | (~MouseButtons.Right);
                }
            }
        }
        public bool MouseMiddleButtonDown
        {
            get { return (mButtons & MouseButtons.Middle) == MouseButtons.Middle; }
            set
            {
                if (value)
                {
                    mButtons = mButtons | MouseButtons.Middle;
                }
                else
                {
                    mButtons = mButtons | (~MouseButtons.Middle);
                }
            }
        }

        CSUtility.Support.Point mPosition;
        public CSUtility.Support.Point Position
        {
            get { return mPosition; }
            set
            {
                mPosition = value;
            }
        }

        public bool IsHitOnWin()
        {
            if(FocusWin == null)
                return false;

            if (!FocusWin.HitThrough &&
                !FocusWin.IsTransparent)
                return true;

            if (FocusWin is WinForm)
            {
                if (((WinForm)FocusWin).ShowAsDialog)
                    return true;
            }

            var parent = FocusWin.Parent as WinBase;
            while (parent != null)
            {
                if (!parent.HitThrough && !parent.IsTransparent)
                    return true;
                if (parent is WinForm)
                {
                    if (((WinForm)parent).ShowAsDialog)
                        return true;
                }

                parent = parent.Parent as WinBase;
            }

            return false;
        }


        #region 控件鼠标捕获

        private WinBase mCapMouseWin;
        private WinBase mCapMouseWin2;
        private WinBase mCapMouseWin3;

        public WinBase GetCapMouseWin(int type)
        {
            switch (type)
            {
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer2Down:
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer2Up:
                    {
                        return mCapMouseWin2;
                    }
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer3Down:
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer3Up:
                    {
                        return mCapMouseWin3;
                    }
                default:
                    {
                        return mCapMouseWin;
                    }
            }
        }
//         public WinBase CapMouseWin
//         {
//             get { return mCapMouseWin; }
//             //set { mCapMouseWin = value; }
//         }

        public WinBase Capture(WinBase win,int type)
        {
            switch(type)
            {
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer2Down:
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer2Up:
                    {
                        var ret = GetCapMouseWin(type);
                        mCapMouseWin2 = win;
                        return ret;
                    }
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer3Down:
                case (int)CCore.MsgProc.BehaviorType.BHT_Pointer3Up:
                    {
                        var ret = GetCapMouseWin(type);
                        mCapMouseWin3 = win;
                        return ret;
                    }
                default:
                    {
                        var ret = GetCapMouseWin(type);
                        mCapMouseWin = win;
                        return ret;
                    }
            }
        }
        public void ReleaseCapture(int index)
        {
#if WIN
            mCapMouseWin = null;
            mCapMouseWin2 = null;
            mCapMouseWin3 = null;
#elif ANDROID || IOS
            if(index <= mCapFingers.Length)
            {
                mCapFingers[index] = null;
            }
#endif
        }

        #endregion

        #region 鼠标焦点控件

        private WinBase mFocusWin = null;
        public WinBase FocusWin
        {
            get { return mFocusWin; }
        }
        public bool Focus(WinBase win)
        {
            //if (!(win is UISystem.Interfaces.IMouseFocusAble))
            //    return false;

            //var curMFW = mFocusWin as UISystem.Interfaces.IMouseFocusAble;
            //if (curMFW != null)
            //    curMFW.MouseFocused = false;
            if (mFocusWin != null)
                mFocusWin.OnMouseUnFocus();
            mFocusWin = win;
            if (mFocusWin != null)
                mFocusWin.OnMouseFocus();

            //curMFW = win as UISystem.Interfaces.IMouseFocusAble;
            //if (curMFW != null && curMFW.MouseFocusEnable)
            //{
            //    mFocusWin = win;
            //    curMFW.MouseFocused = true;
            //}
            //else
            //    mFocusWin = null;

            return true;
        }
        public void UnFocus(int  index)
        {
            //var curMFW = mFocusWin as UISystem.Interfaces.IMouseFocusAble;
            //if (curMFW != null)
            //    curMFW.MouseFocused = false;
#if WIN
            if (mFocusWin != null)
                mFocusWin.OnMouseUnFocus();
            mFocusWin = null;
#elif ANDROID || IOS
            if (CapFingers[index] != null)
                CapFingers[index].OnMouseUnFocus();
            CapFingers[index] = null;
#endif

        }

        #endregion

        #region  Mobile
        private WinBase[] mCapFingers = new WinBase[11];

        public WinBase[]  CapFingers
        {
            get { return mCapFingers; }
        }

        public WinBase GetCapFinger(int index)
        {
            return mCapFingers[index];
        }

        public void  FocusFinger(WinBase win, int index)
        {
            if (index >= mCapFingers.Length)
                return;

            if (mCapFingers[index] != null)
                mCapFingers[index].OnMouseUnFocus();
            mCapFingers[index] = win;
            if (mCapFingers[index] != null)
                mCapFingers[index].OnMouseFocus();
        }


#endregion

    }
}
