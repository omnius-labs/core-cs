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
            private readonly AesGcmDecrypter _decrypter;
            private readonly CancellationTokenSource _cancellationTokenSource;

            private Exception? _exception = null;

            private readonly SemaphoreSlim _semaphoreSlim;
            private readonly ManualResetEventSlim _resetEvent;

            private long _receivedByteCount;

            public ConnectionReceiver(IConnectionReceiver receiver, int maxReceiveByteCount, byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool, CancellationTokenSource cancellationTokenSource)
            {
                _receiver = receiver;
                _maxReceiveByteCount = maxReceiveByteCount;
                _decrypter = new AesGcmDecrypter(cryptoKey, nonce, bytesPool);
                _bytesPool = bytesPool;
                _cancellationTokenSource = cancellationTokenSource;

                _semaphoreSlim = new SemaphoreSlim(0, 1);
                _resetEvent = new ManualResetEventSlim(true);
            }

            protected override void OnDispose(bool disposing)
            {
                if (disposing)
                {
                    _semaphoreSlim.Dispose();
                    _resetEvent.Dispose();
                    _decrypter.Dispose();
                }
            }

            public long TotalBytesReceived => Interlocked.Read(ref _receivedByteCount);

            internal async ValueTask InternalWaitToReceiveAsync(CancellationToken cancellationToken = default)
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

            internal void InternalReceive()
            {
                if (_exception is not null) return;

                try
                {
                    if (!_resetEvent.IsSet) return;

                    _receiver.TryReceive(sequence =>
                    {
                        if (!_decrypter.TryWrite(sequence, out var isCompleted)) return;
                        if (_decrypter.WrittenBytes > _maxReceiveByteCount) throw new OmniSecureConnectionException("Receive buffer overflow.");

                        if (isCompleted)
                        {
                            _resetEvent.Reset();
                            _semaphoreSlim.Release();
                        }
                    });
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
                _decrypter.Read(action);
                _decrypter.Reset();

                _resetEvent.Set();
            }
        }

        internal sealed class AesGcmDecrypter : DisposableBase
        {
            private readonly byte[] _cryptoKey;
            private readonly byte[] _nonce;
            private readonly IBytesPool _bytesPool;
            private readonly BytesPipe _bytesPipe;
            private int _payloadSize = -1;
            private ReadOnlySequence<byte> _sequence;

            public AesGcmDecrypter(byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool)
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

            public long WrittenBytes => _sequence.Length;

            public void Reset()
            {
                _bytesPipe.Reset();
                _payloadSize = -1;
                _sequence = ReadOnlySequence<byte>.Empty;
            }

            public bool TryWrite(ReadOnlySequence<byte> sequence, out bool isCompleted)
            {
                isCompleted = false;
                if (!TryDecrypt(sequence, _bytesPipe.Writer, _cryptoKey, _nonce, _bytesPool)) return false;

                if (_payloadSize == -1)
                {
                    Span<byte> remainBytesBuffer = stackalloc byte[4];
                    _bytesPipe.Reader.GetSequence().Slice(0, 4).CopyTo(remainBytesBuffer);
                    _bytesPipe.Reader.Advance(4);
                    _payloadSize = BinaryPrimitives.ReadInt32BigEndian(remainBytesBuffer);
                }

                if (_bytesPipe.Reader.RemainBytes >= _payloadSize)
                {
                    _sequence = _bytesPipe.Reader.GetSequence().Slice(0, _payloadSize);
                    isCompleted = true;
                }

                return true;
            }

            public void Read(Action<ReadOnlySequence<byte>> action)
            {
                action.Invoke(_sequence);
            }

            private static bool TryDecrypt(ReadOnlySequence<byte> sequence, IBufferWriter<byte> bufferWriter, byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool)
            {
                if (sequence.Length <= TagSize) return false;

                using var aes = new AesGcm(cryptoKey);

                Span<byte> tag = stackalloc byte[TagSize];
                using var inBuffer = bytesPool.Memory.Rent(FrameSize).Shrink(FrameSize);
                using var outBuffer = bytesPool.Memory.Rent(FrameSize).Shrink(FrameSize);

                sequence.Slice(0, FrameSize).CopyTo(inBuffer.Memory.Span);
                sequence = sequence.Slice(FrameSize);

                sequence.Slice(0, tag.Length).CopyTo(tag);
                sequence = sequence.Slice(tag.Length);

                aes.Decrypt(nonce, inBuffer.Memory.Span, tag, outBuffer.Memory.Span);
                Increment(nonce);

                bufferWriter.Write(outBuffer.Memory.Span);

                return true;
            }
        }
    }
}
