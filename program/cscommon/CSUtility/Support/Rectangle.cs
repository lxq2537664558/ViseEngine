using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.Support
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        public int Left
        {
            get { return X; }
        }
        public int Top
        {
            get { return Y; }
        }
        public int Right
        {
            get { return X + Width; }
        }
        public int Bottom
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

        public bool Equals(Rectangle rhs)
        {
            return (Left == rhs.Left && Top == rhs.Top && Width == rhs.Width && Height == rhs.Height);
        }
        public override bool Equals(object o)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return (Left + Top + Width + Height).GetHashCode();
        }
        
        public static bool operator == (Rectangle lhs, Rectangle rhs)
        {
            return (lhs.Left==rhs.Left && lhs.Top==rhs.Top && lhs.Width==rhs.Width && lhs.Height==rhs.Height);
        }

        public static bool operator !=(Rectangle lhs, Rectangle rhs)
        {
            return !(lhs.Left == rhs.Left && lhs.Top == rhs.Top && lhs.Width == rhs.Width && lhs.Height == rhs.Height);
        }

        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }
        public Point Location
        {
            get { return new Point(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
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

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }
        static Rectangle mEmpty = new Rectangle(0, 0, 0, 0);
        public static Rectangle Empty
        {
            get { return mEmpty; }
        }

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Point pt, Size size)
        {
            X = pt.X;
            Y = pt.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public bool Contains(Point pt)
        {
            if ((pt.X >= X) && (pt.Y >= Y) && (pt.X < Right) && (pt.Y < Bottom))
                return true;

            return false;
        }
        public bool Contains(int x, int y)
        {
            if ((x >= X) && (y >= Y) && (x < Right) && (y < Bottom))
                return true;

            return false;
        }

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            if (a.Left > b.Right || a.Top > b.Bottom || a.Right < b.Left || a.Bottom < b.Top)
                return Rectangle.Empty;

            var left = System.Math.Max(a.Left, b.Left);
            var top = System.Math.Max(a.Top, b.Top);
            var right = System.Math.Min(a.Right, b.Right);
            var bottom = System.Math.Min(a.Bottom, b.Bottom);

            return new Rectangle(left, top, right - left, bottom - top);
        }
    }
}
