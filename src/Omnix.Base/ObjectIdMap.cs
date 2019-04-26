using System.Collections;
using System.Collections.Generic;

namespace Omnix.Base
{
    public sealed class ObjectIdMap<T> : IEnumerable<KeyValuePair<int, T>>, IEnumerable
    {
        private Dictionary<T, int> _objectMap = new Dictionary<T, int>();
        private Dictionary<int, T> _idMap = new Dictionary<int, T>();
        private int _current;

        private readonly object _lockObject = new object();

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
            if (_idMap.TryGetValue(id, out T item)) return item;

            throw new KeyNotFoundException();
        }

        public void Remove(int id)
        {
            T item;
            if (!_idMap.TryGetValue(id, out item)) return;

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
