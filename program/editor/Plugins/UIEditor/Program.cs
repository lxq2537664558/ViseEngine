using System.Collections.ObjectModel;

namespace UIEditor
{
    public class Program
    {
        public static string ControlDragType
        {
            get { return "UIControl"; }
        }

        public enum enAssemblyType
        {
            This,
            UISystem,
            FrameSet,
            All,
        }

        public static ObservableCollection<UISystem.WinBase> mSelectionWinControlsCollection = new ObservableCollection<UISystem.WinBase>();

        public static DrawPanel mDrawPanel = null;
        public static MainControl mMainControl = null;
        // 录制模式
        public static void SetRecordMode(string recordInfoString, bool bRecord)
        {
            if (mDrawPanel != null)
                mDrawPanel.RecordMode = bRecord;

            mMainControl.TextBlock_RecordInfo.Text = recordInfoString;

            if (bRecord)
                mMainControl.Grid_Record.Visibility = System.Windows.Visibility.Visible;
            else
                mMainControl.Grid_Record.Visibility = System.Windows.Visibility.Hidden;
        }

        //public static System.Reflection.Assembly mUISystemAssembly = CSUtility.Program.GetAssemblyFromDllFileName("UISystem.dll");
        //public static System.Reflection.Assembly mFrameSetAssembly = CSUtility.Program.GetAssemblyFromDllFileName("FrameSet.dll");

        //public static Type GetType(string typeStr, enAssemblyType asType)
        //{
        //    Type retType = null;

        //    switch (asType)
        //    {
        //        case enAssemblyType.This:
        //            {
        //                retType = System.Type.GetType(typeStr);
        //                if (retType == null)
        //                    retType = System.Reflection.Assembly.GetExecutingAssembly().GetType(typeStr);
        //                return retType;
        //            }

        //        case enAssemblyType.UISystem:
        //            {
        //                if (mUISystemAssembly != null)
        //                    return mUISystemAssembly.GetType(typeStr);
        //            }
        //            break;
        //        case enAssemblyType.FrameSet:
        //            {
        //                if (mFrameSetAssembly != null)
        //                    return mFrameSetAssembly.GetType(typeStr);
        //            }
        //            break;

        //        case enAssemblyType.All:
        //            {
        //                retType = System.Type.GetType(typeStr);
        //                if (retType == null)
        //                    retType = System.Reflection.Assembly.GetExecutingAssembly().GetType(typeStr);

        //                if (mUISystemAssembly != null && retType == null)
        //                    retType = mUISystemAssembly.GetType(typeStr);

        //                if (mFrameSetAssembly != null && retType == null)
        //                    retType = mFrameSetAssembly.GetType(typeStr);
        //            }
        //            break;
        //    }

        //    return retType;
        //}

        //public static Type[] GetTypes(enAssemblyType asType)
        //{
        //    List<Type> types = new List<Type>();

        //    switch (asType)
        //    {
        //        case enAssemblyType.This:
        //            return System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

        //        case enAssemblyType.UISystem:
        //            {
        //                if (mUISystemAssembly != null)
        //                    return mUISystemAssembly.GetTypes();
        //            }
        //            break;

        //        case enAssemblyType.FrameSet:
        //            {
        //                if (mFrameSetAssembly != null)
        //                    return mFrameSetAssembly.GetTypes();
        //            }
        //            break;

        //        case enAssemblyType.All:
        //            {
        //                if (mUISystemAssembly != null)
        //                    types.AddRange(mUISystemAssembly.GetTypes());

        //                if (mFrameSetAssembly != null)
        //                    types.AddRange(mFrameSetAssembly.GetTypes());

        //                types.AddRange(System.Reflection.Assembly.GetExecutingAssembly().GetTypes());
        //            }
        //            break;
        //    }

        //    return types.ToArray();
        //}
    }
}
