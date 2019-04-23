using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Hall.Map
{
    public class MapTemplateManager
    {
        static MapTemplateManager smInstance = new MapTemplateManager();
        public static MapTemplateManager Instance
        {
            get { return smInstance; }
        }

        #region MapInit
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

                var mapDir = CSUtility.Map.MapManager.Instance.GetMapPath(mapSourceId);
                if (string.IsNullOrEmpty(mapDir))
                    return null;

                var mapConfigFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mapDir + "\\Config.map");
                if (CSUtility.Support.IConfigurator.FillProperty(outInit, mapConfigFileName))
                    mMapInitDictionary[mapSourceId] = outInit;

                return outInit;
            }
        }

        #endregion

        Dictionary<Guid, MapTemplate> mMapTemplateDic = new Dictionary<Guid, MapTemplate>();
        public Dictionary<Guid, MapTemplate> MapTemplateDic
        {
            get { return mMapTemplateDic; }
        }

        public MapTemplate FindMapTemplate(Guid id, bool loadWhenNotFound = true)
        {
            lock (mMapTemplateDic)
            {
                MapTemplate retMapTemplate;
                if (mMapTemplateDic.TryGetValue(id, out retMapTemplate))
                    return retMapTemplate;

                if (loadWhenNotFound)
                {
                    return LoadMapTemplate(id);
                }
            }
            return null;
        }

        public MapTemplate LoadMapTemplate(Guid id, bool forceLoad = false)
        {
            lock (mMapTemplateDic)
            {
                MapTemplate temp;
                if (!forceLoad)
                {
                    if (mMapTemplateDic.TryGetValue(id, out temp))
                        return temp;
                }

                temp = new MapTemplate(id);
                if (temp.LoadMap())
                {
                    mMapTemplateDic[id] = temp;
                    return temp;
                }

                return null;
            }
        }
    }
}
