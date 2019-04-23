using System;
using System.ComponentModel;

namespace CSUtility.Navigation
{
    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 4)]
    public struct NavigationInfo : System.IEquatable<NavigationInfo>
    {
        public UInt32 mLevelLengthX;		// 每个Level X方向上的长度
        public UInt32 mLevelLengthZ;		// 每个Level Z方向上的长度
        public UInt32 mXMaxLevelCount;	// X方向最大Level数量
        public UInt32 mZMaxLevelCount;	// Z方向最大Level数量
        public UInt32 mXValidLevelCount;	// X方向有效Level数量
        public UInt32 mZValidLevelCount;	// Z方向有效Level数量
        public float mMeterPerPixelX;	// 一格代表多少米
        public float mMeterPerPixelZ;	// 一格代表多少米

        //public INavigationInfo()
        //{
        //    mLevelLengthX = 1024;
        //    mLevelLengthZ = 1024;
        //    mXMaxLevelCount = 256;
        //    mZMaxLevelCount = 256;
        //    mXValidLevelCount = 1;
        //    mZValidLevelCount = 1;
        //    mMeterPerPixelX = 0.5f;
        //    mMeterPerPixelZ = 0.5f;
        //}

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

		public float GetPixelXMeterLength()
        {
			return mMeterPerPixelX;
		}
		public float GetPixelZMeterLength()
        {
			return mMeterPerPixelZ;
		}
		public float GetLevelXMeterLength()
        {
			return mMeterPerPixelX * mLevelLengthX;
		}
		public float GetLevelZMeterLength()
        {
			return mMeterPerPixelZ * mLevelLengthZ;
		}
		public UInt32 GetMaxXPixelLength()
        {
			return mLevelLengthX * mXMaxLevelCount;
		}
		public UInt32 GetMaxZPixelLength()
        {
			return mLevelLengthZ * mZMaxLevelCount;
		}
		public UInt32 GetValidXPixelLength()
        {
			return mLevelLengthX * mXValidLevelCount;
		}
		public UInt32 GetValidZPixelLength()
        {
			return mLevelLengthZ * mZValidLevelCount;
		}

        public override bool Equals(object obj)
        {
            var navInfo = (NavigationInfo)obj;

            if (mLevelLengthX == navInfo.mLevelLengthX &&
               mLevelLengthZ == navInfo.mLevelLengthZ &&
               mXMaxLevelCount == navInfo.mXMaxLevelCount &&
               mZMaxLevelCount == navInfo.mZMaxLevelCount &&
               mXValidLevelCount == navInfo.mXValidLevelCount &&
               mZValidLevelCount == navInfo.mZValidLevelCount &&
               mMeterPerPixelX == navInfo.mMeterPerPixelX &&
               mMeterPerPixelZ == navInfo.mMeterPerPixelZ)
                return true;

            return false;
        }

        public bool Equals(NavigationInfo navInfo)
        {
            return (mLevelLengthX == navInfo.mLevelLengthX &&
                    mLevelLengthZ == navInfo.mLevelLengthZ &&
                    mXMaxLevelCount == navInfo.mXMaxLevelCount &&
                    mZMaxLevelCount == navInfo.mZMaxLevelCount &&
                    mXValidLevelCount == navInfo.mXValidLevelCount &&
                    mZValidLevelCount == navInfo.mZValidLevelCount &&
                    mMeterPerPixelX == navInfo.mMeterPerPixelX &&
                    mMeterPerPixelZ == navInfo.mMeterPerPixelZ);
        }

        public static bool Equals(ref NavigationInfo value1, ref NavigationInfo value2)
        {
            return (value1.mLevelLengthX == value2.mLevelLengthX &&
                    value1.mLevelLengthZ == value2.mLevelLengthZ &&
                    value1.mXMaxLevelCount == value2.mXMaxLevelCount &&
                    value1.mZMaxLevelCount == value2.mZMaxLevelCount &&
                    value1.mXValidLevelCount == value2.mXValidLevelCount &&
                    value1.mZValidLevelCount == value2.mZValidLevelCount &&
                    value1.mMeterPerPixelX == value2.mMeterPerPixelX &&
                    value1.mMeterPerPixelZ == value2.mMeterPerPixelZ);
        }

        public static bool operator ==(NavigationInfo value1, NavigationInfo value2)
        {
            return value1.Equals(value2);
        }
        public static bool operator !=(NavigationInfo value1, NavigationInfo value2)
        {
            return !value1.Equals(value2);
        }

        public override int GetHashCode()
        {
            //return mLevelLengthX.GetHashCode() +
            //       mLevelLengthZ.GetHashCode() +
            //       mXMaxLevelCount.GetHashCode() +
            //       mZMaxLevelCount.GetHashCode() +
            //       mXValidLevelCount.GetHashCode() +
            //       mZValidLevelCount.GetHashCode() +
            //       mMeterPerPixelX.GetHashCode() +
            //       mMeterPerPixelZ.GetHashCode();
            var hashStr = mLevelLengthX.ToString() +
                          mLevelLengthZ.ToString() +
                          mXMaxLevelCount.ToString() +
                          mZMaxLevelCount.ToString() +
                          mXValidLevelCount.ToString() +
                          mZValidLevelCount.ToString() +
                          mMeterPerPixelX.ToString() +
                          mMeterPerPixelZ.ToString();
            return (int)(CSUtility.Support.UniHash.DefaultHash(hashStr));
        }
    }

    public class NavigationInfoOperator : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        NavigationInfo mOpInfo;
        [Browsable(false)]
        public NavigationInfo OpInfo
        {
            get { return mOpInfo; }
        }

        [CSUtility.Support.DataValueAttribute("LevelLengthX")]
        [DisplayName("寻路单个块格子数X")]
        public UInt32 LevelLengthX
        {
            get { return mOpInfo.mLevelLengthX; }
            set
            {
                mOpInfo.mLevelLengthX = value;
                UpdateSizeMeterX();
                OnPropertyChanged("LevelLengthX");
            }
        }

        [CSUtility.Support.DataValueAttribute("LevelLengthZ")]
        [DisplayName("寻路单个块格子数Z")]
        public UInt32 LevelLengthZ
        {
            get { return mOpInfo.mLevelLengthZ; }
            set
            {
                mOpInfo.mLevelLengthZ = value;
                UpdateSizeMeterZ();
                OnPropertyChanged("LevelLengthZ");
            }
        }

        [CSUtility.Support.DataValueAttribute("LevelCountX")]
        [DisplayName("寻路块数X")]
        public UInt32 LevelCountX
        {
            get { return mOpInfo.mXValidLevelCount; }
            set
            {
                mOpInfo.mXValidLevelCount = value;
                if (mOpInfo.mXMaxLevelCount < mOpInfo.mXValidLevelCount)
                    mOpInfo.mXMaxLevelCount = mOpInfo.mXValidLevelCount;
                UpdateSizeMeterX();
                OnPropertyChanged("LevelCountX");
            }
        }
        [CSUtility.Support.DataValueAttribute("LevelCountZ")]
        [DisplayName("寻路块数Z")]
        public UInt32 LevelCountZ
        {
            get { return mOpInfo.mZValidLevelCount; }
            set
            {
                mOpInfo.mZValidLevelCount = value;
                if (mOpInfo.mZMaxLevelCount < mOpInfo.mZValidLevelCount)
                    mOpInfo.mZMaxLevelCount = mOpInfo.mZValidLevelCount;
                UpdateSizeMeterZ();
                OnPropertyChanged("LevelCountZ");
            }
        }

        [CSUtility.Support.DataValueAttribute("MeterPerPixelX")]
        [DisplayName("每格子大小X（米）")]
        public float MeterPerPixelX
        {
            get { return mOpInfo.mMeterPerPixelX; }
            set
            {
                mOpInfo.mMeterPerPixelX = value;
                UpdateSizeMeterX();
                OnPropertyChanged("MeterPerPixelX");
            }
        }
        [CSUtility.Support.DataValueAttribute("MeterPerPixelZ")]
        [DisplayName("每格子大小Z（米）")]
        public float MeterPerPixelZ
        {
            get { return mOpInfo.mMeterPerPixelZ; }
            set
            {
                mOpInfo.mMeterPerPixelZ = value;
                UpdateSizeMeterZ();
                OnPropertyChanged("MeterPerPixelZ");
            }
        }

        float mMapSizeMeterX;
        [Description("自动计算，单位是米")]
        [DisplayName("寻路X大小")]
        public float MapSizeMeterX
        {
            get { return mMapSizeMeterX; }
        }
        float mMapSizeMeterZ;
        [Description("自动计算，单位是米")]
        [DisplayName("寻路Z大小")]
        public float MapSizeMeterZ
        {
            get { return mMapSizeMeterZ; }
        }

        public NavigationInfoOperator()
        {
            mOpInfo = new NavigationInfo();
            mOpInfo.ResetDefault();

            UpdateSizeMeterX();
            UpdateSizeMeterZ();
        }

        private void UpdateSizeMeterX()
        {
            mMapSizeMeterX = LevelLengthX * LevelCountX * MeterPerPixelX;
            OnPropertyChanged("MapSizeMeterX");
        }
        private void UpdateSizeMeterZ()
        {
            mMapSizeMeterZ = LevelLengthZ * LevelCountZ * MeterPerPixelZ;
            OnPropertyChanged("MapSizeMeterZ");
        }
    }
}
