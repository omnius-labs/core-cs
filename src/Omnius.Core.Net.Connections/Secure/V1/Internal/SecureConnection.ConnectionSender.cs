using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Helpers;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public partial class SecureConnection
    {
        internal class ConnectionSender : DisposableBase, IConnectionSender
        {
            private readonly IConnectionSender _sender;
            private readonly IBytesPool _bytesPool;
            private readonly SessionState _sessionState;
            private readonly CancellationTokenSource _cancellationTokenSource;

            private readonly byte[] _headerBuffer = new byte[4];
            private readonly BytesPipe _bytesPipe;
            private int _remainBytes = 0;
            private Exception? _exception = null;

            private readonly SemaphoreSlim _semaphoreSlim;
            private readonly ManualResetEventSlim _resetEvent;

            private long _sentByteCount;

            private readonly object _lockObject = new();

            public ConnectionSender(IConnectionSender sender, IBytesPool bytesPool, SessionState sessionState, CancellationTokenSource cancellationTokenSource)
            {
                _sender = sender;
                _bytesPool = bytesPool;
                _sessionState = sessionState;
                _cancellationTokenSource = cancellationTokenSource;

                _bytesPipe = new BytesPipe(_bytesPool);
                _semaphoreSlim = new SemaphoreSlim(1, 1);
                _resetEvent = new ManualResetEventSlim();
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

            public long TotalBytesSent => Interlocked.Read(ref _sentByteCount);

            internal async ValueTask InternalWaitAsync(CancellationToken cancellationToken = default)
            {
                await _resetEvent.WaitHandle.WaitAsync(cancellationToken);
            }

            internal void InternalSend()
            {
                if (_exception is not null) return;

                try
                {
                    if (!_resetEvent.IsSet) return;

                    lock (_lockObject)
                    {
                        _sender.TrySend(bufferWriter =>
                        {
                            var sequence = this.GetSequence();
                            sequence = sequence.Slice(sequence.Length - _remainBytes, Math.Min(_remainBytes, FrameSize));
                            this.Encrypt(sequence, bufferWriter);
                            _remainBytes -= (int)sequence.Length;

                            if (_remainBytes == 0)
                            {
                                _bytesPipe.Reset();
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
                    _exception = new ConnectionException("send error", e);
                    _cancellationTokenSource.Cancel();
                    return;
                }
            }

            private ReadOnlySequence<byte> GetSequence()
            {
                var memories = new List<ReadOnlyMemory<byte>>();
                memories.Add(_headerBuffer);
                memories.AddRange(ReadOnlySequenceHelper.ToReadOnlyMemories(_bytesPipe.Reader.GetSequence()));
                return ReadOnlySequenceHelper.Create(memories.ToArray());
            }

            private void Encrypt(ReadOnlySequence<byte> sequence, IBufferWriter<byte> bufferWriter)
            {
                try
                {
                    if (_sessionState.CryptoAlgorithm.HasFlag(CryptoAlgorithm.Aes_Gcm_256))
                    {
                        using (var aes = new AesGcm(_sessionState.MyCryptoKey))
                        {
                            Span<byte> tag = stackalloc byte[16];
                            var inBuffer = _bytesPool.Array.Rent(FrameSize - tag.Length);
                            var outBuffer = _bytesPool.Array.Rent(FrameSize - tag.Length);

                            try
                            {
                                while (sequence.Length > 0)
                                {
                                    int contentLength = (int)Math.Min(sequence.Length, FrameSize - tag.Length);

                                    var plaintext = inBuffer.AsSpan(0, contentLength);
                                    var ciphertext = outBuffer.AsSpan(0, contentLength);

                                    sequence.Slice(0, contentLength).CopyTo(plaintext);
                                    sequence = sequence.Slice(contentLength);

                                    aes.Encrypt(_sessionState.MyNonce, plaintext, ciphertext, tag);
                                    Increment(_sessionState.MyNonce);

                                    bufferWriter.Write(ciphertext);
                                    bufferWriter.Write(tag);
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

                throw new OmniSecureConnectionException("Encrypt failed.");
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
                    if (!_semaphoreSlim.Wait(0, _cancellationTokenSource.Token)) return false;
                }
                catch (OperationCanceledException)
                {
                    if (_exception is not null) throw _exception;
                    throw;
                }

                this.WriteBytes(action);
                return true;
            }

            public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
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

                this.WriteBytes(action);
            }

            private void WriteBytes(Action<IBufferWriter<byte>> action)
            {
                action.Invoke(_bytesPipe.Writer);
                _remainBytes = 4 + (int)_bytesPipe.Writer.WrittenBytes;
                BinaryPrimitives.WriteInt32BigEndian(_headerBuffer, _remainBytes);
                _resetEvent.Set();
            }
        }
    }
}
