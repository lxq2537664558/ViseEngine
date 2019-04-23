using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UIEditor
{
    public partial class DrawPanel
    {

        double mAssistSplitLength = 4;
        Brush mAssistBrush = new SolidColorBrush(Color.FromArgb(153, 0, 128, 255));
        List<FrameworkElement> mColumnAssisList = new List<FrameworkElement>();
        List<FrameworkElement> mRowAssisList = new List<FrameworkElement>();
        private void AddColumnAssist(int idx)
        {
            var gridColumnAssist = new Rectangle()
            {
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, -20, 0, 0),
                Height = 10,
                Fill = mAssistBrush,
            };
            Grid.SetColumn(gridColumnAssist, idx);
            gridColumnAssist.MouseDown += Rect_GridColumnAssist_MouseDown;
            gridColumnAssist.MouseUp += Rect_GridColumnAssist_MouseUp;
            gridColumnAssist.MouseMove += Rect_GridColumnAssist_MouseMove;
            gridColumnAssist.MouseEnter += Rect_GridColumnAssist_MouseEnter;
            gridColumnAssist.MouseLeave += Rect_GridColumnAssist_MouseLeave;
            mColumnAssisList.Add(gridColumnAssist);
            GridAssist.Children.Add(gridColumnAssist);
        }

        private void AddRowAssist(int idx)
        {
            var gridRowAssist = new Rectangle()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(-20, 0, 0, 0),
                Width = 10,
                Fill = mAssistBrush,
            };
            Grid.SetRow(gridRowAssist, idx);
            gridRowAssist.MouseDown += Rect_GridRowAssist_MouseDown;
            gridRowAssist.MouseUp += Rect_GridRowAssist_MouseUp;
            gridRowAssist.MouseMove += Rect_GridRowAssist_MouseMove;
            gridRowAssist.MouseEnter += Rect_GridRowAssist_MouseEnter;
            gridRowAssist.MouseLeave += Rect_GridRowAssist_MouseLeave;
            mRowAssisList.Add(gridRowAssist);
            GridAssist.Children.Add(gridRowAssist);
        }
        // 用于操作的Grid目标
        UISystem.Container.Grid.Grid mGridAssistTarget = null;
        internal UISystem.Container.Grid.Grid GridAssistTarget
        {
            get { return mGridAssistTarget; }
            set { mGridAssistTarget = value; }
        }
        internal void InitializeGridAssist(bool force = false)
        {
            if (SelectedWinControls.Count != 1)
            {
                GridAssist.Visibility = Visibility.Collapsed;
                return;
            }

            var grid = SelectedWinControls[0].UIWin as UISystem.Container.Grid.Grid;
            if (grid == null)
            {
                mGridAssistTarget = null;
                GridAssist.Visibility = Visibility.Collapsed;
                return;
            }

            if (mGridAssistTarget == grid && !force)
                return;
            mGridAssistTarget = grid;

            foreach (var column in mColumnAssisList)
            {
                GridAssist.Children.Remove(column);
            }
            mColumnAssisList.Clear();
            foreach (var row in mRowAssisList)
            {
                GridAssist.Children.Remove(row);
            }
            mRowAssisList.Clear();
            GridAssist.ColumnDefinitions.Clear();
            GridAssist.RowDefinitions.Clear();
            GridAssist.Visibility = Visibility.Visible;

            if (mGridAssistTarget.ColumnDefinitions.Count > 0)
            {
                int i = 0;
                foreach (var column in mGridAssistTarget.ColumnDefinitions)
                {
                    var tagColumn = new ColumnDefinition();
                    tagColumn.Width = new GridLength(column.UserSize.Value * GetScaleDelta(), (GridUnitType)(column.UserSize.GridUnitType));
                    tagColumn.MinWidth = column.UserSize.MinLength;
                    tagColumn.MaxWidth = column.UserSize.MaxLength;
                    GridAssist.ColumnDefinitions.Add(tagColumn);

                    AddColumnAssist(GridAssist.ColumnDefinitions.Count - 1);

                    if (i < mGridAssistTarget.ColumnDefinitions.Count - 1)
                    {
                        //var splitterColumn = new ColumnDefinition();
                        //splitterColumn.Width = new GridLength(0, GridUnitType.Pixel);
                        //GridAssist.ColumnDefinitions.Add(splitterColumn);
                        var split = new GridSplitter()
                        {
                            Width = mAssistSplitLength,
                            Background = mAssistBrush,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Margin = new Thickness(-mAssistSplitLength * 0.5, -8, -mAssistSplitLength * 0.5, 0),
                            ShowsPreview = true,
                        };
                        //split.DragDelta += GridAssistColumnSplit_DragDelta;
                        split.DragCompleted += GridAssistColumnSplit_DragCompleted;
                        Grid.SetColumn(split, GridAssist.ColumnDefinitions.Count - 1);
                        if(GridAssistTarget.RowDefinitions.Count > 0)
                            Grid.SetRowSpan(split, GridAssistTarget.RowDefinitions.Count);
                        GridAssist.Children.Add(split);
                        mColumnAssisList.Add(split);
                    }

                    var infoCtrl = new GridLengthAssistControl(this, column, GridLengthAssistControl.enType.Column);
                    Grid.SetColumn(infoCtrl, GridAssist.ColumnDefinitions.Count - 1);
                    if(GridAssistTarget.RowDefinitions.Count > 0)
                        Grid.SetRowSpan(infoCtrl, GridAssistTarget.RowDefinitions.Count);
                    GridAssist.Children.Add(infoCtrl);
                    mColumnAssisList.Add(infoCtrl);
                    i++;
                }

                Grid.SetColumnSpan(Path_GridAssist_Column, GridAssistTarget.ColumnDefinitions.Count);
                Grid.SetColumnSpan(Path_GridAssist_Row, GridAssistTarget.ColumnDefinitions.Count);
            }
            else
            {
                AddColumnAssist(0);
            }

            if (mGridAssistTarget.RowDefinitions.Count > 0)
            {
                int i = 0;
                foreach (var row in mGridAssistTarget.RowDefinitions)
                {
                    var tagRow = new RowDefinition();
                    tagRow.Height = new GridLength(row.UserSize.Value * GetScaleDelta(), (GridUnitType)(row.UserSize.GridUnitType));
                    tagRow.MinHeight = row.UserSize.MinLength;
                    tagRow.MaxHeight = row.UserSize.MaxLength;
                    GridAssist.RowDefinitions.Add(tagRow);

                    AddRowAssist(GridAssist.RowDefinitions.Count - 1);

                    if (i < mGridAssistTarget.RowDefinitions.Count - 1)
                    {
                        //var splitterRow = new RowDefinition();
                        //splitterRow.Height = new GridLength(0, GridUnitType.Pixel);
                        //GridAssist.RowDefinitions.Add(splitterRow);
                        var split = new GridSplitter()
                        {
                            Height = mAssistSplitLength,
                            Background = mAssistBrush,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            Margin = new Thickness(-8, -mAssistSplitLength * 0.5, 0, -mAssistSplitLength * 0.5),
                            ShowsPreview = true,
                        };
                        //split.DragDelta += GridAssistRowSplit_DragDelta;
                        split.DragCompleted += GridAssistRowSplit_DragCompleted;
                        Grid.SetRow(split, GridAssist.RowDefinitions.Count - 1);
                        if(GridAssistTarget.ColumnDefinitions.Count > 0)
                            Grid.SetColumnSpan(split, GridAssistTarget.ColumnDefinitions.Count);
                        GridAssist.Children.Add(split);
                        mRowAssisList.Add(split);
                    }

                    var infoCtrl = new GridLengthAssistControl(this, row, GridLengthAssistControl.enType.Row);
                    Grid.SetRow(infoCtrl, GridAssist.RowDefinitions.Count - 1);
                    if(GridAssistTarget.ColumnDefinitions.Count > 0)
                        Grid.SetColumnSpan(infoCtrl, GridAssistTarget.ColumnDefinitions.Count);
                    GridAssist.Children.Add(infoCtrl);
                    mRowAssisList.Add(infoCtrl);
                    i++;
                }

                Grid.SetRowSpan(Path_GridAssist_Column, GridAssistTarget.RowDefinitions.Count);
                Grid.SetRowSpan(Path_GridAssist_Row, GridAssistTarget.RowDefinitions.Count);
            }
            else
            {
                AddRowAssist(0);
            }
        }
        // 更新列数据
        internal void UpdateGridAssistColmunData(int idx)
        {
            if (mGridAssistTarget == null ||
                mGridAssistTarget.ColumnDefinitions.Count != GridAssist.ColumnDefinitions.Count)
            {
                InitializeGridAssist(true);
                return;
            }

            if (idx < 0 || idx > mGridAssistTarget.ColumnDefinitions.Count - 1)
                return;

            var column = mGridAssistTarget.ColumnDefinitions[idx];
            var assistColumn = GridAssist.ColumnDefinitions[idx];
            var length = column.UserSize.Length / GetScaleDelta();
            assistColumn.Width = new GridLength(length < 0 ? 0 : length, (GridUnitType)column.UserSize.GridUnitType);
        }
        // 更新行数据
        internal void UpdateGridAssistRowData(int idx)
        {
            if(mGridAssistTarget == null ||
               mGridAssistTarget.RowDefinitions.Count != GridAssist.RowDefinitions.Count)
            {
                InitializeGridAssist(true);
                return;
            }

            if (idx < 0 || idx > mGridAssistTarget.RowDefinitions.Count - 1)
                return;

            var row = mGridAssistTarget.RowDefinitions[idx];
            var assistRow = GridAssist.RowDefinitions[idx];
            var length = row.UserSize.Length / GetScaleDelta();
            assistRow.Height = new GridLength(length < 0 ? 0 : length, (GridUnitType)row.UserSize.GridUnitType);
        }
        
        // 列--------------------------
        private void UpdateColumnDefinition()
        {
            for (int i = 0; i < GridAssist.ColumnDefinitions.Count; i++)
            {
                mGridAssistTarget.ColumnDefinitions[i].UserSize.Length = (float)(GridAssist.ColumnDefinitions[i].Width.Value / GetScaleDelta());
                mGridAssistTarget.ColumnDefinitions[i].UserSize.GridUnitType = (UISystem.Container.Grid.GridUnitType)(GridAssist.ColumnDefinitions[i].Width.GridUnitType);
            }
            mGridAssistTarget.UpdateLayout();
            mWinRoot.Tick(CCore.Engine.Instance.GetFrameMillisecond());
        }
        private void GridAssistColumnSplit_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            UpdateColumnDefinition();
        }
        private void GridAssistColumnSplit_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            UpdateColumnDefinition();
        }

        private void CalculateColumnSplit(int oldColumnIdx, double length1, double length2)
        {
            if (oldColumnIdx < 0 || oldColumnIdx >= mGridAssistTarget.ColumnDefinitions.Count)
                return;
            
            foreach (var childWin in mGridAssistTarget.GetChildWindows())
            {
                if(childWin.GridColumn <= oldColumnIdx || ((childWin.GridColumn + childWin.GridColumnSpan) > oldColumnIdx))
                {
                    var childWinLeft = childWin.AbsRect.Left - mGridAssistTarget.AbsRect.Left;// + childWin.Margin.Left;
                    var childWinRight = childWinLeft + childWin.Width;
                    var columnLeft = 0.0;
                    UInt16 startColumnIdx = 0;
                    UInt16 endColumnIdx = 0;
                    UInt16 idx = 0;
                    foreach(UISystem.Container.Grid.ColumnDefinition column in mGridAssistTarget.ColumnDefinitions)
                    {
                        double length = column.ActualWidth;
                        if (idx == oldColumnIdx)
                            length = length1;
                        else if (idx == oldColumnIdx + 1)
                            length = length2;

                        columnLeft += length;
                        if (((int)columnLeft) > childWinLeft)
                        {
                            startColumnIdx = idx;
                            columnLeft -= length;
                            break;
                        }

                        idx++;
                    }
                    idx = 0;
                    var columnRight = 0.0;
                    foreach(UISystem.Container.Grid.ColumnDefinition column in mGridAssistTarget.ColumnDefinitions)
                    {
                        double length = column.ActualWidth;
                        if (idx == oldColumnIdx)
                            length = length1;
                        else if (idx == oldColumnIdx + 1)
                            length = length2;

                        columnRight += length;
                        if(((int)columnRight) >= childWinRight)
                        {
                            endColumnIdx = idx;
                            break;
                        }

                        idx++;
                    }

                    childWin.GridColumn = startColumnIdx;
                    childWin.GridColumnSpan = (UInt16)(endColumnIdx - startColumnIdx + 1);
                    float marginLeft = (float)(childWinLeft - columnLeft), marginRight = (float)(columnRight - childWinRight);
                    float marginTop = childWin.Margin.Top, marginBottom = childWin.Margin.Bottom;
                    childWin.Margin = new CSUtility.Support.Thickness(marginLeft, marginTop, marginRight, marginBottom);
                }
            }
        }

        private void Rect_GridColumnAssist_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var senderObj = sender as FrameworkElement;
            var pos = e.GetPosition(senderObj);
            int columnIdx = 0;
            if (GridAssist.ColumnDefinitions.Count == 0)
            {
                var delta = pos.X / senderObj.ActualWidth;
                var length1 = GridAssistTarget.Width * delta;
                var length2 = GridAssistTarget.Width - length1;

                var def = new UISystem.Container.Grid.ColumnDefinition(mGridAssistTarget);
                def.UserSize.Length = (float)length1;
                def.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Star;
                mGridAssistTarget.ColumnDefinitions.Add(def);

                def = new UISystem.Container.Grid.ColumnDefinition(mGridAssistTarget);
                def.UserSize.Length = (float)length2;
                def.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Star;
                mGridAssistTarget.ColumnDefinitions.Add(def);

                CalculateColumnSplit(columnIdx, length1, length2);
            }
            else
            {
                columnIdx = Grid.GetColumn(senderObj);
                var assistColumn = GridAssist.ColumnDefinitions[columnIdx];
                var delta = pos.X / senderObj.ActualWidth;
                var length1 = senderObj.ActualWidth * delta / GetScaleDelta();
                var length2 = senderObj.ActualWidth * (1 - delta) / GetScaleDelta();
                var column = GridAssistTarget.ColumnDefinitions[columnIdx];
                switch (assistColumn.Width.GridUnitType)
                {
                    case GridUnitType.Auto:
                    case GridUnitType.Pixel:
                        {
                            column.UserSize.Length = (float)length1;
                            column.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Pixel;

                            var def = new UISystem.Container.Grid.ColumnDefinition(mGridAssistTarget);
                            def.UserSize.Length = (float)length2;
                            def.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Pixel;
                            if (columnIdx == GridAssist.ColumnDefinitions.Count - 1)
                                mGridAssistTarget.ColumnDefinitions.Add(def);
                            else
                                mGridAssistTarget.ColumnDefinitions.Insert(columnIdx + 1, def);
                        }
                        break;
                    case GridUnitType.Star:
                        {
                            column.UserSize.Length = (float)length1;
                            column.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Star;

                            var def = new UISystem.Container.Grid.ColumnDefinition(mGridAssistTarget);
                            def.UserSize.Length = (float)length2;
                            def.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Star;
                            if (columnIdx == GridAssist.ColumnDefinitions.Count - 1)
                                mGridAssistTarget.ColumnDefinitions.Add(def);
                            else
                                mGridAssistTarget.ColumnDefinitions.Insert(columnIdx + 1, def);
                        }
                        break;
                }

                CalculateColumnSplit(columnIdx, length1, length2);
            }

            mGridAssistTarget.ColumnDefinitions = new CSUtility.Support.ThreadSafeObservableCollection<UISystem.Container.Grid.DefinitionBase>(mGridAssistTarget.ColumnDefinitions);
            InitializeGridAssist(true);
            mGridAssistTarget.UpdateLayout();
            //mWinRoot.Tick(CCore.Engine.Instance.GetFrameMillisecond());
            e.Handled = true;
        }
        private void Rect_GridColumnAssist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Rect_GridColumnAssist_MouseMove(object sender, MouseEventArgs e)
        {
            var senderObj = sender as FrameworkElement;
            var pos = e.GetPosition(senderObj);
            Path_GridAssist_Column.Margin = new Thickness(pos.X, -20, 0, 0);
        }

        private void Rect_GridColumnAssist_MouseEnter(object sender, MouseEventArgs e)
        {
            Path_GridAssist_Column.Visibility = Visibility.Visible;

            var senderObj = sender as FrameworkElement;
            var columnIdx = Grid.GetColumn(senderObj);
            Grid.SetColumn(Path_GridAssist_Column, columnIdx);
        }

        private void Rect_GridColumnAssist_MouseLeave(object sender, MouseEventArgs e)
        {
            Path_GridAssist_Column.Visibility = Visibility.Collapsed;
        }
        
        // 行----------------------
        private void UpdateRowDefinitions()
        {
            for (int i = 0; i < GridAssist.RowDefinitions.Count; i++)
            {
                mGridAssistTarget.RowDefinitions[i].UserSize.Length = (float)(GridAssist.RowDefinitions[i].Height.Value * GetScaleDelta());
                mGridAssistTarget.RowDefinitions[i].UserSize.GridUnitType = (UISystem.Container.Grid.GridUnitType)(GridAssist.RowDefinitions[i].Height.GridUnitType);
            }
            mGridAssistTarget.UpdateLayout();
            mWinRoot.Tick(CCore.Engine.Instance.GetFrameMillisecond());
        }
        private void GridAssistRowSplit_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            UpdateRowDefinitions();
        }
        private void GridAssistRowSplit_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            UpdateRowDefinitions();
        }

        private void CalculateRowSplit(int oldRowIdx, double length1, double length2)
        {
            if (oldRowIdx < 0 || oldRowIdx >= mGridAssistTarget.RowDefinitions.Count)
                return;

            foreach(var childWin in mGridAssistTarget.GetChildWindows())
            {
                if(childWin.GridRow <= oldRowIdx || ((childWin.GridRow + childWin.GridRowSpan) > oldRowIdx))
                {
                    var childWinTop = childWin.AbsRect.Top - mGridAssistTarget.AbsRect.Top;
                    var childWinBottom = childWinTop + childWin.Height;
                    var rowTop = 0.0;
                    UInt16 startRowIdx = 0;
                    UInt16 endRowIdx = 0;
                    UInt16 idx = 0;
                    foreach(UISystem.Container.Grid.RowDefinition row in mGridAssistTarget.RowDefinitions)
                    {
                        double length = row.ActualHeight;
                        if (idx == oldRowIdx)
                            length = length1;
                        else if (idx == oldRowIdx + 1)
                            length = length2;

                        rowTop += length;
                        if(((int)rowTop) > childWinTop)
                        {
                            startRowIdx = idx;
                            rowTop -= length;
                            break;
                        }

                        idx++;
                    }
                    idx = 0;
                    var rowBottom = 0.0;
                    foreach(UISystem.Container.Grid.RowDefinition row in mGridAssistTarget.RowDefinitions)
                    {
                        double length = row.ActualHeight;
                        if (idx == oldRowIdx)
                            length = length1;
                        else if (idx == oldRowIdx + 1)
                            length = length2;

                        rowBottom += length;
                        if(((int)rowBottom) >= childWinBottom)
                        {
                            endRowIdx = idx;
                            break;
                        }

                        idx++;
                    }

                    childWin.GridRow = startRowIdx;
                    childWin.GridRowSpan = (UInt16)(endRowIdx - startRowIdx + 1);
                    float marginLeft = childWin.Margin.Left, marginRight = childWin.Margin.Right;
                    float marginTop = (float)(childWinTop - rowTop), marginBottom = (float)(rowBottom - childWinBottom);
                    childWin.Margin = new CSUtility.Support.Thickness(marginLeft, marginTop, marginRight, marginBottom);
                }
            }
        }
        
        private void Rect_GridRowAssist_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var senderObj = sender as FrameworkElement;
            var pos = e.GetPosition(senderObj);
            int rowIdx = 0;
            if(GridAssist.RowDefinitions.Count == 0)
            {
                var delta = pos.Y / senderObj.ActualHeight;
                var length1 = GridAssistTarget.Height * delta;
                var length2 = GridAssistTarget.Height - length1;

                var def = new UISystem.Container.Grid.RowDefinition(mGridAssistTarget);
                def.UserSize.Length = (float)length1;
                def.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Star;
                mGridAssistTarget.RowDefinitions.Add(def);

                def = new UISystem.Container.Grid.RowDefinition(mGridAssistTarget);
                def.UserSize.Length = (float)length2;
                def.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Star;
                mGridAssistTarget.RowDefinitions.Add(def);

                CalculateRowSplit(rowIdx, length1, length2);
            }
            else
            {
                rowIdx = Grid.GetRow(senderObj);
                var assistRow = GridAssist.RowDefinitions[rowIdx];
                var delta = pos.Y / senderObj.ActualHeight;
                var length1 = senderObj.ActualHeight * delta / GetScaleDelta();
                var length2 = senderObj.ActualHeight * (1 - delta) / GetScaleDelta();
                var row = GridAssistTarget.RowDefinitions[rowIdx];
                switch(assistRow.Height.GridUnitType)
                {
                    case GridUnitType.Auto:
                    case GridUnitType.Pixel:
                        {
                            row.UserSize.Length = (float)length1;
                            row.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Pixel;

                            var def = new UISystem.Container.Grid.ColumnDefinition(mGridAssistTarget);
                            def.UserSize.Length = (float)length2;
                            def.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Pixel;
                            if (rowIdx == GridAssist.RowDefinitions.Count - 1)
                                mGridAssistTarget.RowDefinitions.Add(def);
                            else
                                mGridAssistTarget.RowDefinitions.Insert(rowIdx + 1, def);
                        }
                        break;
                    case GridUnitType.Star:
                        {
                            row.UserSize.Length = (float)length1;
                            row.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Star;

                            var def = new UISystem.Container.Grid.RowDefinition(mGridAssistTarget);
                            def.UserSize.Length = (float)length2;
                            def.UserSize.GridUnitType = UISystem.Container.Grid.GridUnitType.Star;
                            if (rowIdx == GridAssist.RowDefinitions.Count - 1)
                                mGridAssistTarget.RowDefinitions.Add(def);
                            else
                                mGridAssistTarget.RowDefinitions.Insert(rowIdx + 1, def);
                        }
                        break;
                }

                CalculateRowSplit(rowIdx, length1, length2);
            }

            mGridAssistTarget.RowDefinitions = new CSUtility.Support.ThreadSafeObservableCollection<UISystem.Container.Grid.DefinitionBase>(mGridAssistTarget.RowDefinitions);
            InitializeGridAssist(true);
            mGridAssistTarget.UpdateLayout();
            e.Handled = true;
        }

        private void Rect_GridRowAssist_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Rect_GridRowAssist_MouseMove(object sender, MouseEventArgs e)
        {
            var senderObj = sender as FrameworkElement;
            var pos = e.GetPosition(senderObj);
            Path_GridAssist_Row.Margin = new Thickness(-20, pos.Y, 0, 0);
        }

        private void Rect_GridRowAssist_MouseEnter(object sender, MouseEventArgs e)
        {
            Path_GridAssist_Row.Visibility = Visibility.Visible;

            var senderObj = sender as FrameworkElement;
            var rowIdx = Grid.GetRow(senderObj);
            Grid.SetRow(Path_GridAssist_Row, rowIdx);
        }

        private void Rect_GridRowAssist_MouseLeave(object sender, MouseEventArgs e)
        {
            Path_GridAssist_Row.Visibility = Visibility.Collapsed;
        }
        

    }
}
