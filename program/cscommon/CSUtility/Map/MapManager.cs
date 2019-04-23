using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.Map
{
    public class MapManager
    {
        static MapManager mInstance = new MapManager();
        static public MapManager Instance
        {
            get { return mInstance; }
        }
        private MapManager()
        {
            LoadFileDictionary();
        }

        Dictionary<Guid, string> mMapFileDic = new Dictionary<Guid, string>();
        public Dictionary<Guid, string> MapFileDic
        {
            get { return mMapFileDic; } 
        }

        string mFileName = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + "MapFileDic";
        public void LoadFileDictionary()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(mFileName);
            if (xmlHolder == null)
                return;

            var nodes = xmlHolder.RootNode.FindNodes("FileNode");
            for (int i = 0; i < nodes.Count; i++)
            {
                Guid key = Guid.Empty;
                string file = "";

                var att = nodes[i].FindAttrib("key");
                if (att != null)
                    key = CSUtility.Support.IHelper.GuidParse(att.Value);

                att = nodes[i].FindAttrib("File");
                if (att != null)
                    file = att.Value;

                mMapFileDic[key] = file;
            }
        }
        public void SaveFileDictionary()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FileDic", "");

            foreach(var i in mMapFileDic)
            {
                var node = xmlHolder.RootNode.AddNode("FileNode", "", xmlHolder);
                node.AddAttrib("key", i.Key.ToString());
                node.AddAttrib("File", i.Value);
            }            
            CSUtility.Support.XmlHolder.SaveXML(mFileName, xmlHolder, true);
        }

        public void RemoveMapFile(Guid id)
        {
            mMapFileDic.Remove(id);
            SaveFileDictionary();
        }

        public void SetMapFile(Guid id, string name)
        {
            if (System.IO.Path.IsPathRooted(name))
                name = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(name);
            mMapFileDic[id] = name;
            SaveFileDictionary();
        }

        public string GetMapPath(Guid id)
        {
            if (mMapFileDic.ContainsKey(id))
            {
                string dir = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mMapFileDic[id]);
                if (!System.IO.Directory.Exists(dir))
                {
                    RemoveMapFile(id);
                }
                else
                {
                    return CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mMapFileDic[id]);
                }
            }

            var dirs = System.IO.Directory.GetDirectories(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory, id.ToString(), System.IO.SearchOption.AllDirectories);
            if (dirs.Length == 0)
            {                
                return "";
            }

            var path = dirs[0];
            SetMapFile(id,path);
            return path;
        }
    }
}
