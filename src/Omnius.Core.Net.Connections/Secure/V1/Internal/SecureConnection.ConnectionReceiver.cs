using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Caps;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public partial class SecureConnection
    {
        internal class ConnectionReceiver : DisposableBase, IConnectionReceiver
        {
            private readonly IConnectionReceiver _receiver;
            private readonly int _maxReceiveByteCount;
            private readonly IBytesPool _bytesPool;
            private readonly SessionState _sessionState;
            private readonly CancellationTokenSource _cancellationTokenSource;

            private int _remainBytes = -1;
            private readonly BytesPipe _bytesPipe;
            private Exception? _exception = null;

            private readonly SemaphoreSlim _semaphoreSlim;
            private readonly ManualResetEventSlim _resetEvent;

            private long _receivedByteCount;

            private readonly object _lockObject = new();

            public ConnectionReceiver(IConnectionReceiver receiver, int maxReceiveByteCount, IBytesPool bytesPool, SessionState sessionState, CancellationTokenSource cancellationTokenSource)
            {
                _receiver = receiver;
                _maxReceiveByteCount = maxReceiveByteCount;
                _bytesPool = bytesPool;
                _sessionState = sessionState;
                _cancellationTokenSource = cancellationTokenSource;

                _bytesPipe = new BytesPipe(_bytesPool);
                _semaphoreSlim = new SemaphoreSlim(0, 1);
                _resetEvent = new ManualResetEventSlim(true);
            }

            protected override void OnDispose(bool disposing)
            {
                if (disposing)
                {
                    _bytesPipe.Dispose();
                    _semaphoreSlim.Dispose();
                    _resetEvent.Dispose();
                }
            }

            public long TotalBytesReceived => Interlocked.Read(ref _receivedByteCount);

            internal async ValueTask InternalWaitAsync(CancellationToken cancellationToken = default)
            {
                await _resetEvent.WaitHandle.WaitAsync(cancellationToken);
            }

            internal void InternalReceive()
            {
                if (_exception is not null) return;

                try
                {
                    if (!_resetEvent.IsSet) return;

                    lock (_lockObject)
                    {
                        _receiver.TryReceive(sequence =>
                        {
                            this.Decrypt(sequence, _bytesPipe.Writer);

                            if (_remainBytes == -1)
                            {
                                Span<byte> remainBytesBuffer = stackalloc byte[4];
                                _bytesPipe.Reader.GetSequence().CopyTo(remainBytesBuffer);
                                _bytesPipe.Reader.Advance(4);
                                _remainBytes = BinaryPrimitives.ReadInt32BigEndian(remainBytesBuffer);

                                if (_remainBytes > _maxReceiveByteCount) throw new ConnectionException("receive size over");
                            }

                            _remainBytes -= (int)sequence.Length;

                            if (_remainBytes == 0)
                            {
                                _remainBytes = -1;
                                _resetEvent.Reset();
                                _semaphoreSlim.Release();
                            }
                        });
                    }
                }
                catch (ConnectionException e)
                {
                    _exception = e;
                    _cancellationTokenSource.Cancel();
                    return;
                }
                catch (Exception e)
                {
                    _exception = new ConnectionException("receive error", e);
                    _cancellationTokenSource.Cancel();
                    return;
                }
            }

            private void Decrypt(ReadOnlySequence<byte> sequence, IBufferWriter<byte> bufferWriter)
            {
                try
                {
                    if (_sessionState.CryptoAlgorithm.HasFlag(CryptoAlgorithm.Aes_Gcm_256))
                    {
                        using (var aes = new AesGcm(_sessionState.OtherCryptoKey))
                        {
                            Span<byte> tag = stackalloc byte[16];
                            var inBuffer = _bytesPool.Array.Rent(FrameSize - tag.Length);
                            var outBuffer = _bytesPool.Array.Rent(FrameSize - tag.Length);

                            try
                            {
                                while (sequence.Length > 0)
                                {
                                    if (sequence.Length <= tag.Length) throw new FormatException();

                                    int contentLength = (int)Math.Min(sequence.Length, FrameSize) - tag.Length;

                                    var ciphertext = inBuffer.AsSpan(0, contentLength);
                                    var plaintext = outBuffer.AsSpan(0, contentLength);

                                    sequence.Slice(0, contentLength).CopyTo(ciphertext);
                                    sequence = sequence.Slice(contentLength);

                                    sequence.Slice(0, tag.Length).CopyTo(tag);
                                    sequence = sequence.Slice(tag.Length);

                                    aes.Decrypt(_sessionState.OtherNonce, ciphertext, tag, plaintext);
                                    Increment(_sessionState.OtherNonce);

                                    bufferWriter.Write(plaintext);
                                }
                            }
                            finally
                            {
                                _bytesPool.Array.Return(inBuffer);
                                _bytesPool.Array.Return(outBuffer);
                            }
                        }

                        return;
                    }
                }
                catch (OmniSecureConnectionException)
                {
                    throw;
                }

                throw new OmniSecureConnectionException("Decrypt failed.");
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

                this.ReadBytes(action);
                return true;
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

                this.ReadBytes(action);
            }

            private void ReadBytes(Action<ReadOnlySequence<byte>> action)
            {
                var sequence = _bytesPipe.Reader.GetSequence();
                action.Invoke(sequence);

                _bytesPipe.Reset();
                _resetEvent.Set();
            }
        }
    }
}
