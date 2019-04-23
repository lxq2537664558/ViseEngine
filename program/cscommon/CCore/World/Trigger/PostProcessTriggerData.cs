using System;
using System.Collections.Generic;
using System.Text;
using CSUtility.Map.Trigger;
/// <summary>
/// 触发器的命名空间
/// </summary>
namespace CCore.World.Trigger
{
    /// <summary>
    /// 过程触发器的初始化类
    /// </summary>
    public class PostProcessTriggerDataInit : CSUtility.Map.Trigger.TriggerActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PostProcessTriggerDataInit()
        {
            ForServer = false;
            GameType = (UInt16)CSUtility.Component.EActorGameType.Trigger;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.SaveWithServer;
            // 临时测试添加
            TriggerData = new PostprocessTriggerData();
            TriggerData.Id = Guid.NewGuid();
            //TriggerData.Id = Guid.Parse("bd145de4-5486-2145-6ed1-acde254105d1");
        }
    }
    /// <summary>
    /// 后期特效触发器类
    /// </summary>
    [CCore.EditorAssist.PlantAbleAttribute("触发器.后期特效触发器", "", "")]
    public class PostprocessTriggerData : CSUtility.Map.Trigger.TriggerData, EditorAssist.IPlantAbleObject
    {
        #region 种植选项
        /// <summary>
        /// 获取种植的Actor对象
        /// </summary>
        /// <param name="world">种植触发器的世界</param>
        /// <returns>返回种植的对象</returns>
        public CCore.World.Actor GetPlantActor(CCore.World.World world)
        {
            var triggerActor = new CCore.World.TriggerActor();
            var triggerActorInit = new PostProcessTriggerDataInit();
            triggerActor.Initialize(triggerActorInit);
            triggerActor.SetPlacement(new CSUtility.Component.StandardPlacement(triggerActor));
            triggerActor.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(triggerActor.Id));
            if (mPropertyObject != null)
            {
                triggerActor = mPropertyObject.Duplicate() as CCore.World.TriggerActor;
            }
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
            var triggerActorInit = new PostProcessTriggerDataInit();
            triggerActor.Initialize(triggerActorInit);
            triggerActor.SetPlacement(new CSUtility.Component.StandardPlacement(triggerActor));
            triggerActor.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(triggerActor.Id));
            if (mPropertyObject != null)
            {
                triggerActor = mPropertyObject.Duplicate() as CCore.World.TriggerActor;
            }
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
            var triggerActorInit = new PostProcessTriggerDataInit();
            triggerActor.Initialize(triggerActorInit);
            triggerActor.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(triggerActor.Id));
            //mPropertyObject = triggerActor;
            return triggerActorInit.TriggerData;
        }

        #endregion
        /// <summary>
        /// 构造函数
        /// </summary>
        public PostprocessTriggerData()
        {
            CCore.Graphics.PostProcess_SSAO ssao = new CCore.Graphics.PostProcess_SSAO();
            ssao.Enable = true;
            ssao.RandomNormalTexName = CSUtility.Support.IFileConfig.DefaultRandomNormalFile;
            ssao.DoBlur = true;
            ssao.SampleRad = 1;
            ssao.Bias = 0.15f;
            ssao.Intensity = 1;
            mPostProcesses.Add(ssao);

            CCore.Graphics.PostProcess_Bloom bloom = new CCore.Graphics.PostProcess_Bloom();
            bloom.BloomImageScale = 0.5f;
            bloom.BlurStrength = 0.5f;
            bloom.BlurAmount = 5;
            bloom.BlurType = CCore.Graphics.enBlurType.BoxBlur;
            mPostProcesses.Add(bloom);

            CCore.Graphics.PostProcess_ToneMapping toneMapping = new CCore.Graphics.PostProcess_ToneMapping();
            mPostProcesses.Add(toneMapping);

            CCore.Graphics.PostProcess_ColorGrading colorGrading = new CCore.Graphics.PostProcess_ColorGrading();
            colorGrading.ColorGradingTexName = CSUtility.Support.IFileConfig.DefaultColorGradeFile;
            mPostProcesses.Add(colorGrading);

        }
        /// <summary>
        /// 插值处理
        /// </summary>
        /// <param name="src">源对象列表</param>
        /// <param name="dest">目标对象列表</param>
        void LerpPostProcess(List<CCore.Graphics.PostProcess> src, List<CCore.Graphics.PostProcess> dest)
        {
            foreach (var srcP in src)
            {
                foreach (var destP in dest)
                {
                    if (srcP.m_Type == destP.m_Type)
                    {
                        destP.LerpFrom(srcP, mLerpDuration, 1 - srcP.LerpValue);
                    }
                }
            }
        }
        //[CSUtility.AISystem.Attribute.ToolTip("添加触发器逻辑")]
        //[CSUtility.AISystem.Attribute.AllowMember("触发器.函数.激活触发器", CSUtility.Helper.enCSType.Common, "添加一个触发器")]
        //[CSUtility.Event.Attribute.AllowMember("触发器.函数.激活触发器", CSUtility.Helper.enCSType.Common, "添加一个触发器")]
        /// <summary>
        /// 进入触发器范围
        /// </summary>
        /// <param name="processData">触发器对象</param>
        public override void ProcessEnterOverride(CSUtility.Map.Trigger.TriggerProcessData processData)
        {
            mBackupPostProcesses.Clear();
            mBackupPostProcesses.AddRange(Client.MainWorldInstance.PostProceses);
            Client.MainWorldInstance.PostProceses.Clear();
            Client.MainWorldInstance.PostProceses.AddRange(mPostProcesses);

            LerpPostProcess(mBackupPostProcesses, mPostProcesses);
        }
        //[CSUtility.AISystem.Attribute.ToolTip("移除触发器逻辑")]
        //[CSUtility.AISystem.Attribute.AllowMember("触发器.函数.移除触发器", CSUtility.Helper.enCSType.Common, "删除一个触发器")]
        //[CSUtility.Event.Attribute.AllowMember("触发器.函数.移除触发器", CSUtility.Helper.enCSType.Common, "删除一个触发器")]
        /// <summary>
        /// 离开触发器范围
        /// </summary>
        /// <param name="processData">触发器对象</param>
        public override void ProcessLeaveOverride(CSUtility.Map.Trigger.TriggerProcessData processData)
        {
            Client.MainWorldInstance.PostProceses.Clear();
            Client.MainWorldInstance.PostProceses.AddRange(mBackupPostProcesses);

            LerpPostProcess(mPostProcesses, mBackupPostProcesses);
        }

        long mLerpDuration = 2000;
        /// <summary>
        /// 过渡时间
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [System.ComponentModel.DisplayName("过渡时间"), System.ComponentModel.Category("后期特效")]
        public long LerpDuration
        {
            get { return mLerpDuration; }
            set
            {
                mLerpDuration = value;
            }
        }

        List<CCore.Graphics.PostProcess> mPostProcesses = new List<CCore.Graphics.PostProcess>();
        /// <summary>
        /// 过程出发列表
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        //[System.ComponentModel.Browsable(false)]

        [System.ComponentModel.DisplayName("过程出发列表"), System.ComponentModel.Category("后期特效")]
        public List<CCore.Graphics.PostProcess> PostProcesses
        {
            get { return mPostProcesses; }
            set
            {
                mPostProcesses = value;
            }
        }

        List<CCore.Graphics.PostProcess> mBackupPostProcesses = new List<CCore.Graphics.PostProcess>();

    }
}