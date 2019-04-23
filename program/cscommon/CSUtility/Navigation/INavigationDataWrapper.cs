using System;

namespace CSUtility.Navigation
{
    public class INavigationDataWrapper
    {
        //Guid TestId = Guid.NewGuid();

        public delegate int Delegate_OnDynamicBlockIsBlock(Guid mapInstanceId, Guid actorId);
        Delegate_OnDynamicBlockIsBlock OnGetDynamicBlockIsBlock_C;
        public Delegate_OnDynamicBlockIsBlock OnGetDynamicBlockIsBlock;

        // 编辑器用接口
        public delegate void Delegate_OnGetActorGUID_C(UInt32 id, IntPtr actorId);
        Delegate_OnGetActorGUID_C OnGetActorGuid_C;
        public delegate Guid Delegate_OnGetActorGUIDFromUInt32Id(UInt32 id);
        public Delegate_OnGetActorGUIDFromUInt32Id OnGetActorGUIDFromUInt32Id;
         
        // 编辑器用接口
        public delegate UInt32 Delegate_OnGetActorUIntID(Guid actorId);
        Delegate_OnGetActorUIntID OnGetActorUIntId_C;
        public Delegate_OnGetActorUIntID OnGetActorUIntId;

        private IntPtr mNavData;
        public IntPtr NavData
        {
            get { return mNavData; }
        }

        public enum enNavDataType
        {
            NDT_Auto,
            NDT_Manual,
        }

        public enum enCheckType
        {
            CT_None     = 0,
            CT_Through  = 1,    // 检查通过点
            CT_Extend   = 1<<1, // 检查扩展的点
        }

        public INavigationDataWrapper()
        {
            mNavData = IntPtr.Zero;

            OnGetDynamicBlockIsBlock_C = new Delegate_OnDynamicBlockIsBlock(_OnGetDynamicBlockIsBlock);
            OnGetActorGuid_C = new Delegate_OnGetActorGUID_C(_OnGetActorGUID);
            OnGetActorUIntId_C = new Delegate_OnGetActorUIntID(_OnGetActorUIntID);
        }
        ~INavigationDataWrapper()
        {
            Cleanup();
        }
        public void Cleanup()
        {
            unsafe
            {
                DllImportAPI.NavigationData_Release(mNavData);
                mNavData = IntPtr.Zero;
            }
        }

        private int _OnGetDynamicBlockIsBlock(Guid mapInstanceId, Guid actorId)
        {
            if (OnGetDynamicBlockIsBlock != null)
                return OnGetDynamicBlockIsBlock(mapInstanceId, actorId);
            return 0;
        }
        private void _OnGetActorGUID(UInt32 id, IntPtr actorId)
        {
            unsafe
            {
                if (OnGetActorGUIDFromUInt32Id != null)
                {
                    *(Guid*)actorId = OnGetActorGUIDFromUInt32Id(id);
                    return;
                }

                *(Guid*)actorId = Guid.Empty;
            }
        }
        private UInt32 _OnGetActorUIntID(Guid actorId)
        {
            if (OnGetActorUIntId != null)
                return OnGetActorUIntId(actorId);
            return 0;
        }

        public INavigationDataWrapper Clone()
        {
            INavigationDataWrapper retNavData = new INavigationDataWrapper();
            retNavData.mNavData = this.mNavData;
            retNavData.OnGetDynamicBlockIsBlock = this.OnGetDynamicBlockIsBlock;
            retNavData.OnGetActorGUIDFromUInt32Id = this.OnGetActorGUIDFromUInt32Id;
            retNavData.OnGetActorUIntId = this.OnGetActorUIntId;
            return retNavData;
        }

        public bool ConstrutNavigationData(string name, string path, NavigationInfo info)
        {
            unsafe
            {
                Cleanup();

                mNavData = DllImportAPI.NavigationData_New();
                DllImportAPI.NavigationData_SetDynamicBlockEvent(mNavData, OnGetDynamicBlockIsBlock_C, OnGetActorGuid_C, OnGetActorUIntId_C);
                return DllImportAPI.NavigationData_ConstructNavigation(mNavData, name, path, (IntPtr)(&info));
            }
        }

        public bool LoadNavigationData(string name, string path, bool bForceAll)
        {
            unsafe
            {
                Cleanup();

                mNavData = DllImportAPI.NavigationData_New();
                DllImportAPI.NavigationData_SetDynamicBlockEvent(mNavData, OnGetDynamicBlockIsBlock_C, OnGetActorGuid_C, OnGetActorUIntId_C);
                if (bForceAll)
                {
                    return DllImportAPI.NavigationData_LoadNavigation_All(mNavData, name, path);
                }
                
                return DllImportAPI.NavigationData_LoadNavigation(mNavData, name, path);
            }
        }
        public bool SaveNavigationData(string name, string path, bool forceSave)
        {
            unsafe
            {
                if(mNavData == IntPtr.Zero)
                    return false;

                if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path))
                    return DllImportAPI.NavigationData_SaveDirtyLevel(mNavData, null, null, forceSave);
                else
                    return DllImportAPI.NavigationData_SaveDirtyLevel(mNavData, name, path, forceSave);
            }
        }

        public void CreateLevel(UInt16 iColX, UInt16 iRowZ)
        {
            if(mNavData == IntPtr.Zero)
                return;

            DllImportAPI.NavigationData_NewLevel(mNavData, iColX, iRowZ);
        }
        public bool TravelTo(float x, float z, Int64 time)
        {
            if(mNavData == IntPtr.Zero)
                return false;

            return DllImportAPI.NavigationData_TravelTo(mNavData, x, z, time);
        }
        public void ForceLoadAllLevels(Int64 time)
        {
            if(mNavData == IntPtr.Zero)
                return;

            DllImportAPI.NavigationData_ForceLoadAllLevels(mNavData, time);
        }
        public bool IsCollide(Guid mapInstanceId, float x, float z, Int64 time, UInt64 ckType)
        {
            if(mNavData == IntPtr.Zero)
                return false;

            unsafe
            {
                return !DllImportAPI.NavigationData_CheckNavDataF(mNavData, &mapInstanceId, x, z, ckType);
            }
        }
        public void SetDynamicNavData(Guid mapInstanceId, float x, float z, float radius, bool block)
        {
            if (mNavData == IntPtr.Zero)
                return;

            unsafe
            {
                DllImportAPI.NavigationData_SetDynamicNavDataF(mNavData, &mapInstanceId, x, z, radius, block ? 1 : 0);
            }
        }

        public bool IsAvailable()
        {
            return mNavData != IntPtr.Zero;
        }

        //public IntPtr GetNavData()
        //{
        //    return mNavData;
        //}

        public bool GetNavigationInfo(out NavigationInfo info)
        {
            if (mNavData == IntPtr.Zero)
            {
                info = new NavigationInfo();
                return false;
            }

            unsafe
            {
                NavigationInfo tempInfo = new NavigationInfo();
                DllImportAPI.NavigationData_GetNavigationInfo(mNavData, (IntPtr)(&tempInfo));
                info = tempInfo;
            }

            return true;
        }

        public void Tick(Int64 time)
        {
            if(mNavData == IntPtr.Zero)
                return;

            unsafe
            {
                DllImportAPI.NavigationData_KickOffCache(mNavData, time, 15000);
            }
        }

        public void AddLevel(UInt16 iColX, UInt16 iRowZ, Int64 time)
        {
            if(mNavData == IntPtr.Zero)
                return;

            unsafe
            {
                DllImportAPI.NavigationData_AddLevel(mNavData, iColX, iRowZ, time);
            }
        }
        public void DelLevel(UInt16 iColX, UInt16 iRowZ, Int64 time)
        {
            if(mNavData == IntPtr.Zero)
                return;

            unsafe
            {
                DllImportAPI.NavigationData_DelLevel(mNavData, iColX, iRowZ, time);
            }
        }

        // delta为每像素包含的字节个数
		public bool GenerateNavData(UInt16 iColX, UInt16 iRowZ, IntPtr data, byte delta, Int64 time)
        {
            if (mNavData == IntPtr.Zero)
                return false;

            unsafe
            {
                return DllImportAPI.NavigationData_GenerateNavData(mNavData, iColX, iRowZ, data, delta, time);
            }
        }
        public bool GetNavData(UInt16 iColX, UInt16 iRowZ, IntPtr data, byte delta, Int64 time)
        {
            if (mNavData == IntPtr.Zero)
                return false;

            unsafe
            {
                return DllImportAPI.NavigationData_GetNavData(mNavData, iColX, iRowZ, data, delta, time);
            }
        }

        public bool ClearNavData(enNavDataType dataType, UInt16 iColX, UInt16 iRowZ, Int64 time)
        {
            if (mNavData == IntPtr.Zero)
                return false;

            unsafe
            {
                return DllImportAPI.NavigationData_ClearNavDataWithXZ(mNavData, (int)dataType, iColX, iRowZ, time);
            }
        }
        public bool ClearNavData(enNavDataType dataType, Int64 time)
        {
            if (mNavData == IntPtr.Zero)
                return false;

            unsafe
            {
                return DllImportAPI.NavigationData_ClearNavData(mNavData, (int)dataType, time);
            }
        }

        public Guid[] GetAllDynamicBlockIds(Int64 frameMillisecondTime)
        {
            unsafe
            {
                Guid* ids;
                int count;
                DllImportAPI.NavigationData_GetAllDynamicBlockIDs(mNavData, (IntPtr)(&ids), (IntPtr)(&count), frameMillisecondTime);

                Guid[] retIds = new Guid[count];

                for (int i = 0; i < count; i++)
                {
                    retIds[i] = ids[i];
                }

                if (count > 0)
                {
                    DllImportAPI.NavigationData_DeleteIDArray((IntPtr)ids);
                }

                return retIds;
            }
        }
    }
}
