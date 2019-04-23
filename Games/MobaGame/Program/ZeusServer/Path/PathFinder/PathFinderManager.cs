using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Server.Path.PathFinder
{
    public class PFQuery
    {
        public PFMap Map;

        public Guid MapInstanceId = Guid.Empty;
        public SlimDX.Vector3 FromPt;
        public SlimDX.Vector3 ToPt;
        //下面是远程调用的返回处理数据
        public SCore.TcpServer.TcpConnect Connect;
        public UInt16 SerialId;
        public Int64 QueryTime = CSUtility.DllImportAPI.HighPrecision_GetTickCount();

        public void DoWork()
        {
            FindPath(Map, this);
        }

        private static void FindPath(PFMap map, PFQuery qr)
        {
            //var dist = SlimDX.Vector3.Distance(qr.FromPt,qr.ToPt);
            //if (dist > 100)
            //{
            //    dist = 0;
            //}

            var bt = CSUtility.DllImportAPI.HighPrecision_GetTickCount();

            //这里找到的路径想办法通知给调用者，通常应该是一个RPC的return
            var pathPoints = new List<SlimDX.Vector2>();
            //var pathFindResult = map.NavigationProcess.FindPath_NavTile(qr.FromPt.X, qr.FromPt.Z, qr.ToPt.X, qr.ToPt.Z, 20, map.NavigationData, map.PathFindContext, ref pathPoints, true, false);
            var mapTpl = map.MapTemplate;
            var pathFindResult = mapTpl.NavigationProcess.FindPath(qr.MapInstanceId, qr.FromPt.X, qr.FromPt.Z, qr.ToPt.X, qr.ToPt.Z, 20, mapTpl.NavigationData, map.PathFindContext, mapTpl.NavigationPointData, map.PathFindContext_Navigation, ref pathPoints, 30, true, false);

            var nt = CSUtility.DllImportAPI.HighPrecision_GetTickCount();
            var elapsedTime = (int)(nt - qr.QueryTime);
            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            retPkg.Write((Byte)pathFindResult);
            retPkg.Write(pathPoints.Count);
            foreach (var pt in pathPoints)
            {
                retPkg.Write(pt);
            }
            retPkg.Write(elapsedTime);
            retPkg.DoReturnCommand2(qr.Connect, qr.SerialId);

            if (elapsedTime > 10000)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("FindPath = {0}:{5} [{1}->{2}:{3}:{4}]",
                    elapsedTime, qr.FromPt, qr.ToPt,
                    SlimDX.Vector3.Distance(qr.FromPt, qr.ToPt),
                    pathPoints.Count, nt - bt));
            }
        }
    }

    public class PFUnit
    {
        public PFThread ProcThread = null;
        public Guid UnitId;
        public System.Collections.Concurrent.ConcurrentQueue<PFQuery> Querys = new System.Collections.Concurrent.ConcurrentQueue<PFQuery>();

        public void ClearQuerys()
        {
            PFQuery qr;
            while (Querys.TryDequeue(out qr))
            {
                RPC.PackageWriter retPkg = new RPC.PackageWriter();
                retPkg.Write((Byte)CSUtility.Navigation.INavigationWrapper.enNavFindPathResult.ENFR_Cancel);
                retPkg.DoReturnCommand2(qr.Connect, qr.SerialId);
            }
        }
        public void DoAll()
        {
            PFQuery qr;
            while (Querys.TryDequeue(out qr))
            {
                qr.DoWork();
            }
        }
    }

    public class PFMap
    {
        public PFMapTemplate MapTemplate;
        // 需保证context不再多个线程之间调用
        CSUtility.Navigation.PathFindContextWrapper mPathFindContext = new CSUtility.Navigation.PathFindContextWrapper();
        public CSUtility.Navigation.PathFindContextWrapper PathFindContext
        {
            get { return mPathFindContext; }
        }

        // 路点
        CSUtility.Navigation.PathFindContextWrapper_NavigationPoint mPathFindContext_Navigation = new CSUtility.Navigation.PathFindContextWrapper_NavigationPoint();
        public CSUtility.Navigation.PathFindContextWrapper_NavigationPoint PathFindContext_Navigation
        {
            get { return mPathFindContext_Navigation; }
        }

        public void Init(PFMapTemplate template)
        {
            MapTemplate = template;

            CSUtility.Navigation.NavigationInfo navInfo;
            template.NavigationAssistData.NavigationTileData.GetNavigationInfo(out navInfo);
            mPathFindContext.Initialize(navInfo);
        }
    }

    public class PFMapTemplate
    {
        PFMapDynamicBlocksManager mDynBlockManager = new PFMapDynamicBlocksManager();
        public PFMapDynamicBlocksManager DynBlockManager
        {
            get { return mDynBlockManager; }
        }

        Guid mMapId;
        public Guid MapId
        {
            get { return mMapId; }
        }

        Server.Hall.Map.MapNavigationAssistData mNavigationAssistData = new Server.Hall.Map.MapNavigationAssistData();
        public Server.Hall.Map.MapNavigationAssistData NavigationAssistData
        {
            get { return mNavigationAssistData; }
        }
        public CSUtility.Navigation.INavigationDataWrapper NavigationData
        {
            get { return mNavigationAssistData.NavigationTileData; }
        }

        public CSUtility.Navigation.INavigationPointDataWrapper NavigationPointData
        {
            get { return mNavigationAssistData.NavigationPointData; }
        }

        CSUtility.Navigation.INavigationWrapper mNavigationProcess = new CSUtility.Navigation.INavigationWrapper();
        public CSUtility.Navigation.INavigationWrapper NavigationProcess
        {
            get { return mNavigationProcess; }
        }
        public void InitGlobalMap(Guid mapSourceId, bool forceLoad = false)
        {
            mMapId = mapSourceId;

            mNavigationAssistData = Hall.Map.MapPathManager.Instance.GetGlobalMapNavigationAssistData(mapSourceId, forceLoad);//.Clone();
            if (mNavigationAssistData.NavigationTileData != null)
            {
                mNavigationAssistData.NavigationTileData.OnGetDynamicBlockIsBlock = new CSUtility.Navigation.INavigationDataWrapper.Delegate_OnDynamicBlockIsBlock(_OnGetDynamicBlockIsBlock);
                CSUtility.Navigation.NavigationInfo navInfo;
                mNavigationAssistData.NavigationTileData.GetNavigationInfo(out navInfo);

            }
        }

        public void ReloadNavigation()
        {
            mNavigationAssistData = Hall.Map.MapPathManager.Instance.GetGlobalMapNavigationAssistData(mMapId, true);
            if (mNavigationAssistData.NavigationTileData != null)
            {
                mNavigationAssistData.NavigationTileData.OnGetDynamicBlockIsBlock = new CSUtility.Navigation.INavigationDataWrapper.Delegate_OnDynamicBlockIsBlock(_OnGetDynamicBlockIsBlock);
                CSUtility.Navigation.NavigationInfo navInfo;
                mNavigationAssistData.NavigationTileData.GetNavigationInfo(out navInfo);
                var threads = PathFinderManager.Instance.Threads;
                foreach (var i in threads)
                {
                    var map = i.GetMap(mMapId);
                    map.PathFindContext.Initialize(navInfo);
                }
            }

        }

        private int _OnGetDynamicBlockIsBlock(Guid mapInstanceId, Guid actorId)
        {
            var dynBlocks = DynBlockManager.GetDynBlocks(mapInstanceId, this);
            if (dynBlocks == null)
                return 0;

            var data = dynBlocks.GetBlockData(actorId);
            if (data == null)
                return 0;

            return data.IsBlock ? 1 : 0;
        }
    }

    public class PFMapDynamicBlocks
    {
        Guid mId;
        Dictionary<Guid, CSUtility.Map.DynamicBlock.DynamicBlockData> mBlockDatas = new Dictionary<Guid, CSUtility.Map.DynamicBlock.DynamicBlockData>();

        public PFMapDynamicBlocks()
        {

        }

        public void InitBlocks(Guid id, PFMapTemplate map)
        {
            mId = id;
            mBlockDatas.Clear();

            foreach (var data in map.NavigationAssistData.DynamicBlockDatas)
            {
                var newData = data.CloneObject() as CSUtility.Map.DynamicBlock.DynamicBlockData;
                newData.Id = data.Id;
                mBlockDatas[newData.Id] = newData;
            }
        }

        public void UpdateBlocks(RPC.DataReader blocks)
        {
            UInt16 count = 0;
            blocks.Read(out count);

            for (UInt16 i = 0; i < count; i++)
            {
                Guid blockId;
                blocks.Read(out blockId);

                CSUtility.Map.DynamicBlock.DynamicBlockData blockData;
                if (!mBlockDatas.TryGetValue(blockId, out blockData))
                    continue;

                bool isBlock;
                blocks.Read(out isBlock);
                blockData.IsBlock = isBlock;
            }
        }

        public CSUtility.Map.DynamicBlock.DynamicBlockData GetBlockData(Guid id)
        {
            CSUtility.Map.DynamicBlock.DynamicBlockData retBlockData;
            if (mBlockDatas.TryGetValue(id, out retBlockData))
                return retBlockData;

            return null;
        }
    }

    public class PFMapDynamicBlocksManager
    {
        Dictionary<Guid, PFMapDynamicBlocks> mDynBlocks = new Dictionary<Guid, PFMapDynamicBlocks>();

        //public void InitDynBlocks(PFMap map)
        //{
        //    if (map == null || map.NavigationAssistData == null)
        //        return;

        //    foreach (var data in map.NavigationAssistData.DynamicBlockDatas)
        //    {
        //        var block = new PFMapDynamicBlocks();
        //        block.InitBlocks()
        //        mDynBlocks[data.RoleId] = block;
        //    }
        //}

        public PFMapDynamicBlocks FindDynBlocks(Guid mapInstanceId, PFMapTemplate map)
        {
            PFMapDynamicBlocks result = null;
            if (mDynBlocks.TryGetValue(mapInstanceId, out result))
            {
                return result;
            }

            return null;
        }

        public PFMapDynamicBlocks GetDynBlocks(Guid mapInstanceId, PFMapTemplate map)
        {
            PFMapDynamicBlocks result;
            if (false == mDynBlocks.TryGetValue(mapInstanceId, out result))
            {
                result = new PFMapDynamicBlocks();
                //从map初始化出来一个动态改变的阻塞列表
                result.InitBlocks(mapInstanceId, map);
                mDynBlocks[mapInstanceId] = result;
            }

            return result;
        }
        //public PFMapDynamicBlocks GetDynBlocks(Guid blocksId, PFMap map)
        //{
        //    PFMapDynamicBlocks result;
        //    if (false == mDynBlocks.TryGetValue(blocksId, out result))
        //    {
        //        result = new PFMapDynamicBlocks();
        //        //从map初始化出来一个动态改变的阻塞列表
        //        result.InitBlocks(map);
        //        mDynBlocks[blocksId] = result;
        //    }
        //    return result;
        //}
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class PFThread
    {
        CSUtility.Support.ConcurentObjManager<Guid, PFMap> Maps = new CSUtility.Support.ConcurentObjManager<Guid, PFMap>();
        public PFMap GetMap(Guid mapId)
        {
            var result = Maps.FindObj(mapId);
            if (result != null)
            {
                return result;
            }
            result = new PFMap();
            var template = PathFinderManager.Instance.GetMapTemplate(mapId);
            result.Init(template);
            Maps.Add(mapId, result);
            return result;
        }

        public CSUtility.Support.AsyncObjManager<Guid, PFUnit> Querys = new CSUtility.Support.AsyncObjManager<Guid, PFUnit>();

        public int QueueCount
        {
            get { return Querys.Count; }
        }

        public void Tick()
        {
            Querys.BeforeTick();

            CSUtility.Support.AsyncObjManager<Guid, PFUnit>.FOnVisitObject visitor = delegate (Guid key, PFUnit value, object arg)
            {
                lock (value)
                {
                    if (value.ProcThread != this)
                    {
                        Log.FileLog.WriteLine("value.ProcThread != this");
                        Querys.Remove(key);
                        return CSUtility.Support.EForEachResult.FER_Continue;
                    }
                }

                try
                {
                    value.DoAll();
                }
                catch (Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }

                lock (value)
                {
                    if (value.Querys.Count == 0)
                    {
                        Querys.Remove(key);
                        value.ProcThread = null;
                        return CSUtility.Support.EForEachResult.FER_Continue;
                    }
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            Querys.For_Each(visitor, null);

            Querys.AfterTick();
        }

        public bool IsEmptyPFQuery()
        {
            if (Querys.Count == 0)
                return true;
            return false;
        }

        System.Threading.Thread mThread;
        public System.Threading.Thread Thread
        {
            get { return mThread; }
        }

        bool mRunning = false;
        public void StartThread(string name)
        {
            mRunning = true;
            mThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ThreadLoop));
            mThread.Name = name;
            mThread.Start();
        }

        public void StopThread()
        {
            mRunning = false;
        }

        public void ThreadLoop()
        {
            while (mRunning)
            {
                if (IsEmptyPFQuery() == true)
                    System.Threading.Thread.Sleep(1);

                Tick();
            }
        }
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class PathFinderManager
    {
        static PathFinderManager smInstance = new PathFinderManager();
        public static PathFinderManager Instance
        {
            get { return smInstance; }
        }
        CSUtility.Support.ConcurentObjManager<Guid, PFMapTemplate> TeamplateMaps = new CSUtility.Support.ConcurentObjManager<Guid, PFMapTemplate>();
        public PFMapTemplate GetMapTemplate(Guid mapSourceId)
        {
            lock (this)
            {
                var result = TeamplateMaps.FindObj(mapSourceId);
                if (result != null)
                    return result;

                result = new PFMapTemplate();
                result.InitGlobalMap(mapSourceId);
                TeamplateMaps.Add(mapSourceId, result);
                return result;
            }
        }

        CSUtility.Support.ConcurentObjManager<Guid, PFUnit> Units = new CSUtility.Support.ConcurentObjManager<Guid, PFUnit>();
        private PFUnit GetUnit(Guid unitId)
        {
            var result = Units.FindObj(unitId);
            if (result != null)
                return result;
            result = new PFUnit();
            result.UnitId = unitId;
            Units.Add(unitId, result);
            return result;
        }

        List<PFThread> mThreads = new List<PFThread>();
        public List<PFThread> Threads
        {
            get { return mThreads; }
        }

        private PFThread SelectPFThread()
        {
            PFThread result = null;
            int minValue = int.MaxValue;
            foreach (var i in mThreads)
            {
                if (i.Querys.Count < minValue)
                {
                    minValue = i.Querys.Count;
                    result = i;
                }
            }
            return result;
        }

        public void StartThread(int count)
        {            
            var m = new System.Management.ManagementClass("Win32_Processor");
            var mn = m.GetInstances();
            Log.FileLog.WriteLine("CPU数量：{0}", mn.Count);

            int CpuCoreNumber = 0;
            var MySearcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (var MyObject in MySearcher.Get())
            {
                Log.FileLog.WriteLine("CPU频率：{0}", MyObject.Properties["CurrentClockSpeed"].Value.ToString());

                string numberOfCores = MyObject["NumberOfCores"].ToString();
                int coreCount;
                bool success = int.TryParse(numberOfCores, out coreCount);
                if (success)
                {
                    CpuCoreNumber += coreCount;
                }
            }

            Log.FileLog.WriteLine("CPU内核数量：{0}", CpuCoreNumber);
            if (count == 0)
                count = CpuCoreNumber;

            Log.FileLog.WriteLine("寻路处理器线程数量：{0}", count);

            for (int i = 0; i < count; i++)
            {
                var thread = new PFThread();
                mThreads.Add(thread);
                thread.StartThread("PFThread" + i);
            }
        }

        public void StopThread()
        {
            foreach (var i in mThreads)
            {
                i.StopThread();
            }
        }

        public void QueryGlobalMapPath(SCore.TcpServer.TcpConnect connect, UInt16 serialId, Guid hallsId, Guid mapSourceId, Guid mapInstanceId, Guid roleId, SlimDX.Vector3 from, SlimDX.Vector3 to, bool bClear)
        {
            if (mapSourceId == Guid.Empty)
            {
                RPC.PackageWriter retPkg = new RPC.PackageWriter();
                retPkg.Write((Byte)0);
                retPkg.Write((int)0);
                retPkg.Write((int)0);
                retPkg.DoReturnCommand2(connect, serialId);
                return;
            }
            PFUnit unit = GetUnit(roleId);

            PFQuery query = new PFQuery();
            if (mapInstanceId == Guid.Empty)
                query.MapInstanceId = hallsId;
            else
                query.MapInstanceId = mapInstanceId;
            query.FromPt = from;
            query.ToPt = to;
            query.Connect = connect;
            query.SerialId = serialId;

            if (bClear)
            {
                unit.ClearQuerys();
            }

            lock (unit)
            {
                if (unit.ProcThread != null)
                {
                    //说明还在处理1队列里面，不用再选择处理线程塞入
                    query.Map = unit.ProcThread.GetMap(mapSourceId);
                    unit.Querys.Enqueue(query);
                }
                else
                {
                    var procThread = SelectPFThread();
                    unit.ProcThread = procThread;

                    query.Map = unit.ProcThread.GetMap(mapSourceId);
                    unit.Querys.Enqueue(query);
                    procThread.Querys.Add(roleId, unit);
                }
            }
        }

        public void SetMapBlocks(Guid planesId, Guid mapSourceId, Guid mapInstanceId, RPC.DataReader modifyBlocks)
        {
            var map = GetMapTemplate(mapSourceId);
            if (mapInstanceId == Guid.Empty)
            {
                var dynBlocks = map.DynBlockManager.GetDynBlocks(planesId, map);//全局地图，可以用位面id作为动态块标识
                dynBlocks.UpdateBlocks(modifyBlocks);
            }
            else
            {
                var dynBlocks = map.DynBlockManager.GetDynBlocks(mapInstanceId, map);
                dynBlocks.UpdateBlocks(modifyBlocks);
            }
        }

        public void ReloadNavigation(Guid planesId, Guid mapSourceId)
        {
            var map = GetMapTemplate(mapSourceId);
            map.ReloadNavigation();
        }
    }
}
