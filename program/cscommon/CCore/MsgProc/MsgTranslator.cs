using System;

namespace CCore.MsgProc
{
    /// <summary>
    /// 行为参数
    /// </summary>
    public class BehaviorParameter
    {
        /// <summary>
        /// 键盘消息枚举
        /// </summary>
        public enum enKeys
        {
            None = 0,
            LButton = 1,
            RButton = 2,
            MButton = 4,
            XButton1 = 5,
            XButton2 = 6,
            Back = 8,
            Backspace = 8,
            Tab = 9,
            Return = 13,
            Enter = 13,
            Shift = 16,
            Ctrl = 17,
            Alt = 18,
            Pause = 19,
            Capital = 20,
            CapsLock = 20,
            Esc = 27,
            Space = 32,
            PageUp = 33,
            Next = 34,
            PageDown = 34,
            End = 35,
            Home = 36,
            Left = 37,
            Up = 38,
            Right = 39,
            Down = 40,
            Select = 41,
            Print = 42,
            Execute = 43,
            Snapshot = 44,
            Insert = 45,
            Delete = 46,
            Help = 47,
            D0 = 48,
            D1 = 49,
            D2 = 50,
            D3 = 51,
            D4 = 52,
            D5 = 53,
            D6 = 54,
            D7 = 55,
            D8 = 56,
            D9 = 57,
            A = 65,
            B = 66,
            C = 67,
            D = 68,
            E = 69,
            F = 70,
            G = 71,
            H = 72,
            I = 73,
            J = 74,
            K = 75,
            L = 76,
            M = 77,
            N = 78,
            O = 79,
            P = 80,
            Q = 81,
            R = 82,
            S = 83,
            T = 84,
            U = 85,
            V = 86,
            W = 87,
            X = 88,
            Y = 89,
            Z = 90,
            LWin = 91,
            RWin = 92,
            NumPad0 = 96,
            NumPad1 = 97,
            NumPad2 = 98,
            NumPad3 = 99,
            NumPad4 = 100,
            NumPad5 = 101,
            NumPad6 = 102,
            NumPad7 = 103,
            NumPad8 = 104,
            NumPad9 = 105,
            Multiply = 106, // 小键盘*
            Add = 107,      // 小键盘+
            Separator = 108,    // 小键盘Enter
            Subtract = 109, // 小键盘-
            Decimal = 110,  // 小键盘.
            Divide = 111,   // 小键盘/
            F1 = 112,
            F2 = 113,
            F3 = 114,
            F4 = 115,
            F5 = 116,
            F6 = 117,
            F7 = 118,
            F8 = 119,
            F9 = 120,
            F10 = 121,
            F11 = 122,
            F12 = 123,
            F13 = 124,
            F14 = 125,
            F15 = 126,
            F16 = 127,
            F17 = 128,
            F18 = 129,
            F19 = 130,
            F20 = 131,
            F21 = 132,
            F22 = 133,
            F23 = 134,
            F24 = 135,
            NumLock = 144,
            Scroll = 145,
            LeftShift = 160,
            RightShift = 161,
            LeftCtrl = 162,
            RightCtrl = 163,
            LeftAlt = 164,
            RightAlt = 165,
            Oem1 = 186,    // ;:
            OemSemicolon = 186,
            OemPlus = 187, // =+
            OemComma = 188,    // ,
            OemMinus = 189,    // -_
            OemPeriod = 190,   // .
            Oem2 = 191,    // /?
            OemQuestion = 191,
            Oem3 = 192,    // `~
            OemTilde = 192,
            Oem4 = 219,    // [{
            OemOpenBrackets = 219,
            Oem5 = 220,    // \|
            OemPipe = 220,
            Oem6 = 221,    // ]}
            OemCloseBrackets = 221,
            Oem7 = 222,    // '"
            OemQuotes = 222,
            Count,
        }
        /// <summary>
        /// 行为ID
        /// </summary>
        public int BehaviorId = int.MaxValue;

        //public static enKeys WindowsKeyToKey(System.Windows.Input.Key winKey)
        /// <summary>
        /// 解析按键
        /// </summary>
        /// <param name="winKey">windows按键</param>
        /// <returns>返回转换后的键盘消息</returns>
        public static enKeys WindowsKeyToKey(object winKey)
        {   
            //enKeys key;            
            //if(System.Enum.TryParse<enKeys>(winKey.ToString(), out key))
                //return key;
            //return enKeys.None;

            try
            {
                return (enKeys)System.Enum.Parse(typeof(enKeys), winKey.ToString());
            }
            catch (System.Exception)
            {
                return enKeys.None;
            }
        }
        /// <summary>
        /// 按键
        /// </summary>
        public Object Sender;
        /// <summary>
        /// 获取行为方式
        /// </summary>
        /// <returns>返回值为0</returns>
        public virtual int GetBehaviorType()
        {
            return 0;
        }
        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        public virtual void FromMsg(int msg, UIntPtr wParam, UIntPtr lParam) { }
    }
}

namespace CCore.MsgProc.Behavior
{
    /// <summary>
    /// 键盘消息字符
    /// </summary>
    public class KB_Char : BehaviorParameter
    {
        /// <summary>
        /// 键盘按键
        /// </summary>
        public enKeys Key;
        /// <summary>
        /// 行为类型，判断是键盘消息还是鼠标消息等
        /// </summary>
        public BehaviorType behavior;
        /// <summary>
        /// 获取行为类型
        /// </summary>
        /// <returns>返回该行为的类型</returns>
        public override int GetBehaviorType()
        {
            return (int)behavior;
        }
        /// <summary>
        /// 解析按键消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        public override void FromMsg(int msg, UIntPtr wParam, UIntPtr lParam)
        {
            Key = (enKeys)((wParam.ToUInt32()) & 0xFFFF);
        }
    }
    /// <summary>
    /// 鼠标点击消息
    /// </summary>
    public class Mouse_Key : BehaviorParameter
    {
        /// <summary>
        /// 鼠标点击，单击或者双击
        /// </summary>
        public int Clicks;
        /// <summary>
        /// 点击在屏幕上X坐标
        /// </summary>
        public int X;
        /// <summary>
        /// 点击在屏幕上的Y坐标
        /// </summary>
        public int Y;
        /// <summary>
        /// 行为类型
        /// </summary>
        public BehaviorType behavior;
        /// <summary>
        /// 获取消息行为类型
        /// </summary>
        /// <returns>返回消息行为的类型</returns>
        public override int GetBehaviorType()
        {
            return (int)behavior;
        }
        /// <summary>
        /// 鼠标点击消息解析
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        public override void FromMsg(int msg, UIntPtr wParam, UIntPtr lParam)
        {
            Y = (int)(short)((((lParam.ToUInt32())) >> 16) & 0xFFFF);
            X = (int)(short)(((lParam.ToUInt32())) & 0xFFFF);
        }
    }
    /// <summary>
    /// 鼠标移动处理
    /// </summary>
    public class Mouse_Move : BehaviorParameter
    {
        /// <summary>
        /// 鼠标按键枚举
        /// </summary>
        public enum MouseButtons
        {
            None = 0,
            Left = 1,
            Right = 2,
            Middle = 4,
        }
        /// <summary>
        /// 点击次数
        /// </summary>
        public int Clicks;
        /// <summary>
        /// 鼠标移动的时候的X坐标
        /// </summary>
        public int X;
        /// <summary>
        /// 鼠标移动的时候的Y坐标
        /// </summary>
        public int Y;
        /// <summary>
        /// 鼠标按键
        /// </summary>
        public MouseButtons button;
        /// <summary>
        /// 鼠标移动时是否有点击
        /// </summary>
        /// <param name="btn">按下的鼠标键</param>
        /// <returns>移动时有单击返回true，否则返回false</returns>
        public bool IsKeyDown(MouseButtons btn)
        {
            if ((button & btn) == btn)
                return true;

            return false;
        }
        /// <summary>
        /// 获取行为类型
        /// </summary>
        /// <returns>返回消息行为的类型</returns>
        public override int GetBehaviorType()
        {
            return (int)BehaviorType.BHT_Mouse_Move;
        }
        /// <summary>
        /// 解析鼠标移动的消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        public override void FromMsg(int msg, UIntPtr wParam, UIntPtr lParam)
        {
            Y = (int)(short)(((lParam.ToUInt32()) >> 16) & 0xFFFF);
            X = (int)(short)((lParam.ToUInt32()) & 0xFFFF);
            if ((wParam.ToUInt32() & 0x0001) == 0x0001)
                button = MouseButtons.Left;
            else if ((wParam.ToUInt32() & 0x0002) == 0x0002)
                button = MouseButtons.Right;
            else if (wParam.ToUInt32() == 0x0010)
                button = MouseButtons.Middle;
            else
                button = MouseButtons.None;
        }
    }
    /// <summary>
    /// 鼠标滚轮
    /// </summary>
    public class Mouse_Wheel : BehaviorParameter
    {
        /// <summary>
        /// 鼠标按键枚举
        /// </summary>
        public enum MouseButtons
        {
            None = 0,
            Left = 1,
            Right = 2,
            Middle = 4,
        }
        /// <summary>
        /// 滚轮滚动的距离
        /// </summary>
        public int delta;
        //public int button;
        /// <summary>
        /// 鼠标按键
        /// </summary>
        public MouseButtons button;
        /// <summary>
        /// 获取行为类型
        /// </summary>
        /// <returns>返回消息行为的类型</returns>
        public override int GetBehaviorType()
        {
            return (int)BehaviorType.BHT_Mouse_Wheel;
        }
        /// <summary>
        /// 解析鼠标滚轮消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        public override void FromMsg(int msg, UIntPtr wParam, UIntPtr lParam)
        {
            delta = (int)(short)((((wParam.ToUInt32())) >> 16) & 0xFFFF);
            //button = (int)(short)(((wParam.ToUInt32())) & 0xFFFF);
            if ((wParam.ToUInt32() & 0x0001) == 0x0001)
                button = MouseButtons.Left;
            else if ((wParam.ToUInt32() & 0x0002) == 0x0002)
                button = MouseButtons.Right;
            else if ((wParam.ToUInt32() & 0x00010) == 0x00010)
                button = MouseButtons.Middle;
            else
                button = MouseButtons.None;
        }
    }
    /// <summary>
    /// 画面尺寸改变
    /// </summary>
    public class Win_SizeChanged : BehaviorParameter
    {
        /// <summary>
        /// 画面的宽
        /// </summary>
        public int Width;
        /// <summary>
        /// 画面的高
        /// </summary>
        public int Height;
        /// <summary>
        /// 获取行为类型
        /// </summary>
        /// <returns>返回消息的行为类型</returns>
        public override int GetBehaviorType()
        {
            return (int)BehaviorType.BHT_WinSizeChanged;
        }
    }
    /// <summary>
    /// UV设置
    /// </summary>
    public class UVAnimSetted : BehaviorParameter
    {
        /// <summary>
        /// 获取消息行为类型
        /// </summary>
        /// <returns>返回消息的行为类型</returns>
        public override int GetBehaviorType()
        {
            return (int)BehaviorType.BHT_UVAnimSetted;
        }
    }
    /// <summary>
    /// 文本输入
    /// </summary>
    public class TextInput : BehaviorParameter
    {
        /// <summary>
        /// 设备消息对象
        /// </summary>
        public CCore.MsgProc.DeviceMessage Msg = new CCore.MsgProc.DeviceMessage();
        /// <summary>
        /// 获取行为类型
        /// </summary>
        /// <returns>返回行为类型为文本输入</returns>
        public override int GetBehaviorType()
        {
            return (int)BehaviorType.BHT_TextInput;
        }
    }
}

namespace CCore.MsgProc
{
    /// <summary>
    /// 系统消息的值
    /// </summary>
    public enum SysMessage : uint
	{
        VWM_MOUSEFIRST = 0x0200,        // WM_MOUSEFIRST
        VWM_MOUSELAST = 0x020E,         // WM_MOUSELAST(不同的Windows版本这个值不同...)
        VWM_MOUSEWHEEL = 0x020A,        // WM_MOUSEWHEEL    
        VWM_LBUTTONDOWN = 0x0201,       // WM_LBUTTONDOWN
        VWM_LBUTTONUP = 0x0202,         // WM_LBUTTONUP
        VWM_RBUTTONUP = 0x0205,         // WM_RBUTTONUP
        VWM_MOUSEMOVE = 0x0200,         // WM_MOUSEMOVE
        VWM_SIZE = 0x0005,              // WM_SIZE
        VWM_KEYUP = 0x0101,             // WM_KEYUP
        VWM_IME_COMPOSITION = 0x010F,   // WM_IME_COMPOSITION
        VWM_IME_CHAR = 0x0286,          // WM_IME_CHAR
        VWM_CHAR = 0x0102,              // WM_CHAR

        VWM_USER = 0x0400 + 1000,       // WM_USER
	};
    /// <summary>
    /// 消息行为类型
    /// </summary>
	public enum BehaviorType : int
	{
		BHT_System_Begin	=0,

        BHT_Keyboard_Begin,
		BHT_KB_Char_Down	,
		BHT_KB_Char_Up		,
        BHT_Keyboard_End,

        BHT_Mouse_Begin     ,
		BHT_Mouse_Move		,
		BHT_Mouse_Wheel		,
		BHT_Mouse_Key		,
		BHT_LB_Down			,
		BHT_LB_DoubleClick  ,
		BHT_LB_Up			,
		BHT_RB_Down			,
		BHT_RB_DoubleClick	,
		BHT_RB_Up			,
		BHT_MB_Down			,
		BHT_MB_Up			,
        BHT_Pointer2Down,
        BHT_Pointer2Up,
        BHT_Pointer3Down,
        BHT_Pointer3Up,
        BHT_Mouse_End       ,

        BHT_WinSizeChanged  ,

		BHT_System_End		,
		
		BHT_User_Begin		,
        BHT_UVAnimSetted    ,
        BHT_TextInput       ,
	};
    /// <summary>
    /// 消息分发
    /// </summary>
    public class IMsgTranslator
    {
        /// <summary>
        /// 分发的消息
        /// </summary>
        public class TransResult
        {
            /// <summary>
            /// 行为类型
            /// </summary>
            public int BeType;
            /// <summary>
            /// 行为参数
            /// </summary>
            public BehaviorParameter BeInit;
        }
        /// <summary>
        /// 开始分发消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        /// <returns>返回分发下去的消息</returns>
        public TransResult Translate(int msg, UIntPtr wParam, UIntPtr lParam)
        {
            TransResult ret = new TransResult();
            switch (msg)
            {
		        //case WM_KEYDOWN:
		        //	{
		        //		KB_Char^ _init = gcnew KB_Char();
		        //		_init->FromMsg( msg , wParam , lParam );
		        //		_init->behavior = BehaviorType::BHT_KB_Char_Down;
		        //		ret->BeType = _init->GetBehaviorType();
		        //		ret->BeInit = _init;
		        //	}
		        //	return ret;
		        //case WM_KEYUP:
		        //	{
		        //		KB_Char^ _init = gcnew KB_Char();
		        //		_init->FromMsg( msg , wParam , lParam );
		        //		_init->behavior = BehaviorType::BHT_KB_Char_Up;
		        //		ret->BeType = _init->GetBehaviorType();
		        //		ret->BeInit = _init;
		        //	}
		        //	return ret;
		        //case WM_LBUTTONDOWN:
		        //	{
		        //		LB_Down^ _init = gcnew LB_Down();
		        //		_init->FromMsg( msg , wParam , lParam );
		        //		ret->BeType = _init->GetBehaviorType();
		        //		ret->BeInit = _init;
		        //	}
		        //	return ret;
		
		        // 鼠标消息通过窗口直接Dispatch，不经过系统消息
		        /*case WM_LBUTTONDOWN:
			        {
				        Mouse_Key^ _init = gcnew Mouse_Key();
				        _init->FromMsg(msg, wParam, lParam);
				        _init->behavior = BehaviorType::BHT_LB_Down;
				        ret->BeType =_init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;
		        case WM_LBUTTONDBLCLK:
			        {
				        Mouse_Key^ _init = gcnew Mouse_Key();
				        _init->FromMsg(msg, wParam, lParam);
				        _init->behavior = BehaviorType::BHT_LB_DoubleClick;
				        ret->BeType = _init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;
		        case WM_LBUTTONUP:
			        {
				        Mouse_Key^ _init = gcnew Mouse_Key();
				        _init->FromMsg(msg, wParam, lParam);
				        _init->behavior = BehaviorType::BHT_LB_Up;
				        ret->BeType = _init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;
		        case WM_RBUTTONDOWN:
			        {
				        Mouse_Key^ _init = gcnew Mouse_Key();
				        _init->FromMsg(msg, wParam, lParam);
				        _init->behavior = BehaviorType::BHT_RB_Down;
				        ret->BeType = _init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;
		        case WM_RBUTTONDBLCLK:
			        {
				        Mouse_Key^ _init = gcnew Mouse_Key();
				        _init->FromMsg(msg, wParam, lParam);
				        _init->behavior = BehaviorType::BHT_RB_DoubleClick;
				        ret->BeType = _init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;
		        case WM_RBUTTONUP:
			        {
				        Mouse_Key^ _init = gcnew Mouse_Key();
				        _init->FromMsg(msg, wParam, lParam);
				        _init->behavior = BehaviorType::BHT_RB_Up;
				        ret->BeType = _init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;
		        case WM_MBUTTONDOWN:
			        {
				        Mouse_Key^ _init = gcnew Mouse_Key();
				        _init->FromMsg(msg, wParam, lParam);
				        _init->behavior = BehaviorType::BHT_MB_Down;
				        ret->BeType = _init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;
		        case WM_MBUTTONUP:
			        {
				        Mouse_Key^ _init = gcnew Mouse_Key();
				        _init->FromMsg(msg, wParam, lParam);
				        _init->behavior = BehaviorType::BHT_MB_Up;
				        ret->BeType = _init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;
		        case WM_MOUSEMOVE:
			        {
				        Mouse_Move^ _init = gcnew Mouse_Move();
				        _init->FromMsg( msg , wParam , lParam );
				        //System::Diagnostics::Debug::WriteLine(System::String::Format("-------------WM_MOUSEMOVE Key {0:x4}", _init->button));
				        ret->BeType = _init->GetBehaviorType();
				        ret->BeInit = _init;
			        }
			        return ret;*/
		        case (int)(SysMessage.VWM_MOUSEWHEEL):
			        {
				        var _init = new CCore.MsgProc.Behavior.Mouse_Wheel();
				        _init.FromMsg( msg , wParam , lParam );
				        ret.BeType = _init.GetBehaviorType();
				        ret.BeInit = _init;
			        }
			        return ret;
		    }

            return null;
        }
    }
}
