using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace UIEditor
{
    public partial class DrawPanel
    {
        bool mMouseDown = false;
        protected Point mMouseLeftButtonDownPt;
        protected Point mMouseRightButtonDownPt;
        bool mMoveSelectControls = false;
        bool mMouseNotMove = true;
        private void RectCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                mMouseLeftButtonDownPt = e.GetPosition(RectCanvas);
                mMouseNotMove = true;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Mouse.Capture(sender as UIElement, CaptureMode.Element);
                mMouseRightButtonDownPt = e.GetPosition(ViewBoxMain);
            }
            mMouseDown = true;
        }

        private void RectCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            var rtPt = e.GetPosition(Image_RT);
            if(!mMoveSelectControls)
                UpdatePreSelectRect(rtPt);

            if(e.LeftButton == MouseButtonState.Pressed && mMouseDown)
            {
                var pt = e.GetPosition(RectCanvas);
                var length = System.Math.Sqrt((pt.X - mMouseLeftButtonDownPt.X) * (pt.X - mMouseLeftButtonDownPt.X) +
                                              (pt.Y - mMouseLeftButtonDownPt.Y) * (pt.Y - mMouseLeftButtonDownPt.Y));
                if (length < 2)
                    mMouseNotMove = true;
                else
                    mMouseNotMove = false;
            }
            else if (e.RightButton == MouseButtonState.Pressed && mMouseDown)
            {
                Point pt = e.GetPosition(RectCanvas);
                Point newPos = new Point(pt.X - mMouseRightButtonDownPt.X, pt.Y - mMouseRightButtonDownPt.Y);

                Canvas.SetLeft(ViewBoxMain, newPos.X);
                Canvas.SetTop(ViewBoxMain, newPos.Y);

                // 微量改变容器的大小以便自动调用OnRenderSizeChanged来重新计算容器内部连线位置
                //this.Width = this.ActualWidth + 0.00001;
            }
        }

        private void RectCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            mMouseDown = false;

            if(e.ChangedButton == MouseButton.Left && mMouseNotMove)
            {
                // 选择控件
                var pt = e.GetPosition(Image_RT);

                bool mMultiSelect = false;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    mMultiSelect = true;

                var selCtrl = CheckSelectUI(new CSUtility.Support.Point((int)(pt.X), (int)(pt.Y)), mWinRootForm);
                if (selCtrl == null && !mMultiSelect)
                {
                    SelectedWinControls.Clear();
                }
                else if(!(selCtrl is UISystem.WinRoot))
                {
                    if(!mMultiSelect)
                        SelectedWinControls.Clear();
                    var availWin = WinBase.GetAvailableUIWin(selCtrl);
                    SelectedWinControls.Add(WinBase.GetHostWin(availWin));
                }
            }
        }

        private void RectCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            double delta = 1 + e.Delta / 1000.0;

            Point center = e.GetPosition(RectCanvas);//new Point(RectCanvas.ActualWidth / 2, RectCanvas.ActualHeight / 2);
                                                     //Point center = new Point(RectCanvas.ActualWidth / 2, RectCanvas.ActualHeight / 2);
                                                     //center = RectCanvas.TranslatePoint(center, ViewBoxMain);
                                                     //ViewboxScaleTransform.CenterX = center.X;
                                                     //ViewboxScaleTransform.CenterY = center.Y;

            Point deltaXY = new Point(center.X * delta, center.Y * delta);

            double left = Canvas.GetLeft(ViewBoxMain);
            double top = Canvas.GetTop(ViewBoxMain);

            if (Double.IsNaN(left))
                left = 0;
            if (Double.IsNaN(top))
                top = 0;

            left = (left / center.X) * deltaXY.X + center.X - deltaXY.X;
            top = (top / center.Y) * deltaXY.Y + center.Y - deltaXY.Y;

            Canvas.SetLeft(ViewBoxMain, left);
            Canvas.SetTop(ViewBoxMain, top);

            ViewBoxMain.Width = ViewBoxMain.ActualWidth * delta;
            
            InitializeGridAssist(true);
        }

        private void ViewBoxMain_LayoutUpdated(object sender, EventArgs e)
        {
            var left = Canvas.GetLeft(ViewBoxMain);
            var top = Canvas.GetTop(ViewBoxMain);
            if(!double.IsNaN(left))
                Canvas.SetLeft(Rect_Screen, left);
            if(!double.IsNaN(top))
                Canvas.SetTop(Rect_Screen, top);
            if(!double.IsNaN(ViewBoxMain.Width))
            {
                Rect_Screen.Width = ViewBoxMain.Width;
                Rect_Screen.Height = ViewBoxMain.Width * mWindowsRect.Height / mWindowsRect.Width;
                TextBlock_ScaleInfo.Text = "缩放比例: " + (int)(GetScaleDelta() * 100) + "%";

                UpdateSelectionContainer();
                if (mWinMousePointAtControl != null)
                    SetPreSelectRect(mWinMousePointAtControl.AbsRect);
            }
        }

        double mAnimTagLeft = 0, mAnimTagTop = 0, mAnimTagWidth = 1;
        private void FocusRect(double x, double y, double width, double height)
        {
            var delta = 50;
            var curLeft = x - delta;
            var curTop = y - delta;
            var curRight = x + width + delta;
            var curBottom = y + height + delta;

            var tl = MainDrawCanvas.TranslatePoint(new Point(curLeft, curTop), ViewBoxMain);
            var rb = MainDrawCanvas.TranslatePoint(new Point(curRight, curBottom), ViewBoxMain);
            var tagVBMPt = new Point(tl.X + (rb.X - tl.X) * 0.5, tl.Y + (rb.Y - tl.Y) * 0.5);

            var viewStart = RectCanvas.TranslatePoint(new Point(0, 0), ViewBoxMain);
            var viewEnd = RectCanvas.TranslatePoint(new Point(RectCanvas.ActualWidth, RectCanvas.ActualHeight), ViewBoxMain);
            var curVBMPt = new Point(viewStart.X + (viewEnd.X - viewStart.X) * 0.5, viewStart.Y + (viewEnd.Y - viewStart.Y) * 0.5);

            //var scaleDeltaX = (viewEnd.X - viewStart.X) / (curRight - curLeft);
            //var scaleDeltaY = (viewEnd.Y - viewStart.Y) / (curBottom - curTop);
            var scaleDeltaX = (viewEnd.X - viewStart.X) / (rb.X - tl.X);
            var scaleDeltaY = (viewEnd.Y - viewStart.Y) / (rb.Y - tl.Y);
            var scaleDelta = Math.Min(scaleDeltaX, scaleDeltaY);
            mAnimTagLeft = Canvas.GetLeft(ViewBoxMain) + (curVBMPt.X - tagVBMPt.X);
            mAnimTagTop = Canvas.GetTop(ViewBoxMain) + (curVBMPt.Y - tagVBMPt.Y);

            var deltaX = tagVBMPt.X / ViewBoxMain.ActualWidth;
            var deltaY = tagVBMPt.Y / ViewBoxMain.ActualHeight;
            mAnimTagLeft -= (ViewBoxMain.ActualWidth * scaleDelta - ViewBoxMain.ActualWidth) * deltaX;
            mAnimTagTop -= (ViewBoxMain.ActualHeight * scaleDelta - ViewBoxMain.ActualHeight) * deltaY;

            var transAnim = new DoubleAnimation();
            transAnim.To = mAnimTagLeft;
            transAnim.Duration = TimeSpan.FromSeconds(0.15);
            transAnim.AccelerationRatio = 0.3;
            transAnim.DecelerationRatio = 0.3;
            transAnim.Completed += TransAnimX_Completed;
            ViewBoxMain.BeginAnimation(Canvas.LeftProperty, transAnim);

            transAnim = new DoubleAnimation();
            transAnim.To = mAnimTagTop;
            transAnim.Duration = TimeSpan.FromSeconds(0.15);
            transAnim.AccelerationRatio = 0.3;
            transAnim.DecelerationRatio = 0.3;
            transAnim.Completed += TransAnimY_Completed;
            ViewBoxMain.BeginAnimation(Canvas.TopProperty, transAnim);

            ViewBoxMain.Width = ViewBoxMain.ActualWidth;
            mAnimTagWidth = scaleDelta * ViewBoxMain.ActualWidth;
            //ViewBoxMain.Width = mAnimTagWidth;
            var anim = new DoubleAnimation();
            anim.To = mAnimTagWidth;
            anim.Duration = TimeSpan.FromSeconds(0.15);
            anim.AccelerationRatio = 0.3;
            anim.DecelerationRatio = 0.3;
            anim.Completed += ScaleAnim_Completed;
            ViewBoxMain.BeginAnimation(Viewbox.WidthProperty, anim);
        }

        private void ScaleAnim_Completed(object sender, EventArgs e)
        {
            ViewBoxMain.BeginAnimation(Viewbox.WidthProperty, null);
            ViewBoxMain.Width = mAnimTagWidth;
        }

        private void TransAnimX_Completed(object sender, EventArgs e)
        {
            ViewBoxMain.BeginAnimation(Canvas.LeftProperty, null);
            Canvas.SetLeft(ViewBoxMain, mAnimTagLeft);
        }
        private void TransAnimY_Completed(object sender, EventArgs e)
        {
            ViewBoxMain.BeginAnimation(Canvas.TopProperty, null);
            Canvas.SetTop(ViewBoxMain, mAnimTagTop);
        }

        private void Button_OHP_Click(object sender, RoutedEventArgs e)
        {
            // 100%比例
            Canvas.SetLeft(ViewBoxMain, 0);
            Canvas.SetTop(ViewBoxMain, 0);
            ViewBoxMain.Width = mWindowsRect.Width;
        }

        private void Button_ShowAll_Click(object sender, RoutedEventArgs e)
        {
            FocusRect(0, 0, mWindowsRect.Width, mWindowsRect.Height);
        }

        private void Button_FocusSelect_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWinControls.Count <= 0)
                return;

            //mRenderScaleX = 1;
            //mRenderScaleY = 1;
            //var left = FormHost.ActualWidth / 2 - (mControlContainerRect.Width / 2 + mControlContainerRect.X);
            //var top = FormHost.ActualHeight / 2 - (mControlContainerRect.Height / 2 + mControlContainerRect.Y);
            //mWinUIRoot.Margin = new CSUtility.Support.Thickness(left, top, 0, 0);
        }

        #region DragDrop

        private bool CheckDropAvailable(DragEventArgs e)
        {
            if (UIEditor.Program.ControlDragType.Equals(EditorCommon.DragDrop.DragDropManager.Instance.DragType))
                return true;

            return false;
        }

        EditorCommon.DragDrop.DropAdorner mDropAdorner;
        private void RectCanvas_DragEnter(object sender, DragEventArgs e)
        {
            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(RectCanvas);

            var pos = e.GetPosition(RectCanvas);
            if (pos.X > 0 && pos.X < RectCanvas.ActualWidth &&
               pos.Y > 0 && pos.Y < RectCanvas.ActualHeight)
            {
                var layer = AdornerLayer.GetAdornerLayer(RectCanvas);
                layer.Add(mDropAdorner);
            }

            mDropAdorner.IsAllowDrop = CheckDropAvailable(e);
        }
        private void RectCanvas_DragLeave(object sender, DragEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(RectCanvas);
            layer.Remove(mDropAdorner);
            mWinCreatedParentControl = null;
        }
        private void RectCanvas_DragOver(object sender, DragEventArgs e)
        {
            if(CheckDropAvailable(e))
            {
                var formats = e.Data.GetFormats();
                if (formats == null || formats.Length == 0)
                    return;

                var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                if (datas == null)
                    return;

                UIControlViewModel createdVM = null;
                foreach (var data in datas)
                {
                    var dragedItem = data as UIControlViewModel;
                    if (dragedItem == null)
                        continue;

                    createdVM = dragedItem;
                    break;
                }
                if (createdVM == null)
                    return;

                var pos = e.GetPosition(Image_RT);
                var mousePAC = CheckSelectUI(new CSUtility.Support.Point((int)(pos.X), (int)(pos.Y)), mWinRootForm);
                if(mousePAC != null)
                {
                    var parentCtrl = mousePAC;
                    while(parentCtrl != null)
                    {
                        if (parentCtrl.CanInsertChild())
                            break;

                        parentCtrl = parentCtrl.Parent as UISystem.WinBase;
                    }
                    mWinCreatedParentControl = parentCtrl;
                    SetPreSelectRect(mWinCreatedParentControl.AbsRect);
                    e.Effects = DragDropEffects.Copy;
                    EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "在" + mWinCreatedParentControl.NameInEditor + "中创建" + createdVM.ControlName;

                    return;
                }
            }

            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "不允许拖放到窗口外";
            e.Effects = DragDropEffects.None;
        }
        private void RectCanvas_Drop(object sender, DragEventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(RectCanvas);
            layer.Remove(mDropAdorner);

            if (CheckDropAvailable(e))
            {
                // 节点拖放
                var formats = e.Data.GetFormats();
                if (formats == null || formats.Length == 0)
                    return;

                var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                if (datas == null)
                    return;

                var pos = e.GetPosition(Image_RT);

                if(mWinCreatedParentControl == null)
                    mWinCreatedParentControl = WinRootForm;
                SelectedWinControls.Clear();
                foreach (var data in datas)
                {
                    var dragedItem = data as UIControlViewModel;
                    if (dragedItem == null)
                        continue;

                    // todo: Template创建处理
                    var uiControl = dragedItem.Control.Assembly.CreateInstance(dragedItem.Control.FullName) as UISystem.WinBase;
                    uiControl.OnPropertyChangedEvent += OnWinControlPropertyChanged;
                    uiControl.Width = 100;
                    uiControl.Height = 80;
                    uiControl.Parent = mWinCreatedParentControl;
                    var movePt = new CSUtility.Support.Point((int)(pos.X - mWinCreatedParentControl.AbsRect.Left), 
                                                             (int)(pos.Y - mWinCreatedParentControl.AbsRect.Top));
                    uiControl.MoveWin(ref movePt);
                    SelectedWinControls.Add(WinBase.GetHostWin(uiControl));
                    //uiControl.UpdateLayout();
                }

                //// 用于更新layout
                //mWinRoot.Tick(CCore.Engine.Instance.GetFrameMillisecond());
                //UpdateUIControlRectRestores(SelectedWinControls);
                UpdateSelectionContainer();
            }
        }

        #endregion  // DragDrop

        #region 选择框处理

        // 更新Margin辅助线
        private void UpdateMarginAssist()
        {
            if (SelectedWinControls.Count == 1)
            {
                var win = SelectedWinControls[0];
                if (win.UIWin.Parent != null)
                {
                    var delta = GetScaleDelta();

                    var winAbsRect = win.UIWin.AbsRect;
                    var parentAbsRect = ((UISystem.WinBase)(win.UIWin.Parent)).AbsRect;
                    Path_Top.Visibility = Visibility.Collapsed;
                    Path_Left.Visibility = Visibility.Collapsed;
                    Path_Bottom.Visibility = Visibility.Collapsed;
                    Path_Right.Visibility = Visibility.Collapsed;

                    switch (win.UIWin.HorizontalAlignment)
                    {
                        case UISystem.UI.HorizontalAlignment.Left:
                            {
                                var tempVal = (winAbsRect.X - parentAbsRect.X) * delta;
                                if (tempVal > 0)
                                {
                                    Path_Left.Visibility = Visibility.Visible;
                                    Path_Left.Width = tempVal;
                                    Path_Left.Margin = new Thickness(-tempVal, 0, 0, 0);
                                }
                                else
                                {
                                    Path_Left.Visibility = Visibility.Collapsed;
                                }
                            }
                            break;
                        case UISystem.UI.HorizontalAlignment.Right:
                            {
                                var tempVal = (parentAbsRect.Right - winAbsRect.Right) * delta;
                                if (tempVal > 0)
                                {
                                    Path_Right.Visibility = Visibility.Visible;
                                    Path_Right.Width = tempVal;
                                    Path_Right.Margin = new Thickness(0, 0, -tempVal, 0);
                                }
                                else
                                {
                                    Path_Right.Visibility = Visibility.Collapsed;
                                }
                            }
                            break;
                        case UISystem.UI.HorizontalAlignment.Stretch:
                            {
                                var tempX = (winAbsRect.X - parentAbsRect.X) * delta;
                                if (tempX > 0)
                                {
                                    Path_Left.Visibility = Visibility.Visible;
                                    Path_Left.Width = tempX;
                                    Path_Left.Margin = new Thickness(-tempX, 0, 0, 0);
                                }
                                else
                                    Path_Left.Visibility = Visibility.Collapsed;

                                var tempY = (parentAbsRect.Right - winAbsRect.Right) * delta;
                                if (tempY > 0)
                                {
                                    Path_Right.Visibility = Visibility.Visible;
                                    Path_Right.Width = tempY;
                                    Path_Right.Margin = new Thickness(0, 0, -tempY, 0);
                                }
                                else
                                {
                                    Path_Right.Visibility = Visibility.Collapsed;
                                }
                            }
                            break;
                    }

                    switch (win.UIWin.VerticalAlignment)
                    {
                        case UISystem.UI.VerticalAlignment.Top:
                            {
                                var tempValTop = (winAbsRect.Top - parentAbsRect.Top) * delta;
                                if (tempValTop > 0)
                                {
                                    Path_Top.Visibility = Visibility.Visible;
                                    Path_Top.Height = tempValTop;
                                    Path_Top.Margin = new Thickness(0, -tempValTop, 0, 0);
                                }
                                else
                                    Path_Top.Visibility = Visibility.Collapsed;
                            }
                            break;
                        case UISystem.UI.VerticalAlignment.Bottom:
                            {
                                var temValBottom = (parentAbsRect.Bottom - winAbsRect.Bottom) * delta;
                                if (temValBottom > 0)
                                {
                                    Path_Bottom.Visibility = Visibility.Visible;
                                    Path_Bottom.Height = temValBottom;
                                    Path_Bottom.Margin = new Thickness(0, 0, 0, -temValBottom);
                                }
                                else
                                    Path_Bottom.Visibility = Visibility.Collapsed;
                            }
                            break;
                        case UISystem.UI.VerticalAlignment.Stretch:
                            {
                                var tempValTop = (winAbsRect.Top - parentAbsRect.Top) * delta;
                                if (tempValTop > 0)
                                {
                                    Path_Top.Visibility = Visibility.Visible;
                                    Path_Top.Height = tempValTop;
                                    Path_Top.Margin = new Thickness(0, -tempValTop, 0, 0);
                                }
                                else
                                    Path_Top.Visibility = Visibility.Collapsed;

                                var temValBottom = (parentAbsRect.Bottom - winAbsRect.Bottom) * delta;
                                if (temValBottom > 0)
                                {
                                    Path_Bottom.Visibility = Visibility.Visible;
                                    Path_Bottom.Height = temValBottom;
                                    Path_Bottom.Margin = new Thickness(0, 0, 0, -temValBottom);
                                }
                                else
                                    Path_Bottom.Visibility = Visibility.Collapsed;
                            }
                            break;
                    }

                    return;
                }
            }

            Path_Top.Visibility = Visibility.Collapsed;
            Path_Bottom.Visibility = Visibility.Collapsed;
            Path_Left.Visibility = Visibility.Collapsed;
            Path_Right.Visibility = Visibility.Collapsed;

        }

        // 更新选择框
        private void UpdateSelectionContainer()
        {
            if(!mMoveSelectControls)
            {
                if(SelectedWinControls.Count == 0)
                    Grid_ControlContainer.Visibility = Visibility.Collapsed;
                else
                {
                    var rect = GetSelectionControlsBoundRect(SelectedWinControls);
                    Grid_ControlContainer.Visibility = Visibility.Visible;
                    var pt = Image_RT.TranslatePoint(new Point(rect.X, rect.Y), RectCanvas);
                    Canvas.SetLeft(Grid_ControlContainer, pt.X);
                    Canvas.SetTop(Grid_ControlContainer, pt.Y);
                    var delta = GetScaleDelta();
                    Grid_ControlContainer.Width = rect.Width * delta < 0 ? 0 : rect.Width * delta;
                    Grid_ControlContainer.Height = rect.Height * delta < 0 ? 0 : rect.Height * delta;

                    UpdateMarginAssist();
                    InitializeGridAssist();
                }
            }
        }

        private void UpdatePreSelectRect(Point pt_ImageRT)
        {
            mWinMousePointAtControl = CheckSelectUI(new CSUtility.Support.Point((int)(pt_ImageRT.X), (int)(pt_ImageRT.Y)), mWinRootForm);

            if (mWinMousePointAtControl != null)
            {
                SetPreSelectRect(mWinMousePointAtControl.AbsRect);
            }
            else
            {
                SetPreSelectRect(CSUtility.Support.Rectangle.Empty);
            }
        }

        private void SetPreSelectRect(CSUtility.Support.Rectangle rect_ImageRT)
        {
            if(rect_ImageRT.IsEmpty)
            {
                var storyboard = TryFindResource("Storyboard_PreSelect_Show") as Storyboard;
                storyboard?.Stop();
                storyboard = TryFindResource("Storyboard_PreSelect_Hide") as Storyboard;
                storyboard?.Begin();
            }
            else
            {
                var pt = Image_RT.TranslatePoint(new Point(rect_ImageRT.X, rect_ImageRT.Y), RectCanvas);

                Canvas.SetLeft(Rect_PreSelect, pt.X);
                Canvas.SetTop(Rect_PreSelect, pt.Y);

                var delta = GetScaleDelta();
                Rect_PreSelect.Width = rect_ImageRT.Width * delta < 0 ? 0 : rect_ImageRT.Width * delta;
                Rect_PreSelect.Height = rect_ImageRT.Height * delta < 0 ? 0 : rect_ImageRT.Height * delta;

                var storyboard = TryFindResource("Storyboard_PreSelect_Hide") as Storyboard;
                storyboard?.Stop();
                storyboard = TryFindResource("Storyboard_PreSelect_Show") as Storyboard;
                storyboard?.Begin();
            }
        }

        Visibility mControlContainerVisible = Visibility.Collapsed;
        public Visibility ControlContainerVisible
        {
            get { return mControlContainerVisible; }
            set
            {
                mControlContainerVisible = value;
                OnPropertyChanged("ControlContainerVisible");
            }
        }

        private void UpdateUIControlRectRestores(ObservableCollection<UIEditor.WinBase> selection)
        {
            mSelectionControlRectRestores.Clear();

            if (selection == null)
                return;

            foreach (var ctrl in selection)
            {
                var loc = GetAbsoluteLocation(ctrl.UIWin);
                var boundRect = new System.Drawing.Rectangle(loc.X, loc.Y, ctrl.UIWin.Width, ctrl.UIWin.Height);
                mSelectionControlRectRestores.Add(boundRect);
            }
        }

        private void SnapCalculate(double left, double top, bool calLeft, bool calTop, bool calRight, bool calBottom, out double outLeft, out double outTop)
        {
            outLeft = left;
            outTop = top;

            if (SnapEnable)
            {
                var delta = GetScaleDelta();

                var selCtrl = new UISystem.WinBase[SelectedWinControls.Count];
                for (int i = 0; i < selCtrl.Length; i++)
                {
                    selCtrl[i] = SelectedWinControls[i].UIWin;
                }

                var transPt = RectCanvas.TranslatePoint(new Point(left, top), Image_RT);

                double snapPosX = 0;
                UISystem.WinBase nearestXCtrl = null;
                int leftOutValue = int.MaxValue;
                var nearestLeftControl = WinRootForm.GetNearestX((int)transPt.X, ref leftOutValue, mSnapDistance, selCtrl);
                int rightOutValue = int.MaxValue;
                var nearestRightControl = WinRootForm.GetNearestX((int)(transPt.X + Grid_ControlContainer.Width / delta), ref rightOutValue, mSnapDistance, selCtrl);
                if (nearestLeftControl != null || nearestRightControl != null)
                {
                    if(calLeft && calRight)
                    {
                        var tempLeftPt = Image_RT.TranslatePoint(new Point(leftOutValue, 0), RectCanvas);
                        var leftOffset = System.Math.Abs(left - tempLeftPt.X);
                        var tempRightPt = Image_RT.TranslatePoint(new Point(rightOutValue, 0), RectCanvas);
                        var rightOffset = System.Math.Abs(left + Grid_ControlContainer.Width - tempRightPt.X);
                        if (leftOffset <= rightOffset && leftOffset < mSnapDistance)
                        {
                            outLeft = tempLeftPt.X;
                            snapPosX = outLeft;
                            nearestXCtrl = nearestLeftControl;
                        }
                        else if (rightOffset < leftOffset && rightOffset < mSnapDistance)
                        {
                            outLeft = tempRightPt.X - Grid_ControlContainer.Width;
                            snapPosX = tempRightPt.X;
                            nearestXCtrl = nearestRightControl;
                        }
                    }
                    else if(calLeft)
                    {
                        var tempLeftPt = Image_RT.TranslatePoint(new Point(leftOutValue, 0), RectCanvas);
                        var leftOffset = System.Math.Abs(left - tempLeftPt.X);
                        if(leftOffset < mSnapDistance)
                        {
                            outLeft = tempLeftPt.X;
                            snapPosX = outLeft;
                            nearestXCtrl = nearestLeftControl;
                        }
                    }
                    else if(calRight)
                    {
                        var tempRightPt = Image_RT.TranslatePoint(new Point(rightOutValue, 0), RectCanvas);
                        var rightOffset = System.Math.Abs(left + Grid_ControlContainer.Width - tempRightPt.X);
                        if (rightOffset < mSnapDistance)
                        {
                            outLeft = tempRightPt.X - Grid_ControlContainer.Width;
                            snapPosX = tempRightPt.X;
                            nearestXCtrl = nearestRightControl;
                        }
                    }
                }

                double snapPosY = 0;
                UISystem.WinBase nearestYCtrl = null;
                int topOutValue = int.MaxValue;
                var nearestTopControl = WinRootForm.GetNearestY((int)transPt.Y, ref topOutValue, mSnapDistance, selCtrl);
                int bottomOutValue = int.MaxValue;
                var nearestBottomControl = WinRootForm.GetNearestY((int)(transPt.Y + Grid_ControlContainer.Height / delta), ref bottomOutValue, mSnapDistance, selCtrl);
                if (nearestTopControl == null && nearestBottomControl == null)
                {
                    Line_SnapY.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if(calTop && calBottom)
                    {
                        var tempTopPt = Image_RT.TranslatePoint(new Point(0, topOutValue), RectCanvas);
                        var topOffset = System.Math.Abs(top - tempTopPt.Y);
                        var tempBottomPt = Image_RT.TranslatePoint(new Point(0, bottomOutValue), RectCanvas);
                        var bottomOffset = System.Math.Abs(top + Grid_ControlContainer.Height - tempBottomPt.Y);
                        if (topOffset <= bottomOffset && topOffset < mSnapDistance)
                        {
                            outTop = tempTopPt.Y;
                            snapPosY = outTop;
                            nearestYCtrl = nearestTopControl;
                        }
                        else if (bottomOffset < topOffset && bottomOffset < mSnapDistance)
                        {
                            outTop = tempBottomPt.Y - Grid_ControlContainer.Height;
                            snapPosY = tempBottomPt.Y;
                            nearestYCtrl = nearestBottomControl;
                        }
                    }
                    else if(calTop)
                    {
                        var tempTopPt = Image_RT.TranslatePoint(new Point(0, topOutValue), RectCanvas);
                        var topOffset = System.Math.Abs(top - tempTopPt.Y);
                        if (topOffset < mSnapDistance)
                        {
                            outTop = tempTopPt.Y;
                            snapPosY = outTop;
                            nearestYCtrl = nearestTopControl;
                        }
                    }
                    else if(calBottom)
                    {
                        var tempBottomPt = Image_RT.TranslatePoint(new Point(0, bottomOutValue), RectCanvas);
                        var bottomOffset = System.Math.Abs(top + Grid_ControlContainer.Height - tempBottomPt.Y);
                        if (bottomOffset < mSnapDistance)
                        {
                            outTop = tempBottomPt.Y - Grid_ControlContainer.Height;
                            snapPosY = tempBottomPt.Y;
                            nearestYCtrl = nearestBottomControl;
                        }
                    }
                }

                if (nearestXCtrl != null)
                {
                    Line_SnapX.Visibility = Visibility.Visible;
                    Point ctrlLT = new Point(nearestXCtrl.AbsRect.X, nearestXCtrl.AbsRect.Y);
                    ctrlLT = Image_RT.TranslatePoint(ctrlLT, RectCanvas);
                    Line_SnapX.X1 = snapPosX;
                    Line_SnapX.X2 = snapPosX;
                    Line_SnapX.Y1 = System.Math.Min(outTop, ctrlLT.Y);
                    Line_SnapX.Y2 = System.Math.Max(outTop + Grid_ControlContainer.Height, ctrlLT.Y + nearestXCtrl.AbsRect.Height * delta);
                }
                else
                {
                    Line_SnapX.Visibility = Visibility.Collapsed;
                }
                if (nearestYCtrl != null)
                {
                    Line_SnapY.Visibility = Visibility.Visible;
                    Point ctrlLT = new Point(nearestYCtrl.AbsRect.X, nearestYCtrl.AbsRect.Y);
                    ctrlLT = Image_RT.TranslatePoint(ctrlLT, RectCanvas);
                    Line_SnapY.Y1 = snapPosY;
                    Line_SnapY.Y2 = snapPosY;
                    Line_SnapY.X1 = System.Math.Min(outLeft, ctrlLT.X);
                    Line_SnapY.X2 = System.Math.Max(outLeft + Grid_ControlContainer.Width, ctrlLT.X + nearestYCtrl.AbsRect.Width * delta);
                }
                else
                {
                    Line_SnapY.Visibility = Visibility.Collapsed;
                }
            }
        }

        bool mSelectContainerMouseDown = false;
        Point mSelectContainerMouseDownPosition;
        CSUtility.Support.Rectangle mControlContainerOldRect;
        private void Rect_SelectContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var elm = sender as IInputElement;
            mSelectContainerMouseDownPosition = e.GetPosition(elm);
            Mouse.Capture(elm);
            mSelectContainerMouseDown = true;

            var left = Canvas.GetLeft(Grid_ControlContainer);
            var top = Canvas.GetTop(Grid_ControlContainer);
            mControlContainerOldRect = new CSUtility.Support.Rectangle();
            var delta = GetScaleDelta();
            var rectPt = new Point(left, top);
            rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
            mControlContainerOldRect.X = (int)(rectPt.X);
            mControlContainerOldRect.Y = (int)(rectPt.Y);
            mControlContainerOldRect.Width = (int)(Grid_ControlContainer.Width * delta);
            mControlContainerOldRect.Height = (int)(Grid_ControlContainer.Height * delta);
            
            UpdateUIControlRectRestores(SelectedWinControls);
        }
        private void Rect_SelectContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mSelectContainerMouseDown = false;
            mMoveSelectControls = false;
            //Rect_Snap.Visibility = Visibility.Collapsed;
            Line_SnapX.Visibility = Visibility.Collapsed;
            Line_SnapY.Visibility = Visibility.Collapsed;
            Mouse.Capture(null);

            UpdateUIControlRectRestores(SelectedWinControls);
        }

        private void Rect_Center_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);
                
                left += pt.X - mSelectContainerMouseDownPosition.X;
                top += pt.Y - mSelectContainerMouseDownPosition.Y;
                var delta = GetScaleDelta();

                // 吸附计算
                SnapCalculate(left, top, true, true, true, true, out left, out top);

                if (!double.IsNaN(left))
                    Canvas.SetLeft(Grid_ControlContainer, left);
                if(!double.IsNaN(top))
                    Canvas.SetTop(Grid_ControlContainer, top);

                var tagRect = new CSUtility.Support.Rectangle();
                var rectPt = new Point(left, top);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);

                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }
        private void Rect_LT_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);
                
                var deltaX = pt.X - mSelectContainerMouseDownPosition.X;
                var deltaY = pt.Y - mSelectContainerMouseDownPosition.Y;

                // 吸附计算
                double outLeft, outTop;
                SnapCalculate(left + deltaX, top + deltaY, true, true, false, false, out outLeft, out outTop);
                deltaX = outLeft - left;
                deltaY = outTop - top;

                if(!double.IsNaN(outLeft))
                {
                    Canvas.SetLeft(Grid_ControlContainer, outLeft);
                    Grid_ControlContainer.Width -= deltaX;
                }
                if (!double.IsNaN(outTop))
                {
                    Canvas.SetTop(Grid_ControlContainer, outTop);
                    Grid_ControlContainer.Height -= deltaY;
                }

                var tagRect = new CSUtility.Support.Rectangle();
                var delta = GetScaleDelta();
                var rectPt = new Point(outLeft, outTop);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);
                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }
        private void Rect_L_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);

                var deltaX = pt.X - mSelectContainerMouseDownPosition.X;

                // 吸附计算
                double outLeft, outTop;
                SnapCalculate(left + deltaX, top, true, false, false, false, out outLeft, out outTop);
                deltaX = outLeft - left;

                if(!double.IsNaN(left))
                {
                    Canvas.SetLeft(Grid_ControlContainer, outLeft);
                    Grid_ControlContainer.Width -= deltaX;
                }

                var tagRect = new CSUtility.Support.Rectangle();
                var delta = GetScaleDelta();
                var rectPt = new Point(outLeft, top);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);
                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }
        private void Rect_LB_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);
                
                var deltaX = pt.X - mSelectContainerMouseDownPosition.X;
                var deltaY = pt.Y - mSelectContainerMouseDownPosition.Y;

                double outLeft, outTop;
                SnapCalculate(left + deltaX, top + deltaY, true, false, false, true, out outLeft, out outTop);
                deltaX = outLeft - left;
                deltaY = outTop - top;

                if (!double.IsNaN(outLeft))
                {
                    Canvas.SetLeft(Grid_ControlContainer, outLeft);
                    Grid_ControlContainer.Width -= deltaX;
                }
                Grid_ControlContainer.Height += deltaY;

                var tagRect = new CSUtility.Support.Rectangle();
                var delta = GetScaleDelta();
                var rectPt = new Point(outLeft, top);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);
                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }
        private void Rect_B_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);

                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var deltaY = pt.Y - mSelectContainerMouseDownPosition.Y;

                double outLeft, outTop;
                SnapCalculate(left, top + deltaY, false, false, false, true, out outLeft, out outTop);
                deltaY = outTop - top;

                Grid_ControlContainer.Height += deltaY;

                var tagRect = new CSUtility.Support.Rectangle();
                var delta = GetScaleDelta();
                var rectPt = new Point(left, top);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);
                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }
        private void Rect_RB_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);

                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var deltaX = pt.X - mSelectContainerMouseDownPosition.X;
                var deltaY = pt.Y - mSelectContainerMouseDownPosition.Y;

                double outLeft, outTop;
                SnapCalculate(left + deltaX, top + deltaY, false, false, true, true, out outLeft, out outTop);
                deltaX = outLeft - left;
                deltaY = outTop - top;

                Grid_ControlContainer.Width += deltaX;
                Grid_ControlContainer.Height += deltaY;

                var delta = GetScaleDelta();
                var tagRect = new CSUtility.Support.Rectangle();
                var rectPt = new Point(left, top);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);
                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }
        private void Rect_R_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);

                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var deltaX = pt.X - mSelectContainerMouseDownPosition.X;

                double outLeft, outTop;
                SnapCalculate(left + deltaX, top, false, false, true, false, out outLeft, out outTop);
                deltaX = outLeft - left;

                Grid_ControlContainer.Width += deltaX;

                var delta = GetScaleDelta();
                var tagRect = new CSUtility.Support.Rectangle();
                var rectPt = new Point(left, top);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);
                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }
        private void Rect_RT_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);

                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var deltaX = pt.X - mSelectContainerMouseDownPosition.X;
                var deltaY = pt.Y - mSelectContainerMouseDownPosition.Y;

                double outLeft, outTop;
                SnapCalculate(left + deltaX, top + deltaY, false, true, true, false, out outLeft, out outTop);
                deltaX = outLeft - left;
                deltaY = outTop - top;
                
                if (!double.IsNaN(outTop))
                {
                    Canvas.SetTop(Grid_ControlContainer, outTop);
                    Grid_ControlContainer.Height -= deltaY;
                }
                Grid_ControlContainer.Width += deltaX;

                var delta = GetScaleDelta();
                var tagRect = new CSUtility.Support.Rectangle();
                var rectPt = new Point(left, outTop);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);
                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }
        private void Rect_T_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && mSelectContainerMouseDown)
            {
                mMoveSelectControls = true;
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var left = Canvas.GetLeft(Grid_ControlContainer);
                var top = Canvas.GetTop(Grid_ControlContainer);
                var deltaY = pt.Y - mSelectContainerMouseDownPosition.Y;

                double outLeft, outTop;
                SnapCalculate(left, top + deltaY, false, true, false, false, out outLeft, out outTop);
                deltaY = outTop - top;
                
                if (!double.IsNaN(outTop))
                {
                    Canvas.SetTop(Grid_ControlContainer, outTop);
                    Grid_ControlContainer.Height -= deltaY;
                }

                var delta = GetScaleDelta();
                var tagRect = new CSUtility.Support.Rectangle();
                var rectPt = new Point(left, outTop);
                rectPt = RectCanvas.TranslatePoint(rectPt, Image_RT);
                tagRect.X = (int)(rectPt.X);
                tagRect.Y = (int)(rectPt.Y);
                tagRect.Width = (int)(Grid_ControlContainer.Width * delta);
                tagRect.Height = (int)(Grid_ControlContainer.Height * delta);
                UpdateSelectionControlTransWithRect(ref tagRect, ref mControlContainerOldRect);
                UpdateMarginAssist();
            }
        }

        #endregion  // 选择框处理

        private double GetScaleDelta()
        {
            return ViewBoxMain.Width / mWindowsRect.Width;
        }
    }
}
