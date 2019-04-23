using System;

namespace CSUtility.Support
{
    public class XndHolder
    {
        protected XndNode mNode;
        public XndNode Node
        {
            get { return mNode; }
        }

        public static XndHolder LoadXND( System.String file )
		{
			IntPtr io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(file, CSUtility.EFileType.Xnd, false);
			if(io==(IntPtr)0)
				return null;

			XndNode node = new XndNode((IntPtr)0);
			if(false == node.Load(io))
				return null;
            CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);

			XndHolder holder = new XndHolder();
            holder.mNode = node;

			return holder; 
		}

		public static void SaveXND( System.String file , XndHolder node )
		{
			if(node == null || node.mNode == null)
				return;

			IntPtr io = CSUtility.Support.IFileManager.Instance.NewFileWriter(file, CSUtility.EFileType.Xnd);
			if(io == (IntPtr)0)
				return;

			node.mNode.Save(io);
            CSUtility.Support.IFileManager.Instance.ReleaseFileWriter(io);
		}

		public static XndHolder NewXNDHolder(  )
		{
			XndHolder holder = new XndHolder();
			holder.mNode = new XndNode((IntPtr)0);
			return holder;
		}
    }
}
