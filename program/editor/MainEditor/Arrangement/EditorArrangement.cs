using System;
using System.Collections.Generic;

namespace MainEditor.Arrangement
{
    public class ArrangementData
    {
        [CSUtility.Support.DataValueAttribute("Id")]
        public Guid Id
        {
            get;
            set;
        }

        [CSUtility.Support.DataValueAttribute("Name")]
        public string Name
        {
            get;
            set;
        }
    }

    public class EditorArrangementManager
    {
        static EditorArrangementManager mInstance = new EditorArrangementManager();
        public static EditorArrangementManager Instance
        {
            get { return mInstance; }
        }

        List<ArrangementData> mEditorArrangement = new List<ArrangementData>();
        [CSUtility.Support.DataValueAttribute("EditorArrangement")]
        public List<ArrangementData> EditorArrangement
        {
            get { return mEditorArrangement; }
            set { mEditorArrangement = value; }
        }

        string mArrangementConfigDir = CSUtility.Support.IFileConfig.EditorResourcePath + "\\EditorArrangements";
        
        private EditorArrangementManager()
        {
            LoadAll();            
        }

        public string GetFileName(Guid id)
        {
            return mArrangementConfigDir + "\\" + id.ToString() + ".xml";
        }

        public void LoadAll()
        {
            var dir = CSUtility.Support.IFileManager.Instance.Root + mArrangementConfigDir;
            if (!System.IO.Directory.Exists(dir))
                return;

            mEditorArrangement.Clear();
            CSUtility.Support.IConfigurator.FillProperty(this, mArrangementConfigDir + "\\Config.xml");
        }

        public void SaveConfig()
        {
            CSUtility.Support.IConfigurator.SaveProperty(this, "Config", mArrangementConfigDir + "\\Config.xml");
        }

        public void SaveArrangement(Guid id)
        {
            DockControl.DockManager.Instance.SaveLayoutConfig(Arrangement.EditorArrangementManager.Instance.GetFileName(id));
        }

        public bool ContainArrangement(Guid id)
        {
            bool bFind = false;
            foreach (var arrange in mEditorArrangement)
            {
                if (arrange.Id == id)
                {
                    bFind = true;
                    break;
                }
            }

            return bFind;
        }

        public void AddArrangement(Guid id, string name)
        {
            if (ContainArrangement(id))
                return;

            ArrangementData data = new ArrangementData()
            {
                Id = id,
                Name = name,
            };
            mEditorArrangement.Add(data);
            SaveConfig();
        }

        public void DeleteArrangement(Guid id)
        {
            if (!ContainArrangement(id))
                return;

            foreach (var arrange in mEditorArrangement)
            {
                if (arrange.Id == id)
                {
                    mEditorArrangement.Remove(arrange);
                    break;
                }
            }

            // 删除文件
            var fileName = GetFileName(id);;
            System.IO.File.Delete(CSUtility.Support.IFileManager.Instance.Root + fileName);

            SaveConfig();
        }
    }
}
