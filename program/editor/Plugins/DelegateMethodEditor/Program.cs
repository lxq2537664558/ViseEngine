using System;
using System.Collections.Generic;
using System.Linq;

namespace DelegateMethodEditor
{
    public sealed class ShowInEditorMenu : CodeGenerateSystem.ShowInMenu
    {
        public ShowInEditorMenu(string showNames)
            : base(showNames)
        {

        }
    }

    public static class Program
    {
        public static string CodeFilesFolderName = "CodeFiles";
        public static EventListItem SelectedEventListItem = null;

        /*static System.Reflection.Assembly mFrameSetAssm = null;
        public static System.Reflection.Assembly FrameSetAssm
        {
            get
            {
                if (mFrameSetAssm == null)
                {
                    try
                    {
                        mFrameSetAssm = CSUtility.Program.GetAssemblyFromDllFileName("FrameSet.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                return mFrameSetAssm;
            }
        }
        static System.Reflection.Assembly mCSCommonAssm = null;
        public static System.Reflection.Assembly CSCommonAssm
        {
            get
            {
                if (mCSCommonAssm == null)
                {
                    try
                    {
                        mCSCommonAssm = CSUtility.Program.GetAssemblyFromDllFileName("CSUtility.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                return mCSCommonAssm;
            }
        }
        static System.Reflection.Assembly mServerAssm = null;
        public static System.Reflection.Assembly ServerAssm
        {
            get
            {
                if (mServerAssm == null)
                {
                    try
                    {
                        mServerAssm = CSUtility.Program.GetAssemblyFromDllFileName("ServerCommon.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                return mServerAssm;
            }
        }
        static System.Reflection.Assembly mSlimDXAssm = null;
        public static System.Reflection.Assembly SlimDXAssm
        {
            get
            {
                if (mSlimDXAssm == null)
                {
                    try
                    {
                        mSlimDXAssm = CSUtility.Program.GetAssemblyFromDllFileName("SlimDX.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                return mSlimDXAssm;
            }
        }

        public static Type GetType(string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr))
                return null;

            Type retType = null;
            retType = Type.GetType(typeStr);
            if (retType != null)
                return retType;

            //if (retType == null)
            //    retType = AIStateAssem.GetType(typeStr);
            if (retType == null && CSCommonAssm != null)
                retType = CSCommonAssm.GetType(typeStr);
            if (retType == null && FrameSetAssm != null)
                retType = FrameSetAssm.GetType(typeStr);
            if (retType == null && ServerAssm != null)
                retType = ServerAssm.GetType(typeStr);
            if (retType == null && SlimDXAssm != null)
                retType = SlimDXAssm.GetType(typeStr);

            if (retType != null)
                return retType;

            foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                retType = assem.GetType(typeStr);
                if (retType != null)
                    return retType;
            }

            return retType;
        }

        public static Type GetType(string typeStr, CSUtility.Helper.enCSType csType)
        {
            Type retType = null;

            switch (csType)
            {
                case CSUtility.Helper.enCSType.Common:
                    {
                        if(CSCommonAssm != null)
                            retType = CSCommonAssm.GetType("CSUtility.AISystem.States." + typeStr);
                    }
                    break;

                case CSUtility.Helper.enCSType.Client:
                    {
                        if(FrameSetAssm != null)
                            retType = FrameSetAssm.GetType("FrameSet.ClientStates." + typeStr);
                        if (retType == null && CSCommonAssm != null)
                            retType = CSCommonAssm.GetType("CSUtility.AISystem.States." + typeStr);
                    }
                    break;

                case CSUtility.Helper.enCSType.Server:
                    {
                        if(ServerAssm != null)
                            retType = ServerAssm.GetType("FrameSet.ServerStates." + typeStr);
                        if (retType == null && CSCommonAssm != null)
                            retType = CSCommonAssm.GetType("CSUtility.AISystem.States." + typeStr);
                    }
                    break;
            }

            return retType;
        }

        public static Type[] GetTypes(CSUtility.Helper.enCSType csType)
        {
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Common:
                    {
                        if(CSCommonAssm != null)
                            return CSCommonAssm.GetTypes();

                        return (new List<Type>()).ToArray();
                    }

                case CSUtility.Helper.enCSType.Client:
                    {
                        List<Type> retTypes = new List<Type>();
                        if(FrameSetAssm != null)
                            retTypes.AddRange(FrameSetAssm.GetTypes());
                        if(CSCommonAssm != null)
                            retTypes.AddRange(CSCommonAssm.GetTypes());
                        return retTypes.ToArray();
                    }

                case CSUtility.Helper.enCSType.Server:
                    {
                        List<Type> retTypes = new List<Type>();
                        if(ServerAssm != null)
                            retTypes.AddRange(ServerAssm.GetTypes());
                        if(CSCommonAssm != null)
                            retTypes.AddRange(CSCommonAssm.GetTypes());
                        return retTypes.ToArray();
                    }

                case CSUtility.Helper.enCSType.All:
                    {
                        List<Type> retTypes = new List<Type>();
                        if(FrameSetAssm != null)
                            retTypes.AddRange(FrameSetAssm.GetTypes());
                        if(ServerAssm != null)
                            retTypes.AddRange(ServerAssm.GetTypes());
                        if(CSCommonAssm != null)
                            retTypes.AddRange(CSCommonAssm.GetTypes());
                        return retTypes.ToArray();
                    }
            }

            return null;
        }*/

        public static string GetValuedGUIDString(Guid guid)
        {
            string retString = guid.ToString();
            retString = retString.Replace("-", "_");

            return retString;
        }

        static Dictionary<Type, Dictionary<Guid, EventListItem>> mEventDictionary = new Dictionary<Type, Dictionary<Guid, EventListItem>>();
        public static EventListItem[] GetEventList(Type eventType)
        {
            Dictionary<Guid, EventListItem> retValue = null;
            if (mEventDictionary.TryGetValue(eventType, out retValue))
            {
                return retValue.Values.ToArray();
            }

            retValue = new Dictionary<Guid, EventListItem>();

            // 遍历查找使用此对象的函数实现
            var dir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultEventDirectory;
            if(!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            foreach (var subDir in System.IO.Directory.EnumerateDirectories(dir))
            {
                var dirName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(subDir);
                var relSubDir = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(subDir);
                var fileName = relSubDir + "\\" + dirName + ".xml";

                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(fileName);
                if (xmlHolder == null)
                    continue;

                CSUtility.Helper.EventCallBack evb = new CSUtility.Helper.EventCallBack(CSUtility.Helper.enCSType.Client);
                evb.Load(xmlHolder, CSUtility.Helper.enCSType.Client);

                if (evb.CBType != eventType)
                    continue;

                EventListItem item = new EventListItem(evb);
                item.Load(xmlHolder, CSUtility.Helper.enCSType.Client);

                retValue[item.EventId] = item;
            }
            mEventDictionary[eventType] = retValue;

            return retValue.Values.ToArray();
        }

        public static EventListItem GetEventItem(Type eventType, Guid id)
        {
            Dictionary<Guid, EventListItem> retValue = null;
            if (mEventDictionary.TryGetValue(eventType, out retValue))
            {
                EventListItem retItem = null;
                if (retValue.TryGetValue(id, out retItem))
                    return retItem;
            }

            return null;
        }

        public static void AddEvent(EventListItem item)
        {
            if(item.EventCallBack == null)
                return;

            Dictionary<Guid, EventListItem> retValue = null;
            if (mEventDictionary.TryGetValue(item.EventCallBack.CBType, out retValue))
            {
                retValue[item.EventId] = item;
            }
            else
            {
                retValue = new Dictionary<Guid, EventListItem>();
                retValue[item.EventId] = item;
                mEventDictionary[item.EventCallBack.CBType] = retValue;
            }
        }

        public static void DelEvent(EventListItem item)
        {
            if (item.EventCallBack == null)
                return;

            Dictionary<Guid, EventListItem> retValue = null;
            if (mEventDictionary.TryGetValue(item.EventCallBack.CBType, out retValue))
            {
                retValue.Remove(item.EventId);
                if (retValue.Count == 0)
                    mEventDictionary.Remove(item.EventCallBack.CBType);
            }
        }
    }
}
