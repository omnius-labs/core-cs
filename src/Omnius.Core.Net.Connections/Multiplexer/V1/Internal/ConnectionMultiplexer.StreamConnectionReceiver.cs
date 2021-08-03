using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal partial class ConnectionMultiplexer
    {
        public sealed class StreamConnectionReceiver : IConnectionReceiver
        {
            private readonly IMessagePipeReader<ArraySegment<byte>> _dataReader;
            private readonly IMessagePipeWriter _dataAcceptedWriter;
            private readonly IBytesPool _bytesPool;
            private readonly CancellationToken _cancellationToken;

            public StreamConnectionReceiver(IMessagePipeReader<ArraySegment<byte>> dataReader, IMessagePipeWriter dataAcceptedWriter, IBytesPool bytesPool, CancellationToken cancellationToken)
            {
                _dataReader = dataReader;
                _dataAcceptedWriter = dataAcceptedWriter;
                _bytesPool = bytesPool;
                _cancellationToken = cancellationToken;
            }

            public long TotalBytesReceived => throw new NotImplementedException();

            public async ValueTask WaitToReceiveAsync(CancellationToken cancellationToken = default)
            {
                try
                {
                    using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

                    await _dataReader.WaitToReadAsync(linkedTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    if (_cancellationToken.IsCancellationRequested) throw new Exception(); // FIXME
                }
            }

            public async ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
            {
                try
                {
                    using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

                    var buffer = await _dataReader.ReadAsync(linkedTokenSource.Token);
                    this.InternalReceive(action, buffer);
                }
                catch (OperationCanceledException)
                {
                    if (_cancellationToken.IsCancellationRequested) throw new Exception(); // FIXME
                }
            }

            public bool TryReceive(Action<ReadOnlySequence<byte>> action)
            {
                try
                {
                    if (!_dataReader.TryRead(out var buffer)) return false;
                    this.InternalReceive(action, buffer);
                    return true;
                }
                catch (OperationCanceledException)
                {
                    if (_cancellationToken.IsCancellationRequested) throw new Exception(); // FIXME
                }

                return false;
            }

            private void InternalReceive(Action<ReadOnlySequence<byte>> action, ArraySegment<byte> buffer)
            {
                action.Invoke(new ReadOnlySequence<byte>(buffer.Array!, buffer.Offset, buffer.Count));
                _bytesPool.Array.Return(buffer.Array!);
                _dataAcceptedWriter.TryWrite();
            }
        }
    }
}
