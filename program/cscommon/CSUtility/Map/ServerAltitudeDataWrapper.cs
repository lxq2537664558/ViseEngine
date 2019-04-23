using System;

namespace CSUtility.ServerMap
{
    public class ServerAltitudeInfo : CSUtility.Support.XndSaveLoadProxy
	{
		private UInt32 mLevelLengthX;		// 每个Level X方向上的长度
		private UInt32 mLevelLengthZ;		// 每个Level Z方向上的长度
		private UInt32 mXMaxLevelCount;	// X方向最大Level数量
		private UInt32 mZMaxLevelCount;	// Z方向最大Level数量
		private UInt32 mXValidLevelCount;	// X方向有效Level数量
		private UInt32 mZValidLevelCount;	// Z方向有效Level数量
		private float mMeterPerPixelX;	// 一格代表多少米
		private float mMeterPerPixelZ;	// 一格代表多少米

		public void ResetDefault()
		{
			mLevelLengthX = 1024;
			mLevelLengthZ = 1024;
			mXMaxLevelCount = 256;
			mZMaxLevelCount = 256;
			mXValidLevelCount = 1;
			mZValidLevelCount = 1;
			mMeterPerPixelX = 0.5f;
			mMeterPerPixelZ = 0.5f;
		}
		public float GetPixelXMeterLength(){
			return mMeterPerPixelX;
		}
		public float GetPixelZMeterLength(){
			return mMeterPerPixelZ;
		}
		public float GetLevelXMeterLength(){
			return mMeterPerPixelX * mLevelLengthX;
		}
		public float GetLevelZMeterLength(){
			return mMeterPerPixelZ * mLevelLengthZ;
		}
		public UInt32 GetMaxXPixelLength(){
			return mLevelLengthX * mXMaxLevelCount;
		}
		public UInt32 GetMaxZPixelLength(){
			return mLevelLengthZ * mZMaxLevelCount;
		}
		public UInt32 GetValidXPixelLength(){
			return mLevelLengthX * mXValidLevelCount;
		}
		public UInt32 GetValidZPixelLength(){
			return mLevelLengthZ * mZValidLevelCount;
		}

		[CSUtility.Support.AutoSaveLoadAttribute]
		public UInt32 LevelLengthX
        {
            get{return mLevelLengthX;}
            set { mLevelLengthX = value; }
        }
		[CSUtility.Support.AutoSaveLoadAttribute]
		public UInt32 LevelLengthZ
        {
            get{return mLevelLengthZ;}
            set { mLevelLengthZ = value; }
        }
		[CSUtility.Support.AutoSaveLoadAttribute]
		public UInt32 XMaxLevelCount
        {
            get{return mXMaxLevelCount;}
            set { mXMaxLevelCount = value; }
        }
        [CSUtility.Support.AutoSaveLoadAttribute]
		public UInt32 ZMaxLevelCount
        {
            get{return mZMaxLevelCount;}
            set { mZMaxLevelCount = value; }
        }
        [CSUtility.Support.AutoSaveLoadAttribute]
		public UInt32 XValidLevelCount
        {
            get{return mXValidLevelCount;}
            set { mXValidLevelCount = value; }
        }
        [CSUtility.Support.AutoSaveLoadAttribute]
		public UInt32 ZValidLevelCount
        {
            get{return mZValidLevelCount;}
            set { mZValidLevelCount = value; }
        }
        [CSUtility.Support.AutoSaveLoadAttribute]
        public float MeterPerPixelX
        {
            get { return mMeterPerPixelX; }
            set { mMeterPerPixelX = value; }
        }
        [CSUtility.Support.AutoSaveLoadAttribute]
		public float MeterPerPixelZ
        {
            get { return mMeterPerPixelZ; }
            set { mMeterPerPixelZ = value; }
        }
	}

    public class ServerAltitudeLevelData
	{
        enum EModifyState
        {
            MS_Geom = (1 << 0),
            MS_Norm = (1 << 1),
            MS_Blends = (1 << 2),
            MS_Material = (1 << 3),
            MS_NeedSave = (1 << 4),
            MS_Saving = (1 << 5),
            MS_SceneObject = (1 << 6),
            MS_ServerSaving = (1 << 7),
            MS_NeedServerSave_NPC = (1 << 8),
            MS_NeedServerSave_Trigger = (1 << 9),
        }

		private float[] mHeightArray;
		
		private UInt32 mXIndex;
		private UInt32 mZIndex;
		private UInt32 mLengthX;
		private UInt32 mLengthZ;
		private UInt32 mModifyState;

		public UInt32 XIndex
        {
            get { return mXIndex; }
        }
		public UInt32 ZIndex
        {
            get { return mZIndex; }
        }

		~ServerAltitudeLevelData()
        {
            Clear();
        }

		public void Initialize(UInt32 x, UInt32 z, UInt32 lengthX, UInt32 lengthZ)
        {
            Clear();

		    mXIndex = x;
		    mZIndex = z;
		    mLengthX = lengthX;
		    mLengthZ = lengthZ;

		    mHeightArray = new float[mLengthX * mLengthZ];
        }

		public void Clear()
        {
            mHeightArray = null;
		    mModifyState = 0;
        }

        public bool IsDirty()
        {
            return ((mModifyState & ((UInt32)EModifyState.MS_NeedSave)) == (UInt32)EModifyState.MS_NeedSave);
        }
        public void SetIsDirty(bool bDirty)
        {
            if (bDirty)
                SetModifyState((UInt32)EModifyState.MS_NeedSave);
            else
                RemoveModifyState((UInt32)EModifyState.MS_NeedSave);
        }
        public void SetModifyState(UInt32 state)
        {
            mModifyState = (mModifyState | state);
        }
        public void RemoveModifyState(UInt32 state)
        {
            mModifyState = (mModifyState & (~state));
        }
        public bool IsSaving()
        {
            return (mModifyState & (UInt32)EModifyState.MS_Saving) != 0 ? true : false;
        }

		public bool SetHeight(UInt32 x, UInt32 z, float height)
        {
            if (mHeightArray == null)
                return false;

            var idx = z * mLengthX + x;
		    if(idx >= (UInt32)(mHeightArray.Length))
			    return false;

		    mHeightArray[idx] = height;

		    SetIsDirty(true);

		    return true;
        }
        public float GetHeight(UInt32 x, UInt32 z)
        {
            if (mHeightArray == null)
                return 0;

            var idx = z * mLengthX + x;
		    if(idx >= (UInt32)(mHeightArray.Length))
			    return 0;

		    return mHeightArray[idx];
        }

		// 文件相对于Release路径
        public bool LoadLevel(System.String fileName)
        {
		    Clear();

		    CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.LoadXND(fileName);
		    if(holder == null)
			    return false;

		    var attr = holder.Node.FindAttrib("Header");
		    if(attr != null)
		    {
			    attr.BeginRead();
			    attr.Read(out mLengthX);
                attr.Read(out mLengthZ);
			    attr.EndRead();
		    }

		    attr = holder.Node.FindAttrib("Data");
		    if(attr != null)
		    {
			    attr.BeginRead();
			    UInt32 size = mLengthX * mLengthZ;
			    mHeightArray = new float[size];
			    for(UInt32 i=0; i<size; i++)
			    {
				    float val;
				    attr.Read(out val);
				    mHeightArray[i] = val;
			    }
			    attr.EndRead();
		    }

		    return true;
	    }
		// 文件相对于Release路径
        public bool SaveLevel(System.String fileName)
        {
		    if(IsSaving())
			    return false;

		    SetModifyState((UInt32)EModifyState.MS_Saving);

		    CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.NewXNDHolder();

		    var attr = holder.Node.AddAttrib("Header");
		    if(attr != null)
		    {
			    attr.BeginWrite();
			    attr.Write(mLengthX);
			    attr.Write(mLengthZ);
			    attr.EndWrite();
		    }

		    attr = holder.Node.AddAttrib("Data");
		    if(attr != null)
		    {
			    attr.BeginWrite();
			    for(int i=0; i<mHeightArray.Length; i++)
			    {
				    attr.Write(mHeightArray[i]);
			    }
			    attr.EndWrite();
		    }

		    CSUtility.Support.XndHolder.SaveXND(fileName, holder);

		    RemoveModifyState((UInt32)EModifyState.MS_Saving);

		    return true;
	    }
	}

    public class ServerAltitudeDataWrapper
	{
		protected ServerAltitudeInfo mMapInfo;
		protected ServerAltitudeLevelData[] mLevelArray;

		public ServerAltitudeInfo MapInfo
        {
            get{ return mMapInfo; }
        }

		~ServerAltitudeDataWrapper()
        {
            Cleanup();
        }
		
		public void Cleanup()
        {
            if(mLevelArray != null)
		    {
			    foreach (var level in mLevelArray)
			    {
				    if(level != null)
					    level.Clear();
			    }
			    mLevelArray = null;
		    }
        }

		public void Initialize(ServerAltitudeInfo mapInfo)
        {
		    mMapInfo = mapInfo;
            mLevelArray = new ServerAltitudeLevelData[mapInfo.XValidLevelCount * mapInfo.ZValidLevelCount];
	    }

		// 路径为绝对路径
        public bool Load(System.String name, System.String path)
        {
		    Cleanup();

		    System.String relativePath = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(path);

		    CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.LoadXND(relativePath + name);
            if (holder == null)
                return false;

		    CSUtility.Support.XndAttrib attr = holder.Node.FindAttrib("Header");
		    if(attr != null)
		    {
			    if(mMapInfo == null)
				    mMapInfo = new ServerAltitudeInfo();

			    attr.BeginRead();
			    //attr.Read(mMapInfo);
                mMapInfo.Read(attr);
			    attr.EndRead();

			    
                Initialize(mMapInfo);

			    for(UInt32 z = 0; z < mMapInfo.ZValidLevelCount; z++)
			    {
				    for(UInt32 x = 0; x < mMapInfo.XValidLevelCount; x++)
				    {
					    System.String levelFileName = x + "_" + z + ".SHLevel";

					    ServerAltitudeLevelData data = new ServerAltitudeLevelData();
					    data.Initialize(x, z, mMapInfo.LevelLengthX, mMapInfo.LevelLengthZ);
					    data.LoadLevel(relativePath + levelFileName);

					    mLevelArray[z * mMapInfo.XValidLevelCount + x] = data;
				    }
			    }

			    return true;
		    }

		    return false;
	    }

        public ServerAltitudeLevelData GetLevelData(UInt32 tX, UInt32 tZ, bool createWhenNotExist)
        {
		    if(tX >= mMapInfo.XValidLevelCount || tZ >= mMapInfo.ZValidLevelCount)
			    return null;

		    var idx = tZ * mMapInfo.XValidLevelCount + tX;
		    ServerAltitudeLevelData levelData = mLevelArray[idx];

		    if(levelData == null && createWhenNotExist)
		    {
			    levelData = new ServerAltitudeLevelData();
			    levelData.Initialize(tX, tZ, mMapInfo.LevelLengthX, mMapInfo.LevelLengthZ);
			    levelData.SetIsDirty(true);
			    mLevelArray[idx] = levelData;
		    }

		    return levelData;
	    }
        public ServerAltitudeLevelData[] GetLevelDatas()
		{
			return mLevelArray;
		}

        public float GetAltitude(float x, float z)
        {
            if (mMapInfo == null || mLevelArray == null)
                return 0;

            if (x < 0 || z < 0)
                return 0;

            if (mMapInfo.LevelLengthX == 0 || mMapInfo.LevelLengthZ == 0)
                return 0;
            if (mMapInfo.MeterPerPixelX == 0 || mMapInfo.MeterPerPixelZ == 0)
                return 0;

            UInt32 uX = (UInt32)(x / mMapInfo.MeterPerPixelX);
            UInt32 uZ = (UInt32)(z / mMapInfo.MeterPerPixelZ);

            UInt32 lX = uX / mMapInfo.LevelLengthX;
            UInt32 lZ = uZ / mMapInfo.LevelLengthZ;

            var idx = lZ * mMapInfo.XValidLevelCount + lX;
            if ((int)idx >= mLevelArray.Length)
                return 0;

            ServerAltitudeLevelData  levelData = mLevelArray[idx];
            if (levelData == null)
                return 0;

            UInt32 tX = uX % mMapInfo.LevelLengthX;
            UInt32 tZ = uZ % mMapInfo.LevelLengthZ;
            tZ = mMapInfo.LevelLengthZ - tZ - 1;
            return levelData.GetHeight(tX, tZ);
        }
	};
}
