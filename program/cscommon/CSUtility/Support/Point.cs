using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.Support
{
    public struct Point
    {
        public bool IsEmpty
        {
            get
            {
                if (X == 0 && Y == 0)
                    return true;

                return false;
            }
        }

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        static Point mEmpty = new Point(0, 0);
        public static Point Empty
        {
            get { return mEmpty; }
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
