using System;
/// <summary>
/// 世界对象的命名空间
/// </summary>
namespace CCore.World
{
    /// <summary>
    /// 普通触发器类
    /// </summary>
    [CCore.EditorAssist.PlantAbleAttribute("触发器.普通触发器", "", "")]
    public class TriggerActor : CCore.World.Actor, EditorAssist.IPlantAbleObject
    {
        #region 编辑器种植
        /// <summary>
        /// 获取种植的对象
        /// </summary>
        /// <param name="world">种植的世界</param>
        /// <returns>返回种植的对象</returns>
        public CCore.World.Actor GetPlantActor(CCore.World.World world)
        {
            var triggerActor = new CCore.World.TriggerActor();
            var triggerActorInit = new CSUtility.Map.Trigger.TriggerActorInit();
            
            triggerActor.Initialize(triggerActorInit);                        
            if (mPropertyObject != null)
            {
                triggerActor = mPropertyObject.Duplicate() as CCore.World.TriggerActor;
            }
            triggerActor.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(triggerActor.Id));
            triggerActor.ActorName = "触发器" + Program.GetActorIndex();
            
            return triggerActor;
        }
        /// <summary>
        /// 获取预览用对象，在拖动对象进入场景时显示预览对象
        /// </summary>
        /// <param name="world">拖动进入的世界</param>
        /// <returns>返回预览用的对象</returns>
        public CCore.World.Actor GetPreviewActor(CCore.World.World world)
        {
            if (!CCore.Program.IsActorTypeShow(world, CCore.Program.TriggerAssistTypeName))
                CCore.Program.SetActorTypeShow(world, CCore.Program.TriggerAssistTypeName, true);

            var triggerActor = new CCore.World.TriggerActor();
            var triggerActorInit = new CSUtility.Map.Trigger.TriggerActorInit();
            
            triggerActor.Initialize(triggerActorInit);                        
            if (mPropertyObject != null)
            {
                triggerActor = mPropertyObject.Duplicate() as CCore.World.TriggerActor;                
            }
            triggerActor.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(triggerActor.Id));
            return triggerActor;
        }
        CCore.World.TriggerActor mPropertyObject = null;
        /// <summary>
        /// 获取需要显示属性的对象
        /// </summary>
        /// <returns>返回显示属性的对象</returns>
        public object GetPropertyShowObject()
        {
            var triggerActor = new CCore.World.TriggerActor();
            var triggerActorInit = new CSUtility.Map.Trigger.TriggerActorInit();
            triggerActor.Initialize(triggerActorInit);
            triggerActor.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(triggerActor.Id));
            mPropertyObject = triggerActor;
            return triggerActor.TriggerActorInit.TriggerData;
        }
        #endregion
        /// <summary>
        /// 触发器属性更新时的委托事件
        /// </summary>
        public CSUtility.Map.Trigger.TriggerData.Delegate_OnPropertyUpdate OnTriggerDataPropertyUpdate;
        /// <summary>
        /// 触发器初始化对象
        /// </summary>
        public CSUtility.Map.Trigger.TriggerActorInit TriggerActorInit;
        /// <summary>
        /// 当前对象的ID
        /// </summary>
        public override Guid Id
        {
            get
            {
                if (TriggerActorInit == null)
                    return Guid.Empty;
                if (TriggerActorInit.TriggerData == null)
                    return Guid.Empty;
                return TriggerActorInit.TriggerData.Id;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public TriggerActor()
        {
            mPlacement = new CSUtility.Component.StandardPlacement(this);

            this.Placement.OnLocationChanged += OnLocationChanged;
            this.Placement.OnRotationChanged += OnRotationChanged;
            this.Placement.OnScaleChanged += OnScaleChanged;
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();

            this.Placement.OnLocationChanged -= OnLocationChanged;
            this.Placement.OnRotationChanged -= OnRotationChanged;
            this.Placement.OnScaleChanged -= OnScaleChanged;

            if(TriggerActorInit != null && TriggerActorInit.TriggerData != null)
                TriggerActorInit.TriggerData.OnPropertyUpdate -= _OnTriggerDataPropertyUpdate;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~TriggerActor()
        {
            Cleanup();
        }

        private void _OnTriggerDataPropertyUpdate(CSUtility.Map.Trigger.TriggerData data)
        {
            if (OnTriggerDataPropertyUpdate != null)
                OnTriggerDataPropertyUpdate(data);
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">对象初始化类</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            if (base.Initialize(_init) == false)
                return false;
            
            Visual = new CCore.Component.TriggerVisual();

            TriggerActorInit = _init as CSUtility.Map.Trigger.TriggerActorInit;
            if (TriggerActorInit.TriggerData != null)
            {
                TriggerActorInit.TriggerData.OnPropertyUpdate += _OnTriggerDataPropertyUpdate;
                var transMat = TriggerActorInit.TriggerData.TransMatrix;
                this.Placement.SetMatrix(ref transMat);
            }
            
            return true;
        }

        private void OnLocationChanged(ref SlimDX.Vector3 loc)  
        {
            TriggerActorInit.TriggerData.TransMatrix = mPlacement.mMatrix;
        }
        private void OnRotationChanged(ref SlimDX.Quaternion rot)
        {
            TriggerActorInit.TriggerData.TransMatrix = mPlacement.mMatrix;
        }
        private void OnScaleChanged(ref SlimDX.Vector3 scale)
        {
            TriggerActorInit.TriggerData.TransMatrix = mPlacement.mMatrix;
        }
        /// <summary>
        /// 提交可视化对象
        /// </summary>
        /// <param name="env">渲染环境</param>
        /// <param name="matrix">对象的矩阵</param>
        /// <param name="eye">视野</param>
        public override void OnCommitVisual(CCore.Graphics.REnviroment env, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            
        }
        /// <summary>
        /// 获取显示对象的属性
        /// </summary>
        /// <returns>返回触发器数据</returns>
        public override object GetShowPropertyObj()
        {
            return TriggerActorInit?.TriggerData;
        }
        /// <summary>
        /// 保存场景数据
        /// </summary>
        /// <param name="attribute">XND数据文件</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public override bool SaveSceneData(CSUtility.Support.XndAttrib attribute)
        {
            attribute.Write(Id);

            attribute.Write(CSUtility.Program.GetTypeSaveString(mActorInit.GetType()));
            mActorInit.Write(attribute);

            return true;
        }
        /// <summary>
        /// 加载场景数据
        /// </summary>
        /// <param name="attribute">XND文件</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
            Cleanup();

            Guid id;
            attribute.Read(out id);
            this.mId = id;

            System.String initTypeStr;
            attribute.Read(out initTypeStr);
            System.Type initType = CSUtility.Program.GetTypeFromSaveString(initTypeStr);
            if (initType == null)
            {
                initType = typeof(CSUtility.Map.Trigger.TriggerActorInit);
            }
            var actInit = (CSUtility.Component.ActorInitBase)System.Activator.CreateInstance(initType);
            actInit.Read(attribute);
            Initialize(actInit);
            
            return true;
        }

        CSUtility.Support.ConcurentObjManager<Guid, CSUtility.Map.Trigger.TriggerProcessData> mProcessDatas = new CSUtility.Support.ConcurentObjManager<Guid, CSUtility.Map.Trigger.TriggerProcessData>();
        /// <summary>
        /// 后期触发数据
        /// </summary>
        /// <param name="processData">后期触发的数据属性</param>
        public void ProcessTrigger(CSUtility.Map.Trigger.TriggerProcessData processData)
        {
            if (processData == null)
                return;

            if (!TriggerActorInit.TriggerData.Enable)
                return;

            if (TriggerActorInit.TriggerData.ProcessType == CSUtility.Map.Trigger.TriggerData.enProcessType.Server)
                return;

            if (mProcessDatas.ContainsKey(processData.Id))
                return;

            if (!processData.CanProcessEnter(TriggerActorInit.TriggerData))
                return;

            processData.EnterTime = CCore.Engine.Instance.GetFrameMillisecond();
            mProcessDatas.Add(processData.Id, processData);

            // 调用进入回调
            TriggerActorInit.TriggerData.ProcessEnter(processData);
        }

        static CSUtility.Performance.PerfCounter TickTriggerActorTimer = new CSUtility.Performance.PerfCounter("TileScene.TickTriggerActor");
        long mElapsedMillisecond = 0;
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public override void Tick(long elapsedMillisecond)
        {
            TickTriggerActorTimer.Begin();
            base.Tick(elapsedMillisecond);
            if (TriggerActorInit.TriggerData == null)
                return;
            if (!TriggerActorInit.TriggerData.Enable)
                return;
            if (TriggerActorInit.TriggerData.ProcessType == CSUtility.Map.Trigger.TriggerData.enProcessType.Server)
                return;

            mElapsedMillisecond += elapsedMillisecond;
            if (mElapsedMillisecond > TriggerActorInit.TriggerData.TickIntervalMillisecond)
            {
                mElapsedMillisecond = 0;

                //TriggerActorInit.TriggerData.Tick(elapsedMillisecond);

                mProcessDatas.For_Each((Guid id, CSUtility.Map.Trigger.TriggerProcessData processData, object arg) =>
                {
                    processData.Tick(elapsedMillisecond);

                    if (processData.CanProcessLeave(TriggerActorInit.TriggerData))
                    {
                        processData.LeaveTime = CCore.Engine.Instance.GetFrameMillisecond();
                        TriggerActorInit.TriggerData.ProcessLeave(processData);
                        return CSUtility.Support.EForEachResult.FER_Erase;
                    }

                    return CSUtility.Support.EForEachResult.FER_Continue;
                }, null);

                TriggerActorInit.TriggerData.Tick(elapsedMillisecond);
            }
            TickTriggerActorTimer.End();
        }
        /// <summary>
        /// 复制Actor对象
        /// </summary>
        /// <returns>返回复制的对象</returns>
        public override Actor Duplicate()
        {
            CSUtility.Component.ActorInitBase copyedActorInit = null;
            if (this.ActorInit != null)
            {
                copyedActorInit = (CSUtility.Component.ActorInitBase)System.Activator.CreateInstance(this.ActorInit.GetType());
                copyedActorInit.CopyFrom(this.ActorInit);
            }
            var srcActorTypeStr = this.GetType().FullName;
            var copyedActor = (Actor)this.GetType().Assembly.CreateInstance(srcActorTypeStr);
            copyedActor.Initialize(copyedActorInit);

            return copyedActor;
        }
        #region Editor
        /// <summary>
        /// 获取层名称
        /// </summary>
        /// <returns>返回当前的层名称为Trigger</returns>
        public override string GetLayerName()
        {
            return "Trigger";
        }

        #endregion

    }
}
