using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Connections
{
    public interface IConnectionReceiver
    {
        long TotalBytesReceived { get; }

        ValueTask WaitToReceiveAsync(CancellationToken cancellationToken = default);

        bool TryReceive(Action<ReadOnlySequence<byte>> action);

        ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default);
    }
}
