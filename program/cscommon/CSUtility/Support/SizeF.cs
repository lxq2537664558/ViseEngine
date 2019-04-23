using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.Support
{
    public struct SizeF
    {
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

        static SizeF mEmpty = new SizeF(0.0f, 0.0f);
        public static SizeF Empty
        {
            get { return mEmpty; }
        }

        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public static bool operator ==(SizeF left, SizeF right)
        {
            return ((left.Width == right.Width) && (left.Height == right.Height));
        }
        public static bool operator !=(SizeF left, SizeF right)
        {
            return ((left.Width != right.Width) || (left.Height != right.Height));
        }

        public override bool Equals(object obj)
        {
            var size = (SizeF)obj;
            return ((this.Width == size.Width) && (this.Height == size.Height));
        }

        public override int GetHashCode()
        {
            return (Width.ToString() + Height.ToString()).GetHashCode();
        }
    }
}
