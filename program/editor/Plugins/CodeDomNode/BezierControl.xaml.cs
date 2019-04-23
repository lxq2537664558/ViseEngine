using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for BezierControl.xaml
    /// </summary>
    [CodeGenerateSystem.ShowInNodeList("运算.曲线", "根据输入获取输出")]
    public partial class BezierControl : CodeGenerateSystem.Base.BaseNodeControl
    {
        double mYMax = 100;
        public double YMax
        {
            get { return mYMax; }
            set
            {
                mYMax = value;
                IsDirty = true;
                OnPropertyChanged("YMax");
            }
        }

        double mYMin = 0;
        public double YMin
        {
            get { return mYMin; }
            set
            {
                mYMin = value;
                IsDirty = true;
                OnPropertyChanged("YMin");
            }
        }

        double mXMax = 100;
        public double XMax
        {
            get { return mXMax; }
            set
            {
                mXMax = value;
                IsDirty = true;
                OnPropertyChanged("XMax");
            }
        }

        double mXMin = 0;
        public double XMin
        {
            get { return mXMin; }
            set
            {
                mXMin = value;
                IsDirty = true;
                OnPropertyChanged("XMin");
            }
        }

        bool mIsXLoop = true;
        public bool IsXLoop
        {
            get { return mIsXLoop; }
            set
            {
                mIsXLoop = value;
                IsDirty = true;
                OnPropertyChanged("IsXLoop");
            }
        }

        public BezierControl(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            SetDragObject(Rectangle_Title);
            NodeName = "曲线";

            // X输入
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.NumbericalValue, ValueInputHandle, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, ValueInputHandle.BackBrush, false);
            // Y输出
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.NumbericalValue, ValueOutputHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueOutputHandle.BackBrush, true);
            // Y轴最大值
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.NumbericalValue, ValueYMaxInputHandle, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, ValueYMaxInputHandle.BackBrush, false);
            // Y轴最小值
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.NumbericalValue, ValueYMinInputHandle, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, ValueYMinInputHandle.BackBrush, false);
            // X轴最小值
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.NumbericalValue, ValueXMinInputHandle, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.End, ValueXMinInputHandle.BackBrush, false);
            // X轴最大值
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.NumbericalValue, ValueXMaxInputHandle, CodeGenerateSystem.Base.enBezierType.Bottom, CodeGenerateSystem.Base.enLinkOpType.End, ValueXMaxInputHandle.BackBrush, false);

            LineXBezierCtrl.OnDirtyChanged = OnLineXBezierControlDirtyChanged;
        }

        private void OnLineXBezierControlDirtyChanged(bool bDirty)
        {
            if(bDirty)
                IsDirty = true;
        }

#region SaveLoad

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("YMax", YMax.ToString());
            xmlNode.AddAttrib("YMin", YMin.ToString());
            xmlNode.AddAttrib("XMax", XMax.ToString());
            xmlNode.AddAttrib("XMin", XMin.ToString());
            xmlNode.AddAttrib("IsXLoop", IsXLoop.ToString());
            var pointsNode = xmlNode.AddNode("BezierPoints", "", holder);
            LineXBezierCtrl.Save(pointsNode, holder);

            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            var att = xmlNode.FindAttrib("YMax");
            if (att != null)
                YMax = System.Convert.ToDouble(att.Value);

            att = xmlNode.FindAttrib("YMin");
            if (att != null)
                YMin = System.Convert.ToDouble(att.Value);

            att = xmlNode.FindAttrib("XMax");
            if (att != null)
                XMax = System.Convert.ToDouble(att.Value);

            att = xmlNode.FindAttrib("XMin");
            if (att != null)
                XMin = System.Convert.ToDouble(att.Value);

            att = xmlNode.FindAttrib("IsXLoop");
            if (att != null)
                IsXLoop = System.Convert.ToBoolean(att.Value);

            var pointsNode = xmlNode.FindNode("BezierPoints");
            if (pointsNode != null)
            {
                LineXBezierCtrl.Load(pointsNode);
            }

            base.Load(xmlNode, deltaX, deltaY);

            LineXBezierCtrl.UpdateShow();
        }

#endregion

//#region 添加删除点
//        private void AddBezierPoint(Point pt)
//        {
//            // 按顺序插入
//            int idx = 0;
//            foreach (var bp in mBezierPoints)
//            {
//                if (bp.Position.X > pt.X)
//                {
//                    var bPt = new BezierPoint();
//                    bPt.Position = pt;
//                    var tempPt = pt;
//                    tempPt.X += 15;
//                    if (tempPt.X > MainCanvas.ActualWidth)
//                        tempPt.X = MainCanvas.ActualWidth;
//                    bPt.ControlPoint = tempPt;
//                    mBezierPoints.Insert(idx, bPt);

//                    bPt = new BezierPoint();
//                    bPt.Position = pt;
//                    tempPt = pt;
//                    tempPt.X -= 15;
//                    if (tempPt.X < 0)
//                        tempPt.X = 0;
//                    bPt.ControlPoint = tempPt;
//                    mBezierPoints.Insert(idx, bPt);

//                    break;
//                }
//                idx++;
//            }

//            IsDirty = true;

//            UpdateShow();
//        }

//        private void RemoveBezierPoint(BezierPoint bPt)
//        {
//            var idx = mBezierPoints.IndexOf(bPt);
//            if (idx == 0 || idx == mBezierPoints.Count - 1 || idx < 0)
//                return;

//            mBezierPoints.RemoveRange(idx, 2);

//            IsDirty = true;

//            UpdateShow();
//        }
//#endregion

//#region 更新绘制

//        private void UpdateShow()
//        {
//            switch (ControlMode)
//            {
//                case enControlMode.Bezier:
//                    {
//                        UpdateKeyPoint(true);
//                        UpdateBezierShow();
//                    }
//                    break;
//                case enControlMode.LineXBezier:
//                    {
//                        UpdateKeyPoint(false);
//                        UpdateLineXBezierShow();
//                    }
//                    break;
//            }
//        }

//        // Y轴为贝塞尔曲线，X轴为线性
//        private void UpdateLineXBezierShow()
//        {
//            if (Polyline_LineXBezier.Visibility != Visibility.Visible)
//                return;

//            Polyline_LineXBezier.Points.Clear();

//            if (mBezierPoints.Count < 2)
//                return;

//            for (double x = 0; x < MainCanvas.Width; x += 1)
//            {
//                var bezierPos = CSUtility.Support.BezierCalculate.ValueOnBezier(mBezierPoints, x);
//                Polyline_LineXBezier.Points.Add(new System.Windows.Point(x, bezierPos.Y));
//            }

//            var posX = mBezierPoints[mBezierPoints.Count - 1].Position.X - 0.01;
//            var bPos = CSUtility.Support.BezierCalculate.ValueOnBezier(mBezierPoints, posX);
//            Polyline_LineXBezier.Points.Add(new System.Windows.Point(posX, bPos.Y));
//        }

//        private void UpdateBezierShow()
//        {
//            if (BezierPath.Visibility != Visibility.Visible)
//                return;

//            BezierPathFigure.Segments.Clear();

//            if(mBezierPoints.Count == 0)
//                return;

//            BezierPathFigure.StartPoint = mBezierPoints[0].Position;
//            for(int i=0; i<mBezierPoints.Count; i++)
//            {
//                if (i < mBezierPoints.Count - 1 && (i % 2 == 0))
//                {
//                    var seg = new BezierSegment();
//                    seg.Point1 = mBezierPoints[i].ControlPoint;
//                    seg.Point2 = mBezierPoints[i + 1].ControlPoint;
//                    seg.Point3 = mBezierPoints[i + 1].Position;
//                    BindingOperations.SetBinding(seg, BezierSegment.Point1Property, new Binding("ControlPoint") { Source = mBezierPoints[i] });
//                    BindingOperations.SetBinding(seg, BezierSegment.Point2Property, new Binding("ControlPoint") { Source = mBezierPoints[i + 1] });
//                    BindingOperations.SetBinding(seg, BezierSegment.Point3Property, new Binding("Position") { Source = mBezierPoints[i + 1] });
//                    BezierPathFigure.Segments.Add(seg);
//                }
//            }
//        }

//        private void UpdateKeyPoint(bool withControlPoint = false)
//        {
//            ControlLineGeoGroup.Children.Clear();

//            foreach (var ellipse in mBezierControlPtEllipses)
//            {
//                MainCanvas.Children.Remove(ellipse);
//            }
//            mBezierControlPtEllipses.Clear();
//            foreach (var ellipse in mBezierPtEllipses)
//            {
//                MainCanvas.Children.Remove(ellipse);
//            }
//            mBezierPtEllipses.Clear();

//            for (int i = 0; i < mBezierPoints.Count; i++)
//            {
//                if (withControlPoint)
//                {
//                    var pos = mBezierPoints[i].Position;
//                    var ctrlPos = mBezierPoints[i].ControlPoint;
//                    var lineGeo = new LineGeometry(pos, ctrlPos);
//                    if (i % 2 == 0 && i != 0)
//                        BindingOperations.SetBinding(lineGeo, LineGeometry.StartPointProperty, new Binding("Position") { Source = mBezierPoints[i - 1] });
//                    else
//                        BindingOperations.SetBinding(lineGeo, LineGeometry.StartPointProperty, new Binding("Position") { Source = mBezierPoints[i] });
//                    BindingOperations.SetBinding(lineGeo, LineGeometry.EndPointProperty, new Binding("ControlPoint") { Source = mBezierPoints[i] });
//                    ControlLineGeoGroup.Children.Add(lineGeo);

//                    var ctrlPtEl = new Ellipse()
//                    {
//                        Fill = this.FindResource("ControlPointColor") as Brush,
//                        Stroke = Brushes.Black,
//                        StrokeThickness = 1,
//                        Tag = mBezierPoints[i],
//                        Width = 8,
//                        Height = 8,
//                        RenderTransformOrigin = new Point(0.5, 0.5),
//                        RenderTransform = new TranslateTransform(-4, -4),
//                    };
//                    Canvas.SetLeft(ctrlPtEl, mBezierPoints[i].ControlPoint.X);
//                    Canvas.SetTop(ctrlPtEl, mBezierPoints[i].ControlPoint.Y);
//                    Canvas.SetZIndex(ctrlPtEl, 1);
//                    ((BezierPoint)mBezierPoints[i]).ControlPointEllipse = ctrlPtEl;
//                    mBezierControlPtEllipses.Add(ctrlPtEl);
//                    MainCanvas.Children.Add(ctrlPtEl);
//                    ctrlPtEl.MouseDown += ControlPointEllipse_MouseDown;
//                    ctrlPtEl.MouseMove += ControlPointEllipse_MouseMove;
//                    ctrlPtEl.MouseUp += ControlPointEllipse_MouseUp;
//                    ctrlPtEl.MouseEnter += ControlPointEllipse_MouseEnter;
//                    ctrlPtEl.MouseLeave += ControlPointEllipse_MouseLeave;
//                }

//                if ((i % 2 != 0) || i == 0 || (i == (mBezierPoints.Count - 1)))
//                {
//                    var ptEl = new Ellipse()
//                    {
//                        Fill = this.FindResource("BezierPointColor") as Brush,
//                        Stroke = Brushes.Black,
//                        StrokeThickness = 1,
//                        Tag = mBezierPoints[i],
//                        Width = 10,
//                        Height = 10,
//                        RenderTransformOrigin = new Point(0.5, 0.5),
//                        RenderTransform = new TranslateTransform(-5, -5)
//                    };
//                    Canvas.SetLeft(ptEl, mBezierPoints[i].Position.X);
//                    Canvas.SetTop(ptEl, mBezierPoints[i].Position.Y);
//                    ((BezierPoint)mBezierPoints[i]).PositionEllipse = ptEl;
//                    mBezierPtEllipses.Add(ptEl);
//                    MainCanvas.Children.Add(ptEl);
//                    ptEl.MouseDown += BezierPointEllipse_MouseDown;
//                    ptEl.MouseMove += BezierPointEllipse_MouseMove;
//                    ptEl.MouseUp += BezierPointEllipse_MouseUp;
//                    ptEl.MouseEnter += BezierPointEllipse_MouseEnter;
//                    ptEl.MouseLeave += BezierPointEllipse_MouseLeave;
//                }
//            }
            
//        }


//#endregion

//#region 曲线鼠标操作

//        Point mMouseDownPt = new Point();
//        bool mBezierPointInside = true;
//        void BezierPointEllipse_MouseLeave(object sender, MouseEventArgs e)
//        {
//            var elp = sender as Ellipse;
//            elp.Fill = this.FindResource("BezierPointColor") as Brush;
//        }

//        void BezierPointEllipse_MouseEnter(object sender, MouseEventArgs e)
//        {
//            var elp = sender as Ellipse;
//            elp.Fill = this.FindResource("MousePointAtColor") as Brush;
//        }

//        void BezierPointEllipse_MouseUp(object sender, MouseButtonEventArgs e)
//        {
//            if (mBezierPointInside == false)
//            {
//                var elp = sender as Ellipse;
//                var bPt = elp.Tag as BezierPoint;
//                RemoveBezierPoint(bPt);
//            }

//            Mouse.Capture(null);
//            e.Handled = true;
//        }

//        void BezierPointEllipse_MouseMove(object sender, MouseEventArgs e)
//        {
//            if (e.LeftButton == MouseButtonState.Pressed)
//            {
//                var elp = sender as Ellipse;
//                var pt = e.GetPosition(elp);
//                var deltaX = pt.X - mMouseDownPt.X;
//                var deltaY = pt.Y - mMouseDownPt.Y;
//                var bPt = elp.Tag as BezierPoint;

//                var idx = mBezierPoints.IndexOf(bPt);
//                if (idx == 0 || idx == mBezierPoints.Count - 1)
//                {
//                    deltaX = 0;

//                    if ((bPt.Position.Y + deltaY) < 0)
//                    {
//                        deltaY = -bPt.Position.Y;
//                    }
//                    else if ((bPt.Position.Y + deltaY) > MainCanvas.ActualHeight)
//                    {
//                        deltaY = MainCanvas.ActualHeight - bPt.Position.Y;
//                    }
//                }
//                else
//                {
//                    if (mBezierPointInside)
//                    {
//                        // 超出范围删除该点
//                        if (((bPt.Position.X + deltaX) < 0) ||
//                            ((bPt.Position.X + deltaX) > MainCanvas.ActualWidth) ||
//                            ((bPt.Position.Y + deltaY) < 0) ||
//                            ((bPt.Position.Y + deltaY) > MainCanvas.ActualHeight))
//                        {
//                            mBezierPointInside = false;
//                            elp.Fill = this.FindResource("DeletePointAtColor") as Brush;
//                        }
//                    }
//                    else
//                    {
//                        // 回到范围内增加该点
//                        if (!(((bPt.Position.X + deltaX) < 0) ||
//                            ((bPt.Position.X + deltaX) > MainCanvas.ActualWidth) ||
//                            ((bPt.Position.Y + deltaY) < 0) ||
//                            ((bPt.Position.Y + deltaY) > MainCanvas.ActualHeight)))
//                        {
//                            mBezierPointInside = true;
//                            elp.Fill = this.FindResource("MousePointAtColor") as Brush;
//                        }
//                    }
//                }

//                bPt.Position = new Point(bPt.Position.X + deltaX, bPt.Position.Y + deltaY);
//                bPt.ControlPoint = new Point(bPt.ControlPoint.X + deltaX, bPt.ControlPoint.Y + deltaY);

//                if (bPt.ControlPointEllipse != null)
//                {
//                    Canvas.SetLeft(bPt.ControlPointEllipse, bPt.ControlPoint.X);
//                    Canvas.SetTop(bPt.ControlPointEllipse, bPt.ControlPoint.Y);
//                }

//                if(idx > 0 && idx < mBezierPoints.Count - 1)
//                {
//                    var nbPt = mBezierPoints[idx + 1] as BezierPoint;
//                    nbPt.Position = new Point(bPt.Position.X, bPt.Position.Y);
//                    nbPt.ControlPoint = new Point(nbPt.ControlPoint.X + deltaX, nbPt.ControlPoint.Y + deltaY);
//                    if(nbPt.PositionEllipse != null)
//                    {
//                        Canvas.SetLeft(nbPt.PositionEllipse, nbPt.Position.X);
//                        Canvas.SetTop(nbPt.PositionEllipse, nbPt.Position.Y);
//                    }
//                    if (nbPt.ControlPointEllipse != null)
//                    {
//                        Canvas.SetLeft(nbPt.ControlPointEllipse, nbPt.ControlPoint.X);
//                        Canvas.SetTop(nbPt.ControlPointEllipse, nbPt.ControlPoint.Y);
//                    }
//                }
//                else if (idx == 0)
//                {
//                    BezierPathFigure.StartPoint = bPt.Position;
//                }

//                Canvas.SetLeft(elp, Canvas.GetLeft(elp) + deltaX);
//                Canvas.SetTop(elp, Canvas.GetTop(elp) + deltaY);

//                IsDirty = true;

//                UpdateLineXBezierShow();
//            }
//        }

//        void BezierPointEllipse_MouseDown(object sender, MouseButtonEventArgs e)
//        {
//            if (e.LeftButton == MouseButtonState.Pressed)
//            {
//                mBezierPointInside = true;
//                var elp = sender as Ellipse;
//                mMouseDownPt = e.GetPosition(elp);
//                Mouse.Capture(elp);
//            }

//            e.Handled = true;
//        }

//        void ControlPointEllipse_MouseLeave(object sender, MouseEventArgs e)
//        {
//            var elp = sender as Ellipse;
//            elp.Fill = this.FindResource("ControlPointColor") as Brush;
//        }

//        void ControlPointEllipse_MouseEnter(object sender, MouseEventArgs e)
//        {
//            var elp = sender as Ellipse;
//            elp.Fill = this.FindResource("MousePointAtColor") as Brush;
//        }

//        void ControlPointEllipse_MouseUp(object sender, MouseButtonEventArgs e)
//        {
//            Mouse.Capture(null);
//            e.Handled = true;
//        }

//        void ControlPointEllipse_MouseMove(object sender, MouseEventArgs e)
//        {
//            if (e.LeftButton == MouseButtonState.Pressed)
//            {
//                var elp = sender as Ellipse;
//                var pt = e.GetPosition(elp);
//                var deltaX = pt.X - mMouseDownPt.X;
//                var deltaY = pt.Y - mMouseDownPt.Y;
//                var bPt = elp.Tag as BezierPoint;

//                if ((bPt.ControlPoint.X + deltaX) < 0)
//                {
//                    deltaX = -bPt.ControlPoint.X;
//                }
//                else if ((bPt.ControlPoint.X + deltaX) > MainCanvas.ActualWidth)
//                {
//                    deltaX = MainCanvas.ActualWidth - bPt.ControlPoint.X;
//                }
//                if ((bPt.ControlPoint.Y + deltaY) < 0)
//                {
//                    deltaY = -bPt.ControlPoint.Y;
//                }
//                else if ((bPt.ControlPoint.Y + deltaY) > MainCanvas.ActualHeight)
//                {
//                    deltaY = MainCanvas.ActualHeight - bPt.ControlPoint.Y;
//                }

//                bPt.ControlPoint = new Point(bPt.ControlPoint.X + deltaX, bPt.ControlPoint.Y + deltaY);
//                Canvas.SetLeft(elp, Canvas.GetLeft(elp) + deltaX);
//                Canvas.SetTop(elp, Canvas.GetTop(elp) + deltaY);

//                IsDirty = true;

//                UpdateLineXBezierShow();
//            }
//        }

//        void ControlPointEllipse_MouseDown(object sender, MouseButtonEventArgs e)
//        {
//            if (e.LeftButton == MouseButtonState.Pressed)
//            {
//                var elp = sender as Ellipse;
//                mMouseDownPt = e.GetPosition(elp);
//                Mouse.Capture(elp);
//            }

//            e.Handled = true;
//        }

//        private void BezierPath_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
//        {
//            var point = e.GetPosition((FrameworkElement)sender);
//            Canvas.SetLeft(Ellipse_MousePoint, point.X);
//            Canvas.SetTop(Ellipse_MousePoint, point.Y);
//        }

//        private void BezierPath_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
//        {
//            Ellipse_MousePoint.Visibility = Visibility.Visible;
//        }

//        private void BezierPath_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
//        {
//            Ellipse_MousePoint.Visibility = Visibility.Collapsed;
//        }

//        private void BezierPath_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            if (e.LeftButton == MouseButtonState.Pressed)
//            {
//                var point = e.GetPosition((FrameworkElement)sender);
//                AddBezierPoint(point);
//            }
//        }

//        private void MainCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
//        {
//            if (BezierPosTest.Visibility == System.Windows.Visibility.Visible)
//            {
//                var pos = e.GetPosition(MainCanvas);
//                var bezierPos = CSUtility.Support.BezierCalculate.ValueOnBezier(mBezierPoints, pos.X);
//                Canvas.SetLeft(BezierPosTest, bezierPos.X);
//                Canvas.SetTop(BezierPosTest, bezierPos.Y);
//            }
//        }

//#endregion


        private void TextBox_InputValue_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                var bindingExp = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
                bindingExp.UpdateSource();
            }
        }

        private void TextBox_InputValue_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            var bindingExp = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            bindingExp.UpdateSource();
        }

        #region 代码生成

        protected override void CollectionErrorMsg()
        {
            var outLinkOI = GetLinkObjInfo(ValueOutputHandle);
            if(outLinkOI.HasLink)
            {
                var inLinkOI = GetLinkObjInfo(ValueInputHandle);
                if(!inLinkOI.HasLink)
                    AddErrorMsg(ValueInputHandle, CodeGenerateSystem.Controls.ErrorReportControl.ReportType.Error, "未设置X输入");
            }
        }

        public override bool HasMultiOutLink
        {
            get{ return true; }
        }

        private string StaticBezierPointsName
        {
            get
            {
                return "mBezierPoints_" + CodeGenerateSystem.Program.GetValuedGUIDString(Id);
            }
        }

        private string BezierPointsListType
        {
            get { return "System.Collections.Generic.List<CSUtility.Support.BezierPointBase>"; }
        }

        //private string MinBezierXVarName
        //{
        //    get{ return "MinBezierX_" + Program.GetValuedGUIDString(Id); }
        //}
        //private string MaxBezierXVarName
        //{
        //    get { return "MaxBezierX_" + Program.GetValuedGUIDString(Id); }
        //}
        //private string MinBezierYVarName
        //{
        //    get { return "MinBezierY_" + Program.GetValuedGUIDString(Id); }
        //}
        //private string MaxBezierYVarName
        //{
        //    get { return "MaxBezierY_" + Program.GetValuedGUIDString(Id); }
        //}

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return "System.Double";
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            return "BezierValue_" + CodeGenerateSystem.Program.GetValuedGUIDString(Id);
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            return new System.CodeDom.CodeVariableReferenceExpression(GCode_GetValueName(null));
            //var XInputLinkOI = GetLinkObjInfo(ValueInputHandle);
            //if (!XInputLinkOI.bHasLink)
            //{
            //    return new System.CodeDom.CodePrimitiveExpression(0);
            //}

            //// code: CSUtility.Support.BezierCalculate.ValueOnBezier(mBezierPoints_XXX, XPos);
            //return new System.CodeDom.CodeMethodInvokeExpression(
            //                new System.CodeDom.CodeTypeReferenceExpression(typeof(CSUtility.Support.BezierCalculate)),
            //                "ValueOnBezier",
            //                new System.CodeDom.CodeExpression[]{
            //                    new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeThisReferenceExpression(), StaticBezierPointsName),
            //                    XInputLinkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(XInputLinkOI.GetLinkElement(0, true))
            //                });
        }

        System.CodeDom.CodeMemberField mBezierPointsField = new System.CodeDom.CodeMemberField();
        System.CodeDom.CodeVariableDeclarationStatement mVariableDeclaration = new System.CodeDom.CodeVariableDeclarationStatement();

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            if (!codeClass.Members.Contains(mBezierPointsField))
            {
                // code: private static System.Collections.Generic.List<CSUtility.Support.BezierPointBase> mBezierPoints_XXX = new System.Collections.Generic.List<CSUtility.Support.BezierPointBase>(new BezierPoint[] { new CSUtility.Support.BezierPointBase(), new CSUtility.Support.BezierPointBase() });
                mBezierPointsField.Type = new System.CodeDom.CodeTypeReference(BezierPointsListType);
                mBezierPointsField.Name = StaticBezierPointsName;
                mBezierPointsField.Attributes = System.CodeDom.MemberAttributes.Static | System.CodeDom.MemberAttributes.Private;
                var arrayCreateExp = new System.CodeDom.CodeArrayCreateExpression();
                arrayCreateExp.CreateType = new System.CodeDom.CodeTypeReference(typeof(CSUtility.Support.BezierPointBase));
                foreach (var bPt in LineXBezierCtrl.BezierPoints)
                {
                    var newBPT = new System.CodeDom.CodeObjectCreateExpression();
                    newBPT.CreateType = new System.CodeDom.CodeTypeReference(typeof(CSUtility.Support.BezierPointBase));
                    newBPT.Parameters.Add(new System.CodeDom.CodeObjectCreateExpression(typeof(SlimDX.Vector2),
                                                new System.CodeDom.CodeExpression[]{
                                                    new System.CodeDom.CodePrimitiveExpression(bPt.Position.X),
                                                    new System.CodeDom.CodePrimitiveExpression(bPt.Position.Y)
                                                }));
                    newBPT.Parameters.Add(new System.CodeDom.CodeObjectCreateExpression(typeof(SlimDX.Vector2),
                                                new System.CodeDom.CodeExpression[]{
                                                    new System.CodeDom.CodePrimitiveExpression(bPt.ControlPoint.X),
                                                    new System.CodeDom.CodePrimitiveExpression(bPt.ControlPoint.Y)
                                                }));
                    arrayCreateExp.Initializers.Add(newBPT);
                }
                mBezierPointsField.InitExpression = new System.CodeDom.CodeObjectCreateExpression(BezierPointsListType, arrayCreateExp);
                codeClass.Members.Add(mBezierPointsField);
            }

            // 判断5个输入链接是否需要生成代码
            var XInputLinkOI = GetLinkObjInfo(ValueInputHandle);
            if (XInputLinkOI.HasLink)
            {
                if (!XInputLinkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    XInputLinkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, XInputLinkOI.GetLinkElement(0, true));
            }

            var YMaxLinkOI = GetLinkObjInfo(ValueYMaxInputHandle);
            if (YMaxLinkOI.HasLink)
            {
                if (!YMaxLinkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    YMaxLinkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, YMaxLinkOI.GetLinkElement(0, true));
            }

            var YMinLinkOI = GetLinkObjInfo(ValueYMinInputHandle);
            if (YMinLinkOI.HasLink)
            {
                if (!YMinLinkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    YMinLinkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, YMinLinkOI.GetLinkElement(0, true));
            }

            var XMaxLinkOI = GetLinkObjInfo(ValueXMaxInputHandle);
            if (XMaxLinkOI.HasLink)
            {
                if (!XMaxLinkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    XMaxLinkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, XMaxLinkOI.GetLinkElement(0, true));
            }

            var XMinLinkOI = GetLinkObjInfo(ValueXMinInputHandle);
            if (XMinLinkOI.HasLink)
            {
                if (!XMinLinkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    XMinLinkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, XMinLinkOI.GetLinkElement(0, true));
            }

            //// 自身代码生成
            //if (!codeStatementCollection.Contains(mVariableDeclaration))
            //{
            //    // 声明
            //    mVariableDeclaration.Type = new System.CodeDom.CodeTypeReference(typeof(double));
            //    mVariableDeclaration.Name = GCode_GetValueName(null);
            //    codeStatementCollection.Add(mVariableDeclaration);
            //}

            var assignExp = new System.CodeDom.CodeAssignStatement();
            assignExp.Left = new System.CodeDom.CodeVariableReferenceExpression(GCode_GetValueName(null));

            System.CodeDom.CodeExpression minXExp, maxXExp, minYExp, maxYExp;
            if (XMinLinkOI.HasLink)
                minXExp = XMinLinkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(XMinLinkOI.GetLinkElement(0, true));
            else
                minXExp = new System.CodeDom.CodePrimitiveExpression(XMin);

            if (XMaxLinkOI.HasLink)
                maxXExp = XMaxLinkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(XMaxLinkOI.GetLinkElement(0, true));
            else
                maxXExp = new System.CodeDom.CodePrimitiveExpression(XMax);

            if (YMinLinkOI.HasLink)
                minYExp = YMinLinkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(YMinLinkOI.GetLinkElement(0, true));
            else
                minYExp = new System.CodeDom.CodePrimitiveExpression(YMin);

            if (YMaxLinkOI.HasLink)
                maxYExp = YMaxLinkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(YMaxLinkOI.GetLinkElement(0, true));
            else
                maxYExp = new System.CodeDom.CodePrimitiveExpression(YMax);

            // code: CSUtility.Support.BezierCalculate.ValueOnBezier(mBezierPoints_XXX, XPos);
            assignExp.Right = new System.CodeDom.CodeMethodInvokeExpression(
                                                new System.CodeDom.CodeTypeReferenceExpression(typeof(CSUtility.Support.BezierCalculate)),
                                                "ValueOnBezier",
                                                new System.CodeDom.CodeExpression[]{
                                                new System.CodeDom.CodeVariableReferenceExpression(StaticBezierPointsName),     // bezierPtList
                                                XInputLinkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(XInputLinkOI.GetLinkElement(0, true)), // xValue
                                                minXExp, maxXExp, minYExp, maxYExp,
                                                new System.CodeDom.CodePrimitiveExpression(0),                  // MinBezierX
                                                new System.CodeDom.CodePrimitiveExpression(LineXBezierCtrl.BezierWidth),   // MaxBezierX
                                                new System.CodeDom.CodePrimitiveExpression(0),                  // MinBezierY
                                                new System.CodeDom.CodePrimitiveExpression(LineXBezierCtrl.BezierHeight),  // MaxBezierY
                                                new System.CodeDom.CodePrimitiveExpression(IsXLoop)             // bLoopX
                                        });

            codeStatementCollection.Add(assignExp);
        }

#endregion
    }
}
