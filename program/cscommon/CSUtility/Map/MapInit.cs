using System;
using System.ComponentModel;

namespace CSUtility.Map
{
    public class WorldInit
    {

        public static string ClientSceneGraphFileName = "Scene/SceneGraph.dat";
        public static string ServerSceneGraphFileName = "ServerScene/SceneGraph.dat";
        public static string ServerAltitudeFileName = "ServerHeightMap/SHeightMap.dat";

        bool mIsDirty = false;
        [Browsable(false)]
        public bool IsDirty
        {
            get { return mIsDirty; }
            set { mIsDirty = value; }
        }

        Guid mWorldId = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("WorldId")]
        [Category("基础属性")]
        [System.ComponentModel.DisplayName("地图id")]
        [ReadOnly(true)]
        public Guid WorldId
        {
            get { return mWorldId; }
            set
            {
                mWorldId = value;
                IsDirty = true;
            }
        }

        string mWorldName = "未命名";
        [CSUtility.Support.DataValueAttribute("WorldName")]
        [Category("基础属性")]
        [System.ComponentModel.DisplayName("地图名称")]
        public string WorldName
        {
            get { return mWorldName; }
            set
            {
                mWorldName = value;
                IsDirty = true;
            }
        }

        //////float mSceneMeterX = 256;
        //////[CSUtility.Support.DataValueAttribute("SceneMeterX")]
        //////[System.ComponentModel.Browsable(false)]
        //////public float SceneMeterX
        //////{
        //////    get { return mSceneMeterX; }
        //////    set
        //////    {
        //////        mSceneMeterX = value;
        //////        IsDirty = true;
        //////    }
        //////}

        //////float mSceneMeterY = 256;
        //////[CSUtility.Support.DataValueAttribute("SceneMeterY")]
        //////[System.ComponentModel.Browsable(false)]
        //////public float SceneMeterY
        //////{
        //////    get { return mSceneMeterY; }
        //////    set
        //////    {
        //////        mSceneMeterY = value;
        //////        IsDirty = true;
        //////    }
        //////}

        //////float mSceneMeterZ = 256;
        //////[CSUtility.Support.DataValueAttribute("SceneMeterZ")]
        //////[System.ComponentModel.Browsable(false)]
        //////public float SceneMeterZ
        //////{
        //////    get { return mSceneMeterZ; }
        //////    set
        //////    {
        //////        mSceneMeterZ = value;
        //////        IsDirty = true;
        //////    }
        //////}

        #region 回调
        [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Map")]
        public delegate bool FOnEnterMap(AISystem.IStateHost host);

        CSUtility.Helper.EventCallBack mOnEnterMapCB;
        [System.ComponentModel.Browsable(false)]
        public CSUtility.Helper.EventCallBack OnEnterMapCB
        {
            get { return mOnEnterMapCB; }
        }

        Guid mOnEnterMap = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnEnterMap")]
        [System.ComponentModel.Category("脚本")]
        [System.ComponentModel.DisplayName("进入地图")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet",new object[] { typeof(FOnEnterMap) })]
        public Guid OnEnterMap
        {
            get { return mOnEnterMap; }
            set
            {
                mOnEnterMap = value;
                mOnEnterMapCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnEnterMap), value);
                IsDirty = true;
            }
        }

        [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Map")]
        public delegate bool FOnLeaveMap(AISystem.IStateHost host);

        CSUtility.Helper.EventCallBack mOnLeaveMapCB;
        [System.ComponentModel.Browsable(false)]
        public CSUtility.Helper.EventCallBack OnLeaveMapCB
        {
            get { return mOnLeaveMapCB; }
        }

        Guid mOnLeaveMap = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnLeaveMap")]
        [System.ComponentModel.Category("脚本")]
        [System.ComponentModel.DisplayName("离开地图")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet",new object[] { typeof(FOnLeaveMap) } )]
        public Guid OnLeaveMap
        {
            get { return mOnLeaveMap; }
            set
            {
                mOnLeaveMap = value;
                mOnLeaveMapCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnLeaveMap), value);
                IsDirty = true;
            }
        }

        [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Map")]
        public delegate bool FOnInitMap();

        CSUtility.Helper.EventCallBack mOnInitMapCB;
        [System.ComponentModel.Browsable(false)]
        public CSUtility.Helper.EventCallBack OnInitMapCB
        {
            get { return mOnInitMapCB; }
        }

        Guid mOnInitMap = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnInitMap")]
        [System.ComponentModel.Category("脚本")]
        [System.ComponentModel.DisplayName("初始化地图")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet",new object[] { typeof(FOnInitMap) })]
        public Guid OnInitMap
        {
            get { return mOnInitMap; }
            set
            {
                mOnInitMap = value;
                mOnInitMapCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnInitMap), value);
                IsDirty = true;
            }
        }

        [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Map")]
        public delegate bool FOnTickMap(AISystem.IStateHost host);
        CSUtility.Helper.EventCallBack mOnTickMapCB;
        [System.ComponentModel.Browsable(false)]
        public CSUtility.Helper.EventCallBack OnTickMapCB
        {
            get { return mOnTickMapCB; }
        }
        Guid mOnTickMap = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnTickMap")]
        [System.ComponentModel.Category("脚本")]
        [System.ComponentModel.DisplayName("地图Tick")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet",new object[] { typeof(FOnTickMap) })]
        public Guid OnTickMap
        {
            get { return mOnTickMap; }
            set
            {
                mOnTickMap = value;
                mOnTickMapCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnTickMap), value);
                IsDirty = true;
            }
        }

        [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Map")]
        public delegate bool FOnRoleActorDeath(Component.ActorBase actor);
        CSUtility.Helper.EventCallBack mOnRoleActorDeathCB;
        [System.ComponentModel.Browsable(false)]
        public CSUtility.Helper.EventCallBack OnRoleActorDeathCB
        {
            get { return mOnRoleActorDeathCB; }
        }
        Guid mOnRoleActorDeath = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("OnRoleActorDeath")]
        [System.ComponentModel.Category("脚本")]
        [System.ComponentModel.DisplayName("地图里面RoleActor死亡")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet",new object[] { typeof(FOnRoleActorDeath) } )]
        public Guid OnRoleActorDeath
        {
            get { return mOnRoleActorDeath; }
            set
            {
                mOnRoleActorDeath = value;
                mOnRoleActorDeathCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnRoleActorDeath), value);
                IsDirty = true;
            }
        }
        #endregion

        /// <summary>
        /// 存储地图初始数据
        /// </summary>
        /// <param name="mapFolder">地图路径（绝对路径）</param>
        public void Save(string mapFolder)
        {
            if(CSUtility.Support.IConfigurator.SaveProperty(this, Program.GetTypeSaveString(this.GetType()), mapFolder + "/Config.map"))
                IsDirty = false;
        }
        /// <summary>
        /// 读取地图初始数据
        /// </summary>
        /// <param name="mapFolder">地图路径（绝对路径）</param>
        public void Load(string mapFolder)
        {
            if(CSUtility.Support.IConfigurator.FillProperty(this, mapFolder + "/Config.map"))
                IsDirty = false;
        }
    }
}
