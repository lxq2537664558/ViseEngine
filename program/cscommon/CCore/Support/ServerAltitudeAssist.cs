using System;

namespace CCore.Support
{
    /// <summary>
    /// 服务器高度图的帮助类
    /// </summary>
    public class ServerAltitudeAssist
    {
        IntPtr[] mServerHeightMapRenderEnv; //vSimulation.ServerAltitudeRenderEnv**

        static ServerAltitudeAssist mInstance = new ServerAltitudeAssist();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static ServerAltitudeAssist Instance
        {
            get { return mInstance; }
        }

        CSUtility.ServerMap.ServerAltitudeDataWrapper mMapDataWrapper = null;
        
        //class DirtyData
        //{
        //    public UInt16 LevelX;
        //    public UInt16 LevelZ;
        //}
        //List<DirtyData> DirtyDatas = new List<DirtyData>();
        /// <summary>
        /// 更新服务器高度图等级是否为脏
        /// </summary>
        /// <param name="minX">X轴的最小值</param>
        /// <param name="maxX">X轴的最大值</param>
        /// <param name="minZ">Z轴的最小值</param>
        /// <param name="maxZ">Z轴的最大值</param>
        public void UpdateServerHeightMapLevelDirtys(UInt16 minX, UInt16 maxX, UInt16 minZ, UInt16 maxZ)
        {
            for (var z = minZ; z <= maxZ; z++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    SetLevelDirty(x, z);
                    //bool bFind = false;
                    //foreach (var dirtyData in DirtyDatas)
                    //{
                    //    if (dirtyData.LevelX == x && dirtyData.LevelZ == z)
                    //    {
                    //        bFind = true;
                    //        break;
                    //    }
                    //}

                    //if (!bFind)
                    //{
                    //    var data = new DirtyData()
                    //    {
                    //        LevelX = x,
                    //        LevelZ = z,
                    //    };
                    //    DirtyDatas.Add(data);
                    //}
                }
            }
        }
        /// <summary>
        /// 只读属性，X轴的数量
        /// </summary>
        public uint XCount
        {
            get
            {
                if (mMapDataWrapper == null || mMapDataWrapper.MapInfo == null)
                    return 0;

                return mMapDataWrapper.MapInfo.XValidLevelCount;
            }
        }
        /// <summary>
        /// 只读属性，Z轴的数量
        /// </summary>
        public uint ZCount
        {
            get
            {
                if (mMapDataWrapper == null || mMapDataWrapper.MapInfo == null)
                    return 0;

                return mMapDataWrapper.MapInfo.ZValidLevelCount;
            }
        }
        /// <summary>
        /// 纹理的宽
        /// </summary>
        public uint TextureWidth
        {
            get
            {
                if (mMapDataWrapper == null || mMapDataWrapper.MapInfo == null)
                    return 1024;
                return mMapDataWrapper.MapInfo.LevelLengthX;
            }
        }
        /// <summary>
        /// 纹理的高
        /// </summary>
        public uint TextureHeight
        {
            get
            {
                if (mMapDataWrapper == null || mMapDataWrapper.MapInfo == null)
                    return 1024;
                return mMapDataWrapper.MapInfo.LevelLengthZ;
            }
        }
        /// <summary>
        /// 只读属性，X轴上每个像素对应多少米
        /// </summary>
        public float MeterPerPixelX
        {
            get
            {
                if (mMapDataWrapper == null || mMapDataWrapper.MapInfo == null)
                    return 0.5f;
                return mMapDataWrapper.MapInfo.MeterPerPixelX;
            }
        }
        /// <summary>
        /// 只读属性，Z轴上每个像素对应多少米
        /// </summary>
        public float MeterPerPixelZ
        {
            get
            {
                if (mMapDataWrapper == null || mMapDataWrapper.MapInfo == null)
                    return 0.5f;
                return mMapDataWrapper.MapInfo.MeterPerPixelZ;
            }
        }

        ServerAltitudeAssist()
        {

        }
        ~ServerAltitudeAssist()
        {
            Cleanup();
        }
        /// <summary>
        /// 生成服务器高度图数据
        /// </summary>
        /// <param name="tX">X轴的高度</param>
        /// <param name="tZ">Z轴的高度</param>
        protected void BuildServerHeightMapData(uint tX, uint tZ)
        {
            unsafe
            {
                if(mMapDataWrapper == null)
			        return;

		        if((tX > XCount) || (tZ > ZCount))
			        return;
		        
                CSUtility.ServerMap.ServerAltitudeLevelData mapLevel = mMapDataWrapper.GetLevelData(tX, tZ, true);
                if (!mapLevel.IsDirty())
                    return;

		        // 根据图来生成高度数据
                var pixelBuffer = DllImportAPI.ServerAltitudeRenderEnv_CreatePixelBuffer(mServerHeightMapRenderEnv[tZ * XCount + tX]);
		
		        for(uint h=0; h<TextureHeight; h++)
		        {
                    for (uint w = 0; w < TextureWidth; w++)
			        {
				        // 贴图第一个点为 x=0,z=LevelMaxZ的点
                        float* pixel = DllImportAPI.IG32R32FPixelBuffer_GetPixel(pixelBuffer, (int)w, (int)h);
				        float r = pixel[0];	// 高度信息

				        mapLevel.SetHeight(w, h, r);
			        }
		        }

                DllImportAPI.IG32R32FPixelBuffer_Release(pixelBuffer);
            }
        }
        /// <summary>
        /// 服务器高度图的通用数据
        /// </summary>
        /// <param name="world">服务器高度图的世界</param>
        /// <param name="tX">X轴的高度</param>
        /// <param name="tZ">Z轴的高度</param>
        protected void GenerateServerHeightMapData(CCore.World.World world, uint tX, uint tZ)
        {
            CSUtility.ServerMap.ServerAltitudeLevelData mapLevel = mMapDataWrapper.GetLevelData(tX, tZ, true);
            if (!mapLevel.IsDirty())
                return;

            ClearDrawingCommits(tX, tZ);

            if (world.Terrain != null)
            {
                world.Terrain.RenderServerHeightMap(tX, tZ, this);
            }

            if (world.SceneGraph != null)
            {
                world.SceneGraph.RenderServerHeightMap(tX, tZ, this);
            }

            Draw(tX, tZ);
        }
        /// <summary>
        /// 清除数据，释放指针
        /// </summary>
        public virtual void Cleanup()
        {
            if (mServerHeightMapRenderEnv != null)
            {
                foreach (var env in mServerHeightMapRenderEnv)
                {
                    if(env != IntPtr.Zero)
                        DllImportAPI.ServerAltitudeRenderEnv_Delete(env);
                }
                mServerHeightMapRenderEnv = null;
            }

            if (mMapDataWrapper != null)
            {
                mMapDataWrapper.Cleanup();
                mMapDataWrapper = null;
            }
        }
        /// <summary>
        /// 初始化服务器高度图
        /// </summary>
        /// <param name="info">服务器高度图的信息</param>
        public void InitializeServerHeightMapProxy(CSUtility.ServerMap.ServerAltitudeInfo info)
        {
            unsafe
            {
                Cleanup();

                mMapDataWrapper = new CSUtility.ServerMap.ServerAltitudeDataWrapper();
                mMapDataWrapper.Initialize(info);

                SlimDX.Vector2 tileCount = new SlimDX.Vector2(TextureWidth, TextureHeight);

                float cellX = TextureWidth * MeterPerPixelX;
                float cellZ = TextureHeight * MeterPerPixelZ;

                mServerHeightMapRenderEnv = new IntPtr[XCount * ZCount];
                for (uint z = 0; z < ZCount; z++)
                {
                    for (uint x = 0; x < XCount; x++)
                    {
                        var renderEnv = DllImportAPI.ServerAltitudeRenderEnv_New(Engine.Instance.Client.Graphics.Device);
                        DllImportAPI.ServerAltitudeRenderEnv_SetTexSize(renderEnv, TextureWidth, TextureHeight, MeterPerPixelX, MeterPerPixelZ);
                        SlimDX.Vector3 eyePos = new SlimDX.Vector3(cellX * 0.5f + x * cellX, 800, cellZ * 0.5f + z * cellZ);
                        DllImportAPI.ServerAltitudeRenderEnv_SetCameraPos(renderEnv, &eyePos);

                        mServerHeightMapRenderEnv[z * XCount + x] = renderEnv;
                    }
                }
            }
        }
        /// <summary>
        /// 清除绘制提交
        /// </summary>
        /// <param name="tX">X轴的高度</param>
        /// <param name="tZ">Z轴的高度</param>
		public void ClearDrawingCommits(uint tX, uint tZ)
        {
            if ((tX > XCount) || (tZ > ZCount))
                return;

            if (mServerHeightMapRenderEnv == null)
                return;
            if (mServerHeightMapRenderEnv.Length <= tZ * XCount + tX)
                return;

            DllImportAPI.ServerAltitudeRenderEnv_ClearAllDrawingCommits(mServerHeightMapRenderEnv[tZ * XCount + tX]);
        }
        /// <summary>
        /// 提交mesh对象
        /// </summary>
        /// <param name="tX">X轴的高度</param>
        /// <param name="tZ">Z轴的高度</param>
        /// <param name="mesh">mesh对象</param>
        /// <param name="matrix">mesh对象的矩阵</param>
        public void CommitMesh(uint tX, uint tZ, CCore.Mesh.Mesh mesh, ref SlimDX.Matrix matrix)
        {
            unsafe
            {
                if((tX > XCount) || (tZ > ZCount))
			        return;

                fixed(SlimDX.Matrix* pinMatrix = &matrix)
                {
		            for(int i=0; i<mesh.MeshParts.Count; ++i)
		            {
			            if(mesh.MeshParts[i].SimplifyMesh != IntPtr.Zero)
                            DllImportAPI.ServerAltitudeRenderEnv_CommitMesh(mServerHeightMapRenderEnv[tZ * XCount + tX], Engine.Instance.GetFrameMillisecond(), mesh.MeshParts[i].SimplifyMesh, pinMatrix);
		            }
                }
            }
        }
        /// <summary>
        /// 提交地形
        /// </summary>
        /// <param name="time">提交的时间</param>
        /// <param name="tX">X轴的高度</param>
        /// <param name="tZ">Z轴的高度</param>
        /// <param name="terrainPatch">地形补丁</param>
        /// <param name="matrix">地形矩阵</param>
        public void CommitTerrain(Int64 time, uint tX, uint tZ, IntPtr terrainPatch, ref SlimDX.Matrix matrix)
        {
            unsafe
            {
		        if((tX > XCount) || (tZ > ZCount))
			        return;

                fixed(SlimDX.Matrix* pinMatrix = &matrix)
                {
                    DllImportAPI.ServerAltitudeRenderEnv_CommitTerrain(mServerHeightMapRenderEnv[tZ * XCount + tX], time, terrainPatch, pinMatrix);
                }
            }
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="tX">X轴的高度</param>
        /// <param name="tZ">Z轴的高度</param>
        public void Draw(uint tX, uint tZ)
        {
            if ((tX > XCount) || (tZ > ZCount))
                return;

            DllImportAPI.ServerAltitudeRenderEnv_Draw(mServerHeightMapRenderEnv[tZ * XCount + tX]);
        }
        /// <summary>
        /// 设置脏度
        /// </summary>
        /// <param name="tX">X轴的高度</param>
        /// <param name="tZ">Z轴的高度</param>
        public void SetLevelDirty(uint tX, uint tZ)
        {
		    if(mMapDataWrapper == null)
			    return;

            CSUtility.ServerMap.ServerAltitudeLevelData lvData = mMapDataWrapper.GetLevelData(tX, tZ, true);
		    if(lvData == null)
			    return;

		    lvData.SetIsDirty(true);
        }
        /// <summary>
        /// 设置脏度等级
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="z">Z轴坐标</param>
        public void SetLevelDirtyF(float x, float z)
        {
            uint tX = (uint)(x / (TextureWidth * MeterPerPixelX));
            uint tZ = (uint)(z / (TextureHeight * MeterPerPixelZ));

            SetLevelDirty(tX, tZ);
        }

		// 生成高度信息
        /// <summary>
        /// 生成高度信息
        /// </summary>
        /// <param name="world">所在的世界对象</param>
        public void BuildServerHeightMapData(CCore.World.World world)
        {
		    if(mMapDataWrapper == null)
			    return;

		    for(uint z = 0; z < ZCount; z++)
		    {
			    for(uint x = 0; x < XCount; x++)
			    {
                    CSUtility.ServerMap.ServerAltitudeLevelData level = mMapDataWrapper.GetLevelData(x, z, true);
				    if(level == null)
					    continue;

				    //if(level.IsDirty() == false)
				    //	continue;

				    // 生成高度图
				    GenerateServerHeightMapData(world, level.XIndex, level.ZIndex);

				    // 根据高度图生成高度数据
				    BuildServerHeightMapData(level.XIndex, level.ZIndex);
			    }
		    }
        }
        /// <summary>
        /// 保存服务器高度图数据到文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="filePath">文件路径</param>
        public void Save(string fileName, string filePath)
        {
            if(mMapDataWrapper == null)
			    return;

		    string relativePath = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(filePath);

		    if(!System.IO.Directory.Exists(filePath))
		    {
			    System.IO.Directory.CreateDirectory(filePath);
		    }

		    // 存储基本信息
		    {
			    CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.NewXNDHolder();

			    CSUtility.Support.XndAttrib attr = holder.Node.AddAttrib("Header");
			    attr.BeginWrite();
			    mMapDataWrapper.MapInfo.Write(attr);
			    attr.EndWrite();

			    CSUtility.Support.XndHolder.SaveXND(relativePath + fileName, holder);
		    }

            CSUtility.ServerMap.ServerAltitudeLevelData[] levels = mMapDataWrapper.GetLevelDatas();

		    if(levels == null)
			    return;

		    // 存储Level
		    foreach (CSUtility.ServerMap.ServerAltitudeLevelData level in levels)
		    {
			    if(level == null)
				    continue;

			    if(level.IsDirty() == false)
				    continue;

			    string levelFileName = level.XIndex + "_" + level.ZIndex + ".SHLevel";
                level.SaveLevel(relativePath + levelFileName);

			    //level.SaveLevel();
		    }

		    // todo:通知服务器读取改变的地图
        }
    }
}
