using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace MaterialEditor
{
    public class MaterialFileInfo : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        ~MaterialFileInfo()
        {

        }

        public delegate void Delegate_OnMaterialFileDirtyChange(MaterialFileInfo info);
        public event Delegate_OnMaterialFileDirtyChange OnMaterialFileDirtyChange;

        public MaterialTechniqueInfo DefaultTechnique;
        
        // 此路径相对于 CSUtility.Support.IFileConfig.DefaultMaterialDirectory
        string mMaterialFileName = "";
        [Browsable(false)]
        public string MaterialFileName
        {
            get { return mMaterialFileName; }
        }

        string mExtendString = "";
        [Browsable(false)]
        public string ExtendString
        {
            get { return mExtendString; }
            set { mExtendString = value; }
        }

        string m_strMatName = "UnknowMaterial";
        [DisplayName("材质模板名称")]
        public string MaterialName
        {
            get { return m_strMatName; }
            set
            {
                m_strMatName = value;
                OnPropertyChanged("MaterialName");

                IsDirty = true;
            }
        }
        string m_strRequire = "DiffuseColor|DiffuseUV";
        string m_strMain = "DoMaterial";
        [Browsable(false)]
        public string MainMethodName
        {
            get { return m_strMain; }
            //set
            //{
            //    m_strMain = value;
            //    OnPropertyChanged("MainMethodName");

            //    IsDirty = true;
            //}
        }

        bool m_bIsDirty = false;
        [System.ComponentModel.Browsable(false)]
        public bool IsDirty
        {
            get { return m_bIsDirty; }
            set
            {
                m_bIsDirty = value;

                if(!m_bIsDirty)
                {
                    mOperationQueue.Clear();
                }

                if (OnMaterialFileDirtyChange != null)
                    OnMaterialFileDirtyChange(this);

                OnPropertyChanged("IsDirty");
            }
        }

        Guid mMaterialId = Guid.NewGuid();
        [DisplayName("ID")]
        [System.ComponentModel.ReadOnly(true)]
        public Guid MaterialId
        {
            get { return mMaterialId; }
        }

        UInt32 mVer = 0;
        [System.ComponentModel.ReadOnly(true)]
        public UInt32 Ver
        {
            get { return mVer; }
            set
            {
                mVer = value;

                OnPropertyChanged("Ver");
            }
        }

        string mTechCountInfo = "0个材质实例";
        [System.ComponentModel.Browsable(false)]
        public string TechCountInfo
        {
            get { return mTechCountInfo; }
            set
            {
                mTechCountInfo = value;
                OnPropertyChanged("TechCountInfo");
            }
        }

        //public enum enDirtyType
        //{
        //    None = 0,
        //    ShaderValueChange = 1,
        //    TechniqueChange = 1 << 1,
        //    MaterialChange = 1 << 2,
        //}
        //enDirtyType m_dirtyType = enDirtyType.None;

        // mtl中材质的代码段
        string m_strMaterialCodeStringBlock = "";
        [System.ComponentModel.Browsable(false)]
        public string MaterialCodeStringBlock
        {
            get { return m_strMaterialCodeStringBlock; }
        }

        public MaterialFileInfo()
        {
            DefaultTechnique = new MaterialTechniqueInfo(mMaterialId);
            DefaultTechnique.OnDirtyChanged = _OnDefaultTechniqueDirtyChanged;
        }
        public MaterialFileInfo(Guid id)
        {
            mMaterialId = id;
            DefaultTechnique = new MaterialTechniqueInfo(mMaterialId);
            DefaultTechnique.OnDirtyChanged = _OnDefaultTechniqueDirtyChanged;
        }

        private void _OnDefaultTechniqueDirtyChanged(MaterialTechniqueInfo info)
        {
            if (info.IsDirty)
                IsDirty = true;
        }

        private void UpdateTechCountInfo()
        {
            TechCountInfo = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetMaterialChildrenInstances(MaterialId).Count + "个材质实例";
        }

        public void LoadMaterialFile(string absStrFile)
        {
            // 将绝对路径转为相对路径 todo: 修改
            //if(strFile.Contains(":"))
            mMaterialFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absStrFile, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);

            // 读文件头
            if (!System.IO.File.Exists(absStrFile))
                return;
            var rd = new System.IO.StreamReader(absStrFile, CSUtility.Support.IFileManager.GetEncoding(absStrFile));
            string strFileHead = "";
            if(rd.ReadLine() != "/*Material")
                return;
            string strTemp = rd.ReadLine();
            while (strTemp != "*/" || strTemp == null)
            {
                strFileHead += strTemp;
                strTemp = rd.ReadLine();
            }

            if (strTemp == null)
                return;

            m_strMaterialCodeStringBlock = rd.ReadToEnd();
            rd.Close();

            var xmlHolder = CSUtility.Support.XmlHolder.ParseXML(strFileHead);

            var att = xmlHolder.RootNode.FindAttrib("Name");
            if (att != null)
                m_strMatName = att.Value;
            att = xmlHolder.RootNode.FindAttrib("Require");
            if (att != null)
                m_strRequire = att.Value;
            att = xmlHolder.RootNode.FindAttrib("Main");
            if(att != null)
                m_strMain = att.Value;
            att = xmlHolder.RootNode.FindAttrib("ExtendString");
            if (att != null)
                ExtendString = att.Value;
            if (string.IsNullOrEmpty(m_strMain))
                m_strMain = "DoMaterial";
            att = xmlHolder.RootNode.FindAttrib("Id");
            if(att != null)
            {
                var idStr = att.Value;
                if (!string.IsNullOrEmpty(idStr))
                    System.Guid.TryParse(idStr, out mMaterialId);
            }
            att = xmlHolder.RootNode.FindAttrib("Ver");
            if(att != null)
            {
                var verStr = att.Value;
                if (!string.IsNullOrEmpty(verStr))
                    Ver = System.Convert.ToUInt32(verStr);
            }

            // 取得默认technique信息
            var dtNode = xmlHolder.RootNode.FindNode("DefaultTechnique");
            DefaultTechnique.Load(dtNode);

            IsDirty = false;
        }

        // 此处输入的路径是相对Resource的路径
        public void SaveMaterialFile(string strFileName)
        {
            // 每次保存更新版本号
            Ver++;

            //m_materialFileName = strFileName;

            //if(!strFileName.Contains(":"))
            //    strFileName = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(strFileName);
            //if (strFileName.Contains(":"))
            //    strFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(strFileName, CSUtility.Support.IFileManager.Instance.Root + "Shader/Material");

            System.IO.TextWriter tw = new System.IO.StringWriter();// new System.IO.StreamWriter(sfd.FileName, false, Encoding.Unicode);
            tw.WriteLine("/*Material\n");

            // 文件头            
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Material", "");
            xmlHolder.RootNode.AddAttrib("Name", m_strMatName);
            xmlHolder.RootNode.AddAttrib("Require", m_strRequire);
            xmlHolder.RootNode.AddAttrib("Main", m_strMain);

            // 计算ExtendString,保证材质UniqueId的唯一性
            var uniqueId = CCore.Material.Material.AssignUniqueIDWithString(strFileName + ExtendString);
            string tempFile;
            if (Program.MatFileUniqueIDDic.TryGetValue(uniqueId, out tempFile))
            {
                if (tempFile != strFileName)
                {
                    ExtendString = Program.GetNoRepeatUniqueIdExtString(strFileName, ExtendString);
                }
            }
            else
                Program.MatFileUniqueIDDic[uniqueId] = strFileName;

            xmlHolder.RootNode.AddAttrib("ExtendString", ExtendString);
            xmlHolder.RootNode.AddAttrib("Id", MaterialId.ToString());
            xmlHolder.RootNode.AddAttrib("Ver", Ver.ToString());

            var dtNode = xmlHolder.RootNode.AddNode("DefaultTechnique", "", xmlHolder);
            DefaultTechnique.Name = "DefaultTech";
            DefaultTechnique.IsDirty = false;
            DefaultTechnique.Save(dtNode, xmlHolder);

            string xmlString = "";
            CSUtility.Support.XmlHolder.GetXMLString(ref xmlString, xmlHolder);
            tw.WriteLine(xmlString);

            tw.WriteLine("\r\n*/\r\n");

            if (string.IsNullOrEmpty(m_strMaterialCodeStringBlock))
            {
                tw.WriteLine("#ifdef ByLayerBased");
                tw.WriteLine("void " + m_strMain + "(inout PixelMaterialTrans pssem)");
                tw.WriteLine("#else");
                tw.WriteLine("void	DoMaterial(inout PixelMaterialTrans pssem)");
                tw.WriteLine("#endif");
                tw.WriteLine("{}");
                //m_strMaterialCodeStringBlock = "void " + m_strMain + "(inout PixelMaterialTrans pssem){}";
            }
            tw.Write(m_strMaterialCodeStringBlock);

            var strFullFileName = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + strFileName;
            var file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(strFullFileName);
            var dir = strFullFileName.Replace(file, "");
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            var writer = new System.IO.StreamWriter(strFullFileName, false, System.Text.Encoding.GetEncoding("GB2312"));
            writer.Write(tw);
            writer.Close();

#warning 这里做成LongTimeProcessing
            if (mOperationQueue.Count > 0)
            {
                var techList = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetMaterialChildrenInstances(MaterialId);
                if(techList != null && techList.Count != 0)
                {
                    List<MaterialTechniqueInfo> techInfoList = new List<MaterialTechniqueInfo>();
                    foreach (var techId in techList)
                    {
                        var techFile = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(techId);
                        var absFile = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(techFile);
                        if(System.IO.File.Exists(absFile))
                        {
                            var techInfo = new MaterialTechniqueInfo(MaterialId);
                            techInfo.Load(techFile);
                            techInfoList.Add(techInfo);
                        }
                    }
                    while (mOperationQueue.Count > 0)
                    {
                        // 更新子Technique
                        var op = mOperationQueue.Dequeue();

                        foreach (var info in techInfoList)
                        {
                            switch (op.OperationType)
                            {
                                case TechniqueShaderValueOperation.enOperationType.Rename:
                                    info.ShaderValueRename(op.OldName, op.NewName);
                                    break;
                                case TechniqueShaderValueOperation.enOperationType.Add:
                                    {
                                        var svi = new CCore.Material.MaterialShaderVarInfo();
                                        svi.Copy(op.OperatedInfo);
                                        info.AddShaderValue(svi);
                                    }
                                    break;
                                case TechniqueShaderValueOperation.enOperationType.Del:
                                    {
                                        var svi = new CCore.Material.MaterialShaderVarInfo();
                                        svi.Copy(op.OperatedInfo);
                                        info.RemoveShaderValue(svi);
                                    }
                                    break;
                            }
                        }
                    }

                    foreach (var info in techInfoList)
                    {
                        var techFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetFileDictionaryFileValue(info.Id);
                        info.Save(techFile);
                    }
                }


                // 存储后等待外部设置IsDirty，因为存储有可能是另存
                //IsDirty = false;
            }
        }

        // strFileName路径为相对Resource的路径
        public void NewMaterialFile(string strFileName)
        {
            mMaterialFileName = strFileName;
            m_strMain = "DoMaterial_" + this.MaterialId.ToString().Replace("-", "_");

            System.IO.TextWriter tw = new System.IO.StringWriter();// new System.IO.StreamWriter(sfd.FileName, false, Encoding.Unicode);
            tw.WriteLine("/*Material\n");

            // 文件头
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Material", "");
            xmlHolder.RootNode.AddAttrib("Name", m_strMatName);
            xmlHolder.RootNode.AddAttrib("Require", m_strRequire);
            xmlHolder.RootNode.AddAttrib("Main", m_strMain);
            xmlHolder.RootNode.AddAttrib("ExtendString", ExtendString);
            xmlHolder.RootNode.AddAttrib("Id", MaterialId.ToString());
            xmlHolder.RootNode.AddAttrib("Ver", Ver.ToString());

            //// 至少有一个Technique
            //AddTechnique();
            // 设置默认tech
            var dtNode = xmlHolder.RootNode.AddNode("DefaultTechnique", "", xmlHolder);
            DefaultTechnique = new MaterialTechniqueInfo(MaterialId);
            DefaultTechnique.Name = "DefaultTech";
            DefaultTechnique.IsDirty = false;
            DefaultTechnique.Save(dtNode, xmlHolder);

            string xmlString = "";
            CSUtility.Support.XmlHolder.GetXMLString(ref xmlString, xmlHolder);
            tw.WriteLine(xmlString);

            tw.WriteLine("\r\n*/\r\n");

            tw.Write(m_strMaterialCodeStringBlock);

            var strFullFileName = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + strFileName;
            var writer = new System.IO.StreamWriter(strFullFileName);
            writer.Write(tw);
            writer.Close();
        }

        public void SetMaterialCodeStringBlock(string strCode)
        {
            if (string.IsNullOrEmpty(strCode))
                return;

            m_strMaterialCodeStringBlock = strCode;
        }

        public void SetRequire(string strReq)
        {
            if (string.IsNullOrEmpty(strReq))
                return;

            m_strRequire = strReq;
        }

        public void CopyFrom(MaterialFileInfo info)
        {
            m_strMatName = info.m_strMatName;
            m_strRequire = info.m_strRequire;
            m_strMain = info.m_strMain;
            m_strMaterialCodeStringBlock = info.m_strMaterialCodeStringBlock;
            //ExtendString = info.ExtendString;
            DefaultTechnique.CopyFrom(info.DefaultTechnique);
        }

#region shaderVar

        private class TechniqueShaderValueOperation
        {
            public enum enOperationType
            {
                Rename,
                Add,
                Del,
            }

            public enOperationType OperationType;
            public string OldName;
            public string NewName;

            public CCore.Material.MaterialShaderVarInfo OperatedInfo = new CCore.Material.MaterialShaderVarInfo();
        }
        Queue<TechniqueShaderValueOperation> mOperationQueue = new Queue<TechniqueShaderValueOperation>();

        // 当ShaderVar改变的时候需要设置所有相关的Technique改变其中的ShaderVar设置
        //public void OnTechniqueDirtyChanged(MaterialTechniqueInfo info)
        //{
        //    m_dirtyType |= enDirtyType.TechniqueChange;
        //}

        //public void OnShaderVarDirtyChanged(CCore.Material.MaterialShaderVarInfo info)
        //{
        //    m_dirtyType |= enDirtyType.ShaderValueChange;
        //}
        public bool ShaderValueRename(string strOldName, string strNewName)
        {
            foreach (var shaderVar in DefaultTechnique.ShaderVarInfos)
            {
                if(shaderVar.VarName == strNewName)
                    return false;
            }

            DefaultTechnique.ShaderValueRename(strOldName, strNewName);

            var op = new TechniqueShaderValueOperation()
            {
                OperationType = TechniqueShaderValueOperation.enOperationType.Rename,
                OldName = strOldName,
                NewName = strNewName,
            };
            mOperationQueue.Enqueue(op);

            return true;
        }

        public void AddShaderValue(CCore.Material.MaterialShaderVarInfo shaderVarInfo)
        {
            if (shaderVarInfo == null)
                return;

            if (!DefaultTechnique.AddShaderValue(shaderVarInfo, true))
                return;
            IsDirty = true;

            var op = new TechniqueShaderValueOperation()
            {
                OperationType = TechniqueShaderValueOperation.enOperationType.Add,
            };
            op.OperatedInfo.Copy(shaderVarInfo);
            mOperationQueue.Enqueue(op);

        }

        //public void RemoveShaderValue(string strShaderVarName)
        //{
        //    IsDirty = true;

        //    foreach (var techInfo in m_MaterialTechniqueInfoDic.Values)
        //    {
        //        techInfo.RemoveShaderValue(strShaderVarName);
        //    }
        //}

        public void RemoveShaderValue(CCore.Material.MaterialShaderVarInfo shaderVarInfo)
        {
            if (shaderVarInfo == null)
                return;

            IsDirty = true;
            //m_dirtyType |= enDirtyType.MaterialChange;
            DefaultTechnique.RemoveShaderValue(shaderVarInfo);

            var op = new TechniqueShaderValueOperation()
            {
                OperationType = TechniqueShaderValueOperation.enOperationType.Del,
            };
            op.OperatedInfo.Copy(shaderVarInfo);
            mOperationQueue.Enqueue(op);

        }

        //public void ClearShaderValues()
        //{
            //IsDirty = true;

            //foreach (var techInfo in m_MaterialTechniqueInfoDic.Values)
            //{
            //    techInfo.ShaderVarInfos.Clear();
            //}
        //}

#endregion

    }
}
