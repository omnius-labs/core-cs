using System;
using System.Collections;
using System.Collections.Generic;
using Omnix.Base;

namespace Omnix.DataStructures.Collections
{
    public class LockedQueue<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISynchronized
    {
        private readonly Queue<T> _queue;
        private int? _capacity;

        public LockedQueue()
        {
            _queue = new Queue<T>();
        }

        public LockedQueue(int capacity)
        {
            _queue = new Queue<T>();
            _capacity = capacity;
        }

        public LockedQueue(IEnumerable<T> collection)
        {
            _queue = new Queue<T>();

            foreach (var item in collection)
            {
                this.Enqueue(item);
            }
        }

        public object LockObject { get; } = new object();

        public int? Capacity
        {
            get
            {
                lock (this.LockObject)
                {
                    return _capacity;
                }
            }
            set
            {
                lock (this.LockObject)
                {
                    _capacity = value;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (this.LockObject)
                {
                    return _queue.Count;
                }
            }
        }

        public void Clear()
        {
            lock (this.LockObject)
            {
                _queue.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this.LockObject)
            {
                return _queue.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _queue.CopyTo(array, arrayIndex);
            }
        }

        public T Dequeue()
        {
            lock (this.LockObject)
            {
                return _queue.Dequeue();
            }
        }

        public void Enqueue(T item)
        {
            lock (this.LockObject)
            {
                if (_capacity != null && _queue.Count + 1 > _capacity.Value)
                {
                    throw new OverflowException();
                }

                _queue.Enqueue(item);
            }
        }

        public T Peek()
        {
            lock (this.LockObject)
            {
                return _queue.Peek();
            }
        }

        public T[] ToArray()
        {
            lock (this.LockObject)
            {
                return _queue.ToArray();
            }
        }

        public void TrimExcess()
        {
            lock (this.LockObject)
            {
                _queue.TrimExcess();
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item)
        {
            lock (this.LockObject)
            {
                this.Enqueue(item);
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            lock (this.LockObject)
            {
                return ((ICollection<T>)_queue).Remove(item);
            }
        }

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => this.LockObject;

        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.LockObject)
            {
                ((ICollection)_queue).CopyTo(array, index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this.LockObject)
            {
                foreach (var item in _queue)
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
    }
}
