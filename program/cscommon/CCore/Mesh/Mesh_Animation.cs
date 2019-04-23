using System;

namespace CCore.Mesh
{
    /// <summary>
    /// mesh类
    /// </summary>
    public partial class Mesh
    {
        // 动画树
        /// <summary>
        /// 动画树
        /// </summary>
        protected CCore.AnimTree.AnimTreeNode m_pAnimTree = null;
        //IAnimTreeNode		^m_pOldAnimTree;
        /// <summary>
        /// 动画树播放完成
        /// </summary>
        protected bool mATFinished = false;
        /// <summary>
        /// 动画播放超时
        /// </summary>
        protected int mOverRangeAnimTickTime;


        Int64 mPrevAnimTime = 0;
        //void			SetAnimTree( IXndNode ^pXNDNode );
        /// <summary>
        /// 设置mesh的动画树
        /// </summary>
        /// <param name="pAnimTree">动画树节点</param>
        public virtual void SetAnimTree(CCore.AnimTree.AnimTreeNode pAnimTree)
        {
            var pOldAT = m_pAnimTree;
            m_pAnimTree = pAnimTree;

            if (mIsSkined == false || mFullSkeleton == null)
                return;

            {
                if (pAnimTree != null)
                {
                    m_pAnimTree.SetSkeleton(mFullSkeleton);
                    m_pAnimTree.Update(1);

                    if (pOldAT != null && pOldAT.GetAnimations().Count > 0 && m_pAnimTree.BlendDuration != 0)
                    {
                        //pOldAT.SetPause(true);
                        // 如果之前的ANIMTREE还未过渡完，则不再和新的ANIMTREE过渡，避免计算错误、降低复杂度。
                        //if( pOldAT.GetAnimations().Count==1 )
                        //    m_pAnimTree.AddNode(pOldAT);
                        var sourceAction = pOldAT.GetAnimations()[0] as CCore.AnimTree.AnimTreeNode;
                        sourceAction.SetPause(true);
                        m_pAnimTree.AddNode(sourceAction);

                        //if (m_pAnimTree.mDoFirstTick == false)
                        //{
                        //    m_pAnimTree.Update(1);
                        //    m_pAnimTree.mDoFirstTick = true;
                        //}
                    }
                    pOldAT = null;
                    mATFinished = false;
                }

                UpdateBoundingBox();
            }

            mPrevAnimTime = 0;
            //EnableTrail = false;
            InitializeNotifys(pAnimTree);
        }
        /// <summary>
        /// 获取mesh的动画树
        /// </summary>
        /// <returns>返回mesh的动画</returns>
        public CCore.AnimTree.AnimTreeNode GetAnimTree()
        {
            return m_pAnimTree;
        }

    }
}
