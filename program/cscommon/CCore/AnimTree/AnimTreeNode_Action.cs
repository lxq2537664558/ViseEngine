using System;
using System.Collections.Generic;

namespace CCore.AnimTree
{
    /// <summary>
    /// 动作的动画树
    /// </summary>
    public class AnimTreeNode_Action : AnimTreeNode, CSUtility.Animation.BaseAction
    {
        CSUtility.Animation.EActionPlayerMode mPlayerMode;
		//float m_BlendFactor;
		System.String mActionName;
        CSUtility.Animation.ActionSource mActionSource;
        /// <summary>
        /// 源动作的只读属性
        /// </summary>
        public CSUtility.Animation.ActionSource ActionSource
        {
            get { return mActionSource; }
        }
        /// <summary>
        /// 动作名字属性，可以进行设置和修改
        /// </summary>
        [System.ComponentModel.Category("动作属性")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("ActionSet")]
        public virtual string ActionName
        {
            get{return mActionName;}
            set
            {
                mActionName = value;
                SetAction(value);
            }
        }
        /// <summary>
        /// 播放模式属性
        /// </summary>
        [System.ComponentModel.Category("动作属性")]
        public virtual CSUtility.Animation.EActionPlayerMode PlayerMode
        {
            get{return mPlayerMode;}
            set
            {
		        mPlayerMode = value;
		        switch (mPlayerMode)
		        {
		        case CSUtility.Animation.EActionPlayerMode.Default:
			        SetLoop(false);
			        break;
		        case CSUtility.Animation.EActionPlayerMode.Loop:
			        SetLoop(true);
			        break;
		        case CSUtility.Animation.EActionPlayerMode.Pause:
			        SetLoop(false);
			        break;
		        }
            }
        }
        /// <summary>
        /// 动作的持续时间只读属性
        /// </summary>
        [System.ComponentModel.Category("动作属性")]
        public virtual Int64 Duration
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dAnimTreeNode_SubAction_GetDuration(mInner);
                }
            }
        }
        /// <summary>
        /// 动作的播放速度
        /// </summary>
        [System.ComponentModel.Category("动作属性")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 5)]
        public virtual double PlayRate
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dAnimTreeNode_SubAction_GetPlayRate(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dAnimTreeNode_SubAction_SetPlayRate(mInner, (float)value);
                }
            }
        }
        /// <summary>
        /// 当前的动作时间
        /// </summary>
        public virtual Int64 CurAnimTime
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dAnimTreeNode_SubAction_GetCurAnimTime(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dAnimTreeNode_SubAction_SetCurAnimTime(mInner, value);
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AnimTreeNode_Action()
        {
            mAction = this;
        }
        /// <summary>
        /// 析构函数，释放内存
        /// </summary>
        ~AnimTreeNode_Action()
        {
            Cleanup();
        }
        /// <summary>
        /// 对象初始化，创建实例
        /// </summary>
        /// <returns>初始化成功返回true</returns>
        public override bool Initialize()
        {
            Cleanup();

            mInner = DllImportAPI.v3dAnimTreeNode_SubAction_New();

            return true;
        }
        /// <summary>
        /// 设置动作名称
        /// </summary>
        /// <param name="name">动作的名称</param>
		public void SetAction( string name )
        {
		    if( mInner == IntPtr.Zero )
			    return; 
           
		    float fPreElapsed = DllImportAPI.v3dAnimTreeNode_SubAction_GetCurAnimTime(mInner);
            DllImportAPI.v3dAnimTreeNode_SubAction_SetSubAction(mInner, Engine.Instance.Client.Graphics.Device, name, true);

		    mActionSource = CSUtility.Animation.ActionNodeManager.Instance.GetActionSource(name, false, CSUtility.Helper.enCSType.Client);

		    // DEBUG代码
		    //System.String actionName = name.Substring(name.LastIndexOf("\\") + 1);
		    //if(actionName.Contains("ZombieChampion"))
		    //	System.Diagnostics.Debug.WriteLine(System.String.Format("{0}    {1}", fPreElapsed, actionName ));
        }
        /// <summary>
        /// 清空链接，将该动作从组里面删除
        /// </summary>
		public void ClearLink()
        {
		    if( mInner == IntPtr.Zero )
			    return; 
            DllImportAPI.v3dAnimTreeNode_SubAction_ClearLink(mInner);
        }
        /// <summary>
        /// 得到动作的名称
        /// </summary>
        /// <returns>返回该动作对象的名称</returns>
		System.String GetActionName()
        {
		    if( mInner == IntPtr.Zero )
			    return null;
            return DllImportAPI.v3dAnimTreeNode_SubAction_GetActionName(mInner);
        }
        /// <summary>
        /// 设置是否进行循环播放
        /// </summary>
        /// <param name="bLoop">是否循环播放</param>
		public override void SetLoop(bool bLoop)
        {
		    if( mInner == IntPtr.Zero )
			    return; 
            DllImportAPI.v3dAnimTreeNode_SubAction_SetLoop(mInner, bLoop?1:0);
        }
        /// <summary>
        /// 得到该动作是否进行循环播放
        /// </summary>
        /// <returns>返回动作是否循环播放</returns>
        public override bool GetLoop()
        {
		    if( mInner == IntPtr.Zero )
			    return false;
            return DllImportAPI.v3dAnimTreeNode_SubAction_GetLoop(mInner)==0?false:true;
        }
        float mActionSourcePlayRate = 1.0f;
        /// <summary>
        /// 设置播放速度
        /// </summary>
        /// <param name="playRate">播放速度</param>
        public override void SetPlayRate(float playRate)
        {
            mActionSourcePlayRate = playRate;
            PlayRate = PlayRate * mActionSourcePlayRate;
        }
        /// <summary>
        /// 得到动作的播放速度
        /// </summary>
        /// <returns>返回该对象的播放速度</returns>
        public override float GetPlayRate()
        {
            return (float)PlayRate* mActionSourcePlayRate;
        }
        /// <summary>
        /// 通过名字得到动作的关键帧
        /// </summary>
        /// <param name="name">关键帧的名称</param>
        /// <returns>返回动作的关键帧</returns>
        public virtual CSUtility.Animation.ActionNotifier GetNotifier(System.String name)
        {
		    if(mActionSource==null)
			    return null;

            return mActionSource.GetFirstNotifier(name);
            //foreach (CSUtility.Animation.ActionNotifier notify in mActionSource.NotifierList)
            //{
            //    if(notify.NotifyName == name)
            //        return notify;
            //}

		    //return null;
        }
        /// <summary>
        /// 通过索引得到动作的关键帧
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回动作的关键帧</returns>
        public virtual CSUtility.Animation.ActionNotifier GetNotifier(System.UInt32 index)
        {
            if (mActionSource == null)
                return null;

            return mActionSource.GetNotifier((int)index);
            //if ((int)index < mActionSource.NotifierList.Count)
            //    return mActionSource.NotifierList[(int)index];

            //return null;
        }
        /// <summary>
        /// 得到某一类型的所有的关键帧
        /// </summary>
        /// <param name="notifyType">关键帧类型</param>
        /// <returns>返回关键帧列表</returns>
        public virtual List<CSUtility.Animation.ActionNotifier> GetNotifiers(Type notifyType)
        {
            if (mActionSource == null)
                return new List<CSUtility.Animation.ActionNotifier>();

            return mActionSource.GetNotifier(notifyType);
        }
        /// <summary>
        /// 得到动作的所有的关键帧
        /// </summary>
        /// <returns>返回该对象所有的关键帧</returns>
        public virtual CSUtility.Animation.ActionNotifier[] GetNotifiers()
        {
            if (mActionSource == null)
                return new CSUtility.Animation.ActionNotifier[0];

            return mActionSource.NotifierList;
        }

    }
}
