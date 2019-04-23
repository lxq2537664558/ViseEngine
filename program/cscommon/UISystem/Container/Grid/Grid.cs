using System;
//using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections;

namespace UISystem.Container.Grid
{
    /// <summary>
    /// 布局时类型 
    /// </summary>
    [System.Flags]
    internal enum LayoutTimeSizeType : byte
    {
        None = 0x00,
        Pixel = 0x01,
        Auto = 0x02,
        Star = 0x04,
    }

    [CSUtility.Editor.UIEditor_Control("容器.Grid")]
    public class Grid : UISystem.Container.Container
    {
        /// <summary> 
        /// 比较函数
        /// </summary>
        /// <returns>
        /// </returns> 
        private static bool CompareNullRefs(object x, object y, out int result)
        {
            result = 2;

            if (x == null)
            {
                if (y == null)
                {
                    result = 0;
                }
                else
                {
                    result = -1;
                }
            }
            else
            {
                if (y == null)
                {
                    result = 1;
                }
            }

            return (result != 2);
        }

        private class SpanPreferredDistributionOrderComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                DefinitionBase definitionX = x as DefinitionBase;
                DefinitionBase definitionY = y as DefinitionBase;

                int result;

                if (!CompareNullRefs(definitionX, definitionY, out result))
                {
                    if (definitionX.UserSize.IsAuto)
                    {
                        if (definitionY.UserSize.IsAuto)
                        {
                            result = definitionX.MinSize.CompareTo(definitionY.MinSize);
                        }
                        else
                        {
                            result = -1;
                        }
                    }
                    else
                    {
                        if (definitionY.UserSize.IsAuto)
                        {
                            result = +1;
                        }
                        else
                        {
                            result = definitionX.PreferredSize.CompareTo(definitionY.PreferredSize);
                        }
                    }
                }

                return result;
            }
        }

        private class SpanMaxDistributionOrderComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                DefinitionBase definitionX = x as DefinitionBase;
                DefinitionBase definitionY = y as DefinitionBase;

                int result;

                if (!CompareNullRefs(definitionX, definitionY, out result))
                {
                    if (definitionX.UserSize.IsAuto)
                    {
                        if (definitionY.UserSize.IsAuto)
                        {
                            result = definitionX.SizeCache.CompareTo(definitionY.SizeCache);
                        }
                        else
                        {
                            result = +1;
                        }
                    }
                    else
                    {
                        if (definitionY.UserSize.IsAuto)
                        {
                            result = -1;
                        }
                        else
                        {
                            result = definitionX.SizeCache.CompareTo(definitionY.SizeCache);
                        }
                    }
                }

                return result;
            }
        }

        private class StarDistributionOrderComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                DefinitionBase definitionX = x as DefinitionBase;
                DefinitionBase definitionY = y as DefinitionBase;

                int result;

                if (!CompareNullRefs(definitionX, definitionY, out result))
                {
                    result = definitionX.SizeCache.CompareTo(definitionY.SizeCache);
                }

                return result;
            }
        }

        private class DistributionOrderComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                DefinitionBase definitionX = x as DefinitionBase;
                DefinitionBase definitionY = y as DefinitionBase;

                int result;

                if (!CompareNullRefs(definitionX, definitionY, out result))
                {
                    double xprime = definitionX.SizeCache - definitionX.MinSizeForArrange;
                    double yprime = definitionY.SizeCache - definitionY.MinSizeForArrange;
                    result = xprime.CompareTo(yprime);
                }

                return result;
            }
        }

        private class StarDistributionOrderIndexComparer : IComparer
        {
            private readonly DefinitionBase[] definitions;

            internal StarDistributionOrderIndexComparer(DefinitionBase[] definitions)
            {
                this.definitions = definitions;
            }

            public int Compare(object x, object y)
            {
                int? indexX = x as int?;
                int? indexY = y as int?;

                DefinitionBase definitionX = null;
                DefinitionBase definitionY = null;

                if (indexX != null && indexX.Value < definitions.Length)
                {
                    definitionX = definitions[indexX.Value];
                }
                if (indexY != null && indexY.Value < definitions.Length)
                {
                    definitionY = definitions[indexY.Value];
                }

                int result;

                if (!CompareNullRefs(definitionX, definitionY, out result))
                {
                    result = definitionX.SizeCache.CompareTo(definitionY.SizeCache);
                }

                return result;
            }
        }
        
        private class DistributionOrderIndexComparer : IComparer
        {
            private readonly DefinitionBase[] definitions;

            internal DistributionOrderIndexComparer(DefinitionBase[] definitions)
            {
                //Invariant.Assert(definitions != null);
                this.definitions = definitions;
            }

            public int Compare(object x, object y)
            {
                int? indexX = x as int?;
                int? indexY = y as int?;

                DefinitionBase definitionX = null;
                DefinitionBase definitionY = null;

                if (indexX != null)
                {
                    definitionX = definitions[indexX.Value];
                }
                if (indexY != null)
                {
                    definitionY = definitions[indexY.Value];
                }

                int result;

                if (!CompareNullRefs(definitionX, definitionY, out result))
                {
                    double xprime = definitionX.SizeCache - definitionX.MinSizeForArrange;
                    double yprime = definitionY.SizeCache - definitionY.MinSizeForArrange;
                    result = xprime.CompareTo(yprime);
                }

                return result;
            }
        }

        private enum Flags 
        { 
            ValidDefinitionsUStructure              = 0x00000001,
            ValidDefinitionsVStructure              = 0x00000002, 
            ValidCellsStructure                     = 0x00000004, 

            ShowGridLinesPropertyValue              = 0x00000100,   //  显示grid网格
 
            // bool flags
            ListenToNotifications                   = 0x00001000,   //  "0" 忽略所有通知
            SizeToContentU                          = 0x00002000,   //  "1" U方向按内容大小计算
            SizeToContentV                          = 0x00004000,   //  "1" V方向按内容大小计算
            HasStarCellsU                           = 0x00008000,   //  "1" 最少有一个cell属于按照star方式计算的列
            HasStarCellsV                           = 0x00010000,   //  "1" 最少有一个cell属于按照star方式计算的行
            HasGroup3CellsInAutoRows                = 0x00020000,   //  "1" 最少有一个group3的cell属于按照auto方式计算的行
            MeasureOverrideInProgress               = 0x00040000,   //  "1" MeasureOverride进行中
            ArrangeOverrideInProgress               = 0x00080000,   //  "1" ArrangeOverride进行中
        }
        private Flags _flags;
        private void SetFlags(bool value, Flags flags)
        {
            _flags = value ? (_flags | flags) : (_flags & (~flags));
        }
        private bool CheckFlagsAnd(Flags flags)
        {
            return ((_flags & flags) == flags);
        }
        private bool CheckFlagsOr(Flags flags)
        {
            return (flags == 0 || (_flags & flags) != 0);
        }
        internal bool MeasureOverrideInProgress
        {
            get { return (CheckFlagsAnd(Flags.MeasureOverrideInProgress)); }
            set { SetFlags(value, Flags.MeasureOverrideInProgress); }
        }
        internal bool ArrangeOverrideInProgress
        {
            get { return (CheckFlagsAnd(Flags.ArrangeOverrideInProgress)); }
            set { SetFlags(value, Flags.ArrangeOverrideInProgress); }
        }
        internal bool ColumnDefinitionCollectionDirty
        {
            get { return (!CheckFlagsAnd(Flags.ValidDefinitionsUStructure)); }
            set { SetFlags(!value, Flags.ValidDefinitionsUStructure); }
        }
        internal bool RowDefinitionCollectionDirty
        {
            get { return (!CheckFlagsAnd(Flags.ValidDefinitionsVStructure)); }
            set { SetFlags(!value, Flags.ValidDefinitionsVStructure); }
        }
        
        int[] _definitionIndices;

        private const float c_epsilon = 1e-5f;
        private static bool _IsZero(float d)
        {
            return (Math.Abs(d) < c_epsilon);
        }
        private static bool _AreClose(float d1, float d2)
        {
            return (Math.Asin(d1 - d2) < c_epsilon);
        }

        private const float c_starClip = float.MaxValue; // 1e298
        private static readonly LocalDataStoreSlot s_tempDefinitionsDataSlot = System.Threading.Thread.AllocateDataSlot();
        private static readonly IComparer s_spanPreferredDistributionOrderComparer = new SpanPreferredDistributionOrderComparer();
        private static readonly IComparer s_spanMaxDistributionOrderComparer = new SpanMaxDistributionOrderComparer();
        private static readonly IComparer s_starDistributionOrderComparer = new StarDistributionOrderComparer();
        private static readonly IComparer s_distributionOrderComparer = new DistributionOrderComparer();
        
        private struct CellCache
        {
            internal int ColumnIndex;
            internal int RowIndex;
            internal int ColumnSpan;
            internal int RowSpan;
            internal UISystem.Container.Grid.LayoutTimeSizeType SizeTypeU;
            internal UISystem.Container.Grid.LayoutTimeSizeType SizeTypeV;
            internal int Next;
            internal bool IsStarU { get { return ((SizeTypeU & UISystem.Container.Grid.LayoutTimeSizeType.Star) != 0); } }
            internal bool IsAutoU { get { return ((SizeTypeU & UISystem.Container.Grid.LayoutTimeSizeType.Auto) != 0); } }
            internal bool IsStarV { get { return ((SizeTypeV & UISystem.Container.Grid.LayoutTimeSizeType.Star) != 0); } }
            internal bool IsAutoV { get { return ((SizeTypeV & UISystem.Container.Grid.LayoutTimeSizeType.Auto) != 0); } }
        }

        /// <summary> 
        /// 哈希表key值跨度辅助类
        /// </summary>
        private class SpanKey
        {
            /// <summary> 
            /// 跨度起始索引
            /// </summary> 
            internal int Start { get { return (_start); } }

            /// <summary> 
            /// 跨度数
            /// </summary>
            internal int Count { get { return (_count); } }

            /// <summary>
            /// true - 列跨度
            /// false - 行跨度
            /// </summary>
            internal bool U { get { return (_u); } }

            private int _start;
            private int _count;
            private bool _u; 

            /// <summary>
            /// </summary> 
            /// <param name="start">起始索引</param>
            /// <param name="count">跨度</param> 
            /// <param name="u"><c>true</c> 列; <c>false</c> 行.</param> 
            internal SpanKey(int start, int count, bool u)
            {
                _start = start;
                _count = count;
                _u = u;
            }

            /// <summary> 
            /// <see cref="object.GetHashCode"/> 
            /// </summary>
            public override int GetHashCode()
            {
                int hash = (_start ^ (_count << 2));

                if (_u) hash &= 0x7ffffff;
                else hash |= 0x8000000;

                return (hash);
            }

            /// <summary>
            /// <see cref="object.Equals"/>
            /// </summary>
            public override bool Equals(object obj)
            {
                SpanKey sk = obj as SpanKey;
                return (sk != null
                        && sk._start == _start
                        && sk._count == _count
                        && sk._u == _u);
            }
        }

        private class ExtendedData
        {
            internal CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase> ColumnDefinitions;
            internal CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase> RowDefinitions;
            internal UISystem.Container.Grid.DefinitionBase[] DefinitionsU;
            internal UISystem.Container.Grid.DefinitionBase[] DefinitionsV;
            internal CellCache[] CellCachesCollection;
            internal int CellGroup1;
            internal int CellGroup2;
            internal int CellGroup3;
            internal int CellGroup4;
            internal UISystem.Container.Grid.DefinitionBase[] TempDefinitions;
        }

        ExtendedData _data = new ExtendedData();
        private ExtendedData ExtData
        {
            get { return _data; }
        }

        private CellCache[] PrivateCells
        {
            get { return ExtData.CellCachesCollection; }
        }
        private UISystem.Container.Grid.DefinitionBase[] DefinitionsU
        {
            get { return ExtData.DefinitionsU; }
        }
        private UISystem.Container.Grid.DefinitionBase[] DefinitionsV
        {
            get { return ExtData.DefinitionsV; }
        }

        /// <summary>
        /// 布局阶段缓存Definition数组
        /// </summary> 
        private DefinitionBase[] TempDefinitions
        {
            get
            {
                ExtendedData extData = ExtData;

                if (extData.TempDefinitions == null)
                {
                    int requiredLength = Math.Max(DefinitionsU.Length, DefinitionsV.Length);
                    WeakReference tempDefinitionsWeakRef = (WeakReference)System.Threading.Thread.GetData(s_tempDefinitionsDataSlot);
                    if (tempDefinitionsWeakRef == null)
                    {
                        extData.TempDefinitions = new DefinitionBase[requiredLength * 2];
                        System.Threading.Thread.SetData(s_tempDefinitionsDataSlot, new WeakReference(extData.TempDefinitions));
                    }
                    else
                    {
                        extData.TempDefinitions = (DefinitionBase[])tempDefinitionsWeakRef.Target;
                        if (extData.TempDefinitions == null || extData.TempDefinitions.Length < requiredLength)
                        {
                            extData.TempDefinitions = new DefinitionBase[requiredLength * 2];
                            tempDefinitionsWeakRef.Target = extData.TempDefinitions;
                        }
                    }
                }

                return extData.TempDefinitions;
            }
        }

        /// <summary>
        /// 缓存Definition索引
        /// </summary> 
        private int[] DefinitionIndices
        {
            get
            {
                int requiredLength = Math.Max(DefinitionsU.Length, DefinitionsV.Length);

                if (_definitionIndices == null && requiredLength == 0)
                {
                    _definitionIndices = new int[1];
                }
                else if (_definitionIndices == null || _definitionIndices.Length < requiredLength)
                {
                    _definitionIndices = new int[requiredLength];
                }

                return _definitionIndices;
            }
        }

        private bool HasStarCellsU
        {
            get{ return CheckFlagsAnd(Flags.HasStarCellsU); }
            set{ SetFlags(value, Flags.HasStarCellsU); }
        }
        private bool HasStarCellsV
        {
            get { return CheckFlagsAnd(Flags.HasStarCellsV); }
            set { SetFlags(value, Flags.HasStarCellsV); }
        }
        private bool HasGroup3CellsInAutoRows
        {
            get { return CheckFlagsAnd(Flags.HasGroup3CellsInAutoRows); }
            set { SetFlags(value, Flags.HasGroup3CellsInAutoRows); }
        }
        [Browsable(false)]
        public bool CellsStructureDirty
        {
            get { return (!CheckFlagsAnd(Flags.ValidCellsStructure)); }
            set { SetFlags(!value, Flags.ValidCellsStructure); }
        }
        private bool ListenToNotifications
        {
            get { return (CheckFlagsAnd(Flags.ListenToNotifications)); }
            set { SetFlags(value, Flags.ListenToNotifications); }
        }
        private bool SizeToContentU
        {
            get { return (CheckFlagsAnd(Flags.SizeToContentU)); }
            set { SetFlags(value, Flags.SizeToContentU); }
        }
        private bool SizeToContentV
        {
            get { return (CheckFlagsAnd(Flags.SizeToContentV)); }
            set { SetFlags(value, Flags.SizeToContentV); }
        } 

        //[Category("布局")]
        //[CSUtility.Editor.UIEditor_GridDefinitionEditor(CSUtility.Editor.UIEditor_GridDefinitionEditorAttribute.GridDefinitionType.Column)]
        [Browsable(false)]
        public CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase> ColumnDefinitions
        {
            get
            {
                if (_data == null) { _data = new ExtendedData(); }
                if (_data.ColumnDefinitions == null) { _data.ColumnDefinitions = new CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase>(); }

                return _data.ColumnDefinitions;
            }
            set
            {
                CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase> columns = new CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase>();
                foreach (ColumnDefinition column in value)
                {
                    ColumnDefinition colDef = new ColumnDefinition(this);
                    colDef.WidthProperty = GridLength.CopyFrom(column.WidthProperty);
                    columns.Add(colDef);
                }
                _data.ColumnDefinitions = columns;
                ColumnDefinitionCollectionDirty = true;
                CellsStructureDirty = true;
            }
        }
        //[Category("布局")]
        //[CSUtility.Editor.UIEditor_GridDefinitionEditor(CSUtility.Editor.UIEditor_GridDefinitionEditorAttribute.GridDefinitionType.Row)]
        [Browsable(false)]
        public CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase> RowDefinitions
        {
            get
            {
                if (_data == null) { _data = new ExtendedData(); }
                if (_data.RowDefinitions == null) { _data.RowDefinitions = new CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase>(); }

                return _data.RowDefinitions;
            }
            set
            {
                CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase> rows = new CSUtility.Support.ThreadSafeObservableCollection<DefinitionBase>();
                foreach (RowDefinition row in value)
                {
                    RowDefinition rowDef = new RowDefinition(this);
                    rowDef.HeightProperty = GridLength.CopyFrom(row.HeightProperty);
                    rows.Add(rowDef);
                }
                _data.RowDefinitions = rows;
                RowDefinitionCollectionDirty = true;
                CellsStructureDirty = true;
            }
        }

        public Grid()
        {
            //for (int i = 0; i < 3; i++)
            //{
            //    GridLength c = new GridLength(10, GridUnitType.Pixel);
            //    GridSplit.Columns.Add(c);
            //}
            //GridSplit = new UISystem.Container.Grid.GridSplit(this);
            ContainerType = enContainerType.Multi;

            //GridSplit.OnPropertyValueChanged = new GridSplit.Delegate_OnPropertyValueChanged(GridSplitUpdated);
        }

        protected override void OnAddChild(WinBase child)
        {
            base.OnAddChild(child);
            Invalidate();
            UpdateLayout();
        }

        protected override void RemoveChild(WinBase child)
        {
            base.RemoveChild(child);
            Invalidate();
        }

        protected void GridSplitUpdated()
        {
            ValidateCellsCore();
            UpdateLayout();
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            //if (GridSplit.Columns.Count > 1)
            //{
            //    var columnsNode = pXml.AddNode("Grid.ColumnDefinitions", "");
            //    foreach (var column in GridSplit.Columns)
            //    {
            //        var columnNode = columnsNode.AddNode("ColumnDefinition", "");
            //        var cAtt = columnNode.AddAttrib("Width", column.ToString());
            //    }
            //}

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ClipWithCell"))
                pXml.AddAttrib("ClipWithCell", ClipWithCell.ToString());

            if(ColumnDefinitions.Count > 1)
            {
                var columnsNode = pXml.AddNode("Grid.ColumnDefinitions", "",holder);
                foreach (var definition in ColumnDefinitions)
                {
                    var columnNode = columnsNode.AddNode("ColumnDefinition", "",holder);
                    columnNode.AddAttrib("Width", definition.UserSize.ToString());
                    if (!FloatUtil.AreClose(definition.UserSize.MinLength, 0))
                        columnNode.AddAttrib("MinWidth", definition.UserSize.MinLength.ToString());
                    if (!FloatUtil.AreClose(definition.UserSize.MaxLength, float.PositiveInfinity))
                        columnNode.AddAttrib("MaxWidth", definition.UserSize.MaxLength.ToString());
                }
            }

            //if (GridSplit.Rows.Count > 1)
            //{
            //    var rowsNode = pXml.AddNode("Grid.RowDefinitions", "");
            //    foreach (var row in GridSplit.Rows)
            //    {
            //        var rowNode = rowsNode.AddNode("RowDefinition", "");
            //        var cAtt = rowNode.AddAttrib("Height", row.ToString());
            //    }
            //}
            if (RowDefinitions.Count > 1)
            {
                var rowsNode = pXml.AddNode("Grid.RowDefinitions", "",holder);
                foreach (var definition in RowDefinitions)
                {
                    var rowNode = rowsNode.AddNode("RowDefinition", "",holder);
                    rowNode.AddAttrib("Height", definition.UserSize.ToString());
                    if (!FloatUtil.AreClose(definition.UserSize.MinLength, 0))
                        rowNode.AddAttrib("MinHeight", definition.UserSize.MinLength.ToString());
                    if (!FloatUtil.AreClose(definition.UserSize.MaxLength, float.PositiveInfinity))
                        rowNode.AddAttrib("MaxHeight", definition.UserSize.MaxLength.ToString());
                }
            }
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            var attr = pXml.FindAttrib("ClipWithCell");
            if (attr != null)
                ClipWithCell = System.Convert.ToBoolean(attr.Value);

            //GridSplit.Clear();

            //var columnsNode = pXml.FindNode("Grid.ColumnDefinitions");
            //if (columnsNode != null)
            //{
            //    GridSplit.Columns.Clear();
            //    var cNodes = columnsNode.FindNodes("ColumnDefinition");
            //    foreach (var cNode in cNodes)
            //    {
            //        var cAtt = cNode.FindAttrib("Width");
            //        var def = GridLength.Parse(cAtt.Value);
            //        GridSplit.Columns.Add(def);
            //    }
            //}
            ColumnDefinitions.Clear();
            var columnsNode = pXml.FindNode("Grid.ColumnDefinitions");
            if (columnsNode != null)
            {
                foreach (var cNode in columnsNode.FindNodes("ColumnDefinition"))
                {
                    var definition = new ColumnDefinition(this);
                    var cAtt = cNode.FindAttrib("Width");
                    if (cAtt != null)
                    {
                        definition.UserSize.ParseStr(cAtt.Value);
                    }
                    cAtt = cNode.FindAttrib("MinWidth");
                    if (cAtt != null)
                    {
                        definition.UserSize.MinLength = System.Convert.ToSingle(cAtt.Value);
                    }
                    cAtt = cNode.FindAttrib("MaxWidth");
                    if (cAtt != null)
                    {
                        definition.UserSize.MaxLength = System.Convert.ToSingle(cAtt.Value);
                    }

                    ColumnDefinitions.Add(definition);
                }
            }
            ColumnDefinitionCollectionDirty = true;
            
            //var rowNode = pXml.FindNode("Grid.RowDefinitions");
            //if (rowNode != null)
            //{
            //    GridSplit.Rows.Clear();
            //    var cNodes = rowNode.FindNodes("RowDefinition");
            //    foreach (var cNode in cNodes)
            //    {
            //        var cAtt = cNode.FindAttrib("Height");
            //        var def = GridLength.Parse(cAtt.Value);
            //        GridSplit.Rows.Add(def);
            //    }
            //}
            RowDefinitions.Clear();
            var rowsNode = pXml.FindNode("Grid.RowDefinitions");
            if (rowsNode != null)
            {
                foreach (var cNode in rowsNode.FindNodes("RowDefinition"))
                {
                    var definition = new RowDefinition(this);
                    var cAtt = cNode.FindAttrib("Height");
                    if (cAtt != null)
                    {
                        definition.UserSize.ParseStr(cAtt.Value);
                    }
                    cAtt = cNode.FindAttrib("MinHeight");
                    if (cAtt != null)
                    {
                        definition.UserSize.MinLength = System.Convert.ToSingle(cAtt.Value);
                    }
                    cAtt = cNode.FindAttrib("MaxHeight");
                    if (cAtt != null)
                    {
                        definition.UserSize.MaxLength = System.Convert.ToSingle(cAtt.Value);
                    }

                    RowDefinitions.Add(definition);
                }
            }
            RowDefinitionCollectionDirty = true;
        }

        public override void RenderAssist(UIRenderPipe pipe, int zOrder, CSUtility.Support.Point parentLoc)
        {
            if (mIsInTemplate)
                return;

            parentLoc.X += Left;
            parentLoc.Y += Top;

            if (DefinitionsU != null)
            {
                for (int i = 0; i < DefinitionsU.Length; i++)
                {
                    if (i == 0)
                        continue;

                    int offset = (int)DefinitionsU[i].FinalOffset;
                    UISystem.IRender.GetInstance().DrawLine(pipe, zOrder, new CSUtility.Support.Point(parentLoc.X + offset, parentLoc.Y),
                                                            new CSUtility.Support.Point(parentLoc.X + offset, parentLoc.Y + Height),
                                                            CSUtility.Support.Color.Blue);
                }
            }
            if (DefinitionsV != null)
            {
                for (int i = 0; i < DefinitionsV.Length; i++)
                {
                    if (i == 0)
                        continue;

                    int offset = (int)DefinitionsV[i].FinalOffset;
                    UISystem.IRender.GetInstance().DrawLine(pipe, zOrder, new CSUtility.Support.Point(parentLoc.X, parentLoc.Y + offset),
                                                            new CSUtility.Support.Point(parentLoc.X + Width, parentLoc.Y + offset),
                                                            CSUtility.Support.Color.Blue);
                }
            }

            //int x = parentLoc.X, y = parentLoc.Y;
            //for (int i = 0; i < GridSplit.Columns.Count - 1; i++ )
            //{
            //    var column = GridSplit.Columns[i];
            //    UISystem.IRender.GetInstance().DrawLine(new CSUtility.Support.Point(x + (int)(column.ActualLength), parentLoc.Y),
            //                                            new CSUtility.Support.Point(x + (int)(column.ActualLength), parentLoc.Y + Height),
            //                                            CSUtility.Support.Color.Blue);
            //    x += (int)column.ActualLength;
            //}

            //for (int i = 0; i < GridSplit.Rows.Count - 1; i++)
            //{
            //    var row = GridSplit.Rows[i];
            //    UISystem.IRender.GetInstance().DrawLine(new CSUtility.Support.Point(parentLoc.X, y + (int)(row.ActualLength)),
            //                                            new CSUtility.Support.Point(parentLoc.X + Width, y + (int)(row.ActualLength)),
            //                                            CSUtility.Support.Color.Blue);
            //    y += (int)row.ActualLength;
            //}

            foreach (var child in ChildWindows)
            {
                child.RenderAssist(pipe, zOrder + 10, parentLoc);
            }
        }

        private LayoutTimeSizeType GetLengthTypeForRange(DefinitionBase[] definitions, int start, int count)
        {
            System.Diagnostics.Debug.Assert(0 < count && 0 <= start && (start + count) <= definitions.Length);
            //if (count <= 0)
            //    count = 1;
            //if (start <= 0)
            //    start = 0;
            //if (start + count >= definitions.Length)
            //{
            //    start = 0;
            //    if (definitions.Length > 0)
            //        count = definitions.Length;
            //    else
            //        count = 1;
            //}

            var lengthType = LayoutTimeSizeType.None;
            int i = start + count - 1;

            do 
            {
                lengthType |= definitions[i].SizeType;
            } while (--i >= start);

            return lengthType;
        }
        
        private void EnsureMinSizeInDefinitionRange(
            DefinitionBase[] definitions,
            int start,
            int count,
            float requestedSize,
            float percentReferenceSize)
        {
            Debug.Assert(1 < count && 0 <= start && (start + count) <= definitions.Length);

            if (!_IsZero(requestedSize))
            {
                DefinitionBase[] tempDefinitions = TempDefinitions; //  temp array used to remember definitions for sorting
                int end = start + count;
                int autoDefinitionsCount = 0;
                float rangeMinSize = 0;
                float rangePreferredSize = 0;
                float rangeMaxSize = 0;
                float maxMaxSize = 0;

                //  获取必要的信息
                //  a) 计算大小总和
                //  b) 计算使用auto方式的definition的数量
                //  c) 初始化临时数组
                //  d) 在SizeCache中缓存最大大小
                //  e) 累加最大尺寸
                for (int i = start; i < end; ++i)
                {
                    float minSize = definitions[i].MinSize;
                    float preferredSize = definitions[i].PreferredSize;
                    float maxSize = Math.Max(definitions[i].UserMaxSize, minSize);

                    rangeMinSize += minSize;
                    rangePreferredSize += preferredSize;
                    rangeMaxSize += maxSize;

                    definitions[i].SizeCache = maxSize;

                    Debug.Assert(minSize <= preferredSize
                                && preferredSize <= maxSize
                                && rangeMinSize <= rangePreferredSize
                                && rangePreferredSize <= rangeMaxSize);

                    if (maxMaxSize < maxSize) maxMaxSize = maxSize;
                    if (definitions[i].UserSize.IsAuto) autoDefinitionsCount++;
                    tempDefinitions[i - start] = definitions[i]; 
                }

                if (requestedSize > rangeMinSize)
                {
                    if (requestedSize <= rangePreferredSize)
                    {
                        float sizeToDistribute;
                        int i;

                        Array.Sort(tempDefinitions, 0, count, s_spanPreferredDistributionOrderComparer);
                        for (i = 0, sizeToDistribute = requestedSize; i < autoDefinitionsCount; ++i)
                        {
                            Debug.Assert(tempDefinitions[i].UserSize.IsAuto);

                            sizeToDistribute -= (tempDefinitions[i].MinSize);
                        }

                        for (; i < count; ++i)
                        {
                            Debug.Assert(!tempDefinitions[i].UserSize.IsAuto);

                            float newMinSize = Math.Min(sizeToDistribute / (count - i), tempDefinitions[i].PreferredSize);
                            if (newMinSize > tempDefinitions[i].MinSize) { tempDefinitions[i].UpdateMinSize(newMinSize); }
                            sizeToDistribute -= newMinSize;
                        }

                        //Debug.Assert(_IsZero(sizeToDistribute));
                    }
                    else if (requestedSize <= rangeMaxSize)
                    {
                        float sizeToDistribute;
                        int i;

                        Array.Sort(tempDefinitions, 0, count, s_spanMaxDistributionOrderComparer);
                        for (i = 0, sizeToDistribute = requestedSize - rangePreferredSize; i < count - autoDefinitionsCount; ++i)
                        {
                            Debug.Assert(!tempDefinitions[i].UserSize.IsAuto);

                            float preferredSize = tempDefinitions[i].PreferredSize;
                            float newMinSize = preferredSize + sizeToDistribute / (count - autoDefinitionsCount - i);
                            tempDefinitions[i].UpdateMinSize(Math.Min(newMinSize, tempDefinitions[i].SizeCache));
                            sizeToDistribute -= (tempDefinitions[i].MinSize - preferredSize);
                        }

                        for (; i < count; ++i)
                        {
                            Debug.Assert(tempDefinitions[i].UserSize.IsAuto);

                            float preferredSize = tempDefinitions[i].MinSize;
                            float newMinSize = preferredSize + sizeToDistribute / (count - i);
                            tempDefinitions[i].UpdateMinSize(Math.Min(newMinSize, tempDefinitions[i].SizeCache));
                            sizeToDistribute -= (tempDefinitions[i].MinSize - preferredSize);
                        }

                        //Debug.Assert(_IsZero(sizeToDistribute));
                    }
                    else
                    {
                        float equalSize = requestedSize / count;

                        if (equalSize < maxMaxSize
                            && !_AreClose(equalSize, maxMaxSize))
                        {
                            float totalRemainingSize = maxMaxSize * count - rangeMaxSize;
                            float sizeToDistribute = requestedSize - rangeMaxSize;

                            Debug.Assert(!float.IsInfinity(totalRemainingSize)
                                        && !FloatUtil.IsNaN(totalRemainingSize)
                                        && totalRemainingSize > 0
                                        && !float.IsInfinity(sizeToDistribute)
                                        && !FloatUtil.IsNaN(sizeToDistribute)
                                        && sizeToDistribute > 0);

                            for (int i = 0; i < count; ++i)
                            {
                                float deltaSize = (maxMaxSize - tempDefinitions[i].SizeCache) * sizeToDistribute / totalRemainingSize;
                                tempDefinitions[i].UpdateMinSize(tempDefinitions[i].SizeCache + deltaSize);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                tempDefinitions[i].UpdateMinSize(equalSize);
                            }
                        }
                    }
                }
            }
        }

        private void ResolveStar(
            DefinitionBase[] definitions,
            float availableSize)
        {
            DefinitionBase[] tempDefinitions = TempDefinitions;
            int starDefinitionsCount = 0;
            float takenSize = 0;

            for (int i = 0; i < definitions.Length; ++i )
            {
                switch (definitions[i].SizeType)
                {
                    case LayoutTimeSizeType.Auto:
                        takenSize += definitions[i].MinSize;
                        break;
                    case LayoutTimeSizeType.Pixel:
                        takenSize += definitions[i].MeasureSize;
                        break;
                    case LayoutTimeSizeType.Star:
                        {
                            tempDefinitions[starDefinitionsCount++] = definitions[i];

                            float starValue = definitions[i].UserSize.Value;

                            if (_IsZero(starValue))
                            {
                                definitions[i].MeasureSize = 0;
                                definitions[i].SizeCache = 0;
                            }
                            else
                            {
                                starValue = Math.Min(starValue, c_starClip);

                                definitions[i].MeasureSize = starValue;
                                float maxSize = Math.Max(definitions[i].MinSize, definitions[i].UserMaxSize);
                                maxSize = Math.Min(maxSize, c_starClip);
                                definitions[i].SizeCache = maxSize / starValue;
                            }
                        }
                        break;
                }
            }

            if (starDefinitionsCount > 0)
            {
                Array.Sort(tempDefinitions, 0, starDefinitionsCount, s_starDistributionOrderComparer);

                float allStarWeights = 0;
                int i = starDefinitionsCount - 1;
                do
                {
                    var def = tempDefinitions[i];
                    if (def != null)
                    {
                        allStarWeights += def.MeasureSize;
                        def.SizeCache = allStarWeights;
                    }
                } while (--i >= 0);

                i = 0;
                do
                {
                    var def = tempDefinitions[i];
                    if (def != null)
                    {
                        float resolvedSize;
                        float starValue = def.MeasureSize;

                        if (_IsZero(starValue))
                        {
                            resolvedSize = def.MinSize;
                        }
                        else
                        {
                            float userSize = Math.Max(availableSize - takenSize, 0.0f) * (starValue / def.SizeCache);
                            resolvedSize = Math.Min(userSize, def.UserMaxSize);
                            resolvedSize = Math.Max(def.MinSize, resolvedSize);
                        }

                        def.MeasureSize = resolvedSize;
                        takenSize += resolvedSize;
                    }


                } while (++i < starDefinitionsCount);
            }
        }

        private double CalculateDesiredSize(
            DefinitionBase[] definitions)
        {
            double desiredSize = 0;

            for (int i = 0; i < definitions.Length; ++i)
            {
                desiredSize += definitions[i].MinSize;
            }

            return (desiredSize);
        }

        private void SetFinalSize(
            DefinitionBase[] definitions,
            float finalSize,
            bool columns)
        {
            int starDefinitionsCount = 0;                      
            int nonStarIndex = definitions.Length;                  
            float allPreferredArrangeSize = 0;
            //bool useLayoutRounding = this.UseLayoutRounding;
            int[] definitionIndices = DefinitionIndices;
            //float[] roundingErrors = null;

            for (int i = 0; i < definitions.Length; ++i)
            {
                //  共享的definition不能为star模式
                Debug.Assert(!definitions[i].IsShared || !definitions[i].UserSize.IsStar);

                if (definitions[i].UserSize.IsStar)
                {
                    float starValue = definitions[i].UserSize.Value;

                    if (_IsZero(starValue))
                    {
                        definitions[i].MeasureSize = 0;
                        definitions[i].SizeCache = 0; 
                    }
                    else
                    {
                        starValue = Math.Min(starValue, c_starClip);

                        definitions[i].MeasureSize = starValue;
                        float maxSize = Math.Max(definitions[i].MinSizeForArrange, definitions[i].UserMaxSize);
                        maxSize = Math.Min(maxSize, c_starClip);
                        definitions[i].SizeCache = maxSize / starValue;
                        //if (useLayoutRounding)
                        //{
                        //    roundingErrors[i] = definitions[i].SizeCache;
                        //    definitions[i].SizeCache = UIElement.RoundLayoutValue(definitions[i].SizeCache, dpi);
                        //}
                    }
                    definitionIndices[starDefinitionsCount++] = i;
                }
                else
                {
                    float userSize = 0;

                    switch (definitions[i].UserSize.GridUnitType)
                    {
                        case GridUnitType.Pixel:
                            userSize = definitions[i].UserSize.Value;
                            break;

                        case GridUnitType.Auto:
                            userSize = definitions[i].MinSizeForArrange;
                            break;
                    }

                    float userMaxSize;

                    if (definitions[i].IsShared)
                    {
                        userMaxSize = userSize;
                    }
                    else
                    {
                        userMaxSize = definitions[i].UserMaxSize;
                    }

                    definitions[i].SizeCache = Math.Max(definitions[i].MinSizeForArrange, Math.Min(userSize, userMaxSize));
                    //if (useLayoutRounding)
                    //{
                    //    roundingErrors[i] = definitions[i].SizeCache;
                    //    definitions[i].SizeCache = UIElement.RoundLayoutValue(definitions[i].SizeCache, dpi);
                    //}

                    allPreferredArrangeSize += definitions[i].SizeCache;
                    definitionIndices[--nonStarIndex] = i;
                }
            }

            Debug.Assert(nonStarIndex == starDefinitionsCount);

            if (starDefinitionsCount > 0)
            {
                StarDistributionOrderIndexComparer starDistributionOrderIndexComparer = new StarDistributionOrderIndexComparer(definitions);
                Array.Sort(definitionIndices, 0, starDefinitionsCount, starDistributionOrderIndexComparer);

                float allStarWeights = 0;
                int i = starDefinitionsCount - 1;
                do
                {
                    allStarWeights += definitions[definitionIndices[i]].MeasureSize;
                    definitions[definitionIndices[i]].SizeCache = allStarWeights;
                } while (--i >= 0);

                i = 0;
                do
                {
                    float resolvedSize;
                    float starValue = definitions[definitionIndices[i]].MeasureSize;

                    if (_IsZero(starValue))
                    {
                        resolvedSize = definitions[definitionIndices[i]].MinSizeForArrange;
                    }
                    else
                    {
                        float userSize = Math.Max(finalSize - allPreferredArrangeSize, 0.0f) * (starValue / definitions[definitionIndices[i]].SizeCache);
                        resolvedSize = Math.Min(userSize, definitions[definitionIndices[i]].UserMaxSize);
                        resolvedSize = Math.Max(definitions[definitionIndices[i]].MinSizeForArrange, resolvedSize);
                    }

                    definitions[definitionIndices[i]].SizeCache = resolvedSize;
                    //if (useLayoutRounding)
                    //{
                    //    roundingErrors[definitionIndices[i]] = definitions[definitionIndices[i]].SizeCache;
                    //    definitions[definitionIndices[i]].SizeCache = UIElement.RoundLayoutValue(definitions[definitionIndices[i]].SizeCache, dpi);
                    //}

                    allPreferredArrangeSize += definitions[definitionIndices[i]].SizeCache;
                } while (++i < starDefinitionsCount);
            }

            if (allPreferredArrangeSize > finalSize
                && !_AreClose(allPreferredArrangeSize, finalSize))
            {
                DistributionOrderIndexComparer distributionOrderIndexComparer = new DistributionOrderIndexComparer(definitions);
                Array.Sort(definitionIndices, 0, definitions.Length, distributionOrderIndexComparer);
                float sizeToDistribute = finalSize - allPreferredArrangeSize;

                for (int i = 0; i < definitions.Length; ++i)
                {
                    int definitionIndex = definitionIndices[i];
                    float final = definitions[definitionIndex].SizeCache + (sizeToDistribute / (definitions.Length - i));
                    float finalOld = final;
                    final = Math.Max(final, definitions[definitionIndex].MinSizeForArrange);
                    final = Math.Min(final, definitions[definitionIndex].SizeCache);

                    //if (useLayoutRounding)
                    //{
                    //    roundingErrors[definitionIndex] = final;
                    //    final = UIElement.RoundLayoutValue(finalOld, dpi);
                    //    final = Math.Max(final, definitions[definitionIndex].MinSizeForArrange);
                    //    final = Math.Min(final, definitions[definitionIndex].SizeCache);
                    //}

                    sizeToDistribute -= (final - definitions[definitionIndex].SizeCache);
                    definitions[definitionIndex].SizeCache = final;
                }

                allPreferredArrangeSize = finalSize - sizeToDistribute;
            }

            definitions[0].FinalOffset = 0.0f;
            for (int i = 0; i < definitions.Length; ++i)
            {
                definitions[(i + 1) % definitions.Length].FinalOffset = definitions[i].FinalOffset + definitions[i].SizeCache;
            }
        }

        private double GetFinalSizeForRange(
            DefinitionBase[] definitions,
            int start,
            int count)
        {
            if (definitions == null)
                return 0;

            double size = 0;
            int i = start + count - 1;

            do
            {
                size += definitions[i].SizeCache;
            } while (--i >= start);

            return (size);
        }

        private void SetValid()
        {
            ExtendedData extData = ExtData;
            if (extData != null)
            {
                if (extData.TempDefinitions != null)
                {
                    Array.Clear(extData.TempDefinitions, 0, Math.Max(DefinitionsU.Length, DefinitionsV.Length));
                    extData.TempDefinitions = null;
                }
            }
        }

        internal void Invalidate()
        {
            CellsStructureDirty = true;
            InvalidateMeasure();
        }

        private void ValidateCells()
        {
            if (CellsStructureDirty)
            {
                ValidateCellsCore();
                CellsStructureDirty = false;
            }
        }

        private void ValidateCellsCore()
        {
            ExtendedData extData = ExtData;

            extData.CellCachesCollection = new CellCache[ChildWindows.Count];
            extData.CellGroup1 = int.MaxValue;
            extData.CellGroup2 = int.MaxValue;
            extData.CellGroup3 = int.MaxValue;
            extData.CellGroup4 = int.MaxValue;

            bool hasStarCellsU = false;
            bool hasStarCellsV = false;
            bool hasGroup3CellsInAutoRows = false;

            for (int i = PrivateCells.Length - 1; i >= 0; --i)
            {
                var child = ChildWindows[i];
                if (child == null)
                    continue;

                CellCache cell = new CellCache();
                cell.ColumnIndex = Math.Min(child.GridColumn, DefinitionsU.Length - 1);
                cell.ColumnSpan = Math.Min(child.GridColumnSpan, DefinitionsU.Length - cell.ColumnIndex);
                cell.RowIndex = Math.Min(child.GridRow, DefinitionsV.Length - 1);
                cell.RowSpan = Math.Min(child.GridRowSpan, DefinitionsV.Length - cell.RowIndex);

                System.Diagnostics.Debug.Assert(0 <= cell.ColumnIndex && cell.ColumnIndex < DefinitionsU.Length);
                System.Diagnostics.Debug.Assert(0 <= cell.RowIndex && cell.RowIndex < DefinitionsV.Length);

                cell.SizeTypeU = GetLengthTypeForRange(DefinitionsU, cell.ColumnIndex, cell.ColumnSpan);
                cell.SizeTypeV = GetLengthTypeForRange(DefinitionsV, cell.RowIndex, cell.RowSpan);

                hasStarCellsU |= cell.IsStarU;
                hasStarCellsV |= cell.IsStarV;

                if (!cell.IsStarV)
                {
                    if (!cell.IsStarU)
                    {
                        cell.Next = extData.CellGroup1;
                        extData.CellGroup1 = i;
                    }
                    else
                    {
                        cell.Next = extData.CellGroup3;
                        extData.CellGroup3 = i;

                        hasGroup3CellsInAutoRows |= cell.IsAutoV;
                    }
                }
                else
                {
                    if (cell.IsAutoU && !cell.IsStarU)
                    {
                        cell.Next = extData.CellGroup2;
                        extData.CellGroup2 = i;
                    }
                    else
                    {
                        cell.Next = extData.CellGroup4;
                        extData.CellGroup4 = i;
                    }
                }

                PrivateCells[i] = cell;
            }

            HasStarCellsU = hasStarCellsU;
            HasStarCellsV = hasStarCellsV;
            HasGroup3CellsInAutoRows = hasGroup3CellsInAutoRows;
        }

        private void ValidateDefinitionsUStructure()
        {
            if (ColumnDefinitionCollectionDirty)
            {
                ExtendedData extData = ExtData;

                if (extData.ColumnDefinitions == null)
                {
                    if (extData.DefinitionsU == null)
                    {
                        extData.DefinitionsU = new DefinitionBase[] { new ColumnDefinition(this) };
                    }
                }
                else
                {
                    if(extData.ColumnDefinitions.Count == 0)
                    {
                        extData.DefinitionsU = new DefinitionBase[] { new ColumnDefinition(this) };
                    }
                    else
                    {
                        extData.DefinitionsU = extData.ColumnDefinitions.ToArray();
                    }
                }

                ColumnDefinitionCollectionDirty = false;
            }

            System.Diagnostics.Debug.Assert(ExtData.DefinitionsU != null && ExtData.DefinitionsU.Length > 0);
        }

        private void ValidateDefinitionsVStructure()
        {
            if (RowDefinitionCollectionDirty)
            {
                ExtendedData extData = ExtData;

                if (extData.RowDefinitions == null)
                {
                    if (extData.DefinitionsV == null)
                        extData.DefinitionsV = new DefinitionBase[] { new RowDefinition(this) };
                }
                else
                {
                    if (extData.RowDefinitions.Count == 0)
                    {
                        extData.DefinitionsV = new DefinitionBase[] { new RowDefinition(this) };
                    }
                    else
                    {
                        extData.DefinitionsV = extData.RowDefinitions.ToArray();
                    }
                }

                RowDefinitionCollectionDirty = false;
            }

            System.Diagnostics.Debug.Assert(ExtData.DefinitionsV != null && ExtData.DefinitionsV.Length > 0);
        }

        private void ValidateDefinitionsLayout(DefinitionBase[] definitions, bool treatStarAsAuto)
        {
            for (int i = 0; i < definitions.Length; ++i)
            {
                definitions[i].OnBeforeLayout(this);

                float userMinSize = definitions[i].UserMinSize;
                float userMaxSize = definitions[i].UserMaxSize;
                float userSize = 0;

                switch (definitions[i].UserSize.GridUnitType)
                {
                    case GridUnitType.Pixel:
                        definitions[i].SizeType = LayoutTimeSizeType.Pixel;
                        userSize = definitions[i].UserSize.Value;
                        userMinSize = Math.Max(userMinSize, Math.Min(userSize, userMaxSize));
                        break;
                    case GridUnitType.Auto:
                        definitions[i].SizeType = LayoutTimeSizeType.Auto;
                        userSize = float.PositiveInfinity;
                        break;
                    case GridUnitType.Star:
                        if (treatStarAsAuto)
                        {
                            definitions[i].SizeType = LayoutTimeSizeType.Auto;
                            userSize = float.PositiveInfinity;
                        }
                        else
                        {
                            definitions[i].SizeType = LayoutTimeSizeType.Star;
                            userSize = float.PositiveInfinity;
                        }
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }

                definitions[i].UpdateMinSize(userMinSize);
                definitions[i].MeasureSize = Math.Max(userMinSize, Math.Min(userSize, userMaxSize));
            }
        }

        
        private void MeasureCellsGroup(
            int cellsHead,
            SlimDX.Size referenceSize,
            bool ignoreDesiredSizeU,
            bool forceInfinityV)
        {
            if (cellsHead >= PrivateCells.Length)
            {
                return;
            }

            var children = ChildWindows;
            if (children.Count == 0)
                return;

            Hashtable spanStore = null;
            bool ignoreDesiredSizeV = forceInfinityV;

            int i = cellsHead;
            do 
            {
                MeasureCell(i, forceInfinityV);

                if(!ignoreDesiredSizeU)
                {
                    if(PrivateCells[i].ColumnSpan == 1)
                    {
                        DefinitionsU[PrivateCells[i].ColumnIndex].UpdateMinSize(Math.Min(children[i].DesiredSize.Width, DefinitionsU[PrivateCells[i].ColumnIndex].UserMaxSize));
                    }
                    else
                    {
                        RegisterSpan(
                            ref spanStore,
                            PrivateCells[i].ColumnIndex,
                            PrivateCells[i].ColumnSpan,
                            true,
                            children[i].DesiredSize.Width);
                    }
                }

                if(!ignoreDesiredSizeV)
                {
                    if(PrivateCells[i].RowSpan == 1)
                    {
                        DefinitionsV[PrivateCells[i].RowIndex].UpdateMinSize(Math.Min(children[i].DesiredSize.Height, DefinitionsV[PrivateCells[i].RowIndex].UserMaxSize));
                    }
                    else
                    {
                        RegisterSpan(
                            ref spanStore,
                            PrivateCells[i].RowIndex,
                            PrivateCells[i].RowSpan,
                            false,
                            children[i].DesiredSize.Height);
                    }
                }

                i = PrivateCells[i].Next;

            } while (i < PrivateCells.Length);

            if (spanStore != null)
            {
                foreach (DictionaryEntry e in spanStore)
                {
                    SpanKey key = (SpanKey)e.Key;
                    float requestedSize = System.Convert.ToSingle(e.Value);

                    EnsureMinSizeInDefinitionRange(
                        key.U ? DefinitionsU : DefinitionsV,
                        key.Start,
                        key.Count,
                        requestedSize,
                        key.U ? referenceSize.Width : referenceSize.Height);
                }
            }
        }

        private static void RegisterSpan(
            ref Hashtable store,
            int start,
            int count,
            bool u,
            double value)
        {
            if (store == null)
            {
                store = new Hashtable();
            }

            SpanKey key = new SpanKey(start, count, u);
            object o = store[key];

            if (o == null || value > (double)o)
            {
                store[key] = value;
            }
        }

        private void MeasureCell(
            int cell,
            bool forceInfinityV)
        {
            double cellMeasureWidth;
            double cellMeasureHeight;

            if (PrivateCells[cell].IsAutoU && !PrivateCells[cell].IsStarU)
            {
                cellMeasureWidth = float.PositiveInfinity;
            }
            else
            {
                cellMeasureWidth = GetMeasureSizeForRange(
                                        DefinitionsU, 
                                        PrivateCells[cell].ColumnIndex, 
                                        PrivateCells[cell].ColumnSpan);
            }

            if (forceInfinityV)
            {
                cellMeasureHeight = float.PositiveInfinity;
            }
            else if (PrivateCells[cell].IsAutoV && !PrivateCells[cell].IsStarV)
            {
                cellMeasureHeight = float.PositiveInfinity;
            }
            else
            {
                cellMeasureHeight = GetMeasureSizeForRange(
                                        DefinitionsV,
                                        PrivateCells[cell].RowIndex,
                                        PrivateCells[cell].RowSpan);
            }

            if (ChildWindows.Count > cell)
            {
                WinBase child = ChildWindows[cell];
                if (child != null)
                {
                    var childConstraint = new SlimDX.Size(cellMeasureWidth, cellMeasureHeight);
                    child.Measure(childConstraint);
                }
            }

        }

        private double GetMeasureSizeForRange(
            DefinitionBase[] definitions,
            int start,
            int count)
        {
            Debug.Assert(0 < count && 0 <= start && (start + count) <= definitions.Length);

            double measureSize = 0;
            int i = start + count - 1;

            do 
            {
                measureSize += (definitions[i].SizeType == LayoutTimeSizeType.Auto) ? definitions[i].MinSize : definitions[i].MeasureSize;
            } while (--i >= start);

            return measureSize;
        }

        protected override SlimDX.Size MeasureOverride(SlimDX.Size constraint)
        {
            SlimDX.Size gridDesiredSize;
            ExtendedData extData = ExtData;

            try
            {
                MeasureOverrideInProgress = true;
                if (extData == null)
                {
                    gridDesiredSize = new SlimDX.Size();

                    for (int i = 0; i < ChildWindows.Count; ++i)
                    {
                        WinBase child = ChildWindows[i];
                        if (child != null)
                        {
                            child.Measure(constraint);
                            gridDesiredSize.Width = Math.Max(gridDesiredSize.Width, child.DesiredSize.Width);
                            gridDesiredSize.Height = Math.Max(gridDesiredSize.Height, child.DesiredSize.Height);
                        }
                    }
                }
                else
                {
                    {
                        bool sizeToContentU = float.IsPositiveInfinity(constraint.Width);
                        bool sizeToContentV = float.IsPositiveInfinity(constraint.Height);

                        if (RowDefinitionCollectionDirty || ColumnDefinitionCollectionDirty)
                        {
                            if (_definitionIndices != null)
                            {
                                Array.Clear(_definitionIndices, 0, _definitionIndices.Length);
                                _definitionIndices = null;
                            }
                        }

                        ValidateDefinitionsUStructure();
                        ValidateDefinitionsLayout(DefinitionsU, sizeToContentU);

                        ValidateDefinitionsVStructure();
                        ValidateDefinitionsLayout(DefinitionsV, sizeToContentV);

                        CellsStructureDirty |= (SizeToContentU != sizeToContentU) || (SizeToContentV != sizeToContentV);

                        SizeToContentU = sizeToContentU;
                        SizeToContentV = sizeToContentV;
                    }

                    ValidateCells();

                    Debug.Assert(DefinitionsU.Length > 0 && DefinitionsV.Length > 0);

                    MeasureCellsGroup(extData.CellGroup1, constraint, false, false);

                    {
                        bool canResolveStarsV = !HasGroup3CellsInAutoRows;

                        if (canResolveStarsV)
                        {
                            if (HasStarCellsV) { ResolveStar(DefinitionsV, constraint.Height); }
                            MeasureCellsGroup(extData.CellGroup2, constraint, false, false);
                            if (HasStarCellsU) { ResolveStar(DefinitionsU, constraint.Width); }
                            MeasureCellsGroup(extData.CellGroup3, constraint, false, false);
                        }
                        else
                        {
                            bool canResolveStarsU = extData.CellGroup2 > PrivateCells.Length;
                            if (canResolveStarsU)
                            {
                                if (HasStarCellsU) { ResolveStar(DefinitionsU, constraint.Width); }
                                MeasureCellsGroup(extData.CellGroup3, constraint, false, false);
                                if (HasStarCellsV) { ResolveStar(DefinitionsV, constraint.Height); }
                            }
                            else
                            {
                                MeasureCellsGroup(extData.CellGroup2, constraint, false, true);
                                if (HasStarCellsU) { ResolveStar(DefinitionsU, constraint.Width); }
                                MeasureCellsGroup(extData.CellGroup3, constraint, false, false);
                                if (HasStarCellsV) { ResolveStar(DefinitionsV, constraint.Height); }
                                MeasureCellsGroup(extData.CellGroup2, constraint, true, false);
                            } 
                        }
                    }

                    MeasureCellsGroup(extData.CellGroup4, constraint, false, false);

                    gridDesiredSize = new SlimDX.Size(
                                CalculateDesiredSize(DefinitionsU),
                                CalculateDesiredSize(DefinitionsV));
                }
            }
            finally
            {
                MeasureOverrideInProgress = false;
            }

            return gridDesiredSize;
        }        

        protected override SlimDX.Size ArrangeOverride(SlimDX.Size arrangeSize)
        {
            try
            {
                ArrangeOverrideInProgress = true;

                if (_data == null)
                {
                    for (int i = 0; i < ChildWindows.Count; ++i)
                    {
                        var child = ChildWindows[i];
                        if (child != null)
                        {
                            child.Arrange(new SlimDX.Rect(arrangeSize));
                        }
                    }
                }
                else if (DefinitionsU != null && DefinitionsV!=null )
                {
                    Debug.Assert(DefinitionsU.Length > 0 && DefinitionsV.Length > 0);

                    SetFinalSize(DefinitionsU, arrangeSize.Width, true);
                    SetFinalSize(DefinitionsV, arrangeSize.Height, false);

                    if (ChildWindows.Count > 0)
                    {
                        for (int currentCell = 0; currentCell < PrivateCells.Length; ++currentCell)
                        {
                            if (currentCell >= ChildWindows.Count)
                                continue;

                            var cell = ChildWindows[currentCell];
                            if (cell == null)
                                continue;

                            int columnIndex = PrivateCells[currentCell].ColumnIndex;
                            int rowIndex = PrivateCells[currentCell].RowIndex;
                            int columnSpan = PrivateCells[currentCell].ColumnSpan;
                            int rowSpan = PrivateCells[currentCell].RowSpan;

                            SlimDX.Rect cellRect = new SlimDX.Rect(
                                columnIndex == 0 ? 0.0 : DefinitionsU[columnIndex].FinalOffset,
                                rowIndex == 0 ? 0.0 : DefinitionsV[rowIndex].FinalOffset,
                                GetFinalSizeForRange(DefinitionsU, columnIndex, columnSpan),
                                GetFinalSizeForRange(DefinitionsV, rowIndex, rowSpan));

                            cell.Arrange(cellRect);
                        }
                    }

                }
            }
            finally
            {
                SetValid();
                ArrangeOverrideInProgress = false;
            }

            return arrangeSize;
        }

        public override CSUtility.Support.Point GetChildOffset(WinBase child)
        {
            CSUtility.Support.Point retPoint = CSUtility.Support.Point.Empty;

            var index = IndexOfChild(child);
            int columnIndex = 0;
            int rowIndex = 0;
            if (PrivateCells != null && PrivateCells.Length > index)
            {
                columnIndex = PrivateCells[index].ColumnIndex;
                rowIndex = PrivateCells[index].RowIndex;
            }

            retPoint.X = columnIndex == 0 ? 0 : (int)DefinitionsU[columnIndex].FinalOffset;
            retPoint.Y = rowIndex == 0 ? 0 : (int)DefinitionsV[rowIndex].FinalOffset;

            //var columnIdx = System.Math.Min(child.GridColumn, GridSplit.Columns.Count - 1);
            //if (columnIdx > 0)
            //{
            //    for (int i = 0; i < columnIdx; i++)
            //        retPoint.X += (int)(GridSplit.Columns[i].ActualLength);
            //}

            //var rowIdx = System.Math.Min(child.GridRow, GridSplit.Rows.Count - 1);
            //if (rowIdx > 0)
            //{
            //    for (int i = 0; i < rowIdx; i++)
            //        retPoint.Y += (int)(GridSplit.Rows[i].ActualLength);
            //}

            return retPoint;
        }

        public override CSUtility.Support.Size GetSizeByChild(WinBase child)
        {
            CSUtility.Support.Size retSize = CSUtility.Support.Size.Empty;

            var index = IndexOfChild(child);
            int columnIndex = 0;
            if(PrivateCells != null && PrivateCells.Length > index)
                columnIndex = PrivateCells[index].ColumnIndex;
            int rowIndex = 0;
            if(PrivateCells != null && PrivateCells.Length > index)
                rowIndex = PrivateCells[index].RowIndex;
            int columnSpan = 1;
            if(PrivateCells != null && PrivateCells.Length > index)
                columnSpan = PrivateCells[index].ColumnSpan;
            int rowSpan = 1;
            if(PrivateCells != null && PrivateCells.Length > index)
                rowSpan = PrivateCells[index].RowSpan;

            retSize.Width = (int)GetFinalSizeForRange(DefinitionsU, columnIndex, columnSpan);
            retSize.Height = (int)GetFinalSizeForRange(DefinitionsV, rowIndex, rowSpan);

            //var columnIdx = System.Math.Min(child.GridColumn, GridSplit.Columns.Count - 1);
            //var rowIdx = System.Math.Min(child.GridRow, GridSplit.Rows.Count - 1);

            //if (columnIdx < 0 || rowIdx < 0)
            //    return base.GetSizeByChild(child);

            //for (int i = columnIdx; i < System.Math.Min(columnIdx + child.GridColumnSpan, GridSplit.Columns.Count); i++)
            //{
            //    var column = GridSplit.Columns[i];
            //    retSize.Width += (int)(column.ActualLength);
            //}
            //for (int i = rowIdx; i < System.Math.Min(rowIdx + child.GridRowSpan, GridSplit.Rows.Count); i++)
            //{
            //    var row = GridSplit.Rows[i];
            //    retSize.Height += (int)(row.ActualLength);
            //}

            return retSize;
        }

        bool mClipWithCell = false;
        [Category("布局"), DisplayName("使用网格裁剪")]
        public bool ClipWithCell
        {
            get { return mClipWithCell; }
            set
            {
                mClipWithCell = value;
                UpdateClipRect();

                OnPropertyChanged("ClipWithCell");
            }
        }

        public override CSUtility.Support.Rectangle GetClipRect(WinBase child)
        {
            if (mClipWithCell)
            {
                if (PrivateCells == null)
                    return base.GetClipRect(child);

                var retRect = new CSUtility.Support.Rectangle();

                int columnIndex;
                int rowIndex;
                int columnSpan;
                int rowSpan;
                var index = IndexOfChild(child);
                if (index < PrivateCells.Length)
                {
                    columnIndex = PrivateCells[index].ColumnIndex;
                    rowIndex = PrivateCells[index].RowIndex;
                    columnSpan = PrivateCells[index].ColumnSpan;
                    rowSpan = PrivateCells[index].RowSpan;
                }
                else
                {
                    columnIndex = child.GridColumn;
                    rowIndex = child.GridRow;
                    columnSpan = child.GridColumnSpan;
                    rowSpan = child.GridRowSpan;
                }

                if (columnIndex >= DefinitionsU.Length)
                    columnIndex = DefinitionsU.Length - 1;
                else if (columnIndex < 0)
                    columnIndex = 0;
                if (rowIndex >= DefinitionsV.Length)
                    rowIndex = DefinitionsV.Length - 1;
                else if (rowIndex < 0)
                    rowIndex = 0;

                if (columnSpan < 1)
                    columnSpan = 1;
                else if (columnSpan > DefinitionsU.Length)
                    columnSpan = DefinitionsU.Length;
                if (rowSpan < 1)
                    rowSpan = 1;
                else if (rowSpan > DefinitionsV.Length)
                    rowSpan = DefinitionsV.Length;
            
                retRect.X = (columnIndex == 0 ? 0 : (int)DefinitionsU[columnIndex].FinalOffset) + AbsRect.X;
                retRect.Y = (rowIndex == 0 ? 0 : (int)DefinitionsV[rowIndex].FinalOffset) + AbsRect.Y;
                retRect.Width = (int)GetFinalSizeForRange(DefinitionsU, columnIndex, columnSpan);
                retRect.Height = (int)GetFinalSizeForRange(DefinitionsV, rowIndex, rowSpan);

                return CSUtility.Support.Rectangle.Intersect(ClipRect, retRect);
            }
            else
                return base.GetClipRect(child);

        }
    }
}
