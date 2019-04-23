namespace UISystem
{
    public class Program
    {
        public enum enAssemblyType
        {
            This,
            FrameSet,
            ExamplePlugins,
            All,
        }

        internal static readonly uint TreeLevelLimit = 0x7FF;
        // 一次layout最多执行次数，保证每次tick不会执行布局次数太多
        internal static readonly int LayoutUpdateDeep = 153;

        //public static System.Reflection.Assembly mFrameSetAssembly = CSUtility.Program.GetAssemblyFromDllFileName("FrameSet.dll");
        //public static System.Reflection.Assembly mExamplePlugins = CSUtility.Program.GetAssemblyFromDllFileName("ExamplePlugins.dll");

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

        //        case enAssemblyType.FrameSet:
        //            {
        //                if (mFrameSetAssembly != null)
        //                    return mFrameSetAssembly.GetType(typeStr);
        //            }
        //            break;

        //        case enAssemblyType.ExamplePlugins:
        //            {
        //                if (mExamplePlugins != null)
        //                    return mExamplePlugins.GetType(typeStr);
        //            }
        //            break;

        //        case enAssemblyType.All:
        //            {
        //                retType = System.Type.GetType(typeStr);
        //                if (retType == null)
        //                    retType = System.Reflection.Assembly.GetExecutingAssembly().GetType(typeStr);

        //                if (mFrameSetAssembly != null && retType == null)
        //                    retType = mFrameSetAssembly.GetType(typeStr);

        //                if (mExamplePlugins != null && retType == null)
        //                    retType = mExamplePlugins.GetType(typeStr);
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

        //        case enAssemblyType.FrameSet:
        //            {
        //                if (mFrameSetAssembly != null)
        //                    return mFrameSetAssembly.GetTypes();
        //            }
        //            break;

        //        case enAssemblyType.All:
        //            {
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
