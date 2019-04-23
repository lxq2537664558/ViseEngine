using System;
using System.Collections.Generic;

namespace UISystem.Template
{
    public class ControlTemplateInfo
    {
        ControlTemplate mControlTemplate;
        public ControlTemplate ControlTemplate
        {
            get { return mControlTemplate; }
        }
        string mFileName = "";
        public string FileName
        {
            get { return mFileName; }
        }
        
        public ControlTemplateInfo(ControlTemplate template, string fileName)
        {
            mControlTemplate = template;
            mFileName = fileName;
        }
    }

    public class TemplateMananger
    {
        static TemplateMananger mInstance = new TemplateMananger();
        public static TemplateMananger Instance
        {
            get { return mInstance; }
        }

        public void Cleanup()
        {
            mTemplateIdToFileDictionary.Clear();
            mControlTemplateDictionary.Clear();
        }

        protected Dictionary<Guid, string> mTemplateIdToFileDictionary = new Dictionary<Guid, string>();
        public Dictionary<Guid, string> TemplateIdToFileDictionary
        {
            get { return mTemplateIdToFileDictionary; }
        }

        protected Dictionary<Guid, ControlTemplateInfo> mControlTemplateDictionary = new Dictionary<Guid, ControlTemplateInfo>();
        public Dictionary<Guid, ControlTemplateInfo> ControlTemplateDictionary
        {
            get { return mControlTemplateDictionary; }
        }

        public TemplateMananger()
        {
            //LoadAllTemplates();
            LoadTemplateIDToFileDictionary();
        }

        public string GetFileFromId(Guid id)
        {
            string retValue = "";
            mTemplateIdToFileDictionary.TryGetValue(id, out retValue);
            return retValue;
        }

        public void ReloadAllTemplates()
        {
            ControlTemplateDictionary.Clear();
            LoadTemplateIDToFileDictionary();

            foreach (var file in mTemplateIdToFileDictionary)
            {
                FindControlTemplate(file.Key);
            }
        }

        private void LoadTemplateIDToFileDictionary()
        {
            mTemplateIdToFileDictionary.Clear();

            var templateDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultUITemplateDirectory;
            if(!System.IO.Directory.Exists(templateDir))
            {
                System.IO.Directory.CreateDirectory(templateDir);
            }
            LoadTemplateIdToFileDictionaryInDir(templateDir);
        }
        private void LoadTemplateIdToFileDictionaryInDir(string directory)
        {
            foreach (var file in System.IO.Directory.GetFiles(directory))
            {
                if (file.Substring(file.LastIndexOf('.')) != ".xml")
                    continue;

                var fileTemp = file.Replace("\\", "/");
                fileTemp = fileTemp.Replace(CSUtility.Support.IFileManager.Instance.Root, "");
                
                CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.LoadXML(fileTemp);
                var idAtt = xmlHolder.RootNode.FindAttrib("Id");
                if (idAtt == null)
                    continue;

                Guid id = CSUtility.Support.IHelper.GuidTryParse(idAtt.Value);
                mTemplateIdToFileDictionary[id] = fileTemp;
            }

            foreach (var dir in System.IO.Directory.GetDirectories(directory))
            {
                LoadTemplateIdToFileDictionaryInDir(dir);
            }
        }
        
        public ControlTemplateInfo FindControlTemplate(Guid id)
        {
            ControlTemplateInfo template;
            if (mControlTemplateDictionary.TryGetValue(id, out template))
                return template;

            string tempFile;
            if (!mTemplateIdToFileDictionary.TryGetValue(id, out tempFile))
                return null;

            var rootControl = WinBase.CreateFromXml(tempFile);
            if (!(rootControl is ControlTemplate))
                return null;

            ControlTemplateInfo info = new ControlTemplateInfo(rootControl as ControlTemplate, tempFile);
            mControlTemplateDictionary[rootControl.Id] = info;

            return info;
        }

        //public void LoadAllTemplates()
        //{
        //    mControlTemplateDictionary.Clear();

        //    var templateDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultUITemplateDirectory;
        //    LoadTemplatesInDir(templateDir);
        //}

        //private void LoadTemplatesInDir(string directory)
        //{
        //    foreach (var file in System.IO.Directory.EnumerateFiles(directory))
        //    {
        //        if (file.Substring(file.LastIndexOf('.')) != ".xml")
        //            continue;

        //        var fileTemp = file.Replace("\\", "/");
        //        fileTemp = fileTemp.Replace(CSUtility.Support.IFileManager.Instance.Root, "");
        //        var rootControl = WinBase.CreateFromXml(fileTemp);
        //        if (!(rootControl is ControlTemplate))
        //            continue;

        //        ControlTemplateInfo info = new ControlTemplateInfo(rootControl as ControlTemplate, fileTemp);
        //        mControlTemplateDictionary[rootControl.Id] = info;
        //    }

        //    foreach (var dir in System.IO.Directory.EnumerateDirectories(directory))
        //    {
        //        LoadTemplatesInDir(dir);
        //    }
        //}
    }
}
