using System;
using System.Collections.Generic;

namespace CSUtility.Map
{
    public class ScenePointGroupManager : CSUtility.Support.XndSaveLoadProxy
    {
        // Editor 接口
        public delegate void Delegate_OnCurrentMapChanged(Guid mapId);
        public event Delegate_OnCurrentMapChanged OnCurrentMapChanged;
        public void _OnCurrentMapChanged(Guid mapId)
        {
            if (OnCurrentMapChanged != null)
                OnCurrentMapChanged(mapId);
        }

        static ScenePointGroupManager smInstance = new ScenePointGroupManager();
        public static ScenePointGroupManager Instance
        {
            get { return smInstance; }
        }

        //// 路径相对于Release
        //string mCurrentMapDir = "";
        //public string CurrentMapDir
        //{
        //    get { return mCurrentMapDir; }
        //    set
        //    {
        //        mCurrentMapDir = value;

        //        if (OnCurrentMapChanged != null)
        //            OnCurrentMapChanged();
        //    }
        //}

        Dictionary<Guid, Dictionary<Guid, ScenePointGroup>> mMapGroupDic = new Dictionary<Guid, Dictionary<Guid, ScenePointGroup>>();

        //public string Direction
        //{
        //    get { return CurrentMapDir + "ScenePoint\\"; }
        //}

        public static void FinalInstance()
        {
            smInstance = null;
        }

        public string GetDirectionWithMapId(Guid mapId)
        {
            return CSUtility.Support.IFileConfig.MapDirectory + "\\" + mapId + "\\ScenePoint\\";
        }
        public string GetGroupFileName(Guid mapId, Guid groupId)
        {
            return GetDirectionWithMapId(mapId) + groupId.ToString() + ".sp";
        }

        public ScenePointGroup FourceLoadGroup(Guid mapId, Guid groupId)
        {
            if (mapId == Guid.Empty || groupId == Guid.Empty)
                return null;

            return LoadGroup(mapId, groupId, true);
        }

        public ScenePointGroup FindGroup(Guid mapId, Guid groupId)
        {
            if (mapId == Guid.Empty || groupId == Guid.Empty)
                return null;

            Dictionary<Guid, ScenePointGroup> sGroup;
            if (mMapGroupDic.TryGetValue(mapId, out sGroup))
            {
                ScenePointGroup retGroup;
                if (sGroup.TryGetValue(groupId, out retGroup))
                    return retGroup;
            }

            var gp = LoadGroup(mapId, groupId);

            return gp;
        }

        public List<ScenePointGroup> LoadAllGroups(Guid mapId)
        {
            List<ScenePointGroup> retGroups = new List<ScenePointGroup>();
            if (mapId == Guid.Empty)
                return retGroups;

            Dictionary<Guid, ScenePointGroup> sGroup;
            if (!mMapGroupDic.TryGetValue(mapId, out sGroup))
            {
                sGroup = new Dictionary<Guid, ScenePointGroup>();
                mMapGroupDic[mapId] = sGroup;
            }

            var dir = GetDirectionWithMapId(mapId);
            var absDir = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(dir);
            if (!System.IO.Directory.Exists(absDir))
                return retGroups;

            var allFiles = System.IO.Directory.GetFiles(absDir);
            foreach (var file in allFiles)
            //foreach (var file in System.IO.Directory.EnumerateFiles(absDir))
            {
                var fileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(file);
                var pos = fileName.LastIndexOf('.');
                fileName = fileName.Substring(0, pos);
                Guid tempId = new Guid(fileName);
                //System.Guid.TryParse(fileName, out tempId);

                var group = LoadGroup(mapId, tempId);
                if (group != null)
                {
                    retGroups.Add(group);
                    sGroup[tempId] = group;
                }
            }

            return retGroups;
        }

        private ScenePointGroup LoadGroup(Guid mapId, Guid groupId, bool bForceLoad = false)
        {
            if (mapId == Guid.Empty || groupId == Guid.Empty)
                return null;

            Dictionary<Guid, ScenePointGroup> sGroup;
            if (!mMapGroupDic.TryGetValue(mapId, out sGroup))
            {
                sGroup = new Dictionary<Guid, ScenePointGroup>();
                mMapGroupDic[mapId] = sGroup;
            }

            ScenePointGroup group = null;
            ScenePointGroup tempGroup = null;
            if (sGroup.TryGetValue(groupId, out group))
            {
                if(!bForceLoad)
                    return group;
            }

            var dir = GetDirectionWithMapId(mapId);

            var xndHolder = CSUtility.Support.XndHolder.LoadXND(dir + groupId.ToString() + ".sp");
            if (xndHolder == null || xndHolder.Node == null)
                return null;
            
            var att = xndHolder.Node.FindAttrib("Type");
            if (att != null)
            {
                att.BeginRead();

                string typeString;
                att.Read(out typeString);

                var type = CSUtility.Program.GetTypeFromSaveString(typeString);
                if (type != null)
                {
                    tempGroup = System.Activator.CreateInstance(type) as ScenePointGroup;
                }
                else
                    tempGroup = new ScenePointGroup();

                att.EndRead();
            }
            else
                tempGroup = new ScenePointGroup();

            if (group == null || tempGroup.GetType() != group.GetType())
            {
                sGroup[groupId] = tempGroup;
                group = tempGroup;
            }

            att = xndHolder.Node.FindAttrib("Datas");
            att.BeginRead();
            group.Read(att);
            att.EndRead();

            xndHolder.Node.TryReleaseHolder();

            return group;
        }
        //public ScenePointGroup LoadGroup(string file)
        //{
        //    var ext = CSUtility.Support.IFileManager.Instance.GetFileExtension(file);
        //    if (ext != "sp")
        //        return null;
        //    file = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
        //    var xndHolder = CSUtility.Support.IXndHolder.LoadXND(file, false);
        //    ScenePointGroup group = new ScenePointGroup();
        //    var att = xndHolder.Node.FindAttrib("Datas");
        //    att.BeginRead();
        //    group.Read(att);
        //    att.EndRead();

        //    return group;
        //}

        public bool AddGroup(ScenePointGroup group, Guid mapId)
        {
            if (group == null)
                return false;

            Dictionary<Guid, ScenePointGroup> sGroup;
            if (!mMapGroupDic.TryGetValue(mapId, out sGroup))
            {
                sGroup = new Dictionary<Guid, ScenePointGroup>();
                mMapGroupDic[mapId] = sGroup;
            }

            group.IsDirty = true;

            sGroup[group.Id] = group;

            return true;
        }

        public void DelGroup(Guid mapId, Guid groupId)
        {
            Dictionary<Guid, ScenePointGroup> sGroup;
            if (mMapGroupDic.TryGetValue(mapId, out sGroup))
            {
                sGroup.Remove(groupId);
            }

            var dir = GetDirectionWithMapId(mapId);

            var file = dir + groupId.ToString() + ".sp";
            file = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(file);
            if (!System.IO.File.Exists(file))
                return;

            System.IO.File.Delete(file);
        }

        public List<KeyValuePair<Guid, Guid>> SaveDirtyGroups()
        {
            List<KeyValuePair<Guid, Guid>> retValues = new List<KeyValuePair<Guid, Guid>>();

            foreach (var dicData in mMapGroupDic)
            {
                var dir = GetDirectionWithMapId(dicData.Key);

                var absDir = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(dir);
                if (!System.IO.Directory.Exists(absDir))
                {
                    System.IO.Directory.CreateDirectory(absDir);
                }

                foreach (var group in dicData.Value.Values)
                {
                    if (group.IsDirty)
                    {
                        group.Ver++;

                        var xndHolder = CSUtility.Support.XndHolder.NewXNDHolder();
                        var att = xndHolder.Node.AddAttrib("Type");
                        var groupType = group.GetType();
                        att.BeginWrite();
                        string typeStr = CSUtility.Program.GetTypeSaveString(groupType);
                        att.Write(typeStr);
                        att.EndWrite();
                        att = xndHolder.Node.AddAttrib("Datas");
                        att.BeginWrite();
                        group.Write(att);
                        att.EndWrite();

                        var file = dir + group.Id.ToString() + ".sp";
                        CSUtility.Support.XndHolder.SaveXND(file, xndHolder);

                        group.IsDirty = false;

                        retValues.Add(new KeyValuePair<Guid, Guid>(dicData.Key, group.Id));

                        xndHolder.Node.TryReleaseHolder();
                    }
                }

            }

            return retValues;
        }
    }
}
