using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Network.Connections
{
    public interface IConnection : IDisposable, IAsyncDisposable
    {
        bool IsConnected { get; }

        long TotalBytesSent { get; }
        long TotalBytesReceived { get; }

        void DoEvents();

        ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default);
        ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default);
    }
}
