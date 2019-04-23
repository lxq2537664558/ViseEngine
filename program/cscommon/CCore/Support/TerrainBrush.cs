using System;
using System.Collections.Generic;

namespace CCore.Support
{
    /// <summary>
    /// 地形刷的初始化类
    /// </summary>
    public class ITerrainBrushInit : CCore.World.ActorInit { }
    /// <summary>
    /// 地形刷的可视化类
    /// </summary>
    public class ITerrainBrushVisual : CCore.Component.Visual
    {
        /// <summary>
        /// 每行的对象的数量，默认为360
        /// </summary>
        protected int mLineObjectsCount = 360;
        /// <summary>
        /// 只读属性，每行绘制对象的数量
        /// </summary>
        public int LineObjectsCount
        {
            get { return mLineObjectsCount; }
        }
        /// <summary>
        /// 线条的内轮廓对象地址列表
        /// </summary>
        protected IntPtr[] mInnerLineObjects;
        /// <summary>
        /// 只读属性，线条的内轮廓对象地址列表
        /// </summary>
        public IntPtr[] InnerLineObjects
        {
            get { return mInnerLineObjects; }
        }
        /// <summary>
        /// 线条的外轮廓对象地址列表
        /// </summary>
        protected IntPtr[] mOuterLineObjects;
        /// <summary>
        /// 只读属性，线条的外轮廓对象地址列表
        /// </summary>
        public IntPtr[] OuterLineObjects
        {
            get { return mOuterLineObjects; }
        }
        /// <summary>
        /// 颜色
        /// </summary>
        protected CSUtility.Support.Color mColor;
        /// <summary>
        /// 颜色
        /// </summary>
        public CSUtility.Support.Color Color
        {
            get { return mColor; }
            set
            {
                mColor = value;

                unsafe
                {
                    var colorValue = value.ToArgb();
                    for (int i = 0; i < mLineObjectsCount; ++i)
                    {
                        DllImportAPI.v3dLineObject_SetColor(mInnerLineObjects[i], (UInt32)colorValue);
                        DllImportAPI.v3dLineObject_SetColor(mOuterLineObjects[i], (UInt32)colorValue);
                    }
                }
            }
        }
        /// <summary>
        /// 内接圆半径
        /// </summary>
        protected float mInnerRadius;
        /// <summary>
        /// 只读属性，内接圆半径
        /// </summary>
        public float InnerRadius
        {
            get { return mInnerRadius; }
        }
        /// <summary>
        /// 外切圆半径
        /// </summary>
        protected float mOuterRadius;
        /// <summary>
        /// 只读属性，外切圆半径
        /// </summary>
        public float OuterRadius
        {
            get { return mOuterRadius; }
        }
        /// <summary>
        /// 矩阵型线条的对象指针列表
        /// </summary>
        protected IntPtr[] mRectLineObjects;
        /// <summary>
        /// 线条的宽
        /// </summary>
        protected float mWidth;
        /// <summary>
        /// 线条的高
        /// </summary>
        protected float mHeight;
        /// <summary>
        /// 贴花型画刷
        /// </summary>
        protected CCore.Component.Decal mBrushDecal;
        /// <summary>
        /// 画刷材质
        /// </summary>
        protected CCore.Material.Material mBrushMaterial;
        /// <summary>
        /// 画刷角度
        /// </summary>
        protected float mBrushAngle;
        /// <summary>
        /// 画刷角度
        /// </summary>
        public float BrushAngle
        {
            get { return mBrushAngle; }
            set { mBrushAngle = value; }
        }
        /// <summary>
        /// 画刷类型枚举
        /// </summary>
        public enum enBrushType
        {
            Circle,
            Rect,
            RectCg,
        }
        /// <summary>
        /// 画刷类型
        /// </summary>
        public enBrushType mBrushType;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ITerrainBrushVisual()
        {
            mLayer = RLayer.RL_SystemHelper;

            Cleanup();

            mBrushMaterial = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.TerrainBrushTechnique);
            mBrushDecal = new CCore.Component.Decal();
            mBrushDecal.ShowRangeMesh = false;
            mBrushDecal.ShowSignMesh = false;
            mBrushDecal.SetMaterial(mBrushMaterial);
            mBrushDecal.Visible = false;
            mBrushAngle = 0;

            mInnerLineObjects = new IntPtr[mLineObjectsCount];
            mOuterLineObjects = new IntPtr[mLineObjectsCount];

            for (int i = 0; i < mLineObjectsCount; ++i)
            {
                mInnerLineObjects[i] = DllImportAPI.v3dLineObject_New();
                mOuterLineObjects[i] = DllImportAPI.v3dLineObject_New();
            }

            mInnerRadius = 0;
            mOuterRadius = 0;
            Color = CSUtility.Support.Color.Red;

            mRectLineObjects = new IntPtr[4];
            for (int i = 0; i < 4; ++i)
            {
                mRectLineObjects[i] = DllImportAPI.v3dLineObject_New();
            }
            mWidth = 0;
            mHeight = 0;

            mBrushType = enBrushType.Circle;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~ITerrainBrushVisual()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象，释放指针内存
        /// </summary>
        public override void Cleanup()
        {
            if (mInnerLineObjects != null)
            {
                for (int i = 0; i < mLineObjectsCount; ++i)
                {
                    if(mInnerLineObjects[i] != IntPtr.Zero)
                        DllImportAPI.v3dLineObject_Release(mInnerLineObjects[i]);
                }
                mInnerLineObjects = null;
            }
            if (mOuterLineObjects != null)
            {
                for (int i = 0; i < mLineObjectsCount; ++i)
                {
                    if(mOuterLineObjects[i] != IntPtr.Zero)
                        DllImportAPI.v3dLineObject_Release(mOuterLineObjects[i]);
                }
                mOuterLineObjects = null;
            }
            if (mRectLineObjects != null)
            {
                for (int i = 0; i < 4; ++i)
                {
                    if(mRectLineObjects[i] != IntPtr.Zero)
                        DllImportAPI.v3dLineObject_Release(mRectLineObjects[i]);
                }
                mRectLineObjects = null;
            }
            base.Cleanup();
        }
        /// <summary>
        /// 提交对象
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">对象的矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if (Visible == false)
                return;

            unsafe
            {
                var mat = matrix;
                switch (mBrushType)
                {
                    case enBrushType.Circle:
                        {
                            for (int i = 0; i < mLineObjectsCount; ++i)
                            {
                                DllImportAPI.vDSRenderEnv_CommitHelperLine(renderEnv.DSRenderEnv, (int)mGroup, InnerLineObjects[i], &mat);
                            }

                            for (int i = 0; i < mLineObjectsCount; ++i)
                            {
                                DllImportAPI.vDSRenderEnv_CommitHelperLine(renderEnv.DSRenderEnv, (int)mGroup, OuterLineObjects[i], &mat);
                            }

                            if (mBrushDecal != null)
                            {
                                SlimDX.Matrix decalMatrix = matrix;
                                decalMatrix.M11 = OuterRadius;
                                decalMatrix.M22 = 500;
                                decalMatrix.M33 = OuterRadius;
                                decalMatrix = SlimDX.Matrix.Multiply(SlimDX.Matrix.RotationY(mBrushAngle), decalMatrix);
                                mBrushDecal.Commit(renderEnv, ref decalMatrix, eye);
                            }
                        }
                        break;
                    case enBrushType.Rect:
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                DllImportAPI.vDSRenderEnv_CommitHelperLine(renderEnv.DSRenderEnv, (int)mGroup, mRectLineObjects[i], &mat);
                            }

                            if (mBrushDecal != null)
                            {
                                SlimDX.Matrix decalMatrix = matrix;
                                decalMatrix.M11 = mWidth / 2;
                                decalMatrix.M22 = 500;
                                decalMatrix.M33 = mHeight / 2;
                                decalMatrix.M41 += mWidth / 2;
                                decalMatrix.M43 += mHeight / 2;
                                mBrushDecal.Commit(renderEnv, ref decalMatrix, eye);
                            }
                        }
                        break;
                    case enBrushType.RectCg:
                        {
                            mat = SlimDX.Matrix.Identity;
                            for (int i = 0; i < 4; ++i)
                            {
                                DllImportAPI.vDSRenderEnv_CommitHelperLine(renderEnv.DSRenderEnv, (int)mGroup, mRectLineObjects[i], &mat);
                            }
                        }
                        break;
                }
            }

        }
        /// <summary>
        /// 设置半径
        /// </summary>
        /// <param name="inner">内切半径</param>
        /// <param name="outer">外接半径</param>
        public void SetRadius(float inner, float outer)
        {
            unsafe
            {

                mInnerRadius = inner;
		        mOuterRadius = outer;
		        double angleDelta = System.Math.PI * 2 / mLineObjectsCount;
		        for(int i=0; i<mLineObjectsCount; ++i)
		        {
                    var vInnerStart = new SlimDX.Vector3();
                    var vOuterStart = new SlimDX.Vector3();
			        double angle = System.Math.Cos(i*angleDelta);
                    vInnerStart.X = (float)(angle * inner);
                    vOuterStart.X = (float)(angle * outer);
                    //mInnerLineObjects[i].m_Start.x = angle * inner;
                    //mOuterLineObjects[i].m_Start.x = angle * outer;
			        angle = System.Math.Sin(i*angleDelta);
                    vInnerStart.Z = (float)(angle * inner);
                    vOuterStart.Z = (float)(angle * outer);
                    //mInnerLineObjects[i].m_Start.z = angle * inner;
                    //mOuterLineObjects[i].m_Start.z = angle * outer;
                    vInnerStart.Y = 0;
                    vOuterStart.Y = 0;
                    //mInnerLineObjects[i].m_Start.y = 0;
                    //mOuterLineObjects[i].m_Start.y = 0;
                    DllImportAPI.v3dLineObject_SetStart(mInnerLineObjects[i], &vInnerStart);
                    DllImportAPI.v3dLineObject_SetStart(mOuterLineObjects[i], &vOuterStart);

                    var vInnerEnd = new SlimDX.Vector3();
                    var vOuterEnd = new SlimDX.Vector3();
			        angle = System.Math.Cos(i*angleDelta + angleDelta);
                    vInnerEnd.X = (float)(angle * inner);
                    vOuterEnd.X = (float)(angle * outer);
                    //mInnerLineObjects[i].m_End.x = angle * inner;
                    //mOuterLineObjects[i].m_End.x = angle * outer;
			        angle = System.Math.Sin(i*angleDelta + angleDelta);
                    vInnerEnd.Z = (float)(angle * inner);
                    vOuterEnd.Z = (float)(angle * outer);
                    //mInnerLineObjects[i].m_End.z = angle * inner;
                    //mOuterLineObjects[i].m_End.z = angle * outer;
                    vInnerEnd.Y = 0;
                    vOuterEnd.Y = 0;
                    //mInnerLineObjects[i].m_End.y = 0;
                    //mOuterLineObjects[i].m_End.y = 0;
                    DllImportAPI.v3dLineObject_SetEnd(mInnerLineObjects[i], &vInnerEnd);
                    DllImportAPI.v3dLineObject_SetEnd(mOuterLineObjects[i], &vOuterEnd);
		        }

            }
        }
        /// <summary>
        /// 设置矩形尺寸
        /// </summary>
        /// <param name="leftTop">左上角坐标</param>
        /// <param name="rightTop">右上角坐标</param>
        /// <param name="leftDown">左下角坐标</param>
        /// <param name="rightDown">右下角坐标</param>
        public void SetRectSize(SlimDX.Vector3 leftTop, SlimDX.Vector3 rightTop, SlimDX.Vector3 leftDown, SlimDX.Vector3 rightDown)
        {
            unsafe
            {
                DllImportAPI.v3dLineObject_SetStart(mRectLineObjects[0], &leftTop);
                DllImportAPI.v3dLineObject_SetEnd(mRectLineObjects[0], &rightTop);

                DllImportAPI.v3dLineObject_SetStart(mRectLineObjects[1], &rightTop);
                DllImportAPI.v3dLineObject_SetEnd(mRectLineObjects[1], &rightDown);

                DllImportAPI.v3dLineObject_SetStart(mRectLineObjects[2], &rightDown);
                DllImportAPI.v3dLineObject_SetEnd(mRectLineObjects[2], &leftDown);

                DllImportAPI.v3dLineObject_SetStart(mRectLineObjects[3], &leftDown);
                DllImportAPI.v3dLineObject_SetEnd(mRectLineObjects[3], &leftTop);
            }
        }
        /// <summary>
        /// 设置矩形尺寸
        /// </summary>
        /// <param name="width">矩形的宽</param>
        /// <param name="height">矩形的高</param>
        public void SetRectSize(float width, float height)
        {
            unsafe
            {
                mWidth = width;
                mHeight = height;
                // --0--
                // 3   1
                // --2--
                var vStart = new SlimDX.Vector3(0, 0, height);
                var vEnd = new SlimDX.Vector3(width, 0, height);
                DllImportAPI.v3dLineObject_SetStart(mRectLineObjects[0], &vStart);
                DllImportAPI.v3dLineObject_SetEnd(mRectLineObjects[0], &vEnd);

                vStart = new SlimDX.Vector3(width, 0, height);
                vEnd = new SlimDX.Vector3(width, 0, 0);
                DllImportAPI.v3dLineObject_SetStart(mRectLineObjects[1], &vStart);
                DllImportAPI.v3dLineObject_SetEnd(mRectLineObjects[1], &vEnd);

                vStart = new SlimDX.Vector3(width, 0, 0);
                vEnd = new SlimDX.Vector3(0, 0, 0);
                DllImportAPI.v3dLineObject_SetStart(mRectLineObjects[2], &vStart);
                DllImportAPI.v3dLineObject_SetEnd(mRectLineObjects[2], &vEnd);

                vStart = new SlimDX.Vector3(0, 0, 0);
                vEnd = new SlimDX.Vector3(0, 0, height);
                DllImportAPI.v3dLineObject_SetStart(mRectLineObjects[3], &vStart);
                DllImportAPI.v3dLineObject_SetEnd(mRectLineObjects[3], &vEnd);
            }
        }
        /// <summary>
        /// 设置画刷图片
        /// </summary>
        /// <param name="fileName">图片的文件名称</param>
		public void SetBrushImage(string fileName)
        {
		    if(mBrushMaterial == null)
			    return;

		    if(mBrushDecal.Visible == false)
			    mBrushDecal.Visible = true;

		    var texture = new CCore.Graphics.Texture();
		    texture.LoadTexture(fileName);
            texture.ColorSpace = TextureColorSpace.TCS_LINEAR;
		    mBrushMaterial.SetTexture("GDiffuse", texture);
        }
    }
    /// <summary>
    /// 地形刷的Actor类
    /// </summary>
    public class ITerrainBrushActor : CCore.World.Actor
    {
        /// <summary>
        /// 显示线条列表
        /// </summary>
        protected List<CCore.Component.Line> mShowLineList;
        /// <summary>
        /// 只读属性，外接圆半径
        /// </summary>
        public float OuterRadius
        {
            get
            {
                if (Visual != null)
                {
                    var vis = Visual as ITerrainBrushVisual;
                    return vis.OuterRadius;
                }

                return 0;
            }
        }
        /// <summary>
        /// 只读属性，内切圆半径
        /// </summary>
        public float InnerRadius
        {
            get
            {
                if (Visual != null)
                {
                    var vis = Visual as ITerrainBrushVisual;
                    return vis.InnerRadius;
                }

                return 0;
            }
        }
        /// <summary>
        /// 画刷类型
        /// </summary>
        public ITerrainBrushVisual.enBrushType BrushType
        {
            get
            {
                if (Visual != null)
                {
                    var vis = Visual as ITerrainBrushVisual;
                    return vis.mBrushType;
                }
                return ITerrainBrushVisual.enBrushType.Circle;
            }
            set
            {
                if (Visual != null)
                {
                    var vis = Visual as ITerrainBrushVisual;
                    vis.mBrushType = value;
                }
            }
        }
        /// <summary>
        /// 画刷角度
        /// </summary>
        public float BrushAngle
        {
            get
            {
                if (Visual != null)
                {
                    var vis = Visual as ITerrainBrushVisual;
                    return vis.BrushAngle;
                }

                return 0;
            }
            set
            {
                if (Visual != null)
                {
                    var vis = Visual as ITerrainBrushVisual;
                    vis.BrushAngle = value;
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ITerrainBrushActor()
        {

        }
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="_init">Actor的初始化类对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            base.Initialize(_init);

            Visual = new ITerrainBrushVisual();
            mPlacement = new CSUtility.Component.StandardPlacement(this);

            return true;
        }
        /// <summary>
        /// 设置半径
        /// </summary>
        /// <param name="inner">内切圆半径</param>
        /// <param name="outer">外接圆半径</param>
        public void SetRadius(float inner, float outer)
        {
            if (Visual == null)
                return;

            var vis = Visual as ITerrainBrushVisual;
            vis.SetRadius(inner, outer);
        }
        /// <summary>
        /// 设置矩形大小
        /// </summary>
        /// <param name="width">矩形的宽</param>
        /// <param name="height">矩形的高</param>
        public void SetRectSize(float width, float height)
        {
            if (Visual == null)
                return;

            var vis = Visual as ITerrainBrushVisual;
            vis.SetRectSize(width, height);
        }
        /// <summary>
        /// 设置矩形大小
        /// </summary>
        /// <param name="leftTop">左上角坐标</param>
        /// <param name="rightTop">右上角坐标</param>
        /// <param name="leftDown">左下角坐标</param>
        /// <param name="rightDown">右下角坐标</param>
        public void SetRectSize(SlimDX.Vector3 leftTop, SlimDX.Vector3 rightTop, SlimDX.Vector3 leftDown, SlimDX.Vector3 rightDown)
        {
            if (Visual == null)
                return;

            var vis = Visual as ITerrainBrushVisual;
            vis.SetRectSize(leftTop, rightTop, leftDown, rightDown);
        }
        /// <summary>
        /// 获取内部点列表
        /// </summary>
        /// <returns>返回内部点的列表</returns>
        public List<SlimDX.Vector3> GetInnerPointList()
        {
            unsafe
            {
                if (Visual == null)
                    return new List<SlimDX.Vector3>();

                List<SlimDX.Vector3> ptList = new List<SlimDX.Vector3>();
                var vis = Visual as ITerrainBrushVisual;
                for (int i = 0; i < vis.LineObjectsCount; i++)
                {
                    SlimDX.Vector3 vec;
                    DllImportAPI.v3dLineObject_GetStart(vis.InnerLineObjects[i], &vec);
                    ptList.Add(vec);
                }

                return ptList;
            }
        }
        /// <summary>
        /// 获取外部点列表
        /// </summary>
        /// <returns>返回外部点的列表</returns>
        public List<SlimDX.Vector3> GetOutterPointList()
        {
            unsafe
            {
                if (Visual == null)
                    return new List<SlimDX.Vector3>();

                List<SlimDX.Vector3> ptList = new List<SlimDX.Vector3>();

                var vis = Visual as ITerrainBrushVisual;
                for (int i = 0; i < vis.LineObjectsCount; i++)
                {
                    SlimDX.Vector3 vec;
                    DllImportAPI.v3dLineObject_GetStart(vis.OuterLineObjects[i], &vec);
                    ptList.Add(vec);
                }

                return ptList;
            }
        }
        /// <summary>
        /// 设置内部点列表
        /// </summary>
        /// <param name="pointList">内部点列表</param>
        public void SetInnerPointList(List<SlimDX.Vector3> pointList)
        {
            if (Visual == null)
                return;

            unsafe
            {
                var vis = Visual as ITerrainBrushVisual;

                if (pointList.Count > vis.LineObjectsCount)
                    return;

                for (int i = 0; i < pointList.Count; i++)
                {
                    var pt = pointList[i];
                    DllImportAPI.v3dLineObject_SetStart(vis.InnerLineObjects[i], &pt);

                    if (i == pointList.Count - 1)
                    {
                        pt = pointList[0];
                        DllImportAPI.v3dLineObject_SetEnd(vis.InnerLineObjects[i], &pt);
                    }
                    else
                    {
                        pt = pointList[i + 1];
                        DllImportAPI.v3dLineObject_SetEnd(vis.InnerLineObjects[i], &pt);
                    }
                }
            }
        }
        /// <summary>
        /// 设置外部点列表
        /// </summary>
        /// <param name="pointList">外部点列表</param>
        public void SetOutterPointList(List<SlimDX.Vector3> pointList)
        {
            if (Visual == null)
                return;

            unsafe
            {
                var vis = Visual as ITerrainBrushVisual;

                if (pointList.Count > vis.LineObjectsCount)
                    return;

                for (int i = 0; i < pointList.Count; i++)
                {
                    var pt = pointList[i];
                    DllImportAPI.v3dLineObject_SetStart(vis.OuterLineObjects[i], &pt);

                    if (i == pointList.Count - 1)
                    {
                        pt = pointList[0];
                        DllImportAPI.v3dLineObject_SetEnd(vis.OuterLineObjects[i], &pt);
                    }
                    else
                    {
                        pt = pointList[i + 1];
                        DllImportAPI.v3dLineObject_SetEnd(vis.OuterLineObjects[i], &pt);
                    }
                }
            }
        }
        /// <summary>
        /// 设置画刷的图片文件
        /// </summary>
        /// <param name="imageFile">图片文件名</param>
        public void SetBrushImageFile(string imageFile)
        {
            if (Visual == null)
                return;

            var vis = Visual as ITerrainBrushVisual;
            vis.SetBrushImage(imageFile);
        }
        /// <summary>
        /// 更新画刷
        /// </summary>
        /// <param name="terrain">地形对象</param>
        public void UpdateBrush(CCore.Terrain.Terrain terrain)
        {
            if (terrain == null)
                return;

            var brushLoc = this.Placement.GetLocation();

            switch (BrushType)
            {
                case ITerrainBrushVisual.enBrushType.Circle:
                    {
                        var ptList = GetOutterPointList();

                        for (int i = 0; i < ptList.Count; i++)
                        {
                            var pt = ptList[i];
                            terrain.GetHeightF(pt.X + brushLoc.X, pt.Z + brushLoc.Z, ref pt.Y, false);
                            pt.Y += 0.1f;
                            pt.Y -= brushLoc.Y;
                            ptList[i] = pt;
                        }
                        SetOutterPointList(ptList);

                        ptList = GetInnerPointList();
                        for (int i = 0; i < ptList.Count; i++)
                        {
                            var pt = ptList[i];
                            terrain.GetHeightF(pt.X + brushLoc.X, pt.Z + brushLoc.Z, ref pt.Y, false);
                            pt.Y += 0.1f;
                            pt.Y -= brushLoc.Y;
                            ptList[i] = pt;
                        }
                        SetInnerPointList(ptList);
                    }
                    break;

                case ITerrainBrushVisual.enBrushType.Rect:
                    {

                    }
                    break;
            }
        }
    }
}
