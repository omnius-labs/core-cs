using System.Collections;
using System.Collections.Generic;

namespace Omnius.Core
{
    public sealed class ObjectIdMap<T> : IEnumerable<KeyValuePair<int, T>>, IEnumerable
        where T : notnull
    {
        private readonly Dictionary<T, int> _objectMap = new Dictionary<T, int>();
        private readonly Dictionary<int, T> _idMap = new Dictionary<int, T>();
        private int _current;

        public int Add(T item)
        {
            int id;

            for (; ; )
            {
                id = _current++;
                if (!_idMap.ContainsKey(id)) break;
            }

            _objectMap.Add(item, id);
            _idMap.Add(id, item);

            return id;
        }

        public int GetId(T item)
        {
            if (_objectMap.TryGetValue(item, out int id)) return id;

            throw new KeyNotFoundException();
        }

        public T GetItem(int id)
        {
            if (_idMap.TryGetValue(id, out var item)) return item;

            throw new KeyNotFoundException();
        }

        public void Remove(int id)
        {
            if (!_idMap.TryGetValue(id, out var item)) return;

            _idMap.Remove(id);
            _objectMap.Remove(item);
        }

        public void Clear()
        {
            _objectMap.Clear();
            _idMap.Clear();
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            foreach (var item in _idMap)
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
