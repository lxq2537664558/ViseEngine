using System;
using System.ComponentModel;

namespace UISystem.Container.Grid
{
    public enum GridResizeDirection
    {
        Auto,
        Columns,
        Rows,
    }
    
    public enum GridResizeBehavior
    {
        BasedOnAlignment,
        CurrentAndNext,
        PreviousAndCurrent,
        PreviousAndNext,
    }

    [CSUtility.Editor.UIEditor_Control("常用.GridSplitter")]
    public class GridSplitter : WinBase
    {
        #region Data

        private enum SplitBehavior
        {
            Split, 
            Resize1, 
            Resize2, 
        }

        private class ResizeData
        {
            public float MinChange = 0;
            public float MaxChange = 0;

            public Grid Grid;

            public GridResizeDirection ResizeDirection;
            public GridResizeBehavior ResizeBehavior;

            public DefinitionBase Definition1;
            public DefinitionBase Definition2;

            public SplitBehavior SplitBehavior;

            public int SplitterIndex;

            public int Definition1Index;
            public int Definition2Index;

            public GridLength OriginalDefinition1Length;
            public GridLength OriginalDefinition2Length;
            public float OriginalDefinition1ActualLength;
            public float OriginalDefinition2ActualLength;

            public float SplitterLength; 
        }

        private ResizeData _resizeData;

        #endregion

        #region Propertys

        GridResizeDirection mResizeDirection = GridResizeDirection.Auto;
        [Category("行为"), DisplayName("大小调整方向")]
        public GridResizeDirection ResizeDirection
        {
            get { return mResizeDirection; }
            set { mResizeDirection = value; }
        }

        GridResizeBehavior mResizeBehavior = GridResizeBehavior.BasedOnAlignment;
        [Category("行为"), DisplayName("大小调整行为")]
        public GridResizeBehavior ResizeBehavior
        {
            get { return mResizeBehavior; }
            set { mResizeBehavior = value; }
        }

        float mDragIncrement = 1;
        [Category("行为"), DisplayName("拖动增量")]
        public float DragIncrement
        {
            get{ return mDragIncrement; }
            set{ mDragIncrement = value; }
        }

        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [Browsable(false)]
        public WinState State
        {
            get { return mWinState; }
            set
            {
                mWinState.CopyFrom(value);
                OnPropertyChanged("State");
            }
        }
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("背景图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid StateUVAnimId
        {
            get
            {
                if (mWinState == null)
                    return Guid.Empty;
                return mWinState.UVAnimId;
            }
            set
            {
                if (mWinState != null)
                    mWinState.UVAnimId = value;
                OnPropertyChanged("StateUVAnimId");
            }
        }

        #endregion

        public GridSplitter()
        {
            mWinState = new WinState(this);
            mDragEnable = true;
        }
        
        private void InitializeData()
        {
            Grid grid = Parent as Grid;

            if (grid != null)
            {
                _resizeData = new ResizeData();
                _resizeData.Grid = grid;
                _resizeData.ResizeDirection = GetEffectiveResizeDirection();
                _resizeData.ResizeBehavior = GetEffectiveResizeBehavior(_resizeData.ResizeDirection);
                _resizeData.SplitterLength = System.Math.Min(Width, Height);

                if (!SetupDefinitionsToResize())
                {
                    _resizeData = null;
                    return;
                } 
            }
        }

        // 可以调整横列大小时返回true
        private bool SetupDefinitionsToResize()
        {
            int splitterIndex, index1, index2;

            int gridSpan = _resizeData.ResizeDirection == GridResizeDirection.Columns ? GridColumnSpan : GridRowSpan;

            if (gridSpan == 1)
            {
                splitterIndex = _resizeData.ResizeDirection == GridResizeDirection.Columns ? GridColumn : GridRow;

                // 根据behavior选择列
                switch (_resizeData.ResizeBehavior)
                {
                    case GridResizeBehavior.PreviousAndCurrent:
                        index1 = splitterIndex - 1;
                        index2 = splitterIndex;
                        break;
                    case GridResizeBehavior.CurrentAndNext:
                        index1 = splitterIndex;
                        index2 = splitterIndex + 1;
                        break;
                    default: // GridResizeBehavior.PreviousAndNext
                        index1 = splitterIndex - 1;
                        index2 = splitterIndex + 1;
                        break;
                }

                int count = (_resizeData.ResizeDirection == GridResizeDirection.Columns) ? _resizeData.Grid.ColumnDefinitions.Count : _resizeData.Grid.RowDefinitions.Count;

                if (index1 >= 0 && index2 < count)
                {
                    _resizeData.SplitterIndex = splitterIndex;

                    _resizeData.Definition1Index = index1;
                    _resizeData.Definition1 = GetGridDefinition(_resizeData.Grid, index1, _resizeData.ResizeDirection);
                    _resizeData.OriginalDefinition1Length = _resizeData.Definition1.UserSizeValueCache;  
                    _resizeData.OriginalDefinition1ActualLength = GetActualLength(_resizeData.Definition1);

                    _resizeData.Definition2Index = index2;
                    _resizeData.Definition2 = GetGridDefinition(_resizeData.Grid, index2, _resizeData.ResizeDirection);
                    _resizeData.OriginalDefinition2Length = _resizeData.Definition2.UserSizeValueCache;  
                    _resizeData.OriginalDefinition2ActualLength = GetActualLength(_resizeData.Definition2);

                    // 确定调整列大小方式
                    bool isStar1 = IsStar(_resizeData.Definition1);
                    bool isStar2 = IsStar(_resizeData.Definition2);
                    if (isStar1 && isStar2)
                    {
                        _resizeData.SplitBehavior = SplitBehavior.Split;
                    }
                    else
                    {
                        _resizeData.SplitBehavior = !isStar1 ? SplitBehavior.Resize1 : SplitBehavior.Resize2;
                    }

                    return true;
                }
            }

            return false;
        }

        private GridResizeDirection GetEffectiveResizeDirection()
        {
            GridResizeDirection direction = ResizeDirection;

            if (direction == GridResizeDirection.Auto)
            {
                if (HorizontalAlignment != UI.HorizontalAlignment.Stretch)
                {
                    direction = GridResizeDirection.Columns;
                }
                else if (VerticalAlignment != UI.VerticalAlignment.Stretch)
                {
                    direction = GridResizeDirection.Rows;
                }
                else if (Width <= Height)
                {
                    direction = GridResizeDirection.Columns;
                }
                else
                {
                    direction = GridResizeDirection.Rows;
                }

            }
            return direction;
        }

        private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
        {
            GridResizeBehavior resizeBehavior = ResizeBehavior;

            if (resizeBehavior == GridResizeBehavior.BasedOnAlignment)
            {
                if (direction == GridResizeDirection.Columns)
                {
                    switch (HorizontalAlignment)
                    {
                        case UI.HorizontalAlignment.Left:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case UI.HorizontalAlignment.Right:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
                else
                {
                    switch (VerticalAlignment)
                    {
                        case UI.VerticalAlignment.Top:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case UI.VerticalAlignment.Bottom:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    } 
                }
            }

            return resizeBehavior;
        }

        #region Helper Methods

        #region Row/Column Abstractions
        private static bool IsStar(DefinitionBase definition)
        {
            return definition.UserSizeValueCache.IsStar;
        }

        private static DefinitionBase GetGridDefinition(Grid grid, int index, GridResizeDirection direction)
        {
            return direction == GridResizeDirection.Columns ? (DefinitionBase)grid.ColumnDefinitions[index] : (DefinitionBase)grid.RowDefinitions[index];
        }

        private float GetActualLength(DefinitionBase definition)
        {
            //ColumnDefinition column = definition as ColumnDefinition;

            //return column == null ? ((RowDefinition)definition).ActualHeight : column.ActualWidth;
            return definition.SizeCache;
        }

        private static void SetDefinitionLength(DefinitionBase definition, GridLength length)
        {
            if (definition is ColumnDefinition)
            {
                ((ColumnDefinition)definition).WidthProperty = length;
            }
            else
                ((RowDefinition)definition).HeightProperty = length;
            //definition.SetValue(definition is ColumnDefinition ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty, length);
        }

        #endregion

        #endregion

        private void OnDragDelta(float delta)
        {
            if (_resizeData != null)
            {
                delta = (float)(Math.Round(delta / DragIncrement) * DragIncrement);

                MoveSplitter(delta);
            }
        }

        private void MoveSplitter(float change)
        {
            System.Diagnostics.Debug.Assert(_resizeData != null, "_resizeData should not be null when calling MoveSplitter");

            DefinitionBase definition1 = _resizeData.Definition1;
            DefinitionBase definition2 = _resizeData.Definition2;
            if (definition1 != null && definition2 != null)
            {
                float actualLength1 = GetActualLength(definition1);
                float actualLength2 = GetActualLength(definition2);

                if (_resizeData.SplitBehavior == SplitBehavior.Split &&
                    !FloatUtil.AreClose(actualLength1 + actualLength2, _resizeData.OriginalDefinition1ActualLength + _resizeData.OriginalDefinition2ActualLength))
                {
                    CancelResize();
                    return;
                }

                float min, max;
                GetDeltaConstraints(out min, out max);

                var delta = Math.Min(Math.Max(change, min), max);

                float definition1LengthNew = actualLength1 + delta;
                //double definition2LengthNew = actualLength2 - delta;
                float definition2LengthNew = actualLength1 + actualLength2 - definition1LengthNew;

                SetLengths(definition1LengthNew, definition2LengthNew);
            }
        }

        private void CancelResize()
        {
            Grid grid = Parent as Grid;

            SetDefinitionLength(_resizeData.Definition1, _resizeData.OriginalDefinition1Length);
            SetDefinitionLength(_resizeData.Definition2, _resizeData.OriginalDefinition2Length);

            _resizeData = null;
        }

        private void GetDeltaConstraints(out float minDelta, out float maxDelta)
        {
            float definition1Len = GetActualLength(_resizeData.Definition1);
            float definition1Min = _resizeData.Definition1.UserMinSize;// .UserMinSizeValueCache;
            float definition1Max = _resizeData.Definition1.UserMaxSize;// .UserMaxSizeValueCache;

            float definition2Len = GetActualLength(_resizeData.Definition2);
            float definition2Min = _resizeData.Definition2.UserMinSize;//.UserMinSizeValueCache;
            float definition2Max = _resizeData.Definition2.UserMaxSize;//.UserMaxSizeValueCache;

            if (_resizeData.SplitterIndex == _resizeData.Definition1Index)
            {
                definition1Min = Math.Max(definition1Min, _resizeData.SplitterLength);
            }
            else if (_resizeData.SplitterIndex == _resizeData.Definition2Index)
            {
                definition2Min = Math.Max(definition2Min, _resizeData.SplitterLength);
            }

            if (_resizeData.SplitBehavior == SplitBehavior.Split)
            {
                minDelta = -Math.Min(definition1Len - definition1Min, definition2Max - definition2Len);
                maxDelta = Math.Min(definition1Max - definition1Len, definition2Len - definition2Min);
            }
            else if (_resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                minDelta = definition1Min - definition1Len;
                maxDelta = definition1Max - definition1Len;
            }
            else
            {
                minDelta = definition2Len - definition2Max;
                maxDelta = definition2Len - definition2Min;
            }
        }

        private void SetLengths(float definition1Pixels, float definition2Pixels)
        {
            if (_resizeData.SplitBehavior == SplitBehavior.Split)
            {
                var definitions = _resizeData.ResizeDirection == GridResizeDirection.Columns ? _resizeData.Grid.ColumnDefinitions : _resizeData.Grid.RowDefinitions;

                int i = 0;
                foreach (var definition in definitions)
                {
                    if (i == _resizeData.Definition1Index)
                    {
                        SetDefinitionLength(definition, new GridLength(definition1Pixels, GridUnitType.Star));
                    }
                    else if (i == _resizeData.Definition2Index)
                    {
                        SetDefinitionLength(definition, new GridLength(definition2Pixels, GridUnitType.Star));
                    }
                    else if (IsStar(definition))
                    {
                        SetDefinitionLength(definition, new GridLength(GetActualLength(definition), GridUnitType.Star));
                    }

                    i++;
                }
            }
            else if (_resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                SetDefinitionLength(_resizeData.Definition1, new GridLength(definition1Pixels));
            }
            else
            {
                SetDefinitionLength(_resizeData.Definition2, new GridLength(definition2Pixels));
            }
        }

        protected override void InitializeBehaviorProcesses()
        {
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Down, GridSplitter_OnMouseLeftButtonDown, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_Mouse_Move, GridSplitter_OnMouseMove, WinBase.enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_LB_Up, WinBase_OnMouseLeftButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Down, WinBase_OnMouseRightButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_RB_Up, WinBase_OnMouseRightButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Down, WinBase_OnMouseMidButtonDown, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_MB_Up, WinBase_OnMouseMidButtonUp, enRoutingStrategy.Bubble);
            RegistBehaviorProcess(CCore.MsgProc.BehaviorType.BHT_WinSizeChanged, WinBase_OnPreWinSizeChanged, enRoutingStrategy.Tunnel);
        }

        private void GridSplitter_OnMouseLeftButtonDown(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (mDragEnable)
            {
                mDraging = true;
                mDragLocation = ((WinBase)Parent).GetLocalMousePoint();

                UISystem.Device.Mouse.Instance.Capture(this,bhInit.GetBehaviorType());

                InitializeData();

                eventArgs.Handled = true;
            }
        }

        private void GridSplitter_OnMouseMove(CCore.MsgProc.BehaviorParameter bhInit, UISystem.Message.RoutedEventArgs eventArgs)
        {
            if (Parent != null && mDraging)
            {
                if (_resizeData == null)
                    InitializeData();

                CSUtility.Support.Point ptMouse = ((WinBase)Parent).GetLocalMousePoint();
                switch (_resizeData.ResizeDirection)
                {
                    case GridResizeDirection.Columns:
                        {
                            var delta = ptMouse.X - mDragLocation.X;
                            OnDragDelta(delta);
                        }
                        break;

                    case GridResizeDirection.Rows:
                        {
                            var delta = ptMouse.Y - mDragLocation.Y;
                            OnDragDelta(delta);
                        }
                        break;
                }
                mDragLocation = ptMouse;

                eventArgs.Handled = true;
            }
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (State != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("WinState", "",holder);
                State.OnSave(stateNode,holder);
            }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ResizeDirection"))
                pXml.AddAttrib("ResizeDirection", ResizeDirection.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ResizeBehavior"))
                pXml.AddAttrib("ResizeBehavior", ResizeBehavior.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "DragIncrement"))
                pXml.AddAttrib("DragIncrement", DragIncrement.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlNode stateNode = pXml.FindNode("WinState");
            if (stateNode != null)
            {
                if (State == null)
                    mWinState = new WinState(this);
                State.OnLoad(stateNode);
            }

            var attr = pXml.FindAttrib("ResizeDirection");
            if (attr != null)
            {
                ResizeDirection = (GridResizeDirection)System.Enum.Parse(typeof(GridResizeDirection), attr.Value);
            }
            attr = pXml.FindAttrib("ResizeBehavior");
            if (attr != null)
            {
                ResizeBehavior = (GridResizeBehavior)System.Enum.Parse(typeof(GridResizeBehavior), attr.Value);
            }
            attr = pXml.FindAttrib("DragIncrement");
            if (attr != null)
            {
                DragIncrement = System.Convert.ToSingle(attr.Value);
            }
        }
    }
}
