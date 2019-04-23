using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.Support
{
    public struct Size
    {
        public bool IsEmpty
        {
            get
            {
                if (Width == 0 && Height == 0)
                    return true;

                return false;
            }
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

        static Size mEmpty = new Size(0, 0);
        public static Size Empty
        {
            get { return mEmpty; }
        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            var size = (Size)obj;
            return ((this.Width == size.Width) && (this.Height == size.Height));
        }

        public override int GetHashCode()
        {
            return (Width.ToString() + Height.ToString()).GetHashCode();
        }

        public static bool operator == (Size left, Size right)
        {
            return ((left.Width == right.Width) && (left.Height == right.Height));
        }
        public static bool operator !=(Size left, Size right)
        {
            return ((left.Width != right.Width) || (left.Height != right.Height));
        }
    }
}
