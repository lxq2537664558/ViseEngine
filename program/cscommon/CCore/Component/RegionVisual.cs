using System;

namespace CCore.Component
{
    /// <summary>
    /// 区域部分的初始化
    /// </summary>
    public class RegionInit : CCore.World.ActorInit { }
    /// <summary>
    /// 可视化区域
    /// </summary>
    public class RegionVisual : Visual
    {
        /// <summary>
        /// 显示区域的颜色
        /// </summary>
        protected CSUtility.Support.Color mColor;
        /// <summary>
        /// 显示区域的颜色
        /// </summary>
        public CSUtility.Support.Color Color
        {
            get { return mColor; }
            set
            {
                mColor = value;
            }
        }
        /// <summary>
        /// 区域半径
        /// </summary>
        protected float mRadius;
        /// <summary>
        /// 区域半径
        /// </summary>
        public float Radius
        {
            get { return mRadius; }
            set { mRadius = value; }
        }
        /// <summary>
        /// 显示区域的宽
        /// </summary>
        protected float mWidth;
        /// <summary>
        /// 显示区域的高
        /// </summary>
        protected float mHeight;
        /// <summary>
        /// 区域的贴花
        /// </summary>
        protected Decal mRegionDecal;
        /// <summary>
        /// 区域的材质
        /// </summary>
        protected CCore.Material.Material mRegionMaterial;
        /// <summary>
        /// 区域的弧度
        /// </summary>
        protected float mRegionAngle;
        /// <summary>
        /// 显示区域的弧度
        /// </summary>
        public float RegionAngle
        {
            get { return mRegionAngle; }
            set { mRegionAngle = value; }
        }
        /// <summary>
        /// 可视化区域的构造函数
        /// </summary>
        public RegionVisual()
        {
            mLayer = RLayer.RL_DSTranslucent;

            Cleanup();

            mRegionMaterial = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultRegionTechnique);
            mRegionDecal = new Decal();
            mRegionDecal.ShowRangeMesh = false;
            mRegionDecal.ShowSignMesh = false;
            mRegionDecal.SetMaterial(mRegionMaterial);
            mRegionDecal.Visible = true;
            mRegionAngle = 0;

            mRadius = 0;
            Color = CSUtility.Support.Color.Red;

            mWidth = 0;
            mHeight = 0;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~RegionVisual()
        {
            Cleanup();
        }
        /// <summary>
        /// 清除该区域
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
        }
        /// <summary>
        /// 将该区域提交到世界中
        /// </summary>
        /// <param name="renderEnv">提交到的环境</param>
        /// <param name="matrix">区域的矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if (Visible == false)
                return;

            var mat = matrix;

            if (mRegionDecal != null)
            {
                SlimDX.Matrix decalMatrix = matrix;
                decalMatrix.M11 = Radius;
                decalMatrix.M22 = 500;
                decalMatrix.M33 = Radius;
                decalMatrix = SlimDX.Matrix.Multiply(SlimDX.Matrix.RotationY(mRegionAngle), decalMatrix);
                mRegionDecal.Commit(renderEnv, ref decalMatrix, eye);
            }
        }
        /// <summary>
        /// 设置区域的显示图片
        /// </summary>
        /// <param name="fileName">图片名字</param>
        public void SetRegionImage(string fileName)
        {
            if (mRegionMaterial == null)
                return;

            if (mRegionDecal.Visible == false)
                mRegionDecal.Visible = true;

            var texture = new CCore.Graphics.Texture();
            texture.LoadTexture(fileName);
            mRegionMaterial.SetTexture("GDiffuse", texture);
        }
        /// <summary>
        /// 设置区域材质
        /// </summary>
        /// <param name="matId">材质ID</param>
        public void SetRegionMaterial(Guid matId)
        {
            if (mRegionMaterial == null)
                return;

            if (mRegionDecal.Visible == false)
                mRegionDecal.Visible = true;

            mRegionMaterial = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(matId);
            mRegionDecal.SetMaterial(mRegionMaterial);
        }
    }

}
