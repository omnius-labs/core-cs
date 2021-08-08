using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Caps;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Bridge
{
    public partial class BridgeConnection
    {
        internal class ConnectionReceiver : DisposableBase, IConnectionReceiver
        {
            private readonly ICap _cap;
            private readonly int _maxReceiveByteCount;
            private readonly IBytesPool _bytesPool;
            private readonly CancellationTokenSource _cancellationTokenSource;

            private readonly byte[] _headerBuffer = new byte[4];
            private int _headerBufferPosition = 0;
            private int _remainBytes = -1;
            private readonly BytesPipe _bytesPipe;
            private bool _bytesPipeWriterIsCompleted = false;
            private Exception? _exception = null;

            private readonly SemaphoreSlim _semaphoreSlim;

            private long _receivedByteCount;

            private readonly object _lockObject = new();

            public ConnectionReceiver(ICap cap, int maxReceiveByteCount, IBytesPool bytesPool, CancellationTokenSource cancellationTokenSource)
            {
                _cap = cap;
                _maxReceiveByteCount = maxReceiveByteCount;
                _bytesPool = bytesPool;
                _cancellationTokenSource = cancellationTokenSource;
                _bytesPipe = new BytesPipe(_bytesPool);
                _semaphoreSlim = new SemaphoreSlim(0, 1);
            }

            protected override void OnDispose(bool disposing)
            {
                if (disposing)
                {
                    _bytesPipe?.Dispose();
                    _semaphoreSlim?.Dispose();
                }
            }

            public long TotalBytesReceived => Interlocked.Read(ref _receivedByteCount);

            internal int InternalReceive(int maxSize)
            {
                if (_exception is not null) return 0;

                try
                {
                    lock (_lockObject)
                    {
                        int total = 0;
                        int loopCount = 0;

                        while (total < maxSize)
                        {
                            if (_bytesPipeWriterIsCompleted) break;
                            if (++loopCount > 5) break;

                            if (_remainBytes == -1)
                            {
                                for (; ; )
                                {
                                    if (!_cap.CanReceive()) break;

                                    int receiveLength = _cap.Receive(_headerBuffer.AsSpan().Slice(_headerBufferPosition));
                                    if (receiveLength <= 0) break;

                                    _headerBufferPosition += receiveLength;

                                    if (_headerBufferPosition == 4)
                                    {
                                        var contentLength = BinaryPrimitives.ReadInt32BigEndian(_headerBuffer);
                                        if (contentLength > _maxReceiveByteCount) throw new ConnectionException("This message is too long");

                                        _remainBytes = contentLength;

                                        break;
                                    }
                                }
                            }

                            while (_remainBytes > 0)
                            {
                                if (!_cap.CanReceive()) break;

                                int receiveLength = _cap.Receive(_bytesPipe.Writer.GetSpan(_remainBytes).Slice(0, _remainBytes));
                                if (receiveLength <= 0) break;

                                _bytesPipe.Writer.Advance(receiveLength);
                                _remainBytes -= receiveLength;
                            }

                            if (_remainBytes == 0)
                            {
                                _headerBufferPosition = 0;
                                _remainBytes = -1;
                                _bytesPipeWriterIsCompleted = true;

                                _semaphoreSlim.Release();

                                break;
                            }
                        }

                        return total;
                    }
                }
                catch (ConnectionException e)
                {
                    _exception = e;
                    _cancellationTokenSource.Cancel();
                    return 0;
                }
                catch (Exception e)
                {
                    _exception = new ConnectionException("receive error", e);
                    _cancellationTokenSource.Cancel();
                    return 0;
                }
            }

            public async ValueTask WaitToReceiveAsync(CancellationToken cancellationToken = default)
            {
                await _semaphoreSlim.WaitAsync(cancellationToken);
                _semaphoreSlim.Release();
            }

            public bool TryReceive(Action<ReadOnlySequence<byte>> action)
            {
                try
                {
                    if (!_semaphoreSlim.Wait(0, _cancellationTokenSource.Token)) return false;
                }
                catch (OperationCanceledException)
                {
                    if (_exception is not null) throw _exception;
                    throw;
                }

                return this.TryReadBytes(action);
            }

            public async ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
            {
                try
                {
                    using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                    await _semaphoreSlim.WaitAsync(linkedTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    if (_exception is not null) throw _exception;
                    throw;
                }

                this.TryReadBytes(action);
            }

            private bool TryReadBytes(Action<ReadOnlySequence<byte>> action)
            {
                var sequence = _bytesPipe.Reader.GetSequence();
                if (sequence.Length == 0) return false;

                action.Invoke(sequence);

                _bytesPipe.Reset();
                _bytesPipeWriterIsCompleted = false;

                return true;
            }
        }
    }
}
