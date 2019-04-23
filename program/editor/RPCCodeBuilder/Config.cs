using System;
using System.Collections.Generic;

using System.Text;

namespace RPCCodeBuilder
{
    public class ProjectConfig
    {
        string m_projectName = "";
        public string ProjectName
        {
            get { return m_projectName; }
            set
            {
                if(Config.Instance.GetProject(value) != null)
                {
                    return;
                }
                m_projectName = value;
            }
        }

        string m_serverModuleName = "";        
        public string ServerModuleName
        {
            get { return m_serverModuleName; }
            set { m_serverModuleName = value; }
        }

        string m_clientModuleName = "";
        public string ClientModuleName
        {
            get { return m_clientModuleName; }
            set { m_clientModuleName = value; }
        }

        string m_ClientCallerModuleName = "";
        public string ClientCallerModuleName
        {
            get { return m_ClientCallerModuleName; }
            set { m_ClientCallerModuleName = value; }
        }

        string m_ServerCallerModuleName = "";
        public string ServerCallerModuleName
        {
            get { return m_ServerCallerModuleName; }
            set { m_ServerCallerModuleName = value; }
        }

        string m_ServerCaller = "";
        public string ServerCaller
        {
            get { return m_ServerCaller; }
            set { m_ServerCaller = value; }
        }

        string m_ServerCallee = "";
        public string ServerCallee
        {
            get { return m_ServerCallee; }
            set { m_ServerCallee = value; }
        }

        string m_ClientCaller = "";
        public string ClientCaller
        {
            get { return m_ClientCaller; }
            set { m_ClientCaller = value; }
        }

        string m_ClientCallee = "";
        public string ClientCallee
        {
            get { return m_ClientCallee; }
            set { m_ClientCallee = value; }
        } 

        public void Load(System.Xml.XmlElement element)
        {
            foreach (var property in this.GetType().GetProperties())
            {
                var nodes = element.GetElementsByTagName(property.Name);
                if (nodes != null && nodes.Count > 0)
                {
                    property.SetValue(this, nodes[0].InnerText, null);
                }
            }
        }

        public void Save(System.Xml.XmlElement element, System.Xml.XmlDocument doc)
        {
            foreach (var property in this.GetType().GetProperties())
            {
                var cElement = doc.CreateElement(property.Name);
                cElement.InnerText = property.GetValue(this, null).ToString();
                element.AppendChild(cElement);
            }
        }        
    }

    public class Config
    {
        List<ProjectConfig> m_projCofigList = new List<ProjectConfig>();
        public List<ProjectConfig> ProjConfigList
        {
            get { return m_projCofigList; }
        }
        public string ConfigFile = CSUtility.Support.IFileManager.Instance.Bin + "config/codebuild.xml";
        public ProjectConfig CurProjectConfig;

        static Config m_instance = new Config();
        public static Config Instance
        {
            get { return m_instance; }
        }

        public void Load(string fileName)
        {
            m_projCofigList.Clear();

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            try
            {
                xmlDoc.Load(fileName);
                foreach (System.Xml.XmlElement node in xmlDoc.DocumentElement.ChildNodes)
                {
                    ProjectConfig pgConfig = new ProjectConfig();
                    pgConfig.Load(node);
                    m_projCofigList.Add(pgConfig);
                }

                if (m_projCofigList.Count > 0)
                    CurProjectConfig = m_projCofigList[0];
                else
                    CurProjectConfig = null;
            }
            catch
            {

            }                        
        }

        public void Save(string fileName)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml("<ProjectConfig></ProjectConfig>");
            //xmlDoc.Load(mExePath + "/codebuild.xml");
            foreach (var pgConfig in m_projCofigList)
            {
                var element = xmlDoc.CreateElement(pgConfig.ProjectName);
                pgConfig.Save(element, xmlDoc);
                xmlDoc.DocumentElement.AppendChild(element);
            }
            xmlDoc.Save(fileName);
        }

        public void SetCurProject(string projName)
        {
            foreach (var proj in ProjConfigList)
            {
                if (proj.ProjectName == projName)
                {
                    CurProjectConfig = proj;
                    return;
                }
            }

            CurProjectConfig = null;
        }

        public ProjectConfig GetProject(string projName)
        {
            foreach (var proj in ProjConfigList)
            {
                if (proj.ProjectName == projName)
                {                    
                    return proj;
                }
            }

            return null;
        }
    }
}
