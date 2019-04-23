using System;
using System.Collections.Generic;

namespace CCore.Navigation
{
    /// <summary>
    /// 用于编辑器中显示生成的寻路
    /// </summary>
    public class NavigationAssist : CCore.Component.Visual
    {
        // 与C++层enMeshCommitType保持一致
        /// <summary>
        /// 寻路生成模型绘制类型
        /// </summary>
        enum enMeshCommitType
        {
            /// <summary>
            /// 动态阻挡
            /// </summary>
            MCT_DynamicBlock = 0,
            /// <summary>
            /// 阻挡
            /// </summary>
            MCT_Block        = 1,
            /// <summary>
            /// 可通过
            /// </summary>
            MCT_Path         = 2,
        };

        /// <summary>
        /// 寻路绘制环境数组
        /// </summary>
        IntPtr[] mNavigationRenderEnvArray;

        static NavigationAssist mInstance = new NavigationAssist();
        /// <summary>
        /// 声明该类为单件对象
        /// </summary>
        public static NavigationAssist Instance
        {
            get { return mInstance; }
        }

        UInt32 mTextureWidth = 1024;
        /// <summary>
        /// 只读属性，纹理的宽
        /// </summary>
        public UInt32 TextureWidth
        {
            get { return mTextureWidth; }
        }
        UInt32 mTextureHeight = 1024;
        /// <summary>
        /// 只读属性，纹理的高
        /// </summary>
        public UInt32 TextureHeight
        {
            get { return mTextureHeight; }
        }
        /// <summary>
        /// 只读属性，默认纹理的宽
        /// </summary>
        public UInt32 DefaultTextureWidth
        {
            get { return 1024; }
        }
        /// <summary>
        /// 只读属性，默认纹理的高
        /// </summary>
        public UInt32 DefaultTextureHeight
        {
            get { return 1024; }
        }
        UInt32 mXCount = 0;
        UInt32 mZCount = 0;
        float mMeterPerPixelX = 0.5f;	// Texture横向一像素代表多少米
        /// <summary>
        /// 只读属性，Texture横向一像素代表多少米
        /// </summary>
        public float MeterPerPixelX
        {
            get { return mMeterPerPixelX; }
        }
        float mMeterPerPixelZ = 0.5f;	// Texture纵向一像素代表多少米
        /// <summary>
        /// 只读属性，Texture纵向一像素代表多少米
        /// </summary>
        public float MeterPerPixelZ
        {
            get { return mMeterPerPixelZ; }
        }
        List<CCore.Component.Decal> mNavigationDecals = new List<CCore.Component.Decal>();
        List<CCore.Material.Material> mNavDecalMaterials = new List<CCore.Material.Material>();
        List<UInt32> mNavDrawDirtyLevel = new List<UInt32>();	// 绘制过的需要刷入寻路信息的寻路块
        List<SlimDX.Vector2> mOldPath = new List<SlimDX.Vector2>();

        NavigationGenerateInfo mNavigationGenerateInfo = null;
        /// <summary>
        /// 只读属性，导航的信息
        /// </summary>
        public NavigationGenerateInfo NavigationGenerateInfo
        {
            get { return mNavigationGenerateInfo; }
        }

        List<List<CCore.Graphics.Texture>> mGenerateTextures = new List<List<CCore.Graphics.Texture>>();

        bool mInitialized = false;
        /// <summary>
        /// 是否初始化完成
        /// </summary>
        public bool Initialized
        {
            get { return mInitialized; }
            set
            {
                mInitialized = value;
            }
        }

        bool mShowAutoNavigation = false;
        /// <summary>
        /// 是否显示自动导航
        /// </summary>
        public bool ShowAutoNavigation
        {
            get { return mShowAutoNavigation; }
            set
            {
                mShowAutoNavigation = value;
                UpdateShowDelta();
            }
        }
        bool mShowManualNavigation = false;
        /// <summary>
        /// 是否显示手动导航
        /// </summary>
        public bool ShowManualNavigation
        {
            get { return mShowManualNavigation; }
            set
            {
                mShowManualNavigation = value;
                UpdateShowDelta();
            }
        }
        bool mShowDynamicBlock = false;
        /// <summary>
        /// 是否显示动态块
        /// </summary>
        public bool ShowDynamicBlock
        {
            get { return mShowDynamicBlock; }
            set
            {
                mShowDynamicBlock = value;
                UpdateShowDelta();
            }
        }
        bool mShowFindedPath = false;
        /// <summary>
        /// 是否显示寻路点
        /// </summary>
        public bool ShowFindedPath
        {
            get { return mShowFindedPath; }
            set
            {
                mShowFindedPath = value;
                UpdateShowDelta();
            }
        }
        /// <summary>
        /// 更新显示差值
        /// </summary>
        void UpdateShowDelta()
        {
            SlimDX.Vector4 showDelta0 = new SlimDX.Vector4(1, 1, 1, 1);
            showDelta0.X = mShowManualNavigation ? 1 : 0;
            showDelta0.Y = mShowManualNavigation ? 1 : 0;
            showDelta0.Z = mShowAutoNavigation ? 1 : 0;
            showDelta0.W = mShowDynamicBlock ? 1 : 0;

            SlimDX.Vector4 showDelta1 = new SlimDX.Vector4(1, 1, 1, 1);
            showDelta1.X = mShowFindedPath ? 1 : 0;

            foreach (CCore.Material.Material mat in mNavDecalMaterials)
            {
                mat.SetFloat4("ShowDelta0", ref showDelta0);
                mat.SetFloat4("ShowDelta1", ref showDelta1);
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        NavigationAssist()
        {
            Visible = false;
            CCore.Navigation.Navigation.Instance.OnFindPath = OnFindPath;
        }
        /// <summary>
        /// 析构函数，删除对象
        /// </summary>
        ~NavigationAssist()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除渲染对象，是否指针内存
        /// </summary>
        public override void Cleanup()
        {
            unsafe
            {
                if (mNavigationRenderEnvArray != null)
                {
                    foreach (var renderEnv in mNavigationRenderEnvArray)
                    {
                        if(renderEnv != IntPtr.Zero)
                            DllImportAPI.NavigationRenderEnv_Release(renderEnv);
                    }

                    mNavigationRenderEnvArray = null;
                }
            }

            foreach (var decal in mNavigationDecals)
            {
                decal.Cleanup();
            }
            mNavigationDecals.Clear();

            foreach (var mat in mNavDecalMaterials)
            {
                mat.Cleanup();
            }
            mNavDecalMaterials.Clear();

            foreach (var textureList in mGenerateTextures)
            {
                foreach (var texture in textureList)
                {
                    texture.Cleanup();
                }
            }
            mGenerateTextures.Clear();
        }

        SlimDX.Vector3 mDecalOffsetTest = SlimDX.Vector3.Zero;
        /// <summary>
        /// 将创建的对象提交到渲染环境
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if (mNavigationDecals.Count == 0)
                return;

            // 绘制寻路图
            float cellX = TextureWidth * mMeterPerPixelX;
            float cellZ = TextureHeight * mMeterPerPixelZ;

            for (UInt32 z = 0; z < mZCount; z++)
            {
                for (UInt32 x = 0; x < mXCount; x++)
                {
                    var decal = mNavigationDecals[(int)(z * mXCount + x)];
                    SlimDX.Matrix decalMatrix = matrix;
                    decalMatrix.M11 = cellX * 0.5f;
                    decalMatrix.M22 = 500;
                    decalMatrix.M33 = cellZ * 0.5f;
                    decalMatrix.M41 = x * cellX + cellX * 0.5f + mDecalOffsetTest.X;
                    decalMatrix.M42 = 0 + mDecalOffsetTest.Y;
                    decalMatrix.M43 = z * cellZ + cellZ * 0.5f + mDecalOffsetTest.Z;
                    decal.Commit(renderEnv, ref decalMatrix, eye);
                }
            }
        }
        /// <summary>
        /// 初始化导航代理
        /// </summary>
        /// <param name="info">导航信息</param>
        public void InitializeNavigationProxy(ref CSUtility.Navigation.NavigationInfo info)
        {
            Cleanup();

            mXCount = info.mXValidLevelCount;
            mZCount = info.mZValidLevelCount;
            mTextureWidth = info.mLevelLengthX;
            mTextureHeight = info.mLevelLengthZ;
            SlimDX.Vector2 tileCount = new SlimDX.Vector2(mTextureWidth, mTextureHeight);

            float cellX = mTextureWidth * mMeterPerPixelX;
            float cellZ = mTextureHeight * mMeterPerPixelZ;

            unsafe
            {
                mNavigationRenderEnvArray = new IntPtr[mXCount * mZCount];
                for (UInt32 z = 0; z < mZCount; z++)
                {
                    for (UInt32 x = 0; x < mXCount; x++)
                    {
                        var renderEnv = DllImportAPI.NavigationRenderEnv_New(CCore.Engine.Instance.Client.Graphics.Device);
                        DllImportAPI.NavigationRenderEnv_SetNavigationTexSize(renderEnv, mTextureWidth, mTextureHeight, mMeterPerPixelX, mMeterPerPixelZ);
                        DllImportAPI.NavigationRenderEnv_SetCameraPos(renderEnv,
                                                                    cellX * 0.5f + x * cellX,
                                                                    800,
                                                                    cellZ * 0.5f + z * cellZ);

                        mNavigationRenderEnvArray[z * mXCount + x] = renderEnv;

                        var decalMat = Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.DefaultNavigationDecalTechId);
                        decalMat.SetFloat2("TileCount", ref tileCount);
                        var decal = new CCore.Component.Decal();
                        decal.ShowRangeMesh = false;
                        decal.ShowSignMesh = false;
                        decal.SetMaterial(decalMat);

                        var texList = new List<CCore.Graphics.Texture>();
                        for (int i = 0; i < 2; i++)
                        {
                            var iTexAuto = CCore.Graphics.Texture.CreateTexture(mTextureWidth, mTextureHeight, 21, 0, 0, 1);
                            decalMat.SetTexture("DiffuseTexture" + i, iTexAuto);
                            texList.Add(iTexAuto);
                        }
                        mGenerateTextures.Add(texList);

                        mNavigationDecals.Add(decal);
                        mNavDecalMaterials.Add(decalMat);
                    }
                }
            }
        }
        /// <summary>
        /// 清空提交的绘制对象
        /// </summary>
        /// <param name="tX">绘制到屏幕的X轴坐标</param>
        /// <param name="tZ">绘制到屏幕的Y轴坐标</param>
        public void ClearDrawingCommits(UInt32 tX, UInt32 tZ)
        {
            if ((tX > mXCount) || (tZ > mZCount))
                return;

            var idx = tZ * mXCount + tX;
            if (idx >= mNavigationRenderEnvArray.Length)
                return;

            unsafe
            {
                DllImportAPI.NavigationRenderEnv_ClearAllDrawingCommits(mNavigationRenderEnvArray[idx]);
            }
        }
        /// <summary>
        /// 提交mesh对象
        /// </summary>
        /// <param name="tX">X轴坐标</param>
        /// <param name="tZ">Z轴坐标</param>
        /// <param name="mesh">mesh对象</param>
        /// <param name="matrix">位置矩阵</param>
        public void CommitMesh(UInt32 tX, UInt32 tZ, CCore.Mesh.Mesh mesh, ref SlimDX.Matrix matrix)
        {
            if ((tX > mXCount) || (tZ > mZCount))
                return;

            unsafe
            {
                var tempMat = matrix;

                for (int i = 0; i < mesh.MeshParts.Count; ++i)
                {
                    var idx = tZ * mXCount + tX;

                    if (mesh.HostActor != null && mesh.HostActor is CCore.World.DynamicBlockActor)
                    {
                        if (mesh.MeshParts[i].Mesh != IntPtr.Zero)
                            DllImportAPI.NavigationRenderEnv_CommitMesh(mNavigationRenderEnvArray[idx], Engine.Instance.GetFrameMillisecond(), mesh.MeshParts[i].Mesh, &tempMat, (int)enMeshCommitType.MCT_DynamicBlock);
                    }
                    else
                    {
                        if (mesh.MeshParts[i].SimplifyMesh != IntPtr.Zero)
                            DllImportAPI.NavigationRenderEnv_CommitMesh(mNavigationRenderEnvArray[idx], Engine.Instance.GetFrameMillisecond(), mesh.MeshParts[i].SimplifyMesh, &tempMat, (int)enMeshCommitType.MCT_Block);

                        if (mesh.MeshParts[i].PathMesh != IntPtr.Zero)
                            DllImportAPI.NavigationRenderEnv_CommitMesh(mNavigationRenderEnvArray[idx], Engine.Instance.GetFrameMillisecond(), mesh.MeshParts[i].PathMesh, &tempMat, (int)enMeshCommitType.MCT_Path);
                    }
                }
            }
        }
        /// <summary>
        /// 提交地形
        /// </summary>
        /// <param name="time">提交时间</param>
        /// <param name="tX">X轴坐标</param>
        /// <param name="tZ">Z轴坐标</param>
        /// <param name="patch">地形的 补丁指针</param>
        /// <param name="matrix">位置矩阵</param>
        public void CommitTerrain(Int64 time, UInt32 tX, UInt32 tZ, IntPtr patch, ref SlimDX.Matrix matrix)
        {
            if ((tX > mXCount) || (tZ > mZCount))
                return;

            var idx = tZ * mXCount + tX;

            unsafe
            {
                var matTemp = matrix;
                DllImportAPI.NavigationRenderEnv_CommitTerrain(mNavigationRenderEnvArray[idx], time, patch, &matTemp);
            }
        }
        /// <summary>
        /// 绘制函数
        /// </summary>
        /// <param name="tX">X轴坐标</param>
        /// <param name="tZ">Z轴坐标</param>
        public void Draw(UInt32 tX, UInt32 tZ)
        {
            if ((tX > mXCount) || (tZ > mZCount))
                return;

            unsafe
            {
                DllImportAPI.NavigationRenderEnv_Draw(mNavigationRenderEnvArray[tZ * mXCount + tX]);
            }
        }

        // 自动生成寻路信息
        /// <summary>
        /// 自动生成寻路信息
        /// </summary>
        /// <param name="world">寻路所在的世界</param>
        /// <param name="genInfo">寻路生成参数</param>
        public void GenerateNavigation(CCore.World.World world, NavigationGenerateInfo genInfo)
        {
            if (world == null)
                return;

            mNavigationGenerateInfo = genInfo;

            if (CCore.Navigation.Navigation.Instance.NavigationData != null)
            {
                if (mNavigationGenerateInfo.mClearManualData)
                    CCore.Navigation.Navigation.Instance.NavigationData.ClearNavData(CSUtility.Navigation.INavigationDataWrapper.enNavDataType.NDT_Manual, Engine.Instance.GetFrameMillisecond());

                CCore.Navigation.Navigation.Instance.NavigationData.ClearNavData(CSUtility.Navigation.INavigationDataWrapper.enNavDataType.NDT_Auto, Engine.Instance.GetFrameMillisecond());
                BuildNavigationFromData(CCore.Navigation.Navigation.Instance.NavigationData);
            }

            // 生成寻路图
            GenerateNavigation(world);
        }

        /// <summary>
        /// 生成寻路图
        /// </summary>
        /// <param name="world">寻路所在的世界</param>
        public virtual void GenerateNavigation(CCore.World.World world)
        {
            if (CCore.Navigation.Navigation.Instance.NavigationData == null || !CCore.Navigation.Navigation.Instance.NavigationData.IsAvailable())
            {
                //InitializeNavigation();
            }

            CSUtility.Navigation.NavigationInfo navInfo;// = new Navigation.INavigationInfo();
            CCore.Navigation.Navigation.Instance.NavigationData.GetNavigationInfo(out navInfo);
            System.UInt32 xCount = navInfo.mXValidLevelCount;
            System.UInt32 zCount = navInfo.mZValidLevelCount;
            System.UInt32 texWidth = navInfo.mLevelLengthX;
            System.UInt32 texHeight = navInfo.mLevelLengthZ;
            float cellX = texWidth * navInfo.mMeterPerPixelX;
            float cellZ = texHeight * navInfo.mMeterPerPixelZ;

            //if (mTerrain != null)
            //{
            //    mTerrain.GetLevelAvailableXZCount(ref xCount, ref zCount);

            //    // 根据地形level来生成寻路图
            //    cellX = (UInt32)mTerrain.GetXLengthPerLevel();
            //    cellZ = (UInt32)mTerrain.GetZLengthPerLevel();
            //    //navigationAssist.SetNavigationTexSize(cellX, cellZ);
            //    texWidth = (UInt32)(cellX / navigationAssist.MeterPerPixelX);
            //    texHeight = (UInt32)(cellZ / navigationAssist.MeterPerPixelZ);
            //}
            //else
            //{
            //    // 使用默认贴图大小
            //    //navigationAssist.SetNavigationTexSize(-1, -1);
            //    texWidth = navigationAssist.DefaultTextureWidth;
            //    texHeight = navigationAssist.DefaultTextureHeight;
            //    cellX = texWidth * navigationAssist.MeterPerPixelX;
            //    cellZ = texHeight * navigationAssist.MeterPerPixelZ;
            //    xCount = (UInt32)(m_sceneInit.SceneWidth / cellX);
            //    zCount = (UInt32)(m_sceneInit.SceneHeight / cellZ);
            //    //Tile_2D.GetGrids()
            //}

            //navigationAssist.InitializeNavigationProxy(ref navInfo);//xCount, zCount, texWidth, texHeight);

            // 遍历每一个绘制单元
            for (UInt32 lZ = 0; lZ < zCount; lZ++)
            {
                for (UInt32 lX = 0; lX < xCount; lX++)
                {
                    float startX = lX * cellX;
                    float endX = (lX + 1) * cellX;
                    float startZ = lZ * cellZ;
                    float endZ = (lZ + 1) * cellZ;

                    // 绘制两次，否则画不出来
                    for (int i = 0; i < 2; i++)
                    {
                        ClearDrawingCommits(lX, lZ);
                        if (world.Terrain != null)
                            world.Terrain.RenderNavigation(lX, lZ, startX, startZ, endX, endZ, this);

                        //var drawGrids = Tile_2D.GetGrids(lX * cellX, lZ * cellZ, 
                        //                                (lX + 1) * cellX, (lZ + 1) * cellZ);
                        //foreach (var grid in drawGrids)
                        //{
                        //    foreach (var actor in grid.Actors.Values)
                        //    {
                        //        var mesh = actor.Visual as Mesh.Mesh;
                        //        if (mesh != null && mesh.BlockNavigation)
                        //        {
                        //            SlimDX.Matrix matrix;
                        //            if (actor.Placement != null && actor.Placement.GetAbsMatrix(out matrix))
                        //            {
                        //                navigationAssist.CommitMesh(lX, lZ, mesh, ref matrix);
                        //            }
                        //        }
                        //    }
                        //}
                        world.SceneGraph.RenderNavigation(lX, lZ, startX, startZ, endX, endZ, this);

                        Draw(lX, lZ);
                    }

                    // 根据绘制的结果生成寻路数据
                    var navData = CCore.Navigation.Navigation.Instance.NavigationData;
                    BuildNavigationData(lX, lZ, ref navData);
                }
            }
        }

        /// <summary>
        /// 创建导航数据
        /// </summary>
        /// <param name="tX">X轴坐标</param>
        /// <param name="tZ">Z轴坐标</param>
        /// <param name="navData">导航数据对象</param>
        public void BuildNavigationData(UInt32 tX, UInt32 tZ, ref CSUtility.Navigation.INavigationDataWrapper navData)
        {
            if (mNavigationGenerateInfo == null)
                return;

            if ((tX > mXCount) || (tZ > mZCount))
                return;

            unsafe
            {
                int idx = (int)(tZ * mXCount + tX);
                DllImportAPI.NavigationRenderEnv_BuildNavigationData(mNavigationRenderEnvArray[idx], navData.NavData, tX, tZ, TextureWidth, TextureHeight, mGenerateTextures[idx][0].TexturePtr.ToPointer(), mMeterPerPixelX, mNavigationGenerateInfo.mTerrainBlockAngleDelta, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 根据导航数据创建导航对象
        /// </summary>
        /// <param name="navData">导航数据对象</param>
        public void BuildNavigationFromData(CSUtility.Navigation.INavigationDataWrapper navData)
        {
            CSUtility.Navigation.NavigationInfo navInfo;// = new Navigation.INavigationInfo();
            navData.GetNavigationInfo(out navInfo);

            unsafe
            {
                for (UInt32 lz = 0; lz < mZCount; lz++)
                {
                    for (UInt32 lx = 0; lx < mXCount; lx++)
                    {
                        int idx = (int)(lz * mXCount + lx);
                        var texture = mGenerateTextures[idx][0];

                        DllImportAPI.BuildNavigationFromData(navData.NavData, lx, lz, TextureWidth, TextureHeight, texture.TexturePtr, Engine.Instance.GetFrameMillisecond());
                    }
                }
            }
        }

        // 刷寻路信息
        // centerX,centerZ单位为米, radius单位为像素
        /// <summary>
        /// 绘制导航寻路
        /// </summary>
        /// <param name="centerX">寻路X方向的距离，单位为米</param>
        /// <param name="centerZ">寻路Z方向的距离，单位为米</param>
        /// <param name="radius">半径，单位为像素</param>
        /// <param name="IsBlock">是否为块状</param>
        /// <param name="IsErase">是否擦除</param>
        /// <param name="navData">寻路信息</param>
        public void DrawNavigation(float centerX, float centerZ, int radius, bool IsBlock, bool IsErase, ref CSUtility.Navigation.INavigationDataWrapper navData)
        {
            if (mGenerateTextures.Count == 0)
                return;

            // 获得绘制的贴图范围
            float metRadius = radius * MeterPerPixelX;
            float minX = centerX - metRadius;
            if (minX < 0)
                minX = 0;
            float maxX = centerX + metRadius;
            float minZ = centerZ - metRadius;
            if (minZ < 0)
                minZ = 0;
            float maxZ = centerZ + metRadius;

            UInt32 minTX = (UInt32)(minX / MeterPerPixelX) / TextureWidth;
            UInt32 maxTX = (UInt32)(maxX / MeterPerPixelX) / TextureWidth;
            if (maxTX >= mXCount)
                maxTX = mXCount - 1;
            UInt32 minTZ = (UInt32)(minZ / MeterPerPixelZ) / TextureHeight;
            UInt32 maxTZ = (UInt32)(maxZ / MeterPerPixelZ) / TextureHeight;
            if (maxTZ >= mZCount)
                maxTZ = mZCount - 1;

            for (UInt32 z = minTZ; z <= maxTZ; z++)
            {
                for (UInt32 x = minTX; x <= maxTX; x++)
                {
                    DrawNavigation(x, z, centerX, centerZ, radius, IsBlock, IsErase, ref navData);
                }
            }
        }
        /// <summary>
        /// 绘制寻路信息
        /// </summary>
        /// <param name="tX">X坐标</param>
        /// <param name="tZ">Z轴坐标</param>
        /// <param name="centerX">寻路X方向的距离，单位为米</param>
        /// <param name="centerZ">寻路Z方向的距离，单位为米</param>
        /// <param name="radius">半径，单位为像素</param>
        /// <param name="IsBlock">是否为块状</param>
        /// <param name="IsErase">是否可擦除</param>
        /// <param name="navData">寻路数据</param>
        public void DrawNavigation(UInt32 tX, UInt32 tZ, float centerX, float centerZ, int radius, bool IsBlock, bool IsErase, ref CSUtility.Navigation.INavigationDataWrapper navData)
        {
            if (mGenerateTextures.Count == 0)
                return;

            if ((tX > mXCount) || (tZ > mZCount))
                return;

            var levelIndex = tZ * mXCount + tX;
            var texture = mGenerateTextures[(int)levelIndex][0];

            unsafe
            {
                DllImportAPI.DrawNavigation(texture.TexturePtr, radius, tX, tZ, centerX, centerZ, TextureWidth, TextureHeight, MeterPerPixelX, IsBlock, IsErase, Engine.Instance.GetFrameMillisecond());
            }

            if (!mNavDrawDirtyLevel.Contains(levelIndex))
                mNavDrawDirtyLevel.Add(levelIndex);
        }

        // 将图上的数据存入NavData中
        /// <summary>
        /// 将图上的数据存入NavData中
        /// </summary>
        /// <param name="navData">寻路信息数据</param>
        public void DrawNavigationToData(ref CSUtility.Navigation.INavigationDataWrapper navData)
        {
            if (navData == null)
                return;

            if (mNavDrawDirtyLevel.Count == 0)
                return;

            if (mGenerateTextures.Count == 0)
                return;

            UInt32 tX = 0, tZ = 0;
            foreach (UInt32 index in mNavDrawDirtyLevel)
            {
                tZ = index / mXCount;
                tX = index % mXCount;
                if ((tX > mXCount) || (tZ > mZCount))
                    continue;

                var texture = mGenerateTextures[(int)(tZ * mXCount + tX)][0];

                unsafe
                {
                    DllImportAPI.DrawNavigationToData(navData.NavData, texture.TexturePtr, tX, tZ, TextureWidth, TextureHeight, Engine.Instance.GetFrameMillisecond());
                }
            }

            mNavDrawDirtyLevel.Clear();
        }

        // 绘制寻路路径
        /// <summary>
        /// 绘制寻路路径
        /// </summary>
        /// <param name="path">找到的路径列表</param>
        public void DrawNavigationPath(List<SlimDX.Vector2> path)
        {
            if (mGenerateTextures.Count == 0)
                return;

            // todo: 如果显示比较慢，则根据Level来设置显示以确保lock次数不多来保证效率

            foreach (var pt in mOldPath)
            {
                UInt32 tX = (UInt32)(pt.X / MeterPerPixelX) / mTextureWidth;
                UInt32 tZ = (UInt32)(pt.Y / MeterPerPixelZ) / mTextureHeight;
                if ((tX > mXCount) || (tZ > mZCount))
                    continue;

                UInt32 x = (UInt32)(pt.X / MeterPerPixelX) % mTextureWidth;
                UInt32 z = (UInt32)(mTextureHeight - (UInt32)(pt.Y / MeterPerPixelZ) % mTextureHeight - 1);

                var texture = mGenerateTextures[(int)(tZ * mXCount + tX)][1];

                unsafe
                {
                    DllImportAPI.DrawNavigationPath(texture.TexturePtr, x, z, TextureWidth, TextureHeight, 0);
                }
            }

            foreach (var pt in path)
            {
                UInt32 tX = (UInt32)(pt.X / MeterPerPixelX) / mTextureWidth;
                UInt32 tZ = (UInt32)(pt.Y / MeterPerPixelZ) / mTextureHeight;
                if ((tX > mXCount) || (tZ > mZCount))
                    continue;

                UInt32 x = (UInt32)(pt.X / MeterPerPixelX) % mTextureWidth;
                UInt32 z = (UInt32)(mTextureHeight - (UInt32)(pt.Y / MeterPerPixelZ) % mTextureHeight - 1);

                var texture = mGenerateTextures[(int)(tZ * mXCount + tX)][1];

                unsafe
                {
                    DllImportAPI.DrawNavigationPath(texture.TexturePtr, x, z, TextureWidth, TextureHeight, 255);
                }
            }

            mOldPath = path;
        }
        /// <summary>
        /// 显示生成的寻路
        /// </summary>
        /// <param name="result">找到的路径点列表</param>
        public void OnFindPath(List<SlimDX.Vector2> result)
        {
            if (CCore.Navigation.NavigationAssist.Instance.ShowFindedPath)
            {
                // 显示生成的寻路
                CCore.Navigation.NavigationAssist.Instance.DrawNavigationPath(result);
            }
        }
    }
}
