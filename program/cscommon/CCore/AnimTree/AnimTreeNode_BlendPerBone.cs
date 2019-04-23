namespace CCore.AnimTree
{
    /// <summary>
    /// 每块骨骼的混合
    /// </summary>
    public class AnimTreeNode_BlendPerBone : AnimTreeNode
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AnimTreeNode_BlendPerBone()
        {

        }
        /// <summary>
        /// 对象初始化，创建实例对象
        /// </summary>
        /// <returns>初始化成功返回true</returns>
        public override bool Initialize()
        {
		    Cleanup();

            mInner = DllImportAPI.V3DAnimTreeNode_BlendPerBone_New();
		    return true;
        }
        /// <summary>
        /// 根据XND数据进行初始化
        /// </summary>
        /// <param name="node">XND数据节点</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Support.XndNode node)
        {
		    Cleanup();

            mInner = DllImportAPI.V3DAnimTreeNode_BlendPerBone_New();

		    if( node == null )
			    return false;
            DllImportAPI.V3DAnimTreeNode_LoadFromXnd(mInner, Engine.Instance.Client.Graphics.Device, node.GetRawNode());

		    return true;
        }
        /// <summary>
        /// 添加分支启动骨骼
        /// </summary>
        /// <param name="name">骨骼名称</param>
        public void AddBranchStartBone(System.String name)
	    {
            DllImportAPI.V3DAnimTreeNode_BlendPerBone_AddBranchStartBone(mInner, name);
	    }
        /// <summary>
        /// 清空分支启动骨骼
        /// </summary>
	    public void ClearBranchStartBone()
	    {
            DllImportAPI.V3DAnimTreeNode_BlendPerBone_ClearBranchStartBone(mInner);
        }
    }
}
