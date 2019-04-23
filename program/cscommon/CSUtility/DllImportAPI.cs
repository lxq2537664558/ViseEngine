using System;
using System.Runtime.InteropServices;

namespace CSUtility
{ 
    public class DllImportAPI
    {
        public delegate void Delegate_OnNativeMemAlloc(UInt32 size, IntPtr file, UInt32 line, UInt32 id);
        public delegate void Delegate_OnNativeMemFree(UInt32 size, IntPtr file, UInt32 line, UInt32 id);

#if WIN
        const string ModuleNC = "core.Windows.dll";
#elif IOS
        const string ModuleNC = "__Internal";
#else
        const string ModuleNC = "libcore.so";
#endif
        [DllImport("Winmm.dll", CallingConvention = CallingConvention.Winapi)]
        public extern static UInt32 timeBeginPeriod(UInt32 uPeriod);
        [DllImport("Winmm.dll", CallingConvention = CallingConvention.Winapi)]
        public extern static UInt32 timeEndPeriod(UInt32 uPeriod);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static Int64 HighPrecision_GetTickCount();//返回千分之毫秒数
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static Int64 vfxGetTickCount();//返回毫秒数

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void VCriticalInfoManager_SetEnable(Int32 enable);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr VCriticalInfoManager_PrintLockInfo();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int Debug_SetWriteLogStringCallback(Log.FileLog.Delegate_LogWriteCallback callback);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int Debug_UnSetWriteLogStringCallback();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vfxCompressRLE(IntPtr inData, UInt32 in_length_by_count, IntPtr outData);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vfxUnCompressRLE(IntPtr inData, UInt32 in_length_by_count, IntPtr outData);

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static System.IntPtr vfxMemory_MemoryUsed();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static System.IntPtr vfxMemory_MemoryMax();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static System.IntPtr vfxMemory_MemoryAllocTimes();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vfxMemory_SetMemAllocCallBack(Delegate_OnNativeMemAlloc cb);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vfxMemory_SetMemFreeCallBack(Delegate_OnNativeMemFree cb);


        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDNode_New();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDNode_AddRef(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDNode_Release(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDNode_TryReleaseHolder(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr XNDNode_SetName(IntPtr node, string name);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDNode_GetName(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDNode_AddNode(IntPtr node, IntPtr name, Int64 classId, UInt32 flags);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDNode_AddNodeWithSource(IntPtr node, IntPtr srcNode);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int XNDNode_DelNode(IntPtr node, IntPtr childNode);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr XNDNode_FindNode(IntPtr node, string name);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr XNDNode_AddAttrib(IntPtr node, string name);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr XNDNode_AddAttribWithSource(IntPtr node, IntPtr childAtt);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr XNDNode_FindAttrib(IntPtr node, string name);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int XNDNode_DelAttrib(IntPtr node, string name);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int XNDNode_GetNodeNumber(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDNode_GetNode(IntPtr node, int iNode);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int XNDNode_GetAttribNumber(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDNode_GetAttrib(IntPtr node, int iAttrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int XNDNode_Save(IntPtr node, IntPtr file);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int XNDNode_Load(IntPtr node, IntPtr pRes);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte XNDAttrib_GetVersion(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDAttrib_SetVersion(IntPtr attrib, Byte ver);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt32 XNDAttrib_GetLength(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDAttrib_GetKey(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void XNDAttrib_SetKey(IntPtr attrib, string key);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDAttrib_AddRef(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDAttrib_Release(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDAttrib_GetName(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDAttrib_BeginRead(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDAttrib_EndRead(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDAttrib_BeginWrite(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDAttrib_EndWrite(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int XNDAttrib_Read(IntPtr attrib, IntPtr data, int size);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int XNDAttrib_Write(IntPtr attrib, IntPtr data, int size);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr XNDAttrib_ReadStringW(IntPtr attrib);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void XNDAttrib_FreeStringW(IntPtr str);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void XNDAttrib_WriteStringW(IntPtr attrib, string data);

        //RapidXML
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXml_LoadFileA(string filename);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXml_LoadFileW(string filename);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void RapidXmlA_Delete(IntPtr p);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void RapidXmlW_Delete(IntPtr p);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr RapidXmlA_RootNode(IntPtr p);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr RapidXmlA_NewXmlHolder();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr RapidXmlA_append_node(IntPtr xmlHolder, IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlA_ParseXML(string xmlString);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlA_GetStringFromXML(IntPtr xmlHolder);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void RapidXmlA_FreeString(IntPtr str);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void RapidXmlA_SaveXML(IntPtr xmlHolder, string fileName);

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlNodeA_allocate_node(IntPtr xmlHolder, string name, string value);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlNodeA_allocate_attribute(IntPtr xmlHolder, string name, string value);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void RapidXmlNodeA_append_node(IntPtr node, IntPtr childNode);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void RapidXmlNodeA_append_attribute(IntPtr node, IntPtr childAttr);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlNodeA_first_node(IntPtr node, string name);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlNodeA_first_attribute(IntPtr node, string name);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlNodeA_next_sibling(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlNodeA_name(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlNodeA_value(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlNodeA_GetStringFromNode(IntPtr node);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void RapidXmlNodeA_FreeString(IntPtr str);

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlAttribA_name(IntPtr attr);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlAttribA_value(IntPtr attr);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr RapidXmlAttribA_next_sibling(IntPtr attr);

        // XmlWrapper

        // VFile2Memory
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void VFile2Memory_AddRef(IntPtr res);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void VFile2Memory_Release(IntPtr res);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr VFile2Memory_F2M(string psz, bool bShareWrite);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr VFile2Memory_DownloadAndF2M(string psz);

        // VFile
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr VFile_New();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void VFile_Delete(IntPtr vFile);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int VFile_Open(IntPtr vFile, string fileName, UInt32 openFlags);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void VFile_Close(IntPtr vFile);

        #region PerfCounter
        // Samp
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dSamp_SetEnable(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dSamp_GetEnable(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dSamp_SetAvgCounter(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dSamp_GetAvgCounter(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int64 v3dSamp_GetAvgTime(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dSamp_GetAvgHit(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static Int64 v3dSamp_GetMaxTimeInCounter(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static string v3dSamp_GetName(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static string v3dSamp_GetParentName(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void** v3dSamp_GetParentsInfo(IntPtr str, int* strCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void** v3dSamp_GetParentsName(IntPtr str, int* strCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static float v3dSamp_GetParentRate(IntPtr str, string name);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dSamp_DeleteStrings(void** str, int strCount);
        
        // SampMgr
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dSampMgr_Update();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dSampMgr_FindSamp(string name);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int64 v3dSampMgr_Begin(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dSampMgr_End(Int64 beginTime, IntPtr sampResult);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dSampMgr_GetSampSize();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void** v3dSampMgr_GetSamps(int* sampCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dSampMgr_DeleteSampsPtr(void** p);
        #endregion

        #region Navigation

        // NavigationData
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr NavigationData_New();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationData_AddRef(IntPtr data);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationData_Release(IntPtr data);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static bool NavigationData_ConstructNavigation(IntPtr data, string name, string path, IntPtr info);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static bool NavigationData_LoadNavigation(IntPtr data, string name, string path);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static bool NavigationData_LoadNavigation_All(IntPtr data, string name, string path);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static bool NavigationData_SaveDirtyLevel(IntPtr data, string name, string path, bool bForceSave);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr NavigationData_NewLevel(IntPtr data, UInt16 iColX, UInt16 iRowZ);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool NavigationData_TravelTo(IntPtr data, float x, float z, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationData_ForceLoadAllLevels(IntPtr data, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static bool NavigationData_CheckNavDataF(IntPtr data, System.Guid* mapInstanceId, float x, float z, UInt64 ckType);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationData_SetDynamicNavDataF(IntPtr data, System.Guid* mapInstanceId, float x, float z, float radius, Int32 block);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool NavigationData_GetNavigationInfo(IntPtr data, IntPtr info);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationData_KickOffCache(IntPtr data, Int64 time, UInt32 lifeTime);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationData_AddLevel(IntPtr data, UInt16 iColX, UInt16 iRowZ, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationData_DelLevel(IntPtr data, UInt16 iColX, UInt16 iRowZ, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool NavigationData_GenerateNavData(IntPtr navData, UInt16 iColX, UInt16 iRowZ, IntPtr byteData, byte delta, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool NavigationData_GetNavData(IntPtr navData, UInt16 iColX, UInt16 iRowZ, IntPtr byteData, byte delta, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool NavigationData_ClearNavDataWithXZ(IntPtr data, int dataType, UInt16 iColX, UInt16 iRowZ, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool NavigationData_ClearNavData(IntPtr data, int dataType, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationData_SetNeighborSide(IntPtr data, uint value, Int64 time);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationData_SetDynamicBlockEvent(IntPtr data,
                                                            CSUtility.Navigation.INavigationDataWrapper.Delegate_OnDynamicBlockIsBlock _gtEvent,
                                                            CSUtility.Navigation.INavigationDataWrapper.Delegate_OnGetActorGUID_C _gaEvent,
                                                            CSUtility.Navigation.INavigationDataWrapper.Delegate_OnGetActorUIntID _gaudEvent);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationData_GetAllDynamicBlockIDs(IntPtr data, IntPtr outIds, IntPtr outCount, Int64 frameMillisecondTime);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationData_DeleteIDArray(IntPtr outIds);

        // NavTilePoint
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr NavigationTilePoint_GetParentPoint(IntPtr navPt);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static float NavigationTilePoint_GetPointSceneX(IntPtr navPt);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static float NavigationTilePoint_GetPointSceneZ(IntPtr navPt);

        // Navigation
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr Navigation_New();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void Navigation_Delete(IntPtr data);
        //[System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void Navigation_InitNavigationData(IntPtr data);//, IntPtr navData);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int Navigation_FindPath_NavTile(IntPtr data, System.Guid* mapInstanceId, IntPtr navData, IntPtr findPathContext, float inStartX, float inStartZ, float inEndX, float inEndZ, int range, int reFind, int checkDirLine, out IntPtr navPt);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int Navigation_FindPath_NavPt(IntPtr data, IntPtr navData, float startX, float startY, float startZ, float endX, float endY, float endZ, IntPtr context, out IntPtr result);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int Navigation_HasBarrier(IntPtr data, System.Guid* mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ, IntPtr navData);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void Navigation_SetMaxStep(IntPtr data, int maxStep);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int Navigation_GetMaxStep(IntPtr data);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int Navigation_GetFarthestPathPointFromStartInLine(IntPtr data, System.Guid* mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ, IntPtr outX, IntPtr outZ, IntPtr navData);

        // PathFindContext
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr PathFindContext_New();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PathFindContext_Delete(IntPtr context);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PathFindContext_Initialize(IntPtr context, IntPtr info);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PathFindContext_SetMaxStep(IntPtr context, int maxStep);

        //////////////////////////////////////////////////////////////////////////
        // 路点寻路
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr NavigationPointData_New();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationPointData_AddRef(IntPtr data);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationPointData_Release(IntPtr data);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationPointData_Initialize(IntPtr data, float cellWidth, float cellHeight, float mapWidth, float mapHeight);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void NavigationPointData_LoadData(IntPtr data, string fileName, string path);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void NavigationPointData_SaveData(IntPtr data, string fileName, string path, bool forceSave);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationPointData_AddNavigationPoint(IntPtr data, IntPtr id, float x, float y, float z);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationPointData_RemoveNavigationPoint(IntPtr data, IntPtr id);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationPointData_AddNavigationLink(IntPtr data, IntPtr startId, IntPtr endId, bool twoWay);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationPointData_RemoveNavigationLink(IntPtr data, IntPtr startId, IntPtr endId, bool twoWay);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationPointData_MoveNavigationPoint(IntPtr data, IntPtr id, float x, float y, float z);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationPointData_GetNavigationPointCount(IntPtr data);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationPointData_GetNavigationPointData(IntPtr data, int idx, IntPtr outId, IntPtr outX, IntPtr outY, IntPtr outZ, IntPtr outLinkIds, IntPtr outLinkCount);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationPointData_DeleteIdArray(IntPtr outLinkIds);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int NavigationPointData_HasNavigationPoints(IntPtr data);

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr NavPtPathFindContext_New();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavPtPathFindContext_Delete(IntPtr context);

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr NavigationPoint_GetParentPoint(IntPtr navPt);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void NavigationPoint_GetPosition(IntPtr navPt, IntPtr x, IntPtr y, IntPtr z);


        #endregion

        // PerfCounter
        //[System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public extern static void PerfCounter_QueryPerformanceFrequency(IntPtr freq);
        //[System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public extern static void PerfCounter_QueryPerformanceCounter(IntPtr tick);

        #region v3dxCurve2

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr v3dxCurve2_New(float fBgn, float fEnd, float fAspect, float fMin, float fMax);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_Delete(IntPtr cur2);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_InsertNode(IntPtr cur2, int idx, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_DeleteNode(IntPtr cur2, int idx);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_SetPosition(IntPtr cur2, int idx, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static float v3dxCurve2_GetValue(IntPtr cur2, float fTime);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_SetValBegin(IntPtr cur2, float v);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_SetValEnd(IntPtr cur2, float v);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int v3dxCurve2_GetNodeCount(IntPtr cur2);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_GetNodePos(IntPtr cur2, int index, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_SetNodeBeginVel(IntPtr cur2, int index, IntPtr pos, bool bWithEnd);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_GetNodeBeginVel(IntPtr cur2, int index, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_SetNodeEndVel(IntPtr cur2, int index, IntPtr pos, bool bWithBegin);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_GetNodeEndVel(IntPtr cur2, int index, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxCurve2_BuildVelocity(IntPtr cur2, int index);

        #endregion

        #region v3dxBezier

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr v3dxBezier_New();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_Delete(IntPtr ptr);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_InsertNode(IntPtr ptr, int idx, IntPtr pos, IntPtr ctrlPos1, IntPtr ctrlPos2);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_DeleteNode(IntPtr ptr, int idx);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_GetValue(IntPtr ptr, float fTime, IntPtr valueOut);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_GetPosition(IntPtr ptr, int idx, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_SetPosition(IntPtr ptr, int idx, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_GetControlPos1(IntPtr ptr, int idx, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_SetControlPos1(IntPtr ptr, int idx, IntPtr pos, bool bWithPos2);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_GetControlPos2(IntPtr ptr, int idx, IntPtr pos);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_SetControlPos2(IntPtr ptr, int idx, IntPtr pos, bool bWithPos1);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int v3dxBezier_GetNodesCount(IntPtr ptr);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dxBezier_ClearNodes(IntPtr ptr);
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static float v3dxBezier2D_GetValueY(IntPtr ptr, float fTime);
        #endregion

        #region System

        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt64 CPU_GetProcessorPackageCount();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt64 CPU_GetLogicalProcessorCount();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt64 CPU_GetProcessorCoreCount();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt64 CPU_GetProcessorL1CacheCount();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt64 CPU_GetProcessorL2CacheCount();
        [System.Runtime.InteropServices.DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt64 CPU_GetProcessorL3CacheCount();

        #endregion
    }

    public class TestSDK
    {
        public static void Test()
        {
            var ptr = DllImportAPI.XNDNode_New();
            DllImportAPI.XNDNode_AddRef(ptr);
            DllImportAPI.XNDNode_Release(ptr);
            DllImportAPI.XNDNode_Release(ptr);
        }
    }
}