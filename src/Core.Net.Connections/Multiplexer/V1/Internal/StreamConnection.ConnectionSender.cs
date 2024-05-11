using System.Buffers;
using Core.Base;
using Core.Base.Helpers;
using Core.Pipelines;

namespace Core.Net.Connections.Multiplexer.V1.Internal;

internal partial class StreamConnection
{
    public sealed class ConnectionSender : DisposableBase, IConnectionSender
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IMessagePipeWriter<ArraySegment<byte>> _dataWriter;
        private readonly IActionListener _dataAcceptedListener;
        private readonly IDisposable _dataAcceptedListenerRegister;
        private readonly IBytesPool _bytesPool;
        private readonly CancellationToken _cancellationToken;

        private long _totalBytesSent;

        public ConnectionSender(int maxDataQueueSize, IMessagePipeWriter<ArraySegment<byte>> dataWriter, IActionListener dataAcceptedListener, IBytesPool bytesPool, CancellationToken cancellationToken)
        {
            _semaphoreSlim = new SemaphoreSlim(maxDataQueueSize, maxDataQueueSize);
            _dataWriter = dataWriter;
            _dataAcceptedListener = dataAcceptedListener;
            _dataAcceptedListenerRegister = _dataAcceptedListener.Listen(() => ExceptionHelper.TryCatch<ObjectDisposedException>(() => _semaphoreSlim.Release()));
            _bytesPool = bytesPool;
            _cancellationToken = cancellationToken;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _dataAcceptedListenerRegister.Dispose();
            }
        }

        public long TotalBytesSent => _totalBytesSent;

        public async ValueTask WaitToSendAsync(CancellationToken cancellationToken)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

            await _semaphoreSlim.WaitAsync(linkedTokenSource.Token);
            _semaphoreSlim.Release();
        }

        public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);

            await _semaphoreSlim.WaitAsync(linkedTokenSource.Token);
            this.InternalSend(action);
        }

        public bool TrySend(Action<IBufferWriter<byte>> action)
        {
            if (!_semaphoreSlim.Wait(0)) return false;
            this.InternalSend(action);
            return true;
        }

        private void InternalSend(Action<IBufferWriter<byte>> action)
        {
            using var bytesPipe = new BytesPipe(_bytesPool);
            action.Invoke(bytesPipe.Writer);
            if (bytesPipe.Writer.WrittenBytes == 0) return;

            var sequence = bytesPipe.Reader.GetSequence();
            var buffer = _bytesPool.Array.Rent((int)sequence.Length);
            sequence.CopyTo(buffer.AsSpan());

            _dataWriter.TryWrite(() => new ArraySegment<byte>(buffer, 0, (int)sequence.Length));
            Interlocked.Add(ref _totalBytesSent, sequence.Length);
        }
    }
}
