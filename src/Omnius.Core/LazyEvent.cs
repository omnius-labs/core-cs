using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Extensions;

namespace Omnius.Core
{
    public sealed class LazyEvent<T> : DisposableBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Delegate, EventItem> _events = new Dictionary<Delegate, EventItem>();

        private readonly object _lockObject = new object();

        public LazyEvent()
        {

        }

        public LazyEvent(TimeSpan delay)
        {
            this.Delay = delay;
        }

        public TimeSpan? Delay { get; set; }

        public void Enqueue(params T[] items)
        {
            this.ThrowIfDisposingRequested();

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
            this.ThrowIfDisposingRequested();

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
                this.ThrowIfDisposingRequested();

                lock (_lockObject)
                {
                    _events.Add(value, new EventItem(value, this.Delay));
                }
            }
            remove
            {
                this.ThrowIfDisposingRequested();

                lock (_lockObject)
                {
                    if (!_events.TryGetValue(value, out var item))
                    {
                        return;
                    }

                    item.Dispose();
                    _events.Remove(value);
                }
            }
        }

        private class EventItem : DisposableBase
        {
            private readonly Action<IEnumerable<T>> _action;
            private readonly TimeSpan? _delay;

            private readonly Task _task;

            private readonly LinkedList<T> _queue = new LinkedList<T>();
            private readonly ManualResetEvent _queueResetEvent = new ManualResetEvent(false);
            private readonly ManualResetEvent _delayResetEvent = new ManualResetEvent(false);

            private readonly object _lockObject = new object();

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
                            if (this.IsDisposed)
                            {
                                return;
                            }

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

            protected override void OnDispose(bool disposing)
            {
                if (disposing)
                {
                    _queueResetEvent.Set();
                    _queueResetEvent.Dispose();

                    _delayResetEvent.Set();
                    _delayResetEvent.Dispose();

                    _task.Wait();
                }
            }

            protected override async ValueTask OnDisposeAsync()
            {
                _queueResetEvent.Set();
                _queueResetEvent.Dispose();

                _delayResetEvent.Set();
                _delayResetEvent.Dispose();

                await _task;
            }
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var eventItem in _events.Values)
                {
                    eventItem.Dispose();
                }

                _events.Clear();
            }
        }

        protected override async ValueTask OnDisposeAsync()
        {
            var valueTaskList = new List<ValueTask>();

            foreach (var eventItem in _events.Values)
            {
                valueTaskList.Add(eventItem.DisposeAsync());
            }

            await ValueTaskHelper.WhenAll(valueTaskList.ToArray());

            _events.Clear();
        }
    }
}
