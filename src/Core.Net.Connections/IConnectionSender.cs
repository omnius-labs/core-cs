using System.Buffers;

namespace Omnius.Core.Net.Connections;

public interface IConnectionSender
{
    long TotalBytesSent { get; }

    ValueTask WaitToSendAsync(CancellationToken cancellationToken = default);
    bool TrySend(Action<IBufferWriter<byte>> action);
    ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default);
}
