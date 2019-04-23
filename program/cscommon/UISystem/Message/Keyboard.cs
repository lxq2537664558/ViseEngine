using System;
using System.Collections.Generic;

namespace UISystem.Device
{
    public class Keyboard
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetAsyncKeyState")]
        private static extern UInt16 GetAsyncKeyState(UInt32 vKey);

        private static Keyboard mInstance = new Keyboard();
        public static Keyboard Instance
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
            mFocusWin = null;
        }

#region 获取键盘状态

        public enum enKeyState
        {
            Up,
            Down,
            Press,
        }

        public static enKeyState GetKeyState(CCore.MsgProc.BehaviorParameter.enKeys key)
        {
            var state = GetAsyncKeyState((UInt32)key);
            bool isPress = ((state & 0x8000) != 0);
            bool isPressed = ((state & 0x0001) != 0);

            if (!isPress && isPressed)
                return enKeyState.Press;
            else if (isPress)
                return enKeyState.Down;
            else if (!isPress)
                return enKeyState.Up;

            return enKeyState.Up;
        }

#endregion


        private WinBase mFocusWin;
        public WinBase FocusWin
        {
            get { return mFocusWin; }
            //set { mFocusWin = value; }
        }

        public bool Focus(WinBase win)
        {
            if(!(win is UISystem.Interfaces.IKeyboardFocusAble))
                return false;

            var curKFW = mFocusWin as UISystem.Interfaces.IKeyboardFocusAble;
            if (curKFW != null)
            {
                curKFW.KeyboardFocused = false;
            }

            curKFW = win as UISystem.Interfaces.IKeyboardFocusAble;
            if (curKFW != null && curKFW.KeyboardFocusEnable)
            {
                mFocusWin = win;
                curKFW.KeyboardFocused = true;
            }
            else
                mFocusWin = null;

            return true;
        }

        public void UnFocus()
        {
            var curKFW = mFocusWin as UISystem.Interfaces.IKeyboardFocusAble;

            if (curKFW != null)
                curKFW.KeyboardFocused = false;

            mFocusWin = null;
        }

#region 热键

        Dictionary<string, UISystem.Interfaces.IHotKey> mHotKeyDictionarys = new Dictionary<string, UISystem.Interfaces.IHotKey>();
        Dictionary<CCore.MsgProc.BehaviorParameter.enKeys, UISystem.Interfaces.IHotKey> mPressedHotKeyWin = new Dictionary<CCore.MsgProc.BehaviorParameter.enKeys, UISystem.Interfaces.IHotKey>();

        public void RegisterHotKey(string keys, WinBase win)
        {
            if(!(win is UISystem.Interfaces.IHotKey))
                return;

            mHotKeyDictionarys[keys] = win as UISystem.Interfaces.IHotKey;
        }
        public void UnRegisterHotKey(string keys)
        {
            if (string.IsNullOrEmpty(keys))
                return;

            mHotKeyDictionarys.Remove(keys);
        }

        public void ProcessHotKey(CCore.MsgProc.BehaviorParameter bhInit)
        {
            var kbp = bhInit as CCore.MsgProc.Behavior.KB_Char;
            if (kbp == null)
                return;

            // 如果当前有输入焦点，则快捷键不起作用
            if (FocusWin != null)
                return;

            switch ((CCore.MsgProc.BehaviorType)bhInit.GetBehaviorType())
            {
                case CCore.MsgProc.BehaviorType.BHT_KB_Char_Down:
                    {
                        if (!mPressedHotKeyWin.ContainsKey(kbp.Key))
                        {
                            var hotKeyWin = CheckHotKey(kbp.Key);
                            if (hotKeyWin != null)
                            {
                                hotKeyWin.HotKeyDownProcess(bhInit);
                                mPressedHotKeyWin[kbp.Key] = hotKeyWin;
                            }
                        }
                    }
                    break;

                case CCore.MsgProc.BehaviorType.BHT_KB_Char_Up:
                    {
                        UISystem.Interfaces.IHotKey hotKeyWin;
                        if (mPressedHotKeyWin.TryGetValue(kbp.Key, out hotKeyWin))
                        {
                            hotKeyWin.HotKeyUpProcess(bhInit);
                            mPressedHotKeyWin.Remove(kbp.Key);
                        }
                    }
                    break;
            }
        }
        private UISystem.Interfaces.IHotKey CheckHotKey(CCore.MsgProc.BehaviorParameter.enKeys key)
        {
            switch (key)
            {
                case CCore.MsgProc.BehaviorParameter.enKeys.Alt:
                case CCore.MsgProc.BehaviorParameter.enKeys.LeftAlt:
                case CCore.MsgProc.BehaviorParameter.enKeys.RightAlt:
                case CCore.MsgProc.BehaviorParameter.enKeys.Shift:
                case CCore.MsgProc.BehaviorParameter.enKeys.LeftShift:
                case CCore.MsgProc.BehaviorParameter.enKeys.RightShift:
                case CCore.MsgProc.BehaviorParameter.enKeys.Ctrl:
                case CCore.MsgProc.BehaviorParameter.enKeys.LeftCtrl:
                case CCore.MsgProc.BehaviorParameter.enKeys.RightCtrl:
                    break;

                default:
                    {
                        var str = GetHotKeyString(key);
                        UISystem.Interfaces.IHotKey outWin;
                        mHotKeyDictionarys.TryGetValue(str, out outWin);
                        return outWin;
                    }
            }

            return null;
        }

        public bool IsValidHotKey(string hotKey)
        {
            if (string.IsNullOrEmpty(hotKey))
                return false;

            if (hotKey[hotKey.Length - 1] == '+')
                return false;

            return true;
        }
        public string GetHotKeyString(CCore.MsgProc.BehaviorParameter.enKeys key)
        {
            string hotKeyStr = "";

            switch (key)
            {
                case CCore.MsgProc.BehaviorParameter.enKeys.None:
                    {
                        if (GetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Ctrl) == enKeyState.Down)
                            hotKeyStr = "Ctrl+";
                        if (GetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Alt) == enKeyState.Down)
                            hotKeyStr += "Alt+";
                        if (GetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Shift) == enKeyState.Down)
                            hotKeyStr += "Shift+";
                    }
                    break;

                default:
                    {
                        hotKeyStr = key.ToString();
                        if (GetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Shift) == enKeyState.Down)
                            hotKeyStr = "Shift+" + hotKeyStr;
                        if (GetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Alt) == enKeyState.Down)
                            hotKeyStr = "Alt+" + hotKeyStr;
                        if (GetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Ctrl) == enKeyState.Down)
                            hotKeyStr = "Ctrl+" + hotKeyStr;
                    }
                    break;
            }

            return hotKeyStr;
        }

#endregion
    }
}
