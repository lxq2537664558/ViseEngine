using System;
using System.ComponentModel;

namespace CCore.Component
{
    /// <summary>
    /// 可视化的初始化类
    /// </summary>
    public class VisualInit : CSUtility.Support.XndSaveLoadProxy
    {

    }
    /// <summary>
    /// 可视化类
    /// </summary>
    public class Visual : CSUtility.Component.IComponent
    {
        /// <summary>
        /// 可见性，默认为true
        /// </summary>
        protected bool mVisible = true;
        /// <summary>
        /// 设置其是否可见，默认为true
        /// </summary>
        [Browsable(false)]
        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }
        protected CCore.RLayer mLayer = CCore.RLayer.RL_DSBase;
        /// <summary>
        /// 该对象所在的层
        /// </summary>
        [Browsable(false)]
        public CCore.RLayer Layer
        {
            get { return mLayer; }
            set { mLayer = value; }
        }

        /// <summary>
        /// 该对象所在的坐标系，默认为右手坐标系
        /// </summary>
        protected CCore.RGroup mGroup = CCore.RGroup.RL_World;
        /// <summary>
        /// 所处坐标系，默认为右手坐标系
        /// </summary>
        [Browsable(false)]
        public CCore.RGroup Group
        {
            get { return mGroup; }
        }
        /// <summary>
        /// 该对象的初始化类
        /// </summary>
        protected VisualInit mVisualInit;
        /// <summary>
        /// 只读属性，visual的初始化类
        /// </summary>
        [Browsable(false)]
        public VisualInit VisualInit
        {
            get { return mVisualInit; }
        }
        /// <summary>
        /// 该对象的所属Actor
        /// </summary>
        protected CCore.World.Actor mHostActor;
        /// <summary>
        /// visual的所属Actor
        /// </summary>
        [Browsable(false)]
        public virtual CCore.World.Actor HostActor
        {
            get { return mHostActor; }
            set { mHostActor = value; }
        }
        /// <summary>
        /// 自定义时间，默认为0
        /// </summary>
        protected int mCustomTime = 0;
        /// <summary>
        /// 自定义时间的设置
        /// </summary>
        [Browsable(false)]
        public int CustomTime
        {
            get { return mCustomTime; }
            set { mCustomTime = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Visual()
        {

        }
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
        ~Visual()
        {
            Cleanup();
        }
        /// <summary>
        /// 将Visual进行初始化
        /// </summary>
        /// <param name="_init">visual的初始化类</param>
        /// <param name="host">visual的所属Actor</param>
        /// <returns>初始化成功返回true</returns>
        public virtual bool Initialize(VisualInit _init, CCore.World.Actor host)
        {
            mVisualInit = _init;
            HostActor = host;
            return true;
        }
        /// <summary>
        /// 释放实例内存
        /// </summary>
        public virtual void Cleanup()
        {

        }
        /// <summary>
        /// 提前使用
        /// </summary>
        /// <param name="bForce">是否受外力</param>
        /// <param name="time">时间</param>
        public virtual void PreUse(bool bForce, UInt64 time)
        {

        }
        /// <summary>
        /// 设置所有的visual都进行鼠标点选
        /// </summary>
        /// <param name="hitProxy">点选代理</param>
		public virtual void SetHitProxyAll(System.UInt32 hitProxy) {}
        /// <summary>
        /// 设置符合条件的visual可以进行鼠标点选
        /// </summary>
        /// <param name="MeshIndex">mesh索引</param>
        /// <param name="MaterialIndex">材质索引</param>
        /// <param name="hitProxy">点选代理</param>
		public virtual void SetHitProxy( int MeshIndex, int MaterialIndex, UInt32 hitProxy) {}
        /// <summary>
        /// 线条检查
        /// </summary>
        /// <param name="start">线条起始坐标值</param>
        /// <param name="end">线条的终点坐标值</param>
        /// <param name="matrix">线条的矩阵</param>
        /// <param name="result">点选结果</param>
        /// <returns>此处只返回false</returns>
		public virtual bool LineCheck( ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref SlimDX.Matrix matrix, ref CSUtility.Support.stHitResult result )
        {
            return false;
        }
        /// <summary>
        /// 将实例提交到场景
        /// </summary>
        /// <param name="renderEnv">提交到的场景</param>
        /// <param name="matrix">实例矩阵</param>
        /// <param name="eye">视野</param>
		public virtual void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye){}
        /// <summary>
        /// 提交阴影
        /// </summary>
        /// <param name="light">形成阴影的光源</param>
        /// <param name="matrix">实例矩阵</param>
        /// <param name="isDynamic">是否为动态的(动态静态阴影都实时更新，只是动态阴影更新频率较高)</param>
        public virtual void CommitShadow(CCore.Light.Light light, ref SlimDX.Matrix matrix, bool isDynamic) { }
        /// <summary>
        /// 实例AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点</param>
        /// <param name="vMax">最大顶点</param>
        public virtual void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
		    vMin = -SlimDX.Vector3.UnitXYZ;
		    vMax = SlimDX.Vector3.UnitXYZ;
        }
        /// <summary>
        /// 实例的OBB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点</param>
        /// <param name="vMax">最大顶点</param>
        /// <param name="fixMatrix">转换矩阵</param>
        public virtual void GetOBB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax, ref SlimDX.Matrix fixMatrix)
        {
            vMin = -SlimDX.Vector3.UnitXYZ;
            vMax = SlimDX.Vector3.UnitXYZ;
            fixMatrix = SlimDX.Matrix.Identity;
        }
        /// <summary>
        /// 得到Layer名字
        /// </summary>
        /// <returns>返回实例的Layer名字</returns>
		public virtual string GetLayerName() { return "Other"; }
    }
}
