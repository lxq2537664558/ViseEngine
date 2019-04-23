using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.Support
{
    public struct RectangleF
    {
        public float Left
        {
            get { return X; }
        }
        public float Top
        {
            get { return Y; }
        }
        public float Right
        {
            get { return X + Width; }
        }
        public float Bottom
        {
            get { return Y + Height; }
        }

        public bool IsEmpty
        {
            get
            {
                if (Width == 0 && Height == 0)
                    return true;

                return false;
            }
        }

        public SizeF Size
        {
            get { return new SizeF(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }
        public PointF Location
        {
            get { return new PointF(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

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

        public float Width
        {
            get;
            set;
        }

        public float Height
        {
            get;
            set;
        }

        static RectangleF mEmpty = new RectangleF(0, 0, 0, 0);
        public static RectangleF Empty
        {
            get { return mEmpty; }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(PointF pt, SizeF size)
        {
            X = pt.X;
            Y = pt.Y;
            Width = size.Width;
            Height = size.Height;
        }
        public bool Contains(PointF pt)
        {
            if ((pt.X >= X) && (pt.Y >= Y) && (pt.X < Right) && (pt.Y < Bottom))
                return true;

            return false;
        }
        public bool Contains(float x, float y)
        {
            if ((x >= X) && (y >= Y) && (x < Right) && (y < Bottom))
                return true;

            return false;
        }

        public static RectangleF Intersect( RectangleF a, RectangleF b )
        {
            if (a.Left > b.Right || a.Top > b.Bottom || a.Right < b.Left || a.Bottom < b.Top)
                return RectangleF.Empty;

            var left = System.Math.Max(a.Left, b.Left);
            var top = System.Math.Max(a.Top, b.Top);
            var right = System.Math.Min(a.Right, b.Right);
            var bottom = System.Math.Min(a.Bottom, b.Bottom);

            return new RectangleF(left, top, right - left, bottom - top);
        }
    }
}
