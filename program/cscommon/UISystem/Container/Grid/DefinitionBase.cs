using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UISystem.Container.Grid
{
    public abstract class DefinitionBase
    {
        private class SharedSizeScope
        {
            private System.Collections.Hashtable _registry = new System.Collections.Hashtable();

            internal SharedSizeState EnsureSharedState(string sharedSizeGroup)
            {
                Debug.Assert(sharedSizeGroup != null);

                SharedSizeState sharedState = _registry[sharedSizeGroup] as SharedSizeState;
                if (sharedState == null)
                {
                    sharedState = new SharedSizeState(this, sharedSizeGroup);
                    _registry[sharedSizeGroup] = sharedState;
                }

                return sharedState;
            }

            internal void Remove(object key)
            {
                Debug.Assert(_registry.Contains(key));
                _registry.Remove(key);
            }
        }

        private class SharedSizeState
        {
            private readonly SharedSizeScope _sharedSizeScope;
            private readonly string _sharedSizeGroupId;       
            private readonly List<DefinitionBase> _registry;  
            private readonly EventHandler _layoutUpdated;     
            private WinBase _layoutUpdatedHost;               
            private bool _broadcastInvalidation;              
            private bool _userSizeValid;                      
            private GridLength _userSize;                     
            private float _minSize;                                                                              

            internal SharedSizeState(SharedSizeScope sharedSizeScope, string sharedSizeGroupId)
            {
                Debug.Assert(sharedSizeScope != null && sharedSizeGroupId != null);
                _sharedSizeScope = sharedSizeScope;
                _sharedSizeGroupId = sharedSizeGroupId;
                _registry = new List<DefinitionBase>();
                _layoutUpdated = new EventHandler(OnLayoutUpdated);
                _broadcastInvalidation = true;
            }

            internal void AddMember(DefinitionBase member)
            {
                Debug.Assert(!_registry.Contains(member));
                _registry.Add(member);
                Invalidate();
            }

            internal void RemoveMember(DefinitionBase member)
            {
                Invalidate();
                _registry.Remove(member);

                if (_registry.Count == 0)
                {
                    _sharedSizeScope.Remove(_sharedSizeGroupId);
                }
            }

            internal void Invalidate()
            {
                _userSizeValid = false;

                if (_broadcastInvalidation)
                {
                    for (int i = 0, count = _registry.Count; i < count; ++i)
                    {
                        UISystem.Container.Grid.Grid parentGrid = (UISystem.Container.Grid.Grid)(_registry[i].Parent);
                        parentGrid.Invalidate();
                    }
                    _broadcastInvalidation = false;
                }
            }

            internal void EnsureDeferredValidation(WinBase layoutUpdatedHost)
            {
                if (_layoutUpdatedHost == null)
                {
                    _layoutUpdatedHost = layoutUpdatedHost;
                    _layoutUpdatedHost.LayoutUpdated += _layoutUpdated;
                }
            }

            internal float MinSize
            {
                get
                {
                    if (!_userSizeValid) { EnsureUserSizeValid(); }
                    return (_minSize);
                }
            }

            internal GridLength UserSize
            {
                get
                {
                    if (!_userSizeValid) { EnsureUserSizeValid(); }
                    return (_userSize);
                }
            }

            private void EnsureUserSizeValid()
            {
                _userSize = new GridLength(1, GridUnitType.Auto);

                for (int i = 0, count = _registry.Count; i < count; ++i)
                {
                    Debug.Assert(_userSize.GridUnitType == GridUnitType.Auto
                                || _userSize.GridUnitType == GridUnitType.Pixel);

                    var currentGridLength = _registry[i].UserSizeValueCache;
                    if (currentGridLength.GridUnitType == GridUnitType.Pixel)
                    {
                        if (_userSize.GridUnitType == GridUnitType.Auto)
                        {
                            _userSize = currentGridLength;
                        }
                        else if (_userSize.Value < currentGridLength.Value)
                        {
                            _userSize = currentGridLength;
                        }
                    }
                }
                _minSize = _userSize.IsAbsolute ? _userSize.Value : 0.0f;

                _userSizeValid = true;
            }

            private void OnLayoutUpdated(object sender, EventArgs e)
            {
                float sharedMinSize = 0;

                for (int i = 0, count = _registry.Count; i < count; ++i)
                {
                    sharedMinSize = Math.Max(sharedMinSize, _registry[i].MinSize);
                }

                bool sharedMinSizeChanged = !FloatUtil.AreClose(_minSize, sharedMinSize);

                for (int i = 0, count = _registry.Count; i < count; ++i)
                {
                    DefinitionBase definitionBase = _registry[i];

                    if (sharedMinSizeChanged || definitionBase.LayoutWasUpdated)
                    {
                        if (!FloatUtil.AreClose(sharedMinSize, definitionBase.MinSize))
                        {
                            var parentGrid = (UISystem.Container.Grid.Grid)definitionBase.Parent;
                            parentGrid.InvalidateMeasure();
                            definitionBase.UseSharedMinimum = true;
                        }
                        else
                        {
                            definitionBase.UseSharedMinimum = false;

                            if (!FloatUtil.AreClose(sharedMinSize, definitionBase.SizeCache))
                            {
                                var parentGrid = (UISystem.Container.Grid.Grid)definitionBase.Parent;
                                parentGrid.InvalidateArrange();
                            }
                        }

                        definitionBase.LayoutWasUpdated = false;
                    }
                }

                _minSize = sharedMinSize;

                _layoutUpdatedHost.LayoutUpdated -= _layoutUpdated;
                _layoutUpdatedHost = null;

                _broadcastInvalidation = true;
            }
        }

        [System.Flags]
        private enum Flags : byte
        {
            UseSharedMinimum = 0x00000020,
            LayoutWasUpdated = 0x00000040,     //  父级grid每次测量后设为"1"
        }
        private void SetFlags(bool value, Flags flags)
        {
            _flags = value ? (_flags | flags) : (_flags & (~flags));
        }
        private bool CheckFlagsAnd(Flags flags)
        {
            return ((_flags & flags) == flags);
        }
        private bool UseSharedMinimum
        {
            get { return (CheckFlagsAnd(Flags.UseSharedMinimum)); }
            set { SetFlags(value, Flags.UseSharedMinimum); }
        }
        private bool LayoutWasUpdated
        {
            get { return (CheckFlagsAnd(Flags.LayoutWasUpdated)); }
            set { SetFlags(value, Flags.LayoutWasUpdated); }
        }

        private Flags _flags;  
        
        private LayoutTimeSizeType _sizeType;     

        private float _minSize;                   
        private float _measureSize;               
        private float _sizeCache;                 
        private float _offset;                                                    

        private SharedSizeState _sharedState = null;      

        protected GridLength _userSizeCache = new GridLength();

        internal GridLength UserSizeValueCache 
        {
            get
            {
                return _userSizeCache;
            }
        }

        private WinBase mParent;
        public WinBase Parent
        {
            get { return mParent; }
            set
            {
                mParent = value;
            }
        }
        
        internal bool IsShared
        {
            get { return (_sharedState != null); }
        }

        internal float UserMinSize
        {
            get { return _userSizeCache.MinLength; }
        }

        internal float UserMaxSize
        {
            get { return _userSizeCache.MaxLength; }
        }

        public GridLength UserSize
        {
            get { return (_sharedState != null ? _sharedState.UserSize : _userSizeCache); }
        }

        internal LayoutTimeSizeType SizeType
        {
            get { return _sizeType; }
            set { _sizeType = value; }
        }

        internal float MeasureSize
        {
            get { return _measureSize; }
            set { _measureSize = value; }
        }

        internal float PreferredSize
        {
            get
            {
                float preferredSize = MinSize;
                if (_sizeType != LayoutTimeSizeType.Auto
                    && preferredSize < _measureSize)
                {
                    preferredSize = _measureSize;
                }
                return (preferredSize);
            }
        }

        internal float SizeCache
        {
            get { return (_sizeCache); }
            set { _sizeCache = value; }
        }
        
        internal float MinSize
        {
            get
            {
                float minSize = _minSize;
                if (UseSharedMinimum && _sharedState != null && minSize < _sharedState.MinSize)
                {
                    minSize = _sharedState.MinSize;
                }

                return minSize;
            }
        }

        internal float MinSizeForArrange
        {
            get
            {
                float minSize = _minSize;
                if (_sharedState != null
                    && (UseSharedMinimum || !LayoutWasUpdated)
                    && minSize < _sharedState.MinSize)
                {
                    minSize = _sharedState.MinSize;
                }
                return (minSize);
            }
        }

        internal float FinalOffset
        {
            get { return _offset; }
            set { _offset = value; }
        } 

        internal void OnBeforeLayout(Grid grid)
        { 
            _minSize = 0;
            LayoutWasUpdated = true;

            if (_sharedState != null) { _sharedState.EnsureDeferredValidation(grid); }
        }

        internal void UpdateMinSize(float minSize)
        {
            _minSize = Math.Max(_minSize, minSize);
        }
    }

    public class ColumnDefinition : DefinitionBase
    {
        public GridLength WidthProperty
        {
            get{ return _userSizeCache; }
            set
            {
                _userSizeCache = value;
                ((Grid)Parent).CellsStructureDirty = true;
                Parent.UpdateLayout();
            }
        }

        // 获取实际宽度
        public float ActualWidth
        {
            get { return SizeCache; }
        }

        public ColumnDefinition(WinBase parent)
        {
            Parent = parent;
        }
    }

    public class RowDefinition : DefinitionBase
    {
        public GridLength HeightProperty
        {
            get { return _userSizeCache; }
            set
            {
                _userSizeCache = value;
                ((Grid)Parent).CellsStructureDirty = true;
                Parent.UpdateLayout();
            }
        }

        // 获取实际高度
        public float ActualHeight
        {
            get { return SizeCache; }
        }

        public RowDefinition(WinBase parent)
        {
            Parent = parent;
        }
    }
}
