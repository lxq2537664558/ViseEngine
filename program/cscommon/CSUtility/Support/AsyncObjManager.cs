using System;
using System.Collections;
using System.Collections.Generic;

namespace CSUtility.Support
{
    public enum EForEachResult
    {
        FER_Continue,
        FER_Stop,
        FER_Erase,
    };
    
    
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class AsyncObjManager<K, V> : IXmlSaveLoadProxy, IXndSaveLoadProxy
    {
        [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        Dictionary<K, V> mObjects = new Dictionary<K, V>();
        public Dictionary<K, V> Objects
        {
            get { return mObjects; }
        }
        List<KeyValuePair<K, V>> mAddList = new List<KeyValuePair<K, V>>();
        public List<KeyValuePair<K,V>> AddList
        {
            get { return mAddList; }
        }
        List<K> mRemoveList = new List<K>();
        public List<K> RemoveList
        {
            get { return mRemoveList; }
        }

        //bool ICollection<KeyValuePair<K, V>>.IsReadOnly
        //{
        //    get { return false; }
        //}

        public int Count
        {
            get
            {
                lock (this)
                {
                    var retValue = mAddList.Count + mObjects.Count - mRemoveList.Count;
                    return retValue < 0 ? 0 : retValue;
                }
            }
        }

        public ICollection<K> Keys
        {
            get
            {
                lock(this)
                {
                    K[] ret = new K[Count];
                    int i = 0;
                    foreach(var key in Objects.Keys)
                    {
                        if (mRemoveList.Contains(key))
                            continue;

                        ret[i] = key;
                        i++;
                    }
                    foreach(var value in mAddList)
                    {
                        ret[i] = value.Key;
                        i++;
                    }
                    return ret;
                }
            }
        }

        public ICollection<V> Values
        {
            get
            {
                lock(this)
                {
                    V[] ret = new V[Count];
                    int i = 0;
                    foreach(var value in Objects)
                    {
                        if (mRemoveList.Contains(value.Key))
                            continue;

                        ret[i] = value.Value;
                        i++;
                    }
                    foreach(var value in mAddList)
                    {
                        ret[i] = value.Value;
                        i++;
                    }
                    return ret;
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                mRemoveList.Clear();
                mAddList.Clear();
            }
            lock (mObjects)
            {
                mObjects.Clear();
            }
        }

        public V this[K key]
        {
            get { return FindObj(key); }
            set
            {
                lock(this)
                {
                    for (var i = 0; i < mRemoveList.Count; i++)
                    {
                        if (EqualityComparer<K>.Default.Equals(mRemoveList[i], key))
                        {
                            mRemoveList.RemoveAt(i);
                            i--;
                        }
                    }

                    bool added = false;
                    foreach(var i in mAddList)
                    {
                        if (EqualityComparer<K>.Default.Equals(i.Key, key))
                        {
                            mAddList.Remove(i);
                            mAddList.Add(new KeyValuePair<K, V>(key, value));
                            added = true;
                            break;
                        }
                    }

                    if (!added)
                        mAddList.Add(new KeyValuePair<K, V>(key, value));
                }
            }
        }

        public void Add(K key, V value)
        {
            lock (this)
            {
                for (var i = 0; i < mRemoveList.Count; i++)
                {
                    if (EqualityComparer<K>.Default.Equals(mRemoveList[i], key))
                    {
                        mRemoveList.RemoveAt(i);
                        i--;
                    }
                }

                V fv;
                if (mObjects.TryGetValue(key, out fv))
                    return;

                mAddList.Add(new KeyValuePair<K, V>(key, value));
                return;
            }
        }

        //void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> keyValuePair)
        //{
        //    ((IDictionary<K, V>)this).Add(keyValuePair.Key, keyValuePair.Value);
        //}

        public bool Remove(K key)
        {
            lock (this)
            {
                for(var i=0; i<mAddList.Count; i++)
                {
                    if(EqualityComparer<K>.Default.Equals(mAddList[i].Key, key))
                    {
                        mAddList.RemoveAt(i);
                    }
                }
                for (var i = 0; i < mRemoveList.Count; i++)
                {
                    if (EqualityComparer<K>.Default.Equals(mRemoveList[i], key))
                    {
                        return true;
                    }
                }

                if(Objects.ContainsKey(key))
                    mRemoveList.Add(key);
            }

            return true;
        }

        //bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> keyValuePair)
        //{
        //    if (keyValuePair.Key == null) throw new ArgumentNullException("Item key is null");
        //    return Remove(keyValuePair.Key);
        //}

        public V FindObj(K Key)
        {
            lock (this)
            {
                foreach (var i in mAddList)
                {
                    if (EqualityComparer<K>.Default.Equals(i.Key, Key))
                        return i.Value;
                }

                V fv;
                if (mObjects.TryGetValue(Key, out fv))
                    return fv;
                return default(V);
            }
        }

        public bool TryGetValue(K key, out V value)
        {
            lock(this)
            {
                foreach (var i in mAddList)
                {
                    if (EqualityComparer<K>.Default.Equals(i.Key, key))
                    {
                        value = i.Value;
                        return true;
                    }
                }

                V fv;
                if (mObjects.TryGetValue(key, out fv))
                {
                    value = fv;
                    return true;
                }
                value = default(V);
                return false;
            }
        }

        public bool ContainsKey(K key)
        {
            lock (this)
            {
                foreach (var i in mAddList)
                {
                    if (EqualityComparer<K>.Default.Equals(i.Key, key))
                        return true;
                }

                return mObjects.ContainsKey(key);
            }
        }

        //void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int index)
        //{
        //    if (array == null) throw new ArgumentNullException("array");
        //    if (index < 0) throw new ArgumentOutOfRangeException("index", "Index is negative");

        //    if (array.Length - Count < index || Count < 0) //"count" itself or "count + index" can overflow
        //    {
        //        throw new ArgumentException("Array not large enough");
        //    }

        //    lock(this)
        //    {
        //        int idx = 0;
        //        foreach(var obj in mObjects)
        //        {
        //            if (RemoveList.Contains(obj.Key))
        //                continue;

        //            array[idx] = new KeyValuePair<K, V>(obj.Key, obj.Value);

        //            idx++;
        //        }

        //        for (int i = 0; i < mAddList.Count; i++)
        //        {
        //            array[idx] = new KeyValuePair<K, V>(mAddList[i].Key, mAddList[i].Value);
        //            idx++;
        //        }
        //    }
        //}

        //bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> keyValuePair)
        //{
        //    V value;
        //    if (!TryGetValue(keyValuePair.Key, out value))
        //        return false;

        //    return EqualityComparer<V>.Default.Equals(value, keyValuePair.Value);
        //}

        public void Flush()
        {
            BeforeTick();
        }

        public void BeforeTick()
        {
            lock (this)
            {
                foreach (var i in mAddList)
                {
                    mObjects[i.Key] = i.Value;
                }

                mAddList.Clear();

                foreach (var i in mRemoveList)
                {
                    mObjects.Remove(i);
                }
                mRemoveList.Clear();
            }
        }
        public void AfterTick()
        {
            lock (this)
            {
                foreach (var i in mRemoveList)
                {
                    mObjects.Remove(i);
                }
                mRemoveList.Clear();
            }
        }

        public delegate EForEachResult FOnVisitObject(K key, V value, object arg);
        public bool For_Each(FOnVisitObject fun, object arg)
        {
            lock (this)
            {
                for (int i = 0; i < mAddList.Count && i>=0;)
                {
                    var p = mAddList[i];
                    EForEachResult eResult = fun(p.Key, p.Value, arg);
                    switch (eResult)
                    {
                        case EForEachResult.FER_Continue:
                            i++;
                            break;
                        case EForEachResult.FER_Stop:
                            return false;
                        case EForEachResult.FER_Erase:
                            mAddList.RemoveAt(i);
                            i--;
                            break;
                        default:
                            i++;
                            break;
                    }
                }
            }

            lock (mObjects)
            {
                var lst = new List<K>();
                foreach (var i in mObjects)
                {
                    EForEachResult eResult = fun(i.Key, i.Value, arg);
                    switch (eResult)
                    {
                        case EForEachResult.FER_Continue:
                            break;
                        case EForEachResult.FER_Stop:
                            return false;
                        case EForEachResult.FER_Erase:
                            lst.Add(i.Key);
                            break;
                        default:
                            break;
                    }
                }

                foreach (var i in lst)
                {
                    mObjects.Remove(i);
                }

                return true;
            }
        }

        //public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        //{
        //    return mObjects.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return ((AsyncObjManager<K, V>)this).GetEnumerator();
        //}

        public bool Save(XmlNode node, XmlHolder holder)
        {
            lock (this)
            {
                Flush();
                foreach (var i in mObjects)
                {
                    var childNode = node.AddNode("Data", "", holder);

                    var keyNode = childNode.AddNode("Key", "", holder);
                    CSUtility.Support.IConfigurator.SaveValue(keyNode, i.Key.GetType(), i.Key, holder);
                    var valueNode = childNode.AddNode("Value", "", holder);
                    CSUtility.Support.IConfigurator.SaveValue(valueNode, i.Value.GetType(), i.Value, holder);
                }
            }
            return true;
        }
        public bool Load(XmlNode node)
        {
            lock (this)
            {
                Flush();
                mObjects.Clear();

                foreach (var cNode in node.GetNodes())
                {
                    var keyNode = cNode.FindNode("Key");
                    var keyTypeAtt = keyNode.FindAttrib("Type");
                    if (keyTypeAtt == null)
                        continue;
                    var keyType = CSUtility.Program.GetTypeFromSaveString(keyTypeAtt.Value);
                    var key = CSUtility.Support.IConfigurator.ReadValue(keyNode, keyType);

                    var valueNode = cNode.FindNode("Value");
                    var valueTypeAtt = valueNode.FindAttrib("Type");
                    if (valueTypeAtt == null)
                        continue;
                    var valueType = CSUtility.Program.GetTypeFromSaveString(valueTypeAtt.Value);
                    var value = CSUtility.Support.IConfigurator.ReadValue(valueNode, valueType);

                    mObjects[(K)key] = (V)value;
                }
            }
            return true;
        }

        public bool Read(XndAttrib xndAtt)
        {
            lock (this)
            {
                Flush();
                mObjects.Clear();

                int count = 0;
                xndAtt.Read(out count);

                for (int i = 0; i < count; i++)
                {
                    string keyTypeStr;
                    xndAtt.Read(out keyTypeStr);
                    var keyType = Program.GetTypeFromSaveString(keyTypeStr);
                    object key;
                    if (!CSUtility.Support.XndSaveLoadProxy.ReadValue(xndAtt, keyType, out key))
                        return false;

                    string valueTypeStr;
                    xndAtt.Read(out valueTypeStr);
                    var valueType = Program.GetTypeFromSaveString(valueTypeStr);
                    object value;
                    if (!CSUtility.Support.XndSaveLoadProxy.ReadValue(xndAtt, valueType, out value))
                        return false;

                    mObjects[(K)key] = (V)value;
                }

                return true;
            }
        }
        public bool Write(XndAttrib xndAtt)
        {
            lock (this)
            {
                Flush();
                int count = mObjects.Count;
                xndAtt.Write(count);

                foreach (var i in mObjects)
                {
                    var keyType = i.Key.GetType();
                    var keyTypeStr = Program.GetTypeSaveString(keyType);
                    xndAtt.Write(keyTypeStr);
                    CSUtility.Support.XndSaveLoadProxy.WriteValue(xndAtt, keyType, i.Key);

                    var valueType = i.Value.GetType();
                    var valueTypeStr = Program.GetTypeSaveString(valueType);
                    xndAtt.Write(valueTypeStr);
                    CSUtility.Support.XndSaveLoadProxy.WriteValue(xndAtt, valueType, i.Value);
                }

                return true;
            }
        }
        public bool CopyFrom(ICopyable srcData)
        {
            lock (this)
            {
                var com = srcData as AsyncObjManager<K, V>;
                if (com == null)
                    return false;

                com.Flush();
                Flush();
                mObjects.Clear();

                foreach (var obj in com.Objects)
                {
                    var copyedKey = (K)(Copyable.CopyValue(obj.Key));
                    var copyedValue = (V)(Copyable.CopyValue(obj.Value));
                    mObjects.Add(copyedKey, copyedValue);
                }

                return true;
            }
        }
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class ConcurentObjManager<K, V> : IXmlSaveLoadProxy, IXndSaveLoadProxy
    {
        Dictionary<K, V> mObjects = new Dictionary<K, V>();
        public Dictionary<K, V> Objects
        {
            get { return mObjects; }
        }

        public int Count
        {
            get { return mObjects.Count; }
        }

        public ICollection<K> Keys
        {
            get { return Objects.Keys; }
        }

        public ICollection<V> Values
        {
            get { return Objects.Values; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public V this[K key]
        {
            get
            {
                lock(this)
                {
                    if(mObjects.ContainsKey(key))
                        return mObjects[key];

                    return default(V);
                }
            }

            set
            {
                lock(this)
                {
                    mObjects[key] = value;
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                mObjects.Clear();
            }
        }

        public void Add(K key, V value)
        {
            lock (this)
            {
                mObjects[key] = value;
            }
        }

        public bool Remove(K key)
        {
            lock (this)
            {
                return mObjects.Remove(key);
            }
        }

        public V FindObj(K Key)
        {
            lock (this)
            {
                V fv;
                if (mObjects.TryGetValue(Key, out fv))
                    return fv;
                return default(V);
            }
        }

        public bool ContainsKey(K key)
        {
            lock (this)
            {
                return mObjects.ContainsKey(key);
            }
        }

        public delegate EForEachResult FOnVisitObject(K key, V value, object arg);
        List<K> RemoveList = new List<K>();
        public bool For_Each(FOnVisitObject fun, object arg)
        {
            lock (this)
            {
                if(mObjects.Count !=0)
                {
                    foreach (var i in mObjects)
                    {
                        EForEachResult eResult = fun(i.Key, i.Value, arg);
                        switch (eResult)
                        {
                            case EForEachResult.FER_Continue:
                                break;
                            case EForEachResult.FER_Stop:
                                return false;
                            case EForEachResult.FER_Erase:
                                RemoveList.Add(i.Key);
                                break;
                            default:
                                break;
                        }
                    }
                }
                for(int i=0;i< RemoveList.Count;i++)
                {
                    mObjects.Remove(RemoveList[i]);
                }
                RemoveList.Clear();

                return true;
            }
        }
        
        public bool TryGetValue(K key, out V value)
        {
            lock(this)
            {
                return mObjects.TryGetValue(key, out value);
            }
        }

        //void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> keyValuePair)
        //{
        //    ((IDictionary<K, V>)this).Add(keyValuePair.Key, keyValuePair.Value);
        //}

        //bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> keyValuePair)
        //{
        //    V value;
        //    if (!TryGetValue(keyValuePair.Key, out value))
        //        return false;

        //    return EqualityComparer<V>.Default.Equals(value, keyValuePair.Value);
        //}

        //void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int index)
        //{
        //    if (array == null) throw new ArgumentNullException("array");
        //    if (index < 0) throw new ArgumentOutOfRangeException("index", "Index is negative");

        //    if (array.Length - Count < index || Count < 0) //"count" itself or "count + index" can overflow
        //    {
        //        throw new ArgumentException("Array not large enough");
        //    }

        //    lock (this)
        //    {
        //        int idx = 0;
        //        foreach (var obj in mObjects)
        //        {
        //            array[idx] = new KeyValuePair<K, V>(obj.Key, obj.Value);

        //            idx++;
        //        }
        //    }
        //}

        //bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> keyValuePair)
        //{
        //    if (keyValuePair.Key == null) throw new ArgumentNullException("Item key is null");
        //    return Remove(keyValuePair.Key);
        //}

        //public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        //{
        //    return mObjects.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return ((ConcurentObjManager<K, V>)this).GetEnumerator();
        //}

        public bool Save(XmlNode node, XmlHolder holder)
        {
            lock(this)
            {
                foreach(var i in mObjects)
                {
                    var childNode = node.AddNode("Data", "", holder);

                    var keyNode = childNode.AddNode("Key", "", holder);
                    CSUtility.Support.IConfigurator.SaveValue(keyNode, i.Key.GetType(), i.Key, holder);
                    var valueNode = childNode.AddNode("Value", "", holder);
                    CSUtility.Support.IConfigurator.SaveValue(valueNode, i.Value.GetType(), i.Value, holder);
                }
            }
            return true;
        }
        public bool Load(XmlNode node)
        {
            lock(this)
            {
                mObjects.Clear();

                foreach (var cNode in node.GetNodes())
                {
                    var keyNode = cNode.FindNode("Key");
                    var keyTypeAtt = keyNode.FindAttrib("Type");
                    if (keyTypeAtt == null)
                        continue;
                    var keyType = CSUtility.Program.GetTypeFromSaveString(keyTypeAtt.Value);
                    var key = CSUtility.Support.IConfigurator.ReadValue(keyNode, keyType);

                    var valueNode = cNode.FindNode("Value");
                    var valueTypeAtt = valueNode.FindAttrib("Type");
                    if (valueTypeAtt == null)
                        continue;
                    var valueType = CSUtility.Program.GetTypeFromSaveString(valueTypeAtt.Value);
                    var value = CSUtility.Support.IConfigurator.ReadValue(valueNode, valueType);
                    
                    mObjects[(K)key] = (V)value;
                }
            }
            return true;
        }

        public bool Read(XndAttrib xndAtt)
        {
            lock(this)
            {
                mObjects.Clear();

                int count = 0;
                xndAtt.Read(out count);

                for(int i=0; i<count; i++)
                {
                    string keyTypeStr;
                    xndAtt.Read(out keyTypeStr);
                    var keyType = Program.GetTypeFromSaveString(keyTypeStr);
                    object key;
                    if (!CSUtility.Support.XndSaveLoadProxy.ReadValue(xndAtt, keyType, out key))
                        return false;

                    string valueTypeStr;
                    xndAtt.Read(out valueTypeStr);
                    var valueType = Program.GetTypeFromSaveString(valueTypeStr);
                    object value;
                    if (!CSUtility.Support.XndSaveLoadProxy.ReadValue(xndAtt, valueType, out value))
                        return false;

                    mObjects[(K)key] = (V)value;
                }

                return true;
            }
        }
        public bool Write(XndAttrib xndAtt)
        {
            lock(this)
            {
                int count = mObjects.Count;
                xndAtt.Write(count);

                foreach(var i in mObjects)
                {
                    var keyType = i.Key.GetType();
                    var keyTypeStr = Program.GetTypeSaveString(keyType);
                    xndAtt.Write(keyTypeStr);
                    CSUtility.Support.XndSaveLoadProxy.WriteValue(xndAtt, keyType, i.Key);

                    var valueType = i.Value.GetType();
                    var valueTypeStr = Program.GetTypeSaveString(valueType);
                    xndAtt.Write(valueTypeStr);
                    CSUtility.Support.XndSaveLoadProxy.WriteValue(xndAtt, valueType, i.Value);
                }

                return true;
            }
        }
        public bool CopyFrom(ICopyable srcData)
        {
            lock(this)
            {
                var com = srcData as ConcurentObjManager<K, V>;
                if (com == null)
                    return false;

                mObjects.Clear();

                foreach(var obj in com.Objects)
                {
                    var copyedKey = (K)(Copyable.CopyValue(obj.Key));
                    var copyedValue = (V)(Copyable.CopyValue(obj.Value));
                    mObjects.Add(copyedKey, copyedValue);
                }

                return true;
            }
        }
    }
}