using System;
using System.ComponentModel;

namespace UISystem.RichText
{
    public abstract class BaseObj
    {
        int mWidth = 0;					    // 像素宽度	
	    [Browsable(false)]
        public int Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }
        
        int mHeight = 0;					// 像素高度
        [Browsable(false)]
        public int Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

        int mPenX;					    // 画笔位置
        [Browsable(false)]
        public int PenX
        {
            get { return mPenX; }
            set { mPenX = value; }
        }

        int mPenY;
        [Browsable(false)]
        public int PenY
        {
            get { return mPenY; }
            set { mPenY = value; }
        }

        //int mBeginPos;				    // 在字符串中的起始位置
        //[Browsable(false)]
        //public int BeginPos
        //{
        //    get { return mBeginPos; }
        //    set { mBeginPos = value; }
        //}

        // textObj是字符串长度，imgObj是1
        int mLength = 1;
        [Browsable(false)]
        public int Length
        {
            get { return mLength; }
            set { mLength = value; }
        }

        Int32 mMaxTopLine = 0;
        [Browsable(false)]
        public Int32 MaxTopLine
        {
            get { return mMaxTopLine; }
            set
            {
                mMaxTopLine = value;
            }
        }

        Int32 mMaxBottomLine = 0;
        [Browsable(false)]
        public Int32 MaxBottomLine
        {
            get { return mMaxBottomLine; }
            set
            {
                mMaxBottomLine = value;
            }
        }

        // 渲染函数，依赖于渲染引擎
        abstract public void Draw(UIRenderPipe pipe, int zOrder);
        abstract public bool PointCheck(int x, int y, ref int outCursorX, ref int outCursorY, ref int outCursorHeight, ref int outCurPos, ref FragmentObj pObj);
    }
}
