using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;

namespace CSUtility.Support
{
    //public enum NotifyCollectionChangedAction
    //{
    //    Add,
    //    Remove,
    //    Replace,
    //    Reset,
    //    Move,
    //}

    //public class NotifyCollectionChangedEventArgs : EventArgs
    //{


    //    private NotifyCollectionChangedAction _action;
    //    public NotifyCollectionChangedAction Action
    //    {
    //        get { return _action; }
    //    }

    //    private IList _newItems;
    //    public IList NewItems
    //    {
    //        get { return _newItems; }
    //    }

    //    int _newStartingIndex;
    //    public int NewStartingIndex
    //    {
    //        get { return _newStartingIndex; }
    //    }

    //    IList _oldItems;
    //    public IList OldItems
    //    {
    //        get { return _oldItems; }
    //    }

    //    int _oldStartingIndex;
    //    public int OldStartingIndex
    //    {
    //        get { return _oldStartingIndex; }
    //    }

    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (action != NotifyCollectionChangedAction.Reset)
    //        {
    //            throw new ArgumentException("WrongActionForCtor", "action");
    //        }
    //        this.InitializeAdd(action, null, -1);
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
    //        {
    //            throw new ArgumentException("MustBeResetAddOrRemoveActionForCtor", "action");
    //        }
    //        if (action == NotifyCollectionChangedAction.Reset)
    //        {
    //            if (changedItems != null)
    //            {
    //                throw new ArgumentException("ResetActionRequiresNullItem", "action");
    //            }
    //            this.InitializeAdd(action, null, -1);
    //        }
    //        else
    //        {
    //            if (changedItems == null)
    //            {
    //                throw new ArgumentNullException("changedItems");
    //            }
    //            this.InitializeAddOrRemove(action, changedItems, -1);
    //        }
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
    //        {
    //            throw new ArgumentException("MustBeResetAddOrRemoveActionForCtor", "action");
    //        }
    //        if (action == NotifyCollectionChangedAction.Reset)
    //        {
    //            if (changedItem != null)
    //            {
    //                throw new ArgumentException("ResetActionRequiresNullItem", "action");
    //            }
    //            this.InitializeAdd(action, null, -1);
    //        }
    //        else
    //        {
    //            this.InitializeAddOrRemove(action, new object[] { changedItem }, -1);
    //        }
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (action != NotifyCollectionChangedAction.Replace)
    //        {
    //            throw new ArgumentException("WrongActionForCtor", "action");
    //        }
    //        if (newItems == null)
    //        {
    //            throw new ArgumentNullException("newItems");
    //        }
    //        if (oldItems == null)
    //        {
    //            throw new ArgumentNullException("oldItems");
    //        }
    //        this.InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
    //        {
    //            throw new ArgumentException("MustBeResetAddOrRemoveActionForCtor", "action");
    //        }
    //        if (action == NotifyCollectionChangedAction.Reset)
    //        {
    //            if (changedItems != null)
    //            {
    //                throw new ArgumentException("ResetActionRequiresNullItem", "action");
    //            }
    //            if (startingIndex != -1)
    //            {
    //                throw new ArgumentException("ResetActionRequiresIndexMinus1", "action");
    //            }
    //            this.InitializeAdd(action, null, -1);
    //        }
    //        else
    //        {
    //            if (changedItems == null)
    //            {
    //                throw new ArgumentNullException("changedItems");
    //            }
    //            if (startingIndex < -1)
    //            {
    //                throw new ArgumentException("IndexCannotBeNegative", "startingIndex");
    //            }
    //            this.InitializeAddOrRemove(action, changedItems, startingIndex);
    //        }
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
    //        {
    //            throw new ArgumentException("MustBeResetAddOrRemoveActionForCtor", "action");
    //        }
    //        if (action == NotifyCollectionChangedAction.Reset)
    //        {
    //            if (changedItem != null)
    //            {
    //                throw new ArgumentException("ResetActionRequiresNullItem", "action");
    //            }
    //            if (index != -1)
    //            {
    //                throw new ArgumentException("ResetActionRequiresIndexMinus1", "action");
    //            }
    //            this.InitializeAdd(action, null, -1);
    //        }
    //        else
    //        {
    //            this.InitializeAddOrRemove(action, new object[] { changedItem }, index);
    //        }
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (action != NotifyCollectionChangedAction.Replace)
    //        {
    //            throw new ArgumentException("WrongActionForCtor", "action");
    //        }
    //        this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, -1, -1);
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (action != NotifyCollectionChangedAction.Replace)
    //        {
    //            throw new ArgumentException("WrongActionForCtor", "action");
    //        }
    //        if (newItems == null)
    //        {
    //            throw new ArgumentNullException("newItems");
    //        }
    //        if (oldItems == null)
    //        {
    //            throw new ArgumentNullException("oldItems");
    //        }
    //        this.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (action != NotifyCollectionChangedAction.Move)
    //        {
    //            throw new ArgumentException("WrongActionForCtor", "action");
    //        }
    //        if (index < 0)
    //        {
    //            throw new ArgumentException("IndexCannotBeNegative", "index");
    //        }
    //        this.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (action != NotifyCollectionChangedAction.Move)
    //        {
    //            throw new ArgumentException("WrongActionForCtor", "action");
    //        }
    //        if (index < 0)
    //        {
    //            throw new ArgumentException("IndexCannotBeNegative", "index");
    //        }
    //        object[] newItems = new object[] { changedItem };
    //        this.InitializeMoveOrReplace(action, newItems, newItems, index, oldIndex);
    //    }
    //    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        if (action != NotifyCollectionChangedAction.Replace)
    //        {
    //            throw new ArgumentException("WrongActionForCtor", "action");
    //        }
    //        int oldStartingIndex = index;
    //        this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, oldStartingIndex);
    //    }

    //    internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newIndex, int oldIndex)
    //    {
    //        this._newStartingIndex = -1;
    //        this._oldStartingIndex = -1;
    //        this._action = action;
    //        this._newItems = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
    //        this._oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
    //        this._newStartingIndex = newIndex;
    //        this._oldStartingIndex = oldIndex;
    //    }

    //    private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex)
    //    {
    //        this._action = action;
    //        this._newItems = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
    //        this._newStartingIndex = newStartingIndex;
    //    }
    //    private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
    //    {
    //        if (action == NotifyCollectionChangedAction.Add)
    //        {
    //            this.InitializeAdd(action, changedItems, startingIndex);
    //        }
    //        else if (action == NotifyCollectionChangedAction.Remove)
    //        {
    //            this.InitializeRemove(action, changedItems, startingIndex);
    //        }
    //    }
    //    private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex)
    //    {
    //        this.InitializeAdd(action, newItems, startingIndex);
    //        this.InitializeRemove(action, oldItems, oldStartingIndex);
    //    }
    //    private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex)
    //    {
    //        this._action = action;
    //        this._oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
    //        this._oldStartingIndex = oldStartingIndex;
    //    }

    //}

    public class ThreadSafeObservableCollection<T> : IList<T>, ICollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }
        #endregion

        #region INotifyCollectionChangedMembers
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion
        private class ThreadSafeObservableCollectionEnumerator<T1> : IEnumerator<T1>
        {
            private ThreadSafeObservableCollection<T1> _col;
            private int _index;

            public void Dispose() { }
            public ThreadSafeObservableCollectionEnumerator(ThreadSafeObservableCollection<T1> col)
            {
                _col = col;
                _index = -1;
            }
            public bool MoveNext()
            {
                return ++_index < _col.Count;
            }
            public T1 Current
            {
                get { return _col[_index]; }
            }
            object System.Collections.IEnumerator.Current
            {
                get { return _col[_index]; }
            }
            public void Reset()
            {
                _index = -1;
            }
        }

        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        //private SimpleMonitor _monitor;

        LinkedList<T> _items = new LinkedList<T>();

        System.Collections.Concurrent.BlockingCollection<int> aa = new System.Collections.Concurrent.BlockingCollection<int>();

        public ThreadSafeObservableCollection()
        {
            //this._monitor = new SimpleMonitor();
        }

        public ThreadSafeObservableCollection(IEnumerable<T> collection)
        {
            //this._monitor = new SimpleMonitor();
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.CopyFrom(collection);
        }

        public ThreadSafeObservableCollection(List<T> list)
        {
            //this._monitor = new SimpleMonitor();
            this.CopyFrom(list);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get
            {
                lock (_items)
                {
                    if (index < 0 || index >= _items.Count)
                        return default(T);

                    int i = 0;
                    var retValue = _items.First;
                    while (i < index)
                    {
                        retValue = retValue.Next;
                        i++;
                    }
                    return retValue.Value;
                }
            }
            set
            {
                lock (_items)
                {
                    if (index >= 0 && index < _items.Count)
                    {
                        int i = 0;
                        var opValue = _items.First;
                        while (i < index)
                        {
                            opValue = opValue.Next;
                            i++;
                        }
                        T oldItem = opValue.Value;
                        opValue.Value = value;

                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, value, oldItem, index));
                    }
                }
            }
        }

        public int Count => _items.Count;

        public void Add(T item)
        {
            lock(_items)
            {
                _items.AddLast(item);

                this.OnPropertyChanged(CountString);
                this.OnPropertyChanged(IndexerName);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }
        public void Clear()
        {
            lock(_items)
            {
                var tempList = new ArrayList(_items);
                _items.Clear();

                this.OnPropertyChanged(CountString);
                this.OnPropertyChanged(IndexerName);
                if(tempList.Count != 0)
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tempList, 0));
            }
        }
        public bool Contains(T item)
        {
            lock(_items)
            {
                return _items.Contains(item);
            }
        }
        public void CopyTo(T[] array, int index)
        {
            lock(_items)
            {
                _items.CopyTo(array, index);
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new ThreadSafeObservableCollectionEnumerator<T>(this);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ThreadSafeObservableCollectionEnumerator<T>(this);
        }
        public int IndexOf(T item)
        {
            lock(_items)
            {
                int i = 0;
                var opValue = _items.First;
                while (opValue != null)
                {
                    if (opValue.Value.Equals(item))
                        return i;

                    opValue = opValue.Next;
                    i++;
                }

                return -1;
            }
        }
        public void Insert(int index, T item)
        {
            lock(_items)
            {
                if (index < 0)
                    return;

                if (index >= _items.Count)
                    _items.AddLast(item);

                int i = 0;
                var opValue = _items.First;
                while(i < index)
                {
                    opValue = opValue.Next;
                    i++;
                }
                _items.AddBefore(opValue, item);

                this.OnPropertyChanged(CountString);
                this.OnPropertyChanged(IndexerName);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }
        public bool Remove(T item)
        {
            lock(_items)
            {
                int i = 0;
                var node = _items.First;

                while(node != null)
                {
                    if (node.Value.Equals(item))
                        break;
                    node = node.Next;
                    i++;
                }
                if (i >= _items.Count)
                    return false;

                RemoveAt(i);

                return true;
            }
        }
        public void RemoveAt(int index)
        {
            lock(_items)
            {
                if (index < 0 || index >= _items.Count)
                    return;

                int i = 0;
                var opValue = _items.First;
                while(i < index)
                {
                    opValue = opValue.Next;
                    i++;
                }
                _items.Remove(opValue);

                this.OnPropertyChanged(CountString);
                this.OnPropertyChanged(IndexerName);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, opValue.Value, index));
            }
        }

        //protected IDisposable BlockReentrancy()
        //{
        //    this._monitor.Enter();
        //    return this._monitor;
        //}
        //protected void CheckReentrancy()
        //{
        //    if ((this._monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1))
        //    {
        //        throw new InvalidOperationException("ObservableCollectionReentrancyNotAllowed");
        //    }
        //}

        //protected void ClearItems()
        //{
        //    this.CheckReentrancy();
        //    base.ClearItems();
        //    this.OnPropertyChanged(CountString);
        //    this.OnPropertyChanged(IndexerName);
        //    this.OnCollectionReset();
        //}

        private void CopyFrom(IEnumerable<T> collection)
        {
            lock (_items)
            {
                if ((collection != null) && (_items != null))
                {
                    _items.Clear();
                    using (IEnumerator<T> enumerator = collection.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            _items.AddLast(enumerator.Current);
                        }
                    }
                }
            }
        }

        //protected override void InsertItem(int index, T item)
        //{
        //    this.CheckReentrancy();
        //    base.InsertItem(index, item);
        //    this.OnPropertyChanged(CountString);
        //    this.OnPropertyChanged(IndexerName);
        //    this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        //}

    //    //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void Move(int oldIndex, int newIndex)
        {
            this.MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            lock (_items)
            {
                if (oldIndex < 0 || oldIndex >= _items.Count)
                    return;
                if (newIndex < 0 || newIndex >= _items.Count)
                    return;

                //this.CheckReentrancy();
                T item = this[oldIndex];
                _items.Remove(item);
                if (newIndex < 0)
                    newIndex = 0;
                if (newIndex >= _items.Count)
                    newIndex = _items.Count - 1;

                int i = 0;
                var opValue = _items.First;
                while(i < newIndex)
                {
                    opValue = opValue.Next;
                    i++;
                }
                _items.AddBefore(opValue, item);
            
                this.OnPropertyChanged(IndexerName);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
            }
        }

        //protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    CollectionChanged?.Invoke(this, e);
        //    //if (this.CollectionChanged != null)
        //    //{
        //    //    using (this.BlockReentrancy())
        //    //    {
        //    //        this.CollectionChanged(this, e);
        //    //    }
        //    //}
        //}
        //private void OnCollectionChanged(NotifyCollectionChangedAction action, object item)
        //{
        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item));
        //}
        //private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        //{
        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        //}
        //private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        //{
        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        //}
        //private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        //{
        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        //}
        //private void OnCollectionReset()
        //{
        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //}

        //protected override void RemoveItem(int index)
        //{
        //    this.CheckReentrancy();
        //    T item = base[index];
        //    base.RemoveItem(index);
        //    this.OnPropertyChanged(CountString);
        //    this.OnPropertyChanged(IndexerName);
        //    this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        //}
        //protected override void SetItem(int index, T item)
        //{
        //    this.CheckReentrancy();
        //    T oldItem = base[index];
        //    base.SetItem(index, item);
        //    this.OnPropertyChanged(IndexerName);
        //    this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
        //}

        public T[] ToArray()
        {
            lock (_items)
            {
                T[] destinationArray = new T[this.Count];
                for (int i=0; i<this.Count; i++)
                {
                    destinationArray[i] = this[i];
                }
                return destinationArray;
            }
        }
        
        //[Serializable]
        //private class SimpleMonitor : IDisposable
        //{
        //    private int _busyCount;

        //    public bool Busy
        //    {
        //        get { return (this._busyCount > 0); }
        //    }

        //    public SimpleMonitor()
        //    {
        //    }

        //    public void Dispose()
        //    {
        //        this._busyCount--;
        //    }
        //    public void Enter()
        //    {
        //        this._busyCount++;
        //    }
        //}

    }
}
