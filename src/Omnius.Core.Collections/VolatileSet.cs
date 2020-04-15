using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Omnius.Core.Collections
{
    public class VolatileSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IVolatile<T>
        where T : notnull
    {
        private readonly Dictionary<T, DateTime> _dic;
        private readonly TimeSpan _survivalTime;

        public VolatileSet(TimeSpan survivalTime)
        {
            _dic = new Dictionary<T, DateTime>();
            _survivalTime = survivalTime;
        }

        public VolatileSet(TimeSpan survivalTime, IEqualityComparer<T> comparer)
        {
            _dic = new Dictionary<T, DateTime>(comparer);
            _survivalTime = survivalTime;
        }

        public TimeSpan SurvivalTime => _survivalTime;

        public T[] ToArray()
        {
            return _dic.Keys.ToArray();
        }

        public T[] ToArray(TimeSpan span)
        {
            var now = DateTime.UtcNow;

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

        public void Refresh()
        {
            var now = DateTime.UtcNow;

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
                return _dic.Comparer;
            }
        }

        public int Count
        {
            get
            {
                return _dic.Count;
            }
        }

        public bool Add(T item)
        {
            int count = _dic.Count;
            _dic[item] = DateTime.UtcNow;

            return (count != _dic.Count);
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public bool Contains(T item)
        {
            return _dic.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _dic.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _dic.Remove(item);
        }

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _dic.Keys)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            var now = DateTime.UtcNow;

            foreach (var value in other)
            {
                _dic[value] = now;
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var value in other)
            {
                _dic.Remove(value);
            }
        }

        public void IntersectWith(IEnumerable<T> other)
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
