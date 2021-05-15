using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Connections
{
    public interface IConnection : IDisposable
    {
        bool IsConnected { get; }

        long TotalBytesSent { get; }

        long TotalBytesReceived { get; }

        bool TryEnqueue(Action<IBufferWriter<byte>> action);

        ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default);

        bool TryDequeue(Action<ReadOnlySequence<byte>> action);

        ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default);
    }
}
