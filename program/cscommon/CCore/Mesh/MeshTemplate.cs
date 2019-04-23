using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Mesh
{
    /// <summary>
    /// mesh模板类
    /// </summary>
    [System.ComponentModel.TypeConverter("System.ComponentModel.ExpandableObjectConverter")]
    public class MeshTemplate : CSUtility.Support.XndSaveLoadProxy, INotifyPropertyChanged, CSUtility.Helper.IVersionInterface
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            if (_OnPropertyChanged != null)
                _OnPropertyChanged(propertyName);
        }
        #endregion
        /// <summary>
        /// 声明编辑器更改属性时调用的委托事件
        /// </summary>
        /// <param name="propertyName">改变的属性的名称</param>
        public delegate void Delegate_Editor_OnPropertyChanged(string propertyName);
        /// <summary>
        /// 定义编辑器更改属性时调用的委托事件
        /// </summary>
        public event Delegate_Editor_OnPropertyChanged _OnPropertyChanged;
        /// <summary>
        /// 声明编辑器中置脏时调用的委托事件
        /// </summary>
        /// <param name="mt">mesh模板对象</param>
        public delegate void Delegate_Editor_OnDirtyChanged(MeshTemplate mt);
        /// <summary>
        /// 定义编辑器中置脏时调用的委托事件
        /// </summary>
        public event Delegate_Editor_OnDirtyChanged Editor_OnPropertyChanged;

        private bool m_bIsDirty = false;
        /// <summary>
        /// 是否置脏
        /// </summary>
        [Browsable(false)]
        public bool IsDirty
        {
            get { return m_bIsDirty; }
            set 
            {
                m_bIsDirty = value;
                OnPropertyChanged("IsDirty");

                if(Editor_OnPropertyChanged != null)
                    Editor_OnPropertyChanged(this);
            }
        }
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns>返回当前的版本</returns>
        public UInt32 GetVersion()
        {
            return mVer;
        }

        UInt32 mVer = 0;
        /// <summary>
        /// 版本号
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Ver")]
        [CSUtility.Support.AutoSaveLoad]
        [ReadOnly(true)]
        [Category("基本")]
        [DisplayName("版本")]
        public UInt32 Ver
        {
            get { return mVer; }
            set
            {
                mVer = value;
                OnPropertyChanged("Ver");
            }
        }

        Guid mMeshID;
        /// <summary>
        /// mesh的ID
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MeshID")]
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本")]
        [ReadOnly(true)]
        [DisplayName("模型ID")]
        public Guid MeshID
        {// 模型ID
            get { return mMeshID; }
            set { mMeshID = value; }
        }

        //string mPath = "";
        //[CSUtility.Support.DataValueAttribute("Path")]
        //public string Path
        //{
        //    get { return mPath; }
        //    set 
        //    {
        //        var tempValue = mPath;
        //        mPath = value;
        //        OnPropertyChanged("Path");

        //        IsDirty = (tempValue != mPath);
        //    }
        //}

        string mNickName = "";
        /// <summary>
        /// 模板的名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("NickName")]
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本")]
        [DisplayName("名称")]
        public string NickName
        {
            get { return mNickName; }
            set
            {
                var tempValue = mNickName;
                mNickName = value;
                OnPropertyChanged("NickName");

                IsDirty = (tempValue != mNickName);
            }
        }

        bool m_bNeedCalcFullSkeleton = true;
        /// <summary>
        /// 是否合并子骨架
        /// </summary>
        [CSUtility.Support.DataValueAttribute("NeedCalcFullSkeleton")]
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本")]
        [DisplayName("合并子骨架")]
        [Description("根据模型部件的子骨架信息将多个子骨架合并成一个完整的骨架")]
        [Browsable(false)]
        public bool NeedCalcFullSkeleton
        {
            get { return m_bNeedCalcFullSkeleton; }
            set 
            {
                var tempValue = m_bNeedCalcFullSkeleton;
                m_bNeedCalcFullSkeleton = value;

                OnPropertyChanged("NeedCalcFullSkeleton");

                IsDirty = (tempValue != m_bNeedCalcFullSkeleton);
            }
        }

        List<MeshInitPart> mMeshInitList = new List<MeshInitPart>();
        /// <summary>
        /// 模型部件列表
        /// </summary>
        [Category("模型设置")]
        [DisplayName("模型部件")]
        [CSUtility.Support.DataValueAttribute("MeshInitList")]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshInit")]
        public List<MeshInitPart> MeshInitList
        {
            get { return mMeshInitList; }
            set
            {
                var tempValue = mMeshInitList;
                mMeshInitList = value;

                IsDirty = (tempValue != mMeshInitList);
                if (IsDirty)
                {
                    //MidLayer.IMeshInit mshInit = new MidLayer.IMeshInit();
                    //for (int i = 0; i < mMeshInitList.Count; ++i)
                    //{
                    //    MidLayer.IMeshInitPart mshInitPart = mMeshInitList[i];
                    //    mshInit.MeshInitParts.Add(mshInitPart);
                    //}
                    //var mesh = new MidLayer.IMesh();
                    //mesh.Initialize(mshInit);
                }

                OnPropertyChanged("MeshInitList");
            }
        }


        string m_ActionName;
        /// <summary>
        /// 动作名称
        /// </summary>
        [Category("动画预览")]
        [DisplayName("动作名称")]
        [CSUtility.Support.DataValueAttribute("ActionName")]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("ActionSet")]
        public string ActionName
        {
            get { return m_ActionName; }
            set
            {
                var tempValue = m_ActionName;
                m_ActionName = value;

                OnPropertyChanged("ActionName");

                IsDirty = (tempValue != m_ActionName);
            }
        }

        double mPlayRate = 1;
        /// <summary>
        /// 动画的播放速度
        /// </summary>
        [Category("动画预览")]
        [DisplayName("播放速率")]
        [CSUtility.Support.DataValueAttribute("PlayRate")]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_ValueWithRange(0, 5)]
        public double PlayRate
        {
            get { return mPlayRate; }
            set 
            {
                mPlayRate = value;

                OnPropertyChanged("PlayRate");
                IsDirty = true;
            }
        }

        //bool mPause = false;
        //[Category("动画预览")]
        //[CSUtility.Support.DataValueAttribute("Pause")]
        //public bool Pause
        //{
        //    get { return mPause; }
        //    set
        //    {
        //        mPause = value;
        //        OnPropertyChanged("Pause");
        //        IsDirty = true;
        //    }
        //}

        double mActionPercent = 0;
        /// <summary>
        /// 动画的播放位置
        /// </summary>
        [Category("动画预览")]
        [DisplayName("播放位置")]
        [CSUtility.Support.DataValueAttribute("ActionPercent")]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public double ActionPercent
        {
            get { return mActionPercent; }
            set
            {
                mActionPercent = value;

                OnPropertyChanged("ActionPercent");
                IsDirty = true;
            }
        }

        //bool mLoop = true;
        //[Category("动画预览")]
        //[CSUtility.Support.DataValueAttribute("Loop")]
        //public bool Loop
        //{
        //    get { return mLoop; }
        //    set
        //    {
        //        mLoop = value;
        //        OnPropertyChanged("Loop");
        //        IsDirty = true;
        //    }
        //}


        /*
        组件化扩展
        double mMaskColorOpacity = 0.0f;
        [Category("扩展")]
        [CSUtility.Support.DataValueAttribute("MaskColorOpacity")]
        [CSUtility.Editor.UIEditor_ValueWithRange(-1.0,1.0)]
        [Description("材质中MaskColor参数透明度设置(材质参数名称为MaskColorOpacity)")]
        public double MaskColorOpacity
        {
            get { return mMaskColorOpacity; }
            set
            {
                mMaskColorOpacity = value;
				IsDirty = true;
                OnPropertyChanged("MaskColorOpacity");                
            }
        }

		CSUtility.Support.Color mMaskColor = CSUtility.Support.Color.FromArgb(255,255,255,255);
        [CSUtility.Support.DataValueAttribute("MaskColor")]
        [CSUtility.Editor.Editor_ColorPicker]
        public CSUtility.Support.Color MaskColor
        {
            get { return mMaskColor; }
            set
            {
                mMaskColor = value;
                IsDirty = true;
				OnPropertyChanged("MaskColor");
            }
        }*/

        float mScale = 1.0f;
        /// <summary>
        /// 缩放值
        /// </summary>
        [Category("基本")]
        [CSUtility.Support.DataValueAttribute("Scale")]
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("缩放")]
        public float Scale
        {
            get { return mScale; }
            set
            {
                mScale = value;
				OnPropertyChanged("Scale");
                IsDirty = true;
            }
        }

        string mLayerName = "Other";
        /// <summary>
        /// 层名称
        /// </summary>
        [Category("基本")]
        [CSUtility.Support.DataValueAttribute("LayerName")]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_ActorLayerSetter]
        [DisplayName("层名称")]
        [Description("用来设置使用此模型的场景对象在场景中所在的默认层，方便场景对象管理")]
        public string LayerName
        {
            get { return mLayerName; }
            set
            {
                mLayerName = value;
                IsDirty = true;
				OnPropertyChanged("LayerName");
            }
        }

        CCore.RLayer mLayer = CCore.RLayer.RL_DSBase;
        /// <summary>
        /// 渲染层
        /// </summary>
        [Category("基本")]
        [CSUtility.Support.DataValueAttribute("Layer")]
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("渲染层")]
        public CCore.RLayer Layer
        {
            get { return mLayer; }
            set
            {
                mLayer = value;
                IsDirty = true;
                OnPropertyChanged("Layer");
            }
        }
        
        CSUtility.Support.ConcurentObjManager<Guid, CCore.Socket.ISocketComponentInfo> mSocketComponentInfoList = new CSUtility.Support.ConcurentObjManager<Guid, CCore.Socket.ISocketComponentInfo>();
        /// <summary>
        /// 挂接成员列表
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DataValueAttribute("SocketComponentInfoList")]
        [CSUtility.Support.AutoSaveLoad]
        public CSUtility.Support.ConcurentObjManager<Guid, CCore.Socket.ISocketComponentInfo> SocketComponentInfoList
        {
            get { return mSocketComponentInfoList; }
            set
            {
                mSocketComponentInfoList = value;
                OnPropertyChanged("SocketComponentInfoList");
            }
        }

        bool mCastShadow = true;
        /// <summary>
        /// 是否产生阴影
        /// </summary>
        [Category("基本")]
        [CSUtility.Support.DataValueAttribute("CastShadow")]
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("产生阴影")]
        public bool CastShadow
        {
            get { return mCastShadow; }
            set
            {
                mCastShadow = value;
                OnPropertyChanged("CastShadow");
            }
        }

        //bool mEnableTrail = false;
        //[CSUtility.Support.DataValueAttribute("EnableTrail")]
        //public bool EnableTrail
        //{
        //    get { return mEnableTrail; }
        //    set
        //    {
        //        mEnableTrail = value;
        //        OnPropertyChanged("EnableTrail");
        //    }
        //}
        bool mTickAllTime = false;
        /// <summary>
        /// 是否总是更新
        /// </summary>
        [CSUtility.Support.DataValueAttribute("TickAllTime")]
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本")]
        [DisplayName("总是更新")]
        [Description("为保证效率，模型默认会在超出一定范围内停止更新(如动画播放等), 勾上此选项会保证模型一直进行更新")]
        public bool TickAllTime
        {
            get { return mTickAllTime; }
            set
            {
                mTickAllTime = value;
                OnPropertyChanged("TickAllTime");
            }
        }

        MeshFadeType mMeshFadeType = MeshFadeType.None;
        /// <summary>
        /// 淡入淡出类型
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MeshFadeType")]
        [CSUtility.Support.AutoSaveLoad]
        [Category("淡入淡出")]
        [DisplayName("淡入淡出类型")]
        public MeshFadeType MeshFadeType
        {
            get { return mMeshFadeType; }
            set { mMeshFadeType = value; }
        }
        private UInt32 mFadeInTime = 500;
        /// <summary>
        /// 淡入时间
        /// </summary>
        [CSUtility.Support.DataValueAttribute("FadeInTime")]
        [CSUtility.Support.AutoSaveLoad]
        [Category("淡入淡出")]
        [DisplayName("淡入时间")]
        public UInt32 FadeInTime
        {
            get { return mFadeInTime; }
            set { mFadeInTime = value; }
        }
        private UInt32 mFadeOutTime = 500;
        /// <summary>
        /// 淡出时间
        /// </summary>
        [CSUtility.Support.DataValueAttribute("FadeOutTime")]
        [CSUtility.Support.AutoSaveLoad]
        [Category("淡入淡出")]
        [DisplayName("淡出时间")]
        public UInt32 FadeOutTime
        {
            get { return mFadeOutTime; }
            set { mFadeOutTime = value; }
        }

        bool mIgnoreMouseLineCheckInGame = true;
        /// <summary>
        /// 是否忽略游戏中点击
        /// </summary>
        [Category("基本")]
        [DisplayName("忽略游戏中点击")]
        [Description("勾选后使用此模型的对象能够在游戏中点透")]
        [CSUtility.Support.DataValueAttribute("IgnoreMouseLineCheckInGame")]
        [CSUtility.Support.AutoSaveLoad]
        public bool IgnoreMouseLineCheckInGame
        {
            get { return mIgnoreMouseLineCheckInGame; }
            set
            {
                mIgnoreMouseLineCheckInGame = value;

                IsDirty = true;

                OnPropertyChanged("IgnoreMouseLineCheckInGame");
            }
        }
        /// <summary>
        /// mesh模板的构造函数
        /// </summary>
        public MeshTemplate()
        {
            mMeshID = Guid.NewGuid();
            NickName = "NewMesh";
            ActionName = "";
            IsDirty = false;
        }

        /// <summary>
        /// 克隆mesh模板
        /// </summary>
        /// <returns>返回克隆的mesh模板</returns>
        public MeshTemplate Clone()
        {
            MeshTemplate newMT = new MeshTemplate();
            newMT.ActionName = String.Copy(ActionName);
            newMT.ActionPercent = ActionPercent;
            //newMT.Loop = Loop;
            newMT.MeshID = MeshID;
            //newMT.MeshInitList = MeshInitList;
            foreach (var mInit in MeshInitList)
            {
                var newPart = new MeshInitPart();
                newPart.MeshName = String.Copy(mInit.MeshName);
                //foreach(var mtl in mInit.Materials)
                //{
                //    newPart.Materials.Add(String.Copy(mtl));
                //}
                foreach(var tech in mInit.Techs)
                {
                    newPart.Techs.Add(tech);
                }
                newMT.MeshInitList.Add(newPart);
            }
            newMT.NeedCalcFullSkeleton = NeedCalcFullSkeleton;
            newMT.NickName = NickName;
            //newMT.Path = Path;
            //newMT.Pause = Pause;
            newMT.PlayRate = PlayRate;
            //newMT.MaskColorOpacity = MaskColorOpacity;
            //newMT.MaskColor = CSUtility.Support.Color.FromArgb(MaskColor.A, MaskColor.R, MaskColor.G, MaskColor.B);
            newMT.Scale = Scale;
            newMT.LayerName = String.Copy(LayerName);
            newMT.MeshFadeType = MeshFadeType;
            newMT.FadeInTime = FadeInTime;
            newMT.FadeOutTime = FadeOutTime;
            SocketComponentInfoList.For_Each((Guid id, CCore.Socket.ISocketComponentInfo socketInfo, object arg) =>
            {
                var copyedInfo = System.Activator.CreateInstance(socketInfo.GetType()) as CCore.Socket.ISocketComponentInfo;
                copyedInfo.CopyComponentInfoFrom(socketInfo);
                newMT.SocketComponentInfoList[copyedInfo.SocketComponentInfoId] = copyedInfo;

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null); return newMT;
        }
        /// <summary>
        /// 复制mesh模板
        /// </summary>
        /// <param name="srcMt">复制的mesh模板对象</param>
        public void CopyFrom(MeshTemplate srcMt)
        {
            ActionName = String.Copy(srcMt.ActionName);
            ActionPercent = srcMt.ActionPercent;
            MeshInitList.Clear();
            foreach (var mInit in srcMt.MeshInitList)
            {
                var newPart = new MeshInitPart();
                newPart.MeshName = String.Copy(mInit.MeshName);
                foreach (var tech in mInit.Techs)
                {
                    newPart.Techs.Add(tech);
                }
                MeshInitList.Add(newPart);
            }
            NeedCalcFullSkeleton = srcMt.NeedCalcFullSkeleton;
            NickName = srcMt.NickName + "_Copy";
            //Path = srcMt.Path;
            PlayRate = srcMt.PlayRate;
            //MaskColorOpacity = srcMt.MaskColorOpacity;
            //MaskColor = CSUtility.Support.Color.FromArgb(srcMt.MaskColor.A, srcMt.MaskColor.R, srcMt.MaskColor.G, srcMt.MaskColor.B);
            Scale = srcMt.Scale;
            MeshFadeType = srcMt.MeshFadeType;
            FadeInTime = srcMt.FadeInTime;
            FadeOutTime = srcMt.FadeOutTime;
            LayerName = String.Copy(srcMt.LayerName);
            SocketComponentInfoList.Clear();
            srcMt.SocketComponentInfoList.For_Each((Guid id, CCore.Socket.ISocketComponentInfo socketInfo, object arg) =>
            {
                var copyedInfo = System.Activator.CreateInstance(socketInfo.GetType()) as CCore.Socket.ISocketComponentInfo;
                copyedInfo.CopyComponentInfoFrom(socketInfo);
                SocketComponentInfoList[copyedInfo.SocketComponentInfoId] = copyedInfo;

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }
    }
    /// <summary>
    /// mesh模板信息类
    /// </summary>
    public class MeshTemplateMgr
    {
        static MeshTemplateMgr smInstance = new MeshTemplateMgr();
        CSUtility.Support.ConcurentObjManager<Guid, string> mMeshFileDic = new CSUtility.Support.ConcurentObjManager<Guid, string>();        
        CSUtility.Support.ConcurentObjManager<Guid, MeshTemplate> mMeshs = new CSUtility.Support.ConcurentObjManager<Guid, MeshTemplate>();
        /// <summary>
        /// 设置为单例对象
        /// </summary>
        public static MeshTemplateMgr Instance
        {
            get { return smInstance; }
        }
        /// <summary>
        /// mesh模板列表
        /// </summary>
        public CSUtility.Support.ConcurentObjManager<Guid, MeshTemplate> Meshs
        {
            get { return mMeshs; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        private MeshTemplateMgr()
        {
            LoadFileDictionary();
        }               

        //返回相对路径
        /// <summary>
        /// 根据ID获取mesh模板文件
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <returns>返回mesh模板文件的相对路径</returns>
        public string GetMeshTemplateFile(Guid id)
        {            
            if (mMeshFileDic.ContainsKey(id))
            {
                var file = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mMeshFileDic[id]);
                if (!System.IO.File.Exists(file))
                {
                    RemoveMeshFile(id);
                }
                else
                {
                    return mMeshFileDic[id];
                }
            }
            return GetFileName(id);            
        }
        /// <summary>
        /// 根据ID获取文件名称
        /// </summary>
        /// <param name="id">mesh模板ID</param>
        /// <returns>返回文件名称</returns>
        string GetFileName(Guid id)
        {
            var files = System.IO.Directory.GetFiles(mResDir, id.ToString() + CSUtility.Support.IFileConfig.MeshTemplateExtension, System.IO.SearchOption.AllDirectories);
            if (files.Length == 0)
                return "";

            var name = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(files[0]);
            SetMeshFile(id, name);
            return name;
        }

        string mResDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;
        /// <summary>
        /// 查找mesh模板
        /// </summary>
        /// <param name="id">mesh模板的ID</param>
        /// <param name="folder">查找的文件路径</param>
        /// <param name="bFindInSubFolder">在文件夹中查找</param>
        /// <returns>返回查找到的mesh模板</returns>
        public MeshTemplate FindMeshTemplate(Guid id, string folder, bool bFindInSubFolder = true)
        {
            if (id == Guid.Empty)
                return null;

            if (mMeshFileDic.ContainsKey(id))
            {
                var file = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mMeshFileDic[id]);
                if (!System.IO.File.Exists(file))
                {
                    RemoveMeshFile(id);
                }
                else
                {
                    return LoadMeshTemplate(mMeshFileDic[id]);
                }                
            }

            if (!System.IO.Directory.Exists(folder))
                return null;

            string[] files;
            if (bFindInSubFolder)
            {
                files = System.IO.Directory.GetFiles(folder, id.ToString() + CSUtility.Support.IFileConfig.MeshTemplateExtension, System.IO.SearchOption.AllDirectories);
            }
            else
            {
                files = System.IO.Directory.GetFiles(folder, id.ToString() + CSUtility.Support.IFileConfig.MeshTemplateExtension, System.IO.SearchOption.TopDirectoryOnly);
            }

            if (files.Length == 0)
                return null;

            return LoadMeshTemplate(files[0]);            
        }
        /// <summary>
        /// 通过meshID查找mesh模板
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <returns>返回找到的mesh模板</returns>
        public MeshTemplate FindMeshTemplate(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            if (mMeshFileDic.ContainsKey(id))
            {
                var file = CSUtility.Support.IFileManager.Instance._GetAbsPathFromRelativePath(mMeshFileDic[id]);
                if (!System.IO.File.Exists(file))
                {
                    RemoveMeshFile(id);
                }
                else
                {
                    return LoadMeshTemplate(mMeshFileDic[id]);
                }
            }

            var files = System.IO.Directory.GetFiles(mResDir, id.ToString() + CSUtility.Support.IFileConfig.MeshTemplateExtension, System.IO.SearchOption.AllDirectories);
            if (files.Length == 0)
                return null;
            return LoadMeshTemplate(files[0]);
        }
        /// <summary>
        /// 通过文件名称加载mesh模板
        /// </summary>
        /// <param name="fileName">要加载的文件名称</param>
        /// <returns>返回加载的mesh模板</returns>
        public MeshTemplate LoadMeshTemplate(string fileName)
        {
            var id = CSUtility.Program.GetIdFromFile(fileName);

            MeshTemplate data = mMeshs.FindObj(id);
            if (data != null)
            {                
                return data;
            }                

            fileName = fileName.Replace("\\", "/");
            fileName = fileName.Replace(CSUtility.Support.IFileManager.Instance.Root.Replace("\\", "/"), "");

            data = new MeshTemplate();
            if (CSUtility.Support.IConfigurator.FillProperty(data, fileName) == false)
            {
                var xndNode = LoadXndFile(fileName);
                if (xndNode == null)
                    return null;

                FillReadXnd(data, xndNode);
            }

            mMeshs.Add(id, data);
            SetMeshFile(id, fileName);
            data.IsDirty = false;
            return data;
        }
        /// <summary>
        /// 保存mesh模板
        /// </summary>
        /// <param name="id">mesh模板ID</param>
        /// <param name="notUpdateVer">是否进行版本更新，缺省为false</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        void FillReadXnd(MeshTemplate data, CSUtility.Support.XndNode xndNode)
        {
            foreach (var att in xndNode.GetAttribs())
            {
                att.BeginRead();
                data.Read(att);
                att.EndRead();
            }

            foreach (var node in xndNode.GetNodes())
            {
                FillReadXnd(data, node);
            }
        }

        CSUtility.Support.XndNode LoadXndFile(string file)
        {
            if (!System.IO.Path.IsPathRooted(file))
                file = CSUtility.Support.IFileManager.Instance.Root + file;

            var xndHolder = CSUtility.Support.XndHolder.LoadXND(file);
            if (xndHolder == null)
                return null;
            if (xndHolder.Node == null)
                return null;
            return xndHolder.Node;
        }

        bool FillWriteXnd(MeshTemplate data, CSUtility.Support.XndNode xndNode)
        {
            if (data == null)
                return false;
            var type = data.GetType();
            foreach (System.Reflection.PropertyInfo prop in type.GetProperties())
            {
                if (prop.CanRead == false)
                    continue;
                var attrs = prop.GetCustomAttributes(typeof(CSUtility.Support.DataValueAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;
                CSUtility.Support.DataValueAttribute dv = (CSUtility.Support.DataValueAttribute)attrs[0];
                if (dv == null)
                    continue;
                var node = xndNode.AddNode(dv.Name, 0, 0);
                var att = node.AddAttrib("Data");
                att.BeginWrite();
                data.Write(att);
                att.EndWrite();
            }
            return true;
            //    foreach (var att in xndNode.GetAttribs())
            //{
            //    if (data.Write(att) == false)
            //        return false;
            //}

            //foreach (var node in xndNode.GetNodes())
            //{
            //    if (FillWriteXnd(data, node) == false)
            //        return false;
            //}

            //return true;
        }

        

        public bool SaveMeshTemplate(Guid id, bool notUpdateVer = false, bool isSaveXnd = false)
        {
            var mt = FindMeshTemplate(id);
            if (mt == null)
                return false;

            var fileName = mMeshFileDic.FindObj(id);
            if (string.IsNullOrEmpty(fileName))
                return false;

            if (!notUpdateVer)
                mt.Ver++;

            if (!isSaveXnd)
            {
                CSUtility.Support.IConfigurator.SaveProperty(mt, "MeshTemplate", fileName);
            }
            else
            {
                var xndHolder = CSUtility.Support.XndHolder.NewXNDHolder();
                var att = xndHolder.Node.AddAttrib("Ver");
                att.BeginWrite();
                Int32 hashCode = (Int32)CSUtility.Program.GetNewClassHash(mt.GetType(), typeof(CSUtility.Support.AutoSaveLoadAttribute));
                att.Write(hashCode);
                att.Write(0);
                att.EndWrite();

                FillWriteXnd(mt, xndHolder.Node);
                CSUtility.Support.XndHolder.SaveXND(fileName, xndHolder);
            }
            mt.IsDirty = false;

            return true;
        }
        /// <summary>
        /// 保存mesh模板
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <param name="id">mesh模板ID</param>
        /// <param name="notUpdateVer">是否不进行版本更新，缺省为false</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool SaveMeshTemplate(string file, Guid id, bool notUpdateVer = false, bool isSaveXnd = false)
        {
            var mt = FindMeshTemplate(id);
            if (mt == null)
            {
                mt = new MeshTemplate();
                mt.MeshID = id;                
                mMeshs.Add(id, mt);                
            }

            file = file.Replace("\\", "/");
            file = file.Replace(CSUtility.Support.IFileManager.Instance.Root.Replace("\\", "/"), "");

            if (!notUpdateVer)
                mt.Ver++;
            if (!isSaveXnd)
            {
                CSUtility.Support.IConfigurator.SaveProperty(mt, "MeshTemplate", file);
            }
            else
            {
                var xndHolder = CSUtility.Support.XndHolder.NewXNDHolder();
                var att = xndHolder.Node.AddAttrib("Ver");
                att.BeginWrite();
                Int32 hashCode = (Int32)CSUtility.Program.GetNewClassHash(mt.GetType(), typeof(CSUtility.Support.AutoSaveLoadAttribute));
                att.Write(hashCode);
                att.Write(0);
                att.EndWrite();

                FillWriteXnd(mt, xndHolder.Node);
                CSUtility.Support.XndHolder.SaveXND(file, xndHolder);
            }

            SetMeshFile(id, file);
            mt.IsDirty = false;

            return true;
        }
        /// <summary>
        /// 加载所有的mesh模板
        /// </summary>
        public void LoadAllMeshTemplate()
        {
            var files = System.IO.Directory.GetFiles(mResDir, CSUtility.Support.IFileConfig.MeshTemplateExtension,System.IO.SearchOption.AllDirectories);
            foreach (var file in files)
            {                
                LoadMeshTemplate(file);
            }            
        }
       
        string mFileName = CSUtility.Support.IFileConfig.DefaultResourceDirectory + "/" + "MeshFileDic";
        /// <summary>
        /// 加载文件
        /// </summary>
        public void LoadFileDictionary()
        {                        
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(mFileName);
            if (xmlHolder == null)
                return;

            var nodes = xmlHolder.RootNode.FindNodes("FileNode");
            for (int i = 0; i < nodes.Count; i++)
            {                
                Guid key = Guid.Empty;
                string file = "";

                var att = nodes[i].FindAttrib("key");
                if (att != null)
                    key = CSUtility.Support.IHelper.GuidParse(att.Value);

                att = nodes[i].FindAttrib("File");
                if (att != null)
                    file = att.Value;
                
                mMeshFileDic[key] = file;                                      
            }
        }
        /// <summary>
        /// 重新加载文件
        /// </summary>
        public void ReLoadFileDictionary()
        {
            mMeshFileDic.Clear();
            var files = System.IO.Directory.GetFiles(mResDir, "*" + CSUtility.Support.IFileConfig.MeshTemplateExtension, System.IO.SearchOption.AllDirectories);
            foreach(var file in files)
            {
                var id = CSUtility.Program.GetIdFromFile(file);
                var name = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
                mMeshFileDic[id] = name;
            }

            SaveFileDictionary();
        }
        /// <summary>
        /// 清除文件
        /// </summary>
        public void ClearFileDictionary()
        {
            mMeshFileDic.Clear();            
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        public void SaveFileDictionary()
        {            
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FileDic", "");

            mMeshFileDic.For_Each((Guid key, string value, object argObj) =>
            {
                var node = xmlHolder.RootNode.AddNode("FileNode", "", xmlHolder);
                node.AddAttrib("key", key.ToString());
                node.AddAttrib("File", value);                

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);


            CSUtility.Support.XmlHolder.SaveXML(mFileName, xmlHolder, true);
        }
        /// <summary>
        /// 删除相应ID的mesh文件
        /// </summary>
        /// <param name="id">mesh的ID</param>
        public void RemoveMeshFile(Guid id)
        {
            mMeshs.Remove(id);
            mMeshFileDic.Remove(id);
            SaveFileDictionary();
        }
        /// <summary>
        /// 设置mesh文件
        /// </summary>
        /// <param name="id">mesh的ID</param>
        /// <param name="file">文件路径</param>
        public void SetMeshFile(Guid id, string file)
        {
            if (System.IO.Path.IsPathRooted(file))
                file = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
            string oldFile;
            if(mMeshFileDic.TryGetValue(id, out oldFile))
            {
                if (file.Equals(oldFile))
                    return;
            }

            mMeshFileDic[id] = file;
            SaveFileDictionary();
        }
    }
}
