using System;
using System.Collections.Generic;
using System.Text;

namespace UISystem
{
    public class UIRenderPipe
    {
        IntPtr mInnerPtr;
        public IntPtr InnerPtr
        {
            get { return mInnerPtr; }
        }
        public UIRenderPipe(IntPtr ptr)
        {
            mInnerPtr = ptr;
        }
        public UIRenderPipe()
        {
            mInnerPtr = CCore.DllImportAPI.UIDrawCallManager_New();
        }
        ~UIRenderPipe()
        {
            if(mInnerPtr!=IntPtr.Zero)
            {
                CCore.DllImportAPI.UIDrawCallManager_Delete(mInnerPtr);
                mInnerPtr = IntPtr.Zero;
            }
        }
    }
}
