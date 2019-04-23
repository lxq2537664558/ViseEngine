using System;
using System.ComponentModel;

namespace CCore.World
{
    /// <summary>
    /// 贴花Actor的类型枚举
    /// </summary>
    public enum DecalActorType
    {
        Box,
    }
    /// <summary>
    /// 贴花Actor的初始化类
    /// </summary>
    public class DecalActorInit : CCore.World.ActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DecalActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Decal;
            SceneFlag = CSUtility.Component.enActorSceneFlag.Static;
            mLayer = CCore.RLayer.RL_DSDecal;
        }
    }

    /// <summary>
    /// 贴花形Actor类
    /// </summary>
    public class DecalActor : CCore.World.Actor, INotifyPropertyChanged
    {
        float m_fEditorMeshSizeInScreen = 0.02f;
        /// <summary>
        /// 只读属性，贴花的可视化对象
        /// </summary>
        public CCore.Component.Decal Decal
        {
            get { return (CCore.Component.Decal)Visual; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DecalActor()
        {
            //mPlacement = new CSCommon.Component.IStandPlacement(this);
            AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient);
            m_fEditorMeshSizeInScreen = 0.02f;
            mPlacement = new CSUtility.Component.StandardPlacement(this);
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~DecalActor()
        {
            Cleanup();
        }
        /// <summary>
        /// 贴花初始化
        /// </summary>
        /// <param name="_init">用来初始化该对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            Cleanup();

            if (base.Initialize(_init) == false)
                return false;

            var actorInit = _init as DecalActorInit;

            Visual = new CCore.Component.Decal();

            return true;
        }

        CCore.Material.Material mMaterialObject;
        /// <summary>
        /// 只读属性，贴花的材质
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public CCore.Material.Material MaterialObject
        {
            get { return mMaterialObject; }
        }

        Guid mTechId = Guid.Empty;
        /// <summary>
        /// 材质的TechId
        /// </summary>
        [System.ComponentModel.Category("Decal属性")]
        [CSUtility.Support.DataValueAttribute("材质的TechId")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("TechniqueSet")]
        public Guid TechId
        {
            get { return mTechId; }
            set
            {
                var tempTechId = mTechId;
                mTechId = value;

                if (mMaterialObject != null)
                {
                    mMaterialObject.Cleanup();
                    //mMaterialObject.Dispose();
                }

                mMaterialObject = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(mTechId);
                if (mMaterialObject == null)
                    mMaterialObject = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(mTechId);

                Decal.SetMaterial(mMaterialObject);

                OnPropertyChanged("TechId");

            }
        }
        /// <summary>
        /// 保存场景数据
        /// </summary>
        /// <param name="attribute">XND文件节点</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public override bool SaveSceneData(CSUtility.Support.XndAttrib attribute)
        {
            base.SaveSceneData(attribute);

            attribute.Write(TechId.ToString());

            return true;
        }
        /// <summary>
        /// 从XND文件中加载场景数据
        /// </summary>
        /// <param name="attribute">XND数据节点</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
            base.LoadSceneData(attribute);

            string sTemp;
            attribute.Read(out sTemp);
            TechId = CSUtility.Support.IHelper.GuidTryParse(sTemp);

            return true;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapseMillisecondTime">每帧之间的间隔时间</param>
        public override void Tick(long elapseMillisecondTime)
        {
            base.Tick(elapseMillisecondTime);

            CalcEditorMeshSize(CCore.Client.MainWorldInstance.Camera);
            ShowEditorObj();
        }
        /// <summary>
        /// 计算mesh的大小
        /// </summary>
        /// <param name="eye">视野</param>
        public void CalcEditorMeshSize(CCore.Camera.CameraObject eye)
        {
            if (eye != null)
            {
                var decal = (CCore.Component.Decal)Visual;
                if (decal != null)
                    decal.SignMeshSize = eye.GetScreenSizeInWorld(mPlacement.GetLocation(), m_fEditorMeshSizeInScreen);
            }
        }
        /// <summary>
        /// 被选中
        /// </summary>
        public override void Editor_Selected()
        {
            base.Editor_Selected();

            var decal = (CCore.Component.Decal)Visual;
            if (decal != null)
                decal.ShowRangeMesh = true;
        }
        /// <summary>
        /// 没有被选中
        /// </summary>
        public override void Editor_UnSelected()
        {
            base.Editor_UnSelected();

            var decal = (CCore.Component.Decal)Visual;
            if (decal != null)
                decal.ShowRangeMesh = false;
        }
        /// <summary>
        /// 显示对象
        /// </summary>
        public void ShowEditorObj()
        {
            var decal = (CCore.Component.Decal)Visual;
            if (decal != null)
            {
                //if (CCore.Client.MainWorldInstance.IsActorGameTypeShow((UInt16)CSUtility.Component.EActorGameType.Decal))
                if(CCore.Program.IsActorTypeShow(this.World, CCore.Program.DecalAssistTypeName))
                {
                    decal.ShowSignMesh = true;
                }
                else
                {
                    decal.ShowSignMesh = false;
                    decal.ShowRangeMesh = false;
                }
            }
        }
    }
}
