
//namespace Navigation
//{
//    public class INavigation
//    {
//        public delegate void Delegate_FOnFindPath(List<SlimDX.Vector2> result);
//        public Delegate_FOnFindPath OnFindPath;

//        protected INavigationDataWrapper mNavData;
//        protected INavigationWrapper mNavigation;

//        static INavigation mInstance = new INavigation();
//        public static INavigation Instance
//        {
//            get { return mInstance; }
//        }

//        public INavigationDataWrapper NavigationData
//        {
//            get { return mNavData; }
//        }

//        private INavigation()
//        {
//            mNavigation = new INavigationWrapper();
//        }
//        ~INavigation()
//        {
//            Cleanup();
//            CleanupPathFinder();
//        }
//        public void Cleanup()
//        {
//            if (mNavData != null)
//            {
//                mNavData.Cleanup();
//                mNavData = null;
//            }
//        }
//        public void CleanupPathFinder()
//        {
//            if (mNavigation != null)
//                mNavigation.Cleanup();
//        }

//        public void Initialize(string name, string path, ref INavigationInfo info)
//        {
//            Cleanup();

//            mNavData = new INavigationDataWrapper();
//            mNavData.ConstrutNavigationData(name, path, info);

//            mNavigation.InitNavigationData(mNavData);
//        }

//        public INavigationWrapper.enNavFindPathResult FindPath(float inStartX, float inStartZ, float inEndX, float inEndZ, List<SlimDX.Vector2> result)
//        {
//            if (mNavigation == null || mNavData == null)
//                return INavigationWrapper.enNavFindPathResult.ENFR_Error;

//            var retValue = mNavigation.FindPath(inStartX, inStartZ, inEndX, inEndZ, result);

//            if (OnFindPath != null)
//                OnFindPath(result);

//            return retValue;
//        }
//    }
//}
