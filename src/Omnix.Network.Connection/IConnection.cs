using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Network.Connection
{
    public interface IConnection : IDisposable
    {
        bool IsConnected { get; }
        long ReceivedByteCount { get; }
        long SentByteCount { get; }

        void DoEvents();

        bool TryEnqueue(Action<IBufferWriter<byte>> action);
        ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default);
        void Enqueue(Action<IBufferWriter<byte>> action, CancellationToken token = default);

        bool TryDequeue(Action<ReadOnlySequence<byte>> action);
        ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default);
        void Dequeue(Action<ReadOnlySequence<byte>> action, CancellationToken token = default);
    }
}
