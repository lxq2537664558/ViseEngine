using System;
using System.Collections.Generic;

namespace CCore.Effect
{
    /// <summary>
    /// 粒子系统管理器
    /// </summary>
    public class EffectManager
    {
        static EffectManager smInstance = new EffectManager();
        /// <summary>
        /// 粒子系统管理器单例
        /// </summary>
        public static EffectManager Instance
        {
            get { return smInstance; }
        }
        /// <summary>
        /// 游戏结束时调用，释放该实例
        /// </summary>
        public static void FinalInstance()
        {
            smInstance = null;
        }

        // 存储的路径相对于Release
        Dictionary<Guid, string> mEffectFileNames = new Dictionary<Guid, string>();

        Dictionary<Guid, EffectTemplate> mEffects = new Dictionary<Guid, EffectTemplate>();
        /// <summary>
        /// 只读属性，粒子模板与ID
        /// </summary>
        public Dictionary<Guid, EffectTemplate> Effects
        {
            get { return mEffects; }
        }
        /// <summary>
        /// 根据粒子ID删除粒子模板
        /// </summary>
        /// <param name="id">粒子ID</param>
        public void DeleteEffectTemplate(Guid id)
        {
            mEffectFileNames.Remove(id);
            mEffects.Remove(id);
        }
        /// <summary>
        /// 设置粒子模板的ID和对应模板文件
        /// </summary>
        /// <param name="id">粒子模板</param>
        /// <param name="newFile">模板文件</param>
        public void SetEffectTemplateFile(Guid id, string newFile)
        {
            mEffectFileNames[id] = newFile;
        }
        /// <summary>
        /// 根据粒子ID得到粒子模板文件
        /// </summary>
        /// <param name="id">粒子ID</param>
        /// <returns>返回粒子模板文件名</returns>
        public string GetEffectTemplateFile(Guid id)
        {
            string retValue = "";
            mEffectFileNames.TryGetValue(id, out retValue);
            return retValue;
        }

        //public EffectTemplate FindEffectTemplate(Guid id, string folder, bool bFindInSubFolder = true)
        //{
        //    foreach (var file in System.IO.Directory.GetFiles(folder))
        //    {
        //        if (("." + CSUtility.Support.IFileManager.Instance.GetFileExtension(file)) != CSUtility.Support.IFileConfig.EffectExtension)
        //            continue;

        //        if (CSUtility.Program.GetIdFromFile(file) == id)
        //            return LoadEffectTemplate(file);
        //    }

        //    if (bFindInSubFolder)
        //    {
        //        foreach (var dir in System.IO.Directory.GetDirectories(folder))
        //        {
        //            var retData = FindEffectTemplate(id, dir, bFindInSubFolder);
        //            if (retData != null)
        //                return retData;
        //        }
        //    }

        //    return null;
        //}
        /// <summary>
        /// 根据粒子ID在文件中查找粒子模板
        /// </summary>
        /// <param name="id">粒子ID</param>
        /// <param name="bFindInFile">是否在文件中查找，若不在文件中查找则返回NULL</param>
        /// <returns>返回对应的粒子模板</returns>
        public EffectTemplate FindEffectTemplate(Guid id, bool bFindInFile = true)
        {
            if (id == Guid.Empty)
                return null;

            EffectTemplate data;
            if (mEffects.TryGetValue(id, out data))
                return data;

            if (!bFindInFile)
                return null;

            var sourceDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;
            foreach(var file in System.IO.Directory.GetFiles(sourceDir, id.ToString() + CSUtility.Support.IFileConfig.EffectExtension, System.IO.SearchOption.AllDirectories))
            {
                return LoadEffectTemplate(file);
            }
            return null;
        }
        /// <summary>
        /// 加载粒子模板
        /// </summary>
        /// <param name="filename">粒子模板的文件路径</param>
        /// <returns>粒子模板</returns>
        public EffectTemplate LoadEffectTemplate(string filename)
        {
            var id = CSUtility.Program.GetIdFromFile(filename);

            EffectTemplate data = null;
            if (mEffects.TryGetValue(id, out data))
                return data;

            filename = filename.Replace("/", "\\");
            filename = filename.Replace(CSUtility.Support.IFileManager.Instance.Root.Replace("/", "\\"), "");

            data = new EffectTemplate();
            var holder = CSUtility.Support.XndHolder.LoadXND(filename);
            if (holder == null)
                return null;
            if (data.Load(holder.Node) == false)
            {
                holder.Node.TryReleaseHolder();
                return null;
            }
            holder.Node.TryReleaseHolder();

            mEffects[id] = data;
            mEffectFileNames[id] = filename;
            data.IsDirty = false;
            return data;
        }
        /// <summary>
        /// 保存粒子模板
        /// </summary>
        /// <param name="id">粒子模板ID</param>
        /// <param name="verUpdate">是否更新版本，缺省为true</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool SaveEffectTemplate(Guid id, bool verUpdate = true)
        {
            var et = FindEffectTemplate(id);
            if (et == null)
                return false;

            var fileName = "";
            if (!mEffectFileNames.TryGetValue(id, out fileName))
                return false;

            if(verUpdate)
                et.Ver++;
            var holder = CSUtility.Support.XndHolder.NewXNDHolder();
            et.Save(holder.Node);
            et.IsDirty = false;
            CSUtility.Support.XndHolder.SaveXND(fileName, holder);

            return true;
        }
        /// <summary>
        /// 根据ID存储粒子模板
        /// </summary>
        /// <param name="file">存储的文件名</param>
        /// <param name="id">粒子模板ID</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool SaveEffectTemplate(string file, Guid id)
        {
            var et = FindEffectTemplate(id);
            if (et == null)
            {
                et = new EffectTemplate();
                et.Id = id;
                mEffects[id] = et;
            }

            file = file.Replace("/", "\\");
            file = file.Replace(CSUtility.Support.IFileManager.Instance.Root.Replace("/", "\\"), "");

            et.Ver++;
            var holder = CSUtility.Support.XndHolder.NewXNDHolder();
            et.Save(holder.Node);
            et.IsDirty = false;
            mEffectFileNames[id] = file;
            CSUtility.Support.XndHolder.SaveXND(file, holder);

            return true;
        }
        /// <summary>
        /// 存储所有的粒子脏模板
        /// </summary>
        public void SaveAllDirtyEffectTemplate()
        {
            foreach (var effect in mEffects.Values)
            {
                if (effect.IsDirty)
                {
                    effect.Ver++;
                    string file = mEffectFileNames[effect.Id];
                    var holder = CSUtility.Support.XndHolder.NewXNDHolder();
                    effect.Save(holder.Node);
                    effect.IsDirty = false;
                    CSUtility.Support.XndHolder.SaveXND(file, holder);
                }
            }
        }
        /// <summary>
        /// 加载所有指定路径下的粒子模板
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void LoadAllEffectTemplate(string filePath)
        {
            if (filePath.Contains("/.svn") || filePath.Contains("\\.svn"))
                return;

            var files = System.IO.Directory.GetFiles(filePath);
            foreach (var file in files)
            {
                var idStartIdx = file.LastIndexOf("\\");
                var idEndIdx = file.LastIndexOf(".");
                var ext = file.Substring(idEndIdx);
                if (ext != CSUtility.Support.IFileConfig.EffectExtension)
                    continue;

                LoadEffectTemplate(file);
            }

            var dirs = System.IO.Directory.GetDirectories(filePath);
            foreach (var dir in dirs)
            {
                LoadAllEffectTemplate(dir);
            }
        }
        /// <summary>
        /// 加载粒子模板
        /// </summary>
        public void LoadAllEffectTemplate()
        {
            var sourceDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultEffectDirectory;
            LoadAllEffectTemplate();
        }

    }
}
