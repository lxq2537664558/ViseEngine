using System;
using System.Collections.Generic;

namespace CCore.Material
{
    /// <summary>
    /// 材质信息类
    /// </summary>
    public class MaterialMgr
    {
        /// <summary>
        /// Techniques文件夹的名称
        /// </summary>
        public static System.String TechniqueRootFolder = "Techniques";
        /// <summary>
        /// 材质信息的指针
        /// </summary>
        protected IntPtr mStandMaterialMgr;  // model3.vStandMaterialMgr
        /// <summary>
        /// 材质信息的info类
        /// </summary>
        public class stFileInfo
        {
            /// <summary>
            /// 材质信息的ID
            /// </summary>
            public System.Guid ownerId = Guid.Empty;
            /// <summary>
            /// 材质信息的文件
            /// </summary>
            public System.String file = "";
            //public UInt32 uniqueID;
        };

        List<Guid> mLostTechniques = new List<Guid>();
        //Dictionary<UInt32, stFileInfo> mFilesUniqueDictionary = new Dictionary<UInt32, stFileInfo>();

        //IMaterial LoadTechnique(System.Guid techId, System.String techFile);

        private void RemoveMaterial(System.String file)
        {
            unsafe
            {
                DllImportAPI.vStandMaterialMgr_RemoveMaterial(mStandMaterialMgr, file);
            }
        }
        //private IntPtr ForceLoadMaterial( System.String file , System.String tech )
        //{
        //    // return value model3.v3dStagedMaterialInstance*
        //    if( mStandMaterialMgr==IntPtr.Zero )
        //        return IntPtr.Zero;

        //    var name = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(file);
        //    pin_ptr<const System::Char> pChar = PtrToStringChars(name);
        //    REF<model3::vStandMaterial> m;

        //    REF<VRes2Memory> io;
        //    io.WeakRef( (VRes2Memory*)(VRes2Memory*)(CSUtility::Support::IFileManager::Instance->OpenFileForRead( "Shader/Material/"+file , CSUtility::EFileType::Material , false ).ToPointer() ));
        //    if(io==NULL)
        //        return NULL;
        //    m.WeakRef( mStandMaterialMgr->LoadMaterial( pChar , io ) );
        //    if( m==NULL )
        //    {
        //        return NULL;
        //    }

        //    v3dTechnique* t = NULL;
        //    //if(tech!=NULL)
        //    //	t = m->GetTechnique(tech);
        //    if(tech!=nullptr)
        //        t = m->GetTechnique(StringManage2Native(tech));

        //    model3::v3dStagedMaterialInstance* result = new model3::v3dStagedMaterialInstance();
        //    result->SetMaterial( m , t );

        //    return result;
        //}
        //private model3.v3dStagedMaterialInstance* LoadMaterial( System.String file , LPCTSTR tech )
        //{
        //    return NULL;
        //}
        ////////////private Material LoadMaterial(MaterialParameter mtl)
        ////////////      {
        ////////////          return LoadMaterial(mtl, false);
        ////////////      }
        ////////////public Material LoadMaterial(MaterialParameter mtl, bool bForceLoad)
        ////////////      {
        ////////////          unsafe
        ////////////          {
        ////////////              if (mStandMaterialMgr != IntPtr.Zero)
        ////////////                  return null;

        ////////////              lock(this)
        ////////////              {
        ////////////                  var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory("Shader/Material/" + mtl.Material, CSUtility.EFileType.Material, false);
        ////////////                  if (io == IntPtr.Zero)
        ////////////                      return null;

        ////////////                  var m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, mtl.Material, io, bForceLoad);
        ////////////                  CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);                    
        ////////////                  if (m != IntPtr.Zero)
        ////////////                      return null;

        ////////////                  var t = DllImportAPI.vStandMaterial_GetTechnique(m, mtl.Tech);
        ////////////                  var result = new Material();
        ////////////                  result._Initialize(m, t);

        ////////////                  DllImportAPI.v3dStagedMaterialBase_Release(m);

        ////////////                  return result;
        ////////////              }
        ////////////          }
        ////////////      }

        private IntPtr LoadMaterial(System.Guid id)
        {
            // return model3.vStandMaterial*;
            // 在这个阶段基本上已经找到路径了，所以这里直接取路径
            System.Guid matId = GetFileDictionaryOwnerId(id);
            System.String matFile = GetFileDictionaryFileValue(matId);
            System.String tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;

            var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
            if (io != IntPtr.Zero)
                return IntPtr.Zero;
            var retValue = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io, false);
            CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
            return retValue;
        }
        /// <summary>
        /// 构造函数创建实例对象
        /// </summary>
        public MaterialMgr()
        {
            mStandMaterialMgr = DllImportAPI.vStandMaterialMgr_Get(Engine.Instance.Client.Graphics.Device);
            LoadFileDictionary();
        }
        /// <summary>
        /// 析构函数，释放指针
        /// </summary>
        ~MaterialMgr()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除实例，释放指针
        /// </summary>
        public void Cleanup()
        {
            unsafe
            {
                if (mStandMaterialMgr != IntPtr.Zero)
                {
                    mStandMaterialMgr = IntPtr.Zero;
                }

                ClearFileDictionary();
                //mFilesUniqueDictionary.Clear();
            }
        }

        // 返回值相对于Resource路径
        /// <summary>
        /// 查找材质的Techniques文件
        /// </summary>
        /// <param name="id">材质ID</param>
        /// <param name="folder">材质的文件夹</param>
        /// <returns>返回值相对于Resource路径</returns>
        public System.String FindTechniqueFile(System.Guid id, System.String folder = "")
        {
            if (string.IsNullOrEmpty(folder))
                folder = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;

            if (System.IO.Directory.Exists(folder) == false)
                return "";

            var techName = id.ToString() + CSUtility.Support.IFileConfig.MaterialTechniqueExtension;
            var files = System.IO.Directory.GetFiles(folder, techName, System.IO.SearchOption.AllDirectories);
            if (files.Length == 0)
                return "";

            foreach (var file in files)
            {
                var fileInfo = new System.IO.FileInfo(file);
                if (fileInfo.Extension == CSUtility.Support.IFileConfig.MaterialTechniqueExtension)
                    return CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
            }

            return "";
        }
        /// <summary>
        /// 返回指定材质ID的路径(路径相对于Resource的路径)
        /// </summary>
        /// <param name="id">材质ID</param>
        /// <param name="folder">搜索目录(包含子目录)，如果为空则搜索resource目录</param>
        /// <returns>返回相对于Resource的材质路径</returns>
        public System.String FindMaterialFile(System.Guid id, System.String folder = "")
        {
            string retFile = "";

            if (string.IsNullOrEmpty(folder))
                folder = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;

            if (System.IO.Directory.Exists(folder) == false)
                return retFile;

            retFile = GetFileDictionaryFileValue(id);
            var matRelFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + retFile;
            if (!string.IsNullOrEmpty(retFile) && System.IO.File.Exists(matRelFile))
                return retFile;

            var matName = id.ToString() + CSUtility.Support.IFileConfig.MaterialExtension;
            var files = System.IO.Directory.GetFiles(folder, matName, System.IO.SearchOption.AllDirectories);
            if (files.Length == 0)
                return retFile;

            foreach (var file in files)
            {
                var fileInfo = new System.IO.FileInfo(file);
                if (fileInfo.Extension == CSUtility.Support.IFileConfig.MaterialExtension)
                {
                    retFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file, folder);
                    break;
                }
            }

            SetFileDictionaryFileValue(id, retFile);
            SaveFileDictionary();
            return retFile;

            //      foreach (System.String file in System.IO.Directory.GetFiles(folder))
            //{
            //          var fileInfo = new System.IO.FileInfo(file);
            //          if (fileInfo.Extension != CSUtility.Support.IFileConfig.MaterialExtension)
            //              continue;

            // if(file.Contains(matName))
            //  return CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
            //}

            //foreach (System.String dir in System.IO.Directory.GetDirectories(folder))
            //{
            // System.String findedFile = FindMaterialFile(id, dir);
            // if(!System.String.IsNullOrEmpty(findedFile))
            //  return findedFile;
            //}

            //return "";
        }

        // techFile为相对于material默认保存路径的路径

        //public IMaterial LoadTechnique(System.Guid id)
        //{
        //    return LoadTechnique(id, false);
        //}

        int TechSaveCount = 10;
        //public Material LoadTechnique(System.Guid id, bool bForceLoad = false)
        /// <summary>
        /// 加载technique参数
        /// </summary>
        /// <param name="id">材质ID</param>
        /// <param name="bForceLoad">是否强制从磁盘读取</param>
        /// <returns>返回指定ID的材质</returns>
        public Material LoadTechnique(System.Guid id, bool bForceLoad = false)
        {
            //DllImportAPI.V3DFontRenderParamList_New3();
            unsafe
            {
                lock (this)
                {
                    if (mStandMaterialMgr == IntPtr.Zero)
                        return null;

                    if (id == Guid.Empty)
                        return null;

                    if (mLostTechniques.Contains(id))
                        return null;

                    IntPtr m, t;

                    Guid matId = Guid.Empty;
                    string matFile = "";
                    string techFile = GetFileDictionaryFileValue(id);
                    string techAbsFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + techFile;

                    if (string.IsNullOrEmpty(techFile) || !System.IO.File.Exists(techAbsFile))
                    {
                        techFile = FindTechniqueFile(id, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                        techAbsFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + techFile;
                        if (!string.IsNullOrEmpty(techFile))
                        {
                            SetFileDictionaryFileValue(id, techFile);
                            matId = GetMatGuidFromTechFile(techAbsFile);
                            SetFileDictionaryOwnerIdValue(id, matId);
                            matFile = FindMaterialFile(matId, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);

                            TechSaveCount--;
                            if (TechSaveCount <= 0)
                            {
                                TechSaveCount = 20;
                                SaveFileDictionary();
                            }
                        }
                        else
                        {
                            if (!mLostTechniques.Contains(id))
                            {
                                System.Diagnostics.Trace.WriteLine(string.Format("Technique {0} 找不到", id));
                                mLostTechniques.Add(id);
                            }
                            return null;
                        }
                    }
                    else
                    {
                        matId = GetFileDictionaryOwnerId(id);
                        if (matId == Guid.Empty)
                        {
                            matId = GetMatGuidFromTechFile(techAbsFile);
                            SetFileDictionaryOwnerIdValue(id, matId);
                            SaveFileDictionary();
                        }
                        matFile = FindMaterialFile(matId, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                    }

                    if (bForceLoad)
                    {
                        var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                        var io_m = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                        if (io_m == IntPtr.Zero)
                            return null;

                        m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io_m, true);
                        CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io_m);
                        if (m == IntPtr.Zero)
                            return null;

                        var io_t = CSUtility.Support.IFileManager.Instance.NewRes2Memory(CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + techFile, CSUtility.EFileType.Material, false);
                        if (io_t == IntPtr.Zero)
                        {
                            Log.FileLog.WriteLine("Material={0} can't find technique file {1}", matFile, techFile);
                            DllImportAPI.v3dStagedMaterialBase_Release(m);
                            return null;
                        }

                        t = DllImportAPI.vStandMaterialMgr_LoadTechnique(mStandMaterialMgr, techFile, io_t, true);
                        CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io_t);
                        if (t == IntPtr.Zero)
                        {
                            Log.FileLog.WriteLine("Material={0} can't load technique file {1}", matFile, techFile);
                            DllImportAPI.v3dStagedMaterialBase_Release(m);
                            return null;
                        }

                        DllImportAPI.vStandMaterial_SetTechnique(m, t);
                    }
                    else
                    {
                        t = DllImportAPI.vStandMaterialMgr_FindTechnique(mStandMaterialMgr, techFile);
                        if (t != IntPtr.Zero)
                        {
                            m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, matFile);
                            if (m == IntPtr.Zero)
                            {
                                var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                                var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                                if (io == IntPtr.Zero)
                                {
                                    Log.FileLog.WriteLine("Tech={0} can't find material file {1}", id, matFile);
                                    DllImportAPI.vStandMaterialMgr_RemoveTechnique(mStandMaterialMgr, techFile);
                                    DllImportAPI.v3dTechnique_Release(t);
                                    return null;
                                }

                                m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io, false);
                                CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                                if (m == IntPtr.Zero)
                                {
                                    Log.FileLog.WriteLine("Tech={0} can't load material file {1}", id, matFile);
                                    DllImportAPI.vStandMaterialMgr_RemoveTechnique(mStandMaterialMgr, techFile);
                                    DllImportAPI.v3dTechnique_Release(t);
                                    return null;
                                }

                                DllImportAPI.vStandMaterial_SetTechnique(m, t);

                            }
                        }
                        else
                        {
                            var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + techFile, CSUtility.EFileType.Material, false);
                            if (io == IntPtr.Zero)
                                return null;

                            t = DllImportAPI.vStandMaterialMgr_LoadTechnique(mStandMaterialMgr, techFile, io, false);
                            CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                            if (t == IntPtr.Zero)
                                return null;

                            m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, matFile);
                            if (m == IntPtr.Zero)
                            {
                                var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                                var io_m = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                                if (io_m == IntPtr.Zero)
                                {
                                    Log.FileLog.WriteLine("Tech={0} can't find material file {1}", id, matFile);
                                    DllImportAPI.vStandMaterialMgr_RemoveTechnique(mStandMaterialMgr, techFile);
                                    DllImportAPI.v3dTechnique_Release(t);
                                    return null;
                                }

                                m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io_m, false);
                                CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io_m);
                                if (m == IntPtr.Zero)
                                {
                                    Log.FileLog.WriteLine("Tech={0} can't load material file {1}", id, matFile);
                                    DllImportAPI.vStandMaterialMgr_RemoveTechnique(mStandMaterialMgr, techFile);
                                    DllImportAPI.v3dTechnique_Release(t);
                                    return null;
                                }
                            }

                            DllImportAPI.vStandMaterial_SetTechnique(m, t);

                        }
                    }

                    //var uniqueId = IDllImportAPI.v3dStagedMaterialBase_GetUniqueID(m);
                    //if(!mFilesUniqueDictionary.ContainsKey(uniqueId))
                    //{
                    //    var info = GetFileDictionaryValue(matId);
                    //    info.uniqueID = uniqueId;
                    //    mFilesUniqueDictionary[uniqueId] = info;
                    //}

                    var result = new Material();
                    result._Initialize(m, t);
                    DllImportAPI.v3dStagedMaterialBase_Release(m);
                    DllImportAPI.v3dTechnique_Release(t);

                    return result;
                }
            }
        }
        //public model3.v3dStagedMaterialInstance* LoadMaterialTechInstance(ref System.Guid techId)
        //{
        //    return LoadMaterialTechInstance(techId, false);
        //}
        /*        public Material LoadMaterialByTechId(ref System.Guid techId, bool bForceLoad = false)
                {
                    var inst = LoadMaterialTechInstance(ref techId, bForceLoad);
                    if (inst == IntPtr.Zero)
                        return null;

                    var result = new Material(inst);

                    DllImportAPI.v3dStagedMaterialInstance_Release(inst);

                    return result;
                }*/
        /// <summary>
        /// 加载材质实例
        /// </summary>
        /// <param name="techId">材质的techniqueID</param>
        /// <param name="bForceLoad">是否强制从磁盘读取</param>
        /// <returns>材质实例的指针</returns>
        public IntPtr LoadMaterialTechInstance(ref System.Guid techId, bool bForceLoad = false)
        {
            unsafe
            {
                // return model3.v3dStagedMaterialInstance*
                if (mStandMaterialMgr == IntPtr.Zero)
                    return IntPtr.Zero;

                if (techId == Guid.Empty)
                    return IntPtr.Zero;

                string matFile = "";
                string techFile = GetFileDictionaryFileValue(techId);
                if (string.IsNullOrEmpty(techFile))
                {
                    techFile = FindTechniqueFile(techId, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                    if (!string.IsNullOrEmpty(techFile))
                    {
                        SetFileDictionaryFileValue(techId, techFile);
                        var matId = GetMatGuidFromTechFile(techFile);
                        SetFileDictionaryOwnerIdValue(techId, matId);
                        matFile = FindMaterialFile(matId, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);

                        SaveFileDictionary();
                    }
                }
                else
                {
                    var matId = GetFileDictionaryOwnerId(techId);
                    if (matId == Guid.Empty)
                    {
                        matId = GetMatGuidFromTechFile(techFile);
                        SetFileDictionaryOwnerIdValue(techId, matId);
                        SaveFileDictionary();
                    }
                    matFile = FindMaterialFile(matId, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                }

                IntPtr m, t;

                if (bForceLoad)
                {
                    var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                    var io_m = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                    if (io_m == IntPtr.Zero)
                        return IntPtr.Zero;

                    m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io_m, true);
                    CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io_m);
                    if (m == IntPtr.Zero)
                        return IntPtr.Zero;

                    var io_t = CSUtility.Support.IFileManager.Instance.NewRes2Memory(CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + techFile, CSUtility.EFileType.Material, false);
                    if (io_t == IntPtr.Zero)
                        return IntPtr.Zero;

                    t = DllImportAPI.vStandMaterialMgr_LoadTechnique(mStandMaterialMgr, techFile, io_t, true);
                    CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io_t);
                    if (t == IntPtr.Zero)
                        return IntPtr.Zero;

                    DllImportAPI.vStandMaterial_SetTechnique(m, t);
                }
                else
                {
                    t = DllImportAPI.vStandMaterialMgr_FindTechnique(mStandMaterialMgr, techFile);
                    if (t != IntPtr.Zero)
                    {
                        m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, matFile);
                        if (m == IntPtr.Zero)
                        {
                            var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                            var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                            if (io == IntPtr.Zero)
                                return IntPtr.Zero;

                            m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io, false);
                            CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                            if (m == IntPtr.Zero)
                                return IntPtr.Zero;

                            DllImportAPI.vStandMaterial_SetTechnique(m, t);
                        }
                    }
                    else
                    {
                        var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + techFile, CSUtility.EFileType.Material, false);
                        if (io == IntPtr.Zero)
                            return IntPtr.Zero;

                        t = DllImportAPI.vStandMaterialMgr_LoadTechnique(mStandMaterialMgr, techFile, io, false);
                        CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                        if (t == IntPtr.Zero)
                            return IntPtr.Zero;

                        m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, matFile);
                        if (m == IntPtr.Zero)
                        {
                            var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                            var io_m = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                            if (io_m == IntPtr.Zero)
                                return IntPtr.Zero;

                            m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io_m, false);
                            CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io_m);
                            if (m == IntPtr.Zero)
                                return IntPtr.Zero;
                        }

                        DllImportAPI.vStandMaterial_SetTechnique(m, t);
                    }
                }

                var result = DllImportAPI.v3dStagedMaterialInstance_New();
                DllImportAPI.v3dStagedMaterialInstance_SetMaterial(result, m, t);
                DllImportAPI.v3dStagedMaterialBase_Release(m);
                DllImportAPI.v3dTechnique_Release(t);
                return result;
            }

        }
        /// <summary>
        /// 删除材质
        /// </summary>
        /// <param name="matId">材质的ID</param>
        public void RemoveMaterial(Guid matId)
        {
            var matFile = GetFileDictionaryFileValue(matId);
            DllImportAPI.vStandMaterialMgr_RemoveMaterial(mStandMaterialMgr, matFile);
        }
        /// <summary>
        /// 加载材质实例
        /// </summary>
        /// <param name="matId">材质实例的ID</param>
        /// <returns>返回材质实例的指针</returns>
		public IntPtr LoadMaterialInstance(ref System.Guid matId)
        {
            unsafe
            {
                // return model3.v3dStagedMaterialInstance*
                if (mStandMaterialMgr == IntPtr.Zero)
                    return IntPtr.Zero;

                if (matId == Guid.Empty)
                    return IntPtr.Zero;

                var matFile = FindMaterialFile(matId, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                if (string.IsNullOrEmpty(matFile))
                    return IntPtr.Zero;

                IntPtr m, t;
                m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, matFile);
                if (m == IntPtr.Zero)
                {
                    var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                    var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                    if (io == IntPtr.Zero)
                        return IntPtr.Zero;

                    m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io, false);
                    CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                    if (m == IntPtr.Zero)
                        return IntPtr.Zero;
                }

                t = DllImportAPI.vStandMaterial_GetDefaultTechnique(m);

                var result = DllImportAPI.v3dStagedMaterialInstance_New();
                DllImportAPI.v3dStagedMaterialInstance_SetMaterial(result, m, t);
                DllImportAPI.v3dStagedMaterialBase_Release(m);
                DllImportAPI.v3dTechnique_Release(t);
                return result;
            }
        }

        string mDefaultMatFile = "";

        /// <summary>
        /// 获取指定文件名的材质
        /// </summary>
        /// <param name="materialFileName">材质文件，相对于resource目录</param>
        /// <param name="forceLoad">是否强制从磁盘读取</param>
        /// <returns>返回获取的材质实例</returns>
        public Material GetMaterial(string materialFileName, bool forceLoad)
        {
            IntPtr m = IntPtr.Zero;
            if (!forceLoad)
            {
                m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, materialFileName);
            }

            if (m == IntPtr.Zero)
            {
                var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + materialFileName, CSUtility.EFileType.Material, false);
                if (io == IntPtr.Zero)
                    return null;

                m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, materialFileName, io, forceLoad);
                CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                if (m == IntPtr.Zero)
                    return null;
            }

            var t = DllImportAPI.vStandMaterial_GetDefaultTechnique(m);

            var result = new Material();
            result._Initialize(m, t);
            DllImportAPI.v3dStagedMaterialBase_Release(m);
            DllImportAPI.v3dTechnique_Release(t);
            return result;
        }

        /// <summary>
        /// 获取指定文件名的材质实例
        /// </summary>
        /// <param name="hostMaterialFileName">材质模板文件，相对于resource目录</param>
        /// <param name="instanceFileName">材质实例文件，相对于resource目录</param>
        /// <param name="forceLoad">是否强制从磁盘读取</param>
        /// <returns>返回指定文件名的材质实例</returns>
        public Material GetMaterialInstance(string hostMaterialFileName, string instanceFileName, bool forceLoad)
        {
            IntPtr m = IntPtr.Zero;
            if (!forceLoad)
            {
                m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, hostMaterialFileName);
            }

            IntPtr t = IntPtr.Zero;
            if (!forceLoad)
            {
                t = DllImportAPI.vStandMaterialMgr_FindTechnique(mStandMaterialMgr, instanceFileName);
            }

            if (m == IntPtr.Zero)
            {
                var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + hostMaterialFileName, CSUtility.EFileType.Material, false);
                if (io == IntPtr.Zero)
                    return null;

                m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, hostMaterialFileName, io, forceLoad);
                CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                if (m == IntPtr.Zero)
                    return null;
            }
            if (t == IntPtr.Zero)
            {
                var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + instanceFileName, CSUtility.EFileType.Material, false);
                if (io == IntPtr.Zero)
                    return null;

                t = DllImportAPI.vStandMaterialMgr_LoadTechnique(mStandMaterialMgr, instanceFileName, io, forceLoad);
                CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                if (t == IntPtr.Zero)
                    return null;
            }

            var result = new Material();
            result._Initialize(m, t);
            DllImportAPI.v3dStagedMaterialBase_Release(m);
            DllImportAPI.v3dTechnique_Release(t);
            return result;
        }
        /// <summary>
        /// 获取默认材质
        /// </summary>
        /// <returns>返回默认材质</returns>
        public Material GetDefaultMaterial()
        {
            //return GetMaterialDefaultTechnique(CSUtility.Support.IFileConfig.DefaultMaterialId);
            var basePath = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;//CSUtility.Support.IFileConfig.SystemMaterialPath;

            if (string.IsNullOrEmpty(mDefaultMatFile))
            {
                mDefaultMatFile = FindMaterialFile(CSUtility.Support.IFileConfig.DefaultMaterialId, basePath);
                if (string.IsNullOrEmpty(mDefaultMatFile))
                    return null;
            }

            IntPtr m, t;
            m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, mDefaultMatFile);
            if (m == IntPtr.Zero)
            {
                var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + mDefaultMatFile;
                var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                if (io == IntPtr.Zero)
                    return null;

                m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, mDefaultMatFile, io, false);
                CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                if (m == IntPtr.Zero)
                    return null;
            }

            t = DllImportAPI.vStandMaterial_GetDefaultTechnique(m);

            var result = new Material();
            result._Initialize(m, t);
            DllImportAPI.v3dStagedMaterialBase_Release(m);
            DllImportAPI.v3dTechnique_Release(t);
            return result;
        }
        /// <summary>
        /// 获取材质的默认technique
        /// </summary>
        /// <param name="matId">材质ID</param>
        /// <returns>返回默认technique的材质实例</returns>
		public Material GetMaterialDefaultTechnique(System.Guid matId)
        {
            return GetMaterialDefaultTechnique(matId, false);
        }
        /// <summary>
        /// 获取指定ID的默认technique材质实例
        /// </summary>
        /// <param name="matId">材质ID</param>
        /// <param name="bForceLoad">是否强制从磁盘加载</param>
        /// <returns>返回获取的材质</returns>
		public Material GetMaterialDefaultTechnique(System.Guid matId, bool bForceLoad)
        {
            var matFile = GetFileDictionaryFileValue(matId);
            if (string.IsNullOrEmpty(matFile))
            {
                matFile = FindMaterialFile(matId, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
                if (string.IsNullOrEmpty(matFile))
                    return null;
                SetFileDictionaryFileValue(matId, matFile);
                SaveFileDictionary();
            }

            IntPtr m, t;

            if (bForceLoad)
            {
                var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                if (io == IntPtr.Zero)
                    return null;

                m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io, true);
                CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                if (m == IntPtr.Zero)
                    return null;
            }
            else
            {
                m = DllImportAPI.vStandMaterialMgr_FindMaterial(mStandMaterialMgr, matFile);
                if (m == IntPtr.Zero)
                {
                    var tempFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + matFile;
                    var io = CSUtility.Support.IFileManager.Instance.NewRes2Memory(tempFile, CSUtility.EFileType.Material, false);
                    if (io == IntPtr.Zero)
                        return null;

                    m = DllImportAPI.vStandMaterialMgr_LoadMaterial(mStandMaterialMgr, matFile, io, false);
                    CSUtility.Support.IFileManager.Instance.ReleaseRes2Memory(io);
                    if (m == IntPtr.Zero)
                        return null;
                }
            }

            t = DllImportAPI.vStandMaterial_GetDefaultTechnique(m);

            var result = new Material();
            result._Initialize(m, t);
            DllImportAPI.v3dStagedMaterialBase_Release(m);
            DllImportAPI.v3dTechnique_Release(t);
            return result;
        }
        /// <summary>
        /// 获取材质的版本号
        /// </summary>
        /// <param name="matId">材质ID</param>
        /// <returns>返回指定ID的材质版本号</returns>
		public System.UInt32 GetMaterialVersion(System.Guid matId)
        {
            var mat = LoadMaterialInstance(ref matId);
            if (mat == IntPtr.Zero)
                return UInt32.MaxValue;

            return DllImportAPI.v3dStagedMaterialInstance_GetVer(mat);
        }
        /// <summary>
        /// 获取technique的版本号
        /// </summary>
        /// <param name="techId">材质的ID</param>
        /// <returns>返回technique的版本号</returns>
		public System.UInt32 GetTechniqueVersion(System.Guid techId)
        {
            var mat = LoadTechnique(techId);
            if (mat == null)
                return UInt32.MaxValue;

            return DllImportAPI.v3dStagedMaterialInstance_GetTechniqueVer(mat.MaterialPtr);
        }

        //public model3.v3dStagedMaterialInstance* LoadMaterial(System.String matFile)
        //{
        //    return NULL;
        //}

        //public System.Collections.Generic.List<IMaterialTemplate> GetAllMaterialTemplate()
        //{
        //    System::Collections::Generic::List<IMaterialTemplate^>^ result = gcnew System::Collections::Generic::List<IMaterialTemplate^>();
        //    const std::map<VString,model3::vStandMaterial*>& maps = mStandMaterialMgr->GetMaterials();
        //    for( std::map<VString,model3::vStandMaterial*>::const_iterator i=maps.begin();i!=maps.end();++i )
        //    {
        //        IMaterialTemplate^ mt = gcnew IMaterialTemplate(i->second);
        //        result->Add(mt);
        //    }
        //    return result;
        //}
        /// <summary>
        /// 刷新材质特效
        /// </summary>
        /// <param name="material">材质实例</param>
        public void RefreshEffect(Material material)
        {
            unsafe
            {
                if (material == null || material.MaterialPtr == IntPtr.Zero)
                    return;

                DllImportAPI.vStandMaterialMgr_RefreshEffect(Engine.Instance.Client.Graphics.Device, material.MaterialPtr);
            }
        }
        /// <summary>
        /// 刷新所有的材质特效
        /// </summary>
		public void RefreshAllEffect()
        {
            unsafe
            {
                DllImportAPI.vStandMaterialMgr_RefreshAllEffect(mStandMaterialMgr, Engine.Instance.Client.Graphics.Device);
            }
        }

        /*public System.String GetMatFileFromTechFile(System.String techFile)
        {
            techFile = techFile.Replace("\\", "/");
		    var matDir = techFile.Substring(0, techFile.IndexOf(TechniqueRootFolder) - 1);
		    var matIdStr = matDir.Substring(matDir.LastIndexOf("/") + 1);
		    var matFile = matDir + "/" + matIdStr + CSUtility.Support.IFileConfig.MaterialExtension;

		    return matFile;
        }
		public System.Guid GetMatGuidFromTechFile(System.String techFile)
        {
		    techFile = techFile.Replace("\\", "/");
		                   var matDir = techFile.Substring(0, techFile.IndexOf(TechniqueRootFolder) - 1);
		    var matIdStr = matDir.Substring(matDir.LastIndexOf("/") + 1);

		    System.Guid outId = CSUtility.Support.IHelper.GuidTryParse(matIdStr);

		    return outId;
        }*/
        /// <summary>
        /// 从technique文件中获取相应的材质ID
        /// </summary>
        /// <param name="absTechFile">technique文件的绝对路径</param>
        /// <returns>返回获取的材质ID</returns>
        public System.Guid GetMatGuidFromTechFile(System.String absTechFile)
        {
            CSUtility.Support.XmlHolder holder = CSUtility.Support.XmlHolder.LoadXML(absTechFile);
            if (holder == null || holder.RootNode == null)
                return Guid.Empty;

            var att = holder.RootNode.FindAttrib("HostMaterialId");
            if (att != null)
                return CSUtility.Support.IHelper.GuidTryParse(att.Value);

            return Guid.Empty;
        }

        #region 材质索引文件管理
        // 路径管理表，路径相对于resource路径
        CSUtility.Support.ConcurentObjManager<System.Guid, stFileInfo> mFilesDictionary = new CSUtility.Support.ConcurentObjManager<System.Guid, stFileInfo>();
        // 记录Material对应的Technique列表
        CSUtility.Support.ConcurentObjManager<System.Guid, List<System.Guid>> mMaterialTechniqueDictionary = new CSUtility.Support.ConcurentObjManager<Guid, List<Guid>>();

        /// <summary>
        /// 取得材质索引的文件名（相对于release目录）
        /// </summary>
        /// <returns>材质索引的文件名</returns>
        public string GetFileDictionaryFileName()
        {
            return CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + CSUtility.Support.IFileConfig.DefaultMaterialFileDictionaryFile;
        }
        /// <summary>
        /// 通过材质ID获取对应的材质实例ID
        /// </summary>
        /// <param name="matId">材质模板ID</param>
        /// <returns>以此材质模板为父的材质实例列表</returns>
        public List<System.Guid> GetMaterialChildrenInstances(Guid matId)
        {
            List<System.Guid> retList = new List<Guid>();
            mMaterialTechniqueDictionary.TryGetValue(matId, out retList);
            return retList;
        }
        /// <summary>
        /// 读取文件列表
        /// </summary>
		public void LoadFileDictionary()
        {
            var fileName = GetFileDictionaryFileName();
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(fileName);
            if (xmlHolder == null)
                return;

            var nodes = xmlHolder.RootNode.FindNodes("FileNode");
            for (int i = 0; i < nodes.Count; i++)
            {
                var info = new stFileInfo();

                Guid key = Guid.Empty;
                var att = nodes[i].FindAttrib("key");
                if (att != null)
                    key = CSUtility.Support.IHelper.GuidParse(att.Value);

                att = nodes[i].FindAttrib("File");
                if (att != null)
                    info.file = att.Value;

                att = nodes[i].FindAttrib("OwnerId");
                if (att != null)
                    info.ownerId = CSUtility.Support.IHelper.GuidTryParse(att.Value);

                //att = nodes[i].FindAttrib("UniqueId");
                //if (att != null)
                //    info.uniqueID = System.Convert.ToUInt32(att.Value);

                mFilesDictionary.Add(key, info);

                if (info.ownerId == Guid.Empty)
                {
                    if (!mMaterialTechniqueDictionary.ContainsKey(key))
                    {
                        mMaterialTechniqueDictionary[key] = new List<Guid>();
                    }
                }
                else
                {
                    List<Guid> techList;
                    if (!mMaterialTechniqueDictionary.TryGetValue(info.ownerId, out techList))
                    {
                        techList = new List<Guid>();
                        mMaterialTechniqueDictionary[info.ownerId] = techList;
                    }

                    if (!techList.Contains(key))
                        techList.Add(key);
                }
            }
        }
        /// <summary>
        /// 清空文件列表
        /// </summary>
        public void ClearFileDictionary()
        {
            mFilesDictionary.Clear();
            mMaterialTechniqueDictionary.Clear();
        }
        /// <summary>
        /// 保存文件列表
        /// </summary>
		public void SaveFileDictionary()
        {
            var fileName = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + CSUtility.Support.IFileConfig.DefaultMaterialFileDictionaryFile;
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FileDic", "");

            mFilesDictionary.For_Each((Guid key, stFileInfo value, object argObj) =>
            {
                var node = xmlHolder.RootNode.AddNode("FileNode", "", xmlHolder);
                node.AddAttrib("key", key.ToString());
                node.AddAttrib("File", value.file);
                if (value.ownerId != Guid.Empty)
                    node.AddAttrib("OwnerId", value.ownerId.ToString());

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);


            CSUtility.Support.XmlHolder.SaveXML(fileName, xmlHolder, true);
        }
        /// <summary>
        /// 根据ID获取文件info
        /// </summary>
        /// <param name="key">材质ID</param>
        /// <returns>返回文件info</returns>
		public stFileInfo GetFileDictionaryValue(System.Guid key)
        {
            return mFilesDictionary.FindObj(key);
        }
        /// <summary>
        /// 根据ID获取文件列表中的值
        /// </summary>
        /// <param name="key">材质ID</param>
        /// <returns>返回材质文件名</returns>
		public System.String GetFileDictionaryFileValue(System.Guid key)
        {
            stFileInfo outValue = mFilesDictionary.FindObj(key);
            if (outValue == null)
                return "";

            return outValue.file;
        }
        /// <summary>
        /// 获取文件列表中的唯一ID
        /// </summary>
        /// <param name="key">材质ID</param>
        /// <returns>返回文件列表中的唯一ID</returns>
		public System.Guid GetFileDictionaryOwnerId(System.Guid key)
        {
            stFileInfo outValue = mFilesDictionary.FindObj(key);
            if (outValue == null)
                return Guid.Empty;

            return outValue.ownerId;
        }
        /// <summary>
        /// 设置文件列表中的文件信息
        /// </summary>
        /// <param name="key">材质ID</param>
        /// <param name="file">文件名</param>
		public void SetFileDictionaryFileValue(System.Guid key, System.String file)
        {
            file = file.Replace("\\", "/");

            stFileInfo info = mFilesDictionary.FindObj(key);
            if (info != null)
            {
                info.file = file;
            }
            else
            {
                info = new stFileInfo();
                info.file = file;
                mFilesDictionary.Add(key, info);
            }
        }
        /// <summary>
        /// 设置文件列表的唯一ID
        /// </summary>
        /// <param name="key">材质ID</param>
        /// <param name="ownerId">文件的唯一ID</param>
        public void SetFileDictionaryOwnerIdValue(System.Guid key, System.Guid ownerId)
        {
            stFileInfo info = mFilesDictionary.FindObj(key);
            if (info != null)
            {
                List<Guid> refList;
                if (mMaterialTechniqueDictionary.TryGetValue(info.ownerId, out refList))
                {
                    refList.Remove(key);
                }

                info.ownerId = ownerId;
                if (!mMaterialTechniqueDictionary.TryGetValue(info.ownerId, out refList))
                {
                    refList = new List<Guid>();
                    mMaterialTechniqueDictionary[info.ownerId] = refList;
                }

                if (!refList.Contains(key))
                    refList.Add(key);
            }
            else
            {
                info = new stFileInfo();
                info.ownerId = ownerId;
                mFilesDictionary.Add(key, info);

                List<Guid> refList;
                if (!mMaterialTechniqueDictionary.TryGetValue(info.ownerId, out refList))
                {
                    refList = new List<Guid>();
                    mMaterialTechniqueDictionary[info.ownerId] = refList;
                }
                if (!refList.Contains(key))
                    refList.Add(key);
            }
        }
        /// <summary>
        /// 删除相应ID的文件列表
        /// </summary>
        /// <param name="key">文件ID</param>
		public void RemoveFildDictionary(System.Guid key)
        {
            stFileInfo info;
            if (mFilesDictionary.TryGetValue(key, out info))
            {
                if (info.ownerId != Guid.Empty)
                {
                    List<Guid> refList;
                    if (mMaterialTechniqueDictionary.TryGetValue(info.ownerId, out refList))
                    {
                        refList.Remove(key);
                    }
                }
                else
                {
                    mMaterialTechniqueDictionary.Remove(key);
                }

                mFilesDictionary.Remove(key);
                SaveFileDictionary();
            }
        }
        #endregion
        /// <summary>
        /// 获取shader自动配置的数据
        /// </summary>
        /// <returns>得到shader的配置数据</returns>
        public List<string> GetShaderAutoDatas()
        {
            var retList = new List<string>();

            unsafe
            {

                int strCount = 0;
                void** autoDataStr = DllImportAPI.vStandMaterialMgr_GetShaderAutoDatas(Engine.Instance.Client.Graphics.Device, &strCount);

                for (int i = 0; i < strCount; i += 3)
                {
                    var valueType = System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(autoDataStr[i]));
                    var semantic = System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(autoDataStr[i + 1]));
                    var describe = System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(autoDataStr[i + 2]));

                    retList.Add(valueType + "," + semantic + "," + describe);
                }

                DllImportAPI.vStandMaterialMgr_DeleteShaderAutoDatasString(autoDataStr, strCount);
            }

            return retList;
        }
    }
}