using System;
using System.Collections.Generic;

namespace CSUtility.Support
{
    public class XndNode
    {
        private IntPtr mHandle;
        public IntPtr Ptr
        {
            get { return mHandle; }
        }
        public IntPtr GetRawNode()
        {
            return mHandle;
        }
        public XndNode(IntPtr node)
        {
            unsafe
            {
                if (node != (IntPtr)0)
                    DllImportAPI.XNDNode_AddRef(node);
                else
                    node = (IntPtr)DllImportAPI.XNDNode_New();

                mHandle = node;
            }
        }
        ~XndNode()
        {
            unsafe
            {
                DllImportAPI.XNDNode_Release(mHandle);
            }
        }

        public void TryReleaseHolder()
        {
            unsafe
            {
                DllImportAPI.XNDNode_TryReleaseHolder(mHandle);
            }
        }

        public void SetName(string name)
        {
            unsafe
            {
                DllImportAPI.XNDNode_SetName(mHandle, name);
            }
        }

        public string GetName()
		{
            unsafe
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.XNDNode_GetName(mHandle));
            }
		}

        public XndNode AddNode( System.String name, Int64 classId, UInt32 userFlags )
		{
            unsafe
            {
                IntPtr namePtr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(name);
                IntPtr childNode = DllImportAPI.XNDNode_AddNode(mHandle,namePtr,classId,userFlags);
                System.Runtime.InteropServices.Marshal.FreeHGlobal(namePtr);
                return new XndNode(childNode);
            }
		}
        public XndNode Addnode(XndNode childNode)
        {
            var node = DllImportAPI.XNDNode_AddNodeWithSource(mHandle, childNode.Ptr);
            return new XndNode(node);
        }
		public bool DelNode( XndNode node )
		{
            unsafe
            {
                int ret = DllImportAPI.XNDNode_DelNode(mHandle, node.mHandle);
                if(ret == 0)
                    return false;
                return true;
            }
		}

		public XndNode FindNode(System.String name)
		{
            unsafe
            {
                //IntPtr namePtr = System.Runtime.InteropServices.Marshal.StringToHGlobalUni(name);
                IntPtr node = DllImportAPI.XNDNode_FindNode(mHandle,name);
                if (node == IntPtr.Zero)
                    return null;
			    return new XndNode(node);
            }
		}

		public XndAttrib AddAttrib(System.String name)
		{
			unsafe
            {
                //IntPtr namePtr = System.Runtime.InteropServices.Marshal.StringToHGlobalUni(name);
			    var attr = DllImportAPI.XNDNode_AddAttrib(mHandle,name);
			    return new XndAttrib(attr);
            }
		}
        public XndAttrib AddAttrib(XndAttrib srcAtt)
        {
            var attr = DllImportAPI.XNDNode_AddAttribWithSource(mHandle, srcAtt.Handle);
            return new XndAttrib(attr);
        }
		public XndAttrib FindAttrib(System.String name)
		{
			unsafe
            {
                //IntPtr namePtr = System.Runtime.InteropServices.Marshal.StringToHGlobalUni(name);
			    var attr = DllImportAPI.XNDNode_FindAttrib( mHandle, name );
                if (attr == IntPtr.Zero)
                    return null;

                return new XndAttrib(attr);
            }
		}

		public bool DelAttrib(System.String name)
		{
			unsafe
            {
                //IntPtr namePtr = System.Runtime.InteropServices.Marshal.StringToHGlobalUni(name);
                int ret = DllImportAPI.XNDNode_DelAttrib(mHandle,name);
                if(ret==0)
                    return false;
                return true;
			}
		}

		public List<XndNode> GetNodes()
		{
            unsafe
            {
                List<XndNode> nodeList = new List<XndNode>();
                int count = DllImportAPI.XNDNode_GetNodeNumber(mHandle);
                for(int i=0;i<count;i++)
                {
                    var childHandle = DllImportAPI.XNDNode_GetNode(mHandle,i);
                    XndNode nd = new XndNode( childHandle );
                    nodeList.Add(nd);
                }
                return nodeList;
            }
		}

		public List<XndAttrib> GetAttribs()
		{
			unsafe
            {
                List<XndAttrib> attribList = new List<XndAttrib>();
                int count = DllImportAPI.XNDNode_GetAttribNumber(mHandle);
                for(int i=0;i<count;i++)
                {
                    var attrHandle = DllImportAPI.XNDNode_GetAttrib(mHandle,i);
                    XndAttrib nd = new XndAttrib( attrHandle );
                    attribList.Add(nd);
                }
                return attribList;
            }
		}

		public bool Save( IntPtr io )
		{
            unsafe
            {
                int ret = DllImportAPI.XNDNode_Save(mHandle, io);
                if (ret == 0)
                    return false;
                return true;
            }
		}
		public bool Load( IntPtr pRes )
		{
            unsafe
            {
                int ret = DllImportAPI.XNDNode_Load(mHandle, pRes);
                if (ret == 0)
                    return false;
                return true;
            }
		}
    }
}
