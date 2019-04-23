using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.Data
{
    /// <summary>
    /// 数据模板的接口
    /// </summary>
    public interface IDataTemplateBase<ID_T> : CSUtility.Support.ICopyable
        where ID_T : IComparable, IFormattable, IConvertible, IComparable<ID_T>, IEquatable<ID_T>
    {
        /// <summary>
        /// 数据模板ID，只能是整数值类型（Byte,UInt16,UInt32,UInt64,SByte,Int16,Int32,Int64）
        /// </summary>
        ID_T Id
        {
            get;
            set;
        }

        /// <summary>
        /// 数据模板名称
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 数据模板版本号
        /// </summary>
        UInt32 Version
        {
            get;
            set;
        }

        /// <summary>
        /// 数据模板置藏标志，共编辑器使用
        /// </summary>
        bool IsDirty
        {
            get;
            set;
        }
    }
    
    /// <summary>
    /// 打上此特性的类都会自动拥有DataTemplateManager管理的能力
    /// </summary>
    public sealed class DataTemplateAttribute : Attribute
    {
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension
        {
            get;
            private set;
        }

        /// <summary>
        /// 所在文件夹（相对于模板根目录）
        /// </summary>
        public string FolderName
        {
            get;
            private set;
        }

        /// <summary>
        /// 模板数组大小最大值(该类型模板的最多数量，超出的忽略)
        /// </summary>
        public UInt64 ArrayMaxSize
        {
            get;
            private set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileExtension">文件扩展名</param>
        /// <param name="folderName">文件所在的目录，如果没有设置，则默认使用类型全名称</param>
        /// <param name="maxCount">数据模板的最大数量</param>
        public DataTemplateAttribute(string fileExtension, string folderName = null, UInt64 maxCount = UInt16.MaxValue)
        {
            FileExtension = fileExtension;
            ArrayMaxSize = maxCount;
            FolderName = folderName;
        }
    }

    /// <summary>
    /// 数据管理辅助类
    /// </summary>
    public class DataTemplateManagerAssist
    {
        static DataTemplateManagerAssist smInstance = new DataTemplateManagerAssist();
        public static DataTemplateManagerAssist Instance
        {
            get { return smInstance; }
        }

        /// <summary>
        /// 数据模板操作类型
        /// </summary>
        public enum enDataTemplateOperationType : Byte
        {
            /// <summary>
            /// 增加
            /// </summary>
            Add,
            /// <summary>
            /// 修改
            /// </summary>
            Modify,
            /// <summary>
            /// 删除
            /// </summary>
            Delete,
        }

        public DataTemplateManagerAssist()
        {
            RegistTemplateManagers();
        }

        System.Collections.Concurrent.ConcurrentDictionary<Type, object> mDataTemplateManagerDictionary = new System.Collections.Concurrent.ConcurrentDictionary<Type, object>();
        public System.Collections.Concurrent.ConcurrentDictionary<Type, object> DataTemplateManagerDictionary
        {
            get { return mDataTemplateManagerDictionary; }
        }
        void RegistTemplateManagers()
        {
            var interfaceName = typeof(IDataTemplateBase<>).FullName;
            var managerType = typeof(DataTemplateManager<,>);
            var types = CSUtility.Program.GetTypes();
            for(int i=0; i<types.Length;i++)
            {
                var type = types[i];
                var atts = type.GetCustomAttributes(typeof(DataTemplateAttribute), false);
                if (atts.Length <= 0)
                    continue;

                var att = atts[0] as DataTemplateAttribute;

                var interfaceType = type.GetInterface(interfaceName);
                if (interfaceType == null)
                    continue;

                var idType = GetDataTemplateIDType(type);
                if (idType == null)
                    continue;                

                var templateTypeSet = new Type[] { idType, type };
                var magType = managerType.MakeGenericType(templateTypeSet);
                var property = magType.GetProperty("Instance");
                mDataTemplateManagerDictionary[type] = property.GetValue(null);
            }
        }

        public Type GetDataTemplateIDType(Type dataTemplateType)
        {
            var propertyInfo = dataTemplateType.GetProperty("Id");
            if (propertyInfo == null)
                return null;

            return propertyInfo.PropertyType;
        }

        public object CreateDataTemplate(object id, Type dataTemplateType, string absFolder)
        {
            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplateType, out manager))
                return null;

            return manager.GetType().InvokeMember("CreateDataTemplate", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, manager, new object[] { id, absFolder });
        }

        public object GetDataTemplate(object id, Type dataTemplateType)
        {
            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplateType, out manager))
                return null;

            return manager.GetType().InvokeMember("GetDataTemplate", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, manager, new object[] { id, false });
        }

        public string GetRelativeRootPath(Type dataTemplateType)
        {
            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplateType, out manager))
                return "";

            return (string)manager.GetType().InvokeMember("RelativeRootPath", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.GetProperty, null, manager, null);
        }

        public string GetDataTemplateRelativeFileName(object dataTemplate)
        {
            if (dataTemplate == null)
                return "";

            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplate.GetType(), out manager))
                return "";

            return (string)manager.GetType().InvokeMember("GetDataTemplateRelativeFileName", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, manager, new object[] { dataTemplate });
        }
        public string GetDataTemplateRelativeFileName(object id, Type dataTemplateType)
        {
            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplateType, out manager))
                return "";

            return (string)manager.GetType().InvokeMember("GetDataTemplateRelativeFileName", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, manager, new object[] { id });
        }

        public bool SaveDataTemplate(object dataTemplate)
        {
            if (dataTemplate == null)
                return false;

            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplate.GetType(), out manager))
                return false;

            return (bool)manager.GetType().InvokeMember("SaveDataTemplate", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, manager, new object[] { dataTemplate });
        }

        public bool SaveAsDataTemplate(ref object dataTemplate, object id)
        {
            if (dataTemplate == null)
                return false;

            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplate.GetType(), out manager))
                return false;

            return (bool)manager.GetType().InvokeMember("SaveAsDataTemplate", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, manager, new object[] { dataTemplate, id });
        }

        public bool RemoveDataTemplate(object dataTemplate, bool deleteFile = false)
        {
            if (dataTemplate == null)
                return false;

            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplate.GetType(), out manager))
                return false;

            return (bool)manager.GetType().InvokeMember("RemoveDataTemplate", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, manager, new object[] { dataTemplate, deleteFile });
        }

        public bool ReloadDataTemplate(Type dataTemplateType, object id, enDataTemplateOperationType opType)
        {
            if (dataTemplateType == null)
                return false;
            object manager;
            if (!DataTemplateManagerDictionary.TryGetValue(dataTemplateType, out manager))
                return false;

            return (bool)manager.GetType().InvokeMember("ReloadDataTemplate", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, manager, new object[] { id, opType });
        }
    }

    public class DataTemplateManager<ID_T, Value_T> 
        where Value_T : IDataTemplateBase<ID_T>, new()
        where ID_T : IComparable, IFormattable, IConvertible, IComparable<ID_T>, IEquatable<ID_T>
    {
        static DataTemplateManager<ID_T, Value_T> smInstance = new DataTemplateManager<ID_T, Value_T>();
        public static DataTemplateManager<ID_T, Value_T> Instance
        {
            get { return smInstance; }
        }

        Value_T[] mDataTemplates;
        /// <summary>
        /// 模板文件数组，路径相对于Root
        /// </summary>
        string[] mDataTemplateFiles;

        private DataTemplateManager()
        {
            mDataTemplates = new Value_T[MaxCount];
            mDataTemplateFiles = new string[MaxCount];
        }

        string mRelativeRootPath = "";
        /// <summary>
        /// 数据模板文件所在的目录（目录相对于Root）
        /// </summary>
        public string RelativeRootPath
        {
            get
            {
                if(string.IsNullOrEmpty(mRelativeRootPath))
                {
                    var atts = typeof(Value_T).GetCustomAttributes(typeof(DataTemplateAttribute), false);
                    if (atts.Length <= 0)
                        return mRelativeRootPath;

                    var folderName = ((DataTemplateAttribute)atts[0]).FolderName;
                    if (string.IsNullOrEmpty(folderName))
                        folderName = typeof(Value_T).FullName;

                    mRelativeRootPath = CSUtility.Support.IFileConfig.DefaultTemplateDir + "/" + folderName + "/";
                }

                return mRelativeRootPath;
            }
        }

        string mFileExtension = "";
        public string FileExtension
        {
            get
            {
                if(string.IsNullOrEmpty(mFileExtension))
                {
                    var atts = typeof(Value_T).GetCustomAttributes(typeof(DataTemplateAttribute), false);
                    if (atts.Length <= 0)
                        return mRelativeRootPath;

                    mFileExtension = ((DataTemplateAttribute)atts[0]).FileExtension;
                }

                return mFileExtension;
            }
        }

        UInt64 mMaxCount = 0;
        public UInt64 MaxCount
        {
            get
            {
                if(mMaxCount <= 0)
                {
                    var atts = typeof(Value_T).GetCustomAttributes(typeof(DataTemplateAttribute), false);
                    if (atts.Length <= 0)
                        return mMaxCount;

                    mMaxCount = ((DataTemplateAttribute)atts[0]).ArrayMaxSize;
                }

                return mMaxCount;
            }
        }

        /// <summary>
        /// 获得数据模板文件文件名，文件名相对于Release，如果没有找到则返回默认文件名
        /// </summary>
        /// <param name="dataTemplate"></param>
        /// <returns>数据文件名，路径相对于Release</returns>
        public string GetDataTemplateRelativeFileName(Value_T dataTemplate)
        {
            if (dataTemplate == null)
                return "";

            return GetDataTemplateRelativeFileName(((IDataTemplateBase<ID_T>)dataTemplate).Id);
        }
        
        /// <summary>
        /// 获得数据模板文件文件名，文件名相对于Release，如果没有找到则返回默认文件名
        /// </summary>
        /// <param name="id">数据模板id</param>
        /// <returns>数据文件名，路径相对于Release</returns>
        public string GetDataTemplateRelativeFileName(ID_T id)
        {
            var idx = System.Convert.ToUInt64(id);
            if (idx >= MaxCount)
                return "";

            var fileName = mDataTemplateFiles[idx];
            if (string.IsNullOrEmpty(fileName))
                return RelativeRootPath + id.ToString() + FileExtension;

            return fileName;
        }

        private UInt64 GetIdxFromFileName(string fileName)
        {
            fileName = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(fileName);
            var idStr = fileName.Substring(0, fileName.IndexOf('.'));
            return System.Convert.ToUInt64(idStr);
        }

        /// <summary>
        /// 读取所有Value_T类型的已知文件位置的数据模板
        /// </summary>
        /// <returns>数据模板列表</returns>
        public List<Value_T> LoadAllDataTemplate()
        {
            mDataTemplates = new Value_T[MaxCount];
            mDataTemplateFiles = new string[MaxCount];

            var absFolder = CSUtility.Support.IFileManager.Instance.Root + RelativeRootPath;
            if (!System.IO.Directory.Exists(absFolder))
                return new List<Value_T>();
            var files = System.IO.Directory.GetFiles(absFolder, "*" + FileExtension, System.IO.SearchOption.AllDirectories);
            for(int i=0; i<files.Length; i++)
            {
                var file = files[i];
                Value_T dataTemplate = new Value_T();
                if (dataTemplate == null)
                    continue;

                if (((IDataTemplateBase<ID_T>)dataTemplate) == null)
                    continue;

                if (!CSUtility.Support.IConfigurator.FillProperty(dataTemplate, file))
                    continue;

                var idx = GetIdxFromFileName(file);
                ((IDataTemplateBase<ID_T>)dataTemplate).IsDirty = false;
                mDataTemplates[idx] = dataTemplate;
                mDataTemplateFiles[idx] = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
            }

            var newList = new List<Value_T>(mDataTemplates);

            return newList;
        }

        public bool ReloadDataTemplate(ID_T id, DataTemplateManagerAssist.enDataTemplateOperationType opType)
        {
            var idx = System.Convert.ToUInt64(id);
            if (idx >= MaxCount)
                return false;

            if (object.Equals(GetDataTemplate(id, true), default(Value_T)))
                return false;

            return true;
        }

        /// <summary>
        /// 根据id获取数据模板
        /// </summary>
        /// <param name="id">数据模板id</param>
        /// <param name="forceLoad">强制重新加载</param>
        /// <returns></returns>
        public Value_T GetDataTemplate(ID_T id, bool forceLoad = false)
        {
            Value_T dataTemplate = default(Value_T);
            var idx = System.Convert.ToUInt64(id);
            if (idx >= MaxCount)
                return dataTemplate;

            if (!forceLoad)
            {
                dataTemplate = mDataTemplates[idx];
                if (dataTemplate != null)
                    return dataTemplate;
            }

            var absFolder = CSUtility.Support.IFileManager.Instance.Root + RelativeRootPath;
            if (!System.IO.Directory.Exists(absFolder))
                return dataTemplate;
            var files = System.IO.Directory.GetFiles(absFolder, id.ToString() + FileExtension, System.IO.SearchOption.AllDirectories);
            if (files.Length <= 0)
                return dataTemplate;
            
            dataTemplate = new Value_T();
            if (!CSUtility.Support.IConfigurator.FillProperty(dataTemplate, files[0]))
                return default(Value_T);

            ((IDataTemplateBase<ID_T>)dataTemplate).IsDirty = false;
            mDataTemplates[idx] = dataTemplate;
            mDataTemplateFiles[idx] = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(files[0]);

            return dataTemplate;
        }

        public Value_T CreateDataTemplate(ID_T id, string absFolder)
        {
            var idx = System.Convert.ToUInt64(id);
            if (idx >= MaxCount)
                return default(Value_T);

            var dataTemplate = mDataTemplates[idx];
            if (dataTemplate != null)
                return dataTemplate;

            var atts = typeof(Value_T).GetCustomAttributes(typeof(DataTemplateAttribute), false);
            if (atts.Length <= 0)
            {
                return default(Value_T);
            }

            dataTemplate = new Value_T();
            ((IDataTemplateBase<ID_T>)dataTemplate).Id = id;
            ((IDataTemplateBase<ID_T>)dataTemplate).IsDirty = true;

            var absFileName = absFolder + "/" + id.ToString() + ((DataTemplateAttribute)atts[0]).FileExtension;
            var relFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absFileName);
            mDataTemplates[idx] = dataTemplate;
            mDataTemplateFiles[idx] = relFileName;
            return dataTemplate;
        }

        public bool RemoveDataTemplate(ID_T id, bool deleteFile = false)
        {
            var idx = System.Convert.ToUInt64(id);
            if (idx >= MaxCount)
                return false;

            if (deleteFile)
            {
                var absFileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mDataTemplateFiles[idx]);
                if (System.IO.File.Exists(absFileName))
                {
                    System.IO.File.Delete(absFileName);
                }
            }
            mDataTemplates[idx] = default(Value_T);
            mDataTemplateFiles[idx] = "";

            return true;
        }

        public bool RemoveDataTemplate(Value_T data, bool deleteFile = false)
        {
            var idx = System.Convert.ToUInt64(((IDataTemplateBase<ID_T>)data).Id);
            if (idx >= MaxCount)
                return false;

            if (deleteFile)
            {
                var absFileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mDataTemplateFiles[idx]);
                if (System.IO.File.Exists(absFileName))
                {
                    System.IO.File.Delete(absFileName);
                }
            }
            mDataTemplates[idx] = default(Value_T);
            mDataTemplateFiles[idx] = "";

            return true;
        }

        public bool SaveDataTemplate(ID_T id)
        {
            var idx = System.Convert.ToUInt64(id);
            var dataTemplate = mDataTemplates[idx];
            if (dataTemplate == null)
                return false;

            var relFileName = mDataTemplateFiles[idx];
            if (string.IsNullOrEmpty(relFileName))
                return false;

            ((IDataTemplateBase<ID_T>)dataTemplate).Version++;
            if (CSUtility.Support.IConfigurator.SaveProperty(dataTemplate, typeof(Value_T).FullName, relFileName))
                ((IDataTemplateBase<ID_T>)dataTemplate).IsDirty = false;

            return true;
        }

        public bool SaveDataTemplate(Value_T data)
        {
            var idx = System.Convert.ToUInt64(((IDataTemplateBase<ID_T>)data).Id);
            if (idx >= MaxCount)
                return false;

            if (mDataTemplates[idx] == null)
                mDataTemplates[idx] = data;

            var relFileName = mDataTemplateFiles[idx];
            if(string.IsNullOrEmpty(relFileName))
            {
                relFileName = GetDataTemplateRelativeFileName(data);
                mDataTemplateFiles[idx] = relFileName;
            }

            ((IDataTemplateBase<ID_T>)data).Version++;
            if (CSUtility.Support.IConfigurator.SaveProperty(data, typeof(Value_T).FullName, relFileName))
                ((IDataTemplateBase<ID_T>)data).IsDirty = false;

            return true;
        }

        public bool SaveAsDataTemplate(ref Value_T data, ID_T tagId)
        {
            var idx = System.Convert.ToUInt64(tagId);
            if (idx >= MaxCount)
                return false;

            var fileName = GetDataTemplateRelativeFileName(tagId);

            var dataCopy = new Value_T();
            dataCopy.CopyFrom(data);
            dataCopy.Id = tagId;
            mDataTemplates[idx] = dataCopy;
            mDataTemplateFiles[idx] = fileName;

            ((IDataTemplateBase<ID_T>)dataCopy).Version++;
            if (CSUtility.Support.IConfigurator.SaveProperty(dataCopy, typeof(Value_T).FullName, fileName))
                ((IDataTemplateBase<ID_T>)dataCopy).IsDirty = false;

            data = dataCopy;

            return true;
        }
    }
}
