using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.Support
{
    public struct PointF
    {
        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        static PointF mEmpty = new PointF(0.0f, 0.0f);
        public static PointF Empty
        {
            get { return mEmpty; }
        }

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
