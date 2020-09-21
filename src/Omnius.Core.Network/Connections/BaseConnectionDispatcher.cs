using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Extensions;

namespace Omnius.Core.Network.Connections
{
    public sealed class BaseConnectionDispatcher : AsyncDisposableBase
    {
        private readonly BaseConnectionDispatcherOptions _options;

        private readonly HashSet<BaseConnection> _connections = new HashSet<BaseConnection>();

        private readonly Task _eventLoop;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly object _lockObject = new object();

        public BaseConnectionDispatcher(BaseConnectionDispatcherOptions options)
        {
            _options = options;
            _eventLoop = this.EventLoop(_cancellationTokenSource.Token);
        }

        protected override async ValueTask OnDisposeAsync()
        {
            _cancellationTokenSource.Cancel();

            await _eventLoop;

            _cancellationTokenSource.Dispose();
        }

        private Task EventLoop(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var sendBytesLimiter = new Limiter(_options.MaxSendBytesPerSeconds);
                    var receiveBytesLimiter = new Limiter(_options.MaxReceiveBytesPerSeconds);
                    var random = new Random();

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.WaitHandle.WaitOne(200, cancellationToken);

                        List<BaseConnection>? tempList = null;

                        lock (_lockObject)
                        {
                            tempList = _connections.ToList();
                        }

                        random.Shuffle(tempList);
                        foreach (var connection in tempList)
                        {
                            try
                            {
                                // Send
                                {
                                    var freeBytes = sendBytesLimiter.ComputeFreeBytes();
                                    var consumedBytes = connection.Send(freeBytes);
                                    sendBytesLimiter.AddConsumedBytes(consumedBytes);
                                }

                                // Receive
                                {
                                    var freeBytes = receiveBytesLimiter.ComputeFreeBytes();
                                    var consumedBytes = connection.Receive(freeBytes);
                                    receiveBytesLimiter.AddConsumedBytes(consumedBytes);
                                }
                            }
                            catch (ObjectDisposedException)
                            {
                                _connections.Remove(connection);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {

                }
            }, TaskCreationOptions.LongRunning);
        }

        internal void Add(BaseConnection connection)
        {
            lock (_lockObject)
            {
                _connections.Add(connection);
            }
        }

        private sealed class Limiter
        {
            private readonly Queue<(DateTime time, int size)> _queue = new Queue<(DateTime time, int size)>();
            private readonly int _maxBytesPerSecond;

            public Limiter(int maxBytesPerSecond)
            {
                _maxBytesPerSecond = maxBytesPerSecond;
            }

            public int ComputeFreeBytes()
            {
                var now = DateTime.UtcNow;
                var lowerLimit = now.AddSeconds(-1);

                while (_queue.Count > 0 && _queue.Peek().time < lowerLimit)
                {
                    _queue.Dequeue();
                }

                int result = _maxBytesPerSecond - _queue.ToArray().Sum(n => n.size);

                return result;
            }

            public void AddConsumedBytes(int size)
            {
                if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));

                var now = DateTime.UtcNow;
                _queue.Enqueue((now, size));
            }
        }
    }
}
