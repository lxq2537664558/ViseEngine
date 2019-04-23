namespace UISystem.Container
{
    [CSUtility.Editor.UIEditor_Control("容器.Canvas")]
    public class Canvas : Container
    {
        protected override void OnChildDesiredSizeChanged(WinBase child)
        {

        }

        protected override SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            availableSize.Width = float.MaxValue;
            availableSize.Height = float.MaxValue;
            for (int i = 0; i < ChildWindows.Count;++i )
            {
                var child = ChildWindows[i];
                child.Measure(availableSize);
            }

            SlimDX.Size returnDesiredSize = new SlimDX.Size();
            if (!Width_Auto)
            {
                returnDesiredSize.Width = System.Math.Max(Width, 0);
            }
            if (!Height_Auto)
            {
                returnDesiredSize.Height = System.Math.Max(Height, 0);
            }

            return returnDesiredSize;
        }

        protected override SlimDX.Size ArrangeOverride(SlimDX.Size finalSize)
        {
            for (int i = 0; i < ChildWindows.Count; ++i)
            {
                var child = ChildWindows[i];
                child.Arrange(new SlimDX.Rect(0, 0, float.MaxValue, float.MaxValue));
            }

            return finalSize;
        }

        //protected override void InitializeBehaviorProcesses()
        //{
        //    RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, Canvas_OnMouseMove, WinBase.enRoutingStrategy.Bubble);
        //    RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, Container_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
        //    RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, Container_OnMouseLeftButtonUp, WinBase.enRoutingStrategy.Bubble);
        //}

        //private void Canvas_OnMouseMove(CCore.MsgProc.BehaviorParameter init, UISystem.Message.RoutedEventArgs eventArgs)
        //{
        //    var mm = init as CCore.MsgProc.Behavior.Mouse_Move;
        //    if (mDraging)
        //    {
        //        CSUtility.Support.Point ptMouse = ((WinBase)Parent).AbsToLocal(mm.X, mm.Y);

        //        foreach (var child in ChildWindows)
        //        {
        //            ptMouse.Y = ptMouse.Y - mDragLocation.Y + child.AbsRect.Y;
        //            ptMouse.X = child.AbsRect.X;
        //            if (child.Height > Height)
        //            {
        //                if (ptMouse.Y > 0)
        //                    ptMouse.Y = 0;
        //                else if (ptMouse.Y + child.Height < Height)
        //                    ptMouse.Y = Height - child.Height;
        //            }

        //            child.MoveWin(ref ptMouse);
        //        }
        //        //MoveWin(ref ptMouse);

        //        _FWinDraging(ref ptMouse, this);
        //    }

        //    var arg = new Message.MouseEventArgs(mm.Clicks, mm.X, mm.Y, mm.button);
        //    _FWinMouseMove(this, arg);
        //    eventArgs.Handled = arg.Handled;
        //}
    }
}
