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
            private readonly AesGcmEncrypter _encrypter;
            private readonly CancellationToken _cancellationToken;

            private Exception? _exception = null;

            private readonly SemaphoreSlim _semaphoreSlim;
            private readonly ManualResetEventSlim _resetEvent;

            private long _sentByteCount;

            public ConnectionSender(IConnectionSender sender, CryptoAlgorithmType cryptoAlgorithmType, byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool, CancellationToken cancellationToken)
            {
                _sender = sender;

                if (cryptoAlgorithmType == CryptoAlgorithmType.Aes_Gcm_256)
                {
                    _encrypter = new AesGcmEncrypter(cryptoKey, nonce, bytesPool);
                }
                else
                {
                    throw new NotSupportedException(nameof(cryptoAlgorithmType));
                }

                _bytesPool = bytesPool;
                _cancellationToken = cancellationToken;

                _semaphoreSlim = new SemaphoreSlim(1, 1);
                _resetEvent = new ManualResetEventSlim();
            }

            protected override void OnDispose(bool disposing)
            {
                if (disposing)
                {
                    _semaphoreSlim.Dispose();
                    _resetEvent.Dispose();
                    _encrypter.Dispose();
                }
            }

            public long TotalBytesSent => Interlocked.Read(ref _sentByteCount);

            internal async ValueTask InternalWaitToSendAsync(CancellationToken cancellationToken = default)
            {
                try
                {
                    await _resetEvent.WaitHandle.WaitAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.Debug(e);
                }
            }

            internal void InternalSend()
            {
                if (_exception is not null) throw _exception;

                try
                {
                    if (!_resetEvent.IsSet) return;

                    _sender.TrySend(bufferWriter =>
                    {
                        if (!_encrypter.TryRead(bufferWriter, out var isCompleted)) return;

                        if (isCompleted)
                        {
                            _encrypter.Reset();
                            _resetEvent.Reset();

                            _semaphoreSlim.Release();
                        }
                    });
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
                var succeed = _encrypter.TryWrite(action);

                if (!succeed)
                {
                    _semaphoreSlim.Release();
                    return false;
                }

                _resetEvent.Set();
                return true;
            }
        }

        internal sealed class AesGcmEncrypter : DisposableBase
        {
            private readonly byte[] _cryptoKey;
            private readonly byte[] _nonce;
            private readonly IBytesPool _bytesPool;
            private readonly BytesPipe _bytesPipe;
            private readonly byte[] _payloadSizeBuffer = new byte[4];
            private ReadOnlySequence<byte> _sequence;

            public AesGcmEncrypter(byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool)
            {
                _cryptoKey = cryptoKey;
                _nonce = nonce;
                _bytesPool = bytesPool;
                _bytesPipe = new BytesPipe(_bytesPool);
            }

            protected override void OnDispose(bool disposing)
            {
                if (disposing)
                {
                    _bytesPipe.Dispose();
                }
            }

            public long RemainBytes => _sequence.Length;

            public void Reset()
            {
                _bytesPipe.Reset();
                BytesOperations.Zero(_payloadSizeBuffer);
                _sequence = ReadOnlySequence<byte>.Empty;
            }

            public bool TryWrite(Action<IBufferWriter<byte>> action)
            {
                action.Invoke(_bytesPipe.Writer);
                if (_bytesPipe.Writer.WrittenBytes == 0) return false;

                BinaryPrimitives.WriteInt32BigEndian(_payloadSizeBuffer, (int)_bytesPipe.Writer.WrittenBytes);

                var memories = new List<ReadOnlyMemory<byte>>();
                memories.Add(_payloadSizeBuffer);
                memories.AddRange(ReadOnlySequenceHelper.ToReadOnlyMemories(_bytesPipe.Reader.GetSequence()));
                _sequence = ReadOnlySequenceHelper.Create(memories.ToArray());

                return true;
            }

            public bool TryRead(IBufferWriter<byte> bufferWriter, out bool isCompleted)
            {
                isCompleted = false;

                var result = TryEncrypt(ref _sequence, bufferWriter, _cryptoKey, _nonce, _bytesPool);
                if (_sequence.Length == 0) isCompleted = true;

                return result;
            }

            private static bool TryEncrypt(ref ReadOnlySequence<byte> sequence, IBufferWriter<byte> bufferWriter, byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool)
            {
                if (sequence.Length == 0) return false;

                using var aes = new AesGcm(cryptoKey);

                Span<byte> tag = stackalloc byte[TagSize];
                using var inBuffer = bytesPool.Memory.Rent(FrameSize).Shrink(FrameSize);
                using var outBuffer = bytesPool.Memory.Rent(FrameSize).Shrink(FrameSize);

                int payloadLength = (int)Math.Min(sequence.Length, FrameSize);

                BytesOperations.Zero(inBuffer.Memory.Span);
                sequence.Slice(0, payloadLength).CopyTo(inBuffer.Memory.Span);
                sequence = sequence.Slice(payloadLength);

                aes.Encrypt(nonce, inBuffer.Memory.Span, outBuffer.Memory.Span, tag);
                Increment(nonce);

                bufferWriter.Write(outBuffer.Memory.Span);
                bufferWriter.Write(tag);

                return true;
            }
        }
    }
}
