using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ResourcesBrowser
{
    public class Program
    {
        public static string SnapshotExt
        {
            get { return "_Snapshot.png"; }
        }
        public static string ResourceInfoExt
        {
            get { return ".rinfo"; }
        }
        public static string FolderDragType
        {
            get { return "BrowserFolder"; }
        }
        public static string ResourcItemDragType
        {
            get { return "ResourceItem"; }
        }

        public enum enCompareResult
        {
            Unknow,
            Larger,
            Equal,
            Smaller,
        }
        public static enCompareResult CompareString(string str1, string str2)
        {
            if (str1 == str2)
                return enCompareResult.Equal;

            str1 = str1.ToLower();
            str2 = str2.ToLower();
            var minLength = System.Math.Min(str1.Length, str2.Length);
            for (int cIdx = 0; cIdx < minLength; cIdx++)
            {
                if (str1[cIdx] > str2[cIdx])
                {
                    return enCompareResult.Larger;
                }
                else if (str1[cIdx] < str2[cIdx])
                    return enCompareResult.Smaller;
            }

            return enCompareResult.Smaller;
        }
        
        public static BitmapSource LoadImage(string strFileName)
        {
            var retImage = EditorCommon.ImageInit.GetImage(strFileName) as BitmapSource;
            return retImage;
        }

        public static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        public static DependencyObject VisualTreeChildSearch<T>(DependencyObject source)
        {
            if (source == null || source.GetType() == typeof(T))
                return source;
            
            for(int i=0; i<VisualTreeHelper.GetChildrenCount(source); i++)
            {
                var child = VisualTreeHelper.GetChild(source, i);
                var result = VisualTreeChildSearch<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
