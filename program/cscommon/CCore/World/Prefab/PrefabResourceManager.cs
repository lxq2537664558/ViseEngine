using System;
using System.Collections.Generic;

namespace CCore.World.Prefab
{
    /// <summary>
    /// 预制件资源的管理类
    /// </summary>
    public class PrefabResourceManager
    {
        static PrefabResourceManager smInstance = new PrefabResourceManager();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static PrefabResourceManager Instance
        {
            get { return smInstance; }
        }
        /// <summary>
        /// 释放该单例对象
        /// </summary>
        public static void FinalInstance()
        {
            smInstance = null;
        }

        Dictionary<Guid, PrefabResource> mPrefabResourceDic = new Dictionary<Guid, PrefabResource>();
        /// <summary>
        /// 根据ID查找预制资源
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>返回预制件资源对象</returns>
        public PrefabResource FindPrefabResource(Guid id)
        {
            PrefabResource res = null;
            if (mPrefabResourceDic.TryGetValue(id, out res))
                return res;

            var sourceDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultPrefabFolder;

            foreach (var file in System.IO.Directory.GetFiles(sourceDir))
            {
                var fileExt = CSUtility.Support.IFileManager.Instance.GetFileExtension(file);
                if (("." + fileExt) != CSUtility.Support.IFileConfig.PrefabResExtension)
                    continue;

                if (CSUtility.Program.GetIdFromFile(file) == id)
                    return LoadPrefabResource(file);
            }

            return null;
        }
        /// <summary>
        /// 加载相应文件名称的预制件资源
        /// </summary>
        /// <param name="fileName">文件路径名称</param>
        /// <returns>返回得到的预制资源</returns>
        public PrefabResource LoadPrefabResource(string fileName)
        {
            var id = CSUtility.Program.GetIdFromFile(fileName);
            PrefabResource res = null;
            if (mPrefabResourceDic.TryGetValue(id, out res))
                return res;

            fileName = fileName.Replace("\\", "/");
            fileName = fileName.Replace(CSUtility.Support.IFileManager.Instance.Root.Replace("\\", "/"), "");

            res = new PrefabResource();

            var holder = CSUtility.Support.XndHolder.LoadXND(fileName);
            if (holder == null || holder.Node == null)
                return null;

            var att = holder.Node.FindAttrib("Data");
            att.BeginRead();
            res.Read(att);
            att.EndRead();

            mPrefabResourceDic[res.Id] = res;

            holder.Node.TryReleaseHolder();

            return res;
        }
        /// <summary>
        /// 保存预制件资源
        /// </summary>
        /// <param name="res">预制件资源对象</param>
        public void SavePrefabResource(PrefabResource res)
        {
            var fileName = CSUtility.Support.IFileConfig.DefaultPrefabFolder + "\\" + res.Id.ToString() + CSUtility.Support.IFileConfig.PrefabResExtension;

            var holder = CSUtility.Support.XndHolder.NewXNDHolder();
            var att = holder.Node.AddAttrib("Data");
            att.BeginWrite();
            res.Write(att);
            att.EndWrite();

            CSUtility.Support.XndHolder.SaveXND(fileName, holder);
        }
        /// <summary>
        /// 根据ID删除预制件资源对象
        /// </summary>
        /// <param name="id">预制件资源对象的ID</param>
        public void RemovePrefabResource(Guid id)
        {
            mPrefabResourceDic.Remove(id);
        }
    }
}
