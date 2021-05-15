using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Extensions;

namespace Omnius.Core.Net.Connections
{
    public sealed class BaseConnectionDispatcher : AsyncDisposableBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly BaseConnectionDispatcherOptions _options;

        private readonly HashSet<BaseConnection> _connections = new();

        private readonly Task _eventLoop;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly object _lockObject = new();

        public BaseConnectionDispatcher(BaseConnectionDispatcherOptions options)
        {
            _options = options;
            _eventLoop = this.EventLoopAsync(_cancellationTokenSource.Token);
        }

        protected override async ValueTask OnDisposeAsync()
        {
            _cancellationTokenSource.Cancel();

            await _eventLoop;

            _cancellationTokenSource.Dispose();
        }

        private Task EventLoopAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        var sendBytesLimiter = new Limiter(_options.MaxSendBytesPerSeconds);
                        var receiveBytesLimiter = new Limiter(_options.MaxReceiveBytesPerSeconds);
                        var random = new Random();

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.WaitHandle.WaitOne(200, cancellationToken);

                            lock (_lockObject)
                            {
                                foreach (var connection in _connections.Randomize())
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
                            }
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        _logger.Debug(e);
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

        internal void Remove(BaseConnection connection)
        {
            lock (_lockObject)
            {
                _connections.Remove(connection);
            }
        }

        private sealed class Limiter
        {
            private readonly Queue<(DateTime time, int size)> _queue = new();
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
