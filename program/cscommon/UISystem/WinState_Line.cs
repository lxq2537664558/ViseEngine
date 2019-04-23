using System.ComponentModel;

namespace UISystem
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class WinState_Line : WinState
    {
        SlimDX.Vector2 mStart = SlimDX.Vector2.Zero;
        [Browsable(false)]
        public SlimDX.Vector2 Start
        {
            get { return mStart; }
            set { mStart = value; }
        }

        SlimDX.Vector2 mEnd = SlimDX.Vector2.UnitXY;
        [Browsable(false)]
        public SlimDX.Vector2 End
        {
            get { return mEnd; }
            set { mEnd = value; }
        }

        int mLineWidth = 1;
        [Browsable(false)]
        public int LineWidth
        {
            get{ return mLineWidth; }
            set{ mLineWidth = value; }
        }

        public WinState_Line(WinBase win)
            : base(win)
        {
            
        }

        public override void Draw(UIRenderPipe pipe, int zOrder, WinBase pWin, ref SlimDX.Vector4 backColor, ref SlimDX.Matrix parentMatrix)
        {
            WinRoot root = pWin.GetRoot() as WinRoot;
            if (root == null)
                return;

            IRender pRender = IRender.GetInstance();
            pRender.SetClipRect(pipe, pWin.ClipRect);

            CSUtility.Support.Rectangle dst = pWin.AbsRect;
            var tempStart = new SlimDX.Vector2((int)(Start.X * pWin.Width + dst.Left), (int)(Start.Y * pWin.Height + dst.Top));
            var tempEnd = new SlimDX.Vector2((int)(End.X * pWin.Width + dst.Left), (int)(End.Y * pWin.Height + dst.Top));

            var opacity = pWin.Opacity;
            if (UVAnim != null)
                opacity = pWin.Opacity * UVAnim.Opacity;

            pRender.DrawLine(pipe, zOrder, root.Width, root.Height, tempStart, tempEnd, LineWidth, UVAnim, opacity, backColor, parentMatrix);
        }
    }
}
