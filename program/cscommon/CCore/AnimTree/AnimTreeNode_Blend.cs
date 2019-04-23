namespace CCore.AnimTree
{
    /// <summary>
    /// 动作树混合类
    /// </summary>
    public class AnimTreeNode_Blend : AnimTreeNode
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AnimTreeNode_Blend()
        {

        }
        /// <summary>
        /// 对象的初始化，用于创建实例
        /// </summary>
        /// <returns>初始化成功返回true</returns>
        public override bool Initialize()
        {
		    Cleanup();

            mInner = DllImportAPI.V3DAnimTreeNode_BlendWithPrev_New();
		    return true;
        }
        /// <summary>
        /// 通过已有数据节点进行对象的初始化
        /// </summary>
        /// <param name="node">XND数据节点</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Support.XndNode node)
        {
		    Cleanup();

            mInner = DllImportAPI.V3DAnimTreeNode_BlendWithPrev_New();

		    if( node == null )
			    return false;
            DllImportAPI.V3DAnimTreeNode_LoadFromXnd(mInner, Engine.Instance.Client.Graphics.Device, node.GetRawNode());

		    return true;
        }
        /// <summary>
        /// 设置混合的两个动作持续时间的比例
        /// </summary>
        /// <param name="fTargetDurationPercient">目标动作与该动作持续时间的比例</param>
        void SetBlendDurationPercient(float fTargetDurationPercient)
        {
            unsafe
            {
                DllImportAPI.V3DAnimTreeNode_BlendWithPrev_SetBlendDurationPercient(mInner, fTargetDurationPercient);
            }
        }
        /// <summary>
        /// 得到混合的两个动作持续时间的比例
        /// </summary>
        /// <returns>返回目标动作与该动作持续时间的比例</returns>
        float GetBlendDurationPercient()
        {
            unsafe
            {
                return DllImportAPI.V3DAnimTreeNode_BlendWithPrev_GetBlendDurationPercient(mInner);
            }
        }
    }
}
