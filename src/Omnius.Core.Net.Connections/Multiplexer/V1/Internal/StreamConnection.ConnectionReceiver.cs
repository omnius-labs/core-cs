using System.Buffers;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal partial class StreamConnection
{
    public sealed class ConnectionReceiver : DisposableBase, IConnectionReceiver
    {
        private readonly IMessagePipeReader<ArraySegment<byte>> _dataReader;
        private readonly IMessagePipeWriter _dataAcceptedWriter;
        private readonly IBytesPool _bytesPool;
        private readonly CancellationToken _cancellationToken;

        private long _totalBytesReceived;

        public ConnectionReceiver(IMessagePipeReader<ArraySegment<byte>> dataReader, IMessagePipeWriter dataAcceptedWriter, IBytesPool bytesPool, CancellationToken cancellationToken)
        {
            _dataReader = dataReader;
            _dataAcceptedWriter = dataAcceptedWriter;
            _bytesPool = bytesPool;
            _cancellationToken = cancellationToken;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public long TotalBytesReceived => _totalBytesReceived;

        public async ValueTask WaitToReceiveAsync(CancellationToken cancellationToken = default)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

            await _dataReader.WaitToReadAsync(linkedTokenSource.Token);
        }

        public async ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

            var payload = await _dataReader.ReadAsync(linkedTokenSource.Token);
            this.InternalReceive(action, payload);
        }

        public bool TryReceive(Action<ReadOnlySequence<byte>> action)
        {
            if (!_dataReader.TryRead(out var payload)) return false;
            this.InternalReceive(action, payload);
            return true;
        }

        private void InternalReceive(Action<ReadOnlySequence<byte>> action, ArraySegment<byte> buffer)
        {
            action.Invoke(new ReadOnlySequence<byte>(buffer.Array!, buffer.Offset, buffer.Count));
            Interlocked.Add(ref _totalBytesReceived, buffer.Count);
            _bytesPool.Array.Return(buffer.Array!);
            _dataAcceptedWriter.TryWrite();
        }
    }
}
