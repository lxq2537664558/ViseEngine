using System;

namespace CCore.World
{
    /// <summary>
    /// 动态碰撞体类
    /// </summary>
    [CCore.EditorAssist.PlantAbleAttribute("对象.动态碰撞体", "", "")]
    public class DynamicBlockActor : Actor, EditorAssist.IPlantAbleObject
    {
        #region EditAssist
        /// <summary>
        /// 获取种植的对象
        /// </summary>
        /// <param name="world">种植的世界</param>
        /// <returns>返回种植的对象</returns>
        public CCore.World.Actor GetPlantActor(CCore.World.World world)
        {
            var mesh = new CCore.Mesh.Mesh();
            var mshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = CSUtility.Support.IFileConfig.DynamicBlockMeshTemplate,
            };
            mshInit.CanHitProxy = true;
            mesh.Initialize(mshInit, null);

            var actInit = new CSUtility.Map.DynamicBlock.DynamicBlockActorInit();
            actInit.DynamicBlockData.Id = Guid.NewGuid();
            var actor = new CCore.World.DynamicBlockActor(actInit.DynamicBlockData.Id);
            actor.Initialize(actInit);
            actor.Visual = mesh;
            actor.ActorName = "动态碰撞体" + Program.GetActorIndex();
            mesh.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(actor.Id));
            return actor;
        }
        /// <summary>
        /// 获取拖动显示的图标
        /// </summary>
        /// <param name="world">种植对象的世界</param>
        /// <returns>返回拖动显示的Actor图标</returns>
        public CCore.World.Actor GetPreviewActor(CCore.World.World world)
        {
            if (!CCore.Program.IsActorTypeShow(world, CCore.Program.DynamicBlockTypeName))
                CCore.Program.SetActorTypeShow(world, CCore.Program.DynamicBlockTypeName, true);

            var mesh = new CCore.Mesh.Mesh();
            var mshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = CSUtility.Support.IFileConfig.DynamicBlockMeshTemplate,
            };
            mshInit.CanHitProxy = false;
            mesh.Initialize(mshInit, null);

            var actInit = new CSUtility.Map.DynamicBlock.DynamicBlockActorInit();
            actInit.DynamicBlockData.Id = Guid.NewGuid();
            var actor = new CCore.World.DynamicBlockActor(actInit.DynamicBlockData.Id);
            actor.Initialize(actInit);
            actor.Visual = mesh;

            return actor;
        }
        /// <summary>
        /// 获取显示属性的对象
        /// </summary>
        /// <returns>返回该对象</returns>
        public object GetPropertyShowObject()
        {
            return null;
        }

        #endregion
        /// <summary>
        /// 是否碰撞
        /// </summary>
        public bool IsBlock
        {
            get
            {
                var init = mActorInit as CSUtility.Map.DynamicBlock.DynamicBlockActorInit;
                if(init == null || init.DynamicBlockData == null)
                    return true;

                return init.DynamicBlockData.IsBlock;
            }
            set
            {
                var init = mActorInit as CSUtility.Map.DynamicBlock.DynamicBlockActorInit;
                if (init != null && init.DynamicBlockData != null)
                {
                    init.DynamicBlockData.IsBlock = value;
               }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DynamicBlockActor()
        {
            mId = GenId();
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="id">对象的ID</param>
        public DynamicBlockActor(Guid id)
        {
            mId = id;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();

            if(this.Placement != null)
            {
                this.Placement.OnLocationChanged -= OnLocationChanged;
                this.Placement.OnRotationChanged -= OnRotationChanged;
                this.Placement.OnScaleChanged -= OnScaleChanged;
                this.Placement.Cleanup();
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~DynamicBlockActor()
        {

            Cleanup();
        }

        private void OnLocationChanged(ref SlimDX.Vector3 loc)
        {
            var init = mActorInit as CSUtility.Map.DynamicBlock.DynamicBlockActorInit;
            if (init != null && init.DynamicBlockData != null)
                init.DynamicBlockData.TransMatrix = mPlacement.mMatrix;

        }
        private void OnRotationChanged(ref SlimDX.Quaternion rot)
        {
            var init = mActorInit as CSUtility.Map.DynamicBlock.DynamicBlockActorInit;
            if (init != null && init.DynamicBlockData != null)
                init.DynamicBlockData.TransMatrix = mPlacement.mMatrix;
        }
        private void OnScaleChanged(ref SlimDX.Vector3 scale)
        {
            var init = mActorInit as CSUtility.Map.DynamicBlock.DynamicBlockActorInit;
            if (init != null && init.DynamicBlockData != null)
                init.DynamicBlockData.TransMatrix = mPlacement.mMatrix;
        }

        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">初始化该对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            Cleanup();

            if (base.Initialize(_init) == false)
                return false;

            SetPlacement(new CSUtility.Component.StandardPlacement(this));
            this.Placement.OnLocationChanged += OnLocationChanged;
            this.Placement.OnRotationChanged += OnRotationChanged;
            this.Placement.OnScaleChanged += OnScaleChanged;

            //var dyInit = _init as CSUtility.Data.MapObject.DynamicBlock.DynamicBlockActorInit;
            //if(dyInit != null)
            //    dyInit.DynamicBlockData.RoleId = this.Id;

            return true;
        }
        /// <summary>
        /// 获取层名称
        /// </summary>
        /// <returns>返回层名称为DynamicBlock</returns>
        public override string GetLayerName()
        {
            return "DynamicBlock";
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

            if(Visual != null)
            {
                //if (CCore.Engine.Instance.Client.MainWorld.IsActorGameTypeShow((UInt16)CSUtility.Component.EActorGameType.DynamicBlock))
                if(CCore.Program.IsActorTypeShow(this.World, CCore.Program.DynamicBlockTypeName))
                    Visual.Visible = true;
                else
                    Visual.Visible = false;
            }
        }
        /// <summary>
        /// 获取显示属性的对象
        /// </summary>
        /// <returns>返回当前对象数据</returns>
        public override object GetShowPropertyObj()
        {
            var init = mActorInit as CSUtility.Map.DynamicBlock.DynamicBlockActorInit;
            if (init != null)
                return init.DynamicBlockData;

            return null;
        }

        /// <summary>
        /// 获取服务器端动态阻挡对象的阻挡数据的方法
        /// </summary>
        public static Action<DynamicBlockActor> LoadSceneDataRPCAction = null;

        /// <summary>
        /// 从XND文件中加载场景数据
        /// </summary>
        /// <param name="attribute">XND数据节点</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
            if (!base.LoadSceneData(attribute))
                return false;

            var init = mActorInit as CSUtility.Map.DynamicBlock.DynamicBlockActorInit;
            if(init == null || init.DynamicBlockData == null)
                return true;

            LoadSceneDataRPCAction?.Invoke(this);

            return true;
        }
    }
}
