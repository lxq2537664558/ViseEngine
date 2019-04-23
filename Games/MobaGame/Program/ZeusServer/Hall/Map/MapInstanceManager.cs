using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall.Map
{
    public class MapInstanceManager
    {
        public static MapInstanceManager Instance { get; } = new MapInstanceManager();

        HallManager mHallsManager = new HallManager();
        public HallManager HallsManager
        {
            get { return mHallsManager; }
        }

        public MapInstance[] Maps { get; } = new MapInstance[UInt16.MaxValue];
        public MapInstance GetMapInstance(UInt16 index)
        {
            return Maps[index];
        }

        Dictionary<Guid, MapInstance> mInstanceMaps = new Dictionary<Guid, MapInstance>();
        public Dictionary<Guid, MapInstance> InstanceMaps
        {
            get { return mInstanceMaps; }
        }

        public MapInstance GetInstanceMap(Guid mapId, Guid ownerId)
        {
            foreach (var i in mInstanceMaps)
            {
                if (i.Value.MapTemplate.MapGuid == mapId)
                {
                    return i.Value;
                }
            }
            return null;
        }

        public MapInstance NewMapInstance<MT>() where MT : MapInstance, new()
        {
            lock(this)
            {
                var map = new MT();
                for (var i = 0; i < Maps.Length; i++)
                {
                    if (Maps[i] == null)
                    {
                        Maps[i] = map;
                        map.IndexInManager = (UInt16)i;
                        break;
                    }
                }
                return map;
            }
        }
        public MapInstance RemoveMapInstance(UInt16 index)
        {
            lock(this)
            {
                var result = Maps[index];
                Maps[index].IndexInManager = UInt16.MaxValue;
                Maps[index] = null;
                return result;
            }
        }

        public void Tick(long elapsedMiliSeccond)
        {//暂时不用做任何事，以后也许有一些跨地图的对象需要处理？

        }

        public void PlayerEnterMap(Role.Player.PlayerInstance player, Guid mapSourceId, SlimDX.Vector3 pos)
        {
            MapInstance map=null;
            lock (this)
            {
                var mapInit = Map.MapInstanceManager.GetMapInitBySourceId(mapSourceId);
                var halls = player.HallInstance;
                map = halls.GetGlobalMap(mapSourceId);
                if (map == null)
                {
                    map = CreateMapInstance(halls, mapSourceId);
                    if (map == null)
                    {
                        map = GetDefaultMapInstance(halls);
                    }
                    halls.AddGlobalMap(map.MapSourceId, map);
                }
            }
            if (map.OnPlayerEnterMap(player, pos))
            {
                Log.FileLog.WriteLine(string.Format("Plarer:{0},EnterMap Success",player.RoleTemplate.NickName));
            }
            player.Placement.SetLocation(ref pos);
        }

        public void PlayerLeaveMap(Role.Player.PlayerInstance player, bool bSaveRole)
        {
            player.HostMap.OnPlayerLeaveMap(player);
        }

        static Dictionary<Guid, CSUtility.Map.WorldInit> mMapInitDictionary = new Dictionary<Guid, CSUtility.Map.WorldInit>();
        public static CSUtility.Map.WorldInit GetMapInitBySourceId(Guid mapSourceId)
        {
            lock (mMapInitDictionary)
            {
                CSUtility.Map.WorldInit outInit;
                if (mMapInitDictionary.TryGetValue(mapSourceId, out outInit))
                {
                    return outInit;
                }

                outInit = new CSUtility.Map.WorldInit();

                var mapDir = FindMapConfigFolderInFolder(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory, mapSourceId);
                if (string.IsNullOrEmpty(mapDir))
                {
                    mapSourceId = CSUtility.Support.IFileConfig.DefaultMapId;
                    mapDir = FindMapConfigFolderInFolder(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory, mapSourceId);
                }

                var mapConfigFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mapDir + "\\Config.map");
                CSUtility.Support.IConfigurator.FillProperty(outInit, mapConfigFileName);
                mMapInitDictionary[mapSourceId] = outInit;

                return outInit;
            }
        }
        public static string FindMapConfigFolderInFolder(string folder, Guid mapSourceId)
        {
            var dirs = System.IO.Directory.GetDirectories(folder, mapSourceId.ToString(), System.IO.SearchOption.AllDirectories);
            if (dirs.Length > 0)
                return dirs[0];
//             foreach (var dir in dirs)
//             {
//                 var dirName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(dir);
// 
//                 Guid mapId;
//                 if (System.Guid.TryParse(dirName, out mapId))
//                 {
//                     if (mapId == mapSourceId)
//                         return dir;
//                 }
//                 else
//                 {
//                     var result = FindMapConfigFolderInFolder(dir, mapSourceId);
//                     if (!string.IsNullOrEmpty(result))
//                         return result;
//                 }
//             }
            return "";
        }

        public MapInstance CreateMapInstance(HallInstance halls, Guid mapSourceId)
        {
            for (UInt16 i = 0; i < UInt16.MaxValue; i++)
            {
                if (Maps[i] == null)
                {
                    var map = MapInstance.CreateMapInstance(halls, i, mapSourceId);
                    if (map != null)
                    {
                        Maps[i] = map;
                        LogicProcessorManager.Instance.PushMap(map);
                        return map;
                    }
                    else
                    {
                        Log.FileLog.WriteLine("CreateMapInstance {0} Failed", mapSourceId);
                        return null;
                    }
                }
            }
            return null;
        }

        public void RemoveInstanceMap(Guid mapInstanceId)
        {
            lock (this)
            {
                MapInstance map;
                if (mInstanceMaps.TryGetValue(mapInstanceId, out map))
                {
                    Maps[map.IndexInManager] = null;
                    map.OnMapClosed();
                    map.ReleaseInstanceMap();
                    mInstanceMaps.Remove(mapInstanceId);
                }
            }
        }

        public MapInstance GetDefaultMapInstance(HallInstance planes)
        {
            MapInstance map;
            var mapId = CSUtility.Support.IFileConfig.DefaultMapId;
            if (mInstanceMaps.TryGetValue(mapId, out map))
                return map;
            for (UInt16 i = 0; i < UInt16.MaxValue; i++)
            {
                if (Maps[i] == null)
                {
                    map = MapInstance.CreateMapInstance(planes, i, mapId);
                    if (map != null)
                    {
                        Maps[i] = map;
                        LogicProcessorManager.Instance.PushMap(map);
                        return map;
                    }
                }
            }

            return null;
        }
    }
}
