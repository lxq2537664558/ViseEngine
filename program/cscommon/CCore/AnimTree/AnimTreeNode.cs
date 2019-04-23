using System;
using System.Collections.Generic;

namespace CCore.AnimTree
{
    /// <summary>
    /// 树形动画节点
    /// </summary>
    public class AnimTreeNode : CSUtility.Animation.AnimationTree
    {
        /// <summary>
        /// 实例地址，初始化为空
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 实例地址的属性，用于设置和得到实例的内存地址
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                mInner = value;
            }
        }
        /// <summary>
        /// 树形结构的孩子列表
        /// </summary>
		protected List<CSUtility.Animation.AnimationTree>		mChildren;
        /// <summary>
        /// 动画的动作基类类型
        /// </summary>
		protected CSUtility.Animation.BaseAction mAction;
        /// <summary>
        /// 动画的动作基类类型属性，用于得到和设置该动画的Action
        /// </summary>
        public virtual CSUtility.Animation.BaseAction Action
        {
            get 
            {
                if (mAction != null)
                    return mAction;
                foreach (var child in mChildren)
                {
                    var childNode = child as AnimTreeNode;
                    var childAction = childNode.Action;
                    if (childAction != null)
                        return childAction;
                }
                return null;
            }
            set
            {
                mAction = value;
            }
        }

		System.Int64								mCurNotifyTime;
        /// <summary>
        /// 当前的监听时间
        /// </summary>
        public System.Int64 CurNotifyTime
        {
            get { return mCurNotifyTime; }
            set
            {
                mCurNotifyTime = value;
            }
        }

		CCore.Skeleton.Skeleton mSkeleton;

        CSUtility.Animation.Delegate_OnAnimTreeFinish		mDelegateOnAnimTreeFinish;
        /// <summary>
        /// 动画树完成的委托事件
        /// </summary>
        public CSUtility.Animation.Delegate_OnAnimTreeFinish DelegateOnAnimTreeFinish
        {
            get { return mDelegateOnAnimTreeFinish; }
            set
            {
                mDelegateOnAnimTreeFinish = value;
            }
        }

        CSUtility.Animation.Delegate_OnActionFinish		mDelegateOnActionFinish;
        /// <summary>
        /// 该动作完成的委托事件
        /// </summary>
        public CSUtility.Animation.Delegate_OnActionFinish DelegateOnActionFinish
        {
            get { return mDelegateOnActionFinish; }
            set
            {
                mDelegateOnActionFinish = value;
            }
        }
        /// <summary>
        /// X轴向根动作的类型
        /// </summary>
        [System.ComponentModel.Category("RootMotion")]
        public virtual CSUtility.Animation.AxisRootmotionType XRootmotionType
        {
            get
            {
                unsafe
                {
                    return (CSUtility.Animation.AxisRootmotionType)DllImportAPI.V3DAnimTreeNode_GetXRootMotionType(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DAnimTreeNode_SetXRootMotionType(mInner, (int)value);
                }
            }
        }
        /// <summary>
        /// Y轴向根动作的类型
        /// </summary>
		[System.ComponentModel.Category("RootMotion")]
        public virtual CSUtility.Animation.AxisRootmotionType YRootmotionType
        {
            get
            {
                unsafe
                {
                    return (CSUtility.Animation.AxisRootmotionType)DllImportAPI.V3DAnimTreeNode_GetYRootMotionType(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DAnimTreeNode_SetYRootMotionType(mInner, (int)value);
                }
            }
        }
        /// <summary>
        /// Z轴向根动作的类型
        /// </summary>
		[System.ComponentModel.Category("RootMotion")]
        public virtual CSUtility.Animation.AxisRootmotionType ZRootmotionType
        {
            get
            {
                unsafe
                {
                    return (CSUtility.Animation.AxisRootmotionType)DllImportAPI.V3DAnimTreeNode_GetZRootMotionType(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DAnimTreeNode_SetZRootMotionType(mInner, (int)value);
                }
            }
        }
        /// <summary>
        /// 该动作的混合持续时间
        /// </summary>
        public int BlendDuration
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DAnimTreeNode_GetBlendDuration(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DAnimTreeNode_SetBlendDuration(mInner, value);
                }
            }
        }
        /// <summary>
        /// 是否每帧优先渲染，默认为false
        /// </summary>
        public bool mDoFirstTick = false;
        /// <summary>
        /// 动画树节点的构造函数，创建孩子列表和骨骼并指定根动作类型为转换
        /// </summary>
	    public AnimTreeNode()
	    {
		    mChildren = new List<CSUtility.Animation.AnimationTree>();
		    mSkeleton = new CCore.Skeleton.Skeleton();

            XRootmotionType = CSUtility.Animation.AxisRootmotionType.ART_Translate;
	    }
        /// <summary>
        /// 析构函数，释放实例的内存
        /// </summary>
        ~AnimTreeNode()
	    {
		    Cleanup();
	    }
	    /// <summary>
        /// 将该实例以及其孩子的删除并释放对应的内存
        /// </summary>
        public void Cleanup()
	    {
		    foreach(AnimTreeNode i in mChildren)
		    {
			    i.Cleanup();
		    }
		    mChildren.Clear();
            if(mInner != IntPtr.Zero)
            {
                DllImportAPI.V3DAnimTreeNode_Release(mInner);
                mInner = IntPtr.Zero;
            }
		    mSkeleton.Cleanup();
	    }
        /// <summary>
        /// 该对象的初始化函数，用于创建实例和骨骼
        /// </summary>
        /// <returns>初始化成功返回true</returns>
        public virtual bool Initialize()
        {
		    Cleanup();
            mInner = DllImportAPI.V3DAnimTreeNode_New();
            mSkeleton.Inner = DllImportAPI.V3DAnimTreeNode_GetSkeleton(mInner);

		    return true;
        }
        /// <summary>
        /// 通过已有的节点数据初始化该对象
        /// </summary>
        /// <param name="node">相应的节点数据</param>
        /// <returns>初始化成功返回true</returns>
        public virtual bool Initialize(CSUtility.Support.XndNode node)
        {
            Cleanup();

            mInner = DllImportAPI.V3DAnimTreeNode_New();
            DllImportAPI.V3DAnimTreeNode_LoadFromXnd(mInner, Engine.Instance.Client.Graphics.Device, node.GetRawNode());
            mSkeleton.Inner = DllImportAPI.V3DAnimTreeNode_GetSkeleton(mInner);

    		return true;
        }
        /// <summary>
        /// 根据XND数据里面的名字进行对象的初始化
        /// </summary>
        /// <param name="name">XND数据里保存的名字</param>
        /// <returns>初始化成功返回true</returns>
        public virtual bool Initialize(string name)
        {
		    Cleanup();

            mInner = DllImportAPI.V3DAnimTreeNode_New();

            //CSUtility.Support.IFileManager.Instance.OpenFileForRead(name, CSUtility.EFileType.Xnd, false);
            var holder = CSUtility.Support.XndHolder.LoadXND(name);
            if (holder != null)
            {
                var node = holder.Node.FindNode("AnimTreeNode");
                if (node != null)
                {
                    DllImportAPI.V3DAnimTreeNode_LoadFromXnd(mInner, Engine.Instance.Client.Graphics.Device, node.GetRawNode());
                    mSkeleton.Inner = DllImportAPI.V3DAnimTreeNode_GetSkeleton(mInner);
                }
            }
 
		    return true;
        }
        /// <summary>
        /// 将节点保存到XND数据
        /// </summary>
        /// <param name="node">保存该对象的XND的数据节点</param>
        public void SaveToXnd(CSUtility.Support.XndNode node)
	    {
            if (mInner == IntPtr.Zero)
                return;

            DllImportAPI.V3DAnimTreeNode_SaveToXnd(mInner, node.GetRawNode());
	    }
        /// <summary>
        /// 得到该对象的骨骼对象的地址
        /// </summary>
        /// <returns>骨骼对象的地址</returns>
        public IntPtr GetSkeleton()
        {
            return DllImportAPI.V3DAnimTreeNode_GetSkeleton(mInner);
        }
        /// <summary>
        /// 得到该对象下的骨骼并改变传入的骨骼对象
        /// </summary>
        /// <param name="skeleton">骨骼对象</param>
        public void GetSkeleton(ref CCore.Skeleton.Skeleton skeleton)
        {
            skeleton = mSkeleton;
        }
        /// <summary>
        /// 设置该对象的骨骼对象
        /// </summary>
        /// <param name="skeleton">骨骼对象</param>
		public void SetSkeleton( CCore.Skeleton.Skeleton skeleton )
        {
            DllImportAPI.V3DAnimTreeNode_SetSkeleton(mInner, Engine.Instance.Client.Graphics.Device, skeleton.Inner);
        }
        /// <summary>
        /// 更新动作节点以保证动作的正确性
        /// </summary>
        /// <param name="tm">间隔时间</param>
        public void UpdateCppInner( System.Int64 tm )
	    {
		    if( mInner!=IntPtr.Zero && DllImportAPI.V3DAnimTreeNode_IsActionFinished(mInner)==0 )
		    {
                DllImportAPI.V3DAnimTreeNode_UpdateTick(mInner, tm);
                DllImportAPI.V3DAnimTreeNode_UpdateNode(mInner, tm);
		    }
	    }
        /// <summary>
        /// 更新该对象及其孩子的信息，判断动作是否完成
        /// </summary>
        public void UpdateDoNet()
	    {
            if(mInner==IntPtr.Zero)
                return;

            if(mChildren.Count == 2)
            {
                var blendElapse = DllImportAPI.V3DAnimTreeNode_GetBlendElapse(mInner);
                var rate = (float)blendElapse / (float)BlendDuration;
                if(rate >= 1)
                {
                    RemoveNode(mChildren[1] as AnimTreeNode);
                }
            }

			if( DllImportAPI.V3DAnimTreeNode_IsActionFinished(mInner)!=0 )
			{
				if( mDelegateOnActionFinish!=null)
					mDelegateOnActionFinish();
			}
		    foreach(AnimTreeNode i in mChildren)
		    {
			    i.UpdateDoNet();
		    }
	    }
        /// <summary>
        /// 对象的更新函数
        /// </summary>
        /// <param name="tm">更新的时间间隔</param>
        public void Update( Int64 tm )
	    {
		    UpdateCppInner(tm);
		    UpdateDoNet();		
	    }
        /// <summary>
        /// 向对象及其孩子中添加动作树
        /// </summary>
        /// <param name="animtree">添加的动作树</param>
	    public void AddNode(CSUtility.Animation.AnimationTree animtree )
	    {
		    AnimTreeNode pNode = (AnimTreeNode)animtree;
		    if( mInner == IntPtr.Zero || pNode == null )
			    return;

            DllImportAPI.V3DAnimTreeNode_AddNode(mInner, pNode.Inner);
		    mChildren.Add(animtree);
	    }
        /// <summary>
        /// 删除指定的动作节点
        /// </summary>
        /// <param name="pNode">需要删除的动作节点</param>
	    public void RemoveNode( AnimTreeNode pNode )
	    {
            if (mInner == IntPtr.Zero || pNode == null)
                return;

            DllImportAPI.V3DAnimTreeNode_RemoveNode(mInner, pNode.Inner);
            mChildren.Remove(pNode);
	    }
        /// <summary>
        /// 将动作树置空
        /// </summary>
        public void ClearNode()
	    {
            if (mInner == IntPtr.Zero)
                return;

            DllImportAPI.V3DAnimTreeNode_ClearNode(mInner);
		    mChildren.Clear();
	    }
        /// <summary>
        /// 判断动作是否完成
        /// </summary>
        /// <returns>动作完成返回true，否则返回false</returns>
        public bool GetATFinished()
	    {
            if (mInner == IntPtr.Zero)
			    return false;
            if (CurNotifyTime >Action.Duration +100)
            {
                return true;
            }
		    return DllImportAPI.V3DAnimTreeNode_GetATFinished(mInner)==0 ? false : true;
	    }
        /// <summary>
        /// 设置动作是否完成
        /// </summary>
        /// <param name="bFinished">动作是否完成</param>
        public void SetATFinished(bool bFinished)
	    {
            if (mInner == IntPtr.Zero)
                return;
            DllImportAPI.V3DAnimTreeNode_SetATFinished(mInner, bFinished ? 1 : 0);
	    }
        /// <summary>
        /// 混合因子
        /// </summary>
        /// <returns>得到该对象的混合因子</returns>
        public float GetBlendFactor()
	    {
            if (mInner == IntPtr.Zero)
			    return 0.0f;
		    return  DllImportAPI.V3DAnimTreeNode_GetBlendFactor(mInner);
	    }
        /// <summary>
        /// 设置混合因子
        /// </summary>
        /// <param name="fBlendFactor">混合因子</param>
        public void SetBlendFactor(float fBlendFactor)
	    {
            if (mInner == IntPtr.Zero)
			    return;
            DllImportAPI.V3DAnimTreeNode_SetBlendFactor(mInner, fBlendFactor);
	    }
        /// <summary>
        /// 是否暂停动作
        /// </summary>
        /// <returns>暂停返回true，否则返回false</returns>
        public bool GetPause()
	    {
            if (mInner == IntPtr.Zero)
                return true;

            return DllImportAPI.V3DAnimTreeNode_GetPause(mInner) == 0 ? false : true;
        }
        /// <summary>
        /// 设置是否暂停动作
        /// </summary>
        /// <param name="bPause">需要暂停设置为true，否则为false</param>
	    public void SetPause(bool bPause)
	    {
            if (mInner == IntPtr.Zero)
                return;

            DllImportAPI.V3DAnimTreeNode_SetPause(mInner, bPause ? 1 : 0);
        }
        /// <summary>
        /// 根动作变换的位置偏移值
        /// </summary>
        /// <returns>返回根动作变换的位置偏移值</returns>
        public SlimDX.Vector3 GetDeltaRootmotionPos()
        {
            SlimDX.Vector3 delta = new SlimDX.Vector3();
            unsafe
            {
                DllImportAPI.V3DAnimTreeNode_GetDeltaRootmotionPos(mInner, &delta);
            }
            return delta;
        }
        /// <summary>
        /// 根动作变换前后的四元数差值
        /// </summary>
        /// <returns>得到根动作变换前后四元数的差值</returns>
        public SlimDX.Quaternion GetDeltaRootmotionQuat()
        {
            SlimDX.Quaternion delta = new SlimDX.Quaternion();
            unsafe
            {
                DllImportAPI.V3DAnimTreeNode_GetDeltaRootmotionQuat(mInner, &delta);
            }
            return delta;
        }
        /// <summary>
        /// 动作树列表
        /// </summary>
        /// <returns>得到该动作的动作树列表</returns>
        public virtual List<CSUtility.Animation.AnimationTree> GetAnimations()
		{
			return mChildren;
		}
        /// <summary>
        /// 设置动作树
        /// </summary>
        /// <param name="anims">动作列表</param>
        public void SetAnimations(List<CSUtility.Animation.AnimationTree> anims)
        {
            ClearNode();
            foreach (var i in anims)
            {
                var node = i as AnimTreeNode;
                if(node != null)
                    AddNode(node);
            }
        }
        /// <summary>
        /// 是否完成动作
        /// </summary>
        /// <returns>动作完成返回true，否则返回false</returns>
        public virtual bool IsActionFinished()
        {
            return this.GetATFinished();
        }
        /// <summary>
        /// 设置是否循环播放动作
        /// </summary>
        /// <param name="bLoop">是否循环</param>
        public virtual void SetLoop(bool bLoop)
        {
            foreach(var anim in mChildren)
            {
                anim.SetLoop(bLoop);
            }
        }
        /// <summary>
        /// 动作是否循环播放
        /// </summary>
        /// <returns>动作循环返回true，否则返回false</returns>
        public virtual bool GetLoop()
        {
            foreach (var anim in mChildren)
            {
                if (anim.GetLoop() == true)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 设置播放速度
        /// </summary>
        /// <param name="playRate">播放速度</param>
        public virtual void SetPlayRate(float playRate)
        {
            foreach (var anim in mChildren)
            {
                anim.SetPlayRate(playRate);
            }
        }
        /// <summary>
        /// 动作的播放速度
        /// </summary>
        /// <returns>返回动作的播放速度</returns>
        public virtual float GetPlayRate()
        {
            foreach (var anim in mChildren)
            {
                var anim_action = anim as AnimTreeNode_Action;
                if (anim_action!=null)
                    return anim.GetPlayRate();
            }
            return 1.0f;
        }

    }
}
