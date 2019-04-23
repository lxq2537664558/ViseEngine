using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.Map
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class MapTemplate
    {
        public float CellSize { get; set; } = 4.0F;        

        Guid mMapGuid = Guid.Empty;
        public Guid MapGuid
        {
            get { return mMapGuid; }
        }

        List<GameData.Role.MonsterData> mMonsters { get; set; } = new List<GameData.Role.MonsterData>();
        public GameData.Role.MonsterData[] Monsters
        {
            get
            {
                lock (mTriggers)
                {
                    return mMonsters.ToArray();
                }
            }
        }

        List<CSUtility.Map.Trigger.TriggerData> mTriggers = new List<CSUtility.Map.Trigger.TriggerData>();
        public CSUtility.Map.Trigger.TriggerData[] Triggers
        {
            get
            {
                lock (mTriggers)
                {
                    return mTriggers.ToArray();
                }
            }
        }

        public MapTemplate(Guid mapId)
        {
            mMapGuid = mapId;
        }        

        // 高度图数据
        CSUtility.ServerMap.ServerAltitudeDataWrapper mMapAltitudeDataWrapper = null;
        public CSUtility.ServerMap.ServerAltitudeDataWrapper MapAltitudeDataWrapper
        {
            get { return mMapAltitudeDataWrapper; }
        }

        public bool LoadMap()
        {
            mTriggers.Clear();

            //LoadMapData();
            LoadMapSpecialData(CSUtility.Component.EActorGameType.Trigger);
            LoadMapSpecialData(CSUtility.Component.EActorGameType.NpcInitializer);

            mNavData = Map.MapPathManager.Instance.GetGlobalMapNavigationAssistData(mMapGuid)?.NavigationTileData;
            return true;
        }

        CSUtility.Navigation.INavigationDataWrapper mNavData;
        public void SetDynamicNavData(float x, float z, float radius, bool block)
        {
            if (mNavData != null)
            {
                mNavData.SetDynamicNavData(mMapGuid, x, z, radius, block);                
            }
        }

        public bool HasBarrier(float inStartX, float inStartZ, float inEndX, float inEndZ)
        {
            return mNavigation.HasBarrier(mMapGuid,inStartX,inStartZ,inEndX,inEndZ, mNavData);
        }

        CSUtility.Navigation.INavigationWrapper mNavigation = new CSUtility.Navigation.INavigationWrapper();
        bool LoadMapData()
        {
            var floder = CSUtility.Map.MapManager.Instance.GetMapPath(mMapGuid);
            var sceneFile = floder + "\\" + CSUtility.Map.WorldInit.ServerSceneGraphFileName;
            var sceneDir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(sceneFile);
            sceneDir = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(sceneDir);

            var xndHolder = CSUtility.Support.XndHolder.LoadXND(sceneFile);
            if (xndHolder == null)
                return false;

            var headerAtt = xndHolder.Node.FindAttrib("Header");
            if (headerAtt == null)
                return false;

            UInt32 levelXCount = 0, levelZCount = 0;
            UInt32 patchPerLevelX = 0, patchPerLevelZ = 0;

            headerAtt.BeginRead();
            headerAtt.Read(out levelXCount);
            headerAtt.Read(out levelZCount);
            headerAtt.Read(out patchPerLevelX);
            headerAtt.Read(out patchPerLevelZ);
            headerAtt.EndRead();

            var levelAtt = xndHolder.Node.FindAttrib("Levels");
            levelAtt.BeginRead();
            byte levelState;
            for (UInt32 x = 0; x < levelXCount; x++)
            {
                for (UInt32 z = 0; z < levelZCount; z++)
                {
                    levelAtt.Read(out levelState);

                    if (levelState > 0)
                    {
//                         // 读取Level中的数据
//                         var levelName = sceneDir + x + "_" + z + ".NPC.level";
//                         LoadMapLevel(levelName);

                    }
                }
            }
            levelAtt.EndRead();

            return true;
        }

        void LoadMapSpecialData(CSUtility.Component.EActorGameType type)
        {
            var floder = CSUtility.Map.MapManager.Instance.GetMapPath(mMapGuid);
            var sceneDir = floder + "/" + type.ToString();
            sceneDir = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(sceneDir);

            if (!System.IO.Directory.Exists(sceneDir))
                return;
            var files = System.IO.Directory.GetFiles(sceneDir);
            foreach(var i in files)
            {
                LoadMapLevel(i,type);
            }            
        }

        void LoadMapLevel(string levelFile, CSUtility.Component.EActorGameType type)
        {
            var levelHolder = CSUtility.Support.XndHolder.LoadXND(levelFile);
            if (levelHolder == null)
                return;

            var patchsNode = levelHolder.Node.FindNode("Patch");
            foreach (var node in patchsNode.GetNodes())
            {
                CSUtility.Support.XndNode objNode = null;
                switch(type)
                {
                    case CSUtility.Component.EActorGameType.Trigger:
                        objNode = node.FindNode("TileObjects");
                        break;
                    case CSUtility.Component.EActorGameType.NpcInitializer:
                        objNode = node.FindNode("DynamicOriTileObjects");
                        break;
                }
                
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
                                // Trigger
                                case "cscommon@CCore.World.TriggerActor":
                                    {
                                        objAtt.BeginRead();

                                        Guid id;
                                        objAtt.Read(out id);
                                        string initTypeStr = "";
                                        objAtt.Read(out initTypeStr);
                                        System.Type initType = null;
                                        if (initTypeStr.Contains('@'))
                                        {
                                            var splits = initTypeStr.Split('@');
                                            var assem = CSUtility.Program.GetAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, splits[0]);
                                            initType = assem.GetType(splits[1]);
                                        }
                                        else
                                        {
                                            initType = CSUtility.Program.GetTypeFromSaveString(initTypeStr);
                                        }
                                        if (initType == null)
                                        {
                                            initType = typeof(CSUtility.Map.Trigger.TriggerActorInit);
                                        }
                                        var triggerInit = (CSUtility.Map.Trigger.TriggerActorInit)System.Activator.CreateInstance(initType);
                                        triggerInit.Read(objAtt);
                                        if (triggerInit.ForServer)
                                        {
                                            lock (mTriggers)
                                            {
                                                bool bExist = false;
                                                foreach(var i in mTriggers)
                                                {
                                                    if (i.Id == triggerInit.TriggerData.Id)
                                                        bExist = true;
                                                }
                                                if (!bExist)
                                                    mTriggers.Add(triggerInit.TriggerData);
                                            }
                                        }

                                        objAtt.EndRead();
                                    }
                                    break;
                                case "cscommon@CCore.World.Role.NPCInitializerActor":
                                    {
                                        objAtt.BeginRead();

                                        Guid id;
                                        objAtt.Read(out id);
                                        string initTypeStr = "";
                                        objAtt.Read(out initTypeStr);
                                        System.Type initType = null;
                                        if (initTypeStr.Contains('@'))
                                        {
                                            var splits = initTypeStr.Split('@');
                                            var assem = CSUtility.Program.GetAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, splits[0]);
                                            initType = assem.GetType(splits[1]);
                                        }
                                        else
                                        {
                                            initType = CSUtility.Program.GetTypeFromSaveString(initTypeStr);
                                        }
                                        if (initType == null)
                                        {
                                            initType = typeof(CSUtility.Map.Trigger.TriggerActorInit);
                                        }
                                        var npcInitializerInit = (CSUtility.Map.Role.NPCInitializerActorInit)System.Activator.CreateInstance(initType);
                                        npcInitializerInit.Read(objAtt);

                                        var data = npcInitializerInit.NPCData as GameData.Role.MonsterData;
                                        if (data != null)
                                        {
                                            lock (mMonsters)
                                            {
                                                bool bExist = false;
                                                foreach (var i in mMonsters)
                                                {
                                                    if (i.RoleId == data.RoleId)
                                                        bExist = true;
                                                }

                                                if (!bExist)
                                                    mMonsters.Add(data);
                                            }
                                        }
                                        objAtt.EndRead();
                                    }
                                    break;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.FileLog.WriteLine(ex.ToString());
                            Log.FileLog.WriteLine(ex.StackTrace.ToString());
                        }
                    }
                }
            }
        }

        void InitMapAltitude()
        {
            var mapDir = CSUtility.Map.MapManager.Instance.GetMapPath(mMapGuid);
            var saDir = mapDir + "\\" + CSUtility.Map.WorldInit.ServerAltitudeFileName;
            var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(saDir);
            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(saDir);

            mMapAltitudeDataWrapper = new CSUtility.ServerMap.ServerAltitudeDataWrapper();
            mMapAltitudeDataWrapper.Load(file, path);
        }
    }
}
