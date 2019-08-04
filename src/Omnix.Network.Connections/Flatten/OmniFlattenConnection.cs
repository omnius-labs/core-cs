using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;

namespace Omnix.Network.Connections.Flatten
{
    public sealed class OmniFlattenConnection : DisposableBase, IConnection
    {
        public bool IsConnected { get; }
        public long ReceivedByteCount { get; }
        public long SentByteCount { get; }

        public void DoEvents()
        {
            throw new NotImplementedException();
        }

        public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
        {
            throw new NotImplementedException();
        }

        public ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Dequeue(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public bool TryEnqueue(Action<IBufferWriter<byte>> action)
        {
            throw new NotImplementedException();
        }

        public ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Enqueue(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        protected override void OnDispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        protected override ValueTask OnDisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
