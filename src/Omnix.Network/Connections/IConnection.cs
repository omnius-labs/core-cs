using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Network.Connections
{
    public interface IConnection : IDisposable, IAsyncDisposable
    {
        bool IsConnected { get; }

        long TotalBytesSent { get; }
        long TotalBytesReceived { get; }

        void DoEvents();

        ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default);
        ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default);
    }
}
