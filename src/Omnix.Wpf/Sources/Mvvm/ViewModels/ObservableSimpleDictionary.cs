using Omnius.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Omnius.Wpf
{
    public class ObservableSimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dic;
        private ObservableCollection<TValue> _collection = new ObservableCollection<TValue>();

        public ObservableSimpleDictionary()
            : this(EqualityComparer<TKey>.Default)
        {

        }

        public ObservableSimpleDictionary(IEqualityComparer<TKey> equalityComparer)
        {
            _dic = new Dictionary<TKey, TValue>(equalityComparer);
        }

        public void Sort(Comparison<TValue> comparison)
        {
            var list = _collection.ToList();
            list.Sort(comparison);

            for (int i = 0; i < list.Count; i++)
            {
                int o = _collection.IndexOf(list[i]);
                if (i != o) _collection.Move(o, i);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _dic[key];
            }
            set
            {
                if (_dic.TryGetValue(key, out var oldValue))
                {
                    _collection.Remove(oldValue);
                }

                _dic[key] = value;
                _collection.Add(value);
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return _dic.Keys;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return _dic.Values;
            }
        }

        private ReadOnlyObservableCollection<TValue> _readOnlyValues;

        public ReadOnlyObservableCollection<TValue> Values
        {
            get
            {
                if (_readOnlyValues == null)
                    _readOnlyValues = new ReadOnlyObservableCollection<TValue>(_collection);

                return _readOnlyValues;
            }
        }

        public int Count
        {
            get
            {
                return _dic.Count;
            }
        }

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            this[key] = value;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this[item.Key] = item.Value;
        }

        public void Clear()
        {
            _dic.Clear();
            _collection.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dic).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _dic.ContainsKey(key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dic).CopyTo(array, arrayIndex);
        }

        public bool Remove(TKey key)
        {
            if (_dic.TryRemove(key, out var oldValue))
            {
                _collection.Remove(oldValue);
                return true;
            }

            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (((ICollection<KeyValuePair<TKey, TValue>>)_dic).Remove(item))
            {
                _collection.Remove(item.Value);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dic.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in _dic)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
