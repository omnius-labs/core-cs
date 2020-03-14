using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Network.Connections
{
    public interface IConnection : IDisposable
    {
        bool IsConnected { get; }

        long TotalBytesSent { get; }
        long TotalBytesReceived { get; }

        void RunJobs();

        bool TrySend(Action<IBufferWriter<byte>> action);
        ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default);

        bool TryReceive(Action<ReadOnlySequence<byte>> action);
        ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default);
    }
}
