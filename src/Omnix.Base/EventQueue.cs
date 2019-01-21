using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Base
{
    public sealed class EventQueue<T> : DisposableBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private Dictionary<Delegate, EventItem> _events = new Dictionary<Delegate, EventItem>();

        private readonly object _lockObject = new object();
        private volatile bool _isDisposed;

        public EventQueue()
        {

        }

        public EventQueue(TimeSpan delay)
        {
            this.Delay = delay;
        }

        public TimeSpan? Delay { get; set; }

        public void Enqueue(params T[] items)
        {
            if (_isDisposed) throw new ObjectDisposedException(this.GetType().FullName);

            lock (_lockObject)
            {
                foreach (var eventItem in _events.Values)
                {
                    eventItem.Enqueue(items);
                }
            }
        }

        public void Enqueue(IEnumerable<T> items)
        {
            if (_isDisposed) throw new ObjectDisposedException(this.GetType().FullName);

            lock (_lockObject)
            {
                foreach (var eventItem in _events.Values)
                {
                    eventItem.Enqueue(items);
                }
            }
        }

        public event Action<IEnumerable<T>> Events
        {
            add
            {
                if (_isDisposed) throw new ObjectDisposedException(this.GetType().FullName);

                lock (_lockObject)
                {
                    _events.Add(value, new EventItem(value, Delay));
                }
            }
            remove
            {
                if (_isDisposed) throw new ObjectDisposedException(this.GetType().FullName);

                lock (_lockObject)
                {
                    EventItem item;
                    if (!_events.TryGetValue(value, out item)) return;

                    item.Dispose();
                    _events.Remove(value);
                }
            }
        }

        class EventItem : DisposableBase
        {
            private Action<IEnumerable<T>> _action;
            private TimeSpan? _delay;

            private Task _task;

            private LinkedList<T> _queue = new LinkedList<T>();
            private volatile ManualResetEvent _queueResetEvent = new ManualResetEvent(false);
            private volatile ManualResetEvent _delayResetEvent = new ManualResetEvent(false);

            private readonly object _lockObject = new object();
            private volatile bool _isDisposed;

            public EventItem(Action<IEnumerable<T>> action, TimeSpan? delay)
            {
                _action = action;
                _delay = delay;

                _task = new Task(this.WatchThread);
                _task.Start();
            }

            private void WatchThread()
            {
                try
                {
                    for (; ; )
                    {
                        IEnumerable<T> result;

                        for (; ; )
                        {
                            if (_isDisposed) return;

                            lock (_lockObject)
                            {
                                if (_queue.Count > 0)
                                {
                                    result = _queue.ToList();
                                    _queue.Clear();

                                    break;
                                }
                                else
                                {
                                    _queueResetEvent.Reset();
                                }
                            }

                            _queueResetEvent.WaitOne();

                            // Delay
                            if (_delay != null)
                            {
                                _delayResetEvent.WaitOne(_delay.Value);
                            }
                        }

                        try
                        {
                            _action(result);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw ex;
                }
            }

            public void Enqueue(IEnumerable<T> items)
            {
                lock (_lockObject)
                {
                    foreach (var item in items)
                    {
                        _queue.AddLast(item);
                    }

                    _queueResetEvent.Set();
                }
            }

            protected override void Dispose(bool isDisposing)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                if (isDisposing)
                {
                    if (_queueResetEvent != null)
                    {
                        _queueResetEvent.Set();
                        _queueResetEvent.Dispose();

                        _queueResetEvent = null;
                    }

                    if (_delayResetEvent != null)
                    {
                        _delayResetEvent.Set();
                        _delayResetEvent.Dispose();

                        _delayResetEvent = null;
                    }

                    _task.Wait();
                }
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            _isDisposed = true;

            if (isDisposing)
            {
                foreach (var eventItem in _events.Values)
                {
                    eventItem.Dispose();
                }

                _events.Clear();
            }
        }
    }
}
