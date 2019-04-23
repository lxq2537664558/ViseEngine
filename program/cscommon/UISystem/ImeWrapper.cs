// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.InteropServices;
using System.Text;
//using System.Windows.Input;
//using System.Windows.Interop;
//using System.Windows.Media;

namespace UISystem
{
    /// <summary>
    /// Native API required for IME support.
    /// </summary>
    public static class ImeWrapper
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct CompositionForm
		{
			public int dwStyle;
			public POINT ptCurrentPos;
			public RECT rcArea;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
		}
		
		[StructLayout(LayoutKind.Sequential)]
        public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
		public struct LOGFONT
		{
			public int lfHeight;
			public int lfWidth;
			public int lfEscapement;
			public int lfOrientation;
			public int lfWeight;
			public byte lfItalic;
			public byte lfUnderline;
			public byte lfStrikeOut;
			public byte lfCharSet;
			public byte lfOutPrecision;
			public byte lfClipPrecision;
			public byte lfQuality;
			public byte lfPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)] public string lfFaceName;
		}

        public const int CPS_CANCEL = 0x4;
        public const int NI_COMPOSITIONSTR = 0x15;
		public const int GCS_COMPSTR = 0x0008;
        public const int GCS_RESULTSTR = 0x0800;
		
		public const int WM_IME_COMPOSITION = 0x10F;
		public const int WM_IME_SETCONTEXT = 0x281;
		public const int WM_INPUTLANGCHANGE = 0x51;
		
		[DllImport("imm32.dll")]
		public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
		[DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
		[DllImport("imm32.dll")]
        public static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue = 0);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref CompositionForm form);
		[DllImport("imm32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImmSetCompositionFont(IntPtr hIMC, ref LOGFONT font);
		[DllImport("imm32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImmGetCompositionFont(IntPtr hIMC, out LOGFONT font);
        [DllImport("imm32.dll")]
        public static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, System.Text.StringBuilder lpBuf, int dwBufLen);

#if WIN || ANDROID

        [DllImport("msctf.dll")]
		static extern int TF_CreateThreadMgr(out ITfThreadMgr threadMgr);
		
		[ThreadStatic] static bool textFrameworkThreadMgrInitialized;
		[ThreadStatic] static ITfThreadMgr textFrameworkThreadMgr;
		
		public static ITfThreadMgr GetTextFrameworkThreadManager()
		{
			if (!textFrameworkThreadMgrInitialized) {
				textFrameworkThreadMgrInitialized = true;
				TF_CreateThreadMgr(out textFrameworkThreadMgr);
			}
			return textFrameworkThreadMgr;
		}
#elif IOS

#endif

        public static bool NotifyIme(IntPtr hIMC)
		{
			return ImmNotifyIME(hIMC, NI_COMPOSITIONSTR, CPS_CANCEL);
		}


        public static bool SetCompositionWindow(IntPtr hIMC, SlimDX.Rect rect)
        {
            CompositionForm form = new CompositionForm();
            form.dwStyle = 0x0020;
            form.ptCurrentPos.x = (int)rect.Left;
            form.ptCurrentPos.y = (int)rect.Top;
            form.rcArea.left = (int)rect.Left;
            form.rcArea.top = (int)rect.Top;
            form.rcArea.right = (int)rect.Right;
            form.rcArea.bottom = (int)rect.Bottom;
            return ImmSetCompositionWindow(hIMC, ref form);
        }



	}

    public class ImeSupport
    {
        WinBase mParentControl = null;

        CCore.Graphics.ViewTarget hwndSource;
        IntPtr currentContext;
        IntPtr previousContext;
        IntPtr defaultImeWnd;

        public ImeSupport(WinBase parent)
        {
            mParentControl = parent;
        }

        void UpdateImeEnabled()
        {
            if (hwndSource == null)
            {
                ClearContext(); // clear existing context (on read-only change)
                CreateContext();
            }
        }

        void ClearContext()
        {
            if (hwndSource != null)
            {
                UISystem.ImeWrapper.ImmAssociateContext(hwndSource.Handle, previousContext);
                UISystem.ImeWrapper.ImmReleaseContext(defaultImeWnd, currentContext);
                currentContext = IntPtr.Zero;
                defaultImeWnd = IntPtr.Zero;
                hwndSource = null;
            }
        }

        void CreateContext()
        {
            hwndSource = UISystem.WinRoot.MainForm;
            if (hwndSource != null)
            {
                defaultImeWnd = UISystem.ImeWrapper.ImmGetDefaultIMEWnd(IntPtr.Zero);
                currentContext = UISystem.ImeWrapper.ImmGetContext(defaultImeWnd);

                previousContext = UISystem.ImeWrapper.ImmAssociateContext(hwndSource.Handle, currentContext);
                // UpdateCompositionWindow() will be called by the caret becoming visible

#if WIN || ANDROID
                var threadMgr = UISystem.ImeWrapper.GetTextFrameworkThreadManager();
                if (threadMgr != null)
                {
                    // Even though the docu says passing null is invalid, this seems to help
                    // activating the IME on the default input context that is shared with WPF
                    threadMgr.SetFocus(IntPtr.Zero);
                }
#elif IOS

#endif
            }
        }
        public void OnGotKeyboardFocus()
        {
            UpdateImeEnabled();
        }
        public void OnLostKeyboardFocus()
        {
            if (currentContext != IntPtr.Zero)
                UISystem.ImeWrapper.NotifyIme(currentContext);
            ClearContext();
        }
        public void UpdateCompositionWindow()
        {
            if (hwndSource == null)
                return;

            if (currentContext != IntPtr.Zero)
            {
                //UISystem.ImeWrapper.SetCompositionFont(hwndSource, currentContext, textArea);
                SlimDX.Rect rect = new SlimDX.Rect(mParentControl.Left, mParentControl.Right, mParentControl.Width, mParentControl.Height);
                UISystem.ImeWrapper.SetCompositionWindow(currentContext, rect);
            }
        }

        //输入完成标记：在输入中时，IME组成字符串不为空，置true；输入完成后，IME组成字符串为空，置false	
        bool mInputFinished = false;
        public string GetIMEString()
        {
            StringBuilder outStr = new StringBuilder();

            if (currentContext != IntPtr.Zero)
            {
                //这里先说明一下，以输入“中国”为例	
                //切换到中文输入法后，输入“zhongguo”，这个字符串称作IME组成字符串	
                //而在输入法列表中选择的字符串“中国”则称作IME结果字符串		        
                int charSize = UISystem.ImeWrapper.ImmGetCompositionString(currentContext, UISystem.ImeWrapper.GCS_COMPSTR, null, 0);	//获取IME组成输入的字符串的长度	
                if (charSize == 0 && mInputFinished)	//如果IME组成字符串为空，并且标记为true，则获取IME结果字符串	
                {
                    charSize = UISystem.ImeWrapper.ImmGetCompositionString(currentContext, UISystem.ImeWrapper.GCS_RESULTSTR, null, 0);//获取IME结果字符串的大小	
                    if (charSize > 0)	//如果IME结果字符串不为空，且没有错误	
                    {
                        //charSize += sizeof(Char);//大小要加上NULL结束符	
                        //charSize *= sizeof(Char);//大小要加上NULL结束符	
                        UISystem.ImeWrapper.ImmGetCompositionString(currentContext, UISystem.ImeWrapper.GCS_RESULTSTR, outStr, charSize);//获取IME结果字符串，这里获取的是宽字节	
                        //outStr[charSize] = ;
                    }
                    mInputFinished = false;
                }
            }

            return outStr.ToString();
        }

        void OnClick(WinBase Sender, ref WinMSG msg)
        {
            
        }

        public void OnMsg(ref UISystem.WinMSG msg, ref string inputStr)
        {
            if (msg.message == (int)UISystem.WinMsg.WM_IME_SETCONTEXT && (int)msg.wParam == 1)
            {
                if (hwndSource!=null)
                    UISystem.ImeWrapper.ImmAssociateContext(hwndSource.Handle, currentContext);
            }

            switch (msg.message)
            {
                case (int)UISystem.WinMsg.WM_INPUTLANGCHANGE:
                    // Don't mark the message as handled; other windows
                    // might want to handle it as well.

                    // If we have a context, recreate it
                    if (hwndSource != null)
                    {
                        ClearContext();
                        CreateContext();
                    }
                    break;
                case (int)UISystem.WinMsg.WM_IME_COMPOSITION:
                    {
                        UpdateCompositionWindow();
                        int charSize = UISystem.ImeWrapper.ImmGetCompositionString(currentContext, UISystem.ImeWrapper.GCS_COMPSTR, null, 0);	//获取IME组成输入的字符串的长度	
                        if (charSize > 0)//如果IME组成字符串不为空，且没有错误（此时charSize为负值），则置输入完成标记为true	
                            mInputFinished = true;
                    }
                    break;
                case (int)UISystem.WinMsg.WM_IME_ENDCOMPOSITION:
                    {
                        if (mInputFinished == true)
                        {
                            //inputStr = GetIMEString();
                        }
                    }
                    break;
                //case (int)UISystem.WinMsg.WM_IME_CHAR:
                //    break;
                case (int)UISystem.WinMsg.WM_CHAR:
                    {
                        inputStr = new string((System.Char)msg.wParam, 1);
                    }
                    break;
                default:
                    break;
            }
            // Debug
            if (inputStr != "" && false)
                System.Diagnostics.Debug.WriteLine(inputStr);
        }
    }

#if WIN || ANDROID
      [ComImport, Guid("aa80e801-2021-11d2-93e0-0060b067b86e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ITfThreadMgr
	{
		void Activate(out int clientId);
		void Deactivate();
		void CreateDocumentMgr(out IntPtr docMgr);
		void EnumDocumentMgrs(out IntPtr enumDocMgrs);
		void GetFocus(out IntPtr docMgr);
		void SetFocus(IntPtr docMgr);
		void AssociateFocus(IntPtr hwnd, IntPtr newDocMgr, out IntPtr prevDocMgr);
		void IsThreadFocus([MarshalAs(UnmanagedType.Bool)] out bool isFocus);
		void GetFunctionProvider(ref Guid classId, out IntPtr funcProvider);
		void EnumFunctionProviders(out IntPtr enumProviders);
		void GetGlobalCompartment(out IntPtr compartmentMgr);
	}
#elif IOS

#endif
}
