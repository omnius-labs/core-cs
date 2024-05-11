using System.Buffers;

namespace Core.Net.Connections;

public interface IConnectionReceiver
{
    long TotalBytesReceived { get; }

    ValueTask WaitToReceiveAsync(CancellationToken cancellationToken = default);
    bool TryReceive(Action<ReadOnlySequence<byte>> action);
    ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default);
}
