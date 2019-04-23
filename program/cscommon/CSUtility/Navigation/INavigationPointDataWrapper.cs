using System;
using System.Collections.Generic;

namespace CSUtility.Navigation
{

    // NavigationPoint只记录临时数据，不包含实际C++路点指针
    public class NavigationPoint
    {
        public Guid Id;
        public float PosX;
        public float PosY;
        public float PosZ;
        public Guid[] LinkIds;
    }

    public class INavigationPointDataWrapper
    {
        private IntPtr mNavData;
        public IntPtr NavData
        {
            get { return mNavData; }
        }

        public INavigationPointDataWrapper()
        {
            mNavData = IntPtr.Zero;
        }
        ~INavigationPointDataWrapper()
        {
            Cleanup();
        }

        public void Cleanup()
        {
            unsafe
            {
                if (mNavData != IntPtr.Zero)
                {
                    DllImportAPI.NavigationPointData_Release(mNavData);
                    mNavData = IntPtr.Zero;
                }
            }
        }

        public INavigationPointDataWrapper Clone()
        {
            var retNavData = new INavigationPointDataWrapper();
            retNavData.mNavData = this.mNavData;
            return retNavData;
        }

        public void Initialize(float cellWidth, float cellHeight, float mapWidth, float mapHeight)
        {
            unsafe
            {
                Cleanup();

                mNavData = DllImportAPI.NavigationPointData_New();
                DllImportAPI.NavigationPointData_Initialize(mNavData, cellWidth, cellHeight, mapWidth, mapHeight);
            }
        }

        public bool LoadNavigationData(string fileName, string path)
        {
            unsafe
            {
                if(mNavData == IntPtr.Zero)
                    return false;

                DllImportAPI.NavigationPointData_LoadData(mNavData, fileName, path);
                return true;
            }
        }
        public bool SaveNavigationData(string fileName, string path, bool forceSave)
        {
            unsafe
            {
                if (mNavData == IntPtr.Zero)
                    return false;

                DllImportAPI.NavigationPointData_SaveData(mNavData, fileName, path, forceSave);
                return true;
            }
        }

        public bool AddNavigationPoint(Guid id, float x, float y, float z)
        {
            unsafe
            {
                if (mNavData == IntPtr.Zero)
                    return false;

                return (DllImportAPI.NavigationPointData_AddNavigationPoint(mNavData, (IntPtr)(&id), x, y, z) == 0) ? false : true;
            }
        }

        public bool RemoveNavigationPoint(Guid id)
        {
            unsafe
            {
                if (mNavData == IntPtr.Zero)
                    return false;

                return (DllImportAPI.NavigationPointData_RemoveNavigationPoint(mNavData, (IntPtr)(&id)) == 0) ? false : true;
            }
        }

        public bool AddNavigationLink(Guid startId, Guid endId, bool twoWay = true)
        {
            unsafe
            {
                if (mNavData == IntPtr.Zero)
                    return false;

                return (DllImportAPI.NavigationPointData_AddNavigationLink(mNavData, (IntPtr)(&startId), (IntPtr)(&endId), twoWay) == 0) ? false : true;
            }
        }

        public bool RemoveNavigationLink(Guid startId, Guid endId, bool twoWay = true)
        {
            unsafe
            {
                if (mNavData == IntPtr.Zero)
                    return false;

                return (DllImportAPI.NavigationPointData_RemoveNavigationLink(mNavData, (IntPtr)(&startId), (IntPtr)(&endId), twoWay) == 0) ? false : true;
            }
        }

        public bool MoveNavigationPoint(Guid id, float x, float y, float z)
        {
            unsafe
            {
                if (mNavData == IntPtr.Zero)
                    return false;

                return (DllImportAPI.NavigationPointData_MoveNavigationPoint(mNavData, (IntPtr)(&id), x, y, z) == 0) ? false : true;
            }
        }

        public bool GetNavigationDatasFromPtr(ref List<NavigationPoint> outList)
        {
            unsafe
            {
                if (mNavData == IntPtr.Zero)
                    return false;

                outList = new List<NavigationPoint>();
                var count = DllImportAPI.NavigationPointData_GetNavigationPointCount(mNavData);

                for (int i = 0; i < count; i++)
                {
                    System.Guid outId;
                    float outX, outY, outZ;
                    Guid* ids;
                    int linkCount;
                    if (DllImportAPI.NavigationPointData_GetNavigationPointData(mNavData, i, (IntPtr)(&outId), (IntPtr)(&outX), (IntPtr)(&outY), (IntPtr)(&outZ), (IntPtr)(&ids), (IntPtr)(&linkCount)) == 0)
                        continue;

                    NavigationPoint pt = new NavigationPoint();
                    pt.Id = outId;
                    pt.PosX = outX;
                    pt.PosY = outY;
                    pt.PosZ = outZ;
                    Guid[] linkIds = new Guid[linkCount];
                    for (int linkIdx = 0; linkIdx < linkCount; linkIdx++)
                    {
                        linkIds[linkIdx] = ids[linkIdx];
                    }

                    pt.LinkIds = linkIds;

                    if (linkCount > 0)
                    {
                        DllImportAPI.NavigationPointData_DeleteIdArray((IntPtr)ids);
                    }

                    outList.Add(pt);
                }
            }

            return true;
        }

    }
}
