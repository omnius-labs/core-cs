using System.Buffers;
using System.Buffers.Binary;
using Core.Base;
using Core.Net.Caps;
using Core.Pipelines;

namespace Core.Net.Connections.Bridge;

public partial class BridgeConnection
{
    internal class ConnectionSender : DisposableBase, IConnectionSender
    {
        private readonly ICap _cap;
        private readonly IBytesPool _bytesPool;
        private readonly CancellationToken _cancellationToken;

        private readonly byte[] _headerBuffer = new byte[4];
        private int _headerBufferPosition = -1;
        private readonly BytesPipe _bytesPipe;
        private bool _bytesPipeWriterIsCompleted = false;
        private Exception? _exception = null;

        private readonly SemaphoreSlim _semaphoreSlim;

        private long _sentByteCount;

        private readonly object _lockObject = new();

        public ConnectionSender(ICap cap, IBytesPool bytesPool, CancellationToken cancellationToken)
        {
            _cap = cap;
            _bytesPool = bytesPool;
            _cancellationToken = cancellationToken;

            _bytesPipe = new BytesPipe(_bytesPool);
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _bytesPipe?.Dispose();
                _semaphoreSlim?.Dispose();
            }
        }

        public long TotalBytesSent => Interlocked.Read(ref _sentByteCount);

        internal int InternalSend(int maxSize)
        {
            if (_exception is not null) throw _exception;

            try
            {
                lock (_lockObject)
                {
                    if (!_cap.IsConnected) throw new ConnectionException("cap is not connected");

                    int total = 0;
                    int loopCount = 0;

                    while (total < maxSize)
                    {
                        if (_headerBufferPosition == -1 && !_bytesPipeWriterIsCompleted) break;
                        if (++loopCount > 5) break;

                        if (_headerBufferPosition < 4)
                        {
                            if (!_cap.CanSend()) break;

                            int sendLength = _cap.Send(_headerBuffer.AsSpan()[_headerBufferPosition..]);
                            if (sendLength <= 0) break;

                            _headerBufferPosition += sendLength;
                        }
                        else
                        {
                            if (_bytesPipe.Reader.RemainBytes > 0)
                            {
                                var sequence = _bytesPipe.Reader.GetSequence();
                                var position = sequence.Start;

                                while (total < maxSize && sequence.Length > 0 && sequence.TryGet(ref position, out var memory, false))
                                {
                                    if (!_cap.CanSend()) break;

                                    int readLength = Math.Min(maxSize - total, memory.Length);

                                    int sendLength = _cap.Send(memory.Span[..readLength]);
                                    if (sendLength <= 0) break;

                                    position = sequence.GetPosition(sendLength, position);

                                    total += sendLength;
                                    Interlocked.Add(ref _sentByteCount, sendLength);

                                    _bytesPipe.Reader.Advance(sendLength);
                                }
                            }

                            if (_bytesPipe.Reader.RemainBytes == 0)
                            {
                                _headerBufferPosition = -1;
                                _bytesPipe.Reset();
                                _bytesPipeWriterIsCompleted = false;

                                _semaphoreSlim.Release();

                                break;
                            }
                        }
                    }

                    return total;
                }
            }
            catch (ConnectionException e)
            {
                _exception = e;
                throw;
            }
            catch (Exception e)
            {
                _exception = new ConnectionException("send error", e);
                throw;
            }
        }

        public async ValueTask WaitToSendAsync(CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            _semaphoreSlim.Release();
        }

        public bool TrySend(Action<IBufferWriter<byte>> action)
        {
            try
            {
                if (!_semaphoreSlim.Wait(0, _cancellationToken)) return false;
            }
            catch (OperationCanceledException)
            {
                if (_exception is not null) throw _exception;
                throw;
            }

            return this.TryWriteBytes(action);
        }

        public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            try
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);
                await _semaphoreSlim.WaitAsync(linkedTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                if (_exception is not null) throw _exception;
                throw;
            }

            this.TryWriteBytes(action);
        }

        private bool TryWriteBytes(Action<IBufferWriter<byte>> action)
        {
            action.Invoke(_bytesPipe.Writer);

            if (_bytesPipe.Writer.WrittenBytes == 0)
            {
                _semaphoreSlim.Release();
                return false;
            }

            BinaryPrimitives.WriteInt32BigEndian(_headerBuffer, (int)_bytesPipe.Writer.WrittenBytes);
            _headerBufferPosition = 0;
            _bytesPipeWriterIsCompleted = true;

            return true;
        }
    }
}
