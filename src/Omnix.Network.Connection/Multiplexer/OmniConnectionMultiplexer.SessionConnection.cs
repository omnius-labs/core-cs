using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;

namespace Omnix.Network.Connection.Multiplexer
{
    partial class OmniConnectionMultiplexer<TPriority>
    {
        private sealed class SessionConnection : DisposableBase, IConnection
        {
            private OmniConnectionMultiplexer<TPriority> _multiplexer;

            private volatile bool _disposed;

            public SessionConnection(ulong id, OmniConnectionMultiplexer<TPriority> _multiplexer)
            {
                this.Id = id;
            }

            public ulong Id { get; }

            public bool IsConnected { get; }
            public long ReceivedByteCount { get; }
            public long SentByteCount { get; }

            public void DoEvents()
            {
                throw new NotImplementedException();
            }

            public bool TryEnqueue(Action<IBufferWriter<byte>> action)
            {
                return _multiplexer.TryEnqueue(this.Id, action);
            }

            public async ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default)
            {
                await _multiplexer.EnqueueAsync(this.Id, action, token);
            }

            public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
            {
                return _multiplexer.TryDequeue(this.Id, action);
            }

            public async ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
            {
                await _multiplexer.DequeueAsync(this.Id, action, token);
            }

            protected override void Dispose(bool disposing)
            {
                if (_disposed) return;
                _disposed = true;

                if (disposing)
                {
                    _multiplexer.CloseConnection(this.Id);
                }
            }
        }
    }
}
