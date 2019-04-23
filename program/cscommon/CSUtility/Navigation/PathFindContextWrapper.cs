using System;

namespace CSUtility.Navigation
{
    // 记录寻路的中间数据，同一个PathFindContext不能跨线程使用
    public class PathFindContextWrapper
    {
        private IntPtr mPathFindContext;
        public IntPtr PathFindContext
        {
            get { return mPathFindContext; }
        }

        public PathFindContextWrapper()
        {
            unsafe
            {
                mPathFindContext = DllImportAPI.PathFindContext_New();
            }
        }
        ~PathFindContextWrapper()
        {
            unsafe
            {
                DllImportAPI.PathFindContext_Delete(mPathFindContext);
                mPathFindContext = IntPtr.Zero;
            }
        }

        public void Initialize(NavigationInfo info)
        {
            unsafe
            {
                DllImportAPI.PathFindContext_Initialize(mPathFindContext, (IntPtr)(&info));
            }
        }

        public void SetMaxStep(int step)
        {
            unsafe
            {
                DllImportAPI.PathFindContext_SetMaxStep(mPathFindContext, step);
            }
        }
    }
}
