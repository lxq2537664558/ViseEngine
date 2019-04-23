using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Terrain
{
    /// <summary>
    /// 地形信息结构体
    /// </summary>
    public struct TerrainInfo
    {
        /// <summary>
        /// 格子单位相对于米的比例
        /// </summary>
        public SlimDX.Vector3 GridStep;
        /// <summary>
        /// 起始坐标
        /// </summary>
        public SlimDX.Vector3 Start;
        /// <summary>
        /// 地形块X和Z
        /// </summary>
		public uint	LevelX,LevelZ;
        /// <summary>
        /// 每个地形块包含的地形片数量调节参数
        /// </summary>
		public uint	PatchDepthPerLevel;
        /// <summary>
        /// 每地形块在X和Z方向上分别包含的地形片数量
        /// </summary>
		public uint	PatchPerLevelX,PatchPerLevelZ;
        /// <summary>
        /// 每个地形片包含的格子数量调节参数
        /// </summary>
		public uint	MaxPatchTessellationLevel;
        /// <summary>
        /// 每片在X和Z方向上分别包含格子数
        /// </summary>
        public uint GridPerPatchX,GridPerPatchZ;
        /// <summary>
        /// 恢复为默认值
        /// </summary>
        public void ResetDefault()
		{
			LevelX = LevelZ = 4;
			PatchDepthPerLevel = 2;
			PatchPerLevelX = PatchPerLevelZ = (uint)System.Math.Pow(2,PatchDepthPerLevel);//缺省一个Level有32*32个渲染单位
			MaxPatchTessellationLevel = 3;
            GridPerPatchX = GridPerPatchZ = (uint)System.Math.Pow(2, MaxPatchTessellationLevel);//缺省32米为一个最小渲染单位
			GridStep.X = GridStep.Z = 1.0F;
			GridStep.Y = 0.1f;
			Start.X = Start.Y = Start.Z = 0.0F;
		}
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="x">地形块X</param>
        /// <param name="z">地形块Z</param>
        /// <param name="PatchLevel">每个地形块包含的地形片数量调节参数</param>
        /// <param name="Tesselation">每个地形片包含的格子数量调节参数</param>
		public void SetParameter(uint x,uint z,uint PatchLevel,uint Tesselation)
		{
			LevelX = x;
			LevelZ = z;
			PatchDepthPerLevel = PatchLevel;
			MaxPatchTessellationLevel = Tesselation;
            PatchPerLevelX = PatchPerLevelZ = (uint)System.Math.Pow(2, PatchDepthPerLevel);
            GridPerPatchX = GridPerPatchZ = (uint)System.Math.Pow(2, MaxPatchTessellationLevel);

			GridStep.X = GridStep.Z = 1.0F;
			GridStep.Y = 0.1f;
			Start.X = Start.Y = Start.Z = 0.0F;
		}
        /// <summary>
        /// 获取每个地形片X对应的距离
        /// </summary>
        /// <returns>返回每个地形片X对应的距离</returns>
		public float GetMeterPerPatchX(){
            return GridStep.X * GridPerPatchX;
		}
        /// <summary>
        /// 获取每个地形片Z对应的距离
        /// </summary>
        /// <returns>返回每个地形片Z对应的距离</returns>
		public float GetMeterPerPatchZ(){
            return GridStep.Z * GridPerPatchZ;
		}
        /// <summary>
        /// 获取每个地形块X对应的距离
        /// </summary>
        /// <returns>返回每个地形块X对应的距离</returns>
		public float GetMeterPerLevelX(){
            return GridStep.X * GridPerPatchX * PatchPerLevelX;//缺省1024个格子
		}
        /// <summary>
        /// 获取每个地形块Z对应的距离
        /// </summary>
        /// <returns>返回每个地形块Z对应的距离</returns>
		public float GetMeterPerLevelZ(){
            return GridStep.Z * GridPerPatchZ * PatchPerLevelZ;//缺省1024个格子
		}

    }

    // 可控编辑器操作的地形参数包装类
    /// <summary>
    /// 可控编辑器操作的地形参数包装类
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class TerrainInfoOperator : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        TerrainInfo mOpInfo;
        /// <summary>
        /// 只读属性，地形信息
        /// </summary>
        [Browsable(false)]
        public TerrainInfo OpInfo
        {
            get { return mOpInfo; }
        }
        /// <summary>
        /// 地形块X
        /// </summary>
        [CSUtility.Support.DataValueAttribute("LevelX")]
        [DisplayName("地形块X")]
        public UInt32 LevelX
        {
            get { return mOpInfo.LevelX; }
            set
            {
                mOpInfo.LevelX = value;
                UpdateSizeMeterX();
                OnPropertyChanged("LevelX");
            }
        }
        /// <summary>
        /// 地形块Z
        /// </summary>
        [CSUtility.Support.DataValueAttribute("LevelZ")]
        [DisplayName("地形块Z")]
        public UInt32 LevelZ
        {
            get { return mOpInfo.LevelZ; }
            set
            {
                mOpInfo.LevelZ = value;
                UpdateSizeMeterZ();
                OnPropertyChanged("LevelZ");
            }
        }
        /// <summary>
        /// 每个地形块包含的地形片数量调节参数
        /// </summary>
        [CSUtility.Support.DataValueAttribute("PatchDepthPerLevel")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 6)]
        [Description("每个地形块包含的地形片数量调节参数，计算方式为Pow(2, 数值)")]
        [DisplayName("每块包含的片参数")]
        public UInt32 PatchDepthPerLevel
        {
            get { return mOpInfo.PatchDepthPerLevel; }
            set
            {
                mOpInfo.PatchDepthPerLevel = value;
                mOpInfo.PatchPerLevelX = mOpInfo.PatchPerLevelZ = (uint)System.Math.Pow(2, PatchDepthPerLevel);
                OnPropertyChanged("PatchPerLevelX");
                OnPropertyChanged("PatchPerLevelZ");
                UpdateSizeMeterX();
                UpdateSizeMeterZ();
                OnPropertyChanged("PatchDepthPerLevel");
            }
        }
        /// <summary>
        /// 每地形块在X方向上包含的地形片数量
        /// </summary>
        [Description("每地形块在X方向上包含的地形片数量(自动计算)")]
        [DisplayName("地形片X")]
        public UInt32 PatchPerLevelX
        {
            get { return mOpInfo.PatchPerLevelX; }
        }
        /// <summary>
        /// 每地形块在Z方向上包含的地形片数量
        /// </summary>
        [Description("每地形块在Z方向上包含的地形片数量(自动计算)")]
        [DisplayName("地形片Z")]
        public UInt32 PatchPerLevelZ
        {
            get { return mOpInfo.PatchPerLevelZ; }
        }
        /// <summary>
        /// 每个地形片包含的格子数量调节参数
        /// </summary>
        [CSUtility.Support.DataValueAttribute("MaxPatchTessellationLevel")]
        [Description("每个地形片包含的格子数量调节参数，计算方式为Pow(2, 数值)")]
        [DisplayName("每片包含的格子参数")]
        [CSUtility.Editor.Editor_ValueWithRange(1, 6)]
        public UInt32 MaxPatchTessellationLevel
        {
            get { return mOpInfo.MaxPatchTessellationLevel; }
            set
            {
                mOpInfo.MaxPatchTessellationLevel = value;
                mOpInfo.GridPerPatchX = mOpInfo.GridPerPatchZ = (uint)System.Math.Pow(2, MaxPatchTessellationLevel);//缺省32米为一个最小渲染单位
                OnPropertyChanged("GridPerPatchX");
                OnPropertyChanged("GridPerPatchZ");
                UpdateSizeMeterX();
                UpdateSizeMeterZ();
                OnPropertyChanged("MaxPatchTessellationLevel");
            }
        }
        /// <summary>
        /// 只读属性，每片在X方向上包含格子数
        /// </summary>
        [Description("每片在X方向上包含格子数(自动计算)")]
        [DisplayName("格子数X")]
        public UInt32 GridPerPatchX
        {
            get { return mOpInfo.GridPerPatchX; }
        }
        /// <summary>
        /// 只读属性，每片在Z方向上包含格子数
        /// </summary>
        [Description("每片在Z方向上包含格子数(自动计算)")]
        [DisplayName("格子数Z")]
        public UInt32 GridPerPatchZ
        {
            get { return mOpInfo.GridPerPatchZ; }
        }
        /// <summary>
        /// 格子单位相对于米的比例
        /// </summary>
        [Description("格子单位相对于米的比例（1为每格1米， 0.1为每格0.1米）")]
        [DisplayName("格子数比例")]
        public SlimDX.Vector3 GridStep
        {
            get { return mOpInfo.GridStep; }
            set
            {
                mOpInfo.GridStep = value;
                OnPropertyChanged("GridStep");
            }
        }
        /// <summary>
        /// 起始位置
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Start")]
        [Description("地形0点所在的位置，单位是米")]
        [DisplayName("起始位置")]
        public SlimDX.Vector3 Start
        {
            get { return mOpInfo.Start; }
            set
            {
                mOpInfo.Start = value;
                OnPropertyChanged("Start");
            }
        }

        float mMapSizeMeterX;
        /// <summary>
        /// 地形X大小
        /// </summary>
        [Description("自动计算，单位是米")]
        [DisplayName("地形X大小")]
        public float MapSizeMeterX
        {
            get { return mMapSizeMeterX; }
        }
        float mMapSizeMeterZ;
        /// <summary>
        /// 地形Z大小
        /// </summary>
        [Description("自动计算，单位是米")]
        [DisplayName("地形Z大小")]
        public float MapSizeMeterZ
        {
            get { return mMapSizeMeterZ; }
        }
        /// <summary>
        /// 构造函数，创建实例对象
        /// </summary>
        public TerrainInfoOperator()
        {
            mOpInfo = new TerrainInfo();
            mOpInfo.ResetDefault();

            UpdateSizeMeterX();
            UpdateSizeMeterZ();
        }
        /// <summary>
        /// 更新地形X的大小
        /// </summary>
        private void UpdateSizeMeterX()
        {
            mMapSizeMeterX = mOpInfo.GetMeterPerLevelX() * LevelX;
            OnPropertyChanged("MapSizeMeterX");
        }
        private void UpdateSizeMeterZ()
        {
            mMapSizeMeterZ = mOpInfo.GetMeterPerLevelZ() * LevelZ;
            OnPropertyChanged("MapSizeMeterZ");
        }
    }
    /// <summary>
    /// 高度结果的枚举
    /// </summary>
    public enum EHeightResult : int
	{
		OK,
		OutTerrain,
		EmptyLevel,
		EmptyPatch,
		InvalidValue,
	}
    /// <summary>
    /// 地形类
    /// </summary>
    public class Terrain
    {
        /// <summary>
        /// 地形对象的指针
        /// </summary>
        protected IntPtr mTerrain;  // vSceneGraph.vTerrain*
        /// <summary>
        /// 只读属性，地形对象的指针
        /// </summary>
        public IntPtr Inner
        {
            get { return mTerrain; }
        }
        /// <summary>
        /// 锁定选择
        /// </summary>
        protected bool mLockCulling;
        /// <summary>
        /// 是否锁定当前的选择项
        /// </summary>
        public bool LockCulling
        {
            get{ return mLockCulling; }
            set{ mLockCulling = value; }
        }
        /// <summary>
        /// 当前选择的摄像机
        /// </summary>
        protected IntPtr mCullingCamera;   // v3dCamera*
        /// <summary>
        /// 地形信息
        /// </summary>
        protected TerrainInfo Info;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Terrain()
        {
        }
        /// <summary>
        /// 析构函数
        /// </summary>
		~Terrain()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
		public virtual void Cleanup()
        {
            unsafe
            {
                if(mCullingCamera != IntPtr.Zero)
                {
                    DllImportAPI.v3dCamera_Release(mCullingCamera);
                    mCullingCamera = IntPtr.Zero;
                }
                if(mTerrain != IntPtr.Zero)
                {
                    DllImportAPI.vTerrain_Release(mTerrain);
                    mTerrain = IntPtr.Zero;
                }
            }
        }

        // name路径相对于Release目录
        //bool NewTerrain(string name, TerrainInfo% ti);
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="absFileName">路径相对于Release目录</param>
        /// <param name="ti">地形信息</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(string absFileName, ref TerrainInfo ti)
        {
            Info = ti;

            unsafe
            {
                fixed(TerrainInfo* pinTi = &ti)
                {
                    Cleanup();

                    string path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(absFileName);
			        string file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(absFileName);

                    mTerrain = DllImportAPI.vTerrain_New();
                    if(DllImportAPI.vTerrain_ConstructTerrain(mTerrain, Engine.Instance.Client.Graphics.Device, file, path, pinTi) == 0)
                        return false;

                    mCullingCamera = DllImportAPI.v3dCamera_New(Engine.Instance.Client.Graphics.Device);

                    return true;
                }
            }
        }
        /// <summary>
        /// 加载地形对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public bool LoadTerrain(string name)
        {
            unsafe
            {
                Cleanup();

			    string path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(name);
			    string file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(name);

                mTerrain = DllImportAPI.vTerrain_New();
                if(DllImportAPI.vTerrain_LoadTerrain(mTerrain, Engine.Instance.Client.Graphics.Device, file, path) == 0)
                    return false;

                mCullingCamera = DllImportAPI.v3dCamera_New(Engine.Instance.Client.Graphics.Device);

                return true;
            }
        }
        // name相对于release路径
        /// <summary>
        /// 保存地形数据
        /// </summary>
        /// <param name="name">带路径的名称，相对于release路径</param>
        /// <param name="forceSave">是否强制保存到磁盘</param>
        public void SaveTerrain(string name, bool forceSave)
        {
            unsafe
            {
			    if(mTerrain==IntPtr.Zero)
				    return;

			    if(string.IsNullOrEmpty(name))
			    {
                    DllImportAPI.vTerrain_SaveDirtyLevel(mTerrain, null, null, forceSave);
			    }
			    else
			    {
				    string path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(name);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
				    string file = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(name);

                    DllImportAPI.vTerrain_SaveDirtyLevel(mTerrain, file, path, forceSave);
			    }
            }
        }
        /// <summary>
        /// 创建地形块
        /// </summary>
        /// <param name="x">X方向的值</param>
        /// <param name="z">Z方向的值</param>
        public void CreateLevel(UInt32 x, UInt32 z)
        {
			if(mTerrain == IntPtr.Zero)
				return;

            DllImportAPI.vTerrain_NewLevel(mTerrain, x, z);
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="x">X轴的移动距离</param>
        /// <param name="z">Z轴的移动距离</param>
        /// <returns>移动成功返回true，否则返回false</returns>
        public bool TravelTo(float x, float z)
        {
            if(mTerrain == IntPtr.Zero)
                return false;

            if (x < Info.Start.X)
                x = Info.Start.X;
            if (z < Info.Start.Z)
                z = Info.Start.Z;
            var maxX = Info.GetMeterPerLevelX() * Info.LevelX + Info.Start.X;
            if (x > maxX)
                x = maxX;
            var maxZ = Info.GetMeterPerLevelZ() * Info.LevelZ + Info.Start.Z;
            if (z > maxZ)
                z = maxZ;

            return DllImportAPI.vTerrain_TravelTo(mTerrain, x, z, Engine.Instance.GetFrameMillisecond()) != 0;
        }
        /// <summary>
        /// 获取地形的高度
        /// </summary>
        /// <param name="idu">u向高度</param>
        /// <param name="idv">v向高度</param>
        /// <param name="Alt">键值</param>
        /// <param name="preUse">是否预览</param>
        /// <returns>返回高度的结果的枚举</returns>
		public EHeightResult GetHeight(UInt32 idu,UInt32 idv, ref short Alt,bool preUse)
        {
            unsafe
            {
                fixed (short* pinAlt = &Alt)
                {
                    Int64 time = Engine.Instance.GetFrameMillisecond();
                    EHeightResult result = (EHeightResult)(DllImportAPI.vTerrain_GetHeight(mTerrain, idu, idv, pinAlt, time, preUse));

                    return result;
                }
            }
        }
        /// <summary>
        /// 设置地形高度
        /// </summary>
        /// <param name="idu">u向高度</param>
        /// <param name="idv">v向高度</param>
        /// <param name="Alt">键值</param>
        /// <param name="preUse">是否预览</param>
        /// <returns>返回高度的结果的枚举</returns>
		public EHeightResult SetHeight(UInt32 idu,UInt32 idv,short Alt,bool preUse)
        {
			Int64 time = Engine.Instance.GetFrameMillisecond();
			UInt32 lvlX = GetLevelX(idu);
			UInt32 lvlZ = GetLevelZ(idv);
            CCore.Support.ServerAltitudeAssist.Instance.SetLevelDirty(lvlX, lvlZ);

            return (EHeightResult)DllImportAPI.vTerrain_SetHeight(mTerrain, idu, idv, Alt, time, preUse);
        }
        /// <summary>
        /// 获取地形高度
        /// </summary>
        /// <param name="x">X轴向的值</param>
        /// <param name="z">Z轴向的值</param>
        /// <param name="Alt">键值</param>
        /// <param name="preUse">是否预览</param>
        /// <returns>返回高度的结果的枚举</returns>
		public EHeightResult GetHeightF(float x, float z, ref float Alt,bool preUse)
        {
            unsafe
            {
                fixed (float* pinAlt = &Alt)
                {
                    Int64 time = Engine.Instance.GetFrameMillisecond();
                    EHeightResult result = (EHeightResult)(DllImportAPI.vTerrain_GetHeightF(mTerrain, x, z, pinAlt, time, preUse));

                    return result;
                }
            }
        }
        /// <summary>
        /// 设置高度
        /// </summary>
        /// <param name="x">X轴向的高度值</param>
        /// <param name="z">Z轴向的高度值</param>
        /// <param name="Alt">键值</param>
        /// <param name="preUse">是否预览</param>
        /// <returns>返回高度的结果的枚举</returns>
		public EHeightResult SetHeightF(float x,float z,short Alt,bool preUse)
        {
			Int64 time = Engine.Instance.GetFrameMillisecond();
			UInt32 u = GetUWithX(x);
			UInt32 v = GetVWithZ(z);
			return SetHeight(u,v,Alt,preUse);
        }
        /// <summary>
        /// 升起的高度
        /// </summary>
        /// <param name="x">X轴向的高度</param>
        /// <param name="z">Z轴向的高度</param>
        /// <param name="Alt">键值</param>
        /// <param name="preUse">是否预览</param>
        /// <returns>返回高度的结果的枚举</returns>
		public EHeightResult RiseHeightF(float x,float z,short Alt,bool preUse)
        {
            UInt32 u = GetUWithX(x);
            UInt32 v = GetVWithZ(z);

            return RiseHeight(u, v, Alt, preUse);
        }
        /// <summary>
        /// 升起高度的差值
        /// </summary>
        /// <param name="idu">u向的值</param>
        /// <param name="idv">v向的值</param>
        /// <param name="Alt">键值</param>
        /// <param name="preUse">是否预览</param>
        /// <returns>返回高度的结果的枚举</returns>
        public EHeightResult RiseHeight(UInt32 idu, UInt32 idv, short Alt, bool preUse)
        {
            if (Alt == 0)
                return EHeightResult.InvalidValue;

            Int64 time = Engine.Instance.GetFrameMillisecond();

            short OriAlt = 0;
            GetHeight(idu, idv, ref OriAlt, preUse);
            OriAlt = (short)(OriAlt + Alt);
            return SetHeight(idu, idv, OriAlt, preUse);
        }
        /// <summary>
        /// 当前地形对象是否可用
        /// </summary>
        /// <returns>如果地形不为空返回true，否则返回false</returns>
		public bool IsAvailable(){
			return mTerrain != IntPtr.Zero;
		}
        /// <summary>
        /// 根据X轴向的值获取u值
        /// </summary>
        /// <param name="x">X轴向的值</param>
        /// <returns>返回u值</returns>
		public UInt32 GetUWithX(float x)
        {
            if (mTerrain == IntPtr.Zero)
                return 0;

            unsafe
            {
                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                float _x = x - info.Start.X;
                if (_x < 0)
                    return 0;
                return (UInt32)(_x / info.GridStep.X);
            }
        }
        /// <summary>
        /// 根据Z轴向的值获取v值
        /// </summary>
        /// <param name="z">Z轴向的值</param>
        /// <returns>返回v值</returns>
		public UInt32 GetVWithZ(float z)
        {
            if (mTerrain == IntPtr.Zero)
                return 0;

            unsafe
            {
                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                float _z = z - info.Start.Z;
                if (_z < 0)
                    return 0;
                return (UInt32)(_z / info.GridStep.Z);
            }
        }
        /// <summary>
        /// 获取起始位置坐标
        /// </summary>
        /// <param name="loc">起始位置坐标</param>
        public void GetStartLocation(ref SlimDX.Vector3 loc)
        {
            unsafe
            {
                if (mTerrain != IntPtr.Zero)
                {
                    TerrainInfo info = *(TerrainInfo*)DllImportAPI.vTerrain_GetTerrainInfo(mTerrain);
                    loc.X = info.Start.X;
                    loc.Y = info.Start.Y;
                    loc.Z = info.Start.Z;
                }
            }
        }
        /// <summary>
        /// 获取地形片的位置坐标
        /// </summary>
        /// <param name="x">X轴的高度</param>
        /// <param name="z">Z轴的高度</param>
        /// <param name="loc">地形片的位置坐标</param>
		public void GetPatchLocation(float x, float z, ref SlimDX.Vector3 loc)
        {
            unsafe
            {
                UInt32 u = GetUWithX(x);
                UInt32 v = GetVWithZ(z);
                float vX, vY, vZ;

                DllImportAPI.vTerrain_GetPatchLocation(mTerrain, u, v, out vX, out vY, out vZ);
                loc.X = vX;
                loc.Y = vY;
                loc.Z = vZ;
            }
        }
        /// <summary>
        /// 获取每一个地形块X包含地形片的数量
        /// </summary>
        /// <returns>返回每一个地形块X包含地形片的数量</returns>
		public UInt32 GetPatchXCountPerLevel()
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 0;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.PatchPerLevelX;
            }
        }
        /// <summary>
        /// 获取每一个地形块Z包含地形片的数量
        /// </summary>
        /// <returns>返回每一个地形块Z包含地形片的数量</returns>
		public UInt32 GetPatchZCountPerLevel()
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 0;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.PatchPerLevelZ;
            }
        }
        /// <summary>
        /// 获取地形片的宽
        /// </summary>
        /// <returns>返回地形片的宽</returns>
		public float GetPatchWidth()
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 0;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.GridPerPatchX * info.GridStep.X;
            }
        }
        /// <summary>
        /// 获取地形片的高
        /// </summary>
        /// <returns>返回地形片的高</returns>
		public float GetPatchHeight()
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 0;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.GridPerPatchZ * info.GridStep.Z;
            }
        }
        /// <summary>
        /// 获得X向地形的格子数量
        /// </summary>
        /// <returns>返回X向地形的格子数量</returns>
		public UInt32 GetGridXCount()	// 获得X向地形的格子数量
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 1;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.LevelX * info.PatchPerLevelX * info.GridPerPatchX;
            }
        }
        /// <summary>
        /// 获得Z向地形的格子数量
        /// </summary>
        /// <returns>返回Z向地形的格子数量</returns>
		public UInt32 GetGridZCount()	// 获得Z向地形的格子数量
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 1;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.LevelZ * info.PatchPerLevelZ * info.GridPerPatchZ;
            }
        }
        /// <summary>
        /// 获得X向地形的大小（默认单位）
        /// </summary>
        /// <returns>返回X向地形的大小</returns>
		public float GetGridX()	// 获得X向地形的大小（默认单位）
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 1;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.LevelX * info.PatchPerLevelX * info.GridPerPatchX * info.GridStep.X;
            }
        }
        /// <summary>
        /// 获得Z向地形的大小
        /// </summary>
        /// <returns>返回Z向地形的大小</returns>
		public float GetGridZ()	// 获得Z向地形的大小（默认单位）
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 1;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.LevelZ * info.PatchPerLevelZ * info.GridPerPatchZ * info.GridStep.Z;
            }
        }
        /// <summary>
        /// 获取地形块X的数量
        /// </summary>
        /// <returns>返回地形块X的数量</returns>
		public UInt32 GetLevelXCount()
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 1;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.LevelX;
            }
        }
        /// <summary>
        /// 获取地形块Z的数量
        /// </summary>
        /// <returns>返回地形块Z的数量</returns>
        public UInt32 GetLevelZCount()
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 1;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.LevelZ;
            }
        }
        /// <summary>
        /// 获取地形块X向的大小
        /// </summary>
        /// <returns>返回地形块X向的大小</returns>
		public float GetXLengthPerLevel()	// 取得Level X向大小
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 0;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.PatchPerLevelX * info.GridPerPatchX * info.GridStep.X;
            }
        }
        /// <summary>
        /// 获取地形块Z向的大小
        /// </summary>
        /// <returns>返回地形块Z向的大小</returns>
		public float GetZLengthPerLevel()	// 取得Level Z向大小
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return 0;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));
                return info.PatchPerLevelZ * info.GridPerPatchZ * info.GridStep.Z;
            }
        }
        /// <summary>
        /// 获取可用地形块的XZ向大小
        /// </summary>
        /// <param name="x">x向的值</param>
        /// <param name="z">z向的值</param>
		public void GetLevelAvailableXZCount(ref System.UInt32 x, ref System.UInt32 z)	// 获取可用Level的XZ向大小
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return;

                TerrainInfo info = *(TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain));

                x = 0;
                z = 0;

                for (UInt32 i = 0; i < info.LevelZ; i++)
                {
                    for (UInt32 j = 0; j < info.LevelX; j++)
                    {
                        var level = DllImportAPI.vTerrain_GetLevel(mTerrain, (UInt16)j, (UInt16)i);
                        if (level != IntPtr.Zero)
                        {
                            if (x < j)
                                x = j;
                            if (z < i)
                                z = i;
                        }
                    }
                }

                x += 1;
                z += 1;
            }
        }
        /// <summary>
        /// 获取地形块X
        /// </summary>
        /// <param name="x">x的值</param>
        /// <returns>返回地形块X</returns>
		public UInt32 GetLevelX(UInt32 x)
        {
			return (UInt32)(x / GetXLengthPerLevel());
        }
        /// <summary>
        /// 获取地形块Z
        /// </summary>
        /// <param name="z">z的值</param>
        /// <returns>返回地形块Z</returns>
		public UInt32 GetLevelZ(UInt32 z)
        {
			return (UInt32)(z / GetZLengthPerLevel());
        }
        /// <summary>
        /// 连线检查
        /// </summary>
        /// <param name="start">线的起点坐标</param>
        /// <param name="end">线的终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <returns>连线无问题返回true，否则返回false</returns>
		public virtual bool LineCheck( ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result)
        {
			return LineCheck(ref start, ref end, ref result, false);
        }
        /// <summary>
        /// 连线检查
        /// </summary>
        /// <param name="start">线的起点坐标</param>
        /// <param name="end">线的终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <param name="withDeletedPatch">是否删除地形片</param>
        /// <returns>连线无问题返回true，否则返回false</returns>
		public virtual bool LineCheck( ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result, bool withDeletedPatch )
        {
            unsafe
            {
                if (mTerrain != IntPtr.Zero)
                {
                    fixed(SlimDX.Vector3* pinStart = &start)
                    fixed(SlimDX.Vector3* pinEnd = &end)
                    fixed (CSUtility.Support.stHitResult* pinResult = &result)
                    {
                        return DllImportAPI.vTerrain_LineCheck(mTerrain, pinStart, pinEnd, pinResult, withDeletedPatch) != 0;
                    }
                }

                return false;
            }
        }
        /// <summary>
        /// 连线检查
        /// </summary>
        /// <param name="start">线的起点坐标</param>
        /// <param name="end">线的终点坐标</param>
        /// <param name="result">点击结果</param>
        /// <param name="exceptActor">额外的Actor对象</param>
        /// <returns>连线无问题返回true，否则返回false</returns>
		public virtual bool LineCheck( ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result, CCore.World.Actor exceptActor )
        {
            unsafe
            {
                if (mTerrain != IntPtr.Zero)
                {
                    fixed (SlimDX.Vector3* pinStart = &start)
                    fixed (SlimDX.Vector3* pinEnd = &end)
                    fixed (CSUtility.Support.stHitResult* pinResult = &result)
                    {
                        return DllImportAPI.vTerrain_LineCheck(mTerrain, pinStart, pinEnd, pinResult, false) != 0;
                    }
                }

                return false;
            }
        }

        long kickOffTime = 1000;
        long curKickOffTime = 0;
        /// <summary>
        /// 每帧调用
        /// </summary>
		public virtual void Tick()
        {
			if(mTerrain==IntPtr.Zero)
				return;

            curKickOffTime += Engine.Instance.GetElapsedMillisecond();
            if(curKickOffTime > kickOffTime)
            {
                DllImportAPI.vTerrain_KickOffCache(mTerrain, Engine.Instance.GetFrameMillisecond(), 15000);
                curKickOffTime = 0;
            }
        }

        //virtual bool AddActor( IActor^ act ) override;
        //virtual bool RemoveActor(IActor^ act) override;
        //virtual void RemoveAllActor() override;

        #region RenderVisible
        /// <summary>
        /// 地形片的可视化类
        /// </summary>
        public class PatchVisitorArg
        {
            /// <summary>
            /// 摄像机指针
            /// </summary>
            public IntPtr Camera;
            /// <summary>
            /// 地形片的数量
            /// </summary>
            public int Count;
            /// <summary>
            /// 时间
            /// </summary>
            public Int64 Time;
            /// <summary>
            /// 渲染环境
            /// </summary>
            public CCore.Graphics.REnviroment RenderEnv;
            /// <summary>
            /// 可视化的地形片
            /// </summary>
            public List<IntPtr> VisiblePatchs;
        }
        /// <summary>
        /// 是否在地形片上终止
        /// </summary>
        public bool StopAtPatch = false;
        /// <summary>
        /// 是否忽略子对象
        /// </summary>
        public bool IgnoreChild = false;

        List<IntPtr> mVisiblePatchs = new List<IntPtr>();
        /// <summary>
        /// 声明可视化地形片访问的委托对象
        /// </summary>
        /// <param name="patch">地形片对象指针</param>
        /// <param name="arg">访问的对象指针</param>
        public delegate void Delegate_TerrainVisibleVisitor_Visit(IntPtr patch, IntPtr arg);
        private static void TerrainVisibleVisitor_Visit(IntPtr patch, IntPtr arg)
        {
            unsafe
            {
                PatchVisitorArg visitorArg = (PatchVisitorArg)(((System.Runtime.InteropServices.GCHandle)(arg)).Target);
                visitorArg.VisiblePatchs.Add(patch);

                //IDllImportAPI.vDSRenderEnv_CommitTerrain(visitorArg.RenderEnv.DSRenderEnv, visitorArg.Time, (int)(RGroup.RL_World), patch, visitorArg.Camera);

                //// Commit Grass Mesh
                //IDllImportAPI.vDSRenderEnv_CommitGrass(visitorArg.RenderEnv.DSRenderEnv, patch, visitorArg.Time, (int)(RGroup.RL_World), visitorArg.Camera);

                visitorArg.Count++;
            }
        }
        static Delegate_TerrainVisibleVisitor_Visit terrainVisibleVisitorEvent = TerrainVisibleVisitor_Visit;
        // 根据眼睛的位置来渲染Level
        /// <summary>
        /// 根据眼睛的位置来渲染地形块
        /// </summary>
        /// <param name="eye">视野</param>
        /// <param name="env">渲染环境</param>
        public virtual void RenderVisible(CCore.Camera.CameraObject eye, CCore.Graphics.REnviroment env)
        {
            unsafe
            {
                if (mCullingCamera == IntPtr.Zero)
                    return;

                mVisiblePatchs.Clear();

                PatchVisitorArg visitor = new PatchVisitorArg();

                if (mLockCulling)
                {
                    visitor.Camera = mCullingCamera;
                }
                else
                {
                    visitor.Camera = eye.CameraPtr;
                    DllImportAPI.v3dCamera_CopyData(mCullingCamera, eye.CameraPtr);
                }
                visitor.Time = Engine.Instance.GetFrameMillisecond();
                visitor.RenderEnv = env;
                visitor.Count = 0;
                visitor.VisiblePatchs = mVisiblePatchs;

                IntPtr pinArg = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(visitor));

                DllImportAPI.vTerrain_CheckVisible(mTerrain, terrainVisibleVisitorEvent, pinArg, visitor.Camera, Engine.Instance.GetFrameMillisecond(), StopAtPatch, IgnoreChild);

                foreach(var patch in mVisiblePatchs)
                {
                    DllImportAPI.vDSRenderEnv_CommitTerrain(visitor.RenderEnv.DSRenderEnv, visitor.Time, (int)(RGroup.RL_World), patch, visitor.Camera);
                }
                foreach (var patch in mVisiblePatchs)
                {
                    DllImportAPI.vDSRenderEnv_CommitGrass(visitor.RenderEnv.DSRenderEnv, patch, visitor.Time, (int)(RGroup.RL_World), visitor.Camera);
                }

                var argHandle = (System.Runtime.InteropServices.GCHandle)(pinArg);
                argHandle.Target = null;
                argHandle.Free();

                visitor.RenderEnv = null;
                visitor.Camera = IntPtr.Zero;
            }
        }


#endregion

#region RenderShadow
        /// <summary>
        /// 访问阴影
        /// </summary>
        public class ShadowVisitorArg
        {
            /// <summary>
            /// 阴影数量
            /// </summary>
            public int Count;
            /// <summary>
            /// 时间
            /// </summary>
            public Int64 Time;
            /// <summary>
            /// 产生阴影的光源
            /// </summary>
            public CCore.Light.Light ShadowLight;
            /// <summary>
            /// 摄像机
            /// </summary>
            public IntPtr Camera;
        }

        private static void TerrainShadowVisitor_Visit(IntPtr patch, IntPtr arg)
        {
            unsafe
            {
                ShadowVisitorArg visitorArg = (ShadowVisitorArg)(((System.Runtime.InteropServices.GCHandle)(arg)).Target);

                SlimDX.Matrix mat = SlimDX.Matrix.Identity;
                DllImportAPI.vDSRenderEnv_CommitShadow_Terrain(visitorArg.ShadowLight.Inner, patch, visitorArg.Time, &mat);

                visitorArg.Count++;
            }
        }
        static Delegate_TerrainVisibleVisitor_Visit terrainShadowVisitorEvent = TerrainShadowVisitor_Visit;
        /// <summary>
        /// 渲染阴影
        /// </summary>
        /// <param name="camera">摄像机</param>
        /// <param name="shadowLights">产生阴影的光源</param>
        public virtual void RenderShadow(CCore.Camera.CameraObject camera, CCore.Light.Light[] shadowLights)
        {
			if(shadowLights == null)
				return;

            foreach(var shadowLight in shadowLights)
            {
                if (shadowLight.ShadowType == Light.EShadowType.None)
                    continue;

                ShadowVisitorArg visitor = new ShadowVisitorArg();

                visitor.Camera = camera.CameraPtr;
                visitor.ShadowLight = shadowLight;

                visitor.Time = Engine.Instance.GetFrameMillisecond();
                visitor.Count = 0;

                unsafe
                {
                    IntPtr pinArg = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(visitor));


                    DllImportAPI.vTerrain_CheckVisible(mTerrain, terrainShadowVisitorEvent, pinArg, visitor.Camera, Engine.Instance.GetFrameMillisecond(), StopAtPatch, IgnoreChild);

                    var argHandle = (System.Runtime.InteropServices.GCHandle)(pinArg);
                    argHandle.Target = null;
                    argHandle.Free();
                }
                visitor.ShadowLight = null;
                visitor.Camera = IntPtr.Zero;
            }
        }

#endregion

#region RenderNavigation
        /// <summary>
        /// 地形导航栏
        /// </summary>
        struct NavgationArg
        {
            /// <summary>
            /// 导航对象
            /// </summary>
            public CCore.Navigation.NavigationAssist Nav;
            /// <summary>
            /// 地形块X
            /// </summary>
            public uint lvlX;
            /// <summary>
            /// 地形块Z
            /// </summary>
            public uint lvlZ;
        }
        // 绘制寻路数据以便生成寻路
        /// <summary>
        /// 绘制寻路数据以便生成寻路
        /// </summary>
        /// <param name="navLvlX">地形块X</param>
        /// <param name="navLvlZ">地形块Z</param>
        /// <param name="startX">地形块X的起点</param>
        /// <param name="startZ">地形块Z的起点</param>
        /// <param name="endX">地形块X的终点</param>
        /// <param name="endZ">地形块Z的终点</param>
        /// <param name="nav">导航对象</param>
        /// <returns>生成成功返回true，否则返回false</returns>
		public bool RenderNavigation(UInt32 navLvlX, UInt32 navLvlZ, float startX, float startZ, float endX, float endZ, CCore.Navigation.NavigationAssist nav)
        {
            unsafe
            {
                UInt32 startLvlX = (UInt32)System.Math.Floor(startX / this.Info.GetMeterPerLevelX());
                UInt32 startLvlZ = (UInt32)System.Math.Floor(startZ / this.Info.GetMeterPerLevelZ());
                UInt32 endLvlX = (UInt32)System.Math.Ceiling(endX / this.Info.GetMeterPerLevelX());
                UInt32 endLvlZ = (UInt32)System.Math.Ceiling(endZ / this.Info.GetMeterPerLevelZ());

                for (UInt32 lvlX = startLvlX; lvlX < endLvlX; lvlX++)
                {
                    for(UInt32 lvlZ = startLvlZ; lvlZ < endLvlZ; lvlZ++)
                    {
                        var pLevel = DllImportAPI.vTerrain_GetRealLevel(mTerrain, lvlX, lvlZ, Engine.Instance.GetFrameMillisecond());
                        TerrainInfo info = *((TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain)));
                        var mat = SlimDX.Matrix.Identity;
                        for (UInt32 patchX = 0; patchX < info.PatchPerLevelX; patchX++)
                        {
                            for (UInt32 patchZ = 0; patchZ < info.PatchPerLevelZ; patchZ++)
                            {
                                var patch = DllImportAPI.vTerrainLevel_GetPatch(pLevel, patchX, patchZ, Engine.Instance.GetFrameMillisecond());
                                if (DllImportAPI.vTerrainPatch_IsDeleted(patch) == 0)
                                {
                                    NavgationArg navArg = new NavgationArg();
                                    navArg.Nav = nav;
                                    navArg.lvlX = navLvlX;
                                    navArg.lvlZ = navLvlZ;
                                    IntPtr pinArg = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(navArg));

                                    nav.CommitTerrain(Engine.Instance.GetFrameMillisecond(), navLvlX, navLvlZ, patch, ref mat);

                                    var argHandle = (System.Runtime.InteropServices.GCHandle)(pinArg);
                                    argHandle.Target = null;
                                    argHandle.Free();
                                }
                            }
                        }
                    }
                }


                return true;
            }
        }

        #endregion

        #region RenderServerHeightMap

        //struct ServerHeightMapArg
        //{
        //    public MidLayer.IServerHeightMapAssist Shm;
        //    public uint lvlX;
        //    public uint lvlZ;
        //}
        // 绘制服务器端高度图
        /// <summary>
        /// 绘制服务器端高度图
        /// </summary>
        /// <param name="lvlX">地形块X</param>
        /// <param name="lvlZ">地形块Z</param>
        /// <param name="shm">服务器高度图对象</param>
        /// <returns>绘制成功返回true，否则返回false</returns>
        public bool RenderServerHeightMap(UInt32 lvlX, UInt32 lvlZ, CCore.Support.ServerAltitudeAssist shm)
        {
            unsafe
            {
                var pLevel = DllImportAPI.vTerrain_GetRealLevel(mTerrain, lvlX, lvlZ, Engine.Instance.GetFrameMillisecond());
                if (pLevel == IntPtr.Zero)
                    return false;

                TerrainInfo info = *((TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain)));
                var mat = SlimDX.Matrix.Identity;

                for (UInt32 patchX = 0; patchX < info.PatchPerLevelX; patchX++)
                {
                    for (UInt32 patchZ = 0; patchZ < info.PatchPerLevelZ; patchZ++)
                    {
                        var patch = DllImportAPI.vTerrainLevel_GetPatch(pLevel, patchX, patchZ, Engine.Instance.GetFrameMillisecond());
                        if (DllImportAPI.vTerrainPatch_IsDeleted(patch) == 0)
                            shm.CommitTerrain(Engine.Instance.GetFrameMillisecond(), lvlX, lvlZ, patch, ref mat);
                    }
                }

                return true;
            }
        }

#endregion

        // 层材质及混合数据
		//void SetPatchMaterial(UInt32 x,UInt32 z,UInt32 index, IMaterial^ mtl);
        /// <summary>
        /// 设置基础材质
        /// </summary>
        /// <param name="matId">材质ID</param>
		public void SetBaseMaterial(System.Guid matId)
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return;

                if (matId == System.Guid.Empty)
                    return;

                DllImportAPI.vTerrain_SetBaseLayerMaterial(mTerrain, &matId, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 获取基本材质
        /// </summary>
        /// <returns>返回基本材质的ID</returns>
		public System.Guid GetBaseMaterial()
        {
            unsafe
            {
			    if(mTerrain == IntPtr.Zero)
				    return System.Guid.Empty;

                Guid matId = System.Guid.Empty;
                DllImportAPI.vTerrain_GetBaseLayerMaterial(mTerrain, &matId);

                return matId;
            }
        }
        /// <summary>
        /// 绘制层的数据
        /// </summary>
        /// <param name="mtl">材质对象</param>
        /// <param name="grassData">草数据对象</param>
        /// <param name="value">值</param>
        /// <param name="x">x坐标</param>
        /// <param name="z">z坐标</param>
        /// <returns>绘制成功返回true，否则返回false</returns>
		public bool PaintLayerData(CCore.Material.Material mtl, CCore.Grass.GrassDataBase grassData, int value, UInt32 x, UInt32 z)
        {
            unsafe
            {
                if (mTerrain == IntPtr.Zero)
                    return false;


                var time = Engine.Instance.GetFrameMillisecond();
                DllImportAPI.vTerrain_PaintLayerData(mTerrain, mtl.MaterialPtr, grassData.GetInner(), value, x, z, time, true);

                return true;
            }
        }
        /// <summary>
        /// 获取层的材质
        /// </summary>
        /// <param name="matIdList">材质ID列表</param>
        /// <param name="grassList">草对象指针列表</param>
        /// <param name="remarkList">标记列表</param>
		public void GetLayerMaterials(List<System.Guid> matIdList, List<System.IntPtr> grassList, List<string> remarkList)
        {
            unsafe
            {
			    if(mTerrain == IntPtr.Zero)
				    return;

			    matIdList.Clear();

                int count = 0;
                Guid* ids = DllImportAPI.vTerrain_GetLayerMaterials_Alloc(mTerrain, &count);
                if (((IntPtr)ids) != IntPtr.Zero)
                {
                    for (int i = 0; i < count; i++)
                    {
                        matIdList.Add(ids[i]);
                    }
                }
                DllImportAPI.vTerrain_GetLayerMaterials_Free(ids);

			    // clone出新的GrassData给上层使用
			    grassList.Clear();
                void** grassDatas = DllImportAPI.vTerrain_GetGrassData_Alloc(mTerrain, &count, Engine.Instance.GetFrameMillisecond());
                if(((IntPtr)grassDatas) != IntPtr.Zero)
                {
                    for(int i=0; i<count; i++)
                    {
                        grassList.Add((IntPtr)(grassDatas[i]));
                    }
                }
                DllImportAPI.vTerrain_GetGrassData_Free(grassDatas);

                remarkList.Clear();
                void** remarks = DllImportAPI.vTerrain_GetRemarks_Alloc(mTerrain, &count);
                if (((IntPtr)remarks) != IntPtr.Zero)
                {
                    for (int i = 0; i < count; i++)
                    {
                        remarkList.Add(System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(remarks[i])));
                    }
                }
                DllImportAPI.vTerrain_GetRemarks_Free(remarks, count);
            }
        }
        /// <summary>
        /// 添加层材质
        /// </summary>
        /// <param name="matId">材质ID</param>
		public void AddLayerMaterial(System.Guid matId)
        {
			if(mTerrain == IntPtr.Zero)
				return;

			if(matId == System.Guid.Empty)
				return;

            unsafe
            {
                DllImportAPI.vTerrain_AddLayerMaterial(mTerrain, &matId);
            }
        }
        /// <summary>
        /// 删除相应ID的材质
        /// </summary>
        /// <param name="matId">材质ID</param>
		public void RemoveLayerMaterial(System.Guid matId)
        {
			if(mTerrain == IntPtr.Zero)
				return;

			if(matId == System.Guid.Empty)
				return;

            unsafe
            {
                DllImportAPI.vTerrain_RemoveLayerMaterial(mTerrain, &matId, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 重置层材质
        /// </summary>
        /// <param name="oldMatId">旧材质ID</param>
        /// <param name="newMtl">新的材质对象</param>
		public void ResetLayerMaterial(System.Guid oldMatId, CCore.Material.Material newMtl)
        {
			if(mTerrain == IntPtr.Zero)
				return;
			if(oldMatId == System.Guid.Empty)
				return;

            unsafe
            {
                DllImportAPI.vTerrain_ResetLayerMaterial(mTerrain, &oldMatId, newMtl.MaterialPtr, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 重置层的草对象
        /// </summary>
        /// <param name="matId">材质ID</param>
        /// <param name="grassData">草对象</param>
        public void ResetLayerGrass(System.Guid matId, CCore.Grass.GrassDataBase grassData)
        {
            if (mTerrain == IntPtr.Zero)
                return;
            if (matId == System.Guid.Empty)
                return;

            unsafe
            {
                DllImportAPI.vTerrain_ResetLayerGrass(mTerrain, &matId, grassData.GetInner(), Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 重置层标签
        /// </summary>
        /// <param name="matId">材质ID</param>
        /// <param name="remark">标记名称</param>
        public void ResetLayerRemarks(System.Guid matId, string remark)
        {
            if (mTerrain == IntPtr.Zero)
                return;
            if (matId == System.Guid.Empty)
                return;

            unsafe
            {
                DllImportAPI.vTerrain_ResetLayerRemarks(mTerrain, &matId, remark);
            }
        }
        /// <summary>
        /// 拾取材质
        /// </summary>
        /// <param name="x">x轴的坐标</param>
        /// <param name="z">z轴的坐标</param>
        /// <returns>返回材质ID</returns>
        public System.Guid PickMaterial(UInt32 x, UInt32 z)
        {
			Guid matId;

            unsafe
            {
                DllImportAPI.vTerrain_PickMaterial(mTerrain, &matId, x, z, Engine.Instance.GetFrameMillisecond());
                return matId;
            }
        }
        /// <summary>
        /// 刷新特效
        /// </summary>
        /// <param name="mtl">材质对象</param>
		public void RefreshEffect(CCore.Material.Material mtl)
        {
            unsafe
            {
                DllImportAPI.vTerrain_RefreshEffect(mTerrain, Engine.Instance.Client.Graphics.Device, mtl.MaterialPtr);
            }
        }

		// Patch添加与删除
        /// <summary>
        /// 根据坐标添加地形片
        /// </summary>
        /// <param name="x">x轴坐标</param>
        /// <param name="z">z轴坐标</param>
		public void AddPatchF(float x, float z)
        {
			UInt32 u = GetUWithX(x);
			UInt32 v = GetVWithZ(z);

			AddPatch(u, v);
        }
        /// <summary>
        /// 根据uv值添加地形片
        /// </summary>
        /// <param name="idu">u值</param>
        /// <param name="idv">v值</param>
		public void AddPatch(UInt32 idu, UInt32 idv)
        {
			UInt32 lvlX = GetLevelX(idu);
			UInt32 lvlZ = GetLevelZ(idv);
            CCore.Support.ServerAltitudeAssist.Instance.SetLevelDirty(lvlX, lvlZ);

            unsafe
            {
                DllImportAPI.vTerrain_AddPatch(mTerrain, idu, idv, Engine.Instance.GetFrameMillisecond());
            }
        }
        /// <summary>
        /// 删除相应坐标的地形片
        /// </summary>
        /// <param name="x">x轴坐标</param>
        /// <param name="z">z轴坐标</param>
		public void DelPatchF(float x, float z)
        {
			UInt32 u = GetUWithX(x);
			UInt32 v = GetVWithZ(z);

			DelPatch(u, v);
        }
        /// <summary>
        /// 删除相应uv值的地形片
        /// </summary>
        /// <param name="idu">u值</param>
        /// <param name="idv">v值</param>
		public void DelPatch(UInt32 idu, UInt32 idv)
        {
			UInt32 lvlX = GetLevelX(idu);
			UInt32 lvlZ = GetLevelZ(idv);
            CCore.Support.ServerAltitudeAssist.Instance.SetLevelDirty(lvlX, lvlZ);

            unsafe
            {
                DllImportAPI.vTerrain_DelPatch(mTerrain, idu, idv, Engine.Instance.GetFrameMillisecond());
            }
        }

		// idu,idv为level的在terrain索引
        /// <summary>
        /// 添加地形块
        /// </summary>
        /// <param name="idu">u值</param>
        /// <param name="idv">v值</param>
		public void AddLevel(UInt32 idu, UInt32 idv)
        {
            unsafe
            {
                DllImportAPI.vTerrain_AddLevel(mTerrain, idu, idv, Engine.Instance.GetFrameMillisecond());
            }
        }
        // idu,idv为level的在terrain索引
        /// <summary>
        /// 删除相应的地形块
        /// </summary>
        /// <param name="idu">u值</param>
        /// <param name="idv">v值</param>
        public void DelLevel(UInt32 idu, UInt32 idv)
        {
            unsafe
            {
                DllImportAPI.vTerrain_DelLevel(mTerrain, idu, idv, Engine.Instance.GetFrameMillisecond());
            }
        }

        // 取得Level有无的数据(true - Level存在，false - Level不存在)
        /// <summary>
        /// 获取地形块是否存在的数据，如果为true，则地形块存在，否则地形块不存在
        /// </summary>
        /// <returns>返回地形块是否存在的判断列表</returns>
        public List<System.Boolean> GetLevelData()
        {
            unsafe
            {
                List<System.Boolean> retData = new List<System.Boolean>();
                if (mTerrain == IntPtr.Zero)
                    return retData;

                TerrainInfo info = *((TerrainInfo*)(DllImportAPI.vTerrain_GetTerrainInfo(mTerrain)));

                for (UInt32 z = 0; z < info.LevelZ; z++)
                {
                    for (UInt32 x = 0; x < info.LevelX; x++)
                    {
                        if (DllImportAPI.vTerrain_GetLevel(mTerrain, (UInt16)x, (UInt16)z) == IntPtr.Zero)
                            retData.Add(true);
                        else
                            retData.Add(false);
                    }
                }

                return retData;
            }
        }
        /// <summary>
        /// 设置相邻地形的间隔
        /// </summary>
        /// <param name="value">间隔值</param>
        public void SetNeighborSide(uint value)
        {
            unsafe
            {
                DllImportAPI.vTerrain_SetNeighborSide(mTerrain, value, Engine.Instance.GetFrameMillisecond());
            }
        }
    }
}
