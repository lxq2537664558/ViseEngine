using System;

namespace CSUtility.Navigation
{
    public class PathFindContextWrapper_NavigationPoint
    {
        private IntPtr mPathFindContext;
        public IntPtr PathFindContext
        {
            get { return mPathFindContext; }
        }

        public PathFindContextWrapper_NavigationPoint()
        {
            unsafe
            {
                mPathFindContext = DllImportAPI.NavPtPathFindContext_New();
            }
        }
        ~PathFindContextWrapper_NavigationPoint()
        {
            unsafe
            {
                if (mPathFindContext != IntPtr.Zero)
                {
                    DllImportAPI.NavPtPathFindContext_Delete(mPathFindContext);
                    mPathFindContext = IntPtr.Zero;
                }
            }
        }
    }
}
