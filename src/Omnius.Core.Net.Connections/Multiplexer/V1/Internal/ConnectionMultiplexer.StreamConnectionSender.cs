using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal partial class ConnectionMultiplexer
    {
        public sealed class StreamConnectionSender : IConnectionSender
        {
            private readonly SemaphoreSlim _semaphoreSlim;
            private readonly IMessagePipeWriter<ArraySegment<byte>> _dataWriter;
            private readonly IActionSubscriber _dataAcceptedSubscriber;
            private readonly IBytesPool _bytesPool;
            private readonly CancellationToken _cancellationToken;

            public StreamConnectionSender(int maxDataQueueSize, IMessagePipeWriter<ArraySegment<byte>> dataWriter, IActionSubscriber dataAcceptedSubscriber, IBytesPool bytesPool, CancellationToken cancellationToken, List<IDisposable> disposables)
            {
                _semaphoreSlim = new SemaphoreSlim(0, maxDataQueueSize);
                _dataWriter = dataWriter;
                _dataAcceptedSubscriber = dataAcceptedSubscriber;
                disposables.Add(_dataAcceptedSubscriber.Subscribe(() => _semaphoreSlim.Release()));
                _bytesPool = bytesPool;
                _cancellationToken = cancellationToken;
            }

            public long TotalBytesSent => throw new NotImplementedException();

            public async ValueTask WaitToSendAsync(CancellationToken cancellationToken)
            {
                try
                {
                    using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

                    await _semaphoreSlim.WaitAsync(linkedTokenSource.Token);
                    _semaphoreSlim.Release();

                    await _dataWriter.WaitToWriteAsync(linkedTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    if (_cancellationToken.IsCancellationRequested) throw new Exception(); // FIXME
                }
            }

            public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
            {
                try
                {
                    using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

                    await _semaphoreSlim.WaitAsync(linkedTokenSource.Token);

                    await _dataWriter.WriteAsync(() => this.InternalSend(action), linkedTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    if (_cancellationToken.IsCancellationRequested) throw new Exception(); // FIXME
                }
            }

            public bool TrySend(Action<IBufferWriter<byte>> action)
            {
                if (!_semaphoreSlim.Wait(0)) return false;

                return _dataWriter.TryWrite(() => this.InternalSend(action));
            }

            private ArraySegment<byte> InternalSend(Action<IBufferWriter<byte>> action)
            {
                using var bytesPipe = new BytesPipe(_bytesPool);
                action.Invoke(bytesPipe.Writer);
                var sequence = bytesPipe.Reader.GetSequence();
                var buffer = _bytesPool.Array.Rent((int)sequence.Length);
                sequence.CopyTo(buffer.AsSpan());
                return new ArraySegment<byte>(buffer, 0, (int)sequence.Length);
            }
        }
    }
}
