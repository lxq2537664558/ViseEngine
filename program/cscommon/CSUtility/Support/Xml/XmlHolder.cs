using System;

namespace CSUtility.Support
{
    public class XmlHolder
    {
        private IntPtr mHolder;
        public IntPtr Holder
        {
            get { return mHolder; }
        }
        //public List<string> mSavePtr = new List<string>();

        public XmlHolder()
        {

        }
        ~XmlHolder()
        {
            unsafe
            {
                DllImportAPI.RapidXmlA_Delete(mHolder);
                mHolder = IntPtr.Zero;
            }

            //mSavePtr.Clear();
        }

        public XmlNode RootNode
        {
            get
            {
                unsafe
                {
                    var node = (IntPtr)DllImportAPI.RapidXmlA_RootNode(mHolder);
                    if (node == IntPtr.Zero)
                        return null;

                    var result = new XmlNode(node);
                    result.mHolder = this;
                    return result;
                }
            }
        }

        public static XmlHolder LoadXML(System.String file)
        {
            unsafe
            {
                // 计算绝对路径
                file = IFileManager.Instance._GetAbsPathFromRelativePath(file);
                //IntPtr strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(file);
                //file = file.ToLower();
                var pHolder = DllImportAPI.RapidXml_LoadFileA(file);
                if (pHolder == IntPtr.Zero)
                    return null;

                var holder = new XmlHolder();
                holder.mHolder = pHolder;
                return holder;
            }
        }

        public static void SaveXML(System.String file, XmlHolder node, bool bClearStrPtr)
        {
            // 计算绝对路径
            var fileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(file);

            unsafe
            {
                DllImportAPI.RapidXmlA_SaveXML(node.mHolder, fileName);
            }
        }
        public static XmlHolder NewXMLHolder(System.String name, System.String value)
        {
            unsafe
            {
                var xmlHolder = new XmlHolder();
                xmlHolder.mHolder = DllImportAPI.RapidXmlA_NewXmlHolder();

                var root = DllImportAPI.RapidXmlNodeA_allocate_node(xmlHolder.mHolder, name, "");
                DllImportAPI.RapidXmlA_append_node(xmlHolder.mHolder, root);
                return xmlHolder;
            }
        }

        public static XmlHolder ParseXML(System.String xmlString)
        {
            var pHolder = DllImportAPI.RapidXmlA_ParseXML(xmlString);
            if (pHolder == IntPtr.Zero)
                return null;

            var xmlHolder = new XmlHolder();
            xmlHolder.mHolder = pHolder;
            return xmlHolder;
        }
        public static void GetXMLString(ref string xmlStr, XmlHolder node)
        {
            unsafe
            {
                var str = DllImportAPI.RapidXmlA_GetStringFromXML(node.mHolder);
                xmlStr = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str);
                DllImportAPI.RapidXmlA_FreeString(str);
            }
        }
        public static void GetXMLStringFromNode(ref string xmlStr, XmlNode node)
        {
            unsafe
            {
                var str = DllImportAPI.RapidXmlNodeA_GetStringFromNode(node.Node);
                xmlStr = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str);
                DllImportAPI.RapidXmlNodeA_FreeString(str);
            }
        }
    }
}
