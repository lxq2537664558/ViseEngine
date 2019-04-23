using System;

namespace CCore.World
{
    /// <summary>
    /// 特效对象的初始化类
    /// </summary>
    public class EffectActorInit : ActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EffectActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Effect;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient;
        }
        /// <summary>
        /// 是否可以被点击选中
        /// </summary>
        protected bool mHitProxy = false;
        /// <summary>
        /// 是否可以被点击选中的属性
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool HitProxy
        {
            get { return mHitProxy;  }
            set { mHitProxy = value; }
        }
    }
    /// <summary>
    /// 特效对象的类
    /// </summary>
    public class EffectActor : Actor
    {
        bool mLoopPlay = false;
        /// <summary>
        /// 是否循环播放特效
        /// </summary>
        public bool LoopPlay
        {
            get { return mLoopPlay; }
            set { mLoopPlay = value; }
        }
        /// <summary>
        /// 是否可以被点击
        /// </summary>
        public bool HitProxy
        {
            get 
            {
                var actorInit = ActorInit as EffectActorInit;
                return actorInit.HitProxy;
            }
            set 
            {
                var actorInit = ActorInit as EffectActorInit;
                actorInit.HitProxy = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public EffectActor()
        {

        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">用于初始化该对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            if (!base.Initialize(_init))
                return false;

            return true;
        }
        /// <summary>
        /// 保存场景数据
        /// </summary>
        /// <param name="attribute">XND文件节点</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public override bool SaveSceneData(CSUtility.Support.XndAttrib attribute)
        {
            return base.SaveSceneData(attribute);
        }
        /// <summary>
        /// 加载场景数据
        /// </summary>
        /// <param name="attribute">XND数据节点</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
            if (!base.LoadSceneData(attribute))
                return false;

            var effectVis = Visual as CCore.Component.EffectVisual;
            if (effectVis != null)
            {
                if (effectVis.CanHitProxy)
                {
                    effectVis.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(this.Id));
                }
            }

            return true;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(long elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);

            var effectVis = Visual as CCore.Component.EffectVisual;
            if (effectVis != null && effectVis.EffectInit != null && effectVis.EffectInit.EffectTemplate != null)
            {
                //if (!float.IsPositiveInfinity(effectVis.EffectInit.EffectTemplate.TotalTime))
                if(effectVis.EffectInit.EffectTemplate.IsFinished)
                {
                    if (!LoopPlay)
                    {
                        CCore.Client.MainWorldInstance.RemoveActor(this);
                    }
                }
            }
        }
    }
}
