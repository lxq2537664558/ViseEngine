using System.ComponentModel;

namespace UISystem.Container.Grid
{
    public enum GridUnitType
    {
        /// <summary> 
        /// 自动计算大小
        /// </summary> 
        Auto = 0,
        /// <summary>
        /// 像素大小 
        /// </summary>
        Pixel = 1,
        /// <summary>
        /// 根据权重来调整大小 
        /// </summary>
        Star = 2,
    }

    public class GridLength : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        GridUnitType mGridUnitType = GridUnitType.Star;
        public GridUnitType GridUnitType
        {
            get { return mGridUnitType; }
            set
            {
                mGridUnitType = value;
                OnPropertyChanged("GridUnitType");
            }
        }

        public bool IsAbsolute
        {
            get { return mGridUnitType == GridUnitType.Pixel; }
        }

        public bool IsAuto
        {
            get { return mGridUnitType == GridUnitType.Auto; }
        }

        public bool IsStar
        {
            get { return mGridUnitType == GridUnitType.Star; }
        }

        public float Value { get { return ((mGridUnitType == GridUnitType.Auto) ? 1.0f : Length); } }

        float mLength = 0;
        public float Length
        {
            get { return mLength; }
            set
            {
                mLength = value;
                OnPropertyChanged("Length");
            }
        }

        float mMaxLength = float.PositiveInfinity;
        public float MaxLength
        {
            get { return mMaxLength; }
            set
            {
                mMaxLength = value;
                OnPropertyChanged("MaxLength");
            }
        }

        float mMinLength = 0;
        public float MinLength
        {
            get { return mMinLength; }
            set
            {
                mMinLength = value;
                OnPropertyChanged("MinLength");
            }
        }

        //float mActualLength = 0;
        //public float ActualLength
        //{
        //    get { return mActualLength; }
        //    set
        //    {
        //        mActualLength = value;
        //        OnPropertyChanged("ActualLength");
        //    }
        //}

        UISystem.Container.Grid.Grid mParent = null;
        public UISystem.Container.Grid.Grid Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }



        public GridLength()
        {
            mLength = 1;
            mGridUnitType = GridUnitType.Star;
        }

        public GridLength(float length)
        {
            mLength = length;
        }

        public GridLength(float length, GridUnitType unitType)
        {
            mLength = length;
            mGridUnitType = unitType;
        }

        public override int GetHashCode()
        {
            return (int)Length + (int)mGridUnitType;
        }

        public static bool operator ==(GridLength gl1, GridLength gl2)
        {
            try
            {
                return gl1.GridUnitType == gl2.GridUnitType && gl1.Value == gl2.Value;
            }
            catch (System.Exception) { }

            return false;
        }

        public static bool operator !=(GridLength gl1, GridLength gl2)
        {
            try
            {
                return gl1.GridUnitType != gl2.GridUnitType || gl1.Value != gl2.Value;
            }
            catch (System.Exception) { }

            return true;
        }

        public override bool Equals(object obj)
        {
            //if (obj == null)
            //    return false;

            //if (!(obj is GridLength))
            //    return false;

            //GridLength gObj = obj as GridLength;
            //if (mLength == gObj.Length &&
            //   mGridUnitType == gObj.GridUnitType)
            //    return true;

            //return false;
            if (obj is GridLength)
            {
                GridLength l = (GridLength)obj;
                return (this == l);
            }
            else
                return false;
        }

        public bool Equals(GridLength gridLength)
        {
            return this == gridLength;
        }

        public static GridLength CopyFrom(GridLength length)
        {
            return new GridLength(length.Length, length.GridUnitType);
        }

        public static GridLength Parse(string strValue)
        {
            if (strValue == "Auto")
                return new GridLength(1, GridUnitType.Auto);
            if (strValue.Contains("*"))
            {
                float length = System.Convert.ToSingle(strValue.Replace("*", ""));
                return new GridLength(length, GridUnitType.Star);
            }

            try
            {
                float length = System.Convert.ToSingle(strValue);
                return new GridLength(length, GridUnitType.Pixel);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }

        public void ParseStr(string strValue)
        {
            if (strValue == "Auto")
            {
                Length = 1;
                GridUnitType = GridUnitType.Auto;
            }
            else if (strValue.Contains("*"))
            {
                Length = System.Convert.ToSingle(strValue.Replace("*", ""));
                GridUnitType = GridUnitType.Star;
            }
            else
            {
                Length = System.Convert.ToSingle(strValue);
                GridUnitType = GridUnitType.Pixel;
            }
        }

        public override string ToString()
        {
            switch (mGridUnitType)
            {
                case GridUnitType.Star:
                    return mLength + "*";
                case GridUnitType.Pixel:
                    return mLength.ToString();
                case GridUnitType.Auto:
                    return "Auto";
            }

            return base.ToString();
        }
    }
}
