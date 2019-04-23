using System;
using System.Collections.Generic;
using System.IO;

namespace CSUtility.Data
{
    public abstract class RoleTemplateInitFactory
    {
        public static RoleTemplateInitFactory Instance;
        public abstract RoleTemplateBase CreateRoleTemplate();
    }
    public class RoleTemplateManager
    {
        static RoleTemplateManager smInstance = new RoleTemplateManager();
        public static RoleTemplateManager Instance
        {
            get { return smInstance; }
        }

        private RoleTemplateManager()
        {
            LoadFileDictionary();
        }
        // 记录所有的动作名供编辑器使用
        public List<string> ActionNameList
        {
            get;
        } = new List<string>();

        CSUtility.Support.ConcurentObjManager<UInt16, RoleTemplateBase> mRoles = new Support.ConcurentObjManager<ushort, RoleTemplateBase>();
        CSUtility.Support.ConcurentObjManager<UInt16, string> mRoleFileNames = new Support.ConcurentObjManager<ushort, string>();
        public CSUtility.Support.ConcurentObjManager<UInt16, string> RoleFileNames
        {
            get { return mRoleFileNames; }
        }

        /// <summary>
        /// 读取所有的已知文件位置的角色模板
        /// </summary>
        /// <returns>角色模板列表</returns>
        public List<RoleTemplateBase> LoadAllRoleTemplate()
        {
            List<RoleTemplateBase> newList = new List<RoleTemplateBase>();
            mRoleFileNames.For_Each((UInt16 key, string value, object argObj) =>
            {
                var roleTemplate = RoleTemplateInitFactory.Instance.CreateRoleTemplate();
                if (roleTemplate == null)
                {
                    roleTemplate = new RoleTemplateBase();
                }

                if (false == CSUtility.Support.IConfigurator.FillProperty(roleTemplate, value))
                    return CSUtility.Support.EForEachResult.FER_Continue;

                newList.Add(roleTemplate);
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            return newList;
        }

        string mResDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;
        public RoleTemplateBase FindRoleTemplate(UInt16 id)
        {
            if (mRoleFileNames.ContainsKey(id))
            {
                var file = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mRoleFileNames[id]);
                if (!System.IO.File.Exists(file))
                {
                    RemoveRoleFile(id);
                }
                else
                {
                    return LoadRoleTemplate(mRoleFileNames[id]);
                }
            }

            var files = System.IO.Directory.GetFiles(mResDir, id.ToString() + CSUtility.Support.IFileConfig.RoleTemplateExtension, SearchOption.AllDirectories);
            if (files.Length == 0)
                return null;

            return LoadRoleTemplate(files[0]);                        
        }

        /// <summary>
        /// 读取角色模板
        /// </summary>
        /// <param name="file">角色模板文件</param>
        /// <param name="force">是否强制读取</param>
        /// <returns></returns>
        public RoleTemplateBase LoadRoleTemplate(string file, bool force = false)
        {
            var id = RoleTemplateManager.GetIdFromFile(file);

            RoleTemplateBase roleTemplate;
            if(!force)
            {
                roleTemplate = mRoles.FindObj(id);
                if (roleTemplate != null)
                    return roleTemplate;
            }

            if (RoleTemplateInitFactory.Instance == null)
                return null;
            roleTemplate = RoleTemplateInitFactory.Instance.CreateRoleTemplate();
            if (roleTemplate == null)
            {
                roleTemplate = new RoleTemplateBase();
            }

            if (false == CSUtility.Support.IConfigurator.FillProperty(roleTemplate, file))
                return null;

            roleTemplate.IsDirty = false;

            mRoles.Add(id, roleTemplate);
            SetRoleFile(id,file);

            return roleTemplate;
        }

        public RoleTemplateBase CreateRoleTemplate(UInt16 id,string absFolder)
        {
            var roleTemplate = mRoles.FindObj(id);
            if (roleTemplate != null)
                return roleTemplate;
                      
            roleTemplate = RoleTemplateInitFactory.Instance?.CreateRoleTemplate();
            if (roleTemplate == null)
            {
                roleTemplate = new RoleTemplateBase();
            }
   
            roleTemplate.Id = id;

            var fileName = absFolder + "/" + id.ToString() + CSUtility.Support.IFileConfig.RoleTemplateExtension;

            mRoles.Add(id, roleTemplate);
            SetRoleFile(id,fileName);

            SaveRoleTemplate(id);

            return roleTemplate;
        }

        public static UInt16 GetIdFromFile(string fileName)
        {
            var exten = CSUtility.Support.IFileManager.Instance.GetFileExtension(fileName);
            if (("." + exten) != CSUtility.Support.IFileConfig.RoleTemplateExtension)
                return 0;

            fileName = fileName.Replace('\\', '/');

            var idStartIdx = fileName.LastIndexOf("/");
            var idEndIdx = fileName.LastIndexOf(".");
            var idStr = fileName.Substring(idStartIdx + 1, idEndIdx - idStartIdx - 1);
            
            UInt16 id = Convert.ToUInt16(idStr);

            return id;
        }

        public void SaveRoleTemplate(UInt16 Id)
        {
            var role = mRoles.FindObj(Id);
            if (role == null)
                return;

            var path = mRoleFileNames.FindObj(Id);
            if (path == null)
                return;

            role.Version++;
                        
            if (CSUtility.Support.IConfigurator.SaveProperty(role, "RoleTemplate", path))
            {
                role.IsDirty = false;
            }
        }

        string mFileName = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + "RoleFileDic";

        public void LoadFileDictionary()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(mFileName);
            if (xmlHolder == null)
                return;

            var nodes = xmlHolder.RootNode.FindNodes("FileNode");
            for (int i = 0; i < nodes.Count; i++)
            {
                UInt16 key = 0;
                string file = "";

                var att = nodes[i].FindAttrib("key");
                if (att != null)
                    key = Convert.ToUInt16(att.Value);

                att = nodes[i].FindAttrib("File");
                if (att != null)
                    file = att.Value;

                mRoleFileNames[key] = file;
            }
        }

        public void ReLoadFileDictionary()
        {
            mRoleFileNames.Clear();
            var files = System.IO.Directory.GetFiles(mResDir, "*" + CSUtility.Support.IFileConfig.RoleTemplateExtension, System.IO.SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var id = GetIdFromFile(file);
                var name = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
                mRoleFileNames[id] = name;
            }

            SaveFileDictionary();
        }
        public void ClearFileDictionary()
        {
            mRoleFileNames.Clear();
        }
        public void SaveFileDictionary()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FileDic", "");

            mRoleFileNames.For_Each((UInt16 key, string value, object argObj) =>
            {
                var node = xmlHolder.RootNode.AddNode("FileNode", "", xmlHolder);
                node.AddAttrib("key", key.ToString());
                node.AddAttrib("File", value);

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);


            CSUtility.Support.XmlHolder.SaveXML(mFileName, xmlHolder, true);
        }

        public void RemoveRoleFile(UInt16 id)
        {
            mRoles.Remove(id);
            mRoleFileNames.Remove(id);
            SaveFileDictionary();
        }

        public void SetRoleFile(UInt16 id, string name)
        {
            if (System.IO.Path.IsPathRooted(name))
                name = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(name);
            mRoleFileNames[id] = name;
            SaveFileDictionary();
        }
    }
}
