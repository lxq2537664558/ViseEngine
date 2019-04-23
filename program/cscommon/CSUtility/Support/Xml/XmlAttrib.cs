using System;

namespace CSUtility.Support
{
    public class XmlAttrib
    {
        private IntPtr mAttrib;   // VXmlAttribA*

        public XmlAttrib(IntPtr attr)
        {
            unsafe
            {
                mAttrib = attr;
            }
        }
        ~XmlAttrib()
        {
            unsafe
            {
                mAttrib = IntPtr.Zero;
            }
        }

        public string Name
        {
            get
            {
                unsafe
                {
                    return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.RapidXmlAttribA_name(mAttrib));
                }
            }
        }

        public string Value
        {
            get
            {
                unsafe
                {
                    return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.RapidXmlAttribA_value(mAttrib));
                }
            }
        }
    }
}
