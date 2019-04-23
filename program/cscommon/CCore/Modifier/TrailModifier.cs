using System;

namespace CCore.Modifier
{
    /// <summary>
    /// 拖尾模拟器
    /// </summary>
    public class TrailModifier : MeshModifier
    {
        /// <summary>
        /// 拖尾池容量
        /// </summary>
        public int TrailPoolSize
        {
            get
            {
                unsafe
                {
                    if (mModifier != IntPtr.Zero)
                        return DllImportAPI.V3DTrailModifier_GetTrailPoolSize(mModifier);
                    return 100;
                }
            }
            set
            {
                unsafe
                {
                    if (mModifier != IntPtr.Zero)
                        DllImportAPI.V3DTrailModifier_SetTrailPoolSize(mModifier, value);
                }
            }
        }
        /// <summary>
        /// 构造函数，创建拖尾模拟器对象
        /// </summary>
        public TrailModifier()
            : base(DllImportAPI.V3DTrailModifier_New())
        {
            unsafe
            {
                DllImportAPI.V3DTrailModifier_InitObjects(mModifier, CCore.Engine.Instance.Client.Graphics.Device);
                DllImportAPI.V3DTrailModifier_SetTrailPoolSize(mModifier, 100);
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~TrailModifier()
        {

        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
        }
        /// <summary>
        /// 通过XND文件加载数据
        /// </summary>
        /// <param name="node">XND节点</param>
        public void Load(CSUtility.Support.XndNode node)
        {

        }
        /// <summary>
        /// 保存数据到XND节点
        /// </summary>
        /// <param name="node">XND数据节点</param>
        public void Save(CSUtility.Support.XndNode node)
        {

        }
        /// <summary>
        /// 复制拖尾模拟器
        /// </summary>
        /// <param name="srcModifier">拖尾模拟器对象</param>
        public void CopyFrom(TrailModifier srcModifier)
        {

        }
    }
}
