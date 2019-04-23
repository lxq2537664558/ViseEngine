using System;
using System.Collections.Generic;

namespace CSUtility.Navigation
{
    public class INavigationWrapper
    {
        private IntPtr mNavigation;

        public enum enNavFindPathResult
        {
            ENFR_Error = 0,
            ENFR_NoStart,
            ENFR_NoEnd,
            ENFR_Success,
            ENFR_NoPath,
            ENFR_Step,
            ENFR_SESame,
            ENFR_Cancel,
        }

        public INavigationWrapper()
        {
            unsafe
            {
                mNavigation = DllImportAPI.Navigation_New();
            }
        }
        ~INavigationWrapper()
        {
            Cleanup();
        }
        public void Cleanup()
        {
            unsafe
            {
                if (mNavigation != IntPtr.Zero)
                {
                    DllImportAPI.Navigation_Delete(mNavigation);
                    mNavigation = IntPtr.Zero;

                }
            }
        }

        //public void InitNavigationData(INavigationDataWrapper navData)
        //{
        //    if (mNavigation == IntPtr.Zero)
        //        return;

        //    unsafe
        //    {
        //        IDllImportAPI.Navigation_InitNavigationData(mNavigation);//, navData.NavData);
        //    }
        //}

        public bool HasBarrier(Guid mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ, INavigationDataWrapper navData)
        {
            if (navData == null || mNavigation==null)
                return false;
            unsafe
            {
                return ((DllImportAPI.Navigation_HasBarrier(mNavigation, &mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, navData.NavData) == 0) ? false : true);
            }
        }

        public enNavFindPathResult FindPath_NavTile(Guid mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ, int range, INavigationDataWrapper navData, PathFindContextWrapper context, ref List<SlimDX.Vector2> result, bool reFind, bool checkDirLine)
        {
            if (mNavigation == IntPtr.Zero)
                return enNavFindPathResult.ENFR_Error;

            result.Clear();

            unsafe
            {
                IntPtr navPoint;
                var retResult = (INavigationWrapper.enNavFindPathResult)(DllImportAPI.Navigation_FindPath_NavTile(mNavigation, &mapInstanceId, navData.NavData, context.PathFindContext, inStartX, inStartZ, inEndX, inEndZ, range, reFind ? 1 : 0, checkDirLine ? 1 : 0, out navPoint));

                switch (retResult)
                {
                    case enNavFindPathResult.ENFR_Success:
                        {
                            while (navPoint != IntPtr.Zero && DllImportAPI.NavigationTilePoint_GetParentPoint(navPoint) != IntPtr.Zero)
                            {
                                SlimDX.Vector2 vec = new SlimDX.Vector2()
                                {
                                    X = DllImportAPI.NavigationTilePoint_GetPointSceneX(navPoint),
                                    Y = DllImportAPI.NavigationTilePoint_GetPointSceneZ(navPoint)
                                };

                                result.Insert(0, vec);

                                navPoint = DllImportAPI.NavigationTilePoint_GetParentPoint(navPoint);
                            }
                        }
                        break;

                    case enNavFindPathResult.ENFR_Step:
                        break;
                }

                return retResult;
            }
        }

        public enNavFindPathResult FindPath_NavPt(float startX, float startY, float startZ, float endX, float endY, float endZ, INavigationPointDataWrapper navData, PathFindContextWrapper_NavigationPoint context, ref List<SlimDX.Vector2> result)
        {
            unsafe
            {
                if (mNavigation == IntPtr.Zero)
                    return enNavFindPathResult.ENFR_Error;

                result.Clear();

                unsafe
                {
                    IntPtr navPoint;
                    var retResult = (INavigationWrapper.enNavFindPathResult)(DllImportAPI.Navigation_FindPath_NavPt(mNavigation, navData.NavData, startX, startY, startZ, endX, endY, endZ, context.PathFindContext, out navPoint));

                    switch (retResult)
                    {
                        case enNavFindPathResult.ENFR_Success:
                            {
                                while (navPoint != IntPtr.Zero && DllImportAPI.NavigationPoint_GetParentPoint(navPoint) != IntPtr.Zero)
                                {
                                    var pos = SlimDX.Vector2.Zero;
                                    float y;
                                    DllImportAPI.NavigationPoint_GetPosition(navPoint, (IntPtr)(&pos.X), (IntPtr)(&y), (IntPtr)(&pos.Y));

                                    result.Insert(0, pos);

                                    navPoint = DllImportAPI.NavigationPoint_GetParentPoint(navPoint);
                                }
                            }
                            break;

                        case enNavFindPathResult.ENFR_SESame:
                            {
                                var pos = SlimDX.Vector2.Zero;
                                float y;
                                DllImportAPI.NavigationPoint_GetPosition(navPoint, (IntPtr)(&pos.X), (IntPtr)(&y), (IntPtr)(&pos.Y));

                                result.Insert(0, pos);
                            }
                            break;
                    }

                    return retResult;
                }
            }
        }

        // maxStepToChangeNavigationPoint 超过多少步之后如果没有找到路径就切换到路点寻路
        public enNavFindPathResult FindPath(Guid mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ, int range, INavigationDataWrapper navData, PathFindContextWrapper context, INavigationPointDataWrapper navPtData, PathFindContextWrapper_NavigationPoint navPtContext, ref List<SlimDX.Vector2> result, float maxMeterToChangeNavigationPoint = 30, bool reFind = true, bool checkDirLine = true)
        {
            if (navPtData == null || DllImportAPI.NavigationPointData_HasNavigationPoints(navPtData.NavData) == 0)
            {
                return FindPath_NavTile(mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, range, navData, context, ref result, reFind, checkDirLine);
            }
            else
            {
                var length = System.Math.Sqrt((inEndX - inStartX) * (inEndX - inStartX) + (inEndZ - inStartZ) * (inEndZ - inStartZ));
                var retResult = enNavFindPathResult.ENFR_Step;
                if (length < maxMeterToChangeNavigationPoint)
                {
                    var maxStepToChangeNavigationPoint = (int)(maxMeterToChangeNavigationPoint * 10);
                    //var maxSetp = GetMaxStep();
                    context.SetMaxStep(maxStepToChangeNavigationPoint);

                    retResult = FindPath_NavTile(mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, range, navData, context, ref result, reFind, checkDirLine);
                    context.SetMaxStep(0);
                }

                switch (retResult)
                {
                    case enNavFindPathResult.ENFR_Step:
                        {
                            // 使用路点寻路

                            List<SlimDX.Vector2> ptList = new List<SlimDX.Vector2>();

                            retResult = FindPath_NavPt(inStartX, 0.0f, inStartZ, inEndX, 0.0f, inEndZ, navPtData, navPtContext, ref ptList);
                            switch (retResult)
                            {
                                case enNavFindPathResult.ENFR_Success:
                                    {
                                        // 折返处理
                                        if (ptList.Count > 1)
                                        {
                                            // 判断起始位置和第一个点的连线以及第一个点到第二个点的连线是否折返
                                            SlimDX.Vector2 vStart = new SlimDX.Vector2(inStartX, inStartZ);
                                            if (SlimDX.Vector2.Dot(SlimDX.Vector2.Normalize(vStart - ptList[0]), SlimDX.Vector2.Normalize(ptList[1] - ptList[0])) > 0)
                                            {
                                                ptList.RemoveAt(0);
                                            }
                                        }
                                        else
                                        {
                                            // 使用网格寻路
                                            return FindPath_NavTile(mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, range, navData, context, ref result, reFind, checkDirLine);
                                        }
                                        if (ptList.Count > 1)
                                        {
                                            // 判断结束点位置和最后一个点的连线以及最后一个点与倒数第二个点的连线是否折返
                                            SlimDX.Vector2 vEnd = new SlimDX.Vector2(inEndX, inEndZ);
                                            if (SlimDX.Vector2.Dot(SlimDX.Vector2.Normalize(vEnd - ptList[ptList.Count - 1]), SlimDX.Vector2.Normalize(ptList[ptList.Count - 2] - ptList[ptList.Count - 1])) > 0)
                                            {
                                                ptList.RemoveAt(ptList.Count - 1);
                                            }
                                        }

                                        List<SlimDX.Vector2> frontList = new List<SlimDX.Vector2>();
                                        List<SlimDX.Vector2> backList = new List<SlimDX.Vector2>();
                                        var findPathResult = FindPath_NavTile(mapInstanceId, inStartX, inStartZ, ptList[0].X, ptList[0].Y, range, navData, context, ref frontList, true, false);
                                        if (findPathResult != enNavFindPathResult.ENFR_Success)
                                            return findPathResult;

                                        findPathResult = FindPath_NavTile(mapInstanceId, ptList[ptList.Count - 1].X, ptList[ptList.Count - 1].Y, inEndX, inEndZ, range, navData, context, ref backList, true, true);
                                        if (findPathResult != enNavFindPathResult.ENFR_Success)
                                            return findPathResult;

                                        frontList.RemoveAt(frontList.Count - 1);
                                        ptList.InsertRange(0, frontList);
                                        ptList.AddRange(backList);
                                        result = ptList;

                                        return enNavFindPathResult.ENFR_Success;
                                    }
                                case enNavFindPathResult.ENFR_SESame:
                                    {
                                        // 只找到一个寻路点则用不用路点寻路
                                        retResult = FindPath_NavTile(mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, range, navData, context, ref result, reFind, checkDirLine);
                                        return retResult;
                                    }

                                default:
                                    return retResult;
                            }
                        }

                    default:
                        return retResult;
                }
            }

            //return enNavFindPathResult.ENFR_Error;
        }

        public void SetMaxStep(int maxStep)
        {
            unsafe
            {
                if (mNavigation != IntPtr.Zero)
                    DllImportAPI.Navigation_SetMaxStep(mNavigation, maxStep);
            }
        }

        public int GetMaxStep()
        {
            unsafe
            {
                if (mNavigation != IntPtr.Zero)
                    return DllImportAPI.Navigation_GetMaxStep(mNavigation);

                return 0;
            }
        }

        public bool GetFarthestPathPointFromStartInLine(Guid mapInstanceId, float inStartX, float inStartZ, float inEndX, float inEndZ, out float outX, out float outZ, INavigationDataWrapper navData)
        {
            unsafe
            {
                outX = 0;
                outZ = 0;

                if (mNavigation != IntPtr.Zero)
                {
                    fixed(float* outFltX = &outX)
                    {
                        fixed(float* outFltZ = &outZ)
                        {
                            var retValue = DllImportAPI.Navigation_GetFarthestPathPointFromStartInLine(mNavigation, &mapInstanceId, inStartX, inStartZ, inEndX, inEndZ, (IntPtr)outFltX, (IntPtr)outFltZ, navData.NavData);
                            return (retValue == 0) ? false : true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
