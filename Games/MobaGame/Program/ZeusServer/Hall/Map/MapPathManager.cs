using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall.Map
{
    public class MapNavigationAssistData
    {
        public CSUtility.Navigation.INavigationDataWrapper NavigationTileData;
        public CSUtility.Navigation.INavigationPointDataWrapper NavigationPointData;
        public List<CSUtility.Map.DynamicBlock.DynamicBlockData> DynamicBlockDatas = new List<CSUtility.Map.DynamicBlock.DynamicBlockData>();

        public MapNavigationAssistData Clone()
        {
            MapNavigationAssistData retValue = new MapNavigationAssistData();
            if (NavigationTileData != null)
                retValue.NavigationTileData = NavigationTileData.Clone();
            if (NavigationPointData != null)
                retValue.NavigationPointData = NavigationPointData.Clone();
            retValue.DynamicBlockDatas.Clear();
            foreach (var data in DynamicBlockDatas)
            {
                var newData = new CSUtility.Map.DynamicBlock.DynamicBlockData();
                newData.CopyFrom(data);
                retValue.DynamicBlockDatas.Add(newData);
            }

            return retValue;
        }
    }

    public class MapPathManager
    {
        static MapPathManager smInstance = new MapPathManager();
        public static MapPathManager Instance
        {
            get { return smInstance; }
        }        

        private MapPathManager()
        {
            
        }        

        Dictionary<Guid, MapNavigationAssistData> mNavigationAssistDataDic = new Dictionary<Guid, MapNavigationAssistData>();

        public MapNavigationAssistData GetGlobalMapNavigationAssistData(Guid mapSourceId, bool forceLoad = false)
        {
            if (!forceLoad)
            {
                MapNavigationAssistData navData = null;
                if (mNavigationAssistDataDic.TryGetValue(mapSourceId, out navData))
                    return navData;
            }

            return LoadGlobalMapNavigationAssistData(mapSourceId);
        }

        MapNavigationAssistData LoadGlobalMapNavigationAssistData(Guid mapSourceId)
        {
            lock (this)
            {
                if (mapSourceId == Guid.Empty)
                    return null;

                var dir = CSUtility.Map.MapManager.Instance.GetMapPath(mapSourceId);                

                var navDir = dir + "/Navigation/Navigation.nav";
                var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(navDir);
                var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(navDir);                

                MapNavigationAssistData assistData = new MapNavigationAssistData();

                CSUtility.Navigation.NavigationInfo navInfo = new CSUtility.Navigation.NavigationInfo();
                navInfo.ResetDefault();

                // 读取寻路网格信息
                assistData.NavigationTileData = new CSUtility.Navigation.INavigationDataWrapper();
                assistData.NavigationTileData.ConstrutNavigationData(file, path,navInfo);
                assistData.NavigationTileData.LoadNavigationData(file, path, true);
                assistData.NavigationTileData.GetNavigationInfo(out navInfo);                

                // 读取路点信息
                assistData.NavigationPointData = new CSUtility.Navigation.INavigationPointDataWrapper();
                assistData.NavigationPointData.Initialize(navInfo.GetLevelXMeterLength(), navInfo.GetLevelZMeterLength(),
                                                          navInfo.GetValidXPixelLength() * navInfo.GetPixelXMeterLength(),
                                                          navInfo.GetValidZPixelLength() * navInfo.GetPixelZMeterLength());
                assistData.NavigationPointData.LoadNavigationData(file + "pt", path);                

                // 读取动态碰撞体信息
                LoadMapSpecialData(mapSourceId, CSUtility.Component.EActorGameType.DynamicBlock, assistData);

                mNavigationAssistDataDic[mapSourceId] = assistData;

                return assistData;
            }
        }

        void LoadMapSpecialData(Guid mapId, CSUtility.Component.EActorGameType type, MapNavigationAssistData assistData)
        {
            var floder = CSUtility.Map.MapManager.Instance.GetMapPath(mapId);
            var sceneDir = floder + "/" + type.ToString();
            sceneDir = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(sceneDir);
            if(System.IO.Directory.Exists(sceneDir))
            {
                var files = System.IO.Directory.GetFiles(sceneDir);
                foreach (var i in files)
                {
                    LoadMapLevel(i, assistData);
                }
            }
        }

        void LoadMapLevel(string levelFile, MapNavigationAssistData assistData)
        {
            var levelHolder = CSUtility.Support.XndHolder.LoadXND(levelFile);
            if (levelHolder == null)
                return;

            var patchsNode = levelHolder.Node.FindNode("Patch");
            foreach (var node in patchsNode.GetNodes())
            {
                var objNode = node.FindNode("DynamicTileObjects");
                if (objNode != null)
                {
                    foreach (var actorNode in objNode.GetNodes())
                    {
                        try
                        {
                            var typeName = actorNode.GetName();

                            var objAtt = actorNode.FindAttrib("ActorData");

                            switch (typeName)
                            {
                                // DynamicBlock
                                case "CSUtility.Component.EActorGameType.DynamicBlock":
                                    {
                                        objAtt.BeginRead();

                                        Guid id;
                                        objAtt.Read(out id);
                                        string initTypeStr = "";
                                        objAtt.Read(out initTypeStr);
                                        System.Type initType = null;
                                        if (initTypeStr.Contains("|"))
                                        {
                                            var splits = initTypeStr.Split('|');
                                            var assem = System.Reflection.Assembly.Load(splits[0]);
                                            initType = assem.GetType(splits[1]);
                                        }
                                        else
                                        {
                                            initType = CSUtility.Program.GetTypeFromSaveString(initTypeStr);
                                        }
                                        if (initType == null)
                                        {
                                            initType = typeof(CSUtility.Map.DynamicBlock.DynamicBlockActorInit);
                                        }
                                        var dyInit = (CSUtility.Map.DynamicBlock.DynamicBlockActorInit)System.Activator.CreateInstance(initType);
                                        dyInit.Read(objAtt);
                                        dyInit.DynamicBlockData.Id = id;
                                        assistData.DynamicBlockDatas.Add(dyInit.DynamicBlockData);

                                        objAtt.EndRead();
                                    }
                                    break;
                            }
                        }
                        catch (System.Exception)
                        {

                        }
                    }
                }

            }
        }
    }
}
