using System;
using System.Collections.Generic;

namespace CSUtility.Support
{
    public class XmlNode
    {
        private IntPtr mNode; // VXmlNodeA*
        public IntPtr Node
        {
            get { return mNode; }
        }

        public string Name
        {
            get
            {
                unsafe
                {
                    var retName = DllImportAPI.RapidXmlNodeA_name(mNode);
                    return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(retName);
                }
            }
        }

        public XmlHolder mHolder = null;

        public XmlNode(IntPtr node)
        {
            unsafe
            {
                mNode = node;
            }
        }

        ~XmlNode()
        {
            unsafe
            {
                mNode = IntPtr.Zero;
            }
        }

        public List<XmlNode> FindNodes(string name)
        {
            unsafe
            {
                List<XmlNode> nodeList = new List<XmlNode>();

                if (mNode == IntPtr.Zero)
                    return nodeList;
 
                IntPtr node = DllImportAPI.RapidXmlNodeA_first_node(mNode, name);
                while (node != IntPtr.Zero)
                {
                    var nodeName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.RapidXmlNodeA_name(node));
                    if (name == nodeName)
                    {
                        XmlNode nd = new XmlNode(node);
                        nd.mHolder = mHolder;
                        nodeList.Add(nd);
                    }
                    node = DllImportAPI.RapidXmlNodeA_next_sibling(node);
                }

                return nodeList;
            }
        }

        public XmlNode FindNode(string name)
        {
            unsafe
            {
                if (mNode == IntPtr.Zero)
                    return null;

                var node = DllImportAPI.RapidXmlNodeA_first_node(mNode, name);
                while (node != IntPtr.Zero)
                {
                    var nodeName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.RapidXmlNodeA_name(node));
                    if (name == nodeName)
                    {
                        var nd = new XmlNode(node);
                        nd.mHolder = mHolder;
                        return nd;
                    }
                    node = DllImportAPI.RapidXmlNodeA_next_sibling(node);
                }

                return null;
            }
        }

        public XmlAttrib FindAttrib(string name)
        {
            unsafe
            {
                if (mNode == IntPtr.Zero)
                    return null;

                var attr = DllImportAPI.RapidXmlNodeA_first_attribute(mNode, name);
                if (attr == IntPtr.Zero)
                    return null;

                return new XmlAttrib(attr);
            }
        }

        public List<XmlNode> GetNodes()
        {
            unsafe
            {
                var nodeList = new List<XmlNode>();

                if (mNode == IntPtr.Zero)
                    return nodeList;

                for(var node = DllImportAPI.RapidXmlNodeA_first_node(mNode, null);
                    node != IntPtr.Zero;
                    node = DllImportAPI.RapidXmlNodeA_next_sibling(node))
                {
                    var nd = new XmlNode(node);
                    nd.mHolder = mHolder;
                    nodeList.Add(nd);
                }

                return nodeList;
            }
        }

        public List<XmlAttrib> GetAttribs()
        {
            unsafe
            {
                var attrList = new List<XmlAttrib>();

                if (mNode == IntPtr.Zero)
                    return attrList;

                for(var attr = DllImportAPI.RapidXmlNodeA_first_attribute(mNode, null);
                    attr != IntPtr.Zero;
                    attr = DllImportAPI.RapidXmlAttribA_next_sibling(attr))
                {
                    var nd = new XmlAttrib(attr);
                    attrList.Add(nd);
                }

                return attrList;
            }
        }

        public XmlNode AddNode(string name, string value, XmlHolder holder)
        {
            unsafe
            {
                if(mNode == IntPtr.Zero)
                    return null;

                var node = DllImportAPI.RapidXmlNodeA_allocate_node(holder.Holder, name, value);
                DllImportAPI.RapidXmlNodeA_append_node(mNode, node);
                var result = new XmlNode(node);
                if(holder == null)
                    result.mHolder = mHolder;
                else
                    result.mHolder = holder;
                return result;
            }
        }

        public XmlAttrib AddAttrib(string name, string value)
        {
            unsafe
            {
                //if (name == null)
                //    name = "";
                //if (value == null)
                //    value = "";

                var attr = DllImportAPI.RapidXmlNodeA_allocate_attribute(mHolder.Holder, name, value);
                DllImportAPI.RapidXmlNodeA_append_attribute(mNode, attr);
                return new XmlAttrib(attr);
            }
        }

        public XmlAttrib AddAttrib(string name)
        {
            unsafe
            {
                var attr = DllImportAPI.RapidXmlNodeA_allocate_attribute(mHolder.Holder, name, "");
                DllImportAPI.RapidXmlNodeA_append_attribute(mNode, attr);
                return new XmlAttrib(attr);
            }
        }
    }
}
