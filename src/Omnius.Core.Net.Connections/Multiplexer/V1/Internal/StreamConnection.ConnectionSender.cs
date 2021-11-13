using System.Buffers;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal partial class StreamConnection
{
    public sealed class ConnectionSender : DisposableBase, IConnectionSender
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IMessagePipeWriter<ArraySegment<byte>> _dataWriter;
        private readonly IActionSubscriber _dataAcceptedSubscriber;
        private readonly IBytesPool _bytesPool;
        private readonly CancellationToken _cancellationToken;

        private readonly List<IDisposable> _disposables = new();

        public ConnectionSender(int maxDataQueueSize, IMessagePipeWriter<ArraySegment<byte>> dataWriter, IActionSubscriber dataAcceptedSubscriber, IBytesPool bytesPool, CancellationToken cancellationToken)
        {
            _semaphoreSlim = new SemaphoreSlim(maxDataQueueSize, maxDataQueueSize);
            _dataWriter = dataWriter;
            _dataAcceptedSubscriber = dataAcceptedSubscriber;
            _dataAcceptedSubscriber.Subscribe(() => _semaphoreSlim.Release()).ToAdd(_disposables);
            _bytesPool = bytesPool;
            _cancellationToken = cancellationToken;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }
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
                // FIXME: new Exception()
                if (_cancellationToken.IsCancellationRequested) throw new Exception();
            }
        }

        public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            try
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

                await _semaphoreSlim.WaitAsync(linkedTokenSource.Token);

                var payload = this.GetPayload(action);
                if (payload.Count == 0)
                {
                    _semaphoreSlim.Release();
                    return;
                }

                await _dataWriter.WriteAsync(() => payload, linkedTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // FIXME: new Exception()
                if (_cancellationToken.IsCancellationRequested) throw new Exception();
            }
        }

        public bool TrySend(Action<IBufferWriter<byte>> action)
        {
            if (!_semaphoreSlim.Wait(0)) return false;

            var payload = this.GetPayload(action);
            if (payload.Count == 0)
            {
                _semaphoreSlim.Release();
                return false;
            }

            return _dataWriter.TryWrite(() => payload);
        }

        private ArraySegment<byte> GetPayload(Action<IBufferWriter<byte>> action)
        {
            using var bytesPipe = new BytesPipe(_bytesPool);
            action.Invoke(bytesPipe.Writer);
            if (bytesPipe.Writer.WrittenBytes == 0) return ArraySegment<byte>.Empty;

            var sequence = bytesPipe.Reader.GetSequence();
            var buffer = _bytesPool.Array.Rent((int)sequence.Length);
            sequence.CopyTo(buffer.AsSpan());

            return new ArraySegment<byte>(buffer, 0, (int)sequence.Length);
        }
    }
}