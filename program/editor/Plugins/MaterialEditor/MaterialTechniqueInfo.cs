using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MaterialEditor
{
    public class MaterialTechniqueInfo : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //public static string TechniquesFolder = "Techniques";

        public delegate void Delegate_OnDirtyChanged(MaterialTechniqueInfo info);
        public Delegate_OnDirtyChanged OnDirtyChanged;

        public CCore.Material.MaterialShaderVarInfo.Delegate_OnReName OnReName;

        // 路径为相对于父Material的路径
        //public string mFileName = "";

        bool m_bIsDirty = false;
        [Browsable(false)]
        public bool IsDirty
        {
            get { return m_bIsDirty; }
            set
            {
                m_bIsDirty = value;

                if (OnDirtyChanged != null)
                    OnDirtyChanged(this);

                OnPropertyChanged("IsDirty");
            }
        }

        UInt32 mVer = 0;
        [ReadOnly(true)]
        public UInt32 Ver
        {
            get { return mVer; }
            set
            {
                mVer = value;

                IsDirty = true;

                OnPropertyChanged("Ver");
            }
        }

        //[DefaultParameterValueAttribute("Tech")]
        string m_Name = "Tech";
        public string Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;

                IsDirty = true;

                OnPropertyChanged("Name");
            }
        }

        //[DefaultParameterValueAttribute(0)]
        UInt64 m_AlphaRef = 0;
        public UInt64 AlphaRef
        {
            get { return m_AlphaRef; }
            set
            {
                m_AlphaRef = value;

                IsDirty = true;
            }
        }

        public enum enCullMode
        {
            NONE,
            CW,
            CCW,
        }

        //[DefaultParameterValueAttribute(enCullMode.CCW)]
        enCullMode m_CullMode = enCullMode.CCW;
        public enCullMode CullMode
        {
            get { return m_CullMode; }
            set
            {
                m_CullMode = value;

                IsDirty = true;
                OnPropertyChanged("CullMode");
            }
        }

        //[DefaultParameterValueAttribute(false)]
        bool m_bZDisable = false;
        public bool ZDisable
        {
            get { return m_bZDisable; }
            set
            {
                m_bZDisable = value;

                IsDirty = true;
                OnPropertyChanged("ZDisable");
            }
        }

        //[DefaultParameterValueAttribute(false)]
        bool m_bZWriteDisable = false;
        public bool ZWriteDisable
        {
            get { return m_bZWriteDisable; }
            set
            {
                m_bZWriteDisable = value;

                IsDirty = true;
                OnPropertyChanged("ZWriteDisable");
            }
        }

        //[DefaultParameterValueAttribute(false)]
        bool m_bWireFram = false;
        public bool WireFrame
        {
            get { return m_bWireFram; }
            set
            {
                m_bWireFram = value;

                IsDirty = true;
                OnPropertyChanged("WireFrame");
            }
        }

        bool m_bUseSystemMaterialFilterType = true;
        public bool UseSystemMaterialFilterType
        {
            get { return m_bUseSystemMaterialFilterType; }
            set
            {
                m_bUseSystemMaterialFilterType = value;

                IsDirty = true;
                OnPropertyChanged("UseSystemMaterialFilterType");
            }
        }

        public enum enAlphaType
        {
            Disable,
            OnlyTest,
            TestBlend,
            OnlyBlend,
        }
        //[DefaultParameterValueAttribute(enAlphaType.Disable)]
        enAlphaType m_AlphaType = enAlphaType.Disable;
        public enAlphaType AlphaType
        {
            get { return m_AlphaType; }
            set
            {
                m_AlphaType = value;

                IsDirty = true;
                OnPropertyChanged("AlphaType");
            }
        }

        public enum enBlendOp {
            Add = 1,
            Subtract = 2,
            RevSubtract = 3,
            Min = 4,
            Max = 5,
        }
        enBlendOp mBlendOp = enBlendOp.Add;
        public enBlendOp BlendOp
        {
            get { return mBlendOp; }
            set
            {
                mBlendOp = value;

                IsDirty = true;
                OnPropertyChanged("BlendOp");
            }
        }

        public enum enBlendArg {
            Zero = 1,
            One = 2,
            SrcColor = 3,
            InvSrcColor = 4,
            SrcAlpha = 5,
            InvSrcAlpha = 6,
            DestAlpha = 7,
            InvDestAlpha = 8,
            DestColor = 9,
            InvDestColor = 10,
            SrcAlphaSat = 11,
            BothSrcAlpha = 12,
            BothInvSrcAlpha = 13,
        }
        enBlendArg mBlendSrc = enBlendArg.SrcAlpha;
        public enBlendArg BlendSrc
        {
            get { return mBlendSrc; }
            set
            {
                mBlendSrc = value;

                IsDirty = true;
                OnPropertyChanged("BlendSrc");
            }
        }
        enBlendArg mBlendDest = enBlendArg.InvSrcAlpha;
        public enBlendArg BlendDest
        {
            get { return mBlendDest; }
            set
            {
                mBlendDest = value;

                IsDirty = true;
                OnPropertyChanged("BlendDest");
            }
        }

        public enum enBillboardType
        {
            Disable,
            Free,
            LockY,
        }
        enBillboardType m_BillboardType = enBillboardType.Disable;
        public enBillboardType BillboardType
        {
            get { return m_BillboardType; }
            set
            {
                m_BillboardType = value;

                IsDirty = true;
                OnPropertyChanged("BillboardType");
            }
        }

        public enum enZCmpFunc
        {
            LessEqual,
            Greater,
            Always,
            Never,
        }
        enZCmpFunc m_ZCmpFunc = enZCmpFunc.LessEqual;
        public enZCmpFunc ZCmpFunc
        {
            get { return m_ZCmpFunc; }
            set
            {
                m_ZCmpFunc = value;
                IsDirty = true;
                OnPropertyChanged("ZCmpFunc");
            }
        }

        public enum enCmpFunc {
            Never                = 1,
            Less                 = 2,
            Equal                = 3,
            Lessequal            = 4,
            Greater              = 5,
            Notequal             = 6,
            Greaterequal         = 7,
            Always               = 8,
        }
        public enum enStencilOp {
            Keep           = 1,
            Zero           = 2,
            Replace        = 3,
            Incrsat        = 4,
            Decrsat        = 5,
            Invert         = 6,
            Incr           = 7,
            Decr           = 8,
        }
        bool mStencilEnable = true;
        [System.ComponentModel.Category("Stencil")]
        public bool StencilEnable
        {
            get { return mStencilEnable; }
            set
            {
                mStencilEnable = value;
                IsDirty = true;
                OnPropertyChanged("StencilEnable");
            }
        }
        enCmpFunc mStencilCmp = enCmpFunc.Always;
        [System.ComponentModel.Category("Stencil")]
        public enCmpFunc StencilCmp
        {
            get { return mStencilCmp; }
            set
            {
                mStencilCmp = value;
                IsDirty = true;
                OnPropertyChanged("StencilCmp");
            }
        }
        UInt64 mStencilRef = 0x1;
        [System.ComponentModel.Category("Stencil")]
        [CSUtility.Editor.Editor_HexAttribute]
        public UInt64 StencilRef
        {
            get { return mStencilRef; }
            set
            {
                mStencilRef = value;
                IsDirty = true;
                OnPropertyChanged("StencilRef");
            }
        }
        UInt64 mStencilMask = 0x00000000;
        [System.ComponentModel.Category("Stencil")]
        [CSUtility.Editor.Editor_HexAttribute]
        public UInt64 StencilMask
        {
            get { return mStencilMask; }
            set
            {
                mStencilMask = value;
                IsDirty = true;
                OnPropertyChanged("StencilMask");
            }
        }
        UInt64 mStencilWriteMask = 0x00000000;
        [System.ComponentModel.Category("Stencil")]
        [CSUtility.Editor.Editor_HexAttribute]
        public UInt64 StencilWriteMask
        {
            get { return mStencilWriteMask; }
            set
            {
                mStencilWriteMask = value;
                IsDirty = true;
                OnPropertyChanged("StencilWriteMask");
            }
        }
        enStencilOp mStencilFailOp = enStencilOp.Keep;
        [System.ComponentModel.Category("Stencil")]
        public enStencilOp StencilFailOp
        {
            get { return mStencilFailOp; }
            set
            {
                mStencilFailOp = value;
                IsDirty = true;
                OnPropertyChanged("StencilFailOp");
            }
        }
        enStencilOp mStencilZFailOp = enStencilOp.Keep;
        [System.ComponentModel.Category("Stencil")]
        public enStencilOp StencilZFailOp
        {
            get { return mStencilZFailOp; }
            set
            {
                mStencilZFailOp = value;
                IsDirty = true;
                OnPropertyChanged("StencilZFailOp");
            }
        }
        enStencilOp mStencilPassOp = enStencilOp.Replace;
        [System.ComponentModel.Category("Stencil")]
        public enStencilOp StencilPassOp
        {
            get { return mStencilPassOp; }
            set
            {
                mStencilPassOp = value;
                IsDirty = true;
                OnPropertyChanged("StencilPassOp");
            }
        }

        float mDepthBias = 0;
        [System.ComponentModel.Category("DepthBias")]
        public float DepthBias
        {
            get { return mDepthBias; }
            set
            {
                mDepthBias = value;
                IsDirty = true;
                OnPropertyChanged("DepthBias");
            }
        }

        float mSlopeScaleDepthBias = 0;
        [System.ComponentModel.Category("DepthBias")]
        public float SlopeScaleDepthBias
        {
            get { return mSlopeScaleDepthBias; }
            set
            {
                mSlopeScaleDepthBias = value;
                IsDirty = true;
                OnPropertyChanged("SlopeScaleDepthBias");
            }
        }

        bool mIsDecal = false;
        [DisplayName("是否为Decal材质")]
        public bool IsDecal
        {
            get { return mIsDecal; }
            set
            {
                if (mIsDecal == value)
                    return;

                mIsDecal = value;
                if (mIsDecal == false)
                {
                    AcceptDecal = AcceptDecal;
                }
                else
                {
                    StencilEnable = true;
                    StencilCmp = enCmpFunc.Equal;
                    StencilMask = StencilMask | 0x0000000f;
                    StencilRef = StencilRef & 0x00000000;
                    StencilPassOp = enStencilOp.Keep;
                }

                IsDirty = true;
                OnPropertyChanged("IsDecal");
            }
        }

        bool mAcceptDecal = false;
        [DisplayName("接受Decal")]
        public bool AcceptDecal
        {
            get { return mAcceptDecal; }
            set
            {
                if (mAcceptDecal == value)
                    return;

                mAcceptDecal = value;
                if (mAcceptDecal == false)
                {
                    StencilEnable = true;
                    StencilCmp = enCmpFunc.Always;
                    StencilWriteMask = StencilWriteMask | 0x0000000f;
                    StencilRef = StencilRef | 0x00000001;
                    StencilPassOp = enStencilOp.Replace;
                }
                else
                {
                    StencilEnable = true;
                    StencilCmp = enCmpFunc.Always;
                    StencilWriteMask = StencilWriteMask | 0x0000000f;
                    StencilRef = StencilRef & 0xFFFFFFF0;
                    StencilPassOp = enStencilOp.Replace;
                }

                IsDirty = true;
                OnPropertyChanged("AcceptDecal");
            }
        }

        bool mAcceptLight = true;
        [DisplayName("接受光照")]
        public bool AcceptLight
        {
            get { return mAcceptLight; }
            set
            {
                if (mAcceptLight == value)
                    return;

                mAcceptLight = value;
                if (mAcceptLight == false)
                {
                    StencilEnable = true;
                    StencilCmp = enCmpFunc.Always;
                    StencilWriteMask = StencilWriteMask | 0x000000f0;
                    StencilRef = StencilRef | 0x00000010;
                    StencilPassOp = enStencilOp.Replace;
                }
                else
                {
                    StencilEnable = true;
                    StencilCmp = enCmpFunc.Always;
                    StencilWriteMask = StencilWriteMask | 0x000000f0;
                    StencilRef = StencilRef & 0xFFFFFF0F;
                    StencilPassOp = enStencilOp.Replace;
                }

                IsDirty = true;
                OnPropertyChanged("AcceptLight");
            }
        }


        Guid mHostMaterialId = Guid.Empty;
        public Guid HostMaterialId
        {
            get { return mHostMaterialId; }
            set
            {
                mHostMaterialId = value;

                OnPropertyChanged("HostMaterialId");
            }
        }

        public void CopyFromMaterialDefaultTechnique(Guid materialId)
        {
            var matFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + CCore.Engine.Instance.Client.Graphics.MaterialMgr.FindMaterialFile(materialId, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);
            var matFileInfo = new MaterialFileInfo(materialId);
            matFileInfo.LoadMaterialFile(matFile);
            CopyFrom(matFileInfo.DefaultTechnique);
        }

        Guid mId = Guid.NewGuid();
        public Guid Id
        {
            get { return mId; }
            set
            {
                mId = value;
            }
        }

        List<CCore.Material.MaterialShaderVarInfo> m_ShaderVarInfos = new List<CCore.Material.MaterialShaderVarInfo>();
        [DisplayName("材质参数")]
        //[CSUtility.Editor.Editor_Material_ShaderValueAttribute]
        [CSUtility.Editor.Editor_PropertyGridDataTemplateAttribute("MaterialShaderValue")]
        public List<CCore.Material.MaterialShaderVarInfo> ShaderVarInfos
        {
            get { return m_ShaderVarInfos; }
            set
            {
                m_ShaderVarInfos = value;

                IsDirty = true;
            }
        }

        public MaterialTechniqueInfo(Guid hostMaterialId)
        {
            StencilEnable = true;
            StencilCmp = enCmpFunc.Always;
            StencilWriteMask = StencilWriteMask | 0x000000ff;
            StencilRef = StencilRef | 0x00000001;
            StencilPassOp = enStencilOp.Replace;
            mHostMaterialId = hostMaterialId;
            //ShaderVarInfos = new List<CCore.Material.MaterialShaderVarInfo>();
        }

        //protected void OnShaderVarRename(CCore.Material.MaterialShaderVarInfo svInfo, string oldName, string newName)
        //{
        //    if (OnReName != null)
        //        OnReName(svInfo, oldName, newName);
        //}

        public void ShaderValueRename(string oldName, string newName)
        {
            foreach (var svInfo in ShaderVarInfos)
            {
                if (svInfo.VarName == oldName)
                {
                    svInfo.VarName = newName;
                }
            }

            IsDirty = true;
        }

        public bool AddShaderValue(CCore.Material.MaterialShaderVarInfo shaderVarInfo, [System.Runtime.InteropServices.Optional, System.Runtime.InteropServices.DefaultParameterValue(false)]bool bReplace)
        {
            IsDirty = true;

            for (int i = 0; i < ShaderVarInfos.Count; ++i)
            {
                if (shaderVarInfo.VarName == ShaderVarInfos[i].VarName)
                {
                    if (bReplace)
                    {
                        ShaderVarInfos[i].OnDirtyChanged = null;
                        shaderVarInfo.OnDirtyChanged = new CCore.Material.MaterialShaderVarInfo.Delegate_OnDirtyChanged(_OnShaderVarDirtyChanged);
                        ShaderVarInfos[i] = shaderVarInfo;
                        return true;
                    }
                    return false;
                }
            }
            //foreach (var info in ShaderVarInfos)
            //{
            //    if (info.VarName == shaderVarInfo.VarName)
            //    {
            //        info.Copy(shaderVarInfo);
            //        return;
            //    }
            //}

            shaderVarInfo.OnDirtyChanged = new CCore.Material.MaterialShaderVarInfo.Delegate_OnDirtyChanged(_OnShaderVarDirtyChanged);
            ShaderVarInfos.Add(shaderVarInfo);
            //shaderVarInfo.OnReName = new CCore.Material.MaterialShaderVarInfo.Delegate_OnReName(OnShaderVarRename);
            return true;
        }

        //public void RemoveShaderValue(string strName)
        //{
        //    IsDirty = true;

        //    foreach (var shaVar in ShaderVarInfos)
        //    {
        //        if (shaVar.VarName == strName)
        //        {
        //            ShaderVarInfos.Remove(shaVar);
        //            break;
        //        }
        //    }
        //}

        public void RemoveShaderValue(CCore.Material.MaterialShaderVarInfo info)
        {
            foreach (var shaderVar in ShaderVarInfos)
            {
                if (shaderVar.VarName == info.VarName)
                {
                    shaderVar.OnDirtyChanged = null;
                    ShaderVarInfos.Remove(shaderVar);
                    break;
                }
            }
            //ShaderVarInfos.Remove(info);

            IsDirty = true;
        }

        private void _OnShaderVarDirtyChanged(CCore.Material.MaterialShaderVarInfo info)
        {
            if (info.IsDirty)
                IsDirty = info.IsDirty;
        }

        //public void RemoveShaderValue(CodeGenerateSystem.Base.BaseNodeControl node)
        //{
        //    string strVarName = "";
        //    if(node is Controls.TextureControl)
        //        strVarName = ((Controls.TextureControl)node).GetTextureName();
        //    else if(node is Controls.CommonValueControl)
        //        strVarName = node.GCode_GetValueName(null);

        //    RemoveShaderValue(strVarName);
        //}

        ////////////public void Load(System.Xml.XmlElement xmlNode)
        ////////////{
        ////////////    Name = xmlNode.GetAttribute("Name");
        ////////////    var tempStr = xmlNode.GetAttribute("Ver");
        ////////////    if (!string.IsNullOrEmpty(tempStr))
        ////////////    {
        ////////////        Ver = System.Convert.ToUInt32(tempStr);
        ////////////    }
        ////////////    AlphaRef = System.Convert.ToUInt64(xmlNode.GetAttribute("AlphaRef"));
        ////////////    CullMode = (MaterialTechniqueInfo.enCullMode)System.Enum.Parse(typeof(MaterialTechniqueInfo.enCullMode), xmlNode.GetAttribute("CullMode"));
        ////////////    ZDisable = System.Convert.ToInt32(xmlNode.GetAttribute("ZDisable")) > 0 ? true : false;
        ////////////    ZWriteDisable = System.Convert.ToInt32(xmlNode.GetAttribute("ZWriteDisable")) > 0 ? true : false;
        ////////////    WireFrame = System.Convert.ToInt32(xmlNode.GetAttribute("WireFrame")) > 0 ? true : false;
        ////////////    AlphaType = (MaterialTechniqueInfo.enAlphaType)System.Enum.Parse(typeof(MaterialTechniqueInfo.enAlphaType), xmlNode.GetAttribute("AlphaType"));
        ////////////    string attrValue = xmlNode.GetAttribute("BlendOp");
        ////////////    if (attrValue != "")
        ////////////        BlendOp = (MaterialTechniqueInfo.enBlendOp)System.Enum.Parse(typeof(MaterialTechniqueInfo.enBlendOp), attrValue);
        ////////////    attrValue = xmlNode.GetAttribute("UseSystemMaterialFilterType");
        ////////////    if (attrValue != "")
        ////////////        UseSystemMaterialFilterType = System.Convert.ToInt32(attrValue) > 0 ? true : false;
        ////////////    attrValue = xmlNode.GetAttribute("BlendSrc");
        ////////////    if (attrValue != "")
        ////////////        BlendSrc = (MaterialTechniqueInfo.enBlendArg)System.Enum.Parse(typeof(MaterialTechniqueInfo.enBlendArg), attrValue);
        ////////////    attrValue = xmlNode.GetAttribute("BlendDest");
        ////////////    if (attrValue != "")
        ////////////        BlendDest = (MaterialTechniqueInfo.enBlendArg)System.Enum.Parse(typeof(MaterialTechniqueInfo.enBlendArg), attrValue);
        ////////////    attrValue = xmlNode.GetAttribute("BillboardType");
        ////////////    if (attrValue != "")
        ////////////        BillboardType = (MaterialTechniqueInfo.enBillboardType)System.Enum.Parse(typeof(MaterialTechniqueInfo.enBillboardType), attrValue);
        ////////////    if (xmlNode.GetAttribute("ZCmpFunc") != "")
        ////////////        ZCmpFunc = (MaterialTechniqueInfo.enZCmpFunc)System.Enum.Parse(typeof(MaterialTechniqueInfo.enZCmpFunc), xmlNode.GetAttribute("ZCmpFunc"));
        ////////////    if (xmlNode.GetAttribute("StencilEnable") != "")
        ////////////        StencilEnable = System.Convert.ToInt32(xmlNode.GetAttribute("StencilEnable")) > 0 ? true : false;
        ////////////    if (xmlNode.GetAttribute("StencilCmp") != "")
        ////////////        StencilCmp = (MaterialTechniqueInfo.enCmpFunc)System.Enum.Parse(typeof(MaterialTechniqueInfo.enCmpFunc), xmlNode.GetAttribute("StencilCmp"));
        ////////////    if (xmlNode.GetAttribute("StencilRef") != "")
        ////////////        StencilRef = System.Convert.ToUInt64(xmlNode.GetAttribute("StencilRef"));
        ////////////    if (xmlNode.GetAttribute("StencilMask") != "")
        ////////////        StencilMask = System.Convert.ToUInt64(xmlNode.GetAttribute("StencilMask"));
        ////////////    if (xmlNode.GetAttribute("StencilWriteMask") != "")
        ////////////        StencilWriteMask = System.Convert.ToUInt64(xmlNode.GetAttribute("StencilWriteMask"));
        ////////////    if (xmlNode.GetAttribute("StencilFailOp") != "")
        ////////////        StencilFailOp = (MaterialTechniqueInfo.enStencilOp)System.Enum.Parse(typeof(MaterialTechniqueInfo.enStencilOp), xmlNode.GetAttribute("StencilFailOp"));
        ////////////    if (xmlNode.GetAttribute("StencilZFailOp") != "")
        ////////////        StencilZFailOp = (MaterialTechniqueInfo.enStencilOp)System.Enum.Parse(typeof(MaterialTechniqueInfo.enStencilOp), xmlNode.GetAttribute("StencilZFailOp"));
        ////////////    if (xmlNode.GetAttribute("StencilPassOp") != "")
        ////////////        StencilPassOp = (MaterialTechniqueInfo.enStencilOp)System.Enum.Parse(typeof(MaterialTechniqueInfo.enStencilOp), xmlNode.GetAttribute("StencilPassOp"));
        ////////////    if (xmlNode.GetAttribute("DepthBias") != "")
        ////////////        DepthBias = System.Convert.ToSingle(xmlNode.GetAttribute("DepthBias"));
        ////////////    if (xmlNode.GetAttribute("SlopeScaleDepthBias") != "")
        ////////////        SlopeScaleDepthBias = System.Convert.ToSingle(xmlNode.GetAttribute("SlopeScaleDepthBias"));
        ////////////    // AcceptDecal属性赋值过程中设置了Stencil的部分渲染状态，放在最后在赋值。
        ////////////    if (xmlNode.GetAttribute("AcceptDecal") != "")
        ////////////        AcceptDecal = System.Convert.ToInt32(xmlNode.GetAttribute("AcceptDecal")) > 0 ? true : false;
        ////////////    if (xmlNode.GetAttribute("AcceptLight") != "")
        ////////////        AcceptLight = System.Convert.ToInt32(xmlNode.GetAttribute("AcceptLight")) > 0 ? true : false;
        ////////////    var hostIdStr = xmlNode.GetAttribute("HostMaterialId");
        ////////////    System.Guid.TryParse(hostIdStr, out mHostMaterialId);
        ////////////    var idStr = xmlNode.GetAttribute("Id");
        ////////////    System.Guid.TryParse(idStr, out mId);

        ////////////    var shaderVarElements = xmlNode.GetElementsByTagName("ShaderVar");
        ////////////    if (shaderVarElements.Count > 0)
        ////////////    {
        ////////////        foreach (System.Xml.XmlElement svNode in shaderVarElements[0].ChildNodes)
        ////////////        {
        ////////////            CCore.Material.MaterialShaderVarInfo shaderVarInfo = new CCore.Material.MaterialShaderVarInfo()
        ////////////            {
        ////////////                PreString = svNode.GetAttribute("PreString"),
        ////////////                EditorType = svNode.GetAttribute("EditorType"),
        ////////////                VarName = svNode.Name,
        ////////////                VarType = svNode.GetAttribute("Type"),
        ////////////                VarValue = svNode.GetAttribute("Value"),
        ////////////            };
        ////////////            AddShaderValue(shaderVarInfo);
        ////////////        }
        ////////////    }

        ////////////    IsDirty = false;
        ////////////}

        ////////////public void Save(System.Xml.XmlElement xmlNode, System.Xml.XmlDocument xmlDoc)
        ////////////{
        ////////////    // 存储时增加版本号
        ////////////    Ver++;

        ////////////    xmlNode.SetAttribute("Name", Name);
        ////////////    xmlNode.SetAttribute("Ver", Ver.ToString());
        ////////////    xmlNode.SetAttribute("AlphaRef", AlphaRef.ToString());
        ////////////    xmlNode.SetAttribute("CullMode", CullMode.ToString());
        ////////////    xmlNode.SetAttribute("ZDisable", (ZDisable) ? "1" : "0");
        ////////////    xmlNode.SetAttribute("ZWriteDisable", (ZWriteDisable) ? "1" : "0");
        ////////////    xmlNode.SetAttribute("WireFrame", (WireFrame) ? "1" : "0");
        ////////////    xmlNode.SetAttribute("UseSystemMaterialFilterType", (UseSystemMaterialFilterType) ? "1" : "0");            
        ////////////    xmlNode.SetAttribute("AlphaType", AlphaType.ToString());
        ////////////    xmlNode.SetAttribute("BlendOp", BlendOp.ToString());
        ////////////    xmlNode.SetAttribute("BlendSrc", BlendSrc.ToString());
        ////////////    xmlNode.SetAttribute("BlendDest", BlendDest.ToString());
        ////////////    xmlNode.SetAttribute("BillboardType", BillboardType.ToString());
        ////////////    xmlNode.SetAttribute("ZCmpFunc", ZCmpFunc.ToString());
        ////////////    xmlNode.SetAttribute("HostMaterialId", mHostMaterialId.ToString());
        ////////////    xmlNode.SetAttribute("Id", mId.ToString());
        ////////////    //xmlDoc.DocumentElement.AppendChild(xmlNode);
        ////////////    xmlNode.SetAttribute("AcceptDecal", (AcceptDecal) ? "1" : "0");
        ////////////    xmlNode.SetAttribute("AcceptLight", (AcceptLight) ? "1" : "0");
        ////////////    xmlNode.SetAttribute("StencilEnable", (StencilEnable) ? "1" : "0");
        ////////////    xmlNode.SetAttribute("StencilCmp", StencilCmp.ToString());
        ////////////    xmlNode.SetAttribute("StencilRef", StencilRef.ToString());
        ////////////    xmlNode.SetAttribute("StencilMask", StencilMask.ToString());
        ////////////    xmlNode.SetAttribute("StencilWriteMask", StencilWriteMask.ToString());
        ////////////    xmlNode.SetAttribute("StencilFailOp", StencilFailOp.ToString());
        ////////////    xmlNode.SetAttribute("StencilZFailOp", StencilZFailOp.ToString());
        ////////////    xmlNode.SetAttribute("StencilPassOp", StencilPassOp.ToString());
        ////////////    xmlNode.SetAttribute("DepthBias", DepthBias.ToString());
        ////////////    xmlNode.SetAttribute("SlopeScaleDepthBias", SlopeScaleDepthBias.ToString());

        ////////////    var shaderValElement = xmlDoc.CreateElement("ShaderVar");
        ////////////    xmlNode.AppendChild(shaderValElement);
        ////////////    foreach (var node in ShaderVarInfos)
        ////////////    {
        ////////////        var varElement = xmlDoc.CreateElement(node.VarName);
        ////////////        varElement.SetAttribute("PreString", node.PreString);
        ////////////        varElement.SetAttribute("EditorType", node.EditorType);
        ////////////        varElement.SetAttribute("Type", node.VarType);
        ////////////        varElement.SetAttribute("Value", node.VarValue);
        ////////////        shaderValElement.AppendChild(varElement);
        ////////////    }

        ////////////    IsDirty = false;
        ////////////}

        public void Load(CSUtility.Support.XmlNode node)
        {
            var att = node.FindAttrib("Name");
            if (att != null)
                Name = att.Value;
            att = node.FindAttrib("Ver");
            if (att != null)
                Ver = System.Convert.ToUInt32(att.Value);
            att = node.FindAttrib("AlphaRef");
            if (att != null)
                AlphaRef = System.Convert.ToUInt64(att.Value);
            att = node.FindAttrib("CullMode");
            if (att != null)
                CullMode = (MaterialTechniqueInfo.enCullMode)System.Enum.Parse(typeof(MaterialTechniqueInfo.enCullMode), att.Value);
            att = node.FindAttrib("ZDisable");
            if (att != null)
                ZDisable = System.Convert.ToInt32(att.Value) > 0 ? true : false;
            att = node.FindAttrib("ZWriteDisable");
            if (att != null)
                ZWriteDisable = System.Convert.ToInt32(att.Value) > 0 ? true : false;
            att = node.FindAttrib("WireFrame");
            if (att != null)
                WireFrame = System.Convert.ToInt32(att.Value) > 0 ? true : false;
            att = node.FindAttrib("UseSystemMaterialFilterType");
            if (att != null)
                UseSystemMaterialFilterType = System.Convert.ToInt32(att.Value) > 0 ? true : false;
            att = node.FindAttrib("AlphaType");
            if (att != null)
                AlphaType = (MaterialTechniqueInfo.enAlphaType)System.Enum.Parse(typeof(MaterialTechniqueInfo.enAlphaType), att.Value);
            att = node.FindAttrib("BlendOp");
            if (att != null)
            {
                BlendOp = (MaterialTechniqueInfo.enBlendOp)System.Enum.Parse(typeof(MaterialTechniqueInfo.enBlendOp), att.Value);
            }
            att = node.FindAttrib("BlendSrc");
            if (att != null)
            {
                BlendSrc = (MaterialTechniqueInfo.enBlendArg)System.Enum.Parse(typeof(MaterialTechniqueInfo.enBlendArg), att.Value);
            }
            att = node.FindAttrib("BlendDest");
            if (att != null)
            {
                BlendDest = (MaterialTechniqueInfo.enBlendArg)System.Enum.Parse(typeof(MaterialTechniqueInfo.enBlendArg), att.Value);
            }
            att = node.FindAttrib("BillboardType");
            if (att != null)
                BillboardType = (MaterialTechniqueInfo.enBillboardType)System.Enum.Parse(typeof(MaterialTechniqueInfo.enBillboardType), att.Value);
            att = node.FindAttrib("ZCmpFunc");
            if (att != null)
                ZCmpFunc = (MaterialTechniqueInfo.enZCmpFunc)System.Enum.Parse(typeof(MaterialTechniqueInfo.enZCmpFunc), att.Value);
            att = node.FindAttrib("StencilEnable");
            if (att != null)
                StencilEnable = System.Convert.ToInt32(att.Value) > 0 ? true : false;
            att = node.FindAttrib("StencilCmp");
            if (att != null)
                StencilCmp = (MaterialTechniqueInfo.enCmpFunc)System.Enum.Parse(typeof(MaterialTechniqueInfo.enCmpFunc), att.Value);
            att = node.FindAttrib("StencilRef");
            if (att != null)
                StencilRef = System.Convert.ToUInt64(att.Value);
            att = node.FindAttrib("StencilMask");
            if (att != null)
                StencilMask = System.Convert.ToUInt64(att.Value);
            att = node.FindAttrib("StencilWriteMask");
            if (att != null)
                StencilWriteMask = System.Convert.ToUInt64(att.Value);
            att = node.FindAttrib("StencilFailOp");
            if (att != null)
                StencilFailOp = (MaterialTechniqueInfo.enStencilOp)System.Enum.Parse(typeof(MaterialTechniqueInfo.enStencilOp), att.Value);
            att = node.FindAttrib("StencilZFailOp");
            if (att != null)
                StencilZFailOp = (MaterialTechniqueInfo.enStencilOp)System.Enum.Parse(typeof(MaterialTechniqueInfo.enStencilOp), att.Value);
            att = node.FindAttrib("StencilPassOp");
            if (att != null)
                StencilPassOp = (MaterialTechniqueInfo.enStencilOp)System.Enum.Parse(typeof(MaterialTechniqueInfo.enStencilOp), att.Value);
            att = node.FindAttrib("DepthBias");
            if (att != null)
                DepthBias = System.Convert.ToSingle(att.Value);
            att = node.FindAttrib("SlopeScaleDepthBias");
            if (att != null)
                SlopeScaleDepthBias = System.Convert.ToSingle(att.Value);
            att = node.FindAttrib("AcceptDecal");
            if (att != null)
                AcceptDecal = System.Convert.ToInt32(att.Value) > 0 ? true : false;
            att = node.FindAttrib("AcceptLight");
            if (att != null)
                AcceptLight = System.Convert.ToInt32(att.Value) > 0 ? true : false;

            att = node.FindAttrib("HostMaterialId");
            if (att != null)
                System.Guid.TryParse(att.Value, out mHostMaterialId);
            att = node.FindAttrib("Id");
            if (att != null)
                System.Guid.TryParse(att.Value, out mId);

            foreach (var varInfo in ShaderVarInfos)
                varInfo.OnDirtyChanged = null;
            ShaderVarInfos.Clear();
            var shaderVarNode = node.FindNode("ShaderVar");
            foreach (var svNode in shaderVarNode.GetNodes())
            {
                var shaderVarInfo = new CCore.Material.MaterialShaderVarInfo();
                shaderVarInfo.Load(svNode);

                AddShaderValue(shaderVarInfo);
            }

            IsDirty = false;
        }

        /// <summary>
        /// 读取材质实例文件
        /// </summary>
        /// <param name="strFile">材质实例路径，路径相对于release</param>
        public void Load(string strFile)
        {
            if (strFile.Contains(":"))
                strFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(strFile, CSUtility.Support.IFileManager.Instance.Root);

            //mFileName = strFile.Substring(strFile.IndexOf(MaterialTechniqueInfo.TechniquesFolder));

            CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.LoadXML(strFile);
            Load(xmlHolder.RootNode);
        }

        public void Save(CSUtility.Support.XmlNode techNode, CSUtility.Support.XmlHolder xmlHolder)
        {
            techNode.AddAttrib("Name", Name);
            techNode.AddAttrib("Ver", Ver.ToString());
            techNode.AddAttrib("AlphaRef", AlphaRef.ToString());
            techNode.AddAttrib("CullMode", CullMode.ToString());
            techNode.AddAttrib("ZDisable", (ZDisable) ? "1" : "0");
            techNode.AddAttrib("ZWriteDisable", (ZWriteDisable) ? "1" : "0");
            techNode.AddAttrib("WireFrame", (WireFrame) ? "1" : "0");
            techNode.AddAttrib("UseSystemMaterialFilterType", (UseSystemMaterialFilterType) ? "1" : "0");
            techNode.AddAttrib("AlphaType", AlphaType.ToString());
            techNode.AddAttrib("BlendOp", BlendOp.ToString());
            techNode.AddAttrib("BlendSrc", BlendSrc.ToString());
            techNode.AddAttrib("BlendDest", BlendDest.ToString());
            techNode.AddAttrib("BillboardType", BillboardType.ToString());
            techNode.AddAttrib("ZCmpFunc", ZCmpFunc.ToString());
            techNode.AddAttrib("AcceptDecal", (AcceptDecal) ? "1" : "0");
            techNode.AddAttrib("AcceptLight", (AcceptLight) ? "1" : "0");
            techNode.AddAttrib("StencilEnable", (StencilEnable) ? "1" : "0");
            techNode.AddAttrib("StencilCmp", StencilCmp.ToString());
            techNode.AddAttrib("StencilRef", StencilRef.ToString());
            techNode.AddAttrib("StencilMask", StencilMask.ToString());
            techNode.AddAttrib("StencilWriteMask", StencilWriteMask.ToString());
            techNode.AddAttrib("StencilFailOp", StencilFailOp.ToString());
            techNode.AddAttrib("StencilZFailOp", StencilZFailOp.ToString());
            techNode.AddAttrib("StencilPassOp", StencilPassOp.ToString());
            techNode.AddAttrib("HostMaterialId", mHostMaterialId.ToString());
            techNode.AddAttrib("Id", mId.ToString());
            techNode.AddAttrib("DepthBias", DepthBias.ToString());
            techNode.AddAttrib("SlopeScaleDepthBias", SlopeScaleDepthBias.ToString());

            var shaderValNode = techNode.AddNode("ShaderVar", "", xmlHolder);
            foreach (var node in ShaderVarInfos)
            {
                var varNode = shaderValNode.AddNode(node.VarName, "", xmlHolder);
                node.Save(varNode);
            }
        }

        // absStrFile为绝对路径
        public void Save(string absStrFile, bool withSVN = false)
        {
            var absFileName = absStrFile;
            var dir = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(absStrFile);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);

                if (withSVN)
                {
                    EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                    {
                        if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"材质实例{Name} {dir}使用版本控制添加失败!");
                        }
                    }, dir, $"AutoCommit 增加材质实例{Name}路径", true);
                }
            }
            if (absStrFile.Contains(":"))
                absStrFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(absStrFile, CSUtility.Support.IFileManager.Instance.Root);

            Ver++;

            CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Technique", "");
            Save(xmlHolder.RootNode, xmlHolder);

            CSUtility.Support.XmlHolder.SaveXML(absStrFile, xmlHolder, true);

            IsDirty = false;

            if (withSVN)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{absFileName}使用版本控制提交失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                        {
                            if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{absFileName}使用版本控制提交失败!");
                            }
                        }, absFileName, $"AutoCommit 修改材质实例{Name}");
                    }
                }, absFileName);

                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{absFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制提交失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                        {
                            if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"{absFileName + ResourcesBrowser.Program.SnapshotExt}使用版本控制提交失败!");
                            }
                        }, absFileName + ResourcesBrowser.Program.SnapshotExt, $"AutoCommit 修改材质实例{Name}缩略图");
                    }
                }, absFileName + ResourcesBrowser.Program.SnapshotExt);
            }
        }

        public void CopyFrom(MaterialTechniqueInfo info, bool withHostId = true)
        {
            //Name = info.Name;
            AlphaRef = info.AlphaRef;
            CullMode = info.CullMode;
            ZDisable = info.ZDisable;
            ZWriteDisable = info.ZWriteDisable;
            WireFrame = info.WireFrame;
            UseSystemMaterialFilterType = info.UseSystemMaterialFilterType;
            AlphaType = info.AlphaType;
            BlendOp = info.BlendOp;
            BlendSrc = info.BlendSrc;
            BlendDest = info.BlendDest;
            BillboardType = info.BillboardType;
            ZCmpFunc = info.ZCmpFunc;
            if(withHostId)
                mHostMaterialId = info.HostMaterialId;

            AcceptDecal = info.AcceptDecal;
            AcceptLight = info.AcceptLight;
            StencilEnable = info.StencilEnable;
            StencilCmp = info.StencilCmp;
            StencilRef = info.StencilRef;
            StencilMask = info.StencilMask;
            StencilWriteMask = info.StencilWriteMask;
            StencilFailOp = info.StencilFailOp;
            StencilZFailOp = info.StencilZFailOp;
            StencilPassOp = info.StencilPassOp;

            DepthBias = info.DepthBias;
            SlopeScaleDepthBias = info.SlopeScaleDepthBias;

            foreach(var varInfo in ShaderVarInfos)
                varInfo.OnDirtyChanged = null;
            ShaderVarInfos.Clear();
            foreach (var varInfo in info.ShaderVarInfos)
            {
                CCore.Material.MaterialShaderVarInfo vInfo = new CCore.Material.MaterialShaderVarInfo();
                vInfo.Copy(varInfo);
                ShaderVarInfos.Add(vInfo);
            }
        }
    }
}
