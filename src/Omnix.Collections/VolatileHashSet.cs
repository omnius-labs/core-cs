using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Omnix.Base;

namespace Omnix.Collections
{
    public class VolatileHashSet<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISet<T>, IVolatileCollection<T>, ISynchronized
        where T : notnull
    {
        private readonly Dictionary<T, DateTime> _dic;
        private readonly TimeSpan _survivalTime;

        private readonly object _lockObject = new object();

        public VolatileHashSet(TimeSpan survivalTime)
        {
            _dic = new Dictionary<T, DateTime>();
            _survivalTime = survivalTime;
        }

        public VolatileHashSet(TimeSpan survivalTime, IEqualityComparer<T> comparer)
        {
            _dic = new Dictionary<T, DateTime>(comparer);
            _survivalTime = survivalTime;
        }

        public object LockObject { get; } = new object();

        public TimeSpan SurvivalTime => _survivalTime;

        public T[] ToArray()
        {
            lock (this.LockObject)
            {
                return _dic.Keys.ToArray();
            }
        }

        public T[] ToArray(TimeSpan span)
        {
            var now = DateTime.UtcNow;

            lock (this.LockObject)
            {
                var list = new List<T>(_dic.Count);

                foreach (var (key, value) in _dic)
                {
                    if ((now - value) < span)
                    {
                        list.Add(key);
                    }
                }

                return list.ToArray();
            }
        }

        public void Refresh()
        {
            var now = DateTime.UtcNow;

            lock (this.LockObject)
            {
                var list = new List<T>();

                foreach (var (key, value) in _dic)
                {
                    if ((now - value) > _survivalTime)
                    {
                        list.Add(key);
                    }
                }

                foreach (var key in list)
                {
                    _dic.Remove(key);
                }
            }
        }

        public TimeSpan GetElapsedTime(T item)
        {
            if (!_dic.TryGetValue(item, out var updateTime))
            {
                return _survivalTime;
            }

            var now = DateTime.UtcNow;
            return (now - updateTime);
        }

        public IEqualityComparer<T> Comparer
        {
            get
            {
                lock (this.LockObject)
                {
                    return _dic.Comparer;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (this.LockObject)
                {
                    return _dic.Count;
                }
            }
        }

        public bool Add(T item)
        {
            lock (this.LockObject)
            {
                int count = _dic.Count;
                _dic[item] = DateTime.UtcNow;

                return (count != _dic.Count);
            }
        }

        public void Clear()
        {
            lock (this.LockObject)
            {
                _dic.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this.LockObject)
            {
                return _dic.ContainsKey(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _dic.Keys.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (this.LockObject)
            {
                return _dic.Remove(item);
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item)
        {
            lock (this.LockObject)
            {
                this.Add(item);
            }
        }

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => this.LockObject;

        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.LockObject)
            {
                ((ICollection)_dic.Keys).CopyTo(array, index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this.LockObject)
            {
                foreach (var item in _dic.Keys)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.LockObject)
            {
                return this.GetEnumerator();
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                var now = DateTime.UtcNow;

                foreach (var value in other)
                {
                    _dic[value] = now;
                }
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                foreach (var value in other)
                {
                    _dic.Remove(value);
                }
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                var now = DateTime.UtcNow;
                var tempList = new List<T>();

                foreach (var value in other)
                {
                    if (_dic.ContainsKey(value))
                    {
                        continue;
                    }

                    tempList.Add(value);
                }

                foreach (var key in tempList)
                {
                    _dic.Remove(key);
                }
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
    }
}
