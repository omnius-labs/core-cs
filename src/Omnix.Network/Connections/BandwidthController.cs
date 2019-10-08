using System;
using System.Collections.Generic;
using System.Linq;
using Omnix.Base;

namespace Omnix.Network.Connections
{
    public sealed class BandwidthController
    {
        public Limiter SendBytesLimiter { get; } = new Limiter();
        public Limiter ReceiveBytesLimiter { get; } = new Limiter();

        public sealed class Limiter : ISynchronized
        {
            private readonly Queue<(DateTime time, int size)> _queue = new Queue<(DateTime time, int size)>();

            public int MaxBytesPerSecond { get; set; }
            public object LockObject { get; } = new object();

            public int ComputeFreeBytes()
            {
                lock (this.LockObject)
                {
                    var now = DateTime.UtcNow;
                    var lowerLimit = now.AddSeconds(-1);

                    while (_queue.Peek().time < lowerLimit)
                    {
                        _queue.Dequeue();
                    }

                    int result = this.MaxBytesPerSecond - _queue.ToArray().Sum(n => n.size);

                    return result;
                }
            }

            public void AddConsumedBytes(int size)
            {
                if (size < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(size));
                }

                lock (this.LockObject)
                {
                    var now = DateTime.UtcNow;
                    _queue.Enqueue((now, size));
                }
            }
        }
    }
}
