using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GamePublisher
{
    public class ResourceData : INotifyPropertyChanged, IEquatable<ResourceData>
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public bool Equals(ResourceData other)
        {
            return (this.FileName.ToLower() == other.FileName.ToLower());
        }

        //FrameSet.Resource.ResourceDataBase mResData = new FrameSet.Resource.ResourceDataBase();

        //// 文件大小
        //protected Int64 mFileSize = 0;
        //public Int64 FileSize
        //{
        //    get { return mFileSize; }
        //    set { mFileSize = value; }
        //}

        protected string mFileName = "";
        public string FileName
        {
            get { return mFileName; }
            set
            {
                mFileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(value);
                mFileName = mFileName.Replace("\\", "/");

                //System.IO.FileInfo info = new System.IO.FileInfo(mFileName);
                //FileSize = info.Length;

                if (ResourceType != CSUtility.Support.enResourceType.Folder)
                    MD5 = CSUtility.Program.GetMD5HashFromFile(mFileName);

                TargetFile = mFileName.Replace(Program.SourceFolder.Replace("\\", "/"), Program.FullPackageFolder.Replace("\\", "/"));
            }
        }


        protected string mTargetFile = "";
        public string TargetFile
        {
            get { return mTargetFile; }
            set
            {
                mTargetFile = value;
                RelativeFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mTargetFile, Program.FullPackageFolder);
                OnPropertyChanged("TargetFile");
            }
        }

        protected string mRelativeFile = "";
        public string RelativeFile
        {
            get { return mRelativeFile; }
            set
            {
                mRelativeFile = value;
                if (!string.IsNullOrEmpty(mRelativeFile))
                    mRelativeFile = mRelativeFile.Replace("\\", "/");
                OnPropertyChanged("RelativeFile");
            }
        }

        protected CheckBoxEx.enCheckState mCheckState = CheckBoxEx.enCheckState.UnChecked;
        public CheckBoxEx.enCheckState CheckState
        {
            get { return mCheckState; }
            set
            {
                mCheckState = value;

                switch (mCheckState)
                {
                    case CheckBoxEx.enCheckState.AllChecked:
                        {
                            foreach (var child in ChildrenRes)
                            {
                                child.CheckState = CheckBoxEx.enCheckState.AllChecked;
                            }

                            if (ParentData != null)
                                ParentData.CheckState = CheckBoxEx.enCheckState.PartChecked;
                        }
                        break;

                    case CheckBoxEx.enCheckState.PartChecked:
                        {
                            // AllChecked
                            bool allChildrenChecked = true;
                            foreach (var child in ChildrenRes)
                            {
                                if (child.CheckState != CheckBoxEx.enCheckState.AllChecked)
                                {
                                    allChildrenChecked = false;
                                    break;
                                }
                            }

                            if (allChildrenChecked)
                            {
                                mCheckState = CheckBoxEx.enCheckState.AllChecked;
                                break;
                            }

                            // AllUnchecked
                            bool allChildrenUnChecked = true;
                            foreach (var child in ChildrenRes)
                            {
                                if (child.CheckState != CheckBoxEx.enCheckState.UnChecked)
                                {
                                    allChildrenUnChecked = false;
                                    break;
                                }
                            }

                            if (allChildrenUnChecked)
                            {
                                mCheckState = CheckBoxEx.enCheckState.UnChecked;
                                break;
                            }

                            mCheckState = CheckBoxEx.enCheckState.PartChecked;
                        }
                        break;

                    case CheckBoxEx.enCheckState.UnChecked:
                        {
                            foreach (var child in ChildrenRes)
                            {
                                child.CheckState = CheckBoxEx.enCheckState.UnChecked;
                            }

                            if (ParentData != null)
                                ParentData.CheckState = CheckBoxEx.enCheckState.PartChecked;
                        }
                        break;
                }

                OnPropertyChanged("CheckState");
            }
        }

        //protected bool mIsChecked = false;
        //public bool IsChecked
        //{
        //    get
        //    {
        //        return mIsChecked;
        //    }
        //    set
        //    {
        //        mIsChecked = value;

        //        if (value)
        //        {
        //            var parent = ParentData;
        //            while (parent != null)
        //            {
        //                parent.CheckWithOutChildren();
        //                parent = parent.ParentData;
        //            }
        //        }
        //        if (ParentData != null && value)
        //            ParentData.CheckWithOutChildren();

        //        foreach (var child in ChildrenRes)
        //        {
        //            child.IsChecked = value;
        //        }
        //        OnPropertyChanged("IsChecked");
        //    }
        //}
        //private void CheckWithOutChildren()
        //{
        //    mIsChecked = true;
        //    OnPropertyChanged("IsChecked");
        //}

        protected string mMD5 = "";
        public string MD5
        {
            get { return mMD5; }
            set
            {
                mMD5 = value;
                OnPropertyChanged("MD5");
            }
        }

        protected CSUtility.Support.enResourceType mResourceType = CSUtility.Support.enResourceType.Unknow;
        public CSUtility.Support.enResourceType ResourceType
        {
            get { return mResourceType; }
            set
            {
                mResourceType = value;
                OnPropertyChanged("ResourceType");
            }
        }

        protected Int32 mReference = 0;
        public Int32 Reference
        {
            get { return mReference; }
            set
            {
                mReference = value;
                OnPropertyChanged("Reference");
            }
        }

        ObservableCollection<ResourceData> mChildrenRes = new ObservableCollection<ResourceData>();
        public ObservableCollection<ResourceData> ChildrenRes
        {
            get { return mChildrenRes; }
            set { mChildrenRes = value; }
        }

        Dictionary<string, ReferenceResourceData> mRefSources = new Dictionary<string, ReferenceResourceData>();
        public Dictionary<string, ReferenceResourceData> RefSources
        {
            get { return mRefSources; }
        }

        public ResourceData ParentData = null;

        public ResourceData()
        {

        }
        private ResourceData(CSUtility.Support.enResourceType resType, string fileName)
        {
            ResourceType = resType;
            FileName = fileName;
        }

        public bool CheckNeedCopyFile()
        {
            if (ResourceType == CSUtility.Support.enResourceType.Folder)
                return false;

            if (System.IO.File.Exists(TargetFile))
                return false;

            return true;
        }

        public void SetRefSource(string source)
        {
            if (RefSources.ContainsKey(source))
            {
                RefSources[source].RefCount++;
            }
            else
            {
                var refData = new ReferenceResourceData();
                refData.RefCount = 1;
                refData.RefInfo = source;
                RefSources[source] = refData;
            }

            Reference++;
        }

        public bool Read(CSUtility.Support.XndAttrib xndAtt, bool fullInfo)
        {
            Byte version = 0;
            xndAtt.Read(out version);
            switch (version)
            {
                case 0:
                    {
                        //if (!mResData.Read(xndAtt))
                        //    return false;

                        if (fullInfo)
                        {
                            string fileName;
                            xndAtt.Read(out fileName);
                            FileName = fileName;

                            string md5;
                            xndAtt.Read(out md5);
                            MD5 = md5; 

                            //bool tempChecked;
                            //xndAtt.Read(out tempChecked);
                            //IsChecked = tempChecked;
                            string checkStateStr;
                            xndAtt.Read(out checkStateStr);
                            CheckState = CSUtility.Support.IHelper.EnumTryParse<CheckBoxEx.enCheckState>(checkStateStr);

                            string typeStr;
                            xndAtt.Read(out typeStr);
                            CSUtility.Support.enResourceType resType;
                            if (System.Enum.TryParse<CSUtility.Support.enResourceType>(typeStr, out resType))
                                ResourceType = resType;

                            int reference;
                            xndAtt.Read(out reference);
                            Reference = reference;

                            int count = 0;
                            ChildrenRes.Clear();
                            xndAtt.Read(out count);
                            for (int i = 0; i < count; i++)
                            {
                                var res = new ResourceData();
                                res.Read(xndAtt, fullInfo);
                                ChildrenRes.Add(res);
                            }

                            RefSources.Clear();
                            xndAtt.Read(out count);
                            for (int i = 0; i < count; i++)
                            {
                                string key;
                                xndAtt.Read(out key);
                                var res = new ReferenceResourceData();
                                res.Read(xndAtt);
                                RefSources[key] = res;
                            }
                        }
                        else
                        {
                            string relFileName;
                            xndAtt.Read(out relFileName);
                            RelativeFile = relFileName;

                            string md5;
                            xndAtt.Read(out md5);
                            MD5 = md5;

                            string typeStr;
                            xndAtt.Read(out typeStr);
                            CSUtility.Support.enResourceType resType;
                            if (System.Enum.TryParse<CSUtility.Support.enResourceType>(typeStr, out resType))
                                ResourceType = resType;
                        }
                    }
                    break;
            }

            return true;
        }
        public bool Write(CSUtility.Support.XndAttrib xndAtt, bool fullInfo)
        {
            Byte version = 0;
            xndAtt.Write(version);

            //if (!mResData.Write(xndAtt))
            //    return false;

            if (fullInfo)
            {
                xndAtt.Write(FileName);
                xndAtt.Write(MD5);
                //xndAtt.Write(IsChecked);
                xndAtt.Write(CheckState.ToString());
                var typeStr = ResourceType.ToString();
                xndAtt.Write(typeStr);
                xndAtt.Write(Reference);

                xndAtt.Write(ChildrenRes.Count);
                foreach (var res in ChildrenRes)
                {
                    res.Write(xndAtt, fullInfo);
                }

                xndAtt.Write(RefSources.Count);
                foreach (var data in RefSources)
                {
                    xndAtt.Write(data.Key);
                    data.Value.Write(xndAtt);
                }
            }
            else
            {
                xndAtt.Write(RelativeFile);
                xndAtt.Write(MD5);
                var typeStr = ResourceType.ToString();
                xndAtt.Write(typeStr);
            }

            return true;
        }

        public static ResourceData CreateResourceData(CSUtility.Support.enResourceType resType, string srcFileName)
        {
            if (string.IsNullOrEmpty(srcFileName))
                return null;

            var fileFullName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(srcFileName);

            if (resType == CSUtility.Support.enResourceType.Folder)
            {
                if (!System.IO.Directory.Exists(fileFullName))
                    return null;
            }
            else
                if (!System.IO.File.Exists(fileFullName))
                    return null;

            return new ResourceData(resType, srcFileName);
        }
    }
    public class ReferenceResourceData : CSUtility.Support.XndSaveLoadProxy
    {
        [CSUtility.Support.AutoSaveLoad]
        public UInt32 RefCount
        {
            get;
            set;
        }

        string mRefInfo = "";
        [CSUtility.Support.AutoSaveLoad]
        public string RefInfo
        {
            get { return mRefInfo; }
            set { mRefInfo = value; }
        }
    }

    public class ResourceDataManager
    {
        static ResourceDataManager smInstance = new ResourceDataManager();
        public static ResourceDataManager Instance
        {
            get { return smInstance; }
        }

        // 文件字典表
        Dictionary<string, ResourceData> mResDataDic = new Dictionary<string, ResourceData>();
        public Dictionary<string, ResourceData> ResDataDic
        {
            get { return mResDataDic; }
        }

        // 树形结构资源信息表
        ObservableCollection<ResourceData> mResourceDatas = new ObservableCollection<ResourceData>();
        public ObservableCollection<ResourceData> ResourceDatas
        {
            get { return mResourceDatas; }
        }

        public void Clear()
        {
            ResourceDatas.Clear();
        }

        // 从树形结构的ResourceData中提取出ResourceData列表
        // 排除的类型
        public static List<ResourceData> GetResourceDataListFromTree(ResourceData[] datas, CSUtility.Support.enResourceType[] exceptResourceTypes)
        {
            List<ResourceData> retList = new List<ResourceData>();

            foreach (var data in datas)
            {
                if (exceptResourceTypes != null && exceptResourceTypes.Contains(data.ResourceType))
                    continue;

                retList.Add(data);

                if(data.ChildrenRes.Count > 0)
                    retList.AddRange(GetResourceDataListFromTree(data.ChildrenRes.ToArray<ResourceData>(), exceptResourceTypes));
            }

            return retList;
        }

        public void AddResource(ResourceData res, string refSource, bool copyToZip, ObservableCollection<ResourceData> rootResDatas = null)
        {
            var tagFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(res.TargetFile, Program.FullPackageFolder);

            if (rootResDatas == null)
                rootResDatas = ResourceDatas;

            AddResource(res, refSource, "", tagFile, null, rootResDatas, copyToZip);
        }

        private bool AddResource(ResourceData res, string refSource, string rootFolder, string subFolder, ResourceData parent, ObservableCollection<ResourceData> rootResDatas, bool copyToZip)
        {
            if (string.IsNullOrEmpty(subFolder))
            {
                if (rootResDatas.Contains(res))
                {
                    var idx = rootResDatas.IndexOf(res);
                    rootResDatas[idx].SetRefSource(refSource);
                }
                else
                {
                    res.SetRefSource(refSource);
                    res.ParentData = parent;
                    rootResDatas.Add(res);
                    mResDataDic[res.RelativeFile] = res;

                    if (res.CheckNeedCopyFile())
                    {
                        Program.CopyFile(res.FileName, copyToZip);
                    }
                }

                return true;
            }
            else
            {
                var idx = subFolder.IndexOf('/');
                if (idx >= 0)
                {
                    var folderName = subFolder.Substring(0, idx);
                    if (string.IsNullOrEmpty(rootFolder))
                        rootFolder = folderName;
                    else
                        rootFolder = rootFolder + "/" + folderName;
                    subFolder = subFolder.Substring(idx + 1);
                }
                else
                {
                    if (string.IsNullOrEmpty(rootFolder))
                        rootFolder = subFolder;
                    else
                        rootFolder = rootFolder + "/" + subFolder;
                    subFolder = "";
                }

                if(!string.IsNullOrEmpty(subFolder))
                {
                    foreach (var data in rootResDatas)
                    {
                        if (data.RelativeFile.ToLower() == rootFolder.ToLower())
                        {
                            return AddResource(res, refSource, rootFolder, subFolder, data, data.ChildrenRes, copyToZip);
                        }
                    }
                }

                if (string.IsNullOrEmpty(subFolder))
                {
                    if (rootResDatas.Contains(res))
                    {
                        var dataIdx = rootResDatas.IndexOf(res);
                        if (res.ChildrenRes.Count > 0)
                        {
                            foreach (var childResData in res.ChildrenRes)
                            {
                                var subf = childResData.RelativeFile.Replace(res.RelativeFile + "/", "");
                                AddResource(childResData, refSource, res.RelativeFile, subf, rootResDatas[dataIdx], rootResDatas[dataIdx].ChildrenRes, copyToZip);
                            }
                        }
                        else
                            rootResDatas[dataIdx].SetRefSource(refSource);
                    }
                    else
                    {
                        res.SetRefSource(refSource);
                        res.ParentData = parent;
                        rootResDatas.Add(res);
                        mResDataDic[res.RelativeFile] = res;

                        if (res.CheckNeedCopyFile())
                        {
                            Program.CopyFile(res.FileName, copyToZip);
                        }
                    }
                    return true;
                }
                else
                {
                    var rData = ResourceData.CreateResourceData(CSUtility.Support.enResourceType.Folder, rootFolder);
                    rData.ParentData = parent;
                    rootResDatas.Add(rData);
                    mResDataDic[rData.RelativeFile] = rData;
                    return AddResource(res, refSource, rootFolder, subFolder, rData, rData.ChildrenRes, copyToZip);
                }

            }
        }

        // 存储完整资源数据，用于发布器记录版本信息
        public void SaveFullResourceDatas(string fileName)
        {
            CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.NewXNDHolder();

            var att = holder.Node.AddAttrib("Header");
            att.BeginWrite();
            // 版本号
            att.Write(Program.Version);
            att.EndWrite();
            
            att = holder.Node.AddAttrib("ResDatas");
            att.BeginWrite();

            att.Write(ResourceDatas.Count);
            foreach (var data in mResourceDatas)
            {
                data.Write(att, true);
            }

            att.EndWrite();

            CSUtility.Support.XndHolder.SaveXND(fileName, holder);
        }

        public void SaveCheckedResourceDatas(string fileName, bool fullData)
        {
            CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.NewXNDHolder();

            var att = holder.Node.AddAttrib("Header");
            att.BeginWrite();
            // 版本号
            att.Write(Program.Version);
            att.EndWrite();

            att = holder.Node.AddAttrib("ResDatas");
            att.BeginWrite();

            List<ResourceData> saveDatas = new List<ResourceData>();
            foreach (var data in mResDataDic.Values)
            {
                //if (!data.IsChecked)
                //    continue;
                if (data.CheckState == CheckBoxEx.enCheckState.UnChecked)
                    continue;

                saveDatas.Add(data);
            }

            att.Write(saveDatas.Count);
            foreach (var data in saveDatas)
            {
                data.Write(att, false);
            }

            att.EndWrite();

            CSUtility.Support.XndHolder.SaveXND(fileName, holder);
        }

        // 存储简单资源数据，用于跟随客户端发布
        public void SaveSimpleResourceData(string fileName)
        {
            //CSUtility.Support.IXndHolder holder = CSUtility.Support.IXndHolder.NewXNDHolder();

            //var att = holder.Node.AddAttrib("Header");
            //att.BeginWrite();
            //// 版本号
            //att.Write(Program.Version);
            //att.EndWrite();
            
            //att = holder.Node.AddAttrib("ResDatas");
            //att.BeginWrite();

            //List<ResourceData> saveDatas = new List<ResourceData>();
            //foreach (var data in mResDataDic.Values)
            //{
            //    if (data.ResourceType == CSUtility.Support.enResourceType.Folder)
            //        continue;

            //    saveDatas.Add(data);
            //}

            //att.Write(saveDatas.Count);
            //foreach (var data in saveDatas)
            //{
            //    data.Write(att, false);
            //}

            //att.EndWrite();

            //CSUtility.Support.IXndHolder.SaveXND(fileName, holder);

            SaveSimpleResourceData(fileName, mResDataDic.Values.ToArray<ResourceData>());
        }

        public static void SaveSimpleResourceData(string fileName, ResourceData[] datas)
        {
            CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.NewXNDHolder();

            var att = holder.Node.AddAttrib("Header");
            att.BeginWrite();
            // 版本号
            att.Write(Program.Version);
            att.EndWrite();
            
            att = holder.Node.AddAttrib("ResDatas");
            att.BeginWrite();

            List<ResourceData> saveDatas = new List<ResourceData>();
            foreach (var data in datas)
            {
                if (data.ResourceType == CSUtility.Support.enResourceType.Folder)
                    continue;

                saveDatas.Add(data);
            }

            att.Write(saveDatas.Count);
            foreach (var data in saveDatas)
            {
                data.Write(att, false);
            }

            att.EndWrite();

            CSUtility.Support.XndHolder.SaveXND(fileName, holder);
        }

        public ObservableCollection<ResourceData> LoadResourceDatas(string fileName, bool fullInfo)
        {
            ObservableCollection<ResourceData> retDatas = new ObservableCollection<ResourceData>();

            var holder = CSUtility.Support.XndHolder.LoadXND(fileName);

            var att = holder.Node.FindAttrib("ResDatas");
            att.BeginRead();

            int count = 0;
            att.Read(out count);
            for (int i = 0; i < count; i++)
            {
                ResourceData data = new ResourceData();
                data.Read(att, fullInfo);
                retDatas.Add(data);
            }

            att.EndRead();

            return retDatas;
        }

        public void CheckResourceWithRelativeFiles(string file)
        {
            foreach (var data in ResourceDatas)
            {
                CheckResourceWithRelativeFiles(data, file);
            }
        }

        // file为相对路径
        private void CheckResourceWithRelativeFiles(ResourceData data, string file)
        {
            if (data.RelativeFile.Replace("\\", "/") == file.Replace("\\", "/"))
            {
                //data.IsChecked = true;
                data.CheckState = CheckBoxEx.enCheckState.AllChecked;
                return;
            }

            foreach (var child in data.ChildrenRes)
            {
                CheckResourceWithRelativeFiles(child, file);
            }
        }
    }    
}
